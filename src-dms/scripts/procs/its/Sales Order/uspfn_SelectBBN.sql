if object_id('uspfn_SelectBNN') is not null
	drop procedure uspfn_SelectBNN

go
create procedure uspfn_SelectBNN
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@SalesModelCode varchar(25),
	@SalesModelYear int,
	@UserID varchar(25)
as

begin
	declare @ProfitCenterCode varchar(25);
	set @ProfitCenterCode = (select top 1 a.ProfitCenter from sysUserProfitCenter a where a.UserId = @UserID);

    SELECT DISTINCT 
	       a.SupplierCode
		 , a.SupplierName 
      FROM gnMstSupplier a, gnMstSupplierProfitCenter b, omMstBBNKIR c
     WHERE a.CompanyCode = b.CompanyCode
       AND b.CompanyCode = c.CompanyCode
       AND a.SupplierCode = b.SupplierCode
       AND b.SupplierCode = c.SupplierCode
       AND a.CompanyCode = @CompanyCode
       AND c.BranchCode = @BranchCode
       AND b.ProfitCenterCode = @ProfitCenterCode
       AND c.Status = '1'
       AND c.SalesModelCode = @SalesModelCode
       AND c.SalesModelYear = @SalesModelYear
     ORDER BY a.SupplierCode ASC
end