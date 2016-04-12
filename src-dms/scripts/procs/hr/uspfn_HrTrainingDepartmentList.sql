
go
if object_id('uspfn_HrTrainingDepartmentList') is not null
	drop procedure uspfn_HrTrainingDepartmentList

go
create procedure uspfn_HrTrainingDepartmentList (
	@CompanyCode varchar(15),
	@EmployeeID varchar(15)
)
as
begin
	select distinct
		a.Department as text,
		b.OrgCode as value
	from	
		HrDepartmentTraining a
	inner join	
		gnMstOrgGroup b
	on
		a.CompanyCode = b.CompanyCode
		and
		a.Department = b.OrgCode
	inner join
		HrEmployeeTraining c
	on
		a.CompanyCode = c.CompanyCode
		and
		a.TrainingCode = c.TrainingCode
	where
		a.CompanyCode = @CompanyCode
		and
		c.EmployeeID = @EmployeeID
end
