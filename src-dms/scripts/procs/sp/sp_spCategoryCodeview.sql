
go
if object_id('sp_spCategoryCodeview') is not null
	drop procedure sp_spCategoryCodeview


go
create procedure sp_spCategoryCodeview 
	@CompanyCode varchar(10)
as
begin
	select 
		a.LookupValue 'CategoryCode', a.LookupValueName 'CategoryName', a.CompanyCode
	from 
		gnMstLookupdtl a
    where 
		a.CodeID='CSCT'  and a.CompanyCode=@CompanyCode
end