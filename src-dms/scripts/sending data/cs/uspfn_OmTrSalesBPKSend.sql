
alter procedure uspfn_OmTrSalesBPKSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , BranchCode
     , BPKNo
     , BPKDate
     , DONo
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
  from omTrSalesBPK
 where LastUpdateDate is not null
   and LastUpdateDate > @LastUpdateDate
 order by LastUpdateDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_OmTrSalesBPKSend @LastUpdateDate='2013-01-01', @Segment=1