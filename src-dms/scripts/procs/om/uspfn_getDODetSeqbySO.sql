go 
if object_id('uspfn_getDODetSeqbySO') is not null
	drop procedure uspfn_getDODetSeqbySO

go
create procedure uspfn_getDODetSeqbySO
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(25)
as
begin
	select a.DONo
	     , b.DOSeq
      from omTrSalesDO a
     inner join omTrSalesDODetail b
        on a.CompanyCode = b.CompanyCode
       and a.BranchCode = b.BranchCode
       and a.DONo = b.DONo
     where a.CompanyCode = @CompanyCode
       and a.BranchCode  = @BranchCode
       and a.SONo = @SONumber
end


