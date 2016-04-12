-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<MASTER FAKTUR PAJAK>
-- =============================================

ALTER procedure [dbo].[usprpt_SpRpTrn010]
--declare	
	@CompanyCode		VARCHAR(15),
	@BranchCode			VARCHAR(15),
	@FPJDateStart		DATETIME,
	@FPJDateEnd			DATETIME,
	@FPJNoStart			VARCHAR(30),
	@FPJNoEnd			VARCHAR(30),
	@ProfitCenter		VARCHAR(15),
	@SeqNo				INT
	
--set @CompanyCode = '6115204001'
--set @BranchCode	= '6115204301'
--set @FPJDateStart = '20140801'
--set @FPJDateEnd	= '20141016'
--set @FPJNoStart	= '010.000-14.00000001'
--set @FPJNoEnd = '010.000-14.00000001'
--set @ProfitCenter = '300'
--set @SeqNo = 3

	--exec usprpt_SpRpTrn010 '6115204001','6115204301','20140901','20141022','010.000-14.00000001','010.000-14.00000001','300',3
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
	declare @Flag bit
	set @Flag = (select ParaValue from gnMstLookUpDtl where CompanyCode = @CompanyCode and CodeID = 'FLPG' and LookUpValue = 'NJS')

	select * into #t1 from(
	SELECT a.TPTrans
		, case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fCompName else e.CompanyGovName end)
		  else '' end as compNm
		, case @fStatus when '1' then @fSKP else e.SKPNo end as compSKPNo
		, case @fStatus when '1' then @fSKPDate else e.SKPDate end as compSKPDate
		, case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fAdd1 else 
				(case when @IsHoldingAddr=0 then isnull(e.Address1,'') +' '+isnull(e.Address2,'')+' '+ isnull(e.Address3,'')
					else (select isnull(Address1,'') +' '+isnull(Address2,'')+' '+ isnull(Address3,'') from gnMstCoProfile where CompanyCode=@CompanyCode
					and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
				end)end)
		  else '' end AS compAddr1
		, case @fStatus when '1' then @fAdd2 else 
			(case when @IsHoldingAddr=0 then isnull(e.Address2,'')
				else (select isnull(Address2,'') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)
		 end AS compAddr2
		, case @fStatus when '1' then '' else 
			(case when @IsHoldingAddr=0 then isnull(e.Address3,'')
				else (select isnull(Address3,'') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)
		 end AS compAddr3
		, case @fStatus when '1' then '' else 
			(case when @IsHoldingAddr=0 then isnull(e.Address4,'')
				else (select isnull(Address4,'') from gnMstCoProfile where CompanyCode=@CompanyCode
				and BranchCode=(select BranchCode from gnMstOrganizationDtl where CompanyCode=@CompanyCode and IsBranch=0))
			end)
		 end AS compAddr4
		, case @fStatus when '1' then '' else e.PhoneNo end as compPhoneNo
		, case @fStatus when '1' then '' else e.FaxNo end as compFaxNo
		, case when @fInfoPKP = 1 then
			(case @fStatus when '1' then @fNPWP else e.NPWPNo end)
		  else '' end as compNPWPNo
		, a.FPJNo  fakturFPJNo
		, a.FPJDate  fakturFPJDate
		, a.InvoiceNo  fakturInvNo
		, a.InvoiceDate  fakturInvDate
		, a.FPJGovNo  fakturFPJGovNo
		, a.PickingSlipNo  fakturPickSlipNo
		, a.CustomerCode  fakturCustCode
		, x.CustomerName  custName
		, d.SKPNo  custSKPNo
		, d.SKPDate  custSKPDate
		, isnull(x.Address1,'')  custAddr1
		, isnull(x.Address2,'')  custAddr2
		, isnull(x.Address3,'')  custAddr3
		, isnull(x.Address4,'')  custAddr4
		, d.PhoneNo  custPhoneNo
		, d.FaxNo  custFaxNo
		, d.NPWPNo  custNPWPNo
		, a.DueDate  fakturDueDate
		, a.TotSalesQty  fakturTotSaleQty
		, a.TotSalesAmt  fakturTotSalesAmt
		, a.TotDiscAmt  fakturTotDiscAmt
		, a.TotDppAmt  fakturTotDppAmt
		, a.TotPPNAmt  fakturPPNAmt
		, a.TotFinalSalesAmt  fakturTotFinalSalesAmt
		, a.FPJSignature
		, c.TaxPct  taxPercent
		, ISNULL((SELECT TOP 1 ParaValue FROM GnMstLookUpDtl WHERE CompanyCode = @CompanyCode AND CodeID = 'FPIF'),'0') FlagDetails
		, case @fStatus when '1' then @fCity else 
			(SELECT LookUpValueName FROM gnMstLookupDtl WHERE CodeID = 'CITY' AND LookUpValue = e.CityCode) end as cityNm
		, UPPER (f.SignName)  SignName
		, UPPER (f.TitleSign)  TitleSign
		, a.InvoiceNo 
		, isnull(@Flag, 0) Flag
		, isnull(@fInv,1) ShowInvoice
    FROM SpTrnSFPJHdr a WITH (NOLOCK, NOWAIT)
	LEFT JOIN GnMstTax c WITH (NOLOCK, NOWAIT)ON a.CompanyCode = c.CompanyCode AND c.TaxCode = 'PPN'
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
    WHERE a.CompanyCode = @CompanyCode
		AND a.BranchCode = @BranchCode 
		AND a.isPKP = 1
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
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturPPNAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturPPNAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturPPNAmt,
	--case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + ((select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) * 0.1) else (select fakturTotFinalSalesAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotFinalSalesAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select sum(fakturTotDppAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) + (select sum(fakturPPNAmt) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) else (select fakturTotFinalSalesAmt from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end fakturTotFinalSalesAmt,
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJNo desc) else (select FPJSignature from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end FPJSignature, 
	taxPercent, FlagDetails, cityNm, SignName, TitleSign, 
	case when (select count(fakturFPJNo) from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) > 1 then (select top 1 InvoiceNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJGovNo) + ' s/d ' + (select top 1 InvoiceNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo order by fakturFPJGovNo desc) else (select InvoiceNo from #t1 where fakturFPJGovNo = a.fakturFPJGovNo) end InvoiceNo, 
	Flag, ShowInvoice
from #t1 a
group by TPTrans, compNm, compSKPNo, compSKPDate, compAddr1, compAddr2, compAddr3, compAddr4, compPhoneNo, compFaxNo, compNPWPNo, fakturFPJGovNo, 
	fakturCustCode, custName, custSKPNo, custSKPDate, custAddr1, custAddr2, custAddr3, custAddr4, custPhoneNo, custFaxNo, custNPWPNo, taxPercent, FlagDetails,
	cityNm, SignName, TitleSign, Flag, ShowInvoice			
	
drop table #t1	
END
