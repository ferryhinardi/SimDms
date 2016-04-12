USE [SimDms]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_UtlInsertModule]    Script Date: 12/22/2014 2:41:34 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		David Leonardo
-- Create date: December 22, 2014
-- Description:	Insert Module
-- =============================================
ALTER PROCEDURE [dbo].[uspfn_UtlInsertModule] 
	-- Add the parameters for the stored procedure here
	@roleId varchar(100), 
	@moduleId varchar(50)
AS
BEGIN
	declare @tblAllMenu Table
(
MenuId varchar(100)
)

declare @tblMenu Table
(
MenuId varchar(100)
)

declare @tblCis Table
(
MenuId varchar(100)
)

insert into @tblAllMenu select MenuId from SysMenu where (MenuId = @moduleId and MenuLevel = 0) or ( MenuHeader = @moduleId and MenuLevel = 1)
or (MenuHeader in (select MenuId from SysMenu where MenuHeader = @moduleId and MenuLevel = 1) )

DECLARE @count int

insert into @tblMenu select MenuId from @tblAllMenu
Except
select MenuId from SysRoleMenu where RoleID = @roleId

set @count = (select count(*) from @tblMenu)

if(@count > 0)
begin

if(@moduleId ='snis')
begin

insert into @tblCis 
select MenuId from SdmsCis..CisMenus 
EXCEPT
select MenuUrl from SysMenu where MenuId in( select MenuId from SysRoleMenu where MenuId in (select * from @tblAllMenu))

insert into SdmsCis..CisMenuUserRoles select MenuId, @roleId from @tblCis

end
insert into SysRoleMenu select @roleId,* from @tblMenu
end
END