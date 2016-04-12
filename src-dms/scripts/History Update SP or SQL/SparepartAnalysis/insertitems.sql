-- start on first day on new month


select CompanyCode,BranchCode,PeriodYear,PeriodMonth,PeriodWeek, TypeofGoods, isnull(sum(NilaiStock),0) NilaiStock into #t_items from(
select  i.CompanyCode,i.BranchCode, year(getdate()) PeriodYear, month(getdate()) PeriodMonth,  
(SELECT DATEPART(WEEK, getdate())  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,getdate()), 0))+ 1) AS PeriodWeek, i.TypeOfGoods, (i.OnHand * ip.CostPrice) NilaiStock from spmstitems i
	join spMstItemPrice ip
	on i.CompanyCode = ip.CompanyCode and i.BranchCode = ip.BranchCode and i.PartNo = ip.PartNo
	) #t_items
	group by CompanyCode, BranchCode,PeriodYear, PeriodMonth, PeriodWeek, TypeOfGoods

	insert into spHstSparePartWeeklyStock select * , 'system' as CreatedBy, getdate() as CreatedDate, 'system' as LastUpdateBy, getdate() as LastUpdateDateFrom from #t_items 

	drop table #t_items


