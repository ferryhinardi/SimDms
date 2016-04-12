alter procedure uspfn_CsInqTDayCall
--declare
	@CompanyCode varchar(20),
	@DateFrom varchar(10),
	@DateTo varchar(10)
as

;with x as (
select a.CompanyCode, a.CustomerCode, a.Chassis
     , case a.IsDeliveredA when 1 then 'Ya' else 'Tidak' end IsDeliveredA
     , case a.IsDeliveredB when 1 then 'Ya' else 'Tidak' end IsDeliveredB
     , case a.IsDeliveredC when 1 then 'Ya' else 'Tidak' end IsDeliveredC
     , case a.IsDeliveredD when 1 then 'Ya' else 'Tidak' end IsDeliveredD
     , case a.IsDeliveredE when 1 then 'Ya' else 'Tidak' end IsDeliveredE
     , a.Comment, a.Additional, a.Status
     , DeliveryDate = isnull((    
		  select top 1 y.DODate    
			from omTrSalesDODetail x    
			left join omTrSalesDO y    
			  on y.CompanyCode = x.CompanyCode    
			 and y.BranchCode = x.BranchCode    
			 and y.DONo = x.DONo    
		   where x.CompanyCode = a.CompanyCode    
			 and x.ChassisCode + convert(varchar, x.ChassisNo)= a.Chassis
          ), null)  
     , b.CustomerName, b.Address1, b.Address2, b.PhoneNo, b.HPNo
     , rtrim(b.Address1) + ' ' + rtrim(b.Address2) + rtrim(b.Address3) as Address
     , c.SalesModelCode as CarType
     , c.ColourCode as Color
     , f.PoliceRegNo
     , f.EngineCode + convert(varchar, f.EngineNo) as Engine 
     , Salesman = isnull((
			select top 1 y.EmployeeName from omTrSalesSO x, gnMstEmployee y
			 where x.CompanyCode = y.CompanyCode
			   and x.BranchCode = y.BranchCode
			   and x.Salesman = y.EmployeeID
			   and x.CompanyCode = a.CompanyCode
			   and x.SONo = c.SONo
       ), '')
  from CsTDayCall a
  left join GnMstCustomer b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
  left join omTrSalesSOVin c
    on c.CompanyCode = a.CompanyCode
   and (c.ChassisCode + convert(varchar, c.ChassisNo)) = a.Chassis
  left join svMstCustomerVehicle f
    on f.CompanyCode = a.CompanyCode
   and f.ChassisCode + convert(varchar, f.ChassisNo) = a.Chassis
 where a.CompanyCode = @CompanyCode
)
select * from x where convert(varchar, x.DeliveryDate, 112) between @DateFrom and @DateTo



