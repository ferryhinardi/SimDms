

CREATE TABLE [dbo].[CsBpkbRetrievalInformation](
	[CompanyCode] [nchar](10) NOT NULL,
	[CustomerCode] [nchar](10) NOT NULL,
	[RetrievalEstimationDate] [datetime] NOT NULL,
	[Notes] [varchar](250) NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](50) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_CsBpkbRetrievalInformation] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[CustomerCode] ASC,
	[RetrievalEstimationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


