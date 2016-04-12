

if object_id('uspfn_ValidateUserMenu') is not null
	drop procedure uspfn_ValidateUserMenu

go
create procedure uspfn_ValidateUserMenu
	@UserId varchar(36),
	@Menu varchar(50)
as
begin
	select a.MenuId
	     , b.RoleId
		 , c.UserId
	  from SysMenuDms a
	 inner join sysRoleMenu b
	    on b.MenuId = a.MenuId
	 inner join SysRoleUser c
	    on c.RoleId = b.RoleId
	 where ( a.MenuId = @Menu or a.MenuUrl like '%/' + @Menu)
	   and c.UserId = @UserId
end

go
exec uspfn_ValidateUserMenu @UserId=N'ga',@Menu=N'user'