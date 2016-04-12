
if object_id('uspfn_CRUDSTNKExtResource') is not null
	drop procedure uspfn_CRUDSTNKExtResource

go
create procedure uspfn_CRUDSTNKExtResource
	@CompanyCode varchar(13),
	@CustomerCode varchar(13),
	@Chassis varchar(50)
as
begin

	delete from CsLkuStnkExtView
	 where CompanyCode = @CompanyCode
       and CustomerCode = @CustomerCode
       and Chassis = @Chassis

	if object_id('.#1') is not null
		drop table #1;

	with x as (
			select * 
			  from CsLkuStnkExtViewSource a
			 where a.CompanyCode = @CompanyCode
			   and a.CustomerCode = @CustomerCode
			   and a.Chassis = @Chassis
		)
	select * into #1
	  from x;

	insert into CsLkuStnkExtView
	select * from #1;
end





go
--exec uspfn_CRUDSTNKExtResource @CompanyCode='6115204', @CustomerCode='200039246', @Chassis='MH8NF4FAADJ117592'

