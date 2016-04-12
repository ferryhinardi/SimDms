INSERT INTO [dbo].SysMenuDms (MenuId,MenuCaption,MenuHeader,MenuIndex,MenuLevel,MenuUrl)
VALUES ('csInqoutstandingdlvry','Outstanding Delivery','csinquiry',6, 2,'inquiry/outstandingDelivery')
GO

INSERT INTO [dbo].[sysRoleMenu] ([RoleId],[MenuId])
     VALUES ('ADMIN', 'csInqoutstandingdlvry')
GO