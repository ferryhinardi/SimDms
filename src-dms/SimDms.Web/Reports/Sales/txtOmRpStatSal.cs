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
    public class txtOmRpStatSal : IRptProc 
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
            return CreateReportOmRpStatSal(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpStatSal(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Report Statistik Salesman (OmRpStatSal/OmRpStatSalA/OmRpStatSalB/OmRpStatSalC)
            if (reportID == "OmRpStatSal" || reportID == "OmRpStatSalA" || reportID == "OmRpStatSalB" || reportID == "OmRpStatSalC")
            {
                #region Print Group Header
                gtf.SetGroupHeader(textParam[0].ToString(), 233, ' ', false, true, false, true);
                gtf.SetGroupHeader(textParam[2].ToString(), 233, ' ', false, true, false, true);
                gtf.SetGroupHeader("Branch : " + textParam[3].ToString(), 233, ' ', false, true);
                gtf.SetGroupHeaderLine();

                string salesModelCode = string.Empty;
                decimal decSubTotApril = 0, decSubTotMei = 0, decSubTotJuni = 0, decSubTotJuli = 0, decSubTotAgust = 0, decSubTotSept = 0,
                    decSubTotOkt = 0, decSubTotNov = 0, decSubTotDes = 0, decSubTotJan = 0, decSubTotFeb = 0, decSubTotMaret = 0, decSubTotal = 0;
                decimal decTotApril = 0, decTotMei = 0, decTotJuni = 0, decTotJuli = 0, decTotAgust = 0, decTotSept = 0, decTotOkt = 0,
                    decTotNov = 0, decTotDes = 0, decTotJan = 0, decTotFeb = 0, decTotMaret = 0, decTotal = 0;
                #endregion

                #region Print Detail

                #region OmRpStatSal
                if (reportID == "OmRpStatSal")
                {
                    gtf.SetGroupHeader("NO", 4, ' ', true);
                    gtf.SetGroupHeader("SALESMAN / KEL. AR", 228, ' ', false, true);
                    gtf.SetGroupHeaderSpace(5);
                    gtf.SetGroupHeader("MODEL", 15, ' ', true);
                    gtf.SetGroupHeader("WARNA", 28, ' ', true);
                    gtf.SetGroupHeader("JANUARI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("FEBRUARI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("MARET", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("APRIL", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("MEI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("JUNI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("JULI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("AGUSTUS", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("SEPTEMBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("OKTOBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("NOVEMBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("DESEMBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("TOTAL", 14, ' ', false, true, true);
                    gtf.SetGroupHeaderLine();

                    gtf.PrintHeader();

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("-", 228, '-', false, true);
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("SUB TOTAL : ", 15, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SalesCode"].ToString(), 28, ' ', true);

                            gtf.SetDataDetail(decSubTotJan.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotFeb.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotMaret.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotApril.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotMei.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotJuni.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotJuli.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotAgust.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotSept.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotOkt.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotNov.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotDes.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotal.ToString(), 14, ' ', false, true, true, true, "n0");

                            gtf.PrintData(false);

                            decTotApril += decSubTotApril;
                            decTotMei += decSubTotMei;
                            decTotJuni += decSubTotJuni;
                            decTotJuli += decSubTotJuli;
                            decTotAgust += decSubTotAgust;
                            decTotSept += decSubTotSept;
                            decTotOkt += decSubTotOkt;
                            decTotNov += decSubTotNov;
                            decTotDes += decSubTotDes;
                            decTotJan += decSubTotJan;
                            decTotFeb += decSubTotFeb;
                            decTotMaret += decSubTotMaret;
                            decTotal += decSubTotal;

                            break;
                        }

                        if (salesModelCode == string.Empty)
                        {
                            salesModelCode = dt.Rows[i]["Sales"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 4, ' ', true);
                            gtf.SetDataDetail(salesModelCode, 228, ' ', false, true);
                        }
                        else
                        {
                            if (salesModelCode != dt.Rows[i]["Sales"].ToString())
                            {
                                gtf.SetDataDetailSpace(5);
                                gtf.SetDataDetail("-", 228, '-', false, true);
                                gtf.SetDataDetailSpace(5);
                                gtf.SetDataDetail("SUB TOTAL : ", 15, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i - 1]["SalesCode"].ToString(), 28, ' ', true);

                                gtf.SetDataDetail(decSubTotJan.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotFeb.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotMaret.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotApril.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotMei.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotJuni.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotJuli.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotAgust.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotSept.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotOkt.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotNov.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotDes.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotal.ToString(), 14, ' ', false, true, true, true, "n0");

                                gtf.SetDataDetailLineBreak();

                                counterData += 1;
                                gtf.SetDataDetail(counterData.ToString(), 4, ' ', true);
                                salesModelCode = dt.Rows[i]["Sales"].ToString();
                                gtf.SetDataDetail(salesModelCode, 228, ' ', false, true);

                                decTotApril += decSubTotApril;
                                decTotMei += decSubTotMei;
                                decTotJuni += decSubTotJuni;
                                decTotJuli += decSubTotJuli;
                                decTotAgust += decSubTotAgust;
                                decTotSept += decSubTotSept;
                                decTotOkt += decSubTotOkt;
                                decTotNov += decSubTotNov;
                                decTotDes += decSubTotDes;
                                decTotJan += decSubTotJan;
                                decTotFeb += decSubTotFeb;
                                decTotMaret += decSubTotMaret;
                                decTotal += decSubTotal;

                                decSubTotApril = decSubTotMei = decSubTotJuni = decSubTotJuli = decSubTotAgust = decSubTotSept = decSubTotOkt =
                                    decSubTotNov = decSubTotDes = decSubTotJan = decSubTotFeb = decSubTotMaret = decSubTotal = 0;
                            }
                        }

                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 28, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["JANUARI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["FEBRUARI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["MARET"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["APRIL"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["MEI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["JUNI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["JULI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["AGUSTUS"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["SEPTEMBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["OKTOBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["NOVEMBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["DESEMBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["Total"].ToString(), 14, ' ', false, true, true, true, "n0");

                        decSubTotApril += Convert.ToDecimal(dt.Rows[i]["APRIL"]);
                        decSubTotMei += Convert.ToDecimal(dt.Rows[i]["MEI"]);
                        decSubTotJuni += Convert.ToDecimal(dt.Rows[i]["JUNI"]);
                        decSubTotJuli += Convert.ToDecimal(dt.Rows[i]["JULI"]);
                        decSubTotAgust += Convert.ToDecimal(dt.Rows[i]["AGUSTUS"]);
                        decSubTotSept += Convert.ToDecimal(dt.Rows[i]["SEPTEMBER"]);
                        decSubTotOkt += Convert.ToDecimal(dt.Rows[i]["OKTOBER"]);
                        decSubTotNov += Convert.ToDecimal(dt.Rows[i]["NOVEMBER"]);
                        decSubTotDes += Convert.ToDecimal(dt.Rows[i]["DESEMBER"]);
                        decSubTotJan += Convert.ToDecimal(dt.Rows[i]["JANUARI"]);
                        decSubTotFeb += Convert.ToDecimal(dt.Rows[i]["FEBRUARI"]);
                        decSubTotMaret += Convert.ToDecimal(dt.Rows[i]["MARET"]);
                        decSubTotal += Convert.ToDecimal(dt.Rows[i]["Total"]);

                        gtf.PrintData(false);
                    }
                }
                #endregion

                #region OmRpStatSalA
                if (reportID == "OmRpStatSalA")
                {
                    gtf.SetGroupHeader("NO", 4, ' ', true);
                    gtf.SetGroupHeader("MODEL", 15, ' ', true);
                    gtf.SetGroupHeader("WARNA", 28, ' ', true);
                    gtf.SetGroupHeader("APRIL", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("MEI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("JUNI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("JULI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("AGUSTUS", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("SEPTEMBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("OKTOBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("NOVEMBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("DESEMBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("JANUARI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("FEBRUARI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("MARET", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("TOTAL", 14, ' ', false, true, true);
                    gtf.SetGroupHeaderLine();

                    gtf.PrintHeader();

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("-", 228, '-', false, true);
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("SUB TOTAL : ", 15, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SalesCode"].ToString(), 28, ' ', true);

                            gtf.SetDataDetail(decSubTotApril.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotMei.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotJuni.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotJuli.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotAgust.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotSept.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotOkt.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotNov.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotDes.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotJan.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotFeb.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotMaret.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotal.ToString(), 14, ' ', false, true, true, true, "n0");

                            gtf.PrintData(false);

                            decTotApril += decSubTotApril;
                            decTotMei += decSubTotMei;
                            decTotJuni += decSubTotJuni;
                            decTotJuli += decSubTotJuli;
                            decTotAgust += decSubTotAgust;
                            decTotSept += decSubTotSept;
                            decTotOkt += decSubTotOkt;
                            decTotNov += decSubTotNov;
                            decTotDes += decSubTotDes;
                            decTotJan += decSubTotJan;
                            decTotFeb += decSubTotFeb;
                            decTotMaret += decSubTotMaret;
                            decTotal += decSubTotal;

                            break;
                        }

                        if (salesModelCode == string.Empty)
                        {
                            salesModelCode = dt.Rows[i]["Sales"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 4, ' ', true);
                            gtf.SetDataDetail("SALESMAN / KEL. AR : " + salesModelCode, 163, ' ', false, true);
                        }
                        else
                        {
                            if (salesModelCode != dt.Rows[i]["Sales"].ToString())
                            {
                                gtf.SetDataDetailSpace(5);
                                gtf.SetDataDetail("-", 228, '-', false, true);
                                gtf.SetDataDetailSpace(5);
                                gtf.SetDataDetail("SUB TOTAL : ", 15, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i - 1]["SalesCode"].ToString(), 28, ' ', true);

                                gtf.SetDataDetail(decSubTotApril.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotMei.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotJuni.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotJuli.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotAgust.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotSept.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotOkt.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotNov.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotDes.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotJan.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotFeb.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotMaret.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotal.ToString(), 14, ' ', false, true, true, true, "n0");

                                gtf.SetDataDetailLineBreak();

                                counterData += 1;
                                gtf.SetDataDetail(counterData.ToString(), 4, ' ', true);
                                salesModelCode = dt.Rows[i]["Sales"].ToString();
                                gtf.SetDataDetail("SALESMAN / KEL. AR : " + salesModelCode, 163, ' ', false, true);

                                decTotApril += decSubTotApril;
                                decTotMei += decSubTotMei;
                                decTotJuni += decSubTotJuni;
                                decTotJuli += decSubTotJuli;
                                decTotAgust += decSubTotAgust;
                                decTotSept += decSubTotSept;
                                decTotOkt += decSubTotOkt;
                                decTotNov += decSubTotNov;
                                decTotDes += decSubTotDes;
                                decTotJan += decSubTotJan;
                                decTotFeb += decSubTotFeb;
                                decTotMaret += decSubTotMaret;
                                decTotal += decSubTotal;

                                decSubTotApril = decSubTotMei = decSubTotJuni = decSubTotJuli = decSubTotAgust = decSubTotSept = decSubTotOkt =
                                    decSubTotNov = decSubTotDes = decSubTotJan = decSubTotFeb = decSubTotMaret = decSubTotal = 0;
                            }
                        }

                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 28, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["APRIL"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["MEI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["JUNI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["JULI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["AGUSTUS"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["SEPTEMBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["OKTOBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["NOVEMBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["DESEMBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["JANUARI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["FEBRUARI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["MARET"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["Total"].ToString(), 14, ' ', false, true, true, true, "n0");

                        decSubTotApril += Convert.ToDecimal(dt.Rows[i]["APRIL"]);
                        decSubTotMei += Convert.ToDecimal(dt.Rows[i]["MEI"]);
                        decSubTotJuni += Convert.ToDecimal(dt.Rows[i]["JUNI"]);
                        decSubTotJuli += Convert.ToDecimal(dt.Rows[i]["JULI"]);
                        decSubTotAgust += Convert.ToDecimal(dt.Rows[i]["AGUSTUS"]);
                        decSubTotSept += Convert.ToDecimal(dt.Rows[i]["SEPTEMBER"]);
                        decSubTotOkt += Convert.ToDecimal(dt.Rows[i]["OKTOBER"]);
                        decSubTotNov += Convert.ToDecimal(dt.Rows[i]["NOVEMBER"]);
                        decSubTotDes += Convert.ToDecimal(dt.Rows[i]["DESEMBER"]);
                        decSubTotJan += Convert.ToDecimal(dt.Rows[i]["JANUARI"]);
                        decSubTotFeb += Convert.ToDecimal(dt.Rows[i]["FEBRUARI"]);
                        decSubTotMaret += Convert.ToDecimal(dt.Rows[i]["MARET"]);
                        decSubTotal += Convert.ToDecimal(dt.Rows[i]["Total"]);

                        gtf.PrintData(false);
                    }
                }
                #endregion

                #region OmRpStatSalB
                if (reportID == "OmRpStatSalB")
                {
                    gtf.SetGroupHeader("NO", 4, ' ', true);
                    gtf.SetGroupHeader("MODEL", 288, ' ', false, true);
                    gtf.SetGroupHeaderSpace(5);
                    gtf.SetGroupHeader("SALESMAN / KEL. AR", 21, ' ', true);
                    gtf.SetGroupHeader("WARNA", 22, ' ', true);
                    gtf.SetGroupHeader("JANUARI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("FEBRUARI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("MARET", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("APRIL", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("MEI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("JUNI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("JULI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("AGUSTUS", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("SEPTEMBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("OKTOBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("NOVEMBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("DESEMBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("TOTAL", 14, ' ', false, true, true);
                    gtf.SetGroupHeaderLine();

                    gtf.PrintHeader();

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("-", 228, '-', false, true);
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("SUB TOTAL : " + dt.Rows[i - 1]["SalesModelCode"].ToString(), 44, ' ', true);

                            gtf.SetDataDetail(decSubTotJan.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotFeb.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotMaret.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotApril.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotMei.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotJuni.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotJuli.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotAgust.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotSept.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotOkt.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotNov.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotDes.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotal.ToString(), 14, ' ', false, true, true, true, "n0");

                            gtf.PrintData(false);

                            decTotApril += decSubTotApril;
                            decTotMei += decSubTotMei;
                            decTotJuni += decSubTotJuni;
                            decTotJuli += decSubTotJuli;
                            decTotAgust += decSubTotAgust;
                            decTotSept += decSubTotSept;
                            decTotOkt += decSubTotOkt;
                            decTotNov += decSubTotNov;
                            decTotDes += decSubTotDes;
                            decTotJan += decSubTotJan;
                            decTotFeb += decSubTotFeb;
                            decTotMaret += decSubTotMaret;
                            decTotal += decSubTotal;

                            break;
                        }

                        if (salesModelCode == string.Empty)
                        {
                            salesModelCode = dt.Rows[i]["SalesModelCode"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 4, ' ', true);
                            gtf.SetDataDetail(salesModelCode, 228, ' ', false, true);
                        }
                        else
                        {
                            if (salesModelCode != dt.Rows[i]["SalesModelCode"].ToString())
                            {
                                gtf.SetDataDetailSpace(5);
                                gtf.SetDataDetail("-", 228, '-', false, true);
                                gtf.SetDataDetailSpace(5);
                                gtf.SetDataDetail("SUB TOTAL : " + dt.Rows[i - 1]["SalesModelCode"].ToString(), 44, ' ', true);

                                gtf.SetDataDetail(decSubTotJan.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotFeb.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotMaret.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotApril.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotMei.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotJuni.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotJuli.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotAgust.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotSept.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotOkt.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotNov.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotDes.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotal.ToString(), 14, ' ', false, true, true, true, "n0");

                                gtf.SetDataDetailLineBreak();

                                counterData += 1;
                                gtf.SetDataDetail(counterData.ToString(), 4, ' ', true);
                                salesModelCode = dt.Rows[i]["SalesModelCode"].ToString();
                                gtf.SetDataDetail(salesModelCode, 228, ' ', false, true);

                                decTotApril += decSubTotApril;
                                decTotMei += decSubTotMei;
                                decTotJuni += decSubTotJuni;
                                decTotJuli += decSubTotJuli;
                                decTotAgust += decSubTotAgust;
                                decTotSept += decSubTotSept;
                                decTotOkt += decSubTotOkt;
                                decTotNov += decSubTotNov;
                                decTotDes += decSubTotDes;
                                decTotJan += decSubTotJan;
                                decTotFeb += decSubTotFeb;
                                decTotMaret += decSubTotMaret;
                                decTotal += decSubTotal;

                                decSubTotApril = decSubTotMei = decSubTotJuni = decSubTotJuli = decSubTotAgust = decSubTotSept = decSubTotOkt =
                                    decSubTotNov = decSubTotDes = decSubTotJan = decSubTotFeb = decSubTotMaret = decSubTotal = 0;
                            }
                        }

                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail(dt.Rows[i]["Sales"].ToString(), 21, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 22, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["JANUARI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["FEBRUARI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["MARET"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["APRIL"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["MEI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["JUNI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["JULI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["AGUSTUS"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["SEPTEMBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["OKTOBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["NOVEMBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["DESEMBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["Total"].ToString(), 14, ' ', false, true, true, true, "n0");

                        decSubTotApril += Convert.ToDecimal(dt.Rows[i]["APRIL"]);
                        decSubTotMei += Convert.ToDecimal(dt.Rows[i]["MEI"]);
                        decSubTotJuni += Convert.ToDecimal(dt.Rows[i]["JUNI"]);
                        decSubTotJuli += Convert.ToDecimal(dt.Rows[i]["JULI"]);
                        decSubTotAgust += Convert.ToDecimal(dt.Rows[i]["AGUSTUS"]);
                        decSubTotSept += Convert.ToDecimal(dt.Rows[i]["SEPTEMBER"]);
                        decSubTotOkt += Convert.ToDecimal(dt.Rows[i]["OKTOBER"]);
                        decSubTotNov += Convert.ToDecimal(dt.Rows[i]["NOVEMBER"]);
                        decSubTotDes += Convert.ToDecimal(dt.Rows[i]["DESEMBER"]);
                        decSubTotJan += Convert.ToDecimal(dt.Rows[i]["JANUARI"]);
                        decSubTotFeb += Convert.ToDecimal(dt.Rows[i]["FEBRUARI"]);
                        decSubTotMaret += Convert.ToDecimal(dt.Rows[i]["MARET"]);
                        decSubTotal += Convert.ToDecimal(dt.Rows[i]["Total"]);

                        gtf.PrintData(false);
                    }
                }
                #endregion

                #region OmRpStatSalC
                if (reportID == "OmRpStatSalC")
                {
                    gtf.SetGroupHeader("NO", 4, ' ', true);
                    gtf.SetGroupHeader("MODEL", 288, ' ', false, true);
                    gtf.SetGroupHeaderSpace(5);
                    gtf.SetGroupHeader("SALESMAN / KEL. AR", 21, ' ', true);
                    gtf.SetGroupHeader("WARNA", 22, ' ', true);
                    gtf.SetGroupHeader("APRIL", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("MEI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("JUNI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("JULI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("AGUSTUS", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("SEPTEMBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("OKTOBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("NOVEMBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("DESEMBER", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("JANUARI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("FEBRUARI", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("MARET", 13, ' ', true, false, true);
                    gtf.SetGroupHeader("TOTAL", 14, ' ', false, true, true);
                    gtf.SetGroupHeaderLine();

                    gtf.PrintHeader();

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("-", 228, '-', false, true);
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("SUB TOTAL : " + dt.Rows[i - 1]["SalesModelCode"].ToString(), 44, ' ', true);

                            gtf.SetDataDetail(decSubTotApril.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotMei.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotJuni.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotJuli.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotAgust.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotSept.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotOkt.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotNov.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotDes.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotJan.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotFeb.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotMaret.ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotal.ToString(), 14, ' ', false, true, true, true, "n0");

                            gtf.PrintData(false);

                            decTotApril += decSubTotApril;
                            decTotMei += decSubTotMei;
                            decTotJuni += decSubTotJuni;
                            decTotJuli += decSubTotJuli;
                            decTotAgust += decSubTotAgust;
                            decTotSept += decSubTotSept;
                            decTotOkt += decSubTotOkt;
                            decTotNov += decSubTotNov;
                            decTotDes += decSubTotDes;
                            decTotJan += decSubTotJan;
                            decTotFeb += decSubTotFeb;
                            decTotMaret += decSubTotMaret;
                            decTotal += decSubTotal;

                            break;
                        }

                        if (salesModelCode == string.Empty)
                        {
                            salesModelCode = dt.Rows[i]["SalesModelCode"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 4, ' ', true);
                            gtf.SetDataDetail(salesModelCode, 228, ' ', false, true);
                        }
                        else
                        {
                            if (salesModelCode != dt.Rows[i]["SalesModelCode"].ToString())
                            {
                                gtf.SetDataDetailSpace(5);
                                gtf.SetDataDetail("-", 228, '-', false, true);
                                gtf.SetDataDetailSpace(5);
                                gtf.SetDataDetail("SUB TOTAL : " + dt.Rows[i - 1]["SalesModelCode"].ToString(), 44, ' ', true);

                                gtf.SetDataDetail(decSubTotApril.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotMei.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotJuni.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotJuli.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotAgust.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotSept.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotOkt.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotNov.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotDes.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotJan.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotFeb.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotMaret.ToString(), 13, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotal.ToString(), 14, ' ', false, true, true, true, "n0");

                                gtf.SetDataDetailLineBreak();

                                counterData += 1;
                                gtf.SetDataDetail(counterData.ToString(), 4, ' ', true);
                                salesModelCode = dt.Rows[i]["SalesModelCode"].ToString();
                                gtf.SetDataDetail(salesModelCode, 228, ' ', false, true);

                                decTotApril += decSubTotApril;
                                decTotMei += decSubTotMei;
                                decTotJuni += decSubTotJuni;
                                decTotJuli += decSubTotJuli;
                                decTotAgust += decSubTotAgust;
                                decTotSept += decSubTotSept;
                                decTotOkt += decSubTotOkt;
                                decTotNov += decSubTotNov;
                                decTotDes += decSubTotDes;
                                decTotJan += decSubTotJan;
                                decTotFeb += decSubTotFeb;
                                decTotMaret += decSubTotMaret;
                                decTotal += decSubTotal;

                                decSubTotApril = decSubTotMei = decSubTotJuni = decSubTotJuli = decSubTotAgust = decSubTotSept = decSubTotOkt =
                                    decSubTotNov = decSubTotDes = decSubTotJan = decSubTotFeb = decSubTotMaret = decSubTotal = 0;
                            }
                        }

                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail(dt.Rows[i]["Sales"].ToString(), 21, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 22, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["APRIL"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["MEI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["JUNI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["JULI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["AGUSTUS"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["SEPTEMBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["OKTOBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["NOVEMBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["DESEMBER"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["JANUARI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["FEBRUARI"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["MARET"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["Total"].ToString(), 14, ' ', false, true, true, true, "n0");

                        decSubTotApril += Convert.ToDecimal(dt.Rows[i]["APRIL"]);
                        decSubTotMei += Convert.ToDecimal(dt.Rows[i]["MEI"]);
                        decSubTotJuni += Convert.ToDecimal(dt.Rows[i]["JUNI"]);
                        decSubTotJuli += Convert.ToDecimal(dt.Rows[i]["JULI"]);
                        decSubTotAgust += Convert.ToDecimal(dt.Rows[i]["AGUSTUS"]);
                        decSubTotSept += Convert.ToDecimal(dt.Rows[i]["SEPTEMBER"]);
                        decSubTotOkt += Convert.ToDecimal(dt.Rows[i]["OKTOBER"]);
                        decSubTotNov += Convert.ToDecimal(dt.Rows[i]["NOVEMBER"]);
                        decSubTotDes += Convert.ToDecimal(dt.Rows[i]["DESEMBER"]);
                        decSubTotJan += Convert.ToDecimal(dt.Rows[i]["JANUARI"]);
                        decSubTotFeb += Convert.ToDecimal(dt.Rows[i]["FEBRUARI"]);
                        decSubTotMaret += Convert.ToDecimal(dt.Rows[i]["MARET"]);
                        decSubTotal += Convert.ToDecimal(dt.Rows[i]["Total"]);

                        gtf.PrintData(false);
                    }
                }
                #endregion

                #endregion

                #region Print Footer
                if (reportID == "OmRpStatSalC" || reportID == "OmRpStatSalA")
                {
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("Grand Total : ", 49, ' ', true);
                    gtf.SetTotalDetail(decTotApril.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotMei.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotJuni.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotJuli.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotAgust.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotSept.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotOkt.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotNov.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotDes.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotJan.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotFeb.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotMaret.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotal.ToString(), 14, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();
                }
                else
                {
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("Grand Total : ", 49, ' ', true);
                    gtf.SetTotalDetail(decTotJan.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotFeb.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotMaret.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotApril.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotMei.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotJuni.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotJuli.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotAgust.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotSept.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotOkt.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotNov.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotDes.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(decTotal.ToString(), 14, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();
                }
                #endregion
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}