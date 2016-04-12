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
    public class txtOmRpSalRgs004 : IRptProc 
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
            return CreateReportOmRpSalRgs004(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs004(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Register Faktur Penjualan Unit (OmRpSalRgs004/OmRpSalRgs004HQ)
            if (reportID == "OmRpSalRgs004" || reportID == "OmRpSalRgs004HQ")
            {
                #region Print Group Header
                bCreateBy = false;

                if (textParam != null) gtf.SetGroupHeader(textParam[0].ToString(), 163, ' ', false, true, false, true);
                gtf.SetGroupHeader("SALES TYPE :" + dt.Rows[0]["pSALESTYPE"].ToString(), 60, ' ', true);
                gtf.SetGroupHeader("SALES MODEL :" + dt.Rows[0]["pMODEL"].ToString(), 102, ' ', false, true);
                gtf.SetGroupHeader("CUSTOMER   :" + dt.Rows[0]["pCUST"].ToString(), 60, ' ', true);
                gtf.SetGroupHeader("NO INVOICE  :" + dt.Rows[0]["pINV"].ToString(), 102, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
                gtf.SetGroupHeader("NO. INVOICE", 23, ' ', true);
                gtf.SetGroupHeader("NO.SKPK/NO.REFF", 21, ' ', true);
                gtf.SetGroupHeader("NO. FAKTUR PAJAK", 20, ' ', true);
                gtf.SetGroupHeader("TANGGAL", 12, ' ', true);
                gtf.SetGroupHeader("NAMA FAKTUR PAJAK", 26, ' ', true);
                gtf.SetGroupHeader("PPnBM BAYAR ", 11, ' ', true);
                gtf.SetGroupHeader("NO. DO", 13, ' ', true);
                gtf.SetGroupHeader("NO. SO", 14, ' ', false, true);
                gtf.SetGroupHeaderSpace(5);
                gtf.SetGroupHeader("SEQ", 3, ' ', true, false, true);
                gtf.SetGroupHeader("SURAT JALAN", 14, ' ', true);
                gtf.SetGroupHeader("MODEL", 15, ' ', true);
                gtf.SetGroupHeader("YEAR", 5, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("WARNA", 10, ' ', true);
                gtf.SetGroupHeader("RANGKA", 7, ' ', true);
                gtf.SetGroupHeader("MESIN", 7, ' ', true);
                gtf.SetGroupHeader("QTY", 3, ' ', true);
                gtf.SetGroupHeader("PENJUALAN", 15, ' ', true, false, true);
                gtf.SetGroupHeader("POTONGAN", 9, ' ', true, false, true);
                gtf.SetGroupHeader("DPP", 15, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("PPnBM JUAL", 14, ' ', true, false, true);
                gtf.SetGroupHeader("PPN", 11, ' ', true, false, true);
                gtf.SetGroupHeader("TOTAL", 15, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                string invoiceNo = string.Empty, branch = string.Empty;
                decimal decTotBeforeDiscDPP = 0, decTotDiscExcludePPn = 0, decTotAfterDiscDPP = 0, decTotAfterDiscPPn = 0, decTotAfterDiscPPnBM = 0,
                    decTotAfterDiscTotal = 0, decTotUnit = 0, decTotPPnBMBuyPaid = 0;
                decimal decSubBeforeDiscDPP = 0, decSubDiscExcludePPn = 0, decSubAfterDiscDPP = 0, decSubAfterDiscPPn = 0, decSubAfterDiscPPnBM = 0,
                    decSubAfterDiscTotal = 0, decSubUnit = 0, decSubPPnBMBuyPaid = 0;

                int countDetail = 0;
                #endregion

                #region Print Detail
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        if (reportID == "OmRpSalRgs004HQ")
                        {
                            gtf.SetDataDetailSpace(24);
                            gtf.SetDataDetail("-", 139, '-', false, true);
                            gtf.SetDataDetailSpace(124);
                            gtf.SetDataDetail(decSubPPnBMBuyPaid.ToString(), 11, ' ', false, true, true, true, "n0");
                            gtf.SetDataDetailSpace(24);
                            gtf.SetDataDetail("SUB TOTAL " + dt.Rows[i - 1]["BranchCode"].ToString() + " :", 45, ' ', true);
                            gtf.SetDataDetail(decSubUnit.ToString() + " UNIT", 7, ' ', true);
                            gtf.SetDataDetail(decSubBeforeDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubDiscExcludePPn.ToString(), 9, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubAfterDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetailSpace(1);
                            gtf.SetDataDetail(decSubAfterDiscPPnBM.ToString(), 14, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubAfterDiscPPn.ToString(), 11, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubAfterDiscTotal.ToString(), 15, ' ', false, true, true, true, "n0");
                            gtf.SetDataDetailSpace(24);
                            gtf.SetDataDetail("-", 139, '-', false, true);
                            gtf.SetDataDetail("", 163, ' ', false, true);

                            decTotUnit += decSubUnit;
                            decTotPPnBMBuyPaid += decSubPPnBMBuyPaid;
                            decTotDiscExcludePPn += decSubDiscExcludePPn;
                            decTotBeforeDiscDPP += decSubBeforeDiscDPP;
                            decTotAfterDiscTotal += decSubAfterDiscTotal;
                            decTotAfterDiscPPnBM += decSubAfterDiscPPnBM;
                            decTotAfterDiscPPn += decSubAfterDiscPPn;
                            decTotAfterDiscDPP += decSubAfterDiscDPP;
                        }

                        break;
                    }

                    if (reportID == "OmRpSalRgs004HQ")
                    {
                        if (branch == string.Empty)
                        {
                            branch = dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString();
                            gtf.SetDataDetail(branch, 163, ' ', false, true);
                        }
                        else
                        {
                            if (branch != dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString())
                            {
                                gtf.SetDataDetailSpace(24);
                                gtf.SetDataDetail("-", 139, '-', false, true);
                                gtf.SetDataDetailSpace(124);
                                gtf.SetDataDetail(decSubPPnBMBuyPaid.ToString(), 11, ' ', false, true, true, true, "n0");
                                gtf.SetDataDetailSpace(24);
                                gtf.SetDataDetail("SUB TOTAL " + dt.Rows[i]["BranchCode"].ToString() + " :", 45, ' ', true);
                                gtf.SetDataDetail(decSubUnit.ToString() + " UNIT", 7, ' ', true);
                                gtf.SetDataDetail(decSubBeforeDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubDiscExcludePPn.ToString(), 9, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubAfterDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetailSpace(1);
                                gtf.SetDataDetail(decSubAfterDiscPPnBM.ToString(), 14, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubAfterDiscPPn.ToString(), 11, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubAfterDiscTotal.ToString(), 15, ' ', false, true, true, true, "n0");
                                gtf.SetDataDetailSpace(24);
                                gtf.SetDataDetail("-", 139, '-', false, true);
                                gtf.SetDataDetail("", 163, ' ', false, true);

                                decTotUnit += decSubUnit;
                                decTotPPnBMBuyPaid += decSubPPnBMBuyPaid;
                                decTotDiscExcludePPn += decSubDiscExcludePPn;
                                decTotBeforeDiscDPP += decSubBeforeDiscDPP;
                                decTotAfterDiscTotal += decSubAfterDiscTotal;
                                decTotAfterDiscPPnBM += decSubAfterDiscPPnBM;
                                decTotAfterDiscPPn += decSubAfterDiscPPn;
                                decTotAfterDiscDPP += decSubAfterDiscDPP;

                                decSubUnit = decSubPPnBMBuyPaid = decSubDiscExcludePPn = decSubBeforeDiscDPP = decSubAfterDiscTotal = decSubAfterDiscPPnBM =
                                    decSubAfterDiscPPn = decSubAfterDiscDPP = 0;


                                branch = dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString();
                                gtf.SetDataDetail(branch, 163, ' ', false, true);
                            }
                        }
                    }
                    if (invoiceNo == string.Empty)
                    {
                        invoiceNo = dt.Rows[i]["InvoiceNo"].ToString();
                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 23, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SKPKNo"].ToString(), 21, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["FakturPajakNo"].ToString(), 20, ' ', true);
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["FakturPajakDate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["pCustomer"].ToString(), 26, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["PPnBMBuyPaid"].ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["DONo"].ToString(), 13, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SONo"].ToString(), 14, ' ', false, true);
                    }
                    else
                    {
                        if (invoiceNo != dt.Rows[i]["InvoiceNo"].ToString())
                        {
                            invoiceNo = dt.Rows[i]["InvoiceNo"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 23, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["SKPKNo"].ToString(), 21, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["FakturPajakNo"].ToString(), 20, ' ', true);
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["FakturPajakDate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["pCustomer"].ToString(), 26, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["PPnBMBuyPaid"].ToString(), 11, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["DONo"].ToString(), 13, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["SONo"].ToString(), 14, ' ', false, true);
                            countDetail = 0;
                        }
                    }

                    countDetail += 1;
                    gtf.SetDataDetailSpace(5);
                    gtf.SetDataDetail(countDetail.ToString(), 3, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["BPKNo"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 5, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Qty"].ToString(), 3, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["BeforeDiscDPP"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscExcludePPn"].ToString(), 9, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscDPP"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscPPnBM"].ToString(), 14, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscPPn"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscTotal"].ToString(), 15, ' ', false, true, true, true, "n0");

                    gtf.PrintData(false);

                    if (reportID == "OmRpSalRgs004HQ")
                    {
                        decSubUnit += Convert.ToDecimal(dt.Rows[i]["Qty"]);
                        decSubBeforeDiscDPP += Convert.ToDecimal(dt.Rows[i]["BeforeDiscDPP"]);
                        decSubDiscExcludePPn += Convert.ToDecimal(dt.Rows[i]["DiscExcludePPn"]);
                        decSubAfterDiscDPP += Convert.ToDecimal(dt.Rows[i]["AfterDiscDPP"]);
                        decSubAfterDiscPPn += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPn"]);
                        decSubAfterDiscPPnBM += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPnBM"]);
                        decSubAfterDiscTotal += Convert.ToDecimal(dt.Rows[i]["AfterDiscTotal"]);
                        decSubPPnBMBuyPaid += Convert.ToDecimal(dt.Rows[i]["PPnBMBuyPaid"]);
                    }
                    else
                    {
                        decTotUnit += Convert.ToDecimal(dt.Rows[i]["Qty"]);
                        decTotBeforeDiscDPP += Convert.ToDecimal(dt.Rows[i]["BeforeDiscDPP"]);
                        decTotDiscExcludePPn += Convert.ToDecimal(dt.Rows[i]["DiscExcludePPn"]);
                        decTotAfterDiscDPP += Convert.ToDecimal(dt.Rows[i]["AfterDiscDPP"]);
                        decTotAfterDiscPPn += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPn"]);
                        decTotAfterDiscPPnBM += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPnBM"]);
                        decTotAfterDiscTotal += Convert.ToDecimal(dt.Rows[i]["AfterDiscTotal"]);
                        decTotPPnBMBuyPaid += Convert.ToDecimal(dt.Rows[i]["PPnBMBuyPaid"]);
                    }
                }
                #endregion

                #region Print Footer
                gtf.SetDataReportLine();
                gtf.SetDataDetailSpace(124);
                gtf.SetDataDetail(decTotPPnBMBuyPaid.ToString(), 11, ' ', false, true, true, true, "n0");
                gtf.SetDataDetail("GRAND TOTAL :", 69, ' ', true);
                gtf.SetDataDetail(decTotUnit.ToString() + " UNIT", 7, ' ', true, false, true);
                gtf.SetDataDetail(decTotBeforeDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(decTotDiscExcludePPn.ToString(), 9, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(decTotAfterDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetailSpace(1);
                gtf.SetDataDetail(decTotAfterDiscPPnBM.ToString(), 14, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(decTotAfterDiscPPn.ToString(), 11, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(decTotAfterDiscTotal.ToString(), 15, ' ', false, true, true, true, "n0");
                gtf.SetDataReportLine();
                gtf.PrintData(false);

                gtf.SetDataDetail("", 163, ' ', false, true);
                gtf.SetDataDetail("SUMMARY :", 163, ' ', false, true);
                gtf.SetDataDetail("-", 107, '-', false, true);
                gtf.SetDataDetail("NO.", 3, ' ', true, false, true);
                gtf.SetDataDetailSpace(1);
                gtf.SetDataDetail("MODEL", 16, ' ', true);
                gtf.SetDataDetail("MODEL DESC", 21, ' ', true);
                gtf.SetDataDetail("KD.WARNA", 9, ' ', true);
                gtf.SetDataDetail("WARNA", 21, ' ', true);
                gtf.SetDataDetail("QUANTITY", 9, ' ', true, false, true);
                gtf.SetDataDetailSpace(1);
                gtf.SetDataDetail("AMOUNT / HARGA JUAL", 19, ' ', false, true);
                gtf.SetDataDetail("-", 107, '-', false, true);
                gtf.PrintData(false);

                counterData = 0;
                decSubUnit = decSubAfterDiscTotal = decTotUnit = decTotAfterDiscTotal = 0;
                decimal decSubBranchUnit = 0, decSubBranchAfterDiscTotal = 0;
                branch = string.Empty;
                string salesModelCode = string.Empty;
                for (int i = 0; i <= dt.DataSet.Tables[1].Rows.Count; i++)
                {
                    if (i == dt.DataSet.Tables[1].Rows.Count)
                    {
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("-", 102, '-', false, true);
                        gtf.SetDataDetailSpace(54);
                        gtf.SetDataDetail("Total per Model :", 21, ' ', true, false, true);
                        gtf.SetDataDetail(decSubUnit.ToString() + " Unit", 9, ' ', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(decSubAfterDiscTotal.ToString(), 19, ' ', false, true, true, true, "n0");

                        gtf.PrintData(false);
                        decTotUnit += decSubUnit;
                        decTotAfterDiscTotal += decSubAfterDiscTotal;
                        break;
                    }

                    if (reportID == "OmRpSalRgs004HQ")
                    {
                        if (branch == string.Empty)
                        {
                            branch = dt.DataSet.Tables[1].Rows[i]["BranchCode"].ToString() + " " + dt.DataSet.Tables[1].Rows[i]["CompanyName"].ToString();
                            gtf.SetDataDetail(branch, 107, ' ', false, true);
                        }
                    }

                    if (salesModelCode == string.Empty)
                    {
                        salesModelCode = dt.DataSet.Tables[1].Rows[i]["SalesModelCode"].ToString();
                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    }
                    else
                    {
                        if (salesModelCode != dt.DataSet.Tables[1].Rows[i]["SalesModelCode"].ToString())
                        {
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("-", 102, '-', false, true);
                            gtf.SetDataDetailSpace(54);
                            gtf.SetDataDetail("Total per Model :", 21, ' ', true, false, true);
                            gtf.SetDataDetail(decSubUnit.ToString() + " Unit", 9, ' ', true, false, true);
                            gtf.SetDataDetailSpace(1);
                            gtf.SetDataDetail(decSubAfterDiscTotal.ToString(), 19, ' ', false, true, true, true, "n0");

                            gtf.PrintData(false);

                            if (reportID == "OmRpSalRgs004HQ")
                            {
                                decSubBranchUnit += decSubUnit;
                                decSubBranchAfterDiscTotal += decSubAfterDiscTotal;
                            }
                            else
                            {
                                decTotUnit += decSubUnit;
                                decTotAfterDiscTotal += decSubAfterDiscTotal;
                            }

                            decSubUnit = decSubAfterDiscTotal = 0;

                            if (reportID == "OmRpSalRgs004HQ")
                            {
                                if (branch != dt.DataSet.Tables[1].Rows[i]["BranchCode"].ToString() + " " + dt.DataSet.Tables[1].Rows[i]["CompanyName"].ToString())
                                {
                                    // Nothing to do.
                                }
                                else
                                {
                                    salesModelCode = dt.DataSet.Tables[1].Rows[i]["SalesModelCode"].ToString();
                                    counterData += 1;
                                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                                }
                            }
                            else
                            {
                                salesModelCode = dt.DataSet.Tables[1].Rows[i]["SalesModelCode"].ToString();
                                counterData += 1;
                                gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                            }
                        }

                        if (reportID == "OmRpSalRgs004HQ")
                        {
                            if (branch != dt.DataSet.Tables[1].Rows[i]["BranchCode"].ToString() + " " + dt.DataSet.Tables[1].Rows[i]["CompanyName"].ToString())
                            {
                                gtf.SetDataDetailSpace(22);
                                gtf.SetDataDetail("-", 85, '-', false, true);
                                gtf.SetDataDetailSpace(22);
                                gtf.SetDataDetail("Total per " + dt.DataSet.Tables[1].Rows[i]["BranchCode"].ToString(), 53, ' ', true);
                                gtf.SetDataDetail(": " + decSubBranchUnit.ToString() + " Unit", 9, ' ', true, false, true);
                                gtf.SetDataDetailSpace(1);
                                gtf.SetDataDetail(decSubBranchAfterDiscTotal.ToString(), 19, ' ', false, true, true, true, "n0");

                                decTotUnit += decSubBranchUnit;
                                decTotAfterDiscTotal += decSubBranchAfterDiscTotal;

                                decSubBranchUnit = decSubBranchAfterDiscTotal = 0;
                                counterData += 1;
                                branch = dt.DataSet.Tables[1].Rows[i]["BranchCode"].ToString() + " " + dt.DataSet.Tables[1].Rows[i]["CompanyName"].ToString();
                                gtf.SetDataDetail(branch, 107, ' ', false, true);
                                salesModelCode = dt.DataSet.Tables[1].Rows[i]["SalesModelCode"].ToString();
                                counterData += 1;
                                gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                            }
                        }
                    }

                    if (decSubUnit == 0)
                    {
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["SalesModelCode"].ToString(), 16, ' ', true);
                        gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["SalesModelDesc"].ToString(), 21, ' ', true);
                    }
                    else
                        gtf.SetDataDetailSpace(40);
                    gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["ColourCode"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["ColourName"].ToString(), 21, ' ', true);
                    gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["Unit"].ToString() + " Unit", 9, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["AfterDiscTotal"].ToString(), 19, ' ', false, true, true, true, "n0");

                    gtf.PrintData(false);

                    decSubUnit += Convert.ToDecimal(dt.DataSet.Tables[1].Rows[i]["Unit"]);
                    decSubAfterDiscTotal += Convert.ToDecimal(dt.DataSet.Tables[1].Rows[i]["AfterDiscTotal"]);
                }

                gtf.SetDataDetail("-", 107, '-', false, true);
                gtf.SetDataDetail("GRAND TOTAL :", 75, ' ', true);
                gtf.SetDataDetail(decTotUnit.ToString() + " Unit", 9, ' ', true, false, true);
                gtf.SetDataDetailSpace(1);
                gtf.SetDataDetail(decTotAfterDiscTotal.ToString(), 19, ' ', false, true, true, true, "n0");
                gtf.SetDataDetail("-", 107, '-', false, true);

                gtf.PrintData(false);

                gtf.SetTotalDetailLine();
                #endregion
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}