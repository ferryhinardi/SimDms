
CREATE Procedure Uspfn_spSOAccsUpdateSSReturnNo @CompanyCode varchar(15), @BranchCode varchar(15),  
@ProductType varchar(2), @ReturnNo varchar(25), @PartNo varchar(25), @IsSaveProcess bit, @LastUpdateBy varchar(25)  
as  
SELECT  
 a.CompanyCode  
 , a.BranchCode  
 , d.SONo  
 , a.PartNo  
 , a.ReturnNo SSReturnNo  
 , b.ReturnDate SSReturnDate  
INTO  
 #SOAccs  
FROM  
 spTrnSRturSSDtl a WITH(NOLOCK, NOWAIT)  
 LEFT JOIN spTrnSRturSSHdr b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode  
  AND a.BranchCode = b.BranchCode  
  AND a.ReturnNo = b.ReturnNo  
 LEFT JOIN spTrnSORDHdr c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode  
  AND a.BranchCode = c.BranchCode  
  AND a.DocNo = c.DocNo  
 LEFT JOIN omTrSalesSO d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode  
  AND a.BranchCode = d.BranchCode  
  AND c.UsageDocNo = d.SONo  
 INNER JOIN omTrSalesSOAccsSeq e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode  
  AND a.BranchCode = e.BranchCode  
  AND a.PartNo = e.PartNo  
  AND d.SONo = e.SONo  
        AND e.PartSeq=1  
WHERE  
 a.CompanyCode = @CompanyCode  
 AND a.BranchCode = @BranchCode  
 AND a.ReturnNo = @ReturnNo  
 AND a.PartNo = @PartNo  
  
UPDATE  
 omTrSalesSOAccsSeq  
SET  
 SSReturnNo = CASE @IsSaveProcess WHEN '1' THEN b.SSReturnNo ELSE '' END  
 , SSReturnDate = CASE @IsSaveProcess WHEN '1' THEN b.SSReturnDate ELSE '1900-01-01 00:00:00.000' END  
 , LastUpdateBy = @LastUpdateBy  
 , LastUpdateDate = GETDATE()  
FROM  
 omTrSalesSOAccsSeq a, #SOAccs b  
WHERE  
 a.CompanyCode = b.CompanyCode  
 AND a.BranchCode = b.BranchCode  
 AND a.SONo = b.SONo  
 AND a.PartNo = b.PartNo  
    AND a.PartSeq=1  
  
DROP TABLE #SOAccs