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
    public class txtOmRpSalRgs031 : IRptProc
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
            return CreateReportOmRpSalRgs031(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs031(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Outstanding STNK per Biro Jasa (OmRpSalRgs031)
            if (reportID == "OmRpSalRgs031")
            {
                if (textParam[2].ToString() != string.Empty)
                    gtf.SetGroupHeader(textParam[2].ToString(), 136, ' ', false, true, false, true);
                else
                    gtf.SetGroupHeader("S/D TANGGAL : " + textParam[0].ToString(), 136, ' ', false, true, false, true);
                gtf.SetGroupHeader("", 136, ' ', false, true);
                gtf.SetGroupHeader("CABANG : " + textParam[1].ToString(), 136, ' ', false, true);
                gtf.SetGroupHeader(textParam[3].ToString(), 136, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
                gtf.SetGroupHeader("NAMA", 26, ' ', true);
                gtf.SetGroupHeader("NO RANGKA", 10, ' ', true);
                gtf.SetGroupHeader("NO MESIN", 9, ' ', true);
                gtf.SetGroupHeader("KOTA", 29, ' ', true);
                gtf.SetGroupHeader("TGL. STNK JADI", 15, ' ', true);
                gtf.SetGroupHeader("NO POLISI", 11, ' ', true);
                gtf.SetGroupHeader("TGL. BPKP JADI", 15, ' ', true);
                gtf.SetGroupHeader("NO BPKB", 9, ' ', false, true);
                gtf.SetGroupHeaderLine();

                gtf.PrintHeader();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    counterData += 1;

                    gtf.SetDataDetail(counterData.ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiName"].ToString(), 26, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiAddress3"].ToString(), 29, ' ', true);
                    if (Convert.ToDateTime(dt.Rows[i]["STNKInDate"]).ToShortDateString() != "1/1/1900")
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["STNKInDate"]).ToString("dd-MMM-yyyy"), 15, ' ', true);
                    else
                        gtf.SetDataDetail("", 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PoliceRegistrationNo"].ToString(), 11, ' ', true);
                    if (Convert.ToDateTime(dt.Rows[i]["BPKBInDate"]).ToShortDateString() != "1/1/1900")
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPKBInDate"]).ToString("dd-MMM-yyyy"), 15, ' ', true);
                    else
                        gtf.SetDataDetail("", 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BPKBNo"].ToString(), 9, ' ', false, true);

                    gtf.PrintData(true);
                }

                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}