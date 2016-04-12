alter procedure uspfn_SyncSvRegisterSpk
	@YearReff  as int = 2014,
	@MonthBack as int = 1
as

;with x as (
select a.CompanyCode
     , a.BranchCode
	 , a.ProductType
	 , a.ServiceNo
	 , a.JobType
	 , b.OperationNo
	 , c.Description as OperationName
	 , b.OperationHour
	 , a.MechanicID as FmID
	 , a.ForemanID as SaID 
	 , MechanicID = isnull((select top 1 x.MechanicID
					    from svTrnSrvMechanic x
					   where x.CompanyCode = a.CompanyCode
					     and x.BranchCode = a.BranchCode
						 and x.ProductType = a.ProductType
						 and x.ServiceNo = a.ServiceNo
						 and x.OperationNo = b.OperationNo
					   ), '')
	 , '' as PartNo
	 , '' as PartName
	 , 0 as PartSeq
	 , 0 as DemandQty
	 , 0 as SupplyQty
	 , 0 as ReturnQty
	 , '' as SupplySlipNo
	 , '' as SSReturnNo
	 , 'T' as TaskPartType
  from SvTrnService a
 inner join SvTrnSrvTask b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.ProductType = a.ProductType
   and b.ServiceNo = a.ServiceNo
  left join SvMstTask c
    on c.CompanyCode = a.CompanyCode
   and c.ProductType = a.ProductType
   and c.BasicModel = a.BasicModel
   and c.JobType = (case when a.JobType in ('CLAIM', 'OTHER') then c.JobType else a.JobType end)
   and c.OperationNo = b.OperationNo
 where a.ServiceType = '2'
   and left(a.JobOrderNo, 3) = 'SPK'
   and year(a.JobOrderDate) >= @YearReff
   and a.LastUpdateDate >= dateadd(month, -@MonthBack, getdate()) 

 union all

select a.CompanyCode
     , a.BranchCode
	 , a.ProductType
	 , a.ServiceNo
	 , '' as JobType
	 , '' as OperationNo
	 , '' as OperationName
	 , 0 as OperationHour
	 , a.MechanicID as FmID
	 , a.ForemanID as SaID
	 , '' as MechanicID 
	 , b.PartNo
	 , c.PartName
	 , b.PartSeq
	 , b.DemandQty
	 , b.SupplyQty
	 , b.ReturnQty
	 , b.SupplySlipNo
	 , b.SSReturnNo
	 , 'P' as TaskPartType
  from SvTrnService a
 inner join svTrnSrvItem b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.ProductType = a.ProductType
   and b.ServiceNo = a.ServiceNo
  left join spMstItemInfo c
    on c.CompanyCode = a.CompanyCode
   and c.PartNo = b.PartNo
 where a.ServiceType = '2'
   and left(a.JobOrderNo, 3) = 'SPK'
   and year(a.JobOrderDate) >= @YearReff
   and a.LastUpdateDate >= dateadd(month, -@MonthBack, getdate()) 
)
,y as (
select x.CompanyCode
     , x.BranchCode
	 , x.ProductType
	 , x.ServiceNo
	 , a.JobOrderNo
	 , a.JobOrderDate
	 , TaskPartNo = (case x.TaskPartType when 'T' then x.OperationNo when 'P' then x.PartNo else '' end)
	 , x.PartSeq as TaskPartSeq
	 , a.BasicModel
	 , a.PoliceRegNo
	 , a.Odometer
	 , a.CustomerCode
	 , b.CustomerName
	 , c.GroupJobType
	 , e.Description as GroupJobTypeDesc
	 , x.JobType
	 , d.Description as JobTypeDesc
	 , x.OperationNo
	 , x.OperationName
	 , x.OperationHour
	 , x.FmID
	 , g.EmployeeName as FmName
	 , x.SaID
	 , h.EmployeeName as SaName
	 , x.MechanicID
	 , i.EmployeeName as MechanicName
	 , x.PartNo
	 , x.PartName
	 , x.DemandQty
	 , x.SupplyQty
	 , x.ReturnQty
	 , x.SupplySlipNo
	 , x.SSReturnNo
	 , x.TaskPartType
	 , a.ServiceRequestDesc
	 , a.ServiceStatus
	 , case a.ServiceStatus
		when '0' then 'Open Job Order'    
		when '1' then 'Progress Service'    
		when '2' then 'Test Drive'    
		when '3' then 'Final Check'    
		when '4' then 'Washing'    
		when '5' then 'Closed SPK'    
		when '6' then 'Cancel SPK'    
		when '7' then 'Invoice'    
		when '8' then 'Cancel Invoice'    
		when '9' then 'Faktur Pajak'    
		when 'A' then 'Cancel Faktur Pajak'    
		when 'B' then 'Faktur Pajak Return'   
	   end as ServiceStatusDesc
	 , a.TotalSrvAmount
	 , a.InvoiceNo
	 , a.CreatedBy
	 , a.CreatedDate
	 , a.LastUpdateBy
	 , a.LastUpdateDate
  from x
  left join SvTrnService a
    on a.CompanyCode = x.CompanyCode
   and a.BranchCode = x.BranchCode
   and a.ProductType = x.ProductType
   and a.ServiceNo = x.ServiceNo
  left join GnMstCustomer b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
  left join SvMstJob c
    on c.CompanyCode = a.CompanyCode
   and c.ProductType = a.ProductType
   and c.BasicModel = a.BasicModel
   and c.JobType = a.JobType
  left join SvMstRefferenceService d
    on d.CompanyCode = a.CompanyCode
   and d.ProductType = a.ProductType
   and d.RefferenceType = 'JOBSTYPE'
   and d.RefferenceCode = a.JobType
  left join SvMstRefferenceService e
    on e.CompanyCode = a.CompanyCode
   and e.ProductType = a.ProductType
   and e.RefferenceType = 'GRPJOBTY'
   and e.RefferenceCode = c.GroupJobType
  left join GnMstEmployee g
    on g.CompanyCode = a.CompanyCode
   and g.BranchCode = a.BranchCode
   and g.EmployeeID = x.FmID
  left join GnMstEmployee h
    on h.CompanyCode = a.CompanyCode
   and h.BranchCode = a.BranchCode
   and h.EmployeeID = x.SaID
  left join GnMstEmployee i
    on i.CompanyCode = a.CompanyCode
   and i.BranchCode = a.BranchCode
   and i.EmployeeID = x.MechanicID
)
select * into #t1 from y

delete SvRegisterSpk where exists 
  (
    select 1 from #t1
	 where #t1.CompanyCode = SvRegisterSpk.CompanyCode
	   and #t1.BranchCode = SvRegisterSpk.BranchCode
	   and #t1.ServiceNo = SvRegisterSpk.ServiceNo
  ) 

insert into SvRegisterSpk (CompanyCode, BranchCode, ProductType, ServiceNo, JobOrderNo, JobOrderDate, TaskPartNo, TaskPartSeq, BasicModel, PoliceRegNo, Odometer, CustomerCode, CustomerName, GroupJobType, GroupJobTypeDesc, JobType, JobTypeDesc, OperationNo, OperationName, OperationHour, FmID, FmName, SaID, SaName, MechanicID, MechanicName, PartNo, PartName, DemandQty, SupplyQty, ReturnQty, SupplySlipNo, SSReturnNo, TaskPartType, ServiceRequestDesc, ServiceStatus, ServiceStatusDesc, TotalSrvAmount, InvoiceNo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
select CompanyCode, BranchCode, ProductType, ServiceNo, JobOrderNo, JobOrderDate, TaskPartNo, TaskPartSeq, BasicModel, PoliceRegNo, Odometer, CustomerCode, CustomerName, GroupJobType, GroupJobTypeDesc, JobType, JobTypeDesc, OperationNo, OperationName, OperationHour, FmID, FmName, SaID, SaName, MechanicID, MechanicName, PartNo, PartName, DemandQty, SupplyQty, ReturnQty, SupplySlipNo, SSReturnNo, TaskPartType, ServiceRequestDesc, ServiceStatus, ServiceStatusDesc, TotalSrvAmount, InvoiceNo, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate from #t1

go

uspfn_SyncSvRegisterSpk 2014, 5