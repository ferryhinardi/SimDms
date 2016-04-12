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
    public class txtOmRpSalRgs014 : IRptProc 
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
            return CreateReportOmRpSalRgs014(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs014(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Summary Permohonan Faktur Polisi (OmRpSalRgs014)
            if (reportID == "OmRpSalRgs014")
            {
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeader("MODEL / MODEL DESC", 42, ' ', true);
                gtf.SetGroupHeader("FAKTUR", 21, ' ', true, false, false, true);
                gtf.SetGroupHeader("TOTAL", 10, ' ', false, true, true);
                gtf.SetGroupHeaderSpace(47);
                gtf.SetGroupHeader("-", 22, '-', false, true);
                gtf.SetGroupHeaderSpace(47);
                gtf.SetGroupHeader("MD", 10, ' ', true, false, true);
                gtf.SetGroupHeader("SUB", 11, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                decimal decTotMD = 0, decTotSub = 0, decTotal = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["Model"].ToString(), 42, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["MD"].ToString(), 10, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["SUB"].ToString(), 10, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["TOTAL"].ToString(), 11, ' ', false, true, true);
                    gtf.PrintData(true);

                    decTotMD += Convert.ToDecimal(dt.Rows[i]["MD"]);
                    decTotSub += Convert.ToDecimal(dt.Rows[i]["SUB"]);
                    decTotal += Convert.ToDecimal(dt.Rows[i]["TOTAL"]);
                }

                gtf.SetTotalDetailLine();
                gtf.SetTotalDetail("TOTAL", 46, ' ', true);
                gtf.SetTotalDetail(decTotMD.ToString(), 10, ' ', true, false, true);
                gtf.SetTotalDetail(decTotSub.ToString(), 10, ' ', true, false, true);
                gtf.SetTotalDetail(decTotal.ToString(), 11, ' ', false, true, true);
                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}