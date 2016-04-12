
go
if object_id('uspfn_trim') is not null
	drop function uspfn_trim

go
create function uspfn_trim (
	@paramString varchar(200)
)
returns varchar(200)
as
begin
	return ltrim(rtrim(@paramString));
end;

