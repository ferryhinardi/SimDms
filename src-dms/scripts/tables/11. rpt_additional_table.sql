

CREATE TABLE [dbo].[DealerInfo](
	[DealerCode] [varchar](20) NOT NULL,
	[DealerName] [varchar](250) NULL,
	[ScheduleTime] [varchar](5) NULL,
	[GoLiveDate] [datetime] NULL,
	[SeqNo] [int] NULL,
	[ProductType] [varchar](50) NULL,
	[ShortName] [varchar](50) NULL,
	[WebUrl] [varchar](250) NULL,
	[WebStatus] [varchar](50) NULL,
	[IsSfm] [bit] NULL,
 CONSTRAINT [PK_DealerInfo] PRIMARY KEY CLUSTERED 
(
	[DealerCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[DealerInfo] ADD  CONSTRAINT [DF_DealerInfo_IsSfm]  DEFAULT ((0)) FOR [IsSfm]
GO




go

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[OutletInfo](
	[CompanyCode] [varchar](20) NOT NULL,
	[BranchCode] [varchar](20) NOT NULL,
	[BranchName] [varchar](250) NULL,
	[ShortBranchName] [varchar](50) NULL,
	[IsActive] [bit] NULL,
 CONSTRAINT [PK_OutletInfo] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


