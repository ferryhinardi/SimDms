

-- =============================================
-- Author:		fhy
-- Create date: 29122015
-- Description:	CsReportBPKBRemindfro export
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_CsRptBPKBReminderexport]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
AS
BEGIN
	declare @t_CsReportBPKBRemind as table
(
	[Kode Dealer] varchar(25),
	[Kode Outlet] varchar(25),
	[Nama Outlet                             ]   varchar(150),
	[Tanggan Siap] datetime,
	[Jumlah Customer]  int,
	[Input by CRO]  int,
	[Tidak dapat dihubungi]  int,
	[PERSENTASE]   Decimal(4,0) 
)

declare @t_CsReportBPKBRemindrpt as table
(	
	[Nama Outlet                             ]   varchar(150),
	[Tanggan Siap] varchar(25),
	[Jumlah Customer]  int,
	[Input by CRO]  int,
	[Tidak dapat dihubungi]  int,
	[PERSENTASE]   Decimal(4,0) 
)

insert into @t_CsReportBPKBRemind
select a.CompanyCode, a.BranchCode,d.OutletAbbreviation
		 , convert(datetime, left(convert(varchar, cast(a.BpkbReadyDate as datetime), 121), 7) + '-01') BpkbReadyDate
		 , sum(a.CustomerCount) CustomerCount
		 , sum(isnull(b.InputByCRO, 0)) InputByCRO
		 , sum(isnull(c.Unreachable, 0)) Unreachable
		 , (convert(numeric(5,2), sum(isnull(b.InputByCRO, 0))) / sum(a.CustomerCount) * 100) Percentation
	  from (
			 select BpkbReadyDate, CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuBpkbReminderView
			 group by BpkbReadyDate, CompanyCode, BranchCode
		   ) a
 left join (
			 select BpkbReadyDate, CompanyCode, BranchCode, count(CustomerCode) InputByCRO from CsLkuBpkbReminderView
			 where convert(varchar, InputDate, 121)
					between convert(varchar, year(BpkbReadyDate)) + '-' + right('0' + convert(varchar, month(BpkbReadyDate)), 2) + '-' + '01'
						and convert(varchar(7), dateadd(month, 1, InputDate), 121) + '07'--BpkbPickUp is not null
			 group by BpkbReadyDate, CompanyCode, BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.BpkbReadyDate = b.BpkbReadyDate
 left join (
			 select BpkbReadyDate, CompanyCode, BranchCode, count(CustomerCode) Unreachable from CsLkuBpkbReminderView
			 where Reason is not null
			 group by BpkbReadyDate, CompanyCode, BranchCode
		   ) c
		on a.CompanyCode = c.CompanyCode and a.BranchCode = c.BranchCode and a.BpkbReadyDate = c.BpkbReadyDate
 left join gnmstdealeroutletmapping d
		on a.CompanyCode = d.DealerCode
	   and a.BranchCode = d.OutletCode
	 where a.BpkbReadyDate is not null
	   and a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END
	   and cast(a.BpkbReadyDate as datetime) BETWEEN @DateFrom AND @DateTo
  group by left(convert(varchar, cast(a.BpkbReadyDate as datetime), 121), 7), d.OutletAbbreviation,a.CompanyCode, a.BranchCode


select 
	[Kode Dealer]
	,  [Kode Outlet]
from @t_CsReportBPKBRemind

insert into @t_CsReportBPKBRemindrpt
select 
	[Nama Outlet                             ] 
	, CONVERT(VARCHAR(11),[Tanggan Siap],106)
	,[Jumlah Customer]
	,[Input by CRO] 
	,[Tidak dapat dihubungi]
	,[PERSENTASE]
from @t_CsReportBPKBRemind

select * from @t_CsReportBPKBRemindrpt
union all
select 
	'Total'
	, ''
	,sum([Jumlah Customer])
	,sum([Input by CRO])
	,sum([Tidak dapat dihubungi])
	, case when (sum([Jumlah Customer]))=0 then 0
		else (sum([Input by CRO]) *100) /sum([Jumlah Customer]) end as 'Percent'
from @t_CsReportBPKBRemindrpt

delete @t_CsReportBPKBRemind
delete @t_CsReportBPKBRemindrpt
END

GO


