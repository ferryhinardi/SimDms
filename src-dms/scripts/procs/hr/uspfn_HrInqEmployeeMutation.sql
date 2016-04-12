
go
if object_id('uspfn_HrInqEmployeeMutation') is not null
	drop procedure uspfn_HrInqEmployeeMutation

go
create procedure uspfn_HrInqEmployeeMutation
	@CompanyCode varchar(10),
	@EmployeeID varchar(10)
as

select a.MutationDate, a.BranchCode, a.IsJoinDate
     , Branch = b.CompanyName + ' (' + rtrim(a.BranchCode) + ') '
  from HrEmployeeMutation a
  left join GnMstCoProfile b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
 where a.CompanyCode = @CompanyCode
   and a.EmployeeID = @EmployeeID 
 order by a.MutationDate 
   
    