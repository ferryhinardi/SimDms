if object_id('uspfn_DlrInqMsiV2_SPK') is not null
       drop procedure uspfn_DlrInqMsiV2_SPK
	   

-- =============================================
-- Author:		fhi
-- Create date: 27.03.2015 modified 07.04.2015 // 09042015
-- Description:	inq suzuki msi v2
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_DlrInqMsiV2_SPK]
	@Area varchar(250),
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@PeriodYear varchar(20),
	@Month1     int
AS

BEGIN
declare
@Month0 int
set @Month0=1

IF OBJECT_ID('tempdb..#tempTable1') IS NOT NULL DROP TABLE #tempTable1
IF OBJECT_ID('tempdb..#tempTable2') IS NOT NULL DROP TABLE #tempTable2
IF OBJECT_ID('tempdb..#tempTable3') IS NOT NULL DROP TABLE #tempTable3
IF OBJECT_ID('tempdb..#svHstSzkMSIV2') IS NOT NULL DROP TABLE #svHstSzkMSIV2

if(@Area !='' and @CompanyCode!='' and @BranchCode!='')
	begin
		exec uspfn_DlrInqMsiV2ByBranch_SPK @Area,@CompanyCode,@BranchCode,@PeriodYear,@Month0,@Month1
	end
else
	begin
		exec uspfn_DlrInqMsiV2AllData_SPK @Area,@CompanyCode,@BranchCode,@PeriodYear,@Month0,@Month1
	end

END


