
if object_id('uspfn_pmSelectOrganizationTree') is not null
	drop procedure uspfn_pmSelectOrganizationTree
GO
--created by BENEDICT 11/Mar/2015 LAST UPDATED on 14/Apr/2015

CREATE PROCEDURE uspfn_pmSelectOrganizationTree
@CompanyCode varchar(15),
@BranchCode varchar(15)

--DECLARE @CompanyCode varchar(15) = '6159401000'
--DECLARE @BranchCode varchar(15) = '6159401001'

AS BEGIN
SELECT * INTO #test1 FROM(
	SELECT a.BranchCode, a.EmployeeID, b.EmployeeName, e.PosLevel AS PositionID, b.Position, e.PosName AS PositionName,
	(rtrim(a.EmployeeID) + ' - ' + rtrim(b.EmployeeName)) Employee,
	isnull((
			select count(*) from PmKDP
			 where CompanyCode  = a.CompanyCode
			   and BranchCode   = a.BranchCode
			   and EmployeeID   = a.EmployeeID
			), 0) CountKDP, b.TeamLeader, ISNULL(f.OutletAbbreviation, a.BranchCode) AS BranchAbv, b.IsAssigned
	FROM hrEmployeeMutation a
	JOIN (
		SELECT c.EmployeeId, c.EmployeeName, c.Position, ISNULL(c.TeamLeader, '') AS TeamLeader, MAX(d.MutationDate) AS MutationDate, c.IsAssigned
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON c.EmployeeId = d.EmployeeId
		WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
		GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader, c.IsAssigned
	) b
	ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
	JOIN gnMstPosition e
	ON a.CompanyCode = e.CompanyCode AND e.DeptCode = 'SALES' AND b.Position = e.PosCode
	JOIN gnMstDealerOutletMapping f
	ON a.CompanyCode = f.DealerCode AND a.BranchCode = f.OutletCode
	WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = CASE @BranchCode WHEN '' THEN a.BranchCode ELSE @BranchCode END
UNION
	SELECT a.BranchCode, a.EmployeeID, b.EmployeeName, e.PosLevel AS PositionID, b.Position, e.PosName AS PositionName,
	(rtrim(a.EmployeeID) + ' - ' + rtrim(b.EmployeeName)) Employee,
	isnull((
			select count(*) from PmKDP
			 where CompanyCode  = a.CompanyCode
			   and BranchCode   = a.BranchCode
			   and EmployeeID   = a.EmployeeID
			), 0) CountKDP, b.TeamLeader, ISNULL(f.OutletAbbreviation, a.BranchCode) AS BranchAbv, b.IsAssigned
	FROM hrEmployeeMutation a
	JOIN (
		SELECT c.EmployeeId, c.EmployeeName, c.Position, ISNULL(c.TeamLeader, '') AS TeamLeader, MAX(d.MutationDate) AS MutationDate, c.IsAssigned
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON c.EmployeeId = d.EmployeeId
		WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
		AND c.Position = 'GM'
		GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader, c.IsAssigned
	) b
	ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
	JOIN gnMstPosition e
	ON a.CompanyCode = e.CompanyCode AND e.DeptCode = 'SALES' AND b.Position = e.PosCode
	JOIN gnMstDealerOutletMapping f
	ON a.CompanyCode = f.DealerCode AND a.BranchCode = f.OutletCode
	WHERE a.CompanyCode = @CompanyCode
)#test1

;WITH N(id, lvl, BranchCode, EmployeeID, EmployeeName, PositionID, Position, PositionName, Employee, CountKDP, TeamLeader, BranchAbv, IsAssigned)
AS
(
	SELECT 
		CAST(row_number() OVER(ORDER BY a.EmployeeID) AS varchar) as id,
		0 AS lvl,
		a.BranchCode, a.EmployeeID, a.EmployeeName, a.PositionID, a.Position, a.PositionName, a.Employee, a.CountKDP, a.TeamLeader, a.BranchAbv, a.IsAssigned
	FROM #test1 a
	WHERE a.TeamLeader = ''
	AND a.IsAssigned = 1
	UNION ALL
	SELECT 
		CAST(N.id + '.' + CAST(row_number() OVER(ORDER BY b.EmployeeID) AS varchar) AS varchar) as id,
		N.lvl + 1 AS lvl,
		b.BranchCode, b.EmployeeID, b.EmployeeName, b.PositionID, b.Position, b.PositionName, b.Employee, b.CountKDP, b.TeamLeader, b.BranchAbv, b.IsAssigned
	FROM #test1 b JOIN N ON N.EmployeeID = b.TeamLeader
)
SELECT * FROM N ORDER BY lvl, PositionID DESC, BranchCode, id

DROP TABLE #test1
END
GO
