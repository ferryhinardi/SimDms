select * from HrEmployeeMutation

insert into HrEmployeeMutation
select distinct
	b.CompanyCode, 
	b.EmployeeID,
	b.MutationDate,
	b.BranchCode,
	0,
	'System',
	getDate(),
	'System',
	getDate()
from 
	gnMstEmployeeMutation b



select * from gnMstEmployeeMutation
select * from HrEmployeeMutation
