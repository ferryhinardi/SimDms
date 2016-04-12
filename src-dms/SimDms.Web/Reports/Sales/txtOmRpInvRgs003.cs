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
    public class txtOmRpInvRgs003 : IRptProc  
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
            return CreateReportOmRpInvRgs003(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpInvRgs003(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Stok Inventory (OmRpInvRgs003/OmRpInvRgs003A/OmRpInvRgs003B/OmRpInvRgs003C/OmRpInvRgs003D/OmRpInvRgs003E/OmRpInvRgs003F/OmRpInvRgs003G)
            if (reportID == "OmRpInvRgs003" || reportID == "OmRpInvRgs003A" || reportID == "OmRpInvRgs003B" || reportID == "OmRpInvRgs003C"
                || reportID == "OmRpInvRgs003D" || reportID == "OmRpInvRgs003E" || reportID == "OmRpInvRgs003F" || reportID == "OmRpInvRgs003G")
            {
                #region Print Group Header
                if (reportID == "OmRpInvRgs003E" || reportID == "OmRpInvRgs003F")
                {
                    gtf.SetGroupHeader(textParam[0].ToString(), 233, ' ', false, true, false, true);
                    gtf.SetGroupHeader(textParam[1].ToString(), 233, ' ', false, true, false, true);
                }
                else
                {
                    gtf.SetGroupHeader(textParam[0].ToString(), 163, ' ', false, true, false, true);
                    gtf.SetGroupHeader(textParam[1].ToString(), 163, ' ', false, true, false, true);
                }
                gtf.SetGroupHeaderLine();

                string warehouseCode = string.Empty, model = string.Empty;
                decimal decSubTotUnit = 0, decSubTotAfterDiscPPnBM = 0, decSubTotAfterDiscDPP = 0, decSubTotPPNMasukan = 0, decSubTotPPh = 0,
                    decSubTotOthersDPP = 0, decSubTotKaroseri = 0, decSubTotBiaya = 0, decSubTotal = 0;
                decimal decSubTotWUnit = 0, decSubTotWAfterDiscPPnBM = 0, decSubTotWAfterDiscDPP = 0, decSubTotWPPNMasukan = 0, decSubTotWPPh = 0,
                    decSubTotWOthersDPP = 0, decSubTotWKaroseri = 0, decSubTotWBiaya = 0, decSubTotWTotal = 0;
                decimal decTotUnit = 0, decTotAfterDiscPPnBM = 0, decTotAfterDiscDPP = 0, decTotPPNMasukan = 0, decTotPPh = 0, decTotOthersDPP = 0,
                    decTotKaroseri = 0, decTotBiaya = 0, decTotal = 0;
                int widthUnit = 0, widthPph = 0;
                int widthAfterDiscPPnBM = 0, widthAfterDiscDPP = 0, widthPPNMasukan = 0, widthOthersDPP = 0, widthKaroseri = 0, widthBiaya = 0,
                        widthTotal = 0;
                #endregion

                #region Print Detail

                #region BaseOn Stok (Inventory/Alokasi) SortBy Gudang (/w Colour Desc) (OmRpInvRgs003A/OmRpInvRgs003C/OmRpInvRgs003D/OmRpInvRgs003G)
                if (reportID == "OmRpInvRgs003A" || reportID == "OmRpInvRgs003C" || reportID == "OmRpInvRgs003D" || reportID == "OmRpInvRgs003G")
                {
                    widthUnit = 23; widthPph = 9;

                    widthAfterDiscPPnBM = 11;
                    widthAfterDiscDPP = 14;
                    widthPPNMasukan = 14;
                    widthOthersDPP = 9;
                    widthKaroseri = 8;
                    widthBiaya = 14;
                    widthTotal = 14;

                    #region Print Detail Group Header
                    if (reportID == "OmRpInvRgs003A" || reportID == "OmRpInvRgs003C")
                    {
                        gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
                        gtf.SetGroupHeader("WH", 3, ' ', true);
                        gtf.SetGroupHeader("MODEL", 22, ' ', true);
                        gtf.SetGroupHeader("PENERIMAAN", 25, ' ', false, true, false, true);
                        gtf.SetGroupHeaderSpace(30);
                        gtf.SetGroupHeader("-", 25, '-', false, true);
                        gtf.SetGroupHeaderSpace(8);
                        gtf.SetGroupHeader("WARNA", 5, ' ', true);
                        gtf.SetGroupHeader("CHASSIS", 7, ' ', true, false, true);
                        gtf.SetGroupHeader("MESIN", 7, ' ', true, false, true);
                        gtf.SetGroupHeader("NOMOR", 13, ' ', true);
                        gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
                        gtf.SetGroupHeader("PPN BM PAID", widthAfterDiscPPnBM, ' ', true, false, true);
                        gtf.SetGroupHeader("BELI", widthAfterDiscDPP, ' ', true, false, true);
                        gtf.SetGroupHeader("PPN MASUKAN", widthPPNMasukan, ' ', true, false, true);
                        gtf.SetGroupHeader("PPH", widthPph, ' ', true, false, true);
                        gtf.SetGroupHeader("LAIN-LAIN", widthOthersDPP, ' ', true, false, true);
                        gtf.SetGroupHeader("KAROSERI", widthKaroseri, ' ', true, false, true);
                        gtf.SetGroupHeader("BIAYA", widthBiaya, ' ', true, false, true);
                        gtf.SetGroupHeader("TOTAL", widthTotal, ' ', true, false, true);
                        gtf.SetGroupHeader("LAMA", 4, ' ', false, true, true);
                        gtf.SetGroupHeaderLine();
                        gtf.PrintHeader();
                    }
                    else
                    {
                        gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
                        gtf.SetGroupHeader("WH", 3, ' ', true);
                        gtf.SetGroupHeader("MODEL", 22, ' ', false, true);
                        gtf.SetGroupHeaderSpace(4);
                        gtf.SetGroupHeader("WARNA", 39, ' ', true);
                        gtf.SetGroupHeader("CHASSIS", 7, ' ', true, false, true);
                        gtf.SetGroupHeader("MESIN", 7, ' ', false, true, true);
                        gtf.SetGroupHeaderSpace(4);
                        gtf.SetGroupHeader("DO", 24, ' ', true, false, false, true);
                        gtf.SetGroupHeader("PENERIMAAN", 24, ' ', false, true, false, true);
                        gtf.SetGroupHeaderSpace(4);
                        gtf.SetGroupHeader("-", 24, '-', true);
                        gtf.SetGroupHeader("-", 25, '-', false, true);
                        gtf.SetGroupHeaderSpace(4);
                        gtf.SetGroupHeader("NOMOR", 13, ' ', true);
                        gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
                        gtf.SetGroupHeader("NOMOR", 13, ' ', true);
                        gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
                        gtf.SetGroupHeader("PPN BM PAID", widthAfterDiscPPnBM, ' ', true, false, true);
                        gtf.SetGroupHeader("BELI", widthAfterDiscDPP, ' ', true, false, true);
                        gtf.SetGroupHeader("PPN MASUKAN", widthPPNMasukan, ' ', true, false, true);
                        gtf.SetGroupHeader("PPH", widthPph, ' ', true, false, true);
                        gtf.SetGroupHeader("LAIN-LAIN", widthOthersDPP, ' ', true, false, true);
                        gtf.SetGroupHeader("KAROSERI", widthKaroseri, ' ', true, false, true);
                        gtf.SetGroupHeader("BIAYA", widthBiaya, ' ', true, false, true);
                        gtf.SetGroupHeader("TOTAL", widthTotal, ' ', true, false, true);
                        gtf.SetGroupHeader("LAMA", 4, ' ', false, true, true);
                        gtf.SetGroupHeaderLine();
                        gtf.PrintHeader();
                    }
                    #endregion

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        #region Print Sub Total
                        if (i == dt.Rows.Count)
                        {
                            if (decSubTotUnit > 1)
                            {
                                gtf.SetDataReportLine();
                                gtf.SetDataDetailSpace(16);
                                gtf.SetDataDetail("SUB TOTAL", 16, ' ');
                                gtf.SetDataDetail(" : " + decSubTotUnit.ToString() + " unit", 23, ' ', true);
                                gtf.SetDataDetail(decSubTotAfterDiscPPnBM.ToString(), widthAfterDiscPPnBM, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotAfterDiscDPP.ToString(), widthAfterDiscDPP, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotPPNMasukan.ToString(), widthPPNMasukan, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotPPh.ToString(), widthPph, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotOthersDPP.ToString(), widthOthersDPP, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotKaroseri.ToString(), widthKaroseri, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotBiaya.ToString(), widthBiaya, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotal.ToString(), widthTotal, ' ', false, true, true, true, "n0");
                                gtf.SetDataDetailLineBreak();
                            }

                            decSubTotWUnit += decSubTotUnit;
                            decSubTotWAfterDiscPPnBM += decSubTotAfterDiscPPnBM;
                            decSubTotWAfterDiscDPP += decSubTotAfterDiscDPP;
                            decSubTotWPPNMasukan += decSubTotPPNMasukan;
                            decSubTotWPPh += decSubTotPPh;
                            decSubTotWOthersDPP += decSubTotOthersDPP;
                            decSubTotWKaroseri += decSubTotKaroseri;
                            decSubTotWBiaya += decSubTotBiaya;
                            decSubTotWTotal += decSubTotal;

                            if (decSubTotWUnit > 1)
                            {
                                gtf.SetDataReportLine();
                                gtf.SetDataDetailSpace(16);
                                gtf.SetDataDetail("TOTAL WAREHOUSE", 16, ' ');
                                gtf.SetDataDetail(" : " + decSubTotWUnit.ToString() + " unit", widthUnit, ' ', true);
                                gtf.SetDataDetail(decSubTotWAfterDiscPPnBM.ToString(), widthAfterDiscPPnBM, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotWAfterDiscDPP.ToString(), widthAfterDiscDPP, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotWPPNMasukan.ToString(), widthPPNMasukan, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotWPPh.ToString(), widthPph, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotWOthersDPP.ToString(), widthOthersDPP, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotWKaroseri.ToString(), widthKaroseri, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotWBiaya.ToString(), widthBiaya, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotWTotal.ToString(), widthTotal, ' ', false, true, true, true, "n0");
                                gtf.SetDataDetailLineBreak();
                            }

                            decTotUnit += decSubTotWUnit;
                            decTotAfterDiscPPnBM += decSubTotWAfterDiscPPnBM;
                            decTotAfterDiscDPP += decSubTotWAfterDiscDPP;
                            decTotPPNMasukan += decSubTotWPPNMasukan;
                            decTotPPh += decSubTotWPPh;
                            decTotOthersDPP += decSubTotWOthersDPP;
                            decTotKaroseri += decSubTotWKaroseri;
                            decTotBiaya += decSubTotWBiaya;
                            decTotal += decSubTotWTotal;

                            gtf.PrintData(false);
                            break;
                        }

                        if (warehouseCode == string.Empty && model == string.Empty)
                        {
                            model = dt.Rows[i]["Model"].ToString();
                            warehouseCode = dt.Rows[i]["WarehouseCode"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 3, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["Model"].ToString(), 155, ' ', false, true);
                        }
                        else
                        {
                            if (model != dt.Rows[i]["Model"].ToString() || warehouseCode != dt.Rows[i]["WarehouseCode"].ToString())
                            {
                                if (decSubTotUnit > 1)
                                {
                                    gtf.SetDataReportLine();
                                    gtf.SetDataDetailSpace(16);
                                    gtf.SetDataDetail("SUB TOTAL", 16, ' ');
                                    gtf.SetDataDetail(" : " + decSubTotUnit.ToString() + " unit", widthUnit, ' ', true);
                                    gtf.SetDataDetail(decSubTotAfterDiscPPnBM.ToString(), widthAfterDiscPPnBM, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotAfterDiscDPP.ToString(), widthAfterDiscDPP, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotPPNMasukan.ToString(), widthPPNMasukan, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotPPh.ToString(), widthPph, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotOthersDPP.ToString(), widthOthersDPP, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotKaroseri.ToString(), widthKaroseri, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotBiaya.ToString(), widthBiaya, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotal.ToString(), widthTotal, ' ', false, true, true, true, "n0");
                                    gtf.SetDataDetailLineBreak();

                                    gtf.PrintData(false);
                                }

                                decSubTotWUnit += decSubTotUnit;
                                decSubTotWAfterDiscPPnBM += decSubTotAfterDiscPPnBM;
                                decSubTotWAfterDiscDPP += decSubTotAfterDiscDPP;
                                decSubTotWPPNMasukan += decSubTotPPNMasukan;
                                decSubTotWPPh += decSubTotPPh;
                                decSubTotWOthersDPP += decSubTotOthersDPP;
                                decSubTotWKaroseri += decSubTotKaroseri;
                                decSubTotWBiaya += decSubTotBiaya;
                                decSubTotWTotal += decSubTotal;

                                decSubTotUnit = decSubTotAfterDiscPPnBM = decSubTotAfterDiscDPP = decSubTotPPNMasukan = decSubTotPPh =
                                    decSubTotOthersDPP = decSubTotKaroseri = decSubTotBiaya = decSubTotal = 0;
                                model = dt.Rows[i]["Model"].ToString();

                                if (warehouseCode == dt.Rows[i]["WarehouseCode"].ToString())
                                {
                                    gtf.SetDataDetailSpace(4);
                                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 3, ' ', true);
                                    gtf.SetDataDetail(dt.Rows[i]["Model"].ToString(), 155, ' ', false, true);
                                }
                            }

                            if (warehouseCode != dt.Rows[i]["WarehouseCode"].ToString())
                            {
                                if (decSubTotWUnit > 1)
                                {
                                    gtf.SetDataReportLine();
                                    gtf.SetDataDetailSpace(16);
                                    gtf.SetDataDetail("TOTAL WAREHOUSE", 16, ' ');
                                    gtf.SetDataDetail(" : " + decSubTotWUnit.ToString() + " unit", widthUnit, ' ', true);
                                    gtf.SetDataDetail(decSubTotWAfterDiscPPnBM.ToString(), widthAfterDiscPPnBM, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotWAfterDiscDPP.ToString(), widthAfterDiscDPP, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotWPPNMasukan.ToString(), widthPPNMasukan, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotWPPh.ToString(), widthPph, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotWOthersDPP.ToString(), widthOthersDPP, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotWKaroseri.ToString(), widthKaroseri, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotWBiaya.ToString(), widthBiaya, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotWTotal.ToString(), widthTotal, ' ', false, true, true, true, "n0");

                                    gtf.PrintData(false);
                                }
                                gtf.SetDataDetailLineBreak();

                                decSubTotWUnit += decSubTotUnit;
                                decSubTotWAfterDiscPPnBM += decSubTotAfterDiscPPnBM;
                                decSubTotWAfterDiscDPP += decSubTotAfterDiscDPP;
                                decSubTotWPPNMasukan += decSubTotPPNMasukan;
                                decSubTotWPPh += decSubTotPPh;
                                decSubTotWOthersDPP += decSubTotOthersDPP;
                                decSubTotWKaroseri += decSubTotKaroseri;
                                decSubTotWBiaya += decSubTotBiaya;
                                decSubTotWTotal += decSubTotal;

                                decTotUnit += decSubTotWUnit;
                                decTotAfterDiscPPnBM += decSubTotWAfterDiscPPnBM;
                                decTotAfterDiscDPP += decSubTotWAfterDiscDPP;
                                decTotPPNMasukan += decSubTotWPPNMasukan;
                                decTotPPh += decSubTotWPPh;
                                decTotOthersDPP += decSubTotWOthersDPP;
                                decTotKaroseri += decSubTotWKaroseri;
                                decTotBiaya += decSubTotWBiaya;
                                decTotal += decSubTotWTotal;

                                decSubTotUnit = decSubTotAfterDiscPPnBM = decSubTotAfterDiscDPP = decSubTotPPNMasukan = decSubTotPPh =
                                    decSubTotOthersDPP = decSubTotKaroseri = decSubTotBiaya = decSubTotal = 0;
                                decSubTotWUnit = decSubTotWAfterDiscPPnBM = decSubTotWAfterDiscDPP = decSubTotWPPNMasukan = decSubTotWPPh =
                                    decSubTotWOthersDPP = decSubTotWKaroseri = decSubTotWBiaya = decSubTotWTotal = 0;
                                warehouseCode = dt.Rows[i]["WarehouseCode"].ToString();

                                counterData += 1;
                                gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                                gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 3, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["Model"].ToString(), 155, ' ', false, true);
                            }
                        }
                        #endregion

                        #region Print Data Detail
                        if (reportID == "OmRpInvRgs003A" || reportID == "OmRpInvRgs003C")
                        {
                            gtf.SetDataDetailSpace(8);
                            gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 5, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 7, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 7, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["BPUNo"].ToString(), 13, ' ', true);
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPUDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["AfterDiscPPnBM"].ToString(), widthAfterDiscPPnBM, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["AfterDiscDPP"].ToString(), widthAfterDiscDPP, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["PPNMasukan"].ToString(), widthPPNMasukan, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["PPh"].ToString(), widthPph, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["OthersDPP"].ToString(), widthOthersDPP, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Karoseri"].ToString(), widthKaroseri, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Biaya"].ToString(), widthBiaya, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Total"].ToString(), widthTotal, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Lama"].ToString(), 4, ' ', false, true, true);
                        }
                        else
                        {
                            gtf.SetDataDetailSpace(4);
                            gtf.SetDataDetail(dt.Rows[i]["Colour"].ToString(), 39, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 7, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 7, ' ', false, true, true);
                            gtf.SetDataDetailSpace(4);
                            gtf.SetDataDetail(dt.Rows[i]["RefferenceDONo"].ToString(), 13, ' ', true);
                            if (Convert.ToDateTime(dt.Rows[i]["RefferenceDODate"]).ToShortDateString() == "1/1/1900")
                                gtf.SetDataDetail("-", 11, ' ', true);
                            else
                                gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["RefferenceDODate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["BPUNo"].ToString(), 13, ' ', true);
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPUDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["AfterDiscPPnBM"].ToString(), widthAfterDiscPPnBM, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["AfterDiscDPP"].ToString(), widthAfterDiscDPP, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["PPNMasukan"].ToString(), widthPPNMasukan, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["PPh"].ToString(), widthPph, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["OthersDPP"].ToString(), widthOthersDPP, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Karoseri"].ToString(), widthKaroseri, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Biaya"].ToString(), widthBiaya, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Total"].ToString(), widthTotal, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Lama"].ToString(), 4, ' ', false, true, true);
                        }

                        decSubTotUnit += 1;
                        decSubTotAfterDiscPPnBM += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPnBM"]);
                        decSubTotAfterDiscDPP += Convert.ToDecimal(dt.Rows[i]["AfterDiscDPP"]);
                        decSubTotPPNMasukan += Convert.ToDecimal(dt.Rows[i]["PPNMasukan"]);
                        decSubTotPPh += Convert.ToDecimal(dt.Rows[i]["PPh"]);
                        decSubTotOthersDPP += Convert.ToDecimal(dt.Rows[i]["OthersDPP"]);
                        decSubTotKaroseri += Convert.ToDecimal(dt.Rows[i]["Karoseri"]);
                        decSubTotBiaya += Convert.ToDecimal(dt.Rows[i]["Biaya"]);
                        decSubTotal += Convert.ToDecimal(dt.Rows[i]["Total"]);

                        gtf.PrintData(false);
                        #endregion
                    }
                }
                #endregion

                #region BaseOn Stock (Inventory/Alokasi) SortBy Model (/w Colour Desc) (OmRpInvRgs003/OmRpInvRgs003B/OmRpInvRgs003E/OmRpInvRgs003F)
                if (reportID == "OmRpInvRgs003" || reportID == "OmRpInvRgs003B" || reportID == "OmRpInvRgs003E" || reportID == "OmRpInvRgs003F")
                {
                    #region Print Detail Group Header
                    if (reportID == "OmRpInvRgs003" || reportID == "OmRpInvRgs003B")
                    {
                        widthUnit = 29; widthPph = 5;

                        widthAfterDiscPPnBM = 11;
                        widthAfterDiscDPP = 14;
                        widthPPNMasukan = 14;
                        widthOthersDPP = 9;
                        widthKaroseri = 8;
                        widthBiaya = 14;
                        widthTotal = 14;

                        gtf.SetGroupHeaderSpace(37);
                        gtf.SetGroupHeader("PENERIMAAN", 25, ' ', false, true, false, true);
                        gtf.SetGroupHeaderSpace(37);
                        gtf.SetGroupHeader("-", 25, '-', false, true);
                        gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
                        gtf.SetGroupHeader("MODEL", 5, ' ', true);
                        gtf.SetGroupHeader("GUD", 3, ' ', true);
                        gtf.SetGroupHeader("WARNA", 5, ' ', true);
                        gtf.SetGroupHeader("CHASSIS", 7, ' ', true, false, true);
                        gtf.SetGroupHeader("MESIN", 7, ' ', true, false, true);
                        gtf.SetGroupHeader("NOMOR", 13, ' ', true);
                        gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
                        gtf.SetGroupHeader("PPN BM PAID", 11, ' ', true, false, true);
                        gtf.SetGroupHeader("BELI", 14, ' ', true, false, true);
                        gtf.SetGroupHeader("PPN MASUKAN", 14, ' ', true, false, true);
                        gtf.SetGroupHeader("PPH", 5, ' ', true, false, true);
                        gtf.SetGroupHeader("LAIN-LAIN", 9, ' ', true, false, true);
                        gtf.SetGroupHeader("KAROSERI", 8, ' ', true, false, true);
                        gtf.SetGroupHeader("BIAYA", 14, ' ', true, false, true);
                        gtf.SetGroupHeader("TOTAL", 14, ' ', true, false, true);
                        gtf.SetGroupHeader("LAMA", 4, ' ', false, true, true);
                        gtf.SetGroupHeaderLine();
                        gtf.PrintHeader();
                    }
                    else
                    {
                        widthUnit = 65; widthPph = 16;

                        widthAfterDiscPPnBM = 14;
                        widthAfterDiscDPP = 16;
                        widthPPNMasukan = 16;
                        widthOthersDPP = 13;
                        widthKaroseri = 11;
                        widthBiaya = 16;
                        widthTotal = 16;

                        gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
                        gtf.SetGroupHeaderSpace(4);
                        gtf.SetGroupHeader("MODEL", 25, ' ', false, true);
                        gtf.SetGroupHeaderSpace(8);
                        gtf.SetGroupHeader("GUD", 6, ' ', true);
                        gtf.SetGroupHeader("WARNA", 18, ' ', true);
                        gtf.SetGroupHeader("DO", 31, ' ', true, false, false, true);
                        gtf.SetGroupHeader("PENERIMAAN", 31, ' ', false, true, false, true);
                        gtf.SetGroupHeaderSpace(34);
                        gtf.SetGroupHeader("-", 31, '-', true);
                        gtf.SetGroupHeader("-", 31, '-', false, true);
                        gtf.SetGroupHeaderSpace(12);
                        gtf.SetGroupHeader("CHASSIS", 10, ' ', true, false, true);
                        gtf.SetGroupHeader("MESIN", 10, ' ', true, false, true);
                        gtf.SetGroupHeader("NOMOR", 16, ' ', true);
                        gtf.SetGroupHeader("TANGGAL", 14, ' ', true);
                        gtf.SetGroupHeader("NOMOR", 16, ' ', true);
                        gtf.SetGroupHeader("TANGGAL", 14, ' ', true);
                        gtf.SetGroupHeader("PPN BM PAID", 14, ' ', true, false, true);
                        gtf.SetGroupHeader("BELI", 16, ' ', true, false, true);
                        gtf.SetGroupHeader("PPN MASUKAN", 16, ' ', true, false, true);
                        gtf.SetGroupHeader("PPH", 16, ' ', true, false, true);
                        gtf.SetGroupHeader("LAIN-LAIN", 13, ' ', true, false, true);
                        gtf.SetGroupHeader("KAROSERI", 11, ' ', true, false, true);
                        gtf.SetGroupHeader("BIAYA", 16, ' ', true, false, true);
                        gtf.SetGroupHeader("TOTAL", 16, ' ', true, false, true);
                        gtf.SetGroupHeader("LAMA", 9, ' ', false, true, true);
                        gtf.SetGroupHeaderLine();
                        gtf.PrintHeader();
                    }
                    #endregion

                    for (int i = 0; i <= dt.Rows.Count; i++)
                    {
                        #region Print Sub Total
                        if (i == dt.Rows.Count)
                        {
                            if (decSubTotUnit > 1)
                            {
                                gtf.SetDataReportLine();
                                gtf.SetDataDetailSpace(16);
                                gtf.SetDataDetail("SUB TOTAL", 16, ' ');
                                gtf.SetDataDetail(" : " + decSubTotUnit.ToString() + " unit", widthUnit, ' ', true);
                                gtf.SetDataDetail(decSubTotAfterDiscPPnBM.ToString(), widthAfterDiscPPnBM, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotAfterDiscDPP.ToString(), widthAfterDiscDPP, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotPPNMasukan.ToString(), widthPPNMasukan, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotPPh.ToString(), widthPph, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotOthersDPP.ToString(), widthOthersDPP, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotKaroseri.ToString(), widthKaroseri, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotBiaya.ToString(), widthBiaya, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(decSubTotal.ToString(), widthTotal, ' ', false, true, true, true, "n0");
                                gtf.SetDataDetailLineBreak();
                            }

                            decTotUnit += decSubTotUnit;
                            decTotAfterDiscPPnBM += decSubTotAfterDiscPPnBM;
                            decTotAfterDiscDPP += decSubTotAfterDiscDPP;
                            decTotPPNMasukan += decSubTotPPNMasukan;
                            decTotPPh += decSubTotPPh;
                            decTotOthersDPP += decSubTotOthersDPP;
                            decTotKaroseri += decSubTotKaroseri;
                            decTotBiaya += decSubTotBiaya;
                            decTotal += decSubTotal;

                            gtf.PrintData(false);
                            break;
                        }

                        if (model == string.Empty)
                        {
                            model = dt.Rows[i]["Model"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);

                            if (reportID == "OmRpInvRgs003E" || reportID == "OmRpInvRgs003F")
                            {
                                gtf.SetDataDetailSpace(4);
                            }

                            gtf.SetDataDetail(dt.Rows[i]["Model"].ToString(), 159, ' ', false, true);
                        }
                        else
                        {
                            if (model != dt.Rows[i]["Model"].ToString())
                            {
                                if (decSubTotUnit > 1)
                                {
                                    gtf.SetDataReportLine();
                                    gtf.SetDataDetailSpace(16);
                                    gtf.SetDataDetail("SUB TOTAL", 16, ' ');
                                    gtf.SetDataDetail(" : " + decSubTotUnit.ToString() + " unit", widthUnit, ' ', true);
                                    gtf.SetDataDetail(decSubTotAfterDiscPPnBM.ToString(), widthAfterDiscPPnBM, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotAfterDiscDPP.ToString(), widthAfterDiscDPP, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotPPNMasukan.ToString(), widthPPNMasukan, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotPPh.ToString(), widthPph, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotOthersDPP.ToString(), widthOthersDPP, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotKaroseri.ToString(), widthKaroseri, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotBiaya.ToString(), widthBiaya, ' ', true, false, true, true, "n0");
                                    gtf.SetDataDetail(decSubTotal.ToString(), widthTotal, ' ', false, true, true, true, "n0");
                                    gtf.SetDataDetailLineBreak();

                                    gtf.PrintData(false);
                                }
                                else
                                {
                                    gtf.SetDataReportLine();
                                    gtf.SetDataDetailLineBreak();
                                }

                                decTotUnit += decSubTotUnit;
                                decTotAfterDiscPPnBM += decSubTotAfterDiscPPnBM;
                                decTotAfterDiscDPP += decSubTotAfterDiscDPP;
                                decTotPPNMasukan += decSubTotPPNMasukan;
                                decTotPPh += decSubTotPPh;
                                decTotOthersDPP += decSubTotOthersDPP;
                                decTotKaroseri += decSubTotKaroseri;
                                decTotBiaya += decSubTotBiaya;
                                decTotal += decSubTotal;

                                decSubTotUnit = decSubTotAfterDiscPPnBM = decSubTotAfterDiscDPP = decSubTotPPNMasukan = decSubTotPPh =
                                    decSubTotOthersDPP = decSubTotKaroseri = decSubTotBiaya = decSubTotal = 0;
                                model = dt.Rows[i]["Model"].ToString();
                                counterData += 1;
                                gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                                if (reportID == "OmRpInvRgs003E" || reportID == "OmRpInvRgs003F")
                                {
                                    gtf.SetDataDetailSpace(4);
                                }
                                gtf.SetDataDetail(dt.Rows[i]["Model"].ToString(), 159, ' ', false, true);
                            }
                        }
                        #endregion

                        #region Print Data Detail
                        if (reportID == "OmRpInvRgs003" || reportID == "OmRpInvRgs003B")
                        {
                            gtf.SetDataDetailSpace(10);
                            gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 3, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 5, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 7, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 7, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["BPUNo"].ToString(), 13, ' ', true);
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPUDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["AfterDiscPPnBM"].ToString(), 11, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["AfterDiscDPP"].ToString(), 14, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["PPNMasukan"].ToString(), 14, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["PPh"].ToString(), 5, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["OthersDPP"].ToString(), 9, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Karoseri"].ToString(), 8, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Biaya"].ToString(), 14, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Total"].ToString(), 14, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Lama"].ToString(), 4, ' ', false, true, true);
                        }
                        else
                        {
                            gtf.SetDataDetailSpace(8);
                            gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 6, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["Colour"].ToString(), 218, ' ', false, true);
                            gtf.SetDataDetailSpace(12);
                            gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 10, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 10, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["RefferenceDONo"].ToString(), 16, ' ', true);
                            if (Convert.ToDateTime(dt.Rows[i]["RefferenceDODate"]).ToShortDateString() == "1/1/1900")
                                gtf.SetDataDetail("-", 14, ' ', true);
                            else
                                gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["RefferenceDODate"]).ToString("dd-MMM-yyyy"), 14, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["BPUNo"].ToString(), 16, ' ', true);
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPUDate"]).ToString("dd-MMM-yyyy"), 14, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["AfterDiscPPnBM"].ToString(), 14, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["AfterDiscDPP"].ToString(), 16, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["PPNMasukan"].ToString(), 16, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["PPh"].ToString(), 16, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["OthersDPP"].ToString(), 13, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Karoseri"].ToString(), 11, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Biaya"].ToString(), 16, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Total"].ToString(), 16, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt.Rows[i]["Lama"].ToString(), 9, ' ', false, true, true);
                        }

                        decSubTotUnit += 1;
                        decSubTotAfterDiscPPnBM += Convert.ToDecimal(dt.Rows[i]["AfterDiscPPnBM"]);
                        decSubTotAfterDiscDPP += Convert.ToDecimal(dt.Rows[i]["AfterDiscDPP"]);
                        decSubTotPPNMasukan += Convert.ToDecimal(dt.Rows[i]["PPNMasukan"]);
                        decSubTotPPh += Convert.ToDecimal(dt.Rows[i]["PPh"]);
                        decSubTotOthersDPP += Convert.ToDecimal(dt.Rows[i]["OthersDPP"]);
                        decSubTotKaroseri += Convert.ToDecimal(dt.Rows[i]["Karoseri"]);
                        decSubTotBiaya += Convert.ToDecimal(dt.Rows[i]["Biaya"]);
                        decSubTotal += Convert.ToDecimal(dt.Rows[i]["Total"]);

                        gtf.PrintData(false);
                        #endregion
                    }
                }
                #endregion

                #endregion

                #region Print Footer
                gtf.SetTotalDetailLine();
                gtf.SetTotalDetail("GRAND TOTAL", 32, ' ');
                gtf.SetTotalDetail(" : " + decTotUnit.ToString() + " unit", widthUnit, ' ', true);
                gtf.SetTotalDetail(decTotAfterDiscPPnBM.ToString(), widthAfterDiscPPnBM, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotAfterDiscDPP.ToString(), widthAfterDiscDPP, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotPPNMasukan.ToString(), widthPPNMasukan, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotPPh.ToString(), widthPph, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotOthersDPP.ToString(), widthOthersDPP, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotKaroseri.ToString(), widthKaroseri, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotBiaya.ToString(), widthBiaya, ' ', true, false, true, true, "n0");
                gtf.SetTotalDetail(decTotal.ToString(), widthTotal, ' ', false, true, true, true, "n0");
                gtf.SetTotalDetailLine();
                #endregion
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}