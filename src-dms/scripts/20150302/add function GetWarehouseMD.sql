CREATE FUNCTION [dbo].[GetWarehouseMD] (@CompanyCode VARCHAR(15),@BranchCode VARCHAR(15)) 
RETURNS VARCHAR (20)
AS 
BEGIN
	DECLARE @WarehouseMD AS VARCHAR(15)
	SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

	RETURN @WarehouseMD
END