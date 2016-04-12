USE [SAT_]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_HrGetTeamLeader]    Script Date: 04/23/2015 16:58:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[uspfn_HrGetTeamLeader]
	@CompanyCode varchar(10),
	@DeptCode varchar(10),
	@PosCode varchar(10)
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
	select a.EmployeeID, a.EmployeeName + ' (' + @curpos + ')' 
	  from HrEmployee a
	 where CompanyCode = @CompanyCode
	   and (Department = @DeptCode or Department = 'COM')
	   and Position = @curpos
	   and PersonnelStatus = '1'
   
	set @curpos = isnull((
					select top 1 PosHeader
					  from GnMstPosition
					 where CompanyCode = @CompanyCode
					   and (DeptCode = @DeptCode or DeptCode = 'COM')
					   and PosCode = @curpos
					  ), '') 
end

select * from @table


