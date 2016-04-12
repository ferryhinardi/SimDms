-- Change CS 3 Days Call Components

go
if object_id('uspfn_ReloadSTNKExtResource') is not null
	drop procedure uspfn_ReloadSTNKExtResource

go
create procedure uspfn_ReloadSTNKExtResource
as

begin
	declare @ParamDate datetime;
	declare @RecordCount int;
	
	set @ParamDate = convert(varchar(4), datepart(year, getdate()) )+ '-01-01';

	if object_id('CsLkuStnkExtView') is not null
	begin
		select 'CsLkuStnkExtView is exist';

		delete CsLkuStnkExtView 
		 where BpkbDate >= @ParamDate

		set @RecordCount = ( select count(CompanyCode) from CsLkuStnkExtView );

		if @RecordCount = 0
		begin
			select 'Record is null -> regenerating';

			drop table CsLkuStnkExtView;
			
			with x as (
				select * from CsLkuStnkExtViewSource
			)
			select *
			  into CsLkuStnkExtView
			  from x
		end

		insert into CsLkuStnkExtView
		select *
		  from CsLkuStnkExtViewSource
		 where BpkbDate >= @ParamDate
	end
	else
	begin
		select 'CsLkuStnkExtView is not exists';

		with x as (
			select * from CsLkuStnkExtViewSource
		)
		select *
		  into CsLkuStnkExtView
		  from x
	end

end;



--delete CsLkuStnkExtView



go
exec uspfn_ReloadSTNKExtResource
select * from CsLkuStnkExtView


