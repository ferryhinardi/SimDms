
CREATE VIEW [dbo].[SvTrnServiceMechanicByJobOrderNoView]
AS
SELECT DISTINCT 
     dbo.svTrnService.CompanyCode, 
     dbo.svTrnService.BranchCode, 
     dbo.svTrnService.ProductType, 
     dbo.svTrnService.ServiceNo, 
     dbo.svTrnService.ServiceType, 
     dbo.svTrnService.ForemanID, 
     dbo.svTrnService.EstimationNo, 
     dbo.svTrnService.EstimationDate, 
     dbo.svTrnService.BookingNo, 
     dbo.svTrnService.BookingDate, 
     dbo.svTrnService.JobOrderNo, 
     dbo.svTrnService.JobOrderDate, 
     dbo.svTrnService.InvoiceNo, 
     dbo.svTrnService.PoliceRegNo, 
     dbo.svTrnService.ServiceBookNo, 
     dbo.svTrnService.BasicModel, 
     dbo.svTrnService.TransmissionType, 
     dbo.svTrnService.ChassisCode, 
     dbo.svTrnService.ChassisNo, 
     dbo.svTrnService.EngineCode, 
     dbo.svTrnService.EngineNo, 
     dbo.svTrnService.ChassisCode + ' ' + CAST(dbo.svTrnService.ChassisNo
      AS varchar) AS KodeRangka, 
     dbo.svTrnService.EngineCode + ' ' + CAST(dbo.svTrnService.EngineNo
      AS varchar) AS KodeMesin, 
     dbo.svTrnService.ColorCode, 
     dbo.svTrnService.CustomerCode + ' - ' + dbo.gnMstCustomer.CustomerName
      AS Customer, 
     dbo.svTrnService.CustomerCodeBill + ' - ' + custBill.CustomerName
      AS CustomerBill, dbo.svTrnService.CustomerCode, 
     dbo.svTrnService.CustomerCodeBill, 
     dbo.svTrnService.Odometer, 
     dbo.svTrnService.JobType, 
     reffService.RefferenceCode AS ServiceStatus, 
     reffService.Description AS ServiceStatusDesc, 
     reffService.LockingBy, 
     dbo.svTrnService.InsurancePayFlag, 
     dbo.svTrnService.InsuranceOwnRisk, 
     dbo.svTrnService.InsuranceNo, 
     dbo.svTrnService.InsuranceJobOrderNo, 
     dbo.svTrnService.LaborDiscPct, 
     dbo.svTrnService.PartDiscPct, 
     dbo.svTrnService.MaterialDiscPct, 
     dbo.svTrnService.PPNPct, 
     dbo.svTrnService.ServiceRequestDesc, 
     dbo.svTrnService.ConfirmChangingPart, 
     dbo.svTrnService.EstimateFinishDate, 
     dbo.svTrnService.LaborDppAmt, 
     dbo.svTrnService.PartsDppAmt, 
     dbo.svTrnService.MaterialDppAmt, 
     dbo.svTrnService.TotalDPPAmount, 
     dbo.svTrnService.TotalPpnAmount, 
     dbo.svTrnService.TotalPphAmount, 
     dbo.svTrnService.TotalSrvAmount, 
     employee.EmployeeName AS ForemanName, 
     custBill.Address1 + '' + custBill.Address2 + '' + custBill.Address3
      + '' + custBill.Address4 AS AddressBill, 
     custBill.NPWPNo, custBill.PhoneNo, custBill.FaxNo, 
     custBill.HPNo
FROM dbo.svTrnService WITH (NOLOCK, NOWAIT) 
     LEFT OUTER JOIN
     dbo.gnMstCustomer ON 
     dbo.gnMstCustomer.CompanyCode = dbo.svTrnService.CompanyCode
      AND 
     dbo.gnMstCustomer.CustomerCode = dbo.svTrnService.CustomerCode
      LEFT OUTER JOIN
     dbo.gnMstCustomer AS custBill ON 
     custBill.CompanyCode = dbo.svTrnService.CompanyCode
      AND 
     custBill.CustomerCode = dbo.svTrnService.CustomerCodeBill
      LEFT OUTER JOIN
     dbo.gnMstEmployee AS employee ON 
     employee.CompanyCode = dbo.svTrnService.CompanyCode
      AND 
     employee.BranchCode = dbo.svTrnService.BranchCode AND
      employee.EmployeeID = dbo.svTrnService.ForemanID LEFT
      OUTER JOIN
     dbo.svTrnSrvItem AS srvItem ON 
     srvItem.CompanyCode = dbo.svTrnService.CompanyCode
      AND 
     srvItem.BranchCode = dbo.svTrnService.BranchCode AND
      srvItem.ProductType = dbo.svTrnService.ProductType AND
      srvItem.ServiceNo = dbo.svTrnService.ServiceNo LEFT
      OUTER JOIN
     dbo.svTrnSrvTask AS srvTask ON 
     srvTask.CompanyCode = dbo.svTrnService.CompanyCode
      AND 
     srvTask.BranchCode = dbo.svTrnService.BranchCode AND
      srvTask.ProductType = dbo.svTrnService.ProductType AND
      srvTask.ServiceNo = dbo.svTrnService.ServiceNo LEFT
      OUTER JOIN
     dbo.svMstRefferenceService AS reffService ON 
     reffService.CompanyCode = dbo.svTrnService.CompanyCode
      AND 
     reffService.ProductType = dbo.svTrnService.ProductType
      AND 
     reffService.RefferenceCode = dbo.svTrnService.ServiceStatus
      AND 
     reffService.RefferenceType = 'SERVSTAS' LEFT OUTER JOIN
     dbo.svTrnInvoice AS invoice ON 
     invoice.CompanyCode = dbo.svTrnService.CompanyCode
      AND 
     invoice.BranchCode = dbo.svTrnService.BranchCode AND
      invoice.ProductType = dbo.svTrnService.ProductType AND
      invoice.JobOrderNo = dbo.svTrnService.JobOrderNo LEFT
      OUTER JOIN
     dbo.svTrnSrvVOR AS VOR ON 
     VOR.CompanyCode = dbo.svTrnService.CompanyCode AND
      VOR.BranchCode = dbo.svTrnService.BranchCode AND 
     VOR.ServiceNo = dbo.svTrnService.ServiceNo
WHERE (1 = 1)



GO


