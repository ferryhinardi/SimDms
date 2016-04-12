CREATE Procedure Uspfn_SrvItemUpdateSSReturnNo @CompanyCode varchar(15), @BranchCode varchar(15),  
@ProductType varchar(2), @ReturnNo varchar(25), @PartNo varchar(25), @IsSaveProcess bit, @LastUpdateBy varchar(25)  
as  
SELECT  
 a.CompanyCode  
 , a.BranchCode  
 , d.ProductType  
 , d.ServiceNo  
 , a.PartNo  
 , (SELECT TOP 1 PartSeq FROM svTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode   
        AND ProductType = d.ProductType AND ServiceNo = d.ServiceNo AND PartNo = a.PartNo AND SupplySlipNo =   
        c.DocNo ORDER BY PartSeq DESC) PartSeq  
 , a.ReturnNo SSReturnNo  
 , b.ReturnDate SSReturnDate  
INTO  
 #SrvItem  
FROM   
 spTrnSRturSSDtl a WITH(NOLOCK, NOWAIT)  
 LEFT JOIN spTrnSRturSSHdr b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode  
  AND a.BranchCode = b.BranchCode  
  AND a.ReturnNo = b.ReturnNo  
 LEFT JOIN spTrnSORDHdr c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode  
  AND a.BranchCode = c.BranchCode  
  AND a.DocNo = c.DocNo  
 LEFT JOIN svTrnService d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode  
  AND a.BranchCode = d.BranchCode  
  AND c.UsageDocNo = d.JobOrderNo  
WHERE  
 a.CompanyCode = @CompanyCode  
 AND a.BranchCode = @BranchCode  
 AND d.ProductType = @ProductType  
 AND a.ReturnNo = @ReturnNo  
 AND a.PartNo = @PartNo  
  
UPDATE  
 svTrnSrvItem  
SET  
 SSReturnNo = CASE @IsSaveProcess WHEN '1' THEN b.SSReturnNo ELSE '' END   
    , SSReturnDate = CASE @IsSaveProcess WHEN '1' THEN b.SSReturnDate ELSE '1900-01-01 00:00:00.000' END  
 , LastupdateBy = @LastupdateBy  
 , LastupdateDate = GETDATE()  
FROM  
 svTrnSrvItem a, #SrvItem b  
WHERE  
 a.CompanyCode = b.CompanyCode  
 AND a.BranchCode = b.BranchCode  
 AND a.ProductType = b.ProductType  
 AND a.ServiceNo = b.ServiceNo  
 AND a.PartNo = b.PartNo  
 AND a.PartSeq = b.PartSeq  
  
DROP TABLE #SrvItem