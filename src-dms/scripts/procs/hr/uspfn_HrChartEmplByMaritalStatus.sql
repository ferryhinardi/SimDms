
go
if object_id('uspfn_HrChartEmplByMaritalStatus') is not null
	drop procedure uspfn_HrChartEmplByMaritalStatus

go
create procedure uspfn_HrChartEmplByMaritalStatus
	@CompanyCode varchar(15)

as

declare @config as table(name varchar(20), value varchar(200))
insert into @config values ('type', 'pie')
insert into @config values ('title', 'Employee by Marital Status')


select MaritalStatus as category, count(*) as value
  from HrEmployee
 where CompanyCode = @CompanyCode 
   and PersonnelStatus = '1'
 group by MaritalStatus
 order by MaritalStatus


select * from @config

