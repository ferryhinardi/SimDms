go
alter procedure uspfn_SelectLeasing
	@CompanyCode varchar(17),
	@BranchCode varchar(17),
	@UserID varchar(17)
as

begin
	declare @ProfitCenterCode varchar(10);
	set @ProfitCenterCode = (select top 1 ProfitCenter from sysUserProfitCenter where UserId=@UserID);

	SELECT a.CustomerCode as LeasingCode
	     , a.CustomerName as LeasingName
      FROM gnMstCustomer a 
	 INNER JOIN gnMstCustomerProfitCenter b
        ON a.CompanyCode = b.CompanyCode AND a.CustomerCode = b.CustomerCode
     WHERE a.CompanyCode = @CompanyCode 
       AND b.BranchCode = @BranchCode
       AND b.ProfitCenterCode = @ProfitCenterCode
       AND a.CategoryCode = 32;
end




go
exec uspfn_SelectLeasing '6115204', '611520400', 'huda'
