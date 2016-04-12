ALTER procedure [dbo].[usprpt_SpRpSum001]  
--Declare
@CompanyCode VARCHAR(15),  
@BranchCode VARCHAR(15),  
@MonthPeriod VARCHAR(10),  
@YearPeriod VARCHAR(10),  
@Warehouse VARCHAR(2),  
@PartNumber VARCHAR(20)  
AS  

--set @CompanyCode = '6115204001'
--set @BranchCode = '6115204301'
--set @MonthPeriod = '10'
--set @YearPeriod = '2015'
--set @Warehouse = '00'
--set @PartNumber = '59300B09J00N000'

BEGIN  
  
DECLARE @QueryStockItem AS VARCHAR(MAX)  
DECLARE @QueryStock AS VARCHAR(MAX)  
DECLARE @PartNoAdjInc AS VARCHAR(MAX)  
DECLARE @PartNoAdjOut AS VARCHAR(MAX)  
DECLARE @PartNoRcv AS VARCHAR(MAX)  
DECLARE @PartNoFpj AS VARCHAR(MAX)  
DECLARE @PartNoWtr AS VARCHAR(MAX)  
DECLARE @PartNoWtr1 AS VARCHAR(MAX)  
DECLARE @PartNoBPSF AS VARCHAR(MAX)  
DECLARE @PartNoRetur AS VARCHAR(MAX)
DECLARE @PartNoPRetur AS VARCHAR(MAX)  
DECLARE @PartNoSupplySlip AS VARCHAR(MAX)  
DECLARE @PartNoItem AS VARCHAR(MAX)  
DECLARE @isExist AS INT  
DECLARE @isKartuStok AS INT  
  
  
SET @isKartuStok = isnull((select ParaValue from gnMstLookUpDtl where CodeID = 'KRT_STK'),0)  
  
-- select SUBSTRING(CONVERT(VARCHAR, GETDATE(), 112),1,6)  
  
---------------------------------------------------------------------------  
IF @PartNumber != ''   
BEGIN  
 SET @PartNoAdjInc = ' AND a.PartNo = ''' + @PartNumber + ''''  
 SET @PartNoAdjOut = ' AND a.PartNo = ''' + @PartNumber + ''''  
 SET @PartNoRcv = ' AND a.PartNo = ''' + @PartNumber + ''''  
 SET @PartNoFpj = ' AND a.PartNo = ''' + @PartNumber + ''''  
 SET @PartNoWtr = ' AND a.PartNo = ''' + @PartNumber + ''''  
 SET @PartNoWtr1 = ' AND a.PartNo = ''' + @PartNumber + ''''  
 SET @PartNoBPSF = ' AND a.PartNo = ''' + @PartNumber + ''''  
 SET @PartNoRetur = 'AND a.PartNo = ''' + @PartNumber + '''' 
 SET @PartNoPRetur = 'AND a.PartNo = ''' + @PartNumber + ''''  
 SET @PartNoSupplySlip = 'AND a.PartNo = ''' + @PartNumber + ''''  
 SET @PartNoItem = 'AND a.PartNo = ''' + @PartNumber + ''''  
END  
ELSE  
BEGIN  
 SET @PartNoAdjInc = ''  
 SET @PartNoAdjOut = ''  
 SET @PartNoRcv = ''  
 SET @PartNoFpj = ''  
 SET @PartNoWtr = ''  
 SET @PartNoWtr1 = ''  
 SET @PartNoBPSF = ''  
 SET @PartNoRetur = ''  
 SET @PartNoPRetur = '' 
 SET @PartNoSupplySlip = ''  
 SET @PartNoItem = ''  
END  
  
SET @QueryStock = '  
SELECT *,   
convert(varchar, Tanggal, 112) + replace(convert(varchar, Tanggal,108),'':'','''') TanggalSort,  
1 GroupID  
INTO #spKartuStock FROM (  
SELECT --Adjustment Incoming  
 e.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 d.PartName,  
 a.AdjustmentNo Nomor,   
 b.AdjustmentDate AS Tanggal,   
 f.LookupValueName Pemasok,  
 a.QtyAdjustment Penerimaan,  
 NULL Pengeluaran,  
  0 Qty,  
     0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''in'' RecStat,  
 '' '' Keterangan   
FROM spTrnIAdjustDtl a  
 JOIN spTrnIAdjustHdr b  
  ON a.CompanyCode = b.CompanyCode  
  AND a.BranchCode = b.BranchCode  
  AND a.AdjustmentNo = b.AdjustmentNo  
 JOIN SpHstStockMovement c  
  ON a.CompanyCode = c.CompanyCode  
  AND a.BranchCode = c.BranchCode   
  AND a.PartNo = c.PartNo  
  AND a.WarehouseCode = c.WarehouseCode  
  AND c.Month = ''' + @MonthPeriod + '''  
  AND c.Year = ''' + @YearPeriod +  '''  
 JOIN spMstItemInfo d  
  ON a.CompanyCode = d.CompanyCode  
  AND a.PartNo = d.PartNo  
 JOIN gnMstLookUpDtl e  
  ON a.CompanyCode = e.CompanyCode  
  AND a.WarehouseCode = e.LookUpValue  
  AND e.CodeId = ''WRCD''  
 LEFT JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = f.CompanyCode  
  AND a.ReasonCode = f.LookUpValue  
  AND f.CodeId = ''RSAD''  
    JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
WHERE a.AdjustmentCode = ''+''  
 AND a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
    AND b.Status = 2  
 AND SUBSTRING(CONVERT(VARCHAR, b.AdjustmentDate, 112),1,6) BETWEEN ''' + @YearPeriod+ RIGHT('0' + @MonthPeriod,2) + ''' AND SUBSTRING(CONVERT(VARCHAR,GetDate(),112),1,6)  
 AND a.WarehouseCode = ''' + @Warehouse + '''  
 ' + @PartNoAdjInc + '  
UNION  
  
SELECT --Adjustment Outgoing  
 e.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 d.PartName,  
 a.AdjustmentNo Nomor,   
 b.AdjustmentDate AS Tanggal,   
 f.LookupValueName Pemasok,  
 NULL Penerimaan,  
 a.QtyAdjustment Pengeluaran,  
 0 Qty,  
    0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''out'' RecStat,  
 '' '' Keterangan  
FROM spTrnIAdjustDtl a  
JOIN spTrnIAdjustHdr b  
 ON a.CompanyCode = b.CompanyCode  
 AND a.BranchCode = b.BranchCode  
 AND a.AdjustmentNo = b.AdjustmentNo  
JOIN SpHstStockMovement c  
 ON a.CompanyCode = c.CompanyCode  
 AND a.BranchCode = c.BranchCode   
 AND a.PartNo = c.PartNo  
 AND a.WarehouseCode = c.WarehouseCode  
 AND c.Month = ''' + @MonthPeriod + '''  
 AND c.Year = ''' + @YearPeriod +  '''  
JOIN spMstItemInfo d  
 ON a.CompanyCode = d.CompanyCode  
 AND a.PartNo = d.PartNo  
JOIN gnMstLookUpDtl e  
 ON a.CompanyCode = e.CompanyCode  
 AND a.WarehouseCode = e.LookUpValue  
 AND e.CodeId = ''WRCD''  
LEFT JOIN gnMstLookUpDtl f  
 ON a.CompanyCode = f.CompanyCode  
 AND a.ReasonCode = f.LookUpValue  
 AND f.CodeId = ''RSAD''  
JOIN spMstItemPrice g  
    ON a.CompanyCode = g.CompanyCode   
    AND a.BranchCode = g.BranchCode  
    AND a.PartNo = g.PartNo  
WHERE a.AdjustmentCode = ''-''   
 AND a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''   
    AND b.Status = 2  
 AND SUBSTRING(CONVERT(VARCHAR, b.AdjustmentDate, 112),1,6) BETWEEN ''' + @YearPeriod+ RIGHT('0' + @MonthPeriod,2) + ''' AND SUBSTRING(CONVERT(VARCHAR,GetDate(),112),1,6)  
 AND a.WarehouseCode = ''' + @Warehouse + '''  
 ' + @PartNoAdjOut + '   
UNION  
  
--RECEIVING  
-- - Incoming  
-- - Pemasok/Pelanggan = Supplier Name  
  
SELECT   
 f.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 e.PartName,  
 a.WRSNo Nomor,  
 b.WRSDate AS Tanggal,  
 c.SupplierName Pemasok,  
 SUM(a.ReceivedQty) Penerimaan,  
 NULL Pengeluaran,  
 0 Qty,  
    0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''in'' RecStat,  
 '' '' Keterangan  
FROM spTrnPRcvDtl a  
 JOIN spTrnPRcvHdr b  
  ON a.CompanyCode = b.CompanyCode   
  AND a.BranchCode = b.BranchCode  
  AND a.WRSNo = b.WRSNo   
 JOIN gnMstSupplier c  
  ON b.CompanyCode = c.CompanyCode  
  AND b.SupplierCode = c.SupplierCode   
  JOIN SpHstStockMovement d  
 ON a.CompanyCode = d.CompanyCode  
 AND a.BranchCode = d.BranchCode   
 AND a.PartNo = d.PartNo  
 AND a.WarehouseCode = d.WarehouseCode  
 AND d.Month = ''' + @MonthPeriod + '''   
 AND d.Year = ''' + @YearPeriod +  '''   
JOIN spMstItemInfo e  
  ON a.CompanyCode = e.CompanyCode  
  AND a.PartNo = e.PartNo  
 LEFT JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = d.CompanyCode  
  AND a.WarehouseCode = LookUpValue  
  AND f.CodeId = ''WRCD''  
   JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND SUBSTRING(CONVERT(VARCHAR, b.WRSDate, 112),1,6) BETWEEN ''' + @YearPeriod+ RIGHT('0' + @MonthPeriod,2) + ''' AND SUBSTRING(CONVERT(VARCHAR,GetDate(),112),1,6)  
 AND a.WarehouseCode = ''' + @Warehouse + '''  
 AND b.Status NOT IN ( ''3'', ''0'', ''1'')  
 ' + @PartNoRcv + ' GROUP BY f.LookUpValueName,  
 a.PartNo,  
 e.PartName,  
 a.WRSNo,  
 b.WRSDate,  
 c.SupplierName,  
 d.BOMStock,  
 d.CostPrice,  
 a.CostPrice,  
    g.CostPrice,  
 d.EOMStock  

UNION  
  
--FAKTUR PAJAK  
-- - Outgoing  
-- - Pemasok/Pelanggan = Customer name  
  
SELECT   
 f.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 e.PartName,  
 a.FPJNo Nomor,  
 b.FPJDate AS Tanggal,  
 c.CustomerName Pemasok,  
 NULL Penerimaan,  
 SUM(a.QtyBill) Pengeluaran,  
 0 Qty,  
 0 Pokok,  
 isnull(A.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''out'' RecStat,  
 isnull(a.DocNo, '' '') Keterangan  
FROM spTrnSFPJDtl a  
JOIN spTrnSFPJHdr b  
 ON a.CompanyCode = b.CompanyCode  
 AND a.BranchCode = b.BranchCode  
 AND a.FPJNo = b.FPJNo  
JOIN gnMstCustomer c  
 ON b.CompanyCode = c.CompanyCode  
 AND b.CustomerCode = c.CustomerCode  
JOIN SpHstStockMovement d  
 ON a.CompanyCode = d.CompanyCode  
 AND a.BranchCode = d.BranchCode   
 AND a.PartNo = d.PartNo  
 AND d.WarehouseCode = ''' + @Warehouse + '''  
 AND d.Month = ''' + @MonthPeriod + '''  
 AND d.Year = ''' + @YearPeriod +  '''   
JOIN spMstItemInfo e  
 ON a.CompanyCode = e.CompanyCode  
 AND a.PartNo = e.PartNo  
LEFT JOIN gnMstLookUpDtl f  
 ON a.CompanyCode = d.CompanyCode  
 AND a.WarehouseCode = LookUpValue  
 AND f.CodeId = ''WRCD''  
JOIN spMstItemPrice g  
    ON a.CompanyCode = g.CompanyCode   
    AND a.BranchCode = g.BranchCode  
    AND a.PartNo = g.PartNo  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND SUBSTRING(CONVERT(VARCHAR, b.FpjDate, 112),1,6) BETWEEN ''' + @YearPeriod+ RIGHT('0' + @MonthPeriod,2) + ''' AND SUBSTRING(CONVERT(VARCHAR,GetDate(),112),1,6)  
 AND a.WarehouseCode = ''' + @Warehouse + '''  
 ' + @PartNoFpj + ' GROUP BY f.LookUpValueName, a.PartNo,  
 e.PartName,  
 a.FPJNo,  
 b.FPJDate,  
 c.CustomerName,  
 d.BOMStock,  
 d.CostPrice,  
 A.CostPrice,  
    g.CostPrice,  
 d.EOMStock,  
 a.DocNo  
 
 UNION  
  
--Retur Pembelian  (TAMBAHAN RETUR PEMBELIAN)
-- - Incoming  
-- - Pemasok/Pelanggan = Supplier Name  
  
SELECT   
 f.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 e.PartName,  
 a.ReturnNo Nomor,  
 b.Returndate AS Tanggal,  
 c.SupplierName Pemasok,  
 NULL Penerimaan,  
 SUM(a.QtyReturn) Pengeluaran,  
 0 Qty,  
    0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''out'' RecStat,  
 '' '' Keterangan  
FROM spTrnPRturDtl a  
 JOIN spTrnPRturHdr b  
  ON a.CompanyCode = b.CompanyCode   
  AND a.BranchCode = b.BranchCode  
  AND a.ReturnNo = b.ReturnNo   
 JOIN gnMstSupplier c  
  ON b.CompanyCode = c.CompanyCode  
  AND b.SupplierCode = c.SupplierCode   
  JOIN SpHstStockMovement d  
 ON a.CompanyCode = d.CompanyCode  
 AND a.BranchCode = d.BranchCode   
 AND a.PartNo = d.PartNo  
 AND a.WarehouseCode = d.WarehouseCode  
 AND d.Month = ''' + @MonthPeriod + '''   
 AND d.Year = ''' + @YearPeriod +  '''   
JOIN spMstItemInfo e  
  ON a.CompanyCode = e.CompanyCode  
  AND a.PartNo = e.PartNo  
 LEFT JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = d.CompanyCode  
  AND a.WarehouseCode = LookUpValue  
  AND f.CodeId = ''WRCD''  
   JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND SUBSTRING(CONVERT(VARCHAR, b.ReturnDate, 112),1,6) BETWEEN ''' + @YearPeriod+ RIGHT('0' + @MonthPeriod,2) 
 + ''' AND SUBSTRING(CONVERT(VARCHAR,GetDate(),112),1,6)  
 AND a.WarehouseCode = ''' + @Warehouse + '''  
 AND b.Status NOT IN ( ''3'', ''0'', ''1'')  
 ' + @PartNoPRetur + ' GROUP BY f.LookUpValueName,  
 a.PartNo,  
 e.PartName,  
 a.ReturnNo,  
 b.ReturnDate,  
 c.SupplierName,  
 d.BOMStock,  
 d.CostPrice,  
 a.CostPrice,  
 g.CostPrice,  
 d.EOMStock  
 
UNION  
  
--WAREHOUSE TRANSFER  
 -- Incoming (ToWarehouseCode has entry)  
 -- Outgoing (FromWarehouseCode has entry)  
 -- Pemasok/Pelanggan = ReasonCode  
  
SELECT --Incoming  
 e.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 d.PartName,  
 a.WHTrfNo Nomor,   
 b.WHTrfDate AS Tanggal,  
 CASE WHEN LEFT(b.ReferenceNo,3) = ''CLR'' THEN ''[REC.CLAIM]'' ELSE CASE WHEN LEFT(b.ReferenceNo,3) = ''CLM'' THEN ''[CLAIM]'' ELSE f.LookupValueName END END AS Pemasok,  
 a.Qty Penerimaan,  
 NULL Pengeluaran,  
 0 Qty,  
 0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''in'' RecStat,  
 '' '' Keterangan  
FROM spTrnIWHTrfDtl a  
JOIN spTrnIWHTrfHdr b  
 ON a.CompanyCode = b.CompanyCode  
 AND a.BranchCode = b.BranchCode  
 AND a.WHTrfNo = b.WHTrfNo  
JOIN SpHstStockMovement c  
 ON a.CompanyCode = c.CompanyCode  
 AND a.BranchCode = c.BranchCode   
 AND a.PartNo = c.PartNo  
 AND c.WarehouseCode = ''' + @Warehouse + '''  
 AND c.Month = ''' + @MonthPeriod + '''  
 AND c.Year = ''' + @YearPeriod +  '''   
JOIN spMstItemInfo d  
 ON a.CompanyCode = d.CompanyCode  
 AND a.PartNo = d.PartNo  
JOIN gnMstLookUpDtl e  
 ON a.CompanyCode = e.CompanyCode  
 AND a.ToWarehouseCode = e.LookUpValue  
 AND e.CodeId = ''WRCD''  
LEFT JOIN gnMstLookUpDtl f  
 ON a.CompanyCode = f.CompanyCode  
 AND a.ReasonCode = f.LookUpValue  
 AND f.CodeId = ''RSWT''  
JOIN spMstItemPrice g  
    ON a.CompanyCode = g.CompanyCode   
    AND a.BranchCode = g.BranchCode  
    AND a.PartNo = g.PartNo  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
    AND b.Status = 2  
 AND SUBSTRING(CONVERT(VARCHAR, b.WHTrfDate, 112),1,6) BETWEEN ''' + @YearPeriod+ RIGHT('0' + @MonthPeriod,2) + ''' AND SUBSTRING(CONVERT(VARCHAR,GetDate(),112),1,6)  
 AND a.ToWarehouseCode = ''' + @Warehouse + '''  
 ' + @PartNoWtr + '     
UNION  
  
SELECT --Outgoing  
 e.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 d.PartName,  
 a.WHTrfNo Nomor,   
 b.WHTrfDate AS Tanggal,  
 CASE WHEN LEFT(b.ReferenceNo,3) = ''CLR'' THEN ''[REC.CLAIM]'' ELSE CASE WHEN LEFT(b.ReferenceNo,3) = ''CLM'' THEN ''[CLAIM]'' ELSE f.LookupValueName END END AS Pemasok,  
 NULL Penerimaan,  
 a.Qty Pengeluaran,  
 0 Qty,  
 0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''out'' RecStat,  
 '' '' Keterangan  
FROM spTrnIWHTrfDtl a  
JOIN spTrnIWHTrfHdr b  
 ON a.CompanyCode = b.CompanyCode  
 AND a.BranchCode = b.BranchCode  
 AND a.WHTrfNo = b.WHTrfNo  
JOIN SpHstStockMovement c  
 ON a.CompanyCode = c.CompanyCode  
 AND a.BranchCode = c.BranchCode   
 AND a.PartNo = c.PartNo  
 AND c.WarehouseCode = ''' + @Warehouse + '''  
 AND c.Month = ''' + @MonthPeriod + '''  
 AND c.Year = ''' + @YearPeriod +  '''   
JOIN spMstItemInfo d  
 ON a.CompanyCode = d.CompanyCode  
 AND a.PartNo = d.PartNo  
JOIN gnMstLookUpDtl e  
 ON a.CompanyCode = e.CompanyCode  
 AND a.FromWarehouseCode = e.LookUpValue  
 AND e.CodeId = ''WRCD''  
LEFT JOIN gnMstLookUpDtl f  
 ON a.CompanyCode = f.CompanyCode  
 AND a.ReasonCode = f.LookUpValue  
 AND f.CodeId = ''RSWT''  
JOIN spMstItemPrice g  
    ON a.CompanyCode = g.CompanyCode   
    AND a.BranchCode = g.BranchCode  
 AND a.PartNo = g.PartNo  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
    AND b.Status = 2  
 AND SUBSTRING(CONVERT(VARCHAR, b.WHTrfDate, 112),1,6) BETWEEN ''' + @YearPeriod+ RIGHT('0' + @MonthPeriod,2) + ''' AND SUBSTRING(CONVERT(VARCHAR,GetDate(),112),1,6)  
 AND a.FromWarehouseCode = ''' + @Warehouse + '''  
 ' + @PartNoWtr1 + '   
UNION  
  
--SBPSF  
  
SELECT --Outgoing  
 f.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 e.PartName,  
 b.LmpNo Nomor,  
 b.LmpDate AS Tanggal,  
 c.CustomerName Pemasok,  
 NULL Penerimaan,  
 SUM(a.QtyBill) Pengeluaran,  
 0 Qty,  
 0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''out'' RecStat,  
 case h.UsageDocNo when '' ''  then h.DocNo else h.UsageDocNo end Keterangan  
FROM spTrnSLmpDtl a  
 JOIN spTrnSLmpHdr b  
  ON a.CompanyCode = b.CompanyCode  
  AND a.BranchCode = b.BranchCode  
  AND a.LmpNo = b.LmpNo  
 JOIN gnMstCustomer c  
  ON b.CompanyCode = c.CompanyCode  
  AND b.CustomerCode = c.CustomerCode  
 JOIN SpHstStockMovement d  
  ON a.CompanyCode = d.CompanyCode  
  AND a.BranchCode = d.BranchCode   
  AND a.PartNo = d.PartNo  
  AND a.WarehouseCode = d.WarehouseCode  
  AND d.Month = ''' + @MonthPeriod + '''  
  AND d.Year = ''' + @YearPeriod +  '''   
 JOIN spMstItemInfo e  
  ON a.CompanyCode = e.CompanyCode  
  AND a.PartNo = e.PartNo  
 LEFT JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = d.CompanyCode  
  AND a.WarehouseCode = LookUpValue  
  AND f.CodeId = ''WRCD''  
   JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
   LEFT JOIN spTrnSORDHdr h on h.CompanyCode=a.CompanyCode  
  AND h.BranchCode=a.BranchCode  
  AND h.DocNo=a.DocNo  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND SUBSTRING(CONVERT(VARCHAR, b.BPSFDate, 112),1,6) BETWEEN ''' + @YearPeriod+ RIGHT('0' + @MonthPeriod,2) + ''' AND SUBSTRING(CONVERT(VARCHAR,GetDate(),112),1,6)  
 AND a.WarehouseCode =  ''' + @Warehouse + '''  
 ' + @PartNoBPSF + ' GROUP BY f.LookUpValueName, a.PartNo,  
 e.PartName,  
 b.LmpNo,  
 b.LmpDate,  
 c.CustomerName,  
 d.BOMStock,  
 d.CostPrice,  
 a.CostPrice,  
    g.CostPrice,  
 d.EOMStock,  
 case h.UsageDocNo when '' ''  then h.DocNo else h.UsageDocNo end  
UNION  
  
--Retur  
  
SELECT  
  f.LookupValueName,  
  UPPER(a.PartNo) PartNo,  
  e.PartName,  
  b.ReturnNo Nomor,  
  a.ReturnDate AS Tanggal,   
  c.CustomerName Pemasok,  
  SUM(a.QtyReturn) Penerimaan,  
  NULL Pengeluaran,  
  0 Qty,  
  0 Pokok,  
  isnull(a.CostPrice,0) CostPrice,  
  isnull(g.CostPrice,0) ActCostPrice,  
  ''in'' RecStat,  
  '' '' Keterangan  
FROM spTrnSRturDtl a  
 JOIN spTrnSRturHdr b  
  ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode  
   AND a.ReturnNo = b.ReturnNo  
 JOIN gnMstCustomer c  
  ON b.CompanyCode = c.CompanyCode AND b.CustomerCode = c.CustomerCode  
 JOIN SpHstStockMovement d  
  ON a.CompanyCode = d.CompanyCode  
  AND a.BranchCode = d.BranchCode   
  AND a.PartNo = d.PartNo  
  AND a.WarehouseCode = d.WarehouseCode  
  AND d.Month = ''' + @MonthPeriod + '''  
  AND d.Year = ''' + @YearPeriod +  '''   
 JOIN spMstItemInfo e  
  ON a.CompanyCode = e.CompanyCode AND a.PartNo = e.PartNo  
 LEFT JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = d.CompanyCode  
  AND a.WarehouseCode = LookUpValue  
  AND f.CodeId = ''WRCD''  
   JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
WHERE  
 a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND SUBSTRING(CONVERT(VARCHAR, b.ReturnDate, 112),1,6) BETWEEN ''' + @YearPeriod+ RIGHT('0' + @MonthPeriod,2) + ''' AND SUBSTRING(CONVERT(VARCHAR,GetDate(),112),1,6)  
    ' + @PartNoRetur + ' AND a.WarehouseCode = ''' + @Warehouse + ''' AND b.Status = 2   
 GROUP BY f.LookUpValueName, a.PartNo,  
  e.PartName,  
  b.ReturnNo,  
  a.ReturnDate,  
  c.CustomerName,  
  d.BOMStock,  
  d.CostPrice,  
  a.CostPrice,  
  g.CostPrice,  
  d.EOMStock  
UNION  
  
--SupplySlip  
  
SELECT   
  f.LookupValueName,  
  UPPER(a.PartNo) PartNo,  
  e.PartName,    
  b.ReturnNo Nomor,  
  a.ReturnDate AS Tanggal,  
  c.CustomerName Pemasok,  
  SUM(a.QtyReturn) Penerimaan,  
  NULL Pengeluaran,   
  0 Qty,  
  0 Pokok,  
  isnull(a.CostPrice,0) CostPrice,  
  isnull(g.CostPrice,0) ActCostPrice,  
  ''in'' RecStat,  
  '' '' Keterangan    
FROM spTrnSRturSSDtl a  
 JOIN spTrnSRturSSHdr b  
  ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode  
   AND a.ReturnNo = b.ReturnNo  
 JOIN gnMstCustomer c  
  ON a.CompanyCode = c.CompanyCode AND b.CustomerCode = c.CustomerCode  
 JOIN SpHstStockMovement d  
  ON a.CompanyCode = d.CompanyCode  
  AND a.BranchCode = d.BranchCode   
  AND a.PartNo = d.PartNo  
  AND a.WarehouseCode = d.WarehouseCode  
  AND d.Month = ''' + @MonthPeriod + '''  
  AND d.Year = ''' + @YearPeriod +  '''   
 JOIN spMstItemInfo e  
  ON a.CompanyCode = e.CompanyCode AND a.PartNo = e.PartNo  
 LEFT JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = d.CompanyCode  
  AND a.WarehouseCode = LookUpValue  
  AND f.CodeId = ''WRCD''  
   JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
WHERE  
a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND SUBSTRING(CONVERT(VARCHAR, b.ReturnDate, 112),1,6) BETWEEN ''' + @YearPeriod+ RIGHT('0' + @MonthPeriod,2) + ''' AND SUBSTRING(CONVERT(VARCHAR,GetDate(),112),1,6)  
    ' + @PartNoSupplySlip + ' AND a.WarehouseCode = ''' + @Warehouse + '''  AND b.Status = 2  
 GROUP BY f.LookUpValueName, a.PartNo,  
  e.PartName,  
  b.ReturnNo,  
  a.ReturnDate,  
  c.CustomerName,  
  d.BOMStock,  
  d.CostPrice,  
  a.CostPrice,  
  g.CostPrice,  
  d.EOMStock  
UNION  
  
--Retur Service  
  
SELECT  
  f.LookupValueName,  
  UPPER(a.PartNo) PartNo,  
  e.PartName,  
  b.ReturnNo Nomor,  
  a.ReturnDate AS Tanggal,   
  c.CustomerName Pemasok,  
  SUM(a.QtyReturn) Penerimaan,  
  NULL Pengeluaran,  
  0 Qty,  
  0 Pokok,  
  isnull(a.CostPrice,0) CostPrice,  
  isnull(g.CostPrice,0) ActCostPrice,  
  ''in'' RecStat,  
  '' '' Keterangan   
from spTrnSRturSrvDtl a  
 left join spTrnSRturSrvHdr b on  
  a.CompanyCode = b.CompanyCode  
  and a.BranchCode = b.BranchCode  
  and a.ReturnNo = b.ReturnNo  
 JOIN gnMstCustomer c  
  ON b.CompanyCode = c.CompanyCode   
  AND b.CustomerCode = c.CustomerCode  
 JOIN SpHstStockMovement d  
  ON a.CompanyCode = d.CompanyCode  
  AND a.BranchCode = d.BranchCode   
  AND a.PartNo = d.PartNo  
  AND a.WarehouseCode = d.WarehouseCode  
  AND d.Month = ''' + @MonthPeriod + '''  
  AND d.Year = ''' + @YearPeriod +  '''   
 JOIN spMstItemInfo e  
  ON a.CompanyCode = e.CompanyCode   
  AND a.PartNo = e.PartNo  
 LEFT JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = d.CompanyCode  
  AND a.WarehouseCode = LookUpValue  
  AND f.CodeId = ''WRCD''  
   JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
WHERE  
 a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND SUBSTRING(CONVERT(VARCHAR, b.ReturnDate, 112),1,6) BETWEEN ''' + @YearPeriod+ RIGHT('0' + @MonthPeriod,2) + ''' AND SUBSTRING(CONVERT(VARCHAR,GetDate(),112),1,6)  
    ' + @PartNoRetur + ' AND a.WarehouseCode = ''' + @Warehouse + ''' AND b.Status = 2   
 GROUP BY f.LookUpValueName, a.PartNo,  
  e.PartName,  
  b.ReturnNo,  
  a.ReturnDate,  
  c.CustomerName,  
  d.BOMStock,  
  d.CostPrice,  
  a.CostPrice,  
  g.CostPrice,  
  d.EOMStock  
UNION   
SELECT --BOM  
 e.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 '''' PartName,  
 '''' Nomor,   
 '''' AS Tanggal,   
 ''-'' Pemasok,  
 0 Penerimaan,  
 0 Pengeluaran,  
 CASE WHEN ''' + @MonthPeriod + ''' = Month(getDate()) AND ''' + @YearPeriod + ''' = Year(getDate()) THEN a.BOMInvQty ELSE  c.BOMStock END AS Qty,  
 CASE WHEN ''' + @MonthPeriod + ''' = Month(getDate()) AND ''' + @YearPeriod + ''' = Year(getDate()) THEN a.BOMInvCostPrice ELSE  c.CostPrice END AS Pokok,  
 0 CostPrice,  
    0 ActCostPrice,  
 ''-'' RecStat,  
 '' '' Keterangan  
FROM spMstItems a   
LEFT JOIN gnMstLookUpDtl e  
  ON a.CompanyCode = e.CompanyCode  
  AND e.CodeId = ''WRCD'' AND e.LookupValue = ''' + @Warehouse + '''  
 JOIN SpHstStockMovement c  
  ON a.CompanyCode = c.CompanyCode  
  AND a.BranchCode = c.BranchCode   
  AND a.PartNo = c.PartNo  
  AND c.WarehouseCode = ''' + @Warehouse + '''  
  AND c.Month = ''' + @MonthPeriod + '''  
  AND c.Year = ''' + @YearPeriod +  '''  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''' + @PartNoItem + ') #spKartuStock  
  
select * into #spKartuStockGroup from  
(SELECT distinct partno from #spKartuStock) #spKartuStockGroup  
  
select * into #spKartuStockGroupID from (  
select partno , row_number() over (order by partno Asc) groupId FROM #spKartuStockGroup) #spKartuStockGroupID  
  
update #spKartuStock  
set groupId = b.groupId  
from #spKartuStock a, #spKartuStockGroupID b  
where a.partno = b.partno  
  
select * from #spKartuStock ORDER BY PartNo, TanggalSort ASC  
  
DROP TABLE #spKartuStock  
DROP TABLE #spKartuStockGroup  
DROP TABLE #spKartuStockGroupID  
'  
  
--=====================================================================================================================================  
  
set @QueryStockItem = '  
SELECT *,  
convert(varchar, Tanggal, 112) + replace(convert(varchar, Tanggal,108),'':'','''') TanggalSort,  
1 GroupID  
INTO #spKartuStock FROM (  
SELECT --Adjustment Incoming  
 e.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 d.PartName,  
 a.AdjustmentNo Nomor,   
 b.AdjustmentDate AS Tanggal,   
 f.LookupValueName Pemasok,  
 a.QtyAdjustment Penerimaan,  
 NULL Pengeluaran,  
 0 Qty,  
 0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''in'' RecStat,  
 '' '' Keterangan  
FROM spTrnIAdjustDtl a  
 JOIN spTrnIAdjustHdr b  
  ON a.CompanyCode = b.CompanyCode  
  AND a.BranchCode = b.BranchCode  
  AND a.AdjustmentNo = b.AdjustmentNo  
 JOIN spMstItems c  
  ON a.CompanyCode = c.CompanyCode  
  AND a.BranchCode = c.BranchCode   
  AND a.PartNo = c.PartNo  
 JOIN spMstItemInfo d  
  ON a.CompanyCode = d.CompanyCode  
  AND a.PartNo = d.PartNo  
 JOIN gnMstLookUpDtl e  
  ON a.CompanyCode = e.CompanyCode  
  AND a.WarehouseCode = e.LookUpValue  
  AND e.CodeId = ''WRCD''  
 JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = f.CompanyCode  
  AND a.ReasonCode = f.LookUpValue  
  AND f.CodeId = ''RSAD''  
    JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
WHERE a.AdjustmentCode = ''+''  
 AND a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND MONTH(b.AdjustmentDate) = ''' + @MonthPeriod + '''  
 AND YEAR(b.AdjustmentDate) = ''' + @YearPeriod + '''  
 AND a.WarehouseCode = ''' + @Warehouse + '''  
 ' + @PartNoAdjInc + '  
UNION  
  
SELECT --Adjustment Outgoing  
 e.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 d.PartName,  
 a.AdjustmentNo Nomor,   
 b.AdjustmentDate AS Tanggal,   
 f.LookupValueName Pemasok,  
 NULL Penerimaan,  
 a.QtyAdjustment Pengeluaran,  
 0 Qty,  
 0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''out'' RecStat,  
 '' '' Keterangan  
FROM spTrnIAdjustDtl a  
JOIN spTrnIAdjustHdr b  
 ON a.CompanyCode = b.CompanyCode  
 AND a.BranchCode = b.BranchCode  
 AND a.AdjustmentNo = b.AdjustmentNo  
JOIN spMstItems c  
 ON a.CompanyCode = c.CompanyCode  
 AND a.BranchCode = c.BranchCode   
 AND a.PartNo = c.PartNo  
JOIN spMstItemInfo d  
 ON a.CompanyCode = d.CompanyCode  
 AND a.PartNo = d.PartNo  
JOIN gnMstLookUpDtl e  
 ON a.CompanyCode = e.CompanyCode  
 AND a.WarehouseCode = e.LookUpValue  
 AND e.CodeId = ''WRCD''  
JOIN gnMstLookUpDtl f  
 ON a.CompanyCode = f.CompanyCode  
 AND a.ReasonCode = f.LookUpValue  
 AND f.CodeId = ''RSAD''  
JOIN spMstItemPrice g  
    ON a.CompanyCode = g.CompanyCode   
    AND a.BranchCode = g.BranchCode  
    AND a.PartNo = g.PartNo  
WHERE a.AdjustmentCode = ''-''   
 AND a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND MONTH(b.AdjustmentDate) = ''' + @MonthPeriod + '''  
 AND YEAR(b.AdjustmentDate) = ''' + @YearPeriod + '''  
 AND a.WarehouseCode = ''' + @Warehouse + '''  
 ' + @PartNoAdjOut + '   
UNION  
  
--RECEIVING  
-- - Incoming  
-- - Pemasok/Pelanggan = Supplier Name  
  
SELECT   
 f.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 e.PartName,  
 a.WRSNo Nomor,  
 b.WRSDate AS Tanggal,  
 c.SupplierName Pemasok,  
 SUM(a.ReceivedQty) Penerimaan,  
 NULL Pengeluaran,  
 0 Qty,  
 0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''in'' RecStat,  
 '' '' Keterangan  
FROM spTrnPRcvDtl a  
 JOIN spTrnPRcvHdr b  
  ON a.CompanyCode = b.CompanyCode   
  AND a.BranchCode = b.BranchCode  
  AND a.WRSNo = b.WRSNo  
 JOIN gnMstSupplier c  
  ON b.CompanyCode = c.CompanyCode  
  AND b.SupplierCode = c.SupplierCode   
 JOIN spMstItems d  
  ON a.CompanyCode = d.CompanyCode  
  AND a.BranchCode = d.BranchCode  
  AND a.PartNo = d.PartNo  
 JOIN spMstItemInfo e  
  ON a.CompanyCode = e.CompanyCode  
  AND a.PartNo = e.PartNo  
 JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = f.CompanyCode  
  AND a.WarehouseCode = f.LookUpValue  
  AND f.CodeId = ''WRCD''  
   JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND MONTH( b.WRSDate) = ''' + @MonthPeriod + '''  
 AND YEAR( b.WRSDate) = ''' + @YearPeriod + '''  
 AND a.WarehouseCode = ''' + @Warehouse + '''  
 AND b.Status NOT IN ( ''3'', ''0'', ''1'')  
 ' + @PartNoRcv + ' GROUP BY   
f.LookUpValueName,  
 a.PartNo,  
 e.PartName,  
 a.WRSNo,  
 b.WRSDate,  
 c.SupplierName,  
 a.CostPrice,  
    g.CostPrice  
   
UNION  
  
--FAKTUR PAJAK  
-- - Outgoing  
-- - Pemasok/Pelanggan = Customer name  
--  
SELECT   
 f.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 e.PartName,  
 a.FPJNo Nomor,  
 b.FPJDate AS Tanggal,  
 c.CustomerName Pemasok,  
 NULL Penerimaan,  
 SUM(a.QtyBill) Pengeluaran,  
 0 Qty,  
 0 Pokok,  
 isnull(A.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''out'' RecStat,  
 isnull(a.DocNo, '' '') Keterangan  
FROM spTrnSFPJDtl a  
JOIN spTrnSFPJHdr b  
 ON a.CompanyCode = b.CompanyCode  
 AND a.BranchCode = b.BranchCode  
 AND a.FPJNo = b.FPJNo  
JOIN gnMstCustomer c  
 ON b.CompanyCode = c.CompanyCode  
 AND b.CustomerCode = c.CustomerCode  
JOIN spMstItems d  
 ON a.CompanyCode = d.CompanyCode  
 AND a.BranchCode = d.BranchCode  
 AND a.PartNo = d.PartNo  
JOIN spMstItemInfo e  
 ON a.CompanyCode = e.CompanyCode  
 AND a.PartNo = e.PartNo  
JOIN gnMstLookUpDtl f  
 ON a.CompanyCode = f.CompanyCode  
 AND a.WarehouseCode = f.LookUpValue  
 AND f.CodeId = ''WRCD''  
JOIN spMstItemPrice g  
    ON a.CompanyCode = g.CompanyCode   
    AND a.BranchCode = g.BranchCode  
    AND a.PartNo = g.PartNo  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND MONTH(b.FPJDate) = ''' + @MonthPeriod + '''  
 AND YEAR(b.FPJDate) = ''' + @YearPeriod + '''  
 AND a.WarehouseCode = ''' + @Warehouse + '''  
 ' + @PartNoFpj + ' GROUP BY   
f.LookUpValueName,  
 a.PartNo,  
 e.PartName,  
 a.FPjNO,  
 b.FPJDate,  
 c.CustomerName,  
 a.CostPrice,  
    g.CostPrice,  
    a.DocNo  
UNION  
  
--WAREHOUSE TRANSFER  
 -- Incoming (ToWarehouseCode has entry)  
 -- Outgoing (FromWarehouseCode has entry)  
 -- Pemasok/Pelanggan = ReasonCode  
  
SELECT --Incoming  
 e.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 d.PartName,  
 a.WHTrfNo Nomor,   
 b.WHTrfDate AS Tanggal,  
 CASE WHEN LEFT(b.ReferenceNo,3) = ''CLR'' THEN ''[REC.CLAIM]''  ELSE CASE WHEN LEFT(b.ReferenceNo,3) = ''CLM'' THEN ''[CLAIM]'' ELSE f.LookupValueName END END AS Pemasok,  
 a.Qty Penerimaan,  
 NULL Pengeluaran,  
 0 Qty,  
 0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''in'' RecStat,  
 '' '' Keterangan  
FROM spTrnIWHTrfDtl a  
JOIN spTrnIWHTrfHdr b  
 ON a.CompanyCode = b.CompanyCode  
 AND a.BranchCode = b.BranchCode  
 AND a.WHTrfNo = b.WHTrfNo  
JOIN spMstItems c  
 ON a.CompanyCode = b.CompanyCode  
 AND a.BranchCode = b.BranchCode  
 AND a.PartNo = c.PartNo  
JOIN spMstItemInfo d  
 ON a.CompanyCode = d.CompanyCode  
 AND a.PartNo = d.PartNo  
JOIN gnMstLookUpDtl e  
 ON a.CompanyCode = e.CompanyCode  
 AND a.ToWarehouseCode = e.LookUpValue  
 AND e.CodeId = ''WRCD''  
LEFT JOIN gnMstLookUpDtl f  
 ON a.CompanyCode = f.CompanyCode  
 AND f.LookUpValue = a.ReasonCode   
 AND f.CodeId = ''RSWT''  
JOIN spMstItemPrice g  
    ON a.CompanyCode = g.CompanyCode   
    AND a.BranchCode = g.BranchCode  
    AND a.PartNo = g.PartNo  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND MONTH(b.WHTrfDate) = ''' + @MonthPeriod + '''  
 AND YEAR(b.WHTrfDate) = ''' + @YearPeriod + '''  
 AND a.ToWarehouseCode = ''' + @Warehouse + '''  
 ' + @PartNoWtr + '     
UNION  
  
SELECT   
 e.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 d.PartName,  
 a.WHTrfNo Nomor,   
 b.WHTrfDate AS Tanggal,  
 CASE WHEN LEFT(b.ReferenceNo,3) = ''CLR'' THEN ''[REC.CLAIM]'' ELSE CASE WHEN LEFT(b.ReferenceNo,3) = ''CLM'' THEN ''[CLAIM]'' ELSE f.LookupValueName END END AS Pemasok,  
 NULL Penerimaan,  
 a.Qty Pengeluaran,  
 0 Qty,  
 0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''out'' RecStat,  
 '' '' Keterangan  
FROM spTrnIWHTrfDtl a  
JOIN spTrnIWHTrfHdr b  
 ON a.CompanyCode = b.CompanyCode  
 AND a.BranchCode = b.BranchCode  
 AND a.WHTrfNo = b.WHTrfNo  
JOIN spMstItems c  
 ON a.CompanyCode = b.CompanyCode  
 AND a.BranchCode = b.BranchCode  
 AND a.PartNo = c.PartNo  
JOIN spMstItemInfo d  
 ON a.CompanyCode = d.CompanyCode  
 AND a.PartNo = d.PartNo  
JOIN gnMstLookUpDtl e  
 ON a.CompanyCode = e.CompanyCode  
 AND a.FromWarehouseCode = e.LookUpValue  
 AND e.CodeId = ''WRCD''  
LEFT JOIN gnMstLookUpDtl f  
 ON a.CompanyCode = f.CompanyCode  
 AND f.LookUpValue = a.ReasonCode   
 AND f.CodeId = ''RSWT''  
JOIN spMstItemPrice g  
    ON a.CompanyCode = g.CompanyCode   
    AND a.BranchCode = g.BranchCode  
    AND a.PartNo = g.PartNo  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND MONTH(b.WHTrfDate) = ''' + @MonthPeriod + '''  
 AND YEAR(b.WHTrfDate) = ''' + @YearPeriod + '''  
 AND a.FromWarehouseCode = ''' + @Warehouse + '''  
 ' + @PartNoWtr1 + '   
UNION  
  
--SBPSF  
  
SELECT --Outgoing  
 f.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 e.PartName,  
 b.LmpNo Nomor,  
 b.LmpDate AS Tanggal,  
 c.CustomerName Pemasok,  
 NULL Penerimaan,  
 SUM(a.QtyBill) Pengeluaran,  
 0 Qty,  
 0 Pokok,  
 isnull(a.CostPrice,0) CostPrice,  
    isnull(g.CostPrice,0) ActCostPrice,  
 ''out'' RecStat,  
 case h.UsageDocNo when '' '' then h.DocNo else h.UsageDocNo end Keterangan  
FROM spTrnSLmpDtl a  
 JOIN spTrnSLmpHdr b  
  ON a.CompanyCode = b.CompanyCode  
  AND a.BranchCode = b.BranchCode  
  AND a.LmpNo = b.LmpNo  
 JOIN gnMstCustomer c  
  ON b.CompanyCode = c.CompanyCode  
  AND b.CustomerCode = c.CustomerCode  
 JOIN spMstItems d  
  ON a.CompanyCode = d.CompanyCode  
  AND a.BranchCode = d.BranchCode  
  AND a.PartNo = d.PartNo  
 JOIN spMstItemInfo e  
  ON a.CompanyCode = e.CompanyCode  
  AND a.PartNo = e.PartNo  
 JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = f.CompanyCode  
  AND a.WarehouseCode = f.LookUpValue  
  AND f.CodeId = ''WRCD''  
   JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
   LEFT JOIN spTrnSORDHdr h on h.CompanyCode=a.CompanyCode  
  AND h.BranchCode=a.BranchCode  
  AND h.DocNo=a.DocNo  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND MONTH(b.BPSFDate) =  ''' + @MonthPeriod + '''  
 AND YEAR(b.BPSFDate) =  ''' + @YearPeriod + '''  
 AND a.WarehouseCode =  ''' + @Warehouse + '''  
 ' + @PartNoBPSF + '  
GROUP BY   
f.LookUpValueName,  
 a.PartNo,  
 e.PartName,  
 b.LmpNo,  
 b.LmpDate,  
 c.CustomerName,  
 a.CostPrice,  
    g.CostPrice,  
    case h.UsageDocNo when '' '' then h.DocNo else h.UsageDocNo end  
UNION  
  
--Retur  
  
SELECT  
  f.LookupValueName,  
  UPPER(a.PartNo) PartNo,  
  e.PartName,  
  b.ReturnNo Nomor,  
  a.ReturnDate AS Tanggal,   
  c.CustomerName Pemasok,  
  SUM(a.QtyReturn) Penerimaan,  
  NULL Pengeluaran,  
  0 Qty,  
  0 Pokok,  
  isnull(a.CostPrice,0) CostPrice,  
  isnull(g.CostPrice,0) ActCostPrice,  
  ''in'' RecStat,  
  '' '' Keterangan   
FROM spTrnSRturDtl a  
 JOIN spTrnSRturHdr b  
  ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode  
   AND a.ReturnNo = b.ReturnNo  
 JOIN gnMstCustomer c  
  ON b.CompanyCode = c.CompanyCode AND b.CustomerCode = c.CustomerCode  
 JOIN spMstItems d  
  ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode  
   AND a.PartNo = d.PartNo  
 JOIN spMstItemInfo e  
  ON a.CompanyCode = e.CompanyCode AND a.PartNo = e.PartNo  
 JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = f.CompanyCode  
  AND a.WarehouseCode = f.LookUpValue  
  AND f.CodeId = ''WRCD''  
   JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
WHERE  
 a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND MONTH(a.ReturnDate) = ''' + @MonthPeriod + '''  
 AND YEAR(a.ReturnDate) = ''' + @YearPeriod + '''  
    ' + @PartNoRetur + ' AND b.Status = 2  
 GROUP BY   
f.LookUpValueName,  
 a.PartNo,  
 e.PartName,  
 b.ReturnNo,  
 a.ReturnDate,  
 c.CustomerName,  
 a.CostPrice,  
    g.CostPrice  
  
UNION  
  
--SupplySlip  
  
SELECT   
  f.LookupValueName,  
  UPPER(a.PartNo) PartNo,  
  e.PartName,    
  b.ReturnNo Nomor,  
  a.ReturnDate AS Tanggal,  
  c.CustomerName Pemasok,  
  SUM(a.QtyReturn) Penerimaan,  
  NULL Pengeluaran,   
  0 Qty,  
  0 Pokok,  
  isnull(a.CostPrice,0) CostPrice,  
  isnull(g.CostPrice,0) ActCostPrice,  
  ''in'' RecStat,  
  '' '' Keterangan    
FROM spTrnSRturSSDtl a  
 JOIN spTrnSRturSSHdr b  
  ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode  
   AND a.ReturnNo = b.ReturnNo  
 JOIN gnMstCustomer c  
  ON a.CompanyCode = c.CompanyCode AND b.CustomerCode = c.CustomerCode  
 JOIN spMstItems d  
  ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode  
   AND a.PartNo = d.PartNo  
 JOIN spMstItemInfo e  
  ON a.CompanyCode = e.CompanyCode AND a.PartNo = e.PartNo  
 JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = f.CompanyCode  
  AND a.WarehouseCode = f.LookUpValue  
  AND f.CodeId = ''WRCD''  
   JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
WHERE  
a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND MONTH(a.ReturnDate) = ''' + @MonthPeriod + '''  
 AND YEAR(a.ReturnDate) = ''' + @YearPeriod + '''  
    ' + @PartNoSupplySlip + ' AND b.Status = 2   
 GROUP BY   
f.LookUpValueName,  
 a.PartNo,  
 e.PartName,  
 b.ReturnNo,  
 a.ReturnDate,  
 c.CustomerName,  
 a.CostPrice,  
    g.CostPrice  
UNION  
  
--Retur service  
  
SELECT  
  f.LookupValueName,  
  UPPER(a.PartNo) PartNo,  
  e.PartName,  
  b.ReturnNo Nomor,  
  a.ReturnDate AS Tanggal,   
  c.CustomerName Pemasok,  
  SUM(a.QtyReturn) Penerimaan,  
  NULL Pengeluaran,  
  0 Qty,  
  0 Pokok,  
  isnull(a.CostPrice,0) CostPrice,  
  isnull(g.CostPrice,0) ActCostPrice,  
  ''in'' RecStat,  
  '' '' Keterangan   
FROM spTrnSRturSrvDtl a  
 JOIN spTrnSRturSrvHdr b  
  ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode  
   AND a.ReturnNo = b.ReturnNo  
 JOIN gnMstCustomer c  
  ON b.CompanyCode = c.CompanyCode AND b.CustomerCode = c.CustomerCode  
 JOIN spMstItems d  
  ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode  
   AND a.PartNo = d.PartNo  
 JOIN spMstItemInfo e  
  ON a.CompanyCode = e.CompanyCode AND a.PartNo = e.PartNo  
 JOIN gnMstLookUpDtl f  
  ON a.CompanyCode = f.CompanyCode  
  AND a.WarehouseCode = f.LookUpValue  
  AND f.CodeId = ''WRCD''  
   JOIN spMstItemPrice g  
        ON a.CompanyCode = g.CompanyCode   
        AND a.BranchCode = g.BranchCode  
        AND a.PartNo = g.PartNo  
WHERE  
 a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''  
 AND MONTH(a.ReturnDate) = ''' + @MonthPeriod + '''  
 AND YEAR(a.ReturnDate) = ''' + @YearPeriod + '''  
    ' + @PartNoRetur + ' AND b.Status = 2  
 GROUP BY   
f.LookUpValueName,  
 a.PartNo,  
 e.PartName,  
 b.ReturnNo,  
 a.ReturnDate,  
 c.CustomerName,  
 a.CostPrice,  
    g.CostPrice  
UNION   
SELECT --BOM  
 e.LookUpValueName,  
 UPPER(a.PartNo) PartNo,  
 '''' PartName,  
 '''' Nomor,   
 '''' AS Tanggal,   
 ''-'' Pemasok,  
 0 Penerimaan,  
 0 Pengeluaran,  
 CASE WHEN ''' + @MonthPeriod + ''' = Month(getDate()) AND ''' + @YearPeriod + ''' = Year(getDate()) THEN a.BOMInvQty ELSE  c.BOMStock END AS Qty,  
 CASE WHEN ''' + @MonthPeriod + ''' = Month(getDate()) AND ''' + @YearPeriod + ''' = Year(getDate()) THEN a.BOMInvCostPrice ELSE  c.CostPrice END AS Pokok,  
 0 CostPrice,  
    0 ActCostPrice,  
 ''-'' RecStat,  
 '' '' Keterangan  
 FROM spMstItemLoc a   
LEFT JOIN gnMstLookUpDtl e  
  ON a.CompanyCode = e.CompanyCode  
  AND e.CodeId = ''WRCD'' AND e.LookupValue = ''' + @Warehouse + '''  
 LEFT JOIN SpHstStockMovement c  
  ON a.CompanyCode = c.CompanyCode  
  AND a.BranchCode = c.BranchCode   
  AND a.PartNo = c.PartNo  
  AND c.WarehouseCode = ''' + @Warehouse + '''  
  AND c.Month = ''' + @MonthPeriod + '''  
  AND c.Year = ''' + @YearPeriod +  '''  
WHERE a.CompanyCode = ''' + @CompanyCode + '''  
 AND a.BranchCode = ''' + @BranchCode + '''' + @PartNoItem + ' AND a.WarehouseCode =  ''' + @Warehouse + ''') #spKartuStock  
  
select * into #spKartuStockGroup from  
(SELECT distinct partno from #spKartuStock) #spKartuStockGroup  
  
select * into #spKartuStockGroupID from (  
select partno , row_number() over (order by partno Asc) groupId FROM #spKartuStockGroup) #spKartuStockGroupID  
  
update #spKartuStock  
set groupId = b.groupId  
from #spKartuStock a, #spKartuStockGroupID b  
where a.partno = b.partno  
  
select * from #spKartuStock ORDER BY PartNo, TanggalSort ASC  
  
DROP TABLE #spKartuStock  
DROP TABLE #spKartuStockGroup  
DROP TABLE #spKartuStockGroupID  
'  
  
IF @YearPeriod = YEAR(GETDATE()) AND @MonthPeriod = MONTH(GETDATE())  
 BEGIN  
  EXEC(@QueryStockItem);  
  PRINT(@QueryStockItem);   
 END  
ELSE  
 IF @isKartuStok = 0   
  BEGIN   
   EXEC(@QueryStock);  
   PRINT(@QueryStock);   
     
  END   
 ELSE  
  EXEC(@QueryStockItem);  
  PRINT(@QueryStockItem);   
END  


