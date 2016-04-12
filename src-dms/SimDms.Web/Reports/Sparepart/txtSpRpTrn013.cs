using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn013 : IRptProc
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
            return CreateReportSpRpTrn013(rptId, dt, sparam, printerloc, print, "", fullpage);
        }

        private string CreateReportSpRpTrn013(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            SpGenerateTextFileReport gtf = new SpGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation, fullPage);
            gtf.GenerateHeader();
            //gtf.SetGroupHeader(paramReport, paramReport.Length);
            gtf.SetGroupHeader("NO. LAMPIRAN", 13, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["LMPNo"].ToString(), 59, ' ');
            gtf.SetGroupHeader("NO. SSR", 7, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["ReturnNo"].ToString(), 15, ' ', false, true);
            gtf.SetGroupHeader("NO. SPK", 13, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["SkpNo"].ToString(), 59, ' ');
            gtf.SetGroupHeader("TANGGAL", 7, ' ', true);
            gtf.SetGroupHeader((dt.Rows[0]["ReturnDate"] != null) ? ": " + Convert.ToDateTime(dt.Rows[0]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 15, ' ', false, true);
            gtf.SetGroupHeader("TGL. SPK", 13, ' ', true);
            gtf.SetGroupHeader((dt.Rows[0]["SpkDate"] != null) ? ": " + Convert.ToDateTime(dt.Rows[0]["SpkDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 15, ' ', false, true);
            gtf.SetGroupHeader("PELANGGAN", 13, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["CustomerName"].ToString() + "(" + dt.Rows[0]["CustomerCode"].ToString() + ")", 83, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO.", 3, ' ', true);
            gtf.SetGroupHeader("NO. SS", 13, ' ', true, false);
            gtf.SetGroupHeader("NO. PART", 20, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 34, ' ', true);
            gtf.SetGroupHeader("QTY", 8, ' ', true, false, true);
            gtf.SetGroupHeader("MC", 2, ' ', true);
            gtf.SetGroupHeader("LOKASI", 10, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            decimal ttlQty = 0;
            bool lastData = false;
            string cityName = "", alamat = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["ReturnNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["LMPNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 34, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyReturn"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["MovingCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LocationCode"].ToString(), 10, ' ', false, true);
                    gtf.PrintData(fullPage);
                }
                else if (docNo != dt.Rows[i]["ReturnNo"].ToString())
                {

                    gtf.SetTotalDetailLine();

                    gtf.SetTotalDetail("Total : ", 74, ' ', true, false);
                    gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', false, true, true, true, "n2");
                    gtf.SetTotalDetailLine();

                    cityName = dt.Rows[i]["CityName"].ToString() != "" ? "_____________" : dt.Rows[i]["CityName"].ToString();
                    alamat = string.Format("{0}, {1}", cityName, DateTime.Now.ToString("dd MMMM yyyy").ToUpper());

                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail(alamat, 30, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);

                    gtf.SetTotalDetailSpace(5);
                    gtf.SetTotalDetail("Diterima Oleh : ", 20, ' ', true);
                    gtf.SetTotalDetailSpace(2);
                    gtf.SetTotalDetail("Diserahkan Oleh : ", 20, ' ', true);
                    gtf.SetTotalDetailSpace(2);
                    gtf.SetTotalDetail("Diperiksa Oleh : ", 20, ' ', true);
                    gtf.SetTotalDetailSpace(2);
                    gtf.SetTotalDetail("Dibuat Oleh : ", 20, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(5);
                    gtf.SetTotalDetail("_", 20, '_', true);
                    gtf.SetTotalDetailSpace(2);
                    gtf.SetTotalDetail("_", 20, '_', true);
                    gtf.SetTotalDetailSpace(2);
                    gtf.SetTotalDetail("_", 20, '_', true);
                    gtf.SetTotalDetailSpace(2);
                    gtf.SetTotalDetail("_", 20, '_', false, true);
                    gtf.PrintTotal(false, lastData, false);

                    ttlQty = 0;

                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["LMPNo"].ToString(), dt.Rows[i]["LMPNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["ReturnNo"].ToString(), dt.Rows[i]["ReturnNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["SkpNo"].ToString() != "" ? dt.Rows[i - 1]["SkpNo"].ToString() : "  .   .   . -   .", dt.Rows[i]["SkpNo"].ToString() != "" ? dt.Rows[i]["SkpNo"].ToString() : "  .   .   . -   .", 59);
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["CustomerName"].ToString(), dt.Rows[i]["CustomerName"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["CustomerCode"].ToString(), dt.Rows[i]["CustomerCode"].ToString());
                    gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy"));
                    gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["SpkDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["SpkDate"].ToString()).ToString("dd-MMM-yyyy"));
                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["LMPNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 34, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyReturn"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["MovingCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LocationCode"].ToString(), 10, ' ', false, true);
                    gtf.PrintData(fullPage);

                    docNo = dt.Rows[i]["ReturnNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["ReturnNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["LMPNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 34, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyReturn"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["MovingCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LocationCode"].ToString(), 10, ' ', false, true);
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["ReturnNo"].ToString())
                            gtf.PrintData(fullPage);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlQty += decimal.Parse(dt.Rows[i]["QtyReturn"].ToString());
            }

            gtf.SetTotalDetailLine();

            gtf.SetTotalDetail("Total : ", 74, ' ', true, false);
            gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', false, true, true, true, "n2");
            gtf.SetTotalDetailLine();

            cityName = dt.Rows[dt.Rows.Count - 1]["CityName"].ToString() != "" ? "_____________" : dt.Rows[dt.Rows.Count - 1]["CityName"].ToString();
            alamat = string.Format("{0}, {1}", cityName, DateTime.Now.ToString("dd MMMM yyyy").ToUpper());

            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail(alamat, 30, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);

            gtf.SetTotalDetailSpace(5);
            gtf.SetTotalDetail("Diterima Oleh : ", 20, ' ', true);
            gtf.SetTotalDetailSpace(2);
            gtf.SetTotalDetail("Diserahkan Oleh : ", 20, ' ', true);
            gtf.SetTotalDetailSpace(2);
            gtf.SetTotalDetail("Diperiksa Oleh : ", 20, ' ', true);
            gtf.SetTotalDetailSpace(2);
            gtf.SetTotalDetail("Dibuat Oleh : ", 20, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(5);
            gtf.SetTotalDetail("_", 20, '_', true);
            gtf.SetTotalDetailSpace(2);
            gtf.SetTotalDetail("_", 20, '_', true);
            gtf.SetTotalDetailSpace(2);
            gtf.SetTotalDetail("_", 20, '_', true);
            gtf.SetTotalDetailSpace(2);
            gtf.SetTotalDetail("_", 20, '_', false, true);

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