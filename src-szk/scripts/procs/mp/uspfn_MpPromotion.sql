
go
if object_id('uspfn_MpPromotion') is not null	
	drop procedure uspfn_MpPromotion

go
create procedure uspfn_MpPromotion
	@CompanyCode varchar(25), 
	@BranchCode varchar(25),
	@DateFrom datetime,
	@DateTo datetime
as
begin
	if @BranchCode is null or @BranchCode = ''
	begin
		set @BranchCode = '%';
	end

	begin try
		drop table #t_employee;
	end try
	begin catch
	end catch

	select * 
	  into #t_employee
	  from (
			select a.CompanyCode
				 , BranchCode = ax01.BranchCode
				 , a.EmployeeID
				 , a.EmployeeName
				 , MutationDate = ax01.MutationDate
				 , PreviousDepartment= ''
				 , PreviousPosition = ''
				 , PreviousGrade = ''
				 , AssignedDateOfPreviousPosition = ''
				 , CurrentDepartment = ax02.Department
				 , CurrentPosition = ax02.Position
				 , CurrentGrade = ax02.Grade
				 , AssignedDateOfCurrentPosition = ax02.AssignDate
			  from HrEmployee a
			 outer apply (
						select top 1
							   x.BranchCode
							 , x.MutationDate
						  from HrEmployeeMutation x
						 where x.CompanyCode = a.CompanyCode
						   and x.EmployeeID = a.EmployeeID
						   and (
									x.IsDeleted is null
									or 
									x.IsDeleted = 0
							   )
						   and x.MutationDate between @DateFrom and @DateTo
						 order by x.MutationDate desc
				   ) as ax01
			 outer apply (
						select top 1
							   x.Department
							 , x.Position
							 , x.Grade
							 , x.AssignDate
						  from HrEmployeeAchievement x
						 where x.CompanyCode = a.CompanyCode
						   and x.EmployeeID = a.EmployeeID
						   and (
									x.IsDeleted is null
									or
									x.IsDeleted = 0
							   )
						   and x.AssignDate between @DateFrom and @DateTo
						 order by x.AssignDate desc
				   ) as ax02
			 where a.CompanyCode = @CompanyCode
			   and ax01.BranchCode is not null
			   and (
						a.ResignDate is null
						or
						a.ResignDate = '1900-01-01'
						or
						a.ResignDate > @DateTo
				   )
	) as temp;

	select *
	  into #t_employee_fix
	  from (
			select a.CompanyCode
				 , a.BranchCode
				 , a.MutationDate
				 , a.EmployeeID
				 , a.EmployeeName
				 , a.CurrentDepartment
				 , a.CurrentPosition
				 , a.CurrentGrade
				 , a.AssignedDateOfCurrentPosition
				 , PreviousDepartment= ax01.Department
				 , PreviousPosition = ax01.Position
				 , PreviousGrade = ax01.Grade
				 , AssignedDateOfPreviousPosition = ax01.AssignDate
			  from #t_employee a
			 outer apply (
						select top 1
							   x.*
						  from HrEmployeeAchievement x
						 where x.CompanyCode = a.CompanyCode
						   and x.EmployeeID = a.EmployeeID
						   and (
									x.IsDeleted is null
									or
									x.IsDeleted = 0
							   )
						   and x.AssignDate < a.AssignedDateOfCurrentPosition
						 order by x.AssignDate desc
				   ) as ax01
			 where a.CompanyCode = @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.CurrentPosition is not null
			   and ax01.Position is not null
	) as temp;

	--select *
	--  from #t_employee_fix;

	select a.CompanyCode
	     , a.BranchCode
		 , a.BranchName
		 , TraineeToSilver = (
				select count(x.EmployeeID)
				  from #t_employee_fix x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.CurrentDepartment = 'SALES'
				   and x.PreviousDepartment = 'SALES'
				   and x.CurrentPosition = 'S'
				   and x.PreviousPosition = 'S'
				   and x.CurrentGrade = 2
				   and x.PreviousGrade = 1
		   )
		 , SilverToGold = (
				select count(x.EmployeeID)
				  from #t_employee_fix x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.CurrentDepartment = 'SALES'
				   and x.PreviousDepartment = 'SALES'
				   and x.CurrentPosition = 'S'
				   and x.PreviousPosition = 'S'
				   and x.CurrentGrade = 3
				   and x.PreviousGrade = 2
		   )
		 , GoldToPlatinum = (
				select count(x.EmployeeID)
				  from #t_employee_fix x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.CurrentDepartment = 'SALES'
				   and x.PreviousDepartment = 'SALES'
				   and x.CurrentPosition = 'S'
				   and x.PreviousPosition = 'S'
				   and x.CurrentGrade = 4
				   and x.PreviousGrade = 3
		   )
		 , SalesPersonToSalesCoordinator = (
				select count(x.EmployeeID)
				  from #t_employee_fix x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.CurrentDepartment = 'SALES'
				   and x.PreviousDepartment = 'SALES'
				   and x.CurrentPosition = 'SC'
				   and x.PreviousPosition = 'S'
		   )
		 , SalesCoordinatorToSalesHead = (
				select count(x.EmployeeID)
				  from #t_employee_fix x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.CurrentDepartment = 'SALES'
				   and x.PreviousDepartment = 'SALES'
				   and x.CurrentPosition = 'SH'
				   and x.PreviousPosition = 'SC'
		   )
		 , SalesHeadToBranchManager = (
				select count(x.EmployeeID)
				  from #t_employee_fix x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.CurrentDepartment = 'SALES'
				   and x.PreviousDepartment = 'SALES'
				   and x.CurrentPosition = 'BM'
				   and x.PreviousPosition = 'SC'
		   )
		  
	  from OutletInfo a
	 where a.CompanyCode = @CompanyCode
	 order by a.BranchName asc
end

go
exec uspfn_MpPromotion '6006406', '', '2012-01-01', '2014-06-01'

