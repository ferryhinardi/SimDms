
GO

/****** Object:  StoredProcedure [dbo].[usprpt_SvRpReport021Msi]    Script Date: 9/21/2015 11:10:34 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[usprpt_SvRpReport021Msi]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@PeriodYear  int,
	@Month1      int = 1,
	@Month2      int
as

set nocount on

select CompanyCode, BranchCode, PeriodYear, SeqNo, MsiGroup, MsiDesc, Unit
	 , DealerName = isnull(( select top 1 CompanyName from gnMstCoProfile
							  where CompanyCode = @CompanyCode
								and BranchCode = @BranchCode), '')
	 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
	 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
	 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
	 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) / (@Month2 - @Month1 + 1) as Average
	 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
	 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
	 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
	 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) as Total
	 , isnull( [1], 0)  [1]
	 , isnull( [2], 0)  [2]
	 , isnull( [3], 0)  [3]
	 , isnull( [4], 0)  [4]
	 , isnull( [5], 0)  [5]
	 , isnull( [6], 0)  [6]
	 , isnull( [7], 0)  [7]
	 , isnull( [8], 0)  [8]
	 , isnull( [9], 0)  [9]
	 , isnull([10], 0) [10]
	 , isnull([11], 0) [11]
	 , isnull([12], 0) [12]
  from (
	--select CompanyCode, BranchCode, PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData from SvHstSzkMSI
	select CompanyCode, BranchCode, PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData from svHstSzkMSI_Invoice
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and PeriodYear = @PeriodYear
	   and PeriodMonth >= @Month1
	   and PeriodMonth <= @Month2
  )#
 pivot (sum(MsiData) for PeriodMonth in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) as pvt
 order by pvt.MsiGroup, pvt.SeqNo

GO


