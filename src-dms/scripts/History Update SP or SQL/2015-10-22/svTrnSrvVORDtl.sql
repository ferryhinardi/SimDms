IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[svTrnSrvVORDtl]') AND type in (N'U'))
DROP TABLE [dbo].[svTrnSrvVORDtl]
GO

CREATE TABLE [dbo].[svTrnSrvVORDtl](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[ServiceNo] [bigint] NOT NULL,
	[POSNo] [varchar](15) NOT NULL,
	[PartNo] [varchar](20) NOT NULL,
	[PartQty] [numeric](12, 2) NULL,
	[CreatedBy] [varchar](15) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdateBy] [varchar](15) NULL,
	[LastUpdateDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[ServiceNo] ASC,
	[PartNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
