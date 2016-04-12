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
    public class txtOmRpSalRgs002 : IRptProc
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
            return CreateReportOmRpSalRgs002(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs002(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Register Delivery Order (OmRpSalRgs002/OmRpSalRgs002HQ)
            if (reportID == "OmRpSalRgs002" || reportID == "OmRpSalRgs002HQ")
            {
                #region Print Group Header
                if (textParam != null) gtf.SetGroupHeader(textParam[0].ToString(), 136, ' ', false, true, false, true);
                gtf.SetGroupHeader("SALES TYPE :" + dt.Rows[0]["pSALESTYPE"].ToString(), 60, ' ', true);
                gtf.SetGroupHeader("SALES MODEL :" + dt.Rows[0]["pMODEL"].ToString(), 74, ' ', false, true);
                gtf.SetGroupHeader("CUSTOMER   :" + dt.Rows[0]["pCUST"].ToString(), 60, ' ', true);
                gtf.SetGroupHeader("NO DO       :" + dt.Rows[0]["pDO"].ToString(), 74, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("TANGGAL", 12, ' ', true);
                gtf.SetGroupHeader("NO. DOKUMEN", 16, ' ', true);
                gtf.SetGroupHeader("EXPIRED", 15, ' ', true);
                gtf.SetGroupHeader("PELANGGAN", 43, ' ', true);
                gtf.SetGroupHeader("SALES ORDER", 14, ' ', true);
                gtf.SetGroupHeader("NO. INVOICE", 14, ' ', true);
                gtf.SetGroupHeader("TGL. INV", 11, ' ', false, true);
                gtf.SetGroupHeaderSpace(12);
                gtf.SetGroupHeader("SEQ", 3, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader("MODEL", 16, ' ', true);
                gtf.SetGroupHeader("YEAR", 4, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader("WARNA", 6, ' ', true);
                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader("RANGKA", 35, ' ', true);
                gtf.SetGroupHeader("MESIN", 7, ' ', true);
                gtf.SetGroupHeader("WHS", 14, ' ', true);
                gtf.SetGroupHeader("SURAT JALAN", 14, ' ', true);
                gtf.SetGroupHeader("TGL. SJ", 11, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                string branch = string.Empty;
                decimal totUnit = 0, totSubUnit = 0;
                #endregion

                #region Print Detail
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        if (reportID == "OmRpSalRgs002HQ")
                        {
                            gtf.SetDataDetailSpace(35);
                            gtf.SetDataDetail("-", 101, '-', false, true);
                            gtf.SetDataDetailSpace(35);
                            gtf.SetDataDetail("SUB TOTAL " + dt.Rows[i - 1]["BranchCode"].ToString() + " : " + totSubUnit.ToString() + " UNIT", 101, ' ', false, true);
                            gtf.SetDataDetailSpace(35);
                            gtf.SetDataDetail("-", 101, '-', false, true);

                            totUnit += totSubUnit;
                        }
                        break;
                    }

                    if (reportID == "OmRpSalRgs002HQ")
                    {
                        if (branch == string.Empty)
                        {
                            branch = dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString();
                            gtf.SetDataDetail("Branch : " + branch, 136, ' ', false, true);
                        }
                        else
                        {
                            if (branch != dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString())
                            {
                                gtf.SetDataDetailSpace(35);
                                gtf.SetDataDetail("-", 101, '-', false, true);
                                gtf.SetDataDetailSpace(35);
                                gtf.SetDataDetail("SUB TOTAL " + dt.Rows[i]["BranchCode"].ToString() + " : " + totSubUnit.ToString() + " UNIT", 101, ' ', false, true);
                                gtf.SetDataDetailSpace(35);
                                gtf.SetDataDetail("-", 101, '-', false, true);
                                gtf.SetDataDetail("", 136, ' ', false, true);
                                branch = dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString();
                                gtf.SetDataDetail("Branch : " + branch, 136, ' ', false, true);

                                totUnit += totSubUnit;
                                totSubUnit = 0;
                            }
                        }
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DODate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DONo"].ToString(), 16, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Expired"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["pCustomer"].ToString(), 43, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SONo"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"]).ToString("dd-MMM-yyyy"), 11, ' ', false, true);
                    gtf.SetDataDetailSpace(12);
                    gtf.SetDataDetail(dt.Rows[i]["DOSeq"].ToString(), 3, ' ', true, false, true);
                    gtf.SetDataDetailSpace(2);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 16, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetailSpace(2);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 6, ' ', true);
                    gtf.SetDataDetailSpace(2);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 35, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BPKNo"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPKDate"]).ToString("dd-MMM-yyyy"), 11, ' ', false, true);

                    if (reportID == "OmRpSalRgs002HQ")
                    {
                        totSubUnit += 1;
                        gtf.PrintData(false);
                    }
                    else
                        gtf.PrintData(true);
                }
                #endregion

                #region Print Footer
                if (reportID == "OmRpSalRgs002HQ")
                {
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetailSpace(18);
                    gtf.SetTotalDetail("GRAND TOTAL", 16, ' ', true);
                    gtf.SetTotalDetailSpace(20);
                    gtf.SetTotalDetail(": " + totUnit.ToString() + " UNIT", 65, ' ', false, true);
                }
                gtf.SetTotalDetailLine();
                #endregion
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}