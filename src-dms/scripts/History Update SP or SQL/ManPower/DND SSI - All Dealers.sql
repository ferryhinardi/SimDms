DECLARE
	@DealerCode			VARCHAR(15),
	@OutletCode			VARCHAR(15),
	@LastYearStart		DATE,
	@LastYearEnd		DATE,
	@YTDLastYearStart	DATE,
	@YTDLastYearEnd		DATE,
	@YTDStart			DATE,
	@YTDEnd				DATE,
	@JanStart			DATE,
	@JanEnd				DATE,
	@FebStart			DATE,
	@FebEnd				DATE,
	@MarStart			DATE,
	@MarEnd				DATE,
	@AprStart			DATE,
	@AprEnd				DATE,
	@MayStart			DATE,
	@MayEnd				DATE,
	@JunStart			DATE,
	@JunEnd				DATE
	
SET @DealerCode			= '6006400001'
SET	@OutletCode			= '6006400101'
SET @LastYearStart		= '2014-01-01'
SET @LastYearEnd		= '2014-12-31'
SET	@YTDLastYearStart	= '2014-01-01'
SET	@YTDLastYearEnd		= '2014-06-30'
SET	@YTDStart			= '2015-01-01'
SET	@YTDEnd				= '2015-06-30'
SET @JanStart			= '2015-01-01'
SET @JanEnd				= '2015-01-31'
SET @FebStart			= '2015-02-01'
SET @FebEnd				= '2015-02-28'
SET @MarStart			= '2015-03-01'
SET @MarEnd				= '2015-03-31'
SET @AprStart			= '2015-04-01'
SET @AprEnd				= '2015-04-30'
SET @MayStart			= '2015-05-01'
SET @MayEnd				= '2015-05-31'
SET @JunStart			= '2015-06-01'
SET @JunEnd				= '2015-06-30'
	
DECLARE 
	@PRO_SH_BM		INT,
	@PRO_S_SH		INT,
	@PRO_PLATINUM	INT,
	@PRO_GOLD		INT,
	@PRO_SILVER		INT,
	@PRO_TRAINEE	INT,
	@DEM_BM_SH		INT,
	@DEM_SH_S		INT,
	@DEM_PLATINUM	INT,
	@DEM_GOLD		INT,
	@DEM_SILVER		INT,
	@DEM_TRAINEE	INT,
	@MUT_IN			INT,
	@MUT_OUT		INT,
	@Start			DATE,
	@End			DATE

CREATE TABLE #ResultTable 
(
	ManPower		VARCHAR (50),
	DealerCode		VARCHAR (15),
	OutletCode		VARCHAR (15),
	LastYear		INT,
	YTDLastYear		INT,
	YTD				INT,
	Jan				INT,
	Feb				INT,
	Mar				INT,
	Apr				INT,
	May				INT,
	Jun				INT
)

DECLARE curDealer CURSOR LOCAL FAST_FORWARD FOR SELECT DealerCode FROM gnMstDealerMapping
OPEN curDealer
FETCH NEXT FROM curDealer INTO @DealerCode
WHILE @@FETCH_STATUS = 0 BEGIN

	DECLARE curOutlet CURSOR LOCAL FAST_FORWARD FOR SELECT OutletCode FROM gnMstDealerOutletMapping WHERE DealerCode = @DealerCode
	OPEN curOutlet
	FETCH NEXT FROM curOutlet INTO @OutletCode
	WHILE @@FETCH_STATUS = 0 BEGIN
		--Promotion SH-BM
		DECLARE @Position1 VARCHAR(3) = 'SH'
		DECLARE @Position2 VARCHAR(3) = 'BM'

		DECLARE @i INT = 0
		CREATE TABLE #TempTable1 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable1 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

		;WITH _1 AS (
			SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			FROM HrEmployeeAchievement a
			WHERE 1=1
			AND a.CompanyCode = @DealerCode
			AND a.Department = 'SALES'
		), _2 AS (
			SELECT a.CompanyCode, a.EmployeeID, b.Position
			, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
			, b.AssignDate
			FROM _1 a
			LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
			WHERE a.Position <> b.Position
			AND a.Position = @Position1
			AND b.Position = @Position2
			AND @Start <= CONVERT(date, b.AssignDate)
			AND CONVERT(date, b.AssignDate) <= @End
		), _3 AS (
			SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
			FROM _2 a
			LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
			WHERE 1=1
			AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
			GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
		), _4 AS (
			SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
			FROM _3 a
			INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
		), _5 AS (
			SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
			, ISNULL(a.Position, b.Position) AS Position
			, ISNULL(a.Grade, b.Grade) AS Grade
			, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
			FROM _4 a
			INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
			WHERE b.Department = 'SALES'	
		)
		SELECT * INTO #PRO_SH_BM FROM (
			SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
			FROM _5 a
			WHERE a.BranchCode = @OutletCode
		) #PRO_SH_BM

			SET @PRO_SH_BM = (SELECT COUNT(a.EmployeeID) FROM #PRO_SH_BM a)
			INSERT INTO #TempTable1 VALUES (@i, @PRO_SH_BM)
			DROP TABLE #PRO_SH_BM

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Promotion SH to BM'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable1 WHERE Number = 0)
			, (SELECT Value FROM #TempTable1 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable1 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable1 WHERE Number = 3)
			, (SELECT Value FROM #TempTable1 WHERE Number = 4)
			, (SELECT Value FROM #TempTable1 WHERE Number = 5)
			, (SELECT Value FROM #TempTable1 WHERE Number = 6)
			, (SELECT Value FROM #TempTable1 WHERE Number = 7)
			, (SELECT Value FROM #TempTable1 WHERE Number = 8)
		)

		DROP TABLE #TempTable1

		-- PROMOTION S TO SH
		SET @Position1 = 'S'
		SET @Position2 = 'SH'

		SET @i = 0
		CREATE TABLE #TempTable2 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable2 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position <> b.Position
				AND a.Position = @Position1
				AND b.Position = @Position2
				AND @Start <= CONVERT(date, b.AssignDate)
				AND CONVERT(date, b.AssignDate) <= @End
			), _3 AS (
				SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
				FROM _2 a
				LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1=1
				AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
				GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			), _4 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
				FROM _3 a
				INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
				, ISNULL(a.Position, b.Position) AS Position
				, ISNULL(a.Grade, b.Grade) AS Grade
				, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
				FROM _4 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE b.Department = 'SALES'	
			)
			SELECT * INTO #PRO_S_SH FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode
			) #PRO_S_SH

			SET @PRO_S_SH = (SELECT COUNT(a.EmployeeID) FROM #PRO_S_SH a)
			INSERT INTO #TempTable2 VALUES (@i, @PRO_S_SH)
			DROP TABLE #PRO_S_SH

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Promotion S to SH'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable2 WHERE Number = 0)
			, (SELECT Value FROM #TempTable2 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable2 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable2 WHERE Number = 3)
			, (SELECT Value FROM #TempTable2 WHERE Number = 4)
			, (SELECT Value FROM #TempTable2 WHERE Number = 5)
			, (SELECT Value FROM #TempTable2 WHERE Number = 6)
			, (SELECT Value FROM #TempTable2 WHERE Number = 7)
			, (SELECT Value FROM #TempTable2 WHERE Number = 8)
		)
		DROP TABLE #TempTable2

		-- Promotion to Platinum
		DECLARE @Grade INT = 4

		SET @i = 0
		CREATE TABLE #TempTable3 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable3 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position = 'S'
				AND b.Grade <> a.Grade
				AND b.Grade = @Grade
				AND @Start <= CONVERT(date, b.AssignDate)
				AND CONVERT(date, b.AssignDate) <= @End
			), _3 AS (
				SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
				FROM _2 a
				LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1=1
				AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
				GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			), _4 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
				FROM _3 a
				INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
				, ISNULL(a.Position, b.Position) AS Position
				, ISNULL(a.Grade, b.Grade) AS Grade
				, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
				FROM _4 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE b.Department = 'SALES'	
			)
			SELECT * INTO #PRO_PLATINUM FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode
			) #PRO_PLATINUM

			SET @PRO_S_SH = (SELECT COUNT(a.EmployeeID) FROM #PRO_PLATINUM a)
			INSERT INTO #TempTable3 VALUES (@i, @PRO_S_SH)
			DROP TABLE #PRO_PLATINUM

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Promotion to Platinum'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable3 WHERE Number = 0)
			, (SELECT Value FROM #TempTable3 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable3 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable3 WHERE Number = 3)
			, (SELECT Value FROM #TempTable3 WHERE Number = 4)
			, (SELECT Value FROM #TempTable3 WHERE Number = 5)
			, (SELECT Value FROM #TempTable3 WHERE Number = 6)
			, (SELECT Value FROM #TempTable3 WHERE Number = 7)
			, (SELECT Value FROM #TempTable3 WHERE Number = 8)
		)
		DROP TABLE #TempTable3

		-- To Gold
		SET @Grade = 3

		SET @i = 0
		CREATE TABLE #TempTable4 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable4 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position = 'S'
				AND b.Grade <> a.Grade
				AND b.Grade = @Grade
				AND @Start <= CONVERT(date, b.AssignDate)
				AND CONVERT(date, b.AssignDate) <= @End
			), _3 AS (
				SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
				FROM _2 a
				LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1=1
				AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
				GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			), _4 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
				FROM _3 a
				INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
				, ISNULL(a.Position, b.Position) AS Position
				, ISNULL(a.Grade, b.Grade) AS Grade
				, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
				FROM _4 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE b.Department = 'SALES'	
			)
			SELECT * INTO #PRO_GOLD FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode
			) #PRO_GOLD

			SET @PRO_S_SH = (SELECT COUNT(a.EmployeeID) FROM #PRO_GOLD a)
			INSERT INTO #TempTable4 VALUES (@i, @PRO_S_SH)
			DROP TABLE #PRO_GOLD

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Promotion to Gold'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable4 WHERE Number = 0)
			, (SELECT Value FROM #TempTable4 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable4 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable4 WHERE Number = 3)
			, (SELECT Value FROM #TempTable4 WHERE Number = 4)
			, (SELECT Value FROM #TempTable4 WHERE Number = 5)
			, (SELECT Value FROM #TempTable4 WHERE Number = 6)
			, (SELECT Value FROM #TempTable4 WHERE Number = 7)
			, (SELECT Value FROM #TempTable4 WHERE Number = 8)
		)
		DROP TABLE #TempTable4

		-- To Silver
		SET @Grade = 2

		SET @i = 0
		CREATE TABLE #TempTable5 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable5 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position = 'S'
				AND b.Grade <> a.Grade
				AND b.Grade = @Grade
				AND @Start <= CONVERT(date, b.AssignDate)
				AND CONVERT(date, b.AssignDate) <= @End
			), _3 AS (
				SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
				FROM _2 a
				LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1=1
				AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
				GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			), _4 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
				FROM _3 a
				INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
				, ISNULL(a.Position, b.Position) AS Position
				, ISNULL(a.Grade, b.Grade) AS Grade
				, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
				FROM _4 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE b.Department = 'SALES'	
			)
			SELECT * INTO #PRO_SILVER FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode
			) #PRO_SILVER

			SET @PRO_S_SH = (SELECT COUNT(a.EmployeeID) FROM #PRO_SILVER a)
			INSERT INTO #TempTable5 VALUES (@i, @PRO_S_SH)
			DROP TABLE #PRO_SILVER

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Promotion to Silver'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable5 WHERE Number = 0)
			, (SELECT Value FROM #TempTable5 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable5 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable5 WHERE Number = 3)
			, (SELECT Value FROM #TempTable5 WHERE Number = 4)
			, (SELECT Value FROM #TempTable5 WHERE Number = 5)
			, (SELECT Value FROM #TempTable5 WHERE Number = 6)
			, (SELECT Value FROM #TempTable5 WHERE Number = 7)
			, (SELECT Value FROM #TempTable5 WHERE Number = 8)
		)
		DROP TABLE #TempTable5

		-- Trainee / NEW
		SET @i = 0
		CREATE TABLE #TempTable6 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable6 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS 
			(
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate
				FROM HrEmployeeMutation a
				WHERE a.CompanyCode = @DealerCode
				GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID
			), _2 AS
			(
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate
				FROM HrEmployee a
				INNER JOIN _1 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
				AND @Start <= CONVERT(DATE, a.JoinDate)
				AND CONVERT(DATE, a.JoinDate) <= @End
			)
			SELECT * INTO #PRO_TRAINEE FROM
			(
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _2 a
				WHERE a.BranchCode = @OutletCode
			) #PRO_TRAINEE
			SET @PRO_S_SH = (SELECT COUNT(a.EmployeeID) FROM #PRO_TRAINEE a)
			INSERT INTO #TempTable6 VALUES (@i, @PRO_S_SH)
			DROP TABLE #PRO_TRAINEE

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'New Trainee'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable6 WHERE Number = 0)
			, (SELECT Value FROM #TempTable6 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable6 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable6 WHERE Number = 3)
			, (SELECT Value FROM #TempTable6 WHERE Number = 4)
			, (SELECT Value FROM #TempTable6 WHERE Number = 5)
			, (SELECT Value FROM #TempTable6 WHERE Number = 6)
			, (SELECT Value FROM #TempTable6 WHERE Number = 7)
			, (SELECT Value FROM #TempTable6 WHERE Number = 8)
		)
		DROP TABLE #TempTable6

		-- Demotion BM to SH
		SET @Position1 = 'BM'
		SET @Position2 = 'SH'

		SET @i = 0
		CREATE TABLE #TempTable7 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable7 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position <> b.Position
				AND a.Position = @Position1
				AND b.Position = @Position2
				AND @Start <= CONVERT(date, b.AssignDate)
				AND CONVERT(date, b.AssignDate) <= @End
			), _3 AS (
				SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
				FROM _2 a
				LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1=1
				AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
				GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			), _4 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
				FROM _3 a
				INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
				, ISNULL(a.Position, b.Position) AS Position
				, ISNULL(a.Grade, b.Grade) AS Grade
				, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
				FROM _4 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE b.Department = 'SALES'	
			)
			SELECT * INTO #DEM_BM_SH FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode
			) #DEM_BM_SH

			SET @DEM_BM_SH = (SELECT COUNT(a.EmployeeID) FROM #DEM_BM_SH a)
			INSERT INTO #TempTable7 VALUES (@i, @DEM_BM_SH)
			DROP TABLE #DEM_BM_SH

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Demotion BM to SH'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable7 WHERE Number = 0)
			, (SELECT Value FROM #TempTable7 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable7 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable7 WHERE Number = 3)
			, (SELECT Value FROM #TempTable7 WHERE Number = 4)
			, (SELECT Value FROM #TempTable7 WHERE Number = 5)
			, (SELECT Value FROM #TempTable7 WHERE Number = 6)
			, (SELECT Value FROM #TempTable7 WHERE Number = 7)
			, (SELECT Value FROM #TempTable7 WHERE Number = 8)
		)

		DROP TABLE #TempTable7

		-- Demotion SH to S
		SET @Position1 = 'SH'
		SET @Position2 = 'S'

		SET @i = 0
		CREATE TABLE #TempTable8 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable8 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position <> b.Position
				AND a.Position = @Position1
				AND b.Position = @Position2
				AND @Start <= CONVERT(date, b.AssignDate)
				AND CONVERT(date, b.AssignDate) <= @End
			), _3 AS (
				SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
				FROM _2 a
				LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1=1
				AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
				GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			), _4 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
				FROM _3 a
				INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
				, ISNULL(a.Position, b.Position) AS Position
				, ISNULL(a.Grade, b.Grade) AS Grade
				, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
				FROM _4 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE b.Department = 'SALES'	
			)
			SELECT * INTO #DEM_SH_S FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode
			) #DEM_SH_S

			SET @DEM_SH_S = (SELECT COUNT(a.EmployeeID) FROM #DEM_SH_S a)
			INSERT INTO #TempTable8 VALUES (@i, @DEM_SH_S)
			DROP TABLE #DEM_SH_S

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Demotion SH to S'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable8 WHERE Number = 0)
			, (SELECT Value FROM #TempTable8 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable8 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable8 WHERE Number = 3)
			, (SELECT Value FROM #TempTable8 WHERE Number = 4)
			, (SELECT Value FROM #TempTable8 WHERE Number = 5)
			, (SELECT Value FROM #TempTable8 WHERE Number = 6)
			, (SELECT Value FROM #TempTable8 WHERE Number = 7)
			, (SELECT Value FROM #TempTable8 WHERE Number = 8)
		)

		DROP TABLE #TempTable8

		-- Demotion SH to Platinum
		SET @Position1 = 'SH'
		SET @Position2 = 'S'

		SET @Grade = 4

		SET @i = 0
		CREATE TABLE #TempTable9 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable9 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position <> b.Position
				AND a.Position = @Position1
				AND b.Position = @Position2
				AND b.Grade = @Grade
				AND @Start <= CONVERT(date, b.AssignDate)
				AND CONVERT(date, b.AssignDate) <= @End
			), _3 AS (
				SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
				FROM _2 a
				LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1=1
				AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
				GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			), _4 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
				FROM _3 a
				INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
				, ISNULL(a.Position, b.Position) AS Position
				, ISNULL(a.Grade, b.Grade) AS Grade
				, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
				FROM _4 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE b.Department = 'SALES'	
			)
			SELECT * INTO #DEM_PLATINUM FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode
			) #DEM_PLATINUM

			SET @DEM_PLATINUM = (SELECT COUNT(a.EmployeeID) FROM #DEM_PLATINUM a)
			INSERT INTO #TempTable9 VALUES (@i, @DEM_PLATINUM)
			DROP TABLE #DEM_PLATINUM

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Demotion to Platinum'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable9 WHERE Number = 0)
			, (SELECT Value FROM #TempTable9 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable9 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable9 WHERE Number = 3)
			, (SELECT Value FROM #TempTable9 WHERE Number = 4)
			, (SELECT Value FROM #TempTable9 WHERE Number = 5)
			, (SELECT Value FROM #TempTable9 WHERE Number = 6)
			, (SELECT Value FROM #TempTable9 WHERE Number = 7)
			, (SELECT Value FROM #TempTable9 WHERE Number = 8)
		)

		DROP TABLE #TempTable9

		-- Demotion to Gold
		SET @Position1 = 'SH'
		SET @Position2 = 'S'

		SET @Grade = 3

		SET @i = 0
		CREATE TABLE #TempTable10 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable10 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position <> b.Position
				AND (a.Position = @Position1 OR (a.Position = @Position2 AND a.Grade = 4 ))
				AND b.Position = @Position2
				AND b.Grade = @Grade
				AND @Start <= CONVERT(date, b.AssignDate)
				AND CONVERT(date, b.AssignDate) <= @End
			), _3 AS (
				SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
				FROM _2 a
				LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1=1
				AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
				GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			), _4 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
				FROM _3 a
				INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
				, ISNULL(a.Position, b.Position) AS Position
				, ISNULL(a.Grade, b.Grade) AS Grade
				, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
				FROM _4 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE b.Department = 'SALES'	
			)
			SELECT * INTO #DEM_GOLD FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode
			) #DEM_GOLD

			SET @DEM_GOLD = (SELECT COUNT(a.EmployeeID) FROM #DEM_GOLD a)
			INSERT INTO #TempTable10 VALUES (@i, @DEM_GOLD)
			DROP TABLE #DEM_GOLD

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Demotion to Gold'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable10 WHERE Number = 0)
			, (SELECT Value FROM #TempTable10 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable10 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable10 WHERE Number = 3)
			, (SELECT Value FROM #TempTable10 WHERE Number = 4)
			, (SELECT Value FROM #TempTable10 WHERE Number = 5)
			, (SELECT Value FROM #TempTable10 WHERE Number = 6)
			, (SELECT Value FROM #TempTable10 WHERE Number = 7)
			, (SELECT Value FROM #TempTable10 WHERE Number = 8)
		)

		DROP TABLE #TempTable10

		-- Demotion to Silver
		SET @Position1 = 'SH'
		SET @Position2 = 'S'

		SET @Grade = 2

		SET @i = 0
		CREATE TABLE #TempTable11 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable11 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position <> b.Position
				AND (a.Position = @Position1 OR (a.Position = @Position2 AND (a.Grade IN (4, 3))))
				AND b.Position = @Position2
				AND b.Grade = @Grade
				AND @Start <= CONVERT(date, b.AssignDate)
				AND CONVERT(date, b.AssignDate) <= @End
			), _3 AS (
				SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
				FROM _2 a
				LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1=1
				AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
				GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			), _4 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
				FROM _3 a
				INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
				, ISNULL(a.Position, b.Position) AS Position
				, ISNULL(a.Grade, b.Grade) AS Grade
				, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
				FROM _4 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE b.Department = 'SALES'	
			)
			SELECT * INTO #DEM_SILVER FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode
			) #DEM_SILVER

			SET @DEM_SILVER = (SELECT COUNT(a.EmployeeID) FROM #DEM_SILVER a)
			INSERT INTO #TempTable11 VALUES (@i, @DEM_SILVER)
			DROP TABLE #DEM_SILVER

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Demotion to Silver'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable11 WHERE Number = 0)
			, (SELECT Value FROM #TempTable11 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable11 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable11 WHERE Number = 3)
			, (SELECT Value FROM #TempTable11 WHERE Number = 4)
			, (SELECT Value FROM #TempTable11 WHERE Number = 5)
			, (SELECT Value FROM #TempTable11 WHERE Number = 6)
			, (SELECT Value FROM #TempTable11 WHERE Number = 7)
			, (SELECT Value FROM #TempTable11 WHERE Number = 8)
		)

		DROP TABLE #TempTable11

		-- Demotion to Trainee
		SET @Position1 = 'SH'
		SET @Position2 = 'S'

		SET @Grade = 1

		SET @i = 0
		CREATE TABLE #TempTable12 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable12 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position <> b.Position
				AND (a.Position = @Position1 OR (a.Position = @Position2 AND (a.Grade IN (4, 3, 2))))
				AND b.Position = @Position2
				AND b.Grade = @Grade
				AND @Start <= CONVERT(date, b.AssignDate)
				AND CONVERT(date, b.AssignDate) <= @End
			), _3 AS (
				SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
				FROM _2 a
				LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1=1
				AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
				GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			), _4 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
				FROM _3 a
				INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
				, ISNULL(a.Position, b.Position) AS Position
				, ISNULL(a.Grade, b.Grade) AS Grade
				, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
				FROM _4 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE b.Department = 'SALES'	
			)
			SELECT * INTO #DEM_TRAINEE FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode
			) #DEM_TRAINEE

			SET @DEM_TRAINEE = (SELECT COUNT(a.EmployeeID) FROM #DEM_TRAINEE a)
			INSERT INTO #TempTable12 VALUES (@i, @DEM_TRAINEE)
			DROP TABLE #DEM_TRAINEE

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Demotion to Trainee'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable12 WHERE Number = 0)
			, (SELECT Value FROM #TempTable12 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable12 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable12 WHERE Number = 3)
			, (SELECT Value FROM #TempTable12 WHERE Number = 4)
			, (SELECT Value FROM #TempTable12 WHERE Number = 5)
			, (SELECT Value FROM #TempTable12 WHERE Number = 6)
			, (SELECT Value FROM #TempTable12 WHERE Number = 7)
			, (SELECT Value FROM #TempTable12 WHERE Number = 8)
		)

		DROP TABLE #TempTable12

		-- Mutation In

		SET @i = 0
		CREATE TABLE #TempTable13 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable13 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.MutationDate), a.CompanyCode, a.BranchCode, a.EmployeeID, a.MutationDate
				FROM HrEmployeeMutation a
				WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
			), _2 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, b.MutationDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE @Start <= CONVERT(date, b.MutationDate)
				AND CONVERT(date, b.MutationDate) <= @End
				AND a.BranchCode <> b.BranchCode
			), _3 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.JoinDate, a.MutationDate, b.ResignDate 
				FROM _2 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1 = 1
				AND (CONVERT(date, b.ResignDate) IS NULL OR @End < CONVERT(date, b.ResignDate) OR CONVERT(date, b.ResignDate) <= CONVERT(date, b.JoinDate))
			), _4 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, MAX(b.AssignDate) AS AssignDate, a.ResignDate 
				FROM _3 a
				LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE CONVERT(date, b.AssignDate) < CONVERT(date, a.MutationDate)
				GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, a.ResignDate 
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName
				, ISNULL(b.Position, a.Position) AS Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN a.Grade ELSE ISNULL(b.Grade, a.Grade) END) END AS Grade
				, a.JoinDate, a.MutationDate, a.AssignDate, a.ResignDate 
				FROM _4 a
				INNER JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
			)
			SELECT * INTO #MutationIn FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode		
			) #MutationIn

			SET @MUT_IN = (SELECT COUNT(a.EmployeeID) FROM #MutationIn a)
			DROP TABLE #MutationIn

			-- Department IN
			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate, a.Department
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Department <> b.Department
				AND b.Department = 'SALES'
				AND @Start <= CONVERT(date, b.AssignDate)
				AND CONVERT(date, b.AssignDate) <= @End
			), _3 AS (
				SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
				FROM _2 a
				LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1=1
				AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
				GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			), _4 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
				FROM _3 a
				INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
				, ISNULL(a.Position, b.Position) AS Position
				, ISNULL(a.Grade, b.Grade) AS Grade
				, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
				FROM _4 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
			)
			SELECT * INTO #MUT_DEPT_IN FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode
			) #MUT_DEPT_IN

			SET @MUT_IN = @MUT_IN + (SELECT COUNT(a.EmployeeID) FROM #MUT_DEPT_IN a)

			INSERT INTO #TempTable13 VALUES (@i, @MUT_IN)
			DROP TABLE #MUT_DEPT_IN

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Mutation In'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable13 WHERE Number = 0)
			, (SELECT Value FROM #TempTable13 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable13 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable13 WHERE Number = 3)
			, (SELECT Value FROM #TempTable13 WHERE Number = 4)
			, (SELECT Value FROM #TempTable13 WHERE Number = 5)
			, (SELECT Value FROM #TempTable13 WHERE Number = 6)
			, (SELECT Value FROM #TempTable13 WHERE Number = 7)
			, (SELECT Value FROM #TempTable13 WHERE Number = 8)
		)

		DROP TABLE #TempTable13

		-- Mutation Out

		SET @i = 0
		CREATE TABLE #TempTable14 (Number int, Value int)

		WHILE @i < 9 BEGIN
			IF(@i = 0) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 1) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 2) BEGIN
				INSERT INTO #TempTable14 VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END ELSE IF (@i = 3) BEGIN
				SET @Start = @JanStart
				SET @End = @JanEnd
			END ELSE IF (@i = 4) BEGIN
				SET @Start = @FebStart
				SET @End = @FebEnd
			END ELSE IF (@i = 5) BEGIN
				SET @Start = @MarStart
				SET @End = @MarEnd
			END ELSE IF (@i = 6) BEGIN
				SET @Start = @AprStart
				SET @End = @AprEnd
			END ELSE IF (@i = 7) BEGIN
				SET @Start = @MayStart
				SET @End = @MayEnd
			END ELSE IF (@i = 8) BEGIN
				SET @Start = @JunStart
				SET @End = @JunEnd 
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.MutationDate), a.CompanyCode, a.BranchCode, a.EmployeeID, a.MutationDate
				FROM HrEmployeeMutation a
				WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
			), _2 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, b.MutationDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE @Start <= CONVERT(date, b.MutationDate)
				AND CONVERT(date, b.MutationDate) <= @End
				AND a.BranchCode <> b.BranchCode
			), _3 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.JoinDate, a.MutationDate, b.ResignDate 
				FROM _2 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1 = 1
				AND (CONVERT(date, b.ResignDate) IS NULL OR @End < CONVERT(date, b.ResignDate) OR CONVERT(date, b.ResignDate) <= CONVERT(date, b.JoinDate))
			), _4 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, MAX(b.AssignDate) AS AssignDate, a.ResignDate 
				FROM _3 a
				LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE CONVERT(date, b.AssignDate) < CONVERT(date, a.MutationDate)
				GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, a.ResignDate 
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName
				, ISNULL(b.Position, a.Position) AS Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN a.Grade ELSE ISNULL(b.Grade, a.Grade) END) END AS Grade
				, a.JoinDate, a.MutationDate, a.AssignDate, a.ResignDate 
				FROM _4 a
				INNER JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
			)
			SELECT * INTO #MutationOut FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode		
			) #MutationOut

			SET @MUT_OUT = (SELECT COUNT(a.EmployeeID) FROM #MutationOut a)
			DROP TABLE #MutationOut

			-- Department Out
			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate, a.Department
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Department <> b.Department
				AND a.Department = 'SALES'
				AND @Start <= CONVERT(date, b.AssignDate)
				AND CONVERT(date, b.AssignDate) <= @End
			), _3 AS (
				SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
				FROM _2 a
				LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE 1=1
				AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
				GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
			), _4 AS (
				SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
				FROM _3 a
				INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
			), _5 AS (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
				, ISNULL(a.Position, b.Position) AS Position
				, ISNULL(a.Grade, b.Grade) AS Grade
				, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
				FROM _4 a
				INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
			)
			SELECT * INTO #MUT_DEPT_OUT FROM (
				SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
				FROM _5 a
				WHERE a.BranchCode = @OutletCode
			) #MUT_DEPT_OUT

			SET @MUT_OUT = @MUT_OUT + (SELECT COUNT(a.EmployeeID) FROM #MUT_DEPT_OUT a)

			INSERT INTO #TempTable14 VALUES (@i, @MUT_OUT)
			DROP TABLE #MUT_DEPT_OUT

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Mutation Out'
			, @DealerCode
			, @OutletCode
			, (SELECT Value FROM #TempTable14 WHERE Number = 0)
			, (SELECT Value FROM #TempTable14 WHERE Number = 1)
			, (SELECT SUM(Value) FROM #TempTable14 WHERE Number IN (3,4,5,6,7,8))
			, (SELECT Value FROM #TempTable14 WHERE Number = 3)
			, (SELECT Value FROM #TempTable14 WHERE Number = 4)
			, (SELECT Value FROM #TempTable14 WHERE Number = 5)
			, (SELECT Value FROM #TempTable14 WHERE Number = 6)
			, (SELECT Value FROM #TempTable14 WHERE Number = 7)
			, (SELECT Value FROM #TempTable14 WHERE Number = 8)
		)

		DROP TABLE #TempTable14

		FETCH NEXT FROM curOutlet INTO @OutletCode
	END
	CLOSE curOutlet
	DEALLOCATE curOutlet

	FETCH NEXT FROM curDealer INTO @DealerCode
END
CLOSE curDealer
DEALLOCATE curDealer

SELECT * FROM #ResultTable

IF OBJECT_ID('tempdb..#PRO_SH_BM') IS NOT NULL DROP TABLE #PRO_SH_BM
IF OBJECT_ID('tempdb..#PRO_S_SH') IS NOT NULL DROP TABLE #PRO_S_SH
IF OBJECT_ID('tempdb..#PRO_PLATINUM') IS NOT NULL DROP TABLE #PRO_PLATINUM
IF OBJECT_ID('tempdb..#PRO_GOLD') IS NOT NULL DROP TABLE #PRO_GOLD
IF OBJECT_ID('tempdb..#PRO_SILVER') IS NOT NULL DROP TABLE #PRO_SILVER
IF OBJECT_ID('tempdb..#PRO_TRAINEE') IS NOT NULL DROP TABLE #PRO_TRAINEE
IF OBJECT_ID('tempdb..#DEM_BM_SH') IS NOT NULL DROP TABLE #DEM_BM_SH
IF OBJECT_ID('tempdb..#DEM_SH_S') IS NOT NULL DROP TABLE #DEM_SH_S
IF OBJECT_ID('tempdb..#DEM_PLATINUM') IS NOT NULL DROP TABLE #DEM_PLATINUM
IF OBJECT_ID('tempdb..#DEM_GOLD') IS NOT NULL DROP TABLE #DEM_GOLD
IF OBJECT_ID('tempdb..#DEM_SILVER') IS NOT NULL DROP TABLE #DEM_SILVER
IF OBJECT_ID('tempdb..#DEM_TRAINEE') IS NOT NULL DROP TABLE #DEM_TRAINEE
IF OBJECT_ID('tempdb..#MutationIn') IS NOT NULL DROP TABLE #MutationIn
IF OBJECT_ID('tempdb..#MUT_DEPT_IN') IS NOT NULL DROP TABLE #MUT_DEPT_IN
IF OBJECT_ID('tempdb..#MutationOut') IS NOT NULL DROP TABLE #MutationOut
IF OBJECT_ID('tempdb..#MUT_DEPT_OUT') IS NOT NULL DROP TABLE #MUT_DEPT_OUT
IF OBJECT_ID('tempdb..#TempTable1') IS NOT NULL DROP TABLE #TempTable1
IF OBJECT_ID('tempdb..#TempTable2') IS NOT NULL DROP TABLE #TempTable2
IF OBJECT_ID('tempdb..#TempTable3') IS NOT NULL DROP TABLE #TempTable3
IF OBJECT_ID('tempdb..#TempTable4') IS NOT NULL DROP TABLE #TempTable4
IF OBJECT_ID('tempdb..#TempTable5') IS NOT NULL DROP TABLE #TempTable5
IF OBJECT_ID('tempdb..#TempTable6') IS NOT NULL DROP TABLE #TempTable6
IF OBJECT_ID('tempdb..#TempTable7') IS NOT NULL DROP TABLE #TempTable7
IF OBJECT_ID('tempdb..#TempTable8') IS NOT NULL DROP TABLE #TempTable8
IF OBJECT_ID('tempdb..#TempTable9') IS NOT NULL DROP TABLE #TempTable9
IF OBJECT_ID('tempdb..#TempTable10') IS NOT NULL DROP TABLE #TempTable10
IF OBJECT_ID('tempdb..#TempTable11') IS NOT NULL DROP TABLE #TempTable11
IF OBJECT_ID('tempdb..#TempTable12') IS NOT NULL DROP TABLE #TempTable12
IF OBJECT_ID('tempdb..#TempTable13') IS NOT NULL DROP TABLE #TempTable13
IF OBJECT_ID('tempdb..#TempTable14') IS NOT NULL DROP TABLE #TempTable14
IF OBJECT_ID('tempdb..#ResultTable') IS NOT NULL DROP TABLE #ResultTable