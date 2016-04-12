if object_id('uspfn_DlrInqMsiV2') is not null
       drop procedure uspfn_DlrInqMsiV2

-- =============================================
-- Author:		fhi
-- Create date: 27.03.2015 modified 07.04.2015
-- Description:	inq suzuki msi v2
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_DlrInqMsiV2]
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

if(@Area !='' and @CompanyCode!='' and @BranchCode!='')
	begin
		exec uspfn_DlrInqMsiV2ByBranch @Area,@CompanyCode,@BranchCode,@PeriodYear,@Month0,@Month1
	end
else
	begin
		exec uspfn_DlrInqMsiV2AllData @Area,@CompanyCode,@BranchCode,@PeriodYear
	end

END


