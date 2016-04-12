USE [SAT_]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_SaveEmployeeMutation]    Script Date: 04/23/2015 09:30:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

ALTER procedure [dbo].[uspfn_SaveEmployeeMutation]
	@CompanyCode varchar(20),
	@EmployeeID varchar(20),
	@MutationDate datetime,
	@IsJoinDate bit,
	@BranchCode varchar(20),
	@UserID varchar(64)
as
begin
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
	
	declare @branch varchar(15);
	declare @UserEmployee varchar(15);

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

				if @IsJoinDate=1 
				begin
					update HrEmployeeMutation
					   set IsDeleted=1
					 where  CompanyCode=@CompanyCode and EmployeeID=@EmployeeID  and MutationDate < @MutationDate
				end
				
				set @Message = 'Data has been saved into database.';
				set @Status = convert(bit, 1);
			end
		end
		else
		begin
			insert into 
				   HrEmployeeMutation ( CompanyCode, EmployeeID, MutationDate, BranchCode, IsJoinDate, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsDeleted )
			values
				   (@CompanyCode, @EmployeeID, @MutationDate, @BranchCode, @IsJoinDate, @UserID, @CurrentTime, @UserID, @CurrentTime, 0)

			if @IsJoinDate=1
			begin
				update HrEmployeeMutation
				   set IsDeleted=1
				 where CompanyCode=@CompanyCode and EmployeeID=@EmployeeID  and MutationDate < @MutationDate
			end
				
			set @Status = convert(bit, 1);
			set @Message = 'Data has been saved.';
		end
		
		if exists (select * from HrEmployee where CompanyCode = @CompanyCode and EmployeeID = @EmployeeID and (RelatedUser is not null or RelatedUser = ''))
		begin
			set @UserEmployee = (select RelatedUser from HrEmployee where CompanyCode = @CompanyCode and EmployeeID = @EmployeeID)
			set @branch = (select top 1 BranchCode
						from HrEmployeeMutation
						where CompanyCode = @CompanyCode
						and EmployeeID = @EmployeeID
						and IsDeleted = 0
						order by MutationDate Desc)
			update SysUser
			set BranchCode = @branch
			where UserId = @UserEmployee
		end
	end

	select convert(bit, @Status) as Status, @Message as Message;
end
