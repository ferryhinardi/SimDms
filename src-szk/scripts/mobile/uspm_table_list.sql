alter procedure uspm_table_list
	@DealerCode varchar(20)
as

select DealerCode
     , TableName
     , LastUpdate
     --, Tolerance
     , CheckField
     , IsCustomKey = convert(bit, (case rtrim(isnull(TableKeys, '')) when '' then 0 else 1 end))
     , TableKeys
  from SysDealer
 where DealerCode = @DealerCode

go 

exec uspm_table_list '6021406'

