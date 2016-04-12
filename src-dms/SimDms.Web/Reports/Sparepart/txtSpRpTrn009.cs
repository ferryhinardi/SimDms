using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn009 : IRptProc
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
            return CreateReportSpRpTrn009(rptId, dt, sparam, printerloc, print, "", fullpage);
        }

        #region SpRpTrn009
        private string CreateReportSpRpTrn009(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            string[] arrayHeader = new string[9];
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
            arrayHeader[0] = dt.Rows[0]["DocNo"].ToString();
            arrayHeader[1] = dt.Rows[0]["CustomerName"].ToString();
            arrayHeader[2] = dt.Rows[0]["DocDate"] != null ? Convert.ToDateTime(dt.Rows[0]["DocDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[3] = dt.Rows[0]["CustAddress1"].ToString();
            arrayHeader[4] = dt.Rows[0]["CustAddress2"].ToString();
            arrayHeader[5] = dt.Rows[0]["CustAddress3"].ToString();
            arrayHeader[6] = dt.Rows[0]["CustAddress4"].ToString();
            arrayHeader[7] = dt.Rows[0]["OrderNo"].ToString();
            arrayHeader[8] = dt.Rows[0]["OrderDate"] != null ? Convert.ToDateTime(dt.Rows[0]["OrderDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            CreateHdrSpRpTrn009(gtf, arrayHeader, fullPage);
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            decimal ttlQty = 0, ttlTotal = 0;
            bool lastData = false;
            string cityName = "", place = "";
            string[] terbilang;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["DocNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 22, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["CostPrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["CostPrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Jumlah"].ToString(), 16, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPage);
                }
                else if (docNo != dt.Rows[i]["DocNo"].ToString())
                {

                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("TOTAL : ", 43, ' ', true);
                    gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetailSpace(27);
                    gtf.SetTotalDetail(ttlTotal.ToString(), 16, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();

                    terbilang = gtf.ConvertArrayTerbilang(gtf.Terbilang(Convert.ToInt64(ttlTotal)).ToString(), 50);
                    gtf.SetTotalDetail("TERBILANG : ", 12, ' ');
                    gtf.SetTotalDetail(terbilang[0] != null ? terbilang[0].ToString() : " ", 50, ' ', true);

                    cityName = dt.Rows[i - 1]["CoCity"].ToString() == "" ? string.Empty : dt.Rows[i - 1]["CoCity"].ToString();
                    place = cityName.ToUpper() + ", " + DateTime.Now.ToString("dd MMMM yyyy").ToUpper();

                    gtf.SetTotalDetail(place.ToString(), 34, ' ', false, true);
                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(terbilang[1] != null ? terbilang[1].ToString() : " ", 50, ' ', false, true);
                    if (fullPage == false)
                    {
                        if (terbilang[2] != null)
                        {
                            gtf.SetTotalDetailSpace(12);
                            gtf.SetTotalDetail(terbilang[2].ToString(), 50, ' ', false, true);
                        }
                        else
                            gtf.SetTotalDetail("", 1, ' ', false, true);
                    }
                    else
                    {
                        gtf.SetTotalDetailSpace(12);
                        gtf.SetTotalDetail(terbilang[2] != null ? terbilang[2].ToString() : " ", 50, ' ', false, true);
                    }
                    gtf.SetTotalDetailSpace(63);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName"] != null ? dt.Rows[i - 1]["SignName"].ToString() : " ", 44, ' ', false, true);
                    gtf.SetTotalDetailSpace(63);
                    gtf.SetTotalDetail("-", 33, '-', false, true);
                    gtf.SetTotalDetailSpace(63);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign"] != null ? dt.Rows[i - 1]["TitleSign"].ToString() : " ", 44, ' ', false, true);
                    gtf.PrintTotal(false, lastData, false);

                    ttlQty = 0;

                    arrayHeader[0] = dt.Rows[i]["DocNo"].ToString();
                    arrayHeader[1] = dt.Rows[i]["CustomerName"].ToString();
                    arrayHeader[2] = dt.Rows[i]["DocDate"] != null ? Convert.ToDateTime(dt.Rows[i]["DocDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[3] = dt.Rows[i]["CustAddress1"].ToString();
                    arrayHeader[4] = dt.Rows[i]["CustAddress2"].ToString();
                    arrayHeader[5] = dt.Rows[i]["CustAddress3"].ToString();
                    arrayHeader[6] = dt.Rows[i]["CustAddress4"].ToString();
                    arrayHeader[7] = dt.Rows[i]["OrderNo"].ToString();
                    arrayHeader[8] = dt.Rows[i]["OrderDate"] != null ? Convert.ToDateTime(dt.Rows[i]["OrderDate"].ToString()).ToString("dd-MMM-yyyy") : "";

                    CreateHdrSpRpTrn009(gtf, arrayHeader, fullPage);

                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 22, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["CostPrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["CostPrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Jumlah"].ToString(), 16, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPage);

                    docNo = dt.Rows[i]["DocNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["DocNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 22, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["CostPrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["CostPrice"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Jumlah"].ToString(), 16, ' ', false, true, true, true, "n0");
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
                ttlTotal += decimal.Parse(dt.Rows[i]["Jumlah"].ToString());
            }
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("TOTAL : ", 43, ' ', true);
            gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetailSpace(27);
            gtf.SetTotalDetail(ttlTotal.ToString(), 16, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();

            terbilang = gtf.ConvertArrayTerbilang(gtf.Terbilang(Convert.ToInt64(ttlTotal)).ToString(), 50);
            gtf.SetTotalDetail("TERBILANG : ", 12, ' ');
            gtf.SetTotalDetail(terbilang[0] != null ? terbilang[0].ToString() : " ", 50, ' ', true);

            cityName = dt.Rows[dt.Rows.Count - 1]["CoCity"].ToString() == "" ? string.Empty : dt.Rows[dt.Rows.Count - 1]["CoCity"].ToString();
            place = cityName.ToUpper() + ", " + DateTime.Now.ToString("dd MMMM yyyy").ToUpper();

            gtf.SetTotalDetail(place.ToString(), 34, ' ', false, true);
            gtf.SetTotalDetailSpace(12);
            gtf.SetTotalDetail(terbilang[1] != null ? terbilang[1].ToString() : " ", 50, ' ', false, true);
            if (fullPage == false)
            {
                if (terbilang[2] != null)
                {
                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(terbilang[2].ToString(), 50, ' ', false, true);
                }
                else
                    gtf.SetTotalDetail("", 1, ' ', false, true);
            }
            else
            {
                gtf.SetTotalDetailSpace(12);
                gtf.SetTotalDetail(terbilang[2] != null ? terbilang[2].ToString() : " ", 50, ' ', false, true);
            }
            gtf.SetTotalDetailSpace(63);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName"] != null ? dt.Rows[dt.Rows.Count - 1]["SignName"].ToString() : " ", 44, ' ', false, true);
            gtf.SetTotalDetailSpace(63);
            gtf.SetTotalDetail("-", 33, '-', false, true);
            gtf.SetTotalDetailSpace(63);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign"] != null ? dt.Rows[dt.Rows.Count - 1]["TitleSign"].ToString() : " ", 44, ' ', false, true);

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

        private void CreateHdrSpRpTrn009(SpGenerateTextFileReport gtf, string[] arrayHeader, bool fullPage)
        {
            gtf.CleanHeader();

            gtf.SetGroupHeader(setTextParameter[0].ToString(), 94, ' ', false, true, false, true);
            gtf.SetGroupHeader("KEPADA YTH,", 72, ' ', true);
            gtf.SetGroupHeader("NOMOR", 8, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[0].ToString(), 44, ' ', false, true);
            gtf.SetGroupHeader(arrayHeader[1].ToString(), 72, ' ', true);
            gtf.SetGroupHeader("TANGGAL", 8, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[2].ToString(), 44, ' ', false, true);
            gtf.SetGroupHeader(arrayHeader[3].ToString(), 72, ' ', false, true);
            gtf.SetGroupHeader(arrayHeader[4].ToString(), 72, ' ', false, true);

            if (fullPage == false)
            {
                if (arrayHeader[5] != null)
                    gtf.SetGroupHeader(arrayHeader[5].ToString(), 72, ' ', false, true);
                if (arrayHeader[6] != null)
                    gtf.SetGroupHeader(arrayHeader[6].ToString(), 72, ' ', false, true);
            }
            else
            {
                gtf.SetGroupHeader(arrayHeader[5].ToString(), 72, ' ', false, true);
                gtf.SetGroupHeader(arrayHeader[6].ToString(), 72, ' ', false, true);
            }

            gtf.SetGroupHeader("BERDASARKAN REFERENSI", 22, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[7].ToString(), 15, ' ', true);
            gtf.SetGroupHeader(", TANGGAL", 10, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[8].ToString(), 18, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 15, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 22, ' ', true);
            gtf.SetGroupHeader("QUANTITY", 8, ' ', true, false, true);
            gtf.SetGroupHeader("HARGA", 13, ' ', true, false, true);
            gtf.SetGroupHeader("AVG. COST", 13, ' ', true, false, true);
            gtf.SetGroupHeader("JUMLAH", 16, ' ', false, true, true);
            gtf.SetGroupHeaderLine();

        }
        #endregion
    }
}