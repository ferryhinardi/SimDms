using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Controllers
{
    public class PengeluaranController:BaseController
    {
        public string FakturPenjualan()
        {
            return HtmlRender("pengeluaran/pembuatanfakturpenjualan.js");
        }

        // Entry Pengeluaran Stock
        public string lnk4001()
        {
            return HtmlRender("pengeluaran/EntryPengeluaranStock.js");
        }
        // Pembuatan PL dan Entry Picked Qty PL
        public string lnk4002()
        {
            return HtmlRender("pengeluaran/PembuatanPLdanEntryPickedQtyPL.js");
        }
 
        // Pembuatan Lampiran dokumen Non Penjualan
        public string lnk4004()
        {
            return HtmlRender("pengeluaran/PembuatanLampirandokumenNonPenjualan.js");
        }
        // Pemeliharaan Faktur Penjualan
        public string lnk4005()
        {
            return HtmlRender("pengeluaran/PemeliharaanFakturPenjualan.js");
        }
        // Entry Return Penjualan
        public string lnk4006()
        {
            return HtmlRender("pengeluaran/EntryReturnPenjualan.js");
        }
        // Lampiran Dokumen Service
        public string lnk4007()
        {
            return HtmlRender("pengeluaran/LampiranDokumenService.js");
        }
        // Entry Return Supply Slip
        public string lnk4008()
        {
            return HtmlRender("pengeluaran/EntryReturnSupplySlip.js");
        }
        // Pembatalan Outstanding BO
        public string lnk4009()
        {
            return HtmlRender("pengeluaran/PembatalanOutstandingBO.js");
        }
        // Entry Return Service
        public string lnk4010()
        {
            return HtmlRender("pengeluaran/EntryReturnService.js");
        }
        // Pembatalan Outstanding BO Keseluruhan
        public string lnk4011()
        {
            return HtmlRender("pengeluaran/PembatalanOutstandingBOKeseluruhan.js");
        }
        // Entry Pengeluaran Stock Branch
        public string lnk4012() 
        {
            return HtmlRender("pengeluaran/EntryPengeluaranStockBrnch.js");
        }
    }
}
