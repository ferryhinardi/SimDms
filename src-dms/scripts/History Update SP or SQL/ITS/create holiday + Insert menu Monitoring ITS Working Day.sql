INSERT INTO [dbo].SysMenuDms (MenuId,MenuCaption,MenuHeader,MenuIndex,MenuLevel,MenuUrl)
VALUES ('itsinqMonitoring2','Executive Summary - Current vs Previous Month (2)','itsinq',10, 2,'inquiry/MonitoringByWorkingDay')
GO

INSERT INTO [dbo].[sysRoleMenu] ([RoleId],[MenuId])
     VALUES ('ADMIN', 'itsinqMonitoring2')
GO

CREATE TABLE [dbo].[SysHoliday](
	[Holiday] [datetime] NOT NULL,
 CONSTRAINT [PK_SysHoliday] PRIMARY KEY CLUSTERED 
(
	[Holiday] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

/* 
INSERT INTO [SysHoliday]
SELECT [Holiday] FROM [tbsdmsdb01].[SimDms].[dbo].[SysHoliday]
GO
*/