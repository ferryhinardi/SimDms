
go
if object_id('uspfn_HrTrainingList') is not null
	drop procedure uspfn_HrTrainingList

go
create procedure uspfn_HrTrainingList(
	@CompanyCode varchar(15),
	@Department varchar(15),
	@Position varchar(15),
	@Grade varchar(15)
)
as
begin
	create table #MasterTraining  (
		Department varchar(15),
		Position varchar(15),
		Grade varchar(15),
		TrainingCode varchar(50),
		TrainingName varchar(50),
	);

	delete #MasterTraining;
	insert into #MasterTraining
	select 
		b.Department,
		b.Position,
		b.Grade,
		a.TrainingCode,
		a.TrainingName	
	from
		HrMstTraining a
	inner join
		HrDepartmentTraining b
	on
		a.CompanyCode = b.CompanyCode
		and
		a.TrainingCode = b.TrainingCode
	where
		a.CompanyCode = @CompanyCode
		and
		b.Department = @Department
		and
		b.Position = @Position
	
	--select ('[' + @Grade + ']') as Grade, LEN(LTRIM(RTRIM(@Grade))) as Len
	
	if ISNULL(@Grade, '') <> '-' and LTRIM(RTRIM(@Grade)) <> '' and LEN(LTRIM(RTRIM(@Grade))) > 0
	begin
		select 
			a.TrainingCode as value,
			a.TrainingName as text
		from 
			#MasterTraining a
		where
			a.Grade = lTRIM(RTRIM(@Grade));
	end
	else
	begin
		select 
			a.TrainingCode as value,
			a.TrainingName as text
		from 
			#MasterTraining a
	end
end

