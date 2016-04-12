
go
if object_id('uspfn_spMstCompanyAccount') is not null
	drop procedure uspfn_spMstCompanyAccount

go
create procedure uspfn_spMstCompanyAccount (  
	@CompanyCode varchar(10), 
    @Search varchar(50) = ''
)
as
begin
  IF @Search <> ''
	 select 
		CompanyCode,
		CompanyCodeTo,
		CompanyCodeToDesc,
		BranchCodeTo,
		BranchCodeToDesc,
		WarehouseCodeTo,
		WarehouseCodeToDesc,
		UrlAddress,
		isActive
		from spMstCompanyAccount
		where CompanyCode= @CompanyCode
		and (
		CompanyCodeTo like '%' + @Search + '%' or
		CompanyCodeToDesc like '%' + @Search + '%' or
		BranchCodeTo like '%' + @Search + '%' or
		BranchCodeToDesc like '%' + @Search + '%' or
		WarehouseCodeTo like '%' + @Search + '%' or
		WarehouseCodeToDesc like '%' + @Search + '%' 

		)
  else
		select 
		CompanyCode,
		CompanyCodeTo,
		CompanyCodeToDesc,
		BranchCodeTo,
		BranchCodeToDesc,
		WarehouseCodeTo,
		WarehouseCodeToDesc,
		UrlAddress,
		isActive
		from spMstCompanyAccount
		where CompanyCode= @CompanyCode
end