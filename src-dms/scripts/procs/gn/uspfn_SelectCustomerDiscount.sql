
go
if object_id('uspfn_SelectCustomerDiscount') is not null
	drop procedure uspfn_SelectCustomerDiscount

go
create procedure uspfn_SelectCustomerDiscount
	@CompanyCode varchar(13),
	@BranchCode varchar(13),
	@CustomerCode varchar(25)
as
begin
	if @CustomerCode = '' 
		return

	select a.*
	     , ProfitCenterCodeDisc = a.ProfitCenterCode
		 , ProfitCenterNameDisc = (
				select top 1
				       x.LookUpValueName
				  from gnMstLookUpDtl x
				 where x.CompanyCode = a.CompanyCode
				   and x.LookUpValue = a.ProfitCenterCode
				   and x.CodeID = 'PFCN'
				 
		   )
		 , TypeOfGoodsDisc = a.TypeOfGoods
		 , TypeOfGoodsNameDisc = (
				select top 1
				       x.LookUpValueName
				  from gnMstLookUpDtl x
				 where x.CompanyCode = a.CompanyCode
				   and x.LookUpValue = a.TypeOfGoods
				   and x.CodeID = 'TPGO'
		   )
		 , DiscPctDisc = a.DiscPct
	  from gnMstCustomerDisc a
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and a.CustomerCode = @CustomerCode
end


--select * from gnMstCustomerProfitCenter where CustomerCode=''


go

