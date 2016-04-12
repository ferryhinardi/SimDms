CREATE TABLE [omSDMovement](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[DocNo] [varchar](15) NOT NULL,
	[DocDate] [datetime] NOT NULL,
	[Seq] [int] NOT NULL,
	[SalesModelCode] [varchar](20) NOT NULL,
	[SalesModelYear] [numeric](4, 0) NOT NULL,
	[ChassisCode] [varchar](15) NOT NULL,
	[ChassisNo] [numeric](10, 0) NOT NULL,
	[EngineCode] [varchar](15) NOT NULL,
	[EngineNo] [numeric](10, 0) NOT NULL,
	[ColourCode] [varchar](15) NOT NULL,
	[WarehouseCode] [varchar](15) NOT NULL,
	[CustomerCode] [varchar](15) NOT NULL,
	[QtyFlag] [char](1) NOT NULL,
	[CompanyMD] [varchar](15) NOT NULL,
	[BranchMD] [varchar](15) NOT NULL,
	[WarehouseMD] [varchar](15) NOT NULL,
	[Status] [char](1) NOT NULL,
	[ProcessStatus] [char](1) NOT NULL,
	[ProcessDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](15) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdateBy] [varchar](15) NOT NULL,
	[LastUpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_omSDMovement] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[DocNo] ASC,
	[DocDate] ASC,
	[Seq] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


