go
if object_id('SysMenuView') is not null
	drop view SysMenuView

go
create view SysMenuView
as

select
	a.*,
	MenuHeaderDescription = (
		select top 1
			x.MenuCaption
		from
			SysMenu x
		where
			x.MenuId=a.MenuHeader
	)
from
	SysMenu a


go
select * from SysMenuView
