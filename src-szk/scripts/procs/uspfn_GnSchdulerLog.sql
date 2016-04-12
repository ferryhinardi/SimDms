alter procedure uspfn_GnSchdulerLog
	@DealerCode varchar(20) = ''

as

;with x as (
select distinct a.DealerCode
     , a.ScheduleName as ScheduleName
  from GnSchedulerLog a
  left join DealerInfo b
    on b.DealerCode = a.DealerCode
 where 1 = 1
   and a.DealerCode = (case @DealerCode when '' then a.DealerCode when '0000000' then '' else @DealerCode end)
)
, y as (
select (case isnull(x.DealerCode, '') when '' then '0000000' else x.DealerCode end) as DealerCode
     , (case isnull(x.DealerCode, '') when '' then 'DATA WAREHOUSE SIMSDMS' else b.DealerName end) DealerName 
	 , x.ScheduleName
	 , DateStart = (select top 1 DateStart from GnSchedulerLog where DealerCode = x.DealerCode and ScheduleName = x.ScheduleName order by DateStart desc)
	 , DateFinish = (select top 1 DateFinish from GnSchedulerLog where DealerCode = x.DealerCode and ScheduleName = x.ScheduleName order by DateStart desc)
	 , IsError = (select top 1 (case IsError when 0 then 'No' else 'Yes' end) from GnSchedulerLog where DealerCode = x.DealerCode and ScheduleName = x.ScheduleName order by DateStart desc)
	 , ErrorMessage = (select top 1 ErrorMessage from GnSchedulerLog where DealerCode = x.DealerCode and ScheduleName = x.ScheduleName order by DateStart desc)
	 , Info = (select top 1 Info from GnSchedulerLog where DealerCode = x.DealerCode and ScheduleName = x.ScheduleName order by DateStart desc)
  from x
  left join DealerInfo b
    on b.DealerCode = x.DealerCode
)
, z as (
select datediff(d, y.DateStart, getdate()) as Selisih, y.*
     , RunningCount = (
	     select count(*) from GnSchedulerLog
	      where DealerCode = (case y.DealerCode when '0000000' then '' else y.DealerCode end)
		    and ScheduleName = y.ScheduleName
		    and convert(date, DateStart) = convert(date, y.DateStart))
  from y
)
select * from z where datediff(d, z.DateStart, getdate()) < 1


go

exec uspfn_GnSchdulerLog ''
