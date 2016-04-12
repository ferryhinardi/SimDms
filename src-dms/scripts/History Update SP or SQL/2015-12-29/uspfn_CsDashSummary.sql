/*
	[uspfn_CsDashSummary] '6006400001', '6006400106'
*/
ALTER procedure [dbo].[uspfn_CsDashSummary]
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

exec uspfn_SyncCsCustomerView

exec uspfn_SyncCsCustomerVehicleView

set @CurrDate = getdate()

-- REMDELIVERY
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMDELIVERY'), '0')
insert into @t_rem (RemCode, RemDate) values('REMDELIVERY', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )

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
			from CsCustomerView a
				left join CsCustomerVehicleView b
				on b.CompanyCode = a.CompanyCode
				and b.BranchCode = a.BranchCode
				and b.CustomerCode = a.CustomerCode
			where a.CompanyCode = @CompanyCode
			and a.BranchCode = @BranchCode
			and b.BPKDate >=  (SELECT SettingParam1 FROM CsSettings WHERE SettingCode = 'REMDELIVERY')
			and (b.DeliveryDate < b.BPKDate OR b.DeliveryDate IS NULL OR b.DeliveryDate = '1900-01-01 00:00:00.000')
		)
 where RemCode = 'REMDELIVERY';

update @t_rem set RemValue = (
						select count(a.CustomerCode)
						  from CsLkuStnkExtensionView a
						 where a.CompanyCode like @CompanyCode
						   and a.BranchCode like @BranchCode
						   and a.Outstanding = 'Y'
						   and (datediff(month, isnull(a.StnkDate, a.BpkDate), getdate()) = 11 or datediff(month,getdate(), isnull(a.StnkExpiredDate,getdate())) = 1)
						   --and isnull(a.StnkExpiredDate, isnull(a.StnkDate, a.BpkDate)) >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')
				)
 where RemCode = 'REMSTNKEXT';
 /*
 update @t_rem set RemValue = ( 
		select count(a.CustomerCode)
		from CsLkuTDaysCallView a
		where a.CompanyCode = @CompanyCode
			and a.BranchCode = @BranchCode
			and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')
			and a.Outstanding = 'Y'
	)
*/
update @t_rem set RemValue = ( 
		select count(a.CustomerCode)
			from CsLkuTDaysCallView a
			where a.CompanyCode like @CompanyCode
			and a.BranchCode like @BranchCode
			and a.DeliveryDate IS NOT NULL 
			and a.DeliveryDate <> '1900-01-01 00:00:00.000' 
			and a.DeliveryDate > a.BPKDate
			and a.BPKDate > (select top 1 SettingParam1 from CSSettings where SettingCode = 'REM3DAYSCALL')
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
						  from CsLkuBirthdayView a
						 where a.CompanyCode like @CompanyCode
						   and a.BranchCode like @BranchCode
						   and a.Outstanding = 'Y'
						   and datepart(month, a.CustomerBirthDate) = datepart(month, getdate()) 
						   --and datepart(month, a.BirthDate) >= @PreviousMonth
						   --and datepart(month, a.BirthDate) <= @CurrentMonth
				)
 where RemCode = 'REMBDAYSCALL';

select a.RemCode, a.RemDate, a.RemValue, b.SettingLink1 as ControlLink
  from @t_rem a
  join CsSettings b
    on b.CompanyCode = @CompanyCode
   and b.SettingCode = a.RemCode

