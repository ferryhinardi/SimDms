go
if object_id('uspfn_GetBPKModelbySO') is not null
	drop procedure uspfn_GetBPKModelbySO

go
create procedure uspfn_GetBPKModelbySO
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(25)
as
begin
	select a.BPKNo
	     , b.SalesModelCode
		 , b.SalesModelYear
      from omTrSalesBPK a
     inner join omTrSalesBPKDetail b
        on a.CompanyCode = b.CompanyCode
       and a.BranchCode = b.BranchCode
       and a.BPKNo = b.BPKNo
     where a.CompanyCode = @CompanyCode
       and a.BranchCode  = @BranchCode
       and a.SONo = @SONumber
end