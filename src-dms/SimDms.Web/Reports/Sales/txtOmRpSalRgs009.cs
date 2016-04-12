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
    public class txtOmRpSalRgs009 : IRptProc
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
            return CreateReportOmRpSalRgs009(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs009(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Daftar Rincian Kendaraan Bermotor (OmRpSalRgs009)
            if (reportID == "OmRpSalRgs009")
            {
                gtf.SetGroupHeader(textParam[0].ToString(), 233, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeaderSpace(5);
                gtf.SetGroupHeader("FAKTUR PAJAK", 33, ' ', true, false, false, true);
                gtf.SetGroupHeader("PEMBELI", 71, ' ', true, false, false, true);
                gtf.SetGroupHeaderSpace(77);
                gtf.SetGroupHeader("JUMLAH", 32, ' ', false, true, false, true);

                gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
                gtf.SetGroupHeader("-", 33, '-', true);
                gtf.SetGroupHeader("-", 73, '-', true);
                gtf.SetGroupHeader("NOMOR", 7, ' ', true);
                gtf.SetGroupHeader("NOMOR", 7, ' ', true);
                gtf.SetGroupHeader("MERK / TYPE", 26, ' ', true);
                gtf.SetGroupHeader("MODEL", 11, ' ', true);
                gtf.SetGroupHeader("THN", 4, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("HARGA JUAL", 15, ' ', true, false, true);
                gtf.SetGroupHeader("-", 31, '-', true);
                gtf.SetGroupHeader("KETERANGAN", 10, ' ', false, true);

                gtf.SetGroupHeader("URUT", 4, ' ', true, false, true);
                gtf.SetGroupHeader("NOMOR", 20, ' ', true);
                gtf.SetGroupHeader("TANGGAL", 12, ' ', true);
                gtf.SetGroupHeader("NAMA", 50, ' ', true);
                gtf.SetGroupHeader("NPWP", 22, ' ', true);
                gtf.SetGroupHeader("RANGKA", 7, ' ', true);
                gtf.SetGroupHeader("MESIN", 7, ' ', true);
                gtf.SetGroupHeaderSpace(45);
                gtf.SetGroupHeader("KENDARAAN (RP)", 15, ' ', true, false, true);
                gtf.SetGroupHeader("PPN", 15, ' ', true, false, true);
                gtf.SetGroupHeader("PPN BM", 15, ' ', false, true, true);

                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                decimal decTotAfterDiscDPP = 0, decTotAfterDiscPPN = 0, decTotAfterDiscPPnBM = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    counterData += 1;
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPajakNo"].ToString(), 20, ' ', true);
                    if (Convert.ToDateTime(dt.Rows[i]["FakturPajakDate"]).ToShortDateString() == "1/1/1900")
                        gtf.SetDataDetail("", 12, ' ', true);
                    else
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["FakturPajakDate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 50, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["NPWPNo"].ToString(), 22, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 26, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscDPP"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscPPN"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscPPnBM"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(), 10, ' ', false, true);

                    gtf.PrintData(true);

                    decTotAfterDiscDPP += Convert.ToDecimal(dt.Rows[i]["AfterDiscDPP"]);
                    decTotAfterDiscPPN += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPN"]);
                    decTotAfterDiscPPnBM += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPnBM"]);
                }

                gtf.SetTotalDetailLine();
                gtf.SetTotalDetail("JUMLAH PPN DAN PPnBM", 173, ' ', true);
                gtf.SetTotalDetail(decTotAfterDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotAfterDiscPPN.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotAfterDiscPPnBM.ToString(), 15, ' ', false, true, true, true, "n0");
                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}