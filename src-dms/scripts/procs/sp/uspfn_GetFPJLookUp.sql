CREATE procedure uspfn_spGetFPJLookUp        
@CompanyCode varchar(15),        
@BranchCode  varchar(15),        
@TypeOfGoods varchar(4),        
@IsPKPOnly  varchar(2)        
as        
SELECT         
    TOP 100 a. FPJNo        
    , a.FPJDate        
    , a.PickingSlipNo        
    , a.PickingSlipDate        
    , a.InvoiceNo        
    , a.InvoiceDate        
    , (SELECT CustomerName FROM gnMstCustomer WHERE CompanyCode = a.CompanyCode AND CustomerCode = a.CustomerCode) CustomerName        
    , a.CustomerCode       
    , a.TOPCode      
    , a.TOPDays      
    , a.TotSalesQty      
    , a.TotSalesAmt      
    , a.TotDiscAmt      
    , a.TotDPPAmt      
    , a.TotPPNAmt      
    , a.TotFinalSalesAmt      
    , a.TransType      
    , a.CustomerCodeBill      
    , a.CustomerCodeShip  
    , a.Status  
    , a.FPJGovNo  
    , a.FPJSignature      
    , c.CustomerCode CustomerCodeTagih      
    , b.CustomerName CustomerNameTagih    
    , b.Address1 Address1Tagih    
    , b.Address2 Address2Tagih    
    , b.Address3 Address3Tagih      
    , b.Address4 Address4Tagih     
    , c.CustomerName       
    , c.Address1       
    , c.Address2       
    , c.Address3     
    , c.Address4     
FROM             
    spTrnSFPJHdr a        
    join SpTrnSFPJInfo b      
    on a.CompanyCode = b.CompanyCode      
    and a.BranchCode = b.BranchCode      
    and a.FPJNo = b.FPJNo      
    join gnMstCustomer c      
    on a.CompanyCode = b.CompanyCode      
    and a.CustomerCode = c.CustomerCode      
WHERE             
    a.CompanyCode = @CompanyCode        
    AND a.BranchCode = @BranchCode        
 AND ((CASE WHEN @IsPKPOnly = 1 THEN a.IsPKP END) = 1 OR (CASE WHEN @IsPKPOnly = 0 THEN a.IsPKP END) = a.IsPKP)        
 AND ((CASE WHEN @TypeOfGoods = '%' THEN a.TypeOfGoods END) = a.TypeOfGoods OR (CASE WHEN @TypeOfGoods <> '%' THEN a.TypeOfGoods END) = '0')        
 ORDER BY a.FPJNo DESC