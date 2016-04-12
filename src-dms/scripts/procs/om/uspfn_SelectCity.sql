if object_id('uspfn_SelectCity') is not null
	drop procedure uspfn_SelectCity

go
create procedure uspfn_SelectCity
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@SalesModelCode varchar(25),
	@SalesModelYear int,
	@SupplierCode varchar(25)
as

begin
	SELECT DISTINCT 
	       a.LookUpValue as CityCode
		 , a.LookUpValueName as CityName
		 , b.BBN
		 , b.KIR
	  FROM gnMstLookUpDtl a
	     , omMstBBNKIR b
	 WHERE a.CompanyCode = b.CompanyCode
	   AND a.CompanyCode = @CompanyCode
	   AND a.LookUpValue = b.CityCode
	   AND b.BranchCode = @BranchCode
	   AND a.CodeID = 'CITY'
	   AND b.Status = '1'
	   AND b.SupplierCode = @SupplierCode
	   AND b.SalesModelCode = @SalesModelCode
	   AND b.SalesModelYear = @SalesModelYear
	 ORDER BY a.LookUpValue
end