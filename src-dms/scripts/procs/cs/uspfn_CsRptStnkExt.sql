
go
if object_id('uspfn_CsRptStnkExt') is not null
	drop procedure uspfn_CsRptStnkExt

go
create proc uspfn_CsRptStnkExt
@CompanyCode varchar(20),
@pDateFrom datetime,
@pDateTo datetime,
@pStatus char(1)
as
begin
select a.CompanyCode     
     , b.CustomerCode     
	 , e.BranchCode    
	 , BPKDate = convert(varchar(11),getdate(),106)      
     , STNKDate = convert(varchar(11),getdate(),106) 
  	 , h.LeasingCode
  	 --Finance Institution
  	 , h.Tenor
     , h.IsStnkExtend
  	 , h.CustomerCategory
  	 , h.ReqKtp
  	 , h.ReqStnk
  	 , h.ReqBpkb
     , e.SalesModelCode as CarType
     , e.ColourCode as Color    
     , d.PoliceRegNo     
     , (a.ChassisCode + convert(varchar, a.ChassisNo)) as Chassis     
     , (a.EngineCode + convert(varchar, a.EngineNo)) as Engine 
     , f.Salesman as SalesmanCode     
     , g.EmployeeName as SalesmanName    
     , c.CustomerName    
     , c.PhoneNo      
     , i.CompanyName CompanyName        
     , j.CompanyName BranchName     
	 , StnkExpiredDate =  Convert(varchar(11), h.StnkExpiredDate, 106)
     , (case h.Status when 0 then 'In Progress' else 'Finish' end) as StatusInfo  
     , left(c.Address1, 40) as Address
  	 , h.Comment
  	 , h.Additional
  	 , h.Status
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
  LEFT JOIN CsStnkExt h  
  on h.CompanyCode = a.CompanyCode  
  and h.Chassis = a.ChassisCode + convert(varchar(20), a.ChassisNo)
  left join gnMstOrganizationHdr i        
  on i.CompanyCode = f.CompanyCode        
  left join gnMstCoProfile j        
  on j.CompanyCode = f.CompanyCode        
  and j.BranchCode = f.BranchCode   
	where 1=1
		and j.companycode like @companycode
   and h.Status like case when @pStatus = 0 then @pStatus else '%' end
   and h.Status not like case when @pStatus = 0 then '' else '0' end
end

GO


