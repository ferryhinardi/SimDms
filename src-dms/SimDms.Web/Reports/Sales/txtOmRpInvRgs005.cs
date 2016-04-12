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
    public class txtOmRpInvRgs005 : IRptProc  
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
            return CreateReportOmRpInvRgs005(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpInvRgs005(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Register (Transfer In / Out) Gudang (OmRpInvRgs005/OmRpInvRgs006)
            if (reportID == "OmRpInvRgs005" || reportID == "OmRpInvRgs006")
            {
                #region Print Group Header
                gtf.SetGroupHeader("STATUS : " + textParam[0].ToString(), 233, ' ', false, true, false, true);
                gtf.SetGroupHeader("PERIODE : " + Convert.ToDateTime(textParam[1]).ToString("dd-MMM-yyyy") + " S/D " + Convert.ToDateTime(textParam[2]).ToString("dd-MMM-yyyy"), 233, ' ', false, true, false, true);
                string branchCode = string.Empty;
                if (!string.IsNullOrEmpty(textParam[4].ToString()))
                {
                    branchCode = dt.Rows[0]["BranchCode"].ToString();
                    gtf.SetGroupHeader(textParam[4].ToString() + branchCode, 100, ' ', false, true);
                }
                gtf.SetGroupHeaderLine();

                string transferInOutNo = string.Empty;
                int intSubTotalUnit = 0, intTotalUnit = 0, intBranchTotalUnit = 0;
                decimal decSubTotalCOGSUnit = 0, decSubTotalPpnBmBuy = 0, decTotalCOGSUnit = 0, decTotalPpnBmBuy = 0;
                decimal decBranchTotalCOGSUnit = 0, decBranchTotalPpnBmBuy = 0;
                #endregion

                #region Print Detail

                #region Transfer In
                if (reportID == "OmRpInvRgs005")
                {
                    gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(4);
                    gtf.SetGroupHeader("TANGGAL", 14, ' ', true);
                    gtf.SetGroupHeader("NO. TRF. IN", 16, ' ', true);
                    gtf.SetGroupHeader("NO. TRF. OUT", 16, ' ', true);
                    gtf.SetGroupHeader("GUD", 6, ' ', true);
                    gtf.SetGroupHeader("KETERANGAN", 15, ' ', true);
                    gtf.SetGroupHeader("SEQ", 4, ' ', true, false, true);
                    gtf.SetGroupHeader("MODEL", 47, ' ', true);
                    gtf.SetGroupHeader("WARNA", 8, ' ', true);
                    gtf.SetGroupHeader("GUDANG TUJUAN", 16, ' ', true);
                    gtf.SetGroupHeader("CHS CD", 13, ' ', true);
                    gtf.SetGroupHeader("ENG CD", 10, ' ', true);
                    gtf.SetGroupHeader("CHS NO", 9, ' ', true);
                    gtf.SetGroupHeader("ENG NO", 9, ' ', true);
                    gtf.SetGroupHeader("COGS UNIT", 15, ' ', true, false, true);
                    gtf.SetGroupHeader("PPN BM BUY", 13, ' ', false, true, true);
                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            gtf.SetDataDetailSpace(133);
                            gtf.SetDataDetail("-", 100, '-', false, true);
                            gtf.SetDataDetailSpace(133);
                            gtf.SetDataDetail("SUB TOTAL : ", 59, ' ', true);
                            gtf.SetDataDetail(intSubTotalUnit.ToString() + " unit", 9, ' ', true, false, true);
                            gtf.SetDataDetail(decSubTotalCOGSUnit.ToString(), 16, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotalPpnBmBuy.ToString(), 13, ' ', false, true, true, true, "n0");
                            gtf.SetDataDetailSpace(133);
                            gtf.SetDataDetail("-", 100, '-', false, true);

                            if (!string.IsNullOrEmpty(textParam[4].ToString()))
                            {
                                gtf.SetDataReportLine();
                                gtf.SetDataDetailSpace(127);
                                gtf.SetDataDetail("TOTAL BRANCH " + branchCode + ":", 65, ' ', true);
                                gtf.SetDataDetail(intBranchTotalUnit.ToString() + " unit", 9, ' ', true, false, true);
                                gtf.SetDataDetail(decBranchTotalCOGSUnit.ToString(), 16, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decBranchTotalPpnBmBuy.ToString(), 13, ' ', false, true, true, true, "n0");
                                gtf.SetDataReportLine();
                            }

                            intTotalUnit += intSubTotalUnit;
                            decTotalCOGSUnit += decSubTotalCOGSUnit;
                            decTotalPpnBmBuy += decSubTotalPpnBmBuy;

                            gtf.PrintData(false);
                            break;
                        }

                        if (transferInOutNo == string.Empty)
                        {
                            transferInOutNo = dt.Rows[i]["TransferInNo"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                            gtf.SetDataDetailSpace(4);
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["TransferInDate"]).ToString("dd-MMM-yyyy"), 14, ' ', true);
                            gtf.SetDataDetail(transferInOutNo, 16, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["TransferOutNo"].ToString(), 16, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["WarehouseCodeFrom"].ToString(), 6, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(), 15, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["TransferInSeq"].ToString(), 4, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 47, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 8, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 16, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 13, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 10, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 9, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 9, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["COGSUnit"].ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["PpnBmBuyPaid"].ToString(), 13, ' ', false, true, true, true, "n0");
                        }
                        else
                        {
                            if (transferInOutNo != dt.Rows[i]["TransferInNo"].ToString())
                            {
                                gtf.SetDataDetailSpace(133);
                                gtf.SetDataDetail("-", 100, '-', false, true);
                                gtf.SetDataDetailSpace(133);
                                gtf.SetDataDetail("SUB TOTAL : ", 59, ' ', true);
                                gtf.SetDataDetail(intSubTotalUnit.ToString() + " unit", 9, ' ', true, false, true);
                                gtf.SetDataDetail(decSubTotalCOGSUnit.ToString(), 16, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotalPpnBmBuy.ToString(), 13, ' ', false, true, true, true, "n0");
                                gtf.SetDataDetailSpace(133);
                                gtf.SetDataDetail("-", 100, '-', false, true);

                                intTotalUnit += intSubTotalUnit;
                                decTotalCOGSUnit += decSubTotalCOGSUnit;
                                decTotalPpnBmBuy += decSubTotalPpnBmBuy;

                                intSubTotalUnit = 0;
                                decSubTotalCOGSUnit = decSubTotalPpnBmBuy = 0;

                                if (branchCode != dt.Rows[i]["BranchCode"].ToString() && branchCode != string.Empty)
                                {
                                    gtf.SetDataReportLine();
                                    gtf.SetDataDetailSpace(127);
                                    gtf.SetDataDetail("TOTAL BRANCH " + branchCode + ":", 65, ' ', true);
                                    gtf.SetDataDetail(intBranchTotalUnit.ToString() + " unit", 9, ' ', true, false, true);
                                    gtf.SetDataDetail(decBranchTotalCOGSUnit.ToString(), 16, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decBranchTotalPpnBmBuy.ToString(), 13, ' ', false, true, true, true, "n0");
                                    gtf.SetDataReportLine();

                                    gtf.PrintTotal(true, false, false);

                                    gtf.ReplaceGroupHdr(branchCode, dt.Rows[i]["BranchCode"].ToString());
                                    branchCode = dt.Rows[i]["BranchCode"].ToString();
                                    intBranchTotalUnit = 0;
                                    decBranchTotalCOGSUnit = decBranchTotalPpnBmBuy = 0;
                                    counterData = 0;
                                }

                                transferInOutNo = dt.Rows[i]["TransferInNo"].ToString();
                                counterData += 1;
                                gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                                gtf.SetDataDetailSpace(4);
                                gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["TransferInDate"]).ToString("dd-MMM-yyyy"), 14, ' ', true);
                                gtf.SetDataDetail(transferInOutNo, 16, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["TransferOutNo"].ToString(), 16, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["WarehouseCodeFrom"].ToString(), 6, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(), 15, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["TransferInSeq"].ToString(), 4, ' ', true, false, true);
                                gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 47, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 8, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 16, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 13, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 10, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 9, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 9, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["COGSUnit"].ToString(), 15, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(dt.Rows[i]["PpnBmBuyPaid"].ToString(), 13, ' ', false, true, true, true, "n0");
                            }
                            else
                            {
                                gtf.SetDataDetailSpace(80);
                                gtf.SetDataDetail(dt.Rows[i]["TransferInSeq"].ToString(), 4, ' ', true, false, true);
                                gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 47, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 8, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 16, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 13, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 10, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 9, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 9, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["COGSUnit"].ToString(), 15, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(dt.Rows[i]["PpnBmBuyPaid"].ToString(), 13, ' ', false, true, true, true, "n0");
                            }
                        }

                        intSubTotalUnit += 1;
                        decSubTotalCOGSUnit += dt.Rows[i]["COGSUnit"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["COGSUnit"]) : 0;
                        decSubTotalPpnBmBuy += dt.Rows[i]["PpnBmBuyPaid"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PpnBmBuyPaid"]) : 0;

                        intBranchTotalUnit += 1;
                        decBranchTotalCOGSUnit += dt.Rows[i]["COGSUnit"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["COGSUnit"]) : 0;
                        decBranchTotalPpnBmBuy += dt.Rows[i]["PpnBmBuyPaid"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PpnBmBuyPaid"]) : 0;

                        gtf.PrintData(false);
                    }

                    gtf.SetDataDetail("-", 233, '-', false, true);
                    gtf.SetDataDetail("GRAND TOTAL : ", 192, ' ', true);
                    gtf.SetDataDetail(intTotalUnit.ToString() + " unit", 9, ' ', true, false, true);
                    gtf.SetDataDetail(decTotalCOGSUnit.ToString(), 16, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(decTotalPpnBmBuy.ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.SetDataDetail("-", 233, '-', false, true);
                }
                #endregion

                #region Transfer Out
                else
                {
                    gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(4);
                    gtf.SetGroupHeader("TANGGAL", 14, ' ', true);
                    gtf.SetGroupHeader("NO. DOKUMEN", 16, ' ', true);
                    gtf.SetGroupHeader("GUD", 6, ' ', true);
                    gtf.SetGroupHeader("KETERANGAN", 15, ' ', true);
                    gtf.SetGroupHeader("SEQ", 4, ' ', true, false, true);
                    gtf.SetGroupHeader("MODEL", 37, ' ', true);
                    gtf.SetGroupHeader("WARNA", 8, ' ', true);
                    gtf.SetGroupHeader("NO. DO", 11, ' ', true);
                    gtf.SetGroupHeader("GUDANG TUJUAN", 16, ' ', true);
                    gtf.SetGroupHeader("CHS CD", 13, ' ', true);
                    gtf.SetGroupHeader("ENG CD", 10, ' ', true);
                    gtf.SetGroupHeader("CHS NO", 9, ' ', true);
                    gtf.SetGroupHeader("ENG NO", 9, ' ', true);
                    gtf.SetGroupHeader("COGS UNIT", 15, ' ', true, false, true);
                    gtf.SetGroupHeader("PPN BM BUY", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("TRF. IN", 14, ' ', false, true);
                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            gtf.SetDataDetailSpace(127);
                            gtf.SetDataDetail("-", 106, '-', false, true);
                            gtf.SetDataDetailSpace(127);
                            gtf.SetDataDetail("SUB TOTAL : ", 49, ' ', true);
                            gtf.SetDataDetail(intSubTotalUnit.ToString() + " unit", 9, ' ', true, false, true);
                            gtf.SetDataDetail(decSubTotalCOGSUnit.ToString(), 17, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotalPpnBmBuy.ToString(), 13, ' ', false, true, true, true, "n0");
                            gtf.SetDataDetailSpace(127);
                            gtf.SetDataDetail("-", 106, '-', false, true);

                            if (!string.IsNullOrEmpty(textParam[4].ToString()))
                            {
                                gtf.SetDataReportLine();
                                gtf.SetDataDetailSpace(127);
                                gtf.SetDataDetail("TOTAL BRANCH " + branchCode + ":", 49, ' ', true);
                                gtf.SetDataDetail(intBranchTotalUnit.ToString() + " unit", 9, ' ', true, false, true);
                                gtf.SetDataDetail(decBranchTotalCOGSUnit.ToString(), 17, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decBranchTotalPpnBmBuy.ToString(), 13, ' ', false, true, true, true, "n0");
                                gtf.SetDataReportLine();
                            }

                            intTotalUnit += intSubTotalUnit;
                            decTotalCOGSUnit += decSubTotalCOGSUnit;
                            decTotalPpnBmBuy += decSubTotalPpnBmBuy;

                            gtf.PrintData(false);
                            break;
                        }

                        if (transferInOutNo == string.Empty)
                        {
                            transferInOutNo = dt.Rows[i]["TransferOutNo"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                            gtf.SetDataDetailSpace(4);
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["TransferOutDate"]).ToString("dd-MMM-yyyy"), 14, ' ', true);
                            gtf.SetDataDetail(transferInOutNo, 16, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["WarehouseCodeFrom"].ToString(), 6, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(), 15, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["TransferOutSeq"].ToString(), 4, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 37, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 8, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["RefferenceDONo"].ToString(), 11, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 16, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 13, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 10, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 9, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 9, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["COGSUnit"].ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["PpnBmBuyPaid"].ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["pStatus"].ToString(), 14, ' ', false, true);
                        }
                        else
                        {
                            if (transferInOutNo != dt.Rows[i]["TransferOutNo"].ToString())
                            {
                                gtf.SetDataDetailSpace(127);
                                gtf.SetDataDetail("-", 106, '-', false, true);
                                gtf.SetDataDetailSpace(127);
                                gtf.SetDataDetail("SUB TOTAL : ", 49, ' ', true);
                                gtf.SetDataDetail(intSubTotalUnit.ToString() + " unit", 9, ' ', true, false, true);
                                gtf.SetDataDetail(decSubTotalCOGSUnit.ToString(), 17, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotalPpnBmBuy.ToString(), 13, ' ', false, true, true, true, "n0");
                                gtf.SetDataDetailSpace(127);
                                gtf.SetDataDetail("-", 106, '-', false, true);

                                intTotalUnit += intSubTotalUnit;
                                decTotalCOGSUnit += decSubTotalCOGSUnit;
                                decTotalPpnBmBuy += decSubTotalPpnBmBuy;

                                intSubTotalUnit = 0;
                                decSubTotalCOGSUnit = decSubTotalPpnBmBuy = 0;

                                if (branchCode != dt.Rows[i]["BranchCode"].ToString() && branchCode != string.Empty)
                                {
                                    gtf.SetDataReportLine();
                                    gtf.SetDataDetailSpace(127);
                                    gtf.SetDataDetail("TOTAL BRANCH " + branchCode + ":", 49, ' ', true);
                                    gtf.SetDataDetail(intBranchTotalUnit.ToString() + " unit", 9, ' ', true, false, true);
                                    gtf.SetDataDetail(decBranchTotalCOGSUnit.ToString(), 17, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decBranchTotalPpnBmBuy.ToString(), 13, ' ', false, true, true, true, "n0");
                                    gtf.SetDataReportLine();

                                    gtf.PrintTotal(true, false, false);

                                    gtf.ReplaceGroupHdr(branchCode, dt.Rows[i]["BranchCode"].ToString());
                                    branchCode = dt.Rows[i]["BranchCode"].ToString();
                                    intBranchTotalUnit = 0;
                                    decBranchTotalCOGSUnit = decBranchTotalPpnBmBuy = 0;
                                    counterData = 0;
                                }

                                transferInOutNo = dt.Rows[i]["TransferOutNo"].ToString();
                                counterData += 1;
                                gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                                gtf.SetDataDetailSpace(4);
                                gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["TransferOutDate"]).ToString("dd-MMM-yyyy"), 14, ' ', true);
                                gtf.SetDataDetail(transferInOutNo, 16, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["WarehouseCodeFrom"].ToString(), 6, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(), 15, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["TransferOutSeq"].ToString(), 4, ' ', true, false, true);
                                gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 37, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 8, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["RefferenceDONo"].ToString(), 11, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 16, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 13, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 10, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 9, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 9, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["COGSUnit"].ToString(), 15, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(dt.Rows[i]["PpnBmBuyPaid"].ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(dt.Rows[i]["pStatus"].ToString(), 14, ' ', false, true);
                            }
                            else
                            {
                                gtf.SetDataDetailSpace(63);
                                gtf.SetDataDetail(dt.Rows[i]["TransferOutSeq"].ToString(), 4, ' ', true, false, true);
                                gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 37, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 8, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["RefferenceDONo"].ToString(), 11, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 16, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 13, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 10, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 9, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 9, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["COGSUnit"].ToString(), 15, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(dt.Rows[i]["PpnBmBuyPaid"].ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(dt.Rows[i]["pStatus"].ToString(), 14, ' ', false, true);
                            }
                        }

                        intSubTotalUnit += 1;
                        decSubTotalCOGSUnit += Convert.ToDecimal(dt.Rows[i]["COGSUnit"]);
                        decSubTotalPpnBmBuy += Convert.ToDecimal(dt.Rows[i]["PpnBmBuyPaid"]);

                        intBranchTotalUnit += 1;
                        decBranchTotalCOGSUnit += Convert.ToDecimal(dt.Rows[i]["COGSUnit"]);
                        decBranchTotalPpnBmBuy += Convert.ToDecimal(dt.Rows[i]["PpnBmBuyPaid"]);

                        gtf.PrintData(false);
                    }

                    gtf.SetDataDetail("-", 233, '-', false, true);
                    gtf.SetDataDetail("GRAND TOTAL : ", 176, ' ', true);
                    gtf.SetDataDetail(intTotalUnit.ToString() + " unit", 9, ' ', true, false, true);
                    gtf.SetDataDetail(decTotalCOGSUnit.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(decTotalPpnBmBuy.ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.SetDataDetail("-", 233, '-', false, true);
                }
                #endregion

                #endregion

                #region Print Footer
                gtf.SetDataDetail("PERINCIAN", 223, ' ', false, true);
                gtf.SetDataDetail("-", 83, '-', false, true);
                gtf.SetDataDetail("NO.", 4, ' ', true);
                gtf.SetDataDetail("MODEL", 40, ' ', true);
                gtf.SetDataDetail("UNIT", 4, ' ', true);
                gtf.SetDataDetail("NILAI COST", 15, ' ', true, false, true);
                gtf.SetDataDetail("PPN BM", 16, ' ', false, true, true);
                gtf.SetDataDetail("-", 83, '-', false, true);

                counterData = 0;
                decimal decTotalUnit = 0;
                decTotalCOGSUnit = decTotalPpnBmBuy = 0;
                for (int i = 0; i < dt.DataSet.Tables[1].Rows.Count; i++)
                {
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["SalesModelDescSum"].ToString(), 40, ' ', true);
                    gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["Unit"].ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["NilaiCost"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["PPNBM"].ToString(), 16, ' ', false, true, true, true, "n0");

                    decTotalUnit += Convert.ToDecimal(dt.DataSet.Tables[1].Rows[i]["Unit"]);
                    decTotalCOGSUnit += dt.DataSet.Tables[1].Rows[i]["NilaiCost"].ToString() != "" ? Convert.ToDecimal(dt.DataSet.Tables[1].Rows[i]["NilaiCost"]) : 0;
                    decTotalPpnBmBuy += dt.DataSet.Tables[1].Rows[i]["PPNBM"].ToString() != "" ? Convert.ToDecimal(dt.DataSet.Tables[1].Rows[i]["PPNBM"]) : 0;

                    gtf.PrintData(false);
                }
                gtf.SetDataDetail("-", 83, '-', false, true);
                gtf.SetDataDetail("GRAND TOTAL : ", 45, ' ', true);
                gtf.SetDataDetail(intTotalUnit.ToString(), 4, ' ', true, false, true);
                gtf.SetDataDetail(decTotalCOGSUnit.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(decTotalPpnBmBuy.ToString(), 16, ' ', false, true, true, true, "n0");
                gtf.SetDataDetail("-", 83, '-', false, true);

                gtf.PrintData(false);

                gtf.SetTotalDetailLine();
                #endregion
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}