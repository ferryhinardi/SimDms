
alter procedure uspfn_SvMsVehicle 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , ChassisCode
     , ChassisNo
     , EngineCode
     , EngineNo
     , PoliceRegNo
     , BasicModel
     , TransmissionType
     , TechnicalModelCode
     , ServiceBookNo
     , ColourCode
     , DealerCode
     , FakturPolisiDate
     , CustomerCode
     , ClubCode
     , ClubNo
     , ClubDateStart
     , ClubDateFinish
     , ClubSince
     , IsClubStatus
     , ContractNo
     , IsContractStatus
     , RemainderDate
     , RemainderDescription
     , FirstServiceDate
     , LastJobType
     , LastServiceDate
     , LastServiceOdometer
     , IsActive
     , ProductionYear
     , CreatedBy
     , CreatedDate
     , LastupdateBy
     , LastupdateDate
     , ContactName
     , ContactAddress
     , ContactPhone
  from svMstCustomerVehicle
 where LastupdateDate is not null
   and LastupdateDate > @LastUpdateDate
 order by LastupdateDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_SvMsVehicle @LastUpdateDate='2013-01-01', @Segment=1