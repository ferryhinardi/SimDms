go
if object_id('uspfn_MpDataTrend') is not null
	drop procedure uspfn_MpDataTrend 

go
create procedure uspfn_MpDataTrend 
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@Position varchar(5),
	@Periode varchar(6)
as
begin
	if @BranchCode is null or @BranchCode = ''
	begin
		set @BranchCode = '%';
	end	

	begin try
		drop table #t_employee;
		drop table #t_employee_trend;
		drop table #t_output;
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
	   and (
				@Periode < left(convert(varchar, a.ResignDate, 112), 6) 
				or
				a.ResignDate is null
		   )

	select * 
	  into #t_employee_trend
	  from #t_employee a
	 where a.Department = 'SALES'
	   and a.Position = @Position;

	declare @YearComparator varchar(4);
	set @YearComparator = left(@Periode, 4);

	select *
	  into #t_output
	  from (
		select a.CompanyCode
			 , a.BranchCode
			 , a.BranchName
			 , January = ( select count(x.EmployeeID) from #t_employee_trend x where x.BranchCode = a.BranchCode and left(convert(varchar, x.AssignDate, 112), 6) <= ( @YearComparator + '01' ) )
			 , February = ( select count(x.EmployeeID) from #t_employee_trend x where x.BranchCode = a.BranchCode and left(convert(varchar, x.AssignDate, 112), 6) <= ( @YearComparator + '02' ) )
			 , March = ( select count(x.EmployeeID) from #t_employee_trend x where x.BranchCode = a.BranchCode and left(convert(varchar, x.AssignDate, 112), 6) <= ( @YearComparator + '03' ) )
			 , April = ( select count(x.EmployeeID) from #t_employee_trend x where x.BranchCode = a.BranchCode and left(convert(varchar, x.AssignDate, 112), 6) <= ( @YearComparator + '04' ) )
			 , May = ( select count(x.EmployeeID) from #t_employee_trend x where x.BranchCode = a.BranchCode and left(convert(varchar, x.AssignDate, 112), 6) <= ( @YearComparator + '05' ) )
			 , June = ( select count(x.EmployeeID) from #t_employee_trend x where x.BranchCode = a.BranchCode and left(convert(varchar, x.AssignDate, 112), 6) <= ( @YearComparator + '06' ) )
			 , July = ( select count(x.EmployeeID) from #t_employee_trend x where x.BranchCode = a.BranchCode and left(convert(varchar, x.AssignDate, 112), 6) <= ( @YearComparator + '07' ) )
			 , August = ( select count(x.EmployeeID) from #t_employee_trend x where x.BranchCode = a.BranchCode and left(convert(varchar, x.AssignDate, 112), 6) <= ( @YearComparator + '08' ) )
			 , September = ( select count(x.EmployeeID) from #t_employee_trend x where x.BranchCode = a.BranchCode and left(convert(varchar, x.AssignDate, 112), 6) <= ( @YearComparator + '09' ) )
			 , October = ( select count(x.EmployeeID) from #t_employee_trend x where x.BranchCode = a.BranchCode and left(convert(varchar, x.AssignDate, 112), 6) <= ( @YearComparator + '10' ) )
			 , November = ( select count(x.EmployeeID) from #t_employee_trend x where x.BranchCode = a.BranchCode and left(convert(varchar, x.AssignDate, 112), 6) <= ( @YearComparator + '11' ) )
			 , December = ( select count(x.EmployeeID) from #t_employee_trend x where x.BranchCode = a.BranchCode and left(convert(varchar, x.AssignDate, 112), 6) <= ( @YearComparator + '12' ) )
		 from OutletInfo a
		where a.CompanyCode = @CompanyCode
		  and a.BranchCode like @BranchCode	
	  ) as a;

	declare @MonthComparator varchar(2);
	set @MonthComparator = right(@Periode, 2);

	declare @Iterator int;

	set @Iterator = convert(int, @MonthComparator) + 1;

	if @MonthComparator < 12
	begin
		declare @Qry nvarchar(max);
		set @Qry = 'update #t_output set ';
		
		while @Iterator <= 12
		begin
			set @Qry = @Qry + convert(varchar, (
				case
					when @Iterator = 1 then ' January = 0 ,'
					when @Iterator = 2 then ' February = 0 ,'
					when @Iterator = 3 then ' March = 0 ,'
					when @Iterator = 4 then ' April = 0 ,'
					when @Iterator = 5 then ' May = 0 ,'
					when @Iterator = 6 then ' June = 0 ,'
					when @Iterator = 7 then ' July = 0 ,'
					when @Iterator = 8 then ' August = 0 ,'
					when @Iterator = 9 then ' September = 0 ,'
					when @Iterator = 10 then ' October = 0 ,'
					when @Iterator = 11 then ' November = 0 ,'
					when @Iterator = 12 then ' December = 0 ,'
				end			
			));

			set @Iterator = @Iterator + 1;
		end

		set @Qry = substring(@Qry, 0, len(@Qry));
		exec sp_executesql @Qry;
	end

	select * from #t_output;
end

go
exec uspfn_MpDataTrend '6021406', '', 'BM', '201406'