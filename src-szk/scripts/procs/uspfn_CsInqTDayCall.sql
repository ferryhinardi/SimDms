alter procedure uspfn_CsInqTDayCall
    @CompanyCode varchar(20),
    @BranchCode varchar(20) = '',
    @DateFrom varchar(10),
    @DateTo varchar(10)
as

select b.CompanyCode
     , b.BranchCode  
     , b.CustomerCode
     , b.CustomerName
     , b.Address
     , b.PhoneNo
     , b.HPNo
     , b.AddPhone1
     , b.AddPhone2
     , b.BirthDate
     , c.CarType
     , c.Color
     , c.PoliceRegNo
     , a.Chassis
     , c.Engine 
     , c.SONo
     , c.SalesmanCode
     , c.SalesmanName
     , c.SalesmanName as Salesman
     , b.BirthDate
     , b.ReligionCode
     , case a.IsDeliveredA when 1 then 'Ya' else 'Tidak' end IsDeliveredA
     , case a.IsDeliveredB when 1 then 'Ya' else 'Tidak' end IsDeliveredB
     , case a.IsDeliveredC when 1 then 'Ya' else 'Tidak' end IsDeliveredC
     , case a.IsDeliveredD when 1 then 'Ya' else 'Tidak' end IsDeliveredD
     , case a.IsDeliveredE when 1 then 'Ya' else 'Tidak' end IsDeliveredE
     , case a.IsDeliveredF when 1 then 'Ya' else 'Tidak' end IsDeliveredF
     , case a.IsDeliveredG when 1 then 'Ya' else 'Tidak' end IsDeliveredG
     , a.Comment, a.Additional, a.Status
	 , c.DeliveryDate
     , a.CreatedDate
 from CsTDayCall a
inner join CsCustomerView b
   on b.CompanyCode = a.CompanyCode
  and b.CustomerCode = a.CustomerCode
 inner join CsCustomerVehicleView c
   on c.CompanyCode = a.CompanyCode
  and c.Chassis = a.Chassis
where b.CompanyCode = @CompanyCode
  and b.BranchCode = (case isnull(@BranchCode, '') when '' then b.BranchCode else @BranchCode end)
  and convert(varchar, a.CreatedDate, 112) between @DateFrom and @DateTo
order by a.CreatedDate
