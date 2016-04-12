CREATE PROCEDURE [dbo].[uspfn_spMasterPartLookupNew]
	@CompanyCode varchar(15),
	@TypeOfGoods varchar(15),
	@ProductType varchar(15)
AS
SELECT * FROM (
SELECT DISTINCT ItemInfo.PartNo
,ItemInfo.PartName
,ItemInfo.CompanyCode
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
,ItemInfo.ProductType
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'PRCT' AND 
        LookUpValue = ItemInfo.PartCategory AND 
        CompanyCode = @CompanyCode) AS CategoryName
,ItemInfo.PartCategory
,ItemInfo.OrderUnit
,ItemInfo.SupplierCode
,Supplier.SupplierName
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'TPGO' AND 
        LookUpValue = Items.TypeOfGoods AND 
        CompanyCode = @CompanyCode) AS TypeOfGoods
, (SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'STPR' AND 
        LookUpValue = Items.Status AND 
        CompanyCode = @CompanyCode) AS Status
FROM SpMstItemInfo ItemInfo
LEFT JOIN SpMstItems Items    ON ItemInfo.CompanyCode = Items.CompanyCode AND ItemInfo.PartNo = Items.PartNo
LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode = ItemInfo.CompanyCode AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE ItemInfo.CompanyCode = @CompanyCode AND ItemInfo.ProductType = @ProductType AND Items.TypeOfGoods = @TypeOfGoods
) ItemInfo WHERE ItemInfo.CompanyCode = @CompanyCode