
-- =============================================
-- Author:		fhy
-- Create date: 30122015
-- Description:	CsMonitoringbydateExport
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_CsChartMonitoringbydateexport]
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
	[Tanggal ] varchar(50),
	[DataCount] int,
	[tgl] datetime
)

declare @t_tanggal as table(
	[Tanggal ] varchar(50)
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

insert into @t_CsMonitoringbyperiode
 select 
	 data.DealerCode [Kode Outlet]
	 , map.OutletAbbreviation  [Nama Outlet                    ]
	 ,'Tgl : '+cast((DATEPART(day,InputDate)) as varchar(15))+'/'+cast((datepart(month,InputDate)) as varchar(15))
	 , DataCount  [DataCount]
	 , InputDate
  from @t_data data
  left join gnMstDealerOutletMapping map on map.OutletCode=data.dealercode
  order by InputDate,map.OutletAbbreviation

--select * from @t_CsMonitoringbyperiode

declare @ndays int
declare @count int
set @count=0

set @ndays = (Select datediff(day, @DateFrom, @DateTo))

--select @ndays

while @count <= @ndays
begin
	insert into @t_tanggal
	select 'Tgl : '+cast((DATEPART(day,DATEADD(day,@count,@DateFrom))) as varchar(15))+'/'+cast((datepart(month,DATEADD(day,@count,@DateFrom))) as varchar(15))

	set @count = @count+1
end


--insert into @t_tanggal
--select [Tanggal ]  from @t_CsMonitoringbyperiode group by [Tanggal ],CONVERT(VARCHAR(11),[tgl],106) order by CONVERT(VARCHAR(11),[tgl],106)

DECLARE @columns NVARCHAR(MAX), @sql NVARCHAR(MAX);
SET @columns = N'';
SELECT @columns += N',' + QUOTENAME([Tanggal ])
  FROM (select [Tanggal ] from @t_tanggal ) AS x  ;

--select @columns
--select stuff(REPLACE(@columns,',[',',['),1,1,'')

select * into #temp_CsMonitoringbyperiode from  @t_CsMonitoringbyperiode

SET @sql = N'
SELECT [Nama Outlet                    ],' + stuff(REPLACE(@columns,', p.[',',['),1,1,'') + '
FROM
(
  select [Kode Outlet],[Nama Outlet                    ],[DataCount],[Tanggal ] from #temp_CsMonitoringbyperiode
) AS j
PIVOT
(
  SUM([DataCount]) FOR [Tanggal ] IN ('
  + stuff(REPLACE(@columns,', p.[',',['),1,1,'')
  + ')
)  as pvt ';


PRINT (@sql);
exec (@sql);


delete @t_data
delete @t_CsMonitoringbyperiode
delete @t_tanggal

drop table #temp_CsMonitoringbyperiode


END





GO


