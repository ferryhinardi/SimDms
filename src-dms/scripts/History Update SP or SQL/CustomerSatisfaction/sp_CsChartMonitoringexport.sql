
-- =============================================
-- Author:		fhy
-- Create date: 30122015
-- Description:	CsMonitoring1Export
-- =============================================
Create PROCEDURE [dbo].[uspfn_CsChartMonitoringexport]
	@Inquiry  varchar(50),
	@DateFrom datetime,
	@DateTo   datetime 
AS
BEGIN
	declare @t_data as table (
	InputDate  datetime,
	DealerCode varchar(20),
	DataCount  int
)

declare @t_CsMonitoringbyperiode as table(
	[Kode Outlet] varchar(45),
	[Nama Outlet                    ] varchar(100),
	[Tanggal ] int,
	[DataCount] int
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

--select convert(char(10), InputDate, 120) as InputDate
--     , DealerCode, DataCount
--  from @t_data

--select OutletCode DealerCode
--     , OutletAbbreviation DealerName
--  from gnMstDealerOutletMapping
-- where OutletCode in (select DealerCode from @t_data)

declare
@inguiryname varchar(50)

if(@Inquiry= '3DaysCall')
begin
	set @inguiryname='3 Days Call'
end
else if (@Inquiry= 'Birthday')
begin
	set @inguiryname='Cs Birthday'
end
else if (@Inquiry= 'CsCustBpkb')
begin
	set @inguiryname='Cs Customer BPKB'
end
else if (@Inquiry= 'CsStnkExt')
begin
	set @inguiryname='Cs STNK Extention'
end

select @inguiryname 'Inquiry'

-- select 
--	 data.DealerCode [Kode Outlet]
--	 , map.OutletAbbreviation [Nama Outlet                    ]
--	 , CONVERT(VARCHAR(11),InputDate,106) as [Tanggal Input]
--	 , sum(DataCount)  [DataCount]
--  from @t_data data
--  left join gnMstDealerOutletMapping map on map.OutletCode=data.dealercode
--  group by data.DealerCode,map.OutletAbbreviation,CONVERT(VARCHAR(11),InputDate,106)

--union All

--select 
--'TOTAL'
--,''
--,''
--,sum(DataCount)
--from @t_data

insert into @t_CsMonitoringbyperiode
 select 
	 data.DealerCode [Kode Outlet]
	 , map.OutletAbbreviation  [Nama Outlet                    ]
	 , DATEPART(day,InputDate) inte
	 , DataCount  [DataCount]
  from @t_data data
  left join gnMstDealerOutletMapping map on map.OutletCode=data.dealercode
  
  select 
	[Nama Outlet                    ]       
	, isnull( [1], 0) [Day 1]
		 , isnull( [2], 0) [Day 2]
		 , isnull( [3], 0) [Day 3]
		 , isnull( [4], 0) [Day 4]
		 , isnull( [5], 0) [Day 5]
		 , isnull( [6], 0) [Day 6]
		 , isnull( [7], 0) [Day 7]
		 , isnull( [8], 0) [Day 8]
		 , isnull( [9], 0) [Day 9]
		 , isnull([10], 0) [Day 10]
		 , isnull([11], 0) [Day 11]
		 , isnull([12], 0) [Day 12]
		 , isnull( [13], 0) [Day 13]
		 , isnull( [14], 0) [Day 14]
		 , isnull( [15], 0) [Day 15]
		 , isnull( [16], 0) [Day 16]
		 , isnull( [17], 0) [Day 17]
		 , isnull( [18], 0) [Day 18]
		 , isnull( [19], 0) [Day 19]
		 , isnull( [20], 0) [Day 20]
		 , isnull( [21], 0) [Day 21]
		 , isnull([22], 0) [Day 22]
		 , isnull([23], 0) [Day 23]
		 , isnull([24], 0) [Day 24]
		 , isnull( [25], 0) [Day 25]
		 , isnull( [26], 0) [Day 26]
		 , isnull([27], 0) [Day 27]
		 , isnull([28], 0) [Day 28]
		 , isnull([29], 0) [Day 29]
		 , isnull([30], 0) [Day 30]
		 , isnull([31], 0) [Day 31]
  from (select [Kode Outlet],[Nama Outlet                    ],[DataCount],[Tanggal ] from @t_CsMonitoringbyperiode) x
  pivot (sum([DataCount]) for [Tanggal ] in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10]
  , [11], [12], [13], [14], [15], [16], [17], [18], [19], [20], [21],[22], [23],[24], [25],[26]
  , [27],[28], [29],[30], [31])) as pvt 

delete @t_data
delete @t_CsMonitoringbyperiode

END


GO


