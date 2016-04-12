CREATE PROCEDURE [dbo].[uspfn_SpPembelianBrowse_Web]  (  @CompanyCode varchar(10) ,@BranchCode varchar(10), @TypeOfGoods char(3),  @DynamicFilter varchar(4000) = '', @top int = 100) 
	as
DECLARE @Query VARCHAR(MAX);
SET @Query = 'SELECT TOP ' + CONVERT(VARCHAR, @top) + ' 
	* FROM 
	SpPembelian
	WHERE 
	CompanyCode =''' + @CompanyCode + '''
	AND BranchCode =''' +  @BranchCode  + '''
	AND TypeOfGoods =''' + @TypeOfGoods + '''' 
	+ @DynamicFilter
          
EXEC(@Query)