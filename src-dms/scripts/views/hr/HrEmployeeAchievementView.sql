
go
if object_id('HrEmployeeAchievementView') is not null
	drop view HrEmployeeAchievementView

go
create view HrEmployeeAchievementView
as
select a.CompanyCode
     , a.EmployeeID
     , b.EmployeeName
     , a.Department
     , a.Position
     , a.Grade
     , a.AssignDate
     , a.IsJoinDate
     , DepartmentName = a.Department
     , PositionName = (
		select top 1 Upper(x.PosName)
		  from gnMstPosition x
		 where x.CompanyCode = a.CompanyCode
		   and x.DeptCode = a.Department
		   and x.PosCode = a.Position)
     , GradeName = (
		select top 1 Upper(x.LookUpValueName)
		  from gnMstLookUpDtl x
		 where x.CompanyCode = a.CompanyCode
		   and x.CodeID = 'ITSG'
		   and x.LookUpValue = a.Grade)
     , IsInitialPosition = (case a.IsJoinDate when 1 then 'YES' else 'NO' end)
     , AssignDateStatus = (case a.IsJoinDate when 1 then 'Join Position' else 'Promotion' end)
  from HrEmployeeAchievement a
  left join HrEmployee b
    on a.CompanyCode = b.CompanyCode
   and a.EmployeeID = b.EmployeeID
 where isnull(a.IsDeleted, '') = ''
    or a.IsDeleted = 0