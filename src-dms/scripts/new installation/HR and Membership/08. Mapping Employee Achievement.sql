--select top 1 * from SfEmployeeTitleHistory
--select top 1 * from HrEmployeeAchievement

insert into HrEmployeeAchievement
select 
       a.CompanyCode
	 , a.EmployeeID
	 , a.AssignedDate
	 , a.HistoryDeptCode
	 , a.HistoryPosCode
	 , ''
	 , 0
	 , 'system'
	 , getdate()
	 , 'system'
	 , getdate()
	 , 0
  from SfEmployeeTitleHistory a