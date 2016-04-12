---=========== alter uspfn_sp_partinquiry dynamic query MD/SD ===============

ALTER procedure [dbo].[uspfn_sp_partinquiry]
@CompanyCode varchar(15), @BranchCode varchar(15), @TypeOfGoods varchar(2), @ProductType varchar(2)
AS
begin

--declare @CompanyCode varchar(15), @BranchCode varchar(15), @TypeOfGoods varchar(2), @ProductType varchar(2)
--set @CompanyCode = '6159401000'
--set @BranchCode = '6159401001'
--set @TypeOfGoods = '0'
--set @ProductType = '4W'

DECLARE @Query varchar(max);
SET @Query = 'SELECT 
		 Items.PartNo,ItemInfo.PartName,ItemLoc.WarehouseCode
		,ItemLoc.LocationCode
		,(ItemLoc.OnHand - (ItemLoc.AllocationSP + ItemLoc.AllocationSR + ItemLoc.AllocationSL + ItemLoc.ReservedSP + ItemLoc.ReservedSR + ItemLoc.ReservedSL)) AS QtyAvail
		,Items.OnOrder,ItemPrice.RetailPriceInclTax
		,CASE ItemInfo.IsGenuinePart WHEN 1 THEN ''Ya'' ELSE ''Tidak'' END AS IsGenuinePart
		,ItemInfo.SupplierCode,ItemPrice.RetailPrice
		,Items.ProductType,Items.PartCategory
		,(SELECT LookupValueName 
			FROM gnMstLookupDtl 
		   WHERE CodeID = ''PRCT'' AND 
				 LookUpValue = Items.PartCategory AND 
				 CompanyCode = ''' + @CompanyCode + ''') AS CategoryName
		,CASE Items.Status WHEN 1 THEN ''Aktif'' ELSE ''Tidak'' END AS IsActive
		,ItemInfo.OrderUnit
		,Supplier.SupplierName
		,(SELECT LookupValueName 
			FROM gnMstLookupDtl 
		  WHERE CodeID = ''TPGO'' AND 
				LookUpValue = Items.TypeOfGoods AND 
				CompanyCode =  ''' + @CompanyCode + ''') AS TypeOfGoods
		FROM ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..SpMstItems Items
		INNER JOIN ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
								 AND Items.PartNo = ItemInfo.PartNo
		INNER JOIN ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..SpMstItemLoc ItemLoc ON Items.CompanyCode  = ItemLoc.CompanyCode
			AND Items.BranchCode = ItemLoc.BranchCode	
			AND Items.PartNo = ItemLoc.PartNo
		INNER JOIN ' + dbo.GetDbMD(@CompanyCode, @BranchCode) + '..SpMstItemPrice ItemPrice ON Items.CompanyCode  = ItemPrice.CompanyCode
			AND Items.BranchCode = ItemPrice.BranchCode	
			AND Items.PartNo = ItemPrice.PartNo		 
		LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
								 AND Supplier.SupplierCode = ItemInfo.SupplierCode
		WHERE Items.CompanyCode =  ''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + '''
		AND Items.BranchCode  = ''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''    
		  AND Items.TypeOfGoods =''' +  @TypeOfGoods + '''
		  AND Items.ProductType = ''' + @ProductType + ''' 
		  AND ItemLoc.WarehouseCode = ''00''';

		print(@Query)
		exec (@Query);
END