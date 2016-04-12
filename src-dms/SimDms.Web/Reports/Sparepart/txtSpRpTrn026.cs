using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn026 : IRptProc
    {
        public Models.SysUser CurrentUser { get; set; }
        private SimDms.Sparepart.Models.DataContext ctx = new SimDms.Sparepart.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        private object[] setTextParameter;
        private string msg = "";

        public void SetTextParameter(params object[] textParam)
        {
            setTextParameter = textParam;
        }

        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "exec " + sproc + " " + sparam;
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();

            setTextParameter = oparam;
            string txtParam;
            if (setTextParameter == null)
            {
                txtParam="";
            }
            else
            {
                txtParam = setTextParameter[0].ToString();
            }

            da.Fill(dt);
            return CreateReportSpRpTrn026(rptId, dt, sparam, printerloc, txtParam, print, "", fullpage);
        }

        private string CreateReportSpRpTrn026(string recordId, DataTable dt, string paramReport, string printerLoc, string param1, bool print, string fileLocation, bool fullPage)
        {
            SpGenerateTextFileReport gtf = new SpGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation, fullPage);
            gtf.GenerateHeader();
            //gtf.SetGroupHeader(paramReport, paramReport.Length);
            gtf.SetGroupHeader("NO. HPP", 10, ' ');
            gtf.SetGroupHeader(": " + dt.Rows[0]["HPPNo"].ToString(), 62, ' ');
            gtf.SetGroupHeader("NO. WRS", 9, ' ');
            gtf.SetGroupHeader(": " + dt.Rows[0]["WRSNo"].ToString(), 16, ' ', false, true);
            gtf.SetGroupHeader("PEMASOK", 10, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["SupplierName"] != null) ? ": " + dt.Rows[0]["SupplierName"].ToString() + " [" + dt.Rows[0]["SupplierCode"].ToString() + "]" : ":", 62, ' ');
            gtf.SetGroupHeader("TGL. WRS", 9, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["WRSDate"] != null) ? ": " + Convert.ToDateTime(dt.Rows[0]["WRSDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 16, ' ', false, true);
            gtf.SetGroupHeader("F. PAJAK", 10, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["TaxNo"] != null) ? ": " + dt.Rows[0]["TaxNo"].ToString() + " / " + DateTime.Parse(dt.Rows[0]["TaxDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 62, ' ');
            gtf.SetGroupHeader("DUE DATE", 9, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["DueDate"] != null) ? ": " + DateTime.Parse(dt.Rows[0]["DueDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 16, ' ', false, true);
            gtf.SetGroupHeader("NO. REF", 10, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["ReferenceNo"] != null) ? ": " + dt.Rows[0]["ReferenceNo"].ToString() + " / " + DateTime.Parse(dt.Rows[0]["ReferenceDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 62, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PO", 13, ' ', true);
            gtf.SetGroupHeader("NO. PART", 15, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 23, ' ', true);
            gtf.SetGroupHeader("QTY", 8, ' ', true, false, true);
            gtf.SetGroupHeader("HARGA SATUAN", 13, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 15, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            decimal ttlTotal = 0;
            bool lastData = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["HPPNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["DocNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 23, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReceivedQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["CostPrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DppAmt"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPage);
                }
                else if (docNo != dt.Rows[i]["HPPNo"].ToString())
                {
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("KETERANGAN :", 59, ' ', true);
                    gtf.SetTotalDetail("TOTAL", 8, ' ', true);
                    gtf.SetTotalDetail("DPP", 5, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    gtf.SetTotalDetail(ttlTotal.ToString(), 20, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetail(param1, 68, ' ', true);
                    gtf.SetTotalDetail("PPN", 5, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotTaxAmt"].ToString(), 20, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailSpace(60);
                    gtf.SetTotalDetail("SELISIH", 8, ' ', true);
                    gtf.SetTotalDetail("DPP", 5, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["DiffNetPurchAmt"].ToString(), 20, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailSpace(69);
                    gtf.SetTotalDetail("PPN", 5, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["DiffTaxAmt"].ToString(), 20, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailSpace(76);
                    gtf.SetTotalDetail("-", 20, '-', false, true);
                    gtf.SetTotalDetailSpace(76);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotHPPAmt"].ToString(), 20, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailSpace(76);
                    gtf.SetTotalDetail("-", 20, '-', false, true);
                    gtf.SetTotalDetailSpace(35);
                    gtf.SetTotalDetail("ACCT. DEPT HEAD", 25, ' ', true);
                    gtf.SetTotalDetail("DATA ENTRY", 25, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(35);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName1"].ToString(), 25, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName2"].ToString(), 25, ' ', false, true);
                    gtf.SetTotalDetailSpace(35);
                    gtf.SetTotalDetail("-", 25, '-', true);
                    gtf.SetTotalDetail("-", 25, '-', false, true);
                    gtf.SetTotalDetailSpace(35);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 25, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign2"].ToString(), 25, ' ', false, true);
                    gtf.PrintTotal(false, lastData, false);

                    ttlTotal = 0;

                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["HPPNo"].ToString(), dt.Rows[i]["HPPNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["WRSNo"].ToString(), dt.Rows[i]["WRSNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["SupplierName"].ToString() + " [" + dt.Rows[i - 1]["SupplierCode"].ToString() + "]", dt.Rows[i]["SupplierName"].ToString() + " [" + dt.Rows[i]["SupplierCode"].ToString() + "]", 62);
                    gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["DueDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["DueDate"].ToString()).ToString("dd-MMM-yyyy"));
                    gtf.ReplaceGroupHdr(Convert.ToDateTime(dt.Rows[i - 1]["WRSDate"].ToString()).ToString("dd-MMM-yyyy"), Convert.ToDateTime(dt.Rows[i]["WRSDate"].ToString()).ToString("dd-MMM-yyyy"));

                    if (dt.Rows[i - 1]["TaxNo"].ToString() != "")
                        gtf.ReplaceGroupHdr(dt.Rows[i - 1]["TaxNo"].ToString() + " / " + DateTime.Parse(dt.Rows[i - 1]["TaxDate"].ToString()).ToString("dd-MMM-yyyy"), dt.Rows[i]["TaxNo"].ToString() + " / " + DateTime.Parse(dt.Rows[i]["TaxDate"].ToString()).ToString("dd-MMM-yyyy"));
                    else
                        gtf.ReplaceGroupHdr("F. PAJAK  :", "F. PAJAK  :" + dt.Rows[i]["TaxNo"].ToString() + " / " + DateTime.Parse(dt.Rows[i]["TaxDate"].ToString()).ToString("dd-MMM-yyyy"));

                    if (dt.Rows[i - 1]["ReferenceNo"].ToString() != "")
                        gtf.ReplaceGroupHdr(dt.Rows[i - 1]["ReferenceNo"].ToString() + " / " + DateTime.Parse(dt.Rows[i - 1]["ReferenceDate"].ToString()).ToString("dd-MMM-yyyy"), dt.Rows[i]["ReferenceNo"].ToString() + " / " + DateTime.Parse(dt.Rows[i]["ReferenceDate"].ToString()).ToString("dd-MMM-yyyy"));
                    else
                        gtf.ReplaceGroupHdr("NO. REFF  :", "NO. REFF  :" + dt.Rows[i]["ReferenceNo"].ToString() + " / " + DateTime.Parse(dt.Rows[i]["ReferenceDate"].ToString()).ToString("dd-MMM-yyyy"));

                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["DocNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 23, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReceivedQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["CostPrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DppAmt"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPage);

                    docNo = dt.Rows[i]["HPPNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["HPPNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["DocNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 23, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReceivedQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["CostPrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DppAmt"].ToString(), 15, ' ', false, true, true, true, "n0");
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["HPPNo"].ToString())
                            gtf.PrintData(fullPage);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlTotal += decimal.Parse(dt.Rows[i]["DppAmt"].ToString());
            }
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("KETERANGAN :", 59, ' ', true);
            gtf.SetTotalDetail("TOTAL", 8, ' ', true);
            gtf.SetTotalDetail("DPP", 5, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            gtf.SetTotalDetail(ttlTotal.ToString(), 20, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetail(param1, 68, ' ', true);
            gtf.SetTotalDetail("PPN", 5, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotTaxAmt"].ToString(), 20, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailSpace(60);
            gtf.SetTotalDetail("SELISIH", 8, ' ', true);
            gtf.SetTotalDetail("DPP", 5, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["DiffNetPurchAmt"].ToString(), 20, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailSpace(69);
            gtf.SetTotalDetail("PPN", 5, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["DiffTaxAmt"].ToString(), 20, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailSpace(76);
            gtf.SetTotalDetail("-", 20, '-', false, true);
            gtf.SetTotalDetailSpace(76);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotHPPAmt"].ToString(), 20, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailSpace(76);
            gtf.SetTotalDetail("-", 20, '-', false, true);
            gtf.SetTotalDetailSpace(35);
            gtf.SetTotalDetail("ACCT. DEPT HEAD", 25, ' ', true);
            gtf.SetTotalDetail("DATA ENTRY", 25, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(35);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName1"].ToString(), 25, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName2"].ToString(), 25, ' ', false, true);
            gtf.SetTotalDetailSpace(35);
            gtf.SetTotalDetail("-", 25, '-', true);
            gtf.SetTotalDetail("-", 25, '-', false, true);
            gtf.SetTotalDetailSpace(35);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign1"].ToString(), 25, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign2"].ToString(), 25, ' ', false, true);

            if (print == true)
                gtf.PrintTotal(true, lastData, false);
            else
            {
                if (gtf.PrintTotal(true, lastData, false) == true)
                    msg = "Save Berhasil";
                else
                    msg = "Save Gagal";
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }
    }
}