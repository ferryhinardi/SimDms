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
    public class txtOmRpSalRgs010 : IRptProc
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
            return CreateReportOmRpSalRgs010(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs010(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Laporan Penjualan Terbaik (Per Model/Per Pelanggan) (OmRpSalRgs010/OmRpSalRgs011/OmRpSalRgs012)
            if (reportID == "OmRpSalRgs010" || reportID == "OmRpSalRgs011" || reportID == "OmRpSalRgs012")
            {
                #region Print Group Header
                gtf.SetGroupHeader(textParam[0].ToString(), 80, ' ', false, true, false, true);
                if (textParam[1].ToString() != string.Empty)
                    gtf.SetGroupHeader(textParam[1].ToString(), 80, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);

                if (reportID == "OmRpSalRgs010")
                {
                    gtf.SetGroupHeader("AREA", 43, ' ', true);

                }
                else if (reportID == "OmRpSalRgs011")
                {
                    gtf.SetGroupHeader("PELANGGAN", 43, ' ', true);
                }
                else
                {
                    gtf.SetGroupHeader("MODEL", 43, ' ', true);
                }

                gtf.SetGroupHeader("UNIT", 4, ' ', true);
                gtf.SetGroupHeader("RUPIAH", 15, ' ', true, false, true);
                gtf.SetGroupHeader("(Percent) %", 11, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();
                #endregion

                #region Print Detail
                decimal decPercent = 0, decTotUnit = 0, decTotal = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    string tempkelAR = string.Empty;
                    string tempSalesCode = string.Empty;
                    if (reportID == "OmRpSalRgs010")
                    {
                        if (dt.Rows[i]["kelAR"].ToString().Length > 33)
                        {
                            foreach (char s in dt.Rows[i]["kelAR"].ToString())
                            {
                                if (tempkelAR.Length < 32)
                                {
                                    tempkelAR = tempkelAR + s;
                                }
                                else break;
                            }
                            gtf.SetDataDetail(tempkelAR, 33, ' ', true);
                        }
                        else
                            gtf.SetDataDetail(dt.Rows[i]["kelAR"].ToString(), 33, ' ', true);
                        if (dt.Rows[i]["SalesCode"].ToString().Length > 9)
                        {
                            foreach (char s in dt.Rows[i]["SalesCode"].ToString())
                            {
                                if (tempSalesCode.Length < 8)
                                {
                                    tempSalesCode = tempSalesCode + s;
                                }
                                else break;
                            }
                            gtf.SetDataDetail(tempSalesCode, 9, ' ', true, false, true);
                        }
                        else
                            gtf.SetDataDetail(dt.Rows[i]["SalesCode"].ToString(), 9, ' ', true, false, true);
                    }
                    else if (reportID == "OmRpSalRgs011")
                    {
                        gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 33, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["CustomerCode"].ToString(), 9, ' ', true, false, true);
                    }
                    else
                    {
                        gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 33, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 9, ' ', true, false, true);
                    }

                    gtf.SetDataDetail(dt.Rows[i]["Unit"].ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["Total"].ToString(), 15, ' ', true, false, true, true, "n0");
                    if (Convert.ToDecimal(dt.Rows[i]["Total"]) == 0)
                        gtf.SetDataDetail("0", 11, ' ', false, true, true, true, "n2");
                    else
                    {
                        decPercent = 100 * (Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["Total"])) / Convert.ToDecimal(Convert.ToDecimal(dt.Rows[i]["tot"])));
                        gtf.SetDataDetail(decPercent.ToString(), 11, ' ', false, true, true, true, "n2");
                    }
                    gtf.PrintData(false);

                    if (reportID == "OmRpSalRgs010")
                    {
                        if (tempkelAR != string.Empty || tempSalesCode != string.Empty)
                        {
                            gtf.SetDataDetailSpace(4);
                            if (tempkelAR != string.Empty)
                            {
                                if (dt.Rows[i]["kelAR"].ToString().Substring(tempkelAR.Length)[0] == ' ')
                                    gtf.SetDataDetail(dt.Rows[i]["kelAR"].ToString().Substring(tempkelAR.Length).Remove(0, 1), 33, ' ', true);
                                else
                                    gtf.SetDataDetail(dt.Rows[i]["kelAR"].ToString().Substring(tempkelAR.Length), 33, ' ', true);
                            }
                            else
                                gtf.SetDataDetailSpace(34);
                            if (tempSalesCode != string.Empty)
                                gtf.SetDataDetail(dt.Rows[i]["SalesCode"].ToString().Substring(tempSalesCode.Length), 9, ' ', false, true, true);
                            else
                                gtf.SetDataDetail("", 9, ' ', false, true, true);

                            gtf.PrintData(false);
                        }
                    }

                    decTotUnit += Convert.ToDecimal(dt.Rows[i]["Unit"]);
                    decTotal += Convert.ToDecimal(dt.Rows[i]["Total"]);
                }
                #endregion

                #region Print Footer
                gtf.CheckLastLineforTotal();
                gtf.SetTotalDetailLine();
                gtf.SetTotalDetail("*** TOTAL ***", 47, ' ', true);
                gtf.SetTotalDetail(decTotUnit.ToString(), 4, ' ', true, false, true);
                gtf.SetTotalDetail(decTotal.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail((100).ToString(), 11, ' ', false, true, true, true, "n2");
                gtf.SetTotalDetailLine();
                #endregion
            }
            #endregion
            
            return gtf.sbDataTxt.ToString();
        }
    }
}