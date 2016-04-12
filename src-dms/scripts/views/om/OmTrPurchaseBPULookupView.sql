CREATE view [dbo].[OmTrPurchaseBPULookupView]
as
	SELECT bpu.*, sup.SupplierName
	, (CASE ISNULL(bpu.BPUType, '0') WHEN '0' THEN 'DO' WHEN '1' THEN 'SJ' WHEN '2' THEN 'DO & SJ' WHEN '3' THEN 'SJ Booking' END) AS Tipe
	, (CASE ISNULL(bpu.Status, '0') WHEN '0' THEN 'OPEN' WHEN '1' THEN 'PRINTED' WHEN '2' THEN 'APPROVED' WHEN '3' THEN 'CANCELED' WHEN '9' THEN 'FINISHED' END) AS StatusBPU
	, '' as ExpeditionName, '' as WarehouseName
	FROM OmTrPurchaseBPU bpu
	LEFT JOIN GnMstSupplier sup ON bpu.CompanyCode = sup.CompanyCode AND bpu.SupplierCode = sup.SupplierCode