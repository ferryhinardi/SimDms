go
if object_id('uspfn_MpTrainingDetail') is not null
	drop procedure uspfn_MpTrainingDetail

go
create procedure uspfn_MpTrainingDetail
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@Position varchar(5),
	@Grade char(1),
	@Periode varchar(6)
as
begin
	if @BranchCode is null or @BranchCode = ''
	begin
		set @BranchCode = '%';
	end	

	begin try
		drop table #t_employee;
		drop table #t_employee_training_s;
		drop table #t_employee_training_sc;
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
		 , Training = d.TrainingCode
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
	 outer apply (
		select top 1
		       x.TrainingCode
		  from HrEmployeeTraining x
		 where x.CompanyCode = a.CompanyCode
		   and x.EmployeeID = a.EmployeeID
		   and (
					x.IsDeleted is null
					or 
					x.IsDeleted = 0
		       )
		   and left(convert(varchar, x.TrainingDate, 112), 6) <= @Periode
		 order by x.TrainingDate desc
	 ) as d
	 where a.CompanyCode = @CompanyCode
	   and (
				@Periode < left(convert(varchar, a.ResignDate, 112), 6) 
				or
				a.ResignDate is null
		   )

	if @Position = 'S'
	begin
		begin try
			drop table #t_employee_training_s;
		end try
		begin catch
		end catch

		select *
		  into #t_employee_training_s
		  from #t_employee a
		 where a.Department = 'SALES'
		   and a.Position = 'S'
		   and a.BranchCode is not null

		if @Grade = '1'
		begin
			select a.CompanyCode
				 , a.BranchCode
				 , a.BranchName
				 , SalesTrainee = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '1' )
				 , STDP1 = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '1' and x.Training = 'STDP1' )
				 , STDP2 = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '1' and x.Training = 'STDP2' )
				 , STDP3 = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '1' and x.Training = 'STDP3' )
				 , STDP4 = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '1' and x.Training = 'STDP4' )
				 , STDP5 = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '1' and x.Training = 'STDP5' )
				 , STDP6 = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '1' and x.Training = 'STDP6' )
				 , STDP7 = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '1' and x.Training = 'STDP7' )
			  from OutletInfo a
			 where a.CompanyCode = @CompanyCode
			   and a.BranchCode like @BranchCode	
		end
		
		if @Grade = '2'
		begin
			select a.CompanyCode
				 , a.BranchCode
				 , a.BranchName
				 , SalesSilver = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '2' )
				 , SPSSilver = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '2' and x.Training = 'SPSS' )
			  from OutletInfo a
			 where a.CompanyCode = @CompanyCode
			   and a.BranchCode like @BranchCode	
		end

		if @Grade = '3'
		begin
			select a.CompanyCode
				 , a.BranchCode
				 , a.BranchName
				 , SalesGold = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '3' )
				 , SPSGold = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '3' and x.Training = 'SPSG' )
			  from OutletInfo a
			 where a.CompanyCode = @CompanyCode
			   and a.BranchCode like @BranchCode	
		end

		if @Grade = '4'
		begin
			select a.CompanyCode
				 , a.BranchCode
				 , a.BranchName
				 , SalesPlatinum = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '4' )
				 , SPSPlatinum = ( select count(x.EmployeeID) from #t_employee_training_s x where x.BranchCode = a.BranchCode and x.Grade = '4' and x.Training = 'SPSP' )
			  from OutletInfo a
			 where a.CompanyCode = @CompanyCode
			   and a.BranchCode like @BranchCode	
		end
	end

	if @Position = 'SC'
	begin
		begin try
			drop table #t_employee_training_sc
		end try
		begin catch
		end catch

		select *
		  into #t_employee_training_sc
		  from #t_employee a
		 where a.Department = 'SALES'
		   and a.Position = 'SC'
		   and a.BranchCode is not null
		
		select a.CompanyCode
				 , a.BranchCode
				 , a.BranchName
				 , SalesCoordinator = ( select count(x.EmployeeID) from #t_employee_training_sc x where x.CompanyCode = a.CompanyCode and x.BranchCode = a.BranchCode )
			     , SCBasic = ( select count(x.EmployeeID) from #t_employee_training_sc x where x.CompanyCode = a.CompanyCode and x.BranchCode = a.BranchCode and x.Training = 'SCB' )
			     , SCAdvance = ( select count(x.EmployeeID) from #t_employee_training_sc x where x.CompanyCode = a.CompanyCode and x.BranchCode = a.BranchCode and x.Training = 'SCA' )
			  from OutletInfo a
			 where a.CompanyCode = @CompanyCode
			   and a.BranchCode like @BranchCode	
	end

	if @Position = 'SH'
	begin
		begin try
			drop table #t_employee_training_sh
		end try
		begin catch
		end catch

		select *
		  into #t_employee_training_sh
		  from #t_employee a
		 where a.Department = 'SALES'
		   and a.Position = 'SH'
		   and a.BranchCode is not null
		
		select a.CompanyCode
			 , a.BranchCode
			 , a.BranchName
			 , SalesCoordinator = ( select count(x.EmployeeID) from #t_employee_training_sh x where x.CompanyCode = a.CompanyCode and x.BranchCode = a.BranchCode )
			 , SHBasic = ( select count(x.EmployeeID) from #t_employee_training_sh x where x.CompanyCode = a.CompanyCode and x.BranchCode = a.BranchCode and x.Training = 'SHB' )
			 , SHIntermediate = ( select count(x.EmployeeID) from #t_employee_training_sh x where x.CompanyCode = a.CompanyCode and x.BranchCode = a.BranchCode and x.Training = 'SHI' )
		from OutletInfo a
	   where a.CompanyCode = @CompanyCode
		 and a.BranchCode like @BranchCode	
	end

	if @Position = 'BM'
	begin
		begin try
			drop table #t_employee_training_bm
		end try
		begin catch
		end catch

		select *
		  into #t_employee_training_bm
		  from #t_employee a
		 where a.Department = 'SALES'
		   and a.Position = 'BM'
		   and a.BranchCode is not null
		
		select a.CompanyCode
			 , a.BranchCode
			 , a.BranchName
			 , BranchManager = ( select count(x.EmployeeID) from #t_employee_training_bm x where x.CompanyCode = a.CompanyCode and x.BranchCode = a.BranchCode )
			 , BMDPBasic = ( select count(x.EmployeeID) from #t_employee_training_bm x where x.CompanyCode = a.CompanyCode and x.BranchCode = a.BranchCode and x.Training = 'BMDPB' )
			 , BMDPIntermediate = ( select count(x.EmployeeID) from #t_employee_training_bm x where x.CompanyCode = a.CompanyCode and x.BranchCode = a.BranchCode and x.Training = 'BMDPA' )
		from OutletInfo a
	   where a.CompanyCode = @CompanyCode
		 and a.BranchCode like @BranchCode	
	end
end

go
exec uspfn_MpTrainingDetail '6006406', '6006406', 'BM', '1', '201406'