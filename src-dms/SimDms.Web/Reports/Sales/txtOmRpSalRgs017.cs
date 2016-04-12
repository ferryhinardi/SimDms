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
    public class txtOmRpSalRgs017 : IRptProc
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
            return CreateReportOmRpSalRgs017(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs017(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Register Harian Penerbitan Faktur Polisi (OmRpSalRgs017)
            if (reportID == "OmRpSalRgs017")
            {
                gtf.SetGroupHeader(textParam[0].ToString(), 163, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeaderSpace(55);
                gtf.SetGroupHeader("NO FAKTUR", 11, ' ', true);
                gtf.SetGroupHeader("MODEL KENDARAAN", 38, ' ', true);
                gtf.SetGroupHeader("TAHUN", 5, ' ', true, false, true);
                gtf.SetGroupHeader("NO RANGKA", 19, ' ', true);
                gtf.SetGroupHeader("NAMA PEMILIK", 27, ' ', false, true);
                gtf.SetGroupHeaderSpace(55);
                gtf.SetGroupHeader("-", 76, '-', false, true);
                gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
                gtf.SetGroupHeader("DEALER / SUB DEALER", 37, ' ', true);
                gtf.SetGroupHeader("TGL PROSES", 11, ' ', true);
                gtf.SetGroupHeader("TGL FAKTUR", 11, ' ', true);
                gtf.SetGroupHeader("WARNA", 44, ' ', true);
                gtf.SetGroupHeader("NO MESIN", 19, ' ', true);
                gtf.SetGroupHeader("ALAMAT", 27, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 37, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["ReqDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiNo"].ToString(), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 38, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 5, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["Chassis"].ToString(), 19, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiName"].ToString(), 27, ' ', false, true);
                    gtf.SetDataDetailSpace(55);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["FakturPolisiDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["color"].ToString(), 44, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Engine"].ToString(), 19, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiAddress1"].ToString() + " " + dt.Rows[i]["FakturPolisiAddress2"].ToString() + " " + dt.Rows[i]["FakturPolisiAddress3"].ToString(), 27, ' ', false, true);
                    gtf.PrintData(true);
                }

                gtf.SetDataReportLine();
                gtf.SetDataDetail("TOTAL = " + counterData.ToString() + " UNIT", 163, ' ', false, true);
                gtf.SetDataDetail("", 163, ' ', false, true);
                gtf.SetDataDetail("", 163, ' ', false, true);
                gtf.SetDataDetail("Catatan : Tanda (*) merupakan kendaraan untuk angkot", 163, ' ', false, true);
                gtf.SetDataDetail("", 163, ' ', false, true);
                gtf.SetDataDetail(dt.Rows[0]["City"].ToString() + " , " + DateTime.Now.ToString("dd-MMM-yyyy"), 163, ' ', false, true, true);
                gtf.SetDataDetail("", 163, ' ', false, true);
                gtf.SetDataDetail("", 163, ' ', false, true);
                gtf.SetDataDetail("", 163, ' ', false, true);
                gtf.SetDataDetail(dt.Rows[0]["SignName"].ToString(), 162, ' ', false, true, true);
                gtf.PrintData(false);
                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}