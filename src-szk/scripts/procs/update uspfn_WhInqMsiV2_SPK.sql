-- =============================================
-- Author:		fhi
-- Create date: 27.03.2015 modified 07.04.2015
-- Description:	inq suzuki msi v2
-- =============================================
ALTER PROCEDURE [dbo].[uspfn_WhInqMsiV2_SPK]
	@Area varchar(250),
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@PeriodYear varchar(20)
AS

BEGIN

IF OBJECT_ID('tempdb..#tempTable1') IS NOT NULL DROP TABLE #tempTable1
IF OBJECT_ID('tempdb..#tempTable2') IS NOT NULL DROP TABLE #tempTable2
IF OBJECT_ID('tempdb..#tempTable3') IS NOT NULL DROP TABLE #tempTable3
IF OBJECT_ID('tempdb..#svHstSzkMSIV2') IS NOT NULL DROP TABLE #svHstSzkMSIV2

if(@Area !='' and @CompanyCode!='' and @BranchCode!='')
	begin
		exec uspfn_WhInqMsiV2ByBranch_SPK @Area,@CompanyCode,@BranchCode,@PeriodYear
	end
else
	begin
		exec uspfn_WhInqMsiV2AllData_SPK @Area,@CompanyCode,@BranchCode,@PeriodYear
	end

END









