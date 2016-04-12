/****** Object:  StoredProcedure [dbo].[sp_spMasterPartSelect4Lookup]    Script Date: 02/04/2015 11:19:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- EXEC sp_spMasterPartSelect4Lookup '6006410', '600641001', '0', '4W'
CREATE PROCEDURE [dbo].[sp_spMasterPartSelect4Lookup] ( 
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@TypeOfGoods varchar(15)
	,@ProductType varchar(15)
)
AS
	SELECT 
	 Items.PartNo
	,Items.ProductType
	,(SELECT LookupValueName 
		FROM gnMstLookupDtl 
	   WHERE CodeID = 'PRCT' AND 
			 LookUpValue = Items.PartCategory AND 
			 CompanyCode = @CompanyCode) AS CategoryName
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
			CompanyCode = @CompanyCode) AS TypeOfGoods
	FROM SpMstItems Items
	INNER JOIN SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
							 AND Items.PartNo = ItemInfo.PartNo
	LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
							 AND Supplier.SupplierCode = ItemInfo.SupplierCode
	WHERE Items.CompanyCode = @CompanyCode
	  AND Items.BranchCode  = @BranchCode    
	  AND Items.TypeOfGoods = @TypeOfGoods
	  AND Items.ProductType = @ProductType