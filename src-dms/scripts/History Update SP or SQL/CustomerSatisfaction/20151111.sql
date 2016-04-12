insert into sysMenuDms values ('cschart', 'Chart', 'cs', 5, 1, NULL, NULL)
insert into sysRoleMenu values ('ADMIN', 'cschart')
insert into sysMenuDms values ('cschMonitoring1',			'Monitoring by Date', 'cschart', 1, 2, 'chart/monitoring1', NULL)
insert into sysMenuDms values ('cschMonitoring2',			'Monitoring by Period', 'cschart', 2, 2, 'chart/monitoring2', NULL)
insert into sysMenuDms values ('cschMonitoring3',			'Data Summary', 'cschart', 3, 2, 'chart/monitoring3', NULL)
insert into sysMenuDms values ('cschMonitoring4',			'Data DO vs 3 Days Call', 'cschart', 4, 2, 'chart/monitoring4', NULL)
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring1')
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring2')
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring3')
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring4')
insert into sysMenuDms values ('cschMonitoring5',			'Report 3 Days Call', 'cschart',		 5, 2, 'chart/monitoring5', NULL)
insert into sysMenuDms values ('cschMonitoring6',			'Report Customer Birthday', 'cschart',	 6, 2, 'chart/monitoring6', NULL)
insert into sysMenuDms values ('cschMonitoring7',			'Report BPKB Reminder', 'cschart',		 7, 2, 'chart/monitoring7', NULL)
insert into sysMenuDms values ('cschMonitoring8',			'Report STNK Extension', 'cschart',		 8, 2, 'chart/monitoring8', NULL)

insert into sysRoleMenu values ('ADMIN', 'cschMonitoring5')
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring6')
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring7')
insert into sysRoleMenu values ('ADMIN', 'cschMonitoring8')

GO
IF OBJECT_ID('uspfn_CsGetStnkExt') is not null
	drop proc dbo.uspfn_CsGetStnkExt
GO

IF NOT EXISTS(select top 1 * from information_schema.columns 
where column_name = 'Reason' and table_name = 'CsCustBpkb')
BEGIN
	ALTER TABLE CsCustBpkb
	add Reason varchar(50)
END
GO

ALTER view [dbo].[CsLkuBpkbReminderView]
as
select a.CompanyCode
     , a.BranchCode
	 , a.CustomerCode
	 , a.CustomerName
	 , b.DODate
	 , b.Chassis
	 , b.Engine
	 , b.SalesModelCode
	 , b.SalesModelYear
	 , b.ColourCode
	 , c.BpkbReadyDate
	 , c.BpkbPickUp
	 , b.BPKDate
	 , b.IsLeasing
	 , b.LeasingCo
	 , b.Installment
	 , case isnull(b.isLeasing, 0) when 0 then 'Cash' else 'Leasing' end as LeasingDesc
	 , case isnull(b.isLeasing, 0) when 0 then '-' else b.LeasingName end as LeasingName
	 , case isnull(b.isLeasing, 0) when 0 then '-' else (convert(varchar, isnull(b.Installment, 0)) + ' Month') end as Tenor
	 , a.Address
	 , a.PhoneNo
	 , a.HpNo
	 , a.AddPhone1
	 , a.AddPhone2
	 , b.SalesmanCode 
	 , b.SalesmanName
	 , c.ReqKtp
     , c.ReqStnk
     , c.ReqSuratKuasa
     , c.Comment
     , c.Additional
	 , BpkbDate = isnull(d.BPKBDate, b.BPKDate)
	 , StnkDate = isnull(d.StnkDate, b.BPKDate)
	 , Category = ( 
			case 
				 when isnull(b.isLeasing, 0) = 0 then 'Tunai' 
				 else 'Leasing'
			end
	   )
     , c.Status
	 , (case c.Status when 1 then 'Finish' else 'In Progress' end) as StatusInfo
	 , b.PoliceRegNo
	 , InputDate = a.CreatedDate
	 , DelayedRetrievalDate = (
			select top 1 x.RetrievalEstimationDate
			  from CsBpkbRetrievalInformation x
			 where x.CompanyCode = a.CompanyCode
			   and x.CustomerCode = a.CustomerCode
			 order by x.RetrievalEstimationDate desc
	   )
	 , DelayedRetrievalNote = (
			select top 1 x.Notes
			  from CsBpkbRetrievalInformation x
			 where x.CompanyCode = a.CompanyCode
			   and x.CustomerCode = a.CustomerCode
			 order by x.RetrievalEstimationDate desc
	   )
	 , Outstanding = (
			case 
				when isnull(c.CustomerCode, '') = '' then 'Y'
				when c.BpkbPickUp >= c.BpkbReadyDate then 'N'
				when ( 
						select top 1 x.RetrievalEstimationDate 
						  from CsBpkbRetrievalInformation x 
						 where x.CompanyCode = a.CompanyCode
						   and x.CustomerCode = a.CustomerCode
						 order by x.RetrievalEstimationDate desc
					 ) > getdate() then 'N'
				else 'Y'
			end	
	   ),
	   c.Reason
  from CsCustomerView a
  left join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.CustomerCode = a.CustomerCode
  left join CsCustBpkb c
    on c.CompanyCode = b.CompanyCode
   and c.CustomerCode = b.CustomerCode
  left join CsCustomerVehicle d
    on d.CompanyCode = c.CompanyCode
   and d.Chassis = b.Chassis


GO



SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsGetStnkExt]
	@CompanyCode  varchar(20),
	@CustomerCode varchar(20),
	@Chassis      varchar(50)
as

--select @CompanyCode = '6006406', @CustomerCode = '1000581', @Chassis = 'MHYGDN42VBJ352996'
select a.CompanyCode     
	 , a.BranchCode
	 , b.CustomerCode
	 , c.CustomerName
	 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
	 , a.EngineCode + convert(varchar, a.EngineNo) Engine
	 , a.SalesModelCode
	 , SalesModel = a.SalesModelCode + '-' + (
					select SalesModelDesc
					  from omMstModel
					 where CompanyCode = a.CompanyCode
					   and SalesModelCode = a.SalesModelCode
	   )
	 , a.SalesModelYear
	 , a.ColourCode
	 , Colour = a.ColourCode + '-' + (
					select RefferenceDesc1 
					 from omMstRefference 
					where CompanyCode = a.CompanyCode
					  and RefferenceCode = a.ColourCode
					  and RefferenceType = 'COLO'
	   )
	 , isnull(k.BpkbDate,  g.BPKDate) BpkbDate
     , case when m.STNKInDate = '1900-01-01 00:00:00.000' then null else m.STNKInDate end StnkDate
	 , g.BPKDate
	 , h.isLeasing as IsLeasing
	 , h.LeasingCo
	 , isnull(h.isLeasing, 0) as IsLeasing
	 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
	 , i.CustomerName as LeasingName
	 , h.Installment
	 , convert(varchar, isnull(h.Installment, 0)) + ' Month' as Tenor
     , left(c.Address1, 40) as Address
     , c.PhoneNo
     , h.Salesman
     , j.EmployeeName as SalesmanName
     , k.IsStnkExtend
	 , k.Ownership
     , isnull(k.StnkExpiredDate, dateadd(year, 1, isnull(k.StnkDate, g.BPKDate)))  as StnkExpiredDate
     , k.ReqKtp
     , k.ReqStnk
     , k.ReqBpkb
     , k.ReqSuratKuasa
     , k.Comment, k.Additional, k.Status
     , (case k.Status when 1 then 'Finish' else 'In Progress' end) as StatusInfo
     , case (isnull(k.Chassis, '')) when '' then 1 else 0 end as IsNew
     , l.PoliceRegNo
	 , k.Ownership
  from omTrSalesInvoiceVin a    
  left join omTrSalesInvoice b    
	on b.CompanyCode = a.CompanyCode    
   and b.BranchCode = a.BranchCode    
   and b.InvoiceNo = a.InvoiceNo    
  left join gnMstCustomer c
	on c.CompanyCode = a.CompanyCode    
   and c.CustomerCode = b.CustomerCode
  left join omTrSalesDODetail d
	on d.CompanyCode = a.CompanyCode    
   and d.BranchCode = a.BranchCode
   and d.ChassisCode = a.ChassisCode
   and d.ChassisNo = a.ChassisNo
 -- left join CsCustomerVehicle e
	--on e.CompanyCode = a.CompanyCode    
 --  and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join omTrSalesDO f
	on f.CompanyCode = d.CompanyCode    
   and f.BranchCode = d.BranchCode
   and f.DONo = d.DONo
  left join omTrSalesBPK g
	on g.CompanyCode = f.CompanyCode    
   and g.BranchCode = f.BranchCode
   and g.DONo = f.DONo
  left join omTrSalesSO h
	on h.CompanyCode = g.CompanyCode    
   and h.BranchCode = g.BranchCode
   and h.SONo = g.SONo
  left join gnMstCustomer i
	on i.CompanyCode = h.CompanyCode
   and i.CustomerCode = h.LeasingCo
  left join HrEmployee j
	on j.CompanyCode = h.CompanyCode
   and j.EmployeeID = h.Salesman
  left join CsStnkExt k
	on k.CompanyCode = a.CompanyCode
   and k.CustomerCode = b.CustomerCode
   and k.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join svMstCustomerVehicle l
	on l.CompanyCode = a.CompanyCode
   and l.ChassisCode = a.ChassisCode
   and l.ChassisNo = a.ChassisNo
  left join
	 ( 
	   select distinct 
	     CompanyCode
	   , BranchCode
	   , ChassisCode
	   , ChassisNo
	   , STNKInDate 
	   from omTrSalesSPKDetail
	 ) m
    on m.CompanyCode = a.CompanyCode
   and m.BranchCode = a.BranchCode
   and m.ChassisCode = a.ChassisCode
   and m.ChassisNo = a.ChassisNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and b.CustomerCode = @CustomerCode
   and a.ChassisCode + convert(varchar, a.ChassisNo) = @Chassis

GO

IF (OBJECT_ID('uspfn_CsChartTDayCall') is not null)
	drop proc dbo.uspfn_CsChartTDayCall
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_CsChart3DayCall '6006400131'
create proc [dbo].[uspfn_CsChartTDayCall]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
as
begin
	select a.BranchCode, c.OutletAbbreviation, a.CustomerCount, isnull(b.InputByCRO, 0) InputByCRO, convert(numeric(18,2), isnull(b.InputByCRO, 0)) / a.CustomerCount * 100 Percentation
	  from (
			 select CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuTDaysCallView
			 where BpkDate BETWEEN @DateFrom and @DateTo
			 group by CompanyCode, BranchCode
		   ) a
 left join (
			 select CompanyCode, BranchCode, count(CustomerCode) InputByCRO from CsLkuTDaysCallView
			 where Outstanding = 'N'
			   and BpkDate BETWEEN @DateFrom AND @DateTo
			 group by CompanyCode, BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode
 left join gnmstdealeroutletmapping c
		on a.BranchCode = c.OutletCode
	 where a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END

end
GO

IF(OBJECT_ID('uspfn_GetMonitoringCustBirthday') is not null)
	drop proc dbo.uspfn_GetMonitoringCustBirthday
GO

CREATE PROCEDURE uspfn_GetMonitoringCustBirthday
	@BranchCode VARCHAR(20),
	@PeriodYear INT,
	@ParMonth1 INT,
	@ParMonth2 INT,
	@ParStatus INT,
	@type INT
AS
BEGIN

	DECLARE @tempTable TABLE (
		CompanyCode	VARCHAR(15),
		BranchCode VARCHAR(15),
		CustomerCode VARCHAR(15),
		BirthDate DATETIME,
		IsReminder VARCHAR(3),
		SentGiftDate DATETIME,
		SpouseTelephone VARCHAR(50),
		InputDate DATETIME
	)

	INSERT INTO @tempTable
	SELECT 
		a.CompanyCode
		, a.BranchCode
		, a.CustomerCode
		, a.BirthDate
		, (CASE WHEN c.CustomerCode IS NULL THEN 'N' ELSE 'Y' END) IsReminder
		, c.SentGiftDate
		, c.SpouseTelephone
		, c.CreatedDate as InputDate
	FROM CsCustomerView a
	LEFT JOIN CsCustData b
		ON b.CompanyCode = a.CompanyCode
		AND b.CustomerCode = a.CustomerCode
	LEFT JOIN CsCustBirthDay c
		ON c.CompanyCode = a.CompanyCode
		AND c.CustomerCode = a.CustomerCode
		AND c.PeriodYear = @PeriodYear
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

	IF (@type = 0)
	BEGIN
		SELECT 
			a.Month,
			a.Periode, 
			ISNULL(TotalCustomer, 0) TotalCustomer, 
			ISNULL(Reminder, 0) Reminder, 
			ISNULL(Gift, 0) Gift, 
			ISNULL(Telephone, 0) Telephone
		FROM 
		(
			SELECT 
				DATEPART(MONTH, BirthDate) AS [Month],
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS TotalCustomer 
			FROM @tempTable a
			GROUP BY DATEPART(MONTH, BirthDate), DATEPART(WEEK, BirthDate)
		) a	
		LEFT JOIN (
			SELECT 
				DATEPART(MONTH, BirthDate) AS [Month],
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS [Reminder] 
			FROM @tempTable
			WHERE IsReminder = 'N'
			GROUP BY DATEPART(MONTH, BirthDate), DATEPART(WEEK, BirthDate)
		) b ON a.Month = b.Month AND a.Periode = b.Periode
		LEFT JOIN (
			SELECT 
				DATEPART(MONTH, BirthDate) AS [Month],
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS [Gift] 
			FROM @tempTable
			WHERE SentGiftDate IS NOT NULL
			GROUP BY DATEPART(MONTH, BirthDate), DATEPART(WEEK, BirthDate)
		) c ON a.Month = c.Month AND a.Periode = c.Periode
		LEFT JOIN (
			SELECT 
				DATEPART(MONTH, BirthDate) AS [Month],
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS [Telephone] 
			FROM @tempTable
			WHERE SpouseTelephone IS NOT NULL
			GROUP BY DATEPART(MONTH, BirthDate), DATEPART(WEEK, BirthDate)
		) d ON a.Month = d.Month AND a.Periode = d.Periode
		ORDER BY a.Month, b.Periode
	END
	---------------------------------------------------------------------
	ELSE
	BEGIN
		SELECT 
			a.Periode, 
			ISNULL(TotalCustomer, 0) TotalCustomer, 
			ISNULL(Reminder, 0) Reminder, 
			ISNULL(Gift, 0) Gift, 
			ISNULL(Telephone, 0) Telephone
		FROM 
		(
			SELECT 
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS TotalCustomer 
			FROM @tempTable a
			GROUP BY DATEPART(WEEK, BirthDate)
		) a	
		LEFT JOIN (
			SELECT 
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS [Reminder] 
			FROM @tempTable
			WHERE IsReminder = 'N'
			GROUP BY DATEPART(WEEK, BirthDate)
		) b ON a.Periode = b.Periode
		LEFT JOIN (
			SELECT 
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS [Gift] 
			FROM @tempTable
			WHERE SentGiftDate IS NOT NULL
			GROUP BY DATEPART(WEEK, BirthDate)
		) c ON a.Periode = c.Periode
		LEFT JOIN (
			SELECT 
				DATEPART(WEEK, BirthDate) AS [Periode], 
				COUNT(CompanyCode) AS [Telephone] 
			FROM @tempTable
			WHERE SpouseTelephone IS NOT NULL
			GROUP BY DATEPART(WEEK, BirthDate)
		) d ON a.Periode = d.Periode
		ORDER BY a.Periode
	END
END
GO

IF OBJECT_ID('uspfn_CsDashSummary') is not null
	drop proc dbo.uspfn_CsDashSummary
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE procedure [dbo].[uspfn_CsDashSummary]
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20)
as
declare @CurrDate datetime, @Param1 as varchar(20)
declare @t_rem as table
(
	RemCode varchar(20),
	RemDate datetime,
	RemValue int
)

set @CurrDate = getdate()

-- REM3DAYSCALL
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REM3DAYSCALL'), '0')
insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

-- REMBDAYSCALL
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBDAYSCALL'), '0')
insert into @t_rem (RemCode, RemDate) values('REMBDAYSCALL', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

-- REMHOLIDAYS
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMHOLIDAYS'), '0')
insert into @t_rem (RemCode, RemDate) values('REMHOLIDAYS', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

-- REMSTNKEXT
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMSTNKEXT'), '0')
insert into @t_rem (RemCode, RemDate) values('REMSTNKEXT', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

-- REMBPKB
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBPKB'), '0')
insert into @t_rem (RemCode, RemDate) values('REMBPKB', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )
   
declare @CurrentMonth tinyint;
declare @PreviousMonth tinyint;
declare @NextMonth tinyint;
declare @CurrentDay tinyint;
declare @DateComparison datetime;


set @CurrentDay = datepart(day, getdate());
set @CurrentMonth = DATEPART(month, getdate());

if @CurrentMonth = 1 
	set @PreviousMonth=12
else
	set @PreviousMonth=@CurrentMonth-1;
if @CurrentMonth = 12 
	set @NextMonth=1
else
	set @NextMonth=@CurrentMonth+1;

--update @t_rem set RemValue = (select count(CustomerCode) from CsLkuTDayCallView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'Y' and DODate >= (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')) where RemCode = 'REM3DAYSCALL'
--update @t_rem set RemValue = (select count(CustomerCode) from CsLkuStnkExtView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'Y' and StnkExpiredDate >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')) where RemCode = 'REMSTNKEXT'
--update @t_rem set RemValue = (select count(CustomerCode) from CsLkuBpkbView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'Y' and BpkbDate >= (select RemDate from @t_rem where RemCode = 'REMBPKB')) where RemCode = 'REMBPKB'
--update @t_rem set RemValue = (select count(CustomerCode) from CsLkuBirthdayView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'Y' ) where RemCode = 'REMBDAYSCALL'

update @t_rem set RemValue = (
						select count(a.CustomerCode)
						  from CsCustomerVehicleView a
						 inner join CsCustomerView b 
						    on b.CompanyCode = a.CompanyCode
						   and b.CustomerCode = a.CustomerCode
						  left join CsStnkExt c
						    on c.CompanyCode = a.CompanyCode
						   and c.CustomerCode = a.CustomerCode
						   and c.Chassis = a.Chassis
						 where a.CompanyCode like @CompanyCode
						   and a.BranchCode like @BranchCode
						   and isnull(c.Chassis, '') = ''
						   and isnull(c.StnkExpiredDate, isnull(c.StnkDate, a.BpkDate)) >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')
				)
 where RemCode = 'REMSTNKEXT';

 update @t_rem set RemValue = ( 
		select count(a.CustomerCode)
		from CsLkuTDaysCallView a
		where a.CompanyCode = @CompanyCode
			and a.BranchCode = @BranchCode
			and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')
			and a.Outstanding = 'Y'
	)
 where RemCode = 'REM3DAYSCALL';

  update @t_rem set RemValue = (
						select count(a.CustomerCode)
						  from CsLkuBpkbReminderView a
						 where a.CompanyCode like @CompanyCode
						   and a.BranchCode like @BranchCode
						   and a.Outstanding = 'Y'
						   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')
				)
  where RemCode = 'REMBPKB';
  
  set @DateComparison = (select RemDate from @t_rem where RemCode = 'REMBDAYSCALL');
  set @CurrentMonth = datepart(month, @DateComparison);
  set @NextMonth = @CurrentMonth + 1;
  set @PreviousMonth = @CurrentMonth - 1;

 update @t_rem set RemValue = (
						select count(distinct a.CustomerCode)
						  from CsCustomerVehicleView a
						 inner join CsCustomerView b 
						    on b.CompanyCode = a.CompanyCode
						   and b.CustomerCode = a.CustomerCode
						  left join CsCustBirthday c
						    on c.CompanyCode = a.CompanyCode
						   and c.CustomerCode = a.CustomerCode
						 where a.CompanyCode like @CompanyCode
						   and a.BranchCode like @BranchCode
						   and isnull(c.CustomerCode, '') = ''
						   and datepart(month, b.BirthDate) >= @PreviousMonth
						   and datepart(month, b.BirthDate) <= @CurrentMonth
				)
 where RemCode = 'REMBDAYSCALL';

select a.RemCode, a.RemDate, a.RemValue, b.SettingLink1 as ControlLink
  from @t_rem a
  join CsSettings b
    on b.CompanyCode = @CompanyCode
   and b.SettingCode = a.RemCode

GO

IF OBJECT_ID('uspfn_CsGetBpkb') is not null
	drop proc dbo.uspfn_CsGetBpkb
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE procedure [dbo].[uspfn_CsGetBpkb]
	@CompanyCode  varchar(20),
	@CustomerCode varchar(20),
	@Chassis      varchar(50)
as

--select @CompanyCode = '6006406', @CustomerCode = '1000581', @Chassis = 'MHYGDN42VBJ352996'
select a.CompanyCode     
	 , a.BranchCode
	 , b.CustomerCode
	 , c.CustomerName
	 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
	 , a.EngineCode + convert(varchar, a.EngineNo) Engine
	 , a.SalesModelCode
	 , SalesModel = a.SalesModelCode + '-' + (
					select SalesModelDesc
					  from omMstModel
					 where CompanyCode = a.CompanyCode
					   and SalesModelCode = a.SalesModelCode
	   )
	 , a.SalesModelYear
	 , a.ColourCode
	 , Colour = a.ColourCode + '-' + (
					select RefferenceDesc1 
					 from omMstRefference 
					where CompanyCode = a.CompanyCode
					  and RefferenceCode = a.ColourCode
					  and RefferenceType = 'COLO'
	   )
	 , g.BPKDate		BpkbDate
     , case when e.STNKInDate = '1900-01-01 00:00:00.000' then null else e.STNKInDate end StnkDate
	 , g.BPKDate
	 , h.isLeasing		IsLeasing
	 , h.LeasingCo
	 , isnull(h.isLeasing, 0) as IsLeasing
	 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
	 , i.CustomerName	LeasingName
	 , h.Installment
	 , convert(varchar, isnull(h.Installment, 0)) + ' Month' as Tenor
     , left(c.Address1, 40) as Address
     , c.PhoneNo
     , h.Salesman
     , j.EmployeeName	SalesmanName
     , case when e.BPKBInDate = '1900-01-01 00:00:00.000' then null else e.BPKBInDate end BpkbReadyDate
     , case when e.BPKBOutDate = '1900-01-01 00:00:00.000' then null else e.BPKBOutDate end BpkbPickUp
     , k.ReqInfoLeasing
     , k.ReqInfoCust
     , k.ReqKtp
     , k.ReqStnk
     , k.ReqSuratKuasa
     , k.Comment, k.Additional, k.Status
     , (case k.Status when 1 then 'Finish' else 'In Progress' end) as StatusInfo
     , case (isnull(k.Chassis, '')) when '' then 1 else 0 end as IsNew
     , l.PoliceRegNo
	 , DelayedRetrievalDate = (
			select top 1
			       x.RetrievalEstimationDate
			  from CsBpkbRetrievalInformation x
			 where x.CompanyCode = b.CompanyCode
			   and x.CustomerCode = b.CustomerCode
			 order by x.RetrievalEstimationDate desc
	   )
	 , DelayedRetrievalNote = (
			select top 1
			       x.Notes
			  from CsBpkbRetrievalInformation x
			 where x.CompanyCode = b.CompanyCode
			   and x.CustomerCode = b.CustomerCode
			 order by x.RetrievalEstimationDate desc
	   ), k.Reason

  from omTrSalesInvoiceVin a    
  left join omTrSalesInvoice b    
	on b.CompanyCode = a.CompanyCode    
   and b.BranchCode = a.BranchCode    
   and b.InvoiceNo = a.InvoiceNo    
  left join gnMstCustomer c
	on c.CompanyCode = a.CompanyCode    
   and c.CustomerCode = b.CustomerCode
  left join omTrSalesDODetail d
	on d.CompanyCode = a.CompanyCode    
   and d.BranchCode = a.BranchCode
   and d.ChassisCode = a.ChassisCode
   and d.ChassisNo = a.ChassisNo
  left join
	 ( 
	   select distinct 
		 	  a.CompanyCode
		    , a.BranchCode
		    , a.ChassisCode
		    , a.ChassisNo
		    , a.STNKInDate
		    , a.BPKBInDate
		    , isnull(b.BPKBOutDate, a.BPKBOutDate) BPKBOutDate
		 from omTrSalesSPKDetail a
		 left join omTrSalesSPKSubDetail b
	 	   on b.CompanyCode = a.CompanyCode
	      and b.BranchCode = a.BranchCode
	      and b.ChassisCode = a.ChassisCode
	      and b.ChassisNo = a.ChassisNo
	      and b.SPKNo = a.SPKNo
	 ) e
    on e.CompanyCode = a.CompanyCode
   and e.BranchCode = a.BranchCode
   and e.ChassisCode = a.ChassisCode
   and e.ChassisNo = a.ChassisNo
  left join omTrSalesDO f
	on f.CompanyCode = d.CompanyCode    
   and f.BranchCode = d.BranchCode
   and f.DONo = d.DONo
  left join omTrSalesBPK g
	on g.CompanyCode = f.CompanyCode    
   and g.BranchCode = f.BranchCode
   and g.DONo = f.DONo
  left join omTrSalesSO h
	on h.CompanyCode = g.CompanyCode    
   and h.BranchCode = g.BranchCode
   and h.SONo = g.SONo
  left join gnMstCustomer i
	on i.CompanyCode = h.CompanyCode
   and i.CustomerCode = h.LeasingCo
  left join HrEmployee j
	on j.CompanyCode = h.CompanyCode
   and j.EmployeeID = h.Salesman
  left join CsCustBpkb k
	on k.CompanyCode = a.CompanyCode
   and k.CustomerCode = b.CustomerCode
   and k.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join svMstCustomerVehicle l
	on l.CompanyCode = a.CompanyCode
   and l.ChassisCode = a.ChassisCode
   and l.ChassisNo = a.ChassisNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and b.CustomerCode = @CustomerCode
   and a.ChassisCode + convert(varchar, a.ChassisNo) = @Chassis

GO

IF OBJECT_ID('uspfn_CsGetStnkExt') is not null
	drop proc dbo.uspfn_CsGetStnkExt
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_CsGetStnkExt]
	@CompanyCode  varchar(20),
	@CustomerCode varchar(20),
	@Chassis      varchar(50)
as

--select @CompanyCode = '6006406', @CustomerCode = '1000581', @Chassis = 'MHYGDN42VBJ352996'
select a.CompanyCode     
	 , a.BranchCode
	 , b.CustomerCode
	 , c.CustomerName
	 , a.ChassisCode + convert(varchar, a.ChassisNo) Chassis
	 , a.EngineCode + convert(varchar, a.EngineNo) Engine
	 , a.SalesModelCode
	 , SalesModel = a.SalesModelCode + '-' + (
					select SalesModelDesc
					  from omMstModel
					 where CompanyCode = a.CompanyCode
					   and SalesModelCode = a.SalesModelCode
	   )
	 , a.SalesModelYear
	 , a.ColourCode
	 , Colour = a.ColourCode + '-' + (
					select RefferenceDesc1 
					 from omMstRefference 
					where CompanyCode = a.CompanyCode
					  and RefferenceCode = a.ColourCode
					  and RefferenceType = 'COLO'
	   )
	 , isnull(k.BpkbDate,  g.BPKDate) BpkbDate
     , case when m.STNKInDate = '1900-01-01 00:00:00.000' then null else m.STNKInDate end StnkDate
	 , g.BPKDate
	 , h.isLeasing as IsLeasing
	 , h.LeasingCo
	 , isnull(h.isLeasing, 0) as IsLeasing
	 , case isnull(h.isLeasing, 0) when 0 then 'Tunai' else 'Leasing' end as Category
	 , i.CustomerName as LeasingName
	 , h.Installment
	 , convert(varchar, isnull(h.Installment, 0)) + ' Month' as Tenor
     , left(c.Address1, 40) as Address
     , c.PhoneNo
     , h.Salesman
     , j.EmployeeName as SalesmanName
     , k.IsStnkExtend
	 , k.Ownership
     , isnull(k.StnkExpiredDate, dateadd(year, 1, isnull(k.StnkDate, g.BPKDate)))  as StnkExpiredDate
     , k.ReqKtp
     , k.ReqStnk
     , k.ReqBpkb
     , k.ReqSuratKuasa
     , k.Comment, k.Additional, k.Status
     , (case k.Status when 1 then 'Finish' else 'In Progress' end) as StatusInfo
     , case (isnull(k.Chassis, '')) when '' then 1 else 0 end as IsNew
     , l.PoliceRegNo
	 , k.Ownership
  from omTrSalesInvoiceVin a    
  left join omTrSalesInvoice b    
	on b.CompanyCode = a.CompanyCode    
   and b.BranchCode = a.BranchCode    
   and b.InvoiceNo = a.InvoiceNo    
  left join gnMstCustomer c
	on c.CompanyCode = a.CompanyCode    
   and c.CustomerCode = b.CustomerCode
  left join omTrSalesDODetail d
	on d.CompanyCode = a.CompanyCode    
   and d.BranchCode = a.BranchCode
   and d.ChassisCode = a.ChassisCode
   and d.ChassisNo = a.ChassisNo
 -- left join CsCustomerVehicle e
	--on e.CompanyCode = a.CompanyCode    
 --  and e.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join omTrSalesDO f
	on f.CompanyCode = d.CompanyCode    
   and f.BranchCode = d.BranchCode
   and f.DONo = d.DONo
  left join omTrSalesBPK g
	on g.CompanyCode = f.CompanyCode    
   and g.BranchCode = f.BranchCode
   and g.DONo = f.DONo
  left join omTrSalesSO h
	on h.CompanyCode = g.CompanyCode    
   and h.BranchCode = g.BranchCode
   and h.SONo = g.SONo
  left join gnMstCustomer i
	on i.CompanyCode = h.CompanyCode
   and i.CustomerCode = h.LeasingCo
  left join HrEmployee j
	on j.CompanyCode = h.CompanyCode
   and j.EmployeeID = h.Salesman
  left join CsStnkExt k
	on k.CompanyCode = a.CompanyCode
   and k.CustomerCode = b.CustomerCode
   and k.Chassis = a.ChassisCode + convert(varchar, a.ChassisNo)
  left join svMstCustomerVehicle l
	on l.CompanyCode = a.CompanyCode
   and l.ChassisCode = a.ChassisCode
   and l.ChassisNo = a.ChassisNo
  left join
	 ( 
	   select distinct 
	     CompanyCode
	   , BranchCode
	   , ChassisCode
	   , ChassisNo
	   , STNKInDate 
	   from omTrSalesSPKDetail
	 ) m
    on m.CompanyCode = a.CompanyCode
   and m.BranchCode = a.BranchCode
   and m.ChassisCode = a.ChassisCode
   and m.ChassisNo = a.ChassisNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and b.CustomerCode = @CustomerCode
   and a.ChassisCode + convert(varchar, a.ChassisNo) = @Chassis
GO

IF NOT EXISTS(select top 1 * from information_schema.columns where column_name = 'Reason' and table_name = 'CsTDayCall')
BEGIN
	ALTER TABLE CsTDayCall
	add Reason varchar(50)
END
GO

IF (OBJECT_ID('CsLkuTDaysCallView') IS NOT NULL)
DROP VIEW [dbo].[CsLkuTDaysCallView]
GO

CREATE view [dbo].[CsLkuTDaysCallView]
as
select a.CompanyCode
     , a.BranchCode
	 , a.CustomerCode
	 , a.CustomerName
	 , a.Address 
	 , a.PhoneNo
	 , a.HPNo
	 , AddPhone1 = isnull(a.AddPhone1, '-')
	 , AddPhone2 = isnull(a.AddPhone2, '-')
	 , CreatedDate = c.CreatedDate
	 , Outstanding = (
			case
				when isnull(c.CustomerCode, '') = '' then 'Y'
				else 'N'
			end
	   )
     , a.BirthDate,
	   b.CarType + ' - ' + mo.[SalesModelDesc] CarType, 
	   b.Color + ' - ' + col.[RefferenceDesc1] [Color]
     --, b.CarType
     --, b.Color
     , b.PoliceRegNo
     , b.Chassis
     , b.Engine 
     , b.SONo
     , b.SalesmanCode
     , b.SalesmanName
     , b.SalesmanName as Salesman
     , a.ReligionCode
	 , b.SalesModelCode
	 , b.SalesModelYear
	 , b.DODate
	 , b.BPKDate
     , case c.IsDeliveredA when 1 then 'Ya' else 'Tidak' end IsDeliveredA
     , case c.IsDeliveredB when 1 then 'Ya' else 'Tidak' end IsDeliveredB
     , case c.IsDeliveredC when 1 then 'Ya' else 'Tidak' end IsDeliveredC
     , case c.IsDeliveredD when 1 then 'Ya' else 'Tidak' end IsDeliveredD
     , case c.IsDeliveredE when 1 then 'Ya' else 'Tidak' end IsDeliveredE
     , case c.IsDeliveredF when 1 then 'Ya' else 'Tidak' end IsDeliveredF
     , case c.IsDeliveredG when 1 then 'Ya' else 'Tidak' end IsDeliveredG
     , c.Comment
	 , c.Additional
	 , c.Status
	 , b.DeliveryDate
	 , d.LookUpValueName Religion
	 , c.Reason
  from CsCustomerView a
  left join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.CustomerCode = a.CustomerCode
  left join CsTDayCall c
    on c.CompanyCode = b.CompanyCode
   and c.CustomerCode = b.CustomerCode
   left join gnMstLookUpDtl d ON  (d.CodeID='RLGN' and d.CompanyCode = a.CompanyCode and d.LookUpValue=a.ReligionCode)
    LEFT OUTER JOIN [dbo].[omMstRefference] col on (b.CompanyCode=col.CompanyCode and b.Color = col.[RefferenceCode] and col.[RefferenceType]='COLO')
	LEFT OUTER JOIN [dbo].[omMstModel] mo on (b.CompanyCode=mo.CompanyCode and b.Cartype = mo.[SalesModelCode])

GO


IF(OBJECT_ID('CsLkuStnkExtensionView') is not null)
	drop view dbo.CsLkuStnkExtensionView
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

create view [dbo].[CsLkuStnkExtensionView]
as
select a.CompanyCode
     , a.BranchCode
	 , a.CustomerCode
	 , a.CustomerName 
	 , b.Chassis
	 , b.Engine
	 , b.SalesModelCode
	 , b.SalesModelYear
	 , b.ColourCode
	 , b.DODate
	 , BpkbDate = isnull(c.BpkbDate, b.BPKDate) 
	 , StnkDate = isnull(c.StnkDate, b.BPKDate) 
	 , b.BPKDate
	 , b.IsLeasing
	 , b.LeasingCo
	 , b.Installment
	 , LeasingDesc = case isnull(b.isLeasing, 0) when 0 then 'Cash' else 'Leasing' end
	 , LeasingName = case isnull(b.isLeasing, 0) when 0 then '-' else b.LeasingName end
	 , Tenor = case isnull(b.isLeasing, 0) when 0 then '-' else (convert(varchar, isnull(b.Installment, 0)) + ' Month') end
	 , Address = isnull(a.Address, '-')
	 , PhoneNo = isnull(a.PhoneNo, '-')
	 , HpNo = isnull(a.HpNo, '-')
	 , AddPhone1 = isnull(a.AddPhone1, '-')
	 , AddPhone2 = isnull(a.AddPhone2, '-')
	 , SalesmanCode = isnull(b.SalesmanCode, '-')
	 , SalesmanName = isnull(b.SalesmanName, '-')
	 , IsStnkExtend = isnull(c.IsStnkExtend, 0)
	 , InputDate = c.CreatedDate
	 , Outstanding = (
			case
				when isnull(c.CustomerCode, '') = '' then 'Y'
				when c.Ownership = 0 then 'N'
				when isnull(c.Ownership, '') = '' or c.Ownership = 0 then 'N'
				else 'N'
			end
	   )
	 , Category = ( 
			case 
				 when isnull(b.isLeasing, 0) = 0 then 'Tunai' 
				 else 'Leasing'
			end
	   )
	 , Salesman = isnull(b.SalesmanName, '-')
	 , c.StnkExpiredDate
	 , c.ReqKtp
	 , c.ReqStnk
	 , c.ReqBpkb
	 , c.ReqSuratKuasa
	 , c.Comment
	 , c.Additional
	 , c.Status
	 , c.Ownership
	 , StatusInfo = ''
	 , b.PoliceRegNo
  from CsCustomerView a
  left join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.CustomerCode = a.CustomerCode
  left join CsStnkExt c
    on c.CompanyCode = b.CompanyCode
   and c.CustomerCode = b.CustomerCode

GO


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


GO

IF(OBJECT_ID('uspfn_CsDataMonitoring') is not null)
	drop proc dbo.uspfn_CsDataMonitoring
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_CsDataMonitoring '2015-01-01', '2015-03-01', 3
CREATE procedure [dbo].[uspfn_CsDataMonitoring]
	@DateInit datetime,
	@DateReff datetime,
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
	DateInput  datetime,
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
     , convert(char(10), dateInput, 120) dateInput
     , sum(isnull(C3DaysCall, 0)) C3DaysCall
     , sum(isnull(CsBirthday, 0)) CsBirthday
     , sum(isnull(CsCustBpkb, 0)) CsCustBpkb
     , sum(isnull(CsStnkExt, 0)) CsStnkExt
  from @t_data2
 group by DealerCode, dateInput
 order by DealerCode, dateInput

GO
IF( OBJECT_ID('uspfn_CsDataTDaysCallDO') is not null)
	drop proc dbo.uspfn_CsDataTDaysCallDO
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
					--and convert(varchar(7), CreatedDate, 121) = convert(varchar(4),@year) + '-' + convert(varchar(2), @month)
			 ), 0)
 from r
), t as (

select s.CompanyCode
     , s.DoMonth
	 , s.BranchCode
	 , sum(s.DoData) as DoData
	 , sum(s.TDaysCallData) as TDaysCallData
  from s
  where s.BranchCode = @BranchCode
   and Year(s.DoDate) = @Year
   and Month(s.DoDate) = @Month
 group by s.CompanyCode, s.BranchCode, s.DoMonth
)
select b.DealerCode CompanyCode
     , t.DoMonth
	 , b.OutletCode BranchCode
	 , isnull(DoData, 0) DoData
	 , isnull(TDaysCallData, 0) TDaysCallData
	 , b.OutletAbbreviation BranchName
	 , TDaysCallByInput = isnull((
	       select count(x.Chassis) 
		     from CsTDayCall x
			 left join CsCustomerVehicleView y
			   on y.CompanyCode = x.CompanyCode
			  and y.CustomerCode = x.CustomerCode
			  and y.Chassis = x.Chassis
			where 1 = 1
			  and y.BranchCode = @BranchCode
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

IF (OBJECT_ID('uspfn_CsRptBPKBReminder') is not null)
	DROP PROC dbo.uspfn_CsRptBPKBReminder
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_CsRptBPKBReminder '6006400131', '', ''
CREATE proc [dbo].[uspfn_CsRptBPKBReminder]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
as
begin
	select a.InputDate, a.CustomerCount, isnull(b.InputByCRO, 0) InputByCRO, isnull(c.Unreachable, 0) Unreachable, ((convert(numeric(5,2), isnull(b.InputByCRO, 0)) / a.CustomerCount) * 100) Percentation
	  from (
			 select left(InputDate, 11) InputDate, CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuBpkbReminderView
			 group by left(InputDate, 11), CompanyCode, BranchCode
		   ) a
 left join (
			 select left(InputDate, 11) InputDate, CompanyCode, BranchCode, count(CustomerCode) InputByCRO from CsLkuBpkbReminderView
			 where BpkbPickUp is not null
			 group by left(InputDate, 11), CompanyCode, BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.InputDate = b.InputDate
 left join (
			 select left(InputDate, 11) InputDate, CompanyCode, BranchCode, count(CustomerCode) Unreachable from CsLkuBpkbReminderView
			 where Reason is not null
			 group by left(InputDate, 11), CompanyCode, BranchCode
		   ) c
		on a.CompanyCode = c.CompanyCode and a.BranchCode = c.BranchCode and a.InputDate = c.InputDate
	 where a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END
	   --and a.InputDate BETWEEN @DateFrom AND @DateTo


end
GO
