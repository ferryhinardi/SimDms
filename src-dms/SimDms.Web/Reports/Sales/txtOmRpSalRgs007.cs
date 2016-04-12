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
    public class txtOmRpSalRgs007 : IRptProc
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
            return CreateReportOmRpSalRgs007(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs007(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Register Nota Debet (OmRpSalRgs007/OmRpSalRgs007HQ)
            if (reportID == "OmRpSalRgs007" || reportID == "OmRpSalRgs007HQ")
            {
                bCreateBy = false;
                #region Print Group Header
                if (textParam != null)
                {
                    gtf.SetGroupHeader(textParam[0].ToString(), 163, ' ', false, true, false, true);
                }
                gtf.SetGroupHeader("NO INVOICE : " + dt.Rows[0]["pInvoice"].ToString(), 163, ' ', false, true);
                string branch = dt.Rows[0]["BranchCode"].ToString() + " " + dt.Rows[0]["BranchName"].ToString();
                string branchCode = dt.Rows[0]["BranchCode"].ToString();
                if (textParam[1].ToString() != string.Empty)
                    gtf.SetGroupHeader(textParam[1].ToString() + branch, 163, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
                gtf.SetGroupHeader("NO. D/N", 13, ' ', true);
                gtf.SetGroupHeader("NO. INVOICE", 13, ' ', true);
                gtf.SetGroupHeader("NAMA FAKTUR", 45, ' ', true);
                gtf.SetGroupHeader("NO DO", 13, ' ', true);
                gtf.SetGroupHeader("NO SO", 13, ' ', true);
                gtf.SetGroupHeader("SURAT JALAN", 13, ' ', true);
                gtf.SetGroupHeader("MODEL", 15, ' ', true);
                gtf.SetGroupHeader("YEAR", 5, ' ', true, false, true);
                gtf.SetGroupHeader("WARNA", 5, ' ', true);
                gtf.SetGroupHeader("SEQ", 3, ' ', false, true, true);

                gtf.SetGroupHeaderSpace(4);
                gtf.SetGroupHeader("KETERANGAN BBN", 46, ' ', true);
                gtf.SetGroupHeader("JUMLAH BBN", 15, ' ', true, false, true);
                gtf.SetGroupHeader("ONGKOS KIRIM", 12, ' ', true, false, true);
                gtf.SetGroupHeader("LAIN-LAIN", 9, ' ', true, false, true);
                gtf.SetGroupHeader("KETERANGAN KIR", 14, ' ', true);
                gtf.SetGroupHeaderSpace(31);
                gtf.SetGroupHeader("JML. DEPOSIT", 15, ' ', true, false, true);
                gtf.SetGroupHeader("JUMLAH KIR", 11, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                decimal decSubBBN = 0, decSubShipAmt = 0, decSubOthersAmt = 0, decSubDepositAmt = 0, decSubKIR = 0;
                decimal decTotBBN = 0, decTotShipAmt = 0, decTotOthersAmt = 0, decTotDepositAmt = 0, decTotKIR = 0;
                #endregion

                #region Print Detail
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        if (reportID == "OmRpSalRgs007HQ")
                        {
                            gtf.SetDataReportLine();
                            gtf.SetDataDetail("SUB TOTAL " + branchCode + " :", 50, ' ', true, false, true);
                            gtf.SetDataDetail(decSubBBN.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubShipAmt.ToString(), 12, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubOthersAmt.ToString(), 9, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetailSpace(46);
                            gtf.SetDataDetail(decSubDepositAmt.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubKIR.ToString(), 11, ' ', false, true, true, true, "n0");
                            gtf.SetDataReportLine();

                            gtf.PrintData();
                        }

                        decTotBBN += decSubBBN;
                        decTotShipAmt += decSubShipAmt;
                        decTotOthersAmt += decSubOthersAmt;
                        decTotDepositAmt += decSubDepositAmt;
                        decTotKIR += decSubKIR;
                        break;
                    }

                    #region OmRpSalRgs007HQ
                    if (reportID == "OmRpSalRgs007HQ")
                    {
                        if (branch != dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString())
                        {
                            gtf.SetDataReportLine();
                            gtf.SetDataDetail("SUB TOTAL " + branchCode + " :", 50, ' ', true, false, true);
                            gtf.SetDataDetail(decSubBBN.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubShipAmt.ToString(), 12, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubOthersAmt.ToString(), 9, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetailSpace(46);
                            gtf.SetDataDetail(decSubDepositAmt.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubKIR.ToString(), 11, ' ', false, true, true, true, "n0");
                            gtf.SetDataReportLine();

                            gtf.PrintTotal(true, false, false);

                            gtf.ReplaceGroupHdr(branch, dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString());
                            branch = dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString();
                            branchCode = dt.Rows[i]["BranchCode"].ToString();

                            decTotBBN += decSubBBN;
                            decTotShipAmt += decSubShipAmt;
                            decTotOthersAmt += decSubOthersAmt;
                            decTotDepositAmt += decSubDepositAmt;
                            decTotKIR += decSubKIR;

                            counterData = 0;
                            decSubBBN = decSubShipAmt = decSubOthersAmt = decSubDepositAmt = decSubKIR = 0;
                        }
                    }
                    #endregion

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DNDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DNNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["pCustomer"].ToString(), 45, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DONo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SONo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BPKNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 5, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 5, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DNSeq"].ToString(), 3, ' ', false, true, true);
                    gtf.SetDataDetailSpace(4);
                    gtf.SetDataDetail(dt.Rows[i]["ketBBN"].ToString(), 46, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BBN"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["ShipAmt"].ToString(), 12, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["OthersAmt"].ToString(), 9, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["ketKIR"].ToString(), 14, ' ', true);
                    gtf.SetDataDetailSpace(31);
                    gtf.SetDataDetail(dt.Rows[i]["DepositAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["KIR"].ToString(), 11, ' ', false, true, true, true, "n0");

                    gtf.PrintData(false);

                    decSubBBN += Convert.ToDecimal(dt.Rows[i]["BBN"]);
                    decSubShipAmt += Convert.ToDecimal(dt.Rows[i]["ShipAmt"]);
                    decSubOthersAmt += Convert.ToDecimal(dt.Rows[i]["OthersAmt"]);
                    decSubDepositAmt += Convert.ToDecimal(dt.Rows[i]["DepositAmt"]);
                    decSubKIR += Convert.ToDecimal(dt.Rows[i]["KIR"]);
                }
                #endregion

                #region Print Footer
                gtf.SetTotalDetailLine();
                if (reportID == "OmRpSalRgs007HQ")
                    gtf.SetTotalDetail("GRAND TOTAL :", 50, ' ', true);
                else
                    gtf.SetTotalDetail("TOTAL :", 50, ' ', true);
                gtf.SetTotalDetail(decTotBBN.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotShipAmt.ToString(), 12, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotOthersAmt.ToString(), 9, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetailSpace(46);
                gtf.SetTotalDetail(decTotDepositAmt.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotKIR.ToString(), 11, ' ', false, true, true, true, "n0");
                gtf.SetTotalDetailLine();
                #endregion
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}