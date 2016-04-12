
go
if object_id('uspfn_HrGetDetailsEmployeePosition') is not null
	drop procedure uspfn_HrGetDetailsEmployeePosition

go
create procedure uspfn_HrGetDetailsEmployeePosition(
	@CompanyCode varchar(15),
	@EmployeeID varchar(15),
	@ValidDate varchar(11)
)
as 
begin
	select top 1
		DepartmentCode = a.Department,
		PositionCode = a.Position,
		GradeCode = a.Grade,
		Department = (
			select top 1
				x.OrgName
			from
				gnMstOrgGroup x
			where
				x.CompanyCode = a.CompanyCode
				and
				x.OrgGroupCode = 'DEPT'
				and
				x.OrgCode = a.Department
		),
		Position = (
			select top 1
				x.PosName
			from
				gnMstPosition x
			where
				x.CompanyCode = a.CompanyCode
				and
				x.DeptCode = a.Department
				and
				x.PosCode = a.Position
		),
		Grade = (
			select top 1
				x.LookUpValueName
			from
				gnMstLookUpDtl x
			where
				x.CompanyCode = a.CompanyCode
				and
				x.CodeID = 'ITSG'
				and
				x.LookUpValue = a.Grade
		)
	from
		HrEmployeeAchievement a
	where
		a.CompanyCode = @CompanyCode
		and
		a.EmployeeID = @EmployeeID	
		and
		a.AssignDate <= @ValidDate
		and
		(a.IsDeleted = 0 or a.IsDeleted is null)
	order by
		a.AssignDate desc
end

go
exec uspfn_HrGetDetailsEmployeePosition @CompanyCode='6115204', @EmployeeID='00152', @ValidDate='2014-10-02'


