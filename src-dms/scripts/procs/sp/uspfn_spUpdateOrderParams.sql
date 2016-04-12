
/****** Object:  StoredProcedure [dbo].[uspfn_spUpdateOrderParams]    Script Date: 6/19/2014 10:56:07 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_spUpdateOrderParams]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@SupplierCode varchar(15),
@MovingCode varchar(15),
@LeadTime DECIMAL,
@OrderCycle DECIMAL,
@SafetyStock DECIMAL,
@UserId varchar(32)
AS
UPDATE spMstItems
SET LeadTime = @LeadTime,
    OrderCycle = @OrderCycle,
    SafetyStock = @SafetyStock,
    SafetyStockQty = @SafetyStock * a.DemandAverage,
    OrderPointQty = (a.DemandAverage * (@LeadTime +  @OrderCycle)) + 
                     (a.DemandAverage *  @SafetyStock),
    LastUpdateBy = @UserId,
    LastUpdateDate = getdate()
FROM spMstItems a INNER JOIN spMstItemInfo b 
ON a.CompanyCode=b.CompanyCode
AND a.PartNo=b.PartNo
WHERE a.CompanyCode=@CompanyCode
    AND a.BranchCode=@BranchCode
    AND b.SupplierCode=@SupplierCode
    AND a.MovingCode=@MovingCode