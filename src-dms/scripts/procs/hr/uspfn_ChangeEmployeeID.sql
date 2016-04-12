
go
if object_id('uspfn_ChangeEmployeeID') is not null
	drop procedure uspfn_ChangeEmployeeID

go
create procedure uspfn_ChangeEmployeeID
	@CompanyCode varchar(17),
	@UserID varchar(25),
	@CurrentEmployeeID varchar(25),
	@NewEmployeeID varchar(25)
as
begin
	if @CurrentEmployeeID=@NewEmployeeID
		select convert(bit, 0) as status, 'Current EmployeeID and New EmployeeID must be different.' as message;
	else 
	begin
		begin try
			begin transaction
			
			declare @NumberOfRecord tinyint;
			set @NumberOfRecord  = ( select count(EmployeeID) from HrEmployee where CompanyCode=@CompanyCode and EmployeeID=@NewEmployeeID );

			if @NumberOfRecord > 0
			begin
				select convert(bit, 0) as status, 'There is already Employee that has same ID with your new EmployeeID.' as message;
			end
			else
			begin
				update HrEmployee 
				   set PersonnelStatus='2'
				 where CompanyCode=@CompanyCode
				   and EmployeeID=@CurrentEmployeeID;

				insert into HrEmployee (CompanyCode, EmployeeID, EmployeeName, Email, FaxNo, Handphone1, Handphone2, Handphone3, Handphone4, Telephone1, Telephone2, OfficeLocation, IsLinkedUser, RelatedUser, JoinDate, Department, Position, Grade, Rank, Gender, TeamLeader, PersonnelStatus, ResignDate, ResignDescription, IdentityNo, NPWPNo, NPWPDate, BirthDate, BirthPlace, Address1, Address2, Address3, Address4, Province, District, SubDistrict, Village, ZipCode, DrivingLicense1, DrivingLicense2, MaritalStatus, MaritalStatusCode, Height, Weight, UniformSize, UniformSizeAlt, ShoesSize, FormalEducation, BloodCode, OtherInformation, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, Religion, SelfPhoto, IdentityCardPhoto, IsDeleted)
				select a.CompanyCode
					 , @NewEmployeeID  
					 , a.EmployeeName
					 , a.Email
					 , a.FaxNo
					 , a.Handphone1
					 , a.Handphone2
					 , a.Handphone3
					 , a.Handphone4
					 , a.Telephone1
					 , a.Telephone2
					 , a.OfficeLocation
					 , a.IsLinkedUser
					 , a.RelatedUser
					 , a.JoinDate
					 , a.Department
					 , a.Position
					 , a.Grade
					 , a.Rank
					 , a.Gender
					 , a.TeamLeader
					 , '1' as PersonnelStatus
					 , a.ResignDate
					 , a.ResignDescription
					 , a.IdentityNo
					 , a.NPWPNo
					 , a.NPWPDate
					 , a.BirthDate
					 , a.BirthPlace
					 , a.Address1
					 , a.Address2
					 , a.Address3
					 , a.Address4
					 , a.Province
					 , a.District
					 , a.SubDistrict
					 , a.Village
					 , a.ZipCode
					 , a.DrivingLicense1
					 , a.DrivingLicense2
					 , a.MaritalStatus
					 , a.MaritalStatusCode
					 , a.Height
					 , a.Weight
					 , a.UniformSize
					 , a.UniformSizeAlt
					 , a.ShoesSize
					 , a.FormalEducation
					 , a.BloodCode
					 , a.OtherInformation
					 , @UserID
					 , getdate()
					 , @UserID
					 , getdate()
					 , a.Religion
					 , a.SelfPhoto
					 , a.IdentityCardPhoto
					 , a.IsDeleted
				  from HrEmployee a
				 where a.CompanyCode=@CompanyCode
				   and a.EmployeeID=@CurrentEmployeeID;

				insert into HrEmployeeIDChangedHistory (CompanyCode, OldEmployeeID, NewEmployeeID, CreatedBy, CreatedDate)
				select @CompanyCode
				     , @CurrentEmployeeID
					 , @NewEmployeeID
					 , @UserID
					 , getdate()				

				select convert(bit, 1) as status, 'EmployeeID has been updated.' as message;
			end
			commit transaction
		end try
		begin catch
			rollback transaction
			select convert(bit, 0) as status,  'Sorry, we cannot process your request. Please, try again later!' as message;
		end catch
	end
end


