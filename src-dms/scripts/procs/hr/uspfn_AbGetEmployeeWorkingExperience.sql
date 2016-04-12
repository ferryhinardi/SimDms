if object_id('uspfn_AbGetEmployeeWorkingExperience') is not null
	drop procedure uspfn_AbGetEmployeeWorkingExperience

go
create procedure uspfn_AbGetEmployeeWorkingExperience (
	@CompanyCode varchar(17),
	@EmployeeID varchar(25)
)
as
begin
	select row_number() over (order by a.JoinDate) as [Index]
		 , a.CompanyCode
		 , a.EmployeeID
		 , a.NameOfCompany
		 , a.JoinDate
		 , a.ResignDate
		 , a.ReasonOfResign
		 , a.LeaderName
		 , a.LeaderPhone
		 , a.LeaderHP
	  from HrEmployeeExperience a
	 where 1=1
	   and a.CompanyCode=@CompanyCode
	   and a.EmployeeID=@EmployeeID
end

go
exec uspfn_AbGetEmployeeWorkingExperience @CompanyCode='6006406', @EmployeeID='123'

