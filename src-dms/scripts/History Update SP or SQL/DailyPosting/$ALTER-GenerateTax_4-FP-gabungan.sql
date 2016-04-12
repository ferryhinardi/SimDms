select * into #Tax from (select * from [gnGenerateTax]) #Tax
go 

drop table [gnGenerateTax]
go

CREATE TABLE [dbo].[gnGenerateTax](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[PeriodTaxYear] [int] NOT NULL,
	[PeriodTaxMonth] [int] NOT NULL,
	[ProfitCenterCode] [varchar](15) NOT NULL,
	[FPJGovNo] [varchar](20) NOT NULL,
	[FPJGovDate] [datetime] NULL,
	[DocNo] [varchar](15) NOT NULL,
	[DocDate] [datetime] NULL,
	[RefNo] [varchar](15) NULL,
	[RefDate] [datetime] NULL,
	[CreatedBy] [varchar](36) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_gnGenerateTax] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[PeriodTaxYear] ASC,
	[PeriodTaxMonth] ASC,
	[ProfitCenterCode] ASC,
	[FPJGovNo] ASC,
	[DocNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]
go

insert into [gnGenerateTax] 
	select * from #Tax
go
