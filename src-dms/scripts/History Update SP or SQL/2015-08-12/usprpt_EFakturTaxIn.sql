if object_id('usprpt_EFakturTaxIn') is not null
       drop procedure usprpt_EFakturTaxIn
GO

Create procedure [dbo].[usprpt_EFakturTaxIn]
       @CompanyCode varchar(15)
       ,@BranchCode varchar(15)
       ,@PeriodFrom smalldatetime
       ,@PeriodTo smalldatetime
       ,@ProductType varchar(2)
as

--execute usprpt_EFakturTaxIn '6006400001','6006400106','2014/04/01','2014/04/30','4W'
--select @CompanyCode=N'6006400101',@BranchCode=N'6006400101',@PeriodFrom='2014-04-01',@PeriodTo='2014-04-30',@ProductType=N'4W'

-- 0= Not Recalculate PPN, 1= Recalculate PPN (For FPJ Gabungan)
declare @IsRecalPPN as bit
set @IsRecalPPN=0

       if (select count(ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS') > 0
       set @IsRecalPPN= (select convert(bit,ParaValue) from gnMstLookUpDtl where CodeID='CPPN' and LookUpValue='STATUS')

              SELECT FM
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
                     , Floor((sum (JUMLAH_DPP)) * 0.1) JUMLAH_PPN
                     , JUMLAH_PPNBM
                     , IS_CREDITABLE
              FROM (
                     SELECT
                           'FM' FM
                           , LEFT(TaxNo,2) KD_JENIS_TRANSAKSI
                           , 0 FG_PENGGANTI
                           , REPLACE(REPLACE(TaxNo, '.', ''), '-', '') NOMOR_FAKTUR
                           , (case when len(PeriodMonth) = 1 then convert(varchar, PeriodMonth, 1) 
                                  else convert(varchar, PeriodMonth, 2) end) MASA_PAJAK
                           , PeriodYear TAHUN_PAJAK
                           , CONVERT(VARCHAR, TaxDate, 103) TANGGAL_FAKTUR
                           , REPLACE(REPLACE(NPWP, '.', ''), '-', '') NPWP
                           , SupplierName NAMA_LAWAN_TRANSAKSI
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
                     WHERE
                           CompanyCode = @CompanyCode
                           AND BranchCode like @BranchCode
                           AND ProductType = @ProductType
                           AND CONVERT(VARCHAR, TaxDate, 112) BETWEEN @PeriodFrom AND @PeriodTo 
                           AND IsPKP = 1
)A
group by 
                     FM
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
go
