if object_id('usprpt_PmRpInqByPeriod') is not null
	drop procedure usprpt_PmRpInqByPeriod
GO
CREATE procedure [dbo].[usprpt_PmRpInqByPeriod] 
	-- Add the parameters for the stored procedure here
(
	@CompanyCode	VARCHAR(15),
	@BranchCode		VARCHAR(15),
	@DateAwal		VARCHAR(10),
	@DateAkhir		VARCHAR(10),
	@Outlet			VARCHAR(15),
	@SPV			VARCHAR(15),
	@EMP			VARCHAR(15)
)
AS
BEGIN
	SELECT
		f.OutletName
		,convert(varchar(20),a.InquiryNumber) InquiryNumber
		,a.NamaProspek Pelanggan
		,a.AlamatProspek
		,a.TelpRumah
		,a.NamaPerusahaan
		,a.AlamatPerusahaan
		,a.Handphone
		,CASE(a.InquiryDate) WHEN '19000101' THEN NULL ELSE a.InquiryDate END InquiryDate
		,a.TipeKendaraan
		,a.Variant
		,a.Transmisi
		,b.RefferenceDesc1 Warna
		,a.PerolehanData
		,c.EmployeeName Employee
		,d.EmployeeName Supervisor
		,CASE(e.NextFollowUpDate) WHEN '19000101' THEN NULL ELSE e.NextFollowUpDate END NextFollowUpDate
		,a.LastProgress
		,a.LastUpdateStatus
		,CASE(a.SPKDate) WHEN '19000101' THEN NULL ELSE a.SPKDate END SPKDate
		,CASE(a.LostCaseDate) WHEN '19000101' THEN NULL ELSE a.LostCaseDate END LostCaseDate
	FROM
		PmKDP a
	LEFT JOIN OmMstRefference b
		ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
	LEFT JOIN GnMstEmployee c
		ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.EmployeeID = a.EmployeeID
	LEFT JOIN GnMstEmployee d
		ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.EmployeeID = a.SpvEmployeeID
	LEFT JOIN PmActivities e
		ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
		AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
		AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
	LEFT JOIN PmBranchOutlets f
		ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode AND f.OutletID = a.OutletID
	WHERE
		a.CompanyCode = @CompanyCode 
		AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>''OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
		AND CONVERT(VARCHAR, a.InquiryDate, 112) BETWEEN @DateAwal AND @DateAkhir
		AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
		AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
		AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
	ORDER BY
		a.InquiryNumber 
END