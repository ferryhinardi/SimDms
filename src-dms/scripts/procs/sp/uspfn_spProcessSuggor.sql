/****** Object:  StoredProcedure [dbo].[uspfn_spProcessSuggor]    Script Date: 8/29/2014 9:38:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[uspfn_spProcessSuggor] 
(  
		@CompanyCode varchar(10),
		@BranchCode varchar(10),
		@MovingCode varchar(5),
		@SupplierCode  varchar(10),
		@TypeOfGoods  varchar(15)
)
 as
SELECT
 a.PartNo
,ISNULL(a.DemandAverage, 0) AS DemandAverage
,ISNULL(c.LeadTime, 0) AS LeadTime
,ISNULL(c.OrderCycle, 0) AS OrderCycle
,ISNULL(c.SafetyStock, 0) AS SafetyStock
,CAST(0 AS int) AS No
,CAST(0 AS Numeric(4,0)) AS SeqNo
,CAST(ISNULL(a.OnHand, 0) - (
    ISNULL(a.AllocationSP, 0) 
    + ISNULL(a.AllocationSL, 0) 
    + ISNULL(a.AllocationSR, 0)
    + ISNULL(a.ReservedSP, 0) 
    + ISNULL(a.ReservedSL, 0) 
    + ISNULL(a.ReservedSR, 0)
) AS decimal(18,2)) AS AvailableQty
,CAST(0 AS Numeric(4,0)) AS SuggorQty
,CAST(0 AS Numeric(4,0)) AS SuggorCorrecQty
,CAST('' AS varchar(3)) AS ProductType
,a.PartCategory
,CAST(0 AS Numeric(18,0)) AS PurchasePrice
,CAST(0 AS Numeric(18,0)) AS CostPrice
,ISNULL(a.OrderPointQty, 0) AS OrderPoint
,ISNULL(a.OnHand, 0) AS OnHand
,ISNULL(a.OnOrder, 0) AS OnOrder
,ISNULL(a.InTransit, 0) AS InTransit
,ISNULL(a.AllocationSP, 0) AS AllocationSP
,ISNULL(a.AllocationSR, 0) AS AllocationSR
,ISNULL(a.AllocationSL, 0) AS AllocationSL
,ISNULL(a.BackOrderSP, 0) AS BackOrderSP
,ISNULL(a.BackOrderSR, 0) AS BackOrderSR
,ISNULL(a.BackOrderSL, 0) AS BackOrderSL
,ISNULL(a.ReservedSP, 0) AS ReservedSP
,ISNULL(a.ReservedSR, 0) AS ReservedSR
,ISNULL(a.ReservedSL, 0) AS ReservedSL
FROM spMstItems a with(nolock, nowait)
INNER JOIN spMstItemInfo b with(nolock, nowait) ON b.CompanyCode=a.CompanyCode AND b.PartNo=a.PartNo
INNER JOIN SpMstOrderParam c with(nolock, nowait) ON c.CompanyCode=a.CompanyCode AND c.BranchCode=a.BranchCode AND 
		   c.SupplierCode=b.SupplierCode AND c.MovingCode=a.MovingCode
WHERE a.CompanyCode=@CompanyCode 
AND a.BranchCode=@BranchCode
AND a.MovingCode=@MovingCode
AND b.SupplierCode=@SupplierCode
AND a.TypeOfGoods=@TypeOfGoods
AND a.Status = '1'