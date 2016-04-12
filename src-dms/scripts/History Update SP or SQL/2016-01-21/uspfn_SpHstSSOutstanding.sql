-- =============================================  
-- Author:  <Author,,Name>  
-- Create date: <Create Date,,>  
-- Description: <SS OUTSTANDING HISTORY>  
-- =============================================  
  
ALTER PROCEDURE [dbo].[uspfn_SpHstSSOutstanding]    
 @CompanyCode VARCHAR(15),  
 @BranchCode  VARCHAR(15),  
 @ProductType VARCHAR(15),  
 @ClosingDate DATETIME,         -- Revisi (VARCHAR)
 @User   VARCHAR(15),			-- for Created By  
 @ThisTime  DATETIME			-- for created date  
  
AS  
BEGIN  
-- Get format 'DateTime' for insert into table SpHstSSOutHdr  
DECLARE @Date DATETIME  
SET @Date = (SELECT CONVERT(DATETIME, @ClosingDate))  
--------------------------------------------------------------  
-- DELETE DATA FIRST   
DELETE FROM SpHstSSOutDtl WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND ProductType = @ProductType AND DateClosing = @Date  
DELETE FROm SpHstSSOutHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND ProductType = @ProductType AND DateClosing = @Date  
  
-- DATA HEADER (SpHstSSOutHdr)  
SELECT * INTO #f1 FROM (  
SELECT DISTINCT  
 @CompanyCode CompanyCode,  
 @BranchCode BranchCode,  
 @ProductType ProductType,  
 @Date DateClosing,  
 a.DocNo,  
 a.DocDate,  
 a.UsageDocNo JobOrderNo,  
 a.UsageDocDate JobOrderDate,  
 a.CustomerCode,   
 a.TransType,  
 b.PPnPct,   
 @User CreatedBy,  
 @ThisTime CreatedDate  
FROM  
 spTrnSORDHdr a  
INNER JOIN  
 spTrnSORDDtl b1 ON b1.CompanyCode = a.CompanyCode   
 AND b1.BranchCode = a.BranchCode   
 AND b1.DocNo  = a.DocNo  
LEFT JOIN svTrnService b ON b.CompanyCode = a.CompanyCode   
 AND b.BranchCode = a.BranchCode   
 AND b.ProductType = @ProductType  
 AND b.JobOrderNo = a.UsageDocNo  
INNER JOIN spTrnSLmpDtl c ON c.CompanyCode=b1.CompanyCode   
 AND c.BranchCode = b1.BranchCode  
 AND c.DocNo   = b1.DocNo   
 AND c.PartNo  = b1.PartNo  
WHERE  
 1 = 1  
 AND a.CompanyCode = @CompanyCode  
 AND a.BranchCode = @BranchCode  
 AND REPLACE(CONVERT(VARCHAR, a.DocDate, 111), '/', '') <= REPLACE(CONVERT (VARCHAR, @ClosingDate, 111), '/', '')  
 AND a.TransType LIKE '2%' -- SERVICE  
 AND a.SalesType = '2' -- SPK  
 AND b.ServiceStatus < '6'   
 AND a.Status != 3 -- Status Delete not include  
) #f1  
---------------------------------------------------------------------  
-- INSERT DATA HEADER SpHstSSOutHdr  
INSERT INTO SpHstSSOutHdr  
SELECT * FROM #F1  
---------------------------------------------------------------------  
  
DECLARE @SSNo VARCHAR(15)  
  
DECLARE db_cursor CURSOR FOR  
SELECT DocNo FROM #F1  
  
OPEN db_cursor  
FETCH NEXT FROM db_cursor INTO @SSNo  
  
WHILE @@FETCH_STATUS = 0  
BEGIN  
-- INSERT DATA DETAIL SpHstSSOutDtl  
 INSERT INTO SpHstSSOutDtl  
 SELECT DISTINCT  
  @CompanyCode CompanyCode  
  ,@BranchCode BranchCode  
  ,@ProductType ProductType  
  ,@Date DateClosing  
  ,a.DocNo    
  ,a.PartNo  
  ,(SELECT ISNULL(COUNT(PartNo),0)+1 FROM SpHstSSOutDtl WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode  
   AND ProductType=@ProductType AND DateClosing = @Date AND DocNo = a.DocNo AND PartNo=a.PartNo) SeqNo  
  ,ISNULL((e.SupplyQty - e.ReturnQty),0) Qty  
  ,a.RetailPrice  
  ,ISNULL(((e.SupplyQty - e.ReturnQty) * ISNULL(b1.CostPrice,0)), 0) CostPrice   
 FROM   
  SpTrnSORDDtl a  
 LEFT JOIN spTrnSLmpDtl b1 ON b1.CompanyCode = a.CompanyCode   
  AND b1.BranchCode = a.BranchCode  
  AND b1.DocNo  = a.DocNo   
  AND b1.PartNo  = a.PartNo  
 LEFT JOIN SpHstSSOutHdr b ON b.CompanyCode = a.CompanyCode  
  AND b.BranchCode = a.BranchCode  
  AND b.DocNo = a.DocNo  
 LEFT JOIN   
 (  
  SELECT   
   c.CompanyCode  
   ,c.BranchCode     
   ,d.SupplySlipNo AS DocNo,  
   d.PartNo,  
   SUM(ISNULL(d.SupplyQty,0)) AS SupplyQty,  
   SUM(ISNULL(d.ReturnQty,0)) AS ReturnQty   
  FROM   
   svTrnService c  
  INNER JOIN svTrnSrvItem d ON c.CompanyCode = d.CompanyCode  
   AND c.BranchCode = d.BranchCode  
   AND c.ProductType = d.ProductType  
   AND c.ServiceNo  = d.ServiceNo  
   AND d.SupplyQty - d.ReturnQty >0  
  GROUP BY   
   c.CompanyCode, c.BranchCode, d.SupplySlipNo, d.PartNo  
 ) e ON e.CompanyCode = a.CompanyCode   
  AND e.BranchCode = a.BranchCode   
  AND e.DocNo   = a.DocNo  
  AND e.PartNo  = a.PartNo  
 WHERE  
  1 = 1  
  AND a.CompanyCode = @CompanyCode  
  AND a.BranchCode  = @BranchCode  
  AND a.DocNo    = @SSNo  
  AND (e.SupplyQty - e.ReturnQty) > 0   
----------------------------------------------------------------------------  
 FETCH NEXT FROM db_cursor INTO @SSNo  
END  
  
CLOSE db_cursor  
DEALLOCATE db_cursor  
DROP TABLE #f1  
  
--SELECT * FROM SpHstSSOutHdr  
--SELECT * FROM SpHstSSOutDtl  
  
END
