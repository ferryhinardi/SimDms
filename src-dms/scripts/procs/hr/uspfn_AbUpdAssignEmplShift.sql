
go
if object_id('uspfn_AbUpdAssignEmplShift') is not null
	drop procedure uspfn_AbUpdAssignEmplShift

go
create proc uspfn_AbUpdAssignEmplShift
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@ShiftEdit varchar(50),
	@AttdDateEdit datetime,
	@EmployeeIDEdit varchar(10),
	@UserID varchar(20)
as
if @ShiftEdit = '-'
begin
	update HrEmployeeShift
	   set ShiftCode = ''
		 , UpdatedBy = @UserID
		 , UpdatedDate = getdate()
	 where CompanyCode = @CompanyCode
	   and EmployeeID = @EmployeeIDEdit
	   and AttdDate = @AttdDateEdit
end
else
begin
;with x as (
	select a.CompanyCode, a.AttdDate, b.Department, b.Position, a.EmployeeID, a.ShiftCode
		 , a.OnDutyTime, a.OffDutyTime, a.OnRestTime, a.OffRestTime
		 , a.UpdatedBy, a.UpdatedDate
		 , c.ShiftCode as ShiftCodeTarget
		 , c.OnDutyTime as OnDutyTimeTarget
		 , c.OffDutyTime as OffDutyTimeTarget
		 , c.OnRestTime as OnRestTimeTarget
		 , c.OffRestTime as OffRestTimeTarget
	  from HrEmployeeShift a
	  left join HrEmployee b
		on b.CompanyCode = a.CompanyCode
	   and b.EmployeeID = a.EmployeeID
	  left join HrShift c
		on c.CompanyCode = a.CompanyCode
	   and c.ShiftCode = @ShiftEdit 
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and b.EmployeeID = @EmployeeIDEdit
	   and a.AttdDate = @AttdDateEdit
	)
	update x set ShiftCode = ShiftCodeTarget
			   , UpdatedBy = @UserID
			   , UpdatedDate = getdate()
end
