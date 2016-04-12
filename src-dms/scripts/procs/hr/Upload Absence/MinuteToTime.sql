

go
if object_id('uspfn_MinuteToTime') is not null
	drop function uspfn_MinuteToTime

go
create function uspfn_MinuteToTime (
	@Minute int
)
returns varchar(5)
as

begin
	declare @Result char(5);

	if @Minute != 0 and @Minute != '0'
		set @Result = dateadd(minute, @Minute, '00:00')
	else
		set @Result='';

	return @Result;
end


GO


