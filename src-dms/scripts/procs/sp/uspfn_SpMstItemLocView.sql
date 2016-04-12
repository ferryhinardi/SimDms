ALTER PROCEDURE [dbo].[uspfn_SpMstItemLocView]
@CompanyCode varchar(15),
@BranchCode varchar(15) ,
@TypeOfGoods varchar(15) ,
@ProductType varchar(15) 
 
AS
SELECT 
	 ItemLoc.PartNo
	,ItemInfo.PartName
	,ItemInfo.SupplierCode
	,ItemLoc.WarehouseCode
	,(select LookUpValueName from gnMstLookUpDtl where CompanyCode=ItemLoc.CompanyCode and ItemLoc.WarehouseCode =LookUpValue and CODEID='WRCD') [WarehouseName]
	,ItemLoc.LocationCode
	,Items.PartCategory
	,Items.CompanyCode
	,Items.BranchCode   
	,Items.ProductType
	,Items.TypeOfGoods
	,ItemLoc.LocationSub1
	,ItemLoc.LocationSub2
	,ItemLoc.LocationSub3
	,ItemLoc.LocationSub4
	,ItemLoc.LocationSub5
	,ItemLoc.LocationSub6
FROM spMstItemLoc ItemLoc
INNER JOIN spMstItems Items 
    ON ItemLoc.CompanyCode=Items.CompanyCode
    AND ItemLoc.BranchCode=Items.BranchCode
    AND ItemLoc.PartNo=Items.PartNo
INNER JOIN spMstItemInfo ItemInfo 
    ON ItemLoc.CompanyCode=ItemInfo.CompanyCode
    AND ItemLoc.PartNo=ItemInfo.PartNo
WHERE
	ItemLoc.WarehouseCode NOT LIKE 'X%'
	and Items.CompanyCode = @companycode
	and Items.BranchCode = @branchcode
	AND Items.TypeOfGoods=@TypeOfGoods
    AND Items.ProductType=@ProductType