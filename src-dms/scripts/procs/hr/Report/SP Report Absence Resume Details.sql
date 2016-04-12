if object_id('uspfn_abInqAttendanceResumeDetails') is not null
	drop procedure uspfn_abInqAttendanceResumeDetails

go
create procedure uspfn_abInqAttendanceResumeDetails
	@CompanyCode nvarchar(25),
	@Department nvarchar(17),
	@Position nvarchar(17),
	@Grade nvarchar(17),
	@DateFrom datetime,
	@DateTo datetime,
	@ShiftCode nvarchar(17),
	@EmployeeID varchar(25),
	@EmployeeName varchar(200),
	@State char(1)
as
begin
	  create table #Attendance (
		CompanyCode varchar(17),
		EmployeeID varchar(25),
		EmployeeName varchar(150),
		Department varchar(100),
		Position varchar(55),
		Grade varchar(25),
		DepartmentName varchar(55),
		PositionName varchar(200),
		GradeName varchar(25),
		AttendanceDate char(8),
		ShiftCode varchar(7),
		ShiftName varchar(25),
		IsAbsence varchar(5),
		ClockInTime char(5),
		ClockOutTime char(5),
		OnDutyTime char(5),
		OffDutyTime char(5),
		CalcOvertime int,
		Overtime char(5),
		CalcLateTime int,
		LateTime char(5),
		CalcReturnBeforeTheTime int, 
		ReturnBeforeTheTime char(5)
	  );
	  
	  create table #ResumeAttendance (
		CompanyCode varchar(17),
		EmployeeID varchar(25),
		EmployeeName varchar(150),
		Department varchar(100),
		Position varchar(55),
		Grade varchar(25),
		DepartmentName varchar(55),
		PositionName varchar(200),
		GradeName varchar(25),
		AttendanceDate char(8),
		ShiftCode varchar(7),
		ShiftName varchar(25),
		IsAbsence varchar(5),
		ClockInTime char(5),
		ClockOutTime char(5),
		OnDutyTime char(5),
		OffDutyTime char(5),
		CalcOvertime int,
		Overtime char(5),
		CalcLateTime int,
		LateTime char(5),
		CalcReturnBeforeTheTime int, 
		ReturnBeforeTheTime char(5)
	  );

	  delete #Attendance

	  insert into #Attendance
	  select  a.CompanyCode
			, a.EmployeeID
			, b.EmployeeName
			, b.Department
			, b.Position
			, b.Grade
			, DepartmentName = c.OrgName
			, PositionName = d.PosName
			, GradeName = e.LookUpValueName
			, a.AttdDate 
			, a.ShiftCode
			, f.ShiftName
			, IsAbsence = (
				case
					when a.ClockInTime is null or a.ClockInTime = '' then 'Yes'
					else 'No'
				end
			  )
			, a.ClockInTime
			, a.ClockOutTime
			, a.OnDutyTime
			, a.OffDutyTime
			, isnull(a.CalcOvertime, 0)
			, Overtime = (
				case
					when a.ClockOutTime > a.OffDutyTime then dbo.uspfn_MinuteToTime(dbo.uspfn_CalculateMinute(left(convert(varchar, a.OffDutyTime, 108), 5), left(convert(varchar, a.ClockOutTime, 108), 5)))
					else '-'
				end
		      )
			, CalcLateTime = (
				case 
					when a.ClockInTime > a.OnDutyTime then datediff(minute, a.OnDutyTime, a.ClockInTime)
					else 0
				end
			  )
			, LateTime = (
				case
					when a.ClockInTime > a.OnDutyTime then dbo.uspfn_MinuteToTime(dbo.uspfn_CalculateMinute(left(convert(varchar, a.OnDutyTime, 108), 5), left(convert(varchar, a.ClockInTime, 108), 5)))
					else '-'
				end
		      )
			, CalcReturnBeforeTheTime = (
				case
					when a.ClockOutTime < a.OffDutyTime then dbo.uspfn_CalculateMinute(left(convert(varchar, a.ClockOutTime, 108), 5), left(convert(varchar, a.OffDutyTime, 108), 5))
					else '-'
				end
		      )
			, ReturnBeforeTheTime = (
				case
					when a.ClockOutTime < a.OffDutyTime then dbo.uspfn_MinuteToTime(dbo.uspfn_CalculateMinute(left(convert(varchar, a.ClockOutTime, 108), 5), left(convert(varchar, a.OffDutyTime, 108), 5)))
					else '-'
				end
		      )
		from HrEmployeeShift a
		left join HrEmployee b
		  on a.CompanyCode=b.CompanyCode
		 and a.EmployeeID=b.EmployeeID
		left join gnMstOrgGroup c
		  on c.CompanyCode=a.CompanyCode
		 and c.OrgCode=b.Department
		left join gnMstPosition d
		  on d.CompanyCode=c.CompanyCode
		 and d.DeptCode=b.Department
		 and d.PosCode=b.Position
		left join gnMstLookUpDtl e
		  on e.CompanyCode=a.CompanyCode
		 and e.CodeID='ITSG'
		 and e.LookUpValue=b.Grade
		left join HrShift f
		  on f.CompanyCode=a.CompanyCode
		 and f.ShiftCode=a.ShiftCode
	   where a.CompanyCode=@CompanyCode 
		 and a.AttdDate>=replace(convert(varchar, @DateFrom, 111), '/', '')
		 and a.AttdDate<=replace(convert(varchar, @DateTo, 111), '/', '')
		 and a.ClockInTime is not null

		declare @Query nvarchar(1000);
		declare @ParameterDeclaration nvarchar(1000);
		set @Query='insert into #ResumeAttendance select * from #Attendance a where 1=1 ';
		set @ParameterDeclaration='';
			
		if ltrim(rtrim(@Department)) != '' or @Department is null
		begin
			set @Query = @Query + ' and a.Department=@p0';
			set @ParameterDeclaration = @ParameterDeclaration + N' @p0 nvarchar(17),' 
		end
		else
		begin 
			set @Department='%';
			set @Query = @Query + ' and a.Department like @p0';
			set @ParameterDeclaration = @ParameterDeclaration + N' @p0 nvarchar(17),' 
		end

		if ltrim(rtrim(@Position)) != ''
		begin
			set @Query = @Query + ' and a.Position=@p1';
			set @ParameterDeclaration = @ParameterDeclaration + N' @p1 nvarchar(17),' 
		end
		else
		begin
			set @Position='%';
			set @Query = @Query + ' and a.Position like @p1';
			set @ParameterDeclaration = @ParameterDeclaration + N' @p1 nvarchar(17),' 
		end

		if ltrim(rtrim(@Grade)) != ''
		begin
			set @Query = @Query + ' and a.Grade=@p2';
			set @ParameterDeclaration = @ParameterDeclaration + N' @p2 nvarchar(17),' 
		end
		else
		begin
			set @Grade = '%'
			set @Query = @Query + ' and a.Grade like @p2';
			set @ParameterDeclaration = @ParameterDeclaration + N' @p2 nvarchar(17),' 
		end

		if ltrim(rtrim(@ShiftCode)) != ''
		begin
			set @Query = @Query + ' and a.ShiftCode=@p3';
			set @ParameterDeclaration = @ParameterDeclaration + N' @p3 nvarchar(17),' 
		end
		else
		begin
			set @ShiftCode='%'
			set @Query = @Query + ' and a.ShiftCode like @p3';
			set @ParameterDeclaration = @ParameterDeclaration + N' @p3 nvarchar(17),' 
		end
		
		if ltrim(rtrim(@EmployeeID)) != ''
		begin
			set @Query = @Query + ' and a.EmployeeID=@p4';
			set @ParameterDeclaration = @ParameterDeclaration + N' @p4 nvarchar(25),' 
		end
		else
		begin
			set @EmployeeID='%'
			set @Query = @Query + ' and a.EmployeeID like @p4';
			set @ParameterDeclaration = @ParameterDeclaration + N' @p4 nvarchar(25),' 
		end
		
		if ltrim(rtrim(@EmployeeName)) != ''
		begin
			set @EmployeeName='%' + @EmployeeName + '%'
			set @Query = @Query + ' and a.EmployeeName like @p5';
			set @ParameterDeclaration = @ParameterDeclaration + N' @p5 nvarchar(200),' 
		end
		else
		begin
			set @EmployeeName='%'
			set @Query = @Query + ' and a.EmployeeName like @p5';
			set @ParameterDeclaration = @ParameterDeclaration + N' @p5 nvarchar(200),' 
		end

		set @ParameterDeclaration = substring(@ParameterDeclaration, 0, len(@ParameterDeclaration));
		exec sp_executesql @statement=@Query
		                 , @parameters=@ParameterDeclaration
						 , @p0=@Department
						 , @p1=@Position
						 , @p2=@Grade
						 , @p3=@ShiftCode
						 , @p4=@EmployeeID
						 , @p5=@EmployeeName
		
		declare @EmployeeOnTime int;
		declare @EmployeeLate int;
		declare @EmployeeReturnBeforeTheTime int;
		declare @EmployeeOvertime int;
		
		set @EmployeeOnTime = (select COUNT(EmployeeID) from #ResumeAttendance where ClockInTime <= OnDutyTime);
		set @EmployeeLate = (select COUNT(EmployeeID) from #ResumeAttendance where ClockInTime > OnDutyTime);
		set @EmployeeReturnBeforeTheTime = (select COUNT(EmployeeID) from #ResumeAttendance where ClockOutTime < OffDutyTime);
		set @EmployeeOvertime = (select COUNT(EmployeeID) from #ResumeAttendance where ClockOutTime > OffDutyTime);
		
		if @State='1'
		begin
			select * from #ResumeAttendance where ClockInTime <= OnDutyTime
		end
		
		if @State='2'
		begin
			select * from #ResumeAttendance where ClockInTime > OnDutyTime
		end
		
		if @State='3'
		begin
			select * from #ResumeAttendance where ClockOutTime < OffDutyTime
		end
		
		if @State='4'
		begin
			select * from #ResumeAttendance where ClockOutTime > OffDutyTime
		end
		
		drop table #ResumeAttendance
		drop table #Attendance
end




go
exec uspfn_abInqAttendanceResumeDetails @CompanyCode='6006406'
									  , @Department='SALES'
									  , @Position='S'
									  , @Grade='' 
									  , @DateFrom='2013-11-01'
									  , @DateTo='2013-11-29'
									  , @ShiftCode=''
									  , @EmployeeID=''
									  , @EmployeeName=''
									  , @State='2'
