alter procedure uspfn_SysDealerNotFound
	@id varchar(50)
as

declare @date as datetime
set @date = getdate()

;with x as (
select * from SysDealerHist
 where FilePath in (select FilePath from SysDealerHist where UploadID = @id)
)
update x set Status = 'ERROR', ErrorMessage = 'File Not Found', ProcessedDate = @date, LastRecordUpdate = 0

go 

uspfn_SysDealerNotFound '5a8fb35ac260cd7582b1f0fcf315cafd'