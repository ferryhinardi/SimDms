CREATE procedure [dbo].[uspfn_checkdailyaccess]
@userid varchar(32)
as
if exists (select top 1 1 from sysRoleMenu
where RoleId = (select roleid from sysroleUser where userid=@userid)
and menuid = 'gnpostdaily')
	select 1 [result]
else
	select 0 [result]