CREATE view OmTrPurchaseBPUView
as
	SELECT bpu.*, sup.SupplierName as ExpeditionName, ref.RefferenceDesc1 as WarehouseName, '' as SupplierName
	, (CASE ISNULL(bpu.BPUType, '0') WHEN '0' THEN 'DO' WHEN '1' THEN 'SJ' WHEN '2' THEN 'DO & SJ' WHEN '3' THEN 'SJ Booking' END) AS Tipe
	, (CASE ISNULL(bpu.Status, '0') WHEN '0' THEN 'OPEN' WHEN '1' THEN 'PRINTED' WHEN '2' THEN 'APPROVED' WHEN '3' THEN 'CANCELED' WHEN '9' THEN 'FINISHED' END) AS StatusBPU
	FROM OmTrPurchaseBPU bpu
	LEFT JOIN GnMstSupplier sup ON bpu.CompanyCode = sup.CompanyCode AND bpu.Expedition = sup.SupplierCode
	LEFT JOIN omMstRefference ref ON bpu.CompanyCode = ref.CompanyCode AND bpu.WarehouseCode = ref.RefferenceCode AND ref.RefferenceType = 'WARE'