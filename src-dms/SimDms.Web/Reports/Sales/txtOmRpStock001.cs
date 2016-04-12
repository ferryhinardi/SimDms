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
    public class txtOmRpStock001 : IRptProc 
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
            return CreateReportOmRpStock001(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpStock001(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region OmRpStock001
            if (reportID == "OmRpStock001")
            {
                bCreateBy = false;
                gtf.PrintHeader();

                int loop = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    gtf.SetDataDetail(" -", 95, '-', false, true);
                    if (gtf.line == 31)
                        gtf.PrintData(false, false, false);
                    else
                        gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(40);
                    gtf.SetDataDetail("INVENTORY TAG", 13, ' ');
                    gtf.SetDataDetailSpace(41);
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(" No. S.O.      : " + dt.Rows[i]["STHDRNo"].ToString(), 94, ' ');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(" Seq.          : " + dt.Rows[i]["STNo"].ToString(), 94, ' ');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(" Tgl. S.O.     : " + Convert.ToDateTime(dt.Rows[i]["STDate"]).ToString("dd-MMM-yyyy"), 94, ' ');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 94, '-');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(" Type / Model  : " + dt.Rows[i]["SalesModelCode"].ToString(), 94, ' ');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 94, '-');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(" Warna         : " + dt.Rows[i]["ColourCode"].ToString(), 94, ' ');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 94, '-');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(" No. Rangka    : " + dt.Rows[i]["ChassisNo"].ToString(), 94, ' ');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 94, '-');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(" No. Mesin     : " + dt.Rows[i]["EngineNo"].ToString(), 94, ' ');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 94, '-');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(" Th. Produksi  : " + dt.Rows[i]["SalesModelYear"].ToString(), 94, ' ');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 94, '-');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(" Gudang        : " + dt.Rows[i]["WarehouseCode"].ToString(), 94, ' ');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 94, '-');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(" Lokasi        : ", 94, ' ');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 94, '-');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail(" Catatan       : " + dt.Rows[i]["SONo"].ToString() + " " + dt.Rows[i]["DONo"].ToString(), 94, ' ');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    loop = 30 - gtf.line - 7;

                    for (int j = 0; j < loop; j++)
                    {
                        gtf.SetDataDetail("|", 1, ' ');
                        gtf.SetDataDetailSpace(94);
                        gtf.SetDataDetail("|", 1, ' ', false, true);
                        gtf.PrintData(false, false);
                    }

                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 94, '-');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(7);
                    gtf.SetDataDetail("Auditor", 7, ' ');
                    gtf.SetDataDetailSpace(8);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(8);
                    gtf.SetDataDetail("Gudang", 6, ' ');
                    gtf.SetDataDetailSpace(9);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(8);
                    gtf.SetDataDetail("Acct.", 6, ' ');
                    gtf.SetDataDetailSpace(9);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(7);
                    gtf.SetDataDetail("FAD Head", 8, ' ');
                    gtf.SetDataDetailSpace(8);
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 22, '-');
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 23, '-');
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 23, '-');
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetail("-", 23, '-');
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(22);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(23);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(23);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(23);
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(22);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(23);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(23);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(23);
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(22);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(23);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(23);
                    gtf.SetDataDetail("|", 1, ' ');
                    gtf.SetDataDetailSpace(23);
                    gtf.SetDataDetail("|", 1, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail(" -", 95, '-', false, true);
                    gtf.PrintData(false, false);
                }
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}