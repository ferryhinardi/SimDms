ALTER procedure [dbo].[sp_SpItemPriceView] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
 as

declare @query varchar(max)

declare @DbMD varchar(20)
declare @CompanyMD varchar (20)
declare @BranchMD varchar (20)

set @DbMD = dbo.GetDbMD( @CompanyCode ,@BranchCode ) 
set @CompanyMD = dbo.GetCompanyMD( @CompanyCode , @BranchCode )
set @BranchMD = dbo.GetBranchMD( @CompanyCode , @BranchCode ) 

set @query = ' 
SELECT 
 Items.CompanyCode 
 ,Items.BranchCode
 ,ItemInfo.PartNo
,ItemInfo.PartName
,ItemPrice.PurchasePrice
,ItemInfo.SupplierCode
,ItemPrice.RetailPriceInclTax
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN ''Ya'' ELSE ''Tidak'' END AS IsGenuinePart
,Items.ProductType
,Items.PartCategory
,u.LookupValueName 
 as CategoryName
 ,ItemPrice.CostPrice
 ,ItemPrice.RetailPrice
 ,ItemPrice.LastPurchaseUpdate
 ,ItemPrice.LastRetailPriceUpdate
,ItemPrice.OldCostPrice
,ItemPrice.OldPurchasePrice
,ItemPrice.OldRetailPrice
FROM spMstItemPrice ItemPrice 
INNER JOIN '+@DbMD +'..spMstItems Items 
    ON ItemPrice.PartNo=Items.PartNo
right JOIN spMstItemInfo ItemInfo 
    ON ItemPrice.CompanyCode = ItemInfo.CompanyCode 
    AND ItemPrice.PartNo = ItemInfo.PartNo
	inner join  gnMstLookUpDtl u on (Items.PartCategory =u.ParaValue)
WHERE  u.CodeID=''PRCT''
and Items.CompanyCode=  '+ @CompanyMD+'  and Items.BranchCode= '+ @BranchMD+'
and ItemPrice.CompanyCode=  '+ @CompanyCode+'  and ItemPrice.BranchCode= '+ @BranchCode

exec(@query)