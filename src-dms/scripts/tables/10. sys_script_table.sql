GO
/****** Object:  Table [dbo].[SysControlDms]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysControlDms](
	[CompanyCode] [varchar](25) NULL,
	[MenuID] [varchar](25) NULL,
	[FieldID] [varchar](100) NULL,
	[RoleID] [varchar](36) NULL,
	[Visibility] [tinyint] NULL,
	[Type] [varchar](50) NULL,
	[Title] [varchar](100) NULL
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[sysGroupDms]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sysGroupDms](
	[GroupId] [varchar](20) NOT NULL,
	[GroupName] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[Themes] [varchar](20) NULL,
	[IsAdmin] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SysMenuDms]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysMenuDms](
	[MenuId] [varchar](36) NOT NULL,
	[MenuCaption] [varchar](250) NULL,
	[MenuHeader] [varchar](36) NULL,
	[MenuIndex] [int] NULL,
	[MenuLevel] [int] NULL,
	[MenuUrl] [varchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[MenuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[sysMessage]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sysMessage](
	[MessageCode] [varchar](20) NOT NULL,
	[MessageCaption] [varchar](250) NULL,
 CONSTRAINT [PK_SysMessage] PRIMARY KEY CLUSTERED 
(
	[MessageCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SysMessageBoards]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysMessageBoards](
	[MessageID] [int] NOT NULL,
	[MessageHeader] [varchar](255) NULL,
	[MessageText] [text] NULL,
	[MessageTo] [varchar](10) NULL,
	[MessageTarget] [varchar](max) NULL,
	[MessageParams] [varchar](max) NULL,
	[DateFrom] [datetime] NULL,
	[DateTo] [datetime] NULL,
	[CreatedBy] [varchar](20) NULL,
	[CreatedDate] [datetime] NULL,
	[LastUpdateBy] [varchar](20) NULL,
	[LastUpdateDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MessageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SysModule]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysModule](
	[ModuleId] [varchar](36) NOT NULL,
	[ModuleCaption] [varchar](250) NULL,
	[ModuleIndex] [int] NULL,
	[ModuleUrl] [varchar](250) NULL,
	[InternalLink] [bit] NULL,
	[IsPublish] [bit] NULL,
 CONSTRAINT [PK_SysModule] PRIMARY KEY CLUSTERED 
(
	[ModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[sysParameter]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sysParameter](
	[ParamId] [varchar](20) NOT NULL,
	[ParamValue] [varchar](max) NULL,
	[ParamDescription] [varchar](max) NULL,
 CONSTRAINT [PK_SysParameter] PRIMARY KEY CLUSTERED 
(
	[ParamId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SysReportDms]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysReportDms](
	[ReportID] [varchar](50) NOT NULL,
	[ReportPath] [varchar](500) NOT NULL,
	[ReportProc] [varchar](500) NOT NULL,
 CONSTRAINT [PK_SysReportDms] PRIMARY KEY CLUSTERED 
(
	[ReportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SysReportSettings]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysReportSettings](
	[ReportID] [varchar](50) NOT NULL,
	[Keyword] [varchar](50) NOT NULL,
	[IsVisible] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReportID] ASC,
	[Keyword] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 80) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[sysRole]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sysRole](
	[RoleId] [varchar](36) NOT NULL,
	[RoleName] [varchar](100) NULL,
	[Themes] [varchar](20) NULL,
	[IsActive] [bit] NOT NULL,
	[IsAdmin] [bit] NULL,
 CONSTRAINT [PK_sysRole] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[sysRoleMenu]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[sysRoleMenu](
	[RoleId] [varchar](50) NOT NULL,
	[MenuId] [varchar](50) NOT NULL,
 CONSTRAINT [PK_RoleMenu] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[MenuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SysRoleModule]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysRoleModule](
	[RoleID] [varchar](36) NOT NULL,
	[ModuleID] [varchar](50) NOT NULL,
 CONSTRAINT [PK_SysRoleModule] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC,
	[ModuleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SysRoleUser]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysRoleUser](
	[RoleID] [varchar](36) NOT NULL,
	[UserID] [varchar](36) NOT NULL,
 CONSTRAINT [PK_SysRoleUser] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC,
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SysSession]    Script Date: 12/2/2013 10:38:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SysSession](
	[SessionId] [varchar](50) NOT NULL,
	[SessionUser] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[IsLogout] [bit] NULL,
	[LogoutTime] [datetime] NULL,
 CONSTRAINT [PK_SysSession] PRIMARY KEY CLUSTERED 
(
	[SessionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
