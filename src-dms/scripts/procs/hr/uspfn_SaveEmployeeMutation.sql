
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
			       and ( a.IsDeleted is null or a.IsDeleted = 0 )
				 order by a.MutationDate desc
			);		
			
			set @NextMutation = (
				select top 1
					   a.BranchCode
				  from HrEmployeeMutation a
				 where a.CompanyCode=@CompanyCode
				   and a.EmployeeID=@EmployeeID
				   and convert(datetime, a.MutationDate) > @MutationDate
				   and ( a.IsDeleted is null or a.IsDeleted = 0 )
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
					 where MutationDate < @MutationDate
					   and EmployeeID = @EmployeeID;
				end

				;with x as (
					select a.CompanyCode
					     , @BranchCode as BranchCode
					     , a.EmployeeID
					     , a.EmployeeName
					     , a.Address1
					     , a.Address2
					     , a.Address3
					     , a.Address4
					     , a.Telephone1
					     , a.Handphone1
					     , a.FaxNo
					     , a.JoinDate
					     , a.ResignDate
					     , a.Gender
					     , a.BirthPlace
					     , a.BirthDate
					     , a.MaritalStatus
					     , a.Religion
					     , a.BloodCode
					     , a.IdentityNo
					     , a.Height
					     , a.Weight
					     , a.UniformSize
					     , a.ShoesSize
					     , a.FormalEducation
					     , a.PersonnelStatus
						 , TitleCode = (
							case 
								when a.Position='S' then '12'
								when a.Position='SC' then '13'
								when a.Position='SH' then '14'
								when a.Position='BM' then '1'
							end)
						 , a.ZipCode
						 , a.Province
						 , a.District
						 , a.SubDistrict
					  from HrEmployee a
				     where a.CompanyCode = @CompanyCode
					   and a.EmployeeID = @EmployeeID
				)
				insert into gnMstEmployee (
						    CompanyCode
                          , BranchCode
						  , EmployeeID
						  , EmployeeName
						  , Address1
						  , Address2
						  , Address3
						  , Address4
						  , PhoneNo
						  , HpNo
						  , FaxNo
						  , JoinDate
						  , ResignDate
						  , GenderCode
						  , BirthPlace
						  , BirthDate
						  , MaritalStatusCode
						  , ReligionCode
						  , BloodCode
						  , IdentityNo
						  , Height
						  , Weight
						  , UniformSize
						  , ShoesSize
						  , FormalEducation
						  , PersonnelStatus
				          , TitleCode
						  , ZipNo
						  , ProvinceCode
						  , CityCode
						  , AreaCode
					      , CreatedBy
					      , CreatedDate
					      , LastUpdateBy
					      , LastUpdateDate
                          )
				select *
			         , @UserId
				     , getdate()
				     , @UserId
				     , getdate()
                  from x;

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
				 where MutationDate < @MutationDate
                   and EmployeeID = @EmployeeID;
			end

			begin try
				;with x as (
					select a.CompanyCode
					     , @BranchCode as BranchCode
					     , a.EmployeeID
					     , a.EmployeeName
					     , a.Address1
					     , a.Address2
					     , a.Address3
					     , a.Address4
					     , a.Telephone1
					     , a.Handphone1
					     , a.FaxNo
					     , a.JoinDate
					     , a.ResignDate
					     , a.Gender
					     , a.BirthPlace
					     , a.BirthDate
					     , a.MaritalStatus
					     , a.Religion
					     , a.BloodCode
					     , a.IdentityNo
					     , a.Height
					     , a.Weight
					     , a.UniformSize
					     , a.ShoesSize
					     , a.FormalEducation
					     , a.PersonnelStatus
					  from HrEmployee a
				     where a.CompanyCode = @CompanyCode
					   and a.EmployeeID = @EmployeeID
				)
				insert into gnMstEmployee (
						    CompanyCode
                          , BranchCode
						  , EmployeeID
						  , EmployeeName
						  , Address1
						  , Address2
						  , Address3
						  , Address4
						  , PhoneNo
						  , HpNo
						  , FaxNo
						  , JoinDate
						  , ResignDate
						  , GenderCode
						  , BirthPlace
						  , BirthDate
						  , MaritalStatusCode
						  , ReligionCode
						  , BloodCode
						  , IdentityNo
						  , Height
						  , Weight
						  , UniformSize
						  , ShoesSize
						  , FormalEducation
						  , PersonnelStatus
					      , CreatedBy
					      , CreatedDate
					      , LastUpdateBy
					      , LastUpdateDate
                          )
				select *
			         , @UserId
				     , getdate()
				     , @UserId
				     , getdate()
                  from x;
			end try
			begin catch
				
			end catch

			set @Status = convert(bit, 1);
			set @Message = 'Data has been saved.';
		end
	end

	select convert(bit, @Status) as Status, @Message as Message;
end







go
if object_id('uspfn_HrUpdateJoinDateMutation') is not null
	drop procedure uspfn_HrUpdateJoinDateMutation

go
create procedure uspfn_HrUpdateJoinDateMutation
	@CompanyCode varchar(25),
	@EmployeeID varchar(25),
	@JoinDate datetime

as
begin
	begin try
		update HrEmployeeMutation
		   set MutationDate=@JoinDate
		 where CompanyCode=@CompanyCode
		   and EmployeeID=@EmployeeID
		   and IsJoinDate=1
	end try
	begin catch
		update HrEmployeeMutation
		   set IsDeleted=0
		     , IsJoinDate=1
		 where CompanyCode=@CompanyCode
		   and EmployeeID=@EmployeeID
		   and convert(datetime, MutationDate)=@JoinDate
	end catch

	update HrEmployeeMutation
	   set IsDeleted=1
	 where CompanyCode=@CompanyCode
	   and EmployeeID=@EmployeeID
	   and convert(datetime,MutationDate) < @JoinDate
end

