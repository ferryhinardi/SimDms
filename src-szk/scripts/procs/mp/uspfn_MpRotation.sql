go
if object_id('uspfn_MpRotation') is not null
	drop procedure uspfn_MpRotation 

go
create procedure uspfn_MpRotation 
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@Position varchar(5),
	@Grade varchar(2),
	@Periode varchar(6)
as
begin
	if @BranchCode is null or @BranchCode = ''
	begin
		set @BranchCode = '%';
	end	

	if @Position is null or @Position = ''
	begin
		set @Position = '%';
	end	

	if @Grade is null or @Grade = ''
	begin
		set @Grade = '%';
	end	

	begin try
		drop table #t_employee;
		drop table #t_output;
		drop table #t_result;
	end try
	begin catch
	end catch

	select a.CompanyCode
	     , a.EmployeeID
		 , a.JoinDate
		 , a.ResignDate
		 , c.Department
		 , c.Position
		 , c.Grade
		 , BranchCode = b.BranchCode
		 , c.AssignDate
		 , c.IsJoinDate
	  into #t_employee 
	  from HrEmployee a
	 outer apply (
		select top 1 
		       x.BranchCode 
		  from HrEmployeeMutation x 
		 where x.CompanyCode = a.CompanyCode 
		   and x.EmployeeID = a.EmployeeID 
		   and ( x.IsDeleted = 0 or x.IsDeleted is null ) 
		   and left(convert(varchar, x.MutationDate, 112), 6) <= @Periode
		 order by x.MutationDate desc 
	 ) as b
	 outer apply (
		select top 1
		       x.Department
			 , x.Position
			 , x.Grade
			 , x.AssignDate
			 , x.IsJoinDate
		  from HrEmployeeAchievement x
		 where x.CompanyCode = a.CompanyCode
		   and x.EmployeeID = a.EmployeeID
		   and (
					x.IsDeleted is null
					or
					x.IsDeleted = 0
		       )
		   and left(convert(varchar, x.AssignDate, 112), 6) <= @Periode
		 order by x.AssignDate desc
	 ) as c
	 where a.CompanyCode = @CompanyCode

	select *
	  into #t_employee_rotation
	  from #t_employee a
	 where a.Department = 'SALES'
	   and a.Position like @Position
	   and a.Grade like @Grade;

	select * 
	  into #t_result
	  from (
		select a.CompanyCode
			 , a.BranchCode
			 , a.BranchName
			 , InitialAmount = ( 
						select count(x.EmployeeID) 
						  from #t_employee_rotation x 
						 where x.CompanyCode = a.CompanyCode 
						   and x.BranchCode = a.BranchCode
						   and left(convert(varchar, x.AssignDate, 112), 6) < @Periode
			   )
			 , JoinAmount = (
						select count(x.EmployeeID) 
						  from #t_employee_rotation x 
						 where x.CompanyCode = a.CompanyCode 
						   and x.BranchCode = a.BranchCode
						   and left(convert(varchar, x.AssignDate, 112), 6) = @Periode
						   and x.IsJoinDate = 1
			   )
			 , RotationInAmount = (
						select count(x.EmployeeID) 
						  from #t_employee_rotation x 
						 where x.CompanyCode = a.CompanyCode 
						   and x.BranchCode = a.BranchCode
						   and left(convert(varchar, x.AssignDate, 112), 6) = @Periode
						   and ( x.IsJoinDate = 0 or x.IsJoinDate is null )
			   )
			 , ResignAmount = (
						select count(x.EmployeeID) 
						  from #t_employee_rotation x 
						 where x.CompanyCode = a.CompanyCode 
						   and x.BranchCode = a.BranchCode 
						   and x.ResignDate is not null 
						   and left(convert(varchar, x.ResignDate, 112), 6) = @Periode 
			   )
			 , RotationOutAmount = 0
			 , TotalManPower = ( 
						select count(x.EmployeeID) 
						  from #t_employee_rotation x 
						 where x.CompanyCode = a.CompanyCode 
						   and x.BranchCode = a.BranchCode
						   and left(convert(varchar, x.AssignDate, 112), 6) <= @Periode
						   and (
								(
									x.ResignDate is not null
									and
									left(convert(varchar, x.ResignDate, 112), 6) > @Periode
								)
								or
								x.ResignDate is null
						   )
			   )
		  from OutletInfo a
		 where a.CompanyCode = @CompanyCode
		   and a.BranchCode like @BranchCode
	) as temp;

	select a.CompanyCode
	     , a.BranchCode
		 , a.BranchName 
	     , a.InitialAmount
	     , a.JoinAmount
		 , a.RotationInAmount
		 , a.ResignAmount
		 , RotationOutAmount = - ( (a.TotalManPower + a.ResignAmount) - ( a.InitialAmount + a.JoinAmount + a.RotationInAmount ) )  
		 , a.TotalManPower
	  from #t_result a;
end

go
exec uspfn_MpRotation '6021406', '', 'S', '1', '201406'
