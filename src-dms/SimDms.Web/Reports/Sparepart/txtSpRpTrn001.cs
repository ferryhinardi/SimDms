using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn001 : IRptProc
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
            //DataTable dt = new DataTable();
            DataSet dt = new DataSet();
            da.Fill(dt);

            SetTextParameter(oparam);
            return CreateReportSpRpTrn001(rptId, dt, sparam, printerloc, print, "");
        }

        private string CreateReportSpRpTrn001(string recordId, DataSet dt, string paramReport, string printerLoc, bool print, string fileLocation)
        {
            SpGenerateTextFileReport gtf = new SpGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W163", print, fileLocation);
            gtf.GenerateHeader();
            gtf.SetGroupHeader("NO. DRAFT SUGGOR", 18, ' ');
            gtf.SetGroupHeader(": " + dt.Tables[2].Rows[0]["SuggorNo"].ToString(), 15, ' ', false, true);
            gtf.SetGroupHeader("TGL. DRAFT SUGGOR", 18, ' ');
            gtf.SetGroupHeader(": " + dt.Tables[2].Rows[0]["SuggorDate"].ToString(), 13, ' ', false, true);
            gtf.SetGroupHeader("PEMASOK : ", 18, ' ');
            gtf.SetGroupHeader(": " + dt.Tables[2].Rows[0]["SupplierName"].ToString(), 50, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeaderSpace(47);
            gtf.SetGroupHeader("QUANTITY", 82, ' ', false, true, false, true);
            gtf.SetGroupHeaderSpace(47);
            gtf.SetGroupHeader("-", 82, '-', false, true);
            gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NO. PART", 20, ' ', true);
            gtf.SetGroupHeader("NAMA PART", 17, ' ', true);
            gtf.SetGroupHeader("MC", 2, ' ', true);
            gtf.SetGroupHeader("OH", 10, ' ', true, false, true);
            gtf.SetGroupHeader("OO", 10, ' ', true, false, true);
            gtf.SetGroupHeader("INT", 10, ' ', true, false, true);
            gtf.SetGroupHeader("BO", 9, ' ', true, false, true);
            gtf.SetGroupHeader("OP", 8, ' ', true, false, true);
            gtf.SetGroupHeader("SS", 8, ' ', true, false, true);
            gtf.SetGroupHeader("SUGGOR", 10, ' ', true, false, true);
            gtf.SetGroupHeader("ORDER", 10, ' ', true, false, true);
            gtf.SetGroupHeader("HARGA BELI", 11, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 14, ' ', false, true, true);
            gtf.SetGroupHeaderLine();

            string orgPartNo = "", suggorNo = "";
            int noUrut = 0;
            decimal decTotal = 0, decQtyOrder = 0;

            for (int i = 0; i < dt.Tables[2].Rows.Count; i++)
            {
                bool statusSubs = false;
                if (suggorNo == "")
                {
                    suggorNo = dt.Tables[2].Rows[i]["SuggorNo"].ToString();

                    string cellHdrII = Convert.ToDateTime(dt.Tables[2].Rows[i]["SuggorDate"]).AddMonths(-6).ToString("MM/yy");
                    string cellHdrIII = Convert.ToDateTime(dt.Tables[2].Rows[i]["SuggorDate"]).AddMonths(-5).ToString("MM/yy");
                    string cellHdrIV = Convert.ToDateTime(dt.Tables[2].Rows[i]["SuggorDate"]).AddMonths(-4).ToString("MM/yy");
                    string cellHdrV = Convert.ToDateTime(dt.Tables[2].Rows[i]["SuggorDate"]).AddMonths(-3).ToString("MM/yy");
                    string cellHdrVI = Convert.ToDateTime(dt.Tables[2].Rows[i]["SuggorDate"]).AddMonths(-2).ToString("MM/yy");
                    string cellHdrVII = Convert.ToDateTime(dt.Tables[2].Rows[i]["SuggorDate"]).AddMonths(-1).ToString("MM/yy");

                    gtf.SetGroupHeaderSpace(80);
                    gtf.SetGroupHeader(cellHdrII, 9, ' ', true, false, true);
                    gtf.SetGroupHeader(cellHdrIII, 8, ' ', true, false, true);
                    gtf.SetGroupHeader(cellHdrIV, 8, ' ', true, false, true);
                    gtf.SetGroupHeader(cellHdrV, 10, ' ', true, false, true);
                    gtf.SetGroupHeader(cellHdrVI, 10, ' ', true, false, true);
                    gtf.SetGroupHeader(cellHdrVII, 11, ' ', true, false, true);
                    gtf.SetGroupHeader("TOTAL", 14, ' ', true, false, true);
                    gtf.SetGroupHeader("AVR", 6, ' ', false, true, true);
                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();
                }
                else if (suggorNo != dt.Tables[2].Rows[i]["SuggorNo"].ToString())
                {
                    suggorNo = dt.Tables[2].Rows[i]["SuggorNo"].ToString();

                    string cellHdrII = Convert.ToDateTime(dt.Tables[2].Rows[i]["SuggorDate"]).AddMonths(-6).ToString("MM/yy");
                    string cellHdrIII = Convert.ToDateTime(dt.Tables[2].Rows[i]["SuggorDate"]).AddMonths(-5).ToString("MM/yy");
                    string cellHdrIV = Convert.ToDateTime(dt.Tables[2].Rows[i]["SuggorDate"]).AddMonths(-4).ToString("MM/yy");
                    string cellHdrV = Convert.ToDateTime(dt.Tables[2].Rows[i]["SuggorDate"]).AddMonths(-3).ToString("MM/yy");
                    string cellHdrVI = Convert.ToDateTime(dt.Tables[2].Rows[i]["SuggorDate"]).AddMonths(-2).ToString("MM/yy");
                    string cellHdrVII = Convert.ToDateTime(dt.Tables[2].Rows[i]["SuggorDate"]).AddMonths(-1).ToString("MM/yy");

                    gtf.ReplaceGroupHdr(dt.Tables[2].Rows[i - 1]["SuggorNo"].ToString(), dt.Tables[2].Rows[i]["SuggorNo"].ToString());
                    gtf.ReplaceGroupHdr(dt.Tables[2].Rows[i - 1]["SuggorDate"].ToString(), dt.Tables[2].Rows[i]["SuggorDate"].ToString());
                    gtf.ReplaceGroupHdr(dt.Tables[2].Rows[i - 1]["SupplierName"].ToString(), dt.Tables[2].Rows[i]["SupplierName"].ToString());

                    gtf.SetGroupHeaderSpace(80);
                    gtf.SetGroupHeader(cellHdrII, 9, ' ', true, false, true);
                    gtf.SetGroupHeader(cellHdrIII, 8, ' ', true, false, true);
                    gtf.SetGroupHeader(cellHdrIV, 8, ' ', true, false, true);
                    gtf.SetGroupHeader(cellHdrV, 10, ' ', true, false, true);
                    gtf.SetGroupHeader(cellHdrVI, 10, ' ', true, false, true);
                    gtf.SetGroupHeader(cellHdrVII, 11, ' ', true, false, true);
                    gtf.SetGroupHeader("TOTAL", 14, ' ', true, false, true);
                    gtf.SetGroupHeader("AVR", 6, ' ', false, true, true);
                    gtf.SetGroupHeaderLine();
                    gtf.PrintHeader();
                }

                if (orgPartNo != dt.Tables[2].Rows[i]["PartNoOriginal"].ToString())
                {
                    statusSubs = false;
                    orgPartNo = dt.Tables[2].Rows[i]["PartNoOriginal"].ToString();
                    noUrut++;

                    decimal totSuggor = Convert.ToDecimal(dt.Tables[2].Rows[i]["Suggor"].ToString()) * Convert.ToDecimal(dt.Tables[2].Rows[i]["PurchasePrice"].ToString());

                    decTotal += totSuggor;
                    decQtyOrder += Convert.ToDecimal(dt.Tables[2].Rows[i]["Suggor"].ToString());


                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["PartNoOriginal"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["PartName"].ToString(), 17, ' ', true);
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["MovingCode"].ToString(), 2, ' ', true);
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["OnHand"].ToString(), 10, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["OnOrder"].ToString(), 10, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["InTransit"].ToString(), 10, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["BackOrder"].ToString(), 9, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["OrderPoint"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["SafetyStock"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["Suggor"].ToString(), 10, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["uOrder"].ToString(), 10, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["PurchasePrice"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(totSuggor.ToString(), 14, ' ', false, true, true, true, "n0");
                    gtf.SetDataDetailSpace(80);
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["I"].ToString(), 9, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["II"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["III"].ToString(), 8, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["IV"].ToString(), 10, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["V"].ToString(), 10, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["VI"].ToString(), 11, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["TotalDemand"].ToString(), 14, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Tables[2].Rows[i]["DemandAvg"].ToString(), 6, ' ', false, true, true, true, "n2");
                    gtf.PrintData(false);
                }

                DataRow[] rowDetail = dt.Tables[1].Select(string.Format("PartNoOriginal = '{0}' and PartNo <> '{1}'", dt.Tables[2].Rows[i]["PartNoOriginal"].ToString(), dt.Tables[2].Rows[i]["PartNoOriginal"].ToString()));
                if (rowDetail.Length != 0)
                {
                    int count = 0;
                    gtf.SetDataDetail("SUBS:", 5, ' ');

                    foreach (DataRow row in rowDetail)
                    {
                        if (count != 0)
                            gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail(row["PartNo"].ToString(), 20, ' ', true);
                        gtf.SetDataDetailSpace(54);
                        gtf.SetDataDetail(row["I"].ToString(), 9, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(row["II"].ToString(), 8, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(row["III"].ToString(), 8, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(row["IV"].ToString(), 10, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(row["V"].ToString(), 10, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(row["VI"].ToString(), 11, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(row["TotalDemand"].ToString(), 14, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(row["DemandAvg"].ToString(), 6, ' ', false, true, true, true, "n0");

                        gtf.PrintData(false);
                        count++;
                    }
                }
                gtf.SetDataDetail("-", 163, '-', false, true);
                gtf.PrintData(false);
            }

            gtf.SetTotalDetailLine();
            gtf.SetTotalDetailSpace(109);
            gtf.SetTotalDetail(decQtyOrder.ToString(), 20, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetail(decTotal.ToString(), 26, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                if (gtf.PrintTotal() == true)
                    msg="Save Berhasil";
                else
                    msg = "Save Gagal";

            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }
    }
}