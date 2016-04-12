CREATE procedure sp_SpSOSelectforInsert
	(  @CompanyCode varchar(10) ,@BranchCode varchar(10),@LocationCode varchar(10))
 as
SELECT 
	 ItemLoc.PartNo	
	,ItemLoc.WarehouseCode
	,ItemLoc.LocationCode
	,Items.PartCategory
	,Items.CompanyCode
	,Items.BranchCode   
	,Items.ProductType
	,Items.TypeOfGoods
	,itemloc.OnHand
FROM spMstItemLoc ItemLoc
INNER JOIN spMstItems Items 
    ON ItemLoc.CompanyCode=Items.CompanyCode
    AND ItemLoc.BranchCode=Items.BranchCode
    AND ItemLoc.PartNo=Items.PartNo
WHERE	
	 Items.CompanyCode=@CompanyCode and Items.BranchCode=@BranchCode and ItemLoc.LocationCode like '%' +@LocationCode