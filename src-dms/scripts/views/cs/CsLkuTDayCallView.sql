
go 
if object_id('CsLkuTDayCallView') is not null	
	drop view CsLkuTDayCallView;

go
create view CsLkuTDayCallView
as
select a.CompanyCode
     , b.BranchCode
     , a.CustomerCode
     , Address = rtrim(a.Address1) + ' ' + rtrim(a.Address2) + ' ' + rtrim(a.Address3) + ' ' + rtrim(a.Address4)
	 , PhoneNo = a.PhoneNo
     , (d.ChassisCode + convert(varchar, d.ChassisNo)) as Chassis
	 , (d.EngineCode + convert(varchar, d.EngineNo)) as Engine
	 , b.SONo
     , c.DONo
	 , g.BpkNo
     , d.SalesModelCode as CarType
     , d.ColourCode as Color
	 , b.Salesman as SalesmanCode
	 , f.EmployeeName as SalesmanName
     , PoliceRegNo = e.PoliceRegNo
     , c.DODate as DeliveryDate
	 , d.SalesModelCode
	 , d.SalesModelYear
	 , d.ColourCode
	 , g.BPKDate
	 , b.IsLeasing
	 , b.LeasingCo
	 , h.CustomerName as LeasingName
	 , b.Installment
  from GnMstCustomer a
 inner join omTrSalesSO b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
  left join omTrSalesDO c
    on c.CompanyCode = b.CompanyCode
   and c.CustomerCode = b.CustomerCode
   and c.SONo = b.SONo
  left join omTrSalesDODetail d
    on d.CompanyCode = c.CompanyCode
   and d.BranchCode = c.BranchCode
   and d.DONo = c.DONo
  left join svMstCustomerVehicle e
    on e.CompanyCode = d.CompanyCode
   and e.ChassisCode = d.ChassisCode
   and e.ChassisNo = d.ChassisNo
  left join HrEmployee f
    on f.CompanyCode = b.CompanyCode
   and f.EmployeeID = b.Salesman
  left join OmTrSalesBpk g
    on g.CompanyCode = c.CompanyCode
   and g.BranchCode = c.BranchCode
   and g.DONo = c.DONo
   and g.SONo = c.SONo
  left join gnMstCustomer h
    on h.CompanyCode = b.CompanyCode
   and h.CustomerCode = b.LeasingCo
 where 1 = 1
   and d.ChassisCode is not null
   and d.EngineCode is not null
   and b.SODate is not null
   and c.DODate is not null
   and g.BpkNo is not null
   and isnull(d.StatusBPK, 3) != '3'
   and isnull(g.Status, 3) != '3'
   and year(c.DODate) = year(getdate())
