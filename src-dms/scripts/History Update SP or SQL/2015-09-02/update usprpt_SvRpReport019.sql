-- =============================================  
-- Author:  <Author,,Name>  
-- Create date: <Create Date,,>  
-- Description: <WEEKLY SALES REVIEW SA>  
-- =============================================  
alter procedure [dbo].[usprpt_SvRpReport019]    
  @CompanyCode varchar(15)    
 ,@BranchCode varchar(15)    
 ,@EmployeeID varchar(15)  
 ,@Month int  
 ,@Year int  
 ,@ProductType varchar(2)  
AS    
BEGIN    
  
-- ==============================================================================================================================  
-- TABLE MASTER  
-- ==============================================================================================================================  
  
select * into #TempEmployee FROM (  
SELECT employeeID   
FROM gnMstEmployee  
WHERE   
1 = 1  
AND CompanyCode = @CompanyCode   
AND BranchCode = @BranchCode   
AND TitleCode IN ('3', '7')  
AND EmployeeID = (CASE WHEN @EmployeeID = '' THEN EmployeeID ELSE @EmployeeID END)  
)#TempEmployee  
  
select * into #TempSPKNo FROM (  
SELECT JobOrderNo  
FROM svTrnService  
WHERE   
1 = 1  
AND CompanyCode = @CompanyCode   
AND BranchCode = @BranchCode   
AND ForemanID IN (SELECT EmployeeID FROM #TempEmployee)  
) #TempSPKNo  
  
CREATE TABLE #JobGroup  
(   
 [JobType] varchar(15),  
 [OM]  INT  
)  
  
  
CREATE TABLE #JobGroupOthers  
(   
 [JobType] varchar(15),  
 [GroupJobType] varchar(15)  
)  
  
SELECT * INTO #1 FROM   
(  
 SELECT DISTINCT JobType, REPLACE(SUBSTRING(JobType,3,100),'KM','') OM FROM SvMstJob   
 WHERE CompanyCode = '6092401' and ProductType = '4W' and JobType LIKE 'PB%'  AND GroupJobType = 'RTN'  
) #1   
  
DECLARE @JobTypeGroup VARCHAR(MAX)  
  
DECLARE db_cursorGroup CURSOR FOR  
SELECT JobType FROM #1  
  
  
OPEN db_cursorGroup  
FETCH NEXT FROM db_cursorGroup INTO @JobTypeGroup  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
 BEGIN TRY  
  INSERT INTO #JobGroup  SELECT JobType, CAST(OM AS INT) OM FROM #1 WHERE JobType = @JobTypeGroup  
 END TRY  
 BEGIN CATCH  
  INSERT INTO #JobGroupOthers  VALUES (@JobTypeGroup, '')  
 END CATCH   
 FETCH NEXT FROM db_cursorGroup INTO @JobTypeGroup  
END  
  
INSERT INTO #JobGroupOthers    
SELECT DISTINCT JobType , GroupJobType  
FROM SvMstJob   
WHERE CompanyCode = @CompanyCode   
 and ProductType = @ProductType   
 and (JobType NOT IN ('CLAIM', 'FSC01', 'REWORK', 'PDI')   
   AND GroupJobType NOT IN ('RTN', 'CLM', 'FSC'))  
  
UPDATE #JobGroupOthers   
SET GroupJobType = b.GroupJobType  
FROM #JobGroupOthers a, (SELECT DISTINCT JoBType, GroupJobType FROM svMstJob WHERE CompanyCode = '6092401' and ProductType = '4W') b  
WHERE a.JobType = b.JobType  
 AND a.GroupJobType = ''  
  
CLOSE db_cursorGroup  
DEALLOCATE db_cursorGroup  
  
DECLARE @JobType VARCHAR(15)  
DECLARE @OdoMeter INT  
  
CREATE TABLE #Job1 ( -- Above 30.000  
 [JobType] varchar(15)   
)  
CREATE TABLE #Job2 ( -- multiply 5000 KM  
 [JobType] varchar(15)  
)  
  
  
DECLARE db_cursor CURSOR FOR  
SELECT JobType, OM FROM #JobGroup  
  
OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO @JobType, @OdoMeter  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
 IF (@OdoMeter >= 30000) AND (@Odometer % 10000 = 0)  
 BEGIN  
  INSERT INTO #Job1 VALUES (@JobType)  
 END  
 ELSE  
 IF (@OdoMeter >=5000) AND (@Odometer % 5000 = 0)   
 BEGIN    
  INSERT INTO #Job2 VALUES (@JobType)  
 END  
  
 FETCH NEXT FROM db_cursor INTO @JobType, @OdoMeter  
END  
  
CLOSE db_cursor  
DEALLOCATE db_cursor   
  
create table #t_WeekReview(  
  CompanyCode varchar(15)  
 , BranchCode varchar(15)  
 , EmployeeID varchar(15)  
 , EmployeeName varchar(50)  
 , Description varchar(max)  
 , SeqNo   int  
 , UOM   varchar(max)  
 , Target  decimal  
 , Week1 decimal , Week2 decimal, Week3 decimal  
 , Week4 decimal , Week5 decimal, Week6 decimal
)  
  
DECLARE @MinusWeek  INT  
DECLARE @LastDate  DATETIME  
DECLARE @CurrentDate DATETIME  
SET @LastDate = ISNULL((SELECT DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,CONVERT(VARCHAR, @Year, 1) + '-' + CONVERT(VARCHAR, @Month, 1) +'-01'),0))),'')  
SET @CurrentDate = CONVERT(VARCHAR, @Year,1) +'-'+ CONVERT(VARCHAR, @Month, 1) +'-01'  
SET @MinusWeek = ISNULL((CASE WHEN @Month = 1 THEN 0   
     ELSE   
      CASE WHEN ISNULL((SELECT DATEPART(WEEK, @LastDate)),0) =  ISNULL((SELECT DATEPART(WEEK, @CurrentDate)),0)  
      THEN ISNULL((SELECT DATEPART(WEEK, @LastDate)) - 1,0) ELSE ISNULL((SELECT DATEPART(WEEK, @LastDate)),0)  
     END END),0)  
  
-- ==============================================================================================================================  
-- TOTAL SALES REVENEU  
-- =============================================================================================================================  
  
select * into #TempSalesReveneuPvt from (  
  select *  
        from (  
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    ,  SUM(TotalDPPAmt) TotalSalesReveneu  
   from svTrnInvoice inv  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo  
  ) as Header  
  pivot(  
   sum(TotalSalesReveneu)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempSalesReveneuPvt  
  
select * into #TempSalesReveneu from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, 'Total Sales Revenue' Description, 1 SeqNo, 'Rp.' UOM  
  , (  
    select sum(ISNULL(TargetLaborSalesReveneu,0) + ISNULL(TargetPartSalesReveneu,0) + ISNULL(TargetLubricantSalesReveneu,0)) from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempSalesReveneuPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1 = 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempSalesReveneu  
-- ==============================================================================================================================  
-- TOTAL LABOR SALES REVENEU   
-- ==============================================================================================================================  
  
select * into #TempLaborSalesReveneuPvt from (  
  select *  
        from (  
     
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    ,  SUM(LaborDPPAmt) TotalLaborSalesReveneu  
   from svTrnInvoice inv  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo  
  ) as Header  
  pivot(  
   sum(TotalLaborSalesReveneu)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempLaborSalesReveneu  
  
  
select * into #TempLaborSalesReveneu from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Total Labor Sales Revenue' Description, 2 SeqNo, 'Rp.' UOM  
  , (  
    select sum(ISNULL(TargetLaborSalesReveneu,0)) from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempLaborSalesReveneuPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1 = 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempLaborSalesReveneu  
  
-- ==============================================================================================================================  
-- TOTAL PART SALES REVENEU (SPAREPART)  
-- ==============================================================================================================================  
select * into #TempSalesSparepartReveneuPvt from (  
  select *  
        from (  
     
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    ,  SUM(PartsDppAmt) TotalSalesSparepartReveneu  
   from svTrnInvoice inv  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo  
  ) as Header  
  pivot(  
   sum(TotalSalesSparepartReveneu)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempSalesSparepartReveneuPvt  
  
select * into #TempSalesSparepartReveneu from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Total Parts Sales Revenue' Description, 3 SeqNo, 'Rp.' UOM  
  , (  
     select sum(ISNULL(TargetPartSalesReveneu,0)) from svMstTargetSA  
      where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
      and ProductType = @ProductType  
  ) Target    
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempSalesSparepartReveneuPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1 = 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempSalesSparepartReveneu  
  
  
-- ==============================================================================================================================  
-- TOTAL OIL & MATERIAL SALES REVENEU  
-- ==============================================================================================================================  
select * into #TempSalesOilMaterialReveneuPvt from (  
  select *  
        from (  
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    ,  SUM(MaterialDPPAmt) TotalSalesOilMaterialReveneu  
   from svTrnInvoice inv  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo  
  ) as Header  
  pivot(  
   sum(TotalSalesOilMaterialReveneu)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempSalesOilMaterialReveneuPvt  
  
select * into #TempSalesOilMaterialReveneu from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Total Lubricant & Sublet Sales revenue' Description, 4 SeqNo, 'Rp.' UOM  
  , (  
    select sum(ISNULL(TargetLubricantSalesReveneu,0)) from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempSalesOilMaterialReveneuPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1 = 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempSalesOilMaterialReveneu  
  
-- ==============================================================================================================================  
-- ==============================================================================================================================  
  
-- ==============================================================================================================================  
-- TOTAL UNIT (WORK ORDER)  
-- ==============================================================================================================================  
  
select * into #TempTotalUnitPvt from (  
  select * from (  
  select   
   inv.CompanyCode  
   , inv.BranchCode  
   , inv.EmployeeID  
   , inv.InvoiceWeek  
   , isnull(count(BasicModel),0) TotalUnit  
        from   
   (  
    select distinct(inv.BasicModel), inv.ChassisCode, inv.ChassisNo, inv.CompanyCode, inv.BranchCode  
     , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
      and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
     , day(JobOrderDate) day  
     , month(jobOrderDate) month  
     , 'W' + substring(Convert(varchar, DATEPART(WEEK, inv.InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek  
    from svTrnInvoice inv  
    LEFT JOIN omMstVehicle a ON a.CompanyCode = inv.CompanyCode   
     AND a.ChassisCode = inv.ChassisCode  
     AND a.ChassisNo = inv.ChassisNo  
    left join svMstRefferenceService reff on  
     reff.CompanyCode = inv.CompanyCode  
     AND reff.ProductType = inv.ProductType  
     AND reff.RefferenceCode = inv.BasicModel  
    where   
     1 = 1  
     and inv.CompanyCode = @CompanyCode   
     and inv.BranchCode = @BranchCode  
     and inv.ProductType = @ProductType  
     and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
     and Year(inv.InvoiceDate) = @Year  
     and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   ) inv  
  group by inv.CompanyCode,inv.BranchCode, inv.InvoiceWeek, inv.EmployeeID  
  )  
  as Header  
  pivot(  
   sum(TotalUnit)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempTotalUnitPvt  
  
select * into #TempTotalUnit from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, 'Total Unit (Work Order)' Description, 5 SeqNo, 'Unit' UOM  
  , (  
    select sum(ISNULL(TargetUnitPassenger,0) + ISNULL(TargetUnitCommercial,0)) from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
  from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempTotalUnitPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1 = 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempTotalUnit  
  
-- ==============================================================================================================================  
-- TOTAL UNIT (PASSENGER)  
-- ==============================================================================================================================  
  
select * into #TempTotalPassengerUnitPvt from (  
  select * from (  
  select   
   inv.CompanyCode  
   , inv.BranchCode  
   , inv.EmployeeID  
   , inv.InvoiceWeek  
   , isnull(count(BasicModel),0) TotalUnitPassenger  
        from (  
    select distinct(inv.BasicModel), inv.ChassisCode, inv.ChassisNo, inv.CompanyCode, inv.BranchCode  
     , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
      and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
     , day(JobOrderDate) day  
     , month(jobOrderDate) month  
     , 'W' + substring(Convert(varchar, DATEPART(WEEK, inv.InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek  
    from svTrnInvoice inv  
    LEFT JOIN omMstVehicle a ON a.CompanyCode = inv.CompanyCode   
     AND a.ChassisCode = inv.ChassisCode  
     AND a.ChassisNo = inv.ChassisNo  
    left join svMstRefferenceService reff on  
     reff.CompanyCode = inv.CompanyCode  
     AND reff.ProductType = inv.ProductType  
     AND reff.RefferenceCode = inv.BasicModel  
    where   
     1 = 1  
     AND inv.CompanyCode = @CompanyCode   
     AND inv.BranchCode = @BranchCode  
     AND inv.ProductType = @ProductType  
     AND Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
     AND Year(inv.InvoiceDate) = @Year  
     AND inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
     AND (reff.isLocked IS NULL OR reff.isLocked = 0)  
       
   )inv  
   group by inv.CompanyCode,inv.BranchCode, inv.InvoiceWeek, inv.EmployeeID  
  ) as Header  
  pivot(  
   sum(TotalUnitPassenger)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempTotalUnitPvt  
  
select * into #TempTotalPassengerUnit from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Passenger' Description, 6 SeqNo, 'Unit' UOM  
  , (  
    select sum(ISNULL(TargetUnitPassenger,0)) from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempTotalPassengerUnitPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1 = 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempTotalPassengerUnit  
  
  
-- ==============================================================================================================================  
-- TOTAL UNIT (COMMERCIAL)  
-- ==============================================================================================================================  
  
select * into #TempTotalCommercialUnitPvt from (  
  select * from (  
  select   
   inv.CompanyCode  
   , inv.BranchCode  
   , inv.EmployeeID  
   , inv.InvoiceWeek  
   , isnull(count(BasicModel),0) TotalUnitCommecial  
        from (  
    select distinct(inv.BasicModel), inv.ChassisCode, inv.ChassisNo, inv.CompanyCode, inv.BranchCode  
     , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
      and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
     , day(JobOrderDate) day  
     , month(jobOrderDate) month  
     , 'W' + substring(Convert(varchar, DATEPART(WEEK, inv.InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek  
    from svTrnInvoice inv  
    LEFT JOIN omMstVehicle a ON a.CompanyCode = inv.CompanyCode   
     AND a.ChassisCode = inv.ChassisCode  
     AND a.ChassisNo = inv.ChassisNo  
    left join svMstRefferenceService reff on  
     reff.CompanyCode = inv.CompanyCode  
     AND reff.ProductType = inv.ProductType  
     AND reff.RefferenceCode = inv.BasicModel  
    where   
     1 = 1  
     and inv.CompanyCode = @CompanyCode   
     and inv.BranchCode = @BranchCode  
     and inv.ProductType = @ProductType  
     and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
     and Year(inv.InvoiceDate) = @Year  
     and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)   
     AND reff.isLocked = 1      
   )inv  
   group by inv.CompanyCode,inv.BranchCode, inv.InvoiceWeek, inv.EmployeeID  
  ) as Header  
  pivot(  
   sum(TotalUnitCommecial)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempTotalCommercialUnitPvt  
  
select * into #TempTotalCommercialUnit from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Commercial' Description, 7 SeqNo, 'Unit' UOM  
  , (  
    select sum(ISNULL(TargetUnitCommercial,0)) from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempTotalCommercialUnitPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1 = 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempTotalCommercialUnit  
  
  
-- ==============================================================================================================================  
-- ==============================================================================================================================  
  
-- ==============================================================================================================================  
-- TOTAL JOBS CPUS  
-- ==============================================================================================================================  
select * into #TempJobsCPUSPvt from (  
  select *  
        from (  
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , ISNULL((  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE   
      1 = 1  
      and temp.CompanyCode = inv.CompanyCode   
      and temp.BranchCode = inv.BranchCode   
      and temp.InvoiceNo = inv.InvoiceNo  
      and temp.ProductType = @ProductType  
      ),0) TotalJobCPUS  
   from svTrnInvoice inv  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and (  
      inv.JobType IN (SELECT JobType FROM #Job1)   
      OR inv.JobType IN (SELECT JobType FROM #Job2)   
      OR inv.JobType IN (SELECT JobType FROM #JobGroupOthers)   
      OR inv.JobType IN ('OTHERS')  
     )  
    and inv.JobType NOT IN ('CLAIM', 'FSC01', 'REWORK','PDI')  
    and substring(inv.InvoiceNo, 1,3) <> 'INI'  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobCPUS)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsCPUSPvt  
  
select * into #TempJobsCPUS from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, 'Total Jobs CPUS' Description, 8 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetCPUS30000,0) + ISNULL(TargetCPUS10000,0) + ISNULL(TargetCPUS5000,0) +  
      ISNULL(TargetCPUSOil,0) + ISNULL(TargetCPUSOverhaulRepair,0) + ISNULL(TargetCPUSTyre,0) +  
      ISNULL(TargetCPUSAdjustment,0))   
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsCPUSPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1 = 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsCPUS  
  
-- ==============================================================================================================================  
-- TOTAL JOBS CPUS - ABOVE 30.000 KM  
-- ==============================================================================================================================  
select * into #TempJobsCPUS30000Pvt from (  
  select *  
        from (  
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , ISNULL((  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE   
      1 = 1  
      and temp.CompanyCode = inv.CompanyCode   
      and temp.BranchCode = inv.BranchCode   
      and temp.InvoiceNo = inv.InvoiceNo  
      and temp.ProductType = @ProductType  
      ),0) TotalJobCPUS10000  
   from svTrnInvoice inv  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobType in (select JobType from #Job1)  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobCPUS10000)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsCPUS30000Pvt  
  
select * into #TempJobsCPUS30000 from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Above 30.000 km jobs(10.000 km multiplier)' Description, 9 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetCPUS30000,0))   
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsCPUS30000Pvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1 = 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsCPUS30000  
  
-- ==============================================================================================================================  
-- TOTAL JOBS CPUS - 10.000 - 20.000 KM  
-- ==============================================================================================================================  
select * into #TempJobsCPUS10000Pvt from  
(  
 select * from (  
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , ISNULL((  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE   
      1 = 1  
      and temp.CompanyCode = inv.CompanyCode   
      and temp.BranchCode = inv.BranchCode   
      and temp.InvoiceNo = inv.InvoiceNo  
      and temp.ProductType = @ProductType  
      ),0) TotalJobCPUSFS  
   from svTrnInvoice inv  
   inner join svMstJob job on job.CompanyCode = inv.CompanyCode AND job.basicModel = inv.BasicModel AND job.JobType = inv.JobType AND job.GroupJobType = 'RTN'  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobType IN ('PB10000', 'PB20000')  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
   ) as Header  
   pivot(  
    sum(TotalJobCPUSFS)  
    for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
   ) pvt  
)#TempJobsCPUS10000Pvt  
  
select * into #TempJobsCPUS10000 from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   10.000 & 20.000 km (FS2 & FS3)' Description, 10 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetCPUS10000,0)) from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsCPUS10000Pvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1 = 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsCPUS10000  
  
-- ==============================================================================================================================  
-- TOTAL JOBS CPUS - 5.000 KM ABOVE  
-- ==============================================================================================================================  
select * into #TempJobsCPUS5000Pvt from (  
  select *  
        from (  
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , ISNULL((  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE   
      1 = 1  
      and temp.CompanyCode = inv.CompanyCode   
      and temp.BranchCode = inv.BranchCode   
      and temp.InvoiceNo = inv.InvoiceNo  
      and temp.ProductType = @ProductType  
      ),0) TotalJobCPUS5000  
   from svTrnInvoice inv  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobType IN (select JobType from #Job2)  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobCPUS5000)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsCPUS5000Pvt  
  
select * into #TempJobsCPUS5000 from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   5.000 km multiplier & above' Description, 11 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetCPUS5000,0))  
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsCPUS5000Pvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1= 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsCPUS5000  
  
  
-- ==============================================================================================================================  
-- TOTAL JOBS CPUS - Others - Oil Change Only  
-- ==============================================================================================================================  
select * into #TempJobsCPUSOilPvt from (  
  select *  
        from (  
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , ISNULL((  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE   
      1 = 1  
      and temp.CompanyCode = inv.CompanyCode   
      and temp.BranchCode = inv.BranchCode   
      and temp.InvoiceNo = inv.InvoiceNo  
      and temp.ProductType = @ProductType  
      and temp.OperationNo IN (select RefferenceCode from svmstRefferenceService where RefferenceType = 'CPUS-001')  
      ),0) TotalJobCPUSOil  
   from svTrnInvoice inv  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobType IN ('OTHERS')  
    and substring(inv.InvoiceNo, 1,3) <> 'INI'  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobCPUSOil)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsCPUSOilPvt  
  
select * into #TempJobsCPUSOil from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Others - Oil change only' Description, 12 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetCPUSOil,0))  
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsCPUSOilPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1= 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsCPUSOil  
  
-- ==============================================================================================================================  
-- TOTAL JOBS CPUS - Others - Overhaul & Repairs  
-- ==============================================================================================================================  
select * into #TempJobsCPUSoverhaulRepairPvt from (  
  select *  
        from (  
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , ISNULL((  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE   
      1 = 1  
      and temp.CompanyCode = inv.CompanyCode   
      and temp.BranchCode = inv.BranchCode   
      and temp.InvoiceNo = inv.InvoiceNo  
      and temp.ProductType = @ProductType  
      and temp.OperationNo IN (select RefferenceCode from svmstRefferenceService where RefferenceType = 'CPUS-002')  
      ),0) TotalJobCPUSOverRepair  
   from svTrnInvoice inv  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobType IN ('OTHERS')  
    and substring(inv.InvoiceNo, 1,3) <> 'INI'  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobCPUSOverRepair)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsCPUSoverhaulRepairPvt  
  
select * into #TempJobsCPUSoverhaulRepair from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Others - Overhaul & Repairs' Description, 13 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetCPUSOverhaulRepair,0))  
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsCPUSoverhaulRepairPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1= 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsCPUSoverhaulRepair  
  
-- ==============================================================================================================================  
-- TOTAL JOBS CPUS - Others - Tyre service (Wheel alignment & Balance)  
-- ==============================================================================================================================  
select * into #TempJobsCPUSTyrePvt from (  
  select *  
        from (  
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , ISNULL((  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE   
      1 = 1  
      and temp.CompanyCode = inv.CompanyCode   
      and temp.BranchCode = inv.BranchCode   
      and temp.InvoiceNo = inv.InvoiceNo  
      and temp.ProductType = @ProductType  
      and temp.OperationNo IN (select RefferenceCode from svmstRefferenceService where RefferenceType = 'CPUS-003')  
      ),0) TotalJobCPUSTyre  
   from svTrnInvoice inv  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobType IN ('OTHERS')  
    and substring(inv.InvoiceNo, 1,3) <> 'INI'  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobCPUSTyre)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsCPUSTyrePvt  
  
select * into #TempJobsCPUSTyre from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Others Tyre Service(Wheel Alignment & Balance' Description, 14 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetCPUSTyre,0))  
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsCPUSTyrePvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1= 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsCPUSTyre  
  
-- ==============================================================================================================================  
-- TOTAL JOBS CPUS - Others - adjustment & unclassified  
-- ==============================================================================================================================  
select * into #TempJobsCPUSAdjustmentPvt from (  
  select *  
        from (  
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , ISNULL((  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE   
      1 = 1  
      and temp.CompanyCode = inv.CompanyCode   
      and temp.BranchCode = inv.BranchCode   
      and temp.InvoiceNo = inv.InvoiceNo  
      and temp.ProductType = @ProductType  
      and temp.OperationNo NOT IN (select RefferenceCode from svmstRefferenceService where RefferenceType IN ('CPUS-001','CPUS-002','CPUS-003'))  
      ),0) TotalJobCPUSAdjustment  
   from svTrnInvoice inv  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobType IN (select JobType from #JobGroupOthers WHERE GroupJobType <> 'BDR')  
    and substring(inv.InvoiceNo, 1,3) <> 'INI'  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobCPUSAdjustment)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsCPUSAdjustmentPvt  
  
select * into #TempJobsCPUSAdjustment from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Others - Adjustment & Unclassified' Description, 15 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetCPUSAdjustment,0))   
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsCPUSAdjustmentPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1= 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsCPUSAdjustment  
  
  
-- ==============================================================================================================================  
-- BODY REPAIR  
-- ==============================================================================================================================  
select * into #TempJobsCPUSBodyRepairPvt from (  
  select *  
        from (  
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , ISNULL((  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE   
      1 = 1  
      and temp.CompanyCode = inv.CompanyCode   
      and temp.BranchCode = inv.BranchCode   
      and temp.InvoiceNo = inv.InvoiceNo  
      and temp.ProductType = @ProductType  
      ),0) TotalJobCPUS5000  
   from svTrnInvoice inv  
   where   
    1 = 1  
    and inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and inv.ProductType = @ProductType  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobType IN (select JobType from #JobGroupOthers WHERE GroupJobType = 'BDR')  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
    and substring(inv.InvoiceNo, 1,3) <> 'INI'  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobCPUS5000)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsCPUSBodyRepairPvt  
  
select * into #TempJobsCPUSBodyRepair from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Body Repair' Description, 16 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetCPUS5000,0))  
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsCPUSBodyRepairPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE 1= 1 and a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsCPUSBodyRepair  
  
  
  
-- ==============================================================================================================================  
-- ==============================================================================================================================  
  
-- ==============================================================================================================================  
-- TOTAL JOBS NON CPUS  
-- ==============================================================================================================================  
select * into #TempJobsNonCPUSPvt from (  
  select *  
        from (  
     
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , (  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE temp.InvoiceNo = inv.InvoiceNo  
      ) TotalJobNonCPUS  
   from svTrnInvoice inv  
   where inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and (substring(inv.InvoiceNo, 1,3) = 'INI' OR inv.JobType IN ('CLAIM', 'FSC01', 'REWORK'))  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobNonCPUS)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsNonCPUSPvt  
  
select * into #TempJobsNonCPUS from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, 'Total Jobs Non-CPUS' Description, 17 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetNonCPUSWarranty,0) + ISNULL(TargetNonCPUSFS,0) + ISNULL(TargetNonCPUSRework,0) +  
      ISNULL(TargetNonCPUSinService,0))   
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsNonCPUSPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsNonCPUS  
-- ==============================================================================================================================  
-- TOTAL JOBS NON CPUS (Warranty)  
-- ==============================================================================================================================  
select * into #TempJobsNonCPUSClaimPvt from (  
  select *  
        from (  
     
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , (  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE temp.InvoiceNo = inv.InvoiceNo  
      ) TotalJobNonClaimCPUS  
   from svTrnInvoice inv  
   where inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobType IN ('CLAIM')  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobNonClaimCPUS)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsNonCPUSClaimPvt  
  
select * into #TempJobsNonCPUSClaim from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Warranty' Description, 18 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetNonCPUSWarranty,0))   
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsNonCPUSClaimPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsNonCPUSClaim  
  
-- ==============================================================================================================================  
-- TOTAL JOBS NON CPUS (Free Service)  
-- ==============================================================================================================================  
select * into #TempJobsNonCPUSFSPvt from (  
  select *  
        from (  
     
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , (  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE temp.InvoiceNo = inv.InvoiceNo  
      ) TotalJobNonCPUSFS  
   from svTrnInvoice inv  
   where inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobType IN ('FSC01')  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobNonCPUSFS)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsNonCPUSFSPvt  
  
select * into #TempJobsNonCPUSFS from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Free Service 1' Description, 19 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetNonCPUSFS,0))  
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsNonCPUSFSPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsNonCPUSFS  
  
-- ==============================================================================================================================  
-- TOTAL JOBS NON CPUS (Rework)  
-- ==============================================================================================================================  
select * into #TempJobsNonCPUSReWorkPvt from (  
  select *  
        from (  
     
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , (  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE temp.InvoiceNo = inv.InvoiceNo  
      ) TotalJobNonCPUSReWork  
   from svTrnInvoice inv  
   where inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and inv.JobType IN ('REWORK')  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobNonCPUSReWork)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsNonCPUSReWorkPvt  
  
select * into #TempJobsNonCPUSReWork from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Rework' Description, 20 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetNonCPUSRework,0))        
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsNonCPUSReWorkPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsNonCPUSReWork  
  
-- ==============================================================================================================================  
-- TOTAL JOBS NON CPUS (Internal Service)  
-- ==============================================================================================================================  
select * into #TempJobsNonCPUSInitServPvt from (  
  select *  
        from (  
     
   select inv.CompanyCode, inv.BranchCode  
    , (select spk.ForemanID from svTrnService spk where spk.CompanyCode = inv.CompanyCode and spk.BranchCode = inv.BranchCode  
     and spk.JobOrderNo = inv.JobOrderNo) EmployeeID  
    ,  'W' + substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3) InvoiceWeek   
    , (  
     SELECT Count(temp.OperationNo) FROM  svTrnInvTask temp WHERE temp.InvoiceNo = inv.InvoiceNo  
      ) TotalJobNonCPUSInitServ  
   from svTrnInvoice inv  
   where inv.CompanyCode = @CompanyCode   
    and inv.BranchCode = @BranchCode  
    and Month(inv.InvoiceDate) = substring(Convert(varchar, @Month, 109), 1, 3)   
    and Year(inv.InvoiceDate) = @Year  
    and substring(inv.InvoiceNo,1,3) = 'INI'  
    and inv.JobOrderNo IN (select JobOrderNo from #TempSPKNo)  
   group by inv.CompanyCode,inv.BranchCode, substring(Convert(varchar, DATEPART(WEEK, InvoiceDate) - @MinusWeek), 1, 3), inv.JobOrderNo, inv.InvoiceNo  
  ) as Header  
  pivot(  
   sum(TotalJobNonCPUSInitServ)  
   for InvoiceWeek in (W1,W2,W3,W4,W5,W6)  
  ) pvt  
 )#TempJobsNonCPUSInitServPvt  
  
select * into #TempJobsNonCPUSInitServ from (  
  select isnull(b.CompanyCode, @CompanyCode) CompanyCode, isnull(b.BranchCode, @BranchCode) BranchCode, a.EmployeeID, '   Internal Service/Repairs (Others)' Description, 21 SeqNo, 'Jobs' UOM  
  , (  
    select sum(ISNULL(TargetNonCPUSinService,0))   
     from svMstTargetSA  
     where CompanyCode = @CompanyCode and BranchCode = @BranchCode and month(targetDate) = @Month and year(targetDate) = @year  
     and ProductType = @ProductType  
     ) Target  
  , isnull(b.W1, 0) Week1, isnull(b.W2, 0) Week2, isnull(b.W3, 0) Week3, isnull(b.W4, 0) Week4, isnull(b.W5, 0) Week5, isnull(b.W6,0) Week6  
        from  
   #TempEmployee a  
  LEFT JOIN  
  (   
   SELECT * FROM #TempJobsNonCPUSInitServPvt  
  ) b ON a.EmployeeID = b.EmployeeID  
  WHERE a.EmployeeID IN (select EmployeeID from #TempEmployee)  
 )#TempJobsNonCPUSInitServ  
  
-- ==============================================================================================================================  
-- ==============================================================================================================================  
  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempSalesReveneu a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempLaborSalesReveneu a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempSalesSparepartReveneu a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempSalesOilMaterialReveneu a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempTotalUnit a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempTotalPassengerUnit a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempTotalCommercialUnit a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsCPUS a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsCPUS30000 a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsCPUS10000 a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsCPUS5000 a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsCPUSOil a  
  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsCPUSOverhaulRepair a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsCPUSTyre a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsCPUSAdjustment a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsNonCPUS a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsNonCPUSClaim a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsNonCPUSFS a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsNonCPUSReWork a  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsNonCPUSInitServ a  
  
  
insert into #t_WeekReview select a.CompanyCode, a.BranchCode, a.EmployeeID  
 , (select EmployeeName from gnMstEmployee where EmployeeID = a.EmployeeID and CompanyCode = a.CompanyCode and BranchCode = a.BranchCode) EmployeeName   
 , a.Description  
 , a.SeqNo, a.UOM, a.Target  
 , a.Week1, a.Week2, a.Week3, a.Week4, a.Week5, a.Week6  
 FROM #TempJobsCPUSBodyRepair a  
  
select @Month YearPeriode, (Week1 + Week2 + Week3 + Week4 + Week5 +Week6) Total, * from #t_WeekReview   
order by EmployeeID, SeqNo  
  
DROP TABLE #t_WeekReview   
DROP TABLE #TempEmployee  
DROP TABLE #TempSPKNo  
DROP TABLE #TempSalesReveneuPvt  
DROP TABLE #TempSalesReveneu  
DROP TABLE #TempLaborSalesReveneuPvt  
DROP TABLE #TempLaborSalesReveneu  
DROP TABLE #TempSalesSparepartReveneuPvt  
DROP TABLE #TempSalesSparepartReveneu  
DROP TABLE #TempSalesOilMaterialReveneuPvt  
DROP TABLE #TempSalesOilMaterialReveneu  
DROP TABLE #TempTotalUnitPvt  
DROP TABLE #TempTotalUnit  
DROP TABLE #TempTotalPassengerUnitPvt  
DROP TABLE #TempTotalPassengerUnit  
DROP TABLE #TempTotalCommercialUnitPvt  
DROP TABLE #TempTotalCommercialUnit  
DROP TABLE #TempJobsCPUSPvt  
DROP TABLE #TempJobsCPUS  
DROP TABLE #TempJobsCPUS30000Pvt  
DROP TABLE #TempJobsCPUS30000  
DROP TABLE #TempJobsCPUS10000Pvt  
DROP TABLE #TempJobsCPUS10000  
DROP TABLE #TempJobsCPUS5000Pvt  
DROP TABLE #TempJobsCPUS5000  
DROP TABLE #TempJobsCPUSOilPvt  
DROP TABLE #TempJobsCPUSOil  
DROP TABLE #TempJobsCPUSOverhaulRepairPvt  
DROP TABLE #TempJobsCPUSOverhaulRepair  
DROP TABLE #TempJobsCPUSTyrePvt  
DROP TABLE #TempJobsCPUSTyre  
DROP TABLE #TempJobsCPUSAdjustmentPvt  
DROP TABLE #TempJobsCPUSAdjustment  
DROP TABLE #TempJobsNonCPUSPvt  
DROP TABLE #TempJobsNonCPUS  
DROP TABLE #TempJobsNonCPUSClaimPvt  
DROP TABLE #TempJobsNonCPUSClaim  
DROP TABLE #TempJobsNonCPUSFSPvt  
DROP TABLE #TempJobsNonCPUSFS  
DROP TABLE #TempJobsNonCPUSReWorkPvt  
DROP TABLE #TempJobsNonCPUSReWork  
DROP TABLE #TempJobsNonCPUSInitServPvt  
DROP TABLE #TempJobsNonCPUSInitServ  
DROP TABLE #TempJobsCPUSBodyRepairPvt  
DROP TABLE #TempJobsCPUSBodyRepair  
DROP TABLE #Job1  
DROP TABLE #Job2  
DROP TABLE #1  
DROP TABLE #JobGroup  
DROP TABLE #JobGroupOthers  
END  