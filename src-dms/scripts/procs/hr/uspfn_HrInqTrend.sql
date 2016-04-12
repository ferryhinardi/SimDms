
go
if object_id('uspfn_HrInqTrend') is not null
	drop procedure uspfn_HrInqTrend

go
create procedure uspfn_HrInqTrend
	@CompanyCode varchar(20),
	@DeptCode varchar(20),
	@Position varchar(20) = '',
	@Year varchar(20)
as

declare @t_mutation as table(
	CompanyCode  varchar(20),
	BranchCode   varchar(20),
	EmployeeID   varchar(20),
	MutationDate datetime,
	ResignDate   datetime
)

declare @t_result as table(
	BranchCode varchar(20),
	BranchName varchar(200),
	Month01    int,
	Month02    int,
	Month03    int,
	Month04    int,
	Month05    int,
	Month06    int,
	Month07    int,
	Month08    int,
	Month09    int,
	Month10    int,
	Month11    int,
	Month12    int
)

insert into @t_mutation
select a.CompanyCode, a.BranchCode, a.EmployeeID, a.MutationDate, isnull(b.ResignDate, '01/01/2100')
  from HrEmployeeMutation a
 inner join HrEmployee b
    on b.CompanyCode = a.CompanyCode
   and b.EmployeeID = a.EmployeeID 
   and b.PersonnelStatus in ('1', '3', '4')
 where a.CompanyCode = @CompanyCode
   and b.Department = (case @DeptCode when '' then b.Department else @DeptCode end)
   and b.Position = (case @Position when '' then b.Position else @Position end)

insert into @t_result
select a.BranchCode, a.BranchCode + ' - ' + a.CompanyName as BranchName
     , Month01 = isnull((      
        select count(distinct EmployeeID) from @t_mutation x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @Year + '01' + '99'
           and convert(varchar, x.ResignDate, 112) > @Year + '01' + '99'  
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @Year + '01' + '99'  
				   and convert(varchar, ResignDate, 112) > @Year + '01' + '99'  
				 order by MutationDate desc)
       ), 0)  
     , Month02 = isnull((      
        select count(distinct EmployeeID) from @t_mutation x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @Year + '02' + '99'
           and convert(varchar, x.ResignDate, 112) > @Year + '02' + '99'  
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @Year + '02' + '99'  
				   and convert(varchar, ResignDate, 112) > @Year + '02' + '99'  
				 order by MutationDate desc)
       ), 0)      
     , Month03 = isnull((      
        select count(distinct EmployeeID) from @t_mutation x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @Year + '03' + '99'
           and convert(varchar, x.ResignDate, 112) > @Year + '03' + '99'  
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @Year + '03' + '99'  
				   and convert(varchar, ResignDate, 112) > @Year + '03' + '99'  
				 order by MutationDate desc)
       ), 0)      
     , Month04 = isnull((      
        select count(distinct EmployeeID) from @t_mutation x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @Year + '04' + '99'
           and convert(varchar, x.ResignDate, 112) > @Year + '04' + '99'  
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @Year + '04' + '99'  
				   and convert(varchar, ResignDate, 112) > @Year + '04' + '99'  
				 order by MutationDate desc)
       ), 0)      
     , Month05 = isnull((      
        select count(distinct EmployeeID) from @t_mutation x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @Year + '05' + '99'
           and convert(varchar, x.ResignDate, 112) > @Year + '05' + '99'  
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @Year + '05' + '99'  
				   and convert(varchar, ResignDate, 112) > @Year + '05' + '99'  
				 order by MutationDate desc)
       ), 0)      
     , Month06 = isnull((      
        select count(distinct EmployeeID) from @t_mutation x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @Year + '06' + '99'
           and convert(varchar, x.ResignDate, 112) > @Year + '06' + '99'  
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @Year + '06' + '99'  
				   and convert(varchar, ResignDate, 112) > @Year + '06' + '99'  
				 order by MutationDate desc)
       ), 0)      
     , Month07 = isnull((      
        select count(distinct EmployeeID) from @t_mutation x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @Year + '07' + '99'
           and convert(varchar, x.ResignDate, 112) > @Year + '07' + '99'  
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @Year + '07' + '99'  
				   and convert(varchar, ResignDate, 112) > @Year + '07' + '99'  
				 order by MutationDate desc)
       ), 0)      
     , Month08 = isnull((      
        select count(distinct EmployeeID) from @t_mutation x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @Year + '08' + '99'
           and convert(varchar, x.ResignDate, 112) > @Year + '08' + '99'  
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @Year + '08' + '99'  
				   and convert(varchar, ResignDate, 112) > @Year + '08' + '99'  
				 order by MutationDate desc)
       ), 0)      
     , Month09 = isnull((      
        select count(distinct EmployeeID) from @t_mutation x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @Year + '09' + '99'
           and convert(varchar, x.ResignDate, 112) > @Year + '09' + '99'  
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @Year + '09' + '99'  
				   and convert(varchar, ResignDate, 112) > @Year + '09' + '99'  
				 order by MutationDate desc)
       ), 0)      
     , Month10 = isnull((      
        select count(distinct EmployeeID) from @t_mutation x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @Year + '10' + '99'
           and convert(varchar, x.ResignDate, 112) > @Year + '10' + '99'  
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @Year + '10' + '99'  
				   and convert(varchar, ResignDate, 112) > @Year + '10' + '99'  
				 order by MutationDate desc)
       ), 0)      
     , Month11 = isnull((      
        select count(distinct EmployeeID) from @t_mutation x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @Year + '11' + '99'
           and convert(varchar, x.ResignDate, 112) > @Year + '11' + '99'  
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @Year + '11' + '99'  
				   and convert(varchar, ResignDate, 112) > @Year + '11' + '99'  
				 order by MutationDate desc)
       ), 0)      
     , Month12 = isnull((      
        select count(distinct EmployeeID) from @t_mutation x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @Year + '12' + '99'
           and convert(varchar, x.ResignDate, 112) > @Year + '12' + '99'  
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @Year + '12' + '99'  
				   and convert(varchar, ResignDate, 112) > @Year + '12' + '99'  
				 order by MutationDate desc)
       ), 0)      
  from gnMstCoProfile a
 where a.CompanyCode = @CompanyCode

 if @Year = year(getdate())
 begin
	declare @month int 
	set @month = month(getdate())

	if @month = 1 
		update @t_result set Month02 = null, Month03 = null, Month04 = null, Month05 = null, Month06 = null, Month07 = null, Month08 = null, Month09 = null, Month10 = null, Month11 = null, Month12 = null
	if @month = 2 
		update @t_result set Month03 = null, Month04 = null, Month05 = null, Month06 = null, Month07 = null, Month08 = null, Month09 = null, Month10 = null, Month11 = null, Month12 = null
	if @month = 3 
		update @t_result set Month04 = null, Month05 = null, Month06 = null, Month07 = null, Month08 = null, Month09 = null, Month10 = null, Month11 = null, Month12 = null
	if @month = 4 
		update @t_result set Month05 = null, Month06 = null, Month07 = null, Month08 = null, Month09 = null, Month10 = null, Month11 = null, Month12 = null
	if @month = 5 
		update @t_result set Month06 = null, Month07 = null, Month08 = null, Month09 = null, Month10 = null, Month11 = null, Month12 = null
	if @month = 6 
		update @t_result set Month07 = null, Month08 = null, Month09 = null, Month10 = null, Month11 = null, Month12 = null
	if @month = 7 
		update @t_result set Month08 = null, Month09 = null, Month10 = null, Month11 = null, Month12 = null
	if @month = 8 
		update @t_result set Month09 = null, Month10 = null, Month11 = null, Month12 = null
	if @month = 9 
		update @t_result set Month10 = null, Month11 = null, Month12 = null
	if @month = 10 
		update @t_result set Month11 = null, Month12 = null
	if @month = 11
		update @t_result set Month12 = null
 end

 select * from @t_result
