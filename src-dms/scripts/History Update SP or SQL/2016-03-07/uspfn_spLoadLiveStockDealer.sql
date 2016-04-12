IF object_id('uspfn_spLoadLiveStockDealer') IS NOT NULL DROP PROCEDURE uspfn_spLoadLiveStockDealer
Go
CREATE PROCEDURE [dbo].[uspfn_spLoadLiveStockDealer]
	@CompanyCode varchar(15),
	@Type varchar(30) = NULL,
	@Variant varchar(50) = NULL,
	@Transmission varchar(5) = NULL
AS
BEGIN

DECLARE @Query NVARCHAR(MAX);
DECLARE @Column VARCHAR(500);
DECLARE @Table VARCHAR(500);
DECLARE @WhereClause VARCHAR(500);
DECLARE @GroupBy VARCHAR(500);
DECLARE @OrderBy VARCHAR(500);

set @Column = 'b.GroupCode AS [Type]';
SET @Column = @Column + ', b.TypeCode AS [Variant]';
SET @Column = @Column + ', b.TransmissionType AS [Transmission]';
SET @Column = @Column + ', ISNULL(e.IsVisible, 0) AS IsVisible';
SET @Column = @Column + ', SUM(EndingAV) AS Qty';

SET @Table = 'omTrInventQtyVehicle a';
SET @Table = @Table + ' JOIN omMstModel b ON a.SalesModelCode = b.SalesModelCode';
SET @Table = @Table + ' LEFT JOIN pmGroupTypeSeq c ON b.GroupCode = c.GroupCode AND b.TypeCode = c.TypeCode';
SET @Table = @Table + ' LEFT JOIN omMstLiveStockDealer e ON b.GroupCode = e.Type AND b.TypeCode = e.Variant';

--SET @WhereClause = 'a.EndingAV > 0 AND a.Year = YEAR(GETDATE()) AND a.Month = MONTH(GETDATE())';
SET @WhereClause = 'a.EndingAV > 0 AND a.Year = YEAR(GETDATE()) AND a.Month = MONTH(GETDATE()) AND a.CompanyCode= ''' + @CompanyCode + '''';
IF @Type IS NOT NULL AND @Type <> ''
	SET @WhereClause = @WhereClause + ' AND b.GroupCode = ''' + @Type + '''';
IF @Variant IS NOT NULL AND @Variant <> ''
	SET @WhereClause = @WhereClause + ' AND b.TypeCode = ''' + @Variant + '''';
IF @Transmission IS NOT NULL AND @Transmission <> ''
	SET @WhereClause = @WhereClause + ' AND b.TransmissionType = ''' + @Transmission + '''';

SET @GroupBy = 'b.GroupCode';
SET @GroupBy = @GroupBy + ', b.TypeCode';
SET @GroupBy = @GroupBy + ', b.TransmissionType';
SET @GroupBy = @GroupBy + ', e.IsVisible';

SET @OrderBy = 'Type, Variant';

SET @Query = 'SELECT ' +@Column+ ' FROM ' + @Table + ' WHERE ' + @WhereClause + ' GROUP BY ' + @GroupBy + ' ORDER BY ' + @OrderBy;

--SELECT @Query
EXEC sp_executesql @Query

END