INSERT INTO [dbo].[sysRole] ([RoleId],[RoleName],[Themes],[IsActive],[IsAdmin],[IsChangeBranchCode],[IsChangeBranch])
     VALUES ('Sales GM', 'Sales GM', '', 1, 0, 0, 0)
GO

INSERT INTO [dbo].[sysRole] ([RoleId],[RoleName],[Themes],[IsActive],[IsAdmin],[IsChangeBranchCode],[IsChangeBranch])
     VALUES ('Sales Kacap', 'Sales Kacap', '', 1, 0, 0, 0)
GO

INSERT INTO [dbo].SysMenuDms (MenuId,MenuCaption,MenuHeader,MenuIndex,MenuLevel,MenuUrl)
VALUES ('omInquiryLiveStock','Live Stock','omInquiry',11, 2,'inquiry/LiveStock')
GO

INSERT INTO [dbo].[sysRoleMenu] ([RoleId],[MenuId])
     VALUES ('ADMIN', 'omInquiryLiveStock')
GO
INSERT INTO [dbo].[sysRoleMenu] ([RoleId],[MenuId])
     VALUES ('Sales GM', 'omInquiryLiveStock')
GO
INSERT INTO [dbo].[sysRoleMenu] ([RoleId],[MenuId])
     VALUES ('Sales Kacap', 'omInquiryLiveStock')
GO