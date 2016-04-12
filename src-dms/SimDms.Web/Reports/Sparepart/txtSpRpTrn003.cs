using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn003 : IRptProc
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
            return CreateReportSpRpTrn003A(rptId, dt, sparam, printerloc, print, "");
        }

        private string CreateReportSpRpTrn003A(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation)
        {
            SpGenerateTextFileReport gtf = new SpGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W163", print, fileLocation);
            gtf.GenerateHeader();
            gtf.SetGroupHeader("NO. SURAT JALAN INVOICE", 25, ' ');
            gtf.SetGroupHeader(": " + dt.Rows[0]["DNSupplierNo"].ToString(), 100, ' ', true);
            gtf.SetGroupHeader("NO. BINNING LIST", 20, ' ');
            gtf.SetGroupHeader(": " + dt.Rows[0]["BinningNo"].ToString(), 16, ' ', false, true);
            gtf.SetGroupHeader("TGL. SURAT JALAN INVOICE", 25, ' ');
            gtf.SetGroupHeader(": " + DateTime.Parse(dt.Rows[0]["DNSupplierDate"].ToString()).ToString("dd-MMM-yyyy"), 100, ' ', true);
            gtf.SetGroupHeader("TGL. BINNING LIST", 20, ' ');
            gtf.SetGroupHeader(": " + DateTime.Parse(dt.Rows[0]["BinningDate"].ToString()).ToString("dd-MMM-yyyy"), 16, ' ', false, true);
            gtf.SetGroupHeader("PEMASOK", 25, ' ');
            gtf.SetGroupHeader(": " + dt.Rows[0]["SupplierName"].ToString() + "(" + dt.Rows[0]["SupplierCode"].ToString() + ")", 163, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeaderSpace(93);
            gtf.SetGroupHeader("QUANTITY", 54, ' ', false, true, false, true);
            gtf.SetGroupHeader("NO", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 20, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 38, ' ', true);
            gtf.SetGroupHeader("WH", 2, ' ', true);
            gtf.SetGroupHeader("LOKASI", 8, ' ', true);
            gtf.SetGroupHeader("NO. PETI", 15, ' ', true);
            gtf.SetGroupHeader("RECEIVED", 10, ' ', true, false, true);
            gtf.SetGroupHeader("SHORTAGE", 10, ' ', true, false, true);
            gtf.SetGroupHeader("DAMAGE", 10, ' ', true, false, true);
            gtf.SetGroupHeader("OVER", 10, ' ', true, false, true);
            gtf.SetGroupHeader("WRONG", 10, ' ', true, false, true);
            gtf.SetGroupHeader("NO.ORDER/BPS", 15, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            decimal ttlReceived = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["BinningNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 38, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LocationCode"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BoxNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReceivedQty"].ToString(), 10, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail("_", 10, '_', true, false, true);
                    gtf.SetDataDetail("_", 10, '_', true, false, true);
                    gtf.SetDataDetail("_", 10, '_', true, false, true);
                    gtf.SetDataDetail("_", 10, '_', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["DocNo"].ToString(), 15, ' ', false, true);
                    gtf.PrintData(false);

                    ttlReceived += decimal.Parse(dt.Rows[i]["ReceivedQty"].ToString());
                }
                else
                {
                    if (dt.Rows[i]["BinningNo"].ToString() != docNo)
                    {
                        gtf.SetTotalDetailLine();
                        gtf.SetTotalDetail("Total : ", 93, ' ', true, false, true);
                        gtf.SetTotalDetail(ttlReceived.ToString(), 10, ' ', true, false, true, true, "n2");
                        gtf.SetTotalDetail("_", 10, '_', true, false, true);
                        gtf.SetTotalDetail("_", 10, '_', true, false, true);
                        gtf.SetTotalDetail("_", 10, '_', true, false, true);
                        gtf.SetTotalDetail("_", 10, '_', false, true, true);
                        gtf.SetTotalDetailLine();
                        gtf.SetTotalDetail(" ", 1, ' ', false, true);
                        gtf.SetTotalDetail(" ", 1, ' ', false, true);
                        gtf.SetTotalDetail(" ", 1, ' ', false, true);
                        gtf.SetTotalDetailSpace(93);
                        gtf.SetTotalDetail(dt.Rows[i - 1]["SignName1"].ToString(), 21, ' ', true);
                        gtf.SetTotalDetail(dt.Rows[i - 1]["SignName2"].ToString(), 21, ' ', true);
                        gtf.SetTotalDetail(dt.Rows[i - 1]["SignName3"].ToString(), 21, ' ', false, true);
                        gtf.SetTotalDetailSpace(93);
                        gtf.SetTotalDetail("_", 21, '_', true);
                        gtf.SetTotalDetail("_", 21, '_', true);
                        gtf.SetTotalDetail("_", 21, '_', false, true);
                        gtf.SetTotalDetailSpace(93);
                        gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 21, ' ', true);
                        gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign2"].ToString(), 21, ' ', true);
                        gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign3"].ToString(), 21, ' ', false, true);

                        gtf.PrintTotal(false, false, false);
                        //gtf.RestartTotal();

                        ttlReceived = 0;
                        noUrut = 0;

                        gtf.ReplaceGroupHdr(dt.Rows[i - 1]["DNSupplierNo"].ToString(), dt.Rows[i]["DNSupplierNo"].ToString());
                        gtf.ReplaceGroupHdr(dt.Rows[i - 1]["BinningNo"].ToString(), dt.Rows[i]["BinningNo"].ToString());
                        gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["DNSupplierDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["DNSupplierDate"].ToString()).ToString("dd-MMM-yyyy"));
                        gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["BinningDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["BinningDate"].ToString()).ToString("dd-MMM-yyyy"));
                        gtf.ReplaceGroupHdr(dt.Rows[i - 1]["SupplierName"].ToString(), dt.Rows[i]["SupplierName"].ToString());
                        gtf.ReplaceGroupHdr(dt.Rows[i - 1]["SupplierCode"].ToString(), dt.Rows[i]["SupplierCode"].ToString());

                        gtf.PrintAfterBreak();
                    }
                }
                noUrut++;
                gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 38, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["LocationCode"].ToString(), 8, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["BoxNo"].ToString(), 15, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["ReceivedQty"].ToString(), 10, ' ', true, false, true);
                gtf.SetDataDetail("_", 10, '_', true, false, true);
                gtf.SetDataDetail("_", 10, '_', true, false, true);
                gtf.SetDataDetail("_", 10, '_', true, false, true);
                gtf.SetDataDetail("_", 10, '_', true, false, true);
                gtf.SetDataDetail(dt.Rows[i]["DocNo"].ToString(), 15, ' ', false, true);
                gtf.PrintData(false);

                ttlReceived += decimal.Parse(dt.Rows[i]["ReceivedQty"].ToString());

            }
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("Total : ", 93, ' ', true, false, true);
            gtf.SetTotalDetail(ttlReceived.ToString(), 10, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetail("_", 10, '_', true, false, true);
            gtf.SetTotalDetail("_", 10, '_', true, false, true);
            gtf.SetTotalDetail("_", 10, '_', true, false, true);
            gtf.SetTotalDetail("_", 10, '_', false, true, true);
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(93);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName1"].ToString(), 21, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName2"].ToString(), 21, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName3"].ToString(), 21, ' ', false, true);
            gtf.SetTotalDetailSpace(93);
            gtf.SetTotalDetail("_", 21, '_', true);
            gtf.SetTotalDetail("_", 21, '_', true);
            gtf.SetTotalDetail("_", 21, '_', false, true);
            gtf.SetTotalDetailSpace(93);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign1"].ToString(), 21, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign2"].ToString(), 21, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign"].ToString(), 21, ' ', false, true);

            if (print == true)
                gtf.PrintTotal(true, false, false);
            else
            {
                if (gtf.PrintTotal(true, false, false) == true)
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