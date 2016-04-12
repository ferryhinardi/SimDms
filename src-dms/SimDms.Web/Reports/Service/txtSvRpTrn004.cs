using SimDms.Common;
using SimDms.Service.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;

namespace SimDms.Web.Reports.Service
{
    public class txtSvRpTrn004:IRptProc
    {
        public Models.SysUser CurrentUser { get; set; }
        private SimDms.Service.Models.DataContext ctx = new SimDms.Service.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        private object[] setTextParameter;
        private object[] prm;

        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam,string paramReport)
        {
            var dt = ctx.Database.SqlQuery<SvRpTrn004>(string.Format("exec {0} {1}", sproc, sparam)).ToList();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "exec " + sproc + " " + sparam;
            cmd.CommandType = CommandType.Text;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dtt = new DataTable();
            da.Fill(dtt);
            setTextParameter = oparam;
            prm = setTextParameter[0].ToString().Split(',');// Input Pesanan,JAN PRIHATMOKO,ga

            if (rptId == "SvRpTrn004001" || rptId == "SvRpTrn004001H")
            {
                return CreateReportSvRpTrn004001(rptId, dtt, paramReport, printerloc, print, "", rptId == "SvRpTrn004001");
            }
            else if (rptId == "SvRpTrn004002" || rptId == "SvRpTrn004002H")
            {
                return CreateReportSvRpTrn004002(rptId, dtt, sparam, printerloc, print, "", rptId == "SvRpTrn004002");
            }
            else return CreateReportSvRpTrn004(rptId, dt, paramReport, printerloc, print, "", fullpage);
        }
        private string CreateReportSvRpTrn004(string recordId, List<SvRpTrn004> dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPages)
        {
            if (dt.Count() == 0)
                return "Tidak Ada Data";
            string[] arrayHeader = new string[24];
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1400, 1100);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation, fullPages);
            gtf.GenerateHeader();
            
            
            //----------------------------------------------------------------
            //Header
            string alamat = (dt[0].CustAddr1!= null ? dt[0].CustAddr1.ToString() : "") + " " + (dt[0].CustAddr2!= null ? dt[0].CustAddr2.ToString() : "") + " "
                + (dt[0].CustAddr3!= null ? dt[0].CustAddr3.ToString() : "") + " " + (dt[0].CustAddr4!= null ? dt[0].CustAddr4.ToString() : "");
            string[] listAlamat = gtf.ConvertArrayText(alamat, 32);

            arrayHeader[0] = dt[0].NPWPNo!= null ? dt[0].NPWPNo.ToString() : "";
            arrayHeader[1] = dt[0].PKPNo!= null ? dt[0].PKPNo.ToString() : "";
            arrayHeader[2] = dt[0].PKPDate!= null ? Convert.ToDateTime(dt[0].PKPDate.ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[3] = dt[0].InvoiceNo!= null ? dt[0].InvoiceNo.ToString() : "";
            arrayHeader[4] = dt[0].FPJNo!= null ? dt[0].FPJNo.ToString() : "";
            arrayHeader[5] = dt[0].CustomerName!= null ? dt[0].CustomerName.ToString() : "";
            arrayHeader[6] = dt[0].PoliceRegNo!= null ? dt[0].PoliceRegNo.ToString() : "";
            arrayHeader[7] = dt[0].FPJDate.ToString() != "" ? Convert.ToDateTime(dt[0].FPJDate.ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[8] = listAlamat[0] != null ? listAlamat[0].ToString() : "";
            arrayHeader[9] = dt[0].BasicModel!= null ? dt[0].BasicModel.ToString() : "";
            arrayHeader[10] = dt[0].Payment!= null ? dt[0].Payment.ToString() : "";
            arrayHeader[11] = listAlamat[1] != null ? listAlamat[1].ToString() : "";
            arrayHeader[12] = dt[0].ProductionYear!= null ? dt[0].ProductionYear.ToString() : "";
            arrayHeader[13] = dt[0].DueDate!= null ? Convert.ToDateTime(dt[0].DueDate.ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[14] = listAlamat[2] != null ? listAlamat[2].ToString() : "";
            arrayHeader[15] = dt[0].Odometer!= null ? dt[0].Odometer.ToString() : "";
            arrayHeader[16] = dt[0].JobOrderNo!= null ? dt[0].JobOrderNo.ToString() : "";
            arrayHeader[17] = listAlamat[3] != null ? listAlamat[3].ToString() : "";
            arrayHeader[18] = dt[0].JobOrderDate!= null ? Convert.ToDateTime(dt[0].JobOrderDate.ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[19] = dt[0].CustPhone!= null ? dt[0].CustPhone.ToString() : "";
            arrayHeader[20] = dt[0].Chassis!= null ? dt[0].Chassis.ToString() : "";
            arrayHeader[21] = dt[0].CustNPWP!= null ? dt[0].CustNPWP.ToString() : "";
            arrayHeader[22] = dt[0].Engine!= null ? dt[0].Engine.ToString() : "";
            arrayHeader[23] = dt[0].CustomerCode!= null ? dt[0].CustomerCode.ToString() : "";

            CreateHdrSvRpTrn004(gtf, arrayHeader);
            gtf.PrintHeader();

            string docNo = "", city = "";
            int noUrut = 0;
            string[] nominal;
            string[] remark;

            for (int i = 0; i < dt.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt[i].InvoiceNo.ToString();

                    gtf.SetDataDetail("KELUHAN", 96, ' ', false, true);

                    string[] descs = gtf.ConvertArrayList(dt[i].ServiceRequestDesc!= null ? dt[i].ServiceRequestDesc.ToString() : "", 96, 10);
                    foreach (string desc in descs)
                    {
                        if (desc != "")
                        {
                            gtf.SetDataDetailSpace(2);
                            gtf.SetDataDetail(desc.ToString(), 96, ' ', false, true);
                            gtf.PrintData(true);
                        }
                        else
                            break;
                    }

                    if (dt[i].dataType.ToString() == "JASA PERBAIKAN" || dt[i].dataType.ToString() == "LAIN - LAIN")
                    {
                        if (dt[i].PartName.ToString() == "")
                            gtf.SetDataDetailLineBreak();
                        else if (dt[i].PartName.ToString() == "JASA PERBAIKAN" || dt[i].PartName.ToString() == "LAIN - LAIN")
                            gtf.SetDataDetail(dt[i].PartName.ToString(), 96, ' ', false, true);
                        else
                        {
                            noUrut++;
                            gtf.SetDataDetail(noUrut.ToString(), 3, ' ', true, false, true);
                            gtf.SetDataDetail(dt[i].PartName.ToString(), 75, ' ', true);
                            gtf.SetDataDetail(dt[i].totalPrice.ToString(), 18, ' ', false, true, true, true, "n0");
                        }
                    }
                    else if (dt[i].dataType.ToString() == "PEMAKAIAN SPAREPART / MATERIAL")
                    {
                        if (dt[i].PartName.ToString() == "")
                        {
                            gtf.SetDataDetailLineBreak();
                            noUrut = 0;
                        }
                        else if (dt[i].PartName .ToString() == "PEMAKAIAN SPAREPART / MATERIAL")
                            gtf.SetDataDetail(dt[i].PartName.ToString(), 96, ' ', false, true);
                        else
                        {
                            noUrut++;
                            gtf.SetDataDetail(noUrut.ToString(), 3, ' ', true, false, true);
                            gtf.SetDataDetail(dt[i].PartNo.ToString(), 18, ' ', true);
                            gtf.SetDataDetail(dt[i].PartName.ToString(), 30, ' ', true);
                            gtf.SetDataDetail(dt[i].supplyQty.ToString(), 8, ' ', true, false, true, true, "n2");
                            gtf.SetDataDetail(dt[i].retailPrice.ToString(), 16, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt[i].totalPrice.ToString(), 16, ' ', false, true, true, true, "n0");
                        }
                    }
                    gtf.PrintData(false);
                }
                else if (docNo != dt[i].InvoiceNo.ToString())
                {
                    gtf.SetDataDetailSpace(78);
                    if (setTextParameter[2].ToString() == "False")
                        gtf.SetDataDetail("-", 18, '-', false, true);
                    else
                        gtf.SetDataDetail(" ", 18, ' ', false, true);
                    gtf.SetDataDetailSpace(78);
                    gtf.SetDataDetail(dt[i - 1].TotalDtlAmt.ToString(), 18, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                    gtf.SetDataDetailSpace(66);
                    gtf.SetDataDetail("Potongan", 10, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt[i - 1].DiscAmt.ToString(), 18, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                    if (setTextParameter[2].ToString() == "False")
                        gtf.SetDataDetail("-", 96, '-', false, true);
                    else
                        gtf.SetDataDetail(" ", 96, ' ', false, true);
                    gtf.SetDataDetail("Terbilang :", 11, ' ', true);
                    nominal = gtf.ConvertArrayTerbilang(gtf.Terbilang(Convert.ToInt64(dt[i - 1].TotalSrvAmt.ToString())), 53);
                    gtf.SetDataDetail(nominal[0] != null ? nominal[0].ToString() : "", 53, ' ', true);
                    gtf.SetDataDetail("Total", 10, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt[i - 1].TotalDPPAmt.ToString(), 18, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                    gtf.SetDataDetailSpace(12);
                    gtf.SetDataDetail(nominal[1] != null ? nominal[1].ToString() : "", 53, ' ', true);
                    gtf.SetDataDetail(dt[i - 1].TaxPct.ToString(), 10, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt[i - 1].TaxAmt.ToString(), 18, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                    gtf.SetDataDetailSpace(78);
                    if (setTextParameter[2].ToString() == "False")
                        gtf.SetDataDetail("-", 18, '-', false, true);
                    else
                        gtf.SetDataDetail(" ", 18, ' ', false, true);
                    gtf.SetDataDetailSpace(78);
                    gtf.SetDataDetail(dt[i - 1].TotalSrvAmt.ToString(), 18, ' ', false, true, true, true, "n0");
                    if (setTextParameter[2].ToString() == "False")
                        gtf.SetDataDetail("-", 96, '-', false, true);
                    else
                        gtf.SetDataDetail(" ", 96, ' ', false, true);
                    gtf.PrintData(false);

                    remark = gtf.ConvertArrayText(dt[dt.Count - 1].Remark.Replace(System.Environment.NewLine,"").ToString(), 46);

                    if (dt[i - 1].City.ToString() != "" && dt[i - 1].InvoiceDate.ToString() != "")
                        city = string.Format("{0}", dt[i - 1].City!= null ? dt[i - 1].City.ToString() : "", dt[i - 1].InvoiceDate!= null ? Convert.ToDateTime(dt[i - 1].InvoiceDate.ToString()).ToString("dd-MMM-yyyy") : "");

                    gtf.SetTotalDetail("Informasi :", 11, ' ', true);
                    gtf.SetTotalDetail(remark[0] != null ? remark[0] : "", 46, ' ', true);
                    gtf.SetTotalDetail(city, 20, ' ', false, true);
                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(remark[1] != null ? remark[1] : "", 46, ' ', false, true);
                    //gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(dt[i - 1].InvoiceDate.Value.ToString("dd-MMM-yyyy"), 20, ' ', false, true);
                    gtf.SetTotalDetail(remark[2] != null ? remark[2] : "", 46, ' ', true);
                    gtf.SetTotalDetail(setTextParameter[1] != null ? paramReport[1].ToString() : "", 20, ' ', false, true);
                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(remark[3] != null ? remark[3] : "", 46, ' ', true);
                    if (setTextParameter[2].ToString() == "False")
                        gtf.SetDataDetail("-", 20, '-', false, true);
                    else
                        gtf.SetDataDetail(" ", 20, ' ', false, true);
                    gtf.SetTotalDetailSpace(12);
                    gtf.SetTotalDetail(remark[4] != null ? remark[4] : "", 46, ' ', true);
                    gtf.SetTotalDetail(setTextParameter[2] != null ? paramReport[2].ToString() : "", 20, ' ', false, true);
                    gtf.PrintTotal(false, false, true);
                }
                else
                {
                    if (dt[i].dataType.ToString() == "JASA PERBAIKAN" || dt[i].dataType.ToString() == "LAIN - LAIN")
                    {
                        if (dt[i].PartName.ToString() == "")
                            gtf.SetDataDetailLineBreak();
                        else if (dt[i].PartName.ToString() == "JASA PERBAIKAN"  || dt[i].PartName.ToString() == "LAIN - LAIN")
                            gtf.SetDataDetail(dt[i].PartName.ToString(), 96, ' ', false, true);
                        else
                        {
                            noUrut++;
                            gtf.SetDataDetail(noUrut.ToString(), 3, ' ', true, false, true);
                            gtf.SetDataDetail(dt[i].PartName.ToString(), 75, ' ', true);
                            gtf.SetDataDetail(dt[i].totalPrice.ToString(), 16, ' ', false, true, true, true, "n0");
                        }
                    }
                    else if (dt[i].dataType.ToString() == "PEMAKAIAN SPAREPART / MATERIAL")
                    {
                        if (dt[i].PartName.ToString() == "")
                        {
                            gtf.SetDataDetailLineBreak();
                            noUrut = 0;
                        }
                        else if (dt[i].PartName.ToString() == "PEMAKAIAN SPAREPART / MATERIAL")
                            gtf.SetDataDetail(dt[i].PartName.ToString(), 96, ' ', false, true);
                        else
                        {
                            noUrut++;
                            gtf.SetDataDetail(noUrut.ToString(), 3, ' ', true, false, true);
                            gtf.SetDataDetail(dt[i].PartNo.ToString(), 18, ' ', true);
                            gtf.SetDataDetail(dt[i].PartName.ToString(), 30, ' ', true);
                            gtf.SetDataDetail(dt[i].supplyQty.ToString(), 8, ' ', true, false, true, true, "n2");
                            gtf.SetDataDetail(dt[i].retailPrice.ToString(), 16, ' ', true, false, true, true, "n0");
                            gtf.SetDataDetail(dt[i].totalPrice.ToString(), 16, ' ', false, true, true, true, "n0");
                        }
                        gtf.PrintData(false);
                    }
                }
            }            
            gtf.SetDataDetailSpace(78);
            gtf.PrintData(false,false,true);
            if (setTextParameter[2].ToString() == "False")
                gtf.SetDataDetail("-", 18, '-', false, true);
            else
                gtf.SetDataDetail(" ", 18, ' ', false, true);
            gtf.SetDataDetailSpace(78);
            gtf.SetDataDetail(dt[dt.Count - 1].TotalDtlAmt.ToString(), 18, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);
            gtf.SetDataDetailSpace(64);
            gtf.SetDataDetail("Potongan", 12, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt[dt.Count - 1].DiscAmt.ToString(), 18, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);
            if (setTextParameter[2].ToString() == "False")
                gtf.SetDataDetail("-", 96, '-', false, true);
            else
                gtf.SetDataDetail(" ", 96, ' ', false, true);
            gtf.SetDataDetail("Terbilang :", 11, ' ', true);
            nominal = gtf.ConvertArrayTerbilang(gtf.Terbilang(Convert.ToInt64(dt[dt.Count - 1].TotalSrvAmt.ToString())), 51);
            gtf.SetDataDetail(nominal[0] != null ? nominal[0].ToString() : "", 51, ' ', true);
            gtf.SetDataDetail("Total", 12, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt[dt.Count - 1].TotalDPPAmt.ToString(), 18, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);
            gtf.SetDataDetailSpace(12);
            gtf.SetDataDetail(nominal[1] != null ? nominal[1].ToString() : "", 51, ' ', true);
            gtf.SetDataDetail("PPN (" + dt[dt.Count - 1].TaxPct.ToString() + "%)", 12, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt[dt.Count - 1].TaxAmt.ToString(), 18, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            gtf.SetDataDetailSpace(12);
            gtf.SetDataDetail(nominal[2] != null ? nominal[1].ToString() : "", 51, ' ', true);
            gtf.SetDataDetailSpace(14);
            if (setTextParameter[2].ToString() == "False")
                gtf.SetDataDetail("-", 18, '-', false, true);
            else
                gtf.SetDataDetail(" ", 18, ' ', false, true);
            gtf.SetDataDetailSpace(78);
            gtf.SetDataDetail(dt[dt.Count - 1].TotalSrvAmt.ToString(), 18, ' ', false, true, true, true, "n0");
            if (setTextParameter[2].ToString() == "False")
                gtf.SetDataDetail("-", 96, '-', false, true);
            else
                gtf.SetDataDetail(" ", 96, ' ', false, true);
            gtf.PrintData(false);


            remark = gtf.ConvertArrayText(dt[dt.Count - 1].Remark.ToString().Replace(System.Environment.NewLine, ""), 45);
            


            if (dt[dt.Count - 1].City.ToString() != "" && dt[dt.Count - 1].InvoiceDate.ToString() != "")
                city = string.Format("{0}", dt[dt.Count - 1].City!= null ? dt[dt.Count - 1].City.ToString() : "", dt[dt.Count - 1].InvoiceDate!= null ? Convert.ToDateTime(dt[dt.Count - 1].InvoiceDate.ToString()).ToString("dd-MMM-yyyy") : "");

            var user = CurrentUser;// SysUser.Current;

            string paraValue = "";
            var invfrm = ctx.LookUpDtls.Where(x => x.CompanyCode == user.CompanyCode && x.CodeID == "INV_FRM" && x.LookUpValue == "FRM_SRV").FirstOrDefault();
            if (invfrm != null)
            {
                paraValue = invfrm.ParaValue ?? "";
            }

            string keterangan = "";
            var ket = ctx.LookUpDtls.Where(x => x.CompanyCode == user.CompanyCode && x.CodeID == "NOTE" && x.LookUpValue == "SVFP01AGJ").FirstOrDefault();//.LookUpValueName;
            if (ket != null)
            {
                keterangan = ket.LookUpValueName ?? "";
            } 
            
            if (paraValue == "AGJ")
            {
                gtf.SetTotalDetail("Keterangan :", 12, ' ', true);
                string[] ketArray = gtf.ConvertArrayText(keterangan, 51);
                gtf.SetTotalDetail(ketArray[0], 51, ' ', false, true);
                if (ketArray[1] != null)
                {
                    gtf.SetTotalDetailSpace(13);
                    gtf.SetTotalDetail(ketArray[1], 51, ' ', false, true);
                }
            }
            gtf.SetTotalDetail("Informasi  :", 12, ' ', true);
            gtf.SetTotalDetail(remark[0] != null ? remark[0].ToString().Replace("\n","") : "", 50, ' ', true);
            gtf.SetTotalDetail(city, 20, ' ', false, true);
            gtf.SetTotalDetailSpace(13);
            gtf.SetTotalDetail(remark[1] != null ? remark[1].ToString().Replace("\n","") : "", 50, ' ',  true);
            gtf.SetTotalDetail(dt[dt.Count - 1].InvoiceDate.Value.ToString("dd-MMM-yyyy"), 20, ' ', false, true);
            gtf.SetTotalDetailSpace(13);
            gtf.SetTotalDetail(remark[2] != null ?remark[2].ToString().Replace("\n","") : "", 53, ' ', true);

            gtf.SetTotalDetail( "", 20, ' ', false, true); //employee name
            
            gtf.SetTotalDetailSpace(13);
            gtf.SetTotalDetail(remark[3] != null ? remark[3].ToString().Trim() : "", 50, ' ', true,true);
          
            gtf.SetTotalDetailSpace(13);
            gtf.SetTotalDetail(remark[4] != null ? remark[4].ToString().Trim() : "", 48, ' ', true,false);
            gtf.SetTotalDetail(setTextParameter[0] != null ? setTextParameter[0].ToString() : "", 20, ' ', false, true); //employee name

            gtf.SetTotalDetailSpace(63);

            if (setTextParameter[2].ToString() == "False")
                gtf.SetTotalDetail("-", 20, '-', false, true);
            else
                gtf.SetTotalDetail(" ", 20, ' ', false, true);

            //gtf.SetTotalDetail("", 1, ' ', false, true);
            //gtf.SetTotalDetail("", 1, ' ', false, true);
            
            gtf.SetTotalDetailSpace(64);
            gtf.SetTotalDetail(setTextParameter[1] != null ? setTextParameter[1].ToString() : "", 20, ' ', false, true); //jabatan

            if (print == true)
                gtf.PrintTotal(true, false, false);
            else
            {
                if (gtf.PrintTotal(true, false, false) == true)
                    gtf.sbDataTxt.ToString();
                else
                    return "";
            }

            

            return gtf.sbDataTxt.ToString();
        }

        private void CreateHdrSvRpTrn004(GenerateTextFileReport gtf, string[] arrayHeader)
        {
            gtf.CleanHeader();
            gtf.SetGroupHeader("N.P.W.P", 9, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[0].ToString(), 25, ' ', false, true);

            gtf.SetGroupHeader("No. PKP", 9, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[1].ToString(), 25, ' ', true);
            gtf.SetGroupHeader("Tgl. PKP", 9, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[2].ToString(), 19, ' ', true);
            gtf.SetGroupHeader("Nomor", 12, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[3].ToString(), 13, ' ', false, true);

            gtf.SetGroupHeaderSpace(68);
            gtf.SetGroupHeader("Nomor F.P.S", 12, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[4].ToString(), 13, ' ', false, true);


            gtf.SetGroupHeader("Untuk", 9, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader("(" + arrayHeader[23].ToString() + ")", 10, ' ', false);
            gtf.SetGroupHeader(arrayHeader[5].ToString(), 22, ' ', true);

            gtf.SetGroupHeader("No. Polisi", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[6].ToString(), 10, ' ', true);
            gtf.SetGroupHeader("Tgl. Faktur", 12, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[7].ToString(), 13, ' ', false, true);

            gtf.SetGroupHeaderSpace(11);
            gtf.SetGroupHeader(arrayHeader[8].ToString(), 32, ' ', true);
            gtf.SetGroupHeader("Type/Model", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[9].ToString(), 10, ' ', true);
            gtf.SetGroupHeader("Pembayaran", 12, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[10].ToString(), 13, ' ', false, true);

            gtf.SetGroupHeaderSpace(11);
            gtf.SetGroupHeader(arrayHeader[11].ToString(), 32, ' ', true);
            gtf.SetGroupHeader("TAHUN", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[12].ToString(), 10, ' ', true);
            gtf.SetGroupHeader("Jatuh Tempo", 12, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[13].ToString(), 13, ' ', false, true);

            gtf.SetGroupHeaderSpace(11);
            gtf.SetGroupHeader(arrayHeader[14].ToString(), 32, ' ', true);
            gtf.SetGroupHeader("Kilometer", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[15].ToString(), 10, ' ', true);
            gtf.SetGroupHeader("No. S.P.K.", 12, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[16].ToString(), 13, ' ', false, true);

            gtf.SetGroupHeaderSpace(11);
            gtf.SetGroupHeader(arrayHeader[17].ToString(), 32, ' ', true);
            gtf.SetGroupHeaderSpace(24);
            gtf.SetGroupHeader("Tgl. S.P.K.", 12, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[18].ToString(), 13, ' ', false, true);

            gtf.SetGroupHeader("No. Telp", 9, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[19].ToString(), 32, ' ', true);
            gtf.SetGroupHeader("No. Rangka", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[20].ToString(), 22, ' ', false, true);

            gtf.SetGroupHeader("N.P.W.P.", 9, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[21].ToString(), 32, ' ', true);
            gtf.SetGroupHeader("No. Mesin", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[22].ToString(), 22, ' ', false, true);


            if (setTextParameter[2].ToString() == "False")
                gtf.SetGroupHeader("-", 96, '-', false, true);
            else
                gtf.SetGroupHeader(" ", 96, ' ', false, true);

            gtf.SetGroupHeader("No.", 3, ' ', true, false, true);
            gtf.SetGroupHeader("KETERANGAN", 49, ' ', true);
            gtf.SetGroupHeader("JUMLAH", 8, ' ', true, false, true);
            gtf.SetGroupHeader("HARGA SATUAN", 16, ' ', true, false, true);
            gtf.SetGroupHeader("TOTAL", 16, ' ', false, true, true);

            if (setTextParameter[2].ToString() == "False")
                gtf.SetGroupHeader("-", 96, '-', false, true);
            else
                gtf.SetGroupHeader(" ", 96, ' ', false, true);
        }

        private string CreateReportSvRpTrn004001(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullpage)
        {
            string[] arrayHeader = new string[7];
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);

            gtf.GenerateTextFileReports(recordId, printerLoc, "W80", print, fileLocation, fullpage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header

            arrayHeader[0] = dt.Rows[0]["PONo"] != null ? dt.Rows[0]["PONo"].ToString() : "";
            arrayHeader[1] = dt.Rows[0]["Supplier"] != null ? dt.Rows[0]["Supplier"].ToString() : "";
            arrayHeader[2] = dt.Rows[0]["PoDate"] != null ? Convert.ToDateTime(dt.Rows[0]["PoDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[3] = dt.Rows[0]["JobOrderNo"] != null ? dt.Rows[0]["JobOrderNo"].ToString() : "";
            arrayHeader[4] = dt.Rows[0]["JobOrderDate"] != null ? Convert.ToDateTime(dt.Rows[0]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[5] = dt.Rows[0]["PoliceRegNo"] != null ? dt.Rows[0]["PoliceRegNo"].ToString() : "";
            arrayHeader[6] = dt.Rows[0]["Remarks"] != null ? dt.Rows[0]["Remarks"].ToString() : "";

            CreateHdrSvRpTrn00401(gtf, arrayHeader);
            gtf.PrintHeader();

            string docNo = "", city = "";
            int noUrut = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["PONo"].ToString();

                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["Description"].ToString(), 59, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["POPrice"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);
                }
                else if (docNo != dt.Rows[i]["PONo"].ToString())
                {

                    gtf.SetDataReportLine();

                    gtf.SetDataDetailSpace(46);
                    gtf.SetDataDetail("Diskon ", 6, ' ');
                    gtf.SetDataDetail(dt.Rows[i - 1]["PODisc"].ToString() != "" ? "(" + dt.Rows[i - 1]["PODisc"].ToString() + "%)" : "(0.00 %)", 11, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["DiscAmt"].ToString(), 15, ' ', false, true, true, true, "n0");

                    gtf.SetDataDetailSpace(46);
                    gtf.SetDataDetail("Jumlah", 17, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["DppAmt"].ToString(), 15, ' ', false, true, true, true, "n0");

                    gtf.SetDataDetailSpace(46);
                    gtf.SetDataDetail("PPN ", 6, ' ');
                    gtf.SetDataDetail(dt.Rows[i - 1]["PpnPct"].ToString() != "" ? "(" + dt.Rows[i - 1]["PpnPct"].ToString() + "%)" : "(0.00 %)", 11, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["PpnAmt"].ToString(), 15, ' ', false, true, true, true, "n0");

                    gtf.SetDataDetailSpace(46);
                    gtf.SetDataDetail("Total", 17, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["ServiceAmt"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    city = (dt.Rows[i - 1]["City"].ToString() != null) ? dt.Rows[i - 1]["City"].ToString() + ", " + DateTime.Now.ToString("dd-MMM-yyyy") : DateTime.Now.ToString("dd-MMM-yyyy");

                    gtf.SetTotalDetailSpace(50);
                    gtf.SetTotalDetail(city, 20, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);

                    gtf.SetTotalDetail("Dikerjakan Oleh : ", 49, ' ', true);
                    gtf.SetTotalDetail(prm[0].ToString(), 20, ' ', false, true);

                    gtf.SetTotalDetail(prm[2].ToString(), 49, ' ', true);
                    gtf.SetTotalDetail("-", 20, '-', false, true);

                    gtf.SetTotalDetailSpace(50);
                    gtf.SetTotalDetail(prm[1].ToString(), 20, ' ', false, true);

                    gtf.PrintTotal(false, false, false);

                    arrayHeader[0] = dt.Rows[i]["PONo"] != null ? dt.Rows[i]["PONo"].ToString() : "";
                    arrayHeader[1] = dt.Rows[i]["Supplier"] != null ? dt.Rows[i]["Supplier"].ToString() : "";
                    arrayHeader[2] = dt.Rows[i]["PoDate"] != null ? Convert.ToDateTime(dt.Rows[i]["PoDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[3] = dt.Rows[i]["JobOrderNo"] != null ? dt.Rows[i]["JobOrderNo"].ToString() : "";
                    arrayHeader[4] = dt.Rows[i]["JobOrderDate"] != null ? Convert.ToDateTime(dt.Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[5] = dt.Rows[i]["PoliceRegNo"] != null ? dt.Rows[i]["PoliceRegNo"].ToString() : "";
                    arrayHeader[6] = dt.Rows[i]["Remarks"] != null ? dt.Rows[i]["Remarks"].ToString() : "";

                    CreateHdrSvRpTrn00401(gtf, arrayHeader);
                    gtf.PrintAfterBreak();

                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["Description"].ToString(), 59, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["POPrice"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    docNo = dt.Rows[i]["PONo"].ToString();
                }
                else
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["Description"].ToString(), 59, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["POPrice"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }
            }

            gtf.SetDataReportLine();

            gtf.SetDataDetailSpace(46);
            gtf.SetDataDetail("Diskon ", 6, ' ');
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["PODisc"].ToString() != "" ? "(" + dt.Rows[dt.Rows.Count - 1]["PODisc"].ToString() + "%)" : "(0.00 %)", 11, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["DiscAmt"].ToString(), 15, ' ', false, true, true, true, "n0");

            gtf.SetDataDetailSpace(46);
            gtf.SetDataDetail("Jumlah", 17, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["DppAmt"].ToString(), 15, ' ', false, true, true, true, "n0");

            gtf.SetDataDetailSpace(46);
            gtf.SetDataDetail("PPN ", 6, ' ');
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["PpnPct"].ToString() != "" ? "(" + dt.Rows[dt.Rows.Count - 1]["PpnPct"].ToString() + "%)" : "(0.00 %)", 11, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["PpnAmt"].ToString(), 15, ' ', false, true, true, true, "n0");

            gtf.SetDataDetailSpace(46);
            gtf.SetDataDetail("Total", 17, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["ServiceAmt"].ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            city = (dt.Rows[dt.Rows.Count - 1]["City"].ToString() != null) ? dt.Rows[dt.Rows.Count - 1]["City"].ToString() + ", " + DateTime.Now.ToString("dd-MMM-yyyy") : DateTime.Now.ToString("dd-MMM-yyyy");

            gtf.SetTotalDetailSpace(50);
            gtf.SetTotalDetail(city, 20, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);

            gtf.SetTotalDetail("Dikerjakan Oleh : ", 49, ' ', true);
            gtf.SetTotalDetail(prm[0].ToString(), 20, ' ', false, true);

            gtf.SetTotalDetail(prm[2].ToString(), 49, ' ', true);
            gtf.SetTotalDetail("-", 20, '-', false, true);

            gtf.SetTotalDetailSpace(50);
            gtf.SetTotalDetail(prm[1].ToString(), 20, ' ', false, true);

            gtf.PrintTotal(true, false, false);


            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private void CreateHdrSvRpTrn00401(GenerateTextFileReport gtf, string[] arrayHeader)
        {
            gtf.CleanHeader();
            gtf.SetGroupHeader("Pemasok / Supplier :", 51, ' ', true);

            gtf.SetGroupHeader("Nomor", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[0].ToString(), 15, ' ', false, true);

            gtf.SetGroupHeader(arrayHeader[1].ToString(), 51, ' ', true);
            gtf.SetGroupHeader("Tanggal", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[2].ToString(), 15, ' ', false, true);

            gtf.SetGroupHeaderSpace(52);
            gtf.SetGroupHeader("No. SPK", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[3].ToString(), 15, ' ', false, true);

            gtf.SetGroupHeaderSpace(52);
            gtf.SetGroupHeader("Tgl. SPK", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[4].ToString(), 15, ' ', false, true);

            gtf.SetGroupHeaderSpace(52);
            gtf.SetGroupHeader("No. Polisi", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[5].ToString(), 15, ' ', false, true);

            gtf.SetGroupHeader(" ", 1, ' ', false, true);

            gtf.SetGroupHeader("NOTE : ", 9, ' ', false, true);
            gtf.SetGroupHeader(arrayHeader[6].ToString(), 80, ' ', false, true);

            gtf.SetGroupHeader("-", 80, '-', false, true);
            gtf.SetGroupHeader("No.", 4, ' ', true, false, true);
            gtf.SetGroupHeader("KETERANGAN", 59, ' ', true);
            gtf.SetGroupHeader("HARGA", 15, ' ', false, true, true);
            gtf.SetGroupHeader("-", 80, '-', false, true);
        }

        private string CreateReportSvRpTrn004002(string recordId, DataTable dt, string paramReport, string printerLoc, bool print, string fileLocation,bool fullpage)
        {
            string[] arrayHeader = new string[9];
            //----------------------------------------------------------------
            //Default Parameter
            GenerateTextFileReport gtf = new GenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W80", print, fileLocation,fullpage);
            gtf.GenerateHeader();

            //----------------------------------------------------------------
            //Header

            arrayHeader[0] = dt.Rows[0]["RecNo"] != null ? dt.Rows[0]["RecNo"].ToString() : "";
            arrayHeader[1] = dt.Rows[0]["Supplier"] != null ? dt.Rows[0]["Supplier"].ToString() : "";
            arrayHeader[2] = dt.Rows[0]["RecDate"] != null ? Convert.ToDateTime(dt.Rows[0]["RecDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[3] = dt.Rows[0]["PONo"] != null ? dt.Rows[0]["PONo"].ToString() : "";
            arrayHeader[4] = dt.Rows[0]["PODate"] != null ? Convert.ToDateTime(dt.Rows[0]["PODate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[5] = dt.Rows[0]["JobOrderNo"] != null ? dt.Rows[0]["JobOrderNo"].ToString() : "";
            arrayHeader[6] = dt.Rows[0]["JobOrderDate"] != null ? Convert.ToDateTime(dt.Rows[0]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy") : "";
            arrayHeader[7] = dt.Rows[0]["PoliceRegNo"] != null ? dt.Rows[0]["PoliceRegNo"].ToString() : "";
            arrayHeader[8] = dt.Rows[0]["Remarks"] != null ? dt.Rows[0]["Remarks"].ToString() : "";

            CreateHdrSvRpTrn00402(gtf, arrayHeader);
            gtf.PrintHeader();

            string docNo = "", city = "";
            int noUrut = 0;
            string[] nominal;
            string[] remark;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt.Rows[i]["RecNo"].ToString();

                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["Description"].ToString(), 59, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["POPrice"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(true);
                }
                else if (docNo != dt.Rows[i]["RecNo"].ToString())
                {
                    gtf.SetDataReportLine();

                    gtf.SetDataDetail("No Faktur/Nota", 17, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["FPJNo"].ToString() != "" ? dt.Rows[i - 1]["FPJNo"].ToString() : "", 26, ' ', true);

                    gtf.SetDataDetail("Diskon ", 6, ' ');
                    gtf.SetDataDetail(dt.Rows[i - 1]["PODisc"].ToString() != "" ? "(" + dt.Rows[i - 1]["PODisc"].ToString() + "%)" : "(0.00 %)", 11, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["DiscAmt"].ToString(), 15, ' ', false, true, true, true, "n0");

                    gtf.SetDataDetail("Tgl Faktur/Nota", 17, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["FPJDate"].ToString() != "" ? Convert.ToDateTime(dt.Rows[i - 1]["FPJDate"].ToString()).ToString("dd-MMM-yyyy") : "", 26, ' ', true);

                    gtf.SetDataDetail("Jumlah", 17, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["DppAmt"].ToString(), 15, ' ', false, true, true, true, "n0");

                    gtf.SetDataDetail("No Seri Pajak", 17, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["FPJGovNo"].ToString() != "" ? dt.Rows[i - 1]["FPJGovNo"].ToString() : "", 26, ' ', true);

                    gtf.SetDataDetail("PPN ", 6, ' ');
                    gtf.SetDataDetail(dt.Rows[i - 1]["PpnPct"].ToString() != "" ? "(" + dt.Rows[i - 1]["PpnPct"].ToString() + "%)" : "(0.00 %)", 11, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["PpnAmt"].ToString(), 15, ' ', false, true, true, true, "n0");

                    gtf.SetDataDetail("Waktu Pembayaran", 17, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["DueDate"].ToString() != "" ? dt.Rows[i - 1]["DueDate"].ToString() : "", 26, ' ', true);

                    gtf.SetDataDetail("Total", 17, ' ');
                    gtf.SetDataDetail(":", 1, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i - 1]["ServiceAmt"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);

                    city = (dt.Rows[i - 1]["City"].ToString() != null) ? dt.Rows[i - 1]["City"].ToString() + ", " + DateTime.Now.ToString("dd-MMM-yyyy") : DateTime.Now.ToString("dd-MMM-yyyy");

                    gtf.SetTotalDetailSpace(50);
                    gtf.SetTotalDetail(city, 20, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);
                    gtf.SetTotalDetail(" ", 1, ' ', false, true);

                    gtf.SetTotalDetail("Dikerjakan Oleh : ", 49, ' ', true);
                    gtf.SetTotalDetail(setTextParameter[0].ToString(), 20, ' ', false, true);

                    gtf.SetTotalDetail(setTextParameter[2].ToString(), 49, ' ', true);
                    gtf.SetTotalDetail("-", 20, '-', false, true);

                    gtf.SetTotalDetailSpace(50);
                    gtf.SetTotalDetail(setTextParameter[1].ToString(), 20, ' ', false, true);
                    gtf.PrintTotal(false, false, false);


                    arrayHeader[0] = dt.Rows[i]["RecNo"] != null ? dt.Rows[i]["RecNo"].ToString() : "";
                    arrayHeader[1] = dt.Rows[i]["Supplier"] != null ? dt.Rows[i]["Supplier"].ToString() : "";
                    arrayHeader[2] = dt.Rows[i]["RecDate"] != null ? Convert.ToDateTime(dt.Rows[i]["RecDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[3] = dt.Rows[i]["PONo"] != null ? dt.Rows[i]["PONo"].ToString() : "";
                    arrayHeader[4] = dt.Rows[i]["PODate"] != null ? Convert.ToDateTime(dt.Rows[i]["PODate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[5] = dt.Rows[i]["JobOrderNo"] != null ? dt.Rows[i]["JobOrderNo"].ToString() : "";
                    arrayHeader[6] = dt.Rows[i]["JobOrderDate"] != null ? Convert.ToDateTime(dt.Rows[i]["JobOrderDate"].ToString()).ToString("dd-MMM-yyyy") : "";
                    arrayHeader[7] = dt.Rows[i]["PoliceRegNo"] != null ? dt.Rows[i]["PoliceRegNo"].ToString() : "";
                    arrayHeader[8] = dt.Rows[i]["Remarks"] != null ? dt.Rows[i]["Remarks"].ToString() : "";

                    CreateHdrSvRpTrn00402(gtf, arrayHeader);
                    gtf.PrintAfterBreak();

                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["Description"].ToString(), 59, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["POPrice"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                    docNo = dt.Rows[i]["RecNo"].ToString();
                }
                else
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 4, ' ', true, false, true);
                    gtf.SetDataDetail(dt.Rows[i]["Description"].ToString(), 59, ' ', true);
                    gtf.SetDataDetail(dt.Rows[i]["POPrice"].ToString(), 15, ' ', false, true, true, true, "n0");
                    gtf.PrintData(false);
                }
            }

            gtf.SetDataReportLine();

            gtf.SetDataDetail("No Faktur/Nota", 17, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["FPJNo"].ToString() != "" ? dt.Rows[dt.Rows.Count - 1]["FPJNo"].ToString() : "", 26, ' ', true);

            gtf.SetDataDetail("Diskon ", 6, ' ');
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["PODisc"].ToString() != "" ? "(" + dt.Rows[dt.Rows.Count - 1]["PODisc"].ToString() + "%)" : "(0.00 %)", 11, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["DiscAmt"].ToString(), 15, ' ', false, true, true, true, "n0");

            gtf.SetDataDetail("Tgl Faktur/Nota", 17, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["FPJDate"].ToString() != "" ? Convert.ToDateTime(dt.Rows[dt.Rows.Count - 1]["FPJDate"].ToString()).ToString("dd-MMM-yyyy") : "", 26, ' ', true);

            gtf.SetDataDetail("Jumlah", 17, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["DppAmt"].ToString(), 15, ' ', false, true, true, true, "n0");

            gtf.SetDataDetail("No Seri Pajak", 17, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["FPJGovNo"].ToString() != "" ? dt.Rows[dt.Rows.Count - 1]["FPJGovNo"].ToString() : "", 26, ' ', true);

            gtf.SetDataDetail("PPN ", 6, ' ');
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["PpnPct"].ToString() != "" ? "(" + dt.Rows[dt.Rows.Count - 1]["PpnPct"].ToString() + "%)" : "(0.00 %)", 11, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["PpnAmt"].ToString(), 15, ' ', false, true, true, true, "n0");

            gtf.SetDataDetail("Waktu Pembayaran", 17, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["DueDate"].ToString() != "" ? dt.Rows[dt.Rows.Count - 1]["DueDate"].ToString() : "", 26, ' ', true);

            gtf.SetDataDetail("Total", 17, ' ');
            gtf.SetDataDetail(":", 1, ' ', true);
            gtf.SetDataDetail(dt.Rows[dt.Rows.Count - 1]["ServiceAmt"].ToString(), 15, ' ', false, true, true, true, "n0");
            gtf.PrintData(false);

            city = (dt.Rows[dt.Rows.Count - 1]["City"].ToString() != null) ? dt.Rows[dt.Rows.Count - 1]["City"].ToString() + ", " + DateTime.Now.ToString("dd-MMM-yyyy") : DateTime.Now.ToString("dd-MMM-yyyy");
            gtf.SetTotalDetail("",1,' ',false,true);
            gtf.SetTotalDetailSpace(50);
            gtf.SetTotalDetail(city, 20, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);
            gtf.SetTotalDetail(" ", 1, ' ', false, true);

            var prm= setTextParameter[0].ToString().Split(',');// Input Penerimaan,JAN PRIHATMOKO,ga
            gtf.SetTotalDetail("Dikerjakan Oleh : ", 49, ' ', true);
            gtf.SetTotalDetail(prm[0], 20, ' ', false, true);

            gtf.SetTotalDetail(prm[2], 49, ' ', true);
            gtf.SetTotalDetail("-", 20, '-', false, true);

            gtf.SetTotalDetailSpace(50);
            gtf.SetTotalDetail(prm[1].ToString(), 20, ' ', false, true);

            //if (setTextParameter[2].ToString() == "False")
            //    gtf.SetDataDetail("-", 18, '-', false, true);
            //else
            //    gtf.SetDataDetail(" ", 18, ' ', false, true);

            //gtf.line -= 10;
            //if (print == true)
                gtf.PrintTotal(true, false, false);
           // else
           // {
                //if (gtf.PrintTotal(true, false, false) == true)
                //    XMessageBox.ShowInformation("Save Berhasil");
                //else
                //    XMessageBox.ShowWarning("Save Gagal");
            //}

            if (print == true)
                gtf.CloseConnectionPrinter();

            return gtf.sbDataTxt.ToString();
        }

        private void CreateHdrSvRpTrn00402(GenerateTextFileReport gtf, string[] arrayHeader)
        {
            gtf.CleanHeader();
            gtf.SetGroupHeader("Pemasok / Supplier :", 51, ' ', true);

            gtf.SetGroupHeader("Nomor", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[0].ToString(), 15, ' ', false, true);

            gtf.SetGroupHeader(arrayHeader[1].ToString(), 51, ' ', true);
            gtf.SetGroupHeader("Tanggal", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[2].ToString(), 15, ' ', false, true);

            gtf.SetGroupHeaderSpace(52);
            gtf.SetGroupHeader("No. PO", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[3].ToString(), 15, ' ', false, true);

            gtf.SetGroupHeaderSpace(52);
            gtf.SetGroupHeader("Tgl. PO", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[4].ToString(), 15, ' ', false, true);

            gtf.SetGroupHeaderSpace(52);
            gtf.SetGroupHeader("No. SPK", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[5].ToString(), 15, ' ', false, true);

            gtf.SetGroupHeaderSpace(52);
            gtf.SetGroupHeader("Tgl. SPK", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[6].ToString(), 15, ' ', false, true);

            gtf.SetGroupHeaderSpace(52);
            gtf.SetGroupHeader("No. Polisi", 11, ' ');
            gtf.SetGroupHeader(":", 1, ' ', true);
            gtf.SetGroupHeader(arrayHeader[7].ToString(), 15, ' ', false, true);

            //gtf.SetGroupHeader(" ", 1, ' ', false,);

            gtf.SetGroupHeader("NOTE : ", 9, ' ', false, false);
            gtf.SetGroupHeader(arrayHeader[8].ToString(), 71, ' ', false, true);

            gtf.SetGroupHeader("-", 80, '-', false, true);
            gtf.SetGroupHeader("No.", 4, ' ', true, false, true);
            gtf.SetGroupHeader("KETERANGAN", 59, ' ', true);
            gtf.SetGroupHeader("HARGA", 15, ' ', false, true, true);
            gtf.SetGroupHeader("-", 80, '-', false, true);
        }



    }
}