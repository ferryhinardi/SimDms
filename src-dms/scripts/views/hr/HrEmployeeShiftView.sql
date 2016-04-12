
go
if object_id('HrEmployeeShiftView') is not null
	drop view HrEmployeeShiftView

go
create view HrEmployeeShiftView
as
select a.CompanyCode
     , b.Department
     , c.OrgName as DepartmentName
     , b.Position
     , b.Grade
     , b.Rank
     , d.PosName as PositionName
     , a.EmployeeID
     , b.EmployeeName
     , a.AttdDate
     , a.ShiftCode
     , e.ShiftName as Shift
     , a.ClockInTime
     , a.ClockOutTime
     , a.OnDutyTime
     , a.OffDutyTime
     , a.OnRestTime
     , a.OffRestTime
     , a.CalcOvertime
     , a.ApprOvertime
  from HrEmployeeShift a
  left join HrEmployee b on b.CompanyCode = a.CompanyCode and b.EmployeeID = a.EmployeeID
  left join gnMstOrgGroup c on c.CompanyCode = b.CompanyCode and c.OrgCode = b.Department and OrgGroupCode = 'DEPT'
  left join gnMstPosition d on d.CompanyCode = b.CompanyCode and d.DeptCode = b.Department and d.PosCode = b.Position
  left join HrShift e on e.CompanyCode = a.CompanyCode and e.ShiftCode = a.ShiftCode
  



