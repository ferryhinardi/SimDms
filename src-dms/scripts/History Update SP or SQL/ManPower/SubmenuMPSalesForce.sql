/*
README
Read Editing Menu on Line 54
*/

select * from SysMenuDms where MenuHeader like 'abinqsfm%'
select * from SysRoleMenu where MenuId like '%abinqsfm%' and RoleId like 'admin'

begin tran
/*CREATE MENUHEADER*/
insert into sysMenuDms values ('abinqsfmInfo', 'Info', 'abinqsfm', 1, 3, NULL, NULL)
insert into sysMenuDms values ('abinqsfmTraining', 'Training', 'abinqsfm', 2, 3, NULL, NULL)
insert into sysMenuDms values ('abinqsfmTurnOver', 'Turn Over', 'abinqsfm', 3, 3, NULL, NULL)

insert into sysRoleMenu values ('ADMIN', 'abinqsfmInfo')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmTraining')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmTurnOver')

/*UNDER MENUHEADER INFO*/
insert into sysMenuDms values ('abinqsfmReviewSFM',			'Review SFM', 'abinqsfmInfo', 2, 4, 'inquiry/SfmReviewSFM', NULL)
insert into sysMenuDms values ('abinqsfmSalesTeam',			'Sales Team', 'abinqsfmInfo', 3, 4, 'inquiry/SfmSalesTeam', NULL)
insert into sysMenuDms values ('abinqsfmManPowerDashboard', 'ManPower Dashboard', 'abinqsfmInfo', 4, 4, 'inquiry/SfmMpDashboard', NULL)
insert into sysMenuDms values ('abinqsfmInvalidEmployee',	'Invalid Employee', 'abinqsfmInfo', 5, 4, 'inquiry/SfmInvalidEmployee', NULL)
insert into sysMenuDms values ('abinqsfmDemographicCondition', 'Demographic Condition', 'abinqsfmInfo', 6, 4, 'inquiry/SfmDemographicCondition', NULL)

insert into sysRoleMenu values ('ADMIN', 'abinqsfmReviewSFM')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmSalesTeam')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmManPowerDashboard')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmInvalidEmployee')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmDemographicCondition')

/*UNDER MENUHEADER TRAINING*/
insert into sysMenuDms values ('abinqsfmSalesmanTraining',		'Salesman Training', 'abinqsfmTraining', 1, 4, 'inquiry/SfmSalesmanTraining', NULL)
insert into sysMenuDms values ('abinqsfmSHBMTraining',			'SH BM Training', 'abinqsfmTraining', 2, 4, 'inquiry/SfmSHBMTraining', NULL)
insert into sysMenuDms values ('abinqsfmOutstandingTraining',	'Outstanding Training', 'abinqsfmTraining', 3, 4, 'inquiry/SfmOutstandingTraining', NULL)
insert into sysMenuDms values ('abinqsfmTrainingDashboard',		'Training Dashboard', 'abinqsfmTraining', 4, 4, 'inquiry/SfmTrainingDashboard', NULL)
insert into sysMenuDms values ('abinqsfmTrainingDetail',		'Training Detail', 'abinqsfmTraining', 5, 4, 'inquiry/SfmTrainingDetail', NULL)

insert into sysRoleMenu values ('ADMIN', 'abinqsfmSalesmanTraining')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmSHBMTraining')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmOutstandingTraining')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmTrainingDashboard')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmTrainingDetail')

/*UNDER MENUHEADER TURN OVER*/
insert into sysMenuDms values ('abinqsfmTurnOverMenu',	'Turn Over', 'abinqsfmTurnOver', 3, 4, 'inquiry/SfmTurnOverMenu', NULL)
insert into sysMenuDms values ('abinqsfmTurnOverRatio', 'Turn Over Ratio', 'abinqsfmTurnOver', 4, 4, 'inquiry/SfmTurnOverRatio', NULL)
insert into sysMenuDms values ('abinqsfmInOutData',		'In Out Data', 'abinqsfmTurnOver', 5, 4, 'inquiry/SfmInOutData', NULL)
insert into sysMenuDms values ('abinqsfmPromotion',		'Promotion', 'abinqsfmTurnOver', 6, 4, 'inquiry/SfmPromotion', NULL)
insert into sysMenuDms values ('abinqsfmDemotion',		'Demotion', 'abinqsfmTurnOver', 7, 4, 'inquiry/SfmDemotion', NULL)

insert into sysRoleMenu values ('ADMIN', 'abinqsfmTurnOverMenu')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmTurnOverRatio')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmInOutData')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmPromotion')
insert into sysRoleMenu values ('ADMIN', 'abinqsfmDemotion')


/*Edit existing menu : Personal Information & Data Mutation & Data Trend*/
update SysMenuDms set MenuHeader = 'abinqsfmInfo', MenuLevel = 4 where MenuId in ('abinqsfmpersinfo')
update SysMenuDms set MenuHeader = 'abinqsfmTurnOver', MenuLevel = 4, MenuIndex = 1 where MenuId in ('abinqsfmmutation')
update SysMenuDms set MenuHeader = 'abinqsfmTurnOver', MenuLevel = 4, MenuIndex = 2 where MenuId in ('abinqsfmtrend')


select * from SysMenuDms where MenuHeader like 'abinqsfm%'
select * from SysRoleMenu where MenuId like '%abinqsfm%' and RoleId like 'admin'

commit