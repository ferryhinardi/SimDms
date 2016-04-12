
alter procedure uspfn_CsHolidaySend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , CustomerCode
     , PeriodYear
     , GiftSeq
     , ReligionCode
     , HolidayCode
     , IsGiftCard
     , IsGiftLetter
     , IsGiftSms
     , IsGiftSouvenir
     , SouvenirSent
     , SouvenirReceived
     , Comment
     , Additional
     , Status
     , CreatedBy
     , CreatedDate
     , UpdatedBy
     , UpdatedDate
  from CsCustHoliday
 where UpdatedDate is not null
   and UpdatedDate > @LastUpdateDate
 order by UpdatedDate asc )#t1

select * from #t1

  drop table #t1


go
exec uspfn_CsHolidaySend @LastUpdateDate='2013-01-01', @Segment=1