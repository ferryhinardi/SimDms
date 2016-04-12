

ALTER TABLE HrEmployee
ADD IsAssigned bit


-- status = 1, punya leader atau punya anak buah >> IsAssigned = 1
-- tidak punya leader dan tidak punya anak buah >> IsAssigned = 0
SELECT * INTO #temp1 FROM (
	SELECT EmployeeID, EmployeeName, PersonnelStatus, TeamLeader FROM HrEmployee a WHERE
	(
	(TeamLeader <> '' AND TeamLeader IS NOT NULL)
	OR 
	EXISTS (SELECT b.EmployeeID FROM HrEmployee b WHERE b.TeamLeader = a.EmployeeID AND b.PersonnelStatus = '1'))
	AND a.PersonnelStatus = '1'
	AND a.Department = 'SALES'
)#temp1

UPDATE HrEmployee SET IsAssigned = 1
WHERE EmployeeID IN (SELECT EmployeeID FROM #temp1)

UPDATE HrEmployee SET IsAssigned = 0
WHERE 
(IsAssigned <> 1 OR IsAssigned IS NULL)
AND Department = 'SALES'

DROP TABLE #temp1
