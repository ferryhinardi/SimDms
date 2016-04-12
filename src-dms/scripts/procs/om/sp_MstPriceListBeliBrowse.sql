USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[sp_MstPriceListBeliBrowse]    Script Date: 11/5/2014 10:23:40 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[sp_MstPriceListBeliBrowse] @CompanyCode varchar(10) , @BranchCode varchar(100)
 as

SELECT a.CompanyCode, a.BranchCode, a.SupplierCode, b.SupplierName, a.SalesModelCode, c.SalesModelDesc, a.SalesModelYear
		,a.PPnBMPaid, a.DPP, a.PPn, a.PPnBM, a.Total, a.Remark, cast(a.Status AS bit) as Status
FROM omMstPricelistBuy a
LEFT JOIN gnMstSupplier b
	ON a.SupplierCode = b.SupplierCode
LEFT JOIN omMstModel c
	ON a.SalesModelCode = c.SalesModelCode
WHERE a.CompanyCode = @CompanyCode and a.BranchCode = @BranchCode