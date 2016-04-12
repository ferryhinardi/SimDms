go
if object_id('uspfn_IsNullOrEmpty') is not null
	drop function [dbo].[uspfn_IsNullOrEmpty]

go
create function [dbo].[uspfn_IsNullOrEmpty](
	@paramString varchar(200)
)
returns bit
as
begin
	declare @status bit;
	set @status = 0;

	if isnull(@paramString, '') = ''
	begin
		set @status = 1;
	end
	else
	begin
		set @status = 0;
	end

	if dbo.uspfn_trim(@paramString) = ''
	begin
		set @status = 1;
	end
	else
	begin
		set @status = 0;
	end

	return @status;
end

go
if object_id('uspfn_trim') is not null
	drop function uspfn_trim

go
create function uspfn_trim(
	@paramString varchar(200)
)
returns varchar(200)
as
begin
	return ltrim(rtrim(@paramString));
end;
