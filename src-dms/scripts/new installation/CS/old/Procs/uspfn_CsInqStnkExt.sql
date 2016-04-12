alter procedure uspfn_CsInqStnkExt
	@CompanyCode varchar(20),
	@DateFrom varchar(10),
	@DateTo varchar(10)
as

select distinct a.CompanyCode
     , a.CustomerCode
     , c.CustomerName
     , rtrim(c.Address1) + ' ' + rtrim(c.Address2) + rtrim(c.Address3) as Address
     , c.HPNo
     , b.BpkbDate
     , b.StnkDate
     , a.StnkExpiredDate
     , f.isLeasing
     , f.isLeasing as CustomerCategory
     , case f.isLeasing
		when 1 then 'Leasing' 
		else 'Cash'
	   end as CustCtgDesc
     , f.LeasingCo as LeasingCode
     , d.CustomerName LeasingDesc
     , case f.isLeasing when 1 then (convert(varchar, f.Installment) + ' Months') else '' end as Tenor
     , case a.IsStnkExtend when 1 then 'Ya' when 2 then 'Tidak' else '' end as StnkExtend
     , case a.ReqStnk when 1 then 'Ya' when 2 then 'Tidak' else '' end as ReqStnkDesc
     , e.SalesModelCode
     , e.ColourCode
     , h.PoliceRegNo
     , e.EngineCode + convert(varchar, e.EngineNo) as Engine
     , a.Chassis
     , a.Comment
     , a.Additional
     , g.EmployeeName as SalesmanName
  from CsStnkExt a
  left join CsCustomerVehicle b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.Chassis = a.Chassis
  left join GnMstCustomer c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
  left join omTrSalesSOVin e          
    on e.CompanyCode = a.CompanyCode          
   and e.ChassisCode + convert(varchar, e.ChassisNo) = a.Chassis 
  left join omTrSalesSO f
    on f.CompanyCode = e.CompanyCode          
   and f.BranchCode = e.BranchCode
   and f.SONo = e.SONo
  left join GnMstCustomer d
    on d.CompanyCode = f.CompanyCode
   and d.CustomerCode = f.LeasingCo
  left join HrEmployee g
    on g.CompanyCode = f.CompanyCode          
   and g.EmployeeID = f.Salesman
  left join svMstCustomerVehicle h
    on h.CompanyCode = a.CompanyCode          
   and h.ChassisCode + convert(varchar, h.ChassisNo) = a.Chassis 
 where a.CompanyCode = @CompanyCode
   and convert(varchar, a.StnkExpiredDate, 112) between @DateFrom and @DateTo   
