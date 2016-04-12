
go
if object_id('uspfn_SaveEmployeeMutation') is not null
	drop procedure uspfn_SaveEmployeeMutation

go
create procedure uspfn_SaveEmployeeMutation
	@CompanyCode varchar(20),
	@EmployeeID varchar(20),
	@MutationDate datetime,
	@IsJoinDate bit,
	@BranchCode varchar(20),
	@UserID varchar(64)
as

begin
	-- Declaring variables that hold current execution time
	declare @CurrentTime datetime;
	declare @JoinDate datetime;
	declare @ResignDate datetime;
	declare @Status bit;
	declare @Message varchar(150);
	declare @PrevMutation varchar(17);
	declare @NextMutation varchar(17);
	declare @NumberOfExistingRecord int;
	declare @NextMutationDate datetime;
	declare @PrevMutationDate datetime;

	set @NextMutation = null;
	set @PrevMutationDate = null;
	set @NumberOfExistingRecord = 0;
	set @Status=0;
	set @Message='';
	set @PrevMutation = '';
	set @NextMutation = ''
	set @CurrentTime = getDate();
	set @JoinDate = ( select top 1 a.JoinDate from HrEmployee a where a.CompanyCode=@CompanyCode and a.EmployeeID=@EmployeeID);
	set @ResignDate = ( select top 1 a.ResignDate from HrEmployee a where a.CompanyCode=@CompanyCode and a.EmployeeID=@EmployeeID);

	if @MutationDate < @JoinDate
	begin
		set @Message = 'Mutation datetime cannot less than join datetime.';
	end
	else if @MutationDate > @ResignDate and @ResignDate is not null
	begin
		set @Message = 'Mutation datetime cannot more than resign datetime.';
	end
	else 
	begin
		set @NumberOfExistingRecord = ( select count(*) from HrEmployeeMutation where CompanyCode=@CompanyCode and EmployeeID=@EmployeeID and convert(datetime, MutationDate)=@MutationDate );

		if @NumberOfExistingRecord > 0
		begin
			set @PrevMutation = (
				select top 1
				       a.BranchCode
				  from HrEmployeeMutation a
				 where a.CompanyCode=@CompanyCode
				   and a.EmployeeID=@EmployeeID
				   and convert(datetime, a.MutationDate) < @MutationDate
				 order by a.MutationDate desc
			);		
			
			set @NextMutation = (
				select top 1
					   a.BranchCode
				  from HrEmployeeMutation a
				 where a.CompanyCode=@CompanyCode
				   and a.EmployeeID=@EmployeeID
				   and convert(datetime, a.MutationDate) > @MutationDate
				 order by a.MutationDate desc
			);

			if @BranchCode = @PrevMutation
			begin			
				set @Message='There is mutation in the selected Branch before this mutation datetime.';
			end
			else if @BranchCode = @NextMutation
			begin
				set @Message='There is mutation in the selected Branch after this mutation datetime.';
			end
			else
			begin
				update HrEmployeeMutation
				   set IsDeleted=0
				     , BranchCode=@BranchCode
				 where CompanyCode=@CompanyCode
				   and EmployeeID=@EmployeeID
				   and convert(datetime, MutationDate)=@MutationDate

				 set @Message = 'Data has been saved into database.';
				 set @Status = 1;
			end

		end
		else
		begin
			insert into 
				   HrEmployeeMutation ( CompanyCode, EmployeeID, MutationDate, BranchCode, IsJoinDate, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsDeleted )
			values
				   (@CompanyCode, @EmployeeID, @MutationDate, @BranchCode, @IsJoinDate, @UserID, @CurrentTime, @UserID, @CurrentTime, 0)

			set @Status = 1;
			set @Message = 'Data has been saved.';
		end
	end

	select @Status as Success, @Message as Message;
end





