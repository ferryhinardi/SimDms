
if object_id('sp_spMstSalesTargetDtl') is not null
	drop procedure sp_spMstSalesTargetDtl

go
create procedure sp_spMstSalesTargetDtl  (  
	@CompanyCode varchar(10),
	@BranchCode varchar(10),
	@Year Varchar(4),
	@Month varchar(2))
as
begin
	select 
		a.CompanyCode,
		a.BranchCode,
		a.[Year],
		a.[Month],
		a.CategoryCode,
		b.LookUpValueName  CategoryName,
		a.QtyTarget,
		a.AmountTarget,
		a.TotalJaringan
	from spMstSalesTargetDtl a
   inner join gnMstLookUpDtl b 
      on a.CompanyCode=b.CompanyCode 
	 and a.CategoryCode=b.LookUpValue
	WHERE  b.CodeID='CSCT' and a.CompanyCode=@CompanyCode and  a.BranchCode=@BranchCode
	  and a.[Year]=@Year and a.[Month]=@Month
end