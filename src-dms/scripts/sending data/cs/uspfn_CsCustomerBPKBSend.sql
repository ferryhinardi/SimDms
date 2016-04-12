
alter procedure uspfn_CsCustomerBPKBSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , CustomerCode
	 , Chassis
	 , BpkbReadyDate
	 , BpkbPickUp
	 , ReqInfoLeasing
	 , ReqInfoCust
	 , ReqKtp
	 , ReqStnk
	 , ReqSuratKuasa
	 , Comment
	 , Additional
	 , Status
	 , FinishDate
	 , CreatedBy
	 , CreatedDate
	 , UpdatedBy
	 , UpdatedDate
	 , Tenor
	 , CustomerCategory
  from CsCustBpkb
 where UpdatedDate is not null
   and UpdatedDate > @LastUpdateDate
 order by UpdatedDate asc )#t1

select * from #t1

  drop table #t1
