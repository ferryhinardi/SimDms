alter procedure uspfn_SysDealerFlagQueueing
	@id varchar(50)
as

;with x as (
select * from SysDealerHist
 where FilePath in (select FilePath from SysDealerHist where UploadID = @id)
)
update x set Status = 'QUEUEING', QueueingDate = getdate()

select * from SysDealerHist where FilePath in (select FilePath from SysDealerHist where UploadID = @id)
