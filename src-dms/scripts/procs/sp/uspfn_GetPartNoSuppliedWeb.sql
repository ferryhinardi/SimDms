
CREATE procedure [dbo].[uspfn_GetPartNoSuppliedWeb]   
(  
--DECLARE
@CompanyCode as varchar(15),
@BranchCode as varchar(15),
@ProductType as varchar(15),
@JobOrderNo as varchar(15),
@DocNo as varchar(15)

--set @CompanyCode = '6006406'
--set @BranchCode = '6006400'
--set	@ProductType = '4W'
--set	@JobOrderNo = 'SPK/12/000002'
--set @DocNo = 'SSS/12/000010'

)  
AS  

begin

declare @md bit
declare @dbMD varchar(25)
declare @CompanyMD varchar(25)

set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

if(@md = 1)
BEGIN

SELECT 
DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, ServiceNo
    , SupplySlipDate
	, a.PartNo
	, c.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
	, SUM(QtyOrder) QtyOrder
    , QtySupply
    , DiscPct
FROM
(
SELECT
	DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
    , c.DocDate SupplySlipDate
	, a.PartNo
	, a.RetailPrice
	, f.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, Item.QtyOrder	
    , ISNULL(e.QtySupply,0) QtySupply
    , a.DiscPct
FROM
	SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
	LEFT JOIN SvTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.ProductType = b.ProductType
		AND a.ServiceNo = b.ServiceNo
	LEFT JOIN (SELECT CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice,
				SUM(ISNULL(DemandQty, 0) - ISNULL(SupplyQty, 0)) QtyOrder
				FROM SvTrnSrvItem 
				GROUP BY CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice) Item ON
		Item.CompanyCode = a.CompanyCode AND Item.BranchCode = a.BranchCode AND Item.ProductType =
			a.ProductType AND Item.ServiceNo = a.ServiceNo AND Item.PartNo = a.PartNo and Item.RetailPrice = a.RetailPrice
	LEFT JOIN SpTrnSORDHdr c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND b.JobOrderNo = c.UsageDocNo
	LEFT JOIN SpTrnSOSupply e WITH (NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
		AND a.BranchCode = e.BranchCode
		AND c.DocNo = e.DocNo
		AND a.PartNo = e.PartNo
    INNER JOIN gnMstCompanyMapping cm WITH(NOLOCK, NOWAIT) ON cm.CompanyCode = a.CompanyCode
    LEFT JOIN SpMstItemPrice f WITH(NOLOCK, NOWAIT) ON cm.CompanyMD = f.CompanyCode
		AND a.BranchCode = f.BranchCode
		AND a.PartNo = f.PartNo
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND Item.QtyOrder > 0
    AND c.DocNo = @DocNo
	AND JobOrderNo = @JobOrderNo
    AND a.RetailPrice = Item.RetailPrice
)a
LEFT JOIN SpMstItemPrice c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND a.PartNo = c.PartNo
GROUP BY
a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, ServiceNo
    , SupplySlipDate
	, a.PartNo
	, c.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
    , QtySupply
    , DiscPct

END
ELSE
BEGIN

set @dbMD = (select DISTINCT dbmd from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)

--set @CompanyMD = (select DISTINCT CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
--select @CompanyMD
declare @query varchar(max)

if(isnull(cast(@dbMD as varchar(max)),'') = '')
BEGIN

SELECT 
DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, ServiceNo
    , SupplySlipDate
	, a.PartNo
	, c.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
	, SUM(QtyOrder) QtyOrder
    , QtySupply
    , DiscPct
FROM
(
SELECT
	DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
    , c.DocDate SupplySlipDate
	, a.PartNo
	, a.RetailPrice
	, f.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, Item.QtyOrder	
    , ISNULL(e.QtySupply,0) QtySupply
    , a.DiscPct
FROM
	SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
	LEFT JOIN SvTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.ProductType = b.ProductType
		AND a.ServiceNo = b.ServiceNo
	LEFT JOIN (SELECT CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice,
				SUM(ISNULL(DemandQty, 0) - ISNULL(SupplyQty, 0)) QtyOrder
				FROM SvTrnSrvItem 
				GROUP BY CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice) Item ON
		Item.CompanyCode = a.CompanyCode AND Item.BranchCode = a.BranchCode AND Item.ProductType =
			a.ProductType AND Item.ServiceNo = a.ServiceNo AND Item.PartNo = a.PartNo and Item.RetailPrice = a.RetailPrice
	LEFT JOIN SpTrnSORDHdr c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND b.JobOrderNo = c.UsageDocNo
	LEFT JOIN SpTrnSOSupply e WITH (NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
		AND a.BranchCode = e.BranchCode
		AND c.DocNo = e.DocNo
		AND a.PartNo = e.PartNo
    LEFT JOIN SpMstItemPrice f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
		AND a.BranchCode = f.BranchCode
		AND a.PartNo = f.PartNo
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND Item.QtyOrder > 0
    AND c.DocNo = @DocNo
	AND JobOrderNo = @JobOrderNo
    AND a.RetailPrice = Item.RetailPrice
)a
LEFT JOIN SpMstItemPrice c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND a.PartNo = c.PartNo
GROUP BY
a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, ServiceNo
    , SupplySlipDate
	, a.PartNo
	, c.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
    , QtySupply
    , DiscPct

END
ELSE
BEGIN

set @query = '
SELECT 
DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, ServiceNo
    , SupplySlipDate
	, a.PartNo
	, c.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
	, SUM(QtyOrder) QtyOrder
    , QtySupply
    , DiscPct
FROM
(
SELECT
	DISTINCT(a.CompanyCode) 
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
    , c.DocDate SupplySlipDate
	, a.PartNo
	, a.RetailPrice
	, f.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, Item.QtyOrder	
    , ISNULL(e.QtySupply,0) QtySupply
    , a.DiscPct
FROM
	SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
	LEFT JOIN SvTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.ProductType = b.ProductType
		AND a.ServiceNo = b.ServiceNo
	LEFT JOIN (SELECT CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice,
				SUM(ISNULL(DemandQty, 0) - ISNULL(SupplyQty, 0)) QtyOrder
				FROM SvTrnSrvItem 
				GROUP BY CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, RetailPrice) Item ON
		Item.CompanyCode = a.CompanyCode AND Item.BranchCode = a.BranchCode AND Item.ProductType =
			a.ProductType AND Item.ServiceNo = a.ServiceNo AND Item.PartNo = a.PartNo and Item.RetailPrice = a.RetailPrice
	LEFT JOIN SpTrnSORDHdr c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND b.JobOrderNo = c.UsageDocNo
	LEFT JOIN SpTrnSOSupply e WITH (NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
		AND a.BranchCode = e.BranchCode
		AND c.DocNo = e.DocNo
		AND a.PartNo = e.PartNo
    INNER JOIN gnMstCompanyMapping cm WITH(NOLOCK, NOWAIT) ON cm.CompanyCode = a.CompanyCode
    LEFT JOIN '+@dbMD+'..SpMstItemPrice f WITH(NOLOCK, NOWAIT) ON cm.CompanyMD = f.CompanyCode
		AND a.BranchCode = f.BranchCode
		AND a.PartNo = f.PartNo
WHERE
	a.CompanyCode = '+@CompanyCode+'
	AND a.BranchCode = '+@BranchCode+'
	AND a.ProductType = '+@ProductType+'
	AND Item.QtyOrder > 0
    AND c.DocNo = '+@DocNo+'
	AND JobOrderNo = '+@JobOrderNo+'
    AND a.RetailPrice = Item.RetailPrice
)a
LEFT JOIN SpMstItemPrice c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND a.PartNo = c.PartNo
GROUP BY
a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, ServiceNo
    , SupplySlipDate
	, a.PartNo
	, c.RetailPrice
	, c.CostPrice
    , TypeOfGoods
    , BillType
    , QtySupply
    , DiscPct
'
EXEC(@query)
print(@query)
				
		END	 
	END
end	