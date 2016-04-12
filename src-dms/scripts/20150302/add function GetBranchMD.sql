CREATE FUNCTION [dbo].[GetBranchMD] (@CompanyCode VARCHAR(15),@BranchCode VARCHAR(15)) 
RETURNS VARCHAR (20)
AS 
BEGIN
	DECLARE @BranchMD AS VARCHAR(15)
	SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

	RETURN @BranchMD
END