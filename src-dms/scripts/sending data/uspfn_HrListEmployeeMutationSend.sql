
go
if object_id('uspfn_HrListEmployeeMutationSend') is not null
	drop procedure uspfn_HrListEmployeeMutationSend

go
create procedure uspfn_HrListEmployeeMutationSend 
	@LastUpdateDate datetime,
	@Segment int

--select @LastUpdateDate='1990-01-01 00:00:00',@Segment=500
as

select * into #t1 from (
select top (@Segment) CompanyCode
	 , EmployeeID, MutationDate, BranchCode, IsJoinDate
	 , CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
  from HrEmployeeMutation
 where UpdatedDate is not null
   and UpdatedDate > @LastUpdateDate
 order by UpdatedDate asc )#t1
 
declare @LastUpdateQry datetime
    set @LastUpdateQry = (select top 1 UpdatedDate from #t1 order by UpdatedDate desc)

select * from #t1
 union
select CompanyCode
	 , EmployeeID, MutationDate, BranchCode, IsJoinDate
	 , CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
  from HrEmployeeMutation
 where UpdatedDate = @LastUpdateQry
 
  drop table #t1
