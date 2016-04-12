
go
if object_id('uspfn_AbSplitString') is not null
	drop function uspfn_AbSplitString

GO
create FUNCTION [dbo].[uspfn_AbSplitString] (
	@String varchar(max), @Delimiter char(1)
)
returns @temptable TABLE (Part varchar(8000))
as
begin
	declare @idx int
	declare @slice varchar(8000)

	select @idx = 1

	if len(@String)<1 or @String is null return

	while @idx!= 0
	begin
		set @idx = charindex(@Delimiter,@String)
		if @idx!=0
			set @slice = left(@String,@idx - 1)
		else
			set @slice = @String

		if(len(@slice)>0)
			insert into @temptable(Part) values(@slice)

		set @String = right(@String,len(@String) - @idx)
		if len(@String) = 0 break
	end
	return 
end