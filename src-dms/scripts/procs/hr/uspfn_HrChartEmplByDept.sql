
go
if object_id('uspfn_HrChartEmplByDept') is not null
	drop procedure uspfn_HrChartEmplByDept

go
create procedure uspfn_HrChartEmplByDept
	@CompanyCode varchar(20),
	@ChartType varchar(20) = 'pie'
as

declare @config as table(name varchar(20), value varchar(200))
insert into @config values ('type', @ChartType)
insert into @config values ('title', 'Employee by Department')

select isnull(Department, 'UN-CATEGORY') as category, count(*) as value
  from HrEmployee
 where CompanyCode = @CompanyCode 
   and PersonnelStatus = '1'
 group by Department
 order by count(*) desc

select * from @config