
go
if object_id('uspfn_AbInqAttendance') is not null
	drop procedure uspfn_AbInqAttendance

go
create procedure uspfn_AbInqAttendance
	@CompanyCode varchar(12),
	@EmployeeID varchar(25),
	@EmployeeName varchar(100),
	@DateFrom datetime,
	@DateTo datetime
as

begin
	if dbo.uspfn_IsNullOrEmpty(@EmployeeID) = 1 
	begin
		set @EmployeeID = '';
	end

	if dbo.uspfn_IsNullOrEmpty(@EmployeeName) = 1 
	begin
		set @EmployeeName = '';
	end

	select a.EmployeeID
	     , b.EmployeeName
		 , d.OrgName as Department
		 , c.PosName as Position
		 , isnull(e.LookUpValueName, '-') as Grade
		 , AttendanceDate = convert(datetime, (
				SUBSTRING(a.AttdDate, 0, 5) +
				'-' +
				SUBSTRING(a.AttdDate, 5, 2) +
				'-' +
				SUBSTRING(a.AttdDate, 7, 2) 
		   ))
	     --, a.AttdDate
	     , a.OnDutyTime
		 , a.OffDutyTime
	     , a.ClockInTime
		 , a.ClockOutTime
		 , IsAbsence = (
				case 
					when a.ClockInTime is null and a.ClockOutTime is null then 'Yes'
					when a.ClockInTime is not null then 'No'
					when a.ClockInTime is null then 'Yes'
				end
		   )
		 , LateTime = (
				case
					when a.ClockInTime > a.OnDutyTime then dbo.uspfn_MinuteToTime(dbo.uspfn_CalculateMinute(convert(char(5), a.OnDutyTime), convert(char(5), a.ClockInTime)))
					else '-'
				end
		   )
		 , ReturnBeforeTheTime = (
				case
					when a.ClockOutTime < a.OffDutyTime then dbo.uspfn_MinuteToTime(dbo.uspfn_CalculateMinute(convert(char(5), a.ClockOutTime), convert(char(5), a.OffDutyTime)))
					else '-'
				end
		   )
		 , Overtime = (
				case
					when a.ClockOutTime > a.OffDutyTime then dbo.uspfn_MinuteToTime(dbo.uspfn_CalculateMinute(convert(char(5), a.OffDutyTime), convert(char(5), a.ClockOutTime)))
					else '-'
				end
		   )
		 , WorkingTime = dbo.uspfn_MinuteToTime(dbo.uspfn_CalculateMinute(convert(char(5), a.ClockInTime), convert(char(5), a.ClockOutTime)))
         , Notes = ''
	  from HrEmployeeShift a
	 inner join HrEmployee b
	    on a.CompanyCode=b.CompanyCode
	   and a.EmployeeID=b.EmployeeID
	 inner join gnMstPosition c
	    on c.CompanyCode=a.CompanyCode
	   and c.DeptCode=b.Department
	   and c.PosCode=b.Position
	 inner join gnMstOrgGroup d
	    on d.CompanyCode=a.CompanyCode
	   and d.OrgCode=b.Department
	  left join gnMstLookUpDtl e
	    on e.CompanyCode=a.CompanyCode
	   and e.CodeID='ITSG'
	   and e.LookUpValue=b.Grade
 	 where a.CompanyCode=@CompanyCode 
       and a.AttdDate>=(
				convert(varchar(4), datepart(year, @DateFrom)) +
				right((replicate('0', 1) + convert(varchar(2), datepart(month, @DateFrom))), 2) +
				right((replicate('0', 1) + convert(varchar(2), datepart(day, @DateFrom))), 2)
	       )
	   and a.AttdDate<=(
				convert(varchar(4), datepart(year, @DateTo)) +
				right((replicate('0', 1) + convert(varchar(2), datepart(month, @DateTo))), 2) +
				right((replicate('0', 1) + convert(varchar(2), datepart(day, @DateTo))), 2)
	       )
	   and a.EmployeeID like '%' + @EmployeeID + '%'
	   and a.EmployeeID in (
			select x.EmployeeID
			  from HrEmployee x
			 where x.EmployeeName like '%' + @EmployeeName + '%'
			   and x.EmployeeID like '%' + @EmployeeID + '%'
	   ) 
		
	 order by a.AttdDate asc
end




go
if object_id('uspfn_MinuteToTime') is not null
	drop function uspfn_MinuteToTime

go
create function dbo.uspfn_MinuteToTime (
	@Minute int
)
returns varchar(5)
as

begin
	declare @Result varchar(5);

	if @Minute != 0 and @Minute != '0'
		set @Result = convert(varchar, dateadd(minute, @Minute, '00:00'), 108)
	else
		set @Result='';

	return @Result;
end
--
--select convert(varchar, dateadd(minute, 100, '00:00'), 108)


