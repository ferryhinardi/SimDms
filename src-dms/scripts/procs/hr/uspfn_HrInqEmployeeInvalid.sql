
go
if object_id('uspfn_HrInqEmployeeInvalid') is not null
	drop procedure uspfn_HrInqEmployeeInvalid

go
create procedure uspfn_HrInqEmployeeInvalid
	@CompanyCode varchar(10),
	@DeptCode varchar(10),
	@PosCode varchar(10),
	@Status varchar(10)
as

--select @CompanyCode = '6006406', @DeptCode = 'SALES', @PosCode = '', @Status = ''

;with x as (
select a.CompanyCode, a.EmployeeID, a.EmployeeName, a.PersonnelStatus
     , a.TeamLeader
     , a.JoinDate
     , Status = (case isnull(a.PersonnelStatus, '')
		when '1' then 'Aktif'
		when '2' then 'Non Aktif'
		when '3' then 'Keluar'
		when '4' then 'Pensiun'
		else 'No Status' end)
     , MutationTimes = (select count(*) from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID)
     , AchieveTimes =  (select count(*) from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID)
     , LastPosition = upper(a.Department)
         + upper(case isnull(b.PosName, '') when '' then '' else ' - ' + b.PosName end)
         + upper(case isnull(c.LookUpValueName, '') when '' then '' else ' - ' + c.LookUpValueName end)
  from HrEmployee a
  left join GnMstPosition b
    on b.CompanyCode = a.CompanyCode
   and b.DeptCode = a.Department
   and b.PosCode = a.Position
  left join GnMstLookUpDtl c
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'ITSG'
   and c.LookUpValue = a.Grade
  left join HrEmployee d
    on d.CompanyCode = a.CompanyCode
   and d.EmployeeID = a.TeamLeader
 where a.CompanyCode = @CompanyCode
   and a.Department = @DeptCode
   and a.Position = (case @PosCode when '' then a.Position else @PosCode end)
   and a.PersonnelStatus = (case @Status when '' then a.PersonnelStatus else @Status end)
)
select x.EmployeeID, x.EmployeeName, x.LastPosition, x.Status
     , x.MutationTimes, x.AchieveTimes
     , HaveJoined = case x.MutationTimes when 0 then 'N' else 'Y' end
     , HavePosition = case x.AchieveTimes when 0 then 'N' else 'Y' end
  from x
 where MutationTimes = 0 or AchieveTimes = 0
