create view VW_ROLEMENUPERMISSION
AS
select b.MenuCaption, b.MenuHeader, b.MenuLevel, b.MenuIndex,   a.*
from SysRoleMenuAccess a
inner join vw_treemenus b on (a.menuid = b.menuid)