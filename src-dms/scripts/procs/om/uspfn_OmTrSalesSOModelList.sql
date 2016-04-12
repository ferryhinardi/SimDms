if object_id('uspfn_OmTrSalesSOModelList') is not null
	drop procedure uspfn_OmTrSalesSOModelList

go
create procedure uspfn_OmTrSalesSOModelList
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(50)
	--@GroupPriceCode varchar(25)
as

begin
	select a.*
	  from omTrSalesSOModel a
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and a.SONo = @SONumber
end



go
exec uspfn_OmTrSalesSOModelList '6115204', '611520402', 'SOC/11/000001'

