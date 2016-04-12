
ALTER procedure [dbo].[usprpt_PmRpInqSummaryWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@PeriodBegin		DATETIME,
	@PeriodEnd			DATETIME,
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@Salesman			VARCHAR(15),
	@Jns				VARCHAR(1),
	@print				int = 0
	
)
AS 
BEGIN
SET NOCOUNT ON;
declare @position varchar(20), @SC varchar(20)
set @position= (
				select position 
				from HrEmployee 
				where employeeid=(select TeamLeader from HrEmployee where EmployeeID = @salesman) 
				)
set @SC= (select TeamLeader from HrEmployee where EmployeeID = @salesman)
if @print = 0
begin 
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
				AND b.StatusProspek = '10' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) NEW,
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
		FROM HrEmployee a WHERE a.Department = 'SALES' AND a.CompanyCode = @CompanyCode and a.PersonnelStatus ='1'
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
			   (SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) LOST
			   --(SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') NEW,
			   --(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') PROSPECT,
			   --(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') HOTPROSPECT,
			   --(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') SPK,
			   --(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DO,
			   --(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DELIVERY,
			   --(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') LOST
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
				--SELECT * FROM #qryS_SC WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead) 
				--UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead )
				--) 
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
				SELECT * FROM #qryS_SC WHERE EmployeeID IN (@SC)
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead) 
				UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead )--)
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
			inner join HrEmployee c 
				on b.CompanyCode = c.CompanyCode and b.EmployeeID = c.EmployeeID 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) and c.PersonnelStatus = '1'
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.ModelKendaraan,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.StatusProspek = '10' AND b.EmployeeID = a.EmployeeID 
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
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN(@BranchManager)) 
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
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
				FROM #dSls a 
				WHERE a.TeamLeader = @SalesHead
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.ModelKendaraan
			)#dt27_2

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_2 ORDER BY ModelKendaraan

			DROP TABLE #dt27_2
		END
		ELSE
		BEGIN
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
	END
ELSE IF @Jns = '3'
	BEGIN
		SELECT * INTO #dByTipe2 FROM(
			SELECT b.EmployeeID, b.PerolehanData, LastProgress, StatusProspek FROM PmKdp b 
			inner join HrEmployee c 
				on b.CompanyCode = c.CompanyCode and b.EmployeeID = c.EmployeeID 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) and c.PersonnelStatus = '1'
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
				FROM #dSls2 a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN(@BranchManager))
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
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
				FROM #dSls2 a WHERE a.TeamLeader IN (@SalesHead)
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.PerolehanData
			)#dt37_2

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_2 ORDER BY PerolehanData

			DROP TABLE #dt37_2
		END
		ELSE
		begin
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
				FROM #dSls2 a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.PerolehanData
			)#dt37_3

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_3 ORDER BY PerolehanData

			DROP TABLE #dt37_3

		DROP TABLE #dSls2
		DROP TABLE #dByTipe2
		end
	END
end
else Begin
Print 'For Print Preview'
	SELECT * INTO #deptSalesP FROM(
		SELECT 
				'4' idx,
			   a.EmployeeID,
			   a.Position,
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.StatusProspek = '10' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) NEW,
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
		FROM HrEmployee a WHERE a.Department = 'SALES' AND a.CompanyCode = @CompanyCode and a.PersonnelStatus ='1'
	)#deptSalesP

	--Sales Coordinator
	SELECT * INTO #qryS_SCP FROM(
		SELECT 
				'3' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT ISNULL(SUM(NEW), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) LOST
		FROM #deptSalesP a WHERE a.Position = 'SC' 
	)#qryS_SCP

	--Sales Head
	SELECT * INTO #qrySHP FROM(
		SELECT 
				'2' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
			   (SELECT ISNULL(SUM(NEW), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) LOST
			   --(SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') NEW,
			   --(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') PROSPECT,
			   --(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') HOTPROSPECT,
			   --(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') SPK,
			   --(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DO,
			   --(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DELIVERY,
			   --(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') LOST
		FROM #deptSalesP a WHERE a.Position = 'SH' 
	)#qrySHP
	
	SELECT * INTO #dByTipeP FROM(
			SELECT b.EmployeeID, (b.TipeKendaraan + ' ' + b.Variant) ModelKendaraan, LastProgress, StatusProspek FROM PmKdp b 
			inner join HrEmployee c 
				on b.CompanyCode = c.CompanyCode and b.EmployeeID = c.EmployeeID 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) and c.PersonnelStatus = '1'
		)#dByTipeP

		SELECT * INTO #dSlsP FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.ModelKendaraan,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.StatusProspek = '10' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) LOST
			FROM #dByTipeP a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.ModelKendaraan
		)#dSlsP
		
		SELECT * INTO #dByTipe2P FROM(
			SELECT b.EmployeeID, b.PerolehanData, LastProgress, StatusProspek FROM PmKdp b 
			inner join HrEmployee c 
				on b.CompanyCode = c.CompanyCode and b.EmployeeID = c.EmployeeID 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) and c.PersonnelStatus = '1'
		)#dByTipe2P

		SELECT * INTO #dSls2P FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.PerolehanData,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.StatusProspek <> '20' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) LOST
			FROM #dByTipe2P a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.PerolehanData
		)#dSls2P
		
		IF(@SalesHead = '' AND @Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBMP FROM(
				SELECT 
					'1' idx,
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSalesP a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBMP

			SELECT * INTO #qryAllP FROM(
				SELECT * FROM #qryBMP
				UNION
				SELECT * FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBMP)     
				UNION
				SELECT * FROM #qryS_SCP WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBMP)) 
				UNION
				SELECT * FROM #deptSalesP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM))) 
			)#qryAllP

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAllP a ORDER BY a.idx ASC
		
			DROP TABLE #qryAllP
			DROP TABLE #qryBMP
			
			--table2
			SELECT * INTO #dt27_1P FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSlsP a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN(@BranchManager)) 
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.ModelKendaraan
			)#dt27_1P

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_1P ORDER BY ModelKendaraan

			DROP TABLE #dt27_1P
			
			--table3
			SELECT * INTO #dt37_1P FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSls2P a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN(@BranchManager))
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.PerolehanData
			)#dt37_1P

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_1P ORDER BY PerolehanData

			DROP TABLE #dt37_1
			
		END
	ELSE IF(@Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM2P FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSalesP a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM2P

			SELECT * INTO #qryAll2P FROM(
				SELECT * FROM #qryBM2P
				UNION
				SELECT * FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2P) AND EmployeeID = @SalesHead     
				UNION
				--SELECT * FROM #qryS_SC WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead) 
				--UNION
				SELECT * FROM #deptSalesP WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2P) AND EmployeeID = @SalesHead )
				--) 
			)#qryAll2P

			SELECT 
				'6159401000' CompanyCode, '6159401001' BranchCode, ''TeamID, '' PositionID, a.EmployeeName,
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS PositionName,
				 a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAll2P a ORDER BY a.idx ASC

			DROP TABLE #qryAll2P
			DROP TABLE #qryBM2P
			
			--table2
			SELECT * INTO #dt27_2P FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSlsP a 
				WHERE a.TeamLeader =@SalesHead
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.ModelKendaraan
			)#dt27_2P

			SELECT ModelKendaraan TipeKendaraan, '' GroupCode, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_2P ORDER BY ModelKendaraan

			DROP TABLE #dt27_2P
			
			--table3
			SELECT * INTO #dt37_2P FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2P a WHERE a.TeamLeader IN (@SalesHead)
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.PerolehanData
			)#dt37_2P

			SELECT '6159401000' CompanyCode, PerolehanData Source, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_2P ORDER BY PerolehanData

			DROP TABLE #dt37_2P
			
		END
	ELSE
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM3P FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSalesP a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM3P

			SELECT * INTO #qryAll3P FROM(
				SELECT * FROM #qryBM3P
				UNION
				SELECT * FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3P) AND EmployeeID = @SalesHead     
				UNION
				SELECT * FROM #qryS_SCP WHERE EmployeeID IN (@SC)
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead) 
				UNION
				SELECT * FROM #deptSalesP WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3P) AND EmployeeID = @SalesHead )--)
				AND EmployeeID = @Salesman 
			)#qryAll3P

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST 
			FROM #qryAll3P a ORDER BY a.idx ASC 

			DROP TABLE #qryAll3
			DROP TABLE #qryBM3
			
			--table2
			SELECT * INTO #dt27_3P FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSlsP a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.ModelKendaraan
			)#dt27_3P

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_3P ORDER BY ModelKendaraan

			DROP TABLE #dt27_3P
			
			--table3
			SELECT * INTO #dt37_3P FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2P a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.PerolehanData
			)#dt37_3

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_3P ORDER BY PerolehanData

			DROP TABLE #dt37_3P
			
		END
End
END
