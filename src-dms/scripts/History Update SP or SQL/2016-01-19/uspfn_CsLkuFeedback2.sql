IF EXISTS (SELECT OBJECT_NAME(OBJECT_ID) FROM sys.sql_modules WHERE objectproperty(OBJECT_ID, 'IsProcedure') = 1
AND definition LIKE '%uspfn_CsLkuFeedback2%')
	DROP PROC uspfn_CsLkuFeedback2
GO
CREATE PROCEDURE uspfn_CsLkuFeedback2
	@CompanyCode VARCHAR(20),
	@BranchCode VARCHAR(20),
	@OutStanding CHAR(1),
	@CustomerName VARCHAR(100) = '',
	@VinNo VARCHAR(50) = '',
	@PolReg VARCHAR(15) = ''
AS
BEGIN
	IF @CustomerName IS NULL SET @CustomerName = ''
	IF @VinNo IS NULL SET @VinNo = ''
	IF @PolReg IS NULL SET @PolReg = ''

	SELECT *
	FROM CsLkuFeedbackView 
	WHERE CompanyCode = @CompanyCode 
		AND BranchCode = @BranchCode 
		AND OutStanding = @OutStanding
		AND CustomerName LIKE '%'+@CustomerName+'%'
		AND Chassis LIKE '%'+@VinNo+'%'
		AND PoliceRegNo LIKE '%'+@PolReg+'%'
END