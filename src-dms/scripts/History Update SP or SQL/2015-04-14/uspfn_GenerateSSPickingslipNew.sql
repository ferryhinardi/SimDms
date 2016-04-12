
ALTER procedure [dbo].[uspfn_GenerateSSPickingslipNew]
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

--set	@CompanyCode	= '6156401000'
--set	@BranchCode		= '6156401001'
--set	@JobOrderNo		= 'SPK/15/001833'
--set	@ProductType	= '4W'
--set	@CustomerCode	= '000003'
--set	@TransType		= '20'
--set	@UserID			= 'TRAININGZZZ'
--set	@DocDate		= '3/12/2015 9:47:01 AM'


--exec uspfn_GenerateSSPickingslipNew '6006400001','6006400101','SPK/14/101589','4W','2105885','20','ga','3/2/2015 4:03:01 PM'
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

DECLARE @CompanyMD AS VARCHAR(15)
DECLARE @BranchMD AS VARCHAR(15)
DECLARE @WarehouseMD AS VARCHAR(15)

SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

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
		AND (a.SupplySlipNo is null OR a.SupplySlipNo = '')
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

select * from #srvorder

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
	, @DocDate OrderDate
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
DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

declare @TempAvailStock as table
(
	PartNo varchar(50),
	AvailStock decimal
)

DECLARE @Query AS VARCHAR(MAX)
--SET @Query = 
--'SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
--FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
--WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+''

--INSERT INTO #TempAvailStock

SET @Query = 
'SElect * into #TempAvailStock from (SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+')#TempAvailStock

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
	''' + @CompanyCode +''' CompanyCode
	, ''' + @BranchCode +''' BranchCode
	, ''' + @TempDocNo +''' DocNo 
	, a.PartNo
	, ''00'' WarehouseCode
	, a.PartNo PartNoOriginal
	, ''' + @JobOrderNo +''' ReferenceNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = ''' + @CompanyCode +''' AND BranchCode = ''' + @BranchCode +'''
		AND ProductType = ''' + @ProductType +''' AND JobOrderNo = ''' + @JobOrderNo +''' ) ReferenceDate
	, (SELECT distinct LocationCode FROM ' + @DbMD +'..SpMstItemLoc WHERE CompanyCode = ''' + @CompanyMD +''' AND BranchCode = ''' + @BranchMD +''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo ) LocationCode
	, a.QtyOrder
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBO
	, 0 QtyBOSupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBOCancel
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice * 10 /100) RetailPriceIncltax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder * a.RetailPrice
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice
		END AS SalesAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice) * a.DiscPct/100)
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) * a.DiscPct/100)
		END AS DiscAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS NetSalesAmt
	, 0 PPNAmt
	,  CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS TotSalesAmt
	, (SELECT distinct MovingCode FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +''' ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +'''  AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''2'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
	, a.QtyOrder FirstDemandQty
FROM #SrvOrder a
WHERE a.TypeOfGoods = '+@TypeOfGoods +'


select top 10 * from spTrnSORDDtl order by createddate desc
--===============================================================================================================================
-- INSERT SO SUPPLY
-- ==============================================================================================================================

SELECT * INTO #TempSOSupply FROM (
SELECT
	'''+ @CompanyCode +''' CompanyCode
	, '''+ @BranchCode +''' BranchCode
	, '''+ @TempDocNo +''' DocNo 
	, 0 SupSeq
	, a.PartNo 
	, a.PartNo PartNoOriginal
	, '''' PickingSlipNo	
	, '''+ @JobOrderNo +''' ReferenceNo
	, '''+ CONVERT(varchar, @DefaultDate )+''' ReferenceDate
	, ''00'' WarehouseCode
	, (SELECT distinct LocationCode FROM '+ @DbMD+'..SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD+''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo) LocationCode
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, 0 QtyPicked
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice *10 /100) RetailPriceIncltax
	, a.RetailPrice
	, b.CostPrice
	, a.DiscPct
	, (SELECT distinct MovingCode FROM '+ @DbMD+'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +'''ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''1'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
FROM #SrvOrder a
inner join spMstItemPrice b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.PartNo = b.PartNo
WHERE a.TypeOfGoods = '+ @TypeOfGoods +'
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
	, LastUpdateBy = '''+ @UserID +'''
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandItem a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+ @TempDocNo +'''

UPDATE SpHstDemandCust
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +''' 
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandCust a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +'''
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +'''
	AND a.PartNo = b.PartNo
	AND a.CustomerCode = '''+ @CustomerCode +'''
	AND b.DocNo = '''+ @TempDocNo +'''

INSERT INTO SpHstDemandItem
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +''' Year
	, '''+ Convert(varchar,Month(GetDate())) +''' Month
	, PartNo
	, 1 DemandFreq
	, QtyOrder DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
 AND NOT EXISTS
( SELECT 1 FROM SpHstDemandItem WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND PartNo = a.PartNo
)

INSERT INTO SpHstDemandCust
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +'''  Year
	, '''+ Convert(varchar,Month(GetDate())) +'''  Month
	, '''+ @CustomerCode +''' CustomerCode
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
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' and a.BranchCode= '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
AND NOT EXISTS
( SELECT PartNo FROM SpHstDemandCust WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +'''  
	AND PartNo = a.PartNo
)

--===============================================================================================================================
-- UPDATE LAST DEMAND DATE MASTER
--===============================================================================================================================

UPDATE '+@DbMD+'..SpMstItems 
SET LastDemandDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+@TempDocNo+'''

--===============================================================================================================================
-- UPDATE STOCK AND MOVEMENT
--===============================================================================================================================

UPDATE '+@DbMD+'..spMstItems
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo

UPDATE '+@DbMD+'..spMstItemloc
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItemLoc a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD +'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.WarehouseCode = '''+@WarehouseMD+'''
	AND a.PartNo = b.PartNo

INSERT INTO SpTrnIMovement
SELECT
	'''+@CompanyCode +''' CompanyCode
	, '''+@BranchCode +''' BranchCode
	, a.DocNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyCode +'''
		AND BranchCode = '''+ @BranchCode +''' AND DocNo = a.DocNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),'''+convert(varchar,getdate())+''') CreatedDate 
	, ''00'' WarehouseCode
	, (SELECT LocationCode FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode =  '''+@CompanyCode +'''
		AND BranchCode = '''+@BranchCode +''' AND DocNo = '''+@TempDocNo +''' AND PartNo = a.PartNo)
	  LocationCode
	, a.PartNo
	, ''OUT'' SignCode
	, ''SA-NPJUAL'' SubSignCode
	, a.QtySupply
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, '''+@UserID +''' CreatedBy
FROM #TempSOSupply a

--===============================================================================================================================
-- UPDATE SUPPLY SLIP TO SPK
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = '''+@CompanyCode +''' AND BranchCode = '''+@BranchCode+'''
		AND ProductType = '''+@ProductType +''' AND JobOrderNo = '''+@JobOrderNo+''')

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
	AND a.CompanyCode = '''+@CompanyCode+'''
	AND a.BranchCode = '''+@BranchCode+'''
	AND a.ProductType = '''+@ProductType+'''
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+@ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND (a.SupplySlipNo = '''' OR a.SupplySlipNo IS NULL)
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
	AND a.CompanyCode = '''+ @CompanyCode +''' 
	AND a.BranchCode = '''+ @BranchCode +''' 
	AND a.ProductType = '''+ @ProductType +'''  
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+ @ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo) 
	AND (a.SupplySlipNo != '''' OR a.SupplySlipNo IS NOT NULL)
) #TempServiceItemIns


UPDATE svTrnSrvItem
SET SupplySlipNo = b.DocNo
	, SupplySlipDate = ISNULL((SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
							AND DocNo = b.DocNo),'''+Convert(varchar,@DefaultDate)+''')
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
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, a.DiscPct
FROM #TempServiceItemIns a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = '''+ @CompanyCode +'''
	AND a.BranchCode = '''+ @BranchCode +'''
	AND a.ProductType = '''+ @ProductType+'''


--===============================================================================================================================
DROP TABLE #TempServiceItem 
DROP TABLE #TempServiceItemIns
DROP TABLE #TempSOSupply'

EXEC(@query)

--select convert(xml,@query)


--===============================================================================================================================
-- INSERT SVSDMOVEMENT
--===============================================================================================================================

declare @md bit
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

if(@md = 0)
begin

 declare @QueryTemp as varchar(max)  
 
	set @Query ='insert into '+ @DbMD +'..svSDMovement
	select a.CompanyCode, a.BranchCode, a.DocNo, a.CreatedDate, a.PartNo
	, Seq = convert(integer, ROW_NUMBER() OVER (PARTITION BY a.ReferenceNo ORDER BY a.DocNo)) ,
	a.WarehouseCode, a.QtyOrder, a.QtySupply, a.DiscPct
	,isnull(((select RetailPrice from spTrnSORDDtl
			where CompanyCode = ''' + @CompanyCode + '''  and BranchCode = ''' + @BranchCode  + '''
			and DocNo = ''' + @TempDocNo + ''' and PartNo = a.PartNo) / 1.1 * 
			((100 - isnull((select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
				where CompanyCode = ''' + @CompanyCode + ''' and BranchCode = ''' + @BranchCode  + ''' and SupplierCode = dbo.GetBranchMD(''' + @CompanyCode + ''', ''' + @BranchCode  + ''') 
				and ProfitCenterCode = ''300''),0)) * 0.01)),0) CostPrice
	, a.RetailPrice, b.TypeOfGoods
	, '''+ @CompanyMD +''','''+ @BranchMD +''','''+ @WarehouseMD +''', p.RetailPriceInclTax, p.RetailPrice, p.CostPrice
	,''x'','''+ @producttype +''',''300'',''0'',''0'','''+ convert(varchar,GETDATE()) +''','''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	,'''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	from spTrnSORDDtl a 
	join spTrnSORDHdr b on a.CompanyCode = b.CompanyCode
	and a.BranchCode = b.BranchCode 
	and a.DocNo = b.DocNo
	join spmstitemprice p
	on p.PartNo = a.PartNo
	where p.CompanyCode = '''+ @CompanyCode +'''
	and p.branchcode = '''+ @BranchCode +'''
	and a.ReferenceNo = '''+ @JobOrderNo +''''+
	' and a.DocNo = ''' + @TempDocNo + '''';

	exec (@Query)
	print (@QUERY)

end

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

--rollback tran
END
GO

