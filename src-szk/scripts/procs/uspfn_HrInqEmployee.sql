alter procedure uspfn_HrInqEmployee
--declare
	@CompanyCode varchar(10),
	@DeptCode varchar(10),
	@PosCode varchar(10),
	@Status varchar(10)
as

select a.CompanyCode
     , e.DealerName
     , a.EmployeeID
	 , a.EmployeeName
	 , a.Department
	 , a.Position
	 , isnull(a.Grade, '') Grade
	 , a.PersonnelStatus
     , a.TeamLeader
     , a.JoinDate
     , Status = (case isnull(a.PersonnelStatus, '')
		when '1' then 'Aktif'
		when '2' then 'Non Aktif'
		when '3' then 'Keluar'
		when '4' then 'Pensiun'
		else 'No Status' end)
     , SubOrdinates = (select count(*) from HrEmployee where CompanyCode = a.CompanyCode and TeamLeader = a.EmployeeID)
     , MutationTimes = (select count(*) from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID)
     , AchieveTimes =  (select count(*) from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID)
     , LastBranch = isnull((select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by MutationDate desc), '')
     , LastBranchName = (  
			select top 1 
			       x.BranchName
			  from OutletInfo x
			 where x.CompanyCode=a.CompanyCode
			   and x.BranchCode = (select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by MutationDate desc)
       )
	 , LastPosition = upper(a.Department)
         + upper(case isnull(b.PosName, '') when '' then '' else ' - ' + b.PosName end)
         + upper(case isnull(c.LookUpValueName, '') when '' then '' else ' - ' + c.LookUpValueName end)
     , IsValid = isnull((select top 1 'Y' from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and IsJoinDate = 1 order by MutationDate desc), 'N')
     , IsValidAchieve = isnull((select top 1 'Y' from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and IsJoinDate = 1 order by AssignDate desc), 'N')
     , a.TeamLeader
	 , TeamLeaderName = (
		select top 1 
		       x.EmployeeName
		  from HrEmployee x
		 where x.EmployeeID=a.TeamLeader
		   and x.CompanyCode=a.CompanyCode
	 )
	 , a.ResignDate
	 , a.ResignDescription
	 , MaritalStatus = (
			select top 1
			       x.LookUpValueName
			  from gnMstLookUpDtl x
			 where x.CodeID='MRTL'
			   and x.LookUpValue=a.MaritalStatus
	   )
	 , Religion = (
			select top 1
			       x.LookUpValueName
			  from gnMstLookUpDtl x
			 where x.CodeID='RLGN'
			   and x.LookUpValue=a.Religion
	   )
	 , Gender = (
			select top 1
			       x.LookUpValueName
			  from gnMstLookUpDtl x
			 where x.CodeID='GNDR'
			   and x.LookUpValue=a.Gender
	   )
	 , Education = (
			select top 1
			       x.LookUpValueName
			  from gnMstLookUpDtl x
			 where x.CodeID='FEDU'
			   and x.LookUpValue=a.FormalEducation
	   ) 
     , a.BirthPlace
	 , a.BirthDate
	 , (a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 + ' ' + a.Address4) as Address
	 , a.Province
	 , a.District
	 , a.SubDistrict
	 , a.Village
	 , a.ZipCode
	 , a.IdentityNo
	 , a.NPWPNo
	 , a.NPWPDate
	 , a.Email
	 , a.Telephone1
	 , a.Telephone1
	 , a.Handphone1
	 , a.Handphone2
	 , a.Handphone3
	 , a.Handphone4
	 , a.DrivingLicense1
	 , a.DrivingLicense2
	 , a.Height
	 , a.Weight
	 , a.BloodCode
	 , a.UniformSize
	 , UniformSizeAlt = (
			select top 1
				   x.LookUpValueName
			  from gnMstLookUpDtl x
			 where x.CodeID='SIZEALT'
			   and x.LookUpValue=a.UniformSizeAlt
	   )
	 , a.ShoesSize
  from HrEmployee a
  left join DealerInfo e
    on e.DealerCode = a.CompanyCode
  left join GnMstPosition b
    on b.CompanyCode = a.CompanyCode
   and b.DeptCode = a.Department
   and b.PosCode = a.Position
  left join MstLookUpDtl c
    --on c.CompanyCode = a.CompanyCode
    on c.CodeID = 'ITSG'
   and c.LookUpValue = a.Grade
  left join HrEmployee d
    on d.CompanyCode = a.CompanyCode
   and d.EmployeeID = a.TeamLeader
 where a.CompanyCode = @CompanyCode
   and a.Department = @DeptCode
   and a.Position = (case @PosCode when '' then a.Position else @PosCode end)
   and a.PersonnelStatus = (case @Status when '' then a.PersonnelStatus else @Status end)
   --and a.PersonnelStatus != '2'
 order by e.DealerName, LastBranch, a.EmployeeName


