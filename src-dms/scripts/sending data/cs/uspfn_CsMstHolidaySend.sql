
alter procedure uspfn_CsMstHolidaySend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , HolidayYear
	 , HolidayCode
	 , HolidayDesc
	 , DateFrom
	 , DateTo
	 , CreatedBy
	 , CreatedDate
	 , UpdatedBy
	 , UpdatedDate
  from CsMstHoliday
 where UpdatedDate is not null
   and UpdatedDate > @LastUpdateDate
 order by UpdatedDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_CsMstHolidaySend @LastUpdateDate='2013-01-01', @Segment=1