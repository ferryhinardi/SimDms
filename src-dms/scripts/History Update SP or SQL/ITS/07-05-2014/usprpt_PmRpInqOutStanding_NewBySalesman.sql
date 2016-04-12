USE [PMU]
GO
/****** Object:  StoredProcedure [dbo].[usprpt_PmRpInqOutStanding_NewBySalesman]    Script Date: 5/6/2015 3:17:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[usprpt_PmRpInqOutStanding_NewBySalesman] 
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
			(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode -- AND b.BranchCode = @BranchCode
			AND b.LastProgress = 'P' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) PROSPECT,
			(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode --AND b.BranchCode = @BranchCode
			AND b.LastProgress = 'HP' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) HOTPROSPECT,
			(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode --AND b.BranchCode = @BranchCode
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
ELSE
	BEGIN
	SELECT * INTO #employee_ALL_SM FROM(
		SELECT 
					a.CompanyCode,
					a.EmployeeID,
					a.EmployeeName,
					a.Position,
					'Salesman' PositionName, 
					a.TeamLeader,
					(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode-- AND b.BranchCode = '6159401001'
					AND b.LastProgress = 'P' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) PROSPECT,
					(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode --AND b.BranchCode = '6159401001'
					AND b.LastProgress = 'HP' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) HOTPROSPECT,
					(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode-- AND b.BranchCode = '6159401001'
					AND b.LastProgress = 'SPK' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) SPK
				FROM HrEmployee a
				WHERE a.CompanyCode = @CompanyCode  AND a.Department = 'SALES' AND a.Position = 'S' AND a.PersonnelStatus = '1'
				)#employee_ALL_SM

		select * into #A from(
	select '1' as SeqNo, a.CompanyCode, c.EmployeeID, c.EmployeeName, c.Position, 'Branch Manager' PositionName, c.TeamLeader, SUM(PROSPECT) as PROSPECT, SUM(HOTPROSPECT) as HOTPROSPECT, SUM(SPK) as SPK
	from #employee_ALL_SM a
	inner join HrEmployee b
		on a.CompanyCode = b.CompanyCode and a.TeamLeader = b.EmployeeID
	inner join HrEmployee c
		on b.CompanyCode = c.CompanyCode and b.TeamLeader = c.EmployeeID
	GROUP BY a.CompanyCode, c.EmployeeID, c.EmployeeName, c.Position, c.TeamLeader
	union
	select '2' as SeqNo,a.CompanyCode, b.EmployeeID, b.EmployeeName, b.Position, 'Sales Head' PositionName, b.TeamLeader, SUM(PROSPECT) as PROSPECT, SUM(HOTPROSPECT) as HOTPROSPECT, SUM(SPK) as SPK
	from #employee_ALL_SM a
	inner join HrEmployee b
		on a.CompanyCode = b.CompanyCode and a.TeamLeader = b.EmployeeID
	GROUP BY a.CompanyCode, b.EmployeeID, b.EmployeeName, b.Position, b.TeamLeader
	union
	select '3' as SeqNo, a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Position, 'Salesman' PositionName, a.TeamLeader, SUM(PROSPECT) as PROSPECT, SUM(HOTPROSPECT) as HOTPROSPECT, SUM(SPK) as SPK
	from #employee_ALL_SM a
	GROUP BY a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Position, a.TeamLeader
)#a

		select* from #a
		order by SeqNo Asc

		drop table #a
		drop table #employee_ALL_SM
	END
END

