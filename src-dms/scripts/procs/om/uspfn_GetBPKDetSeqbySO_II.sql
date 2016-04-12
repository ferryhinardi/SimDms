

go
if object_id('uspfn_GetBPKDetSeqbySO') is not null
	drop procedure uspfn_GetBPKDetSeqbySO

go
create procedure uspfn_GetBPKDetSeqbySO
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(125)
as
begin
	select a.BPKNo
	     , b.BPKSeq
      from omTrSalesBPK a
     inner join omTrSalesBPKDetail b
        on a.CompanyCode = b.CompanyCode
       and a.BranchCode = b.BranchCode
       and a.BPKNo = b.BPKNo
     where a.CompanyCode = @CompanyCode
       and a.BranchCode  = @BranchCode
       and a.SONo = @SONumber
end