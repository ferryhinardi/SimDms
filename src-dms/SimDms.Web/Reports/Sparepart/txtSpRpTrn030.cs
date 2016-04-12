using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn030 : IRptProc
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
            return CreateReportSpRpTrn030(rptId, dt, sparam, printerloc, print, "");
        }

        private string  CreateReportSpRpTrn030(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation)
        {
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
            gtf.SetGroupHeaderSpace(72);
            gtf.SetGroupHeader("NO. RESERVED", 15, ' ', true);
            gtf.SetGroupHeader((dt.Rows[0]["ReservedNo"] != null) ? ": " + dt.Rows[0]["ReservedNo"].ToString() : ": -", 51, ' ', true);
            gtf.SetGroupHeader("NO. REFERENSI", 15, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["ReferenceNo"] != null) ? ": " + dt.Rows[0]["ReferenceNo"].ToString() : ": -", 15, ' ', false, true);
            gtf.SetGroupHeader("TGL. RESERVED", 15, ' ', true);
            gtf.SetGroupHeader((dt.Rows[0]["ReservedDate"] != null) ? ": " + DateTime.Parse(dt.Rows[0]["ReservedDate"].ToString()).ToString("dd-MMM-yyyy") : ": -", 51, ' ', true);
            gtf.SetGroupHeader("TGL. REFERENSI", 15, ' ');
            gtf.SetGroupHeader((dt.Rows[0]["ReferenceDate"] != null) ? ": " + DateTime.Parse(dt.Rows[0]["ReferenceDate"].ToString()).ToString("dd-MMM-yyyy") : ": -", 15, ' ', false, true);
            gtf.SetGroupHeader("KODE OPERASI", 15, ' ', true);
            gtf.SetGroupHeader((dt.Rows[0]["OprCode"] != null) ? ": " + dt.Rows[0]["OprCode"].ToString() : ":", 81, ' ', false, true);

            //----------------------------------------------------------------
            //Group Header
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 20, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 59, ' ', true);
            gtf.SetGroupHeader("RESERVED QTY", 11, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            bool lastData = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["ReservedNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 59, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReservedQty"].ToString(), 11, ' ', false, true, true, true, "n2");
                    gtf.PrintData(true);
                }
                else if (docNo != dt.Rows[i]["ReservedNo"].ToString())
                {
                    gtf.SetTotalDetailLine();
                    gtf.PrintTotal(false, lastData, true);

                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["OprCode"].ToString(), dt.Rows[i]["OprCode"].ToString(), 96);
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["ReservedNo"].ToString(), dt.Rows[i]["ReservedNo"].ToString(), 51);
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["ReferenceNo"].ToString(), dt.Rows[i]["ReferenceNo"].ToString(), 15);
                    gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["ReservedDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["ReservedDate"].ToString()).ToString("dd-MMM-yyyy"), 51);
                    gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["ReferenceDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["ReferenceDate"].ToString()).ToString("dd-MMM-yyyy"), 15);
                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 59, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReservedQty"].ToString(), 11, ' ', false, true, true, true, "n2");
                    gtf.PrintData(true);

                    docNo = dt.Rows[i]["ReservedNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["ReservedNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 59, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ReservedQty"].ToString(), 11, ' ', false, true, true, true, "n2");

                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["ReservedNo"].ToString())
                            gtf.PrintData(true);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }
            }

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal(true, lastData, true);
            else
            {
                if (gtf.PrintTotal(true, lastData, true) == true)
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