IF OBJECT_ID('[dbo].[usprpt_abInqTurnOverRatio]') IS NOT NULL
	DROP PROCEDURE [usprpt_abInqTurnOverRatio]
GO

-- CREATED BY Benedict 15-Apr-2015 
-- LAST UPDATE BY Benedict 12-May-2015

CREATE PROCEDURE [dbo].[usprpt_abInqTurnOverRatio]
	@DealerCode varchar(15),
	@OutletCode varchar(15),
	@Start date,
	@End date,
	@Position varchar(5)
AS BEGIN

SET NOCOUNT ON
--DECLARE
--	@DealerCode varchar(15) = '6006400001',
--	@OutletCode varchar(15) = '',
--	@Start date = '2012-06-30',
--	@End date = '2013-06-30',
--	@Position varchar(5) = ''

BEGIN---#POOL START#---
/*
Hanya memilih yang JoinDate <= AssignDate & JoinDate <= MutationDate
apabila AssignDate/MutationDate < JoinDate, maka tidak valid (tidak termasuk)
*/

;WITH _1A AS (
	SELECT a.CompanyCode, a.EmployeeID, MAX(a.AssignDate) AS AssignDate
	FROM HrEmployeeAchievement a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND CONVERT(date, a.AssignDate) <= @Start
	AND a.Department = 'SALES'
	GROUP BY a.CompanyCode, a.EmployeeID
), _1B AS (
	SELECT a.CompanyCode, a.EmployeeID, MIN(a.AssignDate) AS AssignDate
	FROM HrEmployeeAchievement a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND @Start < CONVERT(date, a.AssignDate)
	AND a.Department = 'SALES'
	GROUP BY a.CompanyCode, a.EmployeeID
), _2A AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, a.AssignDate
	FROM _1A a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _2B AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, a.AssignDate
	FROM _1B a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _3A AS (
	SELECT a.CompanyCode, a.EmployeeID, MutationDate = MAX(a.MutationDate)
	FROM HrEmployeeMutation a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND CONVERT(date, a.MutationDate) <= @Start
	GROUP BY a.CompanyCode, a.EmployeeID
), _3B AS (
	SELECT a.CompanyCode, a.EmployeeID, MutationDate = MIN(a.MutationDate)
	FROM HrEmployeeMutation a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND @Start < CONVERT(date, a.MutationDate)
	GROUP BY a.CompanyCode, a.EmployeeID
), _4A AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _3A a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _4B AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _3B a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _5 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Position
	, CASE WHEN a.Position <> 'S' THEN '' ELSE (CASE a.Grade WHEN '' THEN '1' ELSE ISNULL(a.Grade, '1') END) END AS Grade
	, a.JoinDate, a.ResignDate 
	FROM HrEmployee a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
	AND CONVERT(date, a.JoinDate) <= @Start
	AND (@Start < a.ResignDate OR a.ResignDate IS NULL OR a.ResignDate <= a.JoinDate)
), _6 AS (
	SELECT a.CompanyCode
	, BranchCode = ISNULL(b.BranchCode, (SELECT c.BranchCode FROM _4B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID))
	, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate
	, MutationDate = ISNULL(b.MutationDate, (SELECT c.MutationDate FROM _4B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID))
	, a.ResignDate
	FROM _5 a
	LEFT JOIN _4A b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
), _7 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName
	, ISNULL(b.Position, ISNULL((SELECT c.Position FROM _2B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID), a.Position)) AS Position
	, ISNULL(b.Grade, ISNULL((SELECT c.Grade FROM _2B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID), a.Grade)) AS Grade
	, a.JoinDate
	, ISNULL(b.AssignDate, (SELECT c.AssignDate FROM _2B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID)) AS AssignDate
	, a.MutationDate, a.ResignDate
	FROM _6 a
	LEFT JOIN  _2A b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
)
SELECT * INTO #JoinFirstStart FROM (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
	FROM _7 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
	AND a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END
	AND a.Position IS NOT NULL
) #JoinFirstStart

SELECT * INTO #PoolStart FROM(
	SELECT * FROM #JoinFirstStart
) #PoolStart
END

BEGIN---#POOL END#---
/*
Hanya memilih yang JoinDate <= AssignDate & JoinDate <= MutationDate
apabila AssignDate/MutationDate < JoinDate, maka tidak valid (tidak termasuk)
*/

;WITH _1A AS (
	SELECT a.CompanyCode, a.EmployeeID, MAX(a.AssignDate) AS AssignDate
	FROM HrEmployeeAchievement a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND CONVERT(date, a.AssignDate) <= @End
	AND a.Department = 'SALES'
	GROUP BY a.CompanyCode, a.EmployeeID
), _1B AS (
	SELECT a.CompanyCode, a.EmployeeID, MIN(a.AssignDate) AS AssignDate
	FROM HrEmployeeAchievement a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND @End < CONVERT(date, a.AssignDate)
	AND a.Department = 'SALES'
	GROUP BY a.CompanyCode, a.EmployeeID
), _2A AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, a.AssignDate
	FROM _1A a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _2B AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, a.AssignDate
	FROM _1B a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _3A AS (
	SELECT a.CompanyCode, a.EmployeeID, MutationDate = MAX(a.MutationDate)
	FROM HrEmployeeMutation a
	WHERE CONVERT(date, a.MutationDate) <= @End
	GROUP BY a.CompanyCode, a.EmployeeID
), _3B AS(
	SELECT a.CompanyCode, a.EmployeeID, MutationDate = MIN(a.MutationDate)
	FROM HrEmployeeMutation a
	WHERE @End < CONVERT(date, a.MutationDate)
	GROUP BY a.CompanyCode, a.EmployeeID
), _4A AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _3A a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _4B AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _3B a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _5 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Position
	, CASE WHEN a.Position <> 'S' THEN '' ELSE (CASE a.Grade WHEN '' THEN '1' ELSE ISNULL(a.Grade, '1') END) END AS Grade
	, a.JoinDate, a.ResignDate 
	FROM HrEmployee a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
	AND CONVERT(date, a.JoinDate) <= @End
	AND (@End < CONVERT(date, a.ResignDate) OR CONVERT(date, a.ResignDate) IS NULL OR CONVERT(date, a.ResignDate) <= CONVERT(date, a.JoinDate))
), _6 AS (
	SELECT a.CompanyCode
	, BranchCode = ISNULL(b.BranchCode, (SELECT c.BranchCode FROM _4B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID))
	, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate
	, MutationDate = ISNULL(b.MutationDate, (SELECT c.MutationDate FROM _4B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID))
	, a.ResignDate
	FROM _5 a
	LEFT JOIN _4A b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
), _7 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID
	, ISNULL(b.Position, ISNULL((SELECT c.Position FROM _2B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID), a.Position)) AS Position
	, ISNULL(b.Grade, ISNULL((SELECT c.Grade FROM _2B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID), a.Grade)) AS Grade
	FROM _6 a
	LEFT JOIN  _2A b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
)
SELECT * INTO #JoinFirstEnd FROM (
	SELECT *
	FROM _7 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
	AND a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END
	AND a.Position IS NOT NULL
) #JoinFirstEnd

SELECT * INTO #PoolEnd FROM (
	SELECT * FROM #JoinFirstEnd
) #PoolEnd
END

BEGIN---#EMPLOYEE IN#---
/*
Hanya memilih yang JoinDate <= AssignDate & JoinDate <= MutationDate
apabila AssignDate/MutationDate < JoinDate, maka tidak valid (tidak termasuk)
*/

;WITH _1A AS (
	SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate
	FROM HrEmployeeMutation a 
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND b.Department = 'SALES'
	AND CONVERT(date, a.MutationDate) <= CONVERT(date, b.JoinDate)
	GROUP BY a.CompanyCode, a.EmployeeID
), _1B AS (
	SELECT a.CompanyCode, a.EmployeeID, MIN(a.MutationDate) AS MutationDate
	FROM HrEmployeeMutation a 
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND b.Department = 'SALES'
	AND CONVERT(date, b.JoinDate) < CONVERT(date, a.MutationDate)
	GROUP BY a.CompanyCode, a.EmployeeID
), _2A AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _1A a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _2B AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _1B a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _3A AS (
	SELECT a.CompanyCode, a.EmployeeID, MAX(a.AssignDate) AS AssignDate
	FROM HrEmployeeAchievement a
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND b.Department = 'SALES'
	AND CONVERT(date, a.AssignDate) <= CONVERT(date, b.JoinDate)
	GROUP BY a.CompanyCode, a.EmployeeID
), _3B AS (
	SELECT a.CompanyCode, a.EmployeeID, MIN(a.AssignDate) AS AssignDate
	FROM HrEmployeeAchievement a
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND b.Department = 'SALES'
	AND CONVERT(date, b.JoinDate) < CONVERT(date, a.AssignDate)
	GROUP BY a.CompanyCode, a.EmployeeID
), _4A AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE b.Grade WHEN '' THEN (CASE WHEN b.Position <> 'S' THEN '' ELSE '1' END) ELSE b.Grade END AS Grade
	,a.AssignDate
	FROM _3A a
	INNER JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _4B AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE b.Grade WHEN '' THEN (CASE WHEN b.Position <> 'S' THEN '' ELSE '1' END) ELSE b.Grade END AS Grade
	,a.AssignDate
	FROM _3B a
	INNER JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _5 AS (
	SELECT a.CompanyCode
	, ISNULL(b.BranchCode, (SELECT c.BranchCode FROM _2B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID)) AS BranchCode
	, a.EmployeeID, a.Position
	, CASE a.Grade WHEN '' THEN (CASE WHEN a.Position <> 'S' THEN '' ELSE '1' END) ELSE a.Grade END AS Grade
	, a.JoinDate
	FROM HrEmployee a
	LEFT JOIN _2A b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
	AND @Start < CONVERT(date, a.JoinDate)
	AND CONVERT(date, a.JoinDate) <= @End
), _6 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID
	, ISNULL(b.Position, ISNULL((SELECT c.Position FROM _4B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID), a.Position)) AS Position
	, ISNULL(b.Grade, ISNULL((SELECT c.Grade FROM _4B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID), a.Position)) AS Grade
	FROM _5 a
	LEFT JOIN _4A b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE 1 = (CASE WHEN ISNULL(b.AssignDate, (SELECT d.AssignDate FROM _4B d WHERE a.CompanyCode = d.CompanyCode AND a.EmployeeID = d.EmployeeID)) IS NULL THEN 1 ELSE 
			  (CASE WHEN CONVERT(date, a.JoinDate) < CONVERT(date, ISNULL(b.AssignDate, (SELECT d.AssignDate FROM _4B d WHERE a.CompanyCode = d.CompanyCode AND a.EmployeeID = d.EmployeeID))) THEN 1 ELSE 0 END) END)
)
SELECT * INTO #JoinIn FROM (
	SELECT * FROM _6 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
	AND a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END
	AND a.Position IS NOT NULL
) #JoinIn

/*
MUTATION IN
memilih data employee yang pindah cabang
*/
;WITH _1 AS (
	SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.MutationDate), a.CompanyCode, a.BranchCode, a.EmployeeID, a.MutationDate
	FROM HrEmployeeMutation a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
), _2 AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, b.MutationDate
	FROM _1 a
	LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE @Start < CONVERT(date, b.MutationDate)
	AND CONVERT(date, b.MutationDate) < @End
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
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
	AND a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END
) #MutationIn

/*
PROMOTION IN
memilih data employee yang mendapat promosi
*/
;WITH _1 AS (
	SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
	FROM HrEmployeeAchievement a
	WHERE 1=1
	AND a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
), _2 AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, b.AssignDate
	FROM _1 a
	LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.Position <> b.Position
	AND b.Position = @Position
	AND @Start < CONVERT(date, b.AssignDate)
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
SELECT * INTO #PromotionIn FROM (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
	FROM _5 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
) #PromotionIn
END

SELECT * INTO #EmployeeIn FROM (
	SELECT * FROM #JoinIn
	UNION
	SELECT * FROM #MutationIn
	UNION 
	SELECT * FROM #PromotionIn	
) #EmployeeIn

BEGIN---#EMPLOYEE OUT#---
/*
RESIGN OUT
memilih data employee yang Resign
ResignDate <= JoinDate dianggap tidak resign
ResignDate > JoinDate dianggap Resign
MutationDate > ResignDate dianggap Resign
AssignDate > ResignDate dianggap Resign
*/
;WITH _1 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Position
	, CASE WHEN a.Position <> 'S' THEN '' ELSE (CASE a.Grade WHEN '' THEN '1' ELSE ISNULL(a.Grade, '1') END) END AS Grade
	, a.JoinDate, a.ResignDate 
	FROM HrEmployee a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
	AND @Start < CONVERT(date, a.ResignDate)
	AND CONVERT(date, a.ResignDate) <= @End
	AND CONVERT(date, a.JoinDate) < CONVERT(date, a.ResignDate)
), _2 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, MAX(b.MutationDate) AS MutationDate, a.ResignDate
	FROM _1 a
	LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE 1=1
	AND b.MutationDate IS NOT NULL
	GROUP BY a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.ResignDate
), _3 AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, a.ResignDate
	FROM _2 a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _4 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, MAX(b.AssignDate) AS AssignDate, a.ResignDate
	FROM _3 a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, a.ResignDate
), _5 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName
	, ISNULL(b.Position, a.Position) AS Position
	, ISNULL(b.Grade, a.Grade) AS Grade
	, a.JoinDate, a.MutationDate, a.AssignDate, a.ResignDate
	FROM _4 a
	INNER JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
)
SELECT * INTO #ResignOut FROM (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
	FROM _5 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
	AND a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END
) #ResignOut

/*
MUTATION OUT
mengambil data employee yang pindah cabang
*/
;WITH _1 AS (
	SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.MutationDate), a.CompanyCode, a.BranchCode, a.EmployeeID, a.MutationDate
	FROM HrEmployeeMutation a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
), _2 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.MutationDate
	FROM _1 a
	LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE @Start < CONVERT(date, b.MutationDate)
	AND CONVERT(date, b.MutationDate) < @End
	AND a.BranchCode <> b.BranchCode
), _3 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, b.JoinDate, a.MutationDate, b.ResignDate 
	FROM _2 a
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE 1 = 1
	AND (b.ResignDate IS NULL OR @End < CONVERT(date, b.ResignDate) OR CONVERT(date, b.ResignDate) <= CONVERT(date, b.JoinDate))
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
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
	AND a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END
) #MutationOut

/*
PROMOTION OUT
mengambil data employee yang mendapat promosi
*/
;WITH _1 AS (
	SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
	FROM HrEmployeeAchievement a
	WHERE 1=1
	AND a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
), _2 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.Position
	, CASE WHEN a.Position <> 'S' THEN '' ELSE (CASE a.Grade WHEN '' THEN '1' ELSE ISNULL(a.Grade, '1') END) END AS Grade
	, b.AssignDate
	FROM _1 a
	LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.Position <> b.Position
	AND a.Position = @Position
	AND @Start < CONVERT(date, b.AssignDate)
	AND CONVERT(date, b.AssignDate) < @End
), _3 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
	FROM _2 a
	LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE 1 = 1
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
SELECT * INTO #PromotionOut FROM (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
	FROM _5 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
) #PromotionOut

SELECT * INTO #EmployeeOut FROM(
	SELECT * FROM #ResignOut
	UNION
	SELECT * FROM #MutationOut
	UNION
	SELECT * FROM #PromotionOut
) #EmployeeOut
END

BEGIN---#EMPLOYEE STAY#---
SELECT * INTO #employeeStay FROM(
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeId, a.Position, a.Grade
	FROM #PoolStart a
	WHERE a.EmployeeID IN (SELECT b.EmployeeID FROM #PoolEnd b)
)#employeeStay
END

BEGIN---#SUM TABLES#---
DECLARE @ITSG TABLE (Value int, Name varchar(15))
INSERT INTO @ITSG (Value, Name) 
	SELECT LookUpValue, LookUpValueName 
	FROM gnMstLookUpDtl WHERE CompanyCode = CASE @DealerCode WHEN '' THEN CompanyCode ELSE @DealerCode END AND CodeID = 'ITSG'

DECLARE @Platinum int = 4, --(SELECT Value FROM @ITSG WHERE Name = 'Platinum'),
	@Gold int = 3, --(SELECT Value FROM @ITSG WHERE Name = 'Gold'),
	@Silver int = 2, --(SELECT Value FROM @ITSG WHERE Name = 'Silver'),
	@Trainee int = 1 --(SELECT Value FROM @ITSG WHERE Name = 'Trainee')

DECLARE @sum1 TABLE (CompanyCode varchar(15), BranchCode varchar(15), CountPoolStart int)
INSERT INTO @sum1 (CompanyCode, BranchCode, CountPoolStart)
	SELECT a.DealerCode,
		a.OutletCode, 
		(SELECT COUNT(*) 
			FROM #PoolStart b 
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
	JOIN #PoolStart a
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
			FROM #PoolEnd b 
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
	JOIN #PoolEnd a
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
END

BEGIN---#RESULT#---
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
END

--SELECT * FROM #JoinFirstStart 
--SELECT * FROM #PoolStart 
--SELECT * FROM #JoinFirstEnd 
--SELECT * FROM #PoolEnd 
--SELECT * FROM #JoinIn 
--SELECT * FROM #PromotionIn 
--SELECT * FROM #MutationIn 
--SELECT * FROM #EmployeeIn 
--SELECT * FROM #ResignOut 
--SELECT * FROM #MutationOut
--SELECT * FROM #PromotionOut
--SELECT * FROM #EmployeeOut 
--SELECT * FROM #sum1
--SELECT * FROM #sum2
SELECT * FROM #result

IF OBJECT_ID('tempdb..#JoinFirstStart') IS NOT NULL DROP TABLE #JoinFirstStart
IF OBJECT_ID('tempdb..#PoolStart') IS NOT NULL DROP TABLE #PoolStart
IF OBJECT_ID('tempdb..#JoinFirstEnd') IS NOT NULL DROP TABLE #JoinFirstEnd
IF OBJECT_ID('tempdb..#PoolEnd') IS NOT NULL DROP TABLE #PoolEnd
IF OBJECT_ID('tempdb..#employeeStay') IS NOT NULL DROP TABLE #employeeStay
IF OBJECT_ID('tempdb..#JoinIn') IS NOT NULL DROP TABLE #JoinIn
IF OBJECT_ID('tempdb..#MutationIn') IS NOT NULL DROP TABLE #MutationIn
IF OBJECT_ID('tempdb..#PromotionIn') IS NOT NULL DROP TABLE #PromotionIn
IF OBJECT_ID('tempdb..#employeeIn') IS NOT NULL DROP TABLE #employeeIn
IF OBJECT_ID('tempdb..#ResignOut') IS NOT NULL DROP TABLE #ResignOut
IF OBJECT_ID('tempdb..#MutationOut') IS NOT NULL DROP TABLE #MutationOut
IF OBJECT_ID('tempdb..#PromotionOut') IS NOT NULL DROP TABLE #PromotionOut
IF OBJECT_ID('tempdb..#employeeOut') IS NOT NULL DROP TABLE #employeeOut
IF OBJECT_ID('tempdb..#sum1') IS NOT NULL DROP TABLE #sum1
IF OBJECT_ID('tempdb..#sum2') IS NOT NULL DROP TABLE #sum2
IF OBJECT_ID('tempdb..#result') IS NOT NULL DROP TABLE #result

END