ALTER procedure [dbo].[usprpt_GnPostingTaxOut]	
	@CompanyCode as varchar(15)
	,@BranchCode as varchar(15)
	,@PeriodMonth as int 
	,@PeriodYear as int 
	,@ProductType as varchar(15)
	,@LastSeqNo int
	,@UserId as varchar(50)
AS

--declare	@CompanyCode as varchar(15)
--		,@BranchCode as varchar(15)
--		,@PeriodMonth as int 
--		,@PeriodYear as int 
--		,@ProductType as varchar(15)
--		,@LastSeqNo int
--		,@UserId as varchar(50)
--
--set @CompanyCode='6058401'
--set @BranchCode=''
--set @PeriodMonth=7
--set @PeriodYear=2011
--set @ProductType='4W'
--set @UserId='sa'
--set @LastSeqNo=9806

declare @t_tax table(
	CompanyCode varchar(15),
	BranchCode varchar(15),
	ProductType varchar(15),
	PeriodYear numeric(4, 0),
	PeriodMonth numeric(2, 0),
	TaxNo varchar(50),
	ReferenceNo varchar(20),
	ProfitCenter varchar(15),
	DocumentType char(1),
	TypeOfGoods char(1),
	TaxCode varchar(15),
	TransactionCode varchar,
	StatusCode varchar(15),
	DocumentCode varchar(15),
	CustomerCode varchar(15),
	CustomerName varchar(100),
	IsPKP bit,
	NPWP varchar(100),
	SKPNo varchar(50),
	FPJNo varchar(50),
	FPJDate datetime,
	ReferenceDate datetime,
	TaxDate datetime,
	SubmissionDate datetime,
	DPPAmt numeric(30, 0),
	PPNAmt numeric(30, 0),
	PPNBmAmt numeric(30, 0),
	Description varchar(100),
	Quantity numeric(9, 0)
)

declare @IsFPJGab as bit
if (SELECT count(ParaValue) FROM gnMstLookUpDtl WHERE CompanyCode=@CompanyCode AND CodeID='FFPG' AND LookUpValue='STAT') > 0
	SET @IsFPJGab=(SELECT ParaValue FROM gnMstLookUpDtl WHERE CompanyCode=@CompanyCode AND CodeID='FFPG' AND LookUpValue='STAT')
else
	SET @IsFPJGab=0

--------------------------------------------------------------------------------------------
-- Get Record from Spraepart Transaction
--------------------------------------------------------------------------------------------
IF @IsFPJGab = 1 -- if in FPJ Gabungan mode
BEGIN
	insert into @t_tax
	SELECT 
		a.CompanyCode,a.BranchCode,b.ProductType,@PeriodYear AS PeriodYear,@PeriodMonth AS PeriodMonth, a.FpjGovNo AS TaxNo
		,a.InvoiceNo ReferenceNo,'300' ProfitCenter
		,case when d.isPKP = 1 then 'F' else 'S' end DocumentType
		,e.TypeOfGoods,'A' TaxCode,'2' TransactionCode,replace(g.TaxTransCode,'0','') StatusCode
		,CASE d.isPKP WHEN 1 THEN 1 ELSE 2 END AS DocumentCode,a.CustomerCodeBill CustomerCode,c.CustomerGovName CustomerName
		,CASE d.isPKP WHEN 1 THEN 1 ELSE 0 END AS IsPKP,d.NPWPNo NPWP, d.SKPNo,a.FPGNo  AS FPJNo,a.FPGDate AS FPJDate
		,a.InvoiceDate ReferenceDate,a.SignedDate AS TaxDate,a.DueDate SubmissionDate,a.TotDppAmt AS DPPAmt,a.TotPPNAmt AS PPNAmt,0 PPNBmAmt
		,'INV-SP-NO: ' + a.InvoiceNo AS Description,e.TotSalesQty AS Quantity
	FROM 
		SpTrnSFakturPajak a
		LEFT JOIN SpTrnSFPJHdr e on a.CompanyCode = e.CompanyCode AND a.BranchCode = e.BranchCode AND a.FPJNo = e.FPJNo
		LEFT JOIN SpTrnSFpjInfo  d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND a.FPJNo = d.FPJNo
		LEFT JOIN GnMstCoProfile b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
		LEFT JOIN GnMStCustomer  c ON a.CompanyCode = c.CompanyCode AND a.CustomerCodeBill = c.CustomerCode
		LEFT JOIN gnMstCustomerProfitCenter g on a.CompanyCode=g.CompanyCode and a.BranchCode=g.BranchCode and a.CustomerCode=g.CustomerCode
			and g.ProfitCenterCode='300'
	WHERE 
		a.CompanyCode = @CompanyCode 
		AND ((case when @BranchCode='' then a.BranchCode end) <> ''
			or (case when @BranchCode<>'' then a.BranchCode end) = @BranchCode)
		AND month(a.FPGDate) = @PeriodMonth AND year(a.FPGDate) = @PeriodYear AND b.ProductType = @ProductType
END
ELSE -- if not in FPJ Gabungan mode
BEGIN
	insert into @t_tax
	SELECT 
		a.CompanyCode,a.BranchCode,b.ProductType,@PeriodYear AS PeriodYear,@PeriodMonth AS PeriodMonth, a.FpjGovNo AS TaxNo
		,a.InvoiceNo ReferenceNo,'300' ProfitCenter
		,case when d.isPKP = 1 then 'F' else 'S' end DocumentType
		,a.TypeOfGoods,'A' TaxCode,'2' TransactionCode,replace(g.TaxTransCode,'0','') StatusCode
		,CASE d.isPKP WHEN 1 THEN 1 ELSE 2 END AS DocumentCode,a.CustomerCodeBill CustomerCode,c.CustomerGovName CustomerName
		,CASE d.isPKP WHEN 1 THEN 1 ELSE 0 END AS IsPKP,d.NPWPNo NPWP, d.SKPNo,a.FPJNo  AS FPJNo,a.FPJDate AS FPJDate
		,a.InvoiceDate ReferenceDate,a.FPJSignature AS TaxDate,a.DueDate SubmissionDate,a.TotDppAmt AS DPPAmt,a.TotPPNAmt AS PPNAmt,0 PPNBmAmt
		,'INV-SP-NO: ' + a.InvoiceNo AS Description,TotSalesQty AS Quantity
	FROM 
		SpTrnSFpjHdr a
		LEFT JOIN SpTrnSFpjInfo  d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND a.FPJNo = d.FPJNo
		LEFT JOIN GnMstCoProfile b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
		LEFT JOIN GnMStCustomer  c ON a.CompanyCode = c.CompanyCode AND a.CustomerCodeBill = c.CustomerCode
		LEFT JOIN gnMstCustomerProfitCenter g on a.CompanyCode=g.CompanyCode and a.BranchCode=g.BranchCode and a.CustomerCode=g.CustomerCode
			and g.ProfitCenterCode='300'
	WHERE 
		a.CompanyCode = @CompanyCode 
		AND ((case when @BranchCode='' then a.BranchCode end) <> ''
			or (case when @BranchCode<>'' then a.BranchCode end) = @BranchCode)
		AND month(FpjDate) = @PeriodMonth AND year(FpjDate) = @PeriodYear AND b.ProductType = @ProductType
END


--------------------------------------------------------------------------------------------
-- Get Record 3S and Finance from Transaction
--------------------------------------------------------------------------------------------
-- RETUR SALES --
insert into @t_tax
SELECT 
	a.CompanyCode,a.BranchCode,d.ProductType,@PeriodYear PeriodYear,@PeriodMonth PeriodMonth,f.FakturPajakNo AS TaxNo
	,a.ReturnNo + (case when (select ParaValue from gnMstLookupDtl where CodeID = 'RETUR_BRANCH') = '1' 
					then ' (' + substring(a.BranchCode, len(a.CompanyCode)+1, len(a.BranchCode)) + ')' else '' end) ReferenceNo
	,'100' ProfitCenter,'R' DocumentType,NULL TypeOfGoods,'A' TaxCode,'2' TransactionCode,replace(g.TaxTransCode,'0','') StatusCode,'2' DocumentCode,a.CustomerCode
	,e.CustomerGovName CustomerName,e.IsPKP IsPKP,e.NPWPNo NPWP, e.SKPNo,a.FakturPajakNo FPJNo,a.FakturPajakDate FPJDate
	,a.ReturnDate ReferenceDate,f.FakturPajakDate AS TaxDate,a.ReturnDate SubmissionDate
	,(
		SELECT -1 * ISNULL(SUM(x.Quantity * (ISNULL(x.AfterDiscDPP,0) + ISNULL(x.OthersDPP,0))),0) AS DPPAmt
		FROM omTrSalesReturnDetailModel x
		WHERE x.CompanyCode = a.CompanyCode AND x.BranchCode = a.BranchCode AND x.ReturnNo = a.ReturnNo
	)DPPAmt,
	(
		SELECT -1 * ISNULL(SUM(x.Quantity * (ISNULL(x.AfterDiscPPN,0) + ISNULL(x.OthersPPN,0))),0) AS PPNAmt
		FROM omTrSalesReturnDetailModel x
		WHERE x.CompanyCode = a.CompanyCode AND x.BranchCode = a.BranchCode AND x.ReturnNo = a.ReturnNo
	)PPNAmt,
	(
		SELECT -1 * ISNULL(SUM(x.Quantity * (ISNULL(x.AfterDiscPPnBm,0))),0) AS PPNBmAmt
		FROM omTrSalesReturnDetailModel x
		WHERE x.CompanyCode = a.CompanyCode AND x.BranchCode = a.BranchCode AND x.ReturnNo = a.ReturnNo
	)PPNBmAmt,
	'RTR-SLS-NO: ' + a.ReturnNo AS Description
	,(
		select -1 * ISNULL(sum(x.Quantity), 0)
		from omTrSalesReturnDetailModel x
		where x.CompanyCode = a.CompanyCode AND x.BranchCode = a.BranchCode AND x.ReturnNo = a.ReturnNo
	) Quantity
FROM 
	omTrSalesReturn a 
	LEFT JOIN gnMstCoProfile d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode
	LEFT JOIN gnMstCustomer e ON a.CompanyCode = e.CompanyCode AND a.CustomerCode = e.CustomerCode 
	LEFT JOIN omFakturPajakHdr f ON a.CompanyCode = f.CompanyCode AND a.BranchCode = f.BranchCode AND a.FakturPajakNo = f.FakturPajakNo
	LEFT JOIN gnMstCustomerProfitCenter g on a.CompanyCode=g.CompanyCode and a.BranchCode=g.BranchCode and a.CustomerCode=g.CustomerCode
		and g.ProfitCenterCode='100'
WHERE 
	a.CompanyCode = @CompanyCode 
	AND ((case when @BranchCode='' then a.BranchCode end) <> ''
		or (case when @BranchCode<>'' then a.BranchCode end) = @BranchCode)
	AND Month(a.ReturnDate) = @PeriodMonth 
	AND Year(a.ReturnDate) = @PeriodYear AND a.Status IN (2,5) AND d.ProductType =@ProductType

-- RETUR SPAREPART --
insert into @t_tax
SELECT
	 a.CompanyCode,a.BranchCode,d.ProductType,@PeriodYear AS PeriodYear,@PeriodMonth AS PeriodMonth,b.FPJGovNo AS TaxNo
	,a.ReturnNo ReferenceNo,'300' ProfitCenter,'R' DocumentType,a.TypeOfGoods,'A' TaxCode,'2' TransactionCode,replace(g.TaxTransCode,'0','') StatusCode
	,'2' DocumentCode,a.CustomerCode,c.CustomerGovName CustomerName,a.IsPKP,a.NPWPNo AS NPWP, e.SKPNo,a.FPJNo FPJNo
	,a.FPJDate FPJDate,a.ReturnDate ReferenceDate,b.FPJSignature AS TaxDate,a.ReturnDate SubmissionDate
	,-1 * a.TotDPPAmt DPPAmt,-1 * a.TotPPNAmt PPNAmt,0 PPNBmAmt,'RTR-SP-NO: ' + a.ReturnNo AS Description,
	(
		SELECT SUM(QtyReturn) * -1 
		FROM spTrnSRturDtl x 
		WHERE x.CompanyCode = @CompanyCode AND x.BranchCode = @BranchCode AND x.ReturnNo = a.ReturnNo
	)Quantity 
FROM 
	spTrnSRturHdr a
	LEFT JOIN spTrnSFpjHdr   b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.FPJNo = b.FPJNo
	LEFT JOIN SpTrnSFpjInfo  e ON a.CompanyCode = e.CompanyCode AND a.BranchCode = e.BranchCode AND a.FPJNo = e.FPJNo
	LEFT JOIN gnMstCustomer  c ON a.CompanyCode = c.CompanyCode AND a.CustomerCode = c.CustomerCode
	LEFT JOIN gnMstCoProfile d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode
	LEFT JOIN gnMstCustomerProfitCenter g on a.CompanyCode=g.CompanyCode and a.BranchCode=g.BranchCode and a.CustomerCode=g.CustomerCode
		and g.ProfitCenterCode='300'
WHERE 
	a.CompanyCode = @CompanyCode 
	AND ((case when @BranchCode='' then a.BranchCode end) <> ''
		or (case when @BranchCode<>'' then a.BranchCode end) = @BranchCode)
	AND Month(a.ReturnDate) = @PeriodMonth AND Year(a.ReturnDate) = @PeriodYear
	AND a.Status = 2 AND d.ProductType = @ProductType

-- FPJ SPAREPART --
-- RPLACE TO FIRST CODE --

-- FPJ SALES --
insert into @t_tax
SELECT 
    a.CompanyCode,a.BranchCode,b.ProductType,@PeriodYear AS PeriodYear,@PeriodMonth AS PeriodMonth,a.FakturPajakNo AS TaxNo,
    a.InvoiceNo ReferenceNo,'100' ProfitCenter
	,case when TaxType='Standard' then 'F' else 'S' end DocumentType
	,NULL  TypeOfGoods,'A' TaxCode,'2' TransactionCode,replace(g.TaxTransCode,'0','') StatusCode
    ,CASE TaxType WHEN 'Standard' THEN 1 ELSE 2 END AS DocumentCode,a.CustomerCode,c.CustomerGovName CustomerName
    ,CASE TaxType WHEN 'Standard' THEN 1 ELSE 0 END AS IsPKP,c.NPWPNo NPWP, c.SKPNo,a.FakturPajakNo  AS FPJNo,a.FakturPajakdate AS FPJDate
    ,a.InvoiceDate ReferenceDate,a.FakturPajakDate AS TaxDate,a.DueDate SubmissionDate,(a.DPPAmt-a.DiscAmt) DPPAmt,a.PPNAmt,sum(d.AfterDiscPPnBm) PPNBmAmt
    ,'INV-SLS-NO: ' + a.InvoiceNo AS Description,TotQuantity AS Quantity
FROM 
    omFakturPajakHdr a
    LEFT JOIN GnMstCoProfile b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
    LEFT JOIN GnMStCustomer  c ON a.CompanyCode = c.CompanyCode AND a.CustomerCode = c.CustomerCode
    LEFT JOIN omFakturPajakDetail d ON a.CompanyCode= d.CompanyCode AND a.BranchCode = d.BranchCode
	    AND a.InvoiceNo = d.InvoiceNo
	LEFT JOIN gnMstCustomerProfitCenter g on a.CompanyCode=g.CompanyCode and a.BranchCode=g.BranchCode and a.CustomerCode=g.CustomerCode
		and g.ProfitCenterCode='100'
WHERE 
    a.CompanyCode = @CompanyCode 
	AND ((case when @BranchCode='' then a.BranchCode end) <> ''
		or (case when @BranchCode<>'' then a.BranchCode end) = @BranchCode)
	AND month(FakturPajakDate) = @PeriodMonth AND year(FakturPajakDate) = @PeriodYear AND b.ProductType = @ProductType
GROUP BY
    a.CompanyCode,a.BranchCode,b.ProductType,a.FakturPajakNo,a.InvoiceNo,TaxType,a.CustomerCode
    ,c.CustomerGovName,c.NPWPNo, c.SKPNo,a.FakturPajakNo,a.FakturPajakdate,a.InvoiceDate
    ,a.FakturPajakDate,a.DueDate,(a.DPPAmt-a.DiscAmt),a.PPNAmt,TotQuantity,replace(g.TaxTransCode,'0','')
    
-- RETUR SERVICE --
insert into @t_tax
SELECT
	 a.CompanyCode,a.BranchCode,a.ProductType,@PeriodYear AS PeriodYear,@PeriodMonth AS PeriodMonth,a.FPJGovNo AS TaxNo
	,a.ReturnNo ReferenceNo,'200' ProfitCenter,'R' DocumentType,'' TypeOfGoods,'A' TaxCode,'2' TransactionCode,replace(g.TaxTransCode,'0','') StatusCode
	,'2' DocumentCode,a.CustomerCode,c.CustomerGovName CustomerName,b.IsPKP,e.NPWPNo AS NPWP, e.SKPNo,a.FPJNo  FPJNo
	,a.FPJDate FPJDate,a.ReturnDate ReferenceDate,e.FPJDate AS TaxDate,a.ReturnDate SubmissionDate
	,-1 * b.TotalDppAmt DPPAmt,-1 * b.TotalPpnAmt PPNAmt,0 PPNBmAmt,'RTR-SV-NO: ' + a.ReturnNo AS Description
	,-1 Quantity
FROM 
	SvTrnReturn a
	LEFT JOIN svTrnInvoice   b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.FPJNo = b.FPJNo
	LEFT JOIN gnMstCustomer  c ON a.CompanyCode = c.CompanyCode AND a.CustomerCode = c.CustomerCode
	LEFT JOIN gnMstCoProfile d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode
	LEFT JOIN SvTrnFakturPajak  e ON a.CompanyCode = e.CompanyCode AND a.BranchCode = e.BranchCode AND a.FPJNo = e.FPJNo
	LEFT JOIN gnMstCustomerProfitCenter g on a.CompanyCode=g.CompanyCode and a.BranchCode=g.BranchCode and a.CustomerCode=g.CustomerCode
		and g.ProfitCenterCode='200'

WHERE 
	a.CompanyCode = @CompanyCode 
	AND ((case when @BranchCode='' then a.BranchCode end) <> ''
		or (case when @BranchCode<>'' then a.BranchCode end) = @BranchCode)
	AND Month(a.ReturnDate) = @PeriodMonth AND Year(a.ReturnDate) = @PeriodYear
	AND b.InvoiceStatus = 4 AND d.ProductType = @ProductType


-- FPJ SERVICE --
insert into @t_tax
SELECT 
	a.CompanyCode, a.BranchCode, a.ProductType, @PeriodYear AS PeriodYear, @PeriodMonth AS PeriodMonth, d.FpjGovNo AS TaxNo
	,a.InvoiceNo ReferenceNo,'200' ProfitCenter 
	,case when d.isPKP = 1 then 'F' else 'S' end DocumentType
	, NULL TypeOfGoods, 'A' TaxCode, '2' TransactionCode,replace(g.TaxTransCode,'0','') StatusCode
	,CASE d.isPKP WHEN 1 THEN 1 ELSE 2 END AS DocumentCode,d.CustomerCodeBill CustomerCode,c.CustomerGovName CustomerName
	,CASE d.isPKP WHEN 1 THEN 1 ELSE 0 END AS IsPKP,d.NPWPNo NPWP, d.SKPNo,a.FPJNo  AS FPJNo,d.FPJDate AS FPJDate
	,a.InvoiceDate ReferenceDate,d.SignedDate AS TaxDate,d.DueDate SubmissionDate,a.TotalDppAmt AS DPPAmt,a.TotalPPNAmt AS PPNAmt
	,0 PPNBmAmt,'INV-SV-NO: ' + a.InvoiceNo AS Description,1 Quantity
FROM 
	SvTrnInvoice a
	LEFT JOIN SvTrnFakturPajak d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND a.FPJNo = d.FPJNo
	--LEFT JOIN GnMstCoProfile b   ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
	LEFT JOIN GnMStCustomer  c   ON a.CompanyCode = c.CompanyCode AND d.CustomerCodeBill = c.CustomerCode
	LEFT JOIN gnMstCustomerProfitCenter g on a.CompanyCode=g.CompanyCode and a.BranchCode=g.BranchCode and d.CustomerCodeBill=g.CustomerCode
WHERE
	a.CompanyCode = @CompanyCode 
	AND ((case when @BranchCode='' then a.BranchCode end) <> ''
		or (case when @BranchCode<>'' then a.BranchCode end) = @BranchCode) 
	AND month(d.FpjDate) = @PeriodMonth AND year(d.FpjDate) = @PeriodYear 
	AND a.ProductType = @ProductType
	and g.ProfitCenterCode='200'
-- Faktur Pajak Gabungan Holding (SERVICE)
union
SELECT 
	a.CompanyCode, a.BranchCode
	, a.ProductType
	, @PeriodYear AS PeriodYear, @PeriodMonth AS PeriodMonth, d.FpjGovNo AS TaxNo
	,a.InvoiceNo ReferenceNo,'200' ProfitCenter ,'F' DocumentType, NULL TypeOfGoods, 'A' TaxCode, '2' TransactionCode
	,replace(g.TaxTransCode,'0','') StatusCode
	,CASE d.isPKP WHEN 1 THEN 1 ELSE 2 END AS DocumentCode,d.CustomerCodeBill CustomerCode
	,c.CustomerGovName CustomerName
	,CASE d.isPKP WHEN 1 THEN 1 ELSE 0 END AS IsPKP,d.NPWPNo NPWP, d.SKPNo,a.FPJNo  AS FPJNo,d.FPJDate AS FPJDate
	,a.InvoiceDate ReferenceDate,d.SignedDate AS TaxDate,d.DueDate SubmissionDate,a.TotalDppAmt AS DPPAmt,a.TotalPPNAmt AS PPNAmt
	,0 PPNBmAmt,'INV-SV-NO: ' + a.InvoiceNo AS Description,1 Quantity
FROM 
	SvTrnInvoice a
	LEFT JOIN SvTrnFakturPajak d ON a.CompanyCode = d.CompanyCode AND a.FPJNo = d.FPJNo
	--LEFT JOIN GnMstCoProfile b   ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
	LEFT JOIN GnMStCustomer  c   ON a.CompanyCode = c.CompanyCode AND d.CustomerCodeBill = c.CustomerCode
	LEFT JOIN gnMstCustomerProfitCenter g on a.CompanyCode=g.CompanyCode and a.BranchCode=g.BranchCode and d.CustomerCodeBill=g.CustomerCode
		and g.ProfitCenterCode='200'
WHERE 1=1
	and a.CompanyCode = @CompanyCode 
	AND ((case when @BranchCode='' then a.BranchCode end) <> ''
		or (case when @BranchCode<>'' then a.BranchCode end) = @BranchCode) 
	AND month(d.FpjDate) = @PeriodMonth AND year(d.FpjDate) = @PeriodYear 
	AND a.ProductType = @ProductType
	and d.FpjGovNo <> '' and (a.InvoiceNo like 'INF%' or a.InvoiceNo like 'INW%' or a.InvoiceNo like 'INI%') -- penambahan invoice INI
	and a.IsLocked=1

-- FPJ FINANCE --
insert into @t_tax
SELECT 
	a.CompanyCode,a.BranchCode,b.ProductType,@PeriodYear AS PeriodYear,@PeriodMonth AS PeriodMonth,a.FakturPajakNo AS TaxNo
	,a.InvoiceNo ReferenceNo,'000' ProfitCenter
	,case when a.TaxType='Standard' then 'F' else 'S' end DocumentType
	,NULL TypeOfGoods,'A' TaxCode,'2' TransactionCode
	,replace(g.TaxTransCode,'0','') StatusCode,CASE a.TaxType WHEN 'Standard' THEN 1 ELSE 2 END AS DocumentCode,a.CustomerCode,c.CustomerGovName CustomerName
	,CASE a.TaxType WHEN 'Standard' THEN 1 ELSE 0 END AS IsPKP,c.NPWPNo NPWP, c.SKPNo,a.FakturPajakNo  AS FPJNo
	,a.FakturPajakDate AS FPJDate,a.InvoiceDate ReferenceDate,a.FakturPajakDate AS TaxDate,a.DueDate SubmissionDate
	,(a.DPPAmt-a.DiscAmt) DPPAmt,a.PPNAmt,0 PPNBmAmt,'INV-AR-NO: ' + a.InvoiceNo AS Description,1 Quantity
FROM 
	ArFakturPajakHdr a
	LEFT JOIN GnMstCoProfile b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode
	LEFT JOIN GnMStCustomer  c ON a.CompanyCode = c.CompanyCode AND a.CustomerCode = c.CustomerCode
	LEFT JOIN gnMstCustomerProfitCenter g on a.CompanyCode=g.CompanyCode and a.BranchCode=g.BranchCode and a.CustomerCode=g.CustomerCode
		and g.ProfitCenterCode='000'
WHERE 
	a.CompanyCode = @CompanyCode 
	AND ((case when @BranchCode='' then a.BranchCode end) <> ''
		or (case when @BranchCode<>'' then a.BranchCode end) = @BranchCode)
	AND month(FakturPajakDate) = @PeriodMonth AND year(FakturPajakDate) = @PeriodYear AND ProductType = @ProductType

SELECT * INTO #t1
FROM(
	SELECT row_number() over(order by TaxNo,ProfitCenter,ReferenceNo ASC) + @LastSeqNo SeqNo,* FROM @t_tax a
	WHERE 
    a.TaxNo <> '' 
	and NOT exists 
	(
		SELECT 1 
		FROM GnTaxOut 
		WHERE CompanyCode = a.CompanyCode 
			AND BranchCode=a.BranchCode
			AND ProductType = a.ProductType AND PeriodMonth = a.PeriodMonth AND PeriodYear = a.PeriodYear
			AND TaxNo=a.TaxNo AND CustomerCode=a.CustomerCode and ReferenceNo=a.ReferenceNo
	)
	AND NOT exists
	(
		SELECT 1
		FROM GnTaxOutHistory	
		where CompanyCode = a.CompanyCode		
			AND BranchCode=a.BranchCode
			AND ProductType = a.ProductType AND PeriodMonth = a.PeriodMonth AND PeriodYear = a.PeriodYear
			AND TaxNo=a.TaxNo AND CustomerCode=a.CustomerCode and ReferenceNo=a.ReferenceNo 
			AND IsDeleted = '1'
	)
) #t1

INSERT INTO gnTaxOut
SELECT CompanyCode, BranchCode, ProductType, PeriodYear, PeriodMonth, SeqNo, ProfitCenter, 
    TypeOfGoods, TaxCode, TransactionCode, StatusCode, DocumentCode, DocumentType, CustomerCode, CustomerName, 
    IsPKP, NPWP, FPJNo, FPJDate, ReferenceNo, ReferenceDate, TaxNo, TaxDate, SubmissionDate, DPPAmt, PPNAmt, 
    PPNBmAmt, Description, Quantity, 0 IsLocked, null LockingBy, null LockingDate, @UserId CreatedBy, 
    GETDATE() CreatedDate, @UserId LastupdateBy, GETDATE() LastupdateDate,SKPNo
FROM #t1
drop table #t1
