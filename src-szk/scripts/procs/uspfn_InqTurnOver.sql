
if object_id('uspfn_InqTurnOver') is not null
	drop procedure uspfn_InqTurnOver;



go
create procedure uspfn_InqTurnOver
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
		Grade varchar(5),
		PersonnelStatus char(1),
		JoinDate datetime
	);

	insert into @Grades (
		   CompanyCode
		 , BranchCode
		 , EmployeeID
		 , Grade
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
		 , GradeDetail = (
			 select top 1 x.Grade
	           from HrEmployeeAchievement x 
		      where a.CompanyCode = x.CompanyCode
			    and a.EmployeeID = x.EmployeeID
				and a.Department = 'SALES'
				and a.Position = 'S'
				order by x.AssignDate desc
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

	select a.CompanyGovName as CompanyName
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
		 , Total = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade in ( '1', '2', '3', '4' )
				   and x.PersonnelStatus = '1'
				   and x.JoinDate <= @ByDate
		   )
		 , TraineeTerminated = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade in ( '1' )
				   and x.PersonnelStatus in ( '3', '4' )
				   and x.JoinDate <= @ByDate
		   )
		 , SilverTerminated = (
				select count(x.EmployeeID)
				  from @Grades x
				 where x.CompanyCode = a.CompanyCode
				   and x.BranchCode = a.BranchCode
				   and x.Grade in ( '2' )
				   and x.PersonnelStatus in ( '3', '4' )
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
	  from gnMstCoProfile a
	 inner join DealerGroupMapping b
	    on a.CompanyCode = b.DealerCode
     inner join GroupArea c
	    on c.GroupNo = b.GroupNo
	 where b.GroupNo like @GroupArea
	   and a.CompanyCode like @CompanyCode
	   and a.BranchCode like @BranchCode
	 order by a.CompanyGovName asc
	     , a.CompanyName asc;
end



go
exec uspfn_InqTurnOver @GroupArea = '105', @CompanyCode = '', @BranchCode = '', @ByDate = null;



