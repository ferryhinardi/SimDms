create Procedure uspfn_spGetSelectCostPrice @CompanyCode varchar(15), @BranchCode varchar(15), @PartNo varchar(25),   
@TypeOfGoods varchar(3), @ProductType varchar(3)  
as  
SELECT A.CompanyCode, A.BranchCode, A.PartNo,  
       C.SupplierCode, A.OnHand, B.CostPrice  
  FROM spMstItems A  
       INNER JOIN spMstItemPrice B   
            ON (A.CompanyCode = B.CompanyCode) AND (A.BranchCode = B.BranchCode) AND (A.PartNo = B.PartNo)  
        INNER JOIN spMstItemInfo C  
            ON C.CompanyCode = A.CompanyCode AND C.PartNo = A.PartNo  
WHERE A.CompanyCode = @CompanyCode  
AND A.BranchCode = @BranchCode  
AND A.TypeOfGoods = @TypeOfGoods  
AND A.ProductType = @ProductType  
AND A.PartNo = @PartNo  
 ORDER BY  
   A.CompanyCode ASC,  
   A.BranchCode ASC,  
   A.PartNo ASC 
  