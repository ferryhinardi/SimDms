if object_id('uspfn_DlrInqMsiV2ByBranch') is not null
       drop procedure uspfn_DlrInqMsiV2ByBranch


-- =============================================
-- Author:		fhi
-- Create date: 07.04.2015 modified : 04092015
-- Description:	inq suzuki msi v2
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_DlrInqMsiV2ByBranch] 
	@Area varchar(250),
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@PeriodYear varchar(20),
	@Month1      int = 1,
	@Month2      int
AS
BEGIN
	
;with svHstSzkMSIUnion(CompanyCode,BranchCode,PeriodYear,PeriodMonth,SeqNo,MsiGroup,MsiDesc,Unit,MsiData,lastUpdateDate) as
(
select
	CompanyCode,
		BranchCode,
		PeriodYear,
		PeriodMonth,
		SeqNo,
		MsiGroup,
		MsiDesc,
		Unit,
		MsiData,
		CreatedDate 
--from svHstSzkMSI
from svHstSzkMSI_Invoice
where 
CompanyCode =@CompanyCode
and BranchCode=@BranchCode
and PeriodYear=@PeriodYear
AND periodMonth >=@Month1
and periodMonth <=@Month2

)

select * into #svHstSzkMSIV2 from(
	--SELECT * FROM svHstSzkMSIV2
	select * from svHstSzkMSIUnion
	
	)#svHstSzkMSIV2

select CompanyCode, BranchCode, PeriodYear, SeqNo, MsiGroup, MsiDesc, Unit
		 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
		 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
		 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
		 --+  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) / month(getdate()) as Average
		 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) / (@Month2 - @Month1 + 1) as Average
		 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
		 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
		 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
		 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) as Total
		 , isnull( [1], 0) [Month01]
		 , isnull( [2], 0) [Month02]
		 , isnull( [3], 0) [Month03]
		 , isnull( [4], 0) [Month04]
		 , isnull( [5], 0) [Month05]
		 , isnull( [6], 0) [Month06]
		 , isnull( [7], 0) [Month07]
		 , isnull( [8], 0) [Month08]
		 , isnull( [9], 0) [Month09]
		 , isnull([10], 0) [Month10]
		 , isnull([11], 0) [Month11]
		 , isnull([12], 0) [Month12]
	  from (
		select CompanyCode, BranchCode, PeriodYear, PeriodMonth, SeqNo, MsiGroup, MsiDesc, Unit, MsiData
		  from #svHstSzkMSIV2
		 where CompanyCode like case when @CompanyCode = '' then '%%' else @CompanyCode end
		   and BranchCode like case when @BranchCode = '' then '%%' else @BranchCode end
		   and PeriodYear like case when @PeriodYear = '' then '%%' else @PeriodYear end
		   and PeriodMonth >= @Month1
	       and PeriodMonth <= @Month2
	  )#
	 pivot (sum(MsiData) for PeriodMonth in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) as pvt
	 order by pvt.BranchCode

drop table #svHstSzkMSIV2

END

