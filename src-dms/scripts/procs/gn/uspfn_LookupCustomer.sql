
go
if object_id('uspfn_LookupCustomer') is not null
	drop procedure uspfn_LookupCustomer

go
create procedure uspfn_LookupCustomer
	@CompanyCode varchar(13),
	@BranchCode varchar(13)
as
begin
	SELECT distinct 
	       a.CustomerCode
		 , a.CustomerName
	     , isnull(a.Address1,'') + ' ' + isnull(a.Address2,'') + ' ' + isnull(a.Address3,'') +' ' + isnull(a.Address4,'') as Address
		 , a.Address1
		 , a.Address2
		 , a.Address3
		 , a.Address4
		 , '' as LookupValue
		 , '' as ProfitCenter
	  FROM gnMstCustomer a with(nolock, nowait)
	  left JOIN gnMstCustomerProfitCenter b with(nolock, nowait)
		ON b.CompanyCode = a.CompanyCode
	   AND b.CustomerCode = a.CustomerCode
	   AND b.BranchCode = @BranchCode
	   AND b.isBlackList = 0
	  left JOIN gnMstLookUpDtl c
		ON c.CompanyCode = a.CompanyCode
	   AND c.CodeID = 'PFCN'
	   AND c.LookupValue = b.ProfitCenterCode
	 WHERE 1 = 1
	   AND a.CompanyCode = @CompanyCode
end