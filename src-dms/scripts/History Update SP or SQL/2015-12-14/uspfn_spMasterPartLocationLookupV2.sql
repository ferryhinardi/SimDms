-- =============================================
-- Author:		fhy
-- Create date: 27112015
-- Description:	get MasterPartLocationLookupV2
-- =============================================
ALTER PROCEDURE [dbo].[uspfn_spMasterPartLocationLookupV2] 

	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@TypeOfGoods varchar(15),
	@ProductType varchar(15),
	@dynamicfilters varchar(max)=''

AS
BEGIN

declare
@query varchar(max)

set @query='select top 500 * from (SELECT 
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
	FROM SpMstItems Items
	INNER JOIN SpMstItemInfo ItemInfo ON Items.CompanyCode  = ItemInfo.CompanyCode                          
							 AND Items.PartNo = ItemInfo.PartNo
	INNER JOIN spMstItemPrice ItemPrice ON Items.CompanyCode = ItemPrice.CompanyCode
							AND Items.BranchCode = ItemPrice.BranchCode AND Items.PartNo = ItemPrice.PartNo
	INNER JOIN spMstItemLoc ItemLoc ON Items.CompanyCode = ItemLoc.CompanyCode AND Items.BranchCode = ItemLoc.BranchCode
							AND Items.PartNo = ItemLoc.PartNo
	WHERE Items.CompanyCode = '''+@CompanyCode+'''
	  AND Items.BranchCode  = '''+@BranchCode +'''   
	  AND Items.TypeOfGoods = '''+@TypeOfGoods+'''
	  AND Items.ProductType = '''+@ProductType+'''
	  AND Items.Status > 0
	  AND ItemLoc.WarehouseCode = ''00'' ) a
	  where 1=1
	  '+@dynamicfilters+'
	  ORDER BY a.PartNo'

print(@query)
exec (@query)


END


