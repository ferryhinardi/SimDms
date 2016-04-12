IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[uspfn_SpUpdateMovingCode]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[uspfn_SpUpdateMovingCode]
GO

CREATE PROCEDURE uspfn_SpUpdateMovingCode
--declare 
@CompanyCode as varchar(50),
@BranchCode as varchar(50),
@TransDate as datetime

--set @TransDate = GETDATE()
--set @CompanyCode = '6115204001'
--set @BranchCode = '6115204101'

as
begin

select * into #t1 from (
select 
 a.PartNo
,a.DemandFreq
,a.DemandQty
,convert(varchar,a.Year) + right('0' + convert(varchar,a.Month),2) as date0
,convert(varchar(6), dateadd(m,-11,@TransDate), 112) date1
,convert(varchar(6), dateadd(m,-10,@TransDate), 112) date2
,convert(varchar(6), dateadd(m,-9,@TransDate), 112) date3
,convert(varchar(6), dateadd(m,-8,@TransDate), 112) date4
,convert(varchar(6), dateadd(m,-7,@TransDate), 112) date5
,convert(varchar(6), dateadd(m,-6,@TransDate), 112) date6

,convert(varchar(6), dateadd(m,-5,@TransDate), 112) date7
,convert(varchar(6), dateadd(m,-4,@TransDate), 112) date8
,convert(varchar(6), dateadd(m,-3,@TransDate), 112) date9
,convert(varchar(6), dateadd(m,-2,@TransDate), 112) date10
,convert(varchar(6), dateadd(m,-1,@TransDate), 112) date11
,convert(varchar(6), dateadd(m,-0,@TransDate), 112) date12

from spHstDemandItem a WITH(NOWAIT, NOLOCK) 
where a.CompanyCode=@CompanyCode and a.BranchCode=@BranchCode
 and convert(varchar,a.Year) + right('0' + convert(varchar,a.Month),2) >= convert(varchar(6), dateadd(m,-12,@TransDate), 112)
) #t1

select * into #t2 from (
select 
 a.PartNo
,a.DemandFreq
,case when (date0=date1) and a.DemandFreq>0 then 1 else 0 end as T1
,case when (date0=date2) and a.DemandFreq>0 then 1 else 0 end as T2
,case when (date0=date3) and a.DemandFreq>0 then 1 else 0 end as T3
,case when (date0=date4) and a.DemandFreq>0 then 1 else 0 end as T4
,case when (date0=date5) and a.DemandFreq>0 then 1 else 0 end as T5
,case when (date0=date6) and a.DemandFreq>0 then 1 else 0 end as T6

,case when (date0=date7) and a.DemandFreq>0 then 1 else 0 end as T7
,case when (date0=date8) and a.DemandFreq>0 then 1 else 0 end as T8
,case when (date0=date9) and a.DemandFreq>0 then 1 else 0 end as T9
,case when (date0=date10) and a.DemandFreq>0 then 1 else 0 end as T10
,case when (date0=date11) and a.DemandFreq>0 then 1 else 0 end as T11
,case when (date0=date12) and a.DemandFreq>0 then 1 else 0 end as T12
from #t1 a
) #t2

select * into #t3 from (
select
 a.PartNo
,case when (sum(T1)> 0) then 1 else 0 end as D1
,case when (sum(T2)> 0) then 1 else 0 end as D2
,case when (sum(T3)> 0) then 1 else 0 end as D3
,case when (sum(T4)> 0) then 1 else 0 end as D4
,case when (sum(T5)> 0) then 1 else 0 end as D5
,case when (sum(T6)> 0) then 1 else 0 end as D6

,case when (sum(T7)> 0) then 1 else 0 end as D7
,case when (sum(T8)> 0) then 1 else 0 end as D8
,case when (sum(T9)> 0) then 1 else 0 end as D9
,case when (sum(T10)> 0) then 1 else 0 end as D10
,case when (sum(T11)> 0) then 1 else 0 end as D11
,case when (sum(T12)> 0) then 1 else 0 end as D12
from #t2 a
group by a.PartNo
) #t3

select * into #t4 from (
select 
 a.PartNo
,b.NewPartNo
from #t3 a
left join spMstItemMod b WITH(NOWAIT, NOLOCK)
  on b.PartNo = a.PartNo and b.CompanyCode = @CompanyCode
where b.NewPartNo <> ''
) #t4

insert into #t3
select 
 NewPartNo as PartNo
,D1=0,D2=0,D3=0,D4=0,D5=0,D6=0
,D7=0,D8=0,D9=0,D10=0,D11=0,D12=0
from #t4
where NewPartNo not in (select PartNo from #t3)

select * into #t5 from(
select distinct PartNo, D1, D2, D3, D4, D5, D6, D7, D8, D9, D10, D11, D12 from #t3)#t5

select * into #t6 from (
select 
	PartNo
	, CASE WHEN ISNULL((SELECT D1 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D1 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D1
	, CASE WHEN ISNULL((SELECT D2 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D2 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D2
	, CASE WHEN ISNULL((SELECT D3 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D3 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D3
	, CASE WHEN ISNULL((SELECT D4 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D4 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D4
	, CASE WHEN ISNULL((SELECT D5 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D5 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D5
	, CASE WHEN ISNULL((SELECT D6 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D6 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D6

	, CASE WHEN ISNULL((SELECT D7 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D7 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D7
	, CASE WHEN ISNULL((SELECT D8 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D8 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D8
	, CASE WHEN ISNULL((SELECT D9 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D9 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D9
	, CASE WHEN ISNULL((SELECT D10 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D10 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D10
	, CASE WHEN ISNULL((SELECT D11 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D11 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D11
	, CASE WHEN ISNULL((SELECT D12 FROM #t5 WHERE PartnO = a.PartNo),0) + ISNULL((SELECT D12 FROM #t5 WHERE PartnO = a.NewPartNo),0) > 0 THEN 1 ELSE 0 END D12
from #t4 a) #t6

update #t5
set D1 = b.D1
	, D2 = b.D2
	, D3 = b.D3
	, D4 = b.D4
	, D5 = b.D5
	, D6 = b.D6
	, D7 = b.D7
	, D8 = b.D8
	, D9 = b.D9
	, D10 = b.D10
	, D11 = b.D11
	, D12 = b.D12
from #t5 a, #t6 b
where a.partno = b.partno

select * into #t7 from (
select @CompanyCode CompanyCode, @BranchCode BranchCode, partno, D1 + D2 + D3  + D4 + D5 + D6 + D7 + D8 + D9 + D10 + D11 + D12 dTotal from #t5) #t7

update spMstItems 
set MovingCode = CASE WHEN b.dTotal = 0 THEN 5
					  WHEN b.dTotal >= 1 AND b.dTotal <= 3 THEN 4
					  WHEN b.dTotal >= 4 AND b.dTotal <= 8 THEN 3
					  WHEN b.dTotal >= 9 AND b.dTotal <= 11 THEN 2	
					  WHEN b.dTotal = 12 THEN 1
					  ELSE 0
				 END
from spMstItems a, #t7 b
where 
	a.CompanyCode = b.CompanyCode
	and a.branchcode = b.branchcode
	and a.partno = b.partno
	and (datediff(mm, a.BornDate, @transdate) + 1) >= 12 

-- SET MOVING CODE : 0 FOR ITEM THAT BORN DATE < 12 MONTHS
update spMstItems set MovingCode = 0
where CompanyCode = @CompanyCode
  and BranchCode = @BranchCode
  and (datediff(mm, BornDate, @TransDate) + 1) < 12

drop table #t7
drop table #t6
drop table #t5
drop table #t4
drop table #t3
drop table #t2
drop table #t1
end

GO