
go
if object_id('uspfn_HrGetTeamLeader') is not null
	drop procedure uspfn_HrGetTeamLeader

go
create procedure uspfn_HrGetTeamLeader
	@CompanyCode varchar(25),
	@DeptCode varchar(25),
	@PosCode varchar(25)
as

declare @table as table(value varchar(200), text varchar(200))
declare @curpos as varchar(200)

set @curpos = isnull((
				select top 1 PosHeader
				  from GnMstPosition
				 where CompanyCode = @CompanyCode
				   and DeptCode = @DeptCode
				   and PosCode = @PosCode
				  ), '') 

while (@curpos != '' and @DeptCode != 'COM')
begin
	insert into @table
	select a.EmployeeID, a.EmployeeID + ' - ' + a.EmployeeName + ' (' + @curpos + ')' 
	  from HrEmployee a
	 where CompanyCode = @CompanyCode
	   and (Department = @DeptCode or Department = 'COM')
	   and Position = @curpos
   
	set @curpos = isnull((
					select top 1 PosHeader
					  from GnMstPosition
					 where CompanyCode = @CompanyCode
					   and (DeptCode = @DeptCode or DeptCode = 'COM')
					   and PosCode = @curpos
					  ), '') 
end

select * from @table
