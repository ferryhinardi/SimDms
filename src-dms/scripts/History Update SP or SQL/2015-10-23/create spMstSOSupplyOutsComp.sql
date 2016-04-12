CREATE TABLE [dbo].[spMstSOSupplyOutsComp](
	[CompanyCode] [varchar](15) NOT NULL,
	[BranchCode] [varchar](15) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreateBy] [varchar](30) NULL,
	[CreateDate] [datetime] NULL,
	[LastUpdateBy] [varchar](30) NULL,
	[LastUpdateDate] [datetime] NULL,
 CONSTRAINT [PK_spMstSOSupplyOutsComp] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[BranchCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[spMstSOSupplyOutsComp] ADD  CONSTRAINT [DF_Table_2_Actived]  DEFAULT ((1)) FOR [IsActive]
GO