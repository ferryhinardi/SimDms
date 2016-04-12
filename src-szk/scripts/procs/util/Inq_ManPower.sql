;with x as (
select a.EmployeeID, a.EmployeeName, a.Department, a.Position, a.PersonnelStatus, a.JoinDate
     , BranchCode = (
			select top 1 BranchCode
			  from HrEmployeeMutation
			 where CompanyCode = a.CompanyCode
			   and EmployeeID = a.EmployeeID
			 order by MutationDate desc
		)
  from HrEmployee a
 where a.CompanyCode = '6006406'
   and a.Department = 'SALES'
   and a.Position = 'S'
   and a.PersonnelStatus = '1'
   and exists (select 1 from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID)
)
select * from x
 where BranchCode = '6006406'
 order by EmployeeID




--select * from HrEmployeeMutation

