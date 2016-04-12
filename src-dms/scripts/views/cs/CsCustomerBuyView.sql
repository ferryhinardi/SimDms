
go
if object_id('CsCustomerBuyView') is not null
	drop view CsCustomerBuyView

go
create view CsCustomerBuyView        
as    
select a.CompanyCode     
     , b.CustomerCode     
     , (a.ChassisCode + convert(varchar, a.ChassisNo)) as Chassis     
     , (a.EngineCode + convert(varchar, a.EngineNo)) as Engine     
     , DeliveryDate = isnull((    
		  select top 1 y.DODate    
			from omTrSalesDODetail x    
			left join omTrSalesDO y    
			  on y.CompanyCode = x.CompanyCode    
			 and y.BranchCode = x.BranchCode    
			 and y.DONo = x.DONo    
		   where x.CompanyCode = a.CompanyCode    
			 and x.BranchCode = a.BranchCode    
			 and x.ChassisCode = a.ChassisCode    
			 and x.ChassisNo = a.ChassisNo     
       ), null)    
     , IsDeliveredA = convert(bit, 0)    
     , IsDeliveredB = convert(bit, 0)     
     , IsDeliveredC = convert(bit, 0)     
     , IsDeliveredD = convert(bit, 0)    
     , IsDeliveredE = convert(bit, 0)     
     , Comment = ''    
     , Additional = ''    
     , Status = 0    
     , e.SalesModelCode as CarType    
     , e.ColourCode as Color    
     , d.PoliceRegNo     
     , f.Salesman as SalesmanCode     
     , g.EmployeeName as SalesmanName    
     , e.BranchCode    
     , c.CustomerName    
     , left(c.Address1, 40) as Address    
     , c.PhoneNo    
     , '' as StatusInfo    
     , c.BirthDate
     , h.AddPhone1
     , h.AddPhone2
     , h.ReligionCode
  from omTrSalesInvoiceVin a    
  left join omTrSalesInvoice b    
    on b.CompanyCode = a.CompanyCode    
   and b.BranchCode = a.BranchCode    
   and b.InvoiceNo = a.InvoiceNo    
  left join gnMstCustomer c    
    on c.CompanyCode = a.CompanyCode    
   and c.CustomerCode = b.CustomerCode    
  left join svMstCustomerVehicle d    
    on d.CompanyCode = a.CompanyCode    
   and d.ChassisCode = a.ChassisCode    
   and d.ChassisNo = a.ChassisNo    
  left join omTrSalesSOVin e    
    on e.CompanyCode = a.CompanyCode    
   and e.ChassisCode = a.ChassisCode    
   and e.ChassisNo = a.ChassisNo    
  left join omTrSalesSO f    
    on f.CompanyCode = e.CompanyCode    
   and f.BranchCode = e.BranchCode    
   and f.SONo = e.SONo    
  left join gnMstEmployee g    
    on g.CompanyCode = f.CompanyCode    
   and g.BranchCode = f.BranchCode    
   and g.EmployeeID = f.Salesman    
  left join CsCustData h
    on h.CompanyCode = c.CompanyCode    
   and h.CustomerCode = c.CustomerCode
 where 1 = 1
   and exists ( 
select top 1 y.DODate    
  from omTrSalesDODetail x    
  left join omTrSalesDO y    
    on y.CompanyCode = x.CompanyCode    
   and y.BranchCode = x.BranchCode    
   and y.DONo = x.DONo    
 where x.CompanyCode = a.CompanyCode    
   and x.BranchCode = a.BranchCode    
   and x.ChassisCode = a.ChassisCode    
   and x.ChassisNo = a.ChassisNo  
   and y.DODate is not null
 ) 
   and not exists (    
select 1 from CsTDayCall    
 where CompanyCode = a.CompanyCode    
   and Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)       
 ) 
   