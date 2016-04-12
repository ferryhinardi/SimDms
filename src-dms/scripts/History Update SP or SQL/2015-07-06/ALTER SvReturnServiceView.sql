
/****** Object:  View [dbo].[SvReturnServiceView]    Script Date: 7/6/2015 11:19:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[SvReturnServiceView]
AS
SELECT a.CompanyCode, a.BranchCode, a.ProductType, 
     a.InvoiceNo, CASE a.InvoiceDate WHEN ('19000101') 
     THEN NULL 
     ELSE a.InvoiceDate END AS InvoiceDate, f.ReturnNo, 
     e.DescriptionEng AS InvoiceStatus, a.FPJNo, 
     CASE a.FPJDate WHEN ('19000101') THEN NULL 
     ELSE a.FPJDate END AS FPJDate, a.JobOrderNo, 
     CASE a.JobOrderDate WHEN ('19000101') THEN NULL 
     ELSE a.JobOrderDate END AS JobOrderDate, 
     a.JobType, a.ChassisCode, a.ChassisNo, 
     a.EngineCode, a.EngineNo, a.PoliceRegNo, 
     a.BasicModel, a.CustomerCode, a.CustomerCodeBill, 
     a.Remarks, 
     a.CustomerCode + ' - ' + b.CustomerName AS Customer,
      a.CustomerCodeBill + ' - ' + c.CustomerName AS CustomerBill,
      d.ServiceBookNo, a.Odometer, d.TransmissionType, 
     d.ColourCode
FROM dbo.svTrnInvoice AS a LEFT OUTER JOIN
     dbo.gnMstCustomer AS b ON 
     b.CompanyCode = a.CompanyCode AND 
     b.CustomerCode = a.CustomerCode LEFT OUTER JOIN
     dbo.gnMstCustomer AS c ON 
     c.CompanyCode = a.CompanyCode AND 
     c.CustomerCode = a.CustomerCodeBill LEFT OUTER JOIN
     dbo.svMstCustomerVehicle AS d ON 
     a.CompanyCode = d.CompanyCode AND 
     a.ChassisCode = d.ChassisCode AND 
     a.ChassisNo = d.ChassisNo AND 
     a.EngineCode = d.EngineCode AND 
     a.EngineNo = d.EngineNo AND 
     a.BasicModel = d.BasicModel LEFT OUTER JOIN
     dbo.svMstRefferenceService AS e ON 
     a.CompanyCode = e.CompanyCode AND 
     a.ProductType = e.ProductType AND 
     e.RefferenceType = 'INVSTATS' AND 
     a.InvoiceStatus = e.RefferenceCode LEFT OUTER JOIN
     dbo.SvTrnReturn AS f ON 
     a.CompanyCode = f.CompanyCode AND 
     a.BranchCode = f.BranchCode AND 
     a.ProductType = f.ProductType AND 
     a.InvoiceNo = f.InvoiceNo
WHERE (a.InvoiceStatus IN ('0', '1', '2', '3', '4'))

GO