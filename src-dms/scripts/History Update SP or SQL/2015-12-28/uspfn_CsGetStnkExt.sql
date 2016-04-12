
CREATE procedure [dbo].[uspfn_CsGetStnkExt]
	@CompanyCode  varchar(20),
	@CustomerCode varchar(20),
	@Chassis      varchar(50)
as

--select @CompanyCode = '6006406', @CustomerCode = '1000581', @Chassis = 'MHYGDN42VBJ352996'
select a.CompanyCode     
	 , a.BranchCode
	 , b.CustomerCode
	 , c.CustomerName
	 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
	 , a.EngineCode + convert(varchar, a.EngineNo) Engine
	 , a.SalesModelCode
	 , SalesModel = a.SalesModelCode + '-' + (
					select SalesModelDesc
					  from omMstModel
					 where CompanyCode = a.CompanyCode
					   and SalesModelCode = a.SalesModelCode
	   )
	 , a.SalesModelYear
	 , a.ColourCode
	 , Colour = a.ColourCode + '-' + (
					select RefferenceDesc1 
					 from omMstRefference 
					where CompanyCode = a.CompanyCode
					  and RefferenceCode = a.ColourCode
					  and RefferenceType = 'COLO'
	   )
	 , isnull(k.BpkbDate,  g.BPKDate) BpkbDate
     , case when m.STNKInDate = '1900-01-01 00:00:00.000' then null else m.STNKInDate end StnkDate
	 , g.BPKDate
	 , h.isLeasing as IsLeasing
	 , h.LeasingCo
	 , isnull(h.isLeasing, 0) as IsLeasing
	 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
	 , i.CustomerName as LeasingName
	 , h.Installment
	 , convert(varchar, isnull(h.Installment, 0)) + ' Month' as Tenor
     , left(c.Address1, 40) as Address
     , c.PhoneNo
     , h.Salesman
     , j.EmployeeName as SalesmanName
     , k.IsStnkExtend
	 , k.Ownership
     , isnull(k.StnkExpiredDate, case when k.StnkDate is not null then dateadd(year, 1, k.StnkDate) else '' end)  as StnkExpiredDate
     , k.ReqKtp
     , k.ReqStnk
     , k.ReqBpkb
     , k.ReqSuratKuasa
     , k.Comment, k.Additional, k.Status
     , (case k.Status when 1 then 'Finish' else 'In Progress' end) as StatusInfo
     , case (isnull(k.Chassis, '')) when '' then 1 else 0 end as IsNew
     , l.PoliceRegNo
	 , k.Ownership
	 , k.Reason
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
 -- left join CsCustomerVehicle e
	--on e.CompanyCode = a.CompanyCode    
 --  and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
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
  left join HrEmployee j
	on j.CompanyCode = h.CompanyCode
   and j.EmployeeID = h.Salesman
  left join CsStnkExt k
	on k.CompanyCode = a.CompanyCode
   and k.CustomerCode = b.CustomerCode
   and k.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join svMstCustomerVehicle l
	on l.CompanyCode = a.CompanyCode
   and l.ChassisCode = a.ChassisCode
   and l.ChassisNo = a.ChassisNo
  left join
	 ( 
	   select distinct 
	     CompanyCode
	   , BranchCode
	   , ChassisCode
	   , ChassisNo
	   , STNKInDate 
	   from omTrSalesSPKDetail
	 ) m
    on m.CompanyCode = a.CompanyCode
   and m.BranchCode = a.BranchCode
   and m.ChassisCode = a.ChassisCode
   and m.ChassisNo = a.ChassisNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and b.CustomerCode = @CustomerCode
   and a.ChassisCode + convert(varchar, a.ChassisNo) = @Chassis

