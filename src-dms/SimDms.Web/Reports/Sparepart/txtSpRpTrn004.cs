using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn004 : IRptProc
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
            da.Fill(dt);

            SetTextParameter(oparam);

            return CreateReportSpRpTrn004(rptId, dt, sparam, printerloc, setTextParameter[0].ToString(), setTextParameter[1].ToString(), setTextParameter[2].ToString(), print, "", fullpage);
        }

        private string CreateReportSpRpTrn004(string recordId, DataTable dt, string paramReport, string printerLoc, string param1, string param2, string param3, bool print, string fileLocation, bool fullPage)
        {
            SpGenerateTextFileReport gtf = new SpGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation, fullPage);
            gtf.GenerateHeader();
            //gtf.SetGroupHeader(paramReport, paramReport.Length);
            gtf.SetGroupHeader(param3, 94, ' ', false, true, false, true);
            gtf.SetGroupHeader("NO. BINNING LIST", 18, ' ');
            gtf.SetGroupHeader(": " + dt.Rows[0]["BinningNo"].ToString(), 54, ' ');
            gtf.SetGroupHeader("NO. WRS", 9, ' ');
            gtf.SetGroupHeader(": " + dt.Rows[0]["WRSNo"].ToString(), 16, ' ', false, true);
            gtf.SetGroupHeader("TGL. BINNING LIST", 18, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["BinningDate"] != null) ? ": " + DateTime.Parse(dt.Rows[0]["BinningDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 54, ' ');
            gtf.SetGroupHeader("TGL. WRS", 9, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["WRSDate"] != null) ? ": " + Convert.ToDateTime(dt.Rows[0]["WRSDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 16, ' ', false, true);
            gtf.SetGroupHeader("NO. CLAIM", 18, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["ClaimNo"] != null) ? ": " + dt.Rows[0]["ClaimNo"].ToString() : ":", 54, ' ');
            gtf.SetGroupHeader("NO. REF", 9, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["ReferenceNo"] != null) ? ": " + dt.Rows[0]["ReferenceNo"].ToString() : ":", 16, ' ', false, true);
            gtf.SetGroupHeader("PEMASOK", 18, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["SupplierName"] != null) ? ": " + dt.Rows[0]["SupplierName"].ToString() + " [" + dt.Rows[0]["SupplierCode"].ToString() + "]" : ":", 54, ' ');
            gtf.SetGroupHeader("NO. DN", 9, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["DNSupplierNo"] != null) ? ": " + dt.Rows[0]["DNSupplierNo"].ToString() : ":", 16, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeaderSpace(68);
            gtf.SetGroupHeader("DISKON", 16, ' ', false, true, false, true);
            gtf.SetGroupHeaderSpace(68);
            gtf.SetGroupHeader("-", 16, '-', false, true);
            gtf.SetGroupHeader("NO", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 15, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 13, ' ', true);
            gtf.SetGroupHeader("QTY", 8, ' ', true, false, true);
            gtf.SetGroupHeader("HARGA BELI", 11, ' ', true, false, true);
            gtf.SetGroupHeader("NILAI BELI", 11, ' ', true, false, true);
            gtf.SetGroupHeader("%", 5, ' ', true, false, true);
            gtf.SetGroupHeader("NILAI", 10, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 11, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            decimal ttlQty = 0, ttlNilaiBeli = 0, ttlDiscAmt = 0, ttlTotal = 0;
            bool lastData = false;
            string companyGovName, POSDate, alamat;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["WRSNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReceivedQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PurchasePrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PurchaseAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TOTAL"].ToString(), 11, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPage);
                }
                else if (docNo != dt.Rows[i]["WRSNo"].ToString())
                {

                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("Total : ", 34, ' ', true, false);
                    gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(ttlNilaiBeli.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetailSpace(6);
                    gtf.SetTotalDetail(ttlDiscAmt.ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(ttlTotal.ToString(), 11, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail(" ", 61, ' ');

                    companyGovName = dt.Rows[i - 1]["CompanyGovName"] is DBNull ? string.Empty : (string)dt.Rows[i - 1]["CompanyGovName"];
                    POSDate = dt.Rows[i - 1]["WRSDate"] is DBNull ? string.Empty : Convert.ToDateTime(dt.Rows[i - 1]["WRSDate"]).ToString("dd MMMM yyyy").ToUpper();
                    alamat = string.Format("{0}, {1}", companyGovName, POSDate);

                    gtf.SetTotalDetail(alamat, 30, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(61);
                    gtf.SetTotalDetail(param1, 17, ' ', false, true);
                    gtf.SetTotalDetailSpace(61);
                    gtf.SetTotalDetail("-", 17, '-', false, true);
                    gtf.SetTotalDetailSpace(61);
                    gtf.SetTotalDetail(param2, 17, ' ', false, true);
                    gtf.PrintTotal(false, lastData, false);

                    ttlQty = ttlNilaiBeli = ttlDiscAmt = ttlTotal = 0;

                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["WRSNo"].ToString(), dt.Rows[i]["WRSNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["SupplierCode"].ToString(), dt.Rows[i]["SupplierCode"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["BinningNo"].ToString(), dt.Rows[i]["BinningNo"].ToString());
                    gtf.ReplaceGroupHdr("NO. REF  : " + dt.Rows[i - 1]["ReferenceNo"].ToString(), "NO. REF  : " + dt.Rows[i]["ReferenceNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["SupplierName"].ToString(), dt.Rows[i]["SupplierName"].ToString(), 44);
                    gtf.ReplaceGroupHdr("NO. DN   : " + dt.Rows[i - 1]["DNSupplierNo"].ToString(), "NO. DN   : " + dt.Rows[i]["DNSupplierNo"].ToString());
                    gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["BinningDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["BinningDate"].ToString()).ToString("dd-MMM-yyyy"));
                    gtf.ReplaceGroupHdr(Convert.ToDateTime(dt.Rows[i - 1]["WRSDate"].ToString()).ToString("dd-MMM-yyyy"), Convert.ToDateTime(dt.Rows[i]["WRSDate"].ToString()).ToString("dd-MMM-yyyy"));
                    if (dt.Rows[i - 1]["ClaimNo"].ToString() != "")
                        gtf.ReplaceGroupHdr(dt.Rows[i - 1]["ClaimNo"].ToString(), dt.Rows[i]["ClaimNo"].ToString());
                    else
                        gtf.ReplaceGroupHdr("NO. CLAIM         :", "NO. CLAIM         :" + dt.Rows[i]["ClaimNo"].ToString());

                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReceivedQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PurchasePrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PurchaseAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TOTAL"].ToString(), 11, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPage);

                    docNo = dt.Rows[i]["WRSNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["WRSNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReceivedQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PurchasePrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PurchaseAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TOTAL"].ToString(), 11, ' ', false, true, true, true, "n0");
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["WRSNo"].ToString())
                            gtf.PrintData(fullPage);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlQty += decimal.Parse(dt.Rows[i]["ReceivedQty"].ToString());
                ttlDiscAmt += decimal.Parse(dt.Rows[i]["DiscAmt"].ToString());
                ttlNilaiBeli += decimal.Parse(dt.Rows[i]["PurchaseAmt"].ToString());
                ttlTotal += decimal.Parse(dt.Rows[i]["TOTAL"].ToString());
            }

            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("Total : ", 34, ' ', true, false);
            gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetailSpace(12);
            gtf.SetTotalDetail(ttlNilaiBeli.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetailSpace(6);
            gtf.SetTotalDetail(ttlDiscAmt.ToString(), 10, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlTotal.ToString(), 11, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail(" ", 61, ' ');

            companyGovName = dt.Rows[dt.Rows.Count - 1]["CompanyGovName"] is DBNull ? string.Empty : (string)dt.Rows[dt.Rows.Count - 1]["CompanyGovName"];
            POSDate = dt.Rows[dt.Rows.Count - 1]["WRSDate"] is DBNull ? string.Empty : Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1]["WRSDate"]).ToString("dd MMMM yyyy").ToUpper();
            alamat = string.Format("{0}, {1}", companyGovName, POSDate);

            gtf.SetTotalDetail(alamat, 30, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(61);
            gtf.SetTotalDetail(param1, 17, ' ', true, true);
            gtf.SetTotalDetailSpace(61);
            gtf.SetTotalDetail("-", 17, '-', true, true);
            gtf.SetTotalDetailSpace(61);
            gtf.SetTotalDetail(param2, 17, ' ', false, true);

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