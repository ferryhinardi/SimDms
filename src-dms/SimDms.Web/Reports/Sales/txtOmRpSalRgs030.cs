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
    public class txtOmRpSalRgs030 : IRptProc
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
            return CreateReportOmRpSalRgs030(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs030(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Daftar BPKB per Lokasi (OmRpSalRgs030A/OmRpSalRgs030B)
            if (reportID == "OmRpSalRgs030A" || reportID == "OmRpSalRgs030B")
            {
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader("PELANGGAN", 43, ' ', true);
                gtf.SetGroupHeader("NO FPS", 14, ' ', true);
                gtf.SetGroupHeader("TGL. FPS", 12, ' ', true);
                gtf.SetGroupHeader("JENIS", 16, ' ', true);
                gtf.SetGroupHeader("TAHUN", 7, ' ', true, false, true);
                gtf.SetGroupHeader("NO RANGKA", 11, ' ', true);
                gtf.SetGroupHeader("NO MESIN", 10, ' ', true);
                gtf.SetGroupHeader("DITERIMA PUSAT", 15, ' ', true);
                gtf.SetGroupHeader("NO BPKB", 10, ' ', true);
                gtf.SetGroupHeader("NO POLISI", 10, ' ', false, true);
                gtf.SetGroupHeaderLine();

                gtf.PrintHeader();

                string branchName = string.Empty;

                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        gtf.SetDataDetail("Total BPKB : " + counterData.ToString(), 163, ' ', false, true);

                        gtf.PrintData(false);
                        break;
                    }

                    if (branchName == string.Empty)
                    {
                        branchName = "Cabang : " + dt.Rows[i]["BranchName"].ToString();
                        gtf.SetDataDetail(branchName, 163, ' ', false, true);
                    }
                    else
                    {
                        if (branchName != "Cabang : " + dt.Rows[i]["BranchName"].ToString())
                        {
                            gtf.SetDataDetail("Total BPKB : " + counterData.ToString(), 163, ' ', false, true);
                            gtf.SetDataDetailLineBreak();
                            counterData = 0;
                            branchName = "Cabang : " + dt.Rows[i]["BranchName"].ToString();
                            gtf.SetDataDetail(branchName, 163, ' ', false, true);
                        }
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(2);
                    gtf.SetDataDetail("(" + dt.Rows[i]["CustomerCode"].ToString() + ") " + dt.Rows[i]["CustomerName"].ToString(), 43, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReqOutNo"].ToString(), 14, ' ', true);
                    if (Convert.ToDateTime(dt.Rows[i]["ReqDate"]).ToShortDateString() != "1/1/1900")
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["ReqDate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                    else
                        gtf.SetDataDetail("", 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 16, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 7, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 10, ' ', true);
                    if (Convert.ToDateTime(dt.Rows[i]["BPKBInDate"]).ToShortDateString() != "1/1/1900")
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPKBInDate"]).ToString("dd-MMM-yyyy"), 15, ' ', true);
                    else
                        gtf.SetDataDetail("", 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BPKBNo"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PoliceRegistrationNo"].ToString(), 10, ' ', false, true);

                    if (reportID == "OmRpSalRgs030A") gtf.PrintData(false);
                    else gtf.PrintData(true);
                }

                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}