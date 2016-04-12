create procedure sp_SpPosLkp  
@CompanyCode varchar(15),    
@BranchCode varchar(15),     
@TypeOfGoods  varchar(15)  
as  
  
SELECT   
 a.POSNo  
,a.PosDate  
,ISNULL(a.IsDeleted, 0) IsDeleted  
,a.SupplierCode  
,b.SupplierName  
FROM spTrnPPOSHdr a  
INNER JOIN gnMstSupplier b ON b.SupplierCode = a.SupplierCode and b.CompanyCode = a.CompanyCode  
WHERE a.CompanyCode=@CompanyCode   
   AND a.BranchCode=@BranchCode  
      AND a.IsDeleted=0   
      AND TypeOfGoods = @TypeOfGoods  
ORDER BY a.POSNo ASC  

GO