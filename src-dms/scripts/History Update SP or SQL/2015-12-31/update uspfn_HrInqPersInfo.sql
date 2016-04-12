ALTER procedure [dbo].[uspfn_HrInqPersInfo]
  @CompanyCode varchar(20),
  @DeptCode varchar(20),
  @PosCode varchar(20),
  @Status varchar(20),
  @BranchCode varchar(20) = ''
as      

;with x as (      
select a.CompanyCode
     , a.EmployeeID      
     , e.SalesID      
     , a.EmployeeName      
     , BranchCode = isnull((select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and IsDeleted=0 order by MutationDate  desc), '')      
     , DeptCode = isnull((select top 1 HrEmployeeAchievement.Department from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and isnull(isDeleted,0) != 1 and AssignDate <= getdate() order by AssignDate desc), '')      
     , PosCode = isnull((select top 1 HrEmployeeAchievement.Position from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and isnull(isDeleted,0) != 1 and AssignDate <= getdate() order by AssignDate desc), '')      
     , Status = (case isnull(a.PersonnelStatus, '')      
          when '1' then 'Aktif'      
          when '2' then 'Non Aktif'      
          when '3' then 'Keluar'      
          when '4' then 'Pensiun'      
          else 'No Status' end)      
     , a.Address1      
     , a.Address2      
     , a.Address3      
     , a.Address4      
     , Address = isnull(a.Address1, '') --+ ' ' + ltrim(isnull(a.Address2, '')) + ' ' + ltrim(isnull(a.Address3, '')) + ' ' + ltrim(isnull(a.Address4, ''))      
     , a.Grade  
     , convert(varchar(10),a.BirthDate,120) BirthDate      
     , upper(a.BirthPlace) BirthPlace
     , a.BloodCode      
     , SubOrdinates = (select count(x.EmployeeID) from HrEmployee x where x.TeamLeader=a.EmployeeID)      
     , MutationTimes = (select count(x.EmployeeID) from HrEmployeeMutation x where x.CompanyCode=a.CompanyCode and x.EmployeeID=a.EmployeeID and x.IsDeleted != 1)      
     , AchieveTimes = (select count(x.EmployeeID) from HrEmployeeAchievement x where x.CompanyCode=a.CompanyCode and x.EmployeeID=a.EmployeeID and x.IsDeleted != 1)      
     , TeamLeader = (select top 1 x.EmployeeName from HrEmployee x where x.EmployeeID=a.TeamLeader)      
     , convert(varchar(10),a.JoinDate,120) JoinDate      
     , a.ResignDate      
     , a.ResignDescription      
     , MaritalStatus = upper(f.LookUpValueName)
     , Religion = upper(g.LookUpValueName)
     , Education = upper(h.LookUpValueName)
     , Gender = upper(i.LookUpValueName)
     , a.Province      
     , a.District      
     , a.SubDistrict      
     , a.Village      
     , a.ZipCode      
     , a.IdentityNo      
     , a.NPWPNo      
     , a.NPWPDate      
     , a.Email      
     , a.FaxNo      
     , a.Telephone1      
     , a.Telephone2      
     , a.Handphone1      
     , a.Handphone2      
     , a.Handphone3      
     , a.Handphone4      
     , a.DrivingLicense1      
     , a.DrivingLicense2      
     , a.Height      
     , a.Weight      
     , a.UniformSize      
     , a.UniformSizeAlt   
  from HrEmployee a      
  left join GnMstPosition c      
    on c.CompanyCode = a.CompanyCode      
   and c.DeptCode = a.Department      
   and c.PosCode = a.Position      
  left join GnMstLookUpDtl f      
    on f.CompanyCode = a.CompanyCode      
   and f.CodeID = 'MRTL'      
   and f.LookUpValue = a.MaritalStatus      
  left join GnMstLookUpDtl g      
    on g.CompanyCode = a.CompanyCode 
   and g.CodeID = 'RLGN'      
   and g.LookUpValue = a.Religion      
  left join GnMstLookUpDtl h      
    on h.CompanyCode = a.CompanyCode 
   and h.CodeID = 'FEDU'      
   and h.LookUpValue = a.FormalEducation      
  left join GnMstLookUpDtl i      
    on i.CompanyCode = a.CompanyCode 
   and i.CodeID = 'GNDR'      
   and i.LookUpValue = a.Gender  
  left join HrEmployeeSales e      
    on e.CompanyCode = a.CompanyCode      
   and e.EmployeeID = a.EmployeeID      
 where 1 = 1
   --and a.CompanyCode = @CompanyCode      
   and a.CompanyCode = (case @CompanyCode when '' then a.CompanyCode else @CompanyCode end)      
   and a.Department = @DeptCode      
   and a.Position = (case @PosCode when '' then a.Position else @PosCode end)      
   and a.PersonnelStatus = (case @Status when '' then a.PersonnelStatus else @Status end)      
)      
select x.CompanyCode
     , x.EmployeeID
     , x.SalesID
     , x.EmployeeName
     , x.BranchCode
     , c.OutletName BranchName
     , x.DeptCode
     , x.PosCode
     , Position = ''
         + upper(x.DeptCode)      
         + upper(case isnull(b.PosName, '') when '' then '' else ' - ' + b.PosName end)      
         + upper(case isnull(d.LookUpValueName, '') when '' then '' else ' - ' + d.LookUpValueName end)    
     , b.PosName
     , GradeCode = x.Grade
     , Grade = upper(d.LookUpValueName)
     , x.Status
     , x.Address1
     , x.Address2
     , x.Address3
     , x.Address4
     , x.Address
     , x.BirthDate
     , x.BirthPlace
     , x.BloodCode
     , x.SubOrdinates
     , x.MutationTimes
     , x.AchieveTimes
     , x.TeamLeader
     , x.JoinDate
     , x.ResignDate
     , x.ResignDescription
     , x.MaritalStatus
     , x.Religion
     , x.Education
     , x.Gender
     , upper(x.Province) as Province
     , x.District as City
     , x.SubDistrict
     , x.Village
     , x.ZipCode
     , x.IdentityNo
     , x.NPWPNo
     , x.NPWPDate
     , x.Email
     , x.FaxNo
     , x.Telephone1
     , x.Telephone2
     , x.Handphone1
     , x.Handphone2
     , x.Handphone3
     , x.Handphone4
     , x.DrivingLicense1
     , x.DrivingLicense2
     , x.Height
     , x.Weight
     , x.UniformSize
     , x.UniformSizeAlt
     , JobOther1 = ISNULL((
            select y.PosName
              from HrEmployeeAdditionalJob job
              inner join gnMstPosition y on job.CompanyCode = y.CompanyCode and job.Department = y.DeptCode and y.PosCode = job.Position
             where job.CompanyCode = x.CompanyCode and job.EmployeeID = x.EmployeeID and job.SeqNo = 1), '')
     , JobOther2 = ISNULL((
            select y.PosName
              from HrEmployeeAdditionalJob job
              inner join gnMstPosition y on job.CompanyCode = y.CompanyCode and job.Department = x.DeptCode and y.PosCode = job.Position
             where job.CompanyCode = x.CompanyCode and job.EmployeeID = x.EmployeeID and job.SeqNo = 2), '')
  from x 
  left join GnMstPosition b      
    on b.CompanyCode = x.CompanyCode
   and b.DeptCode = x.DeptCode      
   and b.PosCode = x.PosCode  
  left join [gnMstDealerOutletMapping] c
    on c.DealerCode = x.CompanyCode
   and c.OutletCode = x.BranchCode     
  left join GnMstLookUpDtl d      
    on d.CompanyCode = x.CompanyCode     
   and d.CodeID = 'ITSG'      
   and d.LookUpValue = x.Grade      
 where isnull(x.BranchCode, '') = (case @BranchCode when '' then x.BranchCode else @BranchCode end)
