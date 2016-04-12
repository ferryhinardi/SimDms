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
    public class txtOmRpSalesTrn002:IRptProc 
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
            return CreateReportOmRpSalesTrn002(rptId, dt, sparam, printerloc, print, "", fullpage, oparam);
        }
        
        private string CreateReportOmRpSalesTrn002(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] textParam)
        {
            string reportID = recordId; 
            bool bCreateBy = false;
            int counterData = 0;

            #region GenerateHeader
            SalesGenerateTextFileReport gtf = new SalesGenerateTextFileReport();
            gtf.CurrentUser=CurrentUser;
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
                gtf.GenerateHeader();
            }
            #endregion

            #region DO (OmRpSalesTrn002/OmRpSalesTrn002A)
            if (reportID == "OmRpSalesTrn002" || reportID == "OmRpSalesTrn002A")
            {
                bCreateBy = false;

                string nomorDO = dt.Rows[0]["NOMORDO"].ToString();
                string compName = dt.Rows[0]["CompanyName"].ToString();
                string tgglDO = Convert.ToDateTime(dt.Rows[0]["TANGGALDO"].ToString()).ToString("dd-MMM-yyyy");
                string nomorSO = dt.Rows[0]["NOMORSO"].ToString();
                string pelanggan = dt.Rows[0]["PELANGGAN"].ToString();
                string tgglSO = Convert.ToDateTime(dt.Rows[0]["TANGGALSO"].ToString()).ToString("dd-MMM-yyyy");
                string almt1 = dt.Rows[0]["alamat1"].ToString();
                string almt2 = dt.Rows[0]["alamat2"].ToString();
                string almt3 = dt.Rows[0]["alamat3"].ToString();
                string almt4 = dt.Rows[0]["alamat4"].ToString();

                gtf.SetGroupHeader("Kepada Yth :", 51, ' ', true);
                gtf.SetGroupHeader("NOMOR      :", 12, ' ', true);
                gtf.SetGroupHeader(nomorDO, 17, ' ', false, true);
                gtf.SetGroupHeader(compName, 51, ' ', true);
                gtf.SetGroupHeader("TANGGAL DO :", 12, ' ', true);
                gtf.SetGroupHeader(tgglDO, 15, ' ', false, true);
                gtf.SetGroupHeader("Harap diserahkan kepada pemilik DO ini :", 51, ' ', true);
                gtf.SetGroupHeader("NOMOR PP   :", 12, ' ', true);
                gtf.SetGroupHeader(nomorSO, 17, ' ', false, true);
                gtf.SetGroupHeader(pelanggan, 51, ' ', true);
                gtf.SetGroupHeader("Tanggal    :", 12, ' ', true);
                gtf.SetGroupHeader(tgglSO, 15, ' ', false, true);
                gtf.SetGroupHeader(almt1, 80, ' ', false, true);
                gtf.SetGroupHeader(almt2, 80, ' ', false, true);
                gtf.SetGroupHeader(almt3, 80, ' ', false, true);
                gtf.SetGroupHeader(almt4, 80, ' ', false, true);
                gtf.SetGroupHeader("Unit kendaraan sebagai berikut :", 80, ' ', false, true);
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("MODEL", 21, ' ', true);
                gtf.SetGroupHeader("WARNA", 20, ' ', true);
                gtf.SetGroupHeader("UNIT", 4, ' ', true, false, true);
                gtf.SetGroupHeaderSpace(1);
                gtf.SetGroupHeader("NO RANGKA", 10, ' ', true);
                gtf.SetGroupHeader("NO MESIN", 9, ' ', true);
                gtf.SetGroupHeader("TAHUN", 5, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
                gtf.PrintHeader();

                string NomorDO = string.Empty;
                int tempSpace = 0;
                for (int i = 0; i <= dt.Rows.Count; i++)
                {
                    if (i == dt.Rows.Count)
                    {
                        if (reportID == "OmRpSalesTrn002")
                        {
                            for (int k = 0; k < 28; k++)
                            {
                                gtf.SetDataDetail("", 80, ' ', false, true);
                                gtf.PrintData();
                            }
                        }

                        gtf.SetDataReportLine();
                        gtf.PrintData();
                        gtf.SetDataDetail("Ket : " + dt.Rows[i - 1]["KET"].ToString(), 54, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i - 1]["CITY"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALDO"]).ToString("dd-MMM-yyyy"), 40, ' ', false, true,false);
                        gtf.PrintData();
                        if (reportID == "OmRpSalesTrn002")
                        {
                            gtf.SetDataDetail("", 80, ' ', false, true);
                            gtf.PrintData();
                        }
                        gtf.SetDataDetailSpace(11);
                        gtf.SetDataDetail("Penerima", 8, ' ');
                        gtf.SetDataDetailSpace(18);
                        gtf.SetDataDetail("Menyetujui", 10, ' ');
                        gtf.SetDataDetailSpace(13);
                        gtf.SetDataDetail("Fin./Acc. Dept.", 15, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 80, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 80, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetailSpace(3);
                        if (dt.Rows[i - 1]["PELANGGAN"].ToString().Length < 24)
                            tempSpace = (24 - dt.Rows[i - 1]["PELANGGAN"].ToString().Length) / 2;
                        else
                            tempSpace = 0;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["PELANGGAN"].ToString(), dt.Rows[i - 1]["PELANGGAN"].ToString().Length, ' ');
                        gtf.SetDataDetailSpace(tempSpace + 4);
                        if (dt.Rows[i - 1]["SignName1"].ToString().Length < 24)
                            tempSpace = (24 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                        else
                            tempSpace = 0;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ');
                        gtf.SetDataDetailSpace(tempSpace + 2);
                        if (dt.Rows[i - 1]["SignName2"].ToString().Length < 25)
                            tempSpace = (25 - dt.Rows[i - 1]["SignName2"].ToString().Length) / 2;
                        else
                            tempSpace = 0;
                        gtf.SetDataDetailSpace(tempSpace);
                        gtf.SetDataDetail(dt.Rows[i - 1]["SignName2"].ToString(), dt.Rows[i - 1]["SignName2"].ToString().Length, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetailSpace(3);
                        gtf.SetDataDetail("-", 24, '-', true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail("-", 24, '-', true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail("-", 25, '-', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetailSpace(29);
                        gtf.SetDataDetail(PadBoth(dt.Rows[i - 1]["TitleSign1"].ToString(),24), 24, ' ', true);                        
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(PadBoth(dt.Rows[i - 1]["TitleSign2"].ToString(),25), 25, ' ', false, true);                        
                        gtf.PrintData();
                        gtf.SetDataDetail("Note : Setiap kekurangan dan/atau kerusakan yang diajukan setelah", 80, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("       kendaraan tsb keluar dari gudang tidak dapat kami layani", 80, ' ', false, true);
                        gtf.PrintData();

                        break;
                    }

                    if (NomorDO == string.Empty)
                    {
                        NomorDO = dt.Rows[i]["NOMORDO"].ToString();
                    }
                    else
                    {
                        if (NomorDO != dt.Rows[i]["NOMORDO"].ToString())
                        {
                            if (reportID == "OmRpSalesTrn002")
                            {
                                for (int k = 0; k < 28; k++)
                                {
                                    gtf.SetDataDetail("", 80, ' ', false, true);
                                    gtf.PrintData();
                                }
                            }

                            gtf.SetDataReportLine();
                            gtf.PrintData();
                            gtf.SetDataDetail("Ket : " + dt.Rows[i - 1]["KET"].ToString(), 54, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["CITY"].ToString() + ", " + Convert.ToDateTime(dt.Rows[i - 1]["TANGGALDO"]).ToString("dd-MMM-yyyy"), 40, ' ', false, true, false);
                            gtf.PrintData();
                            if (reportID == "OmRpSalesTrn002")
                            {
                                gtf.SetDataDetail("", 80, ' ', false, true);
                                gtf.PrintData();
                            }
                            gtf.SetDataDetailSpace(11);
                            gtf.SetDataDetail("Penerima", 8, ' ');
                            gtf.SetDataDetailSpace(18);
                            gtf.SetDataDetail("Menyetujui", 10, ' ');
                            gtf.SetDataDetailSpace(13);
                            gtf.SetDataDetail("Fin./Acc. Dept.", 15, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 80, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 80, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(3);
                            if (dt.Rows[i - 1]["PELANGGAN"].ToString().Length < 24)
                                tempSpace = (24 - dt.Rows[i - 1]["PELANGGAN"].ToString().Length) / 2;
                            else
                                tempSpace = 0;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["PELANGGAN"].ToString(), dt.Rows[i - 1]["PELANGGAN"].ToString().Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace + 4);
                            if (dt.Rows[i - 1]["SignName1"].ToString().Length < 24)
                                tempSpace = (24 - dt.Rows[i - 1]["SignName1"].ToString().Length) / 2;
                            else
                                tempSpace = 0;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SignName1"].ToString(), dt.Rows[i - 1]["SignName1"].ToString().Length, ' ');
                            gtf.SetDataDetailSpace(tempSpace + 2);
                            if (dt.Rows[i - 1]["SignName2"].ToString().Length < 25)
                                tempSpace = (25 - dt.Rows[i - 1]["SignName2"].ToString().Length) / 2;
                            else
                                tempSpace = 0;
                            gtf.SetDataDetailSpace(tempSpace);
                            gtf.SetDataDetail(dt.Rows[i - 1]["SignName2"].ToString(), dt.Rows[i - 1]["SignName2"].ToString().Length, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(3);
                            gtf.SetDataDetail("-", 24, '-', true);
                            gtf.SetDataDetailSpace(1);
                            gtf.SetDataDetail("-", 24, '-', true);
                            gtf.SetDataDetailSpace(1);
                            gtf.SetDataDetail("-", 25, '-', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(29);
                            gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign1"].ToString(), 24, ' ', true);
                            gtf.SetDataDetailSpace(1);
                            gtf.SetDataDetail(dt.Rows[i - 1]["TitleSign2"].ToString(), 25, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("Note : Setiap kekurangan dan/atau kerusakan yang diajukan setelah", 80, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("       kendaraan tsb keluar dari gudang tidak dapat kami layani", 80, ' ', false, true);
                            gtf.PrintData();

                            gtf.ReplaceGroupHdr(dt.Rows[i]["NOMORDO"].ToString(), nomorDO, 15);
                            gtf.ReplaceGroupHdr(dt.Rows[i]["CompanyName"].ToString(), compName, 51);
                            gtf.ReplaceGroupHdr(Convert.ToDateTime(dt.Rows[i]["TANGGALDO"].ToString()).ToString("dd-MMM-yyyy"), tgglDO, 15);
                            gtf.ReplaceGroupHdr(dt.Rows[i]["NOMORSO"].ToString(), nomorSO, 15);
                            gtf.ReplaceGroupHdr(dt.Rows[i]["PELANGGAN"].ToString(), pelanggan, 51);
                            gtf.ReplaceGroupHdr(Convert.ToDateTime(dt.Rows[i]["TANGGALSO"].ToString()).ToString("dd-MMM-yyyy"), tgglSO, 15);
                            gtf.ReplaceGroupHdr(dt.Rows[i]["alamat1"].ToString(), almt1, 80);
                            gtf.ReplaceGroupHdr(dt.Rows[i]["alamat2"].ToString(), almt2, 80);
                            gtf.ReplaceGroupHdr(dt.Rows[i]["alamat3"].ToString(), almt3, 80);
                            gtf.ReplaceGroupHdr(dt.Rows[i]["alamat4"].ToString(), almt4, 80);

                            nomorDO = dt.Rows[i]["NOMORDO"].ToString();
                            compName = dt.Rows[i]["CompanyName"].ToString();
                            tgglDO = Convert.ToDateTime(dt.Rows[i]["TANGGALDO"].ToString()).ToString("dd-MMM-yyyy");
                            nomorSO = dt.Rows[i]["NOMORSO"].ToString();
                            pelanggan = dt.Rows[i]["PELANGGAN"].ToString();
                            tgglSO = Convert.ToDateTime(dt.Rows[i]["TANGGALSO"].ToString()).ToString("dd-MMM-yyyy");
                            almt1 = dt.Rows[i]["alamat1"].ToString();
                            almt2 = dt.Rows[i]["alamat2"].ToString();
                            almt3 = dt.Rows[i]["alamat3"].ToString();
                            almt4 = dt.Rows[i]["alamat4"].ToString();

                            counterData = 0;
                            NomorDO = dt.Rows[i]["NOMORDO"].ToString();
                        }
                    }

                    counterData += 1;
                    gtf.SetDataDetail(counterData.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["MODEL"].ToString(), 21, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["WARNA"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["UNIT"].ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetailSpace(1);
                    gtf.SetDataDetail(dt.Rows[i]["NORANGKA"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["NOMESIN"].ToString(), 9, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["TAHUN"].ToString(), 5, ' ', false, true, true);

                    gtf.PrintData(true);
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