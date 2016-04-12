begin transaction
declare @CompanyCode varchar(20)
 select @CompanyCode=N'6006406'

;with x as (
select a.CompanyCode
     , a.EmployeeID
	 , a.EmployeeName
	 , a.Department
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
	 , a.JoinDate, a.ResignDate
  from HrEmployee a
 where a.CompanyCode = @CompanyCode
   and a.JoinDate is not null
)
select * from x 
 where Department != LastDepartment or Position != LastPosition

---- insert initial
--insert into HrEmployeeAchievement
--select top 0 * from HrEmployeeAchievement 
-- union all
--select distinct CompanyCode, EmployeeID, JoinDate, Department, Position, isnull(Grade, ''), 1, 'system', getdate(), null, null, null
--  from x where (Department != LastDepartment or Position != LastPosition and Department = '')
--   and not exists(select 1 from HrEmployeeAchievement where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and AssignDate = x.JoinDate)

---- update different
--update x set Department = LastDepartment, Position = LastPosition
-- where Department != LastDepartment or Position != LastPosition

rollback transaction


--select * from HrEmployeeAchievement where EmployeeID = '371'

