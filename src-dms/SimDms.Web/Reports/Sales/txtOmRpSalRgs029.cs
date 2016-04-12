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
    public class txtOmRpSalRgs029 : IRptProc
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
            return CreateReportOmRpSalRgs029(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs029(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Daftar BPKB (OmRpSalRgs029)
            if (reportID == "OmRpSalRgs029")
            {
                gtf.SetGroupHeader(textParam[0].ToString(), 96, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeaderSpace(5);
                gtf.SetGroupHeader("NAMA KONSUMEN", 25, ' ', true);
                gtf.SetGroupHeaderSpace(21);
                gtf.SetGroupHeader("TANGGAL DITERIMA", 23, ' ', false, true, false, true);

                gtf.SetGroupHeaderSpace(5);
                gtf.SetGroupHeader("-", 25, '-', true);
                gtf.SetGroupHeaderSpace(21);
                gtf.SetGroupHeader("-", 23, '-', false, true);

                gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
                gtf.SetGroupHeader("TGL. JUAL", 11, ' ', true);
                gtf.SetGroupHeader("T/C", 3, ' ', true);
                gtf.SetGroupHeader("JENIS", 9, ' ', true);
                gtf.SetGroupHeader("THN", 5, ' ', true, false, true);
                gtf.SetGroupHeader("#RANGKA", 7, ' ', true);
                gtf.SetGroupHeader("#MESIN", 6, ' ', true);
                gtf.SetGroupHeader("PUSAT", 11, ' ', true);
                gtf.SetGroupHeader("CABANG", 11, ' ', true);
                gtf.SetGroupHeader("#BPKB", 8, ' ', true);
                gtf.SetGroupHeader("#POLISI", 10, ' ', false, true);
                gtf.SetGroupHeaderLine();

                gtf.PrintHeader();

                string branch = string.Empty;

                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        break;
                    }

                    if (branch == string.Empty)
                    {
                        branch = "CABANG : " + dt.Rows[i]["RefferenceDesc1"].ToString();
                        gtf.SetDataDetail(branch, 96, ' ', false, true);
                    }
                    else
                    {
                        if (branch != "CABANG : " + dt.Rows[i]["RefferenceDesc1"].ToString())
                        {
                            gtf.SetDataDetailLineBreak();
                            branch = "CABANG : " + dt.Rows[i]["RefferenceDesc1"].ToString();
                            gtf.SetDataDetail(branch, 96, ' ', false, true);
                        }
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail("(" + dt.Rows[i]["CustomerCode"].ToString() + ") " + dt.Rows[i]["CustomerName"].ToString(), 91, ' ', false, true);

                    gtf.SetDataDetailSpace(5);
                    if (Convert.ToDateTime(dt.Rows[i]["InvoiceDate"]).ToShortDateString() != "1/1/1900")
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    else
                        gtf.SetDataDetail("", 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["StatusBPKB"].ToString(), 3, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 5, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 6, ' ', true);
                    if (Convert.ToDateTime(dt.Rows[i]["BPKBInDate"]).ToShortDateString() != "1/1/1900")
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPKBInDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    else
                        gtf.SetDataDetail("", 11, ' ', true);
                    if (Convert.ToDateTime(dt.Rows[i]["DocDate"]).ToShortDateString() != "1/1/1900")
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DocDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    else
                        gtf.SetDataDetail("", 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BPKBNo"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiNo"].ToString(), 10, ' ', false, true);

                    gtf.PrintData(false);
                }

                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}