
go
if object_id('HrEmployeeUpdTrig') is not null
	drop trigger HrEmployeeUpdTrig

go
create trigger HrEmployeeUpdTrig on HrEmployee

for update as
begin
	declare @rows int
	declare @trgdate datetime

	select @rows = @@rowcount, @trgdate = getdate()

	if @rows = 1
	begin
		update gnMstEmployee
		   set EmployeeName = (select top 1 EmployeeName from inserted)
			 , Address1 = (select top 1 Address1 from inserted)
			 , Address2 = (select top 1 Address2 from inserted)
			 , Address3 = (select top 1 Address3 from inserted)
			 , Address4 = (select top 1 Address4 from inserted)
			 , PhoneNo = (select top 1 Telephone1 from inserted)
			 , HpNo = (select top 1 Telephone2 from inserted)
			 , FaxNo = (select top 1 FaxNo from inserted)
			 , ZipNo = (select top 1 ZipCode from inserted)
			 , JoinDate = (select top 1 JoinDate from inserted)
			 , GenderCode  = (select top 1 Gender from inserted)
			 , BirthPlace = (select top 1 BirthPlace from inserted)
			 , BirthDate = (select top 1 BirthDate from inserted)
			 , MaritalStatusCode = (select top 1 MaritalStatus from inserted)
			 , ReligionCode = (select top 1 Religion from inserted)
			 , BloodCode = (select top 1 BloodCode from inserted)
			 , IdentityNo = (select top 1 IdentityNo from inserted)
			 , Height = (select top 1 Height from inserted)
			 , Weight = (select top 1 Weight from inserted)
			 , UniformSize = (select top 1 UniformSize from inserted)
			 , ShoesSize = (select top 1 ShoesSize from inserted)
			 , FormalEducation = (select top 1 FormalEducation from inserted)
			 , PersonnelStatus = (select top 1 PersonnelStatus from inserted)
		 where CompanyCode = (select top 1 CompanyCode from inserted)
		   and EmployeeID = (select top 1 EmployeeID from inserted)	
		   and CreatedBy = 'init-trg' 
		   and LastupdateBy = 'init-trg' 

    insert into gnMstEmployee (CompanyCode, BranchCode, EmployeeID, EmployeeName, Address1, Address2, Address3, Address4, PhoneNo, HpNo, FaxNo, ProvinceCode, AreaCode, CityCode, ZipNo, TitleCode, JoinDate, ResignDate, GenderCode, BirthPlace, BirthDate, MaritalStatusCode, ReligionCode, BloodCode, IdentityNo, Height, Weight, UniformSize, ShoesSize, FormalEducation, PersonnelStatus, IsLocked, LockingBy, LockingDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate)
	select a.CompanyCode
		 , b.BranchCode
		 , a.EmployeeID
		 , a.EmployeeName
		 , a.Address1
		 , a.Address2
		 , a.Address3
		 , a.Address4
		 , a.Telephone1 as PhoneNo
		 , a.Telephone2 as HPNo
		 , a.FaxNo
		 , '' ProvinceCode
		 , '' AreaCode
		 , '' CityCode
		 , a.ZipCode
		 , '' TitleCode
		 , a.JoinDate
		 , null as ResignDate
		 , Gender as GenderCode
		 , a.BirthPlace
		 , a.BirthDate
		 , a.MaritalStatus as MaritalStatusCode
		 , a.Religion as ReligionCode
		 , a.BloodCode
		 , a.IdentityNo
		 , a.Height
		 , a.Weight
		 , a.UniformSize
		 , a.ShoesSize
		 , a.FormalEducation
		 , a.PersonnelStatus
		 , 0 as IsLocked
		 , '' as LockingBy
		 , null as LockingDate
		 , 'init-trg' as CreatedBy
		 , @trgdate as CreatedDate
		 , 'init-trg' as LastUpdateBy
		 , @trgdate as LastUpdateDate
	  from inserted a
	  join gnMstCoProfile b
		on b.CompanyCode = a.CompanyCode
	 where not exists (
			select 1 from gnMstEmployee
			 where CompanyCode = a.CompanyCode
			   and EmployeeID = a.EmployeeID
		)
	end
end

