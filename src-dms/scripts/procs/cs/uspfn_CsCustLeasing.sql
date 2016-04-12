
go
if object_id('uspfn_CsCustLeasingList') is not null
	drop procedure uspfn_CsCustLeasingList

go
create procedure uspfn_CsCustLeasingList(
	@CompanyCode varchar(17)
)
as
select 
	text = a.CustomerName,
	value = a.CustomerCode
from 
	gnMstCustomer a
where
	a.CompanyCode=@CompanyCode
	and
	a.CategoryCode='32'
	
go
exec uspfn_CsCustLeasingList @CompanyCode=''