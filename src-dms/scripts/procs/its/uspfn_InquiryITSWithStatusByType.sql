if object_id('uspfn_InquiryITSWithStatusByType') is not null
	drop procedure uspfn_InquiryITSWithStatusByType
GO
create PROCEDURE [dbo].[uspfn_InquiryITSWithStatusByType]
	@CompanyCode		varchar(15),
	@BranchCode			varchar(15),
	@StartDate			varchar(20),
	@EndDate			varchar(20),
	@LastStartDate		varchar(20),
	@LastEndDate		varchar(20),
	@Area				varchar(100),
	@GroupModel			varchar(100),
	@TipeKendaraan		varchar(100),
	@Variant			varchar(100),
	@Summary			bit
AS
begin

--exec uspfn_InquiryITSWithStatusByType '', '', '20140401', '20140415', '20140301', '20140328', '', '', '', '', 1

--Declare @CompanyCode	varchar(15)
--declare @BranchCOde		varchar(15)
--Declare @StartDate		varchar(20)
--Declare @EndDate		varchar(20)
--Declare @LastStartDate	varchar(20)
--Declare @LastEndDate	varchar(20)
--Declare @Area			varchar(100)
--declare @GroupModel		varchar(100)
--declare @TipeKendaraan	varchar(100)
--declare @Variant		varchar(100)
--declare @Summary		bit

--set @CompanyCode = ''
--set @BranchCode = ''
--set @StartDate = '20140401'
--set @EndDate = '20140415'
--set @LastStartDate = '20140301'
--set @LastEndDate = '20140328'
--set @Area = ''
--set @GroupModel = ''
--set @TipeKendaraan = ''
--set @Variant = ''
--set @Summary = 0

select * into #tThis from(
select c.CompanyCode, c.BranchCode 	
	, c.LastProgress
	, c.InquiryNumber
	, convert(varchar,c.UpdateDate,112)	UpdateDate
	, CASE 
		 WHEN day(c.UpdateDate) >= 1 and day(c.UpdateDate) <= 7  THEN 1
		 WHEN day(c.UpdateDate) >= 8 and day(c.UpdateDate) <= 14  THEN 2
		 WHEN day(c.UpdateDate) >= 15 and day(c.UpdateDate) <= 21  THEN 3
		 WHEN day(c.UpdateDate) >= 22 and day(c.UpdateDate) <= 28  THEN 4
		 WHEN day(c.UpdateDate) >= 29 and day(c.UpdateDate) <= 31  THEN 5
	   END WeekInt	
from SuzukiR4..pmStatusHistory c with (nolock, nowait) 
INNER JOIN SuzukiR4..pmHstITS a with (nolock, nowait)  ON -- penambahan 1 Apr 14
				a.CompanyCode = c.CompanyCode AND
				a.BranchCode = c.BranchCode AND
				a.InquiryNumber = c.InquiryNumber
where
	(case when c.CompanyCode='6015402' then '6015401' when c.CompanyCode='6051402' then '6051401' else c.CompanyCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and c.BranchCode  like case when @BranchCode='' then '%%' else @BranchCode end
	and c.LastProgress in (select LookUpValue from SuzukiR4..gnMstLookupDtl where CodeID = 'PSTS')	
	and (convert(varchar,c.UpdateDate,112) between @StartDate and @EndDate
	or (convert(varchar,c.UpdateDate,112) < @StartDate 
	and (c.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress<>'P'
												and convert(varchar,h.UpdateDate,112)<@StartDate)
	or  c.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress not in ('P','HP')
												and convert(varchar,h.UpdateDate,112)<@StartDate)
	or  c.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress not in ('P','HP','SPK')
												and convert(varchar,h.UpdateDate,112)<@StartDate))))	
union all
select   
h.CompanyCode
, h.BranchCode 
, 'SPK' LastProgress
, h.InquiryNumber
, convert(varchar,h.SPKDate,112)	SPKDate
, CASE 
	 WHEN day(h.SPKDate) >= 1 and day(h.SPKDate) <= 7  THEN 1
	 WHEN day(h.SPKDate) >= 8 and day(h.SPKDate) <= 14  THEN 2
	 WHEN day(h.SPKDate) >= 15 and day(h.SPKDate) <= 21  THEN 3
	 WHEN day(h.SPKDate) >= 22 and day(h.SPKDate) <= 28  THEN 4
	 WHEN day(h.SPKDate) >= 29 and day(h.SPKDate) <= 31  THEN 5
   END WeekInt	
from SuzukiR4..pmHstITS h
where 
(case when h.CompanyCode='6015402' then '6015401' when h.CompanyCode='6051402' then '6051401' else h.CompanyCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and h.BranchCode  like case when @BranchCode='' then '%%' else @BranchCode end
	and h.LastProgress in (select LookUpValue from SuzukiR4..gnMstLookupDtl where CodeID = 'PSTS')
 and convert(varchar,h.SPKDate,112) between @StartDate and @EndDate 
 and not exists (select top 1 1 from SuzukiR4..pmStatusHistory
                  where CompanyCode=h.CompanyCode
                    and BranchCode=h.BranchCode
                    and InquiryNumber=h.InquiryNumber)    
) #tThis order by CompanyCode, BranchCode

--select * into #tThis from(  --- penambahan 23 april 14
--select * from #tThis1
--union all
--select c.CompanyCode, c.BranchCode 	
--	, 'LOST' LastProgress
--	, c.InquiryNumber
--	, convert(varchar,c.UpdateDate,112)	UpdateDate
--	, CASE 
--		 WHEN day(c.UpdateDate) >= 1 and day(c.UpdateDate) <= 7  THEN 1
--		 WHEN day(c.UpdateDate) >= 8 and day(c.UpdateDate) <= 14  THEN 2
--		 WHEN day(c.UpdateDate) >= 15 and day(c.UpdateDate) <= 21  THEN 3
--		 WHEN day(c.UpdateDate) >= 22 and day(c.UpdateDate) <= 28  THEN 4
--		 WHEN day(c.UpdateDate) >= 29 and day(c.UpdateDate) <= 31  THEN 5
--	   END WeekInt	
--from SuzukiR4..pmStatusHistory c with (nolock, nowait) 
--inner join #tThis1 on #tThis1.CompanyCode = c.CompanyCode
--	and #tThis1.BranchCode = c.BranchCode
--	and #tThis1.InquiryNumber = c.InquiryNumber
--	and #tThis1.LastProgress = 'SPK' 
--where c.LastProgress = 'LOST'		
--and convert(varchar, #tThis1.UpdateDate, 112) between @StartDate and @EndDate
--) #tThis order by CompanyCode, BranchCode

select * into #tLast from(
select c.CompanyCode, c.BranchCode 
	, c.LastProgress
	, c.InquiryNumber		 
	, convert(varchar,c.UpdateDate,112)	UpdateDate
	, CASE 
		 WHEN day(c.UpdateDate) >= 1 and day(c.UpdateDate) <= 7  THEN 1
		 WHEN day(c.UpdateDate) >= 8 and day(c.UpdateDate) <= 14  THEN 2
		 WHEN day(c.UpdateDate) >= 15 and day(c.UpdateDate) <= 21  THEN 3
		 WHEN day(c.UpdateDate) >= 22 and day(c.UpdateDate) <= 28  THEN 4
		 WHEN day(c.UpdateDate) >= 29 and day(c.UpdateDate) <= 31  THEN 5
	   END WeekInt		   
from SuzukiR4..pmStatusHistory c with (nolock, nowait) 
INNER JOIN SuzukiR4..pmHstITS a WITH (NOLOCK, NOWAIT) ON -- penambahan 1 Apr 14
				a.CompanyCode = c.CompanyCode AND
				a.BranchCode = c.BranchCode AND
				a.InquiryNumber = c.InquiryNumber -- join Last Progress dihapus, 23 Apr 14
where
	(case when c.CompanyCode='6015402' then '6015401' when c.CompanyCode='6051402' then '6051401' else c.CompanyCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and c.BranchCode  like case when @BranchCode='' then '%%' else @BranchCode end
	and c.LastProgress in (select LookUpValue from SuzukiR4..gnMstLookupDtl where CodeID = 'PSTS')
	and (convert(varchar,c.UpdateDate,112) between @LastStartDate and @LastEndDate
	or (convert(varchar,c.UpdateDate,112) < @LastStartDate 
	and (c.LastProgress='P'   and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress<>'P'
												and convert(varchar,h.UpdateDate,112)<@LastStartDate)
	or  c.LastProgress='HP'  and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress not in ('P','HP')
												and convert(varchar,h.UpdateDate,112)<@LastStartDate)
	or  c.LastProgress='SPK' and not exists (select top 1 1 from SuzukiR4..pmStatusHistory h
											  where h.CompanyCode=c.CompanyCode
												and h.BranchCode=c.BranchCode
												and h.InquiryNumber=c.InquiryNumber
												and h.LastProgress not in ('P','HP','SPK')
												and convert(varchar,h.UpdateDate,112)<@LastStartDate))))	
union all
select   
h.CompanyCode
, h.BranchCode 
, 'SPK' LastProgress
, h.InquiryNumber
, convert(varchar,h.SPKDate,112) SPKDate
, CASE 
	 WHEN day(h.SPKDate) >= 1 and day(h.SPKDate) <= 7  THEN 1
	 WHEN day(h.SPKDate) >= 8 and day(h.SPKDate) <= 14  THEN 2
	 WHEN day(h.SPKDate) >= 15 and day(h.SPKDate) <= 21  THEN 3
	 WHEN day(h.SPKDate) >= 22 and day(h.SPKDate) <= 28  THEN 4
	 WHEN day(h.SPKDate) >= 29 and day(h.SPKDate) <= 31  THEN 5
   END WeekInt	
from SuzukiR4..pmHstITS h
where 
(case when h.CompanyCode='6015402' then '6015401' when h.CompanyCode='6051402' then '6051401' else h.CompanyCode end) like 
				case when @CompanyCode = ''       then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and h.BranchCode  like case when @BranchCode='' then '%%' else @BranchCode end
	and h.LastProgress in (select LookUpValue from SuzukiR4..gnMstLookupDtl where CodeID = 'PSTS')
and convert(varchar,h.SPKDate,112) between @LastStartDate and @LastEndDate 
 and not exists (select top 1 1 from SuzukiR4..pmStatusHistory
                  where CompanyCode=h.CompanyCode
                    and BranchCode=h.BranchCode
                    and InquiryNumber=h.InquiryNumber)     																					
) #tLast order by CompanyCode, BranchCode

select * into #t1 from(
select 
	a.CompanyCode
	, a.BranchCode 
	, isnull((select TOP 1 GroupModel 
						 from SuzukiR4..msMstGroupModel with (nolock,nowait)
						where ModelType = b.TipeKendaraan),'OTHERS') TipeKendaraan
	, isnull(b.Variant,'') Variant
	, convert(varchar, b.InquiryDate, 112) InquiryDate
	, a.UpdateDate
	, a.LastProgress	
	, a.WeekInt
	, a.InquiryNumber
from #tThis a with (nolock, nowait)												
inner join SuzukiR4..pmHstITS b with (nolock, nowait) on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.InquiryNumber = a.InquiryNumber  
where
	(case when @TipeKendaraan <> '' then b.TipeKendaraan else @TipeKendaraan end) = @TipeKendaraan 
	and (case when @Variant <> '' then b.Variant else @Variant end) = @Variant 
	--and (convert(varchar, b.InquiryDate, 112) < @StartDate or convert(varchar, a.UpdateDate, 112) between @StartDate and @EndDate)
	) #t1
order by CompanyCode, BranchCode, TipeKendaraan, Variant
	
select * into #t4 from(
select 	a.CompanyCode
	, a.BranchCode 
	, isnull((select TOP 1 GroupModel 
						 from SuzukiR4..msMstGroupModel with (nolock,nowait)
						where ModelType = b.TipeKendaraan),'OTHERS') TipeKendaraan
	, isnull(b.Variant,'') Variant
	, convert(varchar, b.InquiryDate, 112) InquiryDate
	, a.UpdateDate
	, a.LastProgress
	, a.WeekInt	 
	, a.InquiryNumber
from #tLast a with (nolock, nowait) 
left join SuzukiR4..pmHstITS b with (nolock, nowait) on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.InquiryNumber = a.InquiryNumber  
where
	(case when @TipeKendaraan <> '' then b.TipeKendaraan else @TipeKendaraan end) = @TipeKendaraan 
	and (case when @Variant <> '' then b.Variant else @Variant end) = @Variant 
	--and (convert(varchar, b.InquiryDate, 112) < @LastStartDate or convert(varchar, a.UpdateDate, 112) between @LastStartDate and @LastEndDate)
	) #t4 order by CompanyCode, BranchCode, TipeKendaraan, Variant

select * into #tVehicle from(
select distinct CompanyCode, BranchCode, TipeKendaraan
from #t1 
where (case when @GroupModel <> '' then TipeKendaraan else @GroupModel end) = @GroupModel
group by CompanyCode, BranchCode, TipeKendaraan
union all
select distinct CompanyCode, BranchCode, TipeKendaraan
from #t4 
where (case when @GroupModel <> '' then TipeKendaraan else @GroupModel end) = @GroupModel
group by CompanyCode, BranchCode, TipeKendaraan
) #tVehicle

select * into #t2 from(
select SeqNo, LookupValue LastProgress
from SuzukiR4..gnMstLookupDtl
where CodeID = 'PSTS'
) #t2 order by SeqNo
	
select * into #t3 from(
select 1 WeekInt union select 2 WeekInt union select 3 WeekInt union select 4 WeekInt union select 5 WeekInt
) #t3
	
select * into #tUnion1 from(
select CompanyCode, BranchCode, TipeKendaraan--, InquiryNumber
, LastProgress 

-- Outs
, (select count(*) from #t1
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and InquiryDate < @StartDate and UpdateDate < @StartDate ) SaldoAwal
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 1 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts1	
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 2 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts2		
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 3 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts3		
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 4 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts4
,(select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 5 and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) WeekOuts5
,(select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and InquiryDate < @StartDate and UpdateDate between @StartDate and @EndDate) TotalWeekOuts

-- New
,(select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress 
	and WeekInt = 1 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week1	
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress 
	and WeekInt = 2 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week2		
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress 
	and WeekInt = 3 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week3		
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress 
	and WeekInt = 4 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week4
,(select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress 
	and WeekInt = 5 and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) Week5
, (select count(*) from #t1 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress
	and UpdateDate between @StartDate and @EndDate 
	and (InquiryDate between @StartDate and @EndDate or InquiryDate > @EndDate)) TotalWeek
from #t1 a
group by CompanyCode, BranchCode, TipeKendaraan, LastProgress--, InquiryNumber
) #tUnion1

select * into #tUnion2 from(
select CompanyCode, BranchCode, TipeKendaraan
, LastProgress 

-- Outs
, (select count(*) from #t4
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and InquiryDate < @LastStartDate and UpdateDate < @LastStartDate) SaldoAwalLast
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 1 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts1Last	
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan  
	and LastProgress = a.LastProgress and WeekInt = 2 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts2Last		
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan  
	and LastProgress = a.LastProgress and WeekInt = 3 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts3Last		
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan  
	and LastProgress = a.LastProgress and WeekInt = 4 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts4Last
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 5 and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) WeekOuts5Last
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan  
	and LastProgress = a.LastProgress and InquiryDate < @LastStartDate and UpdateDate between @LastStartDate and @LastEndDate) TotalWeekOutsLast

-- New
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 1  
	and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week1Last	-- perhitungan InquiryDate between dihapus (23 apr 14)
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 2 
	and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week2Last		
, (select count(*) from #t4 
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan 
	and LastProgress = a.LastProgress and WeekInt = 3 
	and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week3Last		
, (select count(*) from #t4
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan  
	and LastProgress = a.LastProgress and WeekInt = 4 
	and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week4Last
, (select count(*) from #t4
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan  
	and LastProgress = a.LastProgress and WeekInt = 5 
	and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) Week5Last
,(select count(*) from #t4
	where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and TipeKendaraan = a.TipeKendaraan
	and LastProgress = a.LastProgress 
	and UpdateDate between @LastStartDate and @LastEndDate
	and (InquiryDate between @LastStartDate and @LastEndDate or InquiryDate > @LastEndDate)) TotalWeekLast
		
from #t4 a

group by CompanyCode, BranchCode, TipeKendaraan
, LastProgress 	
) #tUnion2
			
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
	
	, isnull(h.SaldoAwalLast, 0) SaldoAwalLast
	, isnull(h.WeekOuts1Last, 0) WeekOuts1Last
	, isnull(h.WeekOuts2Last, 0) WeekOuts2Last
	, isnull(h.WeekOuts3Last, 0) WeekOuts3Last
	, isnull(h.WeekOuts4Last, 0) WeekOuts4Last
	, isnull(h.WeekOuts5Last, 0) WeekOuts5Last
	, isnull(h.TotalWeekOutsLast, 0) TotalWeekOutsLast
	
	, isnull(h.Week1Last, 0) Week1Last
	, isnull(h.Week2Last, 0) Week2Last
	, isnull(h.Week3Last, 0) Week3Last
	, isnull(h.Week4Last, 0) Week4Last
	, isnull(h.Week5Last, 0) Week5Last
	, isnull(h.TotalWeekLast, 0) TotalWeekLast
	
	, isnull(h.TotalWeekOutsLast, 0) + isnull(h.TotalWeekLast, 0) TotalLast
	
	,cast((case when isnull(h.TotalWeekOutsLast, 0) + isnull(h.TotalWeekLast, 0) = 0 then 0 else (isnull(g.TotalWeekOuts, 0) + isnull(g.TotalWeek, 0))/ cast((isnull(h.TotalWeekOutsLast, 0) + isnull(h.TotalWeekLast, 0)) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent

from SuzukiR4..gnMstDealerMapping a with (nolock, nowait)
left join SuzukiR4..gnMstDealerOutletMapping b with (nolock, nowait) on a.DealerCode = b.DealerCode
inner join #tVehicle d with (nolock, nowait) on d.CompanyCode = a.DealerCode and d.BranchCode = b.OutletCode
left join #t2 e with (nolock, nowait) on e.SeqNo > 0 
left join #tUnion1 g with (nolock, nowait) on g.CompanyCode = a.DealerCode and g.BranchCode = b.OutletCode and g.TipeKendaraan =  d.TipeKendaraan and g.LastProgress = e.LastProgress
left join #tUnion2 h with (nolock, nowait) on h.CompanyCode = a.DealerCode and h.BranchCode = b.OutletCode and h.TipeKendaraan =  d.TipeKendaraan and h.LastProgress = e.LastProgress
where 
	a.IsActive = 1
	and (a.Area like Case when @Area <> '' then case when (@Area = 'JABODETABEK' or @Area = 'CABANG') then 'JABODETABEK' else @Area end else '%%' end
			or  a.Area like Case when @Area <> '' then case when (@Area='JABODETABEK' or @Area='CABANG') then 'CABANG' else @Area end else '%%' end)		  
	and (case when a.DealerCode='6015402' then '6015401' when a.DealerCode='6051402' then '6051401' else a.DealerCode end) like 
				case when @CompanyCode = '' then '%%'  when @CompanyCode ='6015402' then '6015401' when @CompanyCode ='6051402' then '6051401' else @CompanyCode end		   
	and b.OutletCode  like case when @BranchCode='' then '%%' else @BranchCode end
	) #tGabung
order by GroupNo, CompanyCode, BranchCode, TipeKendaraan, SeqNo
	
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
	,  SeqNo
	,  LastProgress		
	
	, SaldoAwalLast	
	, WeekOuts1Last	
	, WeekOuts2Last		
	, WeekOuts3Last		
	, WeekOuts4Last
	, WeekOuts5Last
	, TotalWeekOutsLast
	, Week1Last
	, Week2Last
	, Week3Last
	, Week4Last
	, Week5Last
	, TotalWeekLast
	, TotalLast
	
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
	
	, TotPercent

	from #tGabung
union
-- Group per Tipe Kendaraan
select 
	1 OrderNo3
	, 1 OrderNo
	, 0 OrderNo1
	, 0 OrderNo2
	
	,  GroupNo
	,  Area
	,  CompanyCode
	,  CompanyName
	,  '' BranchCode 
	,  '' BranchName
	,  TipeKendaraan
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
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
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

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
	, 1 OrderNo
	, 1 OrderNo1
	, 0 OrderNo2

	,  GroupNo
	,  Area
	,  '' CompanyCode
	,  'ZTOTAL' CompanyName
	,  '' BranchCode 
	,  '' BranchName
	,  TipeKendaraan
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
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
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

	from #tGabung
group by  GroupNo
	,   Area	
	,  TipeKendaraan
	,   SeqNo
	,   LastProgress				
union
-- Group per Area
select 
	1 OrderNo3
	, 1 OrderNo
	, 1 OrderNo1
	, 1 OrderNo2
	
	,  999999 GroupNo	
	,  'TOTAL' Area	
	,  'TOTAL' CompanyCode
	,  'ZTOTAL' CompanyName	
	,  '' BranchCode 
	,  '' BranchName		
	,  TipeKendaraan
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
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
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

	from #tGabung
group by TipeKendaraan
	,  SeqNo
	,  LastProgress
union
-- TOTAL
select 
	2 OrderNo3
	, 1 OrderNo
	, 1 OrderNo1
	, 2 OrderNo2
	
	,  0 GroupNo	
	,  'TOTAL' Area	
	,  '' CompanyCode
	,  'ZTOTAL' CompanyName	
	,  '' BranchCode 
	,  '' BranchName		
	,  'TOTAL' TipeKendaraan
	,  SeqNo
	,  LastProgress		
	
	, sum(SaldoAwalLast) SaldoAwalLast	
	, sum(WeekOuts1Last) WeekOuts1Last	
	, sum(WeekOuts2Last) WeekOuts2Last	
	, sum(WeekOuts3Last) WeekOuts3Last		
	, sum(WeekOuts4Last) WeekOuts4Last
	, sum(WeekOuts5Last) WeekOuts5Last
	, sum(TotalWeekOutsLast) TotalWeekOutsLast
	, sum(Week1Last) Week1Last
	, sum(Week2Last) Week2Last
	, sum(Week3Last) Week3Last
	, sum(Week4Last) Week4Last
	, sum(Week5Last) Week5Last
	, sum(TotalWeekLast) TotalWeekLast
	, sum(TotalLast) TotalLast
	
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
	,  sum(TotalLast) + sum(TotalWeek) Total
	
	,  cast((case when sum(TotalLast) = 0 then 0 else sum(Total)/ cast(sum(TotalLast) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent 

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
		 
		 ---- LAST MONTH	
		 , sum(SaldoAwalLast) SaldoAwalLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts1Last) END WeekOuts1Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts2Last) END WeekOuts2Last  
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts3Last) END WeekOuts3Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts4Last) END WeekOuts4Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts5Last) END WeekOuts5Last		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END TotalWeekOutsLast
		 , sum(Week1Last) Week1Last
		 , sum(Week2Last) Week2Last
		 , sum(Week3Last) Week3Last
		 , sum(Week4Last) Week4Last
		 , sum(Week5Last) Week5Last
		 , sum(TotalWeekLast) TotalWeekLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast) TotalLast	 
		 
		 ---- THIS MONTH	
		 , sum(SaldoAwal) SaldoAwal
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts1) END WeekOuts1
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts2) END WeekOuts2
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts3) END WeekOuts3
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts4) END WeekOuts4
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts5) END WeekOuts5		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END TotalWeekOuts
		 , sum(Week1) Week1
		 , sum(Week2) Week2 
		 , sum(Week3) Week3
		 , sum(Week4) Week4
		 , sum(Week5) Week5
		 , sum(TotalWeek) TotalWeek
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek) Total
		 
		 , cast((case when (CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) = 0 then 0 else 
		   (CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek))/ cast((CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent
 from #tFinal
 where OrderNo2 <> 1
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
	order by OrderNo3 Asc,GroupNo asc, CompanyName asc,Area asc,OrderNo1 asc,OrderNo asc,BranchCode asc,OrderNo2 Asc,SeqNo Asc

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
		 ,  SeqNo
		 ,  LastProgress		 		
		 
		 ---- LAST MONTH	
		 , sum(SaldoAwalLast) SaldoAwalLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts1Last) END WeekOuts1Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts2Last) END WeekOuts2Last  
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts3Last) END WeekOuts3Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts4Last) END WeekOuts4Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts5Last) END WeekOuts5Last		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END TotalWeekOutsLast
		 , sum(Week1Last) Week1Last
		 , sum(Week2Last) Week2Last
		 , sum(Week3Last) Week3Last
		 , sum(Week4Last) Week4Last
		 , sum(Week5Last) Week5Last
		 , sum(TotalWeekLast) TotalWeekLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast) TotalLast	 
		 
		 ---- THIS MONTH	
		 , sum(SaldoAwal) SaldoAwal
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts1) END WeekOuts1
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts2) END WeekOuts2
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts3) END WeekOuts3
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts4) END WeekOuts4
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts5) END WeekOuts5		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END TotalWeekOuts
		 , sum(Week1) Week1
		 , sum(Week2) Week2 
		 , sum(Week3) Week3
		 , sum(Week4) Week4
		 , sum(Week5) Week5
		 , sum(TotalWeek) TotalWeek
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek) Total
		 
		 , cast((case when (CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) = 0 then 0 else 
		   (CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek))/ cast((CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent
 from #tFinal
 where GroupNo <> 2 and OrderNo2 <> 2
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
		 ,  SeqNo
		 ,  LastProgress
	 order by  OrderNo3 Asc,  TipeKendaraan Asc, GroupNo asc,  CompanyName asc, OrderNo asc, OrderNo1 asc, BranchCode asc, OrderNo2 Asc, Area asc, SeqNo Asc 	 
end
else if (@Summary = 1)
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
		 ,  TipeKendaraan
		 ,  SeqNo
		 ,  LastProgress		 		
		 
		 ---- LAST MONTH	
		 , sum(SaldoAwalLast) SaldoAwalLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts1Last) END WeekOuts1Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts2Last) END WeekOuts2Last  
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts3Last) END WeekOuts3Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts4Last) END WeekOuts4Last
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(WeekOuts5Last) END WeekOuts5Last		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END TotalWeekOutsLast
		 , sum(Week1Last) Week1Last
		 , sum(Week2Last) Week2Last
		 , sum(Week3Last) Week3Last
		 , sum(Week4Last) Week4Last
		 , sum(Week5Last) Week5Last
		 , sum(TotalWeekLast) TotalWeekLast
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast) TotalLast	 
		 
		 ---- THIS MONTH	
		 , sum(SaldoAwal) SaldoAwal
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts1) END WeekOuts1
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts2) END WeekOuts2
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts3) END WeekOuts3
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts4) END WeekOuts4
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(WeekOuts5) END WeekOuts5		 
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END TotalWeekOuts
		 , sum(Week1) Week1
		 , sum(Week2) Week2 
		 , sum(Week3) Week3
		 , sum(Week4) Week4
		 , sum(Week5) Week5
		 , sum(TotalWeek) TotalWeek
		 , CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek) Total
		 
		 , cast((case when (CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) = 0 then 0 else 
		   (CASE WHEN  LastProgress = 'P' THEN 0 ELSE  sum(TotalWeekOuts) END + sum(TotalWeek))/ cast((CASE WHEN  LastProgress = 'P' THEN 0 ELSE sum(TotalWeekOutsLast) END + sum(TotalWeekLast)) as decimal(38,2)) end) * 100 as decimal(8,1)) TotPercent
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
		 ,  SeqNo
		 ,  LastProgress
	 order by  OrderNo3 Asc,  TipeKendaraan Asc, GroupNo asc,  CompanyName asc, OrderNo asc, OrderNo1 asc, BranchCode asc, OrderNo2 Asc, Area asc, SeqNo Asc 	 
end

drop table #tThis,  #t1, #t2, #t3, #t4, #tLast, #tUnion1, #tUnion2, #tVehicle, #tGabung, #tFinal

end
GO
