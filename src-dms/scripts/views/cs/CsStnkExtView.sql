
go
if object_id('CsStnkExtView') is not null
	drop view CsStnkExtView

go
create view CsStnkExtView        
as        
select distinct a.CompanyCode           
     , b.CustomerCode
     , e.BranchCode          
     , (a.ChassisCode + convert(varchar, a.ChassisNo)) as Chassis           
     , (a.EngineCode + convert(varchar, a.EngineNo)) as Engine    
     , e.SalesModelCode as CarType          
     , e.ColourCode as Color          
     , d.PoliceRegNo           
     , f.Salesman as SalesmanCode           
     , g.EmployeeName as SalesmanName          
     , c.CustomerName          
     , c.PhoneNo
     , i.CompanyName CompanyName  
     , j.CompanyName BranchName
     , h.StnkExpiredDate
     , k.BpkbDate
	 , k.StnkDate
     , (case h.Status when 0 then 'In Progress' when 1 then 'Finish' else 'In Progress' end) as StatusInfo        
     , left(c.Address1, 40) as Address  
     , h.IsStnkExtend
     , h.ReqKtp
     , h.ReqSuratKuasa
     , h.ReqBpkb
     , h.ReqStnk
     , h.Comment
     , h.Additional
     , h.Status
     , h.Tenor
     , h.LeasingCode
     , h.CustomerCategory
     , convert(varchar(15),h.CreatedDate, 106) as CreatedDate
     , convert(varchar(15),h.FinishDate, 106) as FinishDate      
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
  left join CsStnkExt h        
    on h.CompanyCode = a.CompanyCode        
   and h.Chassis = a.ChassisCode + convert(varchar(20), a.ChassisNo)      
  left join gnMstOrganizationHdr i  
    on i.CompanyCode = f.CompanyCode  
  left join gnMstCoProfile j  
    on j.CompanyCode = f.CompanyCode  
   and j.BranchCode = f.BranchCode 
  left join CsCustomerVehicle k  
    on k.CompanyCode = a.CompanyCode  
   and k.Chassis = a.ChassisCode + convert(varchar(20), a.ChassisNo) 
   