go
if OBJECT_ID('HrEmployeeTrainingView') is not null
	drop view HrEmployeeTrainingView
	
go
create view HrEmployeeTrainingView
as
select 
	a.CompanyCode,
	a.EmployeeID,
	b.EmployeeName, 
	a.TrainingCode,
	TrainingName = (
		select top 1
			x.TrainingName
		from 
			HrMstTraining x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.TrainingCode = a.TrainingCode
	),
	a.TrainingDate,
	a.PostTest,
	a.PostTestAlt,
	a.PreTest,
	a.PreTestAlt
from 
	HrEmployeeTraining a
inner join
	HrEmployee b
on
	a.CompanyCode = b.CompanyCode
	and
	a.EmployeeID = b.EmployeeID
	and
	a.IsDeleted != 1
go 
select * from HrEmployeeTrainingView