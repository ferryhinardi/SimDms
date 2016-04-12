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
    public class txtOmRpSalesTrn008:IRptProc  
    {
        private SimDms.Sales.Models.DataContext ctx = new SimDms.Sales.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public Models.SysUser CurrentUser { get; set; }
        private object[] setTextParameter, paramText;

        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            //var dt = ctx.Database.SqlQuery<OmRpSalesTrn002>(string.Format("exec {0} {1}", sproc, sparam));
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "exec " + sproc + " " + sparam;
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return CreateReportOmRpSalesTrn008(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalesTrn008(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
        {
            string reportID = recordId; 
            bool bCreateBy = false; 
            int counterData = 0;

            #region GenerateHeader
            SalesGenerateTextFileReport gtf = new SalesGenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            if (reportID == "OmRpSalesTrn009A" || reportID == "OmRpSalesTrn003A" || reportID == "OmRpSalesTrn003D" ||
                reportID == "OmRpSalesTrn003C" || reportID == "OmRpSalesTrn002A" || reportID == "OmRpSalesTrn001D" ||
                reportID == "OmRpSalesTrn001E" || reportID == "OmRpSalesTrn001F" || reportID == "OmRpSalesTrn001G" ||
                reportID == "OmRpPurTrn001A" || reportID == "OmRpPurTrn002A" || reportID == "OmRpPurTrn003A" ||
                reportID == "OmRpSalesTrn006" || reportID == "OmRpSalesTrn006A" || reportID == "OmRpPurTrn008A" ||
                reportID == "OmRpPurTrn009" || reportID == "OmRpSalesTrn005A" || reportID == "OmRpSalesTrn010A" ||
                reportID == "OmRpStock001") fullPage = false;
            gtf.GenerateTextFileReports(reportID, printerLoc, "W96", print, "", fullPage);
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
                //gtf.GenerateHeader();
            }
            #endregion

            #region Perlengkapan Out (OmRpSalesTrn008)
            if (reportID == "OmRpSalesTrn008")
            {
                string custName = dt.Rows[0]["CustomerName"].ToString();
                string add1 = dt.Rows[0]["Add1"].ToString();
                string add2 = dt.Rows[0]["Add2"].ToString();
                string kota = dt.Rows[0]["Kota"].ToString();
                string perlengkapanNo = "No. Perlengkapan : " + dt.Rows[0]["PerlengkapanNo"].ToString();
                string tgglPerlengkapan = "Tanggal : " + Convert.ToDateTime(dt.Rows[0]["TglPerlengkapan"]).ToString("dd-MMM-yyyy");
                string noReff = "No. Reff         : " + dt.Rows[0]["ReffNo"].ToString();

                gtf.SetGroupHeader(custName, 96, ' ', false, true);
                gtf.SetGroupHeader(add1, 96, ' ', false, true);
                gtf.SetGroupHeader(add2, 96, ' ', false, true);
                gtf.SetGroupHeader(kota, 96, ' ', false, true);
                gtf.SetGroupHeader("", 96, ' ', false, true);
                gtf.SetGroupHeader(perlengkapanNo, 74, ' ', true);
                gtf.SetGroupHeader(tgglPerlengkapan, 21, ' ', false, true);
                gtf.SetGroupHeader(noReff, 96, ' ', false, true);
                gtf.SetGroupHeader("", 96, ' ', false, true);

                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO", 2, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("MODEL", 36, ' ', true);
                gtf.SetGroupHeader("JML. UNIT", 9, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("NAMA PERLENGKAPAN", 37, ' ', true);
                gtf.SetGroupHeader("JUMLAH", 6, ' ', false, true, true);
                gtf.SetGroupHeaderLine();

                gtf.PrintHeader();

                string PerlengkapanNo = string.Empty;
                int loop = 0;
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        loop = 59 - (gtf.line + 7);
                        for (int j = 0; j < loop; j++)
                        {
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                        }

                        gtf.SetDataDetailSpace(8);
                        gtf.SetDataDetail("Yang Menerima", 13, ' ');
                        gtf.SetDataDetailSpace(25);
                        gtf.SetDataDetail("Menyetujui", 10, ' ');
                        gtf.SetDataDetailSpace(17);
                        gtf.SetDataDetail("Yang Menyerahkan", 16, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("-", 29, '-', true);
                        gtf.SetDataDetailSpace(6);
                        gtf.SetDataDetail("-", 29, '-', true);
                        gtf.SetDataDetail("-", 30, '-', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData();
                        break;
                    }

                    if (PerlengkapanNo == string.Empty)
                    {
                        PerlengkapanNo = dt.Rows[i]["PerlengkapanNo"].ToString();
                    }
                    else
                    {
                        if (PerlengkapanNo != dt.Rows[i]["PerlengkapanNo"].ToString())
                        {
                            loop = 59 - (gtf.line + 7);
                            for (int j = 0; j < loop; j++)
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                            }

                            gtf.SetDataDetailSpace(8);
                            gtf.SetDataDetail("Yang Menerima", 13, ' ');
                            gtf.SetDataDetailSpace(25);
                            gtf.SetDataDetail("Menyetujui", 10, ' ');
                            gtf.SetDataDetailSpace(17);
                            gtf.SetDataDetail("Yang Menyerahkan", 16, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("-", 29, '-', true);
                            gtf.SetDataDetailSpace(6);
                            gtf.SetDataDetail("-", 29, '-', true);
                            gtf.SetDataDetail("-", 30, '-', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();

                            counterData = 0;
                            PerlengkapanNo = dt.Rows[i]["PerlengkapanNo"].ToString();

                            gtf.ReplaceGroupHdr(custName, dt.Rows[i]["CustomerName"].ToString(), 96);
                            gtf.ReplaceGroupHdr(add1, dt.Rows[i]["Add1"].ToString(), 96);
                            gtf.ReplaceGroupHdr(add2, dt.Rows[i]["Add2"].ToString(), 96);
                            gtf.ReplaceGroupHdr(kota, dt.Rows[i]["Kota"].ToString(), 96);
                            gtf.ReplaceGroupHdr(perlengkapanNo, "No. Perlengkapan : " + dt.Rows[i]["PerlengkapanNo"].ToString(), 74);
                            gtf.ReplaceGroupHdr(tgglPerlengkapan, "Tanggal : " + Convert.ToDateTime(dt.Rows[i]["TglPerlengkapan"]).ToString("dd-MMM-yyyy"), 21);
                            gtf.ReplaceGroupHdr(noReff, "No. Reff         : " + dt.Rows[i]["ReffNo"].ToString(), 96);

                            custName = dt.Rows[i]["CustomerName"].ToString();
                            add1 = dt.Rows[i]["Add1"].ToString();
                            add2 = dt.Rows[i]["Add2"].ToString();
                            kota = dt.Rows[i]["Kota"].ToString();
                            perlengkapanNo = dt.Rows[i]["PerlengkapanNo"].ToString();
                            tgglPerlengkapan = Convert.ToDateTime(dt.Rows[i]["TglPerlengkapan"]).ToString("dd-MMM-yyyy");
                            noReff = dt.Rows[i]["ReffNo"].ToString();
                        }
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 2, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["Model"].ToString(), 36, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyUnit"].ToString(), 9, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["Perlengkapan"].ToString(), 37, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["QtyPerlengkapan"].ToString(), 6, ' ', false, true, true);

                    gtf.PrintData();
                }
            }
            #endregion
            return gtf.sbDataTxt.ToString();
        }
    }
}