CREATE PROCEDURE uspfn_spGetLookupLMP @CompanyCode varchar(15), @BranchCode varchar(15), @SalesType varchar(15),    
@TypeOfGoods varchar(15), @ProductType varchar(15)    
as    
SELECT * FROM     
(    
SELECT    
 PickingSlipNo, PickingSlipDate,    
 BPSFNo, BPSFDate,    
 (    
   SELECT TOP 1 PRODUCTTYPE FROM spTrnSBPSFDtl    
  WHERE spTrnSBPSFHdr.CompanyCode = spTrnSBPSFDtl.CompanyCode    
  AND spTrnSBPSFHdr.BranchCode = spTrnSBPSFDtl.BranchCode    
  AND spTrnSBPSFHdr.BPSFNo = spTrnSBPSFDtl.BPSFNo    
 ) AS ProductType,  
 b.CustomerCode,  
 b.CustomerName,  
 b.Address1,  
 b.Address2,  
 b.Address3,  
 b.Address4,  
 b.CustomerCode CustomerCodeTagih,  
 b.CustomerName CustomerNameTagih,  
 b.Address1 Address1Tagih,  
 b.Address2 Address2Tagih,  
 b.Address3 Address3Tagih,  
 b.Address4 Address4Tagih,  
 c.LookUpValueName TransType    
FROM spTrnSBPSFHdr    
join gnMstCustomer b  
ON spTrnSBPSFHdr.CompanyCode = b.CompanyCode  
AND spTrnSBPSFHdr.CustomerCode = b.CustomerCode   
join gnMstLookupDtl c on  
spTrnSBPSFHdr.CompanyCode = c.CompanyCode  
and spTrnSBPSFHdr.TransType= c.LookupValue   
AND c.CodeID = 'TTNP'  
WHERE spTrnSBPSFHdr.CompanyCode = @CompanyCode    
AND spTrnSBPSFHdr.BranchCode    = @BranchCode    
AND spTrnSBPSFHdr.SalesType     = @SalesType    
AND spTrnSBPSFHdr.TypeOfGoods   = @TypeOfGoods    
AND (spTrnSBPSFHdr.Status = '1' OR spTrnSBPSFHdr.Status = '0')    
AND (spTrnSBPSFHdr.PickingSlipNo NOT IN (SELECT PickingSlipNo FROM spTrnSLmpHdr where CompanyCode = @CompanyCode AND BranchCode = @BranchCode))    
) A    
WHERE A.ProductType = @ProductType    
ORDER BY A.PickingSlipNo DESC