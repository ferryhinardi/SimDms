
ALTER procedure uspfn_CsCustomerBirthdaySend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode
     , CustomerCode
	 , PeriodYear
	 , TypeOfGift
	 , SentGiftDate
	 , ReceivedGiftDate
	 , Comment
	 , AdditionalInquiries
	 , Status
	 , CreatedBy
	 , CreatedDate
	 , UpdatedBy
	 , UpdatedDate
  from CsCustBirthDay
 where UpdatedDate is not null
   and UpdatedDate > @LastUpdateDate
 order by UpdatedDate asc )#t1

select * from #t1

  drop table #t1
