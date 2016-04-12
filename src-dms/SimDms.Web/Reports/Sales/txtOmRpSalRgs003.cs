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
    public class txtOmRpSalRgs003 : IRptProc 
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
            return CreateReportOmRpSalRgs003(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs003(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Register Bukti Pengiriman Kendaraan (OmRpSalesRgs003/OmRpSalesRgs003A)
            if (reportID == "OmRpSalesRgs003" || reportID == "OmRpSalesRgs003A")
            {
                #region Print Group Header
                if (textParam != null) gtf.SetGroupHeader(textParam[0].ToString(), 163, ' ', false, true, false, true);
                gtf.SetGroupHeader("SALES TYPE :" + dt.Rows[0]["pSALESTYPE"].ToString(), 60, ' ', true);
                gtf.SetGroupHeader("SALES MODEL :" + dt.Rows[0]["pMODEL"].ToString(), 102, ' ', false, true);
                gtf.SetGroupHeader("CUSTOMER   :" + dt.Rows[0]["pCUST"].ToString(), 60, ' ', true);
                gtf.SetGroupHeader("NO BPK      :" + dt.Rows[0]["pBPK"].ToString(), 102, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("BPK", 25, ' ', true, false, false, true);
                gtf.SetGroupHeader("DO", 25, ' ', true, false, false, true);
                gtf.SetGroupHeader("SO", 25, ' ', false, true, false, true);
                gtf.SetGroupHeader("-", 25, '-', true);
                gtf.SetGroupHeader("-", 25, '-', true);
                gtf.SetGroupHeader("-", 25, '-', true);
                gtf.SetGroupHeader("BPK", 3, ' ', true, false, true);
                gtf.SetGroupHeader("DO", 3, ' ', false, true, true);
                gtf.SetGroupHeader("NO. DOKUMEN", 13, ' ', true);
                gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
                gtf.SetGroupHeader("NO. DOKUMEN", 13, ' ', true);
                gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
                gtf.SetGroupHeader("NO. DOKUMEN", 13, ' ', true);
                gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
                gtf.SetGroupHeader("SEQ", 3, ' ', true, false, true);
                gtf.SetGroupHeader("SEQ", 3, ' ', true, false, true);
                gtf.SetGroupHeader("PELANGGAN", 37, ' ', true);
                gtf.SetGroupHeader("WH", 2, ' ', true);
                gtf.SetGroupHeader("MODEL", 15, ' ', true);
                gtf.SetGroupHeader("WARNA", 5, ' ', true);
                gtf.SetGroupHeader("CHASSIS", 7, ' ', true);
                gtf.SetGroupHeader("MESIN", 6, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                string branch = string.Empty;
                #endregion

                #region Print Detail
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count) break;

                    if (reportID == "OmRpSalesRgs003A")
                    {
                        if (branch == string.Empty)
                        {
                            branch = dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString();
                            gtf.SetDataDetail("Branch : " + branch, 163, ' ', false, true);
                        }
                        else
                        {
                            if (branch != dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString())
                            {
                                gtf.SetDataDetail("", 163, ' ', false, true);
                                branch = dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString();
                                gtf.SetDataDetail("Branch : " + branch, 163, ' ', false, true);
                            }
                        }
                    }
                    gtf.SetDataDetail(dt.Rows[i]["BPKNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPKDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DONo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DODate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SONo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["SODate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    if (dt.Rows[i]["BPKNo"].ToString() != string.Empty)
                        gtf.SetDataDetail("1", 3, ' ', true, false, true);
                    else
                        gtf.SetDataDetail("0", 3, ' ', true, false, true);

                    if (dt.Rows[i]["DONo"].ToString() != string.Empty)
                        gtf.SetDataDetail("1", 3, ' ', true, false, true);
                    else
                        gtf.SetDataDetail("0", 3, ' ', true, false, true);

                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 26, ' ', true);
                    gtf.SetDataDetail("[" + dt.Rows[i]["CustomerCode"].ToString() + "]", 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 5, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 6, ' ', false, true);

                    if (reportID == "OmRpSalesRgs003") gtf.PrintData(true);
                    else gtf.PrintData(false);
                }
                #endregion

                #region Print Footer
                gtf.SetDataReportLine();
                gtf.SetDataDetail("", 163, ' ', false, true);
                gtf.SetDataDetail("SUMMARY :", 163, ' ', false, true);
                gtf.SetDataDetail("-", 63, '-', false, true);
                gtf.SetDataDetail("NO.", 3, ' ', true, false, true);
                gtf.SetDataDetailSpace(1);
                gtf.SetDataDetail("MODEL", 16, ' ', true);
                gtf.SetDataDetail("MODEL DESC", 21, ' ', true);
                gtf.SetDataDetail("WARNA", 9, ' ', true);
                gtf.SetDataDetail("QUANTITY", 9, ' ', false, true, true);
                gtf.SetDataDetail("-", 63, '-', false, true);
                gtf.PrintData(false);

                decimal decSubUnit = 0, decTotUnit = 0;

                string salesModelCode = string.Empty;
                for (int i = 0; i <= dt.DataSet.Tables[1].Rows.Count; i++)
                {
                    if (i == dt.DataSet.Tables[1].Rows.Count)
                    {
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("-", 58, '-', false, true);
                        gtf.SetDataDetailSpace(32);
                        gtf.SetDataDetail("Sub Total per Model :", 21, ' ', true);
                        gtf.SetDataDetail(decSubUnit.ToString() + " Unit", 9, ' ', false, true, true);

                        gtf.PrintData(false);
                        decTotUnit += decSubUnit;
                        break;
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
                            gtf.SetDataDetail("-", 58, '-', false, true);
                            gtf.SetDataDetailSpace(32);
                            gtf.SetDataDetail("Sub Total per Model :", 21, ' ', true);
                            gtf.SetDataDetail(decSubUnit.ToString() + " Unit", 9, ' ', false, true, true);

                            gtf.PrintData(false);

                            decTotUnit += decSubUnit;
                            decSubUnit = 0;

                            salesModelCode = dt.DataSet.Tables[1].Rows[i]["SalesModelCode"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                        }
                    }

                    if (decSubUnit == 0)
                    {
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["SalesModelCode"].ToString(), 16, ' ', true);
                        gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["SalesModelDesc"].ToString(), 21, ' ', true);
                    }
                    else
                        gtf.SetDataDetailSpace(44);
                    gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["ColourCode"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.DataSet.Tables[1].Rows[i]["Unit"].ToString() + " Unit", 9, ' ', false, true, true);

                    gtf.PrintData(false);

                    decSubUnit += Convert.ToDecimal(dt.DataSet.Tables[1].Rows[i]["Unit"]);
                }

                gtf.SetDataDetail("-", 63, '-', false, true);
                gtf.SetDataDetail("GRAND TOTAL :", 53, ' ', true);
                gtf.SetDataDetail(decTotUnit.ToString() + " Unit", 9, ' ', false, true, true);
                gtf.SetDataDetail("-", 63, '-', false, true);

                gtf.PrintData(false);

                gtf.SetTotalDetailLine();
                #endregion
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}