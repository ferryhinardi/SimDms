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
--select @CompanyCode=N'6641401',@BranchCodeFrom=N'',@BranchCodeTo=N'',@PeriodFrom='20150701',@PeriodTo='20150731',@ProductType=N'4W', @table='1', 
--@nomorfaktur = '0021563086607'

if @table = 1 or @table = 3
begin
SELECT * INTO #detail FROM(
select [OF],
		CompanyCode,
		BranchCode,
		KODE_OBJEK,
		ProfitCenter,
		NAMA,
		HARGA_SATUAN,
		JUMLAH_BARANG,
		HARGA_TOTAL,
		DISKON,
		FLOOR(DPP) DPP,
		DPP - CONVERT(DECIMAL,FLOOR(DPP)) sisaDPP,
		(DPP*0.1)-CONVERT(DECIMAL, FLOOR((DPP*0.1))) sisaPPN,
		FLOOR((DPP*0.1)) PPN,
		TARIF_PPNBM,
		PPNBM,
		CUSTOMERCODE,
		NOMOR_FAKTUR,
		FPJNO,
		InvoiceNo
from(
					select  
						[OF],
						CompanyCode,
						BranchCode,
						ProfitCenter,
						KODE_OBJEK,
						NAMA,
						HARGA_SATUAN,
						JUMLAH_BARANG,
						HARGA_TOTAL,
						DISKON,
						DPP,
						--PPN,
						TARIF_PPNBM,
						PPNBM,
						CUSTOMERCODE,
						NOMOR_FAKTUR,
						FPJNO,
						InvoiceNo
					from(
					select 'OF' [OF]
					, a.CompanyCode
					, a.BranchCode
					, '300' ProfitCenter
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
					, a.InvoiceNo
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
					, a.CompanyCode
					, a.BranchCode
					, '100' ProfitCenter
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
					, a.InvoiceNo
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
					, a.CompanyCode
					, a.BranchCode
					, '000' ProfitCenter
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
					, a.InvoiceNo
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
						CompanyCode,
						BranchCode,
						ProfitCenter,
						KODE_OBJEK,
						NAMA,
						HARGA_SATUAN,
						JUMLAH_BARANG,
						HARGA_TOTAL,
						DISKON,
						DPP,
						--FLOOR(DPP)*0.1 PPN,
						TARIF_PPNBM,
						PPNBM,
						CUSTOMERCODE,
						NOMOR_FAKTUR,
						FPJNO,
						InvoiceNo
					from(
					select 'OF' [OF]
					, a.CompanyCode
					, a.BranchCode
					, '200' ProfitCenter
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
					, a.InvoiceNo
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
						CompanyCode,
						BranchCode,
						ProfitCenter,
						KODE_OBJEK,
						NAMA,
						HARGA_SATUAN,
						JUMLAH_BARANG,
						HARGA_TOTAL,
						DISKON,
						DPP,
						--FLOOR(DPP)*0.1 PPN,
						TARIF_PPNBM,
						PPNBM,
						CUSTOMERCODE,
						NOMOR_FAKTUR,
						FPJNO,
						InvoiceNo
					from(
					select 'OF' [OF]
					, a.CompanyCode
					, a.BranchCode
					, '200' ProfitCenter
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
					, a.InvoiceNo
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
) b	
order by NOMOR_FAKTUR

DECLARE @CompTemp AS VARCHAR(20),
		@BranchTemp AS VARCHAR(20),
		@NomorFakturTemp AS VARCHAR(100),
		@KodeTempDPP AS VARCHAR(100),
		@KodeTempPPN AS VARCHAR(100),
		@InvoiceNoTemp AS VARCHAR(20),
		@sisaPPN DECIMAL,
		@sisaDPP DECIMAL,
		@ProfCenterTemp AS VARCHAR(3),
		@plusMin AS VARCHAR(1)

DECLARE myCursor cursor for		
SELECT a.CompanyCode, a.BranchCode, a.NOMOR_FAKTUR,
(SELECT TOP 1 KODE_OBJEK FROM #detail WHERE CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and NOMOR_FAKTUR = a.NOMOR_FAKTUR and InvoiceNo = a.InvoiceNo ORDER BY DPP) KodeTempDPP, 
(SELECT TOP 1 KODE_OBJEK FROM #detail WHERE CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and NOMOR_FAKTUR = a.NOMOR_FAKTUR and InvoiceNo = a.InvoiceNo ORDER BY PPN) KodeTempPPN, 
a.InvoiceNo, SUM(sisaPPN) sisaPPN, ROUND(SUM(sisaDPP),0) sisaDPP
FROM #detail a
GROUP BY a.CompanyCode, a.BranchCode, a.NOMOR_FAKTUR, a.InvoiceNo

OPEN myCursor
FETCH NEXT FROM myCursor INTO @CompTemp, @BranchTemp, @NomorFakturTemp, @KodeTempDPP, @KodeTempPPN, @InvoiceNoTemp, @sisaPPN, @sisaDPP

WHILE @@FETCH_STATUS = 0
BEGIN
UPDATE #detail
SET PPN = PPN + ROUND(@sisaPPN,0)
WHERE CompanyCode = @CompTemp AND BranchCode = @BranchTemp AND NOMOR_FAKTUR = @NomorFakturTemp AND KODE_OBJEK = @KodeTempPPN AND InvoiceNo = @InvoiceNoTemp
UPDATE #detail
SET DPP = DPP + ROUND(@sisaDPP,0)
WHERE CompanyCode = @CompTemp AND BranchCode = @BranchTemp AND NOMOR_FAKTUR = @NomorFakturTemp AND KODE_OBJEK = @KodeTempDPP AND InvoiceNo = @InvoiceNoTemp
FETCH NEXT FROM myCursor INTO @CompTemp, @BranchTemp, @NomorFakturTemp, @KodeTempDPP, @KodeTempPPN, @InvoiceNoTemp, @sisaPPN, @sisaDPP
END

CLOSE myCursor
DEALLOCATE myCursor

DECLARE myCursor2 CURSOR FOR 
SELECT a1.CompanyCode, a1.BranchCode, a1.NOMOR_FAKTUR, a1.ProfitCenter, 
(SELECT TOP 1 InvoiceNo FROM #detail WHERE CompanyCode = a1.CompanyCode and BranchCode = a1.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR ORDER BY PPN) InvoiceNo,
(SELECT TOP 1 KODE_OBJEK FROM #detail WHERE CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR 
and InvoiceNo = (SELECT TOP 1 InvoiceNo FROM #detail WHERE CompanyCode = a1.CompanyCode and BranchCode = a1.BranchCode and NOMOR_FAKTUR = a1.NOMOR_FAKTUR ORDER BY PPN)) KodeTempPPN, 
case when a.TotalPpnAmt > a1.JUMLAH_PPN then a.TotalPpnAmt-a1.JUMLAH_PPN else a1.JUMLAH_PPN - a.TotalPpnAmt end selisihPPN,
case when a.TotalPpnAmt > a1.JUMLAH_PPN then '+' else '-' end plusMin
FROM(
SELECT #detail.CompanyCode, #detail.BranchCode, NOMOR_FAKTUR, ProfitCenter, SUM(PPN) JUMLAH_PPN
FROM #detail
GROUP BY #detail.CompanyCode, #detail.BranchCode, NOMOR_FAKTUR, ProfitCenter
) a1
inner join 
(select a.CompanyCode, a.BranchCode, substring(REPLACE(REPLACE( b.FPJGovNo, '.', ''), '-', ''),4,13) TaxNo, SUM(TotalPpnAmt) TotalPpnAmt 
from svTrnInvoice a
inner join svTrnFakturPajak b
on a.CompanyCode = b.CompanyCode
and a.BranchCode = b.BranchCode
and a.FPJNo = b.FPJNo
where 
a.CompanyCode = @CompanyCode
and (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE a.BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
--and substring(REPLACE(REPLACE( b.FPJGovNo, '.', ''), '-', ''),4,13) = @nomorfaktur
group by a.CompanyCode, a.BranchCode, substring(REPLACE(REPLACE( b.FPJGovNo, '.', ''), '-', ''),4,13)) a
on a.CompanyCode = a1.CompanyCode
and a.BranchCode = a1.BranchCode
and a.TaxNo = a1.NOMOR_FAKTUR
where a1.JUMLAH_PPN != a.TotalPpnAmt

OPEN myCursor2
FETCH NEXT FROM myCursor2 INTO @CompTemp, @BranchTemp, @NomorFakturTemp, @ProfCenterTemp, @InvoiceNoTemp, @KodeTempPPN, @sisaPPN, @plusMin

WHILE @@FETCH_STATUS = 0
BEGIN
IF @ProfCenterTemp = '200' and @sisaPPN > 0
BEGIN
	IF @plusMin = '+'
	UPDATE #detail
	SET PPN = PPN + ROUND(@sisaPPN,0)
	WHERE CompanyCode = @CompTemp AND BranchCode = @BranchTemp AND NOMOR_FAKTUR = @NomorFakturTemp AND KODE_OBJEK = @KodeTempPPN AND InvoiceNo = @InvoiceNoTemp
	ELSE
	UPDATE #detail
	SET PPN = PPN - ROUND(@sisaPPN,0)
	WHERE CompanyCode = @CompTemp AND BranchCode = @BranchTemp AND NOMOR_FAKTUR = @NomorFakturTemp AND KODE_OBJEK = @KodeTempPPN AND InvoiceNo = @InvoiceNoTemp
END
FETCH NEXT FROM myCursor2 INTO @CompTemp, @BranchTemp, @NomorFakturTemp, @ProfCenterTemp, @InvoiceNoTemp, @KodeTempPPN, @sisaPPN, @plusMin
END

CLOSE myCursor2
DEALLOCATE myCursor2

if @table = 1
begin
SELECT * INTO #header from(
SELECT 
	FK
	, CompanyCode
	, BranchCode
	, KD_JENIS_TRANSAKSI
	, FG_PENGGANTI
	, NOMOR_FAKTUR
	, MASA_PAJAK
	, TAHUN_PAJAK
	, TANGGAL_FAKTUR
	, NPWP
	, NAMA
	, ALAMAT_LENGKAP
	, CONVERT(DECIMAL,JUMLAH_DPP) JUMLAH_DPP
	, CONVERT(DECIMAL, JUMLAH_PPN) JUMLAH_PPN
	, JUMLAH_PPNBM
	, ID_KETERANGAN_TAMBAHAN
	, FG_UANG_MUKA
	, UANG_MUKA_DPP
	, UANG_MUKA_PPN
	, UANG_MUKA_PPNBM
	, REFERENSI
	, CUSTOMERCODE
FROM(
SELECT FK
, CompanyCode
, BranchCode
, KD_JENIS_TRANSAKSI
, FG_PENGGANTI
, substring(NOMOR_FAKTUR,4,13) NOMOR_FAKTUR
, MASA_PAJAK
, TAHUN_PAJAK
, TANGGAL_FAKTUR
, NPWP
, NAMA_LAWAN_TRANSAKSI NAMA
, REPLACE(REPLACE(ALAMAT_LENGKAP, CHAR(13),''),CHAR(10),'') ALAMAT_LENGKAP
, SUM(JUMLAH_DPP) JUMLAH_DPP
, SUM(JUMLAH_PPN) JUMLAH_PPN
, sum(JUMLAH_PPNBM) JUMLAH_PPNBM
, ID_KETERANGAN_TAMBAHAN
, FG_UANG_MUKA
, UANG_MUKA_DPP
, UANG_MUKA_PPN
, UANG_MUKA_PPNBM
, REFERENSI
, CUSTOMERCODE
, FPJNO
FROM (
SELECT
	'FK' FK
	, CompanyCode
	, BranchCode
	, LEFT(TaxNo,2) KD_JENIS_TRANSAKSI
	, 0 FG_PENGGANTI
	, REPLACE(REPLACE(TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
	, (case when len(PeriodMonth) = 1 then convert(varchar, PeriodMonth, 1) 
		else convert(varchar, PeriodMonth, 2) end) MASA_PAJAK
	, PeriodYear TAHUN_PAJAK
	, CONVERT(VARCHAR, TaxDate, 103) TANGGAL_FAKTUR
	, REPLACE(REPLACE(NPWP, '.', ''), '-', '') NPWP
	, CustomerName NAMA_LAWAN_TRANSAKSI
	, CASE WHEN LEFT(FPJNO,3) = 'FPJ' THEN 
	(SELECT Address1 + ' ' + Address2 FROM SPTRNSFPJINFO WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE AND FPJNO =	gnTaxOut.FPJNO) ELSE  
	  CASE WHEN LEFT(FPJNO,3) = 'FPS' THEN 
	(SELECT Address1 + ' ' + Address2 FROM svTrnFakturPajak WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE AND FPJNO = gnTaxOut.FPJNO) ELSE				(SELECT Address1 + ' ' + Address2 FROM GNMSTCUSTOMER WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE 
	AND CUSTOMERCODE = gnTaxOut.CUSTOMERCODE)	
	  END END ALAMAT_LENGKAP
	, JUMLAH_DPP = 0
	, JUMLAH_PPN = 0
	, PPNBmAmt JUMLAH_PPNBM 
	, '' ID_KETERANGAN_TAMBAHAN
	, 0 FG_UANG_MUKA
	, 0 UANG_MUKA_DPP
	, 0 UANG_MUKA_PPN
	, 0 UANG_MUKA_PPNBM
	, Case when left(FPJNo,3) = 'FPS'
				then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = gnTaxOut.CompanyCode AND a.BranchCode = gnTaxOut.BranchCode AND a.TaxNo = gnTaxOut.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) end)
	       when left(FPJNo,3) = 'FPJ' 
				then (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = gnTaxOut.CompanyCode AND a.BranchCode = gnTaxOut.BranchCode AND a.TaxNo = gnTaxOut.TaxNo) > 1  then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + ReferenceNo + ' (' + FPJNo + ')' + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) end) -- Penambahan
	       when left(FPJNo,3) = 'FPH' then 'No FPJ : ' + FPJNo
	       else (case when (SELECT COUNT(a.TaxNo) FROM gnTaxOut a WHERE a.CompanyCode = gnTaxOut.CompanyCode AND a.BranchCode = gnTaxOut.BranchCode AND a.TaxNo = gnTaxOut.TaxNo) > 1 then 'Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) else 'No Invoice : ' + ReferenceNo + ' Tanggal Jatuh Tempo : ' + convert(varchar(20),SubmissionDate,106) end)
	  end + ' - Cabang ' + SUBSTRING(BranchCode,len(branchcode)-1,2) REFERENSI   
	, CUSTOMERCODE
	, FPJNO
	, ProfitCenter
FROM 
	gnTaxOut 
WHERE
	CompanyCode = @CompanyCode
	AND (CASE WHEN @BranchCodeFrom = '' THEN '' ELSE BranchCode END) BETWEEN @BranchCodeFrom AND @BranchCodeTo
	AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
	AND IsPKP = 1
	AND DocumentType = 'F'
)A
GROUP BY 
FK
, CompanyCode
, BranchCode
, KD_JENIS_TRANSAKSI
, FG_PENGGANTI
, substring(NOMOR_FAKTUR,4,13) 
, MASA_PAJAK
, TAHUN_PAJAK
, TANGGAL_FAKTUR
, NPWP
, NAMA_LAWAN_TRANSAKSI
, REPLACE(REPLACE(ALAMAT_LENGKAP, CHAR(13),''),CHAR(10),'') 
, JUMLAH_DPP
, JUMLAH_PPN
, ID_KETERANGAN_TAMBAHAN
, FG_UANG_MUKA
, UANG_MUKA_DPP
, UANG_MUKA_PPN
, UANG_MUKA_PPNBM
, REFERENSI
, CUSTOMERCODE
, FPJNO
) B
--where NOMOR_FAKTUR = @nomorfaktur
) #header

DECLARE @JUMLAH_DPP AS DECIMAL,
		@JUMLAH_PPN AS DECIMAL

DECLARE myCursor1 cursor for
SELECT CompanyCode, BranchCode, NOMOR_FAKTUR, SUM(DPP) JUMLAH_DPP, SUM(PPN) JUMLAH_PPN
FROM #detail
GROUP BY CompanyCode, BranchCode, NOMOR_FAKTUR

OPEN myCursor1
FETCH NEXT FROM myCursor1 INTO @CompTemp, @BranchTemp, @NomorFakturTemp, @JUMLAH_DPP, @JUMLAH_PPN

WHILE @@FETCH_STATUS = 0
BEGIN
UPDATE #header
SET JUMLAH_DPP = @JUMLAH_DPP, JUMLAH_PPN = @JUMLAH_PPN
WHERE CompanyCode = @CompTemp AND BranchCode = @BranchTemp AND NOMOR_FAKTUR = @NomorFakturTemp 

FETCH NEXT FROM myCursor1 INTO @CompTemp, @BranchTemp, @NomorFakturTemp, @JUMLAH_DPP, @JUMLAH_PPN
END

CLOSE myCursor1
DEALLOCATE myCursor1

SELECT DISTINCT FK
	, KD_JENIS_TRANSAKSI
	, FG_PENGGANTI
	, NOMOR_FAKTUR
	, MASA_PAJAK
	, TAHUN_PAJAK
	, TANGGAL_FAKTUR
	, NPWP
	, NAMA
	, ALAMAT_LENGKAP
	, JUMLAH_DPP
	, JUMLAH_PPN
	, JUMLAH_PPNBM
	, ID_KETERANGAN_TAMBAHAN
	, FG_UANG_MUKA
	, UANG_MUKA_DPP
	, UANG_MUKA_PPN
	, UANG_MUKA_PPNBM
	, REFERENSI
	, CUSTOMERCODE
FROM #header
DROP TABLE #detail, #header
end

if @table = 3
begin
SELECT [OF]
	, KODE_OBJEK
	, NAMA
	, HARGA_SATUAN
	, JUMLAH_BARANG
	, HARGA_TOTAL
	, DISKON
	, DPP
	, PPN
	, TARIF_PPNBM
	, PPNBM
	, CUSTOMERCODE
	, NOMOR_FAKTUR
	, FPJNO
FROM #detail
DROP TABLE #detail
end

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



