begin transaction

--;with x as (
--select a.CompanyCode, a.EmployeeID, a.EmployeeName, a.JoinDate, a.ResignDate, a.PersonnelStatus
--  from HrEmployee a
-- where 1 = 1
--   and a.PersonnelStatus = '1'
--   and a.ResignDate is not null
--   --and a.JoinDate != a.ResignDate
--)
--select * from x

--;with x as (
--select a.CompanyCode, a.EmployeeID, a.EmployeeName, a.JoinDate, a.ResignDate, a.PersonnelStatus
--  from HrEmployee a
-- where 1 = 1
--   and a.PersonnelStatus = '1'
--   and a.ResignDate is not null
--   and a.JoinDate = a.ResignDate
--)
--select * from x

---- Cari yang resign ada comment
--;with x as (
--select a.CompanyCode, a.EmployeeID, a.EmployeeName, a.JoinDate, a.ResignDate, a.PersonnelStatus, a.ResignDescription
--  from HrEmployee a
-- where 1 = 1
--   and a.PersonnelStatus = '1'
--   and a.ResignDate is not null
--   and isnull(a.ResignDescription, '') != '' 
--)
--update x set PersonnelStatus = '3'

--;with x as (
--select a.CompanyCode, a.EmployeeID, a.EmployeeName, a.JoinDate, a.ResignDate, a.PersonnelStatus, a.ResignDescription
--  from HrEmployee a
-- where 1 = 1
--   and a.PersonnelStatus = '1'
--   and a.ResignDate is not null
--   and a.ResignDate < a.JoinDate
--   and isnull(a.ResignDescription, '') = '' 
--)
----select * from x
--update x set ResignDate = null 

--;with x as (
--select a.CompanyCode, a.EmployeeID, a.EmployeeName, a.JoinDate, a.ResignDate, a.PersonnelStatus, a.ResignDescription
--  from HrEmployee a
-- where 1 = 1
--   and a.PersonnelStatus = '1'
--   and a.ResignDate is not null
--   and a.ResignDate > a.JoinDate
--   and isnull(a.ResignDescription, '') = '' 
--)
----select * from x
--update x set ResignDate = null

rollback transaction


