delete from bitpku..sysmenudms
go
insert into bitpku..sysmenudms select * from sysmenudms

insert into bitpku..sysrolemenu select * from sysrolemenu where roleid = 'ADMIN' and menuid not in (select menuid from sbtjkt..sysrolemenu where roleid = 'ADMIN')

insert into bitpku..sysrolemodule select * from sysrolemodule where roleid = 'ADMIN' and moduleid not in (select moduleid from sbtjkt..sysrolemodule where roleid = 'ADMIN')

insert into bitpku..sysmodule select * from sysmodule where moduleid not in (select moduleid from sbtjkt..sysmodule)

