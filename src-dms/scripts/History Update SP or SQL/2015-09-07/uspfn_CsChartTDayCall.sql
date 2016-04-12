IF (OBJECT_ID('uspfn_CsChartTDayCall') is not null)
	drop proc dbo.uspfn_CsChartTDayCall
GO

/****** Object:  StoredProcedure [dbo].[uspfn_CsChart3DayCall]    Script Date: 9/7/2015 11:39:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_CsChart3DayCall '6006400131'
create proc [dbo].[uspfn_CsChartTDayCall]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
as
begin
	select a.BranchCode, c.OutletAbbreviation, a.CustomerCount, isnull(b.InputByCRO, 0) InputByCRO, convert(numeric(18,2), isnull(b.InputByCRO, 0)) / a.CustomerCount * 100 Percentation
	  from (
			 select CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuTDaysCallView
			 where BpkDate BETWEEN @DateFrom and @DateTo
			 group by CompanyCode, BranchCode
		   ) a
 left join (
			 select CompanyCode, BranchCode, count(CustomerCode) InputByCRO from CsLkuTDaysCallView
			 where Outstanding = 'N'
			   and BpkDate BETWEEN @DateFrom AND @DateTo
			 group by CompanyCode, BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode
 left join gnmstdealeroutletmapping c
		on a.BranchCode = c.OutletCode
	 where a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END

end