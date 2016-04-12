
-- =============================================
-- Author:		fhy
-- Create date: 29122015
-- Description:	CsChartStnkExtfor excel
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_CsChartStnkExtforexport] 
	@BranchCode varchar(25),
	@DateFrom datetime,
	@DateTo datetime
AS
BEGIN
	
	declare @t_CsChartStnkExt as table
(
	[No] int,
	[Kode Dealer] varchar(25),
	[Kode Outlet] varchar(25),
	[Area] varchar(25),
	[Dealer] varchar(25),
	[Nama Outlet                             ]   varchar(150),
	[Jumlah STNK]  int,
	[Input 3 STNK by CRO]  int,
	[PERSENTASE]   Decimal(4,0) 
)

declare @t_CsChartStnkExtrpt as table
(
	[No] int,
	[Nama Outlet                             ]   varchar(150),
	[Jumlah STNK]  int,
	[Input 3 STNK by CRO]  int,
	[PERSENTASE]   Decimal(4,0) 
)

insert into @t_CsChartStnkExt
	select row_number() over (order by a.CompanyCode), a.CompanyCode, a.BranchCode, d.Area, d.DealerAbbreviation AS Dealer, c.OutletName AS Outlet, a.CustomerCount, isnull(b.InputByCRO, 0) InputByCRO, isnull((convert(decimal, (b.InputByCRO / a.CustomerCount)) * 100), 0) Percentation
	  from (
			 select CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuStnkExtensionView
			 where CustomerCreatedDate BETWEEN @DateFrom and @DateTo
			 group by CompanyCode, BranchCode
		   ) a
 left join (
			 select CompanyCode, BranchCode, count(CustomerCode) InputByCRO from CsLkuStnkExtensionView
			 where Outstanding = 'N'
			   and InputDate BETWEEN @DateFrom AND @DateTo
			 group by CompanyCode, BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode
 join gnmstdealeroutletmapping c
		on a.CompanyCode = c.DealerCode
		and a.BranchCode = c.OutletCode
 left join gnmstdealerMapping d
		on a.CompanyCode = d.DealerCode
	 where a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END

select [Kode Dealer],[Kode Outlet],[Area] from @t_CsChartStnkExt

insert into @t_CsChartStnkExtrpt
select 
	row_number() over (order by [Kode Dealer])
	, [Nama Outlet                             ]  
	, [Jumlah STNK]
	, [Input 3 STNK by CRO] 
	, [PERSENTASE] 
from @t_CsChartStnkExt

select * from @t_CsChartStnkExtrpt

delete @t_CsChartStnkExt
delete @t_CsChartStnkExtrpt

END

GO


