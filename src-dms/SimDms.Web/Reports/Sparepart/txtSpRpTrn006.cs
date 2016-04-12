using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn006 : IRptProc
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
            return CreateReportSpRpTrn006(rptId, dt, sparam, printerloc, print, "");
        }

        private string CreateReportSpRpTrn006(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation)
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
            gtf.SetGroupHeader(": " + dt.Rows[0]["AdjustmentNo"].ToString(), 15, ' ', false, true);
            gtf.SetGroupHeader("BERDASARKAN NO. REFERENSI", 26, ' ');
            gtf.SetGroupHeader(": " + dt.Rows[0]["ReferenceNo"].ToString(), 46, ' ');
            gtf.SetGroupHeader("TANGGAL", 8, ' ', true);
            gtf.SetGroupHeader((dt.Rows[0]["AdjustmentDate"] != null) ? ": " + Convert.ToDateTime(dt.Rows[0]["AdjustmentDate"].ToString()).ToString("dd-MMM-yyyy") : ":", 15, ' ', false, true);

            //----------------------------------------------------------------
            //Group Header
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeaderSpace(42);
            gtf.SetGroupHeader("QUANTITY", 17, ' ', true, false, false, true);
            gtf.SetGroupHeader("TOTAL COST", 23, ' ', false, true, false, true);
            gtf.SetGroupHeaderSpace(42);
            gtf.SetGroupHeader("-", 19, '-', true);
            gtf.SetGroupHeader("-", 23, '-', false, true);
            gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 15, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 18, ' ', true);
            gtf.SetGroupHeader("WH", 2, ' ', true, false, true);
            gtf.SetGroupHeader("+", 9, ' ', true, false, true);
            gtf.SetGroupHeader("-", 9, ' ', true, false, true);
            gtf.SetGroupHeader("+", 11, ' ', true, false, true);
            gtf.SetGroupHeader("-", 11, ' ', true, false, true);
            gtf.SetGroupHeader("ALASAN", 10, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            decimal ttlQtyAdjPlus = 0, ttlQtyAdjMinus = 0, ttlCstPlus = 0, ttlCstMns = 0;
            bool lastData = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["AdjustmentNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 18, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyAdjPlus"].ToString(), 9, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["QtyAdjMinus"].ToString(), 9, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["CostPricePlus"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["CostPriceMinus"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AdjustmentReason"].ToString(), 10, ' ', false, true);
                    gtf.PrintData(true);
                }
                else if (docNo != dt.Rows[i]["AdjustmentNo"].ToString())
                {
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetailSpace(42);
                    gtf.SetTotalDetail(ttlQtyAdjPlus.ToString(), 9, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetail(ttlQtyAdjMinus.ToString(), 9, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetail(ttlCstPlus.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(ttlCstMns.ToString(), 11, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetailSpace(74);
                    gtf.SetTotalDetail(setTextParameter[0].ToString(), 22, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(74);
                    gtf.SetTotalDetail(setTextParameter[1].ToString(), 22, ' ', false, true);
                    gtf.SetTotalDetailSpace(74);
                    gtf.SetTotalDetail("-", 22, '-', false, true);
                    gtf.SetTotalDetailSpace(74);
                    gtf.SetTotalDetail(setTextParameter[2].ToString(), 22, ' ', false, true);
                    gtf.PrintTotal(false, lastData, true);

                    ttlQtyAdjPlus = ttlQtyAdjMinus = ttlCstPlus = ttlCstMns = 0;

                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["AdjustmentNo"].ToString(), dt.Rows[i]["AdjustmentNo"].ToString(), 13);
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["ReferenceNo"].ToString(), dt.Rows[i]["ReferenceNo"].ToString(), 46);
                    gtf.ReplaceGroupHdr(DateTime.Parse(dt.Rows[i - 1]["AdjustmentDate"].ToString()).ToString("dd-MMM-yyyy"), DateTime.Parse(dt.Rows[i]["AdjustmentDate"].ToString()).ToString("dd-MMM-yyyy"));

                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 18, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyAdjPlus"].ToString(), 9, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["QtyAdjMinus"].ToString(), 9, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["CostPricePlus"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["CostPriceMinus"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AdjustmentReason"].ToString(), 10, ' ', false, true);
                    gtf.PrintData(true);

                    docNo = dt.Rows[i]["AdjustmentNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["AdjustmentNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 18, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WarehouseCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyAdjPlus"].ToString(), 9, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["QtyAdjMinus"].ToString(), 9, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["CostPricePlus"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["CostPriceMinus"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AdjustmentReason"].ToString(), 10, ' ', false, true);

                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["AdjustmentNo"].ToString())
                            gtf.PrintData(true);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlCstMns += decimal.Parse(dt.Rows[i]["CostPriceMinus"].ToString());
                ttlCstPlus += decimal.Parse(dt.Rows[i]["CostPricePlus"].ToString());
                ttlQtyAdjMinus += decimal.Parse(dt.Rows[i]["QtyAdjMinus"].ToString());
                ttlQtyAdjPlus += decimal.Parse(dt.Rows[i]["QtyAdjPlus"].ToString());

            }
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetailSpace(42);
            gtf.SetTotalDetail(ttlQtyAdjPlus.ToString(), 9, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetail(ttlQtyAdjMinus.ToString(), 9, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetail(ttlCstPlus.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlCstMns.ToString(), 11, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetailSpace(74);
            gtf.SetTotalDetail(setTextParameter[0].ToString() + Convert.ToDateTime(dt.Rows[0]["AdjustmentDate"].ToString()).ToString("dd-MMM-yyyy"), 22, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(74);
            gtf.SetTotalDetail(setTextParameter[1].ToString(), 22, ' ', false, true);
            gtf.SetTotalDetailSpace(74);
            gtf.SetTotalDetail("-", 22, '-', false, true);
            gtf.SetTotalDetailSpace(74);
            gtf.SetTotalDetail(setTextParameter[2].ToString(), 22, ' ', false, true);


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