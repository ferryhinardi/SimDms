alter proc uspfn_SvChartUnitIntakeSummary
	@Year  int,
	@Month int,
	@Area  varchar(50) = '',
	@Dealer varchar(50) = ''
as

if (@Dealer != '')
begin
	select b.ShortBranchName as DataKey, count(a.JobOrderNo) DataValue
	  from SvUnitIntake a
	  left join OutletInfo b
	    on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	 where year(a.JobOrderClosed) = @Year
	   and month(a.JobOrderClosed) = @Month
	   and a.CompanyCode = @Dealer
	 group by a.BranchCode, b.BranchName, b.ShortBranchName
	 order by b.BranchName
end
else if (@Area != '')
begin
	select b.DealerName as DataKey, count(a.JobOrderNo) DataValue
	  from SvUnitIntake a
	  left join DealerInfo b
	    on b.DealerCode = a.CompanyCode
	 where year(a.JobOrderClosed) = @Year
	   and month(a.JobOrderClosed) = @Month
	   and a.AreaCode = @Area
	 group by a.Area, b.DealerName, b.ShortName
end
else
begin
	select isnull(Area, '-') DataKey, count(*) DataValue
	  from SvUnitIntake
	 where year(JobOrderClosed) = @Year
	   and month(JobOrderClosed) = @Month
	 group by Area
	 order by count(*)
end

go

--exec uspfn_SvChartUnitIntakeSummary 2014, 1, '100', '6006400001'
--exec uspfn_SvChartUnitIntakeSummary 2014, 1, '', ''
exec uspfn_SvChartUnitIntakeSummary 2014, 1, '100', ''
