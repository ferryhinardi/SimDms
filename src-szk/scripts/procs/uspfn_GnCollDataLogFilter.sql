alter procedure uspfn_GnCollDataLogFilter

as

;with x as (
select distinct DealerCode
  from SysDealerHist where DealerCode not in ('6006400001', '6006410')
)
, y as (
select x.DealerCode as value
     , case x.DealerCode
	    when '6006400001A' then 'SEJAHTERA BUANA TRADA - JKT'
	    when '6006400001B' then 'SEJAHTERA BUANA TRADA - SBY'
	    when '6006400001C' then 'SEJAHTERA BUANA TRADA - PKU'
	    else b.DealerName
	   end as text
  from x
  left join DealerInfo b 
    on b.DealerCode = x.DealerCode
 where 1 = 1
)
select * from y order by text

;with x as (
select distinct DealerCode, TableName
  from SysDealerHist where DealerCode not in ('6006400001', '6006410')
)
, y as (
select x.TableName as value
     , x.TableName as text
	 , x.DealerCode as 'group'
  from x
)
select * from y order by value


select distinct TableName as value, TableName as text
  from SysDealerHist where DealerCode not in ('6006400001', '6006410')
 order by TableName

go


exec uspfn_GnCollDataLogFilter 


