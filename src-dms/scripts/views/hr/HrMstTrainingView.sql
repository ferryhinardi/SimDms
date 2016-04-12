
go
if object_id('HrMstTrainingView') is not null
	drop view HrMstTrainingView

go
create view HrMstTrainingView
as
select 
	a.CompanyCode,
	b.Department,
	b.Position,
	b.Grade,
	a.TrainingCode,
	a.TrainingName
from	
	HrMstTraining a
inner join
	HrDepartmentTraining b
on
	a.CompanyCode = b.CompanyCode
	and
	a.TrainingCode = b.TrainingCode
where
	a.IsDeleted != 1


go
select * from HrDepartmentTraining
