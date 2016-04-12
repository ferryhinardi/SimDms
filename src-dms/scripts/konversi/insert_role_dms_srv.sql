-- insert role admin
insert into SysRoleMenu
select 'b13adcea-e538-49fd-b9e0-34901ce26e2e', MenuId from SysMenu where MenuId not in 
(select MenuID from SysRoleMenu where RoleID = 'b13adcea-e538-49fd-b9e0-34901ce26e2e')

-- insert role sfm
-- select * from SysRole where RoleId = 'B13ADCEA-E538-49FD-B9E0-34901CE26E01'
insert into SysRoleMenu
select 'B13ADCEA-E538-49FD-B9E0-34901CE26E01', MenuId 
  from SysMenu
 where (MenuId in ('mp', 'mpsfm') or MenuHeader in ('mpsfm')) and MenuId not in
(select MenuID from SysRoleMenu where RoleID = 'B13ADCEA-E538-49FD-B9E0-34901CE26E01')

delete SysRoleMenu
 where RoleId = 'B13ADCEA-E538-49FD-B9E0-34901CE26E01'
   and MenuID not in 
   (select MenuId from SysMenu where (MenuId in ('mp', 'mpsfm') or MenuHeader in ('mpsfm')))

-- insert role service
-- select * from SysRole where RoleID = 'b13adcea-e538-49fd-b9e0-34901ce26e02'
insert into SysRoleMenu
select 'b13adcea-e538-49fd-b9e0-34901ce26e02', MenuId 
  from SysMenu
 where (MenuId in ('mp', 'mpsrv','sv','svinquiry') or MenuHeader in ('mpsrv','svinquiry')) and MenuId not in
(select MenuID from SysRoleMenu where RoleID = 'b13adcea-e538-49fd-b9e0-34901ce26e02')
