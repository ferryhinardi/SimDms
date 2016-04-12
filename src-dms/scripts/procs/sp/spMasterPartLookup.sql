

/****** Object:  View [dbo].[spMasterPartLookup]    Script Date: 05/05/2014 11:25:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[spMasterPartLookup]
AS	
	SELECT 
	 Items.CompanyCode,
	 Items.BranchCode,
	 Items.PartNo
	,Items.ProductType
	,(SELECT LookupValueName 
		FROM gnMstLookupDtl 
	   WHERE CodeID = 'PRCT' AND 
			 LookUpValue = Items.PartCategory AND 
			 CompanyCode = Items.CompanyCode) AS CategoryName
	,Items.PartCategory
	,ItemInfo.PartName
	,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
	,CASE Items.Status WHEN 1 THEN 'Aktif' ELSE 'Tidak' END AS IsActive
	,ItemInfo.OrderUnit
	,ItemInfo.SupplierCode
	,Supplier.SupplierName
	,(SELECT LookupValueName 
		FROM gnMstLookupDtl 
	  WHERE CodeID = 'TPGO' AND 
			LookUpValue = Items.TypeOfGoods AND 
			CompanyCode = Items.CompanyCode) AS TypeOfGoods
	FROM SpMstItems Items
	INNER JOIN SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
							 AND Items.PartNo = ItemInfo.PartNo
	LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
							 AND Supplier.SupplierCode = ItemInfo.SupplierCode
GO


