create procedure [dbo].[uspfn_CsDashSummaryNotOutstanding]
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

update @t_rem set RemValue = (select count(CustomerCode) from CsLkuTDayCallView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'N' and DODate >= (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')) where RemCode = 'REM3DAYSCALL'
update @t_rem set RemValue = (select count(CustomerCode) from CsLkuStnkExtView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'N' and StnkExpiredDate >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')) where RemCode = 'REMSTNKEXT'
update @t_rem set RemValue = (select count(CustomerCode) from CsLkuBpkbView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'N' and BpkbDate >= (select RemDate from @t_rem where RemCode = 'REMBPKB')) where RemCode = 'REMBPKB'
update @t_rem set RemValue = (select count(CustomerCode) from CsLkuBirthdayView where CompanyCode = @CompanyCode and BranchCode like @BranchCode and OutStanding = 'N' ) where RemCode = 'REMBDAYSCALL'

select a.RemCode, a.RemDate, a.RemValue, b.SettingLink1 as ControlLink
  from @t_rem a
  join CsSettings b
    on b.CompanyCode = @CompanyCode
   and b.SettingCode = a.RemCode


--select * from CsSettings



--go
--exec uspfn_CsDashSummary @CompanyCode='6006406', @BranchCode='6006406'

--exec sp_executesql N'exec uspfn_CsDashSummary @CompanyCode=@p0, @BranchCode=p1',N'@p0 nvarchar(7),@p1 nvarchar(7)',@p0=N'6006406',@p1=N'6006406'