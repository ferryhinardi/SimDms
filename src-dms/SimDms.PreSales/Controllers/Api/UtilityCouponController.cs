using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using System.Collections.Generic;
using SimDms.PreSales.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace SimDms.PreSales.Controllers.Api
{
    public class UtilityCouponController : BaseController
    {
        public JsonResult Coupons(bool cbAllData)
        {
            var Outlet = Request["Outlet"] ?? "";
            var CouponFrom = Request["CouponFrom"];
            var CouponTo = Request["CouponTo"];
            var DateFrom = Request["DateFrom"];
            var DateTo = Request["DateTo"];

            var qry = from p in ctx.pmKDPCoupon
                      join q in ctx.PmKdps
                        on new { p.CompanyCode, p.InquiryNumber } equals new { q.CompanyCode, q.InquiryNumber }
                      join r in ctx.HrEmployees
                        on new { q.CompanyCode, q.EmployeeID } equals new { r.CompanyCode, r.EmployeeID }
                      join s in ctx.HrEmployeeSales
                        on new { q.CompanyCode, q.EmployeeID } equals new { s.CompanyCode, s.EmployeeID }
                        join t in ctx.CoProfiles on q.BranchCode equals t.BranchCode into leftJoin
                        from t in leftJoin.DefaultIfEmpty()
                      where p.CompanyCode == CompanyCode
                      && q.BranchCode.Contains(Outlet)
                      select new
                      {
                          p.CompanyCode,
                          p.InquiryNumber,
                          p.CoupunNumber,
                          p.NamaProspek,
                          p.ProspekIdentityNo,
                          q.TelpRumah,
                          q.AlamatProspek,
                          p.TestDriveDate,
                          p.Email,
                          q.EmployeeID,
                          r.EmployeeName,
                          r.IdentityNo,
                          s.SalesID,
                          //CompanyName,
                          //BranchName,
                          t.CompanyGovName,
                          t.CompanyName,
                          p.Remark,
                          p.ProcessFlag
                      };

            if (!cbAllData)
            {
                qry = qry.Where(x => !x.ProcessFlag);
            }

            var list = qry.ToList().Where(x =>
                (!string.IsNullOrEmpty(CouponFrom) ? Convert.ToDouble(x.CoupunNumber) >= Convert.ToDouble(CouponFrom) : 1 == 1)
                && (!string.IsNullOrEmpty(CouponTo) ? Convert.ToDouble(x.CoupunNumber) <= Convert.ToDouble(CouponTo) : 1 == 1)
                && (!string.IsNullOrEmpty(DateFrom) ? Convert.ToDateTime(x.TestDriveDate) >= Convert.ToDateTime(DateFrom) : 1 == 1)
                && (!string.IsNullOrEmpty(DateTo) ? Convert.ToDateTime(x.TestDriveDate) <= Convert.ToDateTime(DateTo) : 1 == 1)
                );

            return Json(list.AsQueryable().KGrid());
        }

        public JsonResult Generate(List<CouponModel> CouponDatas)
        {
            List<CouponDataModel> data = new List<CouponDataModel>();

            if (CouponDatas == null)
            {
                return Json(new { message = "Tidak ada data yang di proses" });
            }

            foreach (var item in CouponDatas)
            {
                var entity = ctx.pmKDPCoupon.FirstOrDefault(x => x.CoupunNumber == item.CoupunNumber);
                entity.ProcessFlag = true;
                entity.ProcessDate = DateTime.Now;
            }
            try
            {
                ctx.SaveChanges();
            }
            catch (Exception) { }

            var qry = (from p in ctx.pmKDPCoupon
                      join q in ctx.PmKdps
                        on new { p.CompanyCode, p.InquiryNumber } equals new { q.CompanyCode, q.InquiryNumber }
                      join r in ctx.HrEmployees
                        on new { q.CompanyCode, q.EmployeeID } equals new { r.CompanyCode, r.EmployeeID }
                      join s in ctx.HrEmployeeSales
                        on new { q.CompanyCode, q.EmployeeID } equals new { s.CompanyCode, s.EmployeeID }
                       join t in ctx.CoProfiles on q.BranchCode equals t.BranchCode into leftJoin
                       from t in leftJoin.DefaultIfEmpty()
                      where p.CompanyCode == CompanyCode
                      select new CouponDataModel()
                      {
                          CompanyCode = p.CompanyCode,
                          InquiryNumber = p.InquiryNumber,
                          CoupunNumber = p.CoupunNumber,
                          NamaProspek = p.NamaProspek,
                          ProspekIdentityNo = p.ProspekIdentityNo,
                          TelpRumah = q.TelpRumah,
                          AlamatProspek = q.AlamatProspek,
                          TestDriveDate = p.TestDriveDate,
                          Email = p.Email,
                          EmployeeID = q.EmployeeID,
                          EmployeeName = r.EmployeeName,
                          IdentityNo = r.IdentityNo,
                          SalesID = s.SalesID,
                          //CompanyName = CompanyName,
                          //BranchName = BranchName,
                          CompanyName = t.CompanyGovName,
                          BranchName = t.CompanyName,
                          Remark = p.Remark,
                          ProcessFlag = p.ProcessFlag
                      }).ToList();

            data = qry.Where(x => CouponDatas.Select(c => c.CoupunNumber).Contains(x.CoupunNumber)).ToList();
            

            var package = new ExcelPackage();
            if (data == null) throw new Exception("Harap hubungi IT Support");
            package = GenerateExcelCoupon(package, data);

            var content = package.GetAsByteArray();
            var guid = Guid.NewGuid().ToString();
            TempData.Add(guid, content);

            return Json(new { message = "", value = guid });
        }

        private static ExcelPackage GenerateExcelCoupon(ExcelPackage package, List<CouponDataModel> data)
        {

            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 4;
            double
                w1086 = GetTrueColWidth(10.86),
                w1143 = GetTrueColWidth(11.43),
                w1243 = GetTrueColWidth(12.43),
                w1400 = GetTrueColWidth(14.00),
                w1600 = GetTrueColWidth(16.00),
                w1800 = GetTrueColWidth(18.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00),
                w6000 = GetTrueColWidth(60.00);

            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Coupon Data";
            sheet.Cells[x].Style.Font.Size = 20;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w1086, Value = "No Kupon" },
                new ExcelCellItem { Column = 2, Width = w1600, Value = "Tanggal Test Drive" },
                new ExcelCellItem { Column = 3, Width = w3000, Value = "Nama" },
                new ExcelCellItem { Column = 4, Width = w1243, Value = "No SIM A" },
                new ExcelCellItem { Column = 5, Width = w1600, Value = "Alamat" },
                new ExcelCellItem { Column = 6, Width = w1243, Value = "Telp/HP" },
                new ExcelCellItem { Column = 7, Width = w1400, Value = "Email" },
                new ExcelCellItem { Column = 8, Width = w3000, Value = "Nama Salesman" },
                new ExcelCellItem { Column = 9, Width = w1086, Value = "ID SIS (ITS)" },
                new ExcelCellItem { Column = 10, Width = w1086, Value ="No KTP" },
                new ExcelCellItem { Column = 11, Width = w3000, Value ="Dealer" },
                new ExcelCellItem { Column = 12, Width = w6000, Value ="Daerah" },
                new ExcelCellItem { Column = 13, Width = w1086, Value ="Remarks" }
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
                    new ExcelCellItem { Column = 1, Value = data[i].CoupunNumber },
                    new ExcelCellItem { Column = 2, Value =  data[i].TestDriveDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = 3, Value =  data[i].NamaProspek },
                    new ExcelCellItem { Column = 4, Value =  data[i].ProspekIdentityNo },
                    new ExcelCellItem { Column = 5, Value =  data[i].AlamatProspek },
                    new ExcelCellItem { Column = 6, Value =  data[i].TelpRumah },
                    new ExcelCellItem { Column = 7, Value =  data[i].Email},
                    new ExcelCellItem { Column = 8, Value =  data[i].EmployeeName },
                    new ExcelCellItem { Column = 9, Value =  data[i].EmployeeID },
                    new ExcelCellItem { Column = 10, Value = data[i].IdentityNo },
                    new ExcelCellItem { Column = 11, Value = data[i].CompanyName },
                    new ExcelCellItem { Column = 12, Value = data[i].BranchName },
                    new ExcelCellItem { Column = 13, Value = data[i].Remark },
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

        public class CouponModel
        {
            //public string CompanyCode { get; set; }
            //public string InquiryNumber { get; set; }
            public string CoupunNumber { get; set; }
        }

        public class CouponDataModel
        {

            public string CompanyCode { get; set; }
            public int InquiryNumber { get; set; }
            public string CoupunNumber { get; set; }
            public string NamaProspek { get; set; }
            public string ProspekIdentityNo { get; set; }
            public string TelpRumah { get; set; }
            public string AlamatProspek { get; set; }
            public DateTime? TestDriveDate { get; set; }
            public string Email { get; set; }
            public string EmployeeID { get; set; }
            public string EmployeeName { get; set; }
            public string IdentityNo { get; set; }
            public string SalesID { get; set; }
            public string CompanyName { get; set; }
            public string BranchName { get; set; }
            public string Remark { get; set; }
            public bool ProcessFlag { get; set; }
        }

        public FileContentResult DownloadExcelFiles(string key)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=GenerateCoupon" + string.Format("{0}", DateTime.Now.ToString("ddMMyyyHHmmss")) + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }
    }
}