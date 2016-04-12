
go
if object_id('uspfn_HrUpdateJoinDateAchievement') is not null
	drop procedure uspfn_HrUpdateJoinDateAchievement

go
create procedure uspfn_HrUpdateJoinDateAchievement
	@CompanyCode varchar(25),
	@EmployeeID varchar(25),
	@JoinDate datetime,
	@ResignDate datetime

as
begin
	declare @Department varchar(35);
	declare @Position varchar(35);
	declare @Grade varchar(35);

	set @Department = ( select top 1 Department from HrEmployee where CompanyCode = @CompanyCode and EmployeeID =  @EmployeeID );
	set @Position = ( select top 1 Position from HrEmployee where CompanyCode = @CompanyCode and EmployeeID =  @EmployeeID );
	set @Grade = ( select top 1 Grade from HrEmployee where CompanyCode = @CompanyCode and EmployeeID =  @EmployeeID );

	begin try
		update HrEmployeeAchievement
		   set AssignDate=@JoinDate
		 where CompanyCode=@CompanyCode
		   and EmployeeID=@EmployeeID
		   and IsJoinDate=1
	end try
	begin catch
		update HrEmployeeAchievement
		   set IsDeleted=0
		     , IsJoinDate=1
		 where CompanyCode=@CompanyCode
		   and EmployeeID=@EmployeeID
		   and convert(datetime, AssignDate)=@JoinDate
	end catch

	update HrEmployeeAchievement
	   set IsDeleted=1
	 where CompanyCode=@CompanyCode
	   and EmployeeID=@EmployeeID
	   and convert(datetime,AssignDate) < @JoinDate

	update HrEmployeeAchievement
	   set IsDeleted=1
	 where CompanyCode=@CompanyCode
	   and EmployeeID=@EmployeeID
	   and convert(datetime, AssignDate) > @ResignDate

	update HrEmployee
	   set Department = isnull((
				select top 1
				       a.Department
				  from HrEmployeeAchievement a
				 where a.CompanyCode=@CompanyCode
				   and a.EmployeeID=@EmployeeID
			       and ( a.IsDeleted = 0 or a.IsDeleted is null)
				 order by a.AssignDate desc
		   ), @Department)
		 , Position = isnull((
				select top 1
				       a.Position
				  from HrEmployeeAchievement a
				 where a.CompanyCode=@CompanyCode
				   and a.EmployeeID=@EmployeeID
			       and ( a.IsDeleted = 0 or a.IsDeleted is null)
				 order by a.AssignDate desc
		   ), @Position)
		 , Grade = isnull((
				select top 1
				       a.Grade
				  from HrEmployeeAchievement a
				 where a.CompanyCode=@CompanyCode
				   and a.EmployeeID=@EmployeeID
			       and ( a.IsDeleted = 0 or a.IsDeleted is null)
				 order by a.AssignDate desc
		   ),@Grade)
     where CompanyCode=@CompanyCode
	   and EmployeeID=@EmployeeID;
end

