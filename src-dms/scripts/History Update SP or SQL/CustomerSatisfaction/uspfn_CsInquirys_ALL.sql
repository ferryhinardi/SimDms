update CsSettings set SettingParam1 = '2015-01-01', SettingParam2 = 'DAY', SettingParam3 = 'CUTOFF' where settingcode = 'REMSTNKEXT'
GO

DROP PROCEDURE [dbo].[uspfn_CsDashSummary]
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
declare @Minus2Days int;  
   
set @Minus2Days = -1;  
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
  
if getdate() > DATEADD(day, -day(getdate()), getdate()) and GETDATE() < DATEADD(day, -day(getdate()) + 7, getdate())  
 set @Minus2Days = 1;  
  
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
   and b.BPKDate >=  (select RemDate from @t_rem where RemCode = 'REMDELIVERY')
   and (b.DeliveryDate IS NULL OR b.DeliveryDate = '1900-01-01 00:00:00.000')  
  )  
 where RemCode = 'REMDELIVERY';  
  
update @t_rem set RemValue = (  
      select count(a.CustomerCode)  
        from CsLkuStnkExtensionView a  
       where a.CompanyCode like @CompanyCode  
         and a.BranchCode like @BranchCode  
         and a.Outstanding = 'Y'
         and month(isnull(a.StnkExpiredDate, a.BPKDate)) = month(getdate()) + 1
		 and a.BPKDate >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')
         --and isnull(a.StnkExpiredDate, isnull(a.StnkDate, a.BpkDate)) >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')  
    )  
 where RemCode = 'REMSTNKEXT';  
   
update @t_rem set RemValue = (   
  select count(a.CustomerCode)  
   from CsLkuTDaysCallView a  
   where a.CompanyCode like @CompanyCode  
   and a.BranchCode like @BranchCode  
   and a.DeliveryDate IS NOT NULL   
   and a.DeliveryDate <> '1900-01-01 00:00:00.000'
   and (
		(
			convert(varchar, dateadd(day, 3, a.DeliveryDate), 112) <= convert(varchar, getdate(), 112) 
		AND 
			MONTH(a.DeliveryDate) = MONTH(getdate())
		)
   OR 
		(
			day(getdate()) <= 7
		AND
			MONTH(a.DeliveryDate) = MONTH(getdate()) - 1
		)
	   )
   and convert(varchar, a.DeliveryDate, 112) >= convert(varchar, a.BPKDate, 112)
   and a.BPKDate >= (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')
   and a.Outstanding = 'Y'  
 )  
 where RemCode = 'REM3DAYSCALL';  
  
  update @t_rem set RemValue = (  
      select count(a.CustomerCode)  
        from CsLkuBpkbReminderView a  
       where a.CompanyCode like @CompanyCode  
         and a.BranchCode like @BranchCode
		 and a.Outstanding = 'Y'
         and (case when isnull(isLeasing, 0) = 0 then BpkbPickUp else null end) is null
         and (a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB'))
    )  
  where RemCode = 'REMBPKB';  
    
  set @DateComparison = (select RemDate from @t_rem where RemCode = 'REMBDAYSCALL');  
  set @CurrentMonth = datepart(month, @DateComparison);  
  set @NextMonth = @CurrentMonth + 1;  
  set @PreviousMonth = @CurrentMonth - 1;  
  
 update @t_rem set RemValue = (  
      select count(a.CustomerCode)  
        from CsLkuBirthdayView a  
       where a.CompanyCode like @CompanyCode  
         and a.BranchCode like @BranchCode  
         and a.Outstanding = 'Y'  
		 and (
				right(convert(varchar, a.CustomerBirthDate, 112), 4)
				between right(convert(varchar, dateadd(day, -day(getdate()) - @Minus2Days, getdate()), 112), 4)
					and right(convert(varchar, dateadd(day, -day(getdate()), dateadd(month, 1, getdate())), 112), 4)
			   or
			   (
			   	  (select top 1 customercode from CsCustomerVehicleView where convert(varchar, BPKDate, 112) >= convert(varchar, dateadd(day, -day(getdate()) - @Minus2Days, getdate()), 112) and CustomerCode = a.CustomerCode) is not null
			   	  and MONTH(a.CustomerBirthDate) = MONTH(getdate()) - 1
			   )
			 )
         --and datepart(month, a.CustomerBirthDate) = datepart(month, getdate())   
    )  
 where RemCode = 'REMBDAYSCALL';  
  
select a.RemCode, a.RemDate, a.RemValue, b.SettingLink1 as ControlLink  
  from @t_rem a  
  join CsSettings b  
    on b.CompanyCode = @CompanyCode  
   and b.SettingCode = a.RemCode
GO

DROP PROCEDURE [dbo].[uspfn_CsDashSummaryWithSync]
GO

CREATE procedure [dbo].[uspfn_CsDashSummaryWithSync]
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20)
as

exec uspfn_SyncCsCustomerView

exec uspfn_SyncCsCustomerVehicleView

exec uspfn_CsDashSummary @CompanyCode, @BranchCode
--set @CompanyCode = '6006406'

GO

DROP PROCEDURE [dbo].[uspfn_CsLkuCustBday]
GO

CREATE procedure [dbo].[uspfn_CsLkuCustBday]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@OutStanding char(1), 
	@CustomerCode varchar(13),
	@CustomerName varchar(50)
as

begin
	declare @Minus2Days int;
	set @Minus2Days = -1;

	if getdate() > DATEADD(day, -day(getdate()), getdate()) and GETDATE() < DATEADD(day, -day(getdate()) + 7, getdate())
		set @Minus2Days = 1;

	select *
		from CsLkuBirthdayView a
		where a.CompanyCode like @CompanyCode
		and a.BranchCode like @BranchCode
		and a.Outstanding = 'Y'
		and right(convert(varchar, a.CustomerBirthDate, 112), 4)
		between right(convert(varchar, dateadd(day, -day(getdate()) - @Minus2Days, getdate()), 112), 4)
			and right(convert(varchar, dateadd(day, -day(getdate()), dateadd(month, 1, getdate())), 112), 4)

	--declare @Param1 varchar(25);
	--set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REM3DAYSCALL'), '0');
	
		--select *
		--  from CsCustomerVehicleView a
		-- inner join CsCustomerView b 
		--    on b.CompanyCode = a.CompanyCode
		--   and b.CustomerCode = a.CustomerCode
		--  left join CsTdayCall c
		--    on c.CompanyCode = a.CompanyCode
		--   and c.CustomerCode = a.CustomerCode
		--   and c.Chassis = a.Chassis
		-- where a.CompanyCode like @CompanyCode
		--   and a.BranchCode like @BranchCode
		--   and isnull(c.Chassis, '') = ''
		--   and a.DoDate >=  ( left(convert(varchar, dateadd(month, -convert(int, @Param1), getdate()), 112), 6) + '01' )








	--declare @BottomLimit int;
	--declare @UpperLimit int;
	--declare @CurrentMonth int;
	--declare @Setting int;

	--set @Setting = (select top 1 SettingParam1 from CsSettings where SettingCode='REMBDAY');
	--set @CurrentMonth = datepart(month, getdate());

	--set @BottomLimit = @CurrentMonth - @Setting;
	--set @UpperLimit = @CurrentMonth;

	--if(@BottomLimit <= 0) 
	--	set @BottomLimit = 12 + @BottomLimit;

	--if @UpperLimit >= 12 
	--	set @UpperLimit = @UpperLimit - 12;


	--if(@BottomLimit <= 0) 
	--	set @BottomLimit = 12 + @BottomLimit;

	--if @UpperLimit >= 12 
	--	set @UpperLimit = @UpperLimit - 12;

	--if @CustomerName is null or @CustomerName = ''
	--	set @CustomerName = '%';
	--else
	--	set @CustomerName = '%' + @CustomerName + '%';

	--if @CustomerCode is null or @CustomerCode = ''
	--	set @CustomerCode = '%';
	--else
	--	set @CustomerCode = '%' + @CustomerCode + '%';

	--select distinct a.*
	--  from CsLkuBirthdayView a
	-- where a.CompanyCode = @CompanyCode
	--   and a.BranchCode = @BranchCode
	--   and a.CustomerCode like @CustomerCode
	--   and a.CustomerName like @CustomerName
	--   and a.Outstanding = @OutStanding
	--    and (
	--			DATEPART(month, a.CustomerBirthDate) >= @BottomLimit
	--			or
	--			DATEPART(month, a.CustomerBirthDate) <= @UpperLimit
	--		)

	 --order by datepart(month, a.CustomerBirthDate) asc
end
GO

DROP PROCEDURE [dbo].[uspfn_CsDlvryOutstanding]
GO

CREATE PROCEDURE [dbo].[uspfn_CsDlvryOutstanding]
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20)
AS
BEGIN
	SELECT a.CustomerCode, a.CustomerName, b.Chassis, b.PoliceRegNo, b.BPKDate
	FROM CsCustomerView a
	  LEFT JOIN CsCustomerVehicleView b
		on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	   and b.CustomerCode = a.CustomerCode
	WHERE a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND b.BPKDate >=  (SELECT SettingParam1 FROM CsSettings WHERE SettingCode = 'REMDELIVERY')
	AND (b.DeliveryDate IS NULL OR b.DeliveryDate = '1900-01-01 00:00:00.000')
	ORDER BY b.BPKDate DESC
END

GO

DROP PROCEDURE [dbo].[uspfn_CsLkuFeedback2]
GO

CREATE PROCEDURE [dbo].[uspfn_CsLkuFeedback2]
	@CompanyCode VARCHAR(20),
	@BranchCode VARCHAR(20),
	@OutStanding CHAR(1),
	@CustomerName VARCHAR(100) = '',
	@VinNo VARCHAR(50) = '',
	@PolReg VARCHAR(15) = ''
AS
BEGIN
	IF @CustomerName IS NULL SET @CustomerName = ''
	IF @VinNo IS NULL SET @VinNo = ''
	IF @PolReg IS NULL SET @PolReg = ''

	SELECT *
	FROM CsLkuFeedbackView 
	WHERE CompanyCode = @CompanyCode 
		AND BranchCode = @BranchCode 
		AND OutStanding = @OutStanding
		AND CustomerName LIKE '%'+@CustomerName+'%'
		AND Chassis LIKE '%'+@VinNo+'%'
		AND PoliceRegNo LIKE '%'+@PolReg+'%'
END

GO

DROP PROCEDURE [dbo].[uspfn_CsInqTDaysCall]
GO

CREATE procedure [dbo].[uspfn_CsInqTDaysCall]
 @CompanyCode nvarchar(20),  
 @BranchCode varchar(20),  
 @DateFrom datetime,  
 @DateTo datetime,  
 @Outstanding char(1),  
 @Status varchar(15)  
as  
begin  
 declare @CurrDate datetime, @Param1 as varchar(20)  
 declare @t_rem as table  
 (  
  RemCode varchar(20),  
  RemDate datetime,  
  RemValue int  
 )  
 declare @TDaysCallCutOffPeriod varchar(20);  
 declare @TDaysCallSettingParam3 varchar(20);  
  
 set @TDaysCallCutOffPeriod = '';  
 set @TDaysCallSettingParam3 = '';  
  
 set @CurrDate = getdate()  
  
 -- REM3DAYSCALL  
 set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REM3DAYSCALL'), '0')  
 begin try  
  insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')  
 end try  
 begin catch  
  set @TDaysCallCutOffPeriod = ( select top 1 a.SettingParam1 from CsSettings a where a.CompanyCode = @CompanyCode and SettingCode = 'REM3DAYSCALL');  
  set @TDaysCallSettingParam3 = ( select top 1 a.SettingParam3 from CsSettings a where a.CompanyCode = @CompanyCode and SettingCode = 'REM3DAYSCALL');  
  insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', convert(datetime, @TDaysCallCutOffPeriod));  
 end catch  
  
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
   
 if @Status = 'Inquiry'   
 begin  
  select *  
    from CsLkuTDaysCallView a  
   where a.CompanyCode like @CompanyCode  
     and a.BranchCode like @BranchCode  
     and a.Outstanding = @Outstanding  
     and convert(varchar, a.CreatedDate, 112) between @DateFrom and @DateTo  
     and a.DeliveryDate is not null  
     and convert(varchar, a.DeliveryDate, 112) >= convert(varchar, a.BPKDate, 112)
     and a.DeliveryDate <> '1900-01-01 00:00:00.000'  
 end  
 else if @Status = 'Lookup'  
 begin  
  if @Outstanding = 'Y'  
  begin  
   if @TDaysCallSettingParam3 != 'CUTOFF'  
   begin  
    select *  
      from CsLkuTDaysCallView a  
     where a.CompanyCode like @CompanyCode  
       and a.BranchCode like @BranchCode  
       and a.BPKDate >=  (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')  --
       and a.DeliveryDate is not null  
       and a.DeliveryDate <> '1900-01-01 00:00:00.000'
	   and (
			(
				convert(varchar, dateadd(day, 3, a.DeliveryDate), 112) <= convert(varchar, getdate(), 112) 
			AND 
				MONTH(a.DeliveryDate) = MONTH(getdate())
			)
	   OR 
			(
				day(getdate()) <= 7
			AND
				MONTH(a.DeliveryDate) = MONTH(getdate()) - 1
			)
		   )
       and convert(varchar, a.DeliveryDate, 112) >= convert(varchar, a.BPKDate, 112)
       and a.Outstanding = 'Y'  
     order by a.DeliveryDate desc  
   end  
   else  
   begin  
    select *  
      from CsLkuTDaysCallView a  
     where a.CompanyCode like @CompanyCode  
       and a.BranchCode like @BranchCode  
       and a.DeliveryDate is not null  
       and a.DeliveryDate <> '1900-01-01 00:00:00.000'
	   and (
			(
				convert(varchar, dateadd(day, 3, a.DeliveryDate), 112) <= convert(varchar, getdate(), 112) 
			AND 
				MONTH(a.DeliveryDate) = MONTH(getdate())
			)
	   OR 
			(
				day(getdate()) <= 7
			AND
				MONTH(a.DeliveryDate) = MONTH(getdate()) - 1
			)
		   )
       and convert(varchar, a.DeliveryDate, 112) >= convert(varchar, a.BPKDate, 112)
       and a.BPKDate >= convert(datetime, @TDaysCallCutOffPeriod)  --
       and a.Outstanding = 'Y'  
     order by a.DeliveryDate desc  
   end  
  end  
  else  
  begin  
   select *  
     from CsLkuTDaysCallView a  
    where a.CompanyCode like @CompanyCode  
      and a.BranchCode like @BranchCode  
      and a.BPKDate >=  (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')  
      and a.DeliveryDate is not null
	  and (
		   (
				convert(varchar, dateadd(day, 3, a.DeliveryDate), 112) <= convert(varchar, getdate(), 112) 
		   AND 
				MONTH(dateadd(day, 3, a.DeliveryDate)) = MONTH(getdate())
		   )
		OR 
		   (
				day(getdate()) <= 7
		   AND
				MONTH(dateadd(day, 3, a.DeliveryDate)) = MONTH(getdate()) - 1
		   )
		  )
       and convert(varchar, a.DeliveryDate, 112) >= convert(varchar, a.BPKDate, 112)
      and a.DeliveryDate <> '1900-01-01 00:00:00.000'  
      and a.Outstanding = 'N'  
    order by a.DeliveryDate desc  
  end  
 end  
 end  
  
GO

DROP PROCEDURE [dbo].[uspfn_CsStnkExtension]
GO

CREATE procedure [dbo].[uspfn_CsStnkExtension]
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20),
	@IsStnkExtension bit,
	@DateFrom datetime,
	@DateTo datetime,
	@Outstanding char(1),
	@Status varchar(15)
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

	-- REMSTNKEXT
	set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMSTNKEXT'), '0')
	insert into @t_rem (RemCode, RemDate) values('REMSTNKEXT', case when len(@Param1)=10 then @Param1 else left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01' END )  

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

	if @Status = 'Inquiry'
	begin
		if isnull(@IsStnkExtension, '') = '' 
		begin
			select *
				 , Tenor = case isnull(b.isLeasing, 0) when 0 then '-' else (convert(varchar, isnull(b.Installment, 0)) + ' Month') end
				 , Address = isnull(a.Address, '-')
				 , PhoneNo = isnull(a.PhoneNo, '-')
				 , HpNo = isnull(a.HpNo, '-')
				 , AddPhone1 = isnull(a.AddPhone1, '-')
				 , AddPhone2 = isnull(a.AddPhone2, '-')
				 , SalesmanCode = isnull(b.SalesmanCode, '-')
				 , SalesmanName = isnull(b.SalesmanName, '-')
				 , IsStnkExtend = isnull(c.IsStnkExtend, 0)
			  from CsStnkExt c
		 left join CsCustomerVehicleView b
				on c.CompanyCode = b.CompanyCode
			   and c.CustomerCode = b.CustomerCode
			   and c.Chassis = b.Chassis
		 left join CsCustomerView a
				on b.CompanyCode = a.CompanyCode
			   and b.BranchCode = a.BranchCode
			   and b.CustomerCode = a.CustomerCode
			 where c.CompanyCode like @CompanyCode
			   and b.BranchCode like @BranchCode
			   and c.StnkExpiredDate >= @DateFrom
			   and c.StnkExpiredDate <= @DateTo
--			   and a.Outstanding = @Outstanding
		end
		else
		begin
			select *
				 , Tenor = case isnull(b.isLeasing, 0) when 0 then '-' else (convert(varchar, isnull(b.Installment, 0)) + ' Month') end
				 , Address = isnull(a.Address, '-')
				 , PhoneNo = isnull(a.PhoneNo, '-')
				 , HpNo = isnull(a.HpNo, '-')
				 , AddPhone1 = isnull(a.AddPhone1, '-')
				 , AddPhone2 = isnull(a.AddPhone2, '-')
				 , SalesmanCode = isnull(b.SalesmanCode, '-')
				 , SalesmanName = isnull(b.SalesmanName, '-')
				 , IsStnkExtend = isnull(c.IsStnkExtend, 0)
			  from CsStnkExt c
		 left join CsCustomerVehicleView b
				on c.CompanyCode = b.CompanyCode
			   and c.CustomerCode = b.CustomerCode
			   and c.Chassis = b.Chassis
		 left join CsCustomerView a
				on b.CompanyCode = a.CompanyCode
			   and b.BranchCode = a.BranchCode
			   and b.CustomerCode = a.CustomerCode
			 where c.CompanyCode like @CompanyCode
			   and b.BranchCode like @BranchCode
--			   and a.Outstanding = @Outstanding
			   and c.IsStnkExtend = @IsStnkExtension
			   and c.StnkExpiredDate >= @DateFrom
			   and c.StnkExpiredDate <= @DateTo
		end
	end
	else if @Status = 'Lookup'
	begin
		select *
		  from CsLkuStnkExtensionView a
		 where a.CompanyCode like @CompanyCode
		   and a.BranchCode like @BranchCode
		   and month(isnull(a.StnkExpiredDate, a.BPKDate)) = month(getdate()) + 1
		   --and year(a.StnkExpiredDate) >= year(a.StnkDate) --agar STNK mati tidak muncul lagi di list reminder perpanjang STNK, dicomment supaya cro bisa benerin data lama
		   and a.Outstanding = @Outstanding
		   and a.BPKDate >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')
	--datediff(month, isnull(a.StnkDate, a.BpkDate), getdate()) = 11 or datediff(month,getdate(), isnull(a.StnkExpiredDate,getdate())) =1)
	  order by case when a.Stnkexpireddate is null then a.BPKDate else a.Stnkexpireddate end
	end
 end
GO	

DROP PROCEDURE [dbo].[uspfn_CsInqBpkbReminder]
GO

CREATE procedure [dbo].[uspfn_CsInqBpkbReminder]
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20),
	@DateFrom datetime,
	@DateTo datetime,
	@Outstanding char(1),
	@Status varchar(15)
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


	  --update @t_rem set RemValue = (
			--			select count(a.CustomerCode)
			--			  from CsLkuBpkbReminderView a
			--			 where a.CompanyCode like @CompanyCode
			--			   and a.BranchCode like @BranchCode
			--			   and a.Outstanding = 'Y'
			--			   and (case when isnull(isLeasing, 0) = 0 then BpkbPickUp else null end) is null
			--			   and (a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB') or BpkbReadyDate is not null)
			--		)
	  --where RemCode = 'REMBPKB';

	  if @Status = 'Inquiry'
	  begin
			select*
			  from CsLkuBpkbReminderView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   ----and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')
			   and left(convert(varchar, a.BpkbReadyDate, 121), 10) >= @DateFrom
			   and left(convert(varchar, a.BpkbReadyDate, 121), 10) <=  @DateTo
			   --and a.DoDate >= @DateFrom --harusnya pake ini tapi label filter nya malah BPKB Ready Date
			   --and a.DoDate <=  @DateTo
			   and a.Outstanding = @Outstanding
	  end
	  else if @Status = 'Lookup'
	  begin
		  select *
			from CsLkuBpkbReminderView a
		   where a.CompanyCode like @CompanyCode
			 and a.BranchCode like @BranchCode
			 and a.Outstanding = @Outstanding
			 and (case when isnull(isLeasing, 0) = 0 then BpkbPickUp else null end) is null
			 and (a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB'))
			 order by BpkbReadyDate desc, BpkbPickUp desc, BPKDate desc
	  end
 end
GO

DROP PROCEDURE [dbo].[uspfn_CsInqCustomerBirthday]
GO

-- [uspfn_CsInqCustomerBirthday] '6006400001', '6006400106', 2015, 1, 12, 'N', 'Inquiry'
CREATE procedure [dbo].[uspfn_CsInqCustomerBirthday]
	@CompanyCode nvarchar(20),
	@BranchCode varchar(20),
	@Year int,
	@MonthFrom int,
	@MonthTo int,
	@Outstanding char(1),
	@Status varchar(10)
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

	-- REMBDAYSCALL
	set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REMBDAYSCALL'), '0')
	insert into @t_rem (RemCode, RemDate) values('REMBDAYSCALL', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

	declare @CurrentMonth tinyint;
	declare @PreviousMonth tinyint;
	declare @NextMonth tinyint;
	declare @CurrentDay tinyint;
	declare @DateComparison datetime;
	declare @Minus2Days int;
	
	set @Minus2Days = -1;
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
  
	  set @DateComparison = (select RemDate from @t_rem where RemCode = 'REMBDAYSCALL');
	  set @CurrentMonth = datepart(month, @DateComparison);
	  set @NextMonth = @CurrentMonth + 1;
	  set @PreviousMonth = @CurrentMonth - 1;

	if getdate() > DATEADD(day, -day(getdate()), getdate()) and GETDATE() < DATEADD(day, -day(getdate()) + 7, getdate())
		set @Minus2Days = 1;

	if @Status = 'Inquiry'
	begin
		if (rtrim(@Outstanding) = '') set @Outstanding = null;
		select a.*, dbo.typeofgiftToName(a.TypeOfGift,'|') as [GreetingBy]
	   	from CsLkuBirthdayView a
		where a.CompanyCode like @CompanyCode
			and a.BranchCode like @BranchCode
			and month(a.CustomerBirthDate) between @MonthFrom and @MonthTo
			and (a.PeriodOfYear is null or a.PeriodOfYear = @Year)
			and (@Outstanding is null or a.Outstanding = @Outstanding)
	end
	else if @Status = 'Lookup'
	begin
		if @Outstanding = 'Y'
		begin
			select *	
			  from CsLkuBirthdayView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.Outstanding = @Outstanding
			   and (
					   right(convert(varchar, a.CustomerBirthDate, 112), 4)
					   between right(convert(varchar, dateadd(day, -day(getdate()) - @Minus2Days, getdate()), 112), 4)
						   and right(convert(varchar, dateadd(day, -day(getdate()), dateadd(month, 1, getdate())), 112), 4)
					or
					(
						(select top 1 customercode from CsCustomerVehicleView where convert(varchar, BPKDate, 112) >= convert(varchar, dateadd(day, -day(getdate()) - @Minus2Days, getdate()), 112) and CustomerCode = a.CustomerCode) is not null
						and MONTH(a.CustomerBirthDate) = MONTH(getdate()) - 1
					)
				   )
			   --and datepart(month, a.CustomerBirthDate) >= @PreviousMonth
			   --and datepart(month, a.CustomerBirthDate) <= @CurrentMonth
		end
		else if @Outstanding = 'N'
		begin
			select *	
			  from CsLkuBirthdayView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.Outstanding = @Outstanding
			   and a.PeriodOfYear = @Year
			   and (
					   right(convert(varchar, a.CustomerBirthDate, 112), 4)
					   between right(convert(varchar, dateadd(day, -day(getdate()) - @Minus2Days, getdate()), 112), 4)
						   and right(convert(varchar, dateadd(day, -day(getdate()), dateadd(month, 1, getdate())), 112), 4)
					or
					(
						(select top 1 customercode from CsCustomerVehicleView where convert(varchar, BPKDate, 112) >= convert(varchar, dateadd(day, -day(getdate()) - @Minus2Days, getdate()), 112) and CustomerCode = a.CustomerCode) is not null
						and MONTH(a.CustomerBirthDate) = MONTH(getdate()) - 1
					)
				   )
			   --and datepart(month, a.CustomerBirthDate) >= @PreviousMonth
			   --and datepart(month, a.CustomerBirthDate) <= @CurrentMonth
		end
	end
 end
GO

DROP PROCEDURE [dbo].[uspfn_CsInqCustFeedback]
GO

CREATE procedure [dbo].[uspfn_CsInqCustFeedback]
	@CompanyCode varchar(20),
	@DateFrom varchar(10),
	@DateTo varchar(10)
as

select distinct a.CompanyCode
     , a.CustomerCode
     , c.CustomerName
     , rtrim(c.Address1) + ' ' + rtrim(c.Address2) + rtrim(c.Address3) as Address
     , c.HPNo
     , b.BpkbDate
     , b.StnkDate
     , g.DODate 
     , e.SalesModelCode
     , e.SalesModelYear
     , h.PoliceRegNo
     , a.Chassis
     , a.IsManual
     , a.FeedbackA
     , a.FeedbackB
     , a.FeedbackC
     , a.FeedbackD
     , case a.IsManual when 1 then 'Manual' else 'System' end Feedback
     , case len(rtrim(isnull(a.FeedbackA,''))) when 0 then '-' else 'Ya' end Feedback01
     , case len(rtrim(isnull(a.FeedbackB,''))) when 0 then '-' else 'Ya' end Feedback02
     , case len(rtrim(isnull(a.FeedbackC,''))) when 0 then '-' else 'Ya' end Feedback03
     , case len(rtrim(isnull(a.FeedbackD,''))) when 0 then '-' else 'Ya' end Feedback04
  from CsCustFeedback a
  left join CsCustomerVehicle b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.Chassis = a.Chassis
  left join GnMstCustomer c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
  left join omTrSalesSOVin e          
    on e.CompanyCode = a.CompanyCode          
   and e.ChassisCode + convert(varchar, e.ChassisNo) = a.Chassis 
  left join omTrSalesSO f
    on f.CompanyCode = e.CompanyCode          
   and f.BranchCode = e.BranchCode
   and f.SONo = e.SONo
  left join omTrSalesDO g
    on g.CompanyCode = f.CompanyCode          
   and g.BranchCode = f.BranchCode
   and g.SONo = f.SONo
  left join GnMstCustomer d
    on d.CompanyCode = f.CompanyCode
   and d.CustomerCode = f.LeasingCo
  left join svMstCustomerVehicle h
    on h.CompanyCode = a.CompanyCode          
   and h.ChassisCode + convert(varchar, h.ChassisNo) = a.Chassis 
 where a.CompanyCode = @CompanyCode
   and convert(varchar, g.DODate, 112) between @DateFrom and @DateTo

GO

DROP PROCEDURE [dbo].[uspfn_CsOutstandingDlvryReport]
GO

--uspfn_CsOutstandingDlvryReport '6006400106', '2015-12-01', '2015-12-14', 0
CREATE PROCEDURE [dbo].[uspfn_CsOutstandingDlvryReport]
	@BranchCode VARCHAR(15),
	@DateFrom DATETIME,
	@DateTo DATETIME,
	@Status INT -- NULL, 0, 1
AS
BEGIN
IF @BranchCode = '' SET @BranchCode = NULL

SELECT ROW_NUMBER() OVER(ORDER BY h.BranchCode, h.BPKNo) AS No,
       h.BranchCode, o.OutletAbbreviation, h.BPKNo, h.BPKDate,-- h.SONo, 
	   DeliveryDate = (CASE WHEN YEAR(h.LockingDate)<2000
	                             THEN '' ELSE left(convert(varchar, h.LockingDate, 121), 10) END),
		h.LockingDate,
	   h.CustomerCode, c.CustomerName, d.SalesModelCode, d.SalesModelYear, 
	   d.ChassisCode, d.ChassisNo, d.EngineCode, d.EngineNo
  FROM omTrSalesBPK h, omTrSalesBPKDetail d, gnMstCustomer c, gnMstDealerOutletMapping o
 WHERE (@BranchCode IS NULL OR h.BranchCode = @BranchCode)                     -- per Branch
   AND left(convert(varchar, h.BPKDate, 121), 10) BETWEEN @DateFrom AND @DateTo  -- per periode
   AND h.CompanyCode=d.CompanyCode
   AND h.BranchCode=d.BranchCode
   AND h.BPKNo=d.BPKNo
   AND h.CustomerCode=c.CustomerCode
   AND d.BranchCode=o.OutletCode
   AND (@Status IS NULL OR (@Status = 1 OR h.LockingDate = '1900-01-01 00:00:00.000') AND (@Status = 0 OR h.LockingDate <> '1900-01-01 00:00:00.000'))
END
GO

