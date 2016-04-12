alter table SysRole 
add IsChangeBranchCode bit null default 0
go

alter table SysMenuDms
  add MenuIcon varchar(250) null;
go

alter table [SysModule] add Icon varchar(50)
go
