alter procedure uspfn_GnCollDataLog
	@DealerCode varchar(20),
	@TableName  varchar(20), 
	@DateFrom   date,
	@DateTo     date
as

;with x as (
select a.DealerCode 
     , a.TableName
     , convert(date, a.UploadedDate) UploadDate
	 , count(a.UploadID) as PkgUpload
  from SysDealerHist a
 where a.DealerCode = @DealerCode
   and a.TableName = @TableName
   and a.UploadedDate > @DateFrom
   and convert(date, a.UploadedDate) <= @DateTo
   and a.DealerCode not in ('6006400001', '6006410')
 group by convert(date, a.UploadedDate), a.DealerCode, a.TableName
)
, y as (
select x.DealerCode
     , case x.DealerCode
	    when '6006400001A' then 'SEJAHTERA BUANA TRADA - JKT'
	    when '6006400001B' then 'SEJAHTERA BUANA TRADA - SBY'
	    when '6006400001C' then 'SEJAHTERA BUANA TRADA - PKU'
	    else b.DealerName
	   end as DealerName
     , x.UploadDate
	 , x.TableName 
	 , x.PkgUpload
	 , PkgSuccess = (select count(UploadID) from SysDealerHist where DealerCode = x.DealerCode and TableName = x.TableName and convert(date, UploadedDate) = x.UploadDate and Status = 'PROCESSED')
	 , PkgError = (select count(UploadID) from SysDealerHist where DealerCode = x.DealerCode and TableName = x.TableName and convert(date, UploadedDate) = x.UploadDate and Status = 'ERROR')
  from x
  left join DealerInfo b
    on b.DealerCode = x.DealerCode
)
select *, (PkgUpload - PkgSuccess - PkgError) as PkgInPogress from y order by UploadDate


    -- , case a.DealerCode
	   -- when '6006400001A' then 'SEJAHTERA BUANA TRADA - JKT'
	   -- when '6006400001B' then 'SEJAHTERA BUANA TRADA - SBY'
	   -- when '6006400001C' then 'SEJAHTERA BUANA TRADA - PKU'
	   -- else b.DealerName
	   --end as DealerName

go


exec uspfn_GnCollDataLog '6021406', 'PmHstITS', '2014-04-01', '2014-07-25'


