using SimDms.Common;
using SimDms.Sales.Models.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sales
{
    public class txtOmRpSalesTrn003 : IRptProc
    {
        private SimDms.Sales.Models.DataContext ctx = new SimDms.Sales.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public Models.SysUser CurrentUser { get; set; }

        private object[] setTextParameter, paramText;

        public void SetTextParameter(params object[] textParam)
        {
            setTextParameter = textParam;
        }

        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            //var dt = ctx.Database.SqlQuery<OmRpSalesTrn002>(string.Format("exec {0} {1}", sproc, sparam));
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "exec " + sproc + " " + sparam;
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            DataSet dt = new DataSet();

            SetTextParameter(oparam);

            da.Fill(dt);
            return CreateReportOmRpSalesTrn003(rptId, dt, sparam, printerloc, print, "", fullpage);
        }

        private string CreateReportOmRpSalesTrn003(string recordId, DataSet dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {            
            bool bCreateBy = false;
            int counterData = 0;

            string namaPelanggan = string.Empty, soNO = string.Empty, alamat1 = string.Empty, tgglSO = string.Empty, alamat2 = string.Empty,
                SKPKNo = string.Empty, alamat3 = string.Empty, refferenceNo = string.Empty, alamat4 = string.Empty,
                namaGovPelanggan = string.Empty, leasingName = string.Empty, leasingCo = string.Empty, alamatFaktur1 = string.Empty,
                alamatFaktur2 = string.Empty, alamatFaktur3 = string.Empty, alamatFaktur4 = string.Empty, NPWPNo = string.Empty,
                TOPName = string.Empty, sales = string.Empty, tipeSales = string.Empty, requestDate = string.Empty, shipName = string.Empty,
                soDate = string.Empty, ket = string.Empty;
            string tempTerbilang = string.Empty, tempTerbilang1 = string.Empty, tempTerbilang2 = string.Empty;
            string[] tempSplitTerbilang;
            //var dtt = dt.FirstOrDefault();


            #region GenerateHeader
            SalesGenerateTextFileReport gtf = new SalesGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter("");
            gtf.SetPaperSize(1100, 850);

            if (recordId == "OmRpSalesTrn009A" || recordId == "OmRpSalesTrn003A" || recordId == "OmRpSalesTrn003D" ||
                recordId == "OmRpSalesTrn003C" || recordId == "OmRpSalesTrn002A" || recordId == "OmRpSalesTrn001D" ||
                recordId == "OmRpSalesTrn001E" || recordId == "OmRpSalesTrn001F" || recordId == "OmRpSalesTrn001G" ||
                recordId == "OmRpPurTrn001A" || recordId == "OmRpPurTrn002A" || recordId == "OmRpPurTrn003A" ||
                recordId == "OmRpSalesTrn006" || recordId == "OmRpSalesTrn006A" || recordId == "OmRpPurTrn008A" ||
                recordId == "OmRpPurTrn009" || recordId == "OmRpSalesTrn005A" || recordId == "OmRpSalesTrn010A" ||
                recordId == "OmRpStock001")
                fullPage = false;

            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, "", fullPage);

            if (recordId == "OmRpSalesTrn004" || recordId == "OmRpSalesTrn004A" || recordId == "OmRpSalesTrn003A" ||
                recordId == "OmRpSalesTrn007DNew" || recordId == "OmRpSalesTrn007" || recordId == "OmRpSalesTrn007C" ||
                recordId == "OmRpSalesTrn007A" || recordId == "OmRpSalesTrn010" || recordId == "OmRpSalesTrn010A" ||
                recordId == "OmRpStock002")
            {                
                gtf.GenerateHeader2(false);
            }
            else if (recordId == "OmRpSalesTrn003D" || recordId == "OmRpSalesTrn001D" ||
                recordId == "OmRpSalesTrn001E" || recordId == "OmRpSalesTrn006" || recordId == "OmRpSalesTrn006A" ||
                recordId == "OmRpPurTrn009" || recordId == "OmRpSalesTrn005" || recordId == "OmRpSalesTrn005A" || recordId == "OmRpStock001")
            {
                gtf.GenerateHeader2(false, false);
            }
            else
            {
                gtf.GenerateHeader();
            }


            #endregion

            #region BPK (OmRpSalesTrn003A/OmRpSalesTrn003B/OmRpSalesTrn003C/OmRpSalesTrn003D)
            if (recordId == "OmRpSalesTrn003A" || recordId == "OmRpSalesTrn003B" || recordId == "OmRpSalesTrn003C" || recordId == "OmRpSalesTrn003D")
            {
                bCreateBy = false;
                int loopData = 0;
                DataRow[] dRow = new DataRow[] { };
                string custName = string.Empty, gudang = string.Empty, noBPK = string.Empty,
                    tgglBPK = string.Empty, telp = string.Empty, DONo = string.Empty, SONo = string.Empty,
                    tahun = string.Empty, salesModelDesc = string.Empty;

                string NoBPK = string.Empty, SEQ = string.Empty;

                #region OmRpSalesTrn003A / OmRpSalesTrn003D (Preprinted/Formatted)
                if (recordId == "OmRpSalesTrn003A" || recordId == "OmRpSalesTrn003D")
                {
                    custName = dt.Tables[1].Rows[0]["CustomerName"].ToString();
                    gudang = dt.Tables[1].Rows[0]["GUDANG"].ToString();
                    alamat1 = dt.Tables[1].Rows[0]["ALAMAT1"].ToString();
                    alamat2 = dt.Tables[1].Rows[0]["ALAMAT2"].ToString();
                    noBPK = dt.Tables[1].Rows[0]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[0]["SEQ"].ToString() + ")";
                    alamat3 = dt.Tables[1].Rows[0]["ALAMAT3"].ToString();
                    tgglBPK = Convert.ToDateTime(dt.Tables[1].Rows[0]["TANGGALBPK"]).ToString("dd-MMM-yyyy");
                    telp = dt.Tables[1].Rows[0]["TELP"].ToString();
                    DONo = dt.Tables[1].Rows[0]["DONO"].ToString();
                    SONo = dt.Tables[1].Rows[0]["SONO"].ToString();
                    tahun = dt.Tables[1].Rows[0]["TAHUN"].ToString();
                    salesModelDesc = dt.Tables[1].Rows[0]["SalesModelDesc"].ToString();

                    #region OmRpSalesTrn003A (Preprinted)
                    if (recordId == "OmRpSalesTrn003A")
                    {
                        gtf.SetGroupHeaderSpace(13);
                        gtf.SetGroupHeader(custName, 51, ' ', true);
                        gtf.SetGroupHeaderSpace(10);
                        gtf.SetGroupHeader(gudang, 19, ' ', false, true);
                        gtf.SetGroupHeaderSpace(13);
                        gtf.SetGroupHeader(alamat1, 51, ' ', true);
                        gtf.SetGroupHeader("", 19, ' ', false, true);
                        gtf.SetGroupHeaderSpace(13);
                        gtf.SetGroupHeader(alamat2, 51, ' ', true);
                        gtf.SetGroupHeaderSpace(10);
                        gtf.SetGroupHeader(noBPK, 21, ' ', false, true);
                        gtf.SetGroupHeaderSpace(13);
                        gtf.SetGroupHeader(alamat3, 51, ' ', true);
                        gtf.SetGroupHeaderSpace(10);
                        gtf.SetGroupHeader(tgglBPK, 21, ' ', false, true);
                        gtf.SetGroupHeaderSpace(13);
                        gtf.SetGroupHeader(telp, 51, ' ', true);
                        gtf.SetGroupHeaderSpace(10);
                        gtf.SetGroupHeader(DONo, 21, ' ', false, true);
                        gtf.SetGroupHeaderSpace(75);
                        gtf.SetGroupHeader(SONo, 21, ' ', false, true);
                        gtf.SetGroupHeader("", 96, ' ', false, true);                        

                        gtf.SetGroupHeaderSpace(5);
                        gtf.SetGroupHeader("Produksi", 11, ' ', true);
                        gtf.SetGroupHeader(":", 1, ' ', true);
                        gtf.SetGroupHeader(tahun, 24, ' ', false, true);
                        gtf.SetGroupHeaderSpace(19);
                        gtf.SetGroupHeader(salesModelDesc, 60, ' ', false, true);
                        gtf.PrintHeader();

                        int loop = 0;

                        for (int i = 0; i <= dt.Tables[1].Rows.Count; i++)                        
                        {
                            if (i == 10)
                                break;
                            
                            if (i == dt.Tables[1].Rows.Count && dt.Tables[1].Rows.Count<=8)
                            {                               
                                    loop = 18 - gtf.line;
                                    for (int j = 0; j < loop; j++)
                                    {
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                    }
                               


                                gtf.SetDataDetail("     " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 16, ' ', true);
                                gtf.SetDataDetail("   " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 16, ' ', true);
                                gtf.SetDataDetail(" " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 13, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("     " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 14, ' ', true);
                                gtf.SetDataDetail("     " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 13, ' ', true);
                                gtf.SetDataDetail("", 45, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetailSpace(68);
                                gtf.SetDataDetail(Convert.ToDateTime(dt.Tables[1].Rows[i - 1]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 11, ' ', false, true, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetailSpace(50);
                                gtf.SetDataDetail(dt.Tables[1].Rows[i - 1]["SignName1"].ToString(), 32, ' ', false, false);
                                //gtf.PrintData(false, false);
                                //gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                break;
                            }
                            else if (i>= 8)
                            {
                             
                                gtf.SetDataDetail("   " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 14, ' ', true);
                                gtf.SetDataDetail("   " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 15, ' ', true);
                                gtf.SetDataDetail("    " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 21, ' ', false);
                                gtf.SetDataDetail("[ ] " + dt.Tables[1].Rows[i]["NamaPerlengkapan"].ToString(), 21, ' ',true);
                                gtf.SetDataDetail(dt.Tables[1].Rows[i]["Quantity"].ToString(), 2, ' ', false, true);

                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("   " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 14, ' ', true);                                
                                gtf.SetDataDetail("   " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 36, ' ', true);
                                if(dt.Tables[1].Rows.Count>9)
                                {
                                    gtf.SetDataDetail("[ ] " + dt.Tables[1].Rows[i+1]["NamaPerlengkapan"].ToString(), 21, ' ', true);
                                    gtf.SetDataDetail(dt.Tables[1].Rows[i+1]["Quantity"].ToString(), 2, ' ', false);
                                }
                                gtf.SetDataDetail("", 45, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetailSpace(68);
                                gtf.SetDataDetail(Convert.ToDateTime(dt.Tables[1].Rows[i - 1]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 11, ' ', false, true, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetailSpace(50);
                                gtf.SetDataDetail(dt.Tables[1].Rows[i - 1]["SignName1"].ToString(), 32, ' ', false, false);
                                //gtf.PrintData(false, false);
                                //gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);


                                break;

                            }
                            #region MyRegion
                            if (NoBPK == string.Empty)
                            {
                                #region NoBPK == string.Empty
                                NoBPK = dt.Tables[1].Rows[i]["NOBPK"].ToString();
                                if (SEQ == string.Empty)
                                {
                                    SEQ = dt.Tables[1].Rows[i]["SEQ"].ToString();
                                }
                                #endregion
                            }
                            else
                            {
                                if (NoBPK != dt.Tables[1].Rows[i]["NOBPK"].ToString())
                                {
                                    #region MyRegion
                                    if (SEQ != dt.Tables[1].Rows[i]["SEQ"].ToString())
                                    {
                                        #region MyRegion
                                        loop = (30 - 9) - gtf.line;
                                        for (int j = 0; j < loop; j++)
                                        {
                                            gtf.SetDataDetail("", 96, ' ', false, true);
                                            gtf.PrintData(false, false);
                                        }

                                        gtf.SetDataDetail("  " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("  " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 13, ' ', true);
                                        gtf.SetDataDetail("   " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 13, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("  " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("  " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 13, ' ', true);
                                        gtf.SetDataDetail("", 45, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetailSpace(85);
                                        gtf.SetDataDetail(Convert.ToDateTime(dt.Tables[1].Rows[i - 1]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 11, ' ', false, true, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetailSpace(50);
                                        gtf.SetDataDetail(dt.Tables[1].Rows[i - 1]["SignName1"].ToString(), 32, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);

                                        counterData = 0;
                                        SEQ = dt.Tables[1].Rows[i]["SEQ"].ToString();

                                        gtf.ReplaceGroupHdr(tahun, dt.Tables[1].Rows[i]["TAHUN"].ToString());
                                        gtf.ReplaceGroupHdr(salesModelDesc, dt.Tables[1].Rows[i]["SalesModelDesc"].ToString());
                                        gtf.ReplaceGroupHdr(noBPK, dt.Tables[1].Rows[i]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[i]["SEQ"].ToString() + ")");

                                        tahun = dt.Tables[1].Rows[i]["TAHUN"].ToString();
                                        salesModelDesc = dt.Tables[1].Rows[i]["SalesModelDesc"].ToString();
                                        noBPK = dt.Tables[1].Rows[i]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[i]["SEQ"].ToString() + ")";
                                        #endregion
                                    }

                                    NoBPK = dt.Tables[1].Rows[i]["NOBPK"].ToString();

                                    gtf.ReplaceGroupHdr(custName, dt.Tables[1].Rows[i]["CustomerName"].ToString(), 52);
                                    gtf.ReplaceGroupHdr(gudang, dt.Tables[1].Rows[i]["GUDANG"].ToString(), 19);
                                    gtf.ReplaceGroupHdr(alamat1, dt.Tables[1].Rows[i]["ALAMAT1"].ToString(), 52);
                                    gtf.ReplaceGroupHdr(alamat2, dt.Tables[1].Rows[i]["ALAMAT2"].ToString(), 52);
                                    gtf.ReplaceGroupHdr(noBPK, dt.Tables[1].Rows[i]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[i]["SEQ"].ToString() + ")", 19);
                                    gtf.ReplaceGroupHdr(alamat3, dt.Tables[1].Rows[i]["ALAMAT3"].ToString(), 52);
                                    gtf.ReplaceGroupHdr(tgglBPK, Convert.ToDateTime(dt.Tables[1].Rows[i]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 19);
                                    gtf.ReplaceGroupHdr(telp, dt.Tables[1].Rows[i]["TELP"].ToString(), 52);
                                    gtf.ReplaceGroupHdr(DONo, dt.Tables[1].Rows[i]["DONO"].ToString(), 19);
                                    gtf.ReplaceGroupHdr(SONo, dt.Tables[1].Rows[i]["SONO"].ToString(), 19);

                                    custName = dt.Tables[1].Rows[i]["CustomerName"].ToString();
                                    gudang = dt.Tables[1].Rows[i]["GUDANG"].ToString();
                                    alamat1 = dt.Tables[1].Rows[i]["ALAMAT1"].ToString();
                                    alamat2 = dt.Tables[1].Rows[i]["ALAMAT2"].ToString();
                                    noBPK = dt.Tables[1].Rows[i]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[i]["SEQ"].ToString() + ")";
                                    alamat3 = dt.Tables[1].Rows[i]["ALAMAT3"].ToString();
                                    tgglBPK = Convert.ToDateTime(dt.Tables[1].Rows[i]["TANGGALBPK"]).ToString("dd-MMM-yyyy");
                                    telp = dt.Tables[1].Rows[i]["TELP"].ToString();
                                    DONo = dt.Tables[1].Rows[i]["DONO"].ToString();
                                    SONo = dt.Tables[1].Rows[i]["SONO"].ToString();
                                    #endregion
                                }
                                else
                                {
                                    if (SEQ != dt.Tables[1].Rows[i]["SEQ"].ToString())
                                    {
                                        #region MyRegion
                                        loop = (30 - 9) - gtf.line;
                                        for (int j = 0; j < loop; j++)
                                        {
                                            gtf.SetDataDetail("", 96, ' ', false, true);
                                            gtf.PrintData(false, false);
                                        }

                                        gtf.SetDataDetail("  " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("  " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 13, ' ', true);
                                        gtf.SetDataDetail("   " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 13, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("  " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("  " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 13, ' ', true);
                                        gtf.SetDataDetail("", 45, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetailSpace(85);
                                        gtf.SetDataDetail(Convert.ToDateTime(dt.Tables[1].Rows[i - 1]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 11, ' ', false, true, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetailSpace(50);
                                        gtf.SetDataDetail(dt.Tables[1].Rows[i - 1]["SignName1"].ToString(), 32, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);

                                        counterData = 0;
                                        SEQ = dt.Tables[1].Rows[i]["SEQ"].ToString();

                                        gtf.ReplaceGroupHdr(tahun, dt.Tables[1].Rows[i]["TAHUN"].ToString());
                                        gtf.ReplaceGroupHdr(salesModelDesc, dt.Tables[1].Rows[i]["SalesModelDesc"].ToString());
                                        gtf.ReplaceGroupHdr(noBPK, dt.Tables[1].Rows[i]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[i]["SEQ"].ToString() + ")");

                                        tahun = dt.Tables[1].Rows[i]["TAHUN"].ToString();
                                        salesModelDesc = dt.Tables[1].Rows[i]["SalesModelDesc"].ToString();
                                        noBPK = dt.Tables[1].Rows[i]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[i]["SEQ"].ToString() + ")";
                                        #endregion
                                    }
                                }
                            }
                            
                            #endregion

                            if (counterData == 0)
                            {
                                gtf.SetDataDetailSpace(19);
                                gtf.SetDataDetail(dt.Tables[1].Rows[i]["WARNA"].ToString(), 32, ' ', true,true);
                                gtf.SetDataDetailSpace(51);
                            }
                            else
                            {
                                gtf.SetDataDetailSpace(52);
                            }

                            if (i < 8)
                            {
                                gtf.SetDataDetail("[ ] " + dt.Tables[1].Rows[i]["NamaPerlengkapan"].ToString(), 21, ' ', true);
                                gtf.SetDataDetail(dt.Tables[1].Rows[i]["Quantity"].ToString(), 2, ' ', false, true);
                                gtf.PrintData(false, false);
                            }
                            counterData += 1;
                        }
                    }
                    #endregion

                    #region OmRpSalesTrn003D (Formatted)
                    if (recordId == "OmRpSalesTrn003D")
                    {
                        gtf.SetGroupHeader(dt.Tables[0].Rows[0]["CompanyName"].ToString(), 64, ' ', true);
                        gtf.SetGroupHeader("BUKTI PENYERAHAN KENDARAAN", 26, ' ', false, true);
                        gtf.SetGroupHeader("Kepada Yth :", 12, ' ', true);
                        gtf.SetGroupHeader(custName, 51, ' ', true);
                        gtf.SetGroupHeader("Gudang    :", 11, ' ', true);
                        gtf.SetGroupHeader(gudang, 19, ' ', false, true);
                        gtf.SetGroupHeaderSpace(13);
                        gtf.SetGroupHeader(alamat1, 51, ' ', true);
                        gtf.SetGroupHeader("", 19, ' ', false, true);
                        gtf.SetGroupHeaderSpace(13);
                        gtf.SetGroupHeader(alamat2, 51, ' ', true);
                        gtf.SetGroupHeader("Nomor     :", 11, ' ', true);
                        gtf.SetGroupHeader(noBPK, 19, ' ', false, true);
                        gtf.SetGroupHeaderSpace(13);
                        gtf.SetGroupHeader(alamat3, 51, ' ', true);
                        gtf.SetGroupHeader("Tanggal   :", 11, ' ', true);
                        gtf.SetGroupHeader(tgglBPK, 19, ' ', false, true);
                        gtf.SetGroupHeaderSpace(13);
                        gtf.SetGroupHeader(telp, 51, ' ', true);
                        gtf.SetGroupHeader("DO NO.    :", 11, ' ', true);
                        gtf.SetGroupHeader(DONo, 19, ' ', false, true);
                        gtf.SetGroupHeaderSpace(65);
                        gtf.SetGroupHeader("SKPK NO.  :", 11, ' ', true);
                        gtf.SetGroupHeader(SONo, 19, ' ', false, true);
                        gtf.SetGroupHeader("Dengan ini kami serahkan 1 (satu) unit kendaraan SUZUKI", 96, ' ', false, true);
                        gtf.SetGroupHeader(" -", 95, '-', false, true);
                        gtf.SetGroupHeader("|", 1, ' ', true);
                        gtf.SetGroupHeaderSpace(2);
                        gtf.SetGroupHeader("Produksi", 12, ' ', true);
                        gtf.SetGroupHeader(":", 1, ' ', true);
                        gtf.SetGroupHeader(tahun, 27, ' ', true);
                        gtf.SetGroupHeader("|", 1, ' ', true);
                        gtf.SetGroupHeader("       Setiap Unit dilengkapi dengan", 36, ' ', true);
                        gtf.SetGroupHeaderSpace(9);
                        gtf.SetGroupHeader("|", 1, ' ', false, true);
                        gtf.SetGroupHeader("|", 1, ' ', true);
                        gtf.SetGroupHeaderSpace(2);
                        gtf.SetGroupHeader("Type", 12, ' ', true);
                        gtf.SetGroupHeader(":", 1, ' ', true);
                        gtf.SetGroupHeader(salesModelDesc, 27, ' ', true);
                        gtf.SetGroupHeader("|", 1, ' ');
                        gtf.SetGroupHeader("-", 47, '-');
                        gtf.SetGroupHeader("|", 1, ' ', false, true);
                        gtf.PrintHeader();

                        loopData = 0;
                        int indexData = 10, temp = 0;

                        for (int i = 0; i <= dt.Tables[1].Rows.Count; i++)
                        {
                            if (i == dt.Tables[1].Rows.Count)
                            {
                                if (counterData > 5 && counterData < 10)
                                {
                                    // Print Header No Rangka, No Mesin, Buku Service
                                    if (loopData == 1)
                                    {
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("  No. Rangka  ", 15, ' ');
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("  No. Mesin  ", 14, ' ');
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail(" Buku Service ", 15, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("-", 46, '-');
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|", 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail(" -", 95, '-', false, true);
                                    }
                                    // Print Line
                                    else if (loopData == 2)
                                    {
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("-", 46, '-');
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|", 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail(" -", 95, '-', false, true);
                                    }
                                    else if (loopData == 3)
                                    {
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|", 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail(" -", 95, '-', false, true);
                                    }
                                    else if (loopData == 4)
                                    {
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|", 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail(" -", 95, '-', false, true);
                                    }
                                }
                                else
                                {
                                    gtf.SetDataDetail(" -", 95, '-', false, true);
                                }

                                gtf.SetDataDetail("Kendaraan dalam keadaan baik.", 29, ' ');
                                gtf.SetDataDetail(dt.Tables[1].Rows[i - 1]["CITY"].ToString() + ", " + Convert.ToDateTime(dt.Tables[1].Rows[i - 1]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 61, ' ', false, true, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("Kehilangan, kerusakan setelah diterima bukan", 44, ' ', true);
                                gtf.SetDataDetailSpace(5);
                                gtf.SetDataDetail("Diserahkan Oleh :", 17, ' ', true);
                                gtf.SetDataDetailSpace(11);
                                gtf.SetDataDetail("Diterima Oleh :", 15, ' ', false, true);
                                gtf.SetDataDetail("tanggungan kami lagi", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("[ ] Lengkap", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("[ ] Tidak Lengkap", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail(" _", 21, '_', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("|", 1, ' ');
                                gtf.SetDataDetail("_", 20, '_');
                                gtf.SetDataDetail("|", 1, ' ');
                                temp = 55 - (dt.Tables[1].Rows[i - 1]["SignName1"].ToString().Length);
                                gtf.SetDataDetailSpace(temp - 5);
                                gtf.SetDataDetail("( " + dt.Tables[1].Rows[i - 1]["SignName1"].ToString() + " )",
                                    dt.Tables[1].Rows[i - 1]["SignName1"].ToString().Length + 5, ' ', true);
                                if (dt.Tables[1].Rows[i - 1]["SignName2"].ToString() == string.Empty)
                                {
                                    gtf.SetDataDetail("( ", 2, ' ');
                                    gtf.SetDataDetailSpace(13);
                                    gtf.SetDataDetail(") ", 2, ' ', false, true);
                                }
                                else gtf.SetDataDetail("( " + dt.Tables[1].Rows[i - 1]["SignName2"].ToString() + " )", 16, ' ', false, true);
                                gtf.PrintData(false, false);
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData(false, false);
                                break;
                            }

                            if (NoBPK == string.Empty)
                            {
                                NoBPK = dt.Tables[1].Rows[i]["NOBPK"].ToString();
                                if (SEQ == string.Empty)
                                {
                                    SEQ = dt.Tables[1].Rows[i]["SEQ"].ToString();
                                }
                            }
                            else
                            {
                                if (NoBPK != dt.Tables[1].Rows[i]["NOBPK"].ToString())
                                {
                                    if (SEQ != dt.Tables[1].Rows[i]["SEQ"].ToString())
                                    {
                                        if (counterData > 5)
                                        {
                                            // Print Header No Rangka, No Mesin, Buku Service
                                            if (loopData == 1)
                                            {
                                                gtf.SetDataDetail("|", 1, ' ');
                                                gtf.SetDataDetail("  No. Rangka  ", 15, ' ');
                                                gtf.SetDataDetail("|", 1, ' ');
                                                gtf.SetDataDetail("  No. Mesin  ", 14, ' ');
                                                gtf.SetDataDetail("|", 1, ' ');
                                                gtf.SetDataDetail(" Buku Service ", 15, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail("|", 1, ' ');
                                                gtf.SetDataDetail("-", 46, '-');
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|", 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail(" -", 95, '-', false, true);
                                            }
                                            // Print Line
                                            else if (loopData == 2)
                                            {
                                                gtf.SetDataDetail("|", 1, ' ');
                                                gtf.SetDataDetail("-", 46, '-');
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|", 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail(" -", 95, '-', false, true);
                                            }
                                            else if (loopData == 3)
                                            {
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|", 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail(" -", 95, '-', false, true);
                                            }
                                            else if (loopData == 4)
                                            {
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|", 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail(" -", 95, '-', false, true);
                                            }
                                        }
                                        else
                                        {
                                            gtf.SetDataDetail(" -", 95, '-', false, true);
                                        }

                                        gtf.SetDataDetail("Kendaraan dalam keadaan baik.", 29, ' ');
                                        gtf.SetDataDetail(dt.Tables[1].Rows[i - 1]["CITY"].ToString() + ", " + Convert.ToDateTime(dt.Tables[1].Rows[i - 1]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 61, ' ', false, true, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("Kehilangan, kerusakan setelah diterima bukan", 44, ' ', true);
                                        gtf.SetDataDetailSpace(5);
                                        gtf.SetDataDetail("Diserahkan Oleh :", 17, ' ', true);
                                        gtf.SetDataDetailSpace(11);
                                        gtf.SetDataDetail("Diterima Oleh :", 15, ' ', false, true);
                                        gtf.SetDataDetail("tanggungan kami lagi", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("[ ] Lengkap", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("[ ] Tidak Lengkap", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail(" _", 21, '_', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("_", 20, '_');
                                        gtf.SetDataDetail("|", 1, ' ');
                                        temp = 55 - (dt.Tables[1].Rows[i - 1]["SignName1"].ToString().Length);
                                        gtf.SetDataDetailSpace(temp - 5);
                                        gtf.SetDataDetail("( " + dt.Tables[1].Rows[i - 1]["SignName1"].ToString() + " )",
                                            dt.Tables[1].Rows[i - 1]["SignName1"].ToString().Length + 5, ' ', true);
                                        if (dt.Tables[1].Rows[i - 1]["SignName2"].ToString() == string.Empty)
                                        {
                                            gtf.SetDataDetail("( ", 2, ' ');
                                            gtf.SetDataDetailSpace(13);
                                            gtf.SetDataDetail(") ", 2, ' ', false, true);
                                        }
                                        else gtf.SetDataDetail("( " + dt.Tables[1].Rows[i - 1]["SignName2"].ToString() + " )", 16, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);

                                        counterData = loopData = 0;
                                        SEQ = dt.Tables[1].Rows[i]["SEQ"].ToString();

                                        gtf.ReplaceGroupHdr(tahun, dt.Tables[1].Rows[i]["TAHUN"].ToString());
                                        gtf.ReplaceGroupHdr(salesModelDesc, dt.Tables[1].Rows[i]["SalesModelDesc"].ToString(), 23);
                                        gtf.ReplaceGroupHdr(noBPK, dt.Tables[1].Rows[i]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[i]["SEQ"].ToString() + ")");

                                        tahun = dt.Tables[1].Rows[i]["TAHUN"].ToString();
                                        salesModelDesc = dt.Tables[1].Rows[i]["SalesModelDesc"].ToString();
                                        noBPK = dt.Tables[1].Rows[i]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[i]["SEQ"].ToString() + ")";

                                    }

                                    NoBPK = dt.Tables[1].Rows[i]["NOBPK"].ToString();

                                    gtf.ReplaceGroupHdr(custName, dt.Tables[1].Rows[i]["CustomerName"].ToString(), 51);
                                    gtf.ReplaceGroupHdr(gudang, dt.Tables[1].Rows[i]["GUDANG"].ToString(), 19);
                                    gtf.ReplaceGroupHdr(alamat1, dt.Tables[1].Rows[i]["ALAMAT1"].ToString(), 51);
                                    gtf.ReplaceGroupHdr(alamat2, dt.Tables[1].Rows[i]["ALAMAT2"].ToString(), 51);
                                    gtf.ReplaceGroupHdr(noBPK, dt.Tables[1].Rows[i]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[i]["SEQ"].ToString() + ")", 19);
                                    gtf.ReplaceGroupHdr(alamat3, dt.Tables[1].Rows[i]["ALAMAT3"].ToString(), 51);
                                    gtf.ReplaceGroupHdr(tgglBPK, Convert.ToDateTime(dt.Tables[1].Rows[i]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 19);
                                    gtf.ReplaceGroupHdr(telp, dt.Tables[1].Rows[i]["TELP"].ToString(), 51);
                                    gtf.ReplaceGroupHdr(DONo, dt.Tables[1].Rows[i]["DONO"].ToString(), 19);
                                    gtf.ReplaceGroupHdr(SONo, dt.Tables[1].Rows[i]["SONO"].ToString(), 19);

                                    custName = dt.Tables[1].Rows[i]["CustomerName"].ToString();
                                    gudang = dt.Tables[1].Rows[i]["GUDANG"].ToString();
                                    alamat1 = dt.Tables[1].Rows[i]["ALAMAT1"].ToString();
                                    alamat2 = dt.Tables[1].Rows[i]["ALAMAT2"].ToString();
                                    noBPK = dt.Tables[1].Rows[i]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[i]["SEQ"].ToString() + ")";
                                    alamat3 = dt.Tables[1].Rows[i]["ALAMAT3"].ToString();
                                    tgglBPK = Convert.ToDateTime(dt.Tables[1].Rows[i]["TANGGALBPK"]).ToString("dd-MMM-yyyy");
                                    telp = dt.Tables[1].Rows[i]["TELP"].ToString();
                                    DONo = dt.Tables[1].Rows[i]["DONO"].ToString();
                                    SONo = dt.Tables[1].Rows[i]["SONO"].ToString();

                                    if (loopData == 1)
                                    {
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("  No. Rangka  ", 15, ' ');
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("  No. Mesin  ", 14, ' ');
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail(" Buku Service ", 15, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("-", 46, '-');
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|", 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail(" -", 95, '-', false, true);
                                    }
                                    // Print Line
                                    else if (loopData == 2)
                                    {
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("-", 46, '-');
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|", 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail(" -", 95, '-', false, true);
                                    }
                                    else if (loopData == 3)
                                    {
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|", 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail(" -", 95, '-', false, true);
                                    }
                                    else if (loopData == 4)
                                    {
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|", 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        gtf.SetDataDetail("", 46, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', false, true);
                                        gtf.SetDataDetail(" -", 95, '-', false, true);
                                    }
                                    else if (loopData == 0)
                                    {
                                        gtf.SetDataDetail(" -", 95, '-', false, true);
                                    }

                                    gtf.SetDataDetail("Kendaraan dalam keadaan baik.", 29, ' ');
                                    gtf.SetDataDetail(dt.Tables[1].Rows[i - 1]["CITY"].ToString() + ", " + Convert.ToDateTime(dt.Tables[1].Rows[i - 1]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 61, ' ', false, true, true);
                                    gtf.PrintData(false, false);
                                    gtf.SetDataDetail("Kehilangan, kerusakan setelah diterima bukan", 44, ' ', true);
                                    gtf.SetDataDetailSpace(5);
                                    gtf.SetDataDetail("Diserahkan Oleh :", 17, ' ', true);
                                    gtf.SetDataDetailSpace(11);
                                    gtf.SetDataDetail("Diterima Oleh :", 15, ' ', false, true);
                                    gtf.SetDataDetail("tanggungan kami lagi", 96, ' ', false, true);
                                    gtf.PrintData(false, false);
                                    gtf.SetDataDetail("[ ] Lengkap", 96, ' ', false, true);
                                    gtf.PrintData(false, false);
                                    gtf.SetDataDetail("[ ] Tidak Lengkap", 96, ' ', false, true);
                                    gtf.PrintData(false, false);
                                    gtf.SetDataDetail(" _", 21, '_', false, true);
                                    gtf.PrintData(false, false);
                                    gtf.SetDataDetail("|", 1, ' ');
                                    gtf.SetDataDetail("_", 20, '_');
                                    gtf.SetDataDetail("|", 1, ' ');
                                    temp = 55 - (dt.Tables[1].Rows[i - 1]["SignName1"].ToString().Length);
                                    gtf.SetDataDetailSpace(temp - 5);
                                    gtf.SetDataDetail("( " + dt.Tables[1].Rows[i - 1]["SignName1"].ToString() + " )",
                                        dt.Tables[1].Rows[i - 1]["SignName1"].ToString().Length + 5, ' ', true);
                                    if (dt.Tables[1].Rows[i - 1]["SignName2"].ToString() == string.Empty)
                                    {
                                        gtf.SetDataDetail("( ", 2, ' ');
                                        gtf.SetDataDetailSpace(13);
                                        gtf.SetDataDetail(") ", 2, ' ', false, true);
                                    }
                                    else gtf.SetDataDetail("( " + dt.Tables[1].Rows[i - 1]["SignName2"].ToString() + " )", 16, ' ', false, true);
                                    gtf.PrintData(false, false);
                                    gtf.SetDataDetail("", 96, ' ', false, true);
                                    gtf.PrintData(false, false);

                                    counterData = loopData = 0;
                                    indexData = 10;
                                }
                                else
                                {
                                    if (SEQ != dt.Tables[1].Rows[i]["SEQ"].ToString())
                                    {
                                        if (counterData > 5)
                                        {
                                            // Print Header No Rangka, No Mesin, Buku Service
                                            if (loopData == 1)
                                            {
                                                gtf.SetDataDetail("|", 1, ' ');
                                                gtf.SetDataDetail("  No. Rangka  ", 15, ' ');
                                                gtf.SetDataDetail("|", 1, ' ');
                                                gtf.SetDataDetail("  No. Mesin  ", 14, ' ');
                                                gtf.SetDataDetail("|", 1, ' ');
                                                gtf.SetDataDetail(" Buku Service ", 15, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail("|", 1, ' ');
                                                gtf.SetDataDetail("-", 46, '-');
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|", 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail(" -", 95, '-', false, true);
                                            }
                                            // Print Line
                                            else if (loopData == 2)
                                            {
                                                gtf.SetDataDetail("|", 1, ' ');
                                                gtf.SetDataDetail("-", 46, '-');
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|", 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail(" -", 95, '-', false, true);
                                            }
                                            else if (loopData == 3)
                                            {
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i - 1]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|", 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail(" -", 95, '-', false, true);
                                            }
                                            else if (loopData == 4)
                                            {
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["RANGKA2"].ToString(), 15, ' ', true);
                                                gtf.SetDataDetail("| " + dt.Tables[1].Rows[i - 1]["MESIN2"].ToString(), 14, ' ', true);
                                                gtf.SetDataDetail("|", 15, ' ', true);
                                                gtf.SetDataDetail("|", 1, ' ', true);
                                                gtf.SetDataDetail("", 46, ' ');
                                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                                gtf.SetDataDetail(" -", 95, '-', false, true);
                                            }
                                        }
                                        else
                                        {
                                            gtf.SetDataDetail(" -", 95, '-', false, true);
                                        }

                                        gtf.SetDataDetail("Kendaraan dalam keadaan baik.", 29, ' ');
                                        gtf.SetDataDetail(dt.Tables[1].Rows[i - 1]["CITY"].ToString() + ", " + Convert.ToDateTime(dt.Tables[1].Rows[i - 1]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 61, ' ', false, true, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("Kehilangan, kerusakan setelah diterima bukan", 44, ' ', true);
                                        gtf.SetDataDetailSpace(5);
                                        gtf.SetDataDetail("Diserahkan Oleh :", 17, ' ', true);
                                        gtf.SetDataDetailSpace(11);
                                        gtf.SetDataDetail("Diterima Oleh :", 15, ' ', false, true);
                                        gtf.SetDataDetail("tanggungan kami lagi", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("[ ] Lengkap", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("[ ] Tidak Lengkap", 96, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail(" _", 21, '_', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("_", 20, '_');
                                        gtf.SetDataDetail("|", 1, ' ');
                                        temp = 55 - (dt.Tables[1].Rows[i - 1]["SignName1"].ToString().Length);
                                        gtf.SetDataDetailSpace(temp - 5);
                                        gtf.SetDataDetail("( " + dt.Tables[1].Rows[i - 1]["SignName1"].ToString() + " )",
                                            dt.Tables[1].Rows[i - 1]["SignName1"].ToString().Length + 5, ' ', true);
                                        if (dt.Tables[1].Rows[i - 1]["SignName2"].ToString() == string.Empty)
                                        {
                                            gtf.SetDataDetail("( ", 2, ' ');
                                            gtf.SetDataDetailSpace(13);
                                            gtf.SetDataDetail(") ", 2, ' ', false, true);
                                        }
                                        else gtf.SetDataDetail("( " + dt.Tables[1].Rows[i - 1]["SignName2"].ToString() + " )", 16, ' ', false, true);
                                        gtf.PrintData(false, false);
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData(false, false);

                                        counterData = loopData = 0;
                                        SEQ = dt.Tables[1].Rows[i]["SEQ"].ToString();

                                        gtf.ReplaceGroupHdr(tahun, dt.Tables[1].Rows[i]["TAHUN"].ToString());
                                        gtf.ReplaceGroupHdr(salesModelDesc, dt.Tables[1].Rows[i]["SalesModelDesc"].ToString(), 27);
                                        gtf.ReplaceGroupHdr(noBPK, dt.Tables[1].Rows[i]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[i]["SEQ"].ToString() + ")");

                                        tahun = dt.Tables[1].Rows[i]["TAHUN"].ToString();
                                        salesModelDesc = dt.Tables[1].Rows[i]["SalesModelDesc"].ToString();
                                        noBPK = dt.Tables[1].Rows[i]["NOBPK"].ToString() + "(" + dt.Tables[1].Rows[i]["SEQ"].ToString() + ")";
                                    }
                                }
                            }

                            if (counterData == 0)
                            {
                                gtf.SetDataDetail("|", 1, ' ', true);
                                gtf.SetDataDetailSpace(2);
                                gtf.SetDataDetail("Warna", 12, ' ', true);
                                gtf.SetDataDetail(":", 1, ' ', true);
                                gtf.SetDataDetail(dt.Tables[1].Rows[i]["WARNA"].ToString(), 27, ' ', true);
                                gtf.SetDataDetail("|", 1, ' ', true);
                            }
                            else if (counterData == 1)
                            {
                                gtf.SetDataDetail("|", 1, ' ', true);
                                gtf.SetDataDetailSpace(2);
                                gtf.SetDataDetail("STUJ No.", 12, ' ', true);
                                gtf.SetDataDetail(":", 1, ' ', true);
                                gtf.SetDataDetail("", 27, ' ', true);
                                gtf.SetDataDetail("|", 1, ' ', true);
                            }
                            else if (counterData == 2)
                            {
                                gtf.SetDataDetail("|", 1, ' ', true);
                                gtf.SetDataDetailSpace(2);
                                gtf.SetDataDetail("Kunci Pintu", 12, ' ', true);
                                gtf.SetDataDetail(":", 1, ' ', true);
                                gtf.SetDataDetail("", 27, ' ', true);
                                gtf.SetDataDetail("|", 1, ' ', true);
                            }
                            else if (counterData == 3)
                            {
                                gtf.SetDataDetail("|", 1, ' ', true);
                                gtf.SetDataDetailSpace(2);
                                gtf.SetDataDetail("Kunci Kontak", 12, ' ', true);
                                gtf.SetDataDetail(":", 1, ' ', true);
                                if (setTextParameter[2].ToString() == "4W")
                                    gtf.SetDataDetail(dt.Tables[0].Rows[0]["NOKUNCI"].ToString(), 27, ' ', true);
                                else
                                    gtf.SetDataDetail("", 27, ' ', true);
                                gtf.SetDataDetail("|", 1, ' ', true);
                            }
                            else if (counterData == 4)
                            {
                                gtf.SetDataDetail("|", 1, ' ');
                                gtf.SetDataDetailSpace(46);
                                gtf.SetDataDetail("|", 1, ' ', true);
                            }
                            else if (counterData == 5)
                            {
                                // Print line 
                                gtf.SetDataDetail("|", 1, ' ');
                                gtf.SetDataDetail("-", 46, '-');
                                gtf.SetDataDetail("|", 1, ' ', true);
                                loopData += 1;
                            }
                            else
                            {
                                if (counterData > 5 && counterData < 10)
                                {
                                    // Print Header No Rangka, No Mesin, Buku Service
                                    if (loopData == 1)
                                    {
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("  No. Rangka  ", 15, ' ');
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("  No. Mesin  ", 14, ' ');
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail(" Buku Service ", 15, ' ');
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        loopData += 1;
                                    }
                                    // Print Line
                                    else if (loopData == 2)
                                    {
                                        gtf.SetDataDetail("|", 1, ' ');
                                        gtf.SetDataDetail("-", 46, '-');
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        loopData += 1;
                                    }
                                    else if (loopData == 3)
                                    {
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i]["RANGKA"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i]["MESIN"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|  " + dt.Tables[1].Rows[i]["BUKUSERVICE"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                        loopData += 1;
                                    }
                                    else if (loopData == 4)
                                    {
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i]["RANGKA2"].ToString(), 15, ' ', true);
                                        gtf.SetDataDetail("| " + dt.Tables[1].Rows[i]["MESIN2"].ToString(), 14, ' ', true);
                                        gtf.SetDataDetail("|", 15, ' ', true);
                                        gtf.SetDataDetail("|", 1, ' ', true);
                                    }
                                }
                                else
                                {
                                    loopData = 0;
                                    //gtf.SetDataDetail(" -", 95, '-', false, true);
                                    //gtf.SetDataDetail("|", 1, ' ');
                                    //gtf.SetDataDetailSpace(46);
                                    //gtf.SetDataDetail("|", 1, ' ', true);
                                }
                            }

                            if (counterData < 10)
                            {
                                gtf.SetDataDetail("[ ] " + dt.Tables[1].Rows[i]["NamaPerlengkapan"].ToString(), 21, ' ', true);
                                gtf.SetDataDetail(dt.Tables[1].Rows[i]["Quantity"].ToString(), 2, ' ', true);
                                DataRow[] dRowBPK = dt.Tables[1].Select(string.Format("NOBPK = '{0}'", NoBPK));
                                if (dRowBPK.Length > 10 && indexData < dRowBPK.Length)
                                {
                                    gtf.SetDataDetail("[ ] " + dRowBPK[indexData]["NamaPerlengkapan"].ToString(), 17, ' ', true);
                                    gtf.SetDataDetail(dRowBPK[indexData]["Quantity"].ToString(), 2, ' ', true);
                                    indexData += 1;
                                }
                                else
                                {
                                    gtf.SetDataDetailSpace(21);
                                }
                                gtf.SetDataDetail("|", 1, ' ', false, true);
                                gtf.PrintData(false, false);
                                counterData += 1;
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region OmRpSalesTrn003B/OmRpSalesTrn003C (1 HAL / 1/2 HAL)
                if (recordId == "OmRpSalesTrn003B" || recordId == "OmRpSalesTrn003C")
                {
                    custName = dt.Tables[0].Rows[0]["CustomerName"].ToString();
                    gudang = dt.Tables[0].Rows[0]["GUDANG"].ToString();
                    alamat1 = dt.Tables[0].Rows[0]["ALAMAT1"].ToString();
                    alamat2 = dt.Tables[0].Rows[0]["ALAMAT2"].ToString();
                    noBPK = dt.Tables[0].Rows[0]["NOMORBPK"].ToString() + "(" + dt.Tables[0].Rows[0]["SEQ"].ToString() + ")";
                    tgglBPK = Convert.ToDateTime(dt.Tables[0].Rows[0]["TANGGALBPK"]).ToString("dd-MMM-yyyy");
                    telp = "Telp : " + dt.Tables[0].Rows[0]["TELP"].ToString();
                    DONo = dt.Tables[0].Rows[0]["DONO"].ToString();
                    SONo = dt.Tables[0].Rows[0]["SONO"].ToString();

                    gtf.SetGroupHeader("Kepada Yth :", 12, ' ', true);
                    gtf.SetGroupHeader(custName, 51, ' ', true);
                    gtf.SetGroupHeader("Gudang    :", 11, ' ', true);
                    gtf.SetGroupHeader(gudang, 19, ' ', false, true);
                    gtf.SetGroupHeaderSpace(13);
                    gtf.SetGroupHeader(alamat1, 63, ' ', true);
                    gtf.SetGroupHeader("", 19, ' ', false, true);
                    gtf.SetGroupHeaderSpace(13);
                    gtf.SetGroupHeader(alamat2, 51, ' ', true);
                    gtf.SetGroupHeader("Nomor     :", 11, ' ', true);
                    gtf.SetGroupHeader(noBPK, 19, ' ', false, true);
                    gtf.SetGroupHeaderSpace(13);
                    gtf.SetGroupHeader(telp, 51, ' ', true);
                    gtf.SetGroupHeader("Tanggal   :", 11, ' ', true);
                    gtf.SetGroupHeader(tgglBPK, 19, ' ', false, true);
                    gtf.SetGroupHeaderSpace(65);
                    gtf.SetGroupHeader(telp, 51, ' ', true);
                    gtf.SetGroupHeader("Nomor DO  :", 11, ' ', true);
                    gtf.SetGroupHeader(DONo, 19, ' ', false, true);
                    gtf.SetGroupHeaderSpace(65);
                    gtf.SetGroupHeader("Nomor SO  :", 11, ' ', true);
                    gtf.SetGroupHeader(SONo, 19, ' ', false, true);
                    gtf.SetGroupHeader("Dengan ini kami serahkan unit kendaraan sebagai berikut :", 96, ' ', false, true);

                    gtf.SetGroupHeaderLine();
                    gtf.SetGroupHeader("NO.", 3, ' ', true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("MODEL", 11, ' ', true);
                    gtf.SetGroupHeader("SEQ", 3, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("WARNA", 10, ' ', true);
                    gtf.SetGroupHeader("NO RANGKA", 10, ' ', true);
                    gtf.SetGroupHeader("NO MESIN", 9, ' ', true);
                    gtf.SetGroupHeader("TAHUN", 5, ' ', true, false, true);
                    gtf.SetGroupHeaderSpace(1);
                    gtf.SetGroupHeader("BUKU SERVICE", 13, ' ', true);
                    gtf.SetGroupHeader("NO. KUNCI", 10, ' ', true);
                    gtf.SetGroupHeader("NO. POLISI", 10, ' ', false, true);
                    gtf.SetGroupHeaderLine();
                    gtf.PrintData();

                    gtf.PrintHeader();
                    for (int i = 0; i <= dt.Tables[0].Rows.Count; i++)
                    {
                        if (i == dt.Tables[0].Rows.Count)
                        {
                            if (recordId == "OmRpSalesTrn003B")
                            {
                                loopData = 59 - (gtf.line + 14);
                                for (int k = 0; k < loopData; k++)
                                {
                                    gtf.SetDataDetail("", 96, ' ', false, true);
                                    gtf.PrintData();
                                }
                            }

                            gtf.SetDataReportLine();
                            gtf.PrintData();
                            gtf.SetDataDetail("Total : " + counterData.ToString() + " Unit", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail(dt.Tables[0].Rows[i - 1]["CITY"].ToString() + ", " + Convert.ToDateTime(dt.Tables[0].Rows[i - 1]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                            gtf.PrintData();
                            if (recordId == "OmRpSalesTrn003B")
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                            }
                            gtf.SetDataDetail("* Kendaraan dalam keadaan baik, kehilangan, kerusakan", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("  setelah diterima, bukan menjadi tanggungan kami lagi", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            gtf.SetDataDetailSpace(40);
                            gtf.SetDataDetail("Diserahkan Oleh :", 18, ' ', true);
                            gtf.SetDataDetailSpace(6);
                            gtf.SetDataDetail("Satpam", 6, ' ');
                            gtf.SetDataDetailSpace(6);
                            gtf.SetDataDetail("Diterima Oleh :", 18, ' ', false, true);
                            gtf.PrintData();

                            int loop = 0;
                            if (recordId == "OmRpSalesTrn003B") loop = 4;
                            else loop = 2;

                            for (int j = 0; j < loop; j++)
                            {
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                            }
                            gtf.SetDataDetailSpace(40);
                            gtf.SetDataDetail("-", 18, '-', true);
                            gtf.SetDataDetail("-", 18, '-', true);
                            gtf.SetDataDetail("-", 18, '-', false, true);
                            gtf.PrintData();

                            gtf.SetDataDetail("", 96, ' ', false, true);
                            gtf.PrintData();
                            break;
                        }

                        if (NoBPK == string.Empty)
                        {
                            NoBPK = dt.Tables[0].Rows[i]["NOMORBPK"].ToString();
                        }
                        else
                        {
                            if (NoBPK != dt.Tables[0].Rows[i]["NOMORBPK"].ToString())
                            {
                                if (recordId == "OmRpSalesTrn003B")
                                {
                                    loopData = 59 - (gtf.line + 14);
                                    for (int k = 0; k < loopData; k++)
                                    {
                                        gtf.SetDataDetail("", 96, ' ', false, true);
                                        gtf.PrintData();
                                    }
                                }

                                gtf.SetDataReportLine();
                                gtf.PrintData();
                                gtf.SetDataDetail("Total : " + counterData.ToString() + " Unit", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail(dt.Tables[0].Rows[i - 1]["CITY"].ToString() + ", " + Convert.ToDateTime(dt.Tables[0].Rows[i - 1]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 96, ' ', false, true, true);
                                gtf.PrintData();
                                if (recordId == "OmRpSalesTrn003B")
                                {
                                    gtf.SetDataDetail("", 96, ' ', false, true);
                                    gtf.PrintData();
                                }
                                gtf.SetDataDetail("* Kendaraan dalam keadaan baik, kehilangan, kerusakan", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("  setelah diterima, bukan menjadi tanggungan kami lagi", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();
                                gtf.SetDataDetailSpace(40);
                                gtf.SetDataDetail("Diserahkan Oleh :", 18, ' ', true);
                                gtf.SetDataDetailSpace(6);
                                gtf.SetDataDetail("Satpam", 6, ' ');
                                gtf.SetDataDetailSpace(6);
                                gtf.SetDataDetail("Diterima Oleh :", 18, ' ', false, true);
                                gtf.PrintData();

                                int loop = 0;
                                if (recordId == "OmRpSalesTrn003B") loop = 4;
                                else loop = 2;

                                for (int j = 0; j < loop; j++)
                                {
                                    gtf.SetDataDetail("", 96, ' ', false, true);
                                    gtf.PrintData();
                                }
                                gtf.SetDataDetailSpace(40);
                                gtf.SetDataDetail("-", 18, '-', true);
                                gtf.SetDataDetail("-", 18, '-', true);
                                gtf.SetDataDetail("-", 18, '-', false, true);
                                gtf.PrintData();

                                gtf.SetDataDetail("", 96, ' ', false, true);
                                gtf.PrintData();

                                counterData = 0;
                                NoBPK = dt.Tables[0].Rows[i]["NOMORBPK"].ToString();

                                gtf.ReplaceGroupHdr(custName, dt.Tables[0].Rows[i]["CustomerName"].ToString(), 51);
                                gtf.ReplaceGroupHdr(gudang, dt.Tables[0].Rows[i]["GUDANG"].ToString(), 19);
                                gtf.ReplaceGroupHdr(alamat1, dt.Tables[0].Rows[i]["ALAMAT1"].ToString(), 63);
                                gtf.ReplaceGroupHdr(alamat2, dt.Tables[0].Rows[i]["ALAMAT2"].ToString(), 51);
                                gtf.ReplaceGroupHdr(noBPK, dt.Tables[0].Rows[i]["NOMORBPK"].ToString() + "(" + dt.Tables[0].Rows[i]["SEQ"].ToString() + ")", 19);
                                gtf.ReplaceGroupHdr(tgglBPK, Convert.ToDateTime(dt.Tables[0].Rows[i]["TANGGALBPK"]).ToString("dd-MMM-yyyy"), 19);
                                gtf.ReplaceGroupHdr(telp, "Telp : " + dt.Tables[0].Rows[i]["TELP"].ToString(), 51);
                                gtf.ReplaceGroupHdr(DONo, dt.Tables[0].Rows[i]["DONO"].ToString(), 19);
                                gtf.ReplaceGroupHdr(SONo, dt.Tables[0].Rows[i]["SONO"].ToString(), 19);

                                custName = dt.Tables[0].Rows[i]["CustomerName"].ToString();
                                gudang = dt.Tables[0].Rows[i]["GUDANG"].ToString();
                                alamat1 = dt.Tables[0].Rows[i]["ALAMAT1"].ToString();
                                alamat2 = dt.Tables[0].Rows[i]["ALAMAT2"].ToString();
                                noBPK = dt.Tables[0].Rows[i]["NOMORBPK"].ToString() + "(" + dt.Tables[0].Rows[i]["SEQ"].ToString() + ")";
                                tgglBPK = Convert.ToDateTime(dt.Tables[0].Rows[i]["TANGGALBPK"]).ToString("dd-MMM-yyyy");
                                telp = "Telp : " + dt.Tables[0].Rows[i]["TELP"].ToString();
                                DONo = dt.Tables[0].Rows[i]["DONO"].ToString();
                                SONo = dt.Tables[0].Rows[i]["SONO"].ToString();
                            }
                        }
                        counterData += 1;
                        gtf.SetDataDetail(counterData.ToString(), 3, ' ', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["MODEL"].ToString(), 11, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SEQ"].ToString(), 3, ' ', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["WARNA"].ToString(), 10, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["RANGKA"].ToString(), 10, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["MESIN"].ToString(), 9, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["TAHUN"].ToString(), 5, ' ', true, false, true);
                        gtf.SetDataDetailSpace(1);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["BUKUSERVICE"].ToString(), 13, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["NOKUNCI"].ToString(), 10, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["NOPOLISI"].ToString(), 10, ' ', false, true);

                        gtf.PrintData();
                    }
                }
                #endregion
            }
            #endregion


            return gtf.sbDataTxt.ToString();
        }

    }
}