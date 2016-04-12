
alter procedure uspfn_CsRelationSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , CustomerCode
     , RelationType
     , FullName
     , PhoneNo
     , RelationInfo
     , BirthDate
     , TypeOfGift
     , SentGiftDate
     , ReceivedGiftDate
     , Comment
     , CreatedBy
     , CreatedDate
     , UpdatedBy
     , UpdatedDate
  from CsCustRelation
 where UpdatedDate is not null
   and UpdatedDate > @LastUpdateDate
 order by UpdatedDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_CsRelationSend @LastUpdateDate='2013-01-01', @Segment=1