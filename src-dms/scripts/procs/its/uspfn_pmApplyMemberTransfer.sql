if object_id('uspfn_pmApplyMemberTransfer') is not null
	drop procedure uspfn_pmApplyMemberTransfer
GO
-- Created by Benedict 16 Mar 2015
CREATE PROCEDURE uspfn_pmApplyMemberTransfer
	@p0 varchar(20),  --CompanyCode
	@p1 varchar(20),  --BranchCode
	@p2 varchar(20),  --InquiryNumber IF 'S' || EmployeeID IF NOT 'S'
	@p3 varchar(20),  --EmployeeID IF 'S' || TeamLeader IF NOT 'S'
	@p4 varchar(20)	  --UserID

--DECLARE
--	@p0 varchar(20) = '6006406', --CompanyCode
--	@p1 varchar(20) = '6006401', --BranchCode
--	@p2 varchar(20) = '421468',  --InquiryNumber IF 'S' || EmployeeID IF NOT 'S'
--	@p3 varchar(20) = '52259',   --EmployeeID IF 'S' || TeamLeader IF NOT 'S'
--	@p4 varchar(20) = 'bent'     --UserID
AS
BEGIN

DECLARE @pos1 varchar(5) = (SELECT Position FROM HrEmployee WHERE CompanyCode = @p0 AND EmployeeID = @p3)
IF (@pos1 = 'S')
BEGIN
	IF (@p3 <> (SELECT TOP 1 EmployeeID 
					FROM pmKDP 
					WHERE CompanyCode = @p0
					AND BranchCode = @p1
					AND InquiryNumber = @p2))
	BEGIN
	DECLARE @spvID varchar(15) = (SELECT TeamLeader 
									FROM HrEmployee 
									WHERE CompanyCode = @p0 AND EmployeeID = @p3)
	UPDATE pmKDP
	SET EmployeeID = @p3,
		SpvEmployeeID = @spvID,
		OutletID = @p1,
		LastUpdateBy = @p4,
		LastUpdateDate = getdate()
	WHERE CompanyCode = @p0
		AND BranchCode = @p1
		AND InquiryNumber = @p2
	END
END
ELSE
	IF (@p3 <> (SELECT TeamLeader
				FROM HrEmployee
				WHERE CompanyCode = @p0 AND EmployeeID = @p2))
	BEGIN
		UPDATE HrEmployee
		SET TeamLeader = @p3,
			UpdatedBy = @p4,
			UpdatedDate = getdate()
		WHERE CompanyCode = @p0 AND EmployeeID = @p2
	END


END
GO
