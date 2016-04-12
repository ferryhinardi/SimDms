
go
if object_id('SysModuleView') is not null
	drop view SysModuleView;

go
create view SysModuleView 
as
select
	a.ModuleId,
	a.ModuleCaption,
	a.ModuleIndex,
	a.ModuleUrl,
	a.InternalLink,
	a.IsPublish,
	InternalLinkDescription = (
		case 
			when a.InternalLink=1 then 'Yes'
			else 'No'
		end 
	),
	IsPublishDescription = (
		case 
			when a.IsPublish=1 then 'Yes'
			else 'No'
		end
	)
from
	SysModule a

go
select * from SysModuleView