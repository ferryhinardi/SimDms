 CREATE procedure [dbo].[uspfn_GetTaxInPeriod]
	@CompanyCode nvarchar(25),
	@BranchCode nvarchar(25),
	@ProductType nvarchar(2),
	@PeriodYear int,
	@PeriodMonth int
  
  as
  begin
--DECLARE @CompanyCode nvarchar(25) set @CompanyCode ='6006410'
--DECLARE @BranchCode nvarchar(25) set @BranchCode ='600641001'
--DECLARE @ProductType nvarchar(2) set @ProductType ='4W'
--DECLARE @PeriodYear int set @PeriodYear = 2014
--DECLARE @PeriodMonth int set @PeriodMonth =12

  exec uspfn_GetTaxInPeriod 6006410,600641001,'4W',2014,12


SELECT * INTO #1 FROM (
SELECT BranchCode, TaxNo, SupplierCode FROM gnTaxIn WITH(NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
    AND BranchCode = case @BranchCode when '' then BranchCode else @BranchCode end 
    AND ProductType = @ProductType 
	AND PeriodYear = @PeriodYear 
    AND PeriodMonth = @PeriodMonth
UNION
SELECT BranchCode, TaxNo, SupplierCode FROM gnTaxInHistory WITH(NOLOCK, NOWAIT)
WHERE CompanyCode = @CompanyCode 
    AND BranchCode = case @BranchCode when '' then BranchCode else @BranchCode end 
    AND ProductType = @ProductType 
	AND PeriodYear = @PeriodYear 
    AND PeriodMonth = @PeriodMonth 
    AND IsDeleted = '1'
) #1

/* AMBIL SEMUA DATA HPP SPARE */
select * into #t_1 from (
select b.SupplierCode, a.* 
from spTrnPHpp a
left join spTrnPRcvHdr b on b.CompanyCode = a.CompanyCode 
	and b.BranchCode = a.BranchCode
	and b.WRSNo = a.WRSNo
where
	a.CompanyCode	 = @CompanyCode
	and a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
	and a.YearTax	 = @PeriodYear
	and a.MonthTax	 = @PeriodMonth
)#t_1 

/* AMBIL SEMUA DATA BTT OTHER (AP) */
select * into #t_2 from (
select a.* 
from apTrnBTTOtherHdr a
where 
	a.CompanyCode = @CompanyCode
	and a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
	and a.FPJNo <> ''
	and SUBSTRING(a.FPJPeriod, 1, 4) = @PeriodYear
    and RIGHT(a.FPJPeriod, 2) = @PeriodMonth
)#t_2 

/* DATA HPP SPARE YANG MEMILIKI SUPPLIER DAN TAXNO YANG SAMA DENGAN BTT OTHER (NILAI DIJUMLAHKAN) */
SELECT * INTO #t_3 FROM (
SELECT
  a.CompanyCode
, a.BranchCode
, d.ProductType
, a.YearTax PeriodYear
, a.MonthTax PeriodMonth
, '300' ProfitCenter
, a.TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, b.SupplierCode
, c.SupplierGovName SupplierName
, c.IsPKP
, c.NPWPNo NPWP
, a.ReferenceNo FPJNo
, a.ReferenceDate FPJDate
, a.HPPNo + ' (' + a.ReferenceNo + ')' ReferenceNo
, a.WRSDate ReferenceDate
, a.TaxNo TaxNo
, a.TaxDate TaxDate
, a.DueDate SubmissionDate
, ISNULL((ISNULL(a.TotNetPurchAmt, 0) + ISNULL(a.DiffNetPurchAmt, 0)) + g.DppAmt,0) DPPAmt
, ISNULL((ISNULL(a.TotTaxAmt, 0) + ISNULL(a.DiffTaxAmt, 0)) + g.PPNAmt, 0) PPNAmt
, ISNULL(0 + g.PPNBmAmt, 0) PPNBmAmt
, 'PEMBELIAN SPARE PART' Description
, ISNULL(ISNULL(b.TotItem, 0)+(SELECT COUNT(*) FROM apTrnBTTOtherDtl WHERE CompanyCode = a.CompanyCode 
							AND BranchCode = a.BranchCode AND BTTNo = g.BTTNo), 0) Quantity	
FROM
	spTrnPHPP a	WITH(NOLOCK, NOWAIT)
	LEFT JOIN spTrnPRcvHdr b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.WRSNo = b.WRSNo
	LEFT JOIN gnMstSupplier c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND b.SupplierCode = c.SupplierCode
	LEFT JOIN gnMstCoProfile d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
		AND a.BranchCode = d.BranchCode
    INNER JOIN gnMstSupplierProfitCenter e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	    AND a.BranchCode = e.BranchCode
        AND b.SupplierCode = e.SupplierCode
    INNER JOIN gnMstTax f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
        AND e.TaxCode = f.TaxCode
	INNER JOIN #t_2 g on g.BranchCode=a.BranchCode AND g.SupplierCode = b.SupplierCode
		AND g.FPJNo = a.TaxNo
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
	AND ProductType = @ProductType
	AND a.YearTax = @PeriodYear
	AND a.MonthTax = @PeriodMonth
	AND a.Status = '2'
    AND e.ProfitCenterCode = '300'
    AND f.TaxPct > 0
) #t_3

-----------------------------------------------------------

/* QUERY UTAMA */
SELECT * INTO Query3S FROM (
-- SALES : PURCHASE
SELECT
  a.CompanyCode
, a.BranchCode
, e.ProductType
, YEAR(a.LockingDate) PeriodYear
, MONTH(a.LockingDate) PeriodMonth
, '100' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, a.SupplierCode 
, d.SupplierGovName SupplierName
, d.IsPKP 
, d.NPWPNo NPWP
, a.RefferenceFakturPajakNo FPJNo
, a.RefferenceFakturPajakDate FPJDate
, a.HPPNo + ' (' + a.RefferenceInvoiceNo + ')' ReferenceNo
, a.RefferenceInvoiceDate ReferenceDate
, a.RefferenceFakturPajakNo TaxNo
, a.RefferenceFakturPajakDate TaxDate
, a.DueDate SubmissionDate
, (SELECT SUM(ISNULL(Quantity, 0) * (ISNULL(AfterDiscDPP, 0) + ISNULL(OthersDPP, 0))) FROM omTrPurchaseHPPDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND HPPNo = a.HPPNo ) DPPAmt
, (SELECT SUM(ISNULL(Quantity, 0) * (ISNULL(AfterDiscPPn, 0) + ISNULL(OthersPPn, 0))) FROM omTrPurchaseHPPDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND HPPNo = a.HPPNo ) PPNAmt
, (SELECT SUM(ISNULL(Quantity, 0) * ISNULL(AfterDiscPPnBM, 0)) FROM omTrPurchaseHPPDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND HPPNo = a.HPPNo ) PPNBmAmt
, (SELECT TOP 1 SalesModelCode + ', NO. CHS. ' + CONVERT(VARCHAR, ChassisNo) FROM omTrPurchaseHPPSubDetail 
	WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND HPPNo = a.HPPNo) Description
, (SELECT COUNT(HPPSeq) FROM omTrPurchaseHPPSubDetail c 
    LEFT JOIN omTrPurchaseHPPDetailModel b ON c.CompanyCode = b.CompanyCode AND c.BranchCode = b.BranchCode 
    AND c.HPPNo = b.HPPNo AND c.BPUNo = b.BPUNo
    WHERE c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.HPPNo = a.HPPNo) Quantity
FROM
    omTrPurchaseHPP a WITH(NOLOCK, NOWAIT)
    LEFT JOIN gnMstSupplier d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
	    AND a.SupplierCode = d.SupplierCode
    LEFT JOIN gnMstCoProfile e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	    AND a.BranchCode = e.BranchCode
    INNER JOIN gnMstSupplierProfitCenter f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
	    AND a.BranchCode = f.BranchCode
        AND a.SupplierCode = f.SupplierCode
    INNER JOIN gnMstTax g WITH(NOLOCK, NOWAIT) ON a.CompanyCode = g.CompanyCode
        AND f.TaxCode = g.TaxCode        
WHERE
    a.CompanyCode = @CompanyCode
    AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
    AND ProductType = @ProductType
    AND YEAR(a.LockingDate) = @PeriodYear
    AND MONTH(a.LockingDate) = @PeriodMonth
    AND a.Status NOT IN ('3')
    AND f.ProfitCenterCode = '100'    
    AND g.TaxPct > 0
-------------------------------------------------------------------------------------
UNION
-- SALES : KAROSERI
SELECT
  a.CompanyCode
, a.BranchCode
, ProductType
, YEAR(a.LockingDate) PeriodYear
, MONTH(a.LockingDate) PeriodMonth
, '100' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, a.SupplierCode
, b.SupplierGovName SupplierName
, b.IsPKP
, b.NPWPNo NPWP
, a.RefferenceFakturPajakNo FPJNo
, a.RefferenceFakturPajakDate FPJDate
, a.KaroseriTerimaNo + ' (' + a.RefferenceInvoiceNo + ')' ReferenceNo
, a.RefferenceInvoiceDate ReferenceDate
, a.RefferenceFakturPajakNo TaxNo
, a.RefferenceFakturPajakDate TaxDate
, a.DueDate SubmissionDate
, ISNULL(a.Quantity, 0) * (ISNULL(a.DPPMaterial, 0) + ISNULL(a.DPPFee, 0) + ISNULL(a.DPPOthers, 0)) DPPAmt
, ISNULL(a.Quantity, 0) * ISNULL(a.PPn, 0) PPNAmt
, 0 PPNBmAmt
, 'Karoseri SPK No: ' + a.KaroseriSPKNo Description
, ISNULL(a.Quantity, 0) Quantity
FROM
    omTrPurchaseKaroseriTerima a WITH(NOLOCK, NOWAIT)
    LEFT JOIN gnMstSupplier b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
	    AND a.SupplierCode = b.SupplierCode
    LEFT JOIN gnMstCoProfile c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
	    AND a.BranchCode = c.BranchCode
    INNER JOIN gnMstSupplierProfitCenter d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
	    AND a.BranchCode = d.BranchCode
        AND a.SupplierCode = d.SupplierCode  
    INNER JOIN gnMstTax e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
        AND d.TaxCode = e.TaxCode              
WHERE
    a.CompanyCode = @CompanyCode
    AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
    AND ProductType = @ProductType
    AND YEAR(a.LockingDate) = @PeriodYear
    AND MONTH(a.LockingDate) = @PeriodMonth
    AND a.Status NOT IN ('3')
    AND d.ProfitCenterCode = '100'
    AND e.TaxPct > 0
-----------------------------------------------------------------------------------------
UNION
-- SALES : PURCHASE RETURN
SELECT
  a.CompanyCode
, a.BranchCode
, d.ProductType
, YEAR(ReturnDate) PeriodYear
, MONTH(ReturnDate) PeriodMonth
, '100' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'R' DocumentType
, c.SupplierCode SupplierCode
, c.SupplierGovName SupplierName
, c.IsPKP IsPKP
, c.NPWPNo NPWP
, replace(a.RefferenceNo,substring(a.RefferenceNo,1,1),'9') FPJNo
, a.RefferenceDate FPJDate
, a.ReturnNo + ' (' + a.RefferenceNo + ')' ReferenceNo
, a.RefferenceDate ReferenceDate
, replace(a.RefferenceNo,substring(a.RefferenceNo,1,1),'9') TaxNo  
, a.RefferenceDate TaxDate
, a.ReturnDate SubmissionDate
,(SELECT SUM(ISNULL(Quantity, 0) * (ISNULL(AfterDiscDPP, 0) + ISNULL(OthersDPP, 0))) FROM omTrPurchaseReturnDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND ReturnNo = a.ReturnNo) * -1 DPPAmt
,(SELECT SUM(ISNULL(Quantity, 0) * (ISNULL(AfterDiscPPn, 0) + ISNULL(OthersPPn, 0)))FROM omTrPurchaseReturnDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND ReturnNo = a.ReturnNo) * -1 PPNAmt
,(SELECT SUM(ISNULL(Quantity, 0) * ISNULL(AfterDiscPPnBM, 0)) FROM omTrPurchaseReturnDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND ReturnNo = a.ReturnNo) * -1 PPNBmAmt
, 'RTR-SLS-NO: ' + a.ReturnNo Description
, (SELECT SUM(ISNULL(Quantity, 0))FROM omTrPurchaseReturnDetailModel 
    WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND ReturnNo = a.ReturnNo) * -1 Quantity
FROM
    omTrPurchaseReturn a WITH(NOLOCK, NOWAIT)
    LEFT JOIN omTrPurchaseHPP b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
	    AND a.BranchCode = b.BranchCode
	    AND a.HPPNo = b.HPPNo
    LEFT JOIN gnMstSupplier c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
	    AND b.SupplierCode = c.SupplierCode
    LEFT JOIN gnMstCoProfile d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
	    AND a.BranchCode = d.BranchCode
    INNER JOIN gnMstSupplierProfitCenter e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	    AND a.BranchCode = e.BranchCode
        AND b.SupplierCode = e.SupplierCode  
    INNER JOIN gnMstTax f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
        AND e.TaxCode = f.TaxCode              
WHERE
    a.CompanyCode = @CompanyCode
    AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
    AND ProductType = @ProductType
    AND YEAR(ReturnDate) = @PeriodYear
    AND MONTH(ReturnDate) = @PeriodMonth
    AND a.Status NOT IN ('3')
    AND e.ProfitCenterCode = '100'
    AND f.TaxPct > 0
---------------------------------------------------------------------------------------
UNION
-- SERVICE
SELECT
 a.CompanyCode
, a.BranchCode
, a.ProductType
, YEAR(RecDate) PeriodYear
, MONTH(RecDate) PeriodMonth
, '200' ProfitCenter
, NULL TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, a.SupplierCode SupplierCode
, b.SupplierGovName SupplierName
, b.IsPKP IsPKP
, b.NPWPNo NPWP
, a.FPJNo FPJNo
, a.FPJDate FPJDate
, a.PONo + ' (' + a.JobOrderNo + ')' ReferenceNo
, a.RecDate RefferenceDate
, a.FPJGovNo TaxNo
, a.FPJDate TaxDate
, a.DueDate SubmissionDate
, ISNULL(a.DPPAmt, 0) DPPAmt
, ISNULL(a.PPnAmt, 0) PPNAmt
, 0 PPNBmAmt
, 'REC-SV-NO: ' + a.RecNo Description
, 1 Quantity
FROM
    svTrnPOSubCon a	WITH(NOLOCK, NOWAIT)
    LEFT JOIN gnMstSupplier b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
	    AND a.SupplierCode = b.SupplierCode
    INNER JOIN gnMstSupplierProfitCenter c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
	    AND a.BranchCode = c.BranchCode
        AND a.SupplierCode = c.SupplierCode      
    INNER JOIN gnMstTax d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
        AND c.TaxCode = d.TaxCode          
WHERE
    a.CompanyCode = @CompanyCode
    AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
    AND a.ProductType = @ProductType
    AND YEAR(RecDate) = @PeriodYear
    AND MONTH(RecDate) = @PeriodMonth
    AND c.ProfitCenterCode = '200'
    AND d.TaxPct > 0
---------------------------------------------------------------------------------------
UNION 
-- SPAREPART
SELECT
  a.CompanyCode
, a.BranchCode
, d.ProductType
, a.YearTax PeriodYear
, a.MonthTax PeriodMonth
, '300' ProfitCenter
, a.TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, b.SupplierCode
, c.SupplierGovName SupplierName
, c.IsPKP
, c.NPWPNo NPWP
, a.ReferenceNo FPJNo
, a.ReferenceDate FPJDate
, a.HPPNo + ' (' + a.ReferenceNo + ')' ReferenceNo
, a.WRSDate ReferenceDate
, a.TaxNo TaxNo
, a.TaxDate TaxDate
, a.DueDate SubmissionDate
, ISNULL(a.TotNetPurchAmt, 0) + ISNULL(a.DiffNetPurchAmt, 0) DPPAmt
, ISNULL(a.TotTaxAmt, 0) + ISNULL(a.DiffTaxAmt, 0) PPNAmt
, 0 PPNBmAmt
, 'PEMBELIAN SPARE PART' Description
, ISNULL(b.TotItem, 0) Quantity	
FROM
	spTrnPHPP a	WITH(NOLOCK, NOWAIT)
	LEFT JOIN spTrnPRcvHdr b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
		AND a.BranchCode = b.BranchCode
		AND a.WRSNo = b.WRSNo
	LEFT JOIN gnMstSupplier c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
		AND b.SupplierCode = c.SupplierCode
	LEFT JOIN gnMstCoProfile d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
		AND a.BranchCode = d.BranchCode
    INNER JOIN gnMstSupplierProfitCenter e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	    AND a.BranchCode = e.BranchCode
        AND b.SupplierCode = e.SupplierCode
    INNER JOIN gnMstTax f WITH(NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
        AND e.TaxCode = f.TaxCode
WHERE
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
	AND ProductType = @ProductType
	AND a.YearTax = @PeriodYear
	AND a.MonthTax = @PeriodMonth
	AND a.Status = '2'
    AND e.ProfitCenterCode = '300'
    AND f.TaxPct > 0
	AND b.BranchCode+'-'+b.SupplierCode+'-'+a.TaxNo NOT IN (SELECT BranchCode+'-'+SupplierCode+'-'+TaxNo FROM #t_3)
-------------------------------------------------------------------------------
UNION
-- FINANCE
SELECT
  a.CompanyCode
, a.BranchCode
, c.ProductType
, SUBSTRING(a.FPJPeriod, 1, 4) PeriodYear
, RIGHT(a.FPJPeriod, 2) PeriodMonth
, '000' ProfitCenter
, 'NULL' TypeOfGoods
, 'B' TaxCode
, '2' TransactionCode
, '1' StatusCode
, '1' DocumentCode
, 'F' DocumentType
, a.SupplierCode
, b.SupplierGovName SupplierName
, b.IsPKP
, b.NPWPNo NPWP
, a.FPJNo
, a.FPJDate
, a.BTTNo + ' (' + a.ReffNo + ')' ReferenceNo
, a.ReffDate ReferenceDate
, a.FPJNo TaxNo
, a.FPJDate TaxDate
, a.DueDate SubmissionDate
, a.DPPAmt
, a.PPNAmt
, a.PPnBMAmt
, (SELECT TOP 1 Description FROM apTrnBTTOtherDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND BTTNo = a.BTTNo) Description
, (SELECT COUNT(*) FROM apTrnBTTOtherDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND BTTNo = a.BTTNo) Quantity	
FROM
    apTrnBTTOtherHdr a	WITH(NOLOCK, NOWAIT)    
    LEFT JOIN gnMstSupplier b WITH(NOLOCK, NOWAIT) ON a.CompanyCode = b.CompanyCode
	    AND a.SupplierCode = b.SupplierCode
    LEFT JOIN gnMstCoProfile c WITH(NOLOCK, NOWAIT) ON a.CompanyCode = c.CompanyCode
	    AND a.BranchCode = c.BranchCode
    INNER JOIN gnMstSupplierProfitCenter d WITH(NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
	    AND a.BranchCode = d.BranchCode
        AND a.SupplierCode = d.SupplierCode
    INNER JOIN gnMstTax e WITH(NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
        AND d.TaxCode = e.TaxCode               
WHERE
    a.CompanyCode = @CompanyCode
    AND a.BranchCode = case @BranchCode when '' then a.BranchCode else @BranchCode end
    AND ProductType = @ProductType
    AND SUBSTRING(a.FPJPeriod, 1, 4) = @PeriodYear
    AND RIGHT(a.FPJPeriod, 2) = @PeriodMonth
    AND a.Category = 'INV'
    AND d.ProfitCenterCode = '000'
    AND e.TaxPct > 0
    AND a.BranchCode+'-'+a.SupplierCode+'-'+a.FPJNo NOT IN (SELECT BranchCode+'-'+SupplierCode+'-'+TaxNo FROM #t_3)
-------------------------------------------------------------------------------
UNION
-- SPARE PART DAN BTT YANG MEMILIKI SUPPLIER DAN TAXNO YANG SAMA
SELECT * FROM #t_3
) Query3S

select ROW_NUMBER() OVER(ORDER BY q1.BranchCode, q1.ProfitCenter ASC, q1.TaxNo)  SeqNo
    , q1.CompanyCode
	, q1.BranchCode
	, q1.ProductType
	, q1.PeriodYear
	, q1.PeriodMonth
	, q1.ProfitCenter
	, ISNULL((SELECT TOP 1 TypeOfGoods FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo), '') TypeOfGoods
	, q1.TaxCode
	, q1.TransactionCode
	, q1.StatusCode
	, q1.DocumentCode
	, q1.DocumentType
	, q1.SupplierCode
	, q1.SupplierName
	, q1.IsPKP
	, q1.NPWP
	, (SELECT TOP 1 FPJNo FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) FPJNo
	, (SELECT TOP 1 FPJDate FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) FPJDate
	, (SELECT TOP 1 ReferenceNo FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) ReferenceNo
	, (SELECT TOP 1 ReferenceDate FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) ReferenceDate
	, q1.TaxNo
	, (SELECT TOP 1 TaxDate FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) TaxDate
	, (SELECT TOP 1 SubmissionDate FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) SubmissionDate
	, SUM(q1.DPPAmt) DPPAmt
	, SUM(q1.PPNAmt) PPNAmt
	, SUM(q1.PPnBMAmt) PPnBMAmt
	, (SELECT TOP 1 Description FROM Query3S WHERE CompanyCode = q1.CompanyCode AND BranchCode = q1.BranchCode AND 
		ProductType = q1.ProductType AND PeriodYear = q1.PeriodYear AND PeriodMonth = q1.PeriodMonth AND 
		SupplierCode = q1.SupplierCode AND TaxNo = q1.TaxNo) Description
	, SUM(q1.Quantity) Quantity
from Query3S q1
group by
	q1.CompanyCode
	, q1.BranchCode
	, q1.ProductType
	, q1.PeriodYear
	, q1.PeriodMonth
	, q1.ProfitCenter
	, q1.TaxCode
	, q1.TransactionCode
	, q1.StatusCode
	, q1.DocumentCode
	, q1.DocumentType
	, q1.SupplierCode
	, q1.SupplierName
	, q1.IsPKP
	, q1.NPWP
	, q1.TaxNo 

select
    ISNULL(SUM(DPPAmt),0) DPPAmt
    ,ISNULL(SUM(PPnAmt),0) PPnAmt
	,ISNULL((SELECT SUM(DPPAmt) FROM Query3S WHERE IsPKP='1'),0) SumDPPStd 
	,ISNULL((SELECT SUM(DPPAmt) FROM Query3S WHERE IsPKP='0'),0) SumDPPSdh 
	,ISNULL((SELECT SUM(PPnAmt) FROM Query3S WHERE IsPKP='1'),0) SumPPNStd
	,ISNULL((SELECT SUM(PPnAmt) FROM Query3S WHERE IsPKP='0'),0) SumPPNSdh
	,ISNULL(SUM(PPnBMAmt),0) SumPPnBMAmt
from Query3S

drop table Query3S
drop table #1
drop table #t_1
drop table #t_2
drop table #t_3
end