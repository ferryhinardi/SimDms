alter procedure uspfn_CsInqBpkbReminder
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
	insert into @t_rem (RemCode, RemDate) values('REMBPKB', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')
   
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


	  update @t_rem set RemValue = (
							select count(a.CustomerCode)
							  from CsLkuBpkbReminderView a
							 where a.CompanyCode like @CompanyCode
							   and a.BranchCode like @BranchCode
							   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')
							   and a.Outstanding = 'Y'
					)
	  where RemCode = 'REMBPKB';

	  if @Status = 'Inquiry'
	  begin
			select*
			  from CsLkuBpkbReminderView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   ----and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')
			   and a.DoDate >= @DateFrom
			   and a.DoDate <=  @DateTo
			   and a.Outstanding = @Outstanding
	  end
	  else if @Status = 'Lookup'
	  begin
        if @Outstanding = 'Y'
        begin
			select *
			  from CsLkuBpkbReminderView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')
			   and not exists (select top 1 1 from CsCustBpkb where CompanyCode = a.CompanyCode and CustomerCode = a.CustomerCode and Chassis = a.Chassis)
        end
        else 
        begin
			select distinct *
			  from CsLkuBpkbReminderView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REMBPKB')
			   and exists (select top 1 1 from CsCustBpkb where CompanyCode = a.CompanyCode and CustomerCode = a.CustomerCode and Chassis = a.Chassis)
        end
	  end
 end

