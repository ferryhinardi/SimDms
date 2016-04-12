SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		SDMS - David
-- Create date: 5 Feb 2015
-- Description:	Menututup bulan service
-- =============================================
CREATE PROCEDURE uspfn_SvClosingMonth
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@FiscalYear int,
	@FiscalMonth int,
	@PeriodeNum int,
	@NextFiscalYear int,
	@NextPeriodeNum int,
	@ProfitCenterCode varchar(20),
	@Month int
AS
BEGIN

    update gnMstPeriode
   set StatusService = '2'
 where CompanyCode = @CompanyCode
   and BranchCode = @BranchCode
   and FiscalYear = @FiscalYear
   and FiscalMonth = @FiscalMonth
   and PeriodeNum = @PeriodeNum

update gnMstPeriode
   set StatusService = '1'
 where CompanyCode = @CompanyCode
   and BranchCode = @BranchCode
   and FiscalYear = @NextFiscalYear
   and PeriodeNum = @NextPeriodeNum
   and FiscalMonth = @FiscalMonth

update gnMstCoProfileService 
   set FiscalYear = @NextFiscalYear
      ,FiscalPeriod = @NextPeriodeNum
      ,PeriodBeg = isnull((
            select FromDate from gnMstPeriode
             where CompanyCode = @CompanyCode
               and BranchCode = @BranchCode
               and FiscalYear = @NextFiscalYear
               and PeriodeNum = @NextPeriodeNum
               and FiscalMonth = @FiscalMonth
           ),0)
      ,PeriodEnd = isnull((
            select EndDate from gnMstPeriode
             where CompanyCode = @CompanyCode
               and BranchCode = @BranchCode
               and FiscalYear = @NextFiscalYear
               and PeriodeNum = @NextPeriodeNum
               and FiscalMonth = @FiscalMonth
           ),0)
 where CompanyCode = @CompanyCode
   and BranchCode = @BranchCode

   IF (@Month = 12)
   BEGIN
   update gnMstDocument
   set DocumentYear = convert(int, @FiscalYear) + 1
      ,DocumentSequence = 0
      ,LastUpdateBy = 'closing_month'
      ,LastUpdateDate = GetDate()
 where CompanyCode = @CompanyCode
   and BranchCode = @BranchCode
   and ProfitCenterCode = @ProfitCenterCode
   and DocumentType <> 'LOT'
   END 
   
END
GO
