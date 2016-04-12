-- =============================================
-- Author:		da
-- Create date: 17 June 2013
-- Description:	Inquiry MarketShare
-- Last Update: 17 April 2014
-- =============================================
ALTER  PROCEDURE [dbo].[usprpt_InqMarketShare] 
--DECLARE
	@year numeric(4),
	@province varchar (15),
	@dealer varchar(15),
	@city varchar(15),	
	@isSuzukiClass bit,
	@show int,
	@prevMonth nvarchar(max),
	@currmonth nvarchar(max),
	@dynPrev nvarchar(max),
    @dynCurr nvarchar(max)
		
	--SET @year = 2013
	--SET @province = '1200'
	--SET @dealer ='6002401'
	--SET @city = '%'
	--SET @isSuzukiClass = 1	
	--SET @show = 10
	--SET @currMonth = 'currAug'
	--SET @prevMonth = 'currSep'
	--SET @dynPrev = 'prevJan+prevFeb+prevMar+prevApr+prevMay+prevJun+prevJul+prevAug+prevSep'
 --   SET @dynCurr = 'currJan+currFeb+currMar+currApr+currMay+currJun+currJul+currAug+currSep'


	--exec dbo.usprpt_InqMarketShare 2013,'2000','%',1,10,'currJul','currAug','prevJan+prevFeb+prevMar+prevApr+prevMay+prevJun+prevJul+prevAug','currJan+currFeb+currMar+currApr+currMay+currJun+currJul+currAug'
AS
BEGIN

DECLARE @YearTbl Table
(
year numeric(4)
)

DECLARE @LoopYear decimal(4)
SET @LoopYear = @year - 5

WHILE (@LoopYear <= @Year)
begin
insert into @yeartbl VALUES(@LoopYear)
set @LoopYear = @LoopYear + 1
end

SELECT TOP (@show) * INTO #tBrandExsist FROM
(
SELECT ReportSequence,BrandName FROM msMstReference
WHERE ReferenceType ='BRAND'
GROUP BY ReportSequence,BrandName
)#tBrandExsist

SELECT * INTO #t0 FROM
(
SELECT md.BrandCode,yearTbl.Year
,   sum(ms.Jan) AS Jan
,	sum(ms.Feb) AS Feb
,	sum(ms.Mar) AS Mar
,	sum(ms.Apr) AS Apr
,	sum(ms.May) AS May
,	sum(ms.Jun) AS Jun
,	sum(ms.Jul) AS Jul
,	sum(ms.Aug) AS Aug
,	sum(ms.Sep) AS Sep
,	sum(ms.Oct) AS Oct
,	sum(ms.Nov) AS Nov
,	sum(ms.Dec) AS Dec
,   sum(ms.Jan) + sum(ms.Feb) +	sum(ms.Mar) 
  + sum(ms.Apr) + sum(ms.May) + sum(ms.Jun)
  + sum(ms.Jul) + sum(ms.Aug) + sum(ms.Sep)
  + sum(ms.Oct) + sum(ms.Nov) + sum(ms.Dec)AS Total 
 FROM msMstModel md
  LEFT JOIN @YearTbl yearTbl
  ON  yearTbl.year BETWEEN @year - 5 AND @year
  LEFT JOIN msMstCity ct
on (CASE WHEN @province = '%' THEN @province ELSE ct.ProvinceCode END ) = @province  
LEFT JOIN msTrnMarketShare ms
ON ms.ModelType = md.ModelType AND ms.Variant = md.Variant and ms.Year = yearTbl.year AND ms.CityCode = ct.CityCode   
AND (CASE WHEN @city = '%' THEN @city ELSE ct.CityCode END ) = @city 
AND (CASE WHEN @dealer = '%' THEN @dealer ELSE ms.DealerCode END ) = @dealer
AND (CASE WHEN @isSuzukiClass = 0 THEN @isSuzukiClass ELSE md.isSuzukiClass END) = @isSuzukiClass 
GROUP BY md.BrandCode,yearTbl.Year
)#t0


SELECT * INTO #t1 FROM(
SELECT rf.ReportSequence, rf.BrandName
,isnull(#t0.Year,0) Year
,isnull(#t0.Jan,0)Jan
,isnull(#t0.Feb,0)Feb
,isnull(#t0.Mar,0)Mar
,isnull(#t0.Apr,0)Apr
,isnull(#t0.May,0)May
,isnull(#t0.Jun,0)Jun
,isnull(#t0.Jul,0)Jul
,isnull(#t0.Aug,0)Aug
,isnull(#t0.Sep,0)Sep
,isnull(#t0.Oct,0)Oct
,isnull(#t0.Nov,0)Nov
,isnull(#t0.Dec,0)Dec
,isnull(#t0.Total,0)Total
from msMstReference rf
JOIN #t0
ON rf.BrandName = #t0.BrandCode
WHERE rf.ReferenceType = 'BRAND' )#t1

SELECT * FROM #t1
ORDER BY #t1.ReportSequence, #t1.Year


-- Total OTHERS
SELECT * INTO #tNExists FROM(
SELECT * FROM #t0 ex
WHERE NOT EXISTS
    (SELECT * FROM #tBrandExsist bex
     WHERE bex.BrandName = 
            ex.BrandCode))#tNExists

SELECT Year
,   sum(Jan) AS Jan
,	sum(Feb) AS Feb
,	sum(Mar) AS Mar
,	sum(Apr) AS Apr
,	sum(May) AS May
,	sum(Jun) AS Jun
,	sum(Jul) AS Jul
,	sum(Aug) AS Aug
,	sum(Sep) AS Sep
,	sum(Oct) AS Oct
,	sum(Nov) AS Nov
,	sum(Dec) AS Dec
,   sum(Jan) + sum(Feb) + sum(Mar) 
  + sum(Apr) + sum(May) + sum(Jun)
  + sum(Jul) + sum(Aug) + sum(Sep)
  + sum(Oct) + sum(Nov) + sum(Dec) AS Total FROM #tNExists 
GROUP BY Year

--GRAND TOTAL
SELECT #t0.Year,sum(Jan) AS Jan
,	sum(Feb) AS Feb
,	sum(Mar) AS Mar
,	sum(Apr) AS Apr
,	sum(May) AS May
,	sum(Jun) AS Jun
,	sum(Jul) AS Jul
,	sum(Aug) AS Aug
,	sum(Sep) AS Sep
,	sum(Oct) AS Oct
,	sum(Nov) AS Nov
,	sum(Dec) AS Dec
,   sum(Jan) + sum(Feb) + sum(Mar) 
  + sum(Apr) + sum(May) + sum(Jun)
  + sum(Jul) + sum(Aug) + sum(Sep)
  + sum(Oct) + sum(Nov) + sum(Dec)AS Total FROM #t0 
  GROUP BY #t0.Year

-- Not Exsist

SELECT rf.ReportSequence, rf.BrandName
,isnull(#tNExists.Year,0)Year
,isnull(#tNExists.Jan,0)Jan
,isnull(#tNExists.Feb,0)Feb
,isnull(#tNExists.Mar,0)Mar
,isnull(#tNExists.Apr,0)Apr
,isnull(#tNExists.May,0)May
,isnull(#tNExists.Jun,0)Jun
,isnull(#tNExists.Jul,0)Jul
,isnull(#tNExists.Aug,0)Aug
,isnull(#tNExists.Sep,0)Sep
,isnull(#tNExists.Oct,0)Oct
,isnull(#tNExists.Nov,0)Nov
,isnull(#tNExists.Dec,0)Dec
,isnull(#tNExists.Total,0)Total
from msMstReference rf
LEFT JOIN #tNExists
ON rf.BrandName = #tNExists.BrandCode
WHERE rf.ReferenceType = 'BRAND' and #tNExists.Year <> 0
ORDER BY rf.ReportSequence, #tNExists.Year

-- MarketShare

SELECT * INTO #tCurrent FROM(
SELECT 
 #t1.ReportSequence
,#t1.BrandName
,#t1.Year
,#t1.Jan
,#t1.Feb
,#t1.Mar
,#t1.Apr
,#t1.May
,#t1.Jun
,#t1.Jul
,#t1.Aug
,#t1.Sep
,#t1.Oct
,#t1.Nov
,#t1.Dec
,#t1.Total FROM #t1 
WHERE #t1.Year = @year
)#tCurrent

SELECT * INTO #tBefore FROM(
SELECT 
 #t1.ReportSequence
,#t1.BrandName
,#t1.Year
,#t1.Jan
,#t1.Feb
,#t1.Mar
,#t1.Apr
,#t1.May
,#t1.Jun
,#t1.Jul
,#t1.Aug
,#t1.Sep
,#t1.Oct
,#t1.Nov
,#t1.Dec
,#t1.Total FROM #t1 
WHERE #t1.Year = (@year - 1) 
)#tBefore

SELECT * INTO #t2 FROM
(SELECT DISTINCT 
 #tBefore.ReportSequence
,#tBefore.BrandName
,#tBefore.Jan AS prevJan
,#tBefore.Feb AS prevFeb
,#tBefore.Mar AS prevMar
,#tBefore.Apr AS prevApr
,#tBefore.May AS prevMay
,#tBefore.Jun AS prevJun
,#tBefore.Jul AS prevJul
,#tBefore.Aug AS prevAug
,#tBefore.Sep AS prevSep
,#tBefore.Oct AS prevOct
,#tBefore.Nov AS prevNov
,#tBefore.Dec AS prevDec
,#tBefore.Total AS prevTotal
,#tCurrent.Jan AS currJan
,#tCurrent.Feb AS currFeb
,#tCurrent.Mar AS currMar
,#tCurrent.Apr AS currApr
,#tCurrent.May AS currMay
,#tCurrent.Jun AS currJun
,#tCurrent.Jul AS currJul
,#tCurrent.Aug AS currAug
,#tCurrent.Sep AS currSep
,#tCurrent.Oct AS currOct
,#tCurrent.Nov AS currNov
,#tCurrent.Dec AS currDec
,#tCurrent.Total AS curTotal
FROM  #tbefore
RIGHT JOIN #tcurrent
ON #tBefore.BrandName = #tCurrent.BrandName
)#t2

DECLARE 
		@PrevGT NUMERIC ,
		@CurrGT NUMERIC

SET @PrevGT = (Select isnull(SUM(Total),0) GrandTotal from #t1
WHERE Year = (@year-1))
SET @CurrGT = (Select isnull(SUM(Total),0) GrandTotal from #t1
WHERE Year = @year)

DECLARE @prevMonthTotalQ varchar(max) SET @prevMonthTotalQ = 'SELECT SUM('+@prevMonth+') AS prevMonthTotal FROM #t2'
DECLARE @currMonthTotalQ varchar(max) SET @currMonthTotalQ = 'SELECT SUM('+@CurrMonth+') AS currMonthTotal FROM #t2'

DECLARE @prevMYQ nvarchar(max) SET @prevMYQ = '(SELECT SUM ('+@dynPrev+') AS prevMY FROM #t2)'
DECLARE @currMYQ nvarchar(max) SET @currMYQ = '(SELECT SUM ('+@dynCurr+') AS currMY FROM #t2)'

DECLARE @prevMonthTotalTbl TABLE
(
prevMonthTotal NUMERIC
)

DECLARE @currMonthTotalTbl TABLE
(
currMonthTotal NUMERIC
)

DECLARE @prevTotalMonthYearTbl TABLE
(
prevTotalMonthYear NUMERIC
)

DECLARE @currTotalMonthYearTbl TABLE
(
currTotalMonthYear NUMERIC
)

INSERT INTO @prevMonthTotalTbl EXEC(@prevMonthTotalQ)
DECLARE @prevMonthTotal NUMERIC SET @prevMonthTotal = (SELECT * from @prevMonthTotalTbl)

INSERT INTO @currMonthTotalTbl EXEC(@currMonthTotalQ)
DECLARE @currMonthTotal NUMERIC SET @currMonthTotal = (SELECT * from @CurrMonthTotalTbl)

INSERT INTO @prevTotalMonthYearTbl EXEC(@prevMYQ)
DECLARE @prevTotalMonthYear NUMERIC SET @prevTotalMonthYear = (SELECT * FROM @prevTotalMonthYearTbl)

INSERT INTO @currTotalMonthYearTbl EXEC(@currMYQ)
DECLARE @currTotalMonthYear NUMERIC SET @currTotalMonthYear = (SELECT * FROM @currTotalMonthYearTbl)

DECLARE @MS nvarchar(MAX) SET @MS = 
'SELECT 
  ReportSequence
, BrandName
, prevTotal
, (CASE WHEN '+CAST(@PrevGT AS NVARCHAR(MAX))+' = 0 THEN 0 ELSE (prevtotal/CAST('+CAST(@PrevGT AS NVARCHAR(MAX))+' AS NUMERIC))END) * 100 prevTotalPct
, '+@prevMonth+' prevMonth
, (CASE WHEN '+CAST(@prevMonthTotal AS NVARCHAR(MAX))+' = 0 THEN 0 ELSE ('+@prevMonth+'/CAST('+CAST(@prevMonthTotal AS NVARCHAR(MAX))+' AS NUMERIC)) END ) * 100 prevMonthPct
, '+@currMonth+' curMonth
, (CASE WHEN '+CAST(@CurrMonthTotal AS NVARCHAR(MAX))+' = 0 THEN 0 ELSE ('+@currMonth+'/CAST('+CAST(@CurrMonthTotal AS NVARCHAR(MAX))+' AS NUMERIC))END) * 100 currMonthPct 
, (CASE WHEN '+CAST(@CurrMonthTotal AS NVARCHAR(MAX))+' = 0 OR '+CAST(@prevMonthTotal AS NVARCHAR(MAX))+' = 0 THEN 0 ELSE ('+@currMonth+'/CAST('+CAST(@CurrMonthTotal AS NVARCHAR(MAX))+' AS NUMERIC)) - ('+@prevMonth+'/CAST('+CAST(@prevMonthTotal AS NVARCHAR(MAX))+' AS NUMERIC))END) * 100 Pct1
, (CASE WHEN '+@prevMonth+' = 0 THEN 0 ELSE (CAST(('+@currMonth+')AS NUMERIC)/CAST(('+@prevMonth+')AS NUMERIC) - 1)END) * 100 Pct2
, '+@dynPrev+' prevTotalMonthYear
, (CASE WHEN '+ CAST(@prevTotalMonthYear AS NVARCHAR(MAX))+' = 0 THEN 0 ELSE CAST(('+@dynPrev+')AS NUMERIC) / '+ CAST(@prevTotalMonthYear AS NVARCHAR(MAX))+' END) * 100 prevTotalMonthYearPct
, '+@dynCurr+' curTotalMonthYear
, (CASE WHEN '+ CAST(@currTotalMonthYear AS NVARCHAR(MAX))+' = 0 THEN 0 ELSE CAST(('+@dynCurr+')AS NUMERIC) / CAST('+ CAST(@currTotalMonthYear AS NVARCHAR(MAX))+' AS NUMERIC)END) * 100 curTotalMonthYearPct
, (CASE WHEN '+ CAST(@currTotalMonthYear AS NVARCHAR(MAX))+' = 0 OR '+ CAST(@prevTotalMonthYear AS NVARCHAR(MAX))+' = 0 THEN 0 ELSE (CAST(('+@dynCurr+')AS NUMERIC) /'+ CAST(@currTotalMonthYear AS NVARCHAR(MAX))+') - (CAST(('+@dynPrev+')AS NUMERIC) / '+ CAST(@prevTotalMonthYear AS NVARCHAR(MAX))+')END) * 100 Pct3
, (CASE WHEN '+@dynPrev+' = 0 THEN 0 ELSE (CAST(('+@dynCurr+')AS NUMERIC)/CAST(('+@dynPrev+')AS NUMERIC)) - 1 END) * 100 Pct4
FROM #t2'

--EXEC(@MS)
--PRINT (@MS)

DECLARE @MSTbl TABLE
(
	ReportSequence INT,
	BrandName VARCHAR(MAX),
	prevTotal NUMERIC,
	prevTotalPct DECIMAL(18,5),
	prevMonth NUMERIC,
	prevMonthPct DECIMAL(18,5),
	currMonth NUMERIC,
	currMonthPct DECIMAL(18,5),
	Pct1 DECIMAL(5,1),
	Pct2 DECIMAL(5,1),
	prevTotalMonthYear NUMERIC,
	prevTotalMonthYearPct DECIMAL(18,5),
	currTotalMonthYear NUMERIC,
	currTotalMonthYearPct DECIMAL(18,5),
	Pct3 DECIMAL(5,1),
	Pct4 DECIMAL(5,1)
)

INSERT INTO @MSTbl EXEC(@MS)

-- MS Show
SELECT  top (@show) * FROM @MSTbl

-- MS Not Exist
SELECT * INTO #tMSNExists FROM(
SELECT * FROM @MSTbl ex
WHERE NOT EXISTS
    (SELECT * FROM #tBrandExsist bex
     WHERE bex.BrandName = 
            ex.BrandName))#tMSNExists

-- MS TOTAL OTHERS
SELECT * INTO #tMSOthers FROM(
SELECT
    0 as No
,   'OTHERS' AS BrandName
,   SUM(#tMSNExists.prevTotal) prevTotal 
,	SUM(#tMSNExists.prevTotalPct) prevTotalPct
,	SUM(#tMSNExists.prevMonth) prevMonth 
,	SUM(#tMSNExists.prevMonthPct) prevMonthPct 
,	SUM(#tMSNExists.currMonth) currMonth 
,	SUM(#tMSNExists.currMonthPct) currMonthPct 
,	SUM(#tMSNExists.Pct1) Pct1 
,	SUM(#tMSNExists.prevTotalMonthYear) prevTotalMonthYear
,	SUM(#tMSNExists.prevTotalMonthYearPct) prevTotalMonthYearPct 
,	SUM(#tMSNExists.currTotalMonthYear) currTotalMonthYear 
,	SUM(#tMSNExists.currTotalMonthYearPct) currTotalMonthYearPct 
,	SUM(#tMSNExists.Pct3) Pct3
FROM #tMSNExists)#tMSOthers

SELECT 
    #tMSOthers.No
,   #tMSOthers.BrandName
,   #tMSOthers.prevTotal
,	#tMSOthers.prevTotalPct
,	#tMSOthers.prevMonth
,	#tMSOthers.prevMonthPct 
,	#tMSOthers.currMonth
,	#tMSOthers.currMonthPct
,	cast(#tMSOthers.Pct1 as decimal(5,1)) Pct1
,	cast((CASE WHEN #tMSOthers.prevMonth = 0 THEN 0 ELSE (#tMSOthers.currMonth / #tMSOthers.prevMonth ) - 1 END) *100 as decimal(5,1)) Pct2
,	#tMSOthers.prevTotalMonthYear
,	#tMSOthers.prevTotalMonthYearPct 
,	#tMSOthers.currTotalMonthYear
,	#tMSOthers.currTotalMonthYearPct
,	CAST(#tMSOthers.Pct3 AS DECIMAL(5,1)) Pct3
,	CAST((CASE WHEN #tMSOthers.prevTotalMonthYear = 0 THEN 0 ELSE (#tMSOthers.currTotalMonthYear / #tMSOthers.prevTotalMonthYear) - 1 END)*100 AS DECIMAL(5,1)) Pct4
 FROM #tMSOthers

-- MS GRAND TOTAL
SELECT * INTO #tMSGrandTotal FROM(
SELECT
    0 as No
,   'GRAND TOTAL' AS BrandName
,   SUM(prevTotal) prevTotal 
,	SUM(prevTotalPct) prevTotalPct
,	SUM(prevMonth) prevMonth 
,	SUM(prevMonthPct) prevMonthPct 
,	SUM(currMonth) currMonth 
,	SUM(currMonthPct) currMonthPct 
,	SUM(Pct1) Pct1 
,	SUM(prevTotalMonthYear) prevTotalMonthYear
,	SUM(prevTotalMonthYearPct) prevTotalMonthYearPct 
,	SUM(currTotalMonthYear) currTotalMonthYear 
,	SUM(currTotalMonthYearPct) currTotalMonthYearPct 
,	SUM(Pct3) Pct3
FROM @MSTbl)#tMSGrandTotal

SELECT 
    #tMSGrandTotal.No
,   #tMSGrandTotal.BrandName
,   #tMSGrandTotal.prevTotal
,	#tMSGrandTotal.prevTotalPct
,	#tMSGrandTotal.prevMonth
,	#tMSGrandTotal.prevMonthPct 
,	#tMSGrandTotal.currMonth
,	#tMSGrandTotal.currMonthPct
,	cast(#tMSGrandTotal.Pct1 as decimal(5,1)) Pct1
,	cast((CASE WHEN #tMSGrandTotal.prevMonth = 0 THEN 0 ELSE (#tMSGrandTotal.currMonth / #tMSGrandTotal.prevMonth ) - 1 END)*100 as decimal(5,1)) Pct2
,	#tMSGrandTotal.prevTotalMonthYear
,	#tMSGrandTotal.prevTotalMonthYearPct 
,	#tMSGrandTotal.currTotalMonthYear
,	#tMSGrandTotal.currTotalMonthYearPct
,	cast(#tMSGrandTotal.Pct3 as decimal(5,1)) Pct3
,	cast((CASE WHEN #tMSGrandTotal.prevTotalMonthYear = 0 THEN 0 ELSE (#tMSGrandTotal.currTotalMonthYear / #tMSGrandTotal.prevTotalMonthYear) - 1 END)*100 as decimal(5,1)) Pct4
 FROM #tMSGrandTotal

-- MS Not Exsist
SELECT * FROM #tMSNExists
  
DROP TABLE #t0, #t1, #tBrandExsist, #tNExists, #tCurrent, #tBefore, #t2, #tMSOthers, #tMSGrandTotal, #tMSNExists
END
