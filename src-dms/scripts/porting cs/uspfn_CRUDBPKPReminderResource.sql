
if object_id('uspfn_CRUDBPKPReminderResource') is not null
	drop procedure uspfn_CRUDBPKPReminderResource

go
create procedure uspfn_CRUDBPKPReminderResource
	@CompanyCode varchar(13),
	@CustomerCode varchar(13),
	@Chassis varchar(50)
as
begin
	delete CsLkuBpkbView
	 where CompanyCode = @CompanyCode
       and CustomerCode = @CustomerCode
       and Chassis = @Chassis

	if object_id('.#1') is not null
		drop table #1;

	with x as (
			select * 
			  from CsLkuBpkbViewSource a
			 where a.CompanyCode = @CompanyCode
			   and a.CustomerCode = @CustomerCode
			   and a.Chassis = @Chassis
		)
	select * into #1
	  from x;

	insert into CsLkuBpkbView
	select * from #1;
end


