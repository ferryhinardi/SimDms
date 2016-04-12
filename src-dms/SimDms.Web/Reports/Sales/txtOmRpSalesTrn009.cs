using SimDms.Common;
using SimDms.Sales.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace SimDms.Web.Reports.Sales
{ 
    public class txtOmRpSalesTrn009:IRptProc 
    {
        private SimDms.Sales.Models.DataContext ctx = new SimDms.Sales.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public Models.SysUser CurrentUser { get; set; }
        

        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            //var dt = ctx.Database.SqlQuery<OmRpSalesTrn002>(string.Format("exec {0} {1}", sproc, sparam));
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "exec " + sproc + " " + sparam;
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return CreateReportOmRpSalesTrn009(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalesTrn009(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
        {
            string reportID = recordId; 
            bool bCreateBy = false;
            int counterData = 0;

            #region GenerateHeader
            SalesGenerateTextFileReport gtf = new SalesGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            if (reportID == "OmRpSalesTrn009A" || reportID == "OmRpSalesTrn003A" || reportID == "OmRpSalesTrn003D" ||
                reportID == "OmRpSalesTrn003C" || reportID == "OmRpSalesTrn002A" || reportID == "OmRpSalesTrn001D" ||
                reportID == "OmRpSalesTrn001E" || reportID == "OmRpSalesTrn001F" || reportID == "OmRpSalesTrn001G" ||
                reportID == "OmRpPurTrn001A" || reportID == "OmRpPurTrn002A" || reportID == "OmRpPurTrn003A" ||
                reportID == "OmRpSalesTrn006" || reportID == "OmRpSalesTrn006A" || reportID == "OmRpPurTrn008A" ||
                reportID == "OmRpPurTrn009" || reportID == "OmRpSalesTrn005A" || reportID == "OmRpSalesTrn010A" ||
                reportID == "OmRpStock001") fullPage = false;
            gtf.GenerateTextFileReports(reportID, printerLoc, "W136", print, "", fullPage);
            if (reportID == "OmRpSalesTrn004" || reportID == "OmRpSalesTrn004A" || reportID == "OmRpSalesTrn003A" ||
                reportID == "OmRpSalesTrn007DNew" || reportID == "OmRpSalesTrn007" || reportID == "OmRpSalesTrn007C" ||
                reportID == "OmRpSalesTrn007A" || reportID == "OmRpSalesTrn010" || reportID == "OmRpSalesTrn010A" ||
                reportID == "OmRpStock002")
            {
                gtf.GenerateHeader2(false);
            }
            else if (reportID == "OmRpSalesTrn003D" || reportID == "OmRpSalesTrn001D" ||
                reportID == "OmRpSalesTrn001E" || reportID == "OmRpSalesTrn006" || reportID == "OmRpSalesTrn006A" ||
                reportID == "OmRpPurTrn009" || reportID == "OmRpSalesTrn005" || reportID == "OmRpSalesTrn005A" || reportID == "OmRpStock001")
            {
                gtf.GenerateHeader2(false, false);
            }
            else
            {
                gtf.GenerateHeader();
            }
            #endregion

            #region Nota Debet(OmRpSalesTrn009/OmRpSalesTrn009A)
            if (reportID == "OmRpSalesTrn009" || reportID == "OmRpSalesTrn009A")
            {
                bCreateBy = false;

                string customerCode = string.Empty, DNNo = string.Empty, customerName = string.Empty, invoiceNo = string.Empty, address1 = string.Empty,
                soNo = string.Empty, address2 = string.Empty, bpkNo = string.Empty, skpkNo = string.Empty, npwpNo = string.Empty,
                refferenceNo = string.Empty;

                customerCode = "Kepada Yth. : [" + dt.Rows[0]["CustomerCode"].ToString() + "]";
                DNNo = dt.Rows[0]["DNNo"].ToString();
                customerName = dt.Rows[0]["CustomerName"].ToString();
                invoiceNo = dt.Rows[0]["InvoiceNo"].ToString();
                address1 = dt.Rows[0]["Address1"].ToString();
                soNo = dt.Rows[0]["SONo"].ToString();
                address2 = dt.Rows[0]["Address2"].ToString();
                bpkNo = dt.Rows[0]["BPKNo"].ToString();
                skpkNo = dt.Rows[0]["SKPKNo"].ToString();
                npwpNo = dt.Rows[0]["NPWPNo"].ToString();
                refferenceNo = dt.Rows[0]["RefferenceNo"].ToString();

                gtf.SetGroupHeader(customerCode, 102, ' ', true);
                gtf.SetGroupHeader("No. DN     : " + DNNo, 38, ' ', false, true);
                gtf.SetGroupHeader(customerName, 102, ' ', true);
                gtf.SetGroupHeader("No. Faktur : " + invoiceNo, 38, ' ', false, true);
                gtf.SetGroupHeader(address1, 102, ' ', true);
                gtf.SetGroupHeader("No. SO     : " + soNo, 38, ' ', false, true);
                gtf.SetGroupHeader(address2, 98, ' ', false, true);
                gtf.SetGroupHeader("", 102, ' ', true);
                gtf.SetGroupHeader("No. SKPK   : " + skpkNo, 38, ' ', false, true);
                gtf.SetGroupHeader("NPWP : " + npwpNo, 102, ' ', true);
                gtf.SetGroupHeader("No. Reff   : " + refferenceNo, 38, ' ', false, true);
                gtf.SetGroupHeader("", 96, ' ', false, true);
                gtf.SetGroupHeader("Hari ini, kami telah mendebet rekening piutang saudara dengan perincian sbb : ", 96, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeader("KETERANGAN", 53, ' ', true);
                gtf.SetGroupHeader("No. PERKIRAAN", 62, ' ', true);
                gtf.SetGroupHeader("JUMLAH", 15, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                DataRow[] drSub;
                decimal decTotRupiah = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    counterData = 0;
                    decTotRupiah = 0;

                    drSub = dt.Select(string.Format("InvoiceNo = '{0}'", dt.Rows[i]["InvoiceNo"].ToString()));

                    for (int j = 0; j < drSub.Length; j++)
                    {
                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["Chassis"].ToString(), 53, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["AccNo"].ToString(), 52, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Total"].ToString(), 25, ' ', false, true, true, true, "n0");

                        decTotRupiah += Convert.ToDecimal(dt.Rows[i]["Total"]);

                        gtf.PrintData();
                    }

                    int loop = 0;
                    if (reportID == "OmRpSalesTrn009")
                        loop = 29 - counterData;
                    else
                        loop = 1;

                    for (int k = 0; k < loop; k++)
                    {
                        gtf.SetDataDetail("", 96, ' ', false, true);
                    }

                    gtf.SetDataReportLine();
                    gtf.SetDataDetailSpace(58);
                    gtf.SetDataDetail("T O T A L    : ", 52, ' ', true);
                    gtf.SetDataDetail(decTotRupiah.ToString(), 25, ' ', false, true, true, true, "n0");
                    gtf.SetDataReportLine();
                    gtf.SetDataDetail("TERBILANG : ", 103, ' ', true);
                    gtf.SetDataDetail(dt.Rows[0]["CityName"].ToString().ToUpper() + ", " + Convert.ToDateTime(dt.Rows[0]["DNDate"]).ToString("dd-MMM-yyyy").ToUpper(), 29, ' ', false, true);
                    gtf.SetDataDetail(gtf.Terbilang(Convert.ToInt64(decTotRupiah)).ToUpper() + " RUPIAH", 80, ' ', false, true);

                    if (reportID == "OmRpSalesTrn009")
                        loop = 4;
                    else
                        loop = 2;

                    for (int k = 0; k < loop; k++)
                    {
                        gtf.SetDataDetail("", 96, ' ', false, true);
                    }

                    gtf.SetDataDetailSpace(104);
                    gtf.SetDataDetail(PadBoth(dt.Rows[0]["SignName"].ToString(),29), 29, ' ', false, true);
                    gtf.SetDataDetailSpace(104);
                    gtf.SetDataDetail("-", 29, '-', false, true);
                    gtf.SetDataDetailSpace(104);
                    gtf.SetDataDetail(PadBoth(dt.Rows[0]["TitleSign"].ToString(),29), 29, ' ', false, true);
                    gtf.PrintData();
                    gtf.SetDataDetail("", 96, ' ', false, true);
                    gtf.PrintData();

                    if (i == dt.Rows.Count - 1) break;
                    gtf.ReplaceGroupHdr(customerCode, "Kepada Yth. : [" + dt.Rows[i]["CustomerCode"].ToString() + "]", 57);
                    gtf.ReplaceGroupHdr(customerName, dt.Rows[i + 1]["CustomerName"].ToString(), 57);
                    gtf.ReplaceGroupHdr("No. DN     : " + DNNo, "No. DN     : " + dt.Rows[i + 1]["DNNo"].ToString(), 22);
                    gtf.ReplaceGroupHdr(address1, dt.Rows[i + 1]["Address1"].ToString(), 57);
                    gtf.ReplaceGroupHdr("No. SO     : " + soNo, "No. SO     : " + dt.Rows[i + 1]["SONo"].ToString(), 22);
                    gtf.ReplaceGroupHdr(address2, dt.Rows[i + 1]["Address2"].ToString(), 57);
                    gtf.ReplaceGroupHdr("No. SJ     : " + bpkNo, "No. SJ     : " + dt.Rows[i + 1]["BPKNo"].ToString(), 22);
                    gtf.ReplaceGroupHdr("No. SKPK : " + skpkNo, "No. SKPK : " + dt.Rows[i + 1]["SKPKNo"].ToString(), 22);
                    gtf.ReplaceGroupHdr("NPWP : " + npwpNo, "NPWP : " + dt.Rows[i + 1]["NPWPNo"].ToString(), 57);
                    gtf.ReplaceGroupHdr("No. Reff   : " + refferenceNo, "No. Reff : " + dt.Rows[i + 1]["RefferenceNo"].ToString(), 22);

                    customerCode = "Kepada Yth. : [" + dt.Rows[i + 1]["CustomerCode"].ToString() + "]";
                    customerName = dt.Rows[i + 1]["CustomerName"].ToString();
                    DNNo = dt.Rows[i + 1]["DNNo"].ToString();
                    address1 = dt.Rows[i + 1]["Address1"].ToString();
                    soNo = dt.Rows[i + 1]["SONo"].ToString();
                    address2 = dt.Rows[i + 1]["Address2"].ToString();
                    bpkNo = dt.Rows[i + 1]["BPKNo"].ToString();
                    skpkNo = dt.Rows[i + 1]["SKPKNo"].ToString();
                    npwpNo = dt.Rows[i + 1]["NPWPNo"].ToString();
                    refferenceNo = dt.Rows[i + 1]["RefferenceNo"].ToString();
                }
            }
            #endregion

            return gtf.sbDataTxt.ToString();
        }

        public string PadBoth(string source, int length)
        {
            int spaces = length - source.Length;
            int padLeft = spaces / 2 + source.Length;
            return source.PadLeft(padLeft).PadRight(length);

        }
    }
}