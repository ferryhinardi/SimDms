alter procedure uspfn_WhInqPersInfo
--declare
	@CompanyCode varchar(20),
	@Position varchar(20) = '',
	@Status varchar(20) = ''
as

select a.EmployeeID
     , SalesID = isnull((select top 1 SalesID from HrEmployeeSales where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID), '') 
     , a.EmployeeName as SalesName
     , a.EmployeeName as SalesFullName
     , '5' as AreaCode
     , '' as DealerGroup
     , a.CompanyCode as DealerCode
     , b.CompanyName as DealerName
     , OutletCode = isnull((select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by MutationDate desc), '')
     , 'D' as OutletType
     , OutletName = isnull((select top 1 xb.CompanyName from HrEmployeeMutation xa, gnMstCoProfile xb where xb.CompanyCode = xa.CompanyCode and xb.BranchCode = xa.BranchCode and xa.CompanyCode = a.CompanyCode and xa.EmployeeID = a.EmployeeID order by xa.MutationDate desc), '')
     , JobCode = isnull((select top 1 Position from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by AssignDate desc), '')
     , JobName = isnull((select top 1 xb.PosName
						   from HrEmployeeAchievement xa, gnMstPosition xb
						  where xb.CompanyCode = xa.CompanyCode
						    and xb.DeptCode = xa.Department
						    and xb.PosCode = xa.Position
						    and xa.CompanyCode = a.CompanyCode
						    and xa.EmployeeID = a.EmployeeID
						  order by xa.AssignDate desc), '')
     , GradeCode = isnull((select top 1 Grade from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by AssignDate desc), '')
     , GradeName = isnull((select top 1 xb.LookupValueName
						   from HrEmployeeAchievement xa, gnMstLookupDtl xb
						  where xb.CompanyCode = xa.CompanyCode
						    and xb.CodeID = 'ITSG'
						    and xb.LookUpValue = xa.Grade
						    and xa.CompanyCode = a.CompanyCode
						    and xa.EmployeeID = a.EmployeeID
						  order by xa.AssignDate desc), '')
     , a.JoinDate
     , IsAdditionalJob = 'Tidak'
     , AdditionalJob = ''
     , isnull(a.BirthPlace, '') as BirthPlace
     , a.BirthDate
     , Education = (
			select top 1
			       x.LookUpValueName
			  from gnMstLookupDtl x
			 where x.CodeID='FEDU' 
			   and x.LookUpValue=a.FormalEducation

	   )
     , isnull(a.Handphone1, '') as Phone1
     , isnull(a.Handphone2, '') as Phone2
     , isnull(c.LookupValueName, '') as Religion
     , isnull(convert(varchar(35), a.IdentityNo), '') as KtpNo
     , isnull(a.DrivingLicense1, '') as Sim1
     , isnull(a.DrivingLicense2, '') as Sim2
     --, case a.PersonnelStatus when '1' then 'Aktif' when '2' then 'Non Aktif' when '3' then 'Keluar' when '4' then 'Pension' else 'Invalid' end as PersonnelStatus
     , case a.PersonnelStatus when '1' then '-' when '2' then '-' when '3' then 'Y' when '4' then '-' else 'Invalid' end as TerminateStatus
     , isnull(a.ResignDescription, '') as TerminateReason
     , a.ResignDate as TerminateDate
     , case a.MaritalStatus when 'K' then 'Kawin' else 'Tidak Kawin' end as MaritalStatus
     , case a.PersonnelStatus when '1' then 'Aktif' else 'Non Aktif' end as EmployeeStatus
     , case a.Gender when 'L' then 'Laki-laki' when 'P' then 'Perempuan' else '' end as Gender
     , isnull(a.Email, '') as Email
     , isnull(a.TeamLeader, '') as TeamLeader
     , TeamLeaderSalesID = isnull((select top 1 SalesID from HrEmployeeSales where CompanyCode = a.CompanyCode and EmployeeID = a.TeamLeader), '') 
     , TeamLeaderName = isnull((select top 1 EmployeeName from HrEmployee where CompanyCode = a.CompanyCode and EmployeeID = a.TeamLeader), '') 
     , a.EmployeeID as Nik
     , '' as StatusSf
     , '' as StatusSm
  from HrEmployee a
  left join gnMstOrganizationHdr b
    on b.CompanyCode = a.CompanyCode
  left join gnMstLookupDtl c
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'RLGN'
   and c.LookupValue = a.Religion
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.Department = 'SALES'
   and a.Position = (case @Position when '' then a.Position else @Position end)
   and a.PersonnelStatus = (case @Status when '' then a.PersonnelStatus else @Status end)
   and (case a.PersonnelStatus when '1' then '-' when '2' then '-' when '3' then 'Y' when '4' then '-' else 'Invalid' end) != 'Invalid'
 order by b.CompanyCode, OutletCode, a.EmployeeName 

go
  
uspfn_WhInqPersInfo '6115204', '', ''
  --select * from gnMstCoProfile
  
  