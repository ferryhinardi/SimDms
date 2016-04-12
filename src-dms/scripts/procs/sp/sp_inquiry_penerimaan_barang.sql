create procedure uspfn_sp_inquiry_penerimaan_barang_detail
 @companycode varchar(15), @branchcode varchar(15), @BinningNo varchar(25)
 AS
SELECT
    ROW_NUMBER() OVER(ORDER BY a.DocNo) RowNumber
	, BinningNo
	, a.DocNo
	, a.PartNo
	, (SELECT PartName FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) PartName
	, ISNULL(a.ReceivedQty, 0) ReceivedBinning
	, ISNULL(WRS.ReceivedQty, 0) ReceivedWRS
	, ISNULL(WRS.PurchasePrice, 0) PurchasePrice
	, ISNULL(WRS.PurchaseAmt, 0) PurchaseAmt
	, ISNULL(WRS.DiscPct, 0) DiscPct
	, ISNULL(WRS.DiscAmt, 0) DiscAmt
	, ISNULL(WRS.TotalAmt, 0) TotalAmt
FROM
	spTrnPBinnDtl a WITH(NOLOCK, NOWAIT)
	LEFT JOIN (
		SELECT
			CompanyCode, BranchCode, DocNo, PartNo, ReceivedQty, PurchasePrice, DiscPct, 
			ReceivedQty * PurchasePrice AS PurchaseAmt, 
			ROUND((ReceivedQty * PurchasePrice * DiscPct / 100), 0) AS DiscAmt,
			ReceivedQty * PurchasePrice - ROUND((ReceivedQty * PurchasePrice * DiscPct / 100), 0) TotalAmt
		FROM
			SpTrnPRcvDtl WITH(NOLOCK, NOWAIT)
	) WRS ON a.CompanyCode = WRS.CompanyCode
		AND a.BranchCode = WRS.BranchCode
		AND a.DocNo = WRS.DocNo
		AND a.PartNo = WRS.PartNo
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.BinningNo = @BinningNo