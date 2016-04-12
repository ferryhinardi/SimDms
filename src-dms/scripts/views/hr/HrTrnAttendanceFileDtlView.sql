
if object_id('HrTrnAttendanceFileDtlView') is not null
	drop view HrTrnAttendanceFileDtlView

go
create view HrTrnAttendanceFileDtlView
as
select
	a.FileID,
	a.CompanyCode,
	a.EmployeeID,
	a.EmployeeName,
	a.MachineCode,
	a.AttendanceTime,
	ClockTime = left(convert(varchar, a.AttendanceTime, 108), 5),
	a.IdentityCode,
	AttendanceStatus = (
		case 
			when ltrim(rtrim(replace(a.IdentityCode, char(13), ''))) = 'O' then 'OUT'
			when ltrim(rtrim(replace(a.IdentityCode, char(13), ''))) = 'I' then 'IN'
		end
	),
	(	
		cast(datepart(year, a.AttendanceTime) as varchar(4)) + 
		right('0' + cast(datepart(month, a.AttendanceTime) as varchar(2)), 2) + 
		right('0' + cast(datepart(day, a.AttendanceTime) as varchar(2)), 2) 
	) as AttendanceDate,
	Shift = isnull((
		select top 1
			y.ShiftName
		from
			HrEmployeeShift x
		left join HrShift y
		  on y.CompanyCode=x.CompanyCode
		 and y.ShiftCode=x.ShiftCode
		where
			x.CompanyCode = a.CompanyCode
			and
			x.EmployeeID = a.EmployeeID
			and
			x.AttdDate = (	
				cast(datepart(year, a.AttendanceTime) as varchar(4)) + 
				right('0' + cast(datepart(month, a.AttendanceTime) as varchar(2)), 2) + 
				right('0' + cast(datepart(day, a.AttendanceTime) as varchar(2)), 2) 
			)
	), '-'),
	Status =  (
		case
			when a.IsTransfered = 0 then 'N'
			when a.IsTransfered = 1 then 'Y'
		end
	)
from
	HrTrnAttendanceFileDtl a

GO


select * from HrTrnAttendanceFileDtlView
