/****** Object:  StoredProcedure [dbo].[uspfn_InvoiceFakturPajak]    Script Date: 04/20/2015 15:18:37 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[uspfn_InvoiceFakturPajak]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[uspfn_InvoiceFakturPajak]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_InvoiceFakturPajak]    Script Date: 04/20/2015 15:18:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[uspfn_InvoiceFakturPajak] @CompanyCode varchar(15), @BranchCode varchar(15), @ProductType varchar(2)
as
begin
SELECT TOP 1500
 Invoice.ProductType, Invoice.InvoiceNo, 
 case Invoice.InvoiceDate when ('19000101') then null else Invoice.InvoiceDate end as InvoiceDate
,Invoice.InvoiceStatus, Invoice.FPJNo
,case Invoice.FPJDate when ('19000101') then null else Invoice.FPJDate end as FPJDate
,Invoice.JobOrderNo
,case Invoice.JobOrderDate when ('19000101') then null else Invoice.JobOrderDate end as JobOrderDate
,Invoice.JobType, Invoice.ChassisCode, Invoice.ChassisNo, Invoice.EngineCode
,Invoice.EngineNo, Invoice.PoliceRegNo, Invoice.BasicModel, Invoice.CustomerCode, Invoice.CustomerCodeBill
,Invoice.Remarks, (Invoice.CustomerCode + ' - ' + Cust.CustomerName) as Customer
,(Invoice.CustomerCodeBill + ' - ' + CustBill.CustomerName) as CustomerBill
, vehicle.ServiceBookNo, Invoice.Odometer
FROM svTrnInvoice Invoice
LEFT JOIN gnMstCustomer Cust
    ON Cust.CompanyCode = Invoice.CompanyCode AND Cust.CustomerCode = Invoice.CustomerCode
LEFT JOIN gnMstCustomer CustBill
    ON CustBill.CompanyCode = Invoice.CompanyCode AND CustBill.CustomerCode = Invoice.CustomerCodeBill
LEFT JOIN svMstcustomerVehicle vehicle 
	ON Invoice.CompanyCode = vehicle.CompanyCode and Invoice.ChassisCode = vehicle.ChassisCode and 
	Invoice.ChassisNo = vehicle.ChassisNo and Invoice.EngineCode = vehicle.EngineCode and 
	Invoice.EngineNo = vehicle.EngineNo and Invoice.BasicModel = vehicle.BasicModel	
WHERE Invoice.CompanyCode = @CompanyCode AND Invoice.BranchCode = @BranchCode 
    AND Invoice.ProductType = @ProductType
    AND convert(varchar, Invoice.InvoiceDate, 112) >= isnull((
            select top 1 convert(varchar, FromDate, 112) from gnMstPeriode
             where 1 = 1
               and CompanyCode = @CompanyCode
               and BranchCode = @BranchCode
               and FiscalYear = (select FiscalYear from gnMstCoProfileService where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
             order by FromDate
            ), '') ORDER BY Invoice.InvoiceNo
end
GO


