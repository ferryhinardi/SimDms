begin tran

if exists (select * from sysMenuDms where MenuId='sp9009')
begin
	select * from sysMenuDms where MenuId='sp9009'
end
else
begin
	insert into sysMenuDms values ('sp9009', 'AOS Item Sparepart List', 'sputility', 909, 2, 'utility/lnk9009', NULL)
end

if exists (select * from sysRoleMenu where MenuId='sp9009' and RoleId='ADMIN')
begin
	select * from sysRoleMenu where MenuId='sp9009' and RoleId='ADMIN'
end
else
begin
	insert into sysRoleMenu values ('ADMIN', 'sp9009')
end

commit