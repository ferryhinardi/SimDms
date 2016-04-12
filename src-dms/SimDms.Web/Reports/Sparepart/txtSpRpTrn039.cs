using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn039 : IRptProc
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
            return CreateReportSpRpTrn039(rptId, dt, sparam, printerloc, print, "", fullpage);
        }

        #region SpRpTrn039
        private string CreateReportSpRpTrn039(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
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
            arrayHeader[0] = dt.Rows[0]["UsageDocNo"].ToString();
            arrayHeader[1] = dt.Rows[0]["DocNo"].ToString();
            arrayHeader[2] = dt.Rows[0]["UsageDocDate"] != null ? Convert.ToDateTime(dt.Rows[0]["UsageDocDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[3] = dt.Rows[0]["DocDate"] != null ? Convert.ToDateTime(dt.Rows[0]["DocDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[4] = dt.Rows[0]["Customer"].ToString();
            arrayHeader[5] = dt.Rows[0]["TransType"].ToString();

            CreateHdrSpRpTrn039(gtf, arrayHeader);
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            decimal ttlQty = 0;
            bool lastData = false;
            string cityName = "", place = "";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["DocNo"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 29, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["TypeOfGoods"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Model"].ToString(), 27, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["MovingCode"].ToString(), 2, ' ', false, true);
                    gtf.PrintData(fullPage);
                }
                else if (docNo != dt.Rows[i]["DocNo"].ToString())
                {

                    gtf.SetTotalDetailLine();
                    gtf.SetTotalDetail("TOTAL : ", 84, ' ', true);
                    gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', false, true, true, true, "n2");
                    gtf.SetTotalDetailLine();

                    cityName = dt.Rows[i - 1]["CityName"].ToString() == "" ? string.Empty : dt.Rows[i - 1]["CityName"].ToString();
                    place = string.Format("{0}, {1}", cityName, DateTime.Now.ToString("dd MMMM yyyy").ToUpper());
                    gtf.SetTotalDetail(place.ToString(), 96, ' ', false, true, true);

                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("Diterima oleh :", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("Diserahkan oleh :", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("Diperikasa oleh :", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("Dibuat oleh :", 22, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName1"] != null ? dt.Rows[i - 1]["SignName1"].ToString() : " ", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName2"] != null ? dt.Rows[i - 1]["SignName2"].ToString() : " ", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName3"] != null ? dt.Rows[i - 1]["SignName3"].ToString() : " ", 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["SignName4"] != null ? dt.Rows[i - 1]["SignName4"].ToString() : " ", 22, ' ', false, true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("-", 22, '-', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("-", 22, '-', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("-", 22, '-', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail("-", 22, '-', false, true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign2"].ToString(), 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TiteleSign3"] != null ? dt.Rows[i - 1]["TiteleSign3"].ToString() : string.Empty, 22, ' ', true);
                    gtf.SetTotalDetailSpace(1);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["TitleSign4"].ToString(), 22, ' ', false, true);

                    gtf.PrintTotal(false, lastData, false);

                    ttlQty = 0;

                    arrayHeader[0] = dt.Rows[i]["UsageDocNo"].ToString();
                    arrayHeader[1] = dt.Rows[i]["DocNo"].ToString();
                    arrayHeader[2] = dt.Rows[i]["UsageDocDate"] != null ? Convert.ToDateTime(dt.Rows[i]["UsageDocDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[3] = dt.Rows[i]["DocDate"] != null ? Convert.ToDateTime(dt.Rows[i]["DocDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[4] = dt.Rows[i]["Customer"].ToString();
                    arrayHeader[5] = dt.Rows[i]["TransType"].ToString();

                    CreateHdrSpRpTrn039(gtf, arrayHeader);

                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 29, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["TypeOfGoods"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Model"].ToString(), 27, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["MovingCode"].ToString(), 2, ' ', false, true);
                    gtf.PrintData(fullPage);

                    docNo = dt.Rows[i]["DocNo"].ToString();
                }
                else if (docNo == dt.Rows[i]["DocNo"].ToString())
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 29, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["TypeOfGoods"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Model"].ToString(), 27, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyOrder"].ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["MovingCode"].ToString(), 2, ' ', false, true);
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
            }

            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("TOTAL : ", 84, ' ', true);
            gtf.SetTotalDetail(ttlQty.ToString(), 8, ' ', false, true, true, true, "n2");
            gtf.SetTotalDetailLine();

            cityName = dt.Rows[dt.Rows.Count - 1]["CityName"].ToString() == "" ? string.Empty : dt.Rows[dt.Rows.Count - 1]["CityName"].ToString();
            place = string.Format("{0}, {1}", cityName, DateTime.Now.ToString("dd MMMM yyyy").ToUpper());
            gtf.SetTotalDetail(place.ToString(), 96, ' ', false, true, true);

            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("Diterima oleh :", 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("Diserahkan oleh :", 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("Diperikasa oleh :", 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("Dibuat oleh :", 22, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName1"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName2"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName3"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["SignName4"].ToString(), 22, ' ', false, true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("-", 22, '-', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("-", 22, '-', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("-", 22, '-', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail("-", 22, '-', false, true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign1"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign2"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TiteleSign3"].ToString(), 22, ' ', true);
            gtf.SetTotalDetailSpace(1);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TitleSign4"].ToString(), 22, ' ', false, true);

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

        private void CreateHdrSpRpTrn039(SpGenerateTextFileReport gtf, string[] arrayHeader)
        {
            gtf.CleanHeader();

            if (setTextParameter.Length == 2)
                gtf.SetGroupHeader(setTextParameter[1].ToString(), 94, ' ', false, true, false, true);
            else
                gtf.SetGroupHeader(setTextParameter[0].ToString(), 94, ' ', false, true, false, true);

            gtf.SetGroupHeader("NOMOR SPK", 14, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[0].ToString(), 44, ' ', true);
            gtf.SetGroupHeader("NOMOR S.S", 15, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[1].ToString(), 18, ' ', false, true);
            gtf.SetGroupHeader("TANGGAL S.P.K", 14, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[2].ToString(), 44, ' ', true);
            gtf.SetGroupHeader("TANGGAL S.S", 15, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[3].ToString(), 18, ' ', false, true);
            gtf.SetGroupHeader("PELANGGAN", 14, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[4].ToString(), 44, ' ', true);
            gtf.SetGroupHeader("KODE TRANSAKSI", 15, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[5].ToString(), 18, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO", 3, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 15, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 29, ' ', true);
            gtf.SetGroupHeader("JNS.PART", 8, ' ', true);
            gtf.SetGroupHeader("MODEL", 26, ' ', true);
            gtf.SetGroupHeader("QTY", 7, ' ', true, false, true);
            gtf.SetGroupHeader("MC", 2, ' ', false, true);
            gtf.SetGroupHeaderLine();

        }
        #endregion
    }
}