
ALTER procedure [dbo].[usprpt_PmRpInqPeriodeWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@PeriodBegin		DATETIME,
	@PeriodEnd			DATETIME,
	@BranchManager		VARCHAR(50),
	@SalesHead			VARCHAR(15),
	@Salesman			VARCHAR(15),
	@isexel				varchar(15) = ''
)
AS 
BEGIN
SET NOCOUNT ON;
----
declare @inqno VARCHAR(50)
--set @BranchManager = (SELECT a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
--		AND a.CompanyCode = @CompanyCode AND a.EmployeeID = @SalesHead )
IF(@isexel = '')
begin
	IF(@BranchManager = '' AND @SalesHead ='' AND @Salesman ='')
BEGIN
	SELECT * INTO #empl0 FROM (
		--SH =ALL AND S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  a.Department ='SALES'
		AND a.Position = 'S'
		AND a.PersonnelStatus = '1' 
	)#empl0

	SELECT * INTO #t0 FROM (
		SELECT
			case when @BranchCode = '' then 'SEMUA' else f.BranchName end as OutletName, case when @isexel = '' then a.InquiryNumber else cast( a.InquiryNumber as varchar(50)) end as InquiryNumber
			, a.NamaProspek Pelanggan, a.AlamatProspek, a.TelpRumah, a.NamaPerusahaan, 
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
			AND CONVERT(varchar(20),a.InquiryDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd 
	) #t0

	DROP TABLE #empl0
	SELECT OutletName
		,InquiryNumber
		,Pelanggan
		,AlamatProspek
		,TelpRumah
		,NamaPerusahaan
		,AlamatPerusahaan
		,Handphone
		,CASE(InquiryDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), InquiryDate, 106) END InquiryDate
		,TipeKendaraan
		,Variant
		,Transmisi
		,Warna
		,PerolehanData
		,Employee
		,Supervisor
		,CASE(NextFollowUpDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), NextFollowUpDate, 106) END NextFollowUpDate
		,LastProgress
		,LastUpdateStatus
		,CASE(SPKDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), SPKDate, 106) END SPKDate
		,CASE(LostCaseDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), LostCaseDate, 106) END LostCaseDate, ActivityDetail FROM #t0 ORDER BY InquiryNumber
	DROP TABLE #t0
END
ELSE IF(@SalesHead ='' AND @Salesman ='')
BEGIN
	SELECT * INTO #empl1 FROM (
		--SH =ALL AND S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  a.Department ='SALES'
		AND a.Position = 'S'
		AND a.PersonnelStatus = '1' AND
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
		,CASE(InquiryDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), InquiryDate, 106) END InquiryDate
		,TipeKendaraan
		,Variant
		,Transmisi
		,Warna
		,PerolehanData
		,Employee
		,Supervisor
		,CASE(NextFollowUpDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), NextFollowUpDate, 106) END NextFollowUpDate
		,LastProgress
		,LastUpdateStatus
		,CASE(SPKDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), SPKDate, 106) END SPKDate
		,CASE(LostCaseDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), LostCaseDate, 106) END LostCaseDate, ActivityDetail FROM #t1 ORDER BY InquiryNumber
	DROP TABLE #t1

END
ELSE IF(@Salesman = '')
BEGIN
	SELECT * INTO #empl2 FROM (
		--S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  a.Department ='SALES'
		AND a.Position = 'S'
		AND a.PersonnelStatus = '1' AND
		a.TeamLeader = @SalesHead
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
		,InquiryNumber
		,Pelanggan
		,AlamatProspek
		,TelpRumah
		,NamaPerusahaan
		,AlamatPerusahaan
		,Handphone
		,CASE(InquiryDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), InquiryDate, 106) END InquiryDate
		,TipeKendaraan
		,Variant
		,Transmisi
		,Warna
		,PerolehanData
		,Employee
		,Supervisor
		,CASE(NextFollowUpDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), NextFollowUpDate, 106) END NextFollowUpDate
		,LastProgress
		,LastUpdateStatus
		,CASE(SPKDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), SPKDate, 106) END SPKDate
		,CASE(LostCaseDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), LostCaseDate, 106) END LostCaseDate, ActivityDetail FROM #t2 ORDER BY InquiryNumber
	DROP TABLE #t2
END
ELSE
BEGIN
	SELECT * INTO #empl3 FROM (
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  a.Department ='SALES'
		AND a.Position = 'S'
		AND a.PersonnelStatus = '1' AND
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
		,CASE(InquiryDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), InquiryDate, 106) END InquiryDate
		,TipeKendaraan
		,Variant
		,Transmisi
		,Warna
		,PerolehanData
		,Employee
		,Supervisor
		,CASE(NextFollowUpDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), NextFollowUpDate, 106) END NextFollowUpDate
		,LastProgress
		,LastUpdateStatus
		,CASE(SPKDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), SPKDate, 106) END SPKDate
		,CASE(LostCaseDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), LostCaseDate, 106) END LostCaseDate, ActivityDetail FROM #t3 ORDER BY InquiryNumber
	DROP TABLE #t3
END
end
else begin
	IF(@BranchManager = '' AND @SalesHead ='' AND @Salesman ='')
BEGIN
	SELECT * INTO #empl00 FROM (
		--SH =ALL AND S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  a.Department ='SALES'
		AND a.Position = 'S'
		AND a.PersonnelStatus = '1' 
	)#empl00

	SELECT * INTO #t00 FROM (
		SELECT
			f.BranchName OutletName, case when @isexel = '' then a.InquiryNumber else cast( a.InquiryNumber as varchar(50)) end as InquiryNumber
			, a.NamaProspek Pelanggan, a.AlamatProspek, a.TelpRumah, a.NamaPerusahaan, 
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
			AND CONVERT(varchar(20),a.InquiryDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd 
	) #t00

	DROP TABLE #empl00
	SELECT OutletName
		,convert(varchar(50),InquiryNumber) as InquiryNumber
		,Pelanggan
		,AlamatProspek
		,TelpRumah
		,NamaPerusahaan
		,AlamatPerusahaan
		,Handphone
		,CASE(InquiryDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), InquiryDate, 106) END InquiryDate
		,TipeKendaraan
		,Variant
		,Transmisi
		,Warna
		,PerolehanData
		,Employee
		,Supervisor
		,CASE(NextFollowUpDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), NextFollowUpDate, 106) END NextFollowUpDate
		,LastProgress
		,LastUpdateStatus
		,CASE(SPKDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), SPKDate, 106) END SPKDate
		,CASE(LostCaseDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), LostCaseDate, 106) END LostCaseDate, ActivityDetail FROM #t00 ORDER BY InquiryNumber
	DROP TABLE #t00
END
ELSE IF(@SalesHead ='' AND @Salesman ='')
BEGIN
	SELECT * INTO #empl10 FROM (
		--SH =ALL AND S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  a.Department ='SALES'
		AND a.Position = 'S'
		AND a.PersonnelStatus = '1' AND
		a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader=@BranchManager)
	)#empl10

	SELECT * INTO #t10 FROM (
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
			AND CONVERT(varchar(20),a.InquiryDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl10 g)
	) #t10

	DROP TABLE #empl10
	SELECT OutletName
		,convert(varchar(50), InquiryNumber) as InquiryNumber
		,Pelanggan
		,AlamatProspek
		,TelpRumah
		,NamaPerusahaan
		,AlamatPerusahaan
		,Handphone
		,CASE(InquiryDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), InquiryDate, 106) END InquiryDate
		,TipeKendaraan
		,Variant
		,Transmisi
		,Warna
		,PerolehanData
		,Employee
		,Supervisor
		,CASE(NextFollowUpDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), NextFollowUpDate, 106) END NextFollowUpDate
		,LastProgress
		,LastUpdateStatus
		,CASE(SPKDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), SPKDate, 106) END SPKDate
		,CASE(LostCaseDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), LostCaseDate, 106) END LostCaseDate, ActivityDetail FROM #t10 ORDER BY InquiryNumber
	DROP TABLE #t10

END
ELSE IF(@Salesman = '')
BEGIN
	SELECT * INTO #empl20 FROM (
		--S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  a.Department ='SALES'
		AND a.Position = 'S'
		AND a.PersonnelStatus = '1' AND
		a.TeamLeader = @SalesHead
	)#empl20

	SELECT * INTO #t20 FROM (
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
			AND CONVERT(varchar(20),a.InquiryDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl20 g)
	) #t20
	
	DROP TABLE #empl20
	SELECT OutletName
		,convert(varchar(50), InquiryNumber) as InquiryNumber
		,Pelanggan
		,AlamatProspek
		,TelpRumah
		,NamaPerusahaan
		,AlamatPerusahaan
		,Handphone
		,CASE(InquiryDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), InquiryDate, 106) END InquiryDate
		,TipeKendaraan
		,Variant
		,Transmisi
		,Warna
		,PerolehanData
		,Employee
		,Supervisor
		,CASE(NextFollowUpDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), NextFollowUpDate, 106) END NextFollowUpDate
		,LastProgress
		,LastUpdateStatus
		,CASE(SPKDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), SPKDate, 106) END SPKDate
		,CASE(LostCaseDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), LostCaseDate, 106) END LostCaseDate, ActivityDetail FROM #t20 ORDER BY InquiryNumber
	DROP TABLE #t20
END
ELSE
BEGIN
	SELECT * INTO #empl30 FROM (
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode AND  a.Department ='SALES'
		AND a.Position = 'S'
		AND a.PersonnelStatus = '1' AND
		a.EmployeeID=@Salesman
	)#empl30

	SELECT * INTO #t30 FROM (
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
			AND CONVERT(varchar(20),a.InquiryDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl30 g)
	) #t30

	DROP TABLE #empl30
	SELECT OutletName
		,convert(varchar(50), InquiryNumber) as InquiryNumber
		,Pelanggan
		,AlamatProspek
		,TelpRumah
		,NamaPerusahaan
		,AlamatPerusahaan
		,Handphone
		,CASE(InquiryDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), InquiryDate, 106) END InquiryDate
		,TipeKendaraan
		,Variant
		,Transmisi
		,Warna
		,PerolehanData
		,Employee
		,Supervisor
		,CASE(NextFollowUpDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), NextFollowUpDate, 106) END NextFollowUpDate
		,LastProgress
		,LastUpdateStatus
		,CASE(SPKDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), SPKDate, 106) END SPKDate
		,CASE(LostCaseDate) WHEN '19000101' THEN '' ELSE CONVERT(varchar(20), LostCaseDate, 106) END LostCaseDate, ActivityDetail FROM #t30 ORDER BY InquiryNumber
	DROP TABLE #t30
END
end

----
END

