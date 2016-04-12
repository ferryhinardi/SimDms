if object_id('uspfn_SelectEmployeeOrganizationTree') is not null
	drop procedure uspfn_SelectEmployeeOrganizationTree
GO
CREATE PROCEDURE [dbo].[uspfn_SelectEmployeeOrganizationTree]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@EmployeeID varchar(15)

AS BEGIN
SELECT * INTO #test1 FROM(
SELECT a.BranchCode, a.EmployeeID, b.EmployeeName, b.Position
,(rtrim(a.EmployeeID) + ' - ' + rtrim(b.EmployeeName)) Employee, b.TeamLeader
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, ISNULL(c.TeamLeader, '') AS TeamLeader, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId
	WHERE c.Department = 'SALES' 
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode
)#test1

;WITH N(lvl, BranchCode, value, text, Position, Employee, TeamLeader)
AS
(
	SELECT 
		0 AS lvl,
		a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Employee, a.TeamLeader
	FROM #test1 a
	WHERE a.EmployeeID = @EmployeeID
	UNION ALL
	SELECT 
		N.lvl + 1 AS lvl,
		b.BranchCode, b.EmployeeID, b.EmployeeName, b.Position, b.Employee, b.TeamLeader
	FROM #test1 b JOIN N ON N.value = b.TeamLeader
)
SELECT * FROM N 
WHERE lvl > 0
ORDER BY lvl DESC

DROP TABLE #test1
END
GO
