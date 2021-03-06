

ALTER procedure [dbo].[usprpt_PmRpInqFollowUpDtlNew] 
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
			AND convert(varchar(20),e.NextFollowUpDate,112) BETWEEN @DateAwal AND @DateAkhir
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
				AND convert(varchar(20),e.NextFollowUpDate,112) BETWEEN @DateAwal AND @DateAkhir
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
