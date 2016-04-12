if object_id('uspfn_syscreatepermission') is not null
	drop procedure uspfn_syscreatepermission
GO
CREATE procedure uspfn_syscreatepermission 
@roleid varchar(50)
AS
BEGIN
	insert into SysRoleMenuAccess(roleid,menuid)
	select  @roleid,menuid from VW_TREEMENUS a
	WHERE NOT EXISTS 
    (SELECT 0
     FROM SysRoleMenuAccess WITH (UPDLOCK, HOLDLOCK)
     WHERE RoleId=@roleid and MenuId=a.MenuId) 
END
GO
