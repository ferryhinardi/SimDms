
go
if object_id('uspfn_mpTrainingSummary') is not null
	drop procedure uspfn_mpTrainingSummary

go
create procedure uspfn_mpTrainingSummary
	@CompanyCode varchar (20),
	@BranchCode varchar (20) = '',
	@Periode    varchar (20) = ''

as
begin
	begin try
		drop table #t_temp_1;
	end try
	begin catch
	end catch

	if @BranchCode is null or @BranchCode = ''
	begin
		set @BranchCode = '%';
	end
	

	declare @t_grade as table (Code varchar(10), Name varchar(50))

	insert into @t_grade values ('1', 'Trainee')
	insert into @t_grade values ('2', 'Silver')
	insert into @t_grade values ('3', 'Gold')
	insert into @t_grade values ('4', 'Platinum')

	;with x as (
	select a.CompanyCode
		 , a.EmployeeID
		 , BranchCode = isnull((select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by MutationDate desc), '')
		 , Position = isnull((select top 1 Position from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and left(convert(varchar, AssignDate, 112), 6) <= @Periode order by AssignDate desc), '')
		 , Grade = isnull((select top 1 Grade from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and left(convert(varchar, AssignDate, 112), 6) <= @Periode order by AssignDate desc), '')
	  from HrEmployee a
	 where 1 = 1
	   and a.Department = 'SALES'
	   and a.CompanyCode = @CompanyCode
	   and a.PersonnelStatus != '2'
	   and left(convert(varchar, a.JoinDate, 112), 6) <= @Periode
	   and left(convert(varchar, isnull(a.ResignDate, '2100-01-01'), 112), 6) > @Periode
	)
	,y as (
	select a.CompanyCode, a.BranchCode, a.EmployeeID
		 , a.Position, a.Grade
		 , IsTraining = case isnull(a.Position, 'S')
			 when 'T'  then (select (case when count(*) > 0 then 1 else 0 end) from HrEmployeeTraining where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and left(convert(varchar, TrainingDate, 112), 6) <= @Periode and TrainingCode = 'STDP7')
			 when 'S'  then (select (case when count(*) > 0 then 1 else 0 end) from HrEmployeeTraining where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and left(convert(varchar, TrainingDate, 112), 6) <= @Periode and TrainingCode = 'SPSS')
			 when 'G'  then (select (case when count(*) > 0 then 1 else 0 end) from HrEmployeeTraining where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and left(convert(varchar, TrainingDate, 112), 6) <= @Periode and TrainingCode = 'SPSG')
			 when 'P'  then (select (case when count(*) > 0 then 1 else 0 end) from HrEmployeeTraining where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and left(convert(varchar, TrainingDate, 112), 6) <= @Periode and TrainingCode = 'SPSP')
			 when 'SC' then (select (case when count(*) > 0 then 1 else 0 end) from HrEmployeeTraining where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and left(convert(varchar, TrainingDate, 112), 6) <= @Periode and TrainingCode = 'SCB')
			 when 'SH' then (select (case when count(*) > 0 then 1 else 0 end) from HrEmployeeTraining where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and left(convert(varchar, TrainingDate, 112), 6) <= @Periode and TrainingCode = 'SHB')
			 when 'SM' then (select (case when count(*) > 0 then 1 else 0 end) from HrEmployeeTraining where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and left(convert(varchar, TrainingDate, 112), 6) <= @Periode and TrainingCode = 'BMDPB')
			 else 0
			 end
	  from x a
	)
	,z as (
	select a.CompanyCode
		 , a.BranchCode
		 , BranchName = isnull(b.BranchName, '>> INVALID MUTATION <<')
		 , BM  = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'BM')
		 , BMT = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'BM' and isnull(IsTraining, 0) = 1)
		 , BMN = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'BM' and isnull(IsTraining, 0) = 0)

		 , SH  = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'SH')
		 , SHT = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'SH' and isnull(IsTraining, 0) = 1)
		 , SHN = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'SH' and isnull(IsTraining, 0) = 0)

		 , SC  = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'SC')
		 , SCT = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'SC' and isnull(IsTraining, 0) = 1)
		 , SCN = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'SC' and isnull(IsTraining, 0) = 0)

		 , P   = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and Grade = '4')
		 , PT  = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and Grade = '4' and isnull(IsTraining, 0) = 1)
		 , PN  = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and Grade = '4' and isnull(IsTraining, 0) = 0)

		 , G   = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and Grade = '3')
		 , GT  = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and Grade = '3' and isnull(IsTraining, 0) = 1)
		 , GN  = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and Grade = '3' and isnull(IsTraining, 0) = 0)

		 , S   = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and Grade = '2')
		 , ST  = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and Grade = '2' and isnull(IsTraining, 0) = 1)
		 , SN  = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and Grade = '2' and isnull(IsTraining, 0) = 0)

		 , T   = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and isnull(Grade, '1') = '1')
		 , TT  = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and isnull(Grade, '1') = '1' and isnull(IsTraining, 0) = 1)
		 , TN  = (select count(*) from y where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and isnull(Grade, '1') = '1' and isnull(IsTraining, 0) = 0)
	  from y a
	  left join OutletInfo b
		on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	 group by a.CompanyCode, a.BranchCode, b.BranchName
	)
	select * 
	  into #t_temp_1 
	  from z

	select *
	  from #t_temp_1 a
	 where a.BranchCode like @BranchCode
	 order by a.BranchName asc;
end





go
exec uspfn_mpTrainingSummary '6419401', '', '201406'
--exec uspfn_mpTrainingSummary '6021406', '', '201401'

-- 1 : Active
-- 2 : Non Active
-- 3 : Keluar
-- select * from HrEmployeeTraining
--select * from HrEmployeeTraining where CompanyCode = '6006406' and EmployeeID = '2'

