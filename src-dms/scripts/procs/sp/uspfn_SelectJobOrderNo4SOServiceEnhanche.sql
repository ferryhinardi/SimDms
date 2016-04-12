-- =============================================
-- Author:		<yo>
-- Create date: <26 April 2012>
-- Description:	<SelectJobOrderNo4SOServiceEnhanche> 
-- =============================================
ALTER procedure [dbo].[uspfn_SelectJobOrderNo4SOServiceEnhanche]
	@CompanyCode  varchar(15),
	@BranchCode  varchar(15),
	@ProductType  varchar(15),
	@JobOrderNo  varchar(15)
	
AS
BEGIN

--declare @CompanyCode as varchar(15)
--declare @BranchCode as varchar(15)
--declare @ProductType as varchar(15)
--declare @TypeOfGoods as varchar(15)
--declare @JobOrderNo as varchar(15)

--set @CompanyCode = '6159401000'
--set @BranchCode = '6159401001'
--set @ProductType = '4W'
--set @JobOrderNo = 'SPK/15/002333'

SELECT 
ROW_number() OVER(ORDER BY a.TipePart, a.PartNo ASC) AS NoUrut, * FROM (
SELECT 
DISTINCT(a.CompanyCode) 
    , a.BranchCode
	, a.ProductType
	, ServiceNo
	, a.PartNo
    , (SELECT Paravalue FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'GTGO' AND LookUpValue = a.TypeOfGoods) TipePart
    , (SELECT PartName FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) PartName
	, a.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
	, SUM(QtyOrder) QtyOrder
    , 0 QtySupply
    , 0 QtyBO
    , (SUM(QtyOrder) * a.RetailPrice) * ((100 - PartDiscPct)/100) NetSalesAmt
    , PartDiscPct DiscPct
FROM
(
SELECT
	DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.RetailPrice
	, c.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, ISNULL(Item.QtyOrder,0) AS QtyOrder
    , a.DiscPct PartDiscPct 
FROM
	svTrnSrvItem a WITH (NOLOCK, NOWAIT)
	LEFT JOIN svTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.ProductType = b.ProductType
		AND a.ServiceNo = b.ServiceNo
	LEFT JOIN (SELECT CompanyCode, BranchCode, ProductType, ServiceNo, RetailPrice, PartNo,BillType,
				CASE WHEN SUM(ISNULL(DemandQty, 0)) > SUM(ISNULL(SupplyQty, 0)) THEN SUM(ISNULL(DemandQty, 0) - (ISNULL(SupplyQty, 0))) 
				ELSE SUM(ISNULL(DemandQty, 0)) END QtyOrder
				FROM svTrnSrvItem 
				GROUP BY CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice, BillType) Item ON
		Item.CompanyCode = a.CompanyCode AND Item.BranchCode = a.BranchCode AND Item.ProductType =
			a.ProductType AND Item.ServiceNo = a.ServiceNo AND Item.PartNo = a.PartNo AND Item.RetailPrice = a.RetailPrice AND a.Billtype = Item.BillType
    LEFT JOIN SpMstItemPrice c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.PartNo = c.PartNo 
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	--AND Item.QtyOrder > 0
	AND JobOrderNo = @JobOrderNo
) a
LEFT JOIN SpMstItemPrice c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.PartNo = c.PartNo
GROUP BY
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, ServiceNo
	, a.PartNo
	, a.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
    , PartDiscPct 
) a ORDER BY a.TipePart, a.PartNo ASC

END