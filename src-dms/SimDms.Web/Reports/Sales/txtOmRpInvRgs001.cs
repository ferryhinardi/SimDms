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
    public class txtOmRpInvRgs001 : IRptProc  
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
            return CreateReportOmRpInvRgs001(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpInvRgs001(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Perincian Stok (OmRpInvRgs001/OmRpInvRgs001A/OmRpInvRgs002/OmRpInvRgs002A)
            if (reportID == "OmRpInvRgs001" || reportID == "OmRpInvRgs001A" || reportID == "OmRpInvRgs002" || reportID == "OmRpInvRgs002A")
            {
                #region Print Group Header
                gtf.SetGroupHeader(textParam[0].ToString(), 233, ' ', false, true, false, true);
                if (reportID == "OmRpInvRgs001" || reportID == "OmRpInvRgs001A")
                    gtf.SetGroupHeader("URUT : GUDANG", 233, ' ', false, true, false, true);
                else
                    gtf.SetGroupHeader("URUT : MODEL", 233, ' ', false, true, false, true);
                gtf.SetGroupHeaderLine();

                string warehouseCode = string.Empty, model = string.Empty, warna = string.Empty;
                int intSubTotUnit = 0, intSubTotWUnit = 0, intTotUnit = 0;

                if (reportID == "OmRpInvRgs001" || reportID == "OmRpInvRgs001A")
                {
                    gtf.SetGroupHeaderSpace(54);
                    gtf.SetGroupHeader("W R S (B P U)", 42, ' ', true, false, false, true);
                    gtf.SetGroupHeader("DO PEMASOK", 24, ' ', true, false, false, true);
                    gtf.SetGroupHeader("SRT JLN PEMASOK", 24, ' ', true, false, false, true);
                    gtf.SetGroupHeader("ALOKASI", 57, ' ', false, true, false, true);

                    gtf.SetGroupHeaderSpace(54);
                    gtf.SetGroupHeader("-", 42, '-', true);
                    gtf.SetGroupHeader("-", 24, '-', true);
                    gtf.SetGroupHeader("-", 24, '-', true);
                    gtf.SetGroupHeader("-", 57, '-', true);
                    gtf.SetGroupHeader("DOK", 10, ' ', false, true);

                    gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(2);
                    gtf.SetGroupHeader("GUD", 4, ' ', true);
                    gtf.SetGroupHeader("MODEL", 7, ' ', true);
                    gtf.SetGroupHeader("WARNA", 7, ' ', true);
                    gtf.SetGroupHeader("CHASSIS", 9, ' ', true);
                    gtf.SetGroupHeader("MESIN", 9, ' ', true);
                    gtf.SetGroupHeader("THN", 6, ' ', true, false, true);

                    gtf.SetGroupHeader("NO DOKUMEN", 14, ' ', true);
                    gtf.SetGroupHeader("TGL BPU DO", 13, ' ', true);
                    gtf.SetGroupHeader("TGL BPU SJ", 13, ' ', true);

                    gtf.SetGroupHeader("NO DOK", 10, ' ', true);
                    gtf.SetGroupHeader("TANGGAL", 13, ' ', true);
                    gtf.SetGroupHeader("NO. DOK", 10, ' ', true);
                    gtf.SetGroupHeader("TANGGAL", 13, ' ', true);

                    gtf.SetGroupHeader("DOKUMEN DO", 14, ' ', true);
                    gtf.SetGroupHeader("TANGGAL", 13, ' ', true);
                    gtf.SetGroupHeader("DOKUMEN BPK", 14, ' ', true);
                    gtf.SetGroupHeader("TGL. BPK", 13, ' ', true);

                    gtf.SetGroupHeader("KAROSERI", 10, ' ', true);
                    gtf.SetGroupHeader("STATUS", 7, ' ', true);
                    gtf.SetGroupHeader("PDI", 4, ' ', true);
                    gtf.SetGroupHeader("LAMA", 4, ' ', false, true);

                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();
                }
                //else if (reportID == "OmRpInvRgs001A")
                //{
                //    gtf.SetGroupHeaderSpace(12);
                //    gtf.SetGroupHeader("MODEL", 17, ' ', true);
                //    gtf.SetGroupHeaderSpace(27);
                //    gtf.SetGroupHeader("WRS", 29, ' ', true, false, false, true);
                //    gtf.SetGroupHeader("DO", 12, ' ', true);
                //    gtf.SetGroupHeader("SRT JLN PEMASOK", 29, ' ', true, false, false, true);
                //    gtf.SetGroupHeader("ALOKASI", 59, ' ', true, false, false, true);
                //    gtf.SetGroupHeader("DOK", 12, ' ', false, true);

                //    gtf.SetGroupHeaderSpace(12);
                //    gtf.SetGroupHeader("-", 17, '-', true);
                //    gtf.SetGroupHeader("-", 9, '-', true);
                //    gtf.SetGroupHeader("-", 9, '-', true);
                //    gtf.SetGroupHeaderSpace(7);
                //    gtf.SetGroupHeader("-", 29, '-', true);
                //    gtf.SetGroupHeaderSpace(13);
                //    gtf.SetGroupHeader("-", 29, '-', true);
                //    gtf.SetGroupHeader("-", 59, '-', false, true);
                //    gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
                //    gtf.SetGroupHeaderSpace(2);
                //    gtf.SetGroupHeader("GUD", 5, ' ', true);
                //    gtf.SetGroupHeader("WARNA", 17, ' ', true);
                //    gtf.SetGroupHeader("CHASSIS", 9, ' ', true, false, true);
                //    gtf.SetGroupHeader("MESIN", 9, ' ', true, false, true);
                //    gtf.SetGroupHeader("THN", 6, ' ', true, false, true);
                //    gtf.SetGroupHeader("NO DOKUMEN", 15, ' ', true);
                //    gtf.SetGroupHeader("TANGGAL", 13, ' ', true);
                //    gtf.SetGroupHeader("PEMASOK", 12, ' ', true);
                //    gtf.SetGroupHeader("NO. DOK", 15, ' ', true);
                //    gtf.SetGroupHeader("TANGGAL", 13, ' ', true);
                //    gtf.SetGroupHeader("DOKUMEN DO", 15, ' ', true);
                //    gtf.SetGroupHeader("TANGGAL", 13, ' ', true);
                //    gtf.SetGroupHeader("DOKUMEN BPK", 15, ' ', true);
                //    gtf.SetGroupHeader("TGL. BPK", 13, ' ', true);
                //    gtf.SetGroupHeader("KAROSERI", 15, ' ', true);
                //    gtf.SetGroupHeader("STATUS", 8, ' ', true);
                //    gtf.SetGroupHeader("PDI", 5, ' ', true);
                //    gtf.SetGroupHeader("LAMA", 4, ' ', false, true);
                //    gtf.SetGroupHeaderLine();
                //    gtf.PrintHeader();
                //}
                else if (reportID == "OmRpInvRgs002A")
                {
                    gtf.SetGroupHeaderSpace(27);
                    gtf.SetGroupHeader("WARNA", 26, ' ', true);
                    gtf.SetGroupHeader("W R S (B P U)", 42, ' ', true, false, false, true);
                    gtf.SetGroupHeader("DO PEMASOK", 24, ' ', true, false, false, true);
                    gtf.SetGroupHeader("SRT JLN PEMASOK", 24, ' ', true, false, false, true);
                    gtf.SetGroupHeader("ALOKASI", 57, ' ', false, true, false, true);

                    gtf.SetGroupHeaderSpace(27);
                    gtf.SetGroupHeader("-", 26, '-', true);
                    gtf.SetGroupHeader("-", 42, '-', true);
                    gtf.SetGroupHeader("-", 24, '-', true);
                    gtf.SetGroupHeader("-", 24, '-', true);
                    gtf.SetGroupHeader("-", 57, '-', true);
                    gtf.SetGroupHeader("DOK", 10, ' ', false, true);

                    gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(2);
                    gtf.SetGroupHeader("MODEL", 15, ' ', true);
                    gtf.SetGroupHeader("GUD", 4, ' ', true);

                    gtf.SetGroupHeader("CHASSIS", 9, ' ', true);
                    gtf.SetGroupHeader("MESIN", 9, ' ', true);
                    gtf.SetGroupHeader("THN", 6, ' ', true, false, true);

                    gtf.SetGroupHeader("NO DOKUMEN", 14, ' ', true);
                    gtf.SetGroupHeader("TGL BPU DO", 13, ' ', true);
                    gtf.SetGroupHeader("TGL BPU SJ", 13, ' ', true);

                    gtf.SetGroupHeader("NO DOK", 10, ' ', true);
                    gtf.SetGroupHeader("TANGGAL", 13, ' ', true);
                    gtf.SetGroupHeader("NO. DOK", 10, ' ', true);
                    gtf.SetGroupHeader("TANGGAL", 13, ' ', true);

                    gtf.SetGroupHeader("DOKUMEN DO", 14, ' ', true);
                    gtf.SetGroupHeader("TANGGAL", 13, ' ', true);
                    gtf.SetGroupHeader("DOKUMEN BPK", 14, ' ', true);
                    gtf.SetGroupHeader("TGL. BPK", 13, ' ', true);

                    gtf.SetGroupHeader("KAROSERI", 10, ' ', true);
                    gtf.SetGroupHeader("STATUS", 7, ' ', true);
                    gtf.SetGroupHeader("PDI", 4, ' ', true);
                    gtf.SetGroupHeader("LAMA", 4, ' ', false, true);

                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();
                }
                else
                {
                    gtf.SetGroupHeaderSpace(54);
                    gtf.SetGroupHeader("W R S (B P U)", 42, ' ', true, false, false, true);
                    gtf.SetGroupHeader("DO PEMASOK", 24, ' ', true, false, false, true);
                    gtf.SetGroupHeader("SRT JLN PEMASOK", 24, ' ', true, false, false, true);
                    gtf.SetGroupHeader("ALOKASI", 57, ' ', false, true, false, true);

                    gtf.SetGroupHeaderSpace(54);
                    gtf.SetGroupHeader("-", 42, '-', true);
                    gtf.SetGroupHeader("-", 24, '-', true);
                    gtf.SetGroupHeader("-", 24, '-', true);
                    gtf.SetGroupHeader("-", 57, '-', true);
                    gtf.SetGroupHeader("DOK", 10, ' ', false, true);

                    gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(2);
                    gtf.SetGroupHeader("MODEL", 7, ' ', true);
                    gtf.SetGroupHeader("WARNA", 7, ' ', true);
                    gtf.SetGroupHeader("GUD", 4, ' ', true);
                    gtf.SetGroupHeader("CHASSIS", 9, ' ', true);
                    gtf.SetGroupHeader("MESIN", 9, ' ', true);
                    gtf.SetGroupHeader("THN", 6, ' ', true, false, true);

                    gtf.SetGroupHeader("NO DOKUMEN", 14, ' ', true);
                    gtf.SetGroupHeader("TGL BPU DO", 13, ' ', true);
                    gtf.SetGroupHeader("TGL BPU SJ", 13, ' ', true);

                    gtf.SetGroupHeader("NO DOK", 10, ' ', true);
                    gtf.SetGroupHeader("TANGGAL", 13, ' ', true);
                    gtf.SetGroupHeader("NO. DOK", 10, ' ', true);
                    gtf.SetGroupHeader("TANGGAL", 13, ' ', true);

                    gtf.SetGroupHeader("DOKUMEN DO", 14, ' ', true);
                    gtf.SetGroupHeader("TANGGAL", 13, ' ', true);
                    gtf.SetGroupHeader("DOKUMEN BPK", 14, ' ', true);
                    gtf.SetGroupHeader("TGL. BPK", 13, ' ', true);

                    gtf.SetGroupHeader("KAROSERI", 10, ' ', true);
                    gtf.SetGroupHeader("STATUS", 7, ' ', true);
                    gtf.SetGroupHeader("PDI", 4, ' ', true);
                    gtf.SetGroupHeader("LAMA", 4, ' ', false, true);

                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();
                }
                #endregion

                #region Print Detail
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    #region Base On Gudang (OmRpInvRgs001 / OmRpInvRgs001A)
                    if (reportID == "OmRpInvRgs001" || reportID == "OmRpInvRgs001A")
                    {
                        #region Print Sub Total
                        if (i == dt.Rows.Count)
                        {
                            gtf.SetDataDetailSpace(6);
                            gtf.SetDataDetail("-", 227, '-', false, true);
                            gtf.SetDataDetailSpace(6);
                            gtf.SetDataDetail("Total unit di Gudang : " + warehouseCode + " untuk Model : " + model, 88, ' ', true);
                            gtf.SetDataDetail(intSubTotUnit.ToString() + " UNIT", 13, ' ', false, true, true);
                            gtf.SetDataDetail("", 227, ' ', false, true);

                            intSubTotWUnit += intSubTotUnit;

                            gtf.SetDataDetailSpace(6);
                            gtf.SetDataDetail("-", 227, '-', false, true);
                            gtf.SetDataDetailSpace(6);
                            gtf.SetDataDetail("Total unit di Gudang : " + warehouseCode, 41, ' ', true);
                            gtf.SetDataDetail(intSubTotWUnit.ToString() + " UNIT", 17, ' ', false, true, true);
                            gtf.SetDataDetail("", 227, ' ', false, true);

                            intTotUnit += intSubTotWUnit;

                            gtf.PrintData(false);
                            break;
                        }

                        if (warehouseCode == string.Empty && model == string.Empty)
                        {
                            model = dt.Rows[i]["Model"].ToString();
                            warehouseCode = dt.Rows[i]["Gudang"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                            gtf.SetDataDetailSpace(2);
                            gtf.SetDataDetail(warehouseCode, 4, ' ', true);
                            if (reportID == "OmRpInvRgs001")
                            {
                                gtf.SetDataDetail(model, 221, ' ', false, true);
                                gtf.PrintData(false);
                            }
                            else
                            {
                                gtf.SetDataDetail(model, 15, ' ', true);
                            }
                            //if (reportID == "OmRpInvRgs001A") gtf.SetDataDetail(model, 17, ' ', true);
                        }
                        else
                        {
                            if (model != dt.Rows[i]["Model"].ToString() || warehouseCode != dt.Rows[i]["Gudang"].ToString())
                            {
                                gtf.SetDataDetailSpace(6);
                                gtf.SetDataDetail("-", 227, '-', false, true);
                                gtf.SetDataDetailSpace(6);
                                gtf.SetDataDetail("Total unit di Gudang : " + warehouseCode + " untuk Model : " + model, 88, ' ', true);
                                gtf.SetDataDetail(intSubTotUnit.ToString() + " UNIT", 13, ' ', false, true, true);
                                gtf.SetDataDetail("", 227, ' ', false, true);
                                gtf.PrintData(false);

                                intSubTotWUnit += intSubTotUnit;

                                intSubTotUnit = 0;
                                model = dt.Rows[i]["Model"].ToString();

                                if (warehouseCode != dt.Rows[i]["Gudang"].ToString())
                                {
                                    gtf.SetDataDetailSpace(6);
                                    gtf.SetDataDetail("-", 227, '-', false, true);
                                    gtf.SetDataDetailSpace(6);
                                    gtf.SetDataDetail("Total unit di Gudang : " + warehouseCode, 41, ' ', true);
                                    gtf.SetDataDetail(intSubTotWUnit.ToString() + " UNIT", 17, ' ', false, true, true);
                                    gtf.SetDataDetail("", 227, ' ', false, true);
                                    gtf.PrintData(false);

                                    intSubTotWUnit += intSubTotUnit;
                                    intTotUnit += intSubTotWUnit;

                                    intSubTotUnit = intSubTotWUnit = 0;

                                    warehouseCode = dt.Rows[i]["Gudang"].ToString();
                                    counterData += 1;
                                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                                    gtf.SetDataDetailSpace(2);
                                    gtf.SetDataDetail(warehouseCode, 4, ' ', true);

                                    if (reportID == "OmRpInvRgs001")
                                    {
                                        gtf.SetDataDetail(model, 221, ' ', false, true);
                                        gtf.PrintData(false);
                                    }
                                    else
                                    {
                                        gtf.SetDataDetail(model, 15, ' ', true);
                                    }
                                    //if (reportID == "OmRpInvRgs001A") gtf.SetDataDetail(model, 17, ' ', true);
                                }
                                else
                                {
                                    gtf.SetDataDetailSpace(11);
                                    if (reportID == "OmRpInvRgs001")
                                    {
                                        gtf.SetDataDetail(model, 221, ' ', false, true);
                                        gtf.PrintData(false);
                                    }
                                    else
                                    {
                                        gtf.SetDataDetail(model, 15, ' ', true);
                                    }
                                }

                                //if (warehouseCode == dt.Rows[i]["Gudang"].ToString())
                                //{
                                //    gtf.SetDataDetailSpace(12);
                                //}

                                //if (reportID == "OmRpInvRgs001A") gtf.SetDataDetail(model, 17, ' ', true);
                            }
                        }
                        #endregion

                        #region Detail
                        //if (intSubTotUnit > 0)
                        //{
                        if (warehouseCode == dt.Rows[i]["Gudang"].ToString())
                        {
                            if (reportID == "OmRpInvRgs001")
                                gtf.SetDataDetailSpace(19);
                            else
                            {
                                if (intSubTotUnit > 0)
                                    gtf.SetDataDetailSpace(27);
                            }
                        }

                        //if (reportID == "OmRpInvRgs001A")
                        //{
                        //    if (model == dt.Rows[i]["Model"].ToString())
                        //    {
                        //        gtf.SetDataDetailSpace(18);
                        //    }
                        //}
                        //}

                        if (reportID == "OmRpInvRgs001")
                        {
                            gtf.SetDataDetail(dt.Rows[i]["Warna"].ToString(), 7, ' ', true);
                        }
                        gtf.SetDataDetail(dt.Rows[i]["Chassis"].ToString(), 9, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Mesin"].ToString(), 9, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Tahun"].ToString(), 6, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["BPUNo"].ToString(), 14, ' ', true);
                        if (Convert.ToDateTime(dt.Rows[i]["BPUDate"]).ToShortDateString() == "1/1/1900")
                            gtf.SetDataDetail("", 13, ' ', true);
                        else
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPUDate"]).ToString("dd-MMM-yyyy"), 13, ' ', true);
                        if (Convert.ToDateTime(dt.Rows[i]["BPUSJDate"]).ToShortDateString() == "1/1/1900")
                            gtf.SetDataDetail("", 13, ' ', true);
                        else
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPUSJDate"]).ToString("dd-MMM-yyyy"), 13, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["ReffDONo"].ToString(), 10, ' ', true);
                        if (Convert.ToDateTime(dt.Rows[i]["ReffDODate"]).ToShortDateString() == "1/1/1900")
                            gtf.SetDataDetail("", 13, ' ', true);
                        else
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["ReffDODate"]).ToString("dd-MMM-yyyy"), 13, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SJNo"].ToString(), 10, ' ', true);
                        if (Convert.ToDateTime(dt.Rows[i]["SJDate"]).ToShortDateString() == "1/1/1900")
                            gtf.SetDataDetail("", 13, ' ', true);
                        else
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["SJDate"]).ToString("dd-MMM-yyyy"), 13, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["DONo"].ToString(), 14, ' ', true);
                        if (Convert.ToDateTime(dt.Rows[i]["DODate"]).ToShortDateString() == "1/1/1900")
                            gtf.SetDataDetail("", 13, ' ', true);
                        else
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DODate"]).ToString("dd-MMM-yyyy"), 13, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["BPKNo"].ToString(), 14, ' ', true);
                        if (Convert.ToDateTime(dt.Rows[i]["BPKDate"]).ToShortDateString() == "1/1/1900")
                            gtf.SetDataDetail("", 13, ' ', true);
                        else
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPKDate"]).ToString("dd-MMM-yyyy"), 13, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Karoseri"].ToString(), 10, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Status"].ToString(), 7, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["PDI"].ToString(), 4, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Lama"].ToString(), 4, ' ', false, true, true);
                        if (reportID == "OmRpInvRgs001A")
                        {
                            gtf.SetDataDetailSpace(19);
                            gtf.SetDataDetail(dt.Rows[i]["Colour"].ToString(), 214, ' ', false, true);
                        }

                        intSubTotUnit += 1;

                        gtf.PrintData(false);
                        #endregion
                    }
                    #endregion

                    #region Base On Model (OmRpInvRgs002 / OmRpInvRgs002A)
                    else
                    {
                        if (i == dt.Rows.Count)
                        {
                            gtf.SetDataDetailSpace(6);
                            gtf.SetDataDetail("-", 227, '-', false, true);
                            gtf.SetDataDetailSpace(6);
                            gtf.SetDataDetail("Total unit untuk Model : " + model + "   Warna : " + warna + "   " + intSubTotUnit + " UNIT", 228, ' ', false, true);
                            gtf.SetDataDetailLineBreak();

                            intTotUnit += intSubTotUnit;

                            gtf.PrintData(false);
                            break;
                        }

                        if (warna == string.Empty && model == string.Empty)
                        {
                            model = dt.Rows[i]["Model"].ToString();
                            if (reportID == "OmRpInvRgs002")
                                warna = dt.Rows[i]["Warna"].ToString();
                            else
                                warna = dt.Rows[i]["Colour"].ToString();
                            counterData += 1;
                            gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                            gtf.SetDataDetailSpace(2);
                            if (reportID == "OmRpInvRgs002")
                            {
                                gtf.SetDataDetail(model, 221, ' ', false, true);
                            }
                            else
                            {
                                gtf.SetDataDetail(model, 20, ' ', true);
                                gtf.SetDataDetail(warna, 211, ' ', false, true);
                            }
                            gtf.PrintData(false);
                            //gtf.SetDataDetail(model, 17, ' ', true);
                            //if (reportID == "OmRpInvRgs002A") 
                            //    gtf.SetDataDetail(dt.Rows[i]["Colour"].ToString(), 32, ' ', false, true);
                            //else gtf.SetDataDetail(warna, 7, ' ', true);
                        }
                        else
                        {
                            if (model != dt.Rows[i]["Model"].ToString() || ((reportID == "OmRpInvRgs002") ? warna != dt.Rows[i]["Warna"].ToString() : warna != dt.Rows[i]["Colour"].ToString()))
                            {
                                gtf.SetDataDetailSpace(6);
                                gtf.SetDataDetail("-", 227, '-', false, true);
                                gtf.SetDataDetailSpace(6);
                                gtf.SetDataDetail("Total unit untuk Model : " + model + "   Warna : " + warna + "   " + intSubTotUnit + " UNIT", 228, ' ', false, true);
                                gtf.SetDataDetailLineBreak();
                                gtf.PrintData(false);

                                intTotUnit += intSubTotUnit;

                                intSubTotUnit = 0;
                                model = dt.Rows[i]["Model"].ToString();
                                if (reportID == "OmRpInvRgs002")
                                    warna = dt.Rows[i]["Warna"].ToString();
                                else
                                    warna = dt.Rows[i]["Colour"].ToString();

                                if (model == dt.Rows[i - 1]["Model"].ToString())
                                {
                                    if (reportID == "OmRpInvRgs002A")
                                    {
                                        gtf.SetDataDetailSpace(27);
                                        gtf.SetDataDetail(warna, 206, ' ', false, true);
                                        gtf.PrintData(false);
                                    }
                                }
                                else
                                {
                                    counterData += 1;
                                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                                    gtf.SetDataDetailSpace(2);
                                    if (reportID == "OmRpInvRgs002")
                                    {
                                        gtf.SetDataDetail(model, 221, ' ', false, true);
                                    }
                                    else
                                    {
                                        gtf.SetDataDetail(model, 20, ' ', true);
                                        gtf.SetDataDetail(warna, 211, ' ', false, true);
                                    }
                                    gtf.PrintData(false);
                                }
                                //if (reportID == "OmRpInvRgs002A") gtf.SetDataDetail(dt.Rows[i]["Colour"].ToString(), 32, ' ', false, true);
                                //else gtf.SetDataDetail(warna, 7, ' ', true);
                            }
                        }

                        if (reportID == "OmRpInvRgs002")
                        {
                            if (warna == dt.Rows[i]["Warna"].ToString())
                            {
                                gtf.SetDataDetailSpace(14);
                            }
                        }
                        else
                        {
                            if (warna == dt.Rows[i]["Colour"].ToString())
                            {
                                gtf.SetDataDetailSpace(22);
                            }
                        }

                        if (reportID == "OmRpInvRgs002")
                        {
                            gtf.SetDataDetail(dt.Rows[i]["Warna"].ToString(), 7, ' ', true);
                        }

                        //if (reportID == "OmRpInvRgs002A") gtf.SetDataDetailSpace(24);
                        //else
                        //{
                        //    if (intSubTotUnit > 0) gtf.SetDataDetailSpace(32);
                        //}
                        gtf.SetDataDetail(dt.Rows[i]["Gudang"].ToString(), 4, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Chassis"].ToString(), 9, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Mesin"].ToString(), 9, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Tahun"].ToString(), 6, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["BPUNo"].ToString(), 14, ' ', true);
                        if (Convert.ToDateTime(dt.Rows[i]["BPUDate"]).ToShortDateString() == "1/1/1900")
                            gtf.SetDataDetail("", 13, ' ', true);
                        else
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPUDate"]).ToString("dd-MMM-yyyy"), 13, ' ', true);
                        if (Convert.ToDateTime(dt.Rows[i]["BPUSJDate"]).ToShortDateString() == "1/1/1900")
                            gtf.SetDataDetail("", 13, ' ', true);
                        else
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPUSJDate"]).ToString("dd-MMM-yyyy"), 13, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["ReffDONo"].ToString(), 10, ' ', true);
                        if (Convert.ToDateTime(dt.Rows[i]["ReffDODate"]).ToShortDateString() == "1/1/1900")
                            gtf.SetDataDetail("", 13, ' ', true);
                        else
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["ReffDODate"]).ToString("dd-MMM-yyyy"), 13, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SJNo"].ToString(), 10, ' ', true);
                        if (Convert.ToDateTime(dt.Rows[i]["SJDate"]).ToShortDateString() == "1/1/1900")
                            gtf.SetDataDetail("", 13, ' ', true);
                        else
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["SJDate"]).ToString("dd-MMM-yyyy"), 13, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["DONo"].ToString(), 14, ' ', true);
                        if (Convert.ToDateTime(dt.Rows[i]["DODate"]).ToShortDateString() == "1/1/1900")
                            gtf.SetDataDetail("", 13, ' ', true);
                        else
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DODate"]).ToString("dd-MMM-yyyy"), 13, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["BPKNo"].ToString(), 14, ' ', true);
                        if (Convert.ToDateTime(dt.Rows[i]["BPKDate"]).ToShortDateString() == "1/1/1900")
                            gtf.SetDataDetail("", 13, ' ', true);
                        else
                            gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["BPKDate"]).ToString("dd-MMM-yyyy"), 13, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Karoseri"].ToString(), 10, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Status"].ToString(), 7, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["PDI"].ToString(), 4, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Lama"].ToString(), 4, ' ', false, true, true);

                        intSubTotUnit += 1;

                        gtf.PrintData(false);
                    }
                    #endregion
                }
                #endregion

                #region Print Footer
                gtf.SetTotalDetailLine();
                gtf.SetTotalDetail("Grand Total :", 47, ' ', true);
                gtf.SetTotalDetail(intTotUnit.ToString() + " UNIT", 17, ' ', false, true, true);
                gtf.SetTotalDetailLine();
                #endregion
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}