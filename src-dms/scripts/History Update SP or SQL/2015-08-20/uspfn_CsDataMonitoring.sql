/****** Object:  StoredProcedure [dbo].[uspfn_CsDataMonitoring]    Script Date: 8/20/2015 2:46:48 PM ******/
IF OBJECT_ID('uspfn_CsDataMonitoring') IS NOT NULL
	DROP PROC dbo.uspfn_CsDataMonitoring
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_CsDataMonitoring '2015-01-01', '2015-03-01', 3
CREATE procedure [dbo].[uspfn_CsDataMonitoring]
	@DateInit date,
	@DateReff date,
	@Interval int 

as

declare @t_data1 as table (
	DealerCode varchar(20),
	C3DaysCall int,
	CsBirthday   int,
	CsCustBpkb int,
	CsStnkExt  int
)

declare @t_data2 as table (
	DealerCode varchar(20),
	DateInput  date,
	C3DaysCall int,
	CsBirthday   int,
	CsCustBpkb int,
	CsStnkExt  int
)

insert into @t_data1 (DealerCode, C3DaysCall)
	 select b.BranchCode as DealerCode, count(a.CustomerCode) as DataCount
	   from CsTDayCall a
  left join
			(
			select distinct CompanyCode, BranchCode, Chassis, CustomerCode
			from CsCustomerVehicleView
			) b
		 on a.CompanyCode	= b.CompanyCode
		and a.CustomerCode	= b.CustomerCode
		and a.Chassis		= b.Chassis
	  where convert(date, a.CreatedDate) between @DateInit and @DateReff
	  group by b.BranchCode

insert into @t_data1 (DealerCode, CsBirthday)
select BranchCode as DealerCode, count(a.CustomerCode) as DataCount
  from CsCustBirthDay a
 --where convert(date, a.CreatedDate) between @DateInit and dateadd(day, -@Interval, @DateReff)
  join CsCustomerView b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.CustomerType = 'I'
   and b.BirthDate is not null
   and b.BirthDate > '1900-01-01'
   and (year(getdate() - year(b.BirthDate))) > 5
 where convert(date, a.CreatedDate) between @DateInit and @DateReff
 group by a.CompanyCode, b.BranchCode

insert into @t_data1 (DealerCode, CsCustBpkb)
select b.BranchCode as DealerCode, count(a.CustomerCode) as DataCount
  from CsCustBpkb a
 --where convert(date, a.CreatedDate) between @DateInit and dateadd(day, -@Interval, @DateReff)
  join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.Chassis = a.Chassis
  join CsCustomerView c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
 where convert(date, a.CreatedDate) between @DateInit and @DateReff
 group by a.CompanyCode, b.BranchCode

insert into @t_data1 (DealerCode, CsStnkExt)
select b.BranchCode as DealerCode, count(a.CustomerCode) as DataCount
  from CsStnkExt a
  join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.Chassis = a.Chassis
  join CsCustomerView c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
 --where convert(date, a.CreatedDate) between @DateInit and dateadd(day, -@Interval, @DateReff)
 where convert(date, a.CreatedDate) between @DateInit and @DateReff
 group by a.CompanyCode, b.BranchCode


insert into @t_data2 (DealerCode, DateInput, C3DaysCall)
	 select b.BranchCode as DealerCode
		  , convert(date, a.CreatedDate) as DateInput
		  , count(a.CustomerCode) as DataCount
	   from CsTDayCall a
  left join
			(
			select distinct CompanyCode, BranchCode, Chassis, CustomerCode
			from CsCustomerVehicleView
			) b
		on a.CompanyCode	= b.CompanyCode
	   and a.CustomerCode	= b.CustomerCode
	   and a.Chassis		= b.Chassis
	 where convert(date, a.CreatedDate) between dateadd(day, -@Interval + 1, @DateReff) and @DateReff
	 group by b.BranchCode, convert(date, a.CreatedDate)

insert into @t_data2 (DealerCode, DateInput, CsBirthday)
	select b.BranchCode as DealerCode
		 , convert(date, a.CreatedDate) as DateInput
		 , count(a.CustomerCode) as DataCount
	  from CsCustBirthDay a
 left join
			(
			select distinct CompanyCode, BranchCode, CustomerCode
			from CsCustomerView
			) b
		on a.CompanyCode	= b.CompanyCode
	   and a.CustomerCode	= b.CustomerCode
	 where convert(date, a.CreatedDate) between dateadd(day, -@Interval + 1, @DateReff) and @DateReff
	 group by b.BranchCode, convert(date, a.CreatedDate)

insert into @t_data2 (DealerCode, DateInput, CsCustBpkb)
	select b.BranchCode as DealerCode
		 , convert(date, a.CreatedDate) as DateInput
		 , count(a.CustomerCode) as DataCount
	  from CsCustBpkb a
 left join
			(
			select distinct CompanyCode, BranchCode, Chassis, CustomerCode
			from CsCustomerVehicleView
			) b
		on a.CompanyCode	= b.CompanyCode
	   and a.CustomerCode	= b.CustomerCode
	   and a.Chassis		= b.Chassis
	 where convert(date, a.CreatedDate) between dateadd(day, -@Interval + 1, @DateReff) and @DateReff
	 group by b.BranchCode, convert(date, a.CreatedDate)

insert into @t_data2 (DealerCode, DateInput, CsStnkExt)
	select b.BranchCode as DealerCode
		 , convert(date, a.CreatedDate) as DateInput
		 , count(a.CustomerCode) as DataCount
	  from CsStnkExt a
 left join
			(
			select distinct CompanyCode, BranchCode, Chassis, CustomerCode
			from CsCustomerVehicleView
			) b
		on a.CompanyCode	= b.CompanyCode
	   and a.CustomerCode	= b.CustomerCode
	   and a.Chassis		= b.Chassis
	 where convert(date, a.CreatedDate) between dateadd(day, -@Interval + 1, @DateReff) and @DateReff
	 group by b.BranchCode, convert(date, a.CreatedDate)


select OutletCode DealerCode, OutletAbbreviation DealerName --, (b.SeqNo % 10) as SeqNo
     , sum(isnull(C3DaysCall, 0)) C3DaysCall
     , sum(isnull(CsBirthday, 0)) CsBirthday
     , sum(isnull(CsCustBpkb, 0)) CsCustBpkb
     , sum(isnull(CsStnkExt, 0)) CsStnkExt
  from @t_data1 a
  left join gnmstdealeroutletmapping b 
    on b.OutletCode = a.DealerCode
 group by OutletCode, OutletAbbreviation
 order by OutletAbbreviation

select DealerCode
     , convert(char(10), DateInput, 120) DateInput
     , sum(isnull(C3DaysCall, 0)) C3DaysCall
     , sum(isnull(CsBirthday, 0)) CsBirthday
     , sum(isnull(CsCustBpkb, 0)) CsCustBpkb
     , sum(isnull(CsStnkExt, 0)) CsStnkExt
  from @t_data2
 group by DealerCode, DateInput
 order by DealerCode, DateInput

--go

--exec uspfn_CsDataMonitoring '2014-01-01', '2014-06-13', 7

--select @Inquiry = 'Birthday', @DateInit = '2014-01-01', @DateReff = '2014-06-11', @Interval = 1

--select * from CsCustBpkb

--select * from CsStnkExt where CompanyCode = '6419401'
