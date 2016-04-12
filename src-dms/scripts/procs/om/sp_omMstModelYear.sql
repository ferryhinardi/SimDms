USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[sp_omMstModelYear]    Script Date: 11/13/2014 8:24:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[sp_omMstModelYear] @CompanyCode varchar(10) , @SalesModelCode varchar(100)
 as

SELECT CompanyCode, SalesModelCode, SalesModelYear, SalesModelDesc, ChassisCode, Remark, cast(Status AS bit) as [Status]
FROM omMstModelYear
where CompanyCode=@CompanyCode and SalesModelCode=@SalesModelCode