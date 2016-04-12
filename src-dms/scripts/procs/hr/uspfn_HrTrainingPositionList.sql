
go
if object_id('uspfn_HrTrainingPositionList') is not null
	drop procedure uspfn_HrTrainingPositionList

go
create procedure uspfn_HrTrainingPositionList (
	@CompanyCode varchar(15),
	@EmployeeID varchar(15),
	@Department varchar(15)
)
as
begin
	--declare @CompanyCode varchar(15);
	--declare @EmployeeID varchar(15);

	--set @CompanyCode = '6115204';
	--set @EmployeeID = '00152';
	--set @Department = 'SALES';

	select distinct
		a.Position as value,
		d.PosName as text
	from	
		HrDepartmentTraining a
	inner join	
		gnMstOrgGroup b
	on
		a.CompanyCode = b.CompanyCode
		and
		a.Department = b.OrgCode
	inner join
		HrEmployeeAchievement c
	on
		a.CompanyCode = c.CompanyCode
		and
		a.Department = c.Department
	inner join
		gnMstPosition d
	on
		d.CompanyCode = a.CompanyCode
		and
		a.Department = a.Department
		and
		d.PosCode = a.Position	
	where
		a.CompanyCode = @CompanyCode
		and
		c.EmployeeID = @EmployeeID

	--select * from gnMstOrgGroup
end

go
exec uspfn_HrTrainingPositionList @CompanyCode='6115204', @EmployeeID='00152', @Department='SALES'