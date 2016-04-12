begin transaction

insert into HrEmployeeMutation
select top 0 * from HrEmployeeMutation where IsJoinDate = 1
 union all
select CompanyCode, EmployeeID, JoinDate, BranchCode, 1, 'init', getdate(), 'init', getdate()
  from GnMstEmployee a
 where JoinDate is not null
   and EmployeeID not in (select EmployeeID from GnMstEmployee group by EmployeeID having count(*) > 1)
   and not exists (
	select * from HrEmployeeMutation 
	 where CompanyCode = a.CompanyCode
	   and EmployeeID = a.EmployeeID
	   and convert(varchar, MutationDate, 112) = convert(varchar, a.JoinDate, 112) 
   )

select * from HrEmployeeMutation where IsJoinDate = 1
  
rollback transaction

