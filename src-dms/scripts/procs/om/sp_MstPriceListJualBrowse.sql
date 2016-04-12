USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[sp_MstPriceListJualBrowse]    Script Date: 11/5/2014 10:26:53 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[sp_MstPriceListJualBrowse] @CompanyCode varchar(10) , @BranchCode varchar(100)
 as

SELECT a.CompanyCode, a.BranchCode, a.GroupPriceCode, b.RefferenceDesc1 as GroupPriceName, a.SalesModelCode, c.SalesModelDesc, a.SalesModelYear
		, a.TotalMinStaff, a.DPP, a.PPn, a.PPnBM, a.Total, a.Remark, cast(a.Status AS bit) as Status, a.TaxCode, a.TaxPct
FROM omMstPricelistSell a
LEFT JOIN OmMstRefference b
	ON a.GroupPriceCode = b.RefferenceCode AND b.RefferenceType='GRPR'
LEFT JOIN omMstModel c
	ON a.SalesModelCode = c.SalesModelCode
WHERE a.CompanyCode = @CompanyCode and a.BranchCode = @BranchCode
