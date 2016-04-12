alter procedure uspfn_HrInqMutation
--declare
	@CompanyCode varchar(20),
	@DeptCode varchar(20),
	@Position varchar(20) = '',
	@MutaDate varchar(20)
as

--select @CompanyCode=N'6006408',@DeptCode=N'SALES',@MutaDate=N'20130822'

declare @table1 as table(
	CompanyCode  varchar(20),
	BranchCode   varchar(20),
	EmployeeID   varchar(20),
	MutationDate datetime,
	ResignDate   datetime,
	IsJoinDate   bit
)

declare @table2 as table(
	CompanyCode varchar(20),
	BranchCode  varchar(20),
	BranchName  varchar(420),
	Muta01      int,
	Muta02      int,
	Muta03      int,
	Muta04      int,
	Muta05      int,
	Muta06      int
)

set @MutaDate = replace(@MutaDate, '-', '')

insert into @table1
select a.CompanyCode
     , a.BranchCode
	 , a.EmployeeID
     , a.MutationDate
	 , isnull(b.ResignDate, '01/01/2100')
     , a.IsJoinDate
  from HrEmployeeMutation a
 inner join HrEmployee b
    on b.CompanyCode = a.CompanyCode
   and b.EmployeeID = a.EmployeeID 
 where a.CompanyCode = @CompanyCode
   and b.Department = (case @DeptCode when '' then b.Department else @DeptCode end)
   and b.Position = (case @Position when '' then b.Position else @Position end)
   and b.PersonnelStatus != '2'

insert into @table2
select a.CompanyCode, a.BranchCode
     , a.BranchCode + ' - ' + a.CompanyName as BranchName
     , Muta01 = isnull((      
        select count(distinct EmployeeID) from @table1 x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) < left(@MutaDate, 6) + '01'  
           and convert(varchar, x.ResignDate, 112) >= left(@MutaDate, 6) + '01' 
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) < left(@MutaDate, 6) + '01'
				   and convert(varchar, ResignDate, 112) >= left(@MutaDate, 6) + '01' 
				 order by MutationDate desc)
    --       and not exists (
				--select top 1 EmployeeID from HrEmployeeMutation
				-- where CompanyCode = x.CompanyCode
				--   and BranchCode != x.BranchCode   
				--   and EmployeeID = x.EmployeeID
				--   and convert(varchar, MutationDate, 112) < left(@MutaDate, 6) + '01'
				--   and convert(varchar, ResignDate, 112) >= left(@MutaDate, 6) + '01' 
				--   and IsJoinDate != 1
				-- order by MutationDate desc)
       ), 0)      
     , Muta02 = isnull((      
        select count(distinct EmployeeID) from @table1 x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) >= left(@MutaDate, 6) + '01'  
           and convert(varchar, x.MutationDate, 112) <= @MutaDate
           and convert(varchar, x.ResignDate, 112) >= left(@MutaDate, 6) + '01' 
           and x.IsJoinDate = 1
       ), 0)      
     , Muta03 = isnull((      
        select count(distinct EmployeeID) from @table1 x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) >= left(@MutaDate, 6) + '01'  
           and convert(varchar, x.MutationDate, 112) <= @MutaDate
           and convert(varchar, x.ResignDate, 112) >= left(@MutaDate, 6) + '01' 
           and x.IsJoinDate = 0
       ), 0)      
     , Muta04 = isnull((      
        select count(distinct EmployeeID) from @table1 x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.ResignDate, 112) >= left(@MutaDate, 6) + '01'  
           and convert(varchar, x.ResignDate, 112) <= @MutaDate
       ), 0)      
     , Muta05 = 0  -- will be added later
     , Muta06 = isnull((      
        select count(distinct EmployeeID) from @table1 x
         where x.CompanyCode = a.CompanyCode      
           and x.BranchCode = a.BranchCode      
           and convert(varchar, x.MutationDate, 112) <= @MutaDate
           and convert(varchar, x.ResignDate, 112) > @MutaDate
		   and x.BranchCode = (
				select top 1 BranchCode from HrEmployeeMutation
				 where CompanyCode = x.CompanyCode
				   and EmployeeID = x.EmployeeID
				   and convert(varchar, MutationDate, 112) <= @MutaDate
				   and convert(varchar, ResignDate, 112) > @MutaDate
				 order by MutationDate desc)
    --       and not exists (
				--select top 1 EmployeeID from HrEmployeeMutation
				-- where CompanyCode = x.CompanyCode
				--   and BranchCode != x.BranchCode   
				--   and EmployeeID = x.EmployeeID
				--   and convert(varchar, MutationDate, 112) <= @MutaDate
				--   and convert(varchar, ResignDate, 112) > @MutaDate
				--   and IsJoinDate != 1
				-- order by MutationDate desc)
       ), 0)      
  from gnMstCoProfile a
 where a.CompanyCode = @CompanyCode

update @table2 set Muta05 = Muta01 + Muta02 + Muta03 - Muta04 - Muta06

select * from @table2