
go
if object_id('uspfn_HrGetMutationByEmployeeId') is not null
	drop procedure uspfn_HrGetMutationByEmployeeId

go
create procedure uspfn_HrGetMutationByEmployeeId
	@CompanyCode varchar(25),
	@EmployeeID varchar(25)    
as  
  
select a.EmployeeID
     , MutationDate = convert(varchar(12),a.MutationDate,106)
     , a.BranchCode
     , b.BranchCode+ ' - ' + b.CompanyName as BranchName
     , case when(a.IsJoinDate = 1) then 'Join Date' else '-'  end as MutationInfo
  from HrEmployeeMutation a
  left join gnMstCoProfile b
    on a.CompanyCode = b.CompanyCode
   and a.BranchCode = b.BranchCode  
 where a.CompanyCode = @CompanyCode
   and a.EmployeeID =  @EmployeeID    
   and a.IsDeleted != 1
 order by a.MutationDate asc 