go
if object_id('SysRoleModuleView') is not null
	drop view SysRoleModuleView

go
create view SysRoleModuleView
as
select
	a.RoleId,
	b.RoleName,
	a.ModuleID,
	c.ModuleCaption,
	c.ModuleUrl,
	c.ModuleIndex
from
	SysRoleModule a
inner join
	SysRole b
on
	a.RoleID = b.RoleId
inner join
	SysModule c
on
	c.ModuleId=a.ModuleID
	 
go
select * from SysRoleModuleView

--select * from SysModule
