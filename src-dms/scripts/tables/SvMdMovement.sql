create table SvMdMovement (
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[DocNo] [varchar](15) NOT NULL,
	[DocDate] [datetime] NOT NULL,
	[PartNo] [varchar](20) NOT NULL,
	[PartSeq] [int] NOT NULL,
	[WarehouseCode] [varchar](15) NOT NULL,
	[QtyOrder] [numeric](18, 2) NOT NULL,
	[Qty] [numeric](18, 2) NOT NULL,
	[DiscPct] [numeric](6, 2) NOT NULL,
	[CostPrice] [numeric](18, 2) NOT NULL,
	[RetailPrice] [numeric](18, 2) NOT NULL,
	[TypeOfGoods] [varchar](5) NOT NULL,
	[CompanyMD] [varchar](15) NOT NULL,
	[BranchMD] [varchar](15) NOT NULL,
	[WarehouseMD] [varchar](15) NOT NULL,

	[RetailPriceInclTaxMD] [numeric](18, 2),
	[RetailPriceMD] [numeric](18, 2),
	[CostPriceMD] [numeric](18, 2),

	[QtyFlag] [char](1) NOT NULL,

	[ProductType] [varchar](15),
	[ProfitCenterCode] [varchar](15),

	[Status] [char](1) NOT NULL,
	[CreatedBy] [varchar](15) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdateBy] [varchar](15) NOT NULL,
	[LastUpdateDate] [datetime] NOT NULL,
	[IsPosting] [bit] NOT NULL,
	[PostingDate] [datetime] NOT NULL,
 primary key clustered 
(
	[CompanyCode] asc,
	[BranchCode] asc,
	[DocNo] asc,
	[PartNo] asc,
	[PartSeq] asc
)) 

