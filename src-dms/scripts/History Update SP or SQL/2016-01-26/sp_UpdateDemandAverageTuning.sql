ALTER procedure [dbo].[sp_UpdateDemandAverageTuning] (  

@CompanyCode varchar(10) ,
@BranchCode varchar(10),
@TransDate datetime)

as

--declare @TransDate as datetime
--set @TransDate = '{0}'

select * into #t1 from (
select a.CompanyCode
,a.BranchCode
,a.PartNo
,a.DemandQty
,a.Year
,a.Month
,convert(varchar,a.Year) + right('0' + convert(varchar,a.Month),2) as date0
,convert(varchar(6), dateadd(m,-5,@TransDate), 112) date1
,convert(varchar(6), @TransDate, 112) date2
from spHstDemandItem a WITH(NOWAIT, NOLOCK) where a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode
) #t1

select * into #t2 from (
select 
a.* 
,case when date0 between date1 and date2 then 1 else 0 end as IsValid
from #t1 a
) #t2

select * into #t3 from (
select CompanyCode, BranchCode, PartNo, Sum(DemandQty) DemandQty, Sum(DemandQty)/(6*30) DemandAvg from #t2 
where IsValid = 1 and DemandQty > 0
group by CompanyCode, BranchCode, PartNo
) #t3

select * into #t4 from (
select a.companycode, a.branchcode, a.partno, 0 as DemandAvg 
from spMstItems a
left join #t3 b on a.companycode = b.companycode and a.branchcode = b.branchcode and a.partno = b.partno
where  a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode and b.partno is null
) #t4

update spMstItems set DemandAverage = isnull(b.DemandAvg, 0)
from spMstItems a, #t4 b
where a.CompanyCode=b.CompanyCode
and a.BranchCode=b.BranchCode
and a.PartNo=b.PartNo

update spMstItems set DemandAverage = isnull(b.DemandAvg, a.DemandAverage)
from spMstItems a, #t3 b
where a.CompanyCode=b.CompanyCode
and a.BranchCode=b.BranchCode
and a.PartNo=b.PartNo

select * into #t5 from (
select a.CompanyCode, a.BranchCode, a.PartNo, b.NewPartNo, a.DemandAverage
from spMstItems a
left join spMstItemMod b
on b.CompanyCode = a.CompanyCode
	and b.PartNo = a.PartNo
where a.DemandAverage > 0 and b.NewPartNo <> ''
	and a.CompanyCode = @CompanyCode
	and a.BranchCode = @BranchCode
) #t5

update spMstItems set DemandAverage = ISNULL(a.DemandAverage, a.DemandAverage + b.DemandAverage)
from spMstItems a, #t5 b
where a.CompanyCode = b.CompanyCode
	and a.BranchCode = b.BranchCode
	and a.PartNo = b.NewPartNo


drop table #t4
drop table #t3
drop table #t2
drop table #t1

