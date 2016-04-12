USE [BIT201310]
GO

/****** Object:  StoredProcedure [dbo].[sp_EdpTransNo]    Script Date: 07/04/2014 08:41:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO







CREATE procedure [dbo].[sp_MaintainAvgCostItem] (  

@CompanyCode varchar(10),
@BranchCode varchar(10),
@ProductType varchar(10),
@PartNo varchar (20),
@Option varchar (2)
)


as

IF @Option = 'A'
BEGIN

SELECT TOP 1500
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
,ISNULL(ItemLoc.WarehouseCode,0) WarehouseCode
,ISNULL(ItemLoc.LocationCode,0) LocationCode
,(ISNULL(ItemLoc.OnHand,0) - (ISNULL(ItemLoc.AllocationSP,0) + ISNULL(ItemLoc.AllocationSR,0) + ISNULL(ItemLoc.AllocationSL,0) + ISNULL(ItemLoc.ReservedSP,0) + ISNULL(ItemLoc.ReservedSR,0) + ISNULL(ItemLoc.ReservedSL,0))) AS QtyAvail
,ISNULL(ItemPrice.RetailPrice,0) RetailPrice
,ISNULL(ItemPrice.RetailPriceInclTax,0) RetailPriceInclTax
FROM SpMstItems Items
LEFT JOIN SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                         AND Items.PartNo = ItemInfo.PartNo
LEFT JOIN SpMstItemLoc ItemLoc ON Items.CompanyCode  = ItemLoc.CompanyCode
	AND Items.BranchCode = ItemLoc.BranchCode	
	AND Items.PartNo = ItemLoc.PartNo
LEFT JOIN SpMstItemPrice ItemPrice ON Items.CompanyCode  = ItemPrice.CompanyCode
	AND Items.BranchCode = ItemPrice.BranchCode	
	AND Items.PartNo = ItemPrice.PartNo		 
LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
                         AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE Items.CompanyCode = @CompanyCode
  AND Items.BranchCode  = @BranchCode    
  AND Items.ProductType = @ProductType
  AND ItemLoc.WarehouseCode = '00'
END
ELSE
BEGIN

SELECT TOP 1500
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
,Items.Onhand
,ItemInfo.SupplierCode
,Supplier.SupplierName
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'TPGO' AND 
        LookUpValue = Items.TypeOfGoods AND 
        CompanyCode = @CompanyCode) AS TypeOfGoods
,ItemLoc.WarehouseCode
,ItemLoc.LocationCode
,(ItemLoc.OnHand - (ItemLoc.AllocationSP + ItemLoc.AllocationSR + ItemLoc.AllocationSL + ItemLoc.ReservedSP + ItemLoc.ReservedSR + ItemLoc.ReservedSL)) AS QtyAvail
,ItemPrice.RetailPrice
,ItemPrice.CostPrice
,ItemPrice.RetailPriceInclTax
FROM SpMstItems Items with (nolock, nowait)
LEFT JOIN SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                         AND Items.PartNo = ItemInfo.PartNo
LEFT JOIN SpMstItemLoc ItemLoc ON Items.CompanyCode  = ItemLoc.CompanyCode
	AND Items.BranchCode = ItemLoc.BranchCode	
	AND Items.PartNo = ItemLoc.PartNo
LEFT JOIN SpMstItemPrice ItemPrice ON Items.CompanyCode  = ItemPrice.CompanyCode
	AND Items.BranchCode = ItemPrice.BranchCode	
	AND Items.PartNo = ItemPrice.PartNo		 
LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
                         AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE Items.CompanyCode = @CompanyCode
  AND Items.BranchCode  = @BranchCode    
  AND Items.ProductType = @ProductType
  AND Items.PartNo      = @PartNo
  AND ItemLoc.WarehouseCode = '00'
  END

GO


