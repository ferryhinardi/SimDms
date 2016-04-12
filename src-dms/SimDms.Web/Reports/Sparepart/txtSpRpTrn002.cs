using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn002 : IRptProc
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
            return CreateReportSpRpTrn002(rptId, dt, sparam, printerloc, print, "", fullpage);
        }

        private string CreateReportSpRpTrn002(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            SpGenerateTextFileReport gtf = new SpGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation, fullPage);
            gtf.GenerateHeader();
            //gtf.SetGroupHeader(paramReport, paramReport.Length);
            gtf.SetGroupHeader("TIPE PESANAN", 15, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["LookUpValueName"].ToString(), 57, ' ');
            gtf.SetGroupHeader("NOMOR", 7, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["POSNo"].ToString(), 16, ' ', false, true);
            gtf.SetGroupHeader("BACK ORDER", 15, ' ', true);
            gtf.SetGroupHeader(dt.Rows[0]["isBO"].ToString() == "True" ? ": Y" : ": T", 57, ' ');
            gtf.SetGroupHeader("TANGGAL", 7, ' ', true);
            gtf.SetGroupHeader((dt.Rows[0]["POSDate1"] != null) ? ": " + Convert.ToDateTime(dt.Rows[0]["POSDate1"].ToString()).ToString("dd-MMM-yyyy") : ":", 16, ' ', false, true);
            gtf.SetGroupHeader("PEMASOK", 15, ' ', true);
            gtf.SetGroupHeader(": " + dt.Rows[0]["SupplierName"].ToString() + "(" + dt.Rows[0]["SupplierCode"].ToString() + ")", 57, ' ');
            gtf.SetGroupHeader("VIA", 7, ' ', true);
            gtf.SetGroupHeader((dt.Rows[0]["Transportation"] != null) ? ": " + dt.Rows[0]["Transportation"].ToString() : ":", 16, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeaderSpace(68);
            gtf.SetGroupHeader("DISKON", 16, ' ', false, true, false, true);
            gtf.SetGroupHeaderSpace(68);
            gtf.SetGroupHeader("-", 16, '-', false, true);
            gtf.SetGroupHeader("NO", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 15, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 13, ' ', true);
            gtf.SetGroupHeader("QTY", 8, ' ', true, false, true);
            gtf.SetGroupHeader("HARGA BELI", 11, ' ', true, false, true);
            gtf.SetGroupHeader("NILAI BELI", 11, ' ', true, false, true);
            gtf.SetGroupHeader("%", 5, ' ', true, false, true);
            gtf.SetGroupHeader("NILAI", 10, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 11, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            decimal ttlQty = 0, ttlNilaiBeli = 0, ttlDiscAmt = 0, ttlTotal = 0;
            bool lastData = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["POSNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OrderQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PurchasePrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["SalesAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalAmount"].ToString(), 11, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPage);
                }
                else if (docNo != dt.Rows[i]["POSNo"].ToString())
                {

                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("Total : ", 34, ' ', true, false);
                    gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(ttlNilaiBeli.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetailSpace(6);
                    gtf.SetTotalDetail(ttlDiscAmt.ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetail(ttlTotal.ToString(), 11, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();

                    string date = dt.Rows[i - 1]["POSDate"] == null ? string.Empty :
                        Convert.ToDateTime(dt.Rows[i - 1]["POSDate"]).ToString("MMMM yyyy").ToUpper();

                    gtf.SetTotalDetail("PENGIRIMAN : " + date, 61, ' ');

                    string companyGovName = dt.Rows[i - 1]["CompanyGovName"] is DBNull ? string.Empty : (string)dt.Rows[i - 1]["CompanyGovName"];
                    string POSDate = dt.Rows[i - 1]["POSDate"] is DBNull ? string.Empty : Convert.ToDateTime(dt.Rows[i - 1]["POSDate"]).ToString("dd MMMM yyyy").ToUpper();
                    string alamat = string.Format("{0}, {1}", companyGovName, POSDate);

                    gtf.SetTotalDetail(alamat, 30, ' ', false, true);

                    string remark = dt.Rows[i - 1]["Remark"] is DBNull ? string.Empty : (string)dt.Rows[i - 1]["Remark"];
                    string isIncludePPN = "(BELUM TERMASUK PPN)";

                    string remarks = string.IsNullOrEmpty(remark) ? isIncludePPN : string.Format("{0}\n{1}", remark, isIncludePPN);

                    gtf.SetTotalDetail("KETERANGAN : " + remarks.Replace("\n", " "), 61, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(61);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName1"].ToString(), 17, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName2"].ToString(), 17, ' ', false, true);
                    gtf.SetTotalDetailSpace(61);
                    gtf.SetTotalDetail("-", 17, '-', true);
                    gtf.SetTotalDetail("-", 17, '-', false, true);
                    gtf.SetTotalDetailSpace(61);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 17, ' ', true);
                    gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign2"].ToString(), 17, ' ', false, true);
                    //gtf.CheckLastLineforTotal();
                    gtf.PrintTotal(false, lastData, false);

                    //gtf.RestartTotal();
                    ttlQty = ttlNilaiBeli = ttlDiscAmt = ttlTotal = 0;

                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["POSNo"].ToString(), dt.Rows[i]["POSNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["LookUpValueName"].ToString(), dt.Rows[i]["LookUpValueName"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["POSNo"].ToString(), dt.Rows[i]["POSNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["isBO"].ToString() == "True" ? ": Y" : ": T", dt.Rows[i]["isBO"].ToString() == "True" ? ": Y" : ": T");
                    gtf.ReplaceGroupHdr(dt.Rows[i - 1]["POSDate1"].ToString(), dt.Rows[i]["POSDate1"].ToString());
                    string transportNow = (dt.Rows[i]["Transportation"] != null) ? dt.Rows[i]["Transportation"].ToString() : "";
                    string transportBefore = (dt.Rows[i - 1]["Transportation"] != null) ? dt.Rows[i]["Transportation"].ToString() : "";
                    if (transportBefore != "")
                        gtf.ReplaceGroupHdr(transportBefore, transportNow);
                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OrderQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PurchasePrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["SalesAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalAmount"].ToString(), 11, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPage);

                    docNo = dt.Rows[i]["POSNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["POSNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OrderQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["PurchasePrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["SalesAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DiscPct"].ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["DiscAmt"].ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalAmount"].ToString(), 11, ' ', false, true, true, true, "n0");
                    if (i + 1 < dt.Rows.Count)
                    {
                        if (docNo == dt.Rows[i + 1]["POSNo"].ToString())
                            gtf.PrintData(fullPage);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlQty += decimal.Parse(dt.Rows[i]["OrderQty"].ToString());
                ttlDiscAmt += decimal.Parse(dt.Rows[i]["DiscAmt"].ToString());
                ttlNilaiBeli += decimal.Parse(dt.Rows[i]["SalesAmt"].ToString());
                ttlTotal += decimal.Parse(dt.Rows[i]["TotalAmount"].ToString());
            }

            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("Total : ", 34, ' ', true, false);
            gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetailSpace(12);
            gtf.SetTotalDetail(ttlNilaiBeli.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetailSpace(6);
            gtf.SetTotalDetail(ttlDiscAmt.ToString(), 10, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlTotal.ToString(), 11, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();

            string dates = dt.Rows[dt.Rows.Count - 1]["POSDate"] == null ? string.Empty :
                Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1]["POSDate"]).ToString("MMMM yyyy").ToUpper();

            gtf.SetTotalDetail("PENGIRIMAN : " + dates, 43, ' ');

            string companyGovNames = dt.Rows[dt.Rows.Count - 1]["CompanyGovName"] is DBNull ? string.Empty : (string)dt.Rows[dt.Rows.Count - 1]["CompanyGovName"];
            string POSDates = dt.Rows[dt.Rows.Count - 1]["POSDate"] is DBNull ? string.Empty : Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1]["POSDate"]).ToString("dd MMMM yyyy").ToUpper();
            string alamats = string.Format("{0}, {1}", companyGovNames, POSDates);

            gtf.SetTotalDetail(alamats, 30, ' ', false, true);

            string remark1 = dt.Rows[dt.Rows.Count - 1]["Remark"] is DBNull ? string.Empty : (string)dt.Rows[dt.Rows.Count - 1]["Remark"];
            string isIncludePPNs = "(BELUM TERMASUK PPN)";

            string remarkss = string.IsNullOrEmpty(remark1) ? isIncludePPNs : string.Format("{0}\n{1}", remark1, isIncludePPNs);

            gtf.SetTotalDetail("KETERANGAN : " + remarkss, 61, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(43);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName3"].ToString(), 17, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName1"].ToString(), 17, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName2"].ToString(), 17, ' ', false, true);
            gtf.SetTotalDetailSpace(43);
            gtf.SetTotalDetail("-", 17, '-', true);
            gtf.SetTotalDetail("-", 17, '-', true);
            gtf.SetTotalDetail("-", 17, '-', false, true);
            gtf.SetTotalDetailSpace(43);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign3"].ToString(), 17, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign1"].ToString(), 17, ' ', true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign2"].ToString(), 17, ' ', false, true);

            if (print == true)
                gtf.PrintTotal(true, lastData, false);
            else
            {
                if (gtf.PrintTotal(true, lastData, false) == true)
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
    }
}