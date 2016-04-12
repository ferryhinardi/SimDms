ALTER procedure [dbo].[uspfn_SysDealerHistGet2]
	@length int = 1000
as

;with x as (
--select UploadId = (select top 1 UploadID from SysDealerHist with(nolock, nowait) where TableName = a.TableName and FilePath = a.FilePath and FileSize = a.FileSize order by UploadedDate)
--	 , UploadedDate = (select top 1 UploadedDate from SysDealerHist with(nolock, nowait) where TableName = a.TableName and FilePath = a.FilePath and FileSize = a.FileSize order by UploadedDate)
select a.UploadId
	 , a.UploadedDate
	 , a.TableName
	 , a.FilePath
	 , a.FileSize
	 , 1 as DoubleData
  from SysDealerHist a with (nolock, nowait)
 where a.Status = 'UPLOADED'
   and a.FileSize > 0
 --group by a.TableName, a.FilePath, a.FileSize

--select UploadId = (select top 1 UploadID from SysDealerHist with(nolock, nowait) where TableName = a.TableName and FilePath = a.FilePath and FileSize = a.FileSize order by UploadedDate)
--	 , UploadedDate = (select top 1 UploadedDate from SysDealerHist with(nolock, nowait) where TableName = a.TableName and FilePath = a.FilePath and FileSize = a.FileSize order by UploadedDate)
--	 , a.TableName
--	 , a.FilePath
--	 , a.FileSize
--	 , count(1) as DoubleData
--  from SysDealerHist a with (nolock, nowait)
-- where a.Status = 'UPLOADED'
--   and a.FileSize > 0
-- group by a.TableName, a.FilePath, a.FileSize
)
select top(@length) * from x order by x.DoubleData desc

go

uspfn_SysDealerHistGet2

