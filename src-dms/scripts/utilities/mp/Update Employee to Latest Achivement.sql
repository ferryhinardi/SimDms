
drop table #t_employee;

with x as (
	select a.CompanyCode
	     , a.EmployeeID
	     , a.EmployeeName
	     , Department = (  
				select top 1
				       aa.Department
				  from HrEmployeeAchievement aa
				 where aa.CompanyCode = a.CompanyCode
				   and aa.EmployeeID = a.EmployeeID
				   and ( aa.IsDeleted is null or aa.IsDeleted = 0 )
				 order by aa.AssignDate desc
				  
	       )
	     , Position = (  
				select top 1
				       aa.Position
				  from HrEmployeeAchievement aa
				 where aa.CompanyCode = a.CompanyCode
				   and aa.EmployeeID = a.EmployeeID
				   and ( aa.IsDeleted is null or aa.IsDeleted = 0 )
				 order by aa.AssignDate desc
				  
	       )
	     , Grade = (  
				select top 1
				       aa.Grade
				  from HrEmployeeAchievement aa
				 where aa.CompanyCode = a.CompanyCode
				   and aa.EmployeeID = a.EmployeeID
				   and ( aa.IsDeleted is null or aa.IsDeleted = 0 )
				 order by aa.AssignDate desc
				  
	       )
	  from HrEmployee a
)
select * into #t_employee from x;

--select * from #t_employee;

with x as (
	select *
	  from HrEmployee a
)
update x 
   set Department = ( select top 1 a.Department from #t_employee a where a.CompanyCode = x.CompanyCode and a.EmployeeID = x.EmployeeID)
     , Position = ( select top 1 a.Position from #t_employee a where a.CompanyCode = x.CompanyCode and a.EmployeeID = x.EmployeeID)
     , Grade = ( select top 1 a.Grade from #t_employee a where a.CompanyCode = x.CompanyCode and a.EmployeeID = x.EmployeeID);