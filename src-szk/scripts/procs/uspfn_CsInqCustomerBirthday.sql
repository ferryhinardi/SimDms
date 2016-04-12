
go
if object_id('uspfn_CsInqCustomerBirthday') is not null
	drop procedure uspfn_CsInqCustomerBirthday

go
create procedure uspfn_CsInqCustomerBirthday
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
		   if isnull(@Outstanding, '') = '' or rtrim(@Outstanding) = '' 
		   begin
				select *
	   			  from CsLkuBirthdayView a
				 where a.CompanyCode like @CompanyCode
				   and a.BranchCode like @BranchCode
				   and datepart(month, a.CustomerBirthDate) >= @MonthFrom 
				   and datepart(month, a.CustomerBirthDate) <= @MonthTo
				   and a.PeriodOfYear = @Year
		   end
		   else
		   begin
		   		select *
	   			  from CsLkuBirthdayView a
				 where a.CompanyCode like @CompanyCode
				   and a.BranchCode like @BranchCode
				   and datepart(month, a.CustomerBirthDate) >= @MonthFrom 
				   and datepart(month, a.CustomerBirthDate) <= @MonthTo
				   and a.PeriodOfYear = @Year
				   and a.Outstanding = @Outstanding;
		   end
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
			   and datepart(month, a.CustomerBirthDate) >= @PreviousMonth
			   and datepart(month, a.CustomerBirthDate) <= @CurrentMonth
		end
		else if @Outstanding = 'N'
		begin
			select *	
			  from CsLkuBirthdayView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.Outstanding = @Outstanding
			   and a.PeriodOfYear = @Year
			   and datepart(month, a.CustomerBirthDate) >= @PreviousMonth
			   and datepart(month, a.CustomerBirthDate) <= @CurrentMonth
		end
		
	end
 end




 go
 --exec uspfn_CsInqCustomerBirthday @CompanyCode=N'6021406',@BranchCode=N'602140607',@Year=N'2014',@MonthFrom=N'1',@MonthTo=N'12',@Outstanding=N'Y'
 exec uspfn_CsInqCustomerBirthday @CompanyCode=N'6021406',@BranchCode=N'602140607',@Year=2014,@MonthFrom=1,@MonthTo=1,@Outstanding=N'Y', @Status = 'Lookup'

