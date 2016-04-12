ALTER PROCEDURE [dbo].[usprpt_InqRegPol] 
     @DealerCode varchar(15),
     @ProvinceCode as varchar (15),
	 @RegencyCode varchar (15),
	 @Year varchar(4),
	 @Month varchar(10)

AS
BEGIN

DECLARE @query VARCHAR(max)
DECLARE @cities VARCHAR(max)

--SELECT @cities = STUFF((
--    SELECT DISTINCT
--        '],[' + ltrim(CityCode)
--    FROM msMstCity
--	WHERE (CASE WHEN  @ProvinceCode ='%' THEN '%'  ELSE msMstCity.ProvinceCode END)= @ProvinceCode
--	and AreaCode <> '' and RegionCode <> ''
--    ORDER BY '],[' + ltrim(CityCode)
--    FOR XML PATH('')), 1, 2, '') + ']'


SELECT @cities = STUFF((
    SELECT DISTINCT
        '],[' + LTRIM(CityCode) + ' - ' + LTRIM(CityDesc)
    FROM msMstCity
	WHERE (CASE WHEN  @ProvinceCode ='%' THEN '%'  ELSE msMstCity.ProvinceCode END)= @ProvinceCode
	--and AreaCode <> '' and RegionCode <> ''
    ORDER BY '],[' +  LTRIM(CityCode) + ' - ' + LTRIM(CityDesc)
    FOR XML PATH('')), 1, 2, '') + ']'

IF(@RegencyCode = '%')
BEGIN
SET @query =
    '
	SELECT * FROM
    (
        --SELECT DISTINCT c.ReportSequence,a.DealerAbbreviation,b.BrandCode,(b.ModelType + '+ ''' - ''' + '+ b.ModelName) ModelType,b.CylinderCapacity,b.TransmissionType,d.CityCode,isnull('+@Month+',0) '+@Month+'
	    SELECT DISTINCT c.ReportSequence,a.DealerAbbreviation,b.BrandCode,(b.ModelType + '+ ''' - ''' + '+ b.ModelName) ModelType,b.CylinderCapacity,b.TransmissionType,city.CityCode  + '+ ''' - ''' + '+ city.CityDesc CityDesc,isnull('+@Month+',0) '+@Month+'
		FROM gnMstDealerMapping a
        left join msMstModel b on a.isActive = 1
        inner join msMstReference c on c.BrandName = b.BrandCode AND c.ReferenceType =''BRAND''
		left join msMstDealerCity e on e.DealerCode = a.DealerCode 
		left join msTrnMarketShare d on d.DealerCode = a.DealerCode and d.ModelType = b.ModelType and d.Variant = b.Variant and d.Year ='''+@Year+''' and d.CityCode = e.CityCode
		left join msMstCity city on d.CityCode = city.CityCode
		WHERE (CASE WHEN ''' + @DealerCode + ''' = ' + '''%''' + ' THEN '+ '''%''' +'  ELSE a.DealerCode END)=''' + @DealerCode+'''		
    ) AS t
    PIVOT (max('+@Month+') FOR CityDesc IN (' + @cities + ')) AS pvt'
END
ELSE
BEGIN
    declare @citydesc varchar(max)
	set @citydesc = (SELECT DISTINCT CityDesc FROM msMstCity WHERE CityCode = @RegencyCode)

    SET @query = 
    'SELECT * FROM
    (
        SELECT DISTINCT rf.ReportSequence,dm.DealerAbbreviation,mdl.BrandCode,(mdl.ModelType + '+ ''' - ''' +' + mdl.ModelName) ModelType,mdl.CylinderCapacity,mdl.TransmissionType,isnull('+@Month+',0) as '''+@citydesc+'''
        FROM msMstModel mdl 
        LEFT JOIN msMstReference rf
        ON rf.BrandName = mdl.BrandCode  and rf.ReferenceType =''BRAND''  
		left JOIN msTrnMarketShare ms
		ON mdl.ModelType = ms.ModelType and mdl.Variant = ms.Variant
		AND (CASE WHEN ''' + @DealerCode + ''' = ' + '''%''' + ' THEN '+ '''%''' +'  ELSE ms.DealerCode END)=''' + @DealerCode+'''
        AND(CASE WHEN ''' + @Year + ''' = ''0'' THEN ''0'' ELSE ms.Year END)= '''+@Year+'''
        left JOIN gnMstDealerMapping dm
        ON dm.DealerCode = '''+ @DealerCode+'''
		WHERE ms.CityCode = '''+@RegencyCode+'''
    ) AS t'	
END
    EXECUTE (@query)
    PRINT (@query) 
END

