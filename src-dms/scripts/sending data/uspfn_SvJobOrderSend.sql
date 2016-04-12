
go
if object_id('uspfn_SvJobOrderSend') is not null	
	drop procedure uspfn_SvJobOrderSend

go
create procedure uspfn_SvJobOrderSend 
	@LastUpdateDate datetime,
	@Segment int

--select @LastUpdateDate='1990-01-01 00:00:00',@Segment=500
as

select * into #t1 from (
select top (@Segment) CompanyCode, BranchCode, ProductType, ServiceNo
	 , ServiceType, ServiceStatus, JobOrderNo, JobOrderDate, EstimationNo, EstimationDate
	 , BookingNo, BookingDate, InvoiceNo, ForemanID, MechanicID, CustomerCode, CustomerCodeBill
	 , PoliceRegNo, ServiceBookNo, BasicModel, TransmissionType, VIN, ChassisCode, ChassisNo
	 , EngineCode, EngineNo, ColorCode, Odometer, JobType, ServiceRequestDesc, ConfirmChangingPart
	 , EstimateFinishDate, EstimateFinishDateSys, LaborDiscPct, PartDiscPct
	 , MaterialDiscAmt, InsurancePayFlag, InsuranceOwnRisk, InsuranceNo, InsuranceJobOrderNo
	 , PPNPct, PPHPct, LaborGrossAmt, PartsGrossAmt, MaterialGrossAmt, LaborDiscAmt, PartsDiscAmt
	 , MaterialDiscPct, LaborDppAmt, PartsDppAmt, MaterialDppAmt, TotalDPPAmount
	 , TotalPphAmount, TotalPpnAmount, TotalSrvAmount, PrintSeq, IsLocked, LockingBy, LockingDate
	 , CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, IsSparepartClaim
  from svTrnService
 where LastUpdateDate is not null
   and LastUpdateDate > @LastUpdateDate
 order by LastUpdateDate asc )#t1
 
declare @LastUpdateQry datetime
    set @LastUpdateQry = (select top 1 LastUpdateDate from #t1 order by LastUpdateDate desc)

select * from #t1
 union
select CompanyCode, BranchCode, ProductType, ServiceNo
	 , ServiceType, ServiceStatus, JobOrderNo, JobOrderDate, EstimationNo, EstimationDate
	 , BookingNo, BookingDate, InvoiceNo, ForemanID, MechanicID, CustomerCode, CustomerCodeBill
	 , PoliceRegNo, ServiceBookNo, BasicModel, TransmissionType, VIN, ChassisCode, ChassisNo
	 , EngineCode, EngineNo, ColorCode, Odometer, JobType, ServiceRequestDesc, ConfirmChangingPart
	 , EstimateFinishDate, EstimateFinishDateSys, LaborDiscPct, PartDiscPct
	 , MaterialDiscAmt, InsurancePayFlag, InsuranceOwnRisk, InsuranceNo, InsuranceJobOrderNo
	 , PPNPct, PPHPct, LaborGrossAmt, PartsGrossAmt, MaterialGrossAmt, LaborDiscAmt, PartsDiscAmt
	 , MaterialDiscPct, LaborDppAmt, PartsDppAmt, MaterialDppAmt, TotalDPPAmount
	 , TotalPphAmount, TotalPpnAmount, TotalSrvAmount, PrintSeq, IsLocked, LockingBy, LockingDate
	 , CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, IsSparepartClaim
  from svTrnService
 where LastUpdateDate = @LastUpdateQry
 
  drop table #t1
