using SimDms.Common;
using SimDms.Sales.Models.Reports;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace SimDms.Web.Reports.Sales
{ 
    public class txtOmRpSalesTrn010:IRptProc  
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
            return CreateReportOmRpSalesTrn010(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportOmRpSalesTrn010(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
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

            #region Tanda Terima BPKB (OmRpSalesTrn010/OmRpSalesTrn010A)
            if (reportID == "OmRpSalesTrn010" || reportID == "OmRpSalesTrn010A")
            {
                bCreateBy = false;

                string docNo = "No : " + dt.Rows[0]["DocNo"].ToString();
                string BPKBOut = dt.Rows[0]["BPKBOut"].ToString();

                gtf.SetGroupHeader(docNo, 96, ' ', false, true, true);
                gtf.SetGroupHeader("TANDA TERIMA BPKB", 96, ' ', false, true, false, true);
                gtf.SetGroupHeaderSpace(33);
                gtf.SetGroupHeader("-", 27, '-', false, true);
                gtf.SetGroupHeader(BPKBOut, 96, ' ', false, true, false, true);
                gtf.SetGroupHeader("", 96, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO", 2, ' ', true, false, true);
                gtf.SetGroupHeader("NAMA", 11, ' ', true);
                gtf.SetGroupHeader("NO FPS", 13, ' ', true);
                gtf.SetGroupHeader("TGL FPS", 15, ' ', true);
                gtf.SetGroupHeader("JENIS", 8, ' ', true);
                gtf.SetGroupHeader("KODE RANGKA", 11, ' ', true);
                gtf.SetGroupHeader("KODE MESIN", 10, ' ', true);
                gtf.SetGroupHeader("NO.BPKB", 9, ' ', true);
                gtf.SetGroupHeader("NO.POLISI", 9, ' ', false, true);

                gtf.SetGroupHeaderSpace(3);
                gtf.SetGroupHeader("ALAMAT", 41, ' ', true);
                gtf.SetGroupHeader("WARNA", 8, ' ', true);
                gtf.SetGroupHeader("NO.RANGKA", 11, ' ', true);
                gtf.SetGroupHeader("NO.MESIN", 11, ' ', false, true);

                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                ArrayList address = new ArrayList(), custName = new ArrayList();
                string tempCustName = string.Empty, tempAddr = string.Empty, DocNo = string.Empty;
                string[] tempSplitCustName = new string[] { }, tempSplitAddr = new string[] { };
                int tempLength = 0, countLine = 0, loop = 0, tempSpace = 0;

                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        if (reportID == "OmRpSalesTrn010")
                            loop = 59 - gtf.line - 12;
                        else
                            loop = 30 - gtf.line - 12;

                        for (int j = 0; j < loop; j++)
                        {
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                        }

                        gtf.SetDataReportLine();
                        gtf.PrintData(false, false);

                        gtf.SetDataDetail(dt.Rows[i - 1]["City"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["DocDate"]).ToString("dd MMMM yyyy"), 96, ' ', false, true, true);
                        gtf.PrintData(false, false);

                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);

                        gtf.SetDataDetailSpace(5);
                        gtf.SetDataDetail("Mengetahui,", 11, ' ');
                        gtf.SetDataDetailSpace(12);
                        gtf.SetDataDetail("Mengetahui,", 11, ' ');
                        gtf.SetDataDetailSpace(9);
                        gtf.SetDataDetail("Penerima di Cabang", 18, ' ');
                        gtf.SetDataDetailSpace(6);
                        gtf.SetDataDetail("Yang Menyerahkan,", 17, ' ', false, true);
                        gtf.PrintData(false, false);

                        gtf.SetDataDetailSpace(28);
                        gtf.SetDataDetail("(CCD Pusat)", 11, ' ', true);
                        gtf.SetDataDetailSpace(33);
                        gtf.SetDataDetail("(BAG BPKB Pusat)", 16, ' ', false, true);
                        gtf.PrintData(false, false);

                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);

                        tempSpace = (24 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ');
                        gtf.SetDataDetailSpace(tempSpace);
                        tempSpace = (24 - dt.Rows[i - 1]["SignName2"].ToString().Length) / 2;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["SignName2"].ToString(), dt.Rows[i - 1]["SignName2"].ToString().Length, ' ');
                        gtf.SetDataDetailSpace(tempSpace);
                        tempSpace = (24 - dt.Rows[i - 1]["SignName3"].ToString().Length) / 2;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["SignName3"].ToString(), dt.Rows[i - 1]["SignName3"].ToString().Length, ' ');
                        gtf.SetDataDetailSpace(tempSpace);
                        tempSpace = (24 - dt.Rows[i - 1]["SignName4"].ToString().Length) / 2;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["SignName4"].ToString(), dt.Rows[i - 1]["SignName4"].ToString().Length, ' ', false, true);
                        gtf.PrintData(false, false);

                        gtf.SetDataDetail("-", 22, '-', true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail("-", 22, '-', true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail("-", 22, '-', true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail("-", 22, '-', false, true);
                        gtf.PrintData(false, false);

                        tempSpace = (24 - dt.Rows[i - 1]["TitleSign1"].ToString().Length) / 2;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), dt.Rows[i - 1]["TitleSign1"].ToString().Length, ' ');
                        gtf.SetDataDetailSpace(tempSpace);
                        tempSpace = (24 - dt.Rows[i - 1]["TitleSign2"].ToString().Length) / 2;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign2"].ToString(), dt.Rows[i - 1]["TitleSign2"].ToString().Length, ' ');
                        gtf.SetDataDetailSpace(tempSpace);
                        tempSpace = (24 - dt.Rows[i - 1]["TitleSign3"].ToString().Length) / 2;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign3"].ToString(), dt.Rows[i - 1]["TitleSign3"].ToString().Length, ' ');
                        gtf.SetDataDetailSpace(tempSpace);
                        tempSpace = (24 - dt.Rows[i - 1]["TitleSign4"].ToString().Length) / 2;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign4"].ToString(), dt.Rows[i - 1]["TitleSign4"].ToString().Length, ' ', false, true);
                        gtf.PrintData(false, false);

                        gtf.SetDataDetail("", 96, ' ', false, true);
                        gtf.PrintData(false, false);
                        break;
                    }

                    if (DocNo == string.Empty)
                    {
                        DocNo = dt.Rows[i]["DocNo"].ToString();
                    }
                    else
                    {
                        if (DocNo != dt.Rows[i]["DocNo"].ToString())
                        {
                            if (reportID == "OmRpSalesTrn010")
                                loop = 59 - gtf.line - 12;
                            else
                                loop = 30 - gtf.line - 12;

                            for (int j = 0; j < loop; j++)
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                            }

                            gtf.SetDataReportLine();
                            gtf.PrintData(false, false);

                            gtf.SetDataDetail(dt.Rows[i - 1]["City"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["DocDate"]).ToString("dd MMMM yyyy"), 96, ' ', false, true);
                            gtf.PrintData(false, false);

                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);

                            gtf.SetDataDetailSpace(7);
                            gtf.SetDataDetail("Mengetahui,", 11, ' ');
                            gtf.SetDataDetailSpace(13);
                            gtf.SetDataDetail("Mengetahui,", 11, ' ');
                            gtf.SetDataDetailSpace(16);
                            gtf.SetDataDetail("Penerima di Cabang", 18, ' ');
                            gtf.SetDataDetailSpace(7);
                            gtf.SetDataDetail("Yang Menyerahkan,", 17, ' ', false, true);
                            gtf.PrintData(false, false);

                            gtf.SetDataDetailSpace(80);
                            gtf.SetDataDetail("(BAG BPKB Pusat)", 16, ' ', false, true);
                            gtf.PrintData(false, false);

                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);

                            tempSpace = (24 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace);
                            tempSpace = (24 - dt.Rows[i - 1]["SignName2"].ToString().Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SignName2"].ToString(), dt.Rows[i - 1]["SignName2"].ToString().Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace);
                            tempSpace = (24 - dt.Rows[i - 1]["SignName3"].ToString().Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SignName3"].ToString(), dt.Rows[i - 1]["SignName3"].ToString().Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace);
                            tempSpace = (24 - dt.Rows[i - 1]["SignName4"].ToString().Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SignName4"].ToString(), dt.Rows[i - 1]["SignName4"].ToString().Length, ' ', false, true);
                            gtf.PrintData(false, false);

                            gtf.SetDataDetail("-", 23, '-', true);
                            gtf.SetDataDetail("-", 23, '-', true);
                            gtf.SetDataDetail("-", 23, '-', true);
                            gtf.SetDataDetail("-", 24, '-', false, true);
                            gtf.PrintData(false, false);

                            tempSpace = (24 - dt.Rows[i - 1]["TitleSign1"].ToString().Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), dt.Rows[i - 1]["TitleSign1"].ToString().Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace);
                            tempSpace = (24 - dt.Rows[i - 1]["TitleSign2"].ToString().Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign2"].ToString(), dt.Rows[i - 1]["TitleSign2"].ToString().Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace);
                            tempSpace = (24 - dt.Rows[i - 1]["TitleSign3"].ToString().Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign3"].ToString(), dt.Rows[i - 1]["TitleSign3"].ToString().Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace);
                            tempSpace = (24 - dt.Rows[i - 1]["TitleSign4"].ToString().Length) / 2;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign4"].ToString(), dt.Rows[i - 1]["TitleSign4"].ToString().Length, ' ', false, true);
                            gtf.PrintData(false, false);

                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData(false, false);

                            DocNo = dt.Rows[i]["DocNo"].ToString();
                            counterData = 0;
                        }
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 2, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FakturPolisiNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["FakturPolisiDate"]).ToString("dd-MMM-yyyy"), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SalesModelCode"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisCode"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineCode"].ToString(), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BPKBNo"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PoliceRegistrationNo"].ToString(), 9, ' ', false, true);
                    gtf.PrintData(false, false);

                    gtf.SetDataDetailSpace(3);
                    if (dt.Rows[i]["Address"].ToString().Length > 41)
                    {
                        tempAddr = dt.Rows[i]["Address"].ToString();
                        countLine = tempAddr.Length / 41;

                        for (int j = 0; j < countLine + 1; j++)
                        {
                            tempSplitAddr = tempAddr.Split(' ');
                            tempLength = 0;

                            for (int k = 0; k < tempSplitAddr.Length; k++)
                            {
                                if (tempSplitAddr[k] != string.Empty || tempSplitAddr[k] != " ")
                                {
                                    tempLength += tempSplitAddr[k].Length + 1;
                                    if (k == tempSplitAddr.Length - 1) break;
                                    if (tempLength + tempSplitAddr[k + 1].Length + 1 > 41)
                                    {
                                        if (tempSplitAddr[k + 1][tempSplitAddr[k + 1].Length - 1].ToString() != string.Empty || tempSplitAddr[k + 1][tempSplitAddr[k + 1].Length - 1].ToString() != " ")
                                            break;
                                    }
                                }
                            }

                            if (j == countLine)
                                address.Add(tempAddr);
                            else
                            {
                                address.Add(tempAddr.Substring(0, tempLength));
                                tempAddr = tempAddr.Substring(tempLength);
                            }
                        }
                        gtf.SetDataDetail(address[0].ToString(), 41, ' ', true);
                    }
                    else
                        gtf.SetDataDetail(dt.Rows[i]["Address"].ToString(), 41, ' ', true);

                    gtf.SetDataDetail(dt.Rows[i]["ColourCode"].ToString(), 8, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["ChassisNo"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["EngineNo"].ToString(), 11, ' ', false, true);
                    gtf.PrintData(false, false);

                    if (address.Count > 1)
                    {
                        for (int j = 1; j < address.Count; j++)
                        {
                            gtf.SetDataDetailSpace(3);
                            gtf.SetDataDetail(address[j].ToString(), 41, ' ', false, true);
                            gtf.PrintData(false, false);
                        }
                    }
                }
            }
            #endregion

            return gtf.sbDataTxt.ToString();
        }
    }
}