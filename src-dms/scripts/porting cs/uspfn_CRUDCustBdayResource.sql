
if object_id('uspfn_CRUDCustBdayResource') is not null
	drop procedure uspfn_CRUDCustBdayResource

go
create procedure uspfn_CRUDCustBdayResource
	@CompanyCode varchar(13),
	@CustomerCode varchar(13)
as
begin

	delete from CsLkuBirthdayView
	 where CompanyCode = @CompanyCode
       and CustomerCode = @CustomerCode

	if object_id('.#1') is not null
		drop table #1;

	with x as (
			select * 
			  from CsLkuBirthdayViewSource a
			 where a.CompanyCode = @CompanyCode
			   and a.CustomerCode = @CustomerCode
		)
	select * into #1
	  from x;

	insert into CsLkuBirthdayView
	select * from #1;
end





go

