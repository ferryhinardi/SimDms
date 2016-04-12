SELECT * INTO #t1 FROM
(SELECT * FROM svTrnSrvVOR) #t1
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[svTrnSrvVOR]') AND type in (N'U'))
DROP TABLE [dbo].[svTrnSrvVOR]
GO

CREATE TABLE [dbo].[svTrnSrvVOR](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[ServiceNo] [bigint] NOT NULL,
	[JobOrderNo] [varchar](15) NULL,
	[JobDelayCode] [varchar](20) NULL,
	[JobReasonDesc] [varchar](200) NULL,
	[ClosedDate] [datetime] NULL,
	[IsActive] [bit] NOT NULL,
	[IsSparepart] [bit] NOT NULL,
	[CreatedBy] [varchar](15) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[LastUpdateBy] [varchar](15) NULL,
	[LastUpdateDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC,
	[ServiceNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO svTrnSrvVOR
SELECT CompanyCode, BranchCode, ServiceNo, JobOrderNo, JobDelayCode, JobReasonDesc, ClosedDate, IsActive, 0, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate FROM #t1
GO

DROP TABLE #t1

