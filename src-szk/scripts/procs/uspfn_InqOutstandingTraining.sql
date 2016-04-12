
if object_id('uspfn_InqOutstandingTraining') is not null
	drop procedure uspfn_InqOutstandingTraining;



go
create procedure uspfn_InqOutstandingTraining
	@GroupArea varchar(25),
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@ByDate datetime
as
begin
	--if @GroupArea is null or @GroupArea = ''
	--	set @GroupArea = '%'

	--if @CompanyCode = '' or @CompanyCode is null
	--	set @CompanyCode = '%';
	
	if @BranchCode='' or @BranchCode is null
		set @BranchCode = '%';

	if @ByDate is null or @ByDate = ''
		set @ByDate = GetDate();

	declare @Grades table(
	    GroupArea varchar(10),
		CompanyCode varchar(10),
		BranchCode varchar(10),
		EmployeeID varchar(17),
		Department varchar(50),
		Position varchar(50),
		Grade varchar(5),
		Training varchar(25),
		TrainingDate datetime,
		PersonnelStatus char(1),
		JoinDate datetime
	);

	insert into @Grades (
	       GroupArea
		 , CompanyCode
		 , BranchCode
		 , EmployeeID
		 , Department
		 , Position
		 , Grade
		 , Training
		 , TrainingDate
		 , PersonnelStatus
		 , JoinDate
         )
	select b.GroupNo
	     , a.CompanyCode
	     , BranchCode = (
			 select top 1 x.BranchCode
	           from HrEmployeeMutation x 
		      where a.CompanyCode = x.CompanyCode
			    and a.EmployeeID = x.EmployeeID
				order by x.MutationDate desc
		   )
	     , a.EmployeeID
		 , Department = (
			 select top 1 x.Department
	           from HrEmployeeAchievement x 
		      where a.CompanyCode = x.CompanyCode
			    and a.EmployeeID = x.EmployeeID
				and a.Department = 'SALES'
				order by x.AssignDate desc
		   )
		 , Position = (
			 select top 1 x.Position
	           from HrEmployeeAchievement x 
		      where a.CompanyCode = x.CompanyCode
			    and a.EmployeeID = x.EmployeeID
				and a.Department = 'SALES'
				order by x.AssignDate desc
		   )
		 , Grade = (
			 select top 1 x.Grade
	           from HrEmployeeAchievement x 
		      where a.CompanyCode = x.CompanyCode
			    and a.EmployeeID = x.EmployeeID
				and a.Department = 'SALES'
				order by x.AssignDate desc
		   )
		 , Training = (
			 select top 1 x.TrainingCode
			   from HrEmployeeTraining x
			  where x.CompanyCode = a.CompanyCode
			    and x.EmployeeID = a.EmployeeID
			  order by x.TrainingDate desc
		   )
		 , TrainingDate  = (
			 select top 1 x.TrainingDate
			   from HrEmployeeTraining x
			  where x.CompanyCode = a.CompanyCode
			    and x.EmployeeID = a.EmployeeID
			  order by x.TrainingDate desc
		   )
		 , a.PersonnelStatus
		 , a.JoinDate
	  from HrEmployee a
	 inner join DealerGroupMapping b
	    on a.CompanyCode = b.DealerCode
	 where a.CompanyCode like @CompanyCode
	   and ( select top 1 x.BranchCode
	           from HrEmployeeMutation x 
		      where a.CompanyCode = x.CompanyCode
			    and a.EmployeeID = x.EmployeeID
				order by x.MutationDate desc
		   ) like @BranchCode
	   and a.Department = 'SALES'

	select a.Department
	     , Position = c.PosName
		 , a.Grade
		 , GradeName = (
			 select top 1
			        x.LookUpValueName
			   from gnMstLookUpDtl x
			  where x.CodeID = 'ITSG'
			    and x.LookUpValue = a.Grade
		   )
		 , a.TrainingCode
		 , b.TrainingName
		 , b.TrainingDescription
		 , ManPower = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.GroupArea like @GroupArea
				   and x.CompanyCode like @CompanyCode
				   and x.BranchCode like @BranchCode
				   and x.Department = a.Department
				   and x.Position = a.Position
				   and x.Grade = a.Grade
		   )
		 , Trained = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.GroupArea like @GroupArea
				   and x.CompanyCode like @CompanyCode
				   and x.BranchCode like @BranchCode
				   and x.Department = a.Department
				   and x.Position = a.Position
				   and x.Grade = a.Grade
				   and x.Training = a.TrainingCode
		   )
		 , NotTrained = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.GroupArea like @GroupArea
				   and x.CompanyCode like @CompanyCode
				   and x.BranchCode like @BranchCode
				   and x.Department = a.Department
				   and x.Position = a.Position
				   and x.Grade = a.Grade
		 ) - (
			select count(x.EmployeeID)
				  from @Grades x
				 where x.GroupArea like @GroupArea
				   and x.CompanyCode like @CompanyCode
				   and x.BranchCode like @BranchCode
				   and x.Department = a.Department
				   and x.Position = a.Position
				   and x.Grade = a.Grade
				   and x.Training = a.TrainingCode
		 )
	  from HrDepartmentTraining a
	 inner join HrMstTraining b
	    on a.TrainingCode = b.TrainingCode
	 inner join HrMstPosition c
	    on c.DeptCode = a.Department
	   and c.PosCode = a.Position
     where a.Department = 'SALES'
	   and a.Position = 'S'
	 order by a.Department asc
	     , a.Position asc
		 , a.Grade asc
		 , a.TrainingCode asc
end



go
exec uspfn_InqOutstandingTraining @GroupArea = '100', @CompanyCode = '6006406', @BranchCode = '', @ByDate = '';



