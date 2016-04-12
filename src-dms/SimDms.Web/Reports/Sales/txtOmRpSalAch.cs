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
    public class txtOmRpSalAch : IRptProc 
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
            return CreateReportOmRpSalAch(rptId, dt, sparam, printerloc, print, "", fullpage, oparam); 
        }

        private string CreateReportOmRpSalAch(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Sales Achievement Record (OmRpSalAch)
            if (reportID == "OmRpSalAch")
            {
                bCreateBy = false;
                if (textParam != null) gtf.SetGroupHeader(textParam[0].ToString(), 233, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeaderSpace(149);
                gtf.SetGroupHeader("DELIVERY ORDER", 27, ' ', true, false, false, true);
                gtf.SetGroupHeader("B P K", 27, ' ', true, false, false, true);
                gtf.SetGroupHeaderSpace(16);
                gtf.SetGroupHeader("PAYMENT", 12, ' ', false, true, false, true);
                gtf.SetGroupHeaderSpace(149);
                gtf.SetGroupHeader("-", 27, '-', true, false, false, true);
                gtf.SetGroupHeader("-", 27, '-', true, false, false, true);
                gtf.SetGroupHeaderSpace(16);
                gtf.SetGroupHeader("-", 12, '-', false, true, false, true);
                gtf.SetGroupHeader("NO", 3, ' ', true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("PELANGGAN", 35, ' ', true);
                gtf.SetGroupHeader("ALAMAT", 65, ' ', true);
                gtf.SetGroupHeader("MODEL", 10, ' ', true);
                gtf.SetGroupHeader("COLOUR", 7, ' ', true);
                gtf.SetGroupHeader("CHASSIS", 7, ' ', true);
                gtf.SetGroupHeader("ENGINE", 7, ' ', true);
                gtf.SetGroupHeader("SPKP", 6, ' ', true);
                gtf.SetGroupHeader("NOMOR", 14, ' ', true);
                gtf.SetGroupHeader("TANGGAL", 12, ' ', true);
                gtf.SetGroupHeader("NOMOR", 14, ' ', true);
                gtf.SetGroupHeader("TANGGAL", 12, ' ', true);
                gtf.SetGroupHeader("HARGA JUAL", 15, ' ', true, false, true);
                gtf.SetGroupHeader("CASH LEASING", 12, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                string employeeName = string.Empty;
                decimal decSubUnit = 0, decSubHargaJual = 0, decTotUnit = 0, decTotHargaJual = 0;

                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        gtf.SetDataReportLine();
                        gtf.SetDataDetail("SUB TOTAL", 40, ' ', true);
                        gtf.SetDataDetail(decSubUnit.ToString() + " UNIT", 163, ' ', true);
                        gtf.SetDataDetail(decSubHargaJual.ToString(), 15, ' ', false, true, true, true, "n0");

                        decTotUnit += decSubUnit;
                        decTotHargaJual += decSubHargaJual;

                        gtf.PrintData(false);
                        break;
                    }

                    if (employeeName == string.Empty)
                    {
                        employeeName = dt.Rows[i]["LookupValueName"].ToString();
                        gtf.SetDataDetail("SALESMAN / CATEGORY :", 21, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SalesCode"].ToString() + " -", 10, ' ', true);
                        gtf.SetDataDetail(employeeName, 200, ' ', false, true);
                        gtf.SetDataDetail("SUPERVISOR          :", 21, ' ', true);
                        gtf.SetDataDetail("-", 8, ' ', false, true, true);
                    }
                    else
                    {
                        if (employeeName != dt.Rows[i]["LookupValueName"].ToString())
                        {
                            gtf.SetDataReportLine();
                            gtf.SetDataDetail("SUB TOTAL", 40, ' ', true);
                            gtf.SetDataDetail(decSubUnit.ToString() + " UNIT", 163, ' ', true);
                            gtf.SetDataDetail(decSubHargaJual.ToString(), 15, ' ', false, true, true, true, "n0");
                            gtf.SetDataDetail("", 233, ' ', false, true);

                            decTotUnit += decSubUnit;
                            decTotHargaJual += decSubHargaJual;

                            decSubUnit = decSubHargaJual = 0;

                            employeeName = dt.Rows[i]["LookupValueName"].ToString();
                            gtf.SetDataDetail("SALESMAN / CATEGORY :", 21, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["SalesCode"].ToString() + " -", 10, ' ', true);
                            gtf.SetDataDetail(employeeName, 200, ' ', false, true);
                            gtf.SetDataDetail("SUPERVISOR          :", 21, ' ', true);
                            gtf.SetDataDetail("-", 8, ' ', false, true, true);
                        }
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 24, ' ', true);
                    gtf.SetDataDetail("[" + dt.Rows[i]["CustomerCode"].ToString() + "]", 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Address"].ToString(), 65, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SKPKNo"].ToString(), 6, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DoNo"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DoDate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BPKNo"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPKDate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["HargaJual"].ToString(), 15, ' ', true, false, true, true, "n0");
                    if (dt.Rows[i]["LeasingCo"].ToString() == string.Empty)
                        gtf.SetDataDetail("CASH", 5, ' ', true);
                    else
                        gtf.SetDataDetail("", 5, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LeasingCo"].ToString(), 6, ' ', false, true);

                    decSubUnit += 1;
                    decSubHargaJual += Convert.ToDecimal(dt.Rows[i]["HargaJual"]);

                    gtf.PrintData(false);
                }

                gtf.SetTotalDetailLine();
                gtf.SetTotalDetail("GRAND TOTAL", 40, ' ', true);
                gtf.SetTotalDetail(decTotUnit.ToString() + " UNIT", 163, ' ', true);
                gtf.SetTotalDetail(decTotHargaJual.ToString(), 15, ' ', false, true, true, true, "n0");
                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}