if object_id('uspfn_GenerateBPSLampiranNew') is not null
	drop procedure uspfn_GenerateBPSLampiranNew
GO
create procedure [dbo].[uspfn_GenerateBPSLampiranNew]
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

--DECLARE	@CompanyCode	VARCHAR(MAX),
--		@BranchCode		VARCHAR(MAX),
--		@JobOrderNo		VARCHAR(MAX),
--		@ProductType	VARCHAR(MAX),
--		@CustomerCode	VARCHAR(MAX),
--		@UserID			VARCHAR(MAX),
--		@PickedBy		VARCHAR(MAX)

--SET	@CompanyCode	= '6156401000'
--SET	@BranchCode		= '6156401001'
--SET	@JobOrderNo		= 'SPK/15/001666'
--SET	@ProductType	= '4W'
--SET	@CustomerCode	= '0032710'
--SET	@UserID			= 'ga'
--SET	@PickedBy		= '0004'
		
--exec uspfn_GenerateBPSLampiranNew '6006400001','6006400101','SPK/14/101625','4W','JKT-1852626','ga','00001'

--===============================================================================================================================
-- RE-CALCULATE AMOUNT DETAIL
--===============================================================================================================================
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

if object_id('#tmpSvSDMovement') is not null drop table #tmpSvSDMovement

DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

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
	DECLARE @CompanyMD AS VARCHAR(15)
	DECLARE @BranchMD AS VARCHAR(15)
	DECLARE @WarehouseMD AS VARCHAR(15)

	SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
	SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
	SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

declare @validString varchar(max)

declare @valid_2 table(
PartNo varchar(15),
QtyValidSR NUMERIC(18,2),
QtyValidOnhand NUMERIC(18,2)
)
    set @validString ='SELECT a.PartNo
		, a.AllocationSR - b.QtyBill QtyValidSR
		, a.Onhand - b.QtyBill QtyValidOnhand
	FROM '+ @DbMD +'..SpMstItems a, SpTrnSPickingDtl b
	WHERE 1 = 1
		AND a.CompanyCode = '''+ @CompanyMD +'''
		AND a.BranchCode = '''+ @BranchMD+'''
		AND b.PickingSlipNo = '''+@PickingSlipNo+'''
		--AND a.CompanyCode = b.CompanyCode
		--AND a.BranchCode = b.BranchCode
		AND a.PartNo = b.PartNo'
	
	--print(@validString)
	insert into @valid_2 exec(@validString)

select * from @valid_2

	SET @Allocation_SRValid = ISNULL((SELECT TOP 1 QtyValidSR FROM @valid_2 WHERE QtyValidSR < 0),0)
	SET @Onhand_SRValid = ISNULL((SELECT TOP 1 QtyValidOnhand FROM @valid_2 WHERE QtyValidOnhand < 0),0)
	select @Allocation_SRValid
	select @Onhand_SRValid

	IF (@Onhand_SRValid < 0 OR @Allocation_SRValid < 0)
	BEGIN
		SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
		
		CLOSE db_cursor
		DEALLOCATE db_cursor 
		
		DROP TABLE #TempPickingSlipDtl
		DROP TABLE #TempPickingSlipHdr
		
		RAISERROR (@errmsg,16,1);
		
		RETURN
	END
--===============================================================================================================================

--DECLARE @DbMD AS VARCHAR(15)
--SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

DECLARE @Query AS VARCHAR(MAX)
SET @Query = 
'UPDATE '+@DbMD+'..SpMstItems
SET
	AllocationSR = AllocationSR - b.QtySupply
	, Onhand = Onhand - b.QtyBill
	, LastUpdateBy = ' + '''' + @UserID + '''' +'
	, LastUpdateDate = GetDate()
	, LastSalesDate = GetDate()
FROM ' + @DbMD + '..SpMstItems a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = ' + ''''+@CompanyMD+'''' + '
	AND a.BranchCode = ' + ''''+@BranchMD +''''+ '
	AND b.PickingSlipNo = ' + ''''+@PickingSlipNo+'''' + '
	--AND a.CompanyCode = b.CompanyCode
	--AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo
UPDATE '+ @DbMD +'..SpMstItemLoc
SET
	AllocationSR = AllocationSR - b.QtySupply
	, Onhand = Onhand - b.QtyBill
	, LastUpdateBy = ' + '''' + @UserID + '''' +'
	, LastUpdateDate = GetDate()
FROM ' + @DbMD + '..SpMstItemLoc a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = ' + ''''+@CompanyMD+'''' + '
	AND a.BranchCode = ' + ''''+@BranchMD +''''+ '
	AND a.WarehouseCode = ' + ''''+@WarehouseMD +''''+ '
	AND b.PickingSlipNo = ' + ''''+@PickingSlipNo+'''' + '
	--AND a.CompanyCode = b.CompanyCode
	--AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo'

EXEC(@query)
	--print(@query)
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

--===============================================================================================================================
-- INSERT INTO svSDMovement
--===============================================================================================================================
	set @Query = '
	insert into ' + @DbMD + '..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq
	, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice
	, TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD
	, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus
	, ProcessDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)  
	select h.CompanyCode, h.BranchCode, h.LmpNo, h.LmpDate, d.PartNo, ROW_NUMBER() OVER(Order by d.LmpNo)
	,d.WarehouseCode, d.QtyBill, 0, d.DiscPct, d.CostPrice, d.RetailPrice
	,h.TypeOfGoods, ''' + @CompanyMD + ''', ''' + @BranchMD + ''', ''' + @WarehouseMD + ''', d.RetailPriceInclTax, d.RetailPrice, 
	d.CostPrice,''x'', d.ProductType,''300'', ''0'',''0'',''' + convert(varchar, GETDATE()) + ''',''' + @UserID + ''',''' +
	  convert(varchar, GETDATE()) + ''',''' +  @UserID + ''',''' +  convert(varchar, GETDATE()) + '''
	 from spTrnSLmpDtl d inner join spTrnSLmpHdr h on h.LmpNo = d.LmpNo  
	  where 1 = 1   
		and d.CompanyCode = ''' + @CompanyCode + ''' 
		and d.BranchCode  = ''' + @BranchCode  + '''
		and d.ProductType = ''' + @ProductType  + '''
		and d.LmpNo = ''' + @TempLmpNo + '''';
	
	exec(@Query);

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
end

--rollback tran
GO
