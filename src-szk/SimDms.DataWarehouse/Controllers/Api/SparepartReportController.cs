using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using EP = SimDms.DataWarehouse.Helpers.EPPlusHelper;
using SimDms.DataWarehouse.Helpers;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class SparepartReportController : BaseController
    {
        public JsonResult Years()
        {
            var init = DateTime.Now.Year - 10;
            var years = new List<object>();
            for (int i = 0; i < 11; i++) years.Add(new { value = init + i, text = (init + i).ToString() });
            return Json(years);
        }

        public JsonResult Months()
        {
            var months = new List<object>();
            for (int i = 1; i < 13; i++)
            {
                var date = new DateTime(1900, i, 1);
                var monthString = date.ToString("MMMM");
                months.Add(new { value = i, text = monthString });
            }
            return Json(months);
        }

        public JsonResult Areas()
        {
            var areas = ctx.GnMstDealerMappings.Select(x => new
            {
                value = x.GroupNo,
                text = x.Area
            }).Distinct();
            return Json(areas);
        }

        public JsonResult Dealers(string area)
        {
            if (area != "")
            {
                var groupCode = int.Parse(area);
                var dealers = ctx.GnMstDealerMappings.Where(x => x.GroupNo == groupCode && x.isActive)
                    .Select(x => new
                {
                    value = x.DealerCode,
                    text = "(" + x.DealerAbbreviation + ") - " + x.DealerName
                });
                return Json(dealers);
            }
            return Json(new { });
        }

        public JsonResult Outlets(string area, string dealer)
        {
            if (area != "" && area != null && dealer != "" && dealer != null)
            {
                var groupCode = int.Parse(area);
                var outlets = from a in ctx.GnMstDealerMappings
                              join b in ctx.GnMstDealerOutletMappings
                              on new { a.DealerCode } equals new { b.DealerCode }
                              where a.GroupNo == groupCode
                              && a.DealerCode == dealer
                              && a.isActive
                              select new
                              {
                                  value = b.OutletCode,
                                  text = b.OutletAbbreviation
                              };
                return Json(outlets);
            }
            return Json(new { });
        }

        public JsonResult ExportToExcel(RptModel model)
        {
            string area = Request.Params["Area"];

            try
            {
                ctx.Database.CommandTimeout = 3600;
                //var query = "exec uspfn_spMonthlyReportSGP @p0, @p1, @p2, @p3, @p4, @p5";
                //var result = ctx.Database.SqlQuery<RptResultModel>(query, model.DealerCode ?? "", 
                //    model.OutletCode ?? "", model.StartMonth, model.EndMonth, 
                //    model.StartYear, model.EndYear).ToList();

                var query = "exec uspfn_spMonthlyReportSGP_New @p0, @p1, @p2, @p3, @p4, @p5, @p6";
                var result = ctx.Database.SqlQuery<RptResultModel>(query,area, model.DealerCode ?? "",
                    model.OutletCode ?? "", model.StartMonth, model.EndMonth,
                    model.StartYear, model.EndYear).ToList();

                var package = new ExcelPackage();
                package = GenerateExcel(package, model, result);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = "", value = guid });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public FileContentResult DownloadExcelFile(string key)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=MonthlyReportSGP.xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        private static ExcelPackage GenerateExcel(ExcelPackage package, RptModel model, List<RptResultModel> data)
        {
            var ctx = new DataContext();
            string dealerName = "ALL", outletName = "ALL";
            string[] code = model.DealerCode.Split('|');
            int groupNo = Convert.ToInt32(code[0].ToString());
            string companycode = code[1].ToString();

            if (model.DealerCode != "" && model.DealerCode != null)
            {                
                dealerName = (from a in ctx.GnMstDealerMappingNews
                              //where a.DealerCode == model.DealerCode
                              where a.GroupNo == groupNo && a.DealerCode == companycode
                              select a.DealerAbbreviation).FirstOrDefault();

                if (dealerName == null) throw new Exception("Kode dealer salah / tidak ditemukan");
            }

            if (model.OutletCode != "" && model.OutletCode != null)
            {
                outletName = (from a in ctx.GnMstDealerOutletMappings
                              where a.OutletCode == model.OutletCode
                              select a.OutletAbbreviation).FirstOrDefault();

                if (outletName == null) throw new Exception("Kode outlet salah / tidak ditemukan");
            }

            #region -- Constants --
            string z = "", x = "";
            const int
                rTitle = 1,
                rDealerName = 2,
                rOutletName = 3,
                rSystem = 4,
                rHeader1 = 5,
                rHeader2 = 6,
                rHeader3 = 7,
                rDataStart = 8,

                cTahun              = 1,
                cBulan              = 2,
                cJmlJaringan        = 3,
                cPKWorkshop         = 4,
                cPKCounter          = 5,
                cPKPartshop         = 6,
                cPKSubdealer        = 7,
                cPK                 = 8,
                cPBWorkshop         = 9,
                cPBCounter          = 10,
                cPBPartshop         = 11,
                cPBSubdealer        = 12,
                cPB                 = 13,
                cHPPWorkshop        = 14,
                cHPPCounter         = 15,
                cHPPPartshop        = 16,
                cHPPSubdealer       = 17,
                cHPPTotal           = 18,
                cPenPembelian       = 19,
                cNilaiStock         = 20,
                cITO                = 21,
                cDemandLine         = 22,
                cDemandQty          = 23,
                cDemandNilai        = 24,
                cSupplyLine         = 25,
                cSupplyQty          = 26,
                cSupplyNilai        = 27,
                cSRLine             = 28,
                cSRQty              = 29,
                cSRNilai            = 30,
                cAmtMC0             = 31,
                cQtyMC0             = 32,
                cAmtMC1             = 33,
                cQtyMC1             = 34,
                cAmtMC2             = 35,
                cQtyMC2             = 36,
                cAmtMC3             = 37,
                cQtyMC3             = 38,
                cAmtMC4             = 39,
                cQtyMC4             = 40,
                cAmtMC5             = 41,
                cQtyMC5             = 42,
                cSlowMov            = 43,
                cLTOReg             = 44,
                cLTOEme             = 45;

            double
                wTahun = EP.GetTrueColWidth(14.71),
                wJmlJaringan = EP.GetTrueColWidth(11.00),
                wValue = EP.GetTrueColWidth(16.57);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("MonthlyRptSGP");

            #region -- Title --
            x = EP.GetCell(rTitle, cTahun);
            sheet.Cells[x].Value = "Kode Dealer";

            x = EP.GetCell(rTitle, cBulan);
            //sheet.Cells[x].Value = model.DealerCode; 
            sheet.Cells[x].Value = companycode;

            z = EP.GetRange(rTitle, cJmlJaringan, rTitle, cPKSubdealer);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Value = "SUZUKI GENUINE PARTS (SGP)";
            sheet.Cells[z].Style.Font.Size = 14f;
            sheet.Cells[z].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            x = EP.GetCell(rDealerName, cTahun);
            sheet.Cells[x].Value = "Nama Dealer";

            z = EP.GetRange(rDealerName, cBulan, rDealerName, cPKSubdealer);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Value = dealerName;

            x = EP.GetCell(rOutletName, cTahun);
            sheet.Cells[x].Value = "Nama Outlet";

            z = EP.GetRange(rOutletName, cBulan, rOutletName, cPKSubdealer);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Value = outletName;

            x = EP.GetCell(rSystem, cTahun);
            sheet.Cells[x].Value = "System";

            x = EP.GetCell(rSystem, cBulan);
            sheet.Cells[x].Value = "";

            z = EP.GetRange(rTitle, cTahun, rHeader3, cLTOEme);
            sheet.Cells[z].Style.Font.Bold = true;
            #endregion

            

            #region -- Header --
            z = EP.GetRange(rHeader1, cTahun, rHeader3, cLTOEme);
            sheet.Cells[z].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            sheet.Cells[z].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            z = EP.GetRange(rHeader1, cTahun, rHeader3, cTahun);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            sheet.Cells[z].Value = "Tahun";

            z = EP.GetRange(rHeader1, cBulan, rHeader3, cBulan);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            sheet.Cells[z].Value = "Bulan";

            z = EP.GetRange(rHeader1, cJmlJaringan, rHeader3, cJmlJaringan);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.WrapText = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            sheet.Cells[z].Value = "Jumlah Jaringan";

            z = EP.GetRange(rHeader1, cPKWorkshop, rHeader2, cPK);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Penjualan Kotor";

            x = EP.GetCell(rHeader3, cPKWorkshop);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "WORKSHOP";

            x = EP.GetCell(rHeader3, cPKCounter);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "COUNTER";

            x = EP.GetCell(rHeader3, cPKPartshop);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "PARTSHOP";

            x = EP.GetCell(rHeader3, cPKSubdealer);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "SUB DEALER";

            x = EP.GetCell(rHeader3, cPK);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "TOTAL";

            z = EP.GetRange(rHeader1, cPKWorkshop, rHeader3, cPK);
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);

            z = EP.GetRange(rHeader1, cPBWorkshop, rHeader2, cPB);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Penjualan Bersih";

            x = EP.GetCell(rHeader3, cPBWorkshop);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "WORKSHOP";

            x = EP.GetCell(rHeader3, cPBCounter);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "COUNTER";

            x = EP.GetCell(rHeader3, cPBPartshop);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "PARTSHOP";

            x = EP.GetCell(rHeader3, cPBSubdealer);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "SUB DEALER";

            x = EP.GetCell(rHeader3, cPB);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "TOTAL";

            z = EP.GetRange(rHeader1, cPBWorkshop, rHeader3, cPB);
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            
            z = EP.GetRange(rHeader1, cHPPWorkshop, rHeader2, cHPPTotal);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Harga Pokok (HPP)";

            x = EP.GetCell(rHeader3, cHPPWorkshop);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "WORKSHOP";

            x = EP.GetCell(rHeader3, cHPPCounter);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "COUNTER";

            x = EP.GetCell(rHeader3, cHPPPartshop);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "PARTSHOP";

            x = EP.GetCell(rHeader3, cHPPSubdealer);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "SUB DEALER";

            x = EP.GetCell(rHeader3, cHPPTotal);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "TOTAL HPP";

            z = EP.GetRange(rHeader1, cHPPWorkshop, rHeader3, cHPPTotal);
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);

            z = EP.GetRange(rHeader1, cPenPembelian, rHeader3, cPenPembelian);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.WrapText = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            sheet.Cells[z].Value = "Penerimaan Pembelian";

            z = EP.GetRange(rHeader1, cNilaiStock, rHeader3, cNilaiStock);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.WrapText = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            sheet.Cells[z].Value = "Nilai Stock";

            z = EP.GetRange(rHeader1, cITO, rHeader3, cITO);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.WrapText = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            sheet.Cells[z].Value = "ITO";

            z = EP.GetRange(rHeader1, cDemandLine, rHeader2, cDemandNilai);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Demand";

            x = EP.GetCell(rHeader3, cDemandLine);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Line";

            x = EP.GetCell(rHeader3, cDemandQty);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Quantity";

            x = EP.GetCell(rHeader3, cDemandNilai);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Nilai";

            z = EP.GetRange(rHeader1, cDemandLine, rHeader3, cDemandNilai);
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);

            z = EP.GetRange(rHeader1, cSupplyLine, rHeader2, cSupplyNilai);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Supply";

            x = EP.GetCell(rHeader3, cSupplyLine);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Line";

            x = EP.GetCell(rHeader3, cSupplyQty);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Quantity";

            x = EP.GetCell(rHeader3, cSupplyNilai);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Nilai";

            z = EP.GetRange(rHeader1, cSupplyLine, rHeader3, cSupplyNilai);
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);

            z = EP.GetRange(rHeader1, cSRLine, rHeader2, cSRNilai);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Service Ratio";

            x = EP.GetCell(rHeader3, cSRLine);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Line";

            x = EP.GetCell(rHeader3, cSRQty);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Quantity";

            x = EP.GetCell(rHeader3, cSRNilai);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Nilai";

            z = EP.GetRange(rHeader1, cSRLine, rHeader3, cSRNilai);
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);

            z = EP.GetRange(rHeader1, cAmtMC0, rHeader1, cQtyMC5);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Data Stock";

            z = EP.GetRange(rHeader2, cAmtMC0, rHeader2, cQtyMC0);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Moving Code 0";

            x = EP.GetCell(rHeader3, cAmtMC0);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Amount";

            x = EP.GetCell(rHeader3, cQtyMC0);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Qty";

            z = EP.GetRange(rHeader2, cAmtMC1, rHeader2, cQtyMC1);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Moving Code 1";

            x = EP.GetCell(rHeader3, cAmtMC1);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Amount";

            x = EP.GetCell(rHeader3, cQtyMC1);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Qty";

            z = EP.GetRange(rHeader2, cAmtMC2, rHeader2, cQtyMC2);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Moving Code 2";

            x = EP.GetCell(rHeader3, cAmtMC2);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Amount";

            x = EP.GetCell(rHeader3, cQtyMC2);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Qty";

            z = EP.GetRange(rHeader2, cAmtMC3, rHeader2, cQtyMC3);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Moving Code 3";

            x = EP.GetCell(rHeader3, cAmtMC3);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Amount";

            x = EP.GetCell(rHeader3, cQtyMC3);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Qty";

            z = EP.GetRange(rHeader2, cAmtMC4, rHeader2, cQtyMC4);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Moving Code 4";

            x = EP.GetCell(rHeader3, cAmtMC4);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Amount";

            x = EP.GetCell(rHeader3, cQtyMC4);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Qty";

            z = EP.GetRange(rHeader2, cAmtMC5, rHeader2, cQtyMC5);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Value = "Moving Code 5";

            x = EP.GetCell(rHeader3, cAmtMC5);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Amount";

            x = EP.GetCell(rHeader3, cQtyMC5);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Qty";

            z = EP.GetRange(rHeader1, cAmtMC0, rHeader3, cQtyMC5);
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);

            z = EP.GetRange(rHeader1, cSlowMov, rHeader3, cSlowMov);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            sheet.Cells[z].Style.WrapText = true;
            sheet.Cells[z].Value = "Slow Moving";

            z = EP.GetRange(rHeader1, cLTOReg, rHeader2, cLTOEme);
            sheet.Cells[z].Merge = true;
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[z].Style.WrapText = true;
            sheet.Cells[z].Value = "Lead Time Order (hari)";

            x = EP.GetCell(rHeader3, cLTOReg);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Regular";

            x = EP.GetCell(rHeader3, cLTOEme);
            sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
            sheet.Cells[x].Value = "Emergency";

            z = EP.GetRange(rHeader1, cLTOReg, rHeader3, cLTOEme);
            sheet.Cells[z].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            #endregion

            #region -- Data --
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var fCustom = "_(* #,##0_);_(* (#,##0);_(* \" - \"??_);_(@_)";
                var fPercent = "#0\\.00%";
                var fDecimal = "0.00";
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cTahun, Value = data[i].CurrentYear.Value, Format = "" },
                    new ExcelCellItem { Column = cBulan, Value = EP.GetMonthName(data[i].CurrentMonth.Value), Format = "" },
                    new ExcelCellItem { Column = cJmlJaringan, Value = data[i].JumlahJaringan },
                    new ExcelCellItem { Column = cPKWorkshop, Value = data[i].PenjualanKotorWorkshop },
                    new ExcelCellItem { Column = cPKCounter, Value = data[i].PenjualanKotorCounter },
                    new ExcelCellItem { Column = cPKPartshop, Value = data[i].PenjualanKotorPartshop },
                    new ExcelCellItem { Column = cPKSubdealer, Value = data[i].PenjualanKotorSubDealer },
                    new ExcelCellItem { Column = cPK, Value = data[i].PenjualanKotor },
                    new ExcelCellItem { Column = cPBWorkshop, Value = data[i].PenjualanBersihWorkshop },
                    new ExcelCellItem { Column = cPBCounter, Value = data[i].PenjualanBersihCounter },
                    new ExcelCellItem { Column = cPBPartshop, Value = data[i].PenjualanBersihPartshop },
                    new ExcelCellItem { Column = cPBSubdealer, Value = data[i].PenjualanBersihSubDealer },
                    new ExcelCellItem { Column = cPB, Value = data[i].PenjualanBersih },
                    new ExcelCellItem { Column = cHPPWorkshop, Value = data[i].HargaPokokWorkshop },
                    new ExcelCellItem { Column = cHPPCounter, Value = data[i].HargaPokokCounter },
                    new ExcelCellItem { Column = cHPPPartshop, Value = data[i].HargaPokokPartshop },
                    new ExcelCellItem { Column = cHPPSubdealer, Value = data[i].HargaPokokSubDealer },
                    new ExcelCellItem { Column = cHPPTotal, Value = data[i].TotalHargaPokok },
                    new ExcelCellItem { Column = cPenPembelian, Value = data[i].PenerimaanPembelian },
                    new ExcelCellItem { Column = cNilaiStock, Value = data[i].NilaiStock },
                    new ExcelCellItem { Column = cITO, Value = data[i].ITO, Format = fDecimal },
                    new ExcelCellItem { Column = cDemandLine, Value = data[i].DemandLine },
                    new ExcelCellItem { Column = cDemandQty, Value = data[i].DemandQuantity },
                    new ExcelCellItem { Column = cDemandNilai, Value = data[i].DemandNilai },
                    new ExcelCellItem { Column = cSupplyLine, Value = data[i].SupplyLine },
                    new ExcelCellItem { Column = cSupplyQty, Value = data[i].SupplyQuantity },
                    new ExcelCellItem { Column = cSupplyNilai, Value = data[i].SupplyNilai },
                    new ExcelCellItem { Column = cSRLine, Value = data[i].ServiceRatioLine, Format = fPercent },
                    new ExcelCellItem { Column = cSRQty, Value = data[i].ServiceRatioQuantity, Format = fPercent },
                    new ExcelCellItem { Column = cSRNilai, Value = data[i].ServiceRatioNilai, Format = fPercent },
                    new ExcelCellItem { Column = cAmtMC0, Value = data[i].AmtMC0 },
                    new ExcelCellItem { Column = cQtyMC0, Value = data[i].QtyMC0 },
                    new ExcelCellItem { Column = cAmtMC1, Value = data[i].AmtMC1 },
                    new ExcelCellItem { Column = cQtyMC1, Value = data[i].QtyMC1 },
                    new ExcelCellItem { Column = cAmtMC2, Value = data[i].AmtMC2 },
                    new ExcelCellItem { Column = cQtyMC2, Value = data[i].QtyMC2 },
                    new ExcelCellItem { Column = cAmtMC3, Value = data[i].AmtMC3 },
                    new ExcelCellItem { Column = cQtyMC3, Value = data[i].QtyMC3 },
                    new ExcelCellItem { Column = cAmtMC4, Value = data[i].AmtMC4 },
                    new ExcelCellItem { Column = cQtyMC4, Value = data[i].QtyMC4 },
                    new ExcelCellItem { Column = cAmtMC5, Value = data[i].AmtMC5 },
                    new ExcelCellItem { Column = cQtyMC5, Value = data[i].QtyMC5 },
                    new ExcelCellItem { Column = cSlowMov, Value = data[i].SlowMoving, Format = fPercent },
                    new ExcelCellItem { Column = cLTOReg, Value = data[i].LTORegular },
                    new ExcelCellItem { Column = cLTOEme, Value = data[i].LTOEmergency }
                };

                foreach (var item in items)
                {
                    x = EP.GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    sheet.Cells[x].Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                }
            }

            #endregion

            #region -- Column Width --
            sheet.Column(cTahun).Width = wTahun;
            sheet.Column(cBulan).Width = wTahun;
            sheet.Column(cJmlJaringan).Width = wJmlJaringan;
            for (int i = cPKWorkshop; i <= cLTOEme; i++) sheet.Column(i).Width = wValue;
            #endregion

            return package;
        }

        public JsonResult loadDataWeekly(SparepartWeeklyGrid model)
        {
            try
            {
                var Year = (int)DateTime.Now.DayOfYear;
                var Week = (int)DateTime.Now.DayOfWeek;
                var _week = model.PeriodYear == Year ? Week : 0;

                var gCode = "";
                var cCode = model.CompanyCode ?? "";
                var bCode = model.BranchCode ?? "";

                model.CompanyCode = model.CompanyCode ?? "";
                model.BranchCode = model.BranchCode ?? "";

                if (cCode != "" && cCode.IndexOf('|') > 0)
                {
                    string[] param = cCode.Split('|');
                   gCode = param[0];
                   cCode = param[1];
                }

                var query = "exec sp_GetSparepartAnalysisWeekly @p0,@p1,@p2,@p3,@p4";
                object[] parameters = { cCode, bCode, model.PeriodYear, model.PeriodMonth, model.TypeOfGoods };
                var data = ctx.Database.SqlQuery<SparepartWeeklyGrid>(query, parameters);

                if (data.Count() == 0)
                {
                    var queryInsert = "exec sp_InsertSparepartAnalysisWeekly @p0,@p1,@p2";
                    object[] paramInsert = { model.PeriodYear, model.PeriodMonth, _week };
                    var dataInsert = ctx.Database.ExecuteSqlCommand(queryInsert, paramInsert);
                }

                return Json(new { success = true, data = data });
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        public ActionResult GenerateExcelWeekly(string CompanyCode, string BranchCode, decimal PeriodYear, decimal PeriodMonth, string TypeOfGoods)
        {
            var dealerCode = CompanyCode ?? "";
            var outletCode = BranchCode ?? "";
            var groupNo = "";
            
            if (dealerCode.IndexOf('|') > 0)
            {
                string[] param = dealerCode.Split('|');
                groupNo = param[0];
                dealerCode = param[1];
            }
            
            
            DateTime now = DateTime.Now;
            string fileName = "Sparepart_Analysis_Weekly_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "sp_GetSparepartAnalysisWeekly";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@CompanyCode", dealerCode);
            cmd.Parameters.AddWithValue("@BranchCode", outletCode);
            cmd.Parameters.AddWithValue("@PeriodYear", PeriodYear);
            cmd.Parameters.AddWithValue("@PeriodMonth", PeriodMonth);
            cmd.Parameters.AddWithValue("@TypeOfGoods", TypeOfGoods);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt = ds.Tables[0];

            int recNo = 6;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("AnalysisWeekly");

            var _dealer = "Dealer : ";
            _dealer += outletCode == "" ? "ALL" : ctx.CoProfiles.Find(outletCode, outletCode).CompanyName;
            var _type = "Type Of Goods : ";
            _type += TypeOfGoods == "" ? "ALL" : ctx.GnMstLookUpDtls.Find(dealerCode, "TPGO", TypeOfGoods).LookUpValueName;

            var _company = "Company :";
            _company += dealerCode == "" ? "ALL" : ctx.GnMstDealerMappings.FirstOrDefault(a => a.DealerCode == dealerCode).DealerName;

            ws.Cell("A1").Value = _company;
            ws.Cell("A2").Value = _dealer;
            ws.Cell("A3").Value = _type;

            ws.Cell("A4").Value = "Minggu";
            ws.Range("A4:A5").Merge();
            ws.Range("A4:A5").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("A4:A5").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("A4:A5").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("A4:A5").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Range("B4:C4").Merge();
            ws.Range("B4:C4").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("B4:C4").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("B4:C4").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("B4:C4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("B4").Value = "Sales Out Bengkel ";

            ws.Range("D4:E4").Merge();
            ws.Range("D4:E4").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("D4:E4").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("D4:E4").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("D4:E4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("D4").Value = "Sales Out Counter ";

            ws.Range("F4:G4").Merge();
            ws.Range("F4:G4").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("F4:G4").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("F4:G4").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("F4:G4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            ws.Cell("F4").Value = "Sales Out PartShop ";

            ws.Cell("H4").Value = "Stock";
            ws.Range("H4:H5").Merge();
            ws.Range("H4:H5").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Range("H4:H5").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Range("H4:H5").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Range("H4:H5").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Cell("B5").Value = "Netto";
            ws.Cell("B5").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("B5").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("B5").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("B5").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Cell("C5").Value = "HPP";
            ws.Cell("C5").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("C5").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("C5").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("C5").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Cell("D5").Value = "Netto";
            ws.Cell("D5").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("D5").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("D5").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("D5").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Cell("E5").Value = "HPP";
            ws.Cell("E5").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("E5").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("E5").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("E5").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Cell("F5").Value = "Netto";
            ws.Cell("F5").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("F5").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("F5").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("F5").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            ws.Cell("G5").Value = "HPP";
            ws.Cell("G5").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            ws.Cell("G5").Style.Border.RightBorder = XLBorderStyleValues.Thin;
            ws.Cell("G5").Style.Border.TopBorder = XLBorderStyleValues.Thin;
            ws.Cell("G5").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            foreach (DataRow row in dt.Rows)
            {
                ws.Cell("A" + recNo).Value = "Ke - " + row["PeriodWeek"].ToString();
                ws.Cell("A" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("A" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("B" + recNo).Value = row["Netto_WS"].ToString();
                ws.Cell("B" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("C" + recNo).Value = row["HPP_WS"].ToString();
                ws.Cell("C" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("D" + recNo).Value = row["Netto_C"].ToString();
                ws.Cell("D" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("E" + recNo).Value = row["HPP_C"].ToString();
                ws.Cell("E" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("F" + recNo).Value = row["Netto_PS"].ToString();
                ws.Cell("F" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("G" + recNo).Value = row["HPP_PS"].ToString();
                ws.Cell("G" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + recNo).Style.NumberFormat.Format = "#,##0.00";

                ws.Cell("H" + recNo).Value = row["NilaiStock"].ToString();
                ws.Cell("H" + recNo).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + recNo).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + recNo).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + recNo).Style.NumberFormat.Format = "#,##0.00";

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public class RptModel
        {
            public string DealerCode { get; set; }
            public string OutletCode { get; set; }
            public string StartMonth { get; set; }
            public string StartYear { get; set; }
            public string EndMonth { get; set; }
            public string EndYear { get; set; }
        }

        public class RptResultModel
        {
            public String CompanyCode { get; set; }
            public Int32? CurrentYear { get; set; }
            public Int32? CurrentMonth { get; set; }
            public Decimal? JumlahJaringan { get; set; }
            public Decimal? PenjualanKotorWorkshop { get; set; }
            public Decimal? PenjualanKotorCounter { get; set; }
            public Decimal? PenjualanKotorPartshop { get; set; }
            public Decimal? PenjualanKotorSubDealer { get; set; }
            public Decimal? PenjualanKotor { get; set; }
            public Decimal? PenjualanBersihWorkshop { get; set; }
            public Decimal? PenjualanBersihCounter { get; set; }
            public Decimal? PenjualanBersihPartshop { get; set; }
            public Decimal? PenjualanBersihSubDealer { get; set; }
            public Decimal? PenjualanBersih { get; set; }
            public Decimal? HargaPokokWorkshop { get; set; }
            public Decimal? HargaPokokCounter { get; set; }
            public Decimal? HargaPokokPartshop { get; set; }
            public Decimal? HargaPokokSubDealer { get; set; }
            public Decimal? TotalHargaPokok { get; set; }
            public Decimal? PenerimaanPembelian { get; set; }
            public Decimal? NilaiStock { get; set; }
            public Decimal? ITO { get; set; }
            public Decimal? DemandLine { get; set; }
            public Decimal? DemandQuantity { get; set; }
            public Decimal? DemandNilai { get; set; }
            public Decimal? SupplyLine { get; set; }
            public Decimal? SupplyQuantity { get; set; }
            public Decimal? SupplyNilai { get; set; }
            public Decimal? ServiceRatioLine { get; set; }
            public Decimal? ServiceRatioQuantity { get; set; }
            public Decimal? ServiceRatioNilai { get; set; }
            public Decimal? AmtMC0 { get; set; }
            public Decimal? QtyMC0 { get; set; }
            public Decimal? AmtMC1 { get; set; }
            public Decimal? QtyMC1 { get; set; }
            public Decimal? AmtMC2 { get; set; }
            public Decimal? QtyMC2 { get; set; }
            public Decimal? AmtMC3 { get; set; }
            public Decimal? QtyMC3 { get; set; }
            public Decimal? AmtMC4 { get; set; }
            public Decimal? QtyMC4 { get; set; }
            public Decimal? AmtMC5 { get; set; }
            public Decimal? QtyMC5 { get; set; }
            public Decimal? SlowMoving { get; set; }
            public String LTORegular { get; set; }
            public String LTOEmergency { get; set; }
        }

        public class SparepartWeeklyGrid
        {
            public string CompanyCode { get; set; }
            public string BranchCode { get; set; }
            public decimal? PeriodYear { get; set; }
            public decimal? PeriodMonth { get; set; }
            public decimal? PeriodWeek { get; set; }
            public string TypeOfGoods { get; set; }
            public decimal? Netto_WS { get; set; }
            public decimal? HPP_WS { get; set; }
            public decimal? Netto_PS { get; set; }
            public decimal? HPP_PS { get; set; }
            public decimal? Netto_C { get; set; }
            public decimal? HPP_C { get; set; }
            public decimal? NilaiStock { get; set; }
        }
    }
}