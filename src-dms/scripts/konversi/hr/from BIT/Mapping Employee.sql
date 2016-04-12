
go
if object_id('insertEmployee') is not null
	drop procedure insertEmployee

go
create procedure insertEmployee (
	@CompanyCode varchar(25),
	@EmployeeID varchar(25)
)
as
begin
	declare @rowCount int;

	set @rowCount = (
		select	
			count(a.EmployeeID)
		from
			HrEmployee a
		where
			a.CompanyCode=@CompanyCode
			and
			a.EmployeeID=@EmployeeID
	);

	if @rowCount = 0
	begin
		insert into HrEmployee
		select top 1
			x.CompanyCode,
			x.EmployeeID,
			x.EmployeeName,
			Email = '',
			x.FaxNo,
			Handphone1 = x.HpNo,
			Handphone2 = '',
			Handphone3 = '',
			Handphone4 = '',
			Telephone1 = '',
			Telephone2 = '',
			OfficeLocation = '',
			0,
			RelatedUser = '',
			x.JoinDate,
			Department = '',
			Position = '',
			Grade = '',
			Rank = '',
			Gender = '',
			TeamLeader = '',
			x.PersonnelStatus,
			x.ResignDate,
			ResignDescription = '',
			x.IdentityNo,
			NPWPNo = '',
			NPWPDate = null,
			x.BirthDate,
			x.BirthPlace,
			x.Address1,
			Address2 = '',
			Address3 = '',
			Address4 = '',
			Province =  x.ProvinceCode,
			District = x.CityCode,
			SubDistrict = x.AreaCode,
			Village = '',
			ZipCode = x.ZipNo,
			DrivingLicense1 = (
				select top 1
					a.DrivingLicense1
				from
					gnMstEmployeeData a
				where
					a.EmployeeID = x.EmployeeID
			),
			DrivingLicense2 =  (
				select top 1
					a.DrivingLicense2
				from
					gnMstEmployeeData a
				where
					a.EmployeeID = x.EmployeeID
			),
			MaritalStatus = x.MaritalStatusCode,
			MaritalStatusCode = '',
			x.Height,
			x.Weight,
			UniformSize = 0,
			UniformSizeAlt = '',
			ShoesSize = 0,
			x.FormalEducation,
			BloodCode = '',
			Otherinformation = '',
			x.CreatedBy,
			getDate(),
			x.CreatedBy,
			getDate(),
			Religion= '',
			SelfPhoto = '',
			IdentityCardPhoto = '',
			ResignCategory = ''
		from
			gnMstEmployee x
		where
			x.CompanyCode=@CompanyCode
			and
			x.EmployeeID=@EmployeeID
	end
end;

go
declare c cursor for (
	select 
		a.CompanyCode,
		a.EmployeeID
	from
		gnMstEmployee a
)

declare @CompanyCode varchar(25);
declare @EmployeeID varchar(25);

open c
	fetch next from c into @CompanyCode, @EmployeeID

	while @@FETCH_STATUS=0
	begin
		exec insertEmployee @CompanyCode=@CompanyCode, @EmployeeID=@EmployeeID;

		fetch next from c into @CompanyCode, @EmployeeID
	end
close c
deallocate c

go
