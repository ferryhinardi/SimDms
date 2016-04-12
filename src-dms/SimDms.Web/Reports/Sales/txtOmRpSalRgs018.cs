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
    public class txtOmRpSalRgs018 : IRptProc
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
            return CreateReportOmRpSalRgs018(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalRgs018(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Outstanding Blanko Faktur Polisi (SUMMARY / STANDARD) (OmRpSalRgs018/OmRpSalRgs019)
            if (reportID == "OmRpSalRgs018" || reportID == "OmRpSalRgs019")
            {
                #region Print Group Header
                gtf.SetGroupHeader(textParam[0].ToString(), 96, ' ', false, true, false, true);
                string subDealer = string.Empty;
                decimal decSubTotBlanko = 0, decSubTotFaktur = 0, decSubTotPermohonan = 0;
                decimal decTotBlanko = 0, decTotFaktur = 0, decTotPermohonan = 0;
                int totalCounter = 0;

                if (reportID == "OmRpSalRgs018")
                {
                    gtf.SetGroupHeaderLine();
                    gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
                    gtf.SetGroupHeader("TIPE", 45, ' ', true);
                    gtf.SetGroupHeader("BLANKO", 8, ' ', true, false, true);
                    gtf.SetGroupHeader("FAKTUR", 8, ' ', true, false, true);
                    gtf.SetGroupHeader("PERMOHONAN", 12, ' ', true, false, true);
                    gtf.SetGroupHeader("KETERANGAN", 14, ' ', false, true);
                    gtf.SetGroupHeaderLine();

                }
                else
                {
                    gtf.SetGroupHeaderLine();
                    gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
                    gtf.SetGroupHeader("TGL. TERIMA", 11, ' ', true);
                    gtf.SetGroupHeader("NO FAKTUR", 9, ' ', true);
                    gtf.SetGroupHeader("SJ SUPP", 8, ' ', true);
                    gtf.SetGroupHeader("MODEL", 9, ' ', true);
                    gtf.SetGroupHeader("NO. RANGKA", 10, ' ', true);
                    gtf.SetGroupHeader("NO. MESIN", 8, ' ', true);
                    gtf.SetGroupHeader("WARNA", 15, ' ', true);
                    gtf.SetGroupHeader("KETERANGAN", 14, ' ', false, true);
                    gtf.SetGroupHeaderLine();
                }
                gtf.PrintHeader();
                #endregion

                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    #region Print Sub Total
                    if (i == dt.Rows.Count)
                    {
                        gtf.SetDataReportLine();

                        if (reportID == "OmRpSalRgs018")
                        {
                            gtf.SetDataDetail("SUB TOTAL :", 50, ' ', true, false, true);
                            gtf.SetDataDetail(decSubTotBlanko.ToString(), 8, ' ', true, false, true);
                            gtf.SetDataDetail(decSubTotFaktur.ToString(), 8, ' ', true, false, true);
                            gtf.SetDataDetail(decSubTotPermohonan.ToString(), 12, ' ', false, true, true);

                            decTotBlanko += decSubTotBlanko;
                            decTotFaktur += decSubTotFaktur;
                            decTotPermohonan += decSubTotPermohonan;
                        }
                        else
                        {
                            gtf.SetDataDetail("SUB TOTAL :", 16, ' ', true, false, true);
                            gtf.SetDataDetail(counterData.ToString(), 79, ' ', false, true);

                            totalCounter += counterData;
                        }

                        gtf.SetDataDetailLineBreak();
                        gtf.PrintData(false);

                        break;
                    }

                    if (subDealer == string.Empty)
                    {
                        if (reportID == "OmRpSalRgs018")
                        {
                            subDealer = dt.Rows[i]["SubDealerCode"].ToString() + " " + dt.Rows[i]["DealerName"].ToString();
                        }
                        else
                        {
                            subDealer = dt.Rows[i]["CompanyCode"].ToString() + " " + dt.Rows[i]["DealerName"].ToString();
                        }
                        gtf.SetDataDetail(subDealer, 96, ' ', false, true);
                    }
                    else
                    {
                        if (reportID == "OmRpSalRgs018")
                        {
                            if (subDealer != dt.Rows[i]["SubDealerCode"].ToString() + " " + dt.Rows[i]["DealerName"].ToString())
                            {
                                gtf.SetDataReportLine();
                                gtf.SetDataDetail("SUB TOTAL :", 50, ' ', true, false, true);
                                gtf.SetDataDetail(decSubTotBlanko.ToString(), 8, ' ', true, false, true);
                                gtf.SetDataDetail(decSubTotFaktur.ToString(), 8, ' ', true, false, true);
                                gtf.SetDataDetail(decSubTotPermohonan.ToString(), 12, ' ', false, true, true);

                                gtf.SetDataDetailLineBreak();

                                decTotBlanko += decSubTotBlanko;
                                decTotFaktur += decSubTotFaktur;
                                decTotPermohonan += decSubTotPermohonan;

                                subDealer = dt.Rows[i]["SubDealerCode"].ToString() + " " + dt.Rows[i]["DealerName"].ToString();

                                gtf.SetDataDetail(subDealer, 96, ' ', false, true);
                                counterData = 0;
                                decSubTotBlanko = decSubTotFaktur = decSubTotPermohonan = 0;

                                gtf.PrintData(false);
                            }
                        }
                        else
                        {
                            if (subDealer != dt.Rows[i]["CompanyCode"].ToString() + " " + dt.Rows[i]["DealerName"].ToString())
                            {
                                gtf.SetDataReportLine();
                                gtf.SetDataDetail("SUB TOTAL :", 16, ' ', true, false, true);
                                gtf.SetDataDetail(counterData.ToString(), 79, ' ', false, true);
                                gtf.SetDataDetailLineBreak();

                                totalCounter += counterData;

                                subDealer = dt.Rows[i]["CompanyCode"].ToString() + " " + dt.Rows[i]["DealerName"].ToString();

                                gtf.SetDataDetail(subDealer, 96, ' ', false, true);
                                counterData = 0;

                                gtf.PrintData(false);
                            }
                        }
                    }
                    #endregion

                    #region Print Data Detail
                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);

                    if (reportID == "OmRpSalRgs018")
                    {
                        gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 45, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Blanko"].ToString(), 8, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["Faktur"].ToString(), 8, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["Permohonan"].ToString(), 12, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(), 14, ' ', false, true);

                        decSubTotBlanko += Convert.ToDecimal(dt.Rows[i]["Blanko"]);
                        decSubTotFaktur += Convert.ToDecimal(dt.Rows[i]["Faktur"]);
                        decSubTotPermohonan += Convert.ToDecimal(dt.Rows[i]["Permohonan"]);

                        gtf.PrintData(false);
                    }
                    else
                    {
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["CreatedDate"]).ToString("dd-MMM-yyyy"), 11, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["FakturPolisiNo"].ToString(), 9, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SJImniNo"].ToString(), 8, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 9, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 10, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 8, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["ColourName"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["Remark"].ToString(), 14, ' ', false, true);

                        gtf.PrintData(true);
                    }
                    #endregion
                }

                #region Print Footer
                gtf.SetTotalDetailLine();
                if (reportID == "OmRpSalRgs018")
                {
                    gtf.SetTotalDetail("GRAND TOTAL :", 50, ' ', true, false, true);
                    gtf.SetTotalDetail(decTotBlanko.ToString(), 8, ' ', true, false, true);
                    gtf.SetTotalDetail(decTotFaktur.ToString(), 8, ' ', true, false, true);
                    gtf.SetTotalDetail(decTotPermohonan.ToString(), 12, ' ', false, true, true);
                }
                else
                {
                    gtf.SetTotalDetail("GRAND TOTAL :", 16, ' ', true, false, true);
                    gtf.SetTotalDetail(totalCounter.ToString(), 79, ' ', false, true);
                }
                gtf.SetTotalDetailLine();
                #endregion
            }
            #endregion

            return gtf.sbDataTxt.ToString();
        }
    }
}