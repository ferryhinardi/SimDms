using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn031 : IRptProc
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
            return CreateReportSpRpTrn031(rptId, dt, sparam, printerloc, print, "", fullpage);
        }

        #region SpRpTrn031
        private string CreateReportSpRpTrn031(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            string[] arrayHeader = new string[4];
            //----------------------------------------------------------------
            //Default Parameter
            SpGenerateTextFileReport gtf = new SpGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            arrayHeader[0] = dt.Rows[0]["CustomerCode"].ToString();
            arrayHeader[1] = dt.Rows[0]["DocNo"].ToString();
            arrayHeader[2] = dt.Rows[0]["DocDate"] != null ? ": " + Convert.ToDateTime(dt.Rows[0]["DocDate"].ToString()).ToString("dd-MMM-yyyy") : ":";
            arrayHeader[3] = dt.Rows[0]["TRANSCODE"].ToString();

            CreateHdrSpRpTrn031(gtf, arrayHeader);
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            decimal ttlQty = 0, ttlTotal = 0, ttlDisc = 0;
            bool lastData = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["DocNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 19, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["RetailPrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 7, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 12, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["NetSalesAmt"].ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPage);
                }
                else if (docNo != dt.Rows[i]["DocNo"].ToString())
                {

                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("TOTAL : ", 40, ' ', true);
                    gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetailSpace(20);
                    gtf.SetTotalDetail(ttlDisc.ToString(), 12, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(ttlTotal.ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("KETERANGAN :", 70, ' ');
                    gtf.SetTotalDetail("DPP     :", 9, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["DPPAmt"].ToString(), 16, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetail("DPP INI TIDAK BERSIFAT MENGIKAT", 70, ' ');
                    gtf.SetTotalDetail("PPN     :", 9, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["PPNAmt"].ToString(), 16, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetail("HARAP BAWA DOKUMEN INI, BILA JADI BELI", 70, ' ');
                    gtf.SetTotalDetail("JUMLAH  :", 9, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["Jumlah"].ToString(), 16, ' ', false, true, true, true, "n0");

                    gtf.PrintTotal(false, lastData, false);

                    ttlQty = ttlTotal = ttlDisc = 0;

                    arrayHeader[0] = dt.Rows[i]["CustomerCode"].ToString();
                    arrayHeader[1] = dt.Rows[i]["DocNo"].ToString();
                    arrayHeader[2] = dt.Rows[i]["DocDate"] != null ? ": " + Convert.ToDateTime(dt.Rows[0]["DocDate"].ToString()).ToString("dd-MMM-yyyy") : ":";
                    arrayHeader[3] = dt.Rows[i]["TRANSCODE"].ToString();

                    CreateHdrSpRpTrn031(gtf, arrayHeader);

                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 19, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["RetailPrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 7, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 12, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["NetSalesAmt"].ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPage);

                    docNo = dt.Rows[i]["DocNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["DocNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 19, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["RetailPrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 7, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 12, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["NetSalesAmt"].ToString(), 13, ' ', false, true, true, true, "n0");
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["DocNo"].ToString())
                            gtf.PrintData(fullPage);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlQty += decimal.Parse(dt.Rows[i]["QtyOrder"].ToString());
                ttlTotal += decimal.Parse(dt.Rows[i]["NetSalesAmt"].ToString());
                ttlDisc += decimal.Parse(dt.Rows[i]["DiscAmt"].ToString());

            }
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("TOTAL : ", 40, ' ', true);
            gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetailSpace(20);
            gtf.SetTotalDetail(ttlDisc.ToString(), 12, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlTotal.ToString(), 13, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("KETERANGAN :", 70, ' ');
            gtf.SetTotalDetail("DPP     :", 9, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["DPPAmt"].ToString(), 16, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetail("DPP INI TIDAK BERSIFAT MENGIKAT", 70, ' ');
            gtf.SetTotalDetail("PPN     :", 9, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["PPNAmt"].ToString(), 16, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetail("HARAP BAWA DOKUMEN INI, BILA JADI BELI", 70, ' ');
            gtf.SetTotalDetail("JUMLAH  :", 9, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["Jumlah"].ToString(), 16, ' ', false, true, true, true, "n0");

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

        private void CreateHdrSpRpTrn031(SpGenerateTextFileReport gtf, string[] arrayHeader)
        {
            gtf.CleanHeader();

            gtf.SetGroupHeader("CUST  : ", 8, ' ');
            gtf.SetGroupHeader(arrayHeader[0].ToString(), 90, ' ', false, true);
            gtf.SetGroupHeader("NOMOR : ", 8, ' ');
            gtf.SetGroupHeader(arrayHeader[1].ToString(), 15, ' ', true);
            gtf.SetGroupHeader("TANGGAL : ", 10, ' ');
            gtf.SetGroupHeader(arrayHeader[2].ToString(), 37, ' ', true);
            gtf.SetGroupHeader("TIPE TRANSAKSI : ", 17, ' ');
            gtf.SetGroupHeader(arrayHeader[3].ToString(), 7, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 15, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 19, ' ', true);
            gtf.SetGroupHeader("QTY", 8, ' ', true, false, true);
            gtf.SetGroupHeader("HARGA JUAL", 11, ' ', true, false, true);
            gtf.SetGroupHeader("DISC (%)", 7, ' ', true, false, true);
            gtf.SetGroupHeader("DISC (NILAI)", 12, ' ', true, false, true);
            gtf.SetGroupHeader("JUMLAH", 13, ' ', false, true);
            gtf.SetGroupHeaderLine();

        }
        #endregion
    }
}