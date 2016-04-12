
go
if object_id('uspfn_SelectCustomerSO') is not null
	drop procedure uspfn_SelectCustomerSO

go
create procedure uspfn_SelectCustomerSO
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@UserID varchar(50)
as

begin
	declare @TOPC varchar(5);
	set @TOPC = 'TOPC';

	declare @ProfitCenter varchar(10);
	set @ProfitCenter = (select top 1 ProfitCenter from sysUserProfitCenter where UserId=@UserID);

	SELECT a.CompanyCode
	    , a.CustomerCode
		, a.CustomerName
		, a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4 as Address
		, ISNULL(c.LookUpValueName,'') AS TopCode
		, b.TopCode as TOPCD
		, b.GroupPriceCode
		, d.RefferenceDesc1 as GroupPriceDesc
		, b.SalesCode
	 FROM gnMstCustomer a
	 LEFT JOIN gnMstCustomerProfitCenter b 
       ON b.CompanyCode = a.CompanyCode 
      AND b.CustomerCode = a.CustomerCode
	 LEFT JOIN gnMstLookUpDtl c ON c.CompanyCode=a.CompanyCode
	  AND c.LookUpValue = b.TOPCode    
	  AND c.CodeID = @TOPC
	 LEFT JOIN omMstRefference d ON a.CompanyCode = d.CompanyCode 
	  AND d.RefferenceType = 'GRPR' 
	  AND d.RefferenceCode = b.GroupPriceCode
	WHERE a.CompanyCode = @CompanyCode
	  AND b.BranchCode = @BranchCode
	  AND b.ProfitCenterCode = @ProfitCenter
	  AND a.Status = '1'
	  AND b.SalesType = '1'
	  AND b.isBlackList = 0
	  and a.CustomerCode='200063129'
	ORDER BY CustomerCode
end




go
exec uspfn_SelectCustomerSO '6115204', '611520401', 'ga'


--select * from gnMstCustomer where CustomerName = '200063129'

--select * from gnMstCustomerProfitCenter where CustomerCode='200063129'

--select Status, * from gnMstCustomer where CustomerCode='200063129'