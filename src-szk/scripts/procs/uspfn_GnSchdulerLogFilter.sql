alter procedure uspfn_GnSchdulerLogFilter

as

select distinct (case isnull(a.DealerCode, '') when '' then '0000000' else a.DealerCode end) as value
     , (case isnull(a.DealerCode, '') when '' then 'DATA WAREHOUSE SIMSDMS' else b.DealerName end) text 
  from GnSchedulerLog a
  left join DealerInfo b
    on b.DealerCode = a.DealerCode

go

exec uspfn_GnSchdulerLogFilter 


