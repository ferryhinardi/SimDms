if object_id('usprpt_PmRpInqSummaryWeb') is not null
	drop procedure usprpt_PmRpInqSummaryWeb
GO
CREATE procedure [dbo].[usprpt_PmRpInqSummaryWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@PeriodBegin		DATETIME,
	@PeriodEnd			DATETIME,
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@Salesman			VARCHAR(15),
	@Jns				VARCHAR(1)
	
)
AS 
BEGIN
SET NOCOUNT ON;
IF @Jns = '1'
BEGIN
	SELECT * INTO #deptSales FROM(
		SELECT 
				'4' idx,
			   a.EmployeeID,
			   a.Position,
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.StatusProspek <> '20' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) NEW,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'P' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'HP' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'SPK' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) SPK,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'DO' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) DO,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'DELIVERY' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'LOST' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) LOST
		FROM HrEmployee a WHERE a.Department = 'SALES' AND a.CompanyCode = @CompanyCode
	)#deptSales

	--Sales Coordinator
	SELECT * INTO #qryS_SC FROM(
		SELECT 
				'3' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) LOST
		FROM #deptSales a WHERE a.Position = 'SC'
	)#qryS_SC

	--Sales Head
	SELECT * INTO #qrySH FROM(
		SELECT 
				'2' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
			   (SELECT ISNULL(SUM(NEW), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') NEW,
			   (SELECT ISNULL(SUM(PROSPECT), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') PROSPECT,
			   (SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') HOTPROSPECT,
			   (SELECT ISNULL(SUM(SPK), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') SPK,
			   (SELECT ISNULL(SUM(DO), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DO,
			   (SELECT ISNULL(SUM(DELIVERY), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DELIVERY,
			   (SELECT ISNULL(SUM(LOST), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') LOST
		FROM #deptSales a WHERE a.Position = 'SH'
	)#qrySH

	IF(@SalesHead = '' AND @Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM FROM(
				SELECT 
					'1' idx,
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSales a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM

			SELECT * INTO #qryAll FROM(
				SELECT * FROM #qryBM
				UNION
				SELECT * FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM)     
				UNION
				SELECT * FROM #qryS_SC WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM)) 
				UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN (SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM))) 
			)#qryAll

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAll a ORDER BY a.idx ASC
		
			DROP TABLE #qryAll
			DROP TABLE #qryBM
		END
	ELSE IF(@Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM2 FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSales a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM2

			SELECT * INTO #qryAll2 FROM(
				SELECT * FROM #qryBM2
				UNION
				SELECT * FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead     
				UNION
				SELECT * FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead) 
				UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead )) 
			)#qryAll2

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAll2 a ORDER BY a.idx ASC

			DROP TABLE #qryAll2
			DROP TABLE #qryBM2
		END
	ELSE
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM3 FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSales a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM3

			SELECT * INTO #qryAll3 FROM(
				SELECT * FROM #qryBM3
				UNION
				SELECT * FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead     
				UNION
				SELECT * FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead) 
				UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead ))
				AND EmployeeID = @Salesman 
			)#qryAll3

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST 
			FROM #qryAll3 a ORDER BY a.idx ASC 

			DROP TABLE #qryAll3
			DROP TABLE #qryBM3
		END
		DROP TABLE #deptSales
		DROP TABLE #qryS_SC
		DROP TABLE #qrySH
	END
ELSE IF @Jns = '2'
	BEGIN
		SELECT * INTO #dByTipe FROM(
			SELECT b.EmployeeID, (b.TipeKendaraan + ' ' + b.Variant) ModelKendaraan, LastProgress, StatusProspek FROM PmKdp b 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd)
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.ModelKendaraan,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.StatusProspek <> '20' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) LOST
			FROM #dByTipe a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.ModelKendaraan
		)#dSls

		--Kondisi SH = '' AND S = ''
		IF (@SalesHead = '' AND @Salesman = '')
		BEGIN
			SELECT * INTO #dt27_1 FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.ModelKendaraan
			)#dt27_1

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_1 ORDER BY ModelKendaraan

			DROP TABLE #dt27_1
		END
		--Kondisi S = ''
		ELSE IF (@Salesman = '')
		BEGIN
			SELECT * INTO #dt27_2 FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.ModelKendaraan
			)#dt27_2

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_2 ORDER BY ModelKendaraan

			DROP TABLE #dt27_2
		END
		ELSE
			SELECT * INTO #dt27_3 FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.ModelKendaraan
			)#dt27_3

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_3 ORDER BY ModelKendaraan

			DROP TABLE #dt27_3

		DROP TABLE #dSls
		DROP TABLE #dByTipe
	END
ELSE IF @Jns = '3'
	BEGIN
		SELECT * INTO #dByTipe2 FROM(
			SELECT b.EmployeeID, b.PerolehanData, LastProgress, StatusProspek FROM PmKdp b 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd)
		)#dByTipe2

		SELECT * INTO #dSls2 FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.PerolehanData,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.StatusProspek <> '20' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) LOST
			FROM #dByTipe2 a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.PerolehanData
		)#dSls2

		--Kondisi SH = '' AND S = ''
		IF (@SalesHead = '' AND @Salesman = '')
		BEGIN
			SELECT * INTO #dt37_1 FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSls2 a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.PerolehanData
			)#dt37_1

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_1 ORDER BY PerolehanData

			DROP TABLE #dt37_1
		END
		--Kondisi S = ''
		ELSE IF (@Salesman = '')
		BEGIN
			SELECT * INTO #dt37_2 FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2 a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.PerolehanData
			)#dt37_2

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_2 ORDER BY PerolehanData

			DROP TABLE #dt37_2
		END
		ELSE
			SELECT * INTO #dt37_3 FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.PerolehanData
			)#dt37_3

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_3 ORDER BY PerolehanData

			DROP TABLE #dt37_3

		DROP TABLE #dSls2
		DROP TABLE #dByTipe2
	END
END
GO

if object_id('usprpt_PmRpInqOutStanding_NewByData') is not null
	drop procedure usprpt_PmRpInqOutStanding_NewByData
GO
-- =============================================
-- Author:		RIJAL AL
-- Create date: <Create Date,,>
-- Description:	INQUIRY OUT STANDING PROSPECT
-- =============================================
-- [[usprpt_PmRpInqOutStanding_NewByData]] '6043401', '604340101', '2010-11-01 14:33:00.000', '', 'ALL', 'ALL', 'ALL', 'ALL'

CREATE procedure [dbo].[usprpt_PmRpInqOutStanding_NewByData] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@Period				DATETIME,
	@COO				VARCHAR(15),
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@SalesCoordinator	VARCHAR(15),
	@Salesman			VARCHAR(15)
	
)
AS 
BEGIN
SET NOCOUNT ON;
SELECT * INTO #dByTipe FROM(
			SELECT b.EmployeeID, b.PerolehanData, LastProgress, StatusProspek FROM PmKdp b 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.PerolehanData,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) SPK
			FROM #dByTipe a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.PerolehanData
		)#dSls

		IF @COO = ''
		BEGIN
		IF @SalesHead = '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData
		END
		ELSE IF @SalesHead <> '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData
		END
		ELSE IF @SalesHead <> '' AND @SalesCoordinator <> '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader = @SalesCoordinator
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData	
		END
		ELSE IF @SalesHead <> '' AND @SalesCoordinator <> '' AND @Salesman <> ''
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.EmployeeID = @Salesman  
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData

		DROP TABLE #dSls
		DROP TABLE #dByTipe
		END
END
GO
if object_id('usprpt_PmRpInqOutStanding_NewBySalesman') is not null
	drop procedure usprpt_PmRpInqOutStanding_NewBySalesman
GO
-- =============================================
-- Author:		RIJAL AL
-- Create date: <Create Date,,>
-- Description:	INQUIRY OUT STANDING PROSPECT
-- =============================================
-- usprpt_PmRpInqOutStanding_NewBySalesman '6043401', '604340101', '2010-11-01 14:33:00.000', '', 'ALL', 'ALL', 'ALL', 'ALL'

CREATE procedure [dbo].[usprpt_PmRpInqOutStanding_NewBySalesman] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@Period				DATETIME,
	@COO				VARCHAR(15),
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@SalesCoordinator	VARCHAR(15),
	@Salesman			VARCHAR(15)
	
)
AS 
BEGIN
SET NOCOUNT ON;

-- TABLE INITIAL
--===============================================================================================================================
	SELECT * INTO #employee_stat_SM FROM(
		SELECT 
			a.CompanyCode,
			a.EmployeeID,
			a.EmployeeName,
			a.Position,
			'Salesman' PositionName, 
			a.TeamLeader,
			(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastProgress = 'P' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) PROSPECT,
			(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastProgress = 'HP' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) HOTPROSPECT,
			(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastProgress = 'SPK' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) SPK
		FROM HrEmployee a
		WHERE a.CompanyCode = @CompanyCode  AND a.Department = 'SALES'
			AND a.TeamLeader IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @BranchManager))
	)#employee_stat_SM

	SELECT * INTO #employee_stat_SK FROM(
		SELECT
			a.CompanyCode,
			a.TeamLeader EmployeeID,
			b.EmployeeName,
			'SC' Position,
			'Sales Coordinator' PositionName, 
			b.TeamLeader ShEmployeeID,
			ISNULL(SUM(a.PROSPECT),0) PROSPECT,
			ISNULL(SUM(a.HOTPROSPECT),0) HOTPROSPECT,
			ISNULL(SUM(a.SPK),0) SPK
		FROM #employee_stat_SM a
		LEFT JOIN HrEmployee b
			ON b.CompanyCode = a.CompanyCode AND a.TeamLeader = b.EmployeeID
		WHERE b.TeamLeader IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader IN (SELECT EmployeeID FROM HrEmployee WHERE EmployeeID = @BranchManager))
		GROUP BY a.CompanyCode,
			b.EmployeeName,
			a.TeamLeader, b.TeamLeader
	)#employee_stat_SK

	SELECT * INTO #employee_stat_SH FROM(
		SELECT
			a.CompanyCode,
			a.ShEmployeeID EmployeeID,
			b.EmployeeName,
			'SH' PositionID,
			'Sales Head' PositionName, 
			b.TeamLeader BMEmployeeID,
			ISNULL(SUM(a.PROSPECT),0) PROSPECT,
			ISNULL(SUM(a.HOTPROSPECT),0) HOTPROSPECT,
			ISNULL(SUM(a.SPK),0) SPK
		FROM #employee_stat_SK a
		LEFT JOIN HrEmployee b
			ON b.CompanyCode = a.CompanyCode AND a.ShEmployeeID = b.EmployeeID
		WHERE b.TeamLeader = @BranchManager
		GROUP BY a.CompanyCode,
			b.EmployeeName,
			a.ShEmployeeID, b.TeamLeader
	)#employee_stat_SH

	SELECT * INTO #employee_stat_BM FROM(
		SELECT
			a.CompanyCode,
			a.BMEmployeeID EmployeeID,
			b.EmployeeName,
			'BM' PositionID,
			'Branch Manager' PositionName, 
			'' TeamLeader,
			ISNULL(SUM(a.PROSPECT),0) PROSPECT,
			ISNULL(SUM(a.HOTPROSPECT),0) HOTPROSPECT,
			ISNULL(SUM(a.SPK),0) SPK
		FROM #employee_stat_SH a
		LEFT JOIN HrEmployee b
			ON b.CompanyCode = a.CompanyCode AND a.BMEmployeeID = b.EmployeeID
		WHERE a.BMEmployeeID = @BranchManager
		GROUP BY a.CompanyCode,
			b.EmployeeName,
			a.BMEmployeeID
	)#employee_stat_BM

SELECT * INTO #employee_stat FROM(
	SELECT '4' PositionId, a.* FROM #employee_stat_SM a
	UNION
	SELECT '3' PositionId, a.* FROM #employee_stat_SK a
	UNION
	SELECT '2' PositionId, a.* FROM #employee_stat_SH a
	UNION
	SELECT '1' PositionId, a.* FROM #employee_stat_BM a
	) #employee_stat

	DROP TABLE #employee_stat_SM
	DROP TABLE #employee_stat_SK
	DROP TABLE #employee_stat_SH
	DROP TABLE #employee_stat_BM

IF @COO = ''
	BEGIN
-- == CASE I ==
		IF @SalesHead = '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN		
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode
		END
-- == CASE II ==
		ELSE IF @SalesHead <> '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @BranchManager
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesHead
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @SalesHead)
			UNION			
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @SalesHead))
		END
-- == CASE III ==
		ELSE IF @SalesHead <> '' AND @SalesCoordinator <> '' AND @Salesman = ''
		BEGIN
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @BranchManager
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesHead
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesCoordinator
			UNION			
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @SalesCoordinator)
		END
-- == CASE IV ==
		ELSE IF @SalesHead <> '' AND @SalesCoordinator <> '' AND @Salesman <> ''
		BEGIN
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @BranchManager
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesHead
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesCoordinator
			UNION			
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @Salesman
		END
	END
END
GO


if object_id('usprpt_PmRpInqOutStanding_NewByType') is not null
	drop procedure usprpt_PmRpInqOutStanding_NewByType
GO
-- =============================================
-- Author:		RIJAL AL
-- Create date: <Create Date,,>
-- Description:	INQUIRY OUT STANDING PROSPECT
-- =============================================
-- [usprpt_PmRpInqOutStanding_NewByType] '6043401', '604340101', '2010-11-01 14:33:00.000', '', 'ALL', 'ALL', 'ALL', 'ALL'

CREATE procedure [dbo].[usprpt_PmRpInqOutStanding_NewByType] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@Period				DATETIME,
	@COO				VARCHAR(15),
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@SalesCoordinator	VARCHAR(15),
	@Salesman			VARCHAR(15)
	
)
AS 
BEGIN
SET NOCOUNT ON;
SELECT * INTO #dByTipe FROM(
			SELECT b.EmployeeID, (b.TipeKendaraan + ' ' + b.Variant) ModelKendaraan, LastProgress, StatusProspek FROM PmKdp b 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.ModelKendaraan,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) SPK
			FROM #dByTipe a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.ModelKendaraan
		)#dSls

		IF @COO = ''
		BEGIN
		IF @SalesHead = '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.ModelKendaraan,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))  
				AND a.ModelKendaraan <> ''
				GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan
		END
		ELSE IF @SalesHead <> '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.ModelKendaraan,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)
					AND a.ModelKendaraan <> ''
				GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan	
		END
		ELSE IF @SalesHead <> '' AND @SalesCoordinator <> '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.ModelKendaraan,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader = @SalesCoordinator
					AND a.ModelKendaraan <> ''
				GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan	
		END
		ELSE IF @SalesHead <> '' AND @SalesCoordinator <> '' AND @Salesman <> ''
			SELECT
					a.EmployeeID,
					a.Position,
					a.ModelKendaraan,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.EmployeeID = @Salesman
					AND a.ModelKendaraan <> ''   
				GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan

		DROP TABLE #dSls
		DROP TABLE #dByTipe
		END
END
GO


if object_id('usprpt_PmRpInqOutStanding_NewPrint') is not null
	drop procedure usprpt_PmRpInqOutStanding_NewPrint
GO
-- =============================================
-- Author:		RIJAL AL
-- Create date: <Create Date,,>
-- Description:	INQUIRY OUT STANDING PROSPECT
-- =============================================
-- [[usprpt_PmRpInqOutStanding_NewPrint]] '6043401', '604340101', '2010-11-01 14:33:00.000', '', 'ALL', 'ALL', 'ALL', 'ALL'

CREATE procedure [dbo].[usprpt_PmRpInqOutStanding_NewPrint] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@Period				DATETIME,
	@COO				VARCHAR(15),
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@SalesCoordinator	VARCHAR(15),
	@Salesman			VARCHAR(15)
	
)
AS 
BEGIN
SET NOCOUNT ON;

	EXEC usprpt_PmRpInqOutStanding_NewBySalesman @CompanyCode, @BranchCode, @Period, @COO, @BranchManager, @SalesHead, @SalesCoordinator, @Salesman

	EXEC usprpt_PmRpInqOutStanding_NewByType @CompanyCode, @BranchCode, @Period, @COO, @BranchManager, @SalesHead, @SalesCoordinator, @Salesman

	EXEC usprpt_PmRpInqOutStanding_NewByData @CompanyCode, @BranchCode, @Period, @COO, @BranchManager, @SalesHead, @SalesCoordinator, @Salesman

END
GO


if object_id('uspfn_SvInqFpjStdData') is not null
	drop procedure uspfn_SvInqFpjStdData
GO
CREATE procedure [dbo].[uspfn_SvInqFpjStdData]    
		 @CompanyCode nvarchar(20),    
		 @BranchCode nvarchar(20),    
		 @ProductType nvarchar(2),
		 @FPJNo nvarchar(20),
		 @InvoiceNoStart nvarchar(20),
		 @InvoiceNoEnd nvarchar(20)
		
    
as    

--set @CompanyCode = '6006406'
--set @BranchCode = '6006402'
--set @ProductType = '4W'
--set @FPJNo = 'FPS/13/002075'
--set @InvoiceNoStart = 'INF/13/000472'
--set @InvoiceNoEnd = 'INF/13/000472'
    
declare	 @InvoiceStatus char(1)
set @InvoiceStatus = 0
if(@FPJNo != '')
Set @InvoiceStatus = 1

select	row_number() over(order by inv.InvoiceNo) RowNum
                        , convert(bit, 1) IsSelected
						, inv.CompanyCode
						, inv.BranchCode
                        , inv.ProductType
						, inv.InvoiceNo
		                , inv.InvoiceDate
		                , inv.JobOrderNo
		                , inv.JobOrderDate
		                , inv.TotalDPPAmt
		                , inv.TotalPPHAmt + TotalPPNAmt as TotalPpnAmt
		                , inv.TotalSrvAmt
		                , inv.JobType
		                , inv.PoliceRegNo
		                , inv.BasicModel
		                , inv.ChassisCode
		                , inv.ChassisNo
		                , inv.EngineCode
		                , inv.EngineNo
                        , inv.TOPCode
                        , inv.TOPDays
                        , inv.DueDate
                        , inv.FPJNo
                        , inv.FPJDate
                        , inv.CustomerCodeBill
                        , inv.Odometer
                        , inv.IsPkp
                        , inv.CustomerCode
		                , inv.CustomerCode + ' - ' + cust.CustomerName Pelanggan
		                , inv.CustomerCodeBill + ' - ' + custBill.CustomerName Pembayar
                        , inv.DueDate
                from	svTrnInvoice inv with(nolock, nowait)
		                left join gnMstCustomer cust with(nolock, nowait) on inv.CompanyCode = cust.CompanyCode
			                and inv.CustomerCode = cust.CustomerCode
		                left join gnMstCustomer custBill with(nolock, nowait) on inv.CompanyCode = custBill.CompanyCode
			                and inv.CustomerCodeBill = custBill.CustomerCode
                where	inv.CompanyCode = @CompanyCode 
		                and inv.BranchCode = @BranchCode 
		                and inv.ProductType = @ProductType
		                and inv.InvoiceNo between @InvoiceNoStart and @InvoiceNoEnd
		                and inv.CustomerCodeBill = isnull((select top 1 CustomerCodeBill from svTrnInvoice where InvoiceNo = @InvoiceNoStart),'')
                       and (CASE WHEN inv.InvoiceStatus = 0 THEN @InvoiceStatus ELSE inv.InvoiceStatus END) = inv.InvoiceStatus
                        and inv.FPJNo = @FPJNo
                        and (inv.InvoiceNo like 'INW%' or inv.InvoiceNo like 'INF%')
GO

if object_id('uspfn_spSelectPickingSlipAfterLmp') is not null
	drop procedure uspfn_spSelectPickingSlipAfterLmp
GO
CREATE procedure [dbo].[uspfn_spSelectPickingSlipAfterLmp] 
--declare 
@CompanyCode varchar(15), @BranchCode varchar(15), 
		@ProductType varchar(4), @JobOrderNo varchar(25)
as
--set @CompanyCode = '6159401000'
--set @BranchCode = '6159401001'
--set @ProductType = '4W'
--set @JobOrderNo = 'SPK/15/002385'
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
		--AND b.SupplySlipNo <> ''
		--AND b.PartSeq = (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			  -- AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = b.PartNo)
		--AND d.Status <= 2
)#t1
SELECT Row_number() OVER(ORDER BY DocNo) No, * FROM #t1
DROP TABLE #t1
GO
if object_id('uspfn_spGetUtlPORDDdtl') is not null
	drop procedure uspfn_spGetUtlPORDDdtl
GO
Create procedure [dbo].[uspfn_spGetUtlPORDDdtl]   
(  
--DECLARE
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @CustomerCode varchar(15),
 @OrderNo varchar(20) 

--set @CompanyCode   = 6006400001
--set @BranchCode  = 6006400000
--set @CustomerCode ='0000028'
--set @OrderNo ='SOC/14/100143'

)  
AS  

begin

declare @md bit
declare @dbMD varchar(25)
declare @CompanyMD varchar(25)

set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

if(@md = 1)
BEGIN

SELECT  row_number () OVER (ORDER BY a.CreatedDate ASC) AS No,
	                b.DealerCode,
	                a.PartNo,
	                c.PartName,                    
	                a.Qty AS QtyOrder,
                    0.00 as QtySupply,
                    0.00 as QtyBO,
                    e.RetailPrice,
	                (e.RetailPrice * a.Qty) AS SalesAmt,
	                0.00 AS DiscPct,
                    0.00 AS NetSalesAmt
                FROM
	                spUtlPORDDDtl AS a with(nolock, nowait) , 
					spUtlPORDDHDR AS b with(nolock, nowait) ,
	                spMstItemInfo AS c with(nolock, nowait) ,
					gnMstCompanyMapping AS d with(nolock, nowait),
	                spMstItemPrice AS e with(nolock, nowait)
                WHERE
                    a.CompanyCode=b.CompanyCode AND
					a.BranchCode=b.BranchCode AND					
					a.OrderNo=b.OrderNo AND
	                a.CompanyCode = c.CompanyCode AND
	                a.PartNo = c.PartNo AND
	                a.CompanyCode = d.CompanyCode AND
	                a.BranchCode = d.BranchCode AND
					d.CompanyMD = e.CompanyCode AND
	                a.PartNo = e.PartNo AND
	                a.CompanyCode = @CompanyCode AND
	                a.BranchCode = @BranchCode AND
	                b.DealerCode = @CustomerCode AND
	                a.OrderNo = @OrderNo
                ORDER BY a.PartNo
END
ELSE
BEGIN

set @dbMD = (select DISTINCT dbmd from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)

--set @CompanyMD = (select DISTINCT CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
--select @CompanyMD
declare @query varchar(max)

if(isnull(cast(@dbMD as varchar(max)),'') = '')
BEGIN
SELECT  row_number () OVER (ORDER BY a.CreatedDate ASC) AS No,
	                b.DealerCode,
	                a.PartNo,
	                c.PartName,                    
	                a.Qty AS QtyOrder,
                    0.00 as QtySupply,
                    0.00 as QtyBO,
                    d.RetailPrice,
	                (d.RetailPrice * a.Qty) AS SalesAmt,
	                0.00 AS DiscPct,
                    0.00 AS NetSalesAmt
                FROM
	                spUtlPORDDDtl AS a with(nolock, nowait) , 
					spUtlPORDDHDR AS b with(nolock, nowait) ,
	                spMstItemInfo AS c with(nolock, nowait) ,
	                spMstItemPrice AS d with(nolock, nowait) 
                WHERE
                    a.CompanyCode=b.CompanyCode AND
					a.BranchCode=b.BranchCode AND					
					a.OrderNo=b.OrderNo AND
	                a.CompanyCode = c.CompanyCode AND
	                a.PartNo = c.PartNo AND
	                a.CompanyCode = d.CompanyCode AND
	                a.BranchCode = d.BranchCode AND
	                a.PartNo = d.PartNo AND
	                a.CompanyCode = @CompanyCode AND
	                a.BranchCode = @BranchCode AND
	                b.DealerCode = @CustomerCode AND
	                a.OrderNo = @OrderNo
                ORDER BY a.PartNo
END
ELSE
BEGIN

set @query = '
SELECT  row_number () OVER (ORDER BY a.CreatedDate ASC) AS No,
	                b.DealerCode,
	                a.PartNo,
	                c.PartName,                    
	                a.Qty AS QtyOrder,
                    0.00 as QtySupply,
                    0.00 as QtyBO,
                    e.RetailPrice,
	                (e.RetailPrice * a.Qty) AS SalesAmt,
	                0.00 AS DiscPct,
                    0.00 AS NetSalesAmt
                FROM
	                spUtlPORDDDtl AS a with(nolock, nowait) , 
					spUtlPORDDHDR AS b with(nolock, nowait) ,
	                spMstItemInfo AS c with(nolock, nowait) ,
					gnMstCompanyMapping AS d with(nolock, nowait),
	                '+@dbMD+'..spMstItemPrice AS e with(nolock, nowait)
                WHERE
                    a.CompanyCode=b.CompanyCode AND
					a.BranchCode=b.BranchCode AND					
					a.OrderNo=b.OrderNo AND
	                a.CompanyCode = c.CompanyCode AND
	                a.PartNo = c.PartNo AND
	                a.CompanyCode = d.CompanyCode AND
	                a.BranchCode = d.BranchCode AND
					d.CompanyMD = e.CompanyCode AND
	                a.PartNo = e.PartNo AND
	                a.CompanyCode = '+@CompanyCode+' AND
	                a.BranchCode = '+@BranchCode +'AND
	                b.DealerCode = '''+@CustomerCode +'''AND
	                a.OrderNo = '''+@OrderNo+'''
                ORDER BY a.PartNo'
EXEC(@query)
print(@query)
				
		END	 
	END
end	
GO

if object_id('uspfn_SalesModelCode4PO') is not null
	drop procedure uspfn_SalesModelCode4PO
GO
CREATE procedure [dbo].[uspfn_SalesModelCode4PO] 
 @DB varchar(50),
 @CompanyCode varchar(50),
 @BranchCode varchar(50),
 @SupplierCode varchar(50)

 as

 declare @sqlstr varchar(max)

 set @sqlstr = 'select a.SalesModelCode, b.SalesModelDesc 
from omMstPricelistBuy a
inner join  '+@DB+'.dbo.omMstModel b
	on a.SalesModelCode = b.SalesModelCode
where a.CompanyCode = '''+ @CompanyCode +'''
 and a.BranchCode = '''+ @BranchCode +'''
 and a.SupplierCode = '''+ @SupplierCode +'''
 and b.Status = ''1'''

 --select @sqlstr
 exec (@sqlstr)
 GO

if object_id('uspfn_omSoLkp') is not null
	drop procedure uspfn_omSoLkp
GO
create procedure uspfn_omSoLkp 
(
	@CompanyCode varchar(25),
	@BranchCode varchar(25)
)
as
 
 -- exec uspfn_omSoLkp '6159401000','6159401001'
 
 declare @DbMD as varchar(15)  
 declare @Sql as varchar(max) 
 declare @ssql as varchar(max) 
 
 set @DbMD = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 


 set @ssql='select * from gnMstCompanyMapping '

set @Sql= 'SELECT a.CompanyCode, a.BranchCode,
                a.SONo, a.SODate, a.SalesType, a.CustomerCode, a.TOPCode, a.Installment, a.FinalPaymentDate,
                a.TOPDays, a.BillTo, a.ShipTo, a.ProspectNo, a.SKPKNo, a.Salesman, a.WareHouseCode, a.isLeasing, 
                a.LeasingCo, a.GroupPriceCode, a.Insurance, a.PaymentType, a.PrePaymentAmt, a.PrePaymentBy, 
                a.CommissionBy, a.CommissionAmt, a.PONo, a.ContractNo, a.Remark, a.Status,
                a.SalesCoordinator, a.SalesHead, a.BranchManager, a.RefferenceNo,
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then '''' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDates, 
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDate, 
                CASE convert(varchar, a.RequestDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RequestDate, 111) end as RequestDate,
                CASE convert(varchar, a.PrePaymentDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.PrePaymentDate, 111) end as PrePaymentDate,
                e.Address1 + '' '' + e.Address2 + '' '' + e.Address3 + '' '' + e.Address4 as Address,
                case when year(a.RefferenceDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC1,
                case when a.SKPKNo <> '''' then convert(bit,1) else convert(bit,0) end isC2,
                case when year(a.PrePaymentDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC3,
                case when year(a.RequestDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC4,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS CustomerName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID) as SalesmanName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.Shipto = b.CustomerCode)  
						AS ShipName,
                (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.LeasingCo = b.CustomerCode)  
						AS LeasingCoName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.PrePaymentby = c.EmployeeID) as PrePaymentName,
				(SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode) AS GroupPriceName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS BillName,
				(SELECT b.lookupvaluename
                        FROM '+@DbMD+'..gnMstLookUpDtl b
                        WHERE a.WareHouseCode = b.LookUpValue
						AND a.WareHouseCode = b.LookUpValue and CodeID =''MPWH'')  
						AS WareHouseName,
                (a.CustomerCode
                    + '' || ''
                    + (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode))  
						AS Customer, 
                (a.Salesman
                    + '' || ''
                    + (SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID))  AS Sales, 
                (a.GroupPriceCode
                    + '' || ''
                    + (SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode))  AS GroupPrice, 
                CASE a.Status when 0 then ''OPEN''
                                when 1 then ''PRINTED''
                                when 2 then ''APPROVED''
                                when 3 then ''DELETED''
                                when 4 then ''REJECTED''
                                when 9 then ''FINISHED'' END as Stat
                , CASE ISNULL(a.SalesType, 0) WHEN 0 THEN ''Wholesales'' ELSE ''Direct'' END AS TypeSales
                ,(select distinct x.TipeKendaraan
                    from pmKDP x
	                    left join gnMstEmployee b on x.CompanyCode=b.CompanyCode and x.BranchCode=b.BranchCode
		                    and x.EmployeeID=b.EmployeeID
	                    left join omTrSalesSO c on c.CompanyCode = x.CompanyCode 
		                    and c.BranchCode = x.BranchCode
		                    and c.ProspectNo = x.InquiryNumber
	                    left join omTrSalesInvoice d on d.CompanyCode = x.CompanyCode
		                    and d.BranchCode = x.BranchCode
		                    and d.SONo = c.SONo
	                    left join omTrSalesReturn e on e.CompanyCode = x.CompanyCode
		                    and e.BranchCode = x.BranchCode
		                    and e.InvoiceNo = d.InvoiceNo
                    where x.InquiryNumber=a.ProspectNo) as VehicleType
                FROM omTrSalesSO a
                INNER JOIN gnMstCustomer e
                ON a.CompanyCode = e.CompanyCode AND a.CustomerCode = e.CustomerCode'

--print @Sql

exec (@Sql)
GO

if object_id('uspfn_GetPINVDData') is not null
	drop procedure uspfn_GetPINVDData
GO
CREATE procedure [dbo].[uspfn_GetPINVDData]
 @CompanyCode varchar(15),
 @BranchCode varchar(15),
 @DNNo varchar(15),
 @SupplierCode varchar(15),
 @TypeOfGoods varchar(2),
 @PPN bit
as 
begin
SELECT  row_number () OVER (ORDER BY a.CreatedDate) AS NoUrut,
a.CompanyCode,
a.BranchCode,
a.OrderNo AS DocNo,
a.DeliveryNo,
(
  SELECT TOP 1 DeliveryDate 
    FROM spUtlPINVDHdr d
  WHERE d.CompanyCode=a.CompanyCode AND
    d.BranchCode=a.BranchCode AND
    d.DealerCode=a.DealerCode AND
    d.DeliveryNo=a.DeliveryNo AND
	d.TypeOfGoods = a.typeOfGoods
) AS DeliveryDate,
(
  SELECT TOP 1 d.InvoiceNo 
    FROM spUtlPINVDHdr d
  WHERE d.CompanyCode=a.CompanyCode AND
    d.BranchCode=a.BranchCode AND
    d.DealerCode=a.DealerCode AND
    d.DeliveryNo=a.DeliveryNo AND
	d.TypeOfGoods = a.TypeOfGoods
) AS InvoiceNo,
a.PartNoShip as PartNo,
(
    SELECT PartName 
    FROM spMstItemInfo e
    WHERE e.CompanyCode=a.CompanyCode AND
                e.PartNo=a.PartNoShip
) AS NmPart
,
case when @PPN = 0 then a.PurchasePrice
else round(a.PurchasePrice + (a.PurchasePrice * (select (TaxPct/100) from gnMstTax
where TaxCode = (select ParaValue from gnMstLookUpDtl where CodeID = 'BINS' and SeqNo = 3))),0) end PurchasePrice
,a.DiscPct,a.QtyShipped AS ReceivedQty
,isnull(a.CaseNumber,'-') AS BoxNo,
--,'-' AS BoxNo,
a.SupplierCode,
(
  SELECT CostPrice FROM spMstItemPrice f
  WHERE f.CompanyCode=a.CompanyCode AND
        f.BranchCode=a.BranchCode AND
        f.PartNo=a.PartNoShip
) AS CostPrice, a.ProductType
FROM spUtlPINVDDtl a, spTrnPPOSDtl b
WHERE 
    a.CompanyCode=b.CompanyCode AND
    a.BranchCode=b.BranchCode AND
    a.OrderNo=b.POSNo AND
    a.PartNo=b.PartNo AND
    a.CompanyCode = @CompanyCode AND
    a.BranchCode = @BranchCode AND
    a.DeliveryNo = @DNNo AND
    a.SupplierCode = @SupplierCode AND
    a.Status IN ('0','1') AND
	a.TypeOfGoods = @TypeOfGoods
end
GO

if object_id('uspfn_GetPartNoSuppliedWeb') is not null
	drop procedure uspfn_GetPartNoSuppliedWeb
GO
CREATE procedure [dbo].[uspfn_GetPartNoSuppliedWeb]   
(  
--DECLARE
@CompanyCode as varchar(15),
@BranchCode as varchar(15),
@ProductType as varchar(15),
@JobOrderNo as varchar(15),
@DocNo as varchar(15)

--set @CompanyCode = '6006406'
--set @BranchCode = '6006400'
--set	@ProductType = '4W'
--set	@JobOrderNo = 'SPK/12/000002'
--set @DocNo = 'SSS/12/000010'

)  
AS  

begin

declare @md bit
declare @dbMD varchar(25)
declare @CompanyMD varchar(25)

set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

if(@md = 1)
BEGIN

SELECT 
DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, ServiceNo
    , SupplySlipDate
	, a.PartNo
	, c.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
	, SUM(QtyOrder) QtyOrder
    , QtySupply
    , DiscPct
FROM
(
SELECT
	DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
    , c.DocDate SupplySlipDate
	, a.PartNo
	, a.RetailPrice
	, f.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, Item.QtyOrder	
    , ISNULL(e.QtySupply,0) QtySupply
    , a.DiscPct
FROM
	SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
	LEFT JOIN SvTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.ProductType = b.ProductType
		AND a.ServiceNo = b.ServiceNo
	LEFT JOIN (SELECT CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice,
				SUM(ISNULL(DemandQty, 0) - ISNULL(SupplyQty, 0)) QtyOrder
				FROM SvTrnSrvItem 
				GROUP BY CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice) Item ON
		Item.CompanyCode = a.CompanyCode AND Item.BranchCode = a.BranchCode AND Item.ProductType =
			a.ProductType AND Item.ServiceNo = a.ServiceNo AND Item.PartNo = a.PartNo and Item.RetailPrice = a.RetailPrice
	LEFT JOIN SpTrnSORDHdr c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND b.JobOrderNo = c.UsageDocNo
	LEFT JOIN SpTrnSOSupply e WITH (NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
		AND a.BranchCode = e.BranchCode
		AND c.DocNo = e.DocNo
		AND a.PartNo = e.PartNo
    INNER JOIN gnMstCompanyMapping cm WITH(NOLOCK, NOWAIT) ON cm.CompanyCode = a.CompanyCode
    LEFT JOIN SpMstItemPrice f WITH(NOLOCK, NOWAIT) ON cm.CompanyMD = f.CompanyCode
		AND a.BranchCode = f.BranchCode
		AND a.PartNo = f.PartNo
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND Item.QtyOrder > 0
    AND c.DocNo = @DocNo
	AND JobOrderNo = @JobOrderNo
    AND a.RetailPrice = Item.RetailPrice
)a
LEFT JOIN SpMstItemPrice c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND a.PartNo = c.PartNo
GROUP BY
a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, ServiceNo
    , SupplySlipDate
	, a.PartNo
	, c.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
    , QtySupply
    , DiscPct

END
ELSE
BEGIN

set @dbMD = (select DISTINCT dbmd from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)

--set @CompanyMD = (select DISTINCT CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
--select @CompanyMD
declare @query varchar(max)

if(isnull(cast(@dbMD as varchar(max)),'') = '')
BEGIN

SELECT 
DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, ServiceNo
    , SupplySlipDate
	, a.PartNo
	, c.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
	, SUM(QtyOrder) QtyOrder
    , QtySupply
    , DiscPct
FROM
(
SELECT
	DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
    , c.DocDate SupplySlipDate
	, a.PartNo
	, a.RetailPrice
	, f.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, Item.QtyOrder	
    , ISNULL(e.QtySupply,0) QtySupply
    , a.DiscPct
FROM
	SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
	LEFT JOIN SvTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.ProductType = b.ProductType
		AND a.ServiceNo = b.ServiceNo
	LEFT JOIN (SELECT CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice,
				SUM(ISNULL(DemandQty, 0) - ISNULL(SupplyQty, 0)) QtyOrder
				FROM SvTrnSrvItem 
				GROUP BY CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice) Item ON
		Item.CompanyCode = a.CompanyCode AND Item.BranchCode = a.BranchCode AND Item.ProductType =
			a.ProductType AND Item.ServiceNo = a.ServiceNo AND Item.PartNo = a.PartNo and Item.RetailPrice = a.RetailPrice
	LEFT JOIN SpTrnSORDHdr c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND b.JobOrderNo = c.UsageDocNo
	LEFT JOIN SpTrnSOSupply e WITH (NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
		AND a.BranchCode = e.BranchCode
		AND c.DocNo = e.DocNo
		AND a.PartNo = e.PartNo
    LEFT JOIN SpMstItemPrice f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
		AND a.BranchCode = f.BranchCode
		AND a.PartNo = f.PartNo
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND Item.QtyOrder > 0
    AND c.DocNo = @DocNo
	AND JobOrderNo = @JobOrderNo
    AND a.RetailPrice = Item.RetailPrice
)a
LEFT JOIN SpMstItemPrice c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND a.PartNo = c.PartNo
GROUP BY
a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, ServiceNo
    , SupplySlipDate
	, a.PartNo
	, c.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
    , QtySupply
    , DiscPct

END
ELSE
BEGIN

set @query = '
SELECT 
DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, ServiceNo
    , SupplySlipDate
	, a.PartNo
	, c.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
	, SUM(QtyOrder) QtyOrder
    , QtySupply
    , DiscPct
FROM
(
SELECT
	DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
    , c.DocDate SupplySlipDate
	, a.PartNo
	, a.RetailPrice
	, f.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, Item.QtyOrder	
    , ISNULL(e.QtySupply,0) QtySupply
    , a.DiscPct
FROM
	SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
	LEFT JOIN SvTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.ProductType = b.ProductType
		AND a.ServiceNo = b.ServiceNo
	LEFT JOIN (SELECT CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice,
				SUM(ISNULL(DemandQty, 0) - ISNULL(SupplyQty, 0)) QtyOrder
				FROM SvTrnSrvItem 
				GROUP BY CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice) Item ON
		Item.CompanyCode = a.CompanyCode AND Item.BranchCode = a.BranchCode AND Item.ProductType =
			a.ProductType AND Item.ServiceNo = a.ServiceNo AND Item.PartNo = a.PartNo and Item.RetailPrice = a.RetailPrice
	LEFT JOIN SpTrnSORDHdr c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND b.JobOrderNo = c.UsageDocNo
	LEFT JOIN SpTrnSOSupply e WITH (NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
		AND a.BranchCode = e.BranchCode
		AND c.DocNo = e.DocNo
		AND a.PartNo = e.PartNo
    INNER JOIN gnMstCompanyMapping cm WITH(NOLOCK, NOWAIT) ON cm.CompanyCode = a.CompanyCode
    LEFT JOIN '+@dbMD+'..SpMstItemPrice f WITH(NOLOCK, NOWAIT) ON cm.CompanyMD = f.CompanyCode
		AND a.BranchCode = f.BranchCode
		AND a.PartNo = f.PartNo
WHERE
	a.CompanyCode = '+@CompanyCode+'
	AND a.BranchCode = '+@BranchCode+'
	AND a.ProductType = '+@ProductType+'
	AND Item.QtyOrder > 0
    AND c.DocNo = '+@DocNo+'
	AND JobOrderNo = '+@JobOrderNo+'
    AND a.RetailPrice = Item.RetailPrice
)a
LEFT JOIN SpMstItemPrice c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND a.PartNo = c.PartNo
GROUP BY
a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, ServiceNo
    , SupplySlipDate
	, a.PartNo
	, c.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
    , QtySupply
    , DiscPct
'
EXEC(@query)
print(@query)
				
		END	 
	END
end	
GO

if object_id('uspfn_SelectFakturRetur') is not null
	drop procedure uspfn_SelectFakturRetur
GO
CREATE procedure [dbo].[uspfn_SelectFakturRetur]
@CompanyCode as varchar(15),
@BranchCode as varchar(15),
@TypeOfGoods as varchar(15)
as 
begin
--set @CompanyCode = '6006406'
--set @BranchCode = '6006401'
--set	@FPJNo = 'FPJ/14/000487'
--set	@TypeOfGoods = '5'
--set	@ProductType = '4W'
	select a.FpjNo
    , a.FpjDate
    , a.CustomerCode
    , isnull((
			select CustomerName from gnMstCustomer
			 where CompanyCode = a.CompanyCode and CustomerCode = a.CustomerCode
			), '') as CustomerName  
	 from spTrnSFpjHdr a, gnMstCoProfileSpare b
	where 1 = 1
	  and b.CompanyCode = a.CompanyCode
	  and b.BranchCode = a.BranchCode
	  and a.CompanyCode = @CompanyCode
	  and a.BranchCode  = @BranchCode 
	  and a.TypeOfGoods = @TypeOfGoods 
end
GO
if object_id('uspfn_SpcekvalidTrnPOrderBalance') is not null
	drop procedure uspfn_SpcekvalidTrnPOrderBalance
GO
CREATE PROCEDURE [dbo].[uspfn_SpcekvalidTrnPOrderBalance]
@CompanyCode varchar(15),
@BranchCode varchar(15) ,
@TypeOfGoods varchar(15) ,
@POSNo varchar(15)
AS    
	SELECT 
		DISTINCT a.POSNo
	FROM 
		spTrnPOrderBalance a 
	INNER JOIN gnMstSupplier b 
	   ON b.SupplierCode = a.SupplierCode 
	  AND b.CompanyCode  = a.CompanyCode 
	WHERE a.OrderQty > a.Received
	  AND a.CompanyCode = @CompanyCode
	  AND a.BranchCode  = @BranchCode
	  AND a.TypeOfGoods = @TypeOfGoods
	  AND a.POSNo       = @POSNo
	ORDER BY POSNo DESC
GO

if object_id('uspfn_SvTrnServiceSelectEstimationData') is not null
	drop procedure uspfn_SvTrnServiceSelectEstimationData
GO
CREATE procedure [dbo].[uspfn_SvTrnServiceSelectEstimationData]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType  varchar(15)
AS
SELECT DISTINCT 
    svTrnService.InvoiceNo
    , svTrnService.ServiceNo
    , svTrnService.ServiceType
    , ForemanID
    , EstimationNo
    , EstimationDate
    , BookingNo
    , BookingDate
    , svTrnService.JobOrderNo
    , svTrnService.JobOrderDate
    , svTrnService.PoliceRegNo
    , ServiceBookNo
    , svTrnService.BasicModel
    , TransmissionType
    , svTrnService.ChassisCode
    , svTrnService.ChassisNo
    , svTrnService.EngineCode
    , svTrnService.EngineNo
    , svTrnService.ChassisCode + ' ' + cast(svTrnService.ChassisNo as  varchar) KodeRangka
    , svTrnService.EngineCode + ' ' + cast(svTrnService.EngineNo as varchar) KodeMesin
    , ColorCode
    , (svTrnService.CustomerCode + ' - ' + gnMstCustomer.CustomerName) as Customer
    , (svTrnService.CustomerCodeBill + ' - ' + custBill.CustomerName) as CustomerBill
    , svTrnService.CustomerCode
    , svTrnService.CustomerCodeBill
    , svTrnService.Odometer
    , svTrnService.JobType
    , case when svTrnService.ServiceStatus='4' then
            case when @ProductType='4W' then reffService.Description
                else reffService.LockingBy
            end
        else reffService.Description 
    end ServiceStatus
    --, svTrnService.PoliceRegNo
	--, svTrnService.CustomerCode
    , InsurancePayFlag
    , InsuranceOwnRisk
    , InsuranceNo
    , InsuranceJobOrderNo
    --, svTrnService.CustomerCodeBill
    , svTrnService.LaborDiscPct
    , PartDiscPct
    , svTrnService.MaterialDiscPct
    , svTrnService.PPNPct
    , svTrnService.ServiceRequestDesc
    , ConfirmChangingPart
    --, ForemanID
    , svTrnService.MechanicID
    , EstimateFinishDate
    , svTrnService.LaborDPPAmt
    , svTrnService.PartsDPPAmt
    , svTrnService.MaterialDPPAmt
    , TotalDPPAmount
    , TotalPpnAmount
    , TotalPphAmount
    , TotalSrvAmount
    , employee.EmployeeName
    , (custBill.Address1 + '' + custBill.Address2 + '' + custBill.Address3 + '' + custBill.Address4) as AddressBill
    , custBill.NPWPNo
    , custBill.PhoneNo
    , custBill.HPNo
FROM svTrnService WITH(NOLOCK, NOWAIT)
LEFT JOIN gnMstCustomer 
    ON gnMstCustomer.CompanyCode = svTrnService.CompanyCode 
    AND gnMstCustomer.CustomerCode = svTrnService.CustomerCode
LEFT JOIN gnMstCustomer custBill 
    ON custBill.CompanyCode = svTrnService.CompanyCode
    AND custBill.CustomerCode = svTrnService.CustomerCodeBill
LEFT JOIN gnMstEmployee employee
    ON employee.CompanyCode = svTrnService.CompanyCode
    AND employee.BranchCode = svTrnService.BranchCode
	AND employee.EmployeeID = svTrnService.ForemanID
LEFT JOIN svTrnSrvItem srvItem 
    ON srvItem.CompanyCode = svTrnService.CompanyCode
    AND srvItem.BranchCode = svTrnService.BranchCode
    AND srvItem.ProductType = svTrnService.ProductType
    AND srvItem.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svTrnSrvTask srvTask
    ON srvTask.CompanyCode = svTrnService.CompanyCode
    AND srvTask.BranchCode = svTrnService.BranchCode
    AND srvTask.ProductType = svTrnService.ProductType
    AND srvTask.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svMstRefferenceService reffService
    ON reffService.CompanyCode = svTrnService.CompanyCode
    AND reffService.ProductType = svTrnService.ProductType    
    AND reffService.RefferenceCode = svTrnService.ServiceStatus
    AND reffService.RefferenceType = 'SERVSTAS'
LEFT JOIN svTrnInvoice invoice
	ON invoice.CompanyCode = svTrnService.CompanyCode
	AND invoice.BranchCode = svTrnService.BranchCode
	AND invoice.ProductType = svTrnService.ProductType
	AND invoice.JobOrderNo = svTrnService.JobOrderNo
LEFT JOIN svTrnSrvVOR VOR
    ON VOR.CompanyCode = svTrnService.CompanyCode
	AND VOR.BranchCode = svTrnService.BranchCode
    AND VOR.ServiceNo = svTrnService.ServiceNo
WHERE svTrnService.CompanyCode = @CompanyCode 
    AND svTrnService.BranchCode = @BranchCode
 AND svTrnService.ServiceType in ('0','2') and svTrnService.EstimationNo <> ''
 AND svTrnService.ServiceStatus IN (0,1,2,3,4,5) ORDER BY svTrnService.EstimationNo DESC
 
 GO
