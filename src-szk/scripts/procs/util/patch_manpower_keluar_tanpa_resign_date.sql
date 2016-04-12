begin transaction

;with x as (
select a.CompanyCode, a.EmployeeID, a.EmployeeName, a.ResignDate, a.PersonnelStatus
  from HrEmployee a
 where 1 = 1
   and a.PersonnelStatus = '3'
   and a.ResignDate is null
)
update x set PersonnelStatus = '2'

rollback transaction


