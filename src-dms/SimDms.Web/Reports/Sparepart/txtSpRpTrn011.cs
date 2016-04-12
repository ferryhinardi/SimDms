using SimDms.Common;
using SimDms.Sparepart.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Web.Reports.Sparepart
{
    public class txtSpRpTrn011 : IRptProc
    {
        public Models.SysUser CurrentUser { get; set; }
        private SimDms.Sparepart.Models.DataContext ctx = new SimDms.Sparepart.Models.DataContext(MyHelpers.GetConnString("DataContext"));
        private object[] setTextParameter;

        public string CreateReport(string rptId, string sproc, string sparam, string printerloc, bool print, bool fullpage, object[] oparam,string paramReport)
        {
            var dt = ctx.Database.SqlQuery<SpRpTrn011>(string.Format("exec {0} {1}", sproc, sparam)).ToList();
            setTextParameter = oparam;
            return CreateReportSpRpTrn011(rptId, dt, paramReport, printerloc, print, "", fullpage);// (rptId, dt, sparam, printerloc, print, "", fullpage);

        }
        private string CreateReportSpRpTrn011(string recordId, List<SpRpTrn011> dt, string paramReport, string printerLoc, bool print, string fileLocation, bool fullPages)
        {

            
            string[] arrayHeader = new string[8];

            SpGenerateTextFileReport gtf = new SpGenerateTextFileReport();
            gtf.CurrentUser = CurrentUser;
            gtf.SetParameterPrinter(paramReport);
            gtf.SetPaperSize(1100, 850);
            gtf.GenerateTextFileReports(recordId, printerLoc, "W96", print, fileLocation, fullPages);
            gtf.GenerateHeader(true);

            arrayHeader[0] = dt[0].CustomerName;
            arrayHeader[1] = dt[0].Address1??"";
            arrayHeader[2] = dt[0].Address2 ?? "";
            arrayHeader[3] = dt[0].Address3 ?? "";
            arrayHeader[4] = dt[0].FPJNo ?? "";
            arrayHeader[5] = dt[0].PickingSlipNo ?? "";
            arrayHeader[6] = dt[0].NPWPNo ?? "";

            CreateHdrSpRpTrn011(gtf, arrayHeader, true);
            gtf.PrintHeader();

            string docNo = "";
            int noUrut = 0;
            decimal ttlQty = 0, ttlNilai = 0, ttlJumlah = 0;
            bool lastData = false;
            string[] terbilang;
            for (int i = 0; i < dt.Count; i++)
            {
                if (docNo == "")
                {
                    docNo = dt[i].FPJNo;
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt[i].PartNo, 15, ' ', true);
                    gtf.SetDataDetail(dt[i].PartName, 15, ' ', true);
                    gtf.SetDataDetail(dt[i].QtyBill.Value.ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt[i].RetailPrice.Value.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt[i].DiscPct.Value.ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt[i].DiscAmt.Value.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt[i].NetRetailPrice.Value.ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt[i].NetSalesAmt.Value.ToString(), 12, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPages);
                }
                else if (docNo != dt[i].FPJNo)
                {

                    gtf.SetTotalDetailLine();

                    gtf.SetTotalDetail("Total : ", 35, ' ', true, false);
                    gtf.SetTotalDetail(ttlQty.ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetTotalDetailSpace(18);
                    gtf.SetTotalDetail(ttlNilai.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetTotalDetailSpace(11);
                    gtf.SetTotalDetail(ttlJumlah.ToString(), 12, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailLine();

                    terbilang = gtf.ConvertArrayTerbilang(gtf.Terbilang(Int64.Parse(dt[i - 1].TotFinalSalesAmt.Value.ToString())), 48);

                    gtf.SetTotalDetail("Terbilang", 15, ' ', true);
                    gtf.SetTotalDetail(terbilang[0].ToString().ToUpper(), 48, ' ', true);
                    gtf.SetTotalDetail("D.P.P", 12, ' ', true);
                    gtf.SetTotalDetail(":", 1, ' ');
                    gtf.SetTotalDetail(dt[i - 1].TotDppAmt.Value.ToString(), 17, ' ', false, true, true, true, "n0");
                    gtf.SetTotalDetailSpace(16);
                    gtf.SetTotalDetail((terbilang[1] != null) ? terbilang[1].ToString().ToUpper() : string.Empty, 48, ' ', true);
                    gtf.SetTotalDetail(dt[i - 1].TaxPct.Value.ToString() != "" ? "PPN " + dt[i - 1].TaxPct + "%" : string.Empty, 12, ' ', true);
                    gtf.SetTotalDetail(":", 1, ' ');
                    gtf.SetTotalDetail(dt[i - 1].TotPPNAmt.Value.ToString(), 17, ' ', false, true, true, true, "n0");
                    if (terbilang[2] != null)
                    {
                        gtf.SetTotalDetailSpace(16);
                        gtf.SetTotalDetail(terbilang[2].ToString().ToUpper(), 48, ' ', true);
                        gtf.SetTotalDetail("JUMLAH", 12, ' ', true);
                        gtf.SetTotalDetail(":", 1, ' ');
                        gtf.SetTotalDetail(dt[i - 1].TotFinalSalesAmt.Value.ToString(), 17, ' ', false, true, true, true, "n0");

                        gtf.SetTotalDetail(dt[i - 1].TOPC != "" ? "Dibayar Dengan : " + dt[i - 1].TOPC : string.Empty, 40, ' ', true);
                        gtf.SetTotalDetail("No.Order :", 10, ' ', true);
                        gtf.SetTotalDetail(dt[i - 1].OrderNo != "" ? dt[i - 1].OrderNo : "-", 18, ' ', true);

                        gtf.SetTotalDetail((dt[i - 1].CITY != "") ? dt[i - 1].CITY.ToUpper() + ", " + ((dt[i - 1].FPJDate != null) ? dt[i - 1].FPJDate.Value.ToString("dd-MM-yyyy") : string.Empty) : string.Empty, 31, ' ', false, true);
                        gtf.SetTotalDetail((dt[i - 1].DueDate.Value.ToString() != "") ? "Jatuh Tempo    : " + Convert.ToDateTime(dt[i - 1].DueDate.Value).ToString("dd-MM-yyyy") : string.Empty, 65, ' ', false, true);
                    }
                    else
                    {
                        gtf.SetTotalDetail(dt[i - 1].TOPC != "" ? "Dibayar Dengan : " + dt[i - 1].TOPC : string.Empty, 39, ' ', true);
                        gtf.SetTotalDetail("No.Order :", 10, ' ', true);
                        gtf.SetTotalDetail(dt[i - 1].OrderNo != "" ? dt[i - 1].OrderNo : "-", 13, ' ', true);
                        gtf.SetTotalDetail("JUMLAH", 12, ' ', true);
                        gtf.SetTotalDetail(":", 1, ' ');
                        gtf.SetTotalDetail(dt[i - 1].TotFinalSalesAmt.Value.ToString(), 17, ' ', false, true, true, true, "n0");

                        gtf.SetTotalDetail((dt[i - 1].DueDate.Value.ToString() != "") ? "Jatuh Tempo    : " + Convert.ToDateTime(dt[i - 1].DueDate).ToString("dd-MM-yyyy") : string.Empty, 70, ' ', true);
                        gtf.SetTotalDetail((dt[i - 1].CITY != "") ? dt[i - 1].CITY.ToUpper() + ", " + ((dt[i - 1].FPJDate != null) ?dt[i - 1].FPJDate.Value.ToString("dd-MM-yyyy") : string.Empty) : string.Empty, 31, ' ', false, true);
                    }
                    gtf.SetTotalDetail("KETERANGAN :", 12, ' ', true);
                    gtf.SetTotalDetail(dt[i - 1].Note1, 58, ' ', false, true);
                    gtf.SetTotalDetail(dt[i - 1].Note2, 70, ' ', true);
                    gtf.SetTotalDetail(dt[i - 1].SignName, 25, ' ', false, true);
                    gtf.SetTotalDetail(dt[i - 1].Note3, 70, ' ', true);
                    gtf.SetTotalDetail("-", 25, '-', false, true);
                    gtf.SetTotalDetail((dt[i - 1].Remark != "") ? "Note :" + dt[i - 1].Remark : String.Empty, 70, ' ', true);
                    gtf.SetTotalDetail(dt[i - 1].TitleSign, 25, ' ', false, true);

                    gtf.PrintTotal(false, lastData, false);

                    ttlQty = ttlNilai = ttlJumlah = 0;

                    arrayHeader[0] = dt[0].CustomerName;
                    arrayHeader[1] = dt[0].Address1;
                    arrayHeader[2] = dt[0].Address2;
                    arrayHeader[3] = dt[0].Address3;
                    arrayHeader[4] = dt[0].FPJNo;
                    arrayHeader[5] = dt[0].PickingSlipNo;
                    arrayHeader[6] = dt[0].NPWPNo;

                    CreateHdrSpRpTrn011(gtf, arrayHeader, true);
                    gtf.PrintAfterBreak();

                    lastData = false;
                    noUrut = 1;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt[i].PartNo, 15, ' ', true);
                    gtf.SetDataDetail(dt[i].PartName, 15, ' ', true);
                    gtf.SetDataDetail(dt[i].QtyBill.Value.ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt[i].RetailPrice.Value.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt[i].DiscPct.Value.ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt[i].DiscAmt.Value.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt[i].NetRetailPrice.Value.ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt[i].NetSalesAmt.Value.ToString(), 12, ' ', false, true, true, true, "n0");
                    gtf.PrintData(fullPages);

                    docNo = dt[i].FPJNo;
                }
                else if (docNo == dt[i].FPJNo)
                {
                    noUrut++;
                    gtf.SetDataDetail(noUrut.ToString(), 3, '0', true, false, true);
                    gtf.SetDataDetail(dt[i].PartNo, 15, ' ', true);
                    gtf.SetDataDetail(dt[i].PartName, 15, ' ', true);
                    gtf.SetDataDetail(dt[i].QtyBill.Value.ToString(), 6, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt[i].RetailPrice.Value.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt[i].DiscPct.Value.ToString(), 5, ' ', true, false, true, true, "n2");
                    gtf.SetDataDetail(dt[i].DiscAmt.Value.ToString(), 11, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt[i].NetRetailPrice.Value.ToString(), 10, ' ', true, false, true, true, "n0");
                    gtf.SetDataDetail(dt[i].NetSalesAmt.Value.ToString(), 12, ' ', false, true, true, true, "n0");
                    if (i + 1 < dt.Count)
                    {
                        if (docNo == dt[i + 1].FPJNo)
                            gtf.PrintData(fullPages);
                        else
                            lastData = true;
                    }
                    else
                        lastData = true;
                }

                ttlQty += dt[i].QtyBill.Value;
                ttlNilai +=  dt[i].DiscAmt.Value;
                ttlJumlah += dt[i].NetSalesAmt.Value;
            }


            gtf.SetTotalDetailLine();

            gtf.SetTotalDetail("Total : ", 35, ' ', true, false);
            gtf.SetTotalDetail(ttlQty.ToString(), 6, ' ', true, false, true, true, "n2");
            gtf.SetTotalDetailSpace(18);
            gtf.SetTotalDetail(ttlNilai.ToString(), 11, ' ', true, false, true, true, "n0");
            gtf.SetTotalDetailSpace(11);
            gtf.SetTotalDetail(ttlJumlah.ToString(), 12, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailLine();

            terbilang = gtf.ConvertArrayTerbilang(gtf.Terbilang(Int64.Parse(dt[dt.Count - 1].TotFinalSalesAmt.Value.ToString())), 49);

            gtf.SetTotalDetail("Terbilang", 15, ' ', true);
            gtf.SetTotalDetail(terbilang[0].ToString().ToUpper(), 48, ' ', true);
            gtf.SetTotalDetail("D.P.P", 12, ' ', true);
            gtf.SetTotalDetail(":", 1, ' ');
            gtf.SetTotalDetail(dt[dt.Count - 1].TotDppAmt.Value.ToString(), 17, ' ', false, true, true, true, "n0");
            gtf.SetTotalDetailSpace(16);
            gtf.SetTotalDetail((terbilang[1] != null) ? terbilang[1].ToString().ToUpper() : string.Empty, 48, ' ', true);
            gtf.SetTotalDetail(dt[dt.Count - 1].TaxPct.Value.ToString() != "" ? "PPN " + dt[dt.Count - 1].TaxPct.Value.ToString() + "%" : string.Empty, 12, ' ', true);
            gtf.SetTotalDetail(":", 1, ' ');
            gtf.SetTotalDetail(dt[dt.Count - 1].TotPPNAmt.Value.ToString(), 17, ' ', false, true, true, true, "n0");
            if (terbilang[2] != null)
            {
                gtf.SetTotalDetailSpace(16);
                gtf.SetTotalDetail(terbilang[2].ToString().ToUpper(), 48, ' ', true);
                gtf.SetTotalDetail("JUMLAH", 12, ' ', true);
                gtf.SetTotalDetail(":", 1, ' ');
                gtf.SetTotalDetail(dt[dt.Count - 1].TotFinalSalesAmt.Value.ToString(), 17, ' ', false, true, true, true, "n0");

                gtf.SetTotalDetail(dt[dt.Count - 1].TOPC != "" ? "Dibayar Dengan : " + dt[dt.Count - 1].TOPC : string.Empty, 40, ' ', true);
                gtf.SetTotalDetail("No.Order :", 10, ' ', true);
                gtf.SetTotalDetail(dt[dt.Count - 1].OrderNo != "" ? dt[dt.Count - 1].OrderNo : "-", 18, ' ', true);

                gtf.SetTotalDetail((dt[dt.Count - 1].CITY != "") ? dt[dt.Count - 1].CITY.ToUpper() + ", " + ((dt[dt.Count - 1].FPJDate != null) ? dt[dt.Count - 1].FPJDate.Value.ToString("dd-MM-yyyy") : string.Empty) : string.Empty, 31, ' ', false, true);
                gtf.SetTotalDetail((dt[dt.Count - 1].DueDate.Value.ToString() != "") ? "Jatuh Tempo    : " + Convert.ToDateTime(dt[dt.Count - 1].DueDate).ToString("dd-MM-yyyy") : string.Empty, 65, ' ', false, true);
            }
            else
            {
                gtf.SetTotalDetail(dt[dt.Count - 1].TOPC != "" ? "Dibayar Dengan : " + dt[dt.Count - 1].TOPC : string.Empty, 39, ' ', true);
                gtf.SetTotalDetail("No.Order :", 10, ' ', true);
                gtf.SetTotalDetail(dt[dt.Count - 1].OrderNo != "" ? dt[dt.Count - 1].OrderNo : "-", 13, ' ', true);
                gtf.SetTotalDetail("JUMLAH", 12, ' ', true);
                gtf.SetTotalDetail(":", 1, ' ');
                gtf.SetTotalDetail(dt[dt.Count - 1].TotFinalSalesAmt.Value.ToString(), 17, ' ', false, true, true, true, "n0");

                gtf.SetTotalDetail((dt[dt.Count - 1].DueDate.Value.ToString() != "") ? "Jatuh Tempo    : " + Convert.ToDateTime(dt[dt.Count - 1].DueDate).ToString("dd-MM-yyyy") : string.Empty, 70, ' ', true);
                gtf.SetTotalDetail((dt[dt.Count - 1].CITY != "") ? dt[dt.Count - 1].CITY.ToUpper() + ", " + ((dt[dt.Count - 1].FPJDate != null) ? dt[dt.Count - 1].FPJDate.Value.ToString("dd-MM-yyyy") : string.Empty) : string.Empty, 31, ' ', false, true);
            }
            gtf.SetTotalDetail("KETERANGAN :", 12, ' ', true);
            gtf.SetTotalDetail(dt[dt.Count - 1].Note1??"", 58, ' ', false, true);
            gtf.SetTotalDetail(dt[dt.Count - 1].Note2??"", 70, ' ', true);
            gtf.SetTotalDetail(dt[dt.Count - 1].SignName??"", 25, ' ', false, true);
            gtf.SetTotalDetail(dt[dt.Count - 1].Note3??"", 70, ' ', true);
            gtf.SetTotalDetail("-", 25, '-', false, true);
            gtf.SetTotalDetail(((dt[dt.Count - 1].Remark??"") != "") ? "Note :" + dt[dt.Count - 1].Remark : String.Empty, 70, ' ', true);
            gtf.SetTotalDetail(dt[dt.Count - 1].TitleSign??"", 25, ' ', false, true);

            if (print == true)
                gtf.PrintTotal(true, lastData, false);
            else
            {
                if (gtf.PrintTotal(true, lastData, false) == true)
                    gtf.sbDataTxt.ToString();
                else
                    return "";
            }

            return gtf.sbDataTxt.ToString();
        }

        private void CreateHdrSpRpTrn011(SpGenerateTextFileReport gtf, string[] arrayHeader, bool hargaNet)
        {
            gtf.CleanHeader();

            gtf.SetGroupHeader("KEPADA YTH,", 96, ' ', false, true);
            gtf.SetGroupHeader(arrayHeader[0].ToString(), 60, ' ', false, true);
            gtf.SetGroupHeader(arrayHeader[1].ToString(), 60, ' ', true);
            gtf.SetGroupHeader(arrayHeader[4].ToString() != "" ? "NO. FAKTUR : " + arrayHeader[4].ToString() : "-", 35, ' ', false, true);
            gtf.SetGroupHeader(arrayHeader[2].ToString(), 60, ' ', true);
            gtf.SetGroupHeaderSpace(11);
            gtf.SetGroupHeader(arrayHeader[5].ToString() != "" ? " " + arrayHeader[5].ToString() : "", 12, ' ', false, true, true);
            if (arrayHeader[3] != null)
                gtf.SetGroupHeader(arrayHeader[3].ToString(), 60, ' ', false, true);
            gtf.SetGroupHeader(arrayHeader[6].ToString() != "" ? "NPWP : " + arrayHeader[6].ToString() : "", 96, ' ', false, true);

            if (hargaNet == true)
            {
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeaderSpace(55);
                gtf.SetGroupHeader("DISC", 17, ' ', true, false, false, true);
                gtf.SetGroupHeader("@", 10, ' ', false, true, false, true);
                gtf.SetGroupHeader("NO.", 3, ' ', true);
                gtf.SetGroupHeader("NO. PART", 15, ' ', true);
                gtf.SetGroupHeader("NAMA PART", 15, ' ', true);
                gtf.SetGroupHeader("QTY", 6, ' ', true, false, true);
                gtf.SetGroupHeader("HARGA", 11, ' ', true, false, true);
                gtf.SetGroupHeader("%", 5, ' ', true, false, true);
                gtf.SetGroupHeader("NILAI", 11, ' ', true, false, true);
                gtf.SetGroupHeader("HARGA NET", 10, ' ', true, false, true);
                gtf.SetGroupHeader("JUMLAH", 12, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
            }
            else
            {
                gtf.SetGroupHeaderLine();
                gtf.SetGroupHeaderSpace(65);
                gtf.SetGroupHeader("DISC", 17, ' ', false, true, false, true);
                gtf.SetGroupHeaderSpace(65);
                gtf.SetGroupHeader("-", 17, '-', false, true);
                gtf.SetGroupHeader("NO.", 3, ' ', true);
                gtf.SetGroupHeader("NO. PART", 20, ' ', true);
                gtf.SetGroupHeader("NAMA PART", 20, ' ', true);
                gtf.SetGroupHeader("QTY", 6, ' ', true, false, true);
                gtf.SetGroupHeader("HARGA", 11, ' ', true, false, true);
                gtf.SetGroupHeader("%", 5, ' ', true, false, true);
                gtf.SetGroupHeader("NILAI", 11, ' ', true, false, true);
                gtf.SetGroupHeader("JUMLAH", 13, ' ', false, true, true);
                gtf.SetGroupHeaderLine();
            }
        }

        
    }
}