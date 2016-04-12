if not exists (select * from SysMenuDms where menuid='srvUnitIntake')
begin
	insert into SysMenuDms (MenuId,MenuCaption,MenuHeader,MenuIndex,menulevel,MenuUrl) values('srvUnitIntake','Service Unit Intake','svinquiry',10,2,'inquiry/serviceunitintake')

end

if not exists (select * from sysRoleMenu where RoleId='ADMIN' and menuid='srvUnitIntake')
begin
	insert into sysRoleMenu (RoleId,MenuId) values('ADMIN','srvUnitIntake')

end