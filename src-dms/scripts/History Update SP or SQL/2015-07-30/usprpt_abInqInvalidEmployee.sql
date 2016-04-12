IF OBJECT_ID('[dbo].[usprpt_abInqInvalidEmployee]') IS NOT NULL DROP PROCEDURE [dbo].[usprpt_abInqInvalidEmployee] GO


--CREATED BY Benedict 19-May-2015 LAST UPDATE 20-May-2015
CREATE PROCEDURE usprpt_abInqInvalidEmployee
		@DealerCode varchar(15),
		@OutletCode varchar(15),
		@Status varchar(15),
		@CaseNo int
AS BEGIN

--DECLARE @DealerCode varchar(15) = '',
--		@OutletCode varchar(15) = '',
--		@Status varchar(15) = '1',
--		@CaseNo int = 7

IF(@CaseNo = 1) BEGIN
-- ResignDate <= JoinDate
	;WITH _1 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate
		FROM HrEmployeeMutation a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		GROUP BY a.CompanyCode, a.EmployeeID
	), _2 AS (
		SELECT a.CompanyCode, a.EmployeeID, a.MutationDate, b.BranchCode
		FROM _1 a
		INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
	), _3 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.AssignDate) AS AssignDate
		FROM HrEmployeeAchievement a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		AND a.Department = 'SALES'
		GROUP BY a.CompanyCode, a.EmployeeID
	), _4 AS (
		SELECT a.CompanyCode, a.LookupValue, a.LookupValueName
		FROM gnMstLookupDtl a
		WHERE a.CompanyCode = '0000000' AND a.CodeID = 'PERS'
	), _5 AS (
		SELECT a.CompanyCode, b.BranchCode, e.OutletAbbreviation, a.EmployeeID, a.EmployeeName, a.Department, a.Position, a.Grade, a.JoinDate, b.MutationDate, c.AssignDate, a.ResignDate, d.LookupValueName AS PersonnelStatus
		FROM HrEmployee a
		INNER JOIN _2 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
		INNER JOIN _3 c ON a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID
		INNER JOIN _4 d ON a.PersonnelStatus = d.LookupValue
		INNER JOIN gnMstDealerOutletMapping e ON a.CompanyCode = e.DealerCode AND b.BranchCode = e.OutletCode
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		AND a.Department = 'SALES' AND CONVERT(date, a.ResignDate) <= CONVERT(date, a.JoinDate)
		AND a.PersonnelStatus = CASE @Status WHEN '' THEN a.PersonnelStatus ELSE @Status END
	)
	SELECT * FROM _5
	WHERE BranchCode = CASE @OutletCode WHEN '' THEN BranchCode ELSE @OutletCode END	
END

ELSE IF(@CaseNo = 2) BEGIN
--(MIN)MutationDate < JoinDate
	;WITH _1 AS (
		SELECT a.CompanyCode, a.EmployeeID, MIN(a.MutationDate) AS MutationDate
		FROM HrEmployeeMutation a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		GROUP BY a.CompanyCode, a.EmployeeID
	), _2 AS (
		SELECT a.CompanyCode, a.EmployeeID, a.MutationDate, b.BranchCode
		FROM _1 a
		INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
	), _3 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.AssignDate) AS AssignDate
		FROM HrEmployeeAchievement a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		AND a.Department = 'SALES'
		GROUP BY a.CompanyCode, a.EmployeeID
	), _4 AS (
		SELECT a.LookupValue, a.LookupValueName
		FROM gnMstLookupDtl a
		WHERE a.CompanyCode = '0000000' AND a.CodeID = 'PERS'
	), _5 AS (
		SELECT a.CompanyCode, b.BranchCode, e.OutletAbbreviation, a.EmployeeID, a.EmployeeName, a.Department, a.Position, a.Grade, a.JoinDate, b.MutationDate, c.AssignDate, a.ResignDate, d.LookupValueName AS PersonnelStatus
		FROM HrEmployee a
		INNER JOIN _2 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
		INNER JOIN _3 c ON a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID
		INNER JOIN _4 d ON a.PersonnelStatus = d.LookupValue
		INNER JOIN gnMstDealerOutletMapping e ON a.CompanyCode = e.DealerCode AND b.BranchCode = e.OutletCode
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		AND a.Department = 'SALES' AND CONVERT(date, b.MutationDate) < CONVERT(date, a.JoinDate)
		AND a.PersonnelStatus = CASE @Status WHEN '' THEN a.PersonnelStatus ELSE @Status END
	)
	SELECT * FROM _5
	WHERE BranchCode = CASE @OutletCode WHEN '' THEN BranchCode ELSE @OutletCode END
END

ELSE IF(@CaseNo = 3) BEGIN 
--(MIN)AssignDate < JoinDate
	;WITH _1 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate
		FROM HrEmployeeMutation a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		GROUP BY a.CompanyCode, a.EmployeeID
	), _2 AS (
		SELECT a.CompanyCode, a.EmployeeID, a.MutationDate, b.BranchCode
		FROM _1 a
		INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
	), _3 AS (
		SELECT a.CompanyCode, a.EmployeeID, MIN(a.AssignDate) AS AssignDate
		FROM HrEmployeeAchievement a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END 
		AND a.Department = 'SALES'
		GROUP BY a.CompanyCode, a.EmployeeID
	), _4 AS (
		SELECT a.LookupValue, a.LookupValueName
		FROM gnMstLookupDtl a
		WHERE a.CompanyCode = '0000000' AND a.CodeID = 'PERS'
	), _5 AS (
		SELECT a.CompanyCode, b.BranchCode, e.OutletAbbreviation, a.EmployeeID, a.EmployeeName, a.Department, a.Position, a.Grade, a.JoinDate, b.MutationDate, c.AssignDate, a.ResignDate, d.LookupValueName AS PersonnelStatus
		FROM HrEmployee a
		INNER JOIN _2 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
		INNER JOIN _3 c ON a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID
		INNER JOIN _4 d ON a.PersonnelStatus = d.LookupValue
		INNER JOIN gnMstDealerOutletMapping e ON a.CompanyCode = e.DealerCode AND b.BranchCode = e.OutletCode
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END 
		AND a.Department = 'SALES' AND CONVERT(date, c.AssignDate) < CONVERT(date, a.JoinDate)
		AND a.PersonnelStatus = CASE @Status WHEN '' THEN a.PersonnelStatus ELSE @Status END
	)
	SELECT * FROM _5
	WHERE BranchCode = CASE @OutletCode WHEN '' THEN BranchCode ELSE @OutletCode END
END

ELSE IF(@CaseNo = 4) BEGIN 
-- ResignDate < (MAX)MutationDate
	;WITH _1 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate
		FROM HrEmployeeMutation a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		GROUP BY a.CompanyCode, a.EmployeeID
	), _2 AS (
		SELECT a.CompanyCode, a.EmployeeID, a.MutationDate, b.BranchCode
		FROM _1 a
		INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
	), _3 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.AssignDate) AS AssignDate
		FROM HrEmployeeAchievement a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		AND a.Department = 'SALES'
		GROUP BY a.CompanyCode, a.EmployeeID
	), _4 AS (
		SELECT a.LookupValue, a.LookupValueName
		FROM gnMstLookupDtl a
		WHERE a.CompanyCode = '0000000' AND a.CodeID = 'PERS'
	), _5 AS (
		SELECT a.CompanyCode, b.BranchCode, e.OutletAbbreviation, a.EmployeeID, a.EmployeeName, a.Department, a.Position, a.Grade, a.JoinDate, b.MutationDate, c.AssignDate, a.ResignDate, d.LookupValueName AS PersonnelStatus
		FROM HrEmployee a
		INNER JOIN _2 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
		INNER JOIN _3 c ON a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID
		INNER JOIN _4 d ON a.PersonnelStatus = d.LookupValue
		INNER JOIN gnMstDealerOutletMapping e ON a.CompanyCode = e.DealerCode AND b.BranchCode = e.OutletCode
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		AND a.Department = 'SALES' AND CONVERT(date, a.ResignDate) < CONVERT(date, b.MutationDate)
		AND a.PersonnelStatus = CASE @Status WHEN '' THEN a.PersonnelStatus ELSE @Status END
	)
	SELECT * FROM _5
	WHERE BranchCode = CASE @OutletCode WHEN '' THEN BranchCode ELSE @OutletCode END
END

ELSE IF(@CaseNo = 5) BEGIN 
-- ResignDate < (MAX)AssignDate
	;WITH _1 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate
		FROM HrEmployeeMutation a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		GROUP BY a.CompanyCode, a.EmployeeID
	), _2 AS (
		SELECT a.CompanyCode, a.EmployeeID, a.MutationDate, b.BranchCode
		FROM _1 a
		INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
	), _3 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.AssignDate) AS AssignDate
		FROM HrEmployeeAchievement a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		AND a.Department = 'SALES'
		GROUP BY a.CompanyCode, a.EmployeeID
	), _4 AS (
		SELECT a.LookupValue, a.LookupValueName
		FROM gnMstLookupDtl a
		WHERE a.CompanyCode = '0000000' AND a.CodeID = 'PERS'
	), _5 AS (
		SELECT a.CompanyCode, b.BranchCode, e.OutletAbbreviation, a.EmployeeID, a.EmployeeName, a.Department, a.Position, a.Grade, a.JoinDate, b.MutationDate, c.AssignDate, a.ResignDate, d.LookupValueName AS PersonnelStatus
		FROM HrEmployee a
		INNER JOIN _2 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
		INNER JOIN _3 c ON a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID
		INNER JOIN _4 d ON a.PersonnelStatus = d.LookupValue
		INNER JOIN gnMstDealerOutletMapping e ON a.CompanyCode = e.DealerCode AND b.BranchCode = e.OutletCode
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		AND a.Department = 'SALES' AND CONVERT(date, a.ResignDate) < CONVERT(date, c.AssignDate)
		AND a.PersonnelStatus = CASE @Status WHEN '' THEN a.PersonnelStatus ELSE @Status END
	)
	SELECT * FROM _5
	WHERE BranchCode = CASE @OutletCode WHEN '' THEN BranchCode ELSE @OutletCode END
END

ELSE IF(@CaseNo = 6) BEGIN 
-- MutationDate = NULL (No Branch)
	;WITH _1 AS (
		SELECT a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Department, a.Position, a.Grade, a.JoinDate, a.ResignDate, a.PersonnelStatus
		FROM HrEmployee a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		AND a.Department = 'SALES'
		AND a.PersonnelStatus = CASE @Status WHEN '' THEN a.PersonnelStatus ELSE @Status END
	), _2 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate
		FROM HrEmployeeMutation a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		GROUP BY a.CompanyCode, a.EmployeeID
	), _3 AS (
		SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
		FROM _2 a
		INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
	), _4 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.AssignDate) AS AssignDate
		FROM HrEmployeeAchievement a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		AND a.Department = 'SALES'
		GROUP BY a.CompanyCode, a.EmployeeID
	), _5 AS (
		SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.EmployeeName, a.Department, a.Position, a.Grade, a.JoinDate, b.MutationDate, a.ResignDate, a.PersonnelStatus
		FROM _1 a
		LEFT JOIN _3 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
		WHERE b.BranchCode IS NULL
	), _6 AS (
		SELECT a.LookupValue, a.LookupValueName
		FROM gnMstLookupDtl a
		WHERE a.CompanyCode = '0000000' AND a.CodeID = 'PERS'
	), _7 AS (
		SELECT a.CompanyCode, a.BranchCode, d.OutletAbbreviation, a.EmployeeID, a.EmployeeName, a.Department, a.Position, a.Grade, a.JoinDate, a.MutationDate, b.AssignDate, a.ResignDate, c.LookUpValueName AS PersonnelStatus
		FROM _5 a
		INNER JOIN _4 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
		INNER JOIN _6 c ON a.PersonnelStatus = c.LookUpValue
		INNER JOIN gnMstDealerOutletMapping d ON a.CompanyCode = d.DealerCode AND a.BranchCode = d.OutletCode
	)
	SELECT * FROM _7
	WHERE BranchCode = CASE @OutletCode WHEN '' THEN BranchCode ELSE @OutletCode END
END

ELSE IF(@CaseNo = 7) BEGIN
-- AssignDate = NULL
	;WITH _1 AS (
		SELECT a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Department, a.Position, a.Grade, a.JoinDate, a.ResignDate, a.PersonnelStatus
		FROM HrEmployee a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		AND a.Department = 'SALES'
		AND a.PersonnelStatus = CASE @Status WHEN '' THEN a.PersonnelStatus ELSE @Status END
	), _2 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate
		FROM HrEmployeeMutation a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		GROUP BY a.CompanyCode, a.EmployeeID
	), _3 AS (
		SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
		FROM _2 a
		INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
	), _4 AS (
		SELECT a.CompanyCode, a.EmployeeID, MAX(a.AssignDate) AS AssignDate
		FROM HrEmployeeAchievement a
		WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
		AND a.Department = 'SALES'
		GROUP BY a.CompanyCode, a.EmployeeID
	), _5 AS (
		SELECT a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Department, a.Position, a.Grade, a.JoinDate, b.AssignDate, a.ResignDate, a.PersonnelStatus
		FROM _1 a
		LEFT JOIN _4 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
		WHERE b.AssignDate IS NULL
	), _6 AS (
		SELECT a.LookupValue, a.LookupValueName
		FROM gnMstLookupDtl a
		WHERE a.CompanyCode = '0000000' AND a.CodeID = 'PERS'
	), _7 AS (
		SELECT a.CompanyCode, b.BranchCode, d.OutletAbbreviation, a.EmployeeID, a.EmployeeName, a.Department, a.Position, a.Grade, a.JoinDate, b.MutationDate, a.AssignDate, a.ResignDate, c.LookupValueName AS PersonnelStatus
		FROM _5 a
		INNER JOIN _3 b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
		INNER JOIN _6 c ON a.PersonnelStatus = c.LookupValue
		INNER JOIN gnMstDealerOutletMapping d ON a.CompanyCode = d.DealerCode AND b.BranchCode = d.OutletCode
	)
	SELECT * FROM _7
	WHERE BranchCode = CASE @OutletCode WHEN '' THEN BranchCode ELSE @OutletCode END
END

END