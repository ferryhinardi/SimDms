using SimDms.Common;
using SimDms.Sales.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sales
{
    public class txtOmRpSalesTrn004:IRptProc
    {
        private SimDms.Sales.Models.DataContext ctx = new SimDms.Sales.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public Models.SysUser CurrentUser { get; set; }
        

        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            var dt = ctx.Database.SqlQuery<OmRpSalesTrn004>(string.Format("exec {0} {1}", sproc, sparam));
            return CreateReportOmRpSalesTrn004(rptId, dt, sparam, printerloc, print, "", fullpage);
        }

        private string CreateReportOmRpSalesTrn004(string recordId, IEnumerable<OmRpSalesTrn004> dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            string reportID = recordId;

            bool bCreateBy = false;
            int counterData = 0;

            string namaPelanggan = string.Empty, soNO = string.Empty, alamat1 = string.Empty, tgglSO = string.Empty, alamat2 = string.Empty,
                SKPKNo = string.Empty, alamat3 = string.Empty,  alamat4 = string.Empty,
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

            #region Invoice (OmRpSalesTrn004/OmRpSalesTrn004A)
            if (reportID == "OmRpSalesTrn004" || reportID == "OmRpSalesTrn004A")
            {
                bCreateBy = false;
                if (reportID == "OmRpSalesTrn004")
                {
                    gtf.SetGroupHeader(dtt.CompanyName, 96, ' ', false, true);
                    gtf.SetGroupHeader(dtt.Alamat1, 96, ' ', false, true);
                    gtf.SetGroupHeader(dtt.Alamat2, 96, ' ', false, true);
                    gtf.SetGroupHeader("TELP. " + dtt.CompanyPhone, 96, ' ', false, true);
                    gtf.SetGroupHeader(dtt.City, 96, ' ', false, true);
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        gtf.SetGroupHeader("", 96, ' ', false, true);
                    }
                }

                gtf.SetGroupHeader("N.P.W.P      : " + dtt.NPWPNo, 96, ' ', false, true);
                gtf.SetGroupHeader("NO. PKP      : " + dtt.SKPNo, 57, ' ', true);
                gtf.SetGroupHeader("TGL PKP : " + dtt.SKPDate.Value.ToString("dd-MMM-yyyy"), 38, ' ', false, true);
                gtf.SetGroupHeader("FAKTUR PENJUALAN", 96, ' ', false, true, false, true);
                gtf.SetGroupHeader("Kepada Yth. :", 96, ' ', false, true);

                string customerName = string.Empty, invoiceNo2 = string.Empty, alamatCust1 = string.Empty, fps = string.Empty, alamatCust2 = string.Empty,
                soNo = string.Empty, alamatCust3 = string.Empty, skpkNo = string.Empty, custNPWPNo = string.Empty, refferenceNo = string.Empty;

                customerName = dtt.CustomerName;
                invoiceNo2 = dtt.InvoiceNo2;
                alamatCust1 = dtt.AlamatCust1;
                fps = dtt.FPS;
                alamatCust2 = dtt.AlamatCust2;
                soNo = dtt.SONo;
                alamatCust3 = dtt.AlamatCust3;
                skpkNo = dtt.SKPKNo;
                custNPWPNo = dtt.CustNPWPNo;
                refferenceNo = dtt.RefferenceNo;

                gtf.SetGroupHeader(customerName, 57, ' ', true);
                gtf.SetGroupHeader("No. Inv. : " + invoiceNo2, 38, ' ', false, true);
                gtf.SetGroupHeader(alamatCust1, 57, ' ', true);
                gtf.SetGroupHeader("No. FPS  : " + fps, 38, ' ', false, true);
                gtf.SetGroupHeader(alamatCust2, 57, ' ', true);
                gtf.SetGroupHeader("No. SO   : " + soNo, 38, ' ', false, true);
                gtf.SetGroupHeader(alamatCust3, 57, ' ', true);
                gtf.SetGroupHeader("No. SKPK : " + skpkNo, 38, ' ', false, true);
                gtf.SetGroupHeader("NPWP : " + custNPWPNo, 57, ' ', true);
                gtf.SetGroupHeader("No. Reff : " + refferenceNo, 38, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("No. DO", 16, ' ', true);
                gtf.SetGroupHeader("No. BPK", 15, ' ', true);
                gtf.SetGroupHeader("Tgl. BPK", 25, ' ', true);
                gtf.SetGroupHeader("Harga/Unit", 15, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("J u m l a h", 15, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                decimal decSubJumlah = 0, decSubPotongan = 0, decSubTotal = 0, deSubTotPPnBM = 0, decTotPPn = 0, decTotRupiah = 0;
                string invoiceNo = string.Empty;
                var ldt = dt.ToList();
                for (int i = 0; i <= ldt.Count(); i++)
                {
                    if (i == ldt.Count())
                    {
                        
                        gtf.SetDataDetailSpace(81);
                        gtf.SetDataDetail("-", 15, '-', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(81);
                        gtf.SetDataDetail(decSubJumlah.ToString(), 15, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("Potongan    : ", 71, ' ', true, false, true);
                        gtf.SetDataDetail(decSubPotongan.ToString(), 24, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(81);
                        gtf.SetDataDetail("-", 15, '-', false, true);
                        gtf.PrintData(false, false);
                        decSubTotal = decSubJumlah - decSubPotongan;
                        gtf.SetDataDetailSpace(81);
                        gtf.SetDataDetail(decSubTotal.ToString(), 15, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false, false);

                        int loop = 59 - (gtf.line + 15);

                        for (int k = 0; k < loop; k++)
                        {
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                        }

                        decTotRupiah = decSubTotal + decTotPPn + deSubTotPPnBM;
                         tempTerbilang = Convert.ToString("Terbilang : " + gtf.Terbilang(Convert.ToInt64(decTotRupiah)).ToUpper() + " RUPIAH");
                         tempTerbilang1 = string.Empty;
                         tempTerbilang2 = string.Empty;
                        if (tempTerbilang.Length > 67)
                        {
                           tempSplitTerbilang = tempTerbilang.Split(' ');

                            int tempLength = 0;
                            foreach (string s in tempSplitTerbilang)
                            {
                                tempLength += s.Length;
                                tempTerbilang1 += s;
                                if (tempLength > 67) break;
                            }
                            tempTerbilang1 = tempTerbilang.Substring(0, tempLength);
                            tempTerbilang2 = tempTerbilang.Substring(tempLength);
                        }
                        else
                        {
                            tempTerbilang1 = tempTerbilang;
                        }

                        gtf.SetDataDetail(tempTerbilang1, 67, ' ', true);
                        gtf.SetDataDetail("T o t a l", 11, ' ', true);
                        gtf.SetDataDetail(decSubTotal.ToString(), 16, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(11);
                        gtf.SetDataDetail(tempTerbilang2, 56, ' ', true);
                        gtf.SetDataDetail("PPn        " + Convert.ToDecimal(ldt[i - 1].PPN).ToString("n2") + " %", 11, ' ', true);
                        gtf.SetDataDetail(decTotPPn.ToString(), 16, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(68);
                        gtf.SetDataDetail("PPn BM", 11, ' ', true);
                        gtf.SetDataDetail("", 16, ' ', false, true, true, true, "n0");//gtf.SetDataDetail(deSubTotPPnBM.ToString(), 16, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(81);
                        gtf.SetDataDetail("-", 15, '-', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(81);
                        gtf.SetDataDetail(decTotRupiah.ToString(), 15, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false, false);
                        gtf.SetDataReportLine();
                        gtf.PrintData(false, false);

                        gtf.SetDataDetail(dtt.City + " , " +dtt.InvoiceDate.Value.ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("Total PPn BM Paid : Rp.", 23, ' ', true);
                        gtf.SetDataDetail(deSubTotPPnBM.ToString(), 15, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("Jatuh Tempo       : " + dtt.DueDate.Value.ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(48);
                        int tempSpace = 0;
                        if (dtt.SignName1 != string.Empty)
                        {
                            tempSpace = (23 - dtt.SignName1.Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dtt.SignName1, dtt.SignName1.Length, ' ');
                        }
                        else
                        {
                            gtf.SetDataDetail(dtt.SignName1, 23, ' ', true);
                            tempSpace = 0;
                        }
                        if (dtt.SignName2 != string.Empty)
                        {
                            gtf.SetDataDetailSpace(tempSpace + 1);
                            tempSpace = (24 - dtt.SignName2.Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dtt.SignName2, dtt.SignName2.Length, ' ', false, true);
                        }
                        else
                            gtf.SetDataDetail(dtt.SignName2, 24, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(48);
                        gtf.SetDataDetail("-", 23, '-', true);
                        gtf.SetDataDetail("-", 24, '-', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(48);

                        tempSpace = 0;
                        if (dtt.TitleSign1 != string.Empty)
                        {
                            tempSpace = (23 - dtt.TitleSign1.Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dtt.TitleSign1, dtt.TitleSign1.Length, ' ');
                        }
                        else
                            gtf.SetDataDetail(dtt.TitleSign1, 23, ' ', true);
                        if (dtt.TitleSign2 != string.Empty)
                        {
                            gtf.SetDataDetailSpace(tempSpace + 1);
                            tempSpace = (24 - dtt.TitleSign2.Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dtt.TitleSign2, dtt.TitleSign2.Length, ' ', false, true);
                        }
                        else
                            gtf.SetDataDetail(dtt.TitleSign2, 24, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        break;
                    }

                    if (invoiceNo == string.Empty)
                    {
                        invoiceNo = ldt[i].InvoiceNo;
                    }
                    else
                    {
                        if (invoiceNo != ldt[i].InvoiceNo)
                        {
                            gtf.SetDataDetailSpace(81);
                            gtf.SetDataDetail("-", 15, '-', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(81);
                            gtf.SetDataDetail(decSubJumlah.ToString(), 15, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("Potongan    : ", 71, ' ', true, false, true);
                            gtf.SetDataDetail(decSubPotongan.ToString(), 24, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(81);
                            gtf.SetDataDetail("-", 15, '-', false, true);
                            gtf.PrintData(false, false);
                            decSubTotal = decSubJumlah - decSubPotongan;
                            gtf.SetDataDetailSpace(81);
                            gtf.SetDataDetail(decSubTotal.ToString(), 15, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);

                            int loop = 59 - (gtf.line + 15);

                            for (int k = 0; k < loop; k++)
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                            }

                            decTotRupiah = decSubTotal + decTotPPn + deSubTotPPnBM;
                             tempTerbilang = Convert.ToString("Terbilang : " + gtf.Terbilang(Convert.ToInt64(decTotRupiah)).ToUpper() + " RUPIAH");
                              tempTerbilang1 = string.Empty;
                              tempTerbilang2 = string.Empty;
                            if (tempTerbilang.Length > 67)
                            {
                                 tempSplitTerbilang = tempTerbilang.Split(' ');

                                int tempLength = 0;
                                foreach (string s in tempSplitTerbilang)
                                {
                                    tempLength += s.Length;
                                    tempTerbilang1 += s;
                                    if (tempLength > 67) break;
                                }
                                tempTerbilang1 = tempTerbilang.Substring(0, tempLength);
                                tempTerbilang2 = tempTerbilang.Substring(tempLength);
                            }
                            else
                            {
                                tempTerbilang1 = tempTerbilang;
                            }

                            gtf.SetDataDetail(tempTerbilang1, 67, ' ', true);
                            gtf.SetDataDetail("T o t a l", 11, ' ', true);
                            gtf.SetDataDetail(decSubTotal.ToString(), 16, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(11);
                            gtf.SetDataDetail(tempTerbilang2, 56, ' ', true);
                            gtf.SetDataDetailSpace(68);
                            gtf.SetDataDetail("PPn        " + ldt[i - 1].PPN.Value.ToString("n2") + " %", 11, ' ', true);
                            gtf.SetDataDetail(decTotPPn.ToString(), 16, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(68);
                            gtf.SetDataDetail("PPn BM", 11, ' ', true);
                            gtf.SetDataDetail(deSubTotPPnBM.ToString(), 16, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(81);
                            gtf.SetDataDetail("-", 15, '-', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(81);
                            gtf.SetDataDetail(decTotRupiah.ToString(), 15, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);
                            gtf.SetDataReportLine();
                            gtf.PrintData(false, false);

                            gtf.SetDataDetail(dtt.City + " , " + dtt.InvoiceDate.Value.ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("Total PPn BM Paid : Rp.", 23, ' ', true);
                            gtf.SetDataDetail(deSubTotPPnBM.ToString(), 15, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("Jatuh Tempo       : " + dtt.DueDate.Value.ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(48);
                            int tempSpace = 0;
                            if (dtt.SignName1 != string.Empty)
                            {
                                tempSpace = (23 - dtt.SignName1.Length) / 2;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dtt.SignName1, dtt.SignName1.Length, ' ');
                            }
                            else
                            {
                                gtf.SetDataDetail(dtt.SignName1, 23, ' ', true);
                                tempSpace = 0;
                            }

                            if (dtt.SignName2 != string.Empty)
                            {
                                gtf.SetDataDetailSpace(tempSpace + 1);
                                tempSpace = (24 - dtt.SignName2.Length) / 2;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dtt.SignName2, dtt.SignName2.Length, ' ', false, true);
                            }
                            else
                                gtf.SetDataDetail(dtt.SignName2, 24, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(48);
                            gtf.SetDataDetail("-", 23, '-', true);
                            gtf.SetDataDetail("-", 24, '-', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(48);

                            tempSpace = 0;
                            if (dtt.TitleSign1 != string.Empty)
                            {
                                tempSpace = (23 - dtt.TitleSign1.Length) / 2;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dtt.TitleSign1, dtt.TitleSign1.Length, ' ');
                            }
                            else
                                gtf.SetDataDetail(dtt.TitleSign1, 23, ' ', true);
                            if (dtt.TitleSign2 != string.Empty)
                            {
                                gtf.SetDataDetailSpace(tempSpace + 1);
                                tempSpace = (24 - dtt.TitleSign2.Length) / 2;
                                gtf.SetDataDetailSpace(tempSpace);
                                gtf.SetDataDetail(dtt.TitleSign2, dtt.TitleSign2.Length, ' ', false, true);
                            }
                            else
                                gtf.SetDataDetail(dtt.TitleSign2, 24, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);

                            gtf.ReplaceGroupHdr(customerName, ldt[i].CustomerName, 57);
                            gtf.ReplaceGroupHdr("No. Inv. : " + invoiceNo2, "No. Inv. : " + ldt[i].InvoiceNo, 38);
                            gtf.ReplaceGroupHdr(alamatCust1, ldt[i].AlamatCust1, 57);
                            gtf.ReplaceGroupHdr("No. FPS  : " + fps, "No. FPS  : " + ldt[i].FPS, 38);
                            gtf.ReplaceGroupHdr(alamatCust2, ldt[i].AlamatCust2, 57);
                            gtf.ReplaceGroupHdr("No. SO   : " + soNo, "No. SO   : " + ldt[i].SONo, 38);
                            if (alamatCust3 != ldt[i].AlamatCust3)
                                gtf.ReplaceGroupHdr(alamatCust3, ldt[i].AlamatCust3, 57);
                            gtf.ReplaceGroupHdr("No. SKPK : " + skpkNo, "No. SKPK : " + ldt[i].SKPKNo, 38);
                            gtf.ReplaceGroupHdr("NPWP : " + custNPWPNo, "NPWP : " + ldt[i].CustNPWPNo, 57);
                            gtf.ReplaceGroupHdr("No. Reff : " + refferenceNo, "No. Reff : " + ldt[i].RefferenceNo, 38);

                            customerName = ldt[i ].CustomerName;
                            invoiceNo2 = ldt[i].InvoiceNo;
                            alamatCust1 = ldt[i].AlamatCust1;
                            fps = ldt[i].FPS;
                            alamatCust2 = ldt[i].AlamatCust2;
                            soNo = ldt[i].SONo;
                            alamatCust3 = ldt[i].AlamatCust3;
                            skpkNo = ldt[i].SKPKNo;
                            custNPWPNo = ldt[i].CustNPWPNo;
                            refferenceNo = ldt[i].RefferenceNo;

                            invoiceNo = ldt[i].InvoiceNo;
                            counterData = 0;
                            decSubJumlah = decSubPotongan = deSubTotPPnBM = decTotPPn = 0;
                        }
                    }

                    if (counterData == 0)
                    {
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail(ldt[i].DONo, 16, ' ', true);
                        gtf.SetDataDetail(ldt[i].BPKNo, 15, ' ', true);
                        gtf.SetDataDetail(ldt[i].BPKDate.Value.ToString("dd-MMM-yyyy"), 15, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("M o d e l", 23, ' ', true);
                        gtf.SetDataDetail("Rangka", 6, ' ', true, false, true);
                        gtf.SetDataDetailSpace(2);
                        gtf.SetDataDetail("No. Mesin", 12, ' ', true);
                        gtf.SetDataDetail("PPn BM Paid", 12, ' ', false, true, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("-", 58, '-', false, true);
                        gtf.PrintData(false, false);
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(ldt[i].ModelDesc, 23, ' ', true);
                    gtf.SetDataDetail(ldt[i].Rangka, 6, ' ', true, false, true);
                    gtf.SetDataDetailSpace(2);
                    gtf.SetDataDetail(ldt[i].Mesin, 12, ' ', true);
                    gtf.SetDataDetail(ldt[i].PPnBMPaid.Value.ToString(), 12, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ldt[i].DPPBefore.Value.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(ldt[i].Jumlah.Value.ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false, false);

                    decSubJumlah += ldt[i].Jumlah.Value;
                    decSubPotongan += ldt[i].potongan.Value;
                    deSubTotPPnBM += ldt[i].TotalPPnBm.Value;
                    decTotPPn += ldt[i].AfterDiscPPn.Value;
                }
            }
            #endregion

            return gtf.sbDataTxt.ToString();
        }
    }
}