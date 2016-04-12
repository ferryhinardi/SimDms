alter procedure uspfn_SysDealerHistQueueing
	@DealerCode varchar(20),
	@TableName  varchar(50),
	@DataDate   varchar(20)
as

;with x as (
select * from SysDealerHist
 where DealerCode = @DealerCode
   and TableName = @TableName
   and FileName = (@TableName + '_' + @DataDate + '.zip')
)
update x set Status = 'QUEUEING', QueueingDate = getdate()


