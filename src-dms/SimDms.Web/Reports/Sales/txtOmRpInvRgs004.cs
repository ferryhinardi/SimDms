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
    public class txtOmRpInvRgs004 : IRptProc  
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
            return CreateReportOmRpInvRgs004(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpInvRgs004(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Stok Kendaraan (OmRpInvRgs004/OmRpInvRgs004B)
            if (reportID == "OmRpInvRgs004" || reportID == "OmRpInvRgs004B")
            {
                #region Print Group Header
                gtf.SetGroupHeader(textParam[0].ToString(), 163, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();

                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeader("GUDANG", 26, ' ', true);
                gtf.SetGroupHeader("KODE MODEL", 16, ' ', true);
                gtf.SetGroupHeader("DESKRIPSI", 27, ' ', true);
                gtf.SetGroupHeader("TAHUN", 5, ' ', true, false, true);
                gtf.SetGroupHeader("WARNA", 33, ' ', true);
                gtf.SetGroupHeader("SALDO AWAL", 10, ' ', true, false, true);
                gtf.SetGroupHeader("MASUK", 6, ' ', true, false, true);
                gtf.SetGroupHeader("ALOKASI", 8, ' ', true, false, true);
                gtf.SetGroupHeader("KELUAR", 7, ' ', true, false, true);
                gtf.SetGroupHeader("SALDO AKHIR", 12, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                string salesModelCode = string.Empty, warehouseName = string.Empty;
                decimal decSubTotBeginningAV = 0, decSubTotQtyIn = 0, decSubTotAlocation = 0, decSubTotQtyOut = 0, decSubTotEndingAV = 0;
                decimal decSubTotWBeginningAV = 0, decSubTotWQtyIn = 0, decSubTotWAlocation = 0, decSubTotWQtyOut = 0, decSubTotWEndingAV = 0;
                decimal decTotBeginningAV = 0, decTotQtyIn = 0, decTotAlocation = 0, decTotQtyOut = 0, decTotEndingAV = 0;

                #endregion

                #region Print Detail

                #region Sort By Model (OmRpInvRgs004)
                if (reportID == "OmRpInvRgs004")
                {
                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            gtf.SetDataDetailSpace(82);
                            gtf.SetDataDetail("-", 81, '-', false, true);
                            gtf.SetDataDetailSpace(82);
                            gtf.SetDataDetail("Sub Total By Model :", 33, ' ', true);
                            gtf.SetDataDetail(decSubTotBeginningAV.ToString(), 10, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotQtyIn.ToString(), 6, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotAlocation.ToString(), 8, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotQtyOut.ToString(), 7, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotEndingAV.ToString(), 12, ' ', false, true, true, true, "n0");
                            gtf.SetDataDetailSpace(82);
                            gtf.SetDataDetail("-", 81, '-', false, true);

                            decTotBeginningAV += decSubTotBeginningAV;
                            decTotQtyIn += decSubTotQtyIn;
                            decTotAlocation += decSubTotAlocation;
                            decTotQtyOut += decSubTotQtyOut;
                            decTotEndingAV += decSubTotEndingAV;

                            gtf.PrintData(false);

                            break;
                        }

                        if (salesModelCode == string.Empty)
                        {
                            salesModelCode = dt.Rows[i]["SalesModelCode"].ToString();
                        }
                        else
                        {
                            if (salesModelCode != dt.Rows[i]["SalesModelCode"].ToString())
                            {
                                gtf.SetDataDetailSpace(82);
                                gtf.SetDataDetail("-", 81, '-', false, true);
                                gtf.SetDataDetailSpace(82);
                                gtf.SetDataDetail("Sub Total By Model :", 33, ' ', true);
                                gtf.SetDataDetail(decSubTotBeginningAV.ToString(), 10, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotQtyIn.ToString(), 6, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotAlocation.ToString(), 8, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotQtyOut.ToString(), 7, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotEndingAV.ToString(), 12, ' ', false, true, true, true, "n0");
                                gtf.SetDataDetailSpace(82);
                                gtf.SetDataDetail("-", 81, '-', false, true);

                                decTotBeginningAV += decSubTotBeginningAV;
                                decTotQtyIn += decSubTotQtyIn;
                                decTotAlocation += decSubTotAlocation;
                                decTotQtyOut += decSubTotQtyOut;
                                decTotEndingAV += decSubTotEndingAV;

                                gtf.PrintData(false);

                                counterData = 0;
                                decSubTotBeginningAV = decSubTotQtyIn = decSubTotAlocation = decSubTotQtyOut = decSubTotEndingAV = 0;
                                salesModelCode = dt.Rows[i]["SalesModelCode"].ToString();
                            }
                        }

                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["WarehouseName"].ToString(), 26, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 16, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 27, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["ModelYear"].ToString(), 5, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["ColourName"].ToString(), 33, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["BeginningAV"].ToString(), 10, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["QtyIn"].ToString(), 6, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["Alocation"].ToString(), 8, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["QtyOut"].ToString(), 7, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["EndingAV"].ToString(), 12, ' ', false, true, true, true, "n0");

                        decSubTotBeginningAV += Convert.ToDecimal(dt.Rows[i]["BeginningAV"]);
                        decSubTotQtyIn += Convert.ToDecimal(dt.Rows[i]["QtyIn"]);
                        decSubTotAlocation += Convert.ToDecimal(dt.Rows[i]["Alocation"]);
                        decSubTotQtyOut += Convert.ToDecimal(dt.Rows[i]["QtyOut"]);
                        decSubTotEndingAV += Convert.ToDecimal(dt.Rows[i]["EndingAV"]);

                        gtf.PrintData(false);
                    }
                }
                #endregion

                #region Sort By Gudang (OmRpInvRgs004B)
                if (reportID == "OmRpInvRgs004B")
                {
                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        if (i == dt.Rows.Count)
                        {
                            gtf.SetDataDetailSpace(82);
                            gtf.SetDataDetail("-", 81, '-', false, true);
                            gtf.SetDataDetailSpace(82);
                            gtf.SetDataDetail("Sub Total By Model :", 33, ' ', true);
                            gtf.SetDataDetail(decSubTotBeginningAV.ToString(), 10, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotQtyIn.ToString(), 6, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotAlocation.ToString(), 8, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotQtyOut.ToString(), 7, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotEndingAV.ToString(), 12, ' ', false, true, true, true, "n0");
                            gtf.SetDataDetailSpace(82);
                            gtf.SetDataDetail("-", 81, '-', false, true);

                            decSubTotWBeginningAV += decSubTotBeginningAV;
                            decSubTotWQtyIn += decSubTotQtyIn;
                            decSubTotWAlocation += decSubTotAlocation;
                            decSubTotWQtyOut += decSubTotQtyOut;
                            decSubTotWEndingAV += decSubTotEndingAV;

                            gtf.SetDataDetailSpace(82);
                            gtf.SetDataDetail("Sub Total By Warehouse :", 33, ' ', true);
                            gtf.SetDataDetail(decSubTotWBeginningAV.ToString(), 10, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotWQtyIn.ToString(), 6, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotWAlocation.ToString(), 8, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotWQtyOut.ToString(), 7, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotWEndingAV.ToString(), 12, ' ', false, true, true, true, "n0");
                            gtf.SetDataDetailLineBreak();

                            decTotBeginningAV += decSubTotWBeginningAV;
                            decTotQtyIn += decSubTotWQtyIn;
                            decTotAlocation += decSubTotWAlocation;
                            decTotQtyOut += decSubTotWQtyOut;
                            decTotEndingAV += decSubTotWEndingAV;

                            gtf.PrintData(false);

                            break;
                        }

                        if (salesModelCode == string.Empty && warehouseName == string.Empty)
                        {
                            salesModelCode = dt.Rows[i]["SalesModelCode"].ToString();
                            warehouseName = dt.Rows[i]["WarehouseName"].ToString();
                            counterData += 1;
                        }
                        else
                        {
                            if (salesModelCode != dt.Rows[i]["SalesModelCode"].ToString())
                            {
                                gtf.SetDataDetailSpace(82);
                                gtf.SetDataDetail("-", 81, '-', false, true);
                                gtf.SetDataDetailSpace(82);
                                gtf.SetDataDetail("Sub Total By Model :", 33, ' ', true);
                                gtf.SetDataDetail(decSubTotBeginningAV.ToString(), 10, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotQtyIn.ToString(), 6, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotAlocation.ToString(), 8, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotQtyOut.ToString(), 7, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotEndingAV.ToString(), 12, ' ', false, true, true, true, "n0");
                                gtf.SetDataDetailSpace(82);
                                gtf.SetDataDetail("-", 81, '-', false, true);

                                decSubTotWBeginningAV += decSubTotBeginningAV;
                                decSubTotWQtyIn += decSubTotQtyIn;
                                decSubTotWAlocation += decSubTotAlocation;
                                decSubTotWQtyOut += decSubTotQtyOut;
                                decSubTotWEndingAV += decSubTotEndingAV;

                                gtf.PrintData(false);

                                decSubTotBeginningAV = decSubTotQtyIn = decSubTotAlocation = decSubTotQtyOut = decSubTotEndingAV = 0;
                                salesModelCode = dt.Rows[i]["SalesModelCode"].ToString();
                            }

                            if (warehouseName != dt.Rows[i]["WarehouseName"].ToString())
                            {
                                gtf.SetDataDetailSpace(82);
                                gtf.SetDataDetail("Sub Total By Warehouse :", 33, ' ', true);
                                gtf.SetDataDetail(decSubTotWBeginningAV.ToString(), 10, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotWQtyIn.ToString(), 6, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotWAlocation.ToString(), 8, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotWQtyOut.ToString(), 7, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotWEndingAV.ToString(), 12, ' ', false, true, true, true, "n0");
                                gtf.SetDataDetailLineBreak();

                                decTotBeginningAV += decSubTotWBeginningAV;
                                decTotQtyIn += decSubTotWQtyIn;
                                decTotAlocation += decSubTotWAlocation;
                                decTotQtyOut += decSubTotWQtyOut;
                                decTotEndingAV += decSubTotWEndingAV;

                                gtf.PrintData(false);

                                counterData = 1;
                                decSubTotWBeginningAV = decSubTotWQtyIn = decSubTotWAlocation = decSubTotWQtyOut = decSubTotWEndingAV = 0;
                                warehouseName = dt.Rows[i]["WarehouseName"].ToString();
                            }
                        }

                        if (warehouseName == dt.Rows[i]["WarehouseName"].ToString())
                        {
                            gtf.SetDataDetail(counterData.ToString(), 3, ' ', true, false, true);
                            if (counterData > 1)
                            {
                                gtf.SetDataDetailSpace(27);
                            }
                            else
                            {
                                gtf.SetDataDetail(dt.Rows[i]["WarehouseName"].ToString(), 26, ' ', true);
                            }
                        }

                        gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 16, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 27, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["ModelYear"].ToString(), 5, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["ColourName"].ToString(), 33, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["BeginningAV"].ToString(), 10, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["QtyIn"].ToString(), 6, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["Alocation"].ToString(), 8, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["QtyOut"].ToString(), 7, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["EndingAV"].ToString(), 12, ' ', false, true, true, true, "n0");

                        decSubTotBeginningAV += Convert.ToDecimal(dt.Rows[i]["BeginningAV"]);
                        decSubTotQtyIn += Convert.ToDecimal(dt.Rows[i]["QtyIn"]);
                        decSubTotAlocation += Convert.ToDecimal(dt.Rows[i]["Alocation"]);
                        decSubTotQtyOut += Convert.ToDecimal(dt.Rows[i]["QtyOut"]);
                        decSubTotEndingAV += Convert.ToDecimal(dt.Rows[i]["EndingAV"]);

                        counterData += 1;

                        gtf.PrintData(false);
                    }
                }
                #endregion

                #endregion

                #region Print Footer
                gtf.SetTotalDetailLine();
                gtf.SetTotalDetailSpace(82);
                gtf.SetTotalDetail("Grand Total :", 33, ' ', true);
                gtf.SetTotalDetail(decTotBeginningAV.ToString(), 10, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotQtyIn.ToString(), 6, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotAlocation.ToString(), 8, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotQtyOut.ToString(), 7, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotEndingAV.ToString(), 12, ' ', false, true, true, true, "n0");
                gtf.SetTotalDetailLine();
                #endregion
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}