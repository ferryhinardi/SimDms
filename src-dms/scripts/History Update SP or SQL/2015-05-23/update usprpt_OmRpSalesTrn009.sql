ALTER procedure [dbo].[usprpt_OmRpSalesTrn009] 
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
