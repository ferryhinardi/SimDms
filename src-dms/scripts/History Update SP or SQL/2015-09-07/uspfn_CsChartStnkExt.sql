IF(OBJECT_ID('uspfn_CsChartStnkExt') is not null)
	drop proc dbo.uspfn_CsChartStnkExt
GO

/****** Object:  StoredProcedure [dbo].[uspfn_CsChartStnkExt]    Script Date: 9/7/2015 11:21:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_CsChartStnkExt '', '1-1-2012', '9-9-2015'
CREATE proc [dbo].[uspfn_CsChartStnkExt]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
as
begin
	select a.CompanyCode, a.BranchCode, d.Area, d.DealerAbbreviation AS Dealer, c.OutletName AS Outlet, a.CustomerCount, isnull(b.InputByCRO, 0) InputByCRO, isnull((convert(decimal, (b.InputByCRO / a.CustomerCount)) * 100), 0) Percentation
	  from (
			 select CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuStnkExtensionView
			 where CustomerCreatedDate BETWEEN @DateFrom and @DateTo
			 group by CompanyCode, BranchCode
		   ) a
 left join (
			 select CompanyCode, BranchCode, count(CustomerCode) InputByCRO from CsLkuStnkExtensionView
			 where Outstanding = 'N'
			   and CustomerCreatedDate BETWEEN @DateFrom AND @DateTo
			 group by CompanyCode, BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode
 join gnmstdealeroutletmapping c
		on a.CompanyCode = c.DealerCode
		and a.BranchCode = c.OutletCode
 left join gnmstdealerMapping d
		on a.CompanyCode = d.DealerCode
	 where a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END

end