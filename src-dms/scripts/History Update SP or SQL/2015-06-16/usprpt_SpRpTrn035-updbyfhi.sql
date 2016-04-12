
GO

/****** Object:  StoredProcedure [dbo].[usprpt_SpRpTrn035]    Script Date: 6/16/2015 2:05:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




ALTER procedure [dbo].[usprpt_SpRpTrn035]
--declare
   @CompanyCode		VARCHAR(15),
   @BranchCode		VARCHAR(15),
   @FPJDateStart	DATETIME,
   @FPJDateEnd		DATETIME,
   @FPJNoStart		VARCHAR(30),
   @FPJNoEnd		VARCHAR(30),
   @ProfitCenter	VARCHAR(15),
   @SeqNo			INT
   
AS   
BEGIN

--SELECT @CompanyCode= '6145203',@BranchCode= '614520301',@FPJDateStart= '20150501',@FPJDateEnd= '20150531',@FPJNoStart= '010.001-15.29139679',
--@FPJNoEnd= '010.001-15.29139679',@ProfitCenter= '300',@SeqNo= 1

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

select * into #t1
from (
	SELECT	
		a.TPTrans, 
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fCompName else e.CompanyGovName end)
		else '' end as compNm, 
		case @fStatus when '1' then @fSKP else e.SKPNo end as compSKPNo, 		
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fSKPDate else e.SKPDate end)
		else '' end as compSKPDate,
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd1 else 
				(case when @IsHoldingAddr=0 then ISNULL(e.Address1, '') + ' ' + ISNULL(e.Address2, '') + ' ' + ISNULL(e.Address3, '')
					else (select ISNULL(Address1, '') + ' ' + ISNULL(Address2, '') + ' ' + ISNULL(Address3, '') from gnMstCoProfile where CompanyCode=@CompanyCode
					and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
				end)end)
		else '' end AS compAddr1,
		case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd2 else 
				(case when @IsHoldingAddr=0 then ISNULL(e.Address2, '')
					else (select ISNULL(e.Address2, '') from gnMstCoProfile where CompanyCode=@CompanyCode
					and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
				end)end)
		else '' end AS compAddr2
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
		g.PartNo,
		h.PartName,
		g.QtyBill,
		g.SalesAmt,
		g.DiscAmt,
		g.PPNAmt,
		(
			SELECT SUM(QtyBill) FROM spTrnSFPJDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND FPJNo = a.FPJNo
		) AS TotQtyBill,
		(
		SELECT COUNT (PartNo) FROM spTrnSFPJDtl WHERE CompanyCode = a.CompanyCode AND BranchCode = a.BranchCode AND FPJNo = a.FPJNo
		) AS JumlahPart
		,case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd1 else 
			(case when @IsHoldingAddr=0 then e.Address1+' '+e.Address2
				else (select Address1+' '+Address2 from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)end)
		 else '' end as Alamat1
		,case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd2 else 
			(case when @IsHoldingAddr=0 then e.Address3+' '+e.Address4
				else (select Address3+' '+Address4 from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)end)
		 else '' end as Alamat2,
		ISNULL(x.Address1,'')+' '+ISNULL(x.Address2,'') Alamat3,
		ISNULL(x.Address3,'')+' '+ISNULL(x.Address4,'') Alamat4,
		g.PartNo+' - '+h.PartName Item
		,CASE 
			WHEN @PaymentInfo=1 THEN 'DILUNASI DENGAN ' + (select LookUpValueName from gnMstLookUpDtl where CompanyCode=a.CompanyCode and CodeID='PYBY' and LookUpValue= i.PaymentCode)
			ELSE '' 
		end PaymentInfo,
		 ISNULL((SELECT TOP 1 ParaValue FROM GnMstLookUpDtl WHERE CompanyCode = @CompanyCode AND CodeID = 'FPIF'),'0') FlagDetails
		,(select count(PartNo) from spTrnSFPJDtl where CompanyCode=a.CompanyCode and BranchCode=a.BranchCode and FPJNo=a.FPJNo) MaxRow
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

select * into #t2 from(
select TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo,
	case when (a.countFaktur) > 1 then (select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo) + '-' + (select substring((select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc), 8, 7)) else (select top 1 fakturFPJNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturFPJNo, 
	case when (a.countFaktur) > 1 then (select top 1 fakturFPJDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select top 1 fakturFPJDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturFPJDate,
	case when (a.countFaktur) > 1 then (select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo) + '-' + (select substring((select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo desc), 8, 7)) else (select top 1 fakturInvNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturInvNo, 
	case when (a.countFaktur) > 1 then (select top 1 fakturInvDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturInvNo desc) else (select top 1 fakturInvDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturInvDate,
	fakturFPJGovNo,  
	case when (a.countFaktur) > 1 then (select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo) + '-' + (select substring((select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc), 8, 7)) else (select top 1 fakturPickSlipNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturPickSlipNo,
	case when (a.countFaktur) > 1 then ' ' else (select top 1 OrderFeld from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end OrderFeld,
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, 
	case when (a.countFaktur) > 1 then (select top 1 fakturDueDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select top 1 fakturDueDate from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturDueDate,
	
	case when (a.countFaktur) > 1 then (select sum(QtyBill) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(fakturTotSaleQty) end fakturTotSaleQty, 
	case when (a.countFaktur) > 1 then (select sum(SalesAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(fakturTotSalesAmt) end fakturTotSalesAmt, 
	case when (a.countFaktur) > 1 then (select sum(DiscAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(fakturTotDiscAmt) end fakturTotDiscAmt,
	case when (a.countFaktur) > 1 then (select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(fakturTotDPPAmt) end fakturTotDppAmt, 
	--case when (a.countFaktur) > 1 then ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) * 0.1) else sum(fakturPPNAmt) end fakturPPNAmt, 
	--sum(fakturPPNAmt) fakturPPNAmt, 
	case when (a.countFaktur) > 1 then ((select (sum(PPNAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo)) else sum(fakturPPNAmt) end fakturPPNAmt, 
	--case when (a.countFaktur) > 1 then ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) * 0.1)) else sum(fakturTotFinalSalesAmt) end fakturTotFinalSalesAmt, 
	--case when (a.countFaktur) > 1 then ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + (select sum(fakturPPNAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo)) else sum(fakturTotFinalSalesAmt) end fakturTotFinalSalesAmt, 
	case when (a.countFaktur) > 1 then ((select (sum(SalesAmt) - sum(DiscAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + ((select (sum(PPNAmt)) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo))) else sum(fakturTotFinalSalesAmt) end fakturTotFinalSalesAmt, 

	case when (a.countFaktur) > 1 then (select top 1 FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select top 1 FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end FPJSignature,
	TaxPercent, cityNm, SignName, TitleSign, PartNo, PartName, sum(QtyBill) QtyBill, sum(SalesAmt) SalesAmt, 
	case when (a.countFaktur) > 1 then (select sum(QtyBill) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else sum(TotQtyBill) end TotQtyBill, 
	sum(JumlahPart) JumlahPart, Alamat1, Alamat2, Alamat3, Alamat4, Item, PaymentInfo, FlagDetails, sum(MaxRow) MaxRow, a.countFaktur
from
(
select distinct TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJNo,
	fakturFPJDate, fakturInvNo, fakturInvDate, #t1.fakturFPJGovNo, fakturPickSlipNo, OrderFeld, fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, 
	custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, fakturTotSaleQty, fakturTotSalesAmt, fakturTotDiscAmt, fakturTotDPPAmt,
	fakturPPNAmt, fakturTotFinalSalesAmt, FPJSignature, TaxPercent, cityNm, SignName, TitleSign, PartNo, PartName, sum(QtyBill) QtyBill, sum(SalesAmt) SalesAmt,
	sum(DiscAmt) DiscAmt, TotQtyBill, JumlahPart, Alamat1, Alamat2, Alamat3, Alamat4, Item, PaymentInfo, FlagDetails, b.countFaktur, sum(MaxRow) MaxRow
from #t1 
left join (select fakturFPJGovNo, count(fakturFPJNo) countFaktur
		from(select distinct fakturFPJGovNo, fakturFPJNo from #t1) a
		group by fakturFPJGovNo) b on #t1.fakturFPJGovNo = b.fakturFPJGovNo
group by TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJNo,
	fakturFPJDate, fakturInvNo, fakturInvDate, #t1.fakturFPJGovNo, fakturPickSlipNo, OrderFeld, fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, 
	custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, fakturTotSaleQty, fakturTotSalesAmt, fakturTotDiscAmt, fakturTotDPPAmt,
	fakturPPNAmt, fakturTotFinalSalesAmt, FPJSignature, TaxPercent, cityNm, SignName, TitleSign, PartNo, PartName, Alamat1, Alamat2, Alamat3, Alamat4, Item, 
	PaymentInfo, FlagDetails, b.countFaktur, JumlahPart, TotQtyBill
) a
group by fakturFPJGovNo, TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, 
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, TaxPercent, cityNm, 
	SignName, TitleSign, PartNo, PartName, Alamat1, Alamat2, Alamat3, Alamat4, PaymentInfo, FlagDetails, item, a.countFaktur
) #t2

select TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJNo, fakturFPJDate
	, fakturInvDate, fakturFPJGovNo, fakturPickSlipNo, OrderFeld, fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3
	, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, fakturDueDate, fakturTotSaleQty
	, fakturTotSalesAmt, fakturTotDiscAmt, fakturTotDppAmt, fakturPPNAmt
	, fakturTotFinalSalesAmt, FPJSignature, TaxPercent, cityNm, SignName, TitleSign
	, PartNo, PartName, QtyBill, SalesAmt, TotQtyBill
	, case when (a.countFaktur) > 1 then (select count(Item) from #t2 where fakturFPJGovNo = a.fakturFPJGovNo) else JumlahPart end JumlahPart
	, Alamat1, Alamat2, Alamat3, Alamat4, Item, PaymentInfo, FlagDetails
	, case when (a.countFaktur) > 1 then (select count(Item) from #t2 where fakturFPJGovNo = a.fakturFPJGovNo) else MaxRow end MaxRow
	, case when (case when (a.countFaktur) > 1 then (select count(Item) from #t2 where fakturFPJGovNo = a.fakturFPJGovNo) else MaxRow end) % 17 = 1 then 16 else 17 end as PageBreak
	, isnull(@fInv,1) ShowInvoice
from #t2 a

	drop table #t1, #t2
END




GO


