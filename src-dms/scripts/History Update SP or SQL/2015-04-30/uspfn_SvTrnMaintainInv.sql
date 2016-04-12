
/****** Object:  StoredProcedure [dbo].[uspfn_SvTrnMaintainInv]    Script Date: 04/30/2015 17:43:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE   procedure [dbo].[uspfn_SvTrnMaintainInv]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),	
	@ProfitCenterCode varchar(15),
	@PeriodeDate varchar(8)
	
as   
--exec uspfn_SvTrnMaintainInv 6159401000,6159401001,'4W',200,'2015-04-01 12:00:00'

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
--inner join GLInterface gl on gl.CompanyCode=Invoice.CompanyCode and gl.BranchCode=Invoice.BranchCode and gl.DocNo= Invoice.InvoiceNo  and gl.StatusFlag=0
--inner join ARInterface ar on ar.CompanyCode=Invoice.CompanyCode and ar.BranchCode=Invoice.BranchCode and  gl.DocNo= Invoice.InvoiceNo  and gl.StatusFlag=0
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
 AND CustProfCenter.ProfitCenterCode = '200'  
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
 where 1=1
and Invoice.InvoiceNo in (select DocNo from GLInterface where 1=1
and companycode=@CompanyCode 
and BranchCode=@BranchCode
and ProfitCenterCode=@ProfitCenterCode
and StatusFlag=0)
and Invoice.InvoiceNo in (select DocNo from ARInterface where 1=1
and companycode=@CompanyCode
and BranchCode=@BranchCode
and ProfitCenterCode=@ProfitCenterCode
and StatusFlag=0)
and CONVERT(varchar,InvoiceDate,112) >=@PeriodeDate
and ProductType=@ProductType
order by InvoiceDate 




GO


