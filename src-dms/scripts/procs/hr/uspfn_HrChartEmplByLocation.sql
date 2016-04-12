
go
if object_id('uspfn_HrChartEmplByLocation') is not null
	drop procedure uspfn_HrChartEmplByLocation

go
create procedure uspfn_HrChartEmplByLocation
	@CompanyCode varchar(20),
	@ChartType varchar(20) = 'pie'
as

declare @config as table(name varchar(20), value varchar(200))
insert into @config values ('type', @ChartType)
insert into @config values ('title', 'Employee by Location')

;with x as (
select EmployeeID
     , BranchCode = isnull((
		select top 1 BranchCode
		  from HrEmployeeMutation
		 where EmployeeID = HrEmployee.EmployeeID
		   and PersonnelStatus = '1'
		 order by MutationDate desc
		 ), 'UN-ASSIGN')
  from HrEmployee 
)
select BranchCode as category, count(*) as value from x group by BranchCode order by BranchCode

select * from @config

