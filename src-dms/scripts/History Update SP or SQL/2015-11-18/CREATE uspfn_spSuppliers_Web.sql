CREATE PROCEDURE [dbo].[uspfn_spSuppliers_Web]  (  @CompanyCode varchar(10) ,@BranchCode varchar(10), @ProfitCenterCode char(3),  @DynamicFilter varchar(4000) = '', @top int = 100) 
	as
DECLARE @Query VARCHAR(MAX);
SET @Query = 'SELECT * FROM (SELECT distinct TOP ' + CONVERT(VARCHAR, @top) + ' 
	a.SupplierCode, a.SupplierName, ISNULL((a.address1+''  ''+a.address2+'' ''+a.address3+'' ''+a.address4),'''') as Alamat,
	b.DiscPct as Diskon, (Case a.Status when 0 then ''Tidak Aktif'' else ''Aktif'' end) as [Status],
	(SELECT Lookupvaluename FROM gnmstlookupdtl WHERE codeid=''PFCN'' 
	 AND lookupvalue = b.ProfitCentercode) as Profit
	FROM 
	gnMstSupplier a
	JOIN gnmstSupplierProfitCenter b ON a.CompanyCode= b.CompanyCode
	AND a.SupplierCode = b.SupplierCode
	WHERE 
	a.CompanyCode =''' + @CompanyCode + '''
	AND b.BranchCode =''' +  @BranchCode  + '''
	AND b.ProfitCenterCode =''' + @ProfitCenterCode + '''
	AND b.isBlackList=0
	AND a.status = 1 ) x ' 
			+ CASE @DynamicFilter WHEN '' THEN '' ELSE ' WHERE '
			+ RIGHT(@DynamicFilter, LEN(@DynamicFilter)-5) END
          
EXEC(@Query)