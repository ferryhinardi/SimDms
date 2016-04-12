
go
if object_id('uspfn_MpListOfYear') is not null
	drop procedure uspfn_MpListOfYear;

go
create procedure uspfn_MpListOfYear
as
begin
	declare @YearFrom int;
	declare @YearTo int;
	declare @YearList table (
		text int not null,
		value int not null
	);

	set @YearFrom = ( 
		select top 1 
		       Year(a.JoinDate)
		  from HrEmployee a
		 where a.JoinDate is not null
		   and year(a.JoinDate) >= 1950
		 order by a.JoinDate asc
	);
	set @YearTo = year(getdate());

	delete @YearList;

	while @YearFrom <= @YearTo
	begin
		insert into @YearList (text, value)
		values (@YearFrom, @YearFrom);

		set @YearFrom = @YearFrom + 1;
	end;

	select * from @YearList order by text asc;
end

go
exec uspfn_MpListOfYear