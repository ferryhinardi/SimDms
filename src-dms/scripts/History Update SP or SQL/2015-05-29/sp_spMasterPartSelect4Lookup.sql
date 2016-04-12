ALTER procedure [dbo].[sp_spMasterPartSelect4Lookup] ( 
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@TypeOfGoods varchar(15)
	,@ProductType varchar(15)
)
AS
	declare @query varchar(max)

declare @DbMD varchar(20)
declare @CompanyMD varchar (20)
declare @BranchMD varchar (20)

set @DbMD = dbo.GetDbMD( @CompanyCode ,@BranchCode ) 
set @CompanyMD = dbo.GetCompanyMD( @CompanyCode , @BranchCode )
set @BranchMD = dbo.GetBranchMD( @CompanyCode , @BranchCode ) 

set @query ='
	SELECT 
	 Items.PartNo
	,Items.ProductType
	,(SELECT LookupValueName 
		FROM gnMstLookupDtl 
	   WHERE CodeID = ''PRCT'' AND 
			 LookUpValue = Items.PartCategory AND 
			 CompanyCode = '+ @CompanyCode +') AS CategoryName
	,Items.PartCategory
	,ItemInfo.PartName
	,CASE ItemInfo.IsGenuinePart WHEN 1 THEN ''Ya'' ELSE ''Tidak'' END AS IsGenuinePart
	,CASE Items.Status WHEN 1 THEN ''Aktif'' ELSE ''Tidak'' END AS IsActive
	,ItemInfo.OrderUnit
	,ItemInfo.SupplierCode
	,Supplier.SupplierName
	,(SELECT LookupValueName 
		FROM gnMstLookupDtl 
	  WHERE CodeID = ''TPGO'' AND 
			LookUpValue = Items.TypeOfGoods AND 
			CompanyCode = '+ @CompanyCode +') AS TypeOfGoods
	FROM '+ @DbMD +'..SpMstItems Items
	INNER JOIN SpMstItemInfo ItemInfo ON Items.PartNo = ItemInfo.PartNo
	LEFT JOIN GnMstSupplier Supplier ON Supplier.SupplierCode = ItemInfo.SupplierCode
	WHERE 1 = 1 
	  AND Items.CompanyCode = '+@CompanyMD+'
	  AND Items.BranchCode  = '+@BranchMD   +' 
	  AND Items.TypeOfGoods = '+@TypeOfGoods +'
	  AND Items.ProductType = '''+@ProductType+ '''
	  AND ItemInfo.CompanyCode = '+@CompanyCode+'
	  AND Supplier.CompanyCode = '+@CompanyCode

	  exec (@query)
