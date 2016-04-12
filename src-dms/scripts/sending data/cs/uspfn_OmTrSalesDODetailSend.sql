
alter procedure uspfn_OmTrSalesDODetailSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , BranchCode
     , DONo
     , DOSeq
     , SalesModelCode
     , SalesModelYear
     , ChassisCode
     , ChassisNo
     , EngineCode
     , EngineNo
     , ColourCode
     , Remark
     , StatusBPK
     , CreatedBy
     , CreatedDate
     , LastUpdateBy
     , LastUpdateDate
  from omTrSalesDODetail
 where LastUpdateDate is not null
   and LastUpdateDate > @LastUpdateDate
 order by LastUpdateDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_OmTrSalesDODetailSend @LastUpdateDate='2013-01-01', @Segment=1