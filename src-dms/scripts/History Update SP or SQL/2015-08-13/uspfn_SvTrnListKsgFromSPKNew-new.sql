if object_id('uspfn_SvTrnListKsgFromSPKNew') is not null
	drop procedure uspfn_SvTrnListKsgFromSPKNew
GO

/****** Object:  StoredProcedure [dbo].[uspfn_SvTrnListKsgFromSPKNew]    Script Date: 08/13/2015 14:05:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [dbo].[uspfn_SvTrnListKsgFromSPKNew]
--DECLARE
 @CompanyCode varchar(15),  
 @ProductType varchar(15),   
 @BranchFrom varchar(15),  
 @BranchTo varchar(15),  
 @PeriodFrom datetime,  
 @PeriodTo datetime,  
 @JobPDI as varchar(15),  
 @JobFSC as varchar(15),
 @BranchCode varchar(15)
 
 --select @CompanyCode='6115204001', @ProductType='2W',@BranchFrom='6115204331',@BranchTo='6115204336',@PeriodFrom='20150501',@PeriodTo='20150527',
 --@JobPDI='%PDI%',@JobFSC='',@BranchCode='6115204331'
as        
begin

declare @IsCentralize as varchar(1)
set @IsCentralize = '0'
if(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='SRV_FLAG' and LookUpValue='KSG_HOLDING') > '0'
	set @IsCentralize = '1'
	 	 
select * into #t1 from(  
select  
    convert(bit, 1) Process      
 , srv.BranchCode  
 , srv.JobOrderNo  
 , case when convert(varchar, srv.JobOrderDate, 106) = '19000101' then '' else convert(varchar, srv.JobOrderDate, 106) end JobOrderDate  
 , srv.BasicModel  
 , srv.ServiceBookNo  
 , job.PdiFscSeq  
 , srv.Odometer  
 , srv.LaborGrossAmt  
 , round((select isnull(SUM((SupplyQty - ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = 'F'),0) MaterialGrossAmt --Pembulatan
 , round((srv.LaborGrossAmt + (select isnull(SUM((SupplyQty - ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = 'F')),0) PdiFscAmount  --Pembulatan
 , isnull(case when convert(varchar, veh.FakturPolisiDate, 112) = '19000101' then '' else convert(varchar, veh.FakturPolisiDate, 106) end, '')  FakturPolisiDate  
 , isnull(case when convert(varchar, mstVeh.BPKDate, 112) = '19000101' then '' else convert(varchar, mstVeh.BPKDate, 106) end, '')  BPKDate  
 , srv.ChassisCode  
 , srv.ChassisNo  
 , srv.EngineCode  
 , srv.EngineNo   
    , srv.InvoiceNo  
 , isnull(inv.FPJNo, '') FPJNo  
 , isnull(case when convert(varchar, inv.FPJDate, 112) = '19000101' then '' else convert(varchar, inv.FPJDate, 106) end, '')  FPJDate  
 , isnull(fpj.FPJGovNo, '') FPJGovNo  
 , srv.TransmissionType  
 , srv.ServiceStatus  
 , srv.CompanyCode  
 , srv.ProductType  
from svTrnService srv  
left join svMstJob job  
 on job.CompanyCode = srv.CompanyCode  
  and job.ProductType = srv.ProductType  
  and job.BasicModel = srv.BasicModel  
  and job.JobType = srv.JobType  
left join svMstCustomerVehicle veh  
 on veh.CompanyCode = srv.CompanyCode  
  and veh.ChassisCode = srv.ChassisCode  
  and veh.ChassisNo = srv.ChassisNo  
left join omMstVehicle mstVeh  
 on mstVeh.CompanyCode = srv.CompanyCode  
  and mstVeh.ChassisCode = srv.ChassisCode  
  and mstVeh.ChassisNo = srv.ChassisNo  
left join svTrnInvoice inv  
 on inv.CompanyCode = srv.CompanyCode  
  and inv.BranchCode = srv.BranchCode  
  and inv.ProductType = srv.ProductType  
  and inv.InvoiceNo = srv.InvoiceNo  
left join svTrnFakturPajak fpj  
 on fpj.CompanyCode = srv.CompanyCode  
  and fpj.BranchCode = srv.BranchCode  
  and fpj.FPJNo = inv.FPJNo  
where   
 srv.CompanyCode = @CompanyCode  
 and srv.BranchCode between @BranchFrom and @BranchTo  
 and srv.ProductType = @ProductType  
 --and srv.isLocked = 0  
 and job.GroupJobType = 'FSC'  
 and ((job.GroupJobType like @JobFSC and job.PdiFscSeq > 0 )  or (job.JobType like @JobPDI and job.PdiFscSeq=0))
 and convert(varchar, srv.JobOrderDate, 112) between convert(varchar, @PeriodFrom, 112) and convert(varchar, @PeriodTo , 112)  
 --and  not exists (  
 -- select 1   
 -- from svTrnPdiFscApplication   
 -- where CompanyCode=srv.CompanyCode  
 --  and (case when @IsCentralize = '0' then BranchCode end) = srv.BranchCode   
 --  and InvoiceNo=srv.JobOrderNo  
 --  and ProductType=srv.ProductType					
 and  not exists (  
  select 1   
  from svTrnPdiFscApplication   
  where CompanyCode=srv.CompanyCode  
   and BranchCode = (case when @IsCentralize = '0' then srv.BranchCode  else @BranchCode end)
   and BranchCodeInv=srv.BranchCode
   and InvoiceNo=srv.JobOrderNo  
   and ProductType=srv.ProductType  
 )--)
) #t1  
  
select   
row_number() over (order by #t1.BranchCode, #t1.JobOrderNo) No,  
* from #t1   
where ServiceStatus=5 ---service status hanya yang tutup SPK
-- in (5, 7, 9)  
order by BranchCode, JobOrderNo  
  
select * into #t2 from(  
select   
(row_number() over (order by BasicModel)) RecNo  
,BasicModel  
,PdiFscSeq  
,Count(BasicModel) RecCount  
,sum(PdiFscAmount) PdiFscAmount   
from #t1 where ServiceStatus =5    ---service status hanya yang tutup SPK
--in (5, 7, 9)  
group by BasicModel, PdiFscSeq) #t2     
  
select * from #t2 order by BasicModel  
  
select '' RecNo, 'Total' BasicModel, '' PdiFscSeq, sum(RecCount) RecCount, sum(PdiFscAmount) PdiFscAmount from #t2  
  
select   
 srv.BranchCode  
 , reffService.Description AS Status  
 , employee.EmployeeName   
 , srv.JobOrderNo  
 , srv.JobOrderDate  
 , srv.PoliceRegNo  
 , srv.BasicModel  
 , srv.JobType  
from #t1   
left join svTrnService srv    
on srv.CompanyCode = #t1.CompanyCode  
 and srv.BranchCode = #t1.BranchCode     
 and srv.ProductType = #t1.ProductType    
 and srv.JobOrderNo = #t1.JobOrderNo  
left join svMstRefferenceService reffService  
    on reffService.CompanyCode = srv.CompanyCode  
    and reffService.ProductType = srv.ProductType      
    and reffService.RefferenceCode = srv.ServiceStatus  
    and reffService.RefferenceType = 'SERVSTAS'  
left join gnMstEmployee employee  
    on employee.CompanyCode = srv.CompanyCode  
    and employee.BranchCode = srv.BranchCode  
 and employee.EmployeeID = srv.ForemanID  
where #t1.ServiceStatus < 5  
order by BranchCode, JobOrderNo  
  
drop table #t1  
drop table #t2
end


GO


