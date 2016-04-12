begin transaction

;with x as (
select a.CompanyCode, a.EmployeeID, a.EmployeeName, a.JoinDate, a.ResignDate, a.PersonnelStatus
  from HrEmployee a
 where 1 = 1
   and a.PersonnelStatus = '1'
   and a.ResignDate is not null
)
select * from x

;with x as (
select a.CompanyCode, a.EmployeeID, a.EmployeeName, a.JoinDate, a.ResignDate, a.PersonnelStatus
  from HrEmployee a
 where 1 = 1
   and a.PersonnelStatus = '3'
   and a.ResignDate is null
)
select * from x

rollback transaction


