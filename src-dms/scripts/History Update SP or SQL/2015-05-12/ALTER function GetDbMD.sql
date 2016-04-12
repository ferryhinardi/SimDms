ALTER FUNCTION [dbo].[GetDbMD] (@CompanyCode VARCHAR(15),@BranchCode VARCHAR(15)) 
RETURNS VARCHAR (20)
AS 
BEGIN
	DECLARE @DbMD varchar(20);
	SET @DbMD = (SELECT DISTINCT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
	IF(@DbMD is null) begin
		SET @DbMD = (SELECT DISTINCT DbMD FROM gnMstCompanyMapping WHERE CompanyMD = @CompanyCode AND BranchMD = @BranchCode)
	END
	
	RETURN @DbMD
END
