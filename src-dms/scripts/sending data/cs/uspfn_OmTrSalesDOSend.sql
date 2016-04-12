
alter procedure uspfn_OmTrSalesDOSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , BranchCode
     , DONo
     , DODate
     , SONo
     , CustomerCode
     , ShipTo
     , WareHouseCode
     , Expedition
     , Remark
     , Status
     , CreatedBy
     , CreatedDate
     , LastUpdateBy
     , LastUpdateDate
     , isLocked
     , LockingBy
     , LockingDate
  from omTrSalesDO
 where LastUpdateDate is not null
   and LastUpdateDate > @LastUpdateDate
 order by LastUpdateDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_OmTrSalesDOSend @LastUpdateDate='2013-01-01', @Segment=1