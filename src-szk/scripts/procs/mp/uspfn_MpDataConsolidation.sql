
go
if object_id('uspfn_MpDataConsolidation') is not null
	drop procedure uspfn_MpDataConsolidation

go
create procedure uspfn_MpDataConsolidation
	@CompanyCode varchar(25)
as
begin
	if @CompanyCode is null or @CompanyCode = ''
	begin
		set @CompanyCode = '%';
	end

	begin try
		drop table #t_employee;
	end try
	begin catch
	end catch;

	with x as ( 
		select a.CompanyCode
		     , BranchCode = ax01.BranchCode
			 , ax02.Department
			 , ax02.Position
			 , ax02.Grade
			 , a.EmployeeID
			 , a.EmployeeName
			 , a.JoinDate
			 , a.ResignDate
			 , a.PersonnelStatus
			 , ax02.AssignDate
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
					 order by x.MutationDate desc
		       ) as ax01
		 outer apply (
					select top 1 
					       x.AssignDate
						 , x.Department
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
					 order by x.AssignDate desc
		       ) as ax02
		 where a.CompanyCode like @CompanyCode
	)
	select *
	  into #t_employee
	  from x;

	select a.DealerCode
	     , a.DealerName 
		 , InvalidBranch = (
				select count(x.EmployeeID)
				  from #t_employee x
				 where x.CompanyCode = a.DealerCode
					and  ( 
							x.BranchCode is null
							or
							x.BranchCode = ''
							or 
							len(x.BranchCode) < 7
						  )
		   )
		 , InvalidPosition = (
				select count(x.EmployeeID)
				  from #t_employee x
				 where x.CompanyCode = a.DealerCode
				   and (
							x.Department is null
							or 
							x.Position is null
							or 
							x.Department = ''
							or
							x.Position = ''
					   )
		   )
		 , InvalidStatus = (
				select count(x.EmployeeID)
				  from #t_employee x
				 where x.CompanyCode = a.DealerCode
				   and (
							x.PersonnelStatus is null
							or
							x.PersonnelStatus = ''
				       )
		   )
		 , InvalidResign = (
				select count(x.EmployeeID)
				  from #t_employee x
				 where x.CompanyCode = a.DealerCode
				   and (
							Year(x.ResignDate) <= 1900
							or
							x.ResignDate > getDate()
							or
							(
								x.PersonnelStatus = '1'
								and
								x.ResignDate is not null	
							)
							or
							(
								(
									x.PersonnelStatus = '3'
									or
									x.PersonnelStatus = '4'
								)
								and
								x.ResignDate is null
							)
				       )
		   )
	  from DealerInfo a
	 where a.DealerCode like @CompanyCode
	   and a.ProductType = '4W'
	 order by a.DealerName asc;
end




go
exec uspfn_MpDataConsolidation ''