alter procedure uspfn_SysDealerHistError
	@DealerCode varchar(20),
	@TableName  varchar(50),
	@DataDate   varchar(20)
as

;with x as (
select * from SysDealerHist
 where DealerCode = @DealerCode
   and TableName = @TableName
   and FileName = (@TableName + '_' + @DataDate + '.zip')
   and Status = 'ERROR'
)
update x set Status = 'ERROR', QueueingDate = getdate()


