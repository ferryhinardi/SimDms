
if object_id('uspfn_CsLkuCustBday') is not null
	drop procedure uspfn_CsLkuCustBday

go
create procedure uspfn_CsLkuCustBday
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@OutStanding char(1), 
	@CustomerCode varchar(13),
	@CustomerName varchar(50)
as

begin
	declare @Param1 varchar(25);
	set @Param1 = isnull((select SettingParam1 from CsSettings where CompanyCode = @CompanyCode and SettingCode = 'REM3DAYSCALL'), '0');

		select *
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
		   and a.DoDate >=  ( left(convert(varchar, dateadd(month, -convert(int, @Param1), getdate()), 112), 6) + '01' )
















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

go
exec uspfn_CsLkuCustBday '6006406', '6006406', 'Y', '', ''






go
if object_id('uspfn_CsLkuCustBdayCount') is not null
	drop procedure uspfn_CsLkuCustBdayCount

go
create procedure uspfn_CsLkuCustBdayCount
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@OutStanding char(1),
	@TotalRec int OUTPUT
as

begin
	declare @BottomLimit int;
	declare @UpperLimit int;
	declare @CurrentMonth int;
	declare @Setting int;

	set @Setting = (select top 1 SettingParam1 from CsSettings where SettingCode='REMBDAY');
	set @CurrentMonth = datepart(month, getdate());

	set @BottomLimit = @CurrentMonth - @Setting;
	set @UpperLimit = @CurrentMonth;

	if(@BottomLimit <= 0) 
		set @BottomLimit = 12 + @BottomLimit;

	if @UpperLimit >= 12 
		set @UpperLimit = @UpperLimit - 12;

	

	set @TotalRec = (
			select count(distinct a.CustomerCode)
			  from CsLkuBirthdayView a
			 where a.CompanyCode = @CompanyCode
			   and a.BranchCode = @BranchCode
			   and a.Outstanding = @OutStanding
			   and (
						DATEPART(month, a.CustomerBirthDate) >= @BottomLimit
						or
						DATEPART(month, a.CustomerBirthDate) <= @UpperLimit
				   )
		)

--	select @TotalRec as Total;
	return @TotalRec;
end


