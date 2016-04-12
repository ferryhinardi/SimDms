delete SysMenuDms where MenuId in ('csdashboard','csmaster','csdssum','csmssetting')
insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('csdashboard', 'Dashboard', 'cs', 0, 1, '')
insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('csmaster', 'Master', 'cs', 1, 1, '')

insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('csdssum', 'Summary', 'csdashboard', 1, 2, 'dashboard/summary')
insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('csmssetting', 'Settings', 'csmaster', 1, 2, 'master/setting')

select * from SysMenuDms where MenuHeader in ('cs','csdashboard','csmaster','cstrans') order by MenuLevel, MenuHeader, MenuIndex
