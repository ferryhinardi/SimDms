USE [SAT_UAT]
GO

/****** Object:  StoredProcedure [dbo].[uspfn_GetVehicleInfo_New]    Script Date: 04/25/2015 10:58:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO





ALTER procedure [dbo].[uspfn_GetVehicleInfo_New]  
 @CompanyCode  varchar(20),  
 @BranchCode   varchar(20),  
 @ProductType  varchar(10),  
 @PoliceRegNo  varchar(20),  
 @ChassisCode  varchar(20),  
 @ChassisNo    varchar(10),  
 @BasicModel   varchar(20),  
 @JobOrderDate varchar(20),
 @CustomerCode varchar(20),
 @IsAllBranch  bit = 1
 
as  
  
select * into #t1 from (  
select 0 TaskPartSeq  
     , a.BranchCode  
     , a.JobOrderNo  
     , a.JobOrderDate  
     , d.InvoiceNo  
     , d.InvoiceDate  
     , d.FPJNo  
     , d.FPJDate  
     , a.JobType + ' - ' + e.Description JobType  
     , a.Odometer  
     , c.MechanicId+' - '+  
  (  
  select EmployeeName   
  from gnMstEmployee   
  where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and EmployeeID=c.MechanicId  
  ) MechanicId  
     , a.ForemanId  
     , b.OperationNo  
     , isnull(b.OperationHour, 0) as OperationQty  
     , convert(decimal, isnull(b.OperationCost, 0) * isnull(b.OperationHour, 0) * 1.0) as OperationAmt  
     , convert(decimal, isnull(b.OperationCost, 0) * isnull(b.OperationHour, 0) * 1.1) as TotalSrvAmount  
     , isnull(b.SharingTask, 0) SharingTask  
     , case when f.Description is null then (select TOP 1 Description   
    from svMstTask   
     where BasicModel = a.BasicModel  
      and OperationNo = b.OperationNo) else f.Description end Description  
    , isnull(g.EmployeeName, '') NameSA  
    , isnull(h.EmployeeName, '') NameForeman  
    , b.CreatedDate
    , d.Remarks
 from svTrnService a with(nolock, nowait)  
 left join svTrnSrvTask b with(nolock, nowait)  
   on b.CompanyCode = a.CompanyCode  
  and b.BranchCode  = a.BranchCode  
  and b.ProductType = a.ProductType  
  and b.ServiceNo   = a.ServiceNo  
 left join svTrnSrvMechanic c with(nolock, nowait)  
   on c.CompanyCode = b.CompanyCode  
  and c.BranchCode  = b.BranchCode  
  and c.ProductType = b.ProductType  
  and c.ServiceNo   = b.ServiceNo  
  and c.OperationNo = b.OperationNo  
 left join svTrnInvoice d with(nolock, nowait)  
   on d.CompanyCode = a.CompanyCode  
  and d.BranchCode  = a.BranchCode  
  and d.InvoiceNo   = a.InvoiceNo  
 left join svMstRefferenceService e with(nolock, nowait)  
   on e.CompanyCode    = a.CompanyCode  
  and e.RefferenceCode = a.jobType  
  and e.ProductType    = a.ProductType  
  and e.RefferenceType = 'JOBSTYPE'  
 left join svMstTask f with(nolock, nowait)  
   on f.CompanyCode = b.CompanyCode  
  and f.ProductType = b.ProductType  
  and f.OperationNo = b.OperationNo   
  and f.JobType     = a.JobType  
  and f.BasicModel  = a.BasicModel  
 left join gnMstEmployee g on g.CompanyCode = a.CompanyCode  
  and g.BranchCode = a.BranchCode   
  and g.EmployeeId = a.ForemanID  
 left join gnMstEmployee h on h.CompanyCode = a.CompanyCode  
  and h.BranchCode = a.BranchCode   
  and h.EmployeeId = a.MechanicID  
where a.JobOrderNo <> ''  
  and a.CompanyCode = @CompanyCode  
  and a.BranchCode  = @BranchCode  
  and a.ProductType = @ProductType  
  and a.ChassisCode = @ChassisCode  
  and a.ChassisNo   = @ChassisNo  
  and convert(varchar, a.JobOrderDate, 112) >= @JobOrderDate  
  and a.CustomerCode = @CustomerCode
) #t1  
  
--declare @t_spk_task as table(JobOrderNo varchar(20), OperationNo varchar(20))  
  
--insert into @t_spk_task   
--select a.JobOrderNo  
--  , isnull((  
--  select top 1 OperationNo from #t1  
--      where JobOrderNo = a.JobOrderNo  
--      order by CreatedDate   
--     ), '') OperationNo  
--  from #t1 a group by a.JobOrderNo  
  
insert into #t1  
select 1 TaskPartSeq  
     , a.BranchCode  
     , a.JobOrderNo  
     , a.JobOrderDate  
     , d.InvoiceNo  
     , d.InvoiceDate  
     , d.FPJNo  
     , d.FPJDate  
     , a.JobType + ' - ' + e.Description JobType  
     , a.Odometer  
     , ''  
     , ''  
     , b.PartNo  
     , (isnull(b.DemandQty,0) - isnull(b.ReturnQty,0)) as OperationQty  
     , convert(int, (isnull(b.DemandQty,0) - isnull(b.ReturnQty,0)) * isnull(b.RetailPrice,0) * 1.0) as OperationAmt  
     , convert(int, (isnull(b.DemandQty,0) - isnull(b.ReturnQty,0)) * isnull(b.RetailPrice,0) * 1.1)
     , 1  
     , f.PartName  
     , isnull(g.EmployeeName, '') NameSA  
     , isnull(h.EmployeeName, '') NameForeman  
     , b.CreatedDate
     , d.Remarks 
  from svTrnService a with(nolock, nowait)  
  left join svTrnSrvItem b with(nolock, nowait)  
    on b.CompanyCode = a.CompanyCode  
   and b.BranchCode  = a.BranchCode  
   and b.ProductType = a.ProductType  
   and b.ServiceNo   = a.ServiceNo  
  left join svTrnInvoice d with(nolock, nowait)  
    on d.CompanyCode = a.CompanyCode  
   and d.BranchCode  = a.BranchCode  
   and d.InvoiceNo   = a.InvoiceNo  
  left join svMstRefferenceService e with(nolock, nowait)  
    on e.CompanyCode    = a.CompanyCode  
   and e.RefferenceCode = a.jobType  
   and e.ProductType    = a.ProductType  
   and e.RefferenceType = 'JOBSTYPE'  
  left join spMstItemInfo f with(nolock, nowait)  
    on f.CompanyCode = b.CompanyCode  
   and f.PartNo      = b.PartNo   
  left join gnMstEmployee g on g.CompanyCode = a.CompanyCode  
   and g.BranchCode = a.BranchCode   
   and g.EmployeeId = a.ForemanID  
  left join gnMstEmployee h on h.CompanyCode = a.CompanyCode  
   and h.BranchCode = a.BranchCode   
   and h.EmployeeId = a.MechanicID  
 where a.JobOrderNo <> ''  
   and a.CompanyCode = @CompanyCode  
   and a.BranchCode  = @BranchCode  
   and a.ProductType = @ProductType  
   and a.ChassisCode = @ChassisCode  
   and a.ChassisNo   = @ChassisNo  
   and convert(varchar, a.JobOrderDate, 112) >= @JobOrderDate  
   and a.CustomerCode = @CustomerCode
;with x as (  
select a.JobOrderNo, a.OperationNo, a.TotalSrvAmount, b.TotalSrvAmount TotalSrvAmountNew
  from #t1 a  
  left join svTrnService b  
    on b.CompanyCode = @CompanyCode  
   and b.BranchCode  = @BranchCode  
   and b.ProductType = @ProductType  
   and b.JobOrderNo  = a.JobOrderNo  
)  
update x set TotalSrvAmount = TotalSrvAmountNew  

select a.BranchCode  
     , a.JobOrderNo  
     , SUM(a.OperationAmt)  TotalSrvAmount
     , (CAST(a.Remarks AS varchar(max)))
from #t1 a
where OperationQty > 0
group BY a.BranchCode, a.JobOrderNo, TotalSrvAmount, (CAST(a.Remarks AS varchar(max)))
order by a.JobOrderNo desc
  
drop table #t1  


GO


