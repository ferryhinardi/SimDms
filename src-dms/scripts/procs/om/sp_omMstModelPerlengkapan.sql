USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[sp_omMstModelPerlengkapan]    Script Date: 11/11/2014 11:17:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[sp_omMstModelPerlengkapan] @CompanyCode varchar(10), @BranchCode varchar(10), @SalesModelCode varchar(100)
 as

SELECT a.CompanyCode
		, a.BranchCode
		, a.SalesModelCode
		, a.PerlengkapanCode
		, b.PerlengkapanName
		, a.Quantity
		, a.Remark
		, CAST(a.Status AS bit) as [Status]

FROM omMstModelPerlengkapan a
LEFT JOIN omMstPerlengkapan b
	ON a.PerlengkapanCode = b.PerlengkapanCode
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
where a.CompanyCode=@CompanyCode and a.BranchCode = @BranchCode and SalesModelCode=@SalesModelCode