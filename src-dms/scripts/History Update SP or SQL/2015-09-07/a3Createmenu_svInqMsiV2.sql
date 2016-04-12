begin tran

if exists (select * from sysMenuDms where MenuId='svInqMsiV2')
begin
	select * from sysMenuDms where MenuId='svInqMsiV2'
end
else
begin
	insert into sysMenuDms values ('svInqMsiV2', 'Inquiry Suzuki MSI V2', 'svinquiry', 9, 2, 'inquiry/inqmsiv2', NULL)
end

commit