
if object_id('HrEmployeeInsTrig') is not null
	drop trigger HrEmployeeInsTrig

go
create trigger HrEmployeeInsTrig on HrEmployee

for insert as
begin
	declare @rows int
	declare @trgdate datetime

	select @rows = @@rowcount, @trgdate = getdate()

	if @rows = 0 return

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

