
go
if object_id('uspfn_OmTrSalesSoVinList') is not null
	drop procedure uspfn_OmTrSalesSoVinList

go
create procedure uspfn_OmTrSalesSoVinList
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(35),
	@SalesModelCode varchar(35),
	@SalesModelYear decimal,
	@ColourCode varchar(35)
as

begin
	select a.*
	  from OmTrSalesSoVin a
end





go
exec uspfn_OmTrSalesSoVinList '6115204', '611520402', '', '', 2011, ''

select * from gnMstCoProfile