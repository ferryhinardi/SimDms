alter procedure uspfn_SysDealerLastDate
	@DealerCode varchar(50),
	@TableName  varchar(50)
as

if not exists (select top 1 1 from SysDealer where DealerCode=@DealerCode and TableName=@TableName)
begin
	insert into SysDealer (DealerCode, TableName, LastUpdate, Tolerance, CheckField)
	values (@DealerCode, @TableName, '1900-01-01', 0, 'LastUpdateDate')
end

select top 1 dateadd(day, -Tolerance, LastUpdate) LastUpdate
  from SysDealer
 where DealerCode=@DealerCode
   and TableName=@TableName
