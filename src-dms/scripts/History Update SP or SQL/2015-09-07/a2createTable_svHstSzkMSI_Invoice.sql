
CREATE TABLE [dbo].[svHstSzkMSI_Invoice](
	[CompanyCode] [varchar](20) NOT NULL,
	[BranchCode] [varchar](20) NOT NULL,
	[PeriodYear] [numeric](4, 0) NOT NULL,
	[PeriodMonth] [numeric](2, 0) NOT NULL,
	[SeqNo] [int] NOT NULL,
	[MsiGroup] [varchar](50) NULL,
	[MsiDesc] [varchar](500) NULL,
	[Unit] [varchar](50) NULL,
	[MsiData] [numeric](18, 2) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_svHstSzkMSI_Invoice] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[PeriodYear] ASC,
	[PeriodMonth] ASC,
	[SeqNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

