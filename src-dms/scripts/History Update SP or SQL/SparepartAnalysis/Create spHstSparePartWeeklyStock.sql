--drop table spHstSparePartWeeklyStock

CREATE TABLE [dbo].[spHstSparePartWeeklyStock](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[PeriodYear] [numeric](4, 0) NOT NULL,
	[PeriodMonth] [numeric](2, 0) NOT NULL,
	[PeriodWeek] [numeric](1, 0) NOT NULL,
	[TypeOfGoods] [varchar](15) NOT NULL,
	[NilaiStock] numeric(18,0) NOT NULL,
	[CreatedBy] [varchar](15) NULL,
	[CreatedDate] [datetime] NULL,
	[LastUpdateBy] [varchar](15) NULL,
	[LastUpdateDate] [datetime] NULL,
 CONSTRAINT [PK__spHstSparePartWeeklyStock] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[PeriodYear] ASC,
	[PeriodMonth] ASC,
	[PeriodWeek] ASC,
	[TypeOfGoods] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
) ON [PRIMARY]

