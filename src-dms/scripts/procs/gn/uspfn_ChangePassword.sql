
go
if object_id('uspfn_ChangePassword') is not null
	drop procedure uspfn_ChangePassword

go
create procedure uspfn_ChangePassword
	@UserId varchar(20),
	@Password varchar(200)

as

update SysUser set Password = @Password, RequiredChange = 0 where UserId = @UserId
insert into SysUserPwdHist (Id, UserId, Password, ChangeDate) values (newid(), @UserId, @Password, getdate())

go


