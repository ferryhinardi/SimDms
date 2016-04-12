
GO

/****** Object:  StoredProcedure [dbo].[usprpt_OmFakturPajak]    Script Date: 6/30/2015 9:13:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER procedure [dbo].[usprpt_OmFakturPajak]
--DECLARE
	@CompanyCode	VARCHAR(15),
	@BranchCode	VARCHAR(15),
	@FPJDateStart DATETIME,
	@FPJDateEnd DATETIME,
	@FPJNoStart	VARCHAR(30),
	@FPJNoEnd	VARCHAR(30),
	@SignName VARCHAR(100),
	@TitleSign VARCHAR(100),
	@Param bit = 1
AS
BEGIN

--'6114201','611420100','20150501','20150530','010.001-15.70827239','010.001-15.70827239','Wiwik W','Pimpinan FAD',True

--SELECT @CompanyCode = '6114201',@BranchCode='611420100',@FPJDateStart='20150501',@FPJDateEnd='20150630',@FPJNoStart='010.001-15.70827992',
--@FPJNoEnd='010.001-15.70827992',@SignName='Wiwik W',@TitleSign='Pimpinan FAD',@Param=1

	-- Setting Header Faktur Pajak --
	---------------------------------
	declare @fCompName	varchar(max)
	declare @fAdd		varchar(max)
	declare @fAdd1		varchar(max)
	declare @fAdd2		varchar(max)
	declare @fNPWP		varchar(max)
	declare @fSKP		varchar(max)
	declare @fSKPDate	varchar(max)
	declare @fCity		varchar(max)
	declare @fInv		int
	
	declare @fStatus varchar(1)
	set @fStatus = 0
	
	declare @fInfoPKP varchar(1)
	set @fInfoPKP = 1
	
	if exists (select 1 from gnMstLookUpDtl where codeid='FPJFLAG')
	begin
		set @fStatus = (select paravalue from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='STATUS')
	end
	
	if exists (select * from gnMstLookUpHdr where codeid='FPJ_INFO_PKP')
	begin
		set @fInfoPKP = (select LookupValue from gnmstlookupdtl where codeid='FPJ_INFO_PKP')
	end
	
	if (@fStatus = '1')
	begin
		set @fCompName	= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='NAME')
		set @fAdd1		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='ADD1')
		set @fAdd2		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='ADD2')
		set @fAdd		= @fAdd1+' '+@fAdd2
		set @fNPWP		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='NPWP')
		set @fSKP		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='SKPNO')
		set @fSKPDate	= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='SKPDATE')
		set @fCity		= (select isnull(lookupvaluename,'') from gnmstlookupdtl where codeid='FPJINFO' and lookupvalue='CITY')		
	end
set @fInv		= (select isnull(ParaValue,'1') from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='SALES')		
-- parameter use address holding or not
declare @IsHoldingAddr as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR') > 0
	set @IsHoldingAddr = (select convert(numeric,LookUpValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR')
else
	set @IsHoldingAddr = 0

-- parameter to show info or not
declare @IsShowInfo as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPIF' and LookUpValue='STATUS') > 0
	set @IsShowInfo = (select convert(numeric,ParaValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPIF' and LookUpValue='STATUS')
else
	set @IsShowInfo = 1
	
	
	declare @tabData as table
	(
		CompanyCode varchar(max),
		BranchCode varchar(max),
		InvoiceNo varchar(max),
		ItemOrder varchar(max),
		ItemCode varchar(max),
		ItemName varchar(max),
		PPnBM decimal(18,2),
		PPnBMSell decimal(18,2),
		ItemQuantity decimal(5,2),
		ItemDPP decimal(18,2),
		Potongan decimal(18,2),
		TaxPct decimal(18,2),
		AfterDiscPpn  decimal(18,2)
	)
	IF (@Param=1)
	BEGIN
		-- Sembunyikan Detail Part .....
		SELECT * INTO #t1 FROM (
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, acc.PartNo AS ItemCode
			, ISNULL((acc.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=acc.companycode 
						and branchcode=acc.branchcode and invoiceno=acc.invoiceno) * acc.RetailPrice,0) AS ItemDPP
			, ISNULL((acc.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=acc.companycode 
						and branchcode=acc.branchcode and invoiceno=acc.invoiceno) * acc.DiscExcludePPn, 0) AS Potongan
			, 0 AS TaxPct
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	)#t1
				
		select * into #Others from(
				Select a.CompanyCode
					 , a.BranchCode
					 , a.InvoiceNo
					 , a.SalesModelCode
					 , ISNULL((a.Quantity)/(select count(chassisno) from omFakturPajakDetail where companycode=a.companycode 
						and branchcode=a.branchcode and invoiceno=a.invoiceno) * a.DPP, 0) ItemDPP
					 , 0 Potongan
				from omFakturPajakDetailOthers a
				left join omFakturPajakDetail b on a.CompanyCode = b.CompanyCode
					  and a.BranchCode = b.BranchCode
					  and a.InvoiceNo = b.InvoiceNo
					  and a.SalesModelCode = b.SalesModelCode
				INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = a.CompanyCode
					   AND hdr.BranchCode = a.BranchCode
					   AND hdr.InvoiceNo = a.InvoiceNo
					   AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
					   	   OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
					   AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
					   AND hdr.TaxType = 'Standard'
				where a.CompanyCode = @CompanyCode
				  and a.BranchCode = @BranchCode
		)#Others
		
		--SELECT * INTO #t2 FROM (
		--SELECT
		--	CompanyCode 
		--	, BranchCode
		--	, InvoiceNo 
		--	, SUM(ItemDPP) ItemDPP
		--	, SUM(Potongan) Potongan
		--FROM #t1 GROUP BY CompanyCode, BranchCode, InvoiceNo ) #t2
		
		SELECT CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
		INTO #t2 FROM (
			SELECT
				CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
			FROM #t1 GROUP BY CompanyCode, BranchCode, InvoiceNo
		UNION
			SELECT
				CompanyCode 
				, BranchCode
				, InvoiceNo 
				, SUM(ItemDPP) ItemDPP
				, SUM(Potongan) Potongan
			FROM #Others a
			GROUP BY a.CompanyCode, a.BranchCode, a.InvoiceNo 
		) #t2
		group by CompanyCode,BranchCode,InvoiceNo
		
		-- INSER DATA KE TABEL RESULT
		INSERT INTO @tabData 
		-- Unit
		SELECT 
			mdl.CompanyCode
			, mdl.BranchCode
			, mdl.InvoiceNo
			, '1' AS ItemOrder
			, mdl.SalesModelCode AS ItemCode
			, LEFT(CONVERT(VARCHAR, mdl.ChassisNo) + '             ', 13) +  LEFT(CONVERT(VARCHAR, mdl.EngineNo) + '           ', 11) AS ItemName
			, ISNULL(Vec.PPnBMBuyPaid,0) AS PPnBM
			, mdl.AfterDiscPPnBM AS PPnBMSell
			, 1 AS ItemQuantity
			, mdl.BeforeDiscDPP + (ISNULL(t.ItemDPP,0)) AS ItemDPP
			, mdl.DiscExcludePPN + (ISNULL(t.Potongan,0)) AS Potongan
			, ISNULL((SELECT ISNULL(TaxPct, 0) FROM  GnMstTax INNER JOIN OmMstModel ON OmMstModel.CompanyCode = GnMstTax.CompanyCode AND OmMstModel.PPnBMCodeSell = GnMstTax.TaxCode AND OmMstModel.SalesModelCode = mdl.SalesModelCode), 0) AS TaxPct
			, mdl.AfterDiscPpn		
		FROM 
			omFakturPajakDetail mdl 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = mdl.CompanyCode
			AND hdr.BranchCode = mdl.BranchCode
			AND hdr.InvoiceNo = mdl.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		LEFT JOIN OmMstVehicle vec ON mdl.CompanyCode = vec.CompanyCode 
			AND mdl.ChassisCode = vec.ChassisCode 
			AND mdl.ChassisNo = vec.ChassisNo
		LEFT JOIN #t2 t ON t.CompanyCode = mdl.CompanyCode
			AND t.BranchCode = mdl.BranchCode
			AND t.InvoiceNo = mdl.InvoiceNo
		WHERE 
			mdl.CompanyCode = @CompanyCode AND mdl.BranchCode = @BranchCode 
		UNION ALL
		SELECT distinct
			hdr.CompanyCode
			, hdr.BranchCode
			, hdr.InvoiceNo
			, '2' AS ItemOrder
			, 'SPAREPART/MATERIAL' AS ItemCode
			, '' AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM omFakturPajakDetailAccsSeq acc
		INNER JOIN omFakturPajakHdr hdr ON hdr.CompanyCode=acc.CompanyCode
			AND hdr.BranchCode=acc.BranchCode
			AND hdr.InvoiceNo=acc.InvoiceNo
		WHERE hdr.CompanyCode = @CompanyCode
			AND hdr.BranchCode = @BranchCode
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		UNION ALL
		-- SPAREPART/MATERIAL
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, '3' AS ItemOrder
			, acc.PartNo AS ItemCode
			, acc.PartName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	
		UNION ALL
		SELECT 
			oth.CompanyCode
			, oth.BranchCode
			, oth.InvoiceNo
			, '4' AS ItemOrder
			, oth.OtherCode AS ItemCode
			, oth.OtherName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailOthers oth 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = oth.CompanyCode
			AND hdr.BranchCode = oth.BranchCode
			AND hdr.InvoiceNo = oth.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			oth.CompanyCode = @CompanyCode AND oth.BranchCode = @BranchCode	
							 
		DROP TABLE #t1
		DROP TABLE #t2			
	END
	ELSE
	BEGIN
		-- Tampilkan Part --
		--------------------
		
		-- INSER DATA KE TABEL RESULT
		INSERT INTO @tabData 
		-- Unit
		SELECT 
			mdl.CompanyCode
			, mdl.BranchCode
			, mdl.InvoiceNo
			, '1' AS ItemOrder
			, mdl.SalesModelCode AS ItemCode
			, LEFT(CONVERT(VARCHAR, mdl.ChassisNo) + '             ', 13) +  LEFT(CONVERT(VARCHAR, mdl.EngineNo) + '           ', 11) AS ItemName
			, ISNULL(Vec.PPnBMBuyPaid,0) AS PPnBM
			, mdl.AfterDiscPPnBM AS PPnBMSell
			, 1 AS ItemQuantity
			, mdl.BeforeDiscDPP AS ItemDPP
			, mdl.DiscExcludePPN Potongan
			, ISNULL((SELECT ISNULL(TaxPct, 0) FROM  GnMstTax INNER JOIN OmMstModel ON OmMstModel.CompanyCode = GnMstTax.CompanyCode AND OmMstModel.PPnBMCodeSell = GnMstTax.TaxCode AND OmMstModel.SalesModelCode = mdl.SalesModelCode), 0) AS TaxPct
			, mdl.AfterDiscPpn
		FROM 
			omFakturPajakDetail mdl 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = mdl.CompanyCode
			AND hdr.BranchCode = mdl.BranchCode
			AND hdr.InvoiceNo = mdl.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		LEFT JOIN OmMstVehicle vec ON mdl.CompanyCode = vec.CompanyCode 
			AND mdl.ChassisCode = vec.ChassisCode 
			AND mdl.ChassisNo = vec.ChassisNo
		WHERE 
			mdl.CompanyCode = @CompanyCode AND mdl.BranchCode = @BranchCode 
		UNION ALL
		SELECT distinct
			hdr.CompanyCode
			, hdr.BranchCode
			, hdr.InvoiceNo
			, '2' AS ItemOrder
			, 'SPAREPART/MATERIAL' AS ItemCode
			, '' AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, 0 AS ItemQuantity
			, 0 AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn
		FROM omFakturPajakDetailAccsSeq acc
		INNER JOIN omFakturPajakHdr hdr ON hdr.CompanyCode=acc.CompanyCode
			AND hdr.BranchCode=acc.BranchCode
			AND hdr.InvoiceNo=acc.InvoiceNo
		WHERE hdr.CompanyCode = @CompanyCode
			AND hdr.BranchCode = @BranchCode
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		UNION ALL
		-- SPAREPART/MATERIAL
		SELECT 
			acc.CompanyCode
			, acc.BranchCode
			, acc.InvoiceNo
			, '3' AS ItemOrder
			, acc.PartNo AS ItemCode
			, acc.PartName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, acc.Quantity AS ItemQuantity
			, (acc.Quantity * acc.RetailPrice) AS ItemDPP
			, (acc.Quantity * acc.DiscExcludePPn) AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailAccsSeq acc 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = acc.CompanyCode
			AND hdr.BranchCode = acc.BranchCode
			AND hdr.InvoiceNo = acc.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			acc.CompanyCode = @CompanyCode AND acc.BranchCode = @BranchCode	
		UNION ALL
		SELECT 
			oth.CompanyCode
			, oth.BranchCode
			, oth.InvoiceNo
			, '4' AS ItemOrder
			, oth.OtherCode AS ItemCode
			, oth.OtherName AS ItemName
			, 0 AS PPnBM
			, 0 AS PPnBMSell
			, oth.Quantity AS ItemQuantity
			, (oth.Quantity * oth.DPP) AS ItemDPP
			, 0 AS Potongan
			, 0 AS TaxPct
			, 0 AS AfterDiscPpn			
		FROM 
			omFakturPajakDetailOthers oth 
		INNER JOIN omFakturPajakhdr hdr ON hdr.CompanyCode = oth.CompanyCode
			AND hdr.BranchCode = oth.BranchCode
			AND hdr.InvoiceNo = oth.InvoiceNo
			AND ((CASE WHEN @FPJNoStart = '' THEN hdr.FakturPajakNo END) <> ''
				OR (CASE WHEN @FPJNoStart <> '' THEN hdr.FakturPajakNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
			AND CONVERT(VARCHAR, hdr.FakturPajakDate, 112) BETWEEN CONVERT(VARCHAR, @FPJDateStart, 112) AND CONVERT(VARCHAR, @FPJDateEnd, 112)
			AND hdr.TaxType = 'Standard'
		WHERE 
			oth.CompanyCode = @CompanyCode AND oth.BranchCode = @BranchCode	
	END

SELECT * INTO #hasil FROM (
SELECT
	a.TaxType AS TaxType
	,a.InvoiceNo AS InvoiceNo
	,a.InvoiceDate AS InvoiceDate
	,a.FakturPajakNo AS FPJNo
	,(SELECT dbo.GetDateIndonesian (CONVERT(VARCHAR,a.FakturPajakDate, 101))) AS FPJDate
	,a.CustomerCode AS fakturCustCode
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fCompName else e.CompanyGovName end)
	 else '' end AS CompanyName
	,case @fStatus when '1' then @fSKP else e.SKPNo end AS compSKPNo
	,case @fStatus when '1' then @fSKPDate else e.SKPDate end AS compSKPDate
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fAdd else 
			(case when @IsHoldingAddr=0 then ISNULL(e.Address1,'') + ' ' + ISNULL(e.Address2,'') + ' ' + ISNULL(e.Address3,'') + ' ' + ISNULL(e.Address4,'')
				else (select ISNULL(Address1,'') + ' ' + ISNULL(Address2,'') + ' ' + ISNULL(Address3,'') + ' ' + ISNULL(Address4,'') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))end ) end)
		 else '' end AS compAddr	 
	,e.PhoneNo AS compPhoneNo
	,e.FaxNo AS compFaxNo
	,case when @fInfoPKP = 1 then
		(case @fStatus when '1' then @fNPWP else e.NPWPNo end)
	 else '' end AS compNPWPNo
	,d.CustomerGovName AS CustomerName
	,d.SKPNo AS custSKPNo
	,d.SKPDate AS custSKPDate
	,ISNULL(d.Address1,'') AS custAddr1
	,ISNULL(d.Address2,'') AS custAddr2
	,ISNULL(d.Address3,'')+ ' ' + ISNULL(d.Address4,'') AS custAddr3
	,d.PhoneNo AS custPhoneNo
	,d.FaxNo AS custFaxNo
	,d.NPWPNo AS custNPWPNo
	,a.DueDate AS fakturDueDate
	,a.DiscAmt AS DiscAmt
	,a.DppAmt AS DppAmt
	,a.PPNAmt AS PPNAmt
	,a.DppAmt - a.DiscAmt AS SubAmt
	,a.TotalAmt AS TotalAmt
	,a.TotQuantity
	,a.PPnBMPaid
	,case @fStatus when '1' then @fCity else 
		(SELECT LookUpValueName FROM gnMstLookupDtl WHERE CodeID = 'CITY' AND LookUpValue = e.CityCode) end as cityNm
	,ISNULL(@SignName, '') AS TaxPerson
	,ISNULL(@TitleSign,'') AS JobTitle
	,'Model              No.Rangka    No.Mesin            PPnBM' AS ItemHeader
	,dtl.ItemOrder
	,dtl.ItemCode
	,dtl.ItemName
	,dtl.PPnBM
	,dtl.ItemQuantity
	,dtl.ItemDPP
	,dtl.Potongan
	,dtl.AfterDiscPpn
	,CASE WHEN copro.ProductType = '2W' THEN 
		(CASE WHEN so.SalesType = '0' THEN 
			(SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' AND  LookupValue = '2WWS') 
				ELSE (SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' and  LookupValue = '2WDS') END) 
		ELSE 
		(CASE WHEN so.SalesType = '0' THEN 
			(SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' AND  LookupValue = '4WWS') 
				ELSE (SELECT LookupValuename FROM gnMstLookUpDtl WHERE CodeID = 'FPJN' and  LookupValue = '4WDS') END)  
	END AS Keterangan
	,@IsShowInfo as FlagShowInfo
	,@param HidePart
	,dtl.TaxPct
	,dtl.PPnBMSell
FROM 
	omFakturPajakHdr a
LEFT JOIN GnMstCustomer d ON a.CompanyCode = d.CompanyCode 
	AND a.CustomerCode = d.CustomerCode
LEFT JOIN GnMstCoProfile e ON a.CompanyCode = e.CompanyCode 
	AND a.BranchCode = e.BranchCode
INNER JOIN @tabData dtl ON dtl.CompanyCode = a.CompanyCode
	AND dtl.BranchCode = a.BranchCode
	AND dtl.InvoiceNo = a.InvoiceNo
LEFT JOIN omTrSalesInvoice inv ON inv.CompanyCode = a.CompanyCode 
	AND inv.BranchCode = a.BranchCode 
	AND inv.InvoiceNo = a.InvoiceNo
LEFT JOIN omTrSalesSO so	ON a.CompanyCode = so.CompanyCode
	AND a.BranchCode = so.BranchCode 
	AND inv.SONo = so.SONo
LEFT JOIN GnMstCoProfile copro ON a.CompanyCode = copro.CompanyCode 
	AND a.BranchCode = copro.BranchCode
WHERE  
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode ) #hasil

if (@param=1)
	BEGIN
	SELECT TaxType, InvoiceNo, InvoiceDate, FPJNo, FPJDate, fakturCustCode, CompanyName, compSKPNo, compSKPDate, compAddr, compPhoneNo, compFaxNo, compNPWPNo
		, CustomerName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, DiscAmt, DPPAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(ItemDPP) - sum(Potongan) from #hasil where FPJNo = c.FPJNo ) ELSE SubAmt END SubAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(TotalAmt) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) + (sum(ItemDPP) - sum(Potongan)) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, TotQuantity, PPNBMPaid, cityNm, TaxPerson, JobTitle, ItemHeader, ItemOrder, ItemCode, ItemName, PPnBM, ItemQuantity, ItemDPP, Potongan, Keterangan
		, FlagShowInfo, HidePart, TaxPct, PPnBMSell
		, (SELECT COUNT(INVOICENO) FROM #hasil WHERE FPJNo = c.FPJNo) MaxRow
		, (select sum(ItemQuantity) from #hasil where FPJNo=c.FPJNo and ItemOrder='1') SumQty
		, (select sum(ItemDPP) from #hasil where FPJNo = c.FPJNo ) XAmt
		, (select sum(Potongan) from #hasil where FPJNo = c.FPJNo ) XPotongan
		, @fInv ViewInvoice
	FROM #hasil c order by c.InvoiceNo, c.ItemOrder
	END
else
	SELECT TaxType, InvoiceNo, InvoiceDate, FPJNo, FPJDate, fakturCustCode, CompanyName, compSKPNo, compSKPDate, compAddr, compPhoneNo, compFaxNo, compNPWPNo
		, CustomerName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, DiscAmt, DPPAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo) ELSE PPNAmt END PPNAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(ItemDPP) - sum(Potongan) from #hasil where FPJNo = c.FPJNo ) ELSE SubAmt END SubAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + round(((sum(ItemDPP) - sum(Potongan)) * 0.1),0) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select  (sum(ItemDPP) - sum(Potongan)) + sum(AfterDiscPpn) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		--, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(TotalAmt) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, case when (SELECT COUNT(a.InvoiceNo) FROM (SELECT DISTINCT InvoiceNo, FPJNo FROM #hasil) a WHERE a.FPJNo = c.FPJNo GROUP BY FPJNo) > 1 THEN (select sum(AfterDiscPpn) + (sum(ItemDPP) - sum(Potongan)) from #hasil where FPJNo = c.FPJNo ) ELSE TotalAmt END TotalAmt
		, TotQuantity, PPNBMPaid, cityNm, TaxPerson, JobTitle, ItemHeader, ItemOrder, ItemCode, ItemName, PPnBM, ItemQuantity, ItemDPP, Potongan, Keterangan
		, FlagShowInfo, HidePart, TaxPct, PPnBMSell
		, (SELECT COUNT(INVOICENO) FROM #hasil WHERE FPJNo = c.FPJNo) MaxRow
		, (select sum(ItemQuantity) from #hasil where FPJNo=c.FPJNo) SumQty
		, (select sum(ItemDPP) from #hasil where FPJNo = c.FPJNo ) XAmt
		, (select sum(Potongan) from #hasil where FPJNo = c.FPJNo ) XPotongan
		, @fInv ViewInvoice
	FROM #hasil c order by c.InvoiceNo, c.ItemOrder

DROP TABLE #hasil, #Others--, #t1
END

GO


