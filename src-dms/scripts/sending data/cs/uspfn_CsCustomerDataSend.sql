
alter procedure uspfn_CsCustomerDataSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , CustomerCode
     , AddPhone1
     , AddPhone2
     , ReligionCode
     , IsDeleted
     , CreatedBy
     , CreatedDate
     , UpdatedBy
     , UpdatedDate
  from CsCustData
 where UpdatedDate is not null
   and UpdatedDate > @LastUpdateDate
 order by UpdatedDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_CsCustomerDataSend @LastUpdateDate='2013-01-01', @Segment=1