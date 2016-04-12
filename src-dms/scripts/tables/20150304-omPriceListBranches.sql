CREATE TABLE [dbo].[omPriceListBranches](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[SupplierCode] [varchar](15) NOT NULL,
	[GroupPrice] [varchar](15) NOT NULL,
	[SalesModelCode] [varchar](20) NOT NULL,
	[SalesModelYear] [numeric](4, 0) NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[RetailPriceIncludePPN] [numeric](18, 0) NULL,
	[DiscPriceIncludePPN] [numeric](18, 0) NULL,
	[NetSalesIncludePPN] [numeric](18, 0) NULL,
	[RetailPriceExcludePPN] [numeric](18, 0) NULL,
	[DiscPriceExcludePPN] [numeric](18, 0) NULL,
	[NetSalesExcludePPN] [numeric](18, 0) NULL,
	[PPNBeforeDisc] [numeric](18, 0) NULL,
	[PPNAfterDisc] [numeric](18, 0) NULL,
	[PPNBMPaid] [numeric](18, 0) NULL,
	[isStatus] [bit] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[LastUpdateBy] [varchar](20) NULL,
	[LastUpdateDate] [datetime] NULL,
 CONSTRAINT [PK_omPriceListBranches] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[SupplierCode] ASC,
	[GroupPrice] ASC,
	[SalesModelCode] ASC,
	[SalesModelYear] ASC,
	[EffectiveDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[omPriceListBranchesLog](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[SupplierCode] [varchar](15) NOT NULL,
	[GroupPrice] [varchar](15) NOT NULL,
	[SalesModelCode] [varchar](20) NOT NULL,
	[SalesModelYear] [numeric](4, 0) NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[RetailPriceIncludePPN] [numeric](18, 0) NULL,
	[DiscPriceIncludePPN] [numeric](18, 0) NULL,
	[NetSalesIncludePPN] [numeric](18, 0) NULL,
	[RetailPriceExcludePPN] [numeric](18, 0) NULL,
	[DiscPriceExcludePPN] [numeric](18, 0) NULL,
	[NetSalesExcludePPN] [numeric](18, 0) NULL,
	[PPNBeforeDisc] [numeric](18, 0) NULL,
	[PPNAfterDisc] [numeric](18, 0) NULL,
	[PPNBMPaid] [numeric](18, 0) NULL,
	[isStatus] [bit] NULL,
	[LastUpdateBy] [varchar](20) NULL,
	[LastUpdateDate] [datetime] NULL
) ON [PRIMARY]

GO
