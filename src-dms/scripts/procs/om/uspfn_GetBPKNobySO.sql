
go
if object_id('uspfn_GetBPKNobySO') is not null
	drop procedure uspfn_GetBPKNobySO


go
create procedure uspfn_GetBPKNobySO
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(25)
as
begin
	select a.BPKNo
     from omTrSalesBPK a
    where a.CompanyCode = @CompanyCode
      and a.BranchCode  = @BranchCode
      and a.SONo = @SONumber
end