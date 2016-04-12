

--select top 1 * from HrEmployeeAchievement
--select * from sfEmployeeTitleHistory
insert into HrEmployeeAchievement
select distinct
	x.CompanyCode,
	x.EmployeeID,
	x.AssignedDate,
	Department = (
		select top 1
			x1.DeptCode
		from 
			gnMstPosition x1
		where
			x1.PosCode = x.HistoryPosCode
	),
	x.HistoryPosCode,
	Grade = (
		select top 1
			x1.HistoryGrade
		from
			sfEmployeeGradeHistory x1
		where
			x1.CompanyCode = x.CompanyCode
			and
			x1.EmployeeID = x.EmployeeID
			and
			x1.AssignedDate = x.AssignedDate
	),
	0,
	'System',
	getDate(),
	'System',
	getDate()	
from 
	SfEmployeeTitleHistory x

