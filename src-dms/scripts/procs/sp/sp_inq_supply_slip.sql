

CREATE procedure [dbo].[uspfn_sp_inquiry_supply_slip]
	@companycode varchar(15), 
	@branchcode varchar(15), 
	@OrderBy varchar(50),
	@CustomParameter varchar(400),
	@RangeDate varchar(400) 
 AS
	SET NOCOUNT ON;

	DECLARE @SQLQuery NVARCHAR(MAX)
	
	SET @SQLQuery = '
SELECT row_number() over(order by ' + @OrderBy + ') Nomor, CONVERT(BIT, v.finish) FinishCon, * FROM (
SELECT DISTINCT (a.DocNo) SSNo,
(SELECT LookUpValueName FROM GnMstLookUpDtl where CompanyCode = a.CompanyCode AND CodeID = ''TTSR'' AND lookUpValue = a.TransType) Type,
a.DocDate SSDate,
a.UsageDocNo SPKNo,
a.UsageDocDate SPKDate,
ISNULL(b.PickingSlipNo, ''-'') PickingNo,
b.PickingSlipDate PickingDate,
ISNULL(b.LmpNo, ''-'') LmpNo,
LmpDate,
ISNULL(b.SP, ''-'') SP,
CASE ISNULL((SELECT TOP 1 SupplySlipNo FROM SvTrnInvItemDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND SupplySlipNo = a.DocNo),'''') WHEN '''' THEN 0 ELSE 1 END AS finish,
ISNULL(c.PoliceRegNo,''-'') PoliceNo,
(SELECT x.CustomerName FROM gnMstCustomer x WHERE x.CompanyCode = a.CompanyCode AND x.CustomerCode = a.CustomerCodeBill) + ''('' + a.CustomerCodeBill  + '')'' AS CustomerName,
ISNULL(e.PartsGrossAmtTot,0) PartsGrossAmtTot, 
ISNULL(e.PartsDiscAmtTot,0) PartsDiscAmtTot, 
ISNULL(e.PartsDPPAmtTot,0) PartsDPPAmtTot
FROM spTrnSORDHdr a 
LEFT JOIN
(
	SELECT a.CompanyCode, a.BranchCode, a.PickingSlipNo, b.PickingSlipDate,a.DocNo, b.Status SP, 
	c.BPSFNo,c.BPSFDate,d.LmpNo, d.LmpDate,c.Status SI,ISNULL(d.Status ,''X'') SL FROM spTrnSPickingDtl a 
	INNER JOIN spTrnSPickingHDr b	ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PickingSlipNo = b.PickingSlipNo
	LEFT  JOIN spTrnSBPSFHdr c		ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.PickingSlipNo = c.PickingSlipNo
	LEFT  JOIN spTrnSlmpHdr  d		ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND c.BPSFNo = d.BPSFNo
	WHERE a.PickingSlipNo IN
	(SELECT DISTINCT(PickingSlipNo) FROM spTrnSPickingDtl WHERE DocNo IN 
	(SELECT DocNo FROM SpTrnSORDHdr WHERE SalesType = 2))
) b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.DocNo = b.DocNo
LEFT JOIN svTrnService c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.UsageDocNo = c.JobOrderNo
LEFT JOIN svTrnInvoice d ON d.CompanyCode = c.CompanyCode AND d.BranchCode = c.BranchCode AND d.JobOrderNo = c.JobOrderNo
LEFT JOIN 
(
	SELECT * FROM 
	(
		SELECT DISTINCT(a.SupplySlipNo), 
			a.CompanyCode,
			a.BranchCode,
			a.InvoiceNo,
			b.PartsGrossAmtTot,
			b.PartsDiscAmtTot,
			b.PartsDPPAmtTot
		FROM SvTrnInvItemDtl a
		LEFT JOIN
		(
			SELECT CompanyCode, BranchCode, InvoiceNo,
			ISNULL(SUM(PartsGrossAmt),0) PartsGrossAmtTot,
			ISNULL(SUM(PartsDiscAmt),0) PartsDiscAmtTot,
			ISNULL(SUM(PartsDPPAmt),0) PartsDPPAmtTot
			FROM svTrnInvoice WHERE CompanyCode = ''' + @CompanyCode + ''' AND BranchCode = ''' + @BranchCode + '''
			GROUP BY InvoiceNo, CompanyCode, BranchCode
		) b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.InvoiceNo = b.InvoiceNo 
		WHERE a.CompanyCode = ''' + @CompanyCode + ''' AND a.BranchCode = ''' + @BranchCode + '''
	) x
) e ON a.CompanyCode = e.CompanyCode AND a.BranchCode = e.BranchCode AND a.DocNo = e.SupplySlipNo
WHERE 
a.CompanyCode = ''' + @CompanyCode + ''' AND a.BranchCode = ''' + @BranchCode + ''' AND a.SalesType = 2 
    ' + @CustomParameter + ' 
    ' + @RangeDate 

EXECUTE sp_executesql @SQLQuery	

GO

CREATE procedure [dbo].[uspfn_sp_inquiry_supply_slip_sum]
	@companycode varchar(15), 
	@branchcode varchar(15), 
	@CustomParameter varchar(400),
	@RangeDate varchar(400) 
 AS
	SET NOCOUNT ON;

	DECLARE @SQLQuery NVARCHAR(MAX)
	
	SET @SQLQuery = '
SELECT 
	ISNULL(SUM(GrossAmt),0) PartsGrossAmt, 
	ISNULL(SUM(DPPAmt),0) PartsDPPAmt,
	ISNULL(SUM(DiscAmt),0) PartsDiscAmt 
FROM (
SELECT SSNo, PartNo, ISNULL(SalesAmt,0) GrossAmt, ISNULL(NetSalesAmt,0) DPPAmt, ISNULL(DiscAmt,0) DiscAmt
FROM (
SELECT 
DISTINCT(a.docNo) SSNo,
x.PartNo,
c.JobOrderNo SPKno,
a.DocDate SSDate,
b.PickingSlipDate PickingDate,ISNULL(b.SP, ''-'') SP,
y.SalesAmt,
y.DiscAmt,
y.NetSalesAmt,
(SELECT CustomerName FROM gnMstCustomer WHERE CustomerCode = a.CustomerCodeBill) + ''('' + a.CustomerCodeBill  + '')'' AS CustomerName
FROM spTrnSORDHdr a
LEFT JOIN  spTrnSORDDtl x ON a.CompanyCode = x.CompanyCode AND a.BranchCode = x.BranchCode AND a.DocNo = x.DocNo
LEFT JOIN  spTrnSLmpDtl y ON a.CompanyCode = y.CompanyCode AND a.BranchCode = y.BranchCOde AND x.DocNo = y.DocNo AND x.PartNo = y.PartNo 
LEFT JOIN
	(SELECT a.CompanyCode, a.BranchCode, a.PickingSlipNo, b.PickingSlipDate,a.DocNo, b.Status SP, 
		c.BPSFNo,c.BPSFDate, d.LmpNo, d.LmpDate,c.Status SI,ISNULL(d.Status ,''X'') SL FROM spTrnSPickingDtl a 
		INNER JOIN spTrnSPickingHDr b	ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.PickingSlipNo = b.PickingSlipNo
		LEFT  JOIN spTrnSBPSFHdr c		ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.PickingSlipNo = c.PickingSlipNo
		LEFT  JOIN spTrnSlmpHdr  d		ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND c.BPSFNo = d.BPSFNo
			WHERE a.PickingSlipNo IN
				(SELECT DISTINCT(PickingSlipNo) FROM spTrnSPickingDtl WHERE DocNo IN 
					(SELECT DocNo FROM SpTrnSORDHdr WHERE SalesType = 2))
	) b ON a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.DocNo = b.DocNo
LEFT JOIN svTrnService c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.UsageDocNo = c.JobOrderNo
WHERE 
a.CompanyCode = ''' + @companycode  + ''' AND a.BranchCode = ''' + @branchcode  + ''' AND a.SalesType = 2 
    ' + @CustomParameter + ' 
    ' + @RangeDate + ' ) p'

EXECUTE sp_executesql @SQLQuery	

GO



CREATE procedure [dbo].[uspfn_sp_inquiry_supply_slip_detail]
	@companycode varchar(15), 
	@branchcode varchar(15), 
	@JobOrderNo varchar(20),
	@DocNo varchar(20)
 AS
SELECT a.JobOrderNo, a.ProductType, x.PartNo,
(SELECT s.PartName FROM SpMstItemInfo s where s.CompanyCode = '6159401' AND s.PartNo = x.PartNo) PartName,
x.DemandQty, x.SupplyQty, x.ReturnQty, ISNULL(x.CostPrice,0) CostPrice, ISNULL(x.RetailPrice,0) RetailPrice,
ISNULL(b.InvoiceNo,'-') InvoiceNo,
(SELECT InvoiceDate FROM SvTrnInvoice WHERE CompanyCode = d.CompanyCode 
AND BranchCode = d.BranchCode AND InvoiceNo = b.InvoiceNo) InvoiceDate
FROM SvTrnService a
LEFT JOIN 
(
SELECT CompanyCode, BranchCode, ServiceNo, PartNo, SUM(DemandQty) AS DemandQty, SUM(SupplyQty) AS SupplyQty,
SUM(ReturnQty) AS ReturnQty, CostPrice, RetailPrice
FROM SvTrnSrvItem GROUP BY ServiceNo, PartNo, CompanyCode, BranchCode, CostPrice, RetailPrice
) x ON a.CompanyCode = x.CompanyCode AND a.BranchCode = x.BranchCode AND a.ServiceNo = x.ServiceNo
LEFT  JOIN SpTrnSORDHdr c ON a.CompanyCode = c.CompanyCode AND a.BranchCode = c.BranchCode AND a.JobOrderNo = c.UsageDocNo
INNER JOIN SpTrnSLmpDtl d ON a.CompanyCode = d.CompanyCode AND a.BranchCode = d.BranchCode AND c.DocNo = d.DocNo AND d.PartNo = x.PartNo
LEFT JOIN SvTrnInvItemDtl b ON d.CompanyCode = b.CompanyCode AND d.BranchCode = b.BranchCode AND d.DocNo = b.SupplySlipNo AND d.PartNo = b.Partno
WHERE
a.CompanyCode =  @CompanyCode AND
a.BranchCode  = @BranchCode   AND 
a.JobOrderNo = @JobOrderNo    AND
c.DocNo = @DocNo
GO

CREATE procedure [dbo].[uspfn_sp_inquiry_supply_slip_detail_sum]
	@companycode varchar(15), 
	@branchcode varchar(15), 
	@DocNo varchar(20)
 AS
SELECT 
	ISNULL(SUM(SalesAmt),0) SalesAmt, 
	ISNULL(SUM(DiscAmt),0) DiscAmt, 
	ISNULL(SUM(NetSalesAmt),0) DPPAmt 
FROM spTrnSLmpDtl 
WHERE 
	CompanyCode = @CompanyCode AND 
	BranchCode = @BranchCode AND 
	DocNo = @DocNo
GO
