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
    public class txtOmRpInventTrn003 : IRptProc 
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
            return CreateReportOmRpInventTrn003(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpInventTrn003(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region OmRpInventTrn003
            if (reportID == "OmRpInventTrn003")
            {
                string branchName = "Cabang        : " + dt.Rows[0]["BranchName"].ToString();
                string docNo = "Nomor   : " + dt.Rows[0]["DocNo"].ToString();
                string refferenceNo = "No. Referensi : " + dt.Rows[0]["ReferenceNo"].ToString();
                string docDate = "Tanggal : " + Convert.ToDateTime(dt.Rows[0]["DocDate"]).ToString("dd-MMM-yyyy");
                string referenceDate = "Tgl.Referensi : " + Convert.ToDateTime(dt.Rows[0]["ReferenceDate"]).ToString("dd-MMM-yyyy");
                string remark = "Keterangan    : " + dt.Rows[0]["Remark"].ToString();

                gtf.SetGroupHeader(branchName, 72, ' ');
                gtf.SetGroupHeader(docNo, 24, ' ', false, true);
                gtf.SetGroupHeader(refferenceNo, 72, ' ');
                gtf.SetGroupHeader(docDate, 24, ' ', false, true);
                gtf.SetGroupHeader(referenceDate, 96, ' ', false, true);
                gtf.SetGroupHeader(remark, 96, ' ', false, true);
                gtf.SetGroupHeader("", 96, ' ', false, true);
                gtf.SetGroupHeader("Dengan klasifikasi ganti warna kendaraan sebagai berikut :", 96, ' ', false, true);
                gtf.SetGroupHeader("", 96, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("No", 2, ' ', true, false, true);
                gtf.SetGroupHeader("Model", 8, ' ', true);
                gtf.SetGroupHeader("Thn", 4, ' ', true, false, true);
                gtf.SetGroupHeader("Ket.Model", 10, ' ', true);
                gtf.SetGroupHeader("No. Rangka", 10, ' ', true);
                gtf.SetGroupHeader("No. Mesin", 9, ' ', true);
                gtf.SetGroupHeader("WH", 2, ' ', true);
                gtf.SetGroupHeader("Warna Lama", 16, ' ', true);
                gtf.SetGroupHeader("Warna Baru", 16, ' ', true);
                gtf.SetGroupHeader("Keterangan", 10, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                int tempSpace = 0;
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        gtf.SetDataReportLine();
                        gtf.PrintData();
                        gtf.SetDataDetail("Total Ganti : " + counterData.ToString(), 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("Kami mengucapkan terima kasih atas kerjasamanya.", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetailSpace(65);
                        gtf.SetDataDetail("Kepala Cabang", 14, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetailSpace(48);
                        tempSpace = (48 - dt.Rows[i - 1]["BranchName"].ToString().Length) / 2;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["BranchName"].ToString(), dt.Rows[i - 1]["BranchName"].ToString().Length, ' ', false, true);
                        gtf.PrintData();

                        break;
                    }
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 2, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelYear"].ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelDesc"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCodeFrom"].ToString(), 16, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ColourCodeTo"].ToString(), 16, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["RemarkDtl"].ToString(), 10, ' ', false, true);

                    gtf.PrintData(true);
                }
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}