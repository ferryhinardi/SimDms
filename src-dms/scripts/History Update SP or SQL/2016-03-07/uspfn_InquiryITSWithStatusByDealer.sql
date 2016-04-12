CREATE PROCEDURE [dbo].[uspfn_InquiryITSWithStatusByDealer]  
--DECLARE  
    @StartDate			varchar(20),
	@EndDate			varchar(20),
	@Area				varchar(100),
	@CompanyCode		varchar(15),
	@BranchCode			varchar(15),
	@GroupModel			varchar(100),
	@TipeKendaraan		varchar(100),
	@Variant			varchar(100),
	@Summary			int
AS  
begin  

--uspfn_InquiryITSWithStatusByDealer_rev   '','','20151201','20151201','20151101','20151101','','','','',1
		  

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
		--INNER JOIN gnMstDealerMapping d 
		--	on a.CompanyCode = d.DealerCode-- and d.isActive = 1
		--INNER JOIN gnMstDealerOutletMapping e 
		--	on a.CompanyCode = e.DealerCode and a.BranchCode = e.OutletCode and e.GroupNo = d.GroupNo-- and e.isActive = 1
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
	 --INNER JOIN gnMstDealerMapping c on a.CompanyCode = c.DealerCode-- and c.isActive = 1
	 --INNER JOIN gnMstDealerOutletMapping d on a.CompanyCode = d.DealerCode and a.BranchCode = d.OutletCode and d.GroupNo = c.GroupNo --and d.isActive = 1
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
	 --INNER JOIN gnMstDealerMapping c on a.CompanyCode = c.DealerCode-- and c.isActive = 1
	 --INNER JOIN gnMstDealerOutletMapping d on a.CompanyCode = d.DealerCode and a.BranchCode = d.OutletCode and d.GroupNo = c.GroupNo --and d.isActive = 1
	 WHERE convert(date,InquiryDate) between @Startdate AND @EndDate
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
 --and a.IsActive = 1  
 and a.Area like Case when @Area = '' then '%%' else @Area end  
 and a.DealerCode like Case when @CompanyCode = '' then '%%' else @CompanyCode end      
 and b.OutletCode  like case when @BranchCode='' then '%%' else @BranchCode end  
 ) #tGabung  
order by GroupNo, CompanyCode, BranchCode, TipeKendaraan, Variant, SeqNo  

-- Level 0  
select * into #tFinal from(  
select   
 1 OrderNo3  
 , 0 OrderNo  
 , 0 OrderNo1  
 , 0 OrderNo2  
   
 ,  GroupNo  
 ,  Area  
 ,  CompanyCode  
 ,  CompanyName  
 ,  BranchCode   
 ,  BranchName  
 ,  TipeKendaraan  
 ,  Variant  
 ,  SeqNo  
 ,  LastProgress    
   
 ,  SaldoAwal   
 ,  WeekOuts1   
 ,  WeekOuts2    
 ,  WeekOuts3    
 ,  WeekOuts4  
 ,  WeekOuts5  
 ,  TotalWeekOuts  
 ,  Week1  
 ,  Week2  
 ,  Week3  
 ,  Week4  
 ,  Week5  
 ,  TotalWeek   
 ,  Total  
  
 from #tGabung  
union  
---- Group per Tipe Kendaraan  
select   
 1 OrderNo3  
 , 1 OrderNo  
 , 0 OrderNo1  
 , 0 OrderNo2  
   
 ,  GroupNo  
 ,  Area  
 ,  CompanyCode  
 ,  CompanyName  
 ,  BranchCode   
 ,  BranchName  
 ,  TipeKendaraan  
 ,  '' Variant  
 ,  SeqNo  
 ,  LastProgress    
   
 ,  sum(SaldoAwal) SaldoAwal  
 ,  sum(WeekOuts1) WeekOuts1  
 ,  sum(WeekOuts2) WeekOuts2  
 ,  sum(WeekOuts3) WeekOuts3  
 ,  sum(WeekOuts4) WeekOuts4  
 ,  sum(WeekOuts5) WeekOuts5  
 ,  sum(TotalWeekOuts) TotalWeekOuts  
 ,  sum(Week1) Week1  
 ,  sum(Week2) Week2  
 ,  sum(Week3) Week3  
 ,  sum(Week4) Week4  
 ,  sum(Week5) Week5  
 ,  sum(TotalWeek) TotalWeek  
 ,  sum(Total) Total  

 from #tGabung  
group by   
  GroupNo  
 ,   Area  
 ,   CompanyCode  
 ,   CompanyName  
 ,   BranchCode   
 ,   BranchName  
 ,   TipeKendaraan    
 ,   SeqNo  
 ,   LastProgress   
union  
-- Group per Company  
select   
 1 OrderNo3  
 , 2 OrderNo  
 , 1 OrderNo1  
 , 0 OrderNo2  
  
 ,  GroupNo  
 ,  Area  
 ,  CompanyCode  
 ,  CompanyName  
 ,  BranchCode   
 ,  BranchName  
 ,  '' TipeKendaraan  
 ,  '' Variant  
 ,  SeqNo  
 ,  LastProgress    
   
 ,  sum(SaldoAwal) SaldoAwal  
 ,  sum(WeekOuts1) WeekOuts1  
 ,  sum(WeekOuts2) WeekOuts2  
 ,  sum(WeekOuts3) WeekOuts3  
 ,  sum(WeekOuts4) WeekOuts4  
 ,  sum(WeekOuts5) WeekOuts5  
 ,  sum(TotalWeekOuts) TotalWeekOuts  
 ,  sum(Week1) Week1  
 ,  sum(Week2) Week2  
 ,  sum(Week3) Week3  
 ,  sum(Week4) Week4  
 ,  sum(Week5) Week5  
 ,  sum(TotalWeek) TotalWeek  
 ,  sum(Total) Total  

 from #tGabung  
group by  GroupNo  
 ,  Area  
 ,  CompanyCode  
 ,  CompanyName  
 ,  BranchCode   
 ,  BranchName   
 ,  SeqNo  
 ,  LastProgress    
union  
select   
 1 OrderNo3  
 , 2 OrderNo  
 , 2 OrderNo1  
 , 0 OrderNo2  
  
 ,  GroupNo  
 ,  Area  
 ,  '' CompanyCode  
 ,  'TOTAL' CompanyName  
 ,  '' BranchCode   
 ,  '' BranchName  
 ,  '' TipeKendaraan  
 ,  '' Variant  
 ,  SeqNo  
 ,  LastProgress    
   
 ,  sum(SaldoAwal) SaldoAwal  
 ,  sum(WeekOuts1) WeekOuts1  
 ,  sum(WeekOuts2) WeekOuts2  
 ,  sum(WeekOuts3) WeekOuts3  
 ,  sum(WeekOuts4) WeekOuts4  
 ,  sum(WeekOuts5) WeekOuts5  
 ,  sum(TotalWeekOuts) TotalWeekOuts  
 ,  sum(Week1) Week1  
 ,  sum(Week2) Week2  
 ,  sum(Week3) Week3  
 ,  sum(Week4) Week4  
 ,  sum(Week5) Week5  
 ,  sum(TotalWeek) TotalWeek  
 ,  sum(Total) Total  
   
 from #tGabung  
group by  GroupNo  
 ,  Area   
 ,  SeqNo  
 ,  LastProgress     
union  
-- Group per Area  
select   
 1 OrderNo3  
 , 2 OrderNo  
 , 1 OrderNo1  
 , 1 OrderNo2  
   
 ,  GroupNo   
 ,  Area   
 ,  CompanyCode  
 ,  CompanyName   
 ,  'TOTAL' BranchCode   
 ,  '' BranchName    
 ,  '' TipeKendaraan  
 ,  '' Variant  
 ,  SeqNo  
 ,  LastProgress    
   
 ,  sum(SaldoAwal) SaldoAwal  
 ,  sum(WeekOuts1) WeekOuts1  
 ,  sum(WeekOuts2) WeekOuts2  
 ,  sum(WeekOuts3) WeekOuts3  
 ,  sum(WeekOuts4) WeekOuts4  
 ,  sum(WeekOuts5) WeekOuts5  
 ,  sum(TotalWeekOuts) TotalWeekOuts  
 ,  sum(Week1) Week1  
 ,  sum(Week2) Week2  
 ,  sum(Week3) Week3  
 ,  sum(Week4) Week4  
 ,  sum(Week5) Week5  
 ,  sum(TotalWeek) TotalWeek  
 ,  sum(Total) Total  
    
 from #tGabung  
group by GroupNo   
 ,  Area   
 ,  CompanyCode  
 ,  CompanyName   
 , BranchName  
 ,  SeqNo  
 ,  LastProgress    
union  
-- TOTAL  
select   
 2 OrderNo3  
 , 2 OrderNo  
 , 1 OrderNo1  
 , 1 OrderNo2  
   
 ,  999999 GroupNo   
 ,  'TOTAL' Area   
 ,  '' CompanyCode  
 ,  'TOTAL' CompanyName   
 ,  '' BranchCode   
 ,  '' BranchName    
 ,  '' TipeKendaraan  
 ,  '' Variant  
 ,  SeqNo  
 ,  LastProgress    

 ,  sum(SaldoAwal) SaldoAwal  
 ,  sum(WeekOuts1) WeekOuts1  
 ,  sum(WeekOuts2) WeekOuts2  
 ,  sum(WeekOuts3) WeekOuts3  
 ,  sum(WeekOuts4) WeekOuts4  
 ,  sum(WeekOuts5) WeekOuts5  
 ,  sum(TotalWeekOuts) TotalWeekOuts  
 ,  sum(Week1) Week1  
 ,  sum(Week2) Week2  
 ,  sum(Week3) Week3  
 ,  sum(Week4) Week4  
 ,  sum(Week5) Week5  
 ,  sum(TotalWeek) TotalWeek  
 ,  sum(Total) Total  
 from #tGabung  
group by SeqNo  
 ,  LastProgress) #tFinal  
  
if(@Summary = 0)  
begin   
select   
    OrderNo3  
   ,  OrderNo  
   ,  OrderNo1  
   ,  OrderNo2  
   ,  GroupNo  
   ,  Area  
   ,  CompanyCode  
   ,  CompanyName  
   ,  BranchCode  
   ,  BranchName  
   ,  SeqNo  
   ,  LastProgress       
     
   ---- THIS MONTH   
   , sum(SaldoAwal) SaldoAwal  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts1) END WeekOuts1  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts2) END WeekOuts2  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts3) END WeekOuts3  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts4) END WeekOuts4  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts5) END WeekOuts5 
   , 0 WeekOuts6    
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END TotalWeekOuts  
   , sum(Week1) Week1  
   , sum(Week2) Week2   
   , sum(Week3) Week3  
   , sum(Week4) Week4  
   , sum(Week5) Week5
   , 0 Week6  
   , sum(TotalWeek) TotalWeek  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek) Total  
     
from #tFinal  
 where OrderNo > 0 and OrderNo1 > 0  
  
 group by  OrderNo3  
   ,  OrderNo  
   ,  OrderNo1  
   ,  OrderNo2  
   ,  GroupNo  
   ,  Area  
   ,  CompanyCode  
   ,  CompanyName  
   ,  BranchCode  
   ,  BranchName  
   ,  SeqNo  
   ,  LastProgress   
 --order by OrderNo3 Asc,GroupNo asc, CompanyName asc,Area asc,OrderNo1 asc,OrderNo asc,BranchCode asc,OrderNo2 Asc,SeqNo Asc  
 --order by  OrderNo3 Asc, GroupNo asc,  CompanyName asc, Area asc, OrderNo2 asc, OrderNo1 asc, BranchCode asc,  OrderNo Asc,  SeqNo Asc  
 order by  OrderNo3 Asc, GroupNo asc, OrderNo1 asc, CompanyName asc, BranchCode asc, BranchName asc,Area asc,  OrderNo2 asc,  OrderNo Asc,  SeqNo Asc  
  
select   
    OrderNo3  
   ,  OrderNo  
   ,  OrderNo1  
   ,  OrderNo2  
   ,  GroupNo  
   ,  Area  
   ,  CompanyCode  
   ,  CompanyName  
   ,  BranchCode  
   ,  BranchName  
   ,  TipeKendaraan  
   ,  Variant  
   ,  SeqNo  
   ,  LastProgress       
     
   ---- THIS MONTH   
   , sum(SaldoAwal) SaldoAwal  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts1) END WeekOuts1  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts2) END WeekOuts2  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts3) END WeekOuts3  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts4) END WeekOuts4  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts5) END WeekOuts5 
   , 0 WeekOuts6    
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END TotalWeekOuts  
   , sum(Week1) Week1  
   , sum(Week2) Week2   
   , sum(Week3) Week3  
   , sum(Week4) Week4  
   , sum(Week5) Week5 
   , 0 Week5 
   , sum(TotalWeek) TotalWeek  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek) Total  
     
from #tFinal  
 where OrderNo1 <> 2  
 group by  OrderNo3  
   ,  OrderNo  
   ,  OrderNo1  
   ,  OrderNo2  
   ,  GroupNo  
   ,  Area  
   ,  CompanyCode  
   ,  CompanyName  
   ,  BranchCode  
   ,  BranchName  
   ,  TipeKendaraan  
   ,  Variant  
   ,  SeqNo  
   ,  LastProgress  
  order by  OrderNo3 Asc, GroupNo asc,  CompanyName asc, Area asc, OrderNo2 asc, BranchCode asc, OrderNo1 asc,  TipeKendaraan Asc,  OrderNo Asc,  Variant Asc,  SeqNo Asc  
  --order by  OrderNo3 Asc, GroupNo asc,  CompanyName asc, Area asc, OrderNo2 asc, BranchCode asc, OrderNo1 asc,  TipeKendaraan Asc,  OrderNo Asc,  Variant Asc,  SeqNo Asc  
  --order by  OrderNo3 Asc,  TipeKendaraan Asc, GroupNo asc,  CompanyName asc, OrderNo asc, OrderNo1 asc, BranchCode asc, OrderNo2 Asc, Area asc, SeqNo Asc     
end  
else if (@Summary = 1)  
begin  
 select   
    OrderNo3  
   ,  OrderNo  
   ,  OrderNo1  
   ,  OrderNo2  
   --,  GroupNo  
   ,  Area  
   ,  CompanyCode  
   ,  CompanyName  
   ,  BranchCode  
   ,  BranchName  
   ,  TipeKendaraan  
   ,  Variant  
   ,  SeqNo  
   ,  LastProgress       

   ---- THIS MONTH   
   , sum(SaldoAwal) SaldoAwal  
   --, CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts1) END WeekOuts1  
   --, CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts2) END WeekOuts2  
   --, CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts3) END WeekOuts3  
   --, CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts4) END WeekOuts4  
   --, CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts5) END WeekOuts5     
   --, CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END TotalWeekOuts  
   , sum(WeekOuts1) + sum(Week1) Week1  
   , sum(WeekOuts2) + sum(Week2) Week2   
   , sum(WeekOuts2) + sum(Week3) Week3  
   , sum(WeekOuts2) + sum(Week4) Week4  
   , sum(WeekOuts2) + sum(Week5) Week5  
   , sum(WeekOuts2) + sum(TotalWeek) TotalWeek  
   , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek) Total  
     
from #tFinal  
 group by  OrderNo3  
   ,  OrderNo  
   ,  OrderNo1  
   ,  OrderNo2  
   ,  GroupNo  
   ,  Area  
   ,  CompanyCode  
   ,  CompanyName  
   ,  BranchCode  
   ,  BranchName  
   ,  TipeKendaraan  
   ,  Variant  
   ,  SeqNo  
   ,  LastProgress  
 order by  OrderNo3 Asc, GroupNo asc,  CompanyName asc, Area asc, OrderNo2 asc, BranchCode asc, OrderNo1 asc,  TipeKendaraan Asc,  OrderNo Asc,  Variant Asc,  SeqNo Asc    
  --order by  OrderNo3 Asc,  TipeKendaraan Asc, GroupNo asc,  CompanyName asc, OrderNo asc, OrderNo1 asc, BranchCode asc, OrderNo2 Asc, Area asc, SeqNo Asc     
end  
  
drop table #t1Out, #t1New, #t2, #t3, #tUnion1, #tVehicle, #tGabung, #tFinal, #t1NewP
  
end