-- =============================================
-- Author:		Yo
-- Create date: 29 JULY 2013
-- Description:	Inquiry Format 3, Comparisson By Class
-- Last Update: 27 March 2014 / 12 Dec 2014 / 11 March 2016
-- Notes	  : @Loop diganti dengan @firstmonth / penambahan filter type angkot , cab over
-- Update 11 March 2016 : Add SUV Medium
-- =============================================
ALTER PROCEDURE [dbo].[usprpt_MarketShareYtdByClass] 
@DealerCode varchar(15),
@Year decimal(4),
@FirstMonth int,
@Month int,
@Type char(1),
@AreaCode varchar(15)

--exec usprpt_MarketShareYtdByClass '', 2016,1, 3, '6',''
--declare @DealerCode varchar(15)
--declare @Year decimal(4)
--declare @FirstMonth int
--declare @Month int
--declare @Type char(1)
--declare @AreaCode varchar(15)

--set @DealerCode = ''
--set @Year = 2016
--set @FirstMonth = 1
--set @Month = 3
--set @Type = '3'
--set @AreaCode = ''

AS
BEGIN

if (@Year = 0 and @Month = 0) return

declare @brandTemp VARCHAR(max)
declare @brandTempTot VARCHAR(max)
declare @brandTempTot1 VARCHAR(max)
declare @brandTempTot2 VARCHAR(max)
declare @brandTot VARCHAR(max)
declare @brandTot1 VARCHAR(max)
declare @selectBrand VARCHAR(max)
declare @brandPivot VARCHAR(max)
declare @Show as int

declare @Query as varchar(max)

declare @SegmentCode as varchar(30)

if (@Type = '1')
begin    
	set @SegmentCode = 'BONNET LOW'
end
else if (@Type = '2')
begin
	set @SegmentCode = 'MINI HATCHBACK'
end
else if (@Type = '3')
begin
	set @SegmentCode = 'PICK UP LOW'	
end
else if(@Type = '4')
begin
	set @SegmentCode = 'LCGC'
end
else if(@Type = '5')
begin
	set @SegmentCode = 'ANGKOT'
end
else if(@Type = '6')
begin
	set @SegmentCode = 'CAB OVER'
end
else if(@Type = '7')
begin
	set @SegmentCode = 'SUV MEDIUM'
end

select @brandPivot = STUFF((
	select ltrim(ValCol) from (
	select distinct ',' + quotename(ReferenceCode) ValCol, ReportSequence
	from msMstReference	
	left join msMstModel model on model.GroupModel = ReferenceCode 
	where ReferenceType = 'GROUPMODEL' and model.SegmentCode = @SegmentCode and model.Status = 1) t order by t.ReportSequence
	for xml path('')), 1, 1, '') 
select @selectBrand = STUFF((
	select ltrim(ValCol) from (
	select distinct ',' + 'isnull(' + quotename(ReferenceCode) + ',0) as ' + quotename(ReferenceCode) ValCol, ReportSequence
	from msMstReference	
	left join msMstModel model on model.GroupModel = ReferenceCode 
	where ReferenceType = 'GROUPMODEL' and model.SegmentCode = @SegmentCode and model.Status = 1) t order by t.ReportSequence
	for xml path('')), 1, 1, '') 
select @brandTot = STUFF((
	select ltrim(ValCol) from (
	select distinct ','+'isnull(sum(' + quotename(ReferenceCode) + '),0) as' + quotename(ReferenceCode) ValCol, ReportSequence
	from msMstReference	
	left join msMstModel model on model.GroupModel = ReferenceCode 
	where ReferenceType = 'GROUPMODEL' and model.SegmentCode = @SegmentCode and model.Status = 1) t order by ReportSequence
	for xml path('')), 1, 1, '') 
select @brandTempTot1 = STUFF((
	select ltrim(ValCol) from (
	select distinct '+' + quotename(ReferenceCode) ValCol, ReportSequence
	from msMstReference	
	left join msMstModel model on model.GroupModel = ReferenceCode 
	where ReferenceType = 'GROUPMODEL' and model.SegmentCode = @SegmentCode and model.Status = 1) t order by ReportSequence
	for xml path('')), 1, 1, '') 
select @brandTempTot2 = STUFF((
	select ltrim(ValCol) from (
	select distinct '+' + 'isnull(sum(' + quotename(ReferenceCode) + '),0)' ValCol, ReportSequence
	from msMstReference	
	left join msMstModel model on model.GroupModel = ReferenceCode 
	where ReferenceType = 'GROUPMODEL' and model.SegmentCode = @SegmentCode and model.Status = 1) t order by ReportSequence
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
set @sumMonth = 'sum(' + (select substring(@sumMonthTemp,1,(select len(@sumMonthTemp) - 1))) + ') Val'       

declare @pctBrand as varchar(max)
set @pctBrand =  STUFF((
	select ltrim(ValCol) from (
	select distinct ',' + 'CASE WHEN TOTAL = 0 THEN 0 ELSE CAST(((isnull(' + quotename(ReferenceCode) + ',0) / CAST(TOTAL AS DECIMAL(38,2)))*100)as decimal(5,1)) END as ' + quotename(ReferenceCode) ValCol, ReportSequence
	from msMstReference	
	left join msMstModel model on model.GroupModel = ReferenceCode 
	where ReferenceType = 'GROUPMODEL' and model.SegmentCode = @SegmentCode and model.Status = 1) t order by t.ReportSequence
	for xml path('')), 1, 1, '') 	
    
set @Query = 'select * into #tempUnion from(
select 
area.AreaCode, area.AreaName, rg.RegionCode, rg.RegionName,(select '+@sumMonth+' from msTrnMarketShare 
inner join msMstModel model on model.ModelType = msTrnMarketShare.ModelType AND model.Variant = msTrnMarketShare.Variant
where Year = '+convert(varchar,@Year)+' and DealerCode = ms.DealerCode and msTrnMarketShare.ModelType = ms.ModelType 
and msTrnMarketShare.Variant = ms.Variant and msTrnMarketShare.CityCode = ms.CityCode and model.BrandCode = md.BrandCode and model.Status = 1
group by msTrnMarketShare.DealerCode, msTrnMarketShare.ModelType, msTrnMarketShare.Variant, msTrnMarketShare.CityCode) Val, 
md.GroupModel
from msMstRegion rg
inner join msMstDealer dealer on dealer.DealerCode != '+''''''+'
left join msMstDealerCity dlrCity on dlrCity.DealerCode = dealer.DealerCode
inner join msMstCity ct on rg.RegionCode = ct.RegionCode and ct.CityCode = dlrCity.CityCode
left join msTrnMarketShare ms on ct.CityCode = ms.CityCode 
inner join msMstModel md on md.ModelType = ms.ModelType AND md.Variant = ms.Variant and md.Status = 1
left join msMstArea area on area.AreaCode = ct.AreaCode
where Year = '+convert(varchar,@Year)+'
and (case when '''+@DealerCode+''' = '+''''''+' then '''+@DealerCode+''' else dealer.DealerCode end) = '''+@DealerCode+'''
and (case when '''+@AreaCode+''' = '+''''''+' then '''+@AreaCode+''' else area.AreaCode end) = '''+@AreaCode+'''
group by rg.RegionCode,area.AreaCode, area.AreaName,rg.RegionName,Year, md.GroupModel, ms.dealercode, ms.ModelType, ms.variant, ms.citycode,md.brandcode
) #tempUnion order by AreaCode

select AreaCode, AreaName, RegionCode, RegionName, '+@selectBrand+' into #tempPivot from(
select * from #tempUnion	
pivot (sum(Val) for GroupModel in ('+@brandPivot+')) pvt1
) #tempPivot

select * into #tempTotal from(
select #tempPivot.RegionCode, #tempPivot.RegionName,sum('+ @brandTempTot1 +') Val
from #tempPivot
group by #tempPivot.RegionCode,#tempPivot.RegionName
) #tempTotal order by RegionCode

select * into #tempTotal1 from(
select #tempPivot.AreaCode, #tempPivot.AreaName, sum('+@brandTempTot1+') TOTAL
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
select '+'''GRAND TOTAL'''+' AreaCode, '+'''GRAND TOTAL'''+' AreaName, '+'''GRAND TOTAL'''+' RegionCode, '+''''''+' RegionName, '+ @brandTot +', sum('+@brandTempTot1+') TOTAL
from #tempPivot
) #Pivot order by AreaCode

select * from #Pivot

select 1 SeqNo, '+'''B'''+' RegionName, '+ @brandTot +', '+@brandTempTot2+' TOTAL
from #tempPivot
where RegionCode in (001, 003, 004, 005)
union
select 2 SeqNo, '+'''F'''+' RegionName, '+ @brandTot +', '+@brandTempTot2+' TOTAL
from #tempPivot
where RegionCode in (002, 007, 009)
union
select 3 SeqNo, '+'''T'''+' RegionName, '+ @brandTot +', '+@brandTempTot2+' TOTAL
from #tempPivot
where RegionCode in (008)
union
select 4 SeqNo, '+'''A'''+' RegionName, '+ @brandTot +', '+@brandTempTot2+' TOTAL
from #tempPivot
where RegionCode in (006, 010)
union
select 5 SeqNo, '+'''TOTAL'''+' RegionName, '+ @brandTot +', '+@brandTempTot2+' TOTAL
from #tempPivot
where RegionCode in (001, 003, 004, 005, 002, 007, 009, 008, 006, 010)

select AreaCode, AreaName, #Pivot.RegionCode, #Pivot.RegionName, ' + @pctBrand + '
, CAST(CASE WHEN TOTAL = 0 THEN 0 ELSE (TOTAL/ CAST(TOTAL AS DECIMAL(38,2)))*100 END AS DECIMAL(5,1)) TOTAL from #Pivot'

print(@Query)
exec(@Query)  

end
