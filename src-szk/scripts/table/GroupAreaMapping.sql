GO
/****** Object:  Table [dbo].[DealerGroupMapping]    Script Date: 1/20/2014 9:25:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DealerGroupMapping](
	[GroupNo] [varchar](50) NOT NULL,
	[DealerAbbr] [varchar](20) NOT NULL,
	[SeqNo] [int] NULL,
	[DealerCode] [varchar](20) NULL,
 CONSTRAINT [PK_DealerAreaMapping] PRIMARY KEY CLUSTERED 
(
	[GroupNo] ASC,
	[DealerAbbr] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DealerOutletMapping]    Script Date: 1/20/2014 9:25:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DealerOutletMapping](
	[ID] [varchar](36) NOT NULL,
	[CompanyCode] [varchar](15) NULL,
	[BranchCode] [varchar](15) NULL,
	[ParentCode] [varchar](15) NULL,
 CONSTRAINT [PK_gnMstDealerMapping] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GroupArea]    Script Date: 1/20/2014 9:25:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GroupArea](
	[GroupNo] [varchar](10) NOT NULL,
	[AreaDealer] [varchar](150) NULL,
 CONSTRAINT [PK_GroupArea] PRIMARY KEY CLUSTERED 
(
	[GroupNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
