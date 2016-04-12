
go
if object_id('uspfn_SvInvoiceSend') is not null
	drop procedure uspfn_SvInvoiceSend

go
create procedure uspfn_SvInvoiceSend 
	@LastUpdateDate datetime,
	@Segment int  
--select @LastUpdateDate='1990-01-01 00:00:00',@Segment=500  
as  
  
select * into #t1 from (  
select top (@Segment) CompanyCode, BranchCode, ProductType, InvoiceNo  
  , InvoiceDate, InvoiceStatus, FPJNo, FPJDate, JobOrderNo, JobOrderDate, JobType  
  , ServiceRequestDesc, ChassisCode, ChassisNo, EngineCode, EngineNo, PoliceRegNo  
  , BasicModel, CustomerCode, CustomerCodeBill, Odometer, IsPKP, TOPCode, TOPDays, DueDate  
  , SignedDate, LaborDiscPct, PartsDiscPct, MaterialDiscPct, PphPct, PpnPct, LaborGrossAmt  
  , PartsGrossAmt, MaterialGrossAmt, LaborDiscAmt, PartsDiscAmt, MaterialDiscAmt, LaborDppAmt  
  , PartsDppAmt, MaterialDppAmt, TotalDppAmt, TotalPphAmt, TotalPpnAmt, TotalSrvAmt, Convert(varchar(max), Remarks) as Remarks
  , PrintSeq, PostingFlag, PostingDate, IsLocked, LockingBy  
  , LockingDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate  
  from svTrnInvoice  
 where LastUpdateDate is not null  
   and LastUpdateDate > @LastUpdateDate  
 order by LastUpdateDate asc )#t1  
   
declare @LastUpdateQry datetime  
    set @LastUpdateQry = (select top 1 LastUpdateDate from #t1 order by LastUpdateDate desc)  
  
select * from #t1  
 union 
select top 100 CompanyCode, BranchCode, ProductType, InvoiceNo  
  , InvoiceDate, InvoiceStatus, FPJNo, FPJDate, JobOrderNo, JobOrderDate, JobType  
  , ServiceRequestDesc, ChassisCode, ChassisNo, EngineCode, EngineNo, PoliceRegNo  
  , BasicModel, CustomerCode, CustomerCodeBill, Odometer, IsPKP, TOPCode, TOPDays, DueDate  
  , SignedDate, LaborDiscPct, PartsDiscPct, MaterialDiscPct, PphPct, PpnPct, LaborGrossAmt  
  , PartsGrossAmt, MaterialGrossAmt, LaborDiscAmt, PartsDiscAmt, MaterialDiscAmt, LaborDppAmt  
  , PartsDppAmt, MaterialDppAmt, TotalDppAmt, TotalPphAmt, TotalPpnAmt, TotalSrvAmt, Convert(varchar(max), Remarks) as Remarks
  , PrintSeq, PostingFlag, PostingDate, IsLocked, LockingBy  
  , LockingDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate  
  from svTrnInvoice  
 where LastUpdateDate = @LastUpdateQry  
   
  drop table #t1  