
go
if object_id('uspfn_CekDOBasedSONo') is not null 
	drop procedure uspfn_CekDOBasedSONo

go
create procedure uspfn_CekDOBasedSONo
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@SONumber varchar(50)
as

begin
	  SELECT count(DONo) as Jumlah
	    FROM OmTrSalesDO
	   WHERE CompanyCode = @CompanyCode
         AND BranchCode  = @BranchCode
         AND SONo        = @SONumber 
         AND Status NOT IN (3, 4);
end