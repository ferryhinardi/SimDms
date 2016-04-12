alter Procedure uspfn_CsCustomerDataInq
    @SelectCode varchar(2),          
    @Month int,          
    @YearTo int,          
    @YearFrom int,      
    @BranchCode varchar(15),          
    @CompanyCode varchar(15)          
as          
          
       
--declare @SelectCode varchar(2)          
--set @SelectCode = 'A'          
--declare @Month int          
--set @Month = 3          
--declare @YearTo int          
--set @YearTo = 2013          
--declare @YearFrom int          
--set @YearFrom =  2013         
--declare @BranchCode varchar(15)          
--set @BranchCode = '%'          
--DECLARE @CompanyCode varchar(15)      
--SET @CompanyCode = '%'      
          
IF @SelectCode = 'A'          
begin      
 if @YearTo = 0      
 begin      
  select d.ShortName CompanyName, a.CompanyCode , a.CompanyName BranchName,          
   (SELECT ISNULL(b.NoOfUnitService,0) from gnMstCustDealer b          
    where b.Year = @YearFrom          
    and b.Month = @Month          
    AND b.SelectCode = @SelectCode          
    and b.CompanyCode = a.CompanyCode)NoOfUnitService          
    , (SELECT ISNULL(b.NoOfUnit,0) from gnMstCustDealer b          
    where b.Year = @YearFrom          
    AND b.Month = @Month          
    AND b.CompanyCode = a.CompanyCode          
    AND b.SelectCode = @SelectCode) NoOfUnit          
    , (SELECT ISNULL(b.NoOfService,0) from gnMstCustDealer b          
    where b.Year = @YearFrom          
    AND b.Month = @Month          
    AND b.CompanyCode = a.CompanyCode          
    AND b.SelectCode = @SelectCode ) NoOfService          
    , (SELECT ISNULL(b.NoOfSparePart,0) from gnMstCustDealer b          
    where b.Year = @YearFrom          
    AND b.Month = @Month          
    AND b.CompanyCode = a.CompanyCode          
    AND b.SelectCode = @SelectCode ) NoOfSparePart from gnMstOrganizationHdr a          
    join DealerInfo d           
    ON a.CompanyCode = d.DealerCode          
    where a.COmpanyCode like @CompanyCode       
    and d.ProductType = '4W'      
    order BY d.ShortName asc       
  --print @YearFrom    
 end      
 else      
 begin      
  select d.ShortName CompanyName, a.CompanyCode , a.CompanyName BranchName,          
   (SELECT sum(b.NoOfUnitService) from gnMstCustDealer b          
    where b.SelectCode = @SelectCode          
    and b.CompanyCode = a.CompanyCode      
    and b.Year between @YearFrom and @YearTo)NoOfUnitService        
    , (SELECT sum(b.NoOfUnit) from gnMstCustDealer b          
    where b.CompanyCode = a.CompanyCode          
    AND b.SelectCode = @SelectCode      
    and b.Year between @YearFrom and @YearTo ) NoOfUnit          
    , (SELECT sum(b.NoOfService) from gnMstCustDealer b          
    where b.CompanyCode = a.CompanyCode          
    AND b.SelectCode = @SelectCode       
    and b.Year between @YearFrom and @YearTo ) NoOfService          
    , (SELECT sum(b.NoOfSparePart) from gnMstCustDealer b          
    where b.CompanyCode = a.CompanyCode          
    AND b.SelectCode = @SelectCode        
    and b.Year between @YearFrom and @YearTo ) NoOfSparePart from gnMstOrganizationHdr a          
    join DealerInfo d           
    ON a.CompanyCode = d.DealerCode          
    where a.COmpanyCode like @CompanyCode      
    and d.ProductType = '4W'        
    order BY d.ShortName asc      
 end      
end          
else          
begin       
   IF @YearTo = 0      
   begin      
 --select * from gnMstCustDealer          
  select d.ShortName CompanyName, a.CompanyCode , a.CompanyName BranchName,           
  (SELECT ISNULL(b.NoOfUnitService,0) from gnMstCustDealer b          
   where b.Year = @YearFrom          
   and b.Month = @Month          
   AND b.SelectCode = @SelectCode          
   and b.CompanyCode = a.CompanyCode          
   AND b.BranchCode = a.BranchCode)NoOfUnitService          
   , (SELECT ISNULL(b.NoOfUnit,0) from gnMstCustDealer b          
   where b.Year = @YearFrom    
   AND b.Month = @Month          
   AND b.CompanyCode = a.CompanyCode          
   AND b.SelectCode = @SelectCode          
   AND b.BranchCode = a.BranchCode ) NoOfUnit          
   , (SELECT ISNULL(b.NoOfService,0) from gnMstCustDealer b          
   where b.Year = @YearFrom          
   AND b.Month = @Month          
   AND b.CompanyCode = a.CompanyCode          
   AND b.SelectCode = @SelectCode          
   AND b.BranchCode = a.BranchCode ) NoOfService          
   , (SELECT ISNULL(b.NoOfSparePart,0) from gnMstCustDealer b          
   where b.Year = @YearFrom          
   AND b.Month = @Month          
   AND b.CompanyCode = a.CompanyCode          
   AND b.SelectCode = @SelectCode          
   AND b.BranchCode = a.BranchCode ) NoOfSparePart from gnMstCoProfile a          
   join gnMstOrganizationHdr b          
   ON a.CompanyCode = b.CompanyCode          
   join DealerInfo d           
   ON a.CompanyCode = d.DealerCode          
   where a.BranchCode like @BranchCode          
   and a.CompanyCode like @CompanyCode       
   and d.ProductType = '4W'      
   order BY d.ShortName asc         
    end       
    else      
    begin      
    select d.ShortName CompanyName, a.CompanyCode , a.CompanyName BranchName,           
   (SELECT Sum(b.NoOfUnitService) from gnMstCustDealer b          
    where b.SelectCode = @SelectCode          
    and b.CompanyCode = a.CompanyCode          
    AND b.BranchCode = a.BranchCode      
    and b.Year between @YearFrom and @YearTo )NoOfUnitService          
    , (SELECT Sum(b.NoOfUnit) from gnMstCustDealer b          
    where b.CompanyCode = a.CompanyCode          
    AND b.SelectCode = @SelectCode          
    AND b.BranchCode = a.BranchCode       
    and b.Year between @YearFrom and @YearTo ) NoOfUnit          
    , (SELECT Sum(b.NoOfService) from gnMstCustDealer b          
    where b.CompanyCode = a.CompanyCode          
    AND b.SelectCode = @SelectCode          
    AND b.BranchCode = a.BranchCode       
    and b.Year between @YearFrom and @YearTo ) NoOfService          
    , (SELECT Sum(b.NoOfSparePart) from gnMstCustDealer b          
    where b.CompanyCode = a.CompanyCode          
    AND b.SelectCode = @SelectCode          
    AND b.BranchCode = a.BranchCode       
    and b.Year between @YearFrom and @YearTo ) NoOfSparePart from gnMstCoProfile a          
    join gnMstOrganizationHdr b          
    ON a.CompanyCode = b.CompanyCode          
    join DealerInfo d           
    ON a.CompanyCode = d.DealerCode          
    where a.BranchCode like @BranchCode          
    and a.CompanyCode like @CompanyCode       
    and d.ProductType = '4W'      
    order BY d.ShortName asc       
    end      
end          
          
if @CompanyCode  = '%'          
begin      
 select 'ALL' Company, case when @BranchCode = '%' then 'ALL'  else d.CompanyName end Branch,          
 case when @YearTo = 0 then cast(@Month AS VARCHAR(2))+'/'+ cast(@YearFrom as Varchar(4)) else 'ALL PERIOD' end PeriodeStart,          
 CASE WHEN @SelectCode = 'A' THEN 'Customer Data' WHEN @SelectCode = 'B' THEN 'Customer Data with transaction' WHEN @SelectCode = 'C' THEN 'Customer Suzuki' END InquiryType          
 from gnMstCoProfile d          
end      
else      
begin      
 select h.CompanyName Company, case when @BranchCode = '%' then 'ALL'  else d.CompanyName end Branch,          
 case when @YearTo = 0 then cast(@Month AS VARCHAR(2))+'/'+ cast(@YearFrom as Varchar(4)) else 'ALL PERIOD' end PeriodeStart,          
 CASE WHEN @SelectCode = 'A' THEN 'Customer Data' WHEN @SelectCode = 'B' THEN 'Customer Data with transaction' WHEN @SelectCode = 'C' THEN 'Customer Suzuki' END InquiryType          
 from gnMstCoProfile d          
 join gnMstOrganizationHdr h ON d.CompanyCode = h.CompanyCode          
 where d.BranchCode like @BranchCode  
 and d.CompanyCode like @CompanyCode         
end      
      
--select * from gnMstCoProfile 