-- Change CS 3 Days Call Components

go
if object_id('uspfn_ReloadBPKBReminderResource') is not null
	drop procedure uspfn_ReloadBPKBReminderResource

go
create procedure uspfn_ReloadBPKBReminderResource
as

begin
	declare @ParamDate datetime;
	declare @RecordCount int;
	
	set @ParamDate = convert(varchar(4), datepart(year, getdate()) )+ '-01-01';

	if object_id('CsLkuBpkbView') is not null
	begin
		select 'CsLkuBpkbView is exist';

		delete CsLkuBpkbView 
		 where BpkbDate >= @ParamDate

		set @RecordCount = ( select count(CompanyCode) from CsLkuBpkbView );

		if @RecordCount = 0
		begin
			select 'Record is null -> regenerating';

			drop table CsLkuBpkbView;
			
			with x as (
				select * from CsLkuBpkbViewSource
			)
			select *
			  into CsLkuBpkbView
			  from x
		end

		insert into CsLkuBpkbView
		select *
		  from CsLkuBpkbViewSource
		 where BpkbDate >= @ParamDate
	end
	else
	begin
		select 'CsLkuBpkbView is not exists';

		with x as (
			select * from CsLkuBpkbViewSource
		)
		select *
		  into CsLkuBpkbView
		  from x
	end

end;





go
exec uspfn_ReloadBPKBReminderResource
select * from CsLkuBpkbView


