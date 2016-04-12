IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usprpt_PmRpInqFollowUpWeb]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usprpt_PmRpInqFollowUpWeb]
GO

CREATE procedure [dbo].[usprpt_PmRpInqFollowUpWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@PeriodBegin		DATETIME,
	@PeriodEnd			DATETIME,
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@Salesman			VARCHAR(15)
)
AS 
BEGIN
SET NOCOUNT ON;
----

IF(@SalesHead ='' AND @Salesman ='')
BEGIN
	SELECT * INTO #empl1 FROM (
		--SH =ALL AND S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND 
		a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader=@BranchManager)
		--a.TeamLeader IN (SELECT b.EmployeeID FROM HrEmployee b WHERE b.TeamLeader IN
		--(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader=@BranchManager))
	)#empl1

	SELECT * INTO #t1 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND e.NextFollowUpDate BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl1 g)
			--AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
			--AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
			--AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
	) #t1

	DROP TABLE #empl1
	SELECT InquiryNumber, Pelanggan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
	Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail FROM #t1 ORDER BY InquiryNumber
	DROP TABLE #t1

END
ELSE IF(@Salesman = '')
BEGIN
	SELECT * INTO #empl2 FROM (
		--S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  
		a.TeamLeader = @SalesHead
		--a.TeamLeader IN (SELECT b.EmployeeID FROM HrEmployee b WHERE b.TeamLeader=@SalesHead)
	)#empl2

	SELECT * INTO #t2 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND e.NextFollowUpDate BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl2 g)
			--AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
			--AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
			--AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
	) #t2

	DROP TABLE #empl2
	SELECT InquiryNumber, Pelanggan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
	Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail FROM #t2 ORDER BY InquiryNumber
	DROP TABLE #t2
END
ELSE
BEGIN
	SELECT * INTO #empl3 FROM (
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  
		a.EmployeeID=@Salesman
	)#empl3

	SELECT * INTO #t3 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND e.NextFollowUpDate BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl3 g)
			--AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
			--AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
			--AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
	) #t3

	DROP TABLE #empl3
	SELECT InquiryNumber, Pelanggan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
	Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail FROM #t3 ORDER BY InquiryNumber
	DROP TABLE #t3
END
----
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usprpt_PmRpInqFollowUpDtlNew]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usprpt_PmRpInqFollowUpDtlNew]
GO

CREATE procedure [dbo].[usprpt_PmRpInqFollowUpDtlNew] 
	-- Add the parameters for the stored procedure here
(
	@CompanyCode	VARCHAR(15),
	@BranchCode		VARCHAR(15),
	@DateAwal		DateTime,
	@DateAkhir		DateTime,
	@Outlet			VARCHAR(max),
	@EMP			VARCHAR(max),
	@Param			VARCHAR(1),
	@Head			VARCHAR(max)
)
AS
BEGIN
	SELECT * INTO #t1 FROM (
		SELECT
				a.EmployeeID
				,f.OutletName
				,a.InquiryNumber
				,a.NamaProspek Pelanggan
				,CASE(a.InquiryDate) WHEN '19000101' THEN NULL ELSE a.InquiryDate END InquiryDate
				,a.TipeKendaraan
				,a.Variant
				,a.Transmisi
				,isnull(b.RefferenceDesc1,'') Warna
				,a.PerolehanData
				,isnull(c.EmployeeName,'') Employee
				,d.EmployeeName TeamLeader
				,CASE(e.NextFollowUpDate) WHEN '19000101' THEN NULL ELSE e.NextFollowUpDate END NextFollowUpDate
				,a.LastProgress
				,a.AlamatProspek
				,a.TelpRumah
				,a.NamaPerusahaan
				,a.AlamatPerusahaan
				,a.Handphone
				,a.LastUpdateStatus
				,CASE(a.SPKDate) WHEN '19000101' THEN NULL ELSE a.SPKDate END SPKDate
				,CASE(a.SPKDate) WHEN '19000101' THEN NULL ELSE DAY(a.SPKDate) END DaySPKDate
				,CASE(a.SPKDate) WHEN '19000101' THEN NULL ELSE MONTH(a.SPKDate) END MonthSPKDate
				,CASE(a.SPKDate) WHEN '19000101' THEN NULL ELSE YEAR(a.SPKDate) END YearSPKDate
				,CASE(a.LostCaseDate) WHEN '19000101' THEN NULL ELSE a.LostCaseDate END LostCaseDate
				,CASE(a.InquiryDate) WHEN '19000101' THEN NULL ELSE DAY(a.InquiryDate) END DayInquiryDate
				,CASE(a.InquiryDate) WHEN '19000101' THEN NULL ELSE MONTH(a.InquiryDate) END MonthInquiryDate
				,CASE(a.InquiryDate) WHEN '19000101' THEN NULL ELSE YEAR(a.InquiryDate) END YearInquiryDate
				,CASE(e.NextFollowUpDate) WHEN '19000101' THEN NULL ELSE DAY(e.NextFollowUpDate) END DayNextFollowUpDate
				,CASE(e.NextFollowUpDate) WHEN '19000101' THEN NULL ELSE MONTH(e.NextFollowUpDate) END MonthNextFollowUpDate
				,CASE(e.NextFollowUpDate) WHEN '19000101' THEN NULL ELSE YEAR(e.NextFollowUpDate) END YearNextFollowUpDate
				,a.QuantityInquiry
				,isnull(r.LookUpValueName, '') LostCaseCategory
				,a.LostCaseVoiceOfCustomer
				,(select LookUpValueName from gnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'PMOP' and LookUpValue = a.TestDrive) TestDrive
				, isnull(n.LookUpValueName,'') CaraBayar
				, isnull(o.LookUpValueName,'') Leasing
				, isnull(p.LookUpValueName,'') DownPayment
				, isnull(q.LookUpValueName,'') Tenor
				, a.MerkLain
				, a.SpvEmployeeId
			FROM
				PmKDP a
			INNER JOIN OmMstRefference b ON b.CompanyCode = a.CompanyCode 
				AND b.RefferenceType='COLO' 
				AND b.RefferenceCode=a.ColourCode
			INNER JOIN HrEmployee c ON c.CompanyCode = a.CompanyCode 
				AND c.EmployeeID = a.EmployeeID
			INNER JOIN HrEmployee d ON d.CompanyCode = a.CompanyCode 
				AND c.TeamLeader = d.EmployeeID
			INNER JOIN PmActivities e ON e.CompanyCode = a.CompanyCode 
				AND e.BranchCode = a.BranchCode 
				AND e.InquiryNumber=a.InquiryNumber
				AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
				AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
			INNER JOIN PmBranchOutlets f ON f.CompanyCode = a.CompanyCode 
				AND f.BranchCode = a.BranchCode 
				AND f.OutletID = a.OutletID
			LEFT JOIN GnMstLookUpDtl n on n.CompanyCode=a.CompanyCode
				AND n.CodeId='PMBY'
				AND n.LookUpValue = a.CaraPembayaran
			LEFT JOIN GnMstLookUpDtl o on o.CompanyCode=a.CompanyCode
				AND o.CodeId='LSNG'
				AND o.LookUpValue = a.Leasing
			LEFT JOIN GnMstLookUpDtl p on p.CompanyCode=a.CompanyCode
				AND p.CodeId='DWPM'
				AND p.LookUpValue = a.DownPayment
			LEFT JOIN GnMstLookUpDtl q on q.CompanyCode=a.CompanyCode
				AND q.CodeId='TENOR'
				AND q.LookUpValue = a.Tenor
			LEFT JOIN GnMstLookUpDtl r on r.CompanyCode=a.CompanyCode
				AND r.CodeId='PLCC'
				AND r.LookUpValue = a.LostCaseCategory			
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND e.NextFollowUpDate BETWEEN @DateAwal AND @DateAkhir
			AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
			AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
	) #t1

	IF (@Param = '1')
	BEGIN
		DELETE #t1
		INSERT INTO #t1
			SELECT
				a.EmployeeID
				,f.OutletName
				,a.InquiryNumber
				,a.NamaProspek Pelanggan
				,CASE(a.InquiryDate) WHEN '19000101' THEN NULL ELSE a.InquiryDate END InquiryDate
				,a.TipeKendaraan
				,a.Variant
				,a.Transmisi
				,isnull(b.RefferenceDesc1,'') Warna
				,a.PerolehanData
				,isnull(c.EmployeeName,'') Employee
				,d.EmployeeName TeamLeader
				,CASE(e.NextFollowUpDate) WHEN '19000101' THEN NULL ELSE e.NextFollowUpDate END NextFollowUpDate
				,a.LastProgress
				,a.AlamatProspek
				,a.TelpRumah
				,a.NamaPerusahaan
				,a.AlamatPerusahaan
				,a.Handphone
				,a.LastUpdateStatus
				,CASE(a.SPKDate) WHEN '19000101' THEN NULL ELSE a.SPKDate END SPKDate
				,CASE(a.SPKDate) WHEN '19000101' THEN NULL ELSE DAY(a.SPKDate) END DaySPKDate
				,CASE(a.SPKDate) WHEN '19000101' THEN NULL ELSE MONTH(a.SPKDate) END MonthSPKDate
				,CASE(a.SPKDate) WHEN '19000101' THEN NULL ELSE YEAR(a.SPKDate) END YearSPKDate
				,CASE(a.LostCaseDate) WHEN '19000101' THEN NULL ELSE a.LostCaseDate END LostCaseDate
				,CASE(a.InquiryDate) WHEN '19000101' THEN NULL ELSE DAY(a.InquiryDate) END DayInquiryDate
				,CASE(a.InquiryDate) WHEN '19000101' THEN NULL ELSE MONTH(a.InquiryDate) END MonthInquiryDate
				,CASE(a.InquiryDate) WHEN '19000101' THEN NULL ELSE YEAR(a.InquiryDate) END YearInquiryDate
				,CASE(e.NextFollowUpDate) WHEN '19000101' THEN NULL ELSE DAY(e.NextFollowUpDate) END DayNextFollowUpDate
				,CASE(e.NextFollowUpDate) WHEN '19000101' THEN NULL ELSE MONTH(e.NextFollowUpDate) END MonthNextFollowUpDate
				,CASE(e.NextFollowUpDate) WHEN '19000101' THEN NULL ELSE YEAR(e.NextFollowUpDate) END YearNextFollowUpDate
				,a.QuantityInquiry
				,isnull(r.LookUpValueName, '') LostCaseCategory
				,a.LostCaseVoiceOfCustomer
				,(select LookUpValueName from gnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'PMOP' and LookUpValue = a.TestDrive) TestDrive
				, isnull(n.LookUpValueName,'') CaraBayar
				, isnull(o.LookUpValueName,'') Leasing
				, isnull(p.LookUpValueName,'') DownPayment
				, isnull(q.LookUpValueName,'') Tenor
				, a.MerkLain
				, a.SpvEmployeeId
			FROM
				PmKDP a
			LEFT JOIN OmMstRefference b ON b.CompanyCode = a.CompanyCode 
				AND b.RefferenceType='COLO' 
				AND b.RefferenceCode=a.ColourCode
			LEFT JOIN HrEmployee c ON c.CompanyCode = a.CompanyCode 
				AND c.EmployeeID = a.EmployeeID
			LEFT JOIN HrEmployee d ON d.CompanyCode = a.CompanyCode 
				AND c.TeamLeader = d.EmployeeID
			LEFT JOIN PmActivities e ON e.CompanyCode = a.CompanyCode 
				AND e.BranchCode = a.BranchCode 
				AND e.InquiryNumber=a.InquiryNumber
				AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
				AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
			LEFT JOIN PmBranchOutlets f ON f.CompanyCode = a.CompanyCode 
				AND f.BranchCode = a.BranchCode 
				AND f.OutletID = a.OutletID
			LEFT JOIN GnMstLookUpDtl n on n.CompanyCode=a.CompanyCode
				AND n.CodeId='PMBY'
				AND n.LookUpValue = a.CaraPembayaran
			LEFT JOIN GnMstLookUpDtl o on o.CompanyCode=a.CompanyCode
				AND o.CodeId='LSNG'
				AND o.LookUpValue = a.Leasing
			LEFT JOIN GnMstLookUpDtl p on p.CompanyCode=a.CompanyCode
				AND p.CodeId='DWPM'
				AND p.LookUpValue = a.DownPayment
			LEFT JOIN GnMstLookUpDtl q on q.CompanyCode=a.CompanyCode
				AND q.CodeId='TENOR'
				AND q.LookUpValue = a.Tenor
			LEFT JOIN GnMstLookUpDtl r on r.CompanyCode=a.CompanyCode
				AND r.CodeId='PLCC'
				AND r.LookUpValue = a.LostCaseCategory		
			WHERE
				a.CompanyCode = @CompanyCode 
				AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
				AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
				AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
				AND e.NextFollowUpDate BETWEEN @DateAwal AND @DateAkhir
				AND a.LastProgress in ('P','HP','SPK') 
				--AND not exists (select 1 from #t1 where a.InquiryNumber = #t1.InquiryNumber)
	END
	
	IF (@Head='')
	BEGIN
		SELECT * FROM #t1
		ORDER BY InquiryNumber
	END
	ELSE
	BEGIN
		SELECT * FROM #t1 
		WHERE EmployeeID IN ( SELECT EmployeeID FROM HrEmployee where TeamLeader  = @Head)
		ORDER BY InquiryNumber 
	END
	DROP TABLE #t1
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usprpt_PmRpInqLostCaseWeb]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usprpt_PmRpInqLostCaseWeb]
GO

-- =============================================
-- Author:		<Rijal AL>
-- Create date: <26 Mar 2015>
-- Description:	<Analisis Lost Case For Web>
-- =============================================

CREATE PROCEDURE [dbo].[usprpt_PmRpInqLostCaseWeb] 
@CompanyCode		VARCHAR(15) , --= '6159401000',
	@BranchCode			VARCHAR(15) , --= '6159401001',
	@PeriodBegin		VARCHAR(15) , --= '20140101',
	@PeriodEnd			VARCHAR(15) , --= '20140330',
	@BranchManager		VARCHAR(15) , --= '3-BIT',
	@SalesHead			VARCHAR(15) , --= '028',
	@SalesCoordinator	VARCHAR(15) , --= '028',
	@Salesman			VARCHAR(15) --= ''


AS
BEGIN
	SET NOCOUNT ON;
-- Get EmployeeID
--=======================================================================
DECLARE @SalesmanID		VARCHAR(MAX);

if @SalesHead = '' and @Salesman = ''
begin
set @SalesmanID = 'select EmployeeID from HrEmployee where TeamLeader in (
			select EmployeeID from HrEmployee where TeamLeader = '''+@BranchManager+''')'
end
else if @SalesHead <> '' and @Salesman = ''
begin
set @SalesmanID = 'select EmployeeID from HrEmployee where TeamLeader  = '''+@SalesHead+''''
end
else
begin
set @SalesmanID = 'select EmployeeID from HrEmployee where EmployeeID  = '''+@Salesman+''''
end
--=======================================================================

-- Group By Tipe Kendaraan
--=======================================================================
DECLARE @ByTipeKendaraan		VARCHAR(MAX);
DECLARE @Query1		VARCHAR(MAX);

set @ByTipeKendaraan = 'select
	 a.CompanyCode, a.BranchCode, a.InquiryNumber, a.TipeKendaraan, a.EmployeeID, a.LastProgress
	from 
	 PMKDP a 
	where
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+convert(varchar(30),@PeriodBegin)+''' AND '''+convert(varchar(30),@PeriodEnd)+''')
	 and EmployeeID in ('+@SalesmanID+')
	 and a.TipeKendaraan <> '''''

set @Query1 = 'SELECT 
	DISTINCT(TipeKendaraan),
	(select count(*) from ('+@ByTipeKendaraan+') b where lastprogress <> ''LOST'' AND a.TipeKendaraan = b.TipeKendaraan) Active, 
	(select count(*) from ('+@ByTipeKendaraan+') b where lastprogress = ''LOST'' AND a.TipeKendaraan = b.TipeKendaraan) NonActive
	FROM ('+@ByTipeKendaraan+') a'
	
exec (@Query1)

-- Group By Perolehan Data
--======================================================================
DECLARE @ByPerolehanData		VARCHAR(MAX);
DECLARE @Query2		VARCHAR(MAX);

set @ByPerolehanData = 'select
	 a.CompanyCode, a.BranchCode, a.InquiryNumber, a.PerolehanData, a.EmployeeID, a.LastProgress
	from 
	 PMKDP a 
	where
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and EmployeeID in ('+@SalesmanID+')'

set @Query2 = 'SELECT 
	DISTINCT(PerolehanData),
	(select count(*) from ('+@ByPerolehanData+') b where lastprogress <> ''LOST'' AND a.PerolehanData = b.PerolehanData) Active, 
	(select count(*) from ('+@ByPerolehanData+') b where lastprogress = ''LOST'' AND a.PerolehanData = b.PerolehanData) NonActive
	FROM ('+@ByPerolehanData+') a'
	
exec (@Query2)

-- Group By Salesman
--=====================================================================
DECLARE @BySalesman		VARCHAR(MAX);
DECLARE @Query3		VARCHAR(MAX);

set @BySalesman = 'select
	 a.CompanyCode, a.BranchCode, a.InquiryNumber, b.EmployeeName, a.EmployeeID, a.LastProgress
	from 
	 PMKDP a
	inner join HrEmployee b
		ON b.CompanyCode = a.CompanyCode and b.EmployeeID = a.EmployeeID
	where
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and a.EmployeeID in ('+@SalesmanID+')'

set @Query3 = 'SELECT 
	DISTINCT (EmployeeName) Karyawan,
	(select count(*) from ('+@BySalesman+') b where lastprogress <> ''LOST'' AND a.EmployeeID = b.EmployeeID) Active, 
	(select count(*) from ('+@BySalesman+') b where lastprogress = ''LOST'' AND a.EmployeeID = b.EmployeeID) NonActive
	FROM ('+@BySalesman+') a'
	
exec (@Query3)

-- Group By Sales Coordinator
--=====================================================================
DECLARE @BranchName		VARCHAR(MAX);
DECLARE @Query4		VARCHAR(MAX);
set @BranchName = 'SELECT 
	  a.CompanyCode, a.BranchCode, a.InquiryNumber, a.LastProgress, a.SpvEmployeeID, b.BranchName 
	 FROM PMKDP a 
	 LEFT JOIN GnMstOrganizationDtl b
	 ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
	 WHERE
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and a.EmployeeID in ('+@SalesmanID+')'

set @Query4 = 'SELECT 
	DISTINCT (BranchName) Supervisor,
	(select count(*) from ('+@BranchName+') b where lastprogress <> ''LOST'' AND a.BranchName = b.BranchName) Active, 
	(select count(*) from ('+@BranchName+') b where lastprogress = ''LOST'' AND a.BranchName = b.BranchName) NonActive
	FROM ('+@BranchName+') a'
	
exec (@Query4)

-- Group By Sales Head
--=====================================================================
--DECLARE @BranchName		VARCHAR(MAX);
DECLARE @Query5		VARCHAR(MAX);

set @BranchName = 'SELECT 
	  a.CompanyCode, a.BranchCode, a.InquiryNumber, a.LastProgress, a.SpvEmployeeID, b.BranchName 
	 FROM PMKDP a 
	 LEFT JOIN GnMstOrganizationDtl b
	 ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
	 WHERE
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and a.EmployeeID in ('+@SalesmanID+')'

set @Query5 = 'SELECT 
	DISTINCT (BranchName) SalesHead,
	(select count(*) from ('+@BranchName+') b where lastprogress <> ''LOST'' AND a.BranchName = b.BranchName) Active, 
	(select count(*) from ('+@BranchName+') b where lastprogress = ''LOST'' AND a.BranchName = b.BranchName) NonActive
	FROM ('+@BranchName+') a'
	
exec (@Query5)

-- Group By Branch Name
--=======================================================================
--DECLARE @BranchName		VARCHAR(MAX);
DECLARE @Query6		VARCHAR(MAX);

set @BranchName = 'SELECT 
	  a.CompanyCode, a.BranchCode, a.InquiryNumber, a.LastProgress, a.SpvEmployeeID, b.BranchName 
	 FROM PMKDP a 
	 LEFT JOIN GnMstOrganizationDtl b
	 ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode
	 WHERE
	 a.CompanyCode = '''+@CompanyCode+'''
	 and a.BranchCode = '''+@BranchCode+'''
	 and a.InquiryNumber IN (SELECT DISTINCT InquiryNumber FROM PmStatusHistory WHERE CompanyCode = a.CompanyCode 
		and BranchCode = a.BranchCode AND LastProgress=''LOST'' AND CONVERT(VARCHAR, UpdateDate, 112) 
		BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''')
	 and a.EmployeeID in ('+@SalesmanID+')'

set @Query3 = 'SELECT 
	DISTINCT (BranchName),
	(select count(*) from ('+@BranchName+') b where lastprogress <> ''LOST'' AND a.BranchName = b.BranchName) Active, 
	(select count(*) from ('+@BranchName+') b where lastprogress = ''LOST'' AND a.BranchName = b.BranchName) NonActive
	FROM ('+@BranchName+') a'
	
exec (@Query3)

-- Query Utama
--=======================================================================================
DECLARE @Utama		VARCHAR(MAX);

set @Utama = 'SELECT
 a.CompanyCode, a.BranchCode, a.InquiryNumber, a.NamaProspek, a.Inquirydate, ISNULL(a.TipeKendaraan,''-'') TipeKendaraan, 
 ISNULL(a.Variant,''-'') Variant, ISNULL(a.Transmisi,''-'') Transmisi,
 b.RefferenceDesc1 Warna, a.PerolehanData, c.EmployeeName Employee, d.EmployeeName Supervisor,
 a.LastProgress, e.UpdateDate TglLost, f.LookUpValueName KategoriLost, g.LookUpValueName Reason,
 a.LostCaseVoiceofCustomer VOC, a.SpvEmployeeID
FROM
 PmKDP a
LEFT JOIN OmMstRefference b
ON b.CompanyCode = a.CompanyCode AND b.RefferenceType = ''COLO'' AND b.RefferenceCode = a.ColourCode
LEFT JOIN HrEmployee c
ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
LEFT JOIN HrEmployee d
ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
LEFT JOIN PmStatusHistory e
ON e.InquiryNumber = a.InquiryNumber AND e.CompanyCode = a.CompanyCode 
AND e.BranchCode = a.BranchCode AND e.SequenceNo = (SELECT TOP 1 SequenceNo FROM PmStatusHistory
		WHERE InquiryNumber = a.InquiryNumber AND CompanyCode = a.CompanyCode 
		AND BranchCode = a.BranchCode AND LastProgress=''LOST'' ORDER BY SequenceNo DESC)
LEFT JOIN GnMstLookUpDtl f
ON f.CompanyCode = a.CompanyCode AND f.CodeID = ''PLCC'' AND f.LookUpValue = a.LostCaseCategory
LEFT JOIN GnMstLookUpDtl g
ON g.CompanyCode = a.CompanyCode AND g.CodeID = ''ITLR'' AND g.LookUpValue = a.LostCaseReasonID
WHERE
 a.CompanyCode = '''+@CompanyCode+''' 
 AND a.BranchCode = '''+@BranchCode+'''
 AND a.LastProgress = ''LOST'' 
 AND CONVERT(VARCHAR, e.UpdateDate, 112) BETWEEN '''+@PeriodBegin+''' AND '''+@PeriodEnd+''' 
 AND a.EmployeeID in ('+@SalesmanID+')'

Exec (@Utama)

-- Pivot
--=====================================================================
declare	@columns			VARCHAR(MAX)
declare	@columns2			VARCHAR(MAX)
declare	@Pivot				VARCHAR(MAX)

select	@columns = coalesce(@columns + ',[' + cast(LookUpValue as varchar) + ']',
				'[' + cast(LookUpValue as varchar)+ ']') 
		,@columns2 = coalesce(@columns2 + ',isnull([' + cast(LookUpValue as varchar) + '],0) as '+ LookUpValue +'',
		'isnull([' + cast(LookUpValue as varchar)+ '],0) as '+ LookUpValue +'')
from
(
	select	a.LookUpValue
	from	gnMstLookUpDtl a
	where	CompanyCode=@CompanyCode and CodeID='PLCC'
) as x

set @Pivot='
select 
	p.TipeKendaraan, '+ @columns2 +'
from (
	select 
		a.TipeKendaraan
		,d.LookupValue LostCaseCategory
		,count(d.LookupValue) Quantity
	from 
		pmKDP a	
	left join PmStatusHistory b
	on b.InquiryNumber = a.InquiryNumber AND b.CompanyCode = a.CompanyCode 
	and b.BranchCode = a.BranchCode and b.SequenceNo = (select top 1 SequenceNo from PmStatusHistory
			where InquiryNumber = a.InquiryNumber and CompanyCode = a.CompanyCode 
			and BranchCode = a.BranchCode and LastProgress=''LOST'' order by SequenceNo desc)
	left join gnMstLookUpDtl d on d.CompanyCode=a.CompanyCode and CodeID=''PLCC'' 
		and LookUpValue=a.LostCaseCategory
	where
		a.LastProgress= ''LOST''
		and a.EmployeeID in ('+@SalesmanID+')
		and convert(varchar, b.UpdateDate, 112) between '''+@PeriodBegin+''' and '''+@PeriodEnd+''' 
	group by
		a.CompanyCode,a.BranchCode,d.LookupValue,a.TipeKendaraan
) as b
pivot
(
	sum(Quantity)
	for LostCaseCategory 
	in ('+@columns+')
) as p
order by p.TipeKendaraan'


exec(@Pivot)



-- Get GnMstLookUpDtl (Kategori Lost Case) 
--===========================
SELECT LookupValue+' : '+LookUpValueName AS Kategori, LookupValue FROM gnMstLookupDtl 
WHERE CompanyCode=@CompanyCode AND CodeID='PLCC'

END
GO


alter view [dbo].[SvSaView]
as 
select a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName  from GnMstEmployee a
where a.TitleCode IN ('3')
   AND PersonnelStatus = '1'
GO

if object_id('[uspfn_checkdailyaccess]') is not null
	drop procedure [uspfn_checkdailyaccess]
GO
CREATE procedure [dbo].[uspfn_checkdailyaccess]
@userid varchar(32)
as
if exists (select top 1 1 from sysRoleMenu
where RoleId = (select roleid from sysroleUser where userid=@userid)
and menuid = 'gnpostdaily')
	select 1 [result]
else
	select 0 [result]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usprpt_PmRpInqOutStanding_NewByType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usprpt_PmRpInqOutStanding_NewByType]
GO

CREATE procedure [dbo].[usprpt_PmRpInqOutStanding_NewByType] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@Period				DATETIME,
	@COO				VARCHAR(15),
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@SalesCoordinator	VARCHAR(15),
	@Salesman			VARCHAR(15)
	
)
AS 
BEGIN
SET NOCOUNT ON;
SELECT * INTO #dByTipe FROM(
			SELECT b.EmployeeID, (b.TipeKendaraan + ' ' + b.Variant) ModelKendaraan, LastProgress, StatusProspek FROM PmKdp b 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.ModelKendaraan,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.ModelKendaraan = a.ModelKendaraan) SPK
			FROM #dByTipe a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.ModelKendaraan
		)#dSls

		IF @COO = ''
		BEGIN
		IF @SalesHead = '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.ModelKendaraan TipeKendaraan,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))  
				AND a.ModelKendaraan <> ''
				GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan
		END
		ELSE IF @SalesHead <> '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.ModelKendaraan TipeKendaraan,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader = @SalesHead
					AND a.ModelKendaraan <> ''
				GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan	
		END
		ELSE IF @SalesHead = '' AND @SalesCoordinator <> '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.ModelKendaraan TipeKendaraan,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader = @SalesCoordinator
					AND a.ModelKendaraan <> ''
				GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan	
		END
		ELSE IF (@SalesHead <> '' OR @SalesCoordinator <> '') AND @Salesman <> ''
			SELECT
					a.EmployeeID,
					a.Position,
					a.ModelKendaraan TipeKendaraan,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.EmployeeID = @Salesman
					AND a.ModelKendaraan <> ''   
				GROUP BY a.EmployeeID, a.Position, a.ModelKendaraan

		DROP TABLE #dSls
		DROP TABLE #dByTipe
		END
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usprpt_PmRpInqOutStanding_NewBySalesman]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usprpt_PmRpInqOutStanding_NewBySalesman]
GO

CREATE procedure [dbo].[usprpt_PmRpInqOutStanding_NewBySalesman] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@Period				DATETIME,
	@COO				VARCHAR(15),
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@SalesCoordinator	VARCHAR(15),
	@Salesman			VARCHAR(15)
	
)
AS 
BEGIN
SET NOCOUNT ON;

-- TABLE INITIAL
--===============================================================================================================================
	SELECT * INTO #employee_stat_SM FROM(
		SELECT 
			a.CompanyCode,
			a.EmployeeID,
			a.EmployeeName,
			a.Position,
			'Salesman' PositionName, 
			a.TeamLeader,
			(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastProgress = 'P' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) PROSPECT,
			(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastProgress = 'HP' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) HOTPROSPECT,
			(SELECT COUNT(StatusProspek) FROM PmKdp b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastProgress = 'SPK' AND (b.EmployeeID = a.EmployeeID) AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)) SPK
		FROM HrEmployee a
		WHERE a.CompanyCode = @CompanyCode  AND a.Department = 'SALES'
			AND a.TeamLeader IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @BranchManager)
	)#employee_stat_SM

	SELECT * INTO #employee_stat_SK FROM(
		SELECT
			a.CompanyCode,
			a.TeamLeader EmployeeID,
			b.EmployeeName,
			'SC' Position,
			'Sales Coordinator' PositionName, 
			b.TeamLeader ShEmployeeID,
			ISNULL(SUM(a.PROSPECT),0) PROSPECT,
			ISNULL(SUM(a.HOTPROSPECT),0) HOTPROSPECT,
			ISNULL(SUM(a.SPK),0) SPK
		FROM #employee_stat_SM a
		LEFT JOIN HrEmployee b
			ON b.CompanyCode = a.CompanyCode AND a.TeamLeader = b.EmployeeID
		WHERE b.TeamLeader  = @BranchManager
		GROUP BY a.CompanyCode,
			b.EmployeeName,
			a.TeamLeader, b.TeamLeader
	)#employee_stat_SK

	SELECT * INTO #employee_stat_SH FROM(
		SELECT
			a.CompanyCode,
			a.TeamLeader EmployeeID,
			b.EmployeeName,
			'SH' PositionID,
			'Sales Head' PositionName, 
			b.TeamLeader BMEmployeeID,
			ISNULL(SUM(a.PROSPECT),0) PROSPECT,
			ISNULL(SUM(a.HOTPROSPECT),0) HOTPROSPECT,
			ISNULL(SUM(a.SPK),0) SPK
		FROM #employee_stat_SM a
		LEFT JOIN HrEmployee b
			ON b.CompanyCode = a.CompanyCode AND a.TeamLeader = b.EmployeeID
		WHERE b.TeamLeader = @BranchManager
		GROUP BY a.CompanyCode,
			b.EmployeeName,
			a.TeamLeader, b.TeamLeader
	)#employee_stat_SH

	SELECT * INTO #employee_stat_BM FROM(
		SELECT
			a.CompanyCode,
			a.BMEmployeeID EmployeeID,
			b.EmployeeName,
			'BM' PositionID,
			'Branch Manager' PositionName, 
			'' TeamLeader,
			ISNULL(SUM(a.PROSPECT),0) PROSPECT,
			ISNULL(SUM(a.HOTPROSPECT),0) HOTPROSPECT,
			ISNULL(SUM(a.SPK),0) SPK
		FROM #employee_stat_SH a
		LEFT JOIN HrEmployee b
			ON b.CompanyCode = a.CompanyCode AND a.BMEmployeeID = b.EmployeeID
		WHERE a.BMEmployeeID = @BranchManager
		GROUP BY a.CompanyCode,
			b.EmployeeName,
			a.BMEmployeeID
	)#employee_stat_BM

SELECT * INTO #employee_stat FROM(
	SELECT '3' PositionId, a.* FROM #employee_stat_SM a
	UNION
	SELECT '2' PositionId, a.* FROM #employee_stat_SH a
	UNION
	SELECT '1' PositionId, a.* FROM #employee_stat_BM a
	) #employee_stat

	DROP TABLE #employee_stat_SM
	DROP TABLE #employee_stat_SK
	DROP TABLE #employee_stat_SH
	DROP TABLE #employee_stat_BM

IF @COO = ''
	BEGIN
-- == CASE I ==
		IF @SalesHead = '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN		
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode
		END
-- == CASE II ==
		ELSE IF @SalesHead <> '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @BranchManager
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesHead
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @SalesHead)
		END
-- == CASE III ==
		ELSE IF @SalesHead = '' AND @SalesCoordinator <> '' AND @Salesman = ''
		BEGIN
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @BranchManager
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesCoordinator
			UNION			
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID IN (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @SalesCoordinator)
		END
-- == CASE IV ==
		ELSE IF (@SalesHead <> '' OR @SalesCoordinator <> '') AND @Salesman <> ''
		BEGIN
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @BranchManager
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesHead
			UNION
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @SalesCoordinator
			UNION			
			SELECT * FROM  #employee_stat WHERE CompanyCode = @CompanyCode AND EmployeeID = @Salesman
		END
	END
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[usprpt_PmRpInqOutStanding_NewByData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[usprpt_PmRpInqOutStanding_NewByData]
GO

CREATE procedure [dbo].[usprpt_PmRpInqOutStanding_NewByData] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@Period				DATETIME,
	@COO				VARCHAR(15),
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@SalesCoordinator	VARCHAR(15),
	@Salesman			VARCHAR(15)
	
)
AS 
BEGIN
SET NOCOUNT ON;
SELECT * INTO #dByTipe FROM(
			SELECT b.EmployeeID, b.PerolehanData, LastProgress, StatusProspek FROM PmKdp b 
			WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode
			AND b.LastUpdateDate <=  CONVERT(VARCHAR, @Period, 112)
		)#dByTipe

		SELECT * INTO #dSls FROM (
			SELECT 
				a.EmployeeID,
				c.Position,
				c.TeamLeader,
				a.PerolehanData Source,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'P' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) PROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'HP' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) HOTPROSPECT,
				(SELECT COUNT(StatusProspek) FROM #dByTipe b WHERE b.LastProgress = 'SPK' AND b.EmployeeID = a.EmployeeID 
				AND b.PerolehanData = a.PerolehanData) SPK
			FROM #dByTipe a, HrEmployee c WHERE a.EmployeeID = c.EmployeeID
			GROUP BY a.EmployeeID, c.Position, c.TeamLeader, a.PerolehanData
		)#dSls

		IF @COO = ''
		BEGIN
		IF @SalesHead = '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData Source,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader IN
				(SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader = @BranchManager))   
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData
		END
		ELSE IF @SalesHead <> '' AND @SalesCoordinator = '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData Source,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader = @SalesHead
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData
		END
		ELSE IF @SalesHead = '' AND @SalesCoordinator <> '' AND @Salesman = ''
		BEGIN
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData Source,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.TeamLeader = @SalesCoordinator
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData	
		END
		ELSE IF (@SalesHead <> '' OR @SalesCoordinator <> '') AND @Salesman <> ''
			SELECT
					a.EmployeeID,
					a.Position,
					a.PerolehanData,
					SUM(a.PROSPECT) PROSPECT,
					SUM(a.HOTPROSPECT) HOTPROSPECT,
					SUM(a.SPK) SPK
				FROM #dSls a WHERE a.EmployeeID = @Salesman  
				AND a.PerolehanData <> ''
				GROUP BY a.EmployeeID, a.Position, a.PerolehanData

		DROP TABLE #dSls
		DROP TABLE #dByTipe
		END
END

GO

if object_id('uspfn_GetSpTrnSFPJHdr') is not null
	drop procedure uspfn_GetSpTrnSFPJHdr
GO
create procedure [dbo].[uspfn_GetSpTrnSFPJHdr] 
@CompanyCode varchar(15), @BranchCode varchar(15)  
as  
--declare @CompanyCode varchar(15), @BranchCode varchar(15)  
--set @CompanyCode = '6159401'
--set @BranchCode = '615940100'

select a.FPJNo, a.FPJdate, a.FPJGovNo, a.InvoiceNo, a.PickingSlipNo, a.CustomerCode, b.CustomerName, b.Address1, b.Address2, b.Address3, b.Address4, 
a.TOPDays, a.TOPCode, a.CustomerCode + ' - ' + b.CustomerName Customer 
from spTrnSFPJHdr a  
join gnMstCustomer b  
on b.CustomerCode = a.CustomerCode  
and b.CompanyCode = a.CompanyCode  
where a.CompanyCode = @CompanyCode  
and a.BranchCode = @BranchCode 
order by FPJDate desc
go

if object_id('uspfn_SvTrnServiceSelectDtl') is not null
	drop procedure uspfn_SvTrnServiceSelectDtl
GO
CREATE procedure [dbo].[uspfn_SvTrnServiceSelectDtl]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@ServiceNo   bigint
as      

begin

declare @t1 as table
(
 TaskPartSeq int
,BillType varchar(10)
,BillTypeDesc varchar(max)
,TypeOfGoods varchar(10)
,TypeOfGoodsDesc varchar(70)
,TaskPartNo varchar(50)
,OprHourDemandQty numeric(18,2)
,SupplyQty numeric(18,2)
,ReturnQty numeric(18,2)
,OprRetailPrice numeric(18,2)
,OprRetailPriceTotal numeric(18,2)
,SupplySlipNo varchar(20)
,TaskPartDesc varchar(max)
,BasicModel varchar(15)
,JobType varchar(15)
,IsSubCon bit
,Status varchar(10)
,GTGO varchar(10)
,DiscPct numeric(18,2)
,QtyAvail numeric(18,2)
,TaskStatus varchar(50)
)

declare @JobOrderNo varchar(15)
    set @JobOrderNo = isnull((select JobOrderNo from svTrnService where CompanyCode = @CompanyCode and BranchCode = @BranchCode and ProductType = @ProductType and ServiceNo = @ServiceNo), '')

insert into @t1
select 0 TaskSeq 
      ,a.BillType
      ,b.Description BillTypeDesc
      ,a.TypeOfGoods
      ,case a.TypeOfGoods when 'L' then 'Labor (Jasa)' end TypeOfGoodsDesc
      ,a.OperationNo
      ,a.OperationHour
      ,0 OperationHourSupply
      ,0 OperationHourReturn
      ,a.OperationCost
      ,a.OperationHour * a.OperationCost * (100 - a.DiscPct) * 0.01 OprRetailPriceTotal
      ,'' SupplySlipNo
      ,rtrim(d.Description) OperationDesc 
	  ,c.BasicModel
	  ,c.JobType
	  ,a.IsSubCon
	  ,(select min(MechanicStatus) from svTrnSrvMechanic 
		where CompanyCode = a.CompanyCode 
			and BranchCode = a.BranchCode
			and ProductType = a.ProductType
			and ServiceNo = a.ServiceNo
			and OperationNo = a.OperationNo) MechanicStatus
	  ,''
	  ,a.DiscPct
	  ,0
      ,case(a.TaskStatus)
          when 0 then 'Open Task'
          when 1 then 'Work In Progress'
          when 2 then 'Close Task'
          when 9 then 'Cancel'
       end TaskStatus
  from svTrnSrvTask a with (nolock,nowait)
  left join svMstBillingType b with (nolock,nowait)
    on b.CompanyCode = a.CompanyCode
   and b.BillType = a.BillType
  left join svTrnService c with (nolock,nowait)
    on c.CompanyCode = a.CompanyCode
   and c.BranchCode = a.BranchCode
   and c.ProductType = a.ProductType
   and c.ServiceNo = a.ServiceNo
  left join svMstTask d with (nolock,nowait)
    on d.CompanyCode = a.CompanyCode
   and d.ProductType = a.ProductType
   and d.BasicModel = c.BasicModel
   and (d.JobType = c.JobType or d.JobType = 'CLAIM' or d.JobType = 'OTHER')
   and d.OperationNo = a.OperationNo 
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ProductType = @ProductType
   and a.ServiceNo   = @ServiceNo

declare @tblTemp as table
(
	PartNo  varchar(20),
	QtyAvail decimal
)

declare @DealerCode as varchar(2)
declare @CompanyMD as varchar(15)
declare @BranchMD as varchar(15)

set @DealerCode = 'MD'
set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)

if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)
begin	
	set @DealerCode = 'SD'
	declare @DbName as varchar(50)
	set @DbName = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
	
	declare @QueryTemp as varchar(max)
	
	set @QueryTemp = 'select 
			 distinct
			 a.PartNo
			 , (b.OnHand - (b.AllocationSP + b.AllocationSR + b.AllocationSL) - (b.ReservedSP + b.ReservedSR + b.ReservedSL)) 
		 from svTrnSrvItem a	 
		 left join ' + @DbName + '..spMstItems b on 
			a.PartNo = b.PartNo 
			and b.CompanyCode = ''' + @CompanyMD + '''
			and b.BranchCode = ''' + @BranchMD + '''
		 where a.CompanyCode = ''' + @CompanyCode + '''
		   and a.BranchCode  = ''' + @BranchCode + '''
		   and a.ProductType = ''' + @ProductType + '''
		   and a.ServiceNo   = ' + convert(varchar,@ServiceNo) + ''		   
				
		--print(@QueryTemp)		
		insert into @tblTemp		
		exec (@QueryTemp)		
end

insert into @t1
select a.PartSeq
      ,a.BillType
      ,b.Description BillTypeDesc
      ,a.TypeOfGoods
      ,rtrim(c.LookupValueName) + case lower(g.ParaValue) when 'sparepart' then ' (SPR)' else ' (MTR)' end TypeOfGoodsDesc
      ,a.PartNo
      ,a.DemandQty
      ,a.SupplyQty
      ,a.ReturnQty
      ,a.RetailPrice
      ,(case isnull(a.SupplyQty, 0)
         when 0 then (isnull(a.DemandQty, 0) * isnull(a.RetailPrice, 0))
         else ((isnull(a.SupplyQty, 0) - isnull(a.ReturnQty, 0)) * isnull(a.RetailPrice, 0))
        end) * (100.0 - a.DiscPct) * 0.01
        as RetailPriceTotal
      ,a.SupplySlipNo
      ,rtrim(d.PartName) OperationDesc 
	  ,''
	  ,''
	  ,0
	  ,''
	  ,g.ParaValue
	  ,a.DiscPct
	  ,case when @DealerCode = 'MD' then (i.OnHand - (i.AllocationSP + i.AllocationSR + i.AllocationSL) - (i.ReservedSP + i.ReservedSR + i.ReservedSL)) else e.QtyAvail end QtyAvail
	  ,''
  from svTrnSrvItem a with (nolock,nowait)
  left join svMstBillingType b with (nolock,nowait)
    on b.CompanyCode = a.CompanyCode
   and b.BillType = a.BillType
  left join gnMstLookupDtl c with (nolock,nowait)
    on c.CompanyCode = a.CompanyCode
   and c.CodeID = 'TPGO'
   and c.LookupValue = TypeOfGoods
  left join spMstItemInfo d with (nolock,nowait)
    on d.CompanyCode = a.CompanyCode
   and d.PartNo = a.PartNo
  left join gnMstLookupDtl g with (nolock,nowait)
    on g.CompanyCode = a.CompanyCode
   and g.CodeID = 'GTGO'
   and g.LookupValue = TypeOfGoods
  left join svTrnService s with (nolock,nowait)
    on s.CompanyCode = a.CompanyCode
   and s.BranchCode = a.BranchCode
   and s.ServiceNo = a.ServiceNo
  left join spMstItems i
    on i.CompanyCode = a.CompanyCode 
   and i.BranchCode = a.BranchCode
   and i.ProductType = a.ProductType
   and i.PartNo = a.PartNo
   left join @tblTemp e
    on e.PartNo = a.PartNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ProductType = @ProductType
   and a.ServiceNo   = @ServiceNo

select * into #t1 from (
select 
 a.* 
,P01 = isnull((
	select count(*) from spTrnSORDDtl with(nowait,nolock)
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and PartNo      = a.TaskPartNo
	   and DocNo in (select aa.DocNo
					   from spTrnSORDHdr aa with(nowait,nolock), svTrnService bb with(nowait,nolock)
					  where 1 = 1
						and bb.CompanyCode = aa.CompanyCode
						and bb.BranchCode  = aa.BranchCode
						and bb.JobOrderNo  = aa.UsageDocNo
						and isnull(bb.JobOrderNo, '') <> ''
						and aa.CompanyCode = @CompanyCode
						and aa.BranchCode  = @BranchCode
						and bb.ServiceNo   = @ServiceNo
					 )
	),0)
,P02 = isnull((
	select count(DocNo) from spTrnSOSupply with(nowait,nolock)
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and PartNo = a.TaskPartNo
	   and DocNo  = a.SupplySlipNo
	),0)
,P03 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSPickingHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P04 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSPickingHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	   and bb.Status >= '2'
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P05 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSLmpHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,P06 = isnull((
	select count(bb.PickingSlipNo) from spTrnSOSupply aa with(nowait,nolock)
	  left join spTrnSLmpHdr bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.PickingSlipNo = aa.PickingSlipNo
	   and bb.Status >= '1'
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.PartNo = a.TaskPartNo
	   and aa.DocNo  = a.SupplySlipNo
	   and isnull(aa.PickingSlipNo, '') <> ''
	),0)
,S01 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	),0)
,S02 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '2'
	),0)
,S03 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '3'
	),0)
,S04 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '4'
	),0)
,S05 = isnull((
	select count(bb.PONo) from svTrnPoSubCon aa with(nowait,nolock)
	  left join svTrnPoSubConTask bb with(nowait,nolock) on 1 = 1
	   and bb.CompanyCode   = aa.CompanyCode
	   and bb.BranchCode    = aa.BranchCode
	   and bb.ProductType   = aa.ProductType
	   and bb.PONo          = aa.PONo
	 where 1 = 1
	   and aa.CompanyCode = @CompanyCode
	   and aa.BranchCode  = @BranchCode
	   and aa.ProductType = @ProductType
	   and aa.JobOrderNo  = @JobOrderNo
	   and aa.JobOrderNo <> ''
	   and aa.BasicModel  = a.BasicModel
	   and aa.JobType     = a.JobType
	   and bb.OperationNo = a.TaskPartNo
	   and aa.POStatus >= '5'
	),0)
from @t1 a
)#t1

update #t1
   set Status = (case P01 when 0 then 0 else 1 end)
			  + (case P02 when 0 then 0 else 1 end)
			  + (case P03 when 0 then 0 else 1 end)
			  + (case P04 when 0 then 0 else 1 end)
			  + (case P05 when 0 then 0 else 1 end)
			  + (case P06 when 0 then 0 else 1 end)
 where TypeOfGoods <> 'L'

update #t1
   set Status = (case S01 when 0 then 0 else 1 end)
			  + (case S02 when 0 then 0 else 1 end)
			  + (case S03 when 0 then 0 else 1 end)
			  + (case S04 when 0 then 0 else 1 end)
			  + (case S05 when 0 then 0 else 1 end)
 where TypeOfGoods = 'L' and IsSubCon = '1'

select
 row_number() over (order by TaskPartSeq) SeqNo
,TaskPartSeq
,BillType
,BillTypeDesc
,TypeOfGoods
,TypeOfGoodsDesc
,case isnull(TypeOfGoods, '') when 'L' then 'L' else '0' end ItemType
,TaskPartNo
,OprHourDemandQty
,SupplyQty
,ReturnQty
,OprRetailPrice
,OprRetailPriceTotal
,isnull(SupplySlipNo, '')SupplySlipNo
,TaskPartDesc
,Status
,StatusDesc = 
 case IsSubCon
	when 0 then
		 case TypeOfGoods 
			when 'L' then
				case Status
					when '0' then '0 - Open Task'
					when '1' then '1 - Work In Progress'
					when '2' then '2 - Finish Task'
				end
			else
				case Status
					when '1' then '1 - Entry Stock'
					when '2' then '2 - Alokasi Stock'
					when '3' then '3 - Generate PL'
					when '4' then '4 - Generate Bill'
					when '5' then '5 - Lampiran'
					when '6' then '6 - Print Lampiran'
				end
		 end	
	else
		case Status
			when '1' then '1 - Draft PO'
			when '2' then '2 - Generate PO'
			when '3' then '3 - Draft Receiving'
			when '4' then '4 - Cancel Receiving'
			when '5' then '5 - Receiving PO'
		end
 end
,QtyAvail
,(case when (SupplyQty > 0) then (SupplyQty - ReturnQty) else OprHourDemandQty end) * OprRetailPrice Price
,DiscPct
,OprRetailPriceTotal as PriceNet
,IsSubCon
,TaskStatus
,@ServiceNo ServiceNo
from #t1

drop table #t1

end

GO

if object_id('uspfn_spMstItemsInsertFromMD') is not null
	drop procedure uspfn_spMstItemsInsertFromMD
GO
CREATE procedure [dbo].[uspfn_spMstItemsInsertFromMD]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@PartNo varchar(20),
	@UserID varchar(15)
AS	

BEGIN

--begin tran
--declare @CompanyCode varchar(15)
--declare @BranchCode varchar(15)
--declare @PartNo varchar(20)
--declare @MovingCode varchar(15)
--declare @UserID varchar(15)

--set @CompanyCode = '6159401000'
--set @BranchCode = '6159401001'
--set @PartNo = '990H0-990AX-009'
--set @MovingCode = '0'
--set @UserID = 'ga'
declare @sql NVARCHAR(max)

--==================================================================================================================================
-- Chek PartNo di table spMstItems MD
--==================================================================================================================================
declare @xPartNo varchar(15)		
set @sql = 'select @xPartNo = PartNo from ' 
	+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItems '
	+ ' where CompanyCode = ''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + ''' 
		and BranchCode = ''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
		and PartNo = ''' + @PartNo + ''''
		--and MovingCode = ''0'''
execute sp_executesql @sql, N'@xPartNo varchar(15) OUTPUT', @xPartNo = @xPartNo OUTPUT 

--print @sql;
--select @xPartNo

if (select @xPartNo) is not null begin
	--==================================================================================================================================
	-- INSERT spMstItems
	--==================================================================================================================================
	if not exists (select PartNo from spMstItems  
		where CompanyCode = @CompanyCode 
		and BranchCode = @BranchCode
		and PartNo = @PartNo
		--and MovingCode = 0
		) begin
		
		set @sql = 'select * into #temp from ' 
		+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItems '
		+ ' where CompanyCode = ''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + ''' 
			and BranchCode = ''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
			and PartNo = ''' + @PartNo + ''''
			--and MovingCode = ''0'''
		+ CHAR(13) + CHAR(13) +		
		
		--=== IF spMstItems.TypeOfGoods = 0 THEN spMstItems.PurcDiscPct = null
		'insert into spMstItems(
			CompanyCode,BranchCode,PartNo,MovingCode,DemandAverage,BornDate,ABCClass,LastDemandDate,LastPurchaseDate,LastSalesDate
			,BOMInvAmt,BOMInvQty,BOMInvCostPrice,OnOrder,InTransit,OnHand,AllocationSP,AllocationSR,AllocationSL,BackOrderSP
			,BackOrderSR,BackOrderSL,ReservedSP,ReservedSR,ReservedSL,BorrowQty,BorrowedQty,SalesUnit,OrderUnit,OrderPointQty
			,SafetyStockQty,LeadTime,OrderCycle,SafetyStock,Utility1,Utility2,Utility3,Utility4,TypeOfGoods,Status
			,ProductType,PartCategory,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate,isLocked,LockingBy,LockingDate,PurcDiscPct
		)
		values(
			''' + @CompanyCode + ''',''' + @BranchCode + ''',''' + @PartNo + ''',''0'',0,GetDate(),(select ABCClass from #temp),
			GetDate(),GetDate(),GetDate()
			,0,0,0,0,0,0,0,0,0,0
			,0,0,0,0,0,0,0,(select isnull(SalesUnit,0) from #temp),(select isnull(OrderUnit,0) from #temp),0
			,(select isnull(SafetyStockQty,0) from #temp),(select isnull(LeadTime,0) from #temp),(select isnull(OrderCycle,0) from #temp),(select isnull(SafetyStock,0) from #temp)
			,(select isnull(Utility1,'''') from #temp),(select isnull(Utility2,'''') from #temp),(select isnull(Utility3,'''') from #temp),(select isnull(Utility4,'''') from #temp)
			,(select isnull(TypeOfGoods,'''') from #temp),1
			,(select isnull(ProductType,'''') from #temp),(select isnull(PartCategory,'''') from #temp),''' + @UserID + ''',GetDate(),''' + @UserID + ''',GetDate(),0,'''',GetDate()
			,(select DiscPct from gnMstSupplierProfitCenter where SupplierCode = ''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + ''' and ProfitCenterCode = ''300'')
		)'
		
		+ CHAR(13) + 
		'drop table #temp;'
		
		--print (@sql)
		exec (@sql)
	end	

	--==================================================================================================================================
	-- INSERT spMstItemLoc
	--==================================================================================================================================
	if not exists (select PartNo from spMstItemLoc
		where CompanyCode = @CompanyCode 
		and BranchCode = @BranchCode
		and PartNo = @PartNo
		and WarehouseCode = '00') begin
		
		insert into spMstItemLoc(
			CompanyCode,BranchCode,PartNo,WarehouseCode,LocationCode,LocationSub1,LocationSub2,LocationSub3,LocationSub4,LocationSub5
			,LocationSub6,BOMInvAmount,BOMInvQty,BOMInvCostPrice,OnHand,AllocationSP,AllocationSR,AllocationSL,BackOrderSP,BackOrderSR
			,BackOrderSL,ReservedSP,ReservedSR,ReservedSL,Status,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate,isLocked
			,LockingBy,LockingDate
		)
		values(
			@CompanyCode,@BranchCode,@PartNo,'00',' - ',' - ',' - ',' - ',' - ',' - '
			,' - ',0,0,0,0,0,0,0,0,0
			,0,0,0,0,'1',@UserID,GetDate(),@UserID,GetDate(),0
			,@UserID,GetDate()
		)
	end

	--==================================================================================================================================
	-- INSERT spMstItemPrice
	--==================================================================================================================================
	if not exists (select PartNo from spMstItemPrice
		where CompanyCode = @CompanyCode 
		and BranchCode = @BranchCode
		and PartNo = @PartNo) begin

		set @sql = 'select * into #temp from ' 
			+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemPrice '
			+ ' where CompanyCode = ''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + ''' 
				and BranchCode = ''' + dbo.GetBranchMD(@CompanyCode, @BranchCode) + '''
				and PartNo = ''' + @PartNo + ''''
			+ CHAR(13) + CHAR(13) +		
			
		'insert into spMstItemPrice(
			CompanyCode,BranchCode,PartNo,RetailPrice,RetailPriceInclTax,PurchasePrice,CostPrice,OldRetailPrice,OldPurchasePrice,OldCostPrice
			,LastPurchaseUpdate,LastRetailPriceUpdate,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate,isLocked,LockingBy,LockingDate
		)
		values(
			''' + @CompanyCode + ''',''' + @BranchCode + ''',''' + @PartNo + ''',(select isnull(RetailPrice,0) from #temp),(select isnull(RetailPriceInclTax,0) from #temp)
			,(select isnull(PurchasePrice,0) from #temp),(select isnull(CostPrice,0) from #temp),0,0,0
			,GetDate(),GetDate(),''' + @UserID + ''',GetDate(),''' + @UserID + ''',GetDate(),0,'''',GetDate() 
		)'
		
		+ CHAR(13) + CHAR(13) +	
		'drop table #temp';
		
		--print (@sql)
		exec (@sql);
	end

	--==================================================================================================================================
	-- INSERT spMstItemInfo
	--==================================================================================================================================
	if not exists (select PartNo from spMstItemInfo
		where CompanyCode = @CompanyCode 
		and PartNo = @PartNo) begin

		set @sql = 'select * into #temp from ' 
			+ dbo.GetDbMD(@CompanyCode, @BranchCode) + '..spMstItemInfo '
			+ ' where CompanyCode = ''' + dbo.GetCompanyMD(@CompanyCode, @BranchCode) + ''' 
				and PartNo = ''' + @PartNo + ''''
			+ CHAR(13) + CHAR(13) +		
			
		'insert into spMstItemInfo(
			CompanyCode,PartNo,SupplierCode,PartName,IsGenuinePart,DiscPct,SalesUnit,OrderUnit,PurchasePrice,UOMCode
			,Status,ProductType,PartCategory,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate,isLocked,LockingBy,LockingDate
		)
		values(
			''' + @CompanyCode + ''',''' + @PartNo + ''',(select isnull(SupplierCode,'''') from #temp),(select isnull(PartName,'''') from #temp),(select isnull(IsGenuinePart,0) from #temp),
			(select isnull(DiscPct,0) from #temp),(select isnull(SalesUnit,0) from #temp),(select isnull(OrderUnit,0) from #temp),(select isnull(PurchasePrice,0) from #temp),(select isnull(UOMCode,'''') from #temp)
			,''1'',(select isnull(ProductType,'''') from #temp),(select isnull(PartCategory,'''') from #temp),''' + @UserID + ''',GetDate(),''' + @UserID + ''',GetDate(),0,''' + @UserID + ''',GetDate() 
		)'
		
		+ CHAR(13) + CHAR(13) +	
		'drop table #temp';
		
		--print (@sql)
		exec (@sql);
	end
end

--rollback tran

END
GO
if object_id('uspfn_SvTrnServiceSelectBookingData') is not null
	drop procedure uspfn_SvTrnServiceSelectBookingData
GO
CREATE procedure [dbo].[uspfn_SvTrnServiceSelectBookingData]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType  varchar(15),
	@ShowAll bit,
	@JobOrderNo varchar(20),
	@PoliceRegNo  varchar(20),
	@CustomerName varchar(200),
	@ServiceBookNo  varchar(20)
AS

declare @Query varchar(max), @Filter varchar(max)=''
declare @Condition varchar(4000);

set @Condition = ' ORDER BY svTrnService.BookingNo DESC';
if(@ShowAll = 0) begin
	set @Condition = ' AND svTrnService.ServiceStatus IN (0,1,2,3,4,5) ORDER BY svTrnService.BookingNo DESC';
	if(@JobOrderNo <> '') begin 
		set @Filter = @Filter + ' And BookingNo like ''%'+@JobOrderNo+'%'' '
	end
	if(@PoliceRegNo <> '') begin 
		set @Filter = @Filter + ' And svTrnService.PoliceRegNo like ''%'+@PoliceRegNo+'%'' '
	end
	if(@ServiceBookNo <> '' ) begin 
		set @Filter = @Filter + ' And ServiceBookNo like ''%'+@ServiceBookNo+'%'' '
	end
	if(@CustomerName <> '') begin 
		set @Filter = @Filter + ' And gnMstCustomer.CustomerName like ''%'+@CustomerName+'%'''
	end
end 

if(@ShowAll = 1) begin
	if(@JobOrderNo <> '') begin 
		set @Filter = @Filter + ' And BookingNo like ''%'+@JobOrderNo+'%'' '
	end
	if(@PoliceRegNo <> '') begin 
		set @Filter = @Filter + ' And svTrnService.PoliceRegNo like ''%'+@PoliceRegNo+'%'' '
	end
	if(@ServiceBookNo <> '' ) begin 
		set @Filter = @Filter + ' And ServiceBookNo like ''%'+@ServiceBookNo+'%'' '
	end
	if(@CustomerName <> '') begin 
		set @Filter = @Filter + ' And gnMstCustomer.CustomerName like ''%'+@CustomerName+'%'''
	end
end 
set @Query = '
SELECT DISTINCT 
    svTrnService.InvoiceNo
    , svTrnService.ServiceNo
    , svTrnService.ServiceType
    , ForemanID
    , EstimationNo
    , EstimationDate
    , BookingNo
    , BookingDate
    , svTrnService.JobOrderNo
    , svTrnService.JobOrderDate
    , svTrnService.PoliceRegNo
    , ServiceBookNo
    , svTrnService.BasicModel
    , TransmissionType
    , svTrnService.ChassisCode
    , svTrnService.ChassisNo
    , svTrnService.EngineCode
    , svTrnService.EngineNo
    , svTrnService.ChassisCode + '' '' + cast(svTrnService.ChassisNo as  varchar) KodeRangka
    , svTrnService.EngineCode + '' '' + cast(svTrnService.EngineNo as varchar) KodeMesin
    , ColorCode
    , (svTrnService.CustomerCode + '' - '' + gnMstCustomer.CustomerName) as Customer
    , (svTrnService.CustomerCodeBill + '' - '' + custBill.CustomerName) as CustomerBill
    , svTrnService.CustomerCode
    , svTrnService.CustomerCodeBill
    , svTrnService.Odometer
    , svTrnService.JobType
    , case when svTrnService.ServiceStatus=''4'' then
            case when ''' + @ProductType + '''=''4W'' then reffService.Description
                else reffService.LockingBy
            end
        else reffService.Description 
    end ServiceStatus
    --, svTrnService.PoliceRegNo
	--, svTrnService.CustomerCode
    , InsurancePayFlag
    , InsuranceOwnRisk
    , InsuranceNo
    , InsuranceJobOrderNo
    --, svTrnService.CustomerCodeBill
    , svTrnService.LaborDiscPct
    , PartDiscPct
    , svTrnService.MaterialDiscPct
    , svTrnService.PPNPct
    , svTrnService.ServiceRequestDesc
    , ConfirmChangingPart
    --, ForemanID
    , svTrnService.MechanicID
    , EstimateFinishDate
    , svTrnService.LaborDPPAmt
    , svTrnService.PartsDPPAmt
    , svTrnService.MaterialDPPAmt
    , TotalDPPAmount
    , TotalPpnAmount
    , TotalPphAmount
    , TotalSrvAmount
    , employee.EmployeeName
    , (custBill.Address1 + '''' + custBill.Address2 + '''' + custBill.Address3 + '''' + custBill.Address4) as AddressBill
    , custBill.NPWPNo
    , custBill.PhoneNo
    , custBill.HPNo
FROM svTrnService WITH(NOLOCK, NOWAIT)
LEFT JOIN gnMstCustomer 
    ON gnMstCustomer.CompanyCode = svTrnService.CompanyCode 
    AND gnMstCustomer.CustomerCode = svTrnService.CustomerCode
LEFT JOIN gnMstCustomer custBill 
    ON custBill.CompanyCode = svTrnService.CompanyCode
    AND custBill.CustomerCode = svTrnService.CustomerCodeBill
LEFT JOIN gnMstEmployee employee
    ON employee.CompanyCode = svTrnService.CompanyCode
    AND employee.BranchCode = svTrnService.BranchCode
	AND employee.EmployeeID = svTrnService.ForemanID
LEFT JOIN svTrnSrvItem srvItem 
    ON srvItem.CompanyCode = svTrnService.CompanyCode
    AND srvItem.BranchCode = svTrnService.BranchCode
    AND srvItem.ProductType = svTrnService.ProductType
    AND srvItem.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svTrnSrvTask srvTask
    ON srvTask.CompanyCode = svTrnService.CompanyCode
    AND srvTask.BranchCode = svTrnService.BranchCode
    AND srvTask.ProductType = svTrnService.ProductType
    AND srvTask.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svMstRefferenceService reffService
    ON reffService.CompanyCode = svTrnService.CompanyCode
    AND reffService.ProductType = svTrnService.ProductType    
    AND reffService.RefferenceCode = svTrnService.ServiceStatus
    AND reffService.RefferenceType = ''SERVSTAS''
LEFT JOIN svTrnInvoice invoice
	ON invoice.CompanyCode = svTrnService.CompanyCode
	AND invoice.BranchCode = svTrnService.BranchCode
	AND invoice.ProductType = svTrnService.ProductType
	AND invoice.JobOrderNo = svTrnService.JobOrderNo
LEFT JOIN svTrnSrvVOR VOR
    ON VOR.CompanyCode = svTrnService.CompanyCode
	AND VOR.BranchCode = svTrnService.BranchCode
    AND VOR.ServiceNo = svTrnService.ServiceNo
WHERE svTrnService.CompanyCode = ''' + @CompanyCode + '''
    AND svTrnService.BranchCode = ''' + @BranchCode + '''
 AND svTrnService.ServiceType =''1'''
 + @Filter
 + @Condition;
 print @Query
 exec (@Query); 
go

if object_id('sp_MaintainAvgCostItem') is not null
	drop procedure sp_MaintainAvgCostItem
GO
CREATE procedure [dbo].[sp_MaintainAvgCostItem] (  

@CompanyCode varchar(10),
@BranchCode varchar(10),
@ProductType varchar(10),
@PartNo varchar (20),
@Option varchar (2)
)


as

IF @Option = 'A'
BEGIN

SELECT TOP 1500
 Items.PartNo
,Items.ProductType
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
   WHERE CodeID = 'PRCT' AND 
         LookUpValue = Items.PartCategory AND 
         CompanyCode = @CompanyCode) AS CategoryName
,Items.PartCategory
,ItemInfo.PartName
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
,CASE Items.Status WHEN 1 THEN 'Aktif' ELSE 'Tidak' END AS IsActive
,ItemInfo.OrderUnit
,ItemInfo.SupplierCode
,Supplier.SupplierName
, Items.TypeOfGoods TipePart
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'TPGO' AND 
        LookUpValue = Items.TypeOfGoods AND 
        CompanyCode = @CompanyCode) AS TypeOfGoods
,ISNULL(ItemLoc.WarehouseCode,0) WarehouseCode
,ISNULL(ItemLoc.LocationCode,0) LocationCode
,(ISNULL(ItemLoc.OnHand,0) - (ISNULL(ItemLoc.AllocationSP,0) + ISNULL(ItemLoc.AllocationSR,0) + ISNULL(ItemLoc.AllocationSL,0) + ISNULL(ItemLoc.ReservedSP,0) + ISNULL(ItemLoc.ReservedSR,0) + ISNULL(ItemLoc.ReservedSL,0))) AS QtyAvail
,ISNULL(ItemPrice.RetailPrice,0) RetailPrice
,ISNULL(ItemPrice.RetailPriceInclTax,0) RetailPriceInclTax
FROM SpMstItems Items
LEFT JOIN SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                         AND Items.PartNo = ItemInfo.PartNo
LEFT JOIN SpMstItemLoc ItemLoc ON Items.CompanyCode  = ItemLoc.CompanyCode
	AND Items.BranchCode = ItemLoc.BranchCode	
	AND Items.PartNo = ItemLoc.PartNo
LEFT JOIN SpMstItemPrice ItemPrice ON Items.CompanyCode  = ItemPrice.CompanyCode
	AND Items.BranchCode = ItemPrice.BranchCode	
	AND Items.PartNo = ItemPrice.PartNo		 
LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
                         AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE Items.CompanyCode = @CompanyCode
  AND Items.BranchCode  = @BranchCode    
  AND Items.ProductType = @ProductType
  AND ItemLoc.WarehouseCode = '00'
END
ELSE
BEGIN

SELECT TOP 1500
 Items.PartNo
,Items.ProductType
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
   WHERE CodeID = 'PRCT' AND 
         LookUpValue = Items.PartCategory AND 
         CompanyCode = @CompanyCode) AS CategoryName
,Items.PartCategory
,ItemInfo.PartName
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
,CASE Items.Status WHEN 1 THEN 'Aktif' ELSE 'Tidak' END AS IsActive
,ItemInfo.OrderUnit
,Items.Onhand
,ItemInfo.SupplierCode
,Supplier.SupplierName
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'TPGO' AND 
        LookUpValue = Items.TypeOfGoods AND 
        CompanyCode = @CompanyCode) AS TypeOfGoods
,ItemLoc.WarehouseCode
,ItemLoc.LocationCode
,(ItemLoc.OnHand - (ItemLoc.AllocationSP + ItemLoc.AllocationSR + ItemLoc.AllocationSL + ItemLoc.ReservedSP + ItemLoc.ReservedSR + ItemLoc.ReservedSL)) AS QtyAvail
,ItemPrice.RetailPrice
,ItemPrice.CostPrice
,ItemPrice.RetailPriceInclTax
FROM SpMstItems Items with (nolock, nowait)
LEFT JOIN SpMstItemInfo ItemInfo   ON Items.CompanyCode  = ItemInfo.CompanyCode                          
                         AND Items.PartNo = ItemInfo.PartNo
LEFT JOIN SpMstItemLoc ItemLoc ON Items.CompanyCode  = ItemLoc.CompanyCode
	AND Items.BranchCode = ItemLoc.BranchCode	
	AND Items.PartNo = ItemLoc.PartNo
LEFT JOIN SpMstItemPrice ItemPrice ON Items.CompanyCode  = ItemPrice.CompanyCode
	AND Items.BranchCode = ItemPrice.BranchCode	
	AND Items.PartNo = ItemPrice.PartNo		 
LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
                         AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE Items.CompanyCode = @CompanyCode
  AND Items.BranchCode  = @BranchCode    
  AND Items.ProductType = @ProductType
  AND Items.PartNo      = @PartNo
  AND ItemLoc.WarehouseCode = '00'
  END



GO
if object_id('uspfn_SvTrnServiceSelectBookingData') is not null
	drop procedure uspfn_SvTrnServiceSelectBookingData
GO
create procedure [dbo].[uspfn_SvTrnServiceSelectBookingData]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType  varchar(15),
	@ShowAll bit,
	@JobOrderNo varchar(20),
	@PoliceRegNo  varchar(20),
	@CustomerName varchar(200),
	@ServiceBookNo  varchar(20)
AS

declare @Query varchar(max), @Filter varchar(max)=''
declare @Condition varchar(4000);

set @Condition = ' ORDER BY svTrnService.BookingNo DESC';
if(@ShowAll = 0) begin
	set @Condition = ' AND svTrnService.ServiceStatus IN (0,1,2,3,4,5) ORDER BY svTrnService.BookingNo DESC';
	if(@JobOrderNo <> '') begin 
		set @Filter = @Filter + ' And BookingNo like ''%'+@JobOrderNo+'%'' '
	end
	if(@PoliceRegNo <> '') begin 
		set @Filter = @Filter + ' And svTrnService.PoliceRegNo like ''%'+@PoliceRegNo+'%'' '
	end
	if(@ServiceBookNo <> '' ) begin 
		set @Filter = @Filter + ' And ServiceBookNo like ''%'+@ServiceBookNo+'%'' '
	end
	if(@CustomerName <> '') begin 
		set @Filter = @Filter + ' And gnMstCustomer.CustomerName like ''%'+@CustomerName+'%'''
	end
end 

if(@ShowAll = 1) begin
	if(@JobOrderNo <> '') begin 
		set @Filter = @Filter + ' And BookingNo like ''%'+@JobOrderNo+'%'' '
	end
	if(@PoliceRegNo <> '') begin 
		set @Filter = @Filter + ' And svTrnService.PoliceRegNo like ''%'+@PoliceRegNo+'%'' '
	end
	if(@ServiceBookNo <> '' ) begin 
		set @Filter = @Filter + ' And ServiceBookNo like ''%'+@ServiceBookNo+'%'' '
	end
	if(@CustomerName <> '') begin 
		set @Filter = @Filter + ' And gnMstCustomer.CustomerName like ''%'+@CustomerName+'%'''
	end
end 
set @Query = '
SELECT DISTINCT 
    svTrnService.InvoiceNo
    , svTrnService.ServiceNo
    , svTrnService.ServiceType
    , ForemanID
    , EstimationNo
    , EstimationDate
    , BookingNo
    , BookingDate
    , svTrnService.JobOrderNo
    , svTrnService.JobOrderDate
    , svTrnService.PoliceRegNo
    , ServiceBookNo
    , svTrnService.BasicModel
    , TransmissionType
    , svTrnService.ChassisCode
    , svTrnService.ChassisNo
    , svTrnService.EngineCode
    , svTrnService.EngineNo
    , svTrnService.ChassisCode + '' '' + cast(svTrnService.ChassisNo as  varchar) KodeRangka
    , svTrnService.EngineCode + '' '' + cast(svTrnService.EngineNo as varchar) KodeMesin
    , ColorCode
    , (svTrnService.CustomerCode + '' - '' + gnMstCustomer.CustomerName) as Customer
    , (svTrnService.CustomerCodeBill + '' - '' + custBill.CustomerName) as CustomerBill
    , svTrnService.CustomerCode
    , svTrnService.CustomerCodeBill
    , svTrnService.Odometer
    , svTrnService.JobType
    , case when svTrnService.ServiceStatus=''4'' then
            case when ''' + @ProductType + '''=''4W'' then reffService.Description
                else reffService.LockingBy
            end
        else reffService.Description 
    end ServiceStatus
    --, svTrnService.PoliceRegNo
	--, svTrnService.CustomerCode
    , InsurancePayFlag
    , InsuranceOwnRisk
    , InsuranceNo
    , InsuranceJobOrderNo
    --, svTrnService.CustomerCodeBill
    , svTrnService.LaborDiscPct
    , PartDiscPct
    , svTrnService.MaterialDiscPct
    , svTrnService.PPNPct
    , svTrnService.ServiceRequestDesc
    , ConfirmChangingPart
    --, ForemanID
    , svTrnService.MechanicID
    , EstimateFinishDate
    , svTrnService.LaborDPPAmt
    , svTrnService.PartsDPPAmt
    , svTrnService.MaterialDPPAmt
    , TotalDPPAmount
    , TotalPpnAmount
    , TotalPphAmount
    , TotalSrvAmount
    , employee.EmployeeName
    , (custBill.Address1 + '''' + custBill.Address2 + '''' + custBill.Address3 + '''' + custBill.Address4) as AddressBill
    , custBill.NPWPNo
    , custBill.PhoneNo
    , custBill.HPNo
FROM svTrnService WITH(NOLOCK, NOWAIT)
LEFT JOIN gnMstCustomer 
    ON gnMstCustomer.CompanyCode = svTrnService.CompanyCode 
    AND gnMstCustomer.CustomerCode = svTrnService.CustomerCode
LEFT JOIN gnMstCustomer custBill 
    ON custBill.CompanyCode = svTrnService.CompanyCode
    AND custBill.CustomerCode = svTrnService.CustomerCodeBill
LEFT JOIN gnMstEmployee employee
    ON employee.CompanyCode = svTrnService.CompanyCode
    AND employee.BranchCode = svTrnService.BranchCode
	AND employee.EmployeeID = svTrnService.ForemanID
LEFT JOIN svTrnSrvItem srvItem 
    ON srvItem.CompanyCode = svTrnService.CompanyCode
    AND srvItem.BranchCode = svTrnService.BranchCode
    AND srvItem.ProductType = svTrnService.ProductType
    AND srvItem.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svTrnSrvTask srvTask
    ON srvTask.CompanyCode = svTrnService.CompanyCode
    AND srvTask.BranchCode = svTrnService.BranchCode
    AND srvTask.ProductType = svTrnService.ProductType
    AND srvTask.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svMstRefferenceService reffService
    ON reffService.CompanyCode = svTrnService.CompanyCode
    AND reffService.ProductType = svTrnService.ProductType    
    AND reffService.RefferenceCode = svTrnService.ServiceStatus
    AND reffService.RefferenceType = ''SERVSTAS''
LEFT JOIN svTrnInvoice invoice
	ON invoice.CompanyCode = svTrnService.CompanyCode
	AND invoice.BranchCode = svTrnService.BranchCode
	AND invoice.ProductType = svTrnService.ProductType
	AND invoice.JobOrderNo = svTrnService.JobOrderNo
LEFT JOIN svTrnSrvVOR VOR
    ON VOR.CompanyCode = svTrnService.CompanyCode
	AND VOR.BranchCode = svTrnService.BranchCode
    AND VOR.ServiceNo = svTrnService.ServiceNo
WHERE svTrnService.CompanyCode = ''' + @CompanyCode + '''
    AND svTrnService.BranchCode = ''' + @BranchCode + '''
 AND svTrnService.ServiceType =''1'''
 + @Filter
 + @Condition;
 print @Query
 exec (@Query); 
GO
if object_id('uspfn_SvTrnServiceSelectEstimationData') is not null
	drop procedure uspfn_SvTrnServiceSelectEstimationData
GO
CREATE procedure [dbo].[uspfn_SvTrnServiceSelectEstimationData]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType  varchar(15),
	@ShowAll bit,
	@EstimationNo varchar(20),
	@PoliceRegNo  varchar(20),
	@CustomerName varchar(200),
	@ServiceBookNo  varchar(20)
AS
declare @Query varchar(max), @Filter varchar(max)=''
declare @Condition varchar(4000);

set @Condition = ' ORDER BY svTrnService.EstimationNo DESC';
if(@ShowAll = 0) begin
	set @Condition = ' AND svTrnService.ServiceStatus IN (0,1,2,3,4,5) ORDER BY svTrnService.EstimationNo DESC';
	if(@EstimationNo <> '') begin 
		set @Filter = @Filter + ' And EstimationNo like ''%'+@EstimationNo+'%'' '
	end
	if(@PoliceRegNo <> '') begin 
		set @Filter = @Filter + ' And svTrnService.PoliceRegNo like ''%'+@PoliceRegNo+'%'' '
	end
	if(@ServiceBookNo <> '' ) begin 
		set @Filter = @Filter + ' And ServiceBookNo like ''%'+@ServiceBookNo+'%'' '
	end
	if(@CustomerName <> '') begin 
		set @Filter = @Filter + ' And gnMstCustomer.CustomerName like ''%'+@CustomerName+'%'''
	end
end 

if(@ShowAll = 1) begin
	if(@EstimationNo <> '') begin 
		set @Filter = @Filter + ' And EstimationNo like ''%'+@EstimationNo+'%'' '
	end
	if(@PoliceRegNo <> '') begin 
		set @Filter = @Filter + ' And svTrnService.PoliceRegNo like ''%'+@PoliceRegNo+'%'' '
	end
	if(@ServiceBookNo <> '' ) begin 
		set @Filter = @Filter + ' And ServiceBookNo like ''%'+@ServiceBookNo+'%'' '
	end
	if(@CustomerName <> '') begin 
		set @Filter = @Filter + ' And gnMstCustomer.CustomerName like ''%'+@CustomerName+'%'''
	end
end 

set @Query = '
SELECT DISTINCT 
    svTrnService.InvoiceNo
    , svTrnService.ServiceNo
    , svTrnService.ServiceType
    , ForemanID
    , EstimationNo
    , EstimationDate
    , BookingNo
    , BookingDate
    , svTrnService.JobOrderNo
    , svTrnService.JobOrderDate
    , svTrnService.PoliceRegNo
    , ServiceBookNo
    , svTrnService.BasicModel
    , TransmissionType
    , svTrnService.ChassisCode
    , svTrnService.ChassisNo
    , svTrnService.EngineCode
    , svTrnService.EngineNo
    , svTrnService.ChassisCode + '' '' + cast(svTrnService.ChassisNo as  varchar) KodeRangka
    , svTrnService.EngineCode + '' '' + cast(svTrnService.EngineNo as varchar) KodeMesin
    , ColorCode
    , (svTrnService.CustomerCode + '' - '' + gnMstCustomer.CustomerName) as Customer
    , (svTrnService.CustomerCodeBill + '' - '' + custBill.CustomerName) as CustomerBill
    , svTrnService.CustomerCode
    , svTrnService.CustomerCodeBill
    , svTrnService.Odometer
    , svTrnService.JobType
    , case when svTrnService.ServiceStatus=''4'' then
            case when ''' + @ProductType + '''=''4W'' then reffService.Description
                else reffService.LockingBy
            end
        else reffService.Description 
    end ServiceStatus
    --, svTrnService.PoliceRegNo
	--, svTrnService.CustomerCode
    , InsurancePayFlag
    , InsuranceOwnRisk
    , InsuranceNo
    , InsuranceJobOrderNo
    --, svTrnService.CustomerCodeBill
    , svTrnService.LaborDiscPct
    , PartDiscPct
    , svTrnService.MaterialDiscPct
    , svTrnService.PPNPct
    , svTrnService.ServiceRequestDesc
    , ConfirmChangingPart
    --, ForemanID
    , svTrnService.MechanicID
    , EstimateFinishDate
    , svTrnService.LaborDPPAmt
    , svTrnService.PartsDPPAmt
    , svTrnService.MaterialDPPAmt
    , TotalDPPAmount
    , TotalPpnAmount
    , TotalPphAmount
    , TotalSrvAmount
    , employee.EmployeeName
    , (custBill.Address1 + '''' + custBill.Address2 + '''' + custBill.Address3 + '''' + custBill.Address4) as AddressBill
    , custBill.NPWPNo
    , custBill.PhoneNo
    , custBill.HPNo
FROM svTrnService WITH(NOLOCK, NOWAIT)
LEFT JOIN gnMstCustomer 
    ON gnMstCustomer.CompanyCode = svTrnService.CompanyCode 
    AND gnMstCustomer.CustomerCode = svTrnService.CustomerCode
LEFT JOIN gnMstCustomer custBill 
    ON custBill.CompanyCode = svTrnService.CompanyCode
    AND custBill.CustomerCode = svTrnService.CustomerCodeBill
LEFT JOIN gnMstEmployee employee
    ON employee.CompanyCode = svTrnService.CompanyCode
    AND employee.BranchCode = svTrnService.BranchCode
	AND employee.EmployeeID = svTrnService.ForemanID
LEFT JOIN svTrnSrvItem srvItem 
    ON srvItem.CompanyCode = svTrnService.CompanyCode
    AND srvItem.BranchCode = svTrnService.BranchCode
    AND srvItem.ProductType = svTrnService.ProductType
    AND srvItem.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svTrnSrvTask srvTask
    ON srvTask.CompanyCode = svTrnService.CompanyCode
    AND srvTask.BranchCode = svTrnService.BranchCode
    AND srvTask.ProductType = svTrnService.ProductType
    AND srvTask.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svMstRefferenceService reffService
    ON reffService.CompanyCode = svTrnService.CompanyCode
    AND reffService.ProductType = svTrnService.ProductType    
    AND reffService.RefferenceCode = svTrnService.ServiceStatus
    AND reffService.RefferenceType = ''SERVSTAS''
LEFT JOIN svTrnInvoice invoice
	ON invoice.CompanyCode = svTrnService.CompanyCode
	AND invoice.BranchCode = svTrnService.BranchCode
	AND invoice.ProductType = svTrnService.ProductType
	AND invoice.JobOrderNo = svTrnService.JobOrderNo
LEFT JOIN svTrnSrvVOR VOR
    ON VOR.CompanyCode = svTrnService.CompanyCode
	AND VOR.BranchCode = svTrnService.BranchCode
    AND VOR.ServiceNo = svTrnService.ServiceNo
WHERE svTrnService.CompanyCode = ''' + @CompanyCode + '''
    AND svTrnService.BranchCode = ''' + @BranchCode + '''
 AND svTrnService.ServiceType in (''0'',''2'') and svTrnService.EstimationNo <> '''''
 + @filter
 + @Condition;
 
 exec (@Query); 
GO

--declare	@CompanyCode varchar(15)
--declare	@BranchCode  varchar(15)
--declare	@ProductType varchar(15)
--declare	@ServiceNo   int
--declare	@BillType    char(1)
--declare	@InvoiceNo   varchar(15)
--declare	@Remarks     varchar(max)
--declare	@UserID      varchar(15)

--set	@CompanyCode = '6159401000'
--set	@BranchCode  = '6159401001'
--set	@ProductType = '4W'
--set	@ServiceNo   = '53438'
--set	@BillType    = 'C'
--set	@InvoiceNo   = 'INC/15/002778'
--set	@Remarks     = 'REMARK 001'
--set	@UserID      = 'ws-s'

if object_id('uspfn_SvTrnInvoiceCreate') is not null
	drop procedure uspfn_SvTrnInvoiceCreate
GO
CREATE procedure [dbo].[uspfn_SvTrnInvoiceCreate]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@ServiceNo   int,
	@BillType    char(1),
	@InvoiceNo   varchar(15),
	@Remarks     varchar(max),
	@UserID      varchar(15)
as  

declare @errmsg varchar(max)
--raiserror ('test error',16,1);

DECLARE @CompanyMD AS VARCHAR(15)
DECLARE @BranchMD AS VARCHAR(15)
DECLARE @WarehouseMD AS VARCHAR(15)
DECLARE @DbMD AS VARCHAR(15)
declare @md bit

SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

select BillType as BillType
              from svTrnSrvTask
             where CompanyCode = @companycode
               and BranchCode  = @branchcode
               and ProductType = @productType
               and ServiceNo   = @serviceno
            union
            select BillType as BillType
              from svTrnSrvItem b
             where CompanyCode = @companycode
               and BranchCode  = @branchcode
               and ProductType = @productType
               and ServiceNo   = @serviceno
               and  (SupplyQty - ReturnQty) > 0


-- get data from service
select * into #srv from(
  select * from svTrnService
   where 1 = 1
     and CompanyCode = @CompanyCode
     and BranchCode  = @BranchCode
     and ProductType = @ProductType
     and ServiceNo   = @ServiceNo
 )#srv

 select * from #srv
 select * from svTrnSrvItem where serviceno = @serviceno
 select * from svTrnSrvTask where serviceno = @serviceno

-- get data from task
select * into #tsk from(
  select a.* from svTrnSrvTask a, #srv b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.BillType    = @BillType
 )#tsk

 select * from #tsk

-- get data from item
select * into #mec from(
  select a.* from svTrnSrvMechanic a, #tsk b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.OperationNo = b.OperationNo
     and a.OperationNo <> ''
 )#mec

 select * from #mec

-- get data from item
select * into #itm from(
  select a.* from svTrnSrvItem a, #srv b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.BillType    = @BillType
 )#itm

-- create temporary table detail
create table #pre_dtl(
	BillType char(1),
	TaskPartType char(1),
	TaskPartNo varchar(20),
	TaskPartQty numeric(10,2),
	SupplySlipNo varchar(20)
)

insert into #pre_dtl
select BillType, 'L', OperationNo, OperationHour, ''
  from #tsk

insert into #pre_dtl
select BillType, TypeOfGoods, PartNo
	 , sum(SupplyQty - ReturnQty)
	 , SupplySlipNo
  from #itm
 where BillType = @BillType
   and (SupplyQty - ReturnQty) > 0
 group by BillType, TypeOfGoods, PartNo, SupplySlipNo

-- insert to table svTrnInvoice
declare @CustomerCode varchar(20)
if @BillType = 'C'
begin
	set @CustomerCode = (select CustomerCodeBill from #srv)
end
else if @BillType = 'P'
begin
	set @CustomerCode = (select top 1 a.BillTo from svMstPackage a
				 inner join svMstPackageTask b
					on b.CompanyCode = a.CompanyCode
				   and b.PackageCode = a.PackageCode
				 inner join svMstPackageContract c
					on c.CompanyCode = a.CompanyCode
				   and c.PackageCode = a.PackageCode
				 inner join #srv d
					on d.CompanyCode = a.CompanyCode
				   and d.JobType = a.JobType
				   and d.ChassisCode = c.ChassisCode
				   and d.ChassisNo = c.ChassisNo)
end
else if @BillType in ('F', 'W', 'S')
begin
	set @CustomerCode = (select CustomerCode from svMstBillingType
				 where BillType in ('F', 'W', 'S')
				   and CompanyCode = @CompanyCode
				   and BillType = @BillType)
end
else
begin
	set @CustomerCode = (select CustomerCodeBill from #srv)
end

--set @CustomerCode = isnull((
--				select top 1 a.BillTo from svMstPackage a
--				 inner join svMstPackageTask b
--					on b.CompanyCode = a.CompanyCode
--				   and b.PackageCode = a.PackageCode
--				 inner join svMstPackageContract c
--					on c.CompanyCode = a.CompanyCode
--				   and c.PackageCode = a.PackageCode
--				 inner join #srv d
--					on d.CompanyCode = a.CompanyCode
--				   and d.JobType = a.JobType
--				   and d.ChassisCode = c.ChassisCode
--				   and d.ChassisNo = c.ChassisNo
--				), isnull((
--				select CustomerCode from svMstBillingType
--				 where BillType in ('F')
--				   and CompanyCode = @CompanyCode
--				   and BillType = @BillType
--				), isnull((select CustomerCodeBill from #srv), '')))


if ((select count(*) from #tsk) = 0 and (select count(*) from #itm) = 0)
begin
	drop table #srv
	drop table #tsk
	drop table #mec
	drop table #itm
	drop table #pre_dtl
	return
end

if (@CustomerCode = '')
begin
	set @errmsg = N'Customer Code Bill belum di define...'
				+ char(13) + 'Tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

select * into #cus from (
select a.CompanyCode, a.IsPkp, b.CustomerCode, b.LaborDiscPct, b.PartDiscPct, b.MaterialDiscPct, b.TopCode, b.TaxCode
  from gnMstCustomer a, gnMstCustomerProfitCenter b
 where 1 = 1
   and b.CompanyCode  = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.CompanyCode  = @CompanyCode
   and b.BranchCode   = @BranchCode
   and b.CustomerCode = @CustomerCode
   and b.ProfitCenterCode = '200'
)#cus

if (select count(*) from #cus) <> 1
begin
	set @errmsg = N'Customer ProfitCenter belum di define...'
				+ char(13) + 'Tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

declare @IsPKP bit
    set @IsPKP = isnull((
				 select IsPKP from gnMstCustomer
				  where CompanyCode  = @CompanyCode
				    and CustomerCode = @CustomerCode
				  ), 0)

declare @PPnPct decimal
    set @PPnPct = isnull((
				  select a.TaxPct
				    from gnMstTax a, #cus b
				   where 1 = 1
				     and b.TaxCode     = 'PPN'
				     and a.CompanyCode = b.CompanyCode
				     and a.TaxCode     = b.TaxCode
				  ), 0)

declare @PPhPct decimal
    set @PPhPct = isnull((
				  select a.TaxPct
				    from gnMstTax a, #cus b
				   where 1 = 1
				     and b.TaxCode     = 'PPH'
				     and a.CompanyCode = b.CompanyCode
				     and a.TaxCode     = b.TaxCode
				  ), 0)


-- Insert Into svTrnInvoice
-----------------------------------------------------------------------------------------
insert into svTrnInvoice(
  CompanyCode, BranchCode, ProductType
, InvoiceNo, InvoiceDate, InvoiceStatus
, FPJNo, FPJDate, JobOrderNo, JobOrderDate, JobType
, ServiceRequestDesc, ChassisCode, ChassisNo, EngineCode, EngineNo
, PoliceRegNo, BasicModel, CustomerCode, CustomerCodeBill, Odometer
, IsPKP, TOPCode, TOPDays, DueDate, SignedDate
, LaborDiscPct, PartsDiscPct, MaterialDiscPct, PphPct, PpnPct, Remarks
, PrintSeq, PostingFlag, IsLocked, CreatedBy, CreatedDate
) 
select
  @CompanyCode CompanyCode
, @BranchCode BranchCode
, @ProductType ProductType
, @InvoiceNo InvoiceNo
, getdate() InvoiceDate
, case @IsPKP
	when '0' then '1'
	else (case @BillType when 'F' then '0' when 'W' then '0' else '1' end)
  end as InvoiceStatus
, '' FPJNo
, null FPJDate
, (select JobOrderNo from #srv) JobOrderNo
, (select JobOrderDate from #srv) JobOrderDate
, (select JobType from #srv) JobType
, (select ServiceRequestDesc from #srv) ServiceRequestDesc
, (select ChassisCode from #srv) ChassisCode
, (select ChassisNo from #srv) ChassisNo
, (select EngineCode from #srv) EngineCode
, (select EngineNo from #srv) EngineNo
, (select PoliceRegNo from #srv) PoliceRegNo
, (select BasicModel from #srv) BasicModel
, (select CustomerCode from #srv) CustomerCode
, @CustomerCode as CustomerCodeBill
, (select Odometer from #srv) Odometer
, (select IsPKP from #cus) as IsPKP
, (select TopCode from #cus) as TOPCode
, isnull((
	select b.ParaValue
	  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
	 where a.CompanyCode  = @CompanyCode
	   and a.BranchCode   = @BranchCode
	   and a.CustomerCode = @CustomerCode
	   and a.ProfitCenterCode = '200'
	   and b.CompanyCode  = a.CompanyCode
	   and b.CodeID = 'TOPC'
	   and b.LookUpValue = a.TopCode
	), null) as TOPDays
, isnull((
	select dateadd(day, convert(int,b.ParaValue), convert(varchar, getdate(), 112))
	  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
	 where a.CompanyCode  = @CompanyCode
	   and a.BranchCode   = @BranchCode
	   and a.CustomerCode = @CustomerCode
	   and a.ProfitCenterCode = '200'
	   and b.CompanyCode  = a.CompanyCode
	   and b.CodeID = 'TOPC'
	   and b.LookUpValue  = a.TopCode
	), null) as DueDate
, convert(varchar, getdate(), 112) SignedDate
, case @BillType
	when 'F' then (select LaborDiscPct from #cus) 
    when 'W' then (select LaborDiscPct from #cus) 
    else (select LaborDiscPct from #srv) 
  end as LaborDiscPct
, case @BillType
	when 'F' then (select PartDiscPct from #cus) 
    when 'W' then (select PartDiscPct from #cus) 
    else (select PartDiscPct from #srv) 
  end as PartsDiscPct
, case @BillType
	when 'F' then (select MaterialDiscPct from #cus) 
    when 'W' then (select MaterialDiscPct from #cus) 
    else (select MaterialDiscPct from #srv) 
  end as MaterialDiscPct
, @PPnPct as PPhPct
, @PPnPct as PPnPct
, @Remarks as Remarks
, '0' PrintSeq
, '0' PostingFlag
, '0' IsLocked
, @UserID CreatedBy
, getdate() CreatedDate

-- Insert Into svTrnInvTask
-----------------------------------------------------------------------------------------
insert into svTrnInvTask (
  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
, OperationHour, ClaimHour, OperationCost, SubConPrice
, IsSubCon, SharingTask, DiscPct
)
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
, isnull(OperationHour, 0) OperationHour, isnull(ClaimHour, 0) ClaimHour
, isnull(OperationCost, 0) OperationCost, isnull(SubConPrice, 0) SubConPrice
, isnull(IsSubCon, 0) IsSubCon, isnull(SharingTask, 0) SharingTask
, isnull(DiscPct, 0)
from #tsk

-- Insert Into svTrnInvMechanic
-----------------------------------------------------------------------------------------
insert into svTrnInvMechanic (
  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
, MechanicID, ChiefMechanicID, StartService, FinishService
)
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
, MechanicID, ChiefMechanicID, StartService, FinishService
from #mec

-- Insert Into svTrnInvItem
-----------------------------------------------------------------------------------------
Declare @Query varchar(max)

set @Query = 'select * into #itm1 from (
select CompanyCode, BranchCode, ProductType, '''+ @InvoiceNo +''' as InvoiceNo, PartNo
	 , isnull((
		select MovingCode from '+ @DbMD +'..spMstItems
		 where CompanyCode = '''+ @CompanyMD +'''
		   and BranchCode = '''+ @BranchMD +'''
		   and PartNo = #itm.PartNo
		), '''') as MovingCode
	 , isnull((
		select ABCClass from '+ @DbMD +' ..spMstItems
		 where CompanyCode = '''+ @CompanyMD +'''
		   and BranchCode = '''+ @BranchMD +'''
		   and PartNo = #itm.PartNo
		), '''') as ABCClass
	 , sum(SupplyQty - ReturnQty) as SupplyQty
	 , isnull((
		select 
		  case (sum(b.SupplyQty - b.ReturnQty))
			 when 0 then 0
			 else sum(a.CostPrice * (b.SupplyQty - b.ReturnQty)) / sum(b.SupplyQty - b.ReturnQty)
		  end 
	from SpTrnSLmpDtl a
	left join SvTrnSrvItem b on 1 = 1
	 and b.CompanyCode  = a.CompanyCode
	 and b.BranchCode   = a.BranchCode
	 and b.ProductType  = a.ProductType
	 and b.SupplySlipNo = a.DocNo
	 and b.PartNo = #itm.PartNo
	where 1 = 1
	 and a.CompanyCode = '''+ @CompanyCode +'''
	 and a.BranchCode  = '''+ @BranchCode +'''
	 and a.ProductType = '''+ @ProductType +'''
	 and a.PartNo = #itm.PartNo
	 and a.DocNo in (
			select SupplySlipNo
			 from SvTrnSrvItem
			where 1 = 1
			  and CompanyCode = '''+ @CompanyCode +'''
			  and BranchCode  = '''+ @BranchCode +'''
			  and ProductType = '''+ @ProductType +'''
			  and ServiceNo = '''+ Convert(varchar,@ServiceNo) +'''
			  and PartNo = #itm.PartNo
			)
	), 0) as CostPrice
, RetailPrice
, TypeOfGoods
from #itm
group by CompanyCode, BranchCode, ProductType, PartNo, RetailPrice, TypeOfGoods
)#

insert into svTrnInvItem (
  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo
, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice
, TypeOfGoods, DiscPct
)
select a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo
	 , MovingCode = (select top 1 MovingCode from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , ABCClass = (select top 1 ABCClass from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , sum(SupplyQty) as SupplyQty, 0 as ReturnQty
	 , CostPrice = (select top 1 CostPrice from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by CostPrice desc)
	 , RetailPrice = (select top 1 RetailPrice from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by RetailPrice desc)
	 , TypeOfGoods = (select top 1 TypeOfGoods from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , DiscPct = (select top 1 DiscPct from #itm where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
  from #itm1 a
 where a.SupplyQty > 0
 group by a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo'

 exec(@Query)

-- Insert Into svTrnInvItemDtl
-----------------------------------------------------------------------------------------
insert into svTrnInvItemDtl (
  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, SupplySlipNo
, SupplyQty, CostPrice, CreatedBy, CreatedDate
)
select y.* from (
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, PartNo, SupplySlipNo
, sum(SupplyQty - ReturnQty) as SupplyQty, CostPrice
, @UserID as CreatedBy, getdate() as CreatedDate
from #itm
group by CompanyCode, BranchCode, ProductType, PartNo, SupplySlipNo, CostPrice
) y
where y.SupplyQty > 0

-- Re Calculate Invoice

-----------------------------------------------------------------------------------------
exec uspfn_SvTrnInvoiceReCalculate @CompanyCode=@CompanyCode, @BranchCode=@BranchCode, @ProductType=@ProductType, @InvoiceNo=@InvoiceNo, @UserId=@UserId
-- Insert svsdmovement
-----------------------------------------------------------------------------------------

 if(@md = 0)
 begin

 set @Query ='insert into '+ @DbMD +'..svSDMovement
select a.CompanyCode, a.BranchCode, '''+ convert(varchar,@InvoiceNo) +''','''+ convert(varchar,GETDATE()) +''', a.PartNo
, Seq = convert(integer, ROW_NUMBER() OVER (PARTITION BY a.ServiceNo order by a.ServiceNo)) ,
''00'', a.DemandQty, a.DemandQty, a.DiscPct, a.CostPrice, a.RetailPrice, a.TypeOfGoods
, '''+ @CompanyMD +''','''+ @BranchMD +''','''+ @WarehouseMD +''',p.RetailPriceInclTax,p.RetailPrice,p.CostPrice
,''x'','''+ @producttype +''',''300'',''8'',''0'','''+ convert(varchar,GETDATE()) +''','''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
,'''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
from svTrnSrvItem a 
join spmstitemprice p
on p.PartNo = a.PartNo
where p.CompanyCode = '''+ @CompanyCode +'''
and p.branchcode = '''+ @BranchCode +'''
and a.ServiceNo = '''+ convert(varchar,@ServiceNo) +''''

exec (@Query)

end

drop table #srv
drop table #tsk
drop table #mec
drop table #itm
drop table #cus

drop table #pre_dtl
--rollback tran

GO
if object_id('uspfn_SvTrnJobOrderCreate') is not null
	drop procedure uspfn_SvTrnJobOrderCreate
GO
CREATE procedure [dbo].[uspfn_SvTrnJobOrderCreate]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@ServiceNo bigint,
	@UserID varchar(15)
as      

declare @errmsg varchar(max)

begin try
	begin transaction
		declare @docseq int 
        set @docseq = isnull((
			select DocumentSequence from gnMstDocument 
			 where 1 = 1
			   and CompanyCode  = @CompanyCode
			   and BranchCode   = @BranchCode
			   and DocumentType = 'SPK'),0) + 1
        declare @JobOrderNo varchar(15)
		set @JobOrderNo = 'SPK/' + (select right(convert(char(4),getdate(),112),2)) + '/' 
                                 + right((replicate('0',6) + (select convert(varchar, @docseq))),6)
		update svTrnService
		   set ServiceType    = '2'
              ,JobOrderNo     = @JobOrderNo
              ,JobOrderDate   = getdate()
              ,LastUpdateBy   = @UserID
              ,LastUpdateDate = getdate()
		 where 1 = 1
		   and CompanyCode = @CompanyCode
		   and BranchCode  = @BranchCode
		   and ProductType = @ProductType
		   and ServiceNo   = @ServiceNo
		update gnMstDocument 
		   set DocumentSequence = @docseq
              ,LastUpdateBy     = @UserID
              ,LastUpdateDate   = getdate()
		 where 1 = 1
		   and CompanyCode  = @CompanyCode
		   and BranchCode   = @BranchCode
		   and DocumentType = 'SPK'
	commit transaction

	exec uspfn_SvInsertDefaultTaskMovement @CompanyCode, @BranchCode, @ProductType, @ServiceNo, @UserID

end try
begin catch
	rollback transaction
	set @errmsg = N'tidak dapat konversi ke SPK pada ServiceNo = '
				+ convert(varchar,@ServiceNo)
				+ char(13) + error_message()
	raiserror (@errmsg,16,1);
end catch
GO
if object_id('usprpt_OmRpSalesTrn007') is not null
	drop procedure usprpt_OmRpSalesTrn007
GO
CREATE procedure [dbo].[usprpt_OmRpSalesTrn007]
	-- Add the parameters for the stored procedure here
	@CompanyCode VARCHAR(15),
	@BranchCode	 VARCHAR(15),
	@ReqNoA		 VARCHAR(15),
	@ReqNoB		 VARCHAR(15)

AS

DECLARE
	@QRYTmp		AS varchar(max),
	@DBMD		AS varchar(25),
	@CompanyMD  AS varchar(25)


BEGIN

set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


set @QRYTmp =
'SELECT
	row_number () OVER (ORDER BY a.ReqNo) AS No
	, a.ReqNo
	, a.SKPKNo
	, a.FakturPolisiNo
	, ISNULL(c.SuzukiDONo, '''') DONo
	, (SELECT dbo.GetDateIndonesian (a.FakturPolisiDate)) AS ''Tanggal''
	, ISNULL(c.SuzukiDODate, '''') DODate
	, ISNULL(d.CompanyName, '''') CompanyName
	, ISNULL(d.Address1, '''') CoAdd1
	, ISNULL(d.Address2, '''') CoAdd2
	, ISNULL(d.Address3, '''') CoAdd3
	, case d.ProductType 
		when ''2W'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''4W'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		when ''A'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''B'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		else ''Harap dibuatkan Faktur untuk SUZUKI :''
		end as Note
	, ISNULL(c.SuzukiSJNo, '''') SJNo
	, ISNULL(c.SuzukiSJDate, '''') SJDate
	, ISNULL(c.SalesModelCode, '''') Model
	, ISNULL(f.SalesModelDesc, '''') ModelDesc
	, ISNULL(g.RefferenceDesc1, '''') Warna
	, ISNULL(c.SalesModelYear, 0) Tahun
	, a.ChassisNo
	, ISNULL(c.EngineNo, 0) EngineNo
	, ((CASE ISNULL(a.DealerCategory, '''') WHEN ''M'' THEN ''Main Dealer'' WHEN ''S'' THEN ''Sub Dealer'' WHEN ''R'' THEN ''Show Room'' END) + '' / '' + h.CustomerName) AS  Penjual
	, a.SalesmanName
	, a.SKPKName
	, a.SKPKAddress1 Alamat1
	, a.SKPKAddress2 Alamat2
	, a.SKPKAddress3 Alamat3
	, ISNULL(i.LookUpValueName, '''') City
	, a.SKPKTelp1
	, a.SKPKTelp2
	, a.SKPKHP
	, ISNULL(a.SKPKBirthday, '''') SKPKDay
	, a.FakturPolisiName
	, a.FakturPolisiAddress1
	, a.FakturPolisiAddress2
	, a.FakturPolisiAddress3
	, a.FakturPolisiTelp1
	, a.FakturPolisiTelp2
	, a.FakturPolisiHP
	, a.FakturPolisiBirthday
	, (select ISNULL(LookUpValueName, '''') from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID=''FPCT'' and LookUpValue=a.DealerCategory
		) AS DealerCategory
	, ISNULL(b.Remark, '''') Remark
	, ISNULL(UPPER(z.SignName), '''') AS SignName1
	, ISNULL(UPPER(z.TitleSign), '''') AS TitleSign1 
	, a.IDNo
FROM
 omTrSalesReqDetail a
JOIN
 omTrSalesReq b ON b.CompanyCode=a.CompanyCode AND b.BranchCode=a.BranchCode
 AND b.ReqNo=a.ReqNo 
LEFT JOIN
 ' + @DBMD + '..omMstVehicle c ON c.CompanyCode=''' + @CompanyMD + ''' 
 AND c.ChassisCode=a.ChassisCode
 AND c.ChassisNo=a.ChassisNo
LEFT JOIN
 gnMstCoProfile d ON d.CompanyCode=a.CompanyCode AND d.BranchCode=a.BranchCode
LEFT JOIN
 ' + @DBMD + '..omMstModel f ON f.CompanyCode=''' + @CompanyMD + ''' 
 AND f.SalesModelCode=c.SalesModelCode
LEFT JOIN
 ' + @DBMD + '..omMstRefference g ON g.CompanyCode=''' + @CompanyMD + '''
  AND g.RefferenceType=''COLO''
 AND g.RefferenceCode=c.ColourCode
LEFT JOIN
 gnMstCustomer h ON h.CompanyCode=b.CompanyCode AND h.CustomerCode=b.SubDealerCode
LEFT JOIN
 gnMstLookUpDtl i ON i.CompanyCode=a.CompanyCode AND i.CodeID=''CITY'' 
 AND i.LookUpValue=a.SKPKCity
LEFT JOIN gnMstSIgnature z
	ON z.CompanyCode = a.CompanyCode
	AND z.BranchCode = a.BranchCode
	AND z.ProfitCenterCode = ''100''
	AND z.DocumentType = ''RFP''
	AND z.SeqNo = 1
WHERE
 a.CompanyCode	  = ''' + @CompanyCode + '''
 AND a.BranchCode = ''' + @BranchCode + '''
 AND a.ReqNo BETWEEN ''' + @ReqNoA + ''' AND ''' + @ReqNoB + '''
ORDER BY ReqNo'

Exec (@QRYTmp);

END
GO
if object_id('usprpt_PmRpSalesAchievementWeb') is not null
	drop procedure usprpt_PmRpSalesAchievementWeb
GO
CREATE procedure [dbo].[usprpt_PmRpSalesAchievementWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BMEmployeeID		VARCHAR(15),
	@SHEmployeeID		VARCHAR(15),
	@SCEmployeeID		VARCHAR(15),
	@SMEmployeeID		VARCHAR(15),
	@Year				INT
)
AS
BEGIN
-- Get EmployeeID
--=======================================================================
--DECLARE @SalesmanID		VARCHAR(MAX);
DECLARE @SalesmanID TABLE (EmployeeID varchar(15))

if @SHEmployeeID = '' and @SMEmployeeID = ''
begin
insert into @SalesmanID select EmployeeID from HrEmployee where TeamLeader in (
			select EmployeeID from HrEmployee where TeamLeader = @BMEmployeeID)
end
else if (@SHEmployeeID != '' or @SCEmployeeID != '') and @SMEmployeeID = ''
begin
insert into @SalesmanID  select EmployeeID from HrEmployee where TeamLeader  = @SHEmployeeID
end
else
begin
insert into @SalesmanID select EmployeeID from HrEmployee where EmployeeID  = @SMEmployeeID
end
--=======================================================================
DECLARE @TeamLeadeSalesmanID TABLE( EmployeeID varchar(15))

if(@SHEmployeeID = '') and (@SCEmployeeID = '')
insert into @TeamLeadeSalesmanID  select EmployeeID from HrEmployee where TeamLeader  = @BMEmployeeID
else if (@SHEmployeeID != '') and (@SCEmployeeID = '')
insert into @TeamLeadeSalesmanID  select EmployeeID from HrEmployee where EmployeeID  = @SHEmployeeID
else if (@SHEmployeeID != '') and (@SCEmployeeID = '')
insert into @TeamLeadeSalesmanID  select EmployeeID from HrEmployee where EmployeeID  = @SCEmployeeID
--=======================================================================

select * into #TempSM from (
		select 'SM' Intial, CompanyCode, BranchCode, SpvEmployeeID, EmployeeID, isnull(Jan, 0) Jan
		, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May
		, isnull(Jun, 0) Jun, isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep
		, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec from (
			select kdp.CompanyCode, kdp.BranchCode, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth --[Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec]
				, kdp.SpvEmployeeID, kdp.EmployeeID, count(kdp.EmployeeID) InquiryCount
			from PmKDP kdp
			where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year
				--and kdp.BranchCode in (select BranchCode from #ListOfSalesman)						
				and kdp.EmployeeID in (select EmployeeID from @SalesmanID)	
				and kdp.SpvEmployeeID in (select EmployeeID from @TeamLeadeSalesmanID)					
			group by kdp.CompanyCode, kdp.BranchCode, kdp.SpvEmployeeID, kdp.EmployeeID, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)
		) as Header
		pivot(
			sum(InquiryCount)
			for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)
		) pvt
	)#TempSM 

if @SHEmployeeID = '' and @SCEmployeeID != ''
begin
	select * into #TempSC from (
			select 
			'SC' Initial, SM.CompanyCode, SM.BranchCode, a.TeamLeader SpvEmployeeID, SM.SpvEmployeeID EmployeeID
			, sum(SM.Jan) Jan, sum(SM.Feb) Feb, sum(SM.Mar) Mar
			, sum(SM.Apr) Apr, sum(SM.May) May, sum(SM.Jun) Jun
			, sum(SM.Jul) Jul, sum(SM.Aug) Aug, sum(SM.Sep) Sep
			, sum(SM.Oct) Oct, sum(SM.Nov) Nov, sum(SM.Dec) Dec
			from #TempSM SM
			inner join HrEmployee a
			on SM.CompanyCode = a.CompanyCode and SM.SpvEmployeeID = a.EmployeeID
			where SpvEmployeeID in (select EmployeeID from @TeamLeadeSalesmanID)
			group by SM.CompanyCode, SM.BranchCode, a.TeamLeader, SM.SpvEmployeeID
		) #TempSC
		
end
else
begin
	select * into #TempSH from (
		select 
		'SH' Initial, SM.CompanyCode, SM.BranchCode, a.TeamLeader SpvEmployeeID, SM.SpvEmployeeID EmployeeID
		, sum(SM.Jan) Jan, sum(SM.Feb) Feb, sum(SM.Mar) Mar
		, sum(SM.Apr) Apr, sum(SM.May) May, sum(SM.Jun) Jun
		, sum(SM.Jul) Jul, sum(SM.Aug) Aug, sum(SM.Sep) Sep
		, sum(SM.Oct) Oct, sum(SM.Nov) Nov, sum(SM.Dec) Dec
		from #TempSM SM 
		inner join HrEmployee a
			on SM.CompanyCode = a.CompanyCode and SM.SpvEmployeeID = a.EmployeeID
		where SpvEmployeeID in (select EmployeeID from @TeamLeadeSalesmanID)
		group by SM.CompanyCode, SM.BranchCode, a.TeamLeader, SM.SpvEmployeeID
	) #TempSH
	
	select * into #TempBM from (
		select 
		'BM' Initial, SM.CompanyCode, SM.BranchCode, SM.SpvEmployeeID EmployeeID
		, sum(SM.Jan) Jan, sum(SM.Feb) Feb, sum(SM.Mar) Mar
		, sum(SM.Apr) Apr, sum(SM.May) May, sum(SM.Jun) Jun
		, sum(SM.Jul) Jul, sum(SM.Aug) Aug, sum(SM.Sep) Sep
		, sum(SM.Oct) Oct, sum(SM.Nov) Nov, sum(SM.Dec) Dec
		from #TempSH SM 
		group by SM.CompanyCode, SM.BranchCode, SM.SpvEmployeeID
	) #TempBM

end

--=======================================================================================
-- SALES SOURCE OF DATA
--=======================================================================================
	select * into #SSD from (
		select CompanyCode, '' BranchCode, TypeOf1, TypeOf2
			, isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun
			, isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec
		from (
			select kdp.CompanyCode, '' BranchCode, kdp.PerolehanData TypeOf1, '' TypeOf2
				, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth, count(kdp.EmployeeID) InquiryCount
			from PmKDP kdp
			where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year
				--and kdp.BranchCode in (select BranchCode from #ListOfSalesman)						
				and kdp.EmployeeID in (select EmployeeID from @SalesmanID)	
				and kdp.SpvEmployeeID in (select EmployeeID from @TeamLeadeSalesmanID)	
			group by kdp.CompanyCode, kdp.PerolehanData, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header
		pivot
		(
			sum (inquiryCount)
			for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)
		) pvt
	) #SSD

--=======================================================================================
-- SALES BY TYPE
--=======================================================================================
	select * into #ST from (
		select CompanyCode, '' BranchCode, TypeOf1, TypeOf2
			, isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun
			, isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec
		from (
			select kdp.CompanyCode, '' BranchCode, kdp.TipeKendaraan TypeOf1, kdp.Variant TypeOf2
				, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth
				, count(kdp.EmployeeID) InquiryCount
			from PmKDP kdp
			where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year
				--and kdp.BranchCode in (select BranchCode from #ListOfSalesman)							
				and kdp.EmployeeID in (select EmployeeID from @SalesmanID)	
				and kdp.SpvEmployeeID in (select EmployeeID from @TeamLeadeSalesmanID)					
			group by kdp.CompanyCode, kdp.TipeKendaraan, kdp.Variant 
				, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header
		pivot
		(
			sum(InquiryCount)
			for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)
		) pvt
	) #ST

--=======================================================================================
-- PROSPECT STATUS
--=======================================================================================
	select * into #PS from (
		select CompanyCode, '' BranchCode, TypeOf1, TypeOf2
			, isnull(Jan, 0) Jan, isnull(Feb, 0) Feb, isnull(Mar, 0) Mar, isnull(Apr, 0) Apr, isnull(May, 0) May, isnull(Jun, 0) Jun
			, isnull(Jul, 0) Jul, isnull(Aug, 0) Aug, isnull(Sep, 0) Sep, isnull(Oct, 0) Oct, isnull(Nov, 0) Nov, isnull(Dec, 0) Dec 
		from (
			select kdp.CompanyCode, '' BranchCode, kdp.LastProgress TypeOf1, '' TypeOf2
				, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3) InquiryMonth
				, count(kdp.EmployeeID) InquiryCount
			from PmKDP kdp
			where kdp.CompanyCode = @CompanyCode and year(kdp.InquiryDate) = @Year	
				--and kdp.BranchCode in (select BranchCode from #ListOfSalesman)						
				and kdp.EmployeeID in (select EmployeeID from @SalesmanID)	
				and kdp.SpvEmployeeID in (select EmployeeID from @TeamLeadeSalesmanID)							
			group by kdp.CompanyCode, kdp.BranchCode, kdp.LastProgress
				, substring(Convert(varchar, kdp.InquiryDate, 109), 1, 3)) as Header
		pivot
		(
			sum(InquiryCount)
			for InquiryMonth in (Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec)
		) pvt
	) #PS
	
	-- ===================================================================
-- Collect all Service Achievement by salesman
-- ===================================================================
	-- Salesman ----------
	select 
		inc, initial, CompanyCode, BranchCode, EmployeeID, EmployeeName
		, case cast(Jan as varchar) when '0' then '-' else cast(Jan as varchar) end Jan
		, case cast(Feb as varchar) when '0' then '-' else cast(Feb as varchar) end Feb
		, case cast(Mar as varchar) when '0' then '-' else cast(Mar as varchar) end Mar
		, case cast(Apr as varchar) when '0' then '-' else cast(Apr as varchar) end Apr
		, case cast(May as varchar) when '0' then '-' else cast(May as varchar) end May
		, case cast(Jun as varchar) when '0' then '-' else cast(Jun as varchar) end Jun
		, case cast(Jul as varchar) when '0' then '-' else cast(Jul as varchar) end Jul
		, case cast(Aug as varchar) when '0' then '-' else cast(Aug as varchar) end Aug
		, case cast(Sep as varchar) when '0' then '-' else cast(Sep as varchar) end Sep
		, case cast(Oct as varchar) when '0' then '-' else cast(Oct as varchar) end Oct
		, case cast(Nov as varchar) when '0' then '-' else cast(Nov as varchar) end Nov
		, case cast(Dec as varchar) when '0' then '-' else cast(Dec as varchar) end Dec
		, case cast(Sem1 as varchar) when '0' then '-' else cast(Sem1 as varchar) end Sem1
		, case cast(Sem2 as varchar) when '0' then '-' else cast(Sem2 as varchar) end Sem2
		, case cast(Total as varchar) when '0' then '-' else cast(Total as varchar) end Total
	from (
		select  
			'1' inc, 'Salesman' initial, tempSM.CompanyCode, tempSM.BranchCode, tempSM.EmployeeID
			, emp.EmployeeName, tempSM.Jan, tempSM.Feb, tempSM.Mar, tempSM.Apr, tempSM.May, tempSM.Jun
			, tempSM.Jul, tempSM.Aug, tempSM.Sep, tempSM.Oct, tempSM.Nov, tempSM.Dec
			, (tempSM.Jan + tempSM.Feb + tempSM.Mar + tempSM.Apr + tempSM.May + tempSM.Jun) Sem1
			, (tempSM.Jul + tempSM.Aug + tempSM.Sep + tempSM.Oct + tempSM.Nov+ tempSM.Dec) Sem2
			, (tempSM.Jan + tempSM.Feb + tempSM.Mar + tempSM.Apr + tempSM.May + tempSM.Jun
				+ tempSM.Jul + tempSM.Aug + tempSM.Sep + tempSM.Oct + tempSM.Nov
				+ tempSM.Dec) Total
		from 
			#TempSM tempSM
				left join GnMstEmployee emp on tempSM.CompanyCode = emp.CompanyCode
					and tempSM.BranchCode = emp.BranchCode and tempSM.EmployeeID = emp.EmployeeID
		
		union
		-- Sales Head -------	
		select  
			'2' inc, 'Sales Head' initial, TempSH.CompanyCode, TempSH.BranchCode, TempSH.EmployeeID, emp.EmployeeName
			, TempSH.Jan, TempSH.Feb, TempSH.Mar, TempSH.Apr, TempSH.May, TempSH.Jun
			, TempSH.Jul, TempSH.Aug, TempSH.Sep, TempSH.Oct, TempSH.Nov, TempSH.Dec
			, (TempSH.Jan + TempSH.Feb + TempSH.Mar + TempSH.Apr + TempSH.May + TempSH.Jun) Sem1
			, (TempSH.Jul + TempSH.Aug + TempSH.Sep + TempSH.Oct + TempSH.Nov+ TempSH.Dec) Sem2
			, (TempSH.Jan + TempSH.Feb + TempSH.Mar + TempSH.Apr + TempSH.May + TempSH.Jun
				+ TempSH.Jul + TempSH.Aug + TempSH.Sep + TempSH.Oct + TempSH.Nov
				+ TempSH.Dec) Total
		from 
			#TempSH TempSH
				left join GnMstEmployee emp on TempSH.CompanyCode = emp.CompanyCode
					and TempSH.BranchCode = emp.BranchCode and TempSH.EmployeeID = emp.EmployeeID
		
		union
		-- Branch Manager -------
		select  
			'3' inc, 'Branch Manager' initial, TempBM.CompanyCode, TempBM.BranchCode, TempBM.EmployeeID, emp.EmployeeName
			, TempBM.Jan, TempBM.Feb, TempBM.Mar, TempBM.Apr, TempBM.May, TempBM.Jun
			, TempBM.Jul, TempBM.Aug, TempBM.Sep, TempBM.Oct, TempBM.Nov, TempBM.Dec
			, (TempBM.Jan + TempBM.Feb + TempBM.Mar + TempBM.Apr + TempBM.May + TempBM.Jun) Sem1
			, (TempBM.Jul + TempBM.Aug + TempBM.Sep + TempBM.Oct + TempBM.Nov+ TempBM.Dec) Sem2
			, (TempBM.Jan + TempBM.Feb + TempBM.Mar + TempBM.Apr + TempBM.May + TempBM.Jun
				+ TempBM.Jul + TempBM.Aug + TempBM.Sep + TempBM.Oct + TempBM.Nov
				+ TempBM.Dec) Total
		from 
			#TempBM TempBM
				left join GnMstEmployee emp on TempBM.CompanyCode = emp.CompanyCode
					and TempBM.BranchCode = emp.BranchCode and TempBM.EmployeeID = emp.EmployeeID
	) SASalesman order by SASalesman.inc--, SASalesman.EmployeeName

-- ===================================================================
-- Collect all Service Achievement by Types
-- ===================================================================
	select 
		inc, initial, CompanyCode, BranchCode, TypeOf1, TypeOf2
		, case cast(Jan as varchar) when '0' then '-' else cast(Jan as varchar) end Jan
		, case cast(Feb as varchar) when '0' then '-' else cast(Feb as varchar) end Feb
		, case cast(Mar as varchar) when '0' then '-' else cast(Mar as varchar) end Mar
		, case cast(Apr as varchar) when '0' then '-' else cast(Apr as varchar) end Apr
		, case cast(May as varchar) when '0' then '-' else cast(May as varchar) end May
		, case cast(Jun as varchar) when '0' then '-' else cast(Jun as varchar) end Jun
		, case cast(Jul as varchar) when '0' then '-' else cast(Jul as varchar) end Jul
		, case cast(Aug as varchar) when '0' then '-' else cast(Aug as varchar) end Aug
		, case cast(Sep as varchar) when '0' then '-' else cast(Sep as varchar) end Sep
		, case cast(Oct as varchar) when '0' then '-' else cast(Oct as varchar) end Oct
		, case cast(Nov as varchar) when '0' then '-' else cast(Nov as varchar) end Nov
		, case cast(Dec as varchar) when '0' then '-' else cast(Dec as varchar) end Dec
		, case cast(Sem1 as varchar) when '0' then '-' else cast(Sem1 as varchar) end Sem1
		, case cast(Sem2 as varchar) when '0' then '-' else cast(Sem2 as varchar) end Sem2
		, case cast(Total as varchar) when '0' then '-' else cast(Total as varchar) end Total
	from (
		-- Sales Source of Data ----
		select  
			 '1' inc, 'Sales Source of Data' initial, tempSSD.CompanyCode, '' BranchCode, lkpDtl.LookUpValue TypeOf1, tempSSD.TypeOf2
			, isnull(tempSSD.Jan, 0) Jan, isnull(tempSSD.Feb, 0) Feb, isnull(tempSSD.Mar, 0) Mar
			, isnull(tempSSD.Apr, 0) Apr, isnull(tempSSD.May, 0) May, isnull(tempSSD.Jun, 0) Jun
			, isnull(tempSSD.Jul, 0) Jul, isnull(tempSSD.Aug, 0) Aug, isnull(tempSSD.Sep, 0) Sep
			, isnull(tempSSD.Oct, 0) Oct, isnull(tempSSD.Nov, 0) Nov, isnull(tempSSD.Dec, 0) Dec
			, isnull((tempSSD.Jan + tempSSD.Feb + tempSSD.Mar + tempSSD.Apr + tempSSD.May + tempSSD.Jun), 0) Sem1
			, isnull((tempSSD.Jul + tempSSD.Aug + tempSSD.Sep + tempSSD.Oct + tempSSD.Nov+ tempSSD.Dec), 0) Sem2
			, isnull((tempSSD.Jan + tempSSD.Feb + tempSSD.Mar + tempSSD.Apr + tempSSD.May + tempSSD.Jun
				+ tempSSD.Jul + tempSSD.Aug + tempSSD.Sep + tempSSD.Oct + tempSSD.Nov
				+ tempSSD.Dec), 0) Total
		from 
			GnMstLookUpDtl lkpDtl
				left join #SSD tempSSD on lkpDtl.CompanyCode = tempSSD.CompanyCode
				and lkpDtl.LookUpValue = tempSSD.TypeOf1 
		where 
			lkpDtl.CodeID = 'PSRC' and lkpDtl.CompanyCode = @CompanyCode

		union
		-- Sales by Type ----
		select  
			'2' inc, 'Sales by Type' initial, tempST.CompanyCode, tempST.BranchCode, GTS.GroupCode, GTS.typeCode-- tempST.TypeOf1, tempST.TypeOf2
			, isnull(tempST.Jan, 0) Jan, isnull(tempST.Feb, 0) Feb, isnull(tempST.Mar, 0) Mar
			, isnull(tempST.Apr, 0) Apr, isnull(tempST.May, 0) May, isnull(tempST.Jun, 0) Jun
			, isnull(tempST.Jul, 0) Jul, isnull(tempST.Aug, 0) Aug, isnull(tempST.Sep, 0) Sep
			, isnull(tempST.Oct, 0) Oct, isnull(tempST.Nov, 0) Nov, isnull(tempST.Dec, 0) Dec
			, isnull((tempST.Jan + tempST.Feb + tempST.Mar + tempST.Apr + tempST.May + tempST.Jun), 0) Sem1
			, isnull((tempST.Jul + tempST.Aug + tempST.Sep + tempST.Oct + tempST.Nov+ tempST.Dec), 0) Sem2
			, isnull((tempST.Jan + tempST.Feb + tempST.Mar + tempST.Apr + tempST.May + tempST.Jun
				+ tempST.Jul + tempST.Aug + tempST.Sep + tempST.Oct + tempST.Nov
				+ tempST.Dec), 0) Total
		from 
			(select Distinct CompanyCode, GroupCode, TypeCode  
			from pmGroupTypeSeq 
			group by CompanyCode, GroupCode ,typeCode) GTS
				left join #ST tempST on GTS.CompanyCode = tempST.CompanyCode and GTS.GroupCode = tempST.TypeOf1 and GTS.TypeCode = tempST.TypeOf2
					

		union
		-- Prospect Status ----
		select 
			'3' inc, 'Prospect Status' initial, tempPS.CompanyCode, tempPS.BranchCode, lkpDtl.LookUpValueName TypeOf1, tempPS.TypeOf2
			, isnull(tempPS.Jan, 0) Jan, isnull(tempPS.Feb, 0) Feb, isnull(tempPS.Mar, 0) Mar
			, isnull(tempPS.Apr, 0) Apr, isnull(tempPS.May, 0) May, isnull(tempPS.Jun, 0) Jun
			, isnull(tempPS.Jul, 0) Jul, isnull(tempPS.Aug, 0) Aug, isnull(tempPS.Sep, 0) Sep
			, isnull(tempPS.Oct, 0) Oct, isnull(tempPS.Nov, 0) Nov, isnull(tempPS.Dec, 0) Dec
			, isnull((tempPS.Jan + tempPS.Feb + tempPS.Mar + tempPS.Apr + tempPS.May + tempPS.Jun), 0) Sem1
			, isnull((tempPS.Jul + tempPS.Aug + tempPS.Sep + tempPS.Oct + tempPS.Nov+ tempPS.Dec), 0) Sem2
			, isnull((tempPS.Jan + tempPS.Feb + tempPS.Mar + tempPS.Apr + tempPS.May + tempPS.Jun
				+ tempPS.Jul + tempPS.Aug + tempPS.Sep + tempPS.Oct + tempPS.Nov
				+ tempPS.Dec), 0) Total
		from
			GnMstLookUpDtl lkpDtl
				left join #PS tempPS on lkpDtl.CompanyCode = tempPS.CompanyCode
				and lkpDtl.LookUpValue = tempPS.TypeOf1 
		where lkpDtl.CodeID = 'PSTS' and lkpDtl.CompanyCode = @CompanyCode
	) SATypeOf order by SATypeOf.inc, SATypeOf.TypeOf1
	
	
	Select * from #TempSM	
	Select * from #TempBM
	Select * from #TempSH
	select * from #SSD
	select * from #ST
	select * from #PS

	drop table #TempSM
	drop table #TempBM
	drop table #TempSH
	drop table #SSD
	drop table #ST
	drop table #PS
END
GO

if object_id('usprpt_OmRpSalesTrn007BWeb') is not null
	drop PROCEDURE usprpt_OmRpSalesTrn007BWeb
GO
-- usprpt_OmRpSalesTrn007 '6006406','6006406','RFP/11/000003','RFP/11/000003'
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<PERMOHONAN FAKTUR POLISI>
-- =============================================
CREATE procedure [dbo].[usprpt_OmRpSalesTrn007BWeb]
	-- Add the parameters for the stored procedure here
	@CompanyCode VARCHAR(15),
	@BranchCode	 VARCHAR(15),
	@ReqNoA		 VARCHAR(15),
	@ReqNoB		 VARCHAR(15)

AS

DECLARE
	@QRYTmp		AS varchar(max),
	@DBMD		AS varchar(25),
	@CompanyMD  AS varchar(25)

BEGIN

set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

set @QRYTmp = 
'SELECT
	row_number () OVER (ORDER BY a.ReqNo) AS No
	, a.ReqNo
	, a.SKPKNo
	, a.FakturPolisiNo
	, ISNULL(e.RefferenceDONo, '''') DONo
	, case when convert(varchar,isnull((SELECT dbo.GetDateIndonesian (a.FakturPolisiDate)),''19000101''),112) <> ''19000101''  
		then (SELECT dbo.GetDateIndonesian (a.FakturPolisiDate)) 
		else (SELECT dbo.GetDateIndonesian (b.ReqDate)) 
		end AS ''Tanggal''
	, ISNULL(e.RefferenceDoDate, '''') DODate
	, ISNULL(d.CompanyName, '''') CompanyName
	, ISNULL(d.Address1, '''') CoAdd1
	, ISNULL(d.Address2, '''') CoAdd2
	, ISNULL(d.Address3, '''') CoAdd3
	, case d.ProductType 
		when ''2W'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''4W'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		when ''A'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''B'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		else ''Harap dibuatkan Faktur untuk SUZUKI :''
		end as Note
	, ISNULL(e.RefferenceSJNo, '''') SJNo
	, ISNULL(e.RefferenceSJDate, '''') SJDate
	, ISNULL(c.SalesModelCode, '''') Model
	, ISNULL(f.SalesModelDesc, '''') ModelDesc
	, ISNULL(g.RefferenceDesc1, '''') Warna
	, ISNULL(c.SalesModelYear, 0) Tahun
	, a.ChassisNo
	, ISNULL(c.EngineNo, 0) EngineNo
	, ((CASE ISNULL(a.DealerCategory, '''') WHEN ''M'' THEN ''Main Dealer'' WHEN ''S'' THEN ''Sub Dealer'' WHEN ''R'' THEN ''Show Room'' END) + '' / '' + h.CustomerName) AS  Penjual
	, a.SalesmanName
	, a.SKPKName
	, a.SKPKAddress1 Alamat1
	, a.SKPKAddress2 Alamat2
	, a.SKPKAddress3 Alamat3
	, ISNULL(i.LookUpValueName, '''') City
	, a.SKPKTelp1
	, a.SKPKTelp2
	, a.SKPKHP
	, ISNULL(a.SKPKBirthday, '''') SKPKDay
	, a.FakturPolisiName
	, a.FakturPolisiAddress1
	, a.FakturPolisiAddress2
	, a.FakturPolisiAddress3
	, a.FakturPolisiTelp1
	, a.FakturPolisiTelp2
	, a.FakturPolisiHP
	, a.FakturPolisiBirthday
	, (select ISNULL(LookUpValueName, '''') from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID=''FPCT'' and LookUpValue=a.DealerCategory
		) AS DealerCategory
	, ISNULL(b.Remark, '''') Remark
	, ISNULL(UPPER(z.SignName), '''') AS SignName1
	, ISNULL(UPPER(z.TitleSign), '''') AS TitleSign1 
	, a.IDNo
FROM
 omTrSalesReqDetail a
JOIN
 omTrSalesReq b ON b.CompanyCode=a.CompanyCode AND b.BranchCode=a.BranchCode
 AND b.ReqNo=a.ReqNo 
LEFT JOIN
 ' + @DBMD + '..omMstVehicle c ON c.CompanyCode=''' + @CompanyMD + ''' AND c.ChassisCode=a.ChassisCode
 AND c.ChassisNo=a.ChassisNo
LEFT JOIN
 gnMstCoProfile d ON d.CompanyCode=a.CompanyCode AND d.BranchCode=a.BranchCode
LEFT JOIN omTrPurchaseBPUDetail j on a.CompanyCode=j.CompanyCode and c.ChassisCode=j.ChassisCode
	and a.ChassisNo=j.ChassisNo
LEFT JOIN
 omTrPurchaseBPU e ON e.CompanyCode=j.CompanyCode AND e.BranchCode=j.BranchCode
	and e.BPUNo=j.BPUNo
LEFT JOIN
 ' + @DBMD + '..omMstModel f ON f.CompanyCode=''' + @CompanyMD + ''' AND f.SalesModelCode=c.SalesModelCode
LEFT JOIN
 ' + @DBMD + '..omMstRefference g ON g.CompanyCode=''' + @CompanyMD + ''' AND g.RefferenceType=''COLO''
 AND g.RefferenceCode=c.ColourCode
LEFT JOIN
 gnMstCustomer h ON h.CompanyCode=b.CompanyCode AND h.CustomerCode=b.SubDealerCode
LEFT JOIN
 gnMstLookUpDtl i ON i.CompanyCode=a.CompanyCode AND i.CodeID=''CITY'' 
 AND i.LookUpValue=a.SKPKCity
LEFT JOIN gnMstSIgnature z ON z.CompanyCode = a.CompanyCode
	AND z.BranchCode = a.BranchCode
	AND z.ProfitCenterCode = ''100''
	AND z.DocumentType = ''RFP''
	AND z.SeqNo = 1
LEFT JOIN omMstVehicle k ON k.CompanyCode = a.CompanyCode
	AND k.ChassisCode = a.ChassisCode
	AND k.ChassisNo = a.ChassisNo
WHERE
 a.CompanyCode	  =''' + @CompanyCode + '''
 AND a.BranchCode =''' + @BranchCode + '''
 AND a.ReqNo BETWEEN ''' + @ReqNoA + ''' AND ''' + @ReqNoB + '''
 AND k.SalesModelCode NOT IN (select LookUpValueName from gnMstLookUpDtl where CompanyCode=''' + @CompanyCode + ''' and CodeId =''BLOCK'')
ORDER BY ReqNo'

Exec (@QRYTmp);

END
--------------------------------------------------- BATAS ----------------------------------------------------------
GO

if object_id('usprpt_OmRpSalesTrn007Web') is not null
	drop PROCEDURE usprpt_OmRpSalesTrn007Web
GO
create procedure [dbo].[usprpt_OmRpSalesTrn007Web]
	-- Add the parameters for the stored procedure here
	@CompanyCode VARCHAR(15),
	@BranchCode	 VARCHAR(15),
	@ReqNoA		 VARCHAR(15),
	@ReqNoB		 VARCHAR(15)

AS

DECLARE
	@QRYTmp		AS varchar(max),
	@DBMD		AS varchar(25),
	@CompanyMD  AS varchar(25)


BEGIN

set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


set @QRYTmp =
'SELECT
	row_number () OVER (ORDER BY a.ReqNo) AS No
	, a.ReqNo
	, a.SKPKNo
	, a.FakturPolisiNo
	, ISNULL(c.SuzukiDONo, '''') DONo
	, (SELECT dbo.GetDateIndonesian (a.FakturPolisiDate)) AS ''Tanggal''
	, ISNULL(c.SuzukiDODate, '''') DODate
	, ISNULL(d.CompanyName, '''') CompanyName
	, ISNULL(d.Address1, '''') CoAdd1
	, ISNULL(d.Address2, '''') CoAdd2
	, ISNULL(d.Address3, '''') CoAdd3
	, case d.ProductType 
		when ''2W'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''4W'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		when ''A'' then ''Harap dibuatkan Faktur untuk motor SUZUKI :''
		when ''B'' then ''Harap dibuatkan Faktur untuk mobil SUZUKI :''
		else ''Harap dibuatkan Faktur untuk SUZUKI :''
		end as Note
	, ISNULL(c.SuzukiSJNo, '''') SJNo
	, ISNULL(c.SuzukiSJDate, '''') SJDate
	, ISNULL(c.SalesModelCode, '''') Model
	, ISNULL(f.SalesModelDesc, '''') ModelDesc
	, ISNULL(g.RefferenceDesc1, '''') Warna
	, ISNULL(c.SalesModelYear, 0) Tahun
	, a.ChassisNo
	, ISNULL(c.EngineNo, 0) EngineNo
	, ((CASE ISNULL(a.DealerCategory, '''') WHEN ''M'' THEN ''Main Dealer'' WHEN ''S'' THEN ''Sub Dealer'' WHEN ''R'' THEN ''Show Room'' END) + '' / '' + h.CustomerName) AS  Penjual
	, a.SalesmanName
	, a.SKPKName
	, a.SKPKAddress1 Alamat1
	, a.SKPKAddress2 Alamat2
	, a.SKPKAddress3 Alamat3
	, ISNULL(i.LookUpValueName, '''') City
	, a.SKPKTelp1
	, a.SKPKTelp2
	, a.SKPKHP
	, ISNULL(a.SKPKBirthday, '''') SKPKDay
	, a.FakturPolisiName
	, a.FakturPolisiAddress1
	, a.FakturPolisiAddress2
	, a.FakturPolisiAddress3
	, a.FakturPolisiTelp1
	, a.FakturPolisiTelp2
	, a.FakturPolisiHP
	, a.FakturPolisiBirthday
	, (select ISNULL(LookUpValueName, '''') from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID=''FPCT'' and LookUpValue=a.DealerCategory
		) AS DealerCategory
	, ISNULL(b.Remark, '''') Remark
	, ISNULL(UPPER(z.SignName), '''') AS SignName1
	, ISNULL(UPPER(z.TitleSign), '''') AS TitleSign1 
	, a.IDNo
FROM
 omTrSalesReqDetail a
JOIN
 omTrSalesReq b ON b.CompanyCode=a.CompanyCode AND b.BranchCode=a.BranchCode
 AND b.ReqNo=a.ReqNo 
LEFT JOIN
 ' + @DBMD + '..omMstVehicle c ON c.CompanyCode=''' + @CompanyMD + ''' 
 AND c.ChassisCode=a.ChassisCode
 AND c.ChassisNo=a.ChassisNo
LEFT JOIN
 gnMstCoProfile d ON d.CompanyCode=a.CompanyCode AND d.BranchCode=a.BranchCode
LEFT JOIN
 ' + @DBMD + '..omMstModel f ON f.CompanyCode=''' + @CompanyMD + ''' 
 AND f.SalesModelCode=c.SalesModelCode
LEFT JOIN
 ' + @DBMD + '..omMstRefference g ON g.CompanyCode=''' + @CompanyMD + '''
  AND g.RefferenceType=''COLO''
 AND g.RefferenceCode=c.ColourCode
LEFT JOIN
 gnMstCustomer h ON h.CompanyCode=b.CompanyCode AND h.CustomerCode=b.SubDealerCode
LEFT JOIN
 gnMstLookUpDtl i ON i.CompanyCode=a.CompanyCode AND i.CodeID=''CITY'' 
 AND i.LookUpValue=a.SKPKCity
LEFT JOIN gnMstSIgnature z
	ON z.CompanyCode = a.CompanyCode
	AND z.BranchCode = a.BranchCode
	AND z.ProfitCenterCode = ''100''
	AND z.DocumentType = ''RFP''
	AND z.SeqNo = 1
WHERE
 a.CompanyCode	  = ''' + @CompanyCode + '''
 AND a.BranchCode = ''' + @BranchCode + '''
 AND a.ReqNo BETWEEN ''' + @ReqNoA + ''' AND ''' + @ReqNoB + '''
ORDER BY ReqNo'

Exec (@QRYTmp);

END
--------------------------------------------------- BATAS ----------------------------------------------------------


GO
if object_id('uspfn_omSoLkp') is not null
	drop procedure uspfn_omSoLkp
GO
CREATE procedure [dbo].[uspfn_omSoLkp] 
(
	@CompanyCode varchar(25),
	@BranchCode varchar(25)
)
as
 
 -- exec uspfn_omSoLkp '6159401000','6159401001'
 
 declare @DbMD as varchar(15)  
 declare @Sql as varchar(max) 
 declare @ssql as varchar(max) 
 
 set @DbMD = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 


 set @ssql='select * from gnMstCompanyMapping '

set @Sql= 'SELECT a.CompanyCode, a.BranchCode,
                a.SONo, a.SODate, a.SalesType, a.CustomerCode, a.TOPCode, a.Installment, a.FinalPaymentDate,
                a.TOPDays, a.BillTo, a.ShipTo, a.ProspectNo, a.SKPKNo, a.Salesman, a.WareHouseCode, a.isLeasing, 
                a.LeasingCo, a.GroupPriceCode, a.Insurance, a.PaymentType, a.PrePaymentAmt, a.PrePaymentBy, 
                a.CommissionBy, a.CommissionAmt, a.PONo, a.ContractNo, a.Remark, a.Status,
                a.SalesCoordinator, a.SalesHead, a.BranchManager, a.RefferenceNo,
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then '''' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDates, 
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDate, 
                CASE convert(varchar, a.RequestDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RequestDate, 111) end as RequestDate,
                CASE convert(varchar, a.PrePaymentDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.PrePaymentDate, 111) end as PrePaymentDate,
                e.Address1 + '' '' + e.Address2 + '' '' + e.Address3 + '' '' + e.Address4 as Address,
                case when year(a.RefferenceDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC1,
                case when a.SKPKNo <> '''' then convert(bit,1) else convert(bit,0) end isC2,
                case when year(a.PrePaymentDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC3,
                case when year(a.RequestDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC4,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS CustomerName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID) as SalesmanName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.Shipto = b.CustomerCode)  
						AS ShipName,
                (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.LeasingCo = b.CustomerCode)  
						AS LeasingCoName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.PrePaymentby = c.EmployeeID) as PrePaymentName,
				(SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode) AS GroupPriceName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS BillName,
				(SELECT b.lookupvaluename
                        FROM '+@DbMD+'..gnMstLookUpDtl b
                        WHERE a.WareHouseCode = b.LookUpValue
						AND a.WareHouseCode = b.LookUpValue and CodeID =''MPWH'')  
						AS WareHouseName,
                (a.CustomerCode
                    + '' || ''
                    + (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode))  
						AS Customer, 
                (a.Salesman
                    + '' || ''
                    + (SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID))  AS Sales, 
                (a.GroupPriceCode
                    + '' || ''
                    + (SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode))  AS GroupPrice, 
                CASE a.Status when 0 then ''OPEN''
                                when 1 then ''PRINTED''
                                when 2 then ''APPROVED''
                                when 3 then ''DELETED''
                                when 4 then ''REJECTED''
                                when 9 then ''FINISHED'' END as Stat
                , CASE ISNULL(a.SalesType, 0) WHEN 0 THEN ''Wholesales'' ELSE ''Direct'' END AS TypeSales
                ,(select distinct x.TipeKendaraan
                    from pmKDP x
	                    left join gnMstEmployee b on x.CompanyCode=b.CompanyCode and x.BranchCode=b.BranchCode
		                    and x.EmployeeID=b.EmployeeID
	                    left join omTrSalesSO c on c.CompanyCode = x.CompanyCode 
		                    and c.BranchCode = x.BranchCode
		                    and c.ProspectNo = x.InquiryNumber
	                    left join omTrSalesInvoice d on d.CompanyCode = x.CompanyCode
		                    and d.BranchCode = x.BranchCode
		                    and d.SONo = c.SONo
	                    left join omTrSalesReturn e on e.CompanyCode = x.CompanyCode
		                    and e.BranchCode = x.BranchCode
		                    and e.InvoiceNo = d.InvoiceNo
                    where x.InquiryNumber=a.ProspectNo) as VehicleType
                FROM omTrSalesSO a
                INNER JOIN gnMstCustomer e
                ON a.CompanyCode = e.CompanyCode AND a.CustomerCode = e.CustomerCode
                where a.CompanyCode = '''+ @CompanyCode +''' and a.BranchCode = '''+ @BranchCode +'''
				order by a.SONo desc
				'
print @Sql

exec (@Sql)

GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[uspfn_InvoiceFakturPajak]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[uspfn_InvoiceFakturPajak]
GO

create procedure [dbo].[uspfn_InvoiceFakturPajak] @CompanyCode varchar(15), @BranchCode varchar(15), @ProductType varchar(2)
as
begin
SELECT TOP 1500
 Invoice.ProductType, Invoice.InvoiceNo, 
 case Invoice.InvoiceDate when ('19000101') then null else Invoice.InvoiceDate end as InvoiceDate
,Invoice.InvoiceStatus, Invoice.FPJNo
,case Invoice.FPJDate when ('19000101') then null else Invoice.FPJDate end as FPJDate
,Invoice.JobOrderNo
,case Invoice.JobOrderDate when ('19000101') then null else Invoice.JobOrderDate end as JobOrderDate
,Invoice.JobType, Invoice.ChassisCode, Invoice.ChassisNo, Invoice.EngineCode
,Invoice.EngineNo, Invoice.PoliceRegNo, Invoice.BasicModel, Invoice.CustomerCode, Invoice.CustomerCodeBill
,Invoice.Remarks, (Invoice.CustomerCode + ' - ' + Cust.CustomerName) as Customer
,(Invoice.CustomerCodeBill + ' - ' + CustBill.CustomerName) as CustomerBill
, vehicle.ServiceBookNo, Invoice.Odometer
FROM svTrnInvoice Invoice
LEFT JOIN gnMstCustomer Cust
    ON Cust.CompanyCode = Invoice.CompanyCode AND Cust.CustomerCode = Invoice.CustomerCode
LEFT JOIN gnMstCustomer CustBill
    ON CustBill.CompanyCode = Invoice.CompanyCode AND CustBill.CustomerCode = Invoice.CustomerCodeBill
LEFT JOIN svMstcustomerVehicle vehicle 
	ON Invoice.CompanyCode = vehicle.CompanyCode and Invoice.ChassisCode = vehicle.ChassisCode and 
	Invoice.ChassisNo = vehicle.ChassisNo and Invoice.EngineCode = vehicle.EngineCode and 
	Invoice.EngineNo = vehicle.EngineNo and Invoice.BasicModel = vehicle.BasicModel	
WHERE Invoice.CompanyCode = @CompanyCode AND Invoice.BranchCode = @BranchCode 
    AND Invoice.ProductType = @ProductType
    AND convert(varchar, Invoice.InvoiceDate, 112) >= isnull((
            select top 1 convert(varchar, FromDate, 112) from gnMstPeriode
             where 1 = 1
               and CompanyCode = @CompanyCode
               and BranchCode = @BranchCode
               and FiscalYear = (select FiscalYear from gnMstCoProfileService where CompanyCode = @CompanyCode and BranchCode = @BranchCode)
             order by FromDate
            ), '') ORDER BY Invoice.InvoiceNo
end
GO


if object_id('uspfn_spGetNoPartsReturnSS') is not null
	drop procedure uspfn_spGetNoPartsReturnSS
GO
CREATE procedure [dbo].[uspfn_spGetNoPartsReturnSS] @CompanyCode varchar(15), @BranchCode varchar(15),         
@ProductType varchar(5), @TypeOfGoods varchar(2), @DocNo varchar(25)        
as        
select distinct a.CompanyCode, a.BranchCode, a.DocNo, a.WarehouseCode, a.PartNo, a.PartNoOriginal, a.QtyBill as QtyLmp, a.DocNo,    
    c.LmpNo, c.LmpDate        
                from spTrnSLmpDtl a, spTrnSORDHdr b, spTrnSLmpHdr c with(nolock, nowait)        
                where a.CompanyCode = @CompanyCode        
                        and a.BranchCode = @BranchCode        
                        and b.CompanyCode = @CompanyCode        
                  and b.BranchCode = @BranchCode        
      and a.DocNo = b.DocNo         
      and b.Salestype IN ('2', '3')        
                  and c.CompanyCode = @CompanyCode        
                  and c.BranchCode = @BranchCode        
                        and a.LmpNo = c.LmpNo        
                        and a.LmpNo = @DocNo        
      and ProductType = @ProductType        
      and c.TypeOfGoods = @TypeOfGoods        
                        and (a.QtyBill -         
(select ISNULL(SUM (QtyReturn),0) AS MaxQtyRetur FROM spTrnSRturSSdtl        
            LEFT JOIN spTrnSRturSSHdr ON spTrnSRturSSHdr.ReturnNo = spTrnSRturSSdtl.ReturnNo AND        
                spTrnSRturSSHdr.CompanyCode = spTrnSRturSSdtl.CompanyCode AND        
                spTrnSRturSSHdr.BranchCode = spTrnSRturSSdtl.BranchCode        
            WHERE spTrnSRturSSdtl.CompanyCode = @CompanyCode        
                AND spTrnSRturSSdtl.BranchCode = @BranchCode        
                AND spTrnSRturSSHdr.DocNo = a.DocNo        
                AND spTrnSRturSSdtl.PartNo = a.PartNo         
                AND spTrnSRturSSdtl.PartNoOriginal = a.PartNoOriginal         
    AND spTrnSRturSSDtl.DocNo = a.DocNo         
                AND spTrnSRturSSHdr.Status = 2        
    AND ProductType = @ProductType        
    AND TypeOfGoods = @TypeOfGoods)) > 0
    and not exists (select 1 from spTrnSRturSSDtl 
					LEFT JOIN spTrnSRturSSHdr 
					ON spTrnSRturSSHdr.ReturnNo = spTrnSRturSSdtl.ReturnNo 
					AND spTrnSRturSSHdr.CompanyCode = spTrnSRturSSdtl.CompanyCode 
					AND spTrnSRturSSHdr.BranchCode = spTrnSRturSSdtl.BranchCode 
					where spTrnSRturSSDtl.ReturnNo = spTrnSRturSSHdr.ReturnNo and partno = a.PartNo)

GO

if object_id('uspfn_GenerateSSPickingslipNew') is not null
	drop PROCEDURE uspfn_GenerateSSPickingslipNew
GO
CREATE PROCEDURE [dbo].[uspfn_GenerateSSPickingslipNew]
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@TransType		VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@DocDate		DATETIME
AS
BEGIN

--declare	@CompanyCode	VARCHAR(MAX)
--declare	@BranchCode		VARCHAR(MAX)
--declare	@JobOrderNo		VARCHAR(MAX)
--declare	@ProductType	VARCHAR(MAX)
--declare	@CustomerCode	VARCHAR(MAX)
--declare	@TransType		VARCHAR(MAX)
--declare	@UserID			VARCHAR(MAX)
--declare	@DocDate		DATETIME

--set	@CompanyCode	= '6156401000'
--set	@BranchCode		= '6156401001'
--set	@JobOrderNo		= 'SPK/15/001833'
--set	@ProductType	= '4W'
--set	@CustomerCode	= '000003'
--set	@TransType		= '20'
--set	@UserID			= 'TRAININGZZZ'
--set	@DocDate		= '3/12/2015 9:47:01 AM'


--exec uspfn_GenerateSSPickingslipNew '6006400001','6006400101','SPK/14/101589','4W','2105885','20','ga','3/2/2015 4:03:01 PM'
--================================================================================================================================
-- TABLE MASTER
--================================================================================================================================
-- Temporary for Item --
------------------------
SELECT * INTO #Item FROM (
SELECT a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.RetailPrice
	, a.PartNo
	, a.Billtype
	, SUM(ISNULL(a.DemandQty, 0) - (ISNULL(a.SupplyQty, 0))) QtyOrder
FROM svTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN svTrnService b ON b.CompanyCode = a.CompanyCode
	AND b.BranchCode = a.BranchCode
	AND b.ProductType = a.ProductType
	AND b.ServiceNo = a.ServiceNo
	AND b.JobOrderNo = @JobOrderNo
WHERE a.CompanyCode = @CompanyCode 
	AND a.BranchCode = @BranchCode 
	AND a.ProductType = @ProductType 
GROUP BY a.CompanyCode, a.BranchCode, a.ProductType
	, a.ServiceNo, a.PartNo, a.RetailPrice, a.BillType ) #Item 

DECLARE @CompanyMD AS VARCHAR(15)
DECLARE @BranchMD AS VARCHAR(15)
DECLARE @WarehouseMD AS VARCHAR(15)

SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

SELECT * INTO #SrvOrder FROM (
SELECT DISTINCT(a.CompanyCode) 
    , a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
    , (SELECT Paravalue FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'GTGO' AND LookUpValue = a.TypeOfGoods) TipePart
    , (SELECT PartName FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) PartName
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, SUM(a.QtyOrder) QtyOrder
    , 0 QtySupply
    , 0 QtyBO
    , (SUM(a.QtyOrder) * a.RetailPrice) * ((100 - a.PartDiscPct)/100) NetSalesAmt
    , a.PartDiscPct DiscPct
FROM
(
	SELECT
		DISTINCT(a.CompanyCode) 
		, a.BranchCode
		, a.ProductType
		, a.ServiceNo
		, a.PartNo
		, a.RetailPrice
		, c.CostPrice
		, a.TypeOfGoods
		, a.BillType
		, ISNULL(Item.QtyOrder,0) AS QtyOrder
		, a.DiscPct PartDiscPct 
	FROM
		svTrnSrvItem a WITH (NOLOCK, NOWAIT)
		LEFT JOIN svTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode	
			AND a.ProductType = b.ProductType
			AND a.ServiceNo = b.ServiceNo
		LEFT JOIN #Item Item ON Item.CompanyCode = a.CompanyCode 
			AND Item.BranchCode = a.BranchCode 
			AND Item.ProductType = a.ProductType 
			AND Item.ServiceNo = a.ServiceNo 
			AND Item.PartNo = a.PartNo 
			AND Item.RetailPrice = a.RetailPrice 
			AND Item.BillType = a.Billtype
		LEFT JOIN SpMstItemPrice c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode 
			AND a.BranchCode = c.BranchCode 
			AND a.PartNo = c.PartNo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND a.ProductType = @ProductType
		AND Item.QtyOrder > 0
		AND JobOrderNo = @JobOrderNo
		AND (a.SupplySlipNo is null OR a.SupplySlipNo = '')
) a
GROUP BY
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
    , a.PartDiscPct 
) #SrvOrder

select * from #srvorder

--================================================================================================================================
-- INSERT TABLE SpTrnSORDHdr AND SpTrnSORDDtl
--================================================================================================================================
DECLARE @MaxDocNo			INT
DECLARE	@MaxPickingList		INT
DECLARE @TempDocNo			VARCHAR(MAX)
DECLARE @TempPickingList	VARCHAR(MAX)
DECLARE @TypeOfGoods		VARCHAR(MAX)
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

--===============================================================================================================================
-- LOOPING BASED ON THE TYPE OF GOODS
-- ==============================================================================================================================
DECLARE db_cursor CURSOR FOR
SELECT DISTINCT TypeOfGoods FROM #SrvOrder
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND ProductType = @ProductType 

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @TypeOfGoods

WHILE @@FETCH_STATUS = 0
BEGIN

--===============================================================================================================================
-- INSERT HEADER
-- ==============================================================================================================================
SET @MaxDocNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'SSS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempDocNo = ISNULL((SELECT 'SSS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxDocNo, 1), 6)),'SSS/YY/XXXXXX')

INSERT INTO SpTrnSORDHdr
([CompanyCode]
           ,[BranchCode]
           ,[DocNo]
           ,[DocDate]
           ,[UsageDocNo]
           ,[UsageDocDate]
           ,[CustomerCode]
           ,[CustomerCodeBill]
           ,[CustomerCodeShip]
           ,[isBO]
           ,[isSubstitution]
           ,[isIncludePPN]
           ,[TransType]
           ,[SalesType]
           ,[IsPORDD]
           ,[OrderNo]
           ,[OrderDate]
           ,[TOPCode]
           ,[TOPDays]
           ,[PaymentCode]
           ,[PaymentRefNo]
           ,[TotSalesQty]
           ,[TotSalesAmt]
           ,[TotDiscAmt]
           ,[TotDPPAmt]
           ,[TotPPNAmt]
           ,[TotFinalSalesAmt]
           ,[isPKP]
           ,[ExPickingSlipNo]
           ,[ExPickingSlipDate]
           ,[Status]
           ,[PrintSeq]
           ,[TypeOfGoods]
           ,[isDropsign]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastUpdateBy]
           ,[LastUpdateDate]
           ,[isLocked]
           ,[LockingBy]
           ,[LockingDate])

SELECT 
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, @TempDocNo DocNo 
	, @DocDate DocDate
	, @JobOrderNo UsageDocNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) UsageDocDate
	, (SELECT CustomerCode FROM SvTrnService WHERE 1 = 1AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCode
	, (SELECT CustomerCodeBill FROM SvTrnService WHERE 1 = 1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeBill
	, (SELECT CustomerCode FROM SvTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeShip
	, CONVERT(BIT, 0) isBO
	, CONVERT(BIT, 0) isSubstitution
	, CONVERT(BIT, 1) isIncludePPN
	, @TransType TransType
	, '2' SalesType
	, CONVERT(BIT, 0) isPORDD
	, @JobOrderNo OrderNo
	, @DocDate OrderDate
	, ISNULL((SELECT TOPCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') TOPCode
	, ISNULL((SELECT ParaValue FROM GnMstLookUpDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND CodeID = 'TOPC' AND 
		LookupValue IN 
		(SELECT TOPCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)
	  ),0) TOPDays
	, ISNULL((SELECT PaymentCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') PaymentCode
	, '' PaymentReffNo
	, 0 TotSalesQty
	, 0 TotSalesAmt
	, 0 TotDiscAmt
	, 0 TotDPPAmt
	, 0 TotPPNAmt
	, 0 TotFinalSalesAmt
	, CONVERT(BIT, 0) isPKP
	, NULL ExPickingSlipNo
	, NULL ExPickingSlipDate
	, '4' Status
	, 0 PrintSeq
	, @TypeOfGoods TypeOfGoods
	, NULL IsDropSign
	, @UserID CreatedBY
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate


UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'SSS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT DETAIL
-- ==============================================================================================================================
DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

declare @TempAvailStock as table
(
	PartNo varchar(50),
	AvailStock decimal
)

DECLARE @Query AS VARCHAR(MAX)
--SET @Query = 
--'SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
--FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
--WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+''

--INSERT INTO #TempAvailStock

SET @Query = 
'Select * into #TempAvailStock from (SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+')#TempAvailStock

INSERT INTO SpTrnSORDDtl 
(
	[CompanyCode] ,
	[BranchCode] ,
	[DocNo] ,
	[PartNo] ,
	[WarehouseCode] ,
	[PartNoOriginal] ,
	[ReferenceNo] ,
	[ReferenceDate] ,
	[LocationCode] ,
	[QtyOrder] ,
	[QtySupply] ,
	[QtyBO] ,
	[QtyBOSupply] ,
	[QtyBOCancel] ,
	[QtyBill] ,
	[RetailPriceInclTax] ,
	[RetailPrice] ,
	[CostPrice] ,
	[DiscPct] ,
	[SalesAmt] ,
	[DiscAmt] ,
	[NetSalesAmt] ,
	[PPNAmt] ,
	[TotSalesAmt] ,
	[MovingCode] ,
	[ABCClass] ,
	[ProductType] ,
	[PartCategory] ,
	[Status] ,
	[CreatedBy] ,
	[CreatedDate] ,
	[LastUpdateBy] ,
	[LastUpdateDate] ,
	[StockAllocatedBy] ,
	[StockAllocatedDate] ,
	[FirstDemandQty] )
SELECT
	''' + @CompanyCode +''' CompanyCode
	, ''' + @BranchCode +''' BranchCode
	, ''' + @TempDocNo +''' DocNo 
	, a.PartNo
	, ''00'' WarehouseCode
	, a.PartNo PartNoOriginal
	, ''' + @JobOrderNo +''' ReferenceNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = ''' + @CompanyCode +''' AND BranchCode = ''' + @BranchCode +'''
		AND ProductType = ''' + @ProductType +''' AND JobOrderNo = ''' + @JobOrderNo +''' ) ReferenceDate
	, (SELECT distinct LocationCode FROM ' + @DbMD +'..SpMstItemLoc WHERE CompanyCode = ''' + @CompanyMD +''' AND BranchCode = ''' + @BranchMD +''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo ) LocationCode
	, a.QtyOrder
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBO
	, 0 QtyBOSupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBOCancel
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice * 10 /100) RetailPriceIncltax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder * a.RetailPrice
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice
		END AS SalesAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice) * a.DiscPct/100)
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) * a.DiscPct/100)
		END AS DiscAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS NetSalesAmt
	, 0 PPNAmt
	,  CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS TotSalesAmt
	, (SELECT distinct MovingCode FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +''' ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +'''  AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''2'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
	, a.QtyOrder FirstDemandQty
FROM #SrvOrder a
WHERE a.TypeOfGoods = '+@TypeOfGoods +'


select top 10 * from spTrnSORDDtl order by createddate desc
--===============================================================================================================================
-- INSERT SO SUPPLY
-- ==============================================================================================================================

SELECT * INTO #TempSOSupply FROM (
SELECT
	'''+ @CompanyCode +''' CompanyCode
	, '''+ @BranchCode +''' BranchCode
	, '''+ @TempDocNo +''' DocNo 
	, 0 SupSeq
	, a.PartNo 
	, a.PartNo PartNoOriginal
	, '''' PickingSlipNo	
	, '''+ @JobOrderNo +''' ReferenceNo
	, '''+ CONVERT(varchar, @DefaultDate )+''' ReferenceDate
	, ''00'' WarehouseCode
	, (SELECT distinct LocationCode FROM '+ @DbMD+'..SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD+''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo) LocationCode
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, 0 QtyPicked
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice *10 /100) RetailPriceIncltax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, (SELECT distinct MovingCode FROM '+ @DbMD+'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +'''ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''1'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
FROM #SrvOrder a
--inner join spMstItemPrice b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.PartNo = b.PartNo
WHERE a.TypeOfGoods = '+ @TypeOfGoods +'
)#TempSOSupply

INSERT INTO SpTrnSOSupply SELECT 
	CompanyCode,BranchCode,DocNo,SupSeq,PartNo,PartNoOriginal
	, ROW_NUMBER() OVER(ORDER BY PartNo) PTSeq,PickingSlipNo
	, ReferenceNo,ReferenceDate,WarehouseCode,LocationCode
	, QtySupply,QtyPicked,QtyBill,RetailPriceIncltax,RetailPrice,CostPrice
	, DiscPct,MovingCode,ABCClass,ProductType,PartCategory,Status
	, CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate
FROM #TempSOSupply WHERE QtySupply > 0

--===============================================================================================================================
-- UPDATE STATUS DETAIL BASED ON SUPPLY
--===============================================================================================================================

UPDATE SpTrnSORDDtl
SET Status = 4
FROM SpTrnSORDDtl a, #TempSOSupply b
WHERE 1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo
	
--===============================================================================================================================
-- UPDATE HISTORY DEMAND ITEM AND CUSTOMER
--===============================================================================================================================

UPDATE SpHstDemandItem 
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +'''
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandItem a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+ @TempDocNo +'''

UPDATE SpHstDemandCust
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +''' 
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandCust a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +'''
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +'''
	AND a.PartNo = b.PartNo
	AND a.CustomerCode = '''+ @CustomerCode +'''
	AND b.DocNo = '''+ @TempDocNo +'''

INSERT INTO SpHstDemandItem
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +''' Year
	, '''+ Convert(varchar,Month(GetDate())) +''' Month
	, PartNo
	, 1 DemandFreq
	, QtyOrder DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
 AND NOT EXISTS
( SELECT 1 FROM SpHstDemandItem WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND PartNo = a.PartNo
)

INSERT INTO SpHstDemandCust
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +'''  Year
	, '''+ Convert(varchar,Month(GetDate())) +'''  Month
	, '''+ @CustomerCode +''' CustomerCode
	, PartNo
	, 1 DemandFreq
	, (SELECT QtyOrder FROM SpTrnSORDDTl WITH (NOLOCK, NOWAIT) 
		WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
		AND DocNo = a.DocNo AND PartNo = a.PartNo) DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' and a.BranchCode= '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
AND NOT EXISTS
( SELECT PartNo FROM SpHstDemandCust WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +'''  
	AND PartNo = a.PartNo
)

--===============================================================================================================================
-- UPDATE LAST DEMAND DATE MASTER
--===============================================================================================================================

UPDATE '+@DbMD+'..SpMstItems 
SET LastDemandDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+@TempDocNo+'''

--===============================================================================================================================
-- UPDATE STOCK AND MOVEMENT
--===============================================================================================================================

UPDATE '+@DbMD+'..spMstItems
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo

UPDATE '+@DbMD+'..spMstItemloc
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItemLoc a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD +'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.WarehouseCode = '''+@WarehouseMD+'''
	AND a.PartNo = b.PartNo

INSERT INTO SpTrnIMovement
SELECT
	'''+@CompanyCode +''' CompanyCode
	, '''+@BranchCode +''' BranchCode
	, a.DocNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyCode +'''
		AND BranchCode = '''+ @BranchCode +''' AND DocNo = a.DocNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),'''+convert(varchar,getdate())+''') CreatedDate 
	, ''00'' WarehouseCode
	, (SELECT LocationCode FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode =  '''+@CompanyCode +'''
		AND BranchCode = '''+@BranchCode +''' AND DocNo = '''+@TempDocNo +''' AND PartNo = a.PartNo)
	  LocationCode
	, a.PartNo
	, ''OUT'' SignCode
	, ''SA-NPJUAL'' SubSignCode
	, a.QtySupply
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, '''+@UserID +''' CreatedBy
FROM #TempSOSupply a

--===============================================================================================================================
-- UPDATE SUPPLY SLIP TO SPK
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = '''+@CompanyCode +''' AND BranchCode = '''+@BranchCode+'''
		AND ProductType = '''+@ProductType +''' AND JobOrderNo = '''+@JobOrderNo+''')

SELECT * INTO #TempServiceItem FROM (
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1
	AND a.CompanyCode = '''+@CompanyCode+'''
	AND a.BranchCode = '''+@BranchCode+'''
	AND a.ProductType = '''+@ProductType+'''
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+@ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND (a.SupplySlipNo = '''' OR a.SupplySlipNo IS NULL)
) #TempServiceItem 

SELECT * INTO #TempServiceItemIns FROM( 
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1 
	AND a.CompanyCode = '''+ @CompanyCode +''' 
	AND a.BranchCode = '''+ @BranchCode +''' 
	AND a.ProductType = '''+ @ProductType +'''  
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+ @ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo) 
	AND (a.SupplySlipNo != '''' OR a.SupplySlipNo IS NOT NULL)
) #TempServiceItemIns


UPDATE svTrnSrvItem
SET SupplySlipNo = b.DocNo
	, SupplySlipDate = ISNULL((SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
							AND DocNo = b.DocNo),'''+Convert(varchar,@DefaultDate)+''')
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq
	
--===============================================================================================================================
-- INSERT NEW SRV ITEM BASED SUPPLY SLIP
--===============================================================================================================================
INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq + 1
	, 0 DemandQty
	, 0 SupplyQty
	, 0 ReturnQty
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DocNo SupplySlipNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND DocNo = a.DocNo) SupplySlipDate
	, NULL SSReturnNo
	, NULL SSReturnDate
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, a.DiscPct
FROM #TempServiceItemIns a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = '''+ @CompanyCode +'''
	AND a.BranchCode = '''+ @BranchCode +'''
	AND a.ProductType = '''+ @ProductType+'''


--===============================================================================================================================
DROP TABLE #TempServiceItem 
DROP TABLE #TempServiceItemIns
DROP TABLE #TempSOSupply'

EXEC(@query)

--select convert(xml,@query)


--===============================================================================================================================
-- INSERT SVSDMOVEMENT
--===============================================================================================================================

declare @md bit
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

if(@md = 0)
begin

 declare @QueryTemp as varchar(max)  
 
	set @Query ='insert into '+ @DbMD +'..svSDMovement
	select a.CompanyCode, a.BranchCode, a.DocNo, a.CreatedDate, a.PartNo
	, Seq = convert(integer, ROW_NUMBER() OVER (PARTITION BY a.ReferenceNo ORDER BY a.DocNo)) ,
	a.WarehouseCode, a.QtyOrder, a.QtySupply, a.DiscPct
	,isnull(((select RetailPrice from spTrnSORDDtl
			where CompanyCode = ''' + @CompanyCode + '''  and BranchCode = ''' + @BranchCode  + '''
			and DocNo = ''' + @TempDocNo + ''' and PartNo = a.PartNo) / 1.1 * 
			((100 - isnull((select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
				where CompanyCode = ''' + @CompanyCode + ''' and BranchCode = ''' + @BranchCode  + ''' and SupplierCode = dbo.GetBranchMD(''' + @CompanyCode + ''', ''' + @BranchCode  + ''') 
				and ProfitCenterCode = ''300''),0)) * 0.01)),0) CostPrice
	, a.RetailPrice, b.TypeOfGoods
	, '''+ @CompanyMD +''','''+ @BranchMD +''','''+ @WarehouseMD +''', p.RetailPriceInclTax, p.RetailPrice, p.CostPrice
	,''x'','''+ @producttype +''',''300'',''0'',''0'','''+ convert(varchar,GETDATE()) +''','''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	,'''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	from spTrnSORDDtl a 
	join spTrnSORDHdr b on a.CompanyCode = b.CompanyCode
	and a.BranchCode = b.BranchCode 
	and a.DocNo = b.DocNo
	join spmstitemprice p
	on p.PartNo = a.PartNo
	where p.CompanyCode = '''+ @CompanyCode +'''
	and p.branchcode = '''+ @BranchCode +'''
	and a.ReferenceNo = '''+ @JobOrderNo +''''+
	' and a.DocNo = ''' + @TempDocNo + '''';

	exec (@Query)
	--print (@QUERY)

end

--===============================================================================================================================
-- INSERT PICKING HEADER AND DETAIL
--===============================================================================================================================

SET @MaxPickingList = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'PLS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempPickingList = ISNULL((SELECT 'PLS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxPickingList, 1), 6)),'PLS/YY/XXXXXX')

INSERT INTO SpTrnSPickingHdr 
SELECT 
	CompanyCode
	, BranchCode
	, @TempPickingList PickingSlipNo
	, GetDate() PickingSlipDate
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, '' PickedBy
	, CONVERT(BIT, 0) isBORelease
	, isSubstitution
	, isIncludePPN
	, TransType
	, SalesType
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, '' Remark
	, '0' Status
	, '0' PrintSeq
	, TypeOfGoods
	, CreatedBy
	, CreatedDate
	, LastUpdateBy
	, LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = (SELECT distinct DocNo FROM spTrnSORDDtl WHERE CompanyCode = @CompanyCode AND Branchcode = @BranchCode 
					AND DocNo = @TempDocNo AND QtySupply > 0)		

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'PLS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

-- ==============================================================================================================================
-- UPDATE SALES ORDER HEADER 
-- ==============================================================================================================================
UPDATE SpTrnSORDHdr
	SET ExPickingSlipNo = @TempPickingList,
		ExPickingSlipDate = ISNULL((SELECT PickingSlipDate FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode AND PickingSlipNo = @TempPickingList),'')
	
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo

UPDATE SpTrnSOSupply
	SET PickingSlipNo = @TempPickingList
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
-- ==============================================================================================================================
-- INSERT PICKING DETAIL
-- ==============================================================================================================================

INSERT INTO SpTrnSPickingDtl
SELECT 
	a.CompanyCode
	, a.BranchCode
	, @TempPickingList PickingSlipNo
	, '00' WarehouseCode
	, a.PartNo
	, a.PartNoOriginal
	, a.DocNo
	, b.DocDate 
	, a.ReferenceNo
	, a.ReferenceDate
	, a.LocationCode
	, a.QtySupply QtyOrder
	, a.QtySupply
	, a.QtySupply QtyPicked 
	, 0 QtyBill
	, a.RetailPriceInclTax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, a.SalesAmt
	, a.DiscAmt
	, a.NetSalesAmt
	, a.TotSalesAmt
	, a.MovingCode
	, a.ABCClass
	, a.ProductType
	, a.PartCategory
	, '' ExPickingSlipNo
	, @DefaultDate ExPickingSlipDate
	, CONVERT(BIT, 0) isClosed
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSORDDtl a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSORDHdr b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.DocNo = b.DocNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.DocNo = @TempDocNo
	AND a.QtySupply > 0


--================================================================================================================================
-- UPDATE AMOUNT HEADER
--================================================================================================================================
SELECT * INTO #TempHeader FROM (
SELECT 
	header.CompanyCode
	, header.BranchCode
	, header.DocNo
	, header.TotSalesQty
	, header.TotSalesAmt
	, header.TotDiscAmt
	, header.TotDPPAmt
	, floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100)) 
		TotPPNAmt
	, header.TotDPPAmt + floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100))
		TotFinalSalesAmt
FROM (
SELECT 
	CompanyCode
	, BranchCode
	, DocNo
	, SUM(QtyOrder) TotSalesQty
	, SUM(SalesAmt) TotSalesAmt
	, SUM(DiscAmt) TotDiscAmt
	, SUM(NetSalesAmt) TotDPPAmt
FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
GROUP BY CompanyCode
	, BranchCode
	, DocNo
) header ) #TempHeader

UPDATE SpTrnSORDHdr
SET 
	TotSalesQty = b.TotSalesQty
	, TotSalesAmt = b.TotSalesAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalSalesAmt = b.TotFinalSalesAmt
FROM SpTrnSORDHdr a, #TempHeader b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.DocNo = b.DocNo

DROP TABLE #TempHeader

FETCH NEXT FROM db_cursor INTO @TypeOfGoods
END
CLOSE db_cursor
DEALLOCATE db_cursor 

--===============================================================================================================================
-- Update Transdate
--===============================================================================================================================

update gnMstCoProfileSpare
set TransDate=getdate()
	, LastUpdateBy=@UserID
	, LastUpdateDate=getdate()
where CompanyCode = @CompanyCode AND BranchCode = @BranchCode

--===============================================================================================================================
-- DROP TABLE SECTION 
--===============================================================================================================================
DROP TABLE #SrvOrder
DROP TABLE #Item

--rollback tran
END

GO

if object_id('uspfn_SvTrnJobOrderCreate') is not null
	drop procedure uspfn_SvTrnJobOrderCreate
GO
CREATE procedure [dbo].[uspfn_SvTrnJobOrderCreate]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@ServiceNo bigint,
	@UserID varchar(15)
as      

declare @errmsg varchar(max)

begin try
	begin transaction
		declare @docseq int 
        set @docseq = isnull((
			select DocumentSequence from gnMstDocument 
			 where 1 = 1
			   and CompanyCode  = @CompanyCode
			   and BranchCode   = @BranchCode
			   and DocumentType = 'SPK'),0) + 1
        declare @JobOrderNo varchar(15)
		set @JobOrderNo = 'SPK/' + (select right(convert(char(4),getdate(),112),2)) + '/' 
                                 + right((replicate('0',6) + (select convert(varchar, @docseq))),6)
		update svTrnService
		   set ServiceType    = '2'
              ,JobOrderNo     = @JobOrderNo
              ,JobOrderDate   = getdate()
              ,LastUpdateBy   = @UserID
              ,LastUpdateDate = getdate()
		 where 1 = 1
		   and CompanyCode = @CompanyCode
		   and BranchCode  = @BranchCode
		   and ProductType = @ProductType
		   and ServiceNo   = @ServiceNo
		update gnMstDocument 
		   set DocumentSequence = @docseq
              ,LastUpdateBy     = @UserID
              ,LastUpdateDate   = getdate()
		 where 1 = 1
		   and CompanyCode  = @CompanyCode
		   and BranchCode   = @BranchCode
		   and DocumentType = 'SPK'
	commit transaction

end try
begin catch
	rollback transaction
	set @errmsg = N'tidak dapat konversi ke SPK pada ServiceNo = '
				+ convert(varchar,@ServiceNo)
				+ char(13) + error_message()
	raiserror (@errmsg,16,1);
end catch
GO

if object_id('ITSBrowseSO') is not null
	drop VIEW ITSBrowseSO
GO
CREATE view [dbo].[ITSBrowseSO]  
as  
select distinct a.CompanyCode,a.BranchCode,convert(varchar,a.InquiryNumber) InquiryNo,a.InquiryDate,b.EmployeeName,a.NamaProspek,a.TipeKendaraan,  
 a.EmployeeID, a.LastProgress, a.CreatedBy  
from pmKDP a  
 left join gnMstEmployee b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode  
  and a.EmployeeID=b.EmployeeID  
  where a.LastProgress='SPK'
  
GO

if object_id('uspfn_SvTrnInvoiceDraft') is not null
	drop procedure uspfn_SvTrnInvoiceDraft
GO

CREATE procedure [dbo].[uspfn_SvTrnInvoiceDraft]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@JobOrderNo  varchar(15)
as  

declare @errmsg   varchar(max)
declare @BillType varchar(10)

begin try
--set nocount on

-- get data from SvTrnService
select * into #srv from (
  select * from svTrnService with(nowait,nolock) 
    where 1 = 1  
      and CompanyCode  = @CompanyCode  
      and BranchCode   = @BranchCode  
      and EstimationNo = @JobOrderNo  
      and EstimationNo!= ''
  union all
  select * from svTrnService with(nowait,nolock) 
    where 1 = 1  
      and CompanyCode = @CompanyCode  
      and BranchCode  = @BranchCode  
      and BookingNo   = @JobOrderNo  
      and BookingNo  != ''
  union all
  select * from svTrnService with(nowait,nolock) 
    where 1 = 1  
      and CompanyCode = @CompanyCode  
      and BranchCode  = @BranchCode  
      and JobOrderNo  = @JobOrderNo  
      and JobOrderNo != ''
)#srv

select BillType into #t1 from (
select b.BillType from #srv a, svTrnSrvItem b with(nowait,nolock) 
 where 1 = 1
   and b.CompanyCode = a.CompanyCode
   and b.BranchCode  = a.BranchCode
   and b.ProductType = a.ProductType
   and b.ServiceNo   = a.ServiceNo
union
select b.BillType from #srv a, svTrnSrvTask b with(nowait,nolock) 
 where 1 = 1
   and b.CompanyCode = a.CompanyCode
   and b.BranchCode  = a.BranchCode
   and b.ProductType = a.ProductType
   and b.ServiceNo   = a.ServiceNo
)#

set @BillType = (select top 1 a.BillType from svMstBillingType a with(nowait,nolock), #t1 b where b.BillType = a.BillType order by a.LockingBy)
drop table #t1

-- get dicount from Service
declare @ProductType     varchar(20)  set @ProductType     = isnull((select top 1 ProductType     from #srv),0)
declare @ServiceNo       bigint       set @ServiceNo       = isnull((select top 1 ServiceNo       from #srv),0)

-- get ppn & pph dicount from Service
declare @PPnPct decimal
    set @PPnPct = isnull((select a.TaxPct
 						    from gnMstTax a with(nowait,nolock), gnMstCustomerProfitCenter b with(nowait,nolock) , #srv c
						   where c.CompanyCode  = b.CompanyCode
						     and c.BranchCode   = b.BranchCode
						     and c.CustomerCodeBill = b.CustomerCode
						     and b.CompanyCode  = a.CompanyCode
						     and b.TaxCode      = a.TaxCode
						     and b.ProfitCenterCode = '200'
						     and b.TaxCode      = 'PPN'
							),0)

declare @PPhPct decimal
    set @PPhPct = isnull((select a.TaxPct
							from gnMstTax a with(nowait,nolock), gnMstCustomerProfitCenter b with(nowait,nolock) , #srv c
						   where c.CompanyCode  = b.CompanyCode
						     and c.BranchCode   = b.BranchCode
						     and c.CustomerCodeBill = b.CustomerCode
						     and b.CompanyCode  = a.CompanyCode
						     and b.TaxCode      = a.TaxCode
						     and b.ProfitCenterCode = '200'
						     and b.TaxCode      = 'PPH'
							),0)

-- get data gross amount
declare @LaborGrossAmt decimal
    set @LaborGrossAmt = isnull((
						select ceiling(sum(a.OperationHour * a.OperationCost))
						  from svTrnSrvTask a with(nowait,nolock), #srv b
						 where a.CompanyCode = b.CompanyCode
						   and a.BranchCode  = b.BranchCode
						   and a.ProductType = b.ProductType
						   and a.ServiceNo   = b.ServiceNo
						   and a.BillType    = @BillType
						),0)

declare @PartsGrossAmt decimal
    set @PartsGrossAmt = isnull((
						--select ceiling(sum((i.SupplyQty - i.ReturnQty) * i.RetailPrice))--Sebelum Perubahan
						select ceiling(sum(Round((i.SupplyQty - i.ReturnQty) * i.RetailPrice,0)))--Sesudah Perubahan
						  from svTrnSrvItem i with(nowait,nolock), gnMstLookUpDtl g with(nowait,nolock)
						 where g.CompanyCode = i.CompanyCode
					 	   and g.LookUpValue = i.TypeOfGoods
						   and g.CodeID      = 'GTGO'
						   and g.ParaValue   = 'SPAREPART'
						   and i.CompanyCode = @CompanyCode
						   and i.BranchCode  = @BranchCode
						   and i.ProductType = @ProductType
						   and i.ServiceNo   = @ServiceNo
						   and i.BillType    = @BillType
						),0)

declare @MaterialGrossAmt decimal
    set @MaterialGrossAmt = isnull((
						 --select ceiling(sum((i.SupplyQty - i.ReturnQty) * i.RetailPrice))--Sebelum Perubahan
						 select ceiling(sum(Round((i.SupplyQty - i.ReturnQty) * i.RetailPrice,0)))--Sesudah Perubahan
						   from svTrnSrvItem i with(nowait,nolock), gnMstLookUpDtl g with(nowait,nolock)
						  where g.CompanyCode = i.CompanyCode
							and g.LookUpValue = i.TypeOfGoods
							and g.CodeID      = 'GTGO'
							and g.ParaValue  in ('OLI','MATERIAL')
							and i.CompanyCode = @CompanyCode
							and i.BranchCode  = @BranchCode
							and i.ProductType = @ProductType
							and i.ServiceNo   = @ServiceNo
						    and i.BillType    = @BillType
						  ),0)

-- calculate discount
declare @LaborDiscAmt decimal
    set @LaborDiscAmt = isnull((
						 select ceiling(sum(OperationHour * OperationCost * (DiscPct/100.0)))
						   from svTrnSrvTask with(nowait,nolock)
						  where CompanyCode = @CompanyCode
							and BranchCode = @BranchCode
							and ProductType = @ProductType
							and ServiceNo = @ServiceNo
						    and BillType    = @BillType
						  ),0)

declare @PartsDiscAmt decimal
    set @PartsDiscAmt = isnull((
						 --select ceiling(sum((i.SupplyQty - i.ReturnQty) * i.RetailPrice * (i.DiscPct/100.0)))--Sebelum Perubahan
						 select ceiling(sum(Round((i.SupplyQty - i.ReturnQty) * i.RetailPrice * (i.DiscPct/100.0),0)))--Sesudah Perubahan
						   from svTrnSrvItem i with(nowait,nolock), gnMstLookUpDtl g with(nowait,nolock)
						 where g.CompanyCode = i.CompanyCode
					 	   and g.LookUpValue = i.TypeOfGoods
						   and g.CodeID      = 'GTGO'
						   and g.ParaValue   = 'SPAREPART'
						   and i.CompanyCode = @CompanyCode
						   and i.BranchCode  = @BranchCode
						   and i.ProductType = @ProductType
						   and i.ServiceNo   = @ServiceNo
						   and i.BillType    = @BillType
						  ),0)

declare @MaterialDiscAmt decimal
    set @MaterialDiscAmt = isnull((
						 --select ceiling(sum((i.SupplyQty - i.ReturnQty) * i.RetailPrice * (i.DiscPct/100.0)))--Sebelum Perubahan
						 select ceiling(sum(Round((i.SupplyQty - i.ReturnQty) * i.RetailPrice * (i.DiscPct/100.0),0)))--Sesudah Perubahan
						   from svTrnSrvItem i with(nowait,nolock), gnMstLookUpDtl g with(nowait,nolock)
						  where g.CompanyCode = i.CompanyCode
							and g.LookUpValue = i.TypeOfGoods
							and g.CodeID      = 'GTGO'
							and g.ParaValue  in ('OLI','MATERIAL')
							and i.CompanyCode = @CompanyCode
							and i.BranchCode  = @BranchCode
							and i.ProductType = @ProductType
							and i.ServiceNo   = @ServiceNo
						    and i.BillType    = @BillType
						  ),0)

-- calculate DPP (dasar pengenaan pajak)
--declare @LaborDppAmt     decimal	set @LaborDppAmt     = floor(@LaborGrossAmt    - @LaborDiscAmt)--Sebelum Perubahan
--declare @PartsDppAmt     decimal	set @PartsDppAmt     = floor(@PartsGrossAmt    - @PartsDiscAmt)--Sebelum Perubahan
--declare @MaterialDppAmt  decimal	set @MaterialDppAmt  = floor(@MaterialGrossAmt - @MaterialDiscAmt)--Sebelum Perubahan
--declare @TotalDppAmt     decimal	set @TotalDppAmt     = floor(@LaborDppAmt + @PartsDppAmt + @MaterialDppAmt)--Sebelum Perubahan
declare @LaborDppAmt     decimal	set @LaborDppAmt     = Round(@LaborGrossAmt    - @LaborDiscAmt,0)--Sesudah Perubahan
declare @PartsDppAmt     decimal	set @PartsDppAmt     = Round(@PartsGrossAmt    - @PartsDiscAmt,0)--Sesudah Perubahan
declare @MaterialDppAmt  decimal	set @MaterialDppAmt  = Round(@MaterialGrossAmt - @MaterialDiscAmt,0)--Sesudah Perubahan
declare @TotalDppAmt     decimal	set @TotalDppAmt     = Round(@LaborDppAmt + @PartsDppAmt + @MaterialDppAmt,0)--Sesudah Perubahan

-- calculate ppn & service amount
declare @TotalPpnAmt     decimal	set @TotalPpnAmt = floor(@TotalDppAmt * (@PpnPct / 100.0))
declare @TotalPphAmt     decimal	set @TotalPphAmt = floor(@TotalDppAmt * (@PphPct / 100.0))
declare @TotalSrvAmt     decimal	set @TotalSrvAmt = floor(@TotalDppAmt + @TotalPphAmt + @TotalPpnAmt)

    
;with t1 as (
select a.CompanyCode, a.BranchCode, a.ProductType, a.ServiceNo
     , a.EstimationNo, a.EstimationDate, a.BookingNo, a.BookingDate, a.JobOrderNo, a.JobOrderDate, a.ServiceType, a.IsSparepartClaim
     , a.PoliceRegNo, a.ServiceBookNo, a.BasicModel, a.TransmissionType
     , a.ChassisCode, a.ChassisNo, a.EngineCode, a.EngineNo, a.ColorCode
     , rtrim(rtrim(a.ColorCode)
     + case isnull(b.RefferenceDesc2,'') when '' then '' else ' - ' end
     + isnull(b.RefferenceDesc2,'')) as ColorCodeDesc
     , a.Odometer
     , a.CustomerCode, c.CustomerName, c.Address1 CustAddr1
     , c.Address2 CustAddr2, c.Address3 CustAddr3, c.Address4 CustAddr4
     , c.CityCode CityCode, d.LookupValueName CityName
     , a.InsurancePayFlag, a.InsuranceOwnRisk, a.InsuranceNo, a.InsuranceJobOrderNo
     , a.CustomerCodeBill, e.CustomerName CustomerNameBill
     , e.Address1 CustAddr1Bill, e.Address2 CustAddr2Bill
     , e.Address3 CustAddr3Bill, e.Address4 CustAddr4Bill
     , e.CityCode CityCodeBill, f.LookupValueName CityNameBill
     , e.PhoneNo, e.FaxNo, e.HPNo, a.LaborDiscPct, a.PartDiscPct
     , a.ServiceRequestDesc, a.ConfirmChangingPart, a.EstimateFinishDate
     , a.MaterialDiscPct, a.JobType, a.ForemanID, a.MechanicID
     , a.ServiceStatus
	 , @LaborDppAmt LaborDppAmt, @PartsDppAmt PartsDppAmt, @MaterialDppAmt MaterialDppAmt
	 , @TotalDppAmt TotalDppAmt, @TotalPpnAmt TotalPpnAmt
	 , @TotalSrvAmt TotalSrvAmt
	 , a.LaborDppAmt SrvLaborDppAmt, a.PartsDppAmt SrvPartsDppAmt, a.MaterialDppAmt SrvMaterialDppAmt
	 , a.TotalDppAmount SrvTotalDppAmt, a.TotalPpnAmount SrvTotalPpnAmt
	 , a.TotalSrvAmount SrvTotalSrvAmt
  from svTrnService a with (nowait,nolock)
  left join omMstRefference b with (nowait,nolock)
    on b.CompanyCode = a.CompanyCode
   and b.RefferenceType = 'COLO'
   and b.RefferenceCode = a.ColorCode
  left join gnMstCustomer c with (nowait,nolock)
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
  left join gnMstLookupDtl d with (nowait,nolock)
    on d.CompanyCode = a.CompanyCode
   and d.CodeID = 'CITY'
   and d.LookUpValue = c.CityCode
  left join gnMstCustomer e with (nowait,nolock)
    on e.CompanyCode = a.CompanyCode
   and e.CustomerCode = a.CustomerCodeBill
  left join gnMstLookupDtl f with (nowait,nolock)
    on f.CompanyCode = a.CompanyCode
   and f.CodeID = 'CITY'
   and f.LookUpValue = e.CityCode
 where 1 = 1
   and a.CompanyCode = @CompanyCode
   and a.BranchCode  = @BranchCode
   and a.ServiceNo   = (select ServiceNo from #srv)
) 
select a.CompanyCode, a.BranchCode, a.ProductType --, JobOrderNo, 
	 , a.ServiceNo, a.ServiceType
     , a.EstimationNo, a.EstimationDate, a.BookingNo, a.BookingDate, a.JobOrderNo, a.JobOrderDate
     , '' InvoiceNo, z.Remarks 
     -- Data Kendaraan
     , a.PoliceRegNo, a.ServiceBookNo, a.BasicModel, a.TransmissionType
     , a.ChassisCode, a.ChassisNo, a.EngineCode, a.EngineNo
     , a.ColorCode, a.ColorCodeDesc, a.Odometer
     -- Data Contract
     , b.IsContractStatus IsContract
     , b.ContractNo
	 , c.EndPeriod ContractEndPeriod
	 , c.IsActive ContractStatus
	 , case b.IsContractStatus 
		when 1 then (case c.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end)
		else ''
	   end ContractStatusDesc
     -- Data Contract
	 , b.IsClubStatus IsClub
	 , b.ClubCode
	 , b.ClubDateFinish ClubEndPeriod
	 , d.IsActive ClubStatus
	 , case b.IsClubStatus
		when 1 then (case d.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end)
		else ''
	   end ClubStatusDesc
     -- Data Customer
     , a.CustomerCode, a.CustomerName
     , a.CustAddr1, a.CustAddr2, a.CustAddr3, a.CustAddr4
     , a.CityCode, a.CityName
     -- Data Customer Bill
     , a.InsurancePayFlag, a.InsuranceOwnRisk, a.InsuranceNo, a.InsuranceJobOrderNo
     , a.CustomerCodeBill, a.CustomerNameBill
     , a.CustAddr1Bill, a.CustAddr2Bill, a.CustAddr3Bill, a.CustAddr4Bill
     , a.CityCodeBill, a.CityNameBill
     , a.PhoneNo, a.FaxNo, a.HPNo
     , a.LaborDiscPct, a.PartDiscPct, a.PartDiscPct PartsDiscPct, a.MaterialDiscPct
     --, IsPPnBill
     -- Data Pekerjaan
     , a.ServiceRequestDesc
     , a.JobType, e.Description JobTypeDesc
     , a.ConfirmChangingPart, a.EstimateFinishDate
     , a.ForemanID, g.EmployeeName ForemanName
	 , a.MechanicID, h.EmployeeName MechanicName, a.IsSparepartClaim
	 -- Data Total Biaya Perawatan
     , a.LaborDppAmt, a.PartsDppAmt, a.MaterialDppAmt, a.TotalDppAmt
     , a.TotalPpnAmt, a.TotalSrvAmt
     , a.SrvLaborDppAmt, a.SrvPartsDppAmt, a.SrvMaterialDppAmt, a.SrvTotalDppAmt
     , a.SrvTotalPpnAmt, a.SrvTotalSrvAmt

     , a.ServiceStatus
	 , f.Description ServiceStatusDesc
	 , isnull(i.TaxCode,'') TaxCode
	 , isnull(j.TaxPct,0) TaxPct
  from t1 a
  left join svMstCustomerVehicle b with (nowait,nolock)
    on b.CompanyCode = a.CompanyCode
   and b.ChassisCode = a.ChassisCode
   and b.ChassisNo = a.ChassisNo
  left join svMstContract c with (nowait,nolock)
    on c.CompanyCode = a.CompanyCode
   and c.ContractNo = b.ContractNo
   and b.IsContractStatus = 1
  left join svMstClub d with (nowait,nolock)
    on d.CompanyCode = a.CompanyCode
   and d.ClubCode = b.ClubCode
  left join SvMstRefferenceService e with (nowait,nolock)
    on e.CompanyCode = a.CompanyCode
   and e.ProductType = a.ProductType
   and e.RefferenceCode = a.JobType
   and e.RefferenceType = 'JOBSTYPE'
  left join SvMstRefferenceService f with (nowait,nolock)
    on f.CompanyCode = a.CompanyCode
   and f.ProductType = a.ProductType
   and f.RefferenceCode = a.ServiceStatus
   and f.RefferenceType = 'SERVSTAS'
  left join gnMstEmployee g with (nowait,nolock)
    on g.CompanyCode = a.CompanyCode
   and g.BranchCode = a.BranchCode
   and g.EmployeeID = a.ForemanID
  left join gnMstEmployee h with (nowait,nolock)
    on h.CompanyCode = a.CompanyCode
   and h.BranchCode = a.BranchCode
   and h.EmployeeID = a.MechanicID
  left join gnMstCustomerProfitCenter i with (nowait,nolock)
    on i.CompanyCode = a.CompanyCode
   and i.BranchCode = a.BranchCode
   and i.CustomerCode = a.CustomerCode
   and i.ProfitCenterCode = '200'
  left join gnMstTax j with (nowait,nolock)
    on j.CompanyCode = a.CompanyCode
   and j.TaxCode = i.TaxCode
  left join svTrnInvoice z with (nowait, nolock)
   on a.JobOrderNo = z.JobOrderNo AND a.CompanyCode = z.CompanyCode AND a.BranchCode = z.BranchCode

end try
begin catch
    set @errmsg = 'Error Message:' + char(13) + error_message()
    raiserror (@errmsg,16,1);
	drop table #srv
end catch

GO

if object_id('uspfn_OmPostingSalesInvoice') is not null
	drop procedure uspfn_OmPostingSalesInvoice
GO
CREATE procedure [dbo].[uspfn_OmPostingSalesInvoice]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@DocNo       varchar(20),
	@UserID      varchar(20)
AS

BEGIN

--declare @UserID      varchar(20)
--declare @CompanyCode varchar(max)
--declare @BranchCode varchar(max)
--declare @DocNo varchar(max)

--set @CompanyCode = '6558201'
--set @BranchCode = '655820100'
--set @DocNo= 'IVU/13/001280'
--set @UserID      = 'yo'

declare @JournalGL table
(
	CodeTrans      varchar(50),
	SalesModelCode varchar(50),
	AccountNo      varchar(50),
	AmountDb       decimal,
	AmountCr       decimal
)

insert into @JournalGL
select '01 AR', ''
	 , isnull((
		select cls.ReceivableAccNo
		  from omTrSalesInvoice ivu, GnMstCustomerProfitCenter cus, GnMstCustomerClass cls
		 where 1 = 1
		   and cus.CompanyCode   = ivu.CompanyCode
		   and cus.BranchCode    = ivu.BranchCode
		   and cus.CustomerCode  = ivu.CustomerCode
		   and cus.ProfitCenterCode = '100'
		   and cls.CompanyCode   = ivu.CompanyCode
		   and cls.BranchCode    = ivu.BranchCode
		   and cls.CustomerClass = cus.CustomerClass
		   and cus.CompanyCode   = @CompanyCode
		   and cus.BranchCode    = @BranchCode
		   and ivu.InvoiceNo     = @DocNo
		), '') 
	 , isnull((
		select sum(isnull(Quantity, 0) * (AfterDiscDPP + AfterDiscPPn + AfterDiscPPnBm))
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(mdl.Quantity * (oth.AfterDiscDPP + oth.AfterDiscPPn))
		  from omTrSalesInvoiceOthers oth left join omTrSalesInvoiceModel mdl
			on oth.BranchCode = mdl.BranchCode
			and oth.InvoiceNo = mdl.InvoiceNo
			and oth.BPKNo = mdl.BPKNo
			and oth.SalesModelCode = mdl.SalesModelCode
		 where 1 = 1
		   and oth.CompanyCode = @CompanyCode 
		   and oth.BranchCode  = @BranchCode
		   and oth.InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(DPP + PPN)
		  from omTrSalesInvoiceAccs
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + (select isnull(sum(isnull(Quantity,0)*isnull(Total,0)),0) from omTrSalesInvoiceAccsSeq where CompanyCode=@CompanyCode
		   and BranchCode=@BranchCode and InvoiceNo=@DocNo) 
	 , 0

insert into @JournalGL
select '02 DISCOUNT UNIT', a.SalesModelCode
	 , isnull((
		select acc.DiscountAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '')
	 , sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) as Discount
	 , 0
  from omTrSalesInvoiceModel a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) > 0

insert into @JournalGL
select '03 DISCOUNT AKSESORIS', a.SalesModelCode
	 , isnull((
		select acc.DiscountAccNoAks
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '')
	 , sum(isnull(a.DiscExcludePPn, 0)) as Discount
	 , 0
  from omTrSalesInvoiceOthers a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull(a.DiscExcludePPn, 0)) > 0

insert into @JournalGL
select distinct '04 DISCOUNT SPAREPART['+a.TypeOfGoods+']', a.TypeOfGoods
	, (select top 1 DiscAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, (select sum(isnull(Quantity,0)*isnull(DiscExcludePPn,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode 
		and invoiceno=a.invoiceno and typeofgoods=a.typeofgoods group by typeofgoods) AmountDb
	, 0 AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
where a.companyCode=@CompanyCode 
	and a.BranchCode=@BranchCode 
	and a.InvoiceNo=@DocNo

insert into @JournalGL
select '05 SALES UNIT',a.SalesModelCode
	 , isnull((
		select acc.SalesAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '')
	 , 0
	 , sum(isnull(a.Quantity, 0) * isnull (a.AfterDiscDPP, 0))
	 + sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) as SalesUnit
  from omTrSalesInvoiceModel a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having (sum(isnull(a.Quantity, 0) * isnull (a.AfterDiscDPP, 0)) +
	    sum(isnull(a.Quantity, 0) * isnull (a.DiscExcludePPn, 0))) > 0

insert into @JournalGL
select '06 SALES AKSESORIS',a.SalesModelCode
	 , isnull((
		select acc.SalesAccNoAks
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '')
	 , 0
	 , sum(isnull(b.Quantity, 0) * isnull (a.AfterDiscDPP, 0))
	 + sum(isnull(b.Quantity, 0) * isnull (a.DiscExcludePPn, 0)) as SalesAksesoris
  from omTrSalesInvoiceOthers a, omTrSalesInvoiceModel b
 where 1 = 1
   and b.BranchCode = a.BranchCode 
   and b.InvoiceNo = a.InvoiceNo 
   and b.BPKNo = a.BPKNo 
   and b.SalesModelCode = a.SalesModelCode 
   and b.SalesModelYear = a.SalesModelYear 
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having (sum(isnull(b.Quantity, 0) * isnull (a.AfterDiscDPP, 0)) +
	    sum(isnull(b.Quantity, 0) * isnull (a.DiscExcludePPn, 0))) > 0

insert into @JournalGL
select distinct '07 SALES SPAREPART ['+a.typeOfGoods+']', a.TypeOfGoods
	, (select top 1 SalesAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, 0 AmountDb
	, (select sum(isnull(Quantity,0) * isnull(RetailPrice,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode 
		and invoiceno=a.invoiceno and typeofgoods=a.typeofgoods group by typeofgoods) AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.TypeOfGoods

insert into @JournalGL
select '08 PPN',''
	 , isnull((
		select cls.TaxOutAccNo
		  from omTrSalesInvoice ivu, GnMstCustomerProfitCenter cus, GnMstCustomerClass cls
		 where 1 = 1
		   and cus.CompanyCode   = ivu.CompanyCode
		   and cus.BranchCode    = ivu.BranchCode
		   and cus.CustomerCode  = ivu.CustomerCode
		   and cus.ProfitCenterCode = '100'
		   and cls.CompanyCode   = ivu.CompanyCode
		   and cls.BranchCode    = ivu.BranchCode
		   and cls.CustomerClass = cus.CustomerClass
		   and cus.CompanyCode   = @CompanyCode
		   and cus.BranchCode    = @BranchCode
		   and ivu.InvoiceNo     = @DocNo
		), '') 
	 , 0
	 , isnull((
		select sum(Quantity * AfterDiscPPn)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(mdl.Quantity * oth.AfterDiscPPn)
		  from omTrSalesInvoiceOthers oth left join omTrSalesInvoiceModel mdl
			on oth.BranchCode = mdl.BranchCode
			and oth.InvoiceNo = mdl.InvoiceNo
			and oth.BPKNo = mdl.BPKNo
			and oth.SalesModelCode = mdl.SalesModelCode
		 where 1 = 1
		   and oth.CompanyCode = @CompanyCode 
		   and oth.BranchCode  = @BranchCode
		   and oth.InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(PPN)
		  from omTrSalesInvoiceAccs
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + (select isnull(sum(isnull(Quantity,0)*isnull(PPN,0)),0) from omTrSalesInvoiceAccsSeq where companyCode = @CompanyCode 
		   and BranchCode=@BranchCode and InvoiceNo=@DocNo)
where (isnull((
		select sum(Quantity * AfterDiscPPn)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(AfterDiscPPn)
		  from omTrSalesInvoiceOthers
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
	 + isnull((
		select sum(PPN)
		  from omTrSalesInvoiceAccs
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)) 
	 +(select isnull(sum(isnull(quantity,0)*isnull(PPN,0)),0) from omTrSalesInvoiceAccsSeq where companyCode = @CompanyCode 
		   and BranchCode=@BranchCode and InvoiceNo=@DocNo) > 0

insert into @JournalGL
select '09 PPN BM',''
	 , isnull((
		select cls.LuxuryTaxAccNo
		  from omTrSalesInvoice ivu, GnMstCustomerProfitCenter cus, GnMstCustomerClass cls
		 where 1 = 1
		   and cus.CompanyCode   = ivu.CompanyCode
		   and cus.BranchCode    = ivu.BranchCode
		   and cus.CustomerCode  = ivu.CustomerCode
		   and cus.ProfitCenterCode = '100'
		   and cls.CompanyCode   = ivu.CompanyCode
		   and cls.BranchCode    = ivu.BranchCode
		   and cls.CustomerClass = cus.CustomerClass
		   and cus.CompanyCode   = @CompanyCode
		   and cus.BranchCode    = @BranchCode
		   and ivu.InvoiceNo     = @DocNo
		), '') 
	 , 0
	 , isnull((
		select sum(Quantity * AfterDiscPPnBm)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0)
where isnull((
		select sum(Quantity * AfterDiscPPnBm)
		  from omTrSalesInvoiceModel
		 where 1 = 1
		   and CompanyCode = @CompanyCode 
		   and BranchCode  = @BranchCode
		   and InvoiceNo   = @DocNo
		), 0) > 0

insert into @JournalGL
select '10 HPP Unit', a.SalesModelCode
	 , isnull((
		select acc.COGSAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '')
	 , sum(isnull (a.COGS, 0)) as COGS
	 , 0
  from OmTrSalesInvoiceVin a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull (a.COGS, 0)) > 0

insert into @JournalGL
select '11 INVENTORY UNIT', a.SalesModelCode
	 , isnull((
		select acc.InventoryAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = a.SalesModelCode
		), '')
	 , 0
	 , sum(isnull (a.COGS, 0)) as COGS
  from OmTrSalesInvoiceVin a
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.InvoiceNo   = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.InvoiceNo, a.BPKNo, a.SalesModelCode, a.SalesModelYear
having sum(isnull (a.COGS, 0)) > 0

insert into @JournalGL
select distinct '12 COGS SPAREPART ['+a.TypeOfGoods+']', a.TypeOfGoods
	, (select top 1 COGSAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, (select sum(isnull(Quantity,0)*isnull(COGS,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode 
			and invoiceno=a.invoiceno and typeofgoods=a.typeofgoods group by typeofgoods) AmountDb
	, 0 AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
where a.companyCode=@CompanyCode 
	and a.BranchCode=@BranchCode 
	and a.InvoiceNo=@DocNo

insert into @JournalGL
select distinct '13 INVENTORY AKSESORIES ['+a.TypeOfGoods+']', a.TypeOfGoods
	, (select top 1 InventoryAccNo from spMstAccount where companycode=a.companycode and branchcode=a.branchcode and typeofgoods=a.typeofgoods) AccountNo
	, 0 AmountDb
	, (select sum(isnull(Quantity,0)*isnull(COGS,0)) from omtrsalesinvoiceaccsseq where companycode=a.companycode and branchcode=a.branchcode and invoiceno=a.invoiceno
		and typeofgoods=a.typeofgoods group by typeofgoods) AmountCr
from omTrSalesInvoiceAccsSeq a 
inner join omTrSalesInvoice b on b.CompanyCode=a.CompanyCode
	and b.BranchCode=a.BranchCode
	and b.InvoiceNo=a.InvoiceNo
WHERE a.companyCode=@CompanyCode 
	and a.BranchCode=@BranchCode 
	and a.InvoiceNo=@DocNo

if exists (select * from @JournalGL where isnull(AccountNo, '') = '')
begin
	raiserror('terdapat transaksi yang belum memiliki AccountNo',16 ,1 );
	return
end

if (select abs(sum(AmountDb) - sum(AmountCr)) from @JournalGL) > 0
begin
	raiserror('journal belum balance, mohon di check kembali',16 ,1 );
	return
end

--select * from arInterface where DocNo = @DocNo
--delete arInterface where DocNo = @DocNo
insert into arInterface
(CompanyCode,BranchCode,DocNo,DocDate,ProfitCenterCode
,NettAmt,ReceiveAmt,CustomerCode
,TOPCode,DueDate,TypeTrans
,BlockAmt,DebetAmt,CreditAmt,SalesCode,LeasingCode,StatusFlag
,CreateBy,CreateDate,AccountNo,FakturPajakNo,FakturPajakDate)
select @CompanyCode CompanyCode, @BranchCode BranchCode
	 , b.InvoiceNo, b.InvoiceDate 
	 , '100' as ProfitCenterCode 
	 , a.AmountDb as NettAmt, 0 ReceiceAmt
	 , b.CustomerCode
	 , c.TOPCode, b.DueDate
	 , 'INVOICE' TypeTrans
	 , 0 as BlockAmt, 0 as DbAmt, 0 CrAmt
	 , c.SalesCode, c.LeasingCo
	 , '0' StatusFlag, @UserID CreatedBy, getdate() CreatedDate
	 , a.AccountNo, b.FakturPajakNo, b.FakturPajakDate
  from @JournalGL a, omTrSalesInvoice b, omTrSalesSO c
 where substring(CodeTrans, 4, len(CodeTrans) - 3) = 'AR'
   and c.CompanyCode = b.CompanyCode
   and c.BranchCode  = b.BranchCode
   and c.SONo        = b.SONo
   and b.CompanyCode = @CompanyCode
   and b.BranchCode  = @BranchCode
   and b.InvoiceNo   = @DocNo

--select * from glInterface where DocNo = @DocNo
--delete glInterface where DocNo = @DocNo
insert into glInterface
(CompanyCode,BranchCode,DocNo,SeqNo,DocDate,ProfitCenterCode
,AccDate,AccountNo,JournalCode,TypeJournal,ApplyTo,AmountDb,AmountCr
,TypeTrans,BatchNo,BatchDate,StatusFlag
,CreateBy,CreateDate,LastUpdateBy,LastUpdateDate)
select @CompanyCode CompanyCode, @BranchCode BranchCode
	 , b.InvoiceNo, (row_number() over (order by CodeTrans)) SeqNo
	 , b.InvoiceDate, '100' ProfitCenterCode
	 , b.InvoiceDate AccDate, a.AccountNo
	 , 'UNIT' JournalCode, 'INVOICE' TypeJournal
	 , b.InvoiceNo ApplyTo, a.AmountDb, a.AmountCr
	 , substring(CodeTrans, 4, len(CodeTrans) - 3) TypeTrans
	 , '' BatchNo, null BatchDate
	 , '0' StatusFlag, @UserID CreatedBy, getdate() CreatedDate
	 , @UserID LastUpdBy, getdate() LastUpdDate
  from @JournalGL a, omTrSalesInvoice b
 where 1 = 1
   and b.CompanyCode = @CompanyCode
   and b.BranchCode  = @BranchCode
   and b.InvoiceNo   = @DocNo

update omTrSalesInvoice
   set Status = '5'
 where 1 = 1
   and CompanyCode = @CompanyCode
   and BranchCode  = @BranchCode
   and InvoiceNo   = @DocNo

END
GO

if object_id('uspfn_SaveEmployeeMutation') is not null
	drop procedure uspfn_SaveEmployeeMutation
GO

CREATE procedure [dbo].[uspfn_SaveEmployeeMutation]
	@CompanyCode varchar(20),
	@EmployeeID varchar(20),
	@MutationDate datetime,
	@IsJoinDate bit,
	@BranchCode varchar(20),
	@UserID varchar(64)
as
begin
	declare @CurrentTime datetime;
	declare @JoinDate datetime;
	declare @ResignDate datetime;
	declare @Status bit;

	declare @Message varchar(150);
	declare @PrevMutation varchar(17);
	declare @NextMutation varchar(17);
	declare @NumberOfExistingRecord int;
	declare @NextMutationDate datetime;
	declare @PrevMutationDate datetime;
	
	declare @branch varchar(15);
	declare @UserEmployee varchar(15);

	set @NextMutation = null;
	set @PrevMutationDate = null;
	set @NumberOfExistingRecord = 0;
	set @Status=0;
	set @Message='';
	set @PrevMutation = '';
	set @NextMutation = ''
	set @CurrentTime = getDate();
	set @JoinDate = ( select top 1 a.JoinDate from HrEmployee a where a.CompanyCode=@CompanyCode and a.EmployeeID=@EmployeeID);
	set @ResignDate = ( select top 1 a.ResignDate from HrEmployee a where a.CompanyCode=@CompanyCode and a.EmployeeID=@EmployeeID);

	if @MutationDate < @JoinDate
	begin
		set @Message = 'Mutation datetime cannot less than join datetime.';
	end
	else if @MutationDate > @ResignDate and @ResignDate is not null
	begin
		set @Message = 'Mutation datetime cannot more than resign datetime.';
	end
	else 
	begin
		set @NumberOfExistingRecord = ( select count(*) from HrEmployeeMutation where CompanyCode=@CompanyCode and EmployeeID=@EmployeeID and convert(datetime, MutationDate)=@MutationDate );

		if @NumberOfExistingRecord > 0
		begin
			set @PrevMutation = (
				select top 1
				       a.BranchCode
				  from HrEmployeeMutation a
				 where a.CompanyCode=@CompanyCode
				   and a.EmployeeID=@EmployeeID
				   and convert(datetime, a.MutationDate) < @MutationDate
				 order by a.MutationDate desc
			);		
			
			set @NextMutation = (
				select top 1
					   a.BranchCode
				  from HrEmployeeMutation a
				 where a.CompanyCode=@CompanyCode
				   and a.EmployeeID=@EmployeeID
				   and convert(datetime, a.MutationDate) > @MutationDate
				 order by a.MutationDate desc
			);

			if @BranchCode = @PrevMutation
			begin			
				set @Message='There is mutation in the selected Branch before this mutation datetime.';
			end
			else if @BranchCode = @NextMutation
			begin
				set @Message='There is mutation in the selected Branch after this mutation datetime.';
			end
			else
			begin
				update HrEmployeeMutation
				   set IsDeleted=0
				     , BranchCode=@BranchCode
				 where CompanyCode=@CompanyCode
				   and EmployeeID=@EmployeeID
				   and convert(datetime, MutationDate)=@MutationDate

				if @IsJoinDate=1 
				begin
					update HrEmployeeMutation
					   set IsDeleted=1
					 where  CompanyCode=@CompanyCode and EmployeeID=@EmployeeID  and MutationDate < @MutationDate
				end
				
				set @Message = 'Data has been saved into database.';
				set @Status = convert(bit, 1);
			end
		end
		else
		begin
			insert into 
				   HrEmployeeMutation ( CompanyCode, EmployeeID, MutationDate, BranchCode, IsJoinDate, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, IsDeleted )
			values
				   (@CompanyCode, @EmployeeID, @MutationDate, @BranchCode, @IsJoinDate, @UserID, @CurrentTime, @UserID, @CurrentTime, 0)

			if @IsJoinDate=1
			begin
				update HrEmployeeMutation
				   set IsDeleted=1
				 where CompanyCode=@CompanyCode and EmployeeID=@EmployeeID  and MutationDate < @MutationDate
			end
				
			set @Status = convert(bit, 1);
			set @Message = 'Data has been saved.';
		end
		
		if exists (select * from HrEmployee where CompanyCode = @CompanyCode and EmployeeID = @EmployeeID and (RelatedUser is not null or RelatedUser = ''))
		begin
			set @UserEmployee = (select RelatedUser from HrEmployee where CompanyCode = @CompanyCode and EmployeeID = @EmployeeID)
			set @branch = (select top 1 BranchCode
						from HrEmployeeMutation
						where CompanyCode = @CompanyCode
						and EmployeeID = @EmployeeID
						and IsDeleted = 0
						order by MutationDate Desc)
			update SysUser
			set BranchCode = @branch
			where UserId = @UserEmployee
		end
	end

	select convert(bit, @Status) as Status, @Message as Message;
end

GO

if object_id('uspfn_SvUtlKsgClaimBatchList') is not null
	drop procedure uspfn_SvUtlKsgClaimBatchList
GO
CREATE procedure [dbo].[uspfn_SvUtlKsgClaimBatchList]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@KsgClaim    varchar(15),
	@UserID      varchar(15)
as  

declare @schema as table 
(
	Name    varchar(20),
	Caption varchar(90),
	Width   int,
	Format  varchar(20)
)

insert into @schema values ('BranchCode','Branch Code',80,'')
insert into @schema values ('BatchNo','Batch No',100,'')
insert into @schema values ('BatchDate','Batch Date',120,'dd-MMM-yyy  HH:mm:ss')
insert into @schema values ('FPJNo','FPJNo',100,'')
insert into @schema values ('FPJDate','FPJ Date',120,'dd-MMM-yyy  HH:mm:ss')
insert into @schema values ('FPJGovNo','FPJ Gov. No',130,'')

select * from @schema

if @KsgClaim = 'KSG'
begin
	-- update by fhi 23-04-2015 : penambahan kondisi jika FPJDate null FPJDate='1900-01-01 00:00:00.000'
	--*************************************************************************************************
	--select top 500 BranchCode, BatchNo, BatchDate, ReceiptNo, ReceiptDate, FPJNo, FPJDate, FPJGovNo
	--  from svTrnPdiFscBatch
	select top 500 BranchCode, BatchNo, BatchDate, ReceiptNo, ReceiptDate, FPJNo,
	case when FPJDate is null then '1900-01-01 00:00:00.000' else  FPJDate end FPJDate,
	 FPJGovNo
	  from svTrnPdiFscBatch
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	 order by BatchNo desc
end
else
begin
	select top 500 BranchCode, BatchNo, BatchDate, ReceiptNo, ReceiptDate, FPJNo, FPJDate, FPJGovNo
	  from svTrnClaimBatch
	 where 1 = 1
	   and CompanyCode = @CompanyCode
	   and BranchCode = @BranchCode
	 order by BatchNo desc
end


GO


if object_id('uspfn_HrGetTeamLeader') is not null
	drop procedure uspfn_HrGetTeamLeader
GO

CREATE procedure [dbo].[uspfn_HrGetTeamLeader]
	@CompanyCode varchar(10),
	@DeptCode varchar(10),
	@PosCode varchar(10)
as

declare @table as table(value varchar(200), text varchar(200))
declare @curpos as varchar(200)

set @curpos = isnull((
				select top 1 PosHeader
				  from GnMstPosition
				 where CompanyCode = @CompanyCode
				   and DeptCode = @DeptCode
				   and PosCode = @PosCode
				  ), '') 

while (@curpos != '' and @DeptCode != 'COM')
begin
	insert into @table
	select a.EmployeeID, a.EmployeeName + ' (' + @curpos + ')' 
	  from HrEmployee a
	 where CompanyCode = @CompanyCode
	   and (Department = @DeptCode or Department = 'COM')
	   and Position = @curpos
	   and PersonnelStatus = '1'
   
	set @curpos = isnull((
					select top 1 PosHeader
					  from GnMstPosition
					 where CompanyCode = @CompanyCode
					   and (DeptCode = @DeptCode or DeptCode = 'COM')
					   and PosCode = @curpos
					  ), '') 
end

select * from @table


go

if object_id('uspfn_omSoLkp') is not null
	drop procedure uspfn_omSoLkp
GO
CREATE procedure [dbo].[uspfn_omSoLkp] 
(
	@CompanyCode varchar(25),
	@BranchCode varchar(25)
)
as
 
 -- exec uspfn_omSoLkp '6115204001','6115204105'
 
 declare @DbMD as varchar(15)  
 declare @Sql as varchar(max) 
 declare @ssql as varchar(max) 
 
 set @DbMD = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 


 set @ssql='select * from gnMstCompanyMapping '

set @Sql= 'SELECT a.CompanyCode, a.BranchCode,
                a.SONo, a.SODate, a.SalesType, a.CustomerCode, a.TOPCode, a.Installment, a.FinalPaymentDate,
                a.TOPDays, a.BillTo, a.ShipTo, a.ProspectNo, a.SKPKNo, a.Salesman, a.WareHouseCode, a.isLeasing, 
                a.LeasingCo, a.GroupPriceCode, a.Insurance, a.PaymentType, a.PrePaymentAmt, a.PrePaymentBy, 
                a.CommissionBy, a.CommissionAmt, a.PONo, a.ContractNo, a.Remark, a.Status,
                a.SalesCoordinator, a.SalesHead, a.BranchManager, a.RefferenceNo,
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then '''' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDates, 
                CASE convert(varchar, a.RefferenceDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RefferenceDate, 111) end as RefferenceDate, 
                CASE convert(varchar, a.RequestDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.RequestDate, 111) end as RequestDate,
                CASE convert(varchar, a.PrePaymentDate, 111) when convert(varchar, ''1900/01/01'') 
                then ''undefined'' else convert(varchar, a.PrePaymentDate, 111) end as PrePaymentDate,
                e.Address1 + '' '' + e.Address2 + '' '' + e.Address3 + '' '' + e.Address4 as Address,
                case when year(a.RefferenceDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC1,
                case when a.SKPKNo <> '''' then convert(bit,1) else convert(bit,0) end isC2,
                case when year(a.PrePaymentDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC3,
                case when year(a.RequestDate) <> 1900 then convert(bit,1) else convert(bit,0) end isC4,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS CustomerName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID) as SalesmanName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.Shipto = b.CustomerCode)  
						AS ShipName,
                (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.LeasingCo = b.CustomerCode)  
						AS LeasingCoName,
				(SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.PrePaymentby = c.EmployeeID) as PrePaymentName,
				(SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode) AS GroupPriceName,
				(SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode)  
						AS BillName,
				(SELECT b.lookupvaluename
                        FROM '+@DbMD+'..gnMstLookUpDtl b
                        WHERE a.WareHouseCode = b.LookUpValue
						AND a.WareHouseCode = b.LookUpValue and CodeID =''MPWH'')  
						AS WareHouseName,
                (a.CustomerCode
                    + '' || ''
                    + (SELECT b.CustomerName
                        FROM gnMstCustomer b
                        WHERE a.CompanyCode = b.CompanyCode
						AND a.CustomerCode = b.CustomerCode))  
						AS Customer, 
                (a.Salesman
                    + '' || ''
                    + (SELECT c.EmployeeName
                        FROM gnMstEmployee c
                        WHERE a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode
					    AND a.Salesman = c.EmployeeID))  AS Sales, 
                (a.GroupPriceCode
                    + '' || ''
                    + (SELECT d.RefferenceDesc1
                        FROM omMstRefference d
                        WHERE a.CompanyCode = d.CompanyCode
						AND d.RefferenceType = ''GRPR'' 
                        AND d.RefferenceCode = a.GroupPriceCode))  AS GroupPrice, 
                CASE a.Status when 0 then ''OPEN''
                                when 1 then ''PRINTED''
                                when 2 then ''APPROVED''
                                when 3 then ''DELETED''
                                when 4 then ''REJECTED''
                                when 9 then ''FINISHED'' END as Stat
                , CASE ISNULL(a.SalesType, 0) WHEN 0 THEN ''Wholesales'' ELSE ''Direct'' END AS TypeSales
                ,(select distinct x.TipeKendaraan
                    from pmKDP x
	                    left join gnMstEmployee b on x.CompanyCode=b.CompanyCode and x.BranchCode=b.BranchCode
		                    and x.EmployeeID=b.EmployeeID
	                    left join omTrSalesSO c on c.CompanyCode = x.CompanyCode 
		                    and c.BranchCode = x.BranchCode
		                    and c.ProspectNo = x.InquiryNumber
	                    left join omTrSalesInvoice d on d.CompanyCode = x.CompanyCode
		                    and d.BranchCode = x.BranchCode
		                    and d.SONo = c.SONo
	                    left join omTrSalesReturn e on e.CompanyCode = x.CompanyCode
		                    and e.BranchCode = x.BranchCode
		                    and e.InvoiceNo = d.InvoiceNo
                    where x.InquiryNumber=a.ProspectNo) as VehicleType
                FROM omTrSalesSO a
                INNER JOIN gnMstCustomer e
                ON a.CompanyCode = e.CompanyCode AND a.CustomerCode = e.CustomerCode
				Where a.CompanyCode = '+ @CompanyCode+' and a.BranchCode = '+ @BranchCode +'
				order by a.SONo desc
				'
--print @Sql

exec (@Sql)

GO

if object_id('usprpt_PmRpInqPeriodeWeb') is not null
	drop procedure usprpt_PmRpInqPeriodeWeb
GO

CREATE procedure [dbo].[usprpt_PmRpInqPeriodeWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@PeriodBegin		DATETIME,
	@PeriodEnd			DATETIME,
	@BranchManager		VARCHAR(50),
	@SalesHead			VARCHAR(15),
	@Salesman			VARCHAR(15)
)
AS 
BEGIN
SET NOCOUNT ON;
----
--declare @BranchManager VARCHAR(50)
--set @BranchManager = (SELECT a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
--		AND a.CompanyCode = @CompanyCode AND a.EmployeeID = @SalesHead )
		
IF(@SalesHead ='' AND @Salesman ='')
BEGIN
	SELECT * INTO #empl1 FROM (
		--SH =ALL AND S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  
		a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader=@BranchManager)
	)#empl1

	SELECT * INTO #t1 FROM (
		SELECT
			f.BranchName OutletName, a.InquiryNumber, a.NamaProspek Pelanggan, a.AlamatProspek, a.TelpRumah, a.NamaPerusahaan, 
			a.AlamatPerusahaan, a.Handphone, a.InquiryDate, a.TipeKendaraan, a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, 
			a.PerolehanData, c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress,a.LastUpdateStatus, 
			a.SPKDate, a.LostCaseDate, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND CONVERT(varchar(20),a.InquiryDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl1 g)
	) #t1

	DROP TABLE #empl1
	SELECT OutletName
		,InquiryNumber
		,Pelanggan
		,AlamatProspek
		,TelpRumah
		,NamaPerusahaan
		,AlamatPerusahaan
		,Handphone
		,CASE(InquiryDate) WHEN '19000101' THEN '' ELSE InquiryDate END InquiryDate
		,TipeKendaraan
		,Variant
		,Transmisi
		,Warna
		,PerolehanData
		,Employee
		,Supervisor
		,CASE(NextFollowUpDate) WHEN '19000101' THEN '' ELSE NextFollowUpDate END NextFollowUpDate
		,LastProgress
		,LastUpdateStatus
		,CASE(SPKDate) WHEN '19000101' THEN '' ELSE SPKDate END SPKDate
		,CASE(LostCaseDate) WHEN '19000101' THEN '' ELSE LostCaseDate END LostCaseDate, ActivityDetail FROM #t1 ORDER BY InquiryNumber
	DROP TABLE #t1

END
ELSE IF(@Salesman = '')
BEGIN
	SELECT * INTO #empl2 FROM (
		--S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  
		a.TeamLeader =@SalesHead
	)#empl2

	SELECT * INTO #t2 FROM (
		SELECT
			f.BranchName OutletName, a.InquiryNumber, a.NamaProspek Pelanggan, a.AlamatProspek, a.TelpRumah, a.NamaPerusahaan, 
			a.AlamatPerusahaan, a.Handphone, a.InquiryDate, a.TipeKendaraan, a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, 
			a.PerolehanData, c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress,a.LastUpdateStatus, 
			a.SPKDate, a.LostCaseDate, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND CONVERT(varchar(20),a.InquiryDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl2 g)
	) #t2
	
	DROP TABLE #empl2
	SELECT OutletName
		--,convert(varchar(20),InquiryNumber) 
		,InquiryNumber
		,Pelanggan
		,AlamatProspek
		,TelpRumah
		,NamaPerusahaan
		,AlamatPerusahaan
		,Handphone
		,CASE(InquiryDate) WHEN '19000101' THEN '' ELSE InquiryDate END InquiryDate
		,TipeKendaraan
		,Variant
		,Transmisi
		,Warna
		,PerolehanData
		,Employee
		,Supervisor
		,CASE(NextFollowUpDate) WHEN '19000101' THEN '' ELSE NextFollowUpDate END NextFollowUpDate
		,LastProgress
		,LastUpdateStatus
		,CASE(SPKDate) WHEN '19000101' THEN '' ELSE SPKDate END SPKDate
		,CASE(LostCaseDate) WHEN '19000101' THEN '' ELSE LostCaseDate END LostCaseDate, ActivityDetail FROM #t2 ORDER BY InquiryNumber
	DROP TABLE #t2
END
ELSE
BEGIN
	SELECT * INTO #empl3 FROM (
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  
		a.EmployeeID=@Salesman
	)#empl3

	SELECT * INTO #t3 FROM (
		SELECT
			f.BranchName OutletName, a.InquiryNumber, a.NamaProspek Pelanggan, a.AlamatProspek, a.TelpRumah, a.NamaPerusahaan, 
			a.AlamatPerusahaan, a.Handphone, a.InquiryDate, a.TipeKendaraan, a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, 
			a.PerolehanData, c.EmployeeName Employee, d.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress,a.LastUpdateStatus, 
			a.SPKDate, a.LostCaseDate, e.ActivityDetail
			FROM PmKDP a
		LEFT JOIN OmMstRefference b
			ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
		LEFT JOIN HrEmployee c
			ON c.CompanyCode = a.CompanyCode AND c.EmployeeID = a.EmployeeID
		LEFT JOIN HrEmployee d
			ON d.CompanyCode = a.CompanyCode AND d.EmployeeID = a.SpvEmployeeID
		LEFT JOIN PmActivities e
			ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
			AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
			AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
		LEFT JOIN gnMstOrganizationDtl f
			ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode
		WHERE
			a.CompanyCode = @CompanyCode 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			AND CONVERT(varchar(20),a.InquiryDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl3 g)
	) #t3

	DROP TABLE #empl3
	SELECT OutletName
		,InquiryNumber
		,Pelanggan
		,AlamatProspek
		,TelpRumah
		,NamaPerusahaan
		,AlamatPerusahaan
		,Handphone
		,CASE(InquiryDate) WHEN '19000101' THEN '' ELSE InquiryDate END InquiryDate
		,TipeKendaraan
		,Variant
		,Transmisi
		,Warna
		,PerolehanData
		,Employee
		,Supervisor
		,CASE(NextFollowUpDate) WHEN '19000101' THEN '' ELSE NextFollowUpDate END NextFollowUpDate
		,LastProgress
		,LastUpdateStatus
		,CASE(SPKDate) WHEN '19000101' THEN '' ELSE SPKDate END SPKDate
		,CASE(LostCaseDate) WHEN '19000101' THEN '' ELSE LostCaseDate END LostCaseDate, ActivityDetail FROM #t3 ORDER BY InquiryNumber
	DROP TABLE #t3
END
----
END

GO

if object_id('uspfn_SvTrnServiceSaveItem') is not null
	drop procedure uspfn_SvTrnServiceSaveItem
GO
CREATE procedure [dbo].[uspfn_SvTrnServiceSaveItem]  
--DECLARE
	@CompanyCode varchar(15),  
	@BranchCode varchar(15),  
	@ProductType varchar(15),  
	@ServiceNo bigint,  
	@BillType varchar(15),  
	@PartNo varchar(20),  
	@DemandQty numeric(18,2),  
	@PartSeq numeric(5,2),  
	@UserID varchar(15),  
	@DiscPct numeric(5,2)  
as        
  
--set @CompanyCode = '6115204001'  
--set @BranchCode = '6115204102'  
--set @ProductType = '2W'  
--set @ServiceNo = 16455  
--set @BillType = 'C'  
--set @PartNo = 'K1200-50002-000'  
--set @DemandQty = 1 
--set @PartSeq = -1  
--set @UserID = 'yo'  
--set @DiscPct = 0  

declare @errmsg varchar(max)  
declare @QueryTemp as varchar(max)  
declare @IsSPK as char(1)
  
begin try  
 -- select data svTrnService  
 select * into #srv from (  
   select a.* from svTrnService a  
  where 1 = 1  
    and a.CompanyCode = @CompanyCode  
    and a.BranchCode  = @BranchCode  
    and a.ProductType = @ProductType  
    and a.ServiceNo   = @ServiceNo  
 )#srv  
   
 declare @CostPrice as decimal  
 declare @RetailPrice as decimal  
 declare @TypeOfGoods as varchar(2)  
 declare @CostPriceMD as decimal  
 declare @RetailPriceMD as decimal  
 declare @RetailPriceInclTaxMD as decimal  
   
 declare @DealerCode as varchar(2)  
 declare @CompanyMD as varchar(15)  
 declare @BranchMD as varchar(15)  
 declare @WarehouseMD as varchar(15)  
  
 set @DealerCode = 'MD'  
 set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 set @WarehouseMD = (select WarehouseMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 
if object_id('#tmpSvSDMovement') is not null drop table #tmpSvSDMovement
 
 -- Check MD or SD
	-- If SD  
 if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)   
 begin  
	  set @DealerCode = 'SD'  

	  set @IsSPK = isnull((select a.ServiceType from #srv a where a.ServiceType = '2'),0)
	  
	  declare @DbName as varchar(50)  
	  set @DbName = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
	  
	  declare @PurchaseDisc as decimal  
	  set @PurchaseDisc = (select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
		   where CompanyCode = @CompanyCode   
		   and BranchCode = @BranchCode  
		   and SupplierCode = @BranchMD
		   and ProfitCenterCode = '300')  
	         
	  if (@PurchaseDisc = 0) raiserror ('Purchase Discount belum di-setting untuk Part tersebut!',16,1);            
	       
	  declare @tblTemp as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )
	  
	  declare @tblTemp1 as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )
	    
	  -- Untuk ItemPrice Mengambil dari masing-masing dealer	
		set @QueryTemp = 'select   
			  b.CostPrice   
			 ,b.RetailPrice  
			 ,b.RetailPriceInclTax  
			 ,a.TypeOfGoods 
			from (select
				i.PartNo   
				,i.TypeOfGoods  
				 from ' + @DbName +'..spMstItems i  
				 where i.CompanyCode = ''' + @CompanyMD + '''  
				 and i.BranchCode  = ''' + @BranchMD + '''  
				 and i.PartNo      = ''' + @PartNo + '''
			) a inner join spMstItemPrice b on b.PartNo = a.PartNo
		 where b.CompanyCode = ''' + @CompanyCode + '''
		 and b.BranchCode = ''' + @BranchCode + ''''
		          
	  insert into @tblTemp    
	  exec (@QueryTemp)  
	  
		set @QueryTemp = 'select   
			  b.CostPrice   
			 ,b.RetailPrice  
			 ,b.RetailPriceInclTax  
			 ,a.TypeOfGoods 
			from (select
				i.PartNo   
				,i.TypeOfGoods  
				 from ' + @DbName +'..spMstItems i  
				 where i.CompanyCode = ''' + @CompanyMD + '''  
				 and i.BranchCode  = ''' + @BranchMD + '''  
				 and i.PartNo      = ''' + @PartNo + '''
			) a inner join ' + @DbName +'..spMstItemPrice b on b.PartNo = a.PartNo
		 where b.CompanyCode = ''' + @CompanyMD + '''
		 and b.BranchCode = ''' + @BranchMD + ''''
		 
  	  insert into @tblTemp1
	  exec (@QueryTemp)  
	  print (@QueryTemp)  
	  
	  set @CostPrice = ((select RetailPriceInclTax from @tblTemp1) - ((select RetailPriceInclTax from @tblTemp1) * @PurchaseDisc * 0.01))  
	  select @CostPrice  
	  set @RetailPrice = (select RetailPrice from @tblTemp)
	  --select a.RetailPrice from spMstItemPrice a where a.CompanyCode = @CompanyCode and a.BranchCode = @BranchCode and a.PartNo = @PartNo)    
	  set @TypeOfGoods = (select TypeOfGoods from @tblTemp)  
	    
	  set @CostPriceMD = (select CostPrice from @tblTemp)  
	  set @RetailPriceMD = (select RetailPrice from @tblTemp)  
	  set @RetailPriceInclTaxMD = (select RetailPriceInclTax from @tblTemp)  
	    
	  -- select @PurchaseDisc  
 end   
 -- If MD
 else  
 begin
	 declare @tblTempPart as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )  

	  set @QueryTemp = 'select   
		a.CostPrice   
	   ,a.RetailPrice  
		 from ' + @DbName + '..spMstItemPrice a  
	   where 1 = 1  
		 and a.CompanyCode = ''' + @CompanyMD + '''  
		 and a.BranchCode  = ''' + @BranchMD + '''  
		 and a.PartNo      = ''' + @PartNo + ''''  
	          
	  insert into @tblTempPart    
	  exec (@QueryTemp)  
	   
	  --select * into #part from (  
	  --select   
	  --  a.CostPrice   
	  -- ,a.RetailPrice  
	  --  from spMstItemPrice a  
	  -- where 1 = 1  
	  --   and a.CompanyCode = @CompanyCode  
	  --   and a.BranchCode  = @BranchCode  
	  --   and a.PartNo      = @PartNo  
	  --)#part  
	    
	  set @CostPrice = (select CostPrice from @tblTempPart)  
	  set @RetailPrice = (select RetailPrice from @tblTempPart)  
 end  
 -- EOF Check MD or SD
  
 
 if (@PartSeq > 0)  
 begin    
	-- select data mst job  
	select * into #job from (  
	select b.*  
	from svTrnService a, svMstJob b  
	where 1 = 1  
	 and b.CompanyCode = a.CompanyCode  
	 and b.ProductType = a.ProductType  
	 and b.BasicModel = a.BasicModel  
	 and b.JobType = a.JobType  
		and a.CompanyCode = @CompanyCode  
	 and a.BranchCode  = @BranchCode  
	 and a.ServiceNo   = @ServiceNo  
	 and b.GroupJobType = 'FSC'  
	)#  
	if exists (select * from #job)  
	begin  
	   -- update svTrnSrvItem  
	   set @Querytemp ='
	   update svTrnSrvItem set  
		 DemandQty      = '+ convert(varchar,@DemandQty) +'
		,CostPrice      = '+ convert(varchar,@CostPrice) +' 
		,RetailPrice    = isnull((  
			 select top 1 b.RetailPrice from #srv a, svMstTaskPart b  
			  where b.CompanyCode = a.CompanyCode  
				and b.ProductType = a.ProductType  
				and b.BasicModel = a.BasicModel  
				and b.JobType = a.JobType  
				and b.PartNo = '''+ @PartNo +''' 
				and b.BillType = ''F'' 
			 ), (  
			  select top 1 isnull(RetailPrice, 0) RetailPrice  
				from spMstItemPrice  
			   where 1 = 1  
				 and CompanyCode = '''+ @CompanyCode +'''
				 and BranchCode = '''+ @BranchCode +'''
				 and PartNo = '''+ @PartNo  +'''
			  )  
			 )  
		,LastupdateBy   = (select LastupdateBy from #srv)  
		,LastupdateDate = (select LastupdateDate from #srv)  
		,BillType       = '''+ @BillType +'''
		,DiscPct        = '+ convert(varchar,@DiscPct) +'  
		where 1 = 1  
		  and CompanyCode  = '''+ @CompanyCode +''' 
		  and BranchCode   = '''+ @BranchCode +''' 
		  and ProductType  = (select ProductType from #srv)  
		  and ServiceNo    = (select ServiceNo from #srv)  
		  and PartNo       = '''+ @PartNo +''' 
		  and PartSeq      = '+ convert(varchar,@PartSeq) +'' 
		  
		  exec(@QueryTemp) 
	  end  
	  else  
	  begin  
	   -- update svTrnSrvItem  
	   update svTrnSrvItem set  
		 DemandQty      = @DemandQty  
		,CostPrice      = @CostPrice  
		,RetailPrice    = @RetailPrice  
		,LastupdateBy   = (select LastupdateBy from #srv)  
		,LastupdateDate = (select LastupdateDate from #srv)  
		,BillType       = @BillType  
		,DiscPct        = @DiscPct  
		where 1 = 1  
		  and CompanyCode  = @CompanyCode  
		  and BranchCode   = @BranchCode  
		  and ProductType  = (select ProductType from #srv)  
		  and ServiceNo    = (select ServiceNo from #srv)  
		  and PartNo       = @PartNo  
		  and PartSeq      = @PartSeq           
	  end   
	    
	--update svSDMovement  
	if (@DealerCode = 'SD' and @IsSPK = '2')  
	begin    
		set @QueryTemp = 'update ' + @DbName + '..svSDMovement set    
		QtyOrder    = ' + case when @DemandQty is null then '0' else convert(varchar, @DemandQty) end + ' 
		,DiscPct    = ' + case when  @DiscPct is null then '0' else convert(varchar, @DiscPct) end + '
		,CostPrice    = ' + case when @CostPrice is null then '0' else convert(varchar, @CostPrice) end + '  
		,RetailPrice   = ' + case when @RetailPrice is null then '0' else convert(varchar, @RetailPrice) end + '  
		,CostPriceMD   = ' + case when @CostPriceMD is null then '0' else convert(varchar, @CostPriceMD) end + '  
		,RetailPriceMD   = ' + case when @RetailPriceMD is null then '0' else convert(varchar, @RetailPriceMD) end + '  
		,RetailPriceInclTaxMD = ' + case when @RetailPriceInclTaxMD is null then '0' else convert(varchar, @RetailPriceInclTaxMD) end + '  
		,[Status]  = ''' + case when (select ServiceStatus from #srv) is null then '''' else (select ServiceStatus from #srv) end + '''  
		,LastupdateBy   = ''' + case when (select LastupdateBy from #srv) is null then '''' else (select LastupdateBy from #srv) end + '''  
		,LastupdateDate = ''' + case when (select LastupdateDate from #srv) is null then '''' else convert(varchar,(select LastupdateDate from #srv)) end + '''  
		where CompanyCode = ''' + case when @CompanyCode is null then '''' else @CompanyCode end + '''
		  and BranchCode = ''' + case when @BranchCode is null then '''' else @BranchCode end + '''
		  and DocNo  = ''' + case when (select JobOrderNo from #srv) is null then '''' else (select JobOrderNo from #srv) end + '''  
		  and PartNo       =  ''' + case when @PartNo is null then '''' else @PartNo end  + '''
		  and PartSeq      = ' + case when @PartSeq is null then '0' else convert(varchar, @PartSeq) end + '';  
		  
		  --print @QueryTemp;  
		exec 	(@QueryTemp);
	end
 end  
 else  
 begin  
	if((select count(*) from svTrnSrvItem  
	where 1 = 1  
	  and CompanyCode  = @CompanyCode  
	  and BranchCode   = @BranchCode  
	  and ProductType  = (select ProductType from #srv)  
	  and ServiceNo    = (select ServiceNo from #srv)  
	  and PartNo       = @PartNo  
	  and (isnull(SupplySlipNo,'') = '')  
	) > 0)  
	begin  
		raiserror ('Part yang sama sudah diproses di Entry SPK namun belum dapat No SSS, mohon diselesaikan dahulu!',16,1);  
	end  

	declare @PartSeqNew as int  
	set @PartSeqNew = (isnull((select isnull(max(PartSeq), 0) + 1    
	  from svTrnSrvItem   
		where CompanyCode = @CompanyCode  
	   and BranchCode  = @BranchCode   
	   and ProductType = @ProductType  
	   and ServiceNo   = @ServiceNo  
	   and PartNo      = @PartNo), 1))  
	     
	-- insert svTrnSrvItem  
	set @QueryTemp=' insert into svTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct, MechanicID)  
	select   
	'''+ @CompanyCode +''' CompanyCode  
	,''' + @BranchCode +''' BranchCode  
	,'''+ @ProductType +''' ProductType  
	,'+ convert(varchar,@ServiceNo) +' ServiceNo  
	,a.PartNo  
	,'''+ convert(varchar,@PartSeqNew)  +'''
	--,(row_number() over (order by a.PartNo)) PartSeq  
	,'+ convert(varchar,@DemandQty )+' DemandQty  
	,''0'' SupplyQty  
	,''0'' ReturnQty  
	,'+ convert(varchar,isnull(@CostPrice,0))  +'
	,a.RetailPrice   
	,b.TypeOfGoods  
	,'''+ @BillType +''' BillType  
	,null SupplySlipNo  
	,null SupplySlipDate  
	,null SSReturnNo  
	,null SSReturnDate  
	,c.LastupdateBy CreatedBy  
	,c.LastupdateDate CreatedDate  
	,c.LastupdateBy  
	,c.LastupdateDate  
	,'+ convert(varchar,isnull(@DiscPct,0))  +'
	,(select MechanicID from svTrnService where CompanyCode = '''+ @CompanyCode +''' and BranchCode = '''+ @BranchCode +''' and ServiceNo = '+ convert(varchar,@ServiceNo) +')  
    from spMstItemPrice a, '+ @DbName +'..spMstItems b, 
    #srv c, gnmstcompanymapping d 
   where 1 = 1  
	 and d.CompanyMd = b.CompanyCode
	 and d.BranchMD = b.BranchCode
        and d.CompanyCode = c.CompanyCode  
     and d.BranchCode  = c.BranchCode  
     and b.PartNo      = a.PartNo  
        and (b.CompanyCode = '''+ @CompanyMD +'''
     and b.BranchCode  = '''+ @BranchMD +'''
     and b.PartNo      = '''+ @PartNo +''')
     and (a.CompanyCode = '''+ @CompanyCode +'''
     and a.BranchCode  = '''+ @BranchCode +'''
     and a.PartNo      = '''+ @PartNo +''')' 
		   
	exec(@QueryTemp)

	--print(@QueryTemp)

	--select   @CostPrice   
	--xxx

	if (@DealerCode = 'SD' and @IsSPK = '2')  
	begin
		create table #tmpSvSDMovement(
			CompanyCode varchar(15)
			,BranchCode varchar(15)
			,JobOrderNo varchar(20)   
			,JobOrderDate datetime  
			,PartNo varchar(20)
			,PartSeqNew int
			,WarehouseMD varchar(20)   
			,DemandQty numeric(18,2)
			,Qty numeric(18,2)
			,DiscPct numeric(18,2)
			,CostPrice numeric(18,2)
			,RetailPrice numeric(18,2) 
			,TypeOfGoods varchar(15) 
			,CompanyMD varchar(15)
			,BranchMD varchar(15)   
			,WarehouseMD2 varchar(15)
			,RetailPriceInclTaxMD numeric(18,2) 
			,RetailPriceMD numeric(18,2) 
			,CostPriceMD numeric(18,2)  
			,QtyFlag char(1)
			,ProductType varchar(15) 
			,ProfitCenterCode varchar(15)
			,Status char(1)
			,ProcessStatus char(1)
			,ProcessDate datetime 
			,CreatedBy varchar(15) 
			,CreatedDate datetime 
			,LastUpdateBy varchar(15) 
			,LastUpdateDate datetime	
		);

		insert into #tmpSvSDMovement 
			select case when @CompanyCode is null then '' else @CompanyCode end 
			,case when @BranchCode is null then '' else @BranchCode end
			,case when (select JobOrderNo from #srv) is null then convert(varchar,@ServiceNo) else (select JobOrderNo from #srv) end
			,case when (select JobOrderDate from #srv) is null then '1900/01/01' else (select JobOrderDate from #srv) end 
			,case when @PartNo is null then '' else  @PartNo end 
			,case when @PartSeqNew is null then '0' else convert(varchar, @PartSeqNew) end
			,case when @WarehouseMD is null then '' else @WarehouseMD end  
			,case when @DemandQty  is null then '0' else convert(varchar, @DemandQty) end
 			,case when @DemandQty  is null then '0' else convert(varchar, @DemandQty) end
			,case when @DiscPct is null then '0' else convert(varchar, @DiscPct) end  
			,case when @CostPrice is null then '0' else convert(varchar, @CostPrice) end 
			,case when @RetailPrice is null then '0' else convert(varchar, @RetailPrice) end  
			,case when @TypeOfGoods is null then '' else @TypeOfGoods end 
			,case when @CompanyMD is null then '' else @CompanyMD end   
			,case when @BranchMD is null then '' else @BranchMD end  
			,case when @WarehouseMD is null then '' else @WarehouseMD end  
			,case when @RetailPriceInclTaxMD is null then '0' else convert(varchar, @RetailPriceInclTaxMD) end  
			,case when @RetailPriceMD is null then '0' else convert(varchar, @RetailPriceMD) end   
			,case when @CostPriceMD is null then '0' else convert(varchar, @CostPriceMD) end
			,'x'
			,case when @ProductType is null then '' else @ProductType end  
			,'300'  
			,'0' 
			,'0'
			,'1900/01/01'  
			,case when (select CreatedBy from #srv) is null then '' else (select CreatedBy from #srv) end     
			,case when (select CreatedDate from #srv) is null then '1900/01/01' else convert(varchar,(select CreatedDate from #srv)) end 
			,case when (select LastUpdateBy from #srv) is null then '' else (select LastUpdateBy from #srv) end
			,case when (select LastUpdateDate from #srv) is null then '1900/01/01' else convert(varchar,(select LastUpdateDate from #srv)) end
		 
		set @QueryTemp = '
		insert into ' + @DbName + '..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice,   
		TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, 
		Status, ProcessStatus, ProcessDate, CreatedBy,   
		CreatedDate, LastUpdateBy, LastUpdateDate)  
		select * from #tmpSvSDMovement';
		exec(@QueryTemp);

		drop table #tmpSvSDMovement;     

	end   
 end  

 update svTrnSrvItem  
    set DiscPct = @DiscPct  
  where 1 = 1  
    and CompanyCode = @CompanyCode  
    and BranchCode = @BranchCode  
    and ProductType = @ProductType  
    and ServiceNo = @ServiceNo  
    and PartNo = @PartNo  
   
 if (@DealerCode = 'SD' and @IsSPK = '2')  
 begin    
	set @QueryTemp = 'update ' + @DbName + '..svSDMovement   
	  set DiscPct = ' + convert(varchar,@DiscPct) 
	  + ' where 1 = 1'  
	  +	' and CompanyCode = ''' + case when @CompanyMD is null then '''' else  @CompanyMD end + ''''
	  + ' and BranchCode = ''' + case when @BranchMD is null then '''' else  @BranchMD end + ''''
	  + ' and DocNo = ''' + case when (select JobOrderNo from #srv) is null then '''' else (select JobOrderNo from #srv) end  + ''''
	  + ' and PartNo = ''' + case when @PartNo is null then '''' else @PartNo end + ''''  
	  + ' and PartSeq = ' + convert(varchar,@PartSeq) + '';
  
	exec (@QueryTemp)  
 end  
   
	drop table #srv  
end try  
begin catch  
 set @errmsg = error_message()  
 raiserror (@errmsg,16,1);  
end catch  

--rollback tran
GO

if object_id('uspfn_GetVehicleInfo_New') is not null
	drop procedure uspfn_GetVehicleInfo_New
GO

CREATE procedure [dbo].[uspfn_GetVehicleInfo_New]  
 @CompanyCode  varchar(20),  
 @BranchCode   varchar(20),  
 @ProductType  varchar(10),  
 @PoliceRegNo  varchar(20),  
 @ChassisCode  varchar(20),  
 @ChassisNo    varchar(10),  
 @BasicModel   varchar(20),  
 @JobOrderDate varchar(20),
 @CustomerCode varchar(20),
 @IsAllBranch  bit = 1
 
as  
  
select * into #t1 from (  
select 0 TaskPartSeq  
     , a.BranchCode  
     , a.JobOrderNo  
     , a.JobOrderDate  
     , d.InvoiceNo  
     , d.InvoiceDate  
     , d.FPJNo  
     , d.FPJDate  
     , a.JobType + ' - ' + e.Description JobType  
     , a.Odometer  
     , c.MechanicId+' - '+  
  (  
  select EmployeeName   
  from gnMstEmployee   
  where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and EmployeeID=c.MechanicId  
  ) MechanicId  
     , a.ForemanId  
     , b.OperationNo  
     , isnull(b.OperationHour, 0) as OperationQty  
     , convert(decimal, isnull(b.OperationCost, 0) * isnull(b.OperationHour, 0) * 1.0) as OperationAmt  
     , convert(decimal, isnull(b.OperationCost, 0) * isnull(b.OperationHour, 0) * 1.1) as TotalSrvAmount  
     , isnull(b.SharingTask, 0) SharingTask  
     , case when f.Description is null then (select TOP 1 Description   
    from svMstTask   
     where BasicModel = a.BasicModel  
      and OperationNo = b.OperationNo) else f.Description end Description  
    , isnull(g.EmployeeName, '') NameSA  
    , isnull(h.EmployeeName, '') NameForeman  
    , b.CreatedDate
    , d.Remarks
 from svTrnService a with(nolock, nowait)  
 left join svTrnSrvTask b with(nolock, nowait)  
   on b.CompanyCode = a.CompanyCode  
  and b.BranchCode  = a.BranchCode  
  and b.ProductType = a.ProductType  
  and b.ServiceNo   = a.ServiceNo  
 left join svTrnSrvMechanic c with(nolock, nowait)  
   on c.CompanyCode = b.CompanyCode  
  and c.BranchCode  = b.BranchCode  
  and c.ProductType = b.ProductType  
  and c.ServiceNo   = b.ServiceNo  
  and c.OperationNo = b.OperationNo  
 left join svTrnInvoice d with(nolock, nowait)  
   on d.CompanyCode = a.CompanyCode  
  and d.BranchCode  = a.BranchCode  
  and d.InvoiceNo   = a.InvoiceNo  
 left join svMstRefferenceService e with(nolock, nowait)  
   on e.CompanyCode    = a.CompanyCode  
  and e.RefferenceCode = a.jobType  
  and e.ProductType    = a.ProductType  
  and e.RefferenceType = 'JOBSTYPE'  
 left join svMstTask f with(nolock, nowait)  
   on f.CompanyCode = b.CompanyCode  
  and f.ProductType = b.ProductType  
  and f.OperationNo = b.OperationNo   
  and f.JobType     = a.JobType  
  and f.BasicModel  = a.BasicModel  
 left join gnMstEmployee g on g.CompanyCode = a.CompanyCode  
  and g.BranchCode = a.BranchCode   
  and g.EmployeeId = a.ForemanID  
 left join gnMstEmployee h on h.CompanyCode = a.CompanyCode  
  and h.BranchCode = a.BranchCode   
  and h.EmployeeId = a.MechanicID  
where a.JobOrderNo <> ''  
  and a.CompanyCode = @CompanyCode  
  and a.BranchCode  = @BranchCode  
  and a.ProductType = @ProductType  
  and a.ChassisCode = @ChassisCode  
  and a.ChassisNo   = @ChassisNo  
  and convert(varchar, a.JobOrderDate, 112) >= @JobOrderDate  
  and a.CustomerCode = @CustomerCode
) #t1  
  
--declare @t_spk_task as table(JobOrderNo varchar(20), OperationNo varchar(20))  
  
--insert into @t_spk_task   
--select a.JobOrderNo  
--  , isnull((  
--  select top 1 OperationNo from #t1  
--      where JobOrderNo = a.JobOrderNo  
--      order by CreatedDate   
--     ), '') OperationNo  
--  from #t1 a group by a.JobOrderNo  
  
insert into #t1  
select 1 TaskPartSeq  
     , a.BranchCode  
     , a.JobOrderNo  
     , a.JobOrderDate  
     , d.InvoiceNo  
     , d.InvoiceDate  
     , d.FPJNo  
     , d.FPJDate  
     , a.JobType + ' - ' + e.Description JobType  
     , a.Odometer  
     , ''  
     , ''  
     , b.PartNo  
     , (isnull(b.DemandQty,0) - isnull(b.ReturnQty,0)) as OperationQty  
     , convert(int, (isnull(b.DemandQty,0) - isnull(b.ReturnQty,0)) * isnull(b.RetailPrice,0) * 1.0) as OperationAmt  
     , convert(int, (isnull(b.DemandQty,0) - isnull(b.ReturnQty,0)) * isnull(b.RetailPrice,0) * 1.1)
     , 1  
     , f.PartName  
     , isnull(g.EmployeeName, '') NameSA  
     , isnull(h.EmployeeName, '') NameForeman  
     , b.CreatedDate
     , d.Remarks 
  from svTrnService a with(nolock, nowait)  
  left join svTrnSrvItem b with(nolock, nowait)  
    on b.CompanyCode = a.CompanyCode  
   and b.BranchCode  = a.BranchCode  
   and b.ProductType = a.ProductType  
   and b.ServiceNo   = a.ServiceNo  
  left join svTrnInvoice d with(nolock, nowait)  
    on d.CompanyCode = a.CompanyCode  
   and d.BranchCode  = a.BranchCode  
   and d.InvoiceNo   = a.InvoiceNo  
  left join svMstRefferenceService e with(nolock, nowait)  
    on e.CompanyCode    = a.CompanyCode  
   and e.RefferenceCode = a.jobType  
   and e.ProductType    = a.ProductType  
   and e.RefferenceType = 'JOBSTYPE'  
  left join spMstItemInfo f with(nolock, nowait)  
    on f.CompanyCode = b.CompanyCode  
   and f.PartNo      = b.PartNo   
  left join gnMstEmployee g on g.CompanyCode = a.CompanyCode  
   and g.BranchCode = a.BranchCode   
   and g.EmployeeId = a.ForemanID  
  left join gnMstEmployee h on h.CompanyCode = a.CompanyCode  
   and h.BranchCode = a.BranchCode   
   and h.EmployeeId = a.MechanicID  
 where a.JobOrderNo <> ''  
   and a.CompanyCode = @CompanyCode  
   and a.BranchCode  = @BranchCode  
   and a.ProductType = @ProductType  
   and a.ChassisCode = @ChassisCode  
   and a.ChassisNo   = @ChassisNo  
   and convert(varchar, a.JobOrderDate, 112) >= @JobOrderDate  
   and a.CustomerCode = @CustomerCode
;with x as (  
select a.JobOrderNo, a.OperationNo, a.TotalSrvAmount, b.TotalSrvAmount TotalSrvAmountNew
  from #t1 a  
  left join svTrnService b  
    on b.CompanyCode = @CompanyCode  
   and b.BranchCode  = @BranchCode  
   and b.ProductType = @ProductType  
   and b.JobOrderNo  = a.JobOrderNo  
)  
update x set TotalSrvAmount = TotalSrvAmountNew  

select a.BranchCode  
     , a.JobOrderNo  
     , SUM(a.OperationAmt)  TotalSrvAmount
     , (CAST(a.Remarks AS varchar(max)))
from #t1 a
where OperationQty > 0
group BY a.BranchCode, a.JobOrderNo, TotalSrvAmount, (CAST(a.Remarks AS varchar(max)))
order by a.JobOrderNo desc
  
drop table #t1  


GO


if object_id('uspfn_OmInquiryChassisReqMD') is not null
	drop procedure uspfn_OmInquiryChassisReqMD
GO
-- uspfn_OmInquiryChassisReq '6007402','600740200'
CREATE procedure [dbo].[uspfn_OmInquiryChassisReqMD]
	@CompanyCode as varchar(15)
	,@BranchCode as varchar(15)
	,@Penjual as varchar(15)
	,@CBU as bit
as

declare @isDirect as bit,
		@QRYTmp		AS varchar(max),
		@DBMD		AS varchar(25),
		@CompanyMD  AS varchar(25)

set @isDirect=0
if exists (
	select 1
	from gnMstCoProfile
	where CompanyCode=@CompanyCode and BranchCode=@Penjual
)
set @isDirect=1
set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 
set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 

set @QRYTmp =
'select * into #t1
from (
	select distinct isnull(b.BranchCode, e.BranchCode) BranchCode, isnull(c.CustomerCode, e.CustomerCode) CustomerCode
			,z.ChassisCode,z.BPKNo,z.SONo,e.DONo,convert(varchar,z.chassisNo) chassisNo, z.salesModelCode
			, z.salesModelYear, isnull(z.SuzukiDONo,'''') RefferenceDONo,
			isnull(z.SuzukiDODate,''19000101'') RefferenceDODate, isnull(z.SuzukiSJNo,'''') RefferenceSJNo, 
			isnull(z.SuzukiSJDate,''19000101'') RefferenceSJDate, 
			a.EndUserName, a.EndUserAddress1, a.EndUserAddress2, a.EndUserAddress3,
			c.CustomerName, c.Address1, c.Address2, c.Address3,
			c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = ''CITY'' AND ParaValue = c.CityCode) as CityName, 
			c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT Distinct EmployeeName FROM gnMstEmployee where EmployeeID = b.Salesman) SalesmanName, b.SalesType
	from ' + @DBMD + '.dbo.omMstVehicle z 
		left join omTrSalesSOVin a 
			on a.CompanyCode = ''' + @CompanyCode + ''' 
			and z.SONo=a.SONo
				AND a.ChassisCode = z.ChassisCode
				AND a.ChassisNo = z.ChassisNo
		left join omTrSalesSO b
			on a.companyCode = b.companyCode 
				and a.BranchCode= b.BranchCode
				and a.SONo = b.SONo
				and b.Status = ''2'' 
		left join gnMstCustomer c 
			on b.CompanyCode = c.CompanyCode
				and b.CustomerCode = c.CustomerCode 
		left join OmTrSalesDODetail d 
			on d.CompanyCode = z.CompanyCode and z.DONo=d.DONo
				and d.ChassisCode = z.ChassisCode		
				and d.ChassisNo = z.ChassisNo	
		left join OmTrSalesDO e 
			on e.CompanyCode = d.CompanyCode
				and e.DONo = d.DONo
				and e.BranchCode=d.BranchCode
				and e.Status = ''2''
		inner join omMstModel f
			on f.CompanyCode = ''' + @CompanyCode + '''
				and f.SalesModelCode = z.SalesModelCode
	where 
		z.CompanyCode = ''' + @CompanyMD + '''
		and z.ReqOutNo = ''''
		and z.status in (''3'',''4'',''5'',''6'')
		and not exists (select 1 from omTrSalesReqdetail where ChassisCode=z.ChassisCode and ChassisNo=z.ChassisNo)
		and ((case when z.ChassisNo is not null then z.SONo end) is not null 
			or (case when z.ChassisNo is not null then z.DONo end) is not null)
		and f.IsCBU = ' + CONVERT(VARCHAR, @CBU, 1) + '
) #t1

select * from #t1 

drop table #t1'

Exec (@QRYTmp);


--where ((case when ' + CONVERT(VARCHAR, @isDirect, 1) + ' = ''1'' then BranchCode end)= ''' + @Penjual + '''
--		or (case when ' + CONVERT(VARCHAR, @isDirect, 1) + ' <> ''1'' then BranchCode end)= ''' + @BranchCode + ''' )


GO
if object_id('uspfn_spCustSOPickListNewOrder') is not null
	drop procedure uspfn_spCustSOPickListNewOrder
GO

CREATE procedure [dbo].[uspfn_spCustSOPickListNewOrder]   
--DECLARE
@CompanyCode varchar(15),  
@BranchCode varchar(15),  
@ProfitCenterCode varchar(3),  
@TypeOfGoods varchar(2)  
as  
--SET @CompanyCode = '6006410'
--SET @BranchCode = '600641001'
----SET @ProfitCenterCode = '000' --
--SET @TypeOfGoods = '0'
SELECT x.CustomerCode,  
                    (  
                     SELECT c.CustomerName   
                     FROM gnMstCustomer c with(nolock, nowait)  
                     where  c.CompanyCode=x.CompanyCode  
                     AND c.CustomerCode= x.CustomerCode   
                     AND c.CustomerCode=x.CustomerCode  
                    ) AS CustomerName,  
                    (  
                     SELECT c.Address1+' '+c.Address2+' '+c.Address3+' '+c.Address4   
                     FROM gnMstCustomer c with(nolock, nowait)  
                     where  c.CompanyCode=x.CompanyCode  
                     AND c.CustomerCode= x.CustomerCode   
                     AND c.CustomerCode=x.CustomerCode  
  
                    ) as Address  
                    , z.LookUpValueName as ProfitCenter  
                    FROM   
                    (  
                     SELECT DISTINCT a.CompanyCode, a.BranchCode,  
                     b.CustomerCode   
                     FROM spTrnSOSupply a WITH(nolock, nowait) INNER JOIN   
                        spTrnSOrdHdr b ON a.CompanyCode=b.CompanyCode  
                     and a.BranchCode=b.BranchCode  
                     and a.DocNo=b.DocNo  
                        and b.TypeOfGoods=@TypeOfGoods  
                     WHERE pickingslipno = ''  
                     and a.Status=0  
                    ) x   
                    INNER JOIN gnMstCustomerProfitCenter y WITH(nolock, nowait)  
                    ON y.CompanyCode = x.CompanyCode   
                       AND y.BranchCode = x.BranchCode  
                       AND y.CustomerCode = x.CustomerCode  
                    INNER JOIN gnMstLookUpDtl z WITH(nolock, nowait)  
                    ON z.CompanyCode= x.CompanyCode  
                       AND z.CodeID='PFCN'  
                       AND z.LookupValue=y.ProfitCenterCode  
                    WHERE x.CompanyCode=@CompanyCode  
                        AND x.BranchCode=@BranchCode  
                       AND y.ProfitCenterCode=@ProfitCenterCode


GO


if object_id('uspfn_SvTrnInvInquiryBatch') is not null
	drop procedure uspfn_SvTrnInvInquiryBatch
GO

CREATE procedure [dbo].[uspfn_SvTrnInvInquiryBatch]
	@CompanyCode  varchar(20),
	@BranchCode   varchar(20),
	@GroupJobType varchar(20)
as   

--declare	@CompanyCode  varchar(20)
--declare	@BranchCode   varchar(20)
--declare	@GroupJobType varchar(20)

--set @CompanyCode  = '6159401000' 
--set @BranchCode   = '6159401001'
--set @GroupJobType = 'FSC'

if @GroupJobType = 'ALL'
begin
	select convert(bit, 0) as IsSelect, a.JobOrderNo, a.JobOrderDate, a.JobType
		 , a.LaborDppAmt as LaborAmt
		 , a.PartsDppAmt as PartAmt
		 , a.MaterialDppAmt as MaterialAmt
		 , a.TotalDppAmount as TotalDppAmt
		 , a.TotalPPnAmount as TotalPPnAmt
		 , a.TotalSrvAmount as TotalAmt
		 , convert(varchar, '') as Remarks
	  from SvTrnService a
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and a.ServiceStatus = '5'
end
else
if @GroupJobType = 'FSC'
begin
	select convert(bit, 0) as IsSelect, a.JobOrderNo, a.JobOrderDate, a.JobType
		 , a.LaborDppAmt as LaborAmt
		 , a.PartsDppAmt as PartAmt
		 , a.MaterialDppAmt as MaterialAmt
		 , a.TotalDppAmount as TotalDppAmt
		 , a.TotalPPnAmount as TotalPPnAmt
		 , a.TotalSrvAmount as TotalAmt
		 , convert(varchar, '') as Remarks
	  from SvTrnService a
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and a.ServiceStatus = '5'
	   and a.IsLocked = '1'
end
else
begin
	select convert(bit, 0) as IsSelect, a.JobOrderNo, a.JobOrderDate, a.JobType
		 , a.LaborDppAmt as LaborAmt
		 , a.PartsDppAmt as PartAmt
		 , a.MaterialDppAmt as MaterialAmt
		 , a.TotalDppAmount as TotalDppAmt
		 , a.TotalPPnAmount as TotalPPnAmt
		 , a.TotalSrvAmount as TotalAmt
		 , convert(varchar, '') as Remarks
	  from SvTrnService a
	  left join SvMstJob b on b.JobType = a.JobType
	   and b.CompanyCode = a.CompanyCode
	   and b.ProductType = a.ProductType
	   and b.BasicModel = a.BasicModel
	 where a.CompanyCode = @CompanyCode
	   and a.BranchCode = @BranchCode
	   and a.ServiceStatus = '5'
	   and b.GroupJobType = @GroupJobType
end



select a.JobOrderNo, a.InvoiceNo, a.InvoiceDate, a.JobType
	 , a.LaborDppAmt as LaborAmt
	 , a.PartsDppAmt as PartAmt
	 , a.MaterialDppAmt as MaterialAmt
	 , a.TotalDppAmt
	 , a.TotalPPnAmt
	 , a.TotalSrvAmt as TotalAmt
	 , a.Remarks
  from SvTrnInvoice a, GnMstCoProfileService b, ArInterface c
 where 1 = 1
   and c.CompanyCode = a.CompanyCode
   and c.BranchCode = a.BranchCode
   and c.DocNo = a.InvoiceNo
   and c.StatusFlag < 3
   and b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and a.CompanyCode = @CompanyCode
   and a.BranchCode = @BranchCode


GO

if object_id('uspfn_SvTrnMaintainInv') is not null
	drop procedure uspfn_SvTrnMaintainInv
GO

CREATE   procedure [dbo].[uspfn_SvTrnMaintainInv]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),	
	@ProfitCenterCode varchar(15),
	@PeriodeDate varchar(8)
	
as   
--exec uspfn_SvTrnMaintainInv 6159401000,6159401001,'4W',200,'2015-04-01 12:00:00'

SELECT   
 Invoice.CompanyCode, Invoice.BranchCode, Invoice.ProductType, Invoice.InvoiceNo  
 ,isnull(Invoice.InvoiceDate,'19000101') InvoiceDate  
 ,Invoice.InvoiceStatus, Invoice.FPJNo, FPJ.FPJGovNo  
 ,isnull(Invoice.FPJDate,'19000101') FPJDate  
 ,Invoice.JobOrderNo  
 ,isnull(Invoice.JobOrderDate,'19000101') JobOrderDate  
 ,Invoice.JobType, Invoice.ChassisCode, Invoice.ChassisNo, Invoice.EngineCode  
 ,Invoice.EngineNo, Invoice.PoliceRegNo, Invoice.BasicModel, Invoice.CustomerCode, Invoice.CustomerCodeBill  
 ,Invoice.Remarks, (Invoice.CustomerCode + ' - ' + Cust.CustomerName) as Customer  
 ,(Invoice.CustomerCodeBill + ' - ' + CustBill.CustomerName) as CustomerBill  
 , Invoice.Odometer, Invoice.LaborDiscPct, Invoice.PartsDiscPct, Invoice.MaterialDiscPct  
 , Invoice.LaborDppAmt, Invoice.PartsDppAmt, Invoice.MaterialDppAmt  
 , Invoice.TotalDppAmt, Invoice.TotalPpnAmt, Invoice.TotalSrvAmt  
   
 , vehicle.ServiceBookNo  
   
 , isnull(CustBill.CustomerName, '') CustomerName, isnull(CustBill.Address1, '') Address1, isnull(CustBill.Address2, '') Address2  
 , isnull(CustBill.Address3, '') Address3, isnull(CustBill.Address4, '') Address4, isnull(CustBill.PhoneNo, '') PhoneNo  
 , isnull(CustBill.HPNo, '') HPNo, isnull(CustBill.NPWPNo, '') NPWPNo, isnull(CustBill.NPWPDate,'19000101') NPWPDate, isnull(CustBill.SKPNo, '') SKPNo  
 , isnull(CustBill.SKPDate,'19000101') SKPDate, isnull(CustBill.CityCode, '') CityCode, isnull(CityCode.LookUpValueName, '') CityDesc  
   
 , isnull(CustProfCenter.TOPCode, '') TOPCode  
 , isnull(TOPCode.LookUpValueName, '') TOPDesc  
   
 , isnull(case AR.StatusFlag when '0' then 'Unposted'   
       when '3' then 'Posted'  
       else 'Unknown' end, 'Unknown') StatusAR, isnull(AR.DebetAmt, 0) DebetAmt, isnull(AR.CreditAmt, 0) CreditAmt  
    , isnull(AR.BlockAmt, 0) BlockAmt, isnull(AR.ReceiveAmt, 0) ReceiveAmt    
FROM svTrnInvoice Invoice  
--inner join GLInterface gl on gl.CompanyCode=Invoice.CompanyCode and gl.BranchCode=Invoice.BranchCode and gl.DocNo= Invoice.InvoiceNo  and gl.StatusFlag=0
--inner join ARInterface ar on ar.CompanyCode=Invoice.CompanyCode and ar.BranchCode=Invoice.BranchCode and  gl.DocNo= Invoice.InvoiceNo  and gl.StatusFlag=0
LEFT JOIN gnMstCustomer Cust  
    ON Cust.CompanyCode = Invoice.CompanyCode AND Cust.CustomerCode = Invoice.CustomerCode  
LEFT JOIN gnMstCustomer CustBill  
    ON CustBill.CompanyCode = Invoice.CompanyCode AND CustBill.CustomerCode = Invoice.CustomerCodeBill  
LEFT JOIN svMstcustomerVehicle vehicle   
 ON Invoice.CompanyCode = vehicle.CompanyCode and Invoice.ChassisCode = vehicle.ChassisCode and   
 Invoice.ChassisNo = vehicle.ChassisNo and Invoice.EngineCode = vehicle.EngineCode and   
 Invoice.EngineNo = vehicle.EngineNo and Invoice.BasicModel = vehicle.BasicModel   
LEFT JOIN svTrnFakturPajak FPJ   
 ON FPJ.CompanyCode = Invoice.CompanyCode  
 AND FPJ.BranchCode = Invoice.BranchCode  
 AND FPJ.FPJNo = Invoice.FPJNo  
LEFT JOIN gnMstCustomerProfitCenter CustProfCenter  
 ON CustProfCenter.CompanyCode = Invoice.CompanyCode  
 AND CustProfCenter.BranchCode = Invoice.BranchCode  
 AND CustProfCenter.CustomerCode = Invoice.CustomerCodeBill  
 AND CustProfCenter.ProfitCenterCode = '200'  
LEFT JOIN gnMstLookupDtl TOPCode  
 ON TOPCode.CompanyCode = Invoice.CompanyCode  
 AND TOPCode.CodeID = 'TOPC'  
 AND TOPCode.LookupValue = CustProfCenter.TOPCode  
LEFT JOIN gnMstLookupDtl CityCode  
 ON CityCode.CompanyCode = Invoice.CompanyCode  
 AND CityCode.CodeID = 'CITY'  
 AND CityCode.LookupValue = CustBill.CityCode  
LEFT JOIN ARInterface AR   
 ON AR.CompanyCode = Invoice.CompanyCode  
 AND AR.BranchCode = Invoice.BranchCode  
 AND AR.DocNo = Invoice.InvoiceNo    
 where 1=1
and Invoice.InvoiceNo in (select DocNo from GLInterface where 1=1
and companycode=@CompanyCode 
and BranchCode=@BranchCode
and ProfitCenterCode=@ProfitCenterCode
and StatusFlag=0)
and Invoice.InvoiceNo in (select DocNo from ARInterface where 1=1
and companycode=@CompanyCode
and BranchCode=@BranchCode
and ProfitCenterCode=@ProfitCenterCode
and StatusFlag=0)
and CONVERT(varchar,InvoiceDate,112) >=@PeriodeDate
and ProductType=@ProductType
order by InvoiceDate 




GO


if object_id('uspfn_spPickingSlipForPrint') is not null
	drop procedure uspfn_spPickingSlipForPrint
GO
-- =============================================
-- Author:		David Leonardo
-- Create date: 6 November 2014
-- Description:	Select Supply Slip for Print
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_spPickingSlipForPrint]
	-- Add the parameters for the stored procedure here
	@CompanyCode varchar(20),
	@BranchCode varchar(20),
	@ProductType varchar(5),
	@JobOrderNo varchar(50)
AS
BEGIN
	SELECT * INTO #t1 FROM (
                SELECT
                    DISTINCT c.DocNo, c.DocDate, d.PickingSlipNo, e.PartNo, e.PartNo PartNoOri, e.QtySupply, 
                    e.QtyPicked, e.QtyBill, d.Status, f.LookUpValueName TransTypeDesc, c.TransType, g.LmpNo,
                    d.PickedBy
                FROM
                    svTrnService a
                LEFT JOIN svTrnSrvItem b ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode 
	                AND b.ProductType = a.ProductType AND b.ServiceNo=a.ServiceNo
                LEFT JOIN spTrnSOrdHdr c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode 
	                AND c.DocNo = b.SupplySlipNo
                LEFT JOIN spTrnSPickingHdr d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode 
	                AND d.PickingSlipNo = c.ExPickingSlipNo
                LEFT JOIN spTrnSPickingDtl e ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode 
	                AND e.PickingSlipNo = d.PickingSlipNo
                LEFT JOIN gnMstLookUpDtl f ON f.CompanyCode = a.CompanyCode AND f.CodeId = 'TTSR' 
                    AND f.LookUpValue = c.TransType
                LEFT JOIN spTrnSLmpHdr g ON g.CompanyCode = a.CompanyCode AND g.BranchCode = a.BranchCode 
                    AND g.PickingSlipNo = d.PickingSlipNo
                WHERE 
                    a.CompanyCode     = @CompanyCode
                    AND a.BranchCode  = @BranchCode
                    AND a.ProductType = @ProductType
                    AND a.jobOrderNo  = @JobOrderNo
                    AND b.SupplySlipNo <> ''
                    AND b.PartSeq = (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode =  @CompanyCode AND BranchCode = @BranchCode
		                   AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = b.PartNo)
                    AND d.Status < 2
            )#t1
            
            select a.PickingSlipNo, a.TypeOfGoods, a.TransType, a.SalesType,
            (SELECT TOP 1 DocNo FROM SpTrnSPickingDtl 
                WHERE CompanyCode = a.CompanyCode 
                    AND BranchCode = a.BranchCode 
                    AND PickingSlipNo = a.PickingSlipNo) DocNo 
            from spTrnSpickingHdr a
            where  CompanyCode =  @CompanyCode
				AND BranchCode = @BranchCode
				AND a.pickingSlipNo IN 
                (SELECT DISTINCT PickingSlipNo FROM #t1)
				AND Salestype = 2
            DROP TABLE #t1
END

GO

if object_id('uspfn_GenerateSSPickingslipNew') is not null
	drop procedure uspfn_GenerateSSPickingslipNew
GO

CREATE PROCEDURE [dbo].[uspfn_GenerateSSPickingslipNew]
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@TransType		VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@DocDate		DATETIME
AS
BEGIN

--declare	@CompanyCode	VARCHAR(MAX)
--declare	@BranchCode		VARCHAR(MAX)
--declare	@JobOrderNo		VARCHAR(MAX)
--declare	@ProductType	VARCHAR(MAX)
--declare	@CustomerCode	VARCHAR(MAX)
--declare	@TransType		VARCHAR(MAX)
--declare	@UserID			VARCHAR(MAX)
--declare	@DocDate		DATETIME

--set	@CompanyCode	= '6156401000'
--set	@BranchCode		= '6156401001'
--set	@JobOrderNo		= 'SPK/15/001833'
--set	@ProductType	= '4W'
--set	@CustomerCode	= '000003'
--set	@TransType		= '20'
--set	@UserID			= 'TRAININGZZZ'
--set	@DocDate		= '3/12/2015 9:47:01 AM'


--exec uspfn_GenerateSSPickingslipNew '6006400001','6006400101','SPK/14/101589','4W','2105885','20','ga','3/2/2015 4:03:01 PM'
--================================================================================================================================
-- TABLE MASTER
--================================================================================================================================
-- Temporary for Item --
------------------------
SELECT * INTO #Item FROM (
SELECT a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.RetailPrice
	, a.PartNo
	, a.Billtype
	, SUM(ISNULL(a.DemandQty, 0) - (ISNULL(a.SupplyQty, 0))) QtyOrder
FROM svTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN svTrnService b ON b.CompanyCode = a.CompanyCode
	AND b.BranchCode = a.BranchCode
	AND b.ProductType = a.ProductType
	AND b.ServiceNo = a.ServiceNo
	AND b.JobOrderNo = @JobOrderNo
WHERE a.CompanyCode = @CompanyCode 
	AND a.BranchCode = @BranchCode 
	AND a.ProductType = @ProductType 
GROUP BY a.CompanyCode, a.BranchCode, a.ProductType
	, a.ServiceNo, a.PartNo, a.RetailPrice, a.BillType ) #Item 

DECLARE @CompanyMD AS VARCHAR(15)
DECLARE @BranchMD AS VARCHAR(15)
DECLARE @WarehouseMD AS VARCHAR(15)

SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

SELECT * INTO #SrvOrder FROM (
SELECT DISTINCT(a.CompanyCode) 
    , a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
    , (SELECT Paravalue FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 'GTGO' AND LookUpValue = a.TypeOfGoods) TipePart
    , (SELECT PartName FROM spMstItemInfo WHERE CompanyCode = a.CompanyCode AND PartNo = a.PartNo) PartName
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
	, SUM(a.QtyOrder) QtyOrder
    , 0 QtySupply
    , 0 QtyBO
    , (SUM(a.QtyOrder) * a.RetailPrice) * ((100 - a.PartDiscPct)/100) NetSalesAmt
    , a.PartDiscPct DiscPct
FROM
(
	SELECT
		DISTINCT(a.CompanyCode) 
		, a.BranchCode
		, a.ProductType
		, a.ServiceNo
		, a.PartNo
		, a.RetailPrice
		, a.CostPrice
		, a.TypeOfGoods
		, a.BillType
		, ISNULL(Item.QtyOrder,0) AS QtyOrder
		, a.DiscPct PartDiscPct 
	FROM
		svTrnSrvItem a WITH (NOLOCK, NOWAIT)
		LEFT JOIN svTrnService b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode	
			AND a.ProductType = b.ProductType
			AND a.ServiceNo = b.ServiceNo
		LEFT JOIN #Item Item ON Item.CompanyCode = a.CompanyCode 
			AND Item.BranchCode = a.BranchCode 
			AND Item.ProductType = a.ProductType 
			AND Item.ServiceNo = a.ServiceNo 
			AND Item.PartNo = a.PartNo 
			AND Item.RetailPrice = a.RetailPrice 
			AND Item.BillType = a.Billtype
		LEFT JOIN SpMstItemPrice c WITH (NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode 
			AND a.BranchCode = c.BranchCode 
			AND a.PartNo = c.PartNo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND a.ProductType = @ProductType
		AND Item.QtyOrder > 0
		AND JobOrderNo = @JobOrderNo
		AND (a.SupplySlipNo is null OR a.SupplySlipNo = '')
) a
GROUP BY
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.RetailPrice
	, a.CostPrice
    , a.TypeOfGoods
    , a.BillType
    , a.PartDiscPct 
) #SrvOrder

select * from #srvorder

--================================================================================================================================
-- INSERT TABLE SpTrnSORDHdr AND SpTrnSORDDtl
--================================================================================================================================
DECLARE @MaxDocNo			INT
DECLARE	@MaxPickingList		INT
DECLARE @TempDocNo			VARCHAR(MAX)
DECLARE @TempPickingList	VARCHAR(MAX)
DECLARE @TypeOfGoods		VARCHAR(MAX)
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

--===============================================================================================================================
-- LOOPING BASED ON THE TYPE OF GOODS
-- ==============================================================================================================================
DECLARE db_cursor CURSOR FOR
SELECT DISTINCT TypeOfGoods FROM #SrvOrder
WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND ProductType = @ProductType 

OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @TypeOfGoods

WHILE @@FETCH_STATUS = 0
BEGIN

--===============================================================================================================================
-- INSERT HEADER
-- ==============================================================================================================================
SET @MaxDocNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'SSS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempDocNo = ISNULL((SELECT 'SSS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxDocNo, 1), 6)),'SSS/YY/XXXXXX')

INSERT INTO SpTrnSORDHdr
([CompanyCode]
           ,[BranchCode]
           ,[DocNo]
           ,[DocDate]
           ,[UsageDocNo]
           ,[UsageDocDate]
           ,[CustomerCode]
           ,[CustomerCodeBill]
           ,[CustomerCodeShip]
           ,[isBO]
           ,[isSubstitution]
           ,[isIncludePPN]
           ,[TransType]
           ,[SalesType]
           ,[IsPORDD]
           ,[OrderNo]
           ,[OrderDate]
           ,[TOPCode]
           ,[TOPDays]
           ,[PaymentCode]
           ,[PaymentRefNo]
           ,[TotSalesQty]
           ,[TotSalesAmt]
           ,[TotDiscAmt]
           ,[TotDPPAmt]
           ,[TotPPNAmt]
           ,[TotFinalSalesAmt]
           ,[isPKP]
           ,[ExPickingSlipNo]
           ,[ExPickingSlipDate]
           ,[Status]
           ,[PrintSeq]
           ,[TypeOfGoods]
           ,[isDropsign]
           ,[CreatedBy]
           ,[CreatedDate]
           ,[LastUpdateBy]
           ,[LastUpdateDate]
           ,[isLocked]
           ,[LockingBy]
           ,[LockingDate])

SELECT 
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, @TempDocNo DocNo 
	, @DocDate DocDate
	, @JobOrderNo UsageDocNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) UsageDocDate
	, (SELECT CustomerCode FROM SvTrnService WHERE 1 = 1AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCode
	, (SELECT CustomerCodeBill FROM SvTrnService WHERE 1 = 1 AND CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeBill
	, (SELECT CustomerCode FROM SvTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode 
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo) CustomerCodeShip
	, CONVERT(BIT, 0) isBO
	, CONVERT(BIT, 0) isSubstitution
	, CONVERT(BIT, 1) isIncludePPN
	, @TransType TransType
	, '2' SalesType
	, CONVERT(BIT, 0) isPORDD
	, @JobOrderNo OrderNo
	, @DocDate OrderDate
	, ISNULL((SELECT TOPCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') TOPCode
	, ISNULL((SELECT ParaValue FROM GnMstLookUpDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND CodeID = 'TOPC' AND 
		LookupValue IN 
		(SELECT TOPCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)
	  ),0) TOPDays
	, ISNULL((SELECT PaymentCode FROM GnMstCustomerProfitCenter WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode),'W') PaymentCode
	, '' PaymentReffNo
	, 0 TotSalesQty
	, 0 TotSalesAmt
	, 0 TotDiscAmt
	, 0 TotDPPAmt
	, 0 TotPPNAmt
	, 0 TotFinalSalesAmt
	, CONVERT(BIT, 0) isPKP
	, NULL ExPickingSlipNo
	, NULL ExPickingSlipDate
	, '4' Status
	, 0 PrintSeq
	, @TypeOfGoods TypeOfGoods
	, NULL IsDropSign
	, @UserID CreatedBY
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate


UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'SSS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT DETAIL
-- ==============================================================================================================================
DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

declare @TempAvailStock as table
(
	PartNo varchar(50),
	AvailStock decimal
)

DECLARE @Query AS VARCHAR(MAX)
--SET @Query = 
--'SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
--FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
--WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+''

--INSERT INTO #TempAvailStock

SET @Query = 
'Select * into #TempAvailStock from (SELECT PartNo, (Onhand - AllocationSP - AllocationSL - AllocationSR - ReservedSP - ReservedSL - ReservedSR) AvailStock
FROM ' + @DbMD + '..SpMstItemLoc WITH (NOLOCK, NOWAIT) 
WHERE CompanyCode = '+''''+@CompanyMD+''''+' AND BranchCode ='+''''+@BranchMD +''''+' AND WarehouseCode = '+''''+@WarehouseMD+''''+')#TempAvailStock

INSERT INTO SpTrnSORDDtl 
(
	[CompanyCode] ,
	[BranchCode] ,
	[DocNo] ,
	[PartNo] ,
	[WarehouseCode] ,
	[PartNoOriginal] ,
	[ReferenceNo] ,
	[ReferenceDate] ,
	[LocationCode] ,
	[QtyOrder] ,
	[QtySupply] ,
	[QtyBO] ,
	[QtyBOSupply] ,
	[QtyBOCancel] ,
	[QtyBill] ,
	[RetailPriceInclTax] ,
	[RetailPrice] ,
	[CostPrice] ,
	[DiscPct] ,
	[SalesAmt] ,
	[DiscAmt] ,
	[NetSalesAmt] ,
	[PPNAmt] ,
	[TotSalesAmt] ,
	[MovingCode] ,
	[ABCClass] ,
	[ProductType] ,
	[PartCategory] ,
	[Status] ,
	[CreatedBy] ,
	[CreatedDate] ,
	[LastUpdateBy] ,
	[LastUpdateDate] ,
	[StockAllocatedBy] ,
	[StockAllocatedDate] ,
	[FirstDemandQty] )
SELECT
	''' + @CompanyCode +''' CompanyCode
	, ''' + @BranchCode +''' BranchCode
	, ''' + @TempDocNo +''' DocNo 
	, a.PartNo
	, ''00'' WarehouseCode
	, a.PartNo PartNoOriginal
	, ''' + @JobOrderNo +''' ReferenceNo
	, (SELECT JobOrderDate FROM SvTrnService WHERE 1 =1 AND CompanyCode = ''' + @CompanyCode +''' AND BranchCode = ''' + @BranchCode +'''
		AND ProductType = ''' + @ProductType +''' AND JobOrderNo = ''' + @JobOrderNo +''' ) ReferenceDate
	, (SELECT distinct LocationCode FROM ' + @DbMD +'..SpMstItemLoc WHERE CompanyCode = ''' + @CompanyMD +''' AND BranchCode = ''' + @BranchMD +''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo ) LocationCode
	, a.QtyOrder
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBO
	, 0 QtyBOSupply
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN 0 
		ELSE a.QtyOrder - ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtyBOCancel
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice * 10 /100) RetailPriceIncltax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder * a.RetailPrice
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice
		END AS SalesAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice) * a.DiscPct/100)
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) * a.DiscPct/100)
		END AS DiscAmt
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS NetSalesAmt
	, 0 PPNAmt
	,  CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN floor((a.QtyOrder * a.RetailPrice)- ((a.QtyOrder * a.RetailPrice) * a.DiscPct/100))
		ELSE floor((ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice) - 
			(ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) * a.RetailPrice * a.DiscPct/100))
		END AS TotSalesAmt
	, (SELECT distinct MovingCode FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +''' ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +'''  AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''2'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
	, a.QtyOrder FirstDemandQty
FROM #SrvOrder a
WHERE a.TypeOfGoods = '+@TypeOfGoods +'


select top 10 * from spTrnSORDDtl order by createddate desc
--===============================================================================================================================
-- INSERT SO SUPPLY
-- ==============================================================================================================================

SELECT * INTO #TempSOSupply FROM (
SELECT
	'''+ @CompanyCode +''' CompanyCode
	, '''+ @BranchCode +''' BranchCode
	, '''+ @TempDocNo +''' DocNo 
	, 0 SupSeq
	, a.PartNo 
	, a.PartNo PartNoOriginal
	, '''' PickingSlipNo	
	, '''+ @JobOrderNo +''' ReferenceNo
	, '''+ CONVERT(varchar, @DefaultDate )+''' ReferenceDate
	, ''00'' WarehouseCode
	, (SELECT distinct LocationCode FROM '+ @DbMD+'..SpMstItemLoc WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD+''' AND WarehouseCode = ''00''
		AND PartNo = a.PartNo) LocationCode
	, CASE WHEN 
		ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0) >= a.QtyOrder 
		THEN a.QtyOrder 
		ELSE ISNULL((SELECT distinct AvailStock FROM #TempAvailStock WHERE PartNo = a.PartNo),0)
		END AS QtySupply
	, 0 QtyPicked
	, 0 QtyBill
	, a.RetailPrice + FLOOR(a.RetailPrice *10 /100) RetailPriceIncltax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, (SELECT distinct MovingCode FROM '+ @DbMD+'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		MovingCode
	, (SELECT distinct ABCClass FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		ABCClass
	, '''+ @ProductType +'''ProductType
	, (SELECT distinct PartCategory FROM '+ @DbMD +'..SpMstItems WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyMD +''' AND BranchCode = '''+ @BranchMD +''' AND PartNo = a.PartNo) 
		PartCategory
	, ''1'' Status
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, '''+ @UserID +''' StockAllocatedBy
	, '''+ Convert(varchar,GetDate()) +''' StockAllocatedDate
FROM #SrvOrder a
--inner join spMstItemPrice b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.PartNo = b.PartNo
WHERE a.TypeOfGoods = '+ @TypeOfGoods +'
)#TempSOSupply

INSERT INTO SpTrnSOSupply SELECT 
	CompanyCode,BranchCode,DocNo,SupSeq,PartNo,PartNoOriginal
	, ROW_NUMBER() OVER(ORDER BY PartNo) PTSeq,PickingSlipNo
	, ReferenceNo,ReferenceDate,WarehouseCode,LocationCode
	, QtySupply,QtyPicked,QtyBill,RetailPriceIncltax,RetailPrice,CostPrice
	, DiscPct,MovingCode,ABCClass,ProductType,PartCategory,Status
	, CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate
FROM #TempSOSupply WHERE QtySupply > 0

--===============================================================================================================================
-- UPDATE STATUS DETAIL BASED ON SUPPLY
--===============================================================================================================================

UPDATE SpTrnSORDDtl
SET Status = 4
FROM SpTrnSORDDtl a, #TempSOSupply b
WHERE 1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PartNo = b.PartNo
	
--===============================================================================================================================
-- UPDATE HISTORY DEMAND ITEM AND CUSTOMER
--===============================================================================================================================

UPDATE SpHstDemandItem 
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +'''
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandItem a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+ @TempDocNo +'''

UPDATE SpHstDemandCust
SET DemandFreq = DemandFreq + 1
	, DemandQty = DemandQty + b.QtyOrder
	, LastUpdateBy = '''+ @UserID +''' 
	, LastUpdateDate = '''+ Convert(varchar,GetDate()) +''' 
FROM SpHstDemandCust a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = '''+ Convert(varchar,Year(GetDate())) +'''
	AND a.Month  = '''+ Convert(varchar,Month(GetDate())) +'''
	AND a.PartNo = b.PartNo
	AND a.CustomerCode = '''+ @CustomerCode +'''
	AND b.DocNo = '''+ @TempDocNo +'''

INSERT INTO SpHstDemandItem
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +''' Year
	, '''+ Convert(varchar,Month(GetDate())) +''' Month
	, PartNo
	, 1 DemandFreq
	, QtyOrder DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' AND a.BranchCode = '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
 AND NOT EXISTS
( SELECT 1 FROM SpHstDemandItem WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +''' 
	AND PartNo = a.PartNo
)

INSERT INTO SpHstDemandCust
SELECT 
	CompanyCode
	, BranchCode
	, '''+ Convert(varchar,Year(GetDate())) +'''  Year
	, '''+ Convert(varchar,Month(GetDate())) +'''  Month
	, '''+ @CustomerCode +''' CustomerCode
	, PartNo
	, 1 DemandFreq
	, (SELECT QtyOrder FROM SpTrnSORDDTl WITH (NOLOCK, NOWAIT) 
		WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
		AND DocNo = a.DocNo AND PartNo = a.PartNo) DemandQty
	, 0 SalesFreq
	, 0 SalesQty
	, MovingCode
	, ProductType
	, PartCategory
	, ABCClass
	, '''+ @UserID +''' LastUpdateBy
	, '''+ CONVERT(varchar, GetDate()) +''' LastUpdateDate
FROM SpTrnSordDtl a WITH (NOLOCK, NOWAIT)
WHERE a.CompanyCode= '''+ @CompanyCode +''' and a.BranchCode= '''+ @BranchCode +''' AND a.DocNo = '''+ @TempDocNo +''' -- add CompanyCode and BranchCode 13 Des 2010
AND NOT EXISTS
( SELECT PartNo FROM SpHstDemandCust WITH (NOLOCK, NOWAIT) WHERE 
	1 = 1 
	AND CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode
	AND Month = '''+ Convert(varchar,Month(GetDate())) +''' 
	AND Year = '''+ Convert(varchar,Year(GetDate())) +'''  
	AND PartNo = a.PartNo
)

--===============================================================================================================================
-- UPDATE LAST DEMAND DATE MASTER
--===============================================================================================================================

UPDATE '+@DbMD+'..SpMstItems 
SET LastDemandDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, SpTrnSordDtl b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo
	AND b.DocNo = '''+@TempDocNo+'''

--===============================================================================================================================
-- UPDATE STOCK AND MOVEMENT
--===============================================================================================================================

UPDATE '+@DbMD+'..spMstItems
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItems a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD+'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.PartNo = b.PartNo

UPDATE '+@DbMD+'..spMstItemloc
SET AllocationSR = AllocationSR + b.QtySupply
	, LastUpdateBy = '''+@UserID+'''
	, LastUpdateDate = '''+Convert(varchar,GetDate())+'''
FROM '+@DbMD+'..SpMstItemLoc a, #TempSOSupply b
WHERE 
	1 = 1
	AND a.CompanyCode = '''+@CompanyMD +'''
	AND a.BranchCode = '''+@BranchMD+'''
	AND a.WarehouseCode = '''+@WarehouseMD+'''
	AND a.PartNo = b.PartNo

INSERT INTO SpTrnIMovement
SELECT
	'''+@CompanyCode +''' CompanyCode
	, '''+@BranchCode +''' BranchCode
	, a.DocNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = '''+ @CompanyCode +'''
		AND BranchCode = '''+ @BranchCode +''' AND DocNo = a.DocNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),'''+convert(varchar,getdate())+''') CreatedDate 
	, ''00'' WarehouseCode
	, (SELECT LocationCode FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT) WHERE CompanyCode =  '''+@CompanyCode +'''
		AND BranchCode = '''+@BranchCode +''' AND DocNo = '''+@TempDocNo +''' AND PartNo = a.PartNo)
	  LocationCode
	, a.PartNo
	, ''OUT'' SignCode
	, ''SA-NPJUAL'' SubSignCode
	, a.QtySupply
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, '''+@UserID +''' CreatedBy
FROM #TempSOSupply a

--===============================================================================================================================
-- UPDATE SUPPLY SLIP TO SPK
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = '''+@CompanyCode +''' AND BranchCode = '''+@BranchCode+'''
		AND ProductType = '''+@ProductType +''' AND JobOrderNo = '''+@JobOrderNo+''')

SELECT * INTO #TempServiceItem FROM (
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1
	AND a.CompanyCode = '''+@CompanyCode+'''
	AND a.BranchCode = '''+@BranchCode+'''
	AND a.ProductType = '''+@ProductType+'''
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+@ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND (a.SupplySlipNo = '''' OR a.SupplySlipNo IS NULL)
) #TempServiceItem 

SELECT * INTO #TempServiceItemIns FROM( 
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtySupply
	, b.DocNo
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN #TempSOSupply b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo
WHERE
	1 = 1 
	AND a.CompanyCode = '''+ @CompanyCode +''' 
	AND a.BranchCode = '''+ @BranchCode +''' 
	AND a.ProductType = '''+ @ProductType +'''  
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = '''+ @ProductType +''' AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo) 
	AND (a.SupplySlipNo != '''' OR a.SupplySlipNo IS NOT NULL)
) #TempServiceItemIns


UPDATE svTrnSrvItem
SET SupplySlipNo = b.DocNo
	, SupplySlipDate = ISNULL((SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
							AND DocNo = b.DocNo),'''+Convert(varchar,@DefaultDate)+''')
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq
	
--===============================================================================================================================
-- INSERT NEW SRV ITEM BASED SUPPLY SLIP
--===============================================================================================================================
INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq + 1
	, 0 DemandQty
	, 0 SupplyQty
	, 0 ReturnQty
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DocNo SupplySlipNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND DocNo = a.DocNo) SupplySlipDate
	, NULL SSReturnNo
	, NULL SSReturnDate
	, '''+ @UserID +''' CreatedBy
	, '''+ Convert(varchar,GetDate()) +''' CreatedDate
	, '''+ @UserID +''' LastUpdateBy
	, '''+ Convert(varchar,GetDate()) +''' LastUpdateDate
	, a.DiscPct
FROM #TempServiceItemIns a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = '''+ @CompanyCode +'''
	AND a.BranchCode = '''+ @BranchCode +'''
	AND a.ProductType = '''+ @ProductType+'''


--===============================================================================================================================
DROP TABLE #TempServiceItem 
DROP TABLE #TempServiceItemIns
DROP TABLE #TempSOSupply'

EXEC(@query)

--select convert(xml,@query)


--===============================================================================================================================
-- INSERT SVSDMOVEMENT
--===============================================================================================================================

declare @md bit
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

if(@md = 0)
begin

 declare @QueryTemp as varchar(max)  
 
	set @Query ='insert into '+ @DbMD +'..svSDMovement
	select a.CompanyCode, a.BranchCode, a.DocNo, a.CreatedDate, a.PartNo
	, Seq = convert(integer, ROW_NUMBER() OVER (PARTITION BY a.ReferenceNo ORDER BY a.DocNo)) ,
	a.WarehouseCode, a.QtyOrder, a.QtySupply, a.DiscPct
	--,isnull(((select RetailPrice from spTrnSORDDtl
	--		where CompanyCode = ''' + @CompanyCode + '''  and BranchCode = ''' + @BranchCode  + '''
	--		and DocNo = ''' + @TempDocNo + ''' and PartNo = a.PartNo) / 1.1 * 
	--		((100 - isnull((select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
	--			where CompanyCode = ''' + @CompanyCode + ''' and BranchCode = ''' + @BranchCode  + ''' and SupplierCode = dbo.GetBranchMD(''' + @CompanyCode + ''', ''' + @BranchCode  + ''') 
	--			and ProfitCenterCode = ''300''),0)) * 0.01)),0) CostPrice
	, a.CostPrice
	, a.RetailPrice, b.TypeOfGoods
	, '''+ @CompanyMD +''','''+ @BranchMD +''','''+ @WarehouseMD +''', p.RetailPriceInclTax, p.RetailPrice, p.CostPrice
	,''x'','''+ @producttype +''',''300'',''0'',''0'','''+ convert(varchar,GETDATE()) +''','''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	,'''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
	from spTrnSORDDtl a 
	join spTrnSORDHdr b on a.CompanyCode = b.CompanyCode
	and a.BranchCode = b.BranchCode 
	and a.DocNo = b.DocNo
	join spmstitemprice p
	on p.PartNo = a.PartNo
	where p.CompanyCode = '''+ @CompanyCode +'''
	and p.branchcode = '''+ @BranchCode +'''
	and a.ReferenceNo = '''+ @JobOrderNo +''''+
	' and a.DocNo = ''' + @TempDocNo + '''';

	exec (@Query)
	--print (@QUERY)

end

--===============================================================================================================================
-- INSERT PICKING HEADER AND DETAIL
--===============================================================================================================================

SET @MaxPickingList = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'PLS' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempPickingList = ISNULL((SELECT 'PLS/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxPickingList, 1), 6)),'PLS/YY/XXXXXX')

INSERT INTO SpTrnSPickingHdr 
SELECT 
	CompanyCode
	, BranchCode
	, @TempPickingList PickingSlipNo
	, GetDate() PickingSlipDate
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, '' PickedBy
	, CONVERT(BIT, 0) isBORelease
	, isSubstitution
	, isIncludePPN
	, TransType
	, SalesType
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, '' Remark
	, '0' Status
	, '0' PrintSeq
	, TypeOfGoods
	, CreatedBy
	, CreatedDate
	, LastUpdateBy
	, LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = (SELECT distinct DocNo FROM spTrnSORDDtl WHERE CompanyCode = @CompanyCode AND Branchcode = @BranchCode 
					AND DocNo = @TempDocNo AND QtySupply > 0)		

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'PLS'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

-- ==============================================================================================================================
-- UPDATE SALES ORDER HEADER 
-- ==============================================================================================================================
UPDATE SpTrnSORDHdr
	SET ExPickingSlipNo = @TempPickingList,
		ExPickingSlipDate = ISNULL((SELECT PickingSlipDate FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode AND PickingSlipNo = @TempPickingList),'')
	
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo

UPDATE SpTrnSOSupply
	SET PickingSlipNo = @TempPickingList
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
-- ==============================================================================================================================
-- INSERT PICKING DETAIL
-- ==============================================================================================================================

INSERT INTO SpTrnSPickingDtl
SELECT 
	a.CompanyCode
	, a.BranchCode
	, @TempPickingList PickingSlipNo
	, '00' WarehouseCode
	, a.PartNo
	, a.PartNoOriginal
	, a.DocNo
	, b.DocDate 
	, a.ReferenceNo
	, a.ReferenceDate
	, a.LocationCode
	, a.QtySupply QtyOrder
	, a.QtySupply
	, a.QtySupply QtyPicked 
	, 0 QtyBill
	, a.RetailPriceInclTax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, a.SalesAmt
	, a.DiscAmt
	, a.NetSalesAmt
	, a.TotSalesAmt
	, a.MovingCode
	, a.ABCClass
	, a.ProductType
	, a.PartCategory
	, '' ExPickingSlipNo
	, @DefaultDate ExPickingSlipDate
	, CONVERT(BIT, 0) isClosed
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSORDDtl a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSORDHdr b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.DocNo = b.DocNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.DocNo = @TempDocNo
	AND a.QtySupply > 0


--================================================================================================================================
-- UPDATE AMOUNT HEADER
--================================================================================================================================
SELECT * INTO #TempHeader FROM (
SELECT 
	header.CompanyCode
	, header.BranchCode
	, header.DocNo
	, header.TotSalesQty
	, header.TotSalesAmt
	, header.TotDiscAmt
	, header.TotDPPAmt
	, floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100)) 
		TotPPNAmt
	, header.TotDPPAmt + floor(header.TotDPPAmt * (ISNULL((SELECT TaxPct FROM GnMstTax WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND TaxCode IN (SELECT TaxCode FROM GnMstCustomerProfitCenter WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
			AND ProfitCenterCode = '300' AND CustomerCode = @CustomerCode)),0)/100))
		TotFinalSalesAmt
FROM (
SELECT 
	CompanyCode
	, BranchCode
	, DocNo
	, SUM(QtyOrder) TotSalesQty
	, SUM(SalesAmt) TotSalesAmt
	, SUM(DiscAmt) TotDiscAmt
	, SUM(NetSalesAmt) TotDPPAmt
FROM SpTrnSORDDtl WITH (NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
	AND BranchCode = @BranchCode
	AND DocNo = @TempDocNo
GROUP BY CompanyCode
	, BranchCode
	, DocNo
) header ) #TempHeader

UPDATE SpTrnSORDHdr
SET 
	TotSalesQty = b.TotSalesQty
	, TotSalesAmt = b.TotSalesAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalSalesAmt = b.TotFinalSalesAmt
FROM SpTrnSORDHdr a, #TempHeader b
WHERE 
	1 = 1
	AND a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode
	AND a.DocNo = b.DocNo

DROP TABLE #TempHeader

FETCH NEXT FROM db_cursor INTO @TypeOfGoods
END
CLOSE db_cursor
DEALLOCATE db_cursor 

--===============================================================================================================================
-- Update Transdate
--===============================================================================================================================

update gnMstCoProfileSpare
set TransDate=getdate()
	, LastUpdateBy=@UserID
	, LastUpdateDate=getdate()
where CompanyCode = @CompanyCode AND BranchCode = @BranchCode

--===============================================================================================================================
-- DROP TABLE SECTION 
--===============================================================================================================================
DROP TABLE #SrvOrder
DROP TABLE #Item

--rollback tran
END

GO

if object_id('sp_EdpPartNo_Pembelian') is not null
	drop procedure sp_EdpPartNo_Pembelian
GO
CREATE procedure [dbo].[sp_EdpPartNo_Pembelian]  (  
@CompanyCode varchar(10),
@BranchCode varchar(10),
@DocNo varchar(20),
@bPPN bit
--@BinningNo varchar(20),
--@Opt varchar(10)
)
 as 
 
if @bPPN = 1
begin
SELECT c.PartNo, c.PartName, c.PurchasePrice, c.DiscPct, SUM(c.OnOrder) AS MaxReceived
FROM
(
    SELECT
     a.PartNo
    ,b.PartName 
    ,ISNULL((SELECT round(x.PurchasePrice + (x.PurchasePrice * (select (TaxPct/100) from gnMstTax
where TaxCode = (select ParaValue from gnMstLookUpDtl where CodeID = 'BINS' and SeqNo = 3))),0)
FROM SpMstItemPrice x where x.CompanyCode = a.CompanyCode AND
     x.BranchCode = a.BranchCode AND x.PartNo = a.PartNo),0) AS PurchasePrice
    ,a.OnOrder
    ,a.DiscPct
    FROM spTrnPOrderBalance a 
    INNER JOIN spMstItemInfo b
       ON b.CompanyCode = a.CompanyCode
      AND b.PartNo      = a.PartNo
      WHERE a.CompanyCode = @CompanyCode
      AND a.BranchCode  = @BranchCode
      AND a.PosNo       = @DocNo      
) c
GROUP BY c.PartNo, c.PartName, c.PurchasePrice, c.DiscPct
HAVING sum(c.OnOrder) > 0
end
else
begin
SELECT c.PartNo, c.PartName, c.PurchasePrice,  c.DiscPct, SUM(c.OnOrder) AS MaxReceived
FROM
(
    SELECT
     a.PartNo
    ,b.PartName 
    ,ISNULL((SELECT x.PurchasePrice FROM SpMstItemPrice x where x.CompanyCode = a.CompanyCode AND
     x.BranchCode = a.BranchCode AND x.PartNo = a.PartNo),0) AS PurchasePrice
    ,a.OnOrder
    ,a.DiscPct
    FROM spTrnPOrderBalance a 
    INNER JOIN spMstItemInfo b
       ON b.CompanyCode = a.CompanyCode
      AND b.PartNo      = a.PartNo
    WHERE a.CompanyCode = @CompanyCode
      AND a.BranchCode  = @BranchCode
      AND a.PosNo       = @DocNo
) c
GROUP BY c.PartNo, c.PartName, c.PurchasePrice, c.DiscPct
HAVING sum(c.OnOrder) > 0
end

GO

if object_id('uspfn_spMasterPartLocationLookup') is not null
	drop procedure uspfn_spMasterPartLocationLookup
GO
CREATE PROCEDURE [dbo].[uspfn_spMasterPartLocationLookup]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@TypeOfGoods varchar(15),
	@ProductType varchar(15)
AS
SELECT 
	ItemInfo.PartNo,
	Items.ABCClass,
	ItemLoc.OnHand - itemLoc.ReservedSP - itemLoc.ReservedSR - itemLoc.ReservedSL - itemLoc.AllocationSP - itemLoc.AllocationSL - itemLoc.AllocationSR AS AvailQty,
	Items.OnOrder,
	Items.ReservedSP,
	Items.ReservedSR,
	Items.ReservedSL,
	Items.MovingCode,
	ItemInfo.SupplierCode,
	ItemInfo.PartName,
	ItemPrice.RetailPrice,
	ItemPrice.RetailPriceInclTax,
	ItemPrice.PurchasePrice
	FROM SpMstItems Items
	INNER JOIN SpMstItemInfo ItemInfo ON Items.CompanyCode  = ItemInfo.CompanyCode                          
							 AND Items.PartNo = ItemInfo.PartNo
	INNER JOIN spMstItemPrice ItemPrice ON Items.CompanyCode = ItemPrice.CompanyCode
							AND Items.BranchCode = ItemPrice.BranchCode AND Items.PartNo = ItemPrice.PartNo
	INNER JOIN spMstItemLoc ItemLoc ON Items.CompanyCode = ItemLoc.CompanyCode AND Items.BranchCode = ItemLoc.BranchCode
							AND Items.PartNo = ItemLoc.PartNo
	WHERE Items.CompanyCode = @CompanyCode
	  AND Items.BranchCode  = @BranchCode    
	  AND Items.TypeOfGoods = @TypeOfGoods
	  AND Items.ProductType = @ProductType
	  AND Items.Status > 0
	  AND ItemLoc.WarehouseCode = '00'
	 ORDER BY ItemInfo.PartNo

GO

if object_id('uspfn_spMasterPartLookupNew') is not null
	drop procedure uspfn_spMasterPartLookupNew
GO

CREATE PROCEDURE [dbo].[uspfn_spMasterPartLookupNew]
	@CompanyCode varchar(15),
	@TypeOfGoods varchar(15),
	@ProductType varchar(15)
AS
SELECT * FROM (
SELECT DISTINCT ItemInfo.PartNo
,ItemInfo.PartName
,ItemInfo.CompanyCode
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
,ItemInfo.ProductType
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'PRCT' AND 
        LookUpValue = ItemInfo.PartCategory AND 
        CompanyCode = @CompanyCode) AS CategoryName
,ItemInfo.PartCategory
,ItemInfo.OrderUnit
,ItemInfo.SupplierCode
,Supplier.SupplierName
,(SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'TPGO' AND 
        LookUpValue = Items.TypeOfGoods AND 
        CompanyCode = @CompanyCode) AS TypeOfGoods
, (SELECT LookupValueName 
    FROM gnMstLookupDtl 
  WHERE CodeID = 'STPR' AND 
        LookUpValue = Items.Status AND 
        CompanyCode = @CompanyCode) AS Status
FROM SpMstItemInfo ItemInfo
LEFT JOIN SpMstItems Items    ON ItemInfo.CompanyCode = Items.CompanyCode AND ItemInfo.PartNo = Items.PartNo
LEFT JOIN GnMstSupplier Supplier ON Supplier.CompanyCode = ItemInfo.CompanyCode AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE ItemInfo.CompanyCode = @CompanyCode AND ItemInfo.ProductType = @ProductType AND Items.TypeOfGoods = @TypeOfGoods
) ItemInfo WHERE ItemInfo.CompanyCode = @CompanyCode

GO

if object_id('uspfn_GenerateBPSLampiranNew') is not null
	drop procedure uspfn_GenerateBPSLampiranNew
GO

CREATE procedure [dbo].[uspfn_GenerateBPSLampiranNew] 
(
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@JobOrderNo		VARCHAR(MAX),
	@ProductType	VARCHAR(MAX),
	@CustomerCode	VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@PickedBy		VARCHAR(MAX)
)
AS
BEGIN

/*
PSEUDOCODE PROCESS :
Line 38  : RE-CALCULATE AMOUNT DETAIL
Line 93  : RE-CALCULATE AMOUNT HEADER AND UPDATE STATUS
Line 140 : UPDATE SO SUPPLY AND STATUS HEADER 
Line 167 : UPDATE SUPPLY SLIP QTY SERVICE 
Line 237 : INSERT NEW SRV ITEM BASED PICKING LIST
Line 276 : INSERT BPS AND LAMPIRAN
Line 292 : INSERT BPS HEADER
Line 352 : INSERT BPS DETAIL
Line 395 : INSERT LAMPIRAN HEADER
Line 458 : INSERT LAMPIRAN DETAIL
Line 500 : UPDATE STOCK
Line 571 : UPDATE DEMAND CUST AND DEMAND ITEM
Line 611 : INSERT TO ITEM MOVEMENT
Line 650 : UPDATE TRANSDATE
*/

--DECLARE	@CompanyCode	VARCHAR(MAX),
--		@BranchCode		VARCHAR(MAX),
--		@JobOrderNo		VARCHAR(MAX),
--		@ProductType	VARCHAR(MAX),
--		@CustomerCode	VARCHAR(MAX),
--		@UserID			VARCHAR(MAX),
--		@PickedBy		VARCHAR(MAX)

--SET	@CompanyCode	= '6156401000'
--SET	@BranchCode		= '6156401001'
--SET	@JobOrderNo		= 'SPK/15/001666'
--SET	@ProductType	= '4W'
--SET	@CustomerCode	= '0032710'
--SET	@UserID			= 'ga'
--SET	@PickedBy		= '0004'
		
--exec uspfn_GenerateBPSLampiranNew '6006400001','6006400101','SPK/14/101625','4W','JKT-1852626','ga','00001'

--===============================================================================================================================
-- RE-CALCULATE AMOUNT DETAIL
--===============================================================================================================================
DECLARE @DefaultDate		DATETIME

SET @DefaultDate = '1900-01-01 00:00:00.000'

if object_id('#tmpSvSDMovement') is not null drop table #tmpSvSDMovement

DECLARE @DbMD AS VARCHAR(15)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

  declare @PurchaseDisc as decimal
  set @PurchaseDisc = (select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
       where CompanyCode = @CompanyCode   
       and BranchCode = @BranchCode  
       and SupplierCode = dbo.GetBranchMD(@CompanyCode,@BranchCode)
       and ProfitCenterCode = '300')  
         
   if (@PurchaseDisc = 0) raiserror ('Purchase Discount belum di-setting untuk Part tersebut!',16,1);   

SELECT * INTO #TempPickingSlipDtl FROM (
SELECT
	a.CompanyCode
	, a.BranchCode 
	, a.PickingSlipNo
	, a.PickingSlipDate
	, a.CustomerCode
	, a.TypeOfGoods
	, b.DocNo
	, b.PartNo
	, b.QtyPicked
	, b.QtyPicked * b.RetailPrice SalesAmt
	, Floor((b.QtyPicked * b.RetailPrice) * DiscPct/100) DiscAmt
	, (b.QtyPicked * b.RetailPrice) - Floor((b.QtyPicked * b.RetailPrice) * DiscPct/100) NetSalesAmt
FROM SpTrnSPickingHdr a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode 
	AND a.BranchCode = b.BranchCode AND a.PickingSlipNo = b.PickingSlipNo
WHERE 
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND Status < 2
	AND b.DocNo IN (SELECT DocNo FROM SpTrnSordHdr WITH (NOLOCK, NOWAIT)
				WHERE 
					1 = 1
					AND CompanyCode =@CompanyCode
					AND BranchCode = @BranchCode
					AND UsageDocNo = @JobOrderNo
					AND Status = 4)
) #TempPickingSlipDtl

UPDATE SpTrnSPickingDtl
SET SalesAmt = b.SalesAmt 
	, DiscAmt = b.DiscAmt
	, NetSalesAmt = b.NetSalesAmt
	, TotSalesAmt = b.NetSalesAmt
	, QtyBill = b.QtyPicked
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpTrnSPickingDtl a, #TempPickingSlipDtl b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PickingSlipNo = b.PickingSlipNo
	AND a.PartNo = b.PartNo

--===============================================================================================================================
-- RE-CALCULATE AMOUNT HEADER AND UPDATE STATUS
--===============================================================================================================================
SELECT * INTO #TempPickingSlipHdr FROM (
SELECT
	a.CompanyCode
	, a.BranchCode
	, a.PickingSlipNo	
	, SUM(b.QtyPicked) TotSalesQty
	, SUM(b.SalesAmt) TotSalesAmt
	, SUM(b.DiscAmt) TotDiscAmt
	, SUM(b.NetSalesAmt) TotDPPAmt
	, floor(SUM(b.NetSalesAmt) * (ISNULL((SELECT TaxPct FROM GnMstTax x WITH (NOLOCK, NOWAIT) WHERE x.CompanyCode = @CompanyCode AND x.TaxCode IN 
		(SELECT TaxCode FROM GnMstCustomerProfitCenter y WITH (NOLOCK, NOWAIT) WHERE y.CompanyCode = @CompanyCode AND y.BranchCode = @BranchCode
			AND y.ProfitCenterCode = '300' AND y.CustomerCode = @CustomerCode)),0)/100))
	  TotPPNAmt
FROM spTrnSPickingHdr a WITH (NOLOCK, NOWAIT)
LEFT JOIN spTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PickingSlipNo = b.PickingSlipNo
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.PickingSlipNo IN (SELECT DISTINCT(PickingSlipNo) FROM #TempPickingSlipDtl WITH (NOLOCK, NOWAIT))
GROUP BY a.CompanyCode
	, a.BranchCode
	, a.PickingSlipNo	
) #TempPickingSlipHdr

UPDATE SpTrnSPickingHdr
SET TotSalesQty = b.TotSalesQty
	, TotSalesAmt = b.TotSalesAmt
	, TotDiscAmt = b.TotDiscAmt
	, TotDPPAmt = b.TotDPPAmt
	, TotPPNAmt = b.TotPPNAmt
	, TotFinalSalesAmt = b.TotDPPAmt + b.TotPPNAmt
	, Status = 2
	, PickedBy = @PickedBy
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpTrnSPickingHdr a, #TempPickingSlipHdr b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.PickingSlipNo = b.PickingSlipNo

--===============================================================================================================================
-- UPDATE SO SUPPLY AND STATUS HEADER 
--===============================================================================================================================
UPDATE SpTrnSOSupply
SET	Status = 2
	, QtyPicked = b.QtyPicked
	, QtyBill = b.QtyPicked
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
FROM SpTrnSOSupply a, #TempPickingSlipDtl b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.DocNo = b.DocNo
	AND a.PartNo = b.PartNo

UPDATE SpTrnSORDHdr 
SET Status = 5
	, LastUpdateBy = @UserID
	, LastUpdateDate = GetDate()
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocNo IN (SELECT DISTINCT(DocNo) FROM #TempPickingSlipDtl)

--===============================================================================================================================
-- UPDATE SUPPLY SLIP QTY SERVICE 
--===============================================================================================================================
DECLARE @ServiceNo VARCHAR(MAX)

SET @ServiceNo = (SELECT ServiceNo FROM svTrnService WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		AND ProductType = @ProductType AND JobOrderNo = @JobOrderNo)

SELECT * INTO #TempServiceItem FROM (
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq
	, a.DemandQty
	, a.SupplyQty
	, b.QtyBill
	, b.DocNo
	, c.RetailPriceInclTax - (c.RetailPriceInclTax * isnull(d.DiscPct, 0) * 0.01) CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DiscPct
FROM SvTrnSrvItem a WITH (NOLOCK, NOWAIT)
INNER JOIN SpTrnSPickingDtl b WITH (NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo AND a.SupplySlipNo = b.DocNo
INNER JOIN SpMstItemPrice c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.PartNo = c.PartNo
INNER JOIN gnMstSupplierProfitCenter d ON c.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.ServiceNo = @ServiceNo
	AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
	AND a.SupplySlipNo = b .DocNo
	AND d.SupplierCode = dbo.GetBranchMD(@CompanyCode,@BranchCode)
	AND d.ProfitCenterCode = '300'
) #TempServiceItem 

UPDATE svTrnSrvItem
SET SupplyQty = (CASE WHEN b.QtyBill > b.DemandQty 
				THEN 
					CASE WHEN b.DemandQty = 0 THEN b.QtyBill ELSE b.DemandQty END
				ELSE b.QtyBill END)
	, LastUpdateBy = @UserID
	, LastUpdateDate = Getdate()
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.PartSeq = b.PartSeq
	AND a.SupplySlipNo = b.DocNo

UPDATE svTrnSrvItem
SET CostPrice = b.CostPrice
	, LastUpdateBy = @UserID
	, LastUpdateDate = Getdate()
FROM svTrnSrvItem a, #TempServiceItem b
WHERE
	1 = 1
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.ProductType = b.ProductType
	AND a.ServiceNo = b.ServiceNo
	AND a.PartNo = b.PartNo
	AND a.SupplySlipNo = b.DocNo

--===============================================================================================================================
-- INSERT NEW SRV ITEM BASED PICKING LIST
--===============================================================================================================================
INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
SELECT 
	a.CompanyCode
	, a.BranchCode
	, a.ProductType
	, a.ServiceNo
	, a.PartNo
	, a.PartSeq + 1
	, 0 DemandQty
	, a.QtyBill - a.DemandQty SupplyQty
	, 0 ReturnQty
	, a.CostPrice
	, a.RetailPrice
	, a.TypeOfGoods
	, a.BillType
	, a.DocNo SupplySlipNo
	, (SELECT DocDate FROM SpTrnSORDHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
		AND DocNo = a.DocNo) SupplySlipDate
	, NULL SSReturnNo
	, NULL SSReturnDate
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, a.DiscPct
FROM #TempServiceItem a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.ProductType = @ProductType
	AND a.DemandQty < a.QtyBill
	AND a.QtyBill > 0
	AND a.DemandQty > 0

DROP TABLE #TempServiceItem 

--===============================================================================================================================
-- INSERT BPS AND LAMPIRAN
--===============================================================================================================================
DECLARE @PickingSlipNo	VARCHAR(MAX)
DECLARE	@TempBPSFNo		VARCHAR(MAX)
DECLARE	@TempLMPNo		VARCHAR(MAX)
DECLARE @MaxBPSFNo		INT
DECLARE @MaxLmpNo		INT

DECLARE db_cursor CURSOR FOR
SELECT DISTINCT PickingSlipNo FROM #TempPickingSlipHdr
OPEN db_cursor
FETCH NEXT FROM db_cursor INTO @PickingSlipNo

WHILE @@FETCH_STATUS = 0
BEGIN	

--===============================================================================================================================
-- INSERT BPS HEADER
--===============================================================================================================================
SET @MaxBPSFNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'BPF' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempBPSFNo = ISNULL((SELECT 'BPF/' + RIGHT(YEAR(GETDATE()),2) + '/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxBPSFNo, 1), 6)),'BPF/YY/XXXXXX')

INSERT INTO SpTrnSBPSFHdr
SELECT 
	CompanyCode
	, BranchCode
	, @TempBPSFNo BPSFNo
	, GetDate() BPSFDate
	, PickingSlipNo
	, PickingSlipDate
	, TransType
	, SalesType
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, '2' Status
	, 0 PrintSeq
	, TypeOfGoods
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL isLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSPickingHdr WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND PickingSlipNo = @PickingSlipNo

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'BPF'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT BPS DETAIL
--===============================================================================================================================
INSERT INTO SpTrnSBPSFDtl
SELECT
	CompanyCode
	, BranchCode
	, @TempBPSFNo
	, WarehouseCode
	, PartNo
	, PartNoOriginal
	, DocNo
	, DocDate
	, ReferenceNo
	, ReferenceDate
	, LocationCode
	, QtyBill
	, RetailPriceInclTax
	, RetailPrice
	, CostPrice
	, DiscPct
	, SalesAmt
	, DiscAmt
	, NetSalesAmt
	, 0 PPNAmt
	, TotSalesAmt
	, ProductType
	, PartCategory 
	, MovingCode
	, ABCClass
	, '' ExPickingListNo
	, @DefaultDate ExPickingListDate
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSPickingDtl WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND PickingSlipNo = @PickingSlipNo

--===============================================================================================================================
-- INSERT LAMPIRAN HEADER
--===============================================================================================================================
SET @MaxLmpNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
	CompanyCode = @CompanyCode
		AND BranchCode = @BranchCode
		AND DocumentType = 'LMP' 
		AND ProfitCenterCode = '300' 
		AND DocumentYear = YEAR(GetDate())),0)

SET @TempLmpNo = ISNULL((SELECT 'LMP/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxLmpNo, 1), 6)),'LMP/YY/XXXXXX')

INSERT INTO SpTrnSLmpHdr
SELECT
	CompanyCode
	, BranchCode
	, @TempLmpNo LmpNo	
	, GetDate() LmpDate
	, @TempBPSFNo BPSFNo
	, (SELECT BPSFDate FROM SpTrnSBPSFHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND BPSFNo = @TempBPSFNo)
		BPSFDate
	, PickingSlipNo
	, PickingSlipDate
	, TransType
	, CustomerCode
	, CustomerCodeBill
	, CustomerCodeShip
	, TotSalesQty
	, TotSalesAmt
	, TotDiscAmt
	, TotDPPAmt
	, TotPPNAmt
	, TotFinalSalesAmt
	, CONVERT(BIT, 0) isPKP
	, '0' Status
	, 0 PrintSeq
	, TypeOfGoods
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
	, NULL IsLocked
	, NULL LockingBy
	, NULL LockingDate
FROM SpTrnSPickingHdr WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND PickingSlipNo = @PickingSlipNo

UPDATE GnMstDocument
SET DocumentSequence = DocumentSequence + 1
	, LastUpdateDate = GetDate()
	, LastUpdateBy = @UserID
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND DocumentPrefix = 'LMP'
	AND ProfitCenterCode = '300'
	AND DocumentYear = Year(GetDate())

--===============================================================================================================================
-- INSERT LAMPIRAN DETAIL
--===============================================================================================================================
INSERT INTO SpTrnSLmpDtl
SELECT
	a.CompanyCode
	, a.BranchCode
	, @TempLmpNo LmpNo
	, a.WarehouseCode
	, a.PartNo
	, a.PartNoOriginal
	, a.DocNo
	, a.DocDate
	, a.ReferenceNo
	, a.ReferenceDate
	, a.LocationCode
	, a.QtyBill
	, a.RetailPriceInclTax
	, a.RetailPrice
	, a.CostPrice
	, a.DiscPct
	, a.SalesAmt
	, a.DiscAmt
	, a.NetSalesAmt
	, 0 PPNAmt
	, a.TotSalesAmt
	, a.ProductType
	, a.PartCategory 
	, a.MovingCode
	, a.ABCClass
	, @UserID CreatedBy
	, GetDate() CreatedDate
	, @UserID LastUpdateBy
	, GetDate() LastUpdateDate
FROM SpTrnSPickingDtl a WITH (NOLOCK, NOWAIT)
WHERE 
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND a.PickingSlipNo = @PickingSlipNo
	AND a.QtyPicked > 0

--===============================================================================================================================
-- UPDATE STOCK
--===============================================================================================================================

--===============================================================================================================================
-- VALIDATION QTY
--===============================================================================================================================
	DECLARE @Onhand_SRValid NUMERIC(18,2)	
	DECLARE @Allocation_SRValid NUMERIC(18,2)
	DECLARE @errmsg VARCHAR(MAX)
	DECLARE @CompanyMD AS VARCHAR(15)
	DECLARE @BranchMD AS VARCHAR(15)
	DECLARE @WarehouseMD AS VARCHAR(15)

	SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
	SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
	SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

declare @validString varchar(max)

declare @valid_2 table(
PartNo varchar(15),
QtyValidSR NUMERIC(18,2),
QtyValidOnhand NUMERIC(18,2)
)
    set @validString ='SELECT a.PartNo
		, a.AllocationSR - b.QtyBill QtyValidSR
		, a.Onhand - b.QtyBill QtyValidOnhand
	FROM '+ @DbMD +'..SpMstItems a, SpTrnSPickingDtl b
	WHERE 1 = 1
		AND a.CompanyCode = '''+ @CompanyMD +'''
		AND a.BranchCode = '''+ @BranchMD+'''
		AND b.PickingSlipNo = '''+@PickingSlipNo+'''
        AND b.ReferenceNo = '''+@JobOrderNo+'''
		AND b.CompanyCode = '''+ @CompanyCode +'''
		AND b.BranchCode = '''+ @BranchCode+'''
		AND a.PartNo = b.PartNo'
	
	--print(@validString)
	insert into @valid_2 exec(@validString)

select * from @valid_2

	SET @Allocation_SRValid = ISNULL((SELECT TOP 1 QtyValidSR FROM @valid_2 WHERE QtyValidSR < 0),0)
	SET @Onhand_SRValid = ISNULL((SELECT TOP 1 QtyValidOnhand FROM @valid_2 WHERE QtyValidOnhand < 0),0)
	select @Allocation_SRValid
	select @Onhand_SRValid

	IF (@Onhand_SRValid < 0 OR @Allocation_SRValid < 0)
	BEGIN
		SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
		
		CLOSE db_cursor
		DEALLOCATE db_cursor 
		
		DROP TABLE #TempPickingSlipDtl
		DROP TABLE #TempPickingSlipHdr
		
		RAISERROR (@errmsg,16,1);
		
		RETURN
	END
--===============================================================================================================================

--DECLARE @DbMD AS VARCHAR(15)
--SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

DECLARE @Query AS VARCHAR(MAX)
SET @Query = 
'UPDATE '+@DbMD+'..SpMstItems
SET
	AllocationSR = AllocationSR - b.QtySupply
	, Onhand = Onhand - b.QtyBill
	, LastUpdateBy = ' + '''' + @UserID + '''' +'
	, LastUpdateDate = GetDate()
	, LastSalesDate = GetDate()
FROM ' + @DbMD + '..SpMstItems a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = ' + ''''+@CompanyMD+'''' + '
	AND a.BranchCode = ' + ''''+@BranchMD +''''+ '
	AND b.PickingSlipNo = ' + ''''+@PickingSlipNo+'''' + '
	AND b.ReferenceNo = ' + ''''+@JobOrderNo+'''' + '
	AND b.CompanyCode = ' + ''''+@CompanyCode+'''' + '
	AND b.BranchCode = ' + ''''+@BranchCode+'''' + '
	AND a.PartNo = b.PartNo
UPDATE '+ @DbMD +'..SpMstItemLoc
SET
	AllocationSR = AllocationSR - b.QtySupply
	, Onhand = Onhand - b.QtyBill
	, LastUpdateBy = ' + '''' + @UserID + '''' +'
	, LastUpdateDate = GetDate()
FROM ' + @DbMD + '..SpMstItemLoc a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = ' + ''''+@CompanyMD+'''' + '
	AND a.BranchCode = ' + ''''+@BranchMD +''''+ '
	AND a.WarehouseCode = ' + ''''+@WarehouseMD +''''+ '
	AND b.PickingSlipNo = ' + ''''+@PickingSlipNo+'''' + '
	AND b.CompanyCode = ' + ''''+@CompanyCode+'''' + '
	AND b.BranchCode = ' + ''''+@BranchCode+'''' + '
	AND a.PartNo = b.PartNo'

EXEC(@query)
	--print(@query)
--===============================================================================================================================
-- UPDATE DEMAND CUST AND DEMAND ITEM
--===============================================================================================================================
UPDATE SpHstDemandCust
SET SalesFreq = SalesFreq + 1
	, SalesQty = SalesQty + b.QtyBill
	, LastUpdateBy = @UserID 
	, LastUpdateDate = GetDate()
FROM SpHstDemandCust a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND b.PickingSlipNo = @PickingSlipNo
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = Year(b.DocDate)
	AND a.Month = Month(b.DocDate)
	AND a.CustomerCode IN (SELECT CustomerCode FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = BranchCode
							AND PickingSlipNo = @PickingSlipNo)
	AND a.PartNo = b.PartNo
	

UPDATE SpHstDemandItem
SET SalesFreq = SalesFreq + 1
	, SalesQty = SalesQty + b.QtyBill
	, LastUpdateBy = @UserID 
	, LastUpdateDate = GetDate()
FROM SpHstDemandItem a, SpTrnSPickingDtl b
WHERE
	1 = 1
	AND a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode
	AND b.PickingSlipNo = @PickingSlipNo
	AND a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	AND a.Year = Year(b.DocDate)
	AND a.Month = Month(b.DocDate)
	AND a.PartNo = b.PartNo
--
----=============================================================================================================================
---- INSERT TO ITEM MOVEMENT
----=============================================================================================================================
INSERT INTO SpTrnIMovement
SELECT
	@CompanyCode CompanyCode
	, @BranchCode BranchCode
	, a.LmpNo DocNo
	, (SELECT LmPDate FROM SpTrnSLmpHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode 
		AND BranchCode = @BranchCode AND LmpNo = a.LmpNo) 
	  DocDate
	, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),getdate()) CreatedDate 
	, '00' WarehouseCode
	, LocationCode 
	, a.PartNo
	, 'OUT' SignCode
	, 'LAMPIRAN' SubSignCode
	, a.QtyBill
	, a.RetailPrice
	, a.CostPrice
	, a.ABCClass
	, a.MovingCode
	, a.ProductType
	, a.PartCategory
	, @UserID CreatedBy
FROM SpTrnSLmpDtl a WITH (NOLOCK, NOWAIT)
WHERE
	1 = 1
	AND CompanyCode = @CompanyCode
	AND BranchCode = @BranchCode
	AND LmpNo IN (SELECT LmpNo FROM SpTrnSLmpHdr WITH (NOLOCK, NOWAIT) WHERE CompanyCode = @CompanyCode 
				AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)

--===============================================================================================================================
-- INSERT INTO svSDMovement
--===============================================================================================================================
declare @md bit
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

 if(@md = 0)
 begin
	set @Query = '
	insert into ' + @DbMD + '..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq
	, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice
	, TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD
	, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus
	, ProcessDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)  
	select h.CompanyCode, h.BranchCode, h.LmpNo, h.LmpDate, d.PartNo, ROW_NUMBER() OVER(Order by d.LmpNo)
	,d.WarehouseCode, d.QtyBill, d.QtyBill, d.DiscPct
	--,isnull(((select RetailPrice from spTrnSLmpDtl
	--	where CompanyCode = ''' + @CompanyCode + '''  and BranchCode = ''' + @BranchCode  + '''
	--	and ProductType = ''' + @ProductType  + ''' and LmpNo = ''' + @TempLmpNo + ''' and PartNo = d.PartNo) / 1.1 * 
	--	((100 - isnull((select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
	--		where CompanyCode = ''' + @CompanyCode + ''' and BranchCode = ''' + @BranchCode  + ''' and SupplierCode = dbo.GetBranchMD(''' + @CompanyCode + ''', ''' + @BranchCode  + ''') 
	--		and ProfitCenterCode = ''300''),0)) * 0.01)),0) CostPrice
	, d.CostPrice
	, d.RetailPrice
	,h.TypeOfGoods, ''' + @CompanyMD + ''', ''' + @BranchMD + ''', ''' + @WarehouseMD + ''', p.RetailPriceInclTax, p.RetailPrice, 
	p.CostPrice,''x'', d.ProductType,''300'', ''0'',''0'',''' + convert(varchar, GETDATE()) + ''',''' + @UserID + ''',''' +
	  convert(varchar, GETDATE()) + ''',''' +  @UserID + ''',''' +  convert(varchar, GETDATE()) + '''
	 from spTrnSLmpDtl d 
	 inner join spTrnSLmpHdr h on h.CompanyCode = d.CompanyCode and h.BranchCode = d.BranchCode and h.LmpNo = d.LmpNo  
	 join spmstitemprice p
	 on p.PartNo = d.PartNo and p.CompanyCode = d.CompanyCode and p.BranchCode = d.BranchCode
	  where 1 = 1   
		and d.CompanyCode = ''' + @CompanyCode + ''' 
		and d.BranchCode  = ''' + @BranchCode  + '''
		and d.ProductType = ''' + @ProductType  + '''
		and d.LmpNo = ''' + @TempLmpNo + '''';
	
	exec(@Query);
end

FETCH NEXT FROM db_cursor INTO @PickingSlipNo
END
CLOSE db_cursor
DEALLOCATE db_cursor 

--===============================================================================================================================
-- UPDATE TRANSDATE
--===============================================================================================================================

update gnMstCoProfileSpare
set TransDate=getdate()
	, LastUpdateBy=@UserID
	, LastUpdateDate=getdate()
where CompanyCode = @CompanyCode AND BranchCode = @BranchCode


--===============================================================================================================================
-- DROP SECTION HEADER
--===============================================================================================================================
DROP TABLE #TempPickingSlipDtl
DROP TABLE #TempPickingSlipHdr
end

GO

IF object_id('uspfn_OmFPDetailCustomer') IS NOT NULL
	DROP PROCEDURE uspfn_OmFPDetailCustomer
GO
CREATE procedure [dbo].[uspfn_OmFPDetailCustomer]
	@ChassisCode as varchar(15)
	,@ChassisNo as varchar(15)
as

SELECT a.CompanyCode, a.BranchCode, d.BPKNo, a.SONo, a.EndUserName, b.RefferenceNo SKPKNo, a.EndUserAddress1, a.EndUserAddress2, a.EndUserAddress3, c.CustomerName, c.Address1, c.Address2, c.Address3,
	c.CityCode,(SELECT LookUpValueName FROM gnMstLookUpDtl where CodeID = 'CITY' AND ParaValue = c.CityCode) as CityName, 
	c.PhoneNo, c.HPNo, c.birthDate ,b.Salesman, (SELECT EmployeeName FROM gnMstEmployee where EmployeeID = b.Salesman) SalesmanName, b.SalesType
FROM omTrSalesSOVin a
	left join omTrSalesSO b on a.companyCode = b.companyCode 
		and a.BranchCode= b.BranchCode
		and a.SONo = b.SONo
		and b.Status = '2'
	left join gnMstCustomer c on b.CompanyCode = c.CompanyCode
		and b.CustomerCode = c.CustomerCode
	left join omTrSalesBPK d on a.CompanyCode = d.CompanyCode
		and a.BranchCode = d.BranchCode
		and a.SONo = d.SONo
WHERE a.ChassisCode=@ChassisCode AND a.ChassisNo=@ChassisNo
GO

if object_id('usprpt_OmRpLabaRugi001') is not null
	drop procedure usprpt_OmRpLabaRugi001
GO

CREATE procedure [dbo].[usprpt_OmRpLabaRugi001] 
(
	@CompanyCode Varchar(20),
	@BranchCodeFrom	VARCHAR(15),
	@BranchCodeTo	VARCHAR(15),
	@DateStart Datetime,
	@DateEnd Datetime,
	@SalesType char(1),
	@SalesFrom Varchar(20),
	@SalesTo Varchar(20),
	@param Char(1),
	@pDok CHAR(1)
)
as

--declare	@CompanyCode as varchar(20),
--	@BranchCode as varchar(20),
--	@DateStart as datetime,
--	@DateEnd as datetime,
--	@SalesType as char(1),
--	@SalesFrom as varchar(20),
--	@SalesTo as varchar(20),
--	@param as char(1),
--	@pDok as char(1)
--
--set @CompanyCode = '6058401'
--set	@BranchCode = '605840100'
--set	@DateStart = '20110901'
--set	@DateEnd = '20111130'
--set	@SalesType = ''
--set	@SalesFrom = ''
--set	@SalesTo = ''
--set	@param = '1'
--set	@pDok = '0'

select * into #t1
from (
	select distinct
		a.CompanyCode,a.BranchCode
		, (SELECT CompanyName FROM gnMstCoprofile WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode) BranchName  
		, a.InvoiceNo,a.InvoiceDate, a.CustomerCode
		, e.SalesType, e.SalesCode, e.SalesCode + ' - ' + h.LookupValueName SalesName
		, (case when e.SalesType = '0' then 'W - ' + i.CustomerName else 'D - ' + i.CustomerName end) as CustomerName
		, (e.Salesman + ' - ' + 
			(select EmployeeName 
			from gnMstEmployee 
			where CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = e.Salesman)
		) Salesman
		,(select top 1 DoNo from OmTrSalesDO f where f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode AND f.SONo = e.SONo ) DONo
		,(select top 1 DoDate from OmTrSalesDO f where f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode AND f.SONo = e.SONo ) DODate
		,e.SONo, e.SODate
		, d.SalesModelCode, d.SalesModelYear, d.ColourCode, d.ChassisCode, d.ChassisNo, d.EngineNo
		, 1 QtyUnit
		, isnull((d.COGSUnit + d.COGSOthers +d.COGSKaroseri),0) as COGS
		, isnull(b.AfterDiscDPP,0) AfterDiscDPP	
		, isnull((b.AfterDiscDPP - (d.COGSUnit + d.COGSOthers +d.COGSKaroseri)),0) as LabaRugi
		, isnull(((b.AfterDiscDPP - (d.COGSUnit + d.COGSOthers +d.COGSKaroseri)) / b.AfterDiscDPP),0) * 100 as Percentage		
		, (case @pDok when '0' then d.BPUDate when '1' then g.RefferenceDODate when '2' then g.RefferenceSJDate end) as BPUDate
		, datediff(day, (case @pDok when '0' then d.BPUDate when '1' then g.RefferenceDODate when '2' then g.RefferenceSJDate end), e.SODate) as Lama
		,'Invoice' TypeTrans
		, (select Top 1 CustomerName from GnMstCustomer where CustomerCode = e.LeasingCo) LeasingName
	from
		OmTrSalesInvoice a
		left join OmTrSalesInvoiceModel b on b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and b.InvoiceNo = a.InvoiceNo
		left join OmTrSalesInvoiceVin c on c.CompanyCode = a.CompanyCode and c.BranchCode = a.BranchCode and c.InvoiceNo = a.InvoiceNo
			and c.BPKNo= b.BPKNo and c.SalesModelCode=b.SalesModelCode and c.SalesModelYear=b.SalesModelYear
		left join omMstVehicle d on d.CompanyCode = a.CompanyCode and d.SalesModelCode=c.SalesModelCode and d.SalesModelYear=c.SalesModelYear
			and d.ChassisCode=c.ChassisCode and d.ChassisNo=c.ChassisNo
		left join OmTrSalesSO e on e.CompanyCode = a.CompanyCode and e.BranchCode = a.BranchCode and e.SONo = a.SONo	
		left join OmTrPurchaseBPU g on g.CompanyCode = a.CompanyCode AND g.BranchCode = a.BranchCode AND g.PONo= d.PONo and g.BPUNo = d.BPUNo 
		left join GnMstLookUpDtl h on a.CompanyCode = h.CompanyCode and e.SalesCode = h.LookUpValue and h.CodeID = 'GPAR' 
		left join GnMstCustomer i on a.CompanyCode = i.CompanyCode and a.CustomerCode = i.CustomerCode
	where a.CompanyCode = @CompanyCode 
		AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE a.BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo 		
		and ((case when @param = '0' then convert(varchar, a.InvoiceDate, 112) end)= convert(varchar, a.InvoiceDate, 112)
			or (case when @param = '1' then convert(varchar, a.InvoiceDate, 112) end) between convert(varchar, @DateStart, 112) and convert(varchar, @DateEnd, 112))
		and ((case when @SalesType = '' then e.SalesType end) <> ''
			or (case when @SalesType <> '' then e.SalesType end) = @SalesType)
		and ((case when @SalesFrom = '' then e.SalesCode end) <> ''
			or (case when @SalesFrom <> '' then e.SalesCode end) between @SalesFrom and @SalesTo)
		and a.Status <> '3'
) #t1

insert into #t1
select distinct
	a.CompanyCode,a.BranchCode
	, (SELECT CompanyName FROM gnMstCoprofile WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode) BranchName  
	, a.ReturnNo,a.ReturnDate, a.CustomerCode
	, e.SalesType, e.SalesCode, e.SalesCode + ' - ' + h.LookupValueName SalesName
	, (case when e.SalesType = 0 then 'W - ' + i.CustomerName else 'D - ' + i.CustomerName end) as CustomerName
	, (e.Salesman + ' - ' + 
		(select EmployeeName
		from gnMstEmployee 
		where CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND EmployeeID = e.Salesman)
	) Salesman
	,j.InvoiceNo DONo
	,j.InvoiceDate DODate
	,e.SONo, e.SODate
	, d.SalesModelCode, d.SalesModelYear, d.ColourCode, d.ChassisCode, d.ChassisNo, d.EngineNo
	, -1 QtyUnit
	, isnull((d.COGSUnit + d.COGSOthers + d.COGSKaroseri),0) as COGS
	, isnull(b.AfterDiscDPP,0) AfterDiscDPP
	, isnull((b.AfterDiscDPP - (d.COGSUnit + d.COGSOthers +d.COGSKaroseri)),0) as LabaRugi
	, isnull(((b.AfterDiscDPP - (d.COGSUnit + d.COGSOthers +d.COGSKaroseri)) / b.AfterDiscDPP),0) * 100 as Percentage		
	, (case @pDok when '0' then d.BPUDate when '1' then g.RefferenceDODate when '2' then g.RefferenceSJDate end) as BPUDate
	, datediff(day, (case @pDok when '0' then d.BPUDate when '1' then g.RefferenceDODate when '2' then g.RefferenceSJDate end), e.SODate) as Lama
	,'Retur' TypeTrans
	, (select Top 1 CustomerName from GnMstCustomer where CustomerCode = e.LeasingCo) LeasingName
	
from omTrSalesReturn a
	inner join omTrSalesReturnDetailModel b on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode
		and a.ReturnNo=b.ReturnNo
	inner join omTrSalesReturnVin c on a.CompanyCode=c.CompanyCode and a.BranchCode=c.BranchCode and a.ReturnNo=c.ReturnNo 
		and c.BPKNo= b.BPKNo and b.SalesModelCode=c.SalesModelCode and b.SalesModelYear=c.SalesModelYear
	left join omMstVehicle d on d.CompanyCode = a.CompanyCode and d.SalesModelCode=c.SalesModelCode and d.SalesModelYear=c.SalesModelYear
		and d.ChassisCode=c.ChassisCode and d.ChassisNo=c.ChassisNo
	left join omTrSalesInvoice j on a.CompanyCode=j.CompanyCode and a.BranchCode=j.BranchCode and a.InvoiceNo=j.InvoiceNo
	left join OmTrSalesSO e on e.CompanyCode = a.CompanyCode and e.BranchCode = a.BranchCode and e.SONo = j.SONo	
	left join OmTrPurchaseBPU g on g.CompanyCode = a.CompanyCode AND g.BranchCode = a.BranchCode AND g.PONo= d.PONo and g.BPUNo = d.BPUNo 
	left join GnMstLookUpDtl h on a.CompanyCode = h.CompanyCode and e.SalesCode = h.LookUpValue and h.CodeID = 'GPAR' 
	left join GnMstCustomer i on a.CompanyCode = i.CompanyCode and a.CustomerCode = i.CustomerCode
where a.CompanyCode = @CompanyCode 
	AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE a.BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo 		
	and ((case when @param = '0' then convert(varchar, a.ReturnDate, 112) end)= convert(varchar, a.ReturnDate, 112)
		or (case when @param = '1' then convert(varchar, a.ReturnDate, 112) end) between convert(varchar, @DateStart, 112) and convert(varchar, @DateEnd, 112))
	and ((case when @SalesType = '' then e.SalesType end) <> ''
		or (case when @SalesType <> '' then e.SalesType end) = @SalesType)
	and ((case when @SalesFrom = '' then e.SalesCode end) <> ''
		or (case when @SalesFrom <> '' then e.SalesCode end) between @SalesFrom and @SalesTo)
	and a.Status <> '3'

select * from #t1

drop table #t1
	
GO

if object_id('uspfn_SvTrnListKsgFromSPKPerJobNo') is not null
	drop PROCEDURE uspfn_SvTrnListKsgFromSPKPerJobNo
GO
CREATE procedure [dbo].[uspfn_SvTrnListKsgFromSPKPerJobNo]
 @CompanyCode varchar(15),  
 @BranchCode varchar(max),
 @ProductType varchar(15),   
 @JobOrder varchar(max)
as        
  
declare @sql varchar(max);
 
set @sql =   
'select * into #t1 from(  
select  
    convert(bit, 1) Process      
 , srv.BranchCode  
 , srv.JobOrderNo  
 , case when convert(varchar, srv.JobOrderDate, 106) = ''19000101'' then '''' else convert(varchar, srv.JobOrderDate, 106) end JobOrderDate  
 , srv.BasicModel  
 , srv.ServiceBookNo  
 , job.PdiFscSeq  
 , srv.Odometer  
 , srv.LaborGrossAmt  
 , round((select isnull(SUM((SupplyQty - ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = ''F''),0) MaterialGrossAmt --Pembulatan
 , round((srv.LaborGrossAmt + (select isnull(SUM((SupplyQty-ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = ''F'')),0) PdiFscAmount  --Pembulatan
 , isnull(case when convert(varchar, veh.FakturPolisiDate, 112) = ''19000101'' then '''' else convert(varchar, veh.FakturPolisiDate, 106) end, '''')  FakturPolisiDate  
 , isnull(case when convert(varchar, mstVeh.BPKDate, 112) = ''19000101'' then '''' else convert(varchar, mstVeh.BPKDate, 106) end, '''')  BPKDate  
 , srv.ChassisCode  
 , srv.ChassisNo  
 , srv.EngineCode  
 , srv.EngineNo   
    , srv.InvoiceNo  
 , isnull(inv.FPJNo, '''') FPJNo  
 , isnull(case when convert(varchar, inv.FPJDate, 112) = ''19000101'' then '''' else convert(varchar, inv.FPJDate, 106) end, '''')  FPJDate  
 , isnull(fpj.FPJGovNo, '''') FPJGovNo  
 , srv.TransmissionType  
 , srv.ServiceStatus  
 , srv.CompanyCode  
 , srv.ProductType  
from svTrnService srv  
left join svMstJob job  
 on job.CompanyCode = srv.CompanyCode  
  and job.ProductType = srv.ProductType  
  and job.BasicModel = srv.BasicModel  
  and job.JobType = srv.JobType  
left join svMstCustomerVehicle veh  
 on veh.CompanyCode = srv.CompanyCode  
  and veh.ChassisCode = srv.ChassisCode  
  and veh.ChassisNo = srv.ChassisNo  
left join omMstVehicle mstVeh  
 on mstVeh.CompanyCode = srv.CompanyCode  
  and mstVeh.ChassisCode = srv.ChassisCode  
  and mstVeh.ChassisNo = srv.ChassisNo  
left join svTrnInvoice inv  
 on inv.CompanyCode = srv.CompanyCode  
  and inv.BranchCode = srv.BranchCode  
  and inv.ProductType = srv.ProductType  
  and inv.InvoiceNo = srv.InvoiceNo  
left join svTrnFakturPajak fpj  
 on fpj.CompanyCode = srv.CompanyCode  
  and fpj.BranchCode = srv.BranchCode  
  and fpj.FPJNo = inv.FPJNo  
where   
 srv.CompanyCode =''' + @CompanyCode  + '''
 and srv.ProductType = ''' + @ProductType  + '''
 and job.GroupJobType = ''FSC''  
 and not exists (  
  select 1   
  from svTrnPdiFscApplication   
  where CompanyCode=srv.CompanyCode  
   and BranchCode=srv.BranchCode   
   and InvoiceNo=srv.JobOrderNo  
   and ProductType=srv.ProductType  
 ) and  not exists (  
  select 1   
  from svTrnPdiFscApplication   
  where CompanyCode=srv.CompanyCode  
   and BranchCode in (' + @BranchCode+ ')
   and InvoiceNo=srv.JobOrderNo  
   and ProductType=srv.ProductType  
 )
 and srv.JobOrderNo in (' + @JobOrder + ')
) #t1  
  
select   
row_number() over (order by #t1.BranchCode, #t1.JobOrderNo) No,  
* from #t1   
where ServiceStatus=5 ---service status hanya yang tutup SPK
-- in (5, 7, 9)  
order by BranchCode, JobOrderNo  
  
drop table #t1';

exec (@sql);
GO

if object_id('uspfn_SpMstItemLocView') is not null
	drop procedure uspfn_SpMstItemLocView
GO
CREATE procedure [dbo].[uspfn_SpMstItemLocView]  
@CompanyCode varchar(15),  
@BranchCode varchar(15),  
@TypeOfGoods varchar(5),  
@ProductType varchar(15)  
   
AS  
SELECT   
  ItemLoc.PartNo  
 ,ItemInfo.PartName  
 ,ItemInfo.SupplierCode  
 ,ItemLoc.WarehouseCode  
 ,c.LookUpValueName [WarehouseName]  
 ,ItemLoc.LocationCode  
 ,Items.PartCategory  
 ,Items.CompanyCode  
 ,Items.BranchCode     
 ,Items.ProductType  
 ,Items.TypeOfGoods  
 ,ItemLoc.LocationSub1  
 ,ItemLoc.LocationSub2  
 ,ItemLoc.LocationSub3  
 ,ItemLoc.LocationSub4  
 ,ItemLoc.LocationSub5  
 ,ItemLoc.LocationSub6  
FROM spMstItemLoc ItemLoc  
INNER JOIN spMstItems Items   
    ON ItemLoc.CompanyCode=Items.CompanyCode  
    AND ItemLoc.BranchCode=Items.BranchCode  
    AND ItemLoc.PartNo=Items.PartNo  
INNER JOIN spMstItemInfo ItemInfo   
    ON ItemLoc.CompanyCode=ItemInfo.CompanyCode  
    AND ItemLoc.PartNo=ItemInfo.PartNo  
 inner join gnMstLookUpDtl c  ON ItemLoc.CompanyCode = c.CompanyCode   
       AND ItemLoc.WarehouseCode = c.LookUpValue
	   AND c.CodeID = 'WRCD'   
WHERE  
 Items.CompanyCode= @CompanyCode
    AND Items.BranchCode=@BranchCode    
    AND Items.TypeOfGoods=@TypeOfGoods
    AND Items.ProductType=@ProductType
    AND Items.TypeOfGoods=@TypeOfGoods
	AND ItemLoc.WarehouseCode NOT LIKE 'X%' 
GO

if object_id('uspfn_SpMstItemLocView') is not null
	drop procedure uspfn_SpMstItemLocView
GO
CREATE procedure [dbo].[uspfn_SpMstItemLocView]  
@CompanyCode varchar(15),  
@BranchCode varchar(15),  
@TypeOfGoods varchar(5),  
@ProductType varchar(15)  
   
AS  
SELECT   
  ItemLoc.PartNo  
 ,ItemInfo.PartName  
 ,ItemInfo.SupplierCode  
 ,ItemLoc.WarehouseCode  
 ,c.LookUpValueName [WarehouseName]  
 ,ItemLoc.LocationCode  
 ,Items.PartCategory  
 ,Items.CompanyCode  
 ,Items.BranchCode     
 ,Items.ProductType  
 ,Items.TypeOfGoods  
 ,ItemLoc.LocationSub1  
 ,ItemLoc.LocationSub2  
 ,ItemLoc.LocationSub3  
 ,ItemLoc.LocationSub4  
 ,ItemLoc.LocationSub5  
 ,ItemLoc.LocationSub6  
FROM spMstItemLoc ItemLoc  
INNER JOIN spMstItems Items   
    ON ItemLoc.CompanyCode=Items.CompanyCode  
    AND ItemLoc.BranchCode=Items.BranchCode  
    AND ItemLoc.PartNo=Items.PartNo  
INNER JOIN spMstItemInfo ItemInfo   
    ON ItemLoc.CompanyCode=ItemInfo.CompanyCode  
    AND ItemLoc.PartNo=ItemInfo.PartNo  
 inner join gnMstLookUpDtl c  ON ItemLoc.CompanyCode = c.CompanyCode   
       AND ItemLoc.WarehouseCode = c.LookUpValue  
WHERE  
 Items.CompanyCode= @CompanyCode
    AND Items.BranchCode=@BranchCode    
    AND Items.TypeOfGoods=@TypeOfGoods
    AND Items.ProductType=@ProductType
    AND Items.TypeOfGoods=@TypeOfGoods
	AND ItemLoc.WarehouseCode NOT LIKE 'X%' 
     
GO
if object_id('uspfn_SvTrnServiceSaveItem') is not null
	drop procedure uspfn_SvTrnServiceSaveItem
GO

CREATE procedure [dbo].[uspfn_SvTrnServiceSaveItem]  
--DECLARE
	@CompanyCode varchar(15),  
	@BranchCode varchar(15),  
	@ProductType varchar(15),  
	@ServiceNo bigint,  
	@BillType varchar(15),  
	@PartNo varchar(20),  
	@DemandQty numeric(18,2),  
	@PartSeq numeric(5,2),  
	@UserID varchar(15),  
	@DiscPct numeric(5,2)  
as        
  
--set @CompanyCode = '6115204001'  
--set @BranchCode = '6115204102'  
--set @ProductType = '2W'  
--set @ServiceNo = 16455  
--set @BillType = 'C'  
--set @PartNo = 'K1200-50002-000'  
--set @DemandQty = 1 
--set @PartSeq = -1  
--set @UserID = 'yo'  
--set @DiscPct = 0  

declare @errmsg varchar(max)  
declare @QueryTemp as varchar(max)  
declare @IsSPK as char(1)
  
begin try  
 -- select data svTrnService  
 select * into #srv from (  
   select a.* from svTrnService a  
  where 1 = 1  
    and a.CompanyCode = @CompanyCode  
    and a.BranchCode  = @BranchCode  
    and a.ProductType = @ProductType  
    and a.ServiceNo   = @ServiceNo  
 )#srv  
   
 declare @CostPrice as decimal  
 declare @RetailPrice as decimal  
 declare @TypeOfGoods as varchar(2)  
 declare @CostPriceMD as decimal  
 declare @RetailPriceMD as decimal  
 declare @RetailPriceInclTaxMD as decimal  
   
 declare @DealerCode as varchar(2)  
 declare @CompanyMD as varchar(15)  
 declare @BranchMD as varchar(15)  
 declare @WarehouseMD as varchar(15)  
  
 set @DealerCode = 'MD'  
 set @CompanyMD = (select CompanyMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 set @BranchMD = (select BranchMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 set @WarehouseMD = (select WarehouseMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
 
if object_id('#tmpSvSDMovement') is not null drop table #tmpSvSDMovement
 
 -- Check MD or SD
	-- If SD  
 if (@CompanyCode != @CompanyMD and @BranchCode != @BranchMD)   
 begin  
	  set @DealerCode = 'SD'  

	  set @IsSPK = isnull((select a.ServiceType from #srv a where a.ServiceType = '2'),0)
	  
	  declare @DbName as varchar(50)  
	  set @DbName = (select DbMD from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode)  
	  
	  declare @PurchaseDisc as decimal  
	  set @PurchaseDisc = (select isnull(DiscPct, 0) from gnMstSupplierProfitCenter   
		   where CompanyCode = @CompanyCode   
		   and BranchCode = @BranchCode  
		   and SupplierCode = @BranchMD
		   and ProfitCenterCode = '300')  
	         
	  if (@PurchaseDisc = 0) raiserror ('Purchase Discount belum di-setting untuk Part tersebut!',16,1);            
	       
	  declare @tblTemp as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )
	  
	  declare @tblTemp1 as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )
	    
	  -- Untuk ItemPrice Mengambil dari masing-masing dealer	
		set @QueryTemp = 'select   
			  b.CostPrice   
			 ,b.RetailPrice  
			 ,b.RetailPriceInclTax  
			 ,a.TypeOfGoods 
			from (select
				i.PartNo   
				,i.TypeOfGoods  
				 from ' + @DbName +'..spMstItems i  
				 where i.CompanyCode = ''' + @CompanyMD + '''  
				 and i.BranchCode  = ''' + @BranchMD + '''  
				 and i.PartNo      = ''' + @PartNo + '''
			) a inner join spMstItemPrice b on b.PartNo = a.PartNo
		 where b.CompanyCode = ''' + @CompanyCode + '''
		 and b.BranchCode = ''' + @BranchCode + ''''
		          
	  insert into @tblTemp    
	  exec (@QueryTemp)  
	  
		set @QueryTemp = 'select   
			  b.CostPrice   
			 ,b.RetailPrice  
			 ,b.RetailPriceInclTax  
			 ,a.TypeOfGoods 
			from (select
				i.PartNo   
				,i.TypeOfGoods  
				 from ' + @DbName +'..spMstItems i  
				 where i.CompanyCode = ''' + @CompanyMD + '''  
				 and i.BranchCode  = ''' + @BranchMD + '''  
				 and i.PartNo      = ''' + @PartNo + '''
			) a inner join ' + @DbName +'..spMstItemPrice b on b.PartNo = a.PartNo
		 where b.CompanyCode = ''' + @CompanyMD + '''
		 and b.BranchCode = ''' + @BranchMD + ''''
		 
  	  insert into @tblTemp1
	  exec (@QueryTemp)  
	  print (@QueryTemp)  
	  
	  set @CostPrice = ((select RetailPriceInclTax from @tblTemp1) - ((select RetailPriceInclTax from @tblTemp1) * @PurchaseDisc * 0.01))  
	  select @CostPrice  
	  set @RetailPrice = (select RetailPrice from @tblTemp)
	  --select a.RetailPrice from spMstItemPrice a where a.CompanyCode = @CompanyCode and a.BranchCode = @BranchCode and a.PartNo = @PartNo)    
	  set @TypeOfGoods = (select TypeOfGoods from @tblTemp)  
	    
	  set @CostPriceMD = (select CostPrice from @tblTemp)  
	  set @RetailPriceMD = (select RetailPrice from @tblTemp)  
	  set @RetailPriceInclTaxMD = (select RetailPriceInclTax from @tblTemp)  
	    
	  -- select @PurchaseDisc  
 end   
 -- If MD
 else  
 begin
	 declare @tblTempPart as table  
	  (  
	   CostPrice decimal(18,2),  
	   RetailPrice decimal(18,2),  
	   RetailPriceInclTax decimal(18,2),  
	   TypeOfGoods varchar (2)  
	  )  

	  set @QueryTemp = 'select   
		a.CostPrice   
	   ,a.RetailPrice  
		 from ' + @DbName + '..spMstItemPrice a  
	   where 1 = 1  
		 and a.CompanyCode = ''' + @CompanyMD + '''  
		 and a.BranchCode  = ''' + @BranchMD + '''  
		 and a.PartNo      = ''' + @PartNo + ''''  
	          
	  insert into @tblTempPart    
	  exec (@QueryTemp)  
	   
	  --select * into #part from (  
	  --select   
	  --  a.CostPrice   
	  -- ,a.RetailPrice  
	  --  from spMstItemPrice a  
	  -- where 1 = 1  
	  --   and a.CompanyCode = @CompanyCode  
	  --   and a.BranchCode  = @BranchCode  
	  --   and a.PartNo      = @PartNo  
	  --)#part  
	    
	  set @CostPrice = (select CostPrice from @tblTempPart)  
	  set @RetailPrice = (select RetailPrice from @tblTempPart)  
 end  
 -- EOF Check MD or SD
  
 
 if (@PartSeq > 0)  
 begin    
	-- select data mst job  
	select * into #job from (  
	select b.*  
	from svTrnService a, svMstJob b  
	where 1 = 1  
	 and b.CompanyCode = a.CompanyCode  
	 and b.ProductType = a.ProductType  
	 and b.BasicModel = a.BasicModel  
	 and b.JobType = a.JobType  
		and a.CompanyCode = @CompanyCode  
	 and a.BranchCode  = @BranchCode  
	 and a.ServiceNo   = @ServiceNo  
	 and b.GroupJobType = 'FSC'  
	)#  
	if exists (select * from #job)  
	begin  
	   -- update svTrnSrvItem  
	   set @Querytemp ='
	   update svTrnSrvItem set  
		 DemandQty      = '+ convert(varchar,@DemandQty) +'
		,CostPrice      = '+ convert(varchar,@CostPrice) +' 
		,RetailPrice    = isnull((  
			 select top 1 b.RetailPrice from #srv a, svMstTaskPart b  
			  where b.CompanyCode = a.CompanyCode  
				and b.ProductType = a.ProductType  
				and b.BasicModel = a.BasicModel  
				and b.JobType = a.JobType  
				and b.PartNo = '''+ @PartNo +''' 
				and b.BillType = ''F'' 
			 ), (  
			  select top 1 isnull(RetailPrice, 0) RetailPrice  
				from spMstItemPrice  
			   where 1 = 1  
				 and CompanyCode = '''+ @CompanyCode +'''
				 and BranchCode = '''+ @BranchCode +'''
				 and PartNo = '''+ @PartNo  +'''
			  )  
			 )  
		,LastupdateBy   = (select LastupdateBy from #srv)  
		,LastupdateDate = (select LastupdateDate from #srv)  
		,BillType       = '''+ @BillType +'''
		,DiscPct        = '+ convert(varchar,@DiscPct) +'  
		where 1 = 1  
		  and CompanyCode  = '''+ @CompanyCode +''' 
		  and BranchCode   = '''+ @BranchCode +''' 
		  and ProductType  = (select ProductType from #srv)  
		  and ServiceNo    = (select ServiceNo from #srv)  
		  and PartNo       = '''+ @PartNo +''' 
		  and PartSeq      = '+ convert(varchar,@PartSeq) +'' 
		  
		  exec(@QueryTemp) 
	  end  
	  else  
	  begin  
	   -- update svTrnSrvItem  
	   update svTrnSrvItem set  
		 DemandQty      = @DemandQty  
		,CostPrice      = @CostPrice  
		,RetailPrice    = @RetailPrice  
		,LastupdateBy   = (select LastupdateBy from #srv)  
		,LastupdateDate = (select LastupdateDate from #srv)  
		,BillType       = @BillType  
		,DiscPct        = @DiscPct  
		where 1 = 1  
		  and CompanyCode  = @CompanyCode  
		  and BranchCode   = @BranchCode  
		  and ProductType  = (select ProductType from #srv)  
		  and ServiceNo    = (select ServiceNo from #srv)  
		  and PartNo       = @PartNo  
		  and PartSeq      = @PartSeq           
	  end   
	    
	--update svSDMovement  
	if (@DealerCode = 'SD' and @IsSPK = '2')  
	begin    
		set @QueryTemp = 'update ' + @DbName + '..svSDMovement set    
		QtyOrder    = ' + case when @DemandQty is null then '0' else convert(varchar, @DemandQty) end + ' 
		,DiscPct    = ' + case when  @DiscPct is null then '0' else convert(varchar, @DiscPct) end + '
		,CostPrice    = ' + case when @CostPrice is null then '0' else convert(varchar, @CostPrice) end + '  
		,RetailPrice   = ' + case when @RetailPrice is null then '0' else convert(varchar, @RetailPrice) end + '  
		,CostPriceMD   = ' + case when @CostPriceMD is null then '0' else convert(varchar, @CostPriceMD) end + '  
		,RetailPriceMD   = ' + case when @RetailPriceMD is null then '0' else convert(varchar, @RetailPriceMD) end + '  
		,RetailPriceInclTaxMD = ' + case when @RetailPriceInclTaxMD is null then '0' else convert(varchar, @RetailPriceInclTaxMD) end + '  
		,[Status]  = ''' + case when (select ServiceStatus from #srv) is null then '''' else (select ServiceStatus from #srv) end + '''  
		,LastupdateBy   = ''' + case when (select LastupdateBy from #srv) is null then '''' else (select LastupdateBy from #srv) end + '''  
		,LastupdateDate = ''' + case when (select LastupdateDate from #srv) is null then '''' else convert(varchar,(select LastupdateDate from #srv)) end + '''  
		where CompanyCode = ''' + case when @CompanyCode is null then '''' else @CompanyCode end + '''
		  and BranchCode = ''' + case when @BranchCode is null then '''' else @BranchCode end + '''
		  and DocNo  = ''' + case when (select JobOrderNo from #srv) is null then '''' else (select JobOrderNo from #srv) end + '''  
		  and PartNo       =  ''' + case when @PartNo is null then '''' else @PartNo end  + '''
		  and PartSeq      = ' + case when @PartSeq is null then '0' else convert(varchar, @PartSeq) end + '';  
		  
		  --print @QueryTemp;  
		exec 	(@QueryTemp);
	end
 end  
 else  
 begin  
	if((select count(*) from svTrnSrvItem  
	where 1 = 1  
	  and CompanyCode  = @CompanyCode  
	  and BranchCode   = @BranchCode  
	  and ProductType  = (select ProductType from #srv)  
	  and ServiceNo    = (select ServiceNo from #srv)  
	  and PartNo       = @PartNo  
	  and (isnull(SupplySlipNo,'') = '')  
	) > 0)  
	begin  
		raiserror ('Part yang sama sudah diproses di Entry SPK namun belum dapat No SSS, mohon diselesaikan dahulu!',16,1);  
	end  

	declare @PartSeqNew as int  
	set @PartSeqNew = (isnull((select isnull(max(PartSeq), 0) + 1    
	  from svTrnSrvItem   
		where CompanyCode = @CompanyCode  
	   and BranchCode  = @BranchCode   
	   and ProductType = @ProductType  
	   and ServiceNo   = @ServiceNo  
	   and PartNo      = @PartNo), 1))  
	     
	-- insert svTrnSrvItem  
	set @QueryTemp=' insert into svTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct, MechanicID)  
	select   
	'''+ @CompanyCode +''' CompanyCode  
	,''' + @BranchCode +''' BranchCode  
	,'''+ @ProductType +''' ProductType  
	,'+ convert(varchar,@ServiceNo) +' ServiceNo  
	,a.PartNo  
	,'''+ convert(varchar,@PartSeqNew)  +'''
	--,(row_number() over (order by a.PartNo)) PartSeq  
	,'+ convert(varchar,@DemandQty )+' DemandQty  
	,''0'' SupplyQty  
	,''0'' ReturnQty  
	,'+ convert(varchar,isnull(@CostPrice,0))  +'
	,a.RetailPrice   
	,b.TypeOfGoods  
	,'''+ @BillType +''' BillType  
	,null SupplySlipNo  
	,null SupplySlipDate  
	,null SSReturnNo  
	,null SSReturnDate  
	,c.LastupdateBy CreatedBy  
	,c.LastupdateDate CreatedDate  
	,c.LastupdateBy  
	,c.LastupdateDate  
	,'+ convert(varchar,isnull(@DiscPct,0))  +'
	,(select MechanicID from svTrnService where CompanyCode = '''+ @CompanyCode +''' and BranchCode = '''+ @BranchCode +''' and ServiceNo = '+ convert(varchar,@ServiceNo) +')  
    from spMstItemPrice a, '+ @DbName +'..spMstItems b, 
    #srv c, gnmstcompanymapping d 
   where 1 = 1  
	 and d.CompanyMd = b.CompanyCode
	 and d.BranchMD = b.BranchCode
        and d.CompanyCode = c.CompanyCode  
     and d.BranchCode  = c.BranchCode  
     and b.PartNo      = a.PartNo  
        and (b.CompanyCode = '''+ @CompanyMD +'''
     and b.BranchCode  = '''+ @BranchMD +'''
     and b.PartNo      = '''+ @PartNo +''')
     and (a.CompanyCode = '''+ @CompanyCode +'''
     and a.BranchCode  = '''+ @BranchCode +'''
     and a.PartNo      = '''+ @PartNo +''')' 
		   
	exec(@QueryTemp)

	--print(@QueryTemp)

	--select   @CostPrice   
	--xxx

	if (@DealerCode = 'SD' and @IsSPK = '2')  
	begin
		create table #tmpSvSDMovement(
			CompanyCode varchar(15)
			,BranchCode varchar(15)
			,JobOrderNo varchar(20)   
			,JobOrderDate datetime  
			,PartNo varchar(20)
			,PartSeqNew int
			,WarehouseMD varchar(20)   
			,DemandQty numeric(18,2)
			,Qty numeric(18,2)
			,DiscPct numeric(18,2)
			,CostPrice numeric(18,2)
			,RetailPrice numeric(18,2) 
			,TypeOfGoods varchar(15) 
			,CompanyMD varchar(15)
			,BranchMD varchar(15)   
			,WarehouseMD2 varchar(15)
			,RetailPriceInclTaxMD numeric(18,2) 
			,RetailPriceMD numeric(18,2) 
			,CostPriceMD numeric(18,2)  
			,QtyFlag char(1)
			,ProductType varchar(15) 
			,ProfitCenterCode varchar(15)
			,Status char(1)
			,ProcessStatus char(1)
			,ProcessDate datetime 
			,CreatedBy varchar(15) 
			,CreatedDate datetime 
			,LastUpdateBy varchar(15) 
			,LastUpdateDate datetime	
		);

		insert into #tmpSvSDMovement 
			select case when @CompanyCode is null then '' else @CompanyCode end 
			,case when @BranchCode is null then '' else @BranchCode end
			,case when (select JobOrderNo from #srv) is null then convert(varchar,@ServiceNo) else (select JobOrderNo from #srv) end
			,case when (select JobOrderDate from #srv) is null then '1900/01/01' else (select JobOrderDate from #srv) end 
			,case when @PartNo is null then '' else  @PartNo end 
			,case when @PartSeqNew is null then '0' else convert(varchar, @PartSeqNew) end
			,case when @WarehouseMD is null then '' else @WarehouseMD end  
			,case when @DemandQty  is null then '0' else convert(varchar, @DemandQty) end
 			,case when @DemandQty  is null then '0' else convert(varchar, @DemandQty) end
			,case when @DiscPct is null then '0' else convert(varchar, @DiscPct) end  
			,case when @CostPrice is null then '0' else convert(varchar, @CostPrice) end 
			,case when @RetailPrice is null then '0' else convert(varchar, @RetailPrice) end  
			,case when @TypeOfGoods is null then '' else @TypeOfGoods end 
			,case when @CompanyMD is null then '' else @CompanyMD end   
			,case when @BranchMD is null then '' else @BranchMD end  
			,case when @WarehouseMD is null then '' else @WarehouseMD end  
			,case when @RetailPriceInclTaxMD is null then '0' else convert(varchar, @RetailPriceInclTaxMD) end  
			,case when @RetailPriceMD is null then '0' else convert(varchar, @RetailPriceMD) end   
			,case when @CostPriceMD is null then '0' else convert(varchar, @CostPriceMD) end
			,'x'
			,case when @ProductType is null then '' else @ProductType end  
			,'300'  
			,'0' 
			,'0'
			,'1900/01/01'  
			,case when (select CreatedBy from #srv) is null then '' else (select CreatedBy from #srv) end     
			,case when (select CreatedDate from #srv) is null then '1900/01/01' else convert(varchar,(select CreatedDate from #srv)) end 
			,case when (select LastUpdateBy from #srv) is null then '' else (select LastUpdateBy from #srv) end
			,case when (select LastUpdateDate from #srv) is null then '1900/01/01' else convert(varchar,(select LastUpdateDate from #srv)) end
		 
		declare @intCountTemp int
		set @intCountTemp = (select count(isnull(JobOrderNo,'')) DocNo from #tmpSvSDMovement)
		if (@intCountTemp > 0 ) begin 
			declare @intStringEmpty int
			set @intStringEmpty = (select count(isnull(JobOrderNo,'')) DocNo from #tmpSvSDMovement where JobOrderNo = '' or JobOrderNo is null)
			select @intCountTemp
			select @intStringEmpty
			if (@intStringEmpty < 1) begin
				set @QueryTemp = '
					insert into ' + @DbName + '..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice,   
					TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, 
					Status, ProcessStatus, ProcessDate, CreatedBy,   
					CreatedDate, LastUpdateBy, LastUpdateDate)  
					select * from #tmpSvSDMovement';
				exec(@QueryTemp);
			end
		end
		 
		drop table #tmpSvSDMovement;     
	end   
 end  

 update svTrnSrvItem  
    set DiscPct = @DiscPct  
  where 1 = 1  
    and CompanyCode = @CompanyCode  
    and BranchCode = @BranchCode  
    and ProductType = @ProductType  
    and ServiceNo = @ServiceNo  
    and PartNo = @PartNo  
   
 if (@DealerCode = 'SD' and @IsSPK = '2')  
 begin    
	set @QueryTemp = 'update ' + @DbName + '..svSDMovement   
	  set DiscPct = ' + convert(varchar,@DiscPct) 
	  + ' where 1 = 1'  
	  +	' and CompanyCode = ''' + case when @CompanyMD is null then '''' else  @CompanyMD end + ''''
	  + ' and BranchCode = ''' + case when @BranchMD is null then '''' else  @BranchMD end + ''''
	  + ' and DocNo = ''' + case when (select JobOrderNo from #srv) is null then '''' else (select JobOrderNo from #srv) end  + ''''
	  + ' and PartNo = ''' + case when @PartNo is null then '''' else @PartNo end + ''''  
	  + ' and PartSeq = ' + convert(varchar,@PartSeq) + '';
  
	exec (@QueryTemp)  
 end  
   
	drop table #srv  
end try  
begin catch  
 set @errmsg = error_message()  
 raiserror (@errmsg,16,1);  
end catch  

--rollback tran

GO

if object_id('uspfn_SvTrnInvoiceCreate') is not null
	drop procedure uspfn_SvTrnInvoiceCreate
GO

CREATE procedure [dbo].[uspfn_SvTrnInvoiceCreate]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@ServiceNo   int,
	@BillType    char(1),
	@InvoiceNo   varchar(15),
	@Remarks     varchar(max),
	@UserID      varchar(15)
as  

declare @errmsg varchar(max)
--raiserror ('test error',16,1);

DECLARE @CompanyMD AS VARCHAR(15)
DECLARE @BranchMD AS VARCHAR(15)
DECLARE @WarehouseMD AS VARCHAR(15)
DECLARE @DbMD AS VARCHAR(15)
declare @md bit

SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @WarehouseMD = (SELECT WarehouseMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
SET @DbMD = (SELECT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)

select BillType as BillType
              from svTrnSrvTask
             where CompanyCode = @companycode
               and BranchCode  = @branchcode
               and ProductType = @productType
               and ServiceNo   = @serviceno
            union
            select BillType as BillType
              from svTrnSrvItem b
             where CompanyCode = @companycode
               and BranchCode  = @branchcode
               and ProductType = @productType
               and ServiceNo   = @serviceno
               and  (SupplyQty - ReturnQty) > 0


-- get data from service
select * into #srv from(
  select * from svTrnService
   where 1 = 1
     and CompanyCode = @CompanyCode
     and BranchCode  = @BranchCode
     and ProductType = @ProductType
     and ServiceNo   = @ServiceNo
 )#srv

 select * from #srv
 select * from svTrnSrvItem where serviceno = @serviceno
 select * from svTrnSrvTask where serviceno = @serviceno

-- get data from task
select * into #tsk from(
  select a.* from svTrnSrvTask a, #srv b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.BillType    = @BillType
 )#tsk

 select * from #tsk

-- get data from item
select * into #mec from(
  select a.* from svTrnSrvMechanic a, #tsk b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.OperationNo = b.OperationNo
     and a.OperationNo <> ''
 )#mec

 select * from #mec

-- get data from item
select * into #itm from(
  select a.* from svTrnSrvItem a, #srv b
   where 1 = 1
     and a.CompanyCode = b.CompanyCode
     and a.BranchCode  = b.BranchCode
     and a.ProductType = b.ProductType
     and a.ServiceNo   = b.ServiceNo
     and a.BillType    = @BillType
 )#itm

-- create temporary table detail
create table #pre_dtl(
	BillType char(1),
	TaskPartType char(1),
	TaskPartNo varchar(20),
	TaskPartQty numeric(10,2),
	SupplySlipNo varchar(20)
)

insert into #pre_dtl
select BillType, 'L', OperationNo, OperationHour, ''
  from #tsk

insert into #pre_dtl
select BillType, TypeOfGoods, PartNo
	 , sum(SupplyQty - ReturnQty)
	 , SupplySlipNo
  from #itm
 where BillType = @BillType
   and (SupplyQty - ReturnQty) > 0
 group by BillType, TypeOfGoods, PartNo, SupplySlipNo

-- insert to table svTrnInvoice
declare @CustomerCode varchar(20)
if @BillType = 'C'
begin
	set @CustomerCode = (select CustomerCodeBill from #srv)
end
else if @BillType = 'P'
begin
	set @CustomerCode = (select top 1 a.BillTo from svMstPackage a
				 inner join svMstPackageTask b
					on b.CompanyCode = a.CompanyCode
				   and b.PackageCode = a.PackageCode
				 inner join svMstPackageContract c
					on c.CompanyCode = a.CompanyCode
				   and c.PackageCode = a.PackageCode
				 inner join #srv d
					on d.CompanyCode = a.CompanyCode
				   and d.JobType = a.JobType
				   and d.ChassisCode = c.ChassisCode
				   and d.ChassisNo = c.ChassisNo)
end
else if @BillType in ('F', 'W', 'S')
begin
	set @CustomerCode = (select CustomerCode from svMstBillingType
				 where BillType in ('F', 'W', 'S')
				   and CompanyCode = @CompanyCode
				   and BillType = @BillType)
end
else
begin
	set @CustomerCode = (select CustomerCodeBill from #srv)
end

--set @CustomerCode = isnull((
--				select top 1 a.BillTo from svMstPackage a
--				 inner join svMstPackageTask b
--					on b.CompanyCode = a.CompanyCode
--				   and b.PackageCode = a.PackageCode
--				 inner join svMstPackageContract c
--					on c.CompanyCode = a.CompanyCode
--				   and c.PackageCode = a.PackageCode
--				 inner join #srv d
--					on d.CompanyCode = a.CompanyCode
--				   and d.JobType = a.JobType
--				   and d.ChassisCode = c.ChassisCode
--				   and d.ChassisNo = c.ChassisNo
--				), isnull((
--				select CustomerCode from svMstBillingType
--				 where BillType in ('F')
--				   and CompanyCode = @CompanyCode
--				   and BillType = @BillType
--				), isnull((select CustomerCodeBill from #srv), '')))


if ((select count(*) from #tsk) = 0 and (select count(*) from #itm) = 0)
begin
	drop table #srv
	drop table #tsk
	drop table #mec
	drop table #itm
	drop table #pre_dtl
	return
end

if (@CustomerCode = '')
begin
	set @errmsg = N'Customer Code Bill belum di define...'
				+ char(13) + 'Tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

select * into #cus from (
select a.CompanyCode, a.IsPkp, b.CustomerCode, b.LaborDiscPct, b.PartDiscPct, b.MaterialDiscPct, b.TopCode, b.TaxCode
  from gnMstCustomer a, gnMstCustomerProfitCenter b
 where 1 = 1
   and b.CompanyCode  = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
   and b.CompanyCode  = @CompanyCode
   and b.BranchCode   = @BranchCode
   and b.CustomerCode = @CustomerCode
   and b.ProfitCenterCode = '200'
)#cus

if (select count(*) from #cus) <> 1
begin
	set @errmsg = N'Customer ProfitCenter belum di define...'
				+ char(13) + 'Tolong di check lagi'
				+ char(13) + 'Terima kasih'
	raiserror (@errmsg,16,1);
	return
end

declare @IsPKP bit
    set @IsPKP = isnull((
				 select IsPKP from gnMstCustomer
				  where CompanyCode  = @CompanyCode
				    and CustomerCode = @CustomerCode
				  ), 0)

declare @PPnPct decimal
    set @PPnPct = isnull((
				  select a.TaxPct
				    from gnMstTax a, #cus b
				   where 1 = 1
				     and b.TaxCode     = 'PPN'
				     and a.CompanyCode = b.CompanyCode
				     and a.TaxCode     = b.TaxCode
				  ), 0)

declare @PPhPct decimal
    set @PPhPct = isnull((
				  select a.TaxPct
				    from gnMstTax a, #cus b
				   where 1 = 1
				     and b.TaxCode     = 'PPH'
				     and a.CompanyCode = b.CompanyCode
				     and a.TaxCode     = b.TaxCode
				  ), 0)


-- Insert Into svTrnInvoice
-----------------------------------------------------------------------------------------
insert into svTrnInvoice(
  CompanyCode, BranchCode, ProductType
, InvoiceNo, InvoiceDate, InvoiceStatus
, FPJNo, FPJDate, JobOrderNo, JobOrderDate, JobType
, ServiceRequestDesc, ChassisCode, ChassisNo, EngineCode, EngineNo
, PoliceRegNo, BasicModel, CustomerCode, CustomerCodeBill, Odometer
, IsPKP, TOPCode, TOPDays, DueDate, SignedDate
, LaborDiscPct, PartsDiscPct, MaterialDiscPct, PphPct, PpnPct, Remarks
, PrintSeq, PostingFlag, IsLocked, CreatedBy, CreatedDate
) 
select
  @CompanyCode CompanyCode
, @BranchCode BranchCode
, @ProductType ProductType
, @InvoiceNo InvoiceNo
, getdate() InvoiceDate
, case @IsPKP
	when '0' then '1'
	else (case @BillType when 'F' then '0' when 'W' then '0' else '1' end)
  end as InvoiceStatus
, '' FPJNo
, null FPJDate
, (select JobOrderNo from #srv) JobOrderNo
, (select JobOrderDate from #srv) JobOrderDate
, (select JobType from #srv) JobType
, (select ServiceRequestDesc from #srv) ServiceRequestDesc
, (select ChassisCode from #srv) ChassisCode
, (select ChassisNo from #srv) ChassisNo
, (select EngineCode from #srv) EngineCode
, (select EngineNo from #srv) EngineNo
, (select PoliceRegNo from #srv) PoliceRegNo
, (select BasicModel from #srv) BasicModel
, (select CustomerCode from #srv) CustomerCode
, @CustomerCode as CustomerCodeBill
, (select Odometer from #srv) Odometer
, (select IsPKP from #cus) as IsPKP
, (select TopCode from #cus) as TOPCode
, isnull((
	select b.ParaValue
	  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
	 where a.CompanyCode  = @CompanyCode
	   and a.BranchCode   = @BranchCode
	   and a.CustomerCode = @CustomerCode
	   and a.ProfitCenterCode = '200'
	   and b.CompanyCode  = a.CompanyCode
	   and b.CodeID = 'TOPC'
	   and b.LookUpValue = a.TopCode
	), null) as TOPDays
, isnull((
	select dateadd(day, convert(int,b.ParaValue), convert(varchar, getdate(), 112))
	  from gnMstCustomerProfitCenter a, GnMstLookUpDtl b
	 where a.CompanyCode  = @CompanyCode
	   and a.BranchCode   = @BranchCode
	   and a.CustomerCode = @CustomerCode
	   and a.ProfitCenterCode = '200'
	   and b.CompanyCode  = a.CompanyCode
	   and b.CodeID = 'TOPC'
	   and b.LookUpValue  = a.TopCode
	), null) as DueDate
, convert(varchar, getdate(), 112) SignedDate
, case @BillType
	when 'F' then (select LaborDiscPct from #cus) 
    when 'W' then (select LaborDiscPct from #cus) 
    else (select LaborDiscPct from #srv) 
  end as LaborDiscPct
, case @BillType
	when 'F' then (select PartDiscPct from #cus) 
    when 'W' then (select PartDiscPct from #cus) 
    else (select PartDiscPct from #srv) 
  end as PartsDiscPct
, case @BillType
	when 'F' then (select MaterialDiscPct from #cus) 
    when 'W' then (select MaterialDiscPct from #cus) 
    else (select MaterialDiscPct from #srv) 
  end as MaterialDiscPct
, @PPnPct as PPhPct
, @PPnPct as PPnPct
, @Remarks as Remarks
, '0' PrintSeq
, '0' PostingFlag
, '0' IsLocked
, @UserID CreatedBy
, getdate() CreatedDate

-- Insert Into svTrnInvTask
-----------------------------------------------------------------------------------------
insert into svTrnInvTask (
  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
, OperationHour, ClaimHour, OperationCost, SubConPrice
, IsSubCon, SharingTask, DiscPct
)
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
, isnull(OperationHour, 0) OperationHour, isnull(ClaimHour, 0) ClaimHour
, isnull(OperationCost, 0) OperationCost, isnull(SubConPrice, 0) SubConPrice
, isnull(IsSubCon, 0) IsSubCon, isnull(SharingTask, 0) SharingTask
, isnull(DiscPct, 0)
from #tsk

-- Insert Into svTrnInvMechanic
-----------------------------------------------------------------------------------------
insert into svTrnInvMechanic (
  CompanyCode, BranchCode, ProductType, InvoiceNo, OperationNo
, MechanicID, ChiefMechanicID, StartService, FinishService
)
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, OperationNo
, MechanicID, ChiefMechanicID, StartService, FinishService
from #mec

-- Insert Into svTrnInvItem
-----------------------------------------------------------------------------------------
Declare @Query varchar(max)

set @Query = 'select * into #itm1 from (
select CompanyCode, BranchCode, ProductType, '''+ @InvoiceNo +''' as InvoiceNo, PartNo
	 , isnull((
		select MovingCode from '+ @DbMD +'..spMstItems
		 where CompanyCode = '''+ @CompanyMD +'''
		   and BranchCode = '''+ @BranchMD +'''
		   and PartNo = #itm.PartNo
		), '''') as MovingCode
	 , isnull((
		select ABCClass from '+ @DbMD +' ..spMstItems
		 where CompanyCode = '''+ @CompanyMD +'''
		   and BranchCode = '''+ @BranchMD +'''
		   and PartNo = #itm.PartNo
		), '''') as ABCClass
	 , sum(SupplyQty - ReturnQty) as SupplyQty
	 , isnull((
		select 
		  case (sum(b.SupplyQty - b.ReturnQty))
			 when 0 then 0
			 else sum(a.CostPrice * (b.SupplyQty - b.ReturnQty)) / sum(b.SupplyQty - b.ReturnQty)
		  end 
	from SpTrnSLmpDtl a
	left join SvTrnSrvItem b on 1 = 1
	 and b.CompanyCode  = a.CompanyCode
	 and b.BranchCode   = a.BranchCode
	 and b.ProductType  = a.ProductType
	 and b.SupplySlipNo = a.DocNo
	 and b.PartNo = #itm.PartNo
	where 1 = 1
	 and a.CompanyCode = '''+ @CompanyCode +'''
	 and a.BranchCode  = '''+ @BranchCode +'''
	 and a.ProductType = '''+ @ProductType +'''
	 and a.PartNo = #itm.PartNo
	 and a.DocNo in (
			select SupplySlipNo
			 from SvTrnSrvItem
			where 1 = 1
			  and CompanyCode = '''+ @CompanyCode +'''
			  and BranchCode  = '''+ @BranchCode +'''
			  and ProductType = '''+ @ProductType +'''
			  and ServiceNo = '''+ Convert(varchar,@ServiceNo) +'''
			  and PartNo = #itm.PartNo
			)
	), 0) as CostPrice
, RetailPrice
, TypeOfGoods
from #itm
group by CompanyCode, BranchCode, ProductType, PartNo, RetailPrice, TypeOfGoods
)#

insert into svTrnInvItem (
  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo
, MovingCode, ABCClass, SupplyQty, ReturnQty, CostPrice, RetailPrice
, TypeOfGoods, DiscPct
)
select a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo
	 , MovingCode = (select top 1 MovingCode from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , ABCClass = (select top 1 ABCClass from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , sum(SupplyQty) as SupplyQty, 0 as ReturnQty
	 , CostPrice = (select top 1 CostPrice from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by CostPrice desc)
	 , RetailPrice = (select top 1 RetailPrice from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo order by RetailPrice desc)
	 , TypeOfGoods = (select top 1 TypeOfGoods from #itm1 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
	 , DiscPct = (select top 1 DiscPct from #itm where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InvoiceNo = a.InvoiceNo and PartNo = a.PartNo)
  from #itm1 a
 where a.SupplyQty > 0
 group by a.CompanyCode, a.BranchCode, a.ProductType, a.InvoiceNo, a.PartNo'

 exec(@Query)

-- Insert Into svTrnInvItemDtl
-----------------------------------------------------------------------------------------
insert into svTrnInvItemDtl (
  CompanyCode, BranchCode, ProductType, InvoiceNo, PartNo, SupplySlipNo
, SupplyQty, CostPrice, CreatedBy, CreatedDate
)
select y.* from (
select CompanyCode, BranchCode, ProductType, @InvoiceNo as InvoiceNo, PartNo, SupplySlipNo
, sum(SupplyQty - ReturnQty) as SupplyQty, CostPrice
, @UserID as CreatedBy, getdate() as CreatedDate
from #itm
group by CompanyCode, BranchCode, ProductType, PartNo, SupplySlipNo, CostPrice
) y
where y.SupplyQty > 0

-- Re Calculate Invoice

-----------------------------------------------------------------------------------------
exec uspfn_SvTrnInvoiceReCalculate @CompanyCode=@CompanyCode, @BranchCode=@BranchCode, @ProductType=@ProductType, @InvoiceNo=@InvoiceNo, @UserId=@UserId
-- Insert svsdmovement
-----------------------------------------------------------------------------------------

 if(@md = 0)
 begin

 set @Query ='insert into '+ @DbMD +'..svSDMovement
select a.CompanyCode, a.BranchCode, a.InvoiceNo,a.InvoiceDate, b.PartNo
, Seq = convert(integer, ROW_NUMBER() OVER (PARTITION BY b.PartNo order by b.PartNo)) ,
''00'', b.SupplyQty, b.SupplyQty, b.DiscPct, b.CostPrice, b.RetailPrice, b.TypeOfGoods
, '''+ @CompanyMD +''','''+ @BranchMD +''','''+ @WarehouseMD +''', p.RetailPriceInclTax, p.RetailPrice, p.CostPrice
,''x'','''+ @producttype +''',''300'',''8'',''0'','''+ convert(varchar,GETDATE()) +''','''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
,'''+ @UserID +''','''+ convert(varchar,GETDATE()) +'''
from svTrnInvoice a
join svTrnInvItem b on a.CompanyCode = b.CompanyCode and a.BranchCode =  b.BranchCode and a.ProductType = b.ProductType and a.InvoiceNo = b.InvoiceNo 
join '+ @DbMD +'..spmstitemprice p
on p.CompanyCode = '''+ @CompanyMD +''' and p.BranchCode = '''+ @BranchMD +''' and p.PartNo = b.PartNo
where a.CompanyCode = '''+ @CompanyCode +'''
and a.branchcode = '''+ @BranchCode +'''
and a.InvoiceNo = '''+ convert(varchar,@InvoiceNo) +''''

exec (@Query)

end

drop table #srv
drop table #tsk
drop table #mec
drop table #itm
drop table #cus

drop table #pre_dtl
--rollback tran

GO

if object_id('uspfn_spMasterPartLookup') is not null
	drop procedure uspfn_spMasterPartLookup
GO
CREATE PROCEDURE [dbo].[uspfn_spMasterPartLookup]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@TypeOfGoods varchar(15),
@ProductType varchar(15),
@SEARCH varchar(50) = ''
AS

--declare @CompanyCode varchar(15),
--		@BranchCode varchar(15),
--		@TypeOfGoods varchar(15),
--		@ProductType varchar(15),
--		@SEARCH varchar(50) = ''

--set @CompanyCode = '6115204'
--set @BranchCode = '611520401'
--set @TypeOfGoods = '0'
--set @ProductType = '2W'
--set @SEARCH = ''

IF (@SEARCH='')
SELECT DISTINCT
 ItemInfo.PartNo
,ItemInfo.ProductType
,(SELECT LookupValueName 
	FROM gnMstLookupDtl 
   WHERE CodeID = 'PRCT' AND 
		 LookUpValue = ItemInfo.PartCategory AND 
		 CompanyCode = @CompanyCode) AS CategoryName
,ItemInfo.PartCategory
,ItemInfo.PartName
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
,CASE Items.Status WHEN 1 THEN 'Aktif' ELSE 'Tidak' END AS IsActive
,ItemInfo.OrderUnit
,ItemInfo.SupplierCode
,Supplier.SupplierName
,(SELECT LookupValueName 
	FROM gnMstLookupDtl 
  WHERE CodeID = 'TPGO' AND 
		LookUpValue = Items.TypeOfGoods AND 
		CompanyCode = @CompanyCode) AS TypeOfGoods
FROM SpMstItemInfo ItemInfo
INNER JOIN SpMstItems Items    ON ItemInfo.CompanyCode = Items.CompanyCode AND ItemInfo.PartNo = Items.PartNo
INNER JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
						 AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE ItemInfo.CompanyCode = @CompanyCode AND ItemInfo.ProductType = @ProductType

ELSE

SELECT DISTINCT
 ItemInfo.PartNo
,ItemInfo.ProductType
,(SELECT LookupValueName 
	FROM gnMstLookupDtl 
   WHERE CodeID = 'PRCT' AND 
		 LookUpValue = ItemInfo.PartCategory AND 
		 CompanyCode = @CompanyCode) AS CategoryName
,ItemInfo.PartCategory
,ItemInfo.PartName
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN 'Ya' ELSE 'Tidak' END AS IsGenuinePart
,CASE Items.Status WHEN 1 THEN 'Aktif' ELSE 'Tidak' END AS IsActive
,ItemInfo.OrderUnit
,ItemInfo.SupplierCode
,Supplier.SupplierName
,(SELECT LookupValueName 
	FROM gnMstLookupDtl 
  WHERE CodeID = 'TPGO' AND 
		LookUpValue = Items.TypeOfGoods AND 
		CompanyCode = @CompanyCode) AS TypeOfGoods
FROM SpMstItemInfo ItemInfo
INNER JOIN SpMstItems Items    ON ItemInfo.CompanyCode = Items.CompanyCode AND ItemInfo.PartNo = Items.PartNo
INNER JOIN GnMstSupplier Supplier ON Supplier.CompanyCode  = Items.CompanyCode 
						 AND Supplier.SupplierCode = ItemInfo.SupplierCode
WHERE ItemInfo.CompanyCode = @CompanyCode AND ItemInfo.ProductType = @ProductType
  AND (ItemInfo.PartNo LIKE @SEARCH + '%')

GO

if object_id('GetBranchMD') is not null
	drop FUNCTION GetBranchMD
GO
CREATE FUNCTION [dbo].[GetBranchMD] (@CompanyCode VARCHAR(15),@BranchCode VARCHAR(15)) 
RETURNS VARCHAR (20)
AS 
BEGIN
	DECLARE @BranchMD AS VARCHAR(15)
	SET @BranchMD = (SELECT BranchMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
	if (@BranchMD is null) begin
		SET @BranchMD = (SELECT DISTINCT BranchMD FROM gnMstCompanyMapping WHERE CompanyMD = @CompanyCode AND BranchMD = @BranchCode)
	end

	RETURN @BranchMD
END

GO

if object_id('GetCompanyMD') is not null
	drop FUNCTION GetCompanyMD
GO
CREATE FUNCTION [dbo].[GetCompanyMD] (@CompanyCode VARCHAR(15),@BranchCode VARCHAR(15)) 
RETURNS VARCHAR (20)
AS 
BEGIN
	DECLARE @CompanyMD AS VARCHAR(15)
	SET @CompanyMD = (SELECT CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)

	if(@CompanyMD is null) begin
		SET @CompanyMD = (SELECT DISTINCT CompanyMD FROM gnMstCompanyMapping WHERE CompanyMD = @CompanyCode AND BranchMD = @BranchCode)
	end

	RETURN @CompanyMD
END
GO

if object_id('GetDbMD') is not null
	drop FUNCTION GetDbMD
GO
CREATE FUNCTION [dbo].[GetDbMD] (@CompanyCode VARCHAR(15),@BranchCode VARCHAR(15)) 
RETURNS VARCHAR (20)
AS 
BEGIN
	DECLARE @DbMD varchar(20);
	SET @DbMD = (SELECT DISTINCT DbMD FROM gnMstCompanyMapping WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode)
	IF(@DbMD is null) begin
		SET @DbMD = (SELECT DISTINCT DbMD FROM gnMstCompanyMapping WHERE CompanyMD = @CompanyCode AND BranchMD = @BranchCode)
	END
	
	RETURN @DbMD
END
GO

if object_id('usprpt_SpRpRgs017') is not null
	drop procedure usprpt_SpRpRgs017
GO
CREATE procedure [dbo].[usprpt_SpRpRgs017]
 @CompanyCode VARCHAR(15),  
 @BranchCode VARCHAR(15),   
 @StartDate VARCHAR(30),  
 @EndDate VARCHAR(30),  
 @TransType VARCHAR(2),  
 @TypeOfGoods VARCHAR(1)   
AS  
  

select a.CompanyCode, a.BranchCode
     , a.FPJNo, a.PickingSlipNo
     , replace(convert(varchar, a.FPJDate, 106), ' ', '-') as FPJDate
     , a.TypeOfGoods
     , OrderNoFpj = isnull((
                   select top 1 ReferenceNo
                     from spTrnSFpjDtl
                    where CompanyCode = a.CompanyCode
                      and BranchCode = a.BranchCode
                      and FpjNo = b.FpjNo
                    order by CreatedDate desc
                     ), '')
     , OrderNo = isnull((
                   select top 1 OrderNo
                     from spTrnSORDHdr
                    where CompanyCode = a.CompanyCode
                      and BranchCode = a.BranchCode
                      and DocNo = b.DocNo
                    order by CreatedDate desc
                     ), '')
     , isnull(c.CustomerName, '-') as CustomerName
     , b.PartNo
     , (select PartName from SpMstItemInfo 
         where CompanyCode = a.CompanyCode
           and PartNo = b.PartNo) as PartName
     , b.QtyBill
     , b.RetailPrice * b.QtyBill as HargaJualKotor
     , b.DiscPct
     , b.DiscAmt
     , b.NetSalesAmt
     , a.TotPPNAmt
     , a.TotFinalSalesAmt
     , b.CostPrice * QtyBill as HargaPokok
  from SpTrnSFPJHdr a
 inner join SpTrnSFPJDtl b
    on a.CompanyCode = b.CompanyCode
   and a.BranchCode = b.BranchCode  
   and a.FPJNo = b.FPJNo  
  left join GnMstCustomer c
    on a.CompanyCode = c.CompanyCode  
   and a.CustomerCode = c.CustomerCode  
 where 1 = 1 
   and a.CompanyCode = @CompanyCode
   and a.BranchCode = @BranchCode
   --and a.Transtype like @TransType
   and a.Transtype = (case rtrim(isnull(@Transtype, '')) when '' then a.Transtype else @Transtype end)
   and convert(varchar, a.FPJDate, 112) 
	between convert(varchar, convert(datetime,@StartDate ), 112) 
	    and convert(varchar,convert(datetime,@EndDate ), 112)  
   and a.TypeOfGoods = (case rtrim(isnull(@TypeOfGoods, '')) when '' then a.TypeOfGoods else @TypeOfGoods end)
 order by a.CreatedDate, a.FPJNo asc   
 
GO
if object_id('uspfn_omSlsInvLkpBPK') is not null
	drop procedure uspfn_omSlsInvLkpBPK
GO
CREATE procedure [dbo].[uspfn_omSlsInvLkpBPK]   
--declare
 @CompanyCode varchar(15),  
 @BranchCode varchar(15),
 @SONo varchar(15),
 @INVDate datetime
AS  
BEGIN  
 --exec [uspfn_omSlsInvLkpBPK] '6115204001','6115204502','SO507/15/000153','20150522'
 --select @CompanyCode= '6115204001',
	--	@BranchCode= '6115204507',
	--	@SONo= 'SO507/15/000153',
	--	@INVDate= '20150522'
 
SELECT distinct a.BPKNo,a.BPKDate,a.DONo,a.SONo,a.BPKDate
                  FROM omTrSalesBPK a inner join omTrSalesBPKDetail b
	                on a.companyCode = b.companyCode and a.branchCode = b.branchCode and a.BPKNo = b.BPKNo
                 WHERE a.CompanyCode = @CompanyCode
                       AND a.BranchCode = @BranchCode
                       --AND MONTH(a.BPKDate) = @INVDate
                        AND CONVERT(varchar,a.BPKDate,112) <= CONVERT(varchar, @INVDate,112)
                        AND b.StatusInvoice = '0'
                        AND a.Status = '2'
                        AND SONO = @SONo
                ORDER BY a.BPKNo
END
GO

if object_id('usprpt_OmRpSalesTrn009') is not null
	drop procedure usprpt_OmRpSalesTrn009
GO
CREATE procedure [dbo].[usprpt_OmRpSalesTrn009] 
	@CompanyCode	VARCHAR(15),
	@BranchCode		VARCHAR(15),
	@FirstInvoiceNo	VARCHAR(15),
	@LastInvoiceNo	VARCHAR(15)
AS
BEGIN

----usprpt_OmRpSalesTrn009 '6078401','607840102','ICU/11/000571','ICU/11/000571'

--DECLARE	@CompanyCode	VARCHAR(15),
--		@BranchCode		VARCHAR(15),
--		@FirstInvoiceNo	VARCHAR(15),
--		@LastInvoiceNo	VARCHAR(15)

--SET	@CompanyCode	= '6115204001'
--SET	@BranchCode		= '6115204502'
--SET	@FirstInvoiceNo	= 'IU502/15/000238'
--SET	@LastInvoiceNo	= 'IU502/15/000238'

SELECT DISTINCT
gab.CustomerCode
, gab.CustomerName
, gab.Address1
, gab.Address2
, gab.CityAndZipNo
, gab.NPWPNo
, gab.DNNo
, gab.DNDate
, gab.InvoiceNo
, gab.SONo
, gab.SKPKNo
, gab.RefferenceNo
, gab.BPKNo
, gab.Chassis
, gab.AccNo
, gab.CityName
, gab.SignName
, gab.TitleSign
, sum(Total) as Total
 FROM (
	SELECT DISTINCT
		a.CustomerCode
		, d.CustomerName
		, d.Address1  as Address1, CASE WHEN d.Address2 + ' ' + d.Address3 = '' THEN '-------' ELSE d.Address2 + ' ' + d.Address3  END as Address2
		, (SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND LookUpValue = d.CityCode 
			AND CodeID = 'CITY') + ' ' + d.ZipNo CityAndZipNo
		, d.NPWPNo
		, a.DNNo + case pso.SalesType when '0' then '-W' when '1' then '-D' end as DNNo, DNDate
		, a.InvoiceNo
		, a.SONo, pso.SKPKNo, pso.RefferenceNo
		, '' AS BPKNo
		, 'BBN   No. Chs. : ' + CONVERT(VARCHAR, b.ChassisNo) + '/' + CONVERT(VARCHAR, b.EngineNo) + ' (' + c.BPKNo + ')' as Chassis
		, (SELECT BBNAccNo FROM omMstModelAccount WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
			AND SalesModelCode = c.SalesModelCode) AccNo
		, ISNULL(b.BBN, 0) Total
		, (SELECT LookUpValueName FROM gnMstLookUpDtl dtl
			LEFT JOIN gnMstCoProfile pf ON dtl.CompanyCode = pf.CompanyCode 
			AND dtl.LookUpValue = pf.CityCode AND dtl.CodeID = 'CITY' 
			WHERE dtl.CompanyCode = a.CompanyCode AND pf.BranchCode = a.BranchCode) CityName
		, (SELECT SignName FROM gnMstSignature WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND
			ProfitCenterCode = '100' AND DocumentType = 'DNU' AND SeqNo = 1) SignName
		, (SELECT TitleSign FROM gnMstSignature WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND
			ProfitCenterCode = '100' AND DocumentType = 'DNU' AND SeqNo = 1) TitleSign
	FROM
		omTrSalesDN a WITH(NOLOCK, NOWAIT)
		LEFT JOIN omTrSalesDNVin b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.DNNo = b.DNNo
		LEFT JOIN omTrSalesInvoiceVIN c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode and a.InvoiceNo=c.InvoiceNo
			AND c.ChassisCode = b.ChassisCode AND c.ChassisNo = b.ChassisNo
		LEFT JOIN gnMstCustomer d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
			AND a.CustomerCode = d.CustomerCode
		LEFT JOIN omTrSalesSO pso
			ON a.CompanyCode = pso.CompanyCode
			AND a.BranchCode = pso.BranchCode
			AND a.SONo = pso.SONo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND c.InvoiceNo BETWEEN @FirstInvoiceNo AND @LastInvoiceNo
		AND ISNULL(b.BBN, 0) > 0
	UNION ALL
	SELECT
		a.CustomerCode
		, d.CustomerName
		, d.Address1  as Address1, CASE WHEN d.Address2 + ' ' + d.Address3 = '' THEN '-------' ELSE d.Address2 + ' ' + d.Address3 END as Address2
		, (SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND LookUpValue = d.CityCode 
			AND CodeID = 'CITY') + ' ' + d.ZipNo CityAndZipNo
		, d.NPWPNo
		, a.DNNo + case pso.SalesType when '0' then '-W' when '1' then '-D' end as DNNo, DNDate
		, a.InvoiceNo
		, a.SONo, pso.SKPKNo, pso.RefferenceNo
		, '' AS BPKNo
		, 'KIR   No. Chs. : ' + CONVERT(VARCHAR, b.ChassisNo) + '/' + CONVERT(VARCHAR, b.EngineNo) + ' (' + c.BPKNo + ')' as Chassis
		, (SELECT KIRAccNo FROM omMstModelAccount WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
			AND SalesModelCode = c.SalesModelCode) AccNo
		, ISNULL(b.KIR, 0) Total
		, (SELECT LookUpValueName FROM gnMstLookUpDtl dtl
			LEFT JOIN gnMstCoProfile pf ON dtl.CompanyCode = pf.CompanyCode 
			AND dtl.LookUpValue = pf.CityCode AND dtl.CodeID = 'CITY' 
			WHERE dtl.CompanyCode = a.CompanyCode AND pf.BranchCode = a.BranchCode) CityName
		, (SELECT SignName FROM gnMstSignature WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND
			ProfitCenterCode = '100' AND DocumentType = 'DNU' AND SeqNo = 1) SignName
		, (SELECT TitleSign FROM gnMstSignature WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND
			ProfitCenterCode = '100' AND DocumentType = 'DNU' AND SeqNo = 1) TitleSign
	FROM
		omTrSalesDN a WITH(NOLOCK, NOWAIT)
		LEFT JOIN omTrSalesDNVin b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.DNNo = b.DNNo
		LEFT JOIN omTrSalesInvoiceVIN c ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode and a.InvoiceNo=c.InvoiceNo
			AND c.ChassisCode = b.ChassisCode AND c.ChassisNo = b.ChassisNo
		LEFT JOIN gnMstCustomer d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
			AND a.CustomerCode = d.CustomerCode
		LEFT JOIN omTrSalesSO pso
			ON a.CompanyCode = pso.CompanyCode
			AND a.BranchCode = pso.BranchCode
			AND a.SONo = pso.SONo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND c.InvoiceNo BETWEEN @FirstInvoiceNo AND @LastInvoiceNo
		AND ISNULL(b.KIR, 0) > 0
	UNION ALL
	SELECT
		a.CustomerCode
		, d.CustomerName
		, d.Address1 as Address1, CASE WHEN d.Address2 + ' ' + d.Address3 = ''  THEN '-------' ELSE d.Address2 + ' ' + d.Address3 END as Address2
		, (SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND LookUpValue = d.CityCode 
			AND CodeID = 'CITY') + ' ' + d.ZipNo CityAndZipNo
		, d.NPWPNo
		, a.DNNo + case pso.SalesType when '0' then '-W' when '1' then '-D' end as DNNo, DNDate
		, a.InvoiceNo
		, a.SONo, pso.SKPKNo, pso.RefferenceNo
		, '' AS BPKNo
		, 'Ship Amount (' + c.BPKNo + ')' AS Chassis
		, (SELECT ShipAccNo FROM omMstModelAccount WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
			AND SalesModelCode = c.SalesModelCode) AccNo
		, ISNULL(b.ShipAmt, 0) * c.Quantity Total
		, (SELECT LookUpValueName FROM gnMstLookUpDtl dtl
			LEFT JOIN gnMstCoProfile pf ON dtl.CompanyCode = pf.CompanyCode 
			AND dtl.LookUpValue = pf.CityCode AND dtl.CodeID = 'CITY' 
			WHERE dtl.CompanyCode = a.CompanyCode AND pf.BranchCode = a.BranchCode) CityName
		, (SELECT SignName FROM gnMstSignature WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND
			ProfitCenterCode = '100' AND DocumentType = 'DNU' AND SeqNo = 1) SignName
		, (SELECT TitleSign FROM gnMstSignature WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND
			ProfitCenterCode = '100' AND DocumentType = 'DNU' AND SeqNo = 1) TitleSign
	FROM
		omTrSalesDN a WITH(NOLOCK, NOWAIT)
		LEFT JOIN omTrSalesDNModel b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.DNNo = b.DNNo
		LEFT JOIN omTrSalesInvoiceModel c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
			AND a.BranchCode = c.BranchCode
			AND a.InvoiceNo = c.InvoiceNo
			AND c.SalesModelCode = b.SalesModelCode
			AND c.SalesModelYear = b.SalesModelYear
		LEFT JOIN gnMstCustomer d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
			AND a.CustomerCode = d.CustomerCode
		LEFT JOIN omTrSalesSO pso
			ON a.CompanyCode = pso.CompanyCode
			AND a.BranchCode = pso.BranchCode
			AND a.SONo = pso.SONo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND c.InvoiceNo BETWEEN @FirstInvoiceNo AND @LastInvoiceNo
		AND ISNULL(b.ShipAmt, 0) > 0
	UNION ALL
	SELECT
		a.CustomerCode
		, d.CustomerName
		, d.Address1 as Address1, CASE WHEN d.Address2 + ' ' + d.Address3  = '' THEN '-------' ELSE d.Address2 + ' ' + d.Address3   END as Address2
		, (SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND LookUpValue = d.CityCode 
			AND CodeID = 'CITY') + ' ' + d.ZipNo CityAndZipNo
		, d.NPWPNo
		, a.DNNo + case pso.SalesType when '0' then '-W' when '1' then '-D' end as DNNo, DNDate
		, a.InvoiceNo
		, a.SONo, pso.SKPKNo, pso.RefferenceNo
		, '' AS BPKNo
		, 'Deposit Amount (' + c.BPKNo + ')' AS Chassis
		, (SELECT DepositAccNo FROM omMstModelAccount WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
			AND SalesModelCode = c.SalesModelCode) AccNo
		, ISNULL(b.DepositAmt, 0) * c.Quantity Total
		, (SELECT LookUpValueName FROM gnMstLookUpDtl dtl
			LEFT JOIN gnMstCoProfile pf ON dtl.CompanyCode = pf.CompanyCode 
			AND dtl.LookUpValue = pf.CityCode AND dtl.CodeID = 'CITY' 
			WHERE dtl.CompanyCode = a.CompanyCode AND pf.BranchCode = a.BranchCode) CityName
		, (SELECT SignName FROM gnMstSignature WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND
			ProfitCenterCode = '100' AND DocumentType = 'DNU' AND SeqNo = 1) SignName
		, (SELECT TitleSign FROM gnMstSignature WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND
			ProfitCenterCode = '100' AND DocumentType = 'DNU' AND SeqNo = 1) TitleSign
	FROM
		omTrSalesDN a WITH(NOLOCK, NOWAIT)
		LEFT JOIN omTrSalesDNModel b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.DNNo = b.DNNo
		LEFT JOIN omTrSalesInvoiceModel c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
			AND a.BranchCode = c.BranchCode
			AND a.InvoiceNo = c.InvoiceNo
			AND c.SalesModelCode = b.SalesModelCode
			AND c.SalesModelYear = b.SalesModelYear
		LEFT JOIN gnMstCustomer d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
			AND a.CustomerCode = d.CustomerCode
		LEFT JOIN omTrSalesSO pso
			ON a.CompanyCode = pso.CompanyCode
			AND a.BranchCode = pso.BranchCode
			AND a.SONo = pso.SONo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND c.InvoiceNo BETWEEN @FirstInvoiceNo AND @LastInvoiceNo
		AND ISNULL(b.DepositAmt, 0) > 0
	UNION ALL
	SELECT
		a.CustomerCode
		, d.CustomerName
		, d.Address1 as Address1, CASE WHEN d.Address2 + ' ' + d.Address3  = '' THEN '-------' ELSE d.Address2 + ' ' + d.Address3  END as Address2
		, (SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND LookUpValue = d.CityCode 
			AND CodeID = 'CITY') + ' ' + d.ZipNo CityAndZipNo
		, d.NPWPNo
		, a.DNNo + case pso.SalesType when '0' then '-W' when '1' then '-D' end as DNNo, DNDate
		, a.InvoiceNo
		, a.SONo, pso.SKPKNo, pso.RefferenceNo
		, '' AS BPKNo
		, 'Others Amount (' + c.BPKNo + ')' AS Chassis
		, (SELECT OthersAccNo FROM omMstModelAccount WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode 
			AND SalesModelCode = c.SalesModelCode) AccNo
		, ISNULL(b.OthersAmt, 0) * c.Quantity Total
		, (SELECT LookUpValueName FROM gnMstLookUpDtl dtl
			LEFT JOIN gnMstCoProfile pf ON dtl.CompanyCode = pf.CompanyCode 
			AND dtl.LookUpValue = pf.CityCode AND dtl.CodeID = 'CITY' 
			WHERE dtl.CompanyCode = a.CompanyCode AND pf.BranchCode = a.BranchCode) CityName
		, (SELECT SignName FROM gnMstSignature WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND
			ProfitCenterCode = '100' AND DocumentType = 'DNU' AND SeqNo = 1) SignName
		, (SELECT TitleSign FROM gnMstSignature WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND
			ProfitCenterCode = '100' AND DocumentType = 'DNU' AND SeqNo = 1) TitleSign
	FROM
		omTrSalesDN a WITH(NOLOCK, NOWAIT)
		LEFT JOIN omTrSalesDNModel b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.DNNo = b.DNNo
		LEFT JOIN omTrSalesInvoiceModel c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
			AND a.BranchCode = c.BranchCode
			AND a.InvoiceNo = c.InvoiceNo
			AND c.SalesModelCode = b.SalesModelCode
			AND c.SalesModelYear = b.SalesModelYear
		LEFT JOIN gnMstCustomer d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
			AND a.CustomerCode = d.CustomerCode
		LEFT JOIN omTrSalesSO pso
			ON a.CompanyCode = pso.CompanyCode
			AND a.BranchCode = pso.BranchCode
			AND a.SONo = pso.SONo
	WHERE
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode
		AND c.InvoiceNo BETWEEN @FirstInvoiceNo AND @LastInvoiceNo
		AND ISNULL(b.OthersAmt, 0) > 0
) gab
GROUP BY gab.CustomerCode
	, gab.CustomerName
	, gab.Address1
	, gab.Address2
	, gab.CityAndZipNo
	, gab.NPWPNo
	, gab.DNNo
	, gab.DNDate
	, gab.InvoiceNo
	, gab.SONo
	, gab.SKPKNo
	, gab.RefferenceNo
	, gab.BPKNo
	, gab.Chassis
	, gab.AccNo
	, gab.CityName
	, gab.SignName
	, gab.TitleSign
ORDER BY
	gab.InvoiceNo
	, gab.Chassis

END
GO

if object_id('uspfn_SvTrnListKsgFromSPK') is not null
	drop procedure uspfn_SvTrnListKsgFromSPK
GO

CREATE procedure [dbo].[uspfn_SvTrnListKsgFromSPK]
--DECLARE
 @CompanyCode varchar(15),  
 @ProductType varchar(15),   
 @BranchFrom varchar(15),  
 @BranchTo varchar(15),  
 @PeriodFrom datetime,  
 @PeriodTo datetime,  
 @JobPDI as varchar(15),  
 @JobFSC as varchar(15),
 @BranchCode varchar(15)
as        
  
--SELECT @CompanyCode='6115204001', @ProductType= '2W', @BranchFrom= '6115204401', @BranchTo= '6115204401', @PeriodFrom= '20150501', @PeriodTo= '20150526', 
-- @JobPDI='', @JobFSC= '%FSC%', @BranchCode= '6115204401'
 
select * into #t1 from(  
select  
    convert(bit, 1) Process      
 , srv.BranchCode  
 , srv.JobOrderNo  
 , case when convert(varchar, srv.JobOrderDate, 106) = '19000101' then '' else convert(varchar, srv.JobOrderDate, 106) end JobOrderDate  
 , srv.BasicModel  
 , srv.ServiceBookNo  
 , job.PdiFscSeq  
 , srv.Odometer  
 , srv.LaborGrossAmt  
 , round((select isnull(SUM((SupplyQty - ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = 'F'),0) MaterialGrossAmt --Pembulatan
 , round((srv.LaborGrossAmt + (select isnull(SUM((SupplyQty-ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = 'F')),0) PdiFscAmount  --Pembulatan
 , isnull(case when convert(varchar, veh.FakturPolisiDate, 112) = '19000101' then '' else convert(varchar, veh.FakturPolisiDate, 106) end, '')  FakturPolisiDate  
 , isnull(case when convert(varchar, mstVeh.BPKDate, 112) = '19000101' then '' else convert(varchar, mstVeh.BPKDate, 106) end, '')  BPKDate  
 , srv.ChassisCode  
 , srv.ChassisNo  
 , srv.EngineCode  
 , srv.EngineNo   
    , srv.InvoiceNo  
 , isnull(inv.FPJNo, '') FPJNo  
 , isnull(case when convert(varchar, inv.FPJDate, 112) = '19000101' then '' else convert(varchar, inv.FPJDate, 106) end, '')  FPJDate  
 , isnull(fpj.FPJGovNo, '') FPJGovNo  
 , srv.TransmissionType  
 , srv.ServiceStatus  
 , srv.CompanyCode  
 , srv.ProductType  
from svTrnService srv  
left join svMstJob job  
 on job.CompanyCode = srv.CompanyCode  
  and job.ProductType = srv.ProductType  
  and job.BasicModel = srv.BasicModel  
  and job.JobType = srv.JobType  
left join svMstCustomerVehicle veh  
 on veh.CompanyCode = srv.CompanyCode  
  and veh.ChassisCode = srv.ChassisCode  
  and veh.ChassisNo = srv.ChassisNo  
left join omMstVehicle mstVeh  
 on mstVeh.CompanyCode = srv.CompanyCode  
  and mstVeh.ChassisCode = srv.ChassisCode  
  and mstVeh.ChassisNo = srv.ChassisNo  
left join svTrnInvoice inv  
 on inv.CompanyCode = srv.CompanyCode  
  and inv.BranchCode = srv.BranchCode  
  and inv.ProductType = srv.ProductType  
  and inv.InvoiceNo = srv.InvoiceNo  
left join svTrnFakturPajak fpj  
 on fpj.CompanyCode = srv.CompanyCode  
  and fpj.BranchCode = srv.BranchCode  
  and fpj.FPJNo = inv.FPJNo  
where   
 srv.CompanyCode = @CompanyCode  
 and srv.BranchCode between @BranchFrom and @BranchTo  
 and srv.ProductType = @ProductType  
 --and srv.isLocked = 0  
 and job.GroupJobType = 'FSC'  
 and (job.JobType like @JobFSC or job.JobType like @JobPDI)  
 and convert(varchar, srv.JobOrderDate, 112) between convert(varchar,@PeriodFrom, 112) and convert(varchar,@PeriodTo, 112)   
 and not exists (  
  select 1   
  from svTrnPdiFscApplication   
  where CompanyCode=srv.CompanyCode  
   and BranchCode=srv.BranchCode   
   and InvoiceNo=srv.JobOrderNo  
   and ProductType=srv.ProductType  
 ) and  not exists (  
  select 1   
  from svTrnPdiFscApplication   
  where CompanyCode=srv.CompanyCode  
   and BranchCode= @BranchCode
   and InvoiceNo=srv.JobOrderNo  
   and ProductType=srv.ProductType  
 )
) #t1  
  
select   
row_number() over (order by #t1.BranchCode, #t1.JobOrderNo) No,  
* from #t1   
where ServiceStatus=5 ---service status hanya yang tutup SPK
-- in (5, 7, 9)  
order by BranchCode, JobOrderNo  
  
select * into #t2 from(  
select   
(row_number() over (order by BasicModel)) RecNo  
,BasicModel  
,PdiFscSeq  
,Count(BasicModel) RecCount  
,sum(PdiFscAmount) PdiFscAmount   
from #t1 where ServiceStatus =5    ---service status hanya yang tutup SPK
--in (5, 7, 9)  
group by BasicModel, PdiFscSeq) #t2     
  
select * from #t2 order by BasicModel  
  
select '' RecNo, 'Total' BasicModel, '' PdiFscSeq, sum(RecCount) RecCount, sum(PdiFscAmount) PdiFscAmount from #t2  
  
select   
 srv.BranchCode  
 , reffService.Description AS Status  
 , employee.EmployeeName   
 , srv.JobOrderNo  
 , srv.JobOrderDate  
 , srv.PoliceRegNo  
 , srv.BasicModel  
 , srv.JobType  
from #t1   
left join svTrnService srv    
on srv.CompanyCode = #t1.CompanyCode  
 and srv.BranchCode = #t1.BranchCode     
 and srv.ProductType = #t1.ProductType    
 and srv.JobOrderNo = #t1.JobOrderNo  
left join svMstRefferenceService reffService  
    on reffService.CompanyCode = srv.CompanyCode  
    and reffService.ProductType = srv.ProductType      
    and reffService.RefferenceCode = srv.ServiceStatus  
    and reffService.RefferenceType = 'SERVSTAS'  
left join gnMstEmployee employee  
    on employee.CompanyCode = srv.CompanyCode  
    and employee.BranchCode = srv.BranchCode  
 and employee.EmployeeID = srv.ForemanID  
where #t1.ServiceStatus < 5  
order by BranchCode, JobOrderNo  
  
drop table #t1  
drop table #t2
GO

if object_id('uspfn_SvTrnListKsgFromSPKNew') is not null
	drop procedure uspfn_SvTrnListKsgFromSPKNew
GO
CREATE procedure [dbo].[uspfn_SvTrnListKsgFromSPKNew]
--DECLARE
 @CompanyCode varchar(15),  
 @ProductType varchar(15),   
 @BranchFrom varchar(15),  
 @BranchTo varchar(15),  
 @PeriodFrom datetime,  
 @PeriodTo datetime,  
 @JobPDI as varchar(15),  
 @JobFSC as varchar(15),
 @BranchCode varchar(15)
 
 --select @CompanyCode='6115204001', @ProductType='2W',@BranchFrom='6115204331',@BranchTo='6115204336',@PeriodFrom='20150501',@PeriodTo='20150527',
 --@JobPDI='%PDI%',@JobFSC='',@BranchCode='6115204331'
as        
begin

declare @IsCentralize as varchar(1)
set @IsCentralize = '0'
if(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='SRV_FLAG' and LookUpValue='KSG_HOLDING') > '0'
	set @IsCentralize = '1'
	 	 
select * into #t1 from(  
select  
    convert(bit, 1) Process      
 , srv.BranchCode  
 , srv.JobOrderNo  
 , case when convert(varchar, srv.JobOrderDate, 106) = '19000101' then '' else convert(varchar, srv.JobOrderDate, 106) end JobOrderDate  
 , srv.BasicModel  
 , srv.ServiceBookNo  
 , job.PdiFscSeq  
 , srv.Odometer  
 , srv.LaborGrossAmt  
 , round((select isnull(SUM((SupplyQty - ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = 'F'),0) MaterialGrossAmt --Pembulatan
 , round((srv.LaborGrossAmt + (select isnull(SUM((SupplyQty - ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = 'F')),0) PdiFscAmount  --Pembulatan
 , isnull(case when convert(varchar, veh.FakturPolisiDate, 112) = '19000101' then '' else convert(varchar, veh.FakturPolisiDate, 106) end, '')  FakturPolisiDate  
 , isnull(case when convert(varchar, mstVeh.BPKDate, 112) = '19000101' then '' else convert(varchar, mstVeh.BPKDate, 106) end, '')  BPKDate  
 , srv.ChassisCode  
 , srv.ChassisNo  
 , srv.EngineCode  
 , srv.EngineNo   
    , srv.InvoiceNo  
 , isnull(inv.FPJNo, '') FPJNo  
 , isnull(case when convert(varchar, inv.FPJDate, 112) = '19000101' then '' else convert(varchar, inv.FPJDate, 106) end, '')  FPJDate  
 , isnull(fpj.FPJGovNo, '') FPJGovNo  
 , srv.TransmissionType  
 , srv.ServiceStatus  
 , srv.CompanyCode  
 , srv.ProductType  
from svTrnService srv  
left join svMstJob job  
 on job.CompanyCode = srv.CompanyCode  
  and job.ProductType = srv.ProductType  
  and job.BasicModel = srv.BasicModel  
  and job.JobType = srv.JobType  
left join svMstCustomerVehicle veh  
 on veh.CompanyCode = srv.CompanyCode  
  and veh.ChassisCode = srv.ChassisCode  
  and veh.ChassisNo = srv.ChassisNo  
left join omMstVehicle mstVeh  
 on mstVeh.CompanyCode = srv.CompanyCode  
  and mstVeh.ChassisCode = srv.ChassisCode  
  and mstVeh.ChassisNo = srv.ChassisNo  
left join svTrnInvoice inv  
 on inv.CompanyCode = srv.CompanyCode  
  and inv.BranchCode = srv.BranchCode  
  and inv.ProductType = srv.ProductType  
  and inv.InvoiceNo = srv.InvoiceNo  
left join svTrnFakturPajak fpj  
 on fpj.CompanyCode = srv.CompanyCode  
  and fpj.BranchCode = srv.BranchCode  
  and fpj.FPJNo = inv.FPJNo  
where   
 srv.CompanyCode = @CompanyCode  
 and srv.BranchCode between @BranchFrom and @BranchTo  
 and srv.ProductType = @ProductType  
 --and srv.isLocked = 0  
 and job.GroupJobType = 'FSC'  
 and ((job.GroupJobType like @JobFSC and job.PdiFscSeq > 0 )  or (job.JobType like @JobPDI and job.PdiFscSeq=0))
 and convert(varchar, srv.JobOrderDate, 112) between convert(varchar, @PeriodFrom, 112) and convert(varchar, @PeriodTo , 112)  
 --and  not exists (  
 -- select 1   
 -- from svTrnPdiFscApplication   
 -- where CompanyCode=srv.CompanyCode  
 --  and (case when @IsCentralize = '0' then BranchCode end) = srv.BranchCode   
 --  and InvoiceNo=srv.JobOrderNo  
 --  and ProductType=srv.ProductType					
 and  not exists (  
  select 1   
  from svTrnPdiFscApplication   
  where CompanyCode=srv.CompanyCode  
   and BranchCode = (case when @IsCentralize = '0' then srv.BranchCode  else @BranchCode end)
   and InvoiceNo=srv.JobOrderNo  
   and ProductType=srv.ProductType  
 )--)
) #t1  
  
select   
row_number() over (order by #t1.BranchCode, #t1.JobOrderNo) No,  
* from #t1   
where ServiceStatus=5 ---service status hanya yang tutup SPK
-- in (5, 7, 9)  
order by BranchCode, JobOrderNo  
  
select * into #t2 from(  
select   
(row_number() over (order by BasicModel)) RecNo  
,BasicModel  
,PdiFscSeq  
,Count(BasicModel) RecCount  
,sum(PdiFscAmount) PdiFscAmount   
from #t1 where ServiceStatus =5    ---service status hanya yang tutup SPK
--in (5, 7, 9)  
group by BasicModel, PdiFscSeq) #t2     
  
select * from #t2 order by BasicModel  
  
select '' RecNo, 'Total' BasicModel, '' PdiFscSeq, sum(RecCount) RecCount, sum(PdiFscAmount) PdiFscAmount from #t2  
  
select   
 srv.BranchCode  
 , reffService.Description AS Status  
 , employee.EmployeeName   
 , srv.JobOrderNo  
 , srv.JobOrderDate  
 , srv.PoliceRegNo  
 , srv.BasicModel  
 , srv.JobType  
from #t1   
left join svTrnService srv    
on srv.CompanyCode = #t1.CompanyCode  
 and srv.BranchCode = #t1.BranchCode     
 and srv.ProductType = #t1.ProductType    
 and srv.JobOrderNo = #t1.JobOrderNo  
left join svMstRefferenceService reffService  
    on reffService.CompanyCode = srv.CompanyCode  
    and reffService.ProductType = srv.ProductType      
    and reffService.RefferenceCode = srv.ServiceStatus  
    and reffService.RefferenceType = 'SERVSTAS'  
left join gnMstEmployee employee  
    on employee.CompanyCode = srv.CompanyCode  
    and employee.BranchCode = srv.BranchCode  
 and employee.EmployeeID = srv.ForemanID  
where #t1.ServiceStatus < 5  
order by BranchCode, JobOrderNo  
  
drop table #t1  
drop table #t2
end
GO

IF object_id('ITSBrowseSO4') IS NOT NULL
	DROP VIEW ITSBrowseSO4
GO

CREATE VIEW [dbo].[ITSBrowseSO4]  
as  
select distinct a.CompanyCode,a.BranchCode,convert(varchar,a.InquiryNumber) InquiryNo,a.InquiryDate,b.EmployeeName,a.NamaProspek,a.TipeKendaraan,  
 a.EmployeeID, a.LastProgress, a.CreatedBy  
from pmKDP a  
 left join HrEmployee b on a.CompanyCode=b.CompanyCode  
  and a.EmployeeID=b.EmployeeID  
  where a.LastProgress='SPK'

GO

if object_id('sp_InquirDetailDataKendaraan') is not null
	drop procedure sp_InquirDetailDataKendaraan
GO
CREATE procedure [dbo].[sp_InquirDetailDataKendaraan] 
(
 @CompanyCode varchar(15),
 @BranchCode varchar(15),
 @ChassisCode varchar(100),
 @ChassisNo varchar(100)
)
AS

DECLARE
	@QRYTmp		AS varchar(max),
	@DBMD		AS varchar(25),
	@CompanyMD  AS varchar(25),
	@BranchMD AS varchar(25)


BEGIN

set @CompanyMD = (SELECT TOP 1 CompanyMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode)
set @BranchMD = (SELECT TOP 1 UnitBranchMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 

set @DBMD = (SELECT TOP 1 DbMD FROM gnMstCompanyMapping WHERE CompanyCode=@CompanyCode AND BranchCode=@BranchCode) 

set @QRYTmp = 
'select 
	(select b.refferenceDesc1 from ' + @DBMD + '.dbo.ommstrefference b where b.companyCode = ''' + @CompanyMD + '''
	and b.refferencetype = ''WARE'' and b.refferenceCode = a.warehouseCode) as warehouseName
	,''('' + a.ColourCode+ '') '' +(select c.refferenceDesc1 from ommstrefference c where c.companyCode = ''' + @CompanyCode + '''
	and c.refferencetype = ''COLO'' and c.refferenceCode = a.ColourCode) as ColourName
	, a.servicebookno
	, a.keyno
	, a.cogsunit
	, a.cogsOthers
	, a.cogsKaroseri
    , o.afterdiscdpp dpp
    , o.afterdiscppn ppn
    , p.bbn
	, j.pono
    , convert(varchar, j.podate, 106) podate
	, h.bpuno+'' (''+h.RefferenceDONo+'')'' bpuno
    , convert(varchar, h.bpudate, 106) bpudate
	, aa.sono
    , convert(varchar, d.sodate, 106) sodate
	, k.dono
    , convert(varchar, k.dodate, 106) dodate
    , case d.SKPKNo when '''' then d.RefferenceNo else d.SKPKNo end as SKPKNo
    , convert(varchar, d.sodate, 106)  SKPKDate
    , l.bpkno
    , convert(varchar, l.bpkdate, 106) bpkdate
	, m.invoiceNo
    , convert(varchar, m.invoicedate, 106) invoicedate
    , q.RefferenceSJNo
	, convert(varchar, q.RefferenceSJDate, 106) RefferenceSJDate
    , i.hppno
    , convert(varchar, i.hppdate, 106) hppdate
	, n.reqNo reqOutNo
    , convert(varchar, n.reqDate, 106) reqdate
    , i.RefferenceInvoiceNo reffinv
    , convert(varchar, i.RefferenceInvoiceDate, 106) reffinvdate
    , i.RefferenceFakturPajakNo refffp
    , convert(varchar, i.RefferenceFakturPajakDate , 106) refffpdate
	, s.PoliceRegistrationNo policeregno
    , convert(varchar, s.PoliceRegistrationDate, 106) policeregdate
	, isnull(b.CustomerCode + '' - '' + c.CustomerName, 
		k.CustomerCode + '' - '' + (select CustomerName from gnMstCustomer where CompanyCode = ''' + @CompanyCode + ''' and CustomerCode = k.CustomerCode)
		) as Customer
	, isnull(c.Address1 + '' '' + c.Address2 + '' '' + c.Address3 + '' '' + c.Address4,
		(select Address1 + '' '' + Address2 + '' '' + Address3 + '' '' + Address4 from gnMstCustomer where CompanyCode = ''' + @CompanyCode + ''' and CustomerCode = k.CustomerCode)
		) as Address
	, d.Salesman + '' - '' + f.EmployeeName as Salesman
	, d.LeasingCo + '' - '' + g.CustomerName as Leasing
	, d.SalesCode + '' - '' + e.LookUpValueName as KelAR
    , s.BPKBNo
	, s.SPKNo
	, a.ChassisCode
	, a.SalesModelCode
	, a.ChassisNo
	, a.EngineNo
from 
	' + @DBMD + '.dbo.ommstvehicle a
left join ommstvehicle aa on aa.CompanyCode = ''' + @CompanyCode + ''' and a.ChassisCode = aa.ChassisCode and a.ChassisNo = aa.ChassisNo and aa.ColourCode = a.ColourCode
left join omTrSalesInvoice b on b.CompanyCode = ''' + @CompanyCode + ''' and b.BranchCode like ''' + @BranchCode + ''' 
    and a.InvoiceNo = b.InvoiceNo
left join gnMstCustomer c on c.CompanyCode = ''' + @CompanyCode + ''' and b.Customercode = c.CustomerCode
left join omTrSalesSO d on d.CompanyCode = ''' + @CompanyCode + ''' and d.BranchCode like ''' + @BranchCode + ''' and aa.SONo = d.SONo
left join GnMstLookUpDtl e on e.CompanyCode = ''' + @CompanyCode + ''' and CodeID = ''GPAR'' and e.LookUpValue = d.SalesCode
left join gnMstEmployee f on f.Companycode = ''' + @CompanyCode + ''' and f.BranchCode like ''' + @BranchCode + ''' 
    and f.EmployeeID = d.Salesman
left join gnMstCustomer g on g.CompanyCode = ''' + @CompanyCode + ''' and g.CustomerCode = d.LeasingCo
left join ' + @DBMD + '..omTrPurchaseBPU h on h.CompanyCode = ''' + @CompanyMD + ''' and h.BranchCode like ''' + @BranchMD + ''' 
    and a.PONo = h.PONo and a.BPUNo=h.BPUNo
left join ' + @DBMD + '..omTrPurchaseHPP i on i.CompanyCode = ''' + @CompanyMD + ''' and i.BranchCode like ''' + @BranchMD + ''' and a.HPPNo= i.HPPNo
left join ' + @DBMD + '..omTrPurchasePO j on j.CompanyCode = ''' + @CompanyMD + ''' and j.BranchCode like ''' + @BranchMD + ''' and a.PONo = j.PONo
left join omTrSalesDO k on k.CompanyCode = ''' + @CompanyCode + ''' and k.BranchCode like ''' + @BranchCode + ''' and aa.DONo = k.DONo and aa.SONo= k.SONo
left join omTrSalesBPK l on l.CompanyCode = ''' + @CompanyCode + ''' and l.BranchCode like ''' + @BranchCode + ''' and aa.BPKNo = l.BPKNo
left join omTrSalesInvoice m on m.CompanyCode = ''' + @CompanyCode + ''' and m.BranchCode like ''' + @BranchCode + ''' 
    and aa.InvoiceNo = m.InvoiceNo
left join omTrSalesReq n on n.CompanyCode = ''' + @CompanyCode + ''' and aa.ReqOutNo = n.ReqNo
--and n.BranchCode like ''' + @BranchCode + ''' and aa.ReqOutNo = n.ReqNo
left join omTrSalesSOModel o on o.CompanyCode = ''' + @CompanyCode + ''' and o.BranchCode like ''' + @BranchCode + ''' and aa.SONo = o.SONo 
    and a.SalesModelCode = o.SalesModelCode and a.SalesModelYear = o.SalesModelYear and a.ChassisCode = o.ChassisCode
left join omTrSalesSOVin p on p.CompanyCode = ''' + @CompanyCode + ''' and p.BranchCode like ''' + @BranchCode + ''' and aa.SONo = p.SONo
    and a.SalesModelCode = p.SalesModelCode and a.SalesModelYear = p.SalesModelYear and a.ColourCode = p.ColourCode
    and a.ChassisNo = p.ChassisNo and a.ChassisCode = p.ChassisCode
left join ' + @DBMD + '..omTrPurchaseBPU q on q.CompanyCode = ''' + @CompanyMD + ''' and q.BranchCode like ''' + @BranchMD + ''' and q.PONo = j.PONO 
	and q.BPUNo = a.BPUNo
left join omTrSalesSPKDetail s on s.CompanyCode = ''' + @CompanyCode + ''' 
	--and s.BranchCode like ''' + @BranchCode + '''
	and s.ChassisCode = a.ChassisCode and s.ChassisNo = a.ChassisNo and aa.ReqOutNo = s.ReqInNo
where 
	a.companyCode = ''' + @CompanyMD + ''' and a.chassisCode = ''' + @ChassisCode + ''' and a.chassisNo = ''' + @ChassisNo + ''''

	Exec (@QRYTmp);

END
GO

if object_id('uspfn_SvTrnServiceOutstanding') is not null
	drop procedure uspfn_SvTrnServiceOutstanding
GO

CREATE procedure [dbo].[uspfn_SvTrnServiceOutstanding]
	@OutType     varchar(15),
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType varchar(15),
	@PoliceRegNo varchar(15),
	@JobType     varchar(15),
	@ChassisCode varchar(15),
	@ChassisNo	 varchar(10)

as      

create table #t1(ServiceNo bigint, ServiceType varchar(10), JobOrderNo varchar(15), JobOrderDate datetime, JobType varchar(20), MessageInfo varchar(max))

if @OutType = 'FSC'
begin
	insert into #t1
	select top 1 a.ServiceNo
		 , a.ServiceType
		 , case a.ServiceType
			 when 0 then a.EstimationNo
			 when 1 then a.BookingNo
		     else a.JobOrderNo
		   end JobOrderNo
		 , case a.ServiceType
			 when 0 then a.EstimationDate
			 when 1 then a.BookingDate
		     else a.JobOrderDate
		   end JobOrderDate
		 , a.JobType
		 , 'Kendaraan ini sudah pernah di Free Service, transaksi tidak bisa dilanjutkan' 
	  from svTrnService a, svTrnSrvTask b
	 where a.JobType like 'FSC%'
	   and b.CompanyCode = a.CompanyCode
	   and b.BranchCode  = a.BranchCode
	   and b.ProductType = a.ProductType
	   and b.ServiceNo   = a.ServiceNo
	   and b.BillType    = 'F'
	   and a.CompanyCode = @CompanyCode
	   and a.BranchCode  = @BranchCode
	   and a.ProductType = @ProductType
	   and a.PoliceRegNo = @PoliceRegNo
	   and a.JobType     = @JobType
	   and a.ChassisCode = @ChassisCode
	   and a.ChassisNo	 = CONVERT(varchar, @ChassisNo, 10)
	   and a.ServiceType = 2
	   and a.ServiceStatus <> '6'
end

if @OutType = 'OUT'
begin
	insert into #t1
	select top 1 ServiceNo
		 , ServiceType
		 , case ServiceType
			 when 0 then EstimationNo
			 when 1 then BookingNo
		   else JobOrderNo end JobOrderNo
		 , case ServiceType
			 when 0 then EstimationDate
			 when 1 then BookingDate
		   else JobOrderDate end JobOrderDate
		 , JobType
		 , 'Kendaraan ini masih ada outstanding, masih akan dilanjutkan?' 
	  from svTrnService
	 where ServiceStatus in ('0','1','2','3','4','5')
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and ProductType = @ProductType
	   and PoliceRegNo = @PoliceRegNo
	   and ChassisCode = @ChassisCode
	   and ChassisNo   = CONVERT(varchar, @ChassisNo, 10)
	   and ServiceType = '2'
end

if @OutType = 'BOK'
begin
	insert into #t1
	select top 1 ServiceNo
		 , ServiceType
		 , BookingNo as JobOrderNo
		 , BookingDate as JobOrderDate
		 , JobType
		 , 'Kendaraan ini masih ada outstanding booking, masih akan dilanjutkan?' 
	  from svTrnService
	 where ServiceStatus in ('0','1','2','3','4','5')
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and ProductType = @ProductType
	   and PoliceRegNo = @PoliceRegNo
	   and ChassisCode = @ChassisCode
	   and ChassisNo   = CONVERT(varchar, @ChassisNo, 10)
	   and ServiceType = '1'
	   and datediff(month, BookingDate, getdate()) <= 1
end

if @OutType = 'EST'
begin
	insert into #t1
	select top 1 ServiceNo
		 , ServiceType
		 , EstimationNo as JobOrderNo
		 , EstimationDate as JobOrderDate
		 , JobType
		 , 'Kendaraan ini masih ada outstanding estimasi, masih akan dilanjutkan?' 
	  from svTrnService
	 where ServiceStatus in ('0','1','2','3','4','5')
	   and CompanyCode = @CompanyCode
	   and BranchCode  = @BranchCode
	   and ProductType = @ProductType
	   and PoliceRegNo = @PoliceRegNo
	   and ChassisCode = @ChassisCode
	   and ChassisNo   = CONVERT(varchar, @ChassisNo, 10)
	   and ServiceType = '0'
	   and datediff(month, EstimationDate, getdate()) <= 1
end

select * from #t1
drop table #t1
GO

IF OBJECT_ID('[dbo].[usprpt_abInqTurnOverRatio]') IS NOT NULL
	DROP PROCEDURE [usprpt_abInqTurnOverRatio]
GO

-- CREATED BY Benedict 15-Apr-2015 
-- LAST UPDATE BY Benedict 12-May-2015

CREATE PROCEDURE [dbo].[usprpt_abInqTurnOverRatio]
	@DealerCode varchar(15),
	@OutletCode varchar(15),
	@Start date,
	@End date,
	@Position varchar(5)
AS BEGIN

SET NOCOUNT ON
--DECLARE
--	@DealerCode varchar(15) = '6006400001',
--	@OutletCode varchar(15) = '',
--	@Start date = '2012-06-30',
--	@End date = '2013-06-30',
--	@Position varchar(5) = ''

BEGIN---#POOL START#---
/*
Hanya memilih yang JoinDate <= AssignDate & JoinDate <= MutationDate
apabila AssignDate/MutationDate < JoinDate, maka tidak valid (tidak termasuk)
*/

;WITH _1A AS (
	SELECT a.CompanyCode, a.EmployeeID, MAX(a.AssignDate) AS AssignDate
	FROM HrEmployeeAchievement a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND CONVERT(date, a.AssignDate) <= @Start
	AND a.Department = 'SALES'
	GROUP BY a.CompanyCode, a.EmployeeID
), _1B AS (
	SELECT a.CompanyCode, a.EmployeeID, MIN(a.AssignDate) AS AssignDate
	FROM HrEmployeeAchievement a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND @Start < CONVERT(date, a.AssignDate)
	AND a.Department = 'SALES'
	GROUP BY a.CompanyCode, a.EmployeeID
), _2A AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, a.AssignDate
	FROM _1A a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _2B AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, a.AssignDate
	FROM _1B a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _3A AS (
	SELECT a.CompanyCode, a.EmployeeID, MutationDate = MAX(a.MutationDate)
	FROM HrEmployeeMutation a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND CONVERT(date, a.MutationDate) <= @Start
	GROUP BY a.CompanyCode, a.EmployeeID
), _3B AS (
	SELECT a.CompanyCode, a.EmployeeID, MutationDate = MIN(a.MutationDate)
	FROM HrEmployeeMutation a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND @Start < CONVERT(date, a.MutationDate)
	GROUP BY a.CompanyCode, a.EmployeeID
), _4A AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _3A a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _4B AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _3B a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _5 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Position
	, CASE WHEN a.Position <> 'S' THEN '' ELSE (CASE a.Grade WHEN '' THEN '1' ELSE ISNULL(a.Grade, '1') END) END AS Grade
	, a.JoinDate, a.ResignDate 
	FROM HrEmployee a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
	AND CONVERT(date, a.JoinDate) <= @Start
	AND (@Start < a.ResignDate OR a.ResignDate IS NULL OR a.ResignDate <= a.JoinDate)
), _6 AS (
	SELECT a.CompanyCode
	, BranchCode = ISNULL(b.BranchCode, (SELECT c.BranchCode FROM _4B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID))
	, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate
	, MutationDate = ISNULL(b.MutationDate, (SELECT c.MutationDate FROM _4B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID))
	, a.ResignDate
	FROM _5 a
	LEFT JOIN _4A b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
), _7 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName
	, ISNULL(b.Position, ISNULL((SELECT c.Position FROM _2B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID), a.Position)) AS Position
	, ISNULL(b.Grade, ISNULL((SELECT c.Grade FROM _2B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID), a.Grade)) AS Grade
	, a.JoinDate
	, ISNULL(b.AssignDate, (SELECT c.AssignDate FROM _2B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID)) AS AssignDate
	, a.MutationDate, a.ResignDate
	FROM _6 a
	LEFT JOIN  _2A b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
)
SELECT * INTO #JoinFirstStart FROM (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
	FROM _7 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
	AND a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END
	AND a.Position IS NOT NULL
) #JoinFirstStart

SELECT * INTO #PoolStart FROM(
	SELECT * FROM #JoinFirstStart
) #PoolStart
END

BEGIN---#POOL END#---
/*
Hanya memilih yang JoinDate <= AssignDate & JoinDate <= MutationDate
apabila AssignDate/MutationDate < JoinDate, maka tidak valid (tidak termasuk)
*/

;WITH _1A AS (
	SELECT a.CompanyCode, a.EmployeeID, MAX(a.AssignDate) AS AssignDate
	FROM HrEmployeeAchievement a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND CONVERT(date, a.AssignDate) <= @End
	AND a.Department = 'SALES'
	GROUP BY a.CompanyCode, a.EmployeeID
), _1B AS (
	SELECT a.CompanyCode, a.EmployeeID, MIN(a.AssignDate) AS AssignDate
	FROM HrEmployeeAchievement a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND @End < CONVERT(date, a.AssignDate)
	AND a.Department = 'SALES'
	GROUP BY a.CompanyCode, a.EmployeeID
), _2A AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, a.AssignDate
	FROM _1A a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _2B AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, a.AssignDate
	FROM _1B a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _3A AS (
	SELECT a.CompanyCode, a.EmployeeID, MutationDate = MAX(a.MutationDate)
	FROM HrEmployeeMutation a
	WHERE CONVERT(date, a.MutationDate) <= @End
	GROUP BY a.CompanyCode, a.EmployeeID
), _3B AS(
	SELECT a.CompanyCode, a.EmployeeID, MutationDate = MIN(a.MutationDate)
	FROM HrEmployeeMutation a
	WHERE @End < CONVERT(date, a.MutationDate)
	GROUP BY a.CompanyCode, a.EmployeeID
), _4A AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _3A a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _4B AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _3B a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _5 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Position
	, CASE WHEN a.Position <> 'S' THEN '' ELSE (CASE a.Grade WHEN '' THEN '1' ELSE ISNULL(a.Grade, '1') END) END AS Grade
	, a.JoinDate, a.ResignDate 
	FROM HrEmployee a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
	AND CONVERT(date, a.JoinDate) <= @End
	AND (@End < CONVERT(date, a.ResignDate) OR CONVERT(date, a.ResignDate) IS NULL OR CONVERT(date, a.ResignDate) <= CONVERT(date, a.JoinDate))
), _6 AS (
	SELECT a.CompanyCode
	, BranchCode = ISNULL(b.BranchCode, (SELECT c.BranchCode FROM _4B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID))
	, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate
	, MutationDate = ISNULL(b.MutationDate, (SELECT c.MutationDate FROM _4B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID))
	, a.ResignDate
	FROM _5 a
	LEFT JOIN _4A b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
), _7 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID
	, ISNULL(b.Position, ISNULL((SELECT c.Position FROM _2B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID), a.Position)) AS Position
	, ISNULL(b.Grade, ISNULL((SELECT c.Grade FROM _2B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID), a.Grade)) AS Grade
	FROM _6 a
	LEFT JOIN  _2A b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
)
SELECT * INTO #JoinFirstEnd FROM (
	SELECT *
	FROM _7 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
	AND a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END
	AND a.Position IS NOT NULL
) #JoinFirstEnd

SELECT * INTO #PoolEnd FROM (
	SELECT * FROM #JoinFirstEnd
) #PoolEnd
END

BEGIN---#EMPLOYEE IN#---
/*
Hanya memilih yang JoinDate <= AssignDate & JoinDate <= MutationDate
apabila AssignDate/MutationDate < JoinDate, maka tidak valid (tidak termasuk)
*/

;WITH _1A AS (
	SELECT a.CompanyCode, a.EmployeeID, MAX(a.MutationDate) AS MutationDate
	FROM HrEmployeeMutation a 
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND b.Department = 'SALES'
	AND CONVERT(date, a.MutationDate) <= CONVERT(date, b.JoinDate)
	GROUP BY a.CompanyCode, a.EmployeeID
), _1B AS (
	SELECT a.CompanyCode, a.EmployeeID, MIN(a.MutationDate) AS MutationDate
	FROM HrEmployeeMutation a 
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND b.Department = 'SALES'
	AND CONVERT(date, b.JoinDate) < CONVERT(date, a.MutationDate)
	GROUP BY a.CompanyCode, a.EmployeeID
), _2A AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _1A a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _2B AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.MutationDate
	FROM _1B a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _3A AS (
	SELECT a.CompanyCode, a.EmployeeID, MAX(a.AssignDate) AS AssignDate
	FROM HrEmployeeAchievement a
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND b.Department = 'SALES'
	AND CONVERT(date, a.AssignDate) <= CONVERT(date, b.JoinDate)
	GROUP BY a.CompanyCode, a.EmployeeID
), _3B AS (
	SELECT a.CompanyCode, a.EmployeeID, MIN(a.AssignDate) AS AssignDate
	FROM HrEmployeeAchievement a
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND b.Department = 'SALES'
	AND CONVERT(date, b.JoinDate) < CONVERT(date, a.AssignDate)
	GROUP BY a.CompanyCode, a.EmployeeID
), _4A AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE b.Grade WHEN '' THEN (CASE WHEN b.Position <> 'S' THEN '' ELSE '1' END) ELSE b.Grade END AS Grade
	,a.AssignDate
	FROM _3A a
	INNER JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _4B AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE b.Grade WHEN '' THEN (CASE WHEN b.Position <> 'S' THEN '' ELSE '1' END) ELSE b.Grade END AS Grade
	,a.AssignDate
	FROM _3B a
	INNER JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
), _5 AS (
	SELECT a.CompanyCode
	, ISNULL(b.BranchCode, (SELECT c.BranchCode FROM _2B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID)) AS BranchCode
	, a.EmployeeID, a.Position
	, CASE a.Grade WHEN '' THEN (CASE WHEN a.Position <> 'S' THEN '' ELSE '1' END) ELSE a.Grade END AS Grade
	, a.JoinDate
	FROM HrEmployee a
	LEFT JOIN _2A b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
	AND @Start < CONVERT(date, a.JoinDate)
	AND CONVERT(date, a.JoinDate) <= @End
), _6 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID
	, ISNULL(b.Position, ISNULL((SELECT c.Position FROM _4B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID), a.Position)) AS Position
	, ISNULL(b.Grade, ISNULL((SELECT c.Grade FROM _4B c WHERE a.CompanyCode = c.CompanyCode AND a.EmployeeID = c.EmployeeID), a.Position)) AS Grade
	FROM _5 a
	LEFT JOIN _4A b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE 1 = (CASE WHEN ISNULL(b.AssignDate, (SELECT d.AssignDate FROM _4B d WHERE a.CompanyCode = d.CompanyCode AND a.EmployeeID = d.EmployeeID)) IS NULL THEN 1 ELSE 
			  (CASE WHEN CONVERT(date, a.JoinDate) < CONVERT(date, ISNULL(b.AssignDate, (SELECT d.AssignDate FROM _4B d WHERE a.CompanyCode = d.CompanyCode AND a.EmployeeID = d.EmployeeID))) THEN 1 ELSE 0 END) END)
)
SELECT * INTO #JoinIn FROM (
	SELECT * FROM _6 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
	AND a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END
	AND a.Position IS NOT NULL
) #JoinIn

/*
MUTATION IN
memilih data employee yang pindah cabang
*/
;WITH _1 AS (
	SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.MutationDate), a.CompanyCode, a.BranchCode, a.EmployeeID, a.MutationDate
	FROM HrEmployeeMutation a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
), _2 AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, b.MutationDate
	FROM _1 a
	LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE @Start < CONVERT(date, b.MutationDate)
	AND CONVERT(date, b.MutationDate) < @End
	AND a.BranchCode <> b.BranchCode
), _3 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, b.JoinDate, a.MutationDate, b.ResignDate 
	FROM _2 a
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE 1 = 1
	AND (CONVERT(date, b.ResignDate) IS NULL OR @End < CONVERT(date, b.ResignDate) OR CONVERT(date, b.ResignDate) <= CONVERT(date, b.JoinDate))
), _4 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, MAX(b.AssignDate) AS AssignDate, a.ResignDate 
	FROM _3 a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE CONVERT(date, b.AssignDate) < CONVERT(date, a.MutationDate)
	GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, a.ResignDate 
), _5 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName
	, ISNULL(b.Position, a.Position) AS Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN a.Grade ELSE ISNULL(b.Grade, a.Grade) END) END AS Grade
	, a.JoinDate, a.MutationDate, a.AssignDate, a.ResignDate 
	FROM _4 a
	INNER JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
)
SELECT * INTO #MutationIn FROM (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
	FROM _5 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
	AND a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END
) #MutationIn

/*
PROMOTION IN
memilih data employee yang mendapat promosi
*/
;WITH _1 AS (
	SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
	FROM HrEmployeeAchievement a
	WHERE 1=1
	AND a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
), _2 AS (
	SELECT a.CompanyCode, a.EmployeeID, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, b.AssignDate
	FROM _1 a
	LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.Position <> b.Position
	AND b.Position = @Position
	AND @Start < CONVERT(date, b.AssignDate)
	AND CONVERT(date, b.AssignDate) <= @End
), _3 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
	FROM _2 a
	LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE 1=1
	AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
	GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
), _4 AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
	FROM _3 a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _5 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
	, ISNULL(a.Position, b.Position) AS Position
	, ISNULL(a.Grade, b.Grade) AS Grade
	, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
	FROM _4 a
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE b.Department = 'SALES'	
)
SELECT * INTO #PromotionIn FROM (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
	FROM _5 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
) #PromotionIn
END

SELECT * INTO #EmployeeIn FROM (
	SELECT * FROM #JoinIn
	UNION
	SELECT * FROM #MutationIn
	UNION 
	SELECT * FROM #PromotionIn	
) #EmployeeIn

BEGIN---#EMPLOYEE OUT#---
/*
RESIGN OUT
memilih data employee yang Resign
ResignDate <= JoinDate dianggap tidak resign
ResignDate > JoinDate dianggap Resign
MutationDate > ResignDate dianggap Resign
AssignDate > ResignDate dianggap Resign
*/
;WITH _1 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Position
	, CASE WHEN a.Position <> 'S' THEN '' ELSE (CASE a.Grade WHEN '' THEN '1' ELSE ISNULL(a.Grade, '1') END) END AS Grade
	, a.JoinDate, a.ResignDate 
	FROM HrEmployee a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
	AND @Start < CONVERT(date, a.ResignDate)
	AND CONVERT(date, a.ResignDate) <= @End
	AND CONVERT(date, a.JoinDate) < CONVERT(date, a.ResignDate)
), _2 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, MAX(b.MutationDate) AS MutationDate, a.ResignDate
	FROM _1 a
	LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE 1=1
	AND b.MutationDate IS NOT NULL
	GROUP BY a.CompanyCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.ResignDate
), _3 AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, a.ResignDate
	FROM _2 a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _4 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, MAX(b.AssignDate) AS AssignDate, a.ResignDate
	FROM _3 a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, a.ResignDate
), _5 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName
	, ISNULL(b.Position, a.Position) AS Position
	, ISNULL(b.Grade, a.Grade) AS Grade
	, a.JoinDate, a.MutationDate, a.AssignDate, a.ResignDate
	FROM _4 a
	INNER JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
)
SELECT * INTO #ResignOut FROM (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
	FROM _5 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
	AND a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END
) #ResignOut

/*
MUTATION OUT
mengambil data employee yang pindah cabang
*/
;WITH _1 AS (
	SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.MutationDate), a.CompanyCode, a.BranchCode, a.EmployeeID, a.MutationDate
	FROM HrEmployeeMutation a
	WHERE a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
), _2 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.MutationDate
	FROM _1 a
	LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE @Start < CONVERT(date, b.MutationDate)
	AND CONVERT(date, b.MutationDate) < @End
	AND a.BranchCode <> b.BranchCode
), _3 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName, b.Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN '1' ELSE ISNULL(b.Grade, '1') END) END AS Grade
	, b.JoinDate, a.MutationDate, b.ResignDate 
	FROM _2 a
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE 1 = 1
	AND (b.ResignDate IS NULL OR @End < CONVERT(date, b.ResignDate) OR CONVERT(date, b.ResignDate) <= CONVERT(date, b.JoinDate))
), _4 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, MAX(b.AssignDate) AS AssignDate, a.ResignDate 
	FROM _3 a
	LEFT JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE CONVERT(date, b.AssignDate) < CONVERT(date, a.MutationDate)
	GROUP BY a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName, a.Position, a.Grade, a.JoinDate, a.MutationDate, a.ResignDate 
), _5 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.EmployeeName
	, ISNULL(b.Position, a.Position) AS Position
	, CASE WHEN b.Position <> 'S' THEN '' ELSE (CASE b.Grade WHEN '' THEN a.Grade ELSE ISNULL(b.Grade, a.Grade) END) END AS Grade
	, a.JoinDate, a.MutationDate, a.AssignDate, a.ResignDate 
	FROM _4 a
	INNER JOIN HrEmployeeAchievement b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.AssignDate = b.AssignDate
)
SELECT * INTO #MutationOut FROM (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
	FROM _5 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
	AND a.Position = CASE @Position WHEN '' THEN a.Position ELSE @Position END
) #MutationOut

/*
PROMOTION OUT
mengambil data employee yang mendapat promosi
*/
;WITH _1 AS (
	SELECT rownum = ROW_NUMBER() OVER(ORDER BY a.EmployeeID, a.AssignDate), a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
	FROM HrEmployeeAchievement a
	WHERE 1=1
	AND a.CompanyCode = CASE @DealerCode WHEN '' THEN a.CompanyCode ELSE @DealerCode END
	AND a.Department = 'SALES'
), _2 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.Position
	, CASE WHEN a.Position <> 'S' THEN '' ELSE (CASE a.Grade WHEN '' THEN '1' ELSE ISNULL(a.Grade, '1') END) END AS Grade
	, b.AssignDate
	FROM _1 a
	LEFT JOIN _1 b ON b.rownum = a.rownum + 1 AND a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE a.Position <> b.Position
	AND a.Position = @Position
	AND @Start < CONVERT(date, b.AssignDate)
	AND CONVERT(date, b.AssignDate) < @End
), _3 AS (
	SELECT a.CompanyCode, a.EmployeeID, a.Position, a.Grade, MutationDate = MAX(b.MutationDate), a.AssignDate
	FROM _2 a
	LEFT JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE 1 = 1
	AND CONVERT(date, b.MutationDate) < CONVERT(date, a.AssignDate)
	GROUP BY a.CompanyCode, a.EmployeeID, a.Position, a.Grade, a.AssignDate
), _4 AS (
	SELECT a.CompanyCode, b.BranchCode, a.EmployeeID, a.Position, a.Grade, a.MutationDate, a.AssignDate
	FROM _3 a
	INNER JOIN HrEmployeeMutation b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID AND a.MutationDate = b.MutationDate
), _5 AS (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, b.EmployeeName
	, ISNULL(a.Position, b.Position) AS Position
	, ISNULL(a.Grade, b.Grade) AS Grade
	, b.JoinDate, a.MutationDate, a.AssignDate, b.ResignDate
	FROM _4 a
	INNER JOIN HrEmployee b ON a.CompanyCode = b.CompanyCode AND a.EmployeeID = b.EmployeeID
	WHERE b.Department = 'SALES'
)
SELECT * INTO #PromotionOut FROM (
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeID, a.Position, a.Grade
	FROM _5 a
	WHERE a.BranchCode = CASE @OutletCode WHEN '' THEN a.BranchCode ELSE @OutletCode END
) #PromotionOut

SELECT * INTO #EmployeeOut FROM(
	SELECT * FROM #ResignOut
	UNION
	SELECT * FROM #MutationOut
	UNION
	SELECT * FROM #PromotionOut
) #EmployeeOut
END

BEGIN---#EMPLOYEE STAY#---
SELECT * INTO #employeeStay FROM(
	SELECT a.CompanyCode, a.BranchCode, a.EmployeeId, a.Position, a.Grade
	FROM #PoolStart a
	WHERE a.EmployeeID IN (SELECT b.EmployeeID FROM #PoolEnd b)
)#employeeStay
END

BEGIN---#SUM TABLES#---
DECLARE @ITSG TABLE (Value int, Name varchar(15))
INSERT INTO @ITSG (Value, Name) 
	SELECT LookUpValue, LookUpValueName 
	FROM gnMstLookUpDtl WHERE CompanyCode = CASE @DealerCode WHEN '' THEN CompanyCode ELSE @DealerCode END AND CodeID = 'ITSG'

DECLARE @Platinum int = 4, --(SELECT Value FROM @ITSG WHERE Name = 'Platinum'),
	@Gold int = 3, --(SELECT Value FROM @ITSG WHERE Name = 'Gold'),
	@Silver int = 2, --(SELECT Value FROM @ITSG WHERE Name = 'Silver'),
	@Trainee int = 1 --(SELECT Value FROM @ITSG WHERE Name = 'Trainee')

DECLARE @sum1 TABLE (CompanyCode varchar(15), BranchCode varchar(15), CountPoolStart int)
INSERT INTO @sum1 (CompanyCode, BranchCode, CountPoolStart)
	SELECT a.DealerCode,
		a.OutletCode, 
		(SELECT COUNT(*) 
			FROM #PoolStart b 
			WHERE b.CompanyCode = a.DealerCode
			AND b.BranchCode = a.OutletCode
		)
	FROM gnMstDealerOutletMapping a
	WHERE a.IsActive = 1
	AND a.DealerCode = CASE @DealerCode WHEN '' THEN a.DealerCode ELSE @DealerCode END
	AND a.OutletCode = CASE @OutletCode WHEN '' THEN a.OutletCode ELSE @OutletCode END

SELECT * INTO #sum1 FROM(
	SELECT 
		b.CompanyCode, 
		b.BranchCode,
		COUNT(*) AS EmployeeCount,
		SUM(CASE a.Grade WHEN @Platinum THEN 1 ELSE 0 END) AS Platinum,
		SUM(CASE a.Grade WHEN @Gold THEN 1 ELSE 0 END) AS Gold,
		SUM(CASE a.Grade WHEN @Silver THEN 1 ELSE 0 END) AS Silver,
		SUM(CASE a.Grade WHEN @Trainee THEN 1 ELSE 0 END) AS Trainee
	FROM @sum1 b
	JOIN #PoolStart a
	ON a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	WHERE b.CountPoolStart > 0
	GROUP BY b.CompanyCode, b.BranchCode
UNION ALL 
	SELECT 
		b.CompanyCode,
		b.BranchCode,
		0 AS EmployeeCount,
		0 AS Platinum,
		0 AS Gold,
		0 AS Silver,
		0 AS Trainee
	FROM @sum1 b
	WHERE b.CountPoolStart = 0
) #sum1

DECLARE @sum2 TABLE (CompanyCode varchar(15), BranchCode varchar(15), CountPoolEnd int)
INSERT INTO @sum2 (CompanyCode, BranchCode, CountPoolEnd)
	SELECT a.DealerCode,
		a.OutletCode, 
		(SELECT COUNT(*) 
			FROM #PoolEnd b 
			WHERE b.CompanyCode = a.DealerCode
			AND b.BranchCode = a.OutletCode
		)
	FROM gnMstDealerOutletMapping a
	WHERE a.IsActive = 1
	AND a.DealerCode = CASE @DealerCode WHEN '' THEN a.DealerCode ELSE @DealerCode END
	AND a.OutletCode = CASE @OutletCode WHEN '' THEN a.OutletCode ELSE @OutletCode END

SELECT * INTO #sum2 FROM (
	SELECT 
		b.CompanyCode, 
		b.BranchCode,
		COUNT(*) AS EmployeeCount,
		SUM(CASE a.Grade WHEN @Platinum THEN 1 ELSE 0 END) AS Platinum,
		SUM(CASE a.Grade WHEN @Gold THEN 1 ELSE 0 END) AS Gold,
		SUM(CASE a.Grade WHEN @Silver THEN 1 ELSE 0 END) AS Silver,
		SUM(CASE a.Grade WHEN @Trainee THEN 1 ELSE 0 END) AS Trainee
	FROM @sum2 b
	JOIN #PoolEnd a
	ON a.CompanyCode = b.CompanyCode
	AND a.BranchCode = b.BranchCode
	WHERE b.CountPoolEnd > 0
	GROUP BY b.CompanyCode, b.BranchCode
UNION ALL 
	SELECT 
		b.CompanyCode,
		b.BranchCode,
		0 AS EmployeeCount,
		0 AS Platinum,
		0 AS Gold,
		0 AS Silver,
		0 AS Trainee
	FROM @sum2 b
	WHERE b.CountPoolEnd = 0
) #sum2
END

BEGIN---#RESULT#---
SELECT * INTO #result FROM (
	SELECT 
		a.CompanyCode, 
		a.BranchCode,
		c.DealerAbbreviation, 
		d.OutletAbbreviation, 
			ISNULL(
					(CONVERT(decimal(6,2), a.EmployeeCount) - CONVERT(decimal(6,2), (SELECT COUNT(*) 
											FROM #employeeStay e
											WHERE e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode)))
					/ NULLIF(CONVERT(decimal(6,2), a.EmployeeCount), 0)
					, 0)
		AS Ratio, 
		a.EmployeeCount AS StartEmployeeCount,
			(SELECT COUNT(*) FROM #employeeIn f WHERE f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode)
		AS EmployeeIn,
		a.Platinum AS StartPlatinum, 
		a.Gold AS StartGold, 
		a.Silver AS StartSilver, 
		a.Trainee AS StartTrainee,
			(SELECT COUNT(*) FROM #employeeStay h WHERE h.CompanyCode = a.CompanyCode AND h.BranchCode = a.BranchCode)
		AS LoyalCount,
		b.EmployeeCount AS EndEmployeeCount,
			(SELECT COUNT(*) FROM #employeeOut g WHERE g.CompanyCode = a.CompanyCode AND g.BranchCode = a.BranchCode) 
		AS EmployeeOut,
		b.Platinum AS EndPlatinum, 
		b.Gold AS EndGold, 
		b.Silver AS EndSilver, 
		b.Trainee AS EndTrainee
	FROM #sum1 a
	JOIN #sum2 b 
		ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
	JOIN gnMstDealerMapping c 
		ON c.DealerCode = a.CompanyCode
	JOIN gnMstDealerOutletMapping d 
		ON d.DealerCode = a.CompanyCode AND d.OutletCode = a.BranchCode
)#result
END

--SELECT * FROM #JoinFirstStart 
--SELECT * FROM #PoolStart 
--SELECT * FROM #JoinFirstEnd 
--SELECT * FROM #PoolEnd 
--SELECT * FROM #JoinIn 
--SELECT * FROM #PromotionIn 
--SELECT * FROM #MutationIn 
--SELECT * FROM #EmployeeIn 
--SELECT * FROM #ResignOut 
--SELECT * FROM #MutationOut
--SELECT * FROM #PromotionOut
--SELECT * FROM #EmployeeOut 
--SELECT * FROM #sum1
--SELECT * FROM #sum2
SELECT * FROM #result

IF OBJECT_ID('tempdb..#JoinFirstStart') IS NOT NULL DROP TABLE #JoinFirstStart
IF OBJECT_ID('tempdb..#PoolStart') IS NOT NULL DROP TABLE #PoolStart
IF OBJECT_ID('tempdb..#JoinFirstEnd') IS NOT NULL DROP TABLE #JoinFirstEnd
IF OBJECT_ID('tempdb..#PoolEnd') IS NOT NULL DROP TABLE #PoolEnd
IF OBJECT_ID('tempdb..#employeeStay') IS NOT NULL DROP TABLE #employeeStay
IF OBJECT_ID('tempdb..#JoinIn') IS NOT NULL DROP TABLE #JoinIn
IF OBJECT_ID('tempdb..#MutationIn') IS NOT NULL DROP TABLE #MutationIn
IF OBJECT_ID('tempdb..#PromotionIn') IS NOT NULL DROP TABLE #PromotionIn
IF OBJECT_ID('tempdb..#employeeIn') IS NOT NULL DROP TABLE #employeeIn
IF OBJECT_ID('tempdb..#ResignOut') IS NOT NULL DROP TABLE #ResignOut
IF OBJECT_ID('tempdb..#MutationOut') IS NOT NULL DROP TABLE #MutationOut
IF OBJECT_ID('tempdb..#PromotionOut') IS NOT NULL DROP TABLE #PromotionOut
IF OBJECT_ID('tempdb..#employeeOut') IS NOT NULL DROP TABLE #employeeOut
IF OBJECT_ID('tempdb..#sum1') IS NOT NULL DROP TABLE #sum1
IF OBJECT_ID('tempdb..#sum2') IS NOT NULL DROP TABLE #sum2
IF OBJECT_ID('tempdb..#result') IS NOT NULL DROP TABLE #result

END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[uspfn_GenerateLampiranNPNew]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[uspfn_GenerateLampiranNPNew]
GO

create procedure [dbo].[uspfn_GenerateLampiranNPNew]
--DECLARE
	@CompanyCode	VARCHAR(MAX),
	@BranchCode		VARCHAR(MAX),
	@PickingSlipNo	VARCHAR(MAX),
	@LmpDate		DATETIME,
	@ProductType	VARCHAR(MAX),
	@UserID			VARCHAR(MAX),
	@TypeOfGoods	VARCHAR(MAX)
	
--SET	@CompanyCode	= '6159401000'
--SET	@BranchCode		= '6159401001'
--SET	@PickingSlipNo	= 'PLS/15/010478'
--SET	@LmpDate		= '20150520'
--SET	@ProductType	= '4W'
--SET	@UserID			= 'yo'
--SET	@TypeOfGoods	= '0'

AS
BEGIN
	BEGIN TRY
	BEGIN TRAN
		DECLARE @MaxLmpNo INT
		SET @MaxLmpNo = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
			CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND DocumentType = 'LMP' 
				AND ProfitCenterCode = '300' 
				AND DocumentYear = YEAR(GetDate())),0)

		DECLARE @errmsg VARCHAR(MAX)
		DECLARE @TempLmpNo	VARCHAR(MAX)
		DECLARE @TempBPSFNo	VARCHAR(MAX)
		DECLARE @CustomerCode VARCHAR(MAX)

		SET @TempLmpNo  = ISNULL((SELECT 'LMP/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxLmpNo, 1), 6)),'LMP/YY/XXXXXX')
		SET @TempBPSFNo = ISNULL((SELECT BPSFNo FROM SpTrnSBPSFHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo),'')
		SET @CustomerCode = ISNULL((SELECT CustomerCode FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo),'')

		IF (ISNULL((SELECT Status FROM SpTrnSBPSFhdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND BPSFNo = @TempBPSFno), '0') = '2')
		BEGIN
			SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor picking ini sudah pernah di-proses, hubungi IT support untuk pemerikasaan data lebih lanjut !'
			RAISERROR (@errmsg,16,1);
			RETURN
		END

		UPDATE SpTrnSBPSFHdr
		SET Status = '2'
			, LastUpdateDate = GetDate()
			, LastUpdateBy = @UserID
		WHERE CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND BPSFNo = @TempBPSFNo
		--===============================================================================================================================
		IF (ISNULL((SELECT LmpNo FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo), '') <> '')
		BEGIN
			SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor lampiran sudah ada, periksa setting-an sequence dokumen (LMP) pada general module !'
			RAISERROR (@errmsg,16,1);
			RETURN
		END

		DECLARE @isLocked BIT
		SET @isLocked = (SELECT IsLocked FROM SpTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)

		INSERT INTO SpTrnSLmpHdr
		SELECT
			CompanyCode
			, BranchCode
			, @TempLmpNo LmpNo	
			, GetDate() LmpDate
			, @TempBPSFNo BPSFNo
			, (SELECT BPSFDate FROM SpTrnSBPSFHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND BPSFNo = @TempBPSFNo)
				BPSFDate
			, PickingSlipNo
			, PickingSlipDate
			, TransType
			, CustomerCode
			, CustomerCodeBill
			, CustomerCodeShip
			, TotSalesQty
			, TotSalesAmt
			, TotDiscAmt
			, TotDPPAmt
			, TotPPNAmt
			, TotFinalSalesAmt
			, CONVERT(BIT, 0) isPKP
			, '0' Status
			, 0 PrintSeq
			, TypeOfGoods
			, @UserID CreatedBy
			, GetDate() CreatedDate
			, @UserID LastUpdateBy
			, GetDate() LastUpdateDate
			, @isLocked IsLocked	
			, NULL LockingBy
			, NULL LockingDate
		FROM SpTrnSPickingHdr 
		WHERE 
			1 = 1
			AND CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND PickingSlipNo = @PickingSlipNo
		
		UPDATE GnMstDocument
		SET DocumentSequence = DocumentSequence + 1
			, LastUpdateDate = GetDate()
			, LastUpdateBy = @UserID
		WHERE
			1 = 1
			AND CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND DocumentType = 'LMP'
			AND ProfitCenterCode = '300'
			AND DocumentYear = Year(GetDate())
		
		--===============================================================================================================================
		-- INSERT LAMPIRAN DETAIL
		--===============================================================================================================================
		INSERT INTO SpTrnSLmpDtl
		SELECT
			a.CompanyCode
			, a.BranchCode
			, @TempLmpNo LmpNo
			, a.WarehouseCode
			, a.PartNo
			, a.PartNoOriginal
			, a.DocNo
			, a.DocDate
			, a.ReferenceNo
			, a.ReferenceDate
			, a.LocationCode
			, a.QtyBill
			, a.RetailPriceInclTax
			, a.RetailPrice
			, ISNULL((SELECT CostPrice FROM SpMstItemPrice WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND PartNo = a.PartNo),0) CostPrice
			, a.DiscPct
			, a.SalesAmt
			, a.DiscAmt
			, a.NetSalesAmt
			, 0 PPNAmt
			, a.TotSalesAmt
			, a.ProductType
			, a.PartCategory 
			, a.MovingCode
			, a.ABCClass
			, @UserID CreatedBy
			, GetDate() CreatedDate
			, @UserID LastUpdateBy
			, GetDate() LastUpdateDate
		FROM SpTrnSPickingDtl a
		WHERE 
			1 = 1
			AND a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND a.PickingSlipNo = @PickingSlipNo
			AND a.QtyPicked > 0
		
		--===============================================================================================================================
		-- INSERT SvSdMovement
		--===============================================================================================================================
		declare @SQL as varchar(max)
		declare @md bit
		set @md = (select case WHEN EXISTS(select * from gnMstCompanyMapping where CompanyCode = @CompanyCode and BranchCode = @BranchCode and CompanyMD = @CompanyCode and BranchMD = @BranchCode) then cast(1 as bit) ELSE cast(0 as bit) END)
		if @md = 0
		BEGIN
		set @SQL = '
			insert into ' +dbo.GetDbMD(@CompanyCode,@BranchCode)+'..svSDMovement (CompanyCode, BranchCode, DocNo, DocDate, PartNo, PartSeq
			, WarehouseCode, QtyOrder, Qty, DiscPct, CostPrice, RetailPrice
			, TypeOfGoods, CompanyMD, BranchMD, WarehouseMD, RetailPriceInclTaxMD, RetailPriceMD
			, CostPriceMD, QtyFlag, ProductType, ProfitCenterCode, Status, ProcessStatus
			, ProcessDate, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)  
			select h.CompanyCode, h.BranchCode, h.LmpNo, h.LmpDate, d.PartNo, ROW_NUMBER() OVER(Order by d.LmpNo)
			,d.WarehouseCode, d.QtyBill, d.QtyBill, d.DiscPct
			, d.CostPrice
			, d.RetailPrice
			,h.TypeOfGoods, ''' +dbo.GetCompanyMD(@CompanyCode,@BranchCode)+ ''', ''' + +dbo.GetBranchMD(@CompanyCode,@BranchCode)+ + ''', ''' +dbo.GetWarehouseMD(@CompanyCode,@BranchCode)+ ''', p.RetailPriceInclTax, p.RetailPrice, 
			p.CostPrice,''x'', d.ProductType,''300'', ''0'',''0'',''' + convert(varchar, GETDATE()) + ''',''' + @UserID + ''',''' +
			  convert(varchar, GETDATE()) + ''',''' +  @UserID + ''',''' +  convert(varchar, GETDATE()) + '''
			 from spTrnSLmpDtl d 
			 inner join spTrnSLmpHdr h on h.CompanyCode = d.CompanyCode and h.BranchCode = d.BranchCode and h.LmpNo = d.LmpNo  
			 join spmstitemprice p
			 on p.PartNo = d.PartNo and p.CompanyCode = d.CompanyCode and p.BranchCode = d.BranchCode
			  where 1 = 1   
				and d.CompanyCode = ''' + @CompanyCode + ''' 
				and d.BranchCode  = ''' + @BranchCode  + '''
				and d.ProductType = ''' + @ProductType  + '''
				and d.LmpNo = ''' + @TempLmpNo + ''''
		exec(@SQL)
		END	
		--===============================================================================================================================
		-- UPDATE STOCK
		-- NOTES : Transtype = 11 --> + BorrowedQty
		--         Transtype = 12 --> - BorrowQty
		--===============================================================================================================================
		DECLARE @TempTransType VARCHAR (MAX) 
		SET @TempTransType = ISNULL((SELECT SUBSTRING(TransType,1,1) FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0)
		
		--===============================================================================================================================
		-- VALIDATION QTY
		--===============================================================================================================================
		DECLARE @Onhand_Valid NUMERIC(18,2)	
		DECLARE @Allocation_SRValid NUMERIC(18,2)
		DECLARE @Allocation_SLValid NUMERIC(18,2)
		DECLARE @Allocation_SPValid NUMERIC(18,2)
		DECLARE @Valid2 as table
		(
			PartNo  varchar(20),
			QtyValidSR  decimal,
			QtyValidSL  decimal,
			QtyValidSP  decimal,
			QtyValidOnhand  decimal
		)

		set @SQL = 
		'SELECT * INTO #Valid_2 FROM(
			SELECT a.PartNo
				, b.AllocationSR - a.QtyBill QtyValidSR
				, b.AllocationSL - a.QtyBill QtyValidSL
				, b.AllocationSP - a.QtyBill QtyValidSP
				, b.Onhand - a.QtyBill QtyValidOnhand
			FROM SpTrnSPickingDtl a
			INNER JOIN '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems b ON b.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
				AND b.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
				AND b.PartNo = a.PartNo		
			WHERE 1 = 1
				AND a.CompanyCode = '''+@CompanyCode+'''
				AND a.BranchCode = '''+@BranchCode+'''
				AND a.PickingSlipNo = '''+@PickingSlipNo+'''
		) #Valid_2
				
		select * from #Valid_2
		drop table #Valid_2'
		
		insert into @Valid2
		exec(@SQL)
		SET @Allocation_SRValid = ISNULL((SELECT TOP 1 QtyValidSR FROM @Valid2 WHERE QtyValidSR < 0),0)
		SET @Allocation_SPValid = ISNULL((SELECT TOP 1 QtyValidSP FROM @Valid2 WHERE QtyValidSP < 0),0)
		SET @Allocation_SLValid = ISNULL((SELECT TOP 1 QtyValidSL FROM @Valid2 WHERE QtyValidSL < 0),0)
		SET @Onhand_Valid = ISNULL((SELECT TOP 1 QtyValidOnhand FROM @Valid2 WHERE QtyValidOnhand < 0),0)
		
		IF (@TempTransType = '2')
		BEGIN
			IF (@Onhand_Valid < 0 OR @Allocation_SRValid < 0)
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END 
		END
		
		IF (@TempTransType = '1')
		BEGIN
			IF (@Onhand_Valid < 0 OR @Allocation_SPValid < 0)
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END 
		END	

		IF (@TempTransType = '3')
		BEGIN
			IF (@Onhand_Valid < 0 OR @Allocation_SLValid < 0)
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat part dengan quantity Onhand atau alokasi kurang dari nol !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END 
		END	
		
		--===============================================================================================================================
		IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '11')
		BEGIN
		SET @SQL = 
		'UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems
		SET
			BorrowedQty = BorrowedQty + b.QtyBill
		FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
			AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
			AND b.PickingSlipNo = '''+@PickingSlipNo+'''
			AND a.PartNo = b.PartNo'
		EXEC(@SQL)
		END

		IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '12')
		BEGIN
			SET @SQL =
			'UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems
			SET
				BorrowQty = BorrowQty - b.QtyBill
			FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
				AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
				AND b.PickingSlipNo = '''+@PickingSlipNo+'''
				AND a.PartNo = b.PartNo'
			EXEC(@SQL)	
		END

		IF (@TempTransType = '2')
		BEGIN
		
		SET @SQL =
		'UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems
		SET
			AllocationSR = AllocationSR - b.QtyBill
			, Onhand = Onhand - b.QtyBill
			, LastUpdateBy = '''+@UserID+'''
			, LastUpdateDate = GetDate()
			, LastSalesDate =  GetDate()
		FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
			AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
			AND b.PickingSlipNo = '''+@PickingSlipNo+'''
			AND a.PartNo = b.PartNo

		UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItemLoc
		SET
			AllocationSR = AllocationSR - b.QtyBill
			, Onhand = Onhand - b.QtyBill
			, LastUpdateBy = '''+@UserID+'''
			, LastUpdateDate = GetDate()
		FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItemLoc a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
			AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
			AND a.WarehouseCode = ''00''
			AND b.PickingSlipNo = '''+@PickingSlipNo+'''
			AND a.PartNo = b.PartNo'
		EXEC(@SQL)
		END

		IF (@TempTransType = '1')
		BEGIN
			SET @SQL =
			'UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems
			SET
				AllocationSP = AllocationSP - b.QtyBill
				, Onhand = Onhand - b.QtyBill
				, LastUpdateBy = '''+@UserID+'''
				, LastUpdateDate = GetDate()
				, LastSalesDate = GetDate()
			FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
				AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
				AND b.PickingSlipNo = '''+@PickingSlipNo+'''
				AND a.PartNo = b.PartNo

			UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItemLoc
			SET
				AllocationSP = AllocationSP - b.QtyBill
				, Onhand = Onhand - b.QtyBill
				, LastUpdateBy = '''+@UserID+'''
				, LastUpdateDate = GetDate()
			FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItemLoc a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
				AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
				AND a.WarehouseCode = ''00''
				AND b.PickingSlipNo = '''+@PickingSlipNo+'''
			AND a.PartNo = b.PartNo'
			EXEC(@SQL)
		END

		IF (@TempTransType = '3')
		BEGIN
			SET @SQL =
			'UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems
			SET
				AllocationSL = AllocationSL - b.QtyBill
				, Onhand = Onhand - b.QtyBill
				, LastUpdateBy = '''+@UserID+'''
				, LastUpdateDate = GetDate()
				, LastSalesDate = GetDate()
			FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItems a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
				AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
				AND b.PickingSlipNo = '''+@PickingSlipNo+'''
				AND a.PartNo = b.PartNo

			UPDATE '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItemLoc
			SET
				AllocationSL = AllocationSL - b.QtyBill
				, Onhand = Onhand - b.QtyBill
				, LastUpdateBy = '''+@UserID+'''
				, LastUpdateDate = GetDate()
			FROM '+dbo.GetDbMD(@CompanyCode,@BranchCode)+'..SpMstItemLoc a, SpTrnSPickingDtl b
			WHERE
				1 = 1
				AND a.CompanyCode = '+dbo.GetCompanyMD(@CompanyCode,@BranchCode)+'
				AND a.BranchCode = '+dbo.GetBranchMD(@CompanyCode,@BranchCode)+'
				AND a.WarehouseCode = ''00''
				AND b.PickingSlipNo = '''+@PickingSlipNo+'''
				AND a.PartNo = b.PartNo'
			EXEC(@SQL)
		END
		--===============================================================================================================================
		-- UPDATE DEMAND CUST AND DEMAND ITEM
		--===============================================================================================================================

		UPDATE SpHstDemandCust
		SET SalesFreq = SalesFreq + 1
			, SalesQty = SalesQty + b.QtyBill
			, LastUpdateBy = @UserID 
			, LastUpdateDate = GetDate()
		FROM SpHstDemandCust a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND b.CompanyCode = @CompanyCode
			AND b.BranchCode = @BranchCode
			AND b.PickingSlipNo = @PickingSlipNo
			AND a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.Year = Year(b.DocDate)
			AND a.Month = Month(b.DocDate)
			AND a.CustomerCode = @CustomerCode
			AND a.PartNo = b.PartNo
			
		UPDATE SpHstDemandItem
		SET SalesFreq = SalesFreq + 1
			, SalesQty = SalesQty + b.QtyBill
			, LastUpdateBy = @UserID 
			, LastUpdateDate = GetDate()
		FROM SpHstDemandItem a, SpTrnSPickingDtl b
		WHERE
			1 = 1
			AND a.CompanyCode = @CompanyCode
			AND a.BranchCode = @BranchCode
			AND b.CompanyCode = @CompanyCode
			AND b.BranchCode = @BranchCode
			AND b.PickingSlipNo = @PickingSlipNo
			AND a.CompanyCode = b.CompanyCode
			AND a.BranchCode = b.BranchCode
			AND a.Year = Year(b.DocDate)
			AND a.Month = Month(b.DocDate)
			AND a.PartNo = b.PartNo

		----=============================================================================================================================
		---- INSERT TO ITEM MOVEMENT
		----=============================================================================================================================
		INSERT INTO SpTrnIMovement
		SELECT
			@CompanyCode CompanyCode
			, @BranchCode BranchCode
			, a.LmpNo DocNo
			, (SELECT LmPDate FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode 
				AND BranchCode = @BranchCode AND LmpNo = a.LmpNo) 
			  DocDate
			, dateadd(s,ROW_NUMBER() OVER(Order by a.PartNo),getdate()) CreatedDate 
			, '00' WarehouseCode
			, LocationCode 
			, a.PartNo
			, 'OUT' SignCode
			, 'LAMPIRAN' SubSignCode
			, a.QtyBill
			, a.RetailPrice
			, a.CostPrice
			, a.ABCClass
			, a.MovingCode
			, a.ProductType
			, a.PartCategory
			, @UserID CreatedBy
		FROM SpTrnSLmpDtl a
		WHERE
			1 = 1
			AND CompanyCode = @CompanyCode
			AND BranchCode = @BranchCode
			AND LmpNo = @TempLmpNo


		--===============================================================================================================================
		-- UPDATE AVERAGE COST
		-- NOTES : Transtype = 2% (SERVICE) CHECK ISLINKTOSERVICE
		--===============================================================================================================================
		IF (ISNULL((SELECT SUBSTRING(TransType,1,1) FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '2')
		BEGIN
			IF (CONVERT(VARCHAR,ISNULL((SELECT IsLinkToService FROM gnMstCoProfile WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0),0) = '1')
			BEGIN
				SELECT
					a.CompanyCode
					, a.BranchCode
					, d.ProductType
					, ISNULL(d.ServiceNo, 0) ServiceNo
					, a.PartNo
					, a.DocNo SupplySlipNo
					, ISNULL(a.CostPrice, 0) CostPrice
  					, ISNULL(a.RetailPrice, 0) RetailPrice
				INTO #1
				FROM spTrnSLmpDtl a 
					INNER JOIN spTrnSORDHdr c ON a.CompanyCode = c.CompanyCode
						AND a.BranchCode = c.BranchCode
						AND a.DocNo = c.DocNo
					INNER JOIN svTrnService d ON a.CompanyCode = d.CompanyCode
						AND a.BranchCode = d.BranchCode
				WHERE a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND d.ProductType = @ProductType
					AND c.UsageDocNo = d.JobOrderNo
					AND a.LmpNo = @TempLmpNo

				UPDATE svTrnSrvItem 
				SET	CostPrice = b.CostPrice
					, LastUpdateBy = @UserID
					, LastUpdateDate = GETDATE()
				FROM svTrnSrvItem a, #1 b
				WHERE a.CompanyCode = b.CompanyCode
					AND a.BranchCode = b.BranchCode
					AND a.ProductType = b.ProductType
					AND a.ServiceNo = b.ServiceNo  
					AND a.PartNo = b.PartNo
					AND a.SupplySlipNo = b.SupplySlipNo	

				--===============================================================================================================================
				-- SERVICE PART
				--===============================================================================================================================
				SELECT * INTO #TempServiceItem FROM (
				SELECT 
					a.CompanyCode
					, a.BranchCode
					, a.ProductType
					, a.ServiceNo
					, a.PartNo
					, a.PartSeq
					, a.DemandQty
					, a.SupplyQty
					, b.QtyBill
					, b.DocNo
					, a.CostPrice
					, a.RetailPrice
					, a.TypeOfGoods
					, a.BillType
					, a.DiscPct
				FROM SvTrnSrvItem a 
				INNER JOIN SpTrnSPickingDtl b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PartNo = b.PartNo AND a.SupplySlipNo = b.DocNo
				WHERE
					1 = 1
					AND a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND a.ProductType = @ProductType
					AND a.ServiceNo IN (SELECT ServiceNo 
										FROM SvTrnService 
										WHERE 1 = 1 AND CompanyCode = @CompanyCode 
											AND BranchCode = @BranchCode 
											AND JobOrderNo IN (SELECT ReferenceNo 
																FROM SpTrnSPickingDtl 
																WHERE 1= 1 AND CompanyCode = @CompanyCode 
																	AND  BranchCode = @BranchCode 
																	AND PickingSlipNo = @PickingSlipNo))
					AND a.PartSeq IN (SELECT MAX(PartSeq) FROM SvTrnSrvItem WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
						AND ProductType = @ProductType AND ServiceNo = a.ServiceNo AND PartNo = a.PartNo)
					AND a.SupplySlipNo = b .DocNo
				) #TempServiceItem 

				UPDATE svTrnSrvItem
				SET SupplyQty = (CASE WHEN b.QtyBill > b.DemandQty 
								THEN 
									CASE WHEN b.DemandQty = 0 THEN b.QtyBill ELSE b.DemandQty END
								ELSE b.QtyBill END)
					, LastUpdateBy = @UserID
					, LastUpdateDate = Getdate()
				FROM svTrnSrvItem a, #TempServiceItem b
				WHERE
					1 = 1
					AND a.CompanyCode = b.CompanyCode
					AND a.BranchCode = b.BranchCode
					AND a.ProductType = b.ProductType
					AND a.ServiceNo = b.ServiceNo
					AND a.PartNo = b.PartNo
					AND a.PartSeq = b.PartSeq
					AND a.SupplySlipNo = b.DocNo

				UPDATE svTrnSrvItem
				SET CostPrice = b.CostPrice
					, LastUpdateBy = @UserID
					, LastUpdateDate = Getdate()
				FROM svTrnSrvItem a, #TempServiceItem b
				WHERE
					1 = 1
					AND a.CompanyCode = b.CompanyCode
					AND a.BranchCode = b.BranchCode
					AND a.ProductType = b.ProductType
					AND a.ServiceNo = b.ServiceNo
					AND a.PartNo = b.PartNo
					AND a.SupplySlipNo = b.DocNo

				--===============================================================================================================================
				-- INSERT NEW SRV ITEM BASED PICKING LIST
				--===============================================================================================================================
				INSERT INTO SvTrnSrvItem (CompanyCode, BranchCode, ProductType, ServiceNo, PartNo, PartSeq, DemandQty, SupplyQty, ReturnQty, CostPrice, RetailPrice, TypeOfGoods, BillType, SupplySlipNo, SupplySlipDate, SSReturnNo, SSReturnDate, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, DiscPct)
				SELECT 
					a.CompanyCode
					, a.BranchCode
					, a.ProductType
					, a.ServiceNo
					, a.PartNo
					, (select max(PartSeq)+1 from svTrnSrvItem b where b.CompanyCode = a.CompanyCode and b.BranchCode = a.BranchCode and 
						b.ProductType = a.ProductType and b.ServiceNo = a.ServiceNo) PartSeq
					, 0 DemandQty
					, a.QtyBill - a.DemandQty SupplyQty
					, 0 ReturnQty
					, a.CostPrice
					, a.RetailPrice
					, a.TypeOfGoods
					, a.BillType
					, a.DocNo SupplySlipNo
					, (SELECT TOP 1 DocDate FROM SpTrnSORDHdr WHERE 1= 1 AND CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode
						AND DocNo = a.DocNo) SupplySlipDate
					, NULL SSReturnNo
					, NULL SSReturnDate
					, @UserID CreatedBy
					, GetDate() CreatedDate
					, @UserID LastUpdateBy
					, GetDate() LastUpdateDate
					, a.DiscPct
				FROM #TempServiceItem a 
				WHERE
					1 = 1
					AND a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND a.ProductType = @ProductType
					AND a.DemandQty < a.QtyBill
					AND a.QtyBill > 0
					AND a.DemandQty > 0

				DROP TABLE #TempServiceItem 	
				DROP TABLE #1
			END
		END

		--===============================================================================================================================
		-- GENERATE JOURNAL AND AUTOMATE TRANSFER STOCK
		-- NOTES : Transtype = 10 (TRANSFER STOCK)
		--===============================================================================================================================
		DECLARE @TempJournalPrefix	VARCHAR(MAX)
		DECLARE @MaxTempJournal		INT

		DECLARE @TempJournal		VARCHAR(MAX)
		DECLARE @Amount				NUMERIC(18,2)
		DECLARE @TempFiscalMonth	INT
		DECLARE @TempFiscalYear		INT

		DECLARE @PeriodeNum			NUMERIC(18,0)
		DECLARE @Periode			VARCHAR(MAX)
		DECLARE @PeriodeName		VARCHAR(MAX)
		DECLARE @GLDate				DATETIME

		SET @TempFiscalYear = ISNULL((SELECT FiscalYear FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0) 
		SET @TempFiscalMonth  = ISNULL((SELECT FiscalMonth FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0) 

		SET @PeriodeNum = ISNULL((SELECT  TOP 1 PeriodeNum
				FROM gnMstPeriode 
				WHERE CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode AND FiscalYear = @TempFiscalYear
					AND FiscalMonth = @TempFiscalMonth AND StatusSparepart = 1
					AND (MONTH(FromDate) = MONTH(@LmpDate) AND YEAR(FromDate) = YEAR(@LmpDate))
					AND FiscalStatus = 1 ), NULL) 

		SET @Periode = ISNULL((SELECT  TOP 1 CONVERT(varchar, FiscalYear) + RIGHT('00' + CONVERT(varchar, PeriodeNum), 2) AS Periode
				FROM gnMstPeriode 
				WHERE CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode AND FiscalYear = @TempFiscalYear
					AND FiscalMonth = @TempFiscalMonth AND StatusSparepart = 1
					AND (MONTH(FromDate) = MONTH(@LmpDate) AND YEAR(FromDate) = YEAR(@LmpDate))
					AND FiscalStatus = 1 ), NULL) 

		SET @PeriodeName =  ISNULL((SELECT  TOP 1 PeriodeName
				FROM gnMstPeriode 
				WHERE CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode AND FiscalYear = @TempFiscalYear
					AND FiscalMonth = @TempFiscalMonth AND StatusSparepart = 1
					AND (MONTH(FromDate) = MONTH(@LmpDate) AND YEAR(FromDate) = YEAR(@LmpDate))
					AND FiscalStatus = 1 ), NULL)

		DECLARE @AccountTypeInTran	VARCHAR(MAX)
		DECLARE @AccountTypeInvent	VARCHAR(MAX)

		SET @AccountTypeInTran = ISNULL((SELECT b.AccountType FROM GnMstAccount b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode AND b.AccountNo = 
				ISNULL((SELECT InTransitAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')
				),'')
		SET @AccountTypeInvent = ISNULL((SELECT b.AccountType FROM GnMstAccount b WHERE b.CompanyCode = @CompanyCode AND b.BranchCode = @BranchCode AND b.AccountNo = 
				ISNULL((SELECT InventoryAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')
				),'')

		--===============================================================================================================================
		-- SET ACCOUNT FOR GENERATE JOURNAL
		--===============================================================================================================================
		DECLARE @CustCode VARCHAR(MAX)
		SET @CustCode = (SELECT CustomerCode FROM spTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)
		
		Declare @TPGO VARCHAR(MAX)
		SET @TPGO = (SELECT TypeOfGoods FROM spTrnSPickingHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND PickingSlipNo = @PickingSlipNo)

		DECLARE @InventoryAccNo VARCHAR(MAX)
		DECLARE @InTransitAccNo VARCHAR(MAX)

		IF (@isLocked = '1')
		BEGIN
			DECLARE @CompTo VARCHAR(MAX)
			SET @CompTo = (SELECT ISNULL(CompanyCodeTo,'') FROM spMstCompanyAccount WHERE CompanyCode = @CompanyCode AND BranchCodeTo = @CustCode)
			
			SET @InTransitAccNo =  (SELECT ISNULL(IntercompanyAccNoTo,'') FROM spMstCompanyAccountDtl WHERE CompanyCode = @CompanyCode AND CompanyCodeTo = @CompTo AND TPGO = @TPGO)
		END
		ELSE
			SET @InTransitAccNo =  ISNULL((SELECT InTransitAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')

		SET @InventoryAccNo = ISNULL((SELECT InventoryAccNo FROM SpMstAccount WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND TypeOfGoods = @TypeOfGoods),'')
		SET @Amount = ISNULL((SELECT SUM(QtyBill * CostPrice) FROM SpTrnSLmpDtl WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0)

		--===============================================================================================================================
		-- GENERATE JOURNAL
		-- NOTES : Transtype = 1O (TRANSFER STOCK)
		--===============================================================================================================================
		IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '10')
		BEGIN
			IF (@isLocked != '1')
			BEGIN
				--===============================================================================================================================
				-- AUTOMATE TRANSFER STOCK
				--===============================================================================================================================
				INSERT INTO SpUtlStockTrfHdr
				SELECT
					a.CompanyCode
					, a.CustomerCode BranchCode
					, a.BranchCode DealerCode
					, a.LmpNo LampiranNo
					, a.CustomerCode RcvDealerCode
					, '' InvoiceNo 
					, '' BinningNo 
					, '1900-01-01 00:00:00.000' BinningDate
					, 0 Status
					, @UserID CreatedBy
					, GetDate() CreatedDate
					, @UserID LastUpdateBy
					, GetDate() LastUpdateDate
					, null TypeOfGoods
				FROM SpTrnSLmpHdr a
				WHERE a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND a.LmpNo = @TempLmpNo

				INSERT INTO SpUtlStockTrfDtl
				SELECT
					a.CompanyCode CompanyCode
					, b.CustomerCode BranchCode
					, a.BranchCode DealerCode
					, a.LmpNo LampiranNo
					, a.DocNo OrderNo
					, a.PartNo PartNo
					, b.BPSFno SalesNo
					, a.PartNo PartNoShip
					, a.QtyBill QtyShipped 
					, 1.00 SalesUnit
					, a.CostPrice PurchasePrice
					, a.CostPrice costPrice
					, '1900-01-01 00:00:00.000' ProcessDate
					, @ProductType Producttype
					, '' partCategory
					, @UserID CreatedBy
					, GetDate() CreatedDate
					, @UserID LastUpdateBy
					, GetDate() LastUpdateDate 
				FROM SpTrnSLmpDtl a
				INNER JOIN SpTrnSLmpHdr b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.LmpNo = b.LmpNo
				WHERE a.CompanyCode = @CompanyCode
					AND a.BranchCode = @BranchCode
					AND a.LmpNo = @TempLmpNo
			END

			--===============================================================================================================================
			-- GENERATE GLINTERFACE
			--===============================================================================================================================
			IF (ISNULL((SELECT TOP 1 DocNo FROM glInterface WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND DocNo = @TempLmpNo), '') <> '')
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor lampiran sudah ada dalam glInterface, periksa setting-an sequence dokumen (LMP) !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END
			 
			INSERT INTO GLInterface
			SELECT
				a.CompanyCode
				, a.BranchCode
				, a.LmpNo DocNo
				, 1 SeqNo
				, a.LmpDate DocDate
				, '300' ProfitCenterCode
				, GetDate() AccDate
				, @InTransitAccNo AccountNo
				, 'SPAREPART' JournalCode
				, 'BPS' TypeJournal
				, a.LmpNo ApplyTo
				, @Amount AmountDB
				, 0 AmountCR
				, 'INTRANSIT' TypeTrans
				, '' BatchNo
				, '1900-01-01 00:00:00.000' BatchDate
				, 3 StatusFlag
				, @UserID CreateBy 
				, GetDate() CreateDate
				, @UserID UpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			INSERT INTO GLInterface
			SELECT
				a.CompanyCode
				, a.BranchCode
				, a.LmpNo DocNo
				, 2 SeqNo
				, a.LmpDate DocDate
				, '300' ProfitCenterCode
				, GetDate() AccDate
				, @InventoryAccNo AccountNo
				, 'SPAREPART' JournalCode
				, 'BPS' TypeJournal
				, a.LmpNo ApplyTo
				, 0 AmountDB	
				, @Amount AmountCR
				, 'INVENTORY' TypeTrans
				, '' BatchNo
				, '1900-01-01 00:00:00.000' BatchDate
				, 3 StatusFlag
				, @UserID CreateBy 
				, GetDate() CreateDate
				, @UserID UpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			SET @GLDate = ISNULL((SELECT TOP 1 DocDate FROM GlInterface WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND DocNo = @TempLmpNo), NULL)

			--===============================================================================================================================
			-- GENERATE GLJOURNAL
			--===============================================================================================================================
			SET @MaxTempJournal = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
				CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND DocumentType = 'JTS' 
					AND ProfitCenterCode = '300' 
					AND DocumentYear = YEAR(GetDate())),0)

			SET @TempJournalPrefix = ISNULL((SELECT DocumentPrefix FROM GnMstDocument WHERE 
				CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND DocumentType = 'JTS' 
					AND ProfitCenterCode = '300' 
					AND DocumentYear = YEAR(GetDate())),'XXX')

			SET @TempJournal = ISNULL((SELECT @TempJournalPrefix + '/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxTempJournal, 1), 6)),'XXX/YY/XXXXXX')

			IF (ISNULL((SELECT TOP 1 JournalNo FROM GlJournal WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND JournalNo = @TempJournal), '') <> '')
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor journal sudah terpakai, periksa setting-an sequence dokumen (JTS) !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END

			INSERT INTO GLJournal
			SELECT
				CompanyCode
				, BranchCode
				, @TempFiscalYear FiscalYear
				, '300' ProfitCenterCode
				, @TempJournal JournalNo
				, 'Harian' JournalType
				, GetDate() JournalDate
				, 'SP' DocSource
				, @TempLmpNo + ',' + @TempBPSFNo ReffNo
				, GetDate() ReffDate
				, @TempFiscalMonth FiscalMonth
				, @PeriodeNum
				, @Periode
				, @PeriodeName
				, @GLDate
				, 1 BalanceType
				, @Amount AmountDB 
				, @Amount AmountCR
				, 1 Status
				, '' StatusRecon
				, NULL BatchNo
				, '1900-01-01 00:00:00.000' PostingDate 
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 1 Printseq
				, 0 FSend
				, '' SendBy
				, '1900-01-01 00:00:00.000' SendDate
				, @UserID CreatedBy
				, GetDate() CreatedDate
				, @UserID LastUpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			--===============================================================================================================================
			-- GENERATE GLJOURNALDTL
			--===============================================================================================================================
			INSERT INTO GLJournalDtl
			SELECT
				a.CompanyCode CompanyCode
				, a.BranchCode BranchCode
				, @TempFiscalYear FiscalYear
				, @TempJournal JournalNo
				, 1 SeqNo
				, @InTransitAccNo AccountNo
				, @TempLmpNo + ',' + @TempBPSFNo Description
				, 'Harian' JournalType
				, @Amount AmountDB 
				, 0 AmountCR
				, 'INTRANSIT' TypeTrans
				, @AccountTypeIntran
				, @TempLmpNo  DocNo
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 0 FSend
				, @UserID CreatedBy 
				, GetDate() CreatedDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			INSERT INTO GLJournalDtl
			SELECT
				a.CompanyCode CompanyCode
				, a.BranchCode BranchCode
				, @TempFiscalYear FiscalYear
				, @TempJournal JournalNo
				, 2 SeqNo
				, @InventoryAccNo AccountNo
				, @TempLmpNo + ',' + @TempBPSFNo Description
				, 'Harian' JournalType
				, 0 AmountDB
				, @Amount AmountCR
				, 'INVENTORY' TypeTrans
				, @AccountTypeInvent
				, @TempLmpNo  DocNo
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 0 FSend
				, @UserID CreatedBy 
				, GetDate() CreatedDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			UPDATE GnMstDocument
			SET DocumentSequence = DocumentSequence + 1
				, LastUpdateDate = GetDate()
				, LastUpdateBy = @UserID
			WHERE
				1 = 1
				AND CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND DocumentType = 'JTS'
				AND ProfitCenterCode = '300'
				AND DocumentYear = Year(GetDate())
		END
		--===============================================================================================================================
		-- END GENERATE JOURNAL Transtype = 1O (TRANSFER STOCK) --
		--===============================================================================================================================
		
		--===============================================================================================================================
		-- GENERATE JOURNAL
		-- NOTES : Transtype = 14 (OTHERS)
		--===============================================================================================================================
		IF (ISNULL((SELECT TransType FROM SpTrnSLmpHdr WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND LmpNo = @TempLmpNo),0) = '14')
		BEGIN
			--===============================================================================================================================
			-- GENERATE GLINTERFACE
			--===============================================================================================================================
			IF (ISNULL((SELECT TOP 1 DocNo FROM GlInterface WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND DocNo = @TempLmpNo), '') <> '')
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor lampiran sudah ada dalam glInterface, periksa setting-an sequence dokumen (LMP) !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END

			INSERT INTO GLInterface
			SELECT
				a.CompanyCode
				, a.BranchCode
				, a.LmpNo DocNo
				, 1 SeqNo
				, a.LmpDate DocDate
				, '300' ProfitCenterCode
				, GetDate() AccDate
				, @InTransitAccNo AccountNo
				, 'SPAREPART' JournalCode
				, 'BPS' TypeJournal
				, a.LmpNo ApplyTo
				, @Amount AmountDB
				, 0 AmountCR
				, 'INTRANSIT' TypeTrans
				, '' BatchNo
				, '1900-01-01 00:00:00.000' BatchDate
				, 3 StatusFlag
				, @UserID CreateBy 
				, GetDate() CreateDate
				, @UserID UpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			INSERT INTO GLInterface
			SELECT
				a.CompanyCode
				, a.BranchCode
				, a.LmpNo DocNo
				, 2 SeqNo
				, a.LmpDate DocDate
				, '300' ProfitCenterCode
				, GetDate() AccDate
				, @InventoryAccNo AccountNo
				, 'SPAREPART' JournalCode
				, 'BPS' TypeJournal
				, a.LmpNo ApplyTo
				, 0 AmountDB	
				, @Amount AmountCR
				, 'INVENTORY' TypeTrans
				, '' BatchNo
				, '1900-01-01 00:00:00.000' BatchDate
				, 3 StatusFlag
				, @UserID CreateBy 
				, GetDate() CreateDate
				, @UserID UpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			--===============================================================================================================================
			-- GENERATE GLJOURNAL
			--===============================================================================================================================
			SET @MaxTempJournal = ISNULL((SELECT DocumentSequence + 1 FROM GnMstDocument WHERE 
				CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND DocumentType = 'JTI' 
					AND ProfitCenterCode = '300' 
					AND DocumentYear = YEAR(GetDate())),0)

			SET @TempJournalPrefix = ISNULL((SELECT DocumentPrefix FROM GnMstDocument WHERE 
				CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND DocumentType = 'JTI' 
					AND ProfitCenterCode = '300' 
					AND DocumentYear = YEAR(GetDate())),'XXX')

			SET @TempJournal = ISNULL((SELECT @TempJournalPrefix + '/' + RIGHT(YEAR(GETDATE()),2) +'/' + RIGHT(1000000 + CONVERT(VARCHAR, @MaxTempJournal, 1), 6)),'XXX/YY/XXXXXX')
			SET @TempFiscalYear = ISNULL((SELECT FiscalYear FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0) 
			SET @TempFiscalMonth  = ISNULL((SELECT FiscalMonth FROM GnMstCoProfileSpare WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode),0) 
			IF (ISNULL((SELECT TOP 1 JournalNo FROM GlJournal WHERE CompanyCode = @CompanyCode AND BranchCode = @BranchCode AND JournalNo = @TempJournal), '') <> '')
			BEGIN
				SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Nomor journal sudah terpakai, periksa setting-an sequence dokumen (JTS) !'
				RAISERROR (@errmsg,16,1);
				RETURN
			END

			INSERT INTO GLJournal
			SELECT
				CompanyCode
				, BranchCode
				, @TempFiscalYear FiscalYear
				, '300' ProfitCenterCode
				, @TempJournal JournalNo
				, 'Harian' JournalType
				, GetDate() JournalDate
				, 'SP' DocSource
				, @TempLmpNo ReffNo
				, GetDate() ReffDate
				, @TempFiscalMonth FiscalMonth
				, @PeriodeNum
				, @Periode
				, @PeriodeName
				, @GLDate
				, 1 BalanceType
				, @Amount AmountDB 
				, @Amount AmountCR
				, 1 Status
				, '' StatusRecon
				, NULL BatchNo
				, '1900-01-01 00:00:00.000' PostingDate 
				, '' StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 1 Printseq
				, 0 FSend
				, '' SendBy
				, '1900-01-01 00:00:00.000' SendDate
				, @UserID CreatedBy
				, GetDate() CreatedDate
				, @UserID LastUpdateBy
				, GetDate() LastUpdateDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			--===============================================================================================================================
			-- GENERATE GLJOURNALDTL
			--===============================================================================================================================
			INSERT INTO GLJournalDtl
			SELECT
				a.CompanyCode CompanyCode
				, a.BranchCode BranchCode
				, @TempFiscalYear FiscalYear
				, @TempJournal JournalNo
				, 1 SeqNo
				, @InTransitAccNo AccountNo
				, @TempLmpNo Description
				, 'Harian' JournalType
				, @Amount AmountDB 
				, 0 AmountCR
				, 'INTRANSIT' TypeTrans
				, @AccountTypeIntran
				, @TempLmpNo  DocNo
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 0 FSend
				, @UserID CreatedBy 
				, GetDate() CreatedDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			INSERT INTO GLJournalDtl
			SELECT
				a.CompanyCode CompanyCode
				, a.BranchCode BranchCode
				, @TempFiscalYear FiscalYear
				, @TempJournal JournalNo
				, 2 SeqNo
				, @InventoryAccNo AccountNo
				, @TempLmpNo  Description
				, 'Harian' JournalType
				, 0 AmountDB
				, @Amount AmountCR
				, 'INVENTORY' TypeTrans
				, @AccountTypeInvent
				, @TempLmpNo  DocNo
				, 0 StatusReverse
				, '1900-01-01 00:00:00.000' ReverseDate
				, 0 FSend
				, @UserID CreatedBy 
				, GetDate() CreatedDate
			FROM SpTrnSLmpHdr a
			WHERE a.CompanyCode = @CompanyCode
				AND a.BranchCode = @BranchCode
				AND a.LmpNo = @TempLmpNo

			UPDATE GnMstDocument
			SET DocumentSequence = DocumentSequence + 1
				, LastUpdateDate = GetDate()
				, LastUpdateBy = @UserID
			WHERE
				1 = 1
				AND CompanyCode = @CompanyCode
				AND BranchCode = @BranchCode
				AND DocumentType = 'JTI'
				AND ProfitCenterCode = '300'
				AND DocumentYear = Year(GetDate())
		END
		--===============================================================================================================================
		-- END GENERATE JOURNAL Transtype = 14 (OTHERS) --
		--===============================================================================================================================

		--===============================================================================================================================
		-- UPDATE TRANSDATE
		--===============================================================================================================================
		update gnMstCoProfileSpare
		set TransDate=getdate()
			, LastUpdateBy=@UserID
			, LastUpdateDate=getdate()
		where CompanyCode = @CompanyCode AND BranchCode = @BranchCode
		
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		IF @errmsg = ''
			SET @errmsg = 'PEMBUATAN DOKUMEN LAMPIRAN GAGAL ! ' + char(13) + char(13) + 'Error Message: Terdapat proses yang mem-block proses, harap tunggu beberapa saat kemudian coba lagi !'
		RAISERROR (@errmsg,16,1);
		RETURN
	END CATCH
COMMIT TRAN
END

GO

if object_id('sp_spMasterPartSelect4Lookup') is not null
	drop procedure sp_spMasterPartSelect4Lookup
GO
CREATE procedure [dbo].[sp_spMasterPartSelect4Lookup] ( 
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@TypeOfGoods varchar(15)
	,@ProductType varchar(15)
)
AS
	declare @query varchar(max)

declare @DbMD varchar(20)
declare @CompanyMD varchar (20)
declare @BranchMD varchar (20)

set @DbMD = dbo.GetDbMD( @CompanyCode ,@BranchCode ) 
set @CompanyMD = dbo.GetCompanyMD( @CompanyCode , @BranchCode )
set @BranchMD = dbo.GetBranchMD( @CompanyCode , @BranchCode ) 

set @query ='
	SELECT 
	 Items.PartNo
	,Items.ProductType
	,(SELECT LookupValueName 
		FROM gnMstLookupDtl 
	   WHERE CodeID = ''PRCT'' AND 
			 LookUpValue = Items.PartCategory AND 
			 CompanyCode = '+ @CompanyCode +') AS CategoryName
	,Items.PartCategory
	,ItemInfo.PartName
	,CASE ItemInfo.IsGenuinePart WHEN 1 THEN ''Ya'' ELSE ''Tidak'' END AS IsGenuinePart
	,CASE Items.Status WHEN 1 THEN ''Aktif'' ELSE ''Tidak'' END AS IsActive
	,ItemInfo.OrderUnit
	,ItemInfo.SupplierCode
	,Supplier.SupplierName
	,(SELECT LookupValueName 
		FROM gnMstLookupDtl 
	  WHERE CodeID = ''TPGO'' AND 
			LookUpValue = Items.TypeOfGoods AND 
			CompanyCode = '+ @CompanyCode +') AS TypeOfGoods
	FROM '+ @DbMD +'..SpMstItems Items
	INNER JOIN SpMstItemInfo ItemInfo ON Items.PartNo = ItemInfo.PartNo
	LEFT JOIN GnMstSupplier Supplier ON Supplier.SupplierCode = ItemInfo.SupplierCode
	WHERE 1 = 1 
	  AND Items.CompanyCode = '+@CompanyMD+'
	  AND Items.BranchCode  = '+@BranchMD   +' 
	  AND Items.TypeOfGoods = '+@TypeOfGoods +'
	  AND Items.ProductType = '''+@ProductType+ '''
	  AND ItemInfo.CompanyCode = '+@CompanyCode+'
	  AND Supplier.CompanyCode = '+@CompanyCode

	  exec (@query)
GO

if object_id('uspfn_omSlsInvLkpSO') is not null
	drop procedure uspfn_omSlsInvLkpSO
GO
CREATE procedure [dbo].[uspfn_omSlsInvLkpSO]     
(    
 @CompanyCode varchar(15),    
 @BranchCode varchar(15)   
)    
AS    
BEGIN    
-- exec uspfn_omSlsInvLkpSO 6006410,600641001  
    SELECT tableA.SONo,tableA.QtyBPK,tableA.QtyInvoice, tableB.CustomerCode, tableB.CustomerName, tableB.BillTo, tableB.BillName,  
    tableB.Address,tableB.SalesType,tableB.SalesTypeDsc,tableB.TOPDays, tableB.SKPKNo, tableB.RefferenceNo      
      FROM (SELECT a.SONo, sum (b.QuantityBPK)  AS QtyBPK, sum (b.QuantityInvoice)  AS QtyInvoice                     
              FROM omTrSalesBPK a, omTrSalesBPKModel b  
             WHERE a.CompanyCode = b.CompanyCode  
                   AND a.BranchCode = b.BranchCode  
                   AND a.BPKNo = b.BPKNo  
                   AND a.CompanyCode = @CompanyCode  
                   AND a.BranchCode = @BranchCode  
                   AND a.Status = '2'  
             GROUP BY a.SONo) tableA,  
           (SELECT a.SONo, a.CustomerCode, b.CustomerName, a.BillTo, b.CustomerName as BillName,  
   b.Address1 + ' ' + b.Address2 + ' ' + b.Address3 + ' ' + b.Address4 as Address,a.SalesType  
            , (CASE ISNULL(a.SalesType, 0) WHEN 0 THEN 'WholeSales' ELSE 'Direct' END) AS SalesTypeDsc  
            , ISNULL(a.TOPDays, 0) AS TOPDays, a.SKPKNo, a.RefferenceNo  
              FROM omTrSalesSO a  
     LEFT JOIN gnMstCustomer b ON a.CompanyCode = b.CompanyCode AND a.CustomerCode = b.CustomerCode  
             WHERE a.CompanyCode = @CompanyCode  
                   AND a.BranchCode = @BranchCode  
                   AND a.Status = '2') tableB  
    WHERE tableA.QtyBPK > tableA.QtyInvoice AND tableA.SONo = tableB.SONo 
    AND tableA.SONo NOT IN (SELECT z.SONo FROM omTrSalesInvoice z where z.Status <> 3) -- Tambahan Dimas
      
 ORDER BY tableA.SONo  
END
GO

if object_id('usprpt_SpRpTrn010') is not null
	drop procedure usprpt_SpRpTrn010
GO
CREATE procedure [dbo].[usprpt_SpRpTrn010]
--declare	
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@FPJDateStart		DATETIME,
	@FPJDateEnd			DATETIME,
	@FPJNoStart			VARCHAR(30),
	@FPJNoEnd			VARCHAR(30),
	@ProfitCenter		VARCHAR(15),
	@SeqNo				INT
	
--set @CompanyCode = '6115204001'
--set @BranchCode	= '6115204301'
--set @FPJDateStart = '20140801'
--set @FPJDateEnd	= '20141016'
--set @FPJNoStart	= '010.000-14.00000001'
--set @FPJNoEnd = '010.000-14.00000001'
--set @ProfitCenter = '300'
--set @SeqNo = 3

	--exec usprpt_SpRpTrn010 '6115204001','6115204301','20140901','20141022','010.000-14.00000001','010.000-14.00000001','300',3
AS
BEGIN
   
	-- Setting Header Faktur Pajak --
	---------------------------------
	declare @fCompName	varchar(max)
	declare @fAdd		varchar(max)
	declare @fAdd1		varchar(max)
	declare @fAdd2		varchar(max)
	declare @fNPWP		varchar(max)
	declare @fSKP		varchar(max)
	declare @fSKPDate	varchar(max)
	declare @fCity		varchar(max)
	declare @fInv		int

	declare @fStatus varchar(1)
	set @fStatus = 0
	
	declare @fInfoPKP varchar(1)
	set @fInfoPKP = 1

	if exists (select 1 from gnMstLookUpDtl where codeid='FPJFLAG')
	begin
		set @fStatus = (select paravalue from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='STATUS')
	end

	if exists (select * from gnMstLookUpHdr where codeid='FPJ_INFO_PKP')
	begin
		set @fInfoPKP = (select LookupValue from gnmstlookupdtl where codeid='FPJ_INFO_PKP')
	end

	if (@fStatus = '1')
	begin
		set @fCompName	= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='NAME')
		set @fAdd1		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='ADD1')
		set @fAdd2		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='ADD2')
		set @fAdd		= @fAdd1+' '+@fAdd2
		set @fNPWP		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='NPWP')
		set @fSKP		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='SKPNO')
		set @fSKPDate	= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='SKPDATE')
		set @fCity		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='CITY')		
	end
	set @fInv		= (select isnull(ParaValue,'1') from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='SPARE')		

	-- parameter use address holding or not
	declare @IsHoldingAddr as bit
	if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR') > 0
		set @IsHoldingAddr = (select convert(numeric,LookUpValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR')
	else
		set @IsHoldingAddr = 0
	declare @Flag bit
	set @Flag = (select ParaValue from gnMstLookUpDtl where CompanyCode = @CompanyCode and CodeID = 'FLPG' and LookUpValue = 'NJS')

	select * into #t1 from(
	SELECT a.TPTrans
		, case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fCompName else e.CompanyGovName end)
		  else '' end as compNm
		, case @fStatus when '1' then @fSKP else e.SKPNo end as compSKPNo
		, case @fStatus when '1' then @fSKPDate else e.SKPDate end as compSKPDate
		, case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd1 else 
				(case when @IsHoldingAddr=0 then isnull(e.Address1,'') +' '+isnull(e.Address2,'')+' '+ isnull(e.Address3,'')
					else (select isnull(Address1,'') +' '+isnull(Address2,'')+' '+ isnull(Address3,'') from gnMstCoProfile where CompanyCode=@CompanyCode
					and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
				end)end)
		  else '' end AS compAddr1
		, case @fStatus when '1' then @fAdd2 else 
			(case when @IsHoldingAddr=0 then isnull(e.Address2,'')
				else (select isnull(Address2,'') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)
		 end AS compAddr2
		, case @fStatus when '1' then '' else 
			(case when @IsHoldingAddr=0 then isnull(e.Address3,'')
				else (select isnull(Address3,'') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)
		 end AS compAddr3
		, case @fStatus when '1' then '' else 
			(case when @IsHoldingAddr=0 then isnull(e.Address4,'')
				else (select isnull(Address4,'') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)
		 end AS compAddr4
		, case @fStatus when '1' then '' else e.PhoneNo end as compPhoneNo
		, case @fStatus when '1' then '' else e.FaxNo end as compFaxNo
		, case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fNPWP else e.NPWPNo end)
		  else '' end as compNPWPNo
		, a.FPJNo  fakturFPJNo
		, a.FPJDate  fakturFPJDate
		, a.InvoiceNo  fakturInvNo
		, a.InvoiceDate  fakturInvDate
		, a.FPJGovNo  fakturFPJGovNo
		, a.PickingSlipNo  fakturPickSlipNo
		, a.CustomerCode  fakturCustCode
		, x.CustomerName  custName
		, d.SKPNo  custSKPNo
		, d.SKPDate  custSKPDate
		, isnull(x.Address1,'')  custAddr1
		, isnull(x.Address2,'')  custAddr2
		, isnull(x.Address3,'')  custAddr3
		, isnull(x.Address4,'')  custAddr4
		, d.PhoneNo  custPhoneNo
		, d.FaxNo  custFaxNo
		, d.NPWPNo  custNPWPNo
		, a.DueDate  fakturDueDate
		, a.TotSalesQty  fakturTotSaleQty
		, a.TotSalesAmt  fakturTotSalesAmt
		, a.TotDiscAmt  fakturTotDiscAmt
		, a.TotDppAmt  fakturTotDppAmt
		, a.TotPPNAmt  fakturPPNAmt
		, a.TotFinalSalesAmt  fakturTotFinalSalesAmt
		, a.FPJSignature
		, c.TaxPct  taxPercent
		, ISNULL((SELECT TOP 1 ParaValue FROM GnMstLookUpDtl WHERE CompanyCode = @CompanyCode AND CodeID = 'FPIF'),'0') FlagDetails
		, case @fStatus when '1' then @fCity else 
			(SELECT LookUpValueName FROM gnMstLookupDtl WHERE CodeID = 'CITY' AND LookUpValue = e.CityCode) end as cityNm
		, UPPER (f.SignName)  SignName
		, UPPER (f.TitleSign)  TitleSign
		, a.InvoiceNo 
		, isnull(@Flag, 0) Flag
		, isnull(@fInv,1) ShowInvoice
    FROM SpTrnSFPJHdr a WITH (NOLOCK, NOWAIT)
	LEFT JOIN GnMstTax c WITH (NOLOCK, NOWAIT)ON a.CompanyCode = c.CompanyCode AND c.TaxCode = 'PPN'
	LEFT JOIN GnMstCustomer d WITH (NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
		AND a.CustomerCode = d.CustomerCode
	LEFT JOIN SpTrnSFPJInfo x WITH (NOLOCK, NOWAIT) ON a.CompanyCode = x.CompanyCode
		AND a.BranchCode = x.BranchCode
		AND a.FPJNo = x.FPJNo
	LEFt JOIN GnMstCoProfile e WITH (NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
		AND a.BranchCode = e.BranchCode
	LEFT JOIN GnMstSignature f WITH (NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
		AND a.BranchCode = f.BranchCode
		AND f.ProfitCenterCode = @ProfitCenter
		AND f.DocumentType = CONVERT (VARCHAR (3), a.FPJNo)
		AND f.SeqNo = @SeqNo
    WHERE a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode 
		AND a.isPKP = 1
		AND CONVERT(VARCHAR, a.FPJSignature, 112) BETWEEN @FPJDateStart AND @FPJDateEnd
		AND ((CASE WHEN @FPJNoStart = '' THEN a.FPJGovNo END) <> ''
			OR (CASE WHEN @FPJNoStart <> '' THEN a.FPJGovNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			) #t1
				
select TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo,  
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo) + '-' + (select substring((select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc), 8, 7)) else (select fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturFPJNo, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturFPJDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select fakturFPJDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturFPJDate,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo) + '-' + (select substring((select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo desc), 8, 7)) else (select fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturInvNo, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturInvDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo desc) else (select fakturInvDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturInvDate, fakturFPJGovNo,  
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo) + '-' + (select substring((select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc), 8, 7)) else (select fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturPickSlipNo, 
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturDueDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select fakturDueDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturDueDate, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotSaleQty) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotSaleQty from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotSaleQty, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotSalesAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotSalesAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotSalesAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDiscAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotDiscAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotDiscAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotDppAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotDppAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturPPNAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturPPNAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturPPNAmt,
	--case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + ((select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) * 0.1) else (select fakturTotFinalSalesAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotFinalSalesAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + (select sum(fakturPPNAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotFinalSalesAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotFinalSalesAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end FPJSignature, 
	taxPercent, FlagDetails, cityNm, SignName, TitleSign, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 InvoiceNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJGovNo) + ' s/d ' + (select top 1 InvoiceNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJGovNo desc) else (select InvoiceNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end InvoiceNo, 
	Flag, ShowInvoice
from #t1 a
group by TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJGovNo, 
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, taxPercent, FlagDetails,
	cityNm, SignName, TitleSign, Flag, ShowInvoice			
	
drop table #t1	
END
GO

if object_id('usprpt_SpRpTrn035') is not null
	drop procedure usprpt_SpRpTrn035
GO
CREATE procedure [dbo].[usprpt_SpRpTrn035]
--declare
   @CompanyCode		VARCHAR(15),
   @BranchCode		VARCHAR(15),
   @FPJDateStart	DATETIME,
   @FPJDateEnd		DATETIME,
   @FPJNoStart		VARCHAR(30),
   @FPJNoEnd		VARCHAR(30),
   @ProfitCenter	VARCHAR(15),
   @SeqNo			INT
   
AS   
BEGIN

--set @CompanyCode = '6115204001'
--set @BranchCode	= '6115204301'
--set @FPJDateStart = '20140801'
--set @FPJDateEnd	= '20141030'
--set @FPJNoStart	= '010.000-14.00000016'
--set @FPJNoEnd = '010.000-14.00000016'
--set @ProfitCenter = '300'
--set @SeqNo = 3

-- Setting Header Faktur Pajak --
---------------------------------
declare @fCompName	varchar(max)
declare @fAdd		varchar(max)
declare @fAdd1		varchar(max)
declare @fAdd2		varchar(max)
declare @fNPWP		varchar(max)
declare @fSKP		varchar(max)
declare @fSKPDate	varchar(max)
declare @fCity		varchar(max)
declare @fInv		int

declare @fStatus varchar(1)
set @fStatus = 0

declare @fInfoPKP varchar(1)
set @fInfoPKP = 1

if exists (select 1 from gnMstLookUpDtl where codeid='FPJFLAG')
begin
	set @fStatus = (select paravalue from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='STATUS')
end

if exists (select * from gnMstLookUpHdr where codeid='FPJ_INFO_PKP')
begin
	set @fInfoPKP = (select LookupValue from gnmstlookupdtl where codeid='FPJ_INFO_PKP')
end

if (@fStatus = '1')
begin
	set @fCompName	= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='NAME')
	set @fAdd1		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='ADD1')
	set @fAdd2		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='ADD2')
	set @fAdd		= @fAdd1+' '+@fAdd2
	set @fNPWP		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='NPWP')
	set @fSKP		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='SKPNO')
	set @fSKPDate	= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='SKPDATE')
	set @fCity		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='CITY')		
end
set @fInv		= (select isnull(ParaValue,'1') from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='SPARE')		

-- parameter use address holding or not
declare @IsHoldingAddr as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR') > 0
	set @IsHoldingAddr = (select convert(numeric,LookUpValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR')
else
	set @IsHoldingAddr = 0

declare @PaymentInfo as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='PINF') > 0
	set @PaymentInfo = (select convert(numeric,ParaValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='PINF' AND LookUpValue='STAT')
else
	set @PaymentInfo = 1

select * into #t1
from (
	SELECT	
		a.TPTrans, 
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fCompName else e.CompanyGovName end)
		else '' end as compNm, 
		case @fStatus when '1' then @fSKP else e.SKPNo end as compSKPNo, 		
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fSKPDate else e.SKPDate end)
		else '' end as compSKPDate,
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd1 else 
				(case when @IsHoldingAddr=0 then ISNULL(e.Address1, '') + ' ' + ISNULL(e.Address2, '') + ' ' + ISNULL(e.Address3, '')
					else (select ISNULL(Address1, '') + ' ' + ISNULL(Address2, '') + ' ' + ISNULL(Address3, '') from gnMstCoProfile where CompanyCode=@CompanyCode
					and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
				end)end)
		else '' end AS compAddr1,
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd2 else 
				(case when @IsHoldingAddr=0 then ISNULL(e.Address2, '')
					else (select ISNULL(e.Address2, '') from gnMstCoProfile where CompanyCode=@CompanyCode
					and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
				end)end)
		else '' end AS compAddr2
		,case @fStatus when '1' then '' else 
			(case when @IsHoldingAddr=0 then ISNULL(e.Address3, '')
				else (select ISNULL(e.Address3, '') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)
		 end AS compAddr3
		,case @fStatus when '1' then '' else 
			(case when @IsHoldingAddr=0 then ISNULL(e.Address4, '')
				else (select ISNULL(e.Address4, '') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)
		 end AS compAddr4,
		case @fStatus when '1' then '' else e.PhoneNo end as compPhoneNo, 
		case @fStatus when '1' then '' else e.FaxNo  end as compFaxNo, 
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fNPWP else e.NPWPNo  end)
		else '' end as compNPWPNo, 
		a.FPJNo  fakturFPJNo, 
		a.FPJDate  fakturFPJDate, 
		a.InvoiceNo  fakturInvNo,
		a.InvoiceDate  fakturInvDate, 
		a.FPJGovNo  fakturFPJGovNo, 
		a.PickingSlipNo  fakturPickSlipNo,
		/* RETURN MORE THAN 1 VALUE NEED MORE CHECK, TEMPORARY USING TOP 1 */ 
		--New--
		(SELECT TOP 1 g.OrderNo+', '+CONVERT(VARCHAR(20),g.OrderDate,105)
			FROM spTrnSORDHdr g
				LEFT JOIN spTrnSFPJDtl h ON g.CompanyCode = h.CompanyCode AND g.BranchCode = h.BranchCode AND g.DocNo = h.DocNo
			WHERE h.CompanyCode = a.CompanyCode AND h.CompanyCode = a.CompanyCode AND h.FPJNo = a.FPJNo
			GROUP BY g.OrderNo, g.OrderDate
		 )as OrderFeld,
		--End new 
		a.CustomerCode  fakturCustCode,
		x.CustomerName  custName, 
		d.SKPNo  custSKPNo, 
		d.SKPDate custSKPDate, 
		x.Address1 custAddr1, 
		x.Address2 custAddr2, 
		x.Address3 custAddr3, 
		x.Address4 custAddr4, 
		d.PhoneNo custPhoneNo, 
		d.FaxNo custFaxNo, 
		d.NPWPNo custNPWPNo, 
		a.DueDate fakturDueDate, 
		a.TotSalesQty fakturTotSaleQty, 
		a.TotSalesAmt fakturTotSalesAmt, 
		a.TotDiscAmt fakturTotDiscAmt, 
		a.TotDppAmt fakturTotDppAmt, 
		a.TotPPNAmt fakturPPNAmt, 
		a.TotFinalSalesAmt fakturTotFinalSalesAmt, 
		a.FPJSignature, 
		c.TaxPct  taxPercent, 
		case @fStatus when '1' then @fCity else 
		(SELECT LookUpValueName FROM gnMstLookupDtl WHERE CodeID = 'CITY' AND LookUpValue = e.CityCode) end as cityNm, 
		UPPER (f.SignName) SignName, 
		UPPER (f.TitleSign) TitleSign,
		g.PartNo,
		h.PartName,
		g.QtyBill,
		g.SalesAmt,
		g.DiscAmt,
		(
			SELECT SUM(QtyBill) FROM spTrnSFPJDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND FPJNo = a.FPJNo
		) AS TotQtyBill,
		(
		SELECT COUNT (PartNo) FROM spTrnSFPJDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND FPJNo = a.FPJNo
		) AS JumlahPart
		,case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd1 else 
			(case when @IsHoldingAddr=0 then e.Address1+' '+e.Address2
				else (select Address1+' '+Address2 from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)end)
		 else '' end as Alamat1
		,case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd2 else 
			(case when @IsHoldingAddr=0 then e.Address3+' '+e.Address4
				else (select Address3+' '+Address4 from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)end)
		 else '' end as Alamat2,
		ISNULL(x.Address1,'')+' '+ISNULL(x.Address2,'') Alamat3,
		ISNULL(x.Address3,'')+' '+ISNULL(x.Address4,'') Alamat4,
		g.PartNo+' - '+h.PartName Item
		,CASE 
			WHEN @PaymentInfo=1 THEN 'DILUNASI DENGAN ' + (select LookUpValueName from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID='PYBY' and LookUpValue= i.PaymentCode)
			ELSE '' 
		end PaymentInfo,
		 ISNULL((SELECT TOP 1 ParaValue FROM GnMstLookUpDtl WHERE CompanyCode = @CompanyCode AND CodeID = 'FPIF'),'0') FlagDetails
		,(select count(PartNo) from spTrnSFPJDtl where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and FPJNo=a.FPJNo) MaxRow
	FROM 
		SpTrnSFPJHdr a WITH (NOLOCK, NOWAIT)
	LEFT JOIN GnMstTax c WITH (NOLOCK, NOWAIT)
		ON a.CompanyCode = c.CompanyCode AND c.TaxCode = 'PPN'
	LEFT JOIN GnMstCustomer d WITH (NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
		AND a.CustomerCode = d.CustomerCode
	LEFT JOIN SpTrnSFPJInfo x WITH (NOLOCK, NOWAIT) ON a.CompanyCode = x.CompanyCode
		AND a.BranchCode = x.BranchCode
		AND a.FPJNo = x.FPJNo
	LEFt JOIN GnMstCoProfile e WITH (NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
		AND a.BranchCode = e.BranchCode
	LEFT JOIN GnMstSignature f WITH (NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
		AND a.BranchCode = f.BranchCode
		AND f.ProfitCenterCode = @ProfitCenter
		AND f.DocumentType = CONVERT (VARCHAR (3), a.FPJNo)
		AND f.SeqNo = @SeqNo
	LEFT JOIN spTrnSFPJDtl g WITH (NOLOCK, NOWAIT) ON g.CompanyCode = a.CompanyCode
		AND g.BranchCode = a.BranchCode
		AND g.FPJNo = a.FPJNo
	INNER JOIN spMstItemInfo h WITH (NOLOCK, NOWAIT) ON h.CompanyCode = a.CompanyCode
		AND h.PartNo = g.PartNo
	LEFT JOIN gnMstCustomerProfitCenter i on a.CompanyCode=i.CompanyCode and a.BranchCode=i.BranchCode
		AND a.CustomerCode=i.CustomerCode and i.ProfitCenterCode='300'
	WHERE 
		a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode AND a.isPKP = 1
		AND CONVERT(VARCHAR, a.FPJSignature, 112) BETWEEN @FPJDateStart AND @FPJDateEnd
		AND ((CASE WHEN @FPJNoStart = '' THEN a.FPJGovNo END) <> ''
		OR (CASE WHEN @FPJNoStart <> '' THEN a.FPJGovNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
) #t1

select * into #t2 from(
select TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo,
	case when (a.countFaktur) > 1 then (select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo) + '-' + (select substring((select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc), 8, 7)) else (select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturFPJNo, 
	case when (a.countFaktur) > 1 then (select top 1 fakturFPJDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select top 1 fakturFPJDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturFPJDate,
	case when (a.countFaktur) > 1 then (select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo) + '-' + (select substring((select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo desc), 8, 7)) else (select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturInvNo, 
	case when (a.countFaktur) > 1 then (select top 1 fakturInvDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo desc) else (select top 1 fakturInvDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturInvDate,
	fakturFPJGovNo,  
	case when (a.countFaktur) > 1 then (select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo) + '-' + (select substring((select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc), 8, 7)) else (select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturPickSlipNo,
	case when (a.countFaktur) > 1 then ' ' else (select top 1 OrderFeld from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end OrderFeld,
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, 
	case when (a.countFaktur) > 1 then (select top 1 fakturDueDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select top 1 fakturDueDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturDueDate,
	
	case when (a.countFaktur) > 1 then (select sum(QtyBill) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(fakturTotSaleQty) end fakturTotSaleQty, 
	case when (a.countFaktur) > 1 then (select sum(SalesAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(fakturTotSalesAmt) end fakturTotSalesAmt, 
	case when (a.countFaktur) > 1 then (select sum(DiscAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(fakturTotDiscAmt) end fakturTotDiscAmt,
	case when (a.countFaktur) > 1 then (select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(fakturTotDPPAmt) end fakturTotDppAmt, 
	--case when (a.countFaktur) > 1 then ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) * 0.1) else sum(fakturPPNAmt) end fakturPPNAmt, 
	sum(fakturPPNAmt) fakturPPNAmt, 
	--case when (a.countFaktur) > 1 then ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) * 0.1)) else sum(fakturTotFinalSalesAmt) end fakturTotFinalSalesAmt, 
	case when (a.countFaktur) > 1 then ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + (select sum(fakturPPNAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo)) else sum(fakturTotFinalSalesAmt) end fakturTotFinalSalesAmt, 

	case when (a.countFaktur) > 1 then (select top 1 FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select top 1 FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end FPJSignature,
	TaxPercent, cityNm, SignName, TitleSign, PartNo, PartName, sum(QtyBill) QtyBill, sum(SalesAmt) SalesAmt, 
	case when (a.countFaktur) > 1 then (select sum(QtyBill) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(TotQtyBill) end TotQtyBill, 
	sum(JumlahPart) JumlahPart, Alamat1, Alamat2, Alamat3, Alamat4, Item, PaymentInfo, FlagDetails, sum(MaxRow) MaxRow, a.countFaktur
from
(
select distinct TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJNo,
	fakturFPJDate, fakturInvNo, fakturInvDate, #t1.fakturFPJGovNo, fakturPickSlipNo, OrderFeld, fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, 
	custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, fakturTotSaleQty, fakturTotSalesAmt, fakturTotDiscAmt, fakturTotDPPAmt,
	fakturPPNAmt, fakturTotFinalSalesAmt, FPJSignature, TaxPercent, cityNm, SignName, TitleSign, PartNo, PartName, sum(QtyBill) QtyBill, sum(SalesAmt) SalesAmt,
	sum(DiscAmt) DiscAmt, TotQtyBill, JumlahPart, Alamat1, Alamat2, Alamat3, Alamat4, Item, PaymentInfo, FlagDetails, b.countFaktur, sum(MaxRow) MaxRow
from #t1 
left join (select fakturFPJGovNo, count(fakturFPJNo) countFaktur
		from(select distinct fakturFPJGovNo, fakturFPJNo from #t1) a
		group by fakturFPJGovNo) b on #t1.fakturFPJGovNo = b.fakturFPJGovNo
group by TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJNo,
	fakturFPJDate, fakturInvNo, fakturInvDate, #t1.fakturFPJGovNo, fakturPickSlipNo, OrderFeld, fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, 
	custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, fakturTotSaleQty, fakturTotSalesAmt, fakturTotDiscAmt, fakturTotDPPAmt,
	fakturPPNAmt, fakturTotFinalSalesAmt, FPJSignature, TaxPercent, cityNm, SignName, TitleSign, PartNo, PartName, Alamat1, Alamat2, Alamat3, Alamat4, Item, 
	PaymentInfo, FlagDetails, b.countFaktur, JumlahPart, TotQtyBill
) a
group by fakturFPJGovNo, TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, 
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, TaxPercent, cityNm, 
	SignName, TitleSign, PartNo, PartName, Alamat1, Alamat2, Alamat3, Alamat4, PaymentInfo, FlagDetails, item, a.countFaktur
) #t2

	
select TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJNo, fakturFPJDate
	, fakturInvDate, fakturFPJGovNo, fakturPickSlipNo, OrderFeld, fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3
	, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, fakturTotSaleQty
	, fakturTotSalesAmt, fakturTotDiscAmt, fakturTotDppAmt, fakturPPNAmt
	, fakturTotFinalSalesAmt, FPJSignature, TaxPercent, cityNm, SignName, TitleSign
	, PartNo, PartName, QtyBill, SalesAmt, TotQtyBill
	, case when (a.countFaktur) > 1 then (select count(Item) from #t2 where fakturFPJGovNo = a.fakturFPJGovNo) else JumlahPart end JumlahPart
	, Alamat1, Alamat2, Alamat3, Alamat4, Item, PaymentInfo, FlagDetails
	, case when (a.countFaktur) > 1 then (select count(Item) from #t2 where fakturFPJGovNo = a.fakturFPJGovNo) else MaxRow end MaxRow
	, case when (case when (a.countFaktur) > 1 then (select count(Item) from #t2 where fakturFPJGovNo = a.fakturFPJGovNo) else MaxRow end) % 17 = 1 then 16 else 17 end as PageBreak
	, isnull(@fInv,1) ShowInvoice
from #t2 a

	drop table #t1, #t2
END
GO

if object_id('usprpt_SpRpTrn035Pre') is not null
	drop procedure usprpt_SpRpTrn035Pre
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<MASTER FAKTUR PAJAK DETAIL>
-- [usprpt_SpRpTrn035Pre] '6006406','6006406','20110901','20111130','010.000-11.00038687','010.000-11.00038687','300','1'
-- =============================================

CREATE procedure [dbo].[usprpt_SpRpTrn035Pre]
--DECLARE
   @CompanyCode		VARCHAR(15),
   @BranchCode		VARCHAR(15),
   @FPJDateStart DATETIME,
   @FPJDateEnd DATETIME,
   @FPJNoStart		VARCHAR(30),
   @FPJNoEnd		VARCHAR(30),
   @ProfitCenter	VARCHAR(15),
   @SeqNo INT

--select @CompanyCode= '6114201',@BranchCode='611420100',@FPJDateStart='20150501',@FPJDateEnd='20150529',@FPJNoStart='010.001-15.70826867',
--@FPJNoEnd='010.001-15.70826867',@ProfitCenter='300',@SeqNo=1


AS   
BEGIN

-- Setting Header Faktur Pajak --
---------------------------------
declare @fCompName	varchar(max)
declare @fAdd		varchar(max)
declare @fAdd1		varchar(max)
declare @fAdd2		varchar(max)
declare @fNPWP		varchar(max)
declare @fSKP		varchar(max)
declare @fSKPDate	varchar(max)
declare @fCity		varchar(max)
declare @fInv		int

declare @fStatus varchar(1)
set @fStatus = 0

declare @fInfoPKP varchar(1)
set @fInfoPKP = 1

if exists (select 1 from gnMstLookUpDtl where codeid='FPJFLAG')
begin
	set @fStatus = (select paravalue from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='STATUS')
end

if exists (select * from gnMstLookUpHdr where codeid='FPJ_INFO_PKP')
begin
	set @fInfoPKP = (select LookupValue from gnmstlookupdtl where codeid='FPJ_INFO_PKP')
end

if (@fStatus = '1')
begin
	set @fCompName	= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='NAME')
	set @fAdd1		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='ADD1')
	set @fAdd2		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='ADD2')
	set @fAdd		= @fAdd1+' '+@fAdd2
	set @fNPWP		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='NPWP')
	set @fSKP		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='SKPNO')
	set @fSKPDate	= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='SKPDATE')
	set @fCity		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='CITY')		
end
set @fInv		= (select isnull(ParaValue,'1') from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='SPARE')		

-- parameter use address holding or not
declare @IsHoldingAddr as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR') > 0
	set @IsHoldingAddr = (select convert(numeric,LookUpValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR')
else
	set @IsHoldingAddr = 0

declare @PaymentInfo as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='PINF') > 0
	set @PaymentInfo = (select convert(numeric,ParaValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='PINF' AND LookUpValue='STAT')
else
	set @PaymentInfo = 1

	declare @Flag bit
	set @Flag = (select ParaValue from gnMstLookUpDtl where CompanyCode = @CompanyCode and CodeID = 'FLPG' and LookUpValue = 'NJS')

select * into #t1 from(
SELECT Distinct
	a.TPTrans, 
	case when @fInfoPKP = 1 then 
		(case @fStatus when '1' then @fCompName else e.CompanyGovName  end)
	else '' end as compNm, 
	case @fStatus when '1' then @fSKP else e.SKPNo end as compSKPNo, 
	case @fStatus when '1' then @fSKPDate else e.SKPDate end as compSKPDate,
	case when @fInfoPKP = 1 then 
		(case @fStatus when '1' then @fAdd1 else 
			(case when @IsHoldingAddr=0 then ISNULL(e.Address1, '') + ' ' + ISNULL(e.Address2, '') + ' ' + ISNULL(e.Address3, '')
				else (select ISNULL(Address1, '') + ' ' + ISNULL(Address2, '') + ' ' + ISNULL(Address3, '') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)
		end)
	 else '' end AS compAddr1
	,case @fStatus when '1' then @fAdd2 else 
		(case when @IsHoldingAddr=0 then ISNULL(e.Address2, '')
			else (select ISNULL(e.Address2, '') from gnMstCoProfile where CompanyCode=@CompanyCode
			and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
		end)
	 end AS compAddr2
	,case @fStatus when '1' then '' else 
		(case when @IsHoldingAddr=0 then ISNULL(e.Address3, '')
			else (select ISNULL(e.Address3, '') from gnMstCoProfile where CompanyCode=@CompanyCode
			and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
		end)
	 end AS compAddr3
	,case @fStatus when '1' then '' else 
		(case when @IsHoldingAddr=0 then ISNULL(e.Address4, '')
			else (select ISNULL(e.Address4, '') from gnMstCoProfile where CompanyCode=@CompanyCode
			and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
		end)
	 end AS compAddr4,
	case @fStatus when '1' then '' else e.PhoneNo end as compPhoneNo, 
	case @fStatus when '1' then '' else e.FaxNo  end as compFaxNo, 
	case when @fInfoPKP = 1 then 
		(case @fStatus when '1' then @fNPWP else e.NPWPNo  end)
	else '' end as compNPWPNo, 
	a.FPJNo  fakturFPJNo, 
	a.FPJDate  fakturFPJDate, 
	a.InvoiceNo  fakturInvNo,
	a.InvoiceDate  fakturInvDate, 
	a.FPJGovNo  fakturFPJGovNo, 
	a.PickingSlipNo  fakturPickSlipNo,
	/* RETURN MORE THAN 1 VALUE NEED MORE CHECK, TEMPORARY USING TOP 1 */ 
	--New--
	(SELECT TOP 1 g.OrderNo+', '+CONVERT(VARCHAR(20),g.OrderDate,105)
		FROM spTrnSORDHdr g
			LEFT JOIN spTrnSFPJDtl h ON g.CompanyCode = h.CompanyCode AND g.BranchCode = h.BranchCode AND g.DocNo = h.DocNo
		WHERE h.CompanyCode = a.CompanyCode AND h.CompanyCode = a.CompanyCode AND h.FPJNo = a.FPJNo
		GROUP BY g.OrderNo, g.OrderDate
	 )as OrderFeld,
	--End new 
	a.CustomerCode  fakturCustCode,
	x.CustomerName  custName, 
	d.SKPNo  custSKPNo, 
	d.SKPDate custSKPDate, 
	x.Address1 custAddr1, 
	x.Address2 custAddr2, 
	x.Address3 custAddr3, 
	x.Address4 custAddr4, 
	d.PhoneNo custPhoneNo, 
	d.FaxNo custFaxNo, 
	d.NPWPNo custNPWPNo, 
	a.DueDate fakturDueDate, 
	a.TotSalesQty fakturTotSaleQty, 
	a.TotSalesAmt fakturTotSalesAmt, 
	a.TotDiscAmt fakturTotDiscAmt, 
	a.TotDppAmt fakturTotDppAmt, 
	a.TotPPNAmt fakturPPNAmt, 
	a.TotFinalSalesAmt fakturTotFinalSalesAmt, 
	a.FPJSignature, 
	c.TaxPct  taxPercent, 
	case @fStatus when '1' then @fCity else 
	(SELECT LookUpValueName FROM gnMstLookupDtl WHERE CodeID = 'CITY' AND LookUpValue = e.CityCode) end as cityNm, 
	UPPER (f.SignName) SignName, 
	UPPER (f.TitleSign) TitleSign,
--	g.PartNo,
--	h.PartName,
--	g.QtyBill,
--	g.SalesAmt,
	(
		SELECT SUM(QtyBill) FROM spTrnSFPJDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND FPJNo = a.FPJNo
	) AS TotQtyBill,
	(
	SELECT COUNT (PartNo) FROM spTrnSFPJDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND FPJNo = a.FPJNo
	) AS JumlahPart
	,case @fStatus when '1' then @fAdd1 else 
		(case when @IsHoldingAddr=0 then e.Address1+' '+e.Address2
			else (select Address1+' '+Address2 from gnMstCoProfile where CompanyCode=@CompanyCode
			and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
		end)
	 end as Alamat1
	,case @fStatus when '1' then @fAdd2 else 
		(case when @IsHoldingAddr=0 then e.Address3+' '+e.Address4
			else (select Address3+' '+Address4 from gnMstCoProfile where CompanyCode=@CompanyCode
			and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
		end)
	 end as Alamat2,
	ISNULL(x.Address1,'')+' '+ISNULL(x.Address2,'') Alamat3,
	ISNULL(x.Address3,'')+' '+ISNULL(x.Address4,'') Alamat4,
--	g.PartNo+' - '+h.PartName Item,
	(SELECT COUNT (FPJNo) FROM spTrnSFPJDtl WHERE CompanyCode = g.CompanyCode AND BranchCode = g.BranchCode AND FPJNo = a.FPJNo) MaxRow
	,CASE 
		WHEN @PaymentInfo=1 THEN 'DILUNASI DENGAN ' + (select LookUpValueName from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID='PYBY' and LookUpValue= i.PaymentCode)
		ELSE '' 
	end PaymentInfo,
	 ISNULL((SELECT TOP 1 ParaValue FROM GnMstLookUpDtl WHERE CompanyCode = @CompanyCode AND CodeID = 'FPIF'),'0') FlagDetails	
	,16 PageBreak
, isnull(@Flag, 0) Flag
, isnull(@fInv,1) ShowInvoice
FROM 
	SpTrnSFPJHdr a WITH (NOLOCK, NOWAIT)
LEFT JOIN GnMstTax c WITH (NOLOCK, NOWAIT)
	ON a.CompanyCode = c.CompanyCode AND c.TaxCode = 'PPN'
LEFT JOIN GnMstCustomer d WITH (NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
	AND a.CustomerCode = d.CustomerCode
LEFT JOIN SpTrnSFPJInfo x WITH (NOLOCK, NOWAIT) ON a.CompanyCode = x.CompanyCode
	AND a.BranchCode = x.BranchCode
	AND a.FPJNo = x.FPJNo
LEFt JOIN GnMstCoProfile e WITH (NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	AND a.BranchCode = e.BranchCode
LEFT JOIN GnMstSignature f WITH (NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
	AND a.BranchCode = f.BranchCode
	AND f.ProfitCenterCode = @ProfitCenter
	AND f.DocumentType = CONVERT (VARCHAR (3), a.FPJNo)
	AND f.SeqNo = @SeqNo
LEFT JOIN spTrnSFPJDtl g WITH (NOLOCK, NOWAIT) ON g.CompanyCode = a.CompanyCode
	AND g.BranchCode = a.BranchCode
	AND g.FPJNo = a.FPJNo
INNER JOIN spMstItemInfo h WITH (NOLOCK, NOWAIT) ON h.CompanyCode = a.CompanyCode
	AND h.PartNo = g.PartNo
LEFT JOIN gnMstCustomerProfitCenter i on a.CompanyCode=i.CompanyCode and a.BranchCode=i.BranchCode
	AND a.CustomerCode=i.CustomerCode and i.ProfitCenterCode='300'
WHERE 
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode AND a.isPKP = 1
	AND CONVERT(VARCHAR, a.FPJSignature, 112) BETWEEN @FPJDateStart AND @FPJDateEnd
	AND ((CASE WHEN @FPJNoStart = '' THEN a.FPJGovNo END) <> ''
	OR (CASE WHEN @FPJNoStart <> '' THEN a.FPJGovNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
	) #t1

select TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo,  
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo) + '-' + (select substring((select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc), 8, 7)) else (select fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturFPJNo, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturFPJDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select fakturFPJDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturFPJDate,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo) + '-' + (select substring((select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo desc), 8, 7)) else (select fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturInvNo, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturInvDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo desc) else (select fakturInvDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturInvDate, fakturFPJGovNo,  
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo) + '-' + (select substring((select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc), 8, 7)) else (select fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturPickSlipNo, 	
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturDueDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select fakturDueDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturDueDate, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotSaleQty) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotSaleQty from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotSaleQty, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotSalesAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotSalesAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotSalesAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDiscAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotDiscAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotDiscAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotDppAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotDppAmt,
	--case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then ((select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) * 0.1) else (select fakturPPNAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturPPNAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then ((select sum(fakturPPNAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo)) else (select fakturPPNAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturPPNAmt,
	--case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + ((select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) * 0.1) else (select fakturTotFinalSalesAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotFinalSalesAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + (select sum(fakturPPNAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotFinalSalesAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotFinalSalesAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end FPJSignature, 
	taxPercent, cityNm, SignName, TitleSign, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(TotQtyBill) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select TotQtyBill from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end TotQtyBill,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(JumlahPart) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select JumlahPart from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end JumlahPart,
	Alamat1, Alamat2, Alamat3, Alamat4, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 OrderFeld from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select OrderFeld from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end OrderFeld,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(MaxRow) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select MaxRow from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end MaxRow,
	PaymentInfo, FlagDetails, PageBreak, Flag, ShowInvoice
from #t1 a
group by TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJGovNo, 
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, taxPercent, FlagDetails,
	cityNm, SignName, TitleSign, Flag, ShowInvoice, Alamat1, Alamat2, Alamat3, Alamat4, PaymentInfo, PageBreak, FlagDetails

drop table #t1		
END
GO

if object_id('uspfn_SvTrnPDIFSC') is not null
	drop PROCEDURE uspfn_SvTrnPDIFSC
GO
CREATE PROCEDURE [dbo].[uspfn_SvTrnPDIFSC]
 @CompanyCode varchar(20),   
 @ProductType varchar(15),
 @GenerateNoFrom    varchar(15),  
 @GenerateNoTo    varchar(15) ,
 @SourceData char(1)
as        

select 
a.BranchCode
,a.GenerateNo
,a.GenerateDate
,b.GenerateSeq
,a.SenderDealerCode
,a.SenderDealerName
,a.FpjNo
,case convert(varchar(10), a.FpjDate, 121) when '1900-01-01' then null else a.FpjDate end FpjDate
,a.RefferenceNo
,case convert(varchar(10), a.RefferenceDate, 121) when '1900-01-01' then null else a.RefferenceDate end RefferenceDate
,coalesce(b.SuzukiRefferenceNo, '') as SuzukiRefferenceNo
,coalesce(b.PaymentNo, '') as PaymentNo
,case convert(varchar(10), b.PaymentDate, 121) when '1900-01-01' then null else b.PaymentDate end PaymentDate
,coalesce(b.InvoiceNo,'') as SPKNo
,a.IsCampaign
,b.ServiceBookNo
,b.PdiFsc
,b.Odometer
,b.ServiceDate
,b.DeliveryDate
,b.RegisteredDate
,b.BasicModel
,b.TransmissionType
,b.ChassisCode
,b.ChassisNo
,b.EngineCode
,b.EngineNo
,b.LaborAmount
,b.MaterialAmount
,b.PdiFscAmount
,coalesce(b.LaborPaymentAmount, 0) as LaborPaymentAmount
,coalesce(b.MaterialPaymentAmount, 0) as MaterialPaymentAmount
,coalesce(b.PdiFscPaymentAmount, 0) as PdiFscPaymentAmount
,case a.SourceData
  when '0' then 'Internal Faktur Penjualan'
  when '1' then 'Manual Input'
  when '2' then 'Sub Dealer / Branches'
 end as SourceData
,b.Remarks
from svTrnPdiFsc a
left join svTrnPdiFscApplication b on b.CompanyCode = a.CompanyCode
 and b.BranchCode = a.BranchCode
 and b.ProductType = a.ProductType
 and b.GenerateNo = a.GenerateNo
where a.CompanyCode = @CompanyCode
  and a.ProductType = @ProductType
  and a.GenerateNo between @GenerateNoFrom and @GenerateNoTo
  and a.SourceData = @SourceData
order by a.GenerateNo, b.GenerateSeq
GO


if object_id('usprpt_OmFakturPajak') is not null
	drop procedure usprpt_OmFakturPajak
GO
CREATE procedure [dbo].[usprpt_OmFakturPajak]
--DECLARE
	@CompanyCode	VARCHAR(15),
	@BranchCode	VARCHAR(15),
	@FPJDateStart DATETIME,
	@FPJDateEnd DATETIME,
	@FPJNoStart	VARCHAR(30),
	@FPJNoEnd	VARCHAR(30),
	@SignName VARCHAR(100),
	@TitleSign VARCHAR(100),
	@Param bit = 1
AS
BEGIN

--'6114201','611420100','20150501','20150530','010.001-15.70827239','010.001-15.70827239','Wiwik W','Pimpinan FAD',True

--SELECT @CompanyCode = '6114201',@BranchCode='611420100',@FPJDateStart='20150522',@FPJDateEnd='20150522',@FPJNoStart='010.001-15.70827475',
--@FPJNoEnd='010.001-15.70827475',@SignName='Wiwik W',@TitleSign='Pimpinan FAD',@Param=1

	-- Setting Header Faktur Pajak --
	---------------------------------
	declare @fCompName	varchar(max)
	declare @fAdd		varchar(max)
	declare @fAdd1		varchar(max)
	declare @fAdd2		varchar(max)
	declare @fNPWP		varchar(max)
	declare @fSKP		varchar(max)
	declare @fSKPDate	varchar(max)
	declare @fCity		varchar(max)
	declare @fInv		int
	
	declare @fStatus varchar(1)
	set @fStatus = 0
	
	declare @fInfoPKP varchar(1)
	set @fInfoPKP = 1
	
	if exists (select 1 from gnMstLookUpDtl where codeid='FPJFLAG')
	begin
		set @fStatus = (select paravalue from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='STATUS')
	end
	
	if exists (select * from gnMstLookUpHdr where codeid='FPJ_INFO_PKP')
	begin
		set @fInfoPKP = (select LookupValue from gnmstlookupdtl where codeid='FPJ_INFO_PKP')
	end
	
	if (@fStatus = '1')
	begin
		set @fCompName	= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='NAME')
		set @fAdd1		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='ADD1')
		set @fAdd2		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='ADD2')
		set @fAdd		= @fAdd1+' '+@fAdd2
		set @fNPWP		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='NPWP')
		set @fSKP		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='SKPNO')
		set @fSKPDate	= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='SKPDATE')
		set @fCity		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='CITY')		
	end
set @fInv		= (select isnull(ParaValue,'1') from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='SALES')		
-- parameter use address holding or not
declare @IsHoldingAddr as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR') > 0
	set @IsHoldingAddr = (select convert(numeric,LookUpValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR')
else
	set @IsHoldingAddr = 0

-- parameter to show info or not
declare @IsShowInfo as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPIF' and LookUpValue='STATUS') > 0
	set @IsShowInfo = (select convert(numeric,ParaValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPIF' and LookUpValue='STATUS')
else
	set @IsShowInfo = 1
	
	
	declare @tabData as table
	(
		CompanyCode varchar(max),
		BranchCode varchar(max),
		InvoiceNo varchar(max),
		ItemOrder varchar(max),
		ItemCode varchar(max),
		ItemName varchar(max),
		PPnBM decimal(18,2),
		PPnBMSell decimal(18,2),
		ItemQuantity decimal(5,2),
		ItemDPP decimal(18,2),
		Potongan decimal(18,2),
		TaxPct decimal(18,2),
		AfterDiscPpn  decimal(18,2)
	)
	IF (@Param=1)
	BEGIN
		-- Sembunyikan Detail Part .....
		SELECT * INTO #t1 FROM (
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, acc.PartNo AS ItemCode
			, ISNULL((acc.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=acc.companycode 
						and branchcode=acc.branchcode and invoiceno=acc.invoiceno) * acc.RetailPrice,0) AS ItemDPP
			, ISNULL((acc.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=acc.companycode 
						and branchcode=acc.branchcode and invoiceno=acc.invoiceno) * acc.DiscExcludePPn, 0) AS Potongan
			, 0 AS TaxPct
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	)#t1
				
		select * into #Others from(
				Select a.CompanyCode
					 , a.BranchCode
					 , a.InvoiceNo
					 , a.SalesModelCode
					 , ISNULL((a.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=a.companycode 
						and branchcode=a.branchcode and invoiceno=a.invoiceno) * a.DPP, 0) ItemDPP
					 , 0 Potongan
				from omFakturPajakDetailOthers a
				left join omFakturPajakDetail b on a.CompanyCode = b.CompanyCode
					  and a.BranchCode = b.BranchCode
					  and a.InvoiceNo = b.InvoiceNo
					  and a.SalesModelCode = b.SalesModelCode
				INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = a.CompanyCode
					   AND hdr.BranchCode = a.BranchCode
					   AND hdr.InvoiceNo = a.InvoiceNo
					   AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
					   	   OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
					   AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
					   AND hdr.TaxType = 'Standard'
				where a.CompanyCode = @CompanyCode
				  and a.BranchCode = @BranchCode
		)#Others
		
		--SELECT * INTO #t2 FROM (
		--SELECT
		--	CompanyCode 
		--	, BranchCode
		--	, InvoiceNo 
		--	, SUM(ItemDPP) ItemDPP
		--	, SUM(Potongan) Potongan
		--FROM #t1 GROUP BY CompanyCode, BranchCode, InvoiceNo ) #t2
		
		SELECT CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
		INTO #t2 FROM (
			SELECT
				CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
			FROM #t1 GROUP BY CompanyCode, BranchCode, InvoiceNo
		UNION
			SELECT
				CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
			FROM #Others a
			GROUP BY a.CompanyCode, a.BranchCode, a.InvoiceNo 
		) #t2
		group by CompanyCode,BranchCode,InvoiceNo
		
		-- INSER DATA KE TABEL RESULT
		INSERT INTO @tabData 
		-- Unit
		SELECT 
			mdl.CompanyCode
			, mdl.BranchCode
			, mdl.InvoiceNo
			, '1' AS ItemOrder
			, mdl.SalesModelCode AS ItemCode
			, LEFT(CONVERT(VARCHAR, mdl.ChassisNo) + '             ', 13) +  LEFT(CONVERT(VARCHAR, mdl.EngineNo) + '           ', 11) AS ItemName
			, ISNULL(Vec.PPnBMBuyPaid,0) AS PPnBM
			, mdl.AfterDiscPPnBM AS PPnBMSell
			, 1 AS ItemQuantity
			, mdl.BeforeDiscDPP + (ISNULL(t.ItemDPP,0)) AS ItemDPP
			, mdl.DiscExcludePPN + (ISNULL(t.Potongan,0)) AS Potongan
			, ISNULL((SELECT ISNULL(TaxPct, 0) FROM  GnMstTax INNER JOIN OmMstModel ON OmMstModel.CompanyCode = GnMstTax.CompanyCode AND OmMstModel.PPnBMCodeSell = GnMstTax.TaxCode AND OmMstModel.SalesModelCode = mdl.SalesModelCode), 0) AS TaxPct
			, mdl.AfterDiscPpn		
		FROM 
			omFakturPajakDetail mdl 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = mdl.CompanyCode
			AND hdr.BranchCode = mdl.BranchCode
			AND hdr.InvoiceNo = mdl.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		LEFT JOIN OmMstVehicle vec ON mdl.CompanyCode = vec.CompanyCode 
			AND mdl.ChassisCode = vec.ChassisCode 
			AND mdl.ChassisNo = vec.ChassisNo
		LEFT JOIN #t2 t ON t.CompanyCode = mdl.CompanyCode
			AND t.BranchCode = mdl.BranchCode
			AND t.InvoiceNo = mdl.InvoiceNo
		WHERE 
			mdl.CompanyCode = @CompanyCode AND mdl.BranchCode = @BranchCode 
		UNION ALL
		SELECT distinct
			hdr.CompanyCode
			, hdr.BranchCode
			, hdr.InvoiceNo
			, '2' AS ItemOrder
			, 'SPAREPART/MATERIAL' AS ItemCode
			, '' AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM omFakturPajakDetailAccsSeq acc
		INNER JOIN omFakturPajakHdr hdr ON hdr.CompanyCode=acc.CompanyCode
			AND hdr.BranchCode=acc.BranchCode
			AND hdr.InvoiceNo=acc.InvoiceNo
		WHERE hdr.CompanyCode = @CompanyCode
			AND hdr.BranchCode = @BranchCode
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		UNION ALL
		-- SPAREPART/MATERIAL
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, '3' AS ItemOrder
			, acc.PartNo AS ItemCode
			, acc.PartName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	
		UNION ALL
		SELECT 
			oth.CompanyCode
			, oth.BranchCode
			, oth.InvoiceNo
			, '4' AS ItemOrder
			, oth.OtherCode AS ItemCode
			, oth.OtherName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailOthers oth 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = oth.CompanyCode
			AND hdr.BranchCode = oth.BranchCode
			AND hdr.InvoiceNo = oth.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			oth.CompanyCode = @CompanyCode AND oth.BranchCode = @BranchCode	
							 
		DROP TABLE #t1
		DROP TABLE #t2			
	END
	ELSE
	BEGIN
		-- Tampilkan Part --
		--------------------
		
		-- INSER DATA KE TABEL RESULT
		INSERT INTO @tabData 
		-- Unit
		SELECT 
			mdl.CompanyCode
			, mdl.BranchCode
			, mdl.InvoiceNo
			, '1' AS ItemOrder
			, mdl.SalesModelCode AS ItemCode
			, LEFT(CONVERT(VARCHAR, mdl.ChassisNo) + '             ', 13) +  LEFT(CONVERT(VARCHAR, mdl.EngineNo) + '           ', 11) AS ItemName
			, ISNULL(Vec.PPnBMBuyPaid,0) AS PPnBM
			, mdl.AfterDiscPPnBM AS PPnBMSell
			, 1 AS ItemQuantity
			, mdl.BeforeDiscDPP AS ItemDPP
			, mdl.DiscExcludePPN Potongan
			, ISNULL((SELECT ISNULL(TaxPct, 0) FROM  GnMstTax INNER JOIN OmMstModel ON OmMstModel.CompanyCode = GnMstTax.CompanyCode AND OmMstModel.PPnBMCodeSell = GnMstTax.TaxCode AND OmMstModel.SalesModelCode = mdl.SalesModelCode), 0) AS TaxPct
			, mdl.AfterDiscPpn
		FROM 
			omFakturPajakDetail mdl 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = mdl.CompanyCode
			AND hdr.BranchCode = mdl.BranchCode
			AND hdr.InvoiceNo = mdl.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		LEFT JOIN OmMstVehicle vec ON mdl.CompanyCode = vec.CompanyCode 
			AND mdl.ChassisCode = vec.ChassisCode 
			AND mdl.ChassisNo = vec.ChassisNo
		WHERE 
			mdl.CompanyCode = @CompanyCode AND mdl.BranchCode = @BranchCode 
		UNION ALL
		SELECT distinct
			hdr.CompanyCode
			, hdr.BranchCode
			, hdr.InvoiceNo
			, '2' AS ItemOrder
			, 'SPAREPART/MATERIAL' AS ItemCode
			, '' AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM omFakturPajakDetailAccsSeq acc
		INNER JOIN omFakturPajakHdr hdr ON hdr.CompanyCode=acc.CompanyCode
			AND hdr.BranchCode=acc.BranchCode
			AND hdr.InvoiceNo=acc.InvoiceNo
		WHERE hdr.CompanyCode = @CompanyCode
			AND hdr.BranchCode = @BranchCode
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		UNION ALL
		-- SPAREPART/MATERIAL
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, '3' AS ItemOrder
			, acc.PartNo AS ItemCode
			, acc.PartName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, acc.Quantity AS ItemQuantity
			, (acc.Quantity * acc.RetailPrice) AS ItemDPP
			, (acc.Quantity * acc.DiscExcludePPn) AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	
		UNION ALL
		SELECT 
			oth.CompanyCode
			, oth.BranchCode
			, oth.InvoiceNo
			, '4' AS ItemOrder
			, oth.OtherCode AS ItemCode
			, oth.OtherName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, oth.Quantity AS ItemQuantity
			, (oth.Quantity * oth.DPP) AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailOthers oth 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = oth.CompanyCode
			AND hdr.BranchCode = oth.BranchCode
			AND hdr.InvoiceNo = oth.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			oth.CompanyCode = @CompanyCode AND oth.BranchCode = @BranchCode	
	END

SELECT * INTO #hasil FROM (
SELECT
	a.TaxType AS TaxType
	,a.InvoiceNo AS InvoiceNo
	,a.InvoiceDate AS InvoiceDate
	,a.FakturPajakNo AS FPJNo
	,(SELECT dbo.GetDateIndonesian (CONVERT(VARCHAR,a.FakturPajakDate, 101))) AS FPJDate
	,a.CustomerCode AS fakturCustCode
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fCompName else e.CompanyGovName end)
	 else '' end AS CompanyName
	,case @fStatus when '1' then @fSKP else e.SKPNo end AS compSKPNo
	,case @fStatus when '1' then @fSKPDate else e.SKPDate end AS compSKPDate
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fAdd else 
			(case when @IsHoldingAddr=0 then ISNULL(e.Address1,'') + ' ' + ISNULL(e.Address2,'') + ' ' + ISNULL(e.Address3,'') + ' ' + ISNULL(e.Address4,'')
				else (select ISNULL(Address1,'') + ' ' + ISNULL(Address2,'') + ' ' + ISNULL(Address3,'') + ' ' + ISNULL(Address4,'') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))end ) end)
		 else '' end AS compAddr	 
	,e.PhoneNo AS compPhoneNo
	,e.FaxNo AS compFaxNo
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fNPWP else e.NPWPNo end)
	 else '' end AS compNPWPNo
	,d.CustomerGovName AS CustomerName
	,d.SKPNo AS custSKPNo
	,d.SKPDate AS custSKPDate
	,ISNULL(d.Address1,'') AS custAddr1
	,ISNULL(d.Address2,'') AS custAddr2
	,ISNULL(d.Address3,'')+ ' ' + ISNULL(d.Address4,'') AS custAddr3
	,d.PhoneNo AS custPhoneNo
	,d.FaxNo AS custFaxNo
	,d.NPWPNo AS custNPWPNo
	,a.DueDate AS fakturDueDate
	,a.DiscAmt AS DiscAmt
	,a.DppAmt AS DppAmt
	,a.PPNAmt AS PPNAmt
	,a.DppAmt - a.DiscAmt AS SubAmt
	,a.TotalAmt AS TotalAmt
	,a.TotQuantity
	,a.PPnBMPaid
	,case @fStatus when '1' then @fCity else 
		(SELECT LookUpValueName FROM gnMstLookupDtl WHERE CodeID = 'CITY' AND LookUpValue = e.CityCode) end as cityNm
	,ISNULL(@SignName, '') AS TaxPerson
	,ISNULL(@TitleSign,'') AS JobTitle
	,'Model              No.Rangka    No.Mesin            PPnBM' AS ItemHeader
	,dtl.ItemOrder
	,dtl.ItemCode
	,dtl.ItemName
	,dtl.PPnBM
	,dtl.ItemQuantity
	,dtl.ItemDPP
	,dtl.Potongan
	,dtl.AfterDiscPpn
	,CASE WHEN copro.ProductType = '2W' THEN 
		(CASE WHEN so.SalesType = '0' THEN 
			(SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' AND  LookupValue = '2WWS') 
				ELSE (SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' and  LookupValue = '2WDS') END) 
		ELSE 
		(CASE WHEN so.SalesType = '0' THEN 
			(SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' AND  LookupValue = '4WWS') 
				ELSE (SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' and  LookupValue = '4WDS') END)  
	END AS Keterangan
	,@IsShowInfo as FlagShowInfo
	,@param HidePart
	,dtl.TaxPct
	,dtl.PPnBMSell
FROM 
	omFakturPajakHdr a
LEFT JOIN GnMstCustomer d ON a.CompanyCode = d.CompanyCode 
	AND a.CustomerCode = d.CustomerCode
LEFT JOIN GnMstCoProfile e ON a.CompanyCode = e.CompanyCode 
	AND a.BranchCode = e.BranchCode
INNER JOIN @tabData dtl ON dtl.CompanyCode = a.CompanyCode
	AND dtl.BranchCode = a.BranchCode
	AND dtl.InvoiceNo = a.InvoiceNo
LEFT JOIN omTrSalesInvoice inv ON inv.CompanyCode = a.CompanyCode 
	AND inv.BranchCode = a.BranchCode 
	AND inv.InvoiceNo = a.InvoiceNo
LEFT JOIN omTrSalesSO so	ON a.CompanyCode = so.CompanyCode
	AND a.BranchCode = so.BranchCode 
	AND inv.SONo = so.SONo
LEFT JOIN GnMstCoProfile copro ON a.CompanyCode = copro.CompanyCode 
	AND a.BranchCode = copro.BranchCode
WHERE  
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode ) #hasil

if (@param=1)
	BEGIN
	SELECT TaxType, InvoiceNo, InvoiceDate, FPJNo, FPJDate, fakturCustCode, CompanyName, compSKPNo, compSKPDate, compAddr, compPhoneNo, compFaxNo, compNPWPNo
		, CustomerName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, DiscAmt, DPPAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(ItemDPP) - sum(Potongan) from #hasil where FPJNo = c.FPJNo ) ELSE SubAmt END SubAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, TotQuantity, PPNBMPaid, cityNm, TaxPerson, JobTitle, ItemHeader, ItemOrder, ItemCode, ItemName, PPnBM, ItemQuantity, ItemDPP, Potongan, Keterangan
		, FlagShowInfo, HidePart, TaxPct, PPnBMSell
		, (SELECT COUNT(INVOICENO) FROM #hasil WHERE FPJNo = c.FPJNo) MaxRow
		, (select sum(ItemQuantity) from #hasil where FPJNo=c.FPJNo and ItemOrder='1') SumQty
		, (select sum(ItemDPP) from #hasil where FPJNo = c.FPJNo ) XAmt
		, (select sum(Potongan) from #hasil where FPJNo = c.FPJNo ) XPotongan
		, @fInv ViewInvoice
	FROM #hasil c order by c.InvoiceNo, c.ItemOrder
	END
else
	SELECT TaxType, InvoiceNo, InvoiceDate, FPJNo, FPJDate, fakturCustCode, CompanyName, compSKPNo, compSKPDate, compAddr, compPhoneNo, compFaxNo, compNPWPNo
		, CustomerName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, DiscAmt, DPPAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(ItemDPP) - sum(Potongan) from #hasil where FPJNo = c.FPJNo ) ELSE SubAmt END SubAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, TotQuantity, PPNBMPaid, cityNm, TaxPerson, JobTitle, ItemHeader, ItemOrder, ItemCode, ItemName, PPnBM, ItemQuantity, ItemDPP, Potongan, Keterangan
		, FlagShowInfo, HidePart, TaxPct, PPnBMSell
		, (SELECT COUNT(INVOICENO) FROM #hasil WHERE FPJNo = c.FPJNo) MaxRow
		, (select sum(ItemQuantity) from #hasil where FPJNo=c.FPJNo) SumQty
		, (select sum(ItemDPP) from #hasil where FPJNo = c.FPJNo ) XAmt
		, (select sum(Potongan) from #hasil where FPJNo = c.FPJNo ) XPotongan
		, @fInv ViewInvoice
	FROM #hasil c order by c.InvoiceNo, c.ItemOrder

DROP TABLE #hasil, #Others--, #t1
END
GO

if object_id('uspfn_SvTrnListKsgFromSPKPerJobNo') is not null
	drop procedure uspfn_SvTrnListKsgFromSPKPerJobNo
GO

CREATE procedure [dbo].[uspfn_SvTrnListKsgFromSPKPerJobNo]
--DECLARE
 @CompanyCode varchar(15),  
 @ProductType varchar(15),   
 @BranchFrom varchar(15),  
 @BranchTo varchar(15),  
 @PeriodFrom datetime,  
 @PeriodTo datetime,  
 @JobPDI as varchar(15),  
 @JobFSC as varchar(15),
 @BranchCode varchar(15),
 @BranchCodePar varchar(max),
 @JobOrderPar varchar(max)
as        

--select @CompanyCode = '6115204001',@ProductType='2W',@BranchFrom='6115204101', @BranchTo='6115204125',@PeriodFrom='2015-05-01',
--@PeriodTo='2015-05-27',@JobPDI='True',@JobFSC='False',@BranchCode='6115204101', 
--@BranchCodePar='''6115204102'',''6115204105'',''6115204106'',''6115204107'',''6115204109'',''6115204110'',''6115204111'',''6115204112'',''6115204116'',''6115204117'',''6115204124''',
--@JobOrderPar='''SPK/15/005185'',''SPK/15/005186'',''SPK/15/005187'',''SPK/15/005188'',''SPK/15/005189'',''SPK/15/005194'',''SPK/15/005195'',''SPK/15/005203'',''SPK/15/005204'',''SPK/15/005206'',''SPK/15/005208'',''SPK/15/005209'',''SPK/15/005210'',''SPK/15/005211'',''SPK/15/005214'',''SPK/15/005220'',''SPK/15/005222'',''SPK/15/005223'',''SPK/15/005225'',''SPK/15/005229'',''SPK/15/005231'',''SPK/15/005246'',''SPK/15/005249'',''SPK/15/005386'',''SPK/15/005387'',''SPK/15/005388'',''SPK/15/005389'',''SPK/15/005390'',''SPK/15/005391'',''SPK/15/005392'',''SPK/15/005393'',''SPK/15/005394'',''SPK/15/005395'',''SPK/15/005396'',''SPK/15/005397'',''SPK/15/005398'',''SPK/15/005399'',''SPK/15/005400'',''SPK/15/005449'',''SPK/15/005450'',''SPK/15/005451'',''SPK/15/005452'',''SPK/15/005453'',''SPK/15/005454'',''SPK/15/005455'',''SPK/15/005456'',''SPK/15/005457'',''SPK/15/005458'',''SPK/15/005459'',''SPK/15/005460'',''SPK/15/005461'',''SPK/15/005462'',''SPK/15/005463'',''SPK/15/005464'',''SPK/15/005465'',''SPK/15/005466'',''SPK/15/005467'',''SPK/15/005468'',''SPK/15/005469'',''SPK/15/005470'',''SPK/15/005471'',''SPK/15/005472'',''SPK/15/005473'',''SPK/15/005474'',''SPK/15/005475'',''SPK/15/005476'',''SPK/15/005477'',''SPK/15/005478'',''SPK/15/005479'',''SPK/15/005480'',''SPK/15/005481'',''SPK/15/005482'',''SPK/15/005484'',''SPK/15/005485'',''SPK/15/005486'',''SPK/15/005488'',''SPK/15/005489'',''SPK/15/005490'',''SPK/15/005491'',''SPK/15/005492'',''SPK/15/005493'',''SPK/15/005494'',''SPK/15/002680'',''SPK/15/002708'',''SPK/15/002709'',''SPK/15/002710'',''SPK/15/002711'',''SPK/15/002714'',''SPK/15/002716'',''SPK/15/002717'',''SPK/15/002718'',''SPK/15/002719'',''SPK/15/002722'',''SPK/15/002723'',''SPK/15/002725'',''SPK/15/002726'',''SPK/15/002727'',''SPK/15/002728'',''SPK/15/002729'',''SPK/15/002730'',''SPK/15/002732'',''SPK/15/002734'',''SPK/15/002735'',''SPK/15/002736'',''SPK/15/002737'',''SPK/15/002738'',''SPK/15/002739'',''SPK/15/002740'',''SPK/15/001692'',''SPK/15/001696'',''SPK/15/001697'',''SPK/15/001756'',''SPK/15/001783'',''SPK/15/001822'',''SPK/15/001879'',''SPK/15/001882'',''SPK/15/001883'',''SPK/15/001374'',''SPK/15/001375'',''SPK/15/001380'',''SPK/15/001381'',''SPK/15/001382'',''SPK/15/001385'',''SPK/15/001388'',''SPK/15/001389'',''SPK/15/001390'',''SPK/15/001391'',''SPK/15/001392'',''SPK/15/001393'',''SPK/15/001394'',''SPK/15/001395'',''SPK/15/001396'',''SPK/15/001398'',''SPK/15/001399'',''SPK/15/001400'',''SPK/15/001401'',''SPK/15/001402'',''SPK/15/001403'',''SPK/15/001404'',''SPK/15/001446'',''SPK/15/001447'',''SPK/15/001161'',''SPK/15/001163'',''SPK/15/001164'',''SPK/15/001166'',''SPK/15/001168'',''SPK/15/001171'',''SPK/15/001172'',''SPK/15/001173'',''SPK/15/001174'',''SPK/15/001177'',''SPK/15/001178'',''SPK/15/001179'',''SPK/15/001180'',''SPK/15/001181'',''SPK/15/001182'',''SPK/15/001209'',''SPK/15/001210'',''SPK/15/001212'',''SPK/15/001213'',''SPK/15/001214'',''SPK/15/001216'',''SPK/15/001218'',''SPK/15/001219'',''SPK/15/001220'',''SPK/15/001262'',''SPK/15/001263'',''SPK/15/001270'',''SPK/15/001271'',''SPK/15/001272'',''SPK/15/001273'',''SPK/15/001274'',''SPK/15/001276'',''SPK/15/001277'',''SPK/15/001278'',''SPK/15/001279'',''SPK/15/001280'',''SPK/15/001281'',''SPK/15/001282'',''SPK/15/001283'',''SPK/15/001310'',''SPK/15/001311'',''SPK/15/001312'',''SPK/15/001313'',''SPK/15/001314'',''SPK/15/001315'',''SPK/15/001316'',''SPK/15/001317'',''SPK/15/001318'',''SPK/15/001319'',''SPK/15/001320'',''SPK/15/001327'',''SPK/15/001328'',''SPK/15/001329'',''SPK/15/001330'',''SPK/15/001957'',''SPK/15/001958'',''SPK/15/001970'',''SPK/15/001971'',''SPK/15/001972'',''SPK/15/001973'',''SPK/15/002021'',''SPK/15/002023'',''SPK/15/002032'',''SPK/15/002033'',''SPK/15/002034'',''SPK/15/002035'',''SPK/15/002036'',''SPK/15/002039'',''SPK/15/002040'',''SPK/15/002041'',''SPK/15/002042'',''SPK/15/002044'',''SPK/15/002045'',''SPK/15/002046'',''SPK/15/002047'',''SPK/15/002048'',''SPK/15/002049'',''SPK/15/002050'',''SPK/15/002051'',''SPK/15/002052'',''SPK/15/002053'',''SPK/15/002054'',''SPK/15/002055'',''SPK/15/002056'',''SPK/15/002057'',''SPK/15/002059'',''SPK/15/002060'',''SPK/15/002061'',''SPK/15/002065'',''SPK/15/002066'',''SPK/15/002067'',''SPK/15/002068'',''SPK/15/002069'',''SPK/15/002070'',''SPK/15/002071'',''SPK/15/002072'',''SPK/15/002073'',''SPK/15/002110'',''SPK/15/002111'',''SPK/15/001566'',''SPK/15/001567'',''SPK/15/001681'',''SPK/15/001682'',''SPK/15/001683'',''SPK/15/001684'',''SPK/15/001691'',''SPK/15/001692'',''SPK/15/001693'',''SPK/15/001694'',''SPK/15/001695'',''SPK/15/001696'',''SPK/15/001697'',''SPK/15/001698'',''SPK/15/001699'',''SPK/15/001700'',''SPK/15/001701'',''SPK/15/001702'',''SPK/15/001703'',''SPK/15/001704'',''SPK/15/001705'',''SPK/15/001706'',''SPK/15/001707'',''SPK/15/001708'',''SPK/15/001770'',''SPK/15/001771'',''SPK/15/001772'',''SPK/15/001773'',''SPK/15/001774'',''SPK/15/001775'',''SPK/15/001099'',''SPK/15/001100'',''SPK/15/001101'',''SPK/15/001102'',''SPK/15/001103'',''SPK/15/001104'',''SPK/15/001105'',''SPK/15/001106'',''SPK/15/001107'',''SPK/15/001108'',''SPK/15/001109'',''SPK/15/001111'',''SPK/15/001112'',''SPK/15/001113'',''SPK/15/001114'',''SPK/15/001115'',''SPK/15/001116'',''SPK/15/001117'',''SPK/15/001118'',''SPK/15/001119'',''SPK/15/001120'',''SPK/15/001121'',''SPK/15/001122'',''SPK/15/001123'',''SPK/15/001124'',''SPK/15/001125'',''SPK/15/001126'',''SPK/15/001127'',''SPK/15/001128'',''SPK/15/001129'',''SPK/15/001130'',''SPK/15/001131'',''SPK/15/001132'',''SPK/15/001133'',''SPK/15/001134'',''SPK/15/001135'',''SPK/15/001136'',''SPK/15/001137'',''SPK/15/001138'',''SPK/15/001139'',''SPK/15/001140'',''SPK/15/001141'',''SPK/15/001174'',''SPK/15/001175'',''SPK/15/001218'',''SPK/15/001219'',''SPK/15/002237'',''SPK/15/002238'',''SPK/15/002239'',''SPK/15/002240'',''SPK/15/002242'',''SPK/15/002243'',''SPK/15/002244'',''SPK/15/002245'',''SPK/15/002246'',''SPK/15/002247'',''SPK/15/002248'',''SPK/15/002249'',''SPK/15/002250'',''SPK/15/002252'',''SPK/15/002253'',''SPK/15/002254'',''SPK/15/002255'',''SPK/15/002256'',''SPK/15/002257'',''SPK/15/002258'',''SPK/15/002259'',''SPK/15/001075'',''SPK/15/001076'',''SPK/15/001077'',''SPK/15/001078'',''SPK/15/001079'',''SPK/15/001080'',''SPK/15/001081'',''SPK/15/001082'',''SPK/15/001083'',''SPK/15/001084'',''SPK/15/001087'',''SPK/15/001096'',''SPK/15/001097'',''SPK/15/001098'',''SPK/15/001099'',''SPK/15/001100'',''SPK/15/001101'',''SPK/15/001167'',''SPK/15/000001'''

declare @IsCentralize as varchar(1)
set @IsCentralize = '0'
if(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='SRV_FLAG' and LookUpValue='KSG_HOLDING') > '0'
	set @IsCentralize = '1'
	 
select * into #t1 from(  
select  
    convert(bit, 1) Process      
 , srv.BranchCode  
 , srv.JobOrderNo  
 , case when convert(varchar, srv.JobOrderDate, 106) = '19000101' then '' else convert(varchar, srv.JobOrderDate, 106) end JobOrderDate  
 , srv.BasicModel  
 , srv.ServiceBookNo  
 , job.PdiFscSeq  
 , srv.Odometer  
 , srv.LaborGrossAmt  
 , round((select isnull(SUM((SupplyQty - ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = 'F'),0) MaterialGrossAmt --Pembulatan
 , round((srv.LaborGrossAmt + (select isnull(SUM((SupplyQty - ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = 'F')),0) PdiFscAmount  --Pembulatan
 , isnull(case when convert(varchar, veh.FakturPolisiDate, 112) = '19000101' then '' else convert(varchar, veh.FakturPolisiDate, 106) end, '')  FakturPolisiDate  
 , isnull(case when convert(varchar, mstVeh.BPKDate, 112) = '19000101' then '' else convert(varchar, mstVeh.BPKDate, 106) end, '')  BPKDate  
 , srv.ChassisCode  
 , srv.ChassisNo  
 , srv.EngineCode  
 , srv.EngineNo   
    , srv.InvoiceNo  
 , isnull(inv.FPJNo, '') FPJNo  
 , isnull(case when convert(varchar, inv.FPJDate, 112) = '19000101' then '' else convert(varchar, inv.FPJDate, 106) end, '')  FPJDate  
 , isnull(fpj.FPJGovNo, '') FPJGovNo  
 , srv.TransmissionType  
 , srv.ServiceStatus  
 , srv.CompanyCode  
 , srv.ProductType  
from svTrnService srv  
left join svMstJob job  
 on job.CompanyCode = srv.CompanyCode  
  and job.ProductType = srv.ProductType  
  and job.BasicModel = srv.BasicModel  
  and job.JobType = srv.JobType  
left join svMstCustomerVehicle veh  
 on veh.CompanyCode = srv.CompanyCode  
  and veh.ChassisCode = srv.ChassisCode  
  and veh.ChassisNo = srv.ChassisNo  
left join omMstVehicle mstVeh  
 on mstVeh.CompanyCode = srv.CompanyCode  
  and mstVeh.ChassisCode = srv.ChassisCode  
  and mstVeh.ChassisNo = srv.ChassisNo  
left join svTrnInvoice inv  
 on inv.CompanyCode = srv.CompanyCode  
  and inv.BranchCode = srv.BranchCode  
  and inv.ProductType = srv.ProductType  
  and inv.InvoiceNo = srv.InvoiceNo  
left join svTrnFakturPajak fpj  
 on fpj.CompanyCode = srv.CompanyCode  
  and fpj.BranchCode = srv.BranchCode  
  and fpj.FPJNo = inv.FPJNo  
where   
 srv.CompanyCode = @CompanyCode  
 and srv.BranchCode between @BranchFrom and @BranchTo  
 and srv.ProductType = @ProductType  
 --and srv.isLocked = 0  
 and job.GroupJobType = 'FSC'  
 and ((job.GroupJobType like @JobFSC and job.PdiFscSeq > 0 )  or (job.JobType like @JobPDI and job.PdiFscSeq=0))
 and convert(varchar, srv.JobOrderDate, 112) between convert(varchar, @PeriodFrom, 112) and convert(varchar, @PeriodTo, 112)   
 --and  not exists (  
 -- select 1   
 -- from svTrnPdiFscApplication   
 -- where CompanyCode=srv.CompanyCode  
 --  and (case when @IsCentralize = '0' then BranchCode end) = srv.BranchCode   
 --  and InvoiceNo=srv.JobOrderNo  
 --  and ProductType=srv.ProductType					
 and  not exists (  
  select 1   
  from svTrnPdiFscApplication   
  where CompanyCode=srv.CompanyCode  
   and BranchCode = (case when @IsCentralize = '0' then srv.BranchCode  else @BranchCode end)
   and InvoiceNo=srv.JobOrderNo  
   and ProductType=srv.ProductType  
 )--)
) #t1  
declare @sql as varchar(max)
set @sql =
'select   
row_number() over (order by #t1.BranchCode, #t1.JobOrderNo) No,  
* from #t1   
where ServiceStatus=5 
and BranchCode in (' + @BranchCodePar + ')
and JobOrderNo in (' + @JobOrderPar + ')
order by BranchCode, JobOrderNo'  
exec(@sql)
drop table #t1
GO

if object_id('sp_SpItemPriceView') is not null
	drop procedure sp_SpItemPriceView
GO

CREATE procedure [dbo].[sp_SpItemPriceView] (  @CompanyCode varchar(10) ,@BranchCode varchar(10))
 as

declare @query varchar(max)

declare @DbMD varchar(20)
declare @CompanyMD varchar (20)
declare @BranchMD varchar (20)

set @DbMD = dbo.GetDbMD( @CompanyCode ,@BranchCode ) 
set @CompanyMD = dbo.GetCompanyMD( @CompanyCode , @BranchCode )
set @BranchMD = dbo.GetBranchMD( @CompanyCode , @BranchCode ) 

set @query = ' 
SELECT 
 Items.CompanyCode 
 ,Items.BranchCode
 ,ItemInfo.PartNo
,ItemInfo.PartName
,ItemPrice.PurchasePrice
,ItemInfo.SupplierCode
,ItemPrice.RetailPriceInclTax
,CASE ItemInfo.IsGenuinePart WHEN 1 THEN ''Ya'' ELSE ''Tidak'' END AS IsGenuinePart
,Items.ProductType
,Items.PartCategory
,u.LookupValueName 
 as CategoryName
 ,ItemPrice.CostPrice
 ,ItemPrice.RetailPrice
 ,ItemPrice.LastPurchaseUpdate
 ,ItemPrice.LastRetailPriceUpdate
,ItemPrice.OldCostPrice
,ItemPrice.OldPurchasePrice
,ItemPrice.OldRetailPrice
FROM spMstItemPrice ItemPrice 
INNER JOIN '+@DbMD +'..spMstItems Items 
    ON ItemPrice.PartNo=Items.PartNo
right JOIN spMstItemInfo ItemInfo 
    ON ItemPrice.CompanyCode = ItemInfo.CompanyCode 
    AND ItemPrice.PartNo = ItemInfo.PartNo
	inner join  gnMstLookUpDtl u on (Items.PartCategory =u.ParaValue)
WHERE  u.CodeID=''PRCT''
and Items.CompanyCode=  '+ @CompanyMD+'  and Items.BranchCode= '+ @BranchMD+'
and ItemPrice.CompanyCode=  '+ @CompanyCode+'  and ItemPrice.BranchCode= '+ @BranchCode

exec(@query)
GO








