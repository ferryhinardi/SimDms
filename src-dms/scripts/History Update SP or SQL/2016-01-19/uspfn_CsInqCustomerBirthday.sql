IF EXISTS (SELECT OBJECT_NAME(OBJECT_ID) FROM sys.sql_modules WHERE objectproperty(OBJECT_ID, 'IsProcedure') = 1
AND definition LIKE '%uspfn_CsInqCustomerBirthday%')
	DROP PROC uspfn_CsInqCustomerBirthday
GO
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

	if @Status = 'Inquiry'
	begin
		if (rtrim(@Outstanding) = '') set @Outstanding = null;
		select a.*, dbo.typeofgiftToName(a.TypeOfGift,'|') as [GreetingBy]
	   	from CsLkuBirthdayView a
		where a.CompanyCode like @CompanyCode
			and a.BranchCode like @BranchCode
			and month(a.CustomerBirthDate) between @MonthFrom and @MonthTo
			and a.PeriodOfYear = @Year
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
			   --and datepart(month, a.CustomerBirthDate) = datepart(month, getdate()) 
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
			   --and a.PeriodOfYear = @Year
			   -- and datepart(month, a.CustomerBirthDate) = datepart(month, getdate()) 
			   --and datepart(month, a.CustomerBirthDate) >= @PreviousMonth
			   --and datepart(month, a.CustomerBirthDate) <= @CurrentMonth
		end
	end
 end


