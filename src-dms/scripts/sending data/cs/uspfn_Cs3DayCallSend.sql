
alter procedure uspfn_Cs3DayCallSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , CustomerCode
     , Chassis
     , IsDeliveredA
     , IsDeliveredB
     , IsDeliveredC
     , IsDeliveredD
     , IsDeliveredE
     , IsDeliveredF
     , IsDeliveredG
     , Comment
     , Additional
     , Status
     , FinishDate
     , CreatedBy
     , CreatedDate
     , UpdatedBy
     , UpdatedDate
  from CsTDayCall
 where UpdatedDate is not null
   and UpdatedDate > @LastUpdateDate
 order by UpdatedDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_Cs3DayCallSend @LastUpdateDate='2013-01-01', @Segment=1