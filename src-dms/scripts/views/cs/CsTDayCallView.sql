
go
if object_id('CsTDayCallView') is not null
	drop view CsTDayCallView

go
create view CsTDayCallView
as 
select a.CompanyCode 
     , a.CustomerCode 
     , a.Chassis 
     , a.IsDeliveredA 
     , a.IsDeliveredB 
     , a.IsDeliveredC 
     , a.IsDeliveredD 
     , a.IsDeliveredE 
     , a.IsDeliveredF
     , a.IsDeliveredG 
     , a.Comment 
     , a.Additional 
     , a.Status 
     , (case a.Status when 0 then 'In Progress' else 'Finish' end) as StatusInfo
     , a.CreatedDate as InputDate
     , b.CustomerName
     , left(b.Address1, 40) as Address
     , b.PhoneNo
     , c.BranchCode
     , c.EngineCode + convert(varchar, c.EngineNo) as Engine 
     , c.SalesModelCode as CarType
     , c.ColourCode as Color
     , d.Salesman as SalesmanCode
     , e.EmployeeName as SalesmanName
     , f.PoliceRegNo 
     , a.FinishDate
     , b.BirthDate
     , g.AddPhone1
     , g.AddPhone2
     , g.ReligionCode
  from CsTDayCall a
  left join gnMstCustomer b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
  left join omTrSalesSOVin c
    on c.CompanyCode = a.CompanyCode
   and (c.ChassisCode + convert(varchar, c.ChassisNo)) = a.Chassis
  left join omTrSalesSO d
    on d.CompanyCode = c.CompanyCode
   and d.BranchCode = c.BranchCode
   and d.SONo = c.SONo
  left join gnMstEmployee e
    on e.CompanyCode = d.CompanyCode
   and e.BranchCode = d.BranchCode
   and e.EmployeeID = d.Salesman
  left join svMstCustomerVehicle f
    on f.CompanyCode = a.CompanyCode
   and f.ChassisCode + convert(varchar, f.ChassisNo) = a.Chassis
  left join CsCustData g
    on g.CompanyCode = b.CompanyCode    
   and g.CustomerCode = b.CustomerCode



