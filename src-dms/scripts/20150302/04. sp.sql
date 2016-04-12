create procedure [dbo].[BrowseParameter] 
as
declare @tblParam as table  (
	DBName varchar(50),
	Extensions varchar(50),
	Prefix varchar(50),
	FolderPath varchar(500),
	DCSURL varchar(500),
	TAXURL varchar(500)
)

insert @tblParam (DBName, Extensions, Prefix, FolderPath, DCSURL, TAXURL   )
select (select ParamValue from sysParameter where ParamId='BACKUP_DBNAME'),
(select ParamValue from sysParameter where ParamId='BACKUP_FILEEXT'),
(select ParamValue from sysParameter where ParamId='BACKUP_FILENAME'),
(select ParamValue from sysParameter where ParamId='BACKUP_FOLDERPATH'),
(select ParamValue from sysParameter where ParamId='DCS_URL'),
(select ParamValue from sysParameter where ParamId='TAX_URL')

select  * from @tblParam
GO

CREATE procedure [dbo].[EmployeeTraining] @CompanyCode varchar(50), @BranchCode varchar(50), @EmployeeID varchar(50)
as
select a.CompanyCode, BranchCode, EmployeeID, IsSuzukiTraining, TrainingCode, CONVERT(VARCHAR(50),BeginTrainingDate,106) BeginTrainingDate, CONVERT(VARCHAR(50), EndTrainingDate,106) EndTrainingDate, IsActive, a.CreatedBy, a.CreatedDate, a.LastupdateBy, a.LastupdateDate,
	case when a.IsSuzukiTraining= 1 then c.LookUpValueName else b.LookUpValueName end Notes
	from gnMstEmployeeTraining a
	left join gnMstLookUpDtl b
		on a.CompanyCode = b.CompanyCode and b.CodeID = 'IEDU' and a.TrainingCode=b.LookUpValue 
	left join gnMstLookUpDtl c
		on a.CompanyCode = c.CompanyCode and c.CodeID = 'SZKT' and a.TrainingCode=c.LookUpValue 
	where a.CompanyCode=@CompanyCode and a.BranchCode=@BranchCode and a.EmployeeID=@EmployeeID
GO

CREATE procedure [dbo].[spMstModelColour] (@CompanyCode varchar(10) , @SalesModelCode varchar(10))
 as
SELECT CompanyCode, SalesModelCode, ColourCode, RefferenceDesc1, Remark
FROM omMstModelColourView
where CompanyCode=@CompanyCode and SalesModelCode=@SalesModelCode
GO

create procedure [dbo].[uspfn_AbInqAttendance]
	@CompanyCode varchar(12),
	@EmployeeID varchar(25),
	@EmployeeName varchar(100),
	@DateFrom datetime,
	@DateTo datetime
as

begin
	if dbo.uspfn_IsNullOrEmpty(@EmployeeID) = 1 
	begin
		set @EmployeeID = '';
	end

	if dbo.uspfn_IsNullOrEmpty(@EmployeeName) = 1 
	begin
		set @EmployeeName = '';
	end

	select a.EmployeeID
	     , b.EmployeeName
		 , d.OrgName as Department
		 , c.PosName as Position
		 , isnull(e.LookUpValueName, '-') as Grade
		 , AttendanceDate = convert(datetime, (
				SUBSTRING(a.AttdDate, 0, 5) +
				'-' +
				SUBSTRING(a.AttdDate, 5, 2) +
				'-' +
				SUBSTRING(a.AttdDate, 7, 2) 
		   ))
	     --, a.AttdDate
	     , a.OnDutyTime
		 , a.OffDutyTime
	     , a.ClockInTime
		 , a.ClockOutTime
		 , IsAbsence = (
				case 
					when a.ClockInTime is null and a.ClockOutTime is null then 'Yes'
					when a.ClockInTime is not null then 'No'
					when a.ClockInTime is null then 'Yes'
				end
		   )
		 , LateTime = (
				case
					when a.ClockInTime > a.OnDutyTime then dbo.uspfn_MinuteToTime(dbo.uspfn_CalculateMinute(convert(char(5), a.OnDutyTime), convert(char(5), a.ClockInTime)))
					else '-'
				end
		   )
		 , ReturnBeforeTheTime = (
				case
					when a.ClockOutTime < a.OffDutyTime then dbo.uspfn_MinuteToTime(dbo.uspfn_CalculateMinute(convert(char(5), a.ClockOutTime), convert(char(5), a.OffDutyTime)))
					else '-'
				end
		   )
		 , Overtime = (
				case
					when a.ClockOutTime > a.OffDutyTime then dbo.uspfn_MinuteToTime(dbo.uspfn_CalculateMinute(convert(char(5), a.OffDutyTime), convert(char(5), a.ClockOutTime)))
					else '-'
				end
		   )
		 , WorkingTime = dbo.uspfn_MinuteToTime(dbo.uspfn_CalculateMinute(convert(char(5), a.ClockInTime), convert(char(5), a.ClockOutTime)))
         , Notes = ''
	  from HrEmployeeShift a
	 inner join HrEmployee b
	    on a.CompanyCode=b.CompanyCode
	   and a.EmployeeID=b.EmployeeID
	 inner join gnMstPosition c
	    on c.CompanyCode=a.CompanyCode
	   and c.DeptCode=b.Department
	   and c.PosCode=b.Position
	 inner join gnMstOrgGroup d
	    on d.CompanyCode=a.CompanyCode
	   and d.OrgCode=b.Department
	  left join gnMstLookUpDtl e
	    on e.CompanyCode=a.CompanyCode
	   and e.CodeID='ITSG'
	   and e.LookUpValue=b.Grade
 	 where a.CompanyCode=@CompanyCode 
       and a.AttdDate>=(
				convert(varchar(4), datepart(year, @DateFrom)) +
				right((replicate('0', 1) + convert(varchar(2), datepart(month, @DateFrom))), 2) +
				right((replicate('0', 1) + convert(varchar(2), datepart(day, @DateFrom))), 2)
	       )
	   and a.AttdDate<=(
				convert(varchar(4), datepart(year, @DateTo)) +
				right((replicate('0', 1) + convert(varchar(2), datepart(month, @DateTo))), 2) +
				right((replicate('0', 1) + convert(varchar(2), datepart(day, @DateTo))), 2)
	       )
	   and a.EmployeeID like '%' + @EmployeeID + '%'
	   and a.EmployeeID in (
			select x.EmployeeID
			  from HrEmployee x
			 where x.EmployeeName like '%' + @EmployeeName + '%'
			   and x.EmployeeID like '%' + @EmployeeID + '%'
	   ) 
		
	 order by a.AttdDate asc
end
GO

CREATE procedure [dbo].[uspfn_BrowseParameter] @companycode varchar(50)
as
declare @BACKUP_DBNAME varchar(50),
@BACKUP_FILEEXT varchar(50),
@BACKUP_FILENAME varchar(50),
@BACKUP_FOLDERPATH varchar(50),
@DCS_URL varchar(50),
@TAX_URL varchar(50)
--declare @tblParam as table  (
--	DBName varchar(50),
--	Extensions varchar(50),
--	Prefix varchar(50),
--	FolderPath varchar(500),
--	DCSURL varchar(500),
--	TAXURL varchar(500)
--)

--insert @tblParam (DBName, Extensions, Prefix, FolderPath, DCSURL, TAXURL   )
set @BACKUP_DBNAME= (select ParamValue from sysParameter where ParamId='BACKUP_DBNAME')
set @BACKUP_FILEEXT= (select ParamValue from sysParameter where ParamId='BACKUP_FILEEXT')
set @BACKUP_FILENAME= (select ParamValue from sysParameter where ParamId='BACKUP_FILENAME')
set @BACKUP_FOLDERPATH= (select ParamValue from sysParameter where ParamId='BACKUP_FOLDERPATH')
set @DCS_URL= (select ParamValue from sysParameter where ParamId='DCS_URL')
set @TAX_URL= (select ParamValue from sysParameter where ParamId='TAX_URL')

if exists(Select companycode from sysParam ) begin
	update sysParam
	set dbname = @BACKUP_DBNAME, Extensions=@BACKUP_FILEEXT, Prefix=@BACKUP_FILENAME, FolderPath=@BACKUP_FOLDERPATH, DCSURL=@DCS_URL, TAXURL=@TAX_URL
	where companycode=@companycode
end else begin
	insert sysParam (CompanyCode, DBName, Extensions, Prefix, FolderPath, DCSURL, TAXURL)
	select  @companycode, @BACKUP_DBNAME, @BACKUP_FILEEXT, @BACKUP_FILENAME, @BACKUP_FOLDERPATH, @DCS_URL, @TAX_URL
end
select * from sysParam
GO

CREATE procedure [dbo].[uspfn_CreditLimitView] @CompanyCode varchar(50), @BranchCode varchar(50), @ProfitCenterCode varchar(50), @customerCode varchar(50)
as
SELECT 
convert(varchar(50),(row_number() over(order by b.CustomerCode))) AS  Nomor,
d.LookUpValueName AS ProfitCenterCode,
b.CustomerCode, e.CustomerName,
convert(varchar(50),ISNULL(b.CreditLimit,0)) AS CreditLimit,
convert(varchar(50),ISNULL(c.SalesAmt, 0)) AS SalesAmt,
convert(varchar(50),ISNULL(c.ReceivedAmt,0)) AS ReceivedAmt,
convert(varchar(50),ISNULL(b.CreditLimit,0)-(ISNULL(c.SalesAmt, 0) - ISNULL(c.ReceivedAmt,0))) AS BalanceAmt,
convert(varchar(50),(ISNULL(c.SalesAmt, 0) - ISNULL(c.ReceivedAmt,0))) AS Credit,
ISNULL((Select LookUpValueName From GnMstlookUpDtl x where x.CompanyCode = b.CompanyCode AND x.LookUpValue = b.PaymentCode AND CodeID ='PYBY'),'-') AS PaymentBy
FROM gnMstCustomerProfitCenter b
LEFT JOIN GnTrnBankBook c  ON  c.CompanyCode = b.CompanyCode AND c.BranchCode = b.BranchCode AND b.CustomerCode = c.CustomerCode AND c.ProfitCenterCode = b.ProfitCenterCode
LEFT JOIN GnMstlookUpDtl d ON  d.CompanyCode = b.CompanyCode AND d.CodeId = 'PFCN' AND b.ProfitCenterCode = d.LookUpValue
LEFT JOIN GnMstCustomer e  ON  e.CompanyCode = b.CompanyCode AND e.CustomerCode = b.CustomerCode 
WHERE 
b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode AND b.ProfitCenterCode = @ProfitCenterCode 
and b.CustomerCode = case when isnull(@customerCode,'')='' then b.CustomerCode else @customerCode end
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_CustomerUtilityView]  @flag int
AS

if @flag =1 SELECT * , case when Isautogenerate=1 then 'Automatis' else 'Manual' end GenarateNo from GnMstCustomerUtility
if @flag =2 SELECT * , case when Isautogenerate=1 then 'Automatis' else 'Manual' end GenarateNo from gnMstSupplierUtility
GO

Create procedure [dbo].[uspfn_GetAreaDealerOutlet] @companyCode varchar(50), @BranchCode varchar(50)
as
--declare @companyCode varchar(50)='', @BranchCode varchar(50)=''
select top 1 --case when @GroupNo='' then 'All' else AreaDealer end Area , 
		case when @companyCode='' then 'All' else b.CompanyName end Dealer, 
		case when @BranchCode='' then 'All' else a.BranchName end Showroom
from gnMstOrganizationDtl a
inner join gnMstOrganizationHdr b
	on a.CompanyCode=b.CompanyCode
where 1=1 --a.GroupNo= case when isnull(@GroupNo,'')<>'' then @GroupNo else a.GroupNo end --@GroupNo 
and a.companyCode=case when isnull(@companyCode,'')<>'' then @companyCode else a.companyCode end --@dealercode 
and a.BranchCode= case when isnull(@BranchCode,'')<>'' then @BranchCode else a.BranchCode end --@BranchCode
GO
CREATE procedure [dbo].[uspfn_getBrowseEntryRtrPenjualan] @CompanyCode varchar(15), @BranchCode varchar(15), @TypeOfGoods varchar(5)
as
select	 a.CompanyCode, a.BranchCode,   a.ReturnNo, a.ReturnDate, a.FPJNo, b.FPJDate, a.ReferenceNo, a.ReferenceDate, a.CustomerCode
                from	    spTrnSRturHdr a
                left join  spTrnSFPJHdr b on
	                a.CompanyCode = b.CompanyCode
	                and a.BranchCode = b.BranchCode
	                and a.FPJNo = b.FPJNo
                where	    a.CompanyCode = @CompanyCode
                            and a.BranchCode = @BranchCode	
                            and a.FPJNo = b.FPJNo
                            and a.TypeOfGoods = @TypeOfGoods
                            and a.Status in ('0', '1')
                order by    a.ReturnNo desc
GO
CREATE procedure [dbo].[uspfn_GetSODetails] @CompanyCode varchar(15), @BranchCode varchar(15), @POSNo varchar(20)  
as   
SELECT   
    A.CompanyCode,  
    A.BranchCode,  
    A.POSNo,   
    A.SupplierCode,   
    B.PartNo,   
    B.OrderQty,   
    B.PurchasePrice,  
    B.CostPrice,   
    B.ABCClass,   
    B.MovingCode,   
    B.ProductType,  
    B.PartCategory,   
    A.TypeOfGoods,  
    B.DiscPct,  
    B.Note,  
    C.PartName,
    B.TotalAmount  
FROM spTrnPPOSHdr A   
INNER JOIN spTrnPPOSDtl B ON (A.CompanyCode = B.CompanyCode)  
    AND (A.BranchCode = B.BranchCode)  
    AND (A.POSNo = B.POSNo)  
left JOIN spMstItemInfo C  
on C.CompanyCode = B.CompanyCode  
and C.PartNo = B.PartNo  
WHERE A.CompanyCode = @CompanyCode  
    AND A.BranchCode = @BranchCode  
    AND A.POSNo = @POSNo
GO
CREATE PROCEDURE [dbo].[uspfn_GetSPKForApprovalPdiFsc]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@IsPdi bit
AS
BEGIN
--	declare @CompanyCode as varchar(15)
--	declare @BranchCode as varchar(15)
--	declare @ProductType as varchar(15)
--	declare @IsPDI as bit
--
--	set @CompanyCode = '6014401'
--	set @BranchCode = '601440100'
--	set @ProductType = '4W'
--	set @IsPDI = 0
	
	select convert(bit, 0) IsSelected, row_number() over(order by a.ServiceNo asc) as No,a.BranchCode, a.ServiceNo, a.JobOrderNo, a.JobOrderDate
	, a.ServiceBookNo, a.ChassisNo, a.BasicModel, a.JobType, (isnull(srv.ValItem, 0) + isnull(task.valTask,0)) TotalApprove
	from SvTrnService a
	left join (select CompanyCode, BranchCode, ServiceNo, sum((OperationHour * OperationCost)) valTask
		from svTrnSrvTask
		where BillType = 'F'
		group by CompanyCode, BranchCode, ServiceNo) task on task.CompanyCode = a.CompanyCode
											and task.BranchCode = a.BranchCode
											and task.ServiceNo = a.ServiceNo
	left join (select CompanyCode, BranchCode, ServiceNo, sum(((SupplyQty - ReturnQty) * RetailPrice)) valItem 
		from svTrnSrvItem 
		where BillType = 'F'
		group by CompanyCode, BranchCode, ServiceNo) srv on srv.CompanyCode = a.CompanyCode
											and srv.BranchCode = a.BranchCode
											and srv.ServiceNo = a.ServiceNo
	where a.CompanyCode = @CompanyCode
	and a.BranchCode = @BranchCode
	and a.ProductType = @ProductType
	and a.ServiceStatus = '5 '
	and a.IsLocked = 0
	and a.JobType like (case when @IsPDI = 1 then 'PDI%' else 'FS%' end )
	
END
GO
CREATE PROCEDURE [dbo].[uspfn_GetSPKForUnApprovalPdiFsc]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@IsPdi bit
AS
BEGIN
	--declare @CompanyCode as varchar(15)
	--declare @BranchCode as varchar(15)
	--declare @ProductType as varchar(15)
	--declare @IsPDI as bit

	--set @CompanyCode = '6058401'
	--set @BranchCode = '605840103'
	--set @ProductType = '4W'
	--set @IsPDI = 0
	
	select convert(bit, 0) IsSelected, row_number() over(order by a.ServiceNo asc) as No,a.BranchCode, a.ServiceNo, a.JobOrderNo, a.JobOrderDate
	, a.ServiceBookNo, a.ChassisNo, a.BasicModel, a.JobType, (isnull(srv.ValItem, 0) + isnull(task.valTask,0)) TotalApprove
	from SvTrnService a
	left join (select CompanyCode, BranchCode, ServiceNo, sum((OperationHour * OperationCost)) valTask
		from svTrnSrvTask
		where BillType = 'F'
		group by CompanyCode, BranchCode, ServiceNo) task on task.CompanyCode = a.CompanyCode
											and task.BranchCode = a.BranchCode
											and task.ServiceNo = a.ServiceNo
	left join (select CompanyCode, BranchCode, ServiceNo, sum(((SupplyQty - ReturnQty) * RetailPrice)) valItem 
		from svTrnSrvItem 
		where BillType = 'F'
		group by CompanyCode, BranchCode, ServiceNo) srv on srv.CompanyCode = a.CompanyCode
											and srv.BranchCode = a.BranchCode
											and srv.ServiceNo = a.ServiceNo
	where a.CompanyCode = @CompanyCode
	and a.BranchCode = @BranchCode
	and a.ProductType = @ProductType
	and a.ServiceStatus = '5 '
	and a.IsLocked = 1
	and a.JobType like (case when @IsPDI = 1 then 'PDI%' else 'FS%' end )
END
GO
CREATE procedure [dbo].[uspfn_gnBrowseSupplier] @CompanyCode varchar(15)    
as    
SELECT    
 DISTINCT(a.SupplierCode)    
 , a.SupplierName    
FROM gnMstSupplier a WITH(NOLOCK, NOWAIT)    
LEFT JOIN gnMstSupplierProfitCenter b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode     
    AND a.SupplierCode = b.SupplierCode AND b.IsBlackList = 0  
--LEFT JOIN  gnMstLookUpDtl c ON c.CompanyCode = a.CompanyCode and c.LookUpValue = a.ProvinceCode  
--and c.CodeID = 'PROV'  
--left JOIN gnMstLookUpDtl d ON d.CompanyCode = a.CompanyCode and d.LookUpValue = a.AreaCode  
--and d.CodeID = 'AREA'  
--left JOIN gnMstLookUpDtl e ON e.CompanyCode = a.CompanyCode and e.LookUpValue = a.CityCode  
--and e.CodeID = 'CITY'  
WHERE a.CompanyCode = @CompanyCode AND a.Status = 1
GO
create procedure [dbo].[uspfn_gnGetSegmentAcc] @CompanyCode varchar(15), @BranchCode varchar(15), 
@TipeSegAcc varchar(3)
as
SELECT TipeSegAcc, SegAccNo, Parent, Description  FROM gnMstSegmentAcc 
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND 
TipeSegAcc = @TipeSegAcc ORDER BY TipeSegAcc ASC, SegAccNo ASC
GO
CREATE procedure [dbo].[uspfn_PostingReturnService]    
 @CompanyCode VARCHAR(MAX),    
 @BranchCode  VARCHAR(MAX),    
 @ProductType VARCHAR(MAX),    
 @InvoiceNo  VARCHAR(MAX),    
 @UserID   VARCHAR(MAX)    
AS    
BEGIN    
    
DECLARE @ReturnNo VARCHAR(MAX)    
DECLARE currReturn CURSOR FOR    
SELECT ReturnNo FROM spTrnSRturSrvHdr    
WHERE CompanyCode = @CompanyCode    
 AND BranchCode = @BranchCode    
 AND InvoiceNo = @InvoiceNo    
    
OPEN currReturn    
FETCH NEXT FROM currReturn INTO @ReturnNo    
WHILE @@FETCH_STATUS = 0    
 BEGIN    
 --==========================================================================================================    
 -- AVERAGE COST PRICE (UPDATE MASTER ITEM PRICE AND HISTORY)    
 --==========================================================================================================    
 SELECT * INTO #TempAvgPrice FROM (     
 SELECT    
  a.CompanyCode    
  , a.BranchCode    
  , a.PartNo    
  , ROUND((((b.OnHand * c.CostPrice) + ((a.QtyReturn * a.CostPrice) * (1-(a.DiscPct/100))))     
   / (a.QtyReturn + b.OnHand)),2,2  )AvgCost     
  , c.CostPrice    
 FROM SpTrnSRturSrvDtl a    
  LEFT JOIN SpMstItems b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo    
  LEFT JOIN SpMstItemPrice c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.PartNo = c.PartNo    
 WHERE a.CompanyCode = @CompanyCode    
  AND a.BranchCode = @BranchCode    
  AND a.ReturnNo = @ReturnNo ) #TempAvgPrice    
    
 INSERT INTO [spHstItemPrice]    
           ([CompanyCode]    
           ,[BranchCode]    
           ,[PartNo]    
           ,[UpdateDate]    
           ,[RetailPrice]    
           ,[RetailPriceInclTax]    
           ,[PurchasePrice]    
           ,[CostPrice]    
           ,[Discount]    
           ,[OldRetailPrice]    
           ,[OldPurchasePrice]    
           ,[OldCostPirce]    
           ,[OldDiscount]    
           ,[LastPurchaseUpdate]    
           ,[LastRetailPriceUpdate]    
           ,[CreatedBy]    
           ,[CreatedDate]    
           ,[LastUpdateBy]    
           ,[LastUpdateDate])    
 SELECT a.CompanyCode    
  , a.BranchCode    
  , a.PartNo    
  , DATEADD(ss,row_number() over (order by a.PartNo ASC), GetDate()) UpdateDate    
  , a.RetailPrice     
  , a.RetailPriceInclTax    
  , a.PurchasePrice    
  , b.AvgCost CostPrice    
  , ISNULL((    
   SELECT TOP 1 Discount     
   FROM SpHstItemPrice     
   WHERE CompanyCode = a.CompanyCode     
    AND BranchCode = a.BranchCode    
    AND PartNo = a.PartNo    
   ORDER BY UpdateDate DESC    
   ),0) Discount    
  , a.RetailPrice OldRetailPrice    
  , a.PurchasePrice OldPurchasePrice    
  , a.CostPrice OldCostPrice    
  , ISNULL((    
   SELECT TOP 1 Discount     
   FROM SpHstItemPrice     
   WHERE CompanyCode = a.CompanyCode     
    AND BranchCode = a.BranchCode    
    AND PartNo = a.PartNo    
   ORDER BY UpdateDate DESC    
   ),0) Discount    
  , ISNULL((    
   SELECT TOP 1 LastPurchaseUpdate     
   FROM SpHstItemPrice     
   WHERE CompanyCode = a.CompanyCode     
    AND BranchCode = a.BranchCode    
    AND PartNo = a.PartNo    
   ORDER BY UpdateDate DESC    
   ), NULL)LastPurchaseUpdate    
  , ISNULL((    
   SELECT TOP 1 LastRetailPriceUpdate     
   FROM SpHstItemPrice     
   WHERE CompanyCode = a.CompanyCode     
    AND BranchCode = a.BranchCode    
    AND PartNo = a.PartNo    
   ORDER BY UpdateDate DESC    
   ), NULL) LastRetailPriceUpdate     
  , @UserID CreatedBy    
  , GetDate() CreatedDate    
  , @UserID LastUpdateBy    
  , GetDate() LastUpdateDate    
 FROM spMstItemPrice a    
  INNER JOIN #TempAvgPrice b ON a.CompanyCode = b.CompanyCode     
   AND a.BranchCode = b.BranchCode     
   AND a.PartNo = b.PartNo    
     
 UPDATE SpMstItemPrice    
 SET CostPrice = b.AvgCost    
  , LastUpdateBy = @UserID    
  , LastUpdateDate = GetDate()    
 FROM SpMstItemPrice a, #TempAvgPrice b    
 WHERE a.CompanyCode = b.CompanyCode    
  AND a.BranchCode = b.BranchCode    
  AND a.PartNo = b.PartNo    
 DROP TABLE #TempAvgPrice    
    
 --==========================================================================================================    
 -- UPDATE STOCK AND MOVEMENT    
 --==========================================================================================================    
 UPDATE SpMstItems    
 SET Onhand = Onhand + b.QtyReturn    
  , LastUpdateBy = @UserID    
  , LastUpdateDate = GetDate()    
 FROM SpMstItems a, SpTrnSRturSrvDtl b    
 WHERE a.CompanyCode = b.CompanyCode    
  AND a.BranchCode = b.BranchCode    
  AND a.PartNo = b.PartNo    
  AND b.CompanyCode = @CompanyCode    
  AND b.BranchCode = @BranchCode    
  AND b.ReturnNo = @ReturnNo     
     
    
 UPDATE SpMstItemLoc    
 SET Onhand = Onhand + b.QtyReturn    
  , LastUpdateBy = @UserID    
  , LastUpdateDate = GetDate()    
 FROM SpMstItemLoc a, SpTrnSRturSrvDtl b    
 WHERE a.CompanyCode = b.CompanyCode    
  AND a.BranchCode = b.BranchCode    
  AND a.PartNo = b.PartNo    
  AND a.WarehouseCode = '00'    
  AND b.CompanyCode = @CompanyCode    
  AND b.BranchCode = @BranchCode    
  AND b.ReturnNo = @ReturnNo     
    
 INSERT INTO [spTrnIMovement]    
           ([CompanyCode]    
           ,[BranchCode]    
           ,[DocNo]    
           ,[DocDate]    
           ,[CreatedDate]    
           ,[WarehouseCode]    
           ,[LocationCode]    
           ,[PartNo]    
           ,[SignCode]    
           ,[SubSignCode]    
           ,[Qty]    
           ,[Price]    
           ,[CostPrice]    
           ,[ABCClass]    
           ,[MovingCode]    
           ,[ProductType]    
           ,[PartCategory]    
           ,[CreatedBy])    
 SELECT a.CompanyCode    
  , a.BranchCode    
  , a.ReturnNo DocNo    
  , (    
   SELECT ReturnDate     
   FROM SpTrnSRturSrvHdr     
   WHERE CompanyCode = a.CompanyCode     
    AND BranchCode = a.BranchCode     
    AND ReturnNo = a.ReturnNo    
    ) DocDate    
  , DATEADD(ss,row_number() over (order by a.PartNo ASC), GetDate()) CreatedDate    
  , '00' WarehouseCode    
  , (    
   SELECT LocationCode    
   FROM SpMstItemLoc     
   WHERE CompanyCode = a.CompanyCode    
    AND BranchCode = a.BranchCode    
    AND WarehouseCode = '00'    
    AND PartNo = a.PartNo    
    ) LocationCode    
  , a.PartNo    
  , 'IN' SignCode    
  , 'RSRV' SubSignCode    
  , a.QtyReturn Qty    
  , a.RetailPrice Price    
  , a.CostPrice     
  , a.ABCClass    
  , a.MovingCode    
  , @ProductType ProductType    
  , a.PartCategory    
  , @UserID CreatedBy    
 FROM SpTrnSRturSrvDtl a     
 WHERE CompanyCode = @CompanyCode    
  AND BranchCode = @BranchCode    
  AND ReturnNo = @ReturnNo     
     
 --==========================================================================================================    
 -- INSERT GLINTERFACE (JOURNAL) AND ARINTERFACE    
 --==========================================================================================================    
 INSERT INTO [arInterface]    
           ([CompanyCode]    
           ,[BranchCode]    
           ,[DocNo]    
           ,[DocDate]    
           ,[ProfitCenterCode]    
           ,[NettAmt]    
           ,[ReceiveAmt]    
           ,[CustomerCode]    
           ,[TOPCode]    
           ,[TypeTrans]    
           ,[BlockAmt]    
           ,[DebetAmt]    
           ,[CreditAmt]    
           ,[SalesCode]    
           ,[AccountNo]    
           ,[StatusFlag]    
           ,[CreateBy]    
           ,[CreateDate])     
 SELECT     
  a.CompanyCode    
  , a.BranchCode    
  , a.ReturnNo DocNo    
  , a.ReturnDate DocDate    
  , '300' ProfitCenterCode     
  , a.TotCostAmt NettAmt    
  , 0 ReceiveAmt    
  , a.CustomerCode    
  , (    
   SELECT TOPCode     
   FROM GnMstCustomerProfitCenter    
   WHERE CompanyCode = a.CompanyCode    
    AND BranchCode = a.BranchCode    
    AND CustomerCode = a.CustomerCode    
    AND ProfitCenterCode = '200'    
   ) TOPCode    
  , 'SRETURN' TypeTrans    
  , 0 BlockAmt    
  , 0 DebetAmt    
  , 0 CreditAmt    
  , (    
   SELECT SalesCode     
   FROM GnMstCustomerProfitCenter    
   WHERE CompanyCode = a.CompanyCode    
    AND BranchCode = a.BranchCode    
    AND CustomerCode = a.CustomerCode    
    AND ProfitCenterCode = '200'    
   ) SalesCode    
  , ISNULL((    
   SELECT b.ReceivableAccNo     
   FROM GnMstCustomerClass b    
   WHERE CompanyCode = a.CompanyCode    
    AND BranchCode = a.BranchCode    
    --AND TypeOfGoods = a.TypeOfGoods    
    AND ProfitCenterCode = '200'    
    AND CustomerCode = a.CustomerCode    
    AND CustomerClass = ISNULL((SELECT CustomerClass    
           FROM gnMstCustomerProfitCenter    
           WHERE CompanyCode = a.CompanyCode    
            AND BranchCode = a.BranchCode    
            AND CustomerCode = a.CustomerCode    
            AND ProfitCenterCode = '200'),'')    
    ),'') AccountNo    
  , '0' StatusFlag    
  , @UserId CreatedBy    
  , GetDate() CreatedDate    
 FROM SpTrnSRturSrvHdr a    
 WHERE a.CompanyCode = @CompanyCode    
  AND a.BranchCode = @BranchCode    
  AND a.ReturnNo = @ReturnNo    
     
    
 -- CHECK FOR THE AMOUNT IF EXIST     
 -- ================================    
 DECLARE @counterGL INT    
 DECLARE @TypeReturn VARCHAR(100)    
    
 SET @counterGL = 1    
 SET @TypeReturn = CASE WHEN ISNULL((select ParaValue from gnMstLookupDtl     
        where codeid = 'gtgo'    
         and lookupvalue = isnull((    
          select typeofgoods     
          from sptrnsrtursrvdtl     
          where CompanyCode = @CompanyCode    
           AND BranchCode = @BranchCode    
           AND ReturnNo = @ReturnNo),''))    
       ,'') <> 'SPAREPART' THEN 'MATERIAL' ELSE 'SPAREPART' END    
    
     
 -- INSERT GLINTERFACE NO.1 (RETURN)    
 -- ================================    
 IF(ISNULL((    
  SELECT TotReturAmt     
  FROM SpTrnSRturSrvHdr     
  WHERE CompanyCode = @CompanyCode     
   AND BranchCode = @BranchCode     
   AND ReturnNo = @ReturnNo),0)    
   > 0)    
 BEGIN    
 INSERT INTO [dbo].[glInterface]    
           ([CompanyCode]    
           ,[BranchCode]    
           ,[DocNo]    
           ,[SeqNo]    
           ,[DocDate]    
           ,[ProfitCenterCode]    
           ,[AccDate]    
           ,[AccountNo]    
           ,[JournalCode]    
           ,[TypeJournal]    
           ,[ApplyTo]    
           ,[AmountDb]    
           ,[AmountCr]    
           ,[TypeTrans]    
           ,[BatchNo]    
           ,[BatchDate]    
           ,[StatusFlag]    
           ,[CreateBy]    
           ,[CreateDate]    
           ,[LastUpdateBy]    
           ,[LastUpdateDate])    
 SELECT     
  a.CompanyCode    
  , a.BranchCode    
  , a.ReturnNo DocNo    
  , @counterGL SeqNo    
  , a.ReturnDate DocDate    
  , '200' ProfitCenterCode    
  , GetDate() AccDate    
  , b.ReturnAccNo AccountNo    
  , 'SPAREPART' JournalCode    
  , 'RETURN' TypeJournal    
  , a.InvoiceNo ApplyTo    
  , a.TotReturAmt AmountDB    
  , 0 AmountCR    
  , 'RETURN' + ' ' + @TypeReturn + + '(' + c.TypeOfGoods + ')' TypeTrans    
  , '' BatchNo    
  , '1900-01-01 00:00:00.000' BatchDate    
  , '0' StatusFlag    
  , @UserID CreateBy    
  , GetDate() CreateDate    
  , @UserID LastUpdateBy    
  , GetDate() LastUpdateDate    
 FROM SpTrnSRturSrvHdr a    
LEFT JOIN SpTrnSRturSrvdtl c ON a.CompanyCode = C.CompanyCode AND a.BranchCode = C.BranchCode AND a.ReturnNo = c.ReturnNo  
 LEFT JOIN SpMstAccount b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND c.TypeOfGoods = b.TypeOfGoods    
 WHERE a.CompanyCode = @CompanyCode    
  AND a.BranchCode = @BranchCode    
  AND a.ReturnNo = @ReturnNo    
     
 SET @counterGL = @counterGL + 1    
 END     
    
 -- INSERT GLINTERFACE NO.2 (TAX OUT)    
 -- ================================    
 IF(ISNULL((    
  SELECT TotPPNAmt     
  FROM SpTrnSRturSrvHdr     
  WHERE CompanyCode = @CompanyCode     
   AND BranchCode = @BranchCode     
   AND ReturnNo = @ReturnNo),0)    
   > 0)    
 BEGIN    
 IF(ISNULL((    
   SELECT a.TaxOutAccNo    
   FROM GnMstCustomerClass a    
   WHERE CompanyCode = @CompanyCode    
    AND BranchCode = @BranchCode    
    AND TypeOfGoods = isnull((    
          select typeofgoods     
  from sptrnsrtursrvhdr     
          where CompanyCode = @CompanyCode    
           AND BranchCode = @BranchCode    
           AND ReturnNo = @ReturnNo),'')    
    AND ProfitCenterCode = '200'    
    AND CustomerClass = ISNULL((SELECT CustomerClass    
           FROM gnMstCustomerProfitCenter    
           WHERE CompanyCode = @CompanyCode    
            AND BranchCode = @BranchCode    
            AND CustomerCode = isnull((    
                 select CustomerCode    
                 from sptrnsrtursrvhdr     
                 where CompanyCode = @CompanyCode    
                  AND BranchCode = @BranchCode    
                  AND ReturnNo = @ReturnNo),'')    
            AND ProfitCenterCode = '200'),'')),'') = '')    
 BEGIN    
  DECLARE @errmsg VARCHAR(MAX)    
  SET @errmsg = char(13) + 'Error Message: Customer ini belum mempuyai customer class, Harap periksa kembali setting-an customer class untuk customer ini !'    
  RAISERROR (@errmsg,16,1);    
 END    
 INSERT INTO [dbo].[glInterface]    
           ([CompanyCode]    
           ,[BranchCode]    
           ,[DocNo]    
           ,[SeqNo]    
           ,[DocDate]    
           ,[ProfitCenterCode]    
           ,[AccDate]    
           ,[AccountNo]    
           ,[JournalCode]    
           ,[TypeJournal]    
           ,[ApplyTo]    
           ,[AmountDb]    
           ,[AmountCr]    
           ,[TypeTrans]    
           ,[BatchNo]    
           ,[BatchDate]    
           ,[StatusFlag]    
           ,[CreateBy]    
           ,[CreateDate]    
           ,[LastUpdateBy]    
           ,[LastUpdateDate])    
 SELECT     
  a.CompanyCode    
  , a.BranchCode    
  , a.ReturnNo DocNo    
  , @counterGL SeqNo    
  , a.ReturnDate DocDate    
  , '200' ProfitCenterCode    
  , GetDate() AccDate    
  , ISNULL((    
   SELECT b.TaxOutAccNo    
   FROM GnMstCustomerClass b    
   WHERE CompanyCode = a.CompanyCode    
    AND BranchCode = a.BranchCode    
    AND TypeOfGoods = c.TypeOfGoods    
    AND ProfitCenterCode = '200'    
    AND CustomerCode = a.CustomerCode    
    AND CustomerClass = ISNULL((SELECT CustomerClass    
           FROM gnMstCustomerProfitCenter    
           WHERE CompanyCode = a.CompanyCode    
            AND BranchCode = a.BranchCode    
            AND CustomerCode = a.CustomerCode    
            AND ProfitCenterCode = '200'),'')),'')    
  , 'SPAREPART' JournalCode    
  , 'RETURN' TypeJournal    
  , a.InvoiceNo ApplyTo    
  , a.TotPPNAmt AmountDB    
  , 0 AmountCR    
  , 'TAX OUT' + ' ' + @TypeReturn + + '(' + c.TypeOfGoods + ')' TypeTrans    
  , '' BatchNo    
  , '1900-01-01 00:00:00.000' BatchDate    
  , '0' StatusFlag    
  , @UserID CreateBy    
  , GetDate() CreateDate    
  , @UserID LastUpdateBy    
  , GetDate() LastUpdateDate    
 FROM SpTrnSRturSrvHdr a    
LEFT JOIN SpTrnSRturSrvdtl c ON a.CompanyCode = C.CompanyCode AND a.BranchCode = C.BranchCode AND a.ReturnNo = c.ReturnNo  
 LEFT JOIN SpMstAccount b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND c.TypeOfGoods = b.TypeOfGoods    
 WHERE a.CompanyCode = @CompanyCode    
  AND a.BranchCode = @BranchCode    
  AND a.ReturnNo = @ReturnNo    
    
 SET @counterGL = @counterGL + 1    
 END    
 -- INSERT GLINTERFACE NO.3 (HRETURN)    
 -- ================================    
 IF(ISNULL((    
  SELECT TotFinalReturAmt     
  FROM SpTrnSRturSrvHdr     
  WHERE CompanyCode = @CompanyCode     
   AND BranchCode = @BranchCode     
   AND ReturnNo = @ReturnNo),0)    
   > 0)    
 BEGIN    
 INSERT INTO [dbo].[glInterface]    
           ([CompanyCode]    
           ,[BranchCode]    
           ,[DocNo]    
           ,[SeqNo]    
           ,[DocDate]    
           ,[ProfitCenterCode]    
           ,[AccDate]    
           ,[AccountNo]    
           ,[JournalCode]    
           ,[TypeJournal]    
           ,[ApplyTo]    
           ,[AmountDb]    
           ,[AmountCr]    
           ,[TypeTrans]    
           ,[BatchNo]    
           ,[BatchDate]    
           ,[StatusFlag]    
           ,[CreateBy]    
           ,[CreateDate]    
         ,[LastUpdateBy]    
           ,[LastUpdateDate])    
 SELECT     
  a.CompanyCode    
  , a.BranchCode    
  , a.ReturnNo DocNo    
  , @counterGL SeqNo    
  , a.ReturnDate DocDate    
  , '200' ProfitCenterCode    
  , GetDate() AccDate    
  , b.ReturnPybAccNo AccountNo    
  , 'SPAREPART' JournalCode    
  , 'RETURN' TypeJournal    
  , a.InvoiceNo ApplyTo    
  , 0 AmountDB    
  , a.TotFinalReturAmt AmountCR    
  , 'HRETURN' + ' ' + @TypeReturn + + '(' + c.TypeOfGoods + ')' TypeTrans    
  , '' BatchNo    
  , '1900-01-01 00:00:00.000' BatchDate    
  , '0' StatusFlag    
  , @UserID CreateBy    
  , GetDate() CreateDate    
  , @UserID LastUpdateBy    
  , GetDate() LastUpdateDate    
 FROM SpTrnSRturSrvHdr a    
LEFT JOIN SpTrnSRturSrvdtl c ON a.CompanyCode = C.CompanyCode AND a.BranchCode = C.BranchCode AND a.ReturnNo = c.ReturnNo  
 LEFT JOIN SpMstAccount b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND c.TypeOfGoods = b.TypeOfGoods    
 WHERE a.CompanyCode = @CompanyCode    
  AND a.BranchCode = @BranchCode    
  AND a.ReturnNo = @ReturnNo    
    
 SET @counterGL = @counterGL + 1    
 END    
     
 -- INSERT GLINTERFACE NO.4 (DISC1)    
 -- ================================    
 IF(ISNULL((    
  SELECT TotDiscAmt     
  FROM SpTrnSRturSrvHdr     
  WHERE CompanyCode = @CompanyCode     
   AND BranchCode = @BranchCode     
   AND ReturnNo = @ReturnNo),0)    
   > 0)    
 BEGIN    
 INSERT INTO [dbo].[glInterface]    
           ([CompanyCode]    
           ,[BranchCode]    
           ,[DocNo]    
           ,[SeqNo]    
           ,[DocDate]    
           ,[ProfitCenterCode]    
           ,[AccDate]    
           ,[AccountNo]    
           ,[JournalCode]    
           ,[TypeJournal]    
           ,[ApplyTo]    
           ,[AmountDb]    
           ,[AmountCr]    
           ,[TypeTrans]    
           ,[BatchNo]    
           ,[BatchDate]    
           ,[StatusFlag]    
           ,[CreateBy]    
           ,[CreateDate]    
           ,[LastUpdateBy]    
           ,[LastUpdateDate])    
 SELECT     
  a.CompanyCode    
  , a.BranchCode    
  , a.ReturnNo DocNo    
  , @counterGL SeqNo    
  , a.ReturnDate DocDate    
  , '200' ProfitCenterCode    
  , GetDate() AccDate    
  , b.DiscAccNo AccountNo    
  , 'SPAREPART' JournalCode    
  , 'RETURN' TypeJournal    
  , a.InvoiceNo ApplyTo    
  , 0 AmountDB    
  , a.TotDiscAmt AmountCR    
  , 'DISC1' + ' ' + @TypeReturn + + '(' + c.TypeOfGoods + ')' TypeTrans    
  , '' BatchNo    
  , '1900-01-01 00:00:00.000' BatchDate    
  , '0' StatusFlag    
  , @UserID CreateBy    
  , GetDate() CreateDate    
  , @UserID LastUpdateBy    
  , GetDate() LastUpdateDate    
 FROM SpTrnSRturSrvHdr a    
 LEFT JOIN SpTrnSRturSrvdtl c ON a.CompanyCode = C.CompanyCode AND a.BranchCode = C.BranchCode AND a.ReturnNo = c.ReturnNo  
 LEFT JOIN SpMstAccount b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND c.TypeOfGoods = b.TypeOfGoods    
 WHERE a.CompanyCode = @CompanyCode    
  AND a.BranchCode = @BranchCode    
  AND a.ReturnNo = @ReturnNo    
     
 SET @counterGL = @counterGL + 1    
 END    
    
 -- INSERT GLINTERFACE NO.5 (INVENTORY)    
 -- ================================    
 IF(ISNULL((    
  SELECT TotCostAmt      
  FROM SpTrnSRturSrvHdr     
  WHERE CompanyCode = @CompanyCode     
   AND BranchCode = @BranchCode     
   AND ReturnNo = @ReturnNo),0)    
   > 0)    
 BEGIN    
 INSERT INTO [dbo].[glInterface]    
           ([CompanyCode]    
           ,[BranchCode]    
           ,[DocNo]    
           ,[SeqNo]    
           ,[DocDate]    
           ,[ProfitCenterCode]    
           ,[AccDate]    
           ,[AccountNo]    
           ,[JournalCode]    
           ,[TypeJournal]    
           ,[ApplyTo]    
           ,[AmountDb]    
           ,[AmountCr]    
           ,[TypeTrans]    
           ,[BatchNo]    
           ,[BatchDate]    
           ,[StatusFlag]    
           ,[CreateBy]    
           ,[CreateDate]    
           ,[LastUpdateBy]    
           ,[LastUpdateDate])    
 SELECT     
  a.CompanyCode    
  , a.BranchCode    
  , a.ReturnNo DocNo    
  , @counterGL SeqNo    
  , a.ReturnDate DocDate    
  , '200' ProfitCenterCode    
  , GetDate() AccDate    
  , b.InventoryAccNo AccountNo    
  , 'SPAREPART' JournalCode    
  , 'RETURN' TypeJournal    
  , a.InvoiceNo ApplyTo    
  , a.TotCostAmt AmountDB    
  , 0 AmountCR    
  , 'INVENTORY' + ' ' + @TypeReturn + + '(' + c.TypeOfGoods + ')' TypeTrans    
  , '' BatchNo    
  , '1900-01-01 00:00:00.000' BatchDate    
  , '0' StatusFlag    
  , @UserID CreateBy    
  , GetDate() CreateDate    
  , @UserID LastUpdateBy    
  , GetDate() LastUpdateDate    
 FROM SpTrnSRturSrvHdr a    
LEFT JOIN SpTrnSRturSrvdtl c ON a.CompanyCode = C.CompanyCode AND a.BranchCode = C.BranchCode AND a.ReturnNo = c.ReturnNo  
 LEFT JOIN SpMstAccount b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND c.TypeOfGoods = b.TypeOfGoods    
 WHERE a.CompanyCode = @CompanyCode    
  AND a.BranchCode = @BranchCode    
  AND a.ReturnNo = @ReturnNo    
     
 SET @counterGL = @counterGL + 1    
 END    
    
 -- INSERT GLINTERFACE NO.6 (COGS)    
 -- ================================    
 IF(ISNULL((    
  SELECT TotCostAmt      
  FROM SpTrnSRturSrvHdr     
  WHERE CompanyCode = @CompanyCode     
   AND BranchCode = @BranchCode     
   AND ReturnNo = @ReturnNo),0)    
   > 0)    
 BEGIN    
 INSERT INTO [dbo].[glInterface]    
           ([CompanyCode]    
           ,[BranchCode]    
           ,[DocNo]    
           ,[SeqNo]    
           ,[DocDate]    
           ,[ProfitCenterCode]    
           ,[AccDate]    
           ,[AccountNo]    
           ,[JournalCode]    
           ,[TypeJournal]    
           ,[ApplyTo]    
           ,[AmountDb]    
           ,[AmountCr]    
           ,[TypeTrans]    
           ,[BatchNo]    
           ,[BatchDate]    
           ,[StatusFlag]    
           ,[CreateBy]    
           ,[CreateDate]    
           ,[LastUpdateBy]    
           ,[LastUpdateDate])    
 SELECT     
  a.CompanyCode    
  , a.BranchCode    
  , a.ReturnNo DocNo    
  , @counterGL SeqNo    
  , a.ReturnDate DocDate    
  , '200' ProfitCenterCode    
  , GetDate() AccDate    
  , b.COGSAccNo AccountNo    
  , 'SPAREPART' JournalCode    
  , 'RETURN' TypeJournal    
  , a.InvoiceNo ApplyTo    
  , 0 AmountDB    
  , a.TotCostAmt AmountCR    
  , 'COGS' + ' ' + @TypeReturn + + '(' + c.TypeOfGoods + ')' TypeTrans    
  , '' BatchNo    
  , '1900-01-01 00:00:00.000' BatchDate    
  , '0' StatusFlag    
  , @UserID CreateBy    
  , GetDate() CreateDate    
  , @UserID LastUpdateBy    
  , GetDate() LastUpdateDate    
 FROM SpTrnSRturSrvHdr a    
 LEFT JOIN SpTrnSRturSrvdtl c ON a.CompanyCode = C.CompanyCode AND a.BranchCode = C.BranchCode AND a.ReturnNo = c.ReturnNo  
 LEFT JOIN SpMstAccount b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND c.TypeOfGoods = b.TypeOfGoods    
 WHERE a.CompanyCode = @CompanyCode    
  AND a.BranchCode = @BranchCode    
  AND a.ReturnNo = @ReturnNo    
     
 SET @counterGL = @counterGL + 1    
 END    
     
 --==========================================================================================================    
 -- UPDATE STATUS HEADER    
 --==========================================================================================================    
 UPDATE spTrnSRturSrvHdr    
 SET Status = '2'    
  , LastUpdateBy = @UserID    
  , LastUpdateDate = GetDate()    
 WHERE CompanyCode = @CompanyCode    
  AND BranchCode = @BranchCode    
  AND ReturnNo = @ReturnNo    
 FETCH NEXT FROM currReturn INTO @ReturnNo    
 END    
CLOSE currReturn    
DEALLOCATE currReturn    
    
END
GO
create procedure [dbo].[uspfn_QueryFPJGenerated]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@StartDate varchar(10),
@FPJDate varchar(10),
@ProfitCenter varchar(10),
@IsFPJCentral bit

--declare @CompanyCode as varchar(15)
--declare @BranchCode as varchar(15)
--declare @StartDate as varchar(10)
--declare @FPJDate as varchar(10)
--declare @ProfitCenter as varchar(10)
--declare @IsFPJCentral as bit

--set @CompanyCode = '6006410'
--set @BranchCode  = '600641001'
--set @StartDate  = '20140501'
--set @FPJDate  = '20140513'
--set @ProfitCenter  = '300'
--set @IsFPJCentral  = '1'

as
begin

select 
    y.No,y.chkSelect,y.CompanyCode,y.BranchCode,y.ProfitCenter,y.FPJGovNo
    ,y.FPJGovDate,y.DocNo,convert(datetime,y.DocDate) DocDate
from(
    select 
        row_number() over(order by DocDate, BranchCode,ProfitCenter asc) as No
        , Convert(bit, 0) chkSelect, x.CompanyCode, x.BranchCode
        , a.LookUpValueName ProfitCenter, x.FPJGovNo, x.FPJGovDate
        , x.DocNo, x.DocDate
    from (
	    SELECT	CompanyCode, BranchCode,'300' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate
	    FROM	SpTrnSFPJHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 
                AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'200' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,FPJNo as DocNo,convert(varchar,FPJDate,112) AS DocDate
	    FROM	SvTrnFakturPajak
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, FPJDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'100' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate
	    FROM	OmFakturPajakHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		        ) 
                AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'000' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate
	    FROM	ArFakturPajakHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		        )
                AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
                AND DPPAMt > 0
    ) x 
    left join gnMstLookUpDtl a on a.CompanyCode= x.CompanyCode and a.CodeID='PFCN' and a.LookUpValue= x.ProfitCenter
    where x.ProfitCenter like ''+@ProfitCenter+''
) y
end
GO
create procedure [dbo].[uspfn_SpcekprosesSuggor] (  @CompanyCode varchar(10) ,@BranchCode varchar(10),@MovingCode varchar(5),@SupplierCode  varchar(10),@TypeOfGoods  varchar(15))
 as
SELECT 
 a.PartNo
,ISNULL(a.DemandAverage, 0) AS DemandAverage
,ISNULL(c.LeadTime, 0) AS LeadTime
,ISNULL(c.OrderCycle, 0) AS OrderCycle
,ISNULL(c.SafetyStock, 0) AS SafetyStock
,CAST(0 AS int) AS No
,CAST(0 AS Numeric(4,0)) AS SeqNo
,CAST(ISNULL(a.OnHand, 0) - (
    ISNULL(a.AllocationSP, 0) 
    + ISNULL(a.AllocationSL, 0) 
    + ISNULL(a.AllocationSR, 0)
    + ISNULL(a.ReservedSP, 0) 
    + ISNULL(a.ReservedSL, 0) 
    + ISNULL(a.ReservedSR, 0)
) AS decimal(18,2)) AS AvailableQty
,CAST(0 AS Numeric(4,0)) AS SuggorQty
,CAST(0 AS Numeric(4,0)) AS SuggorCorrecQty
,CAST('' AS varchar(3)) AS ProductType
,a.PartCategory
,CAST(0 AS Numeric(18,0)) AS PurchasePrice
,CAST(0 AS Numeric(18,0)) AS CostPrice
,ISNULL(a.OrderPointQty, 0) AS OrderPoint
,ISNULL(a.OnHand, 0) AS OnHand
,ISNULL(a.OnOrder, 0) AS OnOrder
,ISNULL(a.InTransit, 0) AS InTransit
,ISNULL(a.AllocationSP, 0) AS AllocationSP
,ISNULL(a.AllocationSR, 0) AS AllocationSR
,ISNULL(a.AllocationSL, 0) AS AllocationSL
,ISNULL(a.BackOrderSP, 0) AS BackOrderSP
,ISNULL(a.BackOrderSR, 0) AS BackOrderSR
,ISNULL(a.BackOrderSL, 0) AS BackOrderSL
,ISNULL(a.ReservedSP, 0) AS ReservedSP
,ISNULL(a.ReservedSR, 0) AS ReservedSR
,ISNULL(a.ReservedSL, 0) AS ReservedSL
FROM spMstItems a with(nolock, nowait)
INNER JOIN spMstItemInfo b with(nolock, nowait) ON b.CompanyCode=a.CompanyCode AND b.PartNo=a.PartNo
INNER JOIN SpMstOrderParam c with(nolock, nowait) ON c.CompanyCode=a.CompanyCode AND c.BranchCode=a.BranchCode AND 
		   c.SupplierCode=b.SupplierCode AND c.MovingCode=a.MovingCode
WHERE a.CompanyCode=@CompanyCode 
AND a.BranchCode=@BranchCode
AND a.MovingCode=@MovingCode
AND b.SupplierCode=@SupplierCode
AND a.TypeOfGoods=@TypeOfGoods
AND a.Status = '1'
GO
-- =============================================
-- Author:		David L.
-- Create date: 22 Sep 2014
-- Description:	Get Customer For Report Register 011 & 017B
--              (Report Register Penjualan Per Pelanggan (Tunai&Kredit))
-- =============================================
create PROCEDURE [dbo].[uspfn_spCustRptRgs] 
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@DateFrom datetime,
	@DateTo datetime,
	@PartType varchar(2),
	@PaymentCode nchar(1)
AS
BEGIN
	declare @TOPC varchar(15)
	set @TOPC = 'TOPC'

	if(@PaymentCode = '0')
	begin	
	SELECT DISTINCT
			    a.CustomerCode,
			    UPPER(b.CustomerName) CustomerName
		      FROM 
			    SpTrnSFPJHdr a,
			    GnMstCustomer b,
			    (
				    SELECT 
					    row_number()OVER(ORDER BY SpTrnSFPJHdr.customerCode)rowNum,
					    SpTrnSFPJHdr.customerCode 
				    FROM 
					    SpTrnSFPJHdr 
					    left join gnMstLookupDtl on SpTrnSFPJHdr.CompanyCode = gnMstLookupDtl.CompanyCode
						    AND gnMstLookupDtl.CodeId = @TOPC
				    WHERE
					    SpTrnSFPJHdr.CompanyCode	= @CompanyCode
					    AND SpTrnSFPJHdr.BranchCode = @BranchCode
					    AND convert(varchar,SpTrnSFPJHdr.FPJDate, 112) 
						    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
						    AND convert(varchar, convert(datetime, @DateTo), 112)
					    AND gnMstLookupDtl.ParaValue = 0
				    GROUP BY customerCode
			    )c,
			    gnMstLookupDtl d	
		     WHERE 
			    a.CompanyCode		= b.CompanyCode
			    AND a.CustomerCode	= b.CustomerCode
			    AND a.CustomerCode	= c.CustomerCode
			    AND a.CompanyCode	= @CompanyCode
			    AND a.BranchCode	= @BranchCode
			    AND convert(varchar, a.FPJDate, 112) 
					    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
					    AND convert(varchar, convert(datetime, @DateTo), 112)
			    AND a.CompanyCode	= d.CompanyCode
			    AND d.CodeId		= @TOPC
			    AND a.TOPCode		= d.LookupValue	
			    AND d.ParaValue     = 0 			 		
                AND (CASE WHEN @PartType = '' THEN '' ELSE a.TypeOfGoods END) = @PartType
		     ORDER BY CustomerCode
	end
	else if(@PaymentCode = '1')
	begin 
	SELECT DISTINCT
			    a.CustomerCode,
			    UPPER(b.CustomerName) CustomerName
		      FROM 
			    SpTrnSFPJHdr a,
			    GnMstCustomer b,
			    (
				    SELECT 
					    row_number()OVER(ORDER BY SpTrnSFPJHdr.customerCode)rowNum,
					    SpTrnSFPJHdr.customerCode 
				    FROM 
					    SpTrnSFPJHdr 
					    left join gnMstLookupDtl on SpTrnSFPJHdr.CompanyCode = gnMstLookupDtl.CompanyCode
						    AND gnMstLookupDtl.CodeId = @TOPC
				    WHERE
					    SpTrnSFPJHdr.CompanyCode	= @CompanyCode
					    AND SpTrnSFPJHdr.BranchCode = @BranchCode
					    AND convert(varchar,SpTrnSFPJHdr.FPJDate, 112) 
						    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
						    AND convert(varchar, convert(datetime, @DateTo), 112)
					    AND gnMstLookupDtl.ParaValue = 0
				    GROUP BY customerCode
			    )c,
			    gnMstLookupDtl d	
		     WHERE 
			    a.CompanyCode		= b.CompanyCode
			    AND a.CustomerCode	= b.CustomerCode
			    AND a.CustomerCode	= c.CustomerCode
			    AND a.CompanyCode	= @CompanyCode
			    AND a.BranchCode	= @BranchCode
			    AND convert(varchar, a.FPJDate, 112) 
					    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
					    AND convert(varchar, convert(datetime, @DateTo), 112)
			    AND a.CompanyCode	= d.CompanyCode
			    AND d.CodeId		= @TOPC
			    AND a.TOPCode		= d.LookupValue	
			    AND d.ParaValue     > 0 			 		
                AND (CASE WHEN @PartType = '' THEN '' ELSE a.TypeOfGoods END) = @PartType
		     ORDER BY CustomerCode
	end
	else
	begin
	SELECT DISTINCT
			    a.CustomerCode,
			    UPPER(b.CustomerName) CustomerName
		      FROM 
			    SpTrnSFPJHdr a,
			    GnMstCustomer b,
			    (
				    SELECT 
					    row_number()OVER(ORDER BY SpTrnSFPJHdr.customerCode)rowNum,
					    SpTrnSFPJHdr.customerCode 
				    FROM 
					    SpTrnSFPJHdr 
					    left join gnMstLookupDtl on SpTrnSFPJHdr.CompanyCode = gnMstLookupDtl.CompanyCode
						    AND gnMstLookupDtl.CodeId = @TOPC
				    WHERE
					    SpTrnSFPJHdr.CompanyCode	= @CompanyCode
					    AND SpTrnSFPJHdr.BranchCode = @BranchCode
					    AND convert(varchar,SpTrnSFPJHdr.FPJDate, 112) 
						    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
						    AND convert(varchar, convert(datetime, @DateTo), 112)
					    AND gnMstLookupDtl.ParaValue = 0
				    GROUP BY customerCode
			    )c,
			    gnMstLookupDtl d	
		     WHERE 
			    a.CompanyCode		= b.CompanyCode
			    AND a.CustomerCode	= b.CustomerCode
			    AND a.CustomerCode	= c.CustomerCode
			    AND a.CompanyCode	= @CompanyCode
			    AND a.BranchCode	= @BranchCode
			    AND convert(varchar, a.FPJDate, 112) 
					    BETWEEN convert(varchar, convert(datetime, @DateFrom), 112) 
					    AND convert(varchar, convert(datetime, @DateTo), 112)
			    AND a.CompanyCode	= d.CompanyCode
			    AND d.CodeId		= @TOPC
			    AND a.TOPCode		= d.LookupValue	
                AND (CASE WHEN @PartType = '' THEN '' ELSE a.TypeOfGoods END) = @PartType
		     ORDER BY CustomerCode
	end
END
GO
CREATE procedure [dbo].[uspfn_spGet4FakturRetur] @CompanyCode varchar(15), @BranchCode varchar(15), @TypeOfGoods varchar(15)
as
select a.FPJNo
    , a.FPJDate
    , a.CustomerCode
    , isnull((
			select CustomerName from gnMstCustomer
			 where CompanyCode = a.CompanyCode and CustomerCode = a.CustomerCode
			), '') as CustomerName  
 from spTrnSFpjHdr a, gnMstCoProfileSpare b
where 1 = 1
  and b.CompanyCode = a.CompanyCode
  and b.BranchCode = a.BranchCode
  and a.CompanyCode = @CompanyCode
  and a.BranchCode  = @BranchCode 
  and a.TypeOfGoods = @TypeOfGoods
GO
create procedure [dbo].[uspfn_spGetDcsUploadFile] @ProductType varchar(2), @ProductType1 varchar(2),
   @DataID varchar(6), @DateFrom datetime, @DateTo datetime 
   as
   select ID, DataID, CustomerCode, ProductType, Contents
     , CASE WHEN Status = 'A' THEN 'BARU' ELSE CASE WHEN Status = 'X' THEN 'GAGAL' ELSE CASE WHEN Status = 'P' THEN 'BERHASIL' END END END Status
     , CreatedDate, UpdatedDate, Header
  from gnDcsUploadFile
 where 1 = 1
   and DataID = @DataID
   and (convert(varchar, CreatedDate, 112) between convert(varchar, @DateFrom, 112) and convert(varchar, @DateTo, 112)) 
   and ProductType in (@ProductType,@ProductType1)
   and Status = 'A'
GO
CREATE PROCEDURE [dbo].[uspfn_spGetDcsUploadFileAllStatus] @ProductType varchar(2), @ProductType1 varchar(2),
   @DataID varchar(6), @DateFrom datetime, @DateTo datetime 
   as
   select ID, DataID, CustomerCode, ProductType, Contents
     , CASE WHEN Status = 'A' THEN 'BARU' ELSE CASE WHEN Status = 'X' THEN 'GAGAL' ELSE CASE WHEN Status = 'P' THEN 'BERHASIL' END END END Status
     , CreatedDate, UpdatedDate, Header
  from gnDcsUploadFile
 where 1 = 1
   and DataID = @DataID
   and (convert(varchar, CreatedDate, 112) between convert(varchar, @DateFrom, 112) and convert(varchar, @DateTo, 112)) 
   and ProductType in (@ProductType,@ProductType1)
GO
CREATE procedure [dbo].[uspfn_spGetMaxQtyReturPenjualan] 
@CompanyCode varchar(15), @BranchCode varchar(15), @PartNo varchar(25), 
@PartNoOriginal  varchar(25), @ReturnNo varchar(15), @DocNo varchar(25), @FPJNo varchar(15)
as
SELECT 
            ISNULL((SELECT QtyBill FROM spTrnSFPJDtl WHERE FPJNo = @FPJNo AND 
                PartNo = @PartNo AND 
                PartNoOriginal = @PartNoOriginal AND
				DocNo = @DocNo AND
                CompanyCode = @CompanyCode AND
                BranchCode = @BranchCode
             ) -
            ISNULL(SUM (QtyReturn),0), 0) AS MaxQtyRetur FROM spTrnSRturdtl
            LEFT JOIN spTrnSRturHdr ON spTrnSRturHdr.ReturnNo = spTrnSRturdtl.ReturnNo AND
                spTrnSRturHdr.CompanyCode = spTrnSRturdtl.CompanyCode AND
                spTrnSRturHdr.BranchCode = spTrnSRturdtl.BranchCode
            WHERE spTrnSRturdtl.CompanyCode = @CompanyCode
                AND spTrnSRturdtl.BranchCode = @BranchCode
                AND spTrnSRturHdr.FPJNo = @FPJNo
                AND spTrnSRturdtl.PartNo = @PartNo
                AND spTrnSRturdtl.PartNoOriginal = @PartNoOriginal
				AND spTrnSRturDtl.DocNo = @DocNo
                AND spTrnSRturHdr.Status NOT IN ('3')
                AND spTrnSRturdtl.ReturnNo <> @ReturnNo
GO
CREATE procedure [dbo].[uspfn_spGetPartReturDetails] @CompanyCode varchar(15), @BranchCode varchar(15), @FPJNo varchar(25),
@TypeOfGoods varchar(3), @ProductType varchar(3)
as
select distinct a.PartNo, a.PartNoOriginal, a.QtyBill, a.DocNo
from	spTrnSFPJDtl a, spTrnSFPJHdr c with(nolock, nowait)
where	a.CompanyCode = @CompanyCode
        and a.BranchCode = @BranchCode
        and c.CompanyCode = @CompanyCode
        and c.BranchCode = @BranchCode
        and a.FPJNo = c.FPJNo
        and a.FPJNo = @FPJNo
		and TypeOfGoods = @TypeOfGoods
		and ProductType = @ProductType
        and (a.QtyBill - 
(select ISNULL(SUM (QtyReturn),0) AS MaxQtyRetur FROM spTrnSRturdtl
LEFT JOIN spTrnSRturHdr ON spTrnSRturHdr.ReturnNo = spTrnSRturdtl.ReturnNo AND
spTrnSRturHdr.CompanyCode = spTrnSRturdtl.CompanyCode AND
spTrnSRturHdr.BranchCode = spTrnSRturdtl.BranchCode
WHERE spTrnSRturdtl.CompanyCode = @CompanyCode
AND spTrnSRturdtl.BranchCode = @BranchCode
AND spTrnSRturHdr.FPJNo = a.FPJNo --''FPJ/08/000003''
AND spTrnSRturdtl.PartNo = a.PartNo --''P002''
AND spTrnSRturdtl.PartNoOriginal = a.PartNoOriginal -- ''Y-001''
AND spTrnSRturDtl.DocNo = a.DocNo --''SOC/08/000095''
AND TypeOfGoods = @TypeOfGoods
AND ProductType = @ProductType
AND spTrnSRturHdr.Status = 2)) > 0
GO
CREATE procedure [dbo].[uspfn_spGetReturnDtlByReturnNo] @CompanyCode varchar(15), @BranchCode varchar(15), @ReturnNo varchar(20)
as
select row_number () OVER (ORDER BY spTrnSRturDtl.CreatedDate ASC) AS NoUrut, 
spTrnSRturDtl.PartNo, spTrnSRturDtl.PartNoOriginal, spTrnSRturDtl.DocNo, spTrnSRturDtl.QtyReturn, spTrnSFPJDtl.QtyBill
                from	spTrnSRturDtl
                left join spTrnSRturHdr on spTrnSRturHdr.CompanyCode = spTrnSRturDtl.CompanyCode and
                    spTrnSRturHdr.BranchCode = spTrnSRturDtl.BranchCode and
                    spTrnSRturHdr.ReturnNo = spTrnSRturDtl.ReturnNo
                left join spTrnSFPJDtl on spTrnSFPJDtl.CompanyCode = spTrnSRturDtl.CompanyCode and
                    spTrnSFPJDtl.BranchCode = spTrnSRturDtl.BranchCode and
                    spTrnSFPJDtl.FPJNo = spTrnSRturHdr.FPJNo and
                    spTrnSFPJDtl.DocNo = spTrnSRturDtl.DocNo and
                    spTrnSFPJDtl.PartNo = spTrnSRturDtl.PartNo and
                    spTrnSFPJDtl.PartNoOriginal = spTrnSRturDtl.PartNoOriginal
                where	spTrnSRturDtl.CompanyCode = @CompanyCode and 
                        spTrnSRturDtl.BranchCode = @BranchCode and
                        spTrnSRturDtl.ReturnNo = @ReturnNo
GO
CREATE procedure [dbo].[uspfn_spGetRturSumAmt] @CompanyCode varchar(15), @BranchCode varchar(15), @ReturnNo varchar(20)
as  
 SELECT	ReturnNo, sum(QtyReturn) as TotReturQty, sum(ReturAmt) as TotReturAmt,
                        sum(DiscAmt) as TotDiscAmt, sum(NetReturAmt) as TotDPPAmt,
                        sum(PPNAmt) as TotPPNAmt, sum(TotReturAmt) as TotFinalReturAmt,
                        sum(CostAmt) as TotCostAmt 
                FROM	spTrnSRturDtl with (nolock, nowait)
                WHERE	CompanyCode = @CompanyCode
                        and BranchCode = @BranchCode
                        and ReturnNo = @ReturnNo
                GROUP BY ReturnNo
GO
create procedure [dbo].[uspfn_spGetTrnSFPJDtl] @CompanyCode varchar(15), @BranchCode varchar(15), @FPJNo varchar(25)
as
SELECT 
 row_number () OVER (ORDER BY spTrnSFPJDtl.CreatedDate ASC) AS NoUrut,
 spTrnSFPJDtl.PartNo,
 spTrnSFPJDtl.PartNoOriginal,
 spTrnSFPJDtl.DocNo,
 CONVERT(VARCHAR, spTrnSFPJDtl.DocDate, 106) AS DocDate,
 spTrnSORDHdr.OrderNo,
 spTrnSORDHdr.OrderDate,
 spTrnSFPJDtl.QtyBill
FROM spTrnSFPJDtl
INNER JOIN spTrnSORDHdr ON spTrnSORDHdr.CompanyCode = spTrnSFPJDtl.CompanyCode
    AND spTrnSORDHdr.BranchCode = spTrnSFPJDtl.BranchCode  
	AND spTrnSORDHdr.DocNo = spTrnSFPJDtl.DocNo
WHERE spTrnSFPJDtl.CompanyCode=@CompanyCode AND 
spTrnSFPJDtl.BranchCode=@BranchCode AND 
FPJNo = @FPJNo
GO
create procedure [dbo].[uspfn_spGetTrnSInvoiceDtl] @CompanyCode varchar(15),
	@BranchCode varchar(15), @InvoiceNo varchar(25)
as
SELECT 
 row_number () OVER (ORDER BY spTrnSInvoiceDtl.CreatedDate ASC) AS NoUrut,
 spTrnSInvoiceDtl.PartNo,
 spTrnSInvoiceDtl.PartNoOriginal,
 spTrnSInvoiceDtl.DocNo,
 CONVERT(VARCHAR, spTrnSInvoiceDtl.DocDate, 106) AS DocDate,
 spTrnSORDHdr.OrderNo,
 spTrnSORDHdr.OrderDate,
 spTrnSInvoiceDtl.QtyBill
FROM spTrnSInvoiceDtl
INNER JOIN spTrnSORDHdr ON spTrnSORDHdr.DocNo = spTrnSInvoiceDtl.DocNo AND
spTrnSORDHdr.CompanyCode = spTrnSInvoiceDtl.CompanyCode AND
spTrnSORDHdr.BranchCode = spTrnSInvoiceDtl.BranchCode
WHERE spTrnSInvoiceDtl.CompanyCode=@CompanyCode AND 
spTrnSInvoiceDtl.BranchCode=@BranchCode AND 
InvoiceNo = @InvoiceNo
GO
CREATE procedure [dbo].[uspfn_spInquiry_ListADO]
	@ParentCode		varchar(100),
	@Detail			int 
AS
Begin

	if (@Detail = 1)
	begin
		select distinct convert(varchar,a.GroupNo) [value], a.Area [text]
		from gnMstDealerMapping a
		where a.isActive = 1
		order by [value] asc
	end
	else if(@Detail = 2)
	begin
		
		declare @JBDTB INT
		SET @JBDTB = 0

		if @ParentCode <> ''
		begin
			if (@ParentCode='JABODETABEK' or @ParentCode = 'CABANG')
				SET @JBDTB = 1
		end 
		ELSE
			SET @ParentCode = '%%'

		IF @JBDTB = 1
		BEGIN
			select distinct a.DealerCode [value], a.DealerName [text], convert(varchar,a.GroupNo) [parent]
			from gnMstDealerMapping a
			where a.Area IN ('JABODETABEK', 'CABANG') and a.isActive = 1
			order by a.DealerCode
		END
		ELSE
			select distinct a.DealerCode [value], a.DealerName [text], convert(varchar,a.GroupNo) [parent]
			from gnMstDealerMapping a
			where (a.Area like @ParentCode) and a.isActive = 1
			order by a.DealerCode
	end
	else if(@Detail = 3)
	begin

		select distinct  b.OutletCode [value], b.OutletName [text],a.DealerCode [parent]
		from gnMstDealerMapping a
		left join gnMstDealerOutletMapping b on a.DealerCode = b.DealerCode
		where (a.DealerCode like case when @ParentCode <> '' then @ParentCode else '%%' end) and a.isActive = 1
		order by b.OutletName
	end
end
GO
-- =============================================
-- Author:		David Leonardo
-- Create date: 6 November 2014
-- Description:	Select Lampiran for Print
-- =============================================
create PROCEDURE [dbo].[uspfn_spLampiranForPrint]
	-- Add the parameters for the stored procedure here
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@JobOrderNo varchar(50)
AS
BEGIN
	SELECT a.LmpNo,a.PickingSlipNo, a.TypeOfGoods, a.TransType,
                SubString(a.TransType,1,1) SalesType,
                (SELECT TOP 1 DocNo FROM SpTrnSPickingDtl 
                    WHERE CompanyCode = a.CompanyCode 
                    AND BranchCode = a.BranchCode 
                    AND PickingSlipNo = a.PickingSlipNo) DocNo 
               FROM spTrnSLmpHdr a WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode AND 
                    a.PickingSlipNo IN (@JobOrderNo)
END
GO
-- =============================================
-- Author:		David Leonardo
-- Create date: 23 October 2014
-- Description:	Log Header
-- Sementara ini baru dipakai di Pembelian - Entry Order
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_spLogHeader]
	(
	@DataID varchar(20),
	@CustomerCode varchar(20),
	@ProductType varchar(20),
	@Status varchar(10),
	@CreatedDate datetime,
	@Header varchar(max)
	)
AS
BEGIN
declare 	
@ID numeric(18,0), 
@Range numeric(18,0)

	 select @ID = (select isnull(max(id), 0) from gnDcsUploadFile), @Range = 9000000000
	 if (@ID < @range)
	 set @ID = @Range + 1
     else
	 set @ID = @ID + 1

     insert into gnDcsUploadFile
           ( ID, DataID, CustomerCode, ProductType, Status, CreatedDate, Header)
     values(@ID,@DataID,@CustomerCode,@ProductType,@Status,@CreatedDate,@Header)
END
GO
-- =============================================
-- Author:		David Leonardo
-- Create date: 6 November 2014
-- Description:	Select Supply Slip for Print
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_spPickingSlipForPrint]
	-- Add the parameters for the stored procedure here
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@ProductType varchar(5),
	@JobOrderNo varchar(50)
AS
BEGIN
	SELECT * INTO #t1 FROM (
                SELECT
                    DISTINCT c.DocNo, c.DocDate, d.PickingSlipNo, e.PartNo, e.PartNo PartNoOri, e.QtySupply, 
                    e.QtyPicked, e.QtyBill, d.Status, f.LookUpValueName TransTypeDesc, c.TransType, g.LmpNo,
                    d.PickedBy
                FROM
                    svTrnService a
                LEFT JOIN svTrnSrvItem b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode 
	                AND b.ProductType = a.ProductType AND b.ServiceNo=a.ServiceNo
                LEFT JOIN spTrnSOrdHdr c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode 
	                AND c.DocNo = b.SupplySlipNo
                LEFT JOIN spTrnSPickingHdr d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode 
	                AND d.PickingSlipNo = c.ExPickingSlipNo
                LEFT JOIN spTrnSPickingDtl e ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode 
	                AND e.PickingSlipNo = d.PickingSlipNo
                LEFT JOIN gnMstLookUpDtl f ON f.CompanyCode = a.CompanyCode AND f.CodeId = 'TTSR' 
                    AND f.LookUpValue = c.TransType
                LEFT JOIN spTrnSLmpHdr g ON g.CompanyCode = a.CompanyCode AND g.BranchCode = a.BranchCode 
                    AND g.PickingSlipNo = d.PickingSlipNo
                WHERE 
                    a.CompanyCode     = @CompanyCode
                    AND a.BranchCode  = @BranchCode
                    AND a.ProductType = @ProductType
                    AND a.jobOrderNo  = @JobOrderNo
                    AND b.SupplySlipNo <> ''
                    AND b.PartSeq = (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode =  @CompanyCode AND BranchCode = @BranchCode
		                   AND ProductType = '4W' AND ServiceNo = a.ServiceNo AND PartNo = b.PartNo)
                    AND d.Status < 2
            )#t1
            
            select a.PickingSlipNo, a.TypeOfGoods, a.TransType, a.SalesType,
            (SELECT TOP 1 DocNo FROM SpTrnSPickingDtl 
                WHERE CompanyCode = a.CompanyCode 
                    AND BranchCode = a.BranchCode 
                    AND PickingSlipNo = a.PickingSlipNo) DocNo 
            from spTrnSpickingHdr a
            where  CompanyCode =  @CompanyCode
				AND BranchCode = @BranchCode
				AND a.pickingSlipNo IN 
                (SELECT DISTINCT PickingSlipNo FROM #t1)
				AND Salestype = 2
            DROP TABLE #t1
END
GO
create procedure [dbo].[uspfn_spPPOSDtl4Table] @CompanyCode varchar(15), @BranchCode varchar(15), @POSNo varchar(25)
as
SELECT 
    row_number () OVER (ORDER BY a.CreatedDate desc) AS NoUrut,
    a.PartNo,
    a.OrderQty, 
    a.DiscPct, 
    a.PurchasePrice, 
    (SELECT PartName FROM spMstItemInfo WHERE PartNo = a.PartNo AND CompanyCode = a.CompanyCode) PartName,
    a.CostPrice, a.ABCClass, a.MovingCode, a.TotalAmount
	,case 
		when isnull(b.LockingBy,'')='' then a.CompanyCode
		else b.LockingBy
	end ShipTo
    , (select TOP 1 isnull(ParaValue, '') from gnMstLookUpDtl where companyCode = @CompanyCode and CodeId = 'PORDS') ParaValue
    ,case 
        when isnull(b.LockingBy,'')='' then a.BranchCode
        else b.LockingBy
	end Dealer
    , a.Note
FROM spTrnPPOSDtl a
	left join gnMstCoProfileSpare b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
WHERE a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.POSNo = @POSNo
GO
CREATE procedure [dbo].[uspfn_spProcessSuggor] 
(  
		@CompanyCode varchar(10),
		@BranchCode varchar(10),
		@MovingCode varchar(5),
		@SupplierCode  varchar(10),
		@TypeOfGoods  varchar(15)
)
 as
SELECT
 a.PartNo
,ISNULL(a.DemandAverage, 0) AS DemandAverage
,ISNULL(c.LeadTime, 0) AS LeadTime
,ISNULL(c.OrderCycle, 0) AS OrderCycle
,ISNULL(c.SafetyStock, 0) AS SafetyStock
,CAST(0 AS int) AS No
,CAST(0 AS Numeric(4,0)) AS SeqNo
,CAST(ISNULL(a.OnHand, 0) - (
    ISNULL(a.AllocationSP, 0) 
    + ISNULL(a.AllocationSL, 0) 
    + ISNULL(a.AllocationSR, 0)
    + ISNULL(a.ReservedSP, 0) 
    + ISNULL(a.ReservedSL, 0) 
    + ISNULL(a.ReservedSR, 0)
) AS decimal(18,2)) AS AvailableQty
,CAST(0 AS Numeric(4,0)) AS SuggorQty
,CAST(0 AS Numeric(4,0)) AS SuggorCorrecQty
,CAST('' AS varchar(3)) AS ProductType
,a.PartCategory
,CAST(0 AS Numeric(18,0)) AS PurchasePrice
,CAST(0 AS Numeric(18,0)) AS CostPrice
,ISNULL(a.OrderPointQty, 0) AS OrderPoint
,ISNULL(a.OnHand, 0) AS OnHand
,ISNULL(a.OnOrder, 0) AS OnOrder
,ISNULL(a.InTransit, 0) AS InTransit
,ISNULL(a.AllocationSP, 0) AS AllocationSP
,ISNULL(a.AllocationSR, 0) AS AllocationSR
,ISNULL(a.AllocationSL, 0) AS AllocationSL
,ISNULL(a.BackOrderSP, 0) AS BackOrderSP
,ISNULL(a.BackOrderSR, 0) AS BackOrderSR
,ISNULL(a.BackOrderSL, 0) AS BackOrderSL
,ISNULL(a.ReservedSP, 0) AS ReservedSP
,ISNULL(a.ReservedSR, 0) AS ReservedSR
,ISNULL(a.ReservedSL, 0) AS ReservedSL
FROM spMstItems a with(nolock, nowait)
INNER JOIN spMstItemInfo b with(nolock, nowait) ON b.CompanyCode=a.CompanyCode AND b.PartNo=a.PartNo
INNER JOIN SpMstOrderParam c with(nolock, nowait) ON c.CompanyCode=a.CompanyCode AND c.BranchCode=a.BranchCode AND 
		   c.SupplierCode=b.SupplierCode AND c.MovingCode=a.MovingCode
WHERE a.CompanyCode=@CompanyCode 
AND a.BranchCode=@BranchCode
AND a.MovingCode=@MovingCode
AND b.SupplierCode=@SupplierCode
AND a.TypeOfGoods=@TypeOfGoods
AND a.Status = '1'
GO
CREATE procedure [dbo].[uspfn_spTrnPPOSHdrView] 
@CompanyCode varchar(10),@BranchCode varchar(10),
@TypeOfGoods varchar(10),
 @Status  int
as               
				
				SELECT a.POSNo, a.PosDate , a.Status ,a.SupplierCode ,b.SupplierName
                FROM spTrnPPOSHdr a
                INNER JOIN gnMstSupplier b ON b.SupplierCode = a.SupplierCode and b.CompanyCode = a.CompanyCode
                WHERE a.CompanyCode=@CompanyCode 
                AND a.BranchCode=@BranchCode
                AND a.TypeOfGoods=@TypeOfGoods
GO
CREATE procedure [dbo].[uspfn_SvGetWRcvClaimDtlFile]
	@CompanyCode  varchar(15),
	@BranchCode   varchar(15),
	@ProductType varchar(15),
	@Reimbursement varchar(15),
	@ReceivedDate datetime,
	@ReceivedDealerCode varchar(15) 	
as
set nocount on
begin
	select 
		a.CompanyCode, a.BranchCode, a.ProductType, a.GenerateNo, a.GenerateSeq
		,a.SuzukiRefferenceNo, a.ReceivedDate, a.DivisionCode, a.JudgementCode
		,a.PaymentOprAmt, a.PaymentOprHour, a.PaymentSubletAmt, a.PaymentSubletHour
		, ActualLaborTime = right('000'+convert(varchar(3),convert(int,a.PaymentOprHour * 10)),3)
		, SubletWorkTime = right('000'+convert(varchar(3),convert(int,a.PaymentSubletHour * 10)),3)
		,(a.PaymentOprAmt + a.PaymentSubletAmt + d.PaymentTotalPrice) TotalClaimAmt
		,d.PaymentTotalPrice PartCost
		,b.SenderDealerCode, b.LotNo 	
		,SUBSTRING(c.IssueNo, 1, PATINDEX('%-%', c.IssueNo) - 1) IssueNo
		, c.ServiceBookNo, c.ChassisCode, c.ChassisNo, c.EngineCode
		,c.EngineNo, c.OperationNo, c.TechnicalModel
	from 
		SvTrnClaimJudgement a
		inner join SvTrnClaim b on a.CompanyCode = b.CompanyCode 
			and a.BranchCode = b.BranchCode and a.ProductType = b.ProductType
			and a.GenerateNo = b.GenerateNo
		inner join SvTrnClaimApplication c on a.CompanyCode = c.CompanyCode
			and a.BranchCode = c.BranchCode and a.ProductType = c.ProductType 
			and a.GenerateNo = c.GenerateNo and a.GenerateSeq = c.GenerateSeq		
		inner join (	
			Select 
				a.CompanyCode, a.BranchCode, a.ProductType, a.GenerateNo, a.GenerateSeq
				,sum(b.PaymentTotalPrice) PaymentTotalPrice
			from 
			SvTrnClaimJudgement a
				inner join SvTrnClaimPart b on a.CompanyCode = b.CompanyCode 
					and a.BranchCode = b.BranchCode and a.ProductType = b.ProductType
					and a.GenerateNo = b.GenerateNo and a.GenerateSeq = b.GenerateSeq
				inner join SvTrnClaim c on a.CompanyCode = c.CompanyCode 
					and a.BranchCode = b.BranchCode and a.ProductType = c.ProductType
					and a.GenerateNo = c.GenerateNo
			where 
				a.CompanyCode = @CompanyCode
				and a.BranchCode = @BranchCode
				and a.ProductType = @ProductType
				and a.SuzukiRefferenceNo = @Reimbursement
				and Convert(varchar, a.ReceivedDate, 110) = @ReceivedDate
				and c.SenderDealerCode = @ReceivedDealerCode			
			group by a.CompanyCode, a.BranchCode, a.ProductType, a.GenerateNo, a.GenerateSeq
		) as d on a.CompanyCode = d.CompanyCode and a.BranchCode = d.BranchCode 
			and a.ProductType = d.ProductType and a.GenerateNo = d.GenerateNo 
			and a.GenerateSeq = d.GenerateSeq
	where 
		a.CompanyCode = @CompanyCode
		and a.BranchCode = @BranchCode
		and a.ProductType = @ProductType
		and a.SuzukiRefferenceNo = @Reimbursement
		and Convert(varchar, a.ReceivedDate, 110) = @ReceivedDate
		and b.SenderDealerCode = @ReceivedDealerCode
end
GO
CREATE procedure [dbo].[uspfn_SvGetWRcvClaimHdrFile]
	@CompanyCode  varchar(15),
	@BranchCode   varchar(15),
	@ProductType varchar(15),
	@DataID varchar(15),
	@ReimbursementNo varchar(15),
	@ReceiveDealerCode varchar(15),
	@ReceiveDate datetime,
	@SenderDealerName varchar(50) 	
as

set nocount on
begin
	select
		RecordID, DataID, DealerCode, ReceivedDealerCode, ReceivedDealerName
		, DealerName, TotalItems = (		
			select 
				count(app.GenerateSeq)	
			from 
				SvTrnClaim cla 
				inner join SvTrnClaimJudgement jud on cla.CompanyCode = jud.CompanyCode 
					and cla.BranchCode = jud.BranchCode and cla.ProductType = jud.ProductType
					and cla.GenerateNo = jud.GenerateNo
				inner join SvtrnClaimApplication app on cla.CompanyCode = app.CompanyCode 
					and cla.BranchCode = app.BranchCode and cla.ProductType = app.ProductType
					and cla.GenerateNo = app.GenerateNo and app.GenerateSeq = jud.GenerateSeq
			where
				cla.CompanyCode = @CompanyCode
				and cla.BranchCode = @BranchCode
				and cla.ProductType = @ProductType
				and jud.SuzukiRefferenceNo = @ReimbursementNo
				and cla.SenderDealerCode = @ReceiveDealerCode
				and Convert(varchar, jud.ReceivedDate , 110) = @ReceiveDate	
		), ProductType = '4W' , ReimbursementNo
		, ReimbursementDate , BlankFiller = ''
	from (
		select TOP 1
			RecordID = 'H'
			, DataID = @DataID
			, DealerCode = a.CompanyCode
			, a.SenderDealerCode ReceivedDealerCode
			, a.SenderDealerName ReceivedDealerName
			, DealerName = @SenderDealerName
			, ProductType = a.ProductType
			, b.SuzukiRefferenceNo ReimbursementNo
			, SuzukiRefferenceDate ReimbursementDate
			, BlankFiller = ''
		from 
			SvTrnClaim a
			inner join SvTrnClaimJudgement b on a.CompanyCode = b.CompanyCode 
				and a.BranchCode = b.BranchCode and a.ProductType = b.ProductType
				and a.GenerateNo = b.GenerateNo
		where
			a.CompanyCode = @CompanyCode
			and a.BranchCode = @BranchCode
			and a.ProductType = @ProductType
			and b.SuzukiRefferenceNo = @ReimbursementNo
			and a.SenderDealerCode = @ReceiveDealerCode
			and Convert(varchar, b.ReceivedDate, 110) = @ReceiveDate	
	) as Header
end
GO
create procedure [dbo].[uspfn_SyncCsCustomerViewInitialize]
as
begin
	;with x as (
	select a.CompanyCode
		 , BranchCode = isnull((
			select top 1 BranchCode from OmTrSalesSo
			 where CompanyCode = a.CompanyCode
			  and CustomerCode = a.CustomerCode
			order by SODate desc), '')
		 , a.CustomerCode
		 , a.CustomerName
		 , a.CustomerType
		 , rtrim(a.Address1) + ' ' + rtrim(a.Address2) + rtrim(a.Address3) as Address
		 , a.PhoneNo
		 , a.HPNo
		 , b.AddPhone1
		 , b.AddPhone2
		 , a.BirthDate
		 , b.ReligionCode
		 , a.CreatedDate
		 , a.LastUpdateDate
	  from GnMstCustomer a
	  left join CsCustData b
		on b.CompanyCode = a.CompanyCode
	   and b.CustomerCode = a.CustomerCode
	 where 1 = 1
	   and a.CustomerType = 'I'
	   and a.BirthDate is not null
	   and a.BirthDate > '1900-01-01'
	   and (year(getdate() - year(a.BirthDate))) > 5
	   and year(a.LastUpdateDate) = year(getdate())
	)
	select * into #t1 from (select * from x where BranchCode != '')#

--	delete CsCustomerView
--	 where exists (
--		select top 1 1 from #t1
--		 where #t1.CompanyCode = CsCustomerView.CompanyCode
--		   and #t1.BranchCode = CsCustomerView.BranchCode
--		   and #t1.CustomerCode = CsCustomerView.CustomerCode
--	 )
--	insert into CsCustomerView (CompanyCode, BranchCode, CustomerCode, CustomerName, CustomerType, Address, PhoneNo, HPNo, AddPhone1, AddPhone2, BirthDate, ReligionCode, CreatedDate, LastUpdateDate)
--	select * from #t1

--	drop table CsCustomerView
	select * into CsCustomerView from #t1


	drop table #t1
end
GO
create procedure [dbo].[uspfn_TaxCheckPendingDocument]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@StartDate varchar(10),
@FPJDate varchar(10),
@IsFPJCentral bit

--declare @CompanyCode as varchar(15)
--declare @BranchCode as varchar(15)
--declare @StartDate as varchar(10)
--declare @FPJDate as varchar(10)
--declare @ProfitCenter as varchar(10)
--declare @IsFPJCentral as bit

--set @CompanyCode = '6006410'
--set @BranchCode  = '600641001'
--set @StartDate  = '20140501'
--set @FPJDate  = '20140513'
--set @IsFPJCentral  = '1'

AS
BEGIN
SELECT InvoiceNo , InvoiceDate 
FROM OmTrSalesInvoice 
WHERE CompanyCode = @CompanyCode 
	AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		)
	AND isStandard = '1' 
	AND isnull ( FakturPajakNo , '' ) = '' 
	AND Status <> '3' 
	AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate 
EXCEPT 
SELECT InvoiceNo , InvoiceDate 
FROM OmFakturPajakHdr 
WHERE CompanyCode = @CompanyCode 
	AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		)
	AND TaxType = 'Standard' 
	AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate 

UNION 

SELECT InvoiceNo , InvoiceDate 
FROM ARTrnInvoiceHdr 
WHERE CompanyCode = @CompanyCode 
	AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		) 
	AND StatusTax = '1' 
	AND isnull ( FPJNo , '' ) = '' 
	AND Status <> '1' 
	AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate 
EXCEPT 
SELECT InvoiceNo , InvoiceDate 
FROM ARFakturPajakHdr 
WHERE CompanyCode = @CompanyCode 
	AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		)
	AND TaxType = 'Standard' 
	AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate
END
GO
create procedure [dbo].[uspfn_TaxCheckPendingDocumentFPJ]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@StartDate varchar(10),
@FPJDate varchar(10),
@IsFPJCentral bit,
@DocNo varchar(max)

--declare @CompanyCode as varchar(15)
--declare @BranchCode as varchar(15)
--declare @StartDate as varchar(10)
--declare @FPJDate as varchar(10)
--declare @ProfitCenter as varchar(10)
--declare @IsFPJCentral as bit
--declare @DocNo as varchar(max)

--set @CompanyCode = '6006410'
--set @BranchCode  = '600641001'
--set @StartDate  = '20140501'
--set @FPJDate  = '20140513'
--set @IsFPJCentral  = '1'
--set @DocNo = ''

as
begin
select x.*
from(
    SELECT	BranchCode,InvoiceNo, convert(varchar,InvoiceDate,112) InvoiceDate
    FROM	OmFakturPajakHdr
    WHERE	CompanyCode = @CompanyCode 
			AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
			AND TaxType = 'Standard'
		    AND CONVERT(VARCHAR, InvoiceDate, 112) >= @StartDate AND CONVERT(VARCHAR, InvoiceDate, 112) < @FPJDate
	    UNION
    SELECT	BranchCode,InvoiceNo, convert(varchar,InvoiceDate,112) InvoiceDate
    FROM	ARFakturPajakHdr
    WHERE	CompanyCode = @CompanyCode 
			AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
			AND TaxType = 'Standard'
		    AND CONVERT(VARCHAR, InvoiceDate, 112) >= @StartDate AND CONVERT(VARCHAR, InvoiceDate, 112) < @FPJDate
	    UNION
    SELECT	BranchCode,InvoiceNo, convert(varchar,InvoiceDate,112) InvoiceDate
    FROM	SPTrnSFPJHdr
    WHERE	CompanyCode = @CompanyCode 
			AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
			AND isPKP = 1
		    AND CONVERT(VARCHAR, InvoiceDate, 112) >= @StartDate AND CONVERT(VARCHAR, InvoiceDate, 112) < @FPJDate
	    UNION
    SELECT	BranchCode,FPJNO AS  InvoiceNo, convert(varchar,FPJDate,112) AS InvoiceDate 
    FROM	SVTrnFakturPajak
    WHERE	CompanyCode = @CompanyCode 
			AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
			AND isPKP = 1
		    AND CONVERT(VARCHAR, FPJDate, 112) >= @StartDate AND CONVERT(VARCHAR, FPJDate, 112) < @FPJDate
		    AND (select TOP 1 TotalSrvAmt 
				   from svTrnInvoice 
				  where CompanyCode = @CompanyCode 
				    AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
							or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
							) 
					and FPJNo = SVTrnFakturPajak.FPJNo) <> 0
    EXCEPT
    SELECT	BranchCode,DocNo AS InvoiceNo, convert(varchar,DocDate,112) AS InvoiceDate 
    FROM	GnGenerateTax
    WHERE	CompanyCode = @CompanyCode 
			AND ((case when @IsFPJCentral = 'True' then BranchCode end) <> ''
				or (case when @IsFPJCentral = 'False' then BranchCode end) in (@BranchCode)
			)
		    AND CONVERT(VARCHAR, DocDate, 112) >= @StartDate AND CONVERT(VARCHAR, DocDate, 112) < @FPJDate
) x
where (case when @DocNo != '' then @DocNo else '' end) not like '%|'+x.BranchCode+' '+x.InvoiceNo+'|%'
end
GO
create procedure [dbo].[uspfn_TaxCheckValidTransaction]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@StartDate varchar(10),
@FPJDate varchar(10),
@IsFPJCentral bit

--declare @CompanyCode as varchar(15)
--declare @BranchCode as varchar(15)
--declare @StartDate as varchar(10)
--declare @FPJDate as varchar(10)
--declare @ProfitCenter as varchar(10)
--declare @IsFPJCentral as bit

--set @CompanyCode = '6006410'
--set @BranchCode  = '600641001'
--set @StartDate  = '20140501'
--set @FPJDate  = '20140513'
--set @IsFPJCentral  = '1'

AS
BEGIN
SELECT TR.* FROM 
( 
    SELECT 
		CompanyCode , BranchCode , '300' AS ProfitCenterCode , InvoiceNo AS DocNo , 
		InvoiceDate AS DocDate , DueDate , FPJNo AS RefNo , FPJDate AS RefDate 
    FROM 
		SPTrnSFPJHdr 
    WHERE 
		CompanyCode = @CompanyCode
		AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
				or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
			)
		AND isPKP = 1 
		AND isnull ( FPJGovNo , '' ) = '' 
		AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate
    UNION 

    SELECT 
		CompanyCode , BranchCode , '200' AS ProfitCenterCode , FPJNo AS DocNo , 
		FPJDate AS DocDate , DueDate , '' AS RefNo , NULL AS RefDate 
    FROM 
		SVTrnFakturPajak 
    WHERE 
		CompanyCode = @CompanyCode
		AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
				or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
			)
		AND isPKP = 1 
		AND isnull ( FPJGovNo , '' ) = '' 
		AND convert ( varchar , FPJDate , 112 ) BETWEEN @StartDate AND @FPJDate
    UNION 

    SELECT 
		CompanyCode , BranchCode , '100' AS ProfitCenterCode , InvoiceNo AS DocNo , 
		InvoiceDate AS DocDate , DueDate , '' AS RefNo , NULL AS RefDate 
    FROM 
		OmFakturPajakHdr 
    WHERE 
		CompanyCode = @CompanyCode
		AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
				or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
			)
		AND TaxType = 'Standard' 
		AND isnull ( FakturPajakNo , '' ) = '' 
		AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate
    UNION 

    SELECT 
		CompanyCode , BranchCode , '000' AS ProfitCenterCode , InvoiceNo AS DocNo , 
		InvoiceDate AS DocDate , DueDate , FPJNo AS RefNo , FPJDate AS RefDate 
    FROM 
		ARFakturPajakHdr 
    WHERE 
		CompanyCode = @CompanyCode
		AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
				or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
			)
		AND TaxType = 'Standard' 
		AND isnull ( FakturPajakNo , '' ) = '' 
		AND convert ( varchar , InvoiceDate , 112 ) BETWEEN @StartDate AND @FPJDate 
) 
AS TR
END
GO
-- =============================================
-- Author:		<yo>
-- Create date: <5 Agust 14>
-- Description:	<Query FPJ Generated>
-- =============================================

CREATE procedure [dbo].[uspfn_TaxQueryFPJGenerated]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@StartDate varchar(10),
@FPJDate varchar(10),
@ProfitCenter varchar(10),
@IsFPJCentral bit

--declare @CompanyCode as varchar(15)
--declare @BranchCode as varchar(15)
--declare @StartDate as varchar(10)
--declare @FPJDate as varchar(10)
--declare @ProfitCenter as varchar(10)
--declare @IsFPJCentral as bit

--set @CompanyCode = '6006410'
--set @BranchCode  = '600641001'
--set @StartDate  = '20140501'
--set @FPJDate  = '20140513'
--set @ProfitCenter  = '300'
--set @IsFPJCentral  = '1'

as
begin

select 
    y.No,y.chkSelect,y.CompanyCode,y.BranchCode,y.ProfitCenter,y.FPJGovNo
    ,isnull(y.FPJGovDate,'')FPJGovDate,y.DocNo, convert(varchar, convert(datetime,y.DocDate), 106) DocDate
from(
    select 
        row_number() over(order by DocDate, BranchCode,ProfitCenter asc) as No
        , Convert(bit, 0) chkSelect, x.CompanyCode, x.BranchCode
        , a.LookUpValueName ProfitCenter, x.FPJGovNo, x.FPJGovDate
        , x.DocNo, x.DocDate
    from (
	    SELECT	CompanyCode, BranchCode,'300' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate
	    FROM	SpTrnSFPJHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 
                AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'200' AS ProfitCenter,FPJGovNo
			    ,NULL AS FPJGovDate,FPJNo as DocNo,convert(varchar,FPJDate,112) AS DocDate
	    FROM	SvTrnFakturPajak
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		        )
                AND isPKP = 1 AND ISNULL(FPJGovNo, '') = ''
			    AND CONVERT(VARCHAR, FPJDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'100' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate
	    FROM	OmFakturPajakHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		        ) 
                AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
	    UNION
	    SELECT	CompanyCode, BranchCode,'000' AS ProfitCenter,FakturPajakNo FPJGovNo
			    ,NULL AS FPJGovDate,InvoiceNo as DocNo,convert(varchar,InvoiceDate,112) AS DocDate
	    FROM	ArFakturPajakHdr
	    WHERE	CompanyCode = @CompanyCode 
                AND ((case when @IsFPJCentral = '1' then BranchCode end) <> ''
			        or (case when @IsFPJCentral = '0' then BranchCode end) in (@BranchCode)
		        )
                AND TaxType = 'Standard' AND ISNULL(FakturPajakNo, '') = ''
			    AND CONVERT(VARCHAR, InvoiceDate, 112) BETWEEN @StartDate AND @FPJDate
                AND DPPAMt > 0
    ) x 
    left join gnMstLookUpDtl a on a.CompanyCode= x.CompanyCode and a.CodeID='PFCN' and a.LookUpValue= x.ProfitCenter
    where x.ProfitCenter like ''+@ProfitCenter+''
) y
end
GO
create procedure [dbo].[uspsys_SaveMessageBoard]
	@MessageID     int,
	@MessageHeader varchar(255),
	@MessageText   varchar(max),
	@MessageTo     varchar(10),
	@MessageTarget varchar(max),
    @MessageParams varchar(50),
	@CreatedBy     varchar(20)
as

insert into SysMessageBoards
select (isnull((select max(MessageID) from SysMessageBoards), 0) + 1)
	 , @MessageHeader, @MessageText, @MessageTo, @MessageTarget, @MessageParams,null,null,@CreatedBy, getdate(), null, null
GO
