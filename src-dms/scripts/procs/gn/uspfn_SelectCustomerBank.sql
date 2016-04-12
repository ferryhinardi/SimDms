
go
if object_id('uspfn_SelectCustomerBank') is not null
	drop procedure uspfn_SelectCustomerBank

go
create procedure uspfn_SelectCustomerBank
	@CompanyCode varchar(13),
	@CustomerCode varchar(25)
as
begin
	if @CustomerCode = '' 
		return

	select a.*
	  from gnMstCustomerBank a
	 where a.CompanyCode = @CompanyCode
	   and a.CustomerCode = @CustomerCode
end


--select * from gnMstCustomerProfitCenter where CustomerCode=''


go

select * from gnmstCustomerBank

