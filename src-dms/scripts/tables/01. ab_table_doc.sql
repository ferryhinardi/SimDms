

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[HrAbsenceFile](
	[FileID] [varchar](64) NOT NULL,
	[FileName] [varchar](120) NULL,
	[FileType] [varchar](15) NULL,
	[FileSize] [int] NULL,
	[FileContent] [varbinary](max) NULL,
	[UploadedBy] [varchar](25) NULL,
	[UploadedDate] [datetime] NULL,
 CONSTRAINT [PK_dbo.HrAbsenceFile] PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


