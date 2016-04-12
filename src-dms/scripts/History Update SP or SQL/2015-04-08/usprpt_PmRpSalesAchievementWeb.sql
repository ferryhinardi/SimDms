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