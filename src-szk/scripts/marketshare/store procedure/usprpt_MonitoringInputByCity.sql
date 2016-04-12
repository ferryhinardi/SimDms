-- =============================================  
-- Author:  David Leonardo  
-- Create date: 27 Jan 2014  
-- Description: Monitoring Input  

--Modified by : Mulia, 17 Nov 2014
--Added Group By City
-- =============================================  
ALTER PROCEDURE [dbo].[usprpt_MonitoringInputByCity] --'2014'
--DECLARE  
 @year varchar(4)  
  
--exec usprpt_MonitoringInputByCity 2014  
--SET @year = 2014  
  
AS  
BEGIN  
  
SELECT * INTO #t1 FROM  
(select ms.DealerCode  
, ct.RegionCode  
, ar.AreaCode  
, ar.AreaName  
, (CASE WHEN ms.DealerCode = '6093401' THEN 'SURABAYA 1' ELSE (CASE WHEN ms.DealerCode = '6088401' THEN 'MEDAN 1' ELSE rg.RegionName END) END) as RegionName  
, ct.CityCode  
, ct.CityDesc  
, jan  
, feb  
, mar  
, apr  
, may  
, jun  
, jul  
, aug  
, sep  
, oct  
, nov  
, dec  
FROM msMstRegion rg  
join msMstCity ct  
on ct.RegionCode = rg.RegionCode  
JOIN msMstArea ar  
on ct.AreaCode = ar.AreaCode  
LEFT JOIN msTrnMarketShare ms  
on ct.CityCode = ms.CityCode and ms.Year = @year 
where ct.status = 1
) #t1   


SELECT * into #Temp FROM (  
select Distinct dl.DealerCode, ct.AreaCode, ct.CityCode,  
(CASE WHEN dl.DealerCode = '6093401' THEN 'SURABAYA 1' ELSE (CASE WHEN dl.DealerCode = '6088401' THEN 'MEDAN 1' ELSE rg.RegionName END) END) as RegionName  
,DealerAbbreviation from msMstDealerCity dl  
JOIN msMstCity ct  
on dl.CityCode = ct.CityCode and ct.Status = 1
join msMstRegion rg  
ON ct.RegionCode = rg.RegionCode  
JOIN gnMstDealerMapping  
ON dl.DealerCode = gnMstDealerMapping.DealerCode  
) #Temp  
  
SELECT * INTO #t2 FROM(  
select   
    T1.RegionName, T1.CityCode,  
    stuff(  
        (  
            select ', ' + T2.DealerAbbreviation  
            from #Temp as T2  
            where T2.RegionName = T1.RegionName and T2.CityCode = T1.CityCode   
            for xml path(''), type  
        ).value('data(.)', 'nvarchar(max)')  
    , 1, 2, '') as LISTOFPARTS  
from #Temp as T1   
group by T1.RegionName, T1.CityCode) #t2  
  
  
select * into #tlist FROM(select regioncode,areacode,areaname, #t1.regionname,#t1.CityCode, #t1.CityDesc,
(case when LISTOFPARTS is null then (select DealerAbbreviation from gnMstDealerMapping where DealerCode = max(#t1.DealerCode)) 
else LISTOFPARTS end) as LISTOFPARTS  
,sum(jan) jan  
,sum(feb) feb  
,sum(mar) mar  
,sum(apr) apr  
,sum(may) may  
,sum(jun) jun  
,sum(jul) jul  
,sum(aug) aug  
,sum(sep) sep  
,sum(oct) oct  
,sum(nov) nov  
,sum(dec) dec   
FROM #t1  
LEFT JOIN #t2  
ON #t1.RegionName = #t2.RegionName and #t1.CityCode = #t2.CityCode  
where (case when LISTOFPARTS is null then #t1.DealerCode else LISTOFPARTS end) is not null
Group BY regioncode,areacode,areaname,#t1.CityCode, #t1.CityDesc,#t1.regionname,LISTOFPARTS  
) #tlist
--order by RegionCode, AreaCode, CityCode  
  
SELECT * INTO #tdate FROM(  
select * from (SELECT DISTINCT d.areacode, d.areaname,e.regioncode  
,(CASE WHEN a.DealerCode = '6093401' THEN 'SURABAYA 1' ELSE (CASE WHEN a.DealerCode = '6088401' THEN 'MEDAN 1' ELSE e.RegionName END) END) as RegionName  
,c.CityCode, c.CityDesc  
, b.lastupdatedate, DATENAME(month,DATEADD(month, b.Month, -1 )) shortmonth FROM msmstdealercity a  
JOIN msHstMarketShare b  
on a.dealercode = b.dealercode AND a.citycode = b.citycode  and b.Year = @year   
JOIN msmstcity c  
on a.citycode = c.citycode and c.Status = 1   
JOIN msmstarea d  
on c.areacode= d.areacode  
join msmstregion e  
on c.regioncode = e.regioncode  
) as source  
PIVOT   
(max(lastupdatedate) FOR shortmonth IN   
([January],[February],[March],[April],[May],[June],[July],[August],[September],[October],[November],[December])) as pvt) #tdate  


select a.regioncode,a.areacode,a.regionname,a.LISTOFPARTS  
, ISNULL(a.jan,0) jan  
, b.january   
, ISNULL(a.feb,0) feb  
, b.february   
, ISNULL(a.mar,0) mar  
, b.march   
, ISNULL(a.apr,0) apr  
, b.april   
, ISNULL(a.may,0) may  
, b.may   
, ISNULL(a.jun,0) jun  
, b.june   
, ISNULL(a.jul,0) jul  
, b.july   
, ISNULL(a.aug,0) aug  
, b.august   
, ISNULL(a.sep,0) sep  
, b.september  
, ISNULL(a.oct,0) oct  
, b.october  
, ISNULL(a.nov,0) nov  
, b.november  
, ISNULL(a.dec,0) dec  
, b.december, a.CityCode, a.CityDesc  
FROM #tlist a  
left join #tdate b  
ON a.regionname = b.regionname and a.CityCode = b.CityCode  
order by  a.AreaCode, a.RegionCode , a.citycode
  
drop TABLE #t1  
drop TABLE #t2  
drop TABLE #Temp  
DROP TABLE #tlist  
drop TABLE #tdate  
END  
  
  
  
  
  
  