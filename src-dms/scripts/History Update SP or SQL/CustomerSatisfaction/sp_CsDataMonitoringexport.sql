============================
-- Author:		fhy
-- Create date: 31122015
-- Description:	CsDataMonitoringexport
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_CsDataMonitoringexport]
	@DateInit datetime,
	@DateReff datetime,
	@Interval int 
AS
BEGIN
declare @t_data1 as table (
	DealerCode varchar(20),
	C3DaysCall int,
	CsBirthday   int,
	CsCustBpkb int,
	CsStnkExt  int
)

declare @t_data2 as table (
	DealerCode varchar(20),
	DateInput  datetime,
	C3DaysCall int,
	CsBirthday   int,
	CsCustBpkb int,
	CsStnkExt  int
)

declare @t_csdatamonitoringrpt as table (
	Tanggal varchar(50),
	DealerCode varchar(20),
	DealerName varchar(100),
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
	  where convert(datetime, a.CreatedDate) between @DateInit and @DateReff
	  group by b.BranchCode

insert into @t_data1 (DealerCode, CsBirthday)
select BranchCode as DealerCode, count(a.CustomerCode) as DataCount
  from CsCustBirthDay a
 --where convert(datetime, a.CreatedDate) between @DateInit and dateadd(day, -@Interval, @DateReff)
  join CsCustomerView b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.CustomerType = 'I'
   and b.BirthDate is not null
   and b.BirthDate > '1900-01-01'
   and (year(getdate() - year(b.BirthDate))) > 5
 where convert(datetime, a.CreatedDate) between @DateInit and @DateReff
 group by a.CompanyCode, b.BranchCode

insert into @t_data1 (DealerCode, CsCustBpkb)
select b.BranchCode as DealerCode, count(a.CustomerCode) as DataCount
  from CsCustBpkb a
 --where convert(datetime, a.CreatedDate) between @DateInit and dateadd(day, -@Interval, @DateReff)
  join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.Chassis = a.Chassis
  join CsCustomerView c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
 where convert(datetime, a.CreatedDate) between @DateInit and @DateReff
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
 --where convert(datetime, a.CreatedDate) between @DateInit and dateadd(day, -@Interval, @DateReff)
 where convert(datetime, a.CreatedDate) between @DateInit and @DateReff
 group by a.CompanyCode, b.BranchCode


insert into @t_data2 (DealerCode, dateInput, C3DaysCall)
	 select b.BranchCode as DealerCode
		  , convert(datetime, a.CreatedDate) as dateInput
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
	 where convert(datetime, a.CreatedDate) between dateadd(day, -@Interval + 1, @DateReff) and @DateReff
	 group by b.BranchCode, convert(datetime, a.CreatedDate)

insert into @t_data2 (DealerCode, dateInput, CsBirthday)
	select b.BranchCode as DealerCode
		 , convert(datetime, a.CreatedDate) as dateInput
		 , count(a.CustomerCode) as DataCount
	  from CsCustBirthDay a
 left join
			(
			select distinct CompanyCode, BranchCode, CustomerCode
			from CsCustomerView
			) b
		on a.CompanyCode	= b.CompanyCode
	   and a.CustomerCode	= b.CustomerCode
	 where convert(datetime, a.CreatedDate) between dateadd(day, -@Interval + 1, @DateReff) and @DateReff
	 group by b.BranchCode, convert(datetime, a.CreatedDate)

insert into @t_data2 (DealerCode, dateInput, CsCustBpkb)
	select b.BranchCode as DealerCode
		 , convert(datetime, a.CreatedDate) as dateInput
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
	 where convert(datetime, a.CreatedDate) between dateadd(day, -@Interval + 1, @DateReff) and @DateReff
	 group by b.BranchCode, convert(datetime, a.CreatedDate)

insert into @t_data2 (DealerCode, dateInput, CsStnkExt)
	select b.BranchCode as DealerCode
		 , convert(datetime, a.CreatedDate) as dateInput
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
	 where convert(datetime, a.CreatedDate) between dateadd(day, -@Interval + 1, @DateReff) and @DateReff
	 group by b.BranchCode, convert(datetime, a.CreatedDate)

 declare @flag int
 set @flag=0

 while (@flag <= @Interval-1)
 begin

insert into @t_csdatamonitoringrpt
select CONVERT(VARCHAR(11),dateadd(day, -@flag, @DateReff),106) as Tanggal, OutletCode DealerCode, OutletAbbreviation DealerName --, (b.SeqNo % 10) as SeqNo
     , sum(isnull(C3DaysCall, 0)) C3DaysCall
     , sum(isnull(CsBirthday, 0)) CsBirthday
     , sum(isnull(CsCustBpkb, 0)) CsCustBpkb
     , sum(isnull(CsStnkExt, 0)) CsStnkExt
  from @t_data1 a
  left join gnmstdealeroutletmapping b 
    on b.OutletCode = a.DealerCode
 group by OutletCode, OutletAbbreviation
 order by OutletAbbreviation


	set @flag = @flag+1
 end 

 select 
	 Tanggal as [Tanggal     ],
	 DealerName as [Nama Outlet             ],
	 C3DaysCall as [C3DaysCall     ],
	 CsBirthday as [CsBirthday     ],
	 CsCustBpkb as [CsCustBpkb     ],
	 CsStnkExt as [CsStnkExt      ]
 from @t_csdatamonitoringrpt


--select DealerCode
--     , convert(char(10), dateInput, 120) dateInput
--     , sum(isnull(C3DaysCall, 0)) C3DaysCall
--     , sum(isnull(CsBirthday, 0)) CsBirthday
--     , sum(isnull(CsCustBpkb, 0)) CsCustBpkb
--     , sum(isnull(CsStnkExt, 0)) CsStnkExt
--  from @t_data2
-- group by DealerCode, dateInput
-- order by DealerCode, dateInput


delete @t_data1
delete @t_data2
delete @t_csdatamonitoringrpt
END

GO


