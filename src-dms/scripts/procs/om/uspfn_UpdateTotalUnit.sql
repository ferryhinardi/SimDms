
go
if object_id('uspfn_UpdateTotalUnit') is not null
	drop procedure uspfn_UpdateTotalUnit

go
create procedure uspfn_UpdateTotalUnit
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@SONumber varchar(25),
	@SalesModelCode varchar(25),
	@SalesModelYear decimal	
as

begin
	declare @sum decimal
	set @sum = (select isnull(sum(quantity),0) from omtrsalessomodelcolour 
				where companycode=@CompanyCode and branchcode=@BranchCode and sono=@SONumber)
	
	update omTrSalesSOModel
	   set QuantitySO = @sum
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and SONo = @SONumber
	   and SalesModelCode = @SalesModelCode
	   and SalesModelYear = @SalesModelYear

	if (@sum=0)
	begin
		delete omtrsalessoaccsseq where companycode=@CompanyCode and branchcode=@BranchCode and sono=@SONumber 
	end
	else
	begin
		update omtrsalessoaccsseq 
		   set demandqty=qty*@sum
		 where companycode=@CompanyCode and branchcode=@BranchCode and sono=@SONumber and partseq=1 
	end
end

