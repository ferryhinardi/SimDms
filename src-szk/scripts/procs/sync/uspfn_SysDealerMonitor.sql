alter procedure uspfn_SysDealerMonitor
as


select DealerCode, count(*) as DataCoount
  from SysDealerHist
 group by DealerCode
 order by count(*) desc

go 

uspfn_SysDealerMonitor


