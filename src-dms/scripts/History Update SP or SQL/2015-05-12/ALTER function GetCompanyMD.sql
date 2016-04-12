ALTER FUNCTION [dbo].[GetCompanyMD] (@CompanyCode VARCHAR(15),@BranchCode VARCHAR(15)) 
RETURNS VARCHAR (20)
AS 
BEGIN
	DECLARE @CompanyMD AS VARCHAR(15)
	SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

	if(@CompanyMD is null) begin
		SET @CompanyMD = (SELECT DISTINCT CompanyMD FROM gnMstCompanyMapping WHERE CompanyMD = @CompanyCode AND BranchMD = @BranchCode)
	end

	RETURN @CompanyMD
END
