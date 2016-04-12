
go
if object_id('uspfn_AbExtractData_II') is not null	
	drop procedure uspfn_AbExtractData_II


GO
create procedure uspfn_AbExtractData_II
	@FileID varchar(100),
	@FileContent varchar(max),
	@UserId varchar(25)
as
begin
	--select @FileContent

	update SimDmsIterator 
	   set AttendanceFlatFileExtractionProcessed=0
	     , AttendanceFlatFileExtractionTotal=100

	select * into #a 
	from uspfn_AbSplitString(@FileContent, char(10)+char(13))

	--begin try
		declare c cursor for 
			select * from #a
		declare @Record varchar(100);
		declare @EmployeeID varchar(17);
		declare @AttendanceTime varchar(20);
		declare @MachineCode varchar(15);
		declare @IdentityCode varchar(12);
		declare @CurrentDate datetime;
		declare @CompanyCode varchar(17);
		declare @TrailingZeroIndex tinyint;
		set @CompanyCode = (
			select top 1
			       CompanyCode
			  from gnMstOrganizationHdr 
		);

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
				if @iterator>2
				begin
					set @CompanyCode = (select top 1 CompanyCode from gnMstOrganizationHdr );
					set @EmployeeID = (select * from dbo.SplitString(@Record, ',', 1));
					--set @AttendanceTime = convert(datetime,  (select * from dbo.SplitString(@Record, ',', 3)) + ' ' + (select * from dbo.SplitString(@Record, ',', 4)));
					set @AttendanceTime = (select * from dbo.SplitString(@Record, ',', 3)) + ' ' + (select * from dbo.SplitString(@Record, ',', 4));
					set @MachineCode = (select * from dbo.SplitString(@Record, ',', 5));
					set @IdentityCode = ( 
						case (select * from dbo.SplitString(@Record, ',', 8) )
							when 'scan masuk' then 'I' 
							else 'O' 
						end);


					--select @AttendanceTime as AttendanceTime;
					--select * from dbo.SplitString(@Record, ',', 3);
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
				end

				set @iterator = @iterator + 1;
				fetch next from c into @Record
			end
		close c
		deallocate c

		select Convert(bit, 1);
	--end try
	--begin catch
	--	select Convert(bit, 0);
	--end catch
end


go
if object_id('SplitString') is not null
	drop FUNCTION SplitString

go
create FUNCTION SplitString
(  
 @RowData nvarchar(MAX),
 @SplitOn nvarchar(5),
 @Index int
)    
RETURNS @ReturnValue TABLE   
(Data NVARCHAR(MAX))   
AS
BEGIN
 Declare @Counter int
 Set @Counter = 1 
 While (Charindex(@SplitOn,@RowData)>0) 
 Begin  
  --Insert Into @ReturnValue (data)  
  --Select Data = 
  --    ltrim(rtrim(Substring(@RowData,1,Charindex(@SplitOn,@RowData)-1)))
  Set @RowData = 
      Substring(@RowData,Charindex(@SplitOn,@RowData)+1,len(@RowData)) 

  if @Counter=@Index
  begin
	  Insert Into @ReturnValue (data)  
	  Select Data = 
		  ltrim(rtrim(Substring(@RowData,1,Charindex(@SplitOn,@RowData)-1)))
  end

  Set @Counter = @Counter + 1  
 End 


 --Insert Into @ReturnValue (data)  
 --Select Data = ltrim(rtrim(@RowData))  
 Return  
END

