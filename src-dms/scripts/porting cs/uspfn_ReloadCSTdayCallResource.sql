-- Change CS 3 Days Call Components

go
if object_id('uspfn_ReloadCSTdayCallResource') is not null
	drop procedure uspfn_ReloadCSTdayCallResource

go
create procedure uspfn_ReloadCSTdayCallResource
as

begin
	declare @ParamDate datetime;
	declare @RecordCount int;
	
	set @ParamDate = convert(varchar(4), datepart(year, getdate()) )+ '-01-01';

	if object_id('CsLkuTDayCallView') is not null
	begin
		select 'CsLkuTDayCallView is exist';

		delete CsLkuTDayCallView 
		 where DODate >= @ParamDate

		set @RecordCount = ( select count(CompanyCode) from CsLkuTDayCallView );

		if @RecordCount = 0
		begin
			select 'Record is null -> regenerating';

			drop table CsLkuTDayCallView;
			
			with x as (
				select * from CsLkuTDayCallViewSource
			)
			select *
			  into CsLkuTDayCallView
			  from x
		end
		

		insert into CsLkuTDayCallView
		select *
		  from CsLkuTDayCallViewSource
		 where DODate >= @ParamDate
	end
	else
	begin
		select 'CsLkuTDayCallView is not exists';

		with x as (
			select * from CsLkuTDayCallViewSource
		)
		select *
		  into CsLkuTDayCallView
		  from x
	end

end;




go
exec uspfn_ReloadCSTdayCallResource
select * from CsLkuTDayCallView


