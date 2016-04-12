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
    public class txtOmRpInvRgs007 : IRptProc 
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
            return CreateReportOmRpInvRgs007(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpInvRgs007(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Register Ganti Warna (OmRpInvRgs007)
            if (reportID == "OmRpInvRgs007")
            {
                gtf.SetGroupHeader("STATUS : " + textParam[0].ToString(), 163, ' ', false, true, false, true);
                gtf.SetGroupHeader("PERIODE : " + Convert.ToDateTime(textParam[1]).ToString("dd-MMM-yyyy") + " S/D " + Convert.ToDateTime(textParam[2]).ToString("dd-MMM-yyyy"), 163, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
                gtf.SetGroupHeader("TANGGAL", 12, ' ', true);
                gtf.SetGroupHeader("NO. DOKUMEN", 15, ' ', true);
                gtf.SetGroupHeader("NO. REFF", 9, ' ', true);
                gtf.SetGroupHeader("TGL. REFF", 12, ' ', true);
                gtf.SetGroupHeader("MODEL", 9, ' ', true);
                gtf.SetGroupHeader("THN", 6, ' ', true, false, true);
                gtf.SetGroupHeader("WH", 4, ' ', true);
                gtf.SetGroupHeader("NO.", 6, ' ', true);
                gtf.SetGroupHeader("NO.", 6, ' ', true);
                gtf.SetGroupHeader("WARNA LAMA", 26, ' ', true);
                gtf.SetGroupHeader("WARNA BARU", 26, ' ', true);
                gtf.SetGroupHeader("KETERANGAN", 16, ' ', false, true);
                gtf.SetGroupHeaderSpace(79);
                gtf.SetGroupHeader("RANGKA", 6, ' ', true);
                gtf.SetGroupHeader("MESIN", 6, ' ', false, true);

                gtf.SetGroupHeaderLine();

                gtf.PrintHeader();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DocDate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DocNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReferenceNo"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["ReferenceDate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 6, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 4, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 6, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 6, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCodeFrom"].ToString(), 26, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCodeTo"].ToString(), 26, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["RemarkDtl"].ToString(), 16, ' ', false, true);

                    gtf.PrintData(true);
                }

                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}