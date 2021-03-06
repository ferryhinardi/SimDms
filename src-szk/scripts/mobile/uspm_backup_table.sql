alter procedure uspm_backup_table
	@DealerCode varchar(20)
as

select DealerCode
     , TableName
     , LastUpdate
     , Tolerance
     , CheckField
     , IsCustomKey = convert(bit, (case rtrim(isnull(TableKeys, '')) when '' then 0 else 1 end))
     , TableKeys
  from SysDealer
 where DealerCode = @DealerCode

go 

exec uspm_backup_table '6021406'

