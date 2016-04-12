
go
if object_id('uspfn_mpPersInfoExport') is not null
	drop procedure uspfn_mpPersInfoExport

go
create procedure uspfn_mpPersInfoExport
	@CompanyCode varchar (20),
	@BranchCode varchar (20) = '',
	@Position   varchar (20) = '',
	@PersonnelStatus varchar (20) = '',
	@EmployeeName varchar (20) = ''

as

select *
  from ViewHrInqPersonalInformation a
 where a.CompanyCode like @CompanyCode
   and a.BranchCode like @BranchCode
   and a.PositionCode like @Position
   and a.PersonnelStatusCode like @PersonnelStatus
   and a.EmployeeName like @EmployeeName

go
exec uspfn_mpPersInfoExport '6006406', '%', '%', '%', '%'

-- 1 : Active
-- 2 : Non Active
-- 3 : Keluar






go
if object_id('uspfn_mpPersInfo') is not null
	drop procedure uspfn_mpPersInfo

go
create procedure uspfn_mpPersInfo
	@CompanyCode varchar (20),
	@BranchCode varchar (20) = '',
	@Position   varchar (20) = '',
	@PersonnelStatus varchar (20) = '',
	@EmployeeName varchar (20) = ''

as

declare @t_pstatus as table (Code varchar(10), Name varchar(50))

insert into @t_pstatus values ('1', 'ACTIVE')
insert into @t_pstatus values ('2', 'NON ACTIVE')
insert into @t_pstatus values ('3', 'KELUAR')
insert into @t_pstatus values ('4', 'PENSIUN')

select a.CompanyCode
	 , BranchCode = isnull((select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by MutationDate desc), '')
     , a.EmployeeID
     , a.EmployeeName
	 , a.JoinDate
	 , a.Handphone1
	 , a.MaritalStatus
	 , a.PersonnelStatus
	 , b.Name as PersonnelStatusDesc
	 , PositionCheck = isnull((select top 1 Position from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by AssignDate desc), '')
	 , JoinDateCheck = isnull((select top 1 MutationDate from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and IsJoinDate = 1 order by MutationDate), '')
	 , HistAchievement = (select count(*) from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID)
	 , HistMutation = (select count(*) from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID)
  from HrEmployee a
  left join @t_pstatus b
    on b.Code = a.PersonnelStatus
 where 1 = 1
   and a.Department = 'SALES'
   and a.CompanyCode = @CompanyCode
   and a.Position = (case when isnull(@Position, '') = '' then a.Position else @Position end)
   and a.PersonnelStatus = (case when isnull(@PersonnelStatus, '') = '' then a.PersonnelStatus else @PersonnelStatus end)