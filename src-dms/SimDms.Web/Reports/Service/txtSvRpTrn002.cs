using SimDms.Common;
using SimDms.Service.Models.Reports;
using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace SimDms.Web.Reports.Service
{
    public class txtSvRpTrn002 : IRptProc
    {
        private SimDms.Service.Models.DataContext ctx = new SimDms.Service.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public SysUser CurrentUser { get; set; }
        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            //var dt = ctx.Database.SqlQuery<SvRpTrn001>(string.Format("exec {0} {1}", sproc, sparam));
            //return CreateReportSvRpTrn002("SvRpTrn001", dt.ToList(), "", printerloc, print, "", fullpage);
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "exec " + sproc + " " + sparam;
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return CreateReportSvRpTrn002(rptId, dt, paramReport, printerloc, print, "", fullpage);

        }

        private string CreateReportSvRpTrn002(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            string[] arrayHeader = new string[7];
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            arrayHeader[0] = dt.Rows[0]["JobOrderNo"].ToString();
            arrayHeader[1] = dt.Rows[0]["Chassis"].ToString();
            arrayHeader[2] = dt.Rows[0]["JobOrderDate"] != null ? Convert.ToDateTime(dt.Rows[0]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[3] = dt.Rows[0]["Engine"].ToString();
            arrayHeader[4] = dt.Rows[0]["PoliceRegNo"].ToString();
            arrayHeader[5] = dt.Rows[0]["ServiceBookNo"].ToString();
            arrayHeader[6] = dt.Rows[0]["JobType"].ToString();

            CreateHdrSvRpTrn002(gtf, arrayHeader);
            gtf.PrintHeader();

            string docNo = "", operationNo = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["JobOrderNo"].ToString();
                    operationNo = dt.Rows[i]["OperationNo"].ToString();

                    gtf.SetDataDetail(dt.Rows[i]["OperationNo"] != null ? dt.Rows[i]["OperationNo"].ToString() : "", 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["MechanicId"] != null ? dt.Rows[i]["MechanicId"].ToString() : "", 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EmployeeName"] != null ? dt.Rows[i]["EmployeeName"].ToString() : "", 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["StartService"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i]["StartService"].ToString()).ToString("dd-MMM-yyyy HH:mm") : "", 17, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FinishService"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i]["FinishService"].ToString()).ToString("dd-MMM-yyyy HH:mm") : "", 17, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["MechanicStatus"] != null ? dt.Rows[i]["MechanicStatus"].ToString() : "", 15, ' ', false, true);
                    gtf.PrintData(true);
                }
                else if (docNo != dt.Rows[i]["JobOrderNo"].ToString())
                {
                    gtf.PrintTotal(false, false, true);

                    arrayHeader[0] = dt.Rows[i]["JobOrderNo"].ToString();
                    arrayHeader[1] = dt.Rows[i]["Chassis"].ToString();
                    arrayHeader[2] = dt.Rows[i]["JobOrderDate"] != null ? Convert.ToDateTime(dt.Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[3] = dt.Rows[i]["Engine"].ToString();
                    arrayHeader[4] = dt.Rows[i]["PoliceRegNo"].ToString();
                    arrayHeader[5] = dt.Rows[i]["ServiceBookNo"].ToString();
                    arrayHeader[6] = dt.Rows[i]["JobType"].ToString();

                    CreateHdrSvRpTrn002(gtf, arrayHeader);

                    gtf.PrintAfterBreakSamePage();

                    docNo = dt.Rows[i]["JobOrderNo"].ToString();
                    operationNo = dt.Rows[i]["OperationNo"].ToString();

                    gtf.SetDataDetail(dt.Rows[i]["OperationNo"] != null ? dt.Rows[i]["OperationNo"].ToString() : "", 12, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["MechanicId"] != null ? dt.Rows[i]["MechanicId"].ToString() : "", 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EmployeeName"] != null ? dt.Rows[i]["EmployeeName"].ToString() : "", 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["StartService"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i]["StartService"].ToString()).ToString("dd-MMM-yyyy HH:mm") : "", 17, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FinishService"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i]["FinishService"].ToString()).ToString("dd-MMM-yyyy HH:mm") : "", 17, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["MechanicStatus"] != null ? dt.Rows[i]["MechanicStatus"].ToString() : "", 15, ' ', false, true);
                    gtf.PrintData(true);

                }
                else if (docNo == dt.Rows[i]["JobOrderNo"].ToString())
                {
                    if (operationNo != dt.Rows[i]["OperationNo"].ToString())
                        gtf.SetDataDetail(dt.Rows[i]["OperationNo"] != null ? dt.Rows[i]["OperationNo"].ToString() : "", 12, ' ', true);
                    else
                        gtf.SetDataDetailSpace(13);

                    gtf.SetDataDetail(dt.Rows[i]["MechanicId"] != null ? dt.Rows[i]["MechanicId"].ToString() : "", 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EmployeeName"] != null ? dt.Rows[i]["EmployeeName"].ToString() : "", 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["StartService"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i]["StartService"].ToString()).ToString("dd-MMM-yyyy HH:mm") : "", 17, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FinishService"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i]["FinishService"].ToString()).ToString("dd-MMM-yyyy HH:mm") : "", 17, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["MechanicStatus"] != null ? dt.Rows[i]["MechanicStatus"].ToString() : "", 15, ' ', false, true);
                    gtf.PrintData(true);
                }
            }
            if (print == true)
                gtf.PrintTotal(true, false, true);
            else
            {
                //if (gtf.PrintTotal(true, false, true) == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private void CreateHdrSvRpTrn002(GenerateTextFileReport gtf, string[] arrayHeader)
        {
            gtf.CleanHeader();
            gtf.SetGroupHeader(" ", 1, ' ', false, true);
            gtf.SetGroupHeader("NO. SPK", 10, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[0].ToString(), 13, ' ', true);
            gtf.SetGroupHeader("KODE/NOMOR RANGKA", 18, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[1].ToString(), 20, ' ', true);
            gtf.SetGroupHeader("Tgl. Tutup SPK", 16, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader("", 11, ' ', false, true);

            gtf.SetGroupHeader("TGL. SPK", 10, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[2].ToString(), 13, ' ', true);
            gtf.SetGroupHeader("KODE/NOMOR MESIN", 18, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[3].ToString(), 20, ' ', true);
            gtf.SetGroupHeader("JAM TUTUP SPK", 16, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader("", 11, ' ', false, true);

            gtf.SetGroupHeader("NO POLISI", 10, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[4].ToString(), 13, ' ', true);
            gtf.SetGroupHeader("NO BUKU SERVICE", 18, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[5].ToString(), 20, ' ', true);
            gtf.SetGroupHeader("JENIS PEKERJAAN", 16, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[6].ToString(), 11, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeaderSpace(13);
            gtf.SetGroupHeader("MEKANIK", 31, ' ', true, false, false, true);
            gtf.SetGroupHeader("PERAWATAN", 35, ' ', false, true, false, true);
            gtf.SetGroupHeaderSpace(13);
            gtf.SetGroupHeader("-", 31, '-', true);
            gtf.SetGroupHeader("-", 35, '-', false, true);
            gtf.SetGroupHeader("PEKERJAAN", 12, ' ', true);
            gtf.SetGroupHeader("NIK", 15, ' ', true);
            gtf.SetGroupHeader("NAMA", 15, ' ', true);
            gtf.SetGroupHeader("MULAI", 17, ' ', true);
            gtf.SetGroupHeader("SELESAI", 17, ' ', true);
            gtf.SetGroupHeader("STATUS", 15, ' ', false, true);
            gtf.SetGroupHeaderLine();
        }
    }
}