ALTER procedure [dbo].[uspfn_SpMstItemLocView]  
@CompanyCode varchar(15),  
@BranchCode varchar(15),  
@TypeOfGoods varchar(5),  
@ProductType varchar(15)  
   
AS  
SELECT   
  ItemLoc.PartNo  
 ,ItemInfo.PartName  
 ,ItemInfo.SupplierCode  
 ,ItemLoc.WarehouseCode  
 ,c.LookUpValueName [WarehouseName]  
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
 inner join gnMstLookUpDtl c  ON ItemLoc.CompanyCode = c.CompanyCode   
       AND ItemLoc.WarehouseCode = c.LookUpValue
	   AND c.CodeID = 'WRCD'   
WHERE  
 Items.CompanyCode= @CompanyCode
    AND Items.BranchCode=@BranchCode    
    AND Items.TypeOfGoods=@TypeOfGoods
    AND Items.ProductType=@ProductType
    AND Items.TypeOfGoods=@TypeOfGoods
	AND ItemLoc.WarehouseCode NOT LIKE 'X%' 
     