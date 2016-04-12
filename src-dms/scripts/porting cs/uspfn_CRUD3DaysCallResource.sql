
if object_id('uspfn_CRUD3DaysCallResource') is not null
	drop procedure uspfn_CRUD3DaysCallResource

go
create procedure uspfn_CRUD3DaysCallResource
	@CompanyCode varchar(13),
	@CustomerCode varchar(13),
	@Chassis varchar(50)
as
begin
	delete CsLkuTDayCallView
	 where CompanyCode = @CompanyCode
       and CustomerCode = @CustomerCode
       and Chassis = @Chassis

	if object_id('.#1') is not null
		drop table #1;

	with x as (
			select * 
			  from CsLkuTDayCallViewSource a
			 where a.CompanyCode = @CompanyCode
			   and a.CustomerCode = @CustomerCode
			   and a.Chassis = @Chassis
		)
	select * into #1
	  from x;

	insert into CsLkuTDayCallView
	select * from #1;
end


