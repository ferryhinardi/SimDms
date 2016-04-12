alter procedure uspfn_SysDealerOutstanding
as

;with x as (
select a.DealerCode
     , b.ShortName as DealerName
	 , a.UploadedDate
	 , a.FileName
	 , a.FileSize
  from SysDealerHist a with (nolock,nowait)
  left join DealerInfo b
    on b.DealerCode = a.DealerCode
 where a.Status = 'UPLOADED'
   and a.FileSize > 0
),
y as (
select DealerCode
     , DealerName
	 , count(*) as DataCount
	 , FirstUploadDate = (select top 1 UploadedDate from x as sub where sub.DealerCode = x.DealerCode order by UploadedDate)
	 , LastUploadDate = (select top 1 UploadedDate from x as sub where sub.DealerCode = x.DealerCode order by UploadedDate desc)
  from x
 group by DealerCode, DealerName
),
z as (
select DealerCode 
     , DealerName
	 , DataCount
	 , FirstUploadDate
	 , LastUploadDate
  from y
)
select DealerCode as name
     , DealerName as text
	 , 'First Upload: ' + left(convert(varchar, FirstUploadDate, 21), 19)
	 + ', Last Upload: ' + left(convert(varchar, getdate(), 21), 19) as subtext 
	 , DataCount as infotext
	 , 'pending' as infostatus
  from z
 where 1 = 1
   --and DelayDate > 0
 order by infotext desc

go 

uspfn_SysDealerOutstanding