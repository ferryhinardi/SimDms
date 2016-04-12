ALTER view [dbo].[SysUserView]
as
select a.UserId
     , a.FullName
     , a.Email
     , a.CompanyCode
     , a.BranchCode
	 , d.CompanyName as BranchName
     , a.TypeOfGoods
     , a.IsActive
     , b.RoleId
     , c.RoleName
	 , a.RequiredChange
	 , e.ProfitCenter
  from SysUser a
  left join SysRoleuser b 
    on b.UserId = a.UserId
  left join sysRole c 
    on c.RoleId = b.RoleId
  left join gnMstCoProfile d
    on d.CompanyCode = a.CompanyCode
   and d.BranchCode = a.BranchCode
   left join sysUserProfitCenter e
   on e.UserID = a.UserID

GO