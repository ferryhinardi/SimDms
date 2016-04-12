insert into BITJKT.dbo.SysModule select * from  BIT201310.dbo.SysModule where ModuleId like '%sp%';
insert into BITJKT.dbo.SysMenuDms  select * from  BIT201310.dbo.SysMenuDms where MenuId like '%sp%';
insert into BITJKT.dbo.SysRoleModule  select * from  BIT201310.dbo.SysRoleModule where ModuleId like '%sp%';
insert into BITJKT.dbo.sysRoleMenu  select * from  BIT201310.dbo.sysRoleMenu where MenuId like '%sp%';