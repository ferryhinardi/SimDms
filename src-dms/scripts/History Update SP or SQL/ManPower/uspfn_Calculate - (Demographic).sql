IF(OBJECT_ID('uspfn_CalculateAge') is not null)
	drop proc dbo.uspfn_CalculateAge
GO

/****** Object:  StoredProcedure [dbo].[uspfn_CalculateAge]    Script Date: 9/1/2015 4:44:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* uspfn_CalculateAge '', '', '', '2010-01-01', '2015-07-07', 0, 17 */
create proc [dbo].[uspfn_CalculateAge]
@CompanyCode varchar(20),
@BranchCode varchar(15),
@Position varchar(15),
@PeriodFrom datetime,
@AgeRange1 int,
@AgeRange2 int
as
begin
select count(EmployeeID) as value1 from
(
	select distinct e.CompanyCode, e.EmployeeID
	from HrEmployee e
	  join HrEmployeeMutation m
	on e.CompanyCode = m.CompanyCode and e.EmployeeID = m.EmployeeID
	where 1=1
	  and isnuLL(m.IsDeleted, 0) = 0
	  and datediff(yy, BirthDate, GETDATE()) +
			case when convert(int, convert(varchar, Month(BirthDate)) + convert(varchar, Day(BirthDate))) 
				>= convert(int, convert(varchar, Month(getdate())) + convert(varchar, Day(getdate()))) 
			then 1
			else 0
		  end between @AgeRange1 and @AgeRange2
	  and e.CompanyCode = case when @CompanyCode = '' then e.CompanyCode else @CompanyCode end
	  and m.BranchCode = case when @BranchCode = '' then m.BranchCode else @BranchCode end
	  and e.Position = case when @Position = '' then e.Position else @Position end
	  and m.MutationDate <= @PeriodFrom
	  --and datediff(yy, BirthDate, GETDATE()) >= 17
	--and CompanyCode = @CompanyCode
) a

end
GO

IF(OBJECT_ID('uspfn_CalculateEducation') is not null)
	drop proc dbo.uspfn_CalculateEducation
GO
/****** Object:  StoredProcedure [dbo].[uspfn_CalculateEducation]    Script Date: 9/1/2015 4:44:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*****************************************************************************/
/*****************************************************************************/
/*****************************************************************************/
--select * from INFORMATION_SCHEMA.columns where column_name like '%education%'

--select distinct LookUpValue, LookUpValueName from GnMstLookUpDtl h 
--   where 1=1
--   and h.CodeID like '%FEDU%' 
--   and h.LookUpValue = 0
/* uspfn_CalculateEducation '', '', '', '2010-01-01', '2015-07-07', ('5', '6', '7', 'D1', 'D2', 'D3') */
create proc [dbo].[uspfn_CalculateEducation]
@CompanyCode varchar(20),
@BranchCode varchar(15),
@Position varchar(15),
@PeriodFrom datetime,
@Education varchar(90)
as
begin

select count(EmployeeID) as value1 from
(
	select distinct e.CompanyCode, e.EmployeeID
	from HrEmployee e
	  join HrEmployeeMutation m
	on e.CompanyCode = m.CompanyCode and e.EmployeeID = m.EmployeeID
	where 1=1
	  and isnuLL(m.IsDeleted, 0) = 0
	  and e.FormalEducation in (SELECT Item FROM dbo.SplitStrings(@Education, ','))
	  and e.CompanyCode = case when @CompanyCode = '' then e.CompanyCode else @CompanyCode end
	  and m.BranchCode = case when @BranchCode = '' then m.BranchCode else @BranchCode end
	  and e.Position = case when @Position = '' then e.Position else @Position end
	  and m.MutationDate <= @PeriodFrom
) a

end
GO

IF(OBJECT_ID('uspfn_CalculateGender') is not null)
	drop proc dbo.uspfn_CalculateGender
GO

/********************************************************************************************************/
/********************************************************************************************************/
/********************************************************************************************************/
/****** Object:  StoredProcedure [dbo].[uspfn_CalculateGender]    Script Date: 9/1/2015 4:44:27 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* [uspfn_CalculateGender] '', '', '', '2010-01-01', '2015-07-07' */
create proc [dbo].[uspfn_CalculateGender]
@CompanyCode varchar(20),
@BranchCode varchar(15),
@Position varchar(15),
@PeriodFrom datetime
as
begin

;with x as (
select count(EmployeeID) as value1 from
	(
		select distinct e.CompanyCode, e.EmployeeID
		from HrEmployee e
		  join HrEmployeeMutation m
		on e.CompanyCode = m.CompanyCode and e.EmployeeID = m.EmployeeID
		where 1=1
		  and isnuLL(m.IsDeleted, 0) = 0
		  and e.Gender = 'L'
		  and e.CompanyCode = case when @CompanyCode = '' then e.CompanyCode else @CompanyCode end
		  and m.BranchCode = case when @BranchCode = '' then m.BranchCode else @BranchCode end
		  and e.Position = case when @Position = '' then e.Position else @Position end
		  and m.MutationDate <= @PeriodFrom
	) a
) select * from x,
	(
		select count(EmployeeID) as value2 from
		(
			select distinct e.CompanyCode, e.EmployeeID
			from HrEmployee e
			  join HrEmployeeMutation m
			on e.CompanyCode = m.CompanyCode and e.EmployeeID = m.EmployeeID
			where 1=1
			  and isnuLL(m.IsDeleted, 0) = 0
			  and e.Gender = 'P'
			  and e.CompanyCode = case when @CompanyCode = '' then e.CompanyCode else @CompanyCode end
			  and m.BranchCode = case when @BranchCode = '' then m.BranchCode else @BranchCode end
			  and e.Position = case when @Position = '' then e.Position else @Position end
			  and m.MutationDate <= @PeriodFrom
		) a
	) b
end
GO

IF(OBJECT_ID('uspfn_CalculateWorkingPeriod') is not null)
	drop proc dbo.uspfn_CalculateWorkingPeriod
GO

/***************************************************************************************************************/
/***************************************************************************************************************/
/***************************************************************************************************************/
/****** Object:  StoredProcedure [dbo].[uspfn_CalculateWorkingPeriod]    Script Date: 9/1/2015 4:44:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* uspfn_CalculateWorkingPeriod '', 12, 24, 1 */
create proc [dbo].[uspfn_CalculateWorkingPeriod]
@CompanyCode varchar(20),
@BranchCode varchar(15),
@Position varchar(15),
@PeriodFrom datetime,
@MonthRange1 int,
@MonthRange2 int,
@InifiniteLast bit
as
begin

	declare @query varchar(max)
	set @query = ''

if (@InifiniteLast = 1)
begin

/*contoh : > 60 bulan*/	
select count(EmployeeID) as value1 from
(
	select distinct e.CompanyCode, e.EmployeeID
	from HrEmployee e
	  join HrEmployeeMutation m
	on e.CompanyCode = m.CompanyCode and e.EmployeeID = m.EmployeeID
	where 1=1
	  and isnuLL(m.IsDeleted, 0) = 0
	  and
		  (
		    (
				convert(varchar, (dateadd(m, @MonthRange1, MutationDate)), 112) <= convert(varchar, isnull(ResignDate, dateadd(m, 1, GETDATE())), 112)
			)
			or
			(
				convert(varchar, (dateadd(m, @MonthRange1, MutationDate)), 112) <=
				(
					select top 1 m2.MutationDate from HrEmployeeMutation m2
					where m2.CompanyCode = m.CompanyCode and m2.EmployeeID = m.EmployeeID
					  and m2.MutationDate > m.MutationDate
					order by m2.MutationDate
				)
			)
		  )
	and e.CompanyCode = case when @CompanyCode = '' then e.CompanyCode else @CompanyCode end
	and m.BranchCode = case when @BranchCode = '' then m.BranchCode else @BranchCode end
	and e.Position = case when @Position = '' then e.Position else @Position end
	and m.MutationDate <= @PeriodFrom
) a

end
else
begin

--set @query += '
select count(EmployeeID) as value1 from
(
	select distinct e.CompanyCode, e.EmployeeID
	from HrEmployee e
	  join HrEmployeeMutation m
	on e.CompanyCode = m.CompanyCode and e.EmployeeID = m.EmployeeID
	where 1=1
	  and isnuLL(m.IsDeleted, 0) = 0
	  and
		  (
		    (
				convert(varchar, (dateadd(m, @MonthRange2, MutationDate)), 112) > convert(varchar, isnull(ResignDate, dateadd(m, 1, GETDATE())), 112)
				and
				convert(varchar, (dateadd(m, @MonthRange1, MutationDate)), 112) <= convert(varchar, isnull(ResignDate, dateadd(m, 1, GETDATE())), 112)
			)
--if (@BranchCode != '')
--begin
--set @query +=
			or
			(
				convert(varchar, (dateadd(m, @MonthRange2, MutationDate)), 112) > 
				(
					select top 1 m2.MutationDate from HrEmployeeMutation m2
					where m2.CompanyCode = m.CompanyCode and m2.EmployeeID = m.EmployeeID
					  and m2.MutationDate > m.MutationDate
					order by m2.MutationDate
				)
				and
				convert(varchar, (dateadd(m, @MonthRange1, MutationDate)), 112) <=
				(
					select top 1 m2.MutationDate from HrEmployeeMutation m2
					where m2.CompanyCode = m.CompanyCode and m2.EmployeeID = m.EmployeeID
					  and m2.MutationDate > m.MutationDate
					order by m2.MutationDate
				)
			)
--end

--set @query += '
		  )
	and e.CompanyCode = case when @CompanyCode = '' then e.CompanyCode else @CompanyCode end
	and m.BranchCode = case when @BranchCode = '' then m.BranchCode else @BranchCode end
	and e.Position = case when @Position = '' then e.Position else @Position end
	and m.MutationDate <= @PeriodFrom
) a

--exec(@query)
/*
data tidak valid : resign date lebi kecil dari join date sxpun di table mutation
memastikan tidak ada data aneh seperti tanggal mutasi berikut nya lebi kecil
*/
end

end
GO
