
go
if object_id('uspfn_GetInvoiceNobySO') is not null
	drop procedure uspfn_GetInvoiceNobySO

go
create procedure uspfn_GetInvoiceNobySO
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(25)
as
begin
	select *
      from omTrSalesInvoice a
     where a.CompanyCode = @CompanyCode
       and a.BranchCode  = @BranchCode
       and a.SONo = @SONumber
end