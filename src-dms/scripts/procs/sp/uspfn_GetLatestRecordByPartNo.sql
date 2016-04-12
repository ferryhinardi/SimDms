
CREATE procedure uspfn_spGetLatestRecordByPartNo @CompanyCode varchar(15), @BranchCode varchar(15), @PartNo  varchar(25)  
as  
SELECT  
   CompanyCode,BranchCode,PartNo,UpdateDate,  
   ISNULL(RetailPrice,0) AS RetailPrice,   
   ISNULL(RetailPriceInclTax,0) AS RetailPriceInclTax,   
   ISNULL(PurchasePrice,0) AS PurchasePrice,  
   ISNULL(OldRetailPrice,0) AS OldRetailPrice,   
   ISNULL(OldPurchasePrice,0) AS OldPurchasePrice,  
   ISNULL(Discount,0) AS Discount, ISNULL(OldDiscount,0) AS OldDiscount,  
   ISNULL(CostPrice,0) AS CostPrice, ISNULL(OldCostPirce,0) AS OldCostPirce,  
   CreatedBy,CreatedDate  
FROM  
 spHstItemPrice  
WHERE  
 CompanyCode = @CompanyCode  
 AND BranchCode = @BranchCode  
 AND PartNo = @PartNo  
 AND UpdateDate = ( SELECT  
       MAX(UpdateDate)  
      FROM  
       spHstItemPrice  
      WHERE  
       CompanyCode = @CompanyCode  
       AND BranchCode = @BranchCode  
       AND PartNo = @PartNo)