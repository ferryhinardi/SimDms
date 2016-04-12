GO

/****** Object:  Table [dbo].[HrDepartmentTraining]    Script Date: 1/20/2014 8:01:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[HrDepartmentTraining](
	[CompanyCode] [varchar](20) NOT NULL,
	[Department] [varchar](20) NOT NULL,
	[Position] [varchar](20) NOT NULL,
	[Grade] [varchar](20) NOT NULL,
	[TrainingCode] [varchar](20) NOT NULL,
	[IsRequired] [bit] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](20) NULL,
	[UpdatedDate] [datetime] NULL,
	[IsDeleted] [bit] NULL,
 CONSTRAINT [PK_HrPositionTraining] PRIMARY KEY CLUSTERED 
(
	[CompanyCode] ASC,
	[Department] ASC,
	[Position] ASC,
	[Grade] ASC,
	[TrainingCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[HrDepartmentTraining] ADD  CONSTRAINT [DF__HrDepartm__IsDel__45750E3D]  DEFAULT ((0)) FOR [IsDeleted]
GO


