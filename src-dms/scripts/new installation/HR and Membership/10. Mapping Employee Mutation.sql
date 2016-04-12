--select top 1 *
--  from gnMstEmployeeMutation

--select top 1 *
--  from HrEmployeeMutation


insert into HrEmployeeMutation
select a.CompanyCode
     , a.EmployeeID
	 , a.MutationDate
	 , a.MutationTo
	 , 0
	 , 'system'
	 , getdate()
	 , 'system'
	 , getdate()
	 , 0
  from gnMstEmployeeMutation a