alter procedure uspfn_CsInqBpkbReminder 
	@CompanyCode varchar(15),
	@DateFrom datetime,
	@DateTo datetime
as

select distinct a.CompanyCode
     , a.CustomerCode
     , c.CustomerName
     , rtrim(c.Address1) + ' ' + rtrim(c.Address2) + rtrim(c.Address3) as Address
     , c.PhoneNo as Telephone
     , b.BpkbDate
     , b.StnkDate
     , case g.isLeasing
		when 1 then 'Leasing' 
		else 'Cash'
	   end as IsLeasing
     , FinanceInstitution = d.CustomerName
     , case g.isLeasing when 1 then (convert(varchar, g.Installment) + ' Months') else '' end as Tenor
     , BpkbReminder = 'None'
     , BpkbRequirements = 'None'
     , CarType=isnull(e.BasicModel, '-')
     , CarColor=isnull(e.ColourCode, '-')
     , PoliceNo=isnull(e.PoliceRegNo, '-')
     , Engine=e.EngineNo
     , Chassis=a.Chassis
     , SalesName=h.EmployeeName
     , CustomerComments=isnull(a.Comment, '')
     , AdditionalInquiries=isnull(a.Additional, '-')
  from CsCustBpkb a
  left join CsCustomerVehicle b
    on a.CompanyCode=b.CompanyCode
   and a.Chassis=b.Chassis
   and a.CustomerCode=b.CustomerCode
  left join gnMstCustomer c
    on a.CompanyCode=c.CompanyCode
   and a.CustomerCode=c.CustomerCode
  left join svMstCustomerVehicle e
    on e.CompanyCode = a.CompanyCode
   and e.ChassisCode + convert(varchar, e.ChassisNo) = a.Chassis
  left join omTrSalesSOVin f
    on f.CompanyCode = f.CompanyCode
   and f.ChassisCode + convert(varchar, f.ChassisNo) = a.Chassis
  left join omTrSalesSO g
    on g.CompanyCode = f.CompanyCode
   and g.BranchCode = f.BranchCode
   and g.SONo = f.SONo
  left join gnMstCustomer d
    on d.CompanyCode=g.CompanyCode
   and d.CustomerCode=g.LeasingCo
  left join HrEmployee h
    on h.CompanyCode = g.CompanyCode
   and h.EmployeeID = g.Salesman
 where a.CompanyCode=@CompanyCode
   and b.BpkbDate between @DateFrom and @DateTo

