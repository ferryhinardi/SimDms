go
if object_id('uspfn_HrInsertNewEmployeeMutation') is not null
	drop procedure uspfn_HrInsertNewEmployeeMutation

go
create procedure uspfn_HrInsertNewEmployeeMutation(
	@CompanyCode varchar(15),
	@EmployeeID varchar(15),
	@MutationDate datetime,
	@BranchCode varchar(15),
	@IsJoinDate bit,
	@CreatedBy varchar(15),
	@CreatedDate datetime,
	@UpdatedBy varchar(15),
	@UpdatedDate datetime
)
as
begin
	declare @TransactionName varchar(50);
	set @TransactionName = 'InsertEmployeeMutation';

	begin try
		begin transaction @TransactionName
			insert into 
				HrEmployeeMutation
				(CompanyCode, EmployeeID, MutationDate, BranchCode, IsJoinDate, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate)
			values
				(@CompanyCode, @EmployeeID, @MutationDate, @BranchCode, @IsJoinDate, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate);

			insert into 
				HrEmployeeAdditionalBranch (
					CompanyCode, EmployeeID, AssignDate , SeqNo, BranchCode, ExpiredDate, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
				)
			values
				(@CompanyCode, @EmployeeID, @MutationDate, 0, @BranchCode, null, 'System', getDate(), 'System', getDate());
		commit transaction @TransactionName;
	end try
	begin catch
		rollback transaction @TransactionName;
	end catch
end

