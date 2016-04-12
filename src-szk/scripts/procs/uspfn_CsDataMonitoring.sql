alter procedure uspfn_CsDataMonitoring
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
select a.CompanyCode as DealerCode, count(a.CustomerCode) as DataCount
  from CsTDayCall a
 where convert(date, a.CreatedDate) between @DateInit and @DateReff
 group by a.CompanyCode

insert into @t_data1 (DealerCode, CsBirthday)
select a.CompanyCode as DealerCode, count(a.CustomerCode) as DataCount
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
 group by a.CompanyCode

insert into @t_data1 (DealerCode, CsCustBpkb)
select a.CompanyCode as DealerCode, count(a.CustomerCode) as DataCount
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
 group by a.CompanyCode

insert into @t_data1 (DealerCode, CsStnkExt)
select a.CompanyCode as DealerCode, count(a.CustomerCode) as DataCount
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
 group by a.CompanyCode


insert into @t_data2 (DealerCode, DateInput, C3DaysCall)
select a.CompanyCode as DealerCode
     , convert(date, a.CreatedDate) as DateInput
     , count(a.CustomerCode) as DataCount
  from CsTDayCall a
 where convert(date, a.CreatedDate) between dateadd(day, -@Interval + 1, @DateReff) and @DateReff
 group by a.CompanyCode, convert(date, a.CreatedDate)

insert into @t_data2 (DealerCode, DateInput, CsBirthday)
select a.CompanyCode as DealerCode
     , convert(date, a.CreatedDate) as DateInput
     , count(a.CustomerCode) as DataCount
  from CsCustBirthDay a
 where convert(date, a.CreatedDate) between dateadd(day, -@Interval + 1, @DateReff) and @DateReff
 group by a.CompanyCode, convert(date, a.CreatedDate)

insert into @t_data2 (DealerCode, DateInput, CsCustBpkb)
select a.CompanyCode as DealerCode
     , convert(date, a.CreatedDate) as DateInput
     , count(a.CustomerCode) as DataCount
  from CsCustBpkb a
 where convert(date, a.CreatedDate) between dateadd(day, -@Interval + 1, @DateReff) and @DateReff
 group by a.CompanyCode, convert(date, a.CreatedDate)

insert into @t_data2 (DealerCode, DateInput, CsStnkExt)
select a.CompanyCode as DealerCode
     , convert(date, a.CreatedDate) as DateInput
     , count(a.CustomerCode) as DataCount
  from CsStnkExt a
 where convert(date, a.CreatedDate) between dateadd(day, -@Interval + 1, @DateReff) and @DateReff
 group by a.CompanyCode, convert(date, a.CreatedDate)


select a.DealerCode, b.DealerName, (b.SeqNo % 10) as SeqNo
     , sum(isnull(C3DaysCall, 0)) C3DaysCall
     , sum(isnull(CsBirthday, 0)) CsBirthday
     , sum(isnull(CsCustBpkb, 0)) CsCustBpkb
     , sum(isnull(CsStnkExt, 0)) CsStnkExt
  from @t_data1 a
  left join DealerInfo b 
    on b.DealerCode = a.DealerCode
 group by a.DealerCode, b.DealerName, b.SeqNo
 order by (b.SeqNo % 10)

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
