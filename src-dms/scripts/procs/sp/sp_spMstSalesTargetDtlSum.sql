

go
if object_id('sp_spMstSalesTargetDtlSum') is not null
	drop procedure sp_spMstSalesTargetDtlSum

go
create procedure sp_spMstSalesTargetDtlSum  (  
	@CompanyCode varchar(10) ,
	@BranchCode varchar(10),
	@Year Varchar(4),
	@Month varchar(2))
as
begin
	select 
		sum(a.QtyTarget) as sumQtyTarget,
		sum(a.AmountTarget) as sumAmountTarget,
		sum(a.TotalJaringan) as sumTotalJaringan
	from spMstSalesTargetDtl a
   WHERE a.CompanyCode=@CompanyCode and  a.BranchCode=@BranchCode
	 and a.[Year]=@Year and a.[Month]=@Month
end