-- uspfn_GnGetMutationSparepart '6006400109', '', 2014, 2014, 6, 12
CREATE PROCEDURE uspfn_GnGetMutationSparepart
	@BranchCode Varchar(15),
	@TypeOfGoods VARCHAR(2),
	@FromYear INT,
	@ToYear INT,
	@FromMonth INT,
	@ToMonth INT
AS
BEGIN
	SET NOCOUNT ON;
	--DECLARE @BranchCode Varchar(15) = '6006400109'
	--DECLARE @TypeOfGoods INT = ''
	--DECLARE @FromYear INT = 2014
	--DECLARE @ToYear INT = 2014
	--DECLARE @FromMonth INT = 6
	--DECLARE @ToMonth INT = 12

	IF @BranchCode = '' SET @BranchCode = NULL
	IF @TypeOfGoods = '' SET @TypeOfGoods = NULL

	DECLARE @TempTable TABLE (
		BranchCode Varchar(15),
		BranchName Varchar(100),
		Year INT,
		Month INT,
		PartNo varchar(20),
		PartName varchar(100),
		OnHandQty DECIMAL(18,2),
		PurchaseQty DECIMAL(18,2),
		TransferInQty DECIMAL(18,2),
		OrderQty DECIMAL(18,2),
		TransferOutQty DECIMAL(18,2),
		LastStockQty DECIMAL(18,2),
		OnHandAmt DECIMAL(18,0),
		PurchaseAmt DECIMAL(18,0),
		TransferInAmt DECIMAL(18,0),
		OrderAmt DECIMAL(18,0),
		TransferOutAmt DECIMAL(18,0),
		LastStockAmt DECIMAL(18,0),
		Average DECIMAL(18,2)
	)

	SELECT 
		a.BranchCode,
		c.CompanyName,
		a.Year,
		a.Month,
		a.PartNo, 
		b.PartName, 
		a.OnHandQty,
		(a.ReceivingForPurchaseQty + a.ReceivingForNPurchaseQty) AS PurchaseQty,
		a.WTHInQty AS TransferInQty, 
		((a.SalesForCreditQty - a.ReturForSalesCreditQty) + (a.SalesForCashQty - a.ReturForSalesCashQty) + (a.SalesForBPSQty - a.ReturnForBPSQty) + (a.SalesForServiceQty - ReturForServiceQty) + (a.SalesForUnitQty - a.ReturForUnitQty))
			AS OrderQty, 
		a.WTHOutQty AS TransferOutQty,
		(a.OnHandQty + (a.ReceivingForPurchaseQty + a.ReceivingForNPurchaseQty) + a.WTHInQty - ((a.SalesForCreditQty - a.ReturForSalesCreditQty) + (a.SalesForCashQty - a.ReturForSalesCashQty) + (a.SalesForBPSQty - a.ReturnForBPSQty) + (a.SalesForServiceQty - ReturForServiceQty) + (a.SalesForUnitQty - a.ReturForUnitQty)) - a.WTHOutQty) AS [LastStockQty],
		a.OnHandAmt,
		(a.ReceivingForPurchaseAmt + a.ReceivingForNPurchaseAmt) AS PurchaseAmt,
		a.WTHInAmt AS TransferInAmt, 
		((a.SalesForCreditAmt - a.ReturForSalesCreditAmt) + (a.SalesForCashAmt - a.ReturForSalesCashAmt) + (a.SalesForBPSAmt - a.ReturnForBPSAmt) + (a.SalesForServiceAmt - ReturForServiceAmt) + (a.SalesForUnitAmt - a.ReturForUnitAmt)) 
			AS OrderAmt, 
		a.WTHOutAmt AS TransferOutAmt,
		(a.OnHandAmt + (a.ReceivingForPurchaseAmt + a.ReceivingForNPurchaseAmt) + a.WTHInAmt - ((a.SalesForCreditAmt - a.ReturForSalesCreditAmt) + (a.SalesForCashAmt - a.ReturForSalesCashAmt) + (a.SalesForBPSAmt - a.ReturnForBPSAmt) + (a.SalesForServiceAmt - ReturForServiceAmt) + (a.SalesForUnitAmt - a.ReturForUnitAmt)) - a.WTHOutAmt) AS [LastStockAmt],
		dbo.CheckDivided(
		(a.OnHandAmt + (a.ReceivingForPurchaseAmt + a.ReceivingForNPurchaseAmt) + a.WTHInAmt - (a.OnHandQty + (a.ReceivingForPurchaseQty + a.ReceivingForNPurchaseQty) + a.WTHInQty - ((a.SalesForCreditQty - a.ReturForSalesCreditQty) + (a.SalesForCashQty - a.ReturForSalesCashQty) + (a.SalesForBPSQty - a.ReturnForBPSQty) + (a.SalesForServiceQty - ReturForServiceQty) + (a.SalesForUnitQty - a.ReturForUnitQty)) - a.WTHOutQty) - a.WTHOutAmt),
		(a.OnHandQty + (a.ReceivingForPurchaseQty + a.ReceivingForNPurchaseQty) + a.WTHInQty - ((a.SalesForCreditAmt - a.ReturForSalesCreditAmt) + (a.SalesForCashAmt - a.ReturForSalesCashAmt) + (a.SalesForBPSAmt - a.ReturnForBPSAmt) + (a.SalesForServiceAmt - ReturForServiceAmt) + (a.SalesForUnitAmt - a.ReturForUnitAmt)) - a.WTHOutQty))
		AS [Average]
	INTO #TempTransaction
	FROM spHstTransaction a
	LEFT JOIN spMstItemInfo b ON a.CompanyCode = b.CompanyCode AND a.PartNo = b.PartNo
	LEFT JOIN gnMstCoProfile c ON a.BranchCode = c.BranchCode
	WHERE (@BranchCode IS NULL OR a.BranchCode = @BranchCode)
	AND (@TypeOfGoods IS NULL OR a.TypeOfGoods = @TypeOfGoods)
	AND a.Year BETWEEN @FromYear AND @ToYear
	AND (
		a.OnHandQty <> 0 OR 
		(a.ReceivingForPurchaseQty + a.ReceivingForNPurchaseQty) <> 0 OR
		a.WTHInQty <> 0 OR 
		((a.SalesForCreditQty - a.ReturForSalesCreditQty) + (a.SalesForCashQty - a.ReturForSalesCashQty) + (a.SalesForBPSQty - a.ReturnForBPSQty) + (a.SalesForServiceQty - ReturForServiceQty) + (a.SalesForUnitQty - a.ReturForUnitQty)) <> 0 OR
		a.WTHOutQty <> 0 OR
		(a.OnHandQty + (a.ReceivingForPurchaseQty + a.ReceivingForNPurchaseQty) + a.WTHInQty - ((a.SalesForCreditQty - a.ReturForSalesCreditQty) + (a.SalesForCashQty - a.ReturForSalesCashQty) + (a.SalesForBPSQty - a.ReturnForBPSQty) + (a.SalesForServiceQty - ReturForServiceQty) + (a.SalesForUnitQty - a.ReturForUnitQty)) - a.WTHOutQty) <> 0
	)
	AND (
		a.OnHandAmt <> 0 OR 
		(a.ReceivingForPurchaseAmt + a.ReceivingForNPurchaseAmt) <> 0 OR
		a.WTHInAmt <> 0 OR 
		((a.SalesForCreditAmt - a.ReturForSalesCreditAmt) + (a.SalesForCashAmt - a.ReturForSalesCashAmt) + (a.SalesForBPSAmt - a.ReturnForBPSAmt) + (a.SalesForServiceAmt - ReturForServiceAmt) + (a.SalesForUnitAmt - a.ReturForUnitAmt)) <> 0 OR
		a.WTHOutAmt <> 0 OR
		(a.OnHandAmt + (a.ReceivingForPurchaseAmt + a.ReceivingForNPurchaseAmt) + a.WTHInAmt - ((a.SalesForCreditAmt - a.ReturForSalesCreditAmt) + (a.SalesForCashAmt - a.ReturForSalesCashAmt) + (a.SalesForBPSAmt - a.ReturnForBPSAmt) + (a.SalesForServiceAmt - ReturForServiceAmt) + (a.SalesForUnitAmt - a.ReturForUnitAmt)) - a.WTHOutAmt) <> 0
	)
	ORDER BY a.BranchCode, a.PartNo

	IF (@FromYear <> @ToYear)
	BEGIN
		INSERT INTO @TempTable
		SELECT * FROM #TempTransaction a WHERE a.Year = @FromYear AND a.Month BETWEEN @FromMonth AND 12

		INSERT INTO @TempTable
		SELECT * FROM #TempTransaction a WHERE a.Year > @FromYear AND a.Year < @ToYear
	
		INSERT INTO @TempTable
		SELECT * FROM #TempTransaction a WHERE a.Year = @ToYear AND a.Month BETWEEN 1 AND @ToMonth
	END
	ELSE
	BEGIN
		INSERT INTO @TempTable
		SELECT * FROM #TempTransaction a WHERE a.Year = @FromYear AND a.Month BETWEEN @FromMonth AND @ToMonth
	END

	DROP TABLE #TempTransaction
	
	SELECT BranchCode, BranchName, PartNo, PartName,
		SUM(OnHandQty) [OnHandQty], SUM(PurchaseQty) [PurchaseQty], SUM(TransferInQty) [TransferInQty], SUM(OrderQty) [OrderQty], SUM(TransferOutQty) [TransferOutQty], SUM(LastStockQty) [LastStockQty],
		SUM(OnHandAmt) [OnHandAmt], SUM(PurchaseAmt) [PurchaseAmt], SUM(TransferInAmt) [TransferInAmt], SUM(OrderAmt) [OrderAmt], SUM(TransferOutAmt) [TransferOutAmt], SUM(LastStockAmt) [LastStockAmt],
		SUM(Average) [Average]
	FROM @TempTable
	GROUP BY BranchCode, BranchName, PartNo, PartName
	ORDER BY BranchCode, PartNo
END