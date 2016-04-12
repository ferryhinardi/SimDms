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
    public class txtOmRpSalesTrn005:IRptProc 
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
            return CreateReportOmRpSalesTrn005(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalesTrn005(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Return (OmRpSalesTrn005/OmRpSalesTrn005A)
            if (reportID == "OmRpSalesTrn005" || reportID == "OmRpSalesTrn005A")
            {
                bCreateBy = false;

                string customer = "N a m a            : " + dt.Rows[0]["CustomerCode"].ToString() + " " + dt.Rows[0]["CustomerName"].ToString();
                string address = string.Empty;
                if (reportID == "OmRpSalesTrn005")
                    address = "A l a m a t        : " + dt.Rows[0]["CustomerAddress"].ToString();
                else
                    address = "A l a m a t        : " + dt.Rows[0]["CustomerAddress"].ToString() + ", " + dt.Rows[0]["CustomerCityCode"].ToString();

                string custCityCode = dt.Rows[0]["CustomerCityCode"].ToString();
                string NPWP = "N P W P            : " + dt.Rows[0]["CustomerNPWP"].ToString();
                string ReturnNo = "No. : " + dt.Rows[0]["ReturnNo"].ToString();
                string subTitle = "(Atas Faktur Pajak No. " + dt.Rows[0]["FakturPajakNo"].ToString() + " Tgl. " + Convert.ToDateTime(dt.Rows[0]["FakturPajakDate"]).ToString("dd-MMM-yyyy") + ")";
                string company = "N a m a            : " + dt.Rows[0]["CompanyName"].ToString();
                string addrCompany = "A l a m a t        : " + dt.Rows[0]["CompanyAddress"].ToString();
                string compCityCode = dt.Rows[0]["CompanyCityCode"].ToString();
                string compNPWP = "N P W P            : " + dt.Rows[0]["CompanyNPWP"].ToString();
                string compSKP = "No. Pengukuhan PKP : " + dt.Rows[0]["CompanySKP"].ToString();

                gtf.SetGroupHeader("PEMBELI", 96, ' ', false, true);
                if (reportID == "OmRpSalesTrn005")
                {
                    gtf.SetGroupHeader(customer, 96, ' ', false, true);
                }
                else
                {
                    gtf.SetGroupHeader(customer, 47, ' ', true);
                    gtf.SetGroupHeader(NPWP, 48, ' ', false, true, true);
                }
                gtf.SetGroupHeader(address, 96, ' ', false, true);
                if (reportID == "OmRpSalesTrn005")
                {
                    gtf.SetGroupHeaderSpace(21);
                    gtf.SetGroupHeader(custCityCode, 75, ' ', false, true);
                    gtf.SetGroupHeader("", 96, ' ', false, true);
                    gtf.SetGroupHeader(NPWP, 96, ' ', false, true);
                    gtf.SetGroupHeaderLine();
                }
                gtf.SetGroupHeaderSpace(43);
                gtf.SetGroupHeader("NOTA RETUR", 10, ' ');
                if (setTextParameter[0].ToString() != string.Empty)
                    gtf.SetGroupHeader(ReturnNo + " (" + setTextParameter[0].ToString() + ")", 43, ' ', false, true, true);
                else
                    gtf.SetGroupHeader(ReturnNo, 43, ' ', false, true, true);
                gtf.SetGroupHeader(subTitle, 96, ' ', false, true, false, true);
                if (reportID == "OmRpSalesTrn005")
                    gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("KEPADA PENJUAL", 96, ' ', false, true);
                if (reportID == "OmRpSalesTrn005")
                {
                    gtf.SetGroupHeader(company, 96, ' ', false, true);
                }
                else
                {
                    gtf.SetGroupHeader(company, 47, ' ', true);
                    gtf.SetGroupHeader(compNPWP, 48, ' ', false, true, true);
                }
                gtf.SetGroupHeader(addrCompany, 96, ' ', false, true);
                if (reportID == "OmRpSalesTrn005")
                {
                    gtf.SetGroupHeaderSpace(21);
                    gtf.SetGroupHeader(compCityCode, 75, ' ', false, true);
                    gtf.SetGroupHeader("", 96, ' ', false, true);
                    gtf.SetGroupHeader(compNPWP, 96, ' ', false, true);
                }
                gtf.SetGroupHeader(compSKP, 96, ' ', false, true);
                gtf.SetGroupHeader(" -", 95, '-', false, true);

                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("No.", 4, ' ', true, false, true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Nama Barang Kena Pajak/Barang Mewah Yang", 40, ' ');
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeaderSpace(8);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Harga Satuan Menurut", 20, ' ', true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Harga Jual Yang", 15, ' ', true, false, true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Urut", 4, ' ', true, false, true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Dikembalikan", 39, ' ', true, false, false, true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Kuantum", 7, ' ', true, false, true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Faktur Pajak", 20, ' ', true, false, false, true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("Dikembalikan", 15, ' ', true, false, false, true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeaderSpace(5);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeaderSpace(40);
                gtf.SetGroupHeader("|", 1);
                gtf.SetGroupHeaderSpace(8);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("(Rp)", 20, ' ', true, false, false, true);
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("(Rp)", 15, ' ', true, false, false, true);
                gtf.SetGroupHeader("|", 1, ' ', false, true);

                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 5, '-');
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 40, '-');
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 8, '-');
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 21, '-');
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeader("-", 16, '-');
                gtf.SetGroupHeader("|", 1, ' ', false, true);
                gtf.PrintHeader();

                string returnNo = string.Empty;
                int loop = 0;
                decimal totDiscExcludePPN = 0, totAfterDiscPPN = 0, totAfterDiscPPNBM = 0, totAfterDiscDPP = 0;
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        if (reportID == "OmRpSalesTrn005")
                        {
                            gtf.SetDataDetail(" -", 95, '-', false, true);
                            gtf.PrintData(false, false);
                            loop = 59 - (gtf.line + 23);
                            for (int j = 0; j < loop; j++)
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                            }
                        }

                        gtf.SetDataReportLine();
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("Jumlah Diskon", 72, ' ', true);
                        if (reportID == "OmRpSalesTrn005")
                            gtf.SetDataDetail("|", 1, ' ');
                        else
                            gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(totDiscExcludePPN.ToString(), 17, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false, false);
                        if (reportID == "OmRpSalesTrn005")
                        {
                            gtf.SetDataDetail("-", 78, '-');
                            gtf.SetDataDetail("|", 1, ' ');
                            gtf.SetDataDetail("-", 17, '-', false, true);
                            gtf.PrintData(false, false);
                        }
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("Jumlah Harga Jual Yang Dikembalikan", 72, ' ', true);
                        if (reportID == "OmRpSalesTrn005")
                            gtf.SetDataDetail("|", 1, ' ');
                        else
                            gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(totAfterDiscDPP.ToString(), 17, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false, false);
                        if (reportID == "OmRpSalesTrn005")
                        {
                            gtf.SetDataDetail("-", 78, '-');
                            gtf.SetDataDetail("|", 1, ' ');
                            gtf.SetDataDetail("-", 17, '-', false, true);
                            gtf.PrintData(false, false);
                        }
                        gtf.SetDataDetailSpace(5);
                        if (reportID == "OmRpSalesTrn005")
                        {
                            gtf.SetDataDetail("Jumlah Pajak Yang Dikurangkan :", 72, ' ', true);
                            gtf.SetDataDetail("|", 1, ' ', false, true);
                        }
                        else
                            gtf.SetDataDetail("Jumlah Pajak Yang Dikurangkan :", 72, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("a. Pajak Pertambahan Nilai", 72, ' ', true);
                        if (reportID == "OmRpSalesTrn005")
                            gtf.SetDataDetail("|", 1, ' ');
                        else
                            gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(totAfterDiscPPN.ToString(), 17, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("a. Pajak Penjualan atas Barang Mewah", 72, ' ', true);
                        if (reportID == "OmRpSalesTrn005")
                            gtf.SetDataDetail("|", 1, ' ');
                        else
                            gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(totAfterDiscPPNBM.ToString(), 17, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false, false);
                        if (reportID == "OmRpSalesTrn005")
                        {
                            gtf.SetDataReportLine();
                            gtf.PrintData(false, false);
                        }
                        gtf.SetDataDetailSpace(78);
                        gtf.SetDataDetail(DateTime.Now.ToString("dd MMMM yyyy").ToUpper(), 18, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(62);
                        gtf.SetDataDetail("Pembeli,", 34, ' ', false, true);
                        if (reportID == "OmRpSalesTrn005")
                        {
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                        }
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(62);
                        gtf.SetDataDetail(dt.Rows[i - 1]["CustomerName"].ToString(), 34, ' ', false, true);
                        gtf.PrintData(false, false);
                        if (reportID == "OmRpSalesTrn005")
                        {
                            gtf.SetDataReportLine();
                            gtf.PrintData(false, false);
                        }
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("Lembar ke-1 : Untuk Pengusaha Kena Pajak yang menerbitkan Faktur Pajak", 91, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("Lembar ke-2 : Untuk Pembeli", 91, ' ', false, true);
                        gtf.PrintData(false, false);
                        if (reportID == "OmRpSalesTrn005")
                        {
                            gtf.SetDataReportLine();
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("Keterangan : No. DO  : " + dt.Rows[i - 1]["DONo"].ToString() + "         " + "Tgl. : " + Convert.ToDateTime(dt.Rows[i - 1]["DODate"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("             No. SO  : " + dt.Rows[i - 1]["SONo"].ToString() + "         " + "Tgl. : " + Convert.ToDateTime(dt.Rows[i - 1]["SODate"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                            gtf.PrintData(false, false);
                        }
                        else
                        {
                            gtf.SetDataDetail("Keterangan : No. DO  : " + dt.Rows[i - 1]["DONo"].ToString() + " " + "Tgl. : " + Convert.ToDateTime(dt.Rows[i - 1]["DODate"]).ToString("dd-MMM-yyyy") + "   " + "No. SO  : " + dt.Rows[i - 1]["SONo"].ToString() + " " + "Tgl. " + Convert.ToDateTime(dt.Rows[i - 1]["SODate"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                            gtf.PrintData(false, false);
                        }

                        gtf.SetDataDetail("             No. BPK : " + dt.Rows[i - 1]["BPKNo"].ToString() + "         " + "Tgl. : " + Convert.ToDateTime(dt.Rows[i - 1]["BPKDate"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        if (reportID == "OmRpSalesTrn005")
                        {
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                        }
                        break;
                    }

                    if (returnNo == string.Empty)
                    {
                        returnNo = dt.Rows[i]["ReturnNo"].ToString();
                    }
                    else
                    {
                        if (returnNo != dt.Rows[i]["ReturnNo"].ToString())
                        {
                            if (reportID == "OmRpSalesTrn005")
                            {
                                gtf.SetDataDetail(" -", 95, '-', false, true);
                                gtf.PrintData(false, false);
                                loop = 59 - (gtf.line + 23);
                                for (int j = 0; j < loop; j++)
                                {
                                    gtf.SetDataDetail("", 96, ' ', false, true);
                                    gtf.PrintData(false, false);
                                }
                            }

                            gtf.SetDataReportLine();
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("Jumlah Diskon", 72, ' ', true);
                            if (reportID == "OmRpSalesTrn005")
                                gtf.SetDataDetail("|", 1, ' ');
                            else
                                gtf.SetDataDetailSpace(1);
                            gtf.SetDataDetail(totDiscExcludePPN.ToString(), 17, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);
                            if (reportID == "OmRpSalesTrn005")
                            {
                                gtf.SetDataDetail("-", 78, '-');
                                gtf.SetDataDetail("|", 1, ' ');
                                gtf.SetDataDetail("-", 17, '-', false, true);
                                gtf.PrintData(false, false);
                            }
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("Jumlah Harga Jual Yang Dikembalikan", 72, ' ', true);
                            if (reportID == "OmRpSalesTrn005")
                                gtf.SetDataDetail("|", 1, ' ');
                            else
                                gtf.SetDataDetailSpace(1);
                            gtf.SetDataDetail(totAfterDiscDPP.ToString(), 17, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);
                            if (reportID == "OmRpSalesTrn005")
                            {
                                gtf.SetDataDetail("-", 78, '-');
                                gtf.SetDataDetail("|", 1, ' ');
                                gtf.SetDataDetail("-", 17, '-', false, true);
                                gtf.PrintData(false, false);
                            }
                            gtf.SetDataDetailSpace(5);
                            if (reportID == "OmRpSalesTrn005")
                            {
                                gtf.SetDataDetail("Jumlah Pajak Yang Dikurangkan :", 72, ' ', true);
                                gtf.SetDataDetail("|", 1, ' ', false, true);
                            }
                            else
                                gtf.SetDataDetail("Jumlah Pajak Yang Dikurangkan :", 72, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("a. Pajak Pertambahan Nilai", 72, ' ', true);
                            if (reportID == "OmRpSalesTrn005")
                                gtf.SetDataDetail("|", 1, ' ');
                            else
                                gtf.SetDataDetailSpace(1);
                            gtf.SetDataDetail(totAfterDiscPPN.ToString(), 17, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("a. Pajak Penjualan atas Barang Mewah", 72, ' ', true);
                            if (reportID == "OmRpSalesTrn005")
                                gtf.SetDataDetail("|", 1, ' ');
                            else
                                gtf.SetDataDetailSpace(1);
                            gtf.SetDataDetail(totAfterDiscPPNBM.ToString(), 17, ' ', false, true, true, true, "n0");
                            gtf.PrintData(false, false);
                            if (reportID == "OmRpSalesTrn005")
                            {
                                gtf.SetDataReportLine();
                                gtf.PrintData(false, false);
                            }
                            gtf.SetDataDetailSpace(78);
                            gtf.SetDataDetail(DateTime.Now.ToString("dd MMMM yyyy").ToUpper(), 18, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(62);
                            gtf.SetDataDetail("Pembeli,", 34, ' ', false, true);
                            if (reportID == "OmRpSalesTrn005")
                            {
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                            }
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(62);
                            gtf.SetDataDetail(dt.Rows[i - 1]["CustomerName"].ToString(), 34, ' ', false, true);
                            gtf.PrintData(false, false);
                            if (reportID == "OmRpSalesTrn005")
                            {
                                gtf.SetDataReportLine();
                                gtf.PrintData(false, false);
                            }
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("Lembar ke-1 : Untuk Pengusaha Kena Pajak yang menerbitkan Faktur Pajak", 91, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("Lembar ke-2 : Untuk Pembeli", 91, ' ', false, true);
                            gtf.PrintData(false, false);
                            if (reportID == "OmRpSalesTrn005")
                            {
                                gtf.SetDataReportLine();
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("Keterangan : No. DO  : " + dt.Rows[i - 1]["DONo"].ToString() + "         " + "Tgl. : " + Convert.ToDateTime(dt.Rows[i - 1]["DODate"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("             No. SO  : " + dt.Rows[i - 1]["SONo"].ToString() + "         " + "Tgl. : " + Convert.ToDateTime(dt.Rows[i - 1]["SODate"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                                gtf.PrintData(false, false);
                            }
                            else
                            {
                                gtf.SetDataDetail("Keterangan : No. DO  : " + dt.Rows[i - 1]["DONo"].ToString() + " " + "Tgl. : " + Convert.ToDateTime(dt.Rows[i - 1]["DODate"]).ToString("dd-MMM-yyyy") + "   " + "No. SO  : " + dt.Rows[i - 1]["SONo"].ToString() + " " + "Tgl. " + Convert.ToDateTime(dt.Rows[i - 1]["SODate"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                                gtf.PrintData(false, false);
                            }

                            gtf.SetDataDetail("             No. BPK : " + dt.Rows[i - 1]["BPKNo"].ToString() + "         " + "Tgl. : " + Convert.ToDateTime(dt.Rows[i - 1]["BPKDate"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            if (reportID == "OmRpSalesTrn005")
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                            }

                            counterData = 0;
                            returnNo = dt.Rows[i]["ReturnNo"].ToString();
                            totDiscExcludePPN = totAfterDiscPPN = totAfterDiscPPNBM = totAfterDiscDPP = 0;

                            gtf.ReplaceGroupHdr(customer, "N a m a            : " + dt.Rows[i]["CustomerCode"].ToString() + " " + dt.Rows[i]["CustomerName"].ToString(), 96);
                            gtf.ReplaceGroupHdr(address, "A l a m a t        : " + dt.Rows[i]["CustomerAddress"].ToString(), 96);
                            gtf.ReplaceGroupHdr(custCityCode, dt.Rows[i]["CustomerCityCode"].ToString(), 75);
                            gtf.ReplaceGroupHdr(NPWP, "N P W P            : " + dt.Rows[i]["CustomerNPWP"].ToString(), 96);
                            gtf.ReplaceGroupHdr(ReturnNo, "No. : " + dt.Rows[i]["ReturnNo"].ToString(), 43);
                            gtf.ReplaceGroupHdr(subTitle, "(Atas Faktur Pajak No. " + dt.Rows[i]["FakturPajakNo"].ToString() + " Tgl. " + Convert.ToDateTime(dt.Rows[i]["FakturPajakDate"]).ToString("dd-MMM-yyyy") + ")", 96);
                            gtf.ReplaceGroupHdr(company, "N a m a            : " + dt.Rows[i]["CompanyName"].ToString(), 96);
                            gtf.ReplaceGroupHdr(addrCompany, "A l a m a t        : " + dt.Rows[i]["CompanyAddress"].ToString(), 96);
                            gtf.ReplaceGroupHdr(compCityCode, dt.Rows[i]["CompanyCityCode"].ToString(), 75);
                            gtf.ReplaceGroupHdr(compNPWP, "N P W P            : " + dt.Rows[i]["CompanyNPWP"].ToString(), 96);
                            gtf.ReplaceGroupHdr(compSKP, "No. Pengukuhan PKP : " + dt.Rows[i]["CompanySKP"].ToString(), 96);

                            customer = "N a m a            : " + dt.Rows[i]["CustomerCode"].ToString() + " " + dt.Rows[i]["CustomerName"].ToString();
                            address = "A l a m a t        : " + dt.Rows[i]["CustomerAddress"].ToString();
                            custCityCode = dt.Rows[i]["CustomerCityCode"].ToString();
                            NPWP = "N P W P            : " + dt.Rows[i]["CustomerNPWP"].ToString();
                            ReturnNo = "No. : " + dt.Rows[i]["ReturnNo"].ToString();
                            subTitle = "Atas Faktur Pajak No. " + dt.Rows[i]["FakturPajakNo"].ToString() + " Tgl. " + Convert.ToDateTime(dt.Rows[i]["FakturPajakDate"]).ToString("dd-MMM-yyyy");
                            company = "N a m a            : " + dt.Rows[i]["CompanyName"].ToString();
                            addrCompany = "A l a m a t        : " + dt.Rows[i]["CompanyAddress"].ToString();
                            compCityCode = dt.Rows[i]["CompanyCityCode"].ToString();
                            compNPWP = "N P W P            : " + dt.Rows[i]["CompanyNPWP"].ToString();
                            compSKP = "No. Pengukuhan PKP : " + dt.Rows[i]["CompanySKP"].ToString();
                        }
                    }
                    counterData += 1;
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(counterData.ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 39, ' ', true);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(dt.Rows[i]["Quantity"].ToString(), 7, ' ', true, false, true);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(dt.Rows[i]["BeforeDiscDPP"].ToString(), 20, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(dt.Rows[i]["TotalBeforeDisc"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);

                    totDiscExcludePPN += Convert.ToDecimal(dt.Rows[i]["DiscExcludePPN"]);
                    totAfterDiscDPP += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPN"]);
                    totAfterDiscPPNBM += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPNBM"]);
                    totAfterDiscDPP += Convert.ToDecimal(dt.Rows[i]["TotalAfterDiscDPP"]);
                }
            }
            #endregion

            return gtf.sbDataTxt.ToString();
        }
    }
}