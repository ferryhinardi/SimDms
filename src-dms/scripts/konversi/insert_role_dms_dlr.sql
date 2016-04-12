-- ROLE ADMIN
delete SysRoleModule where RoleId = 'ADMIN'
insert into SysRoleModule 
select 'ADMIN', ModuleId from SysModule

delete SysRoleMenu where RoleId = 'ADMIN'
insert into SysRoleMenu
select 'ADMIN', MenuId from SysMenuDms 

-- ROLE CS
delete SysRoleModule where RoleId = 'CS'
insert into SysRoleModule 
select 'CS', ModuleID from SysModule where ModuleId in ('cs')

delete SysRoleMenu where RoleId = 'CS'
insert into SysRoleMenu
select 'CS', MenuId from SysMenuDms where MenuId in ('cs','csdashboard','csinquiry','cstrans')
 union
select 'CS', MenuId from SysMenuDms where MenuHeader in ('csdashboard','csinquiry','cstrans')

-- ROLE SFM
delete SysRoleModule where RoleId = 'SFM'
insert into SysRoleModule 
select 'SFM', ModuleID from SysModule where ModuleId in ('ab')

delete SysRoleMenu where RoleId = 'SFM'
insert into SysRoleMenu
select 'SFM', MenuId from SysMenuDms where MenuId in ('ab','abempl','abreport')
 union
select 'SFM', MenuId from SysMenuDms where MenuHeader in ('abempl','abreport')


-- ROLE SFM ADMIN
delete SysRoleModule where RoleId = 'SFM-ADMIN'
insert into SysRoleModule 
select 'SFM-ADMIN', ModuleID from SysModule where ModuleId in ('ab', 'gn')

delete SysRoleMenu where RoleId = 'SFM-ADMIN'
insert into SysRoleMenu
select 'SFM-ADMIN', MenuId from SysMenuDms where MenuId in ('ab','abmaster','abempl','abtrans','abreport','gnmember','gnuser')
 union
select 'SFM-ADMIN', MenuId from SysMenuDms where MenuHeader in ('abmaster','abempl','abtrans','abreport')
