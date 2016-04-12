--insert into SysMenuDms VALUES ('spx821','Laporan Sparepart Analisis Mingguan','spx008',21,3,'report/lnkx821', NULL)
--INSERT INTO sysRoleMenu VALUES ('ADMIN', 'spx821')

drop table spHstSparepartWeekly

CREATE TABLE [dbo].[spHstSparepartWeekly](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[PeriodYear] [numeric](4, 0) NOT NULL,
	[PeriodMonth] [numeric](2, 0) NOT NULL,
	[PeriodWeek] [numeric](1, 0) NOT NULL,
	[TypeOfGoods] [varchar](15) NOT NULL,
	[Netto_WS] [numeric](18,2) NOT NULL, 
	[HPP_WS] [numeric](18,2) NOT NULL, 
	[Netto_PS] [numeric](18,2) NOT NULL, 
	[HPP_PS] [numeric](18,2) NOT NULL, 
	[Netto_C] [numeric](18,2) NOT NULL, 
	[HPP_C] [numeric](18,2) NOT NULL, 
	[Netto_U] [numeric](18,2) NOT NULL, 
	[HPP_U] [numeric](18,2) NOT NULL, 
	[NilaiStock] [numeric](18, 2) NOT NULL,
	[CreatedBy] [varchar](15) NULL,
	[CreatedDate] [datetime] NULL,
	[LastUpdateBy] [varchar](15) NULL,
	[LastUpdateDate] [datetime] NULL,
 CONSTRAINT [PK__spHstSparepartWeekly] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[PeriodYear] ASC,
	[PeriodMonth] ASC,
	[PeriodWeek] ASC,
	[TypeOfGoods] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
) ON [PRIMARY]





