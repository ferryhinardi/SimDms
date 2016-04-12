USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_spMasterPartView]    Script Date: 8/5/2014 9:18:51 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[uspfn_spMasterPartView]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@PartNo varchar(50)
AS
if @PartNo=''
SELECT 
	ItemInfo.PartNo,
	Items.ABCClass,
	ItemLoc.OnHand - itemLoc.ReservedSP - itemLoc.ReservedSR - itemLoc.ReservedSL - itemLoc.AllocationSP - itemLoc.AllocationSL - itemLoc.AllocationSR AS AvailQty,
	Items.OnOrder,
	Items.ReservedSP,
	Items.ReservedSR,
	Items.ReservedSL,
	Items.MovingCode,
	ItemInfo.SupplierCode,
	ItemInfo.PartName,
	ItemPrice.RetailPrice,
	ItemPrice.RetailPriceInclTax,
	ItemPrice.PurchasePrice
	,Items.CompanyCode
	,Items.BranchCode   
	,Items.ProductType
	,Items.TypeOfGoods
FROM SpMstItems Items
INNER JOIN SpMstItemInfo ItemInfo ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                         AND Items.PartNo = ItemInfo.PartNo
INNER JOIN spMstItemPrice ItemPrice ON Items.CompanyCode = ItemPrice.CompanyCode
                        AND Items.BranchCode = ItemPrice.BranchCode AND Items.PartNo = ItemPrice.PartNo
INNER JOIN spMstItemLoc ItemLoc ON Items.CompanyCode = ItemLoc.CompanyCode AND Items.BranchCode = ItemLoc.BranchCode
                        AND Items.PartNo = ItemLoc.PartNo
WHERE Items.Status > 0
  AND ItemLoc.WarehouseCode = '00'
  	and Items.CompanyCode = @CompanyCode
	  AND Items.BranchCode  = @BranchCode  
	  
else
SELECT 
	ItemInfo.PartNo,
	Items.ABCClass,
	ItemLoc.OnHand - itemLoc.ReservedSP - itemLoc.ReservedSR - itemLoc.ReservedSL - itemLoc.AllocationSP - itemLoc.AllocationSL - itemLoc.AllocationSR AS AvailQty,
	Items.OnOrder,
	Items.ReservedSP,
	Items.ReservedSR,
	Items.ReservedSL,
	Items.MovingCode,
	ItemInfo.SupplierCode,
	ItemInfo.PartName,
	ItemPrice.RetailPrice,
	ItemPrice.RetailPriceInclTax,
	ItemPrice.PurchasePrice
	,Items.CompanyCode
	,Items.BranchCode   
	,Items.ProductType
	,Items.TypeOfGoods
FROM SpMstItems Items
INNER JOIN SpMstItemInfo ItemInfo ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                         AND Items.PartNo = ItemInfo.PartNo
INNER JOIN spMstItemPrice ItemPrice ON Items.CompanyCode = ItemPrice.CompanyCode
                        AND Items.BranchCode = ItemPrice.BranchCode AND Items.PartNo = ItemPrice.PartNo
INNER JOIN spMstItemLoc ItemLoc ON Items.CompanyCode = ItemLoc.CompanyCode AND Items.BranchCode = ItemLoc.BranchCode
                        AND Items.PartNo = ItemLoc.PartNo
WHERE Items.Status > 0
  AND ItemLoc.WarehouseCode = '00'
  	and Items.CompanyCode = @CompanyCode
	  AND Items.BranchCode  = @BranchCode   
	  and ItemInfo.PartNo=@PartNo   
 