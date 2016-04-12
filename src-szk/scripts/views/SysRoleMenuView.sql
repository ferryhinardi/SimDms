go
if object_id('SysRoleMenuView') is not null
	drop view SysRoleMenuView

go
create view SysRoleMenuView
as
select
	a.RoleID,
	b.RoleName,
	a.MenuID,
	c.MenuCaption,
	c.MenuUrl,
	c.MenuLevel,
	c.MenuIndex
from
	SysRoleMenu a
inner join
	SysRole b
on
	a.RoleID=b.RoleId
inner join
	SysMenu c
on
	a.MenuID = c.MenuId

go
select * from SysRoleMenuView
