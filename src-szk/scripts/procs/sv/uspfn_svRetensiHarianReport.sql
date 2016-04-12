/*
	[uspfn_svRetensiHarianReport] '100', '6006400001', '6006400101', '20140301', '20140331', '', '', '20150101', '20150131'
	[uspfn_svRetensiHarianReport] '100', '6006406', '6006406', '20140301', '20140331', '5000', '', '20140301', '20140331'
	DROP PROC [uspfn_svRetensiHarianReport]
*/
ALTER PROCEDURE [dbo].[uspfn_svRetensiHarianReport]
	@GroupNo Varchar(10),
	@CompanyCode Varchar(15),
	@BranchCode Varchar(15),
	@StartVisitedDate Varchar(15),
	@EndVisitedDate Varchar(15),
	@PMNext VARCHAR(15),
	@Activity CHAR(1), -- R = REMINDER, F = FOLLOW UP
	@StartPeriode Varchar(15),
	@EndPeriode Varchar(15)
AS
IF @GroupNo = '' SET @GroupNo = NULL;
IF @CompanyCode = '' SET @CompanyCode = NULL;
IF @BranchCode = '' SET @BranchCode = NULL;
IF @Activity = '' SET @Activity = NULL;
IF @PMNext = '' SET @PMNext = NULL;

DECLARE @Declare VARCHAR(MAX)
DECLARE @Sql VARCHAR(MAX)
DECLARE @Where VARCHAR(MAX)
DECLARE @Query VARCHAR(MAX)

IF @GroupNo IS NULL SET @Declare = 'DECLARE @GroupNo Varchar(10) = NULL' ELSE SET @Declare = 'DECLARE @GroupNo Varchar(10) = ''' + @GroupNo + ''''
IF @CompanyCode IS NULL SET @Declare = @Declare + ' DECLARE @CompanyCode Varchar(15) = NULL' ELSE SET @Declare = @Declare + ' DECLARE @CompanyCode Varchar(15) = ''' + @CompanyCode + ''''
IF @BranchCode IS NULL SET @Declare = @Declare + ' DECLARE @BranchCode Varchar(15) = NULL' ELSE SET @Declare = @Declare + ' DECLARE @BranchCode Varchar(15) = ''' + @BranchCode + ''''
IF @PMNext IS NULL SET @Declare = @Declare + ' DECLARE @PMNext INT = NULL' ELSE SET @Declare = @Declare + ' DECLARE @PMNext INT = ' + @PMNext + ''
SET @Declare = @Declare 
			+ ' DECLARE @StartVisitedDate DATETIME = ''' + @StartVisitedDate + ''''
			+ ' DECLARE @EndVisitedDate DATETIME = ''' + @EndVisitedDate + ''''
			+ ' DECLARE @StartPeriode DATETIME = ''' + @StartPeriode + ''''
			+ ' DECLARE @EndPeriode DATETIME = ''' + @EndPeriode + ''''

SET @Sql = @Declare + ' SELECT DISTINCT ' +
	'e.LookUpValueName AS Inisial' +
	', b.BasicModel AS [Type]' + 
	', b.PoliceRegNo AS [NoPolisi]' +
	', CASE WHEN b.TransmissionType IS NULL THEN ''MT'' ELSE CASE WHEN b.TransmissionType = '' '' THEN ''MT'' ELSE b.TransmissionType END END TM' +
	', YEAR(GETDATE()) AS [Tahun]' + 
	', b.EngineCode AS [KodeMesin]' +
	', CAST(b.EngineNo AS VARCHAR) AS [NoMesin]' +
	', b.ChassisCode AS [KodeRangka]' + 
	', CAST(b.ChassisNo AS VARCHAR) AS [NoRangka]' + 
	', c.CustomerName AS [NamaPelanggan]' +
	', ISNULL(c.Address1, '''') + '' '' + ISNULL(c.Address2, '''') + '' '' + ISNULL(c.Address3, '''') + '' '' + ISNULL(c.Address4, '''') AS [AlamatPelanggan]' +
	', c.PhoneNo AS [TelponRumah]' +
	', c.OfficePhoneNo AS [TelponKantor]' +
	', CASE WHEN (CONVERT(VARCHAR, b.LastServiceDate, 112) = ''19000101'') THEN a.JobOrderDate WHEN b.LastServiceDate < a.JobOrderDate THEN a.JobOrderDate ELSE b.LastServiceDate END AS [TanggalKunjungan]' +
	', c.HPNo AS [HP]' +
	', CAST(a.Odometer AS VARCHAR) AS [RM]' +
	', CAST(d.PMNow AS VARCHAR) AS [PMSekarang]' +
	', CAST(d.PMNext AS VARCHAR) AS [PMBerikut]';
IF @Activity = 'F' OR @Activity = 'R'
	SET @Sql = @Sql +
	', ISNULL(' +
		'CASE WHEN a.JobType LIKE ''PB%'' THEN ' +
 			'CASE WHEN REPLACE(a.JobType,''KM'','''') = ''PB100000'' THEN ' +
				'CASE WHEN b.LastServiceDate IS NOT NULL THEN ' +
					'DATEADD(MONTH,3, b.LastServiceDate) ' +
				'ELSE b.LastServiceDate END' +
			' ELSE DATEADD(MONTH, ' +
					'(SELECT CONVERT(INT, ' +
						'(SELECT b.TimePeriod FROM svMstRoutineService b ' +
							'WHERE JobType = ''PB'' + CONVERT(VARCHAR, CONVERT(INT, SUBSTRING(REPLACE(a.JobType,''KM'',''''), 3, LEN(a.JobType)-2))+5000)' +
						')) - CONVERT(INT,(SELECT TimePeriod FROM svMstRoutineService ' +
							'WHERE JobType = (SELECT JobType FROM svTrnService WHERE CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and ServiceNo = a.ServiceNo))) TimePeriod)' +
					',b.LastServiceDate)' +
			' END' +
		' ELSE DATEADD(MONTH, 3, b.LastServiceDate)' +
		' END, '''') AS [EstimasiBerikut]';
ELSE SET @Sql = @Sql + ', DateAdd(MONTH,3, b.LastServiceDate) as [Estimasi Berikut]';
SET @Sql = @Sql +
	', CASE(d.ReminderDate) WHEN ''19000101'' THEN NULL ELSE d.ReminderDate END AS [TglReminder]' + 
	', CASE(d.IsConfirmed) WHEN ''1'' THEN ''Ya'' ELSE ''Tidak'' END AS [BerhasilDiHubungi]' +
	', CASE(d.IsBooked) WHEN ''1'' THEN ''Ya'' ELSE ''Tidak'' END AS [Booking]' +
	', CASE(d.BookingDate) WHEN ''19000101'' THEN NULL ELSE d.BookingDate END AS [TglBooking]' +
	', CASE(d.IsVisited) WHEN ''1'' THEN ''Ya'' ELSE ''Tidak'' END AS [KonsumenDatang]' + 
	', CASE(d.FollowUpDate) WHEN ''19000101'' THEN NULL ELSE d.FollowUpDate END AS [TglFollowUp]' +
	', CASE(d.IsSatisfied) WHEN ''1'' THEN ''Ya'' ELSE ''Tidak'' END AS [Kepuasan]' +
	', d.Reason AS Alasan' +
	', CASE WHEN b.ContactName IS NULL THEN CONVERT(VARCHAR,c.CustomerName) ELSE b.ContactName END AS [NamaKontak]' +
	', CASE WHEN b.ContactAddress IS NULL THEN ISNULL(c.Address1, '''') + '' '' + ISNULL(c.Address2, '''') + '' '' + ISNULL(c.Address3, '''') +'' ''+ ISNULL(c.Address4, '''') ELSE b.ContactAddress END AS [AlamatKontak]' +
	', CASE WHEN b.ContactPhone IS NULL THEN c.PhoneNo ELSE b.ContactPhone END AS [TeleponKontak]' +
	', h.EmployeeName AS [NamaServiceAdvisor]' +
	', i.EmployeeName AS [NamaMekanik]' + 
	', a.ServiceRequestDesc AS [PermintaanPerawatan]' +
	', CONVERT(VARCHAR, g.Remarks) AS [Rekomendasi]' +
	' FROM svTrnService a' +
	' LEFT JOIN SvMstCustomerVehicle b ON b.Companycode = a.CompanyCode AND b.ChassisCode = a.ChassisCode AND b.ChassisNo = a.ChassisNo'+
	' LEFT JOIN GnMstCustomer c ON c.CompanyCode = a.CompanyCode AND c.CustomerCode = a.CustomerCode';
	IF @Activity = 'F' OR @Activity = 'R'
	  SET @Sql = @Sql + ' LEFT JOIN svTrnDailyRetention d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.CustomerCode = a.CustomerCode AND d.ChassisCode = a.ChassisCode AND d.ChassisNo = a.ChassisNo';
	ELSE
	  SET @Sql = @Sql + ' LEFT JOIN (SELECT CompanyCode, BranchCode, ChassisCode, ChassisNo, VisitInitial, VisitDate, CustomerCategory, RetentionNo, PMNow, PMNext, ReminderDate, IsConfirmed, IsBooked, BookingDate, IsVisited, FollowUpDate, IsSatisfied, Reason, LastRemark FROM svTrnDailyRetention DR WHERE RetentionNo = (SELECT TOP 1 RetentionNo FROM svTrnDailyRetention WHERE CompanyCode = DR.CompanyCode AND BranchCode = DR.BranchCode AND ChassisCode = DR.ChassisCode AND ChassisNo = DR.ChassisNo ORDER BY RetentionNo DESC)) d ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.ChassisCode = a.ChassisCode AND d.ChassisNo = a.ChassisNo';

	  SET @Sql = @Sql + ' LEFT JOIN gnMstLookupDtl e ON e.CompanyCode = a.CompanyCode AND e.CodeID = ''CIRS'' AND e.LookupValue = d.VisitInitial' +
	  ' LEFT JOIN gnMstLookUpDtl f ON b.CompanyCode = a.CompanyCode AND f.CodeId = ''CCRS'' AND f.LookUpValue = d.CustomerCategory' +
	  ' LEFT JOIN svTrnInvoice g ON g.CompanyCode = a.CompanyCode AND g.BranchCode = a.BranchCode AND g.ProductType = a.ProductType AND g.InvoiceNo = a.InvoiceNo' +
	  ' LEFT JOIN gnMstEmployee h ON h.CompanyCode = a.CompanyCode AND h.BranchCode = a.BranchCode AND h.EmployeeID = a.ForemanID' +
	  ' LEFT JOIN gnMstEmployee i ON i.CompanyCode = a.CompanyCode AND i.BranchCode = a.BranchCode AND i.EmployeeID = a.MechanicID' +
	  ' LEFT JOIN gnMstDealerOutletMapping j ON j.DealerCode = a.CompanyCode AND j.OutletCode = a.BranchCode';
	
SET @Where = ' WHERE 1 = 1' +
			' AND (@GroupNo IS NULL OR j.GroupNo = @GroupNo)' +
			' AND (@CompanyCode IS NULL OR a.CompanyCode = @CompanyCode)' +
			' AND (@BranchCode IS NULL OR a.BranchCode  = @BranchCode)' + 
			' AND (@PMNext IS NULL OR d.PMNext = @PMNext)' +
			' AND CASE WHEN (CONVERT(VARCHAR, b.LastServiceDate, 112) = ''19000101'') THEN a.JobOrderDate WHEN b.LastServiceDate < a.JobOrderDate THEN a.JobOrderDate ELSE b.LastServiceDate END BETWEEN @StartVisitedDate AND @EndVisitedDate';

IF @Activity = 'F' OR @Activity = 'R'
	SET @Where = @Where + ' AND (b.LastServiceDate IS NULL OR CAST(a.JobOrderDate AS DATE) >= CAST(b.LastServiceDate AS DATE))'+
						' AND d.RetentionNo = ISNULL((SELECT MAX(RetentionNo) FROM svTrnDailyRetention WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode and PeriodYear = YEAR(a.JobOrderDate) AND PeriodMonth = MONTH(a.JobOrderDate) AND CustomerCode = a.CustomerCode), 0)';
ELSE
	SET @Where = @Where + ' AND (b.LastServiceDate IS NULL OR CAST(a.JobOrderDate AS DATE) >= CAST(b.LastServiceDate AS DATE))';

IF @Activity = 'F' -- Follow
	SET @Where = @Where + ' AND a.ServiceStatus not in (''0'',''1'',''2'',''3'',''4'',''5'',''6'') AND CAST(d.FollowUpDate AS DATE) BETWEEN CAST(@StartPeriode AS DATE) AND CAST(@EndPeriode AS DATE)';
ELSE IF @Activity = 'R' -- Reminder
	SET @Where = @Where + ' AND CAST(d.ReminderDate AS DATE) BETWEEN CAST(@StartPeriode AS DATE) AND CAST(@EndPeriode AS DATE)';
ELSE -- All
	SET @Where = @Where + ' AND a.ServiceStatus in (''7'',''9'')';  -- 7:Invoice, 9:Faktur Pajak

SET @Query = @Sql + @Where;
-- SELECT @Query;
EXEC sp_sqlexec @Query;