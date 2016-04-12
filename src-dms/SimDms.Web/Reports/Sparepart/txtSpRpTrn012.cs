using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn012 : IRptProc
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
            if (setTextParameter[0].ToString()=="SpRpTrn012A")
            {
                return CreateReportSpRpTrn012A(rptId, dt, sparam, printerloc, print, "", fullpage);
            }
            else
            {
                return CreateReportSpRpTrn012(rptId, dt, sparam, printerloc, print, "", fullpage);
            }
            
        }

        #region SpRpTrn012A

        private string CreateReportSpRpTrn012A(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            string[] arrayHeader = new string[10];

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

            arrayHeader[0] = dt.Rows[0]["ReferenceNo"].ToString();
            arrayHeader[1] = dt.Rows[0]["InvoiceNo"].ToString();
            arrayHeader[2] = dt.Rows[0]["ReferenceDate"] != null ? Convert.ToDateTime(dt.Rows[0]["ReferenceDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[3] = dt.Rows[0]["ReturnDate"] != null ? Convert.ToDateTime(dt.Rows[0]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[4] = dt.Rows[0]["CustomerName"].ToString() != "" ? dt.Rows[0]["CustomerName"].ToString() : string.Empty;
            arrayHeader[9] = dt.Rows[0]["CustAddress1"].ToString() != "" ? dt.Rows[0]["CustAddress1"].ToString() : string.Empty;
            arrayHeader[5] = dt.Rows[0]["CustCityName"].ToString() != "" ? dt.Rows[0]["CustCityName"].ToString() : string.Empty;
            arrayHeader[6] = dt.Rows[0]["CustPhoneNo"].ToString() != "" ? dt.Rows[0]["CustPhoneNo"].ToString() : string.Empty;
            arrayHeader[7] = dt.Rows[0]["FPJNo"].ToString() != "" ? "UNTUK MENGEMBALIKAN SPAREPARTS, DARI FAKTUR PENJUALAN NOMOR : " + dt.Rows[0]["FPJNo"].ToString() + "," : string.Empty;
            arrayHeader[8] = (dt.Rows[0]["FPJDate"].ToString() != "" && dt.Rows[0]["CompanyName"].ToString() != "") ? "TANGGAL : " + Convert.ToDateTime(dt.Rows[0]["FPJDate"].ToString()).ToString("dd-MMM-yyyy") + " PADA " + dt.Rows[0]["CompanyName"].ToString().ToString() : "TANGGAL : ";

            CreateHdrSpRpTrn012(gtf, arrayHeader, fullPage);


            gtf.PrintHeader();

            string docNo = "", city = "", returnNo = "";
            string[] terbilang = new string[3];
            int noUrut = 0;
            decimal ttlQty = 0, ttlTotal = 0, ttlNilai = 0;
            bool lastData = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["InvoiceNo"].ToString();
                    returnNo = dt.Rows[i]["ReturnNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail("NO. RETURN : ", 13, ' ');
                    gtf.SetDataDetail(dt.Rows[i]["ReturnNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail("TANGGAL RETURN : ", 17, ' ');
                    gtf.SetDataDetail(dt.Rows[i]["ReturnDate"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy") : string.Empty, 15, ' ', false, true);
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 25, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyReturn"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["RetailPrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["NetReturAmt"].ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);
                }
                else if (docNo != dt.Rows[i]["InvoiceNo"].ToString())
                {
                    gtf.SetTotalDetail("KETERANGAN :", 96, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    if (fullPage == true)
                        gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("TOTAL : ", 45, ' ', true);
                    gtf.SetTotalDetail(ttlQty.ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetailSpace(18);
                    gtf.SetTotalDetail(ttlNilai.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(ttlTotal.ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("TERBILANG : ", 12, ' ');

                    terbilang = gtf.ConvertArrayTerbilang(gtf.Terbilang(Convert.ToInt64(dt.Rows[i - 1]["TotFinalReturAmt"].ToString())), 53);
                    gtf.SetTotalDetail(terbilang[0].ToString(), 53, ' ', true);
                    gtf.SetTotalDetail("D.P.P", 12, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotDPPAmt"].ToString(), 16, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(terbilang[1] != null ? terbilang[1].ToString() : string.Empty, 53, ' ', true);
                    gtf.SetTotalDetail("PPN 10.00%", 12, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotPPNAmt"].ToString(), 16, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(terbilang[2] != null ? terbilang[2].ToString() : string.Empty, 53, ' ', true);
                    gtf.SetTotalDetail("PPN 10.00%", 12, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotFinalReturAmt"].ToString(), 16, ' ', false, true, true, true, "n0");

                    city = dt.Rows[i - 1]["CityName"].ToString() != "" ? dt.Rows[i - 1]["CityName"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["ReturnDate"].ToString()).ToString("dd MMMM yyyy").ToUpper() : string.Empty;

                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail(city.ToString(), 30, ' ', false, true);
                    gtf.SetTotalDetail("KETERANGAN", 66, ' ', false, true);
                    gtf.SetTotalDetail("Harap dibuatkan Credit Note", 66, ' ', false, true);
                    if (fullPage == true)
                        gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName"].ToString(), 25, ' ', false, true);
                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail("-", 25, '-', false, true);
                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign"].ToString(), 25, ' ', false, true);

                    gtf.PrintTotal(false, lastData, true);

                    ttlQty = ttlTotal = ttlNilai = 0;

                    arrayHeader[0] = dt.Rows[i]["ReferenceNo"].ToString();
                    arrayHeader[1] = dt.Rows[i]["ReturnNo"].ToString();
                    arrayHeader[2] = dt.Rows[i]["ReferenceDate"] != null ? Convert.ToDateTime(dt.Rows[i]["ReferenceDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[3] = dt.Rows[i]["ReturnDate"] != null ? Convert.ToDateTime(dt.Rows[i]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[4] = dt.Rows[i]["CustomerName"].ToString() != "" ? dt.Rows[i]["CustomerName"].ToString() : string.Empty;
                    arrayHeader[5] = dt.Rows[i]["CustCityName"].ToString() != "" ? dt.Rows[i]["CustCityName"].ToString() : string.Empty;
                    arrayHeader[6] = dt.Rows[i]["CustPhoneNo"].ToString() != "" ? dt.Rows[i]["CustPhoneNo"].ToString() : string.Empty;
                    arrayHeader[7] = dt.Rows[i]["FPJNo"].ToString() != "" ? "UNTUK MENGEMBALIKAN SPAREPARTS, DARI FAKTUR PENJUALAN NOMOR : " + dt.Rows[i]["FPJNo"].ToString() + "," : string.Empty;
                    arrayHeader[8] = (dt.Rows[i]["FPJDate"].ToString() != "" && dt.Rows[i]["CompanyName"].ToString() != "") ? "TANGGAL : " + Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy") + " PADA " + dt.Rows[i]["CompanyName"].ToString().ToString() : "TANGGAL : ";
                    arrayHeader[9] = dt.Rows[i]["CustAddress1"].ToString() != "" ? dt.Rows[i]["CustAddress1"].ToString() : string.Empty;

                    CreateHdrSpRpTrn012(gtf, arrayHeader, fullPage);

                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    if (returnNo != dt.Rows[i]["ReturnNo"].ToString())
                    {
                        gtf.SetDataDetail("NO. RETURN : ", 13, ' ');
                        gtf.SetDataDetail(dt.Rows[i]["ReturnNo"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail("TANGGAL RETURN : ", 17, ' ');
                        gtf.SetDataDetail(dt.Rows[i]["ReturnDate"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy") : string.Empty, 15, ' ', false, true);
                        returnNo = dt.Rows[i]["ReturnNo"].ToString();
                    }
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 25, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyReturn"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["RetailPrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["NetReturAmt"].ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);

                    docNo = dt.Rows[i]["InvoiceNo"].ToString();

                }
                else if (docNo == dt.Rows[i]["InvoiceNo"].ToString())
                {
                    noUrut++;
                    if (returnNo != dt.Rows[i]["ReturnNo"].ToString())
                    {
                        gtf.SetDataDetail("NO. RETURN : ", 13, ' ');
                        gtf.SetDataDetail(dt.Rows[i]["ReturnNo"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail("TANGGAL RETURN : ", 17, ' ');
                        gtf.SetDataDetail(dt.Rows[i]["ReturnDate"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy") : string.Empty, 15, ' ', false, true);
                        returnNo = dt.Rows[i]["ReturnNo"].ToString();
                    }
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 25, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyReturn"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["RetailPrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["NetReturAmt"].ToString(), 13, ' ', false, true, true, true, "n0");
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["InvoiceNo"].ToString())
                            gtf.PrintData(true);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlQty += decimal.Parse(dt.Rows[i]["QtyReturn"].ToString());
                ttlTotal += decimal.Parse(dt.Rows[i]["NetReturAmt"].ToString());
                ttlNilai += decimal.Parse(dt.Rows[i]["DiscAmt"].ToString());
            }
            gtf.SetTotalDetail("KETERANGAN :", 96, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            if (fullPage == true)
                gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("TOTAL : ", 45, ' ', true);
            gtf.SetTotalDetail(ttlQty.ToString(), 6, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetailSpace(18);
            gtf.SetTotalDetail(ttlNilai.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlTotal.ToString(), 13, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("TERBILANG : ", 12, ' ');

            terbilang = gtf.ConvertArrayTerbilang(gtf.Terbilang(Convert.ToInt64(dt.Rows[dt.Rows.Count - 1]["TotFinalReturAmt"].ToString())), 53);
            gtf.SetTotalDetail(terbilang[0].ToString(), 53, ' ', true);
            gtf.SetTotalDetail("D.P.P", 12, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotDPPAmt"].ToString(), 16, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailSpace(12);
            gtf.SetTotalDetail(terbilang[1] != null ? terbilang[1].ToString() : string.Empty, 53, ' ', true);
            gtf.SetTotalDetail("PPN 10.00%", 12, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotPPNAmt"].ToString(), 16, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailSpace(12);
            gtf.SetTotalDetail(terbilang[2] != null ? terbilang[2].ToString() : string.Empty, 53, ' ', true);
            gtf.SetTotalDetail("PPN 10.00%", 12, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotFinalReturAmt"].ToString(), 16, ' ', false, true, true, true, "n0");

            city = dt.Rows[dt.Rows.Count - 1]["CityName"].ToString() != "" ? dt.Rows[dt.Rows.Count - 1]["CityName"].ToString() + ", " + Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1]["ReturnDate"].ToString()).ToString("dd MMMM yyyy").ToUpper() : string.Empty;

            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail(city.ToString(), 30, ' ', false, true);
            gtf.SetTotalDetail("KETERANGAN", 66, ' ', false, true);
            gtf.SetTotalDetail("Harap dibuatkan Credit Note", 66, ' ', false, true);
            if (fullPage == true)
                gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName"].ToString(), 25, ' ', false, true);
            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail("-", 25, '-', false, true);
            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign"].ToString(), 25, ' ', false, true);

            if (print == true)
                gtf.PrintTotal(true, lastData, true);
            else
            {
                if (gtf.PrintTotal(true, lastData, true) == true)
                    //XMessageBox.ShowInformation("Save Berhasil");
                    msg = "Save Berhasil";
                else
                    //XMessageBox.ShowWarning("Save Gagal");
                    msg = "Save Gagal";
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSpRpTrn012(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            string[] arrayHeader = new string[10];

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

            arrayHeader[0] = dt.Rows[0]["ReferenceNo"].ToString();
            arrayHeader[1] = dt.Rows[0]["ReturnNo"].ToString();
            arrayHeader[2] = dt.Rows[0]["ReferenceDate"] != null ? Convert.ToDateTime(dt.Rows[0]["ReferenceDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[3] = dt.Rows[0]["ReturnDate"] != null ? Convert.ToDateTime(dt.Rows[0]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[4] = dt.Rows[0]["CustomerName"].ToString() != "" ? dt.Rows[0]["CustomerName"].ToString() : string.Empty;
            arrayHeader[5] = dt.Rows[0]["CustCityName"].ToString() != "" ? dt.Rows[0]["CustCityName"].ToString() : string.Empty;
            arrayHeader[6] = dt.Rows[0]["CustPhoneNo"].ToString() != "" ? dt.Rows[0]["CustPhoneNo"].ToString() : string.Empty;
            arrayHeader[7] = dt.Rows[0]["FPJNo"].ToString() != "" ? "UNTUK MENGEMBALIKAN SPAREPARTS, DARI FAKTUR PENJUALAN NOMOR : " + dt.Rows[0]["FPJNo"].ToString() + "," : string.Empty;
            arrayHeader[8] = (dt.Rows[0]["FPJDate"].ToString() != "" && dt.Rows[0]["CompanyName"].ToString() != "") ? "TANGGAL : " + Convert.ToDateTime(dt.Rows[0]["FPJDate"].ToString()).ToString("dd-MMM-yyyy") + " PADA " + dt.Rows[0]["CompanyName"].ToString().ToString() : "TANGGAL : ";
            arrayHeader[9] = dt.Rows[0]["CustAddress1"].ToString() != "" ? dt.Rows[0]["CustAddress1"].ToString() : string.Empty;

            CreateHdrSpRpTrn012(gtf, arrayHeader, fullPage);


            gtf.PrintHeader();

            string docNo = "", city = "";
            string[] terbilang = new string[3];
            int noUrut = 0;
            decimal ttlQty = 0, ttlTotal = 0, ttlNilai = 0;
            bool lastData = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["ReturnNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 25, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyReturn"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["RetailPrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["NetReturAmt"].ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);
                }
                else if (docNo != dt.Rows[i]["ReturnNo"].ToString())
                {
                    gtf.SetTotalDetail("KETERANGAN :", 96, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    if (fullPage == true)
                        gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("TOTAL : ", 45, ' ', true);
                    gtf.SetTotalDetail(ttlQty.ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetailSpace(18);
                    gtf.SetTotalDetail(ttlNilai.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(ttlTotal.ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("TERBILANG : ", 12, ' ');

                    terbilang = gtf.ConvertArrayTerbilang(gtf.Terbilang(Convert.ToInt64(dt.Rows[i - 1]["TotFinalReturAmt"].ToString())), 53);
                    gtf.SetTotalDetail(terbilang[0].ToString(), 53, ' ', true);
                    gtf.SetTotalDetail("D.P.P", 12, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotDPPAmt"].ToString(), 16, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(terbilang[1] != null ? terbilang[1].ToString() : string.Empty, 53, ' ', true);
                    gtf.SetTotalDetail("PPN 10.00%", 12, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotPPNAmt"].ToString(), 16, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(terbilang[2] != null ? terbilang[2].ToString() : string.Empty, 53, ' ', true);
                    gtf.SetTotalDetail("PPN 10.00%", 12, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TotFinalReturAmt"].ToString(), 16, ' ', false, true, true, true, "n0");

                    city = dt.Rows[i - 1]["CityName"].ToString() != "" ? dt.Rows[i - 1]["CityName"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["ReturnDate"].ToString()).ToString("dd MMMM yyyy").ToUpper() : string.Empty;

                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail(city.ToString(), 30, ' ', false, true);
                    gtf.SetTotalDetail("KETERANGAN", 66, ' ', false, true);
                    gtf.SetTotalDetail("Harap dibuatkan Credit Note", 66, ' ', false, true);
                    if (fullPage == true)
                        gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName"].ToString(), 25, ' ', false, true);
                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail("-", 25, '-', false, true);
                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign"].ToString(), 25, ' ', false, true);

                    gtf.PrintTotal(false, lastData, true);

                    ttlQty = ttlTotal = ttlNilai = 0;

                    arrayHeader[0] = dt.Rows[i]["ReferenceNo"].ToString();
                    arrayHeader[1] = dt.Rows[i]["ReturnNo"].ToString();
                    arrayHeader[2] = dt.Rows[i]["ReferenceDate"] != null ? Convert.ToDateTime(dt.Rows[i]["ReferenceDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[3] = dt.Rows[i]["ReturnDate"] != null ? Convert.ToDateTime(dt.Rows[i]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[4] = dt.Rows[i]["CustomerName"].ToString() != "" ? dt.Rows[i]["CustomerName"].ToString() : string.Empty;
                    arrayHeader[5] = dt.Rows[i]["CustCityName"].ToString() != "" ? dt.Rows[i]["CustCityName"].ToString() : string.Empty;
                    arrayHeader[6] = dt.Rows[i]["CustPhoneNo"].ToString() != "" ? dt.Rows[i]["CustPhoneNo"].ToString() : string.Empty;
                    arrayHeader[7] = dt.Rows[i]["FPJNo"].ToString() != "" ? "UNTUK MENGEMBALIKAN SPAREPARTS, DARI FAKTUR PENJUALAN NOMOR : " + dt.Rows[i]["FPJNo"].ToString() + "," : string.Empty;
                    arrayHeader[8] = (dt.Rows[i]["FPJDate"].ToString() != "" && dt.Rows[i]["CompanyName"].ToString() != "") ? "TANGGAL : " + Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy") + " PADA " + dt.Rows[i]["CompanyName"].ToString().ToString() : "TANGGAL : ";
                    arrayHeader[9] = dt.Rows[i]["CustAddress1"].ToString() != "" ? dt.Rows[i]["CustAddress1"].ToString() : string.Empty;

                    CreateHdrSpRpTrn012(gtf, arrayHeader,fullPage);

                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 25, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyReturn"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["RetailPrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["NetReturAmt"].ToString(), 13, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);

                    docNo = dt.Rows[i]["ReturnNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["ReturnNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 25, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyReturn"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["RetailPrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["NetReturAmt"].ToString(), 13, ' ', false, true, true, true, "n0");
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["ReturnNo"].ToString())
                            gtf.PrintData(true);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlQty += decimal.Parse(dt.Rows[i]["QtyReturn"].ToString());
                ttlTotal += decimal.Parse(dt.Rows[i]["NetReturAmt"].ToString());
                ttlNilai += decimal.Parse(dt.Rows[i]["DiscAmt"].ToString());
            }
            gtf.SetTotalDetail("KETERANGAN :", 96, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            if (fullPage == true)
                gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("TOTAL : ", 45, ' ', true);
            gtf.SetTotalDetail(ttlQty.ToString(), 6, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetailSpace(18);
            gtf.SetTotalDetail(ttlNilai.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlTotal.ToString(), 13, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("TERBILANG : ", 12, ' ');

            terbilang = gtf.ConvertArrayTerbilang(gtf.Terbilang(Convert.ToInt64(dt.Rows[dt.Rows.Count - 1]["TotFinalReturAmt"].ToString())), 53);
            gtf.SetTotalDetail(terbilang[0].ToString(), 53, ' ', true);
            gtf.SetTotalDetail("D.P.P", 12, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotDPPAmt"].ToString(), 16, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailSpace(12);
            gtf.SetTotalDetail(terbilang[1] != null ? terbilang[1].ToString() : string.Empty, 53, ' ', true);
            gtf.SetTotalDetail("PPN 10.00%", 12, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotPPNAmt"].ToString(), 16, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailSpace(12);
            gtf.SetTotalDetail(terbilang[2] != null ? terbilang[2].ToString() : string.Empty, 53, ' ', true);
            gtf.SetTotalDetail("PPN 10.00%", 12, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotFinalReturAmt"].ToString(), 16, ' ', false, true, true, true, "n0");

            city = dt.Rows[dt.Rows.Count - 1]["CityName"].ToString() != "" ? dt.Rows[dt.Rows.Count - 1]["CityName"].ToString() + ", " + Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1]["ReturnDate"].ToString()).ToString("dd MMMM yyyy").ToUpper() : string.Empty;

            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail(city.ToString(), 30, ' ', false, true);
            gtf.SetTotalDetail("KETERANGAN", 66, ' ', false, true);
            gtf.SetTotalDetail("Harap dibuatkan Credit Note", 66, ' ', false, true);
            if (fullPage == true)
                gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName"].ToString(), 25, ' ', false, true);
            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail("-", 25, '-', false, true);
            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign"].ToString(), 25, ' ', false, true);

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

        private void CreateHdrSpRpTrn012(SpGenerateTextFileReport gtf, string[] arrayHeader, bool fullPage)
        {
            gtf.CleanHeader();

            gtf.SetGroupHeader("BERDASARKAN PERMOHONAN", 25, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[0].ToString(), 44, ' ', true);
            gtf.SetGroupHeader("NOMOR", 9, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[1].ToString(), 13, ' ', false, true);
            gtf.SetGroupHeader("TANGGAL", 25, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[2].ToString(), 44, ' ', true);
            gtf.SetGroupHeader("TANGGAL", 9, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[3].ToString(), 13, ' ', false, true);
            gtf.SetGroupHeader(" ", 1, ' ', false, true);
            gtf.SetGroupHeader("DIBERIKAN PERSETUJUAN KEPADA", 29, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[4].ToString(), 65, ' ', false, true);
            gtf.SetGroupHeaderSpace(31);
            gtf.SetGroupHeader(arrayHeader[9].ToString(), 65, ' ', false, true);
            gtf.SetGroupHeaderSpace(31);
            gtf.SetGroupHeader(arrayHeader[5].ToString(), 65, ' ', false, true);
            gtf.SetGroupHeaderSpace(31);
            gtf.SetGroupHeader(arrayHeader[6].ToString(), 65, ' ', false, true);
            gtf.SetGroupHeader(" ", 1, ' ', false, true);
            gtf.SetGroupHeader(arrayHeader[7].ToString(), 96, ' ', false, true);
            gtf.SetGroupHeader(arrayHeader[8].ToString(), 96, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 15, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 24, ' ', true);
            gtf.SetGroupHeader("QTY", 6, ' ', true, false, true);
            gtf.SetGroupHeader("HARGA", 11, ' ', true, false, true);
            gtf.SetGroupHeader("DISCOUNT", 17, ' ', true, false, false, true);
            gtf.SetGroupHeader("JUMLAH", 13, ' ', false, true, true);
            if (fullPage == true)
            {
                gtf.SetGroupHeaderSpace(65);
                gtf.SetGroupHeader("-", 17, '-', false, true);
            }
            gtf.SetGroupHeaderSpace(65);
            gtf.SetGroupHeader("%", 5, ' ', true, false, true);
            gtf.SetGroupHeader("NILAI", 11, ' ', false, true, true);

            gtf.SetGroupHeaderLine();

        }
        #endregion
    }


}