CREATE procedure [dbo].[uspfn_OmGetStockDataTable]
	@CompanyCode varchar(20),
	@DateBegin datetime,
	@DateEnd datetime
AS
--exec uspfn_OmGetStockDataTable '6006406','20120101','20121230'
BEGIN
	SELECT '1'  AS RecordID, b.BPUDate  AS transactionDate, CASE
			  WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = b.CompanyCode AND SupplierCode = b.SupplierCode)  = '2000000'
		   THEN
				 'B1'
			  ELSE
				 'B2'
		   END  AS transactionType, '' AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, b.RefferenceDONo AS DONo,

		   (CASE WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = b.CompanyCode AND SupplierCode = b.SupplierCode)  = '2000000'
			THEN '' ELSE b.SupplierCode END)  AS 'Supplier_CustomerCode', (
		   SELECT c.SupplierName
		   FROM gnMstSupplier c
		   WHERE b.SupplierCode = c.SupplierCode)  AS 'Supplier_CustomerName'
	  FROM omMstVehicle a
		LEFT JOIN omTrPurchaseBPUDetail c on a.CompanyCode = c.CompanyCode and a.ChassisCode = c.ChassisCode and a.ChassisNo = c.ChassisNo
		LEFT JOIN omTrPurchaseBPU b on b.CompanyCode = c.CompanyCode and b.BranchCode = c.BranchCode and b.BPUNo = c.BPUNo
	 WHERE a.CompanyCode = b.CompanyCode
		   AND a.BPUNo = b.BPUNo
		   AND CONVERT(VARCHAR, b.BPUDate, 112) BETWEEN @DateBegin AND @DateEnd
		   AND b.CompanyCode = @CompanyCode 

	UNION
	SELECT '1'  AS RecordID, b.BPUDate  AS transactionDate, CASE
			  WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = b.CompanyCode AND SupplierCode = b.SupplierCode)  = '2000000'
		   THEN
				 'B1'
			  ELSE
				 'B2'
		   END  AS transactionType, '' AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, b.RefferenceDONo AS DONo,
			(CASE WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = b.CompanyCode AND SupplierCode = b.SupplierCode)  = '2000000'
			THEN '' ELSE b.SupplierCode END) AS 'Supplier_CustomerCode', (
		   SELECT c.SupplierName
		   FROM gnMstSupplier c
		   WHERE b.SupplierCode = c.SupplierCode)  AS 'Supplier_CustomerName'
	  FROM omMstVehicleTemp a, omTrPurchaseBPU b
	 WHERE a.CompanyCode = b.CompanyCode
		   AND a.BPUNo = b.BPUNo AND a.IsActive = 1
		   AND CONVERT(VARCHAR, b.BPUDate, 112) BETWEEN @DateBegin AND @DateEnd
		   AND b.CompanyCode = @CompanyCode 


	UNION
	SELECT '1'  AS RecordID, b.ReturnDate  AS transactionDate, CASE
			  WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = x.CompanyCode AND SupplierCode = x.SupplierCode) = '2000000'
		   THEN
				 'R1'
			  ELSE
				 'R2'
		   END  AS transactionType, '' AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, x.RefferenceDoNo AS DONo,
			(CASE WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = b.CompanyCode AND SupplierCode = x.SupplierCode)  = '2000000'
			THEN '' ELSE x.SupplierCode END) AS 'Supplier_CustomerCode', (
		   SELECT c.SupplierName
		   FROM gnMstSupplier c
		   WHERE x.SupplierCode = c.SupplierCode)  AS 'Supplier_CustomerName'
	 FROM omMstVehicle a
	 INNER JOIN omTrPurchaseReturn b ON b.CompanyCode = a.CompanyCode AND b.ReturnNo = a.POReturnNo AND b.HPPNo = a.HPPNo
	 LEFT JOIN omTrPurchaseBPU x ON x.CompanyCode = a.CompanyCode AND x.BranchCode = b.BranchCode AND x.BPUNo = a.BPUNo
	 WHERE CONVERT(VARCHAR, b.ReturnDate, 112) BETWEEN @DateBegin AND @DateEnd
		   AND a.CompanyCode = @CompanyCode 
	UNION
	SELECT '1'  AS RecordID, b.ReturnDate  AS transactionDate, CASE
			  WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = x.CompanyCode AND SupplierCode = x.SupplierCode) = '2000000'
		   THEN
				 'R1'
			  ELSE
				 'R2'
		   END  AS transactionType, '' AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, 
		   (CASE WHEN charindex('-', a.RefDoNo) = 0 THEN a.RefDONo ELSE  SUBSTRING(a.RefDoNo, 0, charindex('-', a.RefDoNo) ) END) AS DONo,
			(CASE WHEN (SELECT StandardCode FROM GnMstSupplier WHERE CompanyCode = b.CompanyCode AND SupplierCode = x.SupplierCode)  = '2000000'
			THEN '' ELSE x.SupplierCode END) AS 'Supplier_CustomerCode', (
		   SELECT c.SupplierName
		   FROM gnMstSupplier c
		   WHERE x.SupplierCode = c.SupplierCode)  AS 'Supplier_CustomerName'
	  FROM omMstVehicleTemp a
	 INNER JOIN omTrPurchaseReturn b ON b.CompanyCode = a.CompanyCode AND b.ReturnNo = a.POReturnNo AND b.HPPNo = a.HPPNo
	 LEFT JOIN omTrPurchaseBPU x ON x.CompanyCode = a.CompanyCode AND x.BranchCode = b.BranchCode AND x.BPUNo = a.BPUNo
	 WHERE CONVERT(VARCHAR, b.ReturnDate, 112) BETWEEN @DateBegin AND @DateEnd
		   AND a.CompanyCode = @CompanyCode 

	UNION
	SELECT '1'  AS RecordID, b.SODate  AS transactionDate, 
		   CASE WHEN b.SalesType = '1' THEN 'S1' ELSE 'S2' END AS transactionType, ''  AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, x.RefferenceDoNo AS DONo,
		   ''  AS 'Supplier_CustomerCode', (
		   SELECT c.CustomerName
		   FROM gnMstCustomer c
		   WHERE b.CustomerCode = c.CustomerCode)  AS 'Supplier_CustomerName'
	  FROM omMstVehicle a 
	  INNER JOIN omTrSalesSO b
	  ON a.CompanyCode = b.CompanyCode
	  AND a.SONo = b.SONo
	  LEFT JOIN omTrPurchaseBPUDetail y ON a.CompanyCode = y.CompanyCode and a.ChassisCOde = y.ChassisCode AND a.ChassisNo = y.ChassisNo
	  LEFT JOIN omTrPurchaseBPU x ON x.CompanyCode = y.CompanyCode AND x.BranchCode = y.BranchCode AND x.BPUNo = y.BPUNo
	  WHERE CONVERT(VARCHAR, b.SODate, 112) BETWEEN @DateBegin AND @DateEnd  
	  AND b.CompanyCode = @CompanyCode 
	UNION
	SELECT '1'  AS RecordID, b.ReqDate  AS transactionDate, 
	CASE WHEN b.StatusFaktur = '1' AND b.SubDealerCode = b.CompanyCode THEN 'F1' ELSE 
	(CASE WHEN b.StatusFaktur = '1' AND b.SubDealerCode <> b.CompanyCode THEN 'F2' ELSE 
	(CASE WHEN b.StatusFaktur <> '1' AND b.SubDealerCode = b.CompanyCode THEN 'F3' ELSE 'F4' END) END) END
	AS transactionType, (SELECT TOP 1 z.ReasonCode
		   FROM omTrSalesReqDetail z
		   WHERE z.ChassisNo = a.ChassisNo
		   AND z.ChassisCode = a.ChassisCode)  AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, x.RefferenceDoNo AS DONo,
			(CASE WHEN b.SubDealerCode  = a.CompanyCode THEN '' ELSE b.SubDealerCode END) AS 'Supplier_CustomerCode'
		   , (
		   SELECT c.CustomerName
		   FROM gnMstCustomer c
		   WHERE b.SubDealerCode = c.CustomerCode)  AS 'Supplier_CustomerName'
	  FROM omMstVehicle a
	  INNER JOIN omTrSalesReqDetail z ON a.CompanyCode = z.CompanyCode AND a.ChassisCode = z.ChassisCode AND a.ChassisNo = z.ChassisNo
	  INNER JOIN omTrSalesReq b	ON b.CompanyCode = z.CompanyCode AND b.BranchCode = z.BranchCode AND b.ReqNo = z.ReqNo
	  LEFT JOIN omTrPurchaseBPUDetail y
		ON y.CompanyCode = a.CompanyCode AND y.ChassisCode = a.ChassisCode AND y.ChassisNo = a.ChassisNo
	  LEFT JOIN omTrPurchaseBPU x 
		ON x.CompanyCode = y.CompanyCode AND x.BranchCode = y.BranchCode AND x.BPUNo = y.BPUNo
	  WHERE CONVERT(VARCHAR, b.ReqDate, 112) BETWEEN @DateBegin AND @DateEnd
	  AND b.CompanyCode = @CompanyCode 
	UNION
	SELECT '1'  AS RecordID, b.ReturnDate  AS transactionDate, 'U1'  AS
		   transactionType, ''  AS ReasonCode, a.ChassisCode  AS
		   ChassisCode, a.ChassisNo  AS ChassisNo, a.EngineCode  AS EngineCode, a.EngineNo  AS EngineNo, x.RefferenceDoNo AS DONo,
		   ''  AS 'Supplier_CustomerCode', (
		   SELECT c.CustomerName
		   FROM gnMstCustomer c
		   WHERE b.CustomerCode = c.CustomerCode)  AS 'Supplier_CustomerName'
	  FROM omMstVehicle a
	  INNER JOIN omTrSalesReturn b
	  ON a.CompanyCode = b.CompanyCode
	  AND a.SOReturnNo = b.ReturnNo
	LEFT JOIN omTrPurchaseBPUDetail y ON y.CompanyCode = a.CompanyCode AND y.ChassisCode = a.ChassisCode AND y.ChassisNo = a.ChassisNo
	LEFT JOIN omTrPurchaseBPU x ON x.CompanyCode = y.CompanyCode AND x.BranchCode = y.BranchCode AND x.BPUNo = y.BPUNo
	  WHERE CONVERT(VARCHAR, b.ReturnDate, 112) BETWEEN @DateBegin AND @DateEnd
	  AND b.CompanyCode = @CompanyCode;
END