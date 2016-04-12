begin transaction
declare
	@CompanyCode varchar(20),
	@DeptCode varchar(20),
	@PosCode varchar(20),
	@Status varchar(20),
	@BranchCode varchar(20)

select  @CompanyCode=N'6006406',@DeptCode=N'SALES',@PosCode=N'S',@Status=N'1',@BranchCode=N''

;with x as (
select a.CompanyCode
     , a.EmployeeID
	 , a.EmployeeName
	 , a.Department
	 , a.JoinDate
	 , LastDepartment = isnull((
			select top 1 Department from HrEmployeeAchievement
			 where CompanyCode = a.CompanyCode
			   and EmployeeID = a.EmployeeID
			 order by AssignDate desc
		), '')
	 , a.Position
	 , LastPosition = isnull((
			select top 1 Position from HrEmployeeAchievement
			 where CompanyCode = a.CompanyCode
			   and EmployeeID = a.EmployeeID
			 order by AssignDate desc
		), '')
	 , a.Grade
	 , LastGrade = isnull((
			select top 1 Grade from HrEmployeeAchievement
			 where CompanyCode = a.CompanyCode
			   and EmployeeID = a.EmployeeID
			 order by AssignDate desc
		), '')
  from HrEmployee a
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.Department = @DeptCode
   --and a.Position = @PosCode
   and a.PersonnelStatus = @Status
)
select * from x where Department != LastDepartment or Position != LastPosition

---- insert initial
--insert into HrEmployeeAchievement
--select top 0 * from HrEmployeeAchievement 
-- union all
--select CompanyCode, EmployeeID, JoinDate, Department, Position, isnull(Grade, ''), 1, 'system', getdate(), null, null, null
--  from x where Department != LastDepartment or Position != LastPosition and Department = ''

---- update different
--update x set Department = LastDepartment, Position = LastPosition
-- where Department != LastDepartment or Position != LastPosition

rollback transaction


--select * from HrEmployeeAchievement where EmployeeID = '371'

