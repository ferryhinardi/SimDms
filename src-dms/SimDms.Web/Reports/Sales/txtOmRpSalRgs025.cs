﻿using SimDms.Common;
using SimDms.Sales.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace SimDms.Web.Reports.Sales
{
    public class txtOmRpSalRgs025 : IRptProc
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
            return CreateReportOmRpSalRgs025(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs025(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Rekapitulasi Harian (OmRpSalRgs026)
            if (reportID == "OmRpSalRgs026")
            {
                gtf.SetGroupHeader(textParam[0].ToString(), 233, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeader("NAMA", 22, ' ', true);
                gtf.SetGroupHeader("ALAMAT", 90, ' ', true);
                gtf.SetGroupHeader("KOTA", 13, ' ', true);
                gtf.SetGroupHeader("CHASSIS CODE & NO", 19, ' ', true);
                gtf.SetGroupHeader("ENGINE CODE & NO", 18, ' ', true);
                gtf.SetGroupHeader("TYPE / MODEL", 13, ' ', true);
                gtf.SetGroupHeader("WARNA", 42, ' ', true);
                gtf.SetGroupHeader("PARAF", 5, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiName"].ToString(), 22, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiAddress1"].ToString() + " " + dt.Rows[i]["FakturPolisiAddress2"].ToString() + " " + dt.Rows[i]["FakturPolisiAddress3"].ToString(), 90, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CityName"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString() + " " + dt.Rows[i]["ChassisNo"].ToString(), 19, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString() + " " + dt.Rows[i]["EngineNo"].ToString(), 18, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString() + " " + dt.Rows[i]["RefferenceDesc1"].ToString(), 42, ' ', true);
                    gtf.SetDataDetail("", 5, ' ', false, true);
                    gtf.PrintData(true);
                }
                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}