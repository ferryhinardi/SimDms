
go
if object_id('uspfn_IsNeedChangePassword') is not null
	drop procedure uspfn_IsNeedChangePassword

go
create procedure uspfn_IsNeedChangePassword
	@UserId varchar(20) = ''

as

select isnull(RequiredChange, 0)
     , info = 'Demi keamanan dan IT Policy
Silahkan ubah password lama anda'
  from SysUser where UserId = @UserId

go

