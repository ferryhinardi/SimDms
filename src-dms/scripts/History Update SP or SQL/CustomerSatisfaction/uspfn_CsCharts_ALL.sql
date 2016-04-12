DROP FUNCTION [dbo].[splitstring]
GO

CREATE FUNCTION [dbo].[splitstring] ( @stringToSplit VARCHAR(MAX), @delimiter varchar(5) = ',' )
RETURNS
 @returnList TABLE ([id] int identity(1, 1), [Name] [nvarchar] (500))
AS
BEGIN

 DECLARE @name NVARCHAR(255)
 DECLARE @pos INT

 WHILE CHARINDEX(@delimiter, @stringToSplit) > 0
 BEGIN
  SELECT @pos  = CHARINDEX(@delimiter, @stringToSplit)  
  SELECT @name = SUBSTRING(@stringToSplit, 1, @pos-1)

  INSERT INTO @returnList 
  SELECT @name

  SELECT @stringToSplit = SUBSTRING(@stringToSplit, @pos+1, LEN(@stringToSplit)-@pos)
 END

 INSERT INTO @returnList
 SELECT @stringToSplit

 RETURN
END

GO

DROP PROCEDURE [dbo].[uspfn_CsChartStnkExt]
GO

--uspfn_CsChartStnkExt '', '1-1-2012', '9-9-2015'
CREATE proc [dbo].[uspfn_CsChartStnkExt]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
as
begin
	select a.CompanyCode, a.BranchCode, d.Area, d.DealerAbbreviation AS Dealer, c.OutletName AS Outlet, a.CustomerCount, isnull(b.InputByCRO, 0) InputByCRO, (convert(numeric(18, 2), (isnull(b.InputByCRO, 0))) / a.CustomerCount * 100) Percentation
	  from (
			 select CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuStnkExtensionView
			 where month(StnkExpiredDate) between month(@DateFrom) + 1 and month(@DateTo) + 1
			 group by CompanyCode, BranchCode
		   ) a
 left join (
			select c.CompanyCode, b.BranchCode, count(c.CustomerCode) InputByCRO from CsStnkExt c
			  join CsCustomerVehicleView b
				on c.CompanyCode = b.CompanyCode
			   and c.CustomerCode = b.CustomerCode
			   and c.Chassis = b.Chassis
			  join CsCustomerView a
				on b.CompanyCode = a.CompanyCode
			   and b.BranchCode = a.BranchCode
			   and b.CustomerCode = a.CustomerCode
			 where convert(varchar, c.CreatedDate, 121) BETWEEN @DateFrom AND @DateTo
			   and month(StnkExpiredDate) between month(@DateFrom) + 1 and month(@DateTo) + 1
			 group by c.CompanyCode, b.BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode
 join gnmstdealeroutletmapping c
		on a.CompanyCode = c.DealerCode
		and a.BranchCode = c.OutletCode
 left join gnmstdealerMapping d
		on a.CompanyCode = d.DealerCode
	 where a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END

end

GO

DROP PROCEDURE [dbo].[uspfn_CsChartStnkExtforexport]
GO

CREATE PROCEDURE [dbo].[uspfn_CsChartStnkExtforexport] 
	@BranchCode varchar(25),
	@DateFrom datetime,
	@DateTo datetime
AS
BEGIN
	
	declare @t_CsChartStnkExt as table
(
	[No] int,
	[Kode Dealer] varchar(25),
	[Kode Outlet] varchar(25),
	[Area] varchar(25),
	[Dealer] varchar(25),
	[Nama Outlet                             ]   varchar(150),
	[Jumlah STNK]  int,
	[Input 3 STNK by CRO]  int,
	[PERSENTASE]   Decimal(4,0) 
)

declare @t_CsChartStnkExtrpt as table
(
	[No] int,
	[Nama Outlet                             ]   varchar(150),
	[Jumlah STNK]  int,
	[Input 3 STNK by CRO]  int,
	[PERSENTASE]   Decimal(4,0) 
)

insert into @t_CsChartStnkExt
	select row_number() over (order by a.CompanyCode), a.CompanyCode, a.BranchCode, d.Area, d.DealerAbbreviation AS Dealer, c.OutletName AS Outlet, a.CustomerCount, isnull(b.InputByCRO, 0) InputByCRO, (convert(numeric(18, 2), (isnull(b.InputByCRO, 0))) / a.CustomerCount * 100) Percentation
	  from (
			 select CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuStnkExtensionView
			 where month(StnkExpiredDate) between month(@DateFrom) + 1 and month(@DateTo) + 1
			 group by CompanyCode, BranchCode
		   ) a
 left join (
			select c.CompanyCode, b.BranchCode, count(c.CustomerCode) InputByCRO from CsStnkExt c
			  join CsCustomerVehicleView b
				on c.CompanyCode = b.CompanyCode
			   and c.CustomerCode = b.CustomerCode
			   and c.Chassis = b.Chassis
			  join CsCustomerView a
				on b.CompanyCode = a.CompanyCode
			   and b.BranchCode = a.BranchCode
			   and b.CustomerCode = a.CustomerCode
			 where convert(varchar, c.CreatedDate, 121) BETWEEN @DateFrom AND @DateTo
			   and month(StnkExpiredDate) between month(@DateFrom) + 1 and month(@DateTo) + 1
			 group by c.CompanyCode, b.BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode
 join gnmstdealeroutletmapping c
		on a.CompanyCode = c.DealerCode
		and a.BranchCode = c.OutletCode
 left join gnmstdealerMapping d
		on a.CompanyCode = d.DealerCode
	 where a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END

select [Kode Dealer],[Kode Outlet],[Area] from @t_CsChartStnkExt

insert into @t_CsChartStnkExtrpt
select 
	row_number() over (order by [Kode Dealer])
	, [Nama Outlet                             ]  
	, [Jumlah STNK]
	, [Input 3 STNK by CRO] 
	, [PERSENTASE] 
from @t_CsChartStnkExt

select * from @t_CsChartStnkExtrpt

delete @t_CsChartStnkExt
delete @t_CsChartStnkExtrpt

END

GO

DROP PROCEDURE [dbo].[uspfn_CsChartTDayCall]
GO

--uspfn_CsChartTDayCall '6006400106', '2015-10-30', '2016-01-01'
create proc [dbo].[uspfn_CsChartTDayCall]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
as
begin
	select a.BranchCode, c.OutletAbbreviation, a.CustomerCount, isnull(b.InputByCRO, 0) InputByCRO, convert(numeric(18,2), isnull(b.InputByCRO, 0)) / a.CustomerCount * 100 Percentation
	  from (
			 select CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuTDaysCallView
			 where convert(varchar, DeliveryDate, 112) BETWEEN convert(varchar, @DateFrom, 112) and convert(varchar, @DateTo, 112)
			 group by CompanyCode, BranchCode
		   ) a
 left join (
			 select CompanyCode, BranchCode, count(CustomerCode) InputByCRO from CsLkuTDaysCallView
			 where Outstanding = 'N'
			   and convert(varchar, DeliveryDate, 112) BETWEEN convert(varchar, @DateFrom, 112) and convert(varchar, @DateTo, 112)
			   and convert(varchar, CreatedDate, 112) BETWEEN convert(varchar, @DateFrom, 112) and convert(varchar, DATEADD(day, 7, @DateTo), 112)
			 group by CompanyCode, BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode
 left join gnmstdealeroutletmapping c
		on a.BranchCode = c.OutletCode
	 where a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END

end
GO

DROP PROCEDURE [dbo].[uspfn_CsChartTDayCallExport]
GO

--uspfn_CsChart3DayCall '6006400131'
CREATE proc [dbo].[uspfn_CsChartTDayCallExport]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
as
begin
-- declare table 3dcall
declare @t_3dcall as table
(
	[Kode Outlet] varchar(25),
	[Nama Outlet                             ]   varchar(150),
	[Jumlah Delivery]  int,
	[Input 3 Days by CRO]  int,
	[PERSENTASE]   numeric(4,0) 
)

declare @t_3dcallrpt as table
(
	[Nama Outlet                             ]   varchar(150),
	[Jumlah Delivery]  int,
	[Input 3 Days by CRO]  int,
	[PERSENTASE]   decimal(4,0) 
)

insert into @t_3dcall
	select a.BranchCode, c.OutletAbbreviation, a.CustomerCount, isnull(b.InputByCRO, 0) InputByCRO, convert(numeric(18,2), isnull(b.InputByCRO, 0)) / a.CustomerCount * 100 Percentation
	  from (
			 select CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuTDaysCallView
			 where convert(varchar, DeliveryDate, 112) BETWEEN convert(varchar, @DateFrom, 112) and convert(varchar, @DateTo, 112)
			 group by CompanyCode, BranchCode
		   ) a
 left join (
			 select CompanyCode, BranchCode, count(CustomerCode) InputByCRO from CsLkuTDaysCallView
			 where Outstanding = 'N'
			   and convert(varchar, DeliveryDate, 112) BETWEEN convert(varchar, @DateFrom, 112) and convert(varchar, @DateTo, 112)
			   and convert(varchar, CreatedDate, 112) BETWEEN convert(varchar, @DateFrom, 112) and convert(varchar, DATEADD(day, 7, @DateTo), 112)
			 group by CompanyCode, BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode
 left join gnmstdealeroutletmapping c
		on a.BranchCode = c.OutletCode
	 where a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END

select [Kode Outlet],[Nama Outlet] from @t_3dcall

insert into @t_3dcallrpt
select 
	[Nama Outlet]
	, [Jumlah Delivery]
	, [Input 3 Days by CRO]
	, [PERSENTASE]
from @t_3dcall
union all
select 
	'Total'
	, sum([Jumlah Delivery])
	, sum([Input 3 Days by CRO])
	, case when (sum([Jumlah Delivery]))=0 then 0
		else (sum([Input 3 Days by CRO])*100) /sum([Jumlah Delivery]) end as 'Percent'
from @t_3dcall

select *
from @t_3dcallrpt

delete @t_3dcall
delete @t_3dcallrpt

end

GO

DROP PROCEDURE [dbo].[uspfn_CsDataTDaysCallDO]
GO

--uspfn_CsDataTDaysCallDO '6006400131', 2015, 9
CREATE procedure [dbo].[uspfn_CsDataTDaysCallDO]
	@BranchCode varchar(50),
	@Year        int,
	@Month       int 

as
 
;with r as (
select a.CompanyCode
     , a.BranchCode
	 , convert(datetime, a.DODate) as DoDate
	 , count(*) as DoData
  from CsCustomerVehicleView a
 where a.BranchCode = @BranchCode
   and a.DODate is not null
   and convert(varchar(7), a.DODate, 121) = convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2)
 group by a.CompanyCode, a.BranchCode, convert(datetime, a.DODate)
)
, s as (
select CompanyCode
     , BranchCode
	 , Month(DoDate) as DoMonth
	 , DoDate
	 , DoData 
	 , TDaysCallData = isnull((
	         select count(a.Chassis)
			   from CsCustomerVehicleView a
			   join CsTDayCall b
				 on b.CompanyCode	= a.CompanyCode
				and b.CustomerCode = a.CustomerCode
				and b.Chassis		= a.Chassis
	          where a.CompanyCode = r.CompanyCode
			    and a.BranchCode = r.BranchCode
				and convert(datetime, a.DoDate) = r.DoDate
				and convert(varchar, b.CreatedDate, 121)
					between convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2) + '-' + '01'
						and convert(varchar(7), dateadd(month, 1, b.CreatedDate), 121) + '07'
			 ), 0)
 from r
)
, t as (
select s.CompanyCode
     , s.DoMonth
	 , s.BranchCode
	 , sum(s.DoData) as DoData
	 , sum(s.TDaysCallData) as TDaysCallData
  from s
 where 1=1
   and Year(s.DoDate) = @Year
   and Month(s.DoDate) = @Month
 group by s.CompanyCode, s.BranchCode, s.DoMonth
)
select b.DealerCode CompanyCode
     , t.DoMonth
	 , b.OutletCode BranchCode
	 , b.OutletAbbreviation BranchName
	 , isnull(DoData, 0) DoData
	 , isnull((select count(i.CustomerCode) from CsLkuTDaysCallView i
			    where i.BranchCode = @BranchCode and i.DeliveryDate is not null
				and convert(varchar(7), i.DeliveryDate, 121) = convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2)
		),0) DeliveryDate

	 --, isnull(TDaysCallData, 0) TDaysCallData
	 , TDaysCallData = isnull((
			 select count(x.CustomerCode) from CsLkuTDaysCallView x
			 where x.Outstanding = 'N'
			   and x.BranchCode = @BranchCode
			   and Year(x.DeliveryDate) = @Year
			   and Month(x.DeliveryDate) = @Month
			   and convert(varchar(10), x.CreatedDate, 121) between convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2) + '-01'
			   and dateadd(month, 1, convert(datetime, convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2) + '-07'))
			), 0)
	 , TDaysCallByInput = isnull((
			 select count(x.CustomerCode) from CsLkuTDaysCallView x
			 where x.Outstanding = 'N'
			   and x.BranchCode = @BranchCode
			   and Year(x.CreatedDate) = @Year
			   and Month(x.CreatedDate) = @Month
			), 0)
  from t
 right join gnMstDealerOutletMapping b
    on b.DealerCode = t.CompanyCode
   and b.OutletCode = t.BranchCode
 where b.OutletCode = @BranchCode
 order by CompanyCode, DoMonth, BranchCode

--select * from SysDealer where DealerCode = @BranchCode and TableName = 'CsTDayCall'
select getdate() LastUpdate

GO

DROP PROCEDURE [dbo].[uspfn_CsDataTDaysCallDOexport]
GO

-- =============================================
-- Author:		fhy
-- Create date: 30122015
-- Description:	CsDataTDaysCallDO export
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_CsDataTDaysCallDOexport]
	@BranchCode varchar(50),
	@Periode    varchar(50),
	@Year        int,
	@Month       int 
AS
BEGIN

declare @t_CsDataTDaysCallDO as table
(
	[Kode Dealer] varchar(25),
	[Bulan DO] varchar(25),
	[Kode Outlet] varchar(25),
	[Nama Outlet                             ]   varchar(150),
	[Data DO       ] int,
	[Delivered     ] int,
	[3 Days Call (By DO)] int,
	[3 Days Call (By Input)]  int
)

declare @t_CsDataTDaysCallDOrpt as table
(
	[Periode          ] varchar(50),
	[Nama Outlet                             ]   varchar(150),
	[Data DO       ] int,
	[Delivered     ] int,
	[3 Days Call (By DO)] int,
	[3 Days Call (By Input)]  int
)

;with r as (
select a.CompanyCode
     , a.BranchCode
	 , convert(datetime, a.DODate) as DoDate
	 , count(*) as DoData
  from CsCustomerVehicleView a
 where a.BranchCode = @BranchCode
   and a.DODate is not null
   and convert(varchar(7), a.DODate, 121) = convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2)
 group by a.CompanyCode, a.BranchCode, convert(datetime, a.DODate)
)
, s as (
select CompanyCode
     , BranchCode
	 , Month(DoDate) as DoMonth
	 , DoDate
	 , DoData 
	 , TDaysCallData = isnull((
	         select count(a.Chassis)
			   from CsCustomerVehicleView a
			   join CsTDayCall b
				 on b.CompanyCode	= a.CompanyCode
				and b.CustomerCode = a.CustomerCode
				and b.Chassis		= a.Chassis
	          where a.CompanyCode = r.CompanyCode
			    and a.BranchCode = r.BranchCode
				and convert(datetime, a.DoDate) = r.DoDate
				and convert(varchar, b.CreatedDate, 121)
					between convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2) + '-' + '01'
						and convert(varchar(7), dateadd(month, 1, b.CreatedDate), 121) + '07'
			 ), 0)
 from r
)
, t as (
select s.CompanyCode
     , s.DoMonth
	 , s.BranchCode
	 , sum(s.DoData) as DoData
	 , sum(s.TDaysCallData) as TDaysCallData
  from s
 where 1=1
   and Year(s.DoDate) = @Year
   and Month(s.DoDate) = @Month
 group by s.CompanyCode, s.BranchCode, s.DoMonth
)
insert into @t_CsDataTDaysCallDO
select b.DealerCode CompanyCode
     , t.DoMonth
	 , b.OutletCode BranchCode
	 , b.OutletAbbreviation BranchName
	 , isnull(DoData, 0) DoData
	 , isnull((select count(i.CustomerCode) from CsCustomerVehicleView i
			    where i.BranchCode = @BranchCode and i.DeliveryDate is not null
				and convert(varchar(7), i.DeliveryDate, 121) = convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2)
		),0) DeliveryDate

	 --, isnull(TDaysCallData, 0) TDaysCallData
	 , TDaysCallData = isnull((
			 select count(x.CustomerCode) from CsLkuTDaysCallView x
			 where x.Outstanding = 'N'
			   and x.BranchCode = @BranchCode
			   and Year(x.DeliveryDate) = @Year
			   and Month(x.DeliveryDate) = @Month
			   and convert(varchar(10), x.CreatedDate, 121) between convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2) + '-01'
			   and dateadd(month, 1, convert(datetime, convert(varchar, @Year) + '-' + right('0' + convert(varchar, @Month), 2) + '-07'))
			), 0)
	 , TDaysCallByInput = isnull((
			 select count(x.CustomerCode) from CsLkuTDaysCallView x
			 where x.Outstanding = 'N'
			   and x.BranchCode = @BranchCode
			   and Year(x.CreatedDate) = @Year
			   and Month(x.CreatedDate) = @Month
			), 0)
 from t
 right join gnMstDealerOutletMapping b
    on b.DealerCode = t.CompanyCode
   and b.OutletCode = t.BranchCode
 where 
 b.OutletCode = @BranchCode
 order by CompanyCode, DoMonth, BranchCode
 
 select [Kode Outlet],[Nama Outlet                             ] from @t_CsDataTDaysCallDO

 insert into @t_CsDataTDaysCallDOrpt
 select 
	@Periode as  [Periode          ],
	[Kode Outlet] + ' - ' + [Nama Outlet] [Nama Outlet],
	[Data DO       ],
	[Delivered     ],
	[3 Days Call (By DO)],
	[3 Days Call (By Input)] 
 from @t_CsDataTDaysCallDO

 select * from @t_CsDataTDaysCallDOrpt
 union All
 select
	'Total' 
	,''
	,ISNULL([Data DO       ],0)
	,ISNULL([Delivered     ],0)
	,ISNULL([3 Days Call (By DO)],0)
	,ISNULL([3 Days Call (By Input)] ,0)
 from @t_CsDataTDaysCallDOrpt
 
delete @t_CsDataTDaysCallDO
delete @t_CsDataTDaysCallDOrpt

END
GO

DROP PROCEDURE [dbo].[uspfn_CsRptBPKBReminder]
GO

--uspfn_CsRptBPKBReminder '6006400131', '', ''
CREATE proc [dbo].[uspfn_CsRptBPKBReminder]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
as
begin
	declare @CurrDate datetime, @Param1 as varchar(20)
	declare @t_rem as table
	(
		RemCode varchar(20),
		RemDate datetime,
		RemValue int
	)

	set @CurrDate = getdate()
	set @Param1 = isnull((select top 1 SettingParam1 from CsSettings where SettingCode = 'REMBPKB'), '0')
	insert into @t_rem (RemCode, RemDate) values('REMBPKB', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

	select d.OutletAbbreviation
		 , convert(datetime, left(convert(varchar, cast(a.BpkbReadyDate as datetime), 121), 7) + '-01') BpkbReadyDate
		 , sum(a.CustomerCount) CustomerCount
		 , sum(isnull(b.InputByCRO, 0)) InputByCRO
		 , sum(isnull(c.Unreachable, 0)) Unreachable
		 , (convert(numeric(5,2), sum(isnull(b.InputByCRO, 0))) / sum(a.CustomerCount) * 100) Percentation
	  from (
			 select BpkbReadyDate, CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuBpkbReminderView
			  where (DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB'))
			  group by BpkbReadyDate, CompanyCode, BranchCode
		   ) a
 left join (
			 select BpkbReadyDate, CompanyCode, BranchCode, count(CustomerCode) InputByCRO from CsLkuBpkbReminderView
			 where convert(varchar, InputDate, 121)
					between convert(varchar, year(BpkbReadyDate)) + '-' + right('0' + convert(varchar, month(BpkbReadyDate)), 2) + '-' + '01'
						and convert(varchar(7), dateadd(month, 1, BpkbReadyDate), 121) + '-' + '07'--BpkbPickUp is not null
			   and (DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB'))
			 group by BpkbReadyDate, CompanyCode, BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.BpkbReadyDate = b.BpkbReadyDate
 left join (
			 select BpkbReadyDate, CompanyCode, BranchCode, count(CustomerCode) Unreachable from CsLkuBpkbReminderView
			 where Reason is not null
			   and (DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB'))

			 group by BpkbReadyDate, CompanyCode, BranchCode
		   ) c
		on a.CompanyCode = c.CompanyCode and a.BranchCode = c.BranchCode and a.BpkbReadyDate = c.BpkbReadyDate
 left join gnmstdealeroutletmapping d
		on a.CompanyCode = d.DealerCode
	   and a.BranchCode = d.OutletCode
	 where a.BpkbReadyDate is not null
	   and a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END
	   and left(convert(varchar, a.BpkbReadyDate, 121), 10) BETWEEN @DateFrom AND @DateTo
  group by left(convert(varchar, cast(a.BpkbReadyDate as datetime), 121), 7), d.OutletAbbreviation

end

GO

DROP PROCEDURE [dbo].[uspfn_CsRptBPKBReminderexport]
GO
-- Author:		fhy	 -- Create date: 29122015
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_CsRptBPKBReminderexport]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
AS
BEGIN
	declare @t_CsReportBPKBRemind as table
(
	[Kode Dealer] varchar(25),
	[Kode Outlet] varchar(25),
	[Nama Outlet                             ]   varchar(150),
	[Tanggan Siap] datetime,
	[Jumlah Customer]  int,
	[Input by CRO]  int,
	[Tidak dapat dihubungi]  int,
	[PERSENTASE]   Decimal(4,0) 
)

declare @t_CsReportBPKBRemindrpt as table
(	
	[Nama Outlet                             ]   varchar(150),
	[Tanggan Siap] varchar(25),
	[Jumlah Customer]  int,
	[Input by CRO]  int,
	[Tidak dapat dihubungi]  int,
	[PERSENTASE]   Decimal(4,0) 
)

	declare @CurrDate datetime, @Param1 as varchar(20)
	declare @t_rem as table
	(
		RemCode varchar(20),
		RemDate datetime,
		RemValue int
	)

	set @CurrDate = getdate()
	set @Param1 = isnull((select top 1 SettingParam1 from CsSettings where SettingCode = 'REMBPKB'), '0')
	insert into @t_rem (RemCode, RemDate) values('REMBPKB', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

insert into @t_CsReportBPKBRemind
select a.CompanyCode, a.BranchCode,d.OutletAbbreviation
		 , convert(datetime, left(convert(varchar, cast(a.BpkbReadyDate as datetime), 121), 7) + '-01') BpkbReadyDate
		 , sum(a.CustomerCount) CustomerCount
		 , sum(isnull(b.InputByCRO, 0)) InputByCRO
		 , sum(isnull(c.Unreachable, 0)) Unreachable
		 , (convert(numeric(5,2), sum(isnull(b.InputByCRO, 0))) / sum(a.CustomerCount) * 100) Percentation
	  from (
			 select BpkbReadyDate, CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuBpkbReminderView
			  where (DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB'))
			 group by BpkbReadyDate, CompanyCode, BranchCode
		   ) a
 left join (
			 select BpkbReadyDate, CompanyCode, BranchCode, count(CustomerCode) InputByCRO from CsLkuBpkbReminderView
			 where convert(varchar, InputDate, 121)
					between convert(varchar, year(BpkbReadyDate)) + '-' + right('0' + convert(varchar, month(BpkbReadyDate)), 2) + '-' + '01'
						and convert(varchar(7), dateadd(month, 1, BpkbReadyDate), 121) + '-07'--BpkbPickUp is not null
			   and (DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB'))
			 group by BpkbReadyDate, CompanyCode, BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.BpkbReadyDate = b.BpkbReadyDate
 left join (
			 select BpkbReadyDate, CompanyCode, BranchCode, count(CustomerCode) Unreachable from CsLkuBpkbReminderView
			 where Reason is not null
			   and (DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB'))
			 group by BpkbReadyDate, CompanyCode, BranchCode
		   ) c
		on a.CompanyCode = c.CompanyCode and a.BranchCode = c.BranchCode and a.BpkbReadyDate = c.BpkbReadyDate
 left join gnmstdealeroutletmapping d
		on a.CompanyCode = d.DealerCode
	   and a.BranchCode = d.OutletCode
	 where a.BpkbReadyDate is not null
	   and a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END
	   and left(convert(varchar, a.BpkbReadyDate, 121), 10) BETWEEN @DateFrom AND @DateTo
  group by left(convert(varchar, cast(a.BpkbReadyDate as datetime), 121), 7), d.OutletAbbreviation,a.CompanyCode, a.BranchCode


select 
	[Kode Dealer]
	,  [Kode Outlet]
from @t_CsReportBPKBRemind

insert into @t_CsReportBPKBRemindrpt
select 
	[Nama Outlet                             ] 
	, CONVERT(VARCHAR(11),[Tanggan Siap],106)
	,[Jumlah Customer]
	,[Input by CRO] 
	,[Tidak dapat dihubungi]
	,[PERSENTASE]
from @t_CsReportBPKBRemind

select * from @t_CsReportBPKBRemindrpt
union all
select 
	'Total'
	, ''
	,sum([Jumlah Customer])
	,sum([Input by CRO])
	,sum([Tidak dapat dihubungi])
	, case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Input by CRO]) *100) /sum([Jumlah Customer]) end as 'Percent'
from @t_CsReportBPKBRemindrpt

delete @t_CsReportBPKBRemind
delete @t_CsReportBPKBRemindrpt
END

GO

DROP PROCEDURE [dbo].[uspfn_GetMonitoringCustBirthday]
GO

CREATE PROCEDURE [dbo].[uspfn_GetMonitoringCustBirthday]
	@BranchCode VARCHAR(20),
	@PeriodYear INT,
	@ParMonth1 INT,
	@ParMonth2 INT,
	@ParStatus INT
AS
BEGIN

	DECLARE @tempTable TABLE (
		CompanyCode	VARCHAR(15),
		BranchCode VARCHAR(15),
		OutletName VARCHAR(100),
		CustomerCode VARCHAR(15),
		BirthDate DATETIME,
		IsReminder VARCHAR(3),
		SentGiftDate DATETIME,
		SpouseTelephone VARCHAR(50),
		InputDate DATETIME,
		TypeOfGift VARCHAR(50)
	)

	INSERT INTO @tempTable
	SELECT 
		a.CompanyCode
		, a.BranchCode
		, d.OutletAbbreviation
		, a.CustomerCode
		, a.BirthDate
		, (CASE WHEN c.CustomerCode IS NULL THEN 'Y' ELSE 'N' END) IsReminder
		, c.SentGiftDate
		, c.SpouseTelephone
		, c.CreatedDate as InputDate
		, c.TypeOfGift
	FROM CsCustomerView a
	LEFT JOIN CsCustData b
		ON b.CompanyCode = a.CompanyCode
		AND b.CustomerCode = a.CustomerCode
	LEFT JOIN CsCustBirthDay c
		ON c.CompanyCode = a.CompanyCode
		AND c.CustomerCode = a.CustomerCode
		AND c.PeriodYear = @PeriodYear
	LEFT JOIN gnMstDealerOutletMapping d
		ON a.CompanyCode = d.DealerCode
		AND a.BranchCode = d.OutletCode
	WHERE a.BranchCode = (CASE ISNULL(@BranchCode, '') WHEN '' THEN a.BranchCode ELSE @BranchCode END)
	AND a.CustomerType = 'I'
	AND a.BirthDate IS NOT NULL
	AND a.BirthDate > '1900-01-01'
	AND (YEAR(GETDATE() - YEAR(a.BirthDate))) > 5
	AND MONTH(a.BirthDate) BETWEEN @ParMonth1 AND @ParMonth2
	AND ISNULL(c.CustomerCode, '1900-01-01') = (CASE @ParStatus
		WHEN 0 THEN ISNULL(c.CustomerCode, '1900-01-01')
		WHEN 1 THEN '1900-01-01'
		ELSE c.CustomerCode
		END)
	ORDER BY DAY(a.BirthDate)

	SELECT 
		a.Month,
		a.OutletName,
		ISNULL(TotalCustomer, 0) TotalCustomer, 
		ISNULL(Reminder, 0) Reminder,
		ISNULL(Gift, 0) Gift,
		ISNULL(SMS, 0) SMS,
		ISNULL(Telephone, 0) Telephone,
		ISNULL(Letter, 0) Letter,
		ISNULL(Souvenir, 0) Souvenir
	FROM 
	(
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS TotalCustomer 
		FROM @tempTable a
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) a	
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Reminder] 
		FROM @tempTable
		WHERE IsReminder = 'N'
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) b ON a.Month = b.Month AND a.OutletName = b.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Gift] 
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift, '|') where id = 1) = 'true'
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) c ON a.Month = c.Month AND a.OutletName = c.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [SMS] 
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift, '|') where id = 3) = 'true' 
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) d ON a.Month = d.Month AND a.OutletName = d.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Telephone] 
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift, '|') where id = 5) = 'true' 
		--WHERE SpouseTelephone IS NOT NULL
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) e ON a.Month = e.Month AND a.OutletName = e.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Letter] 
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift, '|') where id = 2) = 'true' 
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) f ON a.Month = f.Month AND a.OutletName = f.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Souvenir]
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift, '|') where id = 4) = 'true' 
		--WHERE SpouseTelephone IS NOT NULL
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) g ON a.Month = g.Month AND a.OutletName = g.OutletName
	ORDER BY a.OutletName, a.Month
END

GO

DROP PROCEDURE [dbo].[uspfn_GetMonitoringCustBirthdayexport]
GO

-- =============================================
-- Author:		fhy
-- Create date: 29122015
-- Description:	CsReportCustBirthdayexport
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_GetMonitoringCustBirthdayexport] 
	@BranchCode VARCHAR(20),
	@PeriodYear INT,
	@ParMonth1 INT,
	@ParMonth2 INT,
	@ParStatus INT
AS
BEGIN
	DECLARE @tempTable TABLE (
		CompanyCode	VARCHAR(15),
		BranchCode VARCHAR(15),
		OutletName VARCHAR(100),
		CustomerCode VARCHAR(15),
		BirthDate DATETIME,
		IsReminder VARCHAR(3),
		SentGiftDate DATETIME,
		SpouseTelephone VARCHAR(50),
		InputDate DATETIME,
		TypeOfGift VARCHAR(50)
	)

DECLARE @t_CsReportCustBirthday TABLE (
		[Dealer Kode]	VARCHAR(15),
		[Bulan] VARCHAR(100),
		[Nama Outlet] VARCHAR(150),
		[Jumlah Customer]  int,
		[Input by CRO]  int,
		[Gift] int,
		[SMS] int,
		[Telephone] int,
		[Letter] int,
		[Souvenir] int
	)

DECLARE @t_CsReportCustBirthdayrpt TABLE (		
		No varchar(5),
		[Nama Outlet] VARCHAR(150),
		[Bulan] VARCHAR(100),
		[Jumlah Customer] Numeric(18,2),
		[Input by CRO] Numeric(18,2),
		[Gift] Numeric(18,2),
		[SMS] Numeric(18,2),
		[Telephone] Numeric(18,2),
		[Letter] Numeric(18,2),
		[Souvenir] Numeric(18,2)
	)

	INSERT INTO @tempTable
	SELECT 
		a.CompanyCode
		, a.BranchCode
		, d.OutletAbbreviation
		, a.CustomerCode
		, a.BirthDate
		, (CASE WHEN c.CustomerCode IS NULL THEN 'Y' ELSE 'N' END) IsReminder
		, c.SentGiftDate
		, c.SpouseTelephone
		, c.CreatedDate as InputDate
		, c.TypeOfGift
	FROM CsCustomerView a
	LEFT JOIN CsCustData b
		ON b.CompanyCode = a.CompanyCode
		AND b.CustomerCode = a.CustomerCode
	LEFT JOIN CsCustBirthDay c
		ON c.CompanyCode = a.CompanyCode
		AND c.CustomerCode = a.CustomerCode
		AND c.PeriodYear = @PeriodYear
	LEFT JOIN gnMstDealerOutletMapping d
		ON a.CompanyCode = d.DealerCode
		AND a.BranchCode = d.OutletCode
	WHERE a.BranchCode = (CASE ISNULL(@BranchCode, '') WHEN '' THEN a.BranchCode ELSE @BranchCode END)
	AND a.CustomerType = 'I'
	AND a.BirthDate IS NOT NULL
	AND a.BirthDate > '1900-01-01'
	AND (YEAR(GETDATE() - YEAR(a.BirthDate))) > 5
	AND MONTH(a.BirthDate) BETWEEN @ParMonth1 AND @ParMonth2
	AND ISNULL(c.CustomerCode, '1900-01-01') = (CASE @ParStatus
		WHEN 0 THEN ISNULL(c.CustomerCode, '1900-01-01')
		WHEN 1 THEN '1900-01-01'
		ELSE c.CustomerCode
		END)
	ORDER BY DAY(a.BirthDate)

insert into @t_CsReportCustBirthday
	SELECT 
		a.CompanyCode,
		a.Month,
		a.OutletName,
		ISNULL(TotalCustomer, 0) TotalCustomer, 
		ISNULL(Reminder, 0) Reminder,
		ISNULL(Gift, 0) Gift,
		ISNULL(SMS, 0) SMS,
		ISNULL(Telephone, 0) Telephone,
		ISNULL(Letter, 0) Letter,
		ISNULL(Souvenir, 0) Souvenir
	FROM 
	(
		SELECT 
			CompanyCode,DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS TotalCustomer 
		FROM @tempTable a
		GROUP BY DATEPART(MONTH, BirthDate), OutletName,CompanyCode
	) a	
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Reminder] 
		FROM @tempTable
		WHERE IsReminder = 'N'
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) b ON a.Month = b.Month AND a.OutletName = b.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Gift] 
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift, '|') where id = 1) = 'true'
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) c ON a.Month = c.Month AND a.OutletName = c.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [SMS] 
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift, '|') where id = 3) = 'true' 
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) d ON a.Month = d.Month AND a.OutletName = d.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Telephone] 
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift, '|') where id = 5) = 'true' 
		--WHERE SpouseTelephone IS NOT NULL
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) e ON a.Month = e.Month AND a.OutletName = e.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Letter] 
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift, '|') where id = 2) = 'true' 
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) f ON a.Month = f.Month AND a.OutletName = f.OutletName
	LEFT JOIN (
		SELECT 
			DATEPART(MONTH, BirthDate) AS [Month],
			OutletName,
			COUNT(CompanyCode) AS [Souvenir]
		FROM @tempTable
		WHERE TypeOfGift IS NOT NULL and (SELECT Name FROM dbo.splitstring(TypeOfGift, '|') where id = 4) = 'true' 
		--WHERE SpouseTelephone IS NOT NULL
		GROUP BY DATEPART(MONTH, BirthDate), OutletName
	) g ON a.Month = g.Month AND a.OutletName = g.OutletName
	ORDER BY a.OutletName, a.Month,a.CompanyCode

select * from @t_CsReportCustBirthday

insert into @t_CsReportCustBirthdayrpt
select 
	ROW_NUMBER() over (order by [Dealer Kode])
	, [Nama Outlet] 
	, case when [Bulan]	=1 then 'Januari' 
		when [Bulan]=2 then 'Februari' 
		when [Bulan]=3 then 'Maret' 
		when [Bulan]=4 then 'April' 
		when [Bulan]=5 then 'Mei' 
		when [Bulan]=6 then 'Juni' 
		when [Bulan]=7 then 'Juli' 
		when [Bulan]=8 then 'Agustus' 
		when [Bulan]=9 then 'September' 
		when [Bulan]=10 then 'Oktober' 
		when [Bulan]=11 then 'Nopember' 
		else  'Desember'  end 
	, [Jumlah Customer]
	, [Input by CRO]
	, [Gift]
	, [SMS]
	, [Telephone]
	, [Letter]
	, [Souvenir]
from @t_CsReportCustBirthday

select * from @t_CsReportCustBirthdayrpt
Union all
select 
'Total'
, ''
, ''
, ISNULL(sum([Jumlah Customer]),0)
, ISNULL(sum([Input by CRO]),0)
, ISNULL(sum([Gift]),0)
, ISNULL(sum([SMS]),0)
, ISNULL(sum([Telephone]),0)
, ISNULL(sum([Letter]),0)
, ISNULL(sum([Souvenir]),0)
from @t_CsReportCustBirthdayrpt

Union all
select 
'Persentase  (%)'
, ''
, ''
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Jumlah Customer]) *100) /sum([Jumlah Customer]) end,0)
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Input by CRO]) *100) /sum([Jumlah Customer]) end,0)
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Gift]) *100) /sum([Jumlah Customer]) end,0)
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([SMS]) *100) /sum([Jumlah Customer]) end,0)
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Telephone]) *100) /sum([Jumlah Customer]) end,0)
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Letter]) *100) /sum([Jumlah Customer]) end,0)
, isnull(case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Souvenir]) *100) /sum([Jumlah Customer]) end,0)
from @t_CsReportCustBirthdayrpt

delete @tempTable
delete @t_CsReportCustBirthday
delete @t_CsReportCustBirthdayrpt
END

GO
