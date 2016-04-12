USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[sp_KaroseriBrowse]    Script Date: 11/5/2014 10:19:02 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[sp_KaroseriBrowse]  @CompanyCode varchar(10), @BranchCode varchar(10)
 as

select a.CompanyCode, a.BranchCode, a.SupplierCode, b.SupplierName, a.SalesModelCode, c.SalesModelDesc, a.SalesModelCodeNew, d.SalesModelDesc as SalesModelDescNew
		,a.DPPMaterial, a.DPPFee, a.DPPOthers, a.PPn, a.Total, a.Remark, cast(a.Status AS bit) as Status
from omMstKaroseri a
INNER JOIN gnMstSupplier b
	ON a.SupplierCode = b.SupplierCode
INNER JOIN omMstModel c
	ON a.SalesModelCode = c.SalesModelCode
INNER JOIN omMstModel d
	ON a.SalesModelCodeNew = d.SalesModelCode
WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = @BranchCode 