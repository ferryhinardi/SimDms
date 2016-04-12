
alter procedure uspfn_CsCustomerVehicleSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , CustomerCode
     , Chassis
     , StnkDate
     , BpkbDate
     , CreatedBy
     , CreatedDate
     , UpdatedBy
     , UpdatedDate
  from CsCustomerVehicle
 where UpdatedDate is not null
   and UpdatedDate > @LastUpdateDate
 order by UpdatedDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_CsCustomerVehicleSend @LastUpdateDate='2013-01-01', @Segment=1