IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usprpt_PmRpInqOutStanding_NewBySalesman]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usprpt_PmRpInqOutStanding_NewBySalesman]
GO

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
			AND a.TeamLeader IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @BranchManager)
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
		WHERE b.TeamLeader  = @BranchManager
		GROUP BY a.CompanyCode,
			b.EmployeeName,
			a.TeamLeader, b.TeamLeader
	)#employee_stat_SK

	SELECT * INTO #employee_stat_SH FROM(
		SELECT
			a.CompanyCode,
			a.TeamLeader EmployeeID,
			b.EmployeeName,
			'SH' PositionID,
			'Sales Head' PositionName, 
			b.TeamLeader BMEmployeeID,
			ISNULL(SUM(a.PROSPECT),0) PROSPECT,
			ISNULL(SUM(a.HOTPROSPECT),0) HOTPROSPECT,
			ISNULL(SUM(a.SPK),0) SPK
		FROM #employee_stat_SM a
		LEFT JOIN HrEmployee b
			ON b.CompanyCode = a.CompanyCode AND a.TeamLeader = b.EmployeeID
		WHERE b.TeamLeader = @BranchManager
		GROUP BY a.CompanyCode,
			b.EmployeeName,
			a.TeamLeader, b.TeamLeader
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
	SELECT '3' PositionId, a.* FROM #employee_stat_SM a
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
		END
-- == CASE III ==
		ELSE IF @SalesHead = '' AND @SalesCoordinator <> '' AND @Salesman = ''
		BEGIN
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @BranchManager
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesCoordinator
			UNION			
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @SalesCoordinator)
		END
-- == CASE IV ==
		ELSE IF (@SalesHead <> '' OR @SalesCoordinator <> '') AND @Salesman <> ''
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

