
go
if object_id('HrEmployeeView') is not null
	drop view HrEmployeeView

go
create view HrEmployeeView
as
SELECT 
	a.CompanyCode,
	a.EmployeeID,
	b.SalesID,
	c.ServiceID,
	a.EmployeeName,
	a.Email,
	a.FaxNo,
	a.Handphone1,
	a.Handphone2,
	a.Handphone3,
	a.Handphone4,
	a.Telephone1,
	a.Telephone2,
	a.OfficeLocation,
	a.IsLinkedUser,
	a.RelatedUser,
	FullName = (
		select top 1
			x.FullName
		from 
			SysUser x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.UserId = a.RelatedUser
	),
	a.JoinDate,
	a.Department,
	a.Position,
	a.Grade,
	a.Rank,
	Departmentstext = (
		select top 1
			x.OrgName
		from 
			gnMstOrgGroup x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.OrgGroupCode = 'DEPT'
			and
			x.OrgCode = a.Department
	),
	Positionstext = (
		select top 1
			x.PosName
		from	
			gnMstPosition x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.DeptCode = a.Department
			and
			x.PosCode = a.Position
	),
	a.Gender,
	a.TeamLeader,
	TeamLeaderName = (
		select top 1
			x.EmployeeName
		from	
			HrEmployee x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.EmployeeID = a.TeamLeader
	),
	a.PersonnelStatus,
	a.ResignDate, 
	a.ResignDescription,
	a.ResignCategory,
	a.IdentityNo,
	a.NPWPNo,
	a.NPWPDate,
	a.BirthDate,
	a.BirthPlace,
	a.Address1 as Address,
	a.Address1,
	a.Address2,
	a.Address3,
	a.Address4,
	a.Province,
	a.District,
	a.SubDistrict,
	a.Village,
	a.ZipCode,
	a.DrivingLicense1,
	a.DrivingLicense2,
	a.MaritalStatus,
	a.MaritalStatusCode,
	a.Height,
	a.Weight,
	a.UniformSize,
	a.UniformSizeAlt,
	a.ShoesSize,
	a.FormalEducation,
	a.BloodCode,
	a.OtherInformation,
	a.Religion,
	AdditionalJob1 = (
		select top 1
			x.Position
		from
			HrEmployeeAdditionalJob x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.EmployeeID = a.EmployeeID
			and
			x.SeqNo = 1
	),
	AdditionalJob2 = (
		select top 1
			x.Position
		from
			HrEmployeeAdditionalJob x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.EmployeeID = a.EmployeeID
			and
			x.SeqNo = 2
	),
	Status = (CASE
		WHEN a.PersonnelStatus = '1' THEN
		'Aktif'
		WHEN a.PersonnelStatus = '2' THEN
		'Non Aktif'
		WHEN a.PersonnelStatus = '3' THEN
		'Keluar'
		WHEN a.PersonnelStatus = '4' THEN
		'Pensiun'
	END),
	DepartmentName = (
		select top 1
			x.OrgName
		from 
			gnMstOrgGroup x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.OrgGroupCode = 'DEPT'
			and
			x.OrgCode = a.Department
	),
	PositionName = (
		select top 1
			x.PosName
		from	
			gnMstPosition x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.DeptCode = a.Department
			and
			x.PosCode = a.Position
	),
	GradeName = (
		select top 1
			x.LookUpValueName
		from
			gnMstLookUpDtl x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CodeID = 'ITSG'
			and
			x.LookUpValue = a.Grade
	),
	AdditionalJob1Name = (
		select top 1
			x.PosName
		from	
			gnMstPosition x
		inner join	
			HrEmployeeAdditionalJob y
		on
			x.CompanyCode = y.CompanyCode
			and
			x.DeptCode = y.Department
			and
			x.PosCode = y.Position
		where
			x.CompanyCode = a.CompanyCode
			and
			x.DeptCode = a.Department
			and
			x.PosCode = a.Position
			and
			y.EmployeeID = a.EmployeeID
			and
			y.SeqNo = 1
	),
	AdditionalJob2Name = (
		select top 1
			x.PosName
		from	
			gnMstPosition x
		inner join	
			HrEmployeeAdditionalJob y
		on
			x.CompanyCode = y.CompanyCode
			and
			x.DeptCode = y.Department
			and
			x.PosCode = y.Position
		where
			x.CompanyCode = a.CompanyCode
			and
			x.DeptCode = a.Department
			and
			x.PosCode = a.Position
			and
			y.EmployeeID = a.EmployeeID
			and
			y.SeqNo = 2
	),
	RankName = (
		select top 1
			x.LookUpValueName
		from
			gnMstLookUpDtl x
		where
			x.CompanyCode = a.CompanyCode
			and
			x.CodeID = 'RANK'
			and
			x.LookUpValue = a.Rank
	),
	a.SelfPhoto,
	a.IdentityCardPhoto
FROM
  HrEmployee a
left join
	HrEmployeeSales b
on
	a.CompanyCode = b.CompanyCode
	and
	a.EmployeeID = b.EmployeeID
left join 
	HrEmployeeService c
on
	a.CompanyCode = c.CompanyCode
	and
	a.EmployeeID = c.EmployeeID

GO


