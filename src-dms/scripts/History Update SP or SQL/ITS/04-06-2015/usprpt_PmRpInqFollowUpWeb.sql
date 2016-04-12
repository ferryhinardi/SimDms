if object_id('usprpt_PmRpInqFollowUpWeb') is not null
	drop procedure usprpt_PmRpInqFollowUpWeb

GO
/****** Object:  StoredProcedure [dbo].[usprpt_PmRpInqFollowUpWeb]    Script Date: 6/4/2015 5:31:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[usprpt_PmRpInqFollowUpWeb] 
(
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@PeriodBegin		DATETIME,
	@PeriodEnd			DATETIME,
	@BranchManager		VARCHAR(15),
	@SalesHead			VARCHAR(15),
	@Salesman			VARCHAR(15),
	@Outlet				VARCHAR(15),
	@Param				VARCHAR(2)
)
AS 
BEGIN
SET NOCOUNT ON;
----

IF(@BranchManager='' AND @SalesHead ='' AND @Salesman ='')
BEGIN
	SELECT * INTO #empl FROM (
		--SH =ALL AND S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a 
		WHERE a.Department ='SALES'
		--AND a.Position = 'S'
		AND a.PersonnelStatus = '1'
		AND a.CompanyCode = @CompanyCode
	)#empl

	SELECT * INTO #t FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.AlamatProspek, a.TelpRumah, a.NamaPerusahaan, CONVERT(VARCHAR(20),a.InquiryDate,106) InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, CONVERT(VARCHAR(20),e.NextFollowUpDate,106) NextFollowUpDate, a.LastProgress, e.ActivityDetail
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
			AND LastProgress in ('P','HP','SPK')
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			--AND CONVERT(varchar(20),e.NextFollowUpDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd
			AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
			--AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
			--AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
	) #t

	DROP TABLE #empl
	IF (@Param = '0') 
	BEGIN
		SELECT InquiryNumber, Pelanggan, AlamatProspek, TelpRumah, NamaPerusahaan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
		Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail 
		FROM #t 
		WHERE CONVERT(varchar(20),NextFollowUpDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd
		ORDER BY InquiryNumber
	END
	ELSE 
	BEGIN
		SELECT InquiryNumber, Pelanggan, AlamatProspek, TelpRumah, NamaPerusahaan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
		Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail 
		FROM #t 
		ORDER BY InquiryNumber
	END
	DROP TABLE #t

END
ELSE IF(@BranchManager<>'' AND @SalesHead ='' AND @Salesman ='')
BEGIN
	SELECT * INTO #empl1 FROM (
		--SH =ALL AND S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode
		--AND a.Position = 'S'
		AND a.PersonnelStatus = '1'
		AND a.TeamLeader IN (SELECT c.EmployeeID FROM HrEmployee c WHERE c.TeamLeader=@BranchManager)
	)#empl1

	SELECT * INTO #t1 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.AlamatProspek, a.TelpRumah, a.NamaPerusahaan, CONVERT(VARCHAR(20),a.InquiryDate,106) InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, CONVERT(VARCHAR(20),e.NextFollowUpDate,106) NextFollowUpDate, a.LastProgress, e.ActivityDetail
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
			AND LastProgress in ('P','HP','SPK')
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			--AND CONVERT(varchar(20),e.NextFollowUpDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd 
			--AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl1 g)
			AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
			--AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
			--AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
	) #t1

	DROP TABLE #empl1
	IF (@Param = '0') 
	BEGIN
		SELECT InquiryNumber, Pelanggan, AlamatProspek, TelpRumah, NamaPerusahaan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
		Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail 
		FROM #t1 
		WHERE CONVERT(varchar(20),NextFollowUpDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd 
		ORDER BY InquiryNumber
	END
	ELSE 
	BEGIN
		SELECT InquiryNumber, Pelanggan, AlamatProspek, TelpRumah, NamaPerusahaan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
		Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail 
		FROM #t1
		ORDER BY InquiryNumber
	END
	DROP TABLE #t1

END
ELSE IF(@BranchManager<>'' AND @SalesHead <>'' AND @Salesman ='')
BEGIN
	SELECT * INTO #empl2 FROM (
		--S=ALL
		SELECT a.EmployeeID, a.Position, a.TeamLeader FROM HrEmployee a WHERE a.Department ='SALES'
		AND a.CompanyCode = @CompanyCode 
		--AND a.Position = 'S'
		AND a.PersonnelStatus = '1'
		AND a.TeamLeader = @SalesHead
	)#empl2

	SELECT * INTO #t2 FROM (
		SELECT
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.AlamatProspek, a.TelpRumah, a.NamaPerusahaan, CONVERT(VARCHAR(20),a.InquiryDate,106) InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, CONVERT(VARCHAR(20),e.NextFollowUpDate,106) NextFollowUpDate, a.LastProgress, e.ActivityDetail
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
			AND LastProgress in ('P','HP','SPK') 
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			--AND CONVERT(varchar(20),e.NextFollowUpDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd 
			AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl2 g)
			AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
			--AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
			--AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
	) #t2

	DROP TABLE #empl2
	IF (@Param = '0') 
	BEGIN
		SELECT InquiryNumber, Pelanggan, AlamatProspek, TelpRumah, NamaPerusahaan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
		Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail 
		FROM #t2 
		WHERE CONVERT(varchar(20),NextFollowUpDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd
		ORDER BY InquiryNumber
	END
	ELSE 
	BEGIN
		SELECT InquiryNumber, Pelanggan, AlamatProspek, TelpRumah, NamaPerusahaan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
		Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail 
		FROM #t2 
		ORDER BY InquiryNumber
	END
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
			f.BranchName, a.InquiryNumber, a.NamaProspek Pelanggan, a.AlamatProspek, a.TelpRumah, a.NamaPerusahaan, CONVERT(VARCHAR(20),a.InquiryDate,106) InquiryDate, a.TipeKendaraan,
			a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
			c.EmployeeName Employee, d.EmployeeName Supervisor, CONVERT(VARCHAR(20),e.NextFollowUpDate,106) NextFollowUpDate, a.LastProgress, e.ActivityDetail
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
			AND LastProgress in ('P','HP','SPK')
			AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>'' OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
			--AND CONVERT(varchar(20),e.NextFollowUpDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd 
			AND a.EmployeeID IN (SELECT g.EmployeeID FROM #empl3 g)
			AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
			--AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
			--AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
	) #t3

	DROP TABLE #empl3
	IF (@Param = '0') 
	BEGIN
		SELECT InquiryNumber, Pelanggan, AlamatProspek, TelpRumah, NamaPerusahaan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
		Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail 
		FROM #t3 
		WHERE CONVERT(varchar(20),NextFollowUpDate, 112) BETWEEN @PeriodBegin AND @PeriodEnd
		ORDER BY InquiryNumber
	END
	ELSE 
	BEGIN
		SELECT InquiryNumber, Pelanggan, AlamatProspek, TelpRumah, NamaPerusahaan, InquiryDate, TipeKendaraan, Variant, Transmisi, Warna, PerolehanData,
		Employee, Supervisor, NextFollowUpDate, LastProgress, ActivityDetail 
		FROM #t3 
		ORDER BY InquiryNumber
	END
	DROP TABLE #t3
END
----
END

