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
    public class txtSvRpReport : IRptProc 
    {
        private SimDms.Service.Models.DataContext ctx = new SimDms.Service.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        public SysUser CurrentUser { get; set; }
        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam, string paramReport)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "exec " + sproc + " " + sparam;
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            da.Fill(dt);

            if (rptId == "SvRpReport014")
            {
                da.Fill(ds);
            }
            
            switch (rptId)
            {
                case "SvRpReport00101":
                    return CreateReportSvRpReport00101(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam );
                case "SvRpReport00102":
                    return CreateReportSvRpReport00102(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport00103":
                    return CreateReportSvRpReport00103(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport00104":
                    return CreateReportSvRpReport00104(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport00105":
                    return CreateReportSvRpReport00105(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport002":
                    return CreateReportSvRpReport002(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport003":
                    return CreateReportSvRpReport003(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport004":
                    return CreateReportSvRpReport004(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport005":
                    return CreateReportSvRpReport005(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport006":
                    return CreateReportSvRpReport006(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport007":
                    return CreateReportSvRpReport007(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport010MK":
                    return CreateReportSvRpReport010MK(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport010SA":
                    return CreateReportSvRpReport010SA(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport010FM":
                    return CreateReportSvRpReport010FM(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport011MK":
                    return CreateReportSvRpReport011MK(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport011SA":
                    return CreateReportSvRpReport011SA(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport011FM":
                    return CreateReportSvRpReport011FM(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport011V2":
                    return CreateReportSvRpReport011V2(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport012":
                    return CreateReportSvRpReport012(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport013":
                    return CreateReportSvRpReport013(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport014":
                    return CreateReportSvRpReport014(rptId, ds, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport015":
                    return CreateReportSvRpReport015(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport016":
                    return CreateReportSvRpReport016(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport017":
                    return CreateReportSvRpReport017(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport022":
                    return CreateReportSvRpReport022(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport023":
                    return CreateReportSvRpReport023(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport026":
                    return CreateReportSvRpReport026(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                case "SvRpReport028":
                    return CreateReportSvRpReport028(rptId, dt, paramReport, printerloc, print, "", fullpage, oparam);
                default :
                    return "";
            }

        }

        #region SvRpReport

        private string CreateReportSvRpReport00101(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter) 
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W233", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader("PERIODE : " + Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyyy") + " s/d " + Convert.ToDateTime(setTextParameter[2].ToString()).ToString("dd-MMM-yyyy"), 230, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
            gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NOMOR", 27, ' ', true, false, false, true);
            gtf.SetGroupHeader("PELANGGAN", 28, ' ', true);
            gtf.SetGroupHeader("NILAI", 47, ' ', true, false, false, true);
            gtf.SetGroupHeader("POTONGAN", 35, ' ', true, false, false, true);
            gtf.SetGroupHeader("DPP", 15, ' ', true, false, true);
            gtf.SetGroupHeader("PPN", 11, ' ', true, false, true);
            gtf.SetGroupHeader("HASIL PENJUALAN", 47, ' ', false, true, false, true);

            gtf.SetGroupHeaderSpace(17);
            gtf.SetGroupHeader("-", 27, '-', true, false, false, true);
            gtf.SetGroupHeaderSpace(29);
            gtf.SetGroupHeader("-", 47, '-', true, false, false, true);
            gtf.SetGroupHeader("-", 35, '-', true, false, false, true);
            gtf.SetGroupHeaderSpace(28);
            gtf.SetGroupHeader("-", 47, '-', false, true, false, true);

            gtf.SetGroupHeaderSpace(17);
            gtf.SetGroupHeader("FAKTUR", 13, ' ', true);
            gtf.SetGroupHeader("SPK", 13, ' ', true);
            gtf.SetGroupHeaderSpace(29);
            gtf.SetGroupHeader("JASA", 15, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 15, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 15, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 11, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 11, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 11, ' ', true, false, true);
            gtf.SetGroupHeaderSpace(28);
            gtf.SetGroupHeader("TOTAL", 15, ' ', true, false, true);
            gtf.SetGroupHeader("CASH", 15, ' ', true, false, true);
            gtf.SetGroupHeader("CREDIT", 15, ' ', false, true, true);

            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string invDate = "", groupHeader2 = "";
            int noUrut = 0;

            decimal ttlNilaiJasa = 0, ttlNilaiSparepart = 0, ttlNilaiMaterial = 0;
            decimal ttlPotJasa = 0, ttlPotSparepart = 0, ttlPotMaterial = 0;
            decimal ttlDPP = 0, ttlPPN = 0;
            decimal ttlJualTotal = 0, ttlJualCash = 0, ttlJualCredit = 0;

            decimal subttlNilaiJasa = 0, subttlNilaiSparepart = 0, subttlNilaiMaterial = 0;
            decimal subttlPotJasa = 0, subttlPotSparepart = 0, subttlPotMaterial = 0;
            decimal subttlDPP = 0, subttlPPN = 0;
            decimal subttlJualTotal = 0, subttlJualCash = 0, subttlJualCredit = 0;

            decimal ttlNilaiJasaGrnd = 0, ttlNilaiSparepartGrnd = 0, ttlNilaiMaterialGrnd = 0;
            decimal ttlPotJasaGrnd = 0, ttlPotSparepartGrnd = 0, ttlPotMaterialGrnd = 0;
            decimal ttlDPPGrnd = 0, ttlPPNGrnd = 0;
            decimal ttlJualTotalGrnd = 0, ttlJualCashGrnd = 0, ttlJualCreditGrnd = 0;


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (invDate == "")
                {
                    invDate = Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy");
                    groupHeader2 = dt.Rows[i]["GroupHeader2"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Cash"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Credit"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }
                else if (invDate != Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"))
                {
                    gtf.SetDataDetailSpace(17);
                    gtf.SetDataDetail("-", 216, '-', false, true);
                    gtf.SetDataDetailSpace(17);
                    gtf.SetDataDetail("SubTotal", 13, ' ', true);
                    gtf.SetDataDetail(invDate.ToString(), 13, ' ', true);
                    gtf.SetDataDetailSpace(29);
                    gtf.SetDataDetail(subttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    subttlDPP = subttlJualCash = subttlJualCredit = subttlJualTotal = subttlNilaiJasa = subttlNilaiMaterial =
                        subttlNilaiSparepart = subttlPotJasa = subttlPotMaterial = subttlPotSparepart = subttlPPN = 0;

                    if (groupHeader2 != dt.Rows[i]["GroupHeader2"].ToString())
                    {
                        gtf.SetDataDetailSpace(17);
                        gtf.SetDataDetail(dt.Rows[i - 1]["GroupHeader2Info"].ToString(), 13, ' ', true);
                        gtf.SetDataDetailSpace(43);
                        gtf.SetDataDetail(ttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false);

                        ttlDPP = ttlJualCash = ttlJualCredit = ttlJualTotal = ttlNilaiJasa = ttlNilaiMaterial = ttlNilaiSparepart = ttlPotJasa =
                            ttlPotMaterial = ttlPotSparepart = ttlPPN = 0;

                        groupHeader2 = dt.Rows[i]["GroupHeader2"].ToString();
                    }

                    invDate = Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy");
                    noUrut++;
                    gtf.SetDataDetail(" ", 1, ' ', false, true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Cash"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Credit"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                }
                else if (invDate == Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"))
                {
                    noUrut++;
                    gtf.SetDataDetailSpace(12);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Cash"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Credit"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }

                subttlNilaiJasa += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                subttlNilaiSparepart += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                subttlNilaiMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                subttlPotJasa += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                subttlPotSparepart += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                subttlPotMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                subttlDPP += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                subttlPPN += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                subttlJualTotal += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());
                subttlJualCash += Convert.ToDecimal(dt.Rows[i]["Cash"].ToString());
                subttlJualCredit += Convert.ToDecimal(dt.Rows[i]["Credit"].ToString());

                ttlNilaiJasa += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                ttlNilaiSparepart += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                ttlNilaiMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                ttlPotJasa += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                ttlPotSparepart += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                ttlPotMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                ttlDPP += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                ttlPPN += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                ttlJualTotal += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());
                ttlJualCash += Convert.ToDecimal(dt.Rows[i]["Cash"].ToString());
                ttlJualCredit += Convert.ToDecimal(dt.Rows[i]["Credit"].ToString());

                ttlNilaiJasaGrnd += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                ttlNilaiSparepartGrnd += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                ttlNilaiMaterialGrnd += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                ttlPotJasaGrnd += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                ttlPotSparepartGrnd += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                ttlPotMaterialGrnd += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                ttlDPPGrnd += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                ttlPPNGrnd += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                ttlJualTotalGrnd += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());
                ttlJualCashGrnd += Convert.ToDecimal(dt.Rows[i]["Cash"].ToString());
                ttlJualCreditGrnd += Convert.ToDecimal(dt.Rows[i]["Credit"].ToString());
            }
            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail("-", 216, '-', false, true);
            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail("SubTotal", 13, ' ', true);
            gtf.SetDataDetail(invDate.ToString(), 13, ' ', true);
            gtf.SetDataDetailSpace(29);
            gtf.SetDataDetail(subttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["GroupHeader2Info"].ToString(), 13, ' ', true);
            gtf.SetDataDetailSpace(43);
            gtf.SetDataDetail(ttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail("Grand Total", 13, ' ', true);
            gtf.SetDataDetailSpace(43);
            gtf.SetDataDetail(ttlNilaiJasaGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiSparepartGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiMaterialGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotJasaGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotSparepartGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotMaterialGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPPNGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualTotalGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCashGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCreditGrnd.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.SetDataDetail("-", 233, '-', false, true);
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport00102(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W233", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader("PERIODE : " + Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyyy") + " s/d " + Convert.ToDateTime(setTextParameter[2].ToString()).ToString("dd-MMM-yyyy"), 230, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
            gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NOMOR", 27, ' ', true, false, false, true);
            gtf.SetGroupHeader("PELANGGAN", 28, ' ', true);
            gtf.SetGroupHeader("NILAI", 47, ' ', true, false, false, true);
            gtf.SetGroupHeader("POTONGAN", 35, ' ', true, false, false, true);
            gtf.SetGroupHeader("DPP", 15, ' ', true, false, true);
            gtf.SetGroupHeader("PPN", 11, ' ', true, false, true);
            gtf.SetGroupHeader("HASIL PENJUALAN", 47, ' ', false, true, false, true);

            gtf.SetGroupHeaderSpace(17);
            gtf.SetGroupHeader("-", 27, '-', true, false, false, true);
            gtf.SetGroupHeaderSpace(29);
            gtf.SetGroupHeader("-", 47, '-', true, false, false, true);
            gtf.SetGroupHeader("-", 35, '-', true, false, false, true);
            gtf.SetGroupHeaderSpace(28);
            gtf.SetGroupHeader("-", 47, '-', false, true, false, true);

            gtf.SetGroupHeaderSpace(17);
            gtf.SetGroupHeader("FAKTUR", 13, ' ', true);
            gtf.SetGroupHeader("SPK", 13, ' ', true);
            gtf.SetGroupHeaderSpace(29);
            gtf.SetGroupHeader("JASA", 15, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 15, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 15, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 11, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 11, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 11, ' ', true, false, true);
            gtf.SetGroupHeaderSpace(28);
            gtf.SetGroupHeader("TOTAL", 15, ' ', true, false, true);
            gtf.SetGroupHeader("CASH", 15, ' ', true, false, true);
            gtf.SetGroupHeader("CREDIT", 15, ' ', false, true, true);

            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string invDate = "", groupHeader2 = "";
            int noUrut = 0;

            decimal ttlNilaiJasa = 0, ttlNilaiSparepart = 0, ttlNilaiMaterial = 0;
            decimal ttlPotJasa = 0, ttlPotSparepart = 0, ttlPotMaterial = 0;
            decimal ttlDPP = 0, ttlPPN = 0;
            decimal ttlJualTotal = 0, ttlJualCash = 0, ttlJualCredit = 0;

            decimal subttlNilaiJasa = 0, subttlNilaiSparepart = 0, subttlNilaiMaterial = 0;
            decimal subttlPotJasa = 0, subttlPotSparepart = 0, subttlPotMaterial = 0;
            decimal subttlDPP = 0, subttlPPN = 0;
            decimal subttlJualTotal = 0, subttlJualCash = 0, subttlJualCredit = 0;

            decimal ttlNilaiJasaGrnd = 0, ttlNilaiSparepartGrnd = 0, ttlNilaiMaterialGrnd = 0;
            decimal ttlPotJasaGrnd = 0, ttlPotSparepartGrnd = 0, ttlPotMaterialGrnd = 0;
            decimal ttlDPPGrnd = 0, ttlPPNGrnd = 0;
            decimal ttlJualTotalGrnd = 0, ttlJualCashGrnd = 0, ttlJualCreditGrnd = 0;


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (invDate == "")
                {
                    invDate = Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy");
                    noUrut++;
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["FPJNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Cash"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Credit"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);
                }
                else if (invDate != Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy"))
                {
                    gtf.SetDataDetailSpace(17);
                    gtf.SetDataDetail("-", 216, '-', false, true);
                    gtf.SetDataDetailSpace(17);
                    gtf.SetDataDetail("SubTotal", 13, ' ', true);
                    gtf.SetDataDetail(invDate.ToString(), 13, ' ', true);
                    gtf.SetDataDetailSpace(29);
                    gtf.SetDataDetail(subttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    subttlDPP = subttlJualCash = subttlJualCredit = subttlJualTotal = subttlNilaiJasa = subttlNilaiMaterial =
                        subttlNilaiSparepart = subttlPotJasa = subttlPotMaterial = subttlPotSparepart = subttlPPN = 0;

                    invDate = Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy");
                    noUrut++;
                    gtf.SetDataDetail(" ", 1, ' ', false, true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["FPJNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Cash"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Credit"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);

                }
                else if (invDate == Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy"))
                {
                    noUrut++;
                    gtf.SetDataDetailSpace(12);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["FPJNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Cash"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Credit"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);
                }

                subttlNilaiJasa += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                subttlNilaiSparepart += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                subttlNilaiMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                subttlPotJasa += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                subttlPotSparepart += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                subttlPotMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                subttlDPP += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                subttlPPN += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                subttlJualTotal += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());
                subttlJualCash += Convert.ToDecimal(dt.Rows[i]["Cash"].ToString());
                subttlJualCredit += Convert.ToDecimal(dt.Rows[i]["Credit"].ToString());

                ttlNilaiJasaGrnd += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                ttlNilaiSparepartGrnd += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                ttlNilaiMaterialGrnd += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                ttlPotJasaGrnd += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                ttlPotSparepartGrnd += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                ttlPotMaterialGrnd += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                ttlDPPGrnd += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                ttlPPNGrnd += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                ttlJualTotalGrnd += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());
                ttlJualCashGrnd += Convert.ToDecimal(dt.Rows[i]["Cash"].ToString());
                ttlJualCreditGrnd += Convert.ToDecimal(dt.Rows[i]["Credit"].ToString());
            }
            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail("-", 216, '-', false, true);
            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail("SubTotal", 13, ' ', true);
            gtf.SetDataDetail(invDate.ToString(), 13, ' ', true);
            gtf.SetDataDetailSpace(29);
            gtf.SetDataDetail(subttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail("Grand Total", 13, ' ', true);
            gtf.SetDataDetailSpace(43);
            gtf.SetDataDetail(ttlNilaiJasaGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiSparepartGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiMaterialGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotJasaGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotSparepartGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotMaterialGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPPNGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualTotalGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCashGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCreditGrnd.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.SetDataDetail("-", 233, '-', false, true);
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport00103(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W272", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader("PERIODE : " + Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyyy") + " s/d " + Convert.ToDateTime(setTextParameter[2].ToString()).ToString("dd-MMM-yyyy"), 230, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
            gtf.SetGroupHeader("SERI PAJAK", 32, ' ', true, false, false, true);
            gtf.SetGroupHeader("PELANGGAN", 54, ' ', true, false, false, true);
            gtf.SetGroupHeader("FAKTUR / INVOICE", 25, ' ', true, false, false, true);
            gtf.SetGroupHeader("SURAT PERINTAH KERJA", 25, ' ', true, false, false, true);
            gtf.SetGroupHeader("NILAI", 47, ' ', true, false, false, true);
            gtf.SetGroupHeader("POTONGAN", 35, ' ', true, false, false, true);
            gtf.SetGroupHeader("DPP", 15, ' ', true, false, true);
            gtf.SetGroupHeader("PPN", 11, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 15, ' ', false, true, true);

            gtf.SetGroupHeaderSpace(5);
            gtf.SetGroupHeader("-", 32, '-', true);
            gtf.SetGroupHeader("-", 54, '-', true);
            gtf.SetGroupHeader("-", 25, '-', true);
            gtf.SetGroupHeader("-", 25, '-', true);
            gtf.SetGroupHeader("-", 47, '-', true);
            gtf.SetGroupHeader("-", 35, '-', false, true);

            gtf.SetGroupHeaderSpace(5);
            gtf.SetGroupHeader("NOMOR", 20, ' ', true);
            gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
            gtf.SetGroupHeader("NAMA", 33, ' ', true);
            gtf.SetGroupHeader("N.P.W.P", 20, ' ', true);
            gtf.SetGroupHeader("NOMOR", 13, ' ', true);
            gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
            gtf.SetGroupHeader("NOMOR", 13, ' ', true);
            gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
            gtf.SetGroupHeader("JASA", 15, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 15, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 15, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 11, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 11, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 11, ' ', true, false, true);
            gtf.SetGroupHeaderSpace(28);
            gtf.SetGroupHeader("SERVICE", 15, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string invDate = "";
            int noUrut = 0;

            decimal subttlNilaiJasa = 0, subttlNilaiSparepart = 0, subttlNilaiMaterial = 0;
            decimal subttlPotJasa = 0, subttlPotSparepart = 0, subttlPotMaterial = 0;
            decimal subttlDPP = 0, subttlPPN = 0, subttlTotal = 0;

            decimal ttlNilaiJasaGrnd = 0, ttlNilaiSparepartGrnd = 0, ttlNilaiMaterialGrnd = 0;
            decimal ttlPotJasaGrnd = 0, ttlPotSparepartGrnd = 0, ttlPotMaterialGrnd = 0;
            decimal ttlDPPGrnd = 0, ttlPPNGrnd = 0, ttlJualTotalGrnd = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (invDate == "")
                {
                    invDate = Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy");
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["FPJGovNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 33, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["NPWPNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);
                }
                else if (invDate != Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"))
                {
                    gtf.SetDataDetailSpace(119);
                    gtf.SetDataDetail("-", 153, '-', false, true);
                    gtf.SetDataDetailSpace(119);
                    gtf.SetDataDetail("SubTotal", 25, ' ', true);
                    gtf.SetDataDetail(subttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlTotal.ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    subttlDPP = subttlTotal = subttlNilaiJasa = subttlNilaiMaterial =
                        subttlNilaiSparepart = subttlPotJasa = subttlPotMaterial = subttlPotSparepart = subttlPPN = 0;

                    invDate = Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy");
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["FPJGovNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 33, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["NPWPNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);

                }
                else if (invDate == Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"))
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["FPJGovNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 33, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["NPWPNo"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);
                }

                subttlNilaiJasa += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                subttlNilaiSparepart += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                subttlNilaiMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                subttlPotJasa += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                subttlPotSparepart += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                subttlPotMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                subttlDPP += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                subttlPPN += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                subttlTotal += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());

                ttlNilaiJasaGrnd += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                ttlNilaiSparepartGrnd += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                ttlNilaiMaterialGrnd += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                ttlPotJasaGrnd += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                ttlPotSparepartGrnd += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                ttlPotMaterialGrnd += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                ttlDPPGrnd += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                ttlPPNGrnd += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                ttlJualTotalGrnd += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());
            }
            gtf.SetDataDetailSpace(119);
            gtf.SetDataDetail("-", 153, '-', false, true);
            gtf.SetDataDetailSpace(119);
            gtf.SetDataDetail("SubTotal", 25, ' ', true);
            gtf.SetDataDetail(subttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlTotal.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(119);
            gtf.SetDataDetail("Grand Total", 25, ' ', true);
            gtf.SetDataDetail(ttlNilaiJasaGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiSparepartGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiMaterialGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotJasaGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotSparepartGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotMaterialGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPPNGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualTotalGrnd.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport00104(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W233", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader("PERIODE : " + Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyyy") + " s/d " + Convert.ToDateTime(setTextParameter[2].ToString()).ToString("dd-MMM-yyyy"), 230, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
            gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NOMOR", 27, ' ', true, false, false, true);
            gtf.SetGroupHeader("PELANGGAN", 28, ' ', true);
            gtf.SetGroupHeader("NILAI", 47, ' ', true, false, false, true);
            gtf.SetGroupHeader("POTONGAN", 35, ' ', true, false, false, true);
            gtf.SetGroupHeader("DPP", 15, ' ', true, false, true);
            gtf.SetGroupHeader("PPN", 11, ' ', true, false, true);
            gtf.SetGroupHeader("HASIL PENJUALAN", 47, ' ', false, true, false, true);

            gtf.SetGroupHeaderSpace(17);
            gtf.SetGroupHeader("-", 27, '-', true, false, false, true);
            gtf.SetGroupHeaderSpace(29);
            gtf.SetGroupHeader("-", 47, '-', true, false, false, true);
            gtf.SetGroupHeader("-", 35, '-', true, false, false, true);
            gtf.SetGroupHeaderSpace(28);
            gtf.SetGroupHeader("-", 47, '-', false, true, false, true);

            gtf.SetGroupHeaderSpace(17);
            gtf.SetGroupHeader("FAKTUR", 13, ' ', true);
            gtf.SetGroupHeader("SPK", 13, ' ', true);
            gtf.SetGroupHeaderSpace(29);
            gtf.SetGroupHeader("JASA", 15, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 15, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 15, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 11, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 11, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 11, ' ', true, false, true);
            gtf.SetGroupHeaderSpace(28);
            gtf.SetGroupHeader("TOTAL", 15, ' ', true, false, true);
            gtf.SetGroupHeader("CASH", 15, ' ', true, false, true);
            gtf.SetGroupHeader("CREDIT", 15, ' ', false, true, true);

            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string invDate = "", groupHeader2 = "";
            int noUrut = 0;

            decimal ttlNilaiJasa = 0, ttlNilaiSparepart = 0, ttlNilaiMaterial = 0;
            decimal ttlPotJasa = 0, ttlPotSparepart = 0, ttlPotMaterial = 0;
            decimal ttlDPP = 0, ttlPPN = 0;
            decimal ttlJualTotal = 0, ttlJualCash = 0, ttlJualCredit = 0;

            decimal subttlNilaiJasa = 0, subttlNilaiSparepart = 0, subttlNilaiMaterial = 0;
            decimal subttlPotJasa = 0, subttlPotSparepart = 0, subttlPotMaterial = 0;
            decimal subttlDPP = 0, subttlPPN = 0;
            decimal subttlJualTotal = 0, subttlJualCash = 0, subttlJualCredit = 0;

            decimal ttlNilaiJasaGrnd = 0, ttlNilaiSparepartGrnd = 0, ttlNilaiMaterialGrnd = 0;
            decimal ttlPotJasaGrnd = 0, ttlPotSparepartGrnd = 0, ttlPotMaterialGrnd = 0;
            decimal ttlDPPGrnd = 0, ttlPPNGrnd = 0;
            decimal ttlJualTotalGrnd = 0, ttlJualCashGrnd = 0, ttlJualCreditGrnd = 0;


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (invDate == "")
                {
                    invDate = Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy");
                    groupHeader2 = dt.Rows[i]["GroupHeader2"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Cash"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Credit"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);
                }
                else if (invDate != Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"))
                {
                    gtf.SetDataDetailSpace(17);
                    gtf.SetDataDetail("-", 216, '-', false, true);
                    gtf.SetDataDetailSpace(17);
                    gtf.SetDataDetail("SubTotal", 13, ' ', true);
                    gtf.SetDataDetail(invDate.ToString(), 13, ' ', true);
                    gtf.SetDataDetailSpace(29);
                    gtf.SetDataDetail(subttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    subttlDPP = subttlJualCash = subttlJualCredit = subttlJualTotal = subttlNilaiJasa = subttlNilaiMaterial =
                        subttlNilaiSparepart = subttlPotJasa = subttlPotMaterial = subttlPotSparepart = subttlPPN = 0;

                    if (groupHeader2 != dt.Rows[i]["GroupHeader2"].ToString())
                    {
                        gtf.SetDataDetailSpace(17);
                        gtf.SetDataDetail(dt.Rows[i - 1]["GroupHeader2Info"].ToString(), 13, ' ', true);
                        gtf.SetDataDetailSpace(43);
                        gtf.SetDataDetail(ttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false);

                        ttlDPP = ttlJualCash = ttlJualCredit = ttlJualTotal = ttlNilaiJasa = ttlNilaiMaterial = ttlNilaiSparepart = ttlPotJasa =
                            ttlPotMaterial = ttlPotSparepart = ttlPPN = 0;

                        groupHeader2 = dt.Rows[i]["GroupHeader2"].ToString();
                    }

                    invDate = Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy");
                    noUrut++;
                    gtf.SetDataDetail(" ", 1, ' ', false, true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Cash"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Credit"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);

                }
                else if (invDate == Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"))
                {
                    noUrut++;
                    gtf.SetDataDetailSpace(12);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Cash"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Credit"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);
                }

                subttlNilaiJasa += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                subttlNilaiSparepart += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                subttlNilaiMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                subttlPotJasa += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                subttlPotSparepart += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                subttlPotMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                subttlDPP += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                subttlPPN += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                subttlJualTotal += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());
                subttlJualCash += Convert.ToDecimal(dt.Rows[i]["Cash"].ToString());
                subttlJualCredit += Convert.ToDecimal(dt.Rows[i]["Credit"].ToString());

                ttlNilaiJasa += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                ttlNilaiSparepart += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                ttlNilaiMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                ttlPotJasa += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                ttlPotSparepart += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                ttlPotMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                ttlDPP += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                ttlPPN += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                ttlJualTotal += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());
                ttlJualCash += Convert.ToDecimal(dt.Rows[i]["Cash"].ToString());
                ttlJualCredit += Convert.ToDecimal(dt.Rows[i]["Credit"].ToString());

                ttlNilaiJasaGrnd += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                ttlNilaiSparepartGrnd += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                ttlNilaiMaterialGrnd += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                ttlPotJasaGrnd += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                ttlPotSparepartGrnd += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                ttlPotMaterialGrnd += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                ttlDPPGrnd += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                ttlPPNGrnd += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                ttlJualTotalGrnd += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());
                ttlJualCashGrnd += Convert.ToDecimal(dt.Rows[i]["Cash"].ToString());
                ttlJualCreditGrnd += Convert.ToDecimal(dt.Rows[i]["Credit"].ToString());
            }
            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail("-", 216, '-', false, true);
            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail("SubTotal", 13, ' ', true);
            gtf.SetDataDetail(invDate.ToString(), 13, ' ', true);
            gtf.SetDataDetailSpace(29);
            gtf.SetDataDetail(subttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["GroupHeader2Info"].ToString(), 13, ' ', true);
            gtf.SetDataDetailSpace(43);
            gtf.SetDataDetail(ttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail("Grand Total", 13, ' ', true);
            gtf.SetDataDetailSpace(43);
            gtf.SetDataDetail(ttlNilaiJasaGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiSparepartGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiMaterialGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotJasaGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotSparepartGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotMaterialGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPPNGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualTotalGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCashGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCreditGrnd.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.SetDataDetail("-", 233, '-', false, true);
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport00105(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W233", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader("PERIODE : " + Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyyy") + " s/d " + Convert.ToDateTime(setTextParameter[2].ToString()).ToString("dd-MMM-yyyy"), 230, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
            gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NOMOR", 27, ' ', true, false, false, true);
            gtf.SetGroupHeader("PELANGGAN", 28, ' ', true);
            gtf.SetGroupHeader("NILAI", 47, ' ', true, false, false, true);
            gtf.SetGroupHeader("POTONGAN", 35, ' ', true, false, false, true);
            gtf.SetGroupHeader("DPP", 15, ' ', true, false, true);
            gtf.SetGroupHeader("PPN", 11, ' ', true, false, true);
            gtf.SetGroupHeader("HASIL PENJUALAN", 47, ' ', false, true, false, true);

            gtf.SetGroupHeaderSpace(17);
            gtf.SetGroupHeader("-", 27, '-', true, false, false, true);
            gtf.SetGroupHeaderSpace(29);
            gtf.SetGroupHeader("-", 47, '-', true, false, false, true);
            gtf.SetGroupHeader("-", 35, '-', true, false, false, true);
            gtf.SetGroupHeaderSpace(28);
            gtf.SetGroupHeader("-", 47, '-', false, true, false, true);

            gtf.SetGroupHeaderSpace(17);
            gtf.SetGroupHeader("FAKTUR", 13, ' ', true);
            gtf.SetGroupHeader("SPK", 13, ' ', true);
            gtf.SetGroupHeaderSpace(29);
            gtf.SetGroupHeader("JASA", 15, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 15, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 15, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 11, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 11, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 11, ' ', true, false, true);
            gtf.SetGroupHeaderSpace(28);
            gtf.SetGroupHeader("TOTAL", 15, ' ', true, false, true);
            gtf.SetGroupHeader("CASH", 15, ' ', true, false, true);
            gtf.SetGroupHeader("CREDIT", 15, ' ', false, true, true);

            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string invDate = "", groupHeader2 = "";
            int noUrut = 0;

            decimal ttlNilaiJasa = 0, ttlNilaiSparepart = 0, ttlNilaiMaterial = 0;
            decimal ttlPotJasa = 0, ttlPotSparepart = 0, ttlPotMaterial = 0;
            decimal ttlDPP = 0, ttlPPN = 0;
            decimal ttlJualTotal = 0, ttlJualCash = 0, ttlJualCredit = 0;

            decimal subttlNilaiJasa = 0, subttlNilaiSparepart = 0, subttlNilaiMaterial = 0;
            decimal subttlPotJasa = 0, subttlPotSparepart = 0, subttlPotMaterial = 0;
            decimal subttlDPP = 0, subttlPPN = 0;
            decimal subttlJualTotal = 0, subttlJualCash = 0, subttlJualCredit = 0;

            decimal ttlNilaiJasaGrnd = 0, ttlNilaiSparepartGrnd = 0, ttlNilaiMaterialGrnd = 0;
            decimal ttlPotJasaGrnd = 0, ttlPotSparepartGrnd = 0, ttlPotMaterialGrnd = 0;
            decimal ttlDPPGrnd = 0, ttlPPNGrnd = 0;
            decimal ttlJualTotalGrnd = 0, ttlJualCashGrnd = 0, ttlJualCreditGrnd = 0;


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (invDate == "")
                {
                    invDate = Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy");
                    groupHeader2 = dt.Rows[i]["GroupHeader2"].ToString();
                    noUrut++;
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Cash"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Credit"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }
                else if (invDate != Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"))
                {
                    gtf.SetDataDetailSpace(17);
                    gtf.SetDataDetail("-", 216, '-', false, true);
                    gtf.SetDataDetailSpace(17);
                    gtf.SetDataDetail("SubTotal", 13, ' ', true);
                    gtf.SetDataDetail(invDate.ToString(), 13, ' ', true);
                    gtf.SetDataDetailSpace(29);
                    gtf.SetDataDetail(subttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    subttlDPP = subttlJualCash = subttlJualCredit = subttlJualTotal = subttlNilaiJasa = subttlNilaiMaterial =
                        subttlNilaiSparepart = subttlPotJasa = subttlPotMaterial = subttlPotSparepart = subttlPPN = 0;

                    if (groupHeader2 != dt.Rows[i]["GroupHeader2"].ToString())
                    {
                        gtf.SetDataDetailSpace(17);
                        gtf.SetDataDetail(dt.Rows[i - 1]["GroupHeader2Info"].ToString(), 13, ' ', true);
                        gtf.SetDataDetailSpace(43);
                        gtf.SetDataDetail(ttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false);

                        ttlDPP = ttlJualCash = ttlJualCredit = ttlJualTotal = ttlNilaiJasa = ttlNilaiMaterial = ttlNilaiSparepart = ttlPotJasa =
                            ttlPotMaterial = ttlPotSparepart = ttlPPN = 0;

                        groupHeader2 = dt.Rows[i]["GroupHeader2"].ToString();
                    }

                    invDate = Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy");
                    noUrut++;
                    gtf.SetDataDetail(" ", 1, ' ', false, true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Cash"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Credit"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                }
                else if (invDate == Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"))
                {
                    noUrut++;
                    gtf.SetDataDetailSpace(12);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 28, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Cash"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["Credit"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }

                subttlNilaiJasa += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                subttlNilaiSparepart += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                subttlNilaiMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                subttlPotJasa += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                subttlPotSparepart += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                subttlPotMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                subttlDPP += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                subttlPPN += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                subttlJualTotal += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());
                subttlJualCash += Convert.ToDecimal(dt.Rows[i]["Cash"].ToString());
                subttlJualCredit += Convert.ToDecimal(dt.Rows[i]["Credit"].ToString());

                ttlNilaiJasa += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                ttlNilaiSparepart += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                ttlNilaiMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                ttlPotJasa += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                ttlPotSparepart += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                ttlPotMaterial += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                ttlDPP += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                ttlPPN += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                ttlJualTotal += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());
                ttlJualCash += Convert.ToDecimal(dt.Rows[i]["Cash"].ToString());
                ttlJualCredit += Convert.ToDecimal(dt.Rows[i]["Credit"].ToString());

                ttlNilaiJasaGrnd += Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString());
                ttlNilaiSparepartGrnd += Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString());
                ttlNilaiMaterialGrnd += Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString());
                ttlPotJasaGrnd += Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString());
                ttlPotSparepartGrnd += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                ttlPotMaterialGrnd += Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString());
                ttlDPPGrnd += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                ttlPPNGrnd += Convert.ToDecimal(dt.Rows[i]["TotalPPNAmt"].ToString());
                ttlJualTotalGrnd += Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString());
                ttlJualCashGrnd += Convert.ToDecimal(dt.Rows[i]["Cash"].ToString());
                ttlJualCreditGrnd += Convert.ToDecimal(dt.Rows[i]["Credit"].ToString());
            }
            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail("-", 216, '-', false, true);
            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail("SubTotal", 13, ' ', true);
            gtf.SetDataDetail(invDate.ToString(), 13, ' ', true);
            gtf.SetDataDetailSpace(29);
            gtf.SetDataDetail(subttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["GroupHeader2Info"].ToString(), 13, ' ', true);
            gtf.SetDataDetailSpace(43);
            gtf.SetDataDetail(ttlNilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiSparepart.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotJasa.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotSparepart.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotMaterial.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPP.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPPN.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualTotal.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCash.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCredit.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(17);
            gtf.SetDataDetail("Grand Total", 13, ' ', true);
            gtf.SetDataDetailSpace(43);
            gtf.SetDataDetail(ttlNilaiJasaGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiSparepartGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiMaterialGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotJasaGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotSparepartGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotMaterialGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPPNGrnd.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualTotalGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCashGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJualCreditGrnd.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.SetDataDetail("-", 233, '-', false, true);
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport002(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {

            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W136", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            int year = Convert.ToInt32(dt.Rows[0]["PeriodYear"].ToString());
            int month = Convert.ToInt32(dt.Rows[0]["PeriodMonth"].ToString());

            string periode = string.Format("{0} {1} {2}", "Bulan", gtf.GetIndonesianMonth(month), year);
            string DppText = string.Format("{0} {1}", "Komulatif untuk tahun", year);

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode, 136, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("KETERANGAN", 20, ' ', true);
            gtf.SetGroupHeader("UNIT", 21, ' ', true, false, false, true);
            gtf.SetGroupHeader("NILAI PENJUALAN (DPP)", 35, ' ', true, false, false, true);
            gtf.SetGroupHeader("PERBANDINGAN NILAI PENJUALAN", 30, ' ', true, false, false, true);
            gtf.SetGroupHeader(DppText, 26, ' ', false, true);
            gtf.SetGroupHeaderSpace(21);
            gtf.SetGroupHeader("-", 21, '-', true);
            gtf.SetGroupHeader("-", 35, '-', true);
            gtf.SetGroupHeader("-", 30, '-', false, true);
            gtf.SetGroupHeaderSpace(21);
            gtf.SetGroupHeader("BULAN INI", 10, ' ', true, false, true);
            gtf.SetGroupHeader("BULAN LALU", 10, ' ', true, false, true);
            gtf.SetGroupHeader("BULAN INI", 17, ' ', true, false, true);
            gtf.SetGroupHeader("BULAN LALU", 17, ' ', true, false, true);
            gtf.SetGroupHeader("SELISIH", 17, ' ', true, false, true);
            gtf.SetGroupHeader("%", 12, ' ', true, false, true);
            gtf.SetGroupHeader("S/D BULAN INI", 26, ' ', false, true, true);

            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            decimal ttlUnitCurr = 0, ttlPrevCurr = 0, ttlDPPCurr = 0, ttlDPPPrev = 0, ttlRatioDiff = 0, ttlYtdOfDpp = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {

                gtf.SetDataDetail(dt.Rows[i]["Description"].ToString(), 20, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["UnitCurrMonth"].ToString(), 10, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["UnitPrevMonth"].ToString(), 10, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DPPCurrMonth"].ToString(), 17, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DPPPrevMonth"].ToString(), 17, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["RatioDiff"].ToString(), 17, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["RatioPct"].ToString(), 12, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["YtdOfDpp"].ToString(), 26, ' ', false, true, true, true, "n0");
                gtf.PrintData(false);

                ttlUnitCurr += dt.Rows[i]["UnitCurrMonth"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["UnitCurrMonth"].ToString()) : 0;
                ttlPrevCurr += dt.Rows[i]["UnitPrevMonth"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["UnitPrevMonth"].ToString()) : 0;
                ttlDPPCurr += dt.Rows[i]["DPPCurrMonth"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DPPCurrMonth"].ToString()) : 0;
                ttlDPPPrev += dt.Rows[i]["DPPPrevMonth"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DPPPrevMonth"].ToString()) : 0;
                ttlRatioDiff += dt.Rows[i]["RatioDiff"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["RatioDiff"].ToString()) : 0;
                ttlYtdOfDpp += dt.Rows[i]["YtdOfDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["YtdOfDpp"].ToString()) : 0;
            }

            if (dt.Rows[0]["Param"].ToString() != "1")
            {
                gtf.SetDataReportLine();
                gtf.SetDataDetail("TOTAL : ", 20, ' ', true);
                gtf.SetDataDetail(ttlUnitCurr.ToString(), 10, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlPrevCurr.ToString(), 10, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlDPPCurr.ToString(), 17, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlDPPPrev.ToString(), 17, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlRatioDiff.ToString(), 17, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(" ", 12, ' ', true);
                gtf.SetDataDetail(ttlYtdOfDpp.ToString(), 26, ' ', false, true, true, true, "n0");
                gtf.SetDataReportLine();
            }

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport003(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {

            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W163", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            int year = Convert.ToInt32(dt.Rows[0]["PeriodYear"].ToString());
            int month = Convert.ToInt32(dt.Rows[0]["PeriodMonth"].ToString());

            string periode = string.Format("{0} {1} {2}", "Bulan", gtf.GetIndonesianMonth(month), year);
            string DppText = string.Format("{0} {1}", "Komulatif untuk tahun", year);

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode, 136, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("KELOMPOK", 31, ' ', true);
            gtf.SetGroupHeader("BASIC", 15, ' ', true);
            gtf.SetGroupHeader("UNIT", 21, ' ', true, false, false, true);
            gtf.SetGroupHeader("NILAI PENJUALAN (DPP)", 35, ' ', true, false, false, true);
            gtf.SetGroupHeader("PERBANDINGAN NILAI PENJUALAN", 30, ' ', true, false, false, true);
            gtf.SetGroupHeader(DppText, 26, ' ', false, true);
            gtf.SetGroupHeaderSpace(48);
            gtf.SetGroupHeader("-", 21, '-', true);
            gtf.SetGroupHeader("-", 35, '-', true);
            gtf.SetGroupHeader("-", 30, '-', false, true);
            gtf.SetGroupHeader("JENIS PEKERJAAN", 31, ' ', true);
            gtf.SetGroupHeader("MODEL", 15, ' ', true);
            gtf.SetGroupHeader("BULAN INI", 10, ' ', true, false, true);
            gtf.SetGroupHeader("BULAN LALU", 10, ' ', true, false, true);
            gtf.SetGroupHeader("BULAN INI", 17, ' ', true, false, true);
            gtf.SetGroupHeader("BULAN LALU", 17, ' ', true, false, true);
            gtf.SetGroupHeader("SELISIH", 17, ' ', true, false, true);
            gtf.SetGroupHeader("%", 12, ' ', true, false, true);
            gtf.SetGroupHeader("S/D BULAN INI", 26, ' ', false, true, true);

            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "";
            decimal ttlUnitCurr = 0, ttlPrevCurr = 0, ttlDPPCurr = 0, ttlDPPPrev = 0, ttlRatioDiff = 0, ttlYtdOfDpp = 0;
            decimal ttlUnitCurrGrnd = 0, ttlPrevCurrGrnd = 0, ttlDPPCurrGrnd = 0, ttlDPPPrevGrnd = 0, ttlRatioDiffGrnd = 0, ttlYtdOfDppGrnd = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["OperationNo"].ToString();

                    gtf.SetDataDetail(dt.Rows[i]["OperationDesc"].ToString(), 31, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BasicModel"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["UnitCurrMonth"].ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["UnitPrevMonth"].ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DPPCurrMonth"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DPPPrevMonth"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["RatioDiff"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["RatioPct"].ToString(), 12, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["YtdOfDpp"].ToString(), 26, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }
                else if (docNo != dt.Rows[i]["OperationNo"].ToString())
                {
                    gtf.SetDataDetailSpace(32);
                    gtf.SetDataDetail("-", 131, '-', false, true);
                    gtf.SetDataDetailSpace(32);
                    gtf.SetDataDetail("TOTAL", 15, ' ', true);
                    gtf.SetDataDetail(ttlUnitCurr.ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlPrevCurr.ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDPPCurr.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDPPPrev.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlRatioDiff.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(" ", 12, ' ', true);
                    gtf.SetDataDetail(ttlYtdOfDpp.ToString(), 26, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    ttlUnitCurr = ttlPrevCurr = ttlDPPCurr = ttlDPPPrev = ttlRatioDiff = ttlYtdOfDpp = 0;

                    docNo = dt.Rows[i]["OperationNo"].ToString();
                    gtf.SetDataDetail(" ", 1, ' ', false, true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationDesc"].ToString(), 31, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["BasicModel"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["UnitCurrMonth"].ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["UnitPrevMonth"].ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DPPCurrMonth"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DPPPrevMonth"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["RatioDiff"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["RatioPct"].ToString(), 12, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["YtdOfDpp"].ToString(), 26, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }
                else if (docNo == dt.Rows[i]["OperationNo"].ToString())
                {
                    gtf.SetDataDetailSpace(32);
                    gtf.SetDataDetail(dt.Rows[i]["BasicModel"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["UnitCurrMonth"].ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["UnitPrevMonth"].ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DPPCurrMonth"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DPPPrevMonth"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["RatioDiff"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["RatioPct"].ToString(), 12, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["YtdOfDpp"].ToString(), 26, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                }

                ttlUnitCurr += dt.Rows[i]["UnitCurrMonth"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["UnitCurrMonth"].ToString()) : 0;
                ttlPrevCurr += dt.Rows[i]["UnitPrevMonth"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["UnitPrevMonth"].ToString()) : 0;
                ttlDPPCurr += dt.Rows[i]["DPPCurrMonth"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DPPCurrMonth"].ToString()) : 0;
                ttlDPPPrev += dt.Rows[i]["DPPPrevMonth"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DPPPrevMonth"].ToString()) : 0;
                ttlRatioDiff += dt.Rows[i]["RatioDiff"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["RatioDiff"].ToString()) : 0;
                ttlYtdOfDpp += dt.Rows[i]["YtdOfDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["YtdOfDpp"].ToString()) : 0;

                ttlUnitCurrGrnd += dt.Rows[i]["UnitCurrMonth"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["UnitCurrMonth"].ToString()) : 0;
                ttlPrevCurrGrnd += dt.Rows[i]["UnitPrevMonth"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["UnitPrevMonth"].ToString()) : 0;
                ttlDPPCurrGrnd += dt.Rows[i]["DPPCurrMonth"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DPPCurrMonth"].ToString()) : 0;
                ttlDPPPrevGrnd += dt.Rows[i]["DPPPrevMonth"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DPPPrevMonth"].ToString()) : 0;
                ttlRatioDiffGrnd += dt.Rows[i]["RatioDiff"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["RatioDiff"].ToString()) : 0;
                ttlYtdOfDppGrnd += dt.Rows[i]["YtdOfDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["YtdOfDpp"].ToString()) : 0;
            }
            gtf.SetDataDetailSpace(32);
            gtf.SetDataDetail("-", 131, '-', false, true);
            gtf.SetDataDetailSpace(32);
            gtf.SetDataDetail("TOTAL", 15, ' ', true);
            gtf.SetDataDetail(ttlUnitCurr.ToString(), 10, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPrevCurr.ToString(), 10, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPCurr.ToString(), 17, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPPrev.ToString(), 17, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlRatioDiff.ToString(), 17, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(" ", 12, ' ', true);
            gtf.SetDataDetail(ttlYtdOfDpp.ToString(), 26, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(32);
            gtf.SetDataDetail("-", 131, '-', false, true);

            gtf.SetDataDetailSpace(32);
            gtf.SetDataDetail("GRAND TOTAL", 15, ' ', true);
            gtf.SetDataDetail(ttlUnitCurrGrnd.ToString(), 10, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPrevCurrGrnd.ToString(), 10, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPCurrGrnd.ToString(), 17, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPPrevGrnd.ToString(), 17, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlRatioDiffGrnd.ToString(), 17, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(" ", 12, ' ', true);
            gtf.SetDataDetail(ttlYtdOfDppGrnd.ToString(), 26, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport004(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {

            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W272", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            string periode = string.Format("{0} {1}", "Tahun", dt.Rows[0]["PeriodYear"].ToString());

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode, 272, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("KETERANGAN", 48, ' ', true);
            gtf.SetGroupHeader("JANUARI", 16, ' ', true, false, true);
            gtf.SetGroupHeader("FEBRUARI", 16, ' ', true, false, true);
            gtf.SetGroupHeader("MARET", 16, ' ', true, false, true);
            gtf.SetGroupHeader("APRIL", 16, ' ', true, false, true);
            gtf.SetGroupHeader("MEI", 16, ' ', true, false, true);
            gtf.SetGroupHeader("JUNI", 16, ' ', true, false, true);
            gtf.SetGroupHeader("JULI", 16, ' ', true, false, true);
            gtf.SetGroupHeader("AUGSTUS", 16, ' ', true, false, true);
            gtf.SetGroupHeader("SEPTEMBER", 16, ' ', true, false, true);
            gtf.SetGroupHeader("OKTOBER", 16, ' ', true, false, true);
            gtf.SetGroupHeader("NOVEMBER", 16, ' ', true, false, true);
            gtf.SetGroupHeader("DESEMBER", 16, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 19, ' ', false, true, true);

            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            decimal ttlJan = 0, ttlFeb = 0, ttlMar = 0, ttlApr = 0, ttlMay = 0, ttlJun = 0, ttlJul = 0, ttlAgs = 0, ttlSep = 0, ttlOct = 0, ttlNov = 0, ttlDec = 0, ttlTotal = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                gtf.SetDataDetail(dt.Rows[i]["Description"].ToString(), 48, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["JanDpp"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["FebDpp"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["MarDpp"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["AprDpp"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["MayDpp"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["JunDpp"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["JulDpp"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["AugDpp"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["SepDpp"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["OctDpp"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["NovDpp"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DecDpp"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TotDpp"].ToString(), 19, ' ', false, true, true, true, "n0");
                gtf.PrintData(true);

                ttlJan = dt.Rows[i]["JanDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["JanDpp"].ToString()) : 0;
                ttlFeb = dt.Rows[i]["FebDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["FebDpp"].ToString()) : 0;
                ttlMar = dt.Rows[i]["MarDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["MarDpp"].ToString()) : 0;
                ttlApr = dt.Rows[i]["AprDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["AprDpp"].ToString()) : 0;
                ttlMay = dt.Rows[i]["MayDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["MayDpp"].ToString()) : 0;
                ttlJun = dt.Rows[i]["JunDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["JunDpp"].ToString()) : 0;
                ttlJul = dt.Rows[i]["JulDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["JulDpp"].ToString()) : 0;
                ttlAgs = dt.Rows[i]["AugDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["AugDpp"].ToString()) : 0;
                ttlSep = dt.Rows[i]["SepDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["SepDpp"].ToString()) : 0;
                ttlOct = dt.Rows[i]["OctDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["OctDpp"].ToString()) : 0;
                ttlNov = dt.Rows[i]["NovDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["NovDpp"].ToString()) : 0;
                ttlDec = dt.Rows[i]["DecDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DecDpp"].ToString()) : 0;
                ttlTotal = dt.Rows[i]["TotDpp"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["TotDpp"].ToString()) : 0;
            }
            if (dt.Rows[0]["param"].ToString() != "1")
            {
                gtf.SetDataReportLine();
                gtf.SetDataDetail("TOTAL", 48, ' ', true);
                gtf.SetDataDetail(ttlJan.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlFeb.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlMar.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlApr.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlMay.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlJun.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlJul.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlAgs.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlSep.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlOct.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlNov.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlDec.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(ttlTotal.ToString(), 19, ' ', false, true, true, true, "n0");
                gtf.SetDataReportLine();
                gtf.PrintData(true);
            }
            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport005(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W233", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header

            gtf.SetGroupHeader(string.Format("{0} {1}", "Tahun", dt.Rows[0]["PeriodYear"].ToString()), 233, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("KELOMPOK", 10, ' ', true);
            gtf.SetGroupHeader("BASIC", 15, ' ', true);
            gtf.SetGroupHeader("JANUARI", 15, ' ', true, false, true);
            gtf.SetGroupHeader("FEBRUARI", 15, ' ', true, false, true);
            gtf.SetGroupHeader("MARET", 15, ' ', true, false, true);
            gtf.SetGroupHeader("APRIL", 15, ' ', true, false, true);
            gtf.SetGroupHeader("MEI", 15, ' ', true, false, true);
            gtf.SetGroupHeader("JUNI", 15, ' ', true, false, true);
            gtf.SetGroupHeader("JULY", 15, ' ', true, false, true);
            gtf.SetGroupHeader("AGUSTUS", 15, ' ', true, false, true);
            gtf.SetGroupHeader("SEPTEMBER", 15, ' ', true, false, true);
            gtf.SetGroupHeader("OCTOBER", 15, ' ', true, false, true);
            gtf.SetGroupHeader("NOVEMBER", 15, ' ', true, false, true);
            gtf.SetGroupHeader("DECEMBER", 15, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 15, ' ', false, true, true);
            gtf.SetGroupHeader("PEKERJAAN", 10, ' ', true);
            gtf.SetGroupHeader("MODEL", 15, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "";
            decimal ttlJan = 0, ttlFeb = 0, ttlMar = 0, ttlApr = 0, ttlMay = 0, ttlJun = 0, ttlJul = 0, ttlAug = 0, ttlSep = 0, ttlOct = 0, ttlNov = 0, ttlDec = 0, ttlTotal = 0;
            decimal ttlJanGrnd = 0, ttlFebGrnd = 0, ttlMarGrnd = 0, ttlAprGrnd = 0, ttlMayGrnd = 0, ttlJunGrnd = 0, ttlJulGrnd = 0, ttlAugGrnd = 0, ttlSepGrnd = 0, ttlOctGrnd = 0, ttlNovGrnd = 0, ttlDecGrnd = 0, ttlTotalGrnd = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["OperationNo"].ToString();

                    gtf.SetDataDetail(dt.Rows[i]["OperationDesc"].ToString(), 233, ' ', false, true);

                    gtf.SetDataDetailSpace(11);
                    gtf.SetDataDetail(dt.Rows[i]["BasicModel"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JanDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["FebDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MarDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AprDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MayDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["JunDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["JulDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AugDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["SepDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["OctDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["NovDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DecDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotDpp"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }
                else if (docNo != dt.Rows[i]["OperationNo"].ToString())
                {
                    gtf.SetDataDetailSpace(11);
                    gtf.SetDataDetail("-", 222, '-', false, true);
                    gtf.SetDataDetailSpace(11);
                    gtf.SetDataDetail("TOTAL", 15, ' ', true);
                    gtf.SetDataDetail(ttlJan.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlFeb.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlMar.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlApr.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlMay.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlJun.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlJul.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlAug.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlSep.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlOct.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlNov.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDec.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlTotal.ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    ttlJan = ttlFeb = ttlMar = ttlApr = ttlMay = ttlJun = ttlJul = ttlAug = ttlSep = ttlOct = ttlNov = ttlDec = ttlTotal = 0;

                    docNo = dt.Rows[i]["OperationNo"].ToString();

                    gtf.SetDataDetail(" ", 1, ' ', false, true);

                    gtf.SetDataDetail(dt.Rows[i]["OperationDesc"].ToString(), 233, ' ', false, true);

                    gtf.SetDataDetailSpace(11);
                    gtf.SetDataDetail(dt.Rows[i]["BasicModel"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JanDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["FebDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MarDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AprDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MayDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["JunDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["JulDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AugDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["SepDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["OctDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["NovDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DecDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotDpp"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }
                else if (docNo == dt.Rows[i]["OperationNo"].ToString())
                {
                    gtf.SetDataDetailSpace(11);
                    gtf.SetDataDetail(dt.Rows[i]["BasicModel"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JanDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["FebDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MarDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AprDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["MayDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["JunDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["JulDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["AugDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["SepDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["OctDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["NovDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["DecDpp"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TotDpp"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                }

                ttlJan += decimal.Parse(dt.Rows[i]["JanDpp"].ToString());
                ttlFeb += decimal.Parse(dt.Rows[i]["FebDpp"].ToString());
                ttlMar += decimal.Parse(dt.Rows[i]["MarDpp"].ToString());
                ttlApr += decimal.Parse(dt.Rows[i]["AprDpp"].ToString());
                ttlMay += decimal.Parse(dt.Rows[i]["MayDpp"].ToString());
                ttlJun += decimal.Parse(dt.Rows[i]["JunDpp"].ToString());
                ttlJul += decimal.Parse(dt.Rows[i]["JulDpp"].ToString());
                ttlAug += decimal.Parse(dt.Rows[i]["AugDpp"].ToString());
                ttlSep += decimal.Parse(dt.Rows[i]["SepDpp"].ToString());
                ttlOct += decimal.Parse(dt.Rows[i]["OctDpp"].ToString());
                ttlNov += decimal.Parse(dt.Rows[i]["NovDpp"].ToString());
                ttlDec += decimal.Parse(dt.Rows[i]["DecDpp"].ToString());
                ttlTotal += decimal.Parse(dt.Rows[i]["TotDpp"].ToString());

                ttlJanGrnd += decimal.Parse(dt.Rows[i]["JanDpp"].ToString());
                ttlFebGrnd += decimal.Parse(dt.Rows[i]["FebDpp"].ToString());
                ttlMarGrnd += decimal.Parse(dt.Rows[i]["MarDpp"].ToString());
                ttlAprGrnd += decimal.Parse(dt.Rows[i]["AprDpp"].ToString());
                ttlMayGrnd += decimal.Parse(dt.Rows[i]["MayDpp"].ToString());
                ttlJunGrnd += decimal.Parse(dt.Rows[i]["JunDpp"].ToString());
                ttlJulGrnd += decimal.Parse(dt.Rows[i]["JulDpp"].ToString());
                ttlAugGrnd += decimal.Parse(dt.Rows[i]["AugDpp"].ToString());
                ttlSepGrnd += decimal.Parse(dt.Rows[i]["SepDpp"].ToString());
                ttlOctGrnd += decimal.Parse(dt.Rows[i]["OctDpp"].ToString());
                ttlNovGrnd += decimal.Parse(dt.Rows[i]["NovDpp"].ToString());
                ttlDecGrnd += decimal.Parse(dt.Rows[i]["DecDpp"].ToString());
                ttlTotalGrnd += decimal.Parse(dt.Rows[i]["TotDpp"].ToString());
            }
            gtf.SetDataDetailSpace(11);
            gtf.SetDataDetail("-", 222, '-', false, true);
            gtf.SetDataDetailSpace(11);
            gtf.SetDataDetail("TOTAL", 15, ' ', true);
            gtf.SetDataDetail(ttlJan.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlFeb.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlMar.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlApr.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlMay.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJun.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJul.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlAug.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlSep.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlOct.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNov.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDec.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlTotal.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(11);
            gtf.SetDataDetail("-", 222, '-', false, true);
            gtf.SetDataDetailSpace(11);
            gtf.SetDataDetail("GRAND TOTAL", 15, ' ', true);
            gtf.SetDataDetail(ttlJanGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlFebGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlMarGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlAprGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlMayGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJunGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJulGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlAugGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlSepGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlOctGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNovGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDecGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlTotalGrnd.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport006(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {

            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W272", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            string periode = "Periode Tahun : " + dt.Rows[0]["PeriodYear"].ToString();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode, 272, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("KETERANGAN", 31, ' ', true);
            gtf.SetGroupHeaderSpace(17);
            gtf.SetGroupHeader("JANUARI", 16, ' ', true, false, true);
            gtf.SetGroupHeader("FEBRUARI", 16, ' ', true, false, true);
            gtf.SetGroupHeader("MARET", 16, ' ', true, false, true);
            gtf.SetGroupHeader("APRIL", 16, ' ', true, false, true);
            gtf.SetGroupHeader("MEI", 16, ' ', true, false, true);
            gtf.SetGroupHeader("JUNI", 16, ' ', true, false, true);
            gtf.SetGroupHeader("JULI", 16, ' ', true, false, true);
            gtf.SetGroupHeader("AUGSTUS", 16, ' ', true, false, true);
            gtf.SetGroupHeader("SEPTEMBER", 16, ' ', true, false, true);
            gtf.SetGroupHeader("OKTOBER", 16, ' ', true, false, true);
            gtf.SetGroupHeader("NOVEMBER", 16, ' ', true, false, true);
            gtf.SetGroupHeader("DESEMBER", 16, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 19, ' ', false, true, true);

            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            decimal curr01 = 0, curr02 = 0, curr03 = 0, curr04 = 0, curr05 = 0, curr06 = 0, curr07 = 0, curr08 = 0, curr09 = 0, curr10 = 0, curr11 = 0, curr12 = 0, curr13 = 0;
            decimal prev01 = 0, prev02 = 0, prev03 = 0, prev04 = 0, prev05 = 0, prev06 = 0, prev07 = 0, prev08 = 0, prev09 = 0, prev10 = 0, prev11 = 0, prev12 = 0, prev13 = 0;
            decimal diff01 = 0, diff02 = 0, diff03 = 0, diff04 = 0, diff05 = 0, diff06 = 0, diff07 = 0, diff08 = 0, diff09 = 0, diff10 = 0, diff11 = 0, diff12 = 0, diff13 = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i != 0)
                    gtf.SetDataDetail(" ", 1, ' ', false, true);
                gtf.SetDataDetail(dt.Rows[i]["DESCR"].ToString(), 31, ' ', true);
                gtf.SetDataDetail("Tahun ini", 16, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["CURR01"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["CURR02"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["CURR03"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["CURR04"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["CURR05"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["CURR06"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["CURR07"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["CURR08"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["CURR09"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["CURR10"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["CURR11"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["CURR12"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["CURR13"].ToString(), 19, ' ', false, true, true, true, "n0");
                gtf.SetDataDetailSpace(32);
                gtf.SetDataDetail("Tahun lalu", 16, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["PREV01"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PREV02"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PREV03"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PREV04"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PREV05"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PREV06"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PREV07"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PREV08"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PREV09"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PREV10"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PREV11"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PREV12"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PREV13"].ToString(), 19, ' ', false, true, true, true, "n0");
                gtf.SetDataDetailSpace(32);
                gtf.SetDataDetail("-", 240, '-', false, true);
                gtf.SetDataDetailSpace(32);
                gtf.SetDataDetail("SELISIH", 16, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["DIFF01"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DIFF02"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DIFF03"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DIFF04"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DIFF05"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DIFF06"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DIFF07"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DIFF08"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DIFF09"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DIFF10"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DIFF11"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DIFF12"].ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["DIFF13"].ToString(), 19, ' ', false, true, true, true, "n0");

                gtf.PrintData(false);

                curr01 += dt.Rows[i]["CURR01"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR01"].ToString()) : 0;
                curr02 += dt.Rows[i]["CURR02"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR02"].ToString()) : 0;
                curr03 += dt.Rows[i]["CURR03"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR03"].ToString()) : 0;
                curr04 += dt.Rows[i]["CURR04"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR04"].ToString()) : 0;
                curr05 += dt.Rows[i]["CURR05"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR05"].ToString()) : 0;
                curr06 += dt.Rows[i]["CURR06"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR06"].ToString()) : 0;
                curr07 += dt.Rows[i]["CURR07"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR07"].ToString()) : 0;
                curr08 += dt.Rows[i]["CURR08"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR08"].ToString()) : 0;
                curr09 += dt.Rows[i]["CURR09"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR09"].ToString()) : 0;
                curr10 += dt.Rows[i]["CURR10"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR10"].ToString()) : 0;
                curr11 += dt.Rows[i]["CURR11"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR11"].ToString()) : 0;
                curr12 += dt.Rows[i]["CURR12"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR12"].ToString()) : 0;
                curr13 += dt.Rows[i]["CURR13"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["CURR13"].ToString()) : 0;

                prev01 += dt.Rows[i]["PREV01"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV01"].ToString()) : 0;
                prev02 += dt.Rows[i]["PREV02"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV02"].ToString()) : 0;
                prev03 += dt.Rows[i]["PREV03"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV03"].ToString()) : 0;
                prev04 += dt.Rows[i]["PREV04"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV04"].ToString()) : 0;
                prev05 += dt.Rows[i]["PREV05"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV05"].ToString()) : 0;
                prev06 += dt.Rows[i]["PREV06"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV06"].ToString()) : 0;
                prev07 += dt.Rows[i]["PREV07"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV07"].ToString()) : 0;
                prev08 += dt.Rows[i]["PREV08"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV08"].ToString()) : 0;
                prev09 += dt.Rows[i]["PREV09"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV09"].ToString()) : 0;
                prev10 += dt.Rows[i]["PREV10"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV10"].ToString()) : 0;
                prev11 += dt.Rows[i]["PREV11"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV11"].ToString()) : 0;
                prev12 += dt.Rows[i]["PREV12"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV12"].ToString()) : 0;
                prev13 += dt.Rows[i]["PREV13"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PREV13"].ToString()) : 0;

                diff01 += dt.Rows[i]["DIFF01"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF01"].ToString()) : 0;
                diff02 += dt.Rows[i]["DIFF02"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF02"].ToString()) : 0;
                diff03 += dt.Rows[i]["DIFF03"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF03"].ToString()) : 0;
                diff04 += dt.Rows[i]["DIFF04"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF04"].ToString()) : 0;
                diff05 += dt.Rows[i]["DIFF05"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF05"].ToString()) : 0;
                diff06 += dt.Rows[i]["DIFF06"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF06"].ToString()) : 0;
                diff07 += dt.Rows[i]["DIFF07"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF07"].ToString()) : 0;
                diff08 += dt.Rows[i]["DIFF08"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF08"].ToString()) : 0;
                diff09 += dt.Rows[i]["DIFF09"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF09"].ToString()) : 0;
                diff10 += dt.Rows[i]["DIFF10"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF10"].ToString()) : 0;
                diff11 += dt.Rows[i]["DIFF11"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF11"].ToString()) : 0;
                diff12 += dt.Rows[i]["DIFF12"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF12"].ToString()) : 0;
                diff13 += dt.Rows[i]["DIFF13"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["DIFF13"].ToString()) : 0;
            }

            if (dt.Rows[0]["param"].ToString() != "1")
            {
                gtf.SetDataDetail("GRAND TOTAL :", 31, ' ', true);
                gtf.SetDataDetail("Tahun ini", 16, ' ', true);
                gtf.SetDataDetail(curr01.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(curr02.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(curr03.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(curr04.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(curr05.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(curr06.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(curr07.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(curr08.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(curr09.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(curr10.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(curr11.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(curr12.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(curr13.ToString(), 19, ' ', false, true, true, true, "n0");
                gtf.SetDataDetailSpace(32);
                gtf.SetDataDetail("Tahun lalu", 16, ' ', true);
                gtf.SetDataDetail(prev01.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(prev02.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(prev03.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(prev04.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(prev05.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(prev06.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(prev07.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(prev08.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(prev09.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(prev10.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(prev11.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(prev12.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(prev13.ToString(), 19, ' ', false, true, true, true, "n0");
                gtf.SetDataDetailSpace(32);
                gtf.SetDataDetail("-", 240, '-', false, true);
                gtf.SetDataDetailSpace(32);
                gtf.SetDataDetail("SELISIH", 16, ' ', true);
                gtf.SetDataDetail(diff01.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(diff02.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(diff03.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(diff04.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(diff05.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(diff06.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(diff07.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(diff08.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(diff09.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(diff10.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(diff11.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(diff12.ToString(), 16, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(diff13.ToString(), 19, ' ', false, true, true, true, "n0");

                gtf.PrintData(false);
            }

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport007(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {

            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W233", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            DateTime startDate = Convert.ToDateTime(dt.Rows[0]["StartDate"].ToString());
            DateTime endDate = Convert.ToDateTime(dt.Rows[0]["EndDate"].ToString());

            string periode = "Periode : " + startDate.ToString("dd-MMM-yyyy") + " s/d " + endDate.ToString("dd-MMM-yyyy");

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode, 233, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("JENIS PEKERJAAN", 54, ' ', true);
            gtf.SetGroupHeader("NILAI", 62, ' ', true, false, false, true);
            gtf.SetGroupHeader("POTONGAN", 47, ' ', true, false, false, true);
            gtf.SetGroupHeader("DPP", 20, ' ', true, false, true);
            gtf.SetGroupHeader("PPN", 20, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 25, ' ', false, true, true);
            gtf.SetGroupHeaderSpace(55);
            gtf.SetGroupHeader("-", 62, '-', true);
            gtf.SetGroupHeader("-", 47, '-', false, true);
            gtf.SetGroupHeaderSpace(55);
            gtf.SetGroupHeader("JASA", 20, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 20, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 20, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 15, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 15, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 15, ' ', true, false, true);
            gtf.SetGroupHeaderSpace(42);
            gtf.SetGroupHeader("PENJUALAN", 25, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            decimal ttlNilaiJasa = 0, ttlNilaiSpare = 0, ttlNilaiMaterial = 0, ttlPotJasa = 0, ttlPotSpare = 0, ttlPotMaterial = 0, ttlDPP = 0, ttlPPN = 0, ttlTotal = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                gtf.SetDataDetail(dt.Rows[i]["JobType"].ToString(), 54, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 20, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 20, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 20, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 20, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TotalPPnAmt"].ToString(), 20, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmt"].ToString(), 25, ' ', false, true, true, true, "n0");
                gtf.PrintData(false);

                ttlNilaiJasa += dt.Rows[i]["LaborGrossAmt"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["LaborGrossAmt"].ToString()) : 0;
                ttlNilaiSpare += dt.Rows[i]["PartsGrossAmt"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PartsGrossAmt"].ToString()) : 0;
                ttlNilaiMaterial += dt.Rows[i]["MaterialGrossAmt"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["MaterialGrossAmt"].ToString()) : 0;
                ttlPotJasa += dt.Rows[i]["LaborDiscAmt"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["LaborDiscAmt"].ToString()) : 0;
                ttlPotSpare += dt.Rows[i]["PartsDiscAmt"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString()) : 0;
                ttlPotMaterial += dt.Rows[i]["MaterialDiscAmt"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["MaterialDiscAmt"].ToString()) : 0;
                ttlDPP += dt.Rows[i]["TotalDPPAmt"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString()) : 0;
                ttlPPN += dt.Rows[i]["TotalPPnAmt"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["TotalPPnAmt"].ToString()) : 0;
                ttlTotal += dt.Rows[i]["TotalSrvAmt"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["TotalSrvAmt"].ToString()) : 0;
            }
            gtf.SetDataReportLine();
            gtf.SetDataDetail("GRAND TOTAL :", 54, ' ', true, false, true);
            gtf.SetDataDetail(ttlNilaiJasa.ToString(), 20, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiSpare.ToString(), 20, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNilaiMaterial.ToString(), 20, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotSpare.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPP.ToString(), 20, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPPN.ToString(), 20, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlTotal.ToString(), 25, ' ', false, true, true, true, "n0");
            gtf.SetDataReportLine();
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport010MK(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W233", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            string periode = string.Format("Periode : {0} s/d {1}", Convert.ToDateTime(setTextParameter[0].ToString()).ToString("dd-MMM-yyy"), Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyy"));

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode.ToUpper(), 233, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NIK", 7, ' ', true);
            gtf.SetGroupHeader("NAMA KARYAWAN", 34, ' ', true);
            gtf.SetGroupHeader("NO. SPK", 18, ' ', true);
            gtf.SetGroupHeader("NO. FAKTUR", 19, ' ', true);
            gtf.SetGroupHeader("TGL. FAKTUR", 15, ' ', true);
            gtf.SetGroupHeader("FRT", 15, ' ', true);
            gtf.SetGroupHeader("N.K", 15, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 19, ' ', true, false, true);
            gtf.SetGroupHeader("POTONGAN", 15, ' ', true, false, true);
            gtf.SetGroupHeader("DPP", 19, ' ', true, false, true);
            gtf.SetGroupHeader("KETERANGAN", 41, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string mechanicId = "", docNo = "";
            int noUrut = 0;

            decimal ttlNK = 0, ttlJasa = 0, ttlPotonganJasa = 0, ttlDPPJasa = 0;
            decimal subttlNK = 0, subttlJasa = 0, subttlPotonganJasa = 0, subttlDPPJasa = 0;
            decimal ttlNKGrnd = 0, ttlJasaGrnd = 0, ttlPotonganJasaGrnd = 0, ttlDPPJasaGrnd = 0;
            decimal nilaiNk = 0, nilaiJasa = 0, nilaiPotongan = 0;
            decimal nilaiDPP = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (mechanicId == "")
                {
                    mechanicId = dt.Rows[i]["MechanicID"].ToString();
                    docNo = dt.Rows[i]["InvoiceNo"].ToString();
                    noUrut++;

                    gtf.SetDataDetail(dt.Rows[i]["MechanicID"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["MechanicName"].ToString(), 34, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 18, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 19, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 15, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 19, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 19, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDescription"].ToString(), 41, ' ', false, true);
                    gtf.PrintData();

                    nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                    nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                    nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                    nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                }
                else if (mechanicId == dt.Rows[i]["MechanicID"].ToString())
                {
                    if (docNo != dt.Rows[i]["InvoiceNo"].ToString())
                    {
                        subttlNK += nilaiNk;
                        subttlJasa += nilaiJasa;
                        subttlPotonganJasa += nilaiPotongan;
                        subttlDPPJasa += nilaiDPP;

                        ttlNK += subttlNK;
                        ttlJasa += subttlJasa;
                        ttlPotonganJasa += subttlPotonganJasa;
                        ttlDPPJasa += subttlDPPJasa;
                        noUrut++;
                        docNo = dt.Rows[i]["InvoiceNo"].ToString();

                        gtf.SetDataDetailSpace(43);
                        gtf.SetDataDetail("-", 190, '-', false, true);
                        gtf.SetDataDetailSpace(61);
                        gtf.SetDataDetail("SubTotal :", 52, ' ', true, false, true);
                        gtf.SetDataDetail(subttlNK.ToString(), 15, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(subttlJasa.ToString(), 19, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(subttlPotonganJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(subttlDPPJasa.ToString(), 19, ' ', false, true, true, true, "n0");

                        gtf.PrintData();
                        nilaiNk = nilaiJasa = nilaiPotongan = nilaiDPP = 0;
                        subttlNK = subttlJasa = subttlPotonganJasa = subttlDPPJasa = 0;

                        gtf.SetDataDetailSpace(43);

                        gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 18, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 19, ' ', true);
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 15, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["OperationNo"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 15, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 19, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 19, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDescription"].ToString(), 41, ' ', false, true);
                        gtf.PrintData();

                        nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                        nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                        nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                        nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                        noUrut++;
                    }
                    else
                    {
                        if (noUrut == 0)
                        {
                            gtf.SetDataDetail(dt.Rows[i]["MechanicID"].ToString(), 7, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["MechanicName"].ToString(), 34, ' ', true);
                        }
                        else
                            gtf.SetDataDetailSpace(43);

                        gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 18, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 19, ' ', true);
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 15, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["OperationNo"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 15, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 19, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 19, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDescription"].ToString(), 41, ' ', false, true);
                        gtf.PrintData();

                        nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                        nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                        nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                        nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                        noUrut++;
                    }
                }
                else
                {
                    noUrut = 0;
                    subttlNK += nilaiNk;
                    subttlJasa += nilaiJasa;
                    subttlPotonganJasa += nilaiPotongan;
                    subttlDPPJasa += nilaiDPP;

                    ttlNK += subttlNK;
                    ttlJasa += subttlJasa;
                    ttlPotonganJasa += subttlPotonganJasa;
                    ttlDPPJasa += subttlDPPJasa;

                    ttlNKGrnd += ttlNK;
                    ttlJasaGrnd += ttlJasa;
                    ttlPotonganJasaGrnd += ttlPotonganJasa;
                    ttlDPPJasaGrnd += ttlDPPJasa;

                    docNo = dt.Rows[i]["InvoiceNo"].ToString();
                    mechanicId = dt.Rows[i]["MechanicID"].ToString();

                    gtf.SetDataDetailSpace(43);
                    gtf.SetDataDetail("-", 190, '-', false, true);
                    gtf.SetDataDetailSpace(61);
                    gtf.SetDataDetail("SubTotal :", 52, ' ', true, false, true);
                    gtf.SetDataDetail(subttlNK.ToString(), 15, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(subttlJasa.ToString(), 19, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotonganJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPPJasa.ToString(), 19, ' ', false, true, true, true, "n0");

                    gtf.PrintData();

                    gtf.SetDataDetailSpace(43);
                    gtf.SetDataDetail("-", 190, '-', false, true);
                    gtf.SetDataDetailSpace(61);
                    gtf.SetDataDetail("TOTAL :", 52, ' ', true, false, true);
                    gtf.SetDataDetail(ttlNK.ToString(), 15, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(ttlJasa.ToString(), 19, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlPotonganJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDPPJasa.ToString(), 19, ' ', false, true, true, true, "n0");

                    //gtf.PrintData(false, true);

                    nilaiNk = nilaiJasa = nilaiPotongan = nilaiDPP = 0;
                    subttlNK = subttlJasa = subttlPotonganJasa = subttlDPPJasa = 0;
                    ttlNK = ttlJasa = ttlPotonganJasa = ttlDPPJasa = 0;

                    gtf.SetDataDetail(dt.Rows[i]["MechanicID"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["MechanicName"].ToString(), 34, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 18, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 19, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 15, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 19, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 19, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDescription"].ToString(), 41, ' ', false, true);
                    gtf.PrintData();

                    nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                    nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                    nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                    nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                    noUrut++;
                }

                if (i == dt.Rows.Count - 1)
                {
                    subttlNK += nilaiNk;
                    subttlJasa += nilaiJasa;
                    subttlPotonganJasa += nilaiPotongan;
                    subttlDPPJasa += nilaiDPP;

                    ttlNK += subttlNK;
                    ttlJasa += subttlJasa;
                    ttlPotonganJasa += subttlPotonganJasa;
                    ttlDPPJasa += subttlDPPJasa;

                    ttlNKGrnd += ttlNK;
                    ttlJasaGrnd += ttlJasa;
                    ttlPotonganJasaGrnd += ttlPotonganJasa;
                    ttlDPPJasaGrnd += ttlDPPJasa;

                    gtf.SetDataDetailSpace(43);
                    gtf.SetDataDetail("-", 190, '-', false, true);
                    gtf.SetDataDetailSpace(61);
                    gtf.SetDataDetail("SubTotal :", 52, ' ', true, false, true);
                    gtf.SetDataDetail(subttlNK.ToString(), 15, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(subttlJasa.ToString(), 19, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotonganJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPPJasa.ToString(), 19, ' ', false, true, true, true, "n0");

                    gtf.PrintData();

                    gtf.SetDataDetailSpace(43);
                    gtf.SetDataDetail("-", 190, '-', false, true);
                    gtf.SetDataDetailSpace(61);
                    gtf.SetDataDetail("TOTAL :", 52, ' ', true, false, true);
                    gtf.SetDataDetail(ttlNK.ToString(), 15, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(ttlJasa.ToString(), 19, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlPotonganJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDPPJasa.ToString(), 19, ' ', false, true, true, true, "n0");

                    gtf.PrintData();
                }
            }
            gtf.SetDataReportLine();
            gtf.SetDataDetailSpace(61);
            gtf.SetDataDetail("GRAND TOTAL :", 52, ' ', true, false, true);
            gtf.SetDataDetail(ttlNKGrnd.ToString(), 15, ' ', true, false, true, true, "n2");
            gtf.SetDataDetail(ttlJasaGrnd.ToString(), 19, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotonganJasaGrnd.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPJasaGrnd.ToString(), 19, ' ', true, false, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport010SA(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W233", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            string periode = string.Format("Periode : {0} s/d {1}", Convert.ToDateTime(setTextParameter[0].ToString()).ToString("dd-MMM-yyy"), Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyy"));

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode.ToUpper(), 233, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NIK", 7, ' ', true);
            gtf.SetGroupHeader("NAMA KARYAWAN", 34, ' ', true);
            gtf.SetGroupHeader("NO. FAKTUR", 15, ' ', true);
            gtf.SetGroupHeader("TGL. FAKTUR", 14, ' ', true);
            gtf.SetGroupHeader("FRT", 11, ' ', true);
            gtf.SetGroupHeader("N.K", 13, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 17, ' ', true, false, true);
            gtf.SetGroupHeader("POTONGAN", 13, ' ', true, false, true);
            gtf.SetGroupHeader("DPP", 17, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 17, ' ', true, false, true);
            gtf.SetGroupHeader("POTONGAN", 13, ' ', true, false, true);
            gtf.SetGroupHeader("DPP", 17, ' ', true, false, true);
            gtf.SetGroupHeader("KETERANGAN", 47, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string saId = "", docNo = "";
            int noUrut = 0;

            decimal nilaiNk = 0, nilaiJasa = 0, nilaiPotongan = 0, nilaiDPP = 0, nilaiParts = 0, nilaiDiscParts = 0, nilaiDPPParts = 0;
            decimal subttlNK = 0, subttlJasa = 0, subttlPotonganJasa = 0, subttlDPPJasa = 0, subttlParts = 0, subttlDiscParts = 0, subttlDPPParts = 0;
            decimal ttlNK = 0, ttlJasa = 0, ttlPotonganJasa = 0, ttlDPPJasa = 0, ttlParts = 0, ttlDiscParts = 0, ttlDPPParts = 0;
            decimal ttlNKGrnd = 0, ttlJasaGrnd = 0, ttlPotonganJasaGrnd = 0, ttlDPPJasaGrnd = 0, ttlPartsGrnd = 0, ttlDiscPartsGrnd = 0, ttlDPPPartsGrnd = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (saId == "")
                {
                    noUrut++;
                    saId = dt.Rows[i]["SaID"].ToString();
                    docNo = dt.Rows[i]["InvoiceNo"].ToString();

                    gtf.SetDataDetail(dt.Rows[i]["SaID"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SaName"].ToString(), 34, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationNo"].ToString(), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 13, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDescription"].ToString(), 47, ' ', false, true);
                    gtf.PrintData();

                    nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                    nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                    nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                    nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                    nilaiParts += Convert.ToDecimal(dt.Rows[i]["PartsAmt"].ToString());
                    nilaiDiscParts += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                    nilaiDPPParts += Convert.ToDecimal(dt.Rows[i]["PartsDPPAmt"].ToString());
                }
                else if (saId == dt.Rows[i]["SaID"].ToString())
                {
                    if (docNo != dt.Rows[i]["InvoiceNo"].ToString())
                    {
                        subttlNK += nilaiNk;
                        subttlJasa += nilaiJasa;
                        subttlPotonganJasa += nilaiPotongan;
                        subttlDPPJasa += nilaiDPP;
                        subttlParts += nilaiParts;
                        subttlDiscParts += nilaiDiscParts;
                        subttlDPPParts += nilaiDPPParts;

                        ttlNK += subttlNK;
                        ttlJasa += subttlJasa;
                        ttlPotonganJasa += subttlPotonganJasa;
                        ttlDPPJasa += subttlDPPJasa;
                        ttlParts += subttlParts;
                        ttlDiscParts += subttlDiscParts;
                        ttlDPPParts += subttlDPPParts;

                        noUrut++;
                        docNo = dt.Rows[i]["InvoiceNo"].ToString();

                        gtf.SetDataDetailSpace(43);
                        gtf.SetDataDetail("-", 190, '-', false, true);
                        gtf.SetDataDetailSpace(74);
                        gtf.SetDataDetail("SubTotal :", 11, ' ', true, false, true);
                        gtf.SetDataDetail(subttlNK.ToString(), 13, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(subttlJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(subttlPotonganJasa.ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(subttlDPPJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(subttlParts.ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(subttlDiscParts.ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(subttlDPPParts.ToString(), 17, ' ', false, true, true, true, "n0");

                        gtf.PrintData();
                        nilaiNk = nilaiJasa = nilaiPotongan = nilaiDPP = nilaiParts = nilaiDiscParts = nilaiDPPParts = 0;
                        subttlNK = subttlJasa = subttlPotonganJasa = subttlDPPJasa = subttlParts = subttlDiscParts = subttlDPPParts = 0;

                        gtf.SetDataDetailSpace(43);

                        gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 14, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["OperationNo"].ToString(), 11, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 13, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["PartsAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["PartsDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDescription"].ToString(), 47, ' ', false, true);
                        gtf.PrintData();

                        nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                        nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                        nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                        nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                        nilaiParts += Convert.ToDecimal(dt.Rows[i]["PartsAmt"].ToString());
                        nilaiDiscParts += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                        nilaiDPPParts += Convert.ToDecimal(dt.Rows[i]["PartsDPPAmt"].ToString());
                        noUrut++;
                    }
                    else
                    {
                        if (noUrut == 0)
                        {
                            gtf.SetDataDetail(dt.Rows[i]["SaID"].ToString(), 7, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["SaName"].ToString(), 34, ' ', true);
                        }
                        else
                            gtf.SetDataDetailSpace(43);

                        gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 14, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["OperationNo"].ToString(), 11, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 13, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["PartsAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["PartsDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDescription"].ToString(), 47, ' ', false, true);
                        gtf.PrintData();

                        nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                        nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                        nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                        nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                        nilaiParts += Convert.ToDecimal(dt.Rows[i]["PartsAmt"].ToString());
                        nilaiDiscParts += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                        nilaiDPPParts += Convert.ToDecimal(dt.Rows[i]["PartsDPPAmt"].ToString());
                        noUrut++;
                    }
                }
                else
                {
                    noUrut = 0;
                    subttlNK += nilaiNk;
                    subttlJasa += nilaiJasa;
                    subttlPotonganJasa += nilaiPotongan;
                    subttlDPPJasa += nilaiDPP;
                    subttlParts += nilaiParts;
                    subttlDiscParts += nilaiDiscParts;
                    subttlDPPParts += nilaiDPPParts;

                    ttlNK += subttlNK;
                    ttlJasa += subttlJasa;
                    ttlPotonganJasa += subttlPotonganJasa;
                    ttlDPPJasa += subttlDPPJasa;
                    ttlParts += subttlParts;
                    ttlDiscParts += subttlDiscParts;
                    ttlDPPParts += subttlDPPParts;

                    ttlNKGrnd += ttlNK;
                    ttlJasaGrnd += ttlJasa;
                    ttlPotonganJasaGrnd += ttlPotonganJasa;
                    ttlDPPJasaGrnd += ttlDPPJasa;
                    ttlPartsGrnd += ttlParts;
                    ttlDiscPartsGrnd += ttlDiscParts;
                    ttlDPPPartsGrnd += ttlDPPParts;

                    docNo = dt.Rows[i]["InvoiceNo"].ToString();
                    saId = dt.Rows[i]["SaID"].ToString();

                    gtf.SetDataDetailSpace(43);
                    gtf.SetDataDetail("-", 190, '-', false, true);
                    gtf.SetDataDetailSpace(74);
                    gtf.SetDataDetail("SubTotal :", 11, ' ', true, false, true);
                    gtf.SetDataDetail(subttlNK.ToString(), 13, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(subttlJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotonganJasa.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPPJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlParts.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDiscParts.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPPParts.ToString(), 17, ' ', false, true, true, true, "n0");

                    gtf.PrintData();

                    gtf.SetDataDetailSpace(43);
                    gtf.SetDataDetail("-", 190, '-', false, true);
                    gtf.SetDataDetailSpace(74);
                    gtf.SetDataDetail("TOTAL :", 11, ' ', true, false, true);
                    gtf.SetDataDetail(ttlNK.ToString(), 13, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(ttlJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlPotonganJasa.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDPPJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlParts.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDiscParts.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDPPParts.ToString(), 17, ' ', false, true, true, true, "n0");

                    //gtf.PrintData(false, true);

                    nilaiNk = nilaiJasa = nilaiPotongan = nilaiDPP = nilaiParts = nilaiDiscParts = nilaiDPPParts = 0;
                    subttlNK = subttlJasa = subttlPotonganJasa = subttlDPPJasa = subttlParts = subttlDiscParts = subttlDPPParts = 0;
                    ttlNK = ttlJasa = ttlPotonganJasa = ttlDPPJasa = ttlParts = ttlDiscParts = ttlDPPParts = 0;

                    gtf.SetDataDetail(dt.Rows[i]["SaID"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["SaName"].ToString(), 34, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationNo"].ToString(), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 13, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDescription"].ToString(), 47, ' ', false, true);
                    gtf.PrintData();

                    nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                    nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                    nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                    nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                    nilaiParts += Convert.ToDecimal(dt.Rows[i]["PartsAmt"].ToString());
                    nilaiDiscParts += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                    nilaiDPPParts += Convert.ToDecimal(dt.Rows[i]["PartsDPPAmt"].ToString());
                    noUrut++;
                }

                if (i == dt.Rows.Count - 1)
                {
                    subttlNK += nilaiNk;
                    subttlJasa += nilaiJasa;
                    subttlPotonganJasa += nilaiPotongan;
                    subttlDPPJasa += nilaiDPP;
                    subttlParts += nilaiParts;
                    subttlDiscParts += nilaiDiscParts;
                    subttlDPPParts += nilaiDPPParts;

                    ttlNK += subttlNK;
                    ttlJasa += subttlJasa;
                    ttlPotonganJasa += subttlPotonganJasa;
                    ttlDPPJasa += subttlDPPJasa;
                    ttlParts += subttlParts;
                    ttlDiscParts += subttlDiscParts;
                    ttlDPPParts += subttlDPPParts;

                    ttlNKGrnd += ttlNK;
                    ttlJasaGrnd += ttlJasa;
                    ttlPotonganJasaGrnd += ttlPotonganJasa;
                    ttlDPPJasaGrnd += ttlDPPJasa;
                    ttlPartsGrnd += ttlParts;
                    ttlDiscPartsGrnd += ttlDiscParts;
                    ttlDPPPartsGrnd += ttlDPPParts;

                    gtf.SetDataDetailSpace(43);
                    gtf.SetDataDetail("-", 190, '-', false, true);
                    gtf.SetDataDetailSpace(74);
                    gtf.SetDataDetail("SubTotal :", 11, ' ', true, false, true);
                    gtf.SetDataDetail(subttlNK.ToString(), 13, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(subttlJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotonganJasa.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPPJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlParts.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDiscParts.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPPParts.ToString(), 17, ' ', false, true, true, true, "n0");

                    gtf.PrintData();

                    gtf.SetDataDetailSpace(43);
                    gtf.SetDataDetail("-", 190, '-', false, true);
                    gtf.SetDataDetailSpace(74);
                    gtf.SetDataDetail("TOTAL :", 11, ' ', true, false, true);
                    gtf.SetDataDetail(ttlNK.ToString(), 13, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(ttlJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlPotonganJasa.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDPPJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlParts.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDiscParts.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDPPParts.ToString(), 17, ' ', false, true, true, true, "n0");
                    gtf.PrintData();
                }
            }
            gtf.SetDataReportLine();
            gtf.SetDataDetailSpace(72);
            gtf.SetDataDetail("GRAND TOTAL :", 13, ' ', true, false, true);
            gtf.SetDataDetail(ttlNKGrnd.ToString(), 13, ' ', true, false, true, true, "n2");
            gtf.SetDataDetail(ttlJasaGrnd.ToString(), 17, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotonganJasaGrnd.ToString(), 13, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPJasaGrnd.ToString(), 17, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPartsGrnd.ToString(), 17, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDiscPartsGrnd.ToString(), 13, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPPartsGrnd.ToString(), 17, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport010FM(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W233", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            string periode = string.Format("Periode : {0} s/d {1}", Convert.ToDateTime(setTextParameter[0].ToString()).ToString("dd-MMM-yyy"), Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyy"));

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode.ToUpper(), 233, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NIK", 7, ' ', true);
            gtf.SetGroupHeader("NAMA KARYAWAN", 34, ' ', true);
            gtf.SetGroupHeader("NO. FAKTUR", 15, ' ', true);
            gtf.SetGroupHeader("TGL. FAKTUR", 14, ' ', true);
            gtf.SetGroupHeader("FRT", 11, ' ', true);
            gtf.SetGroupHeader("N.K", 13, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 17, ' ', true, false, true);
            gtf.SetGroupHeader("POTONGAN", 13, ' ', true, false, true);
            gtf.SetGroupHeader("DPP", 17, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 17, ' ', true, false, true);
            gtf.SetGroupHeader("POTONGAN", 13, ' ', true, false, true);
            gtf.SetGroupHeader("DPP", 17, ' ', true, false, true);
            gtf.SetGroupHeader("KETERANGAN", 47, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string fmId = "", docNo = "";
            int noUrut = 0;

            decimal nilaiNk = 0, nilaiJasa = 0, nilaiPotongan = 0, nilaiDPP = 0, nilaiParts = 0, nilaiDiscParts = 0, nilaiDPPParts = 0;
            decimal subttlNK = 0, subttlJasa = 0, subttlPotonganJasa = 0, subttlDPPJasa = 0, subttlParts = 0, subttlDiscParts = 0, subttlDPPParts = 0;
            decimal ttlNK = 0, ttlJasa = 0, ttlPotonganJasa = 0, ttlDPPJasa = 0, ttlParts = 0, ttlDiscParts = 0, ttlDPPParts = 0;
            decimal ttlNKGrnd = 0, ttlJasaGrnd = 0, ttlPotonganJasaGrnd = 0, ttlDPPJasaGrnd = 0, ttlPartsGrnd = 0, ttlDiscPartsGrnd = 0, ttlDPPPartsGrnd = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (fmId == "")
                {
                    noUrut++;
                    fmId = dt.Rows[i]["FmID"].ToString();
                    docNo = dt.Rows[i]["InvoiceNo"].ToString();

                    gtf.SetDataDetail(dt.Rows[i]["FmID"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FmName"].ToString(), 34, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationNo"].ToString(), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 13, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDescription"].ToString(), 47, ' ', false, true);
                    gtf.PrintData();

                    nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                    nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                    nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                    nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                    nilaiParts += Convert.ToDecimal(dt.Rows[i]["PartsAmt"].ToString());
                    nilaiDiscParts += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                    nilaiDPPParts += Convert.ToDecimal(dt.Rows[i]["PartsDPPAmt"].ToString());
                }
                else if (fmId == dt.Rows[i]["FmID"].ToString())
                {
                    if (docNo != dt.Rows[i]["InvoiceNo"].ToString())
                    {
                        subttlNK += nilaiNk;
                        subttlJasa += nilaiJasa;
                        subttlPotonganJasa += nilaiPotongan;
                        subttlDPPJasa += nilaiDPP;
                        subttlParts += nilaiParts;
                        subttlDiscParts += nilaiDiscParts;
                        subttlDPPParts += nilaiDPPParts;

                        ttlNK += subttlNK;
                        ttlJasa += subttlJasa;
                        ttlPotonganJasa += subttlPotonganJasa;
                        ttlDPPJasa += subttlDPPJasa;
                        ttlParts += subttlParts;
                        ttlDiscParts += subttlDiscParts;
                        ttlDPPParts += subttlDPPParts;

                        noUrut++;
                        docNo = dt.Rows[i]["InvoiceNo"].ToString();

                        gtf.SetDataDetailSpace(43);
                        gtf.SetDataDetail("-", 190, '-', false, true);
                        gtf.SetDataDetailSpace(74);
                        gtf.SetDataDetail("SubTotal :", 11, ' ', true, false, true);
                        gtf.SetDataDetail(subttlNK.ToString(), 13, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(subttlJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(subttlPotonganJasa.ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(subttlDPPJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(subttlParts.ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(subttlDiscParts.ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(subttlDPPParts.ToString(), 17, ' ', false, true, true, true, "n0");

                        gtf.PrintData();
                        nilaiNk = nilaiJasa = nilaiPotongan = nilaiDPP = nilaiParts = nilaiDiscParts = nilaiDPPParts = 0;
                        subttlNK = subttlJasa = subttlPotonganJasa = subttlDPPJasa = subttlParts = subttlDiscParts = subttlDPPParts = 0;

                        gtf.SetDataDetailSpace(43);

                        gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 14, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["OperationNo"].ToString(), 11, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 13, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["PartsAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["PartsDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDescription"].ToString(), 47, ' ', false, true);
                        gtf.PrintData();

                        nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                        nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                        nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                        nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                        nilaiParts += Convert.ToDecimal(dt.Rows[i]["PartsAmt"].ToString());
                        nilaiDiscParts += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                        nilaiDPPParts += Convert.ToDecimal(dt.Rows[i]["PartsDPPAmt"].ToString());
                        noUrut++;
                    }
                    else
                    {
                        if (noUrut == 0)
                        {
                            gtf.SetDataDetail(dt.Rows[i]["FmID"].ToString(), 7, ' ', true);
                            gtf.SetDataDetail(dt.Rows[i]["SaName"].ToString(), 34, ' ', true);
                        }
                        else
                            gtf.SetDataDetailSpace(43);

                        gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 15, ' ', true);
                        gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 14, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["OperationNo"].ToString(), 11, ' ', true);
                        gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 13, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["PartsAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["PartsDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(dt.Rows[i]["TaskDescription"].ToString(), 47, ' ', false, true);
                        gtf.PrintData();

                        nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                        nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                        nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                        nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                        nilaiParts += Convert.ToDecimal(dt.Rows[i]["PartsAmt"].ToString());
                        nilaiDiscParts += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                        nilaiDPPParts += Convert.ToDecimal(dt.Rows[i]["PartsDPPAmt"].ToString());
                        noUrut++;
                    }
                }
                else
                {
                    noUrut = 0;
                    subttlNK += nilaiNk;
                    subttlJasa += nilaiJasa;
                    subttlPotonganJasa += nilaiPotongan;
                    subttlDPPJasa += nilaiDPP;
                    subttlParts += nilaiParts;
                    subttlDiscParts += nilaiDiscParts;
                    subttlDPPParts += nilaiDPPParts;

                    ttlNK += subttlNK;
                    ttlJasa += subttlJasa;
                    ttlPotonganJasa += subttlPotonganJasa;
                    ttlDPPJasa += subttlDPPJasa;
                    ttlParts += subttlParts;
                    ttlDiscParts += subttlDiscParts;
                    ttlDPPParts += subttlDPPParts;

                    ttlNKGrnd += ttlNK;
                    ttlJasaGrnd += ttlJasa;
                    ttlPotonganJasaGrnd += ttlPotonganJasa;
                    ttlDPPJasaGrnd += ttlDPPJasa;
                    ttlPartsGrnd += ttlParts;
                    ttlDiscPartsGrnd += ttlDiscParts;
                    ttlDPPPartsGrnd += ttlDPPParts;

                    docNo = dt.Rows[i]["InvoiceNo"].ToString();
                    fmId = dt.Rows[i]["FmID"].ToString();

                    gtf.SetDataDetailSpace(43);
                    gtf.SetDataDetail("-", 190, '-', false, true);
                    gtf.SetDataDetailSpace(74);
                    gtf.SetDataDetail("SubTotal :", 11, ' ', true, false, true);
                    gtf.SetDataDetail(subttlNK.ToString(), 13, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(subttlJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotonganJasa.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPPJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlParts.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDiscParts.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPPParts.ToString(), 17, ' ', false, true, true, true, "n0");

                    gtf.PrintData();

                    gtf.SetDataDetailSpace(43);
                    gtf.SetDataDetail("-", 190, '-', false, true);
                    gtf.SetDataDetailSpace(74);
                    gtf.SetDataDetail("TOTAL :", 11, ' ', true, false, true);
                    gtf.SetDataDetail(ttlNK.ToString(), 13, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(ttlJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlPotonganJasa.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDPPJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlParts.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDiscParts.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDPPParts.ToString(), 17, ' ', false, true, true, true, "n0");

                    //gtf.PrintData(false, true);

                    nilaiNk = nilaiJasa = nilaiPotongan = nilaiDPP = nilaiParts = nilaiDiscParts = nilaiDPPParts = 0;
                    subttlNK = subttlJasa = subttlPotonganJasa = subttlDPPJasa = subttlParts = subttlDiscParts = subttlDPPParts = 0;
                    ttlNK = ttlJasa = ttlPotonganJasa = ttlDPPJasa = ttlParts = ttlDiscParts = ttlDPPParts = 0;

                    gtf.SetDataDetail(dt.Rows[i]["FmID"].ToString(), 7, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["FmName"].ToString(), 34, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["InvoiceNo"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["InvoiceDate"].ToString()).ToString("dd-MMM-yyyy"), 14, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationNo"].ToString(), 11, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 13, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["PartsDPPAmt"].ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Rows[i]["TaskDescription"].ToString(), 47, ' ', false, true);
                    gtf.PrintData();

                    nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                    nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                    nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                    nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                    nilaiParts += Convert.ToDecimal(dt.Rows[i]["PartsAmt"].ToString());
                    nilaiDiscParts += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                    nilaiDPPParts += Convert.ToDecimal(dt.Rows[i]["PartsDPPAmt"].ToString());
                    noUrut++;
                }

                if (i == dt.Rows.Count - 1)
                {
                    subttlNK += nilaiNk;
                    subttlJasa += nilaiJasa;
                    subttlPotonganJasa += nilaiPotongan;
                    subttlDPPJasa += nilaiDPP;
                    subttlParts += nilaiParts;
                    subttlDiscParts += nilaiDiscParts;
                    subttlDPPParts += nilaiDPPParts;

                    ttlNK += subttlNK;
                    ttlJasa += subttlJasa;
                    ttlPotonganJasa += subttlPotonganJasa;
                    ttlDPPJasa += subttlDPPJasa;
                    ttlParts += subttlParts;
                    ttlDiscParts += subttlDiscParts;
                    ttlDPPParts += subttlDPPParts;

                    ttlNKGrnd += ttlNK;
                    ttlJasaGrnd += ttlJasa;
                    ttlPotonganJasaGrnd += ttlPotonganJasa;
                    ttlDPPJasaGrnd += ttlDPPJasa;
                    ttlPartsGrnd += ttlParts;
                    ttlDiscPartsGrnd += ttlDiscParts;
                    ttlDPPPartsGrnd += ttlDPPParts;

                    gtf.SetDataDetailSpace(43);
                    gtf.SetDataDetail("-", 190, '-', false, true);
                    gtf.SetDataDetailSpace(74);
                    gtf.SetDataDetail("SubTotal :", 11, ' ', true, false, true);
                    gtf.SetDataDetail(subttlNK.ToString(), 13, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(subttlJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotonganJasa.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPPJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlParts.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDiscParts.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPPParts.ToString(), 17, ' ', false, true, true, true, "n0");

                    gtf.PrintData();

                    gtf.SetDataDetailSpace(43);
                    gtf.SetDataDetail("-", 190, '-', false, true);
                    gtf.SetDataDetailSpace(74);
                    gtf.SetDataDetail("TOTAL :", 11, ' ', true, false, true);
                    gtf.SetDataDetail(ttlNK.ToString(), 13, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(ttlJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlPotonganJasa.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDPPJasa.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlParts.ToString(), 17, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDiscParts.ToString(), 13, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDPPParts.ToString(), 17, ' ', false, true, true, true, "n0");
                    gtf.PrintData();
                }
            }
            gtf.SetDataReportLine();
            gtf.SetDataDetailSpace(72);
            gtf.SetDataDetail("GRAND TOTAL :", 13, ' ', true, false, true);
            gtf.SetDataDetail(ttlNKGrnd.ToString(), 13, ' ', true, false, true, true, "n2");
            gtf.SetDataDetail(ttlJasaGrnd.ToString(), 17, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotonganJasaGrnd.ToString(), 13, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPJasaGrnd.ToString(), 17, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPartsGrnd.ToString(), 17, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDiscPartsGrnd.ToString(), 13, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPPartsGrnd.ToString(), 17, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport011MK(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W136", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            string periode = string.Format("Periode : {0} s/d {1}", Convert.ToDateTime(setTextParameter[0].ToString()).ToString("dd-MMM-yyy"), Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyy"));

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode, 136, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NIK", 21, ' ', true);
            gtf.SetGroupHeader("NAMA KARYAWAN", 30, ' ', true);
            gtf.SetGroupHeader("N.K", 18, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 20, ' ', true, false, true);
            gtf.SetGroupHeader("POTONGAN", 18, ' ', true, false, true);
            gtf.SetGroupHeader("DPP", 20, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            decimal nilaiNk = 0, nilaiJasa = 0, nilaiPotongan = 0, nilaiDPP = 0, nilaiParts = 0, nilaiDiscParts = 0, nilaiDPPParts = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                gtf.SetDataDetail(dt.Rows[i]["MechanicID"].ToString(), 21, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["MechanicName"].ToString(), 30, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 18, ' ', true, false, true, true, "n2");
                gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 20, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 18, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 20, ' ', false, true, true, true, "n0");
                gtf.PrintData();

                nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
            }

            gtf.SetDataReportLine();
            gtf.SetDataDetailSpace(39);
            gtf.SetDataDetail("GRAND TOTAL :", 13, ' ', true, false, true);
            gtf.SetDataDetail(nilaiNk.ToString(), 18, ' ', true, false, true, true, "n2");
            gtf.SetDataDetail(nilaiJasa.ToString(), 20, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(nilaiPotongan.ToString(), 18, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(nilaiDPP.ToString(), 20, ' ', true, false, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();


            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport011SA(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W136", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            string periode = string.Format("Periode : {0} s/d {1}", Convert.ToDateTime(setTextParameter[0].ToString()).ToString("dd-MMM-yyy"), Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyy"));

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode, 136, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NIK", 7, ' ', true);
            gtf.SetGroupHeader("NAMA KARYAWAN", 27, ' ', true);
            gtf.SetGroupHeader("N.K", 10, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 15, ' ', true, false, true);
            gtf.SetGroupHeader("POTONGAN", 12, ' ', true, false, true);
            gtf.SetGroupHeader("DPP", 15, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 15, ' ', true, false, true);
            gtf.SetGroupHeader("POTONGAN", 12, ' ', true, false, true);
            gtf.SetGroupHeader("DPP", 15, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            decimal nilaiNk = 0, nilaiJasa = 0, nilaiPotongan = 0, nilaiDPP = 0, nilaiParts = 0, nilaiDiscParts = 0, nilaiDPPParts = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                gtf.SetDataDetail(dt.Rows[i]["SaID"].ToString(), 7, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["SaName"].ToString(), 27, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 10, ' ', true, false, true, true, "n2");
                gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 12, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PartsAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 12, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PartsDPPAmt"].ToString(), 15, ' ', false, true, true, true, "n0");
                gtf.PrintData();

                nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                nilaiParts += Convert.ToDecimal(dt.Rows[i]["PartsAmt"].ToString());
                nilaiDiscParts += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                nilaiDPPParts += Convert.ToDecimal(dt.Rows[i]["PartsDPPAmt"].ToString());
            }

            gtf.SetDataReportLine();
            gtf.SetDataDetailSpace(22);
            gtf.SetDataDetail("GRAND TOTAL :", 13, ' ', true, false, true);
            gtf.SetDataDetail(nilaiNk.ToString(), 10, ' ', true, false, true, true, "n2");
            gtf.SetDataDetail(nilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(nilaiPotongan.ToString(), 12, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(nilaiDPP.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(nilaiParts.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(nilaiDiscParts.ToString(), 12, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(nilaiDPPParts.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);
            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport011FM(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W136", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            string periode = string.Format("Periode : {0} s/d {1}", Convert.ToDateTime(setTextParameter[0].ToString()).ToString("dd-MMM-yyy"), Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyy"));

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode, 136, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NIK", 7, ' ', true);
            gtf.SetGroupHeader("NAMA KARYAWAN", 27, ' ', true);
            gtf.SetGroupHeader("N.K", 10, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 15, ' ', true, false, true);
            gtf.SetGroupHeader("POTONGAN", 12, ' ', true, false, true);
            gtf.SetGroupHeader("DPP", 15, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 15, ' ', true, false, true);
            gtf.SetGroupHeader("POTONGAN", 12, ' ', true, false, true);
            gtf.SetGroupHeader("DPP", 15, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            decimal nilaiNk = 0, nilaiJasa = 0, nilaiPotongan = 0, nilaiDPP = 0, nilaiParts = 0, nilaiDiscParts = 0, nilaiDPPParts = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                gtf.SetDataDetail(dt.Rows[i]["FmID"].ToString(), 7, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["FmName"].ToString(), 27, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["OperationHour"].ToString(), 10, ' ', true, false, true, true, "n2");
                gtf.SetDataDetail(dt.Rows[i]["TaskAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TaskDiscAmt"].ToString(), 12, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TaskDPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PartsAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 12, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PartsDPPAmt"].ToString(), 15, ' ', false, true, true, true, "n0");
                gtf.PrintData();

                nilaiNk += Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString());
                nilaiJasa += Convert.ToDecimal(dt.Rows[i]["TaskAmt"].ToString());
                nilaiPotongan += Convert.ToDecimal(dt.Rows[i]["TaskDiscAmt"].ToString());
                nilaiDPP += Convert.ToDecimal(dt.Rows[i]["TaskDPPAmt"].ToString());
                nilaiParts += Convert.ToDecimal(dt.Rows[i]["PartsAmt"].ToString());
                nilaiDiscParts += Convert.ToDecimal(dt.Rows[i]["PartsDiscAmt"].ToString());
                nilaiDPPParts += Convert.ToDecimal(dt.Rows[i]["PartsDPPAmt"].ToString());
            }

            gtf.SetDataReportLine();
            gtf.SetDataDetailSpace(22);
            gtf.SetDataDetail("GRAND TOTAL :", 13, ' ', true, false, true);
            gtf.SetDataDetail(nilaiNk.ToString(), 10, ' ', true, false, true, true, "n2");
            gtf.SetDataDetail(nilaiJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(nilaiPotongan.ToString(), 12, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(nilaiDPP.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(nilaiParts.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(nilaiDiscParts.ToString(), 12, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(nilaiDPPParts.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);
            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport011V2(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {

            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W136", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            string periode = string.Format("Periode : {0} s/d {1}", Convert.ToDateTime(setTextParameter[0].ToString()).ToString("dd-MMM-yyy"), Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyy"));

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode, 136, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NIK", 15, ' ', true);
            gtf.SetGroupHeader("NAMA KARYAWAN", 30, ' ', true);
            gtf.SetGroupHeader("N.K", 12, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 23, ' ', true, false, true);
            gtf.SetGroupHeader("POTONGAN", 20, ' ', true, false, true);
            gtf.SetGroupHeader("DPP", 23, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string mechanicId = "", mechanicGroup = "", docNo = "", employeeName = "";
            int noUrut = 0;

            decimal ttlNK = 0, ttlJasa = 0, ttlPotonganJasa = 0, ttlDPPJasa = 0;

            decimal subttlNK = 0, subttlJasa = 0, subttlPotonganJasa = 0, subttlDPPJasa = 0;

            decimal ttlNKGrnd = 0, ttlJasaGrnd = 0, ttlPotonganJasaGrnd = 0, ttlDPPJasaGrnd = 0;

            decimal oprtHour = 0, shrgTask = 0, nilaiNk = 0, nilaiJasa = 0, oprtCost = 0, nilaiPotongan = 0, laborDiscPct = 0;
            decimal nilaiDPP = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (mechanicId == "")
                {
                    mechanicId = dt.Rows[i]["MechanicID"].ToString();
                    mechanicGroup = dt.Rows[i]["MechanicGroup"].ToString();
                    docNo = dt.Rows[i]["InvoiceNo"].ToString();
                    employeeName = dt.Rows[i]["EmployeeName"].ToString();

                    oprtHour = dt.Rows[i]["OperationHour"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString()) : 0;
                    shrgTask = dt.Rows[i]["SharingTask"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["SharingTask"].ToString()) : 0;
                    if (oprtHour != 0 && shrgTask != 0)
                        nilaiNk = oprtHour / shrgTask;

                    oprtCost = dt.Rows[i]["OperationCost"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["OperationCost"].ToString()) : 0;
                    nilaiJasa = Math.Ceiling(oprtHour * oprtCost);

                    laborDiscPct = dt.Rows[i]["LaborDiscPct"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["LaborDiscPct"].ToString()) : 0;
                    nilaiPotongan = Math.Ceiling((oprtHour * oprtCost) * (laborDiscPct / 100));

                    nilaiDPP = (oprtHour * oprtCost) - Math.Ceiling(((oprtHour * oprtCost) * (laborDiscPct / 100)));

                }
                else if (mechanicId != dt.Rows[i]["MechanicID"].ToString() && setTextParameter[3].ToString() == "NIK")
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["MechanicId"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["EmployeeName"].ToString(), 30, ' ', true);
                    gtf.SetDataDetail(subttlNK.ToString(), 12, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(subttlJasa.ToString(), 23, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotonganJasa.ToString(), 20, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPPJasa.ToString(), 23, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    subttlNK = subttlJasa = subttlPotonganJasa = subttlDPPJasa = 0;

                    if (mechanicGroup != dt.Rows[i]["MechanicGroup"].ToString())
                    {
                        gtf.SetDataDetailSpace(21);
                        gtf.SetDataDetail("-", 115, '-', false, true);
                        gtf.SetDataDetailSpace(21);
                        gtf.SetDataDetail("TOTAL " + mechanicGroup.ToString() + " :", 30, ' ', true, false, true);
                        gtf.SetDataDetail(ttlNK.ToString(), 12, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(ttlJasa.ToString(), 23, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPotonganJasa.ToString(), 20, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlDPPJasa.ToString(), 23, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false);
                        gtf.SetDataDetailLineBreak();
                        ttlNK = ttlJasa = ttlPotonganJasa = ttlDPPJasa = 0;
                    }

                    mechanicId = dt.Rows[i]["MechanicID"].ToString();
                    mechanicGroup = dt.Rows[i]["MechanicGroup"].ToString();
                    docNo = dt.Rows[i]["InvoiceNo"].ToString();
                    employeeName = dt.Rows[i]["EmployeeName"].ToString();

                    oprtHour = dt.Rows[i]["OperationHour"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString()) : 0;
                    shrgTask = dt.Rows[i]["SharingTask"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["SharingTask"].ToString()) : 0;
                    if (oprtHour != 0)
                        nilaiNk = oprtHour / shrgTask;

                    oprtCost = dt.Rows[i]["OperationCost"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["OperationCost"].ToString()) : 0;
                    nilaiJasa = Math.Ceiling(oprtHour * oprtCost);

                    laborDiscPct = dt.Rows[i]["LaborDiscPct"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["LaborDiscPct"].ToString()) : 0;
                    nilaiPotongan = Math.Ceiling((oprtHour * oprtCost) * (laborDiscPct / 100));

                    nilaiDPP = (oprtHour * oprtCost) - Math.Ceiling(((oprtHour * oprtCost) * (laborDiscPct / 100)));
                }
                else if (employeeName != dt.Rows[i]["EmployeeName"].ToString() && setTextParameter[3].ToString() == "NAME")
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["MechanicId"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["EmployeeName"].ToString(), 30, ' ', true);
                    gtf.SetDataDetail(subttlNK.ToString(), 12, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(subttlJasa.ToString(), 23, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlPotonganJasa.ToString(), 20, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(subttlDPPJasa.ToString(), 23, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    subttlNK = subttlJasa = subttlPotonganJasa = subttlDPPJasa = 0;

                    if (mechanicGroup != dt.Rows[i]["MechanicGroup"].ToString())
                    {
                        gtf.SetDataDetailSpace(21);
                        gtf.SetDataDetail("-", 115, '-', false, true);
                        gtf.SetDataDetailSpace(21);
                        gtf.SetDataDetail("TOTAL " + mechanicGroup.ToString() + " :", 30, ' ', true, false, true);
                        gtf.SetDataDetail(ttlNK.ToString(), 12, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(ttlJasa.ToString(), 23, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlPotonganJasa.ToString(), 20, ' ', true, false, true, true, "n0");
                        gtf.SetDataDetail(ttlDPPJasa.ToString(), 23, ' ', false, true, true, true, "n0");
                        gtf.PrintData(false);
                        gtf.SetDataDetailLineBreak();

                        ttlNK = ttlJasa = ttlPotonganJasa = ttlDPPJasa = 0;
                    }

                    mechanicId = dt.Rows[i]["MechanicID"].ToString();
                    mechanicGroup = dt.Rows[i]["MechanicGroup"].ToString();
                    docNo = dt.Rows[i]["InvoiceNo"].ToString();
                    employeeName = dt.Rows[i]["EmployeeName"].ToString();

                    oprtHour = dt.Rows[i]["OperationHour"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString()) : 0;
                    shrgTask = dt.Rows[i]["SharingTask"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["SharingTask"].ToString()) : 0;
                    if (oprtHour != 0 && shrgTask != 0)
                        nilaiNk = oprtHour / shrgTask;

                    oprtCost = dt.Rows[i]["OperationCost"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["OperationCost"].ToString()) : 0;
                    nilaiJasa = Math.Ceiling(oprtHour * oprtCost);

                    laborDiscPct = dt.Rows[i]["LaborDiscPct"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["LaborDiscPct"].ToString()) : 0;
                    nilaiPotongan = Math.Ceiling((oprtHour * oprtCost) * (laborDiscPct / 100));

                    nilaiDPP = (oprtHour * oprtCost) - Math.Ceiling(((oprtHour * oprtCost) * (laborDiscPct / 100)));
                }
                else if (mechanicId == dt.Rows[i]["MechanicID"].ToString())
                {

                    oprtHour = dt.Rows[i]["OperationHour"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["OperationHour"].ToString()) : 0;
                    shrgTask = dt.Rows[i]["SharingTask"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["SharingTask"].ToString()) : 0;
                    if (oprtHour != 0 && shrgTask != 0)
                        nilaiNk = oprtHour / shrgTask;

                    oprtCost = dt.Rows[i]["OperationCost"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["OperationCost"].ToString()) : 0;
                    nilaiJasa = Math.Ceiling(oprtHour * oprtCost);

                    laborDiscPct = dt.Rows[i]["LaborDiscPct"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["LaborDiscPct"].ToString()) : 0;
                    nilaiPotongan = Math.Ceiling((oprtHour * oprtCost) * (laborDiscPct / 100));

                    nilaiDPP = (oprtHour * oprtCost) - Math.Ceiling(((oprtHour * oprtCost) * (laborDiscPct / 100)));

                    if (docNo != dt.Rows[i]["InvoiceNo"].ToString())
                    {
                        docNo = dt.Rows[i]["InvoiceNo"].ToString();

                    }
                }

                ttlNK += nilaiNk;
                ttlJasa += nilaiJasa;
                ttlPotonganJasa += nilaiPotongan;
                ttlDPPJasa += nilaiDPP;

                subttlNK += nilaiNk;
                subttlJasa += nilaiJasa;
                subttlPotonganJasa += nilaiPotongan;
                subttlDPPJasa += nilaiDPP;

                ttlNKGrnd += nilaiNk;
                ttlJasaGrnd += nilaiJasa;
                ttlPotonganJasaGrnd += nilaiPotongan;
                ttlDPPJasaGrnd += nilaiDPP;
            }
            noUrut++;
            gtf.SetDataDetail(noUrut.ToString(), 4, ' ', true, false, true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["MechanicId"].ToString(), 15, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["EmployeeName"].ToString(), 30, ' ', true);
            gtf.SetDataDetail(subttlNK.ToString(), 12, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlJasa.ToString(), 23, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlPotonganJasa.ToString(), 20, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(subttlDPPJasa.ToString(), 23, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(21);
            gtf.SetDataDetail("-", 115, '-', false, true);
            gtf.SetDataDetailSpace(21);
            gtf.SetDataDetail("TOTAL " + mechanicGroup.ToString() + " :", 30, ' ', true, false, true);
            gtf.SetDataDetail(ttlNK.ToString(), 12, ' ', true, false, true, true, "n2");
            gtf.SetDataDetail(ttlJasa.ToString(), 23, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotonganJasa.ToString(), 20, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPJasa.ToString(), 23, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(21);
            gtf.SetDataDetail("-", 115, '-', false, true);
            gtf.SetDataDetailSpace(21);
            gtf.SetDataDetail("GRAND TOTAL :", 30, ' ', true, false, true);
            gtf.SetDataDetail(ttlNKGrnd.ToString(), 12, ' ', true, false, true, true, "n2");
            gtf.SetDataDetail(ttlJasaGrnd.ToString(), 23, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlPotonganJasaGrnd.ToString(), 20, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDPPJasaGrnd.ToString(), 23, ' ', true, false, true, true, "n0");
            gtf.PrintData(false);
            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport012(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {

            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W136", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            string periode = "Periode : " + setTextParameter[1].ToString();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(periode, 136, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeaderSpace(5);
            gtf.SetGroupHeader("IDENTITAS KENDARAAN", 25, ' ', true, false, false, true);
            gtf.SetGroupHeader("IDENTITAS PELANGGAN", 42, ' ', true, false, false, true);
            gtf.SetGroupHeader("TGL.SERVICE", 11, ' ', true);
            gtf.SetGroupHeader("PERIODE SERVICE", 50, ' ', false, true, false, true);
            gtf.SetGroupHeaderSpace(5);
            gtf.SetGroupHeader("-", 25, '-', true);
            gtf.SetGroupHeader("-", 42, '-', true);
            gtf.SetGroupHeader("-", 11, '-', true);
            gtf.SetGroupHeader("-", 50, '-', false, true);
            gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NO POLISI", 12, ' ', true);
            gtf.SetGroupHeader("NO RANGKA", 12, ' ', true);
            gtf.SetGroupHeader("NAMA PELANGGAN", 42, ' ', true);
            gtf.SetGroupHeader("PERTAMA", 11, ' ', true);
            gtf.SetGroupHeader("01", 6, ' ', true, false, true);
            gtf.SetGroupHeader("02", 6, ' ', true, false, true);
            gtf.SetGroupHeader("03", 6, ' ', true, false, true);
            gtf.SetGroupHeader("04", 6, ' ', true, false, true);
            gtf.SetGroupHeader("05", 6, ' ', true, false, true);
            gtf.SetGroupHeader("06", 6, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 8, ' ', false, true, true);
            gtf.SetGroupHeaderSpace(5);
            gtf.SetGroupHeader("BASIC MODEL", 12, ' ', true);
            gtf.SetGroupHeader("NO MESIN", 12, ' ', true);
            gtf.SetGroupHeader("KODE", 10, ' ', true);
            gtf.SetGroupHeader("TELEPON", 15, ' ', true);
            gtf.SetGroupHeader("HANDPHONE", 15, ' ', true);
            gtf.SetGroupHeader("TERAKHIR", 11, ' ', true);
            gtf.SetGroupHeader("07", 6, ' ', true, false, true);
            gtf.SetGroupHeader("08", 6, ' ', true, false, true);
            gtf.SetGroupHeader("09", 6, ' ', true, false, true);
            gtf.SetGroupHeader("10", 6, ' ', true, false, true);
            gtf.SetGroupHeader("11", 6, ' ', true, false, true);
            gtf.SetGroupHeader("12", 6, ' ', true, false, true);
            gtf.SetGroupHeader(" ", 8, ' ', false, true, true);

            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            int noUrut = 0, countData = 0;

            decimal ttlJan = 0, ttlFeb = 0, ttlMar = 0, ttlApr = 0, ttlMay = 0, ttlJun = 0, ttlJul = 0, ttlAug = 0, ttlSep = 0, ttlOct = 0, ttlNov = 0, ttlDec = 0, ttlTotal = 0;
            decimal ttlJanGrnd = 0, ttlFebGrnd = 0, ttlMarGrnd = 0, ttlAprGrnd = 0, ttlMayGrnd = 0, ttlJunGrnd = 0, ttlJulGrnd = 0, ttlAugGrnd = 0, ttlSepGrnd = 0, ttlOctGrnd = 0, ttlNovGrnd = 0, ttlDecGrnd = 0, ttlTotalGrnd = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                noUrut++;

                if (countData > 20)
                {
                    gtf.SetDataDetailSpace(74);
                    gtf.SetDataDetail("-", 62, '-', false, true);
                    gtf.SetDataDetailSpace(74);
                    gtf.SetDataDetail("TOTAL :", 11, ' ', true);
                    gtf.SetDataDetail(ttlJan.ToString(), 6, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlFeb.ToString(), 6, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlMar.ToString(), 6, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlApr.ToString(), 6, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlMay.ToString(), 6, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlJun.ToString(), 6, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlTotal.ToString(), 8, ' ', false, true, true, true, "n0");
                    gtf.SetDataDetailSpace(86);
                    gtf.SetDataDetail(ttlJul.ToString(), 6, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlAug.ToString(), 6, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlSep.ToString(), 6, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlOct.ToString(), 6, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlNov.ToString(), 6, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(ttlDec.ToString(), 6, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(" ", 8, ' ', false, true, true);
                    gtf.PrintData(false);
                    countData = 0;
                    ttlJan = ttlFeb = ttlMar = ttlApr = ttlMay = ttlJun = ttlJul = ttlAug = ttlSep = ttlOct = ttlNov = ttlDec = ttlTotal = 0;
                }

                gtf.SetDataDetail(noUrut.ToString(), 4, ' ', true, false, true);
                gtf.SetDataDetail(dt.Rows[i]["PoliceRegNo"] != null ? dt.Rows[i]["PoliceRegNo"].ToString() : "", 12, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["ChassisNo"] != null ? dt.Rows[i]["ChassisNo"].ToString() : "", 12, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["CustomerName"] != null ? dt.Rows[i]["CustomerName"].ToString() : "", 42, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["FirstServiceDate"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i]["FirstServiceDate"].ToString()).ToString("dd-MMM-yyyy") : "", 11, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["Periode1"] != null ? dt.Rows[i]["Periode1"].ToString() : "0", 6, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["Periode2"] != null ? dt.Rows[i]["Periode2"].ToString() : "0", 6, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["Periode3"] != null ? dt.Rows[i]["Periode3"].ToString() : "0", 6, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["Periode4"] != null ? dt.Rows[i]["Periode4"].ToString() : "0", 6, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["Periode5"] != null ? dt.Rows[i]["Periode5"].ToString() : "0", 6, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["Periode6"] != null ? dt.Rows[i]["Periode6"].ToString() : "0", 6, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TotalPeriode"] != null ? dt.Rows[i]["TotalPeriode"].ToString() : "0", 8, ' ', false, true, true, true, "n0");
                gtf.SetDataDetailSpace(5);
                gtf.SetDataDetail(dt.Rows[i]["BasicModel"] != null ? dt.Rows[i]["BasicModel"].ToString() : " ", 12, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["EngineNo"] != null ? dt.Rows[i]["EngineNo"].ToString() : " ", 12, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["CustomerCode"] != null ? dt.Rows[i]["CustomerCode"].ToString() : " ", 10, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["PhoneNo"] != null ? dt.Rows[i]["PhoneNo"].ToString() : " ", 15, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["HPNo"] != null ? dt.Rows[i]["HPNo"].ToString() : "", 15, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["LastServiceDate"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i]["LastServiceDate"].ToString()).ToString("dd-MMM-yyyy") : "", 11, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["Periode7"] != null ? dt.Rows[i]["Periode7"].ToString() : "0", 6, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["Periode8"] != null ? dt.Rows[i]["Periode8"].ToString() : "0", 6, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["Periode9"] != null ? dt.Rows[i]["Periode9"].ToString() : "0", 6, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["Periode10"] != null ? dt.Rows[i]["Periode10"].ToString() : "0", 6, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["Periode11"] != null ? dt.Rows[i]["Periode11"].ToString() : "0", 6, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["Periode12"] != null ? dt.Rows[i]["Periode12"].ToString() : "0", 6, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(" ", 8, ' ', false, true, true);

                gtf.PrintData(false);
                countData++;

                ttlJan += dt.Rows[i]["Periode1"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode1"].ToString()) : 0;
                ttlFeb += dt.Rows[i]["Periode2"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode2"].ToString()) : 0;
                ttlMar += dt.Rows[i]["Periode3"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode3"].ToString()) : 0;
                ttlApr += dt.Rows[i]["Periode4"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode4"].ToString()) : 0;
                ttlMay += dt.Rows[i]["Periode5"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode5"].ToString()) : 0;
                ttlJun += dt.Rows[i]["Periode6"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode6"].ToString()) : 0;
                ttlJul += dt.Rows[i]["Periode7"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode7"].ToString()) : 0;
                ttlAug += dt.Rows[i]["Periode8"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode8"].ToString()) : 0;
                ttlSep += dt.Rows[i]["Periode9"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode9"].ToString()) : 0;
                ttlOct += dt.Rows[i]["Periode10"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode10"].ToString()) : 0;
                ttlNov += dt.Rows[i]["Periode11"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode11"].ToString()) : 0;
                ttlDec += dt.Rows[i]["Periode12"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode12"].ToString()) : 0;
                ttlTotal += dt.Rows[i]["TotalPeriode"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["TotalPeriode"].ToString()) : 0;

                ttlJanGrnd += dt.Rows[i]["Periode1"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode1"].ToString()) : 0;
                ttlFebGrnd += dt.Rows[i]["Periode2"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode2"].ToString()) : 0;
                ttlMarGrnd += dt.Rows[i]["Periode3"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode3"].ToString()) : 0;
                ttlAprGrnd += dt.Rows[i]["Periode4"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode4"].ToString()) : 0;
                ttlMayGrnd += dt.Rows[i]["Periode5"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode5"].ToString()) : 0;
                ttlJunGrnd += dt.Rows[i]["Periode6"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode6"].ToString()) : 0;
                ttlJulGrnd += dt.Rows[i]["Periode7"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode7"].ToString()) : 0;
                ttlAugGrnd += dt.Rows[i]["Periode8"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode8"].ToString()) : 0;
                ttlSepGrnd += dt.Rows[i]["Periode9"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode9"].ToString()) : 0;
                ttlOctGrnd += dt.Rows[i]["Periode10"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode10"].ToString()) : 0;
                ttlNovGrnd += dt.Rows[i]["Periode11"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode11"].ToString()) : 0;
                ttlDecGrnd += dt.Rows[i]["Periode12"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["Periode12"].ToString()) : 0;
                ttlTotalGrnd += dt.Rows[i]["TotalPeriode"].ToString() != "" ? Convert.ToDecimal(dt.Rows[i]["TotalPeriode"].ToString()) : 0;

            }
            gtf.SetDataDetailSpace(74);
            gtf.SetDataDetail("-", 62, '-', false, true);
            gtf.SetDataDetailSpace(74);
            gtf.SetDataDetail("TOTAL :", 11, ' ', true);
            gtf.SetDataDetail(ttlJan.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlFeb.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlMar.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlApr.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlMay.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJun.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlTotal.ToString(), 8, ' ', false, true, true, true, "n0");
            gtf.SetDataDetailSpace(86);
            gtf.SetDataDetail(ttlJul.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlAug.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlSep.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlOct.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNov.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDec.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(" ", 8, ' ', false, true, true);
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(74);
            gtf.SetDataDetail("-", 62, '-', false, true);
            gtf.SetDataDetailSpace(74);
            gtf.SetDataDetail("GRAND TOTAL :", 11, ' ', true);
            gtf.SetDataDetail(ttlJanGrnd.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlFebGrnd.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlMarGrnd.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlAprGrnd.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlMayGrnd.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlJunGrnd.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlTotalGrnd.ToString(), 8, ' ', false, true, true, true, "n0");
            gtf.SetDataDetailSpace(86);
            gtf.SetDataDetail(ttlJulGrnd.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlAugGrnd.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlSepGrnd.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlOctGrnd.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlNovGrnd.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(ttlDecGrnd.ToString(), 6, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(" ", 8, ' ', false, true, true);
            gtf.PrintData(false);


            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport013(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W163", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader("REGISTER " + setTextParameter[2].ToString(), 160, ' ', false, true, false, true);
            gtf.SetGroupHeader("PERIODE TANGGAL : " + setTextParameter[1].ToString(), 160, ' ', false, true, false, true);

            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeaderSpace(4);
            gtf.SetGroupHeader("DOKUMEN", 25, ' ', true, false, false, true);
            gtf.SetGroupHeader("NO POLISI", 10, ' ', true);
            gtf.SetGroupHeader("IDENTITAS KENDARAAN", 24, ' ', true);
            gtf.SetGroupHeader("IDENTITAS PELANGGAN", 51, ' ', true, false, false, true);
            gtf.SetGroupHeader("NILAI", 11, ' ', true, false, true);
            gtf.SetGroupHeader("%Pot", 9, ' ', true, false, true);
            gtf.SetGroupHeader("POTONGAN", 10, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 12, ' ', false, true, true);

            gtf.SetGroupHeaderSpace(4);
            gtf.SetGroupHeader("-", 25, '-', true);
            gtf.SetGroupHeader("-", 10, '-', true);
            gtf.SetGroupHeader("-", 24, '-', true);
            gtf.SetGroupHeader("-", 51, '-', true);
            gtf.SetGroupHeader("-", 11, '-', true);
            gtf.SetGroupHeader("-", 9, '-', true);
            gtf.SetGroupHeader("-", 10, '-', true);
            gtf.SetGroupHeader("-", 12, '-', false, true);

            gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
            gtf.SetGroupHeader("NO.", 13, ' ', true);
            gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
            gtf.SetGroupHeader("TERAKHIR", 10, ' ', true);
            gtf.SetGroupHeader("BASIC MODEL", 24, ' ', true);
            gtf.SetGroupHeader("NAMA PELANGGAN", 35, ' ', true);
            gtf.SetGroupHeader("TELEPON", 15, ' ', true);
            gtf.SetGroupHeader("JASA", 11, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 9, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 10, ' ', true, false, true);
            gtf.SetGroupHeader("DPP", 12, ' ', false, true, true);

            gtf.SetGroupHeaderSpace(30);
            gtf.SetGroupHeader("LAMA", 10, ' ', true);
            gtf.SetGroupHeader("NO. RANGKA", 24, ' ', true);
            gtf.SetGroupHeader("NAMA PEMBAYAR", 35, ' ', true);
            gtf.SetGroupHeader("HP", 15, ' ', true);
            gtf.SetGroupHeader("SPAREPART", 11, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 9, ' ', true, false, true);
            gtf.SetGroupHeader("SPAREPART", 10, ' ', true, false, true);
            gtf.SetGroupHeader("PPN", 12, ' ', false, true, true);

            gtf.SetGroupHeaderSpace(41);
            gtf.SetGroupHeader("NO. MESIN", 24, ' ', true);
            gtf.SetGroupHeaderSpace(52);
            gtf.SetGroupHeader("MATERIAL", 11, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 9, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 10, ' ', true, false, true);
            gtf.SetGroupHeader("SERVICE", 12, ' ', false, true, true);

            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            int noUrut = 0;
            decimal ttlNilai = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                noUrut++;
                gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                gtf.SetDataDetail(dt.Rows[i]["DocumentNo"].ToString(), 13, ' ', true);
                gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["DocumentDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["PoliceRegNo"].ToString(), 10, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["BasicModel"].ToString(), 24, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["CustomerFullName"].ToString(), 35, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["PhoneNo"].ToString(), 15, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["LaborGrossAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["LaborDiscPct"].ToString(), 9, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["LaborDiscAmt"].ToString(), 10, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmount"].ToString(), 12, ' ', false, true, true, true, "n0");
                gtf.PrintData(false);

                gtf.SetDataDetailSpace(4);
                gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString() != "" ? dt.Rows[i]["JobOrderNo"].ToString() : "", 13, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["JobOrderDate"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy") : "", 11, ' ', true);
                gtf.SetDataDetail(" ", 10, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["Chassis"].ToString(), 24, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["CustomerFullNameBill"].ToString(), 35, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["HPNo"].ToString(), 15, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["PartsGrossAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PartDiscPct"] != null ? dt.Rows[i]["PartDiscPct"].ToString() : "0", 9, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PartsDiscAmt"].ToString(), 10, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TotalPPNAmount"].ToString(), 12, ' ', false, true, true, true, "n0");
                gtf.PrintData(false);

                gtf.SetDataDetailSpace(41);
                gtf.SetDataDetail(dt.Rows[i]["Engine"].ToString(), 24, ' ', true);
                gtf.SetDataDetailSpace(52);
                gtf.SetDataDetail(dt.Rows[i]["MaterialGrossAmt"].ToString(), 11, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["MaterialDiscPct"].ToString(), 9, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["MaterialDiscAmt"].ToString(), 10, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["TotalSrvAmount"].ToString(), 12, ' ', false, true, true, true, "n0");
                gtf.PrintData(false);

                gtf.SetDataDetailSpace(18);
                gtf.SetDataDetail("Permintaan Perawatan :", 22, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["JobType"].ToString(), 122, ' ', false, true);

                string[] splitString = new string[2];
                splitString[0] = "\n";
                splitString[1] = "\r";

                string[] arrayDesc = dt.Rows[i]["ServiceRequestDesc"].ToString().Split(splitString, StringSplitOptions.None);
                foreach (string row in arrayDesc)
                {
                    if (row != "")
                    {
                        gtf.SetDataDetailSpace(41);
                        gtf.SetDataDetail(row, 122, ' ', false, true);
                        gtf.PrintData(false);
                    }
                }
                gtf.SetDataDetail(" ", 1, ' ', false, true);
                gtf.PrintData(false);
            }

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport014(string recordId, DataSet dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W163", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(setTextParameter[2].ToString(), 163, ' ', false, true, false, true);
            gtf.SetGroupHeader("KELOMPOK PEKERJAAN : " + dt.Tables[0].Rows[0]["DescGroupJobType"].ToString(), 163, ' ', false, true);
            gtf.SetGroupHeaderLine();

            gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
            gtf.SetGroupHeader("NO. SPK", 13, ' ', true);
            gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
            gtf.SetGroupHeader("MODEL", 15, ' ', true);
            gtf.SetGroupHeader("NO. POLISI", 10, ' ', true);
            gtf.SetGroupHeader("JAM", 5, ' ', true);
            gtf.SetGroupHeader("KM AKHIR", 8, ' ', true, false, true);
            gtf.SetGroupHeader("TGL. KELUAR", 11, ' ', true);
            gtf.SetGroupHeader("JAM", 5, ' ', true);
            gtf.SetGroupHeader("FAKTUR", 13, ' ', true);
            gtf.SetGroupHeader("USER", 11, ' ', true);
            gtf.SetGroupHeader("SA/FM", 12, ' ', true);
            gtf.SetGroupHeader("FOREMAN", 14, ' ', true);
            gtf.SetGroupHeader("STATUS", 18, ' ', false, true);

            gtf.SetGroupHeaderSpace(19);
            gtf.SetGroupHeader("PEMILIK", 34, ' ', true);
            gtf.SetGroupHeader("JENIS KENDARAAN", 36, ' ', true);
            gtf.SetGroupHeader("PERMINTAAN PENAWARAN", 72, ' ', false, true);

            gtf.SetGroupHeaderSpace(58);
            gtf.SetGroupHeader("PEKERJAAN", 28, ' ', true);
            gtf.SetGroupHeader("NILAI KERJA", 11, ' ', true, false, true);
            gtf.SetGroupHeaderSpace(21);
            gtf.SetGroupHeader("MEKANIK", 43, ' ', false, true);

            gtf.SetGroupHeaderSpace(19);
            gtf.SetGroupHeader("PART NO", 27, ' ', true);
            gtf.SetGroupHeader("PART NAME", 43, ' ', true);
            gtf.SetGroupHeader("DEMAND QTY", 13, ' ', true, false, true);
            gtf.SetGroupHeader("SUPPLY QTY", 11, ' ', true, false, true);
            gtf.SetGroupHeader("RETURN QTY", 12, ' ', true, false, true);
            gtf.SetGroupHeader("NO SUPPLY SLIP", 14, ' ', true, false, true);
            gtf.SetGroupHeader("NO SS RETURN", 18, ' ', false, true);

            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "", task = "";
            int noUrut = 0;
            bool statusDesc = false;

            for (int i = 0; i < dt.Tables[0].Rows.Count; i++)
            {
                statusDesc = false;
                if (docNo == "")
                {
                    docNo = dt.Tables[0].Rows[i]["JobOrderNo"].ToString();

                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Tables[0].Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["BasicModel"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["policeRegNo"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(Convert.ToDateTime(dt.Tables[0].Rows[i]["JobOrderTime"].ToString()).ToString("HH:mm"), 5, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["Odometer"].ToString(), 8, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["LockingDate"].ToString() != "" ? Convert.ToDateTime(dt.Tables[0].Rows[i]["LockingDate"].ToString()).ToString("dd-MMM-yyyy") : "", 11, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["LockingTime"].ToString() != "" ? Convert.ToDateTime(dt.Tables[0].Rows[i]["LockingTime"].ToString()).ToString("HH:mm") : "", 5, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["InvoiceNo"].ToString(), 13, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["CreatedBy"].ToString(), 11, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["ForemanID"].ToString(), 12, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["MechanicID"].ToString(), 14, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["Status"].ToString(), 18, ' ', false, true);
                    gtf.PrintData(false);

                    gtf.SetDataDetailSpace(19);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["Customer"].ToString(), 34, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["JobType"].ToString(), 36, ' ', true);

                    string[] splitString = new string[2];
                    splitString[0] = "\n";
                    splitString[1] = "\r";

                    string[] arrayDesc = dt.Tables[0].Rows[i]["ServiceRequestDesc"].ToString().Split(splitString, StringSplitOptions.None);

                    foreach (string row in arrayDesc)
                    {
                        if (row != "")
                        {
                            if (statusDesc == true)
                                gtf.SetDataDetailSpace(91);
                            gtf.SetDataDetail(row, 72, ' ', false, true);
                            gtf.PrintData(false);
                            statusDesc = true;
                        }
                    }

                    DataRow[] dr = dt.Tables[1].Select(string.Format("JobOrderNo = '{0}'", dt.Tables[0].Rows[i]["JobOrderNo"].ToString()));
                    foreach (DataRow row in dr)
                    {
                        gtf.SetDataDetailSpace(58);
                        gtf.SetDataDetail(row[1] != null ? row[1].ToString() : "", 28, ' ', true);
                        gtf.SetDataDetail(row[2] != null ? row[2].ToString() : "", 11, ' ', true, false, true);
                        gtf.SetDataDetailSpace(21);
                        gtf.SetDataDetail(row[3] != null ? row[3].ToString() : "", 43, ' ', false, true);
                        gtf.PrintData(false);
                    }

                    if (task == "")
                    {
                        gtf.SetDataDetailSpace(19);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["PartNo"].ToString() != null ? dt.Tables[0].Rows[i]["PartNo"].ToString() : "", 27, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["PartName"].ToString() != null ? dt.Tables[0].Rows[i]["PartName"].ToString() : "", 43, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["DemandQty"].ToString() != null ? dt.Tables[0].Rows[i]["DemandQty"].ToString() : "", 13, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SupplyQty"].ToString() != null ? dt.Tables[0].Rows[i]["SupplyQty"].ToString() : "", 11, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["ReturnQty"].ToString() != null ? dt.Tables[0].Rows[i]["ReturnQty"].ToString() : "", 12, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SSNo"].ToString() != null ? dt.Tables[0].Rows[i]["SSNo"].ToString() : "", 14, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SSReturn"].ToString() != null ? dt.Tables[0].Rows[i]["SSReturn"].ToString() : "", 18, ' ', false, true);
                        gtf.PrintData(false);
                        task = dt.Tables[0].Rows[i]["PartNo"].ToString();
                    }
                    else if (task != dt.Tables[0].Rows[i]["PartNo"].ToString())
                    {
                        gtf.SetDataDetailSpace(19);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["PartNo"].ToString() != null ? dt.Tables[0].Rows[i]["PartNo"].ToString() : "", 27, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["PartName"].ToString() != null ? dt.Tables[0].Rows[i]["PartName"].ToString() : "", 43, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["DemandQty"].ToString() != null ? dt.Tables[0].Rows[i]["DemandQty"].ToString() : "", 13, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SupplyQty"].ToString() != null ? dt.Tables[0].Rows[i]["SupplyQty"].ToString() : "", 11, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["ReturnQty"].ToString() != null ? dt.Tables[0].Rows[i]["ReturnQty"].ToString() : "", 12, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SSNo"].ToString() != null ? dt.Tables[0].Rows[i]["SSNo"].ToString() : "", 14, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SSReturn"].ToString() != null ? dt.Tables[0].Rows[i]["SSReturn"].ToString() : "", 18, ' ', false, true);
                        gtf.PrintData(false);
                        task = dt.Tables[0].Rows[i]["PartNo"].ToString();
                    }
                }
                else if (docNo != dt.Tables[0].Rows[i]["JobOrderNo"].ToString())
                {
                    gtf.SetDataDetail("-", 163, '-', false, true);
                    gtf.PrintData(false);

                    task = "";

                    noUrut++;

                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["JobOrderNo"].ToString() != null ? dt.Tables[0].Rows[i]["JobOrderNo"].ToString() : "", 13, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["JobOrderDate"].ToString() != null ? Convert.ToDateTime(dt.Tables[0].Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy") : "", 11, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["BasicModel"].ToString() != null ? dt.Tables[0].Rows[i]["BasicModel"].ToString() : "", 15, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["policeRegNo"].ToString() != null ? dt.Tables[0].Rows[i]["policeRegNo"].ToString() : "", 10, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["JobOrderTime"].ToString() != null ? Convert.ToDateTime(dt.Tables[0].Rows[i]["JobOrderTime"].ToString()).ToString("HH:mm") : "", 5, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["Odometer"].ToString() != null ? dt.Tables[0].Rows[i]["Odometer"].ToString() : "", 8, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["LockingDate"].ToString() != "" ? Convert.ToDateTime(dt.Tables[0].Rows[i]["LockingDate"].ToString()).ToString("dd-MMM-yyyy") : "", 11, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["LockingTime"].ToString() != "" ? Convert.ToDateTime(dt.Tables[0].Rows[i]["LockingTime"].ToString()).ToString("HH:mm") : "", 5, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["InvoiceNo"].ToString() != null ? dt.Tables[0].Rows[i]["InvoiceNo"].ToString() : "", 13, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["CreatedBy"].ToString() != null ? dt.Tables[0].Rows[i]["CreatedBy"].ToString() : "", 11, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["ForemanID"].ToString() != null ? dt.Tables[0].Rows[i]["ForemanID"].ToString() : "", 12, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["MechanicID"].ToString() != null ? dt.Tables[0].Rows[i]["MechanicID"].ToString() : "", 14, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["Status"].ToString() != null ? dt.Tables[0].Rows[i]["Status"].ToString() : "", 18, ' ', false, true);
                    gtf.PrintData(false);

                    gtf.SetDataDetailSpace(19);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["Customer"].ToString(), 34, ' ', true);
                    gtf.SetDataDetail(dt.Tables[0].Rows[i]["JobType"].ToString(), 36, ' ', true);

                    string[] splitString = new string[2];
                    splitString[0] = "\n";
                    splitString[1] = "\r";

                    string[] arrayDesc = dt.Tables[0].Rows[i]["ServiceRequestDesc"].ToString().Split(splitString, StringSplitOptions.None);
                    foreach (string row in arrayDesc)
                    {
                        if (row != "")
                        {
                            if (statusDesc == true)
                                gtf.SetDataDetailSpace(91);
                            gtf.SetDataDetail(row, 72, ' ', false, true);
                            gtf.PrintData(false);
                            statusDesc = true;
                        }
                    }

                    DataRow[] dr = dt.Tables[1].Select(string.Format("JobOrderNo = '{0}'", dt.Tables[0].Rows[i]["JobOrderNo"].ToString()));
                    foreach (DataRow row in dr)
                    {
                        gtf.SetDataDetailSpace(58);
                        gtf.SetDataDetail(row[1] != null ? row[1].ToString() : "", 28, ' ', true);
                        gtf.SetDataDetail(row[2] != null ? row[2].ToString() : "", 11, ' ', true, false, true);
                        gtf.SetDataDetailSpace(21);
                        gtf.SetDataDetail(row[3] != null ? row[3].ToString() : "", 43, ' ', false, true);
                        gtf.PrintData(false);
                    }

                    if (task == "")
                    {
                        gtf.SetDataDetailSpace(19);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["PartNo"].ToString() != null ? dt.Tables[0].Rows[i]["PartNo"].ToString() : "", 27, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["PartName"].ToString() != null ? dt.Tables[0].Rows[i]["PartName"].ToString() : "", 43, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["DemandQty"].ToString() != null ? dt.Tables[0].Rows[i]["DemandQty"].ToString() : "", 13, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SupplyQty"].ToString() != null ? dt.Tables[0].Rows[i]["SupplyQty"].ToString() : "", 11, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["ReturnQty"].ToString() != null ? dt.Tables[0].Rows[i]["ReturnQty"].ToString() : "", 12, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SSNo"].ToString() != null ? dt.Tables[0].Rows[i]["SSNo"].ToString() : "", 14, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SSReturn"].ToString() != null ? dt.Tables[0].Rows[i]["SSReturn"].ToString() : "", 18, ' ', false, true);
                        gtf.PrintData(false);
                        task = dt.Tables[0].Rows[i]["PartNo"].ToString();
                    }
                    else if (task != dt.Tables[0].Rows[i]["PartNo"].ToString())
                    {
                        gtf.SetDataDetailSpace(19);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["PartNo"].ToString() != null ? dt.Tables[0].Rows[i]["PartNo"].ToString() : "", 27, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["PartName"].ToString() != null ? dt.Tables[0].Rows[i]["PartName"].ToString() : "", 43, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["DemandQty"].ToString() != null ? dt.Tables[0].Rows[i]["DemandQty"].ToString() : "", 13, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SupplyQty"].ToString() != null ? dt.Tables[0].Rows[i]["SupplyQty"].ToString() : "", 11, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["ReturnQty"].ToString() != null ? dt.Tables[0].Rows[i]["ReturnQty"].ToString() : "", 12, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SSNo"].ToString() != null ? dt.Tables[0].Rows[i]["SSNo"].ToString() : "", 14, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SSReturn"].ToString() != null ? dt.Tables[0].Rows[i]["SSReturn"].ToString() : "", 18, ' ', false, true);
                        gtf.PrintData(false);
                        task = dt.Tables[0].Rows[i]["PartNo"].ToString();
                    }

                    docNo = dt.Tables[0].Rows[i]["JobOrderNo"].ToString();
                }
                else if (docNo == dt.Tables[0].Rows[i]["JobOrderNo"].ToString())
                {
                    if (task == "")
                    {
                        gtf.SetDataDetailSpace(19);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["PartNo"].ToString() != null ? dt.Tables[0].Rows[i]["PartNo"].ToString() : "", 27, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["PartName"].ToString() != null ? dt.Tables[0].Rows[i]["PartName"].ToString() : "", 43, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["DemandQty"].ToString() != null ? dt.Tables[0].Rows[i]["DemandQty"].ToString() : "", 13, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SupplyQty"].ToString() != null ? dt.Tables[0].Rows[i]["SupplyQty"].ToString() : "", 11, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["ReturnQty"].ToString() != null ? dt.Tables[0].Rows[i]["ReturnQty"].ToString() : "", 12, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SSNo"].ToString() != null ? dt.Tables[0].Rows[i]["SSNo"].ToString() : "", 14, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SSReturn"].ToString() != null ? dt.Tables[0].Rows[i]["SSReturn"].ToString() : "", 18, ' ', false, true);
                        gtf.PrintData(false);
                        task = dt.Tables[0].Rows[i]["PartNo"].ToString();
                    }
                    else if (task != dt.Tables[0].Rows[i]["PartNo"].ToString())
                    {
                        gtf.SetDataDetailSpace(19);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["PartNo"].ToString() != null ? dt.Tables[0].Rows[i]["PartNo"].ToString() : "", 27, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["PartName"].ToString() != null ? dt.Tables[0].Rows[i]["PartName"].ToString() : "", 43, ' ', true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["DemandQty"].ToString() != null ? dt.Tables[0].Rows[i]["DemandQty"].ToString() : "", 13, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SupplyQty"].ToString() != null ? dt.Tables[0].Rows[i]["SupplyQty"].ToString() : "", 11, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["ReturnQty"].ToString() != null ? dt.Tables[0].Rows[i]["ReturnQty"].ToString() : "", 12, ' ', true, false, true, true, "n2");
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SSNo"].ToString() != null ? dt.Tables[0].Rows[i]["SSNo"].ToString() : "", 14, ' ', true, false, true);
                        gtf.SetDataDetail(dt.Tables[0].Rows[i]["SSReturn"].ToString() != null ? dt.Tables[0].Rows[i]["SSReturn"].ToString() : "", 18, ' ', false, true);
                        gtf.PrintData(false);
                        task = dt.Tables[0].Rows[i]["PartNo"].ToString();
                    }
                }
            }

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport015(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W80", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(setTextParameter[2].ToString(), 80, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO.", 4, ' ', true, false, true);
            gtf.SetGroupHeader("MODEL", 15, ' ', true);
            gtf.SetGroupHeader("KETERANGAN", 51, ' ', true);
            gtf.SetGroupHeader("TOTAL", 7, ' ', false, true, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            int noUrut = 0;
            decimal ttlNilai = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                noUrut++;
                gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                gtf.SetDataDetail(dt.Rows[i]["BasicModel"].ToString(), 15, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["Description"].ToString(), 51, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["BSAccount"].ToString(), 7, ' ', false, true, true, true, "n0");
                gtf.PrintData(true);

                ttlNilai += decimal.Parse(dt.Rows[i]["BSAccount"].ToString());
            }
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("TOTAL :", 72, ' ', true, false, true);
            gtf.SetTotalDetail(ttlNilai.ToString(), 7, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport016(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W136", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            string start = Convert.ToDateTime(setTextParameter[0].ToString()).ToString("dd-MMM-yyy");
            string end = Convert.ToDateTime(setTextParameter[1].ToString()).ToString("dd-MMM-yyy");

            string periode = "";

            //----------------------------------------------------------------
            //Header
            if (start != "")
            {
                periode = string.Format("Periode : {0} s/d {1}", start, end);
                gtf.SetGroupHeader(periode.ToString(), 136, ' ', false, true, false, true);
            }
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeaderSpace(5);
            gtf.SetGroupHeader("MODEL", 51, ' ', true, false, false, true);
            gtf.SetGroupHeader("PELANGGAN", 49, ' ', false, true, false, true);
            gtf.SetGroupHeaderSpace(5);
            gtf.SetGroupHeader("-", 51, '-', true);
            gtf.SetGroupHeader("-", 49, '-', false, true);
            gtf.SetGroupHeader("NO.", 4, ' ', true);
            gtf.SetGroupHeader("TIPE", 20, ' ', true);
            gtf.SetGroupHeader("KETERANGAN", 30, ' ', true);
            gtf.SetGroupHeader("KODE", 15, ' ', true);
            gtf.SetGroupHeader("NAMA", 33, ' ', true);
            gtf.SetGroupHeader("NO. POLISI", 10, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 18, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0, unit = 0, unitGrnd = 0;
            decimal ttlTotal = 0, ttlTotalGrand = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["BasicModel"].ToString();

                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["BasicModel"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Description"].ToString(), 30, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerCodeBill"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 33, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PoliceRegNo"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }
                else if (docNo != dt.Rows[i]["BasicModel"].ToString())
                {
                    gtf.SetDataDetailSpace(57);
                    gtf.SetDataDetail("-", 79, '-', false, true);
                    gtf.SetDataDetailSpace(57);
                    gtf.SetDataDetail("SUBTOTAL : " + unit.ToString("n0") + " UNITS", 60, ' ', true);
                    gtf.SetDataDetail(ttlTotal.ToString(), 18, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    ttlTotal = 0;
                    docNo = dt.Rows[i]["BasicModel"].ToString();

                    noUrut++;
                    gtf.SetDataDetail(" ", 1, ' ', false, true);
                    gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["BasicModel"].ToString(), 20, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["Description"].ToString(), 30, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerCodeBill"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 33, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PoliceRegNo"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }
                else if (docNo == dt.Rows[i]["BasicModel"].ToString())
                {
                    gtf.SetDataDetailSpace(57);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerCodeBill"].ToString(), 15, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["CustomerName"].ToString(), 33, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["PoliceRegNo"].ToString(), 10, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["TotalDPPAmt"].ToString(), 18, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }

                ttlTotal += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                ttlTotalGrand += Convert.ToDecimal(dt.Rows[i]["TotalDPPAmt"].ToString());
                unit++;
                unitGrnd++;
            }
            gtf.SetDataDetailSpace(57);
            gtf.SetDataDetail("-", 79, '-', false, true);
            gtf.SetDataDetailSpace(57);
            gtf.SetDataDetail("SUBTOTAL : " + unit.ToString("n0") + " UNITS", 60, ' ', true);
            gtf.SetDataDetail(ttlTotal.ToString(), 18, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataReportLine();
            gtf.SetDataDetailSpace(57);
            gtf.SetDataDetail("GRAND TOTAL : " + unitGrnd.ToString("n0") + " UNITS", 60, ' ', true);
            gtf.SetDataDetail(ttlTotalGrand.ToString(), 18, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport017(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            string periode = "";

            //----------------------------------------------------------------
            //Header

            periode = string.Format("Periode : {0} ", setTextParameter[1].ToString());
            gtf.SetGroupHeader(periode.ToString(), 96, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO.", 3, ' ', true, false, true);
            gtf.SetGroupHeader("DOKUMENT", 25, ' ', true, false, false, true);
            gtf.SetGroupHeader("PELANGGAN / PEMASOK", 31, ' ', true, false, false, true);
            gtf.SetGroupHeader("STATUS", 34, ' ', false, true);
            gtf.SetGroupHeaderSpace(4);
            gtf.SetGroupHeader("-", 25, '-', true);
            gtf.SetGroupHeader("-", 31, '-', false, true);
            gtf.SetGroupHeaderSpace(4);
            gtf.SetGroupHeader("NOMOR", 13, ' ', true);
            gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
            gtf.SetGroupHeader("KODE", 8, ' ', true);
            gtf.SetGroupHeader("NAMA", 22, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            int noUrut = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                noUrut++;
                gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                gtf.SetDataDetail(dt.Rows[i]["docNo"].ToString(), 13, ' ', true);
                gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["docDate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["custCode"].ToString(), 8, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["custName"].ToString(), 22, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["status"].ToString(), 34, ' ', false, true);
                gtf.PrintData(false);
            }
            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport022(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W163", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(setTextParameter[0].ToString(), 163, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO.", 4, ' ', true);
            gtf.SetGroupHeader("NO. SPK", 13, ' ', true);
            gtf.SetGroupHeader("NO. ORDER", 13, ' ', true);
            gtf.SetGroupHeader("TANGGAL", 11, ' ', true);
            gtf.SetGroupHeader("PEMILIK", 24, ' ', true);
            gtf.SetGroupHeader("MODEL", 12, ' ', true);
            gtf.SetGroupHeader("NO. POLISI", 12, ' ', true);
            gtf.SetGroupHeader("JENIS PEKERJAAN", 20, ' ', true);
            gtf.SetGroupHeader("SA / FM", 14, ' ', true);
            gtf.SetGroupHeader("FOREMAN", 14, ' ', true);
            gtf.SetGroupHeader("STATUS", 15, ' ', false, true);
            gtf.SetGroupHeaderSpace(45);
            gtf.SetGroupHeader("-", 102, '-', false, true);
            gtf.SetGroupHeaderSpace(45);
            gtf.SetGroupHeader("SUPPLIER", 24, ' ', true);
            gtf.SetGroupHeader("JUMLAH", 12, ' ', true, false, true);
            gtf.SetGroupHeader("PPN", 12, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 20, ' ', true, false, true);
            gtf.SetGroupHeader("USER", 46, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            int noUrut = 0;
            decimal ttlJumlah = 0, ttlPPN = 0, ttlTotal = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                noUrut++;
                gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 13, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["PONo"].ToString(), 13, ' ', true);
                gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["PODate"].ToString()).ToString("dd-MMM-yyyy"), 11, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["Customer"].ToString(), 24, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["BasicModel"].ToString(), 12, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["PoliceRegNo"].ToString(), 12, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["JobType"].ToString(), 20, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["SA"].ToString(), 14, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["Foreman"].ToString(), 14, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["Status"].ToString(), 15, ' ', false, true);
                gtf.SetDataDetailSpace(45);
                gtf.SetDataDetail("-", 102, '-', false, true);
                gtf.SetDataDetailSpace(45);
                gtf.SetDataDetail(dt.Rows[i]["Supplier"].ToString(), 24, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["DPPAmt"].ToString(), 12, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PPNAmt"].ToString(), 12, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["ServiceAmt"].ToString(), 20, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["LastUpdateBy"].ToString(), 46, ' ', false, true);
                gtf.PrintData(false);

                ttlJumlah += Convert.ToDecimal(dt.Rows[i]["DPPAmt"].ToString());
                ttlPPN += Convert.ToDecimal(dt.Rows[i]["PPNAmt"].ToString());
                ttlTotal += Convert.ToDecimal(dt.Rows[i]["ServiceAmt"].ToString());
            }
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("TOTAL :", 70, ' ', true, false, true);
            gtf.SetTotalDetail(ttlJumlah.ToString(), 12, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlPPN.ToString(), 12, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlTotal.ToString(), 20, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport023(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W233", print, fileLocation, fullPage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(setTextParameter[0].ToString(), 230, ' ', false, true, false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("NO.", 4, ' ', true);
            gtf.SetGroupHeader("NO. SPK", 14, ' ', true);
            gtf.SetGroupHeader("NO. ORDER", 14, ' ', true);
            gtf.SetGroupHeader("NO. RECEIVING", 14, ' ', true);
            gtf.SetGroupHeader("TANGGAL", 12, ' ', true);
            gtf.SetGroupHeader("PEMILIK", 35, ' ', true);
            gtf.SetGroupHeader("MODEL", 15, ' ', true);
            gtf.SetGroupHeader("NO. POLISI", 15, ' ', true);
            gtf.SetGroupHeader("JENIS PEKERJAAN", 30, ' ', true);
            gtf.SetGroupHeader("FAKTUR PAJAK", 13, ' ', true);
            gtf.SetGroupHeader("TGL. FPJ", 15, ' ', true);
            gtf.SetGroupHeader("SA / FM", 15, ' ', true);
            gtf.SetGroupHeader("STATUS", 25, ' ', false, true);
            gtf.SetGroupHeaderSpace(63);
            gtf.SetGroupHeader("-", 144, '-', false, true);
            gtf.SetGroupHeaderSpace(63);
            gtf.SetGroupHeader("SUPPLIER", 35, ' ', true);
            gtf.SetGroupHeader("JUMLAH", 15, ' ', true, false, true);
            gtf.SetGroupHeader("PPN", 15, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 30, ' ', true, false, true);
            gtf.SetGroupHeader("NOTA/REFF", 13, ' ', true);
            gtf.SetGroupHeader("USER", 15, ' ', true);
            gtf.SetGroupHeader("FOREMAN", 41, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.PrintHeader();

            int noUrut = 0;
            decimal ttlJumlah = 0, ttlPPN = 0, ttlTotal = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                noUrut++;
                gtf.SetDataDetail(noUrut.ToString(), 4, '0', true, false, true);
                gtf.SetDataDetail(dt.Rows[i]["JobOrderNo"].ToString(), 14, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["PONo"].ToString(), 14, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["RecNo"].ToString(), 14, ' ', true);
                gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["RecDate"].ToString()).ToString("dd-MMM-yyyy"), 12, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["Customer"].ToString(), 35, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["BasicModel"].ToString(), 15, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["PoliceRegNo"].ToString(), 15, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["JobType"].ToString(), 30, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["FPJGovNo"].ToString(), 13, ' ', true);
                gtf.SetDataDetail(Convert.ToDateTime(dt.Rows[i]["FPJDate"].ToString()).ToString("dd-MMM-yyyy"), 15, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["SA"].ToString(), 15, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["Status"].ToString(), 25, ' ', false, true);
                gtf.SetDataDetailSpace(63);
                gtf.SetDataDetail("-", 144, '-', false, true);
                gtf.SetDataDetailSpace(63);
                gtf.SetDataDetail(dt.Rows[i]["Supplier"].ToString(), 35, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["DPPAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["PPNAmt"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["ServiceAmt"].ToString(), 30, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["FPJNo"].ToString(), 13, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["LastUpdateBy"].ToString(), 15, ' ', true);
                gtf.SetDataDetail(dt.Rows[i]["Foreman"].ToString(), 41, ' ', false, true);
                gtf.PrintData(false);

                ttlJumlah += Convert.ToDecimal(dt.Rows[i]["DPPAmt"].ToString());
                ttlPPN += Convert.ToDecimal(dt.Rows[i]["PPNAmt"].ToString());
                ttlTotal += Convert.ToDecimal(dt.Rows[i]["ServiceAmt"].ToString());
            }
            gtf.SetTotalDetailLine();
            gtf.SetTotalDetail("TOTAL :", 98, ' ', true, false, true);
            gtf.SetTotalDetail(ttlJumlah.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlPPN.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetail(ttlTotal.ToString(), 30, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();

            if (print == true)
                gtf.PrintTotal();
            else
            {
                //if (gtf.PrintTotal() == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private string CreateReportSvRpReport026(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W80", print, fileLocation, false);

            //----------------------------------------------------------------
            //Header
            gtf.SetGroupHeader(paramReport, paramReport.Length, ' ');
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("|", 22, ' ');
            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeader(" ", 34, ' ');
            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeader("NO.", 4, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(dt.Rows[0]["InvoiceNo"].ToString(), 14, ' ', true);
            gtf.SetGroupHeader("|", 1, ' ', false, true);

            gtf.SetGroupHeader("|", 22, ' ');
            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeader("IZIN KELUAR KENDARAAN", 34, ' ', false, false, false, true);
            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeader("-", 21, '-');
            gtf.SetGroupHeader("|", 1, ' ', false, true);

            gtf.SetGroupHeader("|", 22, ' ');
            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeader(" ", 34, ' ');
            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeader("TGL.", 4, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(setTextParameter[1].ToString(), 14, ' ', true);
            gtf.SetGroupHeader("|", 1, ' ', false, true);

            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeader("-", 78, '-');
            gtf.SetGroupHeader("|", 1, ' ', false, true);

            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeaderSpace(2);
            gtf.SetGroupHeader("PEMILIK KENDARAAN", 76, ' ');
            gtf.SetGroupHeader("|", 1, ' ', false, true);

            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeaderSpace(2);
            gtf.SetGroupHeader("NAMA", 7, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(dt.Rows[0]["CustomerName"].ToString(), 67, ' ');
            gtf.SetGroupHeader("|", 1, ' ', false, true);

            string alamat = dt.Rows[0]["Address1"].ToString() + " " + dt.Rows[0]["Address2"].ToString() + " " + dt.Rows[0]["Address3"].ToString() + " " + dt.Rows[0]["Address4"].ToString();
            string[] arrayAlamat = gtf.ConvertArrayTerbilang(alamat, 67);

            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeaderSpace(2);
            gtf.SetGroupHeader("ALAMAT", 7, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayAlamat[0].ToString().Replace(" rupiah", " "), 67, ' ');
            gtf.SetGroupHeader("|", 1, ' ', false, true);

            if (arrayAlamat[1] != null)
            {
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader(" ", 7, ' ');
                gtf.SetGroupHeader(" ", 1, ' ', true);
                gtf.SetGroupHeader(arrayAlamat[1].ToString().Replace(" rupiah", " "), 67, ' ');
                gtf.SetGroupHeader("|", 1, ' ', false, true);
            }
            else
            {
                gtf.SetGroupHeader("|", 1, ' ');
                gtf.SetGroupHeaderSpace(2);
                gtf.SetGroupHeader(" ", 7, ' ');
                gtf.SetGroupHeader(" ", 1, ' ', true);
                gtf.SetGroupHeader(" ", 67, ' ');
                gtf.SetGroupHeader("|", 1, ' ', false, true);
            }

            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeader("-", 78, '-');
            gtf.SetGroupHeader("|", 1, ' ', false, true);

            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeader("DATA KENDARAAN", 78, ' ', false, false, false, true);
            gtf.SetGroupHeader("|", 1, ' ', false, true);

            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeader("-", 78, '-');
            gtf.SetGroupHeader("|", 1, ' ', false, true);

            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeaderSpace(2);
            gtf.SetGroupHeader("MERK", 11, ' ', true);
            gtf.SetGroupHeader("TYPE", 11, ' ', true);
            gtf.SetGroupHeader("JENIS", 11, ' ', true);
            gtf.SetGroupHeader("NO. RANGKA", 12, ' ', true);
            gtf.SetGroupHeader("NO. MESIN", 12, ' ', true);
            gtf.SetGroupHeader("NO. POLISI", 11, ' ', true);
            gtf.SetGroupHeader("  |", 3, ' ', false, true);

            gtf.SetGroupHeader("|", 1, ' ');
            gtf.SetGroupHeader("-", 78, '-');
            gtf.SetGroupHeader("|", 1, ' ', false, true);

            gtf.PrintHeader();

            gtf.SetDataDetail("|", 1, ' ');
            gtf.SetDataDetailSpace(2);
            gtf.SetDataDetail(setTextParameter[0].ToString(), 11, ' ', true);
            gtf.SetDataDetail(dt.Rows[0]["BasicModel"].ToString(), 11, ' ', true);
            gtf.SetDataDetail(dt.Rows[0]["Description"].ToString(), 11, ' ', true);
            gtf.SetDataDetail(dt.Rows[0]["ChassisNo"].ToString(), 12, ' ', true);
            gtf.SetDataDetail(dt.Rows[0]["EngineNo"].ToString(), 12, ' ', true);
            gtf.SetDataDetail(dt.Rows[0]["PoliceRegNo"].ToString(), 11, ' ', true);
            gtf.SetDataDetailSpace(2);
            gtf.SetDataDetail("|", 1, ' ', false, true);
            gtf.PrintData(false);

            gtf.SetDataDetail("|", 1, ' ');
            gtf.SetDataDetail("-", 78, '-');
            gtf.SetDataDetail("|", 1, ' ', false, true);

            gtf.SetDataDetail("|", 1, ' ');
            gtf.SetDataDetailSpace(2);
            gtf.SetDataDetail("Harap diijinkan pembawa dokumen ini untuk membawa kendaraannya keluar", 76, ' ');
            gtf.SetDataDetail("|", 1, ' ', false, true);

            gtf.SetDataDetail("|", 1, ' ');
            gtf.SetDataDetail("-", 78, '-');
            gtf.SetDataDetail("|", 1, ' ', false, true);

            gtf.SetDataDetail("|", 1, ' ');
            gtf.SetDataDetailSpace(5);
            gtf.SetDataDetail("Telah menerima mobil berikut", 30, ' ');
            gtf.SetDataDetailSpace(10);

            string place = dt.Rows[0]["LookUpValueName"].ToString() + ", " + setTextParameter[1].ToString();
            gtf.SetDataDetail(place, 33, ' ');
            gtf.SetDataDetail("|", 1, ' ', false, true);
            gtf.PrintData(false);

            gtf.SetDataDetail("|", 1, ' ');
            gtf.SetDataDetailSpace(5);
            gtf.SetDataDetail("peralatannya dengan lengkap", 30, ' ');
            gtf.SetDataDetailSpace(43);
            gtf.SetDataDetail("|", 1, ' ', false, true);
            gtf.PrintData(false);

            gtf.SetDataDetail("|", 1, ' ');
            gtf.SetDataDetailSpace(78);
            gtf.SetDataDetail("|", 1, ' ', false, true);
            gtf.PrintData(false);

            gtf.SetDataDetail("|", 1, ' ');
            gtf.SetDataDetailSpace(78);
            gtf.SetDataDetail("|", 1, ' ', false, true);
            gtf.PrintData(false);

            gtf.SetDataDetail("|", 1, ' ');
            gtf.SetDataDetailSpace(5);
            gtf.SetDataDetail("-", 30, '-');
            gtf.SetDataDetailSpace(10);

            gtf.SetDataDetail("-", 29, '-');
            gtf.SetDataDetailSpace(4);
            gtf.SetDataDetail("|", 1, ' ', false, true);
            gtf.PrintData(false);

            gtf.SetDataDetail("|", 1, ' ');
            gtf.SetDataDetailSpace(5);
            gtf.SetDataDetail("Pemilik / Wakil", 30, ' ');
            gtf.SetDataDetailSpace(10);

            gtf.SetDataDetail("Frontman", 33, ' ');
            gtf.SetDataDetail("|", 1, ' ', false, true);
            gtf.SetDataReportLine();
            gtf.PrintData(false);

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

        private string CreateReportSvRpReport028(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPage, object[] setTextParameter)
        {
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W80", print, fileLocation, true);
            gtf.GenerateHeader();

            gtf.SetGroupHeader(dt.Rows[0]["pPeriode"].ToString(), 80, ' ', false, true, false, true);
            string KdBranch = "KODE CABANG : " + dt.Rows[0]["BranchCode"].ToString();
            gtf.SetGroupHeader(KdBranch, 80, ' ', false, true);
            gtf.SetGroupHeaderLine();
            gtf.SetGroupHeader("BASIC MODEL", 16, ' ', true);
            gtf.SetGroupHeader("FSC#", 4, ' ', true, false, true);
            gtf.SetGroupHeader("QTY", 10, ' ', true, false, true);
            gtf.SetGroupHeader("MATERIAL", 15, ' ', true, false, true);
            gtf.SetGroupHeader("JASA", 15, ' ', true, false, true);
            gtf.SetGroupHeader("AMOUNT", 15, ' ', false, true, true);
            gtf.SetGroupHeaderLine();

            gtf.PrintHeader();

            string basicModel = string.Empty;
            string branchCode = string.Empty;
            int counterData = 0;
            decimal decSubTotQty = 0, decSubTotMaterial = 0, decSubTotJasa = 0, decSubTotAmount = 0;
            decimal decTotQty = 0, decTotMaterial = 0, decTotJasa = 0, decTotAmount = 0;
            int loop = 0;
            DataRow[] dRow = new DataRow[] { };

            for (int i = 0; i <= dt.Rows.Count; i++)
            {
                if (i == dt.Rows.Count)
                {
                    gtf.SetDataReportLine();
                    gtf.PrintData();
                    gtf.SetDataDetail("SUB TOTAL " + basicModel + " : ", 21, ' ', true);
                    gtf.SetDataDetail(decSubTotQty.ToString(), 10, ' ', true, false, true);
                    gtf.SetDataDetail(decSubTotMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(decSubTotJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(decSubTotAmount.ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData();

                    decTotQty += decSubTotQty;
                    decTotMaterial += decSubTotMaterial;
                    decTotJasa += decSubTotJasa;
                    decTotAmount += decSubTotAmount;
                    break;
                }

                if (branchCode == string.Empty)
                {
                    branchCode = dt.Rows[i]["BranchCode"].ToString();
                    if (basicModel == string.Empty)
                    {
                        basicModel = dt.Rows[i]["BasicModel"].ToString();
                    }
                }
                else
                {
                    if (branchCode != dt.Rows[i]["BranchCode"].ToString())
                    {
                        if (basicModel != dt.Rows[i]["BasicModel"].ToString())
                        {
                            gtf.SetDataReportLine();
                            gtf.PrintData();
                            gtf.SetDataDetail("SUB TOTAL " + basicModel + " : ", 21, ' ', true);
                            gtf.SetDataDetail(decSubTotQty.ToString(), 10, ' ', true, false, true);
                            gtf.SetDataDetail(decSubTotMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotAmount.ToString(), 15, ' ', false, true, true, true, "n0");
                            gtf.PrintData();

                            gtf.SetDataDetail("", 80, ' ', false, true);
                            gtf.PrintData();

                            basicModel = dt.Rows[i]["BasicModel"].ToString();
                            if (gtf.line == 57)
                            {
                                dRow = dt.Select(string.Format("BranchCode = '{0}' and BasicModel = '{1}'", branchCode, basicModel));
                                if (dRow.Length == 1)
                                {
                                    gtf.SetDataDetail("", 80, ' ', false, true);
                                    gtf.PrintData();
                                    gtf.SetDataDetail("", 80, ' ', false, true);
                                    gtf.PrintData();
                                }
                            }
                            counterData = 0;

                            decTotQty += decSubTotQty;
                            decTotMaterial += decSubTotMaterial;
                            decTotJasa += decSubTotJasa;
                            decTotAmount += decSubTotAmount;

                            decSubTotQty = decSubTotMaterial = decSubTotJasa = decSubTotAmount = 0;
                        }

                        branchCode = dt.Rows[i]["BranchCode"].ToString();
                        gtf.ReplaceGroupHdr(KdBranch, "KODE CABANG : " + branchCode);
                        KdBranch = "KODE CABANG : " + branchCode;

                        loop = 59 - gtf.line;
                        for (int j = 0; j < loop; j++)
                        {
                            gtf.SetDataDetail("", 80, ' ', false, true);
                            gtf.PrintData();
                        }
                    }
                    else
                    {
                        if (basicModel != dt.Rows[i]["BasicModel"].ToString())
                        {
                            gtf.SetDataReportLine();
                            gtf.PrintData();
                            gtf.SetDataDetail("SUB TOTAL " + basicModel + " : ", 21, ' ', true);
                            gtf.SetDataDetail(decSubTotQty.ToString(), 10, ' ', true, false, true);
                            gtf.SetDataDetail(decSubTotMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotJasa.ToString(), 15, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(decSubTotAmount.ToString(), 15, ' ', false, true, true, true, "n0");
                            gtf.PrintData();

                            gtf.SetDataDetail("", 80, ' ', false, true);
                            gtf.PrintData();

                            basicModel = dt.Rows[i]["BasicModel"].ToString();
                            if (gtf.line == 57)
                            {
                                dRow = dt.Select(string.Format("BranchCode = '{0}' and BasicModel = '{1}'", branchCode, basicModel));
                                if (dRow.Length == 1)
                                {
                                    gtf.SetDataDetail("", 80, ' ', false, true);
                                    gtf.PrintData();
                                    gtf.SetDataDetail("", 80, ' ', false, true);
                                    gtf.PrintData();
                                }
                            }
                            counterData = 0;

                            decTotQty += decSubTotQty;
                            decTotMaterial += decSubTotMaterial;
                            decTotJasa += decSubTotJasa;
                            decTotAmount += decSubTotAmount;

                            decSubTotQty = decSubTotMaterial = decSubTotJasa = decSubTotAmount = 0;
                        }
                    }
                }

                if (counterData == 0)
                    gtf.SetDataDetail(basicModel, 16, ' ', true);
                else
                    gtf.SetDataDetailSpace(17);
                gtf.SetDataDetail(dt.Rows[i]["PdiFsc"].ToString(), 4, ' ', true, false, true);
                gtf.SetDataDetail(dt.Rows[i]["Qty"].ToString(), 10, ' ', true, false, true);
                gtf.SetDataDetail(dt.Rows[i]["Material"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["Jasa"].ToString(), 15, ' ', true, false, true, true, "n0");
                gtf.SetDataDetail(dt.Rows[i]["Amount"].ToString(), 15, ' ', false, true, true, true, "n0");

                if (gtf.line == 57)
                {
                    dRow = dt.Select(string.Format("BranchCode = '{0}' and BasicModel = '{1}'", branchCode, basicModel));
                    if (counterData == dRow.Length - 2)
                    {
                        gtf.SetDataDetail("", 80, ' ', false, true);
                        gtf.PrintData();
                        gtf.SetDataDetail("", 80, ' ', false, true);
                    }
                }

                gtf.PrintData();

                decSubTotQty += Convert.ToDecimal(dt.Rows[i]["Qty"]);
                decSubTotMaterial += Convert.ToDecimal(dt.Rows[i]["Material"]);
                decSubTotJasa += Convert.ToDecimal(dt.Rows[i]["Jasa"]);
                decSubTotAmount += Convert.ToDecimal(dt.Rows[i]["Amount"]);
                counterData += 1;
            }

            if (gtf.line > 48)
            {
                loop = (59 - gtf.line) + (60 - 22);
            }
            else
            {
                loop = (60 - 11) - gtf.line;
            }

            for (int i = 0; i < loop; i++)
            {
                gtf.SetDataDetail("", 80, ' ', false, true);
                gtf.PrintData();
            }

            gtf.SetDataReportLine();
            gtf.PrintData();
            gtf.SetDataDetail("GRAND TOTAL :", 21, ' ', true, false, true);
            gtf.SetDataDetail(decTotQty.ToString(), 10, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(decTotMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(decTotJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            gtf.SetDataDetail(decTotAmount.ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData();
            gtf.SetDataReportLine();
            gtf.PrintData();
            gtf.SetDataDetail("", 80, ' ', false, true);
            gtf.PrintData();
            gtf.SetDataDetailSpace(3);
            gtf.SetDataDetail("Diterima Oleh", 13, ' ');
            gtf.SetDataDetailSpace(27);
            gtf.SetDataDetail("Disiapkan Oleh", 14, ' ');
            gtf.SetDataDetailSpace(8);
            gtf.SetDataDetail("Mengetahui", 10, ' ', false, true);
            gtf.PrintData();
            gtf.SetDataDetail("", 80, ' ', false, true);
            gtf.PrintData();
            gtf.SetDataDetail("", 80, ' ', false, true);
            gtf.PrintData();
            gtf.SetDataDetail("", 80, ' ', false, true);
            gtf.PrintData();
            gtf.SetDataDetail("(                  )", 20, ' ');
            gtf.SetDataDetailSpace(20);
            gtf.SetDataDetail("(                 )", 19, ' ', true);
            gtf.SetDataDetail("(                  )", 20, ' ', false, true);
            gtf.PrintData();
            gtf.SetDataDetail("", 80, ' ', false, true);
            gtf.PrintData();

            //gtf.SetTotalDetailLine();
            //gtf.SetTotalDetail("GRAND TOTAL :", 21, ' ', true, false, true);
            //gtf.SetTotalDetail(decTotQty.ToString(), 10, ' ', true, false, true, true, "n0");
            //gtf.SetTotalDetail(decTotMaterial.ToString(), 15, ' ', true, false, true, true, "n0");
            //gtf.SetTotalDetail(decTotJasa.ToString(), 15, ' ', true, false, true, true, "n0");
            //gtf.SetTotalDetail(decTotAmount.ToString(), 15, ' ', false, true, true, true, "n0");
            //gtf.SetTotalDetailLine();
            //gtf.SetTotalDetail("", 80, ' ', false, true);
            //gtf.SetTotalDetailSpace(3);
            //gtf.SetTotalDetail("Diterima Oleh", 13, ' ');
            //gtf.SetTotalDetailSpace(27);
            //gtf.SetTotalDetail("Disiapkan Oleh", 14, ' ');
            //gtf.SetTotalDetailSpace(8);
            //gtf.SetTotalDetail("Mengetahui", 10, ' ', false, true);
            //gtf.SetTotalDetail("", 80, ' ', false, true);
            //gtf.SetTotalDetail("", 80, ' ', false, true);
            //gtf.SetTotalDetail("", 80, ' ', false, true);
            //gtf.SetTotalDetail("(                  )", 20, ' ');
            //gtf.SetTotalDetailSpace(20);
            //gtf.SetTotalDetail("(                 )", 19, ' ', true);
            //gtf.SetTotalDetail("(                  )", 20, ' ', false, true);
            //gtf.SetTotalDetail("", 80, ' ', false, true);

            if (print == true)
                gtf.PrintTotal(false, false, true);
            else
            {
                //if (gtf.PrintTotal(true, false, true) == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            }

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        #endregion
    }
}