using SimDms.Absence.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using TracerX;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using SimDms.Common;

namespace SimDms.Absence.Controllers.Api
{
    public class InquiryController : BaseController
    {
        public JsonResult Employees()
        {
            string dept = Request["Department"] ?? "--";
            string post = Request["Position"];
            string status = Request["Status"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrInqEmployee";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@DeptCode", dept);
            cmd.Parameters.AddWithValue("@PosCode", post);
            cmd.Parameters.AddWithValue("@Status", status);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult EmployeeMutations()
        {
            string id = Request["EmployeeID"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrInqEmployeeMutation";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@EmployeeID", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult EmployeeAchievements()
        {
            string id = Request["EmployeeID"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrInqEmployeeAchievement";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@EmployeeID", id);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult EmployeeSubOrdinates()
        {
            string id = Request["EmployeeID"];
            string status = Request["Status"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrInqEmployeeSubOrdinates";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@EmployeeID", id);
            cmd.Parameters.AddWithValue("@Status", status);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult EmployeeInvalid()
        {
            string dept = Request["Department"] ?? "--";
            string post = Request["Position"];
            string status = Request["Status"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrInqEmployeeInvalid";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@DeptCode", dept);
            cmd.Parameters.AddWithValue("@PosCode", post);
            cmd.Parameters.AddWithValue("@Status", status);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult SfmPersInfo()
        {
            string dept = "SALES";
            string post = Request["Position"];
            string status = Request["Status"];
            string branch = Request["Branch"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrInqPersInfo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@DeptCode", dept);
            cmd.Parameters.AddWithValue("@PosCode", post);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@BranchCode", branch);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return Json(GetJson(dt));
        }

        public JsonResult SfmPersInfoXls()
        {
            string dept = "SALES";
            string post = Request["Position"];
            string status = Request["Status"];
            string branch = Request["Branch"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrInqPersInfoDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode );
            cmd.Parameters.AddWithValue("@DeptCode", dept);
            cmd.Parameters.AddWithValue("@PosCode", post);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@BranchCode", branch);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            //return Json(GetJson(dt));

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Dealer", ctx.OrganizationHdrs.Find(CompanyCode).CompanyName });
            header.Add(new List<dynamic> { "Cut Off", string.Format("{0}", DateTime.Now.ToString("dd-MMM-yyy HH:mm")) });

            return GenerateReportXls(dt, "Personal Info", "PersonalInfo", header);
        }

        public JsonResult SfmPersInfo2W()
        {
            string dept = "SALES";
            string post = Request["Position"];
            string status = Request["Status"];
            string branch = Request["Branch"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrInqPersInfo2W";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@DeptCode", dept);
            cmd.Parameters.AddWithValue("@PosCode", post);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@BranchCode", branch);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult SfmMutation()
        {
            string dept = "SALES";
            string date = Request["MutaDate"];
            string posi = Request["Position"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrInqMutation";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@DeptCode", dept);
            cmd.Parameters.AddWithValue("@Position", posi);
            cmd.Parameters.AddWithValue("@MutaDate", date);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult SfmTrend()
        {
            string dept = "SALES";
            string posi = Request["Position"] ?? "";
            string year = Request["Year"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrInqTrend";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@DeptCode", dept);
            cmd.Parameters.AddWithValue("@Position", posi);
            cmd.Parameters.AddWithValue("@Year", year);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult Attendance()
        {
            string companyCode = CompanyCode;
            string department = Request["Department"];
            string position = Request["Position"];
            string grade = Request["Grade"];
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string shiftCode = Request["ShiftCode"];
            string employeeID = Request["EmployeeID"];
            string employeeName = Request["EmployeeName"];

            var data = ctx.Database.SqlQuery<AttendanceDaily>("exec uspfn_abInqAttendanceDaily @CompanyCode=@p0, @Department=@p1, @Position=@p2, @Grade=@p3, @DateFrom=@p4, @DateTo=@p5, @ShiftCode=@p6, @EmployeeID=@p7, @EmployeeName=@p8", companyCode, department, position, grade, dateFrom, dateTo, shiftCode, employeeID, employeeName);

            return Json(data);
        }

        [HttpPost]
        public JsonResult AttendanceResume()
        {
            string companyCode = CompanyCode;
            string department = Request["Department"];
            string position = Request["Position"];
            string grade = Request["Grade"];
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string shiftCode = Request["ShiftCode"];
            string employeeID = Request["EmployeeID"];
            string employeeName = Request["EmployeeName"];

            var data = ctx.Database.SqlQuery<AttendanceResume>("exec uspfn_abInqAttendanceResume @CompanyCode=@p0, @Department=@p1, @Position=@p2, @Grade=@p3, @DateFrom=@p4, @DateTo=@p5, @ShiftCode=@p6, @EmployeeID=@p7, @EmployeeName=@p8", companyCode, department, position, grade, dateFrom, dateTo, shiftCode, employeeID, employeeName).FirstOrDefault();

            return Json(data);
        }

        public JsonResult AttendanceResumeDetails()
        {
            string companyCode = CompanyCode;
            string department = Request["Department"];
            string position = Request["Position"];
            string grade = Request["Grade"];
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string shiftCode = Request["ShiftCode"];
            string employeeID = Request["EmployeeID"];
            string employeeName = Request["EmployeeName"];
            string state = Request["State"];

            var data = ctx.Database.SqlQuery<AttendanceDaily>("exec uspfn_abInqAttendanceResumeDetails @CompanyCode=@p0, @Department=@p1, @Position=@p2, @Grade=@p3, @DateFrom=@p4, @DateTo=@p5, @ShiftCode=@p6, @EmployeeID=@p7, @EmployeeName=@p8, @State=@p9", companyCode, department, position, grade, dateFrom, dateTo, shiftCode, employeeID, employeeName, state).AsQueryable();

            return Json(data.KGrid());
        }

        #region Promotion Region

        public JsonResult Promotion()
        {
            string comp = Request["BranchCode"] ?? BranchCode;
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<PromotionViewModel>("exec uspfn_MpViewPromotion @BranchCode=@p0, @PeriodFrom=@p1, @PeriodTo=@p2", comp, DateFrom, DateTo).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PromotionTotal()
        {
            string comp = Request["BranchCode"] ?? BranchCode;
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<PromotionViewModel>("exec uspfn_MpViewPromotion @BranchCode=@p0, @PeriodFrom=@p1, @PeriodTo=@p2", comp, DateFrom, DateTo).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            var total = new PromotionViewModel();
            total.BM = data.Sum(x => x.BM);
            total.SH = data.Sum(x => x.SH);
            total.Platinum = data.Sum(x => x.Platinum);
            total.Gold = data.Sum(x => x.Gold);
            total.Silver = data.Sum(x => x.Silver);

            return Json(total, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PromotionDetail()
        {
            string comp = Request["CompanyCode"];// ?? "--";
            string bran = Request["BranchCode"] ?? BranchCode;
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];
            string Promosi = Request["PromosiType"];

            int to = 0; bool fa = true;
            switch (Promosi)
            {
                case "3": Promosi = "2"; break;
                case "4": Promosi = "3"; break;
                case "5": Promosi = "4"; break;
                case "6": Promosi = "SH"; break;
                case "7": Promosi = "BM"; break;
                default: break;
            }

            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<PromotionDetailViewModel>("exec uspfn_MpViewPromotionDetail @CompanyCode=@p0, @BranchCode=@p1, @PeriodFrom=@p2, @PeriodTo=@p3, @PromosiType=@p4", comp, bran, DateFrom, DateTo, Promosi).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PromotionExcel()
        {
            string comp = Request["BranchCode"] ?? BranchCode;
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];

            var message = "";
            try
            {
                var data = ctx.Database.SqlQuery<PromotionViewModel>("exec uspfn_MpViewPromotion @BranchCode=@p0, @PeriodFrom=@p1, @PeriodTo=@p2", comp, DateFrom, DateTo).ToList();
                //return Json(data);
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");

                comp = comp == "" ? comp = "All Dealer" : ctx.GnMstDealerMappings.FirstOrDefault(x => x.DealerCode == comp).DealerName;
                package = GEPromotionExcel(package, data, comp, DateFrom, DateTo);

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

        private static ExcelPackage GEPromotionExcel(ExcelPackage package, List<PromotionViewModel> data, string comp, string DateFrom, string DateTo)
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
                w1500 = GetTrueColWidth(15.00),
                w1800 = GetTrueColWidth(18.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Promotion Data";
            sheet.Cells[x].Style.Font.Size = 20;

            x = GetCell(rTitle + 2, cStart);
            sheet.Cells[x].Value = "Dealer";
            sheet.Cells[x].Style.Font.Bold = true;
            x = GetCell(rTitle + 2, cStart + 1);
            sheet.Cells[x].Value = comp;

            x = GetCell(rTitle + 3, cStart);
            sheet.Cells[x].Value = "Period";
            sheet.Cells[x].Style.Font.Bold = true;
            x = GetCell(rTitle + 3, cStart + 1);
            sheet.Cells[x].Value = DateTime.Parse(DateFrom).ToString("dd MMM yyyy");
            x = GetCell(rTitle + 3, cStart + 2);
            sheet.Cells[x].Value = "to";
            sheet.Cells[x].Style.Font.Bold = true;
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(rTitle + 3, cStart + 3);
            sheet.Cells[x].Value = DateTime.Parse(DateTo).ToString("dd MMM yyyy");

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w3000, Value = "Outlet" },
                new ExcelCellItem { Column = 2, Width = w1500, Value = "Trainee/to/Silver".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 3, Width = w1500, Value = "Silver/to/Gold".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 4, Width = w1500, Value = "Gold/to/Platinum".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 5, Width = w1500, Value = "Sales Person/to/Sales Head".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 6, Width = w1500, Value = "Sales Head/to/Branch Manager".Replace("/", System.Environment.NewLine) },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Value = header.Value;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.Font.Bold = true;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
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
                    new ExcelCellItem { Column = 1, Value = data[i].Outlet },
                    new ExcelCellItem { Column = 2, Value = data[i].Silver },
                    new ExcelCellItem { Column = 3, Value = data[i].Gold },
                    new ExcelCellItem { Column = 4, Value = data[i].Platinum },
                    new ExcelCellItem { Column = 5, Value = data[i].SH },
                    new ExcelCellItem { Column = 6, Value = data[i].BM },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            //TABLE FOOTER
            var rFooter = rDataStart + data.Count();

            var footers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Value = "TOTAL" },
                new ExcelCellItem { Column = 2, Value = data.Sum(o => o.Silver) },
                new ExcelCellItem { Column = 3, Value = data.Sum(o => o.Gold) },
                new ExcelCellItem { Column = 4, Value = data.Sum(o => o.Platinum) },
                new ExcelCellItem { Column = 5, Value = data.Sum(o => o.SH) },
                new ExcelCellItem { Column = 6, Value = data.Sum(o => o.BM) },
            };

            foreach (var footer in footers)
            {
                x = GetCell(rFooter, footer.Column);
                sheet.Cells[x].Value = footer.Value;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            x = GetCell(rFooter, 1);
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            return package;
        }

        public class PromotionViewModel
        {
            public string OutletCode { get; set; }
            public string Outlet { get; set; }
            public int? BM { get; set; }
            public int? SH { get; set; }
            public int? Platinum { get; set; }
            public int? Gold { get; set; }
            public int? Silver { get; set; }
        }

        public class PromotionDetailViewModel
        {
            public string OutletName { get; set; }
            public string Name { get; set; }
            public string Position { get; set; }
            public string Grade { get; set; }
            public DateTime? JoinDate { get; set; }
        }
        #endregion

        #region Demotion Region

        public JsonResult Demotion()
        {
            string comp = Request["BranchCode"] ?? BranchCode;
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<DemotionViewModel>("exec uspfn_MpViewDemotion @BranchCode=@p0, @PeriodFrom=@p1, @PeriodTo=@p2", comp, DateFrom, DateTo).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DemotionTotal()
        {
            string comp = Request["BranchCode"] ?? BranchCode;
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<DemotionViewModel>("exec uspfn_MpViewDemotion @BranchCode=@p0, @PeriodFrom=@p1, @PeriodTo=@p2", comp, DateFrom, DateTo).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            var total = new DemotionViewModel();
            total.SH = data.Sum(x => x.SH);
            total.SC = data.Sum(x => x.SC);
            total.Gold = data.Sum(x => x.Gold);
            total.Silver = data.Sum(x => x.Silver);

            return Json(total, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DemotionDetail()
        {
            string comp = Request["CompanyCode"];// ?? "--";
            string bran = Request["BranchCode"] ?? BranchCode;
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];
            string Demosi = Request["DemosiType"];

            int to = 0; bool fa = true;
            switch (Demosi)
            {
                case "3": Demosi = "SH"; break;
                case "4": Demosi = "SC"; break;
                case "5": Demosi = "3"; break;
                case "6": Demosi = "2"; break;
                default: break;
            }

            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<DemotionDetailViewModel>("exec uspfn_MpViewDemotionDetail @CompanyCode=@p0, @BranchCode=@p1, @PeriodFrom=@p2, @PeriodTo=@p3, @DemosiType=@p4", comp, bran, DateFrom, DateTo, Demosi).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DemotionExcel()
        {
            string comp = Request["BranchCode"] ?? BranchCode;
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"];

            var message = "";
            try
            {
                var data = ctx.Database.SqlQuery<DemotionViewModel>("exec uspfn_MpViewDemotion @BranchCode=@p0, @PeriodFrom=@p1, @PeriodTo=@p2", comp, DateFrom, DateTo).ToList();
                //return Json(data);
                var package = new ExcelPackage();
                if (data == null) throw new Exception("Lookup Detail belum di-set di database. Harap hubungi IT Support");

                comp = comp == "" ? comp = "All Dealer" : ctx.GnMstDealerMappings.FirstOrDefault(x => x.DealerCode == comp).DealerName;
                package = GEDemotionExcel(package, data, comp, DateFrom, DateTo);

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

        private static ExcelPackage GEDemotionExcel(ExcelPackage package, List<DemotionViewModel> data, string comp, string DateFrom, string DateTo)
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
                w1500 = GetTrueColWidth(15.00),
                w1800 = GetTrueColWidth(18.00),
                w2000 = GetTrueColWidth(20.00),
                w3000 = GetTrueColWidth(30.00);

            double cWidth = GetTrueColWidth(14.00);
            #endregion

            var sheet = package.Workbook.Worksheets.Add("book1");

            //TITLE
            #region -- Title & Header --
            z = GetRange(rTitle, cStart, rTitle, cTitleVal);
            sheet.Cells[z].Merge = true;

            x = GetCell(rTitle, cStart);
            sheet.Cells[x].Value = "Report Demotion Data";
            sheet.Cells[x].Style.Font.Size = 20;

            x = GetCell(rTitle + 2, cStart);
            sheet.Cells[x].Value = "Dealer";
            sheet.Cells[x].Style.Font.Bold = true;
            x = GetCell(rTitle + 2, cStart + 1);
            sheet.Cells[x].Value = comp;

            x = GetCell(rTitle + 3, cStart);
            sheet.Cells[x].Value = "Period";
            sheet.Cells[x].Style.Font.Bold = true;
            x = GetCell(rTitle + 3, cStart + 1);
            sheet.Cells[x].Value = DateTime.Parse(DateFrom).ToString("dd MMM yyyy");
            x = GetCell(rTitle + 3, cStart + 2);
            sheet.Cells[x].Value = "to";
            sheet.Cells[x].Style.Font.Bold = true;
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            x = GetCell(rTitle + 3, cStart + 3);
            sheet.Cells[x].Value = DateTime.Parse(DateTo).ToString("dd MMM yyyy");

            //TABLE HEADER
            var headers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Width = w3000, Value = "Outlet" },
                new ExcelCellItem { Column = 2, Width = w1500, Value = "Branch Manager/to/Sales Head".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 3, Width = w1500, Value = "Sales Head/to/Sales Person".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 4, Width = w1500, Value = "Platinum/to/Gold".Replace("/", System.Environment.NewLine) },
                new ExcelCellItem { Column = 5, Width = w1500, Value = "Gold/to/Silver".Replace("/", System.Environment.NewLine) },
            };

            foreach (var header in headers)
            {
                x = GetCell(rHdrFirst, header.Column);
                sheet.Column(header.Column).Width = header.Width;
                sheet.Cells[x].Value = header.Value;
                sheet.Cells[x].Style.WrapText = true;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(219, 229, 241));
                sheet.Cells[x].Style.Font.Bold = true;
                sheet.Cells[x].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
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
                    new ExcelCellItem { Column = 1, Value = data[i].Outlet },
                    new ExcelCellItem { Column = 2, Value = data[i].SH },
                    new ExcelCellItem { Column = 3, Value = data[i].SC },
                    new ExcelCellItem { Column = 4, Value = data[i].Gold },
                    new ExcelCellItem { Column = 5, Value = data[i].Silver },
                };

                foreach (var item in items)
                {
                    x = GetCell(row, item.Column);
                    sheet.Cells[x].Value = item.Value;
                    sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    if (item.Format != null) sheet.Cells[x].Style.Numberformat.Format = item.Format;
                }
            }

            //TABLE FOOTER
            var rFooter = rDataStart + data.Count();

            var footers = new List<ExcelCellItem>
            {
                new ExcelCellItem { Column = 1, Value = "TOTAL" },
                new ExcelCellItem { Column = 2, Value = data.Sum(o => o.SH) },
                new ExcelCellItem { Column = 3, Value = data.Sum(o => o.SC) },
                new ExcelCellItem { Column = 4, Value = data.Sum(o => o.Gold) },
                new ExcelCellItem { Column = 5, Value = data.Sum(o => o.Silver) },
            };

            foreach (var footer in footers)
            {
                x = GetCell(rFooter, footer.Column);
                sheet.Cells[x].Value = footer.Value;
                sheet.Cells[x].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                sheet.Cells[x].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[x].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            }

            x = GetCell(rFooter, 1);
            sheet.Cells[x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            return package;
        }

        public class DemotionViewModel
        {
            public string OutletCode { get; set; }
            public string Outlet { get; set; }
            public int? SH { get; set; }
            public int? SC { get; set; }
            public int? Gold { get; set; }
            public int? Silver { get; set; }
        }

        public class DemotionDetailViewModel : PromotionDetailViewModel { }

        #endregion

        #region Excel Package methods 
        
        public FileContentResult DownloadExcelFile(string key, string filename)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + "_" + string.Format("{0}", DateTime.Now.ToString("dd-MMM-yyy HH.mm")) + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        internal static string GetColName(int number)
        {
            return EPPlusHelper.ExcelColumnNameFromNumber(number);
        }

        internal static int GetColNumber(string name)
        {
            return EPPlusHelper.ExcelColumnNameToNumber(name);
        }

        internal static string GetCell(int row, int col)
        {
            return GetColName(col) + row.ToString();
        }

        internal static string GetRange(int row1, int col1, int row2, int col2)
        {
            return GetCell(row1, col1) + ":" + GetCell(row2, col2);
        }

        internal class ExcelCellItem
        {
            public int Column { get; set; }
            public double Width { get; set; }
            public object Value { get; set; }
            public string Format { get; set; }
        }

        internal static double GetTrueColWidth(double width)
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

        #endregion
    }
}
