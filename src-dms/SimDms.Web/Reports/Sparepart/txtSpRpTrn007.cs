using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn007 : IRptProc
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
            return CreateReportSpRpTrn007(rptId, dt, sparam, printerloc, print, "");
        }

        private string CreateReportSpRpTrn007(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation)
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
            gtf.SetGroupHeaderSpace(72);
            gtf.SetGroupHeader("NOMOR", 8, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["WHTrfNo"].ToString(), 15, ' ', false, true);
            gtf.SetGroupHeader("BERDASARKAN NO. REFERENSI", 26, ' ');
            gtf.SetGroupHeader(": " + dt.Rows[0]["ReferenceNo"].ToString(), 46, ' ');
            gtf.SetGroupHeader("TANGGAL", 8, ' ', true);
            gtf.SetGroupHeader((dt.Rows[0]["WHTrfDate"] != null) ? ": " + Convert.ToDateTime(dt.Rows[0]["WHTrfDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 15, ' ', false, true);

            //----------------------------------------------------------------
            //Group Header
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 20, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 19, ' ', true);
            gtf.SetGroupHeader("GD.ASAL", 7, ' ', true, false, true);
            gtf.SetGroupHeader("GD.TUJUAN", 9, ' ', true, false, true);
            gtf.SetGroupHeader("QTY", 8, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL COST", 13, ' ', true, false, true);
            gtf.SetGroupHeader("REASON", 10, ' ', false, true);
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
                    docNo = dt.Rows[i]["WHTrfNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 19, ' ', true);
                    gtf.SetDataDetail("   " + dt.Rows[i]["FromWarehouseCode"].ToString(), 7, ' ', true, false);
                    gtf.SetDataDetail("    " + dt.Rows[i]["ToWarehouseCode"].ToString(), 9, ' ', true, false);
                    gtf.SetDataDetail(dt.Rows[i]["Qty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["TotalCost"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["ReasonCode"].ToString(), 10, ' ', false, true);
                    gtf.PrintData(true);
                }
                else if (docNo != dt.Rows[i]["WHTrfNo"].ToString())
                {

                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetailSpace(63);
                    gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetail(ttlTotal.ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();
                    gtf.PrintTotal(false, lastData, true);

                    ttlQty = ttlTotal = 0;

                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["WHTrfNo"].ToString(), dt.Rows[i]["WHTrfNo"].ToString(), 13);
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["ReferenceNo"].ToString(), dt.Rows[i]["ReferenceNo"].ToString(), 46);
                    gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["WHTrfDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["WHTrfDate"].ToString()).ToString("dd-MMM-yyyy"));

                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 19, ' ', true);
                    gtf.SetDataDetail("   " + dt.Rows[i]["FromWarehouseCode"].ToString(), 7, ' ', true, false);
                    gtf.SetDataDetail("    " + dt.Rows[i]["ToWarehouseCode"].ToString(), 9, ' ', true, false);
                    gtf.SetDataDetail(dt.Rows[i]["Qty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["TotalCost"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["ReasonCode"].ToString(), 10, ' ', false, true);
                    gtf.PrintData(true);

                    docNo = dt.Rows[i]["WHTrfNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["WHTrfNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 19, ' ', true);
                    gtf.SetDataDetail("   " + dt.Rows[i]["FromWarehouseCode"].ToString(), 7, ' ', true, false);
                    gtf.SetDataDetail("    " + dt.Rows[i]["ToWarehouseCode"].ToString(), 9, ' ', true, false);
                    gtf.SetDataDetail(dt.Rows[i]["Qty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["TotalCost"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["ReasonCode"].ToString(), 10, ' ', false, true);
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["WHTrfNo"].ToString())
                            gtf.PrintData(true);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlQty += decimal.Parse(dt.Rows[i]["Qty"].ToString());
                ttlTotal += decimal.Parse(dt.Rows[i]["TotalCost"].ToString());
            }
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetailSpace(63);
            gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetail(ttlTotal.ToString(), 13, ' ', false, true, true, true, "n0");
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