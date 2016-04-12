using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Sparepart.Controllers
{
    public class ReportController : BaseController
    {
        //
        // GET: /Report/

        #region master report
        //Daftar Master Sparepart
        public string lnkx101()
        {
            return HtmlRender("report/master/ListMstSparepart.js");
        }
        //List Master Substitusi
        public string lnkx102()
        {
            return HtmlRender("report/master/LstMstSubstitusi.js");
        }

        //Daftar Perubahan OutStanding
        public string lnkx103()
        {
            return HtmlRender("report/master/LstPerubahanOutsndg.js");
        }

        #endregion

        #region Report Pembelian 
        //Suggestion Order
        public string lnkx201()
        {
            return HtmlRender("report/pembelian/LstSugestionOrder.js");
        }

        //Daftar Pesanan Sparepart
        public string lnkx202()
        {
            return HtmlRender("report/pembelian/LstPesananSparepart.js");
        }

        //Daftar On Order per Part
        public string lnkx203()
        {
            return HtmlRender("report/pembelian/LstOnOrderperPart.js");
        } 

        /// <summary>
        /// Report AOS List
        /// </summary>
        /// <returns></returns>
        public string lnkx204()
        {
            return HtmlRender("report/pembelian/AOSList.js");
        }
        #endregion

        #region penerimaan report
        /* Binning List Pembelian Report */
        public string lnkx301()
        {
            return HtmlRender("report/penerimaan/BinninglistPembelian.js");
        }

        public string lnkx302()
        {
            return HtmlRender("report/penerimaan/WRSPembelian.js");
        }

        public string lnkx303()
        {
            return HtmlRender("report/penerimaan/EntryHPP.js");
        }

        public string lnkx304()
        {
            return HtmlRender("report/penerimaan/ClaimVendorForm.js");
        }

        public string lnkx305()
        {
            return HtmlRender("report/penerimaan/ReceivedClaimVendorForm.js");
        }


        #endregion

        #region Report Inventory
        public string lnkx401()
        {
            return HtmlRender("report/inventory/InventoryAdjustment.js");
        }

        public string lnkx402()
        {
            return HtmlRender("report/inventory/WarehouseTransfer.js");
        }

        public string lnkx403()
        {
            return HtmlRender("report/inventory/ReservedSparepart.js");
        }
        
        #endregion

        #region Report Penjualan

        //Daftar Picking List
        public string lnkx501()
        {
            return HtmlRender("report/penjualan/LstPickingList.js");
        }
        
        //Daftar Pengeluaran Stock
        public string lnkx502()
        {
            return HtmlRender("report/penjualan/LstPengeluaranStok.js");
        }
        
        //Daftar Faktur Penjualan
        public string lnkx503()
        {
            return HtmlRender("report/penjualan/LstFakturPenjualan.js");
        }

        //Daftar Lampiran Dokumen Nonpenjualan
        public string lnkx504()
        {
            return HtmlRender("report/penjualan/lstLampiranDkmnNonPenj.js");
        }

        //Daftar Lampiran Faktur Penjualan
        public string lnkx505()
        {
            return HtmlRender("report/penjualan/lstLampiranFakturPenjualan.js");
        }

        //Daftar Sales Retur Memo
        public string lnkx506()
        {
            return HtmlRender("report/penjualan/LstSalesReturMemo.js");
        }

        //Daftar Cancel Faktur Service
        public string lnkx507()
        {
            return HtmlRender("report/penjualan/LstCancelFakturService.js");
        }

        //Daftar Slip Supply Retur Memo
        public string lnkx508()
        {
            return HtmlRender("report/penjualan/LstSlipSupplyReturMemo.js");
        }
        
        //Daftar Outstanding Supply Slip
        public string lnkx509()
        {
            return HtmlRender("report/penjualan/LstOutstandingSupplySlip.js");
        }

         //Daftar Back Order Bulanan Per Langganan
        public string lnkx510()
        {
            return HtmlRender("report/penjualan/LstBOBulananLangganan.js");
        }

        //Daftar Back Order Harian Per Part
        public string lnkx511()
        {
            return HtmlRender("report/penjualan/LstBOHarianPart.js");
        }

        //Daftar Back Order Harian Langganan
        public string lnkx512()
        {
            return HtmlRender("report/penjualan/LstBOHarianLangganan.js");
        }

        //Daftar Back Order Cancel
        public string lnkx513()
        {
            return HtmlRender("report/penjualan/LstBOCancel.js");
        }

        //Daftar Faktur Penjualan (Pre-printed)
        public string lnkx514()
        {
            return HtmlRender("report/penjualan/LstFakturPenjualanPrePrinted.js");
        }
        
        #endregion

        #region "Report Reports"
        
        //Report -> Reports -> Kartu Stock
        public string lnkx801()
        {
            return HtmlRender("report/reports/KartuStock.js");
        }

        public string lnkx802()
        {
            return HtmlRender("report/reports/DftItemPenjualanTerbaik.js");
        }

        public string lnkx803()
        {
            return HtmlRender("report/reports/DftAnalisaServiceRatioPelanggan.js");
        }

        public string lnkx804()
        {
            return HtmlRender("report/reports/DftPartPerMCABCclassDemand.js");
        }

        public string lnkx805()
        {
            return HtmlRender("report/reports/DftRingkasanServiceRatio.js");
        }

        public string lnkx806()
        {
            return HtmlRender("report/reports/DftPenjualanPerPelanggan.js");
        }

        public string lnkx807()
        {
            return HtmlRender("report/reports/DftBarangPinjam.js");
        }

        public string lnkx808()
        {
            return HtmlRender("report/reports/DftBarangKembali.js");
        }

        public string lnkx809()
        {
            return HtmlRender("report/reports/DftPerubahanHrgPotonganBeliJual.js");
        }

        public string lnkx810()
        {
            return HtmlRender("report/reports/DftPerubahanAverageCost.js");
        }

        public string lnkx811()
        {
            return HtmlRender("report/reports/DftPajakKeluaran.js");
        }

        public string lnkx812()
        {
            return HtmlRender("report/reports/DftPartPerNoPart.js");
        }

        public string lnkx813()
        {
            return HtmlRender("report/reports/FakturSupplySlipUnit.js");
        }

        public string lnkx814()
        {
            return HtmlRender("report/reports/StockInformation.js");
        }

        public string lnkx815()
        {
            return HtmlRender("report/reports/PenjualanClassAreaLokasi.js");
        }

        public string lnkx816()
        {
            return HtmlRender("report/reports/AktivitasTrx.js");
        }

        public string lnkx817()
        {
            return HtmlRender("report/reports/DftOverStock.js");
        }

        public string lnkx818()
        {
            return HtmlRender("report/reports/LabaRugi.js");
        }

        public string lnkx819()
        {
            return HtmlRender("report/reports/RekapitulasiOmzet.js");
        }
        public string lnkx820()
        {
            return HtmlRender("report/reports/LapSparepartBulanan.js");
        }

        public string lnkx821()
        {
            return HtmlRender("report/reports/AnalysisWeekly.js");
        }
        #endregion

        #region posting
        public string lnkx601()
        {
            return HtmlRender("report/posting/AnalisaSPMnrtABCclass.js");
        }
        public string lnkx602()
        {
            return HtmlRender("report/posting/AnalisaSPMnrtMvngCode.js");
        }
        public string lnkx603()
        {
            return HtmlRender("report/posting/MutasiStockBulanan.js");
        }
        #endregion

        #region Stock Taking
        public string lnkx701()
        {
            return HtmlRender("report/stocktaking/InvStockTakingTag.js");
        }
        public string lnkx702()
        {
            return HtmlRender("report/stocktaking/InvStockTakingForm.js");
        }
        public string lnkx703()
        {
            return HtmlRender("report/stocktaking/AnalisaStockTaking.js");
        }
        public string lnkx704()
        {
            return HtmlRender("report/stocktaking/DaftarInvFT.js");
        }
        public string lnkx705()
        {
            return HtmlRender("report/stocktaking/DaftarOutInvFT.js");
        }
        public string lnkx706()
        {
            return HtmlRender("report/stocktaking/DaftarTrxStockTakingHarian.js");
        }
        #endregion

        #region Report Register

        public string lnkx901()
        {
            return HtmlRender("report/register/SupplySlip.js");
        }

        public string lnkx902()
        {
            return HtmlRender("report/register/SupplySlipOutstanding.js");
        }

        public string lnkx903()
        {
            return HtmlRender("report/register/HistSupplySlipOutstanding.js");
        }

        public string lnkx904()
        {
            return HtmlRender("report/register/RegNonSalesInvoiceAttachments.js");
        }

        public string lnkx905()
        {
            return HtmlRender("report/register/RegBonPengeluaranSparePart.js");
        }

        public string lnkx906()
        {
            return HtmlRender("report/register/RegPenjualanTunaiKredit.js");
        }

        public string lnkx907()
        {
            return HtmlRender("report/register/RegBonPengeluaranSparePartOutstanding.js");
        }

        public string lnkx908()
        {
            return HtmlRender("report/register/RegPenjualanPerPelangganTunaiKredit.js");
        }

        public string lnkx909()
        {
            return HtmlRender("report/register/RegisterPenjualan.js");
        }

        public string lnkx910()
        {
            return HtmlRender("report/register/RegFakturPenjualan.js");
        }

        public string lnkx911()
        {
            return HtmlRender("report/register/RegReturPenjualan.js");
        }

        public string lnkx912()
        {
            return HtmlRender("report/register/RegReturSupplySlip.js");
        }

        public string lnkx913()
        {
            return HtmlRender("report/register/RegInvoicePenjualanPerPelanggan.js");
        }

        public string lnkx914()
        {
            return HtmlRender("report/register/RegDetailReturPenjualan.js");
        }

        public string lnkx915()
        {
            return HtmlRender("report/register/RegClaimForm.js");
        }

        public string lnkx916()
        {
            return HtmlRender("report/register/RegClaimCancel.js");
        }

        public string lnkx917()
        {
            return HtmlRender("report/register/RegOustandingClaim.js");
        }

        public string lnkx918()
        {
            return HtmlRender("report/register/RegPesananSparepart.js");
        }

        public string lnkx919()
        {
            return HtmlRender("report/register/RegPenerimaanPembelian.js");
        }

        public string lnkx920()
        {
            return HtmlRender("report/register/RegWarehouseRecievingSheet.js");
        }

        public string lnkx921()
        {
            return HtmlRender("report/register/RegHPPSparepart.js");
        }

        public string lnkx922()
        {
            return HtmlRender("report/register/RegInventoryAdjusment.js");
        }

        public string lnkx923()
        {
            return HtmlRender("report/register/RegWarehouseTransfer.js");

        }

        public string lnkx924()
        {
            return HtmlRender("report/register/RegPendingDocument.js");
        }

        public string lnkx925()
        {
            return HtmlRender("report/register/RegRptForTaxOffice.js");
        }

        public string lnkx926()
        {
            return HtmlRender("report/register/RegRptPelangganOverlimit.js");
        }

        public string lnkx927()
        {
            return HtmlRender("report/register/RegRekonSparepart.js");
        }

        public string lnkx928()
        {
            return HtmlRender("report/register/RegAverageCostChage.js");
        }

        public string lnkx929()
        {
            return HtmlRender("report/register/RegPermohonanSparepart.js");
        }

       

        #endregion

        #region Report Mutation
        public string lnkx1001()
        {
            return HtmlRender("report/mutation/MutationStock.js");
        }
        #endregion
    }
}
