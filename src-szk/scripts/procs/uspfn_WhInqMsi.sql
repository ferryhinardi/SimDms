alter procedure uspfn_WhInqMsi
--declare
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@PeriodYear varchar(20)
as

--select @CompanyCode = '6006406', @BranchCode = '6006406', @PeriodYear = '2013'

select CompanyCode, BranchCode, PeriodYear, SeqNo, MsiGroup, MsiDesc, Unit
	 , (isnull( [1], 0) + isnull( [2], 0) + isnull( [3], 0) 
	 +  isnull( [4], 0) + isnull( [5], 0) + isnull( [6], 0) 
	 +  isnull( [7], 0) + isnull( [8], 0) + isnull( [9], 0)
	 +  isnull([10], 0) + isnull([11], 0) + isnull([12], 0)) / month(getdate()) as Average
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
	  from SvHstSzkMSI
	 where CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	   and PeriodYear = @PeriodYear
  )#
 pivot (sum(MsiData) for PeriodMonth in ([1], [2], [3], [4], [5], [6], [7], [8], [9], [10], [11], [12])) as pvt
 order by pvt.MsiGroup, pvt.SeqNo
 