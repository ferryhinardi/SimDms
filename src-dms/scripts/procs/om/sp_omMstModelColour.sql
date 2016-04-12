USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[sp_omMstModelColour]    Script Date: 11/11/2014 8:55:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[sp_omMstModelColour] @CompanyCode varchar(10) , @SalesModelCode varchar(100)
 as

SELECT a.CompanyCode, a.SalesModelCode, b.SalesModelDesc, ColourCode, RefferenceDesc1, a.Remark, cast(a.Status AS bit) as [Status]
FROM omMstModelColourView a
INNER JOIN omMstModel b
	ON a.SalesModelCode = b.SalesModelCode
where a.CompanyCode=@CompanyCode and a.SalesModelCode=@SalesModelCode