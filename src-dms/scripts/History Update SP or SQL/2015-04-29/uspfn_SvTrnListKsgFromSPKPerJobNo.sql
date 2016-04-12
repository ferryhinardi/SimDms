if object_id('uspfn_SvTrnListKsgFromSPKPerJobNo') is not null
	drop PROCEDURE uspfn_SvTrnListKsgFromSPKPerJobNo
GO
CREATE procedure [dbo].[uspfn_SvTrnListKsgFromSPKPerJobNo]
 @CompanyCode varchar(15),  
 @BranchCode varchar(max),
 @ProductType varchar(15),   
 @JobOrder varchar(max)
as        
  
declare @sql varchar(max);
 
set @sql =   
'select * into #t1 from(  
select  
    convert(bit, 1) Process      
 , srv.BranchCode  
 , srv.JobOrderNo  
 , case when convert(varchar, srv.JobOrderDate, 106) = ''19000101'' then '''' else convert(varchar, srv.JobOrderDate, 106) end JobOrderDate  
 , srv.BasicModel  
 , srv.ServiceBookNo  
 , job.PdiFscSeq  
 , srv.Odometer  
 , srv.LaborGrossAmt  
 , round((select isnull(SUM((SupplyQty - ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = ''F''),0) MaterialGrossAmt --Pembulatan
 , round((srv.LaborGrossAmt + (select isnull(SUM((SupplyQty-ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = ''F'')),0) PdiFscAmount  --Pembulatan
 , isnull(case when convert(varchar, veh.FakturPolisiDate, 112) = ''19000101'' then '''' else convert(varchar, veh.FakturPolisiDate, 106) end, '''')  FakturPolisiDate  
 , isnull(case when convert(varchar, mstVeh.BPKDate, 112) = ''19000101'' then '''' else convert(varchar, mstVeh.BPKDate, 106) end, '''')  BPKDate  
 , srv.ChassisCode  
 , srv.ChassisNo  
 , srv.EngineCode  
 , srv.EngineNo   
    , srv.InvoiceNo  
 , isnull(inv.FPJNo, '''') FPJNo  
 , isnull(case when convert(varchar, inv.FPJDate, 112) = ''19000101'' then '''' else convert(varchar, inv.FPJDate, 106) end, '''')  FPJDate  
 , isnull(fpj.FPJGovNo, '''') FPJGovNo  
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
 srv.CompanyCode =''' + @CompanyCode  + '''
 and srv.ProductType = ''' + @ProductType  + '''
 and job.GroupJobType = ''FSC''  
 and not exists (  
  select 1   
  from svTrnPdiFscApplication   
  where CompanyCode=srv.CompanyCode  
   and BranchCode=srv.BranchCode   
   and InvoiceNo=srv.JobOrderNo  
   and ProductType=srv.ProductType  
 ) and  not exists (  
  select 1   
  from svTrnPdiFscApplication   
  where CompanyCode=srv.CompanyCode  
   and BranchCode in (' + @BranchCode+ ')
   and InvoiceNo=srv.JobOrderNo  
   and ProductType=srv.ProductType  
 )
 and srv.JobOrderNo in (' + @JobOrder + ')
) #t1  
  
select   
row_number() over (order by #t1.BranchCode, #t1.JobOrderNo) No,  
* from #t1   
where ServiceStatus=5 ---service status hanya yang tutup SPK
-- in (5, 7, 9)  
order by BranchCode, JobOrderNo  
  
drop table #t1';

exec (@sql);