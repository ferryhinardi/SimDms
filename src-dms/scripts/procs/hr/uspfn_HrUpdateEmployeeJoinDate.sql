
go
if object_id('uspfn_HrUpdateEmployeeJoinDate') is not null
	drop procedure uspfn_HrUpdateEmployeeJoinDate

go
create procedure uspfn_HrUpdateEmployeeJoinDate
	@CompanyCode varchar(25),
	@EmployeeID varchar(25),
	@JoinDate datetime
as
begin
	update HrEmployee
	   set JoinDate = @JoinDate
	 where CompanyCode = @CompanyCode
	   and EmployeeID = EmployeeID
	   and JoinDate = @JoinDate
end

