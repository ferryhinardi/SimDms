alter procedure uspfn_SysDealerUpdated
	@id varchar(50)
as

declare @date       as datetime
declare @TableName  as varchar(60)
declare @DealerCode as varchar(60)

set @date = getdate()
set @TableName = (select TableName from SysDealerHist where UploadID = @id)
set @DealerCode = (select DealerCode from SysDealerHist where UploadID = @id)

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
 where FilePath in (select FilePath from SysDealerHist where UploadID = @id)
)
update x set Status = 'PROCESSED', ProcessedDate = getdate()

go

uspfn_SysDealerUpdated '5a8fb35ac260cd7582b1f0fcf315cafd'
 
