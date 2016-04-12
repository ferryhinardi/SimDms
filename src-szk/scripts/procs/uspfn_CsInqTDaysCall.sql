
go
if object_id('uspfn_CsInqTDaysCall') is not null
	drop procedure uspfn_CsInqTDaysCall

go
create procedure uspfn_CsInqTDaysCall
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

	-- REM3DAYSCALL
	set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REM3DAYSCALL'), '0')
	insert into @t_rem (RemCode, RemDate) values('REM3DAYSCALL', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

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
	end
	else if @Status = 'Lookup'
	begin
		if @Outstanding = 'Y'
		begin
			select *
			  from CsLkuTDaysCallView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')
			   and a.Outstanding = 'Y'
		end
		else
		begin
			select *
			  from CsLkuTDaysCallView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and a.DoDate >=  (select RemDate from @t_rem where RemCode = 'REM3DAYSCALL')
			   and a.Outstanding = 'N'
		end
	end
 end

 

 go
 exec uspfn_CsInqTDaysCall '6021406', '602140602', '2014-01-01', '2014-05-28', 'N', 'Lookup'
 

