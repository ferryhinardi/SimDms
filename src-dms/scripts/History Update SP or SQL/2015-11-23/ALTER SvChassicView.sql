ALTER VIEW [dbo].[SvChassicView]
AS
select a.CompanyCode, a.ChassisCode, cast(a.ChassisNo as varchar) ChassisNo, a.EngineCode, a.SalesModelYear
     , cast(a.EngineNo as varchar) EngineNo, a.ServiceBookNo
     , case a.PoliceRegistrationDate when ('19000101') then NULL else a.PoliceRegistrationDate end as PoliceRegistrationDate
     , a.PoliceRegistrationNo, FakturPolisiNo, d.FakturPolisiDate
     , case a.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end as Status
     , b.BasicModel
     , b.TechnicalModelCode
     , b.TransmissionType
     , a.ColourCode
     , c.CustomerCode
  from omMstVehicle a
  left join omMstModel b
    on b.CompanyCode = a.CompanyCode
   and b.SalesModelCode = a.SalesModelCode
  left join omTrSalesInvoice c
    on c.CompanyCode = a.CompanyCode
   and c.InvoiceNo = a.InvoiceNo
  LEFT JOIN svMstCustomerVehicle d
	ON a.CompanyCode = d.CompanyCode 
	AND a.ChassisCode = d.ChassisCode 
	AND a.ChassisNo = d.ChassisNo
 where --a.ChassisCode = 'MA3GXB72SE0' AND a.ChassisNo='477379' and--a.CompanyCode = @CompanyCode
    a.IsActive = 1



