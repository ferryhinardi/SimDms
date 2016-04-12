create procedure uspfn_sp_inquiry_sales_order
	@companycode varchar(15), 
	@branchcode varchar(15), 
	@OrderBy varchar(50),
	@CustomParameter varchar(100),
	@RangeDate varchar(100) 
 AS
	SET NOCOUNT ON;

	DECLARE @SQLQuery NVARCHAR(MAX)
	
	SET @SQLQuery = '
	SELECT
    ROW_NUMBER() OVER(ORDER BY ' + @OrderBy + ' ) RowNumber
    , (SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = ''PYBY'' AND 
        LookUpValue = SOHdr.PaymentCode) PaymentBy
	, a.PickingSlipNo
	, a.PickingSlipDate
    , ISNULL(UPPER((SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 
        ''STAT'' AND LookUpValue = a.Status)), ''-'') StatusPickingSlip
	, ISNULL(b.InvoiceNo, ''-'') InvoiceNo
	, b.InvoiceDate
    , ISNULL(UPPER((SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 
        ''STAT'' AND LookUpValue = b.Status)), ''-'') StatusInvoice
	, ISNULL(c.FPJNo, ''-'') FPJNo
    , c.FPJDate
    , ISNULL(UPPER((SELECT LookUpValueName FROM gnMstLookUpDtl WHERE CompanyCode = a.CompanyCode AND CodeID = 
        ''STAT'' AND LookUpValue = c.Status)), ''-'') StatusFPJ
	, Cust.CustomerName
	, c.DueDate
	, c.FPJGovNo
	, c.FPJSignature
	, ISNULL(c.TOPDays, 0) TOPDays
	, ISNULL(c.TotSalesAmt, 0) TotSalesAmt
	, ISNULL(c.TotDiscAmt, 0) TotDiscAmt
	, ISNULL(c.TotDPPAmt, 0) TotDPPAmt
	, ISNULL(c.TotPPNAmt, 0) TotPPNAmt
	, ISNULL(c.TotFinalSalesAmt, 0) TotFinalSalesAmt
    , CONVERT(BIT, CASE WHEN LEN(ISNULL(c.FPJNo, '''')) = 0 THEN 0 ELSE 1 END) IsTransferGL
FROM
	spTrnSPickingHdr a WITH(NOLOCK, NOWAIT)
	LEFT JOIN spTrnSInvoiceHdr b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.PickingSlipNo = b.PickingSlipNo
	LEFT JOIN spTrnSFPJHdr c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND b.FPJNo = c.FPJNo	    
	LEFT JOIN (
		SELECT	
			DISTINCT(x.CompanyCode), x.BranchCode, PickingSlipNo, PaymentCode
		FROM
			spTrnSORDHdr x WITH(NOLOCK, NOWAIT)
			LEFT JOIN spTrnSPickingDtl y WITH(NOLOCK, NOWAIT) ON x.CompanyCode = y.CompanyCode
				AND x.BranchCode = y.BranchCode
				AND x.DocNo = y.DocNo
	) SOHdr ON a.CompanyCode = SOHdr.CompanyCode
		AND a.BranchCode = SOHdr.BranchCode
		AND a.PickingSlipNo = SOHdr.PickingSlipNo
    LEFT JOIN (
        SELECT 
            CompanyCode, CustomerCode, CustomerName + '' ('' + CustomerCode + '')'' CustomerName
        FROM 
            gnMstCustomer WITH(NOLOCK, NOWAIT)
    ) Cust ON a.CompanyCode = Cust.CompanyCode
	    AND a.CustomerCode = Cust.CustomerCode
WHERE
	a.CompanyCode ='''+ @CompanyCode + '''
	AND a.BranchCode =''' + @BranchCode + '''
    AND a.SalesType = ''0''
	--AND a.TypeOfGoods = @TypeOfGoods
    ' + @CustomParameter + ' 
    ' + @RangeDate + '
ORDER BY
	RowNumber
SELECT
	SUM(ISNULL(c.TotSalesAmt, 0)) TotSalesAmt
	, SUM(ISNULL(c.TotDiscAmt, 0)) TotDiscAmt
	, SUM(ISNULL(c.TotDPPAmt, 0)) TotDPPAmt
	, SUM(ISNULL(c.TotPPNAmt, 0)) TotPPNAmt
	, SUM(ISNULL(c.TotFinalSalesAmt, 0)) TotFinalSalesAmt
FROM
	spTrnSPickingHdr a WITH(NOLOCK, NOWAIT)
	LEFT JOIN spTrnSInvoiceHdr b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.PickingSlipNo = b.PickingSlipNo
	LEFT JOIN spTrnSFPJHdr c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND a.BranchCode = c.BranchCode
		AND b.FPJNo = c.FPJNo	    
	LEFT JOIN (
		SELECT	
			DISTINCT(x.CompanyCode), x.BranchCode, PickingSlipNo, PaymentCode
		FROM
			spTrnSORDHdr x WITH(NOLOCK, NOWAIT)
			LEFT JOIN spTrnSPickingDtl y WITH(NOLOCK, NOWAIT) ON x.CompanyCode = y.CompanyCode
				AND x.BranchCode = y.BranchCode
				AND x.DocNo = y.DocNo
	) SOHdr ON a.CompanyCode = SOHdr.CompanyCode
		AND a.BranchCode = SOHdr.BranchCode
		AND a.PickingSlipNo = SOHdr.PickingSlipNo
    LEFT JOIN (
        SELECT 
            CompanyCode, CustomerCode, CustomerName + '' ('' + CustomerCode + '')'' CustomerName
        FROM 
            gnMstCustomer WITH(NOLOCK, NOWAIT)
    ) Cust ON a.CompanyCode = Cust.CompanyCode
	    AND a.CustomerCode = Cust.CustomerCode
WHERE
	a.CompanyCode = ''' + @CompanyCode + '''
	AND a.BranchCode = ''' + @BranchCode + '''
    AND a.SalesType = ''0''
	--AND a.TypeOfGoods = @TypeOfGoods
    ' + @CustomParameter + ' 
    ' + @RangeDate 

	EXECUTE sp_executesql @SQLQuery	

GO