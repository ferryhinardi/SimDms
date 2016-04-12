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
    public class txtOmRpSalRgs005 : IRptProc
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
            return CreateReportOmRpSalRgs005(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs005(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Register Retur Invoice (OmRpSalRgs005)
            if (reportID == "OmRpSalRgs005")
            {
                if (textParam != null) gtf.SetGroupHeader(textParam[0].ToString(), 163, ' ', false, true, false, true);
                gtf.SetGroupHeader("SALES TYPE :" + dt.Rows[0]["pSalesType"].ToString(), 60, ' ', true);
                gtf.SetGroupHeader("SALES MODEL :" + dt.Rows[0]["pModel"].ToString(), 102, ' ', false, true);
                gtf.SetGroupHeader("CUSTOMER   :" + dt.Rows[0]["pCust"].ToString(), 60, ' ', true);
                gtf.SetGroupHeader("NO RETUR    :" + dt.Rows[0]["pRtr"].ToString(), 102, ' ', false, true);
                string branchCode = string.Empty;
                if (!string.IsNullOrEmpty(textParam[1].ToString()))
                {
                    branchCode = dt.Rows[0]["BranchCode"].ToString();
                    gtf.SetGroupHeader(textParam[1].ToString() + branchCode, 60, ' ', false, true);
                }
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("TANGGAL", 12, ' ', true);
                gtf.SetGroupHeader("NO. DOKUMEN", 23, ' ', true);
                gtf.SetGroupHeader("PELANGGAN", 41, ' ', true);
                gtf.SetGroupHeader("NO. INVOICE", 14, ' ', true);
                gtf.SetGroupHeader("TGL. INVOICE", 21, ' ', true);
                gtf.SetGroupHeader("NO. DO", 14, ' ', true);
                gtf.SetGroupHeader("TGL. DO", 11, ' ', true);
                gtf.SetGroupHeader("TERM OF PAYMENT", 15, ' ', false, true);
                gtf.SetGroupHeaderSpace(5);
                gtf.SetGroupHeader("SEQ", 3, ' ', true, false, true);
                gtf.SetGroupHeader("MODEL", 19, ' ', true);
                gtf.SetGroupHeader("YEAR", 5, ' ', true, false, true);
                gtf.SetGroupHeader("WARNA", 6, ' ', true);
                gtf.SetGroupHeader("RANGKA", 7, ' ', true);
                gtf.SetGroupHeader("MESIN", 14, ' ', true);
                gtf.SetGroupHeader("PENJUALAN", 15, ' ', true, false, true);
                gtf.SetGroupHeader("POTONGAN", 14, ' ', true, false, true);
                gtf.SetGroupHeader("DPP", 15, ' ', true, false, true);
                gtf.SetGroupHeader("PPN", 15, ' ', true, false, true);
                gtf.SetGroupHeader("PPnBM", 15, ' ', true, false, true);
                gtf.SetGroupHeader("TOTAL", 19, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                DateTime dtReturnDate = new DateTime();
                decimal decTotBeforeDiscDPP = 0, decTotDiscExcludePPn = 0, decTotAfterDiscDPP = 0, decTotAfterDiscPPn = 0, decTotAfterDiscPPnBM = 0,
                    decTotAfterDiscTotal = 0;
                decimal decTotBranchBeforeDiscDPP = 0, decTotBranchDiscExcludePPn = 0, decTotBranchAfterDiscDPP = 0, decTotBranchAfterDiscPPn = 0,
                    decTotBranchAfterDiscPPnBM = 0, decTotBranchAfterDiscTotal = 0;

                int countDetail = 0;
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        if (string.IsNullOrEmpty(textParam[1].ToString())) break;

                        gtf.SetDataReportLine();
                        gtf.SetDataDetail("TOTAL BRANCH " + branchCode + " :", 64, ' ', true);
                        gtf.SetDataDetail(decTotBranchBeforeDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decTotBranchDiscExcludePPn.ToString(), 14, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decTotBranchAfterDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decTotBranchAfterDiscPPn.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decTotBranchAfterDiscPPnBM.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decTotBranchAfterDiscTotal.ToString(), 19, ' ', false, true, true, true, "n0");
                        gtf.SetDataReportLine();

                        gtf.PrintData();
                        break;
                    }

                    if (branchCode != dt.Rows[i]["BranchCode"].ToString() && branchCode != string.Empty)
                    {
                        gtf.SetDataReportLine();
                        gtf.SetDataDetail("TOTAL BRANCH " + branchCode + " :", 64, ' ', true);
                        gtf.SetDataDetail(decTotBranchBeforeDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decTotBranchDiscExcludePPn.ToString(), 14, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decTotBranchAfterDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decTotBranchAfterDiscPPn.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decTotBranchAfterDiscPPnBM.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decTotBranchAfterDiscTotal.ToString(), 19, ' ', false, true, true, true, "n0");
                        gtf.SetDataReportLine();

                        gtf.PrintTotal(true, false, false);

                        gtf.ReplaceGroupHdr(branchCode, dt.Rows[i]["BranchCode"].ToString());
                        branchCode = dt.Rows[i]["BranchCode"].ToString();
                        decTotBranchBeforeDiscDPP = decTotBranchDiscExcludePPn = decTotBranchAfterDiscDPP = decTotBranchAfterDiscPPn =
                            decTotBranchAfterDiscPPnBM = decTotBranchAfterDiscTotal = 0;
                        counterData = 0;
                    }

                    if (dtReturnDate == DateTime.MinValue)
                    {
                        dtReturnDate = Convert.ToDateTime(dt.Rows[i]["ReturnDate"]);
                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dtReturnDate.ToString("dd-MMM-yyyy"), 12, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["ReturnNo"].ToString(), 23, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["pCustomer"].ToString(), 41, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 14, ' ', true);
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"]).ToString("dd-MMM-yyyy"), 21, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["DONo"].ToString(), 14, ' ', true);
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DODate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["TOPName"].ToString(), 15, ' ', false, true);
                    }
                    else
                    {
                        if (dtReturnDate != Convert.ToDateTime(dt.Rows[i]["ReturnDate"]))
                        {
                            dtReturnDate = Convert.ToDateTime(dt.Rows[i]["ReturnDate"]);
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                            gtf.SetDataDetailSpace(1);
                            gtf.SetDataDetail(dtReturnDate.ToString("dd-MMM-yyyy"), 12, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ReturnNo"].ToString(), 23, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["pCustomer"].ToString(), 41, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 14, ' ', true);
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"]).ToString("dd-MMM-yyyy"), 21, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["DONo"].ToString(), 14, ' ', true);
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DODate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["TOPName"].ToString(), 15, ' ', false, true);
                            countDetail = 0;
                        }
                    }

                    countDetail += 1;
                    gtf.SetDataDetailSpace(5);
                    gtf.SetDataDetail(countDetail.ToString(), 3, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 19, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 5, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 6, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BeforeDiscDPP"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscExcludePPn"].ToString(), 14, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscDPP"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscPPn"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscPPnBM"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscTotal"].ToString(), 19, ' ', false, true, true, true, "n0");

                    gtf.PrintData(false);

                    decTotBeforeDiscDPP += Convert.ToDecimal(dt.Rows[i]["BeforeDiscDPP"]);
                    decTotDiscExcludePPn += Convert.ToDecimal(dt.Rows[i]["DiscExcludePPn"]);
                    decTotAfterDiscDPP += Convert.ToDecimal(dt.Rows[i]["AfterDiscDPP"]);
                    decTotAfterDiscPPn += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPn"]);
                    decTotAfterDiscPPnBM += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPnBM"]);
                    decTotAfterDiscTotal += Convert.ToDecimal(dt.Rows[i]["AfterDiscTotal"]);

                    decTotBranchBeforeDiscDPP += Convert.ToDecimal(dt.Rows[i]["BeforeDiscDPP"]);
                    decTotBranchDiscExcludePPn += Convert.ToDecimal(dt.Rows[i]["DiscExcludePPn"]);
                    decTotBranchAfterDiscDPP += Convert.ToDecimal(dt.Rows[i]["AfterDiscDPP"]);
                    decTotBranchAfterDiscPPn += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPn"]);
                    decTotBranchAfterDiscPPnBM += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPnBM"]);
                    decTotBranchAfterDiscTotal += Convert.ToDecimal(dt.Rows[i]["AfterDiscTotal"]);
                }

                gtf.SetTotalDetailLine();
                gtf.SetTotalDetail("TOTAL :", 64, ' ', true);
                gtf.SetTotalDetail(decTotBeforeDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotDiscExcludePPn.ToString(), 14, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotAfterDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotAfterDiscPPn.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotAfterDiscPPnBM.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotAfterDiscTotal.ToString(), 19, ' ', false, true, true, true, "n0");
                gtf.SetTotalDetailLine();
            }

            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}