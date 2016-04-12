
go
if object_id('uspfn_GetElementSettings') is not null
	drop procedure uspfn_GetElementSettings

go
create procedure uspfn_GetElementSettings (
	@HashLink varchar(100) 
)
as
begin
	select 
		* 
	from 
		SysMenuDms a
	--where
		--a.MenuUrl like '%@HashLink%'
end

go
exec uspfn_GetElementSettings @HashLink='ab/empl/persinfo'

select * from SysMenuDms where MenuUrl like '%empl/persinfo%'

