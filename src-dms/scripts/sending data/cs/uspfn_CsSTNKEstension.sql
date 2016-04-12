
alter procedure uspfn_CsSTNKEstension 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , CustomerCode
     , Chassis
     , IsStnkExtend
     , StnkExpiredDate
     , ReqKtp
     , ReqStnk
     , ReqBpkb
     , ReqSuratKuasa
     , Comment
     , Additional
     , Status
     , FinishDate
     , CreatedBy
     , CreatedDate
     , LastUpdatedBy
     , LastUpdatedDate
     , Tenor
     , LeasingCode
     , CustomerCategory
  from CsStnkExt
 where LastUpdatedDate is not null
   and LastUpdatedDate > @LastUpdateDate
 order by LastUpdatedDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_CsSTNKEstension @LastUpdateDate='2013-01-01', @Segment=1