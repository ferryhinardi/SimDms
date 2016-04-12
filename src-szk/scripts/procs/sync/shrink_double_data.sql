alter procedure uspfn_remove_duplicate
as

declare @t_double as table (
	FilePath varchar(100),
	Status varchar(100),
	DataCount int
)

insert into @t_double
select FilePath, Status, count(*)
  from SysDealerHist
 where Status = 'UPLOADED'
 group by FilePath, Status
having count(*) > 1
 order by count(*) desc

;with x as (
select a.UploadID, a.FilePath, a.UploadedDate, a.Status, a.ErrorMessage, b.DataCount
     , case when (a.UploadID = (select top 1 UploadID from SysDealerHist where FilePath = a.FilePath order by UploadedDate))
	   then 'A' else 'X' end as Flag
  from SysDealerHist a
 inner join @t_double b
    on b.FilePath = a.FilePath
)
--select * from x where Flag = 'A'
update x set Status = 'ERROR', ErrorMessage = 'duplicate data' where status = 'UPLOADED'




