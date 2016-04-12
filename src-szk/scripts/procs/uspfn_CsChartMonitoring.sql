alter procedure uspfn_CsChartMonitoring
	@Inquiry  varchar(50),
	@DateFrom date,
	@DateTo   date 

as

declare @t_data as table (
	InputDate  date,
	DealerCode varchar(20),
	DataCount  int
)

if @Inquiry = '3DaysCall'
begin
	insert into @t_data
	select convert(date, a.CreatedDate) as InputedDate
		 , a.CompanyCode
		 , count(a.Chassis) as DataCount
	  from CsTDayCall a
	 where 1 = 1
	   and convert(date, CreatedDate) between @DateFrom and @DateTo
	 group by convert(date, a.CreatedDate), a.CompanyCode
end

if @Inquiry = 'Birthday'
begin
	insert into @t_data
	select convert(date, a.CreatedDate) as InputedDate
		 , a.CompanyCode
		 , count(a.CustomerCode) as DataCount
	  from CsCustBirthDay a
	 where 1 = 1
	   and convert(date, CreatedDate) between @DateFrom and @DateTo
	 group by convert(date, a.CreatedDate), a.CompanyCode
end


if @Inquiry = 'CsCustBpkb'
begin
	insert into @t_data
	select convert(date, a.CreatedDate) as InputedDate
		 , a.CompanyCode
		 , count(a.Chassis) as DataCount
	  from CsCustBpkb a
	 where 1 = 1
	   and convert(date, CreatedDate) between @DateFrom and @DateTo
	 group by convert(date, a.CreatedDate), a.CompanyCode
end

if @Inquiry = 'CsStnkExt'
begin
	insert into @t_data
	select convert(date, a.CreatedDate) as InputedDate
		 , a.CompanyCode
		 , count(a.Chassis) as DataCount
	  from CsStnkExt a
	 where 1 = 1
	   and convert(date, CreatedDate) between @DateFrom and @DateTo
	 group by convert(date, a.CreatedDate), a.CompanyCode
end

select convert(char(10), InputDate, 120) as InputDate
     , DealerCode, DataCount
  from @t_data
select DealerCode
     , DealerName
  from DealerInfo
 where DealerCode in (select DealerCode from @t_data)

go

--uspfn_CsChartMonitoring '3DaysCall', '2014-06-01', '2014-06-06'
uspfn_CsChartMonitoring 'CsStnkExt', '2014-04-01', '2014-06-06'


--select * from CsCustBpkb
