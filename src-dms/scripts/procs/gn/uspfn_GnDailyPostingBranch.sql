alter procedure uspfn_GnDailyPostingBranch
    @CompanyCode varchar(20),
    @BranchCode  varchar(20),
    @DocDate     date
as

set nocount on

begin transaction

declare @DbMD      varchar(50)
declare @SqlQry1   varchar(max)
declare @SqlQry2   varchar(max)
declare @SqlQry3   varchar(max)
declare @SqlQry4   varchar(max)

set @DbMD   = isnull((select top 1 DbMD from GnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode), '')

-- insert into SvSDMovement
set @SqlQry1 = N'
;with x as (
select a.CompanyCode
	, a.BranchCode
	, a.DocNo
	, a.DocDate
	, a.PartNo
	, a.PartSeq
	, a.WarehouseCode
	, a.QtyOrder
	, a.Qty
	, a.DiscPct
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.CompanyMD
	, a.BranchMD
	, a.WarehouseMD
	, a.RetailPriceInclTaxMD
	, a.RetailPriceMD
	, a.CostPriceMD
	, a.QtyFlag
	, a.ProductType
	, a.ProfitCenterCode
	, a.Status
	, a.CreatedBy
	, a.CreatedDate
	, a.LastUpdateBy
	, a.LastUpdateDate
	, IsPosting = 0
	, PostingDate = getdate()
  from SvSDMovement a
 where 1 = 1
   and CompanyCode = '''+ @CompanyCode + N'''
   and BranchCode = '''+ @BranchCode + N'''
   and convert(date, DocDate) = '''+ convert(varchar, @DocDate, 112) + N'''
)
insert into ' + @DbMD + N'..SvMDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice, TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, IsPosting, PostingDate)
select * from x
 where not exists (
   select top 1 1 from ' + @DbMD + N'..SvMDMovement
    where CompanyCode = x.CompanyCode
      and BranchCode = x.BranchCode
      and DocNo = x.DocNo
      and PartNo = x.PartNo
      and PartSeq = x.PartSeq
 )';

-- insert into spTrnSOrdHdr
set @SqlQry2 = N'
;with x as (
select a.CompanyMD as CompanyCode
	, a.BranchMD as BranchCode
	, DocNo = (
select top 1 DocumentPrefix + ''/'' 
     + right(convert(varchar, DocumentYear), 2) + ''/''
  from ' + @DbMD + '..gnMstDocument
 where CompanyCode = a.CompanyMD
   and BranchCode = a.BranchMD
   and DocumentType = ''SOC'')
	, DocSeq = (
select top 1 convert(varchar, DocumentSequence)
  from ' + @DbMD + '..gnMstDocument
 where CompanyCode = a.CompanyMD
   and BranchCode = a.BranchMD
   and DocumentType = ''SOC'')
	, row_number() over(order by a.DocNo) as SeqNo
	, DocDate = (select top 1 DocDate from ' + @DbMD + '..SvMdMovement where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and DocNo = a.DocNo)
	, UsageDocNo = a.DocNo
	, CustomerCode = a.CompanyCode
	, CustomerCodeBill = a.CompanyCode
	, CustomerCodeShip = a.CompanyCode
	, OrderNo = a.BranchCode
	, TypeOfGoods = a.TypeOfGoods
	, ProfitCenterCode
  from ' + @DbMD + '..SvMdMovement a
 where isnull(a.IsPosting, 0) = 0
   and CompanyCode = '''+ @CompanyCode + N'''
   and BranchCode = '''+ @BranchCode + N'''
 group by a.CompanyCode, a.BranchCode, a.CompanyMD, a.BranchMD, a.DocNo, a.TypeOfGoods, a.ProfitCenterCode
)
, y as (
select x.CompanyCode
	, x.BranchCode
	, (x.DocNo + right(''000000'' + convert(varchar, x.DocSeq + x.SeqNo), 6)) as DocNo
	, x.DocDate
	, x.UsageDocNo
	, UsageDocDate = x.DocDate
	, x.CustomerCode
	, x.CustomerCodeBill
	, x.CustomerCodeShip
	, 0 as isBO
	, 0 as isSubstitution
	, 1 as isIncludePPN
	, ''00'' as TransType
	, ''0'' as SalesType
	, 0 as IsPORDD
	, x.OrderNo
	, OrderDate = x.DocDate
	, TOPCode = isnull((select top 1 a.TopCode 
                  from ' + @DbMD + '..gnMstCustomerProfitCenter a
                 where CompanyCode = x.CompanyCode
                   and BranchCode = x.BranchCode
                   and CustomerCode = x.CustomerCode
                   and ProfitCenterCode = x.ProfitCenterCode), ''--'')
	, TOPDays = isnull((select b.ParaValue
                  from ' + @DbMD + '..gnMstCustomerProfitCenter a
                  join ' + @DbMD + '..gnMstLookupDtl b
                    on b.CompanyCode = a.CompanyCode
                   and b.CodeID = ''TOPC''
                   and b.LookUpValue = a.TopCode
                 where a.CompanyCode = x.CompanyCode
                   and a.BranchCode = x.BranchCode
                   and a.CustomerCode = x.CustomerCode
                   and a.ProfitCenterCode = x.ProfitCenterCode), ''0'')
	, ''CR'' PaymentCode
	, null as PaymentRefNo
	, 0 as TotSalesQty
	, 0 as TotSalesAmt
	, 0 as TotDiscAmt
	, 0 as TotDPPAmt
	, 0 as TotPPNAmt
	, 0 as TotFinalSalesAmt
	, 1 as isPKP
	, null as ExPickingSlipNo
	, null as ExPickingSlipDate
	, 5 as Status
	, 1 as PrintSeq
	, x.TypeOfGoods
	, ''DAILYPOSTING'' as CreatedBy
	, getdate() as CreatedDate
	, (select top 1 CreatedBy from ' + @DbMD + '..SvMdMovement where CompanyMD = x.CompanyCode and BranchMD = x.BranchCode and DocNo = x.UsageDocNo) as LastUpdateBy
	, (select top 1 CreatedDate from ' + @DbMD + '..SvMdMovement where CompanyMD = x.CompanyCode and BranchMD = x.BranchCode and DocNo = x.UsageDocNo) as LastUpdateDate
	, 0 isLocked
	, 0 isDropSign
  from x
)
insert into ' + @DbMD + '..spTrnSORDHdr (CompanyCode, BranchCode, DocNo, DocDate, UsageDocNo, UsageDocDate, CustomerCode, CustomerCodeBill, CustomerCodeShip, isBO, isSubstitution, isIncludePPN, TransType, SalesType, IsPORDD, OrderNo, OrderDate, TOPCode, TOPDays, PaymentCode, PaymentRefNo, TotSalesQty, TotSalesAmt, TotDiscAmt, TotDPPAmt, TotPPNAmt, TotFinalSalesAmt, isPKP, ExPickingSlipNo, ExPickingSlipDate, Status, PrintSeq, TypeOfGoods, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, isLocked, isDropSign)
select * from y';

-- insert into spTrnSOrdDtl (1)
set @SqlQry3 = N'
;with x as (
select distinct a.CompanyMD as CompanyCode
	, a.BranchMD as BranchCode
	, DocNo = (select top 1 DocNo from ' + @DbMD + '..spTrnSORDHdr where CompanyCode = a.CompanyMD and BranchCode = a.BranchMD and UsageDocNo = a.DocNo)
	, a.PartNo, a.WarehouseCode, PartNoOriginal = a.PartNo
	, UsageDocNo = a.DocNo, CustomerCode = a.CompanyCode
	, a.ProfitCenterCode
	, a.ProductType
  from ' + @DbMD + '..SvMdMovement a
 where isnull(a.IsPosting, 0) = 0
   and CompanyCode = '''+ @CompanyCode + N'''
   and BranchCode = '''+ @BranchCode + N'''
)
, y as (
select x.*, b.LocationCode, ReferenceNo = x.UsageDocNo
	, ReferenceDate = (select top 1 DocDate from ' + @DbMD + '..SvMdMovement where CompanyMD = x.CompanyCode and BranchMD = x.BranchCode and DocNo = x.UsageDocNo and PartNo = x.PartNo and WarehouseCode = x.WarehouseCode)
	, QtyOrder = (select sum(QtyOrder) from ' + @DbMD + '..SvMdMovement where CompanyMD = x.CompanyCode and BranchMD = x.BranchCode and DocNo = x.UsageDocNo and PartNo = x.PartNo and WarehouseCode = x.WarehouseCode)
	, QtySupply = (select sum(Qty) from ' + @DbMD + '..SvMdMovement where CompanyMD = x.CompanyCode and BranchMD = x.BranchCode and DocNo = x.UsageDocNo and PartNo = x.PartNo and WarehouseCode = x.WarehouseCode)
	, QtyBill = (select sum(Qty) from ' + @DbMD + '..SvMdMovement where CompanyMD = x.CompanyCode and BranchMD = x.BranchCode and DocNo = x.UsageDocNo and PartNo = x.PartNo and WarehouseCode = x.WarehouseCode)
	, RetailPriceInclTax = (select top 1 RetailPriceInclTaxMD from ' + @DbMD + '..SvMdMovement where CompanyMD = x.CompanyCode and BranchMD = x.BranchCode and DocNo = x.UsageDocNo and PartNo = x.PartNo and WarehouseCode = x.WarehouseCode)
	, RetailPrice = (select top 1 RetailPriceMD from ' + @DbMD + '..SvMdMovement where CompanyMD = x.CompanyCode and BranchMD = x.BranchCode and DocNo = x.UsageDocNo and PartNo = x.PartNo and WarehouseCode = x.WarehouseCode)
	, CostPrice = (select top 1 CostPrice from ' + @DbMD + '..SvMdMovement where CompanyMD = x.CompanyCode and BranchMD = x.BranchCode and DocNo = x.UsageDocNo and PartNo = x.PartNo and WarehouseCode = x.WarehouseCode)
	, isnull(c.DiscPct, 0) as DiscPct
	, CreatedBy = (select top 1 CreatedBy from ' + @DbMD + '..SvMdMovement where CompanyMD = x.CompanyCode and BranchMD = x.BranchCode and DocNo = x.UsageDocNo and PartNo = x.PartNo and WarehouseCode = x.WarehouseCode)
	, CreatedDate = (select top 1 CreatedDate from ' + @DbMD + '..SvMdMovement where CompanyMD = x.CompanyCode and BranchMD = x.BranchCode and DocNo = x.UsageDocNo and PartNo = x.PartNo and WarehouseCode = x.WarehouseCode)
	, c.TaxCode
  from x
  left join ' + @DbMD + '..spMstItemLoc b
    on b.CompanyCode = x.CompanyCode 
   and b.BranchCode = x.BranchCode
   and b.PartNo = x.PartNo
   and b.WarehouseCode = x.WarehouseCode
  left join ' + @DbMD + '..gnMstCustomerProfitCenter c
    on c.CompanyCode = x.CompanyCode 
   and c.BranchCode = x.BranchCode
   and c.CustomerCode = x.CustomerCode
   and c.ProfitCenterCode = x.ProfitCenterCode
)
, z as (
select y.*
	, y.QtyBill * y.RetailPrice as SalesAmt
	, y.DiscPct * (y.QtyBill * y.RetailPrice) * 0.01 as DiscAmt
	, (1 - (y.DiscPct * 0.01)) * y.QtyBill * y.RetailPrice as NetSalesAmt
	, (1 - (y.DiscPct * 0.01)) * y.QtyBill * y.RetailPrice * (isnull(c.TaxPct, 0)) * 0.01 as PpnAmt
	, b.MovingCode, b.ABCClass, b.PartCategory
  from y
  left join ' + @DbMD + '..spMstItems b
    on b.CompanyCode = y.CompanyCode
   and b.BranchCode = y.BranchCode
   and b.PartNo = y.PartNo
  left join ' + @DbMD + '..gnMstTax c
    on c.CompanyCode = y.CompanyCode 
   and c.TaxCode = y.TaxCode
)
select * into ##TSOrdDtl from z
';

-- insert into spTrnSOrdDtl (2)
set @SqlQry4 = N'
insert into ' + @DbMD + '..spTrnSORDDtl (CompanyCode, BranchCode, DocNo, PartNo, WarehouseCode, PartNoOriginal, ReferenceNo, ReferenceDate, LocationCode, QtyOrder, QtySupply, QtyBO, QtyBOSupply, QtyBOCancel, QtyBill, RetailPriceInclTax, RetailPrice, CostPrice, DiscPct, SalesAmt, DiscAmt, NetSalesAmt, PPNAmt, TotSalesAmt, MovingCode, ABCClass, ProductType, PartCategory, Status, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate, StockAllocatedBy, StockAllocatedDate, FirstDemandQty)
select CompanyCode
	, BranchCode
	, DocNo
	, PartNo
	, WarehouseCode
	, PartNoOriginal
	, ReferenceNo
	, ReferenceDate
	, LocationCode
	, QtyOrder
	, QtySupply
	, 0 QtyBO
	, 0 QtyBOSupply
	, 0 QtyBOCancel
	, QtyBill
	, RetailPriceInclTax
	, RetailPrice
	, CostPrice
	, DiscPct
	, SalesAmt
	, DiscAmt
	, NetSalesAmt
	, PPNAmt
	, (NetSalesAmt - PPNAmt)TotSalesAmt
	, MovingCode
	, ABCClass
	, ProductType
	, PartCategory
	, 5 as Status
	, ''DAILYPOSTING'' as CreatedBy
	, getdate() CreatedDate
	, CreatedBy as LastUpdateBy
	, CreatedDate as LastUpdateDate
	, CreatedBy as StockAllocatedBy
	, CreatedDate as StockAllocatedDate
	, QtyOrder as FirstDemandQty
  from ##TSOrdDtl a

;with x as (select top 1 * from ##TSOrdDtl order by DocNo desc)
,y as (
select a.CompanyCode, a.BranchCode, a.DocumentType, a.DocumentPrefix
     , a.DocumentYear, a.DocumentSequence, x.DocNo
	 , convert(int, right(x.DocNo, 6)) DocSeqNew
  from ' + @DbMD + '..gnMstDocument a
  join x
    on a.CompanyCode = x.CompanyCode
   and a.BranchCode = x.BranchCode
   and a.DocumentType = ''SOC''
)
update y set DocumentSequence = DocSeqNew';

--print (@SqlQry1)
--print (@SqlQry2)

exec (@SqlQry1)
exec (@SqlQry2)
exec (@SqlQry3)
exec (@SqlQry4)

rollback transaction

go

exec uspfn_GnDailyPostingBranch '6006400001', '6006400101', '20140708'

