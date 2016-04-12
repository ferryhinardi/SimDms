/****** Object:  StoredProcedure [dbo].[uspfn_SelectDailyRetention]    Script Date: 11/12/2015 3:23:13 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[uspfn_SelectDailyRetentionWeb]
     @CompanyCode varchar(20),
     @BranchCode  varchar(20),
     @DateParam   datetime,
     @OptionType  varchar(20),
     @Interval    int,
     @Range       int,
     @InclPdi     bit = 0,
     @UserID      varchar(20) = '',
	 @IsOdom	  bit
as

exec uspfn_SvDrhInitialWeb
	 @CompanyCode = @CompanyCode,
	 @BranchCode = @BranchCode,
	 @DateParam = @DateParam,
	 @OptionType = @OptionType,
	 @Interval = @Interval,
	 @Range = @Range,
	 @InclPdi = @InclPdi,
	 @UserID = @UserID,
	 @IsOdom = @IsOdom

exec uspfn_SvDrhSelect
	 @CompanyCode = @CompanyCode,
	 @BranchCode = @BranchCode,
	 @DateParam = @DateParam,
	 @OptionType = @OptionType,
	 @Interval = @Interval,
	 @Range = @Range,
	 @InclPdi = @InclPdi

