CREATE PROCEDURE [dbo].[uspfn_gnMstZipCodeBrowse_Web]  (  @CompanyCode varchar(10), @DynamicFilter varchar(4000) = '', @top int = 100) 
	as
DECLARE @Query VARCHAR(MAX);
SET @Query = 'SELECT DISTINCT TOP ' + CONVERT(VARCHAR, @top) + '
	a.ZipCode, a.KelurahanDesa,a.KecamatanDistrik, a.KotaKabupaten, a.IbuKota,  a.isCity
	    , CityCode = isnull((select top 1 LookUpValue from  gnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = ''City'' and LookUpValueName = a.KotaKabupaten and LookUpValue is not null order by LastUpdateDate desc),'''')
	    , AreaCode = isnull((select top 1 LookUpValue from  gnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = ''AREA'' and LookUpValue is not null  order by LastUpdateDate desc),'''') 
        from GnMstZipCode a
	WHERE 
	a.CompanyCode =''' + @CompanyCode + ''''
		+ @DynamicFilter

EXEC(@Query)