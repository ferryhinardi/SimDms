IF OBJECT_ID('[dbo].[usprpt_mpInOutDataDealer]') IS NOT NULL DROP PROCEDURE [dbo].[usprpt_mpInOutDataDealer]
GO

GO
/****** Object:  StoredProcedure [dbo].[usprpt_mpInOutData]    Script Date: 9/2/2015 2:15:26 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--CREATED BY Benedict 3 Sep 2015 LAST UPDATE BY Benedict 3 Sep 2015

CREATE PROCEDURE [dbo].[usprpt_mpInOutDataDealer]
--DECLARE 
	@DealerCode			VARCHAR(15),
	@Position			VARCHAR(5),
	@Start				DATE,
	@End				DATE,
	@Outlet				VARCHAR(15),
	@Filter				VARCHAR(15)

--exec usprpt_mpInOutData '', '', '01/01/2014', '06/30/2015', '', ''
--exec usprpt_mpInOutData '6006400001', 'S', '01/01/2014', '06/30/2015', '6006400101', 'start'

--SELECT @DealerCode = '6006406',
--	@Position = 'S',
--	@Start = '01/01/2014',
--	@End = '06/30/2015',
--	@Outlet = '6006406',--'6006406',
--	@Filter = '' --'start'

AS

DECLARE @OutletCode	VARCHAR(15),
	@OutletAbbr		VARCHAR(50)

SET @OutletCode = @Outlet

DECLARE @DealerOutlet TABLE
(
	DealerCode	VARCHAR(15),
	OutletCode	VARCHAR(15),
	OutletAbbr	VARCHAR(50)
)

INSERT INTO @DealerOutlet
SELECT a.DealerCode, a.OutletCode, a.OutletAbbreviation AS OutletAbbr
FROM gnMstDealerOutletMapping a
WHERE a.isActive = 1 AND a.DealerCode = CASE @DealerCode WHEN '' THEN a.DealerCode ELSE @DealerCode END
AND a.OutletCode = @Outlet
--SELECT DealerCode, OutletCode, OutletAbbr FROM @DealerOutlet

DECLARE @ResultTable TABLE
(
	OutletCode	VARCHAR(15),
	OutletAbbr	VARCHAR(50),
	Start		INT,
	[Join]		INT,
	Resign		INT,
	MutationIn	INT,
	MutationOut	INT,
	[End]		INT
)

SELECT * INTO #all_data FROM
(
	SELECT a.CompanyCode, a.EmployeeID, a.Position, ISNULL(a.Grade, '') Grade, a.JoinDate, a.ResignDate
	FROM HrEmployee a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
	AND CONVERT(DATE, a.JoinDate) <= @End
	AND (CONVERT(DATE, a.ResignDate) > @Start OR a.ResignDate IS NULL OR a.ResignDate <= a.JoinDate)
) #all_data

DECLARE @TemplateTable TABLE (
	CompanyCode		VARCHAR(15), 
	BranchCode		VARCHAR(15), 
	EmployeeID		VARCHAR(20), 
	Position		VARCHAR(5), 
	Grade			VARCHAR(2), 
	JoinDate		DATE, 
	ResignDate		DATE, 
	MutationDate	DATE, 
	AssignDate		DATE
)

SELECT TOP 0 * INTO #all_start  FROM @TemplateTable
SELECT TOP 0 * INTO #all_join   FROM @TemplateTable
SELECT TOP 0 * INTO #all_resign FROM @TemplateTable
SELECT TOP 0 * INTO #all_in     FROM @TemplateTable
SELECT TOP 0 * INTO #all_out    FROM @TemplateTable
SELECT TOP 0 * INTO #all_end    FROM @TemplateTable

DECLARE @Query1 NVARCHAR(MAX)
	, @Query2 NVARCHAR(MAX)
	, @Query3 NVARCHAR(MAX)
	, @Where1 NVARCHAR(MAX)
	, @Where2 NVARCHAR(MAX)
	, @Query NVARCHAR(MAX)
	, @Params NVARCHAR(MAX)

SET @Params = N'@DealerCode VARCHAR(15), @OutletCode VARCHAR(15), @Start DATE, @End DATE, @Position VARCHAR(4)'

SET @Query1 = N';WITH _1 AS
				(
					SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, MAX(b.MutationDate) MutationDate
					FROM #all_data a
					LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
					WHERE 1 = 1 
					AND b.BranchCode = @OutletCode'

SET @Query2 = N'	GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate
				), _2 AS
				(
					SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, b.MutationDate, MAX(c.AssignDate) AS AssignDate
					FROM _1 a
					INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
					LEFT JOIN HrEmployeeAchievement c ON a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID
					GROUP BY a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, b.MutationDate
				), _3 AS
				(
					SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.Position, ISNULL(b.Grade, a.Grade) Grade, a.JoinDate, a.ResignDate, a.MutationDate, a.AssignDate
					FROM _2 a
					LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
				)
				SELECT * FROM _3 WHERE Position = CASE @Position WHEN '''' THEN Position ELSE @Position END'

-- START
IF(@Filter = 'start' OR @Filter = '') BEGIN
	SELECT @Where1 = N' AND CONVERT(DATE, b.MutationDate) < @Start ',
		   @Query = REPLACE(REPLACE(REPLACE(REPLACE(@Query1 + @Where1 + @Query2, '  ', ''), CHAR(9), ''), CHAR(10), ''), CHAR(13), ' ')
	INSERT INTO #all_start
	EXEC sp_executesql @Query, @Params, @DealerCode=@DealerCode, @OutletCode=@OutletCode, @Start=@Start, @End=@End, @Position=@Position
END

-- JOIN
IF(@Filter = 'join' OR @Filter = '') BEGIN
	SELECT @Where1 = N' AND CONVERT(DATE, a.JoinDate) >= @Start ',
		   @Query = REPLACE(REPLACE(REPLACE(REPLACE(@Query1 + @Where1 + @Query2, '  ', ''), CHAR(9), ''), CHAR(10), ''), CHAR(13), ' ')
	INSERT INTO #all_join
	EXEC sp_executesql @Query, @Params, @DealerCode=@DealerCode, @OutletCode=@OutletCode, @Start=@Start, @End=@End, @Position=@Position
END

-- RESIGN
IF(@Filter = 'resign' OR @Filter = '') BEGIN
	SELECT @Where1 = N' AND CONVERT(DATE, a.ResignDate) < @End ',
		   @Query = REPLACE(REPLACE(REPLACE(REPLACE(@Query1 + @Where1 + @Query2, '  ', ''), CHAR(9), ''), CHAR(10), ''), CHAR(13), ' ')
	INSERT INTO #all_resign
	EXEC sp_executesql @Query, @Params, @DealerCode=@DealerCode, @OutletCode=@OutletCode, @Start=@Start, @End=@End, @Position=@Position
END

-- END
IF(@Filter = 'end' OR @Filter = '') BEGIN
	SELECT @Where1 = N' AND (CONVERT(DATE, a.ResignDate) > @End OR a.ResignDate IS NULL)',
		   @Query = REPLACE(REPLACE(REPLACE(REPLACE(@Query1 + @Where1 + @Query2, '  ', ''), CHAR(9), ''), CHAR(10), ''), CHAR(13), ' ')
	INSERT INTO #all_end
	EXEC sp_executesql @Query, @Params, @DealerCode=@DealerCode, @OutletCode=@OutletCode, @Start=@Start, @End=@End, @Position=@Position
END

SELECT  @Query1 = N';WITH _1 AS
					(
						SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.MutationDate), a.CompanyCode, a.BranchCode, a.EmployeeID, b.Position, b.Grade, b.JoinDate, b.ResignDate, a.MutationDate
						FROM HrEmployeeMutation a
						JOIN #all_data b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
						WHERE CONVERT(DATE, a.MutationDate) > @Start
						AND CONVERT(DATE, a.MutationDate) < @End
					), _2 AS
					( ',
		@Query2 = N'	FROM _1 a
						LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID 
						WHERE 1 = 1 ',
		@Query3 = N'), _3 AS
					(
						SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, a.MutationDate, MAX(b.AssignDate) AssignDate
						FROM _2 a
						LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
						WHERE b.Department = ''SALES''
						AND CONVERT(DATE, ISNULL(b.AssignDate, a.JoinDate)) <= CONVERT(DATE, a.MutationDate)
						GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, a.MutationDate
					), _4 AS
					(
						SELECT a.CompanyCode, a.BranchCode, a.EmployeeID
						, ISNULL(b.Position, a.Position) AS Position
						, CASE WHEN b.Position <> ''S'' THEN '''' ELSE (CASE b.Grade WHEN '''' THEN ''1'' ELSE ISNULL(b.Grade, ''1'') END) END AS Grade
						, a.JoinDate, a.ResignDate, a.MutationDate, a.AssignDate
						FROM _3 a
						JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
					)
					SELECT * FROM _4 WHERE Position = CASE @Position WHEN '''' THEN Position ELSE @Position END'		
-- MUTATION IN
IF (@Filter = 'in' OR @Filter = '') BEGIN
	SELECT @Where1 = N'SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, a.MutationDate ',
		   @Where2 = N'AND a.BranchCode <> ISNULL(b.BranchCode, '''') ',
		   @Query = REPLACE(REPLACE(REPLACE(REPLACE(@Query1 + @Where1 + @Query2 + @Where2 + @Query3, '  ', ''), CHAR(9), ''), CHAR(10), ''), CHAR(13), ' ')
	INSERT INTO #all_in
	EXEC sp_executesql @Query, @Params, @DealerCode=@DealerCode, @OutletCode=@OutletCode, @Start=@Start, @End=@End, @Position=@Position
END

-- MUTATION OUT
IF (@Filter = 'out' OR @Filter = '') BEGIN
	SELECT @Where1 = N'SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, b.MutationDate ',
		   @Where2 = N'AND a.BranchCode <> b.BranchCode ',
		   @Query = REPLACE(REPLACE(REPLACE(REPLACE(@Query1 + @Where1 + @Query2 + @Where2 + @Query3, '  ', ''), CHAR(9), ''), CHAR(10), ''), CHAR(13), ' ')
	INSERT INTO #all_out
	EXEC sp_executesql @Query, @Params, @DealerCode=@DealerCode, @OutletCode=@OutletCode, @Start=@Start, @End=@End, @Position=@Position
END

/* --- MUTATION QUERY ---
;WITH _1 AS
(
	SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.MutationDate), a.CompanyCode, a.BranchCode, a.EmployeeID, b.Position, b.Grade, b.JoinDate, b.ResignDate, a.MutationDate
	FROM HrEmployeeMutation a
	JOIN #all_data b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE CONVERT(DATE, a.MutationDate) > @Start
	AND CONVERT(DATE, a.MutationDate) < @End
), _2 AS
(
	--SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, a.MutationDate
	--ALL :: SELECT a.CompanyCode, a.EmployeeID, a.BranchCode AS B1, a.MutationDate AS M1, b.BranchCode AS B2, b.MutationDate AS M2
	--IN  :: a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, a.MutationDate
	--OUT :: a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, b.MutationDate
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, a.MutationDate
	FROM _1 a
	LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID 
	WHERE 1 = 1 
	AND a.BranchCode <> ISNULL(b.BranchCode, '')
	--IN   :: AND a.BranchCode <> ISNULL(b.BranchCode, '')
	--OUT  :: AND a.BranchCode <> b.BranchCode
	
), _3 AS
(
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, a.MutationDate, MAX(b.AssignDate) AssignDate
	FROM _2 a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE b.Department = 'SALES'
	AND CONVERT(DATE, ISNULL(b.AssignDate, a.JoinDate)) <= CONVERT(DATE, a.MutationDate)
	GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate, a.ResignDate, a.MutationDate
), _4 AS
(
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID
	, ISNULL(b.Position, a.Position) AS Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, a.JoinDate, a.ResignDate, a.MutationDate, a.AssignDate
	FROM _3 a
	JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
)
SELECT * FROM _4 WHERE Position = @Position
*/

IF(@Filter = '') BEGIN
	DECLARE curOutlet CURSOR LOCAL FAST_FORWARD FOR SELECT DealerCode, OutletCode, OutletAbbr FROM @DealerOutlet WHERE OutletCode = @OutletCode
	OPEN curOutlet
	FETCH NEXT FROM curOutlet INTO @DealerCode, @OutletCode, @OutletAbbr
	WHILE @@FETCH_STATUS = 0 BEGIN
		INSERT INTO @ResultTable
		VALUES (
			  @OutletCode
			, @OutletAbbr
			, (SELECT COUNT(*) FROM #all_start  a WHERE a.CompanyCode = @DealerCode AND a.BranchCode = @OutletCode)
			, (SELECT COUNT(*) FROM #all_join   a WHERE a.CompanyCode = @DealerCode AND a.BranchCode = @OutletCode)
			, (SELECT COUNT(*) FROM #all_resign a WHERE a.CompanyCode = @DealerCode AND a.BranchCode = @OutletCode)
			, (SELECT COUNT(*) FROM #all_in     a WHERE a.CompanyCode = @DealerCode AND a.BranchCode = @OutletCode)
			, (SELECT COUNT(*) FROM #all_out    a WHERE a.CompanyCode = @DealerCode AND a.BranchCode = @OutletCode)
			, (SELECT COUNT(*) FROM #all_end    a WHERE a.CompanyCode = @DealerCode AND a.BranchCode = @OutletCode)
			)
	
		FETCH NEXT FROM curOutlet INTO @DealerCode, @OutletCode, @OutletAbbr
	END CLOSE curOutlet DEALLOCATE curOutlet

	SELECT * FROM @ResultTable

	-- TOTAL
	SELECT SUM(a.Start) AS Start
		 , SUM(a.[Join]) AS [Join]
		 , SUM(a.Resign) AS Resign
		 , SUM(a.MutationIn) AS MutationIn
		 , SUM(a.MutationOut) AS MutationOut
		 , SUM(a.[End]) AS [End]
		FROM @ResultTable a


END ELSE BEGIN
	SET @OutletCode = @Outlet
	DECLARE @FilterResult TABLE 
	(
		DealerCode		VARCHAR(15),
		OutletCode		VARCHAR(15),
		OutletAbbr		VARCHAR(35),
		EmployeeID		VARCHAR(20),
		EmployeeName	VARCHAR(150),
		Position		VARCHAR(4),
		PosName			VARCHAR(15),
		Grade			VARCHAR(2),
		GradeName		VARCHAR(15),
		JoinDate		DATE
	)
	SELECT @Query1 = N'SELECT 
							a.CompanyCode AS DealerCode
						  , a.BranchCode AS OutletCode
						  , c.OutletAbbreviation AS OutletAbbr
						  , a.EmployeeID
						  , b.EmployeeName
						  , a.Position
						  , d.PosName
						  , a.Grade
						  , ISNULL(e.LookupValueName, '''') AS GradeName
						  , a.JoinDate ',
		   @Query2 = N'INNER JOIN hrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
					   INNER JOIN gnMstDealerOutletMapping c ON a.BranchCode = c.OutletCode
					   INNER JOIN gnMstPosition d ON d.CompanyCode = a.CompanyCode AND d.DeptCode = ''SALES'' AND d.PosCode = a.Position
					   LEFT JOIN gnMstLookUpDtl e ON e.CompanyCode = a.CompanyCode AND e.CodeID = ''ITSG'' AND e.LookUpValue = a.Grade
					   WHERE a.CompanyCode = @DealerCode AND a.BranchCode = @OutletCode'
	
	IF	    (@Filter = 'start' ) SET @Where1 = N'FROM #all_start a '
	ELSE IF (@Filter = 'join'  ) SET @Where1 = N'FROM #all_join a '
	ELSE IF (@Filter = 'resign') SET @Where1 = N'FROM #all_resign a '
	ELSE IF (@Filter = 'in'    ) SET @Where1 = N'FROM #all_in a '
	ELSE IF (@Filter = 'out'   ) SET @Where1 = N'FROM #all_out a '
	ELSE IF (@Filter = 'end'   ) SET @Where1 = N'FROM #all_end a '
	
	SET @Query = REPLACE(REPLACE(REPLACE(REPLACE(@Query1 + @Where1 + @Query2, '  ', ''), CHAR(9), ''), CHAR(10), ''), CHAR(13), ' ')
	INSERT INTO @FilterResult
	EXEC sp_executesql @Query, @Params, @DealerCode=@DealerCode, @OutletCode=@OutletCode, @Start=@Start, @End=@End, @Position=@Position
	SELECT * FROM @FilterResult
END

IF OBJECT_ID('tempdb..#all_data  ') IS NOT NULL DROP TABLE #all_data
IF OBJECT_ID('tempdb..#all_start ') IS NOT NULL DROP TABLE #all_start 
IF OBJECT_ID('tempdb..#all_join  ') IS NOT NULL DROP TABLE #all_join  
IF OBJECT_ID('tempdb..#all_resign') IS NOT NULL DROP TABLE #all_resign
IF OBJECT_ID('tempdb..#all_in    ') IS NOT NULL DROP TABLE #all_in    
IF OBJECT_ID('tempdb..#all_out   ') IS NOT NULL DROP TABLE #all_out   
IF OBJECT_ID('tempdb..#all_end   ') IS NOT NULL DROP TABLE #all_end   
