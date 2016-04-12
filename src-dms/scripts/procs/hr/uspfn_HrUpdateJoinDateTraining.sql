
go
if object_id('uspfn_HrUpdateJoinDateTraining') is not null
	drop procedure uspfn_HrUpdateJoinDateTraining

go
create procedure uspfn_HrUpdateJoinDateTraining
	@CompanyCode varchar(25),
	@EmployeeID varchar(25),
	@JoinDate datetime,
	@ResignDate datetime

as
begin
	update HrEmployeeTraining
	   set IsDeleted=1
	 where CompanyCode=@CompanyCode
	   and EmployeeID=@EmployeeID
	   and convert(datetime,TrainingDate) < @JoinDate

	update HrEmployeeTraining
	   set IsDeleted=1
	 where CompanyCode=@CompanyCode
	   and EmployeeID=@EmployeeID
	   and convert(datetime, TrainingDate) > @ResignDate
end

