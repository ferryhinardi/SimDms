CREATE TABLE [dbo].[SysRoleMenuAccess](
	RoleId varchar(60) NOT NULL,
	MenuId varchar(60) NOT NULL,
	Navigation	bit NOT NULL default(0),
	AllowCreate	bit NOT NULL default(0),
	AllowEdit	bit NOT NULL default(0),
	AllowDelete	bit NOT NULL default(0),
	AllowPrint bit NOT NULL default(0),
 CONSTRAINT [PK_RoleMenuAccess] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[MenuId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
) ON [PRIMARY]

GO