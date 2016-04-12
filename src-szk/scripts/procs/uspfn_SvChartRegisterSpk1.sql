alter procedure uspfn_SvChartRegisterSpk1
	@DateFrom date,
	@DateTo   date,
	@Area  varchar(50) = '',
	@Dealer   varchar(20) = '' 

as

if (@Dealer != '')
begin 
	select a.BranchCode + ' - ' + b.ShortBranchName as DataSeries
		 , convert(varchar, a.JobOrderDate, 112) DataKey
		 , count(*) DataValue
	  from SvRegisterSpk a
      left join OutletInfo b
	    on b.CompanyCode = a.CompanyCode
	   and b.BranchCode = a.BranchCode
	 where 1 = 1
	   and convert(date, a.JobOrderDate) between @DateFrom and @DateTo 
	   and a.CompanyCode = @Dealer
	 group by a.BranchCode, b.ShortBranchName, convert(varchar, JobOrderDate, 112)
	 order by a.BranchCode, convert(varchar, JobOrderDate, 112)
end
else if (@Area != '')
begin
	select a.CompanyCode + ' - ' + c.DealerName as DataSeries
		 , convert(varchar, JobOrderDate, 112) DataKey
		 , count(*) DataValue
	  from SvRegisterSpk a
	  left join gnMstDealerMapping b
	    on b.DealerCode = a.CompanyCode
	  left join DealerInfo c
	    on c.DealerCode = a.CompanyCode
	 where 1 = 1
	   and convert(date, JobOrderDate) between @DateFrom and @DateTo 
	   and b.Area = @Area
	 group by a.CompanyCode, c.DealerName, convert(varchar, JobOrderDate, 112)
	 order by CompanyCode, convert(varchar, JobOrderDate, 112)
end
else 
begin
	select b.Area as DataSeries
		 , convert(varchar, JobOrderDate, 112) DataKey
		 , count(*) DataValue
	  from SvRegisterSpk a
	  left join gnMstDealerMapping b
	    on b.DealerCode = a.CompanyCode
	 where 1 = 1
	   and convert(date, JobOrderDate) between @DateFrom and @DateTo 
	 group by Area, convert(varchar, JobOrderDate, 112)
	 order by Area, convert(varchar, JobOrderDate, 112)
end


go

--uspfn_SvChartRegisterSpk1 '2014-01-01', '2014-06-06', 'JABODETABEK', '6021406'
uspfn_SvChartRegisterSpk1 '2014-01-01', '2014-06-06'
