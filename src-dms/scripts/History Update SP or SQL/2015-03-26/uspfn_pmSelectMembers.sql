if object_id('uspfn_pmSelectMembers') is not null
	drop procedure uspfn_pmSelectMembers
GO
--Created by Benedict 13 Mar 2015
CREATE PROCEDURE uspfn_pmSelectMembers
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@EmployeeID varchar(20)
--DECLARE	
--	@CompanyCode varchar(20) = '6006406',
--	@BranchCode  varchar(20) = '6006401',
--	@EmployeeID varchar(20) = '51343'
AS
BEGIN
DECLARE 
	@pos1 varchar(5) = (SELECT Position FROM HrEmployee WHERE CompanyCode = @CompanyCode AND EmployeeID = @EmployeeID)

IF (@pos1 = 'S' )
BEGIN

SELECT BranchCode, CONVERT(varchar, InquiryNumber) AS KeyID, NamaProspek, PerolehanData
	 , CONVERT(varchar, InquiryNumber) + ' - ' + RTRIM(NamaProspek)
	 + CASE WHEN RTRIM(PerolehanData) = '' THEN '' ELSE 
	 ' (' + RTRIM(PerolehanData) + ')' END AS Member
  FROM pmKdp
 WHERE CompanyCode = @CompanyCode
   AND BranchCode = @BranchCode
   AND EmployeeID = @EmployeeID
END

IF (@pos1 <> 'S')
BEGIN

SELECT a.BranchCode, a.EmployeeID AS KeyID, 
	CONVERT(varchar, a.EmployeeID) + ' - ' + RTRIM(b.EmployeeName)
	 + CASE WHEN RTRIM(e.PosName) = '' THEN '' ELSE 
	 ' (' + RTRIM(e.PosName) + ')' END AS Member
FROM hrEmployeeMutation a
JOIN (
	SELECT c.EmployeeId, c.EmployeeName, c.Position, MAX(d.MutationDate) AS MutationDate
	FROM hrEmployee c
	JOIN hrEmployeeMutation d
	ON c.EmployeeId = d.EmployeeId AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
	WHERE c.Department = 'SALES' AND c.TeamLeader = @EmployeeID
	GROUP BY c.EmployeeId, c.EmployeeName, c.Position
) b
ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
JOIN gnMstPosition e
ON a.CompanyCode = e.CompanyCode AND e.DeptCode = 'SALES' AND b.Position = e.PosCode
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode

END
END
GO
