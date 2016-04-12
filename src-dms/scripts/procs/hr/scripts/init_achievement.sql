begin transaction

insert into HrEmployeeAchievement 
select top 0 * from HrEmployeeAchievement where IsJoinDate = 1
 union all
select CompanyCode, EmployeeID, JoinDate, Department, Position, Grade, 1, 'init', getdate(), 'init', getdate()
  from HrEmployee a
 where JoinDate is not null
   and isnull(Department, '') != ''
   and isnull(Position, '') != ''
   and Grade = (case when (Position = 'S' and isnull(Grade, '') = '') then '1' else Grade end)
   and not exists (
	select 1 from HrEmployeeAchievement
	 where CompanyCode = a.CompanyCode
	   and EmployeeID = a.EmployeeID
	   and convert(varchar, AssignDate, 112) = convert(varchar, a.JoinDate, 112)
   ) 
  order by EmployeeID

select * from HrEmployeeAchievement where IsJoinDate = 1
  
rollback transaction

