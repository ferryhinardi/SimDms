if object_id('uspfn_HrListEmployeeSend') is not null
	drop procedure uspfn_HrListEmployeeSend

go
create procedure [dbo].[uspfn_HrListEmployeeSend] 
	@LastUpdateDate datetime,
	@Segment int

--select @LastUpdateDate='1990-01-01 00:00:00',@Segment=500
as

select * into #t1 from (
select top (@Segment) CompanyCode
     , EmployeeID, EmployeeName
	 , Email, FaxNo
	 , Handphone1, Handphone2, Handphone3, Handphone4
	 , Telephone1, Telephone2
	 , OfficeLocation, IsLinkedUser
	 , RelatedUser, JoinDate
	 , Department, Position, Grade, Rank
	 , Gender, TeamLeader
	 , PersonnelStatus, ResignDate, ResignDescription
	 , IdentityNo, NPWPNo, NPWPDate, BirthDate, BirthPlace
	 , Address1, Address2, Address3, Address4
	 , Province, District, SubDistrict, Village
	 , ZipCode, DrivingLicense1, DrivingLicense2
	 , MaritalStatus, MaritalStatusCode
	 , Height, Weight, UniformSize, UniformSizeAlt
	 , ShoesSize, FormalEducation, BloodCode
	 , OtherInformation, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
	 , Religion, SelfPhoto, IdentityCardPhoto
  from HrEmployee
 where UpdatedDate is not null
   and UpdatedDate > @LastUpdateDate
 order by UpdatedDate asc )#t1
 
declare @LastUpdateQry datetime
    set @LastUpdateQry = (select top 1 UpdatedDate from #t1 order by UpdatedDate desc)

select * from #t1
 union
select top 1000 CompanyCode
     , EmployeeID, EmployeeName
	 , Email, FaxNo
	 , Handphone1, Handphone2, Handphone3, Handphone4
	 , Telephone1, Telephone2
	 , OfficeLocation, IsLinkedUser
	 , RelatedUser, JoinDate
	 , Department, Position, Grade, Rank
	 , Gender, TeamLeader
	 , PersonnelStatus, ResignDate, ResignDescription
	 , IdentityNo, NPWPNo, NPWPDate, BirthDate, BirthPlace
	 , Address1, Address2, Address3, Address4
	 , Province, District, SubDistrict, Village
	 , ZipCode, DrivingLicense1, DrivingLicense2
	 , MaritalStatus, MaritalStatusCode
	 , Height, Weight, UniformSize, UniformSizeAlt
	 , ShoesSize, FormalEducation, BloodCode
	 , OtherInformation, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
	 , Religion, SelfPhoto, IdentityCardPhoto
  from HrEmployee
 where UpdatedDate = @LastUpdateQry
 
  drop table #t1

GO


