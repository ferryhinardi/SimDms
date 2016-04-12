
go
if object_id('uspfn_mpDashboard') is not null
	drop procedure uspfn_mpDashboard

go
create procedure uspfn_mpDashboard
	@CompanyCode varchar (20),
	@BranchCode varchar (20) = '',
	@Periode    varchar (20) = ''

as
begin
	if @BranchCode is null or @BranchCode = ''
	begin
		set @BranchCode = '%';
	end

	begin try
		drop table #temp_1;
	end try
	begin catch
	end catch

	declare @t_grade as table (Code varchar(10), Name varchar(50))

	insert into @t_grade values ('1', 'Trainee')
	insert into @t_grade values ('2', 'Silver')
	insert into @t_grade values ('3', 'Gold')
	insert into @t_grade values ('4', 'Platinum')

	;with x as (
	select a.CompanyCode
		 , BranchCode = isnull((select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by MutationDate desc), '')
		 , a.EmployeeID
		 , Position = isnull((
					  select top 1 Position
						from HrEmployeeAchievement
					   where CompanyCode = a.CompanyCode
						  and EmployeeID = a.EmployeeID
						  and left(convert(varchar, AssignDate, 112), 6) <= @Periode
						order by AssignDate desc), '')
		 , Grade = isnull((
					  select top 1 Grade
						from HrEmployeeAchievement
					   where CompanyCode = a.CompanyCode
						  and EmployeeID = a.EmployeeID
						  and left(convert(varchar, AssignDate, 112), 6) <= @Periode
						order by AssignDate desc), '')
		 , a.JoinDate
		 , a.ResignDate
	  from HrEmployee a
	 where 1 = 1
	   and a.Department = 'SALES'
	   and a.CompanyCode = @CompanyCode
	   and a.PersonnelStatus != '2'
	   and left(convert(varchar, a.JoinDate, 112), 6) <= @Periode
	   and left(convert(varchar, isnull(a.ResignDate, '2100-01-01'), 112), 6) > @Periode
	)
	,y as (
	select a.CompanyCode
		 , BranchCode = isnull(a.BranchCode, '-')
		 , BranchName = isnull(b.BranchName, '>> INVALID MUTATION <<')
		 , BranchManager = (select count(*) from x where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'BM')
		 , SalesHead = (select count(*) from x where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'SH')
		 , SalesCoordinator = (select count(*) from x where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'SC')
		 , Salesman = (select count(*) from x where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S')
		 , SalesmanPlatinum = (select count(*) from x where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and Grade = '4')
		 , SalesmanGold = (select count(*) from x where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and Grade = '3')
		 , SalesmanSilver = (select count(*) from x where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and Grade = '2')
		 , SalesmanTrainee = (select count(*) from x where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and Position = 'S' and isnull(Grade, '1') = '1')
		 , TotalSalesForce = (select count(*) from x where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode)
	  from x a
	  left join OutletInfo b
		on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	 group by a.CompanyCode, a.BranchCode, b.BranchName
	)
	select * 
	  into #temp_1
	  from y;

	with x as (
		select * 
		  from #temp_1 a
		 where a.BranchCode like @BranchCode
	)
	select * from x order by BranchName asc;
end



go
exec uspfn_mpDashboard '6489401', '', '201401'

-- 1 : Active
-- 2 : Non Active
-- 3 : Keluar
-- select * from HrEmployeeAchievement
