
go
if object_id('uspfn_StnkExtensionOutstanding') is not null
	drop procedure uspfn_StnkExtensionOutstanding

go
create procedure uspfn_StnkExtensionOutstanding
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
insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

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
   
declare @CurrentMonth tinyint;
declare @PreviousMonth tinyint;
declare @NextMonth tinyint;
declare @CurrentDay tinyint;
declare @DateComparison datetime;
  
set @DateComparison = (select RemDate from @t_rem where RemCode = 'REMBDAYSCALL');
set @CurrentMonth = datepart(month, @DateComparison);
set @NextMonth = @CurrentMonth + 1;
set @PreviousMonth = @CurrentMonth - 1;

select a.CustomerCode
     , b.CustomerName
	 , StnkExpiredDate = isnull(c.StnkExpiredDate, isnull(c.StnkDate, a.BpkDate)) 
	 , a.PoliceRegNo
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

go
exec uspfn_StnkExtensionOutstanding '6156401', '615640100'