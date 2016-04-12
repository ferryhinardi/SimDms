IF (OBJECT_ID('uspfn_CsChartTDayCall') is not null)
	drop proc dbo.uspfn_CsChartTDayCall
GO

--uspfn_CsChartTDayCall '6006400106', '2015-10-30', '2016-01-01'
create proc [dbo].[uspfn_CsChartTDayCall]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
as
begin
	select a.BranchCode, c.OutletAbbreviation, a.CustomerCount, isnull(b.InputByCRO, 0) InputByCRO, convert(numeric(18,2), isnull(b.InputByCRO, 0)) / a.CustomerCount * 100 Percentation
	  from (
			 select CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuTDaysCallView
			 where DeliveryDate BETWEEN @DateFrom and @DateTo
			 group by CompanyCode, BranchCode
		   ) a
 left join (
			 select CompanyCode, BranchCode, count(CustomerCode) InputByCRO from CsLkuTDaysCallView
			 where Outstanding = 'N'
			   and DeliveryDate BETWEEN @DateFrom AND @DateTo
			 group by CompanyCode, BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode
 left join gnmstdealeroutletmapping c
		on a.BranchCode = c.OutletCode
	 where a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END

end