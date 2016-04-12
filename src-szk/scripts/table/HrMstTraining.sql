GO

/****** Object:  Table [dbo].[HrMstTraining]    Script Date: 1/20/2014 8:01:40 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[HrMstTraining](
	[CompanyCode] [varchar](20) NOT NULL,
	[TrainingCode] [varchar](20) NOT NULL,
	[TrainingName] [varchar](150) NULL,
	[TrainingDescription] [varchar](500) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_HrMstTraining] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[TrainingCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[HrMstTraining] ADD  CONSTRAINT [DF__HrMstTrai__IsDel__1B7ED471]  DEFAULT ((0)) FOR [IsDeleted]
GO


