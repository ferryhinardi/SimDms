INSERT INTO [dbo].SysMenuDms (MenuId,MenuCaption,MenuHeader,MenuIndex,MenuLevel,MenuUrl)
VALUES ('itsinqMonitoring','Executive Summary - Current vs Previous Month','itsinq',9, 2,'inquiry/monitoring')
GO

INSERT INTO [dbo].[sysRoleMenu] ([RoleId],[MenuId])
     VALUES ('ADMIN', 'itsinqMonitoring')
GO