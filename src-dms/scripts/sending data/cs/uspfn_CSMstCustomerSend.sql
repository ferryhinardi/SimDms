
alter procedure uspfn_CSMstCustomerSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , CustomerCode
     , StandardCode
     , CustomerName
     , CustomerAbbrName
     , CustomerGovName
     , CustomerType
     , CategoryCode
     , Address1
     , Address2
     , Address3
     , Address4
     , PhoneNo
     , HPNo
     , FaxNo
     , isPKP
     , NPWPNo
     , NPWPDate
     , SKPNo
     , SKPDate
     , ProvinceCode
     , AreaCode
     , CityCode
     , ZipNo
     , Status
     , CreatedBy
     , CreatedDate
     , LastUpdateBy
     , LastUpdateDate
     , isLocked
     , LockingBy
     , LockingDate
     , Email
     , BirthDate
     , Spare01
     , Spare02
     , Spare03
     , Spare04
     , Spare05
     , Gender
     , OfficePhoneNo
     , KelurahanDesa
     , KecamatanDistrik
     , KotaKabupaten
     , IbuKota
     , CustomerStatus
  from gnMstCustomer
 where LastUpdateDate is not null
   and LastUpdateDate > @LastUpdateDate
 order by LastUpdateDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_CSMstCustomerSend @LastUpdateDate='2013-01-01', @Segment=1