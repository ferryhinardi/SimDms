delete SysMenuDms where MenuHeader in ('abinquiry','abinqgen','abinqsfm','abdashboard')

insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abinqgen', 'General', 'abinquiry', 1, 2, '')
insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abinqempl', 'Employee List', 'abinqgen', 1, 3, 'inquiry/perslist')
insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abinqpersmuta', 'Employee Mutation', 'abinqgen', 2, 3, 'inquiry/persmuta')
insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abinqpersachieve', 'Employee Achievement', 'abinqgen', 3, 3, 'inquiry/persachieve')
insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abinqpersinvalid', 'Invalid Employee', 'abinqgen', 4, 3, 'inquiry/persinvalid')

insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abinqsfm', 'Sales Force', 'abinquiry', 2, 2, '')
insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abinqsfmpersinfo', 'Personal Information', 'abinqsfm', 1, 3, 'inquiry/sfmpersinfo')
insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abinqsfmmutation', 'Data Mutation', 'abinqsfm', 2, 3, 'inquiry/sfmmutation')
insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abinqsfmtrend', 'Data Trend', 'abinqsfm', 3, 3, 'inquiry/sfmtrend')
--insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abinqsfmturnover', 'Data Turn Over', 'abinqsfm', 4, 3, 'inquiry/sfmturnover')
--insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abinqsfmsummary', 'Data Summary', 'abinqsfm', 5, 3, 'inquiry/sfmsummary')

insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abdashboard', 'Dashboard', 'ab', 5, 1, '')
insert into SysMenuDms (MenuId, MenuCaption, MenuHeader, MenuIndex, MenuLevel, MenuUrl) values ('abdasempldept', 'Employee By Dept', 'abdashboard', 1, 2, 'dashboard/emplbydept')




