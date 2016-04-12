----SP to get CUSTDATA        
CREATE PROCEDURE uspfn_CsCustomerData              
@LastUpdateDate datetime,              
@segment int              
as              
            
--declare @LastUpdateDate datetime              
--set @LastUpdateDate = dateadd(mm,-2,getdate())             
declare @SelectCode varchar(2)             
set @SelectCode = 'A'               
DECLARE @IsDataExist bit              
DECLARE @LastUpdateBy varchar(8)              
SET @LastUpdateBy ='AUTOUPDT'     
declare @DateData datetime             
              
IF(Year(@LastUpdateDate) >= Year(getdate()) and Month(@LastUpdateDate) >= month(getdate()))    
 begin    
  set @LastUpdateDate = getdate()    
 end  
 set @DateData = dateadd(mm,1,(select convert(datetime,dateadd(mm,datediff(mm,0,@LastUpdateDate),0))))  
-- SELECT @LastUpdateDate as LastUpdateDate  
-- select @DateData as  DateData            
          
-- Query all customer with/without transaction              
select * into #t0 from              
        (select distinct c.CustomerCode, p.ProfitCenterCode from gnMstCustomer c, gnMstCustomerProfitCenter p               
           where c.CompanyCode=p.CompanyCode and c.CustomerCode=p.CustomerCode and c.Status=1               
            AND c.CreatedDate < @DateData ) #t0              
select * into #t1 from              
 (select CompanyCode, CompanyName,               
        ( select count(*) from #t0 where ProfitCenterCode='100' ) "TOTALUNIT",              
        ( select count(*) from #t0 where ProfitCenterCode='200' ) "TOTALSERVICE",              
        ( select count(*) from #t0 where ProfitCenterCode='300' ) "TOTALSPAREPART",              
    'All Customer with/without transaction' Notes              
     from gnMstOrganizationHdr)#t1                 
--drop table #t0            
--drop table #t1            
select @IsDataExist = (select count(*) from gnMstCustDealer where Year = Year(@LastUpdateDate) and Month = Month(@LastUpdateDate) and SelectCode = @SelectCode)              
  print @IsDataExist            
IF @IsDataExist = 0              
begin              
 --insert new data to gnMstCustDealer               
 insert INTO gnMstCustDealer (CompanyCode, BranchCode, SelectCode, Year, Month, NoOfUnitService, NoOfUnit, NoOfService, NoOfSparePart, LastUpdateBy, LastUpdateDate)              
 SELECT a.CompanyCode,               
   '' BranchCode,              
   @SelectCode,              
   Year(@LastUpdateDate) Year,              
   Month(@LastUpdateDate) Month,              
   0 NoOfUnitService,              
   a.TotalUnit NoOfUnit,              
   a.TotalService NoOfService,              
   a.TotalSparePart NoOfSparePart,              
   @LastUpdateBy LastUpdateBy,              
   @LastUpdateDate LastUpdateDate              
 from #t1 a              
end              
else               
begin              
 Update gnMstCustDealer              
 set gnMstCustDealer.NoOfService = b.TotalService              
 , gnMstCustDealer.NoOfUnit = b.TotalUnit              
 , gnMstCustDealer.NoOfSparePart =  b.TotalSparePart              
 , gnMstCustDealer.LastUpdateBy = @LastUpdateBy              
 , gnMstCustDealer.LastUpdateDate = @LastUpdateDate              
 from #t1 b              
 where gnMstCustDealer.CompanyCode = b.CompanyCode              
 and gnMstCustDealer.SelectCode = @SelectCode              
 and gnMstCustDealer.Year = year(@LastUpdateDate)               
 AND gnMstCustDealer.Month = month(@LastUpdateDate)              
            
            
end              
              
DROP TABLE #t0, #t1              
              
SELECT CompanyCode, BranchCode, SelectCode, Year, Month, NoOfUnitService, NoOfUnit, NoOfService, NoOfSparePart,              
  LastUpdateBy, LastUpdateDate from gnMstCustDealer              
where SelectCode = @SelectCode and Year = Year(@LastUpdateDate) and Month = Month(@LastUpdateDate)    
    