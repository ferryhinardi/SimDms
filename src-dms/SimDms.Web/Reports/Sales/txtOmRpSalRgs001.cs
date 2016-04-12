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
    public class txtOmRpSalRgs001 : IRptProc
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
            return CreateReportOmRpSalRgs001(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs001(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Register Sales Order (OmRpSalesRgs001/OmRpSalesRgs001A)
            if (reportID == "OmRpSalesRgs001" || reportID == "OmRpSalesRgs001A")
            {
                #region Print Group Header
                if (textParam != null) gtf.SetGroupHeader(textParam[0].ToString(), 233, ' ', false, true, false, true);
                gtf.SetGroupHeader("SALES TYPE :" + dt.Rows[0]["pSALESTYPE"].ToString(), 60, ' ', true);
                gtf.SetGroupHeader("SALES MODEL :" + dt.Rows[0]["pMODEL"].ToString(), 171, ' ', false, true);
                gtf.SetGroupHeader("CUSTOMER   :" + dt.Rows[0]["pCUST"].ToString(), 60, ' ', true);
                gtf.SetGroupHeader("NO SO       :" + dt.Rows[0]["pSO"].ToString(), 171, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("TANGGAL", 12, ' ', true);
                gtf.SetGroupHeader("NO. DOKUMEN", 14, ' ', true);
                gtf.SetGroupHeader("PELANGGAN", 77, ' ', true);
                gtf.SetGroupHeader("PAYMENT TERM", 15, ' ', true);
                gtf.SetGroupHeader("NAMA SALES", 24, ' ', true);
                gtf.SetGroupHeader("SEQ", 3, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("MODEL", 10, ' ', true);
                gtf.SetGroupHeader("BBN", 15, ' ', true, false, true);
                gtf.SetGroupHeader("PENJUALAN", 15, ' ', true, false, true);
                gtf.SetGroupHeader("DISKON", 15, ' ', true, false, true);
                gtf.SetGroupHeader("DPP", 15, ' ', true, false, true);
                gtf.SetGroupHeader("SO", 2, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("DO", 2, ' ', false, true, true);
                gtf.SetGroupHeaderSpace(13);
                gtf.SetGroupHeader("NO. SKPK", 14, ' ', true);
                gtf.SetGroupHeader("LEASING", 93, ' ', true);
                gtf.SetGroupHeader("KELOMPOK AR", 15, ' ', true);
                gtf.SetGroupHeaderSpace(14);
                gtf.SetGroupHeader("WARNA", 11, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                string branch = string.Empty;
                decimal decTotBeforeDiscTotal = 0, decTotDiscIncludePPN = 0, decTotAfterDiscTotal = 0, decTotQuantitySO = 0, decTotQuantityDO = 0;
                #endregion

                #region Print Detail
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count) break;

                    if (reportID == "OmRpSalesRgs001A")
                    {
                        if (branch == string.Empty)
                        {
                            branch = dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString();
                            gtf.SetDataDetail("Branch : " + branch, 233, ' ', false, true);
                        }
                        else
                        {
                            if (branch != dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString())
                            {
                                gtf.SetDataDetail("", 233, ' ', false, true);
                                branch = dt.Rows[i]["BranchCode"].ToString() + " " + dt.Rows[i]["BranchName"].ToString();
                                gtf.SetDataDetail("Branch : " + branch, 233, ' ', false, true);
                            }
                        }
                    }

                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["SODate"]).ToString("dd-MMM-yyyy"), 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SONo"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 61, ' ', true);
                    gtf.SetDataDetail("(" + dt.Rows[i]["GroupPriceCode"].ToString() + ")", 4, ' ', true);
                    gtf.SetDataDetail("[" + dt.Rows[i]["CustomerCode"].ToString() + "]", 10, ' ', true);

                    gtf.SetDataDetail(dt.Rows[i]["LookupValueName"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Salesman"].ToString(), 24, ' ', true);
                    if (dt.Rows[i]["SONo"].ToString() != string.Empty)
                        gtf.SetDataDetail("1", 3, ' ', true, false, true);
                    else
                        gtf.SetDataDetail("0", 3, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BBN"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["BeforeDiscTotal"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscIncludePPN"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AfterDiscTotal"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["QuantitySO"].ToString(), 2, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["QuantityDO"].ToString(), 2, ' ', false, true, true);
                    gtf.SetDataDetailSpace(13);
                    gtf.SetDataDetail(dt.Rows[i]["SKPKNo"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LeasingName"].ToString(), 93, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesCode"].ToString(), 15, ' ', true);
                    gtf.SetDataDetailSpace(14);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 11, ' ', false, true);

                    decTotBeforeDiscTotal += Convert.ToDecimal(dt.Rows[i]["BeforeDiscTotal"]);
                    decTotDiscIncludePPN += Convert.ToDecimal(dt.Rows[i]["DiscIncludePPN"]);
                    decTotAfterDiscTotal += Convert.ToDecimal(dt.Rows[i]["AfterDiscTotal"]);
                    decTotQuantitySO += Convert.ToDecimal(dt.Rows[i]["QuantitySO"]);
                    decTotQuantityDO += Convert.ToDecimal(dt.Rows[i]["QuantityDO"]);

                    gtf.PrintData(false);
                }
                #endregion

                #region Print Footer
                gtf.SetTotalDetailLine();
                gtf.SetTotalDetail("GRAND TOTAL", 178, ' ', true);
                gtf.SetTotalDetail(decTotBeforeDiscTotal.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetailSpace(16);
                gtf.SetTotalDetail(decTotAfterDiscTotal.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetailSpace(4);
                gtf.SetTotalDetail(decTotQuantityDO.ToString(), 2, ' ', false, true, true, true, "n0");
                gtf.SetTotalDetail("", 195, ' ', true);
                gtf.SetTotalDetail(decTotDiscIncludePPN.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetailSpace(16);
                gtf.SetTotalDetail(decTotQuantitySO.ToString(), 2, ' ', false, true, true, true, "n0");
                gtf.SetTotalDetailLine();
                #endregion
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}