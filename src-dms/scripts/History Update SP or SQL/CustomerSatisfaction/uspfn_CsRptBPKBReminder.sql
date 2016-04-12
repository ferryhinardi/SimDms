IF (OBJECT_ID('uspfn_CsRptBPKBReminder') is not null)
	DROP PROC dbo.uspfn_CsRptBPKBReminder
GO

--uspfn_CsRptBPKBReminder '6006400131', '', ''
CREATE proc [dbo].[uspfn_CsRptBPKBReminder]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
as
begin
	select d.OutletAbbreviation
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
  group by left(convert(varchar, cast(a.BpkbReadyDate as datetime), 121), 7), d.OutletAbbreviation



end