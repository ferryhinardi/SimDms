
go
if object_id('uspfn_CekBPKBasedSONo') is not null
	drop procedure uspfn_CekBPKBasedSONo

go
create procedure uspfn_CekBPKBasedSONo
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@SONumber varchar(50)
as

begin
	  SELECT count(*) as Jumlah
        FROM omTrSalesBPK
       WHERE CompanyCode = @CompanyCode
         AND BranchCode = @BranchCode
         AND SONo = @SONumber 
         AND Status NOT IN (3, 4);
end