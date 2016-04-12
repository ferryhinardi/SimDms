alter procedure uspfn_ValidateUserMenu
	@UserId varchar(36),
	@Menu varchar(50)

as

select a.RoleID, a.MenuID, c.MenuUrl
  from SysRoleMenu a, SysRoleUser b, SysMenu c
 where b.RoleId = a.RoleID
   and c.MenuId = a.MenuID
   and (a.MenuID = @Menu or c.MenuUrl like ('%' + @Menu))
   and convert(varchar(36), UserId) = @UserId

go

exec uspfn_ValidateUserMenu 'B13ADCEA-E538-49FD-B9E0-34981CE26E2E', 'slidechart'

