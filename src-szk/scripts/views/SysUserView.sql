
go
if object_id('SysUserView') is not null
	drop view SysUserView

go
create view SysUserView 
as
select
	a.*,
	RoleId = upper(b.RoleId),
	c.RoleName,
	IsApprovedDescription = (
		case
			when a.IsApproved = 1 then 'Yes'
			else 'No'
		end	
	)
from
	SysUser a
left join
	SysRoleUser b
on
	b.UserId=a.UserId
left join 
	SysRole c
on
	c.RoleId = b.RoleId

go
select * from SysUserView