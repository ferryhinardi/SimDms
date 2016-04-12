USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[sp_BBNKIRBrowse]    Script Date: 11/5/2014 10:29:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[sp_BBNKIRBrowse]  @CompanyCode varchar(10), @BranchCode varchar(10)
 as
 
SELECT a.CompanyCode, a.BranchCode, a.SupplierCode, b.SupplierName, a.CityCode, c.LookUpValueName as CityName
		, a.SalesModelCode, d.SalesModelDesc, a.SalesModelYear, d.SalesModelDesc as SalesModelYearDesc, CAST(a.Status as bit) as Status
		, a.BBN, a.KIR, a.Remark
FROM omMstBBNKIR a
INNER JOIN gnMstSupplier b
	ON a.SupplierCode = b.SupplierCode
INNER JOIN gnMstLookUpDtl c
	ON a.CityCode = c.LookUpValue AND c.CodeID = 'CITY'
INNER JOIN omMstModel d
	ON a.SalesModelCode = d.SalesModelCode
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode 