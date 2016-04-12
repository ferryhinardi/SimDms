-- =============================================
-- Author:		David
-- Create date: 27 January 2016
-- Description:	Recount Stock Value / month
-- =============================================
create procedure sp_RecountStockValue

as
begin
select CompanyCode,BranchCode,PeriodYear,PeriodMonth,PeriodWeek, TypeofGoods, isnull(sum(NilaiStock),0) NilaiStock into #t_items from(
select  i.CompanyCode,i.BranchCode, year(getdate()) PeriodYear, month(getdate()) PeriodMonth,  
case when DAY(getdate()) <= 7 then 1 
     when DAY(getdate()) between 8 and 15 then 2 
	 when DAY(getdate()) between 16 and 22 then 3 
	 when DAY(getdate()) >= 23 then 4
end PeriodWeek, i.TypeOfGoods, (i.OnHand * ip.CostPrice) NilaiStock from spmstitems i
	join spMstItemPrice ip
	on i.CompanyCode = ip.CompanyCode and i.BranchCode = ip.BranchCode and i.PartNo = ip.PartNo
	) #t_items
	group by CompanyCode, BranchCode,PeriodYear, PeriodMonth, PeriodWeek, TypeOfGoods

	insert into spHstSparePartWeeklyStock select * , 'system' as CreatedBy, getdate() as CreatedDate, 'system' as LastUpdateBy, getdate() as LastUpdateDateFrom from #t_items 

	drop table #t_items

end