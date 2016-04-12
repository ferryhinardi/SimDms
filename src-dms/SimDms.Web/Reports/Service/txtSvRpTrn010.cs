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
    public class txtSvRpTrn010 : IRptProc
    {
        private SimDms.Service.Models.DataContext ctx = new SimDms.Service.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public SysUser CurrentUser { get; set; }
        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            //var dt = ctx.Database.SqlQuery<SvRpTrn001>(string.Format("exec {0} {1}", sproc, sparam));            
            //return CreateReportSvRpTrn010("SvRpTrn001", dt.ToList(), "", printerloc, print, "", fullpage);
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "exec " + sproc + " " + sparam;
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet dt = new DataSet();
            da.Fill(dt);
            return CreateReportSvRpTrn010(rptId, dt, paramReport, printerloc, print, "", fullpage);
        }

        private string CreateReportSvRpTrn010(string recordId, DataSet dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage)
        {
            string[] arrayHeader = new string[14];
            string[] arrayHeader2 = new string[6];
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W163", print, fileLocation, fullPage);
            gtf.GenerateHeader();
            gtf.SetGroupHeader("Mulai Tanggal Service : " + Convert.ToDateTime(dt.Tables[0].Rows[0]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy"), 163, ' ', false, true, false, true);

            arrayHeader2 = dt.Tables[0].Rows[0]["Remarks"].ToString().Split('\n');

            //----------------------------------------------------------------
            //Header
            arrayHeader[0] = dt.Tables[0].Rows[0]["PoliceRegNo"].ToString();
            arrayHeader[1] = dt.Tables[0].Rows[0]["BasicModel"].ToString() + " / " + dt.Tables[0].Rows[0]["TransmissionType"].ToString();
            arrayHeader[2] = dt.Tables[0].Rows[0]["ChassisCode"].ToString();
            arrayHeader[3] = dt.Tables[0].Rows[0]["ChassisNo"].ToString();
            arrayHeader[4] = dt.Tables[0].Rows[0]["Colour"].ToString();
            arrayHeader[5] = dt.Tables[0].Rows[0]["EngineCode"].ToString();
            arrayHeader[6] = dt.Tables[0].Rows[0]["EngineNo"].ToString();
            arrayHeader[7] = dt.Tables[0].Rows[0]["LastServiceOdometer"].ToString();
            arrayHeader[8] = dt.Tables[0].Rows[0]["Dealer"].ToString();
            arrayHeader[9] = (dt.Tables[0].Rows[0]["LastServiceDate"] != null && !(dt.Tables[0].Rows[0]["LastServiceDate"] is DBNull)) ? Convert.ToDateTime(dt.Tables[0].Rows[0]["LastServiceDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[10] = dt.Tables[0].Rows[0]["Customer"].ToString();
            arrayHeader[11] = (dt.Tables[0].Rows[0]["FakturPajakDate"] != null && !(dt.Tables[0].Rows[0]["FakturPajakDate"] is DBNull)) ? Convert.ToDateTime(dt.Tables[0].Rows[0]["FakturPajakDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[12] = (Convert.ToBoolean(dt.Tables[0].Rows[0]["isActive"].ToString())) ? "Aktif" : "Tidak Aktif";
            arrayHeader[13] = dt.Tables[0].Rows[0]["ProductionYear"].ToString();

            CreateHdrSvRpTrn010(gtf, arrayHeader, arrayHeader2);
            gtf.PrintHeader();

            string docNo = "", jobOrderNo = "", jobOrderDate = "";
            int noUrut = 0;
            decimal ttlNilai = 0;
            bool lastData = false;

            for (int i = 0; i <= dt.Tables[0].Rows.Count; i++)
            {
                if (i == dt.Tables[0].Rows.Count)
                {
                    //gtf.SetDataDetail(" ", 1, ' ', false, true);                   
                    if (jobOrderNo != dt.Tables[0].Rows[i - 1]["JobOrderNo"].ToString())
                    {
                        gtf.SetDataDetailSpace(131);
                        gtf.SetDataDetail("-", 16, '-', false, true);
                        gtf.SetDataDetailSpace(41);
                        gtf.SetDataDetail("", 89, ' ', true);
                        gtf.SetDataDetail(ttlNilai.ToString(), 16, '-', false, true, true, true, "n0");
                        gtf.SetDataDetail(" ", 1, ' ', false, true);
                        ttlNilai = 0;

                        noUrut += 1;
                        gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i - 1]["JobOrderDate"] != null ? Convert.ToDateTime(dt.Tables[0].Rows[i - 1]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy") : "", 11, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i - 1]["JobOrderNo"].ToString(), 13, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i - 1]["Odometer"].ToString(), 9, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i - 1]["ServiceRequestDesc"].ToString().Replace("\n", " ").Replace("\r", " "), 122, ' ', false, true);

                        gtf.SetDataDetailSpace(41);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i - 1]["Kode"].ToString(), 25, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i - 1]["Description"].ToString(), 52, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i - 1]["Qty"].ToString(), 10, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i - 1]["Amt"].ToString(), 16, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i - 1]["MechanicID"].ToString(), 15, ' ', false, true);

                        ttlNilai += decimal.Parse(dt.Tables[0].Rows[i - 1]["Amt"].ToString());

                        gtf.SetDataDetailSpace(131);
                        gtf.SetDataDetail("-", 16, '-', false, true);
                        gtf.SetDataDetailSpace(41);
                        gtf.SetDataDetail("No Faktur : " + dt.Tables[0].Rows[i - 1]["FPJNo"].ToString() + " , Tgl : " + Convert.ToDateTime(dt.Tables[0].Rows[i - 1]["FPJDate"].ToString()).ToString("dd-MMM-yyyy"), 89, ' ', true);
                        gtf.SetDataDetail(ttlNilai.ToString(), 16, '-', false, true, true, true, "n0");
                        gtf.SetDataDetail(" ", 1, ' ', false, true);
                    }
                    else
                    {
                        gtf.SetDataDetailSpace(131);
                        gtf.SetDataDetail("-", 16, '-', false, true);
                        gtf.SetDataDetailSpace(41);
                        gtf.SetDataDetail("No Faktur : " + dt.Tables[0].Rows[i - 1]["FPJNo"].ToString() + " , Tgl : " + Convert.ToDateTime(dt.Tables[0].Rows[i - 1]["FPJDate"].ToString()).ToString("dd-MMM-yyyy"), 89, ' ', true);
                        gtf.SetDataDetail(ttlNilai.ToString(), 16, '-', false, true, true, true, "n0");
                        gtf.SetDataDetail(" ", 1, ' ', false, true);
                    }

                    gtf.PrintData(false);

                    break;
                }
                if (docNo == "")
                {
                    docNo = dt.Tables[0].Rows[i]["PoliceRegNo"].ToString();
                    jobOrderNo = dt.Tables[0].Rows[i]["JobOrderNo"].ToString();
                    jobOrderDate = Convert.ToDateTime(dt.Tables[0].Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy");

                    noUrut += 1;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["JobOrderDate"] != null ? Convert.ToDateTime(dt.Tables[0].Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy") : "", 11, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["Odometer"].ToString(), 9, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["ServiceRequestDesc"].ToString().Replace("\n", " ").Replace("\r", " "), 122, ' ', false, true);
                }
                else
                {
                    if (docNo != dt.Tables[0].Rows[i]["PoliceRegNo"].ToString())
                    {
                        gtf.SetDataDetailSpace(131);
                        gtf.SetDataDetail("-", 16, '-', false, true);
                        gtf.SetDataDetailSpace(41);
                        gtf.SetDataDetail("No Faktur : " + dt.Tables[0].Rows[i]["FPJNo"].ToString() + " , Tgl : " + Convert.ToDateTime(dt.Tables[0].Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy"), 89, ' ', true);
                        gtf.SetDataDetail(ttlNilai.ToString(), 16, '-', false, true, true, true, "n0");
                        gtf.SetDataDetail(" ", 1, ' ', false, true);
                        gtf.PrintData(false);

                        ttlNilai = 0;

                        arrayHeader[0] = dt.Tables[0].Rows[i]["PoliceRegNo"].ToString();
                        arrayHeader[1] = dt.Tables[0].Rows[i]["BasicModel"].ToString() + " / " + dt.Tables[0].Rows[i]["TransmissionType"].ToString();
                        arrayHeader[2] = dt.Tables[0].Rows[i]["ChassisCode"].ToString();
                        arrayHeader[3] = dt.Tables[0].Rows[i]["ChassisNo"].ToString();
                        arrayHeader[4] = dt.Tables[0].Rows[i]["Colour"].ToString();
                        arrayHeader[5] = dt.Tables[0].Rows[i]["EngineCode"].ToString();
                        arrayHeader[6] = dt.Tables[0].Rows[i]["EngineNo"].ToString();
                        arrayHeader[7] = dt.Tables[0].Rows[i]["LastServiceOdometer"].ToString();
                        arrayHeader[8] = dt.Tables[0].Rows[i]["Dealer"].ToString();
                        arrayHeader[9] = (dt.Tables[0].Rows[i]["LastServiceDate"] != null && !(dt.Tables[0].Rows[i]["LastServiceDate"] is DBNull)) ? Convert.ToDateTime(dt.Tables[0].Rows[i]["LastServiceDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                        arrayHeader[10] = dt.Tables[0].Rows[i]["Customer"].ToString();
                        arrayHeader[11] = (dt.Tables[0].Rows[i]["FakturPajakDate"] != null && !(dt.Tables[0].Rows[i]["FakturPajakDate"] is DBNull)) ? Convert.ToDateTime(dt.Tables[0].Rows[i]["FakturPajakDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                        arrayHeader[12] = (Convert.ToBoolean(dt.Tables[0].Rows[i]["isActive"].ToString())) ? "Aktif" : "Tidak Aktif";
                        arrayHeader[13] = dt.Tables[0].Rows[i]["ProductionYear"].ToString();

                        arrayHeader2 = dt.Tables[0].Rows[i]["Remarks"].ToString().Split('\n');

                        gtf.CleanHeader();
                        CreateHdrSvRpTrn010(gtf, arrayHeader, arrayHeader2);

                        gtf.PrintAfterBreakSamePage();

                        lastData = false;
                        noUrut = 0;

                        docNo = dt.Tables[0].Rows[i]["PoliceRegNo"].ToString();
                        jobOrderNo = dt.Tables[0].Rows[i]["JobOrderNo"].ToString();
                        jobOrderDate = Convert.ToDateTime(dt.Tables[0].Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy");

                    }
                }

                if (noUrut == 0)
                {
                    noUrut += 1;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["JobOrderDate"] != null ? Convert.ToDateTime(dt.Tables[0].Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy") : "", 11, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["Odometer"].ToString(), 9, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["ServiceRequestDesc"].ToString().Replace("\n", " ").Replace("\r", " "), 122, ' ', false, true);
                }

                gtf.SetDataDetailSpace(41);
                gtf.SetDataDetail(dt.Tables[0].Rows[i]["Kode"].ToString(), 25, ' ', true);
                gtf.SetDataDetail(dt.Tables[0].Rows[i]["Description"].ToString(), 52, ' ', true);
                gtf.SetDataDetail(dt.Tables[0].Rows[i]["Qty"].ToString(), 10, ' ', true, false, true, true, "n2");
                gtf.SetDataDetail(dt.Tables[0].Rows[i]["Amt"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Tables[0].Rows[i]["MechanicID"].ToString(), 15, ' ', false, true);

                //}

                //if (i + 1 < dt.Tables[0].Rows.Count)
                //{
                //    if (docNo == dt.Tables[0].Rows[i + 1]["PoliceRegNo"].ToString())
                gtf.PrintData(false);
                //    else
                //        lastData = true;
                //}
                //else
                //    lastData = true;

                ttlNilai += decimal.Parse(dt.Tables[0].Rows[i]["Amt"].ToString());
            }

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal(true, lastData, true);
            else
            {
                //if (gtf.PrintTotal(true, lastData, true) == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();


            return gtf.sbDataTxt.ToString();
        }

        private void CreateHdrSvRpTrn010(GenerateTextFileReport gtf, string[] arrayHeader, string[] arrayHeader2)
        {
            //gtf.SetGroupHeader("Mulai Tanggal Service", 21, ' ', true);
            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(Convert.ToDateTime(dsSource.Tables[0].Rows[0]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy"), 25, ' ', false, true);
            gtf.SetGroupHeader("", 163, ' ', false, true);
            gtf.SetGroupHeader("No Polisi : " + arrayHeader[0].ToString(), 53, ' ', true);
            int intArrayHeader2 = 0;
            if (arrayHeader2.Length > 0 && arrayHeader2.Length > intArrayHeader2)
            {
                gtf.SetGroupHeader("Remarks : " + arrayHeader2[intArrayHeader2].ToString().Trim(), 54, ' ', true);
                intArrayHeader2 += 1;
            }
            else
                gtf.SetGroupHeader("", 54, ' ', true);

            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(, 26, ' ', true);
            //gtf.SetGroupHeader("Basic Model/Trans", 21, ' ');
            gtf.SetGroupHeader("Basic Model/Trans    : " + arrayHeader[1].ToString(), 54, ' ', false, true);
            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[1].ToString(), 27, ' ', true);
            //gtf.SetGroupHeader("Dealer", 14, ' ');
            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[8].ToString(), 43, ' ', false, true);

            //gtf.PrintHeader();

            gtf.SetGroupHeader("No Rangka : " + arrayHeader[2].ToString(), 24, ' ', true);
            gtf.SetGroupHeader(arrayHeader[3].ToString(), 28, ' ', true);
            if (arrayHeader2.Length > 0 && arrayHeader2.Length > intArrayHeader2)
            {
                gtf.SetGroupHeaderSpace(10);
                gtf.SetGroupHeader(arrayHeader2[intArrayHeader2].ToString().Trim(), 44, ' ', true);
                intArrayHeader2 += 1;
            }
            else
                gtf.SetGroupHeader("", 54, ' ', true);

            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[2].ToString(), 15, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[3].ToString(), 10, ' ', true);
            //gtf.SetGroupHeader("Warna Kendaraan", 21, ' ');
            gtf.SetGroupHeader("Warna Kendaraan      : " + arrayHeader[4].ToString(), 54, ' ', false, true);
            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[4].ToString(), 27, ' ', true);
            //gtf.SetGroupHeader("Pelanggan", 14, ' ');
            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[10].ToString(), 43, ' ', false, true);

            gtf.SetGroupHeader("No Mesin  : " + arrayHeader[5].ToString(), 24, ' ', true);
            gtf.SetGroupHeader(arrayHeader[6].ToString(), 28, ' ', true);
            if (arrayHeader2.Length > 0 && arrayHeader2.Length > intArrayHeader2)
            {
                gtf.SetGroupHeaderSpace(10);
                gtf.SetGroupHeader(arrayHeader2[intArrayHeader2].ToString().Trim(), 44, ' ', true);
                intArrayHeader2 += 1;
            }
            else
                gtf.SetGroupHeader("", 54, ' ', true);

            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[5].ToString(), 15, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[6].ToString(), 10, ' ', true);
            //gtf.SetGroupHeader("Km Service Terakhir", 21, ' ');
            gtf.SetGroupHeader("Km Service Terakhir  : " + arrayHeader[7].ToString(), 54, ' ', false, true);
            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[7].ToString(), 27, ' ', true);
            //gtf.SetGroupHeader("Status", 14, ' ');
            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[12].ToString(), 43, ' ', false, true);

            //gtf.SetGroupHeader("Tahun Produksi", 22, ' ');
            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[13].ToString(), 26, ' ', true);
            //gtf.SetGroupHeader("Tgl Service Terakhir", 21, ' ');
            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[9].ToString(), 27, ' ', true);
            //gtf.SetGroupHeader("Tgl Pembelian", 14, ' ');
            //gtf.SetGroupHeader(":", 1, ' ', true);
            //gtf.SetGroupHeader(arrayHeader[11].ToString(), 43, ' ', false, true);

            gtf.SetGroupHeader("Dealer    : " + arrayHeader[8].ToString(), 53, ' ', true);
            if (arrayHeader2.Length > 0 && arrayHeader2.Length > intArrayHeader2)
            {
                gtf.SetGroupHeaderSpace(10);
                gtf.SetGroupHeader(arrayHeader2[intArrayHeader2].ToString().Trim(), 44, ' ', true);
                intArrayHeader2 += 1;
            }
            else
                gtf.SetGroupHeader("", 54, ' ', true);
            gtf.SetGroupHeader("Tgl Service Terakhir : " + arrayHeader[9].ToString(), 54, ' ', false, true);
            gtf.SetGroupHeader("Pelanggan : " + arrayHeader[10].ToString(), 53, ' ', true);
            if (arrayHeader2.Length > 0 && arrayHeader2.Length > intArrayHeader2)
            {
                gtf.SetGroupHeaderSpace(10);
                gtf.SetGroupHeader(arrayHeader2[intArrayHeader2].ToString().Trim(), 44, ' ', true);
                intArrayHeader2 += 1;
            }
            else
                gtf.SetGroupHeader("", 54, ' ', true);
            gtf.SetGroupHeader("Tgl Pembelian        : " + arrayHeader[11].ToString(), 54, ' ', false, true);
            gtf.SetGroupHeader("Status    : " + arrayHeader[12].ToString(), 53, ' ', true);
            if (arrayHeader2.Length > 0 && arrayHeader2.Length > intArrayHeader2)
            {
                gtf.SetGroupHeaderSpace(10);
                gtf.SetGroupHeader(arrayHeader2[intArrayHeader2].ToString().Trim(), 44, ' ', true);
                intArrayHeader2 += 1;
            }
            else
                gtf.SetGroupHeader("", 54, ' ', true);
            gtf.SetGroupHeader("Tahun Produksi       : " + arrayHeader[13].ToString(), 54, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("No", 4, ' ', true, false, true);
            gtf.SetGroupHeader("TGL. SPK", 11, ' ', true);
            gtf.SetGroupHeader("NO. SPK", 13, ' ', true);
            gtf.SetGroupHeader("KM", 9, ' ', true, false, true);
            gtf.SetGroupHeader("KODE", 25, ' ', true);
            gtf.SetGroupHeader("KETERANGAN", 52, ' ', true);
            gtf.SetGroupHeader("NK/QTY", 10, ' ', true, false, true);
            gtf.SetGroupHeader("NILAI", 16, ' ', true, false, true);
            gtf.SetGroupHeader("MEKANIK", 15, ' ', false, true);
            gtf.SetGroupHeaderLine();
        }

        
       
    }
}