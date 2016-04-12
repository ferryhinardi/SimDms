
/****** Object:  StoredProcedure [dbo].[uspfn_spMasterItemBrowse]    Script Date: 6/19/2014 10:57:49 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_spMasterItemBrowse]
@CompanyCode varchar(15), 
@BranchCode varchar(15),
@TPGO varchar(15), 
@ProductType varchar(15),
@SEARCH varchar(50) = ''
AS
IF @SEARCH<>''
SELECT 
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
,Items.TypeOfGoods 
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

WHERE Items.CompanyCode = @CompanyCode
  AND Items.BranchCode  = @BranchCode    
  AND Items.TypeOfGoods = @TPGO
  AND Items.ProductType = @ProductType
  AND (
	Items.PartNo LIKE @SEARCH + '%' OR
	ItemInfo.PartName  LIKE @SEARCH + '%'
  )

ELSE

SELECT 
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
,Items.TypeOfGoods 
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

WHERE Items.CompanyCode = @CompanyCode
  AND Items.BranchCode  = @BranchCode    
  AND Items.TypeOfGoods = @TPGO
  AND Items.ProductType = @ProductType
