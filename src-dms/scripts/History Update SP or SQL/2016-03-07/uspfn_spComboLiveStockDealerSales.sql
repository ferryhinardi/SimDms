IF object_id('uspfn_spComboLiveStockDealerSales') IS NOT NULL DROP PROCEDURE uspfn_spComboLiveStockDealerSales
GO
CREATE PROCEDURE [dbo].[uspfn_spComboLiveStockDealerSales]
	@TypeCombo VARCHAR(15),
	@Type VARCHAR(30) = NULL,
	@Variant VARCHAR(30) = NULL,
	@Colour VARCHAR(10) = NULL,
	@IsVisible BIT = NULL
AS
BEGIN

	IF @Type = '' SET @Type = NULL;
	IF @Variant = '' SET @Variant = NULL;
	IF @Colour = '' SET @Colour = NULL;

	DECLARE @Query NVARCHAR(MAX);
	DECLARE @Column VARCHAR(500);
	DECLARE @Table VARCHAR(500);
	DECLARE @WhereClause VARCHAR(500);
	DECLARE @OrderBy VARCHAR(500);

	IF @TypeCombo = 'Type'
	BEGIN
		SET @Column = 'CASE WHEN ISNULL(b.GroupCode, '''') = '''' THEN ''***''+b.SalesModelCode ELSE b.GroupCode END AS [value]';
		SET @Column = @Column + ', CASE WHEN ISNULL(b.GroupCode, '''') = '''' THEN ''***''+b.SalesModelCode ELSE b.GroupCode END AS [text] ';
	END
	ELSE IF @TypeCombo = 'Variant'
	BEGIN
		SET @Column = 'c.TypeCode AS [value], c.TypeCode AS [text]';
	END
	ELSE IF @TypeCombo = 'Colour'
	BEGIN
		SET @Column = 'a.ColourCode AS [value], a.ColourCode AS [text]';
	END

	SET @Table = 'omTrInventQtyVehicle a';
	SET @Table = @Table + ' JOIN omMstModel b ON a.SalesModelCode = b.SalesModelCode';
	SET @Table = @Table + ' LEFT JOIN pmGroupTypeSeq c ON b.GroupCode = c.GroupCode AND b.TypeCode = c.TypeCode';
	SET @Table = @Table + ' LEFT JOIN omMstLiveStockDealer e ON c.GroupCode = e.Type AND c.TypeCode = e.Variant';

	SET @WhereClause = 'a.EndingAV > 0 AND a.Year = YEAR(GETDATE()) AND a.Month = MONTH(GETDATE())';
	--SET @WhereClause = 'a.EndingAV > 0';

	IF @IsVisible IS NOT NULL
		SET @WhereClause = @WhereClause + ' AND ISNULL(e.IsVisible, 0) = ' + CAST(@IsVisible AS VARCHAR);
	
	IF @TypeCombo = 'Type'
		SET @OrderBy = 'CASE WHEN ISNULL(b.GroupCode, '''') = '''' THEN ''***''+b.SalesModelCode ELSE b.GroupCode END';
	IF @TypeCombo = 'Variant'
	BEGIN
		IF @Colour IS NOT NULL AND @Colour <> ''
			SET @WhereClause = @WhereClause + ' AND c.GroupCode = ''' + @Type + ''' AND a.ColourCode = ''' + @Colour + '''';
		SET @WhereClause = @WhereClause + ' AND c.TypeCode IS NOT NULL';
		SET @OrderBy = 'c.TypeCode';
	END
	IF @TypeCombo = 'Colour'
	BEGIN
		IF @Variant IS NOT NULL AND @Variant <> ''
			SET @WhereClause = @WhereClause + ' AND c.GroupCode = ''' + @Type + ''' AND c.TypeCode = ''' + @Variant + '''';
		SET @OrderBy = 'ColourCode DESC';
	END

	SET @Query = 'SELECT DISTINCT ' +@Column+ ' FROM ' + @Table + ' WHERE ' + @WhereClause + ' ORDER BY ' + @OrderBy;

	--SELECT @Query
	EXEC sp_executesql @Query
END