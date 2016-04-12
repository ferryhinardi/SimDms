delete SysMenu where MenuId = 'inqexecits'
insert into SysMenu (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('inqexecits', 'ITS Data Received', 'inqits', '19', '2', 'inq/SummaryDataIts')


delete SysRoleMenu where MenuID = 'inqexecits' and RoleID in (select RoleID from SysRole where RoleName = 'admin')
insert into SysRoleMenu (RoleID, MenuID) select RoleId, 'inqexecits' from SysRole where RoleName = 'admin'

select * from SysMenu where MenuHeader in ('its','inqits') order by MenuLevel, MenuIndex
select * from SysRoleMenu where MenuID = 'inqexecsum' and RoleID in (select RoleID from SysRole where RoleName = 'admin')

