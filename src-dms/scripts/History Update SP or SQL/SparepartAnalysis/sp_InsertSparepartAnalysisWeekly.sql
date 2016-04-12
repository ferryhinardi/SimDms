 /**=============================================
 Author:		David
 Create date: 15 January 2016
 Description:	Insert Sparepart Analysis Weekly
 =============================================**/
create procedure sp_InsertSparepartAnalysisWeekly

--declare
@periodYear numeric(4,0),
@periodMonth numeric(2,0),
@periodWeek numeric(1,0)

--set @periodYear = 2015
--set @periodMonth = 1
--set @periodWeek = 0

as
begin
-- workshop

select CompanyCode, BranchCode,PeriodYear, PeriodMonth,PeriodWeek,TypeOfGoods, sum(SalesAmt) TotalSalesAmt, sum(HppAmt) TotalHppAmt into #t_ws from (
select a.CompanyCode,a.BranchCode, YEAR(a.LmpDate) PeriodYear,Month(a.LmpDate) PeriodMonth,
(SELECT DATEPART(WEEK, a.LmpDate)  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,a.LmpDate), 0))+ 1) AS PeriodWeek,c.TypeOfGoods,
 (b.QtyBill * b.RetailPrice) - (b.QtyBill * b.RetailPrice * b.DiscPct * 0.01)  SalesAmt, (b.QtyBill * b.CostPrice) HppAmt 
from spTrnSLmpHdr a
join spTrnSLmpDtl b
on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.LmpNo = b.LmpNo
join spMstItems c 
on b.CompanyCode = c.CompanyCode and b.BranchCode = c.BranchCode and b.PartNo = c.PartNo
where YEAR(a.LmpDate) = @periodYear and MONTH(a.LmpDate) = @periodMonth 
and (SELECT DATEPART(WEEK, a.LmpDate)  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,a.LmpDate), 0))+ 1) = case when @periodWeek = 0 
	then (SELECT DATEPART(WEEK, a.LmpDate)  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,a.LmpDate), 0))+ 1) else @periodWeek end
	and left(b.DocNo,3)='SSS'
) #t_ws
group by  CompanyCode, BranchCode,PeriodYear, PeriodMonth,PeriodWeek,TypeOfGoods
order by BranchCode, PeriodYear,PeriodMonth,TypeOfGoods, PeriodWeek

-- Unit

select CompanyCode, BranchCode,PeriodYear, PeriodMonth,PeriodWeek,TypeOfGoods, sum(SalesAmt) TotalSalesAmt, sum(HppAmt) TotalHppAmt into #t_u from (
select a.CompanyCode,a.BranchCode, YEAR(a.LmpDate) PeriodYear,Month(a.LmpDate) PeriodMonth,
(SELECT DATEPART(WEEK, a.LmpDate)  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,a.LmpDate), 0))+ 1) AS PeriodWeek,c.TypeOfGoods,
 (b.QtyBill * b.RetailPrice) - (b.QtyBill * b.RetailPrice * b.DiscPct * 0.01)  SalesAmt, (b.QtyBill * b.CostPrice) HppAmt 
from spTrnSLmpHdr a
join spTrnSLmpDtl b
on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.LmpNo = b.LmpNo
join spMstItems c 
on b.CompanyCode = c.CompanyCode and b.BranchCode = c.BranchCode and b.PartNo = c.PartNo
where YEAR(a.LmpDate) = @periodYear and MONTH(a.LmpDate) = @periodMonth 
and (SELECT DATEPART(WEEK, a.LmpDate)  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,a.LmpDate), 0))+ 1) = case when @periodWeek = 0 
	then (SELECT DATEPART(WEEK, a.LmpDate)  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,a.LmpDate), 0))+ 1) else @periodWeek end
	and left(b.DocNo,3)='SSU'
) #t_u
group by  CompanyCode, BranchCode,PeriodYear, PeriodMonth,PeriodWeek,TypeOfGoods
order by BranchCode, PeriodYear,PeriodMonth,TypeOfGoods, PeriodWeek

-- partshop
select CompanyCode, BranchCode,PeriodYear, PeriodMonth,PeriodWeek,TypeOfGoods, sum(SalesAmt) TotalSalesAmt, sum(HppAmt) TotalHppAmt  into #t_ps from (
select a.CompanyCode,a.BranchCode, YEAR(a.FPJDate) PeriodYear,MONTH(a.FPJDate) PeriodMonth, 
(SELECT DATEPART(WEEK, a.FPJDate)  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,a.FPJDate), 0))+ 1) AS PeriodWeek, c.TypeOfGoods,
	 (b.QtyBill * b.RetailPrice) - (b.QtyBill * b.RetailPrice * b.DiscPct * 0.01) SalesAmt, (b.QtyBill * b.CostPrice) HppAmt
from spTrnSFPJHdr a
join spTrnSFPJDtl b
on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.FPJNo = b.FPJNo
join spMstItems c 
on b.CompanyCode = c.CompanyCode and b.BranchCode = c.BranchCode and b.PartNo = c.PartNo
join gnMstCustomer d
on a.CompanyCode = d.CompanyCode and a.CustomerCode = d.CustomerCode
where d.CategoryCode = 15 and YEAR(a.FPJDate) = @periodYear and MONTH(a.FPJDate) = @periodMonth
and (SELECT DATEPART(WEEK, a.FPJDate)  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,a.FPJDate), 0))+ 1)  = case when @periodWeek = 0 
	then (SELECT DATEPART(WEEK, a.FPJDate)  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,a.FPJDate), 0))+ 1)  else @periodWeek end
) #t_ps
group by  CompanyCode, BranchCode,PeriodYear, PeriodMonth,PeriodWeek,TypeOfGoods
order by BranchCode, PeriodYear,PeriodMonth,TypeOfGoods, PeriodWeek

-- counter

select CompanyCode, BranchCode,PeriodYear, PeriodMonth,PeriodWeek,TypeOfGoods, sum(SalesAmt) TotalSalesAmt, sum(HppAmt) TotalHppAmt into #t_c from(
select a.CompanyCode,a.BranchCode, YEAR(a.FPJDate) PeriodYear,MONTH(a.FPJDate) PeriodMonth, 
(SELECT DATEPART(WEEK, a.FPJDate)  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,a.FPJDate), 0))+ 1) AS PeriodWeek, c.TypeOfGoods,
	 (b.QtyBill * b.RetailPrice)- (b.QtyBill * b.RetailPrice * b.DiscPct * 0.01)  SalesAmt, (b.QtyBill * b.CostPrice) HppAmt
from spTrnSFPJHdr a
join spTrnSFPJDtl b
on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.FPJNo = b.FPJNo
join spMstItems c 
on b.CompanyCode = c.CompanyCode and b.BranchCode = c.BranchCode and b.PartNo = c.PartNo
join gnMstCustomer d
on a.CompanyCode = d.CompanyCode and a.CustomerCode = d.CustomerCode
where d.CategoryCode <> 15 and YEAR(a.FPJDate) = @periodYear and MONTH(a.FPJDate) = @periodMonth
and (SELECT DATEPART(WEEK, a.FPJDate)  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,a.FPJDate), 0))+ 1) = case when @periodWeek = 0 
	then (SELECT DATEPART(WEEK, a.FPJDate)  -
    DATEPART(WEEK, DATEADD(MM, DATEDIFF(MM,0,a.FPJDate), 0))+ 1) else @periodWeek end
) #t_c
group by CompanyCode, BranchCode,PeriodYear, PeriodMonth,PeriodWeek,TypeOfGoods
order by BranchCode, PeriodYear,PeriodMonth,TypeOfGoods, PeriodWeek

-- insert

delete spHstSparepartWeekly
where PeriodYear = @periodYear and PeriodMonth = @periodMonth 
and PeriodWeek = case when @periodWeek = 0 then  PeriodWeek else @periodWeek  end

select * into #t_insert from(
select ws.CompanyCode,ws.BranchCode, ws.PeriodYear,ws.PeriodMonth, ws.PeriodWeek, ws.TypeOfGoods
, isnull(ws.TotalSalesAmt,0) Netto_WS, isnull(ws.TotalHppAmt,0) HPP_WS
, isnull(ps.TotalSalesAmt,0) Netto_PS, isnull(ps.TotalHppAmt,0) HPP_PS
, isnull(c.TotalSalesAmt,0) Netto_C, isnull(c.TotalHppAmt,0) HPP_C 
, isnull(u.TotalSalesAmt,0) Netto_U, isnull(u.TotalHppAmt,0) HPP_U 
, isnull(s.NilaiStock,0) NilaiStock, 'system' as CreatedBy, getdate() as CreatedDate, 'system' as LastUpdateBy, getdate() as LastUpdateDate from #t_ws ws
left join #t_ps ps 
on ws.companycode = ps.companycode and ws.branchcode = ps.branchcode and ws.periodyear = ps.periodyear and ws.periodmonth  = ps.periodmonth and ws.periodweek = ps.periodweek and ws.typeofgoods = ps.typeofgoods
left join #t_c c
on ws.companycode = c.companycode and ws.branchcode = c.branchcode and ws.periodyear = c.periodyear and ws.periodmonth  = c.periodmonth and ws.periodweek = c.periodweek and ws.typeofgoods = c.typeofgoods
left join #t_u u
on ws.companycode = u.companycode and ws.branchcode = u.branchcode and ws.periodyear = u.periodyear and ws.periodmonth  = u.periodmonth and ws.periodweek = u.periodweek and ws.typeofgoods = u.typeofgoods
left join spHstSparePartWeeklyStock s
on ws.companycode = s.companycode and ws.branchcode = s.branchcode and ws.periodyear = s.periodyear and ws.periodmonth  = s.periodmonth and ws.periodweek = s.periodweek and ws.typeofgoods = s.typeofgoods
) #t_insert

insert into spHstSparepartWeekly select * from #t_insert
 
--select ws.PeriodWeek, isnull(sum(ws.TotalSalesAmt),0) Netto_WS, isnull(sum(ws.TotalHppAmt),0) HPP_WS, isnull(sum(ps.TotalSalesAmt),0) Netto_PS, isnull(sum(ps.TotalHppAmt),0) HPP_PS, isnull(sum(c.TotalSalesAmt),0) Netto_C, isnull(sum(c.TotalHppAmt),0) HPP_C , isnull(sum(s.NilaiStock),0) NilaiStock from #t_ws ws
--left join #t_ps ps 
--on ws.companycode = ps.companycode and ws.branchcode = ps.branchcode and ws.periodyear = ps.periodyear and ws.periodmonth  = ps.periodmonth and ws.periodweek = ps.periodweek and ws.typeofgoods = ps.typeofgoods
--left join #t_c c
--on ws.companycode = c.companycode and ws.branchcode = c.branchcode and ws.periodyear = c.periodyear and ws.periodmonth  = c.periodmonth and ws.periodweek = c.periodweek and ws.typeofgoods = c.typeofgoods
--left join spHstSparePartWeeklyStock s
--on ws.companycode = s.companycode and ws.branchcode = s.branchcode and ws.periodyear = s.periodyear and ws.periodmonth  = s.periodmonth and ws.periodweek = s.periodweek and ws.typeofgoods = s.typeofgoods
--group by  ws.CompanyCode, ws.PeriodYear,ws.PeriodMonth, ws.PeriodWeek

drop table #t_insert
drop table #t_ws
drop table #t_ps
drop table #t_c
drop TABLE #t_u

end
