if object_id('uspfn_GenerateBPSLampiran') is not null
	drop procedure uspfn_GenerateBPSLampiran
GO
CREATE procedure [dbo].[uspfn_GenerateBPSLampiran]
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@PickedBy		VARCHAR(MAX)
AS
BEGIN

/*
PSEUDOCODE PROCESS :
Line 38  : RE-CALCULATE AMOUNT DETAIL
Line 93  : RE-CALCULATE AMOUNT HEADER AND UPDATE STATUS
Line 140 : UPDATE SO SUPPLY AND STATUS HEADER 
Line 167 : UPDATE SUPPLY SLIP QTY SERVICE 
Line 237 : INSERT NEW SRV ITEM BASED PICKING LIST
Line 276 : INSERT BPS AND LAMPIRAN
Line 292 : INSERT BPS HEADER
Line 352 : INSERT BPS DETAIL
Line 395 : INSERT LAMPIRAN HEADER
Line 458 : INSERT LAMPIRAN DETAIL
Line 500 : UPDATE STOCK
Line 571 : UPDATE DEMAND CUST AND DEMAND ITEM
Line 611 : INSERT TO ITEM MOVEMENT
Line 650 : UPDATE TRANSDATE
*/

--===============================================================================================================================
-- RE-CALCULATE AMOUNT DETAIL
--===============================================================================================================================
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

SELECT * INTO #TempPickingSlipDtl FROM (
SELECT
	a.CompanyCode
	, a.BranchCode 
	, a.PickingSlipNo
	, a.PickingSlipDate
	, a.CustomerCode
	, a.TypeOfGoods
	, b.DocNo
	, b.PartNo
	, b.QtyPicked
	, b.QtyPicked * b.RetailPrice SalesAmt
	, Floor((b.QtyPicked * b.RetailPrice) * DiscPct/100) DiscAmt
	, (b.QtyPicked * b.RetailPrice) - Floor((b.QtyPicked * b.RetailPrice) * DiscPct/100) NetSalesAmt
FROM SpTrnSPickingHdr a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode AND a.PickingSlipNo = b.PickingSlipNo
WHERE 
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND Status < 2
	AND b.DocNo IN (SELECT DocNo FROM SpTrnSordHdr WITH (NOLOCK, NOWAIT)
				WHERE 
					1 = 1
					AND CompanyCode =@CompanyCode
					AND BranchCode = @BranchCode
					AND UsageDocNo = @JobOrderNo
					AND Status = 4)
) #TempPickingSlipDtl


UPDATE SpTrnSPickingDtl
SET SalesAmt = b.SalesAmt 
	, DiscAmt = b.DiscAmt
	, NetSalesAmt = b.NetSalesAmt
	, TotSalesAmt = b.NetSalesAmt
	, QtyBill = b.QtyPicked
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpTrnSPickingDtl a, #TempPickingSlipDtl b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PickingSlipNo = b.PickingSlipNo
	AND a.PartNo = b.PartNo

--===============================================================================================================================
-- RE-CALCULATE AMOUNT HEADER AND UPDATE STATUS
--===============================================================================================================================

SELECT * INTO #TempPickingSlipHdr FROM (
SELECT
	a.CompanyCode
	, a.BranchCode
	, a.PickingSlipNo	
	, SUM(b.QtyPicked) TotSalesQty
	, SUM(b.SalesAmt) TotSalesAmt
	, SUM(b.DiscAmt) TotDiscAmt
	, SUM(b.NetSalesAmt) TotDPPAmt
	, floor(SUM(b.NetSalesAmt) * (ISNULL((SELECT TaxPct FROM GnMstTax x WITH (NOLOCK, NOWAIT) WHERE x.CompanyCode = @CompanyCode AND x.TaxCode IN 
		(SELECT TaxCode FROM GnMstCustomerProfitCenter y WITH (NOLOCK, NOWAIT) WHERE y.CompanyCode = @CompanyCode AND y.BranchCode = @BranchCode
			AND y.ProfitCenterCode = '300' AND y.CustomerCode = @CustomerCode)),0)/100))
	  TotPPNAmt
FROM spTrnSPickingHdr a WITH (NOLOCK, NOWAIT)
LEFT JOIN spTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PickingSlipNo = b.PickingSlipNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.PickingSlipNo IN (SELECT DISTINCT(PickingSlipNo) FROM #TempPickingSlipDtl WITH (NOLOCK, NOWAIT))
GROUP BY a.CompanyCode
	, a.BranchCode
	, a.PickingSlipNo	
) #TempPickingSlipHdr

UPDATE SpTrnSPickingHdr
SET TotSalesQty = b.TotSalesQty
	, TotSalesAmt = b.TotSalesAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalSalesAmt = b.TotDPPAmt + b.TotPPNAmt
	, Status = 2
	, PickedBy = @PickedBy
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpTrnSPickingHdr a, #TempPickingSlipHdr b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PickingSlipNo = b.PickingSlipNo

--===============================================================================================================================
-- UPDATE SO SUPPLY AND STATUS HEADER 
--===============================================================================================================================
UPDATE SpTrnSOSupply
SET	Status = 2
	, QtyPicked = b.QtyPicked
	, QtyBill = b.QtyPicked
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpTrnSOSupply a, #TempPickingSlipDtl b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.DocNo = b.DocNo
	AND a.PartNo = b.PartNo

UPDATE SpTrnSORDHdr 
SET Status = 5
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo IN (SELECT DISTINCT(DocNo) FROM #TempPickingSlipDtl)

--===============================================================================================================================
-- UPDATE SUPPLY SLIP QTY SERVICE 
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo)

SELECT * INTO #TempServiceItem FROM (
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtyBill
	, b.DocNo
	, c.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo AND a.SupplySlipNo = b.DocNo
INNER JOIN SpMstItemPrice c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.PartNo = c.PartNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND a.SupplySlipNo = b .DocNo
) #TempServiceItem 

UPDATE svTrnSrvItem
SET SupplyQty = (CASE WHEN b.QtyBill > b.DemandQty 
				THEN 
					CASE WHEN b.DemandQty = 0 THEN b.QtyBill ELSE b.DemandQty END
				ELSE b.QtyBill END)
	, LastUpdateBy = @UserID
	, LastUpdateDate = Getdate()
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq
	AND a.SupplySlipNo = b.DocNo

UPDATE svTrnSrvItem
SET CostPrice = b.CostPrice
	, LastUpdateBy = @UserID
	, LastUpdateDate = Getdate()
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.SupplySlipNo = b.DocNo

--===============================================================================================================================
-- INSERT NEW SRV ITEM BASED PICKING LIST
--===============================================================================================================================
INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq + 1
	, 0 DemandQty
	, a.QtyBill - a.DemandQty SupplyQty
	, 0 ReturnQty
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DocNo SupplySlipNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND DocNo = a.DocNo) SupplySlipDate
	, NULL SSReturnNo
	, NULL SSReturnDate
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, a.DiscPct
FROM #TempServiceItem a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.DemandQty < a.QtyBill
	AND a.QtyBill > 0
	AND a.DemandQty > 0

DROP TABLE #TempServiceItem 

--===============================================================================================================================
-- INSERT BPS AND LAMPIRAN
--===============================================================================================================================
DECLARE @PickingSlipNo	VARCHAR(MAX)
DECLARE	@TempBPSFNo		VARCHAR(MAX)
DECLARE	@TempLMPNo		VARCHAR(MAX)
DECLARE @MaxBPSFNo		INT
DECLARE @MaxLmpNo		INT

DECLARE db_cursor CURSOR FOR
SELECT DISTINCT PickingSlipNo FROM #TempPickingSlipHdr
OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @PickingSlipNo

WHILE @@FETCH_STATUS = 0
BEGIN	
--===============================================================================================================================
-- INSERT BPS HEADER
--===============================================================================================================================
SET @MaxBPSFNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'BPF' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempBPSFNo = ISNULL((SELECT 'BPF/' + RIGHT(YEAR(GETDATE()),2) + '/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxBPSFNo, 1), 6)),'BPF/YY/XXXXXX')

INSERT INTO SpTrnSBPSFHdr
SELECT 
	CompanyCode
	, BranchCode
	, @TempBPSFNo BPSFNo
	, GetDate() BPSFDate
	, PickingSlipNo
	, PickingSlipDate
	, TransType
	, SalesType
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, '2' Status
	, 0 PrintSeq
	, TypeOfGoods
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSPickingHdr WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND PickingSlipNo = @PickingSlipNo

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'BPF'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT BPS DETAIL
--===============================================================================================================================
INSERT INTO SpTrnSBPSFDtl
SELECT
	CompanyCode
	, BranchCode
	, @TempBPSFNo
	, WarehouseCode
	, PartNo
	, PartNoOriginal
	, DocNo
	, DocDate
	, ReferenceNo
	, ReferenceDate
	, LocationCode
	, QtyBill
	, RetailPriceInclTax
	, RetailPrice
	, CostPrice
	, DiscPct
	, SalesAmt
	, DiscAmt
	, NetSalesAmt
	, 0 PPNAmt
	, TotSalesAmt
	, ProductType
	, PartCategory 
	, MovingCode
	, ABCClass
	, '' ExPickingListNo
	, @DefaultDate ExPickingListDate
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSPickingDtl WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND PickingSlipNo = @PickingSlipNo

--===============================================================================================================================
-- INSERT LAMPIRAN HEADER
--===============================================================================================================================
SET @MaxLmpNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'LMP' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempLmpNo = ISNULL((SELECT 'LMP/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxLmpNo, 1), 6)),'LMP/YY/XXXXXX')

INSERT INTO SpTrnSLmpHdr
SELECT
	CompanyCode
	, BranchCode
	, @TempLmpNo LmpNo	
	, GetDate() LmpDate
	, @TempBPSFNo BPSFNo
	, (SELECT BPSFDate FROM SpTrnSBPSFHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND BPSFNo = @TempBPSFNo)
		BPSFDate
	, PickingSlipNo
	, PickingSlipDate
	, TransType
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, CONVERT(BIT, 0) isPKP
	, '0' Status
	, 0 PrintSeq
	, TypeOfGoods
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL IsLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSPickingHdr WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND PickingSlipNo = @PickingSlipNo

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'LMP'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT LAMPIRAN DETAIL
--===============================================================================================================================
INSERT INTO SpTrnSLmpDtl
SELECT
	a.CompanyCode
	, a.BranchCode
	, @TempLmpNo LmpNo
	, a.WarehouseCode
	, a.PartNo
	, a.PartNoOriginal
	, a.DocNo
	, a.DocDate
	, a.ReferenceNo
	, a.ReferenceDate
	, a.LocationCode
	, a.QtyBill
	, a.RetailPriceInclTax
	, a.RetailPrice
	, ISNULL((SELECT CostPrice FROM SpMstItemPrice WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo),0) CostPrice
	, a.DiscPct
	, a.SalesAmt
	, a.DiscAmt
	, a.NetSalesAmt
	, 0 PPNAmt
	, a.TotSalesAmt
	, a.ProductType
	, a.PartCategory 
	, a.MovingCode
	, a.ABCClass
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSPickingDtl a WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.PickingSlipNo = @PickingSlipNo
	AND a.QtyPicked > 0

--===============================================================================================================================
-- UPDATE STOCK
--===============================================================================================================================

--===============================================================================================================================
-- VALIDATION QTY
--===============================================================================================================================
	DECLARE @Onhand_SRValid NUMERIC(18,2)	
	DECLARE @Allocation_SRValid NUMERIC(18,2)
	DECLARE @errmsg VARCHAR(MAX)
	
	SELECT * INTO #Valid_2 FROM(
	SELECT a.PartNo
		, a.AllocationSR - b.QtyBill QtyValidSR
		, a.Onhand - b.QtyBill QtyValidOnhand
	FROM SpMstItems a, SpTrnSPickingDtl b
	WHERE 1 = 1
		AND a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND b.PickingSlipNo = @PickingSlipNo
		AND a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.PartNo = b.PartNo) #Valid_2

	SET @Allocation_SRValid = ISNULL((SELECT TOP 1 QtyValidSR FROM #Valid_2 WHERE QtyValidSR < 0),0)
	SET @Onhand_SRValid = ISNULL((SELECT TOP 1 QtyValidOnhand FROM #Valid_2 WHERE QtyValidOnhand < 0),0)

	DROP TABLE #Valid_2

	IF (@Onhand_SRValid < 0 OR @Allocation_SRValid < 0)
	BEGIN
		SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
		RAISERROR (@errmsg,16,1);
		RETURN
	END
--===============================================================================================================================

UPDATE SpMstItems
SET
	AllocationSR = AllocationSR - b.QtySupply
	, Onhand = Onhand - b.QtyBill
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
	, LastSalesDate = GetDate()
FROM SpMstItems a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND b.PickingSlipNo = @PickingSlipNo
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo

UPDATE SpMstItemLoc
SET
	AllocationSR = AllocationSR - b.QtySupply
	, Onhand = Onhand - b.QtyBill
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpMstItemLoc a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.WarehouseCode = '00'
	AND b.PickingSlipNo = @PickingSlipNo
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo

--===============================================================================================================================
-- UPDATE DEMAND CUST AND DEMAND ITEM
--===============================================================================================================================
UPDATE SpHstDemandCust
SET SalesFreq = SalesFreq + 1
	, SalesQty = SalesQty + b.QtyBill
	, LastUpdateBy = @UserID 
	, LastUpdateDate = GetDate()
FROM SpHstDemandCust a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND b.PickingSlipNo = @PickingSlipNo
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = Year(b.DocDate)
	AND a.Month = Month(b.DocDate)
	AND a.CustomerCode IN (SELECT CustomerCode FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = BranchCode
							AND PickingSlipNo = @PickingSlipNo)
	AND a.PartNo = b.PartNo
	

UPDATE SpHstDemandItem
SET SalesFreq = SalesFreq + 1
	, SalesQty = SalesQty + b.QtyBill
	, LastUpdateBy = @UserID 
	, LastUpdateDate = GetDate()
FROM SpHstDemandItem a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND b.PickingSlipNo = @PickingSlipNo
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = Year(b.DocDate)
	AND a.Month = Month(b.DocDate)
	AND a.PartNo = b.PartNo
--
----=============================================================================================================================
---- INSERT TO ITEM MOVEMENT
----=============================================================================================================================
INSERT INTO SpTrnIMovement
SELECT
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, a.LmpNo DocNo
	, (SELECT LmPDate FROM SpTrnSLmpHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode 
		AND BranchCode = @BranchCode AND LmpNo = a.LmpNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),getdate()) CreatedDate 
	, '00' WarehouseCode
	, LocationCode 
	, a.PartNo
	, 'OUT' SignCode
	, 'LAMPIRAN' SubSignCode
	, a.QtyBill
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, @UserID CreatedBy
FROM SpTrnSLmpDtl a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND LmpNo IN (SELECT LmpNo FROM SpTrnSLmpHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode 
				AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)

FETCH NEXT FROM db_cursor INTO @PickingSlipNo
END
CLOSE db_cursor
DEALLOCATE db_cursor 


--===============================================================================================================================
-- UPDATE TRANSDATE
--===============================================================================================================================

update gnMstCoProfileSpare
set TransDate=getdate()
	, LastUpdateBy=@UserID
	, LastUpdateDate=getdate()
where CompanyCode = @CompanyCode AND BranchCode = @BranchCode

--===============================================================================================================================
-- DROP SECTION HEADER
--===============================================================================================================================
DROP TABLE #TempPickingSlipDtl
DROP TABLE #TempPickingSlipHdr
END
GO

if object_id('uspfn_GenerateLampiranNP') is not null
	drop procedure uspfn_GenerateLampiranNP
GO
-- uspfn_GenerateLampiranNP '6092401', '609240100', 'PLS/11/014757', '2011-04-28 08:25:59.060', '2W', 'ARYO', '1'
CREATE procedure [dbo].[uspfn_GenerateLampiranNP]
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@PickingSlipNo	VARCHAR(MAX),
	@LmpDate		DATETIME,
	@ProductType	VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@TypeOfGoods	VARCHAR(MAX)
AS
BEGIN
	BEGIN TRY
		DECLARE @MaxLmpNo INT
		SET @MaxLmpNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
			CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND DocumentType = 'LMP' 
				AND ProfitCenterCode = '300' 
				AND DocumentYear = YEAR(GetDate())),0)

		DECLARE @errmsg VARCHAR(MAX)
		DECLARE @TempLmpNo	VARCHAR(MAX)
		DECLARE @TempBPSFNo	VARCHAR(MAX)
		DECLARE @CustomerCode VARCHAR(MAX)

		SET @TempLmpNo  = ISNULL((SELECT 'LMP/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxLmpNo, 1), 6)),'LMP/YY/XXXXXX')
		SET @TempBPSFNo = ISNULL((SELECT BPSFNo FROM SpTrnSBPSFHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo),'')
		SET @CustomerCode = ISNULL((SELECT CustomerCode FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo),'')

		IF (ISNULL((SELECT Status FROM SpTrnSBPSFhdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND BPSFNo = @TempBPSFno), '0') = '2')
		BEGIN
			SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor picking ini sudah pernah di-proses, hubungi IT support untuk pemerikasaan data lebih lanjut !'
			RAISERROR (@errmsg,16,1);
			RETURN
		END

		UPDATE SpTrnSBPSFHdr
		SET Status = '2'
			, LastUpdateDate = GetDate()
			, LastUpdateBy = @UserID
		WHERE CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND BPSFNo = @TempBPSFNo

		--===============================================================================================================================
		IF (ISNULL((SELECT LmpNo FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo), '') <> '')
		BEGIN
			SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor lampiran sudah ada, periksa setting-an sequence dokumen (LMP) pada general module !'
			RAISERROR (@errmsg,16,1);
			RETURN
		END

		DECLARE @isLocked BIT
		SET @isLocked = (SELECT IsLocked FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)

		INSERT INTO SpTrnSLmpHdr
		SELECT
			CompanyCode
			, BranchCode
			, @TempLmpNo LmpNo	
			, GetDate() LmpDate
			, @TempBPSFNo BPSFNo
			, (SELECT BPSFDate FROM SpTrnSBPSFHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND BPSFNo = @TempBPSFNo)
				BPSFDate
			, PickingSlipNo
			, PickingSlipDate
			, TransType
			, CustomerCode
			, CustomerCodeBill
			, CustomerCodeShip
			, TotSalesQty
			, TotSalesAmt
			, TotDiscAmt
			, TotDPPAmt
			, TotPPNAmt
			, TotFinalSalesAmt
			, CONVERT(BIT, 0) isPKP
			, '0' Status
			, 0 PrintSeq
			, TypeOfGoods
			, @UserID CreatedBy
			, GetDate() CreatedDate
			, @UserID LastUpdateBy
			, GetDate() LastUpdateDate
			, @isLocked IsLocked	
			, NULL LockingBy
			, NULL LockingDate
		FROM SpTrnSPickingHdr 
		WHERE 
			1 = 1
			AND CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND PickingSlipNo = @PickingSlipNo

		UPDATE GnMstDocument
		SET DocumentSequence = DocumentSequence + 1
			, LastUpdateDate = GetDate()
			, LastUpdateBy = @UserID
		WHERE
			1 = 1
			AND CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND DocumentType = 'LMP'
			AND ProfitCenterCode = '300'
			AND DocumentYear = Year(GetDate())

		--===============================================================================================================================
		-- INSERT LAMPIRAN DETAIL
		--===============================================================================================================================
		INSERT INTO SpTrnSLmpDtl
		SELECT
			a.CompanyCode
			, a.BranchCode
			, @TempLmpNo LmpNo
			, a.WarehouseCode
			, a.PartNo
			, a.PartNoOriginal
			, a.DocNo
			, a.DocDate
			, a.ReferenceNo
			, a.ReferenceDate
			, a.LocationCode
			, a.QtyBill
			, a.RetailPriceInclTax
			, a.RetailPrice
			, ISNULL((SELECT CostPrice FROM SpMstItemPrice WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo),0) CostPrice
			, a.DiscPct
			, a.SalesAmt
			, a.DiscAmt
			, a.NetSalesAmt
			, 0 PPNAmt
			, a.TotSalesAmt
			, a.ProductType
			, a.PartCategory 
			, a.MovingCode
			, a.ABCClass
			, @UserID CreatedBy
			, GetDate() CreatedDate
			, @UserID LastUpdateBy
			, GetDate() LastUpdateDate
		FROM SpTrnSPickingDtl a
		WHERE 
			1 = 1
			AND a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND a.PickingSlipNo = @PickingSlipNo
			AND a.QtyPicked > 0


		--===============================================================================================================================
		-- UPDATE STOCK
		-- NOTES : Transtype = 11 --> + BorrowedQty
		--         Transtype = 12 --> - BorrowQty
		--===============================================================================================================================
		DECLARE @TempTransType VARCHAR (MAX) 
		SET @TempTransType = ISNULL((SELECT SUBSTRING(TransType,1,1) FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0)

		--===============================================================================================================================
		-- VALIDATION QTY
		--===============================================================================================================================
		DECLARE @Onhand_Valid NUMERIC(18,2)	
		DECLARE @Allocation_SRValid NUMERIC(18,2)
		DECLARE @Allocation_SLValid NUMERIC(18,2)
		DECLARE @Allocation_SPValid NUMERIC(18,2)
		
		SELECT * INTO #Valid_2 FROM(
			SELECT a.PartNo
				, b.AllocationSR - a.QtyBill QtyValidSR
				, b.AllocationSL - a.QtyBill QtyValidSL
				, b.AllocationSP - a.QtyBill QtyValidSP
				, b.Onhand - a.QtyBill QtyValidOnhand
			FROM SpTrnSPickingDtl a
			INNER JOIN SpMstItems b ON b.CompanyCode = a.CompanyCode 
				AND b.BranchCode = a.BranchCode 
				AND b.PartNo = a.PartNo		
			WHERE 1 = 1
				AND a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.PickingSlipNo = @PickingSlipNo 
		) #Valid_2
		
		SET @Allocation_SRValid = ISNULL((SELECT TOP 1 QtyValidSR FROM #Valid_2 WHERE QtyValidSR < 0),0)
		SET @Allocation_SPValid = ISNULL((SELECT TOP 1 QtyValidSP FROM #Valid_2 WHERE QtyValidSP < 0),0)
		SET @Allocation_SLValid = ISNULL((SELECT TOP 1 QtyValidSL FROM #Valid_2 WHERE QtyValidSL < 0),0)
		SET @Onhand_Valid = ISNULL((SELECT TOP 1 QtyValidOnhand FROM #Valid_2 WHERE QtyValidOnhand < 0),0)

		DROP TABLE #Valid_2
		IF (@TempTransType = '2')
		BEGIN
			IF (@Onhand_Valid < 0 OR @Allocation_SRValid < 0)
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END 
		END
		
		IF (@TempTransType = '1')
		BEGIN
			IF (@Onhand_Valid < 0 OR @Allocation_SPValid < 0)
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END 
		END	

		IF (@TempTransType = '3')
		BEGIN
			IF (@Onhand_Valid < 0 OR @Allocation_SLValid < 0)
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END 
		END	
		--===============================================================================================================================
		IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '11')
		BEGIN
		UPDATE SpMstItems
		SET
			BorrowedQty = BorrowedQty + b.QtyBill
		FROM SpMstItems a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND b.PickingSlipNo = @PickingSlipNo
			AND a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.PartNo = b.PartNo
		END

		IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '12')
		BEGIN
			UPDATE SpMstItems
			SET
				BorrowQty = BorrowQty - b.QtyBill
			FROM SpMstItems a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND b.PickingSlipNo = @PickingSlipNo
				AND a.CompanyCode = b.CompanyCode
				AND a.BranchCode = b.BranchCode
				AND a.PartNo = b.PartNo
		END

		IF (@TempTransType = '2')
		BEGIN
		UPDATE SpMstItems
		SET
			AllocationSR = AllocationSR - b.QtyBill
			, Onhand = Onhand - b.QtyBill
			, LastUpdateBy = @UserID
			, LastUpdateDate = GetDate()
			, LastSalesDate = GetDate()
		FROM SpMstItems a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND b.PickingSlipNo = @PickingSlipNo
			AND a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.PartNo = b.PartNo

		UPDATE SpMstItemLoc
		SET
			AllocationSR = AllocationSR - b.QtyBill
			, Onhand = Onhand - b.QtyBill
			, LastUpdateBy = @UserID
			, LastUpdateDate = GetDate()
		FROM SpMstItemLoc a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND a.WarehouseCode = '00'
			AND b.PickingSlipNo = @PickingSlipNo
			AND a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.PartNo = b.PartNo
		END

		IF (@TempTransType = '1')
		BEGIN
			UPDATE SpMstItems
			SET
				AllocationSP = AllocationSP - b.QtyBill
				, Onhand = Onhand - b.QtyBill
				, LastUpdateBy = @UserID
				, LastUpdateDate = GetDate()
				, LastSalesDate = GetDate()
			FROM SpMstItems a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND b.PickingSlipNo = @PickingSlipNo
				AND a.CompanyCode = b.CompanyCode
				AND a.BranchCode = b.BranchCode
				AND a.PartNo = b.PartNo

			UPDATE SpMstItemLoc
			SET
				AllocationSP = AllocationSP - b.QtyBill
				, Onhand = Onhand - b.QtyBill
				, LastUpdateBy = @UserID
				, LastUpdateDate = GetDate()
			FROM SpMstItemLoc a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.WarehouseCode = '00'
				AND b.PickingSlipNo = @PickingSlipNo
				AND a.CompanyCode = b.CompanyCode
				AND a.BranchCode = b.BranchCode
			AND a.PartNo = b.PartNo
		END

		IF (@TempTransType = '3')
		BEGIN
			UPDATE SpMstItems
			SET
				AllocationSL = AllocationSL - b.QtyBill
				, Onhand = Onhand - b.QtyBill
				, LastUpdateBy = @UserID
				, LastUpdateDate = GetDate()
				, LastSalesDate = GetDate()
			FROM SpMstItems a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND b.PickingSlipNo = @PickingSlipNo
				AND a.CompanyCode = b.CompanyCode
				AND a.BranchCode = b.BranchCode
				AND a.PartNo = b.PartNo

			UPDATE SpMstItemLoc
			SET
				AllocationSL = AllocationSL - b.QtyBill
				, Onhand = Onhand - b.QtyBill
				, LastUpdateBy = @UserID
				, LastUpdateDate = GetDate()
			FROM SpMstItemLoc a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.WarehouseCode = '00'
				AND b.PickingSlipNo = @PickingSlipNo
				AND a.CompanyCode = b.CompanyCode
				AND a.BranchCode = b.BranchCode
				AND a.PartNo = b.PartNo
		END
		--===============================================================================================================================
		-- UPDATE DEMAND CUST AND DEMAND ITEM
		--===============================================================================================================================

		UPDATE SpHstDemandCust
		SET SalesFreq = SalesFreq + 1
			, SalesQty = SalesQty + b.QtyBill
			, LastUpdateBy = @UserID 
			, LastUpdateDate = GetDate()
		FROM SpHstDemandCust a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND b.CompanyCode = @CompanyCode
			AND b.BranchCode = @BranchCode
			AND b.PickingSlipNo = @PickingSlipNo
			AND a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.Year = Year(b.DocDate)
			AND a.Month = Month(b.DocDate)
			AND a.CustomerCode = @CustomerCode
			AND a.PartNo = b.PartNo
			
		UPDATE SpHstDemandItem
		SET SalesFreq = SalesFreq + 1
			, SalesQty = SalesQty + b.QtyBill
			, LastUpdateBy = @UserID 
			, LastUpdateDate = GetDate()
		FROM SpHstDemandItem a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND b.CompanyCode = @CompanyCode
			AND b.BranchCode = @BranchCode
			AND b.PickingSlipNo = @PickingSlipNo
			AND a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.Year = Year(b.DocDate)
			AND a.Month = Month(b.DocDate)
			AND a.PartNo = b.PartNo

		----=============================================================================================================================
		---- INSERT TO ITEM MOVEMENT
		----=============================================================================================================================
		INSERT INTO SpTrnIMovement
		SELECT
			@CompanyCode CompanyCode
			, @BranchCode BranchCode
			, a.LmpNo DocNo
			, (SELECT LmPDate FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode 
				AND BranchCode = @BranchCode AND LmpNo = a.LmpNo) 
			  DocDate
			, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),getdate()) CreatedDate 
			, '00' WarehouseCode
			, LocationCode 
			, a.PartNo
			, 'OUT' SignCode
			, 'LAMPIRAN' SubSignCode
			, a.QtyBill
			, a.RetailPrice
			, a.CostPrice
			, a.ABCClass
			, a.MovingCode
			, a.ProductType
			, a.PartCategory
			, @UserID CreatedBy
		FROM SpTrnSLmpDtl a
		WHERE
			1 = 1
			AND CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND LmpNo = @TempLmpNo


		--===============================================================================================================================
		-- UPDATE AVERAGE COST
		-- NOTES : Transtype = 2% (SERVICE) CHECK ISLINKTOSERVICE
		--===============================================================================================================================
		IF (ISNULL((SELECT SUBSTRING(TransType,1,1) FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '2')
		BEGIN
			IF (CONVERT(VARCHAR,ISNULL((SELECT IsLinkToService FROM gnMstCoProfile WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0),0) = '1')
			BEGIN
				SELECT
					a.CompanyCode
					, a.BranchCode
					, d.ProductType
					, ISNULL(d.ServiceNo, 0) ServiceNo
					, a.PartNo
					, a.DocNo SupplySlipNo
					, ISNULL(a.CostPrice, 0) CostPrice
  					, ISNULL(a.RetailPrice, 0) RetailPrice
				INTO #1
				FROM spTrnSLmpDtl a 
					INNER JOIN spTrnSORDHdr c ON a.CompanyCode = c.CompanyCode
						AND a.BranchCode = c.BranchCode
						AND a.DocNo = c.DocNo
					INNER JOIN svTrnService d ON a.CompanyCode = d.CompanyCode
						AND a.BranchCode = d.BranchCode
				WHERE a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND d.ProductType = @ProductType
					AND c.UsageDocNo = d.JobOrderNo
					AND a.LmpNo = @TempLmpNo

				UPDATE svTrnSrvItem 
				SET	CostPrice = b.CostPrice
					, LastUpdateBy = @UserID
					, LastUpdateDate = GETDATE()
				FROM svTrnSrvItem a, #1 b
				WHERE a.CompanyCode = b.CompanyCode
					AND a.BranchCode = b.BranchCode
					AND a.ProductType = b.ProductType
					AND a.ServiceNo = b.ServiceNo  
					AND a.PartNo = b.PartNo
					AND a.SupplySlipNo = b.SupplySlipNo	

				--===============================================================================================================================
				-- SERVICE PART
				--===============================================================================================================================
				SELECT * INTO #TempServiceItem FROM (
				SELECT 
					a.CompanyCode
					, a.BranchCode
					, a.ProductType
					, a.ServiceNo
					, a.PartNo
					, a.PartSeq
					, a.DemandQty
					, a.SupplyQty
					, b.QtyBill
					, b.DocNo
					, a.CostPrice
					, a.RetailPrice
					, a.TypeOfGoods
					, a.BillType
					, a.DiscPct
				FROM SvTrnSrvItem a 
				INNER JOIN SpTrnSPickingDtl b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo AND a.SupplySlipNo = b.DocNo
				WHERE
					1 = 1
					AND a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND a.ProductType = @ProductType
					AND a.ServiceNo IN (SELECT ServiceNo 
										FROM SvTrnService 
										WHERE 1 = 1 AND CompanyCode = @CompanyCode 
											AND BranchCode = @BranchCode 
											AND JobOrderNo IN (SELECT ReferenceNo 
																FROM SpTrnSPickingDtl 
																WHERE 1= 1 AND CompanyCode = @CompanyCode 
																	AND  BranchCode = @BranchCode 
																	AND PickingSlipNo = @PickingSlipNo))
					AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
						AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
					AND a.SupplySlipNo = b .DocNo
				) #TempServiceItem 

				UPDATE svTrnSrvItem
				SET SupplyQty = (CASE WHEN b.QtyBill > b.DemandQty 
								THEN 
									CASE WHEN b.DemandQty = 0 THEN b.QtyBill ELSE b.DemandQty END
								ELSE b.QtyBill END)
					, LastUpdateBy = @UserID
					, LastUpdateDate = Getdate()
				FROM svTrnSrvItem a, #TempServiceItem b
				WHERE
					1 = 1
					AND a.CompanyCode = b.CompanyCode
					AND a.BranchCode = b.BranchCode
					AND a.ProductType = b.ProductType
					AND a.ServiceNo = b.ServiceNo
					AND a.PartNo = b.PartNo
					AND a.PartSeq = b.PartSeq
					AND a.SupplySlipNo = b.DocNo

				UPDATE svTrnSrvItem
				SET CostPrice = b.CostPrice
					, LastUpdateBy = @UserID
					, LastUpdateDate = Getdate()
				FROM svTrnSrvItem a, #TempServiceItem b
				WHERE
					1 = 1
					AND a.CompanyCode = b.CompanyCode
					AND a.BranchCode = b.BranchCode
					AND a.ProductType = b.ProductType
					AND a.ServiceNo = b.ServiceNo
					AND a.PartNo = b.PartNo
					AND a.SupplySlipNo = b.DocNo

				--===============================================================================================================================
				-- INSERT NEW SRV ITEM BASED PICKING LIST
				--===============================================================================================================================
				INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
				SELECT 
					a.CompanyCode
					, a.BranchCode
					, a.ProductType
					, a.ServiceNo
					, a.PartNo
					, (select max(PartSeq)+1 from svTrnSrvItem b where b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and 
						b.ProductType = a.ProductType and b.ServiceNo = a.ServiceNo) PartSeq
					, 0 DemandQty
					, a.QtyBill - a.DemandQty SupplyQty
					, 0 ReturnQty
					, a.CostPrice
					, a.RetailPrice
					, a.TypeOfGoods
					, a.BillType
					, a.DocNo SupplySlipNo
					, (SELECT TOP 1 DocDate FROM SpTrnSORDHdr WHERE 1= 1 AND CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
						AND DocNo = a.DocNo) SupplySlipDate
					, NULL SSReturnNo
					, NULL SSReturnDate
					, @UserID CreatedBy
					, GetDate() CreatedDate
					, @UserID LastUpdateBy
					, GetDate() LastUpdateDate
					, a.DiscPct
				FROM #TempServiceItem a 
				WHERE
					1 = 1
					AND a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND a.ProductType = @ProductType
					AND a.DemandQty < a.QtyBill
					AND a.QtyBill > 0
					AND a.DemandQty > 0

				DROP TABLE #TempServiceItem 	
				DROP TABLE #1
			END
		END

		--===============================================================================================================================
		-- GENERATE JOURNAL AND AUTOMATE TRANSFER STOCK
		-- NOTES : Transtype = 10 (TRANSFER STOCK)
		--===============================================================================================================================
		DECLARE @TempJournalPrefix	VARCHAR(MAX)
		DECLARE @MaxTempJournal		INT

		DECLARE @TempJournal		VARCHAR(MAX)
		DECLARE @Amount				NUMERIC(18,2)
		DECLARE @TempFiscalMonth	INT
		DECLARE @TempFiscalYear		INT

		DECLARE @PeriodeNum			NUMERIC(18,0)
		DECLARE @Periode			VARCHAR(MAX)
		DECLARE @PeriodeName		VARCHAR(MAX)
		DECLARE @GLDate				DATETIME

		SET @TempFiscalYear = ISNULL((SELECT FiscalYear FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0) 
		SET @TempFiscalMonth  = ISNULL((SELECT FiscalMonth FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0) 

		SET @PeriodeNum = ISNULL((SELECT  TOP 1 PeriodeNum
				FROM gnMstPeriode 
				WHERE CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode AND FiscalYear = @TempFiscalYear
					AND FiscalMonth = @TempFiscalMonth AND StatusSparepart = 1
					AND (MONTH(FromDate) = MONTH(@LmpDate) AND YEAR(FromDate) = YEAR(@LmpDate))
					AND FiscalStatus = 1 ), NULL) 

		SET @Periode = ISNULL((SELECT  TOP 1 CONVERT(varchar, FiscalYear) + RIGHT('00' + CONVERT(varchar, PeriodeNum), 2) AS Periode
				FROM gnMstPeriode 
				WHERE CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode AND FiscalYear = @TempFiscalYear
					AND FiscalMonth = @TempFiscalMonth AND StatusSparepart = 1
					AND (MONTH(FromDate) = MONTH(@LmpDate) AND YEAR(FromDate) = YEAR(@LmpDate))
					AND FiscalStatus = 1 ), NULL) 

		SET @PeriodeName =  ISNULL((SELECT  TOP 1 PeriodeName
				FROM gnMstPeriode 
				WHERE CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode AND FiscalYear = @TempFiscalYear
					AND FiscalMonth = @TempFiscalMonth AND StatusSparepart = 1
					AND (MONTH(FromDate) = MONTH(@LmpDate) AND YEAR(FromDate) = YEAR(@LmpDate))
					AND FiscalStatus = 1 ), NULL)

		DECLARE @AccountTypeInTran	VARCHAR(MAX)
		DECLARE @AccountTypeInvent	VARCHAR(MAX)

		SET @AccountTypeInTran = ISNULL((SELECT b.AccountType FROM GnMstAccount b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode AND b.AccountNo = 
				ISNULL((SELECT InTransitAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')
				),'')
		SET @AccountTypeInvent = ISNULL((SELECT b.AccountType FROM GnMstAccount b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode AND b.AccountNo = 
				ISNULL((SELECT InventoryAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')
				),'')

		--===============================================================================================================================
		-- SET ACCOUNT FOR GENERATE JOURNAL
		--===============================================================================================================================
		DECLARE @CustCode VARCHAR(MAX)
		SET @CustCode = (SELECT CustomerCode FROM spTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)
		
		Declare @TPGO VARCHAR(MAX)
		SET @TPGO = (SELECT TypeOfGoods FROM spTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)

		DECLARE @InventoryAccNo VARCHAR(MAX)
		DECLARE @InTransitAccNo VARCHAR(MAX)

		IF (@isLocked = '1')
		BEGIN
			DECLARE @CompTo VARCHAR(MAX)
			SET @CompTo = (SELECT ISNULL(CompanyCodeTo,'') FROM spMstCompanyAccount WHERE CompanyCode = @CompanyCode AND BranchCodeTo = @CustCode)
			
			SET @InTransitAccNo =  (SELECT ISNULL(IntercompanyAccNoTo,'') FROM spMstCompanyAccountDtl WHERE CompanyCode = @CompanyCode AND CompanyCodeTo = @CompTo AND TPGO = @TPGO)
		END
		ELSE
			SET @InTransitAccNo =  ISNULL((SELECT InTransitAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')

		SET @InventoryAccNo = ISNULL((SELECT InventoryAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')
		SET @Amount = ISNULL((SELECT SUM(QtyBill * CostPrice) FROM SpTrnSLmpDtl WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0)

		--===============================================================================================================================
		-- GENERATE JOURNAL
		-- NOTES : Transtype = 1O (TRANSFER STOCK)
		--===============================================================================================================================
		IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '10')
		BEGIN
			IF (@isLocked != '1')
			BEGIN
				--===============================================================================================================================
				-- AUTOMATE TRANSFER STOCK
				--===============================================================================================================================
				INSERT INTO SpUtlStockTrfHdr
				SELECT
					a.CompanyCode
					, a.CustomerCode BranchCode
					, a.BranchCode DealerCode
					, a.LmpNo LampiranNo
					, a.CustomerCode RcvDealerCode
					, '' InvoiceNo 
					, '' BinningNo 
					, '1900-01-01 00:00:00.000' BinningDate
					, 0 Status
					, @UserID CreatedBy
					, GetDate() CreatedDate
					, @UserID LastUpdateBy
					, GetDate() LastUpdateDate
					, null TypeOfGoods
				FROM SpTrnSLmpHdr a
				WHERE a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND a.LmpNo = @TempLmpNo

				INSERT INTO SpUtlStockTrfDtl
				SELECT
					a.CompanyCode CompanyCode
					, b.CustomerCode BranchCode
					, a.BranchCode DealerCode
					, a.LmpNo LampiranNo
					, a.DocNo OrderNo
					, a.PartNo PartNo
					, b.BPSFno SalesNo
					, a.PartNo PartNoShip
					, a.QtyBill QtyShipped 
					, 1.00 SalesUnit
					, a.CostPrice PurchasePrice
					, a.CostPrice costPrice
					, '1900-01-01 00:00:00.000' ProcessDate
					, @ProductType Producttype
					, '' partCategory
					, @UserID CreatedBy
					, GetDate() CreatedDate
					, @UserID LastUpdateBy
					, GetDate() LastUpdateDate 
				FROM SpTrnSLmpDtl a
				INNER JOIN SpTrnSLmpHdr b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.LmpNo = b.LmpNo
				WHERE a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND a.LmpNo = @TempLmpNo
			END

			--===============================================================================================================================
			-- GENERATE GLINTERFACE
			--===============================================================================================================================
			IF (ISNULL((SELECT TOP 1 DocNo FROM glInterface WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND DocNo = @TempLmpNo), '') <> '')
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor lampiran sudah ada dalam glInterface, periksa setting-an sequence dokumen (LMP) !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END
			 
			INSERT INTO GLInterface
			SELECT
				a.CompanyCode
				, a.BranchCode
				, a.LmpNo DocNo
				, 1 SeqNo
				, a.LmpDate DocDate
				, '300' ProfitCenterCode
				, GetDate() AccDate
				, @InTransitAccNo AccountNo
				, 'SPAREPART' JournalCode
				, 'BPS' TypeJournal
				, a.LmpNo ApplyTo
				, @Amount AmountDB
				, 0 AmountCR
				, 'INTRANSIT' TypeTrans
				, '' BatchNo
				, '1900-01-01 00:00:00.000' BatchDate
				, 3 StatusFlag
				, @UserID CreateBy 
				, GetDate() CreateDate
				, @UserID UpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			INSERT INTO GLInterface
			SELECT
				a.CompanyCode
				, a.BranchCode
				, a.LmpNo DocNo
				, 2 SeqNo
				, a.LmpDate DocDate
				, '300' ProfitCenterCode
				, GetDate() AccDate
				, @InventoryAccNo AccountNo
				, 'SPAREPART' JournalCode
				, 'BPS' TypeJournal
				, a.LmpNo ApplyTo
				, 0 AmountDB	
				, @Amount AmountCR
				, 'INVENTORY' TypeTrans
				, '' BatchNo
				, '1900-01-01 00:00:00.000' BatchDate
				, 3 StatusFlag
				, @UserID CreateBy 
				, GetDate() CreateDate
				, @UserID UpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			SET @GLDate = ISNULL((SELECT TOP 1 DocDate FROM GlInterface WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND DocNo = @TempLmpNo), NULL)

			--===============================================================================================================================
			-- GENERATE GLJOURNAL
			--===============================================================================================================================
			SET @MaxTempJournal = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
				CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND DocumentType = 'JTS' 
					AND ProfitCenterCode = '300' 
					AND DocumentYear = YEAR(GetDate())),0)

			SET @TempJournalPrefix = ISNULL((SELECT DocumentPrefix FROM GnMstDocument WHERE 
				CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND DocumentType = 'JTS' 
					AND ProfitCenterCode = '300' 
					AND DocumentYear = YEAR(GetDate())),'XXX')

			SET @TempJournal = ISNULL((SELECT @TempJournalPrefix + '/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxTempJournal, 1), 6)),'XXX/YY/XXXXXX')

			IF (ISNULL((SELECT TOP 1 JournalNo FROM GlJournal WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND JournalNo = @TempJournal), '') <> '')
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor journal sudah terpakai, periksa setting-an sequence dokumen (JTS) !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END

			INSERT INTO GLJournal
			SELECT
				CompanyCode
				, BranchCode
				, @TempFiscalYear FiscalYear
				, '300' ProfitCenterCode
				, @TempJournal JournalNo
				, 'Harian' JournalType
				, GetDate() JournalDate
				, 'SP' DocSource
				, @TempLmpNo + ',' + @TempBPSFNo ReffNo
				, GetDate() ReffDate
				, @TempFiscalMonth FiscalMonth
				, @PeriodeNum
				, @Periode
				, @PeriodeName
				, @GLDate
				, 1 BalanceType
				, @Amount AmountDB 
				, @Amount AmountCR
				, 1 Status
				, '' StatusRecon
				, NULL BatchNo
				, '1900-01-01 00:00:00.000' PostingDate 
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 1 Printseq
				, 0 FSend
				, '' SendBy
				, '1900-01-01 00:00:00.000' SendDate
				, @UserID CreatedBy
				, GetDate() CreatedDate
				, @UserID LastUpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			--===============================================================================================================================
			-- GENERATE GLJOURNALDTL
			--===============================================================================================================================
			INSERT INTO GLJournalDtl
			SELECT
				a.CompanyCode CompanyCode
				, a.BranchCode BranchCode
				, @TempFiscalYear FiscalYear
				, @TempJournal JournalNo
				, 1 SeqNo
				, @InTransitAccNo AccountNo
				, @TempLmpNo + ',' + @TempBPSFNo Description
				, 'Harian' JournalType
				, @Amount AmountDB 
				, 0 AmountCR
				, 'INTRANSIT' TypeTrans
				, @AccountTypeIntran
				, @TempLmpNo  DocNo
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 0 FSend
				, @UserID CreatedBy 
				, GetDate() CreatedDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			INSERT INTO GLJournalDtl
			SELECT
				a.CompanyCode CompanyCode
				, a.BranchCode BranchCode
				, @TempFiscalYear FiscalYear
				, @TempJournal JournalNo
				, 2 SeqNo
				, @InventoryAccNo AccountNo
				, @TempLmpNo + ',' + @TempBPSFNo Description
				, 'Harian' JournalType
				, 0 AmountDB
				, @Amount AmountCR
				, 'INVENTORY' TypeTrans
				, @AccountTypeInvent
				, @TempLmpNo  DocNo
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 0 FSend
				, @UserID CreatedBy 
				, GetDate() CreatedDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			UPDATE GnMstDocument
			SET DocumentSequence = DocumentSequence + 1
				, LastUpdateDate = GetDate()
				, LastUpdateBy = @UserID
			WHERE
				1 = 1
				AND CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND DocumentType = 'JTS'
				AND ProfitCenterCode = '300'
				AND DocumentYear = Year(GetDate())
		END
		--===============================================================================================================================
		-- END GENERATE JOURNAL Transtype = 1O (TRANSFER STOCK) --
		--===============================================================================================================================
		
		--===============================================================================================================================
		-- GENERATE JOURNAL
		-- NOTES : Transtype = 14 (OTHERS)
		--===============================================================================================================================
		IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '14')
		BEGIN
			--===============================================================================================================================
			-- GENERATE GLINTERFACE
			--===============================================================================================================================
			IF (ISNULL((SELECT TOP 1 DocNo FROM GlInterface WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND DocNo = @TempLmpNo), '') <> '')
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor lampiran sudah ada dalam glInterface, periksa setting-an sequence dokumen (LMP) !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END

			INSERT INTO GLInterface
			SELECT
				a.CompanyCode
				, a.BranchCode
				, a.LmpNo DocNo
				, 1 SeqNo
				, a.LmpDate DocDate
				, '300' ProfitCenterCode
				, GetDate() AccDate
				, @InTransitAccNo AccountNo
				, 'SPAREPART' JournalCode
				, 'BPS' TypeJournal
				, a.LmpNo ApplyTo
				, @Amount AmountDB
				, 0 AmountCR
				, 'INTRANSIT' TypeTrans
				, '' BatchNo
				, '1900-01-01 00:00:00.000' BatchDate
				, 3 StatusFlag
				, @UserID CreateBy 
				, GetDate() CreateDate
				, @UserID UpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			INSERT INTO GLInterface
			SELECT
				a.CompanyCode
				, a.BranchCode
				, a.LmpNo DocNo
				, 2 SeqNo
				, a.LmpDate DocDate
				, '300' ProfitCenterCode
				, GetDate() AccDate
				, @InventoryAccNo AccountNo
				, 'SPAREPART' JournalCode
				, 'BPS' TypeJournal
				, a.LmpNo ApplyTo
				, 0 AmountDB	
				, @Amount AmountCR
				, 'INVENTORY' TypeTrans
				, '' BatchNo
				, '1900-01-01 00:00:00.000' BatchDate
				, 3 StatusFlag
				, @UserID CreateBy 
				, GetDate() CreateDate
				, @UserID UpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			--===============================================================================================================================
			-- GENERATE GLJOURNAL
			--===============================================================================================================================
			SET @MaxTempJournal = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
				CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND DocumentType = 'JTI' 
					AND ProfitCenterCode = '300' 
					AND DocumentYear = YEAR(GetDate())),0)

			SET @TempJournalPrefix = ISNULL((SELECT DocumentPrefix FROM GnMstDocument WHERE 
				CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND DocumentType = 'JTI' 
					AND ProfitCenterCode = '300' 
					AND DocumentYear = YEAR(GetDate())),'XXX')

			SET @TempJournal = ISNULL((SELECT @TempJournalPrefix + '/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxTempJournal, 1), 6)),'XXX/YY/XXXXXX')
			SET @TempFiscalYear = ISNULL((SELECT FiscalYear FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0) 
			SET @TempFiscalMonth  = ISNULL((SELECT FiscalMonth FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0) 
			IF (ISNULL((SELECT TOP 1 JournalNo FROM GlJournal WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND JournalNo = @TempJournal), '') <> '')
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor journal sudah terpakai, periksa setting-an sequence dokumen (JTS) !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END

			INSERT INTO GLJournal
			SELECT
				CompanyCode
				, BranchCode
				, @TempFiscalYear FiscalYear
				, '300' ProfitCenterCode
				, @TempJournal JournalNo
				, 'Harian' JournalType
				, GetDate() JournalDate
				, 'SP' DocSource
				, @TempLmpNo ReffNo
				, GetDate() ReffDate
				, @TempFiscalMonth FiscalMonth
				, @PeriodeNum
				, @Periode
				, @PeriodeName
				, @GLDate
				, 1 BalanceType
				, @Amount AmountDB 
				, @Amount AmountCR
				, 1 Status
				, '' StatusRecon
				, NULL BatchNo
				, '1900-01-01 00:00:00.000' PostingDate 
				, '' StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 1 Printseq
				, 0 FSend
				, '' SendBy
				, '1900-01-01 00:00:00.000' SendDate
				, @UserID CreatedBy
				, GetDate() CreatedDate
				, @UserID LastUpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			--===============================================================================================================================
			-- GENERATE GLJOURNALDTL
			--===============================================================================================================================
			INSERT INTO GLJournalDtl
			SELECT
				a.CompanyCode CompanyCode
				, a.BranchCode BranchCode
				, @TempFiscalYear FiscalYear
				, @TempJournal JournalNo
				, 1 SeqNo
				, @InTransitAccNo AccountNo
				, @TempLmpNo Description
				, 'Harian' JournalType
				, @Amount AmountDB 
				, 0 AmountCR
				, 'INTRANSIT' TypeTrans
				, @AccountTypeIntran
				, @TempLmpNo  DocNo
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 0 FSend
				, @UserID CreatedBy 
				, GetDate() CreatedDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			INSERT INTO GLJournalDtl
			SELECT
				a.CompanyCode CompanyCode
				, a.BranchCode BranchCode
				, @TempFiscalYear FiscalYear
				, @TempJournal JournalNo
				, 2 SeqNo
				, @InventoryAccNo AccountNo
				, @TempLmpNo  Description
				, 'Harian' JournalType
				, 0 AmountDB
				, @Amount AmountCR
				, 'INVENTORY' TypeTrans
				, @AccountTypeInvent
				, @TempLmpNo  DocNo
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 0 FSend
				, @UserID CreatedBy 
				, GetDate() CreatedDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			UPDATE GnMstDocument
			SET DocumentSequence = DocumentSequence + 1
				, LastUpdateDate = GetDate()
				, LastUpdateBy = @UserID
			WHERE
				1 = 1
				AND CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND DocumentType = 'JTI'
				AND ProfitCenterCode = '300'
				AND DocumentYear = Year(GetDate())
		END
		--===============================================================================================================================
		-- END GENERATE JOURNAL Transtype = 14 (OTHERS) --
		--===============================================================================================================================

		--===============================================================================================================================
		-- UPDATE TRANSDATE
		--===============================================================================================================================
		update gnMstCoProfileSpare
		set TransDate=getdate()
			, LastUpdateBy=@UserID
			, LastUpdateDate=getdate()
		where CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		
	END TRY
	BEGIN CATCH
		SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat proses yang mem-block proses, harap tunggu beberapa saat kemudian coba lagi !'
		RAISERROR (@errmsg,16,1);
		RETURN
	END CATCH
END
GO
if object_id('uspfn_GenerateSSPickingslip') is not null
	drop procedure uspfn_GenerateSSPickingslip
GO
CREATE procedure [dbo].[uspfn_GenerateSSPickingslip]
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@TransType		VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@DocDate		DATETIME
AS
BEGIN

--declare	@CompanyCode	VARCHAR(MAX)
--declare	@BranchCode		VARCHAR(MAX)
--declare	@JobOrderNo		VARCHAR(MAX)
--declare	@ProductType	VARCHAR(MAX)
--declare	@CustomerCode	VARCHAR(MAX)
--declare	@TransType		VARCHAR(MAX)
--declare	@UserID			VARCHAR(MAX)
--declare	@DocDate		DATETIME

--set	@CompanyCode	= '6006406'
--set	@BranchCode		= '6006402'
--set	@JobOrderNo		= 'SPK/12/003020'
--set	@ProductType	= '4W'
--set	@CustomerCode	= '1443'
--set	@TransType		= '20'
--set	@UserID			= 'sa'
--set	@DocDate		= getdate()


--================================================================================================================================
-- TABLE MASTER
--================================================================================================================================
-- Temporary for Item --
------------------------
SELECT * INTO #Item FROM (
SELECT a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.RetailPrice
	, a.PartNo
	, a.Billtype
	, SUM(ISNULL(a.DemandQty, 0) - (ISNULL(a.SupplyQty, 0))) QtyOrder
FROM svTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN svTrnService b ON b.CompanyCode = a.CompanyCode
	AND b.BranchCode = a.BranchCode
	AND b.ProductType = a.ProductType
	AND b.ServiceNo = a.ServiceNo
	AND b.JobOrderNo = @JobOrderNo
WHERE a.CompanyCode = @CompanyCode 
	AND a.BranchCode = @BranchCode 
	AND a.ProductType = @ProductType 
GROUP BY a.CompanyCode, a.BranchCode, a.ProductType
	, a.ServiceNo, a.PartNo, a.RetailPrice, a.BillType ) #Item 

SELECT * INTO #SrvOrder FROM (
SELECT DISTINCT(a.CompanyCode) 
    , a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
    , (SELECT Paravalue FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'GTGO' AND LookUpValue = a.TypeOfGoods) TipePart
    , (SELECT PartName FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) PartName
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, SUM(a.QtyOrder) QtyOrder
    , 0 QtySupply
    , 0 QtyBO
    , (SUM(a.QtyOrder) * a.RetailPrice) * ((100 - a.PartDiscPct)/100) NetSalesAmt
    , a.PartDiscPct DiscPct
FROM
(
	SELECT
		DISTINCT(a.CompanyCode) 
		, a.BranchCode
		, a.ProductType
		, a.ServiceNo
		, a.PartNo
		, a.RetailPrice
		, c.CostPrice
		, a.TypeOfGoods
		, a.BillType
		, ISNULL(Item.QtyOrder,0) AS QtyOrder
		, a.DiscPct PartDiscPct 
	FROM
		svTrnSrvItem a WITH (NOLOCK, NOWAIT)
		LEFT JOIN svTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode	
			AND a.ProductType = b.ProductType
			AND a.ServiceNo = b.ServiceNo
		LEFT JOIN #Item Item ON Item.CompanyCode = a.CompanyCode 
			AND Item.BranchCode = a.BranchCode 
			AND Item.ProductType = a.ProductType 
			AND Item.ServiceNo = a.ServiceNo 
			AND Item.PartNo = a.PartNo 
			AND Item.RetailPrice = a.RetailPrice 
			AND Item.BillType = a.Billtype
		LEFT JOIN SpMstItemPrice c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode 
			AND a.BranchCode = c.BranchCode 
			AND a.PartNo = c.PartNo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND a.ProductType = @ProductType
		AND Item.QtyOrder > 0
		AND JobOrderNo = @JobOrderNo
) a
GROUP BY
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
    , a.PartDiscPct 
) #SrvOrder


--================================================================================================================================
-- INSERT TABLE SpTrnSORDHdr AND SpTrnSORDDtl
--================================================================================================================================
DECLARE @MaxDocNo			INT
DECLARE	@MaxPickingList		INT
DECLARE @TempDocNo			VARCHAR(MAX)
DECLARE @TempPickingList	VARCHAR(MAX)
DECLARE @TypeOfGoods		VARCHAR(MAX)
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

--===============================================================================================================================
-- LOOPING BASED ON THE TYPE OF GOODS
-- ==============================================================================================================================
DECLARE db_cursor CURSOR FOR
SELECT DISTINCT TypeOfGoods FROM #SrvOrder
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND ProductType = @ProductType 

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @TypeOfGoods

WHILE @@FETCH_STATUS = 0
BEGIN

--===============================================================================================================================
-- INSERT HEADER
-- ==============================================================================================================================
SET @MaxDocNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'SSS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempDocNo = ISNULL((SELECT 'SSS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxDocNo, 1), 6)),'SSS/YY/XXXXXX')

INSERT INTO SpTrnSORDHdr
([CompanyCode]
           ,[BranchCode]
           ,[DocNo]
           ,[DocDate]
           ,[UsageDocNo]
           ,[UsageDocDate]
           ,[CustomerCode]
           ,[CustomerCodeBill]
           ,[CustomerCodeShip]
           ,[isBO]
           ,[isSubstitution]
           ,[isIncludePPN]
           ,[TransType]
           ,[SalesType]
           ,[IsPORDD]
           ,[OrderNo]
           ,[OrderDate]
           ,[TOPCode]
           ,[TOPDays]
           ,[PaymentCode]
           ,[PaymentRefNo]
           ,[TotSalesQty]
           ,[TotSalesAmt]
           ,[TotDiscAmt]
           ,[TotDPPAmt]
           ,[TotPPNAmt]
           ,[TotFinalSalesAmt]
           ,[isPKP]
           ,[ExPickingSlipNo]
           ,[ExPickingSlipDate]
           ,[Status]
           ,[PrintSeq]
           ,[TypeOfGoods]
           ,[isDropsign]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastUpdateBy]
           ,[LastUpdateDate]
           ,[isLocked]
           ,[LockingBy]
           ,[LockingDate])

SELECT 
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, @TempDocNo DocNo 
	, @DocDate DocDate
	, @JobOrderNo UsageDocNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) UsageDocDate
	, (SELECT CustomerCode FROM SvTrnService WHERE 1 = 1AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCode
	, (SELECT CustomerCodeBill FROM SvTrnService WHERE 1 = 1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeBill
	, (SELECT CustomerCode FROM SvTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeShip
	, CONVERT(BIT, 0) isBO
	, CONVERT(BIT, 0) isSubstitution
	, CONVERT(BIT, 1) isIncludePPN
	, @TransType TransType
	, '2' SalesType
	, CONVERT(BIT, 0) isPORDD
	, @JobOrderNo OrderNo
	, @DocDate OrderNo
	, ISNULL((SELECT TOPCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') TOPCode
	, ISNULL((SELECT ParaValue FROM GnMstLookUpDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND CodeID = 'TOPC' AND 
		LookupValue IN 
		(SELECT TOPCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)
	  ),0) TOPDays
	, ISNULL((SELECT PaymentCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') PaymentCode
	, '' PaymentReffNo
	, 0 TotSalesQty
	, 0 TotSalesAmt
	, 0 TotDiscAmt
	, 0 TotDPPAmt
	, 0 TotPPNAmt
	, 0 TotFinalSalesAmt
	, CONVERT(BIT, 0) isPKP
	, NULL ExPickingSlipNo
	, NULL ExPickingSlipDate
	, '4' Status
	, 0 PrintSeq
	, @TypeOfGoods TypeOfGoods
	, NULL IsDropSign
	, @UserID CreatedBY
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate


UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'SSS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT DETAIL
-- ==============================================================================================================================

INSERT INTO SpTrnSORDDtl 
(
	[CompanyCode] ,
	[BranchCode] ,
	[DocNo] ,
	[PartNo] ,
	[WarehouseCode] ,
	[PartNoOriginal] ,
	[ReferenceNo] ,
	[ReferenceDate] ,
	[LocationCode] ,
	[QtyOrder] ,
	[QtySupply] ,
	[QtyBO] ,
	[QtyBOSupply] ,
	[QtyBOCancel] ,
	[QtyBill] ,
	[RetailPriceInclTax] ,
	[RetailPrice] ,
	[CostPrice] ,
	[DiscPct] ,
	[SalesAmt] ,
	[DiscAmt] ,
	[NetSalesAmt] ,
	[PPNAmt] ,
	[TotSalesAmt] ,
	[MovingCode] ,
	[ABCClass] ,
	[ProductType] ,
	[PartCategory] ,
	[Status] ,
	[CreatedBy] ,
	[CreatedDate] ,
	[LastUpdateBy] ,
	[LastUpdateDate] ,
	[StockAllocatedBy] ,
	[StockAllocatedDate] ,
	[FirstDemandQty] )
SELECT
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, @TempDocNo DocNo 
	, a.PartNo
	, '00' WarehouseCode
	, a.PartNo PartNoOriginal
	, @JobOrderNo ReferenceNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) ReferenceDate
	, (SELECT LocationCode FROM SpMstItemLoc WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND WarehouseCode = '00'
		AND PartNo = a.PartNo) LocationCode
	, a.QtyOrder
	, CASE WHEN 
		ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
			FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
			AND PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
			FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
			AND PartNo = a.PartNo),0)
		END AS QtySupply
	, CASE WHEN 
		ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
			FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
			AND PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
			FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
			AND PartNo = a.PartNo),0)
		END AS QtyBO
	, 0 QtyBOSupply
	, CASE WHEN 
		ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
			FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
			AND PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
				FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
				AND PartNo = a.PartNo),0)
		END AS QtyBOCancel
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice * 10 /100) RetailPriceIncltax
	, a.RetailPrice
	, b.CostPrice
	, a.DiscPct
	, CASE WHEN 
		ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
			FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
			AND PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder * a.RetailPrice
		ELSE ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
				FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
				AND PartNo = a.PartNo),0) * a.RetailPrice
		END AS SalesAmt
	, CASE WHEN 
		ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
			FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
			AND PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice) * a.DiscPct/100)
		ELSE floor((ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
				FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
				AND PartNo = a.PartNo),0) * a.RetailPrice) * a.DiscPct/100)
		END AS DiscAmt
	, CASE WHEN 
		ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
			FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
			AND PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
				FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
				AND PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
					FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
					AND PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS NetSalesAmt
	, 0 PPNAmt
	,  CASE WHEN 
		ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
			FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
			AND PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
				FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
				AND PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
					FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
					AND PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS TotSalesAmt
	, (SELECT MovingCode FROM SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT ABCClass FROM SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo) 
		ABCClass
	, @ProductType ProductType
	, (SELECT PartCategory FROM SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo) 
		PartCategory
	, '2' Status
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, @UserID StockAllocatedBy
	, GetDate() StockAllocatedDate
	, a.QtyOrder FirstDemandQty
FROM #SrvOrder a
inner join spMstItemPrice b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE a.TypeOfGoods = @TypeOfGoods

--===============================================================================================================================
-- INSERT SO SUPPLY
-- ==============================================================================================================================

SELECT * INTO #TempSOSupply FROM (
SELECT
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, @TempDocNo DocNo 
	, 0 SupSeq
	, a.PartNo 
	, a.PartNo PartNoOriginal
	, '' PickingSlipNo	
	, @JobOrderNo ReferenceNo
	, @DefaultDate ReferenceDate
	, '00' WarehouseCode
	, (SELECT LocationCode FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND WarehouseCode = '00'
		AND PartNo = a.PartNo) LocationCode
	, CASE WHEN 
		ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
			FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
			AND PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR
				FROM SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode =@BranchCode AND WarehouseCode = '00'	
				AND PartNo = a.PartNo),0)
		END AS QtySupply
	, 0 QtyPicked
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice *10 /100) RetailPriceIncltax
	, a.RetailPrice
	, b.CostPrice
	, a.DiscPct
	, (SELECT MovingCode FROM SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT ABCClass FROM SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo) 
		ABCClass
	, @ProductType ProductType
	, (SELECT PartCategory FROM SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo) 
		PartCategory
	, '1' Status
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, @UserID StockAllocatedBy
	, GetDate() StockAllocatedDate
FROM #SrvOrder a
inner join spMstItemPrice b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE a.TypeOfGoods = @TypeOfGoods
)#TempSOSupply

INSERT INTO SpTrnSOSupply SELECT 
	CompanyCode,BranchCode,DocNo,SupSeq,PartNo,PartNoOriginal
	, ROW_NUMBER() OVER(ORDER BY PartNo) PTSeq,PickingSlipNo
	, ReferenceNo,ReferenceDate,WarehouseCode,LocationCode
	, QtySupply,QtyPicked,QtyBill,RetailPriceIncltax,RetailPrice,CostPrice
	, DiscPct,MovingCode,ABCClass,ProductType,PartCategory,Status
	, CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate
FROM #TempSOSupply WHERE QtySupply > 0

--===============================================================================================================================
-- UPDATE STATUS DETAIL BASED ON SUPPLY
--===============================================================================================================================
UPDATE SpTrnSORDDtl
SET Status = 4
FROM SpTrnSORDDtl a, #TempSOSupply b
WHERE 1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo

--===============================================================================================================================
-- UPDATE HISTORY DEMAND ITEM AND CUSTOMER
--===============================================================================================================================

UPDATE SpHstDemandItem 
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpHstDemandItem a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = Year(GetDate())
	AND a.Month  = Month(GetDate())
	AND a.PartNo = b.PartNo
	AND b.DocNo = @TempDocNo

UPDATE SpHstDemandCust
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpHstDemandCust a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = Year(GetDate())
	AND a.Month  = Month(GetDate())
	AND a.PartNo = b.PartNo
	AND a.CustomerCode = @CustomerCode
	AND b.DocNo = @TempDocNo

INSERT INTO SpHstDemandItem
SELECT 
	CompanyCode
	, BranchCode
	, Year(GetDate()) Year
	, Month(GetDate()) Month
	, PartNo
	, 1 DemandFreq
	, QtyOrder DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= @CompanyCode AND a.BranchCode= @BranchCode AND a.DocNo = @TempDocNo -- add CompanyCode and BranchCode 13 Des 2010
 AND NOT EXISTS
( SELECT 1 FROM SpHstDemandItem WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = Month(GetDate())
	AND Year = Year(GetDate())
	AND PartNo = a.PartNo
)

INSERT INTO SpHstDemandCust
SELECT 
	CompanyCode
	, BranchCode
	, Year(GetDate()) Year
	, Month(GetDate()) Month
	, @CustomerCode CustomerCode
	, PartNo
	, 1 DemandFreq
	, (SELECT QtyOrder FROM SpTrnSORDDTl WITH (NOLOCK, NOWAIT) 
		WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
		AND DocNo = a.DocNo AND PartNo = a.PartNo) DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= @CompanyCode and a.BranchCode= @BranchCode AND a.DocNo = @TempDocNo -- add CompanyCode and BranchCode 13 Des 2010
AND NOT EXISTS
( SELECT PartNo FROM SpHstDemandCust WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = Month(GetDate())
	AND Year = Year(GetDate())
	AND PartNo = a.PartNo
)

--===============================================================================================================================
-- UPDATE LAST DEMAND DATE MASTER
--===============================================================================================================================
UPDATE SpMstItems 
SET LastDemandDate = GetDate()
FROM SpMstItems a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo
	AND b.DocNo = @TempDocNo

--===============================================================================================================================
-- UPDATE STOCK AND MOVEMENT
--===============================================================================================================================

UPDATE spMstItems
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpMstItems a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo

UPDATE spMstItemloc
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpMstItemLoc a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.WarehouseCode = '00'
	AND a.PartNo = b.PartNo

INSERT INTO SpTrnIMovement
SELECT
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, a.DocNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode 
		AND BranchCode = @BranchCode AND DocNo = a.DocNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),getdate()) CreatedDate 
	, '00' WarehouseCode
	, (SELECT LocationCode FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode =  @CompanyCode 
		AND BranchCode = @BranchCode AND DocNo = @TempDocNo AND PartNo = a.PartNo)
	  LocationCode
	, a.PartNo
	, 'OUT' SignCode
	, 'SA-NPJUAL' SubSignCode
	, a.QtySupply
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, @UserID CreatedBy
FROM #TempSOSupply a

--===============================================================================================================================
-- UPDATE SUPPLY SLIP TO SPK
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo)

SELECT * INTO #TempServiceItem FROM (
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND (a.SupplySlipNo = '' OR a.SupplySlipNo IS NULL)
) #TempServiceItem 

SELECT * INTO #TempServiceItemIns FROM( 
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND (a.SupplySlipNo <> '' OR a.SupplySlipNo IS NOT NULL)
) #TempServiceItemIns

UPDATE svTrnSrvItem
SET SupplySlipNo = b.DocNo
	, SupplySlipDate = ISNULL((SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
							AND DocNo = b.DocNo),@DefaultDate)
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq

--===============================================================================================================================
-- INSERT NEW SRV ITEM BASED SUPPLY SLIP
--===============================================================================================================================
INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq + 1
	, 0 DemandQty
	, 0 SupplyQty
	, 0 ReturnQty
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DocNo SupplySlipNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND DocNo = a.DocNo) SupplySlipDate
	, NULL SSReturnNo
	, NULL SSReturnDate
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, a.DiscPct
FROM #TempServiceItemIns a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType	

--===============================================================================================================================
DROP TABLE #TempServiceItem 
DROP TABLE #TempServiceItemIns
--===============================================================================================================================
-- INSERT PICKING HEADER AND DETAIL
--===============================================================================================================================

SET @MaxPickingList = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'PLS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempPickingList = ISNULL((SELECT 'PLS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxPickingList, 1), 6)),'PLS/YY/XXXXXX')
INSERT INTO SpTrnSPickingHdr 
SELECT 
	CompanyCode
	, BranchCode
	, @TempPickingList PickingSlipNo
	, GetDate() PickingSlipDate
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, '' PickedBy
	, CONVERT(BIT, 0) isBORelease
	, isSubstitution
	, isIncludePPN
	, TransType
	, SalesType
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, '' Remark
	, '0' Status
	, '0' PrintSeq
	, TypeOfGoods
	, CreatedBy
	, CreatedDate
	, LastUpdateBy
	, LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = (SELECT distinct DocNo FROM spTrnSORDDtl WHERE CompanyCode = @CompanyCode AND Branchcode = @BranchCode 
					AND DocNo = @TempDocNo AND QtySupply > 0)		

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'PLS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

-- ==============================================================================================================================
-- UPDATE SALES ORDER HEADER 
-- ==============================================================================================================================
UPDATE SpTrnSORDHdr
	SET ExPickingSlipNo = @TempPickingList,
		ExPickingSlipDate = ISNULL((SELECT PickingSlipDate FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode AND PickingSlipNo = @TempPickingList),'')
	
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo

UPDATE SpTrnSOSupply
	SET PickingSlipNo = @TempPickingList
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
-- ==============================================================================================================================
-- INSERT PICKING DETAIL
-- ==============================================================================================================================

INSERT INTO SpTrnSPickingDtl
SELECT 
	a.CompanyCode
	, a.BranchCode
	, @TempPickingList PickingSlipNo
	, '00' WarehouseCode
	, a.PartNo
	, a.PartNoOriginal
	, a.DocNo
	, b.DocDate 
	, a.ReferenceNo
	, a.ReferenceDate
	, a.LocationCode
	, a.QtySupply QtyOrder
	, a.QtySupply
	, a.QtySupply QtyPicked 
	, 0 QtyBill
	, a.RetailPriceInclTax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, a.SalesAmt
	, a.DiscAmt
	, a.NetSalesAmt
	, a.TotSalesAmt
	, a.MovingCode
	, a.ABCClass
	, a.ProductType
	, a.PartCategory
	, '' ExPickingSlipNo
	, @DefaultDate ExPickingSlipDate
	, CONVERT(BIT, 0) isClosed
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSORDDtl a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSORDHdr b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.DocNo = b.DocNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.DocNo = @TempDocNo
	AND a.QtySupply > 0

DROP TABLE #TempSOSupply

--================================================================================================================================
-- UPDATE AMOUNT HEADER
--================================================================================================================================
SELECT * INTO #TempHeader FROM (
SELECT 
	header.CompanyCode
	, header.BranchCode
	, header.DocNo
	, header.TotSalesQty
	, header.TotSalesAmt
	, header.TotDiscAmt
	, header.TotDPPAmt
	, floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100)) 
		TotPPNAmt
	, header.TotDPPAmt + floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100))
		TotFinalSalesAmt
FROM (
SELECT 
	CompanyCode
	, BranchCode
	, DocNo
	, SUM(QtyOrder) TotSalesQty
	, SUM(SalesAmt) TotSalesAmt
	, SUM(DiscAmt) TotDiscAmt
	, SUM(NetSalesAmt) TotDPPAmt
FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
GROUP BY CompanyCode
	, BranchCode
	, DocNo
) header ) #TempHeader

UPDATE SpTrnSORDHdr
SET 
	TotSalesQty = b.TotSalesQty
	, TotSalesAmt = b.TotSalesAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalSalesAmt = b.TotFinalSalesAmt
FROM SpTrnSORDHdr a, #TempHeader b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.DocNo = b.DocNo

DROP TABLE #TempHeader

FETCH NEXT FROM db_cursor INTO @TypeOfGoods
END
CLOSE db_cursor
DEALLOCATE db_cursor 

--===============================================================================================================================
-- Update Transdate
--===============================================================================================================================

update gnMstCoProfileSpare
set TransDate=getdate()
	, LastUpdateBy=@UserID
	, LastUpdateDate=getdate()
where CompanyCode = @CompanyCode AND BranchCode = @BranchCode

--===============================================================================================================================
-- DROP TABLE SECTION 
--===============================================================================================================================
DROP TABLE #SrvOrder
DROP TABLE #Item
END
GO

if object_id('uspfn_GetCustomerByProfitCenterCodeId') is not null
	drop procedure uspfn_GetCustomerByProfitCenterCodeId
GO
CREATE procedure [dbo].[uspfn_GetCustomerByProfitCenterCodeId]  
@CompanyCode varchar(15),  
@BranchCode varchar(15),  
@ProfitCenterCode varchar(15)  
as  
SELECT a.CustomerCode, a.CustomerName  
     , a.Address1 + ' ' + a.Address2 + ' ' + a.Address3 +' ' + a.Address4 as Address, 
     a.Address1, a.Address2, a.Address3, a.Address4  
  , c.LookupValue, c.LookUpValueName as ProfitCenter  
     , b.Salesman  
  FROM gnMstCustomer a with(nolock, nowait)  
 INNER JOIN gnMstCustomerProfitCenter b with(nolock, nowait)  
 ON b.CompanyCode = a.CompanyCode  
   AND b.CustomerCode = a.CustomerCode  
 INNER JOIN gnMstLookUpDtl c  
 ON c.CompanyCode = a.CompanyCode  
   AND c.CodeID = 'PFCN'  
   AND c.LookupValue = b.ProfitCenterCode  
 WHERE 1 = 1  
   AND a.CompanyCode = @CompanyCode  
   AND b.BranchCode = @BranchCode 
   AND b.ProfitCenterCode = @ProfitCenterCode  
   AND b.isBlackList = 0  
   AND c.LookupValue= b.ProfitCenterCode   
 AND a.Status = 1 order by a.CustomerCode
GO


if object_id('uspfn_GetLmpDoc') is not null
	drop procedure uspfn_GetLmpDoc
GO
CREATE procedure [dbo].[uspfn_GetLmpDoc] @CompanyCode varchar(15), @BranchCode varchar(15), @TypeOfGoods varchar(5), @TransType varchar(5), @CodeID varchar(6),@BeginDate datetime, @EndDate datetime  
as  
--declare @CompanyCode varchar(15)  
--declare @BranchCode varchar(15)  
--declare @TypeOfGoods varchar(15)  
--declare @TransType varchar(5)  
--declare @BeginDate Datetime  
--declare @EndDate datetime  
--set @CompanyCode = '6006406'  
--set @BranchCode = '6006401'  
--set @TypeOfGoods = '0'  
--set @TransType = '1%'  
--set @BeginDate = '2014/03/01'  
--set @EndDate = '2014/05/30'  
SELECT  
  
 spTrnSLmpHdr.CompanyCode,  
 spTrnSLmpHdr.BranchCode,  
 spTrnSLmpHdr.LmpNo  
,spTrnSLmpHdr.LmpDate  
,spTrnSLmpHdr.BPSFNo  
,spTrnSLmpHdr.BPSFDate  
,spTrnSLmpHdr.PickingSlipNo  
,spTrnSLmpHdr.PickingSlipDate  
,spTrnSLmpHdr.TypeOfGoods  
,spTrnSLmpHdr.CustomerCodeShip  
,spTrnSLmpHdr.CustomerCode  
,spTrnSLmpHdr.CustomerCodeBill  
,spTrnSLmpHdr.TotSalesQty  
,spTrnSLmpHdr.TotSalesAmt  
,spTrnSLmpHdr.TotDiscAmt  
,spTrnSLmpHdr.TotDPPAmt  
,spTrnSLmpHdr.TotPPNAmt  
,spTrnSLmpHdr.CreatedBy  
,spTrnSLmpHdr.CreatedDate  
,spTrnSLmpHdr.LastUpdateBy  
,spTrnSLmpHdr.LastUpdateDate  
,spTrnSLmpHdr.LastUpdateBy  
,spTrnSLmpHdr.LastUpdateDate  
,spTrnSLmpHdr.isPKP  
,spTrnSLmpHdr.isLocked  
,spTrnSLmpHdr.LockingBy  
,spTrnSLmpHdr.LockingDate,  
 spTrnSLmpHdr.CustomerCode,  
 spTrnSLmpHdr.CustomerCodeShip,
 spTrnSLmpHdr.CustomerCodeBill as CustomerCodeTagih,
 b.CustomerName,  
 b.Address1,  
 b.Address2,  
 b.Address3,  
 b.Address4,  
 b.CustomerName CustomerNameTagih,  
 b.Address1 Address1Tagih,  
 b.Address2 Address2Tagih,  
 b.Address3 Address3Tagih,  
 b.Address4 Address4Tagih,  
 c.LookUpValueName TransType    
  
FROM spTrnSLmpHdr  
left join gnMstCustomer b  
ON spTrnSLmpHdr.CompanyCode = b.CompanyCode  
AND spTrnSLmpHdr.CustomerCode = b.CustomerCode   
left join gnMstLookupDtl c on  
spTrnSLmpHdr.CompanyCode = c.CompanyCode  
and spTrnSLmpHdr.TransType= c.LookupValue   
AND c.CodeID = @CodeID  
WHERE spTrnSLmpHdr.CompanyCode=@CompanyCode  
  AND spTrnSLmpHdr.BranchCode=@BranchCode  
  AND spTrnSLmpHdr.TypeOfGoods = @TypeOfGoods   
  AND Convert(Varchar, spTrnSLmpHdr.LmpDate, 111) between @BeginDate and @EndDate  
  AND TransType LIKE @TransType  
ORDER BY spTrnSLmpHdr.LmpNo DESC
GO


if object_id('uspfn_GetLookupLMP') is not null
	drop procedure uspfn_GetLookupLMP
GO
CREATE PROCEDURE [dbo].[uspfn_GetLookupLMP] @CompanyCode varchar(15), @BranchCode varchar(15), @SalesType varchar(15), @CodeID varchar(6),  
@TypeOfGoods varchar(15), @ProductType varchar(15)  
as  
SELECT * FROM   
(  
SELECT  
 PickingSlipNo, PickingSlipDate,  
 BPSFNo, BPSFDate,  
 (  
   SELECT TOP 1 PRODUCTTYPE FROM spTrnSBPSFDtl  
  WHERE spTrnSBPSFHdr.CompanyCode = spTrnSBPSFDtl.CompanyCode  
  AND spTrnSBPSFHdr.BranchCode = spTrnSBPSFDtl.BranchCode  
  AND spTrnSBPSFHdr.BPSFNo = spTrnSBPSFDtl.BPSFNo  
 ) AS ProductType,
 spTrnSBPSFHdr.CustomerCode,
 spTrnSBPSFHdr.CustomerCodeShip,
 spTrnSBPSFHdr.CustomerCodeBill as CustomerCodeTagih,
 b.CustomerName,
 b.Address1,
 b.Address2,
 b.Address3,
 b.Address4,
 b.CustomerName CustomerNameTagih,
 b.Address1 Address1Tagih,
 b.Address2 Address2Tagih,
 b.Address3 Address3Tagih,
 b.Address4 Address4Tagih,
 c.LookUpValueName TransType  
FROM spTrnSBPSFHdr  
left join gnMstCustomer b
ON spTrnSBPSFHdr.CompanyCode = b.CompanyCode
AND spTrnSBPSFHdr.CustomerCode = b.CustomerCode 
left join gnMstLookupDtl c on
spTrnSBPSFHdr.CompanyCode = c.CompanyCode
and spTrnSBPSFHdr.TransType= c.LookupValue 
AND c.CodeID = @CodeID
WHERE spTrnSBPSFHdr.CompanyCode = @CompanyCode  
AND spTrnSBPSFHdr.BranchCode    = @BranchCode  
AND spTrnSBPSFHdr.SalesType     = @SalesType  
AND spTrnSBPSFHdr.TypeOfGoods   = @TypeOfGoods  
AND (spTrnSBPSFHdr.Status = '1' OR spTrnSBPSFHdr.Status = '0')  
AND (spTrnSBPSFHdr.PickingSlipNo NOT IN (SELECT PickingSlipNo FROM spTrnSLmpHdr where CompanyCode = @CompanyCode AND BranchCode = @BranchCode))  
) A  
WHERE A.ProductType = @ProductType  
ORDER BY A.PickingSlipNo DESC
GO


if object_id('uspfn_GetLookupLMP4Srv') is not null
	drop procedure uspfn_GetLookupLMP4Srv
GO
CREATE procedure [dbo].[uspfn_GetLookupLMP4Srv]   
@CompanyCode varchar(15), @BranchCode varchar(15),  
@SalesType varchar(5), @TypeOfGoods varchar(15), @ProductType varchar(15)  
as  
SELECT * FROM   
(  
SELECT  
 PickingSlipNo,   
 PickingSlipDate,  
 BPSFNo,   
 BPSFDate,  
 (  
   SELECT TOP 1 PRODUCTTYPE FROM spTrnSBPSFDtl  
  WHERE spTrnSBPSFHdr.CompanyCode = spTrnSBPSFDtl.CompanyCode  
  AND spTrnSBPSFHdr.BranchCode = spTrnSBPSFDtl.BranchCode  
  AND spTrnSBPSFHdr.BPSFNo = spTrnSBPSFDtl.BPSFNo  
 ) AS ProductType,
 spTrnSBPSFHdr.CustomerCode,
 spTrnSBPSFHdr.CustomerCodeShip,
 spTrnSBPSFHdr.CustomerCodeBill as CustomerCodeTagih,
 b.CustomerName,
 b.Address1,
 b.Address2,
 b.Address3,
 b.Address4,
 b.CustomerName CustomerNameTagih,
 b.Address1 Address1Tagih,
 b.Address2 Address2Tagih,
 b.Address3 Address3Tagih,
 b.Address4 Address4Tagih,
 c.LookUpValueName TransType  
FROM spTrnSBPSFHdr  
join gnMstCustomer b
ON spTrnSBPSFHdr.CompanyCode = b.CompanyCode
AND spTrnSBPSFHdr.CustomerCode = b.CustomerCode
join gnMstLookupDtl c on
spTrnSBPSFHdr.CompanyCode = c.CompanyCode
and spTrnSBPSFHdr.TransType= c.LookupValue 
AND c.CodeID = 'TTSR'
WHERE spTrnSBPSFHdr.CompanyCode = @CompanyCode  
AND spTrnSBPSFHdr.BranchCode    = @BranchCode  
AND spTrnSBPSFHdr.SalesType     = @SalesType  
AND spTrnSBPSFHdr.TypeOfGoods   = @TypeOfGoods  
AND (spTrnSBPSFHdr.Status = '1' OR spTrnSBPSFHdr.Status = '0')  
AND (spTrnSBPSFHdr.PickingSlipNo NOT IN (SELECT PickingSlipNo FROM spTrnSLmpHdr where CompanyCode = @CompanyCode AND BranchCode = @BranchCode))  
) A  
WHERE A.ProductType = @ProductType
GO


if object_id('uspfn_GnUpdateTax') is not null
	drop procedure uspfn_GnUpdateTax
GO
/****** Object:  StoredProcedure [dbo].[uspfn_GnUpdateTax]    Script Date: 12/15/2011 16:56:31 ******/
-- uspfn_GnUpdateTax '6051401','010.000-13.00000004','INV: 0','FPS/13/000003','ga'
CREATE procedure [dbo].[uspfn_GnUpdateTax]
   @CompanyCode	varchar(15),
   @FPJGovNo	varchar(50),
   @FPJGovNoNew	varchar(50),
   @DocNo    	varchar(20),
   @UserID      varchar(15)

as

declare @errmsg varchar(max)

if exists (select * from gnGenerateTax
		where 1 = 1
		  and CompanyCode = @CompanyCode
		  and right(FPJGovNo, 8) = right(@FPJGovNoNew, 8)
		  and right(FPJGovNo, 8) <> right(@FPJGovNo, 8)
		  and @FPJGovNoNew not like 'IN%'
		)
begin
	set @errmsg = N'Dear ' + isnull((select FullName from sysuser where userid = @UserID), @UserID) + ',' + char(13)
				+ N'No Pajak ' + @FPJGovNoNew + ' sudah pernah digunakan untuk generate pajak'
				+ char(13) + 'Silahkan di coba nomor yang lain'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	select @errmsg
	return
end

if (@FPJGovNoNew not like 'IN%')
begin
	-- update gnGenerateTax
	;with x as (
	select 'TAX' code, FPJGovNo, @FPJGovNoNew FPJNew, DocNo, DocDate, CreatedDate
	  from gnGenerateTax 
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and FPJGovNo = @FPJGovNo
	   and DocNo = @DocNo
	)
	update x set FPJGovNo = FPJNew
end
else
begin
	delete GnGenerateTax where CompanyCode = @CompanyCode and FPJGovNo = @FPJGovNo and DocNo = @Docno
end

-- update Sparepart
;with x as (
select 'SP' code, FPJGovNo, @FPJGovNoNew FPJNew, InvoiceNo, InvoiceDate, CreatedDate
  from SPTrnSFPJHdr
 where 1 = 1
   and CompanyCode = @CompanyCode
   and FPJGovNo = @FPJGovNo
   and InvoiceNo = @DocNo
)
update x set FPJGovNo = FPJNew

-- update Service
;with x as (
select 'SV' code, FPJGovNo, @FPJGovNoNew FPJNew, FPJNo, FPJDate, CreatedDate
  from svTrnFakturPajak
 where 1 = 1
   and CompanyCode = @CompanyCode
   and FPJGovNo = @FPJGovNo
   and FPJNo = @DocNo
)
--select * from x
update x set FPJGovNo = FPJNew

-- update Sales
;with x as (
select 'SL' code, FakturPajakNo, @FPJGovNoNew FPJNew, InvoiceNo, InvoiceDate, CreatedDate
  from OmFakturPajakHdr
 where 1 = 1
   and CompanyCode = @CompanyCode
   and FakturPajakNo = @FPJGovNo
   and InvoiceNo = @DocNo
)
--select * from x
update x set FakturPajakNo = FPJNew

-- update Finance
;with x as (
select 'FN' code, FakturPajakNo, @FPJGovNoNew FPJNew, InvoiceNo, InvoiceDate, CreatedDate
  from ArFakturPajakHdr
 where 1 = 1
   and CompanyCode = @CompanyCode
   and FakturPajakNo = @FPJGovNo
   and InvoiceNo = @DocNo
)
--select * from x
update x set FakturPajakNo = FPJNew

-- update Konsolidasi Pajak Keluaran
;with x as (
select 'KPK' code, TaxNo FakturPajakNo, @FPJGovNoNew FPJNew, FPJNo
	, case when TaxNo = FPJNo then @FPJGovNoNew
		else FPJNo
	end FPJNoNew
	, case when ProfitCenter='200' then FPJNo 
		else ReferenceNo
	end InvoiceNo
	, case when ProfitCenter='200' then FPJDate 
		else ReferenceDate
	end InvoiceDate
	, CreatedDate
  from gnTaxOut
 where 1 = 1
   and CompanyCode = @CompanyCode
   and TaxNo = @FPJGovNo
   and (case when ProfitCenter='200' then FPJNo else ReferenceNo end) = @DocNo
)
--select * from x
update x 
set FakturPajakNo = FPJNew
	,FPJNo= FPJNoNew

insert into gnTaxHistUpd(CompanyCode,FPJGovNo,FPJGovNoNew,DocNo,UpdateBy,UpdateDate)
     values(@CompanyCode, @FPJGovNo, @FPJGovNoNew, @DocNo, @UserID, getdate())
GO



if object_id('uspfn_LookupCustomer') is not null
	drop procedure uspfn_LookupCustomer
GO
create procedure [dbo].[uspfn_LookupCustomer]
	@CompanyCode varchar(13),
	@BranchCode varchar(13)
as
begin
	SELECT distinct 
	       a.CustomerCode
		 , a.CustomerName
	     , isnull(a.Address1,'') + ' ' + isnull(a.Address2,'') + ' ' + isnull(a.Address3,'') +' ' + isnull(a.Address4,'') as Address
		 , a.Address1
		 , a.Address2
		 , a.Address3
		 , a.Address4
		 , '' as LookupValue
		 , '' as ProfitCenter
	  FROM gnMstCustomer a with(nolock, nowait)
	  left JOIN gnMstCustomerProfitCenter b with(nolock, nowait)
		ON b.CompanyCode = a.CompanyCode
	   AND b.CustomerCode = a.CustomerCode
	   AND b.BranchCode = @BranchCode
	   AND b.isBlackList = 0
	  left JOIN gnMstLookUpDtl c
		ON c.CompanyCode = a.CompanyCode
	   AND c.CodeID = 'PFCN'
	   AND c.LookupValue = b.ProfitCenterCode
	 WHERE 1 = 1
	   AND a.CompanyCode = @CompanyCode
end
GO


if object_id('uspfn_LookupCustomerview') is not null
	drop procedure uspfn_LookupCustomerview
GO
CREATE procedure [dbo].[uspfn_LookupCustomerview] (  @CompanyCode varchar(10), @BranchCode varchar(10) )
 as
SELECT distinct a.CustomerCode, a.CustomerName
     , isnull(a.Address1,'') + ' ' + isnull(a.Address2,'') + ' ' + isnull(a.Address3,'') +' ' + isnull(a.Address4,'') as Address
	 , '' LookupValue, '' as ProfitCenter
  FROM gnMstCustomer a with(nolock, nowait)
 left JOIN gnMstCustomerProfitCenter b with(nolock, nowait)
	ON b.CompanyCode = a.CompanyCode
   AND b.CustomerCode = a.CustomerCode
   AND b.BranchCode = @BranchCode
   AND b.isBlackList = 0
 left JOIN gnMstLookUpDtl c
	ON c.CompanyCode = a.CompanyCode
   AND c.CodeID = 'PFCN'
   AND c.LookupValue = b.ProfitCenterCode
 WHERE 1 = 1
   AND a.CompanyCode = @CompanyCode
GO

if object_id('uspfn_OmGetJournalDebetCredit') is not null
	drop procedure uspfn_OmGetJournalDebetCredit
GO
--declare @CompanyCode varchar(20)
--declare @BranchCode  varchar(20)
--declare @TypeJournal  varchar(20)
--declare @DocNo   varchar(20)

--set @CompanyCode = '6558201'
--set @BranchCode  = '655820100'
--set @TypeJournal = 'invoice'
--set @DocNo       = 'IVU/13/001280'

-- =============================================
-- Author:		<xxxxxx>
-- Create date: <xxxxxx>
-- Description:	<Get Journal>
-- Last Update By:	<yo - 29 Nov 2013>
-- =============================================

CREATE procedure [dbo].[uspfn_OmGetJournalDebetCredit]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@TypeJournal varchar(50),
	@DocNo       varchar(50)
as 

declare @t_journal as table (
	SeqCode     varchar(50),
	TypeTrans   varchar(50),
	AccountNo   varchar(50),
	AccountDesc varchar(100),
	AmountDb    decimal,
	AmountCr    decimal
)

--#region TypeJournal = 'TRANSFEROUT'
if @TypeJournal = 'TRANSFEROUT'
begin
	declare @t_trans_out as table (
		CompanyCode varchar(50),
		BranchCode  varchar(50),
		DocInfo     varchar(50),
		Amount      decimal
	)

	insert into @t_trans_out
	select a.CompanyCode, a.BranchCode, a.SalesModelCode 
		 , isnull(b.CogsUnit, 0) + isnull(b.COGSKaroseri, 0) + isnull(b.COGSOthers, 0)
	  from omTrInventTransferOutDetail a
	  left join omMstVehicle b on 1 = 1
	   and b.CompanyCode = a.CompanyCode
	   and b.ChassisCode = a.ChassisCode
	   and b.ChassisNo   = a.ChassisNo
	   and b.EngineCode  = a.EngineCode
	   and b.EngineNo    = a.EngineNo
	   and b.SalesModelCode = a.SalesModelCode
	   and b.SalesModelYear = a.SalesModelYear
	 where 1 = 1 
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode  = @BranchCode
	   and a.TransferOutNo = @DocNo

	insert into @t_journal
	select '01', 'PSEMENTARA'
		 , isnull(b.InTransitTransferStockAccNo,'')
		 , isnull(c.Description,'')
		 , a.Amount
		 , 0
	  from @t_trans_out a
		left join omMstModelAccount b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
			and a.DocInfo=b.SalesModelCode
		left join gnMstAccount c on b.CompanyCode=c.CompanyCode and b.BranchCode=c.BranchCode
			and b.InTransitTransferStockAccNo= c.AccountNo

	insert into @t_journal
	select '02', 'INVENTORY'
		 , isnull(b.InventoryAccNo,'')
		 , isnull(c.Description,'')
		 , 0
		 , a.Amount
	  from @t_trans_out a
		left join omMstModelAccount b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
			and a.DocInfo=b.SalesModelCode
		left join gnMstAccount c on b.CompanyCode=c.CompanyCode and b.BranchCode=c.BranchCode
			and b.InventoryAccNo= c.AccountNo
end

--#region TypeJournal = 'TRANSFERIN'
if @TypeJournal = 'TRANSFERIN'
begin
	declare @t_trans_in as table (
		CompanyCode varchar(50),
		BranchCode  varchar(50),
		BranchCodeFrom varchar(50),
		BranchCodeTo varchar(50),
		DocInfo     varchar(50),
		Amount      decimal
	)

	insert into @t_trans_in
	select a.CompanyCode, a.BranchCode, c.BranchCodeFrom, c.BranchCodeTo, a.SalesModelCode 
		 , isnull(b.CogsUnit, 0) + isnull(b.COGSKaroseri, 0) + isnull(b.COGSOthers, 0)
	  from omTrInventTransferInDetail a
	  left join omMstVehicle b on 1 = 1
	   and b.CompanyCode = a.CompanyCode
	   and b.ChassisCode = a.ChassisCode
	   and b.ChassisNo   = a.ChassisNo
	   and b.EngineCode  = a.EngineCode
	   and b.EngineNo    = a.EngineNo
	   and b.SalesModelCode = a.SalesModelCode
	   and b.SalesModelYear = a.SalesModelYear
	  left join omTrInventTransferIn c on 1 = 1
	   and c.CompanyCode = a.CompanyCode
	   and c.BranchCode  = a.BranchCode
	   and c.TransferInNo = a.TransferInNo
	 where 1 = 1 
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode  = @BranchCode
	   and a.TransferInNo = @DocNo

	insert into @t_journal
	select '01', 'INVENTORY'
		 , isnull(b.InventoryAccNo,'')
		 , isnull(c.Description,'')
		 , a.Amount
		 , 0
	  from @t_trans_in a
		left join omMstModelAccount b on a.CompanyCode=b.CompanyCode and a.BranchCodeTo=b.BranchCode
			and a.DocInfo=b.SalesModelCode
		left join gnMstAccount c on b.CompanyCode=c.CompanyCode and b.BranchCode=c.BranchCode
			and b.InventoryAccNo= c.AccountNo

	insert into @t_journal
	select '02', 'PSEMENTARA'
		 , isnull(b.InTransitTransferStockAccNo,'')
		 , isnull(c.Description,'')
		 , 0
		 , a.Amount
	  from @t_trans_in a
		left join omMstModelAccount b on a.CompanyCode=b.CompanyCode and a.BranchCodeFrom=b.BranchCode
			and a.DocInfo=b.SalesModelCode
		left join gnMstAccount c on b.CompanyCode=c.CompanyCode and b.BranchCode=c.BranchCode
			and b.InTransitTransferStockAccNo= c.AccountNo

end
--#endregion

--#region TypeJournal = 'TRANSFEROUTMULTIBRANCH'
if @TypeJournal = 'TRANSFEROUTMULTIBRANCH'
begin
	declare @t_trans_outMB as table (
	CompanyCode		varchar(50),
	BranchCode		varchar(50),
	CompanyCodeTo	varchar(50),
	DocInfo			varchar(50),
	Amount			decimal
	)

	insert into @t_trans_outMB
		select a.CompanyCode
			, a.BranchCode
			, b.CompanyCodeTo
			, a.SalesModelCode 
			, isnull(a.CogsUnit, 0) + isnull(a.COGSKaroseri, 0) + isnull(a.COGSOthers, 0)
		from omTrInventTransferOutDetailMultiBranch a
		left join omTrInventTransferOutMultiBranch b on b.CompanyCode = a.CompanyCode
			and b.BranchCode = a.BranchCode
			and b.TransferOutNo = a.TransferOutNo
		where 1 = 1 
		   and a.CompanyCode = @CompanyCode
		   and a.BranchCode  = @BranchCode
		   and a.TransferOutNo = @DocNo
		   
	insert into @t_journal
	select '01', 'PSEMENTARA'
		 , isnull(b.InterCompanyAccNoTo,'')
		 , isnull(c.Description,'')
		 , a.Amount
		 , 0
	from @t_trans_outMB a
	left join omMstCompanyAccount b on b.CompanyCode = a.CompanyCode
		and b.CompanyCodeTo = a.CompanyCodeTo
	left join gnMstAccount c on b.CompanyCode=c.CompanyCode and a.BranchCode=c.BranchCode
		and b.InterCompanyAccNoTo = c.AccountNo

	insert into @t_journal
	select '02', 'INVENTORY'
		 , isnull(b.InventoryAccNo,'')
		 , isnull(c.Description,'')
		 , 0
		 , a.Amount
	from @t_trans_outMB a
	left join omMstModelAccount b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
		and a.DocInfo=b.SalesModelCode
	left join gnMstAccount c on b.CompanyCode=c.CompanyCode and b.BranchCode=c.BranchCode
		and b.InventoryAccNo= c.AccountNo
end
--#endregion

--#region TypeJournal = 'TRANSFERINMULTIBRANCH'
if @TypeJournal = 'TRANSFERINMULTIBRANCH'
begin
	declare @t_trans_inMB as table (
	CompanyCode		varchar(50),
	BranchCode		varchar(50),
	CompanyCodeFrom	varchar(50),
	BranchCodeFrom	varchar(50),
	BranchCodeTo	varchar(50),
	DocInfo			varchar(50),
	Amount			decimal
)

insert into @t_trans_inMB
	select a.CompanyCode
		, a.BranchCode
		, b.CompanyCodeFrom
		, b.BranchCodeFrom
		, b.BranchCodeTo
		, a.SalesModelCode 
		, isnull(a.CogsUnit, 0) + isnull(a.COGSKaroseri, 0) + isnull(a.COGSOthers, 0)
	from omTrInventTransferInDetailMultiBranch a
	left join omTrInventTransferInMultiBranch b on 1 = 1
	   and b.CompanyCode	= a.CompanyCode
	   and b.BranchCode		= a.BranchCode
	   and b.TransferInNo	= a.TransferInNo
	where 1 = 1 
	   and a.CompanyCode	= @CompanyCode
	   and a.BranchCode		= @BranchCode
	   and a.TransferInNo	= @DocNo

	insert into @t_journal
	select '01', 'INVENTORY'
		 , isnull(b.InventoryAccNo,'')
		 , isnull(c.Description,'')
		 , a.Amount
		 , 0
	  from @t_trans_inMB a
		left join omMstModelAccount b on a.CompanyCode=b.CompanyCode and a.BranchCodeTo=b.BranchCode
			and a.DocInfo=b.SalesModelCode
		left join gnMstAccount c on b.CompanyCode=c.CompanyCode and b.BranchCode=c.BranchCode
			and b.InventoryAccNo= c.AccountNo
			
	insert into @t_journal
	select '02', 'PSEMENTARA'
		 , isnull(b.InterCompanyAccNoTo,'')
		 , isnull(c.Description,'')
		 , 0
		 , a.Amount
	from @t_trans_inMB a
	left join omMstCompanyAccount b on b.CompanyCode = a.CompanyCode
		and b.CompanyCodeTo = a.CompanyCodeFrom
	left join gnMstAccount c on b.CompanyCode=c.CompanyCode and a.BranchCode=c.BranchCode
		and b.InterCompanyAccNoTo = c.AccountNo

end
--#endregion

--#region TypeJournal = 'KAROSERI'
if @TypeJournal = 'KAROSERI'
begin
	declare @t_karoseri as table (
		SeqCode     varchar(50),
		TypeTrans   varchar(50),
		AccountNo   varchar(50),
		AmountDb    decimal,
		AmountCr    decimal
	)
	
	insert into @t_karoseri
	select '01', 'INVENTORY'
		 , isnull((
			select acc.InventoryAccNo
			  from omMstModelAccount acc
			 where 1 = 1
			   and acc.CompanyCode    = a.CompanyCode
			   and acc.BranchCode     = a.BranchCode
			   and acc.SalesModelCode = a.SalesModelCodeNew
			), '')
		 , sum(isnull(c.COGSKaroseri, 0) + (isnull(c.COGsUnit, 0) + isnull(c.COGsOthers, 0)))
		 , 0
	  from OmTrPurchaseKaroseriTerima a, OmTrPurchaseKaroseriTerimaDetail b, OmMstVehicle c
	 where 1 = 1
	   and c.CompanyCode = b.CompanyCode 
	   and c.ChassisCode = b.ChassisCode
	   and c.ChassisNo = b.ChassisNo 
	   and b.CompanyCode = a.CompanyCode 
	   and b.BranchCode = a.BranchCode
	   and b.KaroseriTerimaNo = a.KaroseriTerimaNo
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.KaroseriTerimaNo = @DocNo 
	 group by a.CompanyCode, a.BranchCode, a.KaroseriTerimaNo, a.SalesModelCodeNew
	having sum(isnull(a.Quantity, 0) * (isnull(a.Total, 0) - isnull(a.PPn, 0))) > 0

	insert into @t_karoseri
	select '02', 'PPN'
		 , isnull((
			select cls.TaxInAccNo
			  from gnMstSupplierProfitCenter sup, gnMstSupplierClass cls
			 where 1 = 1
			   and cls.CompanyCode   = sup.CompanyCode
			   and cls.BranchCode    = sup.BranchCode
			   and cls.SupplierClass = sup.SupplierClass
			   and sup.CompanyCode   = a.CompanyCode
			   and sup.BranchCode    = a.BranchCode
			   and sup.SupplierCode  = a.SupplierCode
			   and sup.ProfitCenterCode = '100'
			), '')
		 , sum(isnull(a.Quantity, 0) * isnull(a.PPn, 0))
		 , 0
	  from OmTrPurchaseKaroseriTerima a
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.KaroseriTerimaNo = @DocNo 
	 group by a.CompanyCode, a.BranchCode, a.KaroseriTerimaNo, a.SalesModelCodeNew, a.SupplierCode
	having sum(isnull(a.Quantity, 0) * (isnull(a.Total, 0) - isnull(a.PPn, 0))) > 0

	insert into @t_karoseri
	select '03', 'INVENTORY'
		 , isnull((
			select acc.InventoryAccNo
			  from omMstModelAccount acc
			 where 1 = 1
			   and acc.CompanyCode    = a.CompanyCode
			   and acc.BranchCode     = a.BranchCode
			   and acc.SalesModelCode = a.SalesModelCodeOld
			), '')
		 , 0
		 , sum((isnull (c.COGsUnit, 0) + isnull (c.COGsOthers, 0 )))
	  from OmTrPurchaseKaroseriTerima a, OmTrPurchaseKaroseriTerimaDetail b, OmMstVehicle c
	 where 1 = 1
	   and c.CompanyCode = b.CompanyCode 
	   and c.ChassisCode = b.ChassisCode
	   and c.ChassisNo = b.ChassisNo 
	   and b.CompanyCode = a.CompanyCode 
	   and b.BranchCode = a.BranchCode
	   and b.KaroseriTerimaNo = a.KaroseriTerimaNo
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.KaroseriTerimaNo = @DocNo 
	 group by a.CompanyCode, a.BranchCode, a.KaroseriTerimaNo, a.SalesModelCodeOld
	having sum(isnull(a.Quantity, 0) * (isnull(a.Total, 0) - isnull(a.PPn, 0))) > 0

	insert into @t_karoseri
	select '04', 'AP'
		 , isnull((
			select cls.PayableAccNo
			  from gnMstSupplierProfitCenter sup, gnMstSupplierClass cls
			 where 1 = 1
			   and cls.CompanyCode   = sup.CompanyCode
			   and cls.BranchCode    = sup.BranchCode
			   and cls.SupplierClass = sup.SupplierClass
			   and sup.CompanyCode   = a.CompanyCode
			   and sup.BranchCode    = a.BranchCode
			   and sup.SupplierCode  = a.SupplierCode
			   and sup.ProfitCenterCode = '100'
			), '')
		 , 0
		 , sum(isnull(a.Quantity, 0) * isnull(a.Total, 0))
	  from OmTrPurchaseKaroseriTerima a
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.KaroseriTerimaNo = @DocNo 
	 group by a.CompanyCode, a.BranchCode, a.KaroseriTerimaNo, a.SupplierCode
	having sum(isnull(a.Quantity, 0) * (isnull(a.Total, 0) - isnull(a.PPn, 0))) > 0

	insert into @t_journal
	select a.SeqCode, a.TypeTrans, a.AccountNo
		 , b.Description AccountDesc, a.AmountDb, a.AmountCr    
	  from @t_karoseri a, gnMstAccount b
	 where b.CompanyCode = @CompanyCode
	   and b.BranchCode = @BranchCode 
	   and b.AccountNo = a.AccountNo
end
--#endregion

--#region TRANS TYPE PURCHASE
IF @TypeJournal = 'PURCHASE'
	BEGIN	
	Select * into #t1 from
	(
		select distinct a.CompanyCode
				, a.BranchCode
				, a.HPPNo
				, a.BPUNo
				, a.SalesModelCode
				, a.SalesModelYear
				, a.OthersCode
				, isnull(b.OthersNonInventoryAccNo,'') AccountNo
				, SUM(a.OthersDPP) DPP
				, SUM(a.OthersPPN) PPN
		from omTrPurchaseHPPDetailModelOthers a
		left join omMstOthersNonInventory b on a.CompanyCode = b.CompanyCode
			and a.BranchCode = b.BranchCode
			--and a.OthersCode = b.OthersNonInventory
		where a.CompanyCode = @CompanyCode
			and a.BranchCode = @BrancHCode
			and a.HPPNo = @DocNo
			and isnull(b.OthersNonInventoryAccNo,'') <> ''
		group by a.CompanyCode, a.BranchCode, a.HPPNo, a.BPUNo, a.SalesModelCode, a.SalesModelYear, a.OthersCode,b.OthersNonInventoryAccNo		
	)#t1


	select * into #Inventory from(
	select 'INVENTORY' CodeTrans
		 , @DocNo DocNo
		 , a.SalesModelCode DocInfo
		 , isnull((
			select acc.InventoryAccNo
			  from omMstModelAccount acc
			 where 1 = 1
			   and acc.CompanyCode    = a.CompanyCode
			   and acc.BranchCode     = a.BranchCode
			   and acc.SalesModelCode = a.SalesModelCode
			), '') AccountNo
		 , isnull(a.Quantity, 0) Quantity
		 , isnull(a.AfterDiscDPP, 0) AfterDiscDPP	 
		 , case when (select COUNT(*) from #t1 where HPPNo = a.HppNo and BPUNo = a.BPUNo and SalesModelCode = a.SalesModelCode and SalesModelYear = SalesModelYear) > 0 
			   then isnull((select distinct (b.DPP)
					from #t1 b
					where b.CompanyCode = a.CompanyCode
						and b.BranchCode = a.BranchCode
						and b.HPPNo = a.HPPNo
						and b.SalesModelCode = a.SalesModelCode
						and b.SalesModelYear = a.SalesModelYear
						and b.OthersCode not in (select distinct OthersNonInventory 
								from omMstOthersNonInventory))
				, 0) else a.OthersDPP end OthersDPP
		 , isnull(a.AfterDiscPPnBM, 0) AfterDiscPPnBM
		 , 0 AmountCr
	  from omTrPurchaseHPPDetailModel a
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.HPPNo       = @DocNo 
	)#Inventory

	insert into @t_journal
	select	1
			, a.CodeTrans
			, a.AccountNo
			, '' AccountDesc
			, sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscDPP, 0) + isnull(a.OthersDPP, 0) + isnull(a.AfterDiscPPnBM, 0))) as DPP
			, 0
	  from #Inventory a
	 group by a.CodeTrans, a.DocNo, a.DocInfo, a.AccountNo
	having sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscDPP, 0) + isnull(a.OthersDPP, 0) + isnull(a.AfterDiscPPnBM, 0))) > 0

	select * into #OthersInv from(
	select 'OTHERS' CodeTrans 		
		 , isnull((
			select acc.OthersNonInventoryAccNo
			  from omMstOthersNonInventory acc
			 where 1 = 1
			   and acc.CompanyCode    = a.CompanyCode
			   and acc.BranchCode     = a.BranchCode
			   and acc.OthersNonInventory = b.OthersCode
			), '') AccountNo
		 , '' AccountDesc
		 , sum(isnull(a.Quantity, 0) * isnull(b.DPP, 0)) as DPP
		 , 0 AmountCr
	  from omTrPurchaseHPPDetailModel a, #t1 b
	 where 1 = 1
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.HPPNo       = @DocNo 
	   and a.SalesModelCode = b.SalesModelCode
	   and a.SalesModelYear = b.SalesModelYear
	   and a.BPUNo = b.BPUNo
	   and b.OthersCode in (select distinct OthersNonInventory 
									from omMstOthersNonInventory) 
	 group by a.CompanyCode, a.BranchCode, a.HPPNo, a.SalesModelCode,a.SalesModelYear,b.OthersCode,b.AccountNo
	having sum(isnull(a.Quantity, 0) * isnull(b.DPP, 0)) > 0
	)#OthersInv
	
	insert into @t_journal
	select 2, CodeTrans			
			, AccountNo
			, '' AccountDesc
			, SUM(DPP)
			, AmountCr
	  from #OthersInv a
	 group by a.CodeTrans, a.AccountNo,a.AmountCr
	having sum(DPP) > 0

	insert into @t_journal
	select 3, 'PPN'
		 , isnull((
			select cls.TaxInAccNo
			  from omTrPurchaseHPP pur, gnMstSupplierProfitCenter sup, gnMstSupplierClass cls
			 where 1 = 1
			   and sup.CompanyCode   = pur.CompanyCode
			   and sup.BranchCode    = pur.BranchCode
			   and sup.SupplierCode  = pur.SupplierCode
			   and sup.ProfitCenterCode = '100'
			   and cls.CompanyCode   = pur.CompanyCode
			   and cls.BranchCode    = pur.BranchCode
			   and cls.SupplierClass = sup.SupplierClass
			   and sup.CompanyCode   = @CompanyCode
			   and sup.BranchCode    = @BranchCode
			   and pur.HPPNo         = @DocNo
			), '')
		 , '' AccountDesc
		 , sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscPPn, 0) + isnull(a.OthersPPn, 0))) as PPN
		 , 0
	  from omTrPurchaseHPPDetailModel a, omTrPurchaseHPP b
	 where 1 = 1
	   and b.CompanyCode = a.CompanyCode 
	   and b.BranchCode  = a.BranchCode
	   and b.HPPNo       = a.HPPNo
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.HPPNo       = @DocNo 
	 group by a.CompanyCode, a.BranchCode, a.HPPNo, b.SupplierCode
	having sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscDPP, 0) + isnull(a.OthersDPP, 0) + isnull(a.AfterDiscPPnBM, 0))) > 0

	insert into @t_journal
	select 4, 'AP'
		 , isnull((
			select cls.PayableAccNo
			  from omTrPurchaseHPP pur, gnMstSupplierProfitCenter sup, gnMstSupplierClass cls
			 where 1 = 1
			   and sup.CompanyCode   = pur.CompanyCode
			   and sup.BranchCode    = pur.BranchCode
			   and sup.SupplierCode  = pur.SupplierCode
			   and sup.ProfitCenterCode = '100'
			   and cls.CompanyCode   = pur.CompanyCode
			   and cls.BranchCode    = pur.BranchCode
			   and cls.SupplierClass = sup.SupplierClass
			   and sup.CompanyCode   = @CompanyCode
			   and sup.BranchCode    = @BranchCode
			   and pur.HPPNo         = @DocNo
			), '')
		 , '' AccountDesc
		 , 0
		 , sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscDPP, 0) + isnull(a.OthersDPP, 0) + isnull(a.AfterDiscPPnBM, 0)))
		 + sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscPPn, 0) + isnull(a.OthersPPn, 0))) as TotalTransAmt
	  from omTrPurchaseHPPDetailModel a, omTrPurchaseHPP b
	 where 1 = 1
	   and b.CompanyCode = a.CompanyCode 
	   and b.BranchCode  = a.BranchCode
	   and b.HPPNo       = a.HPPNo
	   and a.CompanyCode = @CompanyCode 
	   and a.BranchCode  = @BranchCode
	   and a.HPPNo       = @DocNo 
	 group by a.CompanyCode, a.BranchCode, a.HPPNo, b.SupplierCode
	having sum(isnull(a.Quantity, 0) * (isnull(a.AfterDiscDPP, 0) + isnull(a.OthersDPP, 0) + isnull(a.AfterDiscPPnBM, 0))) > 0	

	drop table #t1
	drop table #Inventory
	drop table #OthersInv
		
	END
--#endregion

--#region TypeJournal = 'INVOICE'
IF @TypeJournal = 'INVOICE'
BEGIN
	insert into @t_journal
		select 1, 'AR'
			 , isnull((
				select cls.ReceivableAccNo
				  from omTrSalesInvoice ivu, GnMstCustomerProfitCenter cus, GnMstCustomerClass cls
				 where 1 = 1
				   and cus.CompanyCode   = ivu.CompanyCode
				   and cus.BranchCode    = ivu.BranchCode
				   and cus.CustomerCode  = ivu.CustomerCode
				   and cus.ProfitCenterCode = '100'
				   and cls.CompanyCode   = ivu.CompanyCode
				   and cls.BranchCode    = ivu.BranchCode
				   and cls.CustomerClass = cus.CustomerClass
				   and cus.CompanyCode   = @CompanyCode
				   and cus.BranchCode    = @BranchCode
				   and ivu.InvoiceNo     = @DocNo
				), '') AccounNo
			 , '' AccountDesc
			 , isnull((
				select sum(isnull(Quantity, 0) * (AfterDiscDPP + AfterDiscPPn + AfterDiscPPnBm))
				  from omTrSalesInvoiceModel
				 where 1 = 1
				   and CompanyCode = @CompanyCode 
				   and BranchCode  = @BranchCode
				   and InvoiceNo   = @DocNo
				), 0)
			 + isnull((
				select sum(mdl.Quantity * (oth.AfterDiscDPP + oth.AfterDiscPPn))
				  from omTrSalesInvoiceOthers oth left join omTrSalesInvoiceModel mdl
					on oth.BranchCode = mdl.BranchCode
					and oth.InvoiceNo = mdl.InvoiceNo
					and oth.BPKNo = mdl.BPKNo
					and oth.SalesModelCode = mdl.SalesModelCode
				 where 1 = 1
				   and oth.CompanyCode = @CompanyCode 
				   and oth.BranchCode  = @BranchCode
				   and oth.InvoiceNo   = @DocNo
				), 0)
			 + isnull((
				select sum(DPP + PPN)
				  from omTrSalesInvoiceAccs
				 where 1 = 1
				   and CompanyCode = @CompanyCode 
				   and BranchCode  = @BranchCode
				   and InvoiceNo   = @DocNo
				), 0)
			 + (select isnull(sum(isnull(Quantity,0)*isnull(Total,0)),0) from omTrSalesInvoiceAccsSeq where CompanyCode=@CompanyCode
				   and BranchCode=@BranchCode and InvoiceNo=@DocNo) 
			 , 0

insert into @t_journal
select 2, 'DISCOUNT UNIT'
	 , isnull((
		select acc.DiscountAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '') AccountNo
	 , '' AccountDesc
	 , sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) as Discount
	 , 0
  from omTrSalesInvoiceModel a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) > 0

insert into @t_journal
select 3, 'DISCOUNT AKSESORIS'
	 , isnull((
		select acc.DiscountAccNoAks
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '') AccountNo
	 , '' AccountDesc
	 , sum(isnull(a.DiscExcludePPn, 0)) as Discount
	 , 0
  from omTrSalesInvoiceOthers a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull(a.DiscExcludePPn, 0)) > 0

insert into @t_journal
select distinct 4, 'DISCOUNT SPAREPART['+a.TypeOfGoods+']'
	, (select top 1 DiscAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, '' AccountDesc
	, (select sum(isnull(Quantity,0)*isnull(DiscExcludePPn,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode 
		and invoiceno=a.invoiceno and typeofgoods=a.typeofgoods group by typeofgoods) AmountDb
	, 0 AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
where a.companyCode=@CompanyCode 
	and a.BranchCode=@BranchCode 
	and a.InvoiceNo=@DocNo

insert into @t_journal
select 5, 'SALES UNIT'
	 , isnull((
		select acc.SalesAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '') AccountNo
	 , '' AccountDesc
	 , 0
	 , sum(isnull(a.Quantity, 0) * isnull (a.AfterDiscDPP, 0))
	 + sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) 
  from omTrSalesInvoiceModel a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having (sum(isnull(a.Quantity, 0) * isnull (a.AfterDiscDPP, 0)) +
	    sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0))) > 0

insert into @t_journal
select 6, 'SALES AKSESORIS'
	 , isnull((
		select acc.SalesAccNoAks
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '') AccountNo
	 , '' AccountDesc
	 , 0
	 , sum(isnull(b.Quantity, 0) * isnull (a.AfterDiscDPP, 0))
	 + sum(isnull(b.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) 
  from omTrSalesInvoiceOthers a, omTrSalesInvoiceModel b
 where 1 = 1
   and b.BranchCode = a.BranchCode 
   and b.InvoiceNo = a.InvoiceNo 
   and b.BPKNo = a.BPKNo 
   and b.SalesModelCode = a.SalesModelCode 
   and b.SalesModelYear = a.SalesModelYear 
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having (sum(isnull(b.Quantity, 0) * isnull (a.AfterDiscDPP, 0)) +
	    sum(isnull(b.Quantity, 0) * isnull (a.DiscExcludePPn, 0))) > 0

insert into @t_journal
select distinct 7, 'SALES SPAREPART ['+a.typeOfGoods+']'
	, (select top 1 SalesAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, '' AccountDesc
	, 0 AmountDb
	, (select sum(isnull(Quantity,0) * isnull(RetailPrice,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode 
		and invoiceno=a.invoiceno and typeofgoods=a.typeofgoods group by typeofgoods) AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.TypeOfGoods

insert into @t_journal
select 8, 'PPN'
	 , isnull((
		select cls.TaxOutAccNo
		  from omTrSalesInvoice ivu, GnMstCustomerProfitCenter cus, GnMstCustomerClass cls
		 where 1 = 1
		   and cus.CompanyCode   = ivu.CompanyCode
		   and cus.BranchCode    = ivu.BranchCode
		   and cus.CustomerCode  = ivu.CustomerCode
		   and cus.ProfitCenterCode = '100'
		   and cls.CompanyCode   = ivu.CompanyCode
		   and cls.BranchCode    = ivu.BranchCode
		   and cls.CustomerClass = cus.CustomerClass
		   and cus.CompanyCode   = @CompanyCode
		   and cus.BranchCode    = @BranchCode
		   and ivu.InvoiceNo     = @DocNo
		), '') AccountNo
	 , '' AccountDesc
	 , 0
	 , isnull((
		select sum(Quantity * AfterDiscPPn)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(mdl.Quantity * oth.AfterDiscPPn)
		  from omTrSalesInvoiceOthers oth left join omTrSalesInvoiceModel mdl
			on oth.BranchCode = mdl.BranchCode
			and oth.InvoiceNo = mdl.InvoiceNo
			and oth.BPKNo = mdl.BPKNo
			and oth.SalesModelCode = mdl.SalesModelCode
		 where 1 = 1
		   and oth.CompanyCode = @CompanyCode 
		   and oth.BranchCode  = @BranchCode
		   and oth.InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(PPN)
		  from omTrSalesInvoiceAccs
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + (select isnull(sum(isnull(Quantity,0)*isnull(PPN,0)),0) from omTrSalesInvoiceAccsSeq where companyCode = @CompanyCode 
		   and BranchCode=@BranchCode and InvoiceNo=@DocNo)
where (isnull((
		select sum(Quantity * AfterDiscPPn)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(AfterDiscPPn)
		  from omTrSalesInvoiceOthers
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(PPN)
		  from omTrSalesInvoiceAccs
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)) 
	 +(select isnull(sum(isnull(quantity,0)*isnull(PPN,0)),0) from omTrSalesInvoiceAccsSeq where companyCode = @CompanyCode 
		   and BranchCode=@BranchCode and InvoiceNo=@DocNo) > 0

insert into @t_journal
select 9, 'PPN BM'
	 , isnull((
		select cls.LuxuryTaxAccNo
		  from omTrSalesInvoice ivu, GnMstCustomerProfitCenter cus, GnMstCustomerClass cls
		 where 1 = 1
		   and cus.CompanyCode   = ivu.CompanyCode
		   and cus.BranchCode    = ivu.BranchCode
		   and cus.CustomerCode  = ivu.CustomerCode
		   and cus.ProfitCenterCode = '100'
		   and cls.CompanyCode   = ivu.CompanyCode
		   and cls.BranchCode    = ivu.BranchCode
		   and cls.CustomerClass = cus.CustomerClass
		   and cus.CompanyCode   = @CompanyCode
		   and cus.BranchCode    = @BranchCode
		   and ivu.InvoiceNo     = @DocNo
		), '') AccountNo
	 , '' AccountDesc
	 , 0
	 , isnull((
		select sum(Quantity * AfterDiscPPnBm)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
where isnull((
		select sum(Quantity * AfterDiscPPnBm)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0) > 0

insert into @t_journal
select 10, 'HPP Unit'
	 , isnull((
		select acc.COGSAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '') AccountNo
	 , '' AccountDesc
	 , sum(isnull (a.COGS, 0)) as COGS
	 , 0
  from OmTrSalesInvoiceVin a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull (a.COGS, 0)) > 0

insert into @t_journal
select 11, 'INVENTORY UNIT'
	 , isnull((
		select acc.InventoryAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '') AccountNo
	 , '' AccountDesc
	 , 0
	 , sum(isnull (a.COGS, 0)) as COGS
  from OmTrSalesInvoiceVin a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull (a.COGS, 0)) > 0

insert into @t_journal
select distinct 12, 'COGS SPAREPART ['+a.TypeOfGoods+']'
	, (select top 1 COGSAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, '' AccountDesc
	, (select sum(isnull(Quantity,0)*isnull(COGS,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode 
			and invoiceno=a.invoiceno and typeofgoods=a.typeofgoods group by typeofgoods) AmountDb
	, 0 AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
where a.companyCode=@CompanyCode 
	and a.BranchCode=@BranchCode 
	and a.InvoiceNo=@DocNo

insert into @t_journal
select distinct 13, 'INVENTORY AKSESORIES ['+a.TypeOfGoods+']'
	, (select top 1 InventoryAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, '' AccountDesc
	, 0 AmountDb
	, (select sum(isnull(Quantity,0)*isnull(COGS,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode and invoiceno=a.invoiceno
		and typeofgoods=a.typeofgoods group by typeofgoods) AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
WHERE a.companyCode=@CompanyCode 
	and a.BranchCode=@BranchCode 
	and a.InvoiceNo=@DocNo

END
--#endregion

select sum(a.AmountDb) as AmountDb, sum(a.AmountCr) as AmountCr
  from @t_journal a
GO


if object_id('uspfn_OmInquiryChassisReq') is not null
	drop procedure uspfn_OmInquiryChassisReq
GO
-- uspfn_OmInquiryChassisReq '6007402','600740200'
CREATE procedure [dbo].[uspfn_OmInquiryChassisReq]
	@CompanyCode as varchar(15)
	,@BranchCode as varchar(15)
	,@Penjual as varchar(15)
	,@CBU as bit
as

declare @isDirect as bit
set @isDirect=0
if exists (
	select 1
	from gnMstCoProfile
	where CompanyCode=@CompanyCode and BranchCode=@Penjual
)
set @isDirect=1

--DECLARE @CompanyCode as varchar(15)
--	,@Penjual as varchar(15)
--
--set @CompanyCode='6117201'
--set @Penjual='6117201'

select * into #t1
from (
	select distinct isnull(b.BranchCode, e.BranchCode) BranchCode, isnull(c.CustomerCode, e.CustomerCode) CustomerCode
			,z.ChassisCode,z.BPKNo,z.SONo,e.DONo,convert(varchar,z.chassisNo) chassisNo, z.salesModelCode
			, z.salesModelYear, isnull(z.SuzukiDONo,'') RefferenceDONo,
			isnull(z.SuzukiDODate,'19000101') RefferenceDODate, isnull(z.SuzukiSJNo,'') RefferenceSJNo, 
			isnull(z.SuzukiSJDate,'19000101') RefferenceSJDate, 
			a.EndUserName, a.EndUserAddress1, a.EndUserAddress2, a.EndUserAddress3,
			c.CustomerName, c.Address1, c.Address2, c.Address3,
			c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = 'CITY' AND ParaValue = c.CityCode) as CityName, 
			c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT EmployeeName FROM gnMstEmployee where EmployeeID = b.Salesman) SalesmanName, b.SalesType
	from omMstVehicle z 
		left join omTrSalesSOVin a 
			on a.CompanyCode = z.CompanyCode and z.SONo=a.SONo
				AND a.ChassisCode = z.ChassisCode
				AND a.ChassisNo = z.ChassisNo
		left join omTrSalesSO b
			on a.companyCode = b.companyCode 
				and a.BranchCode= b.BranchCode
				and a.SONo = b.SONo
				and b.Status = '2' 
		left join gnMstCustomer c 
			on b.CompanyCode = c.CompanyCode
				and b.CustomerCode = c.CustomerCode 
		left join OmTrSalesDODetail d 
			on d.CompanyCode = z.CompanyCode and z.DONo=d.DONo
				and d.ChassisCode = z.ChassisCode		
				and d.ChassisNo = z.ChassisNo	
		left join OmTrSalesDO e 
			on e.CompanyCode = d.CompanyCode
				and e.DONo = d.DONo
				and e.BranchCode=d.BranchCode
				and e.Status = '2'
		inner join omMstModel f
			on f.CompanyCode = z.CompanyCode
				and f.SalesModelCode = z.SalesModelCode
	where 
		z.CompanyCode = @CompanyCode
		and z.ReqOutNo = ''
		and z.status in ('3','4','5','6')
		and not exists (select 1 from omTrSalesReqdetail where ChassisCode=z.ChassisCode and ChassisNo=z.ChassisNo)
		and ((case when z.ChassisNo is not null then z.SONo end) is not null 
			or (case when z.ChassisNo is not null then z.DONo end) is not null)
		and f.IsCBU = @CBU
) #t1

select * from #t1 
where ((case when @isDirect = 1 then BranchCode end)= @Penjual
		or (case when @isDirect <> 1 then CustomerCode end)= @Penjual)
drop table #t1
GO

if object_id('uspfn_omSlsDoLkpShipto') is not null
	drop procedure uspfn_omSlsDoLkpShipto
GO
CREATE procedure [dbo].[uspfn_omSlsDoLkpShipto]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @ProfitCenterCode varchar(3)
) 
as begin
--exec uspfn_omSlsDoLkpShipto 6006410,600641001,300
SELECT a.CompanyCode
	,a.CustomerCode
	,a.StandardCode
	,a.CustomerName
	,a.CustomerAbbrName
	,a.CustomerGovName
	,a.CustomerType
	,a.CategoryCode
	,a.Address1
	,a.Address2
	,a.Address3
	,a.Address4
	,a.PhoneNo
	,a.HPNo
	,a.FaxNo
	,a.isPKP
	,a.NPWPNo
	,a.NPWPDate
	,a.SKPNo
	,a.SKPDate
	,a.ProvinceCode
	,a.AreaCode
	,a.CityCode
	,a.ZipNo
	,a.Status
	,a.CreatedBy
	,a.CreatedDate
	,a.LastUpdateBy
	,a.LastUpdateDate
	,a.isLocked
	,a.LockingBy
	,a.LockingDate
	,a.Email
	,a.BirthDate
	,a.Spare01
	,a.Spare02
	,a.Spare03
	,a.Spare04
	,a.Spare05
	,a.Gender
	,a.OfficePhoneNo
	,a.KelurahanDesa
	,a.KecamatanDistrik
	,a.KotaKabupaten
	,a.IbuKota
	,a.CustomerStatus
	  FROM gnMstCustomer a 
	INNER JOIN gnMstCustomerProfitCenter b
	  ON a.CompanyCode = b.CompanyCode AND 
		 a.CustomerCode = b.CustomerCode AND
		 b.BranchCode = @BranchCode
	WHERE a.CompanyCode = @CompanyCode AND 
		  b.ProfitCenterCode = @ProfitCenterCode                      
end
GO

if object_id('uspfn_QueryCustomerDealer') is not null
	drop procedure uspfn_QueryCustomerDealer
GO
-- usprpt_QueryCustomerDealer '0','2012-1-1','2012-12-12','Cabang','6006406','6006401'
CREATE procedure [dbo].[uspfn_QueryCustomerDealer]
	@CheckDate	bit,
	@StartDate	Datetime,
	@EndDate	Datetime,
	@Area		varchar(15),
	@Dealer		varchar(15),
	@Outlet		varchar(15)
as 
if @Outlet = ''
begin
	select a.CompanyCode
		, b.DealerAbbreviation
		, a.CustomerCode
		, a.SuzukiCode
		, a.Suzuki2Code
		, a.CustomerName
		, a.CustomerGovName
		, a.Address1 + a.Address2 + a.Address3 + a.Address4 Address
		, a.ProvinceCode
		, a.ProvinceName
		, a.CityCode
		, a.CityName
		, a.ZipNo
		, a.KelurahanDesa
		, a.KecamatanDistrik
		, a.KotaKabupaten
		, a.IbuKota
		, a.PhoneNo
		, a.HPNo
		, a.FaxNo
		, a.OfficePhoneNo
		, a.Email
		, a.Gender
		, a.BirthDate
		, a.isPKP
		, a.NPWPNo
		, a.NPWPDate
		, a.SKPNo
		, a.SKPDate
		, a.CustomerType
		, a.CustomerTypeDesc
		, a.CategoryCode
		, a.CategoryDesc
		, a.Status
		, a.CustomerStatus
	from gnMstCustomerDealer a
	left join GnMstDealerMapping b on a.CompanyCode = b.DealerCode
	where  case when @CheckDate = 1 
				then convert(varchar,a.CreatedDate,112) 
				else convert(varchar,@StartDate,112) 
				end between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112) 
	  and isnull(b.Area,'')			like case when isnull(@Area,'') <> ''	then @Area		else '%%' end
	  and isnull(b.DealerCode,'')	like case when isnull(@Dealer,'') <> '' then @Dealer	else '%%' end
end
else
begin
	select a.CompanyCode
		, c.DealerAbbreviation
		, a.CustomerCode
		, a.SuzukiCode
		, a.Suzuki2Code
		, a.CustomerName
		, a.CustomerGovName
		, a.Address1 + a.Address2 + a.Address3 + a.Address4 Address
		, a.ProvinceCode
		, a.ProvinceName
		, a.CityCode
		, a.CityName
		, a.ZipNo
		, a.KelurahanDesa
		, a.KecamatanDistrik
		, a.KotaKabupaten
		, a.IbuKota
		, a.PhoneNo
		, a.HPNo
		, a.FaxNo
		, a.OfficePhoneNo
		, a.Email
		, a.Gender
		, a.BirthDate
		, a.isPKP
		, a.NPWPNo
		, a.NPWPDate
		, a.SKPNo
		, a.SKPDate
		, a.CustomerType
		, a.CustomerTypeDesc
		, a.CategoryCode
		, a.CategoryDesc
		, a.Status
		, a.CustomerStatus
	from gnMstCustomerDealer a
	left join GnMstDealerMapping c on a.CompanyCode = c.DealerCode
	where  case when @CheckDate = 1 
				then convert(varchar,a.CreatedDate,112) 
				else convert(varchar,@StartDate,112) 
				end between convert(varchar,@StartDate,112) and convert(varchar,@EndDate,112) 
	  and isnull(c.Area,'')			like case when isnull(@Area,'') <> ''	then @Area		else '%%' end
	  and isnull(c.DealerCode,'')	like case when isnull(@Dealer,'') <> '' then @Dealer	else '%%' end
	  and a.CustomerCode in (select distinct k.CustomerCode 
									from gnMstCustomerDealerProfitCenter k
									left join GnMstDealerMapping l on k.CompanyCode = l.DealerCode
									where isnull(l.Area,'')			 like case when isnull(@Area,'') <> ''	then @Area		else '%%' end
										and isnull(k.CompanyCode,'') like case when isnull(@Dealer,'') <> '' then @Dealer	else '%%' end
										and isnull(k.BranchCode,'')	 like case when isnull(@Outlet,'') <> '' then @Outlet	else '%%' end)
end

--select * from OmHstInquirySales
--select * from GnMstCustomerDealer
GO

if object_id('uspfn_spCustSOPickListNewOrder') is not null
	drop procedure uspfn_spCustSOPickListNewOrder
GO
CREATE procedure [dbo].[uspfn_spCustSOPickListNewOrder]   
--DECLARE
@CompanyCode varchar(15),  
@BranchCode varchar(15),  
@ProfitCenterCode varchar(3),  
@TypeOfGoods varchar(2)  
as  
--SET @CompanyCode = '6006410'
--SET @BranchCode = '600641001'
----SET @ProfitCenterCode = '000' --
--SET @TypeOfGoods = '0'
SELECT x.CustomerCode,  
                    (  
                     SELECT c.CustomerName   
                     FROM gnMstCustomer c with(nolock, nowait)  
                     where  c.CompanyCode=x.CompanyCode  
                     AND c.CustomerCode= x.CustomerCode   
                     AND c.CustomerCode=x.CustomerCode  
                    ) AS CustomerName,  
                    (  
                     SELECT c.Address1+' '+c.Address2+' '+c.Address3+' '+c.Address4   
                     FROM gnMstCustomer c with(nolock, nowait)  
                     where  c.CompanyCode=x.CompanyCode  
                     AND c.CustomerCode= x.CustomerCode   
                     AND c.CustomerCode=x.CustomerCode  
  
                    ) as Address  
                    , z.LookUpValueName as ProfitCenter  
                    FROM   
                    (  
                     SELECT DISTINCT a.CompanyCode, a.BranchCode,  
                     b.CustomerCode   
                     FROM spTrnSOSupply a WITH(nolock, nowait) INNER JOIN   
                        spTrnSOrdHdr b ON a.CompanyCode=b.CompanyCode  
                     and a.BranchCode=b.BranchCode  
                     and a.DocNo=b.DocNo  
                        and b.TypeOfGoods=@TypeOfGoods  
                     WHERE pickingslipno = ''  
                    ) x   
                    INNER JOIN gnMstCustomerProfitCenter y WITH(nolock, nowait)  
                    ON y.CompanyCode = x.CompanyCode   
                       AND y.BranchCode = x.BranchCode  
                       AND y.CustomerCode = x.CustomerCode  
                    INNER JOIN gnMstLookUpDtl z WITH(nolock, nowait)  
                    ON z.CompanyCode= x.CompanyCode  
                       AND z.CodeID='PFCN'  
                       AND z.LookupValue=y.ProfitCenterCode  
                    WHERE x.CompanyCode=@CompanyCode  
                        AND x.BranchCode=@BranchCode  
                       AND y.ProfitCenterCode=@ProfitCenterCode
GO




if object_id('uspfn_spGetLookupLMP4Srv') is not null
	drop procedure uspfn_spGetLookupLMP4Srv
GO
CREATE procedure [dbo].[uspfn_spGetLookupLMP4Srv]     
@CompanyCode varchar(15), @BranchCode varchar(15),    
@SalesType varchar(5), @TypeOfGoods varchar(15), @ProductType varchar(15)    
as    
SELECT * FROM     
(    
SELECT    
 PickingSlipNo,     
 PickingSlipDate,    
 BPSFNo,     
 BPSFDate,    
 (    
   SELECT TOP 1 PRODUCTTYPE FROM spTrnSBPSFDtl    
  WHERE spTrnSBPSFHdr.CompanyCode = spTrnSBPSFDtl.CompanyCode    
  AND spTrnSBPSFHdr.BranchCode = spTrnSBPSFDtl.BranchCode    
  AND spTrnSBPSFHdr.BPSFNo = spTrnSBPSFDtl.BPSFNo    
 ) AS ProductType,  
 spTrnSBPSFHdr.CustomerCode,
 spTrnSBPSFHdr.CustomerCodeShip,
 spTrnSBPSFHdr.CustomerCodeBill as CustomerCodeTagih, 
 b.CustomerName,  
 b.Address1,  
 b.Address2,  
 b.Address3,  
 b.Address4,  
 b.CustomerName CustomerNameTagih,  
 b.Address1 Address1Tagih,  
 b.Address2 Address2Tagih,  
 b.Address3 Address3Tagih,  
 b.Address4 Address4Tagih,  
 c.LookUpValueName TransType    
FROM spTrnSBPSFHdr    
join gnMstCustomer b  
ON spTrnSBPSFHdr.CompanyCode = b.CompanyCode  
AND spTrnSBPSFHdr.CustomerCode = b.CustomerCode  
join gnMstLookupDtl c on  
spTrnSBPSFHdr.CompanyCode = c.CompanyCode  
and spTrnSBPSFHdr.TransType= c.LookupValue   
AND c.CodeID = 'TTSR'  
WHERE spTrnSBPSFHdr.CompanyCode = @CompanyCode    
AND spTrnSBPSFHdr.BranchCode    = @BranchCode    
AND spTrnSBPSFHdr.SalesType     = @SalesType    
AND spTrnSBPSFHdr.TypeOfGoods   = @TypeOfGoods    
AND (spTrnSBPSFHdr.Status = '1' OR spTrnSBPSFHdr.Status = '0')    
AND (spTrnSBPSFHdr.PickingSlipNo NOT IN (SELECT PickingSlipNo FROM spTrnSLmpHdr where CompanyCode = @CompanyCode AND BranchCode = @BranchCode))    
) A    
WHERE A.ProductType = @ProductType
GO

if object_id('uspfn_SpOrderSparepartView') is not null
	drop procedure uspfn_SpOrderSparepartView
GO
CREATE procedure [dbo].[uspfn_SpOrderSparepartView] @CompanyCode varchar(10) ,@BranchCode varchar(10), @TypeOfGoods varchar(2), @ProductType varchar(2)
as  
SELECT    
ItemInfo.PartNo,  
Items.ABCClass,  
ItemLoc.OnHand - itemLoc.ReservedSP - itemLoc.ReservedSR - itemLoc.ReservedSL - itemLoc.AllocationSP - itemLoc.AllocationSL - itemLoc.AllocationSR AS AvailQty,  
Items.OnOrder,  
Items.ReservedSP,  
Items.ReservedSR,  
Items.ReservedSL,  
Items.MovingCode,  
ItemInfo.SupplierCode,  
ItemInfo.PartName,  
ItemInfo.DiscPct,
ItemPrice.RetailPrice,  
ItemPrice.RetailPriceInclTax,  
ItemPrice.PurchasePrice  
FROM SpMstItems Items  
INNER JOIN SpMstItemInfo ItemInfo ON Items.CompanyCode  = ItemInfo.CompanyCode                            
                         AND Items.PartNo = ItemInfo.PartNo  
INNER JOIN spMstItemPrice ItemPrice ON Items.CompanyCode = ItemPrice.CompanyCode  
                        AND Items.BranchCode = ItemPrice.BranchCode AND Items.PartNo = ItemPrice.PartNo  
INNER JOIN spMstItemLoc ItemLoc ON Items.CompanyCode = ItemLoc.CompanyCode AND Items.BranchCode = ItemLoc.BranchCode  
                        AND Items.PartNo = ItemLoc.PartNo  
WHERE Items.CompanyCode = @CompanyCode  
  AND Items.BranchCode  = @BranchCode 
  AND Items.TypeOfGoods = @TypeOfGoods
  AND Items.ProductType = @ProductType  
  AND Items.Status > 0  
  AND ItemLoc.WarehouseCode = '00'
GO


if object_id('uspfn_spPickingHdrSales') is not null
	drop procedure uspfn_spPickingHdrSales
GO
CREATE procedure [dbo].[uspfn_spPickingHdrSales] @CompanyCode varchar(15), @BranchCode varchar(15), @PickingSlipNo varchar(25), @CodeID varchar(6)  
as      
SELECT DISTINCT  
                            spTrnSORDHdr.DocNo,        
                            spTrnSORDHdr.DocDate,                                                    
                            PaymentName =   
                            (select LookUpValueName from gnMstLookupDtl   
                            where LookupValue = spTrnSORDHdr.PaymentCode and CodeID = @CodeID),  
                            spTrnSORDHdr.OrderNo,                              
                            spTrnSORDHdr.OrderDate,  
                            CONVERT(bit, 1) ChkSelect,
                            spTrnSPickingHdr.Remark                            
                            FROM spTrnSPickingHdr   
                            LEFT JOIN spTrnSPickingDtl ON spTrnSPickingHdr.PickingSlipNo = spTrnSPickingDtl.PickingSlipNo AND  
                                spTrnSPickingHdr.CompanyCode = spTrnSPickingDtl.CompanyCode AND  
                                spTrnSPickingHdr.BranchCode = spTrnSPickingDtl.BranchCode      
                            INNER JOIN spTrnSORDHdr ON spTrnSORDHdr.DocNo = spTrnSPickingDtl.DocNo AND  
                                spTrnSORDHdr.CompanyCode = spTrnSPickingDtl.CompanyCode AND  
                                spTrnSORDHdr.BranchCode = spTrnSPickingDtl.BranchCode  
                            WHERE spTrnSPickingHdr.PickingSlipNo = @PickingSlipNo AND  
                            spTrnSPickingHdr.CompanyCode = @CompanyCode AND  
                            spTrnSPickingHdr.BranchCode = @BranchCode
GO


if object_id('uspfn_SpposView') is not null
	drop procedure uspfn_SpposView
GO
CREATE procedure [dbo].[uspfn_SpposView] (  @CompanyCode varchar(15) ,@BranchCode varchar(15), @TypeOfGoods	 varchar(15))
   as
            SELECT 
                DISTINCT a.POSNo, a.SupplierCode, b.SupplierName 
            FROM 
                spTrnPOrderBalance a 
            INNER JOIN gnMstSupplier b 
               ON b.SupplierCode = a.SupplierCode 
              AND b.CompanyCode  = a.CompanyCode 
            WHERE a.OrderQty > a.Received
              AND a.CompanyCode = @CompanyCode
              AND a.BranchCode  = @BranchCode
			  AND a.TypeOfGoods = @TypeOfGoods	
            ORDER BY POSNo DESC
GO


if object_id('uspfn_spSelectPickingSlip') is not null
	drop procedure uspfn_spSelectPickingSlip
GO
CREATE procedure [dbo].[uspfn_spSelectPickingSlip] @CompanyCode varchar(15), @BranchCode varchar(15), 
@ProductType varchar(4), @JobOrderNo varchar(25)
as
SELECT * INTO #t1 FROM (
                SELECT
                    DISTINCT c.DocNo, c.DocDate, d.PickingSlipNo, e.PartNo, e.PartNo PartNoOri, e.QtySupply, 
                    e.QtyPicked, e.QtyBill, d.Status, f.LookUpValueName TransTypeDesc, c.TransType, g.LmpNo,
                    d.PickedBy
                FROM
                    svTrnService a
                LEFT JOIN svTrnSrvItem b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode 
	                AND b.ProductType = a.ProductType AND b.ServiceNo=a.ServiceNo
                LEFT JOIN spTrnSOrdHdr c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode 
	                AND c.DocNo = b.SupplySlipNo
                LEFT JOIN spTrnSPickingHdr d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode 
	                AND d.PickingSlipNo = c.ExPickingSlipNo
                LEFT JOIN spTrnSPickingDtl e ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode 
	                AND e.PickingSlipNo = d.PickingSlipNo
                LEFT JOIN gnMstLookUpDtl f ON f.CompanyCode = a.CompanyCode AND f.CodeId = 'TTSR' 
                    AND f.LookUpValue = c.TransType
                LEFT JOIN spTrnSLmpHdr g ON g.CompanyCode = a.CompanyCode AND g.BranchCode = a.BranchCode 
                    AND g.PickingSlipNo = d.PickingSlipNo
                WHERE 
                    a.CompanyCode     = @CompanyCode
                    AND a.BranchCode  = @BranchCode
                    AND a.ProductType = @ProductType
                    AND a.jobOrderNo  = @JobOrderNo
                    AND b.SupplySlipNo <> ''
                    AND b.PartSeq = (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		                   AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = b.PartNo)
                    AND d.Status <= 2
            )#t1
            SELECT Row_number() OVER(ORDER BY DocNo) No, * FROM #t1
            DROP TABLE #t1
GO


if object_id('uspfn_spSuppliers') is not null
	drop procedure uspfn_spSuppliers
GO
CREATE procedure [dbo].[uspfn_spSuppliers]  (  @CompanyCode varchar(10) ,@BranchCode varchar(10), @ProfitCenterCode char(3)) 
	   as
	            SELECT distinct
                    a.SupplierCode, a.SupplierName, (a.address1+' '+a.address2+' '+a.address3+' '+a.address4) as Alamat,
                    b.DiscPct as Diskon, (Case a.Status when 0 then 'Tidak Aktif' else 'Aktif' end) as [Status],
                    (SELECT Lookupvaluename FROM gnmstlookupdtl WHERE codeid='PFCN' 
                     AND lookupvalue = b.ProfitCentercode) as Profit
                FROM 
                    gnMstSupplier a
                JOIN gnmstSupplierProfitCenter b ON a.CompanyCode= b.CompanyCode
	                AND a.SupplierCode = b.SupplierCode
                WHERE 
                    a.CompanyCode=@CompanyCode
                    AND b.BranchCode=@BranchCode
					AND b. ProfitCenterCode=@ProfitCenterCode
                    AND b.isBlackList=0
                    AND a.status = 1
GO

if object_id('uspfn_spTrnIAdjustDtlview') is not null
	drop procedure uspfn_spTrnIAdjustDtlview
GO
CREATE PROCEDURE [dbo].[uspfn_spTrnIAdjustDtlview]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@AdjustmentNo varchar(25) 
 
AS
SELECT 
        row_number () OVER (ORDER BY spTrnIAdjustDtl.CreatedDate) AS No,
        spTrnIAdjustDtl.PartNo,
        spMstItemInfo.PartName,
        spTrnIAdjustDtl.WarehouseCode,	
        spTrnIAdjustDtl.LocationCode,
		spTrnIAdjustDtl.AdjustmentCode,
        gnMstLookUpDtl_1.LookUpValueName AS AdjustmentDesc,
	    gnMstLookUpDtl_2.LookUpValueName AS WarehouseName,
        spTrnIAdjustDtl.QtyAdjustment,
		spTrnIAdjustDtl.ReasonCode,
        gnMstLookUpDtl.LookUpValueName AS ReasonDesc
    FROM 
        spTrnIAdjustDtl
        INNER JOIN spTrnIAdjustHdr ON spTrnIAdjustHdr.AdjustmentNo = spTrnIAdjustDtl.AdjustmentNo 
            AND spTrnIAdjustHdr.CompanyCode =  spTrnIAdjustDtl.CompanyCode
            AND spTrnIAdjustHdr.BranchCode =  spTrnIAdjustDtl.BranchCode
        INNER JOIN spMstItemInfo ON spMstItemInfo.PartNo = spTrnIAdjustDtl.PartNo
            AND spMstItemInfo.CompanyCode = spTrnIAdjustDtl.CompanyCode
        INNER JOIN gnMstLookUpDtl ON gnMstLookUpDtl.LookUpValue = spTrnIAdjustDtl.ReasonCode
            AND gnMstLookUpDtl.CompanyCode = spTrnIAdjustDtl.CompanyCode
        INNER JOIN gnMstLookUpDtl AS gnMstLookUpDtl_1 ON gnMstLookUpDtl_1.LookUpValue = spTrnIAdjustDtl.AdjustmentCode
            AND gnMstLookUpDtl_1.CompanyCode = spTrnIAdjustDtl.CompanyCode
	    INNER JOIN gnMstLookUpDtl AS gnMstLookUpDtl_2 ON gnMstLookUpDtl_2.LookUpValue = spTrnIAdjustDtl.WarehouseCode
            AND gnMstLookUpDtl_2.CompanyCode = spTrnIAdjustDtl.CompanyCode
    WHERE 
        spTrnIAdjustDtl.CompanyCode = @CompanyCode
        AND spTrnIAdjustDtl.BranchCode = @BranchCode
        AND gnMstLookUpDtl.CodeId='RSAD'
	    AND gnMstLookUpDtl_2.CodeId='WRCD'
        AND gnMstLookUpDtl_1.CodeID = 'ADJS'
        AND spTrnIAdjustDtl.AdjustmentNo =  @AdjustmentNo
    ORDER BY spTrnIAdjustDtl.CreatedDate
GO


if object_id('uspfn_spTrnPPOSHdr') is not null
	drop procedure uspfn_spTrnPPOSHdr
GO
CREATE procedure [dbo].[uspfn_spTrnPPOSHdr] 
@CompanyCode varchar(10),@BranchCode varchar(10),
@TypeOfGoods varchar(10),
 @Status  int
as 
SELECT a.POSNo, a.PosDate , a.Status ,a.SupplierCode ,b.SupplierName, a.OrderType
                FROM spTrnPPOSHdr a
                INNER JOIN gnMstSupplier b ON b.SupplierCode = a.SupplierCode and b.CompanyCode = a.CompanyCode
    WHERE a.CompanyCode=@CompanyCode 
                AND a.BranchCode=@BranchCode
                AND a.TypeOfGoods=@TypeOfGoods
                AND a.Status <=@Status           ORDER BY a.POSNo DESC
GO


if object_id('uspfn_spTrnPSUGGORHdr') is not null
	drop procedure uspfn_spTrnPSUGGORHdr
GO
CREATE procedure [dbo].[uspfn_spTrnPSUGGORHdr] (  @CompanyCode varchar(15) ,  @BranchCode varchar(15) )
 as

SELECT 
 a.SuggorNo
--,a.SuggorDate
,CONVERT(varchar(15), a.SuggorDate, 103) as SuggorDate
,a.SupplierCode
,b.SupplierName
FROM spTrnPSUGGORHdr a
LEFT JOIN gnMstSupplier b on b.CompanyCode=a.CompanyCode AND b.SupplierCode=a.SupplierCode                               
WHERE a.CompanyCode=@CompanyCode AND a.BranchCode=@BranchCode

AND a.status < 2
ORDER BY a.SuggorNo DESC
GO

if object_id('uspfn_SvGetCustVehicle') is not null
	drop procedure uspfn_SvGetCustVehicle
GO
CREATE procedure [dbo].[uspfn_SvGetCustVehicle]
   @CompanyCode varchar(15)
as

select a.ChassisCode, cast(a.ChassisNo as varchar) ChassisNo, a.EngineCode, a.SalesModelYear
     , cast(a.EngineNo as varchar) EngineNo, a.ServiceBookNo
     , case a.PoliceRegistrationDate when ('19000101') then NULL else a.PoliceRegistrationDate end as PoliceRegistrationDate
     , a.PoliceRegistrationNo
     , case a.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end as Status
     , b.BasicModel
     , b.TechnicalModelCode
     , b.TransmissionType
     , a.ColourCode
     , c.CustomerCode
     , c.CompanyCode
     , '' ContactName
     , '' ContactAddress
     , '' ContactPhone
  from omMstVehicle a
  left join omMstModel b
    on b.CompanyCode = a.CompanyCode
   and b.SalesModelCode = a.SalesModelCode
  left join omTrSalesInvoice c
    on c.CompanyCode = a.CompanyCode
   and c.InvoiceNo = a.InvoiceNo
 where a.CompanyCode = @CompanyCode
   and a.IsActive = 1
 order by a.ChassisCode, a.ChassisNo
 
 select * from SvChassicView
GO

if object_id('uspfn_SvInqGetClaimLku') is not null
	drop procedure uspfn_SvInqGetClaimLku
GO
CREATE procedure [dbo].[uspfn_SvInqGetClaimLku]
	 @CompanyCode		varchar(20),
	 @BranchCodeFrom    varchar(20),
	 @BranchCodeTo		varchar(20),
	 @IsSprClaim		bit = 0
	 
as

declare @prefix varchar(10)

if exists (select * from gnMstLookUpDtl 
			 where CompanyCode = @CompanyCode 
			   and CodeId = 'SRV_FLAG' 
			   and LookUpValue = 'CLM_MODE'
			   and ParaValue = '1')  -- 0: SPK, 1: INV --> Default: SPK
	set @prefix = 'INW%'
else
	set @prefix = 'SPK%'

if @BranchCodeFrom = '[All Branch]' and @BranchCodeTo = '[All Branch]'
begin
	select a.BranchCode, a.InvoiceNo, b.JobOrderDate as InvoiceDate, a.IsCbu, a.CategoryCode, a.ComplainCode, a.DefectCode  
	  , a.SubletHour, a.SubletAmt, a.CausalPartNo, a.TroubleDescription  
	  , a.ProblemExplanation, a.OperationNo, isnull(a.OperationHour,0) as OperationHour, isnull(a.OperationAmt,0) as OperationAmt    
	  from svTrnInvClaim a  
	  left join svTrnService b  
		on b.CompanyCode = a.CompanyCode  
	   and b.BranchCode = a.BranchCode  
	   and b.JobOrderNo = a.InvoiceNo  
	 where 1 = 1  
	   and a.CompanyCode = @CompanyCode  
--	   and a.BranchCode between @BranchCodeFrom and @BranchCodeTo  
	   and not exists (  
		  select 1 from svTrnClaimApplication  
		   where 1 = 1  
			 and CompanyCode = a.CompanyCode  
			 and BranchCode = a.BranchCode  
			 and ProductType = a.ProductType  
			 and InvoiceNo = a.InvoiceNo  
		  )  
	   and a.InvoiceNo like @prefix  
	   and isnull(b.IsSparepartClaim, 0) = @IsSprClaim
	 order by a.InvoiceNo  
end
else
begin
	select a.BranchCode, a.InvoiceNo, b.JobOrderDate as InvoiceDate, a.IsCbu, a.CategoryCode, a.ComplainCode, a.DefectCode  
	  , a.SubletHour, a.SubletAmt, a.CausalPartNo, a.TroubleDescription  
	  , a.ProblemExplanation, a.OperationNo, isnull(a.OperationHour,0) as OperationHour, isnull(a.OperationAmt,0) as OperationAmt  
	  from svTrnInvClaim a  
	  left join svTrnService b  
		on b.CompanyCode = a.CompanyCode  
	   and b.BranchCode = a.BranchCode  
	   and b.JobOrderNo = a.InvoiceNo  
	 where 1 = 1  
	   and a.CompanyCode = @CompanyCode  
	   and a.BranchCode between @BranchCodeFrom and @BranchCodeTo  
	   and not exists (  
		  select 1 from svTrnClaimApplication  
		   where 1 = 1  
			 and CompanyCode = a.CompanyCode  
			 and BranchCode = a.BranchCode  
			 and ProductType = a.ProductType  
			 and InvoiceNo = a.InvoiceNo  
		  )  
	   and a.InvoiceNo like @prefix  
	   and isnull(b.IsSparepartClaim, 0) = @IsSprClaim
	 order by a.InvoiceNo  
end
GO

if object_id('uspfn_SvTrnInvoiceCreate') is not null
	drop procedure uspfn_SvTrnInvoiceCreate
GO
--declare	@CompanyCode varchar(15)
--declare	@BranchCode  varchar(15)
--declare	@ProductType varchar(15)
--declare	@ServiceNo   int
--declare	@BillType    char(1)
--declare	@InvoiceNo   varchar(15)
--declare	@Remarks     varchar(max)
--declare	@UserID      varchar(15)
--
--set	@CompanyCode = '6026401'
--set	@BranchCode  = '602640100'
--set	@ProductType = '4W'
--set	@ServiceNo   = '266'
--set	@BillType    = 'C'
--set	@InvoiceNo   = 'INF/XX/123456'
--set	@Remarks     = 'REMARK 001'
--set	@UserID      = 'ws-s'

CREATE procedure [dbo].[uspfn_SvTrnInvoiceCreate]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@ServiceNo   int,
	@BillType    char(1),
	@InvoiceNo   varchar(15),
	@Remarks     varchar(max),
	@UserID      varchar(15)
as  

declare @errmsg varchar(max)
--raiserror ('test error',16,1);

-- get data from service
select * into #srv from(
  select * from svTrnService
   where 1 = 1
     and CompanyCode = @CompanyCode
     and BranchCode  = @BranchCode
     and ProductType = @ProductType
     and ServiceNo   = @ServiceNo
 )#srv

-- get data from task
select * into #tsk from(
  select a.* from svTrnSrvTask a, #srv b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.BillType    = @BillType
 )#tsk

-- get data from item
select * into #mec from(
  select a.* from svTrnSrvMechanic a, #tsk b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.OperationNo = b.OperationNo
     and a.OperationNo <> ''
 )#mec

-- get data from item
select * into #itm from(
  select a.* from svTrnSrvItem a, #srv b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.BillType    = @BillType
 )#itm

-- create temporary table detail
create table #pre_dtl(
	BillType char(1),
	TaskPartType char(1),
	TaskPartNo varchar(20),
	TaskPartQty numeric(10,2),
	SupplySlipNo varchar(20)
)

insert into #pre_dtl
select BillType, 'L', OperationNo, OperationHour, ''
  from #tsk

insert into #pre_dtl
select BillType, TypeOfGoods, PartNo
	 , sum(SupplyQty - ReturnQty)
	 , SupplySlipNo
  from #itm
 where BillType = @BillType
   and (SupplyQty - ReturnQty) > 0
 group by BillType, TypeOfGoods, PartNo, SupplySlipNo

-- insert to table svTrnInvoice
declare @CustomerCode varchar(20)
if @BillType = 'C'
begin
	set @CustomerCode = (select CustomerCodeBill from #srv)
end
else if @BillType = 'P'
begin
	set @CustomerCode = (select top 1 a.BillTo from svMstPackage a
				 inner join svMstPackageTask b
					on b.CompanyCode = a.CompanyCode
				   and b.PackageCode = a.PackageCode
				 inner join svMstPackageContract c
					on c.CompanyCode = a.CompanyCode
				   and c.PackageCode = a.PackageCode
				 inner join #srv d
					on d.CompanyCode = a.CompanyCode
				   and d.JobType = a.JobType
				   and d.ChassisCode = c.ChassisCode
				   and d.ChassisNo = c.ChassisNo)
end
else if @BillType in ('F', 'W', 'S')
begin
	set @CustomerCode = (select CustomerCode from svMstBillingType
				 where BillType in ('F', 'W', 'S')
				   and CompanyCode = @CompanyCode
				   and BillType = @BillType)
end
else
begin
	set @CustomerCode = (select CustomerCodeBill from #srv)
end

--set @CustomerCode = isnull((
--				select top 1 a.BillTo from svMstPackage a
--				 inner join svMstPackageTask b
--					on b.CompanyCode = a.CompanyCode
--				   and b.PackageCode = a.PackageCode
--				 inner join svMstPackageContract c
--					on c.CompanyCode = a.CompanyCode
--				   and c.PackageCode = a.PackageCode
--				 inner join #srv d
--					on d.CompanyCode = a.CompanyCode
--				   and d.JobType = a.JobType
--				   and d.ChassisCode = c.ChassisCode
--				   and d.ChassisNo = c.ChassisNo
--				), isnull((
--				select CustomerCode from svMstBillingType
--				 where BillType in ('F')
--				   and CompanyCode = @CompanyCode
--				   and BillType = @BillType
--				), isnull((select CustomerCodeBill from #srv), '')))


if ((select count(*) from #tsk) = 0 and (select count(*) from #itm) = 0)
begin
	drop table #srv
	drop table #tsk
	drop table #mec
	drop table #itm
	drop table #pre_dtl
	return
end

if (@CustomerCode = '')
begin
	set @errmsg = N'Customer Code Bill belum di define...'
				+ char(13) + 'Tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

select * into #cus from (
select a.CompanyCode, a.IsPkp, b.CustomerCode, b.LaborDiscPct, b.PartDiscPct, b.MaterialDiscPct, b.TopCode, b.TaxCode
  from gnMstCustomer a, gnMstCustomerProfitCenter b
 where 1 = 1
   and b.CompanyCode  = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.CompanyCode  = @CompanyCode
   and b.BranchCode   = @BranchCode
   and b.CustomerCode = @CustomerCode
   and b.ProfitCenterCode = '200'
)#cus

if (select count(*) from #cus) <> 1
begin
	set @errmsg = N'Customer ProfitCenter belum di define...'
				+ char(13) + 'Tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

declare @IsPKP bit
    set @IsPKP = isnull((
				 select IsPKP from gnMstCustomer
				  where CompanyCode  = @CompanyCode
				    and CustomerCode = @CustomerCode
				  ), 0)

declare @PPnPct decimal
    set @PPnPct = isnull((
				  select a.TaxPct
				    from gnMstTax a, #cus b
				   where 1 = 1
				     and b.TaxCode     = 'PPN'
				     and a.CompanyCode = b.CompanyCode
				     and a.TaxCode     = b.TaxCode
				  ), 0)

declare @PPhPct decimal
    set @PPhPct = isnull((
				  select a.TaxPct
				    from gnMstTax a, #cus b
				   where 1 = 1
				     and b.TaxCode     = 'PPH'
				     and a.CompanyCode = b.CompanyCode
				     and a.TaxCode     = b.TaxCode
				  ), 0)


-- Insert Into svTrnInvoice
-----------------------------------------------------------------------------------------
insert into svTrnInvoice(
  CompanyCode, BranchCode, ProductType
, InvoiceNo, InvoiceDate, InvoiceStatus
, FPJNo, FPJDate, JobOrderNo, JobOrderDate, JobType
, ServiceRequestDesc, ChassisCode, ChassisNo, EngineCode, EngineNo
, PoliceRegNo, BasicModel, CustomerCode, CustomerCodeBill, Odometer
, IsPKP, TOPCode, TOPDays, DueDate, SignedDate
, LaborDiscPct, PartsDiscPct, MaterialDiscPct, PphPct, PpnPct, Remarks
, PrintSeq, PostingFlag, IsLocked, CreatedBy, CreatedDate
) 
select
  @CompanyCode CompanyCode
, @BranchCode BranchCode
, @ProductType ProductType
, @InvoiceNo InvoiceNo
, getdate() InvoiceDate
, case @IsPKP
	when '0' then '1'
	else (case @BillType when 'F' then '0' when 'W' then '0' else '1' end)
  end as InvoiceStatus
, '' FPJNo
, null FPJDate
, (select JobOrderNo from #srv) JobOrderNo
, (select JobOrderDate from #srv) JobOrderDate
, (select JobType from #srv) JobType
, (select ServiceRequestDesc from #srv) ServiceRequestDesc
, (select ChassisCode from #srv) ChassisCode
, (select ChassisNo from #srv) ChassisNo
, (select EngineCode from #srv) EngineCode
, (select EngineNo from #srv) EngineNo
, (select PoliceRegNo from #srv) PoliceRegNo
, (select BasicModel from #srv) BasicModel
, (select CustomerCode from #srv) CustomerCode
, @CustomerCode as CustomerCodeBill
, (select Odometer from #srv) Odometer
, (select IsPKP from #cus) as IsPKP
, (select TopCode from #cus) as TOPCode
, isnull((
	select b.ParaValue
	  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
	 where a.CompanyCode  = @CompanyCode
	   and a.BranchCode   = @BranchCode
	   and a.CustomerCode = @CustomerCode
	   and a.ProfitCenterCode = '200'
	   and b.CompanyCode  = a.CompanyCode
	   and b.CodeID = 'TOPC'
	   and b.LookUpValue = a.TopCode
	), null) as TOPDays
, isnull((
	select dateadd(day, convert(int,b.ParaValue), convert(varchar, getdate(), 112))
	  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
	 where a.CompanyCode  = @CompanyCode
	   and a.BranchCode   = @BranchCode
	   and a.CustomerCode = @CustomerCode
	   and a.ProfitCenterCode = '200'
	   and b.CompanyCode  = a.CompanyCode
	   and b.CodeID = 'TOPC'
	   and b.LookUpValue  = a.TopCode
	), null) as DueDate
, convert(varchar, getdate(), 112) SignedDate
, case @BillType
	when 'F' then (select LaborDiscPct from #cus) 
    when 'W' then (select LaborDiscPct from #cus) 
    else (select LaborDiscPct from #srv) 
  end as LaborDiscPct
, case @BillType
	when 'F' then (select PartDiscPct from #cus) 
    when 'W' then (select PartDiscPct from #cus) 
    else (select PartDiscPct from #srv) 
  end as PartsDiscPct
, case @BillType
	when 'F' then (select MaterialDiscPct from #cus) 
    when 'W' then (select MaterialDiscPct from #cus) 
    else (select MaterialDiscPct from #srv) 
  end as MaterialDiscPct
, @PPnPct as PPhPct
, @PPnPct as PPnPct
, @Remarks as Remarks
, '0' PrintSeq
, '0' PostingFlag
, '0' IsLocked
, @UserID CreatedBy
, getdate() CreatedDate

-- Insert Into svTrnInvTask
-----------------------------------------------------------------------------------------
insert into svTrnInvTask (
  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
, OperationHour, ClaimHour, OperationCost, SubConPrice
, IsSubCon, SharingTask, DiscPct
)
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
, isnull(OperationHour, 0) OperationHour, isnull(ClaimHour, 0) ClaimHour
, isnull(OperationCost, 0) OperationCost, isnull(SubConPrice, 0) SubConPrice
, isnull(IsSubCon, 0) IsSubCon, isnull(SharingTask, 0) SharingTask
, isnull(DiscPct, 0)
from #tsk

-- Insert Into svTrnInvMechanic
-----------------------------------------------------------------------------------------
insert into svTrnInvMechanic (
  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
, MechanicID, ChiefMechanicID, StartService, FinishService
)
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
, MechanicID, ChiefMechanicID, StartService, FinishService
from #mec

-- Insert Into svTrnInvItem
-----------------------------------------------------------------------------------------
select * into #itm1 from (
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, PartNo
	 , isnull((
		select MovingCode from spMstItems
		 where CompanyCode = @CompanyCode
		   and BranchCode = @BranchCode
		   and PartNo = #itm.PartNo
		), '') as MovingCode
	 , isnull((
		select ABCClass from spMstItems
		 where CompanyCode = @CompanyCode
		   and BranchCode = @BranchCode
		   and PartNo = #itm.PartNo
		), '') as ABCClass
	 , sum(SupplyQty - ReturnQty) as SupplyQty
	 , isnull((
		select 
		  case (sum(b.SupplyQty - b.ReturnQty))
			 when 0 then 0
			 else sum(a.CostPrice * (b.SupplyQty - b.ReturnQty)) / sum(b.SupplyQty - b.ReturnQty)
		  end 
	from SpTrnSLmpDtl a
	left join SvTrnSrvItem b on 1 = 1
	 and b.CompanyCode  = a.CompanyCode
	 and b.BranchCode   = a.BranchCode
	 and b.ProductType  = a.ProductType
	 and b.SupplySlipNo = a.DocNo
	 and b.PartNo = #itm.PartNo
	where 1 = 1
	 and a.CompanyCode = @CompanyCode
	 and a.BranchCode  = @BranchCode
	 and a.ProductType = @ProductType
	 and a.PartNo = #itm.PartNo
	 and a.DocNo in (
			select SupplySlipNo
			 from SvTrnSrvItem
			where 1 = 1
			  and CompanyCode = @CompanyCode
			  and BranchCode  = @BranchCode
			  and ProductType = @ProductType
			  and ServiceNo = @ServiceNo
			  and PartNo = #itm.PartNo
			)
	), 0) as CostPrice
, RetailPrice
, TypeOfGoods
from #itm
group by CompanyCode, BranchCode, ProductType, PartNo, RetailPrice, TypeOfGoods
)#

insert into svTrnInvItem (
  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo
, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice
, TypeOfGoods, DiscPct
)
select a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo
	 , MovingCode = (select top 1 MovingCode from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , ABCClass = (select top 1 ABCClass from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , sum(SupplyQty) as SupplyQty, 0 as ReturnQty
	 , CostPrice = (select top 1 CostPrice from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by CostPrice desc)
	 , RetailPrice = (select top 1 RetailPrice from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by RetailPrice desc)
	 , TypeOfGoods = (select top 1 TypeOfGoods from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , DiscPct = (select top 1 DiscPct from #itm where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
  from #itm1 a
 where a.SupplyQty > 0
 group by a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo


-- Insert Into svTrnInvItemDtl
-----------------------------------------------------------------------------------------
insert into svTrnInvItemDtl (
  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, SupplySlipNo
, SupplyQty, CostPrice, CreatedBy, CreatedDate
)
select y.* from (
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, PartNo, SupplySlipNo
, sum(SupplyQty - ReturnQty) as SupplyQty, CostPrice
, @UserID as CreatedBy, getdate() as CreatedDate
from #itm
group by CompanyCode, BranchCode, ProductType, PartNo, SupplySlipNo, CostPrice
) y
where y.SupplyQty > 0

-- Re Calculate Invoice
-----------------------------------------------------------------------------------------
exec uspfn_SvTrnInvoiceReCalculate @CompanyCode=@CompanyCode, @BranchCode=@BranchCode, @ProductType=@ProductType, @InvoiceNo=@InvoiceNo, @UserId=@UserId

drop table #srv
drop table #tsk
drop table #mec
drop table #itm
drop table #cus

drop table #pre_dtl
GO

if object_id('uspfn_SvTrnServiceSaveTask') is not null
	drop procedure uspfn_SvTrnServiceSaveTask
GO
--uspfn_SvTrnServiceSaveTask '6006410', '600641001', '4W', 4, 'C', 'PAKET GOM', 0.43, '1', 200000, 0, 'yo'

CREATE procedure [dbo].[uspfn_SvTrnServiceSaveTask]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@ServiceNo bigint,
	@BillType varchar(15),
    @OperationNo varchar(50),
    @OperationHour numeric(5,2),
    @UserPrice bit = 0,
    @TaskPrice numeric(18,0) = 0,
    @DiscPct numeric(5,2) = 0,
	@UserID varchar(15)
as      

declare @errmsg varchar(max)

begin try
	-- select data svTrnService
	select * into #srv from (
	  select a.* from svTrnService a
		where 1 = 1
		  and a.CompanyCode = @CompanyCode
		  and a.BranchCode  = @BranchCode
		  and a.ProductType = @ProductType
		  and a.ServiceNo   = @ServiceNo
	)#srv

	-- check jika count svTrnSrvTask ada, maka diupdate
	if(select count(*) from svTrnSrvTask
        where 1 = 1
          and CompanyCode = @CompanyCode
          and BranchCode  = @BranchCode
          and ProductType = @ProductType
          and ServiceNo   = @ServiceNo
          and OperationNo = @OperationNo) = 1
	begin
	    print 'update svTrnSrvTask'
		select 'update svTrnSrvTask'
		-- update svTrnSrvTask
		update svTrnSrvTask set
			   OperationHour  = @OperationHour
			  ,OperationCost  =
					case @UserPrice
					 when 0 then
							isnull((
						   (select LaborPrice from svMstTaskPrice
							 where CompanyCode = @CompanyCode
							   and BranchCode  = @BranchCode
							   and ProductType = (select ProductType from #srv)
							   and BasicModel  = (select BasicModel from #srv)
							   and JobType     = (select JobType from #srv)
							   and OperationNo = @OperationNo)
							),
						   (select LaborPrice from svMstTask
							 where CompanyCode = @CompanyCode
							   and ProductType = (select ProductType from #srv)
							   and BasicModel  = (select BasicModel from #srv)
							   and JobType     = (select JobType from #srv)
							   and OperationNo = @OperationNo)
							)
					 else @TaskPrice
					end	
			  ,BillType       = @BillType
			  ,LastupdateBy   = (select LastupdateBy from #srv)
			  ,LastupdateDate = (select LastupdateDate from #srv)
			  ,DiscPct        = @DiscPct 
		 where 1 = 1
	       and CompanyCode = @CompanyCode
		   and BranchCode  = @BranchCode
		   and ProductType = (select ProductType from #srv)
		   and ServiceNo   = (select ServiceNo from #srv)
		   and OperationNo = @OperationNo
	end
	else
	begin
		-- select svTrnSrvTask
		-----------------------------------------------
		 print 'insert svTrnSrvTask'
		 select top 1
			 a.CompanyCode
			,a.ProductType
			,a.BasicModel
			,a.JobType
			,a.OperationNo
			,case b.JobType when 'CLAIM' then isnull(c.ClaimHour, a.ClaimHour) else isnull(c.OperationHour, a.OperationHour) end as OperationHour
			,case b.JobType
			   when 'CLAIM' then a.LaborCost
				else (case @UserPrice when 0 then isnull(c.LaborPrice, a.LaborPrice) else @TaskPrice end)
			   end as LaborPrice
			,a.IsSubCon
			,isnull(c.LaborCost, a.LaborCost) LaborCost
			,isnull(c.ClaimHour, a.ClaimHour) ClaimHour
			,a.IsCampaign
			,a.LastUpdateBy
			,a.LastUpdateDate
		  from svMstTask a
		 inner join #srv b
			on b.CompanyCode = a.CompanyCode
		   and b.ProductType = a.ProductType
		   and b.BasicModel  = a.BasicModel
		  left join svMstTaskPrice c
			on c.CompanyCode = a.CompanyCode
		   and c.BranchCode  = b.BranchCode
		   and c.ProductType = a.ProductType
		   and c.BasicModel  = a.BasicModel
		   and c.JobType     = a.JobType
		   and c.OperationNo = a.OperationNo
		 where 1 = 1
		   and a.OperationNo = @OperationNo
		   and a.JobType in (b.JobType,'CLAIM','OTHER')
		   
		select * into #task from(
		select top 1
			 a.CompanyCode
			,a.ProductType
			,a.BasicModel
			,a.JobType
			,a.OperationNo
			,case b.JobType when 'CLAIM' then isnull(c.ClaimHour, a.ClaimHour) else isnull(c.OperationHour, a.OperationHour) end as OperationHour
			,case b.JobType
			   when 'CLAIM' then a.LaborCost
				else (case @UserPrice when 0 then isnull(c.LaborPrice, a.LaborPrice) else @TaskPrice end)
			   end as LaborPrice
			,a.IsSubCon
			,isnull(c.LaborCost, a.LaborCost) LaborCost
			,isnull(c.ClaimHour, a.ClaimHour) ClaimHour
			,a.IsCampaign
			,a.LastUpdateBy
			,a.LastUpdateDate
		  from svMstTask a
		 inner join #srv b
			on b.CompanyCode = a.CompanyCode
		   and b.ProductType = a.ProductType
		   and b.BasicModel  = a.BasicModel
		  left join svMstTaskPrice c
			on c.CompanyCode = a.CompanyCode
		   and c.BranchCode  = b.BranchCode
		   and c.ProductType = a.ProductType
		   and c.BasicModel  = a.BasicModel
		   and c.JobType     = a.JobType
		   and c.OperationNo = a.OperationNo
		 where 1 = 1
		   and a.OperationNo = @OperationNo
		   and a.JobType in (b.JobType,'CLAIM','OTHER')
		)#task

		if (select count(*) from #task) <> 1 return

		-- insert svMstJob
		select * into #job from(
		select a.* from svMstJob a, #task b
		 where 1 = 1
		   and a.CompanyCode = b.CompanyCode
		   and a.ProductType = b.ProductType
		   and a.BasicModel  = b.BasicModel
		   and a.JobType     = b.JobType
		)#job

		-- prepare data svTrnSrvTask yg akan di Insert
		if (left(@OperationNo,3) = 'FSC' or left(@OperationNo,3) = 'PDI')
		begin
			insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
			select
				 @CompanyCode CompanyCode
				,@BranchCode BranchCode
				,@ProductType ProductType
				,@ServiceNo ServiceNo
				,a.OperationNo
				,(case @OperationHour when 0 then a.OperationHour else @OperationHour end) OperationHour
				,OperationCost = isnull((select top 1 a.RegularLaborAmount
								   from svMstPdiFscRate a, #srv b, #task c, #job d 
								  where 1 = 1
									and a.CompanyCode = b.CompanyCode
									and a.ProductType = b.ProductType
									and a.BasicModel  = b.BasicModel
									and a.IsCampaign  = c.IsCampaign
									and a.TransmissionType = a.TransmissionType
									and a.PdiFscSeq = d.PdiFscSeq
									and a.EffectiveDate <= getdate()
									and a.IsActive = 1
								 order by a.EffectiveDate desc),0)
				,a.IsSubCon
				,a.LaborCost SubConPrice
				,'' PONo
				,a.ClaimHour
				,'L' TypeOfGoods
				,'F' BillType
				,'0' SharingTask
				,'0' TaskStatus
				,null StartService
				,null FinishService
				,b.LastupdateBy CreatedBy
				,b.LastupdateDate CreatedDate
				,b.LastupdateBy
				,b.LastupdateDate
				,@DiscPct
			from #task a, #srv b
		end
		else
		begin
			insert into svTrnSrvTask (CompanyCode, BranchCode, ProductType, ServiceNo, OperationNo, OperationHour, OperationCost, IsSubCon, SubConPrice, PONo, ClaimHour, TypeOfGoods, BillType, SharingTask, TaskStatus, StartService, FinishService, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
			select
				 @CompanyCode CompanyCode
				,@BranchCode BranchCode
				,@ProductType ProductType
				,@ServiceNo ServiceNo
				,a.OperationNo
				,(case @OperationHour when 0 then a.OperationHour else @OperationHour end) OperationHour
			    ,a.LaborPrice as OperationCost
				,a.IsSubCon
				,a.LaborCost SubConPrice
				,'' PONo
				,a.ClaimHour
				,'L' TypeOfGoods
				,@BillType BillType
				,'0' SharingTask
				,'0' TaskStatus
				,null StartService
				,null FinishService
				,b.LastupdateBy CreatedBy
				,b.LastupdateDate CreatedDate
				,b.LastupdateBy
				,b.LastupdateDate
				,@DiscPct
			from #task a, #srv b
		end

		-----------------------------------------------
		-- insert default svTrnSrvItem
		-----------------------------------------------
		select * into #part from(
		select a.* from svMstTaskPart a, #task b
		 where 1 = 1
		   and a.CompanyCode = b.CompanyCode
		   and a.ProductType = b.ProductType
		   and a.BasicModel  = b.BasicModel
		   and a.JobType     = b.JobType
		   and a.OperationNo = b.OperationNo
		)#part

		insert into svTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
		select 
			 @CompanyCode CompanyCode
			,@BranchCode BranchCode
			,@ProductType ProductType
			,@ServiceNo ServiceNo
			,a.PartNo
			,(row_number() over (order by a.PartNo)) PartSeq
			,a.Quantity DemandQty
			,'0' SupplyQty
			,'0' ReturnQty
			,c.CostPrice
			,case rtrim(a.BillType) when 'F' then a.RetailPrice else c.RetailPrice end RetailPrice
			,d.TypeOfGoods
			,a.BillType
			,null SupplySlipNo
			,null SupplySlipDate
			,null SSReturnNo
			,null SSReturnDate
			,b.LastupdateBy CreatedBy
			,b.LastupdateDate CreatedDate
			,b.LastupdateBy
			,b.LastupdateDate
			,@DiscPct
		  from #part a
		  left join #task b
			on b.CompanyCode = a.CompanyCode
		   and b.ProductType = a.ProductType
		   and b.BasicModel  = a.BasicModel
		   and b.JobType     = a.JobType
		   and b.OperationNo = a.OperationNo
		  left join spMstItemPrice c
			on c.CompanyCode = a.CompanyCode
		   and c.BranchCode  = @BranchCode
		   and c.PartNo      = a.PartNo
		  left join spMstItems d
			on d.CompanyCode = a.CompanyCode
		   and d.BranchCode  = @BranchCode
		   and d.PartNo      = a.PartNo
		 where 1 = 1
		   and b.CompanyCode = a.CompanyCode
		   and b.ProductType = a.ProductType
		   and b.BasicModel  = a.BasicModel
		   and b.JobType     = a.JobType
		   and b.OperationNo = a.OperationNo
		   and not exists (
				select 1 from svTrnSrvItem k, svTrnService l
				 where 1 = 1
				   and k.CompanyCode = l.CompanyCode
				   and k.BranchCode  = l.BranchCode
				   and k.ProductType = l.ProductType
				   and k.ServiceNo   = l.ServiceNo
				   and k.ServiceNo   = @ServiceNo
				   and k.PartNo      = a.PartNo
				)

		drop table #task
		drop table #part
		drop table #job
		end
	drop table #srv
end try
begin catch
	set @errmsg = error_message()
	raiserror (@errmsg,16,1);
end catch
GO


if object_id('uspfn_SvTrnServiceSelectDtl') is not null
	drop procedure uspfn_SvTrnServiceSelectDtl
GO
--declare	@CompanyCode varchar(15)
--declare	@BranchCode  varchar(15)
--declare	@ProductType varchar(15)
--declare	@ServiceNo   bigint

--set @CompanyCode = '6006400001' 
--set @BranchCode  = '6006400104'
--set @ProductType = '4W'
--set @ServiceNo   = 43543

--uspfn_SvTrnServiceSelectDtl '6006400001', '6006400104', '4w', 43543

CREATE procedure [dbo].[uspfn_SvTrnServiceSelectDtl]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@ServiceNo   bigint
as      

begin

declare @t1 as table
(
 TaskPartSeq int
,BillType varchar(10)
,BillTypeDesc varchar(max)
,TypeOfGoods varchar(10)
,TypeOfGoodsDesc varchar(70)
,TaskPartNo varchar(50)
,OprHourDemandQty numeric(18,2)
,SupplyQty numeric(18,2)
,ReturnQty numeric(18,2)
,OprRetailPrice numeric(18,2)
,OprRetailPriceTotal numeric(18,2)
,SupplySlipNo varchar(20)
,TaskPartDesc varchar(max)
,BasicModel varchar(15)
,JobType varchar(15)
,IsSubCon bit
,Status varchar(10)
,GTGO varchar(10)
,DiscPct numeric(18,2)
,QtyAvail numeric(18,2)
,TaskStatus varchar(50)
)

declare @JobOrderNo varchar(15)
    set @JobOrderNo = isnull((select JobOrderNo from svTrnService where CompanyCode = @CompanyCode and BranchCode = @BranchCode and ProductType = @ProductType and ServiceNo = @ServiceNo), '')

insert into @t1
select 0 TaskSeq 
      ,a.BillType
      ,b.Description BillTypeDesc
      ,a.TypeOfGoods
      ,case a.TypeOfGoods when 'L' then 'Labor (Jasa)' end TypeOfGoodsDesc
      ,a.OperationNo
      ,a.OperationHour
      ,0 OperationHourSupply
      ,0 OperationHourReturn
      ,a.OperationCost
      ,a.OperationHour * a.OperationCost * (100 - a.DiscPct) * 0.01 OprRetailPriceTotal
      ,'' SupplySlipNo
      ,rtrim(d.Description) OperationDesc 
	  ,c.BasicModel
	  ,c.JobType
	  ,a.IsSubCon
	  ,(select min(MechanicStatus) from svTrnSrvMechanic 
		where CompanyCode = a.CompanyCode 
			and BranchCode = a.BranchCode
			and ProductType = a.ProductType
			and ServiceNo = a.ServiceNo
			and OperationNo = a.OperationNo) MechanicStatus
	  ,''
	  ,a.DiscPct
	  ,0
      ,case(a.TaskStatus)
          when 0 then 'Open Task'
          when 1 then 'Work In Progress'
          when 2 then 'Close Task'
          when 9 then 'Cancel'
       end TaskStatus
  from svTrnSrvTask a with (nolock,nowait)
  left join svMstBillingType b with (nolock,nowait)
    on b.CompanyCode = a.CompanyCode
   and b.BillType = a.BillType
  left join svTrnService c with (nolock,nowait)
    on c.CompanyCode = a.CompanyCode
   and c.BranchCode = a.BranchCode
   and c.ProductType = a.ProductType
   and c.ServiceNo = a.ServiceNo
  left join svMstTask d with (nolock,nowait)
    on d.CompanyCode = a.CompanyCode
   and d.ProductType = a.ProductType
   and d.BasicModel = c.BasicModel
   and (d.JobType = c.JobType or d.JobType = 'CLAIM' or d.JobType = 'OTHER')
   and d.OperationNo = a.OperationNo 
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ProductType = @ProductType
   and a.ServiceNo   = @ServiceNo

declare @tblTemp as table
(
	PartNo  varchar(15),
	QtyAvail decimal
)

declare @DealerCode as varchar(2)
declare @CompanyMD as varchar(15)
declare @BranchMD as varchar(15)

set @DealerCode = 'MD'
set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)

if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
begin	
	set @DealerCode = 'SD'
	declare @DbName as varchar(50)
	set @DbName = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	
	declare @QueryTemp as varchar(max)
	
	set @QueryTemp = 'select 
			 a.PartNo
			 , (b.OnHand - (b.AllocationSP + b.AllocationSR + b.AllocationSL) - (b.ReservedSP + b.ReservedSR + b.ReservedSL)) 
		 from svTrnSrvItem a	 
		 left join ' + @DbName + '..spMstItems b on 
			a.PartNo = b.PartNo 
			and b.CompanyCode = ''' + @CompanyMD + '''
			and b.BranchCode = ''' + @BranchMD + '''
		 where a.CompanyCode = ''' + @CompanyCode + '''
		   and a.BranchCode  = ''' + @BranchCode + '''
		   and a.ProductType = ''' + @ProductType + '''
		   and a.ServiceNo   = ' + convert(varchar,@ServiceNo) + ''		   
				
		print(@QueryTemp)		
		insert into @tblTemp		
		exec (@QueryTemp)		
end

insert into @t1
select a.PartSeq
      ,a.BillType
      ,b.Description BillTypeDesc
      ,a.TypeOfGoods
      ,rtrim(c.LookupValueName) + case lower(g.ParaValue) when 'sparepart' then ' (SPR)' else ' (MTR)' end TypeOfGoodsDesc
      ,a.PartNo
      ,a.DemandQty
      ,a.SupplyQty
      ,a.ReturnQty
      ,a.RetailPrice
      ,(case isnull(a.SupplyQty, 0)
         when 0 then (isnull(a.DemandQty, 0) * isnull(a.RetailPrice, 0))
         else ((isnull(a.SupplyQty, 0) - isnull(a.ReturnQty, 0)) * isnull(a.RetailPrice, 0))
        end) * (100.0 - a.DiscPct) * 0.01
        as RetailPriceTotal
      ,a.SupplySlipNo
      ,rtrim(d.PartName) OperationDesc 
	  ,''
	  ,''
	  ,0
	  ,''
	  ,g.ParaValue
	  ,a.DiscPct
	  ,case when @DealerCode = 'MD' then (i.OnHand - (i.AllocationSP + i.AllocationSR + i.AllocationSL) - (i.ReservedSP + i.ReservedSR + i.ReservedSL)) else e.QtyAvail end QtyAvail
	  ,''
  from svTrnSrvItem a with (nolock,nowait)
  left join svMstBillingType b with (nolock,nowait)
    on b.CompanyCode = a.CompanyCode
   and b.BillType = a.BillType
  left join gnMstLookupDtl c with (nolock,nowait)
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'TPGO'
   and c.LookupValue = TypeOfGoods
  left join spMstItemInfo d with (nolock,nowait)
    on d.CompanyCode = a.CompanyCode
   and d.PartNo = a.PartNo
  left join gnMstLookupDtl g with (nolock,nowait)
    on g.CompanyCode = a.CompanyCode
   and g.CodeID = 'GTGO'
   and g.LookupValue = TypeOfGoods
  left join svTrnService s with (nolock,nowait)
    on s.CompanyCode = a.CompanyCode
   and s.BranchCode = a.BranchCode
   and s.ServiceNo = a.ServiceNo
  left join spMstItems i
    on i.CompanyCode = a.CompanyCode 
   and i.BranchCode = a.BranchCode
   and i.ProductType = a.ProductType
   and i.PartNo = a.PartNo
   left join @tblTemp e
    on e.PartNo = a.PartNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ProductType = @ProductType
   and a.ServiceNo   = @ServiceNo

select * into #t1 from (
select 
 a.* 
,P01 = isnull((
	select count(*) from spTrnSORDDtl with(nowait,nolock)
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and PartNo      = a.TaskPartNo
	   and DocNo in (select aa.DocNo
					   from spTrnSORDHdr aa with(nowait,nolock), svTrnService bb with(nowait,nolock)
					  where 1 = 1
						and bb.CompanyCode = aa.CompanyCode
						and bb.BranchCode  = aa.BranchCode
						and bb.JobOrderNo  = aa.UsageDocNo
						and isnull(bb.JobOrderNo, '') <> ''
						and aa.CompanyCode = @CompanyCode
						and aa.BranchCode  = @BranchCode
						and bb.ServiceNo   = @ServiceNo
					 )
	),0)
,P02 = isnull((
	select count(DocNo) from spTrnSOSupply with(nowait,nolock)
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and PartNo = a.TaskPartNo
	   and DocNo  = a.SupplySlipNo
	),0)
,P03 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSPickingHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P04 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSPickingHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	   and bb.Status >= '2'
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P05 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSLmpHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P06 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSLmpHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	   and bb.Status >= '1'
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,S01 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	),0)
,S02 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '2'
	),0)
,S03 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '3'
	),0)
,S04 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '4'
	),0)
,S05 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '5'
	),0)
from @t1 a
)#t1

update #t1
   set Status = (case P01 when 0 then 0 else 1 end)
			  + (case P02 when 0 then 0 else 1 end)
			  + (case P03 when 0 then 0 else 1 end)
			  + (case P04 when 0 then 0 else 1 end)
			  + (case P05 when 0 then 0 else 1 end)
			  + (case P06 when 0 then 0 else 1 end)
 where TypeOfGoods <> 'L'

update #t1
   set Status = (case S01 when 0 then 0 else 1 end)
			  + (case S02 when 0 then 0 else 1 end)
			  + (case S03 when 0 then 0 else 1 end)
			  + (case S04 when 0 then 0 else 1 end)
			  + (case S05 when 0 then 0 else 1 end)
 where TypeOfGoods = 'L' and IsSubCon = '1'

select
 row_number() over (order by TaskPartSeq) SeqNo
,TaskPartSeq
,BillType
,BillTypeDesc
,TypeOfGoods
,TypeOfGoodsDesc
,case isnull(TypeOfGoods, '') when 'L' then 'L' else '0' end ItemType
,TaskPartNo
,OprHourDemandQty
,SupplyQty
,ReturnQty
,OprRetailPrice
,OprRetailPriceTotal
,isnull(SupplySlipNo, '')SupplySlipNo
,TaskPartDesc
,Status
,StatusDesc = 
 case IsSubCon
	when 0 then
		 case TypeOfGoods 
			when 'L' then
				case Status
					when '0' then '0 - Open Task'
					when '1' then '1 - Work In Progress'
					when '2' then '2 - Finish Task'
				end
			else
				case Status
					when '1' then '1 - Entry Stock'
					when '2' then '2 - Alokasi Stock'
					when '3' then '3 - Generate PL'
					when '4' then '4 - Generate Bill'
					when '5' then '5 - Lampiran'
					when '6' then '6 - Print Lampiran'
				end
		 end	
	else
		case Status
			when '1' then '1 - Draft PO'
			when '2' then '2 - Generate PO'
			when '3' then '3 - Draft Receiving'
			when '4' then '4 - Cancel Receiving'
			when '5' then '5 - Receiving PO'
		end
 end
,QtyAvail
,(case when (SupplyQty > 0) then (SupplyQty - ReturnQty) else OprHourDemandQty end) * OprRetailPrice Price
,DiscPct
,OprRetailPriceTotal as PriceNet
,IsSubCon
,TaskStatus
,@ServiceNo ServiceNo
from #t1

drop table #t1

end
GO


if object_id('uspfn_SyncCsCustomerVehicleViewInitialize') is not null
	drop procedure uspfn_SyncCsCustomerVehicleViewInitialize
GO

create procedure [dbo].[uspfn_SyncCsCustomerVehicleViewInitialize]
as
begin
	;with x as (
	select a.CompanyCode
		 , b.BranchCode
		 , a.CustomerCode
		 , (d.ChassisCode + convert(varchar, d.ChassisNo)) as Chassis
		 , (d.EngineCode + convert(varchar, d.EngineNo)) as Engine
		 , b.SONo
		 , c.DONo
		 , c.DODate
		 , g.BpkNo
		 , d.SalesModelCode as CarType
		 , d.ColourCode as Color
		 , b.Salesman as SalesmanCode
		 , f.EmployeeName as SalesmanName
		 , e.PoliceRegNo
		 , c.DODate as DeliveryDate
		 , d.SalesModelCode
		 , d.SalesModelYear
		 , d.ColourCode
		 , g.BPKDate
		 , b.IsLeasing
		 , b.LeasingCo
		 , h.CustomerName as LeasingName
		 , b.Installment
	  from GnMstCustomer a
	 inner join omTrSalesSO b
		on b.CompanyCode = a.CompanyCode
	   and b.CustomerCode = a.CustomerCode
	  left join omTrSalesDO c
		on c.CompanyCode = b.CompanyCode
	   and c.CustomerCode = b.CustomerCode
	   and c.SONo = b.SONo
	  left join omTrSalesDODetail d
		on d.CompanyCode = c.CompanyCode
	   and d.BranchCode = c.BranchCode
	   and d.DONo = c.DONo
	  left join svMstCustomerVehicle e
		on e.CompanyCode = d.CompanyCode
	   and e.ChassisCode = d.ChassisCode
	   and e.ChassisNo = d.ChassisNo
	  left join HrEmployee f
		on f.CompanyCode = b.CompanyCode
	   and f.EmployeeID = b.Salesman
	  left join OmTrSalesBpk g
		on g.CompanyCode = c.CompanyCode
	   and g.BranchCode = c.BranchCode
	   and g.DONo = c.DONo
	   and g.SONo = c.SONo
	  left join gnMstCustomer h
		on h.CompanyCode = b.CompanyCode
	   and h.CustomerCode = b.LeasingCo
	 where 1 = 1
	   and d.ChassisCode is not null
	   and d.EngineCode is not null
	   and b.SODate is not null
	   and c.DODate is not null
	   and g.BpkNo is not null
	   and isnull(d.StatusBPK, 3) != '3'
	   and isnull(g.Status, 3) != '3'
	   and year(c.DODate) = year(getdate())
	)

	--select * from x
	select * into #t1 from (select * from x)#

--	delete CsCustomerVehicleView
--	 where exists (
--		select top 1 1 from #t1
--		 where #t1.CompanyCode = CsCustomerVehicleView.CompanyCode
--		   and #t1.BranchCode = CsCustomerVehicleView.BranchCode
--		   and #t1.Chassis = CsCustomerVehicleView.Chassis
--	 )
--
--	insert into CsCustomerVehicleView (CompanyCode, BranchCode, CustomerCode, Chassis, Engine, SONo, DONo, DoDate, BpkNo, CarType, Color, SalesmanCode, SalesmanName, PoliceRegNo, DeliveryDate, SalesModelCode, SalesModelYear, ColourCode, BpkDate, IsLeasing, LeasingCo,
--	 LeasingName, Installment)
--	select * from #t1

--	drop table CsCustomerVehicleView
	select * into CsCustomerVehicleView from #t1

	drop table #t1
end
GO



if object_id('uspfn_TransferEmployeeToSimDMS') is not null
	drop procedure uspfn_TransferEmployeeToSimDMS
GO
create procedure [dbo].[uspfn_TransferEmployeeToSimDMS]   
 @CompanyCode varchar(17),  
 @EmployeeID varchar(25),  
 @UserID varchar(25)  
as  
  
begin  
 declare @TransactionName varchar(25);  
 set @TransactionName='TransferEmployee';  
  
 begin transaction  
  
 begin try  
  declare @NumberOfRecord bit;  
  set @NumberOfRecord = (select count(EmployeeID) from HrEmployee where EmployeeID=@EmployeeID);  
  --select @CompanyCode, @EmployeeID, @NumberOfRecord as NumberOfRecord;  
  
  if(@NumberOfRecord < 1)   
  begin  
   insert into HrEmployee ( CompanyCode  
           , EmployeeID  
           , EmployeeName  
           , Email  
           , FaxNo  
           , Handphone1  
           , Handphone2  
           , Handphone3  
           , Handphone4  
           , Telephone1  
           , Telephone2   
           , OfficeLocation  
           , IsLinkedUser  
           , RelatedUser  
           , JoinDate  
           , Department  
           , Position  
           , Grade  
           , Rank  
           , Gender  
           , TeamLeader  
           , PersonnelStatus  
           , ResignDate  
           , ResignDescription  
           , IdentityNo  
           , NPWPNo  
           , NPWPDate  
           , BirthDate  
           , BirthPlace  
           , Address1  
           , Address2  
           , Address3  
           , Address4  
           , Province  
           , District  
           , SubDistrict  
           , Village  
           , ZipCode  
           , DrivingLicense1  
           , DrivingLicense2  
           , MaritalStatus  
           , MaritalStatusCode  
           , Height  
           , Weight  
           , UniformSize  
           , UniformSizeAlt  
           , ShoesSize  
           , FormalEducation  
           , BloodCode  
           , OtherInformation  
           , CreatedBy  
           , CreatedDate  
           , UpdatedBy  
           , UpdatedDate  
           , Religion  
           , SelfPhoto  
           , IdentityCardPhoto  
           , IsDeleted   
           )  
    select @CompanyCode              
         , a.EmployeeID  
      , a.EmployeeName  
      , Email = (  
       select top 1  
           x.Email  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        )  
      , a.FaxNo  
      , Handphone1 = (  
       select top 1  
           x.Phone2  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        )   
      , Handphone2 = (  
       select top 1  
           x.Phone2  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        )   
      , Handphone3 = (  
       select top 1  
           x.Phone3  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        )   
      , Handphone4 = (  
       select top 1  
           x.Phone4  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        )   
      , Telephone1 = a.HpNo  
      , Telephone2 = a.PhoneNo  
      , OfficeLocation = ''  
      , IsLinkedUser = 0  
      , RelatedUser = ''  
      , Convert(datetime, a.JoinDate)  
      , Department = isnull( (  
       select top 1  
              x.HistoryDeptCode  
         from SfEmployeeTitleHistory x  
        where x.CompanyCode=a.CompanyCode  
          and a.EmployeeID=@EmployeeID  
        order by x.AssignedDate desc  
        ), (  
       select top 1  
           x.OrgCode  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        order by x.UpdatedDate desc  
        ) )  
      , Position = isnull( (  
       select top 1  
              x.HistoryPosCode  
         from SfEmployeeTitleHistory x  
        where x.CompanyCode=a.CompanyCode  
          and a.EmployeeID=@EmployeeID  
        order by x.AssignedDate desc  
        ), (  
       select top 1  
           x.PosCode  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        order by x.UpdatedDate desc  
        ) )  
      , Grade = isnull( (  
       select top 1  
              x.HistoryGrade  
         from SfEmployeeGradeHistory x  
        where x.CompanyCode=a.CompanyCode  
          and a.EmployeeID=@EmployeeID  
        order by x.AssignedDate desc  
        ), (  
       select top 1  
           x.GradeCode  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        order by x.UpdatedDate desc  
        ) )  
      , (  
       select top 1  
           x.RankCode  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        order by x.UpdatedDate desc  
        ) as RankCode  
      , a.GenderCode              
      , TeamLeader = (  
          select top 1  
           x.TeamLeaderID  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        order by x.UpdatedDate desc  
        )  
      , PersonnelStatus = a.PersonnelStatus  
      , Convert(datetime, a.ResignDate)  
      , ResignDescription = (  
          select top 1  
           x.ResignReason  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        order by x.UpdatedDate desc  
        )  
      , a.IdentityNo  
      , NPWPNo = (  
       select top 1  
           x.NPWPNo  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        order by x.UpdatedDate desc  
        )  
      , NPWPDate = (  
       select top 1  
           x.NpwpDate  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        order by x.UpdatedDate desc  
        )  
      , a.BirthDate  
      , a.BirthPlace  
      , a.Address1  
      , a.Address2  
      , a.Address3  
      , a.Address4  
      , a.ProvinceCode  
      , a.CityCode as District  
      , a.AreaCode as SubDistrict   
      , '' as Village  
      , a.ZipNo as ZipCode  
      , DrivingLicense1 = (  
       select top 1  
           x.DrivingLicense1  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        order by x.UpdatedDate desc  
        )  
      , DrivingLicense1 = (  
       select top 1  
           x.DrivingLicense2  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        order by x.UpdatedDate desc  
        )  
      , a.MaritalStatusCode as MaritalStatus  
      , '' as MaritalStatusCode  
      , a.Height  
      , a.Weight  
      , a.UniformSize  
      , UniformSizeAlt = (  
       select top 1  
           x.SizeAlt  
         from gnMstEmployeeData x  
        where x.CompanyCode=a.CompanyCode  
          and x.EmployeeID=a.EmployeeID  
        order by x.UpdatedDate desc  
        )  
      , a.ShoesSize  
      , a.FormalEducation  
      , a.BloodCode  
      , '' as OtherInformation  
      , @UserID as CreatedBy  
      , getdate()  
      , @UserID as UpdatedBy  
      , getdate()  
      , a.ReligionCode  
      , a.EmpImageID  
      , a.EmpIdentityCardImageID  
      , convert(bit, 0) as IsDeleted  
      from gnMstEmployee a  
     where 1=1  
       --and a.CompanyCode=@CompanyCode  
       and a.EmployeeID=@EmployeeID  
  
     
   insert into HrEmployeeAchievement  
   select   
       a.CompanyCode  
     , a.EmployeeID  
     , a.AssignedDate  
     , a.HistoryDeptCode  
     , a.HistoryPosCode  
     , ''  
     , 0  
     , @UserID  
     , getdate()  
     , @UserID  
     , getdate()  
     , 0  
     from SfEmployeeTitleHistory a  
    where a.CompanyCode=@CompanyCode  
      and a.EmployeeID=@EmployeeID  
  
  
  
   insert into HrEmployeeTraining  
   select   
     a.CompanyCode  
     , a.EmployeeID  
     , a.TrnCode  
     , a.TrnDate  
     , a.TrnSeq  
     , 0  
     , a.PreTest  
     , ''  
     , a.PostTest  
     , ''  
     , 'system'  
     , getdate()  
     , 'system'  
     , getdate()  
     , 0  
     from SfEmployeeTraining a  
    where a.CompanyCode=@CompanyCode  
      and a.EmployeeID=@EmployeeID  
  
  
   insert into HrEmployeeMutation  
   select a.CompanyCode  
     , a.EmployeeID  
     , a.MutationDate  
     , a.MutationTo  
     , 0  
     , 'system'  
     , getdate()  
     , 'system'  
     , getdate()  
     , 0  
     from gnMstEmployeeMutation a  
    where a.CompanyCode=@CompanyCode  
      and a.EmployeeID=@EmployeeID  
  
   insert into HrEmployeeSales  
   select @CompanyCode  
        , @EmployeeID  
     , SalesID=(  
      select top 1  
             x.SalesID  
        from gnMstEmployeeData x  
       where x.CompanyCode=@CompanyCode  
         and x.EmployeeID=@EmployeeID  
         and x.SalesID is not null  
       )  
     , @UserID  
     , getdate()  
     , convert(bit, 0)  
     , convert(bit, 0)  
     , null  
     , null  
     , convert(bit, 0)  
	 , @UserID  
     , getdate() 
   select convert(bit, 1) as Result;  
  end  
  else  
  begin  
   select convert(bit, 0) as Result;  
  end  
  
  commit transaction;  
 end try  
 begin catch  
  rollback transaction;  
  select convert(bit, 0) as Result;  
 end catch  
end  
  
--select * from HrEmployeeSAles
GO

if object_id('uspfn_omSlsDoLkpShipto') is not null
	drop procedure uspfn_omSlsDoLkpShipto
GO
CREATE procedure [dbo].[uspfn_omSlsDoLkpShipto]   
(  
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),  
 @ProfitCenterCode varchar(3)
) 
as begin
--exec uspfn_omSlsDoLkpShipto 6006410,600641001,300
SELECT a.CompanyCode
	,a.CustomerCode
	,a.StandardCode
	,a.CustomerName
	,a.CustomerAbbrName
	,a.CustomerGovName
	,a.CustomerType
	,a.CategoryCode
	,a.Address1
	,a.Address2
	,a.Address3
	,a.Address4
	,a.PhoneNo
	,a.HPNo
	,a.FaxNo
	,a.isPKP
	,a.NPWPNo
	,a.NPWPDate
	,a.SKPNo
	,a.SKPDate
	,a.ProvinceCode
	,a.AreaCode
	,a.CityCode
	,a.ZipNo
	,a.Status
	,a.CreatedBy
	,a.CreatedDate
	,a.LastUpdateBy
	,a.LastUpdateDate
	,a.isLocked
	,a.LockingBy
	,a.LockingDate
	,a.Email
	,a.BirthDate
	,a.Spare01
	,a.Spare02
	,a.Spare03
	,a.Spare04
	,a.Spare05
	,a.Gender
	,a.OfficePhoneNo
	,a.KelurahanDesa
	,a.KecamatanDistrik
	,a.KotaKabupaten
	,a.IbuKota
	,a.CustomerStatus
	  FROM gnMstCustomer a 
	INNER JOIN gnMstCustomerProfitCenter b
	  ON a.CompanyCode = b.CompanyCode AND 
		 a.CustomerCode = b.CustomerCode AND
		 b.BranchCode = @BranchCode
	WHERE a.CompanyCode = @CompanyCode AND 
		  b.ProfitCenterCode = @ProfitCenterCode                      
end
GO





