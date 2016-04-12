create procedure [dbo].[usprpt_GnGenerateCsvSkemaTaxOut]
	@CompanyCode varchar(15)
	,@BranchCode varchar(15)
	,@PeriodMonth int
	,@PeriodYear int
	,@ProductType varchar(2)
	,@table varchar(1)=1
as

--select @CompanyCode=N'6354401',@BranchCode=N'%',@PeriodMonth=10,@PeriodYear=2011,@ProductType=N'4W'

-- 0= Not Recalculate PPN, 1= Recalculate PPN (For FPJ Gabungan)
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
			, NAMA_LAWAN_TRANSAKSI
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
			, FPJNO
		FROM (
			SELECT
				'FK' FK
				, LEFT(TaxNo,2) KD_JENIS_TRANSAKSI
				, 0 FG_PENGGANTI
				, REPLACE(REPLACE(TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
				, (case when len(PeriodMonth) = 1 then '0' + convert(varchar, PeriodMonth, 1) + '0' +  convert(varchar, PeriodMonth, 1) 
					else convert(varchar, PeriodMonth, 2)+convert(varchar, PeriodMonth, 2) end) MASA_PAJAK
				, PeriodYear TAHUN_PAJAK
				, CONVERT(VARCHAR, TaxDate, 103) TANGGAL_FAKTUR
				, '''' + REPLACE(REPLACE(NPWP, '.', ''), '-', '') NPWP
				, CustomerName NAMA_LAWAN_TRANSAKSI
				, CASE WHEN LEFT(FPJNO,3) = 'FPJ' THEN 
				(SELECT Address1 + ' ' + Address2 FROM SPTRNSFPJINFO WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE AND FPJNO =	gnTaxOut.FPJNO) ELSE  
				  CASE WHEN LEFT(FPJNO,3) = 'FPS' THEN 
				(SELECT Address1 + ' ' + Address2 FROM svTrnFakturPajak WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE AND FPJNO = gnTaxOut.FPJNO) ELSE				(SELECT Address1 + ' ' + Address2 FROM GNMSTCUSTOMER WHERE COMPANYCODE = gnTaxOut.COMPANYCODE AND BRANCHCODE = gnTaxOut.BRANCHCODE 
				AND CUSTOMERCODE = gnTaxOut.CUSTOMERCODE)	
				  END END ALAMAT_LENGKAP
				, ISNULL(DPPAmt, 0) JUMLAH_DPP
				, ISNULL(PPNAmt, 0) JUMLAH_PPN
				, ISNULL(PPNBmAmt, 0) JUMLAH_PPNBM
				, '' ID_KETERANGAN_TAMBAHAN
				, 0 FG_UANG_MUKA
				, 0 UANG_MUKA_DPP
				, 0 UANG_MUKA_PPN
				, 0 UANG_MUKA_PPNBM
				, '' REFERENSI
				, CUSTOMERCODE
				, FPJNO
			FROM 
				gnTaxOut 
			WHERE
				CompanyCode = @CompanyCode
				AND BranchCode like @BranchCode
				AND ProductType = @ProductType
				AND PeriodYear = @PeriodYear
				AND PeriodMonth = @PeriodMonth
				AND IsPKP = 1
)A
end
if @table = 2
begin
	select  'LT' LT, NPWPNo NPWP, CustomerName NAMA, Address1 + Address2 JALAN, '-' BLOK, '-' NOMOR, '-' RT, '-' RW, 
	KecamatanDistrik KECAMATAN, KelurahanDesa KELURAHAN, KotaKabupaten KABUPATEN, 
	case when isnull(ProvinceCode,'') = '' then '-' else (select top 1 lookupvaluename from gnmstlookupdtl where LookUpValue = ProvinceCode )end PROPINSI, 
	ZipNo KODE_POS, PhoneNo NOMOR_TELEPON, CUSTOMERCODE
	from gnMstCustomer 
	where CustomerCode in 
	(select distinct CustomerCode from gnTaxOut WHERE
					CompanyCode = @CompanyCode
					AND BranchCode like @BranchCode
					AND ProductType = @ProductType
					AND PeriodYear = @PeriodYear
					AND PeriodMonth = @PeriodMonth
					AND IsPKP = 1)
end
if @table = 3
begin
		select * from (
						select 'OF' [OF]
						, PartNo KODE_OBJEK
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						, isnull ((convert(decimal(12,2),a.retailprice)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,2),a.QtyBill)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,2),a.SalesAmt)), 0)HARGA_TOTAL
						, isnull ((convert(decimal(12,2),a.DiscAmt)), 0) DISKON
						, isnull ((convert(decimal(12,2),a.NetSalesAmt)), 0) DPP
						, isnull ((convert(decimal(12,2),a.PPNAmt)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, b.FPJNO
						from spTrnSInvoicedtl a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											AND PeriodYear = @PeriodYear
											AND PeriodMonth = @PeriodMonth
											AND IsPKP = 1
											AND left(fpjno,3)='FPJ'
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo
						union all
						select 'OF' [OF]
						, SalesModelCode KODE_OBJEK
						, (select SalesModelDesc from ommstmodel where companycode = a.companycode and salesmodelcode = a.salesmodelcode) NAMA
						, isnull ((convert(decimal(12,2),a.BeforeDiscDPP)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,2),a.Quantity)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,2),a.BeforeDiscDPP * a.Quantity)),0) HARGA_TOTAL
						, isnull ((convert(decimal(12,2),a.DiscExcludePPn * a.Quantity)), 0) DISKON
						, isnull ((convert(decimal(12,2),a.AfterDiscTotal * a.Quantity)), 0) DPP
						, isnull ((convert(decimal(12,2),a.AfterDiscPPn * a.Quantity)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, b.FPJNO
						from omTrSalesInvoicemodel a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode = @BranchCode
											AND ProductType = @ProductType
											AND PeriodYear = @PeriodYear
											AND PeriodMonth = @PeriodMonth
											AND IsPKP = 1
											AND left(fpjno,3)<>'FPJ' AND left(fpjno,3)<>'FPS' 
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	
						union all 
						select 'OF' [OF]
						, rtrim(PartNo) KODE_OBJEK
						, (select replace(rtrim(Partname),',',' ') from spmstiteminfo where companycode = a.companycode and partno = a.partno) NAMA
						, isnull ((convert(decimal(12,2),a.RetailPrice)), 0) HARGA_SATUAN
						, isnull ((convert(decimal(12,2),a.SupplyQty)), 0) JUMLAH_BARANG
						, isnull ((convert(decimal(12,2),a.RetailPrice * a.SupplyQty)), 0)HARGA_TOTAL
						, isnull ((convert(decimal(12,2),((a.RetailPrice * a.SupplyQty)*DiscPct)/100 )), 0) DISKON
						, isnull ((convert(decimal(12,2),(a.RetailPrice * a.SupplyQty) - (((a.RetailPrice * a.SupplyQty)*DiscPct)/100))), 0) DPP
						, isnull ((convert(decimal(12,2),(a.RetailPrice * a.SupplyQty) - (((a.RetailPrice * a.SupplyQty)*DiscPct)/100)* 0.1)), 0) PPN
						, 0 TARIF_PPNBM
						, 0 PPNBM
						, b.CUSTOMERCODE 
						, b.FPJNO
						from svTrnInvItem a
						inner join (select companycode, branchcode, fpjno, customercode, referenceNo
									from gntaxout 
									 WHERE CompanyCode = @CompanyCode
											AND BranchCode like @BranchCode
											AND ProductType = @ProductType
											AND PeriodYear = @PeriodYear
											AND PeriodMonth = @PeriodMonth
											AND IsPKP = 1
											AND left(fpjno,3)='FPS' 
									) b
							on a.companycode = b.companycode and a.branchcode = b.branchcode
								and a.InvoiceNo = b.referenceNo	

				) a
	 order by FPJNO
end

