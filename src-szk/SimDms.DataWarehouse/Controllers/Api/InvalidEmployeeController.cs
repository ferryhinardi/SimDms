using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;
using EP = SimDms.DataWarehouse.Helpers.EPPlusHelper;
using SimDms.DataWarehouse.Helpers;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class InvalidEmployeeController : BaseController
    {
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
                var dealers = ctx.GnMstDealerMappings.Where(x => x.GroupNo == groupCode).Select(x => new
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
                              select new
                              {
                                  value = b.OutletCode,
                                  text = b.OutletAbbreviation
                              };
                return Json(outlets);
            }
            return Json(new { });
        }

        public JsonResult Cases()
        {
            var cases = from a in ctx.GnMstLookUpDtls
                        where a.CompanyCode == CompanyCode && a.CodeID == "IVLD"
                        orderby a.LookUpValue
                        select new
                        {
                            value = a.LookUpValue,
                            text = a.LookUpValueName
                        };
            return Json(cases);
        }

        public JsonResult PersonnelStatus()
        {
            var statuses = from a in ctx.GnMstLookUpDtls
                           where a.CompanyCode == CompanyCode && a.CodeID == "PERS"
                           orderby a.LookUpValue
                           select new
                           {
                               value = a.LookUpValue,
                               text = a.LookUpValueName
                           };
            return Json(statuses);
        }

        public JsonResult Query(string dealer, string outlet, string status, string caseNo)
        {
            var message = "";

            try
            {
                var companyName = ctx.GnMstDealerMappings.FirstOrDefault(x => 
                    x.DealerCode == (dealer == "" ? x.DealerCode : dealer)).DealerName;
                outlet = outlet == null ? "" : outlet;
                var outletAbv = outlet != "" ? ctx.GnMstDealerOutletMappings.FirstOrDefault(x => 
                    x.DealerCode == (dealer == "" ? x.DealerCode : dealer)
                    && x.OutletCode == outlet).OutletAbbreviation : "";
                
                var query = "exec usprpt_abInqInvalidEmployee @p0, @p1, @p2, @p3";

                var issueRows = ctx.GnMstLookUpDtls.Where(x => x.CompanyCode == "0000000" && x.CodeID == "IVLD").ToList();
                if (issueRows.Count() == 0) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");

                var package = new ExcelPackage();

                if (caseNo != "")
                {
                    var result = ctx.Database.SqlQuery<InvalidEmployeeData>(query, dealer, outlet, status, caseNo).ToList();
                    var issueRow = issueRows.FirstOrDefault(x => x.LookUpValue == caseNo);
                    if (issueRow == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                    var model = new InvalidEmployeeModel
                    {
                        CaseNo = issueRow.LookUpValue,
                        Case = issueRow.LookUpValueName,
                        DealerName = companyName,
                        OutletCode = outlet,
                        OutletAbv = outletAbv
                    };
                    package = GenerateExcel(package, model, result);
                    
                }
                else //SELECT ALL CASE
                {
                    foreach (var issueRow in issueRows)
                    {
                        var result = ctx.Database.SqlQuery<InvalidEmployeeData>(query, dealer, outlet, status, issueRow.LookUpValue).ToList();
                        var model = new InvalidEmployeeModel
                        {
                            CaseNo = issueRow.LookUpValue,
                            Case = issueRow.LookUpValueName,
                            DealerName = "ALL"
                        };
                        package = GenerateExcel(package, model, result);
                    }
                }

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

        public JsonResult QueryNew(string groupArea, string companyCode, string outlet, string status, string caseNo)
        {
            var message = "";

            try
            {
                var companyName = ctx.GnMstDealerMappings.FirstOrDefault(x =>
                    x.DealerCode == (ParamDealerCode == "" ? x.DealerCode : ParamDealerCode)).DealerName;
                outlet = outlet == null ? "" : outlet;

                var outletAbv = outlet != "" ? ctx.GnMstDealerOutletMappings.FirstOrDefault(x =>
                    x.DealerCode == (ParamDealerCode == "" ? x.DealerCode : ParamDealerCode)
                    && x.OutletCode == outlet).OutletAbbreviation : "";

                var query = "exec usprpt_abInqInvalidEmployeeNew @p0, @p1, @p2, @p3, @p4, @p5";

                var issueRows = ctx.GnMstLookUpDtls.Where(x => x.CompanyCode == "0000000" && x.CodeID == "IVLD").ToList();
                if (issueRows.Count() == 0) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");

                var package = new ExcelPackage();

                if (caseNo != "")
                {
                    var result = ctx.Database.SqlQuery<InvalidEmployeeData>(query, groupArea, ParamDealerCode, outlet, status, caseNo, ParamGroupNoNew).ToList();
                    var issueRow = issueRows.FirstOrDefault(x => x.LookUpValue == caseNo);
                    if (issueRow == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");
                    var model = new InvalidEmployeeModel
                    {
                        CaseNo = issueRow.LookUpValue,
                        Case = issueRow.LookUpValueName,
                        DealerName = companyName,
                        OutletCode = outlet,
                        OutletAbv = outletAbv
                    };
                    package = GenerateExcel(package, model, result);

                }
                else //SELECT ALL CASE
                {
                    foreach (var issueRow in issueRows)
                    {
                        var result = ctx.Database.SqlQuery<InvalidEmployeeData>(query, groupArea, ParamDealerCode, outlet, status, issueRow.LookUpValue, ParamGroupNoNew).ToList();
                        var model = new InvalidEmployeeModel
                        {
                            CaseNo = issueRow.LookUpValue,
                            Case = issueRow.LookUpValueName,
                            DealerName = "ALL"
                        };
                        package = GenerateExcel(package, model, result);
                    }
                }

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

        public FileContentResult DownloadExcelFile(string key)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=InvalidEmployeeData.xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        private static ExcelPackage GenerateExcel(ExcelPackage package, InvalidEmployeeModel model, List<InvalidEmployeeData> data)
        {
            #region -- Constants --
            var ctx = new DataContext();
            // for convenient addressing
            // z = range, x = cell
            string z = "", x = "";
            const int
                rTitle = 1,
                rDealer = 2,
                rCase = 3,
                rHdrFirst = 6,
                rDataStart = 7,

                cStart = 1,
                cTitleVal = 2,

                cEmpId = 1,
                cEmpName = 2,
                cOutletName = 3,
                cDept = 4,
                cPos = 5,
                cGrade = 6,
                cJoin = 7,
                cMut = 8,
                cAssign = 9,
                cResign = 10,
                cStatus = 11;

            double
                wEmpId = EP.GetTrueColWidth(10.86),
                wEmpName = EP.GetTrueColWidth(42.14),
                wOutletName = EP.GetTrueColWidth(30.00),
                wDept = EP.GetTrueColWidth(8.43),
                wPos = EP.GetTrueColWidth(8.43),
                wGrade = EP.GetTrueColWidth(8.43),
                wJoin = EP.GetTrueColWidth(13.00),
                wMut = EP.GetTrueColWidth(13.00),
                wAssign = EP.GetTrueColWidth(13.00),
                wResign = EP.GetTrueColWidth(13.00),
                wStatus = EP.GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add(model.Case);
            
            //TITLE
            #region -- Title & Header --
            z = EP.GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = EP.GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Invalid Employee Data";
            sheet.Cells[x].Style.Font.Size = 20;

            x = EP.GetCell(rDealer, cStart);
            sheet.Cells[x].Value = "Dealer :";

            x = EP.GetCell(rDealer, cTitleVal);
            sheet.Cells[x].Value = model.DealerName;

            x = EP.GetCell(rCase, cStart);
            sheet.Cells[x].Value = "Case :";

            x = EP.GetCell(rCase, cTitleVal);
            sheet.Cells[x].Value = model.Case;

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = cEmpId, Width = wEmpId, Value = "NIK" },
                new ExcelCellItem { Column = cEmpName, Width = wEmpName, Value = "Nama" },
                new ExcelCellItem { Column = cOutletName, Width = wOutletName, Value = "Outlet" },
                new ExcelCellItem { Column = cDept, Width = wDept, Value = "Dept" },
                new ExcelCellItem { Column = cPos, Width = wPos, Value = "Position" },
                new ExcelCellItem { Column = cGrade, Width = wGrade, Value = "Grade" },
                new ExcelCellItem { Column = cJoin, Width = wJoin, Value = "Join Date" },
                new ExcelCellItem { Column = cMut, Width = wMut, Value = "Mutation Date" },
                new ExcelCellItem { Column = cAssign, Width = wAssign, Value = "Assign Date" },
                new ExcelCellItem { Column = cResign, Width = wResign, Value = "Resign Date" },
                new ExcelCellItem { Column = cStatus, Width = wStatus, Value = "Status" },
            };

            foreach (var header in headers)
            {
                x = EP.GetCell(rHdrFirst, header.Column);
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
                    new ExcelCellItem { Column = cEmpId, Value = data[i].EmployeeID },
                    new ExcelCellItem { Column = cEmpName, Value = data[i].EmployeeName },
                    new ExcelCellItem { Column = cOutletName, Value = data[i].OutletAbbreviation },
                    new ExcelCellItem { Column = cDept, Value = data[i].Department },
                    new ExcelCellItem { Column = cPos, Value = data[i].Position },
                    new ExcelCellItem { Column = cGrade, Value = data[i].Grade },
                    new ExcelCellItem { Column = cJoin, Value = data[i].JoinDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = cMut, Value = data[i].MutationDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = cAssign, Value = data[i].AssignDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = cResign, Value = data[i].ResignDate, Format = "dd-MMM-YYYY" },
                    new ExcelCellItem { Column = cStatus, Value = data[i].PersonnelStatus },
                };

                foreach (var item in items)
                {
                    x = EP.GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            return package;

        }

        private class InvalidEmployeeData
        {
            public String CompanyCode { get; set; }
            public String BranchCode { get; set; }
            public String OutletAbbreviation { get; set; }
            public String EmployeeID { get; set; }
            public String EmployeeName { get; set; }
            public String Department { get; set; }
            public String Position { get; set; }
            public String Grade { get; set; }
            public DateTime? JoinDate { get; set; }
            public DateTime? MutationDate { get; set; }
            public DateTime? AssignDate { get; set; }
            public DateTime? ResignDate { get; set; }
            public String PersonnelStatus { get; set; }
        }

        private class InvalidEmployeeModel
        {
            public string DealerName { get; set; }
            public string OutletCode { get; set; }
            public string OutletAbv { get; set; }
            public string Case { get; set; }
            public string CaseNo { get; set; }
        }
    }
}