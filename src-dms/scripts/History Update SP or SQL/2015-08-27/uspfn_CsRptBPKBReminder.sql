/****** Object:  StoredProcedure [dbo].[uspfn_CsReportBPKBReminder]    Script Date: 8/27/2015 4:09:14 PM ******/
IF (OBJECT_ID('uspfn_CsRptBPKBReminder') is not null)
	DROP PROC dbo.uspfn_CsRptBPKBReminder
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--uspfn_CsRptBPKBReminder '6006400131'
CREATE proc [dbo].[uspfn_CsRptBPKBReminder]
@BranchCode varchar(25),
@DateFrom datetime,
@DateTo datetime
as
begin
	select a.InputDate, a.CustomerCount, isnull(b.InputByCRO, 0) InputByCRO, isnull(c.Unreachable, 0) Unreachable, ((convert(numeric(5,2), isnull(b.InputByCRO, 0)) / a.CustomerCount) * 100) Percentation
	  from (
			 select convert(date, InputDate) InputDate, CompanyCode, BranchCode, count(CustomerCode) CustomerCount from CsLkuBpkbReminderView
			 group by convert(date, InputDate), CompanyCode, BranchCode
		   ) a
 left join (
			 select convert(date, InputDate) InputDate, CompanyCode, BranchCode, count(CustomerCode) InputByCRO from CsLkuBpkbReminderView
			 where BpkbPickUp is not null
			 group by convert(date, InputDate), CompanyCode, BranchCode
		   ) b
		on a.CompanyCode = b.CompanyCode and a.BranchCode = b.BranchCode and a.InputDate = b.InputDate
 left join (
			 select convert(date, InputDate) InputDate, CompanyCode, BranchCode, count(CustomerCode) Unreachable from CsLkuBpkbReminderView
			 where Reason is not null
			 group by convert(date, InputDate), CompanyCode, BranchCode
		   ) c
		on a.CompanyCode = c.CompanyCode and a.BranchCode = c.BranchCode and a.InputDate = c.InputDate
	 where a.BranchCode = CASE WHEN @BranchCode = '' THEN a.BranchCode ELSE @BranchCode END
	   and a.InputDate BETWEEN @DateFrom AND @DateTo


end