
go
if object_id('uspfn_AbExtractData') is not null	
	drop procedure uspfn_AbExtractData

GO
create procedure uspfn_AbExtractData
	@FileID varchar(100),
	@FileContent varchar(max),
	@UserId varchar(25)
as
begin


	begin try
		declare c cursor for 
			select * from uspfn_AbSplitString(@FileContent, char(10))
		declare @Record varchar(100);
		declare @EmployeeID varchar(17);
		declare @AttendanceTime varchar(20);
		declare @MachineCode varchar(2);
		declare @IdentityCode varchar(1);
		declare @CurrentDate datetime;
		declare @CompanyCode varchar(17);
		declare @TrailingZeroIndex tinyint;
		set @CompanyCode = (
			select top 1
			       CompanyCode
			  from gnMstOrganizationHdr 
		);

		
		select * into #a 
	     from uspfn_AbSplitString(@FileContent, char(10)+char(13))

		declare @NumberOfIteratorRecords int;
		declare @NumberOfRecord int;
		set @NumberOfIteratorRecords = ( select count(*) from SimDmsIterator );
		set @NumberOfRecord = (select count(*) from #a);

		if @NumberOfIteratorRecords = 0
		begin
			insert into SimDmsIterator (AttendanceFlatFileExtractionProcessed, AttendanceFlatFileExtractionTotal)
			values (0, 100)
		end

		update SimDmsIterator
		   set AttendanceFlatFileExtractionProcessed = 0
		     , AttendanceFlatFileExtractionTotal = @NumberOfRecord
			 

		open c
			fetch next from c into @Record

			declare @iterator int;
			set @iterator=1;
			while @@FETCH_STATUS=0
			begin
				set @EmployeeID = substring(@Record, 0, 7);
				set @TrailingZeroIndex = ( select patindex('%[^0]%', @EmployeeID) );
				set @EmployeeID = substring(@EmployeeID, @TrailingZeroIndex, 8-@TrailingZeroIndex);
				set @AttendanceTime = substring(@Record, 8, 19);
				set @MachineCode = substring(@Record, 28, 1);
				set @IdentityCode = substring(@Record, 30, 1);
				
				--select @AttendanceTime as AttendanceTime;
				exec uspfn_AbInsertAttendanceData 
				     @CompanyCode = @CompanyCode
				   , @FileID = @FileID
				   , @Iterator = @Iterator
				   , @EmployeeID = @EmployeeID
				   , @AttendanceTime = @AttendanceTime
				   , @MachineCode = @MachineCode
				   , @IdentityCode = @IdentityCode
				   , @UserId = @UserID

				update SimDmsIterator 
				   set AttendanceFlatFileExtractionProcessed = @iterator	

				set @iterator = @iterator + 1;
				fetch next from c into @Record
			end
		close c
		deallocate c

		select Convert(bit, 1);
	end try
	begin catch
		select Convert(bit, 0);
	end catch
end