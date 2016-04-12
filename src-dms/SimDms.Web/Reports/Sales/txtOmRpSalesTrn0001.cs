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
    public class txtOmRpSalesTrn0001:IRptProc  
    {
        private SimDms.Sales.Models.DataContext ctx = new SimDms.Sales.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public Models.SysUser CurrentUser { get; set; }
        

        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            //var dt = ctx.Database.SqlQuery<OmRpSalesTrn002>(string.Format("exec {0} {1}", sproc, sparam));
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "exec " + sproc + " " + sparam;
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return CreateReportOmRpSalesTrn0001(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalesTrn0001(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
        {
            string reportID = recordId; 

            bool bCreateBy = false;
            int counterData = 0;

            #region GenerateHeader
            SalesGenerateTextFileReport gtf = new SalesGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            //gtf.SetParameterPrinter(paramReport);
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
                gtf.GenerateHeader();
            }
            #endregion

            #region SO (OmRpSalesTrn001/OmRpSalesTrn001A/OmRpSalesTrn001B/OmRpSalesTrn001C/OmRpSalesTrn001D/OmRpSalesTrn001E/OmRpSalesTrn001F/OmRpSalesTrn001G)
            if (reportID == "OmRpSalesTrn001" || reportID == "OmRpSalesTrn001A" || reportID == "OmRpSalesTrn001B" || reportID == "OmRpSalesTrn001C"
                || reportID == "OmRpSalesTrn001D" || reportID == "OmRpSalesTrn001E" || reportID == "OmRpSalesTrn001F" || reportID == "OmRpSalesTrn001G")
            {
                bCreateBy = false;
                string namaPelanggan = string.Empty, soNO = string.Empty, alamat1 = string.Empty, tgglSO = string.Empty, alamat2 = string.Empty,
                    SKPKNo = string.Empty, alamat3 = string.Empty, refferenceNo = string.Empty, alamat4 = string.Empty,
                    namaGovPelanggan = string.Empty, leasingName = string.Empty, leasingCo = string.Empty, alamatFaktur1 = string.Empty,
                    alamatFaktur2 = string.Empty, alamatFaktur3 = string.Empty, alamatFaktur4 = string.Empty, NPWPNo = string.Empty,
                    TOPName = string.Empty, sales = string.Empty, tipeSales = string.Empty, requestDate = string.Empty, shipName = string.Empty,
                    soDate = string.Empty, ket = string.Empty;
                string tempTerbilang = string.Empty, tempTerbilang1 = string.Empty, tempTerbilang2 = string.Empty;
                string[] tempSplitTerbilang;

                #region OmRpSalesTrn001
                if (reportID == "OmRpSalesTrn001")
                {
                    namaPelanggan = dt.Rows[0]["NAMAPELANGGAN"].ToString();
                    soNO = dt.Rows[0]["NOMORSO"].ToString();
                    alamat1 = dt.Rows[0]["ALAMAT1"].ToString();
                    tgglSO = Convert.ToDateTime(dt.Rows[0]["TANGGALSO"]).ToString("dd-MMM-yyyy");
                    alamat2 = dt.Rows[0]["ALAMAT2"].ToString();
                    SKPKNo = dt.Rows[0]["SKPKNO"].ToString();
                    alamat3 = dt.Rows[0]["ALAMAT3"].ToString();
                    refferenceNo = dt.Rows[0]["RefferenceNo"].ToString();
                    alamat4 = dt.Rows[0]["ALAMAT4"].ToString();
                    namaGovPelanggan = dt.Rows[0]["NAMAGOVPELANGGAN"].ToString();
                    leasingName = dt.Rows[0]["LeasingName"].ToString();
                    leasingCo = dt.Rows[0]["LeasingCo"].ToString();
                    alamatFaktur1 = dt.Rows[0]["ALAMAT1"].ToString();
                    alamatFaktur2 = dt.Rows[0]["ALAMAT2"].ToString();
                    alamatFaktur3 = dt.Rows[0]["ALAMAT3"].ToString();
                    alamatFaktur4 = dt.Rows[0]["ALAMAT4"].ToString();
                    NPWPNo = dt.Rows[0]["NPWPNo"].ToString();

                    gtf.SetGroupHeader("NAMA         :", 14, ' ', true);
                    gtf.SetGroupHeader(namaPelanggan, 55, ' ', true);
                    gtf.SetGroupHeader("NOMOR   :", 9, ' ', true);
                    gtf.SetGroupHeader(soNO, 15, ' ', false, true);
                    gtf.SetGroupHeader("ALAMAT       :", 14, ' ', true);
                    gtf.SetGroupHeader(alamat1, 55, ' ', true);
                    gtf.SetGroupHeader("TANGGAL :", 9, ' ', true);
                    gtf.SetGroupHeader(tgglSO, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamat2, 55, ' ', true);
                    gtf.SetGroupHeader("No.SKPK :", 9, ' ', true);
                    gtf.SetGroupHeader(SKPKNo, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamat3, 55, ' ', true);
                    gtf.SetGroupHeader("No.Reff :", 9, ' ', true);
                    gtf.SetGroupHeader(refferenceNo, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamat4, 81, ' ', false, true);
                    gtf.SetGroupHeader("", 96, ' ', false, true);
                    gtf.SetGroupHeader("Faktur Pajak dibuat atas :", 96, ' ', false, true);
                    gtf.SetGroupHeader("NAMA         :", 14, ' ', true);
                    gtf.SetGroupHeader(namaGovPelanggan, 29, ' ', true);
                    if (leasingName != string.Empty)
                    {
                        gtf.SetGroupHeader("QQ : " + leasingName, 51 - (dt.Rows[0]["LeasingCo"].ToString().Length + 2), ' ', true, false, true);
                        gtf.SetGroupHeader("[" + leasingCo + "]", dt.Rows[0]["LeasingCo"].ToString().Length + 2, ' ', false, true);
                    }
                    else
                        gtf.SetGroupHeader("", 51, ' ', false, true);
                    gtf.SetGroupHeader("ALAMAT       :", 14, ' ', true);
                    gtf.SetGroupHeader(alamatFaktur1, 81, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamatFaktur2, 81, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamatFaktur3, 81, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamatFaktur4, 81, ' ', false, true);
                    gtf.SetGroupHeader("NPWP         :", 14, ' ', true);
                    gtf.SetGroupHeader(NPWPNo, 81, ' ', false, true);
                    gtf.SetGroupHeader("", 96, ' ', false, true);
                    gtf.SetGroupHeader("Kami memesan kendaraan kepada Saudara dengan perincian", 96, ' ', false, true);

                    gtf.SetGroupHeaderLine();
                    gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("MODEL", 15, ' ', true);
                    gtf.SetGroupHeader("WARNA", 35, ' ', true);
                    gtf.SetGroupHeader("UNIT", 5, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("HARGA", 15, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("JUMLAH", 15, ' ', false, true, true);
                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();

                    decimal decJumlah = 0;
                    string NoSO = string.Empty;
                    int loop = 0, tempSpace = 0;
                    int tempLength = 0;

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            loop = 59 - gtf.line - 21;
                            for (int j = 0; j < loop; j++)
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                            }

                            gtf.SetDataReportLine();
                            gtf.PrintData();
                            tempTerbilang = Convert.ToString("Terbilang : " + gtf.Terbilang(Convert.ToInt32(decJumlah)).ToUpper() + " RUPIAH");
                            tempTerbilang1 = tempTerbilang2 = string.Empty;
                            if (tempTerbilang.Length > 80)
                            {
                                tempSplitTerbilang = tempTerbilang.Split(' ');
                                foreach (string s in tempSplitTerbilang)
                                {
                                    if (tempTerbilang1.Length + s.Length + 1 > 80) break;
                                    tempTerbilang1 += s + ' ';
                                }
                                tempLength = tempTerbilang1.Length;

                                tempTerbilang1 = tempTerbilang.Substring(0, tempTerbilang1.Length);
                                tempTerbilang2 = tempTerbilang.Substring(tempLength);
                            }
                            else
                            {
                                tempTerbilang1 = tempTerbilang;
                            }

                            gtf.SetDataDetail(tempTerbilang1, 80, ' ', true);
                            gtf.SetDataDetail(decJumlah.ToString(), 15, ' ', false, true, true, true, "n0");
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(13);
                            gtf.SetDataDetail(tempTerbilang2, 67, ' ', true);
                            gtf.SetDataDetail("-", 15, '-', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataReportLine();
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(37);
                            gtf.SetDataDetail("P e r s y a r a t a n", 22, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("Pembayaran : " + dt.Rows[i - 1]["PEMBAYARAN"].ToString(), 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("Perkiraan", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("Penyerahan : ", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("Lain-lain  : " + dt.Rows[i - 1]["LAINLAIN"].ToString(), 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataReportLine();
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail(dt.Rows[i - 1]["City"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALSO"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                            gtf.PrintData();
                            if (dt.Rows[i - 1]["BBN"] == null || dt.Rows[i - 1]["BBN"] is DBNull)
                            {
                                gtf.SetDataDetail("- Nilai BBN                : 0", 55, ' ', true);
                            }
                            else
                                gtf.SetDataDetail("- Nilai BBN                : " + Convert.ToDecimal(dt.Rows[i - 1]["BBN"]).ToString("n0"), 55, ' ', true);
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("Menyetujui", 10, ' ');
                            gtf.SetDataDetailSpace(10);
                            gtf.SetDataDetail("Pemesan", 10, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("- PP ini berlaku 30 hari s/d " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALSO"]).AddDays(30).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("- Harga belum termasuk STUJ, BBN, BPKB, dll", 96, ' ', false, true);
                            gtf.PrintData();
                            string tempSplitString1 = string.Empty;
                            string tempSplitString2 = string.Empty;
                            if (dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length > 20)
                            {
                                gtf.SetDataDetail("- PP ini tidak berlaku bila terjadi hal-hal di luar", 75, ' ', true);
                                string[] tempArrayString1 = dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Split(' ');
                                string tempString = string.Empty;
                                foreach (string s in tempArrayString1)
                                {
                                    if (Convert.ToString(tempString + s + ' ').Length < 20)
                                        tempString = tempString += s + ' ';
                                    else
                                        break;
                                }
                                tempSplitString1 = tempString;
                                tempSplitString2 = dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Substring(tempString.Length, dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length - tempString.Length);

                                tempSpace = (20 - tempSplitString1.Length) / 2;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(tempSplitString1, tempSplitString1.Length, ' ', false, true);
                            }
                            else
                            {
                                gtf.SetDataDetail("- PP ini tidak berlaku bila terjadi hal-hal di luar", 96, ' ', false, true);
                            }
                            gtf.PrintData();
                            gtf.SetDataDetail("  kemampuan kami (Force Majeur), seperti perubahan", 55, ' ', true);
                            if (dt.Rows[i - 1]["SignName1"].ToString().Length < 20)
                                tempSpace = (20 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace);
                            if (tempSplitString1 != string.Empty)
                            {
                                tempSpace = (20 - tempSplitString2.Length) / 2;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(tempSplitString2, tempSplitString2.Length, ' ', false, true);
                            }
                            else
                            {
                                if (dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length < 20)
                                    tempSpace = (20 - dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace + 1);
                                gtf.SetDataDetail(dt.Rows[i - 1]["NAMAPELANGGAN"].ToString(), dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length, ' ', false, true);
                            }
                            gtf.PrintData();
                            gtf.SetDataDetail("  Peraturan Pemerintah dalam bidang Perpajakan", 55, ' ', true);
                            gtf.SetDataDetail("-", 19, '-', true);
                            gtf.SetDataDetail("-", 20, '-', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("  dan/atau Moneter dan sebagainya", 55, ' ', true);
                            if (dt.Rows[i - 1]["TitleSign1"].ToString().Length < 20)
                                tempSpace = (20 - dt.Rows[i - 1]["TitleSign1"].ToString().Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 20, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            break;
                        }

                        if (NoSO == string.Empty)
                        {
                            NoSO = dt.Rows[0]["NOMORSO"].ToString();
                        }
                        else
                        {
                            if (NoSO != dt.Rows[i]["NOMORSO"].ToString())
                            {
                                loop = 59 - gtf.line - 21;
                                for (int j = 0; j < loop; j++)
                                {
                                    gtf.SetDataDetail("", 96, ' ', false, true);
                                    gtf.PrintData();
                                }

                                gtf.SetDataReportLine();
                                gtf.PrintData();
                                tempTerbilang = Convert.ToString("Terbilang : " + gtf.Terbilang(Convert.ToInt32(decJumlah)).ToUpper() + " RUPIAH");
                                tempTerbilang1 = tempTerbilang2 = string.Empty;
                                if (tempTerbilang.Length > 80)
                                {
                                    tempSplitTerbilang = tempTerbilang.Split(' ');
                                    foreach (string s in tempSplitTerbilang)
                                    {
                                        if (tempTerbilang1.Length + s.Length + 1 > 80) break;
                                        tempTerbilang1 += s + ' ';
                                    }
                                    tempLength = tempTerbilang1.Length;

                                    tempTerbilang1 = tempTerbilang.Substring(0, tempTerbilang1.Length);
                                    tempTerbilang2 = tempTerbilang.Substring(tempLength);
                                }
                                else
                                {
                                    tempTerbilang1 = tempTerbilang;
                                }

                                gtf.SetDataDetail(tempTerbilang1, 80, ' ', true);
                                gtf.SetDataDetail(decJumlah.ToString(), 15, ' ', false, true, true, true, "n0");
                                gtf.PrintData();
                                gtf.SetDataDetailSpace(13);
                                gtf.SetDataDetail(tempTerbilang2, 67, ' ', true);
                                gtf.SetDataDetail("-", 15, '-', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataReportLine();
                                gtf.PrintData();
                                gtf.SetDataDetailSpace(37);
                                gtf.SetDataDetail("P e r s y a r a t a n", 22, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("Pembayaran : " + dt.Rows[i - 1]["PEMBAYARAN"].ToString(), 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("Perkiraan", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("Penyerahan : ", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("Lain-lain  : " + dt.Rows[i - 1]["LAINLAIN"].ToString(), 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataReportLine();
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail(dt.Rows[i - 1]["City"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALSO"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                                gtf.PrintData();
                                if (dt.Rows[i - 1]["BBN"] == null || dt.Rows[i - 1]["BBN"] is DBNull)
                                {
                                    gtf.SetDataDetail("- Nilai BBN                : 0", 55, ' ', true);
                                }
                                else
                                    gtf.SetDataDetail("- Nilai BBN                : " + Convert.ToDecimal(dt.Rows[i - 1]["BBN"]).ToString("n0"), 55, ' ', true);
                                gtf.SetDataDetailSpace(5);
                                gtf.SetDataDetail("Menyetujui", 10, ' ');
                                gtf.SetDataDetailSpace(10);
                                gtf.SetDataDetail("Pemesan", 10, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("- PP ini berlaku 30 hari s/d " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALSO"]).AddDays(30).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("- Harga belum termasuk STUJ, BBN, BPKB, dll", 96, ' ', false, true);
                                gtf.PrintData();
                                string tempSplitString1 = string.Empty;
                                string tempSplitString2 = string.Empty;
                                if (dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length > 20)
                                {
                                    gtf.SetDataDetail("- PP ini tidak berlaku bila terjadi hal-hal di luar", 75, ' ', true);
                                    string[] tempArrayString1 = dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Split(' ');
                                    string tempString = string.Empty;
                                    foreach (string s in tempArrayString1)
                                    {
                                        if (Convert.ToString(tempString + s + ' ').Length < 20)
                                            tempString = tempString += s + ' ';
                                        else
                                            break;
                                    }
                                    tempSplitString1 = tempString;
                                    tempSplitString2 = dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Substring(tempString.Length, dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length - tempString.Length);

                                    tempSpace = (20 - tempSplitString1.Length) / 2;
                                    gtf.SetDataDetailSpace(tempSpace);
                                    gtf.SetDataDetail(tempSplitString1, tempSplitString1.Length, ' ', false, true);
                                }
                                else
                                {
                                    gtf.SetDataDetail("- PP ini tidak berlaku bila terjadi hal-hal di luar", 96, ' ', false, true);
                                }
                                gtf.PrintData();
                                gtf.SetDataDetail("  kemampuan kami (Force Majeur), seperti perubahan", 55, ' ', true);
                                if (dt.Rows[i - 1]["SignName1"].ToString().Length < 20)
                                    tempSpace = (20 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ');
                                gtf.SetDataDetailSpace(tempSpace);
                                if (tempSplitString1 != string.Empty)
                                {
                                    tempSpace = (20 - tempSplitString2.Length) / 2;
                                    gtf.SetDataDetailSpace(tempSpace);
                                    gtf.SetDataDetail(tempSplitString2, tempSplitString2.Length, ' ', false, true);
                                }
                                else
                                {
                                    if (dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length < 20)
                                        tempSpace = (20 - dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length) / 2;
                                    else
                                        tempSpace = 1;
                                    gtf.SetDataDetailSpace(tempSpace + 1);
                                    gtf.SetDataDetail(dt.Rows[i - 1]["NAMAPELANGGAN"].ToString(), dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length, ' ', false, true);
                                }
                                gtf.PrintData();
                                gtf.SetDataDetail("  Peraturan Pemerintah dalam bidang Perpajakan", 55, ' ', true);
                                gtf.SetDataDetail("-", 19, '-', true);
                                gtf.SetDataDetail("-", 20, '-', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("  dan/atau Moneter dan sebagainya", 55, ' ', true);
                                if (dt.Rows[i - 1]["TitleSign1"].ToString().Length < 20)
                                    tempSpace = (20 - dt.Rows[i - 1]["TitleSign1"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 20, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();

                                NoSO = dt.Rows[i]["NOMORSO"].ToString();
                                decJumlah = 0;
                                counterData = 0;

                                gtf.ReplaceGroupHdr(namaPelanggan, dt.Rows[i]["NAMAPELANGGAN"].ToString(), 55);
                                gtf.ReplaceGroupHdr(soNO, dt.Rows[i]["NOMORSO"].ToString(), 15);
                                gtf.ReplaceGroupHdr(alamat1, dt.Rows[i]["ALAMAT1"].ToString(), 55);
                                gtf.ReplaceGroupHdr(tgglSO, Convert.ToDateTime(dt.Rows[i]["TANGGALSO"]).ToString("dd-MMM-yyyy"), 15);
                                gtf.ReplaceGroupHdr(alamat2, dt.Rows[i]["ALAMAT2"].ToString(), 55);
                                gtf.ReplaceGroupHdr(SKPKNo, dt.Rows[i]["SKPKNO"].ToString(), 15);
                                gtf.ReplaceGroupHdr(alamat3, dt.Rows[i]["ALAMAT3"].ToString(), 55);
                                gtf.ReplaceGroupHdr(refferenceNo, dt.Rows[i]["RefferenceNo"].ToString(), 15);
                                gtf.ReplaceGroupHdr(alamat4, dt.Rows[i]["ALAMAT4"].ToString(), 81);
                                gtf.ReplaceGroupHdr(namaGovPelanggan, dt.Rows[i]["NAMAGOVPELANGGAN"].ToString(), 29);
                                gtf.ReplaceGroupHdr(leasingName, dt.Rows[i]["LeasingName"].ToString(), 51 - (dt.Rows[i]["LeasingCo"].ToString().Length + 2), 717);
                                gtf.ReplaceGroupHdr(leasingCo, dt.Rows[i]["LeasingCo"].ToString(), dt.Rows[i]["LeasingCo"].ToString().Length + 2, 717 + 51 - (dt.Rows[i - 1]["LeasingCo"].ToString().Length + 2));
                                gtf.ReplaceGroupHdr(alamatFaktur1, dt.Rows[i]["ALAMAT1"].ToString(), 81);
                                gtf.ReplaceGroupHdr(alamatFaktur2, dt.Rows[i]["ALAMAT2"].ToString(), 81);
                                gtf.ReplaceGroupHdr(alamatFaktur3, dt.Rows[i]["ALAMAT3"].ToString(), 81);
                                gtf.ReplaceGroupHdr(alamatFaktur4, dt.Rows[i]["ALAMAT4"].ToString(), 81);
                                gtf.ReplaceGroupHdr(NPWPNo, dt.Rows[i]["NPWPNo"].ToString(), 81);

                                namaPelanggan = dt.Rows[i]["NAMAPELANGGAN"].ToString();
                                soNO = dt.Rows[i]["NOMORSO"].ToString();
                                alamat1 = dt.Rows[i]["ALAMAT1"].ToString();
                                tgglSO = Convert.ToDateTime(dt.Rows[i]["TANGGALSO"]).ToString("dd-MMM-yyyy");
                                alamat2 = dt.Rows[i]["ALAMAT2"].ToString();
                                SKPKNo = dt.Rows[i]["SKPKNO"].ToString();
                                alamat3 = dt.Rows[i]["ALAMAT3"].ToString();
                                refferenceNo = dt.Rows[i]["RefferenceNo"].ToString();
                                alamat4 = dt.Rows[i]["ALAMAT4"].ToString();
                                namaGovPelanggan = dt.Rows[i]["NAMAGOVPELANGGAN"].ToString();
                                leasingName = dt.Rows[i]["LeasingName"].ToString();
                                leasingCo = dt.Rows[i]["LeasingCo"].ToString();
                                alamatFaktur1 = dt.Rows[i]["ALAMAT1"].ToString();
                                alamatFaktur2 = dt.Rows[i]["ALAMAT2"].ToString();
                                alamatFaktur3 = dt.Rows[i]["ALAMAT3"].ToString();
                                alamatFaktur4 = dt.Rows[i]["ALAMAT4"].ToString();
                                NPWPNo = dt.Rows[i]["NPWPNo"].ToString();
                            }
                        }
                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["MODEL"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["COLOURNAME"].ToString(), 35, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["UNIT"].ToString(), 5, ' ', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["HARGA"].ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["JUMLAH"].ToString(), 15, ' ', false, true, true, true, "n0");

                        decJumlah += Convert.ToDecimal(dt.Rows[i]["JUMLAH"]);

                        gtf.PrintData(true);
                    }
                }
                #endregion

                #region OmRpSalesTrn001D
                else if (reportID == "OmRpSalesTrn001D")
                {
                    namaPelanggan = dt.Rows[0]["NAMAPELANGGAN"].ToString();
                    soNO = dt.Rows[0]["NOMORSO"].ToString();
                    tgglSO = Convert.ToDateTime(dt.Rows[0]["TANGGALSO"]).ToString("dd-MMM-yyyy");
                    SKPKNo = dt.Rows[0]["SKPKNO"].ToString();
                    alamat3 = dt.Rows[0]["ALAMAT3"].ToString();
                    refferenceNo = dt.Rows[0]["RefferenceNo"].ToString();
                    alamat4 = dt.Rows[0]["ALAMAT4"].ToString();
                    namaGovPelanggan = dt.Rows[0]["NAMAGOVPELANGGAN"].ToString();
                    leasingName = dt.Rows[0]["LeasingName"].ToString();
                    leasingCo = dt.Rows[0]["LeasingCo"].ToString();
                    alamatFaktur1 = dt.Rows[0]["ALAMAT1"].ToString();
                    alamatFaktur2 = dt.Rows[0]["ALAMAT2"].ToString();
                    alamatFaktur3 = dt.Rows[0]["ALAMAT3"].ToString();
                    alamatFaktur4 = dt.Rows[0]["ALAMAT4"].ToString();
                    NPWPNo = dt.Rows[0]["NPWPNo"].ToString();

                    gtf.SetGroupHeader("PESANAN PENJUALAN", 96, ' ', false, true);
                    gtf.SetGroupHeader("NAMA         :", 14, ' ', true);
                    gtf.SetGroupHeader(namaPelanggan, 55, ' ', true);
                    gtf.SetGroupHeader("NOMOR   :", 9, ' ', true);
                    gtf.SetGroupHeader(soNO, 15, ' ', false, true);
                    if (dt.Rows[0]["ALAMAT2"].ToString() != string.Empty)
                        alamat1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " + dt.Rows[0]["ALAMAT2"].ToString();
                    else
                        alamat1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString();

                    gtf.SetGroupHeader(alamat1, 70, ' ', true);
                    gtf.SetGroupHeader("TANGGAL :", 9, ' ', true);
                    gtf.SetGroupHeader(tgglSO, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    if (dt.Rows[0]["ALAMAT3"].ToString() != string.Empty)
                    {
                        if (dt.Rows[0]["ALAMAT4"].ToString() != string.Empty)
                            alamat2 = dt.Rows[0]["ALAMAT3"].ToString() + ", " + dt.Rows[0]["ALAMAT4"].ToString();
                        else
                            alamat2 = dt.Rows[0]["ALAMAT3"].ToString();

                        gtf.SetGroupHeader(alamat2, 55, ' ', true);
                    }
                    else
                    {
                        if (dt.Rows[0]["ALAMAT4"].ToString() != string.Empty)
                        {
                            alamat2 = dt.Rows[0]["ALAMAT4"].ToString();
                            gtf.SetGroupHeader(alamat2, 55, ' ', true);
                        }
                        else
                            gtf.SetGroupHeaderSpace(71);
                    }
                    gtf.SetGroupHeader("No.SKPK :", 9, ' ', true);
                    gtf.SetGroupHeader(SKPKNo, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(71);
                    gtf.SetGroupHeader("No.Reff :", 9, ' ', true);
                    gtf.SetGroupHeader(refferenceNo, 15, ' ', false, true);
                    gtf.SetGroupHeader("Faktur Pajak dibuat atas :", 96, ' ', false, true);
                    gtf.SetGroupHeader("NAMA         :", 14, ' ', true);
                    gtf.SetGroupHeader(namaGovPelanggan, 29, ' ', true);
                    if (leasingName != string.Empty)
                    {
                        gtf.SetGroupHeader("QQ : " + leasingName, 51 - (dt.Rows[0]["LeasingCo"].ToString().Length + 2), ' ', true, false, true);
                        gtf.SetGroupHeader("[" + leasingCo + "]", dt.Rows[0]["LeasingCo"].ToString().Length + 2, ' ', false, true);
                    }
                    else
                        gtf.SetGroupHeader("", 51, ' ', false, true);

                    if (dt.Rows[0]["ALAMAT2"].ToString() != string.Empty)
                    {
                        if (dt.Rows[0]["ALAMAT3"].ToString() != string.Empty)
                        {
                            if (dt.Rows[0]["ALAMAT4"].ToString() != string.Empty)
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT2"].ToString() + ", " + dt.Rows[0]["ALAMAT3"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT4"].ToString();
                            else
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT2"].ToString() + ", " + dt.Rows[0]["ALAMAT3"].ToString();
                        }
                        else
                        {
                            if (dt.Rows[0]["ALAMAT4"].ToString() != string.Empty)
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT2"].ToString() + ", " + dt.Rows[0]["ALAMAT4"].ToString();
                            else
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT2"].ToString();
                        }
                    }
                    else
                    {
                        if (dt.Rows[0]["ALAMAT3"].ToString() != string.Empty)
                        {
                            if (dt.Rows[0]["ALAMAT4"].ToString() != string.Empty)
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT3"].ToString() + ", " + dt.Rows[0]["ALAMAT4"].ToString();
                            else
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT3"].ToString();
                        }
                        else
                        {
                            if (dt.Rows[0]["ALAMAT4"].ToString() != string.Empty)
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT4"].ToString();
                            else
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString();
                        }
                    }
                    gtf.SetGroupHeader(alamatFaktur1, 96, ' ', false, true);
                    gtf.SetGroupHeader("NPWP         :", 14, ' ', true);
                    gtf.SetGroupHeader(NPWPNo, 81, ' ', false, true);
                    gtf.SetGroupHeader("Kami memesan kendaraan kepada Saudara dengan perincian", 96, ' ', false, true);

                    gtf.SetGroupHeaderLine();
                    gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("MODEL", 15, ' ', true);
                    gtf.SetGroupHeader("WARNA", 35, ' ', true);
                    gtf.SetGroupHeader("UNIT", 5, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("HARGA", 15, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("JUMLAH", 15, ' ', false, true, true);
                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();

                    decimal decJumlah = 0;
                    string NoSO = string.Empty;
                    int loop = 0, tempSpace = 0;

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            loop = 30 - gtf.line - 15;
                            for (int j = 0; j < loop; j++)
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                            }

                            gtf.SetDataReportLine();
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("Terbilang : " + gtf.Terbilang(Convert.ToInt32(decJumlah)).ToUpper() + " RUPIAH", 80, ' ', true);
                            gtf.SetDataDetail(decJumlah.ToString(), 15, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);
                            gtf.SetDataReportLine();
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(37);
                            gtf.SetDataDetail("P e r s y a r a t a n", 22, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("Pembayaran : " + dt.Rows[i - 1]["PEMBAYARAN"].ToString(), 23, ' ', true);
                            gtf.SetDataDetail("Perkiraan", 23, ' ', true);
                            gtf.SetDataDetail("Penyerahan : ", 23, ' ', true);
                            gtf.SetDataDetail("Lain-lain  : " + dt.Rows[i - 1]["LAINLAIN"].ToString(), 24, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataReportLine();
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail(dt.Rows[i - 1]["City"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALSO"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                            gtf.PrintData(false, false);
                            if (dt.Rows[i - 1]["BBN"] == null || dt.Rows[i - 1]["BBN"] is DBNull)
                            {
                                gtf.SetDataDetail("- Nilai BBN                : 0", 55, ' ', true);
                            }
                            else
                                gtf.SetDataDetail("- Nilai BBN                : " + Convert.ToDecimal(dt.Rows[i - 1]["BBN"]).ToString("n0"), 55, ' ', true);
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("Menyetujui", 10, ' ');
                            gtf.SetDataDetailSpace(10);
                            gtf.SetDataDetail("Pemesan", 10, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("- PP ini berlaku 30 hari s/d " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALSO"]).AddDays(30).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("- Harga belum termasuk STUJ, BBN, BPKB, dll", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            string tempSplitString1 = string.Empty;
                            string tempSplitString2 = string.Empty;
                            if (dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length > 20)
                            {
                                gtf.SetDataDetail("- PP ini tidak berlaku bila terjadi hal-hal di luar", 75, ' ', true);
                                string[] tempArrayString1 = dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Split(' ');
                                string tempString = string.Empty;
                                foreach (string s in tempArrayString1)
                                {
                                    if (Convert.ToString(tempString + s + ' ').Length < 20)
                                        tempString = tempString += s + ' ';
                                    else
                                        break;
                                }
                                tempSplitString1 = tempString;
                                tempSplitString2 = dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Substring(tempString.Length, dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length - tempString.Length);

                                tempSpace = (20 - tempSplitString1.Length) / 2;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(tempSplitString1, tempSplitString1.Length, ' ', false, true);
                            }
                            else
                            {
                                gtf.SetDataDetail("- PP ini tidak berlaku bila terjadi hal-hal di luar", 96, ' ', false, true);
                            }
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("  kemampuan kami (Force Majeur), seperti perubahan", 55, ' ', true);
                            if (dt.Rows[i - 1]["SignName1"].ToString().Length < 20)
                                tempSpace = (20 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace);
                            if (tempSplitString1 != string.Empty)
                            {
                                tempSpace = (20 - tempSplitString2.Length) / 2;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(tempSplitString2, tempSplitString2.Length, ' ', false, true);
                            }
                            else
                            {
                                if (dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length < 20)
                                    tempSpace = (20 - dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace + 1);
                                gtf.SetDataDetail(dt.Rows[i - 1]["NAMAPELANGGAN"].ToString(), dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length, ' ', false, true);
                            }
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("  Peraturan Pemerintah dalam bidang Perpajakan", 55, ' ', true);
                            gtf.SetDataDetail("-", 19, '-', true);
                            gtf.SetDataDetail("-", 20, '-', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("  dan/atau Moneter dan sebagainya", 55, ' ', true);
                            if (dt.Rows[i - 1]["TitleSign1"].ToString().Length < 20)
                                tempSpace = (20 - dt.Rows[i - 1]["TitleSign1"].ToString().Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 20, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            break;
                        }

                        if (NoSO == string.Empty)
                        {
                            NoSO = dt.Rows[0]["NOMORSO"].ToString();
                        }
                        else
                        {
                            if (NoSO != dt.Rows[i]["NOMORSO"].ToString())
                            {
                                loop = 30 - gtf.line - 15;
                                for (int j = 0; j < loop; j++)
                                {
                                    gtf.SetDataDetail("", 96, ' ', false, true);
                                    gtf.PrintData(false, false);
                                }

                                gtf.SetDataReportLine();
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("Terbilang : " + gtf.Terbilang(Convert.ToInt32(decJumlah)).ToUpper() + " RUPIAH", 80, ' ', true);
                                gtf.SetDataDetail(decJumlah.ToString(), 15, ' ', false, true, true, true, "n0");
                                gtf.PrintData(false, false);
                                gtf.SetDataReportLine();
                                gtf.PrintData(false, false);
                                gtf.SetDataDetailSpace(37);
                                gtf.SetDataDetail("P e r s y a r a t a n", 22, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("Pembayaran : " + dt.Rows[i - 1]["PEMBAYARAN"].ToString(), 23, ' ', true);
                                gtf.SetDataDetail("Perkiraan", 23, ' ', true);
                                gtf.SetDataDetail("Penyerahan : ", 23, ' ', true);
                                gtf.SetDataDetail("Lain-lain  : " + dt.Rows[i - 1]["LAINLAIN"].ToString(), 24, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataReportLine();
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail(dt.Rows[i - 1]["City"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALSO"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                                gtf.PrintData(false, false);
                                if (dt.Rows[i - 1]["BBN"] == null || dt.Rows[i - 1]["BBN"] is DBNull)
                                {
                                    gtf.SetDataDetail("- Nilai BBN                : 0", 55, ' ', true);
                                }
                                else
                                    gtf.SetDataDetail("- Nilai BBN                : " + Convert.ToDecimal(dt.Rows[i - 1]["BBN"]).ToString("n0"), 55, ' ', true);
                                gtf.SetDataDetailSpace(5);
                                gtf.SetDataDetail("Menyetujui", 10, ' ');
                                gtf.SetDataDetailSpace(10);
                                gtf.SetDataDetail("Pemesan", 10, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("- PP ini berlaku 30 hari s/d " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALSO"]).AddDays(30).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("- Harga belum termasuk STUJ, BBN, BPKB, dll", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                string tempSplitString1 = string.Empty;
                                string tempSplitString2 = string.Empty;
                                if (dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length > 20)
                                {
                                    gtf.SetDataDetail("- PP ini tidak berlaku bila terjadi hal-hal di luar", 75, ' ', true);
                                    string[] tempArrayString1 = dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Split(' ');
                                    string tempString = string.Empty;
                                    foreach (string s in tempArrayString1)
                                    {
                                        if (Convert.ToString(tempString + s + ' ').Length < 20)
                                            tempString = tempString += s + ' ';
                                        else
                                            break;
                                    }
                                    tempSplitString1 = tempString;
                                    tempSplitString2 = dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Substring(tempString.Length, dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length - tempString.Length);

                                    tempSpace = (20 - tempSplitString1.Length) / 2;
                                    gtf.SetDataDetailSpace(tempSpace);
                                    gtf.SetDataDetail(tempSplitString1, tempSplitString1.Length, ' ', false, true);
                                }
                                else
                                {
                                    gtf.SetDataDetail("- PP ini tidak berlaku bila terjadi hal-hal di luar", 96, ' ', false, true);
                                }
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("  kemampuan kami (Force Majeur), seperti perubahan", 55, ' ', true);
                                if (dt.Rows[i - 1]["SignName1"].ToString().Length < 20)
                                    tempSpace = (20 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ');
                                gtf.SetDataDetailSpace(tempSpace);
                                if (tempSplitString1 != string.Empty)
                                {
                                    tempSpace = (20 - tempSplitString2.Length) / 2;
                                    gtf.SetDataDetailSpace(tempSpace);
                                    gtf.SetDataDetail(tempSplitString2, tempSplitString2.Length, ' ', false, true);
                                }
                                else
                                {
                                    if (dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length < 20)
                                        tempSpace = (20 - dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length) / 2;
                                    else
                                        tempSpace = 1;
                                    gtf.SetDataDetailSpace(tempSpace + 1);
                                    gtf.SetDataDetail(dt.Rows[i - 1]["NAMAPELANGGAN"].ToString(), dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length, ' ', false, true);
                                }
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("  Peraturan Pemerintah dalam bidang Perpajakan", 55, ' ', true);
                                gtf.SetDataDetail("-", 19, '-', true);
                                gtf.SetDataDetail("-", 20, '-', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("  dan/atau Moneter dan sebagainya", 55, ' ', true);
                                if (dt.Rows[i - 1]["TitleSign1"].ToString().Length < 20)
                                    tempSpace = (20 - dt.Rows[i - 1]["TitleSign1"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 20, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);

                                NoSO = dt.Rows[i]["NOMORSO"].ToString();
                                decJumlah = 0;
                                counterData = 0;

                                gtf.ReplaceGroupHdr(namaPelanggan, dt.Rows[i]["NAMAPELANGGAN"].ToString(), 55);
                                gtf.ReplaceGroupHdr(soNO, dt.Rows[i]["NOMORSO"].ToString(), 15);
                                if (dt.Rows[i]["ALAMAT2"].ToString() != string.Empty)
                                {
                                    gtf.ReplaceGroupHdr(alamat1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " + dt.Rows[i]["ALAMAT2"].ToString(), 70);
                                    alamat1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " + dt.Rows[i]["ALAMAT2"].ToString();
                                }
                                else
                                {
                                    gtf.ReplaceGroupHdr(alamat1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString(), 70);
                                    alamat1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString();
                                }
                                gtf.ReplaceGroupHdr(tgglSO, Convert.ToDateTime(dt.Rows[i]["TANGGALSO"]).ToString("dd-MMM-yyyy"), 15);

                                if (dt.Rows[i]["ALAMAT3"].ToString() != string.Empty)
                                {
                                    if (dt.Rows[i]["ALAMAT4"].ToString() != string.Empty)
                                    {
                                        if (alamat2 == string.Empty || alamat2 == " ")
                                            gtf.ReplaceGroupHdr(alamat2, dt.Rows[i]["ALAMAT3"].ToString() + ", " + dt.Rows[i]["ALAMAT4"].ToString(), 55, 288, 55);
                                        else
                                            gtf.ReplaceGroupHdr(alamat2, dt.Rows[i]["ALAMAT3"].ToString() + ", " + dt.Rows[i]["ALAMAT4"].ToString(), 55);
                                        alamat2 = dt.Rows[i]["ALAMAT3"].ToString() + ", " + dt.Rows[0]["ALAMAT4"].ToString();
                                    }
                                    else
                                    {
                                        gtf.ReplaceGroupHdr(alamat2, dt.Rows[i]["ALAMAT3"].ToString(), 55);
                                        alamat2 = dt.Rows[i]["ALAMAT3"].ToString();
                                    }
                                }
                                else
                                {
                                    if (dt.Rows[i]["ALAMAT4"].ToString() != string.Empty)
                                    {
                                        gtf.ReplaceGroupHdr(alamat2, dt.Rows[i]["ALAMAT4"].ToString(), 55);
                                        alamat2 = dt.Rows[i]["ALAMAT4"].ToString();
                                    }
                                }

                                gtf.ReplaceGroupHdr(SKPKNo, dt.Rows[i]["SKPKNO"].ToString(), 15);
                                gtf.ReplaceGroupHdr(alamat3, dt.Rows[i]["ALAMAT3"].ToString(), 55);
                                gtf.ReplaceGroupHdr(refferenceNo, dt.Rows[i]["RefferenceNo"].ToString(), 15);
                                gtf.ReplaceGroupHdr(alamat4, dt.Rows[i]["ALAMAT4"].ToString(), 81);
                                gtf.ReplaceGroupHdr(leasingName, dt.Rows[i]["LeasingName"].ToString(), 51 - (dt.Rows[i]["LeasingCo"].ToString().Length + 2));
                                gtf.ReplaceGroupHdr(leasingCo, dt.Rows[i]["LeasingCo"].ToString(), dt.Rows[i]["LeasingCo"].ToString().Length + 2);

                                if (dt.Rows[i]["ALAMAT2"].ToString() != string.Empty)
                                {
                                    if (dt.Rows[i]["ALAMAT3"].ToString() != string.Empty)
                                    {
                                        if (dt.Rows[i]["ALAMAT4"].ToString() != string.Empty)
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString() + ", " + dt.Rows[i]["ALAMAT3"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT4"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString() + ", " + dt.Rows[i]["ALAMAT3"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT4"].ToString();
                                        }
                                        else
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString() + ", " + dt.Rows[i]["ALAMAT3"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString() + ", " + dt.Rows[i]["ALAMAT3"].ToString();
                                        }
                                    }
                                    else
                                    {
                                        if (dt.Rows[i]["ALAMAT4"].ToString() != string.Empty)
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString() + ", " + dt.Rows[i]["ALAMAT4"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString() + ", " + dt.Rows[i]["ALAMAT4"].ToString();
                                        }
                                        else
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    if (dt.Rows[i]["ALAMAT3"].ToString() != string.Empty)
                                    {
                                        if (dt.Rows[i]["ALAMAT4"].ToString() != string.Empty)
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT3"].ToString() + ", " + dt.Rows[i]["ALAMAT4"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT3"].ToString() + ", " + dt.Rows[i]["ALAMAT4"].ToString();
                                        }
                                        else
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT3"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT3"].ToString();
                                        }
                                    }
                                    else
                                    {
                                        if (dt.Rows[i]["ALAMAT4"].ToString() != string.Empty)
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT4"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT4"].ToString();
                                        }
                                        else
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString();
                                        }
                                    }
                                }


                                namaPelanggan = dt.Rows[i]["NAMAPELANGGAN"].ToString();
                                soNO = dt.Rows[i]["NOMORSO"].ToString();
                                tgglSO = Convert.ToDateTime(dt.Rows[i]["TANGGALSO"]).ToString("dd-MMM-yyyy");
                                SKPKNo = dt.Rows[i]["SKPKNO"].ToString();
                                refferenceNo = dt.Rows[i]["RefferenceNo"].ToString();
                                namaGovPelanggan = dt.Rows[i]["NAMAGOVPELANGGAN"].ToString();
                                leasingName = dt.Rows[i]["LeasingName"].ToString();
                                leasingCo = dt.Rows[i]["LeasingCo"].ToString();
                                NPWPNo = dt.Rows[i]["NPWPNo"].ToString();
                            }
                        }
                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["MODEL"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["COLOURNAME"].ToString(), 35, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["UNIT"].ToString(), 5, ' ', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["HARGA"].ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["JUMLAH"].ToString(), 15, ' ', false, true, true, true, "n0");

                        decJumlah += Convert.ToDecimal(dt.Rows[i]["JUMLAH"]);

                        gtf.PrintData(false, false);
                    }
                }
                #endregion

                #region OmRpSalesTrn001A
                if (reportID == "OmRpSalesTrn001A")
                {
                    namaPelanggan = dt.Rows[0]["NAMAPELANGGAN"].ToString();
                    soNO = dt.Rows[0]["NOMORSO"].ToString();
                    alamat1 = dt.Rows[0]["ALAMAT1"].ToString();
                    tgglSO = Convert.ToDateTime(dt.Rows[0]["TANGGALSO"]).ToString("dd-MMM-yyyy");
                    alamat2 = dt.Rows[0]["ALAMAT2"].ToString();
                    SKPKNo = dt.Rows[0]["SKPKNO"].ToString();
                    alamat3 = dt.Rows[0]["ALAMAT3"].ToString();
                    refferenceNo = dt.Rows[0]["RefferenceNo"].ToString();
                    alamat4 = dt.Rows[0]["ALAMAT4"].ToString();
                    namaGovPelanggan = dt.Rows[0]["NAMAGOVPELANGGAN"].ToString();
                    TOPName = "TIPE PEMBAYARAN : " + dt.Rows[0]["TOPName"].ToString();
                    leasingName = dt.Rows[0]["Leasing"].ToString();
                    alamatFaktur1 = dt.Rows[0]["ALAMAT1"].ToString();
                    alamatFaktur2 = dt.Rows[0]["ALAMAT2"].ToString();
                    alamatFaktur3 = dt.Rows[0]["ALAMAT3"].ToString();
                    alamatFaktur4 = dt.Rows[0]["ALAMAT4"].ToString();
                    NPWPNo = dt.Rows[0]["NPWPNo"].ToString();

                    gtf.SetGroupHeader("NAMA         :", 14, ' ', true);
                    gtf.SetGroupHeader(namaPelanggan, 55, ' ', true);
                    gtf.SetGroupHeader("NOMOR   :", 9, ' ', true);
                    gtf.SetGroupHeader(soNO, 15, ' ', false, true);
                    gtf.SetGroupHeader("ALAMAT       :", 14, ' ', true);
                    gtf.SetGroupHeader(alamat1, 55, ' ', true);
                    gtf.SetGroupHeader("TANGGAL :", 9, ' ', true);
                    gtf.SetGroupHeader(tgglSO, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamat2, 55, ' ', true);
                    gtf.SetGroupHeader("No.SKPK :", 9, ' ', true);
                    gtf.SetGroupHeader(SKPKNo, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamat3, 55, ' ', true);
                    gtf.SetGroupHeader("No.Reff :", 9, ' ', true);
                    gtf.SetGroupHeader(refferenceNo, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamat4, 81, ' ', false, true);
                    gtf.SetGroupHeader("", 96, ' ', false, true);
                    gtf.SetGroupHeader("Faktur Pajak dibuat atas :", 96, ' ', false, true);
                    gtf.SetGroupHeader("NAMA         :", 14, ' ', true);
                    gtf.SetGroupHeader(namaGovPelanggan, 96 - (TOPName.Length + 18), ' ', true);
                    gtf.SetGroupHeader(TOPName, TOPName.Length, ' ', false, true, true);
                    gtf.SetGroupHeader("ALAMAT       :", 14, ' ', true);
                    gtf.SetGroupHeader(alamatFaktur1, 81, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamatFaktur2, 81, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamatFaktur3, 81, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamatFaktur4, 81, ' ', false, true);
                    gtf.SetGroupHeader("NPWP         :", 14, ' ', true);
                    gtf.SetGroupHeader(NPWPNo, 81, ' ', false, true);
                    gtf.SetGroupHeader("LEASING      :", 14, ' ', true);
                    gtf.SetGroupHeader(leasingName, 81, ' ', false, true);
                    gtf.SetGroupHeader("Kami memesan kendaraan kepada Saudara dengan perincian", 96, ' ', false, true);

                    gtf.SetGroupHeaderLine();
                    gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("MODEL", 15, ' ', true);
                    gtf.SetGroupHeader("MODEL YEAR", 10, ' ', true, false, true);
                    gtf.SetGroupHeader("WARNA", 25, ' ', true);
                    gtf.SetGroupHeader("UNIT", 5, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("NO. RANGKA", 11, ' ', true);
                    gtf.SetGroupHeader("NO. MESIN", 11, ' ', true);
                    gtf.SetGroupHeader("KOREKSI", 7, ' ', false, true);
                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();

                    string NoSO = string.Empty;
                    int tempSpace = 0, loop = 0;
                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            loop = 59 - gtf.line - 15;
                            for (int j = 0; j < loop; j++)
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                            }

                            gtf.SetDataReportLine();
                            gtf.PrintData();
                            gtf.SetDataDetail("Total : " + counterData.ToString() + " Unit", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("* Kendaraan dalam keadaan baik, kehilangan, kerusakan", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("  setelah diterima, bukan menjadi tanggungan kami lagi", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail(dt.Rows[i - 1]["City"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALSO"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(50);
                            gtf.SetDataDetail("Diserahkan Oleh :", 17, ' ');
                            gtf.SetDataDetailSpace(8);
                            gtf.SetDataDetail("Diterima Oleh :", 17, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(46);
                            if (dt.Rows[i - 1]["SignName1"].ToString().Length < 25)
                                tempSpace = (25 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace);
                            if (dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length < 25)
                                tempSpace = (25 - dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace + 1);
                            gtf.SetDataDetail(dt.Rows[i - 1]["NAMAPELANGGAN"].ToString(), dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(46);
                            gtf.SetDataDetail("-", 24, '-', true);
                            gtf.SetDataDetail("-", 25, '-', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(46);
                            if (dt.Rows[i - 1]["TitleSign1"].ToString().Length < 25)
                                tempSpace = (25 - dt.Rows[i - 1]["TitleSign1"].ToString().Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 25, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            break;
                        }

                        if (NoSO == string.Empty)
                        {
                            NoSO = dt.Rows[0]["NOMORSO"].ToString();
                        }
                        else
                        {
                            if (NoSO != dt.Rows[i]["NOMORSO"].ToString())
                            {
                                loop = 59 - gtf.line - 15;
                                for (int j = 0; j < loop; j++)
                                {
                                    gtf.SetDataDetail("", 96, ' ', false, true);
                                    gtf.PrintData();
                                }

                                gtf.SetDataReportLine();
                                gtf.PrintData();
                                gtf.SetDataDetail("Total : " + counterData.ToString() + " Unit", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("* Kendaraan dalam keadaan baik, kehilangan, kerusakan", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("  setelah diterima, bukan menjadi tanggungan kami lagi", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail(dt.Rows[i - 1]["City"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALSO"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                                gtf.PrintData();
                                gtf.SetDataDetailSpace(50);
                                gtf.SetDataDetail("Diserahkan Oleh :", 17, ' ');
                                gtf.SetDataDetailSpace(8);
                                gtf.SetDataDetail("Diterima Oleh :", 17, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetailSpace(46);
                                if (dt.Rows[i - 1]["SignName1"].ToString().Length < 25)
                                    tempSpace = (25 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ');
                                gtf.SetDataDetailSpace(tempSpace);
                                if (dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length < 25)
                                    tempSpace = (25 - dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace + 1);
                                gtf.SetDataDetail(dt.Rows[i - 1]["NAMAPELANGGAN"].ToString(), dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetailSpace(46);
                                gtf.SetDataDetail("-", 24, '-', true);
                                gtf.SetDataDetail("-", 25, '-', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetailSpace(46);
                                if (dt.Rows[i - 1]["TitleSign1"].ToString().Length < 25)
                                    tempSpace = (25 - dt.Rows[i - 1]["TitleSign1"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 25, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();

                                NoSO = dt.Rows[i]["NOMORSO"].ToString();
                                counterData = 0;

                                gtf.ReplaceGroupHdr(namaPelanggan, dt.Rows[i]["NAMAPELANGGAN"].ToString(), 55);
                                gtf.ReplaceGroupHdr(soNO, dt.Rows[i]["NOMORSO"].ToString(), 15);
                                gtf.ReplaceGroupHdr(alamat1, dt.Rows[i]["ALAMAT1"].ToString(), 55);
                                gtf.ReplaceGroupHdr(tgglSO, Convert.ToDateTime(dt.Rows[i]["TANGGALSO"]).ToString("dd-MMM-yyyy"), 15);
                                gtf.ReplaceGroupHdr(alamat2, dt.Rows[i]["ALAMAT2"].ToString(), 55);
                                gtf.ReplaceGroupHdr(SKPKNo, dt.Rows[i]["SKPKNO"].ToString(), 15);
                                gtf.ReplaceGroupHdr(alamat3, dt.Rows[i]["ALAMAT3"].ToString(), 55);
                                gtf.ReplaceGroupHdr(refferenceNo, dt.Rows[i]["RefferenceNo"].ToString(), 15);
                                gtf.ReplaceGroupHdr(alamat4, dt.Rows[i]["ALAMAT4"].ToString(), 81);
                                gtf.ReplaceGroupHdr(namaGovPelanggan, dt.Rows[i]["NAMAGOVPELANGGAN"].ToString(), 96 - (TOPName.Length + 18));
                                gtf.ReplaceGroupHdr(TOPName, "TIPE PEMBAYARAN : " + dt.Rows[i]["TOPName"].ToString(), TOPName.Length);
                                gtf.ReplaceGroupHdr(alamatFaktur1, dt.Rows[i]["ALAMAT1"].ToString(), 81);
                                gtf.ReplaceGroupHdr(alamatFaktur2, dt.Rows[i]["ALAMAT2"].ToString(), 81);
                                gtf.ReplaceGroupHdr(alamatFaktur3, dt.Rows[i]["ALAMAT3"].ToString(), 81);
                                gtf.ReplaceGroupHdr(alamatFaktur4, dt.Rows[i]["ALAMAT4"].ToString(), 81);
                                gtf.ReplaceGroupHdr(NPWPNo, dt.Rows[i]["NPWPNo"].ToString(), 81);
                                gtf.ReplaceGroupHdr(leasingName, dt.Rows[i]["Leasing"].ToString(), 81);

                                namaPelanggan = dt.Rows[i]["NAMAPELANGGAN"].ToString();
                                soNO = dt.Rows[i]["NOMORSO"].ToString();
                                alamat1 = dt.Rows[i]["ALAMAT1"].ToString();
                                tgglSO = Convert.ToDateTime(dt.Rows[i]["TANGGALSO"]).ToString("dd-MMM-yyyy");
                                alamat2 = dt.Rows[i]["ALAMAT2"].ToString();
                                SKPKNo = dt.Rows[i]["SKPKNO"].ToString();
                                alamat3 = dt.Rows[i]["ALAMAT3"].ToString();
                                refferenceNo = dt.Rows[i]["RefferenceNo"].ToString();
                                alamat4 = dt.Rows[i]["ALAMAT4"].ToString();
                                namaGovPelanggan = dt.Rows[i]["NAMAGOVPELANGGAN"].ToString();
                                TOPName = "TIPE PEMBAYARAN : " + dt.Rows[i]["TOPName"].ToString();
                                leasingName = dt.Rows[i]["Leasing"].ToString();
                                alamatFaktur1 = dt.Rows[i]["ALAMAT1"].ToString();
                                alamatFaktur2 = dt.Rows[i]["ALAMAT2"].ToString();
                                alamatFaktur3 = dt.Rows[i]["ALAMAT3"].ToString();
                                alamatFaktur4 = dt.Rows[i]["ALAMAT4"].ToString();
                                NPWPNo = dt.Rows[i]["NPWPNo"].ToString();
                            }
                        }

                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["MODEL"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["TAHUN"].ToString(), 10, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["COLOURNAME"].ToString(), 25, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["UNIT"].ToString(), 5, ' ', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["Rangka"].ToString(), 11, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Mesin"].ToString(), 11, ' ', true);
                        gtf.SetDataDetail(" [   ] ", 7, ' ', false, true);

                        gtf.PrintData(true);
                    }
                }
                #endregion

                #region OmRpSalesTrn001E
                if (reportID == "OmRpSalesTrn001E")
                {
                    namaPelanggan = dt.Rows[0]["NAMAPELANGGAN"].ToString();
                    soNO = dt.Rows[0]["NOMORSO"].ToString();
                    tgglSO = Convert.ToDateTime(dt.Rows[0]["TANGGALSO"]).ToString("dd-MMM-yyyy");
                    SKPKNo = dt.Rows[0]["SKPKNO"].ToString();
                    alamat3 = dt.Rows[0]["ALAMAT3"].ToString();
                    refferenceNo = dt.Rows[0]["RefferenceNo"].ToString();
                    alamat4 = dt.Rows[0]["ALAMAT4"].ToString();
                    namaGovPelanggan = dt.Rows[0]["NAMAGOVPELANGGAN"].ToString();
                    TOPName = "TIPE PEMBAYARAN : " + dt.Rows[0]["TOPName"].ToString();
                    leasingName = dt.Rows[0]["Leasing"].ToString();
                    NPWPNo = dt.Rows[0]["NPWPNo"].ToString();

                    gtf.SetGroupHeader("PESANAN PENJUALAN", 96, ' ', false, true);
                    gtf.SetGroupHeader("NAMA         :", 14, ' ', true);
                    gtf.SetGroupHeader(namaPelanggan, 55, ' ', true);
                    gtf.SetGroupHeader("NOMOR   :", 9, ' ', true);
                    gtf.SetGroupHeader(soNO, 15, ' ', false, true);
                    if (dt.Rows[0]["ALAMAT2"].ToString() != string.Empty)
                        alamat1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " + dt.Rows[0]["ALAMAT2"].ToString();
                    else
                        alamat1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString();
                    gtf.SetGroupHeader(alamat1, 70, ' ', true);
                    gtf.SetGroupHeader("TANGGAL :", 9, ' ', true);
                    gtf.SetGroupHeader(tgglSO, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    if (dt.Rows[0]["ALAMAT3"].ToString() != string.Empty)
                    {
                        if (dt.Rows[0]["ALAMAT4"].ToString() != string.Empty)
                            alamat2 = dt.Rows[0]["ALAMAT3"].ToString() + ", " + dt.Rows[0]["ALAMAT4"].ToString();
                        else
                            alamat2 = dt.Rows[0]["ALAMAT3"].ToString();

                        gtf.SetGroupHeader(alamat2, 55, ' ', true);
                    }
                    else
                    {
                        if (dt.Rows[0]["ALAMAT4"].ToString() != string.Empty)
                        {
                            alamat2 = dt.Rows[0]["ALAMAT4"].ToString();
                            gtf.SetGroupHeader(alamat2, 55, ' ', true);
                        }
                        else
                            gtf.SetGroupHeaderSpace(56);
                    }

                    gtf.SetGroupHeader("No.SKPK :", 9, ' ', true);
                    gtf.SetGroupHeader(SKPKNo, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(71);
                    gtf.SetGroupHeader("No.Reff :", 9, ' ', true);
                    gtf.SetGroupHeader(refferenceNo, 15, ' ', false, true);
                    gtf.SetGroupHeader("Faktur Pajak dibuat atas :", 96, ' ', false, true);
                    gtf.SetGroupHeader("NAMA         :", 14, ' ', true);
                    gtf.SetGroupHeader(namaGovPelanggan, 96 - (TOPName.Length + 15), ' ', true);
                    gtf.SetGroupHeader(TOPName, TOPName.Length, ' ', false, true, true);
                    if (dt.Rows[0]["ALAMAT2"].ToString() != string.Empty)
                    {
                        if (dt.Rows[0]["ALAMAT3"].ToString() != string.Empty)
                        {
                            if (dt.Rows[0]["ALAMAT4"].ToString() != string.Empty)
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT2"].ToString() + ", " + dt.Rows[0]["ALAMAT3"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT4"].ToString();
                            else
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT2"].ToString() + ", " + dt.Rows[0]["ALAMAT3"].ToString();
                        }
                        else
                        {
                            if (dt.Rows[0]["ALAMAT4"].ToString() != string.Empty)
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT2"].ToString() + ", " + dt.Rows[0]["ALAMAT4"].ToString();
                            else
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT2"].ToString();
                        }
                    }
                    else
                    {
                        if (dt.Rows[0]["ALAMAT3"].ToString() != string.Empty)
                        {
                            if (dt.Rows[0]["ALAMAT4"].ToString() != string.Empty)
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT3"].ToString() + ", " + dt.Rows[0]["ALAMAT4"].ToString();
                            else
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT3"].ToString();
                        }
                        else
                        {
                            if (dt.Rows[0]["ALAMAT4"].ToString() != string.Empty)
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString() + ", " +
                                    dt.Rows[0]["ALAMAT4"].ToString();
                            else
                                alamatFaktur1 = "ALAMAT       : " + dt.Rows[0]["ALAMAT1"].ToString();
                        }
                    }
                    gtf.SetGroupHeader(alamatFaktur1, 96, ' ', false, true);
                    gtf.SetGroupHeader("NPWP         :", 14, ' ', true);
                    gtf.SetGroupHeader(NPWPNo, 81, ' ', false, true);
                    gtf.SetGroupHeader("LEASING      :", 14, ' ', true);
                    gtf.SetGroupHeader(leasingName, 81, ' ', false, true);
                    gtf.SetGroupHeader("Kami memesan kendaraan kepada Saudara dengan perincian", 96, ' ', false, true);

                    gtf.SetGroupHeaderLine();
                    gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("MODEL", 15, ' ', true);
                    gtf.SetGroupHeader("MODEL YEAR", 10, ' ', true, false, true);
                    gtf.SetGroupHeader("WARNA", 25, ' ', true);
                    gtf.SetGroupHeader("UNIT", 5, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("NO. RANGKA", 11, ' ', true);
                    gtf.SetGroupHeader("NO. MESIN", 11, ' ', true);
                    gtf.SetGroupHeader("KOREKSI", 7, ' ', false, true);
                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();

                    string NoSO = string.Empty;
                    int tempSpace = 0, loop = 0;
                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            loop = 30 - gtf.line - 10;
                            for (int j = 0; j < loop; j++)
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                            }

                            gtf.SetDataReportLine();
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("Total : " + counterData.ToString() + " Unit", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("* Kendaraan dalam keadaan baik, kehilangan, kerusakan", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("  setelah diterima, bukan menjadi tanggungan kami lagi", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail(dt.Rows[i - 1]["City"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALSO"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(50);
                            gtf.SetDataDetail("Diserahkan Oleh :", 17, ' ');
                            gtf.SetDataDetailSpace(8);
                            gtf.SetDataDetail("Diterima Oleh :", 17, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(46);
                            if (dt.Rows[i - 1]["SignName1"].ToString().Length < 25)
                                tempSpace = (25 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ');
                            if (dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length < 25)
                                tempSpace = (25 - dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace + 1);
                            gtf.SetDataDetail(dt.Rows[i - 1]["NAMAPELANGGAN"].ToString(), dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(46);
                            gtf.SetDataDetail("-", 24, '-', true);
                            gtf.SetDataDetail("-", 25, '-', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(46);
                            if (dt.Rows[i - 1]["TitleSign1"].ToString().Length < 25)
                                tempSpace = (25 - dt.Rows[i - 1]["TitleSign1"].ToString().Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 25, ' ', false, true);
                            gtf.PrintData(false, false);

                            break;
                        }

                        if (NoSO == string.Empty)
                        {
                            NoSO = dt.Rows[0]["NOMORSO"].ToString();
                        }
                        else
                        {
                            if (NoSO != dt.Rows[i]["NOMORSO"].ToString())
                            {
                                loop = 30 - gtf.line - 10;
                                for (int j = 0; j < loop; j++)
                                {
                                    gtf.SetDataDetail("", 96, ' ', false, true);
                                    gtf.PrintData(false, false);
                                }

                                gtf.SetDataReportLine();
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("Total : " + counterData.ToString() + " Unit", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("* Kendaraan dalam keadaan baik, kehilangan, kerusakan", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("  setelah diterima, bukan menjadi tanggungan kami lagi", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail(dt.Rows[i - 1]["City"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALSO"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetailSpace(50);
                                gtf.SetDataDetail("Diserahkan Oleh :", 17, ' ');
                                gtf.SetDataDetailSpace(8);
                                gtf.SetDataDetail("Diterima Oleh :", 17, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetailSpace(46);
                                if (dt.Rows[i - 1]["SignName1"].ToString().Length < 25)
                                    tempSpace = (25 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ');
                                gtf.SetDataDetailSpace(tempSpace);
                                if (dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length < 25)
                                    tempSpace = (25 - dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace + 1);
                                gtf.SetDataDetail(dt.Rows[i - 1]["NAMAPELANGGAN"].ToString(), dt.Rows[i - 1]["NAMAPELANGGAN"].ToString().Length, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetailSpace(46);
                                gtf.SetDataDetail("-", 24, '-', true);
                                gtf.SetDataDetail("-", 25, '-', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetailSpace(46);
                                if (dt.Rows[i - 1]["TitleSign1"].ToString().Length < 25)
                                    tempSpace = (25 - dt.Rows[i - 1]["TitleSign1"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 25, ' ', false, true);
                                gtf.PrintData(false, false);

                                NoSO = dt.Rows[i]["NOMORSO"].ToString();
                                counterData = 0;

                                gtf.ReplaceGroupHdr(namaPelanggan, dt.Rows[i]["NAMAPELANGGAN"].ToString(), 55);
                                gtf.ReplaceGroupHdr(soNO, dt.Rows[i]["NOMORSO"].ToString(), 15);
                                if (dt.Rows[i]["ALAMAT2"].ToString() != string.Empty)
                                {
                                    gtf.ReplaceGroupHdr(alamat1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " + dt.Rows[i]["ALAMAT2"].ToString(), 70);
                                    alamat1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " + dt.Rows[i]["ALAMAT2"].ToString();
                                }
                                else
                                {
                                    gtf.ReplaceGroupHdr(alamat1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString(), 70);
                                    alamat1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString();
                                }

                                if (dt.Rows[i]["ALAMAT3"].ToString() != string.Empty)
                                {
                                    if (dt.Rows[i]["ALAMAT4"].ToString() != string.Empty)
                                    {
                                        if (alamat2 == string.Empty || alamat2 == " ")
                                            gtf.ReplaceGroupHdr(alamat2, dt.Rows[i]["ALAMAT3"].ToString() + ", " + dt.Rows[i]["ALAMAT4"].ToString(), 55, 288, 55);
                                        else
                                            gtf.ReplaceGroupHdr(alamat2, dt.Rows[i]["ALAMAT3"].ToString() + ", " + dt.Rows[i]["ALAMAT4"].ToString(), 55);
                                        alamat2 = dt.Rows[i]["ALAMAT3"].ToString() + ", " + dt.Rows[0]["ALAMAT4"].ToString();
                                    }
                                    else
                                    {
                                        gtf.ReplaceGroupHdr(alamat2, dt.Rows[i]["ALAMAT3"].ToString(), 55);
                                        alamat2 = dt.Rows[i]["ALAMAT3"].ToString();
                                    }
                                }
                                else
                                {
                                    if (dt.Rows[i]["ALAMAT4"].ToString() != string.Empty)
                                    {
                                        gtf.ReplaceGroupHdr(alamat2, dt.Rows[i]["ALAMAT4"].ToString(), 55);
                                        alamat2 = dt.Rows[i]["ALAMAT4"].ToString();
                                    }
                                }

                                if (dt.Rows[i]["ALAMAT2"].ToString() != string.Empty)
                                {
                                    if (dt.Rows[i]["ALAMAT3"].ToString() != string.Empty)
                                    {
                                        if (dt.Rows[i]["ALAMAT4"].ToString() != string.Empty)
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString() + ", " + dt.Rows[i]["ALAMAT3"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT4"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString() + ", " + dt.Rows[i]["ALAMAT3"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT4"].ToString();
                                        }
                                        else
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString() + ", " + dt.Rows[i]["ALAMAT3"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString() + ", " + dt.Rows[i]["ALAMAT3"].ToString();
                                        }
                                    }
                                    else
                                    {
                                        if (dt.Rows[i]["ALAMAT4"].ToString() != string.Empty)
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString() + ", " + dt.Rows[i]["ALAMAT4"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString() + ", " + dt.Rows[i]["ALAMAT4"].ToString();
                                        }
                                        else
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT2"].ToString();
                                        }
                                    }
                                }
                                else
                                {
                                    if (dt.Rows[i]["ALAMAT3"].ToString() != string.Empty)
                                    {
                                        if (dt.Rows[i]["ALAMAT4"].ToString() != string.Empty)
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT3"].ToString() + ", " + dt.Rows[i]["ALAMAT4"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT3"].ToString() + ", " + dt.Rows[i]["ALAMAT4"].ToString();
                                        }
                                        else
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT3"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT3"].ToString();
                                        }
                                    }
                                    else
                                    {
                                        if (dt.Rows[i]["ALAMAT4"].ToString() != string.Empty)
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT4"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString() + ", " +
                                                dt.Rows[i]["ALAMAT4"].ToString();
                                        }
                                        else
                                        {
                                            gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString(), 96);
                                            alamatFaktur1 = "ALAMAT       : " + dt.Rows[i]["ALAMAT1"].ToString();
                                        }
                                    }
                                }
                                gtf.ReplaceGroupHdr(tgglSO, Convert.ToDateTime(dt.Rows[i]["TANGGALSO"]).ToString("dd-MMM-yyyy"), 15);
                                gtf.ReplaceGroupHdr(SKPKNo, dt.Rows[i]["SKPKNO"].ToString(), 15);
                                gtf.ReplaceGroupHdr(refferenceNo, dt.Rows[i]["RefferenceNo"].ToString(), 15);
                                gtf.ReplaceGroupHdr(namaGovPelanggan, dt.Rows[i]["NAMAGOVPELANGGAN"].ToString(), 96 - (TOPName.Length + 15));
                                gtf.ReplaceGroupHdr(TOPName, "TIPE PEMBAYARAN : " + dt.Rows[i]["TOPName"].ToString(), TOPName.Length);
                                gtf.ReplaceGroupHdr(NPWPNo, dt.Rows[i]["NPWPNo"].ToString(), 81);
                                gtf.ReplaceGroupHdr(leasingName, dt.Rows[i]["Leasing"].ToString(), 81);

                                namaPelanggan = dt.Rows[i]["NAMAPELANGGAN"].ToString();
                                soNO = dt.Rows[i]["NOMORSO"].ToString();
                                tgglSO = Convert.ToDateTime(dt.Rows[i]["TANGGALSO"]).ToString("dd-MMM-yyyy");
                                SKPKNo = dt.Rows[i]["SKPKNO"].ToString();
                                refferenceNo = dt.Rows[i]["RefferenceNo"].ToString();
                                namaGovPelanggan = dt.Rows[i]["NAMAGOVPELANGGAN"].ToString();
                                TOPName = "TIPE PEMBAYARAN : " + dt.Rows[i]["TOPName"].ToString();
                                leasingName = dt.Rows[i]["Leasing"].ToString();
                                NPWPNo = dt.Rows[i]["NPWPNo"].ToString();
                            }
                        }

                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["MODEL"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["TAHUN"].ToString(), 10, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["COLOURNAME"].ToString(), 25, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["UNIT"].ToString(), 5, ' ', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["Rangka"].ToString(), 11, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Mesin"].ToString(), 11, ' ', true);
                        gtf.SetDataDetail(" [   ] ", 7, ' ', false, true);

                        gtf.PrintData(true);
                    }
                }
                #endregion

                #region OmRpSalesTrn001B / OmRpSalesTrn001F
                if (reportID == "OmRpSalesTrn001B" || reportID == "OmRpSalesTrn001F")
                {
                    sales = "NAMA S/E  : " + dt.Rows[0]["Sales"].ToString();
                    tipeSales = "TP. SALES : " + dt.Rows[0]["TipeSales"].ToString() + " REFF : " + dt.Rows[0]["SKPKNo"].ToString();
                    namaPelanggan = "PELANGGAN : " + dt.Rows[0]["Pelanggan"].ToString();
                    soNO = dt.Rows[0]["SONO"].ToString();
                    requestDate = Convert.ToDateTime(dt.Rows[0]["RequestDate"]).ToString("dd-MMM-yyyy");
                    shipName = "DIKIRIMKAN KE : " + dt.Rows[0]["ShipName"].ToString();
                    soDate = Convert.ToDateTime(dt.Rows[0]["SODate"]).ToString("dd-MMM-yyyy");
                    ket = "KET       : " + dt.Rows[0]["Ket"].ToString();
                    SKPKNo = dt.Rows[0]["SKPKNO"].ToString();
                    refferenceNo = dt.Rows[0]["RefferenceNo"].ToString();

                    gtf.SetGroupHeader(sales, 96, ' ', false, true);
                    gtf.SetGroupHeader(tipeSales, 96, ' ', false, true);
                    gtf.SetGroupHeader("", 96, ' ', false, true);
                    gtf.SetGroupHeader(namaPelanggan, 67, ' ', true);
                    gtf.SetGroupHeader("NOMOR    :", 10, ' ', true);
                    gtf.SetGroupHeader(soNO, 17, ' ', false, true);
                    gtf.SetGroupHeader("ESTIMASI  : ", 12, ' ', true);
                    if (Convert.ToDateTime(dt.Rows[0]["RequestDate"]).ToShortDateString() == "1/1/1900")
                        gtf.SetGroupHeaderSpace(12);
                    else
                        gtf.SetGroupHeader(requestDate, 11, ' ', true);
                    gtf.SetGroupHeader(shipName, 42, ' ', true);
                    gtf.SetGroupHeader("TANGGAL  :", 10, ' ', true);
                    gtf.SetGroupHeader(soDate, 15, ' ', false, true);
                    gtf.SetGroupHeader(ket, 67, ' ', true);
                    gtf.SetGroupHeader("No. SKPK :", 10, ' ', true);
                    gtf.SetGroupHeader(SKPKNo, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(68);
                    gtf.SetGroupHeader("No. Reff :", 10, ' ', true);
                    gtf.SetGroupHeader(refferenceNo, 15, ' ', false, true);

                    gtf.SetGroupHeaderLine();
                    gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
                    gtf.SetGroupHeader("MODEL", 8, ' ', true);
                    gtf.SetGroupHeader("WARNA", 5, ' ', true);
                    gtf.SetGroupHeader("HARGA STDR", 10, ' ', true, false, true);
                    gtf.SetGroupHeader("QTY", 3, ' ', true, false, true);
                    gtf.SetGroupHeader("TOTAL", 10, ' ', true, false, true);
                    gtf.SetGroupHeader("BBN/KIR", 10, ' ', true, false, true);
                    gtf.SetGroupHeader("ASESORIS", 8, ' ', true, false, true);
                    gtf.SetGroupHeader("POTONGAN", 10, ' ', true, false, true);
                    gtf.SetGroupHeader("LAIN-LAIN", 9, ' ', true, false, true);
                    gtf.SetGroupHeader("JUMLAH", 10, ' ', false, true, true);
                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();

                    string SONo = string.Empty;
                    decimal decTotQty = 0, decTotTotal = 0, decTotBBN = 0, decTotAccesories = 0, decTotPotongan = 0, decTotLainLain = 0, decTotSubTotal = 0;
                    int loop = 0;
                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            if (reportID == "OmRpSalesTrn001B")
                                loop = 59 - gtf.line - 12;
                            else
                                loop = 30 - gtf.line - 12;

                            for (int k = 0; k < loop; k++)
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                            }

                            gtf.SetDataDetailSpace(30);
                            gtf.SetDataDetail("-", 66, '-', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(19);
                            gtf.SetDataDetail("Total :", 10, ' ', true);
                            gtf.SetDataDetail(decTotQty.ToString(), 3, ' ', true, false, true);
                            gtf.SetDataDetail(decTotTotal.ToString(), 10, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decTotBBN.ToString(), 10, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decTotAccesories.ToString(), 8, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decTotPotongan.ToString(), 10, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decTotLainLain.ToString(), 9, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decTotSubTotal.ToString(), 10, ' ', false, true, true, true, "n0");
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(30);
                            gtf.SetDataDetail("-", 66, '-', false, true);
                            gtf.PrintData();
                            gtf.SetDataReportLine();
                            gtf.PrintData();
                            gtf.SetDataDetail("T.O.P       : " + dt.Rows[i - 1]["TOPCode"].ToString(), 29, ' ', true);
                            gtf.SetDataDetail("UANG MUKA : " + Convert.ToDecimal(dt.Rows[i - 1]["PrePaymentAmt"]).ToString("n0"), 66, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("JATUH TEMPO : " + Convert.ToDateTime(dt.Rows[i - 1]["JatuhTempo"]).ToString("dd-MMM-yyyy"), 29, ' ', true);
                            gtf.SetDataDetail("LEASING   : " + dt.Rows[i - 1]["Leasing"].ToString(), 66, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("Menyetujui Pengirim :", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            //gtf.SetDataDetail(dt.Rows[i - 1]["SignName"].ToString(), 96, ' ', false, true);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SignName"].ToString(), 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("-", 21, '-', false, true);
                            gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign"].ToString(), 96, ' ', false, true);
                            gtf.PrintData();
                            //gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            break;
                        }

                        if (SONo == string.Empty)
                        {
                            SONo = dt.Rows[i]["SONo"].ToString();
                        }
                        else
                        {
                            if (SONo != dt.Rows[i]["SONo"].ToString())
                            {
                                if (reportID == "OmRpSalesTrn001B")
                                    loop = 59 - gtf.line - 12;
                                else
                                    loop = 30 - gtf.line - 12;

                                for (int k = 0; k < loop; k++)
                                {
                                    gtf.SetDataDetail("", 96, ' ', false, true);
                                    gtf.PrintData();
                                }

                                gtf.SetDataDetailSpace(30);
                                gtf.SetDataDetail("-", 66, '-', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetailSpace(19);
                                gtf.SetDataDetail("Total :", 10, ' ', true);
                                gtf.SetDataDetail(decTotQty.ToString(), 3, ' ', true, false, true);
                                gtf.SetDataDetail(decTotTotal.ToString(), 10, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decTotBBN.ToString(), 10, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decTotAccesories.ToString(), 8, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decTotPotongan.ToString(), 10, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decTotLainLain.ToString(), 9, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decTotSubTotal.ToString(), 10, ' ', false, true, true, true, "n0");
                                gtf.PrintData();
                                gtf.SetDataDetailSpace(30);
                                gtf.SetDataDetail("-", 66, '-', false, true);
                                gtf.PrintData();
                                gtf.SetDataReportLine();
                                gtf.PrintData();
                                gtf.SetDataDetail("T.O.P       : " + dt.Rows[i - 1]["TOPCode"].ToString(), 29, ' ', true);
                                gtf.SetDataDetail("UANG MUKA : " + Convert.ToDecimal(dt.Rows[i - 1]["PrePaymentAmt"]).ToString("n0"), 66, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("JATUH TEMPO : " + Convert.ToDateTime(dt.Rows[i - 1]["JatuhTempo"]).ToString("dd-MMM-yyyy"), 29, ' ', true);
                                gtf.SetDataDetail("LEASING   : " + dt.Rows[i - 1]["Leasing"].ToString(), 66, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("Menyetujui Pengirim :", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail(dt.Rows[i - 1]["SignName"].ToString(), 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("-", 21, '-', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();

                                SONo = dt.Rows[i]["SONo"].ToString();
                                counterData = 0;

                                gtf.ReplaceGroupHdr(sales, "NAMA S/E  : " + dt.Rows[i]["Sales"].ToString(), 96);
                                gtf.ReplaceGroupHdr(tipeSales, "TP. SALES : " + dt.Rows[i]["TipeSales"].ToString() + " REFF : " + dt.Rows[i]["SKPKNo"].ToString(), 96);
                                gtf.ReplaceGroupHdr(namaPelanggan, "PELANGGAN : " + dt.Rows[i]["Pelanggan"].ToString(), 69);
                                gtf.ReplaceGroupHdr(soNO, dt.Rows[i]["SONO"].ToString(), 15);
                                if (Convert.ToDateTime(dt.Rows[i]["RequestDate"]).ToShortDateString() == "1/1/1900")
                                {
                                    //
                                }
                                else
                                    gtf.ReplaceGroupHdr(requestDate, Convert.ToDateTime(dt.Rows[i]["RequestDate"]).ToString("dd-MMM-yyyy"), 11);
                                gtf.ReplaceGroupHdr(shipName, "DIKIRIMKAN KE : " + dt.Rows[i]["ShipName"].ToString(), 44);
                                gtf.ReplaceGroupHdr(soDate, Convert.ToDateTime(dt.Rows[i]["SODate"]).ToString("dd-MMM-yyyy"), 15);
                                gtf.ReplaceGroupHdr(ket, "KET       : " + dt.Rows[i]["Ket"].ToString(), 69);
                                gtf.ReplaceGroupHdr(SKPKNo, dt.Rows[i]["SKPKNO"].ToString(), 15);
                                gtf.ReplaceGroupHdr(refferenceNo, dt.Rows[i]["RefferenceNo"].ToString(), 15);

                                sales = "NAMA S/E  : " + dt.Rows[i]["Sales"].ToString();
                                tipeSales = "TP. SALES : " + dt.Rows[i]["TipeSales"].ToString() + " REFF : " + dt.Rows[i]["SKPKNo"].ToString();
                                namaPelanggan = "PELANGGAN : " + dt.Rows[i]["Pelanggan"].ToString();
                                soNO = dt.Rows[i]["SONO"].ToString();
                                requestDate = Convert.ToDateTime(dt.Rows[i]["RequestDate"]).ToString("dd-MMM-yyyy");
                                shipName = "DIKIRIMKAN KE : " + dt.Rows[i]["ShipName"].ToString();
                                soDate = Convert.ToDateTime(dt.Rows[i]["SODate"]).ToString("dd-MMM-yyyy");
                                ket = "KET       : " + dt.Rows[i]["Ket"].ToString();
                                SKPKNo = dt.Rows[i]["SKPKNO"].ToString();
                                refferenceNo = dt.Rows[i]["RefferenceNo"].ToString();
                            }
                        }

                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 15, ' ', false);                        
                        gtf.SetDataDetail(dt.Rows[i]["BeforeDiscTotal"].ToString(), 10, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["Quantity"].ToString(), 3, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["Total"].ToString(), 10, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["BBN"].ToString(), 10, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["Accesories"].ToString(), 8, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["Potongan"].ToString(), 10, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["LainLain"].ToString(), 9, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["SubTotal"].ToString(), 10, ' ', false, true, true, true, "n0");

                        gtf.SetDataDetailSpace(4);
                        gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(),51 , ' ',false,false);
                        gtf.SetDataDetail(string.Format(@"Chassis No : {0} \ Engine No : {1}",dt.Rows[i]["ChassisNo"].ToString(),dt.Rows[i]["EngineNo"].ToString()),41 , ' ',false,true);
                        

                        gtf.PrintData(true);

                        decTotQty += Convert.ToDecimal(dt.Rows[i]["Quantity"]);
                        decTotTotal += Convert.ToDecimal(dt.Rows[i]["Total"]);
                        decTotBBN += Convert.ToDecimal(dt.Rows[i]["BBN"]);
                        decTotAccesories += Convert.ToDecimal(dt.Rows[i]["Accesories"]);
                        decTotPotongan += Convert.ToDecimal(dt.Rows[i]["Potongan"]);
                        decTotLainLain += Convert.ToDecimal(dt.Rows[i]["LainLain"]);
                        decTotSubTotal += Convert.ToDecimal(dt.Rows[i]["SubTotal"]);
                    }
                }
                #endregion

                #region OmRpSalesTrn001C / OmRpSalesTrn001G
                if (reportID == "OmRpSalesTrn001C" || reportID == "OmRpSalesTrn001G")
                {
                    namaPelanggan = dt.Rows[0]["NAMAPELANGGAN"].ToString();
                    soNO = dt.Rows[0]["SONO"].ToString();
                    alamat1 = dt.Rows[0]["ALAMAT1"].ToString();
                    tgglSO = Convert.ToDateTime(dt.Rows[0]["TANGGALSO"]).ToString("dd-MMM-yyyy");
                    alamat2 = dt.Rows[0]["ALAMAT2"].ToString();
                    SKPKNo = dt.Rows[0]["SKPKNO"].ToString();
                    refferenceNo = dt.Rows[0]["RefferenceNo"].ToString();
                    namaGovPelanggan = dt.Rows[0]["NAMAGOVPELANGGAN"].ToString();
                    NPWPNo = dt.Rows[0]["NPWPNo"].ToString();

                    gtf.SetGroupHeader("NAMA         :", 14, ' ', true);
                    gtf.SetGroupHeader(namaPelanggan, 55, ' ', true);
                    gtf.SetGroupHeader("NOMOR   :", 9, ' ', true);
                    gtf.SetGroupHeader(soNO, 15, ' ', false, true);
                    gtf.SetGroupHeader("ALAMAT       :", 14, ' ', true);
                    gtf.SetGroupHeader(alamat1, 55, ' ', true);
                    gtf.SetGroupHeader("TANGGAL :", 9, ' ', true);
                    gtf.SetGroupHeader(tgglSO, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(15);
                    gtf.SetGroupHeader(alamat2, 55, ' ', true);
                    gtf.SetGroupHeader("No.SKPK :", 9, ' ', true);
                    gtf.SetGroupHeader(SKPKNo, 15, ' ', false, true);
                    gtf.SetGroupHeaderSpace(71);
                    gtf.SetGroupHeader("No.Reff :", 9, ' ', true);
                    gtf.SetGroupHeader(refferenceNo, 15, ' ', false, true);
                    gtf.SetGroupHeader("Perincian lain-lain", 96, ' ', false, true);

                    gtf.SetGroupHeaderLine();
                    gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("UNIT", 5, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("MODEL", 13, ' ', true);
                    gtf.SetGroupHeader("TAHUN", 5, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("WARNA", 20, ' ', true);
                    gtf.SetGroupHeader("SEQ", 3, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("KETERANGAN", 21, ' ', true);
                    gtf.SetGroupHeader("JUMLAH", 15, ' ', false, true, true);
                    gtf.SetGroupHeaderLine();

                    string SONo = string.Empty;
                    int tempSpace = 0, loop = 0;
                    decimal decJumlah = 0, decGrandTotJumlah = 0;

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            if (reportID == "OmRpSalesTrn001C")
                                loop = 59 - gtf.line - 11;
                            else
                                loop = 30 - gtf.line - 11;
                            for (int j = 0; j < loop; j++)
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                            }

                            gtf.SetDataDetailSpace(33);
                            gtf.SetDataDetail("J U M L A H : ", 25, ' ', true, false, true);
                            gtf.SetDataDetail(decJumlah.ToString(), 37, ' ', false, true, true, true, "n2");
                            gtf.PrintData();
                            gtf.SetDataReportLine();
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(33);
                            gtf.SetDataDetail("GRAND TOTAL : ", 25, ' ', true, false, true);
                            gtf.SetDataDetail(counterData.ToString() + " UNIT", 21, ' ', true);
                            gtf.SetDataDetail(decGrandTotJumlah.ToString(), 15, ' ', false, true, true, true, "n2");
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(4);
                            gtf.SetDataDetail("MENGETAHUI :", 12, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            if (dt.Rows[i - 1]["SignName1"].ToString().Length < 20)
                                tempSpace = (20 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("-", 20, '-', false, true);
                            gtf.PrintData();
                            if (dt.Rows[i - 1]["TitleSign1"].ToString().Length < 20)
                                tempSpace = (20 - dt.Rows[i - 1]["TitleSign1"].ToString().Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), dt.Rows[i - 1]["TitleSign1"].ToString().Length, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();

                            break;
                        }

                        if (SONo == string.Empty)
                        {
                            SONo = dt.Rows[i]["SONo"].ToString();
                        }
                        else
                        {
                            if (SONo != dt.Rows[i]["SONo"].ToString())
                            {
                                if (reportID == "OmRpSalesTrn001C")
                                    loop = 59 - gtf.line - 11;
                                else
                                    loop = 30 - gtf.line - 11;
                                for (int j = 0; j < loop; j++)
                                {
                                    gtf.SetDataDetail("", 96, ' ', false, true);
                                    gtf.PrintData();
                                }

                                gtf.SetDataDetailSpace(33);
                                gtf.SetDataDetail("J U M L A H : ", 25, ' ', true, false, true);
                                gtf.SetDataDetail(decJumlah.ToString(), 37, ' ', false, true, true, true, "n2");
                                gtf.PrintData();
                                gtf.SetDataReportLine();
                                gtf.PrintData();
                                gtf.SetDataDetailSpace(33);
                                gtf.SetDataDetail("GRAND TOTAL : ", 25, ' ', true, false, true);
                                gtf.SetDataDetail(counterData.ToString() + " UNIT", 21, ' ', true);
                                gtf.SetDataDetail(decGrandTotJumlah.ToString(), 15, ' ', false, true, true, true, "n2");
                                gtf.PrintData();
                                gtf.SetDataDetailSpace(4);
                                gtf.SetDataDetail("MENGETAHUI :", 12, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                if (dt.Rows[i - 1]["SignName1"].ToString().Length < 20)
                                    tempSpace = (20 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("-", 20, '-', false, true);
                                gtf.PrintData();
                                if (dt.Rows[i - 1]["TitleSign1"].ToString().Length < 20)
                                    tempSpace = (20 - dt.Rows[i - 1]["TitleSign1"].ToString().Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), dt.Rows[i - 1]["TitleSign1"].ToString().Length, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();

                                SONo = dt.Rows[i]["SONo"].ToString();
                                counterData = 0;
                            }
                        }
                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["Quantity"].ToString(), 5, ' ', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 13, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 5, ' ', true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["COLOURNAME"].ToString(), 20, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SeqNo"].ToString(), 3, ' ', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Rows[i]["Ket"].ToString(), 21, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Jumlah"].ToString(), 15, ' ', true, false, true, true, "n2");

                        decJumlah += Convert.ToDecimal(dt.Rows[i]["Jumlah"]);

                        gtf.PrintData(true);
                    }
                }
                #endregion
            }
            #endregion

            return gtf.sbDataTxt.ToString();
        }
    }
}