alter procedure uspfn_CsInqTDayCall
	@CompanyCode varchar(20),
	@BranchCode varchar(20) = '',
	@DateFrom varchar(10),
	@DateTo varchar(10)
as

--declare
--	@CompanyCode varchar(20),
--	@DateFrom varchar(10),
--	@DateTo varchar(10)

--select @CompanyCode=N'6006406',@DateFrom=N'20140201',@DateTo=N'20140311'

begin
	;with x as (
	select a.CompanyCode
	     , c.BranchCode
	     , a.CustomerCode
		 , b.CustomerName
		 , rtrim(b.Address1) + ' ' + rtrim(b.Address2) + rtrim(b.Address3) as Address
		 , b.PhoneNo
		 , b.HPNo
		 , c.SalesModelCode as CarType
		 , c.ColourCode as Color
		 , d.PoliceRegNo
		 , a.Chassis
		 , c.EngineCode + convert(varchar, c.EngineNo) as Engine 
		 , c.SONo
		 , Salesman = isnull((
				select top 1 y.EmployeeName from omTrSalesSO x, HrEmployee y
				 where x.CompanyCode = y.CompanyCode
				   and x.Salesman = y.EmployeeID
				   and x.SONo = c.SONo
		   ), '')
		 , e.AddPhone1
		 , e.AddPhone2
		 , b.BirthDate
		 , e.ReligionCode
		 , case a.IsDeliveredA when 1 then 'Ya' else 'Tidak' end IsDeliveredA
		 , case a.IsDeliveredB when 1 then 'Ya' else 'Tidak' end IsDeliveredB
		 , case a.IsDeliveredC when 1 then 'Ya' else 'Tidak' end IsDeliveredC
		 , case a.IsDeliveredD when 1 then 'Ya' else 'Tidak' end IsDeliveredD
		 , case a.IsDeliveredE when 1 then 'Ya' else 'Tidak' end IsDeliveredE
		 , case a.IsDeliveredF when 1 then 'Ya' else 'Tidak' end IsDeliveredF
		 , case a.IsDeliveredG when 1 then 'Ya' else 'Tidak' end IsDeliveredG
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
		 , a.CreatedDate
	  from CsTDayCall a
	  left join GnMstCustomer b
		on b.CompanyCode = a.CompanyCode
	   and b.CustomerCode = a.CustomerCode
	  left join omTrSalesSOVin c
		on c.CompanyCode = a.CompanyCode
	   and (c.ChassisCode + convert(varchar, c.ChassisNo)) = a.Chassis
	  left join svMstCustomerVehicle d
		on d.CompanyCode = a.CompanyCode
	   and d.ChassisCode + convert(varchar, d.ChassisNo) = a.Chassis
	  left join CsCustData e
		on e.CompanyCode = a.CompanyCode
	   and e.CustomerCode = a.CustomerCode
	 where a.CompanyCode = @CompanyCode
	)

	select * from x 
	 where convert(varchar, x.CreatedDate, 112) between @DateFrom and @DateTo
	   and BranchCode = (case isnull(@BranchCode, '') when '' then BranchCode else @BranchCode end)
end


--go
--exec uspfn_CsInqTDayCall '6006406', '20140101', '20140225'






