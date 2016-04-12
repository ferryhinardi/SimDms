DECLARE
	@DealerCode			VARCHAR(15),
	@OutletCode			VARCHAR(15),
	@Year				INT,
	@Date				DATE,
	@YTDMonth			INT,
	@LastYearStart		DATE,
	@LastYearEnd		DATE,
	@YTDLastYearStart	DATE,
	@YTDLastYearEnd		DATE,
	@YTDStart			DATE,
	@YTDEnd				DATE

SELECT @Year = 2015, @YTDMonth = 7

SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + MONTH(GETDATE()) - 1 , DAY(GETDATE()) - 1)
SELECT @LastYearStart		= DATEADD(yy, DATEDIFF(yy,0,@Date) - 1, 0),
	   @LastYearEnd			= DATEADD(yy, DATEDIFF(yy,0,@Date), -1),
	   @YTDLastYearStart	= DATEADD(yy, DATEDIFF(yy,0,@Date) - 1, 0),
	   @YTDLastYearEnd		= DATEADD(dd,-(DAY(DATEADD(mm,1,DATEADD(yy, -1, @Date)))),DATEADD(mm,1,DATEADD(yy, -1, @Date))),
	   @YTDStart			= DATEADD(yy, DATEDIFF(yy,0,@Date), 0),
	   @YTDEnd				= DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
	
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
	Jun				INT,
	Jul				INT,
	Aug				INT,
	Sep				INT,
	Oct				INT,
	Nov				INT,
	[Dec]			INT
)

DECLARE @Months TABLE (	M INT )
;WITH _1 AS ( SELECT 1 AS val UNION ALL SELECT val + 1 FROM _1 WHERE val < 12 ) INSERT INTO @Months SELECT * FROM _1

DECLARE @TempTable TABLE (Number int, Value int)

DECLARE curDealer CURSOR LOCAL FAST_FORWARD FOR SELECT DealerCode FROM gnMstDealerMapping WHERE isActive = 1
OPEN curDealer
FETCH NEXT FROM curDealer INTO @DealerCode
WHILE @@FETCH_STATUS = 0 BEGIN

	DECLARE curOutlet CURSOR LOCAL FAST_FORWARD FOR SELECT OutletCode FROM gnMstDealerOutletMapping WHERE DealerCode = @DealerCode AND isActive = 1
	OPEN curOutlet
	FETCH NEXT FROM curOutlet INTO @OutletCode
	WHILE @@FETCH_STATUS = 0 BEGIN
		--Promotion SH-BM
		DECLARE @Position1 VARCHAR(3) = 'SH'
		DECLARE @Position2 VARCHAR(3) = 'BM'

		DECLARE @i INT = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END
		
			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
				AND ISNULL(a.IsDeleted, 0) = 0
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
			INSERT INTO @TempTable VALUES (@i, @PRO_SH_BM)
			DROP TABLE #PRO_SH_BM

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Promotion SH to BM'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)

		DELETE FROM @TempTable

		-- PROMOTION S TO SH
		SET @Position1 = 'S'
		SET @Position2 = 'SH'

		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
				AND ISNULL(a.IsDeleted, 0) = 0
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
			INSERT INTO @TempTable VALUES (@i, @PRO_S_SH)
			DROP TABLE #PRO_S_SH

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Promotion S to SH'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable

		-- Promotion to Platinum
		DECLARE @Grade INT = 4

		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
				AND ISNULL(a.IsDeleted, 0) = 0
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position = 'S'
				AND b.Grade > a.Grade
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
			INSERT INTO @TempTable VALUES (@i, @PRO_S_SH)
			DROP TABLE #PRO_PLATINUM

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Promotion to Platinum'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable

		-- To Gold
		SET @Grade = 3

		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
				AND ISNULL(a.IsDeleted, 0) = 0
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position = 'S'
				AND b.Grade > a.Grade
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
			INSERT INTO @TempTable VALUES (@i, @PRO_S_SH)
			DROP TABLE #PRO_GOLD

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Promotion to Gold'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable

		-- To Silver
		SET @Grade = 2

		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
				AND ISNULL(a.IsDeleted, 0) = 0
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Position = 'S'
				AND b.Grade > a.Grade
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
			INSERT INTO @TempTable VALUES (@i, @PRO_S_SH)
			DROP TABLE #PRO_SILVER

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Promotion to Silver'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable

		-- Trainee / NEW
		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
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
			INSERT INTO @TempTable VALUES (@i, @PRO_S_SH)
			DROP TABLE #PRO_TRAINEE

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'New Trainee'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable

		-- Demotion BM to SH
		SET @Position1 = 'BM'
		SET @Position2 = 'SH'

		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
				AND ISNULL(a.IsDeleted, 0) = 0
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
			INSERT INTO @TempTable VALUES (@i, @DEM_BM_SH)
			DROP TABLE #DEM_BM_SH

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Demotion BM to SH'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable
		
		-- Demotion SH to S
		SET @Position1 = 'SH'
		SET @Position2 = 'S'

		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
				AND ISNULL(a.IsDeleted, 0) = 0
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
			INSERT INTO @TempTable VALUES (@i, @DEM_SH_S)
			DROP TABLE #DEM_SH_S

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Demotion SH to S'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable

		-- Demotion SH to Platinum
		SET @Position1 = 'SH'
		SET @Position2 = 'S'

		SET @Grade = 4

		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
				AND ISNULL(a.IsDeleted, 0) = 0
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Grade <> b.Grade
				AND (a.Position = @Position1 OR a.Position = 'BM')
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
			INSERT INTO @TempTable VALUES (@i, @DEM_PLATINUM)
			DROP TABLE #DEM_PLATINUM

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Demotion to Platinum'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable

		-- Demotion to Gold
		SET @Position1 = 'SH'
		SET @Position2 = 'S'

		SET @Grade = 3

		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
				AND ISNULL(a.IsDeleted, 0) = 0
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Grade <> b.Grade
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
			INSERT INTO @TempTable VALUES (@i, @DEM_GOLD)
			DROP TABLE #DEM_GOLD

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Demotion to Gold'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable

		-- Demotion to Silver
		SET @Position1 = 'SH'
		SET @Position2 = 'S'

		SET @Grade = 2

		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
				AND ISNULL(a.IsDeleted, 0) = 0
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Grade <> b.Grade
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
			INSERT INTO @TempTable VALUES (@i, @DEM_SILVER)
			DROP TABLE #DEM_SILVER

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Demotion to Silver'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable

		-- Demotion to Trainee
		SET @Position1 = 'SH'
		SET @Position2 = 'S'

		SET @Grade = 1

		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
			END

			;WITH _1 AS (
				SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
				FROM HrEmployeeAchievement a
				WHERE 1=1
				AND a.CompanyCode = @DealerCode
				AND a.Department = 'SALES'
				AND ISNULL(a.IsDeleted, 0) = 0
			), _2 AS (
				SELECT a.CompanyCode, a.EmployeeID, b.Position
				, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
				, b.AssignDate
				FROM _1 a
				LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
				WHERE a.Grade <> b.Grade
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
			INSERT INTO @TempTable VALUES (@i, @DEM_TRAINEE)
			DROP TABLE #DEM_TRAINEE

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Demotion to Trainee'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable

		-- Mutation In

		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
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
				AND ISNULL(b.IsDeleted, 0) = 0
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
				AND ISNULL(a.IsDeleted, 0) = 0
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

			INSERT INTO @TempTable VALUES (@i, @MUT_IN)
			DROP TABLE #MUT_DEPT_IN

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Mutation In'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable

		-- Mutation Out

		SET @i = 1
		WHILE @i <= 15 BEGIN
			IF(@i <= 12) BEGIN
				IF(@i > @YTDMonth) BEGIN
					INSERT INTO @TempTable VALUES (@i, 0)
					SET @i = @i + 1
					CONTINUE;	
				END
				SELECT @Date = DATEADD(mm, (@Year - 1900) * 12 + @i - 1 , 0)
				SELECT @Start = DATEADD(dd,-(DAY(@Date)-1),@Date),
					   @End = DATEADD(dd,-(DAY(DATEADD(mm,1,@Date))),DATEADD(mm,1,@Date))
			END ELSE IF (@i = 13) BEGIN
				SET @Start = @LastYearStart
				SET @End = @LastYearEnd	
			END ELSE IF (@i = 14) BEGIN
				SET @Start = @YTDLastYearStart
				SET @End = @YTDLastYearEnd	
			END ELSE IF (@i = 15) BEGIN
				INSERT INTO @TempTable VALUES (@i, 0)
				SET @i = @i + 1
				CONTINUE;
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
				AND ISNULL(b.IsDeleted, 0) = 0
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
				AND ISNULL(a.IsDeleted, 0) = 0
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

			INSERT INTO @TempTable VALUES (@i, @MUT_OUT)
			DROP TABLE #MUT_DEPT_OUT

			SET @i = @i + 1
		END

		INSERT INTO #ResultTable VALUES 
		(
			'Mutation Out'
			, @DealerCode
			, @OutletCode
			, (SELECT Value		 FROM @TempTable WHERE Number = 13)
			, (SELECT Value		 FROM @TempTable WHERE Number = 14)
			, (SELECT SUM(Value) FROM @TempTable WHERE Number IN (SELECT M FROM @Months))
			, (SELECT Value		 FROM @TempTable WHERE Number = 1)
			, (SELECT Value		 FROM @TempTable WHERE Number = 2)
			, (SELECT Value		 FROM @TempTable WHERE Number = 3)
			, (SELECT Value		 FROM @TempTable WHERE Number = 4)
			, (SELECT Value		 FROM @TempTable WHERE Number = 5)
			, (SELECT Value		 FROM @TempTable WHERE Number = 6)
			, (SELECT Value		 FROM @TempTable WHERE Number = 7)
			, (SELECT Value		 FROM @TempTable WHERE Number = 8)
			, (SELECT Value		 FROM @TempTable WHERE Number = 9)
			, (SELECT Value		 FROM @TempTable WHERE Number = 10)
			, (SELECT Value		 FROM @TempTable WHERE Number = 11)
			, (SELECT Value		 FROM @TempTable WHERE Number = 12)
		)
		DELETE FROM @TempTable

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
IF OBJECT_ID('tempdb..#ResultTable') IS NOT NULL DROP TABLE #ResultTable