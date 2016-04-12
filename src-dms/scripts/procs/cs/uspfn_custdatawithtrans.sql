--  --SELECT * FROM gnMstCustDealer    
CREATE procedure uspfn_custdatawithtrans    
--uspfn_custdatawithtrans '02-01-2013', 100    
@lastupdatedate datetime,    
@segment int    
as    
--select dateadd(mm,-1,getdate())  
-- declare @LastUpdateDate datetime    
-- set @LastUpdateDate = dateadd(mm,-1,getdate())  
declare @SelectCode varchar(2)    
DECLARE @IsDataExist bit    
DECLARE @LastUpdateBy varchar(8)    
SET @LastUpdateBy ='AUTOUPDT'    
set @SelectCode = 'B'  
declare @DateData datetime    
    
IF(Year(@LastUpdateDate) >= Year(getdate()) and Month(@LastUpdateDate) >= month(getdate()))    
 begin    
  set @LastUpdateDate = getdate()    
 end  
 set @DateData = dateadd(mm,1,(select convert(datetime,dateadd(mm,datediff(mm,0,@LastUpdateDate),0))))  
-- SELECT @LastUpdateDate  
-- select @DateData  
    
-- Query all customer have transaction    
   select * into #t1 from    
        ( select 'UNIT' Kode, a.BranchCode, count(distinct a.CustomerCode) Total from omTrSalesInvoice a    
           where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)    
           AND a.InvoiceDate < @DateData   
           group by a.BranchCode ) #t1    
    
   select * into #t2 from    
        ( select 'SERVICE' Kode, a.BranchCode, count(distinct a.CustomerCode) Total from svTrnInvoice a    
           where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)    
           AND a.InvoiceDate < @DateData   
           group by a.BranchCode ) #t2    
    
   select * into #t3 from    
        ( select 'SPAREPART' Kode, a.BranchCode, count(distinct a.CustomerCode) Total from spTrnSInvoiceHdr a    
           where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)    
           AND a.InvoiceDate < @DateData   
           group by a.BranchCode ) #t3    
    
   select * into #t4 from    
       (( select 'UNIT&SERVICE' Kode, a.BranchCode, a.CustomerCode from omTrSalesInvoice a    
        where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)    
        AND a.InvoiceDate < @DateData   
     union     
     ( select 'UNIT&SERVICE' Kode, a.BranchCode, a.CustomerCode from svTrnInvoice a    
     where exists (select 1 from gnMstCustomer where CompanyCode=a.CompanyCode and CustomerCode=a.CustomerCode)    
     AND a.InvoiceDate < @DateData   
       )))#t4    
    
   select * into #t5 from    
        ( select Kode, BranchCode, count(distinct CustomerCode) Total from #t4    
           group by Kode, BranchCode) #t5    
     
 SELECT * INTO #t6 FROM(    
    select h.CompanyCode, d.BranchCode,    
     isnull(#t5.Total,0) 'TotalUnitService', isnull(#t1.Total,0) 'TotalUnit',     
     isnull(#t2.Total,0) 'TotalService', isnull(#t3.Total,0) 'TotalSparePart'    
   from gnmstOrganizationHdr h    
     inner join gnMstOrganizationDtl d on h.CompanyCode=d.CompanyCode    
      left join #t1 on #t1.BranchCode=d.BranchCode    
      left join #t2 on #t2.BranchCode=d.BranchCode    
      left join #t3 on #t3.BranchCode=d.BranchCode    
      left join #t5 on #t5.BranchCode=d.BranchCode)#t6    
    
    
 DECLARE   @tmpBranchCode      varchar(15)     
 DECLARE   c          CURSOR    
 FOR       SELECT     BranchCode     
     FROM       #t6    
 OPEN      c    
 FETCH     NEXT FROM c     
 INTO      @tmpBranchCode    
 WHILE     @@FETCH_STATUS = 0    
 BEGIN    
    --print @CustDataTrans    
    select @IsDataExist = (select count(*) from gnMstCustDealer where Year = Year(@LastUpdateDate) and Month = Month(@LastUpdateDate) and SelectCode = @SelectCode and BranchCode = @tmpBranchCode)    
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
      a.TotalSparePart NoOfSparePart,    
      @LastUpdateBy LastUpdateBy,    
      @LastUpdateDate LastUpdateDate    
    from #t6 a    
    where a.BranchCode = @tmpBranchCode     
    end    
    else    
    begin    
    Update gnMstCustDealer    
    set gnMstCustDealer.NoOfService = b.TotalService    
    , gnMstCustDealer.NoOfUnit = b.TotalUnit    
    , gnMstCustDealer.NoOfSparePart =  b.TotalSparePart    
    , gnMstCustDealer.NoOfUnitService = b.TotalUnitService    
    , gnMstCustDealer.LastUpdateBy = @LastUpdateBy    
    , gnMstCustDealer.LastUpdateDate = @LastUpdateDate    
    from #t6 b    
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
     
drop table #t1, #t2, #t3, #t4, #t5, #t6    
SELECT CompanyCode, BranchCode, SelectCode, Year, Month, NoOfUnitService, NoOfUnit, NoOfService, NoOfSparePart,    
  LastUpdateBy, LastUpdateDate from gnMstCustDealer    
where SelectCode = @SelectCode and Year = Year(@LastUpdateDate) and Month = Month(@LastUpdateDate)    
  