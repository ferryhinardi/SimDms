INSERT INTO SysMenuDms (
	MenuId,
	MenuCaption,
	MenuHeader,
	MenuIndex,
	MenuLevel,
	MenuUrl,
	MenuIcon
)
VALUES (
	'spx010',
	'Report - Mutation',
	'splaporan',
	10, 
	2,
	NULL,
	'glyph-exec-dashboard'
)
GO

INSERT INTO SysRoleMenu(RoleId, MenuId) VALUES('ADMIN', 'spx010')
GO

INSERT INTO SysMenuDms (
	MenuId,
	MenuCaption,
	MenuHeader,
	MenuIndex,
	MenuLevel,
	MenuUrl
)
VALUES (
	'spx1001',
	'Mutation Stock',
	'spx010',
	1, 
	3,
	'report/lnkx1001'
)
GO

INSERT INTO SysRoleMenu(RoleId, MenuId) VALUES('ADMIN', 'spx1001')
GO