using SimDms.Common;
using SimDms.Service.Models.Reports;
using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Service
{
    public class txtSvRpTrn001PrePrinted : IRptProc
    {
        private SimDms.Service.Models.DataContext ctx = new SimDms.Service.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public SysUser CurrentUser { get; set; }
        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            var dt = ctx.Database.SqlQuery<SvRpTrn001>(string.Format("exec {0} {1}", sproc, sparam));
            return CreateReportSvRpTrn001PrePrinted(rptId, dt, sparam, printerloc, print, "", fullpage);


        }

        private string CreateReportSvRpTrn001PrePrinted(string recordId, IEnumerable<SvRpTrn001> dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
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
            gtf.GenerateTextFileReports(recordId, printerLoc, "W163A", print, fileLocation, fullPage);


            //gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            //for (int z = 0; z < dt.Count; z++)
            var tdt = dt.ToList();
            dt.ToList().ForEach(x =>
            {
                gtf.SetGroupHeader(" ", 1, ' ', false, true);//1
                gtf.SetGroupHeader(" ", 1, ' ', false, true);//2
                gtf.SetGroupHeader(" ", 1, ' ', false, true);//3
                gtf.SetGroupHeader(" ", 1, ' ', false, true);//4
                gtf.SetGroupHeader(" ", 1, ' ', false, true);//5
                gtf.SetGroupHeader(" ", 1, ' ', false, true);//6
                //gtf.SetGroupHeader(" ", 1, ' ', false, true);//7

                gtf.SetGroupHeaderSpace(4);
                gtf.SetGroupHeader(x.DeskrispiPekerjaan == null ? "" : (x.DeskrispiPekerjaan.ToString() != "") ? x.DeskrispiPekerjaan.ToString() : "", 136, ' ');
                gtf.SetGroupHeader((x.Nomor.ToString() != "") ? x.Nomor.ToString() : "", 18, ' ', false, true);//7
                gtf.SetGroupHeader(" ", 10, ' ', false, true);//8

                gtf.SetGroupHeaderSpace(15);
                pemilik = gtf.ConvertArrayText((x.Pemilik.ToString() != "") ? x.Pemilik.ToString() : "", 42);
                gtf.SetGroupHeader(pemilik[0] != null ? pemilik[0].ToString() : "", 40, ' ', false, true);//9

                gtf.SetGroupHeaderSpace(15);
                gtf.SetGroupHeader(pemilik[1] != null ? pemilik[1].ToString() : "", 40, ' ');
                gtf.SetGroupHeaderSpace(4);
                gtf.SetGroupHeader((x.TglPenyerahan.ToString() != "") ? Convert.ToDateTime(x.TglPenyerahan.ToString()).ToString("dd-MMM-yyyy") : "", 19, ' ');
                gtf.SetGroupHeaderSpace(3);

                fakturKepada = gtf.ConvertArrayText((x.FakturKepada.ToString() != "") ? x.FakturKepada.ToString() : "", 50);

                gtf.SetGroupHeader(fakturKepada[0] != null ? fakturKepada[0].ToString() : "", 50, ' ');
                gtf.SetGroupHeaderSpace(5);
                gtf.SetGroupHeader((x.TglJamMasuk.ToString() != "") ? Convert.ToDateTime(x.TglJamMasuk.ToString()).ToString("dd-MMM-yyyy HH:mm") : "", 19, ' ', false, true);//10

                gtf.SetGroupHeaderSpace(15);
                gtf.SetGroupHeader(pemilik[2] != null ? pemilik[2].ToString() : "", 40, ' ');
                gtf.SetGroupHeaderSpace(26);
                gtf.SetGroupHeader(fakturKepada[1] != null ? fakturKepada[1].ToString() : "", 50, ' ', false, true);//11

                alamat = gtf.ConvertArrayText((x.Alamat.ToString() != "") ? x.Alamat.ToString() : "", 40);

                gtf.SetGroupHeaderSpace(15);
                gtf.SetGroupHeader(alamat[0] != null ? alamat[0].ToString() : "", 40, ' ');
                gtf.SetGroupHeaderSpace(26);
                gtf.SetGroupHeader(fakturKepada[2] != null ? fakturKepada[2].ToString() : "", 50, ' ', false, true);//12

                gtf.SetGroupHeaderSpace(15);
                gtf.SetGroupHeader(alamat[1] != null ? alamat[1].ToString() : "", 40, ' ');
                gtf.SetGroupHeaderSpace(4);
                gtf.SetGroupHeader(x.Warna == null ? "" : (x.Warna.ToString() != "") ? x.Warna.ToString() : "", 19, ' ');
                gtf.SetGroupHeaderSpace(3);
                gtf.SetGroupHeader(fakturKepada[3] != null ? fakturKepada[3].ToString() : "", 50, ' ');
                gtf.SetGroupHeaderSpace(5);
                gtf.SetGroupHeader("", 20, ' ', false, true);//13

                gtf.SetGroupHeaderSpace(15);
                gtf.SetGroupHeader(alamat[2] != null ? alamat[2].ToString() : "", 40, ' ');
                gtf.SetGroupHeaderSpace(26);
                gtf.SetGroupHeader(fakturKepada[4] != null ? fakturKepada[4].ToString() : "", 50, ' ', false, true);//14

                telp = gtf.ConvertArrayText((x.PhoneNo.ToString() != "" && x.HPNo.ToString() != "") ? x.PhoneNo.ToString() + " / " + x.HPNo.ToString() : ((x.PhoneNo.ToString() != "") ? x.PhoneNo.ToString() : x.HPNo.ToString()), 40);

                gtf.SetGroupHeaderSpace(15);
                gtf.SetGroupHeader(telp[0] != null ? telp[0].ToString() : "", 40, ' ', true);
                gtf.SetGroupHeaderSpace(26);
                gtf.SetGroupHeader(fakturKepada[5] != null ? fakturKepada[5].ToString() : "", 50, ' ', false, true);//15

                gtf.SetGroupHeaderSpace(15);
                gtf.SetGroupHeader(telp[1] != null ? telp[1].ToString() : "", 40, ' ', true);
                gtf.SetGroupHeaderSpace(3);
                gtf.SetGroupHeader((x.NoPolisi.ToString() != "") ? x.NoPolisi.ToString() : "", 20, ' ', true);
                gtf.SetGroupHeaderSpace(3);
                gtf.SetGroupHeader(fakturKepada[6] != null ? fakturKepada[6].ToString() : "", 50, ' ');
                gtf.SetGroupHeaderSpace(5);
                gtf.SetGroupHeader((x.KM.ToString() != "") ? Convert.ToDecimal(x.KM.ToString()).ToString("n0") : "0", 20, ' ', false, true);//16

                gtf.SetGroupHeader(" ", 1, ' ', false, true);//17
                gtf.SetGroupHeader(" ", 1, ' ', false, true);//18

                gtf.SetGroupHeaderSpace(3);
                gtf.SetGroupHeader((x.NoChasis.ToString() != "") ? x.NoChasis.ToString() : "", 26, ' ');
                gtf.SetGroupHeaderSpace(4);
                gtf.SetGroupHeader((x.NoMesin.ToString() != "") ? x.NoMesin.ToString() : "", 21, ' ', true);
                gtf.SetGroupHeaderSpace(3);
                gtf.SetGroupHeader((x.Dealer.ToString() != "") ? x.Dealer.ToString() + (x.ProductionYear == null ? "" : (x.ProductionYear.ToString() != "") ? " (" + x.ProductionYear.ToString() + ")" : "") : "", 74, ' ');
                gtf.SetGroupHeaderSpace(16);
                gtf.SetGroupHeader((x.Frontman.ToString() != "") ? x.Frontman.ToString() : "", 11, ' ', false, true);//19

                gtf.SetGroupHeader(" ", 1, ' ', false, true);//20
                gtf.SetGroupHeader(" ", 1, ' ', false, true);//21

                gtf.SetGroupHeaderSpace(58);
                gtf.SetGroupHeader(x.JenisPekerjaan == null ? "" : (x.JenisPekerjaan.ToString() != "") ? x.JenisPekerjaan.ToString() : "", 95, ' ', false, true);//22

                string[] keluhan = gtf.ConvertArrayList(x.Keluhan.ToString(), 108, 7);

                gtf.SetGroupHeaderSpace(58);
                gtf.SetGroupHeader(keluhan[0] != null ? keluhan[0].ToString().Replace('\r', ' ') : "", 96, ' ', false, true);//23

                gtf.SetGroupHeaderSpace(58);
                gtf.SetGroupHeader(keluhan[1] != null ? keluhan[1].ToString().Replace('\r', ' ') : "", 96, ' ', false, true);//24
                gtf.SetGroupHeaderSpace(58);
                gtf.SetGroupHeader(keluhan[2] != null ? keluhan[2].ToString().Replace('\r', ' ') : "", 96, ' ', false, true);//25
                gtf.SetGroupHeaderSpace(58);
                gtf.SetGroupHeader(keluhan[3] != null ? keluhan[3].ToString().Replace('\r', ' ') : "", 96, ' ', false, true);//26
                gtf.SetGroupHeaderSpace(58);
                gtf.SetGroupHeader(keluhan[4] != null ? keluhan[4].ToString().Replace('\r', ' ') : "", 96, ' ', false, true);//27
                gtf.SetGroupHeaderSpace(58);
                gtf.SetGroupHeader(keluhan[5] != null ? keluhan[5].ToString().Replace('\r', ' ') : "", 96, ' ', false, true);//28
                gtf.SetGroupHeaderSpace(58);
                gtf.SetGroupHeader(keluhan[6] != null ? keluhan[6].ToString().Replace('\r', ' ') : "", 96, ' ', false, true);//29

                for (int d = 0; d < 22; d++)
                    gtf.SetGroupHeader(" ", 1, ' ', false, true);//52

                if (x.PenggantianSukuCadang.ToString() != "")
                {
                    if (!Convert.ToBoolean(x.PenggantianSukuCadang.ToString()))
                    {
                        gtf.SetGroupHeaderSpace(104);
                        gtf.SetGroupHeader("X", 18, 'X', false, true);//53
                    }
                    else
                    {
                        gtf.SetGroupHeaderSpace(85);
                        gtf.SetGroupHeader("X", 17, 'X', false, true);//53
                    }
                }
                else
                {
                    gtf.SetGroupHeader("", 1, ' ', false, true);//53

                }

                gtf.SetGroupHeader("", 1, ' ', false, true);//54
                gtf.SetGroupHeader("", 1, ' ', false, true);//55
                gtf.SetGroupHeaderSpace(137);
                gtf.SetGroupHeader((x.Nomor.ToString() != "") ? x.Nomor.ToString() : "", 19, ' ', false, true, true);//57

                gtf.SetGroupHeaderSpace(145);
                gtf.SetGroupHeader(DateTime.Now.ToString("dd-MMM-yyyy"), 11, ' ', false, true);//57
                gtf.SetGroupHeader("", 1, ' ', false, true);
                gtf.SetGroupHeader("", 1, ' ', false, true);
                gtf.SetGroupHeader("", 1, ' ', false, true);
                gtf.SetGroupHeader("", 1, ' ', false, true);
                gtf.SetGroupHeaderSpace(100);
                gtf.SetGroupHeader(pemilik[0] != null ? pemilik[0].ToString() : "", 100, ' ', false, true);//9

                gtf.PrintHeader();
                gtf.PrintTotal(true, false, false, true);
            });

            return gtf.sbDataTxt.ToString();
            //if (print == true)
            //    gtf.PrintTotal(true, false, false);
            //else
            //{
            //    if (gtf.PrintTotal(true, false, false) == true)
            //        //    XMessageBox.ShowInformation("Save Berhasil");
            //        return gtf.sbDataTxt.ToString();
            //    else
            //        //  XMessageBox.ShowWarning("Save Gagal");
            //        return "";
            //}

            //if (print == true)
            //    gtf.CloseConnectionPrinter();

            //return true;
            //return "";
        }



    }
}