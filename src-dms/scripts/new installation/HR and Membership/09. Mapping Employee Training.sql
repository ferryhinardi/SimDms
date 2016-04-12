
--select top 1 * from SfEmployeeTraining
--select top 1 * from HrEmployeeTraining

insert into HrEmployeeTraining
select 
	   a.CompanyCode
	 , a.EmployeeID
	 , a.TrnCode
	 , a.TrnDate
	 , a.TrnSeq
	 , 0
	 , a.PreTest
	 , ''
	 , a.PostTest
	 , ''
	 , 'system'
	 , getdate()
	 , 'system'
	 , getdate()
	 , 0
  from SfEmployeeTraining a

