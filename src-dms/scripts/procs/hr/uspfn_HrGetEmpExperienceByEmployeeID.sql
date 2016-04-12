
go
if object_id('uspfn_HrGetEmpExperienceByEmployeeID') is not null
	drop procedure uspfn_HrGetEmpExperienceByEmployeeID

go
create procedure uspfn_HrGetEmpExperienceByEmployeeID
	@CompanyCode varchar(25),
	@EmployeeID varchar(25)
as
select a.EmployeeID, a.NameOfCompany, convert(varchar(12), a.JoinDate,106) as JoinDate,
	Convert(varchar(12), a.ResignDate,106) as ResignDate, 
	Substring(a.ReasonOfResign,0,25) as ReasonOfResignShort,
	a.ReasonOfResign,
	a.LeaderName,
	a.LeaderPhone,
	a.LeaderHP,
	a.ExpSeq
from HrEmployeeExperience a
where a.CompanyCode = @CompanyCode
  and a.EmployeeID = @EmployeeID
order BY a.JoinDate asc