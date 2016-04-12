
CREATE VIEW [dbo].[SvTrnInvoiceCancelView]
AS

SELECT 
	Invoice.CompanyCode, Invoice.BranchCode, Invoice.ProductType, Invoice.InvoiceNo
	,isnull(Invoice.InvoiceDate,'19000101') InvoiceDate
	,Invoice.InvoiceStatus, Invoice.FPJNo, FPJ.FPJGovNo
	,isnull(Invoice.FPJDate,'19000101') FPJDate
	,Invoice.JobOrderNo
	,isnull(Invoice.JobOrderDate,'19000101') JobOrderDate
	,Invoice.JobType, Invoice.ChassisCode, Invoice.ChassisNo, Invoice.EngineCode
	,Invoice.EngineNo, Invoice.PoliceRegNo, Invoice.BasicModel, Invoice.CustomerCode, Invoice.CustomerCodeBill
	,Invoice.Remarks, (Invoice.CustomerCode + ' - ' + Cust.CustomerName) as Customer
	,(Invoice.CustomerCodeBill + ' - ' + CustBill.CustomerName) as CustomerBill
	, Invoice.Odometer, Invoice.LaborDiscPct, Invoice.PartsDiscPct, Invoice.MaterialDiscPct
	, Invoice.LaborDppAmt, Invoice.PartsDppAmt, Invoice.MaterialDppAmt
	, Invoice.TotalDppAmt, Invoice.TotalPpnAmt, Invoice.TotalSrvAmt
	
	, vehicle.ServiceBookNo
	
	, isnull(CustBill.CustomerName, '') CustomerName, isnull(CustBill.Address1, '') Address1, isnull(CustBill.Address2, '') Address2
	, isnull(CustBill.Address3, '') Address3, isnull(CustBill.Address4, '') Address4, isnull(CustBill.PhoneNo, '') PhoneNo
	, isnull(CustBill.HPNo, '') HPNo, isnull(CustBill.NPWPNo, '') NPWPNo, isnull(CustBill.NPWPDate,'19000101') NPWPDate, isnull(CustBill.SKPNo, '') SKPNo
	, isnull(CustBill.SKPDate,'19000101') SKPDate, isnull(CustBill.CityCode, '') CityCode, isnull(CityCode.LookUpValueName, '') CityDesc
	
	, isnull(CustProfCenter.TOPCode, '') TOPCode
	, isnull(TOPCode.LookUpValueName, '') TOPDesc
	
	, isnull(case AR.StatusFlag when '0' then 'Unposted' 
						 when '3' then 'Posted'
						 else 'Unknown' end, 'Unknown') StatusAR, isnull(AR.DebetAmt, 0) DebetAmt, isnull(AR.CreditAmt, 0) CreditAmt
    , isnull(AR.BlockAmt, 0) BlockAmt, isnull(AR.ReceiveAmt, 0) ReceiveAmt		
FROM svTrnInvoice Invoice
LEFT JOIN gnMstCustomer Cust
    ON Cust.CompanyCode = Invoice.CompanyCode AND Cust.CustomerCode = Invoice.CustomerCode
LEFT JOIN gnMstCustomer CustBill
    ON CustBill.CompanyCode = Invoice.CompanyCode AND CustBill.CustomerCode = Invoice.CustomerCodeBill
LEFT JOIN svMstcustomerVehicle vehicle 
	ON Invoice.CompanyCode = vehicle.CompanyCode and Invoice.ChassisCode = vehicle.ChassisCode and 
	Invoice.ChassisNo = vehicle.ChassisNo and Invoice.EngineCode = vehicle.EngineCode and 
	Invoice.EngineNo = vehicle.EngineNo and Invoice.BasicModel = vehicle.BasicModel	
LEFT JOIN svTrnFakturPajak FPJ 
	ON FPJ.CompanyCode = Invoice.CompanyCode
	AND FPJ.BranchCode = Invoice.BranchCode
	AND FPJ.FPJNo = Invoice.FPJNo
LEFT JOIN gnMstCustomerProfitCenter CustProfCenter
	ON CustProfCenter.CompanyCode = Invoice.CompanyCode
	AND CustProfCenter.BranchCode = Invoice.BranchCode
	AND CustProfCenter.CustomerCode = Invoice.CustomerCodeBill
	AND ProfitCenterCode = '200'
LEFT JOIN gnMstLookupDtl TOPCode
	ON TOPCode.CompanyCode = Invoice.CompanyCode
	AND TOPCode.CodeID = 'TOPC'
	AND TOPCode.LookupValue = CustProfCenter.TOPCode
LEFT JOIN gnMstLookupDtl CityCode
	ON CityCode.CompanyCode = Invoice.CompanyCode
	AND CityCode.CodeID = 'CITY'
	AND CityCode.LookupValue = CustBill.CityCode
LEFT JOIN ARInterface AR 
	ON AR.CompanyCode = Invoice.CompanyCode
	AND AR.BranchCode = Invoice.BranchCode
	AND AR.DocNo = Invoice.InvoiceNo
WHERE
EXISTS (
    select * from glInterface
     where CompanyCode = Invoice.CompanyCode
       and BranchCode = Invoice.BranchCode
       and DocNo = Invoice.InvoiceNo
       and StatusFlag  = 0
    )
AND (NOT EXISTS (
    select * from arInterface
     where CompanyCode = Invoice.CompanyCode
       and BranchCode = Invoice.BranchCode
       and DocNo = Invoice.InvoiceNo
    ) 
    OR EXISTS (
    select * from arInterface
     where CompanyCode = Invoice.CompanyCode
       and BranchCode = Invoice.BranchCode
       and DocNo = Invoice.InvoiceNo
       and StatusFlag  = 0
       and ReceiveAmt  = 0
       and BlockAmt    = 0
       and DebetAmt    = 0
       and CreditAmt   = 0
    )
)
AND NOT EXISTS (
select * from svTrnFakturPajak
 where CompanyCode = Invoice.CompanyCode
   and BranchCode = Invoice.BranchCode
   and FPJNo = Invoice.FPJNo
   and isnull(FPJGovNo, '') <> '')

GO


