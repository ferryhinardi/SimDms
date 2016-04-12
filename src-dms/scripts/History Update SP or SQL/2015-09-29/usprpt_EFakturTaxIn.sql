ALTER procedure [dbo].[usprpt_EFakturTaxIn]
--DECLARE
       @CompanyCode varchar(15)
       ,@BranchCode varchar(15)
       ,@PeriodFrom smalldatetime
       ,@PeriodTo smalldatetime
       ,@ProductType varchar(2)
as

--execute usprpt_EFakturTaxIn '6006400001','6006400106','2014/04/01','2014/04/30','4W'
--declare @NomorFaktur as varchar(100)
--set @NomorFaktur = '0011539303581'
--select @CompanyCode=N'6115204001',@BranchCode=N'6115204501',@PeriodFrom='20150701',@PeriodTo='20150731',@ProductType=N'2W'

-- 0= Not Recalculate PPN, 1= Recalculate PPN (For FPJ Gabungan)
declare @IsRecalPPN as bit
set @IsRecalPPN=0

if (select count(ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS') > 0
set @IsRecalPPN= (select convert(bit,ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS')

SELECT 
			FM
             , KD_JENIS_TRANSAKSI
             , FG_PENGGANTI
             , substring(NOMOR_FAKTUR,4,13) NOMOR_FAKTUR
             , MASA_PAJAK
             , TAHUN_PAJAK
             , TANGGAL_FAKTUR
             , NPWP
             , NAMA_LAWAN_TRANSAKSI NAMA
             , REPLACE(REPLACE(ALAMAT_LENGKAP, CHAR(13),''),CHAR(10),'') ALAMAT_LENGKAP
             , sum (JUMLAH_DPP) JUMLAH_DPP
             , SUM(JUMLAH_PPN) JUMLAH_PPN
             --, Floor((sum (JUMLAH_DPP)) * 0.1) JUMLAH_PPN
             , JUMLAH_PPNBM
             , IS_CREDITABLE
      FROM (
             SELECT
                   'FM' FM
                   , LEFT(TaxNo,2) KD_JENIS_TRANSAKSI
                   , 0 FG_PENGGANTI
                   , TaxNo
                   , REPLACE(REPLACE(TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
                   , (case when len(PeriodMonth) = 1 then convert(varchar, PeriodMonth, 1) 
                          else convert(varchar, PeriodMonth, 2) end) MASA_PAJAK
                   , PeriodYear TAHUN_PAJAK
                   , CONVERT(VARCHAR, TaxDate, 103) TANGGAL_FAKTUR
                   , REPLACE(REPLACE(ISNULL(a.NPWPNo, b.NPWPNo), '.', ''), '-', '') NPWP
                   , gnTaxIn.SupplierName NAMA_LAWAN_TRANSAKSI
                   , (SELECT Address1 + ' ' + Address2  + ' ' + Address3  + ' ' + Address4 FROM gnMstSupplier 
                          WHERE COMPANYCODE = gnTaxIn.COMPANYCODE AND SupplierCode = gnTaxIn.SupplierCode) ALAMAT_LENGKAP
                   , JUMLAH_DPP =  isnull(DPPAmt,0)
                   , JUMLAH_PPN = isnull(PPNAmt,0)
                   , JUMLAH_PPNBM = isnull(PPNBmAmt, 0)
                   , 1 IS_CREDITABLE --(select top 1 case topcode 
                   --            when 'D00' then 0 
                   --            when 'C00' then 0
                   --            else 1 end from gnMstSupplierProfitCenter
                   --     where CompanyCode = gnTaxIn.CompanyCode AND BranchCode = gnTaxIn.BranchCode AND SupplierCode = gnTaxIn.SupplierCode
                   --  ) IS_CREDITABLE  
             FROM 
                   gnTaxIn
             LEFT JOIN gnMstCustomer a ON a.CompanyCode = gnTaxIn.CompanyCode
				AND a.CustomerCode = gnTaxIn.SupplierCode
			 LEFT JOIN GnMstSupplier b ON b.CompanyCode = gnTaxIn.CompanyCode
				AND b.SupplierCode = gnTaxIn.SupplierCode	
             WHERE
                   gnTaxIn.CompanyCode = @CompanyCode
                   AND BranchCode like @BranchCode
                   AND ProductType = @ProductType
                   --AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo		--Asli
                   AND PeriodMonth = MONTH(@PeriodFrom)							--Perubahan 
                   AND PeriodYear = YEAR(@PeriodFrom)							--Perubahan 
                   AND DAY(TaxDate) between DAY(@PeriodFrom) and DAY(@PeriodTo)				--Perubahan 
                   AND gnTaxIn.IsPKP = 1
)A
--where substring(NOMOR_FAKTUR,4,13) = @NomorFaktur
group by 
             TaxNo
             ,FM
             , KD_JENIS_TRANSAKSI
             , FG_PENGGANTI
             , substring(NOMOR_FAKTUR,4,13)
             , MASA_PAJAK
             , TAHUN_PAJAK
             , TANGGAL_FAKTUR
             , NPWP
             , NAMA_LAWAN_TRANSAKSI
             , ALAMAT_LENGKAP
             , JUMLAH_DPP
             , JUMLAH_PPN
             , JUMLAH_PPNBM
             , IS_CREDITABLE
order by substring(NOMOR_FAKTUR,4,13)
