ALTER PROCEDURE [dbo].[uspfn_spMasterPartLookup]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@TypeOfGoods varchar(15),
@ProductType varchar(15),
@SEARCH varchar(50) = ''
AS

--declare @CompanyCode varchar(15),
--		@BranchCode varchar(15),
--		@TypeOfGoods varchar(15),
--		@ProductType varchar(15),
--		@SEARCH varchar(50) = ''

--set @CompanyCode = '6115204'
--set @BranchCode = '611520401'
--set @TypeOfGoods = '0'
--set @ProductType = '2W'
--set @SEARCH = ''

IF (@SEARCH='')
SELECT DISTINCT
 ItemInfo.PartNo
,ItemInfo.ProductType
,(SELECT LookupValueName 
	FROM gnMstLookupDtl 
   WHERE CodeID = 'PRCT' AND 
		 LookUpValue = ItemInfo.PartCategory AND 
		 CompanyCode = @CompanyCode) AS CategoryName
,ItemInfo.PartCategory
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
FROM SpMstItemInfo ItemInfo
INNER JOIN SpMstItems Items    ON ItemInfo.CompanyCode = Items.CompanyCode AND ItemInfo.PartNo = Items.PartNo
INNER JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
						 AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE ItemInfo.CompanyCode = @CompanyCode AND ItemInfo.ProductType = @ProductType

ELSE

SELECT DISTINCT
 ItemInfo.PartNo
,ItemInfo.ProductType
,(SELECT LookupValueName 
	FROM gnMstLookupDtl 
   WHERE CodeID = 'PRCT' AND 
		 LookUpValue = ItemInfo.PartCategory AND 
		 CompanyCode = @CompanyCode) AS CategoryName
,ItemInfo.PartCategory
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
FROM SpMstItemInfo ItemInfo
INNER JOIN SpMstItems Items    ON ItemInfo.CompanyCode = Items.CompanyCode AND ItemInfo.PartNo = Items.PartNo
INNER JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
						 AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE ItemInfo.CompanyCode = @CompanyCode AND ItemInfo.ProductType = @ProductType
  AND (ItemInfo.PartNo LIKE @SEARCH + '%')
