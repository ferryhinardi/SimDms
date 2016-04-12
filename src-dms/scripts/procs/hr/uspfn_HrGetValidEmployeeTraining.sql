
go
if object_id('uspfn_HrGetValidEmployeeTraining') is not null
	drop procedure uspfn_HrGetValidEmployeeTraining

go
create procedure uspfn_HrGetValidEmployeeTraining (
	@CompanyCode varchar(15),
	@Department varchar(15),
	@Position varchar(15),
	@Grade varchar(15)
)
as 
begin
	declare @tbl_ListTraining table (
		CompanyCode varchar(15),
		Department varchar(15),
		Position varchar(15),
		Grade varchar(15),
		TrainingCode varchar(15),
		TrainingName varchar(100)
	)

	delete @tbl_ListTraining;

	insert into @tbl_ListTraining
	select 
		a.*
	from
		HrMstTrainingView a
	where
		a.CompanyCode = @CompanyCode
		and
		a.Department = @Department
		and
		a.Position = @Position

	--select dbo.uspfn_IsNullOrEmpty(@Grade); 

	declare @isEmpty bit;
	set @isEmpty = 0;

	set @isEmpty = dbo.uspfn_IsNullOrEmpty(@Grade);

	--if dbo.uspfn_IsNullOrEmpty(@Grade) = 0
	if @isEmpty = 0
	begin
		select 
			a.Department,
			a.Position,
			a.Grade,
			value = a.TrainingCode,
			text = a.TrainingName
		from 
			@tbl_ListTraining a
		where
			a.Grade = @Grade
	end
	else
	begin
		select 
			a.Department,
			a.Position,
			a.Grade,
			value = a.TrainingCode,
			text = a.TrainingName
		from 
			@tbl_ListTraining a
	end
end

go
exec uspfn_HrGetValidEmployeeTraining @CompanyCode='6115204', @Department='SALES', @Position='S', @Grade='1'

--select * from gnMstOrganizationDtl

