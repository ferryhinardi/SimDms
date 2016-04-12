USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[sp_SpMstItemLocView]    Script Date: 10/21/2014 4:45:45 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[sp_RefferenceTypeLookup]  @CompanyCode varchar(10)
 as
SELECT DISTINCT  * 
FROM (
	SELECT distinct(a.RefferenceType)  AS RefferenceType
	FROM dbo.omMstRefference a
	WHERE a.CompanyCode = '6006410'
	UNION
	SELECT distinct(b.RefferenceCode)  AS RefferenceType
	FROM dbo.omMstRefference b
	WHERE b.CompanyCode = '6006410' AND b.RefferenceType = 'REFF'
	) tab
ORDER BY RefferenceType ASC



