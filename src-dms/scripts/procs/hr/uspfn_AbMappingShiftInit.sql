
go
if object_id('uspfn_AbMappingShiftInit') is not null
	drop procedure uspfn_AbMappingShiftInit

go
create procedure uspfn_AbMappingShiftInit
	@CompanyCode varchar(20),
	@BranchCode varchar(20), 
	@Department varchar(20),
	@Position varchar(20), 
	@DateFrom datetime = '19000101',
	@DateTo datetime = '19000101',
	@UserID varchar(20)
as

declare @sDateFrom char(8), @sDateTo char(8), @ActDate datetime
select @sDateFrom = convert(varchar, @DateFrom, 112), @sDateTo = convert(varchar, @DateTo, 112)

if (@sDateFrom > '19000101') and (@sDateFrom <= @sDateTo) 
begin
	set @ActDate = @DateFrom

	while (@ActDate <= @DateTo)
	begin
		print @ActDate
		
		insert into HrEmployeeShift (CompanyCode, EmployeeID, AttdDate, ShiftCode, CreatedBy, CreatedDate)
		select a.CompanyCode, a.EmployeeID, convert(varchar, @ActDate, 112), '', @UserID, getdate()
		  from HrEmployee a
		 where a.CompanyCode = @CompanyCode
		   and a.Department = @Department
		   and a.Position = (case @Position when '' then a.Position else @Position end)
		   and not exists (
			select 1 from HrEmployeeShift
			 where CompanyCode = a.CompanyCode
			   and EmployeeID = a.EmployeeID
			   and AttdDate = convert(varchar, @ActDate, 112)
		   )

		set @ActDate = dateadd(day, 1, @ActDate)
	end
end





