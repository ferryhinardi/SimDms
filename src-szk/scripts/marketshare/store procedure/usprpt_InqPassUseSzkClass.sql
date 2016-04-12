-- =============================================
-- Author:		yo
-- Create date: 29 JULY 2013
-- Description:	Format 1
-- Last Update: 17 April 2014
-- =============================================

ALTER PROCEDURE [dbo].[usprpt_InqPassUseSzkClass]
@ProvinceCode varchar(10),
@DealerCode varchar(10),
@RegencyCode varchar(10),
@Year decimal(4),
@Month int

--exec usprpt_InqPassUseSzkClass '0800','6469401','', 2014, 10
--declare @ProvinceCode varchar(10)
--declare @DealerCode varchar(10)
--declare @RegencyCode varchar(10)
--declare @Year decimal(4)
--declare @Month int

--set @ProvinceCode = '0800'
--set @DealerCode = '6469401'
--set @RegencyCode = ''
--set @Year = 2014
--set @Month = 10

as
begin 

declare @tblYear as table
(
	Year decimal(4)
)

declare @tblMonth as table
(
	Month int,
	VarMonth varchar(10)
)

declare @FromYear as decimal(4)
set @FromYear = @Year - 5

while @FromYear <= @Year
begin
	insert into @tblYear values (@FromYear)
	set @FromYear = @FromYear + 1
end

declare @Loop as int 
set @Loop = 1

while @Loop <= 12
begin
	insert into @tblMonth select @Loop, substring(datename(mm, dateadd(month, @Loop, 0)-1),1, 3)
	set @Loop = @Loop + 1
end

declare @sumMonth as varchar(max)

select @sumMonth = STUFF((
select case when Month=12 then 
	'sum(' + quotename(VarMonth+convert(varchar,y.Year)) + ') as' + quotename(VarMonth + convert(varchar,y.Year)) + ', sum(Jan'+convert(varchar,y.Year)+'+Feb'+convert(varchar,y.Year)+'+Mar'+convert(varchar,y.Year)+'+Apr'+convert(varchar,y.Year)+
	   '+May'+convert(varchar,y.Year)+'+Jun'+convert(varchar,y.Year)+'+Jul'+convert(varchar,y.Year)+'+Aug'+convert(varchar,y.Year)+
	   '+Sep'+convert(varchar,y.Year)+'+Oct'+convert(varchar,y.Year)+'+Nov'+convert(varchar,y.Year)+'+Dec'+convert(varchar,y.Year)+ ') as [Total'+
	   convert(varchar,y.Year)+'] ' + ','  
	 else
		'sum(' + quotename(VarMonth+convert(varchar,y.Year)) + ') as' + quotename(VarMonth + convert(varchar,y.Year)) + ', '
	end  
from @tblMonth	    
left join @tblYear y on y.Year != 0
order by y.Year, Month
for xml path('')), 1, 0, '') 

declare @selectMonth as varchar(max)
select @selectMonth = STUFF((
   select 'case when ' + VarMonth + 'Year = ' + ''''+ VarMonth+convert(varchar,y.Year) + ''''+ ' then ' + VarMonth + ' else 0 end ' + quotename(VarMonth + convert(varchar,y.Year)) + ',' 
    from @tblMonth	    
    left join @tblYear y on y.Year != 0
    order by Month
    for xml path('')), 1, 0, '') 

declare @sumMonthPrev as varchar(max)
select @sumMonthPrev = STUFF((
	select VarMonth+convert(varchar, @Year-1)+'+' 
	from @tblMonth
	where Month <= @Month
	   for xml path('')), 1, 0, '') 

declare @sumMonthCur as varchar(max)
select @sumMonthCur = STUFF((
	select VarMonth+convert(varchar, @Year)+'+' 
	from @tblMonth
	where Month <= @Month
	   for xml path('')), 1, 0, '') 

declare @selectCol as varchar(max)
select @selectCol = STUFF((
	select case when Month=12 then 
		VarMonth+convert(varchar, y.Year)+',Total'+convert(varchar, y.Year)+',' 
		else
		VarMonth+convert(varchar, y.Year)+',' end
	from @tblMonth	
	left join @tblYear y on y.Year != 0
	order by y.Year, Month
	   for xml path('')), 1, 0, '') 

declare @Query1 as varchar(max)


set @Query1 = 	
'select * into #t1 from(
select DealerCode, Dealer, ReportSequence, ReferenceCode, BrandName, ModelType, IsSuzukiClass,' + (select substring(@sumMonth,1,(select len(@sumMonth) - 1))) +'
from (
	select distinct CityCode, DealerCode, Dealer, ReportSequence, ReferenceCode, BrandName, ModelType, IsSuzukiClass
	, '+ (select substring(@selectMonth,1,(select len(@selectMonth) - 1))) +
	' from(
		SELECT distinct model.*,
		SUM(Jan) Jan, SUM(Feb) Feb, SUM(Mar) Mar, SUM(Apr) Apr, SUM(May) May, SUM(Jun) Jun, SUM(Jul) Jul, 
		SUM(Aug) Aug, SUM(Sep) Sep, SUM(Oct) Oct, SUM(Nov) Nov, SUM(Dec) Dec, 
		'+'''Jan'''+'+convert(varchar,Year,4) JanYear, '+'''Feb'''+'+convert(varchar,Year,4) FebYear,
		'+'''Mar'''+'+convert(varchar,Year,4) MarYear, '+'''Apr'''+'+convert(varchar,Year,4) AprYear, 
		'+'''May'''+'+convert(varchar,Year,4) MayYear, '+'''Jun'''+'+convert(varchar,Year,4) JunYear,
		'+'''Jul'''+'+convert(varchar,Year,4) JulYear, '+'''Aug'''+'+convert(varchar,Year,4) AugYear,
		'+'''Sep'''+'+convert(varchar,Year,4) SepYear, '+'''Oct'''+'+convert(varchar,Year,4) OctYear,
		'+'''Nov'''+'+convert(varchar,Year,4) NovYear, '+'''Dec'''+'+convert(varchar,Year,4) DecYear
		FROM 
			(
			select distinct 
				gnMstDealerMapping.DealerCode, 
				gnMstDealerMapping.DealerCode + '+''' (''+ gnMstDealerMapping.DealerAbbreviation + '')''' +' Dealer, ref.ReportSequence, 
				SegmentCode ReferenceCode, ref.BrandName, ModelType, isSuzukiClass,  msMstCity.CityCode
			from msMstModel
				inner join msMstReference ref on msMstModel.SegmentCode = ref.ReferenceCode and ref.ReferenceType = '+'''SEGMENT'''+'
				left join gnMstDealerMapping on DealerCode != '+''''''+'
				left join msMstDealerCity on gnMstDealerMapping.DealerCode = msMstDealerCity.DealerCode 				
				left join msMstCity on msMstDealerCity.CityCode = msMstCity.CityCode
				where SegmentCode != '+''''''+' 
				and (case when '''+ @DealerCode + ''' = '+''''''+' then ''' + @DealerCode + ''' else gnMstDealerMapping.DealerCode end) = '''+ @DealerCode +'''
				and (case when '''+ @ProvinceCode + ''' = '+''''''+' then ''' + @ProvinceCode + ''' else msMstCity.ProvinceCode end) = '''+ @ProvinceCode +'''
				and (case when '''+ @RegencyCode + ''' = '+''''''+' then ''' + @RegencyCode + ''' else msMstCity.CityCode end) = '''+ @RegencyCode +'''
			) model 
		inner join msTrnMarketShare marketShare on marketShare.ModelType = model.ModelType and marketshare.DealerCode = model.DealerCode and marketshare.CityCode = model.CityCode				
		and Year between '+ convert(varchar,@Year-5) +' and ' + convert(varchar,@Year) +'	    
		group by marketShare.Year, model.DealerCode, model.Dealer, model.ReportSequence, model.ReferenceCode, model.BrandName, model.ModelType, model.IsSuzukiClass, model.CityCode
	) #temp 
) #temp1
group by DealerCode, Dealer, ReportSequence, ReferenceCode, BrandName, ModelType, IsSuzukiClass) #t1
order by DealerCode, IsSuzukiClass desc, ReportSequence

select * into #t2 from(
	select DealerCode, Dealer, 1000 ReportSequence, ReferenceCode, BrandName, '+''''''+' ModelType, IsSuzukiClass, ' + (select substring(@sumMonth,1,(select len(@sumMonth) - 1))) + '
	from #t1
	group by DealerCode, Dealer, ReportSequence, ReferenceCode, BrandName, IsSuzukiClass) #t2

select * into #t3 from(
select * 
	, case when 
		(select top 1 [Total'+ convert(varchar,@Year-1) +'] from #t2 where DealerCode = temp.DealerCode and ReportSequence = 1000 and ReferenceCode = temp.ReferenceCode and IsSuzukiClass = temp.IsSuzukiClass and ModelType = '+''''''+') = 0 then 0 
	  else
		(cast([Total'+ convert(varchar,@Year-1) +'] as decimal(38,2)) / (select top 1 [Total'+ convert(varchar,@Year-1) +'] from #t2 where DealerCode = temp.DealerCode and ReportSequence = 1000 and ReferenceCode = temp.ReferenceCode and IsSuzukiClass = temp.IsSuzukiClass and ModelType = '+''''''+'))  * 100
	  end PercentPrevYear
	, ''' + (select substring(upper(datename(mm, dateadd(month, @Month - 1, 0)-1)), 1, 3)) + ''' ColumnNameCur1
	, [' + (select substring(upper(datename(mm, dateadd(month, @Month - 1, 0)-1)), 1, 3)) + convert(varchar,@Year) + '] ValCur1 
	, case when 
		(select top 1 ['+ + (select substring(upper(datename(mm, dateadd(month, @Month - 1, 0)-1)), 1, 3)) + convert(varchar,@Year) + '] from #t2 where DealerCode = temp.DealerCode and ReportSequence = 1000 and ReferenceCode = temp.ReferenceCode and IsSuzukiClass = temp.IsSuzukiClass and ModelType = '+''''''+') = 0 then 0 
	  else		
		(cast([' + (select substring(upper(datename(mm, dateadd(month, @Month - 1, 0)-1)), 1, 3)) + convert(varchar,@Year) + '] as decimal(38,2)) / (select top 1 ['+ + (select substring(upper(datename(mm, dateadd(month, @Month - 1, 0)-1)), 1, 3)) + convert(varchar,@Year) + '] from #t2 where DealerCode = temp.DealerCode and ReportSequence = 1000 and ReferenceCode = temp.ReferenceCode and IsSuzukiClass = temp.IsSuzukiClass and ModelType = '+''''''+')) * 100
	  end PercentCur1
	, ''' + (select substring(upper(datename(mm, dateadd(month, @Month, 0)-1)), 1, 3)) + ''' ColumnNameCur2
	, [' + (select substring(upper(datename(mm, dateadd(month, @Month, 0)-1)), 1, 3)) + convert(varchar,@Year) + '] ValCur2 
	, case when 
		(select top 1 ['+ + (select substring(upper(datename(mm, dateadd(month, @Month, 0)-1)), 1, 3)) + convert(varchar,@Year) + '] from #t2 where DealerCode = temp.DealerCode and ReportSequence = 1000 and ReferenceCode = temp.ReferenceCode and IsSuzukiClass = temp.IsSuzukiClass and ModelType = '+''''''+') = 0 then 0 
	  else		
		(cast([' + (select substring(upper(datename(mm, dateadd(month, @Month, 0)-1)), 1, 3)) + convert(varchar,@Year) + '] as decimal(38,2)) / (select top 1 ['+ + (select substring(upper(datename(mm, dateadd(month, @Month, 0)-1)), 1, 3)) + convert(varchar,@Year) + '] from #t2 where DealerCode = temp.DealerCode and ReportSequence = 1000 and ReferenceCode = temp.ReferenceCode and IsSuzukiClass = temp.IsSuzukiClass and ModelType = '+''''''+')) * 100
	  end PercentCur2
from
	(
	select * from #t1
	union
	select * from #t2
	) temp ) #t3

select * into #t4 from(
select DealerCode
		, Dealer, ReportSequence, ReferenceCode, BrandName, ModelType, IsSuzukiClass
		, ' + (select substring(@selectCol,1,(select len(@selectCol) - 1))) + '		
		, PercentPrevYear
		, ColumnNameCur1, ValCur1
		, PercentCur1
		, ColumnNameCur2, ValCur2
		, PercentCur2
		, PercentCur2 - PercentCur1 PlusMinCurPercent 
		, cast((case when ValCur1 = 0 then 0 else (ValCur2/ cast(ValCur1 as decimal(38,2)) - 1) end) *100 as decimal(6,1)) PlusMinCurVal
		, '+ (select substring(@sumMonthPrev,1,(select len(@sumMonthPrev) - 1))) +' ColumnValPrev
		, case when (select top 1 ('+ (select substring(@sumMonthPrev,1,(select len(@sumMonthPrev) - 1))) +') from #t2 where DealerCode = #t3.DealerCode and ReportSequence = 1000 and ReferenceCode = #t3.ReferenceCode and IsSuzukiClass = #t3.IsSuzukiClass and ModelType = '+''''''+') = 0 then 0
			else
		  (('+ (select substring(@sumMonthPrev,1,(select len(@sumMonthPrev) - 1))) +') / cast((select top 1 ('+ (select substring(@sumMonthPrev,1,(select len(@sumMonthPrev) - 1))) +') from #t2 where DealerCode = #t3.DealerCode and ReportSequence = 1000 and ReferenceCode = #t3.ReferenceCode and IsSuzukiClass = #t3.IsSuzukiClass and ModelType = '+''''''+') as decimal(38,2))) * 100
		  end PercentPrev  		
		, '+ (select substring(@sumMonthCur,1,(select len(@sumMonthCur) - 1))) +' ColumnValCur
		, case when (select top 1 ('+ (select substring(@sumMonthCur,1,(select len(@sumMonthCur) - 1))) +') from #t2 where DealerCode = #t3.DealerCode and ReportSequence = 1000 and ReferenceCode = #t3.ReferenceCode and IsSuzukiClass = #t3.IsSuzukiClass and ModelType = '+''''''+') = 0 then 0
			else
		  (('+ (select substring(@sumMonthCur,1,(select len(@sumMonthCur) - 1))) +') / cast((select top 1 ('+ (select substring(@sumMonthCur,1,(select len(@sumMonthCur) - 1))) +') from #t2 where DealerCode = #t3.DealerCode and ReportSequence = 1000 and ReferenceCode = #t3.ReferenceCode and IsSuzukiClass = #t3.IsSuzukiClass and ModelType = '+''''''+') as decimal(38,2))) * 100
		  end PercentCur		
	from #t3) #t4	

select DealerCode
	, Dealer, ReportSequence, ReferenceCode, BrandName, ModelType, IsSuzukiClass
	, ' + (select substring(@selectCol,1,(select len(@selectCol) - 1))) + '		
	, PercentPrevYear, ColumnNameCur1, ValCur1, PercentCur1, ColumnNameCur2, ValCur2, PercentCur2, PlusMinCurPercent, PlusMinCurVal, ColumnValPrev
	, PercentPrev, ColumnValCur, PercentCur, PercentCur - PercentPrev PlusMinValCurPrev
	, (case when ColumnValPrev = 0 then 0 else ColumnValCur / cast(ColumnValPrev as decimal(13,1)) -1 end) * 100 PlusMinPercentCurPrev	
	from #t4
order by DealerCode, IsSuzukiClass desc, ReferenceCode, ReportSequence asc

drop table #t1 , #t2, #t3, #t4'
 
print(@Query1)

declare @tblTemp1 as table
(
	DealerCode varchar(50), Dealer varchar(100), ReportSequence int, ReferenceCode varchar(50), BrandName varchar(50), ModelType varchar(50), IsSuzukiClass bit,
	JanYear_5 bigint, FebYear_5 bigint, MarYear_5 bigint, AprYear_5 bigint, MayYear_5 bigint, JunYear_5 bigint, JulYear_5 bigint, AugYear_5 bigint, SepYear_5 bigint, OctYear_5 bigint, NovYear_5 bigint, DecYear_5 bigint, TotalYear_5 bigint,
	JanYear_4 bigint, FebYear_4 bigint, MarYear_4 bigint, AprYear_4 bigint, MayYear_4 bigint, JunYear_4 bigint, JulYear_4 bigint, AugYear_4 bigint, SepYear_4 bigint, OctYear_4 bigint, NovYear_4 bigint, DecYear_4 bigint, TotalYear_4 bigint,
	JanYear_3 bigint, FebYear_3 bigint, MarYear_3 bigint, AprYear_3 bigint, MayYear_3 bigint, JunYear_3 bigint, JulYear_3 bigint, AugYear_3 bigint, SepYear_3 bigint, OctYear_3 bigint, NovYear_3 bigint, DecYear_3 bigint, TotalYear_3 bigint,	
	JanYear_2 bigint, FebYear_2 bigint, MarYear_2 bigint, AprYear_2 bigint, MayYear_2 bigint, JunYear_2 bigint, JulYear_2 bigint, AugYear_2 bigint, SepYear_2 bigint, OctYear_2 bigint, NovYear_2 bigint, DecYear_2 bigint, TotalYear_2 bigint,
	JanPrevYear bigint, FebPrevYear bigint, MarPrevYear bigint, AprPrevYear bigint, MayPrevYear bigint, JunPrevYear bigint, JulPrevYear bigint, AugPrevYear bigint, SepPrevYear bigint, OctPrevYear bigint, NovPrevYear bigint, DecPrevYear bigint, TotalPrevYear bigint,
	JanCurYear bigint, FebCurYear bigint, MarCurYear bigint, AprCurYear bigint, MayCurYear bigint, JunCurYear bigint, JulCurYear bigint, AugCurYear bigint, SepCurYear bigint, OctCurYear bigint, NovCurYear bigint, DecCurYear bigint, TotalCurYear bigint,
	PercentPrevYear decimal(13,1), ColumnNameCur1 varchar(50), ValCur1 int, PercentCur1 decimal(13,1), ColumnNameCur2 varchar(50), ValCur2 int,
	PercentCur2 decimal(13,1), PlusMinCurPercent decimal(13,1), PlusMinCurVal decimal(13,1), ColumnValPrev int, PercentPrev decimal(13,1),
	ColumnValCur int, PercentCur decimal(13,1), PlusMinValCurPrev decimal(13,1), PlusMinPercentCurPrev decimal(13,1)
)

insert into @tblTemp1
exec(@query1) 

select tblTemp.* from @tblTemp1 tblTemp
inner join msMstReference ref on tblTemp.ReferenceCode = ref.ReferenceCode and ref.ReferenceType = 'SEGMENT'
order by tblTemp.DealerCode, IsSuzukiClass desc, ref.ReportSequence

select row_number() over (order by DealerCode) SeqNo, DealerCode, Dealer, Abrv from	
(
select distinct
	msMstDealer.DealerCode, 
	msMstDealer.DealerCode + ' ('+ msMstDealer.DealerAbbreviation +')' Dealer, msMstDealer.DealerAbbreviation Abrv
from msMstDealer
	inner join msMstDealerCity on msMstDealerCity.DealerCode = msMstDealer.DealerCode
	inner join msMstCity on msMstCity.CityCode = msMstDealerCity.CityCode --and msMstCity.AreaCode = msMstArea.AreaCode
where (case when @DealerCode = '' then @DealerCode else msMstDealer.DealerCode end) = @DealerCode
and (case when @ProvinceCode = '' then @ProvinceCode else msMstCity.ProvinceCode end) = @ProvinceCode
) a
	
select * into #tSummary from(
	select DealerCode, ReportSequence, IsSuzukiClass, BrandName, ReferenceCode, 
	sum(JanYear_5) JanYear_5, sum(FebYear_5) FebYear_5, sum(MarYear_5) MarYear_5, sum(AprYear_5) AprYear_5, sum(MayYear_5) MayYear_5, sum(JunYear_5) JunYear_5, sum(JulYear_5) JulYear_5, sum(AugYear_5) AugYear_5, sum(SepYear_5) SepYear_5, sum(OctYear_5) OctYear_5, sum(NovYear_5) NovYear_5, sum(DecYear_5) DecYear_5, sum(TotalYear_5) TotalYear_5,
	sum(JanYear_4) JanYear_4, sum(FebYear_4) FebYear_4, sum(MarYear_4) MarYear_4, sum(AprYear_4) AprYear_4, sum(MayYear_4) MayYear_4, sum(JunYear_4) JunYear_4, sum(JulYear_4) JulYear_4, sum(AugYear_4) AugYear_4, sum(SepYear_4) SepYear_4, sum(OctYear_4) OctYear_4, sum(NovYear_4) NovYear_4, sum(DecYear_4) DecYear_4, sum(TotalYear_4) TotalYear_4,
	sum(JanYear_3) JanYear_3, sum(FebYear_3) FebYear_3, sum(MarYear_3) MarYear_3, sum(AprYear_3) AprYear_3, sum(MayYear_3) MayYear_3, sum(JunYear_3) JunYear_3, sum(JulYear_3) JulYear_3, sum(AugYear_3) AugYear_3, sum(SepYear_3) SepYear_3, sum(OctYear_3) OctYear_3, sum(NovYear_3) NovYear_3, sum(DecYear_3) DecYear_3, sum(TotalYear_3) TotalYear_3,
	sum(JanYear_2) JanYear_2, sum(FebYear_2) FebYear_2, sum(MarYear_2) MarYear_2, sum(AprYear_2) AprYear_2, sum(MayYear_2) MayYear_2, sum(JunYear_2) JunYear_2, sum(JulYear_2) JulYear_2, sum(AugYear_2) AugYear_2, sum(SepYear_2) SepYear_2, sum(OctYear_2) OctYear_2, sum(NovYear_2) NovYear_2, sum(DecYear_2) DecYear_2, sum(TotalYear_2) TotalYear_2,
	sum(JanPrevYear) JanPrevYear, sum(FebPrevYear) FebPrevYear, sum(MarPrevYear) MarPrevYear, sum(AprPrevYear) AprPrevYear, sum(MayPrevYear) MayPrevYear, sum(JunPrevYear) JunPrevYear, sum(JulPrevYear) JulPrevYear, sum(AugPrevYear) AugPrevYear, sum(SepPrevYear) SepPrevYear, sum(OctPrevYear) OctPrevYear, sum(NovPrevYear) NovPrevYear, sum(DecPrevYear) DecPrevYear, sum(TotalPrevYear) TotalPrevYear,	
	sum(JanCurYear) JanCurYear, sum(FebCurYear) FebCurYear, sum(MarCurYear) MarCurYear, sum(AprCurYear) AprCurYear, sum(MayCurYear) MayCurYear, sum(JunCurYear) JunCurYear, sum(JulCurYear) JulCurYear, sum(AugCurYear) AugCurYear, sum(SepCurYear) SepCurYear, sum(OctCurYear) OctCurYear, sum(NovCurYear) NovCurYear, sum(DecCurYear) DecCurYear, sum(TotalCurYear) TotalCurYear,		
	sum(PercentPrevYear) PercentPrevYear, sum(ValCur1) ValCur1, sum(PercentCur1) PercentCur1, sum(ValCur2) ValCur2, sum(PercentCur2) PercentCur2,
	sum(PlusMinCurPercent) PlusMinCurPercent, sum(PlusMinCurVal) PlusMinCurVal, sum(ColumnValPrev) ColumnValPrev, sum(PercentPrev) PercentPrev,
	sum(ColumnValCur) ColumnValCur, sum(PercentCur) PercentCur, sum(PlusMinValCurPrev) PlusMinValCurPrev, 
	sum(PlusMinPercentCurPrev) PlusMinPercentCurPrev
	from @tblTemp1 where ModelType = '' 
	group by DealerCode, ReportSequence, IsSuzukiClass,  ReferenceCode, BrandName, ColumnNameCur1, ColumnNameCur2	
	) #tSummary order by DealerCode, IsSuzukiClass desc, ReportSequence
					
select DealerCode, IsSuzukiClass, BrandName, 'SUB TOTAL ' + BrandName ModelType,
	sum(JanYear_5) JanYear_5, sum(FebYear_5) FebYear_5, sum(MarYear_5) MarYear_5, sum(AprYear_5) AprYear_5, sum(MayYear_5) MayYear_5, sum(JunYear_5) JunYear_5, sum(JulYear_5) JulYear_5, sum(AugYear_5) AugYear_5, sum(SepYear_5) SepYear_5, sum(OctYear_5) OctYear_5, sum(NovYear_5) NovYear_5, sum(DecYear_5) DecYear_5, sum(TotalYear_5) TotalYear_5,
	sum(JanYear_4) JanYear_4, sum(FebYear_4) FebYear_4, sum(MarYear_4) MarYear_4, sum(AprYear_4) AprYear_4, sum(MayYear_4) MayYear_4, sum(JunYear_4) JunYear_4, sum(JulYear_4) JulYear_4, sum(AugYear_4) AugYear_4, sum(SepYear_4) SepYear_4, sum(OctYear_4) OctYear_4, sum(NovYear_4) NovYear_4, sum(DecYear_4) DecYear_4, sum(TotalYear_4) TotalYear_4,
	sum(JanYear_3) JanYear_3, sum(FebYear_3) FebYear_3, sum(MarYear_3) MarYear_3, sum(AprYear_3) AprYear_3, sum(MayYear_3) MayYear_3, sum(JunYear_3) JunYear_3, sum(JulYear_3) JulYear_3, sum(AugYear_3) AugYear_3, sum(SepYear_3) SepYear_3, sum(OctYear_3) OctYear_3, sum(NovYear_3) NovYear_3, sum(DecYear_3) DecYear_3, sum(TotalYear_3) TotalYear_3,
	sum(JanYear_2) JanYear_2, sum(FebYear_2) FebYear_2, sum(MarYear_2) MarYear_2, sum(AprYear_2) AprYear_2, sum(MayYear_2) MayYear_2, sum(JunYear_2) JunYear_2, sum(JulYear_2) JulYear_2, sum(AugYear_2) AugYear_2, sum(SepYear_2) SepYear_2, sum(OctYear_2) OctYear_2, sum(NovYear_2) NovYear_2, sum(DecYear_2) DecYear_2, sum(TotalYear_2) TotalYear_2,
	sum(JanPrevYear) JanPrevYear, sum(FebPrevYear) FebPrevYear, sum(MarPrevYear) MarPrevYear, sum(AprPrevYear) AprPrevYear, sum(MayPrevYear) MayPrevYear, sum(JunPrevYear) JunPrevYear, sum(JulPrevYear) JulPrevYear, sum(AugPrevYear) AugPrevYear, sum(SepPrevYear) SepPrevYear, sum(OctPrevYear) OctPrevYear, sum(NovPrevYear) NovPrevYear, sum(DecPrevYear) DecPrevYear, sum(TotalPrevYear) TotalPrevYear,	
	sum(JanCurYear) JanCurYear, sum(FebCurYear) FebCurYear, sum(MarCurYear) MarCurYear, sum(AprCurYear) AprCurYear, sum(MayCurYear) MayCurYear, sum(JunCurYear) JunCurYear, sum(JulCurYear) JulCurYear, sum(AugCurYear) AugCurYear, sum(SepCurYear) SepCurYear, sum(OctCurYear) OctCurYear, sum(NovCurYear) NovCurYear, sum(DecCurYear) DecCurYear, sum(TotalCurYear) TotalCurYear,		
	case when sum(PercentPrevYear) = 0 then 0 else 100 end PercentPrevYear, sum(ValCur1) ValCur1, case when sum(PercentCur1) = 0 then 0 else 100 end PercentCur1, 
	sum(ValCur2) ValCur2, case when sum(PercentCur2) = 0 then 0 else 100 end PercentCur2,
	sum(PlusMinCurPercent) PlusMinCurPercent, sum(PlusMinCurVal) PlusMinCurVal, sum(ColumnValPrev) ColumnValPrev, case when sum(PercentPrev) = 0 then 0 else 100 end PercentPrev,
	sum(ColumnValCur) ColumnValCur, case when sum(PercentCur) = 0 then 0 else 100 end PercentCur, sum(PlusMinValCurPrev) PlusMinValCurPrev, 
	sum(PlusMinPercentCurPrev) PlusMinPercentCurPrev
	from #tSummary
	group by DealerCode, IsSuzukiClass, BrandName
having count(ReferenceCode) > 1
order by DealerCode, IsSuzukiClass desc

select * into #tTotal from(
select DealerCode, IsSuzukiClass, (case when IsSuzukiClass = 1 then 'TOTAL SUZUKI CLASS' else 'TOTAL NON SUZUKI CLASS' end) ModelType,
	sum(JanYear_5) JanYear_5, sum(FebYear_5) FebYear_5, sum(MarYear_5) MarYear_5, sum(AprYear_5) AprYear_5, sum(MayYear_5) MayYear_5, sum(JunYear_5) JunYear_5, sum(JulYear_5) JulYear_5, sum(AugYear_5) AugYear_5, sum(SepYear_5) SepYear_5, sum(OctYear_5) OctYear_5, sum(NovYear_5) NovYear_5, sum(DecYear_5) DecYear_5, sum(TotalYear_5) TotalYear_5,
	sum(JanYear_4) JanYear_4, sum(FebYear_4) FebYear_4, sum(MarYear_4) MarYear_4, sum(AprYear_4) AprYear_4, sum(MayYear_4) MayYear_4, sum(JunYear_4) JunYear_4, sum(JulYear_4) JulYear_4, sum(AugYear_4) AugYear_4, sum(SepYear_4) SepYear_4, sum(OctYear_4) OctYear_4, sum(NovYear_4) NovYear_4, sum(DecYear_4) DecYear_4, sum(TotalYear_4) TotalYear_4,
	sum(JanYear_3) JanYear_3, sum(FebYear_3) FebYear_3, sum(MarYear_3) MarYear_3, sum(AprYear_3) AprYear_3, sum(MayYear_3) MayYear_3, sum(JunYear_3) JunYear_3, sum(JulYear_3) JulYear_3, sum(AugYear_3) AugYear_3, sum(SepYear_3) SepYear_3, sum(OctYear_3) OctYear_3, sum(NovYear_3) NovYear_3, sum(DecYear_3) DecYear_3, sum(TotalYear_3) TotalYear_3,
	sum(JanYear_2) JanYear_2, sum(FebYear_2) FebYear_2, sum(MarYear_2) MarYear_2, sum(AprYear_2) AprYear_2, sum(MayYear_2) MayYear_2, sum(JunYear_2) JunYear_2, sum(JulYear_2) JulYear_2, sum(AugYear_2) AugYear_2, sum(SepYear_2) SepYear_2, sum(OctYear_2) OctYear_2, sum(NovYear_2) NovYear_2, sum(DecYear_2) DecYear_2, sum(TotalYear_2) TotalYear_2,
	sum(JanPrevYear) JanPrevYear, sum(FebPrevYear) FebPrevYear, sum(MarPrevYear) MarPrevYear, sum(AprPrevYear) AprPrevYear, sum(MayPrevYear) MayPrevYear, sum(JunPrevYear) JunPrevYear, sum(JulPrevYear) JulPrevYear, sum(AugPrevYear) AugPrevYear, sum(SepPrevYear) SepPrevYear, sum(OctPrevYear) OctPrevYear, sum(NovPrevYear) NovPrevYear, sum(DecPrevYear) DecPrevYear, sum(TotalPrevYear) TotalPrevYear,	
	sum(JanCurYear) JanCurYear, sum(FebCurYear) FebCurYear, sum(MarCurYear) MarCurYear, sum(AprCurYear) AprCurYear, sum(MayCurYear) MayCurYear, sum(JunCurYear) JunCurYear, sum(JulCurYear) JulCurYear, sum(AugCurYear) AugCurYear, sum(SepCurYear) SepCurYear, sum(OctCurYear) OctCurYear, sum(NovCurYear) NovCurYear, sum(DecCurYear) DecCurYear, sum(TotalCurYear) TotalCurYear,		
	case when sum(PercentPrevYear) = 0 then 0 else 100 end PercentPrevYear, sum(ValCur1) ValCur1, case when sum(PercentCur1) = 0 then 0 else 100 end PercentCur1, sum(ValCur2) ValCur2, case when sum(PercentCur2) = 0 then 0 else 100 end PercentCur2,
	sum(PlusMinCurPercent) PlusMinCurPercent, 
	case when sum(ValCur1) = 0 then 0 else cast((sum(ValCur2)/cast(sum(ValCur1) as decimal(38,2))-1)*100 as decimal(5,1)) end PlusMinCurVal, 
	sum(ColumnValPrev) ColumnValPrev, case when sum(PercentPrev) = 0 then 0 else 100 end PercentPrev,
	sum(ColumnValCur) ColumnValCur, case when sum(PercentCur) = 0 then 0 else 100 end PercentCur, sum(PlusMinValCurPrev) PlusMinValCurPrev, 
	case when sum(ColumnValPrev) = 0 then 0 else cast((sum(ColumnValCur)/cast(sum(ColumnValPrev) as decimal(38,2))-1) * 100 as decimal(5,1)) end PlusMinPercentCurPrev
	from #tSummary
group by DealerCode, IsSuzukiClass) #tTotal
order by DealerCode, IsSuzukiClass desc

select temp.*
	from (select DealerCode, ReportSequence, IsSuzukiClass, ReferenceCode, BrandName, 'SUB TOTAL ' + BrandName ModelType,
	sum(JanYear_5) JanYear_5, sum(FebYear_5) FebYear_5, sum(MarYear_5) MarYear_5, sum(AprYear_5) AprYear_5, sum(MayYear_5) MayYear_5, sum(JunYear_5) JunYear_5, sum(JulYear_5) JulYear_5, sum(AugYear_5) AugYear_5, sum(SepYear_5) SepYear_5, sum(OctYear_5) OctYear_5, sum(NovYear_5) NovYear_5, sum(DecYear_5) DecYear_5, sum(TotalYear_5) TotalYear_5,
	sum(JanYear_4) JanYear_4, sum(FebYear_4) FebYear_4, sum(MarYear_4) MarYear_4, sum(AprYear_4) AprYear_4, sum(MayYear_4) MayYear_4, sum(JunYear_4) JunYear_4, sum(JulYear_4) JulYear_4, sum(AugYear_4) AugYear_4, sum(SepYear_4) SepYear_4, sum(OctYear_4) OctYear_4, sum(NovYear_4) NovYear_4, sum(DecYear_4) DecYear_4, sum(TotalYear_4) TotalYear_4,
	sum(JanYear_3) JanYear_3, sum(FebYear_3) FebYear_3, sum(MarYear_3) MarYear_3, sum(AprYear_3) AprYear_3, sum(MayYear_3) MayYear_3, sum(JunYear_3) JunYear_3, sum(JulYear_3) JulYear_3, sum(AugYear_3) AugYear_3, sum(SepYear_3) SepYear_3, sum(OctYear_3) OctYear_3, sum(NovYear_3) NovYear_3, sum(DecYear_3) DecYear_3, sum(TotalYear_3) TotalYear_3,
	sum(JanYear_2) JanYear_2, sum(FebYear_2) FebYear_2, sum(MarYear_2) MarYear_2, sum(AprYear_2) AprYear_2, sum(MayYear_2) MayYear_2, sum(JunYear_2) JunYear_2, sum(JulYear_2) JulYear_2, sum(AugYear_2) AugYear_2, sum(SepYear_2) SepYear_2, sum(OctYear_2) OctYear_2, sum(NovYear_2) NovYear_2, sum(DecYear_2) DecYear_2, sum(TotalYear_2) TotalYear_2,
	sum(JanPrevYear) JanPrevYear, sum(FebPrevYear) FebPrevYear, sum(MarPrevYear) MarPrevYear, sum(AprPrevYear) AprPrevYear, sum(MayPrevYear) MayPrevYear, sum(JunPrevYear) JunPrevYear, sum(JulPrevYear) JulPrevYear, sum(AugPrevYear) AugPrevYear, sum(SepPrevYear) SepPrevYear, sum(OctPrevYear) OctPrevYear, sum(NovPrevYear) NovPrevYear, sum(DecPrevYear) DecPrevYear, sum(TotalPrevYear) TotalPrevYear,	
	sum(JanCurYear) JanCurYear, sum(FebCurYear) FebCurYear, sum(MarCurYear) MarCurYear, sum(AprCurYear) AprCurYear, sum(MayCurYear) MayCurYear, sum(JunCurYear) JunCurYear, sum(JulCurYear) JulCurYear, sum(AugCurYear) AugCurYear, sum(SepCurYear) SepCurYear, sum(OctCurYear) OctCurYear, sum(NovCurYear) NovCurYear, sum(DecCurYear) DecCurYear, sum(TotalCurYear) TotalCurYear,		
	case when (select TotalPrevYear from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) = 0 then 0 else (TotalPrevYear / cast((select TotalPrevYear from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) as decimal(38,2))) * 100 end PercentPrevYear,
	sum(ValCur1) ValCur1, 
	case when (select ValCur1 from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) = 0 then 0 else (ValCur1 / cast((select ValCur1 from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) as decimal(38,2))) * 100 end PercentCur1, 
	sum(ValCur2) ValCur2, 
	case when (select ValCur2 from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) = 0 then 0 else (ValCur2 / cast((select ValCur2 from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) as decimal(38,2))) * 100 end PercentCur2,
	(case when (select ValCur2 from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) = 0 then 0 else (ValCur2 / cast((select ValCur2 from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) as decimal(38,2))) * 100 end) - (case when (select ValCur1 from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) = 0 then 0 else (ValCur1 / cast((select ValCur1 from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) as decimal(38,2))) * 100 end) PlusMinCurPercent, 
	sum(PlusMinCurVal) PlusMinCurVal, sum(ColumnValPrev) ColumnValPrev, 
	case when (select ColumnValPrev from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) = 0 then 0 else (ColumnValPrev / cast((select ColumnValPrev from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) as decimal(38,2))) * 100 end PercentPrev,
	sum(ColumnValCur) ColumnValCur, 
	case when (select ColumnValCur from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) = 0 then 0 else (ColumnValCur / cast((select ColumnValCur from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) as decimal(38,2))) * 100 end PercentCur, 
	(case when (select ColumnValCur from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) = 0 then 0 else (ColumnValCur / cast((select ColumnValCur from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) as decimal(38,2))) * 100 end) - (case when (select ColumnValPrev from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) = 0 then 0 else (ColumnValPrev / cast((select ColumnValPrev from #tTotal where DealerCode = #tSummary.DealerCode and IsSuzukiClass = #tSummary.IsSuzukiClass) as decimal(38,2))) * 100 end) PlusMinValCurPrev, 
	sum(PlusMinPercentCurPrev) PlusMinPercentCurPrev
	from #tSummary
group by DealerCode, ReportSequence, IsSuzukiClass, ReferenceCode, BrandName, TotalPrevYear, ValCur1, ValCur2, ColumnValPrev, ColumnValCur) temp
inner join msMstReference ref on temp.ReferenceCode = ref.ReferenceCode and ref.ReferenceType = 'SEGMENT'
order by temp.DealerCode, temp.IsSuzukiClass desc, ref.ReportSequence

select * from #tTotal

select IsSuzukiClass, (case when IsSuzukiClass = 1 then 'TOTAL SUZUKI CLASS' else 'TOTAL NON SUZUKI CLASS' end) ModelType,
	sum(JanYear_5) JanYear_5, sum(FebYear_5) FebYear_5, sum(MarYear_5) MarYear_5, sum(AprYear_5) AprYear_5, sum(MayYear_5) MayYear_5, sum(JunYear_5) JunYear_5, sum(JulYear_5) JulYear_5, sum(AugYear_5) AugYear_5, sum(SepYear_5) SepYear_5, sum(OctYear_5) OctYear_5, sum(NovYear_5) NovYear_5, sum(DecYear_5) DecYear_5, sum(TotalYear_5) TotalYear_5,
	sum(JanYear_4) JanYear_4, sum(FebYear_4) FebYear_4, sum(MarYear_4) MarYear_4, sum(AprYear_4) AprYear_4, sum(MayYear_4) MayYear_4, sum(JunYear_4) JunYear_4, sum(JulYear_4) JulYear_4, sum(AugYear_4) AugYear_4, sum(SepYear_4) SepYear_4, sum(OctYear_4) OctYear_4, sum(NovYear_4) NovYear_4, sum(DecYear_4) DecYear_4, sum(TotalYear_4) TotalYear_4,
	sum(JanYear_3) JanYear_3, sum(FebYear_3) FebYear_3, sum(MarYear_3) MarYear_3, sum(AprYear_3) AprYear_3, sum(MayYear_3) MayYear_3, sum(JunYear_3) JunYear_3, sum(JulYear_3) JulYear_3, sum(AugYear_3) AugYear_3, sum(SepYear_3) SepYear_3, sum(OctYear_3) OctYear_3, sum(NovYear_3) NovYear_3, sum(DecYear_3) DecYear_3, sum(TotalYear_3) TotalYear_3,
	sum(JanYear_2) JanYear_2, sum(FebYear_2) FebYear_2, sum(MarYear_2) MarYear_2, sum(AprYear_2) AprYear_2, sum(MayYear_2) MayYear_2, sum(JunYear_2) JunYear_2, sum(JulYear_2) JulYear_2, sum(AugYear_2) AugYear_2, sum(SepYear_2) SepYear_2, sum(OctYear_2) OctYear_2, sum(NovYear_2) NovYear_2, sum(DecYear_2) DecYear_2, sum(TotalYear_2) TotalYear_2,
	sum(JanPrevYear) JanPrevYear, sum(FebPrevYear) FebPrevYear, sum(MarPrevYear) MarPrevYear, sum(AprPrevYear) AprPrevYear, sum(MayPrevYear) MayPrevYear, sum(JunPrevYear) JunPrevYear, sum(JulPrevYear) JulPrevYear, sum(AugPrevYear) AugPrevYear, sum(SepPrevYear) SepPrevYear, sum(OctPrevYear) OctPrevYear, sum(NovPrevYear) NovPrevYear, sum(DecPrevYear) DecPrevYear, sum(TotalPrevYear) TotalPrevYear,	
	sum(JanCurYear) JanCurYear, sum(FebCurYear) FebCurYear, sum(MarCurYear) MarCurYear, sum(AprCurYear) AprCurYear, sum(MayCurYear) MayCurYear, sum(JunCurYear) JunCurYear, sum(JulCurYear) JulCurYear, sum(AugCurYear) AugCurYear, sum(SepCurYear) SepCurYear, sum(OctCurYear) OctCurYear, sum(NovCurYear) NovCurYear, sum(DecCurYear) DecCurYear, sum(TotalCurYear) TotalCurYear,		
	case when sum(PercentPrevYear) = 0 then 0 else 100 end PercentPrevYear, sum(ValCur1) ValCur1, 
	case when sum(PercentCur1) = 0 then 0 else 100 end PercentCur1, 
	sum(ValCur2) ValCur2, case when sum(PercentCur2) = 0 then 0 else 100 end PercentCur2,
	sum(PlusMinCurPercent) PlusMinCurPercent, 
	case when sum(ValCur1) = 0 then 0 else cast((sum(ValCur2)/cast(sum(ValCur1) as decimal(38,2))-1)*100 as decimal(5,1)) end PlusMinCurVal, 
	sum(ColumnValPrev) ColumnValPrev, 
	case when sum(PercentPrev) = 0 then 0 else 100 end PercentPrev, sum(ColumnValCur) ColumnValCur, 
	case when sum(PercentCur) = 0 then 0 else 100 end PercentCur, sum(PlusMinValCurPrev) PlusMinValCurPrev, 
	case when sum(ColumnValPrev) = 0 then 0 else cast((sum(ColumnValCur)/cast(sum(ColumnValPrev) as decimal(38,2))-1) * 100 as decimal(5,1)) end PlusMinPercentCurPrev
	from #tSummary
group by IsSuzukiClass
order by IsSuzukiClass desc

drop table #tSummary,#tTotal
end

