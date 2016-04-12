
go
if object_id('SpUpdatePosition') is not null
	drop procedure SpUpdatePosition

go
create procedure SpUpdatePosition (
	@CompanyCode varchar(25),
	@EmployeeID varchar(25)
)
as
begin
	declare @Department varchar(25);
	declare @Position varchar(25);
	declare @Grade varchar(25);

	set @Department = (
		select top 1

		from 
			HrEmployee
	)
end