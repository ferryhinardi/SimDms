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
    public class txtOmRpSalRgs032 : IRptProc
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
            return CreateReportOmRpSalRgs032(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs032(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Tanda Terima Faktur / Pengajuan Pengurusan BBN (OmRpSalRgs032)

            if (reportID == "OmRpSalRgs032")
            {
                gtf.SetGroupHeader(textParam[0].ToString(), 96, ' ', false, true, false, true);
                gtf.SetGroupHeader("Biro Jasa : " + dt.Rows[0]["SupplierName"].ToString(), 96, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeaderSpace(5);
                gtf.SetGroupHeader("NAMA", 29, ' ', true);
                gtf.SetGroupHeader("CABANG", 23, ' ', false, true);
                gtf.SetGroupHeaderSpace(5);
                gtf.SetGroupHeader("-", 29, '-', true);
                gtf.SetGroupHeader("-", 23, '-', false, true);
                gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
                gtf.SetGroupHeader("ALAMAT", 29, ' ', true);
                gtf.SetGroupHeader("KABUPATEN", 23, ' ', true);
                gtf.SetGroupHeader("TIPE", 15, ' ', true);
                gtf.SetGroupHeader("#RANGKA", 7, ' ', true, false, true);
                gtf.SetGroupHeader("#MESIN", 6, ' ', true, false, true);
                gtf.SetGroupHeader("FAKTUR", 6, ' ', false, true);
                gtf.SetGroupHeaderLine();

                gtf.PrintHeader();


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    counterData += 1;

                    gtf.SetDataDetail(counterData.ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail("(" + dt.Rows[i]["CustomerCode"].ToString() + ") " + dt.Rows[i]["CustomerName"].ToString(), 29, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BranchCode"].ToString() + " -" + dt.Rows[i]["BranchName"].ToString().Trim().Split('-')[1], 23, ' ', false, true);
                    gtf.SetDataDetailSpace(5);
                    gtf.SetDataDetail(dt.Rows[i]["Address1"].ToString() + " " + dt.Rows[i]["Address2"].ToString() + " " + dt.Rows[i]["Address3"].ToString() + " " + dt.Rows[i]["Address4"].ToString(), 29, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LookupValueName"].ToString(), 23, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 7, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 6, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiNo"].ToString(), 6, ' ', false, true);

                    gtf.PrintData(true);
                }
                gtf.SetDataReportLine();
                gtf.SetDataDetail(dt.Rows[0]["CityName"].ToString() + " , " + DateTime.Now.ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                gtf.SetDataDetail("", 96, ' ', false, true);
                gtf.SetDataDetail("YANG MENERIMA,", 63, ' ', true, false, true);
                gtf.SetDataDetailSpace(15);
                gtf.SetDataDetail("YANG MENYERAHKAN,", 17, ' ', false, true);

                gtf.SetTotalDetailLine();
            }

            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}