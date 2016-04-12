
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
			select count(a.CustomerCode)
			  from CsLkuBirthdayView a
			 where a.CompanyCode = @CompanyCode
			   and a.BranchCode = @BranchCode
			   --and a.Outstanding = @OutStanding
			   and (case	 
							when (  select top 1

										   x.CustomerCode
									  from CsCustBirthDay x
									 where x.CompanyCode=a.CompanyCode
									   and x.CustomerCode=a.CustomerCode
							) is null then 'Y'
							--when @UpperLimit > @BottomLimit and ( datepart(month, a.CustomerBirthDate) < @BottomLimit or datepart(month, a.CustomerBirthDate) > @UpperLimit) then 'Y'
							else 'N'
					 end	 
				   ) = @OutStanding
			   and (
						DATEPART(month, a.CustomerBirthDate) >= @BottomLimit
						or
						DATEPART(month, a.CustomerBirthDate) <= @UpperLimit
				   )
		)

	return @TotalRec;
end


