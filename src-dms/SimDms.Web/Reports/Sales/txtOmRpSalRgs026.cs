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
    public class txtOmRpSalRgs026 : IRptProc
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
            return CreateReportOmRpSalRgs026(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs026(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Tanda Terima BPKB (OmRpSalRgs028)
            if (reportID == "OmRpSalRgs028")
            {
                gtf.SetGroupHeader("TEMPAT PENYIMPANAN : " + dt.Rows[0]["NameStorage"].ToString(), 96, ' ', false, true, false, true);

                gtf.SetGroupHeader("NO : " + dt.Rows[0]["DocNo"].ToString(), 96, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeader("NAMA KONSUMEN", 25, ' ', true);
                gtf.SetGroupHeader("NO BPKB", 8, ' ', true);
                gtf.SetGroupHeader("NO POLISI", 10, ' ', true);
                gtf.SetGroupHeader("JENIS", 9, ' ', true);
                gtf.SetGroupHeader("WARNA", 17, ' ', true);
                gtf.SetGroupHeader("NO RANGKA", 9, ' ', true);
                gtf.SetGroupHeader("NO MESIN", 8, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                string docNo = string.Empty;

                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        gtf.SetDataReportLine();

                        gtf.SetTotalDetailSpace(6);
                        gtf.SetTotalDetail("Diterima oleh :", 15, ' ', true);
                        gtf.SetTotalDetailSpace(7);
                        gtf.SetTotalDetail("Diserahkan oleh :", 17, ' ', true);
                        gtf.SetTotalDetailSpace(19);
                        gtf.SetTotalDetail("Mengetahui,", 11, ' ', false, true);
                        gtf.SetTotalDetail("", 96, ' ', false, true);
                        gtf.SetTotalDetail("", 96, ' ', false, true);
                        gtf.SetTotalDetail("", 96, ' ', false, true);
                        gtf.SetTotalDetail("", 96, ' ', false, true);
                        gtf.SetTotalDetail("", 96, ' ', false, true);
                        gtf.SetTotalDetailSpace(6);
                        gtf.SetTotalDetail("(             )", 15, ' ', true);
                        gtf.SetTotalDetailSpace(7);
                        gtf.SetTotalDetail("(               )", 17, ' ', true);
                        gtf.SetTotalDetailSpace(6);
                        gtf.SetTotalDetail("(               )", 17, ' ', true);
                        gtf.SetTotalDetailSpace(1);
                        gtf.SetTotalDetail("(               )", 17, ' ', false, true);

                        break;
                    }

                    if (docNo == string.Empty) docNo = dt.Rows[i]["DocNo"].ToString();
                    else
                    {
                        if (docNo != dt.Rows[i]["DocNo"].ToString())
                        {
                            gtf.SetDataReportLine();

                            gtf.CheckLastLineforTotal();

                            gtf.SetTotalDetailSpace(6);
                            gtf.SetTotalDetail("Diterima oleh : ", 15, ' ', true);
                            gtf.SetTotalDetailSpace(7);
                            gtf.SetTotalDetail("Diserahkan oleh : ", 17, ' ', true);
                            gtf.SetTotalDetailSpace(19);
                            gtf.SetTotalDetail("Mengetahui,", 11, ' ', false, true);
                            gtf.SetTotalDetail("", 96, ' ', false, true);
                            gtf.SetTotalDetail("", 96, ' ', false, true);
                            gtf.SetTotalDetail("", 96, ' ', false, true);
                            gtf.SetTotalDetail("", 96, ' ', false, true);
                            gtf.SetTotalDetail("", 96, ' ', false, true);
                            gtf.SetTotalDetailSpace(6);
                            gtf.SetTotalDetail("(             )", 15, ' ', true);
                            gtf.SetTotalDetailSpace(7);
                            gtf.SetTotalDetail("(               )", 17, ' ', true);
                            gtf.SetTotalDetailSpace(6);
                            gtf.SetTotalDetail("(               )", 17, ' ', true);
                            gtf.SetTotalDetailSpace(1);
                            gtf.SetTotalDetail("(               )", 17, ' ', false, true);

                            gtf.PrintTotal();

                            gtf.ReplaceGroupHdr("NO : " + docNo, "NO : " + dt.Rows[i]["DocNo"].ToString());
                            gtf.PrintAfterBreak();
                            counterData = 0;
                            docNo = dt.Rows[i]["DocNo"].ToString();
                        }
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail("(" + dt.Rows[i]["CustomerCode"].ToString() + ") " + dt.Rows[i]["CustomerName"].ToString(), 25, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BPKBNo"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiNo"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["RefferenceDesc1"].ToString(), 17, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 8, ' ', false, true);

                    gtf.PrintData(false);
                }
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}