go
if object_id('uspfn_HrInqPersInfo') is not null
	drop procedure uspfn_HrInqPersInfo

go
create procedure uspfn_HrInqPersInfo      
 @CompanyCode varchar(20),      
 @DeptCode varchar(20),      
 @PosCode varchar(20),      
 @Status varchar(20),      
 @BranchCode varchar(20) = ''      
as      
      
;with x as (      
select a.EmployeeID      
     , e.SalesID      
     , a.EmployeeName      
     , BranchCode = isnull((select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and isDeleted != 1 order by MutationDate desc), '')      
     , BranchName = isnull((      
  select top 1 y.CompanyName      
    from HrEmployeeMutation x      
    left join GnMstCoProfile y      
      on y.CompanyCode = x.CompanyCode      
     and y.BranchCode = x.BranchCode      
   where x.CompanyCode = a.CompanyCode      
     and x.EmployeeID = a.EmployeeID      
     and x.IsDeleted != 1    
   order by x.MutationDate desc), '')  
     , a.Department    
     , Position = upper(a.Department)      
         + upper(case isnull(c.PosName, '') when '' then '' else ' - ' + c.PosName end)      
         + upper(case isnull(d.LookUpValueName, '') when '' then '' else ' - ' + d.LookUpValueName end)      
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
  , Address = a.Address1 + ' ' + ltrim(isnull(a.Address2, '')) + ' ' + ltrim(isnull(a.Address3, '')) + ' ' + ltrim(isnull(a.Address4, ''))      
  , Grade = d.LookUpValueName      
  , a.BirthDate      
  , a.BirthPlace      
  , a.BloodCode      
  , SubOrdinates = (select count(x.EmployeeID) from HrEmployee x where x.TeamLeader=a.EmployeeID)      
  , MutationTimes = (select count(x.EmployeeID) from HrEmployeeMutation x where x.CompanyCode=a.CompanyCode and x.EmployeeID=a.EmployeeID and x.IsDeleted != 1)      
  , AchieveTimes = (select count(x.EmployeeID) from HrEmployeeAchievement x where x.CompanyCode=a.CompanyCode and x.EmployeeID=a.EmployeeID and x.IsDeleted != 1)      
  , TeamLeader = (select top 1 x.EmployeeName from HrEmployee x where x.EmployeeID=a.TeamLeader)      
  , a.JoinDate      
  , a.ResignDate      
  , a.ResignDescription      
  , MaritalStatus = f.LookUpValueName      
  , Religion = g.LookUpValueName      
  , Education = h.LookUpValueName      
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
  , ShoesSize  
  , Gender = ( case when Gender='L' then 'Pria' else 'Wanita' end )
  from HrEmployee a      
  left join GnMstPosition c      
    on c.CompanyCode = a.CompanyCode      
   and c.DeptCode = a.Department      
   and c.PosCode = a.Position      
  left join GnMstLookUpDtl d      
    on d.CompanyCode = a.CompanyCode      
   and d.CodeID = 'ITSG'      
   and d.LookUpValue = a.Grade      
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
  left join HrEmployeeSales e      
    on e.CompanyCode = a.CompanyCode      
   and e.EmployeeID = a.EmployeeID      
 where a.CompanyCode = @CompanyCode      
   and a.Department = @DeptCode      
   and a.Position = (case @PosCode when '' then a.Position else @PosCode end)      
   and a.PersonnelStatus = (case @Status when '' then a.PersonnelStatus else @Status end)      
)      
select * from x where isnull(x.BranchCode, '') = (case @BranchCode when '' then x.BranchCode else @BranchCode end) 