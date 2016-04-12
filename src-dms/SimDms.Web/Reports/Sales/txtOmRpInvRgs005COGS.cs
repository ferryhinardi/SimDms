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
    public class txtOmRpInvRgs005COGS : IRptProc  
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
            return CreateReportOmRpInvRgs005COGS(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpInvRgs005COGS(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Register (Transfer In / Out) COGS Gudang (OmRpInvRgs005COGS/OmRpInvRgs006COGS)
            if (reportID == "OmRpInvRgs005COGS" || reportID == "OmRpInvRgs006COGS")
            {
                #region Print Group Header
                gtf.SetGroupHeader("STATUS : " + textParam[0].ToString(), 233, ' ', false, true, false, true);
                gtf.SetGroupHeader("PERIODE : " + Convert.ToDateTime(textParam[1]).ToString("dd-MMM-yyyy") + " S/D " + Convert.ToDateTime(textParam[2]).ToString("dd-MMM-yyyy"), 233, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();

                string transferInOutNo = string.Empty;
                int intSubTotalUnit = 0, intTotalUnit = 0;
                decimal decSubTotalCOGSUnit = 0, decSubTotalPpnBmBuy = 0, decTotalCOGSUnit = 0, decTotalPpnBmBuy = 0;
                #endregion

                #region Print Detail

                #region Transfer In
                if (reportID == "OmRpInvRgs005COGS")
                {
                    gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(4);
                    gtf.SetGroupHeader("TANGGAL", 15, ' ', true);
                    gtf.SetGroupHeader("NO. TRF. IN", 17, ' ', true);
                    gtf.SetGroupHeader("NO. TRF. OUT", 17, ' ', true);
                    gtf.SetGroupHeader("GUD", 7, ' ', true);
                    gtf.SetGroupHeader("KETERANGAN", 20, ' ', true);
                    gtf.SetGroupHeader("SEQ", 4, ' ', true, false, true);
                    gtf.SetGroupHeader("MODEL", 48, ' ', true);
                    gtf.SetGroupHeader("WARNA", 9, ' ', true);
                    gtf.SetGroupHeader("GUDANG TUJUAN", 30, ' ', true);
                    gtf.SetGroupHeader("CHS CD", 14, ' ', true);
                    gtf.SetGroupHeader("ENG CD", 11, ' ', true);
                    gtf.SetGroupHeader("CHS NO", 10, ' ', true);
                    gtf.SetGroupHeader("ENG NO", 10, ' ', false, true);
                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            gtf.SetDataDetailSpace(144);
                            gtf.SetDataDetail("-", 89, '-', false, true);
                            gtf.SetDataDetailSpace(144);
                            gtf.SetDataDetail("SUB TOTAL : ", 59, ' ', true);
                            gtf.SetDataDetail(intSubTotalUnit.ToString() + " unit", 9, ' ', false, true);
                            gtf.SetDataDetailSpace(144);
                            gtf.SetDataDetail("-", 89, '-', false, true);

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
                            gtf.SetDataDetail(counterData.ToString(), 4, '0', true, false, true);
                            gtf.SetDataDetailSpace(4);
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["TransferInDate"]).ToString("dd-MMM-yyyy"), 15, ' ', true);
                            gtf.SetDataDetail(transferInOutNo, 17, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["TransferOutNo"].ToString(), 17, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["WarehouseCodeFrom"].ToString(), 7, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(), 20, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["TransferInSeq"].ToString(), 4, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 48, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 9, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 30, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 14, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 11, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 10, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 10, ' ', false, true);
                        }
                        else
                        {
                            if (transferInOutNo != dt.Rows[i]["TransferInNo"].ToString())
                            {
                                gtf.SetDataDetailSpace(144);
                                gtf.SetDataDetail("-", 89, '-', false, true);
                                gtf.SetDataDetailSpace(144);
                                gtf.SetDataDetail("SUB TOTAL : ", 59, ' ', true);
                                gtf.SetDataDetail(intSubTotalUnit.ToString() + " unit", 9, ' ', false, true);
                                gtf.SetDataDetailSpace(144);
                                gtf.SetDataDetail("-", 89, '-', false, true);

                                intTotalUnit += intSubTotalUnit;

                                intSubTotalUnit = 0;
                                decSubTotalCOGSUnit = decSubTotalPpnBmBuy = 0;

                                transferInOutNo = dt.Rows[i]["TransferInNo"].ToString();
                                counterData += 1;
                                gtf.SetDataDetail(counterData.ToString(), 4, '0', true, false, true);
                                gtf.SetDataDetailSpace(4);
                                gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["TransferInDate"]).ToString("dd-MMM-yyyy"), 15, ' ', true);
                                gtf.SetDataDetail(transferInOutNo, 17, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["TransferOutNo"].ToString(), 17, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["WarehouseCodeFrom"].ToString(), 7, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(), 20, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["TransferInSeq"].ToString(), 4, ' ', true, false, true);
                                gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 48, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 9, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 30, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 14, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 11, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 10, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 10, ' ', false, true);
                            }
                            else
                            {
                                gtf.SetDataDetailSpace(90);
                                gtf.SetDataDetail(dt.Rows[i]["TransferInSeq"].ToString(), 4, ' ', true, false, true);
                                gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 48, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 9, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 30, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 14, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 11, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 10, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 10, ' ', false, true);
                            }
                        }

                        intSubTotalUnit += 1;

                        gtf.PrintData(false);
                    }

                    gtf.SetDataDetail("-", 233, '-', false, true);
                    gtf.SetDataDetail("GRAND TOTAL : ", 203, ' ', true);
                    gtf.SetDataDetail(intTotalUnit.ToString() + " unit", 9, ' ', false, true);
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
                    gtf.SetGroupHeader("KETERANGAN", 20, ' ', true);
                    gtf.SetGroupHeader("SEQ", 4, ' ', true, false, true);
                    gtf.SetGroupHeader("MODEL", 37, ' ', true);
                    gtf.SetGroupHeader("WARNA", 8, ' ', true);
                    gtf.SetGroupHeader("NO. DO", 11, ' ', true);
                    gtf.SetGroupHeader("GUDANG TUJUAN", 25, ' ', true);
                    gtf.SetGroupHeader("CHASSIS CODE", 14, ' ', true);
                    gtf.SetGroupHeader("ENGINE CODE", 14, ' ', true);
                    gtf.SetGroupHeader("CHASSIS NO", 14, ' ', true);
                    gtf.SetGroupHeader("ENGINE NO", 14, ' ', true);
                    gtf.SetGroupHeader("TRF. IN", 14, ' ', false, true);
                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            gtf.SetDataDetailSpace(132);
                            gtf.SetDataDetail("-", 100, '-', false, true);
                            gtf.SetDataDetailSpace(132);
                            gtf.SetDataDetail("SUB TOTAL : ", 49, ' ', true);
                            gtf.SetDataDetail(intSubTotalUnit.ToString() + " unit", 9, ' ', false, true, true);
                            gtf.SetDataDetailSpace(132);
                            gtf.SetDataDetail("-", 100, '-', false, true);

                            intTotalUnit += intSubTotalUnit;

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
                            gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(), 20, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["TransferOutSeq"].ToString(), 4, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 37, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 8, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["RefferenceDONo"].ToString(), 11, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 25, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 14, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 14, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 14, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 14, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["pStatus"].ToString(), 14, ' ', false, true);
                        }
                        else
                        {
                            if (transferInOutNo != dt.Rows[i]["TransferOutNo"].ToString())
                            {
                                gtf.SetDataDetailSpace(132);
                                gtf.SetDataDetail("-", 100, '-', false, true);
                                gtf.SetDataDetailSpace(132);
                                gtf.SetDataDetail("SUB TOTAL : ", 49, ' ', true);
                                gtf.SetDataDetail(intSubTotalUnit.ToString() + " unit", 9, ' ', false, true, true);
                                gtf.SetDataDetailSpace(132);
                                gtf.SetDataDetail("-", 100, '-', false, true);

                                intTotalUnit += intSubTotalUnit;

                                intSubTotalUnit = 0;

                                transferInOutNo = dt.Rows[i]["TransferOutNo"].ToString();
                                counterData += 1;
                                gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                                gtf.SetDataDetailSpace(4);
                                gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["TransferOutDate"]).ToString("dd-MMM-yyyy"), 14, ' ', true);
                                gtf.SetDataDetail(transferInOutNo, 16, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["WarehouseCodeFrom"].ToString(), 6, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(), 20, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["TransferOutSeq"].ToString(), 4, ' ', true, false, true);
                                gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 37, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 8, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["RefferenceDONo"].ToString(), 11, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 25, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 14, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 14, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 14, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 14, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["pStatus"].ToString(), 14, ' ', false, true);
                            }
                            else
                            {
                                gtf.SetDataDetailSpace(68);
                                gtf.SetDataDetail(dt.Rows[i]["TransferOutSeq"].ToString(), 4, ' ', true, false, true);
                                gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 37, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 8, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["RefferenceDONo"].ToString(), 11, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 25, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 14, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 14, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 14, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 14, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["pStatus"].ToString(), 14, ' ', false, true);
                            }
                        }

                        intSubTotalUnit += 1;

                        gtf.PrintData(false);
                    }

                    gtf.SetDataDetail("-", 233, '-', false, true);
                    gtf.SetDataDetail("GRAND TOTAL : ", 181, ' ', true);
                    gtf.SetDataDetail(intTotalUnit.ToString() + " unit", 9, ' ', false, true, true);
                    gtf.SetDataDetail("-", 233, '-', false, true);
                }
                #endregion

                #endregion

                #region Print Footer
                gtf.SetDataDetail("PERINCIAN", 223, ' ', false, true);
                gtf.SetDataDetail("-", 50, '-', false, true);
                gtf.SetDataDetail("NO.", 4, ' ', true);
                gtf.SetDataDetail("MODEL", 40, ' ', true);
                gtf.SetDataDetail("UNIT", 4, ' ', false, true);
                gtf.SetDataDetail("-", 50, '-', false, true);

                counterData = 0;
                decimal decTotalUnit = 0;
                decTotalCOGSUnit = decTotalPpnBmBuy = 0;
                for (int i = 0; i < dt.DataSet.Tables[1].Rows.Count; i++)
                {
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["SalesModelDescSum"].ToString(), 40, ' ', true);
                    gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["Unit"].ToString(), 4, ' ', false, true, true);

                    decTotalUnit += Convert.ToDecimal(dt.DataSet.Tables[1].Rows[i]["Unit"]);

                    gtf.PrintData(false);
                }
                gtf.SetDataDetail("-", 50, '-', false, true);
                gtf.SetDataDetail("GRAND TOTAL : ", 45, ' ', true);
                gtf.SetDataDetail(intTotalUnit.ToString(), 4, ' ', false, true, true);
                gtf.SetDataDetail("-", 50, '-', false, true);

                gtf.PrintData(false);

                gtf.SetTotalDetailLine();
                #endregion
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}