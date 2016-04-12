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
    public class txtOmRpLabaRugi001 : IRptProc 
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
            return CreateReportOmRpLabaRugi001(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpLabaRugi001(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region OmRpLabaRugi001
            if (reportID == "OmRpLabaRugi001")
            {
                gtf.SetGroupHeader(textParam[0].ToString(), 233, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();

                gtf.SetGroupHeaderSpace(4);
                gtf.SetGroupHeader("INVOICE", 31, ' ', true, false, false, true);
                gtf.SetGroupHeader("PELANGGAN", 44, ' ', true);
                gtf.SetGroupHeader("DO", 15, ' ', true, false, false, true);
                gtf.SetGroupHeader("SO", 16, ' ', false, true, false, true);

                gtf.SetGroupHeaderSpace(4);
                gtf.SetGroupHeader("-", 31, '-', true);
                gtf.SetGroupHeaderSpace(44);
                gtf.SetGroupHeader("-", 15, '-', true);
                gtf.SetGroupHeader("-", 15, '-', false, true);

                gtf.SetGroupHeader("NO.", 4, ' ', true);
                gtf.SetGroupHeader("NOMOR", 17, ' ', true);
                gtf.SetGroupHeader("TANGGAL", 12, ' ', true);
                gtf.SetGroupHeader("SALESMAN", 44, ' ', true);
                gtf.SetGroupHeader("NOMOR / TGL", 15, ' ', true);
                gtf.SetGroupHeader("NOMOR / TGL", 15, ' ', true);
                gtf.SetGroupHeader("MODEL", 15, ' ', true);
                gtf.SetGroupHeader("WARNA", 5, ' ', true);
                gtf.SetGroupHeader("THN.", 5, ' ', true, false, true);
                gtf.SetGroupHeader("RANGKA", 8, ' ', true, false, true);
                gtf.SetGroupHeader("MESIN", 8, ' ', true, false, true);
                gtf.SetGroupHeader("PENJUALAN", 15, ' ', true, false, true);
                gtf.SetGroupHeader("BIAYA", 15, ' ', true, false, true);
                gtf.SetGroupHeader("LABA-RUGI", 15, ' ', true, false, true);
                gtf.SetGroupHeader("%", 7, ' ', true, false, true);
                gtf.SetGroupHeader("TGL. TERIMA", 12, ' ', true);
                gtf.SetGroupHeader("LAMA", 5, ' ', false, true, true);
                gtf.SetGroupHeaderLine();

                gtf.PrintHeader();

                string salesCode = string.Empty;
                int totCounterData = 0;
                decimal decSubTotAfterDiscDPP = 0, decSubTotCOGs = 0, decSubTotLabaRugi = 0;
                decimal decTotAfterDiscDPP = 0, decTotCOGs = 0, decTotLabaRugi = 0;

                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        gtf.SetDataReportLine();
                        gtf.SetDataDetailSpace(112);
                        gtf.SetDataDetail("SUB TOTAL :", 15, ' ', true, false, true);
                        gtf.SetDataDetail(counterData.ToString() + " unit", 30, ' ', true);
                        gtf.SetDataDetail(decSubTotAfterDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decSubTotCOGs.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decSubTotLabaRugi.ToString(), 15, ' ', false, true, true, true, "n0");

                        totCounterData += counterData;
                        decTotAfterDiscDPP += decSubTotAfterDiscDPP;
                        decTotCOGs += decSubTotCOGs;
                        decTotLabaRugi += decSubTotLabaRugi;

                        gtf.PrintData(false);
                        break;
                    }

                    if (salesCode == string.Empty)
                    {
                        salesCode = dt.Rows[i]["SalesCode"].ToString() + " - " + dt.Rows[i]["SalesName"].ToString();
                        gtf.SetDataDetail(salesCode, 233, ' ', false, true);
                    }
                    else
                    {
                        if (salesCode != dt.Rows[i]["SalesCode"].ToString() + " - " + dt.Rows[i]["SalesName"].ToString())
                        {
                            gtf.SetDataReportLine();
                            gtf.SetDataDetailSpace(112);
                            gtf.SetDataDetail("SUB TOTAL :", 15, ' ', true, false, true);
                            gtf.SetDataDetail(counterData.ToString() + " unit", 30, ' ', true);
                            gtf.SetDataDetail(decSubTotAfterDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotCOGs.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotLabaRugi.ToString(), 15, ' ', false, true, true, true, "n0");

                            gtf.SetDataDetailLineBreak();

                            gtf.PrintData(false);

                            totCounterData += counterData;
                            decTotAfterDiscDPP += decSubTotAfterDiscDPP;
                            decTotCOGs += decSubTotCOGs;
                            decTotLabaRugi += decSubTotLabaRugi;

                            counterData = 0;
                            decSubTotAfterDiscDPP = decSubTotCOGs = decSubTotLabaRugi = 0;
                        }
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 17, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 44, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DONo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SONo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 5, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 5, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 8, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 8, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscDPP"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["COGs"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LabaRugi"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Percentage"].ToString(), 7, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPUDate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Lama"].ToString(), 5, ' ', false, true, true);
                    gtf.SetDataDetailSpace(36);
                    gtf.SetDataDetail(dt.Rows[i]["Salesman"].ToString(), 44, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DODate"]).ToString("dd-MMM-yyyy"), 15, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["SODate"]).ToString("dd-MMM-yyyy"), 15, ' ', false, true);

                    decSubTotAfterDiscDPP += Convert.ToDecimal(dt.Rows[i]["AfterDiscDPP"]);
                    decSubTotCOGs += Convert.ToDecimal(dt.Rows[i]["COGs"]);
                    decSubTotLabaRugi += Convert.ToDecimal(dt.Rows[i]["LabaRugi"]);

                    gtf.PrintData(true);
                }

                gtf.SetTotalDetailLine();
                gtf.SetTotalDetailSpace(112);
                gtf.SetTotalDetail("GRAND TOTAL :", 15, ' ', true, false, true);
                gtf.SetTotalDetail(totCounterData.ToString() + " unit", 30, ' ', true);
                gtf.SetTotalDetail(decTotAfterDiscDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotCOGs.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotLabaRugi.ToString(), 15, ' ', false, true, true, true, "n0");
                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}