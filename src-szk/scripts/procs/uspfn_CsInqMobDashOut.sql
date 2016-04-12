
go
if object_id('uspfn_CsInqMobDashOut') is not null
	drop procedure uspfn_CsInqMobDashOut

go
create procedure uspfn_CsInqMobDashOut
	@DealerCode varchar(20) = '',
	@BranchCode varchar(20) = '',
	@ParDate    datetime = null
as

begin
	declare @tdash as table (
		code  varchar(60),
		name  varchar(60),
		value int
	)

	insert into @tdash values ('tdayscall','3 Days Call', 123)
	insert into @tdash values ('birthdayc','Birthday Call', 567)

	select name, value from @tdash
end