using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn027 : IRptProc
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
            return CreateReportSpRpTrn027(rptId, dt, sparam, printerloc, print, "");
        }

        private string CreateReportSpRpTrn027(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation)
        {
            string companyGovName, POSDate, alamat;

            //----------------------------------------------------------------
            //Default Parameter
            SpGenerateTextFileReport gtf = new SpGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader("NO. RECEIVING CLAIM", 21, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["ClaimReceivedNo"].ToString(), 49, ' ');
            gtf.SetGroupHeader("NO. WRS", 9, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["WRSNo"].ToString(), 15, ' ', false, true);
            gtf.SetGroupHeader("TGL. RECEIVING CLAIM", 21, ' ', true);
            gtf.SetGroupHeader((dt.Rows[0]["ClaimReceivedDate"] != null) ? ": " + Convert.ToDateTime(dt.Rows[0]["ClaimReceivedDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 49, ' ');
            gtf.SetGroupHeader("TGL. WRS", 9, ' ', true);
            gtf.SetGroupHeader((dt.Rows[0]["WRSDate"] != null) ? ": " + Convert.ToDateTime(dt.Rows[0]["WRSDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 15, ' ', false, true);
            gtf.SetGroupHeader("NO. CLAIM", 21, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["ClaimNo"].ToString(), 49, ' ');
            gtf.SetGroupHeader("NO. REF", 9, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["ReferenceNo"].ToString(), 15, ' ', false, true);
            gtf.SetGroupHeader("NO. WH. TRANSFER", 21, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["WHTrfNo"].ToString(), 75, ' ', false, true);
            gtf.SetGroupHeader("PEMASOK", 21, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["SupplierName"].ToString(), 75, ' ', false, true);

            //----------------------------------------------------------------
            //Group Header
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 20, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 33, ' ', true);
            gtf.SetGroupHeader("QTY", 8, ' ', true, false, true);
            gtf.SetGroupHeader("HARGA BELI", 13, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 13, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            decimal ttlQty = 0, ttlTotal = 0;
            bool lastData = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["ClaimReceivedNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 33, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["TotRcvClaimQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PurchasePrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["total"].ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);
                }
                else if (docNo != dt.Rows[i]["ClaimReceivedNo"].ToString())
                {

                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetailSpace(60);
                    gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetailSpace(16);
                    gtf.SetTotalDetail(ttlTotal.ToString(), 11, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetailSpace(60);

                    companyGovName = dt.Rows[i - 1]["CityName"] is DBNull ? string.Empty : (string)dt.Rows[i - 1]["CityName"];
                    POSDate = DateTime.Now.ToString("dd MMMM yyyy");
                    alamat = string.Format("{0}, {1}", companyGovName, POSDate);

                    gtf.SetTotalDetail(alamat, 30, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(60);
                    gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName"].ToString(), 30, ' ', false, true);
                    gtf.SetTotalDetailSpace(60);
                    gtf.SetTotalDetail("-", 30, '-', false, true);
                    gtf.SetTotalDetailSpace(60);
                    gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign"].ToString(), 30, ' ', false, true);

                    gtf.PrintTotal(false, lastData, false);

                    ttlQty = ttlTotal = 0;

                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["ClaimReceivedNo"].ToString(), dt.Rows[i]["ClaimReceivedNo"].ToString(), 49);
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["WRSNo"].ToString(), dt.Rows[i]["WRSNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["ClaimNo"].ToString(), dt.Rows[i]["ClaimNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["WHTrfNo"].ToString(), dt.Rows[i]["WHTrfNo"].ToString());
                    gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["ClaimReceivedDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["ClaimReceivedDate"].ToString()).ToString("dd-MMM-yyyy"));
                    gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["WRSDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["WRSDate"].ToString()).ToString("dd-MMM-yyyy"));
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["SupplierName"].ToString(), dt.Rows[i]["SupplierName"].ToString());

                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 33, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["TotRcvClaimQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PurchasePrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["total"].ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);

                    docNo = dt.Rows[i]["ClaimReceivedNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["ClaimReceivedNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 33, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["TotRcvClaimQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PurchasePrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["total"].ToString(), 13, ' ', false, true, true, true, "n0");
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["ClaimReceivedNo"].ToString())
                            gtf.PrintData(true);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlQty += decimal.Parse(dt.Rows[i]["TotRcvClaimQty"].ToString());
                ttlTotal += decimal.Parse(dt.Rows[i]["total"].ToString());
            }

            gtf.SetTotalDetailLine();
            gtf.SetTotalDetailSpace(60);
            gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetailSpace(16);
            gtf.SetTotalDetail(ttlTotal.ToString(), 11, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetailSpace(60);

            companyGovName = dt.Rows[dt.Rows.Count - 1]["CityName"] is DBNull ? string.Empty : (string)dt.Rows[dt.Rows.Count - 1]["CityName"];
            POSDate = DateTime.Now.ToString("dd MMMM yyyy");
            alamat = string.Format("{0}, {1}", companyGovName, POSDate);

            gtf.SetTotalDetail(alamat, 30, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(60);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName"].ToString(), 30, ' ', false, true);
            gtf.SetTotalDetailSpace(60);
            gtf.SetTotalDetail("-", 30, '-', false, true);
            gtf.SetTotalDetailSpace(60);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign"].ToString(), 30, ' ', false, true);

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