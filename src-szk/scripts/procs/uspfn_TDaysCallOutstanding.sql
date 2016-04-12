
go
if object_id('uspfn_TDaysCallOutstanding') is not null
	drop procedure uspfn_TDaysCallOutstanding

go
create procedure uspfn_TDaysCallOutstanding
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20)
as

--set @CompanyCode = '6006406'

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

begin try
	insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')
end try
begin catch
end catch

-- REMBDAYSCALL
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBDAYSCALL'), '0')
insert into @t_rem (RemCode, RemDate) values('REMBDAYSCALL', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

-- REMHOLIDAYS
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMHOLIDAYS'), '0')
insert into @t_rem (RemCode, RemDate) values('REMHOLIDAYS', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

-- REMSTNKEXT
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMSTNKEXT'), '0')
insert into @t_rem (RemCode, RemDate) values('REMSTNKEXT', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

-- REMBPKB
set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBPKB'), '0')
insert into @t_rem (RemCode, RemDate) values('REMBPKB', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')
   

declare @TDaysCallCutOffPeriod varchar(20);
declare @TDaysCallSettingParam3 varchar(20);

set @TDaysCallCutOffPeriod = ( select top 1 a.SettingParam1 from CsSettings a where a.CompanyCode = @CompanyCode);
set @TDaysCallSettingParam3 = ( select top 1 a.SettingParam3 from CsSettings a where a.CompanyCode = @CompanyCode);

declare @CurrentMonth tinyint;
declare @PreviousMonth tinyint;
declare @NextMonth tinyint;
declare @CurrentDay tinyint;
declare @DateComparison datetime;

if @TDaysCallSettingParam3 != 'CUTOFF'
begin
	  select a.CustomerCode
		   , b.CustomerName
		   , a.Chassis
		   , isnull(a.PoliceRegNo, '') as PoliceRegNo
		   , a.DODate
		from CsCustomerVehicleView a
	   inner join CsCustomerView b 
		  on b.CompanyCode = a.CompanyCode
		 and b.CustomerCode = a.CustomerCode
		left join CsTdayCall c
		  on c.CompanyCode = a.CompanyCode
		 and c.CustomerCode = a.CustomerCode
		 and c.Chassis = a.Chassis
	   where a.CompanyCode like @CompanyCode
 		 and a.BranchCode like @BranchCode
		 and isnull(c.Chassis, '') = ''
		 and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')
end
else
begin
	select a.CustomerCode
		   , b.CustomerName
		   , a.Chassis
		   , isnull(a.PoliceRegNo, '') as PoliceRegNo
		   , a.DODate
		from CsCustomerVehicleView a
	   inner join CsCustomerView b 
		  on b.CompanyCode = a.CompanyCode
		 and b.CustomerCode = a.CustomerCode
		left join CsTdayCall c
		  on c.CompanyCode = a.CompanyCode
		 and c.CustomerCode = a.CustomerCode
		 and c.Chassis = a.Chassis
	   where a.CompanyCode like @CompanyCode
 		 and a.BranchCode like @BranchCode
		 and isnull(c.Chassis, '') = ''
		 and a.DoDate >=  convert(datetime, @TDaysCallCutOffPeriod);
end


go
exec uspfn_TDaysCallOutstanding '6021406', '602140601'