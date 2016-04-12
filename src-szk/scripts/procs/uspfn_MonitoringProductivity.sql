if object_id('uspfn_MonitoringProductivity') is not null
	drop procedure uspfn_MonitoringProductivity

go
create procedure uspfn_MonitoringProductivity
	@CompanyCode	varchar(15),
	@BranchCode		varchar(15),
	@PeriodDate		date
as
begin   
	declare @StartDate		varchar(10);
	declare @EndDate		varchar(10);

	set @PeriodDate = getdate()
	set @StartDate  = convert(varchar,dateadd(dd,-(day(@PeriodDate)-1),@PeriodDate),112)
	set @EndDate    = convert(varchar,dateadd(dd,-1,dateadd(mm,1,@StartDate)),112)

	if @CompanyCode is null or @CompanyCode = ''
		set @CompanyCode = null;

	if @BranchCode is null or @BranchCode = ''
		set @BranchCode = null;

	if @PeriodDate is null
		set @PeriodDate = getdate();

	select * into #ITS
		from ( select CompanyCode, BranchCode, InquiryDate, CreatedDate, 
					day(CreatedDate) DayCrt, datediff(day,CreatedDate,InquiryDate) INQ
				from pmHstITS
				where convert(varchar,CreatedDate,112) between @StartDate and @EndDate
				and CompanyCode=isnull(@CompanyCode, CompanyCode)
				and BranchCode=isnull(@BranchCode, BranchCode)
			) #ITS
    
	--rawdata
	--select INQ, DayCrt, count(*) Total from #ITS group by INQ, DayCrt order by INQ desc

	--pivot
	select * 
		into #InqMonProdTemp
		from (select INQ, DayCrt, count(*) Total from #ITS
				group by INQ, DayCrt
			) as ITS
		pivot (sum(Total) for [DayCrt] in ([01],[02],[03],[04],[05],[06],[07],[08],[09],[10],
										[11],[12],[13],[14],[15],[16],[17],[18],[19],[20],
										[21],[22],[23],[24],[25],[26],[27],[28],[29],[30],[31])
			) as pvt

	select -7 as INQ
			, sum(isnull([01], 0)) as [01]
			, sum(isnull([02], 0)) as [02]
			, sum(isnull([03], 0)) as [03]
			, sum(isnull([04], 0)) as [04]
			, sum(isnull([05], 0)) as [05]
			, sum(isnull([06], 0)) as [06]
			, sum(isnull([07], 0)) as [07]
			, sum(isnull([08], 0)) as [08]
			, sum(isnull([09], 0)) as [09]
			, sum(isnull([10], 0)) as [10]
			, sum(isnull([11], 0)) as [11]
			, sum(isnull([12], 0)) as [12]
			, sum(isnull([13], 0)) as [13]
			, sum(isnull([14], 0)) as [14]
			, sum(isnull([15], 0)) as [15]
			, sum(isnull([16], 0)) as [16]
			, sum(isnull([17], 0)) as [17]
			, sum(isnull([18], 0)) as [18]
			, sum(isnull([19], 0)) as [19]
			, sum(isnull([20], 0)) as [20]
			, sum(isnull([21], 0)) as [21]
			, sum(isnull([22], 0)) as [22]
			, sum(isnull([23], 0)) as [23]
			, sum(isnull([24], 0)) as [24]
			, sum(isnull([25], 0)) as [25]
			, sum(isnull([26], 0)) as [26]
			, sum(isnull([27], 0)) as [27]
			, sum(isnull([28], 0)) as [28]
			, sum(isnull([29], 0)) as [29]
			, sum(isnull([30], 0)) as [30]
			, sum(isnull([31], 0)) as [31]
		into #InqModProdWrapper
		from #InqMonProdTemp a
		where a.INQ <= -7
	 

		insert into #InqModProdWrapper
		select '7' as INQ
			, sum(isnull([01], 0)) as [01]
			, sum(isnull([02], 0)) as [02]
			, sum(isnull([03], 0)) as [03]
			, sum(isnull([04], 0)) as [04]
			, sum(isnull([05], 0)) as [05]
			, sum(isnull([06], 0)) as [06]
			, sum(isnull([07], 0)) as [07]
			, sum(isnull([08], 0)) as [08]
			, sum(isnull([09], 0)) as [09]
			, sum(isnull([10], 0)) as [10]
			, sum(isnull([11], 0)) as [11]
			, sum(isnull([12], 0)) as [12]
			, sum(isnull([13], 0)) as [13]
			, sum(isnull([14], 0)) as [14]
			, sum(isnull([15], 0)) as [15]
			, sum(isnull([16], 0)) as [16]
			, sum(isnull([17], 0)) as [17]
			, sum(isnull([18], 0)) as [18]
			, sum(isnull([19], 0)) as [19]
			, sum(isnull([20], 0)) as [20]
			, sum(isnull([21], 0)) as [21]
			, sum(isnull([22], 0)) as [22]
			, sum(isnull([23], 0)) as [23]
			, sum(isnull([24], 0)) as [24]
			, sum(isnull([25], 0)) as [25]
			, sum(isnull([26], 0)) as [26]
			, sum(isnull([27], 0)) as [27]
			, sum(isnull([28], 0)) as [28]
			, sum(isnull([29], 0)) as [29]
			, sum(isnull([30], 0)) as [30]
			, sum(isnull([31], 0)) as [31]
		from #InqMonProdTemp a
		where a.INQ >= 7

	insert into #InqModProdWrapper
	select INQ
			, isnull([01], 0) as [01]
			, isnull([02], 0) as [02]
			, isnull([03], 0) as [03]
			, isnull([04], 0) as [04]
			, isnull([05], 0) as [05]
			, isnull([06], 0) as [06]
			, isnull([07], 0) as [07]
			, isnull([08], 0) as [08]
			, isnull([09], 0) as [09]
			, isnull([10], 0) as [10]
			, isnull([11], 0) as [11]
			, isnull([12], 0) as [12]
			, isnull([13], 0) as [13]
			, isnull([14], 0) as [14]
			, isnull([15], 0) as [15]
			, isnull([16], 0) as [16]
			, isnull([17], 0) as [17]
			, isnull([18], 0) as [18]
			, isnull([19], 0) as [19]
			, isnull([20], 0) as [20]
			, isnull([21], 0) as [21]
			, isnull([22], 0) as [22]
			, isnull([23], 0) as [23]
			, isnull([24], 0) as [24]
			, isnull([25], 0) as [25]
			, isnull([26], 0) as [26]
			, isnull([27], 0) as [27]
			, isnull([28], 0) as [28]
			, isnull([29], 0) as [29]
			, isnull([30], 0) as [30]
			, isnull([31], 0) as [31]
		from #InqMonProdTemp a
		where a.INQ between -6 and 6

	  select INQ
	       , isnull([01], 0) as [01]
	       , isnull([02], 0) as [02]
	       , isnull([03], 0) as [03]
	       , isnull([04], 0) as [04]
	       , isnull([05], 0) as [05]
	       , isnull([06], 0) as [06]
	       , isnull([07], 0) as [07]
	       , isnull([08], 0) as [08]
	       , isnull([09], 0) as [09]
	       , isnull([10], 0) as [10]
	       , isnull([11], 0) as [11]
	       , isnull([12], 0) as [12]
	       , isnull([13], 0) as [13]
	       , isnull([14], 0) as [14]
	       , isnull([15], 0) as [15]
	       , isnull([16], 0) as [16]
	       , isnull([17], 0) as [17]
	       , isnull([18], 0) as [18]
	       , isnull([19], 0) as [19]
	       , isnull([20], 0) as [20]
	       , isnull([21], 0) as [21]
	       , isnull([22], 0) as [22]
	       , isnull([23], 0) as [23]
	       , isnull([24], 0) as [24]
	       , isnull([25], 0) as [25]
	       , isnull([26], 0) as [26]
	       , isnull([27], 0) as [27]
	       , isnull([28], 0) as [28]
	       , isnull([29], 0) as [29]
	       , isnull([30], 0) as [30]
	       , isnull([31], 0) as [31]
		from #InqModProdWrapper a
		order by a.INQ asc

	drop table #ITS
	drop table #InqModProdWrapper
	drop table #InqMonProdTemp
end






go
exec uspfn_MonitoringProductivity '', '', ''
--exec uspfn_MonitoringProductivity '', '', '', '', ''