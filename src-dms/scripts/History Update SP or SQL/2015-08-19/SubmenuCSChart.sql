/*
README
*/

select * from SysMenuDms where MenuHeader like '%cs%' and MenuLevel = 1
select * from SysRoleMenu where MenuId like '%cs%' and RoleId like 'admin'

begin tran
/*CREATE MENUHEADER*/
insert into sysMenuDms values ('cschart', 'Chart', 'cs', 5, 1, NULL, NULL)

insert into sysRoleMenu values ('ADMIN', 'cschart')

/*UNDER MENUHEADER Chart*/
insert into sysMenuDms values ('cschMonitoring1',			'Monitoring by Date', 'cschart', 1, 2, 'chart/monitoring1', NULL)
insert into sysMenuDms values ('cschMonitoring2',			'Monitoring by Period', 'cschart', 2, 2, 'chart/monitoring2', NULL)
insert into sysMenuDms values ('cschMonitoring3',			'Data Summary', 'cschart', 3, 2, 'chart/monitoring3', NULL)
insert into sysMenuDms values ('cschMonitoring4',			'Data DO vs 3 Days Call', 'cschart', 4, 2, 'chart/monitoring4', NULL)

insert into sysRoleMenu values ('ADMIN', 'cschMonitoring1')
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring2')
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring3')
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring4')


select * from SysMenuDms where MenuHeader like 'cs%' and (MenuLevel = 1 or MenuLevel = 2)
select * from SysRoleMenu where MenuId like '%cs%' and RoleId like 'admin'

commit