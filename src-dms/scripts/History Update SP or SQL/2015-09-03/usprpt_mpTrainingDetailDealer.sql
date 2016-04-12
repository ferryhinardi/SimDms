IF OBJECT_ID('[dbo].[usprpt_mpTrainingDetailDealer]') IS NOT NULL DROP PROCEDURE [dbo].[usprpt_mpTrainingDetailDealer]
GO

GO
/****** Object:  StoredProcedure [dbo].[usprpt_mpTrainingDetailDealer]    Script Date: 9/4/2015 10:20:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usprpt_mpTrainingDetailDealer]
-- Created By Benedict 10 Aug 2015 , Last Update By Benedict 3 Sept 2015
-- modified by Fhi 11082015
--DECLARE 
		--@areaGroup		VARCHAR(15),
		@DealerCode		VARCHAR(15),
		@OutletCode		VARCHAR(15),
		@Position		VARCHAR(4),
		@TrainingCode	VARCHAR(10),
		@FilterOutlet	VARCHAR(15),
		@FilterCol		VARCHAR(15)

AS

-- exec usprpt_mpTrainingDetail '', '', '', 'STDP1', '', ''
-- exec usprpt_mpTrainingDetail '', '', '', 'STDP1', '650040103', 'jml'
-- exec usprpt_mpTrainingDetail '', '', '', 'STDP1', '650040103', 't'
-- exec usprpt_mpTrainingDetail '', '', '', 'STDP1', '650040103', 'nt'

--SELECT @DealerCode = '6006400001',
--	   @OutletCode = '6006400131',
--	   @Position = '',
--	   @TrainingCode = 'STDP1',
--	   @FilterOutlet = '6006400131', --'650040103',
--	   @FilterCol = 'nt'			--'nt'

DECLARE @DealerOutlet TABLE
(
	--GroupNo		INT,
	DealerCode	VARCHAR(15),
	OutletCode	VARCHAR(15),
	OutletAbbr	VARCHAR(35)
)

INSERT INTO @DealerOutlet
SELECT a.DealerCode, a.OutletCode, a.OutletAbbreviation AS OutletAbbr
FROM gnMstDealerOutletMapping a
WHERE a.isActive = 1 
--and a.GroupNo= case @areaGroup when '' then a.GroupNo else @areaGroup end
AND a.DealerCode = CASE @DealerCode WHEN '' THEN a.DealerCode ELSE @DealerCode END
AND a.OutletCode = CASE @OutletCode WHEN '' THEN a.OutletCode ELSE @OutletCode END

DECLARE @tempTbl1 TABLE
(
	CompanyCode		VARCHAR(50),
	BranchCode		VARCHAR(50),
	EmployeeID		VARCHAR(50),
	Position		VARCHAR(50),
	Grade			VARCHAR(15),
	Positions		VARCHAR(50)
)

DECLARE @tempTbl2 TABLE
(
	CompanyCode		VARCHAR(50),
	BranchCode		VARCHAR(50),
	EmployeeID		VARCHAR(50),
	Position		VARCHAR(50),
	Grade			VARCHAR(15),
	TrainingCode	VARCHAR(25),
	Positions		VARCHAR(50)
)

DECLARE @emp_all_temp TABLE
(
	CompanyCode		VARCHAR(50),
	BranchCode		VARCHAR(50),
	EmployeeID		VARCHAR(50),
	Position		VARCHAR(50),
	Grade			VARCHAR(15),
	Positions		VARCHAR(50)
)

DECLARE @TrainingDetail_temp TABLE
(
	CompanyCode		VARCHAR(50),
	BranchCode		VARCHAR(50),
	EmployeeID		VARCHAR(50),
	Position		VARCHAR(50),
	Grade			VARCHAR(15),
	TrainingCode	VARCHAR(25),
	Positions		VARCHAR(50)
)

DECLARE @ResultTable TABLE
(
	OutletCode		VARCHAR(15),
	OutletAbbr		varchar(500),
	Jml				INT,
	T				INT,
	NT				AS (Jml - T)
)

;WITH _1 AS
(
	SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate FROM HrEmployeeMutation a
	GROUP BY a.CompanyCode, a.EmployeeID
), _2 AS
(
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, c.Position, c.Grade, 
			case when c.Position='S' then C.Position+C.Grade
				else c.Position
				end  Positions
	FROM _1 a 
	JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
	JOIN HrEmployee c ON a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID
	WHERE c.Department = 'SALES' AND c.PersonnelStatus = '1' AND (c.ResignDate IS NULL OR c.ResignDate <= c.JoinDate)
)
--SELECT * INTO #emp_all FROM _2 a 
--WHERE a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END 

insert into @tempTbl1
select * from _2

if (@Position='S0')
begin
	insert  INTO @emp_all_temp
	select * FROM @tempTbl1 a 
	WHERE a.Positions in ('S1','S2','S3','S4')
end
else
begin
	insert  INTO @emp_all_temp
	select * FROM @tempTbl1 a 
	WHERE a.Positions = CASE @Position 
						WHEN 'S1' THEN (select top 1 Positions from @tempTbl1 where Positions in ('S1'))
						WHEN 'S2' THEN (select top 1 Positions from @tempTbl1 where Positions in ('S2'))
						WHEN 'S3' THEN (select top 1 Positions from @tempTbl1 where Positions in ('S3'))
						WHEN 'S4' THEN (select top 1 Positions from @tempTbl1 where Positions in ('S4'))
						WHEN 'SH' THEN (select top 1 Positions from @tempTbl1 where Positions in ('SH'))
						WHEN 'BM' THEN (select top 1 Positions from @tempTbl1 where Positions in ('BM'))
						WHEN '' THEN a.Positions
						end
end

SELECT * INTO #emp_all FROM @emp_all_temp

;WITH _1 AS
(
	SELECT a.CompanyCode, a.EmployeeID, a.TrainingCode, MAX(b.MutationDate) MutationDate
	FROM HrEmployeeTraining a
	JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	GROUP BY a.CompanyCode, a.EmployeeID, a.TrainingCode
), _2 AS
(
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, c.Position, c.Grade, a.TrainingCode,
			case when c.Position='S' then C.Position+C.Grade
				else c.Position
				end  Positions
	FROM _1 a
	JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
	JOIN HrEmployee c ON a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID
	WHERE c.Department = 'SALES' AND c.PersonnelStatus = '1' AND (c.ResignDate IS NULL OR c.ResignDate <= c.JoinDate)
)
--SELECT * INTO #TrainingDetail FROM _2 a
--WHERE a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END 
--AND a.TrainingCode = CASE @TrainingCode WHEN '' THEN a.TrainingCode ELSE @TrainingCode END

insert into @tempTbl2
select * from _2

if (@Position='S0')
begin
	insert  INTO @TrainingDetail_temp
	select * FROM @tempTbl2 a 
	WHERE a.Positions in ('S1','S2','S3','S4')
	AND a.TrainingCode = CASE @TrainingCode WHEN '' THEN a.TrainingCode ELSE @TrainingCode END
end
else
begin
	insert  INTO @TrainingDetail_temp
	select * from @tempTbl2 a
	WHERE a.Positions = CASE @Position 
						WHEN 'S1' THEN (select top 1 Positions from @tempTbl2 where Positions in ('S1'))
						WHEN 'S2' THEN (select top 1 Positions from @tempTbl2 where Positions in ('S2'))
						WHEN 'S3' THEN (select top 1 Positions from @tempTbl2 where Positions in ('S3'))
						WHEN 'S4' THEN (select top 1 Positions from @tempTbl2 where Positions in ('S4'))
						WHEN 'SH' THEN (select top 1 Positions from @tempTbl2 where Positions in ('SH'))
						WHEN 'BM' THEN (select top 1 Positions from @tempTbl2 where Positions in ('BM'))
						WHEN '' THEN a.Positions 
						END 
	AND a.TrainingCode = CASE @TrainingCode WHEN '' THEN a.TrainingCode ELSE @TrainingCode END

end

SELECT * INTO #TrainingDetail FROM @TrainingDetail_temp

IF(@FilterOutlet = '' AND @FilterCol = '') BEGIN
	INSERT INTO @ResultTable
	SELECT a.OutletCode
		, a.OutletAbbr
		, (SELECT COUNT(*) FROM #emp_all b WHERE a.OutletCode = b.BranchCode) AS Jml
		, (SELECT COUNT(*) FROM #TrainingDetail b WHERE a.OutletCode = b.BranchCode) AS T
	FROM @DealerOutlet a
	
	SELECT 
		a.OutletCode
	  ,	a.OutletAbbr
	  ,	SUM(a.Jml) AS Jml
	  , SUM(a.T) AS T
	  , SUM(a.NT) AS NT
	FROM @ResultTable a
	group by a.OutletCode,a.OutletAbbr
END 
ELSE BEGIN
	SELECT * INTO #FilterTable FROM 
	(
		SELECT a.CompanyCode
			, a.BranchCode AS OutletCode
			, b.OutletAbbreviation AS OutletAbbr
			, a.EmployeeID
			, c.EmployeeName
			, a.Position
			, d.PosName
			, a.Grade
			, ISNULL(e.LookupValueName, '') AS GradeName
			, c.JoinDate
			, c.Gender
			, c.BirthDate
			, c.Email
			, (c.Telephone1 + ' / ' + c.Telephone2) AS Telephone
		FROM #emp_all a 
		INNER JOIN gnMstDealerOutletMapping b ON a.BranchCode = b.OutletCode
		INNER JOIN HrEmployee c ON a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID
		INNER JOIN gnMstPosition d ON d.CompanyCode = a.CompanyCode AND d.DeptCode = 'SALES' AND d.PosCode = a.Position
		LEFT JOIN gnMstLookUpDtl e ON e.CompanyCode = a.CompanyCode AND e.CodeID = 'ITSG' AND e.LookUpValue = a.Grade
		WHERE a.BranchCode = @FilterOutlet
	) #FilterTable
		
	IF(@FilterCol = 'jml') BEGIN
		SELECT * FROM #FilterTable a
	END ELSE IF (@FilterCol = 't') BEGIN
		SELECT * FROM #FilterTable a WHERE a.EmployeeID IN (SELECT b.EmployeeID FROM #TrainingDetail b WHERE b.TrainingCode = @TrainingCode)
	END 
	ELSE IF (@FilterCol = 'nt') BEGIN
		SELECT * FROM #FilterTable a WHERE a.EmployeeID NOT IN (SELECT EmployeeID FROM #TrainingDetail b WHERE b.TrainingCode = @TrainingCode) 
	END
END
IF OBJECT_ID('tempdb..#emp_all') IS NOT NULL DROP TABLE #emp_all
IF OBJECT_ID('tempdb..#TrainingDetail') IS NOT NULL DROP TABLE #TrainingDetail
IF OBJECT_ID('tempdb..#FilterTable') IS NOT NULL DROP TABLE #FilterTable

