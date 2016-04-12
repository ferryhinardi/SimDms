using SimDms.Common;
using SimDms.Service.Models.Reports;
using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Service
{
    public class txtSvRpTrn001 : IRptProc
    {
        private SimDms.Service.Models.DataContext ctx = new SimDms.Service.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public SysUser CurrentUser { get; set; }
        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            var dt = ctx.Database.SqlQuery<SvRpTrn001>(string.Format("exec {0} {1}", sproc, sparam));            
            return CreateReportSvRpTrn001("SvRpTrn001", dt.ToList(), "", printerloc, print, "", fullpage);

        }

        private string CreateReportSvRpTrn001(string rptId, List<SvRpTrn001> dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            string[] pemilik = new string[10];
            string[] fakturKepada = new string[10];
            string[] alamat = new string[10];
            string[] telp = new string[10];
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(rptId, printerLoc, "W163A", print, fileLocation, fullPage);



            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            dt.ForEach(x =>
            {
                //SimDms. ReplaceNullable(x);               

                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader(x.DeskrispiPekerjaan == null ? "" : (x.DeskrispiPekerjaan.ToString() != "") ? x.DeskrispiPekerjaan.ToString() : "", 134, ' ');
                gtf.SetGroupHeader((x.Nomor.ToString() != "") ? "P" + "\x1B\x45" + x.Nomor.ToString() : "", 18, ' ', false, true);
                gtf.SetGroupHeader(" ", 1, ' ');
                gtf.SetGroupHeader("M" + "\x1B\x46" + "-", 166, '-');
                gtf.SetGroupHeader(" ", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Pemilik", 8, ' ');
                gtf.SetGroupHeader(":", 1, ' ', true);

                pemilik = gtf.ConvertArrayText((x.Pemilik.ToString() != "") ? x.Pemilik.ToString() : "", 38);
                gtf.SetGroupHeader(pemilik[0] != null ? pemilik[0].ToString() : "", 38, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Tgl.Penyerahan", 25, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader("Faktur Kepada", 53, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Tgl./Jam Masuk", 25, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(10);
                gtf.SetGroupHeader(pemilik[1] != null ? pemilik[1].ToString() : "", 38, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader((x.TglPenyerahan.ToString() != "") ? Convert.ToDateTime(x.TglPenyerahan.ToString()).ToString("dd-MMM-yyyy") : "", 25, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');

                gtf.SetGroupHeaderSpace(2);
                fakturKepada = gtf.ConvertArrayText((x.FakturKepada.ToString() != "") ? x.FakturKepada.ToString() : "", 53);
                gtf.SetGroupHeader(fakturKepada[0] != null ? fakturKepada[0].ToString() : "", 53, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader((x.TglJamMasuk.ToString() != "") ? Convert.ToDateTime(x.TglJamMasuk.ToString()).ToString("dd-MMM-yyyy") : "", 25, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                alamat = gtf.ConvertArrayText((x.Alamat.ToString() != "") ? x.Alamat.ToString() : "", 38);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Alamat", 8, ' ');
                gtf.SetGroupHeader(":", 1, ' ', true);
                gtf.SetGroupHeader(alamat[0] != null ? alamat[0].ToString() : "", 38, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 26, '-');
                gtf.SetGroupHeader("|", 1, ' ');

                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader(fakturKepada[1] != null ? fakturKepada[1].ToString() : "", 53, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader((x.TglJamMasuk.ToString() != "") ? Convert.ToDateTime(x.TglJamMasuk.ToString()).ToString("HH:mm") : "", 25, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(10);
                gtf.SetGroupHeader(alamat[1] != null ? alamat[1].ToString() : "", 38, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Warna", 25, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');

                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader(fakturKepada[2] != null ? fakturKepada[2].ToString() : "", 53, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 26, '-');
                //
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(10);
                gtf.SetGroupHeader(alamat[2] != null ? alamat[2].ToString() : "", 38, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader(x.Warna == null ? "" : (x.Warna.ToString() != "") ? x.Warna.ToString() : "", 25, ' ', true);//
                gtf.SetGroupHeader("|", 1, ' ');

                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader(fakturKepada[3] != null ? fakturKepada[3].ToString() : "", 53, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Tgl./Jam Keluar", 25, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(10);
                gtf.SetGroupHeader(alamat[3] != null ? alamat[3].ToString() : "", 38, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 26, '-');
                gtf.SetGroupHeader("|", 1, ' ');

                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader(fakturKepada[4] != null ? fakturKepada[4].ToString() : "", 53, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("", 25, ' ', true);
                //gtf.SetGroupHeader("-", 18, '-');
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Telp", 8, ' ');
                gtf.SetGroupHeader(":", 1, ' ', true);

                telp = gtf.ConvertArrayText((x.PhoneNo.ToString() != "" && x.HPNo.ToString() != "") ? x.PhoneNo.ToString() + " / " + x.HPNo.ToString() : ((x.PhoneNo.ToString() != "") ? x.PhoneNo.ToString() : x.HPNo.ToString()), 38);
                gtf.SetGroupHeader(telp[0] != null ? telp[0].ToString() : "", 38, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("No. Polisi", 25, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader(fakturKepada[5] != null ? fakturKepada[5].ToString() : "", 53, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader(" ", 25, ' ', true);
                //gtf.SetGroupHeader("KM.", 17, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(10);
                gtf.SetGroupHeader(telp[1] != null ? telp[1].ToString() : "", 38, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader((x.NoPolisi.ToString() != "") ? x.NoPolisi.ToString() : "", 25, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');

                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader(fakturKepada[6] != null ? fakturKepada[6].ToString() : "", 53, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 26, '-');
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(10);
                gtf.SetGroupHeader(telp[2] != null ? telp[2].ToString() : "", 38, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader(" ", 25, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');

                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader(fakturKepada[7] != null ? fakturKepada[7].ToString() : "", 53, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader((x.KM.ToString() != "") ? "KM. " + Convert.ToDecimal(x.KM.ToString()).ToString("n0") : "KM. ", 25, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 161, '-');
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("No. Chassis", 23, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("No. Mesin", 22, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("DEALER", 81, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Sign Test", 11, ' ');
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Frontman", 14, ' ');
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader((x.NoChasis.ToString() != "") ? x.NoChasis.ToString() : "", 23, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader((x.NoMesin.ToString() != "") ? x.NoMesin.ToString() : "", 22, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader((x.Dealer.ToString() != "") ? x.Dealer.ToString() + (x.ProductionYear == null ? "" : (x.ProductionYear.ToString() != "") ? " (" + x.ProductionYear.ToString() + ")" : "") : "", 81, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader(" ", 11, ' ');
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader((x.Frontman.ToString() != "") ? x.Frontman.ToString() : "", 14, ' ');
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(" ", 23, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("", 22, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("", 81, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader(" ", 11, ' ');
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("", 14, ' ');
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 161, '-');
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("L", 2, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Langganan", 18, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("P", 2, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Periksa", 17, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Keluhan", 108, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("-", 49, '-');
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(x.JenisPekerjaan == null ? "" : (x.JenisPekerjaan.ToString() != "") ? x.JenisPekerjaan.ToString() : "", 108, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                string[] keluhan = gtf.ConvertArrayList(x.Keluhan.ToString(), 108, 7);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("I", 2, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Intern", 18, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("O", 2, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Overhoul", 17, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(keluhan[0] != null ? keluhan[0].ToString().Replace('\r', ' ') : "", 108, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("-", 49, '-');
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(keluhan[1] != null ? keluhan[1].ToString().Replace('\r', ' ') : "", 108, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("W", 2, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Warranty", 18, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("S", 2, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Setel", 17, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(keluhan[2] != null ? keluhan[2].ToString().Replace('\r', ' ') : "", 108, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("-", 49, '-');
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(keluhan[3] != null ? keluhan[3].ToString().Replace('\r', ' ') : "", 108, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("A", 2, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Asuransi", 18, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("G", 2, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Ganti", 17, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(keluhan[4] != null ? keluhan[4].ToString().Replace('\r', ' ') : "", 108, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("-", 49, '-');
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(keluhan[5] != null ? keluhan[5].ToString().Replace('\r', ' ') : "", 108, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("P", 2, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Pek. Kembali", 18, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("C", 2, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("........... KM", 17, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(keluhan[6] != null ? keluhan[6].ToString().Replace('\r', ' ') : "", 108, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 161, '-');
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(" ", 2, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("No. Operasi", 18, ' ', true, false, false, true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Pekerjaan", 88, ' ', true, false, false, true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(" FRT  ", 6, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(" MEK. ", 6, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Keterangan", 24, ' ');
                gtf.SetGroupHeader("|", 1, ' ', false, true, false, true);

                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 161, '-');
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                for (int i = 0; i < 12; i++)
                {
                    gtf.SetGroupHeader("|", 2, ' ');
                    gtf.SetGroupHeader(" ", 2, ' ', true);
                    gtf.SetGroupHeader("|", 2, ' ');
                    gtf.SetGroupHeader(" ", 18, ' ', true);
                    gtf.SetGroupHeader("|", 2, ' ');
                    gtf.SetGroupHeader(" ", 88, ' ', true);
                    gtf.SetGroupHeader("|", 2, ' ');
                    gtf.SetGroupHeader(" ", 6, ' ', true);
                    gtf.SetGroupHeader("|", 2, ' ');
                    gtf.SetGroupHeader(" ", 6, ' ', true);
                    gtf.SetGroupHeader("|", 2, ' ');
                    gtf.SetGroupHeader(" ", 25, ' ');
                    gtf.SetGroupHeader("|", 1, ' ', false, true);
                }

                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 161, '-');
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader(x.PenggantianSukuCadang.ToString() != "" ? (!Convert.ToBoolean(x.PenggantianSukuCadang.ToString()) ? "PENGGANTIAN SUKU CADANG = LANGSUNG" : "PENGGANTIAN SUKU CADANG = IZIN PEMILIK") : "", 161, ' ', false, false, false, true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 161, '-');
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                string message = "Dengan ini kami memberi kuasa penuh kepada " + x.CompanyName.ToString() + " untuk mengerjakan segala pekerjaan yang tertulis pada order ini. Dan juga kami memberikan izin untuk mencoba kendaraan tersebut diluar bengkel " + x.CompanyName.ToString();
                string[] listMessage = gtf.ConvertArrayText(message, 161);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader(listMessage[0] != null ? listMessage[0].ToString() : "", 161, ' ');
                gtf.SetGroupHeader("|", 1, ' ', false, true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader(listMessage[1] != null ? listMessage[1].ToString() : "", 161, ' ');
                gtf.SetGroupHeader("|", 1, ' ', false, true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 161, '-');
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Perhatian :", 100, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Pelanggan", 30, ' ');
                gtf.SetGroupHeader("TANGGAL : " + DateTime.Now.ToString("dd-MMM-yyyy"), 27, ' ');
                gtf.SetGroupHeader("|", 1, ' ', false, true);


                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader("Kendaraan yang sudah selesai & belum diambil :", 100, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(57);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                string[] perhatian1 = gtf.ConvertArrayText("Dalam jangka waktu 1 (satu) bulan kami tidak bertanggungjawab atas kerusakan lain dan kehilangan benda yang tertinggal dalam kendaraan yang disebabkan karena hal-hal diluar kemampuan kami.", 100);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(perhatian1[0] != null ? "1. " + perhatian1[0].ToString() : "1. ", 100, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(57);
                gtf.SetGroupHeader("|", 1, ' ', false, true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(3);
                gtf.SetGroupHeader(perhatian1[1] != null ? perhatian1[1].ToString() : "", 97, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(57);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                string[] perhatian2 = gtf.ConvertArrayText("Kami tidak bisa bertanggung jawab atas kerusakan yang disebabkan adanya bencana alam / huru hara.", 97);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeader(perhatian2[0] != null ? "2. " + perhatian2[0].ToString() : "2. ", 100, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(57);
                gtf.SetGroupHeader("|", 1, ' ', false, true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(3);
                gtf.SetGroupHeader(perhatian2[1] != null ? perhatian2[1].ToString() : "", 97, ' ', true);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(57);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(101);
                gtf.SetGroupHeader("|", 2, ' ');
                gtf.SetGroupHeaderSpace(57);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader(" ", 1, ' ');
                gtf.SetGroupHeader("-", 161, '-');
                gtf.SetGroupHeader(" ", 1, ' ', false, true);

                gtf.PrintHeader();
                //gtf.PrintTotal(false, false, false, true);
            }
                );


            if (print == true)
                gtf.PrintTotal(true, false, false);
            else
            {
                if (gtf.PrintTotal(true, false, false) == true)
                    //XMessageBox.ShowInformation("Save Berhasil");

                    return gtf.sbDataTxt.ToString();
                else
                    //  XMessageBox.ShowWarning("Save Gagal");
                    return gtf.sbDataTxt.ToString();
            }


            if (print == true)
                gtf.CloseConnectionPrinter();
            return "";
        }



       
    }


    public class txtSvRpTrn00101 : IRptProc
    {
        private SimDms.Service.Models.DataContext ctx = new SimDms.Service.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public SysUser CurrentUser { get; set; }
        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            var dt = ctx.Database.SqlQuery<SvRpTrn00101>(string.Format("exec {0} {1}", sproc, sparam));
            return CreateReportSvRpTrn00101("SvRpTrn00101", dt.ToList(), paramReport, printerloc, print, "", fullpage);

        }

        private string CreateReportSvRpTrn00101(string rptId, List<SvRpTrn00101> dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            if (dt.Count() == 0)
                return "Tidak Ada Data";

            var hdr=dt[0];
            string[] arrayHeader = new string[24];
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter("");
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(rptId, printerLoc, "W163", print, fileLocation, fullPage);
            gtf.GenerateHeader();




            ////----------------------------------------------------------------
            ////Header
            //string alamat = (dt[0].Address1 != null ? dt[0].Address1.ToString() : "") + " " + (dt[0].Address2 != null ? dt[0].Address2.ToString() : "") + " "
            //  + (dt[0].Address3 != null ? dt[0].Address3.ToString() : "") + " " + (dt[0].Address4 != null ? dt[0].Address4.ToString() : "");
            //string[] listAlamat = gtf.ConvertArrayText(alamat, 32);

            //gtf.SetGroupHeaderSpace(10);

            gtf.SetGroupHeader("Kepada: ",8);            
            gtf.SetGroupHeader(hdr.CustomerName, 125, ' ', false);
            gtf.SetGroupHeader("No         : ", 13,' ',false);
            gtf.SetGroupHeader(hdr.EstimationNo, 25, ' ', false,true);

            gtf.SetGroupHeaderSpace(8);
            gtf.SetGroupHeader(hdr.Address1, 125, ' ', false);
            gtf.SetGroupHeader("Tanggal    : ", 13, ' ', false);
            gtf.SetGroupHeader(hdr.EstimationDate.Value.ToString("dd-MMM-yyyy"), 25, ' ', false, true);

            gtf.SetGroupHeaderSpace(8);
            gtf.SetGroupHeader(hdr.Address2, 125, ' ', false);
            gtf.SetGroupHeader("No Polisi  : ", 13, ' ', false);
            gtf.SetGroupHeader(hdr.PoliceRegNo, 25, ' ', false, true);

            gtf.SetGroupHeaderSpace(8);
            gtf.SetGroupHeader(hdr.Address3, 125, ' ', false);
            gtf.SetGroupHeader("Model/Type : ", 13, ' ',false);
            gtf.SetGroupHeader(hdr.BasicModel, 25, ' ', false, true);

            gtf.SetGroupHeaderSpace(8);
            gtf.SetGroupHeader(hdr.Address4, 125, ' ', false,true);

            gtf.SetGroupHeader("",0, ' ', false, true);
            
            gtf.SetGroupHeader("Bersama ini kami sampaikan perkiraan biaya perbaikan kendaraan Bapak/Ibu dengan perincian sbb :", 95,' ',false,true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("No.   Kode                    Keterangan                                                                                                                      Total", 164, ' ', false, true);
            gtf.SetGroupHeaderLine();

            gtf.PrintHeader();

            string stype="";
            int cntr = 0;
            Action<SvRpTrn00101> subtotal = (x) =>
            {
                gtf.SetDataDetailSpace(140);
                gtf.SetDataDetail("-----------------------", 23, ' ', false, true);


                gtf.SetDataDetailSpace(140);
                gtf.SetDataDetail(x.TotalGrossAmt.Value == 0 ? "0" : x.TotalGrossAmt.Value.ToString("###,###"), 23, ' ', false, true, true);
                

                gtf.SetDataDetailSpace(124);
                gtf.SetDataDetail("Potongan      : ", 16, ' ', false, false);
                gtf.SetDataDetail(x.Potongan.Value == 0 ? "0" : x.Potongan.Value.ToString("###,###"), 23, ' ', false, true, true);
            };

            SvRpTrn00101 dtl = new SvRpTrn00101();
            for (int i = 0; i < dt.Count; i++)
            {
                dtl=dt[i];
                if(stype!=dtl.Flag)
                {
                    if (stype != "")
                    {
                        subtotal(dt[i-1]);
                        gtf.SetDataDetailLineBreak();
                    }

                    cntr=0;
                    gtf.SetDataDetail(dtl.Flag, dtl.Flag.Length, ' ', false, true);
                    gtf.SetDataDetail("-", dtl.Flag.Length, '-', false, true);
                    stype = dtl.Flag;
                }
                cntr++;
                gtf.SetDataDetail(cntr.ToString(), 6, ' ', false);
                gtf.SetDataDetail(dtl.Kode, 24, ' ', false);
                gtf.SetDataDetail(dtl.Keterangan, 110, ' ', false);
                gtf.SetDataDetail(dtl.TotalPrice.Value==0?"0": dtl.TotalPrice.Value.ToString("###,###"), 23, ' ', false, true, true);                             
            }
            subtotal(dtl);
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();            
            gtf.SetTotalDetail("", 124, ' ',false,false);
            gtf.SetTotalDetail("Jumlah        : ", 16, ' ', false, false);
            gtf.SetTotalDetail(dtl.TotalDPPAmount.Value == 0 ? "0" : dtl.TotalDPPAmount.Value.ToString("###,###"), 23, ' ', false, true, true);
            
            gtf.SetTotalDetail("", 124, ' ', false, false);
            gtf.SetTotalDetail("PPN (10.00 %) : ", 16, ' ', false, false);
            gtf.SetTotalDetail(dtl.TotalPpnAmount.Value == 0 ? "0" : dtl.TotalPpnAmount.Value.ToString("###,###"), 23, ' ', false, true, true);

            gtf.SetTotalDetail("", 124, ' ', false, false);
            gtf.SetTotalDetail("Total         : ", 16, ' ', false, false);
            gtf.SetTotalDetail(dtl.TotalSrvAmount.Value == 0 ? "0" : dtl.TotalSrvAmount.Value.ToString("###,###"), 23, ' ', false, true, true);

            gtf.SetTotalDetail("", 1, ' ', false, true);
            gtf.SetTotalDetail("", 1, ' ', false, true);

            gtf.SetTotalDetail("", 124, ' ', false, false);
            gtf.SetTotalDetail(dtl.City+","+dtl.Tanggal.Value.ToString("dd-MMM-yyyy"), 39, ' ', false, true);
            gtf.SetTotalDetail("", 1, ' ', false, true);
            gtf.SetTotalDetail("", 1, ' ', false, true);
            gtf.SetTotalDetail("", 1, ' ', false, true);
            gtf.SetTotalDetail("", 124, ' ', false,false);
            var signer = paramReport.Split(',');

            gtf.SetTotalDetail("", 124, ' ', false, false);
            gtf.SetTotalDetail(signer[0], 30, ' ', false, true);

            gtf.SetTotalDetail("Note : Perkiraan harga tersebut di atas sewaktu-waktu dapat berubah", 124, ' ', false, false);
            gtf.SetTotalDetail("", 30, '-', false, true);   
            gtf.SetTotalDetail("       tanpa pemberitahuan lebih dahulu", 124, ' ', false, false);
            gtf.SetTotalDetail(signer[1], 30, ' ', false, false);

            //gtf.PrintData();
            gtf.PrintTotal(true, false, false);
                

            

            return gtf.sbDataTxt.ToString();            
        }
    }
}