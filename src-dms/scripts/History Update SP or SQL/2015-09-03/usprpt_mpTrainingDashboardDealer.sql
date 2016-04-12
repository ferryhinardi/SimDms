IF OBJECT_ID('[dbo].[usprpt_mpTrainingDashboardDealer]') IS NOT NULL DROP PROCEDURE [dbo].[usprpt_mpTrainingDashboardDealer]
GO

GO
/****** Object:  StoredProcedure [dbo].[usprpt_mpTrainingDashboard]    Script Date: 9/2/2015 2:09:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--CREATED BY Benedict 3 Sep 2015 LAST UPDATE BY Benedict 3 Sep 2015

--exec usprpt_mpTrainingDashboard '500', '6093401', '1/1/2008', '12/31/2014', '', ''
--exec usprpt_mpTrainingDashboard '500', '6093401', '1/1/2008', '12/31/2014', '609340119', 'SH_nt'
--exec usprpt_mpTrainingDashboard '', '', '1/1/2008', '12/31/2014', '', ''

CREATE PROCEDURE [dbo].[usprpt_mpTrainingDashboardDealer]
--DECLARE
	@DealerCode varchar(15),
	@Start date,
	@End date,
	@OutletCode varchar(15),
	@FilterColumn varchar(10)

AS

--SELECT 
--	@DealerCode = '6006406',
--	@Start = '1/1/2008', 
--	@End = '06/30/2015',
--	@OutletCode = '6006406',
--	@FilterColumn = 'S2_jml'

DECLARE @Positions TABLE 
(
	Position varchar(2)	
)
INSERT INTO @Positions VALUES ('BM')
INSERT INTO @Positions VALUES ('SH')
INSERT INTO @Positions VALUES ('S1')
INSERT INTO @Positions VALUES ('S2')
INSERT INTO @Positions VALUES ('S3')
INSERT INTO @Positions VALUES ('S4')

DECLARE @RowTable TABLE
(
	OutletCode	varchar(15),
	Position	varchar(20),
	Jml			int,
	T			int,
	NT			AS (Jml - T)
)

;WITH _1 AS
(
	SELECT CompanyCode, EmployeeID, MAX(AssignDate) AssignDate
	  FROM HrEmployeeAchievement
	 WHERE 1 = 1
	   AND CompanyCode = CASE @DealerCode WHEN '' THEN CompanyCode ELSE @DealerCode END
	   AND CONVERT(DATE, AssignDate) BETWEEN @Start AND @End
	   AND Department = 'SALES'
  GROUP BY CompanyCode, EmployeeID
), _2 AS
(
	SELECT b.CompanyCode, b.EmployeeID, b.Position, b.Grade
	  FROM _1 a
	  JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _3 AS
(
	SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MAX(b.MutationDate) MutationDate
	  FROM _2 a
	  JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	 WHERE 1 = 1
	   AND CONVERT(DATE, b.MutationDate) BETWEEN @Start AND @End
	   AND b.BranchCode = @OutletCode
  GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade
), _4 AS
(
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade
	  FROM _3 a
	  JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _5 AS
(
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, b.JoinDate
	  FROM _4 a
	  JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	 WHERE (b.ResignDate IS NULL OR b.ResignDate <= b.JoinDate OR CONVERT(DATE, b.ResignDate) >= @End)
)
SELECT * INTO #emp_all FROM _5

SELECT * INTO #emp_training FROM
(
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, b.TrainingCode, a.JoinDate
	  FROM #emp_all a
 LEFT JOIN HrEmployeeTraining b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	 WHERE b.TrainingDate <= @End
  GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, b.TrainingCode, a.JoinDate
) #emp_training

--SELECT * FROM #emp_training DROP TABLE #emp_training DROP TABLE #emp_all

--SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate FROM #emp_training a GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate

IF(@FilterColumn = '') BEGIN
	INSERT INTO @RowTable 
		 SELECT a.OutletCode
			  , b.Position
			  , Jml = (SELECT COUNT(*) FROM (SELECT c.CompanyCode, c.BranchCode, c.EmployeeID, c.Position, c.Grade, c.JoinDate 
											   FROM #emp_all c 
											  WHERE c.BranchCode = a.OutletCode AND c.Position + ISNULL(c.Grade, '') = b.Position 
										   GROUP BY c.CompanyCode, c.BranchCode, c.EmployeeID, c.Position, c.Grade, c.JoinDate) c)
			  , T   = CASE b.Position 
					  WHEN 'BM' THEN (SELECT COUNT(*) FROM #emp_training c WHERE c.BranchCode = a.OutletCode AND c.Position + ISNULL(c.Grade, '') = b.Position AND c.TrainingCode = 'BMDPB')
					  WHEN 'SH' THEN (SELECT COUNT(*) FROM #emp_training c WHERE c.BranchCode = a.OutletCode AND c.Position + ISNULL(c.Grade, '') = b.Position AND c.TrainingCode = 'SHB')
					  ELSE			 (SELECT COUNT(*) FROM (SELECT c.EmployeeID FROM #emp_training c 
													  WHERE c.BranchCode = a.OutletCode AND c.Position + ISNULL(c.Grade, '') = b.Position
													  AND c.TrainingCode IN ('STDP1', 'STDP2', 'STDP3', 'STDP4', 'STDP5') 
													  GROUP BY c.EmployeeID HAVING COUNT(*) = 5) d)
					  END
 		   FROM gnMstDealerOutletMapping a
	 CROSS JOIN @Positions b
 		  WHERE 1 = 1
			AND a.OutletCode = @OutletCode
 			--AND a.DealerCode = CASE @DealerCode WHEN '' THEN a.DealerCode ELSE @DealerCode END
  			--AND a.GroupNo = CASE @AreaCode WHEN '' THEN a.GroupNo ELSE @AreaCode END

	;WITH pvtJml AS
	(
		SELECT OutletCode, BM AS BM_jml, SH AS SH_jml, S1 AS S1_jml, S2 AS S2_jml, S3 AS S3_jml, S4 AS S4_jml 
		FROM (SELECT OutletCode, Position, Jml FROM @RowTable) a
		PIVOT(
			MAX(Jml) FOR a.Position IN (BM, SH, S1, S2, S3, S4)
		) b
	), pvtT AS
	(
		SELECT OutletCode, BM AS BM_t, SH AS SH_t, S1 AS S1_t, S2 AS S2_t, S3 AS S3_t, S4 AS S4_t
		FROM (SELECT OutletCode, Position, T FROM @RowTable) a
		PIVOT(
			MAX(T) FOR a.Position IN (BM, SH, S1, S2, S3, S4)
		) b
	), pvtNT AS
	(
		SELECT OutletCode, BM AS BM_nt, SH AS SH_nt, S1 AS S1_nt, S2 AS S2_nt, S3 AS S3_nt, S4 AS S4_nt
		FROM (SELECT OutletCode, Position, NT FROM @RowTable) a
		PIVOT(
			MAX(NT) FOR a.Position IN (BM, SH, S1, S2, S3, S4)
		) b
	)
	SELECT * INTO #resultTable FROM
	(
		SELECT a.OutletCode, d.OutletAbbreviation AS OutletAbbr, BM_jml, BM_t, BM_nt, SH_jml, SH_t, SH_nt, S4_jml, S4_t, S4_nt, S3_jml, S3_t, S3_nt, S2_jml, S2_t, S2_nt, S1_jml, S1_t, S1_nt
		FROM pvtJml a
		JOIN pvtT b ON a.OutletCode = b.OutletCode
		JOIN pvtNT c ON a.OutletCode = c.OutletCode
		JOIN gnMstDealerOutletMapping d ON a.OutletCode = d.OutletCode
	) #resultTable

	SELECT * FROM #resultTable

	SELECT
		  SUM(BM_jml) AS ΣBM_jml
		, SUM(BM_t  ) AS ΣBM_t  
		, SUM(BM_nt	) AS ΣBM_nt	
		, SUM(SH_jml) AS ΣSH_jml
		, SUM(SH_t	) AS ΣSH_t	
		, SUM(SH_nt	) AS ΣSH_nt	
		, SUM(S4_jml) AS ΣS4_jml
		, SUM(S4_t	) AS ΣS4_t	
		, SUM(S4_nt	) AS ΣS4_nt	
		, SUM(S3_jml) AS ΣS3_jml
		, SUM(S3_t	) AS ΣS3_t	
		, SUM(S3_nt	) AS ΣS3_nt	
		, SUM(S2_jml) AS ΣS2_jml
		, SUM(S2_t	) AS ΣS2_t	
		, SUM(S2_nt	) AS ΣS2_nt	
		, SUM(S1_jml) AS ΣS1_jml
		, SUM(S1_t	) AS ΣS1_t	
		, SUM(S1_nt	) AS ΣS1_nt	
	FROM #resultTable
END ELSE BEGIN
	DECLARE @Query NVARCHAR(4000), @Params NVARCHAR(1000)
	DECLARE @Position VARCHAR (3), @FilterType VARCHAR(4)

	DECLARE @FilterResult TABLE
	(
		CompanyCode VARCHAR(15),
		BranchCode	VARCHAR(15),
		EmployeeID	VARCHAR(20),
		Position	VARCHAR(3),
		Grade		VARCHAR(3),
		JoinDate	DATE
	)

	SELECT
		@Position = SUBSTRING(@FilterColumn, 0, 3),
		@FilterType = SUBSTRING(@FilterColumn, 4, 4)
	
	SELECT * INTO #tbl_filter FROM
	(
		SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate
		FROM #emp_all a
		WHERE a.CompanyCode = @DealerCode AND a.BranchCode = @OutletCode AND a.Position + ISNULL(a.Grade, '') = @Position
		GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade, a.JoinDate
	) #tbl_filter

	IF(@FilterType = 'jml') BEGIN
		INSERT INTO @FilterResult SELECT * FROM #tbl_filter 
	END ELSE BEGIN
		
		DECLARE @exec_result TABLE (EmployeeID varchar(50))

		SELECT @Query = N'SELECT EmployeeID FROM #emp_training b WHERE b.CompanyCode = @DealerCode AND b.BranchCode = @OutletCode AND b.Position + ISNULL(b.Grade, '''') = @Position AND b.TrainingCode '
			, @Params = N'@DealerCode varchar(15), @OutletCode varchar(15), @Position varchar(3)'
		
		IF(@Position = 'BM') BEGIN
			SET @Query += N'= ''BMDPB'''
		END ELSE IF(@Position = 'SH') BEGIN
			SET @Query += N'= ''SHB'''
		END ELSE BEGIN
			SET @Query += N'IN (''STDP1'', ''STDP2'', ''STDP3'', ''STDP4'', ''STDP5'') GROUP BY b.EmployeeID HAVING COUNT(*) = 5'
		END

		INSERT INTO @exec_result EXEC sp_executesql @Query, @Params, @DealerCode, @OutletCode, @Position

		IF(@FilterType = 't') BEGIN
			INSERT INTO @FilterResult SELECT * FROM #tbl_filter a WHERE a.EmployeeID IN (SELECT b.EmployeeID FROM @exec_result b)
		END ELSE IF (@FilterType = 'nt') BEGIN
			INSERT INTO @FilterResult SELECT * FROM #tbl_filter a WHERE a.EmployeeID NOT IN (SELECT b.EmployeeID FROM @exec_result b)
		END
	END
	
	SELECT a.BranchCode
	, b.OutletAbbreviation AS OutletAbbr
	, a.EmployeeID
	, c.EmployeeName
	, a.Position
	, d.PosName
	, a.Grade
	, ISNULL(e.LookUpValueName, '') AS GradeName
	, a.JoinDate
	FROM @FilterResult a
	INNER JOIN gnMstDealerOutletMapping b ON a.CompanyCode = b.DealerCode AND a.BranchCode = b.OutletCode
	INNER JOIN HrEmployee c ON a.CompanyCode = c.CompanyCode AND c.Department = 'SALES' AND a.EmployeeID = c.EmployeeID
	INNER JOIN gnMstPosition d ON d.CompanyCode = a.CompanyCode AND d.DeptCode = 'SALES' AND d.PosCode = a.Position
	LEFT JOIN gnMstLookUpDtl e ON e.CompanyCode = a.CompanyCode AND e.CodeID = 'ITSG' AND e.LookUpValue = a.Grade
	--INNER JOIN gnMstPosition d ON d.CompanyCode = '0000000' AND d.DeptCode = 'SALES' AND d.PosCode = a.Position
	--LEFT JOIN gnMstLookUpDtl e ON e.CompanyCode = '0000000' AND e.CodeID = 'ITSG' AND e.LookUpValue = a.Grade
END

IF OBJECT_ID('tempdb..#emp_training') IS NOT NULL DROP TABLE #emp_training
IF OBJECT_ID('tempdb..#emp_all') IS NOT NULL DROP TABLE #emp_all
IF OBJECT_ID('tempdb..#tbl_filter') IS NOT NULL DROP TABLE #tbl_filter
IF OBJECT_ID('tempdb..#tbl_filter_t') IS NOT NULL DROP TABLE #tbl_filter_t
IF OBJECT_ID('tempdb..#resultTable') IS NOT NULL DROP TABLE #resultTable

