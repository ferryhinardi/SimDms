
if object_id('uspfn_InqSalesmanTraining') is not null
	drop procedure uspfn_InqSalesmanTraining;



go
create procedure uspfn_InqSalesmanTraining
	@GroupArea varchar(10),
	@CompanyCode varchar(25),
	@BranchCode varchar(25),
	@ByDate datetime
as
begin
	--if @GroupArea = '' or @GroupArea is null
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
				and a.Position = 'S'
				order by x.AssignDate desc
		   )
		 , Position = (
			 select top 1 x.Position
	           from HrEmployeeAchievement x 
		      where a.CompanyCode = x.CompanyCode
			    and a.EmployeeID = x.EmployeeID
				and a.Department = 'SALES'
				and a.Position = 'S'
				order by x.AssignDate desc
		   )
		 , Grade = (
			 select top 1 x.Grade
	           from HrEmployeeAchievement x 
		      where a.CompanyCode = x.CompanyCode
			    and a.EmployeeID = x.EmployeeID
				and a.Department = 'SALES'
				and a.Position = 'S'
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
	   and a.Position = 'S'


	select c.AreaDealer as Area
	     , a.CompanyGovName as CompanyName
	     , a.CompanyName as BranchName
		 , Trainee = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '1'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
		   )
		 , Silver = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '2'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
		   )
		 , Gold = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '3'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
		   )
		 , Platinum = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '4'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
		   )
		 , TotalSalesman = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade in ( '1', '2', '3', '4' )
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
		   )
		 , GoldTerminated = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade in ( '3' )
				   and x.PersonnelStatus in ( '3', '4' )
				   and x.JoinDate <= @ByDate
		   )
		 , PlatinumTerminated = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade in ( '4' )
				   and x.PersonnelStatus in ( '3', '4' )
				   and x.JoinDate <= @ByDate
		   )
		 , STDP1 = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '1'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
				   and x.Training = 'STDP1'
		   )
		 , STDP2 = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '1'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
				   and x.Training = 'STDP2'
		   )
		 , STDP3 = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '1'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
				   and x.Training = 'STDP3'
		   )
		 , STDP4 = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '1'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
				   and x.Training = 'STDP4'
		   )
		 , STDP5 = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '1'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
				   and x.Training = 'STDP5'
		   )
		 , STDP6 = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '1'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
				   and x.Training = 'STDP6'
		   )
		 , STDP7 = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '1'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
				   and x.Training = 'STDP7'
		   )
		 , TotalSTDP = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '1'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
				   and x.Training in ('STDP1', 'STDP2', 'STDP3', 'STDP4', 'STDP5', 'STDP6', 'STDP7')
		   )
		 , SPSSilver = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '2'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
				   and x.Training = 'SPSS'
		   )
		 , SPSGOld = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '3'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
				   and x.Training = 'SPSG'
		   )
		 , SPSPlatinum = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade = '4'
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
				   and x.Training = 'SPSP'
		   )
	  from gnMstCoProfile a
	 inner join DealerGroupMapping b
	    on a.CompanyCode = b.DealerCode
	 inner join GroupArea c
	    on c.GroupNo = b.GroupNo
	 where c.GroupNo like @GroupArea
	   and a.CompanyCode like @CompanyCode
	 order by c.AreaDealer asc
	     , a.CompanyGovName asc
	     , a.CompanyName asc;
end



go
exec uspfn_InqSalesmanTraining @GroupArea = '', @CompanyCode = '', @BranchCode = '', @ByDate = '';



