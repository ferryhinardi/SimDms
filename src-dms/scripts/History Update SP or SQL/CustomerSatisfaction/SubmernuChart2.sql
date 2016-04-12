/*
README
*/

select * from SysMenuDms where MenuHeader like '%cs%' and MenuLevel = 1
select * from SysRoleMenu where MenuId like '%cs%' and RoleId like 'admin'

begin tran
/*UNDER MENUHEADER Chart*/
insert into sysMenuDms values ('cschMonitoring5',			'Report 3 Days Call', 'cschart',		 5, 2, 'chart/monitoring5', NULL)
insert into sysMenuDms values ('cschMonitoring6',			'Report Customer Birthday', 'cschart',	 6, 2, 'chart/monitoring6', NULL)
insert into sysMenuDms values ('cschMonitoring7',			'Report BPKB Reminder', 'cschart',		 7, 2, 'chart/monitoring7', NULL)
insert into sysMenuDms values ('cschMonitoring8',			'Report STNK Extension', 'cschart',		 8, 2, 'chart/monitoring8', NULL)

insert into sysRoleMenu values ('ADMIN', 'cschMonitoring5')
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring6')
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring7')
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring8')


select * from SysMenuDms where MenuHeader like 'cs%' and (MenuLevel = 1 or MenuLevel = 2)
select * from SysRoleMenu where MenuId like '%cs%' and RoleId like 'admin'

commit