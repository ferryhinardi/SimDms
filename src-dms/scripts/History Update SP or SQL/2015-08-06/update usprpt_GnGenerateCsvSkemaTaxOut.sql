ALTER procedure [dbo].[usprpt_GnGenerateCsvSkemaTaxOut]
--DECLARE
	@CompanyCode varchar(15)
	,@BranchCodeFrom varchar(15)
	,@BranchCodeTo varchar(15)
	,@PeriodFrom smalldatetime		
	,@PeriodTo smalldatetime
	,@ProductType varchar(2)
	,@table varchar(1)=1
as

--execute usprpt_GnGenerateCsvSkemaTaxOut '6115204','611520401','2015-07-01','2015-07-28','2W',3
--declare @nomorfaktur as varchar(100)
--select @CompanyCode=N'6118201',@BranchCodeFrom=N'611820100',@BranchCodeTo=N'611820100',@PeriodFrom='2015-07-01',@PeriodTo='2015-07-31',@ProductType=N'2W', @table='1', @nomorfaktur = '0011539303581'

 --0= Not Recalculate PPN, 1= Recalculate PPN (For FPJ Gabungan)
declare @IsRecalPPN as bit
set @IsRecalPPN=0

if @table = 1
begin
	if (select count(ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS') > 0
	set @IsRecalPPN= (select convert(bit,ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS')
	
SELECT 
	FK
		, KD_JENIS_TRANSAKSI
		, FG_PENGGANTI
		, NOMOR_FAKTUR
		, MASA_PAJAK
		, TAHUN_PAJAK
		, TANGGAL_FAKTUR
		, NPWP
		, NAMA
		, ALAMAT_LENGKAP
		, FLOOR(SUM(JUMLAH_DPP)) JUMLAH_DPP
		, FLOOR(SUM(JUMLAH_PPN))  JUMLAH_PPN
		, FLOOR(SUM(JUMLAH_PPNBM)) JUMLAH_PPNBM
		, ID_KETERANGAN_TAMBAHAN
		, FG_UANG_MUKA
		, UANG_MUKA_DPP
		, UANG_MUKA_PPN
		, UANG_MUKA_PPNBM
		, REFERENSI
		, CUSTOMERCODE
	FROM(	
	select 
		FK
		, KD_JENIS_TRANSAKSI
		, FG_PENGGANTI
		, substring(NOMOR_FAKTUR,4,13) NOMOR_FAKTUR
		, MASA_PAJAK
		, TAHUN_PAJAK
		, TANGGAL_FAKTUR
		, NPWP
		, NAMA_LAWAN_TRANSAKSI NAMA
		, ALAMAT_LENGKAP
		, JUMLAH_DPP
		, ROUND(JUMLAH_PPN,0)  JUMLAH_PPN
		, JUMLAH_PPNBM
		, ID_KETERANGAN_TAMBAHAN
		, FG_UANG_MUKA
		, UANG_MUKA_DPP
		, UANG_MUKA_PPN
		, UANG_MUKA_PPNBM
		, REFERENSI
		, CUSTOMERCODE
			from (
						select 'FK' [FK]
						, LEFT(b.TaxNo,2) KD_JENIS_TRANSAKSI
						, 0 FG_PENGGANTI
						, REPLACE(REPLACE(b.TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
						, (case when len(b.PeriodMonth) = 1 then convert(varchar, b.PeriodMonth, 1) 
							else convert(varchar, b.PeriodMonth, 2) end) MASA_PAJAK
						, b.PeriodYear TAHUN_PAJAK
						, CONVERT(VARCHAR, b.TaxDate, 103) TANGGAL_FAKTUR
						
						, REPLACE(REPLACE(b.NPWP, '.', ''), '-', '') NPWP
						, b.CustomerName NAMA_LAWAN_TRANSAKSI
						, CASE WHEN LEFT(b.FPJNo,3) = 'FPJ' THEN 
						(SELECT Address1 + ' ' + Address2 FROM spTrnSFPJInfo WHERE CompanyCode = b.CompanyCode AND BranchCode = b.BranchCode AND FPJNo =b.FPJNo) ELSE  
						  CASE WHEN LEFT(b.FPJNo,3) = 'FPS' THEN 
						(SELECT Address1 + ' ' + Address2 FROM svTrnFakturPajak WHERE CompanyCode = b.CompanyCode AND BranchCode = b.BranchCode AND FPJNo = b.FPJNo) ELSE
						(SELECT Address1 + ' ' + Address2 FROM gnMstCustomer WHERE CompanyCode = b.CompanyCode AND CustomerCode = b.CustomerCode)	
						END END ALAMAT_LENGKAP
						, JUMLAH_DPP = a.NetSalesAmt
						, JUMLAH_PPN = FLOOR(a.NetSalesAmt * 0.10)
						, b.PPNBmAmt JUMLAH_PPNBM 
						, '' ID_KETERANGAN_TAMBAHAN
						, 0 FG_UANG_MUKA
						, 0 UANG_MUKA_DPP
						, 0 UANG_MUKA_PPN
						, 0 UANG_MUKA_PPNBM
						, Case when left(b.FPJNo,3) = 'FPS'
									then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end)
							   when left(FPJNo,3) = 'FPJ' 
									then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' (' + b.FPJNo + ')' + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end) -- Penambahan
							   when left(FPJNo,3) = 'FPH' then 'No FPJ : ' + FPJNo
							   else (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1 then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end)
						  end + ' - Cabang ' + SUBSTRING(b.BranchCode,len(b.branchcode)-1,2) REFERENSI   
						, b.CUSTOMERCODE
						, b.FPJNO
						
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
					
						from spTrnSInvoicedtl a
						inner join (select CompanyCode, BranchCode, FPJNo, CustomerCode, ReferenceNo, TaxNo, PeriodMonth, PeriodYear, TaxDate, NPWP, CustomerName, PPNBmAmt, SubmissionDate
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
											AND ProductType = @ProductType
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3)='FPJ'
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo
						union all
						select 'FK' [FK]
						, LEFT(b.TaxNo,2) KD_JENIS_TRANSAKSI
						, 0 FG_PENGGANTI
						, REPLACE(REPLACE(b.TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
						, (case when len(b.PeriodMonth) = 1 then convert(varchar, b.PeriodMonth, 1) 
							else convert(varchar, b.PeriodMonth, 2) end) MASA_PAJAK
						, b.PeriodYear TAHUN_PAJAK
						, CONVERT(VARCHAR, b.TaxDate, 103) TANGGAL_FAKTUR
						
						, REPLACE(REPLACE(b.NPWP, '.', ''), '-', '') NPWP
						, b.CustomerName NAMA_LAWAN_TRANSAKSI
						, CASE WHEN LEFT(b.FPJNo,3) = 'FPJ' THEN 
						(SELECT Address1 + ' ' + Address2 FROM spTrnSFPJInfo WHERE CompanyCode = b.CompanyCode AND BranchCode = b.BranchCode AND FPJNo =b.FPJNo) ELSE  
						  CASE WHEN LEFT(b.FPJNo,3) = 'FPS' THEN 
						(SELECT Address1 + ' ' + Address2 FROM svTrnFakturPajak WHERE CompanyCode = b.CompanyCode AND BranchCode = b.BranchCode AND FPJNo = b.FPJNo) ELSE
						(SELECT Address1 + ' ' + Address2 FROM gnMstCustomer WHERE CompanyCode = b.CompanyCode AND CustomerCode = b.CustomerCode)	
						END END ALAMAT_LENGKAP
						, JUMLAH_DPP = a.BeforeDiscDPP - a.DiscExcludePPn
						, JUMLAH_PPN =  FLOOR(a.AfterDiscPPn)
						, b.PPNBmAmt JUMLAH_PPNBM 
						, '' ID_KETERANGAN_TAMBAHAN
						, 0 FG_UANG_MUKA
						, 0 UANG_MUKA_DPP
						, 0 UANG_MUKA_PPN
						, 0 UANG_MUKA_PPNBM
						, Case when left(b.FPJNo,3) = 'FPS'
									then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end)
							   when left(FPJNo,3) = 'FPJ' 
									then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' (' + b.FPJNo + ')' + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end) -- Penambahan
							   when left(FPJNo,3) = 'FPH' then 'No FPJ : ' + FPJNo
							   else (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1 then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end)
						  end + ' - Cabang ' + SUBSTRING(b.BranchCode,len(b.branchcode)-1,2) REFERENSI   
						, b.CUSTOMERCODE
						, b.FPJNO
						
						, 'Sales Model Desc : '+ (select SalesModelDesc from omMstModel where CompanyCode = a.CompanyCode and SalesModelCode = a.SalesModelCode)
						  +' Nomor Rangka : '+ CONVERT(varchar, c.ChassisNo,100)
						  +' Nomor Mesin : '+  CONVERT(varchar, c.EngineNo,100) NAMA
						
						from omTrSalesInvoicemodel a
						inner join (select CompanyCode, BranchCode, FPJNo, CustomerCode, ReferenceNo, TaxNo, PeriodMonth, PeriodYear, TaxDate, NPWP, CustomerName, PPNBmAmt, SubmissionDate
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3) not in ('FPJ','FPS')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						inner join omTrSalesInvoiceVin c on a.CompanyCode = c.CompanyCode
							and a.BranchCode = c.BranchCode
							and a.InvoiceNo = c.InvoiceNo
							and a.BPKNo = c.BPKNo
							and a.SalesModelCode = c.SalesModelCode
							and a.SalesModelYear = c.SalesModelYear
						union all
						select 'FK' [FK]
						, LEFT(b.TaxNo,2) KD_JENIS_TRANSAKSId
						, 0 FG_PENGGANTI
						, REPLACE(REPLACE(b.TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
						, (case when len(b.PeriodMonth) = 1 then convert(varchar, b.PeriodMonth, 1) 
							else convert(varchar, b.PeriodMonth, 2) end) MASA_PAJAK
						, b.PeriodYear TAHUN_PAJAK
						, CONVERT(VARCHAR, b.TaxDate, 103) TANGGAL_FAKTUR
						
						, REPLACE(REPLACE(b.NPWP, '.', ''), '-', '') NPWP
						, b.CustomerName NAMA_LAWAN_TRANSAKSI
						, CASE WHEN LEFT(b.FPJNo,3) = 'FPJ' THEN 
						(SELECT Address1 + ' ' + Address2 FROM spTrnSFPJInfo WHERE CompanyCode = b.CompanyCode AND BranchCode = b.BranchCode AND FPJNo =b.FPJNo) ELSE  
						  CASE WHEN LEFT(b.FPJNo,3) = 'FPS' THEN 
						(SELECT Address1 + ' ' + Address2 FROM svTrnFakturPajak WHERE CompanyCode = b.CompanyCode AND BranchCode = b.BranchCode AND FPJNo = b.FPJNo) ELSE
						(SELECT Address1 + ' ' + Address2 FROM gnMstCustomer WHERE CompanyCode = b.CompanyCode AND CustomerCode = b.CustomerCode)	
						END END ALAMAT_LENGKAP
						, JUMLAH_DPP = a.UnitPriceAmt * a.Quantity
						, JUMLAH_PPN =  FLOOR(a.PPNAmt)
						, b.PPNBmAmt JUMLAH_PPNBM 
						, '' ID_KETERANGAN_TAMBAHAN
						, 0 FG_UANG_MUKA
						, 0 UANG_MUKA_DPP
						, 0 UANG_MUKA_PPN
						, 0 UANG_MUKA_PPNBM
						, Case when left(b.FPJNo,3) = 'FPS'
									then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end)
							   when left(FPJNo,3) = 'FPJ' 
									then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' (' + b.FPJNo + ')' + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end) -- Penambahan
							   when left(FPJNo,3) = 'FPH' then 'No FPJ : ' + FPJNo
							   else (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1 then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end)
						  end + ' - Cabang ' + SUBSTRING(b.BranchCode,len(b.branchcode)-1,2) REFERENSI   
						, b.CUSTOMERCODE
						, b.FPJNO
						
						, Description NAMA
					
						from arTrnInvoiceDtl a
						inner join (select CompanyCode, BranchCode, FPJNo, CustomerCode, ReferenceNo, TaxNo, PeriodMonth, PeriodYear, TaxDate, NPWP, CustomerName, PPNBmAmt, SubmissionDate
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
											AND ProductType = @ProductType
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3) not in ('FPJ','FPS')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						union all 
						select  
							[FK],
							KD_JENIS_TRANSAKSI,
							FG_PENGGANTI,
							NOMOR_FAKTUR,
							MASA_PAJAK,
							TAHUN_PAJAK,
							TANGGAL_FAKTUR,
							NPWP,
							NAMA_LAWAN_TRANSAKSI NAMA,
							REPLACE(REPLACE(ALAMAT_LENGKAP, CHAR(13),''),CHAR(10),'') ALAMAT_LENGKAP,
							FLOOR(JUMLAH_DPP) JUMLAH_DPP,
							FLOOR(JUMLAH_DPP) * 0.1 JUMLAH_PPN,
							 --FLOOR((FLOOR(JUMLAH_DPP) * 0.1)) JUMLAH_PPN,
							JUMLAH_PPNBM,
							ID_KETERANGAN_TAMBAHAN,
							FG_UANG_MUKA,
							UANG_MUKA_DPP,
							UANG_MUKA_PPN,
							UANG_MUKA_PPNBM,
							REFERENSI,
							CUSTOMERCODE,
							FPJNO,
							NAMA
						from(
						select 'FK' [FK]
						, LEFT(b.TaxNo,2) KD_JENIS_TRANSAKSI
						, 0 FG_PENGGANTI
						, REPLACE(REPLACE(b.TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
						, (case when len(b.PeriodMonth) = 1 then convert(varchar, b.PeriodMonth, 1) 
							else convert(varchar, b.PeriodMonth, 2) end) MASA_PAJAK
						, b.PeriodYear TAHUN_PAJAK
						, CONVERT(VARCHAR, b.TaxDate, 103) TANGGAL_FAKTUR
						
						, REPLACE(REPLACE(b.NPWP, '.', ''), '-', '') NPWP
						, b.CustomerName NAMA_LAWAN_TRANSAKSI
						, CASE WHEN LEFT(b.FPJNo,3) = 'FPJ' THEN 
						(SELECT Address1 + ' ' + Address2 FROM spTrnSFPJInfo WHERE CompanyCode = b.CompanyCode AND BranchCode = b.BranchCode AND FPJNo =b.FPJNo) ELSE  
						  CASE WHEN LEFT(b.FPJNo,3) = 'FPS' THEN 
						(SELECT Address1 + ' ' + Address2 FROM svTrnFakturPajak WHERE CompanyCode = b.CompanyCode AND BranchCode = b.BranchCode AND FPJNo = b.FPJNo) ELSE
						(SELECT Address1 + ' ' + Address2 FROM gnMstCustomer WHERE CompanyCode = b.CompanyCode AND CustomerCode = b.CustomerCode)	
						END END ALAMAT_LENGKAP
						, JUMLAH_DPP = (a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100)
						--, JUMLAH_PPN = isnull (convert(decimal(12,2),(((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100.00))*0.10),0),0.00) 
						, b.PPNBmAmt JUMLAH_PPNBM 
						, '' ID_KETERANGAN_TAMBAHAN
						, 0 FG_UANG_MUKA
						, 0 UANG_MUKA_DPP
						, 0 UANG_MUKA_PPN
						, 0 UANG_MUKA_PPNBM
						, Case when left(b.FPJNo,3) = 'FPS'
									then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end)
							   when left(b.FPJNo,3) = 'FPJ' 
									then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' (' + b.FPJNo + ')' + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end) -- Penambahan
							   when left(b.FPJNo,3) = 'FPH' then 'No FPJ : ' + FPJNo
							   else (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1 then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end)
						  end + ' - Cabang ' + SUBSTRING(b.BranchCode,len(b.branchcode)-1,2) REFERENSI   
						, b.CUSTOMERCODE
						, b.FPJNO
						
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						
						from svTrnInvItem a
						inner join (select CompanyCode, BranchCode, FPJNo, CustomerCode, ReferenceNo, TaxNo, PeriodMonth, PeriodYear, TaxDate, NPWP, CustomerName, PPNBmAmt, SubmissionDate
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
											AND ProductType = @ProductType
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND (left(fpjno,3)='FPS' OR LEFT(FPJNo,3)='FPH')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
					)Z
					
---- tambahan untuk jasa service
						union all 
						select  
							[FK],
							KD_JENIS_TRANSAKSI,
							FG_PENGGANTI,
							NOMOR_FAKTUR,
							MASA_PAJAK,
							TAHUN_PAJAK,
							TANGGAL_FAKTUR
							, NPWP
							, NAMA_LAWAN_TRANSAKSI NAMA
							, REPLACE(REPLACE(ALAMAT_LENGKAP, CHAR(13),''),CHAR(10),'') ALAMAT_LENGKAP
							, FLOOR(JUMLAH_DPP) JUMLAH_DPP
							, FLOOR(JUMLAH_DPP) * 0.1 JUMLAH_PP
							, JUMLAH_PPNBM
							, ID_KETERANGAN_TAMBAHAN
							, FG_UANG_MUKA
							, UANG_MUKA_DPP
							, UANG_MUKA_PPN
							, UANG_MUKA_PPNBM
							, REFERENSI
							, CUSTOMERCODE
							, FPJNO
							
							, NAMA
						from(
						select 'FK' [FK]
						, a.CompanyCode
						, a.BranchCode
						, LEFT(b.TaxNo,2) KD_JENIS_TRANSAKSI
						, 0 FG_PENGGANTI
						, REPLACE(REPLACE(b.TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
						, (case when len(b.PeriodMonth) = 1 then convert(varchar, b.PeriodMonth, 1) 
							else convert(varchar, b.PeriodMonth, 2) end) MASA_PAJAK
						, b.PeriodYear TAHUN_PAJAK
						, CONVERT(VARCHAR, b.TaxDate, 103) TANGGAL_FAKTUR
						
						, REPLACE(REPLACE(b.NPWP, '.', ''), '-', '') NPWP
						, b.CustomerName NAMA_LAWAN_TRANSAKSI
						, CASE WHEN LEFT(b.FPJNo,3) = 'FPJ' THEN 
						(SELECT Address1 + ' ' + Address2 FROM spTrnSFPJInfo WHERE CompanyCode = b.CompanyCode AND BranchCode = b.BranchCode AND FPJNo =b.FPJNo) ELSE  
						  CASE WHEN LEFT(b.FPJNo,3) = 'FPS' THEN 
						(SELECT Address1 + ' ' + Address2 FROM svTrnFakturPajak WHERE CompanyCode = b.CompanyCode AND BranchCode = b.BranchCode AND FPJNo = b.FPJNo) ELSE
						(SELECT Address1 + ' ' + Address2 FROM gnMstCustomer WHERE CompanyCode = b.CompanyCode AND CustomerCode = b.CustomerCode)	
						END END ALAMAT_LENGKAP
						, JUMLAH_DPP = (a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100)
						--, JUMLAH_PPN = isnull (convert(decimal(12,2),(((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100.00))*0.10),0),0.00) 
						, b.PPNBmAmt JUMLAH_PPNBM 
						, '' ID_KETERANGAN_TAMBAHAN
						, 0 FG_UANG_MUKA
						, 0 UANG_MUKA_DPP
						, 0 UANG_MUKA_PPN
						, 0 UANG_MUKA_PPNBM
						, Case when left(b.FPJNo,3) = 'FPS'
									then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end)
							   when left(b.FPJNo,3) = 'FPJ' 
									then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' (' + b.FPJNo + ')' + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end) -- Penambahan
							   when left(b.FPJNo,3) = 'FPH' then 'No FPJ : ' + b.FPJNo
							   else (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = b.CompanyCode AND a.BranchCode = b.BranchCode AND a.TaxNo = b.TaxNo) > 1 then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) else 'No Invoice : ' + b.ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),b.SubmissionDate,106) end)
						  end + ' - Cabang ' + SUBSTRING(b.BranchCode,len(b.branchcode)-1,2) REFERENSI   
						, b.CUSTOMERCODE
						, b.FPJNO
						
						, (select top 1 replace(rtrim(Description),',',' ') from svMstTask 
						    where CompanyCode=a.CompanyCode and BasicModel=x.BasicModel and OperationNo=a.OperationNo
						  ) NAMA
						
						from svTrnInvTask a
						inner join svTrnInvoice x
						    on a.CompanyCode=x.CompanyCode and a.BranchCode=x.BranchCode and a.InvoiceNo=x.InvoiceNo
						inner join (select CompanyCode, BranchCode, FPJNo, CustomerCode, ReferenceNo, TaxNo, PeriodMonth, PeriodYear, TaxDate, NPWP, CustomerName, PPNBmAmt, SubmissionDate
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
											AND ProductType = @ProductType
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND (left(fpjno,3)='FPS' OR LEFT(FPJNo,3)='FPH')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						
					)Y
				) a --where substring(NOMOR_FAKTUR,4,13) = @NomorFaktur
			) b
		group by FK, KD_JENIS_TRANSAKSI, FG_PENGGANTI
							, NOMOR_FAKTUR
							, MASA_PAJAK, TAHUN_PAJAK, tanggal_faktur, npwp, NAMA, ALAMAT_LENGKAP
							, ID_KETERANGAN_TAMBAHAN
							, FG_UANG_MUKA
							, UANG_MUKA_DPP
							, UANG_MUKA_PPN
							, UANG_MUKA_PPNBM
							, REFERENSI
							, CUSTOMERCODE
		order by NOMOR_FAKTUR
end
if @table = 2
begin
	select  'LT' LT, REPLACE(REPLACE(NPWPNo, '.', ''), '-', '') NPWP, CustomerName NAMA, REPLACE(REPLACE(Address1, CHAR(13),''),CHAR(10),'')  + REPLACE(REPLACE(Address2, CHAR(13),''),CHAR(10),'') JALAN, '-' BLOK, '-' NOMOR, '0' RT, '0' RW, 
	KecamatanDistrik KECAMATAN, KelurahanDesa KELURAHAN, KotaKabupaten KABUPATEN, 
	case when isnull(ProvinceCode,'') = '' then '-' else (isnull((select top 1 lookupvaluename from gnmstlookupdtl where codeid='PROV' and LookUpValue = ProvinceCode),'-' ))end PROPINSI, 
	ZipNo KODE_POS, PhoneNo NOMOR_TELEPON, CUSTOMERCODE
	from gnMstCustomer a
	where CustomerCode in 
	(select distinct CustomerCode from gnTaxOut a WHERE
					CompanyCode = @CompanyCode
					AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
					AND ProductType = @ProductType
					AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
					AND IsPKP = 1)
end
if @table = 3
begin
select [OF],
		KODE_OBJEK,
		NAMA,
		HARGA_SATUAN,
		JUMLAH_BARANG,
		HARGA_TOTAL,
		DISKON,
		DPP,
		CONVERT(INT, PPN) PPN,
		TARIF_PPNBM,
		PPNBM,
		CUSTOMERCODE,
		NOMOR_FAKTUR,
		FPJNO
--SUM(dpp), SUM(ppn) 
from(
					select  
						[OF],
						KODE_OBJEK,
						NAMA,
						HARGA_SATUAN,
						JUMLAH_BARANG,
						HARGA_TOTAL,
						DISKON,
						DPP,
						FLOOR(ROUND(PPN,0))PPN,
						TARIF_PPNBM,
						PPNBM,
						CUSTOMERCODE,
						NOMOR_FAKTUR,
						FPJNO
					from(
					select 'OF' [OF]
					, PartNo KODE_OBJEK
					, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
					, isnull ((convert(decimal(12,2),a.retailprice)), 0.00) HARGA_SATUAN
					, isnull ((convert(decimal(12,2),a.QtyBill)), 0.00) JUMLAH_BARANG
					, isnull ((convert(decimal(12,2),a.SalesAmt)), 0.00)HARGA_TOTAL
					, isnull ((convert(decimal(12,2),a.DiscAmt)), 0.00) DISKON
					--, isnull ((convert(decimal(12,2),a.NetSalesAmt)), 0.00) DPP
					, a.NetSalesAmt DPP
					--, FLOOR(isnull ((convert(decimal(12,2),a.NetSalesAmt * 0.10)), 0.00)) PPN
					, FLOOR(a.NetSalesAmt * 0.10) PPN
					, 0 TARIF_PPNBM
					, 0 PPNBM
					, b.CUSTOMERCODE 
					, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
					, b.FPJNo
					from spTrnSInvoicedtl a
					inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
								from gntaxout 
								 WHERE CompanyCode = @CompanyCode
										AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
										AND ProductType = @ProductType
										AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
										AND IsPKP = 1
										AND left(fpjno,3)='FPJ'
								) b
						on a.companycode = b.companycode and a.branchcode = b.branchcode
							and a.InvoiceNo = b.referenceNo
					union all
					select 'OF' [OF]
					, a.SalesModelCode KODE_OBJEK
					--,  NAMA
					, 'Sales Model Desc : '+ (select SalesModelDesc from omMstModel where CompanyCode = a.CompanyCode and SalesModelCode = a.SalesModelCode)
					  +' Nomor Rangka : '+ CONVERT(varchar, c.ChassisNo,100)
					  +' Nomor Mesin : '+  CONVERT(varchar, c.EngineNo,100) NAMA
					, isnull ((convert(decimal(12,0),a.BeforeDiscDPP)), 0) HARGA_SATUAN
					, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
					, isnull ((convert(decimal(12,0),a.BeforeDiscDPP)),0) HARGA_TOTAL
					, isnull ((convert(decimal(12,0),a.DiscExcludePPn)), 0) DISKON
					--, isnull ((convert(decimal(12,0),a.AfterDiscDPP)), 0) DPP
					--, isnull ((convert(decimal(12,0), a.BeforeDiscDPP - a.DiscExcludePPn)), 0) DPP
					, a.BeforeDiscDPP - a.DiscExcludePPn DPP
					--, FLOOR(isnull ((convert(decimal(12,0),a.AfterDiscPPn)), 0)) PPN
					, FLOOR(a.AfterDiscPPn) PPN
					, 0 TARIF_PPNBM
					, 0 PPNBM
					, b.CUSTOMERCODE 
					, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) TaxNo
					, b.FPJNo
					from omTrSalesInvoicemodel a
					inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo, DPPAmt
								from gntaxout 
								 WHERE CompanyCode = @CompanyCode
										AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
										AND ProductType = @ProductType
										AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
										AND IsPKP = 1
										AND left(fpjno,3) not in ('FPJ','FPS')
								) b
						on a.companycode = b.companycode and a.branchcode = b.branchcode
							and a.InvoiceNo = b.referenceNo	
					inner join omTrSalesInvoiceVin c on a.CompanyCode = c.CompanyCode
						and a.BranchCode = c.BranchCode
						and a.InvoiceNo = c.InvoiceNo
						and a.BPKNo = c.BPKNo
						and a.SalesModelCode = c.SalesModelCode
						and a.SalesModelYear = c.SalesModelYear
					union all
					select 'OF' [OF]
					, AccountNo KODE_OBJEK
					, Description NAMA
					, isnull ((convert(decimal(12,0),a.UnitPriceAmt)), 0) HARGA_SATUAN
					, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
					, isnull ((convert(decimal(12,0),a.UnitPriceAmt * a.Quantity)),0) HARGA_TOTAL
					, isnull ((convert(decimal(12,0),a.DiscAmt)), 0) DISKON
					--, isnull ((convert(decimal(12,0),a.UnitPriceAmt * a.Quantity)), 0) DPP
					, a.UnitPriceAmt * a.Quantity DPP
					--, FLOOR(isnull ((convert(decimal(12,0),a.PPNAmt)), 0)) PPN
					, FLOOR(a.PPNAmt) PPN
					, 0 TARIF_PPNBM
					, 0 PPNBM
					, b.CUSTOMERCODE 
					, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
					, b.FPJNo
					from arTrnInvoiceDtl a
					inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
								from gntaxout 
								 WHERE CompanyCode = @CompanyCode
										AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
										AND ProductType = @ProductType
										AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
										AND IsPKP = 1
										AND left(fpjno,3) not in ('FPJ','FPS')
								) b
						on a.companycode = b.companycode and a.branchcode = b.branchcode
							and a.InvoiceNo = b.referenceNo) X
					union all 
					select  
						[OF],
						KODE_OBJEK,
						NAMA,
						HARGA_SATUAN,
						JUMLAH_BARANG,
						HARGA_TOTAL,
						DISKON,
						FLOOR(DPP)DPP,
						FLOOR(DPP)*0.1 PPN,
						TARIF_PPNBM,
						PPNBM,
						CUSTOMERCODE,
						NOMOR_FAKTUR,
						FPJNO
					from(
					select 'OF' [OF]
					, a.CompanyCode
					, a.BranchCode
					, rtrim(PartNo) KODE_OBJEK
					, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
					, isnull (convert(decimal(12,2),a.RetailPrice,0),0.00) HARGA_SATUAN
					, isnull (convert(decimal(12,2),a.SupplyQty,0),0.00) JUMLAH_BARANG
					, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty,0),0.00) HARGA_TOTAL
					, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty * DiscPct /100.00,0),0.00) DISKON
					--, isnull (convert(decimal(12,2),(a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100.00),0),0.00) DPP
					, (a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100) DPP
					--, isnull (convert(decimal(12,2),(((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100.00))*0.10),0),0.00) PPN
					, 0 TARIF_PPNBM
					, 0 PPNBM
					, b.CUSTOMERCODE 
					, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
					, b.FPJNo
					, c.countReff
					from svTrnInvItem a
					inner join (select CompanyCode, BranchCode, FPJNo, CustomerCode, ReferenceNo, TaxNo
								from gntaxout 
								 WHERE CompanyCode = @CompanyCode
										AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
										AND ProductType = @ProductType
										AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
										AND IsPKP = 1
										AND (left(fpjno,3)='FPS' OR LEFT(FPJNo,3)='FPH')
								) b
						on a.companycode = b.companycode and a.branchcode = b.branchcode
							and a.InvoiceNo = b.referenceNo	
					inner join (select CompanyCode, BranchCode, TaxNo, COUNT(ReferenceNo) countReff
								from gntaxout 
								 WHERE CompanyCode = @CompanyCode
										AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
										AND ProductType = @ProductType
										AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
										AND IsPKP = 1
										AND (left(fpjno,3)='FPS' OR LEFT(FPJNo,3)='FPH')
								group by CompanyCode, BranchCode, TaxNo
								) c
						on c.CompanyCode = b.CompanyCode and c.branchcode = b.branchcode
							and c.TaxNo = b.TaxNo	
				)Z
-- tambahan untuk jasa service
					union all 
					select  
						[OF],
						KODE_OBJEK,
						NAMA,
						HARGA_SATUAN,
						JUMLAH_BARANG,
						HARGA_TOTAL,
						DISKON,
						FLOOR(DPP)DPP,
						FLOOR(DPP)*0.1 PPN,
						TARIF_PPNBM,
						PPNBM,
						CUSTOMERCODE,
						NOMOR_FAKTUR,
						FPJNO
					from(
					select 'OF' [OF]
					, a.CompanyCode
					, a.BranchCode
					, rtrim(OperationNo) KODE_OBJEK
					, (select top 1 replace(rtrim(Description),',',' ') from svMstTask 
					    where CompanyCode=a.CompanyCode and BasicModel=x.BasicModel and OperationNo=a.OperationNo
					  ) NAMA
					, isnull (convert(decimal(12,2),a.OperationCost,0),0.00) HARGA_SATUAN
					, isnull (convert(decimal(12,2),a.OperationHour,0),0.00) JUMLAH_BARANG
					, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour,0),0.00) HARGA_TOTAL
					, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour * DiscPct /100.00,0),0.00) DISKON
					--, isnull (convert(decimal(12,2),(a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100.00),0),0.00) DPP
					, (a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100) DPP
					--, isnull (convert(decimal(12,2),(((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100.00))*0.10),0),0.00) PPN
					, 0 TARIF_PPNBM
					, 0 PPNBM
					, b.CUSTOMERCODE 
					, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
					, b.FPJNo
					, c.countReff
					from svTrnInvTask a
					inner join svTrnInvoice x
					    on a.CompanyCode=x.CompanyCode and a.BranchCode=x.BranchCode and a.InvoiceNo=x.InvoiceNo
					inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
								from gntaxout 
								 WHERE CompanyCode = @CompanyCode
										AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
										AND ProductType = @ProductType
										AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
										AND IsPKP = 1
										AND (left(fpjno,3)='FPS' OR LEFT(FPJNo,3)='FPH')
								) b
						on a.companycode = b.companycode and a.branchcode = b.branchcode
							and a.InvoiceNo = b.referenceNo	
					inner join (select CompanyCode, BranchCode, TaxNo, COUNT(ReferenceNo) countReff
								from gntaxout 
								 WHERE CompanyCode = @CompanyCode
										AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
										AND ProductType = @ProductType
										AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
										AND IsPKP = 1
										AND (left(fpjno,3)='FPS' OR LEFT(FPJNo,3)='FPH')
								group by CompanyCode, BranchCode, TaxNo
								) c
						on c.CompanyCode = b.CompanyCode and c.branchcode = b.branchcode
							and c.TaxNo = b.TaxNo	
				)Y
			) a 
  --where NOMOR_FAKTUR = @NomorFaktur
	 order by NOMOR_FAKTUR
end

