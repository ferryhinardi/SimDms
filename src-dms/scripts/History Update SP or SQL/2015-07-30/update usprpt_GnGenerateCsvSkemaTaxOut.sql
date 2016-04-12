ALTER procedure [dbo].[usprpt_GnGenerateCsvSkemaTaxOut]
--DECLARE
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@PeriodFrom smalldatetime		
	,@PeriodTo smalldatetime
	,@ProductType varchar(2)
	,@table varchar(1)=1
as

--execute usprpt_GnGenerateCsvSkemaTaxOut '6115204','611520401','2015-07-01','2015-07-28','2W',3
--select @CompanyCode=N'6058401',@BranchCode=N'605840100',@PeriodFrom='2015-07-01',@PeriodTo='2015-07-30',@ProductType=N'2W', @table='1'

 --0= Not Recalculate PPN, 1= Recalculate PPN (For FPJ Gabungan)
declare @IsRecalPPN as bit
set @IsRecalPPN=0

if @table = 1
begin
	if (select count(ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS') > 0
	set @IsRecalPPN= (select convert(bit,ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS')

SELECT FK
		, KD_JENIS_TRANSAKSI
		, FG_PENGGANTI
		, NOMOR_FAKTUR
		, MASA_PAJAK
		, TAHUN_PAJAK
		, TANGGAL_FAKTUR
		, NPWP
		, NAMA
		, ALAMAT_LENGKAP
		, SUM(JUMLAH_DPP) JUMLAH_DPP
		, CASE WHEN ProfitCenter != '300' THEN sum(JUMLAH_PPN)  else floor(sum(JUMLAH_DPP * 0.1)) end JUMLAH_PPN
		--, floor(sum(JUMLAH_DPP * 0.1)) JUMLAH_PPN
		--, sum(JUMLAH_PPN) JUMLAH_PPN
		, SUM(JUMLAH_PPNBM) JUMLAH_PPNBM
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
			, sum(JUMLAH_DPP) JUMLAH_DPP
			, CASE WHEN LEFT(FPJNO,2) = 'FP' THEN floor(sum(JUMLAH_DPP * 0.1)) else sum(a.JUMLAH_PPN) end JUMLAH_PPN
			, sum(JUMLAH_PPNBM) JUMLAH_PPNBM
			, ID_KETERANGAN_TAMBAHAN
			, FG_UANG_MUKA
			, UANG_MUKA_DPP
			, UANG_MUKA_PPN
			, UANG_MUKA_PPNBM
			, REFERENSI
			, CUSTOMERCODE
			, FPJNO
			, ProfitCenter
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
				, JUMLAH_DPP = (case when left(FPJNO,3) = 'FPJ'
										  then floor((select sum(isnull(NetSalesAmt,0)) from spTrnSInvoicedtl
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
											  	    and InvoiceNo=gnTaxOut.ReferenceNo))
									 when left(FPJNo,3) = 'FPS'
									      then floor((select isnull(sum((RetailPrice * SupplyQty)-(RetailPrice * SupplyQty * DiscPct /100.00)),0) from svTrnInvItem
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo) 
											   +(select isnull(sum((OperationCost * OperationHour)-(OperationCost * OperationHour * DiscPct /100.00)),0) from svTrnInvTask
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo))
									 else DPPAmt end)
				, JUMLAH_PPN = (case when left(FPJNO,3) = 'FPJ'
										  then floor(floor((select sum(isnull(NetSalesAmt,0)) from spTrnSInvoicedtl
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo)))
									 when left(FPJNo,3) = 'FPS'
									      then floor((floor((select isnull(sum((RetailPrice * SupplyQty)-(RetailPrice * SupplyQty * DiscPct /100.00)),0) from svTrnInvItem
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												    and InvoiceNo=gnTaxOut.ReferenceNo) 
											   +(select isnull(sum((OperationCost * OperationHour)-(OperationCost * OperationHour * DiscPct /100.00)),0) from svTrnInvTask
										          where CompanyCode=gnTaxOut.CompanyCode
												    and BranchCode=gnTaxOut.BranchCode
												  and InvoiceNo=gnTaxOut.ReferenceNo))))
									 else PPNAmt end)
				, PPNBmAmt JUMLAH_PPNBM --, (select sum(PPNBmAmt) from gnTaxOut a where a.CompanyCode = CompanyCode and a.BranchCode = BranchCode and a.TaxNo = TaxNo) JUMLAH_PPNBM
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
				AND BranchCode = @BranchCode
				AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
				AND IsPKP = 1
				AND DocumentType = 'F'
				)A
				group by FK, CompanyCode
							, BranchCode,KD_JENIS_TRANSAKSI, FG_PENGGANTI
							, substring(NOMOR_FAKTUR,4,13)
							, MASA_PAJAK, TAHUN_PAJAK, tanggal_faktur, npwp, NAMA_LAWAN_TRANSAKSI, ALAMAT_LENGKAP--, JUMLAH_DPP, JUMLAH_PPNBM
							, ID_KETERANGAN_TAMBAHAN
							, FG_UANG_MUKA
							, UANG_MUKA_DPP
							, UANG_MUKA_PPN
							, UANG_MUKA_PPNBM
							, REFERENSI
							, CUSTOMERCODE
							, FPJNO
							, ProfitCenter
			)B 
GROUP BY FK
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
		, ID_KETERANGAN_TAMBAHAN
		, FG_UANG_MUKA
		, UANG_MUKA_DPP
		, UANG_MUKA_PPN
		, UANG_MUKA_PPNBM
		, REFERENSI
		, CUSTOMERCODE
		, ProfitCenter

end
if @table = 2
begin
	select  'LT' LT, REPLACE(REPLACE(NPWPNo, '.', ''), '-', '') NPWP, CustomerName NAMA, REPLACE(REPLACE(Address1, CHAR(13),''),CHAR(10),'')  + REPLACE(REPLACE(Address2, CHAR(13),''),CHAR(10),'') JALAN, '-' BLOK, '-' NOMOR, '0' RT, '0' RW, 
	KecamatanDistrik KECAMATAN, KelurahanDesa KELURAHAN, KotaKabupaten KABUPATEN, 
	case when isnull(ProvinceCode,'') = '' then '-' else (isnull((select top 1 lookupvaluename from gnmstlookupdtl where codeid='PROV' and LookUpValue = ProvinceCode),'-' ))end PROPINSI, 
	ZipNo KODE_POS, PhoneNo NOMOR_TELEPON, CUSTOMERCODE
	from gnMstCustomer 
	where CustomerCode in 
	(select distinct CustomerCode from gnTaxOut WHERE
					CompanyCode = @CompanyCode
					AND BranchCode = @BranchCode
					AND ProductType = @ProductType
					--AND PeriodYear = @PeriodYear
					--AND PeriodMonth = @PeriodMonth
					AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
					AND IsPKP = 1)
end
if @table = 3
begin
		select * from (
						select 'OF' [OF]
						, PartNo KODE_OBJEK
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						, isnull ((convert(decimal(12,2),a.retailprice)), 0.00) HARGA_SATUAN
						, isnull ((convert(decimal(12,2),a.QtyBill)), 0.00) JUMLAH_BARANG
						, isnull ((convert(decimal(12,2),a.SalesAmt)), 0.00)HARGA_TOTAL
						, isnull ((convert(decimal(12,2),a.DiscAmt)), 0.00) DISKON
						, isnull ((convert(decimal(12,2),a.NetSalesAmt)), 0.00) DPP
						, isnull ((convert(decimal(12,2),a.NetSalesAmt * 0.10)), 0.00) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
						, b.FPJNo
						from spTrnSInvoicedtl a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3)='FPJ'
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo
						union all
						select 'OF' [OF]
						, SalesModelCode KODE_OBJEK
						, (select SalesModelDesc from ommstmodel where companycode = a.companycode and salesmodelcode = a.salesmodelcode) NAMA
						--, 'Sales Model Desc : '+ (select SalesModelDesc from ommstmodel where companycode = a.companycode and salesmodelcode = a.salesmodelcode) + 
						--  +' Nomor Rangka : '+ (select CONVERT(varchar, ChassisNo, 100) from omTrSalesInvoiceVin where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and 
						--						InvoiceNo = a.InvoiceNo and BPKNo = a.BPKNo and SalesModelCode = a.SalesModelCode and SalesModelYear = a.SalesModelYear)
						--  +' Nomor Mesin : '+ (select CONVERT(varchar, EngineNo, 100) from omTrSalesInvoiceVin where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and 
						--						InvoiceNo = a.InvoiceNo and BPKNo = a.BPKNo and SalesModelCode = a.SalesModelCode and SalesModelYear = a.SalesModelYear) NAMA
						, isnull ((convert(decimal(12,0),a.BeforeDiscDPP)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,0),a.BeforeDiscDPP * a.Quantity)),0) HARGA_TOTAL
						, isnull ((convert(decimal(12,0),a.DiscExcludePPn * a.Quantity)), 0) DISKON
						--, isnull ((convert(decimal(12,0),a.AfterDiscDPP * a.Quantity)), 0) DPP
						, DPPAmt
						, isnull ((convert(decimal(12,0),a.AfterDiscPPn * a.Quantity)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) TaxNo
						, b.FPJNo
						from omTrSalesInvoicemodel a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo, DPPAmt
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3) not in ('FPJ','FPS')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						union all
						select 'OF' [OF]
						, AccountNo KODE_OBJEK
						, Description NAMA
						, isnull ((convert(decimal(12,0),a.UnitPriceAmt)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,0),a.Quantity)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,0),a.UnitPriceAmt * a.Quantity)),0) HARGA_TOTAL
						, isnull ((convert(decimal(12,0),a.DiscAmt)), 0) DISKON
						, isnull ((convert(decimal(12,0),a.UnitPriceAmt * a.Quantity)), 0) DPP
						, isnull ((convert(decimal(12,0),a.PPNAmt)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
						, b.FPJNo
						from arTrnInvoiceDtl a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND left(fpjno,3) not in ('FPJ','FPS')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						union all 
						select  
							[OF],
							KODE_OBJEK,
							NAMA,
							HARGA_SATUAN,
							JUMLAH_BARANG,
							HARGA_TOTAL,
							DISKON,
							DPP,
							convert(decimal(12,2),(dpp * 0.1)) PPN,
							TARIF_PPNBM,
							PPNBM,
							CUSTOMERCODE,
							NOMOR_FAKTUR,
							FPJNO
						from(
						select 'OF' [OF]
						, rtrim(PartNo) KODE_OBJEK
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						, isnull (convert(decimal(12,2),a.RetailPrice,0),0.00) HARGA_SATUAN
						, isnull (convert(decimal(12,2),a.SupplyQty,0),0.00) JUMLAH_BARANG
						, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty,0),0.00) HARGA_TOTAL
						, isnull (convert(decimal(12,2),a.RetailPrice * a.SupplyQty * DiscPct /100.00,0),0.00) DISKON
						, isnull (convert(decimal(12,2),(a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100.00),0),0.00) DPP
						--, isnull (convert(decimal(12,2),(((a.RetailPrice * a.SupplyQty)-(a.RetailPrice * a.SupplyQty * DiscPct /100.00))*0.10),0),0.00) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
						, b.FPJNo
						from svTrnInvItem a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											--AND PeriodYear = @PeriodYear
											--AND PeriodMonth = @PeriodMonth
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND (left(fpjno,3)='FPS' OR LEFT(FPJNo,3)='FPH')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
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
							DPP,
							convert(decimal(12,2),(dpp * 0.1)) PPN,
							TARIF_PPNBM,
							PPNBM,
							CUSTOMERCODE,
							NOMOR_FAKTUR,
							FPJNO
						from(
						select 'OF' [OF]
						, rtrim(OperationNo) KODE_OBJEK
						, (select top 1 replace(rtrim(Description),',',' ') from svMstTask 
						    where CompanyCode=a.CompanyCode and BasicModel=x.BasicModel and OperationNo=a.OperationNo
						  ) NAMA
						, isnull (convert(decimal(12,2),a.OperationCost,0),0.00) HARGA_SATUAN
						, isnull (convert(decimal(12,2),a.OperationHour,0),0.00) JUMLAH_BARANG
						, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour,0),0.00) HARGA_TOTAL
						, isnull (convert(decimal(12,2),a.OperationCost * a.OperationHour * DiscPct /100.00,0),0.00) DISKON
						, isnull (convert(decimal(12,2),(a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100.00),0),0.00) DPP
						, isnull (convert(decimal(12,2),(((a.OperationCost * a.OperationHour)-(a.OperationCost * a.OperationHour * DiscPct /100.00))*0.10),0),0.00) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, substring(REPLACE(REPLACE(b.TaxNo, '.', ''), '-', ''),4,13) NOMOR_FAKTUR
						, b.FPJNo
						from svTrnInvTask a
						inner join svTrnInvoice x
						    on a.CompanyCode=x.CompanyCode and a.BranchCode=x.BranchCode and a.InvoiceNo=x.InvoiceNo
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo, TaxNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
											AND IsPKP = 1
											AND (left(fpjno,3)='FPS' OR LEFT(FPJNo,3)='FPH')
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
					)Y
				) a
	 order by NOMOR_FAKTUR
end

