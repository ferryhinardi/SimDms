
create view [dbo].[SpEdpPartNo_Internal]  as    
SELECT 
    TOP 500 a.CompanyCode, a.BranchCode, a.ProductType, TypeOfGoods, ItemInfo.PartNo,
    ItemInfo.PartName,
    c.OnHand - (c.AllocationSP + c.AllocationSR + c.AllocationSL + c.ReservedSP + c.ReservedSR + c.ReservedSL) AS Available,
	a.MovingCode
FROM 
    SpMstItems a
    INNER JOIN SpMstItemInfo ItemInfo ON ItemInfo.PartNo = a.PartNo 
        AND ItemInfo.CompanyCode = a.CompanyCode
    INNER JOIN SpMstItemLoc c ON a.CompanyCode = c.CompanyCode
        AND a.BranchCode = c.BranchCode
        AND a.PartNo = c.PartNo
    INNER JOIN SpMstItemPrice d ON a.CompanyCode = d.CompanyCode 
		AND a.BranchCode = d.BranchCode
		AND a.PArtNo = d.PartNo
WHERE a.Status = '1'  AND WarehouseCode = '00'
/*    a.CompanyCode = @CompanyCode
    AND a.BranchCode  = @BranchCode 
    AND a.ProductType  = @ProductType
    AND a.TypeOfGoods = @TypeOfGoods
    AND a.Status = @Status
    AND c.WarehouseCode = @WarehouseCode*/

GO


