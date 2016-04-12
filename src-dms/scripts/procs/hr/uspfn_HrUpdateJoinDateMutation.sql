
go
if object_id('uspfn_HrUpdateJoinDateMutation') is not null
	drop procedure uspfn_HrUpdateJoinDateMutation

go
create procedure uspfn_HrUpdateJoinDateMutation
	@CompanyCode varchar(25),
	@EmployeeID varchar(25),
	@JoinDate datetime,
	@ResignDate datetime

as
begin
	update HrEmployee
	   set JoinDate = @JoinDate
	 where CompanyCode = @CompanyCode
	   and EmployeeID = EmployeeID
	   and JoinDate = @JoinDate

	begin try
		update HrEmployeeMutation
		   set MutationDate=@JoinDate
		 where CompanyCode=@CompanyCode
		   and EmployeeID=@EmployeeID
		   and IsJoinDate=1
	end try
	begin catch
		update HrEmployeeMutation
		   set IsDeleted=0
		     , IsJoinDate=1
		 where CompanyCode=@CompanyCode
		   and EmployeeID=@EmployeeID
		   and convert(datetime, MutationDate)=@JoinDate
	end catch

	--update HrEmployeeMutation
	--   set IsDeleted=1
	-- where CompanyCode=@CompanyCode
	--   and EmployeeID=@EmployeeID
	--   and convert(datetime,MutationDate) < @JoinDate

	update HrEmployeeMutation
	   set IsDeleted=1
	 where CompanyCode=@CompanyCode
	   and EmployeeID=@EmployeeID
	   and MutationDate < @JoinDate

	update HrEmployeeMutation
	   set IsDeleted=1
	 where CompanyCode=@CompanyCode
	   and EmployeeID=@EmployeeID
	   and convert(datetime, MutationDate) > @ResignDate
end

