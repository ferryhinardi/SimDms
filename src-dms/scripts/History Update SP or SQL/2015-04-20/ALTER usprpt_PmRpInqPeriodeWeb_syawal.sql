
ALTER procedure [dbo].[usprpt_PmRpInqPeriodeWeb] 
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

