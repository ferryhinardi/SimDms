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
    public class txtOmRpSalesTrn006:IRptProc 
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
            return CreateReportOmRpSalesTrn006(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalesTrn006(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region SPK & Tracking BBN (OmRpSalesTrn006/OmRpSalesTrn006A)
            if (reportID == "OmRpSalesTrn006" || reportID == "OmRpSalesTrn006A")
            {
                bCreateBy = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    counterData += 1;
                    gtf.SetDataDetail("Mi" + dt.Rows[i]["CompanyName"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail(dt.Rows[i]["ComAddress1"].ToString() + ", " + dt.Rows[i]["ComAddress2"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail(dt.Rows[i]["ComAddress3"].ToString() + " " + dt.Rows[i]["ComAddress4"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("No. : " + dt.Rows[i]["SPKNo"].ToString() + "               (" + counterData.ToString() + ")", 47, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CityName"] + ", " + Convert.ToDateTime(dt.Rows[i]["SPKDate"]).ToString("dd-MMM-yyyy"), 48, ' ', false, true, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataReportLine();
                    gtf.PrintData(false, false);
                    gtf.SetDataDetailSpace(35);
                    gtf.SetDataDetail("SURAT PERINTAH KERJA - BBN", 26, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("Kepada Yth,", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail(dt.Rows[i]["SupplierName"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    if (dt.Rows[i]["SupAddress1"].ToString() != string.Empty)
                    {
                        if (dt.Rows[i]["SupAddress2"].ToString() != string.Empty)
                        {
                            gtf.SetDataDetail(dt.Rows[i]["SupAddress1"].ToString() + ", " + dt.Rows[i]["SupAddress2"].ToString(), 96, ' ', false, true);
                        }
                        else
                        {
                            gtf.SetDataDetail(dt.Rows[i]["SupAddress1"].ToString(), 96, ' ', false, true);
                        }
                    }
                    else
                    {
                        if (dt.Rows[i]["SupAddress2"].ToString() != string.Empty)
                        {
                            gtf.SetDataDetail(dt.Rows[i]["SupAddress2"].ToString(), 96, ' ', false, true);
                        }
                        else
                        {
                            gtf.SetDataDetail("", 96, ' ', false, true);
                        }
                    }
                    gtf.PrintData(false, false);
                    if (dt.Rows[i]["SupAddress3"].ToString() != string.Empty)
                    {
                        if (dt.Rows[i]["SupAddress4"].ToString() != string.Empty)
                        {
                            gtf.SetDataDetail(dt.Rows[i]["SupAddress3"].ToString() + ", " + dt.Rows[i]["SupAddress4"].ToString(), 96, ' ', false, true);
                        }
                        else
                        {
                            gtf.SetDataDetail(dt.Rows[i]["SupAddress3"].ToString(), 96, ' ', false, true);
                        }
                    }
                    else
                    {
                        if (dt.Rows[i]["SupAddress4"].ToString() != string.Empty)
                        {
                            gtf.SetDataDetail(dt.Rows[i]["SupAddress4"].ToString(), 96, ' ', false, true);
                        }
                        else
                        {
                            gtf.SetDataDetail("", 96, ' ', false, true);
                        }
                    }

                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("Dengan Hormat,", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("Mohon dapat dilaksanakan : Pengurusan BBN", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("Atas kendaraan SUZUKI dengan data sebagai berikut :", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetailSpace(3);
                    gtf.SetDataDetail("VIN    : " + dt.Rows[i]["ChassisCode"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail("No Rangka : " + dt.Rows[i]["ChassisNo"].ToString(), 31, ' ', true);
                    gtf.SetDataDetail("Warna     : " + dt.Rows[i]["ColourCodeNew"].ToString(), 32, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetailSpace(3);
                    gtf.SetDataDetail("Type   : " + dt.Rows[i]["SalesModelCode"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail("No Mesin  : " + dt.Rows[i]["EngineNo"].ToString(), 31, ' ', true);
                    gtf.SetDataDetail("SO Number : " + dt.Rows[i]["SONo"].ToString(), 32, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetailSpace(3);
                    gtf.SetDataDetail("Harga / Biaya : " + Convert.ToDecimal(dt.Rows[i]["Total"]).ToString("n0"), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetailSpace(3);
                    gtf.SetDataDetail("Nama   : " + dt.Rows[i]["CustomerName"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetailSpace(3);
                    gtf.SetDataDetail("Alamat : " + dt.Rows[i]["Address1"].ToString() + " " + dt.Rows[i]["Address2"].ToString() + " " + dt.Rows[i]["Address3"].ToString() + " " + dt.Rows[i]["Address4"].ToString(), 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataReportLine();
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("Keterangan", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataReportLine();
                    gtf.PrintData(false, false);
                    gtf.SetDataDetailSpace(12);
                    gtf.SetDataDetail("Diterima/Dilaksanakan,", 21, ' ');
                    gtf.SetDataDetailSpace(33);
                    gtf.SetDataDetail("Hormat Kami,", 12, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetailSpace(12);
                    gtf.SetDataDetail("(                   )", 21, ' ');
                    gtf.SetDataDetailSpace(29);
                    gtf.SetDataDetail("(                   )", 21, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("(Waktu Penagihan SPK asli harus dilampirkan).", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData(false, false);
                }
            }
            #endregion

            return gtf.sbDataTxt.ToString();
        }
    }
}