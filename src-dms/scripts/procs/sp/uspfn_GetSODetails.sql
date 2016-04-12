CREATE procedure [dbo].[uspfn_GetSODetails] @CompanyCode varchar(15), @BranchCode varchar(15), @POSNo varchar(20)  
as   
SELECT   
    A.CompanyCode,  
    A.BranchCode,  
    A.POSNo,   
    A.SupplierCode,   
    B.PartNo,   
    B.OrderQty,   
    B.PurchasePrice,  
    B.CostPrice,   
    B.ABCClass,   
    B.MovingCode,   
    B.ProductType,  
    B.PartCategory,   
    A.TypeOfGoods,  
    B.DiscPct,  
    B.Note,  
    C.PartName,
    B.TotalAmount  
FROM spTrnPPOSHdr A   
INNER JOIN spTrnPPOSDtl B ON (A.CompanyCode = B.CompanyCode)  
    AND (A.BranchCode = B.BranchCode)  
    AND (A.POSNo = B.POSNo)  
left JOIN spMstItemInfo C  
on C.CompanyCode = B.CompanyCode  
and C.PartNo = B.PartNo  
WHERE A.CompanyCode = @CompanyCode  
    AND A.BranchCode = @BranchCode  
    AND A.POSNo = @POSNo