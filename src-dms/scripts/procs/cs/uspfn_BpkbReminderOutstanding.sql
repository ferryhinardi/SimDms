
go
if object_id('uspfn_BpkbReminderOutstanding') is not null
	drop procedure uspfn_BpkbReminderOutstanding

go
create procedure uspfn_BpkbReminderOutstanding
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
	   , a.BPKDate
	   , a.PoliceRegNo
	   , a.Chassis
	from CsCustomerVehicleView a
   inner join CsCustomerView b 
	  on b.CompanyCode = a.CompanyCode
	 and b.CustomerCode = a.CustomerCode
	left join CsCustBpkb c
	  on c.CompanyCode = a.CompanyCode
	 and c.CustomerCode = a.CustomerCode
	 and c.Chassis = a.Chassis
   where a.CompanyCode like @CompanyCode
	 and a.BranchCode like @BranchCode
	 and ( 
			isnull(c.Chassis, '') = ''
			or 
			isnull(c.BpkbReadyDate, '') = ''
			or
			isnull(isnull(c.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=c.CustomerCode and (x.IsDeleted = 0 or x.IsDeleted is null) order by x.RetrievalEstimationDate desc)), '') = ''
			or
			isnull(isnull(c.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=c.CustomerCode order by x.RetrievalEstimationDate desc)), '') != '' and isnull(c.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=c.CustomerCode and (x.IsDeleted = 0 or x.IsDeleted is null) order by x.RetrievalEstimationDate desc)) < getdate() 
			or
			isnull(c.BpkbReadyDate, '') != '' and isnull(isnull(c.BpkbPickUp, (select top 1 x.RetrievalEstimationDate from CsBpkbRetrievalInformation x where x.CompanyCode=a.CompanyCode and x.CustomerCode=c.CustomerCode and (x.IsDeleted = 0 or x.IsDeleted is null) order by x.RetrievalEstimationDate desc)), '') = ''
		 )
	 and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')








go
exec uspfn_BpkbReminderOutstanding '6156401', '615640100'