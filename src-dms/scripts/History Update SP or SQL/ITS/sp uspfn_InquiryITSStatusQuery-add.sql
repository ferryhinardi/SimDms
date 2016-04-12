
GO

/****** Object:  StoredProcedure [dbo].[uspfn_InquiryITSStatusQuery]    Script Date: 5/7/2015 5:33:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Seandy A.K
-- Create date: 2-7-2013
-- Description:	Inquiry ITS with Status
-- Query : uspfn_InquiryITSStatusQuery_Tuning '20130601','20130628','','','','','APV-MB','',1
-- =============================================
ALTER PROCEDURE [dbo].[uspfn_InquiryITSStatusQuery]
	@StartDate			varchar(20),
	@EndDate			varchar(20),
	@Area				varchar(100),
	@CompanyCode		varchar(15),
	@BranchCode			varchar(15),
	@GroupModel			varchar(100),
	@TipeKendaraan		varchar(100),
	@Variant			varchar(100),
	@Summary			bit
AS
--Declare @StartDate		varchar(20)
--Declare @EndDate		varchar(20)
--Declare @Area			varchar(100)
--Declare @CompanyCode	varchar(15)
--declare @BranchCOde		varchar(15)
--declare @TipeKendaraan	varchar(100)
--declare @Variant		varchar(100)
--declare @Summary		bit
--set @StartDate = '20130701'
--set @EndDate = '20130708'
--set @Area = ''
--set @CompanyCode = ''
--set @BranchCode = ''
--set @TipeKendaraan = 'APV-MB'
--set @Variant = ''
--set @Summary = 0
declare @National varchar(10)
set @National = (select top 1 ISNULL(ParaValue,0) from gnMstLookUpDtl
                  where CodeID='QSLS' and LookUpValue='NATIONAL')

declare @Week int
set @Week = (DATEDIFF(week, DATEADD(MONTH, DATEDIFF(MONTH, 0, convert(datetime,@EndDate)), 0), convert(datetime,@EndDate)) + 1)

if(@National = 1)
begin
	select * into #StatusWoArea from(
		Select c.GroupNo
		 , c.Area
		 , b.CompanyCode
		 , c.DealerAbbreviation
		 , b.BranchCode
		 , b.InquiryNumber
		 , b.SequenceNo
		 , b.LastProgress
		 , b.UpdateDate
		 , (DATEDIFF(week, DATEADD(MONTH, DATEDIFF(MONTH, 0, b.UpdateDate), 0), b.UpdateDate) +1) WeekInt
		 from pmStatusHistory b with (nolock, nowait)
		 left join gnMstDealerMapping c with (nolock, nowait) on b.CompanyCode = c.DealerCode
		 where (c.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or c.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end) 
		   and b.CompanyCode like case when @CompanyCode = '' then '%%' else @CompanyCode end
		   and b.BranchCode like case when @BranchCode = '' then '%%' else @BranchCode end
		   and b.LastProgress in (select LookUpValue from gnMstLookUpDtl where CodeID = 'PSTS')
		   and convert(varchar,b.UpdateDate,112) < @EndDate
	)#StatusWoArea
	
	select * into #VehicleTemplateWoArea from(
		select distinct a.GroupNo
			 , a.Area
			 , a.CompanyCode
			 , a.DealerAbbreviation
			 , a.BranchCode
			 , isnull(b.TipeKendaraan,'') TipeKendaraan
			 , isnull(b.Variant,'') Variant
			 , c.SeqNo
			 , c.LookUpValue LastProgress
		  from (select distinct GroupNo
						  , Area
						  , CompanyCode
						  , DealerAbbreviation
						  , BranchCode
						  , InquiryNumber
					   from #StatusWoArea) a
		  INNER JOIN gnMstLookUpDtl c with (nolock, nowait) on c.CodeID = 'PSTS' 
		  LEFT JOIN pmHstITS b with (nolock, nowait) on a.CompanyCode = b.CompanyCode
			    and a.BranchCode = b.BranchCode
			    and a.InquiryNumber = b.InquiryNumber
		  where TipeKendaraan like case when @TipeKendaraan = '' then '%%' else @TipeKendaraan end
			and Variant like case when @Variant = '' then '%%' else @Variant end
	)#VehicleTemplateWoArea
	
	select * into #VehicleWoArea from(
		select a.GroupNo
			 , a.Area
			 , a.CompanyCode
			 , a.DealerAbbreviation
			 , a.BranchCode
			 , a.InquiryNumber
			 , b.InquiryDate
			 , isnull(b.TipeKendaraan,'') TipeKendaraan
			 , isnull(b.Variant,'') Variant
			 , a.SequenceNo
			 , a.LastProgress
			 , a.UpdateDate
			 , a.WeekInt
		  from #StatusWoArea a
		  INNER join pmHstITS b with (nolock, nowait) on a.CompanyCode = b.CompanyCode
			    and a.BranchCode = b.BranchCode
			    and a.InquiryNumber = b.InquiryNumber
		  where TipeKendaraan like case when @TipeKendaraan = '' then '%%' else @TipeKendaraan end
			and Variant like case when @Variant = '' then '%%' else @Variant end
	)#VehicleWoArea
	
	select * into #TestSummaryFirst from(
		select distinct '1' OrderNo3
			 , '0' OrderNo
			 , '0' OrderNo1
			 , '0' OrderNo2
			 , a.GroupNo
			 , a.Area
			 , a.CompanyCode
			 , a.DealerAbbreviation
			 , a.BranchCode
			 , a.TipeKendaraan
			 , a.Variant
			 , (select SeqNo from GnMstLookUpDtl where CodeID = 'PSTS' and LookUpValue = a.LastProgress) seqNo
			 , a.LastProgress
			 , a.WeekInt
			 ,(select COUNT(*) 
				 from #VehicleWoArea 
				where CompanyCode = a.CompanyCode 
				  and BranchCode = a.BranchCode 
				  and TipeKendaraan = a.TipeKendaraan 
				  and Variant = a.Variant 
				  and LastProgress = a.LastProgress 
				  and convert(varchar,UpdateDate,112) < @StartDate 
				  and convert(varchar,InquiryDate,112) < @StartDate) SaldoAwal
			 ,(select COUNT(*) 
				 from #VehicleWoArea 
				where CompanyCode = a.CompanyCode 
				  and BranchCode = a.BranchCode 
				  and TipeKendaraan = a.TipeKendaraan 
				  and Variant = a.Variant 
				  and LastProgress = a.LastProgress 
				  and WeekInt = a.WeekInt
				  and convert(Varchar,UpdateDate,112) between @StartDate and @EndDate
				  and convert(varchar,InquiryDate,112) < @StartDate) TotalOuts
			 ,(select COUNT(*) 
				 from #VehicleWoArea 
				where CompanyCode = a.CompanyCode 
				  and BranchCode = a.BranchCode 
				  and TipeKendaraan = a.TipeKendaraan 
				  and Variant = a.Variant 
				  and LastProgress = a.LastProgress 
				  and WeekInt = a.WeekInt
				  and convert(Varchar,UpdateDate,112) between @StartDate and @EndDate
				  and convert(varchar,InquiryDate,112) between @StartDate and @EndDate) Total
		from #VehicleWoArea a
	)#TestSummaryFirst

	select * into #TestPivotFirst from(
		select OrderNo3
			 , OrderNo
			 , OrderNo1
			 , OrderNo2
			 , GroupNo
			 , Area
			 , CompanyCode
			 , DealerAbbreviation
			 , BranchCode
			 , TipeKendaraan
			 , Variant
			 , SeqNo
			 , LastProgress
			 , SaldoAwal
			 , isnull([1],0) Week1, isnull([2],0) Week2, isnull([3],0) Week3,isnull([4],0) Week4, isnull([5],0) Week5, isnull([6],0) Week6
			 , 0 WeekOuts1, 0 WeekOuts2, 0 WeekOuts3, 0 WeekOuts4, 0 WeekOuts5, 0 WeekOuts6
		  from (
			select OrderNo3
				 , OrderNo
				 , OrderNo1
				 , OrderNo2
				 , GroupNo
				 , Area
				 , CompanyCode
				 , DealerAbbreviation
				 , BranchCode
				 , TipeKendaraan
				 , Variant
				 , SeqNo
				 , LastProgress
				 , SaldoAwal
				 , weekInt
				 , Total
			 from #TestSummaryFirst
			where (SaldoAwal + TotalOuts + Total) <> 0
		  ) as Header
		  pivot (SUM(Total)
			for WeekInt in ([1],[2],[3],[4],[5],[6])
		  )pvt
	UNION
		Select '1' OrderNo3
			 , '0' OrderNo
			 , '0' OrderNo1
			 , '0' OrderNo2
			 , GroupNo
			 , Area
			 , CompanyCode
			 , DealerAbbreviation
			 , BranchCode
			 , TipeKendaraan
			 , Variant
			 , SeqNo
			 , LastProgress
			 ,(select COUNT(*) 
			 from #VehicleWoArea 
			where GroupNo = a.GroupNo
		      and Area = a.Area
		      and CompanyCode = a.CompanyCode 
			  and BranchCode = a.BranchCode 
			  and TipeKendaraan = a.TipeKendaraan
			  and Variant = a.Variant
			  and SeqNo = a.SeqNo
			  and LastProgress = a.LastProgress 
			  and convert(varchar,UpdateDate,112) < @StartDate 
			  and convert(varchar,InquiryDate,112) < @StartDate) SaldoAwal
			 , 0, 0, 0, 0, 0, 0
			 , 0, 0, 0, 0, 0, 0
		  from #VehicleTemplateWoArea a
		  where not exists (select 1 
							  from #TestSummaryFirst
							 where GroupNo = a.GroupNo
							   and Area = a.Area
							   and CompanyCode = a.CompanyCode
							   and BranchCode = a.BranchCode
							   and TipeKendaraan = a.TipeKendaraan
							   and Variant = a.Variant
							   and SeqNo = a.SeqNo
							   and LastProgress = a.LastProgress
							   and (SaldoAwal + TotalOuts + Total) <> 0)
	)#TestPivotFirst
	
	select * into #TestPivotOutsFirst from(
		select OrderNo3
			 , OrderNo
			 , OrderNo1
			 , OrderNo2
			 , GroupNo
			 , Area
			 , CompanyCode
			 , DealerAbbreviation
			 , BranchCode
			 , TipeKendaraan
			 , Variant
			 , SeqNo
			 , LastProgress
			 , SaldoAwal
			 , 0 Week1, 0 Week2, 0 Week3, 0 Week4, 0 Week5, 0 Week6
			 , isnull([1],0) WeekOuts1 , isnull([2],0) WeekOuts2 , isnull([3],0) WeekOuts3 ,isnull([4],0) WeekOuts4 , isnull([5],0) WeekOuts5 , isnull([6],0) WeekOuts6 
		  from (
			select OrderNo3
				 , OrderNo
				 , OrderNo1
				 , OrderNo2
				 , GroupNo
				 , Area
				 , CompanyCode
				 , DealerAbbreviation
				 , BranchCode
				 , TipeKendaraan
				 , Variant
				 , SeqNo
				 , LastProgress
				 , SaldoAwal
				 , weekInt
				 , TotalOuts
			 from #TestSummaryFirst
			where (SaldoAwal + TotalOuts + Total) <> 0
		  ) as Header
		  pivot (SUM(TotalOuts)
			for WeekInt in ([1],[2],[3],[4],[5],[6])
		  )pvt
	UNION
		Select '1' OrderNo3
			 , '0' OrderNo
			 , '0' OrderNo1
			 , '0' OrderNo2
			 , GroupNo
			 , Area
			 , CompanyCode
			 , DealerAbbreviation
			 , BranchCode
			 , TipeKendaraan
			 , Variant
			 , SeqNo
			 , LastProgress
			 , 0
			 , 0, 0, 0, 0, 0, 0
			 , 0, 0, 0, 0, 0, 0
		  from #VehicleTemplateWoArea a
		  where not exists (select 1 
							  from #TestSummaryFirst
							 where CompanyCode = a.CompanyCode
							   and BranchCode = a.BranchCode
							   and TipeKendaraan = a.TipeKendaraan
							   and Variant = a.Variant
							   and SeqNo = a.SeqNo
							   and LastProgress = a.LastProgress
							   and (SaldoAwal + TotalOuts + Total) <> 0)
	)#TestPivotOutsFirst

	select * into #TestFinalSummaryFirst from(
		select '1' OrderNo3
			 , '0' OrderNo
			 , '0' OrderNo1
			 , '0' OrderNo2
			 , a.GroupNo
			 , a.Area
			 , a.CompanyCode
			 , a.DealerAbbreviation
			 , a.BranchCode
			 , a.TipeKendaraan
			 , a.Variant
			 , a.SeqNo
			 , a.LastProgress
			 , a.SaldoAwal
			 , case when b.LastProgress = 'P' then 0 else b.WeekOuts1 end WeekOuts1
			 , case when b.LastProgress = 'P' then 0 else b.WeekOuts2 end WeekOuts2
			 , case when b.LastProgress = 'P' then 0 else b.WeekOuts3 end WeekOuts3
			 , case when b.LastProgress = 'P' then 0 else b.WeekOuts4 end WeekOuts4
			 , case when b.LastProgress = 'P' then 0 else b.WeekOuts5 end WeekOuts5
			 , case when b.LastProgress = 'P' then 0 else b.WeekOuts6 end WeekOuts6
			 , a.Week1
			 , a.Week2
			 , a.Week3
			 , a.Week4
			 , a.Week5
			 , a.Week6
		  from #TestPivotFirst a
		  left join #TestPivotOutsFirst b on a.GroupNo = b.GroupNo
				and a.OrderNo = b.OrderNo
				and a.OrderNo1 = b.OrderNo1
				and a.OrderNo2 = b.OrderNo2
			    and a.GroupNo = b.GroupNo
				and a.Area = b.Area
				and a.CompanyCode = b.CompanyCode
				and a.BranchCode = b.BranchCode
				and a.TipeKendaraan = b.TipeKendaraan
				and a.Variant = b.Variant
				and a.SeqNo = b.SeqNo
				and a.LastProgress = b.LastProgress
	)#TestFinalSummaryFirst
	
	select * into #SummaryDetail from(
			Select a.OrderNo3
				 , a.OrderNo
				 , a.OrderNo1
				 , a.OrderNo2
				 , a.GroupNo
				 , a.Area
				 , a.CompanyCode
				 , a.DealerAbbreviation
				 , a.BranchCode
				 , a.TipeKendaraan
				 , a.Variant
				 , a.SeqNo
				 , a.LastProgress
				 , a.SaldoAwal
				 , a.WeekOuts1
				 , a.WeekOuts2
				 , a.WeekOuts3
				 , a.WeekOuts4
				 , a.WeekOuts5
				 , a.WeekOuts6
				 , (case when a.LastProgress = 'P' then 0 else a.WeekOuts1 + a.WeekOuts2 + a.WeekOuts3 + a.WeekOuts4 + a.WeekOuts5 + a.WeekOuts6 end) TotalWeekOuts
				 , a.Week1
				 , a.Week2
				 , a.Week3
				 , a.Week4
				 , a.Week5
				 , a.Week6
				 , (a.Week1 + a.Week2 + a.Week3 + a.Week4 + a.Week5 + a.Week6) TotalWeek
				 , (case when a.LastProgress = 'P' then 0 else a.WeekOuts1 + a.WeekOuts2 + a.WeekOuts3 + a.WeekOuts4 + a.WeekOuts5 + a.WeekOuts6 end + a.Week1 + a.Week2 + a.Week3 + a.Week4 + a.Week5 + a.Week6) Total
			 from #TestFinalSummaryFirst a
	)#SummaryDetail
	
	select a.OrderNo3
		 , a.OrderNo
		 , a.OrderNo1
		 , a.OrderNo2
		 , a.GroupNo
		 , a.Area
		 , a.CompanyCode
		 , a.DealerAbbreviation CompanyName
		 , a.BranchCode
		 , isnull(c.OutletAbbreviation,'') BranchName
		 , a.TipeKendaraan
		 , a.Variant
		 , a.SeqNo
		 , a.LastProgress
		 , a.SaldoAwal
		 , a.WeekOuts1
		 , a.WeekOuts2
		 , a.WeekOuts3
		 , a.WeekOuts4
		 , a.WeekOuts5
		 , a.WeekOuts6
		 , a.TotalWeekOuts
		 , a.Week1
		 , a.Week2
		 , a.Week3
		 , a.Week4
		 , a.Week5
		 , a.Week6
		 , a.TotalWeek
		 , a.Total
	from #SummaryDetail a
	left join gnMstDealerOutletMapping c on a.CompanyCode = c.DealerCode and a.BranchCode = c.OutletCode
	where OrderNo1 <> 2
	order by a.OrderNo3 Asc,a.GroupNo asc, a.DealerAbbreviation asc,a.Area asc,a.OrderNo2 asc,a.BranchCode asc,a.OrderNo1 asc, a.TipeKendaraan Asc, a.OrderNo Asc, a.Variant Asc, a.SeqNo Asc
-- Query : uspfn_InquiryITSStatus '20130301','20130330','','','','','',0
	
	drop table #SummaryDetail
	drop table #TestFinalSummaryFirst
	drop table #TestPivotOutsFirst
	drop table #TestPivotFirst
	drop table #TestSummaryFirst
	drop table #VehicleWoArea
	drop table #VehicleTemplateWoArea
	drop table #StatusWoArea
end
else
-- add by fhi 19-03-2015 : penambahan else 
begin
	select * into #StatusWoAreaDlr from(
		Select c.GroupNo
		 , c.Area
		 , b.CompanyCode
		 , c.DealerAbbreviation
		 , b.BranchCode
		 , b.InquiryNumber
		 , b.SequenceNo
		 , b.LastProgress
		 , b.UpdateDate
		 , (DATEDIFF(week, DATEADD(MONTH, DATEDIFF(MONTH, 0, b.UpdateDate), 0), b.UpdateDate) +1) WeekInt
		 from pmStatusHistory b with (nolock, nowait)
		 left join gnMstDealerMapping c with (nolock, nowait) on b.CompanyCode = c.DealerCode
		 where (c.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'JABODETABEK'
										else @Area end
								else '%%' end
			or c.Area like Case when @Area <> '' 
								then  case when (@Area = 'JABODETABEK' or @Area = 'CABANG')
										then 'CABANG'
										else @Area end
								else '%%' end) 
		   and b.CompanyCode like case when @CompanyCode = '' then '%%' else @CompanyCode end
		   and b.BranchCode like case when @BranchCode = '' then '%%' else @BranchCode end
		   and b.LastProgress in (select LookUpValue from gnMstLookUpDtl where CodeID = 'PSTS')
		   and convert(varchar,b.UpdateDate,112) < @EndDate
	)#StatusWoAreaDlr
	
	select * into #VehicleTemplateWoAreaDlr from(
		select distinct a.GroupNo
			 , a.Area
			 , a.CompanyCode
			 , a.DealerAbbreviation
			 , a.BranchCode
			 , isnull(b.TipeKendaraan,'') TipeKendaraan
			 , isnull(b.Variant,'') Variant
			 , c.SeqNo
			 , c.LookUpValue LastProgress
		  from (select distinct GroupNo
						  , Area
						  , CompanyCode
						  , DealerAbbreviation
						  , BranchCode
						  , InquiryNumber
					   from #StatusWoAreaDlr) a
		  INNER JOIN gnMstLookUpDtl c with (nolock, nowait) on c.CodeID = 'PSTS' 
		  LEFT JOIN pmKDP b with (nolock, nowait) on a.CompanyCode = b.CompanyCode
			    and a.BranchCode = b.BranchCode
			    and a.InquiryNumber = b.InquiryNumber
		  where TipeKendaraan like case when @TipeKendaraan = '' then '%%' else @TipeKendaraan end
			and Variant like case when @Variant = '' then '%%' else @Variant end
	)#VehicleTemplateWoAreaDlr
	
	select * into #VehicleWoAreaDlr from(
		select a.GroupNo
			 , a.Area
			 , a.CompanyCode
			 , a.DealerAbbreviation
			 , a.BranchCode
			 , a.InquiryNumber
			 , b.InquiryDate
			 , isnull(b.TipeKendaraan,'') TipeKendaraan
			 , isnull(b.Variant,'') Variant
			 , a.SequenceNo
			 , a.LastProgress
			 , a.UpdateDate
			 , a.WeekInt
		  from #StatusWoAreaDlr a
		  INNER join pmKDP b with (nolock, nowait) on a.CompanyCode = b.CompanyCode
			    and a.BranchCode = b.BranchCode
			    and a.InquiryNumber = b.InquiryNumber
		  where TipeKendaraan like case when @TipeKendaraan = '' then '%%' else @TipeKendaraan end
			and Variant like case when @Variant = '' then '%%' else @Variant end
	)#VehicleWoAreaDlr
	
	select * into #TestSummaryFirstDlr from(
		select distinct '1' OrderNo3
			 , '0' OrderNo
			 , '0' OrderNo1
			 , '0' OrderNo2
			 , a.GroupNo
			 , a.Area
			 , a.CompanyCode
			 , a.DealerAbbreviation
			 , a.BranchCode
			 , a.TipeKendaraan
			 , a.Variant
			 , (select SeqNo from GnMstLookUpDtl where CodeID = 'PSTS' and LookUpValue = a.LastProgress) seqNo
			 , a.LastProgress
			 , a.WeekInt
			 ,(select COUNT(*) 
				 from #VehicleWoAreaDlr 
				where CompanyCode = a.CompanyCode 
				  and BranchCode = a.BranchCode 
				  and TipeKendaraan = a.TipeKendaraan 
				  and Variant = a.Variant 
				  and LastProgress = a.LastProgress 
				  and convert(varchar,UpdateDate,112) < @StartDate 
				  and convert(varchar,InquiryDate,112) < @StartDate) SaldoAwal
			 ,(select COUNT(*) 
				 from #VehicleWoAreaDlr 
				where CompanyCode = a.CompanyCode 
				  and BranchCode = a.BranchCode 
				  and TipeKendaraan = a.TipeKendaraan 
				  and Variant = a.Variant 
				  and LastProgress = a.LastProgress 
				  and WeekInt = a.WeekInt
				  and convert(Varchar,UpdateDate,112) between @StartDate and @EndDate
				  and convert(varchar,InquiryDate,112) < @StartDate) TotalOuts
			 ,(select COUNT(*) 
				 from #VehicleWoAreaDlr 
				where CompanyCode = a.CompanyCode 
				  and BranchCode = a.BranchCode 
				  and TipeKendaraan = a.TipeKendaraan 
				  and Variant = a.Variant 
				  and LastProgress = a.LastProgress 
				  and WeekInt = a.WeekInt
				  and convert(Varchar,UpdateDate,112) between @StartDate and @EndDate
				  and convert(varchar,InquiryDate,112) between @StartDate and @EndDate) Total
		from #VehicleWoAreaDlr a
	)#TestSummaryFirstDlr

	select * into #TestPivotFirstDlr from(
		select OrderNo3
			 , OrderNo
			 , OrderNo1
			 , OrderNo2
			 , GroupNo
			 , Area
			 , CompanyCode
			 , DealerAbbreviation
			 , BranchCode
			 , TipeKendaraan
			 , Variant
			 , SeqNo
			 , LastProgress
			 , SaldoAwal
			 , isnull([1],0) Week1, isnull([2],0) Week2, isnull([3],0) Week3,isnull([4],0) Week4, isnull([5],0) Week5, isnull([6],0) Week6
			 , 0 WeekOuts1, 0 WeekOuts2, 0 WeekOuts3, 0 WeekOuts4, 0 WeekOuts5, 0 WeekOuts6
		  from (
			select OrderNo3
				 , OrderNo
				 , OrderNo1
				 , OrderNo2
				 , GroupNo
				 , Area
				 , CompanyCode
				 , DealerAbbreviation
				 , BranchCode
				 , TipeKendaraan
				 , Variant
				 , SeqNo
				 , LastProgress
				 , SaldoAwal
				 , weekInt
				 , Total
			 from #TestSummaryFirstDlr
			where (SaldoAwal + TotalOuts + Total) <> 0
		  ) as Header
		  pivot (SUM(Total)
			for WeekInt in ([1],[2],[3],[4],[5],[6])
		  )pvt
	UNION
		Select '1' OrderNo3
			 , '0' OrderNo
			 , '0' OrderNo1
			 , '0' OrderNo2
			 , GroupNo
			 , Area
			 , CompanyCode
			 , DealerAbbreviation
			 , BranchCode
			 , TipeKendaraan
			 , Variant
			 , SeqNo
			 , LastProgress
			 ,(select COUNT(*) 
			 from #VehicleWoAreaDlr 
			where GroupNo = a.GroupNo
		      and Area = a.Area
		      and CompanyCode = a.CompanyCode 
			  and BranchCode = a.BranchCode 
			  and TipeKendaraan = a.TipeKendaraan
			  and Variant = a.Variant
			  and SeqNo = a.SeqNo
			  and LastProgress = a.LastProgress 
			  and convert(varchar,UpdateDate,112) < @StartDate 
			  and convert(varchar,InquiryDate,112) < @StartDate) SaldoAwal
			 , 0, 0, 0, 0, 0, 0
			 , 0, 0, 0, 0, 0, 0
		  from #VehicleTemplateWoAreaDlr a
		  where not exists (select 1 
							  from #TestSummaryFirstDlr
							 where GroupNo = a.GroupNo
							   and Area = a.Area
							   and CompanyCode = a.CompanyCode
							   and BranchCode = a.BranchCode
							   and TipeKendaraan = a.TipeKendaraan
							   and Variant = a.Variant
							   and SeqNo = a.SeqNo
							   and LastProgress = a.LastProgress
							   and (SaldoAwal + TotalOuts + Total) <> 0)
	)#TestPivotFirstDlr
	
	select * into #TestPivotOutsFirstDlr from(
		select OrderNo3
			 , OrderNo
			 , OrderNo1
			 , OrderNo2
			 , GroupNo
			 , Area
			 , CompanyCode
			 , DealerAbbreviation
			 , BranchCode
			 , TipeKendaraan
			 , Variant
			 , SeqNo
			 , LastProgress
			 , SaldoAwal
			 , 0 Week1, 0 Week2, 0 Week3, 0 Week4, 0 Week5, 0 Week6
			 , isnull([1],0) WeekOuts1 , isnull([2],0) WeekOuts2 , isnull([3],0) WeekOuts3 ,isnull([4],0) WeekOuts4 , isnull([5],0) WeekOuts5 , isnull([6],0) WeekOuts6 
		  from (
			select OrderNo3
				 , OrderNo
				 , OrderNo1
				 , OrderNo2
				 , GroupNo
				 , Area
				 , CompanyCode
				 , DealerAbbreviation
				 , BranchCode
				 , TipeKendaraan
				 , Variant
				 , SeqNo
				 , LastProgress
				 , SaldoAwal
				 , weekInt
				 , TotalOuts
			 from #TestSummaryFirstDlr
			where (SaldoAwal + TotalOuts + Total) <> 0
		  ) as Header
		  pivot (SUM(TotalOuts)
			for WeekInt in ([1],[2],[3],[4],[5],[6])
		  )pvt
	UNION
		Select '1' OrderNo3
			 , '0' OrderNo
			 , '0' OrderNo1
			 , '0' OrderNo2
			 , GroupNo
			 , Area
			 , CompanyCode
			 , DealerAbbreviation
			 , BranchCode
			 , TipeKendaraan
			 , Variant
			 , SeqNo
			 , LastProgress
			 , 0
			 , 0, 0, 0, 0, 0, 0
			 , 0, 0, 0, 0, 0, 0
		  from #VehicleTemplateWoAreaDlr a
		  where not exists (select 1 
							  from #TestSummaryFirstDlr
							 where CompanyCode = a.CompanyCode
							   and BranchCode = a.BranchCode
							   and TipeKendaraan = a.TipeKendaraan
							   and Variant = a.Variant
							   and SeqNo = a.SeqNo
							   and LastProgress = a.LastProgress
							   and (SaldoAwal + TotalOuts + Total) <> 0)
	)#TestPivotOutsFirstDlr

	select * into #TestFinalSummaryFirstDlr from(
		select '1' OrderNo3
			 , '0' OrderNo
			 , '0' OrderNo1
			 , '0' OrderNo2
			 , a.GroupNo
			 , a.Area
			 , a.CompanyCode
			 , a.DealerAbbreviation
			 , a.BranchCode
			 , a.TipeKendaraan
			 , a.Variant
			 , a.SeqNo
			 , a.LastProgress
			 , a.SaldoAwal
			 , case when b.LastProgress = 'P' then 0 else b.WeekOuts1 end WeekOuts1
			 , case when b.LastProgress = 'P' then 0 else b.WeekOuts2 end WeekOuts2
			 , case when b.LastProgress = 'P' then 0 else b.WeekOuts3 end WeekOuts3
			 , case when b.LastProgress = 'P' then 0 else b.WeekOuts4 end WeekOuts4
			 , case when b.LastProgress = 'P' then 0 else b.WeekOuts5 end WeekOuts5
			 , case when b.LastProgress = 'P' then 0 else b.WeekOuts6 end WeekOuts6
			 , a.Week1
			 , a.Week2
			 , a.Week3
			 , a.Week4
			 , a.Week5
			 , a.Week6
		  from #TestPivotFirstDlr a
		  left join #TestPivotOutsFirstDlr b on a.GroupNo = b.GroupNo
				and a.OrderNo = b.OrderNo
				and a.OrderNo1 = b.OrderNo1
				and a.OrderNo2 = b.OrderNo2
			    and a.GroupNo = b.GroupNo
				and a.Area = b.Area
				and a.CompanyCode = b.CompanyCode
				and a.BranchCode = b.BranchCode
				and a.TipeKendaraan = b.TipeKendaraan
				and a.Variant = b.Variant
				and a.SeqNo = b.SeqNo
				and a.LastProgress = b.LastProgress
	)#TestFinalSummaryFirstDlr
	
	select * into #SummaryDetailDlr from(
			Select a.OrderNo3
				 , a.OrderNo
				 , a.OrderNo1
				 , a.OrderNo2
				 , a.GroupNo
				 , a.Area
				 , a.CompanyCode
				 , a.DealerAbbreviation
				 , a.BranchCode
				 , a.TipeKendaraan
				 , a.Variant
				 , a.SeqNo
				 , a.LastProgress
				 , a.SaldoAwal
				 , a.WeekOuts1
				 , a.WeekOuts2
				 , a.WeekOuts3
				 , a.WeekOuts4
				 , a.WeekOuts5
				 , a.WeekOuts6
				 , (case when a.LastProgress = 'P' then 0 else a.WeekOuts1 + a.WeekOuts2 + a.WeekOuts3 + a.WeekOuts4 + a.WeekOuts5 + a.WeekOuts6 end) TotalWeekOuts
				 , a.Week1
				 , a.Week2
				 , a.Week3
				 , a.Week4
				 , a.Week5
				 , a.Week6
				 , (a.Week1 + a.Week2 + a.Week3 + a.Week4 + a.Week5 + a.Week6) TotalWeek
				 , (case when a.LastProgress = 'P' then 0 else a.WeekOuts1 + a.WeekOuts2 + a.WeekOuts3 + a.WeekOuts4 + a.WeekOuts5 + a.WeekOuts6 end + a.Week1 + a.Week2 + a.Week3 + a.Week4 + a.Week5 + a.Week6) Total
			 from #TestFinalSummaryFirstDlr a
	)#SummaryDetailDlr
	
	select a.OrderNo3
		 , a.OrderNo
		 , a.OrderNo1
		 , a.OrderNo2
		 , a.GroupNo
		 , a.Area
		 , a.CompanyCode
		 , a.DealerAbbreviation CompanyName
		 , a.BranchCode
		 , isnull(c.OutletAbbreviation,'') BranchName
		 , a.TipeKendaraan
		 , a.Variant
		 , a.SeqNo
		 , a.LastProgress
		 , a.SaldoAwal
		 , a.WeekOuts1
		 , a.WeekOuts2
		 , a.WeekOuts3
		 , a.WeekOuts4
		 , a.WeekOuts5
		 , a.WeekOuts6
		 , a.TotalWeekOuts
		 , a.Week1
		 , a.Week2
		 , a.Week3
		 , a.Week4
		 , a.Week5
		 , a.Week6
		 , a.TotalWeek
		 , a.Total
	from #SummaryDetailDlr a
	left join gnMstDealerOutletMapping c on a.CompanyCode = c.DealerCode and a.BranchCode = c.OutletCode
	where OrderNo1 <> 2
	order by a.OrderNo3 Asc,a.GroupNo asc, a.DealerAbbreviation asc,a.Area asc,a.OrderNo2 asc,a.BranchCode asc,a.OrderNo1 asc, a.TipeKendaraan Asc, a.OrderNo Asc, a.Variant Asc, a.SeqNo Asc
-- Query : uspfn_InquiryITSStatus '20130301','20130330','','','','','',0
	
	drop table #SummaryDetailDlr
	drop table #TestFinalSummaryFirstDlr
	drop table #TestPivotOutsFirstDlr
	drop table #TestPivotFirstDlr
	drop table #TestSummaryFirstDlr
	drop table #VehicleWoAreaDlr
	drop table #VehicleTemplateWoAreaDlr
	drop table #StatusWoAreaDlr
end


GO


