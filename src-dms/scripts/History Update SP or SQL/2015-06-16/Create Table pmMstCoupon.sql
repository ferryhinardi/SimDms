

/****** Object:  Table [dbo].[pmMstCoupon]    Script Date: 6/16/2015 11:16:50 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[pmMstCoupon](
	[CompanyCode] [varchar](15) NOT NULL,
	[TipeKendaraan] [varchar](20) NOT NULL,
	[Variant] [varchar](50) NOT NULL,
	[SeqNo] [int] NOT NULL,
	[BeginPeriod] [datetime] NOT NULL,
	[EndPeriod] [datetime] NOT NULL,
	[isActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](15) NOT NULL,
	[LastUpdateBy] [varchar](15) NOT NULL,
	[LastUpdateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_pmMstCoupon1] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[TipeKendaraan] ASC,
	[Variant] ASC,
	[SeqNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

INSERT into pmMstCoupon
VALUES('6641401','KARIMUN WAGON R','GL MC AT','1','015-06-01 00:00:00.000','2015-06-30 00:00:00.000','1','2015-06-10 00:00:00.000','ga','ga','2015-06-10 00:00:00.000')
INSERT into pmMstCoupon
VALUES('6641401','KARIMUN WAGON R','GS MC AT','1','015-06-01 00:00:00.000','2015-06-30 00:00:00.000','1','2015-06-10 00:00:00.000','ga','ga','2015-06-10 00:00:00.000')
INSERT into pmMstCoupon
VALUES('6641401','KARIMUN WAGON R','GX MC AT','1','015-06-01 00:00:00.000','2015-06-30 00:00:00.000','1','2015-06-10 00:00:00.000','ga','ga','2015-06-10 00:00:00.000')

