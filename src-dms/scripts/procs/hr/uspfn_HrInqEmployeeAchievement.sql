
go
if object_id('uspfn_HrInqEmployeeAchievement') is not null
	drop procedure uspfn_HrInqEmployeeAchievement

go
create procedure uspfn_HrInqEmployeeAchievement
	@CompanyCode varchar(10),
	@EmployeeID varchar(10)
as

select a.AssignDate
     , ActivePosition = upper(a.Department)
         + upper(case isnull(b.PosName, '') when '' then '' else ' - ' + b.PosName end)
         + upper(case isnull(c.LookUpValueName, '') when '' then '' else ' - ' + c.LookUpValueName end)
     , a.IsJoinDate
  from HrEmployeeAchievement a
  left join GnMstPosition b
    on b.CompanyCode = a.CompanyCode
   and b.DeptCode = a.Department
   and b.PosCode = a.Position
  left join GnMstLookUpDtl c
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'ITSG'
   and c.LookUpValue = a.Grade
 where a.CompanyCode = @CompanyCode
   and a.EmployeeID = @EmployeeID 
 order by a.AssignDate 
   
    