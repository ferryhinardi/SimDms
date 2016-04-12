select * from HrEmployeeSales

select 
	a.CompanyCode,
	a.EmployeeID,
	a.SalesID,
	'System',
	getDate(),
	
from 
	SyncEmployeeData a
where
	a.SalesID is not null
