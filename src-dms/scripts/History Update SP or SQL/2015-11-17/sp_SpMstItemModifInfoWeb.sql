IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_SpMstItemModifInfoWeb]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[sp_SpMstItemModifInfoWeb]
GO

Create procedure [dbo].[sp_SpMstItemModifInfoWeb]  (  @CompanyCode varchar(10),@DynamicFilter varchar(4000) = ''  )    
 as    
 begin  
  DECLARE @Query varchar(max);   
    
   SET @Query = '  
     
   select top 500 * from   
(SELECT  distinct    
                     ItemInfo.PartNo    
                    ,ItemInfo.ProductType    
        ,(SELECT LookupValueName     
                        FROM gnMstLookupDtl     
                       WHERE CodeID = ''PRDT'' AND     
                             LookUpValue = ItemInfo.ProductType AND     
                             CompanyCode = ''' + @CompanyCode +''') AS ProductTypeName    
                    ,(SELECT LookupValueName     
                        FROM gnMstLookupDtl     
                       WHERE CodeID = ''PRCT'' AND     
                             LookUpValue = ItemInfo.PartCategory AND     
                             CompanyCode = ''' + @CompanyCode +''') AS CategoryName    
                    ,ItemInfo.PartCategory    
                    ,ItemInfo.PartName    
                    ,CASE ItemInfo.IsGenuinePart WHEN 1 THEN ''Ya'' ELSE ''Tidak'' END AS IsGenuinePart    
                    ,ItemInfo.OrderUnit    
                    ,ItemInfo.SupplierCode    
                    ,Supplier.SupplierName ,ItemInfo.CompanyCode     
                FROM SpMstItemInfo ItemInfo    
                LEFT JOIN GnMstSupplier Supplier ON     
                    Supplier.CompanyCode  = ItemInfo.CompanyCode     
                AND Supplier.SupplierCode = ItemInfo.SupplierCode      
    where  ItemInfo.CompanyCode= ''' +@CompanyCode +''') itm  
    where 1=1 ' + @DynamicFilter  
      
      
exec (@Query)     
end  
GO