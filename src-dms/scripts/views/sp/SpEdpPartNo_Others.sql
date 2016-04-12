


create view [dbo].[SpEdpPartNo_Others]  as    
SELECT 
    TOP 500 a.CompanyCode, a.BranchCode, SupplierCode, a.ProductType, TypeOfGoods, ItemInfo.PartNo,
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
WHERE a.Status = '1'
    AND c.WarehouseCode = '00'

GO


