
IF OBJECT_ID('usprpt_abInqTurnOverRatio') IS NOT NULL
	DROP PROCEDURE usprpt_abInqTurnOverRatio
GO
-- CREATED BY Benedict 15-Apr-2015 
-- LAST UPDATE BY Benedict 24-Apr-2015

CREATE PROCEDURE usprpt_abInqTurnOverRatio
	@DealerCode varchar(15),
	@OutletCode varchar(15),
	@Start date,
	@End date,
	@Position varchar(5)
AS BEGIN

--DECLARE
--	@DealerCode varchar(15) = '6006400001',
--	@OutletCode varchar(15) = '',
--	@Start date = '2013-06-30',
--	@End date = '2013-09-30',
--	@Position varchar(5) = ''

DECLARE @ITSG TABLE (Value int, Name varchar(15))
INSERT INTO @ITSG (Value, Name) 
	SELECT LookUpValue, LookUpValueName 
	FROM gnMstLookUpDtl WHERE CompanyCode = CASE @DealerCode WHEN '' THEN CompanyCode ELSE @DealerCode END AND CodeID = 'ITSG'

DECLARE @Platinum int = (SELECT Value FROM @ITSG WHERE Name = 'Platinum'),
	@Gold int = (SELECT Value FROM @ITSG WHERE Name = 'Gold'),
	@Silver int = (SELECT Value FROM @ITSG WHERE Name = 'Silver'),
	@Trainee int = (SELECT Value FROM @ITSG WHERE Name = 'Trainee')

SELECT * INTO #InPeriodMutationStart FROM (
		SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MIN(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @Start
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND c.JoinDate <= @Start 
		AND @Start < d.MutationDate
		AND (c.ResignDate IS NULL OR @Start < c.ResignDate)
		AND 1 = (SELECT CASE WHEN h.BranchCode <> d.BranchCode THEN 0 ELSE 1 END
					FROM HrEmployeeMutation h
					JOIN (SELECT g.CompanyCode, g.EmployeeID, MIN(g.MutationDate) AS MutationDate
							FROM HrEmployeeMutation g
							WHERE g.CompanyCode = d.CompanyCode
							AND g.EmployeeID = d.EmployeeID
							AND @Start < g.MutationDate
							GROUP BY g.CompanyCode, g.EmployeeID) i
					ON h.CompanyCode = i.CompanyCode
					AND h.EmployeeID = i.EmployeeID
					WHERE h.MutationDate = i.MutationDate
		)
		AND 1 = (SELECT CASE WHEN COUNT(*) > 1 THEN 0 ELSE 1 END
					FROM HrEmployeeMutation j
					WHERE j.CompanyCode = d.CompanyCode
					AND j.EmployeeID = d.EmployeeID
		)
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
) #InPeriodMutationStart

SELECT * INTO #LateMutationStart FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MIN(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @Start
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND c.JoinDate <= @Start 
		AND @End < d.MutationDate
		
		AND (c.ResignDate IS NULL OR @Start < c.ResignDate)
		AND 1 = (SELECT CASE WHEN h.BranchCode <> d.BranchCode THEN 0 ELSE 1 END
					FROM HrEmployeeMutation h
					JOIN (SELECT g.CompanyCode, g.EmployeeID, MIN(g.MutationDate) AS MutationDate
							FROM HrEmployeeMutation g
							WHERE g.CompanyCode = d.CompanyCode
							AND g.EmployeeID = d.EmployeeID
							AND @End < g.MutationDate
							GROUP BY g.CompanyCode, g.EmployeeID) i
					ON h.CompanyCode = i.CompanyCode
					AND h.EmployeeID = i.EmployeeID
					WHERE h.MutationDate = i.MutationDate
		)
		AND 1 = (SELECT CASE WHEN COUNT(*) > 1 THEN 0 ELSE 1 END
					FROM HrEmployeeMutation j
					WHERE j.CompanyCode = d.CompanyCode
					AND j.EmployeeID = d.EmployeeID
		)
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
) #LateMutationStart

SELECT * INTO #WrongResignStart FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MAX(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @Start
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND c.JoinDate <= @Start 
		AND d.MutationDate <= @Start
		AND 1 = (CASE WHEN c.ResignDate < @Start
					THEN (CASE WHEN e.AssignDate > c.ResignDate THEN 1 ELSE 0 END)
					END				
				)
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
) #WrongResignStart

SELECT * INTO #poolStart FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MAX(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @Start
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND c.JoinDate <= @Start 
		AND d.MutationDate <= @Start
		AND (c.ResignDate IS NULL OR @Start < c.ResignDate OR c.ResignDate <= c.JoinDate)
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
UNION
	SELECT * FROM #InPeriodMutationStart
UNION
	SELECT * FROM #LateMutationStart
UNION
	SELECT * FROM #WrongResignStart
)#poolStart

SELECT * INTO #LateMutationEnd FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate,
		e.AssignDate, MIN(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @End
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND c.JoinDate <= @End 
		AND d.MutationDate > @End
		AND (c.ResignDate IS NULL OR @End < c.ResignDate)
		AND 1 = (SELECT CASE WHEN h.BranchCode <> d.BranchCode THEN 0 ELSE 1 END
					FROM HrEmployeeMutation h
					JOIN (SELECT g.CompanyCode, g.EmployeeID, MIN(g.MutationDate) AS MutationDate
							FROM HrEmployeeMutation g
							WHERE g.CompanyCode = d.CompanyCode
							AND g.EmployeeID = d.EmployeeID
							AND g.MutationDate > @End
							GROUP BY g.CompanyCode, g.EmployeeID) i
					ON h.CompanyCode = i.CompanyCode
					AND h.EmployeeID = i.EmployeeID
					WHERE h.MutationDate = i.MutationDate
		)
		--CRITICAL WHERE, must be added to all Late Selects
		AND 1 = (SELECT CASE WHEN COUNT(*) > 1 THEN 0 ELSE 1 END
					FROM HrEmployeeMutation j
					WHERE j.CompanyCode = d.CompanyCode
					AND j.EmployeeID = d.EmployeeID
		)
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
) #LateMutationEnd 

SELECT * INTO #WrongResignEnd FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
	e.Position, e.Grade, 
	c.JoinDate,
	e.AssignDate, MAX(d.MutationDate) AS MutationDate, c.ResignDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
	JOIN (SELECT a.* 
			FROM HrEmployeeAchievement a
			INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
						FROM HrEmployeeAchievement f 
						WHERE f.AssignDate <= @End
						GROUP BY f.CompanyCode, f.EmployeeID) b
			ON a.CompanyCode = b.CompanyCode
			AND a.EmployeeID = b.EmployeeID
			AND a.AssignDate = b.AssignDate
			WHERE a.Department = 'SALES'
			) e
	ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
	WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
	AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
	AND c.Department = 'SALES' 
	AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
	AND c.JoinDate <= @End 
	AND d.MutationDate <= @End
	AND 1 = (CASE WHEN c.ResignDate < @End 
				THEN (CASE WHEN e.AssignDate > c.ResignDate THEN 1 ELSE 0 END)
				END				
			)
	GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
	c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
) #WrongResignEnd

SELECT * INTO #poolEnd FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate,
		e.AssignDate, MAX(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @End
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND c.JoinDate <= @End 
		AND d.MutationDate <= @End
		AND (c.ResignDate IS NULL OR @End < c.ResignDate OR c.ResignDate <= c.JoinDate)
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
UNION
	SELECT * FROM #LateMutationEnd
UNION
	SELECT * FROM #WrongResignEnd
)#poolEnd

SELECT * INTO #PromotionIn FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MAX(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @End
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE @Position <> ''
		AND @Position <> 'S'
		AND d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND d.MutationDate <= @End
		AND c.EmployeeID IN (
							SELECT b.EmployeeID
							FROM hrEmployeeAchievement b
							INNER JOIN (
										SELECT c.CompanyCode, c.EmployeeID, MAX(c.AssignDate) AS AssignDate
										FROM hrEmployeeAchievement c
										WHERE c.Position = @Position
											AND @Start < c.AssignDate
											AND c.AssignDate <= @End
										GROUP BY c.CompanyCode, c.EmployeeID
										) d
							ON b.CompanyCode = d.CompanyCode
							AND b.EmployeeID = d.EmployeeID
							AND b.AssignDate = d.AssignDate
							WHERE b.CompanyCode = CASE @DealerCode WHEN '' THEN b.CompanyCode ELSE @DealerCode END
							AND b.Department = 'SALES'
						)
		AND 1 = (SELECT CASE WHEN h.BranchCode <> d.BranchCode THEN 1 ELSE 0 END
					FROM HrEmployeeMutation h
					JOIN (SELECT g.CompanyCode, g.EmployeeID, MIN(g.MutationDate) AS MutationDate
							FROM HrEmployeeMutation g
							WHERE g.CompanyCode = d.CompanyCode
							AND g.EmployeeID = d.EmployeeID
							AND g.MutationDate <= @End
							GROUP BY g.CompanyCode, g.EmployeeID) i
					ON h.CompanyCode = i.CompanyCode
					AND h.EmployeeID = i.EmployeeID
					WHERE h.MutationDate = i.MutationDate
		)
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
) #PromotionIn

SELECT * INTO #EarlyAssignedIn FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MAX(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @End
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND d.MutationDate <= @End 
		AND e.AssignDate <= @Start 
		AND c.EmployeeID NOT IN (SELECT g.EmployeeID 
									FROM #poolStart g
									WHERE g.CompanyCode = d.CompanyCode
									AND g.BranchCode = d.BranchCode
									)
		AND (@Start < c.ResignDate OR c.ResignDate IS NULL)
		AND (d.MutationDate < c.ResignDate OR c.ResignDate IS NULL)
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
) #EarlyAssignedIn

SELECT * INTO #LateAssignedIn FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MAX(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @End
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND d.MutationDate <= @End 
		AND (@Start < e.AssignDate AND e.AssignDate <= @End)
		AND c.EmployeeID NOT IN (SELECT g.EmployeeID 
									FROM #poolStart g
									WHERE g.CompanyCode = d.CompanyCode
									AND g.BranchCode = d.BranchCode
									)
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
) #LateAssignedIn 

SELECT * INTO #LateMutationIn FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MIN(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @End
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND (@Start < c.JoinDate AND c.JoinDate <= @End)
		AND d.MutationDate > @End
		AND 1 = (SELECT CASE WHEN h.BranchCode <> d.BranchCode THEN 0 ELSE 1 END
					FROM HrEmployeeMutation h
					JOIN (SELECT g.CompanyCode, g.EmployeeID, MIN(g.MutationDate) AS MutationDate
							FROM HrEmployeeMutation g
							WHERE g.CompanyCode = d.CompanyCode
							AND g.EmployeeID = d.EmployeeID
							AND g.MutationDate > @End
							GROUP BY g.CompanyCode, g.EmployeeID) i
					ON h.CompanyCode = i.CompanyCode
					AND h.EmployeeID = i.EmployeeID
					WHERE h.MutationDate = i.MutationDate
		)
		AND 1 = (SELECT CASE WHEN COUNT(*) > 1 THEN 0 ELSE 1 END
					FROM HrEmployeeMutation j
					WHERE j.CompanyCode = d.CompanyCode
					AND j.EmployeeID = d.EmployeeID
		)
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
) #LateMutationIn

SELECT * INTO #employeeStay FROM(
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MAX(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @End
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND c.JoinDate <= @Start
		AND (c.ResignDate IS NULL OR @End < c.ResignDate)
		AND d.MutationDate <= @Start
		AND c.EmployeeID NOT IN (SELECT EmployeeID FROM #PromotionIn)
		AND c.EmployeeID NOT IN (SELECT EmployeeID FROM #LateAssignedIn)
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
)#employeeStay

SELECT * INTO #employeeIn FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MAX(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @End
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND @Start < c.JoinDate 
		AND c.JoinDate <= @End
		AND @Start < d.MutationDate
		AND d.MutationDate <= @End
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
UNION
	SELECT * FROM #PromotionIn
UNION
	SELECT * FROM #EarlyAssignedIn
UNION
	SELECT * FROM #LateAssignedIn
UNION
	SELECT * FROM #LateMutationIn
) #employeeIn

SELECT * INTO #poolStartEmployeeIn FROM (
	SELECT * FROM #poolStart
UNION 
	SELECT * FROM #employeeIn
) #poolStartEmployeeIn

SELECT * INTO #PromotionOut FROM (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeId, a.EmployeeName, 
		a.Position, a.Grade, 
		a.JoinDate, a.AssignDate, a.MutationDate, a.ResignDate
		FROM #poolStartEmployeeIn a
		WHERE @Position <> ''
		AND a.EmployeeID IN (
							SELECT b.EmployeeID
							FROM HrEmployeeAchievement b
							INNER JOIN (
											SELECT c.CompanyCode, c.EmployeeID, MAX(c.AssignDate) AS AssignDate
											FROM HrEmployeeAchievement c
											WHERE 
											@Start < c.AssignDate
											AND c.AssignDate <= @End
											GROUP BY c.CompanyCode, c.EmployeeID
										) d
							ON b.CompanyCode = d.CompanyCode
							AND b.EmployeeID = d.EmployeeID
							AND b.AssignDate = d.AssignDate
							WHERE b.CompanyCode = CASE @DealerCode WHEN '' THEN b.CompanyCode ELSE @DealerCode END
							AND b.Department = 'SALES'
							AND b.Position <> @Position
							)
) #PromotionOut

SELECT * INTO #LateMutationOut FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MIN(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @End
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND @Start < c.ResignDate 
		AND c.ResignDate <= @End
		AND d.MutationDate > @End
		AND 1 = (SELECT CASE WHEN h.BranchCode <> d.BranchCode THEN 0 ELSE 1 END
					FROM HrEmployeeMutation h
					JOIN (SELECT g.CompanyCode, g.EmployeeID, MIN(g.MutationDate) AS MutationDate
							FROM HrEmployeeMutation g
							WHERE g.CompanyCode = d.CompanyCode
							AND g.EmployeeID = d.EmployeeID
							AND g.MutationDate > @End
							GROUP BY g.CompanyCode, g.EmployeeID) i
					ON h.CompanyCode = i.CompanyCode
					AND h.EmployeeID = i.EmployeeID
					WHERE h.MutationDate = i.MutationDate
		)
		AND 1 = (SELECT CASE WHEN COUNT(*) > 1 THEN 0 ELSE 1 END
					FROM HrEmployeeMutation j
					WHERE j.CompanyCode = d.CompanyCode
					AND j.EmployeeID = d.EmployeeID
		)
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
) #LateMutationOut

SELECT * INTO #employeeOut FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MAX(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @End
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE a.Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND e.Position = CASE @Position WHEN '' THEN e.Position ELSE @Position END
		AND @Start < c.ResignDate 
		AND c.ResignDate <= @End
		AND d.MutationDate <= @End
		AND c.JoinDate < c.ResignDate
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
UNION
	SELECT * FROM #PromotionOut
UNION
	SELECT * FROM #LateMutationOut
) #employeeOut

DECLARE @sum1 TABLE (CompanyCode varchar(15), BranchCode varchar(15), CountPoolStart int)
INSERT INTO @sum1 (CompanyCode, BranchCode, CountPoolStart)
	SELECT a.DealerCode,
		a.OutletCode, 
		(SELECT COUNT(*) 
			FROM #poolStart b 
			WHERE b.CompanyCode = a.DealerCode
			AND b.BranchCode = a.OutletCode
		)
	FROM gnMstDealerOutletMapping a
	WHERE a.IsActive = 1
	AND a.DealerCode = CASE @DealerCode WHEN '' THEN a.DealerCode ELSE @DealerCode END
	AND a.OutletCode = CASE @OutletCode WHEN '' THEN a.OutletCode ELSE @OutletCode END

SELECT * INTO #sum1 FROM(
	SELECT 
		b.CompanyCode, 
		b.BranchCode,
		COUNT(*) AS EmployeeCount,
		SUM(CASE a.Grade WHEN @Platinum THEN 1 ELSE 0 END) AS Platinum,
		SUM(CASE a.Grade WHEN @Gold THEN 1 ELSE 0 END) AS Gold,
		SUM(CASE a.Grade WHEN @Silver THEN 1 ELSE 0 END) AS Silver,
		SUM(CASE a.Grade WHEN @Trainee THEN 1 ELSE 0 END) AS Trainee
	FROM @sum1 b
	JOIN #poolStart a
	ON a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	WHERE b.CountPoolStart > 0
	GROUP BY b.CompanyCode, b.BranchCode
UNION ALL 
	SELECT 
		b.CompanyCode,
		b.BranchCode,
		0 AS EmployeeCount,
		0 AS Platinum,
		0 AS Gold,
		0 AS Silver,
		0 AS Trainee
	FROM @sum1 b
	WHERE b.CountPoolStart = 0
) #sum1

DECLARE @sum2 TABLE (CompanyCode varchar(15), BranchCode varchar(15), CountPoolEnd int)
INSERT INTO @sum2 (CompanyCode, BranchCode, CountPoolEnd)
	SELECT a.DealerCode,
		a.OutletCode, 
		(SELECT COUNT(*) 
			FROM #poolEnd b 
			WHERE b.CompanyCode = a.DealerCode
			AND b.BranchCode = a.OutletCode
		)
	FROM gnMstDealerOutletMapping a
	WHERE a.IsActive = 1
	AND a.DealerCode = CASE @DealerCode WHEN '' THEN a.DealerCode ELSE @DealerCode END
	AND a.OutletCode = CASE @OutletCode WHEN '' THEN a.OutletCode ELSE @OutletCode END

SELECT * INTO #sum2 FROM (
	SELECT 
		b.CompanyCode, 
		b.BranchCode,
		COUNT(*) AS EmployeeCount,
		SUM(CASE a.Grade WHEN @Platinum THEN 1 ELSE 0 END) AS Platinum,
		SUM(CASE a.Grade WHEN @Gold THEN 1 ELSE 0 END) AS Gold,
		SUM(CASE a.Grade WHEN @Silver THEN 1 ELSE 0 END) AS Silver,
		SUM(CASE a.Grade WHEN @Trainee THEN 1 ELSE 0 END) AS Trainee
	FROM @sum2 b
	JOIN #poolEnd a
	ON a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	WHERE b.CountPoolEnd > 0
	GROUP BY b.CompanyCode, b.BranchCode
UNION ALL 
	SELECT 
		b.CompanyCode,
		b.BranchCode,
		0 AS EmployeeCount,
		0 AS Platinum,
		0 AS Gold,
		0 AS Silver,
		0 AS Trainee
	FROM @sum2 b
	WHERE b.CountPoolEnd = 0
) #sum2

SELECT * INTO #result FROM (
	SELECT 
		a.CompanyCode, 
		a.BranchCode,
		c.DealerAbbreviation, 
		d.OutletAbbreviation, 
			ISNULL(
					(CONVERT(decimal(6,2), a.EmployeeCount) - CONVERT(decimal(6,2), (SELECT COUNT(*) 
											FROM #employeeStay e
											WHERE e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode)))
					/ NULLIF(CONVERT(decimal(6,2), a.EmployeeCount), 0)
					, 0)
		AS Ratio, 
		a.EmployeeCount AS StartEmployeeCount,
			(SELECT COUNT(*) FROM #employeeIn f WHERE f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode)
		AS EmployeeIn,
		a.Platinum AS StartPlatinum, 
		a.Gold AS StartGold, 
		a.Silver AS StartSilver, 
		a.Trainee AS StartTrainee,
			(SELECT COUNT(*) FROM #employeeStay h WHERE h.CompanyCode = a.CompanyCode AND h.BranchCode = a.BranchCode)
		AS LoyalCount,
		b.EmployeeCount AS EndEmployeeCount,
			(SELECT COUNT(*) FROM #employeeOut g WHERE g.CompanyCode = a.CompanyCode AND g.BranchCode = a.BranchCode) 
		AS EmployeeOut,
		b.Platinum AS EndPlatinum, 
		b.Gold AS EndGold, 
		b.Silver AS EndSilver, 
		b.Trainee AS EndTrainee
	FROM #sum1 a
	JOIN #sum2 b 
		ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
	JOIN gnMstDealerMapping c 
		ON c.DealerCode = a.CompanyCode
	JOIN gnMstDealerOutletMapping d 
		ON d.DealerCode = a.CompanyCode AND d.OutletCode = a.BranchCode
)#result

--SELECT * FROM #InPeriodMutationStart
--SELECT * FROM #LateMutationStart 
--SELECT * FROM #WrongResignStart
--SELECT * FROM #poolStart 
--SELECT * FROM #WrongResignEnd
--SELECT * FROM #LateMutationEnd 
--SELECT * FROM #poolEnd 
--SELECT * FROM #promotionIn 
--SELECT * FROM #EarlyAssignedIn
--SELECT * FROM #LateAssignedIn 
--SELECT * FROM #LateMutationIn 
--SELECT * FROM #employeeStay 
--SELECT * FROM #employeeIn
--SELECT * FROM #poolStartEmployeeIn 
--SELECT * FROM #PromotionOut 
--SELECT * FROM #LateMutationOut
--SELECT * FROM #employeeOut 
--SELECT * FROM #sum1
--SELECT * FROM #sum2
SELECT * FROM #result

--WHERE 1=1
--AND BranchCode = '6006400000' 
----AND EmployeeID NOT IN (SELECT EmployeeID FROM #EmployeeIn)
----AND EmployeeID IN ('408','453')
--ORDER BY EmployeeID

--Cari ID yg hilang:
--SELECT * FROM #poolStartEmployeeIn a WHERE a.EmployeeID NOT IN (SELECT EmployeeID FROM #EmployeeOut UNION SELECT EmployeeID FROM #poolEnd)

--SELECT EmployeeID, EmployeeName, Position, JoinDate, ResignDate, IsDeleted FROM HrEmployee WHERE EmployeeID IN ('51666')
--SELECT * FROM hrEmployeeMutation WHERE EmployeeID IN ('51666')
--SELECT * FROM HrEmployeeAchievement WHERE EmployeeID IN ('51666')

DROP TABLE #InPeriodMutationStart
DROP TABLE #LateMutationStart
DROP TABLE #WrongResignStart
DROP TABLE #poolStart
DROP TABLE #WrongResignEnd
DROP TABLE #LateMutationEnd
DROP TABLE #poolEnd
DROP TABLE #poolStartEmployeeIn
DROP TABLE #EarlyAssignedIn 
DROP TABLE #LateAssignedIn
DROP TABLE #LateMutationIn
DROP TABLE #employeeStay
DROP TABLE #employeeIn
DROP TABLE #PromotionIn
DROP TABLE #PromotionOut
DROP TABLE #LateMutationOut
DROP TABLE #employeeOut
DROP TABLE #sum1
DROP TABLE #sum2
DROP TABLE #result

END
GO