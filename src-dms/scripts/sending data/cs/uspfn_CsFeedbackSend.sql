
alter procedure uspfn_CsFeedbackSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , CustomerCode
     , Chassis
     , IsManual
     , FeedbackA
     , FeedbackB
     , FeedbackC
     , FeedbackD
     , CreatedBy
     , CreatedDate
     , UpdatedBy
     , UpdatedDate
  from CsCustFeedback
 where UpdatedDate is not null
   and UpdatedDate > @LastUpdateDate
 order by UpdatedDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_CsFeedbackSend @LastUpdateDate='2013-01-01', @Segment=1