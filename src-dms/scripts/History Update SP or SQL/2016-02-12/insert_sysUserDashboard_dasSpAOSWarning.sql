--begin tran
alter table sysUserDashboard
drop constraint [PK_sysUserDashboard]
go

alter table sysUserDashboard
alter column UserID [varchar](20) not null
go

alter table sysUserDashboard
add constraint [PK_sysUserDashboard] PRIMARY KEY 
(
	[UserID] ASC,
	[ModuleID] ASC
)
go

insert into sysUserDashboard 
select UserID ,'mdlSparepart' ,'dasSpAOSWarning', ''  from sysUser
go
--rollback tran