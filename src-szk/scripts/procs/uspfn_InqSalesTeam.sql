go
if object_id('uspfn_InqSalesTeam') is not null
	drop procedure uspfn_InqSalesTeam

go
create procedure uspfn_InqSalesTeam
	@GroupArea varchar(10),
	@CompanyCode varchar(17),
	@BranchCode varchar(17)

as
begin
	--if @GroupArea is null or @GroupArea = ''
	--	set @GroupArea = '%';

	--if @CompanyCode is null or @CompanyCode = ''
	--	set @CompanyCode = '%';

	if @BranchCode is null or @BranchCode = ''
		set @BranchCode = '%';
			
	select d.AreaDealer as Area
		 , e.CompanyGovName as CompanyName
		 , e.CompanyName as BranchName
		 , a.EmployeeID 
		 , a.EmployeeName
		 , c.PosLevel
		 , Department = (
				select top 1
				       x.Department
				  from HrEmployeeAchievement x
				 where x.CompanyCode = a.CompanyCode
				   and x.EmployeeID = a.EmployeeID
				 order by x.AssignDate desc
		   )
		 --, Position = (
			--	select top 1
			--	       x.Position
			--	  from HrEmployeeAchievement x
			--	 where x.CompanyCode = a.CompanyCode
			--	   and x.EmployeeID = a.EmployeeID
			--	 order by x.AssignDate desc
		 --  )
		 , PositionName = (
				select top 1
				       y.PosName
				  from HrEmployeeAchievement x
				 inner join HrMstPosition y
				    on y.DeptCode = x.Department
				   and y.PosCode = x.Position
				 where x.CompanyCode = a.CompanyCode
				   and x.EmployeeID = a.EmployeeID
				 order by x.AssignDate desc
		   )
		 --, Grade = (
			--	select top 1
			--	       x.Grade
			--	  from HrEmployeeAchievement x
			--	 where x.CompanyCode = a.CompanyCode
			--	   and x.EmployeeID = a.EmployeeID
			--	 order by x.AssignDate desc
		 --  )
		 , GradeName = (
				select top 1
				       y.LookupValueName
				  from gnMstLookUpDtl y
				 where y.CodeID = 'ITSG'
				   and y.LookupValue = (
						select top 1
						       x.Grade
						  from HrEmployeeAchievement x
						 where x.CompanyCode = a.CompanyCode
						   and x.EmployeeID = a.EmployeeID
						 order by x.AssignDate desc
					   )
		   )
		 , LeaderID = (
				select top 1
				       x.EmployeeID
				  from HrEmployee x
				 where x.CompanyCode = a.CompanyCode
				   and x.EmployeeID = a.TeamLeader
				   and x.PersonnelStatus = '1'
				   and (
							select top 1 
								   y.BranchCode
							  from HrEmployeeMutation y
							 where y.CompanyCode = x.CompanyCode
							   and y.EmployeeID = x.EmployeeID
							 order by y.MutationDate desc
					   ) = (
							select top 1 
								   y.BranchCode
							  from HrEmployeeMutation y
							 where y.CompanyCode = a.CompanyCode
							   and y.EmployeeID = a.EmployeeID
							 order by y.MutationDate desc
					   )	 
		   )
		 , LeaderName = (
				select top 1
				       x.EmployeeName
				  from HrEmployee x
				 where x.CompanyCode = a.CompanyCode
				   and x.EmployeeID = a.TeamLeader
				   and x.PersonnelStatus = '1'
				   and (
							select top 1 
								   y.BranchCode
							  from HrEmployeeMutation y
							 where y.CompanyCode = x.CompanyCode
							   and y.EmployeeID = x.EmployeeID
							 order by y.MutationDate desc
					   ) = (
							select top 1 
								   y.BranchCode
							  from HrEmployeeMutation y
							 where y.CompanyCode = a.CompanyCode
							   and y.EmployeeID = a.EmployeeID
							 order by y.MutationDate desc
					   )
		   )
 	  from HrEmployee a
	 inner join DealerGroupMapping b
	    on a.CompanyCode = b.DealerCode
	 inner join HrMstPosition c
	    on c.DeptCode = (
				select top 1
				       x.Department
				  from HrEmployeeAchievement x
				 where x.CompanyCode = a.CompanyCode
				   and x.EmployeeID = a.EmployeeID
				 order by x.AssignDate desc
		   )
	   and c.PosCode = (
				select top 1
				       x.Position
				  from HrEmployeeAchievement x
				 where x.CompanyCode = a.CompanyCode
				   and x.EmployeeID = a.EmployeeID
				 order by x.AssignDate desc
		   )
	 inner join GroupArea d
	    on d.GroupNo = b.GroupNo
	  left join gnMstCoProfile e
	    on e.CompanyCode = a.CompanyCode
	   and e.BranchCode = (
				select top 1 
				       x.BranchCode
				  from HrEmployeeMutation x
				 where x.CompanyCode = a.CompanyCode
				   and x.EmployeeID = a.EmployeeID
				 order by x.MutationDate desc
		   )
	 where b.GroupNo like @GroupArea
	   and a.CompanyCode like @CompanyCode
	   and (
				select top 1 
				       x.BranchCode
				  from HrEmployeeMutation x
				 where x.CompanyCode = a.CompanyCode
				   and x.EmployeeID = a.EmployeeID
				 order by x.MutationDate desc
		   ) like @BranchCode
	   and (
				select top 1
				       x.Department
				  from HrEmployeeAchievement x
				 where x.CompanyCode = a.CompanyCode
				   and x.EmployeeID = a.EmployeeID
				 order by x.AssignDate desc
		   ) = 'SALES'
	   and a.PersonnelStatus = '1'
	 order by d.AreaDealer asc 
	     , e.CompanyGovName asc
	     , e.CompanyName asc
	     , Department asc
	     , c.PosLevel desc
		 , LeaderName asc
		 , a.EmployeeName asc
end





go
exec uspfn_InqSalesTeam @GroupArea='100', @CompanyCode='6006406', @BranchCode=''