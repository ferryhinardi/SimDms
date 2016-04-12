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
    public class txtOmRpStock002 : IRptProc 
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
            return CreateReportOmRpStock002(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpStock002(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region OmRpStock002
            if (reportID == "OmRpStock002")
            {
                string companyName = dt.Rows[0]["CompanyName"].ToString();

                gtf.SetGroupHeader(companyName, 96, ' ', false, true);
                gtf.SetGroupHeader("STOCK OPNAME KENDARAAN", 96, ' ', false, true);

                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeader("NO. TAG", 18, ' ', true, false, false, true);
                gtf.SetGroupHeader("MODEL", 15, ' ', true);
                gtf.SetGroupHeader("JML", 3, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("NO RANGKA", 9, ' ', true);
                gtf.SetGroupHeader("NO MESIN", 9, ' ', true);
                gtf.SetGroupHeader("WARNA", 5, ' ', true);
                gtf.SetGroupHeader("TAHUN", 5, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("WH", 2, ' ', true);
                gtf.SetGroupHeader("STATUS", 16, ' ', false, true, false, true);

                gtf.SetGroupHeaderSpace(4);
                gtf.SetGroupHeader("-", 17, '-', true);
                gtf.SetGroupHeaderSpace(58);
                gtf.SetGroupHeader("-", 16, '-', false, true);

                gtf.SetGroupHeaderSpace(4);
                gtf.SetGroupHeader("NO", 13, ' ', true);
                gtf.SetGroupHeader("SEQ", 3, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(58);
                gtf.SetGroupHeader("FORM", 6, ' ', true);
                gtf.SetGroupHeader("KENDARAAN", 9, ' ', false, true);
                gtf.SetGroupHeaderLine();

                gtf.PrintHeader();

                string STHdrNo = string.Empty;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["STHdrNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["STNo"].ToString(), 3, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Jml"].ToString(), 3, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 5, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 5, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["StatusForm"].ToString(), 6, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["StatusKend"].ToString(), 9, ' ', false, true);

                    gtf.PrintData(false, false);
                }
            }
            #endregion

            return gtf.sbDataTxt.ToString();
        }
    }
}