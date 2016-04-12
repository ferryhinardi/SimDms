alter procedure uspfn_SysDealerFlagData
	@id      varchar(50),
	@Status  varchar(20) = '',
	@Message varchar(100) = ''
as

declare @date as datetime
set @date = getdate()

;with x as (
select * from SysDealerHist
 where FilePath in (select FilePath from SysDealerHist where UploadID = @id)
)
update x set Status = 'ERROR', ErrorMessage = @Message, ProcessedDate = @date, LastRecordUpdate = 0
