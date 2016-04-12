ALTER PROCEDURE [dbo].[uspfn_spTrnPPOSHdr_Web] 
(@CompanyCode VARCHAR(10),@BranchCode VARCHAR(10), @TypeOfGoods VARCHAR(10), @Status char, @DynamicFilter VARCHAR(4000) = '', @top INT = 100)
AS
DECLARE @Query VARCHAR(MAX);
SET @Query = 'SELECT TOP ' + CONVERT(VARCHAR, @top) + '	a.POSNo, a.PosDate , a.Status ,a.SupplierCode ,b.SupplierName, a.isBO
        FROM spTrnPPOSHdr a
        INNER JOIN gnMstSupplier b ON b.SupplierCode = a.SupplierCode and b.CompanyCode = a.CompanyCode
	WHERE 
	a.CompanyCode =''' + @CompanyCode + '''
	AND a.BranchCode=''' + @BranchCode + '''
    AND a.TypeOfGoods=''' + @TypeOfGoods + '''
    AND a.Status <=''' + @Status + ''''
		+ @DynamicFilter
	+ ' ORDER BY a.POSNo DESC'

EXEC(@Query)