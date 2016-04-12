
go
if object_id('uspfn_GetDONobySO') is not null
	drop procedure uspfn_GetDONobySO

go
create procedure uspfn_GetDONobySO
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(25)
as

begin
	 select a.DoNo
       from omTrSalesDO a
      where a.CompanyCode = @CompanyCode
        and a.BranchCode  = @BranchCode
        and a.SONo = @SONumber
end