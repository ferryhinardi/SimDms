
alter procedure uspfn_OmTrSalesInvoiceVinSend 
	@LastUpdateDate datetime,
	@Segment int
as

select * into #t1 from (
select top (@Segment) CompanyCode 
     , BranchCode
	 , InvoiceNo
	 , BPKNo
	 , SalesModelCode
	 , SalesModelYear
	 , InvoiceSeq
	 , ColourCode
	 , ChassisCode
	 , ChassisNo
	 , EngineCode
	 , EngineNo
	 , COGS
	 , IsReturn
	 , CreatedBy
	 , CreatedDate
	 , LastUpdateBy
	 , LastUpdateDate
  from omTrSalesInvoiceVin
 where LastUpdateDate is not null
   and LastUpdateDate > @LastUpdateDate
 order by LastUpdateDate asc )#t1

select * from #t1

  drop table #t1


--go
--exec uspfn_OmTrSalesInvoiceVinSend @LastUpdateDate='2013-01-01', @Segment=1