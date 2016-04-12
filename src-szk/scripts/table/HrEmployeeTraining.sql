GO

/****** Object:  Table [dbo].[HrEmployeeTraining]    Script Date: 1/20/2014 8:01:38 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[HrEmployeeTraining](
	[CompanyCode] [varchar](20) NOT NULL,
	[EmployeeID] [varchar](20) NOT NULL,
	[TrainingCode] [varchar](20) NOT NULL,
	[TrainingDate] [datetime] NOT NULL,
	[TrainingSeq] [int] NULL,
	[TrainingDuration] [int] NULL,
	[PreTest] [int] NULL,
	[PreTestAlt] [varchar](10) NULL,
	[PostTest] [int] NULL,
	[PostTestAlt] [varchar](10) NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_HrEmployeeTraining_1] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[EmployeeID] ASC,
	[TrainingCode] ASC,
	[TrainingDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[HrEmployeeTraining] ADD  CONSTRAINT [DF_HrEmployeeTraining_TrainingSeq]  DEFAULT ((0)) FOR [TrainingSeq]
GO

ALTER TABLE [dbo].[HrEmployeeTraining] ADD  CONSTRAINT [DF__HrEmploye__IsDel__17AE438D]  DEFAULT ((0)) FOR [IsDeleted]
GO


