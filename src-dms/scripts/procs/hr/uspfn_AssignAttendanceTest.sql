
go
if object_id('uspfn_AbAssignAttendance') is not null
	drop procedure uspfn_AbAssignAttendance

go
create procedure uspfn_AbAssignAttendance
	@CompanyCode varchar(20),
	@FileID varchar(84)
as
begin
	declare @TransactioName varchar(25);
	set @TransactioName = 'SetEmployeeAttendance';

	create table #TableAttendance(
		CompanyCode varchar(10),
		EmployeeID varchar(20),
		IdentityCode varchar(5),
		IsTransfered bit,
		AttendanceTime datetime,
		AttendanceDate varchar(10),
		AttdDate varchar(10),
		ShifCode varchar(10),
	);

	;with x as (
		select distinct
		     a.CompanyCode
		   , a.EmployeeID
		   , a.IdentityCode
		   , a.IsTransfered
		   , a.AttendanceTime
		   , ( 	
		    	cast(datepart(year, a.AttendanceTime) as varchar(4)) + 
		    	right('0' + cast(datepart(month, a.AttendanceTime) as varchar(2)), 2) + 
		    	right('0' + cast(datepart(day, a.AttendanceTime) as varchar(2)), 2) 
		     ) as AttendanceDate
		   , b.AttdDate
		   , b.ShiftCode
		from HrTrnAttendanceFileDtl a
		left join HrEmployeeShift b
		  on b.EmployeeID=a.EmployeeID
		 and b.AttdDate = (
				cast(datepart(year, a.AttendanceTime) as varchar(4)) + 
		    	right('0' + cast(datepart(month, a.AttendanceTime) as varchar(2)), 2) + 
		    	right('0' + cast(datepart(day, a.AttendanceTime) as varchar(2)), 2) 
		      )
	   where a.FileID = @FileID
		 and b.ShiftCode is not null
	)
	
	insert into #TableAttendance
	select * from x



	declare @MinDate varchar(25);
	declare @MaxDate varchar(25);

	set @MinDate = (
		select top 1
			(
				cast(datepart(year, Min(a.AttendanceTime)) as varchar(4)) + 
				right('0' + cast(datepart(month, Min(a.AttendanceTime)) as varchar(2)), 2) + 
				right('0' + cast(datepart(day, Min(a.AttendanceTime)) as varchar(2)), 2) 
			)
		from
			#TableAttendance a
	);
	set @MaxDate= (
		select top 1
			(
				cast(datepart(year, Max(a.AttendanceTime)) as varchar(4)) + 
				right('0' + cast(datepart(month, Max(a.AttendanceTime)) as varchar(2)), 2) + 
				right('0' + cast(datepart(day, Max(a.AttendanceTime)) as varchar(2)), 2) 
			)
		from
			#TableAttendance a
	);

	--select EmployeeID
	--     , AttdDate
	--	 , ClockInTime = (
	--			select top 1
	--				   Convert(varchar(5), Convert(time, x.AttendanceTime))
	--			  from #TableAttendance x
	--			 where x.CompanyCode = HrEmployeeShift.CompanyCode
	--			   and x.EmployeeID = HrEmployeeShift.EmployeeID
	--			   and x.AttendanceDate = HrEmployeeShift.AttdDate
	--			   and ltrim(rtrim(replace(x.IdentityCode, char(13), ''))) = 'I'
	--	   )
	--	 , ClockOutTime = (
	--			select top 1
	--				   Convert(varchar(5), Convert(time, x.AttendanceTime))
	--			  from #TableAttendance x
	--			 where x.CompanyCode = HrEmployeeShift.CompanyCode
	--			   and x.EmployeeID = HrEmployeeShift.EmployeeID
	--			   and x.AttendanceDate = HrEmployeeShift.AttdDate
	--			   and ltrim(rtrim(replace(x.IdentityCode, char(13), ''))) = 'O'
	--	   )
	--	 --, OffDutyTime
	--	 , CalcOvertime = dbo.uspfn_CalculateMinute(
	--							HrEmployeeShift.OffDutyTime,
	--							(
	--								select top 1
	--									   Convert(varchar(5), Convert(time, x.AttendanceTime))
	--								  from #TableAttendance x
	--								 where x.CompanyCode = HrEmployeeShift.CompanyCode
	--								   and x.EmployeeID = HrEmployeeShift.EmployeeID
	--								   and x.AttendanceDate = HrEmployeeShift.AttdDate
	--								   and ltrim(rtrim(replace(x.IdentityCode, char(13), ''))) = 'O'
	--							)
	--					  )
 --        , ApprOvertime = 0
	--  from HrEmployeeShift 
	-- where HrEmployeeShift.CompanyCode = @CompanyCode
	--   and HrEmployeeShift.EmployeeID in ( select x.EmployeeID from #TableAttendance x )
	--   and HrEmployeeShift.AttdDate >= @MinDate
	--   and HrEmployeeShift.AttdDate <= @MaxDate
 --      and ISNULL(HrEmployeeShift.ShiftCode, '') != ''
 
	update HrEmployeeShift
	   set ClockInTime = (
				select top 1
					   Convert(varchar(5), Convert(time, x.AttendanceTime))
				  from #TableAttendance x
				 where x.CompanyCode = HrEmployeeShift.CompanyCode
				   and x.EmployeeID = HrEmployeeShift.EmployeeID
				   and x.AttendanceDate = HrEmployeeShift.AttdDate
				   and ltrim(rtrim(replace(x.IdentityCode, char(13), ''))) = 'I'
		   )
		 , ClockOutTime = (
				select top 1
					   Convert(varchar(5), Convert(time, x.AttendanceTime))
				  from #TableAttendance x
				 where x.CompanyCode = HrEmployeeShift.CompanyCode
				   and x.EmployeeID = HrEmployeeShift.EmployeeID
				   and x.AttendanceDate = HrEmployeeShift.AttdDate
				   and ltrim(rtrim(replace(x.IdentityCode, char(13), ''))) = 'O'
		   )
		 , CalcOvertime = dbo.uspfn_CalculateMinute(
								HrEmployeeShift.OffDutyTime,
								(
									select top 1
										   Convert(varchar(5), Convert(time, x.AttendanceTime))
									  from #TableAttendance x
									 where x.CompanyCode = HrEmployeeShift.CompanyCode
									   and x.EmployeeID = HrEmployeeShift.EmployeeID
									   and x.AttendanceDate = HrEmployeeShift.AttdDate
									   and ltrim(rtrim(replace(x.IdentityCode, char(13), ''))) = 'O'
								)
						  )
         , ApprOvertime = 0
	 where HrEmployeeShift.CompanyCode = @CompanyCode
	   and HrEmployeeShift.EmployeeID in ( select x.EmployeeID from #TableAttendance x )
	   and HrEmployeeShift.AttdDate >= @MinDate
	   and HrEmployeeShift.AttdDate <= @MaxDate
       and HrEmployeeShift.ShiftCode is not null





	update
		HrTrnAttendanceFileDtl
	set
		IsTransfered = 1
	where
		FileID = @FileID
		and
		HrTrnAttendanceFileDtl.EmployeeID in ( select x.EmployeeID from #TableAttendance x )
		and
		(
			cast(datepart(year, HrTrnAttendanceFileDtl.AttendanceTime) as varchar(4)) + 
		    right('0' + cast(datepart(month, HrTrnAttendanceFileDtl.AttendanceTime) as varchar(2)), 2) + 
		    right('0' + cast(datepart(day, HrTrnAttendanceFileDtl.AttendanceTime) as varchar(2)), 2) 
		) in (
			select x.AttendanceDate
			  from #TableAttendance x
			 where x.CompanyCode = HrTrnAttendanceFileDtl.CompanyCode
			   and x.EmployeeID = HrTrnAttendanceFileDtl.EmployeeID
		);


	declare @totalRecord int;
	declare @processedRecord int;
	declare @unprocessedRecord int;
	declare @fileStatus int;
	declare @Temp table (
		CompanyCode varchar(17),
		EmployeeID varchar(25),
		AttendanceTime datetime,
		IsTransfered bit
	);

	delete @Temp;

	insert into @Temp
	select distinct
		   a.CompanyCode
		 , a.EmployeeID
		 , a.AttendanceTime
		 , a.IsTransfered
	  from HrTrnAttendanceFileDtl a
	 where a.FileID = @FileID

	set @totalRecord = (select count(CompanyCode) from @Temp);

	set @processedRecord = (select count(CompanyCode) from @Temp where IsTransfered = 1);

	set @unprocessedRecord = (select count(CompanyCode) from @Temp where IsTransfered = 0 or IsTransfered is null);

	if @unprocessedRecord = @totalRecord
	begin
		set @fileStatus = 0;	
	end

	if @unprocessedRecord > 0 and @unprocessedRecord < @totalRecord
	begin
		set @fileStatus = 1;	
	end

	if @processedRecord = @totalRecord
	begin
		set @fileStatus = 1;	
	end

	update
			HrTrnAttendanceFileHdr
		set
			IsTransfered = @fileStatus
		where
			FileID = @FileID;
end



go
if OBJECT_ID('uspfn_CalculateMinute') is not null
	drop function uspfn_CalculateMinute
	
go
create function uspfn_CalculateMinute (
	@Time1 time,
	@Time2 time
)
returns int
as
begin
	declare @Difference int;
	set @Difference = DATEDIFF(minute, @Time1, @Time2);
	return abs(@Difference);
end

go
--select dbo.uspfn_CalculateMinute ('19:30:00', '23:00:00')