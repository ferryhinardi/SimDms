-- Change CS 3 Days Call Components

go
if object_id('uspfn_ReloadCustBDayResource') is not null
	drop procedure uspfn_ReloadCustBDayResource

go
create procedure uspfn_ReloadCustBDayResource
as

begin
	declare @ParamDate datetime;
	declare @RecordCount int;
	
	set @ParamDate = convert(varchar(4), datepart(year, getdate()) )+ '-01-01';

	if object_id('CsLkuBirthdayView') is not null
	begin
		select 'CsLkuBirthdayView is exist';

		 delete CsLkuBirthdayView 
		 --where DODate >= @ParamDate

		set @RecordCount = ( select count(CompanyCode) from CsLkuBirthdayView );

		if @RecordCount = 0
		begin
			select 'Record is null -> regenerating';

			drop table CsLkuBirthdayView;
			
			with x as (
				select * from CsLkuBirthdayViewSource
			)
			select *
			  into CsLkuBirthdayView
			  from x
		end
		

		insert into CsLkuBirthdayView
		select *
		  from CsLkuBirthdayViewSource
	end
	else
	begin
		select 'CsLkuTDayCallView is not exists';

		with x as (
			select * from CsLkuBirthdayViewSource
		)
		select *
		  into CsLkuBirthdayView
		  from x
	end

end;



go
exec uspfn_ReloadCustBDayResource
--select * from CsLkuBirthdayView

