
go
if object_id('SvJobOrderView') is not null
	drop view SvJobOrderView;

go
CREATE VIEW [dbo].[SvJobOrderView]
AS
SELECT a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo,
     a.ServiceNo, a.JobOrderNo, a.JobOrderDate, 
     a.EstimationNo, a.EstimationDate, a.BookingNo, 
     a.BookingDate, a.PoliceRegNo, a.ServiceBookNo, 
     a.BasicModel, a.TransmissionType, a.ChassisCode, 
     a.ChassisNo, 
     a.ChassisCode + ' ' + CAST(a.ChassisNo AS varchar) 
     AS Chassis, a.EngineCode, a.EngineNo, 
     a.EngineCode + ' ' + CAST(a.EngineNo AS varchar) 
     AS Engine, a.ColorCode,
         (SELECT RefferenceDesc1
       FROM dbo.omMstRefference AS g
       WHERE (CompanyCode = a.CompanyCode) AND 
             (RefferenceType = 'COLO') AND 
             (RefferenceCode = a.ColorCode)) 
     AS ColorName, a.CustomerCode, 
     a.CustomerCode + ' - ' + b.CustomerName AS Customer,
      b.CustomerName, 
     b.Address1 + ' ' + b.Address2 + ' ' + b.Address3 + ' ' + b.Address4
      AS CustomerAddress, 
     a.CustomerCodeBill + ' - ' + c.CustomerName AS CustomerBill,
      a.CustomerCodeBill, 
     c.CustomerName AS CustomerNameBill, 
     c.Address1 + ' ' + c.Address2 + ' ' + c.Address3 + ' ' + c.Address4
      AS CustomerBillAddress, c.NPWPNo, c.PhoneNo, 
     c.HPNo, a.Odometer, a.JobType, a.ForemanID, 
     a.MechanicID, a.ServiceStatus, a.ServiceType, 
     CASE WHEN a.ServiceStatus IN (0, 1, 2, 3, 4, 5) 
     THEN 'Outstanding Task' ELSE 'Finish Task' END AS StatusTask,
      CASE WHEN a.ServiceStatus = '4' THEN CASE WHEN '4W'
      = '4W' THEN d .Description ELSE d .LockingBy END ELSE
      d .Description END AS ServiceStatusDesc, 
     a.ServiceRequestDesc, 
     e.EmployeeName AS ForemanName, 
     f.EmployeeName AS MechanicName,
     a.TotalSrvAmount
FROM dbo.svTrnService AS a LEFT OUTER JOIN
     dbo.gnMstCustomer AS b ON 
     b.CompanyCode = a.CompanyCode AND 
     b.CustomerCode = a.CustomerCode LEFT OUTER JOIN
     dbo.gnMstCustomer AS c ON 
     c.CompanyCode = a.CompanyCode AND 
     c.CustomerCode = a.CustomerCodeBill LEFT OUTER JOIN
     dbo.svMstRefferenceService AS d ON 
     d.CompanyCode = a.CompanyCode AND 
     d.ProductType = a.ProductType AND 
     d.RefferenceCode = a.ServiceStatus AND 
     d.RefferenceType = 'SERVSTAS' LEFT OUTER JOIN
     dbo.gnMstEmployee AS e ON 
     e.CompanyCode = a.CompanyCode AND 
     e.BranchCode = a.BranchCode AND 
     e.EmployeeID = a.ForemanID LEFT OUTER JOIN
     dbo.gnMstEmployee AS f ON 
     f.CompanyCode = a.CompanyCode AND 
     f.BranchCode = a.BranchCode AND 
     f.EmployeeID = a.MechanicID
