using SimDms.Common;
using SimDms.Sales.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sales
{
    public class txtOmRpSalesTrn001:IRptProc
    {
        private SimDms.Sales.Models.DataContext ctx = new SimDms.Sales.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public Models.SysUser CurrentUser { get; set; }        

        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            var dt = ctx.Database.SqlQuery<OmRpSalesTrn001>(string.Format("exec {0} {1}", sproc, sparam));
            return CreateReportOmRpSalesTrn001(rptId, dt, sparam, printerloc, print, "", fullpage);
        }

        private string CreateReportOmRpSalesTrn001(string recordId, IEnumerable<OmRpSalesTrn001> dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {


            bool bCreateBy = false;
            int counterData = 0;

            string namaPelanggan = string.Empty, soNO = string.Empty, alamat1 = string.Empty, tgglSO = string.Empty, alamat2 = string.Empty,
                SKPKNo = string.Empty, alamat3 = string.Empty, refferenceNo = string.Empty, alamat4 = string.Empty,
                namaGovPelanggan = string.Empty, leasingName = string.Empty, leasingCo = string.Empty, alamatFaktur1 = string.Empty,
                alamatFaktur2 = string.Empty, alamatFaktur3 = string.Empty, alamatFaktur4 = string.Empty, NPWPNo = string.Empty,
                TOPName = string.Empty, sales = string.Empty, tipeSales = string.Empty, requestDate = string.Empty, shipName = string.Empty,
                soDate = string.Empty, ket = string.Empty;
            string tempTerbilang = string.Empty, tempTerbilang1 = string.Empty, tempTerbilang2 = string.Empty;
            string[] tempSplitTerbilang;
            var dtt = dt.FirstOrDefault();


            #region GenerateHeader
            SalesGenerateTextFileReport gtf = new SalesGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter("");
            gtf.SetPaperSize(1100, 850);

            if (recordId == "OmRpSalesTrn009A" || recordId == "OmRpSalesTrn003A" || recordId == "OmRpSalesTrn003D" ||
                recordId == "OmRpSalesTrn003C" || recordId == "OmRpSalesTrn002A" || recordId == "OmRpSalesTrn001D" ||
                recordId == "OmRpSalesTrn001E" || recordId == "OmRpSalesTrn001F" || recordId == "OmRpSalesTrn001G" ||
                recordId == "OmRpPurTrn001A" || recordId == "OmRpPurTrn002A" || recordId == "OmRpPurTrn003A" ||
                recordId == "OmRpSalesTrn006" || recordId == "OmRpSalesTrn006A" || recordId == "OmRpPurTrn008A" ||
                recordId == "OmRpPurTrn009" || recordId == "OmRpSalesTrn005A" || recordId == "OmRpSalesTrn010A" ||
                recordId == "OmRpStock001")
                fullPage = false;

            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, "", fullPage);

            if (recordId == "OmRpSalesTrn004" || recordId == "OmRpSalesTrn004A" || recordId == "OmRpSalesTrn003A" ||
                recordId == "OmRpSalesTrn007DNew" || recordId == "OmRpSalesTrn007" || recordId == "OmRpSalesTrn007C" ||
                recordId == "OmRpSalesTrn007A" || recordId == "OmRpSalesTrn010" || recordId == "OmRpSalesTrn010A" ||
                recordId == "OmRpStock002")
            {
                gtf.GenerateHeader2(false);
            }
            else if (recordId == "OmRpSalesTrn003D" || recordId == "OmRpSalesTrn001D" ||
                recordId == "OmRpSalesTrn001E" || recordId == "OmRpSalesTrn006" || recordId == "OmRpSalesTrn006A" ||
                recordId == "OmRpPurTrn009" || recordId == "OmRpSalesTrn005" || recordId == "OmRpSalesTrn005A" || recordId == "OmRpStock001")
            {
                gtf.GenerateHeader2(false, false);
            }
            else
            {
                gtf.GenerateHeader();
            }
            
            //gtf.GenerateHeader();

            #endregion

            #region detil

            #region OmRpSalesTrn001
            if (recordId == "OmRpSalesTrn001")
            {

                namaPelanggan = dtt.NAMAPELANGGAN;
                soNO = dtt.NOMORSO;
                alamat1 = dtt.ALAMAT1;
                tgglSO = dtt.TANGGALSO.Value.ToString("dd-MMM-yyyy");
                alamat2 = dtt.ALAMAT2;
                SKPKNo = dtt.SKPKNo;
                alamat3 = dtt.ALAMAT3;
                refferenceNo = dtt.RefferenceNo;
                alamat4 = dtt.ALAMAT4;
                namaGovPelanggan = dtt.NAMAGOVPELANGGAN;
                leasingName = dtt.LeasingName;
                leasingCo = dtt.LeasingCo;
                alamatFaktur1 = dtt.ALAMAT1;
                alamatFaktur2 = dtt.ALAMAT2;
                alamatFaktur3 = dtt.ALAMAT3;
                alamatFaktur4 = dtt.ALAMAT4;
                NPWPNo = dtt.NPWPNo;

                gtf.SetGroupHeader("NAMA         :", 14, ' ', true);
                gtf.SetGroupHeader(namaPelanggan, 53, ' ', true);
                gtf.SetGroupHeader("NOMOR   :", 9, ' ', true);
                gtf.SetGroupHeader(soNO, 17, ' ', false, true);
                gtf.SetGroupHeader("ALAMAT       :", 14, ' ', true);
                gtf.SetGroupHeader(alamat1, 53, ' ', true);
                gtf.SetGroupHeader("TANGGAL :", 9, ' ', true);
                gtf.SetGroupHeader(tgglSO, 15, ' ', false, true);
                gtf.SetGroupHeaderSpace(15);
                gtf.SetGroupHeader(alamat2, 53, ' ', true);
                gtf.SetGroupHeader("No.SKPK :", 9, ' ', true);
                gtf.SetGroupHeader(SKPKNo, 15, ' ', false, true);
                gtf.SetGroupHeaderSpace(15);
                gtf.SetGroupHeader(alamat3, 53, ' ', true);
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
                    gtf.SetGroupHeader("QQ : " + leasingName, 51 - (dtt.LeasingCo.Length + 2), ' ', true, false, true);
                    gtf.SetGroupHeader("[" + leasingCo + "]", dtt.LeasingCo.Length + 2, ' ', false, true);
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
                gtf.SetGroupHeader("KETERANGAN", 35, ' ', true);
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
                var ldt = dt.ToList();
                for (int i = 0; i <= ldt.Count; i++)
                {
                    if (i == ldt.Count)
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
                        gtf.SetDataDetail("Pembayaran : " + ldt[i - 1].PEMBAYARAN, 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("Perkiraan", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("Penyerahan : ", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("Lain-lain  : " + ldt[i - 1].LAINLAIN, 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataReportLine();
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail(ldt[i - 1].City + ", " +ldt[i - 1].TANGGALSO.Value.ToString("dd MMMM yyyy"), 96, ' ', false, true, true);
                        gtf.PrintData();
                        if (ldt[i - 1].BBN == null)
                        {
                            gtf.SetDataDetail("- Nilai BBN                : 0", 55, ' ', true);
                        }
                        else
                            gtf.SetDataDetail("- Nilai BBN                : " + Convert.ToDecimal(ldt[i - 1].BBN).ToString("n0"), 55, ' ', true);
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("Menyetujui", 10, ' ');
                        gtf.SetDataDetailSpace(10);
                        gtf.SetDataDetail("Pemesan", 10, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("- PP ini berlaku 30 hari s/d " + Convert.ToDateTime(ldt[i - 1].TANGGALSO).AddDays(30).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("- Harga belum termasuk STUJ, BBN, BPKB, dll", 96, ' ', false, true);
                        gtf.PrintData();
                        string tempSplitString1 = string.Empty;
                        string tempSplitString2 = string.Empty;
                        if (ldt[i - 1].NAMAPELANGGAN.Length > 20)
                        {
                            gtf.SetDataDetail("- PP ini tidak berlaku bila terjadi hal-hal di luar", 75, ' ', true);
                            string[] tempArrayString1 = ldt[i - 1].NAMAPELANGGAN.Split(' ');
                            string tempString = string.Empty;
                            foreach (string s in tempArrayString1)
                            {
                                if (Convert.ToString(tempString + s + ' ').Length < 20)
                                    tempString = tempString += s + ' ';
                                else
                                    break;
                            }
                            tempSplitString1 = tempString;
                            tempSplitString2 = ldt[i - 1].NAMAPELANGGAN.Substring(tempString.Length, ldt[i - 1].NAMAPELANGGAN.Length - tempString.Length);

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
                        if (ldt[i - 1].SignName1 == null)
                            ldt[i - 1].SignName1 = "";
                        if (ldt[i - 1].SignName1.Length < 20)
                            tempSpace = (20 - ldt[i - 1].SignName1.Length) / 2;
                        else
                            tempSpace = 1;

                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(ldt[i - 1].SignName1, ldt[i - 1].SignName1.Length, ' ');
                        gtf.SetDataDetailSpace(tempSpace);
                        if (tempSplitString1 != string.Empty)
                        {
                            tempSpace = (20 - tempSplitString2.Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(tempSplitString2, tempSplitString2.Length, ' ', false, true);
                        }
                        else
                        {
                            if (ldt[i - 1].NAMAPELANGGAN.Length < 20)
                                tempSpace = (20 - ldt[i - 1].NAMAPELANGGAN.Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace + 1);
                            gtf.SetDataDetail(ldt[i - 1].NAMAPELANGGAN, ldt[i - 1].NAMAPELANGGAN.Length, ' ', false, true);
                        }
                        gtf.PrintData();
                        gtf.SetDataDetail("  Peraturan Pemerintah dalam bidang Perpajakan", 55, ' ', true);
                        gtf.SetDataDetail("-", 19, '-', true);
                        gtf.SetDataDetail("-", 20, '-', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("  dan/atau Moneter dan sebagainya", 55, ' ', true);
                        if (ldt[i - 1].TitleSign1 == null)
                            ldt[i - 1].TitleSign1 = "";
                        if (ldt[i - 1].TitleSign1.Length < 20)
                            tempSpace = (20 - ldt[i - 1].TitleSign1.Length) / 2;
                        else
                            tempSpace = 1;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(ldt[i - 1].TitleSign1, 20, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        break;
                    }

                    if (NoSO == string.Empty)
                    {
                        NoSO = dtt.NOMORSO;
                    }
                    else
                    {

                        if (NoSO != dtt.NOMORSO)
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
                            gtf.SetDataDetail("Pembayaran : " + ldt[i - 1].PEMBAYARAN, 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("Perkiraan", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("Penyerahan : ", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("Lain-lain  : " + ldt[i - 1].LAINLAIN, 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataReportLine();
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail(ldt[i - 1].City + ", " + Convert.ToDateTime(ldt[i - 1].TANGGALSO).ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                            gtf.PrintData();
                            if (ldt[i - 1].BBN == null)
                            {
                                gtf.SetDataDetail("- Nilai BBN                : 0", 55, ' ', true);
                            }
                            else
                                gtf.SetDataDetail("- Nilai BBN                : " + Convert.ToDecimal(ldt[i - 1].BBN).ToString("n0"), 55, ' ', true);
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("Menyetujui", 10, ' ');
                            gtf.SetDataDetailSpace(10);
                            gtf.SetDataDetail("Pemesan", 10, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("- PP ini berlaku 30 hari s/d " + Convert.ToDateTime(ldt[i - 1].TANGGALSO).AddDays(30).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("- Harga belum termasuk STUJ, BBN, BPKB, dll", 96, ' ', false, true);
                            gtf.PrintData();
                            string tempSplitString1 = string.Empty;
                            string tempSplitString2 = string.Empty;
                            if (ldt[i - 1].NAMAPELANGGAN.Length > 20)
                            {
                                gtf.SetDataDetail("- PP ini tidak berlaku bila terjadi hal-hal di luar", 75, ' ', true);
                                string[] tempArrayString1 = ldt[i - 1].NAMAPELANGGAN.Split(' ');
                                string tempString = string.Empty;
                                foreach (string s in tempArrayString1)
                                {
                                    if (Convert.ToString(tempString + s + ' ').Length < 20)
                                        tempString = tempString += s + ' ';
                                    else
                                        break;
                                }
                                tempSplitString1 = tempString;
                                tempSplitString2 = ldt[i - 1].NAMAPELANGGAN.Substring(tempString.Length, ldt[i - 1].NAMAPELANGGAN.Length - tempString.Length);

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
                            if (ldt[i - 1].SignName1.Length < 20)
                                tempSpace = (20 - ldt[i - 1].SignName1.Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(ldt[i - 1].SignName1, ldt[i - 1].SignName1.Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace);
                            if (tempSplitString1 != string.Empty)
                            {
                                tempSpace = (20 - tempSplitString2.Length) / 2;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(tempSplitString2, tempSplitString2.Length, ' ', false, true);
                            }
                            else
                            {
                                if (ldt[i - 1].NAMAPELANGGAN.Length < 20)
                                    tempSpace = (20 - ldt[i - 1].NAMAPELANGGAN.Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace + 1);
                                gtf.SetDataDetail(ldt[i - 1].NAMAPELANGGAN, ldt[i - 1].NAMAPELANGGAN.Length, ' ', false, true);
                            }
                            gtf.PrintData();
                            gtf.SetDataDetail("  Peraturan Pemerintah dalam bidang Perpajakan", 55, ' ', true);
                            gtf.SetDataDetail("-", 19, '-', true);
                            gtf.SetDataDetail("-", 20, '-', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("  dan/atau Moneter dan sebagainya", 55, ' ', true);
                            if (ldt[i - 1].TitleSign1.Length < 20)
                                tempSpace = (20 - ldt[i - 1].TitleSign1.Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(ldt[i - 1].TitleSign1, 20, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();

                            NoSO = dtt.NOMORSO;
                            decJumlah = 0;
                            counterData = 0;

                            gtf.ReplaceGroupHdr(namaPelanggan, dtt.NAMAPELANGGAN, 55);
                            gtf.ReplaceGroupHdr(soNO, dtt.NOMORSO, 15);
                            gtf.ReplaceGroupHdr(alamat1, dtt.ALAMAT1, 55);
                            gtf.ReplaceGroupHdr(tgglSO, Convert.ToDateTime(dtt.TANGGALSO).ToString("dd-MMM-yyyy"), 15);
                            gtf.ReplaceGroupHdr(alamat2, dtt.ALAMAT2, 55);
                            gtf.ReplaceGroupHdr(SKPKNo, dtt.SKPKNo, 15);
                            gtf.ReplaceGroupHdr(alamat3, dtt.ALAMAT3, 55);
                            gtf.ReplaceGroupHdr(refferenceNo, dtt.RefferenceNo, 15);
                            gtf.ReplaceGroupHdr(alamat4, dtt.ALAMAT4, 81);
                            gtf.ReplaceGroupHdr(namaGovPelanggan, dtt.NAMAGOVPELANGGAN, 29);
                            gtf.ReplaceGroupHdr(leasingName, dtt.LeasingName, 51 - (dtt.LeasingCo.Length + 2), 717);
                            gtf.ReplaceGroupHdr(leasingCo, dtt.LeasingCo, dtt.LeasingCo.Length + 2, 717 + 51 - (ldt[i - 1].LeasingCo.Length + 2));
                            gtf.ReplaceGroupHdr(alamatFaktur1, dtt.ALAMAT1, 81);
                            gtf.ReplaceGroupHdr(alamatFaktur2, dtt.ALAMAT2, 81);
                            gtf.ReplaceGroupHdr(alamatFaktur3, dtt.ALAMAT3, 81);
                            gtf.ReplaceGroupHdr(alamatFaktur4, dtt.ALAMAT4, 81);
                            gtf.ReplaceGroupHdr(NPWPNo, dtt.NPWPNo, 81);

                            namaPelanggan = dtt.NAMAPELANGGAN;
                            soNO = dtt.NOMORSO;
                            alamat1 = dtt.ALAMAT1;
                            tgglSO = Convert.ToDateTime(dtt.TANGGALSO).ToString("dd-MMM-yyyy");
                            alamat2 = dtt.ALAMAT2;
                            SKPKNo = dtt.SKPKNo;
                            alamat3 = dtt.ALAMAT3;
                            refferenceNo = dtt.RefferenceNo;
                            alamat4 = dtt.ALAMAT4;
                            namaGovPelanggan = dtt.NAMAGOVPELANGGAN;
                            leasingName = dtt.LeasingName;
                            leasingCo = dtt.LeasingCo;
                            alamatFaktur1 = dtt.ALAMAT1;
                            alamatFaktur2 = dtt.ALAMAT2;
                            alamatFaktur3 = dtt.ALAMAT3;
                            alamatFaktur4 = dtt.ALAMAT4;
                            NPWPNo = dtt.NPWPNo;
                        }
                    }
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(ldt[i].MODEL, 15, ' ', true);
                    gtf.SetDataDetail(ldt[i].COLOURNAME, 35, ' ', true);
                    gtf.SetDataDetail(ldt[i].UNIT.ToString(), 5, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(ldt[i].HARGA.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(ldt[i].JUMLAH.ToString(), 15, ' ', false, true, true, true, "n0");

                    decJumlah += Convert.ToDecimal(ldt[i].JUMLAH);

                    gtf.PrintData(true);


                }

            } 
            #endregion
            #region OmRpSalesTrn001D
            else if (recordId == "OmRpSalesTrn001D")
            {
                namaPelanggan =dtt.NAMAPELANGGAN;
                soNO =dtt.NOMORSO;
                tgglSO = dtt.TANGGALSO.Value.ToString("dd-MMM-yyyy");
                SKPKNo =dtt.SKPKNo;
                alamat3 =dtt.ALAMAT3;
                refferenceNo =dtt.RefferenceNo;
                alamat4 =dtt.ALAMAT4;
                namaGovPelanggan =dtt.NAMAGOVPELANGGAN;
                leasingName =dtt.LeasingName;
                leasingCo =dtt.LeasingCo;
                alamatFaktur1 =dtt.ALAMAT1;
                alamatFaktur2 =dtt.ALAMAT2;
                alamatFaktur3 =dtt.ALAMAT3;
                alamatFaktur4 =dtt.ALAMAT4;
                NPWPNo =dtt.NPWPNo;

                gtf.SetGroupHeader("PESANAN PENJUALAN", 96, ' ', false, true);
                gtf.SetGroupHeader("NAMA         :", 14, ' ', true);
                gtf.SetGroupHeader(namaPelanggan, 55, ' ', true);
                gtf.SetGroupHeader("NOMOR   :", 9, ' ', true);
                gtf.SetGroupHeader(soNO, 15, ' ', false, true);
                if (dtt.ALAMAT2 != string.Empty)
                    alamat1 = "ALAMAT       : " +dtt.ALAMAT1 + ", " +dtt.ALAMAT2;
                else
                    alamat1 = "ALAMAT       : " +dtt.ALAMAT1;

                gtf.SetGroupHeader(alamat1, 70, ' ', true);
                gtf.SetGroupHeader("TANGGAL :", 9, ' ', true);
                gtf.SetGroupHeader(tgglSO, 15, ' ', false, true);
                gtf.SetGroupHeaderSpace(15);
                if (dtt.ALAMAT3 != string.Empty)
                {
                    if (dtt.ALAMAT4 != string.Empty)
                        alamat2 =dtt.ALAMAT3 + ", " +dtt.ALAMAT4;
                    else
                        alamat2 =dtt.ALAMAT3;

                    gtf.SetGroupHeader(alamat2, 55, ' ', true);
                }
                else
                {
                    if (dtt.ALAMAT4 != string.Empty)
                    {
                        alamat2 =dtt.ALAMAT4;
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
                    gtf.SetGroupHeader("QQ : " + leasingName, 51 - (dtt.LeasingCo.Length + 2), ' ', true, false, true);
                    gtf.SetGroupHeader("[" + leasingCo + "]",dtt.LeasingCo.Length + 2, ' ', false, true);
                }
                else
                    gtf.SetGroupHeader("", 51, ' ', false, true);

                if (dtt.ALAMAT2 != string.Empty)
                {
                    if (dtt.ALAMAT3 != string.Empty)
                    {
                        if (dtt.ALAMAT4 != string.Empty)
                            alamatFaktur1 = "ALAMAT       : " +dtt.ALAMAT1 + ", " +
                               dtt.ALAMAT2 + ", " +dtt.ALAMAT3 + ", " +
                               dtt.ALAMAT4;
                        else
                            alamatFaktur1 = "ALAMAT       : " +dtt.ALAMAT1 + ", " +
                               dtt.ALAMAT2 + ", " +dtt.ALAMAT3;
                    }
                    else
                    {
                        if (dtt.ALAMAT4 != string.Empty)
                            alamatFaktur1 = "ALAMAT       : " +dtt.ALAMAT1 + ", " +
                               dtt.ALAMAT2 + ", " +dtt.ALAMAT4;
                        else
                            alamatFaktur1 = "ALAMAT       : " +dtt.ALAMAT1 + ", " +
                               dtt.ALAMAT2;
                    }
                }
                else
                {
                    if (dtt.ALAMAT3 != string.Empty)
                    {
                        if (dtt.ALAMAT4 != string.Empty)
                            alamatFaktur1 = "ALAMAT       : " +dtt.ALAMAT1 + ", " +
                               dtt.ALAMAT3 + ", " +dtt.ALAMAT4;
                        else
                            alamatFaktur1 = "ALAMAT       : " +dtt.ALAMAT1 + ", " +
                               dtt.ALAMAT3;
                    }
                    else
                    {
                        if (dtt.ALAMAT4 != string.Empty)
                            alamatFaktur1 = "ALAMAT       : " +dtt.ALAMAT1 + ", " +
                               dtt.ALAMAT4;
                        else
                            alamatFaktur1 = "ALAMAT       : " +dtt.ALAMAT1;
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
                var ldt = dt.ToList();
                for (int i = 0; i <= ldt.Count; i++)
                {
                    if (i == ldt.Count)
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
                        gtf.SetDataDetail("Pembayaran : " + ldt[i - 1].PEMBAYARAN, 23, ' ', true);
                        gtf.SetDataDetail("Perkiraan", 23, ' ', true);
                        gtf.SetDataDetail("Penyerahan : ", 23, ' ', true);
                        gtf.SetDataDetail("Lain-lain  : " + ldt[i - 1].LAINLAIN, 24, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataReportLine();
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail(ldt[i - 1].City + ", " + ldt[i - 1].TANGGALSO.Value.ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                        gtf.PrintData(false, false);
                        if (ldt[i - 1].BBN == null )
                        {
                            gtf.SetDataDetail("- Nilai BBN                : 0", 55, ' ', true);
                        }
                        else
                            gtf.SetDataDetail("- Nilai BBN                : " + ldt[i - 1].BBN.Value.ToString("n0"), 55, ' ', true);
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("Menyetujui", 10, ' ');
                        gtf.SetDataDetailSpace(10);
                        gtf.SetDataDetail("Pemesan", 10, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("- PP ini berlaku 30 hari s/d " + ldt[i - 1].TANGGALSO.Value.AddDays(30).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("- Harga belum termasuk STUJ, BBN, BPKB, dll", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        string tempSplitString1 = string.Empty;
                        string tempSplitString2 = string.Empty;
                        if (ldt[i - 1].NAMAPELANGGAN.Length > 20)
                        {
                            gtf.SetDataDetail("- PP ini tidak berlaku bila terjadi hal-hal di luar", 75, ' ', true);
                            string[] tempArrayString1 = ldt[i - 1].NAMAPELANGGAN.Split(' ');
                            string tempString = string.Empty;
                            foreach (string s in tempArrayString1)
                            {
                                if (Convert.ToString(tempString + s + ' ').Length < 20)
                                    tempString = tempString += s + ' ';
                                else
                                    break;
                            }
                            tempSplitString1 = tempString;
                            tempSplitString2 = ldt[i - 1].NAMAPELANGGAN.Substring(tempString.Length, ldt[i - 1].NAMAPELANGGAN.Length - tempString.Length);

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
                        if (ldt[i - 1].SignName1 == null)
                            ldt[i - 1].SignName1 = "";
                        if (ldt[i - 1].SignName1.Length < 20)
                            tempSpace = (20 - ldt[i - 1].SignName1.Length) / 2;
                        else
                            tempSpace = 1;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(ldt[i - 1].SignName1, ldt[i - 1].SignName1.Length, ' ');
                        gtf.SetDataDetailSpace(tempSpace);
                        if (tempSplitString1 != string.Empty)
                        {
                            tempSpace = (20 - tempSplitString2.Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(tempSplitString2, tempSplitString2.Length, ' ', false, true);
                        }
                        else
                        {
                            if (ldt[i - 1].NAMAPELANGGAN.Length < 20)
                                tempSpace = (20 - ldt[i - 1].NAMAPELANGGAN.Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace + 1);
                            gtf.SetDataDetail(ldt[i - 1].NAMAPELANGGAN, ldt[i - 1].NAMAPELANGGAN.Length, ' ', false, true);
                        }
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("  Peraturan Pemerintah dalam bidang Perpajakan", 55, ' ', true);
                        gtf.SetDataDetail("-", 19, '-', true);
                        gtf.SetDataDetail("-", 20, '-', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("  dan/atau Moneter dan sebagainya", 55, ' ', true);
                        if (ldt[i - 1].TitleSign1 == null)
                            ldt[i - 1].TitleSign1 = "";
                        if (ldt[i - 1].TitleSign1.Length < 20)
                            tempSpace = (20 - ldt[i - 1].TitleSign1.Length) / 2;
                        else
                            tempSpace = 1;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(ldt[i - 1].TitleSign1, 20, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        break;
                    }

                    if (NoSO == string.Empty)
                    {
                        NoSO =dtt.NOMORSO;
                    }
                    else
                    {
                        if (NoSO != dtt.NOMORSO)
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
                            gtf.SetDataDetail("Pembayaran : " + ldt[i - 1].PEMBAYARAN, 23, ' ', true);
                            gtf.SetDataDetail("Perkiraan", 23, ' ', true);
                            gtf.SetDataDetail("Penyerahan : ", 23, ' ', true);
                            gtf.SetDataDetail("Lain-lain  : " + ldt[i - 1].LAINLAIN, 24, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataReportLine();
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail(ldt[i - 1].City + ", " + ldt[i - 1].TANGGALSO.Value.ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                            gtf.PrintData(false, false);
                            if (ldt[i - 1].BBN == null)
                            {
                                gtf.SetDataDetail("- Nilai BBN                : 0", 55, ' ', true);
                            }
                            else
                                gtf.SetDataDetail("- Nilai BBN                : " + ldt[i - 1].BBN.Value.ToString("n0"), 55, ' ', true);
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("Menyetujui", 10, ' ');
                            gtf.SetDataDetailSpace(10);
                            gtf.SetDataDetail("Pemesan", 10, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("- PP ini berlaku 30 hari s/d " + ldt[i - 1].TANGGALSO.Value.AddDays(30).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("- Harga belum termasuk STUJ, BBN, BPKB, dll", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            string tempSplitString1 = string.Empty;
                            string tempSplitString2 = string.Empty;
                            if (ldt[i - 1].NAMAPELANGGAN.Length > 20)
                            {
                                gtf.SetDataDetail("- PP ini tidak berlaku bila terjadi hal-hal di luar", 75, ' ', true);
                                string[] tempArrayString1 = ldt[i - 1].NAMAPELANGGAN.Split(' ');
                                string tempString = string.Empty;
                                foreach (string s in tempArrayString1)
                                {
                                    if (Convert.ToString(tempString + s + ' ').Length < 20)
                                        tempString = tempString += s + ' ';
                                    else
                                        break;
                                }
                                tempSplitString1 = tempString;
                                tempSplitString2 = ldt[i - 1].NAMAPELANGGAN.Substring(tempString.Length, ldt[i - 1].NAMAPELANGGAN.Length - tempString.Length);

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
                            if (ldt[i - 1].SignName1.Length < 20)
                                tempSpace = (20 - ldt[i - 1].SignName1.Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(ldt[i - 1].SignName1, ldt[i - 1].SignName1.Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace);
                            if (tempSplitString1 != string.Empty)
                            {
                                tempSpace = (20 - tempSplitString2.Length) / 2;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(tempSplitString2, tempSplitString2.Length, ' ', false, true);
                            }
                            else
                            {
                                if (ldt[i - 1].NAMAPELANGGAN.Length < 20)
                                    tempSpace = (20 - ldt[i - 1].NAMAPELANGGAN.Length) / 2;
                                else
                                    tempSpace = 1;
                                gtf.SetDataDetailSpace(tempSpace + 1);
                                gtf.SetDataDetail(ldt[i - 1].NAMAPELANGGAN, ldt[i - 1].NAMAPELANGGAN.Length, ' ', false, true);
                            }
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("  Peraturan Pemerintah dalam bidang Perpajakan", 55, ' ', true);
                            gtf.SetDataDetail("-", 19, '-', true);
                            gtf.SetDataDetail("-", 20, '-', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("  dan/atau Moneter dan sebagainya", 55, ' ', true);
                            if (ldt[i - 1].TitleSign1.Length < 20)
                                tempSpace = (20 - ldt[i - 1].TitleSign1.Length) / 2;
                            else
                                tempSpace = 1;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(ldt[i - 1].TitleSign1, 20, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);

                            NoSO = dtt.NOMORSO;
                            decJumlah = 0;
                            counterData = 0;

                            gtf.ReplaceGroupHdr(namaPelanggan, dtt.NAMAPELANGGAN, 55);
                            gtf.ReplaceGroupHdr(soNO, dtt.NOMORSO, 15);
                            if (dtt.ALAMAT2 != string.Empty)
                            {
                                gtf.ReplaceGroupHdr(alamat1, "ALAMAT       : " + dtt.ALAMAT1 + ", " + dtt.ALAMAT2, 70);
                                alamat1 = "ALAMAT       : " + dtt.ALAMAT1 + ", " + dtt.ALAMAT2;
                            }
                            else
                            {
                                gtf.ReplaceGroupHdr(alamat1, "ALAMAT       : " + dtt.ALAMAT1, 70);
                                alamat1 = "ALAMAT       : " + dtt.ALAMAT1;
                            }
                            gtf.ReplaceGroupHdr(tgglSO, dtt.TANGGALSO.Value.ToString("dd-MMM-yyyy"), 15);

                            if (dtt.ALAMAT3 != string.Empty)
                            {
                                if (dtt.ALAMAT4 != string.Empty)
                                {
                                    if (alamat2 == string.Empty || alamat2 == " ")
                                        gtf.ReplaceGroupHdr(alamat2, dtt.ALAMAT3 + ", " + dtt.ALAMAT4, 55, 288, 55);
                                    else
                                        gtf.ReplaceGroupHdr(alamat2, dtt.ALAMAT3 + ", " + dtt.ALAMAT4, 55);
                                    alamat2 = dtt.ALAMAT3 + ", " +dtt.ALAMAT4;
                                }
                                else
                                {
                                    gtf.ReplaceGroupHdr(alamat2, dtt.ALAMAT3, 55);
                                    alamat2 = dtt.ALAMAT3;
                                }
                            }
                            else
                            {
                                if (dtt.ALAMAT4 != string.Empty)
                                {
                                    gtf.ReplaceGroupHdr(alamat2, dtt.ALAMAT4, 55);
                                    alamat2 = dtt.ALAMAT4;
                                }
                            }

                            gtf.ReplaceGroupHdr(SKPKNo, dtt.SKPKNo, 15);
                            gtf.ReplaceGroupHdr(alamat3, dtt.ALAMAT3, 55);
                            gtf.ReplaceGroupHdr(refferenceNo, dtt.RefferenceNo, 15);
                            gtf.ReplaceGroupHdr(alamat4, dtt.ALAMAT4, 81);
                            gtf.ReplaceGroupHdr(leasingName, dtt.LeasingName, 51 - (dtt.LeasingCo.Length + 2));
                            gtf.ReplaceGroupHdr(leasingCo, dtt.LeasingCo, dtt.LeasingCo.Length + 2);

                            if (dtt.ALAMAT2 != string.Empty)
                            {
                                if (dtt.ALAMAT3 != string.Empty)
                                {
                                    if (dtt.ALAMAT4 != string.Empty)
                                    {
                                        gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT2 + ", " + dtt.ALAMAT3 + ", " +
                                            dtt.ALAMAT4, 96);
                                        alamatFaktur1 = "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT2 + ", " + dtt.ALAMAT3 + ", " +
                                            dtt.ALAMAT4;
                                    }
                                    else
                                    {
                                        gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT2 + ", " + dtt.ALAMAT3, 96);
                                        alamatFaktur1 = "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT2 + ", " + dtt.ALAMAT3;
                                    }
                                }
                                else
                                {
                                    if (dtt.ALAMAT4 != string.Empty)
                                    {
                                        gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT2 + ", " + dtt.ALAMAT4, 96);
                                        alamatFaktur1 = "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT2 + ", " + dtt.ALAMAT4;
                                    }
                                    else
                                    {
                                        gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT2, 96);
                                        alamatFaktur1 = "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT2;
                                    }
                                }
                            }
                            else
                            {
                                if (dtt.ALAMAT3 != string.Empty)
                                {
                                    if (dtt.ALAMAT4 != string.Empty)
                                    {
                                        gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT3 + ", " + dtt.ALAMAT4, 96);
                                        alamatFaktur1 = "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT3 + ", " + dtt.ALAMAT4;
                                    }
                                    else
                                    {
                                        gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT3, 96);
                                        alamatFaktur1 = "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT3;
                                    }
                                }
                                else
                                {
                                    if (dtt.ALAMAT4 != string.Empty)
                                    {
                                        gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT4, 96);
                                        alamatFaktur1 = "ALAMAT       : " + dtt.ALAMAT1 + ", " +
                                            dtt.ALAMAT4;
                                    }
                                    else
                                    {
                                        gtf.ReplaceGroupHdr(alamatFaktur1, "ALAMAT       : " + dtt.ALAMAT1, 96);
                                        alamatFaktur1 = "ALAMAT       : " + dtt.ALAMAT1;
                                    }
                                }
                            }


                            namaPelanggan = dtt.NAMAPELANGGAN;
                            soNO = dtt.NOMORSO;
                            tgglSO = dtt.TANGGALSO.Value.ToString("dd-MMM-yyyy");
                            SKPKNo = dtt.SKPKNo;
                            refferenceNo = dtt.RefferenceNo;
                            namaGovPelanggan = dtt.NAMAGOVPELANGGAN;
                            leasingName = dtt.LeasingName;
                            leasingCo = dtt.LeasingCo;
                            NPWPNo = dtt.NPWPNo;
                        }
                    }
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(ldt[i].MODEL, 15, ' ', true);
                    gtf.SetDataDetail(ldt[i].COLOURNAME, 35, ' ', true);
                    gtf.SetDataDetail(ldt[i].UNIT.ToString(), 5, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(ldt[i].HARGA.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(ldt[i].JUMLAH.ToString(), 15, ' ', false, true, true, true, "n0");

                    decJumlah += Convert.ToDecimal(ldt[i].JUMLAH);

                    gtf.PrintData(false, false);
                }

            } 
            #endregion
            #endregion


            return gtf.sbDataTxt.ToString();
        }
    }
}