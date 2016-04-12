  
--DECLARE @as datetime    
--SET @as = getdate()    
--exec uspfn_CsCustomerSuzukiData @as, 100    
CREATE PROCEDURE uspfn_CsCustomerSuzukiData    
--uspfn_CsCustomerSuzukiData '02-01-2013', 100    
@LastUpdateDate datetime,    
@segment int    
as    
--declare @LastUpdateDate datetime    
--set @LastUpdateDate = dateadd(mm,-0,getdate())  
declare @SelectCode varchar(2)    
DECLARE @IsDataExist bit    
DECLARE @LastUpdateBy varchar(10)    
SET @LastUpdateBy ='AUTOUPDT'    
set @SelectCode = 'C'   
declare @DateData datetime             
              
IF(Year(@LastUpdateDate) >= Year(getdate()) and Month(@LastUpdateDate) >= month(getdate()))    
 begin    
  set @LastUpdateDate = getdate()    
 end  
 set @DateData = dateadd(mm,1,(select convert(datetime,dateadd(mm,datediff(mm,0,@LastUpdateDate),0))))  
--SELECT @LastUpdateDate as LastUpdateDate  
--select @DateData as  DateData     
    
     
-- Query all Suzuki customer (3 digits VIN : JS2, JS3, JS4, JSA, MHY, MA3 & MMS)    
   select * into #t11 from    
        ( select 'UNIT SUZUKI' Kode, a.BranchCode, count(distinct a.CustomerCode) Total from omTrSalesInvoice a    
     where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)    
             and exists (select top 1 1 from omTrSalesInvoiceVIN    
                          where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and InvoiceNo=a.InvoiceNo    
                            and substring(ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS'))    
             AND a.InvoiceDate < @DateData   
           group by a.BranchCode) #t11    
    
   select * into #t8 from    
        ( select 'SERVICE SUZUKI' Kode, BranchCode, count(distinct a.CustomerCode) Total from svTrnInvoice a    
     where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)    
             and substring(a.ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS')    
             AND a.InvoiceDate < @DateData   
           group by a.BranchCode) #t8    
    
   select * into #t9 from    
       (( select 'UNIT&SERVICE' Kode, a.BranchCode, a.CustomerCode from omTrSalesInvoice a    
        where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)    
             and exists (select top 1 1 from omTrSalesInvoiceVIN     
           where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and InvoiceNo=a.InvoiceNo    
                            and substring(ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS'))    
             AND a.InvoiceDate < @DateData )    
    union     
     ( select 'UNIT&SERVICE' Kode, a.BranchCode, a.CustomerCode from svTrnInvoice a    
     where exists (select 1 from gnMstCustomer     
               where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode    
                            and substring(a.ChassisCode,1,3) in ('JS2','JS3','JS4','JSA','MHY','MA3','MMS'))    
           AND a.InvoiceDate < @DateData )     
       )  #t9    
    
   select * into #t10 from    
        ( select Kode, BranchCode, count(distinct CustomerCode) Total from #t9    
           group by Kode, BranchCode) #t10    
               
   select * INTO #t12 FROM(    
   select h.CompanyCode, d.BranchCode,    
       isnull(#t10.Total,0) 'TotalUnitService', isnull(#t11.Total,0) 'TotalUnit',     
    isnull(#t8.Total,0) 'TotalService'    
     from gnmstOrganizationHdr h    
          inner join gnMstOrganizationDtl d on h.CompanyCode=d.CompanyCode    
           left join #t11 on #t11.BranchCode=d.BranchCode    
           left join #t8 on #t8.BranchCode=d.BranchCode    
           left join #t10 on #t10.BranchCode=d.BranchCode)#t12    
     
 DECLARE   @tmpBranchCode      varchar(15)     
 DECLARE   c          CURSOR    
 FOR       SELECT     BranchCode     
     FROM       #t12    
 OPEN      c    
 FETCH     NEXT FROM c     
 INTO      @tmpBranchCode    
 WHILE     @@FETCH_STATUS = 0    
 BEGIN    
    --print @CustDataTrans        select @IsDataExist = (select count(*) from gnMstCustDealer where Year = Year(@LastUpdateDate) and Month = Month(@LastUpdateDate) and SelectCode = @SelectCode and BranchCode = @tmpBranchCode)    
    IF @IsDataExist = 0    
    begin     
    insert INTO gnMstCustDealer (CompanyCode, BranchCode, SelectCode, Year, Month, NoOfUnitService, NoOfUnit, NoOfService, NoOfSparePart, LastUpdateBy, LastUpdateDate)    
    SELECT a.CompanyCode CompanyCode,     
      @tmpBranchCode BranchCode,    
      @SelectCode SelectCode,    
      year(@LastUpdateDate) Year,    
      Month(@LastUpdateDate) Month,    
      a.TotalUnitService NoOfUnitService,    
      a.TotalUnit NoOfUnit,    
      a.TotalService NoOfService,    
      0 NoOfSparePart,    
      @LastUpdateBy LastUpdateBy,    
      @LastUpdateDate LastUpdateDate    
    from #t12 a    
    where a.BranchCode = @tmpBranchCode     
    end    
    else    
    begin    
    Update gnMstCustDealer    
    set gnMstCustDealer.NoOfService = b.TotalService    
    , gnMstCustDealer.NoOfUnit = b.TotalUnit    
    , gnMstCustDealer.NoOfUnitService = b.TotalUnitService    
    , gnMstCustDealer.LastUpdateBy = @LastUpdateBy    
    , gnMstCustDealer.LastUpdateDate = @LastUpdateDate    
    from #t12 b    
    join gnMstCustDealer on b.CompanyCode = gnMstCustDealer.CompanyCode    
    and b.BranchCode = gnMstCustDealer.BranchCode    
    and gnMstCustDealer.SelectCode = @SelectCode    
    and gnMstCustDealer.Year =  Year(@LastUpdateDate)    
    and gnMstCustDealer.Month = Month(@LastUpdateDate)    
    where  gnMstCustDealer.BranchCode = @tmpBranchCode and gnMstCustDealer.SelectCode = @SelectCode    
    and gnMstCustDealer.Year =  Year(@LastUpdateDate) and gnMstCustDealer.Month = Month(@LastUpdateDate)    
    end    
    FETCH NEXT from c INTO @tmpBranchCode    
 END    
 CLOSE      c    
 DEALLOCATE c    
   drop table #t11, #t8, #t9, #t10, #t12    
       
       
SELECT CompanyCode, BranchCode, SelectCode, Year, Month, NoOfUnitService, NoOfUnit, NoOfService, NoOfSparePart,    
  LastUpdateBy, LastUpdateDate from gnMstCustDealer    
where SelectCode = @SelectCode and Year = Year(@LastUpdateDate) and Month = Month(@LastUpdateDate)  
  