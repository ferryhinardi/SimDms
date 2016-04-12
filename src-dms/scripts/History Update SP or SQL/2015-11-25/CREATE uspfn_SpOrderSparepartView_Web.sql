CREATE PROCEDURE [dbo].[uspfn_SpOrderSparepartView_Web] (@CompanyCode varchar(10) ,@BranchCode varchar(10), @TypeOfGoods varchar(2), @ProductType varchar(2),  @DynamicFilter varchar(4000) = '', @top int = 100) 
	as
DECLARE @Query VARCHAR(MAX);
SET @Query = 'SELECT * FROM (
	SELECT TOP ' + CONVERT(VARCHAR, @top) + '    
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
		ItemInfo.DiscPct,
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
		WHERE Items.CompanyCode = ''' + @CompanyCode + ''' 
		  AND Items.BranchCode  = ''' + @BranchCode + '''
		  AND Items.TypeOfGoods = ''' + @TypeOfGoods + '''
		  AND Items.ProductType = ''' + @ProductType + ''' 
		  AND Items.Status > 0 
		  AND ItemLoc.WarehouseCode = ''00'') x ' 
			+ CASE @DynamicFilter WHEN '' THEN '' ELSE ' WHERE '
			+ RIGHT(@DynamicFilter, LEN(@DynamicFilter)-5) END

         
EXEC(@Query)
