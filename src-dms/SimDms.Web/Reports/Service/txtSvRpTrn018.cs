using SimDms.Common;
using SimDms.Service.Models.Reports;
using SimDms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace SimDms.Web.Reports.Service
{
    public class txtSvRpTrn018 : IRptProc
    {
        private SimDms.Service.Models.DataContext ctx = new SimDms.Service.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public SysUser CurrentUser { get; set; }
        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            //var dt = ctx.Database.SqlQuery<SvRpTrn001>(string.Format("exec {0} {1}", sproc, sparam));            
            //return CreateReportSvRpTrn018("SvRpTrn001", dt.ToList(), "", printerloc, print, "", fullpage);
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "exec " + sproc + " " + sparam;
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return CreateReportSvRpTrn018(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
        }

        private string CreateReportSvRpTrn018(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        { 
            string[] arrayHeader = new string[11];

            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header

            arrayHeader[0] = dt.Rows[0]["RefferenceNo"].ToString();
            arrayHeader[1] = dt.Rows[0]["ReturnNo"].ToString();
            arrayHeader[2] = dt.Rows[0]["RefferenceDate"] != null ? Convert.ToDateTime(dt.Rows[0]["RefferenceDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[3] = dt.Rows[0]["ReturnDate"] != null ? Convert.ToDateTime(dt.Rows[0]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[4] = dt.Rows[0]["CustName"].ToString() != "" ? dt.Rows[0]["CustName"].ToString() : string.Empty;
            arrayHeader[5] = dt.Rows[0]["CustAddr1"].ToString() != "" ? dt.Rows[0]["CustAddr1"].ToString() : string.Empty;
            arrayHeader[6] = dt.Rows[0]["CustAddr2"].ToString() != "" ? dt.Rows[0]["CustAddr2"].ToString() : string.Empty;
            arrayHeader[7] = dt.Rows[0]["CustPhone"].ToString() != "" ? dt.Rows[0]["CustPhone"].ToString() : string.Empty;
            arrayHeader[8] = dt.Rows[0]["FPJNo"].ToString() != "" ? "UNTUK MENGEMBALIKAN PERAWATAN BENGKEL, DARI FAKTUR PENJUALAN NOMOR : " + dt.Rows[0]["FPJNo"].ToString() + "," : string.Empty;
            arrayHeader[9] = (dt.Rows[0]["FPJDate"].ToString() != "" && dt.Rows[0]["CompanyName"].ToString() != "") ? "TANGGAL : " + Convert.ToDateTime(dt.Rows[0]["FPJDate"].ToString()).ToString("dd-MMM-yyyy") + " PADA " + dt.Rows[0]["CompanyName"].ToString().ToString() : "TANGGAL : ";
            arrayHeader[10] = dt.Rows[0]["CustomerName"].ToString() != "" ? dt.Rows[0]["CustomerName"].ToString() : string.Empty;

            CreateHdrSvRpTrn018(gtf, arrayHeader);
            gtf.PrintHeader();

            string docNo = "", city = "";
            int noUrut = 0;
            string[] nominal;
            string[] remark;
            bool statusSp = false;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["InvoiceNo"].ToString();

                    if (dt.Rows[i]["dataType"].ToString() == "JASA PERBAIKAN")
                    {
                        if (dt.Rows[i]["partName"].ToString() == "")
                            gtf.SetDataDetailLineBreak();
                        else if (dt.Rows[i]["partName"].ToString() == "JASA PERBAIKAN")
                            gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 96, ' ', false, true);
                        else
                        {
                            noUrut++;
                            gtf.SetDataDetail(noUrut.ToString(), 3, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 75, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["totalPrice"].ToString(), 18, ' ', false, true, true, true, "n0");
                        }
                    }
                    else if (dt.Rows[i]["dataType"].ToString() == "PEMAKAIAN SPAREPART / MATERIAL")
                    {
                        if (dt.Rows[i]["partName"].ToString() == "")
                        {
                            gtf.SetDataDetailLineBreak();
                            noUrut = 0;
                        }
                        else if (dt.Rows[i]["partName"].ToString() == "PEMAKAIAN SPAREPART / MATERIAL")
                            gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 96, ' ', false, true);
                        else
                        {
                            noUrut++;
                            gtf.SetDataDetail(noUrut.ToString(), 3, ' ', true, false, true);
                            if (dt.Rows[i]["PartNo"].ToString() != "")
                            {
                                gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 18, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 30, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["supplyQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                                gtf.SetDataDetail(dt.Rows[i]["retailPrice"].ToString(), 16, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(dt.Rows[i]["totalPrice"].ToString(), 16, ' ', false, true, true, true, "n0");
                            }
                            else
                            {
                                gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 92, ' ', false, true);
                                statusSp = true;
                            }
                        }
                    }
                    gtf.PrintData(false);
                }
                else if (docNo != dt.Rows[i]["InvoiceNo"].ToString())
                {
                    gtf.SetTotalDetailSpace(78);
                    gtf.SetTotalDetail("-", 18, '-', false, true);
                    gtf.SetTotalDetailSpace(78);
                    if (statusSp == false)
                        gtf.SetTotalDetail(dt.Rows[i - 1]["TotalDtlAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
                    else
                        gtf.SetTotalDetail("0", 18, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail("Potongan", 10, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    if (statusSp == false)
                        gtf.SetTotalDetail(dt.Rows[i - 1]["DiscAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
                    else
                        gtf.SetTotalDetail("0", 18, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetail("-", 96, '-', false, true);
                    gtf.SetTotalDetail("Terbilang :", 11, ' ', true);

                    nominal = gtf.ConvertArrayTerbilang(gtf.Terbilang(Convert.ToInt64(dt.Rows[i - 1]["TotalSrvAmt"].ToString())), 53);
                    if (statusSp == false)
                        gtf.SetTotalDetail(nominal[0] != null ? nominal[0].ToString().ToUpper() : "", 53, ' ', true);
                    else
                        gtf.SetTotalDetail("", 53, ' ', true);
                    gtf.SetTotalDetail("Total", 10, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    if (statusSp == false)
                        gtf.SetTotalDetail(dt.Rows[i - 1]["TotalDppAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
                    else
                        gtf.SetTotalDetail(" ", 18, ' ', false, true, true, true, "n0");

                    gtf.SetTotalDetailSpace(12);
                    if (statusSp == false)
                        gtf.SetTotalDetail(nominal[1] != null ? nominal[1].ToString().ToUpper() : "", 53, ' ', true);
                    else
                        gtf.SetTotalDetail("", 53, ' ', true);

                    gtf.SetTotalDetail("PPN " + dt.Rows[i - 1]["TaxPct"].ToString() + "%", 10, ' ');
                    gtf.SetTotalDetail(":", 1, ' ', true);
                    if (statusSp == false)
                        gtf.SetTotalDetail(dt.Rows[i - 1]["TaxAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
                    else
                        gtf.SetTotalDetail("", 18, ' ', false, true, true, true, "n0");

                    gtf.SetTotalDetailSpace(78);
                    gtf.SetTotalDetail("-", 18, '-', false, true);
                    gtf.SetTotalDetailSpace(78);
                    if (statusSp == false)
                        gtf.SetTotalDetail(dt.Rows[i - 1]["TotalSrvAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
                    else
                        gtf.SetTotalDetail(" ", 18, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetail("-", 96, '-', false, true);

                    remark = gtf.ConvertArrayText(dt.Rows[i - 1]["Remark"].ToString(), 46);

                    if (dt.Rows[i - 1]["City"].ToString() != "")
                        city = string.Format("{0}, {1}", dt.Rows[i - 1]["City"] != null ? dt.Rows[i - 1]["City"].ToString() : "", DateTime.Now.ToString("dd-MMM-yyyy").ToUpper());

                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail(city.ToString(), 30, ' ', false, true);
                    gtf.SetTotalDetail("KETERANGAN", 66, ' ', false, true);
                    gtf.SetTotalDetail(dt.Rows[i - 1]["totRemarks"].ToString(), 66, ' ', false, true);
                    if (fullPage == true)
                        gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail(setTextParameter[0].ToString(), 25, ' ', false, true);
                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail("-", 25, '-', false, true);
                    gtf.SetTotalDetailSpace(66);
                    gtf.SetTotalDetail(setTextParameter[1].ToString(), 25, ' ', false, true);
                    gtf.PrintTotal(false, false, false);

                    arrayHeader[0] = dt.Rows[i]["RefferenceNo"].ToString();
                    arrayHeader[1] = dt.Rows[i]["ReturnNo"].ToString();
                    arrayHeader[2] = dt.Rows[i]["RefferenceDate"] != null ? Convert.ToDateTime(dt.Rows[i]["RefferenceDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[3] = dt.Rows[i]["ReturnDate"] != null ? Convert.ToDateTime(dt.Rows[i]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[4] = dt.Rows[i]["CustName"].ToString() != "" ? dt.Rows[i]["CustName"].ToString() : string.Empty;
                    arrayHeader[5] = dt.Rows[i]["CustAddr1"].ToString() != "" ? dt.Rows[i]["CustAddr1"].ToString() : string.Empty;
                    arrayHeader[6] = dt.Rows[i]["CustAddr2"].ToString() != "" ? dt.Rows[i]["CustAddr2"].ToString() : string.Empty;
                    arrayHeader[7] = dt.Rows[i]["CustPhone"].ToString() != "" ? dt.Rows[i]["CustPhone"].ToString() : string.Empty;
                    arrayHeader[8] = dt.Rows[i]["FPJNo"].ToString() != "" ? "UNTUK MENGEMBALIKAN PERAWATAN BENGKEL, DARI FAKTUR PENJUALAN NOMOR : " + dt.Rows[i]["FPJNo"].ToString() + "," : string.Empty;
                    arrayHeader[9] = (dt.Rows[i]["FPJDate"].ToString() != "" && dt.Rows[i]["CompanyName"].ToString() != "") ? "TANGGAL : " + Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy") + " PADA " + dt.Rows[i]["CompanyName"].ToString().ToString() : "TANGGAL : ";
                    arrayHeader[10] = dt.Rows[i]["CustomerName"].ToString() != "" ? dt.Rows[i]["CustomerName"].ToString() : string.Empty;

                    CreateHdrSvRpTrn018(gtf, arrayHeader);
                    gtf.PrintAfterBreak();

                    if (dt.Rows[i]["dataType"].ToString() == "JASA PERBAIKAN")
                    {
                        if (dt.Rows[i]["partName"].ToString() == "")
                            gtf.SetDataDetailLineBreak();
                        else if (dt.Rows[i]["partName"].ToString() == "JASA PERBAIKAN")
                            gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 96, ' ', false, true);
                        else
                        {
                            noUrut++;
                            gtf.SetDataDetail(noUrut.ToString(), 3, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 75, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["totalPrice"].ToString(), 18, ' ', false, true, true, true, "n0");
                        }
                        gtf.PrintData(false);
                    }
                    else if (dt.Rows[i]["dataType"].ToString() == "PEMAKAIAN SPAREPART / MATERIAL")
                    {
                        if (dt.Rows[i]["partName"].ToString() == "")
                        {
                            gtf.SetDataDetailLineBreak();
                            noUrut = 0;
                        }
                        else if (dt.Rows[i]["partName"].ToString() == "PEMAKAIAN SPAREPART / MATERIAL")
                            gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 96, ' ', false, true);
                        else
                        {
                            noUrut++;
                            gtf.SetDataDetail(noUrut.ToString(), 3, ' ', true, false, true);
                            if (dt.Rows[i]["PartNo"].ToString() != "")
                            {
                                gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 18, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 30, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["supplyQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                                gtf.SetDataDetail(dt.Rows[i]["retailPrice"].ToString(), 16, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(dt.Rows[i]["totalPrice"].ToString(), 16, ' ', false, true, true, true, "n0");
                            }
                            else
                            {
                                gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 92, ' ', false, true);
                                statusSp = true;
                            }
                        }
                    }
                    gtf.PrintData(false);

                }
                else
                {
                    if (dt.Rows[i]["dataType"].ToString() == "JASA PERBAIKAN")
                    {
                        if (dt.Rows[i]["partName"].ToString() == "")
                            gtf.SetDataDetailLineBreak();
                        else if (dt.Rows[i]["partName"].ToString() == "JASA PERBAIKAN")
                            gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 96, ' ', false, true);
                        else
                        {
                            noUrut++;
                            gtf.SetDataDetail(noUrut.ToString(), 3, ' ', true, false, true);
                            gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 75, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["totalPrice"].ToString(), 16, ' ', false, true, true, true, "n0");
                        }
                        gtf.PrintData(false);
                    }
                    else if (dt.Rows[i]["dataType"].ToString() == "PEMAKAIAN SPAREPART / MATERIAL")
                    {
                        if (dt.Rows[i]["partName"].ToString() == "")
                        {
                            gtf.SetDataDetailLineBreak();
                            noUrut = 0;
                        }
                        else if (dt.Rows[i]["partName"].ToString() == "PEMAKAIAN SPAREPART / MATERIAL")
                            gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 96, ' ', false, true);
                        else
                        {
                            noUrut++;
                            gtf.SetDataDetail(noUrut.ToString(), 3, ' ', true, false, true);
                            if (dt.Rows[i]["PartNo"].ToString() != "")
                            {
                                gtf.SetDataDetail(dt.Rows[i]["PartNo"].ToString(), 18, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 30, ' ', true);
                                gtf.SetDataDetail(dt.Rows[i]["supplyQty"].ToString(), 8, ' ', true, false, true, true, "n2");
                                gtf.SetDataDetail(dt.Rows[i]["retailPrice"].ToString(), 16, ' ', true, false, true, true, "n0");
                                gtf.SetDataDetail(dt.Rows[i]["totalPrice"].ToString(), 16, ' ', false, true, true, true, "n0");
                            }
                            else
                            {
                                gtf.SetDataDetail(dt.Rows[i]["PartName"].ToString(), 92, ' ', false, true);
                                statusSp = true;
                            }
                        }
                        gtf.PrintData(false);
                    }
                }
            }
            gtf.SetTotalDetailSpace(78);
            gtf.SetTotalDetail("-", 18, '-', false, true);
            gtf.SetTotalDetailSpace(78);
            if (statusSp == false)
                gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotalDtlAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
            else
                gtf.SetTotalDetail("0", 18, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail("Potongan", 10, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            if (statusSp == false)
                gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["DiscAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
            else
                gtf.SetTotalDetail("0", 18, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetail("-", 96, '-', false, true);
            gtf.SetTotalDetail("Terbilang :", 11, ' ', true);

            nominal = gtf.ConvertArrayTerbilang(gtf.Terbilang(Convert.ToInt64(dt.Rows[dt.Rows.Count - 1]["TotalSrvAmt"].ToString())), 53);
            if (statusSp == false)
                gtf.SetTotalDetail(nominal[0] != null ? nominal[0].ToString().ToUpper() : "", 53, ' ', true);
            else
                gtf.SetTotalDetail("", 53, ' ', true);
            gtf.SetTotalDetail("Total", 10, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            if (statusSp == false)
                gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotalDppAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
            else
                gtf.SetTotalDetail(" ", 18, ' ', false, true, true, true, "n0");

            gtf.SetTotalDetailSpace(12);
            if (statusSp == false)
                gtf.SetTotalDetail(nominal[1] != null ? nominal[1].ToString().ToUpper() : "", 53, ' ', true);
            else
                gtf.SetTotalDetail("", 53, ' ', true);
            gtf.SetTotalDetail("PPN " + dt.Rows[dt.Rows.Count - 1]["TaxPct"].ToString() + "%", 10, ' ');
            gtf.SetTotalDetail(":", 1, ' ', true);
            if (statusSp == false)
                gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TaxAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
            else
                gtf.SetTotalDetail("", 18, ' ', false, true, true, true, "n0");

            gtf.SetTotalDetailSpace(78);
            gtf.SetTotalDetail("-", 18, '-', false, true);
            gtf.SetTotalDetailSpace(78);
            if (statusSp == false)
                gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["TotalSrvAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
            else
                gtf.SetTotalDetail(" ", 18, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetail("-", 96, '-', false, true);

            remark = gtf.ConvertArrayText(dt.Rows[dt.Rows.Count - 1]["Remark"].ToString(), 46);

            if (dt.Rows[dt.Rows.Count - 1]["City"].ToString() != "")
                city = string.Format("{0}, {1}", dt.Rows[dt.Rows.Count - 1]["City"] != null ? dt.Rows[dt.Rows.Count - 1]["City"].ToString() : "", DateTime.Now.ToString("dd-MMM-yyyy").ToUpper());

            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail(city.ToString(), 30, ' ', false, true);
            gtf.SetTotalDetail("KETERANGAN", 66, ' ', false, true);
            gtf.SetTotalDetail(dt.Rows[dt.Rows.Count - 1]["totRemarks"].ToString(), 66, ' ', false, true);
            if (fullPage == true)
                gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail(setTextParameter[0].ToString(), 25, ' ', false, true);
            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail("-", 25, '-', false, true);
            gtf.SetTotalDetailSpace(66);
            gtf.SetTotalDetail(setTextParameter[1].ToString(), 25, ' ', false, true);

            if (print == true)
                gtf.PrintTotal(true, false, false);
            else
            {
                //if (gtf.PrintTotal(true, false, false) == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private void CreateHdrSvRpTrn018(GenerateTextFileReport gtf, string[] arrayHeader)
        {
            //arrayHeader[0] = dt.Rows[i]["RefferenceNo"].ToString();
            //arrayHeader[1] = dt.Rows[i]["ReturnNo"].ToString();
            //arrayHeader[2] = dt.Rows[i]["RefferenceDate"] != null ? Convert.ToDateTime(dt.Rows[i]["RefferenceDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            //arrayHeader[3] = dt.Rows[i]["ReturnDate"] != null ? Convert.ToDateTime(dt.Rows[i]["ReturnDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            //arrayHeader[4] = dt.Rows[i]["CustName"].ToString() != "" ? dt.Rows[i]["CustName"].ToString() : string.Empty;
            //arrayHeader[5] = dt.Rows[i]["CustAddr1"].ToString() != "" ? dt.Rows[i]["CustAddr1"].ToString() : string.Empty;
            //arrayHeader[6] = dt.Rows[i]["CustAddr2"].ToString() != "" ? dt.Rows[i]["CustAddr2"].ToString() : string.Empty;
            //arrayHeader[7] = dt.Rows[i]["CustPhone"].ToString() != "" ? dt.Rows[i]["CustPhone"].ToString() : string.Empty;
            //arrayHeader[8] = dt.Rows[i]["FPJNo"].ToString() != "" ? "UNTUK MENGEMBALIKAN PERAWATAN BENGKEL, DARI FAKTUR PENJUALAN NOMOR : " + dt.Rows[i]["FPJNo"].ToString() + "," : string.Empty;
            //arrayHeader[9] = (dt.Rows[i]["FPJDate"].ToString() != "" && dt.Rows[i]["CompanyName"].ToString() != "") ? "TANGGAL : " + Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy") + " PADA " + dt.Rows[i]["CompanyName"].ToString().ToString() : "TANGGAL : ";
            //arrayHeader[10] = dt.Rows[i]["CustomerName"].ToString() != "" ? dt.Rows[i]["CustomerName"].ToString() : string.Empty;

            gtf.CleanHeader();

            gtf.SetGroupHeader("BERDASARKAN NO PERMOHONAN", 25, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[0].ToString(), 44, ' ', true);
            gtf.SetGroupHeader("NOMOR", 9, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[1].ToString(), 13, ' ', false, true);
            gtf.SetGroupHeader("TGL. PERMOHONAN", 25, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[2].ToString(), 44, ' ', true);
            gtf.SetGroupHeader("TANGGAL", 9, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);

            if (Convert.ToDateTime(arrayHeader[3].ToString()).Year != 1900)
                gtf.SetGroupHeader(arrayHeader[3].ToString(), 13, ' ', false, true);
            else
                gtf.SetGroupHeader("", 13, ' ', false, true);

            gtf.SetGroupHeader(" ", 1, ' ', false, true);
            gtf.SetGroupHeader("DIBERIKAN PERSETUJUAN KEPADA", 29, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[4].ToString(), 65, ' ', false, true);
            gtf.SetGroupHeaderSpace(31);
            gtf.SetGroupHeader(arrayHeader[5].ToString(), 65, ' ', false, true);
            gtf.SetGroupHeaderSpace(31);
            gtf.SetGroupHeader(arrayHeader[6].ToString(), 65, ' ', false, true);
            gtf.SetGroupHeaderSpace(31);
            gtf.SetGroupHeader(arrayHeader[7].ToString(), 65, ' ', false, true);
            gtf.SetGroupHeader(" ", 1, ' ', false, true);
            gtf.SetGroupHeader(arrayHeader[8].ToString(), 96, ' ', false, true);
            gtf.SetGroupHeader(arrayHeader[9].ToString(), 96, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("No.", 3, ' ', true, false, true);
            gtf.SetGroupHeader("KETERANGAN", 49, ' ', true);
            gtf.SetGroupHeader("JUMLAH", 8, ' ', true, false, true);
            gtf.SetGroupHeader("HARGA SATUAN", 16, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 16, ' ', false, true, true);
            gtf.SetGroupHeaderLine();

        }
    }
}