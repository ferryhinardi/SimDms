CREATE TABLE [dbo].[sysRole1](
	[RoleId] [varchar](60) NOT NULL,
	[RoleName] [varchar](100) NULL,
	[Themes] [varchar](20) NULL,
	[IsActive] [bit] NOT NULL,
	[IsAdmin] [bit] NULL,
	[IsChangeBranchCode] [bit] NULL,
	[IsChangeBranch] [bit] NULL,
 CONSTRAINT [PK_sysRole1] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 70) ON [PRIMARY]
) ON [PRIMARY]

insert into sysRole1
select RoleId, RoleName, Themes, IsActive, IsAdmin, IsChangeBranchCode, 0 from SysRole

drop table SysRole

CREATE TABLE [dbo].[sysRole](
	[RoleId] [varchar](60) NOT NULL,
	[RoleName] [varchar](100) NULL,
	[Themes] [varchar](20) NULL,
	[IsActive] [bit] NOT NULL,
	[IsAdmin] [bit] NULL,
	[IsChangeBranchCode] [bit] NULL,
	[IsChangeBranch] [bit] NULL,
 CONSTRAINT [PK_sysRole] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 70) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[sysRole] ADD  DEFAULT ((0)) FOR [IsChangeBranchCode]

ALTER TABLE [dbo].[sysRole] ADD  CONSTRAINT [DF__sysRole__IsChang__6141E84D]  DEFAULT ((0)) FOR [IsChangeBranch]

insert into SysRole
select * from sysRole1

drop table sysRole1

select * from SysRole