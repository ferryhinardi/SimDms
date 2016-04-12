-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<MASTER FAKTUR PAJAK DETAIL>
-- [usprpt_SpRpTrn035Pre] '6006406','6006406','20110901','20111130','010.000-11.00038687','010.000-11.00038687','300','1'
-- =============================================

ALTER procedure [dbo].[usprpt_SpRpTrn035Pre]
--DECLARE
   @CompanyCode		VARCHAR(15),
   @BranchCode		VARCHAR(15),
   @FPJDateStart DATETIME,
   @FPJDateEnd DATETIME,
   @FPJNoStart		VARCHAR(30),
   @FPJNoEnd		VARCHAR(30),
   @ProfitCenter	VARCHAR(15),
   @SeqNo INT

--select @CompanyCode= '6114201',@BranchCode='611420100',@FPJDateStart='20150501',@FPJDateEnd='20150529',@FPJNoStart='010.001-15.70826867',
--@FPJNoEnd='010.001-15.70826867',@ProfitCenter='300',@SeqNo=1


AS   
BEGIN

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
set @fInv		= (select isnull(ParaValue,'1') from gnmstlookupdtl where codeid='FPJFLAG' and lookupvalue='SPARE')		

-- parameter use address holding or not
declare @IsHoldingAddr as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR') > 0
	set @IsHoldingAddr = (select convert(numeric,LookUpValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='FPJADDR')
else
	set @IsHoldingAddr = 0

declare @PaymentInfo as bit
if (select count(*) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='PINF') > 0
	set @PaymentInfo = (select convert(numeric,ParaValue) from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='PINF' AND LookUpValue='STAT')
else
	set @PaymentInfo = 1

	declare @Flag bit
	set @Flag = (select ParaValue from gnMstLookUpDtl where CompanyCode = @CompanyCode and CodeID = 'FLPG' and LookUpValue = 'NJS')

select * into #t1 from(
SELECT Distinct
	a.TPTrans, 
	case when @fInfoPKP = 1 then 
		(case @fStatus when '1' then @fCompName else e.CompanyGovName  end)
	else '' end as compNm, 
	case @fStatus when '1' then @fSKP else e.SKPNo end as compSKPNo, 
	case @fStatus when '1' then @fSKPDate else e.SKPDate end as compSKPDate,
	case when @fInfoPKP = 1 then 
		(case @fStatus when '1' then @fAdd1 else 
			(case when @IsHoldingAddr=0 then ISNULL(e.Address1, '') + ' ' + ISNULL(e.Address2, '') + ' ' + ISNULL(e.Address3, '')
				else (select ISNULL(Address1, '') + ' ' + ISNULL(Address2, '') + ' ' + ISNULL(Address3, '') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)
		end)
	 else '' end AS compAddr1
	,case @fStatus when '1' then @fAdd2 else 
		(case when @IsHoldingAddr=0 then ISNULL(e.Address2, '')
			else (select ISNULL(e.Address2, '') from gnMstCoProfile where CompanyCode=@CompanyCode
			and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
		end)
	 end AS compAddr2
	,case @fStatus when '1' then '' else 
		(case when @IsHoldingAddr=0 then ISNULL(e.Address3, '')
			else (select ISNULL(e.Address3, '') from gnMstCoProfile where CompanyCode=@CompanyCode
			and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
		end)
	 end AS compAddr3
	,case @fStatus when '1' then '' else 
		(case when @IsHoldingAddr=0 then ISNULL(e.Address4, '')
			else (select ISNULL(e.Address4, '') from gnMstCoProfile where CompanyCode=@CompanyCode
			and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
		end)
	 end AS compAddr4,
	case @fStatus when '1' then '' else e.PhoneNo end as compPhoneNo, 
	case @fStatus when '1' then '' else e.FaxNo  end as compFaxNo, 
	case when @fInfoPKP = 1 then 
		(case @fStatus when '1' then @fNPWP else e.NPWPNo  end)
	else '' end as compNPWPNo, 
	a.FPJNo  fakturFPJNo, 
	a.FPJDate  fakturFPJDate, 
	a.InvoiceNo  fakturInvNo,
	a.InvoiceDate  fakturInvDate, 
	a.FPJGovNo  fakturFPJGovNo, 
	a.PickingSlipNo  fakturPickSlipNo,
	/* RETURN MORE THAN 1 VALUE NEED MORE CHECK, TEMPORARY USING TOP 1 */ 
	--New--
	(SELECT TOP 1 g.OrderNo+', '+CONVERT(VARCHAR(20),g.OrderDate,105)
		FROM spTrnSORDHdr g
			LEFT JOIN spTrnSFPJDtl h ON g.CompanyCode = h.CompanyCode AND g.BranchCode = h.BranchCode AND g.DocNo = h.DocNo
		WHERE h.CompanyCode = a.CompanyCode AND h.CompanyCode = a.CompanyCode AND h.FPJNo = a.FPJNo
		GROUP BY g.OrderNo, g.OrderDate
	 )as OrderFeld,
	--End new 
	a.CustomerCode  fakturCustCode,
	x.CustomerName  custName, 
	d.SKPNo  custSKPNo, 
	d.SKPDate custSKPDate, 
	x.Address1 custAddr1, 
	x.Address2 custAddr2, 
	x.Address3 custAddr3, 
	x.Address4 custAddr4, 
	d.PhoneNo custPhoneNo, 
	d.FaxNo custFaxNo, 
	d.NPWPNo custNPWPNo, 
	a.DueDate fakturDueDate, 
	a.TotSalesQty fakturTotSaleQty, 
	a.TotSalesAmt fakturTotSalesAmt, 
	a.TotDiscAmt fakturTotDiscAmt, 
	a.TotDppAmt fakturTotDppAmt, 
	a.TotPPNAmt fakturPPNAmt, 
	a.TotFinalSalesAmt fakturTotFinalSalesAmt, 
	a.FPJSignature, 
	c.TaxPct  taxPercent, 
	case @fStatus when '1' then @fCity else 
	(SELECT LookUpValueName FROM gnMstLookupDtl WHERE CodeID = 'CITY' AND LookUpValue = e.CityCode) end as cityNm, 
	UPPER (f.SignName) SignName, 
	UPPER (f.TitleSign) TitleSign,
--	g.PartNo,
--	h.PartName,
--	g.QtyBill,
--	g.SalesAmt,
	(
		SELECT SUM(QtyBill) FROM spTrnSFPJDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND FPJNo = a.FPJNo
	) AS TotQtyBill,
	(
	SELECT COUNT (PartNo) FROM spTrnSFPJDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND FPJNo = a.FPJNo
	) AS JumlahPart
	,case @fStatus when '1' then @fAdd1 else 
		(case when @IsHoldingAddr=0 then e.Address1+' '+e.Address2
			else (select Address1+' '+Address2 from gnMstCoProfile where CompanyCode=@CompanyCode
			and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
		end)
	 end as Alamat1
	,case @fStatus when '1' then @fAdd2 else 
		(case when @IsHoldingAddr=0 then e.Address3+' '+e.Address4
			else (select Address3+' '+Address4 from gnMstCoProfile where CompanyCode=@CompanyCode
			and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
		end)
	 end as Alamat2,
	ISNULL(x.Address1,'')+' '+ISNULL(x.Address2,'') Alamat3,
	ISNULL(x.Address3,'')+' '+ISNULL(x.Address4,'') Alamat4,
--	g.PartNo+' - '+h.PartName Item,
	(SELECT COUNT (FPJNo) FROM spTrnSFPJDtl WHERE CompanyCode = g.CompanyCode AND BranchCode = g.BranchCode AND FPJNo = a.FPJNo) MaxRow
	,CASE 
		WHEN @PaymentInfo=1 THEN 'DILUNASI DENGAN ' + (select LookUpValueName from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID='PYBY' and LookUpValue= i.PaymentCode)
		ELSE '' 
	end PaymentInfo,
	 ISNULL((SELECT TOP 1 ParaValue FROM GnMstLookUpDtl WHERE CompanyCode = @CompanyCode AND CodeID = 'FPIF'),'0') FlagDetails	
	,16 PageBreak
, isnull(@Flag, 0) Flag
, isnull(@fInv,1) ShowInvoice
FROM 
	SpTrnSFPJHdr a WITH (NOLOCK, NOWAIT)
LEFT JOIN GnMstTax c WITH (NOLOCK, NOWAIT)
	ON a.CompanyCode = c.CompanyCode AND c.TaxCode = 'PPN'
LEFT JOIN GnMstCustomer d WITH (NOLOCK, NOWAIT) ON a.CompanyCode = d.CompanyCode
	AND a.CustomerCode = d.CustomerCode
LEFT JOIN SpTrnSFPJInfo x WITH (NOLOCK, NOWAIT) ON a.CompanyCode = x.CompanyCode
	AND a.BranchCode = x.BranchCode
	AND a.FPJNo = x.FPJNo
LEFt JOIN GnMstCoProfile e WITH (NOLOCK, NOWAIT) ON a.CompanyCode = e.CompanyCode
	AND a.BranchCode = e.BranchCode
LEFT JOIN GnMstSignature f WITH (NOLOCK, NOWAIT) ON a.CompanyCode = f.CompanyCode
	AND a.BranchCode = f.BranchCode
	AND f.ProfitCenterCode = @ProfitCenter
	AND f.DocumentType = CONVERT (VARCHAR (3), a.FPJNo)
	AND f.SeqNo = @SeqNo
LEFT JOIN spTrnSFPJDtl g WITH (NOLOCK, NOWAIT) ON g.CompanyCode = a.CompanyCode
	AND g.BranchCode = a.BranchCode
	AND g.FPJNo = a.FPJNo
INNER JOIN spMstItemInfo h WITH (NOLOCK, NOWAIT) ON h.CompanyCode = a.CompanyCode
	AND h.PartNo = g.PartNo
LEFT JOIN gnMstCustomerProfitCenter i on a.CompanyCode=i.CompanyCode and a.BranchCode=i.BranchCode
	AND a.CustomerCode=i.CustomerCode and i.ProfitCenterCode='300'
WHERE 
	a.CompanyCode = @CompanyCode
	AND a.BranchCode = @BranchCode AND a.isPKP = 1
	AND CONVERT(VARCHAR, a.FPJSignature, 112) BETWEEN @FPJDateStart AND @FPJDateEnd
	AND ((CASE WHEN @FPJNoStart = '' THEN a.FPJGovNo END) <> ''
	OR (CASE WHEN @FPJNoStart <> '' THEN a.FPJGovNo END) BETWEEN @FPJNoStart AND @FPJNoEnd)
	) #t1

select TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo,  
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo) + '-' + (select substring((select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc), 8, 7)) else (select fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturFPJNo, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturFPJDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select fakturFPJDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturFPJDate,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo) + '-' + (select substring((select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo desc), 8, 7)) else (select fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturInvNo, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturInvDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo desc) else (select fakturInvDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturInvDate, fakturFPJGovNo,  
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo) + '-' + (select substring((select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc), 8, 7)) else (select fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturPickSlipNo, 	
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 fakturDueDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select fakturDueDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturDueDate, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotSaleQty) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotSaleQty from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotSaleQty, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotSalesAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotSalesAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotSalesAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDiscAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotDiscAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotDiscAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotDppAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotDppAmt,
	--case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then ((select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) * 0.1) else (select fakturPPNAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturPPNAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then ((select sum(fakturPPNAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo)) else (select fakturPPNAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturPPNAmt,
	--case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + ((select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) * 0.1) else (select fakturTotFinalSalesAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotFinalSalesAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + (select sum(fakturPPNAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotFinalSalesAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotFinalSalesAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end FPJSignature, 
	taxPercent, cityNm, SignName, TitleSign, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(TotQtyBill) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select TotQtyBill from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end TotQtyBill,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(JumlahPart) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select JumlahPart from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end JumlahPart,
	Alamat1, Alamat2, Alamat3, Alamat4, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 OrderFeld from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select OrderFeld from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end OrderFeld,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(MaxRow) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select MaxRow from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end MaxRow,
	PaymentInfo, FlagDetails, PageBreak, Flag, ShowInvoice
from #t1 a
group by TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJGovNo, 
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, taxPercent, FlagDetails,
	cityNm, SignName, TitleSign, Flag, ShowInvoice, Alamat1, Alamat2, Alamat3, Alamat4, PaymentInfo, PageBreak, FlagDetails

drop table #t1		
END
