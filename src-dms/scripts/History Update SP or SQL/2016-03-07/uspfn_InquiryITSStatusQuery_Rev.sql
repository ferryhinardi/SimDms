CREATE PROCEDURE [dbo].[uspfn_InquiryITSStatusQuery_Rev]
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

--declare @CompanyCode  varchar(15) = '6006400001',  
-- @BranchCode   varchar(15) = '6006400104',  
-- @StartDate   varchar(20) = '20151201',  
-- @EndDate   varchar(20) = '20151231',  
-- @GroupModel   varchar(100) = 'OTHERS',  
-- @TipeKendaraan  varchar(100) = 'NEW ERTIGA' ,
-- @Variant   varchar(100) = ''


select * into #t1OutMax from(
SELECT a.InquiryNumber, a.CompanyCode, a.BranchCode, MAX(a.UpdateDate) UpdateDate 
			  FROM pmStatusHistory a with (NOWAIT, NOLOCK) 
			 WHERE 1=1
			   AND CONVERT(DATE, a.UpdateDate) < @StartDate
			 GROUP BY InquiryNumber, CompanyCode, BranchCode
)#t1OutMax

select * into #t1Out from(  
select   
 c.CompanyCode  
 , c.BranchCode   
 , isnull((select TOP 1 GroupModel   
       from msMstGroupModel  
      where ModelType = c.TipeKendaraan),'OTHERS') GroupModel  
 , isnull(c.TipeKendaraan,'') TipeKendaraan  
 , isnull(c.Variant,'') Variant  
 , CONVERT(DATE, a.UpdateDate) UpdateDate  
 , b.LastProgress   
 , CASE   
   WHEN day(a.UpdateDate) >= 1 and day(a.UpdateDate) <= 7  THEN 1  
   WHEN day(a.UpdateDate) >= 8 and day(a.UpdateDate) <= 14  THEN 2  
   WHEN day(a.UpdateDate) >= 15 and day(a.UpdateDate) <= 21  THEN 3  
   WHEN day(a.UpdateDate) >= 22 and day(a.UpdateDate) <= 28  THEN 4  
   WHEN day(a.UpdateDate) >= 29 and day(a.UpdateDate) <= 31  THEN 5  
    END WeekInt     
		FROM #t1OutMax a
		INNER JOIN (SELECT InquiryNumber,CompanyCode,BranchCode,UpdateDate ,LastProgress from pmStatusHistory) b
			ON b.InquiryNumber = a.InquiryNumber AND b.CompanyCode = a.CompanyCode 
			AND b.BranchCode = a.BranchCode AND b.UpdateDate = a.UpdateDate
			AND LastProgress IN ('P','HP','SPK')
		INNER JOIN (SELECT CompanyCode, BranchCode, InquiryNumber, TipeKendaraan, Variant from pmKDP with (NOWAIT, NOLOCK)) c 
			on c.InquiryNumber = a.InquiryNumber AND c.BranchCode = a.BranchCode AND c.CompanyCode = a.CompanyCode
 ) #t1Out  
order by CompanyCode, BranchCode, TipeKendaraan, Variant

select * into #t1New from(  
select   
 b.CompanyCode  
 , b.BranchCode   
 , isnull((select TOP 1 GroupModel   
       from msMstGroupModel 
      where ModelType = b.TipeKendaraan),'OTHERS') GroupModel  
 , isnull(b.TipeKendaraan,'') TipeKendaraan  
 , isnull(b.Variant,'') Variant  
 , CONVERT(date,b.InquiryDate) InquiryDate
 , CONVERT(date,a.UpdateDate) UpdateDate  
 , a.LastProgress   
 , CASE   
   WHEN day(a.UpdateDate) >= 1 and day(a.UpdateDate) <= 7  THEN 1  
   WHEN day(a.UpdateDate) >= 8 and day(a.UpdateDate) <= 14  THEN 2  
   WHEN day(a.UpdateDate) >= 15 and day(a.UpdateDate) <= 21  THEN 3  
   WHEN day(a.UpdateDate) >= 22 and day(a.UpdateDate) <= 28  THEN 4  
   WHEN day(a.UpdateDate) >= 29 and day(a.UpdateDate) <= 31  THEN 5  
    END WeekInt
	  FROM (select distinct inquirynumber,companycode,branchcode,UpdateDate, hh.LastProgress from pmStatusHistory  hh                   
        where 1=1 --and hh.LastProgress='SPK'   
        and convert(date,hh.UpdateDate) BETWEEN @StartDate AND @EndDate   
        and hh.sequenceno=( select max(sequenceno) from pmStatusHistory   
						  where inquirynumber=hh.inquirynumber and companycode=hh.companycode and branchcode=hh.branchcode 
						  AND hh.LastProgress = LastProgress 
						  AND LastProgress in (select LookupValue  from gnMstLookupDtl where CodeID = 'PSTS' ) 
						  group by inquirynumber,companycode,branchcode, LastProgress)
		
	  ) a 
	 LEFT JOIN (SELECT CompanyCode, BranchCode, InquiryNumber, InquiryDate, TipeKendaraan, Variant from pmKDP with (NOWAIT, NOLOCK)) b 
	 on b.InquiryNumber = a.InquiryNumber AND b.BranchCode = a.BranchCode AND b.CompanyCode = a.CompanyCode 
 ) #t1New  
order by CompanyCode, BranchCode, TipeKendaraan, Variant 

select * into #t1NewP from(  
select   
 a.CompanyCode  
 , a.BranchCode   
 , isnull((select TOP 1 GroupModel   
       from msMstGroupModel 
      where ModelType = a.TipeKendaraan),'OTHERS') GroupModel  
 , isnull(a.TipeKendaraan,'') TipeKendaraan  
 , isnull(a.Variant,'') Variant  
 , CONVERT(date,a.InquiryDate) InquiryDate
 , CONVERT(date,a.InquiryDate) UpdateDate  
 , 'P' LastProgress   
 , CASE   
   WHEN day(a.InquiryDate) >= 1 and day(a.InquiryDate) <= 7  THEN 1  
   WHEN day(a.InquiryDate) >= 8 and day(a.InquiryDate) <= 14  THEN 2  
   WHEN day(a.InquiryDate) >= 15 and day(a.InquiryDate) <= 21  THEN 3  
   WHEN day(a.InquiryDate) >= 22 and day(a.InquiryDate) <= 28  THEN 4  
   WHEN day(a.InquiryDate) >= 29 and day(a.InquiryDate) <= 31  THEN 5  
    END WeekInt
	  FROM pmKDP a with (NOWAIT, NOLOCK) 
	 INNER JOIN gnMstDealerMapping c on a.CompanyCode = c.DealerCode-- and c.isActive = 1
	 INNER JOIN gnMstDealerOutletMapping d on a.CompanyCode = d.DealerCode and a.BranchCode = d.OutletCode and d.GroupNo = c.GroupNo --and d.isActive = 1
	 WHERE convert(date,InquiryDate) between @StartDate AND @EndDate
 ) #t1NewP  

delete #t1New
where LastProgress = 'P'

INSERT into #t1New
SELECT * FROM #t1NewP

select * into #tVehicle from(  
select CompanyCode, BranchCode, TipeKendaraan, Variant  
from #t1Out   
where (case when @GroupModel <> '' then GroupModel else @GroupModel end) = @GroupModel  
 and (case when @TipeKendaraan <> '' then TipeKendaraan else @TipeKendaraan end) = @TipeKendaraan   
 and (case when @Variant <> '' then Variant else @Variant end) = @Variant   
group by CompanyCode, BranchCode, TipeKendaraan, Variant  
union      
select CompanyCode, BranchCode, TipeKendaraan, Variant  
from #t1New   
where (case when @GroupModel <> '' then GroupModel else @GroupModel end) = @GroupModel  
 and (case when @TipeKendaraan <> '' then TipeKendaraan else @TipeKendaraan end) = @TipeKendaraan   
 and (case when @Variant <> '' then Variant else @Variant end) = @Variant   
group by CompanyCode, BranchCode, TipeKendaraan, Variant  
) #tVehicle  

select * into #t2 from(  
select SeqNo, LookupValue LastProgress  
from gnMstLookupDtl  
where CodeID = 'PSTS'  
) #t2 order by SeqNo  
  
select * into #t3 from(  
select 1 WeekInt union select 2 WeekInt union select 3 WeekInt union select 4 WeekInt union select 5 WeekInt  
) #t3  
   
select * into #tUnion1 from(     
SELECT CompanyCode, BranchCode, TipeKendaraan, Variant , LastProgress
		,SUM(SaldoAwal) as SaldoAwal, SUM(WeekOuts1) as WeekOuts1, SUM(WeekOuts2) as WeekOuts2, SUM(WeekOuts3) as WeekOuts3
		,SUM(WeekOuts4) as WeekOuts4, SUM(WeekOuts5) as WeekOuts5, SUM(TotalWeekOuts) as TotalWeekOuts 
		,SUM(Week1) as Week1, SUM(Week2) as Week2, SUM(Week3) as Week3, SUM(Week4) as Week4
		,SUM(Week5) as Week5, SUM(TotalWeek) as TotalWeek
		FROM (
 --Outs 
	select CompanyCode, BranchCode, TipeKendaraan, Variant , LastProgress 
	, (select count(UpdateDate) from #t1Out  
	 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt and UpdateDate < @StartDate ) SaldoAwal   
	, 0 WeekOuts1   
	, 0 WeekOuts2    
	, 0 WeekOuts3    
	, 0 WeekOuts4  
	, 0 WeekOuts5  
	, 0 TotalWeekOuts   
	, 0 Week1   
	, 0 Week2    
	, 0 Week3    
	, 0 Week4  
	, 0 Week5  
	, 0 TotalWeek   
	from #t1Out a    
	group by CompanyCode, BranchCode, GroupModel, TipeKendaraan, Variant , LastProgress, UpdateDate, WeekInt 
	UNION ALL   
	-- New  
	select CompanyCode, BranchCode, TipeKendaraan, Variant , LastProgress  
	, 0 SaldoAwal 
	, (select count(UpdateDate) from #t1New   
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt  and WeekInt = 1 and (UpdateDate between @StartDate and @EndDate) AND InquiryDate < @StartDate) WeekOuts1   
	, (select count(UpdateDate) from #t1New   
	 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt and WeekInt = 2 and (UpdateDate between @StartDate and @EndDate) AND InquiryDate < @StartDate) WeekOuts2    
	, (select count(UpdateDate) from #t1New   
	 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt  and WeekInt = 3 and (UpdateDate between @StartDate and @EndDate) AND InquiryDate < @StartDate) WeekOuts3    
	, (select count(UpdateDate) from #t1New   
	 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt and WeekInt = 4 and (UpdateDate between @StartDate and @EndDate) AND InquiryDate < @StartDate) WeekOuts4  
	, (select count(UpdateDate) from #t1New   
	 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt  and WeekInt = 5 and (UpdateDate between @StartDate and @EndDate) AND InquiryDate < @StartDate) WeekOuts5  
	, (select count(UpdateDate) from #t1New   
	 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt  and (UpdateDate between @StartDate and @EndDate) AND InquiryDate < @StartDate) TotalWeekOuts    
	, (select count(UpdateDate) from #t1New   
	 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt and WeekInt = 1 and UpdateDate between @StartDate and @EndDate
	 and (InquiryDate BETWEEN @StartDate AND @EndDate or InquiryDate > @EndDate)) Week1   
	,  (select count(UpdateDate) from #t1New   
	 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt and WeekInt = 2 and UpdateDate between @StartDate and @EndDate
	 and (InquiryDate BETWEEN @StartDate AND @EndDate or InquiryDate > @EndDate)) Week2    
	, (select count(UpdateDate) from #t1New   
	 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt and WeekInt = 3 and UpdateDate between @StartDate and @EndDate
	 and (InquiryDate BETWEEN @StartDate AND @EndDate or InquiryDate > @EndDate)) Week3    
	, (select count(UpdateDate) from #t1New   
	 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt and WeekInt = 4 and UpdateDate between @StartDate and @EndDate
	 and (InquiryDate BETWEEN @StartDate AND @EndDate or InquiryDate > @EndDate)) Week4  
	, (select count(UpdateDate) from #t1New   
	 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt and WeekInt = 5 and UpdateDate between @StartDate and @EndDate
	 and (InquiryDate BETWEEN @StartDate AND @EndDate or InquiryDate > @EndDate)) Week5  
	, (select count(UpdateDate) from #t1New   
	 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode AND GroupModel = a.GroupModel 
	 and TipeKendaraan = a.TipeKendaraan and Variant = a.Variant AND UpdateDate = a.UpdateDate
	 and LastProgress = a.LastProgress and WeekInt = a.WeekInt and UpdateDate between @StartDate and @EndDate
	 and (InquiryDate BETWEEN @StartDate AND @EndDate or InquiryDate > @EndDate)) TotalWeek     
	from #t1New a   
	group by CompanyCode, BranchCode, GroupModel, TipeKendaraan, Variant , LastProgress, UpdateDate, WeekInt       
) #tUnion 
GROUP by CompanyCode, BranchCode, TipeKendaraan, Variant , LastProgress
) #tUnion1  

-- This Month  
select * into #tGabung from(   
select distinct  
 a.GroupNo  
 , a.Area  
 , a.DealerCode CompanyCode  
 , a.DealerAbbreviation CompanyName  
 , b.OutletCode BranchCode   
 , b.OutletAbbreviation BranchName  
 , d.TipeKendaraan  
 , d.Variant  
 , e.SeqNo  
 , e.LastProgress    
   
 , isnull(g.SaldoAwal, 0) SaldoAwal  
 , isnull(g.WeekOuts1, 0) WeekOuts1  
 , isnull(g.WeekOuts2, 0) WeekOuts2  
 , isnull(g.WeekOuts3, 0) WeekOuts3  
 , isnull(g.WeekOuts4, 0) WeekOuts4  
 , isnull(g.WeekOuts5, 0) WeekOuts5  
 , isnull(g.TotalWeekOuts, 0) TotalWeekOuts  
   
 , isnull(g.Week1, 0) Week1  
 , isnull(g.Week2, 0) Week2  
 , isnull(g.Week3, 0) Week3  
 , isnull(g.Week4, 0) Week4  
 , isnull(g.Week5, 0) Week5  
 , isnull(g.TotalWeek, 0) TotalWeek  
   
 , isnull(g.TotalWeekOuts, 0) + isnull(g.TotalWeek, 0) Total    
  
from gnMstDealerMapping a with (nolock, nowait)  
left join gnMstDealerOutletMapping b with (nolock, nowait) on a.DealerCode = b.DealerCode  
and a.GroupNo = b.GroupNo  
left join #tVehicle d with (nolock, nowait) on d.CompanyCode = a.DealerCode and d.BranchCode = b.OutletCode  
left join #t2 e with (nolock, nowait) on e.SeqNo > 0 
--left join #t3 f with (nolock, nowait) on f.WeekInt > 0  
left join #tUnion1 g with (nolock, nowait) on g.CompanyCode = a.DealerCode and g.BranchCode = b.OutletCode and g.TipeKendaraan =  d.TipeKendaraan  
 and g.Variant = d.Variant   
 and g.LastProgress = e.LastProgress  
where 1=1   
 and a.DealerCode like Case when @CompanyCode = '' then '%%' else @CompanyCode end      
 and b.OutletCode  like case when @BranchCode='' then '%%' else @BranchCode end  
 ) #tGabung  
order by GroupNo, CompanyCode, BranchCode, TipeKendaraan, Variant, SeqNo  

SELECT * FROM #tGabung

drop TABLE #t1OutMax, #t1Out, #t1New, #t1NewP, #tVehicle, #t2, #t3, #tUnion1, #tGabung

