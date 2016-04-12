
alter procedure uspfn_OmTrSalesInvoiceSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , BranchCode
     , InvoiceNo
     , InvoiceDate
     , SONo
     , CustomerCode
     , BillTo
     , FakturPajakNo
     , FakturPajakDate
     , DueDate
     , isStandard
     , Remark
     , Status
     , CreatedBy
     , CreatedDate
     , LastUpdateBy
     , LastUpdateDate
     , isLocked
     , LockingBy
     , LockingDate
  from omTrSalesInvoice
 where LastUpdateDate is not null
   and LastUpdateDate > @LastUpdateDate
 order by LastUpdateDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_OmTrSalesInvoiceSend @LastUpdateDate='2013-01-01', @Segment=1