CREATE TABLE [dbo].[svSDMovement](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[DocNo] [varchar](15) NOT NULL,
	[DocDate] [datetime] NOT NULL,
	[PartNo] [varchar](20) NOT NULL,
	[PartSeq] [int] NOT NULL,
	[WarehouseCode] [varchar](15) NOT NULL,
	[QtyOrder] [numeric](18, 2) NOT NULL,
	[Qty] [numeric](18, 2) NOT NULL,
	[DiscPct] [numeric](5, 2) NOT NULL,
	[CostPrice] [numeric](18, 2) NOT NULL,
	[RetailPrice] [numeric](18, 2) NOT NULL,
	[TypeOfGoods] [varchar](5) NOT NULL,
	[CompanyMD] [varchar](15) NOT NULL,
	[BranchMD] [varchar](15) NOT NULL,
	[WarehouseMD] [varchar](15) NOT NULL,
	[RetailPriceInclTaxMD] [numeric](18, 2) NOT NULL,
	[RetailPriceMD] [numeric](18, 2) NOT NULL,
	[CostPriceMD] [numeric](18, 2) NOT NULL,
	[QtyFlag] [char](1) NOT NULL,
	[ProductType] [varchar](15) NOT NULL,
	[ProfitCenterCode] [varchar](15) NOT NULL,
	[Status] [char](1) NOT NULL,
	[ProcessStatus] [char](1) NOT NULL,
	[ProcessDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](15) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdateBy] [varchar](15) NULL,
	[LastUpdateDate] [datetime] NULL,
 CONSTRAINT [PK_svSDMovement1] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[DocNo] ASC,
	[PartNo] ASC,
	[PartSeq] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
) ON [PRIMARY]
