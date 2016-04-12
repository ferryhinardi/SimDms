delete HrEmployee
insert into HrEmployee
select a.CompanyCode
     , a.EmployeeID
     , a.EmployeeName
     , a.Email
     , a.FaxNo
     , coalesce(a.HpNo, a.Phone1, a.Phone2, a.Phone3)
     , coalesce(a.Phone1, a.Phone2, a.Phone3, a.Phone4)
     , a.Phone2 
     , a.Phone3 
     , a.Phone4
     , a.PhoneNo 
     , '.' as OfficeLocation
     , 0 as IsLinkedUser
     , '' RelatedUser
     , a.JoinDate
     , a.DeptCode
     , a.PosCode
     , a.Grade
     , '' as Rank
     , a.GenderCode
     , a.TeamLeaderID
     , a.PersonnelStatus
     , a.ResignDate
     , a.ResignReason
     , a.IdentityNo
     , (select top 1 NPWPNo from gnMstEmployeeData where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by UpdatedDate desc) NPWPNo
     , (select top 1 NPWPDate from gnMstEmployeeData where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by UpdatedDate desc) NPWPDate
     , a.BirthDate
     , a.BirthPlace
     , a.Address1
     , a.Address2
     , a.Address3
     , '' Address4
     , (select top 1 ProvinceCode from gnMstEmployeeData where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and ProvinceCode is not null order by UpdatedDate desc) Province
     , (select top 1 District from gnMstEmployeeData where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and District is not null order by UpdatedDate desc) District
     , (select top 1 AreaCode from gnMstEmployeeData where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and AreaCode is not null order by UpdatedDate desc) SubDistrict
     , '' Village
     , a.ZipNo
     , a.DrivingLicense1
     , a.DrivingLicense2
     , a.MaritalStatusCode
     , a.MaritalStatusCode
     , a.Height
     , a.Weight
     , a.UniformSize
     , a.SizeAlt
     , a.ShoesSize
     , a.FormalEducation
     , a.BloodCode
     , '' OtherInformation
     , 'konversi'
     , a.CreatedDate
     , 'konversi'
     , getdate()
     , a.ReligionCode
     , null
     , null
  from SyncEmployeeData a

select * from HrEmployee 


