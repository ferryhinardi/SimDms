
/****** Object:  StoredProcedure [dbo].[uspfn_SpWrongPart]    Script Date: 6/19/2014 10:57:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- uspfn_SvGetClaim '6006406','6006406','4W','CLA/11/000002'
CREATE procedure [dbo].[uspfn_SpWrongPart]
--declare 
	 @CompanyCode  varchar(20),
	 @ProductType  varchar(20)
--
as

SELECT TOP 1500 * FROM (
SELECT DISTINCT ItemInfo.PartNo as PartNoWrong
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
WHERE ItemInfo.CompanyCode = @CompanyCode AND ItemInfo.ProductType = @ProductType) ItemInfo WHERE ItemInfo.CompanyCode = @CompanyCode

