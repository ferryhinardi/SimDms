alter procedure uspfn_SysDealerTable
as

select distinct TableName as value, TableName as text
  from SysDealer order by TableName
go 

uspfn_SysDealerTable