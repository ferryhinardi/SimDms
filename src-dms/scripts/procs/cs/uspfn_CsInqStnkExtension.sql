
go
if object_id('uspfn_CsStnkExtension') is not null
	drop procedure uspfn_CsStnkExtension

go
create procedure uspfn_CsStnkExtension
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
	insert into @t_rem (RemCode, RemDate) values('REMSTNKEXT', left(convert(varchar, dateadd(month, -convert(int, @Param1), @CurrDate), 112), 6) + '01')

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
			  from CsLkuStnkExtensionView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and isnull(a.StnkExpiredDate, isnull(a.StnkDate, a.BpkDate)) >= @DateFrom
			   and isnull(a.StnkExpiredDate, isnull(a.StnkDate, a.BpkDate)) <= @DateTo
			   and a.Outstanding = @Outstanding
		end
		else
		begin
			select *
			  from CsLkuStnkExtensionView a
			 where a.CompanyCode like @CompanyCode
			   and a.BranchCode like @BranchCode
			   and isnull(a.StnkExpiredDate, isnull(a.StnkDate, a.BpkDate)) >= @DateFrom
			   and isnull(a.StnkExpiredDate, isnull(a.StnkDate, a.BpkDate)) <= @DateTo
			   and a.Outstanding = @Outstanding
			   and a.IsStnkExtend = @IsStnkExtension
		end
	end
	else if @Status = 'Lookup'
	begin
		select *
		  from CsLkuStnkExtensionView a
		 where a.CompanyCode like @CompanyCode
		   and a.BranchCode like @BranchCode
		   and isnull(a.StnkExpiredDate, isnull(a.StnkDate, a.BpkDate)) >= (select RemDate from @t_rem where RemCode = 'REMSTNKEXT')
		   and a.Outstanding = @Outstanding
	end
 end

