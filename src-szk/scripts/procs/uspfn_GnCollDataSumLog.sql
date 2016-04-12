alter procedure uspfn_GnCollDataSumLog
	@TableName  varchar(20) = '',
	@DateFrom   date,
	@DateTo     date
as

;with x as (
select a.DealerCode 
	 , count(a.UploadID) as PkgReceived
	 , PkgSuccess = (select count(UploadID) 
	                 from SysDealerHist
					where DealerCode = a.DealerCode
					  and UploadedDate > @DateFrom
					  and convert(date, UploadedDate) <= @DateTo
					  and TableName = (case @TableName when '' then TableName else @TableName end)
					  and Status = 'PROCESSED')
	 , PkgError = (select count(UploadID) 
	                 from SysDealerHist
					where DealerCode = a.DealerCode
					  and UploadedDate > @DateFrom
					  and convert(date, UploadedDate) <= @DateTo
					  and TableName = (case @TableName when '' then TableName else @TableName end)
					  and Status = 'ERROR')
  from SysDealerHist a
 where a.UploadedDate > @DateFrom
   and convert(date, a.UploadedDate) <= @DateTo
   and a.DealerCode not in ('6006400001', '6006410')
   and a.TableName = (case @TableName when '' then a.TableName else @TableName end)
 group by a.DealerCode
)
, y as (
select x.DealerCode
     , case x.DealerCode
	    when '6006400001A' then 'SEJAHTERA BUANA TRADA - JKT'
	    when '6006400001B' then 'SEJAHTERA BUANA TRADA - SBY'
	    when '6006400001C' then 'SEJAHTERA BUANA TRADA - PKU'
	    else b.DealerName
	   end as DealerName
	 , x.PkgReceived
	 , x.PkgSuccess
	 , x.PkgError
	 , PkgInPogress = (x.PkgReceived - x.PkgSuccess - x.PkgError)
  from x
  left join DealerInfo b
    on b.DealerCode = x.DealerCode
)
select * from y order by DealerCode

go


exec uspfn_GnCollDataSumLog '', '2014-07-01', '2014-07-25'


