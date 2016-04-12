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
    public class txtOmRpStock003 : IRptProc 
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
            return CreateReportOmRpStock003(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpStock003(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region OmRpStock003
            if (reportID == "OmRpStock003")
            {
                gtf.SetGroupHeader(textParam[0].ToString(), 163, ' ', false, true, false, true);
                gtf.SetGroupHeader("", 136, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("No", 3, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("No.StockTaking", 15, ' ', true);
                gtf.SetGroupHeader("Tanggal", 12, ' ', true);
                gtf.SetGroupHeader("Warehouse", 20, ' ', true);
                gtf.SetGroupHeader("Merk/Type", 23, ' ', true);
                gtf.SetGroupHeader("Model", 16, ' ', true);
                gtf.SetGroupHeader("Tahun", 5, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("No.Rangka", 10, ' ', true);
                gtf.SetGroupHeader("No.Mesin", 10, ' ', true);
                gtf.SetGroupHeader("Warna", 25, ' ', true);
                gtf.SetGroupHeader("Keterangan", 12, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["STHdrNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["STDate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 23, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 16, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 5, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 25, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(), 12, ' ', false, true);

                    gtf.PrintData(true);
                }

                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}