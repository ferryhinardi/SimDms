USE [SimDMSAP]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_HrInqPersInfo]    Script Date: 7/22/2015 4:07:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[uspfn_HrInqPersInfoCutOff]
  @GroupNo varchar(10),
  @CompanyCode varchar(20),
  @DeptCode varchar(20),
  @PosCode varchar(20),
  @Status varchar(20),
  @BranchCode varchar(20) = '',
  @CutOff varchar(30)
as      

--create realtime employees data
;with x as (
select max(MutationDate) as mutadate, CompanyCode, EmployeeID from hremployeemutation e
group by CompanyCode, EmployeeID--, BranchCode
--having count(employeeid) > 1
) select * into #temp from x

select h.* into #tHrEmployee from HrEmployee h
inner join hremployeemutation e
		on e.CompanyCode = h.CompanyCode 
	   and e.EmployeeID = h.EmployeeID
inner join #temp t
		on e.CompanyCode = t.CompanyCode 
	   and e.EmployeeID = t.EmployeeID
	   and e.MutationDate = t.mutadate
inner join gnMstDealerOutletMapping o
		on o.DealerCode = e.CompanyCode
	   and o.OutletCode = e.BranchCode
	 where isnull(h.isDeleted, 0) = 0
	   and o.GroupNo = (case when (@GroupNo = '' OR @GroupNo IS NULL) then o.GroupNo else @GroupNo end)
	   and o.DealerCode = (case when (@CompanyCode = '' OR @CompanyCode IS NULL) then o.DealerCode else @CompanyCode end)
	   and case h.JoinDate when null then h.CreatedDate else h.JoinDate end <= @CutOff
	   --and e.EmployeeID = 'S20106'

drop table #temp

;with x as (      
select a.CompanyCode
     , a.EmployeeID      
     , e.SalesID      
     , a.EmployeeName      
     , BranchCode = isnull((select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by MutationDate desc), '')      
     , DeptCode = a.Department
     , PosCode = a.Position
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
     , a.BirthDate      
     , upper(a.BirthPlace) BirthPlace
     , a.BloodCode      
     , SubOrdinates = (select count(x.EmployeeID) from HrEmployee x where x.TeamLeader=a.EmployeeID)      
     , MutationTimes = (select count(x.EmployeeID) from HrEmployeeMutation x where x.CompanyCode=a.CompanyCode and x.EmployeeID=a.EmployeeID and x.IsDeleted != 1)      
     , AchieveTimes = (select count(x.EmployeeID) from HrEmployeeAchievement x where x.CompanyCode=a.CompanyCode and x.EmployeeID=a.EmployeeID and x.IsDeleted != 1)      
     , TeamLeader = (select top 1 x.EmployeeName from HrEmployee x where x.EmployeeID=a.TeamLeader)      
     , a.JoinDate      
     , a.ResignDate      
     , a.ResignDescription      
     , MaritalStatus = upper(f.LookUpValueName)
     , Religion = upper(g.LookUpValueName)
     , Education = upper(h.LookUpValueName)
     , Gender = upper(i.LookUpValueName)
	 , AdditionalJob = (select top 1 x.Position from HrEmployeeAdditionalJob x where x.CompanyCode=a.CompanyCode and x.EmployeeID=a.EmployeeID and x.IsDeleted != 1 and @CutOff BETWEEN CONVERT(DATE,x.AssignDate) and CONVERT(DATE, x.ExpiredDate) )  
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
  from #tHrEmployee a      
  left join GnMstPosition c      
    on c.CompanyCode = a.CompanyCode      
   and c.DeptCode = a.Department      
   and c.PosCode = a.Position      
  left join GnMstLookUpDtl f      
    on f.CompanyCode = '0000000'      
   and f.CodeID = 'MRTL'      
   and f.LookUpValue = a.MaritalStatus      
  left join GnMstLookUpDtl g      
    on g.CompanyCode = '0000000'
   and g.CodeID = 'RLGN'      
   and g.LookUpValue = a.Religion      
  left join GnMstLookUpDtl h      
    on h.CompanyCode = '0000000'
   and h.CodeID = 'FEDU'      
   and h.LookUpValue = a.FormalEducation      
  left join GnMstLookUpDtl i      
    on i.CompanyCode = '0000000'
   and i.CodeID = 'GNDR'      
   and i.LookUpValue = a.Gender  
  left join HrEmployeeSales e      
    on e.CompanyCode = a.CompanyCode      
   and e.EmployeeID = a.EmployeeID      
 where 1 = 1
   -- and a.CompanyCode = (case @CompanyCode when '' then a.CompanyCode else @CompanyCode end)
   and a.Department = @DeptCode      
   and a.Position = (case @PosCode when '' then a.Position else @PosCode end)      
   and a.PersonnelStatus = (case @Status when '' then a.PersonnelStatus else @Status end)      
)    
,y as (  
select x.CompanyCode
     , e.DealerName
     , x.EmployeeID
	 , e.ShortName + ' - ' 
		+ substring(cast(year(x.JoinDate) as varchar), len(cast(year(x.JoinDate) as varchar))-1,2) + ' - ' 
		+ right(replicate('0',7) + cast(x.SalesID as varchar(7)), 7) AS SalesID
	 , x.EmployeeName
	 , x.BranchCode
	 , c.ShortBranchName as BranchName
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
	 , x.AdditionalJob
	 , upper(x.Province) as Province
	 , x.District
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
     , PreTraining = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP1')
     , PreTrainingPostTest = ISNULL((select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP1'),0)
     , Pembekalan = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP2')
     , PembekalanPostTest = ISNULL((select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP2'),0)
     , Salesmanship = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP3')
     , SalesmanshipPostTest = ISNULL((select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP3'),0)
     , Ojt = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP4')
     , OjtPostTest = ISNULL((select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP4'),0)
     , FinalReview = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP7')
     , FinalReviewPostTest = ISNULL((select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP7'),0)
     , SpsSlv = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SPSS')
     , SpsSlvPostTest = ISNULL((select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SPSS'),0)
     , SpsGld = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SPSG')
     , SpsGldPostTest = ISNULL((select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SPSG'),0)
     , SpsPlt = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SPSP')
     , SpsPltPostTest = ISNULL((select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SPSP'),0)
     , SCBsc = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SCB')
     , SCAdv = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SCA')
     , SHBsc = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SHB')
     , SHInt = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SHI')
     , BMBsc = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'BMB')
     , BMInt = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'BMI')
  from x 
  left join GnMstPosition b      
    on b.CompanyCode = '0000000'
   and b.DeptCode = x.DeptCode      
   and b.PosCode = x.PosCode  
  left join OutletInfo c
    on c.CompanyCode = x.CompanyCode
   and c.BranchCode = x.BranchCode     
  left join GnMstLookUpDtl d      
    on d.CompanyCode = '0000000'      
   and d.CodeID = 'ITSG'      
   and d.LookUpValue = x.Grade      
  left join DealerInfo e      
    on e.DealerCode = x.CompanyCode
 where isnull(x.BranchCode, '') = (case when (@BranchCode = '' OR @BranchCode IS NULL) then x.BranchCode else @BranchCode end)
)

-- select * from y
select 
	   y.SalesID -- as [ATPM ID] 
	 , y.EmployeeID -- as Nik
     , y.DeptCode -- as Department
	 , y.CompanyCode -- as [Kode Dealer]
     , y.DealerName -- as [Nama Dealer]
     , y.BranchCode -- as [Kode Outlet]
     , y.BranchName -- as [Nama Outlet]
     , y.EmployeeName -- as Nama
     , y.PosName -- as Jabatan
     , y.Grade -- as Grade
     , y.AdditionalJob
     , y.TeamLeader -- as [Leader Name]
     , y.Status
     , y.JoinDate -- as [Tanggal Join]
     , y.ResignDate -- as [Tanggal Resign]
     , y.ResignDescription -- as [Alasan Resign]
     , y.BirthDate -- as [ Tanggal Lahir]
     , y.BirthPlace -- as [Tempat Lahir]
     , y.Gender -- as [Jenis Kelamin]
     , y.Religion -- as Agama
     , y.MaritalStatus -- as [Status Perkawinan]
     , y.Education -- as Pendidikan
     , y.Address -- as Alamat
     , y.District as City -- as Kota
     , y.Province -- as Provinsi
     , y.ZipCode -- as [Kode Pos]
     , y.IdentityNo -- as KTP
     , y.NPWPNo -- as NPWP
	 , y.DrivingLicense1 -- as [SIM 1]
	 , y.DrivingLicense2 -- as [SIM 2]
     , y.Email 
     , y.Telephone1 -- as [Phone]
     --, y.Telephone2
     , y.Handphone1 -- as [HP 1]
     , y.Handphone2 -- as [HP 2]
     , y.Handphone3 -- as [Pin BB]
     --, y.Handphone4 -- as PinBB
     , y.Height -- as [Tinggi Badan]
     , y.Weight -- as [Berat Badan]
     , y.UniformSize
     , y.UniformSizeAlt
     , y.PreTraining
     , y.PreTrainingPostTest -- as [Nilai Pre Training]
     , y.Pembekalan 
     , y.PembekalanPostTest -- as [Nilai Pembekalan]
     , y.Salesmanship as [Salesmanship]
     , y.SalesmanshipPostTest -- as [Nilai Salesmanship]
     , y.Ojt as [OJT]
     --, y.OjtPostTest -- as [Nilai Ojt]
     , y.FinalReview -- as [Final Review]
     , y.FinalReviewPostTest -- as [Nilai Akhir STDP]
     , y.SpsSlv -- as [SPS Silver]
     --, y.SpsSlvPostTest -- as [Sales Silver PostTest]
     , y.SpsGld -- as [SPS Gold]
     --, y.SpsGldPostTest -- as [Sales Gold PostTest]
     , y.SpsPlt -- as [SPS Platinum]
     --, y.SpsPltPostTest -- as [Sales Platinum PostTest]
     --, y.SCBsc -- as [SC Basic]
     --, y.SCAdv -- as [SC Advance]
     , y.SHBsc -- as [SH Basic]
     , y.SHInt -- as [SH Intermediate]
     , y.BMBsc -- as [BMDP Basic]
     , y.BMInt -- as [BMDP Intermediate]
  from y where 1 = 1
  order by y.DealerName, y.BranchCode, y.EmployeeID
   --and (y.PreTraining is not null or y.Pembekalan is not null or y.Salesmanship is not null or y.Ojt is not null or y.FinalReview is not null )
   --and (y.SpsSlv is not null or y.SpsGld is not null or y.SpsPlt is not null)
   --and (y.SCBsc is not null or y.SCAdv is not null)
   --and (y.SHBsc is not null or y.SHInt is not null)

   drop table #tHrEmployee