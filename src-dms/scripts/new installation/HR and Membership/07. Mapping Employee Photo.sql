update
	HrEmployee
set
	HrEmployee.SelfPhoto = (
		select top 1
			x.EmpImageID
		from 
			gnMstEmployee x
		where
			(
				x.EmpImageID is not null
				or
				x.EmpIdentityCardImageID is not null
			)
			and
			x.CompanyCode = HrEmployee.CompanyCode
			and
			x.EmployeeID = HrEmployee.EmployeeID
	),
	IdentityCardPhoto = (
		select top 1
			x.EmpIdentityCardImageID
		from 
			gnMstEmployee x
		where
			(
				x.EmpImageID is not null
				or
				x.EmpIdentityCardImageID is not null
			)
			and
			x.CompanyCode = HrEmployee.CompanyCode
			and
			x.EmployeeID = HrEmployee.EmployeeID
	)



--select 
--	HrEmployee.CompanyCode,
--	HrEmployee.EmployeeID,
--	HrEmployee.EmployeeName,
--	HrEmployee.SelfPhoto,
--	HrEmployee.IdentityCardPhoto,
--	OldPhoto = (
--		select top 1
--			x.EmpImageID
--		from 
--			gnMstEmployee x
--		where
--			(
--				x.EmpImageID is not null
--				or
--				x.EmpIdentityCardImageID is not null
--			)
--			and
--			x.CompanyCode = HrEmployee.CompanyCode
--			and
--			x.EmployeeID = HrEmployee.EmployeeID
--	),
--	OldIdentityPhoto = (
--		select top 1
--			x.EmpIdentityCardImageID
--		from 
--			gnMstEmployee x
--		where
--			(
--				x.EmpImageID is not null
--				or
--				x.EmpIdentityCardImageID is not null
--			)
--			and
--			x.CompanyCode = HrEmployee.CompanyCode
--			and
--			x.EmployeeID = HrEmployee.EmployeeID
--	)
--from
--	HrEmployee







