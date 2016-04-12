USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_SvInqFpjList]    Script Date: 2/9/2015 11:17:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create procedure [dbo].[uspfn_SvInqFpjHQList]
	@CompanyCode nvarchar(20),
	@BranchCode nvarchar(20)

as

 select distinct
	trnFakturPajak.FPJNo
	, trnFakturPajak.FPJDate
	, (
		(	
			select top 1 InvoiceNo+' ('+substring(BranchCode,len(BranchCode)-1,3)+')'
			from svTrnInvoice 
			where CompanyCode = trnFakturPajak.CompanyCode 
				and FPJNo = trnFakturPajak.FPJNo 
				and isLocked = 1
			order by BranchCode,InvoiceNo
		) 
		+ ' s/d ' + 
		(	
			select top 1 InvoiceNo+' ('+substring(BranchCode,len(BranchCode)-1,3)+')'
			from svTrnInvoice 
			where CompanyCode = trnFakturPajak.CompanyCode 
				and FPJNo = trnFakturPajak.FPJNo 
				and isLocked = 1
			order by InvoiceNo,BranchCode desc
		)
	) as Invoice	
	, (
		select top 1 BranchCode 
		from svTrnInvoice 
		where CompanyCode = trnFakturPajak.CompanyCode  
			and FPJNo = trnFakturPajak.FPJNo 
			and isLocked = 1
		order by BranchCode, InvoiceNo
	) as BranchStart
    , (
		select top 1 BranchCode 
		from svTrnInvoice	
		where CompanyCode = trnFakturPajak.CompanyCode  
			and FPJNo = trnFakturPajak.FPJNo 
			and isLocked = 1
		order by BranchCode desc
	) as BranchEnd
	, (	trnFakturPajak.CustomerCode + ' - ' + 
		(
			select CustomerName 
			from gnMstCustomer
			where CompanyCode = trnFakturPajak.CompanyCode  and CustomerCode = trnFakturPajak.CustomerCode
		)
	) as Customer
	, (	trnFakturPajak.CustomerCodeBill + ' - ' + 
		(
			select CustomerName 
			from gnMstCustomer 
			where CompanyCode = trnFakturPajak.CompanyCode  and CustomerCode = trnFakturPajak.CustomerCodeBill
		)
	) as CustomerBill
from svTrnFakturPajak trnFakturPajak
left join svTrnInvoice trnInvoice on 
	trnInvoice.CompanyCode = trnFakturPajak.CompanyCode 
	--and trnInvoice.BranchCode = trnFakturPajak.BranchCode
	and trnInvoice.FPJNo = trnFakturPajak.FPJNo
	and trnInvoice.IsLocked=1
where 
    trnFakturPajak.CompanyCode = @CompanyCode
	and trnFakturPajak.BranchCode = @BranchCode
	and trnFakturPajak.FPJNo like 'FPH%' 
order by trnFakturPajak.FPJNo desc
 
 
  
  