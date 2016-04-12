
go
if object_id('uspfn_CekINVBasedSONo') is not null
	drop procedure uspfn_CekINVBasedSONo

go
create procedure uspfn_CekINVBasedSONo
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@SONumber varchar(50)
as

begin
	  select count(*) as Jumlah 
		from omTrSalesBPK a
	   inner join omTrSalesBPKDetail b
		  on a.CompanyCode = b.CompanyCode
		 and a.BranchCode = b.BranchCode
		 and a.BPKNo = b.BPKNo
	   where a.CompanyCode = @CompanyCode
	     and a.BranchCode  = @BranchCode
		 and a.SONo = @SONumber
		 and b.StatusInvoice = 1
		  and a.Status <> 3;
end