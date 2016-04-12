
if object_id('uspfn_InqSCSHBMTraining') is not null
	drop procedure uspfn_InqSCSHBMTraining;



go
create procedure uspfn_InqSCSHBMTraining
	@GroupArea varchar(17),
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@ByDate datetime
as
begin
	--if @GroupArea is null or @GroupArea = ''
	--	set @GroupArea = '%';

	--if @CompanyCode = '' or @CompanyCode is null
	--	set @CompanyCode = '%';
	
	if @BranchCode='' or @BranchCode is null
		set @BranchCode = '%';

	if @ByDate is null or @ByDate = ''
		set @ByDate = GetDate();

	declare @Grades table(
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
		   CompanyCode
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
	select a.CompanyCode
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
	 where a.CompanyCode like @CompanyCode
	   and ( select top 1 x.BranchCode
	           from HrEmployeeMutation x 
		      where a.CompanyCode = x.CompanyCode
			    and a.EmployeeID = x.EmployeeID
				order by x.MutationDate desc
		   ) like @BranchCode
	   and a.Department = 'SALES'

	select c.AreaDealer as Area
	     , a.CompanyGovName as CompanyName
	     , a.CompanyName as BranchName
		 , SC = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Department = 'SALES'
				   and x.Position = 'SC'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
		   )
		 , SH = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Department = 'SALES'
				   and x.Position = 'SH'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
		   )
		 , BM = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Department = 'SALES'
				   and x.Position = 'BM'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
		   )
		 , SCBasic = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Department = 'SALES'
				   and x.Position = 'BM'
				   and x.PersonnelStatus = '1'
				   and x.Training = 'SCB'
				   and x.JoinDate <= @ByDate
		   )
		 , SCAdvance = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Department = 'SALES'
				   and x.Position = 'BM'
				   and x.PersonnelStatus = '1'
				   and x.Training = 'SCA'
				   and x.JoinDate <= @ByDate
		   )
		 , SHBasic = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Department = 'SALES'
				   and x.Position = 'BM'
				   and x.PersonnelStatus = '1'
				   and x.Training = 'SHB'
				   and x.JoinDate <= @ByDate
		   )
		 , SHIntermediate = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Department = 'SALES'
				   and x.Position = 'BM'
				   and x.PersonnelStatus = '1'
				   and x.Training = 'SHI'
				   and x.JoinDate <= @ByDate
		   )
		 , SHAdvance = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Department = 'SALES'
				   and x.Position = 'BM'
				   and x.PersonnelStatus = '1'
				   and x.Training = 'SHA'
				   and x.JoinDate <= @ByDate
		   )
		 , BMBasic = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Department = 'SALES'
				   and x.Position = 'BM'
				   and x.PersonnelStatus = '1'
				   and x.Training = 'BMB'
				   and x.JoinDate <= @ByDate
		   )
		 , BMIntermediate = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Department = 'SALES'
				   and x.Position = 'BM'
				   and x.PersonnelStatus = '1'
				   and x.Training = 'BMI'
				   and x.JoinDate <= @ByDate
		   )
		 , BMAdvance = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Department = 'SALES'
				   and x.Position = 'BM'
				   and x.PersonnelStatus = '1'
				   and x.Training = 'BMA'
				   and x.JoinDate <= @ByDate
		   )
	  from gnMstCoProfile a
	 inner join DealerGroupMapping b
	    on b.DealerCode = a.CompanyCode
	 inner join GroupArea c
	    on c.GroupNo = b.GroupNo
	 where c.GroupNo like @GroupArea
	   and a.CompanyCode like @CompanyCode 
	   and a.BranchCode like @BranchCode
	 order by c.AreaDealer asc 
	     , a.CompanyGovName asc
	     , a.CompanyName asc;
end



go
exec uspfn_InqSCSHBMTraining @GroupArea = '', @CompanyCode = '', @BranchCode = '', @ByDate = '';



