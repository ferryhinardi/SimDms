
alter procedure uspfn_OmTrSalesSOSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , BranchCode
     , SONo
     , SODate
     , SalesType
     , RefferenceNo
     , RefferenceDate
     , CustomerCode
     , TOPCode
     , TOPDays
     , BillTo
     , ShipTo
     , ProspectNo
     , SKPKNo
     , Salesman
     , WareHouseCode
     , isLeasing
     , LeasingCo
     , GroupPriceCode
     , Insurance
     , PaymentType
     , PrePaymentAmt
     , PrePaymentDate
     , PrePaymentBy
     , CommissionBy
     , CommissionAmt
     , PONo
     , ContractNo
     , RequestDate
     , Remark
     , Status
     , ApproveBy
     , ApproveDate
     , RejectBy
     , RejectDate
     , CreatedBy
     , CreatedDate
     , LastUpdateBy
     , LastUpdateDate
     , isLocked
     , LockingBy
     , LockingDate
     , SalesCode 
     , Installment
     , FinalPaymentDate
     , SalesCoordinator
     , SalesHead
     , BranchManager
  from omTrSalesSO
 where LastUpdateDate is not null
   and LastUpdateDate > @LastUpdateDate
 order by LastUpdateDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_OmTrSalesSOSend @LastUpdateDate='2013-01-01', @Segment=1