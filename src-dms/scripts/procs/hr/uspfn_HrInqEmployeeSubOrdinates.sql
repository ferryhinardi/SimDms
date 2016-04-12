
GO
if object_id('uspfn_HrInqEmployeeSubOrdinates') is not null
	drop PROCEDURE uspfn_HrInqEmployeeSubOrdinates

go
create procedure uspfn_HrInqEmployeeSubOrdinates  
--declare  
 @CompanyCode varchar(20),  
 @EmployeeID varchar(20),
 @Status varchar(10)
as  
 
--select @CompanyCode = '6006406', @EmployeeID = '341'
select a.CompanyCode, a.EmployeeID, a.EmployeeName
     , a.JoinDate
     , LastPosition = upper(a.Department)
         + upper(case isnull(b.PosName, '') when '' then '' else ' - ' + b.PosName end)
         + upper(case isnull(c.LookUpValueName, '') when '' then '' else ' - ' + c.LookUpValueName end)
		 , Status = d.LookUpValueName
  from HrEmployee a
  left join GnMstPosition b
    on b.CompanyCode = a.CompanyCode
   and b.DeptCode = a.Department
   and b.PosCode = a.Position
  left join GnMstLookUpDtl c
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'ITSG'
   and c.LookUpValue = a.Grade
   left join GnMstLookUpDtl d
   on a.CompanyCode = d.CompanyCode
   and d.CodeID='PERS' and d.LookUpValue=a.PersonnelStatus
 where a.CompanyCode = @CompanyCode
   and a.TeamLeader = @EmployeeID 
   and a.PersonnelStatus = case when @status='' then a.PersonnelStatus else @status end
 order by a.EmployeeID
   

     