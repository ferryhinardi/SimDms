
create view [dbo].[SpEdpPartNo_Pembelian]  as    
SELECT CompanyCode, BranchCode, c.PartNo, c.PartName, c.PurchasePrice,  c.DiscPct, SUM(c.OnOrder) AS MaxReceived, POSNo
FROM
(
    SELECT a.CompanyCode, BranchCode, POSNo,
     a.PartNo
    ,b.PartName 
    ,ISNULL((SELECT x.PurchasePrice FROM SpMstItemPrice x where x.CompanyCode = a.CompanyCode AND
     x.BranchCode = a.BranchCode AND x.PartNo = a.PartNo),0) AS PurchasePrice
    ,a.OnOrder
    ,a.DiscPct
    FROM spTrnPOrderBalance a 
    INNER JOIN spMstItemInfo b
       ON b.CompanyCode = a.CompanyCode
      AND b.PartNo      = a.PartNo
    /*WHERE a.CompanyCode = @CompanyCode
      AND a.BranchCode  = @BranchCode
      AND a.PosNo       = @PosNo*/
) c
GROUP BY c.PartNo, c.PartName, c.PurchasePrice, c.DiscPct, CompanyCode, BranchCode, POSNo
HAVING sum(c.OnOrder) > 0

GO


