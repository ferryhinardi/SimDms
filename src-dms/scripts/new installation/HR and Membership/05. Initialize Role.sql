declare @CompanyCode varchar(17);
set @CompanyCode = (
	select top 1 
		   a.CompanyCode
	  from gnMstOrganizationHdr a
);


insert into SysUser 
values ('sfm', '202CB962AC59075B964B07152D234B70', 'SALES FORCE MANAGEMENT', '', @CompanyCode, '', '', 1, 0)
insert into SysUser 
values ('sfm-admin', '202CB962AC59075B964B07152D234B70', 'SALES FORCE MANAGEMENT', '', @CompanyCode, '', '', 1, 0)


insert into SysRoleUser
select 'ga', 'ADMIN'

insert into SysRoleUser
select  'sfm', 'SFM'

insert into SysRoleUser
select 'sfm-admin', 'SFM-ADMIN'





insert into SysRoleModule
select 'ADMIN', 'ab'

insert into SysRoleModule
select 'ADMIN', 'cs'

insert into SysRoleModule
select 'ADMIN', 'gn'

insert into SysRoleModule
select 'ADMIN', 'sv'

insert into SysRoleModule
select 'SFM', 'ab'

insert into SysRoleModule
select 'SFM-ADMIN', 'ab'

insert into SysRoleModule
select 'SFM-ADMIN', 'gn'







insert into sysRoleMenu
select 'ADMIN', a.MenuId
  from SysMenuDms a


 select *
   from SysRoleMenu
  where RoleId='SFM'

insert into SysRoleMenu values ('SFM', 'abempl')
insert into SysRoleMenu values ('SFM', 'abinqempl')
insert into SysRoleMenu values ('SFM', 'abinqgen')
insert into SysRoleMenu values ('SFM', 'abinqpersachieve')
insert into SysRoleMenu values ('SFM', 'abinqpersinvalid')
insert into SysRoleMenu values ('SFM', 'abinqpersmuta')
insert into SysRoleMenu values ('SFM', 'abinqsfm')
insert into SysRoleMenu values ('SFM', 'abinqsfmmutation')
insert into SysRoleMenu values ('SFM', 'abinqsfmpersinfo')
insert into SysRoleMenu values ('SFM', 'abinqsfmtrend')
insert into SysRoleMenu values ('SFM', 'abpersinfo')
insert into SysRoleMenu values ('SFM', 'abreport')
insert into SysRoleMenu values ('SFM', 'absalesinfo')
insert into SysRoleMenu values ('SFM', 'abserviceinfo')
insert into SysRoleMenu values ('SFM', 'abupload')

insert into SysRoleMenu values ('SFM-ADMIN', 'abdasempldept')
insert into SysRoleMenu values ('SFM-ADMIN', 'abdasempldist')
insert into SysRoleMenu values ('SFM-ADMIN', 'abdashboard')
insert into SysRoleMenu values ('SFM-ADMIN', 'abdept')
insert into SysRoleMenu values ('SFM-ADMIN', 'abempl')
insert into SysRoleMenu values ('SFM-ADMIN', 'abholiday')
insert into SysRoleMenu values ('SFM-ADMIN', 'abinqempl')
insert into SysRoleMenu values ('SFM-ADMIN', 'abinqgen')
insert into SysRoleMenu values ('SFM-ADMIN', 'abinqpersachieve')
insert into SysRoleMenu values ('SFM-ADMIN', 'abinqpersinvalid')
insert into SysRoleMenu values ('SFM-ADMIN', 'abinqpersmuta')
insert into SysRoleMenu values ('SFM-ADMIN', 'abinqsfm')
insert into SysRoleMenu values ('SFM-ADMIN', 'abinqsfmmutation')
insert into SysRoleMenu values ('SFM-ADMIN', 'abinqsfmpersinfo')
insert into SysRoleMenu values ('SFM-ADMIN', 'abinqsfmtrend')
insert into SysRoleMenu values ('SFM-ADMIN', 'ablookup')
insert into SysRoleMenu values ('SFM-ADMIN', 'abmaster')
insert into SysRoleMenu values ('SFM-ADMIN', 'abovertime')
insert into SysRoleMenu values ('SFM-ADMIN', 'abpersinfo')
insert into SysRoleMenu values ('SFM-ADMIN', 'abposition')
insert into SysRoleMenu values ('SFM-ADMIN', 'abreport')
insert into SysRoleMenu values ('SFM-ADMIN', 'absalesinfo')
insert into SysRoleMenu values ('SFM-ADMIN', 'abserviceinfo')
insert into SysRoleMenu values ('SFM-ADMIN', 'abshift')
insert into SysRoleMenu values ('SFM-ADMIN', 'abshiftmapping')
insert into SysRoleMenu values ('SFM-ADMIN', 'abtrans')
insert into SysRoleMenu values ('SFM-ADMIN', 'abupload')
insert into SysRoleMenu values ('SFM-ADMIN', 'gnmember')
insert into SysRoleMenu values ('SFM-ADMIN', 'gnuser')
