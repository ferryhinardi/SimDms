declare
	@CompanyCode varchar(20),
	@DeptCode varchar(20),
	@PosCode varchar(20),
	@Status varchar(20),
	@BranchCode varchar(20)

select  @CompanyCode=N'6006406',@DeptCode=N'SALES',@PosCode=N'S',@Status=N'1',@BranchCode=N'6006402'

;with p as (
select a.CompanyCode, a.EmployeeID, b.SalesID, a.EmployeeName
     , BranchCode = isnull((select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by MutationDate desc), '')
     , DeptCode = isnull((select top 1 Department from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by AssignDate desc), '')
     , PosCode = isnull((select top 1 Position from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by AssignDate desc), '')
     , Grade = isnull((select top 1 Grade from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by AssignDate desc), '')
	 , a.PersonnelStatus
  from HrEmployee a
  left join HrEmployeeSales b
    on b.CompanyCode = a.CompanyCode
   and b.EmployeeID = a.EmployeeID
 where a.CompanyCode = @CompanyCode
   and a.Department = @DeptCode
   --and a.PersonnelStatus = (case @Status when '' then a.PersonnelStatus else @Status end)
)
select p.EmployeeID, p.SalesID, p.EmployeeName
     , p.BranchCode
	 , q.CompanyName as BranchName
     , Position = upper(p.DeptCode)
         + upper(case isnull(c.PosName, '') when '' then '' else ' - ' + c.PosName end)
         + upper(case isnull(d.LookUpValueName, '') when '' then '' else ' - ' + d.LookUpValueName end)
     , Status = (case isnull(p.PersonnelStatus, '')
		when '1' then 'Aktif'
		when '2' then 'Non Aktif'
		when '3' then 'Keluar'
		when '4' then 'Pensiun'
		else 'No Status' end)
  from p
  left join gnMstCoProfile q
    on q.CompanyCode = p.CompanyCode
   and q.BranchCode = p.BranchCode
  left join GnMstPosition c
    on c.CompanyCode = p.CompanyCode
   and c.DeptCode = p.DeptCode
   and c.PosCode = p.PosCode
  left join GnMstLookUpDtl d
    on d.CompanyCode = p.CompanyCode
   and d.CodeID = 'ITSG'
   and d.LookUpValue = p.Grade
 where 1 = 1
   --and p.DeptCode = @DeptCode
   --and p.PosCode = (case @PosCode when '' then p.PosCode else @PosCode end)
   --and p.BranchCode = (case @BranchCode when '' then p.BranchCode else @BranchCode end)
   and p.EmployeeID = '51843'
 order by BranchCode, EmployeeID


 
;with x as (
select a.EmployeeID
     , e.SalesID
     , a.EmployeeName
     , BranchCode = isnull((select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID order by MutationDate desc), '')
     , BranchName = isnull((
		select top 1 y.CompanyName
		  from HrEmployeeMutation x
		  left join GnMstCoProfile y
		    on y.CompanyCode = x.CompanyCode
		   and y.BranchCode = x.BranchCode
		 where x.CompanyCode = a.CompanyCode
		   and x.EmployeeID = a.EmployeeID
		 order by x.MutationDate desc), '')
     , Position = upper(a.Department)
         + upper(case isnull(c.PosName, '') when '' then '' else ' - ' + c.PosName end)
         + upper(case isnull(d.LookUpValueName, '') when '' then '' else ' - ' + d.LookUpValueName end)
     , Status = (case isnull(a.PersonnelStatus, '')
		when '1' then 'Aktif'
		when '2' then 'Non Aktif'
		when '3' then 'Keluar'
		when '4' then 'Pensiun'
		else 'No Status' end)
  from HrEmployee a
  left join GnMstPosition c
    on c.CompanyCode = a.CompanyCode
   and c.DeptCode = a.Department
   and c.PosCode = a.Position
  left join GnMstLookUpDtl d
    on d.CompanyCode = a.CompanyCode
   and d.CodeID = 'ITSG'
   and d.LookUpValue = a.Grade
  left join HrEmployeeSales e
    on e.CompanyCode = a.CompanyCode
   and e.EmployeeID = a.EmployeeID
 where a.CompanyCode = @CompanyCode
   and a.Department = @DeptCode
   and a.Position = (case @PosCode when '' then a.Position else @PosCode end)
   and a.PersonnelStatus = (case @Status when '' then a.PersonnelStatus else @Status end)
)
select * from x where isnull(x.BranchCode, '') = (case @BranchCode when '' then x.BranchCode else @BranchCode end)
