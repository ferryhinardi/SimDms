
ALTER procedure [dbo].[usprpt_SvRpCrm003V2]
	@CompanyCode	  Varchar(15),
	@BranchCode		  Varchar(15),
	@DateParam		  Datetime,
	@OptionType		  Varchar(100), 
	@Range			  int, 
	@Interval		  int,
	@IncPDI			  bit = 1,
	@ServiceDateFrom  Varchar(15),
	@ServiceDateTo    Varchar(15),
	@ReminderDateFrom Varchar(15),
	@ReminderDateTo   Varchar(15),
	@FollowUpDateFrom Varchar(15),
	@FollowUpDateTo   Varchar(15)
AS

if (isnull(@ServiceDateFrom, '') = '' and isnull(@ServiceDateTo, '') = '' and isnull(@ReminderDateFrom, '') = '' and
    isnull(@ReminderDateTo, '') = '' and isnull(@FollowUpDateFrom, '') = '' and isnull(@FollowUpDateTo, '') = '')
	--exec usprpt_SvRpCrm003V2_Drh 
	--				@CompanyCode=@CompanyCode,@BranchCode=@BranchCode,@DateParam=@DateParam,
	--				@OptionType=@OptionType,@Range=@Range,@Interval=@Interval,@IncPDI=@IncPDI
	exec usprpt_SvRpCrm003V2_Drh_Web 
					@CompanyCode=@CompanyCode,@BranchCode=@BranchCode,@DateParam=@DateParam,
					@OptionType=@OptionType,@Range=@Range,@Interval=@Interval,@IncPDI=@IncPDI
else
	--exec usprpt_SvRpCrm003V2_DrhPer
	--				@CompanyCode=@CompanyCode,@BranchCode=@BranchCode,@OptionType=@OptionType,
	--				@ServiceDateFrom=@ServiceDateFrom,@ServiceDateTo=@ServiceDateTo,
	--				@ReminderDateFrom=@ReminderDateFrom,@ReminderDateTo=@ReminderDateTo,
	--				@FollowUpDateFrom=@FollowUpDateFrom,@FollowUpDateTo=@FollowUpDateTo
	exec usprpt_SvRpCrm003V2_DrhPer_Web
					@CompanyCode=@CompanyCode,@BranchCode=@BranchCode,@OptionType=@OptionType,
					@ServiceDateFrom=@ServiceDateFrom,@ServiceDateTo=@ServiceDateTo,
					@ReminderDateFrom=@ReminderDateFrom,@ReminderDateTo=@ReminderDateTo,
					@FollowUpDateFrom=@FollowUpDateFrom,@FollowUpDateTo=@FollowUpDateTo
