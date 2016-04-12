IF object_id('uspfn_spCheckLiveStockDealerSales') IS NOT NULL DROP PROCEDURE uspfn_spCheckLiveStockDealerSales
Go
CREATE PROCEDURE [dbo].[uspfn_spCheckLiveStockDealerSales]
	@CompanyCode Varchar(15),
	@Type VARCHAR(50),
	@Variant VARCHAR(40),
	@IsVisible BIT
AS 
IF EXISTS(
	SELECT NULL 
	FROM omTrInventQtyVehicle a 
	JOIN omMstModel b ON a.SalesModelCode = b.SalesModelCode
	LEFT JOIN pmGroupTypeSeq c ON b.GroupCode = c.GroupCode AND b.TypeCode = c.TypeCode
	LEFT JOIN omMstLiveStockDealer e ON b.GroupCode = e.Type AND b.TypeCode = e.Variant
	AND e.CompanyCode = a.CompanyCode 
	WHERE a.EndingAV > 0 AND a.Year = YEAR(GETDATE()) AND a.Month = MONTH(GETDATE()) 
	AND b.GroupCode = @Type AND b.TypeCode = @Variant AND ISNULL(e.IsVisible, 0) = @IsVisible
)
	SELECT 1 AS [Result]
ELSE
	SELECT 0 AS [Result]