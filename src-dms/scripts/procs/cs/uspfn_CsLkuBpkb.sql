
go
if object_id('uspfn_CsLkuBpkb') is not null
	drop procedure uspfn_CsLkuBpkb

go
create procedure uspfn_CsLkuBpkb
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@OutStanding bit
as

declare @IsHolding bit
set @IsHolding = isnull((select top 1 1 from gnMstOrganizationDtl where CompanyCode = @CompanyCode and BranchCode = @BranchCode and IsBranch = 0), 0)

if @OutStanding = 1 
begin
	select top 5000 a.CompanyCode     
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
		 , PoliceRegNo = isnull(k.PoliceRegistrationNo, j.PoliceRegNo)
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
	  left join omTrSalesBPKBDetail k
		on k.CompanyCode = a.CompanyCode
	   and k.ChassisCode = a.ChassisCode
	   and k.ChassisNo = a.ChassisNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = (case @IsHolding when 1 then a.BranchCode else @BranchCode end)
	   and isnull(a.IsReturn, 0) = 0
	   and not exists (
			select 1 from CsCustBpkb 
			 where CompanyCode = a.CompanyCode
			   and CustomerCode = b.CustomerCode
			   and Chassis = a.ChassisCode + convert(varchar, a.ChassisNo))
end   
else
begin
	select top 5000 a.CompanyCode     
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
		 , PoliceRegNo = isnull(k.PoliceRegistrationNo, j.PoliceRegNo)
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
	  left join omTrSalesBPKBDetail k
		on k.CompanyCode = a.CompanyCode
	   and k.ChassisCode = a.ChassisCode
	   and k.ChassisNo = a.ChassisNo
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode = (case @IsHolding when 1 then a.BranchCode else @BranchCode end)
	   and isnull(a.IsReturn, 0) = 0
	   and exists (
			select 1 from CsCustBpkb 
			 where CompanyCode = a.CompanyCode
			   and CustomerCode = b.CustomerCode
			   and Chassis = a.ChassisCode + convert(varchar, a.ChassisNo))
end   
