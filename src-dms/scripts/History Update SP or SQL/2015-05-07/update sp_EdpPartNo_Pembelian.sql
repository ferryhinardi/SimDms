ALTER procedure [dbo].[sp_EdpPartNo_Pembelian]  (  
@CompanyCode varchar(10),
@BranchCode varchar(10),
@DocNo varchar(20),
@bPPN bit
--@BinningNo varchar(20),
--@Opt varchar(10)
)
 as 
 
if @bPPN = 1
begin
SELECT c.PartNo, c.PartName, c.PurchasePrice, c.DiscPct, SUM(c.OnOrder) AS MaxReceived
FROM
(
    SELECT
     a.PartNo
    ,b.PartName 
    ,ISNULL((SELECT round(x.PurchasePrice + (x.PurchasePrice * (select (TaxPct/100) from gnMstTax
where TaxCode = (select ParaValue from gnMstLookUpDtl where CodeID = 'BINS' and SeqNo = 3))),0)
FROM SpMstItemPrice x where x.CompanyCode = a.CompanyCode AND
     x.BranchCode = a.BranchCode AND x.PartNo = a.PartNo),0) AS PurchasePrice
    ,a.OnOrder
    ,a.DiscPct
    FROM spTrnPOrderBalance a 
    INNER JOIN spMstItemInfo b
       ON b.CompanyCode = a.CompanyCode
      AND b.PartNo      = a.PartNo
      WHERE a.CompanyCode = @CompanyCode
      AND a.BranchCode  = @BranchCode
      AND a.PosNo       = @DocNo      
) c
GROUP BY c.PartNo, c.PartName, c.PurchasePrice, c.DiscPct
HAVING sum(c.OnOrder) > 0
end
else
begin
SELECT c.PartNo, c.PartName, c.PurchasePrice,  c.DiscPct, SUM(c.OnOrder) AS MaxReceived
FROM
(
    SELECT
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
    WHERE a.CompanyCode = @CompanyCode
      AND a.BranchCode  = @BranchCode
      AND a.PosNo       = @DocNo
) c
GROUP BY c.PartNo, c.PartName, c.PurchasePrice, c.DiscPct
HAVING sum(c.OnOrder) > 0
end