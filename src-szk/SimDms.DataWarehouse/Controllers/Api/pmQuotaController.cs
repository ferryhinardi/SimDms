using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using GeLang;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SimDms.DataWarehouse.Models;
using System.Data.SqlClient;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Transactions;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class pmQuotaController : BaseController
    {
        public JsonResult Default()
        {
            var quota = ctx.pmQuotas.Where(a => a.CompanyCode == CompanyCode).FirstOrDefault();
            if (quota != null)
            {
                return Json(new
                {
                    success = true,
                    QuotaBy = quota.QuotaBy,
                    PeriodYear = quota.PeriodYear
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    PeriodYear = DateTime.Now.Year
                });
            }
        }

        public JsonResult QuotaBy()
        {
            List<object> listOfQuotaBy = new List<object>();

            listOfQuotaBy.Add(new { value = "TYP", text = "TYP" });
            listOfQuotaBy.Add(new { value = "VAR", text = "VAR" });
            listOfQuotaBy.Add(new { value = "COL", text = "COL" });

            return Json(listOfQuotaBy);
        }

        public JsonResult Dealer()
        {
            var qry = from p in ctx.GnMstDealerMappings
                      where p.isActive == true
                      select new
                      {
                          p.DealerCode,
                          p.DealerAbbreviation,
                          p.DealerName
                      };

            return Json(qry.Distinct().KGrid());
        }

        public JsonResult Tipe()
        {
            var qry = from p in ctx.OmMstModels
                      join q in ctx.GnMstLookUpDtls
                      on p.GroupCode equals q.LookUpValue
                      where q.CompanyCode == "0000000"
                      && q.CodeID == "INDENT"
                      select new
                      {
                          p.GroupCode
                      };

            return Json(qry.Distinct().KGrid());
        }

        public JsonResult Varian()
        {
            string Tipe = Request["Tipe"] ?? "";
            var qry = from p in ctx.OmMstModels
                      where p.GroupCode == Tipe
                      select new
                      {
                          p.GroupCode,
                          p.TypeCode,
                          p.TransmissionType
                      };

            return Json(qry.Distinct().KGrid());
        }

        public JsonResult Tranmisi()
        {
            string Tipe = Request["Tipe"] ?? "";
            string Varian = Request["Varian"] ?? "";
            string query = Request["query"] ?? "";
            string sql = string.Format(@"SELECT b.TransmissionType FROM pmGroupTypeSeq a
                inner JOIN MstModel b
                on a.GroupCode = b.GroupCode
                and a.TypeCode = b.TypeCode
                where a.GroupCode = '" + Tipe + "' and a.TypeCode = '" + Varian + "' and b.TransmissionType like '" + query + "%' order by TransmissionType asc");
            var data = ctx.Database.SqlQuery<string>(sql);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ColorCode()
        {
            string Tipe = Request["Tipe"] ?? "";
            string Varian = Request["Varian"] ?? "";
            string Tranmisi = Request["Tranmisi"] ?? "";
            var qry = from p in ctx.OmMstModels
                      join q in ctx.OmMstModelColours
                      on p.SalesModelCode equals q.SalesModelCode
                      join r in ctx.OmMstRefferences
                      on new { RefferenceType = "COLO", q.ColourCode } equals new { r.RefferenceType, ColourCode = r.RefferenceCode }
                      where p.GroupCode == Tipe
                      && p.TypeCode == Varian
                      && p.TransmissionType == Tranmisi
                      select new
                      {
                          q.ColourCode,
                          ColourName = r.RefferenceDesc1
                      };

            return Json(qry.Distinct().KGrid());
        }

        private class Combo
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        public JsonResult ReloadQuota()
        {
            var Company = Request["CompanyCode"];
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonth = Request["PeriodMonth"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal Month = Convert.ToInt32(PeriodMonth != "" ? PeriodMonth : "0");
            try
            {
                string qry = String.Format(@"SELECT CompanyCode, PeriodYear, PeriodMonth, TipeKendaraan, Variant, Transmisi
                    ,ColourCode,QuotaQty, IndentQty, QuotaBy, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
                    FROM pmQuota where ((case when @p2='' then CompanyCode end)<>'' or (case when @p2<>'' then CompanyCode end)=@p2)
                    and ((case when @p0=0 then PeriodYear end)<>0 or (case when @p0<>0 then PeriodYear end)=@p0)
                    and ((case when @p1=0 then PeriodMonth end)<>0 or (case when @p1<>0 then PeriodMonth end)=@p1)");

//                string qry = String.Format(@"SELECT CompanyCode, PeriodYear, PeriodMonth, TipeKendaraan, Variant, Transmisi
//                    ,case when ColourCode <> '-' THEN ColourCode +'-'+(SELECT RefferenceDesc1 from omMstRefference where RefferenceCode = ColourCode) ELSE '-' end as ColourCode
//                    ,QuotaQty, IndentQty, QuotaBy, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate
//                    FROM pmQuota where  
//                    CompanyCode = @p2 and
//                    ((case when @p0=0 then PeriodYear end)<>0 or (case when @p0<>0 then PeriodYear end)=@p0)
//                    and ((case when @p1=0 then PeriodMonth end)<>0 or (case when @p1<>0 then PeriodMonth end)=@p1)");
//                    //PeriodYear = @p0 and PeriodMonth = @p1");

                var data = ctx.Database.SqlQuery<pmQuota>(qry, Year, Month, Company);

                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.InnerException });
            }
        }

        public JsonResult Save()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonth = Request["PeriodMonth"];
            var QuotaBy = Request["QuotaBy"];
            var data = Request["listScore"];
            var Year = DateTime.Now.Year;
            var Month = DateTime.Now.Month;
            int n = 0;

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<pmQuota> listscore = ser.Deserialize<List<pmQuota>>(data);
            List<pmQuota> listscoreerror = new List<pmQuota>();

            using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                foreach (var item in listscore)
                {
                    if(item.PeriodYear < Year)
                    {
                        tranScope.Dispose();
                        return Json(new { success = false, message = "Tidak bisa menyimpan data jika tahun periode quota lebih kecil dari tahun sekarang, Silakan periksa dan coba kembali !" });
                    }
                    else if (item.PeriodMonth < Month)
                    {
                        tranScope.Dispose();
                        return Json(new { success = false, message = "Tidak bisa menyimpan data jika bulan periode quota lebih kecil dari bulan sekarang, Silakan periksa dan coba kembali !" });
                    }

                    if (item.ColourCode.Length > 3) {
                        var ColourCode = ctx.OmMstRefferences.Where(a => a.RefferenceType == "COLO" && a.RefferenceDesc1 == item.ColourCode).FirstOrDefault();
                        if (ColourCode == null) {
                            tranScope.Dispose();
                            return Json(new { success = false, message = "ColourName " + item.ColourCode + " tidak ada di master Refferences" });
                        }
                    }
                    if (QuotaBy == "TYP") { item.Variant = "-"; item.Transmisi = "-"; item.ColourCode = "-"; }
                    if (QuotaBy == "VAR") { item.ColourCode = "-"; }
                    var ent = ctx.pmQuotas.Find(item.CompanyCode, item.PeriodYear, item.PeriodMonth, item.TipeKendaraan, item.Variant, item.Transmisi, item.ColourCode);
                    if (ent == null)
                    {
                        ent = new pmQuota
                        {
                            CompanyCode = item.CompanyCode,
                            PeriodYear = item.PeriodYear,
                            PeriodMonth = item.PeriodMonth,
                            TipeKendaraan = item.TipeKendaraan,
                            Variant = item.Variant,
                            Transmisi = item.Transmisi,
                            ColourCode = item.ColourCode,
                            QuotaQty = item.QuotaQty,
                            IndentQty = item.IndentQty,
                            QuotaBy = QuotaBy,
                            CreatedBy = CurrentUser.Username,
                            CreatedDate = DateTime.Now,
                            LastUpdateBy = CurrentUser.Username,
                            LastUpdateDate = DateTime.Now,
                        };

                        ctx.pmQuotas.Add(ent);

                        n = ctx.SaveChanges();
                    }
                    else
                    {
                        if (item.QuotaQty < item.IndentQty)
                        {
                            tranScope.Dispose();
                            return Json(new { success = false, message = "Qty Quota tidak boleh lebih kecil dari Qty Indent" });
                        }
                        if (ent.QuotaQty != item.QuotaQty)
                        {
                            ent.QuotaQty = item.QuotaQty;
                            ent.LastUpdateBy = CurrentUser.Username;
                            ent.LastUpdateDate = DateTime.Now;

                            n = ctx.SaveChanges();

                            if (ent.QuotaBy == "VAR")
                            {
                                ctx.Database.ExecuteSqlCommand("exec uspfn_RecalculateFromPmQuota '" + item.CompanyCode + "','" + item.TipeKendaraan + "','" + item.Variant + "'," + item.PeriodMonth + "," + item.PeriodYear + "");
                            }
                            if (ent.QuotaBy == "COL")
                            {
                                ctx.Database.ExecuteSqlCommand("exec uspfn_RecalculateFromPmQuotaByColour '" + item.CompanyCode + "','" + item.TipeKendaraan + "','" + item.Variant + "','" + item.ColourCode + "'," + item.PeriodMonth + "," + item.PeriodYear + "");
                            }
                        }
                    }

                }

                try
                {
                    tranScope.Complete();
                }
                catch (Exception ex)
                {
                    tranScope.Dispose();
                    return Json(new { success = false, message = ex.Message });
                }
            }

            

            if (listscoreerror.Count() == 0 && n > 0)
            {
                var package = new ExcelPackage();


                if (listscore == null) throw new Exception("Harap hubungi IT Support");
                package = GenerateExcelQuota(package, listscore);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { success = true, value = guid });
            }
            return Json(new { success = false, errorlist = listscoreerror });
        }

        public JsonResult Edit()
        {
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonth = Request["PeriodMonth"];
            var QuotaBy = Request["QuotaBy"];
            var data = Request["listScore"];

            int n = 0;

            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<pmQuota> listscore = ser.Deserialize<List<pmQuota>>(data);
            List<pmQuota> listscoreerror = new List<pmQuota>();

            using (var tranScope = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                foreach (var item in listscore)
                {
                    if (item.QuotaQty < item.IndentQty) {
                        tranScope.Dispose();
                        return Json(new { success = false, message = "Qty Quota tidak boleh lebih kecil dari Qty Indent" });
                    }
                    else
                    {
                        if(item.ColourCode != "-"){
                            item.ColourCode = item.ColourCode.Substring(0, 3);
                        }

                        var ent = ctx.pmQuotas.Find(CompanyCode, item.PeriodYear, item.PeriodMonth, item.TipeKendaraan, item.Variant, item.Transmisi, item.ColourCode);

                        ent.QuotaQty = item.QuotaQty;
                        ent.LastUpdateBy = CurrentUser.Username;
                        ent.LastUpdateDate = DateTime.Now;
                    }
                }

                try
                {
                    n = ctx.SaveChanges();
                    tranScope.Complete();
                }
                catch (Exception ex)
                {
                    tranScope.Dispose();
                    return Json(new { success = false, message = ex.Message });
                }
            }



            if (listscoreerror.Count() == 0 && n > 0)
            {
                var package = new ExcelPackage();


                if (listscore == null) throw new Exception("Harap hubungi IT Support");
                package = GenerateExcelQuota(package, listscore);

                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { success = true, value = guid });
            }
            return Json(new { success = false, errorlist = listscoreerror });
        }

        private static string GetColName(int number)
        {
            return Helpers.EPPlusHelper.ExcelColumnNameFromNumber(number);
        }

        private static int GetColNumber(string name)
        {
            return Helpers.EPPlusHelper.ExcelColumnNameToNumber(name);
        }

        private static string GetCell(int row, int col)
        {
            return GetColName(col) + row.ToString();
        }

        private static string GetRange(int row1, int col1, int row2, int col2)
        {
            return GetCell(row1, col1) + ":" + GetCell(row2, col2);
        }

        private class ExcelCellItem
        {
            public int Column { get; set; }
            public double Width { get; set; }
            public object Value { get; set; }
            public string Format { get; set; }
        }

        private static double GetTrueColWidth(double width)
        {
            //DEDUCE WHAT THE COLUMN WIDTH WOULD REALLY GET SET TO
            double z = 1d;
            if (width >= (1 + 2 / 3))
            {
                z = Math.Round((Math.Round(7 * (width - 1 / 256), 0) - 5) / 7, 2);
            }
            else
            {
                z = Math.Round((Math.Round(12 * (width - 1 / 256), 0) - Math.Round(5 * width, 0)) / 12, 2);
            }

            //HOW FAR OFF? (WILL BE LESS THAN 1)
            double errorAmt = width - z;

            //CALCULATE WHAT AMOUNT TO TACK ONTO THE ORIGINAL AMOUNT TO RESULT IN THE CLOSEST POSSIBLE SETTING 
            double adj = 0d;
            if (width >= (1 + 2 / 3))
            {
                adj = (Math.Round(7 * errorAmt - 7 / 256, 0)) / 7;
            }
            else
            {
                adj = ((Math.Round(12 * errorAmt - 12 / 256, 0)) / 12) + (2 / 12);
            }

            //RETURN A SCALED-VALUE THAT SHOULD RESULT IN THE NEAREST POSSIBLE VALUE TO THE TRUE DESIRED SETTING
            if (z > 0)
            {
                return width + adj;
            }

            return 0d;
        }

        private static ExcelPackage GenerateExcelQuota(ExcelPackage package, List<pmQuota> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 3,
                rDataStart = 4,

                cStart = 1,
                cTitleVal = 4;
            double
                wCompanyCode = GetTrueColWidth(10.00),
                wPeriodYear = GetTrueColWidth(10.00),
                wPeriodMonth = GetTrueColWidth(10.00),
                wTipeKendaraan = GetTrueColWidth(30.00),
                wVariant = GetTrueColWidth(20.00),
                wTransmisi = GetTrueColWidth(10.00),
                wColourCode = GetTrueColWidth(15.00),
                wQuotaQty = GetTrueColWidth(10.00),
                wIndentQty = GetTrueColWidth(10.00);
                //wQuotaBy = GetTrueColWidth(10.00);
                //wCreatedBy = GetTrueColWidth(10.86),
                //wCreatedDate = GetTrueColWidth(10.86),
                //wLastUpdateBy = GetTrueColWidth(10.86),
                //wLastUpdateDate = GetTrueColWidth(10.86);
            #endregion
            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Data Master Quota";
            sheet.Cells[x].Style.Font.Size = 15;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = wCompanyCode, Value = "Dealer Code" },
                new ExcelCellItem { Column = 2, Width = wPeriodYear, Value = "Period Year" },
                new ExcelCellItem { Column = 3, Width = wPeriodMonth, Value = "Period Month" },
                new ExcelCellItem { Column = 4, Width = wTipeKendaraan, Value = "Tipe Kendaraan" },
                new ExcelCellItem { Column = 5, Width = wVariant, Value = "Variant" },
                new ExcelCellItem { Column = 6, Width = wTransmisi, Value = "Transmisi" },
                new ExcelCellItem { Column = 7, Width = wColourCode, Value = "Colour Code" },
                new ExcelCellItem { Column = 8, Width = wQuotaQty, Value = "Quota Qty" },
                new ExcelCellItem { Column = 9, Width = wIndentQty, Value = "Indent Qty" },
                //new ExcelCellItem { Column = 9, Width = wQuotaBy, Value = "Quota By" },
                //new ExcelCellItem { Column = 10, Width = wCreatedBy, Value = "Created By" },
                //new ExcelCellItem { Column = 11, Width = wCreatedDate, Value = "Created Date" },
                //new ExcelCellItem { Column = 12, Width = wLastUpdateBy, Value = "Last Update By" },
                //new ExcelCellItem { Column = 13, Width = wLastUpdateDate, Value = "Last Update Date" },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Cells[x].Value = header.Value;
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
            #endregion

            //DATA
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rDataStart + i;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = data[i].CompanyCode },
                    new ExcelCellItem { Column = 2, Value = data[i].PeriodYear },
                    new ExcelCellItem { Column = 3, Value = data[i].PeriodMonth },
                    new ExcelCellItem { Column = 4, Value = data[i].TipeKendaraan },
                    new ExcelCellItem { Column = 5, Value = data[i].Variant },
                    new ExcelCellItem { Column = 6, Value = data[i].Transmisi },
                    new ExcelCellItem { Column = 7, Value = data[i].ColourCode },
                    new ExcelCellItem { Column = 8, Value = data[i].QuotaQty },
                    new ExcelCellItem { Column = 9, Value = data[i].IndentQty },
                    //new ExcelCellItem { Column = 9, Value = data[i].QuotaBy },
                    //new ExcelCellItem { Column = 10, Value = data[i].CreatedBy },
                    //new ExcelCellItem { Column = 11, Value = data[i].CreatedDate, Format = "DD - MMM - YYYY" },
                    //new ExcelCellItem { Column = 12, Value = data[i].LastUpdateBy },
                    //new ExcelCellItem { Column = 13, Value = data[i].LastUpdateDate , Format = "DD - MMM - YYYY"},
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            return package;
        }

        public FileContentResult DownloadExcelFile(string listScore)
        {
            //var data = Request["listScore"];
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<pmQuota> listscore = ser.Deserialize<List<pmQuota>>(listScore);

            var package = new ExcelPackage();

            if (listscore == null) throw new Exception("Harap hubungi IT Support");
            package = GenerateExcelQuota(package, listscore);

            var content = package.GetAsByteArray();
            var guid = Guid.NewGuid().ToString();
            TempData.Add(guid, content);

            content = TempData.FirstOrDefault(x => x.Key == guid).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var Tanggal = DateTime.Now.ToString("dd-mm-yyyy");
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment;filename=Master_Quota_" + Tanggal +".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }
    }
}