using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SimDms.Common;
using System.Web.Mvc;
using System.Drawing;
using EP = SimDms.Common.EPPlusHelper;
using SimDms.Common.Models;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace SimDms.Sparepart.Controllers.Api
{
    public class ReportMutationStockController : BaseController
    {
        public JsonResult Printxls(string BranchCodeParam, string TypeOfGoodsParam, string FromMonthParam, string FromYearParam, string ToMonthParam, string ToYearParam)
        {
            var message = "";
            var BranchCode = (BranchCodeParam == null) ? Request.Params["BranchCode"] : BranchCodeParam;
            var TypeOfGoods = (TypeOfGoodsParam == null) ? Request.Params["TypeOfGoods"] : TypeOfGoodsParam;
            var FromMonth = (FromMonthParam == null) ? Request.Params["FromMonth"] : FromMonthParam;
            var FromYear = (FromYearParam == null) ? Request.Params["FromYear"] : FromYearParam;
            var ToMonth = (ToMonthParam == null) ? Request.Params["ToMonth"] : ToMonthParam;
            var ToYear = (ToYearParam == null) ? Request.Params["ToYear"] : ToYearParam;
                        
            try
            {
                var package = new ExcelPackage();
                var model = new {
                    BranchCode = BranchCode,
                    TypeOfGoods = TypeOfGoods,
                    FromMonth = FromMonth,
                    FromYear = FromYear,
                    ToMonth = ToMonth,
                    ToYear = ToYear
                };
                
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600;
                cmd.CommandText = "uspfn_GnGetMutationSparepart";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("BranchCode", BranchCode);
                cmd.Parameters.AddWithValue("TypeOfGoods", TypeOfGoods);
                cmd.Parameters.AddWithValue("FromYear", FromYear);
                cmd.Parameters.AddWithValue("ToYear", ToYear);
                cmd.Parameters.AddWithValue("FromMonth", FromMonth);
                cmd.Parameters.AddWithValue("ToMonth", ToMonth);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                List<MutationStockModel> data = new List<MutationStockModel>();
                data = ConvertDataTable<MutationStockModel>(dt);
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");

                List<CoProfile> AllBranch = ctx.CoProfiles.OrderBy(x => x.BranchCode).ToList();
                package = GenerateExcel(package, model, AllBranch, data);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);
                return Json(new { message = message, value = guid });
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Json(new { message = message });
            }
        }

        private static ExcelPackage GenerateExcel(ExcelPackage package, dynamic model, List<CoProfile> BranchCode, List<MutationStockModel> data)
        {
            string z = "", x = "";
            var sheet = new Dictionary<string, ExcelWorksheet>();

            #region -- Constants --
            const int
                rTitle = 1,
                rHdrFirst = 5,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 6;
            double
                w1086 = EP.GetTrueColWidth(10.86),
                w1143 = EP.GetTrueColWidth(11.43),
                w1243 = EP.GetTrueColWidth(12.43),
                w1600 = EP.GetTrueColWidth(16.00),
                w1700 = EP.GetTrueColWidth(17.00),
                w1800 = EP.GetTrueColWidth(18.00),
                w1900 = EP.GetTrueColWidth(19.00),
                w3000 = EP.GetTrueColWidth(30.00),
                w4214 = EP.GetTrueColWidth(42.14);
            int index = 0;
            #endregion
                        
            //TITLE
            #region -- Title & Header --
            if (model.BranchCode == "")
            {
                for (var i = 0; i < BranchCode.Count; i++)
                {
                    sheet[BranchCode[i].BranchCode] = package.Workbook.Worksheets.Add(BranchCode[i].BranchCode);

                    z = EP.GetRange(rTitle, cStart, rTitle, cTitleVal);
                    sheet[BranchCode[i].BranchCode].Cells[z].Merge = true;

                    x = EP.GetCell(rTitle, cStart);
                    sheet[BranchCode[i].BranchCode].Cells[x].Value = BranchCode[i].CompanyName;
                    sheet[BranchCode[i].BranchCode].Cells[x].Style.Font.Size = 20;

                    x = EP.GetCell((rTitle + 1), cStart);
                    sheet[BranchCode[i].BranchCode].Cells[x].Value = "LAPORAN MUTASI STOCK SPARE PART";
                    sheet[BranchCode[i].BranchCode].Cells[x].Style.Font.Size = 13;

                    x = EP.GetCell((rTitle + 2), cStart);
                    sheet[BranchCode[i].BranchCode].Cells[x].Value = "PERIODE " + DateTimeFormatInfo.CurrentInfo.GetMonthName(Convert.ToInt32(model.FromMonth)) + " " + model.FromYear + " - " + DateTimeFormatInfo.CurrentInfo.GetMonthName(Convert.ToInt32(model.ToMonth)) + " " + model.ToYear;
                    sheet[BranchCode[i].BranchCode].Cells[x].Style.Font.Size = 13;

                    //TABLE HEADER
                    var headers = new List<ExcelCellItem>
                    {
                        new ExcelCellItem { Column = 1, Width = w1086, Value = "NO" },
                        new ExcelCellItem { Column = 2, Width = w1600, Value = "NO PART" },
                        new ExcelCellItem { Column = 3, Width = w1900, Value = "NAMA PART" },
                        new ExcelCellItem { Column = 4, Width = w1600, Value = "QTY" },
                        new ExcelCellItem { Column = 5, Width = w1600, Value = "" },
                        new ExcelCellItem { Column = 6, Width = w1600, Value = "" },
                        new ExcelCellItem { Column = 7, Width = w1600, Value = "" },
                        new ExcelCellItem { Column = 8, Width = w1600, Value = "" },
                        new ExcelCellItem { Column = 9, Width = w1600, Value = "" },
                        new ExcelCellItem { Column = 10, Width = w1900, Value = "RUPIAH" },
                        new ExcelCellItem { Column = 11, Width = w1900, Value = "" },
                        new ExcelCellItem { Column = 12, Width = w1900, Value = "" },
                        new ExcelCellItem { Column = 13, Width = w1900, Value = "" },
                        new ExcelCellItem { Column = 14, Width = w1900, Value = "" },
                        new ExcelCellItem { Column = 15, Width = w1900, Value = "" },
                        new ExcelCellItem { Column = 16, Width = w1900, Value = "AVERAGE" },
                    };

                    sheet[BranchCode[i].BranchCode].Cells[EP.GetRange(rHdrFirst, 4, rHdrFirst, 9)].Merge = true;
                    sheet[BranchCode[i].BranchCode].Cells[EP.GetRange(rHdrFirst, 10, rHdrFirst, 15)].Merge = true;

                    sheet[BranchCode[i].BranchCode].Cells[EP.GetRange(rHdrFirst, 1, (rHdrFirst + 1), 1)].Merge = true;
                    sheet[BranchCode[i].BranchCode].Cells[EP.GetRange(rHdrFirst, 2, (rHdrFirst + 1), 2)].Merge = true;
                    sheet[BranchCode[i].BranchCode].Cells[EP.GetRange(rHdrFirst, 3, (rHdrFirst + 1), 3)].Merge = true;
                    sheet[BranchCode[i].BranchCode].Cells[EP.GetRange(rHdrFirst, 16, (rHdrFirst + 1), 16)].Merge = true;

                    var headers2 = new List<ExcelCellItem>
                    {
                        new ExcelCellItem { Column = 1, Width = w1086, Value = "" },
                        new ExcelCellItem { Column = 2, Width = w1600, Value = "" },
                        new ExcelCellItem { Column = 3, Width = w1900, Value = "" },
                        new ExcelCellItem { Column = 4, Width = w1600, Value = "SALDO AWAL" },
                        new ExcelCellItem { Column = 5, Width = w1600, Value = "PEMBELIAN" },
                        new ExcelCellItem { Column = 6, Width = w1600, Value = "TRANSFER IN" },
                        new ExcelCellItem { Column = 7, Width = w1600, Value = "PENJUALAN" },
                        new ExcelCellItem { Column = 8, Width = w1600, Value = "TRANSFER OUT" },
                        new ExcelCellItem { Column = 9, Width = w1600, Value = "SALDO AKHIR" },
                        new ExcelCellItem { Column = 10, Width = w1900, Value = "SALDO AWAL" },
                        new ExcelCellItem { Column = 11, Width = w1900, Value = "PEMBELIAN" },
                        new ExcelCellItem { Column = 12, Width = w1900, Value = "TRANSFER IN" },
                        new ExcelCellItem { Column = 13, Width = w1900, Value = "PENJUALAN" },
                        new ExcelCellItem { Column = 14, Width = w1900, Value = "TRANSFER OUT" },
                        new ExcelCellItem { Column = 15, Width = w1900, Value = "SALDO AKHIR" },
                        new ExcelCellItem { Column = 16, Width = w1900, Value = "" },
                    };

                    foreach (var header in headers)
                    {
                        x = EP.GetCell(rHdrFirst, header.Column);
                        sheet[BranchCode[i].BranchCode].Cells[x].Value = header.Value;
                        sheet[BranchCode[i].BranchCode].Column(header.Column).Width = header.Width;
                        sheet[BranchCode[i].BranchCode].Cells[x].Style.WrapText = true;
                        sheet[BranchCode[i].BranchCode].Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                        sheet[BranchCode[i].BranchCode].Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet[BranchCode[i].BranchCode].Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                        sheet[BranchCode[i].BranchCode].Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet[BranchCode[i].BranchCode].Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    foreach (var header in headers2)
                    {
                        x = EP.GetCell((rHdrFirst + 1), header.Column);
                        sheet[BranchCode[i].BranchCode].Cells[x].Value = header.Value;
                        sheet[BranchCode[i].BranchCode].Column(header.Column).Width = header.Width;
                        sheet[BranchCode[i].BranchCode].Cells[x].Style.WrapText = true;
                        sheet[BranchCode[i].BranchCode].Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                        sheet[BranchCode[i].BranchCode].Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet[BranchCode[i].BranchCode].Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                        sheet[BranchCode[i].BranchCode].Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet[BranchCode[i].BranchCode].Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                }
            }
            else
            {
                sheet[model.BranchCode] = package.Workbook.Worksheets.Add(model.BranchCode);

                z = EP.GetRange(rTitle, cStart, rTitle, cTitleVal);
                sheet[model.BranchCode].Cells[z].Merge = true;

                x = EP.GetCell(rTitle, cStart);
                sheet[model.BranchCode].Cells[x].Value = BranchCode.Where(p => p.BranchCode == model.BranchCode).Select(p => new { text = p.CompanyName }).ToList()[0].text.ToString();
                sheet[model.BranchCode].Cells[x].Style.Font.Size = 20;

                x = EP.GetCell((rTitle + 1), cStart);
                sheet[model.BranchCode].Cells[x].Value = "LAPORAN MUTASI STOCK SPARE PART";
                sheet[model.BranchCode].Cells[x].Style.Font.Size = 13;

                x = EP.GetCell((rTitle + 2), cStart);
                sheet[model.BranchCode].Cells[x].Value = "PERIODE " + DateTimeFormatInfo.CurrentInfo.GetMonthName(Convert.ToInt32(model.FromMonth)) + " " + model.FromYear + " - " + DateTimeFormatInfo.CurrentInfo.GetMonthName(Convert.ToInt32(model.ToMonth)) + " " + model.ToYear;
                sheet[model.BranchCode].Cells[x].Style.Font.Size = 13;
                //TABLE HEADER
                var headers = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Width = w1086, Value = "NO" },
                    new ExcelCellItem { Column = 2, Width = w1600, Value = "NO PART" },
                    new ExcelCellItem { Column = 3, Width = w1900, Value = "NAMA PART" },
                    new ExcelCellItem { Column = 4, Width = w1600, Value = "QTY" },
                    new ExcelCellItem { Column = 5, Width = w1600, Value = "" },
                    new ExcelCellItem { Column = 6, Width = w1600, Value = "" },
                    new ExcelCellItem { Column = 7, Width = w1600, Value = "" },
                    new ExcelCellItem { Column = 8, Width = w1600, Value = "" },
                    new ExcelCellItem { Column = 9, Width = w1600, Value = "" },
                    new ExcelCellItem { Column = 10, Width = w1900, Value = "RUPIAH" },
                    new ExcelCellItem { Column = 11, Width = w1900, Value = "" },
                    new ExcelCellItem { Column = 12, Width = w1900, Value = "" },
                    new ExcelCellItem { Column = 13, Width = w1900, Value = "" },
                    new ExcelCellItem { Column = 14, Width = w1900, Value = "" },
                    new ExcelCellItem { Column = 15, Width = w1900, Value = "" },
                    new ExcelCellItem { Column = 16, Width = w1900, Value = "AVERAGE" },
                };

                sheet[model.BranchCode].Cells[EP.GetRange(rHdrFirst, 4, rHdrFirst, 9)].Merge = true;
                sheet[model.BranchCode].Cells[EP.GetRange(rHdrFirst, 10, rHdrFirst, 15)].Merge = true;

                sheet[model.BranchCode].Cells[EP.GetRange(rHdrFirst, 1, (rHdrFirst + 1), 1)].Merge = true;
                sheet[model.BranchCode].Cells[EP.GetRange(rHdrFirst, 2, (rHdrFirst + 1), 2)].Merge = true;
                sheet[model.BranchCode].Cells[EP.GetRange(rHdrFirst, 3, (rHdrFirst + 1), 3)].Merge = true;
                sheet[model.BranchCode].Cells[EP.GetRange(rHdrFirst, 16, (rHdrFirst + 1), 16)].Merge = true;

                var headers2 = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Width = w1086, Value = "" },
                    new ExcelCellItem { Column = 2, Width = w1600, Value = "" },
                    new ExcelCellItem { Column = 3, Width = w1900, Value = "" },
                    new ExcelCellItem { Column = 4, Width = w1600, Value = "SALDO AWAL" },
                    new ExcelCellItem { Column = 5, Width = w1600, Value = "PEMBELIAN" },
                    new ExcelCellItem { Column = 6, Width = w1600, Value = "TRANSFER IN" },
                    new ExcelCellItem { Column = 7, Width = w1600, Value = "PENJUALAN" },
                    new ExcelCellItem { Column = 8, Width = w1600, Value = "TRANSFER OUT" },
                    new ExcelCellItem { Column = 9, Width = w1600, Value = "SALDO AKHIR" },
                    new ExcelCellItem { Column = 10, Width = w1900, Value = "SALDO AWAL" },
                    new ExcelCellItem { Column = 11, Width = w1900, Value = "PEMBELIAN" },
                    new ExcelCellItem { Column = 12, Width = w1900, Value = "TRANSFER IN" },
                    new ExcelCellItem { Column = 13, Width = w1900, Value = "PENJUALAN" },
                    new ExcelCellItem { Column = 14, Width = w1900, Value = "TRANSFER OUT" },
                    new ExcelCellItem { Column = 15, Width = w1900, Value = "SALDO AKHIR" },
                    new ExcelCellItem { Column = 16, Width = w1900, Value = "" },
                };

                foreach (var header in headers)
                {
                    x = EP.GetCell(rHdrFirst, header.Column);
                    sheet[model.BranchCode].Cells[x].Value = header.Value;
                    sheet[model.BranchCode].Column(header.Column).Width = header.Width;
                    sheet[model.BranchCode].Cells[x].Style.WrapText = true;
                    sheet[model.BranchCode].Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    sheet[model.BranchCode].Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet[model.BranchCode].Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                    sheet[model.BranchCode].Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet[model.BranchCode].Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }

                foreach (var header in headers2)
                {
                    x = EP.GetCell((rHdrFirst + 1), header.Column);
                    sheet[model.BranchCode].Cells[x].Value = header.Value;
                    sheet[model.BranchCode].Column(header.Column).Width = header.Width;
                    sheet[model.BranchCode].Cells[x].Style.WrapText = true;
                    sheet[model.BranchCode].Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    sheet[model.BranchCode].Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sheet[model.BranchCode].Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                    sheet[model.BranchCode].Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet[model.BranchCode].Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
            }
            #endregion

            #region -- Data --
            if (data.Count() == 0) return package;
            for (int i = 0; i < data.Count(); i++)
            {
                if (i != 0)
                {
                    if (data[i].BranchCode != data[i-1].BranchCode)
                    {
                        index = 0;
                    }
                }
                var row = rDataStart + index;
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = 1, Value = (index+1) },
                    new ExcelCellItem { Column = 2, Value = data[i].PartNo },
                    new ExcelCellItem { Column = 3, Value = data[i].PartName },
                    new ExcelCellItem { Column = 4, Value = data[i].OnHandQty },
                    new ExcelCellItem { Column = 5, Value = data[i].PurchaseQty },
                    new ExcelCellItem { Column = 6, Value = data[i].TransferInQty },
                    new ExcelCellItem { Column = 7, Value = data[i].OrderQty },
                    new ExcelCellItem { Column = 8, Value = data[i].TransferOutQty },
                    new ExcelCellItem { Column = 9, Value = data[i].LastStockQty },
                    new ExcelCellItem { Column = 10, Value = data[i].OnHandAmt, Format = "#,##0" },
                    new ExcelCellItem { Column = 11, Value = data[i].PurchaseAmt, Format = "#,##0" },
                    new ExcelCellItem { Column = 12, Value = data[i].TransferInAmt, Format = "#,##0" },
                    new ExcelCellItem { Column = 13, Value = data[i].OrderAmt, Format = "#,##0" },
                    new ExcelCellItem { Column = 14, Value = data[i].TransferOutAmt, Format = "#,##0" },
                    new ExcelCellItem { Column = 15, Value = data[i].LastStockAmt, Format = "#,##0" },
                    new ExcelCellItem { Column = 16, Value = data[i].Average, Format = "#,##0" },
                };

                foreach (var item in items)
                {
                    x = EP.GetCell(row, item.Column);
                    sheet[data[i].BranchCode].Cells[x].Value = item.Value;
                    sheet[data[i].BranchCode].Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet[data[i].BranchCode].Cells[x].Style.Numberformat.Format = item.Format;
                }
                index++;
            }
            #endregion

            return package;
        }

        public FileContentResult DownloadExcelFile(string key, string filename)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + "_" + DateTime.Now.ToString("dd-M-yyyy hh:mm:ss") + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }
        
        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }

        #region -- Model Data --
        private class Branch
        {
            public string BranchCode { get; set; }
        }

        private class MutationStockModel
        {
            public string BranchCode { get; set; }
            public string BranchName { get; set; }
            public string PartNo { get; set; }
            public string PartName { get; set; }
            public decimal OnHandQty { get; set; }
            public decimal PurchaseQty { get; set; }
            public decimal TransferInQty { get; set; }
            public decimal OrderQty { get; set; }
            public decimal TransferOutQty { get; set; }
            public decimal LastStockQty { get; set; }
            public decimal OnHandAmt { get; set; }
            public decimal PurchaseAmt { get; set; }
            public decimal TransferInAmt { get; set; }
            public decimal OrderAmt { get; set; }
            public decimal TransferOutAmt { get; set; }
            public decimal LastStockAmt { get; set; }
            public decimal Average { get; set; }
        }
        #endregion
    }
}
