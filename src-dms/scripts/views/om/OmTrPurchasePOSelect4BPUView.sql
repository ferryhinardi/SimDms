CREATE VIEW OmTrPurchasePOSelect4BPUView
AS
SELECT PO.*, sup.SupplierName FROM omTrPurchasePO PO
LEFT JOIN gnMstSupplier sup ON PO.CompanyCode = sup.CompanyCode
AND PO.SupplierCode = sup.SupplierCode
WHERE (
	SELECT SUM(ISNULL(QuantityPO, 0)) - SUM(ISNULL(QuantityBPU, 0)) 
	FROM omTrPurchasePOModel
	WHERE CompanyCode = PO.CompanyCode AND BranchCode = PO.BranchCode
	AND PONo = PO.PONo
) > 0