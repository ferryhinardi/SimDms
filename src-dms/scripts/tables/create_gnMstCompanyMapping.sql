CREATE TABLE [dbo].[gnMstCompanyMapping](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[BranchName] [varchar](100) NOT NULL,
	[DbName] [varchar](50) NOT NULL,
	[MappingType] [bit] NOT NULL,
	[CompanyMD] [varchar](15) NOT NULL,
	[BranchMD] [varchar](15) NULL,
	[UnitBranchMD] [varchar](15) NULL,
	[BranchNameMD] [varchar](100) NOT NULL,
	[DbMD] [varchar](50) NOT NULL,
	[WarehouseMD] [varchar](15) NOT NULL,
	[isStatus] [bit] NOT NULL,
 CONSTRAINT [PK_gnMstCompanyMapping] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 70) ON [PRIMARY]
) ON [PRIMARY]