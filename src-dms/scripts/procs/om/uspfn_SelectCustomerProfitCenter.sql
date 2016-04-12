

go
if object_id('uspfn_SelectCustomerProfitCenter') is not null
	drop procedure uspfn_SelectCustomerProfitCenter

go
create procedure uspfn_SelectCustomerProfitCenter
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@CustomerCode varchar(25)
as
begin
	declare @CodeID varchar(13);
	set @CodeID = 'PFCN';

	if @CustomerCode = '' return

	SELECT a.*
	     , ProfitCenterName = (
				select top 1
				       x.LookUpValueName
				  from gnMstLookUpDtl x
				 where x.CompanyCode = @CompanyCode
				   and x.CodeID = 'PFCN'
				   and x.LookUpValue = a.ProfitCenterCode
		   )
		 , CustomerClassName = (
				select top 1
				       x.CustomerClassName
				  from GnMstCustomerClass x
				 where x.CompanyCode = @CompanyCode
				   and x.ProfitCenterCode = a.ProfitCenterCode
		   )
		 , TaxDesc = (
				select top 1
				       x.Description
				  from gnMstTax x
				 where x.CompanyCode = @CompanyCode
				   and x.TaxCode = a.TaxCode
		   )
		 , CollectorName = (
				select top 1
				       x.CollectorName
				  from gnMstCollector x
				 where x.CompanyCode = @CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.ProfitCenterCode = a.ProfitCenterCode
				   and x.CollectorCode = a.CollectorCode
		   )
	     , TaxTransDesc = (
				select top 1
				       x.LookUpValueName
				  from gnMstLookUpDtl x
				 where x.CompanyCode = @CompanyCode
				   and x.CodeID = 'TRPJ'
				   and x.LookUpValue = a.TaxTransCode
		   )
	     , SalesmanName = (
				select top 1
				       x.EmployeeName
				  from HrEmployee x
				 where x.CompanyCode = @CompanyCode
				   and x.EmployeeID = a.Salesman
		   )
		 , a.SalesCode as KelAR
	     , KelARDesc = (
				select top 1
				       x.LookUpValueName
				  from gnMstLookUpDtl x
				 where x.CompanyCode = @CompanyCode
				   and x.CodeID = 'GPAR'
				   and x.LookUpValue = a.SalesCode
		   )
	     , CustomerGradeName = (
				select top 1
				       x.LookUpValueName
				  from gnMstLookUpDtl x
				 where x.CompanyCode = @CompanyCode
				   and x.CodeID = 'CSGR'
				   and x.LookUpValue = a.CustomerGrade
		   )
		 --, b.LookUpValueName as ProfitCenterName
		 , c.CustomerGovName
		 , GroupPrice = a.GroupPriceCode
		 , GroupPriceDesc = ( 
				select top 1
					   x.RefferenceDesc1
				  from omMstRefference x
				 where x.CompanyCode = a.CompanyCode
				   and x.RefferenceType = 'GRPR'
				   and x.RefferenceCode = a.GroupPriceCode
		   )
		 --, a.CustomerClass
		 --, a.ContactPerson 
	  FROM gnMstCustomerProfitCenter a
	  LEFT JOIN GnMstLookupDtl b 
	    ON b.CompanyCode=a.CompanyCode 
	   AND b.LookUpValue  = a.ProfitCenterCode
	  LEFT JOIN gnMstCustomer c ON c.CompanyCode = a.CompanyCode 
	   AND c.CustomerCode = a.CustomerCode
	 WHERE a.CompanyCode  = @CompanyCode 
	   AND a.BranchCode   = @BranchCode
	   AND a.CustomerCode = @CustomerCode
	   AND b.CodeID       = @CodeID
end


--select * from gnMstCustomerProfitCenter where CustomerCode=''



GO


