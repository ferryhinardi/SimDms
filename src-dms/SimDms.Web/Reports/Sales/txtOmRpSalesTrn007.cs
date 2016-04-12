using SimDms.Common;
using SimDms.Sales.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace SimDms.Web.Reports.Sales
{ 
    public class txtOmRpSalesTrn007:IRptProc  
    {
        private SimDms.Sales.Models.DataContext ctx = new SimDms.Sales.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public Models.SysUser CurrentUser { get; set; }
        private object[] setTextParameter, paramText;

        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            //var dt = ctx.Database.SqlQuery<OmRpSalesTrn002>(string.Format("exec {0} {1}", sproc, sparam));
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "exec " + sproc + " " + sparam;
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return CreateReportOmRpSalesTrn007(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalesTrn007(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
        {
            string reportID = recordId; 
            bool bCreateBy = false; 
            int counterData = 0;

            #region GenerateHeader
            SalesGenerateTextFileReport gtf = new SalesGenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            if (reportID == "OmRpSalesTrn009A" || reportID == "OmRpSalesTrn003A" || reportID == "OmRpSalesTrn003D" ||
                reportID == "OmRpSalesTrn003C" || reportID == "OmRpSalesTrn002A" || reportID == "OmRpSalesTrn001D" ||
                reportID == "OmRpSalesTrn001E" || reportID == "OmRpSalesTrn001F" || reportID == "OmRpSalesTrn001G" ||
                reportID == "OmRpPurTrn001A" || reportID == "OmRpPurTrn002A" || reportID == "OmRpPurTrn003A" ||
                reportID == "OmRpSalesTrn006" || reportID == "OmRpSalesTrn006A" || reportID == "OmRpPurTrn008A" ||
                reportID == "OmRpPurTrn009" || reportID == "OmRpSalesTrn005A" || reportID == "OmRpSalesTrn010A" ||
                reportID == "OmRpStock001") fullPage = false;
            gtf.GenerateTextFileReports(reportID, printerLoc, "W96", print, "", fullPage);
            if (reportID == "OmRpSalesTrn004" || reportID == "OmRpSalesTrn004A" || reportID == "OmRpSalesTrn003A" ||
                reportID == "OmRpSalesTrn007DNew" || reportID == "OmRpSalesTrn007" || reportID == "OmRpSalesTrn007C" ||
                reportID == "OmRpSalesTrn007A" || reportID == "OmRpSalesTrn010" || reportID == "OmRpSalesTrn010A" ||
                reportID == "OmRpStock002")
            {
                gtf.GenerateHeader2(false);
            }
            else if (reportID == "OmRpSalesTrn003D" || reportID == "OmRpSalesTrn001D" ||
                reportID == "OmRpSalesTrn001E" || reportID == "OmRpSalesTrn006" || reportID == "OmRpSalesTrn006A" ||
                reportID == "OmRpPurTrn009" || reportID == "OmRpSalesTrn005" || reportID == "OmRpSalesTrn005A" || reportID == "OmRpStock001")
            {
                gtf.GenerateHeader2(false, false);
            }
            else
            {
                //gtf.GenerateHeader();
            }
            #endregion

            #region Permohonan Faktur Polisi (OmRpSalesTrn007DNew/OmRpSalesTrn007/OmRpSalesTrn007C/OmRpSalesTrn007A)

            #region Faktur Polisi (OmRpSalesTrn007DNew/OmRpSalesTrn007)
            if (reportID == "OmRpSalesTrn007DNewWeb" || reportID == "OmRpSalesTrn007")
            {
                bCreateBy = false;

                string companyName = dt.Rows[0]["CompanyName"].ToString();
                string coAdd1 = dt.Rows[0]["CoAdd1"].ToString();
                string coAdd2 = dt.Rows[0]["CoAdd2"].ToString();
                string coAdd3 = dt.Rows[0]["CoAdd3"].ToString();
                string fakturPolisiNo = dt.Rows[0]["FakturPolisiNo"].ToString();

                gtf.SetGroupHeader(companyName, 47, ' ', true);
                gtf.SetGroupHeader(textParam[0].ToString(), 48, ' ', false, true, true);
                gtf.SetGroupHeader(coAdd1, 96, ' ', false, true);
                gtf.SetGroupHeader(coAdd2, 96, ' ', false, true);
                gtf.SetGroupHeader(coAdd3, 96, ' ', false, true);
                gtf.SetGroupHeaderSpace(36);
                gtf.SetGroupHeader("PERMOHONAN FAKTUR POLISI", 24, ' ');
                gtf.SetGroupHeader(fakturPolisiNo, 36, ' ', false, true, true);
                gtf.PrintHeader();

                string reqNo = string.Empty;
                int loop = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    gtf.SetDataDetail("No Surat Permohonan : " + dt.Rows[i]["ReqNo"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("No. Surat Konfirmasi Penjualan (SKPK) : " + dt.Rows[i]["SKPKNo"].ToString(), 96, ' ', false, true, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("No Urut : " + dt.Rows[i]["No"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("Dengan Hormat,", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("Berdasarkan D.O. No    : " + dt.Rows[i]["DONo"].ToString(), 54, ' ', true);
                    gtf.SetDataDetail("Tgl       : " + Convert.ToDateTime(dt.Rows[i]["DODate"]).ToString("dd/M/yyyy"), 41, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            a/n        : " + dt.Rows[i]["CompanyName"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            S.J. No    : " + dt.Rows[i]["SJNo"].ToString(), 54, ' ', true);
                    gtf.SetDataDetail("Tgl       : " + Convert.ToDateTime(dt.Rows[i]["SJDate"]).ToString("dd/M/yyyy"), 41, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            a/n        : " + dt.Rows[i]["CompanyName"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Tipe       : " + dt.Rows[i]["Model"].ToString(), 54, ' ', true);
                    gtf.SetDataDetail("Model     : " + dt.Rows[i]["ModelDesc"].ToString(), 41, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Warna      : " + dt.Rows[i]["Warna"].ToString(), 54, ' ', true);
                    gtf.SetDataDetail("Tahun     : " + dt.Rows[i]["Tahun"].ToString(), 41, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            No Rangka  : " + dt.Rows[i]["ChassisNo"].ToString(), 54, ' ', true);
                    gtf.SetDataDetail("No Mesin  : " + dt.Rows[i]["EngineNo"].ToString(), 41, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Penjual    : " + dt.Rows[i]["Penjual"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Salesman   : " + dt.Rows[i]["SalesmanName"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    string[] namaSKPK = new string[] { };
                    if (reportID == "OmRpSalesTrn007DNew")
                    {
                        namaSKPK = dt.Rows[i]["SKPKName"].ToString().Split('\n');
                        gtf.SetDataDetail("            Nama SKPK  : " + namaSKPK[0].ToString(), 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        if (namaSKPK.Length > 1)
                            gtf.SetDataDetail("                       : " + ((namaSKPK[1] != string.Empty) ? namaSKPK[1].ToString() : string.Empty), 96, ' ', false, true);
                        else
                            gtf.SetDataDetail("                       : ", 96, ' ', false, true);
                    }
                    else
                        gtf.SetDataDetail("            Nama SKPK  : " + dt.Rows[i]["SKPKName"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Alamat     : " + dt.Rows[i]["Alamat1"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("                       : " + dt.Rows[i]["Alamat2"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("                       : " + dt.Rows[i]["Alamat3"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Nama Kota  : " + dt.Rows[i]["City"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Telepon 1  : " + dt.Rows[i]["SKPKTelp1"].ToString(), 54, ' ', true);
                    gtf.SetDataDetail("Tgl.Lahir : " + Convert.ToDateTime(dt.Rows[i]["SKPKDay"]).ToString("dd/M/yyyy"), 41, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Telepon 2  : " + dt.Rows[i]["SKPKTelp2"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Handphone  : " + dt.Rows[i]["SKPKHP"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("        Jenis Dokumen  : ", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Kategori   : " + dt.Rows[i]["DealerCategory"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Keterangan : " + dt.Rows[i]["Remark"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail(dt.Rows[i]["Note"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    string[] nama = new string[] { };
                    if (reportID == "OmRpSalesTrn007DNew")
                    {
                        nama = dt.Rows[i]["FakturPolisiName"].ToString().Split('\n');
                        gtf.SetDataDetail("Kepada :    Nama       : " + nama[0].ToString(), 96, ' ', false, true);
                        if (nama.Length > 1)
                            gtf.SetDataDetail("            Nama       : " + ((nama[1] != string.Empty) ? nama[1].ToString() : ""), 96, ' ', false, true);
                        else
                            gtf.SetDataDetail("            Nama       : ", 96, ' ', false, true);
                    }
                    else
                        gtf.SetDataDetail("Kepada :    Nama       : " + dt.Rows[i]["FakturPolisiName"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Alamat     : " + dt.Rows[i]["FakturPolisiAddress1"], 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("                       : " + dt.Rows[i]["FakturPolisiAddress2"], 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("                       : " + dt.Rows[i]["FakturPolisiAddress3"], 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Telepon 1  : " + dt.Rows[i]["FakturPolisiTelp1"], 54, ' ', true);
                    gtf.SetDataDetail("Tgl.Lahir : " + Convert.ToDateTime(dt.Rows[i]["FakturPolisiBirthday"]).ToString("dd/M/yyyy"), 41, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Telepon 2  : " + dt.Rows[i]["FakturPolisiTelp2"], 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("            Handphone  : " + dt.Rows[i]["FakturPolisiHP"], 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    if (reportID == "OmRpSalesTrn007DNew")
                    {
                        gtf.SetDataDetail("            No ID      : " + dt.Rows[i]["IDNo"], 96, ' ', false, true);
                        gtf.PrintData(false, false);
                    }
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("Dengan ini kami terangkan, bahwa kendaraan tersebut belum didaftarkan pada POLISI", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("setempat dan kami bertanggungjawab atas penyerahan faktur/surat-surat lainnya", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("kepada pemilik", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail(" -", 95, '-', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(12);
                    gtf.SetDataDetail("CATATAN", 7, ' ');
                    gtf.SetDataDetailSpace(11);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(6);
                    gtf.SetDataDetail("DIISI OLEH PT.SIS / PT.SIM", 26, ' ');
                    gtf.SetDataDetailSpace(5);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(9);
                    gtf.SetDataDetail("PEMOHON", 7, ' ');
                    gtf.SetDataDetailSpace(9);
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail(" -", 95, '-', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(30);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(37);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(25);
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(30);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(37);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(25);
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(30);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(37);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(25);
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(30);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(37);
                    gtf.SetDataDetail("|", 1, ' ');
                    int temp = (25 - dt.Rows[i]["SignName1"].ToString().Length) / 2;
                    gtf.SetDataDetailSpace(temp);
                    gtf.SetDataDetail(dt.Rows[i]["SignName1"].ToString(), dt.Rows[i]["SignName1"].ToString().Length, ' ');
                    gtf.SetDataDetailSpace(temp);
                    temp = temp + temp + dt.Rows[i]["SignName1"].ToString().Length;
                    temp = 25 - temp;
                    if (temp > 0)
                    {
                        gtf.SetDataDetailSpace(temp);
                    }
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(30);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(37);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 25, '-');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(30);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(37);
                    gtf.SetDataDetail("|", 1, ' ');
                    temp = (25 - dt.Rows[i]["TitleSign1"].ToString().Length) / 2;
                    gtf.SetDataDetailSpace(temp);
                    gtf.SetDataDetail(dt.Rows[i]["TitleSign1"].ToString(), dt.Rows[i]["TitleSign1"].ToString().Length, ' ');
                    gtf.SetDataDetailSpace(temp);
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail(" -", 95, '-', false, true);
                    gtf.PrintData(false, false);

                    if (reportID == "OmRpSalesTrn007DNew") loop = 1;
                    else loop = 4;

                    for (int j = 0; j < loop; j++)
                    {
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                    }

                    if (i == dt.Rows.Count - 1) break;
                    gtf.ReplaceGroupHdr(fakturPolisiNo, dt.Rows[i + 1]["FakturPolisiNo"].ToString());
                    fakturPolisiNo = dt.Rows[i + 1]["FakturPolisiNo"].ToString();
                }
            }
            #endregion

            #region Blanko (OmRpSalesTrn007C/OmRpSalesTrn007A)
            if (reportID == "OmRpSalesTrn007C" || reportID == "OmRpSalesTrn007A")
            {
                bCreateBy = false;
                int loop = 0, space = 0;
                if (reportID == "OmRpSalesTrn007C")
                    space = 33;
                else
                    space = 57;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (reportID == "OmRpSalesTrn007C")
                        loop = 8;
                    else
                        loop = 10;

                    for (int j = 0; j < loop; j++)
                    {
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        if (gtf.line == 60)
                            gtf.PrintData(false, false, false);
                        else
                            gtf.PrintData(false, false);
                    }

                    if (reportID == "OmRpSalesTrn007C")
                        gtf.SetDataDetailSpace(69);
                    else
                        gtf.SetDataDetailSpace(64);

                    gtf.SetDataDetail(dt.Rows[i]["Tanggal"].ToString(), 27, ' ', false, true);
                    gtf.PrintData(false, false);

                    for (int j = 0; j < 2; j++)
                    {
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                    }

                    gtf.SetDataDetailSpace(space);
                    if (reportID == "OmRpSalesTrn007C")
                    {
                        string[] strFPolisi = dt.Rows[i]["FakturPolisiName"].ToString().Split('\n');
                        gtf.SetDataDetail(strFPolisi[0].ToString(), 96 - space, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(33);
                        if (strFPolisi.Length > 1)
                            gtf.SetDataDetail(strFPolisi[1].ToString(), 96 - space, ' ', false, true);
                        else
                            gtf.SetDataDetail("", 96 - space, ' ', false, true);
                    }
                    else
                        gtf.SetDataDetail(dt.Rows[i]["FakturPolisiName"].ToString(), 96 - space, ' ', false, true);

                    gtf.PrintData(false, false);
                    gtf.SetDataDetailSpace(space);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiAddress1"].ToString(), 96 - space, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetailSpace(space);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiAddress2"].ToString(), 96 - space, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetailSpace(space);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiAddress3"].ToString(), 96 - space, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);

                    if (reportID == "OmRpSalesTrn007C")
                    {
                        gtf.SetDataDetailSpace(space);
                        gtf.SetDataDetail(dt.Rows[i]["IDNo"].ToString(), 63, ' ', false, true);

                    }
                    else
                        gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);

                    loop = 59 - gtf.line;
                    for (int j = 0; j < loop; j++)
                    {
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                    }
                }
            }
            #endregion

            #region Sertifikat (OmRpSalesTrn007B)
            if (reportID == "OmRpSalesTrn007B")
            {
                bCreateBy = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < 40; j++)
                    {
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                    }

                    gtf.SetDataDetailSpace(54);
                    gtf.SetDataDetail(dt.Rows[i]["Tanggal"].ToString(), 42, ' ', false, true);
                    gtf.PrintData(false, false);

                    int loop = 59 - gtf.line;
                    for (int j = 0; j < loop; j++)
                    {
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                    }
                }
            }
            #endregion

            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}