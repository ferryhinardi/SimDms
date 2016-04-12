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
    public class txtOmRpInventTrn001 : IRptProc 
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
            return CreateReportOmRpInventTrn001(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpInventTrn001(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region OmRpInventTrn001 / OmRpInventTrn002
            if (reportID == "OmRpInventTrn001" || reportID == "OmRpInventTrn002")
            {
                bCreateBy = false;
                string transferNo = string.Empty, transferDate = string.Empty;

                string branchCodeFrom = "Cabang      : " + dt.Rows[0]["BranchCodeFrom"].ToString();
                string branchCodeTo = "Kepada Yth  : " + dt.Rows[0]["BranchCodeTo"].ToString();
                if (reportID == "OmRpInventTrn001")
                {
                    transferNo = "Nomor   : " + dt.Rows[0]["TransferOutNo"].ToString();
                    transferDate = "Tanggal : " + Convert.ToDateTime(dt.Rows[0]["TransferOutDate"]).ToString("dd-MMM-yyyy");
                }
                else
                {
                    transferNo = "Nomor   : " + dt.Rows[0]["TransferInNo"].ToString();
                    transferDate = "Tanggal : " + Convert.ToDateTime(dt.Rows[0]["TransferInDate"]).ToString("dd-MMM-yyyy");
                }
                string warehouseCodeFrom = "Dari gudang : " + dt.Rows[0]["WarehouseCodeFrom"].ToString();

                gtf.SetGroupHeader(branchCodeFrom, 96, ' ', false, true);
                gtf.SetGroupHeader(branchCodeTo, 72, ' ');
                gtf.SetGroupHeader(transferNo, 24, ' ', false, true);
                gtf.SetGroupHeaderSpace(72);
                gtf.SetGroupHeader(transferDate, 24, ' ', false, true);
                gtf.SetGroupHeader("Dengan hormat,", 96, ' ', false, true);
                gtf.SetGroupHeader("Mohon diterima kiriman dari kami, beberapa buah unit kendaraan suzuki,", 96, ' ', false, true);
                gtf.SetGroupHeader("", 96, ' ', false, true);
                gtf.SetGroupHeader(warehouseCodeFrom, 96, ' ', false, true);
                gtf.SetGroupHeader("Keterangan  : ", 96, ' ', false, true);
                gtf.SetGroupHeader("", 96, ' ', false, true);
                gtf.SetGroupHeader("Dengan klasifikasi sebagai berikut :", 96, ' ', false, true);
                gtf.SetGroupHeader("", 96, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("Seq", 3, ' ', true, false, true);
                gtf.SetGroupHeader("VIN", 20, ' ', true);
                gtf.SetGroupHeader("Model", 14, ' ', true);
                gtf.SetGroupHeader("Ket.Model", 14, ' ', true);
                gtf.SetGroupHeader("Warna", 14, ' ', true);
                gtf.SetGroupHeader("No. Rangka", 10, ' ', true);
                gtf.SetGroupHeader("No. Mesin", 9, ' ', true);
                gtf.SetGroupHeader("Tahun", 5, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                string TransferNo = string.Empty;
                int loop = 0, tempSpace = 0;

                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        loop = 59 - gtf.line - 10;
                        for (int j = 0; j < loop; j++)
                        {
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                        }

                        gtf.SetDataDetail("Total Transfer : " + counterData.ToString(), 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataReportLine();
                        gtf.PrintData();
                        gtf.SetDataDetail("Kami mengucapkan terima kasih atas kerjasamanya.", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetailSpace(20);
                        gtf.SetDataDetail("Pengirim", 8, ' ');
                        gtf.SetDataDetailSpace(40);
                        gtf.SetDataDetail("Penerima", 8, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetailSpace(14);
                        gtf.SetDataDetail("-", 20, '-');
                        gtf.SetDataDetailSpace(28);
                        gtf.SetDataDetail("-", 20, '-', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetailSpace(14);
                        tempSpace = (20 - dt.Rows[i - 1]["WarehouseCodeFrom"].ToString().Length) / 2;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["WarehouseCodeFrom"].ToString(), dt.Rows[i - 1]["WarehouseCodeFrom"].ToString().Length, ' ');
                        gtf.SetDataDetailSpace(tempSpace);
                        tempSpace = (20 - dt.Rows[i - 1]["WarehouseCodeTo"].ToString().Length) / 2;
                        gtf.SetDataDetailSpace(29 + tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["WarehouseCodeTo"].ToString(), dt.Rows[i - 1]["WarehouseCodeTo"].ToString().Length, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        break;
                    }

                    if (TransferNo == string.Empty)
                    {
                        if (reportID == "OmRpInventTrn001")
                            TransferNo = dt.Rows[i]["TransferOutNo"].ToString();
                        else
                            TransferNo = dt.Rows[i]["TransferInNo"].ToString();
                    }
                    else
                    {
                        if (TransferNo != ((reportID == "OmRpInventTrn001") ? dt.Rows[i]["TransferOutNo"].ToString() : dt.Rows[i]["TransferInNo"].ToString()))
                        {
                            loop = 59 - gtf.line - 10;
                            for (int j = 0; j < loop; j++)
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                            }

                            gtf.SetDataDetail("Total Transfer : " + counterData.ToString(), 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataReportLine();
                            gtf.PrintData();
                            gtf.SetDataDetail("Kami mengucapkan terima kasih atas kerjasamanya.", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(20);
                            gtf.SetDataDetail("Pengirim", 8, ' ');
                            gtf.SetDataDetailSpace(40);
                            gtf.SetDataDetail("Penerima", 8, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(14);
                            gtf.SetDataDetail("-", 20, '-');
                            gtf.SetDataDetailSpace(28);
                            gtf.SetDataDetail("-", 20, '-', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(14);
                            tempSpace = (20 - dt.Rows[i - 1]["WarehouseCodeFrom"].ToString().Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["WarehouseCodeFrom"].ToString(), dt.Rows[i - 1]["WarehouseCodeFrom"].ToString().Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace);
                            tempSpace = (20 - dt.Rows[i - 1]["WarehouseCodeTo"].ToString().Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["WarehouseCodeTo"].ToString(), dt.Rows[i - 1]["WarehouseCodeTo"].ToString().Length, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();

                            if (reportID == "OmRpInventTrn001")
                                TransferNo = dt.Rows[i]["TransferOutNo"].ToString();
                            else
                                TransferNo = dt.Rows[i]["TransferInNo"].ToString();
                            counterData = 0;

                            gtf.ReplaceGroupHdr("Cabang      : " + dt.Rows[i]["BranchCodeFrom"].ToString(), branchCodeFrom, 96);
                            gtf.ReplaceGroupHdr("Kepada Yth  : " + dt.Rows[i]["BranchCodeTo"].ToString(), branchCodeTo, 72);
                            if (reportID == "OmRpInventTrn001")
                            {
                                gtf.ReplaceGroupHdr("Nomor   : " + dt.Rows[i]["TransferOutNo"].ToString(), transferNo, 24);
                                gtf.ReplaceGroupHdr("Tanggal : " + Convert.ToDateTime(dt.Rows[i]["TransferOutDate"]).ToString("dd-MMM-yyyy"), transferDate, 24);
                            }
                            else
                            {
                                gtf.ReplaceGroupHdr("Nomor   : " + dt.Rows[i]["TransferInNo"].ToString(), transferNo, 24);
                                gtf.ReplaceGroupHdr("Tanggal : " + Convert.ToDateTime(dt.Rows[i]["TransferInDate"]).ToString("dd-MMM-yyyy"), transferDate, 24);
                            }
                            gtf.ReplaceGroupHdr("Dari gudang : " + dt.Rows[i]["WarehouseCodeFrom"].ToString(), warehouseCodeFrom, 96);

                            branchCodeFrom = "Cabang      : " + dt.Rows[i]["BranchCodeFrom"].ToString();
                            branchCodeTo = "Kepada Yth  : " + dt.Rows[i]["BranchCodeTo"].ToString();
                            if (reportID == "OmRpInventTrn001")
                            {
                                transferNo = "Nomor   : " + dt.Rows[i]["TransferInNo"].ToString();
                                transferDate = "Tanggal : " + Convert.ToDateTime(dt.Rows[i]["TransferInDate"]).ToString("dd-MMM-yyyy");
                            }
                            else
                            {
                                transferNo = "Nomor   : " + dt.Rows[i]["TransferOutNo"].ToString();
                                transferDate = "Tanggal : " + Convert.ToDateTime(dt.Rows[i]["TransferOutDate"]).ToString("dd-MMM-yyyy");
                            }
                            warehouseCodeFrom = "Dari gudang : " + dt.Rows[i]["WarehouseCodeFrom"].ToString();
                        }
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["VIN"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Colour"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 5, ' ', false, true, true);
                    gtf.PrintData(true);
                }

            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}