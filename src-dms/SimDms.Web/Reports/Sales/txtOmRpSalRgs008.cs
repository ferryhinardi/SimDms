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
    public class txtOmRpSalRgs008 : IRptProc
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
            return CreateReportOmRpSalRgs008(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs008(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Register Pelanggan Perleasing (OmRpSalRgs008)
            if (reportID == "OmRpSalRgs008")
            {
                if (textParam != null) gtf.SetGroupHeader(textParam[0].ToString(), 233, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeader("NAMA LEASING", 62, ' ', true);
                gtf.SetGroupHeader("PELANGGAN", 63, ' ', true);
                gtf.SetGroupHeader("NO FAKTUR", 13, ' ', true);
                gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
                gtf.SetGroupHeader("NO DO", 13, ' ', true);
                gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
                gtf.SetGroupHeader("MODEL", 10, ' ', true);
                gtf.SetGroupHeader("WARNA", 5, ' ', true);
                gtf.SetGroupHeader("RANGKA", 7, ' ', true);
                gtf.SetGroupHeader("MESIN", 7, ' ', true);
                gtf.SetGroupHeader("HARGA JUAL", 17, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                string namaLeasing = string.Empty;
                decimal decSubUnit = 0, decSubAfterDiscTotal = 0;
                decimal decTotUnit = 0, decTotAfterDiscTotal = 0;

                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        gtf.SetDataDetailSpace(157);
                        gtf.SetDataDetail("-", 76, '-', false, true);
                        gtf.SetDataDetailSpace(157);
                        gtf.SetDataDetail("SUB TOTAL :", 13, ' ', true);
                        gtf.SetDataDetail(decSubUnit.ToString(), 44, ' ', true);
                        gtf.SetDataDetail(decSubAfterDiscTotal.ToString(), 17, ' ', false, true, true, true, "n0");

                        decTotUnit += decSubUnit;
                        decTotAfterDiscTotal += decSubAfterDiscTotal;

                        break;
                    }

                    if (namaLeasing == string.Empty)
                    {
                        namaLeasing = dt.Rows[i]["NamaLeasing"].ToString();
                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                        gtf.SetDataDetail(namaLeasing, 52, ' ', true);
                    }
                    else
                    {
                        if (namaLeasing != dt.Rows[i]["NamaLeasing"].ToString())
                        {
                            gtf.SetDataDetailSpace(157);
                            gtf.SetDataDetail("-", 76, '-', false, true);
                            gtf.SetDataDetailSpace(157);
                            gtf.SetDataDetail("SUB TOTAL :", 13, ' ', true);
                            gtf.SetDataDetail(decSubUnit.ToString(), 44, ' ', true);
                            gtf.SetDataDetail(decSubAfterDiscTotal.ToString(), 17, ' ', false, true, true, true, "n0");
                            gtf.SetDataDetail("", 233, ' ', false, true);

                            namaLeasing = dt.Rows[i]["NamaLeasing"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                            gtf.SetDataDetail(namaLeasing, 52, ' ', true);

                            decTotUnit += decSubUnit;
                            decTotAfterDiscTotal += decSubAfterDiscTotal;

                            decSubUnit = decSubAfterDiscTotal = 0;
                        }
                    }

                    if (decSubUnit > 0) gtf.SetDataDetailSpace(57);
                    gtf.SetDataDetail(dt.Rows[i]["KodeLeasing"].ToString(), 9, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["Pelanggan"].ToString(), 53, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["KodePelanggan"].ToString(), 9, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DONo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DODate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 5, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscTotal"].ToString(), 17, ' ', false, true, true, true, "n0");

                    gtf.PrintData(true);

                    decSubUnit += 1;
                    decSubAfterDiscTotal += Convert.ToDecimal(dt.Rows[i]["AfterDiscTotal"]);
                }

                gtf.SetTotalDetailLine();
                gtf.SetTotalDetailSpace(157);
                gtf.SetTotalDetail("GRAND TOTAL :", 13, ' ', true);
                gtf.SetTotalDetail(decTotUnit.ToString(), 44, ' ', true);
                gtf.SetTotalDetail(decTotAfterDiscTotal.ToString(), 17, ' ', false, true, true, true, "n0");
                gtf.SetTotalDetailLine();
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}