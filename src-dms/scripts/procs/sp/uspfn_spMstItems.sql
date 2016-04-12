
/****** Object:  StoredProcedure [dbo].[uspfn_spMstItems]    Script Date: 6/19/2014 10:44:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_spMstItems] 
@CompanyCode varchar(10),@BranchCode varchar(10),
 @UserId varchar(10),
 @SupplierCode varchar(20),@MovingCode varchar(10),
 @LeadTime decimal, @OrderCycle decimal,@SafetyStock decimal
as
UPDATE spMstItems
SET LeadTime = CONVERT(DECIMAL, @LeadTime),
    OrderCycle = CONVERT(DECIMAL, @OrderCycle),
    SafetyStock = CONVERT(DECIMAL, @SafetyStock),
    SafetyStockQty = CONVERT(DECIMAL, @SafetyStock) * a.DemandAverage,
    OrderPointQty = ((a.DemandAverage * (CONVERT(DECIMAL, @LeadTime) + CONVERT(DECIMAL, @OrderCycle))) + 
                     (a.DemandAverage * CONVERT(DECIMAL, @SafetyStock))),
    LastUpdateBy = @UserId,
    LastUpdateDate = getdate()
FROM spMstItems a INNER JOIN spMstItemInfo b 
ON a.CompanyCode=b.CompanyCode
AND a.PartNo=b.PartNo
WHERE a.CompanyCode=@CompanyCode
    AND a.BranchCode=@BranchCode
    AND b.SupplierCode=@SupplierCode
    AND a.MovingCode=@MovingCode