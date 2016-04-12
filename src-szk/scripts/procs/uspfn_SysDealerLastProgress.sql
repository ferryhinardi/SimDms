alter procedure uspfn_SysDealerLastProgress
	@DealerCode varchar(50),
	@TableName  varchar(50)
as

if not exists (select top 1 1 from SysDealer where DealerCode=@DealerCode and TableName=@TableName)
begin
	insert into SysDealer (DealerCode, TableName, LastUpdate, Tolerance, CheckField)
	values (@DealerCode, @TableName, '1900-01-01', 0, 'LastUpdateDate')
end



declare @query varchar(max)
declare @chkfield varchar(50)

set @chkfield = isnull((select CheckField from SysDealer where DealerCode = @DealerCode and TableName = @TableName), 'CreatedDate')

set @query = N'
with x as (
select LastUpdate
     , LastUpdateNew = isnull((
		select top 1 ' + @chkfield + ' from ' + @TableName + ' order by ' + @chkfield + ' desc
	 ), getdate())
  from SysDealer where rtrim(left(DealerCode, 10)) = ''' + rtrim(left(@DealerCode, 10)) + ''' and TableName = ''' + @TableName + '''
)
update x set LastUpdate = (case when (LastUpdateNew > getdate()) then getdate() else LastUpdateNew end)'

exec(@query)

;with x as (
select * from SysDealerHist
 where rtrim(left(DealerCode, 10)) = rtrim(left(@DealerCode, 10))
   and TableName = @TableName
   and FileName like (@TableName + '_%.zip')
)
update x set Status = 'PROCESSED', ProcessedDate = getdate()

select * from SysDealer where DealerCode = @DealerCode and TableName = @TableName
select top 10 * from SysDealerHist where DealerCode = @DealerCode and rtrim(left(TableName, 10)) = rtrim(left(@TableName, 10)) and Status = 'PROCESSED' order by ProcessedDate desc

--go

--uspfn_SysDealerLastProgress '6468401', 'gnMstCustomerProfitCenter'
