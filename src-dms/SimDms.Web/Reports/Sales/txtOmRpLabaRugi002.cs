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
    public class txtOmRpLabaRugi002 : IRptProc 
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
            return CreateReportOmRpLabaRugi002(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpLabaRugi002(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region OmRpLabaRugi002
            if (reportID == "OmRpLabaRugi002")
            {
                gtf.SetGroupHeader(textParam[0].ToString(), 96, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();

                gtf.SetGroupHeader("NO.", 4, ' ', true);
                gtf.SetGroupHeader("MODEL / DESKRIPSI", 30, ' ', true);
                gtf.SetGroupHeader("UNIT", 5, ' ', true, false, true);
                gtf.SetGroupHeader("PENJUALAN", 15, ' ', true, false, true);
                gtf.SetGroupHeader("BIAYA", 15, ' ', true, false, true);
                gtf.SetGroupHeader("LABA-RUGI", 15, ' ', true, false, true);
                gtf.SetGroupHeader("%", 6, ' ', false, true, true);
                gtf.SetGroupHeaderLine();

                gtf.PrintHeader();

                string salesType = string.Empty;
                decimal decSubTotUnit = 0, decSubTotPenjualan = 0, decSubTotBiaya = 0, decSubTotLabaRugi = 0, decSubTotPercentage = 0;
                decimal decTotUnit = 0, decTotPenjualan = 0, decTotBiaya = 0, decTotLabaRugi = 0, decTotPercentage = 0;

                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        gtf.SetDataReportLine();
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("SUB TOTAL :", 30, ' ', true, false, true);
                        gtf.SetDataDetail(decSubTotUnit.ToString(), 5, ' ', true, false, true);
                        gtf.SetDataDetail(decSubTotPenjualan.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decSubTotBiaya.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decSubTotLabaRugi.ToString(), 15, ' ', true, false, true, true, "n0");

                        decSubTotPercentage = (decSubTotLabaRugi / decSubTotPenjualan) * 100;
                        gtf.SetDataDetail(decSubTotPercentage.ToString(), 6, ' ', false, true, true, true, "n2");

                        decTotUnit += decSubTotUnit;
                        decTotPenjualan += decSubTotPenjualan;
                        decTotBiaya += decSubTotBiaya;
                        decTotLabaRugi += decSubTotLabaRugi;
                        decTotPercentage += decSubTotPercentage;

                        gtf.PrintData(false);
                        break;
                    }

                    if (salesType == string.Empty)
                    {
                        salesType = (dt.Rows[i]["SalesType"].ToString() == "1") ? "DIRECTSALES" : "WHOLESALES";
                        gtf.SetDataDetail(salesType, 96, ' ', false, true);
                    }
                    else
                    {
                        if (salesType != ((dt.Rows[i]["SalesType"].ToString() == "1") ? "DIRECTSALES" : "WHOLESALES"))
                        {
                            gtf.SetDataReportLine();
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("SUB TOTAL :", 30, ' ', true, false, true);
                            gtf.SetDataDetail(decSubTotUnit.ToString(), 5, ' ', true, false, true);
                            gtf.SetDataDetail(decSubTotPenjualan.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotBiaya.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotLabaRugi.ToString(), 15, ' ', true, false, true, true, "n0");

                            decSubTotPercentage = (decSubTotLabaRugi / decTotPenjualan) * 100;
                            gtf.SetDataDetail(decSubTotPercentage.ToString(), 6, ' ', false, true, true, true, "n2");

                            gtf.SetDataDetailLineBreak();

                            gtf.PrintData(false);

                            decTotUnit += decSubTotUnit;
                            decTotPenjualan += decSubTotPenjualan;
                            decTotBiaya += decSubTotBiaya;
                            decTotLabaRugi += decSubTotLabaRugi;
                            decTotPercentage += decSubTotPercentage;

                            decSubTotUnit = decSubTotPenjualan = decSubTotBiaya = decSubTotLabaRugi = decSubTotPercentage = 0;
                        }
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 30, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Unit"].ToString(), 5, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["Penjualan"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Biaya"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LabaRugi"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Percentage"].ToString(), 6, ' ', false, true, true, true, "n2");

                    decSubTotUnit += Convert.ToInt32(dt.Rows[i]["Unit"]);
                    decSubTotPenjualan += Convert.ToDecimal(dt.Rows[i]["Penjualan"]);
                    decSubTotBiaya += Convert.ToDecimal(dt.Rows[i]["Biaya"]);
                    decSubTotLabaRugi += Convert.ToDecimal(dt.Rows[i]["LabaRugi"]);

                    gtf.PrintData(true);
                }

                gtf.SetTotalDetailLine();
                gtf.SetTotalDetailSpace(5);
                gtf.SetTotalDetail("GRAND TOTAL :", 30, ' ', true, false, true);
                gtf.SetTotalDetail(decTotUnit.ToString(), 5, ' ', true, false, true);
                gtf.SetTotalDetail(decTotPenjualan.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotBiaya.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotLabaRugi.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotPercentage.ToString(), 6, ' ', false, true, true, true, "n2");
                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}