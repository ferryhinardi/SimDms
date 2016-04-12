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
    public class txtOmRpLabaRugi003 : IRptProc 
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
            return CreateReportOmRpLabaRugi003(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpLabaRugi003(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region OmRpLabaRugi003
            if (reportID == "OmRpLabaRugi003")
            {
                gtf.SetGroupHeader(textParam[0].ToString(), 136, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();

                gtf.SetGroupHeader("NO.", 4, ' ', true);
                gtf.SetGroupHeader("MODEL / DESKRIPSI", 36, ' ', true);
                gtf.SetGroupHeader("UNIT", 5, ' ', true, false, true);
                gtf.SetGroupHeader("PENJUALAN BRUTTO", 16, ' ', true, false, true);
                gtf.SetGroupHeader("DISCOUNT", 15, ' ', true, false, true);
                gtf.SetGroupHeader("PENJUALAN NETTO", 15, ' ', true, false, true);
                gtf.SetGroupHeader("BIAYA", 15, ' ', true, false, true);
                gtf.SetGroupHeader("LABA-RUGI", 15, ' ', true, false, true);
                gtf.SetGroupHeader("%", 6, ' ', false, true, true);
                gtf.SetGroupHeaderLine();

                gtf.PrintHeader();

                string salesType = string.Empty, branchCode = string.Empty;
                decimal decSubTotUnit = 0, decSubTotPenjualanBrutto = 0, decSubTotDiscount = 0, decSubTotPenjualanNetto = 0, decSubTotBiaya = 0, decSubTotLabaRugi = 0, decSubTotPercentage = 0;
                decimal decSubTotSalesTypeUnit = 0, decSubTotSalesTypePenjualanBrutto = 0, decSubTotSalesTypeDiscount = 0, decSubTotSalesTypePenjualanNetto = 0, decSubTotSalesTypeBiaya = 0, decSubTotSalesTypeLabaRugi = 0, decSubTotSalesTypePercentage = 0;
                decimal decTotUnit = 0, decTotPenjualanBrutto = 0, decTotDiscount = 0, decTotPenjualanNetto = 0, decTotBiaya = 0, decTotLabaRugi = 0, decTotPercentage = 0;

                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        gtf.SetDataDetailSpace(21);
                        gtf.SetDataDetail("-", 114, '-', false, true);
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("SUB TOTAL " + branchCode + " : ", 36, ' ', true, false, true);
                        gtf.SetDataDetail(decSubTotUnit.ToString(), 5, ' ', true, false, true);
                        gtf.SetDataDetail(decSubTotPenjualanBrutto.ToString(), 16, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decSubTotDiscount.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decSubTotPenjualanNetto.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decSubTotBiaya.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decSubTotLabaRugi.ToString(), 15, ' ', false, true, true, true, "n0");

                        decSubTotSalesTypeUnit += decSubTotUnit;
                        decSubTotSalesTypePenjualanBrutto += decSubTotPenjualanBrutto;
                        decSubTotSalesTypeDiscount += decSubTotDiscount;
                        decSubTotSalesTypePenjualanNetto += decSubTotPenjualanNetto;
                        decSubTotSalesTypeBiaya += decSubTotBiaya;
                        decSubTotSalesTypeLabaRugi += decSubTotLabaRugi;
                        decSubTotSalesTypePercentage += decSubTotPercentage;

                        decTotUnit += decSubTotSalesTypeUnit;
                        decTotPenjualanBrutto += decSubTotSalesTypePenjualanBrutto;
                        decTotDiscount += decSubTotSalesTypeDiscount;
                        decTotPenjualanNetto += decSubTotSalesTypePenjualanNetto;
                        decTotBiaya += decSubTotSalesTypeBiaya;
                        decTotLabaRugi += decSubTotSalesTypeLabaRugi;
                        decTotPercentage += decSubTotSalesTypePercentage;

                        gtf.SetDataReportLine();
                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("SUB TOTAL " + salesType + " : ", 36, ' ', true, false, true);
                        gtf.SetDataDetail(decSubTotSalesTypeUnit.ToString(), 5, ' ', true, false, true);
                        gtf.SetDataDetail(decSubTotSalesTypePenjualanBrutto.ToString(), 16, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decSubTotSalesTypeDiscount.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decSubTotSalesTypePenjualanNetto.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decSubTotSalesTypeBiaya.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(decSubTotSalesTypeLabaRugi.ToString(), 15, ' ', true, false, true, true, "n0");
                        decTotPercentage = (decTotLabaRugi / decTotPenjualanNetto) * 100;
                        gtf.SetDataDetail(decTotPercentage.ToString(), 6, ' ', false, true, true, true, "n2");

                        gtf.PrintData(false);

                        break;
                    }

                    if (salesType == string.Empty)
                    {
                        salesType = (dt.Rows[i]["SalesType"].ToString() == "1") ? "DIRECTSALES" : "WHOLESALES";
                        gtf.SetDataDetail(salesType, 136, ' ', false, true);
                        gtf.PrintData(false);
                    }
                    else
                    {
                        if (salesType != ((dt.Rows[i]["SalesType"].ToString() == "1") ? "DIRECTSALES" : "WHOLESALES"))
                        {
                            gtf.SetDataReportLine();
                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("SUB TOTAL " + salesType + " : ", 36, ' ', true, false, true);

                            gtf.SetDataDetail(decSubTotSalesTypeUnit.ToString(), 5, ' ', true, false, true);
                            gtf.SetDataDetail(decSubTotSalesTypePenjualanBrutto.ToString(), 16, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotSalesTypeDiscount.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotSalesTypePenjualanNetto.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotSalesTypeBiaya.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotSalesTypeLabaRugi.ToString(), 15, ' ', true, false, true, true, "n0");

                            decTotUnit += decSubTotSalesTypeUnit;
                            decTotPenjualanBrutto += decSubTotSalesTypePenjualanBrutto;
                            decTotDiscount += decSubTotSalesTypeDiscount;
                            decTotPenjualanNetto += decSubTotSalesTypePenjualanNetto;
                            decTotBiaya += decSubTotSalesTypeBiaya;
                            decTotLabaRugi += decSubTotSalesTypeLabaRugi;
                            decTotPercentage += decSubTotSalesTypePercentage;

                            decTotPercentage = (decTotLabaRugi / decTotPenjualanNetto) * 100;
                            gtf.SetDataDetail(decTotPercentage.ToString(), 6, ' ', false, true, true, true, "n2");

                            salesType = (dt.Rows[i]["SalesType"].ToString() == "1") ? "DIRECTSALES" : "WHOLESALES";
                            gtf.SetDataDetailLineBreak();
                            gtf.SetDataDetail(salesType, 136, ' ', false, true);
                            branchCode = dt.Rows[i]["BranchCode"].ToString();
                            gtf.SetDataDetail(branchCode, 15, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["BranchName"].ToString(), 118, ' ', false, true);

                            gtf.PrintData(false);

                            decSubTotSalesTypeUnit = decSubTotSalesTypePenjualanBrutto = decSubTotSalesTypeDiscount = decSubTotSalesTypePenjualanNetto =
                                decSubTotSalesTypeBiaya = decSubTotSalesTypeLabaRugi = decSubTotSalesTypePercentage = 0;
                        }
                    }

                    if (branchCode == string.Empty)
                    {
                        branchCode = dt.Rows[i]["BranchCode"].ToString();
                        gtf.SetDataDetailSpace(2);
                        gtf.SetDataDetail(branchCode, 15, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["BranchName"].ToString(), 118, ' ', false, true);
                        gtf.PrintData(false);
                    }
                    else
                    {
                        if (branchCode != dt.Rows[i]["BranchCode"].ToString())
                        {
                            gtf.SetDataDetailSpace(21);
                            gtf.SetDataDetail("-", 114, '-', false, true);
                            //gtf.PrintData(false);

                            gtf.SetDataDetailSpace(5);
                            gtf.SetDataDetail("SUB TOTAL " + branchCode + " : ", 36, ' ', true, false, true);
                            gtf.SetDataDetail(decSubTotUnit.ToString(), 5, ' ', true, false, true);
                            gtf.SetDataDetail(decSubTotPenjualanBrutto.ToString(), 16, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotDiscount.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotPenjualanNetto.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotBiaya.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotLabaRugi.ToString(), 15, ' ', true, false, true, true, "n0");
                            decSubTotPercentage = (decSubTotLabaRugi / decSubTotPenjualanNetto) * 100;
                            gtf.SetDataDetail(decSubTotPercentage.ToString(), 6, ' ', false, true, true, true, "n2");

                            gtf.SetDataDetailLineBreak();
                            gtf.SetDataDetailSpace(2);
                            branchCode = dt.Rows[i]["BranchCode"].ToString();
                            gtf.SetDataDetail(branchCode, 15, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["BranchName"].ToString(), 118, ' ', false, true);
                            gtf.PrintData(false);

                            decSubTotSalesTypeUnit += decSubTotUnit;
                            decSubTotSalesTypePenjualanBrutto += decSubTotPenjualanBrutto;
                            decSubTotSalesTypeDiscount += decSubTotDiscount;
                            decSubTotSalesTypePenjualanNetto += decSubTotPenjualanNetto;
                            decSubTotSalesTypeBiaya += decSubTotBiaya;
                            decSubTotSalesTypeLabaRugi += decSubTotLabaRugi;
                            decSubTotSalesTypePercentage += decSubTotPercentage;

                            counterData = 0;
                            decSubTotUnit = decSubTotPenjualanBrutto = decSubTotDiscount = decSubTotPenjualanNetto = decSubTotBiaya =
                                decSubTotLabaRugi = decSubTotPercentage = 0;
                        }
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 36, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Unit"].ToString(), 5, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PenjualanBrutto"].ToString(), 16, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Discount"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Penjualan"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Biaya"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LabaRugi"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Percentage"].ToString(), 6, ' ', false, true, true, true, "n2");

                    decSubTotUnit += Convert.ToInt32(dt.Rows[i]["Unit"]);
                    decSubTotPenjualanBrutto += Convert.ToDecimal(dt.Rows[i]["PenjualanBrutto"]);
                    decSubTotDiscount += Convert.ToDecimal(dt.Rows[i]["Discount"]);
                    decSubTotPenjualanNetto += Convert.ToDecimal(dt.Rows[i]["Penjualan"]);
                    decSubTotBiaya += Convert.ToDecimal(dt.Rows[i]["Biaya"]);
                    decSubTotLabaRugi += Convert.ToDecimal(dt.Rows[i]["LabaRugi"]);

                    gtf.PrintData(false);
                }

                gtf.SetTotalDetailLine();
                gtf.SetTotalDetailSpace(5);
                gtf.SetTotalDetail("GRAND TOTAL :", 36, ' ', true, false, true);
                gtf.SetTotalDetail(decTotUnit.ToString(), 5, ' ', true, false, true);
                gtf.SetTotalDetail(decTotPenjualanBrutto.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotDiscount.ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotPenjualanNetto.ToString(), 15, ' ', true, false, true, true, "n0");
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