
go
if object_id('uspfn_AbInsertAttendanceData') is not null
	drop procedure uspfn_AbInsertAttendanceData

GO
create procedure [dbo].[uspfn_AbInsertAttendanceData]
	@CompanyCode varchar(25),
	@FileID varchar(80),
	@Iterator int,
	@EmployeeID varchar(17),
	@AttendanceTime varchar(19),
	@MachineCode varchar(2),
	@IdentityCode varchar(1),
	@UserID varchar(17)
as
begin
	declare @RowCount int;
	set @RowCount = (
		select COUNT(a.FileID)
		  from HrTrnAttendanceFileDtl a
		 where a.CompanyCode=@CompanyCode
		   and a.EmployeeID=@EmployeeID
		   and a.FileID=@FileID
		   and a.AttendanceTime=@AttendanceTime
	)

	if @RowCount = 0
	if not exists (
		select a.FileID
		  from HrTrnAttendanceFileDtl a
		 where a.CompanyCode=@CompanyCode
		   and a.EmployeeID=@EmployeeID
		   and a.FileID=@FileID
		   and a.AttendanceTime=@AttendanceTime
	)
	begin
		insert into HrTrnAttendanceFileDtl (
			  CompanyCode
			 , FileID
			 , GenerateId
			 , SequenceNo
			 , EmployeeID
			 , EmployeeName
			 , AttendanceTime
			 , MachineCode
			 , IdentityCode
			 , IsTransfered
			 , CreatedBy
			 , CreatedDate
			 , UpdatedBy
			 , UpdatedDate
			 , IsDeleted 
	    )
		select @CompanyCode
			 , @FileID
			 , NEWID()
			 , @Iterator
			 , @EmployeeID
			 , EmployeeName = (
					select top 1
						   EmployeeName
					  from HrEmployee x
					 where x.CompanyCode = @CompanyCode
					   and x.EmployeeID=@EmployeeID
			   )
			 , @AttendanceTime
			 , @MachineCode
			 , @IdentityCode
			 , 0
			 , @UserID
			 , getdate()
			 , @UserID
			 , getdate()		 
			 , 0
	end
end





go
ALTER function [dbo].[uspfn_CalculateMinute] (
	@Time1 char(5),
	@Time2 char(5)
)
returns int
as
begin
	declare @Difference int;
	set @Difference = DATEDIFF(minute, @Time1, @Time2);
	return abs(@Difference);
end

