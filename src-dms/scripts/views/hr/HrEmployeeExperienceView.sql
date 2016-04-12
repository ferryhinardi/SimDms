if object_id('HrEmployeeExperienceView') is not null
	drop view HrEmployeeExperienceView

go
create view HrEmployeeExperienceView
as

select
	a.CompanyCode,
	a.EmployeeID,
	a.NameOfCompany,
	a.JoinDate,
	a.ResignDate,
	a.ReasonOfResign,
	a.LeaderName,
	a.LeaderPhone,
	a.LeaderHP
from
	HrEmployeeExperience a

go
select * from HrEmployeeExperienceView