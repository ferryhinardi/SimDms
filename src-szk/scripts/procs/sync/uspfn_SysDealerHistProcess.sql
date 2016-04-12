alter procedure uspfn_SysDealerHistProcess
--declare
	@DealerCode varchar(20),
	@TableName  varchar(50),
	@DataDate   varchar(20)
as

--set	@DealerCode = '6006406'
--set	@TableName  = 'CsTDayCall'
--set	@DataDate   = '2014_0306'

declare @query varchar(max)
declare @chkfield varchar(50)

set @chkfield = isnull((select CheckField from SysDealer where DealerCode = @DealerCode and TableName = @TableName), 'CreatedDate')

set @query = N'
with x as (
select LastUpdate
     , LastUpdateNew = isnull((
		select top 1 ' + @chkfield + ' from ' + @TableName + ' order by ' + @chkfield + ' desc
	 ), getdate())
  from SysDealer where DealerCode = ''' + @DealerCode + ''' and TableName = ''' + @TableName + '''
)
update x set LastUpdate = (case when (LastUpdateNew > getdate()) then getdate() else LastUpdateNew end)'

exec(@query)

;with x as (
select * from SysDealerHist
 where DealerCode = @DealerCode
   and TableName = @TableName
   and FileName = (@TableName + '_' + @DataDate + '.zip')
)
update x set Status = 'PROCESSED', ProcessedDate = getdate()

select * from SysDealer where DealerCode = @DealerCode and TableName = @TableName
select * from SysDealerHist
 where DealerCode = @DealerCode
   and TableName = @TableName
   and FileName = (@TableName + '_' + @DataDate + '.zip')

