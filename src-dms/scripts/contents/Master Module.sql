delete SysModule

go
insert into SysModule (ModuleId, ModuleCaption, ModuleIndex, ModuleUrl, InternalLink, IsPublish)
values ('gn', 'General Module', 1, '', 1, 1)

go
insert into SysModule (ModuleId, ModuleCaption, ModuleIndex, ModuleUrl, InternalLink, IsPublish)
values ('its', 'Inquiry Tracking System', 2, '', 1, 1)

go
insert into SysModule (ModuleId, ModuleCaption, ModuleIndex, ModuleUrl, InternalLink, IsPublish)
values ('om', 'Sales', 3, '', 1, 1)

go
insert into SysModule (ModuleId, ModuleCaption, ModuleIndex, ModuleUrl, InternalLink, IsPublish)
values ('sp', 'Sparepart', 4, '', 1, 1)

go
insert into SysModule (ModuleId, ModuleCaption, ModuleIndex, ModuleUrl, InternalLink, IsPublish)
values ('sv', 'Service', 5, '', 1, 1)

go
insert into SysModule (ModuleId, ModuleCaption, ModuleIndex, ModuleUrl, InternalLink, IsPublish)
values ('tax', 'Tax', 6, '', 1, 1)

go
insert into SysModule (ModuleId, ModuleCaption, ModuleIndex, ModuleUrl, InternalLink, IsPublish)
values ('ab', 'Man Power Management', 7, '', 1, 1)

go
insert into SysModule (ModuleId, ModuleCaption, ModuleIndex, ModuleUrl, InternalLink, IsPublish)
values ('cs', 'Customer Satisfaction', 8, '', 1, 1)

go
insert into SysModule (ModuleId, ModuleCaption, ModuleIndex, ModuleUrl, InternalLink, IsPublish)
values ('srp', 'Suzuki Retention Program', 9, 'srpdemo/home', 0, 0)

go
insert into SysModule (ModuleId, ModuleCaption, ModuleIndex, ModuleUrl, InternalLink, IsPublish)
values ('dc', 'Documentation', 10, 'RedirectToDoc', 1, 0)

go
select * from SysModule