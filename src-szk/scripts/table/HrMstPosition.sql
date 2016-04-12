GO

/****** Object:  Table [dbo].[HrMstPosition]    Script Date: 1/20/2014 10:54:32 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[HrMstPosition](
	[CompanyCode] [nvarchar](128) NOT NULL,
	[DeptCode] [nvarchar](128) NOT NULL,
	[PosCode] [nvarchar](128) NOT NULL,
	[PosName] [nvarchar](max) NULL,
	[PosHeader] [nvarchar](max) NULL,
	[PosLevel] [int] NULL,
	[StartDate] [datetime] NULL,
	[FinishDate] [datetime] NULL,
	[CreatedBy] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](max) NULL,
	[UpdatedDate] [datetime] NULL,
	[TitleCode] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.HrMstPosition] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[DeptCode] ASC,
	[PosCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO


