using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn005 : IRptProc
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
            return CreateReportSpRpTrn005(rptId, dt, sparam, printerloc, print, "");
        }

        private string CreateReportSpRpTrn005(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation)
        {
            SpGenerateTextFileReport gtf = new SpGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation);
            gtf.GenerateHeader();

            gtf.SetGroupHeader("NO. WRS", 17, ' ');
            gtf.SetGroupHeader(": " + dt.Rows[0]["WRSNo"].ToString(), 51, ' ', true);
            gtf.SetGroupHeader("NO. CLAIM", 12, ' ');
            gtf.SetGroupHeader(": " + dt.Rows[0]["ClaimNo"].ToString(), 15, ' ', false, true);
            gtf.SetGroupHeader("TGL. WRS", 17, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["WRSDate"] != null) ? ": " + DateTime.Parse(dt.Rows[0]["WRSDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 51, ' ', true);
            gtf.SetGroupHeader("TGL. CLAIM", 12, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["ClaimDate"] != null) ? ": " + Convert.ToDateTime(dt.Rows[0]["ClaimDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 15, ' ', false, true);
            gtf.SetGroupHeader("NO. SJ / INVOICE", 17, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["BinningNo"] != null) ? ": " + dt.Rows[0]["BinningNo"].ToString() : ":", 54, ' ', false, true);
            gtf.SetGroupHeader("PEMASOK", 17, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["SupplierName"] != null) ? ": " + dt.Rows[0]["SupplierName"].ToString() + " [" + dt.Rows[0]["SupplierCode"].ToString() + "]" : ":", 51, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeaderSpace(4);
            gtf.SetGroupHeader("INVOICE", 22, ' ', true, false, false, true);
            gtf.SetGroupHeader("RECEIVED", 22, ' ', true, false, false, true);
            gtf.SetGroupHeader("TOTAL", 13, ' ', true, false, true);
            gtf.SetGroupHeader("QUANTITY", 32, ' ', false, true, false, true);
            gtf.SetGroupHeaderSpace(4);
            gtf.SetGroupHeader("-", 22, '-', true);
            gtf.SetGroupHeader("-", 22, '-', true);
            gtf.SetGroupHeader(" ", 13, ' ', true);
            gtf.SetGroupHeader("-", 32, '-', false, true);

            gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 15, ' ', true);
            gtf.SetGroupHeader("QTY", 6, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 15, ' ', true);
            gtf.SetGroupHeader("QTY", 6, ' ', true, false, true);
            gtf.SetGroupHeader("HARGA BELI", 13, ' ', true, false, true);
            gtf.SetGroupHeader("SHORTAGE", 8, ' ', true, false, true);
            gtf.SetGroupHeader("DAMAGE", 7, ' ', true, false, true);
            gtf.SetGroupHeader("OVER", 7, ' ', true, false, true);
            gtf.SetGroupHeader("WRONG", 7, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            decimal ttlShortageQty = 0, ttlNilaiBeli = 0, ttlDamageQty = 0, ttlWrongQty = 0, ttlOverQty = 0;
            bool lastData = false;
            string compName = "", claimDate = "";


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["ClaimNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["RcvBinning"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PartNoClaim"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReceivedQty"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PurchasePrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["ShortageQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DemageQty"].ToString(), 7, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["OvertageQty"].ToString(), 7, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["WrongQty"].ToString(), 7, ' ', false, true, true, true, "n2");
                    gtf.PrintData(true);
                }
                else if (docNo != dt.Rows[i]["ClaimNo"].ToString())
                {
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("Total : ", 49, ' ', true, false);
                    gtf.SetTotalDetail(ttlNilaiBeli.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(ttlShortageQty.ToString(), 8, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(ttlDamageQty.ToString(), 7, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(ttlOverQty.ToString(), 7, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(ttlWrongQty.ToString(), 7, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();

                    gtf.SetTotalDetailSpace(70);

                    compName = (dt.Rows[i]["CompanyGovName"].ToString() == "") ? string.Empty : dt.Rows[i]["CompanyGovName"].ToString();
                    claimDate = (dt.Rows[i]["ClaimDate"].ToString() == "") ? string.Empty : Convert.ToDateTime(dt.Rows[i]["ClaimDate"].ToString()).ToString("dd MMMM yyyy").ToUpper();

                    gtf.SetTotalDetail(string.Format("{0}, {1}", compName, claimDate), 30, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(60);
                    gtf.SetTotalDetail(dt.Rows[i]["SignName"].ToString(), 17, ' ', true, true);
                    gtf.SetTotalDetailSpace(60);
                    gtf.SetTotalDetail("-", 29, '-', true, true);
                    gtf.SetTotalDetailSpace(60);
                    gtf.SetTotalDetail(dt.Rows[i]["TitleSign"].ToString(), 17, ' ', false, true);
                    gtf.PrintTotal(false, lastData, false);

                    ttlNilaiBeli = ttlShortageQty = ttlDamageQty = ttlOverQty = ttlWrongQty = 0;

                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["WRSNo"].ToString(), dt.Rows[i]["WRSNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["ClaimNo"].ToString(), dt.Rows[i]["ClaimNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["BinningNo"].ToString(), dt.Rows[i]["BinningNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["SupplierName"].ToString(), dt.Rows[i]["SupplierName"].ToString(), 44);
                    gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["ClaimDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["ClaimDate"].ToString()).ToString("dd-MMM-yyyy"));
                    gtf.ReplaceGroupHdr(Convert.ToDateTime(dt.Rows[i - 1]["WRSDate"].ToString()).ToString("dd-MMM-yyyy"), Convert.ToDateTime(dt.Rows[i]["WRSDate"].ToString()).ToString("dd-MMM-yyyy"));

                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["RcvBinning"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PartNoClaim"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReceivedQty"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PurchasePrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["ShortageQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DemageQty"].ToString(), 7, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["OvertageQty"].ToString(), 7, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["WrongQty"].ToString(), 7, ' ', false, true, true, true, "n2");
                    gtf.PrintData(true);

                    docNo = dt.Rows[i]["ClaimNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["ClaimNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["RcvBinning"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PartNoClaim"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReceivedQty"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PurchasePrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["ShortageQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DemageQty"].ToString(), 7, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["OvertageQty"].ToString(), 7, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["WrongQty"].ToString(), 7, ' ', false, true, true, true, "n2");
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["ClaimNo"].ToString())
                            gtf.PrintData(true);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                //ttlDamageQty += decimal.Parse(dt.Rows[i]["DemageQty"].ToString());
                //ttlShortageQty += decimal.Parse(dt.Rows[i]["ShortageQty"].ToString());
                //ttlNilaiBeli += decimal.Parse(dt.Rows[i]["PurchasePrice"].ToString());
                //ttlWrongQty += decimal.Parse(dt.Rows[i]["WrongQty"].ToString());
                //ttlOverQty += decimal.Parse(dt.Rows[i]["OvertageQty"].ToString());
                ttlDamageQty += Convert.ToDecimal(dt.Rows[i]["DemageQty"]);
                ttlShortageQty += Convert.ToDecimal(dt.Rows[i]["ShortageQty"]);
                ttlNilaiBeli += Convert.ToDecimal(dt.Rows[i]["PurchasePrice"]);
                ttlWrongQty += Convert.ToDecimal((dt.Rows[i]["WrongQty"] != DBNull.Value) ? Convert.ToDecimal(dt.Rows[i]["WrongQty"]) : 0 );
                ttlOverQty += Convert.ToDecimal(dt.Rows[i]["OvertageQty"]);
            }
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("Total : ", 49, ' ', true, false);
            gtf.SetTotalDetail(ttlNilaiBeli.ToString(), 13, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlShortageQty.ToString(), 8, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlDamageQty.ToString(), 7, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlOverQty.ToString(), 7, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlWrongQty.ToString(), 7, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();

            gtf.SetTotalDetailSpace(70);

            compName = (dt.Rows[dt.Rows.Count - 1]["CompanyGovName"].ToString() == "") ? string.Empty : dt.Rows[dt.Rows.Count - 1]["CompanyGovName"].ToString();
            claimDate = (dt.Rows[dt.Rows.Count - 1]["ClaimDate"].ToString() == "") ? string.Empty : Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1]["ClaimDate"].ToString()).ToString("dd MMMM yyyy").ToUpper();

            gtf.SetTotalDetail(string.Format("{0}, {1}", compName, claimDate), 30, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(60);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName"].ToString(), 17, ' ', true, true);
            gtf.SetTotalDetailSpace(60);
            gtf.SetTotalDetail("-", 29, '-', true, true);
            gtf.SetTotalDetailSpace(60);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign"].ToString(), 17, ' ', false, true);

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