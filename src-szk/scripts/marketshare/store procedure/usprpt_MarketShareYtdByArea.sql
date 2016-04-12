-- =============================================
-- Author:		Yo
-- Create date: 29 JULY 2013
-- Description:	Inquiry Format 3, Comparisson By Class
-- Last Update: 14 April 2014
-- Notes      : @Loop diganti dengan @firstmonth 
-- =============================================
ALTER  PROCEDURE [dbo].[usprpt_MarketShareYtdByArea] 
@DealerCode varchar(15),
@Year decimal(4),
@FirstMonth int,
@Month int,
@AreaCode varchar(15)


--declare @DealerCode varchar(15)
--declare @Year decimal(4)
--declare @FirstMonth int
--declare @Month int
--declare @AreaCode varchar(15)

--set @DealerCode = ''
--set @Year = 2013
--set @FirstMonth = 1
--set @Month = 12
--set @AreaCode = ''

as
BEGIN

if (@Year = 0 and @Month = 0) return

declare @brandTempTot1 VARCHAR(max)
declare @brandTot VARCHAR(max)
declare @brandTot1 VARCHAR(max)
declare @selectBrand VARCHAR(max)
declare @brandPivot VARCHAR(max)
declare @Show as int
declare @pctBrand VARCHAR(max)

declare @Query as varchar(max)

select @brandPivot = STUFF((
   select top 9 ',' + quotename(ReferenceCode) 
    from msMstReference	
    where ReferenceType = 'BRAND'
    order by ReportSequence
    for xml path('')), 1, 1, '') 

select @selectBrand = STUFF((
   select top 9 ',' + 'isnull(' + quotename(ReferenceCode) + ', 0) as ' + quotename(ReferenceCode)
    from msMstReference	
    where ReferenceType = 'BRAND'
    order by ReportSequence
    for xml path('')), 1, 1, '') 

select @brandTot = STUFF((
   select top 9 ',' + 'isnull(sum(' + quotename(ReferenceCode) + '), 0) as' + quotename(ReferenceCode)
    from msMstReference	
    where ReferenceType = 'BRAND'
    order by ReportSequence
    for xml path('')), 1, 1, '') 

select @brandTempTot1 = STUFF((
   select top 9 '+' + quotename(ReferenceCode)
    from msMstReference	
    where ReferenceType = 'BRAND'
    order by ReportSequence
    for xml path('')), 1, 1, '') 

declare @sumMonthTemp as varchar(max)
set @sumMonthTemp = ''
declare @loop as integer
set @Loop = @FirstMonth

while (@Loop <= @Month)
begin
	set @sumMonthTemp = @sumMonthTemp + (select substring(datename(mm, dateadd(month, @Loop, 0)-1), 1, 3)) + '+'
	set @Loop = @Loop + 1
end

declare @sumMonth as varchar(max)
set @sumMonth = 'isnull(sum(' + (select substring(@sumMonthTemp,1,(select len(@sumMonthTemp) - 1))) + '), 0) '
    
set @selectBrand = @selectBrand + ',isnull([OTHERS], 0) [OTHERS]'
set @brandTot = @brandTot + ',isnull(sum([OTHERS]),0) [OTHERS]'       
set @brandPivot = @brandPivot + ',[OTHERS]'

set @pctBrand =  STUFF((
   select top 9 ',' + 'CASE WHEN TOTAL = 0 THEN 0 ELSE cast(((isnull(' + quotename(ReferenceCode) + ', 0) / CAST(TOTAL AS DECIMAL(38,2)))*100) as decimal(5,1)) END as ' + quotename(ReferenceCode)
    from msMstReference	
    where ReferenceType = 'BRAND'
    order by ReportSequence
    for xml path('')), 1, 1, '') 
set @pctBrand = @pctBrand + ', CASE WHEN TOTAL = 0 THEN 0 ELSE cast(((isnull([OTHERS], 0) / CAST(TOTAL AS DECIMAL(38,2)))*100) as decimal(5,1)) END AS [OTHERS]'

set @Query = 'select * into #tempUnion from(
select 
area.AreaCode, area.AreaName, rg.RegionCode, rg.RegionName,
'+@sumMonth+' Val, 
(case when md.BrandCode in (select top 9 ReferenceCode from msMstReference	
    where ReferenceType = '+'''BRAND'''+'
    order by ReportSequence) then md.BrandCode else '+'''OTHERS'''+' end) BrandCode
from msMstRegion rg
inner join msMstDealer dealer on dealer.DealerCode != '+''''''+'
left join msMstDealerCity dlrCity on dlrCity.DealerCode = dealer.DealerCode
inner join msMstCity ct on rg.RegionCode = ct.RegionCode and dlrCity.CityCode = ct.CityCode
left join msTrnMarketShare ms on ct.CityCode = ms.CityCode and ms.DealerCode = dealer.DealerCode
inner join msMstModel md on md.ModelType = ms.ModelType AND md.Variant = ms.Variant
left join msMstArea area on area.AreaCode = ct.AreaCode
where Year = '+convert(varchar,@Year)+'
and (case when '''+@DealerCode+''' = '+''''''+' then '''+@DealerCode+''' else dealer.DealerCode end) = '''+@DealerCode+'''
and (case when '''+@AreaCode+''' = '+''''''+' then '''+@AreaCode+''' else area.AreaCode end) = '''+@AreaCode+'''
group by rg.RegionCode,area.AreaCode, area.AreaName,rg.RegionName,Year, md.BrandCode,ms.DealerCode, ms.ModelType, ms.Variant, ms.CityCode
) #tempUnion order by AreaCode

select AreaCode, AreaName, RegionCode, RegionName, ' + @selectBrand + ' into #tempPivot from(
select * from #tempUnion	
pivot (sum(Val) for BrandCode in ('+@brandPivot+')) pvt1
) #tempPivot

select * into #tempTotal from(
select #tempPivot.RegionCode, #tempPivot.RegionName,sum('+ @brandTempTot1 + '+[OTHERS]'+') Val
from #tempPivot
group by #tempPivot.RegionCode,#tempPivot.RegionName
) #tempTotal order by RegionCode

select * into #tempTotal1 from(
select #tempPivot.AreaCode, #tempPivot.AreaName, sum('+ @brandTempTot1 + '+[OTHERS]'+') TOTAL
from #tempPivot
group by #tempPivot.AreaCode, #tempPivot.AreaName
) #tempTotal1

select * into #Pivot from(
select #tempPivot.*, total.Val TOTAL from #tempPivot
left join #tempTotal total on total.RegionCode = #tempPivot.RegionCode 
union
select #tempPivot.AreaCode, #tempPivot.AreaName, '+'''TOTAL '''+'+ #tempPivot.AreaName RegionCode, '+''''''+' RegionName, '+ @brandTot +', total.TOTAL
from #tempPivot
left join #tempTotal1 total on total.AreaCode = #tempPivot.AreaCode 
group by #tempPivot.AreaCode, #tempPivot.AreaName, total.TOTAL
union 
select '+'''GRAND TOTAL'''+' AreaCode, '+'''GRAND TOTAL'''+' AreaName, '+'''GRAND TOTAL'''+' RegionCode, '+''''''+' RegionName, '+ @brandTot +', sum('+@brandTempTot1 + '+[OTHERS]'+') TOTAL
from #tempPivot
) #Pivot order by AreaCode

select * from #Pivot

select 1 SeqNo, '+'''B'''+' RegionName, '+ @brandTot +', isnull(sum('+@brandTempTot1 + '+[OTHERS]'+'), 0) TOTAL
from #tempPivot
where RegionCode in (001, 003, 004, 005)
union
select 2 SeqNo, '+'''F'''+' RegionName, '+ @brandTot +', isnull(sum('+@brandTempTot1 + '+[OTHERS]'+'), 0) TOTAL
from #tempPivot
where RegionCode in (002, 007, 009)
union
select 3 SeqNo, '+'''T'''+' RegionName, '+ @brandTot +', isnull(sum('+@brandTempTot1 + '+[OTHERS]'+'), 0) TOTAL
from #tempPivot
where RegionCode in (008)
union
select 4 SeqNo, '+'''A'''+' RegionName, '+ @brandTot +', isnull(sum('+@brandTempTot1 + '+[OTHERS]'+'), 0) TOTAL
from #tempPivot
where RegionCode in (006, 010)
union
select 5 SeqNo, '+'''TOTAL'''+' RegionName, '+ @brandTot +', isnull(sum('+@brandTempTot1 + '+[OTHERS]'+'), 0) TOTAL
from #tempPivot
where RegionCode in (001, 003, 004, 005, 002, 007, 009, 008, 006, 010)

select AreaCode, AreaName, #Pivot.RegionCode, #Pivot.RegionName, ' + @pctBrand + '
, CAST(CASE WHEN TOTAL = 0 THEN 0 ELSE (TOTAL/ CAST(TOTAL AS DECIMAL(38,2)))*100 END AS DECIMAL(5,1)) TOTAL from #Pivot'

print(@Query)
exec(@Query)  

end

