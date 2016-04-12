using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.Models;
using System.Data.SqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using EP = SimDms.Common.EPPlusHelper;
using System.Drawing;
using SimDms.Common;

namespace SimDms.Sales.Controllers.Api
{
    public class RegisterRevisiFakPolController : BaseController
    {
        public JsonResult GetUserProfile()
        {
            try
            {
                var branch = ctx.SysUsers.FirstOrDefault(x => x.UserId == CurrentUser.UserId).BranchCode;
                var isHolding = !ctx.OrganizationDtls.FirstOrDefault(x => x.BranchCode == branch).IsBranch;

                return Json(new
                {
                    message = "",
                    today = DateTime.Now.Date,
                    isHolding = isHolding
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    message = ex.Message,
                    inner = ex.InnerException != null ? ex.InnerException.Message : ""
                });
            }
        }

        public JsonResult GetDealer()
        {
            try
            {
                var dealer = ctx.GnMstDealerMapping.FirstOrDefault(x => x.DealerCode == CurrentUser.CompanyCode);
                var dealers = ctx.GnMstDealerMapping.Where(x => x.isActive.Value).Select(x => new
                {
                    value = x.DealerCode,
                    text = x.DealerName
                });

                return Json(new 
                { 
                    message = "", 
                    dealerCode = dealer.DealerCode, 
                    dealers = dealers
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    message = ex.Message,
                    inner = ex.InnerException != null ? ex.InnerException.Message : ""
                });
            }
        }

        public JsonResult GetOutlets()
        {
            try
            {
                var branch = ctx.SysUsers.FirstOrDefault(x => x.UserId == CurrentUser.UserId).BranchCode;
                var outlets = ctx.DealerOutletMappings.Where(x => x.DealerCode == CurrentUser.CompanyCode && x.isActive.Value)
                    .Select(x => new 
                    {
                        value = x.OutletCode,
                        text = x.OutletName
                    });

                return Json(new { message = "", outletCode = branch, outlets = outlets });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    message = ex.Message,
                    inner = ex.InnerException != null ? ex.InnerException.Message : ""
                });
            }
        }

        public JsonResult GetReport(RevRegFPolModel model)
        {
            try
            {
                var query = "exec usprpt_omRevRegFakPol @CompanyCode, @BranchCode, @DateFrom, @DateTo";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@CompanyCode", model.CompanyCode),
                    new SqlParameter("@BranchCode", model.BranchCode),
                    new SqlParameter("@DateFrom", model.DateFrom),
                    new SqlParameter("@DateTo", model.DateTo),
                };
                var data = ctx.Database.SqlQuery<RevRegFPolResult>(query, parameters).ToList();
                if (data.Count() == 0) throw new Exception("There is no data that matches your query");

                var package = new ExcelPackage();
                var book = GenerateExcel(package, model, data);
                var content = book.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = "", value = guid });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    message = ex.Message,
                    inner = ex.InnerException != null ? ex.InnerException.Message : ""
                });
            }
        }

        public FileContentResult DownloadExcelFile(string key)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=RevRegFakPol_" + DateTime.Now.ToString("yyyyMMdd-hhmm") + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        private DateTime DateTimeToString(string date)
        {
            var year = int.Parse(date.Substring(0, 4));
            var month = int.Parse(date.Substring(4, 2));
            var day = int.Parse(date.Substring(6, 2));

            return new DateTime(year, month, day);
        }

        private ExcelPackage GenerateExcel(ExcelPackage package, RevRegFPolModel model, List<RevRegFPolResult> data)
        {
            var dateFrom = DateTimeToString(model.DateFrom);
            var dateTo = DateTimeToString(model.DateTo);

            #region -- Constants --
            var ctx = new DataContext();
            var sheet = package.Workbook.Worksheets.Add("book1");
            var z = sheet.Cells[1, 1];

            const int
                rTitle = 1,
                rPeriod = 2,
                rHdr1 = 4,
                rHdr2 = 5,
                rData = 6,

                cNo = 1,
                cRevNo = 2,
                cRevDate = 3,
                cRevSeq = 4,
                cRevCode = 5,
                cRevName = 6,
                cChsCode = 7,
                cChsNo = 8,
                cFPName = 9,
                cAdd1 = 10,
                cAdd2 = 11,
                cAdd3 = 12,
                cPosCode = 13,
                cPosName = 14,
                cCity = 15,
                cTelp1 = 16,
                cTelp2 = 17,
                cHP = 18,
                cBday = 19,
                cFPNo = 20,
                cFPDate = 21,
                cIDNo = 22;

            const string
                fNumber = "#",
                fDate = "dd-mmm-yyyy",
                fGeneral = "";

            #endregion

            #region -- Title --
            z.Address = EP.GetCell(rTitle, cNo);
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 14f;
            z.Value = "REGISTER REVISI FAKTUR POLISI";

            z.Address = EP.GetCell(rPeriod, cNo);
            z.Style.Font.Size = 12f;
            z.Value = "Period Date: " + dateFrom.ToString("dd-MMM-yyyy") + " to " + dateTo.ToString("dd-MMM-yyyy");
            #endregion

            #region -- Headers --
            z.Address = EP.GetRange(rHdr1, cNo, rHdr2, cIDNo);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0));
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 10f;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHdr2, cNo);
            z.Value = "No";

            z.Address = EP.GetRange(rHdr1, cRevNo, rHdr1, cRevName);
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Revision";

            z.Address = EP.GetCell(rHdr2, cRevNo);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "No";

            z.Address = EP.GetCell(rHdr2, cRevDate);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Date";

            z.Address = EP.GetCell(rHdr2, cRevSeq);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Seq";

            z.Address = EP.GetCell(rHdr2, cRevCode);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Code";

            z.Address = EP.GetCell(rHdr2, cRevName);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Name";

            z.Address = EP.GetRange(rHdr1, cChsCode, rHdr1, cChsNo);
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Chassis";

            z.Address = EP.GetCell(rHdr2, cChsCode);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Code";

            z.Address = EP.GetCell(rHdr2, cChsNo);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "No";

            z.Address = EP.GetRange(rHdr1, cFPName, rHdr1, cAdd3);
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Faktur Polisi";

            z.Address = EP.GetCell(rHdr2, cFPName);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Name";

            z.Address = EP.GetCell(rHdr2, cAdd1);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Address1";

            z.Address = EP.GetCell(rHdr2, cAdd2);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Address2";

            z.Address = EP.GetCell(rHdr2, cAdd3);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Address3";

            z.Address = EP.GetRange(rHdr1, cPosCode, rHdr1, cPosName);
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Postal";

            z.Address = EP.GetCell(rHdr2, cPosCode);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Code";

            z.Address = EP.GetCell(rHdr2, cPosName);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Name";

            z.Address = EP.GetRange(rHdr1, cCity, rHdr1, cFPDate);
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Faktur Polisi";

            z.Address = EP.GetCell(rHdr2, cCity);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "City";

            z.Address = EP.GetCell(rHdr2, cTelp1);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Telp1";

            z.Address = EP.GetCell(rHdr2, cTelp2);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Telp2";

            z.Address = EP.GetCell(rHdr2, cHP);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "HP";

            z.Address = EP.GetCell(rHdr2, cBday);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Birthday";

            z.Address = EP.GetCell(rHdr2, cFPNo);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "No";

            z.Address = EP.GetCell(rHdr2, cFPDate);
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "Date";

            z.Address = EP.GetRange(rHdr1, cIDNo, rHdr2, cIDNo);
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            z.Value = "ID No";
            #endregion

            if (data.Count() == 0) return package;

            #region -- Data --
            for (int i = 0; i < data.Count(); i++)
            {
                var row = rData + i;
                
                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cNo, Format = fNumber, Value = data[i].No, Width = 4.00 },
                    new ExcelCellItem { Column = cRevNo, Format = fGeneral, Value = data[i].RevisionNo, Width = 15.00 },
                    new ExcelCellItem { Column = cRevDate, Format = fDate, Value = data[i].RevisionDate, Width = 12.43 },
                    new ExcelCellItem { Column = cRevSeq, Format = fNumber, Value = data[i].RevisionSeq, Width = 4.00 },
                    new ExcelCellItem { Column = cRevCode, Format = fGeneral, Value = data[i].RevisionCode, Width = 7.00 },
                    new ExcelCellItem { Column = cRevName, Format = fGeneral, Value = data[i].RevisionName, Width = 32.00 },
                    new ExcelCellItem { Column = cChsCode, Format = fGeneral, Value = data[i].ChassisCode, Width = 15.00 },
                    new ExcelCellItem { Column = cChsNo, Format = fNumber, Value = data[i].ChassisNo, Width = 8.00 },
                    new ExcelCellItem { Column = cFPName, Format = fGeneral, Value = data[i].FakturPolisiName, Width = 42.00 },
                    new ExcelCellItem { Column = cAdd1, Format = fGeneral, Value = data[i].FakturPolisiAddress1, Width = 42.00 },
                    new ExcelCellItem { Column = cAdd2, Format = fGeneral, Value = data[i].FakturPolisiAddress2, Width = 42.00 },
                    new ExcelCellItem { Column = cAdd3, Format = fGeneral, Value = data[i].FakturPolisiAddress3, Width = 30.00 },
                    new ExcelCellItem { Column = cPosCode, Format = fGeneral, Value = data[i].PostalCode, Width = 6.30 },
                    new ExcelCellItem { Column = cPosName, Format = fGeneral, Value = data[i].PostalCodeDesc, Width = 20.00 },
                    new ExcelCellItem { Column = cCity, Format = fGeneral, Value = data[i].FakturPolisiCity, Width = 30.00 },
                    new ExcelCellItem { Column = cTelp1, Format = fGeneral, Value = data[i].FakturPolisiTelp1, Width = 14.00 },
                    new ExcelCellItem { Column = cTelp2, Format = fGeneral, Value = data[i].FakturPolisiTelp2, Width = 14.00 },
                    new ExcelCellItem { Column = cHP, Format = fGeneral, Value = data[i].FakturPolisiHP, Width = 14.00 },
                    new ExcelCellItem { Column = cBday, Format = fDate, Value = data[i].FakturPolisiBirthday, Width = 12.43 },
                    new ExcelCellItem { Column = cFPNo, Format = fGeneral, Value = data[i].FakturPolisiNo, Width = 14.00 },
                    new ExcelCellItem { Column = cFPDate, Format = fDate, Value = data[i].FakturPolisiDate, Width = 12.43 },
                    new ExcelCellItem { Column = cIDNo, Format = fGeneral, Value = data[i].IDNo, Width = 20.00 }
                };

                foreach (var item in items)
                {
                    z.Address = EP.GetCell(row, item.Column);
                    z.Value = item.Value;
                    z.Style.Numberformat.Format = item.Format;
                    sheet.Column(item.Column).Width = EP.GetTrueColWidth(item.Width);
                }
            }
            #endregion

            return package;
        }

        public class RevRegFPolModel
        {
            public string CompanyCode { get; set; }
            public string BranchCode { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class RevRegFPolResult
        {
            public long? No { get; set; }
            public string RevisionNo { get; set; }
            public DateTime? RevisionDate { get; set; }
            public int? RevisionSeq { get; set; }
            public string RevisionCode { get; set; }
            public string RevisionName { get; set; }
            public string ChassisCode { get; set; }
            public decimal ChassisNo { get; set; }
            public string FakturPolisiName { get; set; }
            public string FakturPolisiAddress1 { get; set; }
            public string FakturPolisiAddress2 { get; set; }
            public string FakturPolisiAddress3 { get; set; }
            public string PostalCode { get; set; }
            public string PostalCodeDesc { get; set; }
            public string FakturPolisiCity { get; set; }
            public string FakturPolisiTelp1 { get; set; }
            public string FakturPolisiTelp2 { get; set; }
            public string FakturPolisiHP { get; set; }
            public DateTime? FakturPolisiBirthday { get; set; }
            public string FakturPolisiNo { get; set; }
            public DateTime? FakturPolisiDate { get; set; }
            public string FakturPolisiArea { get; set; }
            public string IDNo { get; set; }
        }
    }
}