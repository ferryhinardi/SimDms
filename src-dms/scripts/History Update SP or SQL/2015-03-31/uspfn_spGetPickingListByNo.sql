/****** Object:  StoredProcedure [dbo].[uspfn_spGetPickingList]    Script Date: 3/31/2015 2:33:59 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[uspfn_spGetPickingListByNo]        
@CompanyCode varchar(15),          
@BranchCode varchar(15),
@TypeOfGoods varchar(10),          
@ProductType varchar(4),   
@ProfitCenterCode varchar(5),          
@PickingListNo varchar(15)          

as          
SELECT * FROM          
(          
    SELECT           
    a.PickingSlipNo,           
    a.PickingSlipDate,           
    a.InvoiceNo,           
    a.InvoiceDate,          
    (          
        SELECT TOP 1 PRODUCTTYPE FROM spTrnSInvoiceDtl          
        WHERE a.CompanyCode = spTrnSInvoiceDtl.CompanyCode          
        AND a.BranchCode = spTrnSInvoiceDtl.BranchCode          
        AND a.InvoiceNo = spTrnSInvoiceDtl.InvoiceNo          
    ) AS ProductType,      
    a.TotDPPAmt,      
    a.TotPPNAmt,      
    a.TotFinalSalesAmt,      
    b.CustomerCode,      
    b.CustomerName,      
    b.Address1,      
    b.Address2,      
    b.Address3,      
    b.Address4,    
    a.TransType,    
    a.CustomerCodeShip,    
    a.CustomerCodeBill,    
    a.TotSalesAmt,    
    a.TotSalesQty,  
    c.TOPCode,  
    d.ParaValue as TOPDays      
FROM           
    spTrnSInvoiceHdr a         
left JOIN gnMstCustomer b      
on a.CompanyCode = b.CompanyCode      
AND a.CustomerCode = b.CustomerCode     
LEFT join gnMstCustomerDealerProfitCenter c  
ON c.CompanyCode = b.CompanyCode  
and c.BranchCode = a.BranchCode  
AND c.CustomerCode = b.CustomerCode  
and c.ProfitCenterCode = @ProfitCenterCode  
LEFT JOIN gnMstLookUpDtl d  
on d.CompanyCode = a.CompanyCode  
AND d.CodeID = 'TOPC'  
and d.LookUpValue  = c.TOPCode  
WHERE a.CompanyCode = @CompanyCode  
  AND a.BranchCode  = @BranchCode          
    AND a.TypeOfGoods       = @TypeOfGoods          
    AND a.Status        = 0           
) A          
WHERE A.ProductType = @ProductType AND A.PickingSlipNo=@PickingListNo          