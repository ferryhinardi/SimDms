
--uspfn_CsChart3DayCall '6006400131'
CREATE proc [dbo].[uspfn_CsChartTDayCallExport]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
as
begin
-- declare table 3dcall
declare @t_3dcall as table
(
	[Kode Outlet] varchar(25),
	[Nama Outlet                             ]   varchar(150),
	[Jumlah Delivery]  int,
	[Input 3 Days by CRO]  int,
	[PERSENTASE]   numeric(4,0) 
)

declare @t_3dcallrpt as table
(
	[Nama Outlet                             ]   varchar(150),
	[Jumlah Delivery]  int,
	[Input 3 Days by CRO]  int,
	[PERSENTASE]   decimal(4,0) 
)

insert into @t_3dcall
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

select [Kode Outlet],[Nama Outlet                             ] from @t_3dcall

insert into @t_3dcallrpt
select 
	[Nama Outlet                             ]
	, [Jumlah Delivery]
	, [Input 3 Days by CRO]
	, [PERSENTASE]
from @t_3dcall
union all
select 
	'Total'
	, sum([Jumlah Delivery])
	, sum([Input 3 Days by CRO])
	, case when (sum([Jumlah Delivery]))=0 then 0
		else (sum([Input 3 Days by CRO])*100) /sum([Jumlah Delivery]) end as 'Percent'
from @t_3dcall

select *
from @t_3dcallrpt

delete @t_3dcall
delete @t_3dcallrpt

end


GO


