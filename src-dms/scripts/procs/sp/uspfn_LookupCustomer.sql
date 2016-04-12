alter procedure uspfn_LookupCustomer (  @CompanyCode varchar(10) ,@BranchCode varchar(10),@ProfitCenterCode varchar(10))
 as
SELECT  distinct a.CustomerCode, a.CustomerName
     , a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 +' ' + a.Address4 as Address
	  , a.Address1 , a.Address2, a.Address3 , a.Address4 
	 , c.LookupValue, c.LookUpValueName as ProfitCenter

  FROM gnMstCustomer a with(nolock, nowait)
 INNER JOIN gnMstCustomerProfitCenter b with(nolock, nowait)
	ON b.CompanyCode = a.CompanyCode
   AND b.CustomerCode = a.CustomerCode
 INNER JOIN gnMstLookUpDtl c
	ON c.CompanyCode = a.CompanyCode
   AND c.CodeID = 'PFCN'
   AND c.LookupValue = b.ProfitCenterCode
 WHERE  a.CompanyCode = @CompanyCode
   AND b.BranchCode = @BranchCode
   and b.ProfitCenterCode=@ProfitCenterCode
 