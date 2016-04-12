CREATE procedure [dbo].[uspfn_GenerateReturnServiceWeb]
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@InvoiceNo		VARCHAR(MAX),
	@ReturnNo		VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@PartNo			VARCHAR(MAX)
	
AS
BEGIN

DECLARE @ReturnDate DATETIME SET @ReturnDate = GetDate()

--DECLARE 	@CompanyCode	VARCHAR(MAX),
--			@BranchCode		VARCHAR(MAX),
--			@ProductType	VARCHAR(MAX),
--			@InvoiceNo		VARCHAR(MAX),
--			@ReturnNo		VARCHAR(MAX),
--			@UserID			VARCHAR(MAX)

--SET	@CompanyCode	= '6006406'
--SET	@BranchCode		= '6006401'
--SET	@ProductType	= '4W'
--SET	@InvoiceNo		= 'INC/12/001002'
--SET	@ReturnNo		= 'RTN/12/000003'
--SET	@UserID			= 'sa'

-- ============================================================================================================================
-- INITIALIZE DATA SOURCE BY INVOICE SERVICE
-- ============================================================================================================================
SELECT * INTO #InvoicePart FROM (
SELECT
	a.CompanyCode
	, a.BranchCode
	, CustomerCode
	, isPKP
	, FpjNo
	, FpjDate 
	, a.InvoiceNo
	, a.InvoiceDate
	, JobOrderNo
	, JobOrderDate
	, TypeOfGoods
	, PartNo
	, PartsDiscPct
	, MaterialDiscPct 
	, PPNpct
	, SupplyQty
	, CostPrice
	, RetailPrice
	, b.DiscPct
FROM svTrnInvoice a
INNER JOIN svTrnInvItem b  ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.InvoiceNo = b.InvoiceNo
WHERE a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.InvoiceNo = @InvoiceNo 
	AND b.PartNo = @PartNo
	) #InvoicePart

-- ============================================================================================================================
-- INSERT HEADER RETUR SERVICE SPAREPART HEADER
-- ============================================================================================================================
if not exists (select InvoiceNo from spTrnSRturSrvHdr where CompanyCode = @CompanyCode and BranchCode = @BranchCode and InvoiceNo = @InvoiceNo)
begin
	INSERT INTO [spTrnSRturSrvHdr](
		[CompanyCode] 
		,[BranchCode]
		,[ReturnNo] 
		,[ReturnDate] 
		,[CustomerCode] 
		,[InvoiceNo] 
		,[InvoiceDate] 
		,[FPJNo] 
		,[FPJDate] 
		,[ReferenceNo] 
		,[ReferenceDate] 
		,[TotReturQty] 
		,[TotReturAmt] 
		,[TotDiscAmt]
		,[TotDPPAmt] 
		,[TotPPNAmt] 
		,[TotFinalReturAmt] 
		,[TotCostAmt] 
		,[isPKP] 
		,[NPWPNo] 
		,[PrintSeq] 
		,[Status] 
		,[CreatedBy] 
		,[CreatedDate] 
		,[LastUpdateBy] 
		,[LastUpdateDate] ) 
	SELECT TOP 1 CompanyCode
		, BranchCode
		, @ReturnNo ReturnNo
		, @ReturnDate ReturnDate
		, a.CustomerCode
		, a.InvoiceNo
		, a.InvoiceDate
		, a.FpjNo
		, a.FpjDate
		, a.JobOrderNo
		, a.JobOrderDate
		, 0
		, 0
		, 0
		, 0
		, 0
		, 0
		, 0
		, isPKP
		, (SELECT NPWPNo FROM GNMstCustomer WHERE CompanyCode = a.CompanyCode AND CustomerCode = a.CustomerCode) NPWPNo
		, 0 PrintSeq
		, 2 Status
		, @UserId CreatedBy
		, GetDate() CreatedDate
		, @UserId LastUpdateBy
		, GetDate() LastUpdateDate
		FROM #InvoicePart a 
END
-- ============================================================================================================================
-- INSERT RETURN SERVICE SPAREPART DETAIL PART
-- ============================================================================================================================
INSERT INTO [spTrnSRturSrvDtl]
   ([CompanyCode]
   ,[BranchCode]
   ,[ReturnNo]
   ,[PartNo]
   ,[PartNoOriginal]
   ,[WarehouseCode]
   ,[DocNo]
   ,[ReturnDate]
   ,[QtyReturn]
   ,[RetailPriceInclTax]
   ,[RetailPrice]
   ,[CostPrice]
   ,[DiscPct]
   ,[ReturAmt]
   ,[DiscAmt]
   ,[NetReturAmt]
   ,[PPNAmt]
   ,[TotReturAmt]
   ,[CostAmt]
   ,[LocationCode]
   ,[ProductType]
   ,[PartCategory]
   ,[MovingCode]
   ,[ABCClass]
   ,[TypeOfGoods]
   ,[CreatedBy]
   ,[CreatedDate])
SELECT a.CompanyCode
, a.BranchCode
, @ReturnNo ReturnNo
, a.PartNo
, a.PartNo
, '00' WarehouseCode
, a.JobOrderNo DocNo
, @ReturnDate ReturnDate
, a.SupplyQty QtyReturn
, a.RetailPrice * 1.1 RetailPriceInclTax
, a.RetailPrice
, a.CostPrice
, a.DiscPct
, a.RetailPrice * a.SupplyQty ReturAmt
, FLOOR((a.RetailPrice * a.SupplyQty) * (a.DiscPct /100)) DiscAmt
, (a.RetailPrice * a.SupplyQty) - FLOOR((a.RetailPrice * a.SupplyQty) * (a.MaterialDiscPct /100)) NetReturAmt
, FLOOR((a.RetailPrice * a.SupplyQty) * (PPNPct / 100)) PPNAmt
, (a.RetailPrice * a.SupplyQty) - FLOOR((a.RetailPrice * a.SupplyQty) * (a.MaterialDiscPct /100))
			+ FLOOR((a.RetailPrice * a.SupplyQty) * (PPNPct / 100)) TotReturAmt
, a.CostPrice * a.SupplyQty CostAmt
, (
	SELECT LocationCode 
	FROM SpMstItemLoc 
	WHERE CompanyCode = a.CompanyCode 
		AND BranchCode = a.BranchCode 
		AND PartNo = a.PartNo 
	AND WarehouseCode = '00') LocationCode
, @ProductType ProductType
, (
	SELECT PartCategory 
	FROM SpMstItems 
	WHERE CompanyCode = a.CompanyCode 
		AND BranchCode = a.BranchCode 
		AND PartNo = a.PartNo) PartCategory
, (
	SELECT MovingCode 
	FROM SpMstItems 
	WHERE CompanyCode = a.CompanyCode 
		AND BranchCode = a.BranchCode 
		AND PartNo = a.PartNo) MovingCode
, (
	SELECT ABCClass 
	FROM SpMstItems 
	WHERE CompanyCode = a.CompanyCode 
		AND BranchCode = a.BranchCode 
		AND PartNo = a.PartNo) ABCClass
, a.TypeOfGoods
, @UserID CreatedBy
, GetDate() CreatedDate
FROM #InvoicePart a 

-- ============================================================================================================================
-- UPDATE RETURN SERVICE SPAREPART HEADER AMOUNT AND QUANTITY
-- ============================================================================================================================
SELECT * INTO #TempAmtHeader FROM (
SELECT 
	CompanyCode
	, BranchCode	
	, ReturnNo
	, SUM(QtyReturn) TotReturnQty
	, SUM(ReturAmt) TotReturAmt
	, SUM(DiscAmt) TotDiscAmt
	, SUM(NetReturAmt) TotDPPAmt
	, SUM(NetReturAmt * 10 /100) TotPPNAmt
	, SUM(CostAmt) TotCostAmt 	
FROM SpTrnSRturSrvDtl
WHERE CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND ReturnNo = @ReturnNo
GROUP BY CompanyCode, BranchCode, ReturnNo
) #TempAmtHeader


UPDATE spTrnSRturSrvHdr
SET TotReturQty = b.TotReturnQty
	, TotReturAmt = b.TotReturAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalReturAmt = b.TotDPPAmt + b.TotPPNAmt
	, TotCostAmt = b.TotCostAmt
FROM spTrnSRturSrvHdr a, #TempAmtHeader b
WHERE a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ReturnNo = b.ReturnNo

DROP TABLE #TempAmtHeader
-- ============================================================================================================================
-- INITIALIZE DATABASE MAIN DEALER
-- ============================================================================================================================
DECLARE @CompanyMD varchar(15), @BranchMD varchar(15),  @DBMD varchar(15)

set @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)
set @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)  
set @DBMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)

-- ============================================================================================================================
-- AVERAGE COST PRICE (UPDATE MASTER ITEM PRICE AND HISTORY)
-- ============================================================================================================================
DECLARE @Query1 varchar(MAX)

set @Query1 = 'SELECT * INTO #TempAvgPrice FROM ( 
SELECT
	a.CompanyCode
	, a.BranchCode
	, a.PartNo
	, ROUND((((b.OnHand * c.CostPrice) + ((a.QtyReturn * a.CostPrice) * (1-(a.DiscPct/100)))) 
		/ (a.QtyReturn + b.OnHand)),2,2  )AvgCost 
	, c.CostPrice
FROM SpTrnSRturSrvDtl a
	LEFT JOIN '+@DBMD+'..SpMstItems b ON a.PartNo = b.PartNo
	LEFT JOIN '+@DBMD+'..SpMstItemPrice c ON a.PartNo = c.PartNo
WHERE a.CompanyCode = '''+@CompanyCode+'''
	AND a.BranchCode = '''+@BranchCode+'''
	AND a.ReturnNo = '''+@ReturnNo+''' ) #TempAvgPrice

INSERT INTO '+@DBMD+'..[spHstItemPrice]
       ([CompanyCode]
       ,[BranchCode]
       ,[PartNo]
       ,[UpdateDate]
       ,[RetailPrice]
       ,[RetailPriceInclTax]
       ,[PurchasePrice]
       ,[CostPrice]
       ,[Discount]
       ,[OldRetailPrice]
       ,[OldPurchasePrice]
       ,[OldCostPirce]
       ,[OldDiscount]
       ,[LastPurchaseUpdate]
       ,[LastRetailPriceUpdate]
       ,[CreatedBy]
       ,[CreatedDate]
       ,[LastUpdateBy]
       ,[LastUpdateDate])
SELECT a.CompanyCode
	, a.BranchCode
	, a.PartNo
	, DATEADD(ss,row_number() over (order by a.PartNo ASC), GetDate()) UpdateDate
	, a.RetailPrice 
	, a.RetailPriceInclTax
	, a.PurchasePrice
	, b.AvgCost CostPrice
	, ISNULL((
		SELECT TOP 1 Discount 
		FROM SpHstItemPrice 
		WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode
			AND PartNo = a.PartNo
		ORDER BY UpdateDate DESC
		),0) Discount
	, a.RetailPrice OldRetailPrice
	, a.PurchasePrice OldPurchasePrice
	, a.CostPrice OldCostPrice
	, ISNULL((
		SELECT TOP 1 Discount 
		FROM SpHstItemPrice 
		WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode
			AND PartNo = a.PartNo
		ORDER BY UpdateDate DESC
		),0) Discount
	, ISNULL((
		SELECT TOP 1 LastPurchaseUpdate 
		FROM SpHstItemPrice 
		WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode
			AND PartNo = a.PartNo
		ORDER BY UpdateDate DESC
		), NULL)LastPurchaseUpdate
	, ISNULL((
		SELECT TOP 1 LastRetailPriceUpdate 
		FROM SpHstItemPrice 
		WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode
			AND PartNo = a.PartNo
		ORDER BY UpdateDate DESC
		), NULL) LastRetailPriceUpdate 
	, '''+@UserID+''' CreatedBy
	, GetDate() CreatedDate
	, '''+@UserID+''' LastUpdateBy
	, GetDate() LastUpdateDate
FROM '+@DBMD+'..spMstItemPrice a
	INNER JOIN #TempAvgPrice b ON a.CompanyCode = b.CompanyCode 
		AND a.BranchCode = b.BranchCode 
		AND a.PartNo = b.PartNo

UPDATE '+@DBMD+'..SpMstItemPrice
SET CostPrice = b.AvgCost
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = GetDate()
FROM '+@DBMD+'..SpMstItemPrice a, #TempAvgPrice b
WHERE a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo
DROP TABLE #TempAvgPrice'

Print (@Query1)
EXEC (@Query1)

-- ============================================================================================================================
-- UPDATE STOCK AND MOVEMENT
-- ============================================================================================================================
DECLARE @Query2 varchar(max)

set @Query2 = 'UPDATE '+@DBMD+'..SpMstItems
SET Onhand = Onhand + b.QtyReturn
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = GetDate()
FROM '+@DBMD+'..SpMstItems a, SpTrnSRturSrvDtl b
WHERE a.PartNo = b.PartNo
	AND b.CompanyCode = '''+@CompanyCode+'''
	AND b.BranchCode = '''+@BranchCode+'''
	AND b.ReturnNo = '''+@ReturnNo+'''	


UPDATE '+@DBMD+'..SpMstItemLoc
SET Onhand = Onhand + b.QtyReturn
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = GetDate()
FROM '+@DBMD+'..SpMstItemLoc a, SpTrnSRturSrvDtl b
WHERE a.PartNo = b.PartNo
	AND a.WarehouseCode = ''00''
	AND b.CompanyCode = '''+@CompanyCode+'''
	AND b.BranchCode = '''+@BranchCode+'''
	AND b.ReturnNo = '''+@ReturnNo+''''	

Print(@Query2)
Exec(@Query2)

INSERT INTO [spTrnIMovement]
       ([CompanyCode]
       ,[BranchCode]
       ,[DocNo]
       ,[DocDate]
       ,[CreatedDate]
       ,[WarehouseCode]
       ,[LocationCode]
       ,[PartNo]
       ,[SignCode]
       ,[SubSignCode]
       ,[Qty]
       ,[Price]
       ,[CostPrice]
       ,[ABCClass]
       ,[MovingCode]
       ,[ProductType]
       ,[PartCategory]
       ,[CreatedBy])
SELECT a.CompanyCode
	, a.BranchCode
	, a.ReturnNo DocNo
	, (
		SELECT ReturnDate 
		FROM SpTrnSRturSrvHdr 
		WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode 
			AND ReturnNo = a.ReturnNo
	  ) DocDate
	, DATEADD(ss,row_number() over (order by a.PartNo ASC), GetDate()) CreatedDate
	, '00' WarehouseCode
	, (
		SELECT LocationCode
		FROM SpMstItemLoc 
		WHERE CompanyCode = a.CompanyCode
			AND BranchCode = a.BranchCode
			AND WarehouseCode = '00'
			AND PartNo = a.PartNo
	  ) LocationCode
	, a.PartNo
	, 'IN' SignCode
	, 'RSRV' SubSignCode
	, a.QtyReturn Qty
	, a.RetailPrice Price
	, a.CostPrice 
	, a.ABCClass
	, a.MovingCode
	, @ProductType ProductType
	, a.PartCategory
	, @UserID CreatedBy
FROM SpTrnSRturSrvDtl a 
WHERE CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND ReturnNo = @ReturnNo 

-- UPDATE RETURN QTY --
update a
set a.ReturnQty = b.supplyqty
from svTrnInvItem a
inner join #InvoicePart b on b.companycode=a.companycode
	and b.branchcode=a.branchcode
	and b.invoiceno=a.invoiceno 
	and b.partno=a.partno
where producttype=@ProductType

DROP TABLE #InvoicePart

-- SET STATUS HEADER RETUR SERVICE SPAREPART IF PART EXISTS--
IF EXISTS (SELECT a.* FROM SvTrnInvItem a
				WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode
					AND a.InvoiceNo = @InvoiceNo AND a.PartNo NOT IN (SELECT PartNo FROM spTrnSRturSrvHdr z
                					INNER JOIN spTrnSRturSrvDtl y
                						ON z.CompanyCode = y.CompanyCode AND z.BranchCode = y.BranchCode AND z.ReturnNo = y.ReturnNo
                					WHERE y.CompanyCode = @CompanyCode AND y.BranchCode = @BranchCode AND z.InvoiceNo = @InvoiceNo))
BEGIN
	Update spTrnSRturSrvHdr
	Set Status = '1'
	WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND InvoiceNo = @InvoiceNo
END
ELSE 
BEGIN
	Update spTrnSRturSrvHdr
	Set Status = '2'
	WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND InvoiceNo = @InvoiceNo
END              					
END