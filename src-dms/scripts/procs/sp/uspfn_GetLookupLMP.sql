/****** Object:  StoredProcedure [dbo].[uspfn_GetLookupLMP]    Script Date: 02/23/2015 10:26:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[uspfn_GetLookupLMP] @CompanyCode varchar(15), @BranchCode varchar(15), @SalesType varchar(15), @CodeID varchar(6),  
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
 spTrnSBPSFHdr.CustomerCode,
 spTrnSBPSFHdr.CustomerCodeShip,
 spTrnSBPSFHdr.CustomerCodeBill as CustomerCodeTagih,
 b.CustomerName,
 b.Address1,
 b.Address2,
 b.Address3,
 b.Address4,
 b.CustomerName CustomerNameTagih,
 b.Address1 Address1Tagih,
 b.Address2 Address2Tagih,
 b.Address3 Address3Tagih,
 b.Address4 Address4Tagih,
 c.LookUpValueName TransType  
FROM spTrnSBPSFHdr  
left join gnMstCustomer b
ON spTrnSBPSFHdr.CompanyCode = b.CompanyCode
AND spTrnSBPSFHdr.CustomerCode = b.CustomerCode 
left join gnMstLookupDtl c on
spTrnSBPSFHdr.CompanyCode = c.CompanyCode
and spTrnSBPSFHdr.TransType= c.LookupValue 
AND c.CodeID = @CodeID
WHERE spTrnSBPSFHdr.CompanyCode = @CompanyCode  
AND spTrnSBPSFHdr.BranchCode    = @BranchCode  
AND spTrnSBPSFHdr.SalesType     = @SalesType  
AND spTrnSBPSFHdr.TypeOfGoods   = @TypeOfGoods  
AND (spTrnSBPSFHdr.Status = '1' OR spTrnSBPSFHdr.Status = '0')  
AND (spTrnSBPSFHdr.PickingSlipNo NOT IN (SELECT PickingSlipNo FROM spTrnSLmpHdr where CompanyCode = @CompanyCode AND BranchCode = @BranchCode))  
) A  
WHERE A.ProductType = @ProductType  
ORDER BY A.PickingSlipNo DESC