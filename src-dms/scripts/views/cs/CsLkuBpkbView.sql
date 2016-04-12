
go
if object_id('CsLkuBpkbViewSource') is not null
	drop view CsLkuBpkbViewSource

go
create view CsLkuBpkbViewSource
as 

select a.CompanyCode     
	 , a.BranchCode
	 , b.CustomerCode
	 , c.CustomerName
	 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
	 , a.EngineCode + convert(varchar, a.EngineNo) Engine
	 , a.SalesModelCode
	 , a.SalesModelYear
	 , isnull(e.BpkbDate, g.BPKDate) BpkbDate
	 , isnull(e.StnkDate, g.BPKDate) StnkDate
	 , g.BPKDate
	 , isnull(h.isLeasing, 0) as IsLeasing
	 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
	 , h.LeasingCo
	 , i.CustomerName as LeasingName
	 , h.Installment
	 , PoliceRegNo = isnull(l.PoliceRegNo, j.PoliceRegNo)
     --, case isnull(k.Chassis, '') when '' then 'Y' else 'N' end OutStanding
	 , OutStanding = (
			case
				when isnull(k.Chassis, '') = '' then 'Y'
				when isnull(k.BpkbReadyDate, '') = '' then 'Y'
				when isnull(isnull(k.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=k.CustomerCode and (x.IsDeleted = 0 or x.IsDeleted is null) order by x.RetrievalEstimationDate desc)), '') = '' then 'Y'
				when isnull(isnull(k.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=k.CustomerCode order by x.RetrievalEstimationDate desc)), '') != '' and isnull(k.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=k.CustomerCode and (x.IsDeleted = 0 or x.IsDeleted is null) order by x.RetrievalEstimationDate desc)) < getdate() then 'Y'
				when isnull(k.BpkbReadyDate, '') != '' and isnull(isnull(k.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=k.CustomerCode and (x.IsDeleted = 0 or x.IsDeleted is null)order by x.RetrievalEstimationDate desc)), '') = '' then 'Y'
				when (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=k.CustomerCode order by x.RetrievalEstimationDate desc) > getdate() then 'N'
				else 'N'
			end
	   )
	 , DelayedRetrievalDate = (
			select top 1
			       x.RetrievalEstimationDate
			  from CsBpkbRetrievalInformation x
			 where x.CompanyCode = b.CompanyCode
			   and x.CustomerCode = b.CustomerCode
			 order by x.RetrievalEstimationDate desc
	   )
	 , DelayedRetrievalNote = (
			select top 1
			       x.Notes
			  from CsBpkbRetrievalInformation x
			 where x.CompanyCode = b.CompanyCode
			   and x.CustomerCode = b.CustomerCode
			 order by x.RetrievalEstimationDate desc
	   )
  from omTrSalesInvoiceVin a    
  left join omTrSalesInvoice b    
	on b.CompanyCode = a.CompanyCode    
   and b.BranchCode = a.BranchCode    
   and b.InvoiceNo = a.InvoiceNo    
  left join gnMstCustomer c
	on c.CompanyCode = a.CompanyCode    
   and c.CustomerCode = b.CustomerCode
  left join omTrSalesDODetail d
	on d.CompanyCode = a.CompanyCode    
   and d.BranchCode = a.BranchCode
   and d.ChassisCode = a.ChassisCode
   and d.ChassisNo = a.ChassisNo
  left join CsCustomerVehicle e
	on e.CompanyCode = a.CompanyCode    
   and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join omTrSalesDO f
	on f.CompanyCode = d.CompanyCode    
   and f.BranchCode = d.BranchCode
   and f.DONo = d.DONo
  left join omTrSalesBPK g
	on g.CompanyCode = f.CompanyCode    
   and g.BranchCode = f.BranchCode
   and g.DONo = f.DONo
  left join omTrSalesSO h
	on h.CompanyCode = g.CompanyCode    
   and h.BranchCode = g.BranchCode
   and h.SONo = g.SONo
  left join gnMstCustomer i
	on i.CompanyCode = h.CompanyCode
   and i.CustomerCode = h.LeasingCo
  left join svMstCustomerVehicle j
	on j.CompanyCode = a.CompanyCode
   and j.ChassisCode = a.ChassisCode
   and j.ChassisNo = a.ChassisNo
  left join CsCustBpkb k
	on k.CompanyCode = a.CompanyCode
   and k.CustomerCode = b.CustomerCode
   and k.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join svMstCustomerVehicle l
	on l.CompanyCode = a.CompanyCode
   and l.ChassisCode = a.ChassisCode
   and l.ChassisNo = a.ChassisNo
 where 1 = 1
   and isnull(a.IsReturn, 0) = 0
   and c.CustomerType = 'I'



go
select * from CsLkuBpkbViewSource
 --where OutStanding = 'N'
