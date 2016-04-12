if object_id('SvSaView') is not null
	drop view SvSaView
GO
create view [dbo].[SvSaView]
as 
select a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName  from GnMstEmployee a
where a.TitleCode IN ('3')
   AND PersonnelStatus = '1'
GO

if object_id('usprpt_PmRpSalesAchievementWeb') is not null
	drop procedure usprpt_PmRpSalesAchievementWeb
GO
CREATE procedure [dbo].[usprpt_PmRpSalesAchievementWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BMEmployeeID		VARCHAR(15),
	@SHEmployeeID		VARCHAR(15),
	@SCEmployeeID		VARCHAR(15),
	@SMEmployeeID		VARCHAR(15),
	@Year				INT
)
AS
BEGIN
-- Get EmployeeID
--=======================================================================
--DECLARE @SalesmanID		VARCHAR(MAX);
DECLARE @SalesmanID TABLE (EmployeeID varchar(15))

if @SHEmployeeID = '' and @SMEmployeeID = ''
begin
insert into @SalesmanID select EmployeeID from HrEmployee where TeamLeader in (
			select EmployeeID from HrEmployee where TeamLeader = @BMEmployeeID)
end
else if (@SHEmployeeID != '' or @SCEmployeeID != '') and @SMEmployeeID = ''
begin
insert into @SalesmanID  select EmployeeID from HrEmployee where TeamLeader  = @SHEmployeeID
end
else
begin
insert into @SalesmanID select EmployeeID from HrEmployee where EmployeeID  = @SMEmployeeID
end
--=======================================================================
DECLARE @TeamLeadeSalesmanID TABLE( EmployeeID varchar(15))

if(@SHEmployeeID = '') and (@SCEmployeeID = '')
insert into @TeamLeadeSalesmanID  select EmployeeID from HrEmployee where TeamLeader  = @BMEmployeeID
else if (@SHEmployeeID != '') and (@SCEmployeeID = '')
insert into @TeamLeadeSalesmanID  select EmployeeID from HrEmployee where EmployeeID  = @SHEmployeeID
else if (@SHEmployeeID != '') and (@SCEmployeeID = '')
insert into @TeamLeadeSalesmanID  select EmployeeID from HrEmployee where EmployeeID  = @SCEmployeeID
--=======================================================================

select * into #TempSM from (
		select 'SM' Intial, CompanyCode, BranchCode, SpvEmployeeID, EmployeeID, isnull(Jan, 0) Jan
		, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May
		, isnull(Jun, 0) Jun, isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep
		, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec from (
			select kdp.CompanyCode, kdp.BranchCode, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth --[Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec]
				, kdp.SpvEmployeeID, kdp.EmployeeID, count(kdp.EmployeeID) InquiryCount
			from PmKDP kdp
			where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year
				--and kdp.BranchCode in (select BranchCode from #ListOfSalesman)						
				and kdp.EmployeeID in (select EmployeeID from @SalesmanID)	
				and kdp.SpvEmployeeID in (select EmployeeID from @TeamLeadeSalesmanID)					
			group by kdp.CompanyCode, kdp.BranchCode, kdp.SpvEmployeeID, kdp.EmployeeID, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)
		) as Header
		pivot(
			sum(InquiryCount)
			for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)
		) pvt
	)#TempSM 

if @SHEmployeeID = '' and @SCEmployeeID != ''
begin
	select * into #TempSC from (
			select 
			'SC' Initial, SM.CompanyCode, SM.BranchCode, a.TeamLeader SpvEmployeeID, SM.SpvEmployeeID EmployeeID
			, sum(SM.Jan) Jan, sum(SM.Feb) Feb, sum(SM.Mar) Mar
			, sum(SM.Apr) Apr, sum(SM.May) May, sum(SM.Jun) Jun
			, sum(SM.Jul) Jul, sum(SM.Aug) Aug, sum(SM.Sep) Sep
			, sum(SM.Oct) Oct, sum(SM.Nov) Nov, sum(SM.Dec) Dec
			from #TempSM SM
			inner join HrEmployee a
			on SM.CompanyCode = a.CompanyCode and SM.SpvEmployeeID = a.EmployeeID
			where SpvEmployeeID in (select EmployeeID from @TeamLeadeSalesmanID)
			group by SM.CompanyCode, SM.BranchCode, a.TeamLeader, SM.SpvEmployeeID
		) #TempSC
		
end
else
begin
	select * into #TempSH from (
		select 
		'SH' Initial, SM.CompanyCode, SM.BranchCode, a.TeamLeader SpvEmployeeID, SM.SpvEmployeeID EmployeeID
		, sum(SM.Jan) Jan, sum(SM.Feb) Feb, sum(SM.Mar) Mar
		, sum(SM.Apr) Apr, sum(SM.May) May, sum(SM.Jun) Jun
		, sum(SM.Jul) Jul, sum(SM.Aug) Aug, sum(SM.Sep) Sep
		, sum(SM.Oct) Oct, sum(SM.Nov) Nov, sum(SM.Dec) Dec
		from #TempSM SM 
		inner join HrEmployee a
			on SM.CompanyCode = a.CompanyCode and SM.SpvEmployeeID = a.EmployeeID
		where SpvEmployeeID in (select EmployeeID from @TeamLeadeSalesmanID)
		group by SM.CompanyCode, SM.BranchCode, a.TeamLeader, SM.SpvEmployeeID
	) #TempSH
	
	select * into #TempBM from (
		select 
		'BM' Initial, SM.CompanyCode, SM.BranchCode, SM.SpvEmployeeID EmployeeID
		, sum(SM.Jan) Jan, sum(SM.Feb) Feb, sum(SM.Mar) Mar
		, sum(SM.Apr) Apr, sum(SM.May) May, sum(SM.Jun) Jun
		, sum(SM.Jul) Jul, sum(SM.Aug) Aug, sum(SM.Sep) Sep
		, sum(SM.Oct) Oct, sum(SM.Nov) Nov, sum(SM.Dec) Dec
		from #TempSH SM 
		group by SM.CompanyCode, SM.BranchCode, SM.SpvEmployeeID
	) #TempBM

end

--=======================================================================================
-- SALES SOURCE OF DATA
--=======================================================================================
	select * into #SSD from (
		select CompanyCode, '' BranchCode, TypeOf1, TypeOf2
			, isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun
			, isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec
		from (
			select kdp.CompanyCode, '' BranchCode, kdp.PerolehanData TypeOf1, '' TypeOf2
				, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth, count(kdp.EmployeeID) InquiryCount
			from PmKDP kdp
			where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year
				--and kdp.BranchCode in (select BranchCode from #ListOfSalesman)						
				and kdp.EmployeeID in (select EmployeeID from @SalesmanID)	
				and kdp.SpvEmployeeID in (select EmployeeID from @TeamLeadeSalesmanID)	
			group by kdp.CompanyCode, kdp.PerolehanData, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header
		pivot
		(
			sum (inquiryCount)
			for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)
		) pvt
	) #SSD

--=======================================================================================
-- SALES BY TYPE
--=======================================================================================
	select * into #ST from (
		select CompanyCode, '' BranchCode, TypeOf1, TypeOf2
			, isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun
			, isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec
		from (
			select kdp.CompanyCode, '' BranchCode, kdp.TipeKendaraan TypeOf1, kdp.Variant TypeOf2
				, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth
				, count(kdp.EmployeeID) InquiryCount
			from PmKDP kdp
			where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year
				--and kdp.BranchCode in (select BranchCode from #ListOfSalesman)							
				and kdp.EmployeeID in (select EmployeeID from @SalesmanID)	
				and kdp.SpvEmployeeID in (select EmployeeID from @TeamLeadeSalesmanID)					
			group by kdp.CompanyCode, kdp.TipeKendaraan, kdp.Variant 
				, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header
		pivot
		(
			sum(InquiryCount)
			for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)
		) pvt
	) #ST

--=======================================================================================
-- PROSPECT STATUS
--=======================================================================================
	select * into #PS from (
		select CompanyCode, '' BranchCode, TypeOf1, TypeOf2
			, isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun
			, isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec 
		from (
			select kdp.CompanyCode, '' BranchCode, kdp.LastProgress TypeOf1, '' TypeOf2
				, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth
				, count(kdp.EmployeeID) InquiryCount
			from PmKDP kdp
			where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year	
				--and kdp.BranchCode in (select BranchCode from #ListOfSalesman)						
				and kdp.EmployeeID in (select EmployeeID from @SalesmanID)	
				and kdp.SpvEmployeeID in (select EmployeeID from @TeamLeadeSalesmanID)							
			group by kdp.CompanyCode, kdp.BranchCode, kdp.LastProgress
				, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header
		pivot
		(
			sum(InquiryCount)
			for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)
		) pvt
	) #PS
	
	-- ===================================================================
-- Collect all Service Achievement by salesman
-- ===================================================================
	-- Salesman ----------
	select 
		inc, initial, CompanyCode, BranchCode, EmployeeID, EmployeeName
		, case cast(Jan as varchar) when '0' then '-' else cast(Jan as varchar) end Jan
		, case cast(Feb as varchar) when '0' then '-' else cast(Feb as varchar) end Feb
		, case cast(Mar as varchar) when '0' then '-' else cast(Mar as varchar) end Mar
		, case cast(Apr as varchar) when '0' then '-' else cast(Apr as varchar) end Apr
		, case cast(May as varchar) when '0' then '-' else cast(May as varchar) end May
		, case cast(Jun as varchar) when '0' then '-' else cast(Jun as varchar) end Jun
		, case cast(Jul as varchar) when '0' then '-' else cast(Jul as varchar) end Jul
		, case cast(Aug as varchar) when '0' then '-' else cast(Aug as varchar) end Aug
		, case cast(Sep as varchar) when '0' then '-' else cast(Sep as varchar) end Sep
		, case cast(Oct as varchar) when '0' then '-' else cast(Oct as varchar) end Oct
		, case cast(Nov as varchar) when '0' then '-' else cast(Nov as varchar) end Nov
		, case cast(Dec as varchar) when '0' then '-' else cast(Dec as varchar) end Dec
		, case cast(Sem1 as varchar) when '0' then '-' else cast(Sem1 as varchar) end Sem1
		, case cast(Sem2 as varchar) when '0' then '-' else cast(Sem2 as varchar) end Sem2
		, case cast(Total as varchar) when '0' then '-' else cast(Total as varchar) end Total
	from (
		select  
			'1' inc, 'Salesman' initial, tempSM.CompanyCode, tempSM.BranchCode, tempSM.EmployeeID
			, emp.EmployeeName, tempSM.Jan, tempSM.Feb, tempSM.Mar, tempSM.Apr, tempSM.May, tempSM.Jun
			, tempSM.Jul, tempSM.Aug, tempSM.Sep, tempSM.Oct, tempSM.Nov, tempSM.Dec
			, (tempSM.Jan + tempSM.Feb + tempSM.Mar + tempSM.Apr + tempSM.May + tempSM.Jun) Sem1
			, (tempSM.Jul + tempSM.Aug + tempSM.Sep + tempSM.Oct + tempSM.Nov+ tempSM.Dec) Sem2
			, (tempSM.Jan + tempSM.Feb + tempSM.Mar + tempSM.Apr + tempSM.May + tempSM.Jun
				+ tempSM.Jul + tempSM.Aug + tempSM.Sep + tempSM.Oct + tempSM.Nov
				+ tempSM.Dec) Total
		from 
			#TempSM tempSM
				left join GnMstEmployee emp on tempSM.CompanyCode = emp.CompanyCode
					and tempSM.BranchCode = emp.BranchCode and tempSM.EmployeeID = emp.EmployeeID
		
		union
		-- Sales Head -------	
		select  
			'2' inc, 'Sales Head' initial, TempSH.CompanyCode, TempSH.BranchCode, TempSH.EmployeeID, emp.EmployeeName
			, TempSH.Jan, TempSH.Feb, TempSH.Mar, TempSH.Apr, TempSH.May, TempSH.Jun
			, TempSH.Jul, TempSH.Aug, TempSH.Sep, TempSH.Oct, TempSH.Nov, TempSH.Dec
			, (TempSH.Jan + TempSH.Feb + TempSH.Mar + TempSH.Apr + TempSH.May + TempSH.Jun) Sem1
			, (TempSH.Jul + TempSH.Aug + TempSH.Sep + TempSH.Oct + TempSH.Nov+ TempSH.Dec) Sem2
			, (TempSH.Jan + TempSH.Feb + TempSH.Mar + TempSH.Apr + TempSH.May + TempSH.Jun
				+ TempSH.Jul + TempSH.Aug + TempSH.Sep + TempSH.Oct + TempSH.Nov
				+ TempSH.Dec) Total
		from 
			#TempSH TempSH
				left join GnMstEmployee emp on TempSH.CompanyCode = emp.CompanyCode
					and TempSH.BranchCode = emp.BranchCode and TempSH.EmployeeID = emp.EmployeeID
		
		union
		-- Branch Manager -------
		select  
			'3' inc, 'Branch Manager' initial, TempBM.CompanyCode, TempBM.BranchCode, TempBM.EmployeeID, emp.EmployeeName
			, TempBM.Jan, TempBM.Feb, TempBM.Mar, TempBM.Apr, TempBM.May, TempBM.Jun
			, TempBM.Jul, TempBM.Aug, TempBM.Sep, TempBM.Oct, TempBM.Nov, TempBM.Dec
			, (TempBM.Jan + TempBM.Feb + TempBM.Mar + TempBM.Apr + TempBM.May + TempBM.Jun) Sem1
			, (TempBM.Jul + TempBM.Aug + TempBM.Sep + TempBM.Oct + TempBM.Nov+ TempBM.Dec) Sem2
			, (TempBM.Jan + TempBM.Feb + TempBM.Mar + TempBM.Apr + TempBM.May + TempBM.Jun
				+ TempBM.Jul + TempBM.Aug + TempBM.Sep + TempBM.Oct + TempBM.Nov
				+ TempBM.Dec) Total
		from 
			#TempBM TempBM
				left join GnMstEmployee emp on TempBM.CompanyCode = emp.CompanyCode
					and TempBM.BranchCode = emp.BranchCode and TempBM.EmployeeID = emp.EmployeeID
	) SASalesman order by SASalesman.inc--, SASalesman.EmployeeName

-- ===================================================================
-- Collect all Service Achievement by Types
-- ===================================================================
	select 
		inc, initial, CompanyCode, BranchCode, TypeOf1, TypeOf2
		, case cast(Jan as varchar) when '0' then '-' else cast(Jan as varchar) end Jan
		, case cast(Feb as varchar) when '0' then '-' else cast(Feb as varchar) end Feb
		, case cast(Mar as varchar) when '0' then '-' else cast(Mar as varchar) end Mar
		, case cast(Apr as varchar) when '0' then '-' else cast(Apr as varchar) end Apr
		, case cast(May as varchar) when '0' then '-' else cast(May as varchar) end May
		, case cast(Jun as varchar) when '0' then '-' else cast(Jun as varchar) end Jun
		, case cast(Jul as varchar) when '0' then '-' else cast(Jul as varchar) end Jul
		, case cast(Aug as varchar) when '0' then '-' else cast(Aug as varchar) end Aug
		, case cast(Sep as varchar) when '0' then '-' else cast(Sep as varchar) end Sep
		, case cast(Oct as varchar) when '0' then '-' else cast(Oct as varchar) end Oct
		, case cast(Nov as varchar) when '0' then '-' else cast(Nov as varchar) end Nov
		, case cast(Dec as varchar) when '0' then '-' else cast(Dec as varchar) end Dec
		, case cast(Sem1 as varchar) when '0' then '-' else cast(Sem1 as varchar) end Sem1
		, case cast(Sem2 as varchar) when '0' then '-' else cast(Sem2 as varchar) end Sem2
		, case cast(Total as varchar) when '0' then '-' else cast(Total as varchar) end Total
	from (
		-- Sales Source of Data ----
		select  
			 '1' inc, 'Sales Source of Data' initial, tempSSD.CompanyCode, '' BranchCode, lkpDtl.LookUpValue TypeOf1, tempSSD.TypeOf2
			, isnull(tempSSD.Jan, 0) Jan, isnull(tempSSD.Feb, 0) Feb, isnull(tempSSD.Mar, 0) Mar
			, isnull(tempSSD.Apr, 0) Apr, isnull(tempSSD.May, 0) May, isnull(tempSSD.Jun, 0) Jun
			, isnull(tempSSD.Jul, 0) Jul, isnull(tempSSD.Aug, 0) Aug, isnull(tempSSD.Sep, 0) Sep
			, isnull(tempSSD.Oct, 0) Oct, isnull(tempSSD.Nov, 0) Nov, isnull(tempSSD.Dec, 0) Dec
			, isnull((tempSSD.Jan + tempSSD.Feb + tempSSD.Mar + tempSSD.Apr + tempSSD.May + tempSSD.Jun), 0) Sem1
			, isnull((tempSSD.Jul + tempSSD.Aug + tempSSD.Sep + tempSSD.Oct + tempSSD.Nov+ tempSSD.Dec), 0) Sem2
			, isnull((tempSSD.Jan + tempSSD.Feb + tempSSD.Mar + tempSSD.Apr + tempSSD.May + tempSSD.Jun
				+ tempSSD.Jul + tempSSD.Aug + tempSSD.Sep + tempSSD.Oct + tempSSD.Nov
				+ tempSSD.Dec), 0) Total
		from 
			GnMstLookUpDtl lkpDtl
				left join #SSD tempSSD on lkpDtl.CompanyCode = tempSSD.CompanyCode
				and lkpDtl.LookUpValue = tempSSD.TypeOf1 
		where 
			lkpDtl.CodeID = 'PSRC' and lkpDtl.CompanyCode = @CompanyCode

		union
		-- Sales by Type ----
		select  
			'2' inc, 'Sales by Type' initial, tempST.CompanyCode, tempST.BranchCode, GTS.GroupCode, GTS.typeCode-- tempST.TypeOf1, tempST.TypeOf2
			, isnull(tempST.Jan, 0) Jan, isnull(tempST.Feb, 0) Feb, isnull(tempST.Mar, 0) Mar
			, isnull(tempST.Apr, 0) Apr, isnull(tempST.May, 0) May, isnull(tempST.Jun, 0) Jun
			, isnull(tempST.Jul, 0) Jul, isnull(tempST.Aug, 0) Aug, isnull(tempST.Sep, 0) Sep
			, isnull(tempST.Oct, 0) Oct, isnull(tempST.Nov, 0) Nov, isnull(tempST.Dec, 0) Dec
			, isnull((tempST.Jan + tempST.Feb + tempST.Mar + tempST.Apr + tempST.May + tempST.Jun), 0) Sem1
			, isnull((tempST.Jul + tempST.Aug + tempST.Sep + tempST.Oct + tempST.Nov+ tempST.Dec), 0) Sem2
			, isnull((tempST.Jan + tempST.Feb + tempST.Mar + tempST.Apr + tempST.May + tempST.Jun
				+ tempST.Jul + tempST.Aug + tempST.Sep + tempST.Oct + tempST.Nov
				+ tempST.Dec), 0) Total
		from 
			(select Distinct CompanyCode, GroupCode, TypeCode  
			from pmGroupTypeSeq 
			group by CompanyCode, GroupCode ,typeCode) GTS
				left join #ST tempST on GTS.CompanyCode = tempST.CompanyCode and GTS.GroupCode = tempST.TypeOf1 and GTS.TypeCode = tempST.TypeOf2
					

		union
		-- Prospect Status ----
		select 
			'3' inc, 'Prospect Status' initial, tempPS.CompanyCode, tempPS.BranchCode, lkpDtl.LookUpValueName TypeOf1, tempPS.TypeOf2
			, isnull(tempPS.Jan, 0) Jan, isnull(tempPS.Feb, 0) Feb, isnull(tempPS.Mar, 0) Mar
			, isnull(tempPS.Apr, 0) Apr, isnull(tempPS.May, 0) May, isnull(tempPS.Jun, 0) Jun
			, isnull(tempPS.Jul, 0) Jul, isnull(tempPS.Aug, 0) Aug, isnull(tempPS.Sep, 0) Sep
			, isnull(tempPS.Oct, 0) Oct, isnull(tempPS.Nov, 0) Nov, isnull(tempPS.Dec, 0) Dec
			, isnull((tempPS.Jan + tempPS.Feb + tempPS.Mar + tempPS.Apr + tempPS.May + tempPS.Jun), 0) Sem1
			, isnull((tempPS.Jul + tempPS.Aug + tempPS.Sep + tempPS.Oct + tempPS.Nov+ tempPS.Dec), 0) Sem2
			, isnull((tempPS.Jan + tempPS.Feb + tempPS.Mar + tempPS.Apr + tempPS.May + tempPS.Jun
				+ tempPS.Jul + tempPS.Aug + tempPS.Sep + tempPS.Oct + tempPS.Nov
				+ tempPS.Dec), 0) Total
		from
			GnMstLookUpDtl lkpDtl
				left join #PS tempPS on lkpDtl.CompanyCode = tempPS.CompanyCode
				and lkpDtl.LookUpValue = tempPS.TypeOf1 
		where lkpDtl.CodeID = 'PSTS' and lkpDtl.CompanyCode = @CompanyCode
	) SATypeOf order by SATypeOf.inc, SATypeOf.TypeOf1
	
	
	Select * from #TempSM	
	Select * from #TempBM
	Select * from #TempSH
	select * from #SSD
	select * from #ST
	select * from #PS

	drop table #TempSM
	drop table #TempBM
	drop table #TempSH
	drop table #SSD
	drop table #ST
	drop table #PS
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usprpt_PmRpInqOutStanding_NewByData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usprpt_PmRpInqOutStanding_NewByData]
GO

CREATE procedure [dbo].[usprpt_PmRpInqOutStanding_NewByData] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@Period				DATETIME,
	@COO				VARCHAR(15),
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@SalesCoordinator	VARCHAR(15),
	@Salesman			VARCHAR(15)
	
)
AS 
BEGIN
SET NOCOUNT ON;
SELECT * INTO #dByTipe FROM(
			SELECT b.EmployeeID, b.PerolehanData, LastProgress, StatusProspek FROM PmKdp b 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.PerolehanData Source,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) SPK
			FROM #dByTipe a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.PerolehanData
		)#dSls

		IF @COO = ''
		BEGIN
		IF @SalesHead = '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData Source,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData
		END
		ELSE IF @SalesHead <> '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData Source,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader = @SalesHead
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData
		END
		ELSE IF @SalesHead = '' AND @SalesCoordinator <> '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData Source,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader = @SalesCoordinator
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData	
		END
		ELSE IF (@SalesHead <> '' OR @SalesCoordinator <> '') AND @Salesman <> ''
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.EmployeeID = @Salesman  
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData

		DROP TABLE #dSls
		DROP TABLE #dByTipe
		END
END

GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usprpt_PmRpInqOutStanding_NewByType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usprpt_PmRpInqOutStanding_NewByType]
GO

CREATE procedure [dbo].[usprpt_PmRpInqOutStanding_NewByType] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@Period				DATETIME,
	@COO				VARCHAR(15),
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@SalesCoordinator	VARCHAR(15),
	@Salesman			VARCHAR(15)
	
)
AS 
BEGIN
SET NOCOUNT ON;
SELECT * INTO #dByTipe FROM(
			SELECT b.EmployeeID, (b.TipeKendaraan + ' ' + b.Variant) ModelKendaraan, LastProgress, StatusProspek FROM PmKdp b 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.ModelKendaraan,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) SPK
			FROM #dByTipe a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.ModelKendaraan
		)#dSls

		IF @COO = ''
		BEGIN
		IF @SalesHead = '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.ModelKendaraan TipeKendaraan,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))  
				AND a.ModelKendaraan <> ''
				GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan
		END
		ELSE IF @SalesHead <> '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.ModelKendaraan TipeKendaraan,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader = @SalesHead
					AND a.ModelKendaraan <> ''
				GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan	
		END
		ELSE IF @SalesHead = '' AND @SalesCoordinator <> '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.ModelKendaraan TipeKendaraan,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader = @SalesCoordinator
					AND a.ModelKendaraan <> ''
				GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan	
		END
		ELSE IF (@SalesHead <> '' OR @SalesCoordinator <> '') AND @Salesman <> ''
			SELECT
					a.EmployeeID,
					a.Position,
					a.ModelKendaraan TipeKendaraan,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.EmployeeID = @Salesman
					AND a.ModelKendaraan <> ''   
				GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan

		DROP TABLE #dSls
		DROP TABLE #dByTipe
		END
END

GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usprpt_PmRpInqOutStanding_NewBySalesman]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usprpt_PmRpInqOutStanding_NewBySalesman]
GO

CREATE procedure [dbo].[usprpt_PmRpInqOutStanding_NewBySalesman] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@Period				DATETIME,
	@COO				VARCHAR(15),
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@SalesCoordinator	VARCHAR(15),
	@Salesman			VARCHAR(15)
	
)
AS 
BEGIN
SET NOCOUNT ON;

-- TABLE INITIAL
--===============================================================================================================================
	SELECT * INTO #employee_stat_SM FROM(
		SELECT 
			a.CompanyCode,
			a.EmployeeID,
			a.EmployeeName,
			a.Position,
			'Salesman' PositionName, 
			a.TeamLeader,
			(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastProgress = 'P' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) PROSPECT,
			(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastProgress = 'HP' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) HOTPROSPECT,
			(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastProgress = 'SPK' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) SPK
		FROM HrEmployee a
		WHERE a.CompanyCode = @CompanyCode  AND a.Department = 'SALES'
			AND a.TeamLeader IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @BranchManager)
	)#employee_stat_SM

	SELECT * INTO #employee_stat_SK FROM(
		SELECT
			a.CompanyCode,
			a.TeamLeader EmployeeID,
			b.EmployeeName,
			'SC' Position,
			'Sales Coordinator' PositionName, 
			b.TeamLeader ShEmployeeID,
			ISNULL(SUM(a.PROSPECT),0) PROSPECT,
			ISNULL(SUM(a.HOTPROSPECT),0) HOTPROSPECT,
			ISNULL(SUM(a.SPK),0) SPK
		FROM #employee_stat_SM a
		LEFT JOIN HrEmployee b
			ON b.CompanyCode = a.CompanyCode AND a.TeamLeader = b.EmployeeID
		WHERE b.TeamLeader  = @BranchManager
		GROUP BY a.CompanyCode,
			b.EmployeeName,
			a.TeamLeader, b.TeamLeader
	)#employee_stat_SK

	SELECT * INTO #employee_stat_SH FROM(
		SELECT
			a.CompanyCode,
			a.TeamLeader EmployeeID,
			b.EmployeeName,
			'SH' PositionID,
			'Sales Head' PositionName, 
			b.TeamLeader BMEmployeeID,
			ISNULL(SUM(a.PROSPECT),0) PROSPECT,
			ISNULL(SUM(a.HOTPROSPECT),0) HOTPROSPECT,
			ISNULL(SUM(a.SPK),0) SPK
		FROM #employee_stat_SM a
		LEFT JOIN HrEmployee b
			ON b.CompanyCode = a.CompanyCode AND a.TeamLeader = b.EmployeeID
		WHERE b.TeamLeader = @BranchManager
		GROUP BY a.CompanyCode,
			b.EmployeeName,
			a.TeamLeader, b.TeamLeader
	)#employee_stat_SH

	SELECT * INTO #employee_stat_BM FROM(
		SELECT
			a.CompanyCode,
			a.BMEmployeeID EmployeeID,
			b.EmployeeName,
			'BM' PositionID,
			'Branch Manager' PositionName, 
			'' TeamLeader,
			ISNULL(SUM(a.PROSPECT),0) PROSPECT,
			ISNULL(SUM(a.HOTPROSPECT),0) HOTPROSPECT,
			ISNULL(SUM(a.SPK),0) SPK
		FROM #employee_stat_SH a
		LEFT JOIN HrEmployee b
			ON b.CompanyCode = a.CompanyCode AND a.BMEmployeeID = b.EmployeeID
		WHERE a.BMEmployeeID = @BranchManager
		GROUP BY a.CompanyCode,
			b.EmployeeName,
			a.BMEmployeeID
	)#employee_stat_BM

SELECT * INTO #employee_stat FROM(
	SELECT '3' PositionId, a.* FROM #employee_stat_SM a
	UNION
	SELECT '2' PositionId, a.* FROM #employee_stat_SH a
	UNION
	SELECT '1' PositionId, a.* FROM #employee_stat_BM a
	) #employee_stat

	DROP TABLE #employee_stat_SM
	DROP TABLE #employee_stat_SK
	DROP TABLE #employee_stat_SH
	DROP TABLE #employee_stat_BM

IF @COO = ''
	BEGIN
-- == CASE I ==
		IF @SalesHead = '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN		
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode
		END
-- == CASE II ==
		ELSE IF @SalesHead <> '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @BranchManager
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesHead
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @SalesHead)
		END
-- == CASE III ==
		ELSE IF @SalesHead = '' AND @SalesCoordinator <> '' AND @Salesman = ''
		BEGIN
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @BranchManager
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesCoordinator
			UNION			
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @SalesCoordinator)
		END
-- == CASE IV ==
		ELSE IF (@SalesHead <> '' OR @SalesCoordinator <> '') AND @Salesman <> ''
		BEGIN
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @BranchManager
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesHead
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesCoordinator
			UNION			
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @Salesman
		END
	END
END

GO

if object_id('usprpt_PmRpInqSummaryWeb') is not null
	drop procedure usprpt_PmRpInqSummaryWeb
GO
CREATE procedure [dbo].[usprpt_PmRpInqSummaryWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@PeriodBegin		DATETIME,
	@PeriodEnd			DATETIME,
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@Salesman			VARCHAR(15),
	@Jns				VARCHAR(1)
	
)
AS 
BEGIN
SET NOCOUNT ON;
declare @position varchar(20), @SC varchar(20)
set @position= (
				select position 
				from HrEmployee 
				where employeeid=(select TeamLeader from HrEmployee where EmployeeID = @salesman) 
				)
set @SC= (select TeamLeader from HrEmployee where EmployeeID = @salesman)
IF @Jns = '1'
BEGIN
	SELECT * INTO #deptSales FROM(
		SELECT 
				'4' idx,
			   a.EmployeeID,
			   a.Position,
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.StatusProspek = '10' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) NEW,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'P' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'HP' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'SPK' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) SPK,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'DO' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) DO,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'DELIVERY' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'LOST' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) LOST
		FROM HrEmployee a WHERE a.Department = 'SALES' AND a.CompanyCode = @CompanyCode
	)#deptSales

	--Sales Coordinator
	SELECT * INTO #qryS_SC FROM(
		SELECT 
				'3' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) LOST
		FROM #deptSales a WHERE a.Position = 'SC'
	)#qryS_SC

	--Sales Head
	SELECT * INTO #qrySH FROM(
		SELECT 
				'2' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
			   (SELECT ISNULL(SUM(NEW), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') NEW,
			   (SELECT ISNULL(SUM(PROSPECT), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') PROSPECT,
			   (SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') HOTPROSPECT,
			   (SELECT ISNULL(SUM(SPK), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') SPK,
			   (SELECT ISNULL(SUM(DO), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DO,
			   (SELECT ISNULL(SUM(DELIVERY), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DELIVERY,
			   (SELECT ISNULL(SUM(LOST), 0) FROM #qryS_SC b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') LOST
		FROM #deptSales a WHERE a.Position = 'SH'
	)#qrySH

	IF(@SalesHead = '' AND @Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM FROM(
				SELECT 
					'1' idx,
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSales a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM

			SELECT * INTO #qryAll FROM(
				SELECT * FROM #qryBM
				UNION
				SELECT * FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM)     
				UNION
				SELECT * FROM #qryS_SC WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM)) 
				UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN (SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM))) 
			)#qryAll

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAll a ORDER BY a.idx ASC
		
			DROP TABLE #qryAll
			DROP TABLE #qryBM
		END
	ELSE IF(@Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM2 FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSales a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM2

			SELECT * INTO #qryAll2 FROM(
				SELECT * FROM #qryBM2
				UNION
				SELECT * FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead     
				UNION
				SELECT * FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead) 
				UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead )) 
			)#qryAll2

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAll2 a ORDER BY a.idx ASC

			DROP TABLE #qryAll2
			DROP TABLE #qryBM2
		END
	ELSE
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM3 FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSales a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM3

			SELECT * INTO #qryAll3 FROM(
				SELECT * FROM #qryBM3
				UNION
				SELECT * FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead     
				UNION
				SELECT * FROM #qryS_SC WHERE EmployeeID IN (@SC)
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead) 
				UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead ))
				AND EmployeeID = @Salesman 
			)#qryAll3

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST 
			FROM #qryAll3 a ORDER BY a.idx ASC 

			DROP TABLE #qryAll3
			DROP TABLE #qryBM3
		END
		DROP TABLE #deptSales
		DROP TABLE #qryS_SC
		DROP TABLE #qrySH
	END
ELSE IF @Jns = '2'
	BEGIN
		SELECT * INTO #dByTipe FROM(
			SELECT b.EmployeeID, (b.TipeKendaraan + ' ' + b.Variant) ModelKendaraan, LastProgress, StatusProspek FROM PmKdp b 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd)
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.ModelKendaraan,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.StatusProspek = '10' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) LOST
			FROM #dByTipe a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.ModelKendaraan
		)#dSls

		--Kondisi SH = '' AND S = ''
		IF (@SalesHead = '' AND @Salesman = '')
		BEGIN
			SELECT * INTO #dt27_1 FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.ModelKendaraan
			)#dt27_1

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_1 ORDER BY ModelKendaraan

			DROP TABLE #dt27_1
		END
		--Kondisi S = ''
		ELSE IF (@Salesman = '')
		BEGIN
			SELECT * INTO #dt27_2 FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.ModelKendaraan
			)#dt27_2

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_2 ORDER BY ModelKendaraan

			DROP TABLE #dt27_2
		END
		ELSE
		BEGIN
			SELECT * INTO #dt27_3 FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.ModelKendaraan
			)#dt27_3

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_3 ORDER BY ModelKendaraan

			DROP TABLE #dt27_3

		DROP TABLE #dSls
		DROP TABLE #dByTipe
		END
	END
ELSE IF @Jns = '3'
	BEGIN
		SELECT * INTO #dByTipe2 FROM(
			SELECT b.EmployeeID, b.PerolehanData, LastProgress, StatusProspek FROM PmKdp b 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd)
		)#dByTipe2

		SELECT * INTO #dSls2 FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.PerolehanData,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.StatusProspek <> '20' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) LOST
			FROM #dByTipe2 a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.PerolehanData
		)#dSls2

		--Kondisi SH = '' AND S = ''
		IF (@SalesHead = '' AND @Salesman = '')
		BEGIN
			SELECT * INTO #dt37_1 FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSls2 a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.PerolehanData
			)#dt37_1

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_1 ORDER BY PerolehanData

			DROP TABLE #dt37_1
		END
		--Kondisi S = ''
		ELSE IF (@Salesman = '')
		BEGIN
			SELECT * INTO #dt37_2 FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2 a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.PerolehanData
			)#dt37_2

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_2 ORDER BY PerolehanData

			DROP TABLE #dt37_2
		END
		ELSE
		begin
			SELECT * INTO #dt37_3 FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2 a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.PerolehanData
			)#dt37_3

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_3 ORDER BY PerolehanData

			DROP TABLE #dt37_3

		DROP TABLE #dSls2
		DROP TABLE #dByTipe2
		end
	END
END
GO

INSERT INTO SysReport
VALUES ('PmRpInqLostCaseWeb','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqLostCaseRpt','SP','usprpt_PmRpInqLostCaseWeb','PMLetter1','LOST CASE','',	NULL)

INSERT INTO SysReport
VALUES ('PmRpInqFollowUpDet2015','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqFollowUpDet2012Rpt','SP','usprpt_PmRpInqFollowUpDtlNew','A4C','PROSPECT FOLLOW UP LIST DETAIL',NULL,	NULL)

INSERT INTO SysReport
VALUES ('PmRpInqSalesAchievementWeb','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqSalesAchievementRpt','SP','usprpt_PmRpSalesAchievementWeb','PMLetter1','SALES ACHIEVEMENT','',NULL)

INSERT INTO SysReport
VALUES ('PmRpInqSummaryWeb','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqSummaryRpt','SP','usprpt_PmRpInqSummaryWeb','SPLTP','INQUIRY SUMMARY','',NULL)

INSERT INTO SysReport
VALUES ('PmRpInqOutStandingNew','Isi.Dms.Report.PreSales,ReportSource.Inquiry.PmRpInqOutStandingRpt','SP','usprpt_PmRpInqOutStanding_NewPrint','SPLTP','INQUIRY OUTSTANDING PROSPECT','',NULL)
GO

if object_id('usprpt_PmRpInqPeriodeWeb') is not null
	drop procedure usprpt_PmRpInqPeriodeWeb
GO
create procedure [dbo].[usprpt_PmRpInqPeriodeWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@PeriodBegin		DATETIME,
	@PeriodEnd			DATETIME,
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@Salesman			VARCHAR(15)
)
AS 
BEGIN
SET NOCOUNT ON;
----

IF(@SalesHead ='' AND @Salesman ='')
BEGIN
	SELECT * INTO #empl1 FROM (
		--SH =ALL AND S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  
		a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader=@BranchManager)
	)#empl1

	SELECT * INTO #t1 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND a.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl1 g)
	) #t1

	DROP TABLE #empl1
	SELECT InquiryNumber, Pelanggan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
	Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail FROM #t1 ORDER BY InquiryNumber
	DROP TABLE #t1

END
ELSE IF(@Salesman = '')
BEGIN
	SELECT * INTO #empl2 FROM (
		--S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  
		a.TeamLeader =@SalesHead
	)#empl2

	SELECT * INTO #t2 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND a.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl2 g)
	) #t2

	DROP TABLE #empl2
	SELECT InquiryNumber, Pelanggan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
	Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail FROM #t2 ORDER BY InquiryNumber
	DROP TABLE #t2
END
ELSE
BEGIN
	SELECT * INTO #empl3 FROM (
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  
		a.EmployeeID=@Salesman
	)#empl3

	SELECT * INTO #t3 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND a.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl3 g)
	) #t3

	DROP TABLE #empl3
	SELECT InquiryNumber, Pelanggan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
	Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail FROM #t3 ORDER BY InquiryNumber
	DROP TABLE #t3
END
----
END

GO

if object_id('sp_MaintainAvgCostItem') is not null
	drop procedure sp_MaintainAvgCostItem
GO
CREATE procedure [dbo].[sp_MaintainAvgCostItem] (  

@CompanyCode varchar(10),
@BranchCode varchar(10),
@ProductType varchar(10),
@PartNo varchar (20),
@Option varchar (2)
)


as

IF @Option = 'A'
BEGIN

SELECT TOP 1500
 Items.PartNo
,Items.ProductType
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
   WHERE CodeID = 'PRCT' AND 
         LookUpValue = Items.PartCategory AND 
         CompanyCode = @CompanyCode) AS CategoryName
,Items.PartCategory
,ItemInfo.PartName
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
,CASE Items.Status WHEN 1 THEN 'Aktif' ELSE 'Tidak' END AS IsActive
,ItemInfo.OrderUnit
,ItemInfo.SupplierCode
,Supplier.SupplierName
, Items.TypeOfGoods TipePart
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'TPGO' AND 
        LookUpValue = Items.TypeOfGoods AND 
        CompanyCode = @CompanyCode) AS TypeOfGoods
,ISNULL(ItemLoc.WarehouseCode,0) WarehouseCode
,ISNULL(ItemLoc.LocationCode,0) LocationCode
,(ISNULL(ItemLoc.OnHand,0) - (ISNULL(ItemLoc.AllocationSP,0) + ISNULL(ItemLoc.AllocationSR,0) + ISNULL(ItemLoc.AllocationSL,0) + ISNULL(ItemLoc.ReservedSP,0) + ISNULL(ItemLoc.ReservedSR,0) + ISNULL(ItemLoc.ReservedSL,0))) AS QtyAvail
,ISNULL(ItemPrice.RetailPrice,0) RetailPrice
,ISNULL(ItemPrice.RetailPriceInclTax,0) RetailPriceInclTax
FROM SpMstItems Items
LEFT JOIN SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                         AND Items.PartNo = ItemInfo.PartNo
LEFT JOIN SpMstItemLoc ItemLoc ON Items.CompanyCode  = ItemLoc.CompanyCode
	AND Items.BranchCode = ItemLoc.BranchCode	
	AND Items.PartNo = ItemLoc.PartNo
LEFT JOIN SpMstItemPrice ItemPrice ON Items.CompanyCode  = ItemPrice.CompanyCode
	AND Items.BranchCode = ItemPrice.BranchCode	
	AND Items.PartNo = ItemPrice.PartNo		 
LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
                         AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE Items.CompanyCode = @CompanyCode
  AND Items.BranchCode  = @BranchCode    
  AND Items.ProductType = @ProductType
  AND ItemLoc.WarehouseCode = '00'
END
ELSE
BEGIN

SELECT TOP 1500
 Items.PartNo
,Items.ProductType
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
   WHERE CodeID = 'PRCT' AND 
         LookUpValue = Items.PartCategory AND 
         CompanyCode = @CompanyCode) AS CategoryName
,Items.PartCategory
,ItemInfo.PartName
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
,CASE Items.Status WHEN 1 THEN 'Aktif' ELSE 'Tidak' END AS IsActive
,ItemInfo.OrderUnit
,Items.Onhand
,ItemInfo.SupplierCode
,Supplier.SupplierName
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'TPGO' AND 
        LookUpValue = Items.TypeOfGoods AND 
        CompanyCode = @CompanyCode) AS TypeOfGoods
,ItemLoc.WarehouseCode
,ItemLoc.LocationCode
,(ItemLoc.OnHand - (ItemLoc.AllocationSP + ItemLoc.AllocationSR + ItemLoc.AllocationSL + ItemLoc.ReservedSP + ItemLoc.ReservedSR + ItemLoc.ReservedSL)) AS QtyAvail
,ItemPrice.RetailPrice
,ItemPrice.CostPrice
,ItemPrice.RetailPriceInclTax
FROM SpMstItems Items with (nolock, nowait)
LEFT JOIN SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                         AND Items.PartNo = ItemInfo.PartNo
LEFT JOIN SpMstItemLoc ItemLoc ON Items.CompanyCode  = ItemLoc.CompanyCode
	AND Items.BranchCode = ItemLoc.BranchCode	
	AND Items.PartNo = ItemLoc.PartNo
LEFT JOIN SpMstItemPrice ItemPrice ON Items.CompanyCode  = ItemPrice.CompanyCode
	AND Items.BranchCode = ItemPrice.BranchCode	
	AND Items.PartNo = ItemPrice.PartNo		 
LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
                         AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE Items.CompanyCode = @CompanyCode
  AND Items.BranchCode  = @BranchCode    
  AND Items.ProductType = @ProductType
  AND Items.PartNo      = @PartNo
  AND ItemLoc.WarehouseCode = '00'
  END

GO

if object_id('uspfn_SvTrnServiceSelectBookingData') is not null
	drop procedure uspfn_SvTrnServiceSelectBookingData
GO
create procedure [dbo].[uspfn_SvTrnServiceSelectBookingData]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType  varchar(15),
	@ShowAll bit,
	@JobOrderNo varchar(20),
	@PoliceRegNo  varchar(20),
	@CustomerName varchar(200),
	@ServiceBookNo  varchar(20)
AS

declare @Query varchar(max), @Filter varchar(max)=''
declare @Condition varchar(4000);

set @Condition = ' ORDER BY svTrnService.BookingNo DESC';
if(@ShowAll = 0) begin
	set @Condition = ' AND svTrnService.ServiceStatus IN (0,1,2,3,4,5) ORDER BY svTrnService.BookingNo DESC';
	if(@JobOrderNo <> '') begin 
		set @Filter = @Filter + ' And BookingNo like ''%'+@JobOrderNo+'%'' '
	end
	if(@PoliceRegNo <> '') begin 
		set @Filter = @Filter + ' And svTrnService.PoliceRegNo like ''%'+@PoliceRegNo+'%'' '
	end
	if(@ServiceBookNo <> '' ) begin 
		set @Filter = @Filter + ' And ServiceBookNo like ''%'+@ServiceBookNo+'%'' '
	end
	if(@CustomerName <> '') begin 
		set @Filter = @Filter + ' And gnMstCustomer.CustomerName like ''%'+@CustomerName+'%'''
	end
end 

if(@ShowAll = 1) begin
	if(@JobOrderNo <> '') begin 
		set @Filter = @Filter + ' And BookingNo like ''%'+@JobOrderNo+'%'' '
	end
	if(@PoliceRegNo <> '') begin 
		set @Filter = @Filter + ' And svTrnService.PoliceRegNo like ''%'+@PoliceRegNo+'%'' '
	end
	if(@ServiceBookNo <> '' ) begin 
		set @Filter = @Filter + ' And ServiceBookNo like ''%'+@ServiceBookNo+'%'' '
	end
	if(@CustomerName <> '') begin 
		set @Filter = @Filter + ' And gnMstCustomer.CustomerName like ''%'+@CustomerName+'%'''
	end
end 
set @Query = '
SELECT DISTINCT 
    svTrnService.InvoiceNo
    , svTrnService.ServiceNo
    , svTrnService.ServiceType
    , ForemanID
    , EstimationNo
    , EstimationDate
    , BookingNo
    , BookingDate
    , svTrnService.JobOrderNo
    , svTrnService.JobOrderDate
    , svTrnService.PoliceRegNo
    , ServiceBookNo
    , svTrnService.BasicModel
    , TransmissionType
    , svTrnService.ChassisCode
    , svTrnService.ChassisNo
    , svTrnService.EngineCode
    , svTrnService.EngineNo
    , svTrnService.ChassisCode + '' '' + cast(svTrnService.ChassisNo as  varchar) KodeRangka
    , svTrnService.EngineCode + '' '' + cast(svTrnService.EngineNo as varchar) KodeMesin
    , ColorCode
    , (svTrnService.CustomerCode + '' - '' + gnMstCustomer.CustomerName) as Customer
    , (svTrnService.CustomerCodeBill + '' - '' + custBill.CustomerName) as CustomerBill
    , svTrnService.CustomerCode
    , svTrnService.CustomerCodeBill
    , svTrnService.Odometer
    , svTrnService.JobType
    , case when svTrnService.ServiceStatus=''4'' then
            case when ''' + @ProductType + '''=''4W'' then reffService.Description
                else reffService.LockingBy
            end
        else reffService.Description 
    end ServiceStatus
    --, svTrnService.PoliceRegNo
	--, svTrnService.CustomerCode
    , InsurancePayFlag
    , InsuranceOwnRisk
    , InsuranceNo
    , InsuranceJobOrderNo
    --, svTrnService.CustomerCodeBill
    , svTrnService.LaborDiscPct
    , PartDiscPct
    , svTrnService.MaterialDiscPct
    , svTrnService.PPNPct
    , svTrnService.ServiceRequestDesc
    , ConfirmChangingPart
    --, ForemanID
    , svTrnService.MechanicID
    , EstimateFinishDate
    , svTrnService.LaborDPPAmt
    , svTrnService.PartsDPPAmt
    , svTrnService.MaterialDPPAmt
    , TotalDPPAmount
    , TotalPpnAmount
    , TotalPphAmount
    , TotalSrvAmount
    , employee.EmployeeName
    , (custBill.Address1 + '''' + custBill.Address2 + '''' + custBill.Address3 + '''' + custBill.Address4) as AddressBill
    , custBill.NPWPNo
    , custBill.PhoneNo
    , custBill.HPNo
FROM svTrnService WITH(NOLOCK, NOWAIT)
LEFT JOIN gnMstCustomer 
    ON gnMstCustomer.CompanyCode = svTrnService.CompanyCode 
    AND gnMstCustomer.CustomerCode = svTrnService.CustomerCode
LEFT JOIN gnMstCustomer custBill 
    ON custBill.CompanyCode = svTrnService.CompanyCode
    AND custBill.CustomerCode = svTrnService.CustomerCodeBill
LEFT JOIN gnMstEmployee employee
    ON employee.CompanyCode = svTrnService.CompanyCode
    AND employee.BranchCode = svTrnService.BranchCode
	AND employee.EmployeeID = svTrnService.ForemanID
LEFT JOIN svTrnSrvItem srvItem 
    ON srvItem.CompanyCode = svTrnService.CompanyCode
    AND srvItem.BranchCode = svTrnService.BranchCode
    AND srvItem.ProductType = svTrnService.ProductType
    AND srvItem.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svTrnSrvTask srvTask
    ON srvTask.CompanyCode = svTrnService.CompanyCode
    AND srvTask.BranchCode = svTrnService.BranchCode
    AND srvTask.ProductType = svTrnService.ProductType
    AND srvTask.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svMstRefferenceService reffService
    ON reffService.CompanyCode = svTrnService.CompanyCode
    AND reffService.ProductType = svTrnService.ProductType    
    AND reffService.RefferenceCode = svTrnService.ServiceStatus
    AND reffService.RefferenceType = ''SERVSTAS''
LEFT JOIN svTrnInvoice invoice
	ON invoice.CompanyCode = svTrnService.CompanyCode
	AND invoice.BranchCode = svTrnService.BranchCode
	AND invoice.ProductType = svTrnService.ProductType
	AND invoice.JobOrderNo = svTrnService.JobOrderNo
LEFT JOIN svTrnSrvVOR VOR
    ON VOR.CompanyCode = svTrnService.CompanyCode
	AND VOR.BranchCode = svTrnService.BranchCode
    AND VOR.ServiceNo = svTrnService.ServiceNo
WHERE svTrnService.CompanyCode = ''' + @CompanyCode + '''
    AND svTrnService.BranchCode = ''' + @BranchCode + '''
 AND svTrnService.ServiceType =''1'''
 + @Filter
 + @Condition;
 print @Query
 exec (@Query); 

 GO
 
ALTER procedure [dbo].[usprpt_PmRpInqSummaryWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@PeriodBegin		DATETIME,
	@PeriodEnd			DATETIME,
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@Salesman			VARCHAR(15),
	@Jns				VARCHAR(1),
	@print				int = 0
	
)
AS 
BEGIN
SET NOCOUNT ON;
declare @position varchar(20), @SC varchar(20)
set @position= (
				select position 
				from HrEmployee 
				where employeeid=(select TeamLeader from HrEmployee where EmployeeID = @salesman) 
				)
set @SC= (select TeamLeader from HrEmployee where EmployeeID = @salesman)
if @print = 0
begin 
	IF @Jns = '1'
BEGIN
	SELECT * INTO #deptSales FROM(
		SELECT 
				'4' idx,
			   a.EmployeeID,
			   a.Position,
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.StatusProspek = '10' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) NEW,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'P' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'HP' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'SPK' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) SPK,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'DO' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) DO,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'DELIVERY' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'LOST' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) LOST
		FROM HrEmployee a WHERE a.Department = 'SALES' AND a.CompanyCode = @CompanyCode and a.PersonnelStatus ='1'
	)#deptSales

	--Sales Coordinator
	SELECT * INTO #qryS_SC FROM(
		SELECT 
				'3' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) LOST
		FROM #deptSales a WHERE a.Position = 'SC' 
	)#qryS_SC

	--Sales Head
	SELECT * INTO #qrySH FROM(
		SELECT 
				'2' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
			   (SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) LOST
			   --(SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') NEW,
			   --(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') PROSPECT,
			   --(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') HOTPROSPECT,
			   --(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') SPK,
			   --(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DO,
			   --(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DELIVERY,
			   --(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') LOST
		FROM #deptSales a WHERE a.Position = 'SH' 
	)#qrySH

	IF(@SalesHead = '' AND @Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM FROM(
				SELECT 
					'1' idx,
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSales a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM

			SELECT * INTO #qryAll FROM(
				SELECT * FROM #qryBM
				UNION
				SELECT * FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM)     
				UNION
				SELECT * FROM #qryS_SC WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM)) 
				UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN (SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM))) 
			)#qryAll

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAll a ORDER BY a.idx ASC
		
			DROP TABLE #qryAll
			DROP TABLE #qryBM
		END
	ELSE IF(@Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM2 FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSales a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM2

			SELECT * INTO #qryAll2 FROM(
				SELECT * FROM #qryBM2
				UNION
				SELECT * FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead     
				UNION
				--SELECT * FROM #qryS_SC WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead) 
				--UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead )
				--) 
			)#qryAll2

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAll2 a ORDER BY a.idx ASC

			DROP TABLE #qryAll2
			DROP TABLE #qryBM2
		END
	ELSE
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM3 FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSales a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM3

			SELECT * INTO #qryAll3 FROM(
				SELECT * FROM #qryBM3
				UNION
				SELECT * FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead     
				UNION
				SELECT * FROM #qryS_SC WHERE EmployeeID IN (@SC)
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead) 
				UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead )--)
				AND EmployeeID = @Salesman 
			)#qryAll3

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST 
			FROM #qryAll3 a ORDER BY a.idx ASC 

			DROP TABLE #qryAll3
			DROP TABLE #qryBM3
		END
		DROP TABLE #deptSales
		DROP TABLE #qryS_SC
		DROP TABLE #qrySH
	END
ELSE IF @Jns = '2'
	BEGIN
		SELECT * INTO #dByTipe FROM(
			SELECT b.EmployeeID, (b.TipeKendaraan + ' ' + b.Variant) ModelKendaraan, LastProgress, StatusProspek FROM PmKdp b 
			inner join HrEmployee c 
				on b.CompanyCode = c.CompanyCode and b.EmployeeID = c.EmployeeID 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) and c.PersonnelStatus = '1'
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.ModelKendaraan,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.StatusProspek = '10' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) LOST
			FROM #dByTipe a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.ModelKendaraan
		)#dSls

		--Kondisi SH = '' AND S = ''
		IF (@SalesHead = '' AND @Salesman = '')
		BEGIN
			SELECT * INTO #dt27_1 FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN(@BranchManager)) 
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.ModelKendaraan
			)#dt27_1

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_1 ORDER BY ModelKendaraan

			DROP TABLE #dt27_1
		END
		--Kondisi S = ''
		ELSE IF (@Salesman = '')
		BEGIN
			SELECT * INTO #dt27_2 FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls a 
				WHERE a.TeamLeader = @SalesHead
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.ModelKendaraan
			)#dt27_2

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_2 ORDER BY ModelKendaraan

			DROP TABLE #dt27_2
		END
		ELSE
		BEGIN
			SELECT * INTO #dt27_3 FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.ModelKendaraan
			)#dt27_3

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_3 ORDER BY ModelKendaraan

			DROP TABLE #dt27_3

		DROP TABLE #dSls
		DROP TABLE #dByTipe
		END
	END
ELSE IF @Jns = '3'
	BEGIN
		SELECT * INTO #dByTipe2 FROM(
			SELECT b.EmployeeID, b.PerolehanData, LastProgress, StatusProspek FROM PmKdp b 
			inner join HrEmployee c 
				on b.CompanyCode = c.CompanyCode and b.EmployeeID = c.EmployeeID 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) and c.PersonnelStatus = '1'
		)#dByTipe2

		SELECT * INTO #dSls2 FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.PerolehanData,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.StatusProspek <> '20' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) LOST
			FROM #dByTipe2 a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.PerolehanData
		)#dSls2

		--Kondisi SH = '' AND S = ''
		IF (@SalesHead = '' AND @Salesman = '')
		BEGIN
			SELECT * INTO #dt37_1 FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSls2 a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN(@BranchManager))
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.PerolehanData
			)#dt37_1

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_1 ORDER BY PerolehanData

			DROP TABLE #dt37_1
		END
		--Kondisi S = ''
		ELSE IF (@Salesman = '')
		BEGIN
			SELECT * INTO #dt37_2 FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2 a WHERE a.TeamLeader IN (@SalesHead)
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.PerolehanData
			)#dt37_2

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_2 ORDER BY PerolehanData

			DROP TABLE #dt37_2
		END
		ELSE
		begin
			SELECT * INTO #dt37_3 FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2 a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.PerolehanData
			)#dt37_3

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_3 ORDER BY PerolehanData

			DROP TABLE #dt37_3

		DROP TABLE #dSls2
		DROP TABLE #dByTipe2
		end
	END
end
else Begin
Print 'For Print Preview'
	SELECT * INTO #deptSalesP FROM(
		SELECT 
				'4' idx,
			   a.EmployeeID,
			   a.Position,
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.StatusProspek = '10' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) NEW,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'P' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'HP' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'SPK' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) SPK,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'DO' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) DO,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'DELIVERY' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'LOST' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) LOST
		FROM HrEmployee a WHERE a.Department = 'SALES' AND a.CompanyCode = @CompanyCode and a.PersonnelStatus ='1'
	)#deptSalesP

	--Sales Coordinator
	SELECT * INTO #qryS_SCP FROM(
		SELECT 
				'3' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT ISNULL(SUM(NEW), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) LOST
		FROM #deptSalesP a WHERE a.Position = 'SC' 
	)#qryS_SCP

	--Sales Head
	SELECT * INTO #qrySHP FROM(
		SELECT 
				'2' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
			   (SELECT ISNULL(SUM(NEW), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) LOST
			   --(SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') NEW,
			   --(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') PROSPECT,
			   --(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') HOTPROSPECT,
			   --(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') SPK,
			   --(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DO,
			   --(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DELIVERY,
			   --(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') LOST
		FROM #deptSalesP a WHERE a.Position = 'SH' 
	)#qrySHP
	
	SELECT * INTO #dByTipeP FROM(
			SELECT b.EmployeeID, (b.TipeKendaraan + ' ' + b.Variant) ModelKendaraan, LastProgress, StatusProspek FROM PmKdp b 
			inner join HrEmployee c 
				on b.CompanyCode = c.CompanyCode and b.EmployeeID = c.EmployeeID 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) and c.PersonnelStatus = '1'
		)#dByTipeP

		SELECT * INTO #dSlsP FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.ModelKendaraan,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.StatusProspek = '10' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) LOST
			FROM #dByTipeP a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.ModelKendaraan
		)#dSlsP
		
		SELECT * INTO #dByTipe2P FROM(
			SELECT b.EmployeeID, b.PerolehanData, LastProgress, StatusProspek FROM PmKdp b 
			inner join HrEmployee c 
				on b.CompanyCode = c.CompanyCode and b.EmployeeID = c.EmployeeID 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) and c.PersonnelStatus = '1'
		)#dByTipe2P

		SELECT * INTO #dSls2P FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.PerolehanData,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.StatusProspek <> '20' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) LOST
			FROM #dByTipe2P a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.PerolehanData
		)#dSls2P
		
		IF(@SalesHead = '' AND @Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBMP FROM(
				SELECT 
					'1' idx,
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSalesP a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBMP

			SELECT * INTO #qryAllP FROM(
				SELECT * FROM #qryBMP
				UNION
				SELECT * FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBMP)     
				UNION
				SELECT * FROM #qryS_SCP WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBMP)) 
				UNION
				SELECT * FROM #deptSalesP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM))) 
			)#qryAllP

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAllP a ORDER BY a.idx ASC
		
			DROP TABLE #qryAllP
			DROP TABLE #qryBMP
			
			--table2
			SELECT * INTO #dt27_1P FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSlsP a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN(@BranchManager)) 
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.ModelKendaraan
			)#dt27_1P

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_1P ORDER BY ModelKendaraan

			DROP TABLE #dt27_1P
			
			--table3
			SELECT * INTO #dt37_1P FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSls2P a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN(@BranchManager))
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.PerolehanData
			)#dt37_1P

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_1P ORDER BY PerolehanData

			DROP TABLE #dt37_1
			
		END
	ELSE IF(@Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM2P FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSalesP a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM2P

			SELECT * INTO #qryAll2P FROM(
				SELECT * FROM #qryBM2P
				UNION
				SELECT * FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2P) AND EmployeeID = @SalesHead     
				UNION
				--SELECT * FROM #qryS_SC WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead) 
				--UNION
				SELECT * FROM #deptSalesP WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2P) AND EmployeeID = @SalesHead )
				--) 
			)#qryAll2P

			SELECT 
				'6159401000' CompanyCode, '6159401001' BranchCode, ''TeamID, '' PositionID, a.EmployeeName,
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS PositionName,
				 a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAll2P a ORDER BY a.idx ASC

			DROP TABLE #qryAll2P
			DROP TABLE #qryBM2P
			
			--table2
			SELECT * INTO #dt27_2P FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSlsP a 
				WHERE a.TeamLeader =@SalesHead
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.ModelKendaraan
			)#dt27_2P

			SELECT ModelKendaraan TipeKendaraan, '' GroupCode, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_2P ORDER BY ModelKendaraan

			DROP TABLE #dt27_2P
			
			--table3
			SELECT * INTO #dt37_2P FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2P a WHERE a.TeamLeader IN (@SalesHead)
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.PerolehanData
			)#dt37_2P

			SELECT '6159401000' CompanyCode, PerolehanData Source, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_2P ORDER BY PerolehanData

			DROP TABLE #dt37_2P
			
		END
	ELSE
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM3P FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSalesP a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM3P

			SELECT * INTO #qryAll3P FROM(
				SELECT * FROM #qryBM3P
				UNION
				SELECT * FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3P) AND EmployeeID = @SalesHead     
				UNION
				SELECT * FROM #qryS_SCP WHERE EmployeeID IN (@SC)
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead) 
				UNION
				SELECT * FROM #deptSalesP WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3P) AND EmployeeID = @SalesHead )--)
				AND EmployeeID = @Salesman 
			)#qryAll3P

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST 
			FROM #qryAll3P a ORDER BY a.idx ASC 

			DROP TABLE #qryAll3
			DROP TABLE #qryBM3
			
			--table2
			SELECT * INTO #dt27_3P FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSlsP a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.ModelKendaraan
			)#dt27_3P

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_3P ORDER BY ModelKendaraan

			DROP TABLE #dt27_3P
			
			--table3
			SELECT * INTO #dt37_3P FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2P a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.PerolehanData
			)#dt37_3

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_3P ORDER BY PerolehanData

			DROP TABLE #dt37_3P
			
		END
End
END
GO

ALTER procedure [dbo].[usprpt_PmRpInqSummaryWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@PeriodBegin		DATETIME,
	@PeriodEnd			DATETIME,
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@Salesman			VARCHAR(15),
	@Jns				VARCHAR(1),
	@print				int = 0
	
)
AS 
BEGIN
SET NOCOUNT ON;
declare @position varchar(20), @SC varchar(20)
set @position= (
				select position 
				from HrEmployee 
				where employeeid=(select TeamLeader from HrEmployee where EmployeeID = @salesman) 
				)
set @SC= (select TeamLeader from HrEmployee where EmployeeID = @salesman)
if @print = 0
begin 
	IF @Jns = '1'
BEGIN
	SELECT * INTO #deptSales FROM(
		SELECT 
				'4' idx,
			   a.EmployeeID,
			   a.Position,
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.StatusProspek = '10' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) NEW,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'P' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'HP' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'SPK' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) SPK,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'DO' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) DO,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'DELIVERY' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'LOST' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) LOST
		FROM HrEmployee a WHERE a.Department = 'SALES' AND a.CompanyCode = @CompanyCode and a.PersonnelStatus ='1'
	)#deptSales

	--Sales Coordinator
	SELECT * INTO #qryS_SC FROM(
		SELECT 
				'3' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) LOST
		FROM #deptSales a WHERE a.Position = 'SC' 
	)#qryS_SC

	--Sales Head
	SELECT * INTO #qrySH FROM(
		SELECT 
				'2' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
			   (SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID)) LOST
			   --(SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') NEW,
			   --(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') PROSPECT,
			   --(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') HOTPROSPECT,
			   --(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') SPK,
			   --(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DO,
			   --(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DELIVERY,
			   --(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') LOST
		FROM #deptSales a WHERE a.Position = 'SH' 
	)#qrySH

	IF(@SalesHead = '' AND @Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM FROM(
				SELECT 
					'1' idx,
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSales a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM

			SELECT * INTO #qryAll FROM(
				SELECT * FROM #qryBM
				UNION
				SELECT * FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM)     
				UNION
				SELECT * FROM #qryS_SC WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM)) 
				UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN (SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM))) 
			)#qryAll

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAll a ORDER BY a.idx ASC
		
			DROP TABLE #qryAll
			DROP TABLE #qryBM
		END
	ELSE IF(@Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM2 FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSales a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM2

			SELECT * INTO #qryAll2 FROM(
				SELECT * FROM #qryBM2
				UNION
				SELECT * FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead     
				UNION
				--SELECT * FROM #qryS_SC WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead) 
				--UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead )
				--) 
			)#qryAll2

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAll2 a ORDER BY a.idx ASC

			DROP TABLE #qryAll2
			DROP TABLE #qryBM2
		END
	ELSE
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM3 FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySH b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSales a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM3

			SELECT * INTO #qryAll3 FROM(
				SELECT * FROM #qryBM3
				UNION
				SELECT * FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead     
				UNION
				SELECT * FROM #qryS_SC WHERE EmployeeID IN (@SC)
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead) 
				UNION
				SELECT * FROM #deptSales WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead )--)
				AND EmployeeID = @Salesman 
			)#qryAll3

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST 
			FROM #qryAll3 a ORDER BY a.idx ASC 

			DROP TABLE #qryAll3
			DROP TABLE #qryBM3
		END
		DROP TABLE #deptSales
		DROP TABLE #qryS_SC
		DROP TABLE #qrySH
	END
ELSE IF @Jns = '2'
	BEGIN
		SELECT * INTO #dByTipe FROM(
			SELECT b.EmployeeID, (b.TipeKendaraan + ' ' + b.Variant) ModelKendaraan, LastProgress, StatusProspek FROM PmKdp b 
			inner join HrEmployee c 
				on b.CompanyCode = c.CompanyCode and b.EmployeeID = c.EmployeeID 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) and c.PersonnelStatus = '1'
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.ModelKendaraan,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.StatusProspek = '10' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) LOST
			FROM #dByTipe a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.ModelKendaraan
		)#dSls

		--Kondisi SH = '' AND S = ''
		IF (@SalesHead = '' AND @Salesman = '')
		BEGIN
			SELECT * INTO #dt27_1 FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN(@BranchManager)) 
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.ModelKendaraan
			)#dt27_1

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_1 ORDER BY ModelKendaraan

			DROP TABLE #dt27_1
		END
		--Kondisi S = ''
		ELSE IF (@Salesman = '')
		BEGIN
			SELECT * INTO #dt27_2 FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls a 
				WHERE a.TeamLeader = @SalesHead
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.ModelKendaraan
			)#dt27_2

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_2 ORDER BY ModelKendaraan

			DROP TABLE #dt27_2
		END
		ELSE
		BEGIN
			SELECT * INTO #dt27_3 FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.ModelKendaraan
			)#dt27_3

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_3 ORDER BY ModelKendaraan

			DROP TABLE #dt27_3

		DROP TABLE #dSls
		DROP TABLE #dByTipe
		END
	END
ELSE IF @Jns = '3'
	BEGIN
		SELECT * INTO #dByTipe2 FROM(
			SELECT b.EmployeeID, b.PerolehanData, LastProgress, StatusProspek FROM PmKdp b 
			inner join HrEmployee c 
				on b.CompanyCode = c.CompanyCode and b.EmployeeID = c.EmployeeID 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) and c.PersonnelStatus = '1'
		)#dByTipe2

		SELECT * INTO #dSls2 FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.PerolehanData,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.StatusProspek <> '20' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2 b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) LOST
			FROM #dByTipe2 a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.PerolehanData
		)#dSls2

		--Kondisi SH = '' AND S = ''
		IF (@SalesHead = '' AND @Salesman = '')
		BEGIN
			SELECT * INTO #dt37_1 FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSls2 a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN(@BranchManager))
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.PerolehanData
			)#dt37_1

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_1 ORDER BY PerolehanData

			DROP TABLE #dt37_1
		END
		--Kondisi S = ''
		ELSE IF (@Salesman = '')
		BEGIN
			SELECT * INTO #dt37_2 FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2 a WHERE a.TeamLeader IN (@SalesHead)
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.PerolehanData
			)#dt37_2

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_2 ORDER BY PerolehanData

			DROP TABLE #dt37_2
		END
		ELSE
		begin
			SELECT * INTO #dt37_3 FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2 a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.PerolehanData
			)#dt37_3

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_3 ORDER BY PerolehanData

			DROP TABLE #dt37_3

		DROP TABLE #dSls2
		DROP TABLE #dByTipe2
		end
	END
end
else Begin
Print 'For Print Preview'
	SELECT * INTO #deptSalesP FROM(
		SELECT 
				'4' idx,
			   a.EmployeeID,
			   a.Position,
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.StatusProspek = '10' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) NEW,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'P' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'HP' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'SPK' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) SPK,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'DO' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) DO,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'DELIVERY' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
				AND b.LastProgress = 'LOST' AND (b.EmployeeID = a.EmployeeID) AND InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) LOST
		FROM HrEmployee a WHERE a.Department = 'SALES' AND a.CompanyCode = @CompanyCode and a.PersonnelStatus ='1'
	)#deptSalesP

	--Sales Coordinator
	SELECT * INTO #qryS_SCP FROM(
		SELECT 
				'3' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
				(SELECT ISNULL(SUM(NEW), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) LOST
		FROM #deptSalesP a WHERE a.Position = 'SC' 
	)#qryS_SCP

	--Sales Head
	SELECT * INTO #qrySHP FROM(
		SELECT 
				'2' idx,
			   a.EmployeeID,
			   a.Position, 
			   a.EmployeeName,
			   a.TeamLeader,
			   (SELECT ISNULL(SUM(NEW), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) NEW,
				(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) PROSPECT, 
				(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) HOTPROSPECT,
				(SELECT ISNULL(SUM(SPK), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) SPK,
				(SELECT ISNULL(SUM(DO), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) DO,
				(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) DELIVERY,
				(SELECT ISNULL(SUM(LOST), 0) FROM #deptSalesP b WHERE (b.TeamLeader = a.EmployeeID)) LOST
			   --(SELECT ISNULL(SUM(NEW), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') NEW,
			   --(SELECT ISNULL(SUM(PROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') PROSPECT,
			   --(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') HOTPROSPECT,
			   --(SELECT ISNULL(SUM(SPK), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') SPK,
			   --(SELECT ISNULL(SUM(DO), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DO,
			   --(SELECT ISNULL(SUM(DELIVERY), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') DELIVERY,
			   --(SELECT ISNULL(SUM(LOST), 0) FROM #deptSales b WHERE (b.TeamLeader = a.EmployeeID) AND b.Position = 'SC') LOST
		FROM #deptSalesP a WHERE a.Position = 'SH' 
	)#qrySHP
	
	SELECT * INTO #dByTipeP FROM(
			SELECT b.EmployeeID, (b.TipeKendaraan + ' ' + b.Variant) ModelKendaraan, LastProgress, StatusProspek FROM PmKdp b 
			inner join HrEmployee c 
				on b.CompanyCode = c.CompanyCode and b.EmployeeID = c.EmployeeID 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) and c.PersonnelStatus = '1'
		)#dByTipeP

		SELECT * INTO #dSlsP FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.ModelKendaraan,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.StatusProspek = '10' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipeP b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) LOST
			FROM #dByTipeP a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.ModelKendaraan
		)#dSlsP
		
		SELECT * INTO #dByTipe2P FROM(
			SELECT b.EmployeeID, b.PerolehanData, LastProgress, StatusProspek FROM PmKdp b 
			inner join HrEmployee c 
				on b.CompanyCode = c.CompanyCode and b.EmployeeID = c.EmployeeID 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND (b.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd) and c.PersonnelStatus = '1'
		)#dByTipe2P

		SELECT * INTO #dSls2P FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.PerolehanData,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.StatusProspek <> '20' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) NEW,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) SPK,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'DO' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DO,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'DELIVERY' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) DELIVERY,
				(SELECT COUNT(StatusProspek) FROM #dByTipe2P b WHERE b.LastProgress = 'LOST' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) LOST
			FROM #dByTipe2P a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.PerolehanData
		)#dSls2P
		
		IF(@SalesHead = '' AND @Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBMP FROM(
				SELECT 
					'1' idx,
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSalesP a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBMP

			SELECT * INTO #qryAllP FROM(
				SELECT * FROM #qryBMP
				UNION
				SELECT * FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBMP)     
				UNION
				SELECT * FROM #qryS_SCP WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBMP)) 
				UNION
				SELECT * FROM #deptSalesP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN (SELECT EmployeeID FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM))) 
			)#qryAllP

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAllP a ORDER BY a.idx ASC
		
			DROP TABLE #qryAllP
			DROP TABLE #qryBMP
			
			--table2
			SELECT * INTO #dt27_1P FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSlsP a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN(@BranchManager)) 
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.ModelKendaraan
			)#dt27_1P

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_1P ORDER BY ModelKendaraan

			DROP TABLE #dt27_1P
			
			--table3
			SELECT * INTO #dt37_1P FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST 
				FROM #dSls2P a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN(@BranchManager))
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				GROUP BY a.PerolehanData
			)#dt37_1P

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_1P ORDER BY PerolehanData

			DROP TABLE #dt37_1
			
		END
	ELSE IF(@Salesman = '')
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM2P FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSalesP a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM2P

			SELECT * INTO #qryAll2P FROM(
				SELECT * FROM #qryBM2P
				UNION
				SELECT * FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2P) AND EmployeeID = @SalesHead     
				UNION
				--SELECT * FROM #qryS_SC WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2) AND EmployeeID = @SalesHead) 
				--UNION
				SELECT * FROM #deptSalesP WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM2P) AND EmployeeID = @SalesHead )
				--) 
			)#qryAll2P

			SELECT 
				'6159401000' CompanyCode, '6159401001' BranchCode, ''TeamID, '' PositionID, a.EmployeeName,
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS PositionName,
				 a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST  
			FROM #qryAll2P a ORDER BY a.idx ASC

			DROP TABLE #qryAll2P
			DROP TABLE #qryBM2P
			
			--table2
			SELECT * INTO #dt27_2P FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSlsP a 
				WHERE a.TeamLeader =@SalesHead
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.ModelKendaraan
			)#dt27_2P

			SELECT ModelKendaraan TipeKendaraan, '' GroupCode, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_2P ORDER BY ModelKendaraan

			DROP TABLE #dt27_2P
			
			--table3
			SELECT * INTO #dt37_2P FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2P a WHERE a.TeamLeader IN (@SalesHead)
				--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @SalesHead)) -- >ID SH   
				GROUP BY a.PerolehanData
			)#dt37_2P

			SELECT '6159401000' CompanyCode, PerolehanData Source, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_2P ORDER BY PerolehanData

			DROP TABLE #dt37_2P
			
		END
	ELSE
		BEGIN
			--Branch Manager
			SELECT * INTO #qryBM3P FROM(
				SELECT
					'1' idx, 
					a.EmployeeID,
					a.Position, 
					a.EmployeeName,
					a.TeamLeader,
  					(SELECT ISNULL(SUM(NEW), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) NEW,
					(SELECT ISNULL(SUM(PROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) PROSPECT,
					(SELECT ISNULL(SUM(HOTPROSPECT), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) HOTPROSPECT,
					(SELECT ISNULL(SUM(SPK), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) SPK,
					(SELECT ISNULL(SUM(DO), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DO,
					(SELECT ISNULL(SUM(DELIVERY), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) DELIVERY,
					(SELECT ISNULL(SUM(LOST), 0) FROM #qrySHP b WHERE b.TeamLeader = a.EmployeeID) LOST
				FROM #deptSalesP a WHERE a.Position = 'BM' AND a.EmployeeID = @BranchManager
			)#qryBM3P

			SELECT * INTO #qryAll3P FROM(
				SELECT * FROM #qryBM3P
				UNION
				SELECT * FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3P) AND EmployeeID = @SalesHead     
				UNION
				SELECT * FROM #qryS_SCP WHERE EmployeeID IN (@SC)
				--(SELECT EmployeeID FROM #qrySH WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3) AND EmployeeID = @SalesHead) 
				UNION
				SELECT * FROM #deptSalesP WHERE TeamLeader IN 
				--(SELECT EmployeeID FROM #qryS_SC WHERE TeamLeader IN 
				(SELECT EmployeeID FROM #qrySHP WHERE TeamLeader IN (SELECT EmployeeID FROM #qryBM3P) AND EmployeeID = @SalesHead )--)
				AND EmployeeID = @Salesman 
			)#qryAll3P

			SELECT 
				CASE a.Position
				WHEN 'BM' THEN 'Branch Manager'
				WHEN 'SH' THEN 'Sales Head'
				WHEN 'SC' THEN 'Sales Coordinator'
				WHEN 'S' THEN 'Salesman'
				ELSE 'Sales'
				END AS Position,
				a.EmployeeName, a.NEW, a.PROSPECT, a.HOTPROSPECT, a.SPK, a.DO, a.DELIVERY, a.LOST 
			FROM #qryAll3P a ORDER BY a.idx ASC 

			DROP TABLE #qryAll3
			DROP TABLE #qryBM3
			
			--table2
			SELECT * INTO #dt27_3P FROM(
				SELECT
					a.ModelKendaraan,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSlsP a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.ModelKendaraan
			)#dt27_3P

			SELECT ModelKendaraan TipeKendaraan, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt27_3P ORDER BY ModelKendaraan

			DROP TABLE #dt27_3P
			
			--table3
			SELECT * INTO #dt37_3P FROM(
				SELECT
					a.PerolehanData,
					SUM(a.NEW) NEW,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK,
					SUM(a.DO) DO,
					SUM(a.DELIVERY) DELIVERY,
					SUM(a.LOST) LOST
				FROM #dSls2P a WHERE a.EmployeeID = @Salesman   
				GROUP BY a.PerolehanData
			)#dt37_3

			SELECT PerolehanData SumberData, NEW, PROSPECT, HOTPROSPECT, SPK, DO, DELIVERY, LOST FROM #dt37_3P ORDER BY PerolehanData

			DROP TABLE #dt37_3P
			
		END
End
END
GO

ALTER procedure [dbo].[uspfn_SvTrnServiceSelectBookingData]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType  varchar(15),
	@ShowAll bit,
	@JobOrderNo varchar(20),
	@PoliceRegNo  varchar(20),
	@CustomerName varchar(200),
	@ServiceBookNo  varchar(20)
AS

declare @Query varchar(max), @Filter varchar(max)=''
declare @Condition varchar(4000);

set @Condition = ' ORDER BY svTrnService.BookingNo DESC';
if(@ShowAll = 0) begin
	set @Condition = ' AND svTrnService.ServiceStatus IN (0,1,2,3,4,5) ORDER BY svTrnService.BookingNo DESC';
	if(@JobOrderNo <> '') begin 
		set @Filter = @Filter + ' And BookingNo like ''%'+@JobOrderNo+'%'' '
	end
	if(@PoliceRegNo <> '') begin 
		set @Filter = @Filter + ' And svTrnService.PoliceRegNo like ''%'+@PoliceRegNo+'%'' '
	end
	if(@ServiceBookNo <> '' ) begin 
		set @Filter = @Filter + ' And ServiceBookNo like ''%'+@ServiceBookNo+'%'' '
	end
	if(@CustomerName <> '') begin 
		set @Filter = @Filter + ' And gnMstCustomer.CustomerName like ''%'+@CustomerName+'%'''
	end
end 

if(@ShowAll = 1) begin
	if(@JobOrderNo <> '') begin 
		set @Filter = @Filter + ' And BookingNo like ''%'+@JobOrderNo+'%'' '
	end
	if(@PoliceRegNo <> '') begin 
		set @Filter = @Filter + ' And svTrnService.PoliceRegNo like ''%'+@PoliceRegNo+'%'' '
	end
	if(@ServiceBookNo <> '' ) begin 
		set @Filter = @Filter + ' And ServiceBookNo like ''%'+@ServiceBookNo+'%'' '
	end
	if(@CustomerName <> '') begin 
		set @Filter = @Filter + ' And gnMstCustomer.CustomerName like ''%'+@CustomerName+'%'''
	end
end 
set @Query = '
SELECT DISTINCT 
    svTrnService.InvoiceNo
    , svTrnService.ServiceNo
    , svTrnService.ServiceType
    , ForemanID
    , EstimationNo
    , EstimationDate
    , BookingNo
    , BookingDate
    , svTrnService.JobOrderNo
    , svTrnService.JobOrderDate
    , svTrnService.PoliceRegNo
    , ServiceBookNo
    , svTrnService.BasicModel
    , TransmissionType
    , svTrnService.ChassisCode
    , svTrnService.ChassisNo
    , svTrnService.EngineCode
    , svTrnService.EngineNo
    , svTrnService.ChassisCode + '' '' + cast(svTrnService.ChassisNo as  varchar) KodeRangka
    , svTrnService.EngineCode + '' '' + cast(svTrnService.EngineNo as varchar) KodeMesin
    , ColorCode
    , (svTrnService.CustomerCode + '' - '' + gnMstCustomer.CustomerName) as Customer
    , (svTrnService.CustomerCodeBill + '' - '' + custBill.CustomerName) as CustomerBill
    , svTrnService.CustomerCode
    , svTrnService.CustomerCodeBill
    , svTrnService.Odometer
    , svTrnService.JobType
    , case when svTrnService.ServiceStatus=''4'' then
            case when ''' + @ProductType + '''=''4W'' then reffService.Description
                else reffService.LockingBy
            end
        else reffService.Description 
    end ServiceStatus
    --, svTrnService.PoliceRegNo
	--, svTrnService.CustomerCode
    , InsurancePayFlag
    , InsuranceOwnRisk
    , InsuranceNo
    , InsuranceJobOrderNo
    --, svTrnService.CustomerCodeBill
    , svTrnService.LaborDiscPct
    , PartDiscPct
    , svTrnService.MaterialDiscPct
    , svTrnService.PPNPct
    , svTrnService.ServiceRequestDesc
    , ConfirmChangingPart
    --, ForemanID
    , svTrnService.MechanicID
    , EstimateFinishDate
    , svTrnService.LaborDPPAmt
    , svTrnService.PartsDPPAmt
    , svTrnService.MaterialDPPAmt
    , TotalDPPAmount
    , TotalPpnAmount
    , TotalPphAmount
    , TotalSrvAmount
    , employee.EmployeeName
    , (custBill.Address1 + '''' + custBill.Address2 + '''' + custBill.Address3 + '''' + custBill.Address4) as AddressBill
    , custBill.NPWPNo
    , custBill.PhoneNo
    , custBill.HPNo
FROM svTrnService WITH(NOLOCK, NOWAIT)
LEFT JOIN gnMstCustomer 
    ON gnMstCustomer.CompanyCode = svTrnService.CompanyCode 
    AND gnMstCustomer.CustomerCode = svTrnService.CustomerCode
LEFT JOIN gnMstCustomer custBill 
    ON custBill.CompanyCode = svTrnService.CompanyCode
    AND custBill.CustomerCode = svTrnService.CustomerCodeBill
LEFT JOIN gnMstEmployee employee
    ON employee.CompanyCode = svTrnService.CompanyCode
    AND employee.BranchCode = svTrnService.BranchCode
	AND employee.EmployeeID = svTrnService.ForemanID
LEFT JOIN svTrnSrvItem srvItem 
    ON srvItem.CompanyCode = svTrnService.CompanyCode
    AND srvItem.BranchCode = svTrnService.BranchCode
    AND srvItem.ProductType = svTrnService.ProductType
    AND srvItem.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svTrnSrvTask srvTask
    ON srvTask.CompanyCode = svTrnService.CompanyCode
    AND srvTask.BranchCode = svTrnService.BranchCode
    AND srvTask.ProductType = svTrnService.ProductType
    AND srvTask.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svMstRefferenceService reffService
    ON reffService.CompanyCode = svTrnService.CompanyCode
    AND reffService.ProductType = svTrnService.ProductType    
    AND reffService.RefferenceCode = svTrnService.ServiceStatus
    AND reffService.RefferenceType = ''SERVSTAS''
LEFT JOIN svTrnInvoice invoice
	ON invoice.CompanyCode = svTrnService.CompanyCode
	AND invoice.BranchCode = svTrnService.BranchCode
	AND invoice.ProductType = svTrnService.ProductType
	AND invoice.JobOrderNo = svTrnService.JobOrderNo
LEFT JOIN svTrnSrvVOR VOR
    ON VOR.CompanyCode = svTrnService.CompanyCode
	AND VOR.BranchCode = svTrnService.BranchCode
    AND VOR.ServiceNo = svTrnService.ServiceNo
WHERE svTrnService.CompanyCode = ''' + @CompanyCode + '''
    AND svTrnService.BranchCode = ''' + @BranchCode + '''
 AND svTrnService.ServiceType =''1'''
 + @Filter
 + @Condition;
 print @Query
 exec (@Query); 
GO
ALTER procedure [dbo].[uspfn_SvTrnServiceSelectEstimationData]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType  varchar(15),
	@ShowAll bit,
	@EstimationNo varchar(20),
	@PoliceRegNo  varchar(20),
	@CustomerName varchar(200),
	@ServiceBookNo  varchar(20)
AS
declare @Query varchar(max), @Filter varchar(max)=''
declare @Condition varchar(4000);

set @Condition = ' ORDER BY svTrnService.EstimationNo DESC';
if(@ShowAll = 0) begin
	set @Condition = ' AND svTrnService.ServiceStatus IN (0,1,2,3,4,5) ORDER BY svTrnService.EstimationNo DESC';
	if(@EstimationNo <> '') begin 
		set @Filter = @Filter + ' And EstimationNo like ''%'+@EstimationNo+'%'' '
	end
	if(@PoliceRegNo <> '') begin 
		set @Filter = @Filter + ' And svTrnService.PoliceRegNo like ''%'+@PoliceRegNo+'%'' '
	end
	if(@ServiceBookNo <> '' ) begin 
		set @Filter = @Filter + ' And ServiceBookNo like ''%'+@ServiceBookNo+'%'' '
	end
	if(@CustomerName <> '') begin 
		set @Filter = @Filter + ' And gnMstCustomer.CustomerName like ''%'+@CustomerName+'%'''
	end
end 

if(@ShowAll = 1) begin
	if(@EstimationNo <> '') begin 
		set @Filter = @Filter + ' And EstimationNo like ''%'+@EstimationNo+'%'' '
	end
	if(@PoliceRegNo <> '') begin 
		set @Filter = @Filter + ' And svTrnService.PoliceRegNo like ''%'+@PoliceRegNo+'%'' '
	end
	if(@ServiceBookNo <> '' ) begin 
		set @Filter = @Filter + ' And ServiceBookNo like ''%'+@ServiceBookNo+'%'' '
	end
	if(@CustomerName <> '') begin 
		set @Filter = @Filter + ' And gnMstCustomer.CustomerName like ''%'+@CustomerName+'%'''
	end
end 

set @Query = '
SELECT DISTINCT 
    svTrnService.InvoiceNo
    , svTrnService.ServiceNo
    , svTrnService.ServiceType
    , ForemanID
    , EstimationNo
    , EstimationDate
    , BookingNo
    , BookingDate
    , svTrnService.JobOrderNo
    , svTrnService.JobOrderDate
    , svTrnService.PoliceRegNo
    , ServiceBookNo
    , svTrnService.BasicModel
    , TransmissionType
    , svTrnService.ChassisCode
    , svTrnService.ChassisNo
    , svTrnService.EngineCode
    , svTrnService.EngineNo
    , svTrnService.ChassisCode + '' '' + cast(svTrnService.ChassisNo as  varchar) KodeRangka
    , svTrnService.EngineCode + '' '' + cast(svTrnService.EngineNo as varchar) KodeMesin
    , ColorCode
    , (svTrnService.CustomerCode + '' - '' + gnMstCustomer.CustomerName) as Customer
    , (svTrnService.CustomerCodeBill + '' - '' + custBill.CustomerName) as CustomerBill
    , svTrnService.CustomerCode
    , svTrnService.CustomerCodeBill
    , svTrnService.Odometer
    , svTrnService.JobType
    , case when svTrnService.ServiceStatus=''4'' then
            case when ''' + @ProductType + '''=''4W'' then reffService.Description
                else reffService.LockingBy
            end
        else reffService.Description 
    end ServiceStatus
    --, svTrnService.PoliceRegNo
	--, svTrnService.CustomerCode
    , InsurancePayFlag
    , InsuranceOwnRisk
    , InsuranceNo
    , InsuranceJobOrderNo
    --, svTrnService.CustomerCodeBill
    , svTrnService.LaborDiscPct
    , PartDiscPct
    , svTrnService.MaterialDiscPct
    , svTrnService.PPNPct
    , svTrnService.ServiceRequestDesc
    , ConfirmChangingPart
    --, ForemanID
    , svTrnService.MechanicID
    , EstimateFinishDate
    , svTrnService.LaborDPPAmt
    , svTrnService.PartsDPPAmt
    , svTrnService.MaterialDPPAmt
    , TotalDPPAmount
    , TotalPpnAmount
    , TotalPphAmount
    , TotalSrvAmount
    , employee.EmployeeName
    , (custBill.Address1 + '''' + custBill.Address2 + '''' + custBill.Address3 + '''' + custBill.Address4) as AddressBill
    , custBill.NPWPNo
    , custBill.PhoneNo
    , custBill.HPNo
FROM svTrnService WITH(NOLOCK, NOWAIT)
LEFT JOIN gnMstCustomer 
    ON gnMstCustomer.CompanyCode = svTrnService.CompanyCode 
    AND gnMstCustomer.CustomerCode = svTrnService.CustomerCode
LEFT JOIN gnMstCustomer custBill 
    ON custBill.CompanyCode = svTrnService.CompanyCode
    AND custBill.CustomerCode = svTrnService.CustomerCodeBill
LEFT JOIN gnMstEmployee employee
    ON employee.CompanyCode = svTrnService.CompanyCode
    AND employee.BranchCode = svTrnService.BranchCode
	AND employee.EmployeeID = svTrnService.ForemanID
LEFT JOIN svTrnSrvItem srvItem 
    ON srvItem.CompanyCode = svTrnService.CompanyCode
    AND srvItem.BranchCode = svTrnService.BranchCode
    AND srvItem.ProductType = svTrnService.ProductType
    AND srvItem.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svTrnSrvTask srvTask
    ON srvTask.CompanyCode = svTrnService.CompanyCode
    AND srvTask.BranchCode = svTrnService.BranchCode
    AND srvTask.ProductType = svTrnService.ProductType
    AND srvTask.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svMstRefferenceService reffService
    ON reffService.CompanyCode = svTrnService.CompanyCode
    AND reffService.ProductType = svTrnService.ProductType    
    AND reffService.RefferenceCode = svTrnService.ServiceStatus
    AND reffService.RefferenceType = ''SERVSTAS''
LEFT JOIN svTrnInvoice invoice
	ON invoice.CompanyCode = svTrnService.CompanyCode
	AND invoice.BranchCode = svTrnService.BranchCode
	AND invoice.ProductType = svTrnService.ProductType
	AND invoice.JobOrderNo = svTrnService.JobOrderNo
LEFT JOIN svTrnSrvVOR VOR
    ON VOR.CompanyCode = svTrnService.CompanyCode
	AND VOR.BranchCode = svTrnService.BranchCode
    AND VOR.ServiceNo = svTrnService.ServiceNo
WHERE svTrnService.CompanyCode = ''' + @CompanyCode + '''
    AND svTrnService.BranchCode = ''' + @BranchCode + '''
 AND svTrnService.ServiceType in (''0'',''2'') and svTrnService.EstimationNo <> '''''
 + @filter
 + @Condition;
 
 exec (@Query); 
GO


ALTER procedure [dbo].[uspfn_GenerateSSPickingslipNew]
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@TransType		VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@DocDate		DATETIME
AS
BEGIN

--declare	@CompanyCode	VARCHAR(MAX)
--declare	@BranchCode		VARCHAR(MAX)
--declare	@JobOrderNo		VARCHAR(MAX)
--declare	@ProductType	VARCHAR(MAX)
--declare	@CustomerCode	VARCHAR(MAX)
--declare	@TransType		VARCHAR(MAX)
--declare	@UserID			VARCHAR(MAX)
--declare	@DocDate		DATETIME

--set	@CompanyCode	= '6156401000'
--set	@BranchCode		= '6156401001'
--set	@JobOrderNo		= 'SPK/15/001833'
--set	@ProductType	= '4W'
--set	@CustomerCode	= '000003'
--set	@TransType		= '20'
--set	@UserID			= 'TRAININGZZZ'
--set	@DocDate		= '3/12/2015 9:47:01 AM'


--exec uspfn_GenerateSSPickingslipNew '6006400001','6006400101','SPK/14/101589','4W','2105885','20','ga','3/2/2015 4:03:01 PM'
--================================================================================================================================
-- TABLE MASTER
--================================================================================================================================
-- Temporary for Item --
------------------------
SELECT * INTO #Item FROM (
SELECT a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.RetailPrice
	, a.PartNo
	, a.Billtype
	, SUM(ISNULL(a.DemandQty, 0) - (ISNULL(a.SupplyQty, 0))) QtyOrder
FROM svTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN svTrnService b ON b.CompanyCode = a.CompanyCode
	AND b.BranchCode = a.BranchCode
	AND b.ProductType = a.ProductType
	AND b.ServiceNo = a.ServiceNo
	AND b.JobOrderNo = @JobOrderNo
WHERE a.CompanyCode = @CompanyCode 
	AND a.BranchCode = @BranchCode 
	AND a.ProductType = @ProductType 
GROUP BY a.CompanyCode, a.BranchCode, a.ProductType
	, a.ServiceNo, a.PartNo, a.RetailPrice, a.BillType ) #Item 

DECLARE @CompanyMD AS VARCHAR(15)
DECLARE @BranchMD AS VARCHAR(15)
DECLARE @WarehouseMD AS VARCHAR(15)

SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

SELECT * INTO #SrvOrder FROM (
SELECT DISTINCT(a.CompanyCode) 
    , a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
    , (SELECT Paravalue FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'GTGO' AND LookUpValue = a.TypeOfGoods) TipePart
    , (SELECT PartName FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) PartName
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, SUM(a.QtyOrder) QtyOrder
    , 0 QtySupply
    , 0 QtyBO
    , (SUM(a.QtyOrder) * a.RetailPrice) * ((100 - a.PartDiscPct)/100) NetSalesAmt
    , a.PartDiscPct DiscPct
FROM
(
	SELECT
		DISTINCT(a.CompanyCode) 
		, a.BranchCode
		, a.ProductType
		, a.ServiceNo
		, a.PartNo
		, a.RetailPrice
		, c.CostPrice
		, a.TypeOfGoods
		, a.BillType
		, ISNULL(Item.QtyOrder,0) AS QtyOrder
		, a.DiscPct PartDiscPct 
	FROM
		svTrnSrvItem a WITH (NOLOCK, NOWAIT)
		LEFT JOIN svTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode	
			AND a.ProductType = b.ProductType
			AND a.ServiceNo = b.ServiceNo
		LEFT JOIN #Item Item ON Item.CompanyCode = a.CompanyCode 
			AND Item.BranchCode = a.BranchCode 
			AND Item.ProductType = a.ProductType 
			AND Item.ServiceNo = a.ServiceNo 
			AND Item.PartNo = a.PartNo 
			AND Item.RetailPrice = a.RetailPrice 
			AND Item.BillType = a.Billtype
		LEFT JOIN SpMstItemPrice c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode 
			AND a.BranchCode = c.BranchCode 
			AND a.PartNo = c.PartNo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND a.ProductType = @ProductType
		AND Item.QtyOrder > 0
		AND JobOrderNo = @JobOrderNo
		AND (a.SupplySlipNo is null OR a.SupplySlipNo = '')
) a
GROUP BY
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
    , a.PartDiscPct 
) #SrvOrder

select * from #srvorder

--================================================================================================================================
-- INSERT TABLE SpTrnSORDHdr AND SpTrnSORDDtl
--================================================================================================================================
DECLARE @MaxDocNo			INT
DECLARE	@MaxPickingList		INT
DECLARE @TempDocNo			VARCHAR(MAX)
DECLARE @TempPickingList	VARCHAR(MAX)
DECLARE @TypeOfGoods		VARCHAR(MAX)
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

--===============================================================================================================================
-- LOOPING BASED ON THE TYPE OF GOODS
-- ==============================================================================================================================
DECLARE db_cursor CURSOR FOR
SELECT DISTINCT TypeOfGoods FROM #SrvOrder
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND ProductType = @ProductType 

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @TypeOfGoods

WHILE @@FETCH_STATUS = 0
BEGIN

--===============================================================================================================================
-- INSERT HEADER
-- ==============================================================================================================================
SET @MaxDocNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'SSS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempDocNo = ISNULL((SELECT 'SSS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxDocNo, 1), 6)),'SSS/YY/XXXXXX')

INSERT INTO SpTrnSORDHdr
([CompanyCode]
           ,[BranchCode]
           ,[DocNo]
           ,[DocDate]
           ,[UsageDocNo]
           ,[UsageDocDate]
           ,[CustomerCode]
           ,[CustomerCodeBill]
           ,[CustomerCodeShip]
           ,[isBO]
           ,[isSubstitution]
           ,[isIncludePPN]
           ,[TransType]
           ,[SalesType]
           ,[IsPORDD]
           ,[OrderNo]
           ,[OrderDate]
           ,[TOPCode]
           ,[TOPDays]
           ,[PaymentCode]
           ,[PaymentRefNo]
           ,[TotSalesQty]
           ,[TotSalesAmt]
           ,[TotDiscAmt]
           ,[TotDPPAmt]
           ,[TotPPNAmt]
           ,[TotFinalSalesAmt]
           ,[isPKP]
           ,[ExPickingSlipNo]
           ,[ExPickingSlipDate]
           ,[Status]
           ,[PrintSeq]
           ,[TypeOfGoods]
           ,[isDropsign]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastUpdateBy]
           ,[LastUpdateDate]
           ,[isLocked]
           ,[LockingBy]
           ,[LockingDate])

SELECT 
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, @TempDocNo DocNo 
	, @DocDate DocDate
	, @JobOrderNo UsageDocNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) UsageDocDate
	, (SELECT CustomerCode FROM SvTrnService WHERE 1 = 1AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCode
	, (SELECT CustomerCodeBill FROM SvTrnService WHERE 1 = 1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeBill
	, (SELECT CustomerCode FROM SvTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeShip
	, CONVERT(BIT, 0) isBO
	, CONVERT(BIT, 0) isSubstitution
	, CONVERT(BIT, 1) isIncludePPN
	, @TransType TransType
	, '2' SalesType
	, CONVERT(BIT, 0) isPORDD
	, @JobOrderNo OrderNo
	, @DocDate OrderDate
	, ISNULL((SELECT TOPCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') TOPCode
	, ISNULL((SELECT ParaValue FROM GnMstLookUpDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND CodeID = 'TOPC' AND 
		LookupValue IN 
		(SELECT TOPCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)
	  ),0) TOPDays
	, ISNULL((SELECT PaymentCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') PaymentCode
	, '' PaymentReffNo
	, 0 TotSalesQty
	, 0 TotSalesAmt
	, 0 TotDiscAmt
	, 0 TotDPPAmt
	, 0 TotPPNAmt
	, 0 TotFinalSalesAmt
	, CONVERT(BIT, 0) isPKP
	, NULL ExPickingSlipNo
	, NULL ExPickingSlipDate
	, '4' Status
	, 0 PrintSeq
	, @TypeOfGoods TypeOfGoods
	, NULL IsDropSign
	, @UserID CreatedBY
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate


UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'SSS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT DETAIL
-- ==============================================================================================================================
DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

declare @TempAvailStock as table
(
	PartNo varchar(50),
	AvailStock decimal
)

DECLARE @Query AS VARCHAR(MAX)
--SET @Query = 
--'SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
--FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
--WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+''

--INSERT INTO #TempAvailStock

SET @Query = 
'SElect * into #TempAvailStock from (SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+')#TempAvailStock

INSERT INTO SpTrnSORDDtl 
(
	[CompanyCode] ,
	[BranchCode] ,
	[DocNo] ,
	[PartNo] ,
	[WarehouseCode] ,
	[PartNoOriginal] ,
	[ReferenceNo] ,
	[ReferenceDate] ,
	[LocationCode] ,
	[QtyOrder] ,
	[QtySupply] ,
	[QtyBO] ,
	[QtyBOSupply] ,
	[QtyBOCancel] ,
	[QtyBill] ,
	[RetailPriceInclTax] ,
	[RetailPrice] ,
	[CostPrice] ,
	[DiscPct] ,
	[SalesAmt] ,
	[DiscAmt] ,
	[NetSalesAmt] ,
	[PPNAmt] ,
	[TotSalesAmt] ,
	[MovingCode] ,
	[ABCClass] ,
	[ProductType] ,
	[PartCategory] ,
	[Status] ,
	[CreatedBy] ,
	[CreatedDate] ,
	[LastUpdateBy] ,
	[LastUpdateDate] ,
	[StockAllocatedBy] ,
	[StockAllocatedDate] ,
	[FirstDemandQty] )
SELECT
	''' + @CompanyCode +''' CompanyCode
	, ''' + @BranchCode +''' BranchCode
	, ''' + @TempDocNo +''' DocNo 
	, a.PartNo
	, ''00'' WarehouseCode
	, a.PartNo PartNoOriginal
	, ''' + @JobOrderNo +''' ReferenceNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = ''' + @CompanyCode +''' AND BranchCode = ''' + @BranchCode +'''
		AND ProductType = ''' + @ProductType +''' AND JobOrderNo = ''' + @JobOrderNo +''' ) ReferenceDate
	, (SELECT distinct LocationCode FROM ' + @DbMD +'..SpMstItemLoc WHERE CompanyCode = ''' + @CompanyMD +''' AND BranchCode = ''' + @BranchMD +''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo ) LocationCode
	, a.QtyOrder
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBO
	, 0 QtyBOSupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBOCancel
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice * 10 /100) RetailPriceIncltax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder * a.RetailPrice
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice
		END AS SalesAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice) * a.DiscPct/100)
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) * a.DiscPct/100)
		END AS DiscAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS NetSalesAmt
	, 0 PPNAmt
	,  CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS TotSalesAmt
	, (SELECT distinct MovingCode FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +''' ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +'''  AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''2'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
	, a.QtyOrder FirstDemandQty
FROM #SrvOrder a
WHERE a.TypeOfGoods = '+@TypeOfGoods +'


select top 10 * from spTrnSORDDtl order by createddate desc
--===============================================================================================================================
-- INSERT SO SUPPLY
-- ==============================================================================================================================

SELECT * INTO #TempSOSupply FROM (
SELECT
	'''+ @CompanyCode +''' CompanyCode
	, '''+ @BranchCode +''' BranchCode
	, '''+ @TempDocNo +''' DocNo 
	, 0 SupSeq
	, a.PartNo 
	, a.PartNo PartNoOriginal
	, '''' PickingSlipNo	
	, '''+ @JobOrderNo +''' ReferenceNo
	, '''+ CONVERT(varchar, @DefaultDate )+''' ReferenceDate
	, ''00'' WarehouseCode
	, (SELECT distinct LocationCode FROM '+ @DbMD+'..SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD+''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo) LocationCode
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, 0 QtyPicked
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice *10 /100) RetailPriceIncltax
	, a.RetailPrice
	, b.CostPrice
	, a.DiscPct
	, (SELECT distinct MovingCode FROM '+ @DbMD+'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +'''ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''1'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
FROM #SrvOrder a
inner join spMstItemPrice b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.PartNo = b.PartNo
WHERE a.TypeOfGoods = '+ @TypeOfGoods +'
)#TempSOSupply

INSERT INTO SpTrnSOSupply SELECT 
	CompanyCode,BranchCode,DocNo,SupSeq,PartNo,PartNoOriginal
	, ROW_NUMBER() OVER(ORDER BY PartNo) PTSeq,PickingSlipNo
	, ReferenceNo,ReferenceDate,WarehouseCode,LocationCode
	, QtySupply,QtyPicked,QtyBill,RetailPriceIncltax,RetailPrice,CostPrice
	, DiscPct,MovingCode,ABCClass,ProductType,PartCategory,Status
	, CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate
FROM #TempSOSupply WHERE QtySupply > 0

--===============================================================================================================================
-- UPDATE STATUS DETAIL BASED ON SUPPLY
--===============================================================================================================================

UPDATE SpTrnSORDDtl
SET Status = 4
FROM SpTrnSORDDtl a, #TempSOSupply b
WHERE 1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo
	
--===============================================================================================================================
-- UPDATE HISTORY DEMAND ITEM AND CUSTOMER
--===============================================================================================================================

UPDATE SpHstDemandItem 
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +'''
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandItem a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+ @TempDocNo +'''

UPDATE SpHstDemandCust
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +''' 
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandCust a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +'''
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +'''
	AND a.PartNo = b.PartNo
	AND a.CustomerCode = '''+ @CustomerCode +'''
	AND b.DocNo = '''+ @TempDocNo +'''

INSERT INTO SpHstDemandItem
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +''' Year
	, '''+ Convert(varchar,Month(GetDate())) +''' Month
	, PartNo
	, 1 DemandFreq
	, QtyOrder DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
 AND NOT EXISTS
( SELECT 1 FROM SpHstDemandItem WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND PartNo = a.PartNo
)

INSERT INTO SpHstDemandCust
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +'''  Year
	, '''+ Convert(varchar,Month(GetDate())) +'''  Month
	, '''+ @CustomerCode +''' CustomerCode
	, PartNo
	, 1 DemandFreq
	, (SELECT QtyOrder FROM SpTrnSORDDTl WITH (NOLOCK, NOWAIT) 
		WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
		AND DocNo = a.DocNo AND PartNo = a.PartNo) DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' and a.BranchCode= '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
AND NOT EXISTS
( SELECT PartNo FROM SpHstDemandCust WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +'''  
	AND PartNo = a.PartNo
)

--===============================================================================================================================
-- UPDATE LAST DEMAND DATE MASTER
--===============================================================================================================================

UPDATE '+@DbMD+'..SpMstItems 
SET LastDemandDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+@TempDocNo+'''

--===============================================================================================================================
-- UPDATE STOCK AND MOVEMENT
--===============================================================================================================================

UPDATE '+@DbMD+'..spMstItems
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo

UPDATE '+@DbMD+'..spMstItemloc
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItemLoc a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD +'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.WarehouseCode = '''+@WarehouseMD+'''
	AND a.PartNo = b.PartNo

INSERT INTO SpTrnIMovement
SELECT
	'''+@CompanyCode +''' CompanyCode
	, '''+@BranchCode +''' BranchCode
	, a.DocNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyCode +'''
		AND BranchCode = '''+ @BranchCode +''' AND DocNo = a.DocNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),'''+convert(varchar,getdate())+''') CreatedDate 
	, ''00'' WarehouseCode
	, (SELECT LocationCode FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode =  '''+@CompanyCode +'''
		AND BranchCode = '''+@BranchCode +''' AND DocNo = '''+@TempDocNo +''' AND PartNo = a.PartNo)
	  LocationCode
	, a.PartNo
	, ''OUT'' SignCode
	, ''SA-NPJUAL'' SubSignCode
	, a.QtySupply
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, '''+@UserID +''' CreatedBy
FROM #TempSOSupply a

--===============================================================================================================================
-- UPDATE SUPPLY SLIP TO SPK
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = '''+@CompanyCode +''' AND BranchCode = '''+@BranchCode+'''
		AND ProductType = '''+@ProductType +''' AND JobOrderNo = '''+@JobOrderNo+''')

SELECT * INTO #TempServiceItem FROM (
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1
	AND a.CompanyCode = '''+@CompanyCode+'''
	AND a.BranchCode = '''+@BranchCode+'''
	AND a.ProductType = '''+@ProductType+'''
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+@ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND (a.SupplySlipNo = '''' OR a.SupplySlipNo IS NULL)
) #TempServiceItem 

SELECT * INTO #TempServiceItemIns FROM( 
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1 
	AND a.CompanyCode = '''+ @CompanyCode +''' 
	AND a.BranchCode = '''+ @BranchCode +''' 
	AND a.ProductType = '''+ @ProductType +'''  
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+ @ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo) 
	AND (a.SupplySlipNo != '''' OR a.SupplySlipNo IS NOT NULL)
) #TempServiceItemIns


UPDATE svTrnSrvItem
SET SupplySlipNo = b.DocNo
	, SupplySlipDate = ISNULL((SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
							AND DocNo = b.DocNo),'''+Convert(varchar,@DefaultDate)+''')
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq
	
--===============================================================================================================================
-- INSERT NEW SRV ITEM BASED SUPPLY SLIP
--===============================================================================================================================
INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq + 1
	, 0 DemandQty
	, 0 SupplyQty
	, 0 ReturnQty
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DocNo SupplySlipNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND DocNo = a.DocNo) SupplySlipDate
	, NULL SSReturnNo
	, NULL SSReturnDate
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, a.DiscPct
FROM #TempServiceItemIns a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = '''+ @CompanyCode +'''
	AND a.BranchCode = '''+ @BranchCode +'''
	AND a.ProductType = '''+ @ProductType+'''


--===============================================================================================================================
DROP TABLE #TempServiceItem 
DROP TABLE #TempServiceItemIns
DROP TABLE #TempSOSupply'

EXEC(@query)

--select convert(xml,@query)


--===============================================================================================================================
-- INSERT SVSDMOVEMENT
--===============================================================================================================================

declare @md bit
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

if(@md = 0)
begin

 declare @QueryTemp as varchar(max)  
 
	set @Query ='insert into '+ @DbMD +'..svSDMovement
	select a.CompanyCode, a.BranchCode, a.DocNo, a.CreatedDate, a.PartNo
	, Seq = convert(integer, ROW_NUMBER() OVER (PARTITION BY a.ReferenceNo ORDER BY a.DocNo)) ,
	a.WarehouseCode, a.QtyOrder, a.QtySupply, a.DiscPct
	,isnull(((select RetailPrice from spTrnSORDDtl
			where CompanyCode = ''' + @CompanyCode + '''  and BranchCode = ''' + @BranchCode  + '''
			and DocNo = ''' + @TempDocNo + ''' and PartNo = a.PartNo) / 1.1 * 
			((100 - isnull((select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
				where CompanyCode = ''' + @CompanyCode + ''' and BranchCode = ''' + @BranchCode  + ''' and SupplierCode = dbo.GetBranchMD(''' + @CompanyCode + ''', ''' + @BranchCode  + ''') 
				and ProfitCenterCode = ''300''),0)) * 0.01)),0) CostPrice
	, a.RetailPrice, b.TypeOfGoods
	, '''+ @CompanyMD +''','''+ @BranchMD +''','''+ @WarehouseMD +''', p.RetailPriceInclTax, p.RetailPrice, p.CostPrice
	,''x'','''+ @producttype +''',''300'',''0'',''0'','''+ convert(varchar,GETDATE()) +''','''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	,'''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	from spTrnSORDDtl a 
	join spTrnSORDHdr b on a.CompanyCode = b.CompanyCode
	and a.BranchCode = b.BranchCode 
	and a.DocNo = b.DocNo
	join spmstitemprice p
	on p.PartNo = a.PartNo
	where p.CompanyCode = '''+ @CompanyCode +'''
	and p.branchcode = '''+ @BranchCode +'''
	and a.ReferenceNo = '''+ @JobOrderNo +''''+
	' and a.DocNo = ''' + @TempDocNo + '''';

	exec (@Query)
	print (@QUERY)

end

--===============================================================================================================================
-- INSERT PICKING HEADER AND DETAIL
--===============================================================================================================================

SET @MaxPickingList = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'PLS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempPickingList = ISNULL((SELECT 'PLS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxPickingList, 1), 6)),'PLS/YY/XXXXXX')

INSERT INTO SpTrnSPickingHdr 
SELECT 
	CompanyCode
	, BranchCode
	, @TempPickingList PickingSlipNo
	, GetDate() PickingSlipDate
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, '' PickedBy
	, CONVERT(BIT, 0) isBORelease
	, isSubstitution
	, isIncludePPN
	, TransType
	, SalesType
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, '' Remark
	, '0' Status
	, '0' PrintSeq
	, TypeOfGoods
	, CreatedBy
	, CreatedDate
	, LastUpdateBy
	, LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = (SELECT distinct DocNo FROM spTrnSORDDtl WHERE CompanyCode = @CompanyCode AND Branchcode = @BranchCode 
					AND DocNo = @TempDocNo AND QtySupply > 0)		

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'PLS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

-- ==============================================================================================================================
-- UPDATE SALES ORDER HEADER 
-- ==============================================================================================================================
UPDATE SpTrnSORDHdr
	SET ExPickingSlipNo = @TempPickingList,
		ExPickingSlipDate = ISNULL((SELECT PickingSlipDate FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode AND PickingSlipNo = @TempPickingList),'')
	
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo

UPDATE SpTrnSOSupply
	SET PickingSlipNo = @TempPickingList
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
-- ==============================================================================================================================
-- INSERT PICKING DETAIL
-- ==============================================================================================================================

INSERT INTO SpTrnSPickingDtl
SELECT 
	a.CompanyCode
	, a.BranchCode
	, @TempPickingList PickingSlipNo
	, '00' WarehouseCode
	, a.PartNo
	, a.PartNoOriginal
	, a.DocNo
	, b.DocDate 
	, a.ReferenceNo
	, a.ReferenceDate
	, a.LocationCode
	, a.QtySupply QtyOrder
	, a.QtySupply
	, a.QtySupply QtyPicked 
	, 0 QtyBill
	, a.RetailPriceInclTax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, a.SalesAmt
	, a.DiscAmt
	, a.NetSalesAmt
	, a.TotSalesAmt
	, a.MovingCode
	, a.ABCClass
	, a.ProductType
	, a.PartCategory
	, '' ExPickingSlipNo
	, @DefaultDate ExPickingSlipDate
	, CONVERT(BIT, 0) isClosed
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSORDDtl a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSORDHdr b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.DocNo = b.DocNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.DocNo = @TempDocNo
	AND a.QtySupply > 0


--================================================================================================================================
-- UPDATE AMOUNT HEADER
--================================================================================================================================
SELECT * INTO #TempHeader FROM (
SELECT 
	header.CompanyCode
	, header.BranchCode
	, header.DocNo
	, header.TotSalesQty
	, header.TotSalesAmt
	, header.TotDiscAmt
	, header.TotDPPAmt
	, floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100)) 
		TotPPNAmt
	, header.TotDPPAmt + floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100))
		TotFinalSalesAmt
FROM (
SELECT 
	CompanyCode
	, BranchCode
	, DocNo
	, SUM(QtyOrder) TotSalesQty
	, SUM(SalesAmt) TotSalesAmt
	, SUM(DiscAmt) TotDiscAmt
	, SUM(NetSalesAmt) TotDPPAmt
FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
GROUP BY CompanyCode
	, BranchCode
	, DocNo
) header ) #TempHeader

UPDATE SpTrnSORDHdr
SET 
	TotSalesQty = b.TotSalesQty
	, TotSalesAmt = b.TotSalesAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalSalesAmt = b.TotFinalSalesAmt
FROM SpTrnSORDHdr a, #TempHeader b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.DocNo = b.DocNo

DROP TABLE #TempHeader

FETCH NEXT FROM db_cursor INTO @TypeOfGoods
END
CLOSE db_cursor
DEALLOCATE db_cursor 

--===============================================================================================================================
-- Update Transdate
--===============================================================================================================================

update gnMstCoProfileSpare
set TransDate=getdate()
	, LastUpdateBy=@UserID
	, LastUpdateDate=getdate()
where CompanyCode = @CompanyCode AND BranchCode = @BranchCode

--===============================================================================================================================
-- DROP TABLE SECTION 
--===============================================================================================================================
DROP TABLE #SrvOrder
DROP TABLE #Item

--rollback tran
END
GO

--declare	@CompanyCode varchar(15)
--declare	@BranchCode  varchar(15)
--declare	@ProductType varchar(15)
--declare	@ServiceNo   int
--declare	@BillType    char(1)
--declare	@InvoiceNo   varchar(15)
--declare	@Remarks     varchar(max)
--declare	@UserID      varchar(15)

--set	@CompanyCode = '6159401000'
--set	@BranchCode  = '6159401001'
--set	@ProductType = '4W'
--set	@ServiceNo   = '53438'
--set	@BillType    = 'C'
--set	@InvoiceNo   = 'INC/15/002778'
--set	@Remarks     = 'REMARK 001'
--set	@UserID      = 'ws-s'

if object_id('uspfn_SvTrnInvoiceCreate') is not null
	drop procedure uspfn_SvTrnInvoiceCreate
GO
CREATE procedure [dbo].[uspfn_SvTrnInvoiceCreate]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@ServiceNo   int,
	@BillType    char(1),
	@InvoiceNo   varchar(15),
	@Remarks     varchar(max),
	@UserID      varchar(15)
as  

declare @errmsg varchar(max)
--raiserror ('test error',16,1);

DECLARE @CompanyMD AS VARCHAR(15)
DECLARE @BranchMD AS VARCHAR(15)
DECLARE @WarehouseMD AS VARCHAR(15)
DECLARE @DbMD AS VARCHAR(15)
declare @md bit

SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

select BillType as BillType
              from svTrnSrvTask
             where CompanyCode = @companycode
               and BranchCode  = @branchcode
               and ProductType = @productType
               and ServiceNo   = @serviceno
            union
            select BillType as BillType
              from svTrnSrvItem b
             where CompanyCode = @companycode
               and BranchCode  = @branchcode
               and ProductType = @productType
               and ServiceNo   = @serviceno
               and  (SupplyQty - ReturnQty) > 0


-- get data from service
select * into #srv from(
  select * from svTrnService
   where 1 = 1
     and CompanyCode = @CompanyCode
     and BranchCode  = @BranchCode
     and ProductType = @ProductType
     and ServiceNo   = @ServiceNo
 )#srv

 select * from #srv
 select * from svTrnSrvItem where serviceno = @serviceno
 select * from svTrnSrvTask where serviceno = @serviceno

-- get data from task
select * into #tsk from(
  select a.* from svTrnSrvTask a, #srv b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.BillType    = @BillType
 )#tsk

 select * from #tsk

-- get data from item
select * into #mec from(
  select a.* from svTrnSrvMechanic a, #tsk b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.OperationNo = b.OperationNo
     and a.OperationNo <> ''
 )#mec

 select * from #mec

-- get data from item
select * into #itm from(
  select a.* from svTrnSrvItem a, #srv b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.BillType    = @BillType
 )#itm

-- create temporary table detail
create table #pre_dtl(
	BillType char(1),
	TaskPartType char(1),
	TaskPartNo varchar(20),
	TaskPartQty numeric(10,2),
	SupplySlipNo varchar(20)
)

insert into #pre_dtl
select BillType, 'L', OperationNo, OperationHour, ''
  from #tsk

insert into #pre_dtl
select BillType, TypeOfGoods, PartNo
	 , sum(SupplyQty - ReturnQty)
	 , SupplySlipNo
  from #itm
 where BillType = @BillType
   and (SupplyQty - ReturnQty) > 0
 group by BillType, TypeOfGoods, PartNo, SupplySlipNo

-- insert to table svTrnInvoice
declare @CustomerCode varchar(20)
if @BillType = 'C'
begin
	set @CustomerCode = (select CustomerCodeBill from #srv)
end
else if @BillType = 'P'
begin
	set @CustomerCode = (select top 1 a.BillTo from svMstPackage a
				 inner join svMstPackageTask b
					on b.CompanyCode = a.CompanyCode
				   and b.PackageCode = a.PackageCode
				 inner join svMstPackageContract c
					on c.CompanyCode = a.CompanyCode
				   and c.PackageCode = a.PackageCode
				 inner join #srv d
					on d.CompanyCode = a.CompanyCode
				   and d.JobType = a.JobType
				   and d.ChassisCode = c.ChassisCode
				   and d.ChassisNo = c.ChassisNo)
end
else if @BillType in ('F', 'W', 'S')
begin
	set @CustomerCode = (select CustomerCode from svMstBillingType
				 where BillType in ('F', 'W', 'S')
				   and CompanyCode = @CompanyCode
				   and BillType = @BillType)
end
else
begin
	set @CustomerCode = (select CustomerCodeBill from #srv)
end

--set @CustomerCode = isnull((
--				select top 1 a.BillTo from svMstPackage a
--				 inner join svMstPackageTask b
--					on b.CompanyCode = a.CompanyCode
--				   and b.PackageCode = a.PackageCode
--				 inner join svMstPackageContract c
--					on c.CompanyCode = a.CompanyCode
--				   and c.PackageCode = a.PackageCode
--				 inner join #srv d
--					on d.CompanyCode = a.CompanyCode
--				   and d.JobType = a.JobType
--				   and d.ChassisCode = c.ChassisCode
--				   and d.ChassisNo = c.ChassisNo
--				), isnull((
--				select CustomerCode from svMstBillingType
--				 where BillType in ('F')
--				   and CompanyCode = @CompanyCode
--				   and BillType = @BillType
--				), isnull((select CustomerCodeBill from #srv), '')))


if ((select count(*) from #tsk) = 0 and (select count(*) from #itm) = 0)
begin
	drop table #srv
	drop table #tsk
	drop table #mec
	drop table #itm
	drop table #pre_dtl
	return
end

if (@CustomerCode = '')
begin
	set @errmsg = N'Customer Code Bill belum di define...'
				+ char(13) + 'Tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

select * into #cus from (
select a.CompanyCode, a.IsPkp, b.CustomerCode, b.LaborDiscPct, b.PartDiscPct, b.MaterialDiscPct, b.TopCode, b.TaxCode
  from gnMstCustomer a, gnMstCustomerProfitCenter b
 where 1 = 1
   and b.CompanyCode  = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.CompanyCode  = @CompanyCode
   and b.BranchCode   = @BranchCode
   and b.CustomerCode = @CustomerCode
   and b.ProfitCenterCode = '200'
)#cus

if (select count(*) from #cus) <> 1
begin
	set @errmsg = N'Customer ProfitCenter belum di define...'
				+ char(13) + 'Tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

declare @IsPKP bit
    set @IsPKP = isnull((
				 select IsPKP from gnMstCustomer
				  where CompanyCode  = @CompanyCode
				    and CustomerCode = @CustomerCode
				  ), 0)

declare @PPnPct decimal
    set @PPnPct = isnull((
				  select a.TaxPct
				    from gnMstTax a, #cus b
				   where 1 = 1
				     and b.TaxCode     = 'PPN'
				     and a.CompanyCode = b.CompanyCode
				     and a.TaxCode     = b.TaxCode
				  ), 0)

declare @PPhPct decimal
    set @PPhPct = isnull((
				  select a.TaxPct
				    from gnMstTax a, #cus b
				   where 1 = 1
				     and b.TaxCode     = 'PPH'
				     and a.CompanyCode = b.CompanyCode
				     and a.TaxCode     = b.TaxCode
				  ), 0)


-- Insert Into svTrnInvoice
-----------------------------------------------------------------------------------------
insert into svTrnInvoice(
  CompanyCode, BranchCode, ProductType
, InvoiceNo, InvoiceDate, InvoiceStatus
, FPJNo, FPJDate, JobOrderNo, JobOrderDate, JobType
, ServiceRequestDesc, ChassisCode, ChassisNo, EngineCode, EngineNo
, PoliceRegNo, BasicModel, CustomerCode, CustomerCodeBill, Odometer
, IsPKP, TOPCode, TOPDays, DueDate, SignedDate
, LaborDiscPct, PartsDiscPct, MaterialDiscPct, PphPct, PpnPct, Remarks
, PrintSeq, PostingFlag, IsLocked, CreatedBy, CreatedDate
) 
select
  @CompanyCode CompanyCode
, @BranchCode BranchCode
, @ProductType ProductType
, @InvoiceNo InvoiceNo
, getdate() InvoiceDate
, case @IsPKP
	when '0' then '1'
	else (case @BillType when 'F' then '0' when 'W' then '0' else '1' end)
  end as InvoiceStatus
, '' FPJNo
, null FPJDate
, (select JobOrderNo from #srv) JobOrderNo
, (select JobOrderDate from #srv) JobOrderDate
, (select JobType from #srv) JobType
, (select ServiceRequestDesc from #srv) ServiceRequestDesc
, (select ChassisCode from #srv) ChassisCode
, (select ChassisNo from #srv) ChassisNo
, (select EngineCode from #srv) EngineCode
, (select EngineNo from #srv) EngineNo
, (select PoliceRegNo from #srv) PoliceRegNo
, (select BasicModel from #srv) BasicModel
, (select CustomerCode from #srv) CustomerCode
, @CustomerCode as CustomerCodeBill
, (select Odometer from #srv) Odometer
, (select IsPKP from #cus) as IsPKP
, (select TopCode from #cus) as TOPCode
, isnull((
	select b.ParaValue
	  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
	 where a.CompanyCode  = @CompanyCode
	   and a.BranchCode   = @BranchCode
	   and a.CustomerCode = @CustomerCode
	   and a.ProfitCenterCode = '200'
	   and b.CompanyCode  = a.CompanyCode
	   and b.CodeID = 'TOPC'
	   and b.LookUpValue = a.TopCode
	), null) as TOPDays
, isnull((
	select dateadd(day, convert(int,b.ParaValue), convert(varchar, getdate(), 112))
	  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
	 where a.CompanyCode  = @CompanyCode
	   and a.BranchCode   = @BranchCode
	   and a.CustomerCode = @CustomerCode
	   and a.ProfitCenterCode = '200'
	   and b.CompanyCode  = a.CompanyCode
	   and b.CodeID = 'TOPC'
	   and b.LookUpValue  = a.TopCode
	), null) as DueDate
, convert(varchar, getdate(), 112) SignedDate
, case @BillType
	when 'F' then (select LaborDiscPct from #cus) 
    when 'W' then (select LaborDiscPct from #cus) 
    else (select LaborDiscPct from #srv) 
  end as LaborDiscPct
, case @BillType
	when 'F' then (select PartDiscPct from #cus) 
    when 'W' then (select PartDiscPct from #cus) 
    else (select PartDiscPct from #srv) 
  end as PartsDiscPct
, case @BillType
	when 'F' then (select MaterialDiscPct from #cus) 
    when 'W' then (select MaterialDiscPct from #cus) 
    else (select MaterialDiscPct from #srv) 
  end as MaterialDiscPct
, @PPnPct as PPhPct
, @PPnPct as PPnPct
, @Remarks as Remarks
, '0' PrintSeq
, '0' PostingFlag
, '0' IsLocked
, @UserID CreatedBy
, getdate() CreatedDate

-- Insert Into svTrnInvTask
-----------------------------------------------------------------------------------------
insert into svTrnInvTask (
  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
, OperationHour, ClaimHour, OperationCost, SubConPrice
, IsSubCon, SharingTask, DiscPct
)
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
, isnull(OperationHour, 0) OperationHour, isnull(ClaimHour, 0) ClaimHour
, isnull(OperationCost, 0) OperationCost, isnull(SubConPrice, 0) SubConPrice
, isnull(IsSubCon, 0) IsSubCon, isnull(SharingTask, 0) SharingTask
, isnull(DiscPct, 0)
from #tsk

-- Insert Into svTrnInvMechanic
-----------------------------------------------------------------------------------------
insert into svTrnInvMechanic (
  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
, MechanicID, ChiefMechanicID, StartService, FinishService
)
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
, MechanicID, ChiefMechanicID, StartService, FinishService
from #mec

-- Insert Into svTrnInvItem
-----------------------------------------------------------------------------------------
Declare @Query varchar(max)

set @Query = 'select * into #itm1 from (
select CompanyCode, BranchCode, ProductType, '''+ @InvoiceNo +''' as InvoiceNo, PartNo
	 , isnull((
		select MovingCode from '+ @DbMD +'..spMstItems
		 where CompanyCode = '''+ @CompanyMD +'''
		   and BranchCode = '''+ @BranchMD +'''
		   and PartNo = #itm.PartNo
		), '''') as MovingCode
	 , isnull((
		select ABCClass from '+ @DbMD +' ..spMstItems
		 where CompanyCode = '''+ @CompanyMD +'''
		   and BranchCode = '''+ @BranchMD +'''
		   and PartNo = #itm.PartNo
		), '''') as ABCClass
	 , sum(SupplyQty - ReturnQty) as SupplyQty
	 , isnull((
		select 
		  case (sum(b.SupplyQty - b.ReturnQty))
			 when 0 then 0
			 else sum(a.CostPrice * (b.SupplyQty - b.ReturnQty)) / sum(b.SupplyQty - b.ReturnQty)
		  end 
	from SpTrnSLmpDtl a
	left join SvTrnSrvItem b on 1 = 1
	 and b.CompanyCode  = a.CompanyCode
	 and b.BranchCode   = a.BranchCode
	 and b.ProductType  = a.ProductType
	 and b.SupplySlipNo = a.DocNo
	 and b.PartNo = #itm.PartNo
	where 1 = 1
	 and a.CompanyCode = '''+ @CompanyCode +'''
	 and a.BranchCode  = '''+ @BranchCode +'''
	 and a.ProductType = '''+ @ProductType +'''
	 and a.PartNo = #itm.PartNo
	 and a.DocNo in (
			select SupplySlipNo
			 from SvTrnSrvItem
			where 1 = 1
			  and CompanyCode = '''+ @CompanyCode +'''
			  and BranchCode  = '''+ @BranchCode +'''
			  and ProductType = '''+ @ProductType +'''
			  and ServiceNo = '''+ Convert(varchar,@ServiceNo) +'''
			  and PartNo = #itm.PartNo
			)
	), 0) as CostPrice
, RetailPrice
, TypeOfGoods
from #itm
group by CompanyCode, BranchCode, ProductType, PartNo, RetailPrice, TypeOfGoods
)#

insert into svTrnInvItem (
  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo
, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice
, TypeOfGoods, DiscPct
)
select a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo
	 , MovingCode = (select top 1 MovingCode from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , ABCClass = (select top 1 ABCClass from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , sum(SupplyQty) as SupplyQty, 0 as ReturnQty
	 , CostPrice = (select top 1 CostPrice from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by CostPrice desc)
	 , RetailPrice = (select top 1 RetailPrice from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by RetailPrice desc)
	 , TypeOfGoods = (select top 1 TypeOfGoods from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , DiscPct = (select top 1 DiscPct from #itm where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
  from #itm1 a
 where a.SupplyQty > 0
 group by a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo'

 exec(@Query)

-- Insert Into svTrnInvItemDtl
-----------------------------------------------------------------------------------------
insert into svTrnInvItemDtl (
  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, SupplySlipNo
, SupplyQty, CostPrice, CreatedBy, CreatedDate
)
select y.* from (
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, PartNo, SupplySlipNo
, sum(SupplyQty - ReturnQty) as SupplyQty, CostPrice
, @UserID as CreatedBy, getdate() as CreatedDate
from #itm
group by CompanyCode, BranchCode, ProductType, PartNo, SupplySlipNo, CostPrice
) y
where y.SupplyQty > 0

-- Re Calculate Invoice

-----------------------------------------------------------------------------------------
exec uspfn_SvTrnInvoiceReCalculate @CompanyCode=@CompanyCode, @BranchCode=@BranchCode, @ProductType=@ProductType, @InvoiceNo=@InvoiceNo, @UserId=@UserId
-- Insert svsdmovement
-----------------------------------------------------------------------------------------

 if(@md = 0)
 begin

 set @Query ='insert into '+ @DbMD +'..svSDMovement
select a.CompanyCode, a.BranchCode, '''+ convert(varchar,@InvoiceNo) +''','''+ convert(varchar,GETDATE()) +''', a.PartNo
, Seq = convert(integer, ROW_NUMBER() OVER (PARTITION BY a.ServiceNo order by a.ServiceNo)) ,
''00'', a.DemandQty, a.DemandQty, a.DiscPct, a.CostPrice, a.RetailPrice, a.TypeOfGoods
, '''+ @CompanyMD +''','''+ @BranchMD +''','''+ @WarehouseMD +''',p.RetailPriceInclTax,p.RetailPrice,p.CostPrice
,''x'','''+ @producttype +''',''300'',''8'',''0'','''+ convert(varchar,GETDATE()) +''','''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
,'''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
from svTrnSrvItem a 
join spmstitemprice p
on p.PartNo = a.PartNo
where p.CompanyCode = '''+ @CompanyCode +'''
and p.branchcode = '''+ @BranchCode +'''
and a.ServiceNo = '''+ convert(varchar,@ServiceNo) +''''

exec (@Query)

end

drop table #srv
drop table #tsk
drop table #mec
drop table #itm
drop table #cus

drop table #pre_dtl
--rollback tran

GO
ALTER procedure [dbo].[uspfn_SvTrnJobOrderCreate]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@ServiceNo bigint,
	@UserID varchar(15)
as      

declare @errmsg varchar(max)

begin try
	begin transaction
		declare @docseq int 
        set @docseq = isnull((
			select DocumentSequence from gnMstDocument 
			 where 1 = 1
			   and CompanyCode  = @CompanyCode
			   and BranchCode   = @BranchCode
			   and DocumentType = 'SPK'),0) + 1
        declare @JobOrderNo varchar(15)
		set @JobOrderNo = 'SPK/' + (select right(convert(char(4),getdate(),112),2)) + '/' 
                                 + right((replicate('0',6) + (select convert(varchar, @docseq))),6)
		update svTrnService
		   set ServiceType    = '2'
              ,JobOrderNo     = @JobOrderNo
              ,JobOrderDate   = getdate()
              ,LastUpdateBy   = @UserID
              ,LastUpdateDate = getdate()
		 where 1 = 1
		   and CompanyCode = @CompanyCode
		   and BranchCode  = @BranchCode
		   and ProductType = @ProductType
		   and ServiceNo   = @ServiceNo
		update gnMstDocument 
		   set DocumentSequence = @docseq
              ,LastUpdateBy     = @UserID
              ,LastUpdateDate   = getdate()
		 where 1 = 1
		   and CompanyCode  = @CompanyCode
		   and BranchCode   = @BranchCode
		   and DocumentType = 'SPK'
	commit transaction

	exec uspfn_SvInsertDefaultTaskMovement @CompanyCode, @BranchCode, @ProductType, @ServiceNo, @UserID

end try
begin catch
	rollback transaction
	set @errmsg = N'tidak dapat konversi ke SPK pada ServiceNo = '
				+ convert(varchar,@ServiceNo)
				+ char(13) + error_message()
	raiserror (@errmsg,16,1);
end catch
GO

ALTER procedure [dbo].[usprpt_OmRpSalesTrn007]
	-- Add the parameters for the stored procedure here
	@CompanyCode VARCHAR(15),
	@BranchCode	 VARCHAR(15),
	@ReqNoA		 VARCHAR(15),
	@ReqNoB		 VARCHAR(15)

AS

DECLARE
	@QRYTmp		AS varchar(max),
	@DBMD		AS varchar(25),
	@CompanyMD  AS varchar(25)


BEGIN

set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


set @QRYTmp =
'SELECT
	row_number () OVER (ORDER BY a.ReqNo) AS No
	, a.ReqNo
	, a.SKPKNo
	, a.FakturPolisiNo
	, ISNULL(c.SuzukiDONo, '''') DONo
	, (SELECT dbo.GetDateIndonesian (a.FakturPolisiDate)) AS ''Tanggal''
	, ISNULL(c.SuzukiDODate, '''') DODate
	, ISNULL(d.CompanyName, '''') CompanyName
	, ISNULL(d.Address1, '''') CoAdd1
	, ISNULL(d.Address2, '''') CoAdd2
	, ISNULL(d.Address3, '''') CoAdd3
	, case d.ProductType 
		when ''2W'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''4W'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		when ''A'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''B'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		else ''Harap dibuatkan Faktur untuk SUZUKI :''
		end as Note
	, ISNULL(c.SuzukiSJNo, '''') SJNo
	, ISNULL(c.SuzukiSJDate, '''') SJDate
	, ISNULL(c.SalesModelCode, '''') Model
	, ISNULL(f.SalesModelDesc, '''') ModelDesc
	, ISNULL(g.RefferenceDesc1, '''') Warna
	, ISNULL(c.SalesModelYear, 0) Tahun
	, a.ChassisNo
	, ISNULL(c.EngineNo, 0) EngineNo
	, ((CASE ISNULL(a.DealerCategory, '''') WHEN ''M'' THEN ''Main Dealer'' WHEN ''S'' THEN ''Sub Dealer'' WHEN ''R'' THEN ''Show Room'' END) + '' / '' + h.CustomerName) AS  Penjual
	, a.SalesmanName
	, a.SKPKName
	, a.SKPKAddress1 Alamat1
	, a.SKPKAddress2 Alamat2
	, a.SKPKAddress3 Alamat3
	, ISNULL(i.LookUpValueName, '''') City
	, a.SKPKTelp1
	, a.SKPKTelp2
	, a.SKPKHP
	, ISNULL(a.SKPKBirthday, '''') SKPKDay
	, a.FakturPolisiName
	, a.FakturPolisiAddress1
	, a.FakturPolisiAddress2
	, a.FakturPolisiAddress3
	, a.FakturPolisiTelp1
	, a.FakturPolisiTelp2
	, a.FakturPolisiHP
	, a.FakturPolisiBirthday
	, (select ISNULL(LookUpValueName, '''') from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID=''FPCT'' and LookUpValue=a.DealerCategory
		) AS DealerCategory
	, ISNULL(b.Remark, '''') Remark
	, ISNULL(UPPER(z.SignName), '''') AS SignName1
	, ISNULL(UPPER(z.TitleSign), '''') AS TitleSign1 
	, a.IDNo
FROM
 omTrSalesReqDetail a
JOIN
 omTrSalesReq b ON b.CompanyCode=a.CompanyCode AND b.BranchCode=a.BranchCode
 AND b.ReqNo=a.ReqNo 
LEFT JOIN
 ' + @DBMD + '..omMstVehicle c ON c.CompanyCode=''' + @CompanyMD + ''' 
 AND c.ChassisCode=a.ChassisCode
 AND c.ChassisNo=a.ChassisNo
LEFT JOIN
 gnMstCoProfile d ON d.CompanyCode=a.CompanyCode AND d.BranchCode=a.BranchCode
LEFT JOIN
 ' + @DBMD + '..omMstModel f ON f.CompanyCode=''' + @CompanyMD + ''' 
 AND f.SalesModelCode=c.SalesModelCode
LEFT JOIN
 ' + @DBMD + '..omMstRefference g ON g.CompanyCode=''' + @CompanyMD + '''
  AND g.RefferenceType=''COLO''
 AND g.RefferenceCode=c.ColourCode
LEFT JOIN
 gnMstCustomer h ON h.CompanyCode=b.CompanyCode AND h.CustomerCode=b.SubDealerCode
LEFT JOIN
 gnMstLookUpDtl i ON i.CompanyCode=a.CompanyCode AND i.CodeID=''CITY'' 
 AND i.LookUpValue=a.SKPKCity
LEFT JOIN gnMstSIgnature z
	ON z.CompanyCode = a.CompanyCode
	AND z.BranchCode = a.BranchCode
	AND z.ProfitCenterCode = ''100''
	AND z.DocumentType = ''RFP''
	AND z.SeqNo = 1
WHERE
 a.CompanyCode	  = ''' + @CompanyCode + '''
 AND a.BranchCode = ''' + @BranchCode + '''
 AND a.ReqNo BETWEEN ''' + @ReqNoA + ''' AND ''' + @ReqNoB + '''
ORDER BY ReqNo'

Exec (@QRYTmp);

END
GO

IF OBJECT_ID('usprpt_abInqTurnOverRatio') IS NOT NULL
	DROP PROCEDURE usprpt_abInqTurnOverRatio
GO
-- CREATED BY Benedict 15-Apr-2015

CREATE PROCEDURE usprpt_abInqTurnOverRatio
	@Area varchar(15),
	@DealerCode varchar(15),
	@OutletCode varchar(15),
	@Start date,
	@End date,
	@Position varchar(5)
AS BEGIN

--DECLARE @Area varchar(15) = '105',
--	@DealerCode varchar(15) = '',
--	@OutletCode varchar(15) = '',
--	@Start date = '2013-11-30',
--	@End date = '2013-12-31',
--	@Position varchar(5) = ''

DECLARE @ITSG TABLE (Value int, Name varchar(15))
INSERT INTO @ITSG (Value, Name) 
	SELECT LookUpValue, LookUpValueName 
	FROM gnMstLookUpDtl WHERE CompanyCode = CASE @DealerCode WHEN '' THEN CompanyCode ELSE @DealerCode END AND CodeID = 'ITSG'

DECLARE @Platinum int = (SELECT Value FROM @ITSG WHERE Name = 'Platinum'),
	@Gold int = (SELECT Value FROM @ITSG WHERE Name = 'Gold'),
	@Silver int = (SELECT Value FROM @ITSG WHERE Name = 'Silver'),
	@Trainee int = (SELECT Value FROM @ITSG WHERE Name = 'Trainee')

SELECT * INTO #poolStart FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MAX(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @Start
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND c.Position = CASE @Position WHEN '' THEN c.Position ELSE @Position END
		AND c.JoinDate <= @Start
		AND (c.ResignDate IS NULL OR c.ResignDate > @Start)
		AND d.MutationDate <= @Start
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
)#poolStart

SELECT * INTO #poolEnd FROM (
	SELECT d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		e.Position, e.Grade, 
		c.JoinDate, e.AssignDate, MAX(d.MutationDate) AS MutationDate, c.ResignDate
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON d.CompanyCode = d.CompanyCode AND c.EmployeeId = d.EmployeeId
		JOIN (SELECT a.* 
				FROM HrEmployeeAchievement a
				INNER JOIN (SELECT f.CompanyCode, f.EmployeeID, MAX(f.AssignDate) AS AssignDate
							FROM HrEmployeeAchievement f 
							WHERE f.AssignDate <= @End
							GROUP BY f.CompanyCode, f.EmployeeID) b
				ON a.CompanyCode = b.CompanyCode
				AND a.EmployeeID = b.EmployeeID
				AND a.AssignDate = b.AssignDate
				WHERE Department = 'SALES'
				) e
		ON c.CompanyCode = e.CompanyCode AND c.EmployeeId = e.EmployeeId
		WHERE d.CompanyCode = CASE @DealerCode WHEN '' THEN d.CompanyCode ELSE @DealerCode END
		AND d.BranchCode = CASE @OutletCode WHEN '' THEN d.BranchCode ELSE @OutletCode END
		AND c.Department = 'SALES' 
		AND c.Position = CASE @Position WHEN '' THEN c.Position ELSE @Position END
		AND c.JoinDate <= @End
		AND (c.ResignDate IS NULL OR c.ResignDate > @End)
		AND d.MutationDate <= @End
		GROUP BY d.CompanyCode, d.BranchCode, c.EmployeeId, c.EmployeeName, 
		c.JoinDate, c.ResignDate, e.AssignDate, e.Position, e.Grade
)#poolEnd

SELECT * INTO #employeeOut FROM (
	SELECT * FROM #poolStart a 
	WHERE a.EmployeeID NOT IN (SELECT b.EmployeeID 
								FROM #poolEnd b 
								WHERE b.CompanyCode = a.CompanyCode
								AND b.BranchCode = a.BranchCode)
) #employeeOut

SELECT * INTO #employeeIn FROM (
	SELECT * FROM #poolEnd a
	WHERE a.EmployeeID NOT IN (SELECT b.EmployeeID
								FROM #poolStart b
								WHERE b.CompanyCode = a.CompanyCode
								AND b.BranchCode = a.BranchCode)
) #employeeIn

SELECT * INTO #employeeStay FROM(
	SELECT * FROM #poolStart a
	WHERE a.EmployeeID IN (SELECT b.EmployeeID
							FROM #poolEnd b
							WHERE b.CompanyCode = a.CompanyCode
							AND b.BranchCode = a.BranchCode)
) #employeeStay
				
SELECT * INTO #sum1 FROM(
	SELECT a.CompanyCode, a.BranchCode, COUNT(*) AS EmployeeCount,
		SUM(CASE a.Grade WHEN @Platinum THEN 1 ELSE 0 END) AS Platinum,
		SUM(CASE a.Grade WHEN @Gold THEN 1 ELSE 0 END) AS Gold,
		SUM(CASE a.Grade WHEN @Silver THEN 1 ELSE 0 END) AS Silver,
		SUM(CASE a.Grade WHEN @Trainee THEN 1 ELSE 0 END) AS Trainee
	FROM #poolStart a 
	GROUP BY a.CompanyCode, a.BranchCode
) #sum1

SELECT * INTO #sum2 FROM (
	SELECT a.CompanyCode, a.BranchCode, COUNT(*) AS EmployeeCount,
		SUM(CASE a.Grade WHEN @Platinum THEN 1 ELSE 0 END) AS Platinum,
		SUM(CASE a.Grade WHEN @Gold THEN 1 ELSE 0 END) AS Gold,
		SUM(CASE a.Grade WHEN @Silver THEN 1 ELSE 0 END) AS Silver,
		SUM(CASE a.Grade WHEN @Trainee THEN 1 ELSE 0 END) AS Trainee
	FROM #poolEnd a 
	GROUP BY a.CompanyCode, a.BranchCode
) #sum2

--select * from #pool
--select * from #poolStart
--select * from #poolEnd
--select * from #employeeOut
--select * from #employeeIn
--select * from #employeeStay
--select * from #sum1
--select * from #sum2

SELECT 
	a.CompanyCode, 
	a.BranchCode,
	c.DealerName, 
	d. OutletName, 
		(CONVERT(decimal(6,2), (SELECT COUNT(*) 
								FROM #employeeStay e
								WHERE e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode))
		/ CONVERT(decimal(6,2), a.EmployeeCount)) 
	AS Ratio, 
	a.EmployeeCount AS StartEmployeeCount,
		(SELECT COUNT(*) FROM #employeeIn f WHERE f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode)
	AS EmployeeIn,
	a.Platinum AS StartPlatinum, 
	a.Gold AS StartGold, 
	a.Silver AS StartSilver, 
	a.Trainee AS StartTrainee,
	b.EmployeeCount AS EndEmployeeCount,
		(SELECT COUNT(*) FROM #employeeOut g WHERE g.CompanyCode = a.CompanyCode AND g.BranchCode = a.BranchCode) 
	AS EmployeeOut,
	b.Platinum AS EndPlatinum, 
	b.Gold AS EndGold, 
	b.Silver AS EndSilver, 
	b.Trainee AS EndTrainee
FROM #sum1 a
JOIN #sum2 b 
	ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
JOIN gnMstDealerMapping c 
	ON c.DealerCode = a.CompanyCode
JOIN gnMstDealerOutletMapping d 
	ON d.DealerCode = a.CompanyCode AND d.OutletCode = a.BranchCode

DROP TABLE #poolStart
DROP TABLE #poolEnd
DROP TABLE #employeeOut
DROP TABLE #employeeIn
DROP TABLE #employeeStay
DROP TABLE #sum1
DROP TABLE #sum2

END
GO

if object_id('uspfn_pmSelectOrganizationTree') is not null
	drop procedure uspfn_pmSelectOrganizationTree
GO
--created by BENEDICT 11/Mar/2015 LAST UPDATED on 14/Apr/2015

CREATE PROCEDURE uspfn_pmSelectOrganizationTree
@CompanyCode varchar(15),
@BranchCode varchar(15)

--DECLARE @CompanyCode varchar(15) = '6159401000'
--DECLARE @BranchCode varchar(15) = '6159401001'

AS BEGIN
SELECT * INTO #test1 FROM(
	SELECT a.BranchCode, a.EmployeeID, b.EmployeeName, e.PosLevel AS PositionID, b.Position, e.PosName AS PositionName,
	(rtrim(a.EmployeeID) + ' - ' + rtrim(b.EmployeeName)) Employee,
	isnull((
			select count(*) from PmKDP
			 where CompanyCode  = a.CompanyCode
			   and BranchCode   = a.BranchCode
			   and EmployeeID   = a.EmployeeID
			), 0) CountKDP, b.TeamLeader, ISNULL(f.OutletAbbreviation, a.BranchCode) AS BranchAbv, b.IsAssigned
	FROM hrEmployeeMutation a
	JOIN (
		SELECT c.EmployeeId, c.EmployeeName, c.Position, ISNULL(c.TeamLeader, '') AS TeamLeader, MAX(d.MutationDate) AS MutationDate, c.IsAssigned
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON c.EmployeeId = d.EmployeeId
		WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
		GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader, c.IsAssigned
	) b
	ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
	JOIN gnMstPosition e
	ON a.CompanyCode = e.CompanyCode AND e.DeptCode = 'SALES' AND b.Position = e.PosCode
	JOIN gnMstDealerOutletMapping f
	ON a.CompanyCode = f.DealerCode AND a.BranchCode = f.OutletCode
	WHERE a.CompanyCode = @CompanyCode AND a.BranchCode = CASE @BranchCode WHEN '' THEN a.BranchCode ELSE @BranchCode END
UNION
	SELECT a.BranchCode, a.EmployeeID, b.EmployeeName, e.PosLevel AS PositionID, b.Position, e.PosName AS PositionName,
	(rtrim(a.EmployeeID) + ' - ' + rtrim(b.EmployeeName)) Employee,
	isnull((
			select count(*) from PmKDP
			 where CompanyCode  = a.CompanyCode
			   and BranchCode   = a.BranchCode
			   and EmployeeID   = a.EmployeeID
			), 0) CountKDP, b.TeamLeader, ISNULL(f.OutletAbbreviation, a.BranchCode) AS BranchAbv, b.IsAssigned
	FROM hrEmployeeMutation a
	JOIN (
		SELECT c.EmployeeId, c.EmployeeName, c.Position, ISNULL(c.TeamLeader, '') AS TeamLeader, MAX(d.MutationDate) AS MutationDate, c.IsAssigned
		FROM hrEmployee c
		JOIN hrEmployeeMutation d
		ON c.EmployeeId = d.EmployeeId
		WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
		AND c.Position = 'GM'
		GROUP BY c.EmployeeId, c.EmployeeName, c.Position, c.TeamLeader, c.IsAssigned
	) b
	ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
	JOIN gnMstPosition e
	ON a.CompanyCode = e.CompanyCode AND e.DeptCode = 'SALES' AND b.Position = e.PosCode
	JOIN gnMstDealerOutletMapping f
	ON a.CompanyCode = f.DealerCode AND a.BranchCode = f.OutletCode
	WHERE a.CompanyCode = @CompanyCode
)#test1

;WITH N(id, lvl, BranchCode, EmployeeID, EmployeeName, PositionID, Position, PositionName, Employee, CountKDP, TeamLeader, BranchAbv, IsAssigned)
AS
(
	SELECT 
		CAST(row_number() OVER(ORDER BY a.EmployeeID) AS varchar) as id,
		0 AS lvl,
		a.BranchCode, a.EmployeeID, a.EmployeeName, a.PositionID, a.Position, a.PositionName, a.Employee, a.CountKDP, a.TeamLeader, a.BranchAbv, a.IsAssigned
	FROM #test1 a
	WHERE a.TeamLeader = ''
	AND a.IsAssigned = 1
	UNION ALL
	SELECT 
		CAST(N.id + '.' + CAST(row_number() OVER(ORDER BY b.EmployeeID) AS varchar) AS varchar) as id,
		N.lvl + 1 AS lvl,
		b.BranchCode, b.EmployeeID, b.EmployeeName, b.PositionID, b.Position, b.PositionName, b.Employee, b.CountKDP, b.TeamLeader, b.BranchAbv, b.IsAssigned
	FROM #test1 b JOIN N ON N.EmployeeID = b.TeamLeader
)
SELECT * FROM N ORDER BY lvl, PositionID DESC, BranchCode, id

DROP TABLE #test1
END
GO
if object_id('usprpt_PmRpInqPeriodeWeb') is not null
	drop procedure usprpt_PmRpInqPeriodeWeb
GO
create procedure [dbo].[usprpt_PmRpInqPeriodeWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@PeriodBegin		DATETIME,
	@PeriodEnd			DATETIME,
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@Salesman			VARCHAR(15)
)
AS 
BEGIN
SET NOCOUNT ON;
----

IF(@SalesHead ='' AND @Salesman ='')
BEGIN
	SELECT * INTO #empl1 FROM (
		--SH =ALL AND S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  
		a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader=@BranchManager)
	)#empl1

	SELECT * INTO #t1 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND a.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl1 g)
	) #t1

	DROP TABLE #empl1
	SELECT InquiryNumber, Pelanggan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
	Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail FROM #t1 ORDER BY InquiryNumber
	DROP TABLE #t1

END
ELSE IF(@Salesman = '')
BEGIN
	SELECT * INTO #empl2 FROM (
		--S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  
		a.TeamLeader =@SalesHead
	)#empl2

	SELECT * INTO #t2 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND a.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl2 g)
	) #t2

	DROP TABLE #empl2
	SELECT InquiryNumber, Pelanggan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
	Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail FROM #t2 ORDER BY InquiryNumber
	DROP TABLE #t2
END
ELSE
BEGIN
	SELECT * INTO #empl3 FROM (
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  
		a.EmployeeID=@Salesman
	)#empl3

	SELECT * INTO #t3 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND a.InquiryDate BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl3 g)
	) #t3

	DROP TABLE #empl3
	SELECT InquiryNumber, Pelanggan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
	Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail FROM #t3 ORDER BY InquiryNumber
	DROP TABLE #t3
END
----
END

GO

ALTER PROCEDURE [dbo].[usprpt_PmRpInqLostCaseWeb] 
@CompanyCode		VARCHAR(15) , --= '6159401000',
	@BranchCode			VARCHAR(15) , --= '6159401001',
	@PeriodBegin		VARCHAR(15) , --= '20140101',
	@PeriodEnd			VARCHAR(15) , --= '20140330',
	@BranchManager		VARCHAR(15) , --= '3-BIT',
	@SalesHead			VARCHAR(15) , --= '028',
	@SalesCoordinator	VARCHAR(15) , --= '028',
	@Salesman			VARCHAR(15) --= ''


AS
BEGIN
	SET NOCOUNT ON;
-- Get EmployeeID
--=======================================================================
DECLARE @SalesmanID		VARCHAR(MAX);

if @SalesHead = '' and @Salesman = ''
begin
set @SalesmanID = 'select EmployeeID from HrEmployee where TeamLeader in (
			select EmployeeID from HrEmployee where TeamLeader = '''+@BranchManager+''')
			and Department =''SALES'' and Position = ''S'' and PersonnelStatus = ''1'''
end
else if @SalesHead <> '' and @Salesman = ''
begin
set @SalesmanID = 'select EmployeeID from HrEmployee where TeamLeader  = '''+@SalesHead+'''
			and Department =''SALES'' and Position = ''S'' and PersonnelStatus = ''1'''
end
else
begin
set @SalesmanID = 'select EmployeeID from HrEmployee where EmployeeID  = '''+@Salesman+'''
			and Department =''SALES'' and Position = ''S'' and PersonnelStatus = ''1'''
end
--=======================================================================

-- Group By Tipe Kendaraan
--=======================================================================
DECLARE @ByTipeKendaraan		VARCHAR(MAX);
DECLARE @Query1		VARCHAR(MAX);

set @ByTipeKendaraan = 'select
	 a.CompanyCode, a.BranchCode, a.InquiryNumber, a.TipeKendaraan, a.EmployeeID, a.LastProgress
	from 
	 PMKDP a 
	where
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+convert(varchar(30),@PeriodBegin)+''' AND '''+convert(varchar(30),@PeriodEnd)+''')
	 and EmployeeID in ('+@SalesmanID+')
	 and a.TipeKendaraan <> '''''

set @Query1 = 'SELECT 
	DISTINCT(TipeKendaraan),
	(select count(*) from ('+@ByTipeKendaraan+') b where lastprogress <> ''LOST'' AND a.TipeKendaraan = b.TipeKendaraan) Active, 
	(select count(*) from ('+@ByTipeKendaraan+') b where lastprogress = ''LOST'' AND a.TipeKendaraan = b.TipeKendaraan) NonActive
	FROM ('+@ByTipeKendaraan+') a'
	
exec (@Query1)

-- Group By Perolehan Data
--======================================================================
DECLARE @ByPerolehanData		VARCHAR(MAX);
DECLARE @Query2		VARCHAR(MAX);

set @ByPerolehanData = 'select
	 a.CompanyCode, a.BranchCode, a.InquiryNumber, a.PerolehanData, a.EmployeeID, a.LastProgress
	from 
	 PMKDP a 
	where
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and EmployeeID in ('+@SalesmanID+')'

set @Query2 = 'SELECT 
	DISTINCT(PerolehanData),
	(select count(*) from ('+@ByPerolehanData+') b where lastprogress <> ''LOST'' AND a.PerolehanData = b.PerolehanData) Active, 
	(select count(*) from ('+@ByPerolehanData+') b where lastprogress = ''LOST'' AND a.PerolehanData = b.PerolehanData) NonActive
	FROM ('+@ByPerolehanData+') a'
	
exec (@Query2)

-- Group By Salesman
--=====================================================================
DECLARE @BySalesman		VARCHAR(MAX);
DECLARE @Query3		VARCHAR(MAX);

set @BySalesman = 'select
	 a.CompanyCode, a.BranchCode, a.InquiryNumber, b.EmployeeName, a.EmployeeID, a.LastProgress
	from 
	 PMKDP a
	inner join HrEmployee b
		ON b.CompanyCode = a.CompanyCode and b.EmployeeID = a.EmployeeID
	where
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and a.EmployeeID in ('+@SalesmanID+')'

set @Query3 = 'SELECT 
	DISTINCT (EmployeeName) Karyawan,
	(select count(*) from ('+@BySalesman+') b where lastprogress <> ''LOST'' AND a.EmployeeID = b.EmployeeID) Active, 
	(select count(*) from ('+@BySalesman+') b where lastprogress = ''LOST'' AND a.EmployeeID = b.EmployeeID) NonActive
	FROM ('+@BySalesman+') a'
	
exec (@Query3)

-- Group By Sales Coordinator
--=====================================================================
DECLARE @BranchName		VARCHAR(MAX);
DECLARE @Query4		VARCHAR(MAX);
set @BranchName = 'SELECT 
	  a.CompanyCode, a.BranchCode, a.InquiryNumber, a.LastProgress, a.SpvEmployeeID, b.BranchName 
	 FROM PMKDP a 
	 LEFT JOIN GnMstOrganizationDtl b
	 ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
	 WHERE
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and a.EmployeeID in ('+@SalesmanID+')'

set @Query4 = 'SELECT 
	DISTINCT (BranchName) Supervisor,
	(select count(*) from ('+@BranchName+') b where lastprogress <> ''LOST'' AND a.BranchName = b.BranchName) Active, 
	(select count(*) from ('+@BranchName+') b where lastprogress = ''LOST'' AND a.BranchName = b.BranchName) NonActive
	FROM ('+@BranchName+') a'
	
exec (@Query4)

-- Group By Sales Head
--=====================================================================
--DECLARE @BranchName		VARCHAR(MAX);
DECLARE @Query5		VARCHAR(MAX);

set @BranchName = 'SELECT 
	  a.CompanyCode, a.BranchCode, a.InquiryNumber, a.LastProgress, a.SpvEmployeeID, b.BranchName 
	 FROM PMKDP a 
	 LEFT JOIN GnMstOrganizationDtl b
	 ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
	 WHERE
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and a.EmployeeID in ('+@SalesmanID+')'

set @Query5 = 'SELECT 
	DISTINCT (BranchName) SalesHead,
	(select count(*) from ('+@BranchName+') b where lastprogress <> ''LOST'' AND a.BranchName = b.BranchName) Active, 
	(select count(*) from ('+@BranchName+') b where lastprogress = ''LOST'' AND a.BranchName = b.BranchName) NonActive
	FROM ('+@BranchName+') a'
	
exec (@Query5)

-- Group By Branch Name
--=======================================================================
--DECLARE @BranchName		VARCHAR(MAX);
DECLARE @Query6		VARCHAR(MAX);

set @BranchName = 'SELECT 
	  a.CompanyCode, a.BranchCode, a.InquiryNumber, a.LastProgress, a.SpvEmployeeID, b.BranchName 
	 FROM PMKDP a 
	 LEFT JOIN GnMstOrganizationDtl b
	 ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
	 WHERE
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and a.EmployeeID in ('+@SalesmanID+')'

set @Query3 = 'SELECT 
	DISTINCT (BranchName),
	(select count(*) from ('+@BranchName+') b where lastprogress <> ''LOST'' AND a.BranchName = b.BranchName) Active, 
	(select count(*) from ('+@BranchName+') b where lastprogress = ''LOST'' AND a.BranchName = b.BranchName) NonActive
	FROM ('+@BranchName+') a'
	
exec (@Query3)

-- Query Utama
--=======================================================================================
DECLARE @Utama		VARCHAR(MAX);

set @Utama = 'SELECT
 a.CompanyCode, a.BranchCode, a.InquiryNumber, a.NamaProspek, a.Inquirydate, ISNULL(a.TipeKendaraan,''-'') TipeKendaraan, 
 ISNULL(a.Variant,''-'') Variant, ISNULL(a.Transmisi,''-'') Transmisi,
 b.RefferenceDesc1 Warna, a.PerolehanData, c.EmployeeName Employee, d.EmployeeName Supervisor,
 a.LastProgress, e.UpdateDate TglLost, f.LookUpValueName KategoriLost, g.LookUpValueName Reason,
 a.LostCaseVoiceofCustomer VOC, a.SpvEmployeeID
FROM
 PmKDP a
LEFT JOIN OmMstRefference b
ON b.CompanyCode = a.CompanyCode AND b.RefferenceType = ''COLO'' AND b.RefferenceCode = a.ColourCode
LEFT JOIN HrEmployee c
ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
LEFT JOIN HrEmployee d
ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
LEFT JOIN PmStatusHistory e
ON e.InquiryNumber = a.InquiryNumber AND e.CompanyCode = a.CompanyCode 
AND e.BranchCode = a.BranchCode AND e.SequenceNo = (SELECT TOP 1 SequenceNo FROM PmStatusHistory
		WHERE InquiryNumber = a.InquiryNumber AND CompanyCode = a.CompanyCode 
		AND BranchCode = a.BranchCode AND LastProgress=''LOST'' ORDER BY SequenceNo DESC)
LEFT JOIN GnMstLookUpDtl f
ON f.CompanyCode = a.CompanyCode AND f.CodeID = ''PLCC'' AND f.LookUpValue = a.LostCaseCategory
LEFT JOIN GnMstLookUpDtl g
ON g.CompanyCode = a.CompanyCode AND g.CodeID = ''ITLR'' AND g.LookUpValue = a.LostCaseReasonID
WHERE
 a.CompanyCode = '''+@CompanyCode+''' 
 AND a.BranchCode = '''+@BranchCode+'''
 AND a.LastProgress = ''LOST'' 
 AND CONVERT(VARCHAR, e.UpdateDate, 112) BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''' 
 AND a.EmployeeID in ('+@SalesmanID+')'

Exec (@Utama)

-- Pivot
--=====================================================================
declare	@columns			VARCHAR(MAX)
declare	@columns2			VARCHAR(MAX)
declare	@Pivot				VARCHAR(MAX)

select	@columns = coalesce(@columns + ',[' + cast(LookUpValue as varchar) + ']',
				'[' + cast(LookUpValue as varchar)+ ']') 
		,@columns2 = coalesce(@columns2 + ',isnull([' + cast(LookUpValue as varchar) + '],0) as '+ LookUpValue +'',
		'isnull([' + cast(LookUpValue as varchar)+ '],0) as '+ LookUpValue +'')
from
(
	select	a.LookUpValue
	from	gnMstLookUpDtl a
	where	CompanyCode=@CompanyCode and CodeID='PLCC'
) as x

set @Pivot='
select 
	p.TipeKendaraan, '+ @columns2 +'
from (
	select 
		a.TipeKendaraan
		,d.LookupValue LostCaseCategory
		,count(d.LookupValue) Quantity
	from 
		pmKDP a	
	left join PmStatusHistory b
	on b.InquiryNumber = a.InquiryNumber AND b.CompanyCode = a.CompanyCode 
	and b.BranchCode = a.BranchCode and b.SequenceNo = (select top 1 SequenceNo from PmStatusHistory
			where InquiryNumber = a.InquiryNumber and CompanyCode = a.CompanyCode 
			and BranchCode = a.BranchCode and LastProgress=''LOST'' order by SequenceNo desc)
	left join gnMstLookUpDtl d on d.CompanyCode=a.CompanyCode and CodeID=''PLCC'' 
		and LookUpValue=a.LostCaseCategory
	where
		a.LastProgress= ''LOST''
		and a.EmployeeID in ('+@SalesmanID+')
		and convert(varchar, b.UpdateDate, 112) between '''+@PeriodBegin+''' and '''+@PeriodEnd+''' 
	group by
		a.CompanyCode,a.BranchCode,d.LookupValue,a.TipeKendaraan
) as b
pivot
(
	sum(Quantity)
	for LostCaseCategory 
	in ('+@columns+')
) as p
order by p.TipeKendaraan'


exec(@Pivot)



-- Get GnMstLookUpDtl (Kategori Lost Case) 
--===========================
SELECT LookupValue+' : '+LookUpValueName AS Kategori, LookupValue FROM gnMstLookupDtl 
WHERE CompanyCode=@CompanyCode AND CodeID='PLCC'

END
GO

ALTER procedure [dbo].[usprpt_PmRpInqFollowUpWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@PeriodBegin		DATETIME,
	@PeriodEnd			DATETIME,
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@Salesman			VARCHAR(15)
)
AS 
BEGIN
SET NOCOUNT ON;
----

IF(@SalesHead ='' AND @Salesman ='')
BEGIN
	SELECT * INTO #empl1 FROM (
		--SH =ALL AND S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a 
		WHERE a.CompanyCode = @CompanyCode 
		AND a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader=@BranchManager)
		AND a.Department ='SALES'
		AND a.Position = 'S'
		AND a.PersonnelStatus = '1'
		--a.TeamLeader IN (SELECT b.EmployeeID FROM HrEmployee b WHERE b.TeamLeader IN
		--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader=@BranchManager))
	)#empl1

	SELECT * INTO #t1 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND e.NextFollowUpDate BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl1 g)
			--AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
			--AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
			--AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
	) #t1

	DROP TABLE #empl1
	SELECT InquiryNumber, Pelanggan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
	Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail FROM #t1 ORDER BY InquiryNumber
	DROP TABLE #t1

END
ELSE IF(@Salesman = '')
BEGIN
	SELECT * INTO #empl2 FROM (
		--S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a 
		WHERE a.CompanyCode = @CompanyCode 
		AND a.TeamLeader = @SalesHead
		AND a.Department ='SALES'
		AND a.Position = 'S'
		AND a.PersonnelStatus = '1'
		--a.TeamLeader IN (SELECT b.EmployeeID FROM HrEmployee b WHERE b.TeamLeader=@SalesHead)
	)#empl2

	SELECT * INTO #t2 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND e.NextFollowUpDate BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl2 g)
			--AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
			--AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
			--AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
	) #t2

	DROP TABLE #empl2
	SELECT InquiryNumber, Pelanggan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
	Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail FROM #t2 ORDER BY InquiryNumber
	DROP TABLE #t2
END
ELSE
BEGIN
	SELECT * INTO #empl3 FROM (
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a 
		WHERE  a.CompanyCode = @CompanyCode  
		AND a.EmployeeID=@Salesman
		AND a.Department ='SALES'
		AND a.Position = 'S'
		AND a.PersonnelStatus = '1'
	)#empl3

	SELECT * INTO #t3 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND e.NextFollowUpDate BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl3 g)
			--AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
			--AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
			--AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
	) #t3

	DROP TABLE #empl3
	SELECT InquiryNumber, Pelanggan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
	Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail FROM #t3 ORDER BY InquiryNumber
	DROP TABLE #t3
END
----
END
GO
ALTER procedure [dbo].[uspfn_SvTrnInvoiceCreate]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@ServiceNo   int,
	@BillType    char(1),
	@InvoiceNo   varchar(15),
	@Remarks     varchar(max),
	@UserID      varchar(15)
as  

declare @errmsg varchar(max)
--raiserror ('test error',16,1);

-- get data from service
select * into #srv from(
  select * from svTrnService
   where 1 = 1
     and CompanyCode = @CompanyCode
     and BranchCode  = @BranchCode
     and ProductType = @ProductType
     and ServiceNo   = @ServiceNo
 )#srv

-- get data from task
select * into #tsk from(
  select a.* from svTrnSrvTask a, #srv b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.BillType    = @BillType
 )#tsk

-- get data from item
select * into #mec from(
  select a.* from svTrnSrvMechanic a, #tsk b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.OperationNo = b.OperationNo
     and a.OperationNo <> ''
 )#mec

-- get data from item
select * into #itm from(
  select a.* from svTrnSrvItem a, #srv b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.BillType    = @BillType
 )#itm

-- create temporary table detail
create table #pre_dtl(
	BillType char(1),
	TaskPartType char(1),
	TaskPartNo varchar(20),
	TaskPartQty numeric(10,2),
	SupplySlipNo varchar(20)
)

insert into #pre_dtl
select BillType, 'L', OperationNo, OperationHour, ''
  from #tsk

insert into #pre_dtl
select BillType, TypeOfGoods, PartNo
	 , sum(SupplyQty - ReturnQty)
	 , SupplySlipNo
  from #itm
 where BillType = @BillType
   and (SupplyQty - ReturnQty) > 0
 group by BillType, TypeOfGoods, PartNo, SupplySlipNo

-- insert to table svTrnInvoice
declare @CustomerCode varchar(20)
if @BillType = 'C'
begin
	declare @CCode as varchar(50)							-- Perubahan
	declare @CCodeBill as varchar(50)						-- Perubahan
	
	set @CCode = (select CustomerCode from #srv)			-- Perubahan
	set @CCodeBill = (select CustomerCodeBill from #srv)	-- Perubahan
	
	if (@CCode = @CCodeBill)								-- Perubahan
	begin
		set @CustomerCode = @CCode							-- Perubahan
	end
	else if (@CCode != @CCodeBill)							-- Perubahan
	begin												
		set @CustomerCode = @CCodeBill						-- Perubahan
	end
	
end
else if @BillType = 'P'
begin
	set @CustomerCode = (select top 1 a.BillTo from svMstPackage a
				 inner join svMstPackageTask b
					on b.CompanyCode = a.CompanyCode
				   and b.PackageCode = a.PackageCode
				 inner join svMstPackageContract c
					on c.CompanyCode = a.CompanyCode
				   and c.PackageCode = a.PackageCode
				 inner join #srv d
					on d.CompanyCode = a.CompanyCode
				   and d.JobType = a.JobType
				   and d.ChassisCode = c.ChassisCode
				   and d.ChassisNo = c.ChassisNo)
end
else if @BillType in ('F','W''S')
begin
	set @CustomerCode = (select CustomerCode from svMstBillingType
				 where BillType in ('F','W','S')
				   and CompanyCode = @CompanyCode
				   and BillType = @BillType)
end
else
begin
	set @CustomerCode = (select CustomerCodeBill from #srv)
end

--set @CustomerCode = isnull((
--				select top 1 a.BillTo from svMstPackage a
--				 inner join svMstPackageTask b
--					on b.CompanyCode = a.CompanyCode
--				   and b.PackageCode = a.PackageCode
--				 inner join svMstPackageContract c
--					on c.CompanyCode = a.CompanyCode
--				   and c.PackageCode = a.PackageCode
--				 inner join #srv d
--					on d.CompanyCode = a.CompanyCode
--				   and d.JobType = a.JobType
--				   and d.ChassisCode = c.ChassisCode
--				   and d.ChassisNo = c.ChassisNo
--				), isnull((
--				select CustomerCode from svMstBillingType
--				 where BillType in ('F')
--				   and CompanyCode = @CompanyCode
--				   and BillType = @BillType
--				), isnull((select CustomerCodeBill from #srv), '')))


if ((select count(*) from #tsk) = 0 and (select count(*) from #itm) = 0)
begin
	--drop table #srv
	--drop table #tsk
	--drop table #mec
	--drop table #itm
	--drop table #pre_dtl
	return
end

if (@CustomerCode = '')
begin
	set @errmsg = N'Customer Code Bill belum di define...'
				+ char(13) + 'Tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

select * into #cus from (
select a.CompanyCode, a.IsPkp, b.CustomerCode, b.LaborDiscPct, b.PartDiscPct, b.MaterialDiscPct, b.TopCode, b.TaxCode
  from gnMstCustomer a, gnMstCustomerProfitCenter b
 where 1 = 1
   and b.CompanyCode  = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.CompanyCode  = @CompanyCode
   and b.BranchCode   = @BranchCode
   and b.CustomerCode = @CustomerCode
   and b.ProfitCenterCode = '200'
)#cus

if (select count(*) from #cus) <> 1
begin
	set @errmsg = N'Customer ProfitCenter belum di define...'
				+ char(13) + 'Tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

declare @IsPKP bit
    set @IsPKP = isnull((
				 select IsPKP from gnMstCustomer
				  where CompanyCode  = @CompanyCode
				    and CustomerCode = @CustomerCode
				  ), 0)

declare @PPnPct decimal
    set @PPnPct = isnull((
				  select a.TaxPct
				    from gnMstTax a, #cus b
				   where 1 = 1
				     and b.TaxCode     = 'PPN'
				     and a.CompanyCode = b.CompanyCode
				     and a.TaxCode     = b.TaxCode
				  ), 0)

declare @PPhPct decimal
    set @PPhPct = isnull((
				  select a.TaxPct
				    from gnMstTax a, #cus b
				   where 1 = 1
				     and b.TaxCode     = 'PPH'
				     and a.CompanyCode = b.CompanyCode
				     and a.TaxCode     = b.TaxCode
				  ), 0)

declare @ParaValue as varchar(1)
set @ParaValue = (select ParaValue from gnMstLookUpDtl where CodeID = 'SRV_FLAG' AND LookUpValue = 'CLM_MODE')

declare @JobType as varchar(15)
set @JobType = (select JobType from #srv)

if (@JobType like '%FS%' or @JobType like '%PDI%') and @BillType = 'F'
begin
	if @ParaValue = '1'
	begin
		-- Insert Into svTrnInvoice
		-----------------------------------------------------------------------------------------
		insert into svTrnInvoice(
		  CompanyCode, BranchCode, ProductType
		, InvoiceNo, InvoiceDate, InvoiceStatus
		, FPJNo, FPJDate, JobOrderNo, JobOrderDate, JobType
		, ServiceRequestDesc, ChassisCode, ChassisNo, EngineCode, EngineNo
		, PoliceRegNo, BasicModel, CustomerCode, CustomerCodeBill, Odometer
		, IsPKP, TOPCode, TOPDays, DueDate, SignedDate
		, LaborDiscPct, PartsDiscPct, MaterialDiscPct, PphPct, PpnPct, Remarks
		, PrintSeq, PostingFlag, IsLocked, CreatedBy, CreatedDate
		) 
		select
		  @CompanyCode CompanyCode
		, @BranchCode BranchCode
		, @ProductType ProductType
		, @InvoiceNo InvoiceNo
		, getdate() InvoiceDate
		, case @IsPKP
			when '0' then '1'
			else (case @BillType when 'F' then '0' when 'W' then '0' else '1' end)
		  end as InvoiceStatus
		, '' FPJNo
		, null FPJDate
		, (select JobOrderNo from #srv) JobOrderNo
		, (select JobOrderDate from #srv) JobOrderDate
		, (select JobType from #srv) JobType
		, (select ServiceRequestDesc from #srv) ServiceRequestDesc
		, (select ChassisCode from #srv) ChassisCode
		, (select ChassisNo from #srv) ChassisNo
		, (select EngineCode from #srv) EngineCode
		, (select EngineNo from #srv) EngineNo
		, (select PoliceRegNo from #srv) PoliceRegNo
		, (select BasicModel from #srv) BasicModel
		, (select CustomerCode from #srv) CustomerCode
		, @CustomerCode as CustomerCodeBill
		, (select Odometer from #srv) Odometer
		, (select IsPKP from #cus) as IsPKP
		, (select TopCode from #cus) as TOPCode
		, isnull((
			select b.ParaValue
			  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
			 where a.CompanyCode  = @CompanyCode
			   and a.BranchCode   = @BranchCode
			   and a.CustomerCode = @CustomerCode
			   and a.ProfitCenterCode = '200'
			   and b.CompanyCode  = a.CompanyCode
			   and b.CodeID = 'TOPC'
			   and b.LookUpValue = a.TopCode
			), null) as TOPDays
		, isnull((
			select dateadd(day, convert(int,b.ParaValue), convert(varchar, getdate(), 112))
			  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
			 where a.CompanyCode  = @CompanyCode
			   and a.BranchCode   = @BranchCode
			   and a.CustomerCode = @CustomerCode
			   and a.ProfitCenterCode = '200'
			   and b.CompanyCode  = a.CompanyCode
			   and b.CodeID = 'TOPC'
			   and b.LookUpValue  = a.TopCode
			), null) as DueDate
		, convert(varchar, getdate(), 112) SignedDate
		, case @BillType
			when 'F' then (select LaborDiscPct from #cus) 
			when 'W' then (select LaborDiscPct from #cus) 
			else (select LaborDiscPct from #srv) 
		  end as LaborDiscPct
		, case @BillType
			when 'F' then (select PartDiscPct from #cus) 
			when 'W' then (select PartDiscPct from #cus) 
			else (select PartDiscPct from #srv) 
		  end as PartsDiscPct
		, case @BillType
			when 'F' then (select MaterialDiscPct from #cus) 
			when 'W' then (select MaterialDiscPct from #cus) 
			else (select MaterialDiscPct from #srv) 
		  end as MaterialDiscPct
		, @PPnPct as PPhPct
		, @PPnPct as PPnPct
		, @Remarks as Remarks
		, '0' PrintSeq
		, '0' PostingFlag
		, '0' IsLocked
		, @UserID CreatedBy
		, getdate() CreatedDate

		-- Insert Into svTrnInvTask
		-----------------------------------------------------------------------------------------
		insert into svTrnInvTask (
		  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
		, OperationHour, ClaimHour, OperationCost, SubConPrice
		, IsSubCon, SharingTask, DiscPct
		)
		select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
		, isnull(OperationHour, 0) OperationHour, isnull(ClaimHour, 0) ClaimHour
		, isnull(OperationCost, 0) OperationCost, isnull(SubConPrice, 0) SubConPrice
		, isnull(IsSubCon, 0) IsSubCon, isnull(SharingTask, 0) SharingTask
		, isnull(DiscPct, 0)
		from #tsk

		-- Insert Into svTrnInvMechanic
		-----------------------------------------------------------------------------------------
		insert into svTrnInvMechanic (
		  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
		, MechanicID, ChiefMechanicID, StartService, FinishService
		)
		select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
		, MechanicID, ChiefMechanicID, StartService, FinishService
		from #mec

		-- Insert Into svTrnInvItem
		-----------------------------------------------------------------------------------------
		select * into #itm1 from (
		select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, PartNo
			 , isnull((
				select MovingCode from spMstItems
				 where CompanyCode = @CompanyCode
				   and BranchCode = @BranchCode
				   and PartNo = #itm.PartNo
				), '') as MovingCode
			 , isnull((
				select ABCClass from spMstItems
				 where CompanyCode = @CompanyCode
				   and BranchCode = @BranchCode
				   and PartNo = #itm.PartNo
				), '') as ABCClass
			 , sum(SupplyQty - ReturnQty) as SupplyQty
			 , isnull((
				select 
				  case (sum(b.SupplyQty - b.ReturnQty))
					 when 0 then 0
					 else sum(a.CostPrice * (b.SupplyQty - b.ReturnQty)) / sum(b.SupplyQty - b.ReturnQty)
				  end 
			from SpTrnSLmpDtl a
			left join SvTrnSrvItem b on 1 = 1
			 and b.CompanyCode  = a.CompanyCode
			 and b.BranchCode   = a.BranchCode
			 and b.ProductType  = a.ProductType
			 and b.SupplySlipNo = a.DocNo
			 and b.PartNo = #itm.PartNo
			where 1 = 1
			 and a.CompanyCode = @CompanyCode
			 and a.BranchCode  = @BranchCode
			 and a.ProductType = @ProductType
			 and a.PartNo = #itm.PartNo
			 and a.DocNo in (
					select SupplySlipNo
					 from SvTrnSrvItem
					where 1 = 1
					  and CompanyCode = @CompanyCode
					  and BranchCode  = @BranchCode
					  and ProductType = @ProductType
					  and ServiceNo = @ServiceNo
					  and PartNo = #itm.PartNo
					)
			), 0) as CostPrice
		, RetailPrice
		, TypeOfGoods
		from #itm
		group by CompanyCode, BranchCode, ProductType, PartNo, RetailPrice, TypeOfGoods
		)#

		insert into svTrnInvItem (
		  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo
		, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice
		, TypeOfGoods, DiscPct
		)
		select a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo
			 , MovingCode = (select top 1 MovingCode from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
			 , ABCClass = (select top 1 ABCClass from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
			 , sum(SupplyQty) as SupplyQty, 0 as ReturnQty
			 , CostPrice = (select top 1 CostPrice from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by CostPrice desc)
			 , RetailPrice = (select top 1 RetailPrice from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by RetailPrice desc)
			 , TypeOfGoods = (select top 1 TypeOfGoods from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
			 , DiscPct = (select top 1 DiscPct from #itm where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
		  from #itm1 a
		 where a.SupplyQty > 0
		 group by a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo


		-- Insert Into svTrnInvItemDtl
		-----------------------------------------------------------------------------------------
		insert into svTrnInvItemDtl (
		  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, SupplySlipNo
		, SupplyQty, CostPrice, CreatedBy, CreatedDate
		)
		select y.* from (
		select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, PartNo, SupplySlipNo
		, sum(SupplyQty - ReturnQty) as SupplyQty, CostPrice
		, @UserID as CreatedBy, getdate() as CreatedDate
		from #itm
		group by CompanyCode, BranchCode, ProductType, PartNo, SupplySlipNo, CostPrice
		) y
		where y.SupplyQty > 0

		-- Re Calculate Invoice
		-----------------------------------------------------------------------------------------
		exec uspfn_SvTrnInvoiceReCalculate @CompanyCode=@CompanyCode, @BranchCode=@BranchCode, @ProductType=@ProductType, @InvoiceNo=@InvoiceNo, @UserId=@UserId
		drop table #itm1
	end
	else if @ParaValue = '0'
	begin
		if (select IsLocked from #srv) = 1 and (select ServiceStatus from #srv) = '5'
		begin
			-- Insert Into svTrnInvoice
			-----------------------------------------------------------------------------------------
			insert into svTrnInvoice(
			  CompanyCode, BranchCode, ProductType
			, InvoiceNo, InvoiceDate, InvoiceStatus
			, FPJNo, FPJDate, JobOrderNo, JobOrderDate, JobType
			, ServiceRequestDesc, ChassisCode, ChassisNo, EngineCode, EngineNo
			, PoliceRegNo, BasicModel, CustomerCode, CustomerCodeBill, Odometer
			, IsPKP, TOPCode, TOPDays, DueDate, SignedDate
			, LaborDiscPct, PartsDiscPct, MaterialDiscPct, PphPct, PpnPct, Remarks
			, PrintSeq, PostingFlag, IsLocked, CreatedBy, CreatedDate
			) 
			select
			  @CompanyCode CompanyCode
			, @BranchCode BranchCode
			, @ProductType ProductType
			, @InvoiceNo InvoiceNo
			, getdate() InvoiceDate
			, case @IsPKP
				when '0' then '1'
				else (case @BillType when 'F' then '0' when 'W' then '0' else '1' end)
			  end as InvoiceStatus
			, '' FPJNo
			, null FPJDate
			, (select JobOrderNo from #srv) JobOrderNo
			, (select JobOrderDate from #srv) JobOrderDate
			, (select JobType from #srv) JobType
			, (select ServiceRequestDesc from #srv) ServiceRequestDesc
			, (select ChassisCode from #srv) ChassisCode
			, (select ChassisNo from #srv) ChassisNo
			, (select EngineCode from #srv) EngineCode
			, (select EngineNo from #srv) EngineNo
			, (select PoliceRegNo from #srv) PoliceRegNo
			, (select BasicModel from #srv) BasicModel
			, (select CustomerCode from #srv) CustomerCode
			, @CustomerCode as CustomerCodeBill
			, (select Odometer from #srv) Odometer
			, (select IsPKP from #cus) as IsPKP
			, (select TopCode from #cus) as TOPCode
			, isnull((
				select b.ParaValue
				  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
				 where a.CompanyCode  = @CompanyCode
				   and a.BranchCode   = @BranchCode
				   and a.CustomerCode = @CustomerCode
				   and a.ProfitCenterCode = '200'
				   and b.CompanyCode  = a.CompanyCode
				   and b.CodeID = 'TOPC'
				   and b.LookUpValue = a.TopCode
				), null) as TOPDays
			, isnull((
				select dateadd(day, convert(int,b.ParaValue), convert(varchar, getdate(), 112))
				  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
				 where a.CompanyCode  = @CompanyCode
				   and a.BranchCode   = @BranchCode
				   and a.CustomerCode = @CustomerCode
				   and a.ProfitCenterCode = '200'
				   and b.CompanyCode  = a.CompanyCode
				   and b.CodeID = 'TOPC'
				   and b.LookUpValue  = a.TopCode
				), null) as DueDate
			, convert(varchar, getdate(), 112) SignedDate
			, case @BillType
				when 'F' then (select LaborDiscPct from #cus) 
				when 'W' then (select LaborDiscPct from #cus) 
				else (select LaborDiscPct from #srv) 
			  end as LaborDiscPct
			, case @BillType
				when 'F' then (select PartDiscPct from #cus) 
				when 'W' then (select PartDiscPct from #cus) 
				else (select PartDiscPct from #srv) 
			  end as PartsDiscPct
			, case @BillType
				when 'F' then (select MaterialDiscPct from #cus) 
				when 'W' then (select MaterialDiscPct from #cus) 
				else (select MaterialDiscPct from #srv) 
			  end as MaterialDiscPct
			, @PPnPct as PPhPct
			, @PPnPct as PPnPct
			, @Remarks as Remarks
			, '0' PrintSeq
			, '0' PostingFlag
			, '0' IsLocked
			, @UserID CreatedBy
			, getdate() CreatedDate

			-- Insert Into svTrnInvTask
			-----------------------------------------------------------------------------------------
			insert into svTrnInvTask (
			  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
			, OperationHour, ClaimHour, OperationCost, SubConPrice
			, IsSubCon, SharingTask, DiscPct
			)
			select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
			, isnull(OperationHour, 0) OperationHour, isnull(ClaimHour, 0) ClaimHour
			, isnull(OperationCost, 0) OperationCost, isnull(SubConPrice, 0) SubConPrice
			, isnull(IsSubCon, 0) IsSubCon, isnull(SharingTask, 0) SharingTask
			, isnull(DiscPct, 0)
			from #tsk

			-- Insert Into svTrnInvMechanic
			-----------------------------------------------------------------------------------------
			insert into svTrnInvMechanic (
			  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
			, MechanicID, ChiefMechanicID, StartService, FinishService
			)
			select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
			, MechanicID, ChiefMechanicID, StartService, FinishService
			from #mec

			-- Insert Into svTrnInvItem
			-----------------------------------------------------------------------------------------
			select * into #itm2 from (
			select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, PartNo
				 , isnull((
					select MovingCode from spMstItems
					 where CompanyCode = @CompanyCode
					   and BranchCode = @BranchCode
					   and PartNo = #itm.PartNo
					), '') as MovingCode
				 , isnull((
					select ABCClass from spMstItems
					 where CompanyCode = @CompanyCode
					   and BranchCode = @BranchCode
					   and PartNo = #itm.PartNo
					), '') as ABCClass
				 , sum(SupplyQty - ReturnQty) as SupplyQty
				 , isnull((
					select 
					  case (sum(b.SupplyQty - b.ReturnQty))
						 when 0 then 0
						 else sum(a.CostPrice * (b.SupplyQty - b.ReturnQty)) / sum(b.SupplyQty - b.ReturnQty)
					  end 
				from SpTrnSLmpDtl a
				left join SvTrnSrvItem b on 1 = 1
				 and b.CompanyCode  = a.CompanyCode
				 and b.BranchCode   = a.BranchCode
				 and b.ProductType  = a.ProductType
				 and b.SupplySlipNo = a.DocNo
				 and b.PartNo = #itm.PartNo
				where 1 = 1
				 and a.CompanyCode = @CompanyCode
				 and a.BranchCode  = @BranchCode
				 and a.ProductType = @ProductType
				 and a.PartNo = #itm.PartNo
				 and a.DocNo in (
						select SupplySlipNo
						 from SvTrnSrvItem
						where 1 = 1
						  and CompanyCode = @CompanyCode
						  and BranchCode  = @BranchCode
						  and ProductType = @ProductType
						  and ServiceNo = @ServiceNo
						  and PartNo = #itm.PartNo
						)
				), 0) as CostPrice
			, RetailPrice
			, TypeOfGoods
			from #itm
			group by CompanyCode, BranchCode, ProductType, PartNo, RetailPrice, TypeOfGoods
			)#

			insert into svTrnInvItem (
			  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo
			, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice
			, TypeOfGoods, DiscPct
			)
			select a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo
				 , MovingCode = (select top 1 MovingCode from #itm2 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
				 , ABCClass = (select top 1 ABCClass from #itm2 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
				 , sum(SupplyQty) as SupplyQty, 0 as ReturnQty
				 , CostPrice = (select top 1 CostPrice from #itm2 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by CostPrice desc)
				 , RetailPrice = (select top 1 RetailPrice from #itm2 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by RetailPrice desc)
				 , TypeOfGoods = (select top 1 TypeOfGoods from #itm2 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
				 , DiscPct = (select top 1 DiscPct from #itm where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
			  from #itm2 a
			 where a.SupplyQty > 0
			 group by a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo


			-- Insert Into svTrnInvItemDtl
			-----------------------------------------------------------------------------------------
			insert into svTrnInvItemDtl (
			  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, SupplySlipNo
			, SupplyQty, CostPrice, CreatedBy, CreatedDate
			)
			select y.* from (
			select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, PartNo, SupplySlipNo
			, sum(SupplyQty - ReturnQty) as SupplyQty, CostPrice
			, @UserID as CreatedBy, getdate() as CreatedDate
			from #itm
			group by CompanyCode, BranchCode, ProductType, PartNo, SupplySlipNo, CostPrice
			) y
			where y.SupplyQty > 0

			-- Re Calculate Invoice
			-----------------------------------------------------------------------------------------
			exec uspfn_SvTrnInvoiceReCalculate @CompanyCode=@CompanyCode, @BranchCode=@BranchCode, @ProductType=@ProductType, @InvoiceNo=@InvoiceNo, @UserId=@UserId
			drop table #itm2
		end
	end
end
else
begin
	if (select IsLocked from #srv) = 1 and @BillType = 'C' return
	else
	begin	 
		-- Insert Into svTrnInvoice
		-----------------------------------------------------------------------------------------
		insert into svTrnInvoice(
		  CompanyCode, BranchCode, ProductType
		, InvoiceNo, InvoiceDate, InvoiceStatus
		, FPJNo, FPJDate, JobOrderNo, JobOrderDate, JobType
		, ServiceRequestDesc, ChassisCode, ChassisNo, EngineCode, EngineNo
		, PoliceRegNo, BasicModel, CustomerCode, CustomerCodeBill, Odometer
		, IsPKP, TOPCode, TOPDays, DueDate, SignedDate
		, LaborDiscPct, PartsDiscPct, MaterialDiscPct, PphPct, PpnPct, Remarks
		, PrintSeq, PostingFlag, IsLocked, CreatedBy, CreatedDate
		) 
		select
		  @CompanyCode CompanyCode
		, @BranchCode BranchCode
		, @ProductType ProductType
		, @InvoiceNo InvoiceNo
		, getdate() InvoiceDate
		, case @IsPKP
			when '0' then '1'
			else (case @BillType when 'F' then '0' when 'W' then '0' else '1' end)
		  end as InvoiceStatus
		, '' FPJNo
		, null FPJDate
		, (select JobOrderNo from #srv) JobOrderNo
		, (select JobOrderDate from #srv) JobOrderDate
		, (select JobType from #srv) JobType
		, (select ServiceRequestDesc from #srv) ServiceRequestDesc
		, (select ChassisCode from #srv) ChassisCode
		, (select ChassisNo from #srv) ChassisNo
		, (select EngineCode from #srv) EngineCode
		, (select EngineNo from #srv) EngineNo
		, (select PoliceRegNo from #srv) PoliceRegNo
		, (select BasicModel from #srv) BasicModel
		, (select CustomerCode from #srv) CustomerCode
		, @CustomerCode as CustomerCodeBill
		, (select Odometer from #srv) Odometer
		, (select IsPKP from #cus) as IsPKP
		, (select TopCode from #cus) as TOPCode
		, isnull((
			select b.ParaValue
			  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
			 where a.CompanyCode  = @CompanyCode
			   and a.BranchCode   = @BranchCode
			   and a.CustomerCode = @CustomerCode
			   and a.ProfitCenterCode = '200'
			   and b.CompanyCode  = a.CompanyCode
			   and b.CodeID = 'TOPC'
			   and b.LookUpValue = a.TopCode
			), null) as TOPDays
		, isnull((
			select dateadd(day, convert(int,b.ParaValue), convert(varchar, getdate(), 112))
			  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
			 where a.CompanyCode  = @CompanyCode
			   and a.BranchCode   = @BranchCode
			   and a.CustomerCode = @CustomerCode
			   and a.ProfitCenterCode = '200'
			   and b.CompanyCode  = a.CompanyCode
			   and b.CodeID = 'TOPC'
			   and b.LookUpValue  = a.TopCode
			), null) as DueDate
		, convert(varchar, getdate(), 112) SignedDate
		, case @BillType
			when 'F' then (select LaborDiscPct from #cus) 
			when 'W' then (select LaborDiscPct from #cus) 
			else (select LaborDiscPct from #srv) 
		  end as LaborDiscPct
		, case @BillType
			when 'F' then (select PartDiscPct from #cus) 
			when 'W' then (select PartDiscPct from #cus) 
			else (select PartDiscPct from #srv) 
		  end as PartsDiscPct
		, case @BillType
			when 'F' then (select MaterialDiscPct from #cus) 
			when 'W' then (select MaterialDiscPct from #cus) 
			else (select MaterialDiscPct from #srv) 
		  end as MaterialDiscPct
		, @PPnPct as PPhPct
		, @PPnPct as PPnPct
		, @Remarks as Remarks
		, '0' PrintSeq
		, '0' PostingFlag
		, '0' IsLocked
		, @UserID CreatedBy
		, getdate() CreatedDate

		-- Insert Into svTrnInvTask
		-----------------------------------------------------------------------------------------
		insert into svTrnInvTask (
		  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
		, OperationHour, ClaimHour, OperationCost, SubConPrice
		, IsSubCon, SharingTask, DiscPct
		)
		select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
		, isnull(OperationHour, 0) OperationHour, isnull(ClaimHour, 0) ClaimHour
		, isnull(OperationCost, 0) OperationCost, isnull(SubConPrice, 0) SubConPrice
		, isnull(IsSubCon, 0) IsSubCon, isnull(SharingTask, 0) SharingTask
		, isnull(DiscPct, 0)
		from #tsk

		-- Insert Into svTrnInvMechanic
		-----------------------------------------------------------------------------------------
		insert into svTrnInvMechanic (
		  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
		, MechanicID, ChiefMechanicID, StartService, FinishService
		)
		select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
		, MechanicID, ChiefMechanicID, StartService, FinishService
		from #mec

		-- Insert Into svTrnInvItem
		-----------------------------------------------------------------------------------------
		select * into #itm3 from (
		select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, PartNo
			 , isnull((
				select MovingCode from spMstItems
				 where CompanyCode = @CompanyCode
				   and BranchCode = @BranchCode
				   and PartNo = #itm.PartNo
				), '') as MovingCode
			 , isnull((
				select ABCClass from spMstItems
				 where CompanyCode = @CompanyCode
				   and BranchCode = @BranchCode
				   and PartNo = #itm.PartNo
				), '') as ABCClass
			 , sum(SupplyQty - ReturnQty) as SupplyQty
			 , isnull((
				select 
				  case (sum(b.SupplyQty - b.ReturnQty))
					 when 0 then 0
					 else sum(a.CostPrice * (b.SupplyQty - b.ReturnQty)) / sum(b.SupplyQty - b.ReturnQty)
				  end 
			from SpTrnSLmpDtl a
			left join SvTrnSrvItem b on 1 = 1
			 and b.CompanyCode  = a.CompanyCode
			 and b.BranchCode   = a.BranchCode
			 and b.ProductType  = a.ProductType
			 and b.SupplySlipNo = a.DocNo
			 and b.PartNo = #itm.PartNo
			where 1 = 1
			 and a.CompanyCode = @CompanyCode
			 and a.BranchCode  = @BranchCode
			 and a.ProductType = @ProductType
			 and a.PartNo = #itm.PartNo
			 and a.DocNo in (
					select SupplySlipNo
					 from SvTrnSrvItem
					where 1 = 1
					  and CompanyCode = @CompanyCode
					  and BranchCode  = @BranchCode
					  and ProductType = @ProductType
					  and ServiceNo = @ServiceNo
					  and PartNo = #itm.PartNo
					)
			), 0) as CostPrice
		, RetailPrice
		, TypeOfGoods
		from #itm
		group by CompanyCode, BranchCode, ProductType, PartNo, RetailPrice, TypeOfGoods
		)#

		insert into svTrnInvItem (
		  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo
		, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice
		, TypeOfGoods, DiscPct
		)
		select a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo
			 , MovingCode = (select top 1 MovingCode from #itm3 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
			 , ABCClass = (select top 1 ABCClass from #itm3 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
			 , sum(SupplyQty) as SupplyQty, 0 as ReturnQty
			 , CostPrice = (select top 1 CostPrice from #itm3 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by CostPrice desc)
			 , RetailPrice = (select top 1 RetailPrice from #itm3 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by RetailPrice desc)
			 , TypeOfGoods = (select top 1 TypeOfGoods from #itm3 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
			 , DiscPct = (select top 1 DiscPct from #itm where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
		  from #itm3 a
		 where a.SupplyQty > 0
		 group by a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo

		-- Insert Into svTrnInvItemDtl
		-----------------------------------------------------------------------------------------
		insert into svTrnInvItemDtl (
		  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, SupplySlipNo
		, SupplyQty, CostPrice, CreatedBy, CreatedDate
		)
		select y.* from (
		select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, PartNo, SupplySlipNo
		, sum(SupplyQty - ReturnQty) as SupplyQty, CostPrice
		, @UserID as CreatedBy, getdate() as CreatedDate
		from #itm
		group by CompanyCode, BranchCode, ProductType, PartNo, SupplySlipNo, CostPrice
		) y
		where y.SupplyQty > 0

		-- Re Calculate Invoice
		-----------------------------------------------------------------------------------------
		exec uspfn_SvTrnInvoiceReCalculate @CompanyCode=@CompanyCode, @BranchCode=@BranchCode, @ProductType=@ProductType, @InvoiceNo=@InvoiceNo, @UserId=@UserId
		drop table #itm3
	end
end
          
drop table #srv,#tsk,#mec, #itm, #cus, #pre_dtl                        
GO

ALTER procedure [dbo].[uspfn_SvTrnJobOrderCreate]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@ServiceNo bigint,
	@UserID varchar(15)
as      

declare @errmsg varchar(max)

begin try
	begin transaction
		declare @docseq int 
        set @docseq = isnull((
			select DocumentSequence from gnMstDocument 
			 where 1 = 1
			   and CompanyCode  = @CompanyCode
			   and BranchCode   = @BranchCode
			   and DocumentType = 'SPK'),0) + 1
        declare @JobOrderNo varchar(15)
		set @JobOrderNo = 'SPK/' + (select right(convert(char(4),getdate(),112),2)) + '/' 
                                 + right((replicate('0',6) + (select convert(varchar, @docseq))),6)
		update svTrnService
		   set ServiceType    = '2'
              ,JobOrderNo     = @JobOrderNo
              ,JobOrderDate   = getdate()
              ,LastUpdateBy   = @UserID
              ,LastUpdateDate = getdate()
		 where 1 = 1
		   and CompanyCode = @CompanyCode
		   and BranchCode  = @BranchCode
		   and ProductType = @ProductType
		   and ServiceNo   = @ServiceNo
		update gnMstDocument 
		   set DocumentSequence = @docseq
              ,LastUpdateBy     = @UserID
              ,LastUpdateDate   = getdate()
		 where 1 = 1
		   and CompanyCode  = @CompanyCode
		   and BranchCode   = @BranchCode
		   and DocumentType = 'SPK'
	commit transaction

	--exec uspfn_SvInsertDefaultTaskMovement @CompanyCode, @BranchCode, @ProductType, @ServiceNo, @UserID

end try
begin catch
	rollback transaction
	set @errmsg = N'tidak dapat konversi ke SPK pada ServiceNo = '
				+ convert(varchar,@ServiceNo)
				+ char(13) + error_message()
	raiserror (@errmsg,16,1);
end catch
GO
ALTER procedure [dbo].[usprpt_OmRpSalesTrn007]
	-- Add the parameters for the stored procedure here
	@CompanyCode VARCHAR(15),
	@BranchCode	 VARCHAR(15),
	@ReqNoA		 VARCHAR(15),
	@ReqNoB		 VARCHAR(15)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
SELECT
	row_number () OVER (ORDER BY a.ReqNo) AS No
	, a.ReqNo
	, a.SKPKNo
	, a.FakturPolisiNo
	--, ISNULL(e.RefferenceDONo, '') DONo
	, ISNULL(c.SuzukiDONo, '') DONo
	, (SELECT dbo.GetDateIndonesian (a.FakturPolisiDate)) AS 'Tanggal'
	--, ISNULL(e.RefferenceDoDate, '') DODate
	, ISNULL(c.SuzukiDODate, '') DODate
	, ISNULL(d.CompanyName, '') CompanyName
	, ISNULL(d.Address1, '') CoAdd1
	, ISNULL(d.Address2, '') CoAdd2
	, ISNULL(d.Address3, '') CoAdd3
	, case d.ProductType 
		when '2W' then 'Harap dibuatkan Faktur untuk motor SUZUKI :'
		when '4W' then 'Harap dibuatkan Faktur untuk mobil SUZUKI :'
		when 'A' then 'Harap dibuatkan Faktur untuk motor SUZUKI :'
		when 'B' then 'Harap dibuatkan Faktur untuk mobil SUZUKI :'
		else 'Harap dibuatkan Faktur untuk SUZUKI :'
		end as Note
	--, ISNULL(e.RefferenceSJNo, '') SJNo
	--, ISNULL(e.RefferenceSJDate, '') SJDate
	, ISNULL(c.SuzukiSJNo, '') SJNo
	, ISNULL(c.SuzukiSJDate, '') SJDate
	, ISNULL(c.SalesModelCode, '') Model
	, ISNULL(f.SalesModelDesc, '') ModelDesc
	, ISNULL(g.RefferenceDesc1, '') Warna
	, ISNULL(c.SalesModelYear, 0) Tahun
	, a.ChassisNo
	, ISNULL(c.EngineNo, 0) EngineNo
	, ((CASE ISNULL(a.DealerCategory, '') WHEN 'M' THEN 'Main Dealer' WHEN 'S' THEN 'Sub Dealer' WHEN 'R' THEN 'Show Room' END) + ' / ' + h.CustomerName) AS  Penjual
	, a.SalesmanName
	, a.SKPKName
	, a.SKPKAddress1 Alamat1
	, a.SKPKAddress2 Alamat2
	, a.SKPKAddress3 Alamat3
	, ISNULL(i.LookUpValueName, '') City
	, a.SKPKTelp1
	, a.SKPKTelp2
	, a.SKPKHP
	, ISNULL(a.SKPKBirthday, '') SKPKDay
	, a.FakturPolisiName
	, a.FakturPolisiAddress1
	, a.FakturPolisiAddress2
	, a.FakturPolisiAddress3
	, a.FakturPolisiTelp1
	, a.FakturPolisiTelp2
	, a.FakturPolisiHP
	, a.FakturPolisiBirthday
	, (select ISNULL(LookUpValueName, '') from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID='FPCT' and LookUpValue=a.DealerCategory
		) AS DealerCategory
	, ISNULL(b.Remark, '') Remark
	, ISNULL(UPPER(z.SignName), '') AS SignName1
	, ISNULL(UPPER(z.TitleSign), '') AS TitleSign1 
	, a.IDNo
FROM
 omTrSalesReqDetail a
JOIN
 omTrSalesReq b ON b.CompanyCode=a.CompanyCode AND b.BranchCode=a.BranchCode
 AND b.ReqNo=a.ReqNo 
LEFT JOIN
 sdms..omMstVehicle c ON c.CompanyCode= '6159401'--a.CompanyCode 
 AND c.ChassisCode=a.ChassisCode
 AND c.ChassisNo=a.ChassisNo
LEFT JOIN
 gnMstCoProfile d ON d.CompanyCode=a.CompanyCode AND d.BranchCode=a.BranchCode
--LEFT JOIN omTrPurchaseBPUDetail j on a.CompanyCode=j.CompanyCode and c.ChassisCode=j.ChassisCode
--	and a.ChassisNo=j.ChassisNo
--LEFT JOIN
-- omTrPurchaseBPU e ON e.CompanyCode=j.CompanyCode AND e.BranchCode=j.BranchCode
--	and e.BPUNo=j.BPUNo
--	and e.PONo = c.PONo
LEFT JOIN
 sdms..omMstModel f ON f.CompanyCode= '6159401' --c.CompanyCode 
 AND f.SalesModelCode=c.SalesModelCode
LEFT JOIN
 sdms..omMstRefference g ON g.CompanyCode= '6159401' --c.CompanyCode
  AND g.RefferenceType='COLO'
 AND g.RefferenceCode=c.ColourCode
LEFT JOIN
 gnMstCustomer h ON h.CompanyCode=b.CompanyCode AND h.CustomerCode=b.SubDealerCode
LEFT JOIN
 gnMstLookUpDtl i ON i.CompanyCode=a.CompanyCode AND i.CodeID='CITY' 
 AND i.LookUpValue=a.SKPKCity
LEFT JOIN gnMstSIgnature z
	ON z.CompanyCode = a.CompanyCode
	AND z.BranchCode = a.BranchCode
	AND z.ProfitCenterCode = '100'
	AND z.DocumentType = 'RFP'
	AND z.SeqNo = 1
WHERE
 a.CompanyCode	  = @CompanyCode
 AND a.BranchCode = @BranchCode
 AND a.ReqNo BETWEEN @ReqNoA AND @ReqNoB
ORDER BY ReqNo
END
GO




