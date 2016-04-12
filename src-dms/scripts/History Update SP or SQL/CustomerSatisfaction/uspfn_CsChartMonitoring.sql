IF(OBJECT_ID('uspfn_CsChartMonitoring') is not null)
	drop proc dbo.uspfn_CsChartMonitoring
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_CsChartMonitoring 'CsCustBpkb', '2009-08-01', '2015-08-19'
CREATE procedure [dbo].[uspfn_CsChartMonitoring]
	@Inquiry  varchar(50),
	@DateFrom datetime,
	@DateTo   datetime 

as

declare @t_data as table (
	InputDate  datetime,
	DealerCode varchar(20),
	DataCount  int
)

if @Inquiry = '3DaysCall'
begin
	;with x as
	(
		select convert(datetime, a.CreatedDate) as InputedDate
			 , b.BranchCode
			 , count(a.Chassis) as DataCount
		  from CsTDayCall a
	 left join 
			 (
			   select distinct CompanyCode, BranchCode, Chassis, CustomerCode
			   from CsCustomerVehicleView
			 ) b
			on a.CompanyCode	= b.CompanyCode
		   and a.CustomerCode	= b.CustomerCode
		   and a.Chassis		= b.Chassis
		 where 1 = 1
		   and convert(datetime, a.CreatedDate) between @DateFrom and @DateTo
		 group by convert(datetime, a.CreatedDate), b.BranchCode
	)
	insert into @t_data
	select * from x
end

if @Inquiry = 'Birthday'
begin
	insert into @t_data
	select convert(datetime, a.CreatedDate) as InputedDate
		 , a.BranchCode
		 , count(a.CustomerCode) as DataCount
	  from
	  (
		  select b.CreatedDate
			 , (select top 1 BranchCode from CsCustomerView where CompanyCode = b.CompanyCode and CustomerCode = b.CustomerCode) BranchCode
			 , b.CustomerCode
		  from CsCustBirthDay b
	  ) a
	 where 1 = 1
	   and convert(datetime, CreatedDate) between @DateFrom and @DateTo
	 group by convert(datetime, a.CreatedDate), BranchCode
end


if @Inquiry = 'CsCustBpkb'
begin
	insert into @t_data
	select convert(datetime, a.CreatedDate) as InputedDate
		 , b.BranchCode
		 , count(a.Chassis) as DataCount
	  from CsCustBpkb a
 left join
			(
			select distinct CompanyCode, BranchCode, Chassis, CustomerCode
			from CsCustomerVehicleView
			) b
		on a.CompanyCode	= b.CompanyCode
	   and a.CustomerCode	= b.CustomerCode
	   and a.Chassis		= b.Chassis
	 where 1 = 1
	   and convert(datetime, CreatedDate) between @DateFrom and @DateTo
	 group by convert(datetime, a.CreatedDate), b.BranchCode
end

if @Inquiry = 'CsStnkExt'
begin
	insert into @t_data
	select convert(datetime, a.CreatedDate) as InputedDate
		 , b.BranchCode
		 , count(a.Chassis) as DataCount
	  from CsStnkExt a
 left join
			(
			select distinct CompanyCode, BranchCode, Chassis, CustomerCode
			from CsCustomerVehicleView
			) b
		on a.CompanyCode	= b.CompanyCode
	   and a.CustomerCode	= b.CustomerCode
	   and a.Chassis		= b.Chassis
	 where 1 = 1
	   and convert(datetime, CreatedDate) between @DateFrom and @DateTo
	 group by convert(datetime, a.CreatedDate), b.BranchCode
end

select convert(char(10), InputDate, 120) as InputDate
     , DealerCode, DataCount
  from @t_data
select OutletCode DealerCode
     , OutletAbbreviation DealerName
  from gnMstDealerOutletMapping
 where OutletCode in (select DealerCode from @t_data)


