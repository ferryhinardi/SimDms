using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn028 : IRptProc
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
            return CreateReportSpRpTrn028(rptId, dt, sparam, printerloc, print, "", fullpage);
        }

        #region SpRpTrn028
        private string CreateReportSpRpTrn028(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            string[] arrayHeader = new string[6];
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
            arrayHeader[0] = dt.Rows[0]["CustomerName"].ToString();
            arrayHeader[1] = dt.Rows[0]["SKPNo"].ToString();
            arrayHeader[2] = dt.Rows[0]["NPWPNo"].ToString();
            arrayHeader[3] = dt.Rows[0]["PKP"].ToString();
            arrayHeader[4] = dt.Rows[0]["SKPDate"] != null ? Convert.ToDateTime(dt.Rows[0]["SKPDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[5] = dt.Rows[0]["LmpNo"].ToString();

            CreateHdrSpRpTrn028(gtf, arrayHeader);
            gtf.PrintHeader();

            string docNo = "", city = "";
            int noUrut = 0;
            decimal ttlQty = 0, ttlTotal = 0;
            bool lastData = false;
            string[] terbilang;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["LmpNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["DocNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DocDate"] != null ? Convert.ToDateTime(dt.Rows[i]["DocDate"].ToString()).ToString("dd-MMM-yyyy") : "", 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 16, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyBill"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["RetailPrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["NetSalesAmt"].ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPage);
                }
                else if (docNo != dt.Rows[i]["LmpNo"].ToString())
                {

                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("TOTAL : ", 61, ' ', true);
                    gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(ttlTotal.ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("TERBILANG : ", 12, ' ');

                    terbilang = gtf.ConvertArrayTerbilang(gtf.Terbilang(Convert.ToInt64(dt.Rows[i - 1]["TotFinalSalesAmt"].ToString())), 52);
                    gtf.SetTotalDetail(terbilang[0].ToString(), 52, ' ', true);

                    gtf.SetTotalDetail("TOTAL", 15, ' ');
                    gtf.SetGroupHeader(":", 1, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotDPPAmt"].ToString(), 16, ' ', false, true, true, true, "n0");

                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(terbilang[1] != null ? terbilang[1].ToString() : string.Empty, 52, ' ', true);
                    gtf.SetTotalDetail("PPN", 15, ' ');
                    gtf.SetGroupHeader(":", 1, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotPPNAmt"].ToString(), 16, ' ', false, true, true, true, "n0");

                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(terbilang[2] != null ? terbilang[2].ToString() : string.Empty, 52, ' ', true);
                    gtf.SetTotalDetail("JUMLAH DIBAYAR", 15, ' ');
                    gtf.SetGroupHeader(":", 1, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotFinalSalesAmt"].ToString(), 16, ' ', false, true, true, true, "n0");

                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(65);

                    city = (dt.Rows[i - 1]["city"].ToString() != "") ? dt.Rows[i - 1]["city"].ToString() + ", " +
                        Convert.ToDateTime(dt.Rows[i - 1]["date"].ToString()).ToString("dd MMMM yyyy").ToUpper() : string.Empty;

                    gtf.SetTotalDetail(city, 31, ' ');
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    if (fullPage == true)
                        gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(65);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName"].ToString(), 25, ' ', false, true);
                    gtf.SetTotalDetailSpace(65);
                    gtf.SetTotalDetail("-", 25, '-', false, true);
                    gtf.SetTotalDetailSpace(65);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign"].ToString(), 25, ' ', false, true);

                    gtf.PrintTotal(false, lastData, false);

                    ttlQty = ttlTotal = 0;

                    arrayHeader[0] = dt.Rows[i]["CustomerName"].ToString();
                    arrayHeader[1] = dt.Rows[i]["SKPNo"].ToString();
                    arrayHeader[2] = dt.Rows[i]["NPWPNo"].ToString();
                    arrayHeader[3] = dt.Rows[i]["PKP"].ToString();
                    arrayHeader[4] = dt.Rows[i]["SKPDate"] != null ? Convert.ToDateTime(dt.Rows[i]["SKPDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[5] = dt.Rows[i]["LmpNo"].ToString();

                    CreateHdrSpRpTrn028(gtf, arrayHeader);

                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["DocNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DocDate"] != null ? Convert.ToDateTime(dt.Rows[i]["DocDate"].ToString()).ToString("dd-MMM-yyyy") : "", 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 16, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyBill"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["RetailPrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["NetSalesAmt"].ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPage);

                    docNo = dt.Rows[i]["LmpNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["LmpNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["DocNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["DocDate"] != null ? Convert.ToDateTime(dt.Rows[i]["DocDate"].ToString()).ToString("dd-MMM-yyyy") : "", 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 16, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyBill"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["RetailPrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["NetSalesAmt"].ToString(), 13, ' ', false, true, true, true, "n0");
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["LmpNo"].ToString())
                            gtf.PrintData(fullPage);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlQty += decimal.Parse(dt.Rows[i]["QtyBill"].ToString());
                ttlTotal += decimal.Parse(dt.Rows[i]["NetSalesAmt"].ToString());
            }
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("TOTAL : ", 61, ' ', true);
            gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetailSpace(12);
            gtf.SetTotalDetail(ttlTotal.ToString(), 13, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("TERBILANG : ", 12, ' ');

            terbilang = gtf.ConvertArrayTerbilang(gtf.Terbilang(Convert.ToInt64(dt.Rows[dt.Rows.Count - 1]["TotFinalSalesAmt"])), 52);
            gtf.SetTotalDetail(terbilang[0].ToString(), 52, ' ', true);

            gtf.SetTotalDetail("TOTAL", 15, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotDPPAmt"].ToString(), 16, ' ', false, true, true, true, "n0");

            gtf.SetTotalDetailSpace(12);
            gtf.SetTotalDetail(terbilang[1] != null ? terbilang[1].ToString() : string.Empty, 52, ' ', true);
            gtf.SetTotalDetail("PPN", 15, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotPPNAmt"].ToString(), 16, ' ', false, true, true, true, "n0");

            gtf.SetTotalDetailSpace(12);
            gtf.SetTotalDetail(terbilang[2] != null ? terbilang[2].ToString() : string.Empty, 52, ' ', true);
            gtf.SetTotalDetail("JUMLAH DIBAYAR", 15, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotFinalSalesAmt"].ToString(), 16, ' ', false, true, true, true, "n0");

            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(65);

            city = (dt.Rows[dt.Rows.Count - 1]["city"].ToString() != "") ? dt.Rows[dt.Rows.Count - 1]["city"].ToString() + ", " +
            Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1]["date"].ToString()).ToString("dd MMMM yyyy").ToUpper() : string.Empty;

            gtf.SetTotalDetail(city, 31, ' ');
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            if (fullPage == true)
                gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(65);
            gtf.SetTotalDetail(setTextParameter[0] != null ? setTextParameter[0].ToString() : "" , 25, ' ', false, true);
            
            gtf.SetTotalDetailSpace(65);
            gtf.SetTotalDetail("-", 25, '-', false, true);
            gtf.SetTotalDetailSpace(65);
            gtf.SetTotalDetail(setTextParameter[1] != null ? setTextParameter[1].ToString() : "", 25, ' ', false, true);

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

        private void CreateHdrSpRpTrn028(SpGenerateTextFileReport gtf, string[] arrayHeader)
        {
            gtf.CleanHeader();

            gtf.SetGroupHeader("PELANGGAN", 13, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[0].ToString(), 55, ' ', true);
            gtf.SetGroupHeader("NO. SPK", 10, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[1].ToString(), 13, ' ', false, true);
            gtf.SetGroupHeader("NPWP", 13, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[2].ToString(), 30, ' ', true);
            gtf.SetGroupHeader("PKP", 4, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[3].ToString(), 18, ' ', true);
            gtf.SetGroupHeader("TGL. SPK", 10, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[4].ToString(), 13, ' ', false, true);
            gtf.SetGroupHeader("NO. LAMPIRAN", 13, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[5].ToString(), 56, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NO. SS", 13, ' ', true);
            gtf.SetGroupHeader("TGL. SS", 11, ' ', true);
            gtf.SetGroupHeader("NO. PART", 15, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 15, ' ', true);
            gtf.SetGroupHeader("QTY", 7, ' ', true, false, true);
            gtf.SetGroupHeader("HRG. SATUAN", 11, ' ', true, false, true);
            gtf.SetGroupHeader("JUMLAH", 13, ' ', false, true, true);
            gtf.SetGroupHeaderLine();

        }
        #endregion
    }
}