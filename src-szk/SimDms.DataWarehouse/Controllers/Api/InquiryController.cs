using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using GeLang;
using SimDms.DataWarehouse.Models;
using System.Data.Entity.Core.Objects;
using ClosedXML.Excel;
using SimDms.DataWarehouse.Helpers;
using System.Text;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class InquiryController : BaseController
    {
        enum ExcelCellStyle
        {
            Default,
            Header,
            Header2,
            Standard,
            CenterBorderedBold,
            RightBorderedStandard,
            RightBorderedStandardNumber,
            RightBorderedStandardDecimal,
            RightBorderedStandardRedNumber,
            RightBorderedBold,
            RightBorderedBoldRed,
            RightBorderedBoldNumber,
            RightBorderedBoldDecimal,
            RightBorderedBoldRedNumber,
            LeftBold,
            LeftBorderedBold,
            LeftBorderedTop,
            LeftBorderedStandardWrap,
            LeftBorderedStandardDate,
            LeftBorderedStandard,
            BorderedStandard,
            PinkTotal,
            PinkTotalNumber,
            PinkTotalRedNumber,
            PinkTotalDecimal,
            BrownTotal,
            BrownTotalNumber,
            BrownTotalDecimal,
            BrownTotalRedNumber,
            LightBrownTotal,
            LightBrownTotalNumber,
            LightBrownTotalDecimal,
            LightBrownTotalRedNumber,
            BlueTotal,
            BlueTotalNumber,
            BlueTotalDecimal,
            BlueTotalRedNumber,
            YellowTotal,
            YellowTotalNumber,
            YellowTotalDecimal,
            YellowTotalRedNumber,
            PurpleTotal,
            PurpleTotalNumber,
            PurpleTotalDecimal,
            PurpleTotalRedNumber,
            GreenTotal,
            GreenTotalNumber,
            GreenTotalDecimal,
            GreenTotalRedNumber,
            GrayTotal,
            GrayTotalNumber,
            GrayTotalDecimal,
            GrayTotalRedNumber,
            LightGrayTotal,
            LightGrayTotalNumber,
            LightGrayTotalDecimal,
            LightGrayTotalRedNumber,
            None
        };

        public JsonResult Employees()
        {
            string area = Request["GroupArea"]; //?? "--";
            string comp = Request["CompanyCode"]; //?? "--";
            string dept = Request["Department"]; //?? "--";
            string post = Request["Position"];
            string status = Request["Status"];

            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandTimeout = 3600;
            //cmd.CommandText = "uspfn_HrInqEmployee";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@GroupNo", area);
            //cmd.Parameters.AddWithValue("@CompanyCode", comp);
            //cmd.Parameters.AddWithValue("@DeptCode", dept);
            //cmd.Parameters.AddWithValue("@PosCode", post);
            //cmd.Parameters.AddWithValue("@Status", status);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //da.Fill(dt);

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<HrInqEmployee>("exec uspfn_HrInqEmployee @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @PosCode=@p3, @Status=@p4", area, comp, dept, post, status).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmployeesNew()
        {
            string groupNoNew = ParamGroupNoNew;
            string area = Request["GroupArea"]; //?? "--";
            string comp = ParamDealerCode; // Request["CompanyCode"]; //?? "--";
            string dept = Request["Department"]; //?? "--";
            string post = Request["Position"];
            string status = Request["Status"];

            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandTimeout = 3600;
            //cmd.CommandText = "uspfn_HrInqEmployee";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@GroupNo", area);
            //cmd.Parameters.AddWithValue("@CompanyCode", comp);
            //cmd.Parameters.AddWithValue("@DeptCode", dept);
            //cmd.Parameters.AddWithValue("@PosCode", post);
            //cmd.Parameters.AddWithValue("@Status", status);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //da.Fill(dt);

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<HrInqEmployee>("exec uspfn_HrInqEmployeeNew @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @PosCode=@p3, @Status=@p4, @GroupNoNew=@p5", area, comp, dept, post, status, groupNoNew).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult EmployeeSubOrdinates()
        {
            string comp = ParamDealerCode;//Request["CompanyCode"] ?? "--";
            string empl = Request["EmployeeID"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_HrInqEmployeeSubOrdinates";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@EmployeeID", empl);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult EmployeeAchievements()
        {
            string comp = ParamDealerCode;//Request["CompanyCode"] ?? "--";
            string empl = Request["EmployeeID"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_HrInqEmployeeAchievement";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@EmployeeID", empl);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult EmployeeMutations()
        {
            string comp = ParamDealerCode;//Request["CompanyCode"] ?? "--";
            string empl = Request["EmployeeID"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_HrInqEmployeeMutation";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@EmployeeID", empl);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult EmployeeInvalid()
        {
            string comp = Request["CompanyCode"] ?? "--";
            string dept = Request["Department"] ?? "--";
            string post = Request["Position"];
            string status = Request["Status"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_HrInqEmployeeInvalid";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@DeptCode", dept);
            cmd.Parameters.AddWithValue("@PosCode", post);
            cmd.Parameters.AddWithValue("@Status", status);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult DealerInfo()
        {
            string ptype = Request["ProductType"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_InqDealerInfo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("ProductType", ptype);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult SfmPersInfo()
        {
            string dept = "SALES";
            string area = Request["GroupArea"];
            string comp = Request["CompanyCode"] ?? "--";
            string post = Request["Position"];
            string status = Request["Status"];
            string branch = Request["Branch"];
            /*
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_HrInqPersInfo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@DeptCode", dept);
            cmd.Parameters.AddWithValue("@PosCode", post);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@BranchCode", branch);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
             * */

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<HrInqPersInfo>("exec uspfn_HrInqPersInfo @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @PosCode=@p3, @Status=@p4,   @BranchCode=@p5", area, comp, dept, post, status, branch).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SfmPersInfoNew()
        {
            string dept = "SALES";
            string area = Request["GroupArea"];
            string comp = ParamDealerCode; //Request["CompanyCode"] ?? "--";
            string post = Request["Position"];
            string status = Request["Status"];
            string branch = Request["Branch"];
            string groupNoNew = ParamGroupNoNew;
            /*
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_HrInqPersInfo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@DeptCode", dept);
            cmd.Parameters.AddWithValue("@PosCode", post);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@BranchCode", branch);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
             * */

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<HrInqPersInfo>("exec uspfn_HrInqPersInfoNew @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @PosCode=@p3, @Status=@p4, @BranchCode=@p5, @GroupNoNew=@p6",
                area, comp, dept, post, status, branch, ParamGroupNoNew).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SfmPersInfoXls()
        {
            string dept = "SALES";
            string comp = Request["CompanyCode"] ?? "--";
            string post = Request["Position"];
            string status = Request["Status"];
            string branch = Request["Branch"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_HrInqPersInfoDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@DeptCode", dept);
            cmd.Parameters.AddWithValue("@PosCode", post);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@BranchCode", branch);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            //return Json(GetJson(dt));

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Dealer", (comp == "") ? "ALL" : ctx.DealerInfos.Find(new string[] { comp }).DealerName });
            header.Add(new List<dynamic> { "Cut Off", string.Format("{0}", DateTime.Now.ToString("dd-MMM-yyy HH:mm")) });

            return GenerateReportXls(dt, "Personal Info", "PersonalInfo", header);
        }

        public JsonResult SfmPersList()
        {
            string comp = Request["CompanyCode"] ?? "--";
            string post = Request["Position"];
            string status = Request["Status"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_WhInqPersInfo";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@Position", post);
            cmd.Parameters.AddWithValue("@Status", status);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult SfmPersInfoCutOff()
        {
            string dept = "SALES";
            string area = Request["GroupArea"];
            string comp = Request["CompanyCode"] ?? "--";
            string post = Request["Position"];
            string status = Request["Status"];
            string branch = Request["Branch"];
            string cutoff = Convert.ToDateTime(Request["CutOff"]).ToShortDateString();

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<HrInqPersInfoDetail>("exec uspfn_HrInqPersInfoCutOff @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @PosCode=@p3, @Status=@p4,   @BranchCode=@p5, @CutOff=@p6", area, comp, dept, post, status, branch, cutoff).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SfmPersInfoCutOffNew()
        {
            string dept = "SALES";
            string area = Request["GroupArea"];
            string comp = ParamDealerCode; //Request["CompanyCode"] ?? "--";
            string post = Request["Position"];
            string status = Request["Status"];
            string branch = Request["Branch"];
            string cutoff = Convert.ToDateTime(Request["CutOff"]).ToShortDateString();
            string groupNoNew = ParamGroupNoNew;

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<HrInqPersInfoDetail>("exec uspfn_HrInqPersInfoCutOffNew @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @PosCode=@p3, @Status=@p4, @BranchCode=@p5, @CutOff=@p6, @GroupNoNew=@p7",
                area, comp, dept, post, status, branch, cutoff, groupNoNew).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SfmMutation()
        {
            string area = Request["GroupArea"];// ?? "--";
            string comp = Request["CompanyCode"];// ?? "--";
            string dept = "SALES";
            string posi = Request["Position"] ?? "";
            string date = Request["MutaDate"];

            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_HrInqMutation";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", comp);
            //cmd.Parameters.AddWithValue("@DeptCode", dept);
            //cmd.Parameters.AddWithValue("@Position", posi);
            //cmd.Parameters.AddWithValue("@MutaDate", date);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //da.Fill(dt);

            //return Json(GetJson(dt));

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            //var data = ctx.Database.SqlQuery<HrInqMutation>("exec uspfn_HrInqMutation @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @Position=@p3, @MutaDate=@p4", area, comp, dept, posi, date).AsQueryable();
            var data = ctx.Database.SqlQuery<HrInqMutation>("exec uspfn_HrInqMutation_New @GroupNo=@p0, @Company=@p1, @DeptCode=@p2, @Position=@p3, @MutaDate=@p4", area, comp, dept, posi, date).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SfmTrend()
        {
            string area = Request["GroupArea"];// ?? "--";
            string comp = Request["CompanyCode"];// ?? "--";
            string dept = "SALES";
            string posi = Request["Position"] ?? "";
            string year = Request["Year"];

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            //var data = ctx.Database.SqlQuery<HrInqTrend>("exec uspfn_HrInqTrend @GroupNo=@p0, @CompanyCode=@p1, @DeptCode=@p2, @Position=@p3, @Year=@p4", area, comp, dept, posi, (year == "" ? DateTime.Now.Year.ToString() : year)).AsQueryable();
            var data = ctx.Database.SqlQuery<HrInqTrend>("exec uspfn_HrInqTrend_New @GroupNo=@p0, @Company=@p1, @DeptCode=@p2, @Position=@p3, @Year=@p4", area, comp, dept, posi, (year == "" ? DateTime.Now.Year.ToString() : year)).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SvMsi()
        {
            string comp = Request["CompanyCode"] ?? "--";
            string bran = Request["BranchCode"] ?? "--";
            string year = string.IsNullOrWhiteSpace(Request["Year"]) ? "2100" : Request["Year"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_WhInqMsi";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@BranchCode", bran);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        //start fhi 27-03-2015 : add SvMsiV2
        public ActionResult isNational()
        {
            string comp = CurrentUser.DealerCode ?? "";
            //string comp = "6006400001";
            string brcd = Request["BranchCode"] ?? "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_isNational";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@BranchCode", brcd);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();
            da.Fill(ds);

            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];

            var isnat = dt1.Rows[0][0].ToString();
            var area = dt2.Rows[0][1].ToString();
            var dealerCd = dt2.Rows[0][2].ToString();
            var dealerNm = dt2.Rows[0][2].ToString() + " - " + dt2.Rows[0][3].ToString();
            var outletCd = dt2.Rows[0][4].ToString();
            var outletNm = dt2.Rows[0][4].ToString() + " - " + dt2.Rows[0][5].ToString();

            return Json(new { isNational = isnat, area = area, dealerCd = dealerCd, dealerNm = dealerNm, outletCd = outletCd, outletNm = outletNm });
        }

        public JsonResult SvMsiV2()
        {
            string area = Request["Area"] ?? "";
            string comp = Request["CompanyCode"] ?? "";
            string bran = Request["BranchCode"] ?? "";
            string crYr = Convert.ToString(DateTime.Today.Year);
            string year = string.IsNullOrWhiteSpace(Request["Year"]) ? crYr : Request["Year"];
            string dataSource = Request["DataSource"] ?? "";
            string sp = "";
            if (dataSource == "Invoice")
            {
                sp = "uspfn_WhInqMsiV2";
            }
            else
            {
                sp = "uspfn_WhInqMsiV2_SPK";
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = sp;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 360;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Area", area);
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@BranchCode", bran);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            da.Fill(ds);

            if (area == "" || comp == "" || bran == "")
            {
                dt = ds.Tables[1];
            }
            else
            {
                dt = ds.Tables[0];
            }
            return Json(GetJson(dt));
        }

        public ActionResult exportExcel(string Area, string Dealer, string Outlet, string SpID, string Year, string TextArea, string TextDealer, string TextOutlet, string DataSource)
        {
            string fileName = "";
            fileName = "Inq_Suzuki_MSI" + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            string area = Area ?? "";
            string dealer = Dealer ?? "";
            string outlet = Outlet ?? "";

            string crYr = Convert.ToString(DateTime.Today.Year);
            string year = string.IsNullOrWhiteSpace(Request["Year"]) ? crYr : Request["Year"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "EXEC " + SpID + " '" + area + "','" + dealer + "','" + outlet + "','" + year + "'";
            cmd.CommandTimeout = 360;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            var wb = new XLWorkbook();

            if (area != "" && dealer != "" && outlet != "")
            {
                dt1 = ds.Tables[0];
                if (dt1.Rows.Count == 0)
                {
                    return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
                }
                #region ** format Excell  By Branch **
                else
                {
                    int lastRow = 8;
                    int index = 0;
                    int indexRow = 0;

                    var msiGroupNameCheck = "";
                    var msiGroupName = "";

                    var sheetName = ds.Tables[0].Rows[0][1].ToString();
                    var ws = wb.Worksheets.Add(sheetName);

                    #region ** write header **
                    var hdrTable = ws.Range("A1:U7");
                    hdrTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    var rngTable = ws.Range("A7:U7");
                    rngTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                        .Alignment.SetWrapText();

                    rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTable.Style.Font.Bold = true;

                    ws.Columns("1").Width = 40;
                    ws.Columns("2").Width = 5;
                    ws.Columns("3").Width = 90;
                    ws.Columns("4").Width = 10;
                    ws.Columns("5").Width = 20;
                    ws.Columns("6").Width = 20;
                    ws.Columns("7").Width = 15;
                    ws.Columns("8").Width = 15;
                    ws.Columns("9").Width = 15;
                    ws.Columns("10").Width = 15;
                    ws.Columns("11").Width = 15;
                    ws.Columns("12").Width = 15;
                    ws.Columns("13").Width = 15;
                    ws.Columns("14").Width = 15;
                    ws.Columns("15").Width = 15;
                    ws.Columns("16").Width = 15;
                    ws.Columns("17").Width = 15;
                    ws.Columns("18").Width = 15;

                    //First Names   
                    ws.Cell("A1").Value = "Inquiry Suzuki MSI V2 by " + DataSource;
                    ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                    ws.Cell("A2").Value = "Year";
                    ws.Cell("D2").Value = "Date";
                    ws.Cell("A3").Value = "Area";
                    ws.Cell("A4").Value = "Dealer";
                    ws.Cell("A5").Value = "Outlet";

                    ws.Cell("B2").Value = Year;
                    ws.Cell("E2").Value = DateTime.Now.ToString();
                    ws.Cell("E2").Style.DateFormat.Format = "DD-MMM-YYYY HH:mm";
                    ws.Cell("B3").Value = area;
                    ws.Cell("B4").Value = TextDealer;
                    ws.Cell("B5").Value = TextOutlet;

                    ws.Cell("A7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("A7").Value = "MSI Group ";

                    ws.Cell("B7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //ws.Cell("B7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    //ws.Cell("B7").Value = "No ";

                    //ws.Cell("C7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //ws.Cell("C7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("C7").Value = "Description ";

                    //ws.Cell("D7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    //ws.Cell("D7").Value = "Unit ";

                    ws.Cell("E7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("E7").Value = "Average/Month ";

                    ws.Cell("F7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("F7").Value = "Total ";

                    ws.Cell("G7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("G7").Value = "Jan ";

                    ws.Cell("H7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("H7").Value = "Feb ";

                    ws.Cell("I7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("I7").Value = "Mar ";

                    ws.Cell("J7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("J7").Value = "Apr ";

                    ws.Cell("K7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("K7").Value = "May ";

                    ws.Cell("L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("L7").Value = "Jun ";

                    ws.Cell("M7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("M7").Value = "Jul ";

                    ws.Cell("N7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("N7").Value = "Aug ";

                    ws.Cell("O7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("O7").Value = "Sep ";

                    ws.Cell("P7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("P7").Value = "Oct ";

                    ws.Cell("Q7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("Q7").Value = "Nov ";

                    ws.Cell("R7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("R7").Value = "Dec ";

                    #endregion

                    #region ** field values **
                    foreach (var row in dt1.Rows)
                    {
                        msiGroupName = ((System.Data.DataRow)(row)).ItemArray[4].ToString();
                        if (msiGroupNameCheck != msiGroupName)
                        {
                            indexRow = 0;
                            index = index + 1;
                            if (index == 1)
                            {
                                indexRow = indexRow + (28 - 1);
                            }
                            if (index == 2)
                            {
                                indexRow = indexRow + (5 - 1);
                            }
                            if (index == 3)
                            {
                                indexRow = indexRow + (24 - 1);
                            }

                            if (index == 4)
                            {
                                indexRow = indexRow + (12 - 1);
                            }
                            if (index == 5)
                            {
                                indexRow = indexRow + (9 - 1);
                            }
                            if (index == 6)
                            {
                                indexRow = indexRow + (8 - 1);
                            }
                            if (index == 7)
                            {
                                indexRow = indexRow + (12 - 1);
                            }

                            //MSI GROUP
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Merge();
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;


                        }

                        //NO
                        ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                        ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //MSI DESC
                        //ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //UNIT
                        //ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                        ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //AVERAGE
                        ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                        ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("E" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //TOTAL
                        ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                        ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("F" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH01
                        ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("G" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH02
                        ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                        ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("H" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH03
                        ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                        ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("I" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH04
                        ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                        ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("J" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH05
                        ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                        ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("K" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH06
                        ws.Cell("L" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                        ws.Cell("L" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("L" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH07
                        ws.Cell("M" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                        ws.Cell("M" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("M" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH08
                        ws.Cell("N" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                        ws.Cell("N" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("N" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH09
                        ws.Cell("O" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                        ws.Cell("O" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("O" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH10
                        ws.Cell("P" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                        ws.Cell("P" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("P" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH11
                        ws.Cell("Q" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                        ws.Cell("Q" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("Q" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH12
                        ws.Cell("R" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                        ws.Cell("R" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("R" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        msiGroupNameCheck = msiGroupName;
                        lastRow++;
                    }

                    #endregion

                }
                #endregion

            }
            else
            {
                dt1 = ds.Tables[0];
                dt2 = ds.Tables[1];

                if (dt1.Rows.Count == 0)
                {
                    return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
                }

                #region ** write Area==null **
                if (area == "")
                {
                    int lastRow = 8;
                    int index = 0;
                    int indexRow = 0;

                    area = "-- SELECT ALL --";
                    TextDealer = "-- SELECT ALL --";
                    TextOutlet = "-- SELECT ALL --";

                    var msiGroupNameCheck = "";
                    var msiGroupName = "";

                    var ws = wb.Worksheets.Add(area);

                    #region ** write header **
                    var hdrTable = ws.Range("A1:U7");
                    hdrTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    var rngTable = ws.Range("A7:U7");
                    rngTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                        .Alignment.SetWrapText();

                    rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTable.Style.Font.Bold = true;

                    ws.Columns("1").Width = 40;
                    ws.Columns("2").Width = 5;
                    ws.Columns("3").Width = 90;
                    ws.Columns("4").Width = 10;
                    ws.Columns("5").Width = 20;
                    ws.Columns("6").Width = 20;
                    ws.Columns("7").Width = 15;
                    ws.Columns("8").Width = 15;
                    ws.Columns("9").Width = 15;
                    ws.Columns("10").Width = 15;
                    ws.Columns("11").Width = 15;
                    ws.Columns("12").Width = 15;
                    ws.Columns("13").Width = 15;
                    ws.Columns("14").Width = 15;
                    ws.Columns("15").Width = 15;
                    ws.Columns("16").Width = 15;
                    ws.Columns("17").Width = 15;
                    ws.Columns("18").Width = 15;

                    //First Names   
                    ws.Cell("A1").Value = "Inquiry Suzuki MSI V2";
                    ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                    ws.Cell("A2").Value = "Year";
                    ws.Cell("D2").Value = "Date";
                    ws.Cell("A3").Value = "Area";
                    ws.Cell("A4").Value = "Dealer";
                    ws.Cell("A5").Value = "Outlet";

                    ws.Cell("B2").Value = Year;
                    ws.Cell("E2").Value = DateTime.Now.ToString();
                    ws.Cell("E2").Style.DateFormat.Format = "DD-MMM-YYYY";
                    ws.Cell("B3").Value = area;
                    ws.Cell("B4").Value = TextDealer;
                    ws.Cell("B5").Value = TextOutlet;

                    ws.Cell("A7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("A7").Value = "MSI Group ";

                    ws.Cell("B7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //ws.Cell("B7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    //ws.Cell("B7").Value = "No ";

                    //ws.Cell("C7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //ws.Cell("C7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("C7").Value = "Description ";

                    //ws.Cell("D7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    //ws.Cell("D7").Value = "Unit ";

                    ws.Cell("E7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("E7").Value = "Average/Month ";

                    ws.Cell("F7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("F7").Value = "Total ";

                    ws.Cell("G7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("G7").Value = "Jan ";

                    ws.Cell("H7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("H7").Value = "Feb ";

                    ws.Cell("I7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("I7").Value = "Mar ";

                    ws.Cell("J7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("J7").Value = "Apr ";

                    ws.Cell("K7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("K7").Value = "May ";

                    ws.Cell("L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("L7").Value = "Jun ";

                    ws.Cell("M7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("M7").Value = "Jul ";

                    ws.Cell("N7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("N7").Value = "Aug ";

                    ws.Cell("O7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("O7").Value = "Sep ";

                    ws.Cell("P7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("P7").Value = "Oct ";

                    ws.Cell("Q7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("Q7").Value = "Nov ";

                    ws.Cell("R7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("R7").Value = "Dec ";

                    #endregion

                    #region ** field values **
                    foreach (var row in dt2.Rows)
                    {
                        msiGroupName = ((System.Data.DataRow)(row)).ItemArray[2].ToString();
                        if (msiGroupNameCheck != msiGroupName)
                        {
                            indexRow = 0;
                            index = index + 1;
                            if (index == 1)
                            {
                                indexRow = indexRow + (28 - 1);
                            }
                            if (index == 2)
                            {
                                indexRow = indexRow + (5 - 1);
                            }
                            if (index == 3)
                            {
                                indexRow = indexRow + (24 - 1);
                            }

                            if (index == 4)
                            {
                                indexRow = indexRow + (12 - 1);
                            }
                            if (index == 5)
                            {
                                indexRow = indexRow + (9 - 1);
                            }
                            if (index == 6)
                            {
                                indexRow = indexRow + (8 - 1);
                            }
                            if (index == 7)
                            {
                                indexRow = indexRow + (12 - 1);
                            }

                            //MSI GROUP
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Merge();
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Value = ((System.Data.DataRow)(row)).ItemArray[2];
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;


                        }

                        //NO
                        ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[1];
                        ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //MSI DESC
                        //ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                        ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //UNIT
                        //ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                        ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //AVERAGE
                        ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("E" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //TOTAL
                        ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                        ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("F" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH01
                        ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                        ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("G" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH02
                        ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                        ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("H" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH03
                        ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("I" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH04
                        ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                        ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("J" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH05
                        ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                        ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("K" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH06
                        ws.Cell("L" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                        ws.Cell("L" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("L" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH07
                        ws.Cell("M" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                        ws.Cell("M" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("M" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH08
                        ws.Cell("N" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                        ws.Cell("N" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("N" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH09
                        ws.Cell("O" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                        ws.Cell("O" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("O" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH10
                        ws.Cell("P" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                        ws.Cell("P" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("P" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH11
                        ws.Cell("Q" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                        ws.Cell("Q" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("Q" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH12
                        ws.Cell("R" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                        ws.Cell("R" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("R" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        msiGroupNameCheck = msiGroupName;
                        lastRow++;
                    }
                    #endregion

                }
                #endregion
                else
                {
                    int lastRow = 8;
                    int index = 0;
                    int indexRow = 0;

                    var msiGroupNameCheck = "";
                    var msiGroupName = "";
                    var sheetNameCheck = "";
                    // penambahan untuk mapping area service
                    var sheetName = ctx.SrvGroupAreas.Find(area).AreaDealer;
                    var dealerCode = "";

                    if (sheetName == "JAWA TIMUR / BALI / LOMBOK")
                    {
                        sheetName = "JAWA TIMUR - BALI - LOMBOK";
                    }

                    var ws = wb.Worksheets.Add(sheetName);
                    var hdrTable = ws.Range("A1:U7");
                    var rngTable = ws.Range("A7:U7");

                    #region ** write header summary **
                    hdrTable = ws.Range("A1:U7");
                    hdrTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                    rngTable = ws.Range("A7:U7");
                    rngTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                        .Alignment.SetWrapText();

                    rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTable.Style.Font.Bold = true;

                    ws.Columns("1").Width = 40;
                    ws.Columns("2").Width = 5;
                    ws.Columns("3").Width = 90;
                    ws.Columns("4").Width = 10;
                    ws.Columns("5").Width = 20;
                    ws.Columns("6").Width = 20;
                    ws.Columns("7").Width = 15;
                    ws.Columns("8").Width = 15;
                    ws.Columns("9").Width = 15;
                    ws.Columns("10").Width = 15;
                    ws.Columns("11").Width = 15;
                    ws.Columns("12").Width = 15;
                    ws.Columns("13").Width = 15;
                    ws.Columns("14").Width = 15;
                    ws.Columns("15").Width = 15;
                    ws.Columns("16").Width = 15;
                    ws.Columns("17").Width = 15;
                    ws.Columns("18").Width = 15;

                    //First Names   
                    ws.Cell("A1").Value = "Inquiry Suzuki MSI V2 by " + DataSource;
                    ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                    ws.Cell("A2").Value = "Year";
                    ws.Cell("D2").Value = "Date";
                    ws.Cell("A3").Value = "Area";
                    ws.Cell("A4").Value = "Dealer";
                    ws.Cell("A5").Value = "Outlet";

                    ws.Range("B2:C2").Merge();
                    ws.Cell("B2").Value = Year;
                    ws.Cell("E2").Value = DateTime.Now.ToString();
                    ws.Cell("E2").Style.DateFormat.Format = "DD-MMM-YYYY HH:mm";
                    ws.Range("B3:C3").Merge();
                    //ws.Cell("B3").Value = area;
                    // update untuk mapping dealer service
                    string areaDealer = ctx.SrvGroupAreas.Find(area).AreaDealer;
                    ws.Cell("B3").Value = areaDealer;
                    ws.Range("B4:C4").Merge();
                    ws.Cell("B4").Value = TextDealer;
                    ws.Range("B5:C5").Merge();
                    ws.Cell("B5").Value = TextOutlet;

                    ws.Cell("A7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("A7").Value = "MSI Group ";

                    ws.Cell("B7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //ws.Cell("B7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    //ws.Cell("B7").Value = "No ";

                    //ws.Cell("C7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    //ws.Cell("C7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("C7").Value = "Description ";

                    //ws.Cell("D7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    //ws.Cell("D7").Value = "Unit ";

                    ws.Cell("E7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("E7").Value = "Average/Month ";

                    ws.Cell("F7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("F7").Value = "Total ";

                    ws.Cell("G7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("G7").Value = "Jan ";

                    ws.Cell("H7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("H7").Value = "Feb ";

                    ws.Cell("I7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("I7").Value = "Mar ";

                    ws.Cell("J7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("J7").Value = "Apr ";

                    ws.Cell("K7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("K7").Value = "May ";

                    ws.Cell("L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("L7").Value = "Jun ";

                    ws.Cell("M7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("M7").Value = "Jul ";

                    ws.Cell("N7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("N7").Value = "Aug ";

                    ws.Cell("O7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("O7").Value = "Sep ";

                    ws.Cell("P7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("P7").Value = "Oct ";

                    ws.Cell("Q7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("Q7").Value = "Nov ";

                    ws.Cell("R7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                    ws.Cell("R7").Value = "Dec ";

                    #endregion

                    #region ** field values summary **
                    foreach (var row in dt2.Rows)
                    {
                        msiGroupName = ((System.Data.DataRow)(row)).ItemArray[2].ToString();
                        if (msiGroupNameCheck != msiGroupName)
                        {
                            indexRow = 0;
                            index = index + 1;
                            if (index == 1)
                            {
                                indexRow = indexRow + (28 - 1);
                            }
                            if (index == 2)
                            {
                                indexRow = indexRow + (5 - 1);
                            }
                            if (index == 3)
                            {
                                indexRow = indexRow + (24 - 1);
                            }

                            if (index == 4)
                            {
                                indexRow = indexRow + (12 - 1);
                            }
                            if (index == 5)
                            {
                                indexRow = indexRow + (9 - 1);
                            }
                            if (index == 6)
                            {
                                indexRow = indexRow + (8 - 1);
                            }
                            if (index == 7)
                            {
                                indexRow = indexRow + (12 - 1);
                            }

                            //MSI GROUP
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Merge();
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Value = ((System.Data.DataRow)(row)).ItemArray[2];
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;


                        }

                        //NO
                        ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[1];
                        ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //MSI DESC
                        //ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                        ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //UNIT
                        //ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                        ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //AVERAGE
                        ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("E" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //TOTAL
                        ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                        ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("F" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH01
                        ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                        ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("G" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH02
                        ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                        ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("H" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH03
                        ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("I" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH04
                        ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                        ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("J" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH05
                        ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                        ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("K" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH06
                        ws.Cell("L" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                        ws.Cell("L" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("L" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH07
                        ws.Cell("M" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                        ws.Cell("M" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("M" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH08
                        ws.Cell("N" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                        ws.Cell("N" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("N" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH09
                        ws.Cell("O" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                        ws.Cell("O" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("O" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH10
                        ws.Cell("P" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                        ws.Cell("P" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("P" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH11
                        ws.Cell("Q" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                        ws.Cell("Q" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("Q" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH12
                        ws.Cell("R" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                        ws.Cell("R" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("R" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        msiGroupNameCheck = msiGroupName;
                        lastRow++;
                    }

                    #endregion

                    #region ** write detail **

                    lastRow = 8;
                    index = 0;
                    indexRow = 0;

                    foreach (var row in dt1.Rows)
                    {
                        #region ** write header detail **

                        sheetNameCheck = ((System.Data.DataRow)(row)).ItemArray[1].ToString();
                        // penambahan untuk mapping dealer service.
                        sheetNameCheck = ctx.svMasterDealerOutletMappings.Where(p => p.GNOutletCode == sheetNameCheck).FirstOrDefault().OutletCode;
                        dealerCode = ((System.Data.DataRow)(row)).ItemArray[0].ToString();
                        // penambahan untuk mapping dealer service.
                        dealerCode = ctx.svMasterDealerMappings.Where(p => p.GNDealerCode == dealerCode && p.GroupNo.ToString() == area).FirstOrDefault().DealerCode;
                        if (sheetName != sheetNameCheck)
                        {
                            lastRow = 8;
                            index = 0;
                            indexRow = 0;

                            ws = wb.Worksheets.Add(sheetNameCheck);
                            sheetName = sheetNameCheck;
                            hdrTable = ws.Range("A1:U7");
                            hdrTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);

                            rngTable = ws.Range("A7:U7");
                            rngTable.Style
                                .Font.SetBold()
                                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                .Alignment.SetWrapText();

                            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            rngTable.Style.Font.Bold = true;

                            ws.Columns("1").Width = 40;
                            ws.Columns("2").Width = 5;
                            ws.Columns("3").Width = 90;
                            ws.Columns("4").Width = 10;
                            ws.Columns("5").Width = 20;
                            ws.Columns("6").Width = 20;
                            ws.Columns("7").Width = 15;
                            ws.Columns("8").Width = 15;
                            ws.Columns("9").Width = 15;
                            ws.Columns("10").Width = 15;
                            ws.Columns("11").Width = 15;
                            ws.Columns("12").Width = 15;
                            ws.Columns("13").Width = 15;
                            ws.Columns("14").Width = 15;
                            ws.Columns("15").Width = 15;
                            ws.Columns("16").Width = 15;
                            ws.Columns("17").Width = 15;
                            ws.Columns("18").Width = 15;

                            //First Names   
                            ws.Cell("A1").Value = "Inquiry Suzuki MSI V2 by " + DataSource;
                            ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                            ws.Cell("A2").Value = "Year";
                            ws.Cell("D2").Value = "Date";
                            ws.Cell("A3").Value = "Area";
                            ws.Cell("A4").Value = "Dealer";
                            ws.Cell("A5").Value = "Outlet";

                            ws.Range("B2:C2").Merge();
                            ws.Cell("B2").Value = Year;
                            ws.Cell("E2").Value = DateTime.Now.ToString();
                            ws.Cell("E2").Style.DateFormat.Format = "DD-MMM-YYYY HH:mm";
                            ws.Range("B3:C3").Merge();
                            //ws.Cell("B3").Value = area;
                            // update untuk mapping dealer service
                            ws.Cell("B3").Value = areaDealer;
                            ws.Range("B4:C4").Merge();
                            ws.Cell("B4").Value = dealerCode + " - " + ((System.Data.DataRow)(row)).ItemArray[21];
                            ws.Range("B5:C5").Merge();
                            // penambahan untuk mapping dealer code service.
                            ws.Cell("B5").Value = sheetName + " - " + ((System.Data.DataRow)(row)).ItemArray[22];

                            ws.Cell("A7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("A7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("A7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("A7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("A7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("A7").Value = "MSI Group ";

                            ws.Cell("B7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            //ws.Cell("B7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("B7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("B7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("B7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            //ws.Cell("B7").Value = "No ";

                            //ws.Cell("C7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            //ws.Cell("C7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("C7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("C7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("C7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("C7").Value = "Description ";

                            //ws.Cell("D7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            //ws.Cell("D7").Value = "Unit ";

                            ws.Cell("E7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("E7").Value = "Average/Month ";

                            ws.Cell("F7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("F7").Value = "Total ";

                            ws.Cell("G7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("G7").Value = "Jan ";

                            ws.Cell("H7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("H7").Value = "Feb ";

                            ws.Cell("I7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("I7").Value = "Mar ";

                            ws.Cell("J7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("J7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("J7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("J7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("J7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("J7").Value = "Apr ";

                            ws.Cell("K7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("K7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("K7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("K7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("K7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("K7").Value = "May ";

                            ws.Cell("L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("L7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("L7").Value = "Jun ";

                            ws.Cell("M7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("M7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("M7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("M7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("M7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("M7").Value = "Jul ";

                            ws.Cell("N7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("N7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("N7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("N7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("N7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("N7").Value = "Aug ";

                            ws.Cell("O7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("O7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("O7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("O7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("O7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("O7").Value = "Sep ";

                            ws.Cell("P7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("P7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("P7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("P7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("P7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("P7").Value = "Oct ";

                            ws.Cell("Q7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("Q7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("Q7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("Q7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("Q7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("Q7").Value = "Nov ";

                            ws.Cell("R7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("R7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("R7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("R7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("R7").Style.Fill.BackgroundColor = XLColor.LightBlue;
                            ws.Cell("R7").Value = "Dec ";

                        }
                        #endregion

                        #region** field values **

                        msiGroupName = ((System.Data.DataRow)(row)).ItemArray[4].ToString();
                        if (msiGroupNameCheck != msiGroupName)
                        {
                            indexRow = 0;
                            index = index + 1;
                            if (index == 1)
                            {
                                indexRow = indexRow + (28 - 1);
                            }
                            if (index == 2)
                            {
                                indexRow = indexRow + (5 - 1);
                            }
                            if (index == 3)
                            {
                                indexRow = indexRow + (24 - 1);
                            }

                            if (index == 4)
                            {
                                indexRow = indexRow + (12 - 1);
                            }
                            if (index == 5)
                            {
                                indexRow = indexRow + (9 - 1);
                            }
                            if (index == 6)
                            {
                                indexRow = indexRow + (8 - 1);
                            }
                            if (index == 7)
                            {
                                indexRow = indexRow + (12 - 1);
                            }

                            //MSI GROUP
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Merge();
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            ws.Range("A" + lastRow + ":" + "A" + (lastRow + indexRow)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;


                        }

                        //NO
                        ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                        ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                        //MSI DESC
                        //ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        //ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //UNIT
                        //ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                        ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        //AVERAGE
                        ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                        ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("E" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //TOTAL
                        ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                        ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("F" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH01
                        ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("G" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH02
                        ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                        ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("H" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH03
                        ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                        ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("I" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH04
                        ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                        ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("J" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH05
                        ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                        ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("K" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH06
                        ws.Cell("L" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                        ws.Cell("L" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("L" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH07
                        ws.Cell("M" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                        ws.Cell("M" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("M" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH08
                        ws.Cell("N" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                        ws.Cell("N" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("N" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH09
                        ws.Cell("O" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                        ws.Cell("O" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("O" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH10
                        ws.Cell("P" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                        ws.Cell("P" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("P" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH11
                        ws.Cell("Q" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                        ws.Cell("Q" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("Q" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        //MONTH12
                        ws.Cell("R" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                        ws.Cell("R" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("R" + lastRow).Style.NumberFormat.Format = "#,##0.00";

                        msiGroupNameCheck = msiGroupName;
                        lastRow++;

                        #endregion

                    }

                    #endregion

                }

            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));

        }
        //end

        public JsonResult SvMsiR2()
        {
            string comp = Request["CompanyCode"] ?? "--";
            string bran = Request["BranchCode"] ?? "--";
            string year = string.IsNullOrWhiteSpace(Request["Year"]) ? "2100" : Request["Year"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_WhInqMsiR2";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@BranchCode", bran);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult SvMsiR2V2()
        {
            string comp = Request["CompanyCode"] ?? "--";
            string bran = Request["BranchCode"] ?? "--";
            string year = string.IsNullOrWhiteSpace(Request["Year"]) ? "2100" : Request["Year"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_WhInqMsiR2V2";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@BranchCode", bran);
            cmd.Parameters.AddWithValue("@PeriodYear", year);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.Username);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult Cs3DaysCall()
        {
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsInqTDaysCall";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@DateFrom", dateFrom);
            cmd.Parameters.AddWithValue("@DateTo", dateTo);
            cmd.Parameters.AddWithValue("@Outstanding", "N");
            cmd.Parameters.AddWithValue("@Status", "Inquiry");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult StnkExtension()
        {
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            string stnkext = Request["StnkExt"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsInqStnkExtSim";
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsInqStnkExtSim_New";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@GroupNo", groupArea);
            //cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@Company", companyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@DateFrom", dateFrom);
            cmd.Parameters.AddWithValue("@DateTo", dateTo);
            cmd.Parameters.AddWithValue("@StnkExt", stnkext);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult BpkbReminder()
        {
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsInqBpkbReminderSim";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@DateFrom", dateFrom);
            cmd.Parameters.AddWithValue("@DateTo", dateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult CsFeedback()
        {
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string companyCode = Request["CompanyCode"];
            var data = ctx.Database.SqlQuery<FeedbackModel>("exec uspfn_CsInqCustFeedback @CompanyCode=@p0, @DateFrom=@p1, @DateTo=@p2", companyCode, dateFrom, dateTo).AsQueryable();
            return Json(data);
        }

        public JsonResult CsBirthday()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsInqCustBirthday";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@GroupNo", Request["GroupArea"]);
            cmd.Parameters.AddWithValue("@CompanyCode", Request["CompanyCode"]);
            cmd.Parameters.AddWithValue("@BranchCode", Request["BranchCode"]);
            cmd.Parameters.AddWithValue("@PeriodYear", Request["PeriodYear"]);
            cmd.Parameters.AddWithValue("@ParMonth1", Request["ParMonth1"]);
            cmd.Parameters.AddWithValue("@ParMonth2", Request["ParMonth2"]);
            cmd.Parameters.AddWithValue("@ParStatus", Request["ParStatus"]);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult CustomerStatus()
        {
            DataTable dt = new DataTable();
            if (Request["InqType"].ToString().ToLower().Equals("d"))
            {
                string CompanyCode = (string.IsNullOrWhiteSpace(Request["CompanyCode"]) ? "" : Request["CompanyCode"].ToString());
                string BranchCode = (string.IsNullOrWhiteSpace(Request["BranchCode"]) ? "" : Request["BranchCode"].ToString());
                string Year = (string.IsNullOrWhiteSpace(Request["Year"]) ? DateTime.Now.Year.ToString() : Request["Year"]);
                string Month = (String.IsNullOrWhiteSpace(Request["Month"]) ? DateTime.Now.Month.ToString() : Request["Month"]);

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsCustSzkLastTrans";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                cmd.Parameters.AddWithValue("@Year", Year);
                cmd.Parameters.AddWithValue("@Month", Month);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            else
            {
                string CompanyCode = (string.IsNullOrWhiteSpace(Request["CompanyCode"]) ? "" : Request["CompanyCode"].ToString());
                string BranchCode = (string.IsNullOrWhiteSpace(Request["BranchCode"]) ? "" : Request["BranchCode"].ToString());
                string InqType = (string.IsNullOrWhiteSpace(Request["InqType"]) ? "" : Request["InqType"].ToString());
                string Month = (string.IsNullOrWhiteSpace(Request["Month"]) ? "" : Request["Month"].ToString());
                string YearTo = (string.IsNullOrWhiteSpace(Request["YearTo"]) ? "" : Request["YearTo"].ToString());
                string YearFrom = (string.IsNullOrWhiteSpace(Request["YearFrom"]) ? "" : Request["YearFrom"].ToString());

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsCustomerDataInq";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@SelectCode", InqType);
                cmd.Parameters.AddWithValue("@Month", Month);
                cmd.Parameters.AddWithValue("@YearTo", YearTo);
                cmd.Parameters.AddWithValue("@YearFrom", YearFrom);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return Json(GetJson(dt));
        }

        public JsonResult CustomerSuzukiXls()
        {
            DataSet ds = new DataSet();
            string fileName = "";
            string CompanyCode = Request["CompanyCode"];
            string BranchCode = Request["BranchCode"];
            string InqType = Request["InqType"];
            string Year = Request["Year"];
            string Month = Request["Month"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsCustDealer";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@InqType", InqType);
            cmd.Parameters.AddWithValue("@Year", Year);
            cmd.Parameters.AddWithValue("@Month", Month);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            var header = new List<List<dynamic>>() { };
            var format = new List<List<dynamic>>() { };
            var sheets = new List<string> { };

            switch (InqType)
            {
                case "A":
                    header.Add(new List<dynamic> { "Company", (CompanyCode == "%") ? "ALL" : ctx.DealerInfos.Find(new string[] { CompanyCode }).DealerName });
                    header.Add(new List<dynamic> { "Inquiry Type", "Customer Data" });
                    header.Add(new List<dynamic> { "Periode", string.Format("'{0} / {1}", Month, Year) });

                    format.Add(new List<dynamic> { 
                        new { name = "CompanyAbbr", text = "Company Abbr", width = 20 },
                        new { name = "CompanyName", text = "Company Name", width = 80 },
                        new { name = "NoOfUnit", text = "No of Unit", width = 20 },
                        new { name = "NoOfService", text = "No of Service", width = 20 },
                        new { name = "NoOfSparePart", text = "No of Sparepart", width = 20 }
                    });

                    sheets.Add("Customer Data");
                    fileName = "CustomerData";

                    break;
                case "B":
                    header.Add(new List<dynamic> { "Company", (CompanyCode == "%") ? "ALL" : ctx.DealerInfos.Find(new string[] { CompanyCode }).DealerName });
                    header.Add(new List<dynamic> { "Branch", (BranchCode == "%") ? "ALL" : ctx.OutletInfos.Where(p => p.BranchCode == BranchCode).FirstOrDefault().BranchName });
                    header.Add(new List<dynamic> { "Inquiry Type", "Customer by Transaction" });
                    header.Add(new List<dynamic> { "Periode", string.Format("'{0} / {1}", Month, Year) });

                    format.Add(new List<dynamic> {
                        new { name = "CompanyAbbr", text = "Company Abbr", width = 20 },
                        new { name = "BranchName", text = "Branch Name", width = 80 },
                        new { name = "NoOfUnitService", text = "No of Unit Service", width = 20 },
                        new { name = "NoOfUnit", text = "No of Unit", width = 20 },
                        new { name = "NoOfService", text = "No of Service", width = 20 },
                        new { name = "NoOfSparePart", text = "No of Sparepart", width = 20 }
                    });
                    format.Add(new List<dynamic> { 
                        new { name = "CompanyAbbr", text = "Company Abbr", width = 20 },
                        new { name = "CompanyName", text = "Company Name", width = 80 },
                        new { name = "NoOfUnitService", text = "No of Unit Service", width = 20 },
                        new { name = "NoOfUnit", text = "No of Unit", width = 20 },
                        new { name = "NoOfService", text = "No of Service", width = 20 },
                        new { name = "NoOfSparePart", text = "No of Sparepart", width = 20 }
                    });

                    sheets.Add("Detail");
                    sheets.Add("Summary");
                    fileName = "CustomerByTrans";

                    break;
                case "C":
                    header.Add(new List<dynamic> { "Company", (CompanyCode == "%") ? "ALL" : ctx.DealerInfos.Find(new string[] { CompanyCode }).DealerName });
                    header.Add(new List<dynamic> { "Branch", (BranchCode == "%") ? "ALL" : ctx.OutletInfos.Where(p => p.BranchCode == BranchCode).FirstOrDefault().BranchName });
                    header.Add(new List<dynamic> { "Inquiry Type", "Customer Suzuki by Transaction" });
                    header.Add(new List<dynamic> { "Periode", string.Format("'{0} / {1}", Month, Year) });

                    format.Add(new List<dynamic> {
                        new { name = "CompanyAbbr", text = "Company Abbr", width = 20 },
                        new { name = "BranchName", text = "Branch Name", width = 80 },
                        new { name = "NoOfUnitService", text = "No of Unit Service", width = 20 },
                        new { name = "NoOfUnit", text = "No of Unit", width = 20 },
                        new { name = "NoOfService", text = "No of Service", width = 20 }
                    });
                    format.Add(new List<dynamic> { 
                        new { name = "CompanyAbbr", text = "Company Abbr", width = 20 },
                        new { name = "CompanyName", text = "Company Name", width = 80 },
                        new { name = "NoOfUnitService", text = "No of Unit Service", width = 20 },
                        new { name = "NoOfUnit", text = "No of Unit", width = 20 },
                        new { name = "NoOfService", text = "No of Service", width = 20 }
                    });

                    sheets.Add("Detail");
                    sheets.Add("Summary");
                    fileName = "CusSzkByTrans";

                    break;
                case "D":
                    header.Add(new List<dynamic> { "Company", (CompanyCode == "%") ? "ALL" : ctx.DealerInfos.Find(new string[] { CompanyCode }).DealerName });
                    header.Add(new List<dynamic> { "Branch", (BranchCode == "%") ? "ALL" : ctx.OutletInfos.Where(p => p.BranchCode == BranchCode).FirstOrDefault().BranchName });
                    header.Add(new List<dynamic> { "Inquiry Type", "Customer Suzuki with 3 Years Transaction" });
                    header.Add(new List<dynamic> { "Periode", string.Format("'{0} / {1}", Month, Year) });

                    format.Add(new List<dynamic> {
                        new { name = "CompanyAbbr", text = "Company Abbr", width = 20 },
                        new { name = "BranchName", text = "Branch Name", width = 80 },
                        new { name = "NoOfUnitService", text = "No of Unit Service", width = 20 },
                        new { name = "NoOfUnit", text = "No of Unit", width = 20 },
                        new { name = "NoOfService", text = "No of Service", width = 20 }
                    });
                    format.Add(new List<dynamic> { 
                        new { name = "CompanyAbbr", text = "Company Abbr", width = 20 },
                        new { name = "CompanyName", text = "Company Name", width = 80 },
                        new { name = "NoOfUnitService", text = "No of Unit Service", width = 20 },
                        new { name = "NoOfUnit", text = "No of Unit", width = 20 },
                        new { name = "NoOfService", text = "No of Service", width = 20 }
                    });

                    sheets.Add("Detail");
                    sheets.Add("Summary");
                    fileName = "CusSzk3YearsLastTrans";

                    break;
                default:
                    break;
            }

            return GenerateReportXls(ds, sheets, fileName, header, format);
        }

        public JsonResult CustomerSuzukiLastTrans()
        {
            DataTable dt = new DataTable();
            string CompanyCode = (string.IsNullOrWhiteSpace(Request["CompanyCode"]) ? "" : Request["CompanyCode"].ToString());
            string BranchCode = (string.IsNullOrWhiteSpace(Request["BranchCode"]) ? "" : Request["BranchCode"].ToString());
            string Year = (string.IsNullOrWhiteSpace(Request["Year"]) ? DateTime.Now.Year.ToString() : Request["Year"]);
            string Month = (String.IsNullOrWhiteSpace(Request["Month"]) ? DateTime.Now.Month.ToString() : Request["Month"]);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsCustSzkLastTrans";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@Year", Year);
            cmd.Parameters.AddWithValue("@Month", Month);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Company", (CompanyCode == "%") ? "ALL" : ctx.DealerInfos.Find(new string[] { CompanyCode }).DealerName });
            header.Add(new List<dynamic> { "Branch", (BranchCode == "%") ? "ALL" : ctx.OutletInfos.Where(p => p.BranchCode == BranchCode).FirstOrDefault().BranchName });
            header.Add(new List<dynamic> { "Inquiry Type", "Customer Suzuki with 3 Years Transaction" });
            header.Add(new List<dynamic> { "Periode", string.Format("'{0} / {1}", Month, Year) });

            var format = new List<dynamic>() {
                new { name = "CompanyName", text = "Company Name" , width = 20 },
                new { name = "BranchName", text = "Branch Name" , width = 80 },
                new { name = "NoOfUnitService", text = "No of Unit Service" , width = 20 },
                new { name = "NoOfUnit", text = "No of Unit" , width = 20 },
                new { name = "NoOfService", text = "No of Service" , width = 20 }
            };
            return GenerateReportXls(dt, "Customer Suzuki", "CustomerSuzuki", header, format: format);
        }

        public JsonResult CustomerDealer()
        {
            DataTable dt = new DataTable();
            var CompanyCode = Request["CompanyCode"];
            var BranchCode = Request["BranchCode"];
            var CustType = Request["CustType"];
            var DateFrom = Request["DateFrom"];
            var DateTo = Request["DateTo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsCustDealerDtl";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 3600;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@CustType", CustType);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
            cmd.Parameters.AddWithValue("@DateTo", DateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var header = new List<List<dynamic>>() { };

            header.Add(new List<dynamic> { "Company", (CompanyCode == "%") ? "ALL" : ctx.DealerInfos.Find(new string[] { CompanyCode }).DealerName });
            header.Add(new List<dynamic> { "Branch", (BranchCode == "%") ? "ALL" : ctx.OutletInfos.Where(p => p.BranchCode == BranchCode).FirstOrDefault().BranchName });
            header.Add(new List<dynamic> { "Inquiry Type", "Customer Suzuki Detail" });
            if (DateFrom != "1900-01-01" && DateTo != "2100-01-01")
            {
                header.Add(new List<dynamic> { "Periode", string.Format("{0} s.d {1}", DateFrom, DateTo) });
            }

            if (dt.Rows.Count > 60000)
            {
                return Json(new
                {
                    rows = dt.Rows.Count,
                    cols = dt.Columns.Count,
                });
            }
            else
            {
                return GenerateReportXls(dt, "Customer", "CustomerList", header);
            }
        }

        public JsonResult CustomerStatusDetail()
        {
            string CompanyCode = (string.IsNullOrWhiteSpace(Request["CompanyCode"]) ? "" : Request["CompanyCode"].ToString());
            string BranchCode = (string.IsNullOrWhiteSpace(Request["BranchCode"]) ? "" : Request["BranchCode"].ToString());
            string CustType = (string.IsNullOrWhiteSpace(Request["CustType"]) ? "" : Request["CustType"].ToString());
            string TransDateStart = (string.IsNullOrWhiteSpace(Request["TransDateStart"]) ? "" : Request["TransDateStart"].ToString());
            string TransDateEnd = (string.IsNullOrWhiteSpace(Request["TransDateEnd"]) ? "" : Request["TransDateEnd"].ToString());

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CustSuzukiDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@CustType", CustType);
            cmd.Parameters.AddWithValue("@TransDateStart", TransDateStart);
            cmd.Parameters.AddWithValue("@TransDateEnd", TransDateEnd);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult CsDashboardDefault()
        {
            try
            {
                string CompanyCode = Request["CompanyCode"] ?? "";
                string BranchCode = Request["BanchCode"] ?? "";

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsDashSummary";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var list = GetJson(dt);

                return Json(new { success = true, data = new { RemiderDate = DateTime.Now }, list = list });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult CsDashboard()
        {
            try
            {
                string CompanyCode = Request["CompanyCode"] ?? "";
                string BranchCode = Request["BranchCode"] ?? "%";

                if (string.IsNullOrEmpty(BranchCode))
                {
                    BranchCode = "%";
                }

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsDashSummary";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var list = GetJson(dt);

                return Json(new { success = true, data = new { CompanyName = "", BranchName = "", RemiderDate = DateTime.Now }, list = list });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult TDayCalls()
        {
            //string CompanyCode = Request["CompanyCode"] ?? "";
            //string BranchCode = Request["BranchCode"] ?? "%";

            //var oustanding = Request["OutStanding"];
            //var qry = ctx.CsLkuTDayCallViews.Where(p => p.CompanyCode == CompanyCode);

            //if (string.IsNullOrEmpty(BranchCode) == false)
            //{
            //    qry = qry.Where(x => x.BranchCode == BranchCode);
            //}

            //if (oustanding == "Y")
            //{
            //    var setting = ctx.CsSettings.Find(CompanyCode, "REM3DAYSCALL") ?? new CsSetting();
            //    var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
            //    var date1 = DateTime.Now.AddMonths(-month);
            //    var date2 = new DateTime(date1.Year, date1.Month, 1);
            //    qry = qry.Where(p => p.OutStanding == "Y" && p.DODate >= date2);
            //}
            //else
            //{
            //    var setting = ctx.CsSettings.Find(CompanyCode, "REM3DAYSCALL") ?? new CsSetting();
            //    var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
            //    var date1 = DateTime.Now.AddMonths(-month);
            //    var date2 = new DateTime(date1.Year, date1.Month, 1);
            //    qry = qry.Where(p => p.OutStanding == "N" && p.DODate >= date2);
            //}

            //return Json(qry.KGrid());

            string CompanyCode = Request["CompanyCode"] ?? "";
            string BranchCode = Request["BranchCode"] ?? "";

            if (string.IsNullOrEmpty(BranchCode))
            {
                BranchCode = "%";
            }

            var data = ctx.Database.SqlQuery<OutstandingTDayCall>("exec uspfn_TDaysCallOutstanding @CompanyCode=@p0, @BranchCode=@p1", CompanyCode, BranchCode).AsQueryable();

            return Json(data.KGrid());
        }

        public JsonResult CustomerBirthDays()
        {
            //string CompanyCode = Request["CompanyCode"] ?? "";
            //string BranchCode = Request["BranchCode"] ?? "%";

            //string customerName = Request["CustomerName"] ?? "";
            //string customerCode = Request["CustomerCode"] ?? "";
            //string branchCode = Request["BranchCode"] ?? "";
            //string outStanding = Request["OutStanding"] ?? "";

            //var qry = ctx.CsLkuBirthdayViews.Where(p =>
            //                p.CompanyCode == CompanyCode
            //                &&
            //                p.BranchCode.Contains(BranchCode)
            //          );
            //if (!string.IsNullOrWhiteSpace(outStanding)) { qry = qry.Where(p => p.OutStanding == outStanding); };
            //if (!string.IsNullOrWhiteSpace(customerCode)) { qry = qry.Where(p => p.CustomerCode.Contains(customerCode)); };
            //if (!string.IsNullOrWhiteSpace(customerName)) { qry = qry.Where(p => p.CustomerName.Contains(customerName)); };
            //if (!string.IsNullOrWhiteSpace(branchCode))
            //{
            //    qry = qry.Where(p => p.BranchCode.Contains(branchCode));
            //};

            //return Json(qry.KGrid());


            string CompanyCode = Request["CompanyCode"] ?? "";
            string BranchCode = Request["BranchCode"] ?? "";

            if (string.IsNullOrEmpty(BranchCode))
            {
                BranchCode = "%";
            }

            var data = ctx.Database.SqlQuery<OutstandingCustomerBirthday>("exec uspfn_CustomerBirthdaysOutstanding @CompanyCode=@p0, @BranchCode=@p1", CompanyCode, BranchCode).AsQueryable();

            return Json(data.KGrid());
        }

        public JsonResult StnkExt()
        {
            //string CompanyCode = Request["CompanyCode"] ?? "";
            //string BranchCode = Request["BranchCode"] ?? "%";

            //var oustanding = Request["OutStanding"];
            //var qry = ctx.CsLkuStnkExtViews.Where(p => p.CompanyCode == CompanyCode);

            //if (string.IsNullOrEmpty(BranchCode) == false)
            //{
            //    qry = qry.Where(x => x.BranchCode == BranchCode);
            //}

            //if (oustanding == "Y")
            //{
            //    var setting = ctx.CsSettings.Find(CompanyCode, "REMSTNKEXT") ?? new CsSetting();
            //    var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
            //    var date1 = DateTime.Now.AddMonths(-month);
            //    var date2 = new DateTime(date1.Year, date1.Month, 1);
            //    qry = qry.Where(p => p.OutStanding == "Y" && p.StnkExpiredDate >= date2);
            //}
            //else
            //{
            //    var setting = ctx.CsSettings.Find(CompanyCode, "REMSTNKEXT") ?? new CsSetting();
            //    var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
            //    var date1 = DateTime.Now.AddMonths(-month);
            //    var date2 = new DateTime(date1.Year, date1.Month, 1);
            //    qry = qry.Where(p => p.OutStanding == "N" && p.StnkExpiredDate >= date2);
            //}
            //return Json(qry.KGrid());

            string CompanyCode = Request["CompanyCode"] ?? "";
            string BranchCode = Request["BranchCode"] ?? "";

            if (string.IsNullOrEmpty(BranchCode))
            {
                BranchCode = "%";
            }

            var data = ctx.Database.SqlQuery<OutstandingStnkExtension>("exec uspfn_StnkExtensionOutstanding @CompanyCode=@p0, @BranchCode=@p1", CompanyCode, BranchCode).AsQueryable();

            return Json(data.KGrid());
        }

        public JsonResult Bpkbs()
        {
            //string CompanyCode = Request["CompanyCode"] ?? "";
            //string BranchCode = Request["BranchCode"] ?? "%";

            //var oustanding = Request["OutStanding"];
            //var qry = ctx.CsLkuBpkbViews.Where(p => p.CompanyCode == CompanyCode);

            //if (string.IsNullOrEmpty(BranchCode) == false)
            //{
            //    qry = qry.Where(x => x.BranchCode == BranchCode);
            //}

            //if (oustanding == "Y")
            //{
            //    var setting = ctx.CsSettings.Find(CompanyCode, "REMBPKB") ?? new CsSetting();
            //    var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
            //    var date1 = DateTime.Now.AddMonths(-month);
            //    var date2 = new DateTime(date1.Year, date1.Month, 1);
            //    qry = qry.Where(p => p.OutStanding == "Y" && p.BpkbDate >= date2);
            //}
            //else
            //{
            //    var setting = ctx.CsSettings.Find(CompanyCode, "REMBPKB") ?? new CsSetting();
            //    var month = Convert.ToInt32(setting.SettingParam1 ?? "0");
            //    var date1 = DateTime.Now.AddMonths(-month);
            //    var date2 = new DateTime(date1.Year, date1.Month, 1);
            //    qry = qry.Where(p => p.OutStanding == "N" && p.BpkbDate >= date2);
            //}
            //return Json(qry.KGrid());

            string CompanyCode = Request["CompanyCode"] ?? "";
            string BranchCode = Request["BranchCode"] ?? "";

            if (string.IsNullOrEmpty(BranchCode))
            {
                BranchCode = "%";
            }

            var data = ctx.Database.SqlQuery<OutstandingBpkbReminder>("exec uspfn_BpkbReminderOutstanding @CompanyCode=@p0, @BranchCode=@p1", CompanyCode, BranchCode).AsQueryable();

            return Json(data.KGrid());
        }

        public JsonResult CsDashDefault()
        {
            string vCompanyCode = Request["CompanyCode"] ?? "";
            string vBranchCode = Request["BranchCode"] ?? "";

            try
            {

                //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                //cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_CsDashSummary '" + vCompanyCode + "','" + vBranchCode + "'";
                //cmd.CommandTimeout = 3600;
                //cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.Clear();
                //cmd.Parameters.AddWithValue("@CompanyCode", vCompanyCode);
                //cmd.Parameters.AddWithValue("@BranchCode", vBranchCode);

                //SqlDataAdapter da = new SqlDataAdapter(cmd);
                //DataTable dt = new DataTable();
                //da.Fill(dt);
                //var list = GetJson(dt);

                var list = ctx.Database.SqlQuery<CsDashSummary>("uspfn_CsDashSummary '" + vCompanyCode + "','" + vBranchCode + "'");

                return Json(new { success = true, data = new { RemiderDate = DateTime.Now }, list = list });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            //try
            //{
            //    string CompanyCode = Request["CompanyCode"] ?? "";
            //    string BranchCode = Request["BranchCode"] ?? "";

            //    if (string.IsNullOrEmpty(BranchCode))
            //    {
            //        BranchCode = "%";
            //    }

            //    //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //    //cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_CsDashSummary";
            //    //cmd.CommandType = CommandType.StoredProcedure;
            //    //cmd.Parameters.Clear();
            //    //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //    //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //    //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //    //DataTable dt = new DataTable();
            //    //da.Fill(dt);
            //    //var list = GetJson(dt);

            //    //var data = ctx.Database.SqlQuery<CsSummary>("exec uspfn_CsDashSummary @CompanyCode=@p0, @BranchCode=p1", CompanyCode, BranchCode);
            //    var data = ctx.Database.SqlQuery<CsSummary>("exec uspfn_CsDashSummary '" + CompanyCode + "', '" + BranchCode + "'");

            //    //return Json(new { success = true, data = new { CompanyName = "", BranchName = "", RemiderDate = DateTime.Now }, list = list });
            //    return Json(new { success = true, data = new { CompanyName = "", BranchName = "", RemiderDate = DateTime.Now }, list = data });
            //}
            //catch (Exception ex)
            //{
            //    return Json(new { success = false, message = ex.Message });
            //}
        }

        public JsonResult CsDashDefaultNotOutstanding()
        {
            try
            {
                string CompanyCode = Request["CompanyCode"] ?? "";
                string BranchCode = Request["BranchCode"] ?? "";

                if (string.IsNullOrEmpty(BranchCode))
                {
                    BranchCode = "%";
                }

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsDashSummaryNotOutstanding";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                var list = GetJson(dt);

                //var data = ctx.Database.SqlQuery<CsSummary>("exec uspfn_CsDashSummary @CompanyCode=@p0, @BranchCode=p1", CompanyCode, BranchCode);

                return Json(new { success = true, data = new { CompanyName = "", BranchName = "", ReminderDate = DateTime.Now }, list = list });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GnScheduleLog()
        {
            string dlrcode = Request["DealerCode"] ?? "";
            //if (string.IsNullOrWhiteSpace(dlrcode)) dlrcode = "--";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GnSchdulerLog";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DealerCode", dlrcode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult GnScheduleLogDetails(DateTime? DateStart)
        {
            string dealerCode = Request["DealerCode"] ?? "";
            string scheduleName = Request["ScheduleName"] ?? "";
            //var createdDate = Convert.ToDateTime(Request["DateStart"]);

            if (dealerCode.Contains("0000000"))
            {
                dealerCode = "";
            }

            var data = ctx.GnSchedulerLogs.Where(x => x.DealerCode == dealerCode && x.ScheduleName == scheduleName && x.CreatedDate.Value.Year == DateStart.Value.Year && x.CreatedDate.Value.Month == DateStart.Value.Month && x.CreatedDate.Value.Day == DateStart.Value.Day).OrderByDescending(x => x.DateFinish);

            return Json(data);
        }

        public JsonResult GnCollDataLog()
        {
            var date1 = Request.Params["DateFrom"];
            var date2 = Request.Params["DateTo"];
            string dlrcode = Request["DealerCode"] ?? "--";
            string tblname = Request["TableName"] ?? "--";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GnCollDataLog";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("DealerCode", dlrcode);
            cmd.Parameters.AddWithValue("TableName", tblname);
            cmd.Parameters.AddWithValue("DateFrom", date1);
            cmd.Parameters.AddWithValue("DateTo", date2);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult GnCollDataSumLog()
        {
            var date1 = Request.Params["DateFrom"];
            var date2 = Request.Params["DateTo"];
            string tblname = Request["TableName"] ?? "--";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_GnCollDataSumLog";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("TableName", tblname);
            cmd.Parameters.AddWithValue("DateFrom", date1);
            cmd.Parameters.AddWithValue("DateTo", date2);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult SfmTurnOver()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            //var data = ctx.Database.SqlQuery<TurnOver>("exec uspfn_InqTurnOver @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @ByDate=@p3", groupArea, companyCode, branchCode, byDate).AsQueryable();
            var data = ctx.Database.SqlQuery<TurnOver>("exec uspfn_InqTurnOver_New @GroupArea=@p0, @Company=@p1, @BranchCode=@p2, @ByDate=@p3", groupArea, companyCode, branchCode, byDate).AsQueryable();
            return Json(data.KGrid());
        }

        public JsonResult SfmReview()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            var data = ctx.Database.SqlQuery<ReviewSfm>("exec uspfn_InqReviewSFM @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @ByDate=@p3", groupArea, companyCode, branchCode, byDate).AsQueryable();
            return Json(data.KGrid());
        }

        public JsonResult SfmReviewNew()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = ParamDealerCode; //Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            var groupNoNew = ParamGroupNoNew;

            var data = ctx.Database.SqlQuery<ReviewSfm>("exec uspfn_InqReviewSFMNew @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @ByDate=@p3, @GroupNoNew=@p4",
                groupArea, companyCode, branchCode, byDate, groupNoNew).AsQueryable();
            return Json(data.KGrid());
        }

        public JsonResult SfmScShBmTraining()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            var data = ctx.Database.SqlQuery<ScShBmTraining>("exec uspfn_InqSCSHBMTraining @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @ByDate=@p3", groupArea, companyCode, branchCode, byDate).AsQueryable();
            return Json(data.KGrid());
        }

        public JsonResult SfmScShBmTrainingNew()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = ParamDealerCode; //Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            var groupNoNew = ParamGroupNoNew;

            var data = ctx.Database.SqlQuery<ScShBmTraining>("exec uspfn_InqSCSHBMTrainingNew @p0, @p1, @p2, @p3, @p4", 
                groupArea, companyCode, branchCode, byDate, groupNoNew).AsQueryable();

            return Json(data.KGrid());
        }
        
        public JsonResult SfmSalesmanTraining()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            var data = ctx.Database.SqlQuery<SalesmanTraining>("exec uspfn_InqSalesmanTraining @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @ByDate=@p3", groupArea, companyCode, branchCode, byDate).AsQueryable();
            return Json(data.KGrid());
        }

        public JsonResult SfmSalesmanTrainingNew()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = ParamDealerCode;//Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            var groupNoNew = ParamGroupNoNew;

            var data = ctx.Database.SqlQuery<SalesmanTraining>("exec uspfn_InqSalesmanTrainingNew @p0, @p1, @p2, @p3, @p4", 
                groupArea, companyCode, branchCode, byDate, groupNoNew).AsQueryable();
            
            return Json(data.KGrid());
        }

        public JsonResult SfmOutstandingTraining()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            var data = ctx.Database.SqlQuery<OutstandingTraining>("exec uspfn_InqOutstandingTraining @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @ByDate=@p3", groupArea, companyCode, branchCode, byDate).AsQueryable();
            return Json(data.KGrid());
        }

        public JsonResult SfmOutstandingTrainingNew()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = ParamDealerCode; //Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var byDate = Request["ByDate"];
            var groupNoNew = ParamGroupNoNew;

            var data = ctx.Database.SqlQuery<OutstandingTraining>("exec uspfn_InqOutstandingTrainingNew @p0, @p1, @p2, @p3, @p4", 
                groupArea, companyCode, branchCode, byDate, groupNoNew).AsQueryable();

            return Json(data.KGrid());
        }

        public JsonResult SfmSalesTeamHeader()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var data = ctx.Database.SqlQuery<SalesTeamHeader>("exec uspfn_InqSalesTeamHeader @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2", groupArea, companyCode, branchCode).AsQueryable();
            return Json(data.KGrid());
        }

        public JsonResult SfmSalesTeamHeaderNew()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = ParamDealerCode; // Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var groupNoNew = ParamGroupNoNew;

            var data = ctx.Database.SqlQuery<SalesTeamHeader>("exec uspfn_InqSalesTeamHeaderNew @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @GroupNoNew=@p3", 
                groupArea, companyCode, branchCode, groupNoNew).AsQueryable();
            return Json(data.KGrid());
        }

        public JsonResult SfmSalesTeam()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var data = ctx.Database.SqlQuery<SalesTeam>("exec uspfn_InqSalesTeam @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2", groupArea, companyCode, branchCode).AsQueryable();
            return Json(data);
        }

        public JsonResult SfmSalesTeamNew()
        {
            string groupArea = Request["GroupArea"];
            string companyCode = Request["CompanyCode"];
            string branchCode = Request["BranchCode"];
            var groupNoNew = Request["GroupNoNew"]; ;

            var data = ctx.Database.SqlQuery<SalesTeam>("exec uspfn_InqSalesTeamNew @GroupArea=@p0, @CompanyCode=@p1, @BranchCode=@p2, @GroupNoNew=@p3", 
                groupArea, companyCode, branchCode, groupNoNew).AsQueryable();
            return Json(data);
        }

        public JsonResult Consolidation()
        {
            string area = Request["GroupArea"] ?? "--";
            string comp = Request["CompanyCode"] ?? "--";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_SummaryOfValidEmployee";
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_SummaryOfValidEmployee_New";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@AreaCode", area);
            //cmd.Parameters.AddWithValue("@CompanyCode", comp);
            cmd.Parameters.AddWithValue("@Company", comp);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult PmExecSummary()
        {
            var qry = ctx.PmExecSummaries;
            var take = Request.Params["take"];
            if (take == null)
            {
                return Json(new { header = qry.Where(p => p.FieldType == "H").OrderBy(p => p.Sequence), detail = qry.Where(p => p.FieldType == "D").OrderBy(p => p.Sequence) });
            }
            else
            {
                return Json(qry.KGrid());
            }
        }

        public JsonResult PmDashboardData()
        {
            var qry = ctx.PmDashboardDatas;
            var name = Request.Params["name"];
            return Json(qry.Where(p => p.DashboardName == name).OrderBy(p => p.GroupType).ThenBy(p => p.GroupSeq));
        }

        public JsonResult PmDashboardByDay()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_PmDashboardByDay";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Periode", Request.Params["Periode"]);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult PmExecSummary4()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_GetTotalInquiryAndSpk";
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult PmExecSummary5()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_GetTotalInquiryAndSpk2";
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult PmDashboardByDay2()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_PmDashboardByDay2";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Periode1", Request.Params["Periode1"]);
            cmd.Parameters.AddWithValue("Periode2", Request.Params["Periode2"]);
            cmd.Parameters.AddWithValue("GroupModel", Request.Params["GroupModel"]);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult PmMonitoring()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_PmMonitoring";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Periode", Request.Params["Periode"]);
            cmd.Parameters.AddWithValue("InquiryType", Request.Params["InquiryType"]);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult PmExecSummaryByMonth()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_PmExecSummaryByMonth";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult PmDataIts()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_InqDataIts";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult PmItsByLeadTime()
        {
            //var qry = from p in ctx.PmItsByLeadTimes
            //          select new
            //          {
            //              p.Area,
            //              p.CompanyCode,
            //              p.BranchCode,
            //              p.InquiryNumber,
            //              p.InquiryDate,
            //              p.DealerAbbreviation,
            //              p.OutletAbbreviation,
            //              p.TipeKendaraan,
            //              p.Variant,
            //              p.Transmisi,
            //              p.Period,
            //              p.PDate,
            //              p.HPDate,
            //              p.SPKDate,
            //              p.LeadTimeHp,
            //              p.LeadTimeSpk,
            //              p.LastProgress,
            //              p.CreatedDate,
            //              p.CreatedBy,
            //              p.LastUpdateDate,
            //              p.LastUpdateBy
            //          };

            //var date1 = Request.Params["DateFrom"];
            //var date2 = Request.Params["DateTo"];
            //var area = Request.Params["Area"];
            //var groupNo = "";
            //var dealer = Request.Params["Dealer"];
            //if (dealer.IndexOf("|") > 0)
            //{
            //    string[] xCode = dealer.Split('|');
            //    groupNo = xCode[0];
            //    dealer = xCode[1];
            //}
            //else
            //{
            //    //if (dealer == "")
            //    //{
            //    //    groupNo = area;
            //    //}
            //}

            //var outlet = Request.Params["Outlet"];

            //if (!string.IsNullOrWhiteSpace(date1))
            //{
            //    var DateFrom = Convert.ToDateTime(date1).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InquiryDate) >= DateFrom);
            //}
            //if (!string.IsNullOrWhiteSpace(date2))
            //{
            //    var DateTo = Convert.ToDateTime(date2).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InquiryDate) <= DateTo);
            //}

            //if (!string.IsNullOrWhiteSpace(groupNo)) qry = qry.Where(p => p.Area == groupNo);
            //if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.CompanyCode == dealer);
            //if (!string.IsNullOrWhiteSpace(outlet)) qry = qry.Where(p => p.BranchCode == outlet);

            //return Json(qry.KGrid());
            string areaRaw = Request.Params["Area"] ?? "";
            string dealer = Request.Params["Dealer"] ?? "";
            string outlet = Request.Params["Outlet"] ?? "";
            string dateFrom = Convert.ToDateTime(Request.Params["DateFrom"]).ToString("yyyyMMdd");
            string dateTo = Convert.ToDateTime(Request.Params["DateTo"]).ToString("yyyyMMdd");

            string groupNo = "";
            if (dealer.IndexOf("|") > 0)
            {
                string[] dd = dealer.Split('|');
                groupNo = dd[0];
                dealer = dd[1];
            }

            string query = "";
            if (!string.IsNullOrWhiteSpace(outlet))
            {
                query = String.Format(@"select a.Area, a.CompanyCode, a.BranchCode, a.InquiryNumber, a.InquiryDate, a.DealerAbbreviation, a.OutletAbbreviation,
                                    a.TipeKendaraan, a.Variant, a.Transmisi, a.Period, a.PDate, a.HPDate, a.SPKDate, a.LeadTimeHp, a.LeadTimeSpk, 
                                    a.LastProgress, a.CreatedDate, a.CreatedBy, a.LastUpdateDate, a.LastUpdateBy from PmItsByLeadTime a 
                                    inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}', '{1}', '{2}') o
	                                on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode and o.OutletCode ='{3}'
                                    where convert(varchar, a.InquiryDate, 112) between '{4}' and '{5}'", areaRaw, dealer, groupNo, outlet, dateFrom, dateTo);

            }
            else
            {
                                query = String.Format(@"select a.Area, a.CompanyCode, a.BranchCode, a.InquiryNumber, a.InquiryDate, a.DealerAbbreviation, a.OutletAbbreviation,
                                    a.TipeKendaraan, a.Variant, a.Transmisi, a.Period, a.PDate, a.HPDate, a.SPKDate, a.LeadTimeHp, a.LeadTimeSpk, 
                                    a.LastProgress, a.CreatedDate, a.CreatedBy, a.LastUpdateDate, a.LastUpdateBy from PmItsByLeadTime a 
                                    inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}', '{1}', '{2}') o
	                                on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode
                                    where convert(varchar, a.InquiryDate, 112) between '{3}' and '{4}'", areaRaw, dealer, groupNo, dateFrom, dateTo);
            }


            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<PmItsByLeadTime>(query).AsQueryable();
            
            return Json(data.KGrid());
        }

        public JsonResult PmItsByTestDrive()
        {
            //var qry = from p in ctx.PmItsByTestDrives
            //          select new
            //          {
            //              p.Area,
            //              p.CompanyCode,
            //              p.BranchCode,
            //              p.InquiryNumber,
            //              p.DealerAbbreviation,
            //              p.OutletAbbreviation,
            //              p.InquiryDate,
            //              p.SPKDate,
            //              p.TipeKendaraan,
            //              p.Variant,
            //              p.ColourCode,
            //              p.Transmisi,
            //              p.Inq,
            //              p.InqTestDrive,
            //              p.OutsSPK,
            //              p.OutsSPKTestDrive,
            //              p.NewSPK,
            //              p.NewSPKTestDrive,
            //              p.TotalSPK,
            //              p.TotalSPKTestDrive,
            //          };

            //var date1 = Request.Params["DateFrom"];
            //var date2 = Request.Params["DateTo"];
            //var area = Request.Params["Area"];
            //var groupNo = "";
            //var dealer = Request.Params["Dealer"];
            //if (dealer.IndexOf("|") > 0)
            //{
            //    string[] xCode = dealer.Split('|');
            //    groupNo = xCode[0];
            //    dealer = xCode[1];
            //}
            //else
            //{
            //    //if (dealer=="")
            //    //{
            //    //    groupNo = area;
            //    //}
            //}
            //var outlet = Request.Params["Outlet"];

            //if (!string.IsNullOrWhiteSpace(date1))
            //{
            //    var DateFrom = Convert.ToDateTime(date1).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InquiryDate) >= DateFrom);
            //}
            //if (!string.IsNullOrWhiteSpace(date2))
            //{
            //    var DateTo = Convert.ToDateTime(date2).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InquiryDate) <= DateTo);
            //}

            //if (!string.IsNullOrWhiteSpace(groupNo)) qry = qry.Where(p => p.Area == groupNo);
            //if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.CompanyCode == dealer);
            //if (!string.IsNullOrWhiteSpace(outlet)) qry = qry.Where(p => p.BranchCode == outlet);

            //return Json(qry.KGrid());

            string areaRaw = Request.Params["Area"] ?? "";
            string dealer = Request.Params["Dealer"] ?? "";
            string outlet = Request.Params["Outlet"] ?? "";
            string dateFrom = Convert.ToDateTime(Request.Params["DateFrom"]).ToString("yyyyMMdd");
            string dateTo = Convert.ToDateTime(Request.Params["DateTo"]).ToString("yyyyMMdd");

            string groupNo = "";
            if (dealer.IndexOf("|") > 0)
            {
                string[] dd = dealer.Split('|');
                groupNo = dd[0];
                dealer = dd[1];
            }

            string query = "";
            if (!string.IsNullOrWhiteSpace(outlet))
            {
                query = String.Format(@"select a.Area, a.CompanyCode, a.BranchCode, a.InquiryNumber, a.DealerAbbreviation, a.OutletAbbreviation, a.InquiryDate,
                                    a.SPKDate, a.TipeKendaraan, a.Variant, a.ColourCode, a.Transmisi, a.Inq, a.InqTestDrive, a.OutsSPK, a.OutsSPKTestDrive, a.NewSPK, a.NewSPKTestDrive, 
                                    a.TotalSPK, a.TotalSPKTestDrive from PmItsByTestDrive a 
                                    inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}', '{1}', '{2}') o
	                                on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode and o.OutletCode ='{3}'
                                    where convert(varchar, a.InquiryDate, 112) between '{4}' and '{5}'", areaRaw, dealer, groupNo, outlet, dateFrom, dateTo);
            }
            else
            {
                query = String.Format(@"select a.Area, a.CompanyCode, a.BranchCode, a.InquiryNumber, a.DealerAbbreviation, a.OutletAbbreviation, a.InquiryDate,
                                    a.SPKDate, a.TipeKendaraan, a.Variant, a.ColourCode, a.Transmisi, a.Inq, a.InqTestDrive, a.OutsSPK, a.OutsSPKTestDrive, a.NewSPK, a.NewSPKTestDrive, 
                                    a.TotalSPK, a.TotalSPKTestDrive from PmItsByTestDrive a  
                                    inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}', '{1}', '{2}') o
	                                on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode
                                    where convert(varchar, a.InquiryDate, 112) between '{3}' and '{4}'", areaRaw, dealer, groupNo, dateFrom, dateTo);
            }


            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<PmItsByTestDrive2>(query).AsQueryable();

            return Json(data.KGrid());
        }

        public JsonResult PmItsByLostCase()
        {
            string filter = Request["Filter"] ?? "";
            string areaRaw = Request["Area"] ?? "";
            string dealer = Request["Dealer"] ?? "";
            string outlet = Request["Outlet"] ?? "";
            string dateFrom = Convert.ToDateTime(Request["DateFrom"]).ToString("yyyyMMdd");
            string dateTo = Convert.ToDateTime(Request["DateTo"]).ToString("yyyyMMdd");

            string groupNo = "";
            if (dealer.IndexOf("|") > 0)
            {
                string[] dd = dealer.Split('|');
                groupNo = dd[0];
                dealer = dd[1];
            }

            if (filter == "0")
            {
                string query = "";

                if (!string.IsNullOrWhiteSpace(outlet))
                {
                    query = String.Format(@"select a.* from PmITSByLostCase0 a with(nolock, nowait) 
                                inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}','{1}','{2}') 
                                o on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode and o.OutletCode = '{3}'
                                where  CONVERT(varchar,a.InquiryDate, 112) between '{4}' and '{5}'", areaRaw, dealer, groupNo, outlet, dateFrom, dateTo);
                }
                else
                {
                    query = String.Format(@"select a.* from PmITSByLostCase0 a with(nolock, nowait) 
                                inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}','{1}','{2}') 
                                o on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode
                                where CONVERT(varchar,a.InquiryDate, 112) between '{3}' and '{4}'", areaRaw, dealer, groupNo, dateFrom, dateTo);
                }              
                //BETWEEN '" + dateFrom + "' AND '" + dateTo + "'";
                //if (!string.IsNullOrWhiteSpace(groupNo)) query += " and AreaCode = " + Convert.ToInt32(groupNo);
                //if (!string.IsNullOrWhiteSpace(dealer)) query += " and CompanyCode = " + dealer;
                //if (!string.IsNullOrWhiteSpace(outlet)) query += " and BranchCode = " + outlet;

                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<PmITSByLostCase0>(query).AsQueryable();

                //var data = ctx.PmITSByLostCase0.AsQueryable();

                //if (dateFrom != null && dateTo != null)
                //{
                //    data = data.Where(x => x.InquiryDate >= dateFrom && x.InquiryDate <= dateTo);
                //}

                //if (!string.IsNullOrWhiteSpace(areaRaw))
                //{
                //    int? area = Convert.ToInt32(areaRaw);
                //    data = data.Where(x => x.AreaCode == area);
                //}

                //if (!string.IsNullOrWhiteSpace(dealer))
                //{
                //    data = data.Where(x => x.CompanyCode == dealer);
                //}

                //if (!string.IsNullOrWhiteSpace(outlet))
                //{
                //    data = data.Where(x => x.BranchCode == outlet);
                //}

                return Json(data.KGrid());
            }
            else if (filter == "1")
            {
                string query = "";
                if (!string.IsNullOrWhiteSpace(outlet))
                {
                    query = String.Format(@"select a.* from PmITSByLostCase1 a with(nolock, nowait) 
                                inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}','{1}','{2}') 
                                o on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode and o.OutletCode = '{3}'
                                where  CONVERT(varchar,a.LostCaseDate, 112) between '{4}' and '{5}'", areaRaw, dealer, groupNo, outlet, dateFrom, dateTo);
                }
                else
                {
                    query = String.Format(@"select a.* from PmITSByLostCase1 a with(nolock, nowait) 
                                inner join dbo.DealerMappingNewJoinOutletMappingNew('{0}','{1}','{2}') 
                                o on o.DealerCode = a.CompanyCode and o.OutletCode = a.BranchCode
                                where  CONVERT(varchar,a.LostCaseDate, 112) between '{3}' and '{4}'", areaRaw, dealer, groupNo, dateFrom, dateTo);
                }              

                //string query = "select * from PmITSByLostCase1 with(nolock, nowait) where CONVERT(date,LostCaseDate) BETWEEN '" + dateFrom + "' AND '" + dateTo + "'";

                //if (!string.IsNullOrWhiteSpace(groupNo)) query += " and AreaCode = " + Convert.ToInt32(groupNo);
                //if (!string.IsNullOrWhiteSpace(dealer)) query += " and CompanyCode = " + dealer;
                //if (!string.IsNullOrWhiteSpace(outlet)) query += " and BranchCode = " + outlet;

                ctx.Database.CommandTimeout = 3600;
                var data = ctx.Database.SqlQuery<PmITSByLostCase1>(query).AsQueryable();

                //var data = ctx.PmITSByLostCase1.AsQueryable();

                //if (dateFrom != null && dateTo != null)
                //{
                //    data = data.Where(x => x.LostCaseDate >= dateFrom && x.LostCaseDate <= dateTo);
                //}

                //if (!string.IsNullOrWhiteSpace(areaRaw))
                //{
                //    int? area = Convert.ToInt32(areaRaw);
                //    data = data.Where(x => x.AreaCode == area);
                //}

                //if (!string.IsNullOrWhiteSpace(dealer))
                //{
                //    data = data.Where(x => x.CompanyCode == dealer);
                //}

                //if (!string.IsNullOrWhiteSpace(outlet))
                //{
                //    data = data.Where(x => x.BranchCode == outlet);
                //}

                return Json(data.KGrid());
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PmItsByPerolehanData()
        {
            //string filter = Request["Filter"] ?? "";
            //string areaRaw = Request["Area"] ?? "0";
            //int? area = null;

            //if (!string.IsNullOrWhiteSpace(areaRaw))
            //{
            //    area = Convert.ToInt32(areaRaw);
            //}

            //string dealer = Request["Dealer"] ?? "";
            //string outlet = Request["Outlet"] ?? "";
            //DateTime dateFrom = Convert.ToDateTime(Request["DateFrom"]);
            //DateTime dateTo = Convert.ToDateTime(Request["DateTo"]);

            //if (filter == "0")
            //{
            //    var data = ctx.PmITSByPerolehanData0.AsQueryable();

            //    if (dateFrom != null && dateTo != null)
            //    {
            //        data = data.Where(x => x.InquiryDate >= dateFrom && x.InquiryDate <= dateTo);
            //    }

            //    if (area != null)
            //    {
            //        data = data.Where(x => x.AreaCode == area);
            //    }

            //    if (!string.IsNullOrWhiteSpace(dealer))
            //    {
            //        data = data.Where(x => x.CompanyCode == dealer);
            //    }

            //    if (!string.IsNullOrWhiteSpace(outlet))
            //    {
            //        data = data.Where(x => x.BranchCode == outlet);
            //    }

            //    return Json(data.KGrid());
            //}
            //else if (filter == "1")
            //{
            //    var data = ctx.PmITSByPerolehanData1.AsQueryable();

            //    if (dateFrom != null && dateTo != null)
            //    {
            //        data = data.Where(x => x.UpdateDate >= dateFrom && x.UpdateDate <= dateTo);
            //    }

            //    if (area != null)
            //    {
            //        data = data.Where(x => x.AreaCode == area);
            //    }

            //    if (!string.IsNullOrWhiteSpace(dealer))
            //    {
            //        data = data.Where(x => x.CompanyCode == dealer);
            //    }

            //    if (!string.IsNullOrWhiteSpace(outlet))
            //    {
            //        data = data.Where(x => x.BranchCode == outlet);
            //    }

            //    return Json(data.KGrid());
            //}

            //return Json(null);

            var filter = Request.Params["Filter"];
            var period1 = Request.Params["DateFrom"];
            var period2 = Request.Params["DateTo"];
            var area = Request.Params["Area"];
            var company = Request.Params["Dealer"];
            var outlet = Request.Params["Outlet"];

            string query = " exec uspfn_PmITSByPerolehanData_Grid_New '" + filter + "','" + period1 + "','" + period2 + "','" + area + "','" + company + "','" + outlet + "'";
            ctx.Database.CommandTimeout = 3600;


            if (filter=="0")
            {
                var data = ctx.Database.SqlQuery<PmITSByPerolehanData0>(query).AsQueryable();
                return Json(data.KGrid());
            }
            else if (filter == "1")
            {
                var data = ctx.Database.SqlQuery<PmITSByPerolehanData1>(query).AsQueryable();
                return Json(data.KGrid());
            }

            return Json(null);
        }

        public JsonResult MonitoringProductivity()
        {
            string companyCode = Request["CompanyCode"] ?? "";
            string branchCode = Request["BranchCode"] ?? "";
            string Period = Request["Period"] ?? "";

            SqlCommand cmd = ctxR4.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_MonitoringProductivity";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            cmd.Parameters.AddWithValue("@BranchCode", branchCode);
            cmd.Parameters.AddWithValue("@PeriodDate", Period);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult InqDataSdms()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_InqDataSdms";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("TableName", Request["TableName"] ?? "");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult InqDataMp()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_InqDataMp";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("TableName", Request["TableName"] ?? "");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult PartSalesViews()
        {
            var qry = ctx.PartSalesViews.AsQueryable();

            var date1 = Request.Params["DateFrom"];
            var date2 = Request.Params["DateTo"];
            var area = Request.Params["Area"];
            var dealer = Request.Params["Dealer"];
            var tpgo = Request.Params["TypeOfGoods"];

            if (!string.IsNullOrWhiteSpace(date1))
            {
                var DateFrom = Convert.ToDateTime(date1).Date;
                qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) >= DateFrom);
            }
            if (!string.IsNullOrWhiteSpace(date2))
            {
                var DateTo = Convert.ToDateTime(date2).Date;
                qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) <= DateTo);
            }

            if (!string.IsNullOrWhiteSpace(area)) qry = qry.Where(p => p.Area == area);
            if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.DealerCode == dealer);
            if (!string.IsNullOrWhiteSpace(tpgo)) qry = qry.Where(p => p.TypeOfGoods == tpgo);

            return Json(qry.KGrid());
        }

        public JsonResult HistPartSales()
        {
            var qry = from p in ctx.SpHstPartSalesList
                      join q in ctx.GnMstDealerMappings on p.CompanyCode equals q.DealerCode
                      join r in ctx.GnMstDealerOutletMappings on
                      new { CompanyCode = p.CompanyCode, BranchCode = p.BranchCode } equals
                      new { CompanyCode = r.DealerCode, BranchCode = r.OutletCode }
                      select new
                      {
                          p.RecordID,
                          p.RecordDate,
                          p.CompanyCode,
                          p.BranchCode,
                          p.InvoiceNo,
                          p.InvoiceDate,
                          p.FPJNo,
                          p.FPJDate,
                          p.CustomerCode,
                          p.CustomerName,
                          p.CustomerClass,
                          p.PartNo,
                          p.PartName,
                          p.TypeOfGoods,
                          p.TypeOfGoodsDesc,
                          p.QtyBill,
                          p.CostPrice,
                          p.RetailPrice,
                          p.DiscPct,
                          p.DiscAmt,
                          p.NetSalesAmt,
                          q.Area,
                          q.DealerCode,
                          q.DealerAbbreviation,
                          r.OutletAbbreviation,
                      };

            var date1 = Request.Params["DateFrom"];
            var date2 = Request.Params["DateTo"];
            var area = Request.Params["Area"];
            var dealer = Request.Params["Dealer"];
            var tpgo = Request.Params["TypeOfGoods"];

            if (!string.IsNullOrWhiteSpace(date1))
            {
                var DateFrom = Convert.ToDateTime(date1).Date;
                qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) >= DateFrom);
            }
            if (!string.IsNullOrWhiteSpace(date2))
            {
                var DateTo = Convert.ToDateTime(date2).Date;
                qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) <= DateTo);
            }

            if (!string.IsNullOrWhiteSpace(area)) qry = qry.Where(p => p.Area == area);
            if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.DealerCode == dealer);
            if (!string.IsNullOrWhiteSpace(tpgo)) qry = qry.Where(p => p.TypeOfGoods == tpgo);

            return Json(qry.KGrid());
        }

        public JsonResult HistPartSales4W()
        {
            //var qry = from p in ctx.SpHstPartSalesList
            //          join q in ctx.GnMstDealerMappings on p.CompanyCode equals q.DealerCode
            //          join r in ctx.OutletInfos on
            //          new { CompanyCode = p.CompanyCode, BranchCode = p.BranchCode } equals
            //          new { CompanyCode = r.CompanyCode, BranchCode = r.BranchCode }
            //          join s in ctx.DealerInfos on p.CompanyCode equals s.DealerCode
            //          where s.ProductType == "4W"
            //          select new
            //          {
            //              p.RecordID,
            //              p.RecordDate,
            //              p.CompanyCode,
            //              p.BranchCode,
            //              p.InvoiceNo,
            //              p.InvoiceDate,
            //              p.FPJNo,
            //              p.FPJDate,
            //              p.CustomerCode,
            //              p.CustomerName,
            //              p.CustomerClass,
            //              p.PartNo,
            //              p.PartName,
            //              p.TypeOfGoods,
            //              p.TypeOfGoodsDesc,
            //              p.QtyBill,
            //              p.CostPrice,
            //              p.RetailPrice,
            //              p.DiscPct,
            //              p.DiscAmt,
            //              p.NetSalesAmt,
            //              q.Area,
            //              q.DealerCode,
            //              q.DealerAbbreviation,
            //              r.BranchName,
            //              s.DealerName
            //          };

            //var date1 = Request.Params["DateFrom"];
            //var date2 = Request.Params["DateTo"];
            //var area = Request.Params["Area"];
            //var dealer = Request.Params["Dealer"];
            //var tpgo = Request.Params["TypeOfGoods"];

            //if (!string.IsNullOrWhiteSpace(date1))
            //{
            //    var DateFrom = Convert.ToDateTime(date1).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) >= DateFrom);
            //}
            //if (!string.IsNullOrWhiteSpace(date2))
            //{
            //    var DateTo = Convert.ToDateTime(date2).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) <= DateTo);
            //}

            //if (!string.IsNullOrWhiteSpace(area)) qry = qry.Where(p => p.Area == area);
            //if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.DealerCode == dealer);
            //if (!string.IsNullOrWhiteSpace(tpgo)) qry = qry.Where(p => p.TypeOfGoods == tpgo);

            //return Json(qry.KGrid());

            var date1 = Request.Params["DateFrom"];
            var date2 = Request.Params["DateTo"];
            var area = Request.Params["Area"];
            var dealer = Request.Params["Dealer"];
            var tpgo = Request.Params["TypeOfGoods"];

            string query = " exec uspfn_GeneratePartSales_New '" + date1 + "','" + date2 + "','" + area + "','" + dealer + "','" + tpgo + "'";
            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<GenPartSales>(query).AsQueryable();

            return Json(data.KGrid());
        }

        public JsonResult HistPartSales2W()
        {
            //var qry = from p in ctx.SpHstPartSalesList
            //          join q in ctx.GnMstDealerMappings on p.CompanyCode equals q.DealerCode
            //          join r in ctx.OutletInfos on
            //          new { CompanyCode = p.CompanyCode, BranchCode = p.BranchCode } equals
            //          new { CompanyCode = r.CompanyCode, BranchCode = r.BranchCode }
            //          join s in ctx.DealerInfos on p.CompanyCode equals s.DealerCode
            //          where s.ProductType == "2W"
            //          select new
            //          {
            //              p.RecordID,
            //              p.RecordDate,
            //              p.CompanyCode,
            //              p.BranchCode,
            //              p.InvoiceNo,
            //              p.InvoiceDate,
            //              p.FPJNo,
            //              p.FPJDate,
            //              p.CustomerCode,
            //              p.CustomerName,
            //              p.CustomerClass,
            //              p.PartNo,
            //              p.PartName,
            //              p.TypeOfGoods,
            //              p.TypeOfGoodsDesc,
            //              p.QtyBill,
            //              p.CostPrice,
            //              p.RetailPrice,
            //              p.DiscPct,
            //              p.DiscAmt,
            //              p.NetSalesAmt,
            //              q.Area,
            //              q.DealerCode,
            //              q.DealerAbbreviation,
            //              r.BranchName,
            //              OutletAbbreviation = r.ShortBranchName,
            //              s.DealerName,
            //              s.ShortName
            //          };

            //var date1 = Request.Params["DateFrom"];
            //var date2 = Request.Params["DateTo"];
            //var area = Request.Params["Area"];
            //var dealer = Request.Params["Dealer"];
            //var tpgo = Request.Params["TypeOfGoods"];

            //if (!string.IsNullOrWhiteSpace(date1))
            //{
            //    var DateFrom = Convert.ToDateTime(date1).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) >= DateFrom);
            //}
            //if (!string.IsNullOrWhiteSpace(date2))
            //{
            //    var DateTo = Convert.ToDateTime(date2).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.InvoiceDate) <= DateTo);
            //}

            //if (!string.IsNullOrWhiteSpace(area)) qry = qry.Where(p => p.Area == area);
            //if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.DealerCode == dealer);
            //if (!string.IsNullOrWhiteSpace(tpgo)) qry = qry.Where(p => p.TypeOfGoods == tpgo);

            //return Json(qry.KGrid());

            var date1 = Request.Params["DateFrom"];
            var date2 = Request.Params["DateTo"];
            var area = Request.Params["Area"];
            var dealer = Request.Params["Dealer"];
            var tpgo = Request.Params["TypeOfGoods"];

            string query = " exec uspfn_GeneratePartSales2W_New '" + date1 + "','" + date2 + "','" + area + "','" + dealer + "','" + tpgo + "'";
            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<GenPartSales>(query).AsQueryable();

            return Json(data.KGrid());
        }

        public JsonResult UnitIntake(string DateFrom, string DateTo)
        {
            var data = ctx.SvUnitIntakeViews.AsQueryable();

            var area = Request.Params["Area"];
            var dealer = Request.Params["Dealer"];
            var outlet = Request.Params["Outlet"];

            var novin = Request.Params["NOVIN"];
            var nopol = Request.Params["NOPOL"];
            var pelanggan = Request.Params["PELANGGAN"];
            var rework = Request.Params["Rework"];

            string query = "select * from SvUnitIntakeView with(nolock, nowait) where 1 = 1";
            //int areaCode = 0;
            //try
            //{
            //    areaCode = Convert.ToInt32(area);
            //}
            //catch (Exception)
            //{
            //}

            //if (!string.IsNullOrWhiteSpace(area)) data = data.Where(p => p.GroupNo == area);
            //if (!string.IsNullOrWhiteSpace(dealer)) data = data.Where(p => p.CompanyCode == dealer);
            ////if (!string.IsNullOrWhiteSpace(outlet)) data = data.Where(p => p.BranchCode == outlet.Substring(6));
            //if (!string.IsNullOrWhiteSpace(outlet)) data = data.Where(p => p.OutletCode == outlet);
            //if (!string.IsNullOrWhiteSpace(novin)) data = data.Where(p => p.VinNo.Contains(novin));
            //if (!string.IsNullOrWhiteSpace(nopol)) data = data.Where(p => p.PoliceRegNo.Contains(nopol));
            //if (!string.IsNullOrWhiteSpace(pelanggan)) data = data.Where(p => p.CustomerName.Contains(pelanggan));

            //if (DateFrom != null && DateTo != null)
            //{
            //    data = data.Where(x => x.JobOrderClosed >= EntityFunctions.TruncateTime(DateFrom) && EntityFunctions.TruncateTime(x.JobOrderClosed) <= DateTo);
            //}

            if (!string.IsNullOrWhiteSpace(area)) query += " and GroupNo = " + area;
            if (!string.IsNullOrWhiteSpace(dealer)) query += " and GNDealerCode = '" + dealer + "'";
            //if (!string.IsNullOrWhiteSpace(outlet)) data = data.Where(p => p.BranchCode == outlet.Substring(6));
            if (!string.IsNullOrWhiteSpace(outlet)) query += " and GNOutletCode = '" + outlet + "'";
            if (!string.IsNullOrWhiteSpace(novin)) query += " and VinNo like '%" + novin + "%'";
            if (!string.IsNullOrWhiteSpace(nopol)) query += " and PoliceRegNo like '%" + nopol + "%'";
            if (!string.IsNullOrWhiteSpace(pelanggan)) query += " and CustomerName like '%" + pelanggan + "%'";
            if (!string.IsNullOrWhiteSpace(rework)) query += " and JobType='Rework'";

            if (!string.IsNullOrWhiteSpace(DateFrom) && !string.IsNullOrWhiteSpace(DateTo)) { query += " and left(convert(nvarchar, JobOrderClosed, 121), 10) >= '" + DateFrom + "' and left(convert(nvarchar, JobOrderClosed, 121), 10) <= '" + DateTo + "'"; }

            ctx.Database.CommandTimeout = 3600;
            var data2 = ctx.Database.SqlQuery<SvUnitIntakeView>(query).AsQueryable();

            return Json(data2.KGrid());
        }

        public JsonResult UnitIntakeR2(DateTime? DateFrom, DateTime? DateTo)
        {
            var data = ctx.UnitIntakeViews.AsQueryable();

            var area = Request.Params["Area"];
            var dealer = Request.Params["Dealer"];
            var outlet = Request.Params["Outlet"];

            var novin = Request.Params["NOVIN"];
            var nopol = Request.Params["NOPOL"];
            var pelanggan = Request.Params["PELANGGAN"];

            int areaCode = 0;
            try
            {
                areaCode = Convert.ToInt32(area);
            }
            catch (Exception)
            {
            }

            if (!string.IsNullOrWhiteSpace(area)) data = data.Where(p => p.AreaCode == areaCode);
            if (!string.IsNullOrWhiteSpace(dealer)) data = data.Where(p => p.CompanyCode == dealer);
            if (!string.IsNullOrWhiteSpace(outlet)) data = data.Where(p => p.BranchCode == outlet);

            if (!string.IsNullOrWhiteSpace(novin)) data = data.Where(p => p.VinNo.Contains(novin));
            if (!string.IsNullOrWhiteSpace(nopol)) data = data.Where(p => p.PoliceRegNo.Contains(nopol));
            if (!string.IsNullOrWhiteSpace(pelanggan)) data = data.Where(p => p.CustomerName.Contains(pelanggan));

            if (DateFrom != null && DateTo != null)
            {
                data = data.Where(x => x.JobOrderClosed >= EntityFunctions.TruncateTime(DateFrom) && EntityFunctions.TruncateTime(x.JobOrderClosed) <= DateTo);
            }

            return Json(data.KGrid());
        }
        public JsonResult UnitIntakeXlsR2()
        {
            DataTable dt = new DataTable();
            string Area = Request["Area"];
            string Dealer = Request["Dealer"];
            string Outlet = Request["Outlet"];
            string DateFrom = Request["DateFrom"];
            string DateTo = Request["DateTo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_SvGenerateUnitIntake";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Area", Area);
            cmd.Parameters.AddWithValue("Dealer", (Dealer == "") ? Dealer : Dealer);
            cmd.Parameters.AddWithValue("Outlet", (Outlet == "") ? Outlet : Outlet);
            cmd.Parameters.AddWithValue("DateFrom", DateFrom);
            cmd.Parameters.AddWithValue("DateTo", DateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Report Name", "Service Unit Intake" });
            header.Add(new List<dynamic> { "Area", (Area == "") ? "ALL" : Request["AreaName"] });
            header.Add(new List<dynamic> { "Dealer", (Dealer == "") ? "ALL" : ctx.DealerInfos.Find(new string[] { Dealer }).DealerName });
            header.Add(new List<dynamic> { "Outlet", (Outlet == "") ? "ALL" : ctx.OutletInfos.Where(p => p.BranchCode == Outlet).FirstOrDefault().BranchName });
            header.Add(new List<dynamic> { "Periode", string.Format("'{0} s.d {1}", Convert.ToDateTime(DateFrom).ToString("dd-MMM-yyy"), Convert.ToDateTime(DateTo).ToString("dd-MMM-yyy")) });

            var format = new List<dynamic>() {
                new { name = "Area", text = "Area (Service)" , width = 20 },
                new { name = "CompanyCode", text = "Kode Dealer (Service)" , width = 20 },
                new { name = "CompanyName", text = "Nama Dealer (Service)" , width = 60 },
                new { name = "BranchCode", text = "Kode Outlet (Service)" , width = 20 },
                new { name = "BranchName", text = "Nama Outlet (Service)" , width = 80 },
                new { name = "VinNo", text= "No Vin", width = 20 },
                new { name = "JobOrderClosed", text="Tanggal Tutup SPK", width = 20 },
                new { name = "DealerCode", text="Kode Dealer (Purchase)", width = 20  },
                new { name = "DealerName", text="Nama Dealer (Purchase)", width = 80  },
                new { name = "Odometer", text = "KM", width= 20 },
                new { name = "SalesModelDesc", text =  "Tipe Kendaraan" , width = 20},
                new { name = "SalesModelCode", text = "Sales Model Code" , width = 20},
                new { name = "BasicModel", text = "Basic Model" , width = 20},
                new { name = "ProductionYear", text = "Tahun Produksi" , width = 20},
                new { name = "DoDate", text = "Tanggal Pembelian" , width = 20},
                new { name = "PoliceRegNo", text = "No. Polisi" , width = 20},
                new { name = "EngineNo", text = "No. Mesin" , width = 10},
                new { name = "ChassisNo", width = 20, text = "No. Rangka" },
                new { name = "CustomerName", width = 80, text = "Nama Pelanggan" },
                new { name = "PhoneNo", width = 20, text = "No. Telp. Rumah" },
                new { name = "OfficePhoneNo", width = 20, text = "No. Telp. Kantor" },
                new { name = "HPNo", width = 20, text = "No. HP" },
                new { name = "ContactName", width = 80, text = "Additional Contact" },
                new { name = "Email", width = 30, text = "Email" },
                new { name = "BirthDate", width = 20, text = "Tanggal Lahir" },
                new { name = "Gender", width = 20, text = "Jenis Kelamin" },
                new { name = "Address", width = 120, text = "Alamat" },
                new { name = "GroupJobTypeDesc", width = 50, text = "Jenis Service" },
                new { name = "JobType", width = 50, text = "Jenis Pekerjaan" },
                new { name = "JobTypeDesc", width = 80, text = "Keterangan" },
                new { name = "SaName", width = 80, text = "Nama SA" },
                new { name = "SaNik", width = 20, text = "NIK SA" },
            };


            return GenerateReportXls(dt, "Service Unit Intake", "SvUnitIntake", header, format: format);
        }


        public JsonResult UnitIntakeXls()
        {
            DataTable dt = new DataTable();
            string Area = Request["Area"];
            string Dealer = Request["Dealer"];
            string Outlet = Request["Outlet"];
            string DateFrom = Request["DateFrom"];
            string DateTo = Request["DateTo"];

            var novin = Request.Params["NOVIN"] ?? "";
            var nopol = Request.Params["NOPOL"] ?? "";
            var pelanggan = Request.Params["PELANGGAN"] ?? "";
            var rework = Request.Params["Rework"] ?? "";

            string jobtype = "";
            if (rework == "on")
            {
                jobtype = "Rework";
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_SrvGenerateUnitIntake";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Area", Area);
            cmd.Parameters.AddWithValue("Dealer", Dealer);
            cmd.Parameters.AddWithValue("Outlet", Outlet);
            //cmd.Parameters.AddWithValue("Dealer", (Dealer == "") ? Dealer : Dealer.Substring(6));
            //cmd.Parameters.AddWithValue("Outlet", (Outlet == "") ? Outlet : Outlet.Substring(6));
            cmd.Parameters.AddWithValue("DateFrom", DateFrom);
            cmd.Parameters.AddWithValue("DateTo", DateTo);
            cmd.Parameters.AddWithValue("novin", novin);
            cmd.Parameters.AddWithValue("nopol", nopol);
            cmd.Parameters.AddWithValue("pelanggan", pelanggan);
            cmd.Parameters.AddWithValue("jobtype", jobtype);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            var header = new List<List<dynamic>>() { };
            header.Add(new List<dynamic> { "Report Name", "Service Unit Intake" });
            header.Add(new List<dynamic> { "Area", (Area == "") ? "ALL" : Request["AreaName"] });
            header.Add(new List<dynamic> { "Dealer", (Dealer == "") ? "ALL" : ctx.svMasterDealerMappings.FirstOrDefault(x => x.DealerCode == Dealer).DealerName });
            header.Add(new List<dynamic> { "Outlet", (Outlet == "") ? "ALL" : ctx.GnMstDealerOutletMappings.Where(p => p.OutletCode == Outlet).FirstOrDefault().OutletName });
            //header.Add(new List<dynamic> { "Dealer", (Dealer == "") ? "ALL" : ctx.DealerInfos.Find(new string[] { Dealer.Substring(6) }).DealerName });
            //header.Add(new List<dynamic> { "Outlet", (Outlet == "") ? "ALL" : ctx.OutletInfos.Where(p => p.BranchCode == Outlet.Substring(6)).FirstOrDefault().BranchName });
            header.Add(new List<dynamic> { "Periode", string.Format("'{0} s.d {1}", Convert.ToDateTime(DateFrom).ToString("dd-MMM-yyy"), Convert.ToDateTime(DateTo).ToString("dd-MMM-yyy")) });

            var format = new List<dynamic>() {
                new { name = "Area", text = "Area (Service)" , width = 20 },
                new { name = "CompanyCode", text = "Kode Dealer (Service)" , width = 20 },
                new { name = "CompanyName", text = "Nama Dealer (Service)" , width = 60 },
                new { name = "BranchCode", text = "Kode Outlet (Service)" , width = 20 },
                new { name = "BranchName", text = "Nama Outlet (Service)" , width = 80 },
                new { name = "VinNo", text= "No Vin", width = 20 },
                new { name = "JobOrderClosed", text="Tanggal Tutup SPK", width = 20 },
                new { name = "DealerCode", text="Kode Dealer (Purchase)", width = 20  },
                new { name = "DealerName", text="Nama Dealer (Purchase)", width = 80  },
                new { name = "Odometer", text = "KM", width= 20 },
                new { name = "SalesModelDesc", text =  "Tipe Kendaraan" , width = 20},
                new { name = "SalesModelCode", text = "Sales Model Code" , width = 20},
                new { name = "BasicModel", text = "Basic Model" , width = 20},
                new { name = "ProductionYear", text = "Tahun Produksi" , width = 20},
                new { name = "DoDate", text = "Tanggal Pembelian" , width = 20},
                new { name = "PoliceRegNo", text = "No. Polisi" , width = 20},
                new { name = "EngineNo", text = "No. Mesin" , width = 10},
                new { name = "ChassisNo", width = 20, text = "No. Rangka" },
                new { name = "CustomerName", width = 80, text = "Nama Pelanggan" },
                new { name = "PhoneNo", width = 20, text = "No. Telp. Rumah" },
                new { name = "OfficePhoneNo", width = 20, text = "No. Telp. Kantor" },
                new { name = "HPNo", width = 20, text = "No. HP" },
                //new { name = "ContactName", width = 80, text = "Additional Contact" },
                new { name = "Email", width = 30, text = "Email" },
                new { name = "BirthDate", width = 20, text = "Tanggal Lahir" },
                new { name = "Gender", width = 20, text = "Jenis Kelamin" },
                new { name = "Address", width = 120, text = "Alamat" },
                new { name = "GroupJobTypeDesc", width = 50, text = "Jenis Service" },
                new { name = "JobType", width = 50, text = "Jenis Pekerjaan" },
                new { name = "JobTypeDesc", width = 80, text = "Keterangan" },
                new { name = "SaName", width = 80, text = "Nama SA" },
                new { name = "SaNik", width = 20, text = "NIK SA" },
            };

            return GenerateReportXls(dt, "Service Unit Intake", "SvUnitIntake", header, format: format);

        }

        public JsonResult SvRegisterSpk()
        {
            //var qry = from p in ctx.SvRegisterSpkList
            //          join q in ctx.GnMstDealerMappings on p.CompanyCode equals q.DealerCode
            //          join r in ctx.DealerInfos on p.CompanyCode equals r.DealerCode
            //          join s in ctx.OutletInfos on p.BranchCode equals s.BranchCode
            //          where p.SupplyQty > 0
            //          //join r in ctx.GnMstDealerOutletMappings on
            //          //new { CompanyCode = p.CompanyCode, BranchCode = p.BranchCode } equals
            //          //new { CompanyCode = r.DealerCode, BranchCode = r.OutletCode }
            //          select new
            //          {
            //              p.CompanyCode, //
            //              p.BranchCode, //
            //              p.ProductType,
            //              p.ServiceNo, //
            //              p.TaskPartNo, //
            //              TaskPartName = (p.TaskPartType == "T") ? p.OperationName : p.PartName, //
            //              p.TaskPartSeq, //
            //              p.JobOrderNo, //
            //              p.JobOrderDate, //
            //              p.BasicModel,
            //              p.PoliceRegNo, //
            //              p.Odometer, //
            //              p.CustomerCode, //
            //              p.CustomerName, //
            //              p.GroupJobType,
            //              p.GroupJobTypeDesc,
            //              p.JobType, //
            //              p.JobTypeDesc, //
            //              p.OperationNo, //
            //              p.OperationName, //
            //              p.OperationHour, //
            //              p.FmID, //
            //              p.FmName, //
            //              p.SaID, //
            //              p.SaName, //
            //              p.MechanicID, //
            //              p.MechanicName, //
            //              p.PartNo, //
            //              p.PartName, //
            //              p.DemandQty, //
            //              p.SupplyQty, //
            //              p.ReturnQty, //
            //              p.SupplySlipNo, //
            //              p.SSReturnNo, //
            //              p.TaskPartType, //
            //              p.ServiceRequestDesc, //
            //              p.ServiceStatus,
            //              p.ServiceStatusDesc,//
            //              p.TotalSrvAmount, //
            //              p.InvoiceNo, //
            //              q.Area,
            //              r.DealerName, //
            //              ShortDealerName = r.ShortName, //
            //              s.BranchName, //
            //              s.ShortBranchName //
            //          };

            //var date1 = Request.Params["DateFrom"];
            //var date2 = Request.Params["DateTo"];
            //var area = Request.Params["AreaName"];
            //var dealer = Request.Params["Dealer"];
            //var outlet = Request.Params["Outlet"];

            //if (!string.IsNullOrWhiteSpace(date1))
            //{
            //    var DateFrom = Convert.ToDateTime(date1).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.JobOrderDate) >= DateFrom);
            //}
            //if (!string.IsNullOrWhiteSpace(date2))
            //{
            //    var DateTo = Convert.ToDateTime(date2).Date;
            //    qry = qry.Where(p => EntityFunctions.TruncateTime(p.JobOrderDate) <= DateTo);
            //}

            //if (!string.IsNullOrWhiteSpace(area)) qry = qry.Where(p => p.Area == area);
            //if (!string.IsNullOrWhiteSpace(dealer)) qry = qry.Where(p => p.CompanyCode == dealer);
            //if (!string.IsNullOrWhiteSpace(outlet)) qry = qry.Where(p => p.BranchCode == outlet);

            //return Json(qry.KGrid());

            var Area = Request["AreaName"];
            var Dealer = Request["Dealer"];
            var Outlet = Request["Outlet"];
            var DateFrom = Convert.ToDateTime(Request["DateFrom"]).ToString("yyyyMMdd");
            var DateTo = Convert.ToDateTime(Request["DateTo"]).ToString("yyyyMMdd");

            if (Area == "-- ALL AREA -- ")
            {
                Area = "";
            }

            if (Dealer == "-- ALL DEALER --")
            {
                Dealer = "";
            }
            if (Outlet == "-- ALL OUTLET --")
            {
                Outlet = "";
            }

            ctx.Database.CommandTimeout = 3600;
            var qry = ctx.Database.SqlQuery<DetailRevenueRegSPK>("exec uspfn_SrvSvRegisterSpk @Area=@p0, @DealerCode=@p1, @OutletCode=@p2, @DateFrom=@p3, @DateTo=@p4", Area, Dealer, Outlet, DateFrom, DateTo).AsQueryable();


            return Json(qry.KGrid());
        }

        public JsonResult SvRegisterSpkXls()
        {
            var Area = Request["AreaName"];
            var Dealer = Request["Dealer"];
            var Outlet = Request["Outlet"];
            var DateFrom = Request["DateFrom"];
            var DateTo = Request["DateTo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_SrvRegisterSpkXls";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("Area", Area);
            cmd.Parameters.AddWithValue("Dealer", Dealer);
            cmd.Parameters.AddWithValue("Outlet", Outlet);
            cmd.Parameters.AddWithValue("DateFrom", DateFrom);
            cmd.Parameters.AddWithValue("DateTo", DateTo);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var header = new List<List<dynamic>>() { };
            if (Dealer == "") header.Add(new List<dynamic> { "AREA", (Area == "") ? "ALL" : Area });
            header.Add(new List<dynamic> { "DEALER", (Dealer == "") ? "ALL" : ctx.DealerInfos.Find(new string[] { Dealer }).DealerName });
            header.Add(new List<dynamic> { "OUTLET", (Outlet == "") ? "ALL" : ctx.OutletInfos.Where(p => p.BranchCode == Outlet).FirstOrDefault().BranchName });
            header.Add(new List<dynamic> { "PERIODE", string.Format("{0} s.d {1}", Convert.ToDateTime(DateFrom).ToString("dd-MMM-yyy"), Convert.ToDateTime(DateTo).ToString("dd-MMM-yyy")) });

            return GenerateReportXls(dt, "RegisterSPK", "RegisterSPK", header);
        }

        public JsonResult SvDtlRevenueRegSpkGrid()
        {
            var Area = Request["AreaName"];
            var Dealer = Request["Dealer"];
            var Outlet = Request["Outlet"];
            var DateFrom = Request["DateFrom"] + " 00:00:00";
            var DateTo = Request["DateTo"] + " 23:59:59";
            var Revenue = Request["Revenue"];
            var Pdi = Request["pdi"];

            if (Area == "-- ALL AREA -- ")
            {
                Area = "";
            }

            if (Dealer == "-- ALL DEALER --")
            {
                Dealer = "";
            }
            if (Outlet == "-- ALL OUTLET --")
            {
                Outlet = "";
            }
            if (Revenue == "-- ALL REVENUE --")
            {
                Revenue = "";
            }

            ctx.Database.CommandTimeout = 3600;
            var qry = ctx.Database.SqlQuery<DetailRevenueRegSPK>("exec uspfn_SvDtlRevenueRegSpkGrid @Area=@p0, @Dealer=@p1, @Outlet=@p2, @DateFrom=@p3, @DateTo=@p4, @Revenue=@p5, @Pdi=@p6", Area, Dealer, Outlet, DateFrom, DateTo, Revenue, Pdi).AsQueryable();


            return Json(qry.KGrid());
        }

        public JsonResult SvDtlRevenueRegSpkXls()
        {
            var Area = Request["AreaName"];
            var Dealer = Request["Dealer"];
            var Outlet = Request["Outlet"];
            var DateFrom = Request["DateFrom"] + " 00:00:00";
            var DateTo = Request["DateTo"] + " 23:59:59";
            var Revenue = Request["Revenue"];
            var Pdi = Request["pdi"];
            string fileNames = "DtlRevenueRegSPK_PDI_" + Pdi;

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_SvDtlRevenueRegSpkXls";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Area", Area);
            cmd.Parameters.AddWithValue("@Dealer", Dealer);
            cmd.Parameters.AddWithValue("@Outlet", Outlet);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
            cmd.Parameters.AddWithValue("@DateTo", DateTo);
            cmd.Parameters.AddWithValue("@Revenue", Revenue);
            cmd.Parameters.AddWithValue("@Pdi", Pdi);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            var header = new List<List<dynamic>>() { };
            if (Dealer == "") header.Add(new List<dynamic> { "AREA", (Area == "") ? "ALL" : Area });
            header.Add(new List<dynamic> { "DEALER", (Dealer == "") ? "ALL" : ctx.DealerInfos.Find(new string[] { Dealer }).DealerName });
            header.Add(new List<dynamic> { "OUTLET", (Outlet == "") ? "ALL" : ctx.OutletInfos.Where(p => p.BranchCode == Outlet).FirstOrDefault().BranchName });
            header.Add(new List<dynamic> { "PERIODE", string.Format("{0} s.d {1}", Convert.ToDateTime(DateFrom).ToString("dd-MMM-yyy"), Convert.ToDateTime(DateTo).ToString("dd-MMM-yyy")) });

            return GenerateReportXls(dt, fileNames, fileNames, header);


        }

        public JsonResult MpPersInfo()
        {
            string companyCode = Request["CompanyCode"] ?? "";
            string branchCode = Request["BranchCode"] ?? "";
            string employeeName = Request["EmployeeName"] ?? "";
            string personnelStatus = Request["PersonnelStatus"] ?? "";
            string department = Request["Department"] ?? "";
            string position = Request["Position"] ?? "";

            var data = ctx.ViewHrInqPersonalInformations.AsQueryable();

            if (!string.IsNullOrWhiteSpace(companyCode))
            {
                data = data.Where(x => x.CompanyCode == companyCode);
            }

            if (!string.IsNullOrWhiteSpace(branchCode))
            {
                data = data.Where(x => x.BranchCode == branchCode);
            }

            if (!string.IsNullOrWhiteSpace(position))
            {
                data = data.Where(x => x.PositionCode == position);
            }

            if (!string.IsNullOrWhiteSpace(personnelStatus))
            {
                data = data.Where(x => x.PersonnelStatusCode == personnelStatus);
            }

            if (!string.IsNullOrWhiteSpace(employeeName))
            {
                data = data.Where(x => x.EmployeeName.ToLower().Contains(employeeName.ToLower()));
            }

            return Json(data.KGrid());
        }

        public JsonResult MpDashboard()
        {
            var companyCode = Request["CompanyCode"];
            var branchCode = Request["BranchCode"];
            var periodeRaw = Request["Periode"];
            string periode = ConvertTo112DateFormat(periodeRaw);

            var data = ctx.Database.SqlQuery<MpDashboard>("exec uspfn_mpDashboard @CompanyCode=@p0, @BranchCode=@p1, @Periode=@p2", companyCode, branchCode, periode).AsQueryable();

            //return Json(data.KGrid());
            return Json(data);
        }

        public JsonResult MpTrainingSummary()
        {
            var companyCode = Request["CompanyCode"];
            var branchCode = Request["BranchCode"];
            var periodeRaw = Request["Periode"];
            string periode = ConvertTo112DateFormat(periodeRaw);

            var data = ctx.Database.SqlQuery<MpTrainingSummary>("exec uspfn_mpTrainingSummary @CompanyCode=@p0, @BranchCode=@p1, @Periode=@p2", companyCode, branchCode, periode).AsQueryable();

            //return Json(data.KGrid());
            return Json(data);
        }

        public JsonResult MpDataTrend()
        {
            string companyCode = Request["CompanyCode"] ?? "";
            string branchCode = Request["BranchCode"] ?? "";
            string position = Request["Position"] ?? "";
            string year = Request["Year"] ?? "1900";
            string month = ("0" + (Request["Month"] ?? "00"));
            month = month.Substring(month.Length - 2, 2);

            string periode = year + month;



            var data = ctx.Database.SqlQuery<MpDataTrend>("exec uspfn_MpDataTrend @CompanyCode=@p0, @BranchCode=@p1, @Position=@p2, @Periode=@p3", companyCode, branchCode, position, periode).AsQueryable();

            return Json(data.KGrid());
            //return Json(data);
        }

        public JsonResult MpRotation()
        {
            string companyCode = Request["CompanyCode"] ?? "";
            string branchCode = Request["BranchCode"] ?? "";
            string position = Request["Position"] ?? "";
            string grade = Request["Grade"] ?? "";
            string year = Request["Year"] ?? "1900";
            string month = ("0" + (Request["Month"] ?? "00"));
            month = month.Substring(month.Length - 2, 2);

            string periode = year + month;

            var data = ctx.Database.SqlQuery<MpRotation>("exec uspfn_MpRotation @CompanyCode=@p0, @BranchCode=@p1, @Position=@p2, @Grade=@p3, @Periode=@p4", companyCode, branchCode, position, grade, periode).AsQueryable();

            return Json(data.KGrid());
            //return Json(data);
        }

        public JsonResult LiveStockPart()
        {
            var area = Request.Params["Area"];
            var PartNo = (Request.Params["PartNo"] != null && Request.Params["PartNo"] == "-- ALL PARTS --") ? "" : Request.Params["PartNo"];

            var qry = ctx.Database.SqlQuery<LiveStockPart>("exec uspfn_spLiveStockPart @area=@p0, @partno=@p1", string.IsNullOrEmpty(area) ? "%" : area, string.IsNullOrEmpty(PartNo) ? "%" : PartNo).AsQueryable();

            return Json(qry.KGrid());
        }

        public JsonResult LiveStockPart2()
        {
            var area = Request.Params["Area"];
            var PartNo = (Request.Params["PartNo"] != null && Request.Params["PartNo"] == "-- ALL PARTS --") ? "" : Request.Params["PartNo"];

            var qry = ctx.Database.SqlQuery<LiveStockPart>("exec uspfn_spLiveStockPart2 @area=@p0, @partno=@p1", string.IsNullOrEmpty(area) ? "%" : area, string.IsNullOrEmpty(PartNo) ? "%" : PartNo).AsQueryable();

            return Json(qry.KGrid());
        }

        public JsonResult LiveStockPartDetailInq()
        {
            var PartNo = string.IsNullOrEmpty(Request.Params["PartNo"]) ? null : Request.Params["PartNo"];
            var Area = string.IsNullOrEmpty(Request.Params["Area"]) ? null : Request.Params["Area"];
            var Qty = Request.Params["Qty"];
            int Quantity;
            if (int.TryParse(Qty, out Quantity) == false || string.IsNullOrEmpty(Qty))
            {
                Quantity = 0;
            }
            else
            {
                Quantity = Convert.ToInt32(Qty);
            }
            var qry = ctx.Database.SqlQuery<LiveStockPartInqDetail>(string.Format("exec uspfn_spLiveStockPartInqDetail '{0}', '{1}', {2}", PartNo, Area, Quantity)).AsQueryable();
            ctx.Database.SqlQuery<int>("exec uspfn_LastSpHstLSPLog @UserID = @p0, @DealerCode = @p1, @OutletCode = @p2, @PartNo = @p3, @LSPMode = 'INQ'", CurrentUser.Username, CompanyCode, BranchCode, PartNo).ToList();
            return Json(qry.KGrid());
        }

        public JsonResult LiveStockPartDetailTrans()
        {
            var PartNo = string.IsNullOrEmpty(Request.Params["PartNo"]) ? null : Request.Params["PartNo"];
            var Area = string.IsNullOrEmpty(Request.Params["Area"]) ? null : Request.Params["Area"];
            var qry = ctx.Database.SqlQuery<LiveStockPartTransDetail>(string.Format("exec uspfn_spLiveStockPartTransDetail '{0}', '{1}'", PartNo, Area)).AsQueryable();
            ctx.Database.SqlQuery<int>("exec uspfn_LastSpHstLSPLog @UserID = @p0, @DealerCode = @p1, @OutletCode = @p2, @PartNo = @p3, @LSPMode = 'SLS'", CurrentUser.Username, CompanyCode, BranchCode, PartNo).ToList();
            return Json(qry.KGrid());
        }


        public JsonResult LiveStockPartDetailInq2()
        {
            var PartNo = string.IsNullOrEmpty(Request.Params["PartNo"]) ? null : Request.Params["PartNo"];
            var Area = string.IsNullOrEmpty(Request.Params["Area"]) ? null : Request.Params["Area"];
            var Qty = Request.Params["Qty"];
            int Quantity;
            if (int.TryParse(Qty, out Quantity) == false || string.IsNullOrEmpty(Qty))
            {
                Quantity = 0;
            }
            else
            {
                Quantity = Convert.ToInt32(Qty);
            }
            var qry = ctx.Database.SqlQuery<LiveStockPartInqDetail>(string.Format("exec uspfn_spLiveStockPartInqDetail2 '{0}', '{1}', {2}", PartNo, Area, Quantity)).AsQueryable();
            ctx.Database.SqlQuery<int>("exec uspfn_LastSpHstLSPLog @UserID = @p0, @DealerCode = @p1, @OutletCode = @p2, @PartNo = @p3, @LSPMode = 'INQ'", CurrentUser.Username, CompanyCode, BranchCode, PartNo).ToList();
            return Json(qry.KGrid());
        }

        public JsonResult LiveStockPartDetailTrans2()
        {
            var PartNo = string.IsNullOrEmpty(Request.Params["PartNo"]) ? null : Request.Params["PartNo"];
            var Area = string.IsNullOrEmpty(Request.Params["Area"]) ? null : Request.Params["Area"];
            var qry = ctx.Database.SqlQuery<LiveStockPartTransDetail>(string.Format("exec uspfn_spLiveStockPartTransDetail2 '{0}', '{1}'", PartNo, Area)).AsQueryable();
            ctx.Database.SqlQuery<int>("exec uspfn_LastSpHstLSPLog @UserID = @p0, @DealerCode = @p1, @OutletCode = @p2, @PartNo = @p3, @LSPMode = 'SLS'", CurrentUser.Username, CompanyCode, BranchCode, PartNo).ToList();
            return Json(qry.KGrid());
        }

        public JsonResult LiveStockPartDetail()
        {
            var PartNo = Request.Params["PartNo"];
            var Area = Request.Params["Area"];

            var qry = ctx.Database.SqlQuery<LiveStockPartDetail>("exec uspfn_spLiveStockPartDetail @partno=@p0, @area=@p1", string.IsNullOrEmpty(PartNo) ? "%" : PartNo, string.IsNullOrEmpty(Area) ? "%" : Area).AsQueryable();

            return Json(qry.KGrid());
        }

        public JsonResult LiveStockPartDetail2()
        {
            var PartNo = Request.Params["PartNo"];
            var Area = Request.Params["Area"];

            var qry = ctx.Database.SqlQuery<LiveStockPartDetail>("exec uspfn_spLiveStockPartDetail2 @partno=@p0, @area=@p1", string.IsNullOrEmpty(PartNo) ? "%" : PartNo, string.IsNullOrEmpty(Area) ? "%" : Area).AsQueryable();

            return Json(qry.KGrid());
        }

        public JsonResult InqLastTransDateInfo()
        {
            string ptype = Request["ProductType"] ?? "";
            string dName = Request["Dealer"] ?? "";

            var rawData = ctx.TransactionDateInfo.AsQueryable();

            if (!string.IsNullOrEmpty(ptype))
            {
                rawData = rawData.Where(x => x.ProductType == ptype);
            }

            if (!string.IsNullOrEmpty(dName))
            {
                rawData = rawData.Where(x => x.DealerCode == dName);
            }

            rawData = rawData.OrderBy(x => x.DealerCode).ThenBy(x => x.DealerAbbr).ThenBy(x => x.BranchCode);

            return Json(rawData.ToList());

        }

        public JsonResult MonitoringDealer()
        {
            var ProductType = Request.Params["ProductType"];
            var DealerCode = Request.Params["Dealer"];
            var IsWhat = Request.Params["Aplikasi"];
            var Query = String.Format("exec uspfn_MonitoringDealer '{0}', '{1}', '{2}'", ProductType, DealerCode, IsWhat);
            var qry = ctx.Database.SqlQuery<FormMonitoring>(Query).AsQueryable();

            return Json(qry.ToList());
        }

        public JsonResult MonitoringDealerDetails()
        {
            var DealerCode = Request.Params["Dealer"];
            var OutletCode = Request.Params["Outlet"];
            var ModuleName = Request.Params["Aplikasi"];
            //var Query = String.Format("exec uspfn_MonitoringDealerDetails '{0}', '{1}'", DealerCode, ModuleName);
            //var qry = ctx.Database.SqlQuery<FormMonitoring>(Query).AsQueryable();

            //return Json(qry.ToList());

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_MonitoringDealerDetails";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@DealerCode", DealerCode);
            cmd.Parameters.AddWithValue("@OutletCode", OutletCode);
            cmd.Parameters.AddWithValue("@ModuleName", ModuleName);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult InqKdpDataCoupon()
        {
            string area = Request["Area"] ?? "";
            string dealer = Request["Dealer"] ?? "";
            string outlet = Request["Outlet"] ?? "";
            string dateFrom = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            //DateTime dateFrom = Convert.ToDateTime(Request["DateFrom"]);
            //DateTime dateTo = Convert.ToDateTime(Request["DateTo"]);
            string BeginCoupon = Request["BeginCoupon"] ?? "";
            string EndCoupon = Request["EndCoupon"] ?? "";

            var sql = String.Format(@"SELECT f.Area, a.CompanyCode, b.BranchCode, Convert(varchar(30),a.TestDriveDate,106) as TestDriveDate , a.CoupunNumber, b.NamaProspek, a.ProspekIdentityNo, b.AlamatProspek, b.TelpRumah
		        , a.Email, c.EmployeeName, d.SalesID, c.IdentityNo, e.OutletName, e.OutletArea, a.Remark
                FROM pmKDPCoupon a
                INNER JOIN pmKDP b
	                ON a.CompanyCode = b.CompanyCode
	                and a.InquiryNumber = b.InquiryNumber
	                AND a.NamaProspek = b.NamaProspek
                INNER JOIN HrEmployee c
	                ON a.CompanyCode = c.CompanyCode
	                and a.EmployeeID = c.EmployeeID
                LEFT JOIN HrEmployeeSales d
	                ON c.CompanyCode = d.CompanyCode
	                AND c.EmployeeID = d.EmployeeID
                INNER JOIN gnMstDealerOutletMapping e
	                ON b.CompanyCode = e.DealerCode
	                AND b.BranchCode = e.OutletCode
                INNER JOIN gnMstDealerMapping f
	                ON e.DealerCode = f.DealerCode
	                AND e.GroupNo = f.GroupNo
                WHERE a.TestDriveDate BETWEEN '{0}' AND '{1}'
                AND f.GroupNo = (CASE WHEN '{2}'='' THEN f.GroupNo ELSE '{2}' end)
                AND b.CompanyCode = (CASE WHEN '{3}'='' THEN b.CompanyCode ELSE '{3}' end)
                AND b.BranchCode = (CASE WHEN '{4}'='' THEN b.BranchCode ELSE '{4}' end)
                AND {5}", dateFrom, dateTo, area, dealer, outlet, (BeginCoupon != "" && EndCoupon != "") ? "a.CoupunNumber BETWEEN '" + BeginCoupon + "' AND '" + EndCoupon + "'" : "1=1");

            var data = ctx.Database.SqlQuery<InqKdpCouponView>(sql).AsQueryable();


            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult inqIndent()
        {
            //string area = Request["GroupArea"]; //?? "--";
            //string comp = Request["CompanyCode"]; //?? "--";
            //string outl = Request["Outlet"];
            //string dateFr = Request["DateFrom"];
            //string dateTo = Request["DateTo"];
            var area = Request["GroupArea"]; //?? "--";
            var Company = Request["CompanyCode"];
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonth = Request["PeriodMonth"];

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal Month = Convert.ToInt32(PeriodMonth != "" ? PeriodMonth : "0");

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            //var data = ctx.Database.SqlQuery<InqIndent>("exec uspfn_InqIndent @GroupNo=@p0, @CompanyCode=@p1, @BranchCode=@p1,@DeptCode=@p2, @PosCode=@p3, @Status=@p4", area, comp, dept, post, status).AsQueryable();
            //var data = ctx.Database.SqlQuery<InqIndent>("exec uspfn_InqIndent @GroupNo=@p0, @CompanyCode=@p1, @Outlet=@p2, @DateFrom=@p3, @DateTo=@p4, @IndentNumber=@p5", area, comp, outl, dateFr, dateTo, "").AsQueryable();
            var data = ctx.Database.SqlQuery<InqIndent>("exec uspfn_InqIndent_rev @GroupNo=@p0, @CompanyCode=@p1, @PeriodeYear=@p2, @PeriodeMonth=@p3, @table=@p4", area, Company, Year, Month, "1").AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult inqIndentSub()
        {
            var area = Request["GroupArea"]; //?? "--";
            string comp = Request["CompanyCode"] ?? "";
            string IndentNumber = Request["IndentNumber"]; //?? "--";
            //string area = "";//Request["GroupArea"]; //?? "--";
            //string outl = "";//Request["Outlet"];
            //string dateFr = "";//Request["DateFrom"];
            //string dateTo = "";//Request["DateTo"];

            var PeriodYear = Request["PeriodYear"];
            var PeriodMonth = Request["PeriodMonth"];
            var TipeKendaraan = Request["TipeKendaraan"];
            var Variant = Request["Variant"];
            var ColourCode = Request["ColourCode"];

            ColourCode = ctx.OmMstRefferences.Where(a => a.RefferenceType == "COLO" && a.RefferenceDesc1 == ColourCode).FirstOrDefault().RefferenceCode;

            decimal Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            decimal Month = Convert.ToInt32(PeriodMonth != "" ? PeriodMonth : "0");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_InqIndentDet";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@GroupNo", area);
            cmd.Parameters.AddWithValue("@CompanyCode", comp);
            //cmd.Parameters.AddWithValue("@Outlet", outl);
            //cmd.Parameters.AddWithValue("@DateFrom", dateFr);
            //cmd.Parameters.AddWithValue("@DateTo", dateTo);
            //cmd.Parameters.AddWithValue("@IndentNumber", IndentNumber);
            cmd.Parameters.AddWithValue("@PeriodeYear", Year);
            cmd.Parameters.AddWithValue("@PeriodeMonth", Month);
            cmd.Parameters.AddWithValue("@TipeKendaraan", TipeKendaraan);
            cmd.Parameters.AddWithValue("@Variant", Variant);
            cmd.Parameters.AddWithValue("@ColourCode", ColourCode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult SpLogReport()
        {
            string area = Request["GroupArea"]; //?? "--";
            string comp = Request["CompanyCode"]; //?? "--";
            string outl = Request["Outlet"];
            string dateFr = Request["DateFrom"];
            string dateTo = Request["DateTo"];
            string mode = Request["Mode"] == "false" ? "SLS" : "INQ";

            int to = 0; bool fa = true;
            if (ctx.Database.CommandTimeout.HasValue) to = ctx.Database.CommandTimeout.Value; else fa = false;

            ctx.Database.CommandTimeout = 3600;
            var data = ctx.Database.SqlQuery<inqLogReport>("exec uspfn_SpGnrLogReport @GroupNo=@p0, @CompanyCode=@p1, @Outlet=@p2, @DateFrom=@p3, @DateTo=@p4, @Mode=@p5", area, comp, outl, dateFr, dateTo, mode).OrderBy(p => p.date_access).AsQueryable();
            if (fa) ctx.Database.CommandTimeout = to; else ctx.Database.CommandTimeout = null;

            return Json(data.KGrid(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Review()
        {
            string GroupArea = Request["GroupArea"] == "" ? "0" : Request["GroupArea"];
            string CompanyCode = Request["CompanyCode"] ?? "";
            string BranchCode = Request["BranchCode"] ?? "";
            string DateFrom = Request["DateFrom"];
            string DateTo = Request["DateTo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
//            cmd.CommandText = @"select a.*, dm.DealerCode, dm.DealerAbbreviation, dom.OutletCode, dom.OutletAbbreviation from CsReviews a
//                                inner join gnMstDealerOutletMapping dom 
//                                on dom.DealerCode=a.CompanyCode and dom.OutletCode = a.BranchCode              
//                                inner join gnMstDealerMapping dm 
//                                on  dm.DealerCode = a.CompanyCode and dm.GroupNo = dom.GroupNo
//                                where dm.GroupNo= CASE @GroupArea WHEN 0 THEN dm.GroupNo ELSE @GroupArea END
//                                and CompanyCode= CASE @CompanyCode WHEN '' THEN CompanyCode ELSE @CompanyCode END
//                                and BranchCode= CASE @BranchCode WHEN '' THEN BranchCode ELSE @BranchCode END
//                                and convert(varchar, datefrom, 112) >= @DateFrom and convert(varchar, dateto, 112) <= @DateTo";
//            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "uspfn_InqReview_New";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@GroupArea", Convert.ToInt32(GroupArea));
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@Company", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
            cmd.Parameters.AddWithValue("@DateTo", DateTo);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult DefaultInqSales()
        {
            return Json(new
            {
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                EndDate = DateTime.Now
            });
        }
        public JsonResult GetBranchHead()
        {
            string Area = Request["Area"] ?? "";
            string Dealer = Request["Dealer"] ?? "";
            string Outlet = Request["Outlet"] ?? "";


            //var query = string.Format(@"usprpt_omInqSalesLkpEmployee '{0}','{1}','{2}','{3}','{4}','{5}','{6}'"
            var query = string.Format(@"usprpt_omInqSalesLkpEmployee '{0}','{1}','{2}','','','','BM'"
                    , Area, Dealer, Outlet);
            var queryable = ctx.Database.SqlQuery<omInqSalesLkpEmployee>(query).AsQueryable();

            return Json(queryable.KGrid());
        }

        public JsonResult GetSH()
        {
            string Area = Request["Area"] ?? "";
            string Dealer = Request["Dealer"] ?? "";
            string Outlet = Request["Outlet"] ?? "";
            string BM = Request["BM"] ?? "";


            //var query = string.Format(@"usprpt_omInqSalesLkpEmployee '{0}','{1}','{2}','{3}','{4}','{5}','{6}'"
            var query = string.Format(@"usprpt_omInqSalesLkpEmployee '{0}','{1}','{2}','{3}','','','SC'"
                    , Area, Dealer, Outlet, BM);
            var queryable = ctx.Database.SqlQuery<omInqSalesLkpEmployee>(query).AsQueryable();

            return Json(queryable.KGrid());
        }

        public JsonResult GetSalesman()
        {
            string Area = Request["Area"] ?? "";
            string Dealer = Request["Dealer"] ?? "";
            string Outlet = Request["Outlet"] ?? "";
            string BM = Request["BM"] ?? "";
            string SH = Request["SH"] ?? "";


            //var query = string.Format(@"usprpt_omInqSalesLkpEmployee '{0}','{1}','{2}','{3}','{4}','{5}','{6}'"
            var query = string.Format(@"usprpt_omInqSalesLkpEmployee '{0}','{1}','{2}','{3}','{4}','','SL'"
                    , Area, Dealer, Outlet, BM, SH);
            var queryable = ctx.Database.SqlQuery<omInqSalesLkpEmployee>(query).AsQueryable();

            return Json(queryable.KGrid());
        }

        public JsonResult GetReportOmRpSalRgs039Web(string startDate, string endDate, string area, string dealer, string outlet, string branchHead, string salesHead, string salesman, string SalesType)
        {
            var query = string.Format(@"usprpt_OmRpSalRgs039Web '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'"
                , startDate, endDate, area, dealer, outlet, branchHead, salesHead, salesman, SalesType);
            var queryable = ctx.Database.SqlQuery<InqSales>(query).ToList().Take(52);

            return Json(queryable);
        }

        public ActionResult GenXlsInqSales(DateTime startDate, DateTime endDate, string area, string dealer, string outlet, string branchHead, string salesHead, string salesman, string SalesType,
             string printType, bool isArea, bool isDealer, bool isoutlet, bool isBrachHead, bool isSalesHead, bool isSalesCoordinator, bool isWiraniaga, bool isGrade, bool isModel, bool isWarna, string sModel, string dealername)
        {

            string pserver = Server.MapPath("~/ReportTemp/");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            //DataTable dtDetail;
            DataTable dtHeader = new DataTable();
            DataTable dtHeader1 = new DataTable();

            if (!isModel)
                sModel = "";

            string snotfound = "<h1>Tidak ada data</h1>";
            string query = "";

            string sgroupno = "";
            if (dealer != "")
            {
                sgroupno = ctx.GnMstDealerMappingNews.Where(x => x.DealerName == dealername).FirstOrDefault().GroupNo.ToString();
                dealer = ctx.GnMstDealerMappingNews.Where(x => x.DealerName == dealer).FirstOrDefault().DealerCode;
            }

            if (printType == "0")//Sales
            {
                query = string.Format(@"usprpt_OmRpSalRgs045web '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}'"
                        , startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"), area, dealer, outlet, branchHead, salesHead, salesman, SalesType, isoutlet)
                        + (string.IsNullOrEmpty(sgroupno) ? "" : "," + sgroupno);
                cmd.CommandText = query;
                da.Fill(ds);
                if (ds.Tables.Count == 0)
                    return Content(snotfound);
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return Content(snotfound);
                }


                var dp = new Dictionary<int, string>();
                dp.Add(1, startDate.ToString("dd-MMM-yyyy") + " s/d " + endDate.ToString("dd-MMM-yyyy"));
                dp.Add(2, string.IsNullOrEmpty(branchHead) ? "ALL" : branchHead);
                dp.Add(3, string.IsNullOrEmpty(area) ? "ALL" : area);
                dp.Add(4, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                dp.Add(5, string.IsNullOrEmpty(dealer) ? "ALL" : dealer);
                dp.Add(6, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                dp.Add(7, string.IsNullOrEmpty(outlet) ? "ALL" : outlet);
                dp.Add(8, string.IsNullOrEmpty(salesman) ? "ALL" : salesman);
                dp.Add(9, isoutlet ? "1" : "0");
                dp.Add(10, SalesType);
                dp.Add(11, "DEALER");


                var fileName = ("SalesReport_" + (isoutlet ? "OUTLET_" : "") + startDate.ToString("ddMMyyy") + "_" + endDate.ToString("ddMMyyyy")).Trim() + ".xls";
                OmRpSalRgs045Xcl rpt = new OmRpSalRgs045Xcl(ds, dp, fileName, "0", "UNIT");
                rpt.CreateReport(pserver);

                return Redirect(Url.Content("~/ReportTemp/" + fileName));

            }
            else if (printType == "1") //sales trend report
            {

                query = string.Format(@"usprpt_OmRpSalRgs040web '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}'"
                    , startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"), area, dealer, outlet, branchHead, salesHead, salesman, isArea, isDealer, isoutlet, isModel, isWarna, sModel == "" ? "0" : "1", sModel, SalesType)
                    + (string.IsNullOrEmpty(sgroupno) ? "" : "," + sgroupno);
                cmd.CommandText = query;
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    return Content(snotfound);// Json("Tidak ada Data",JsonRequestBehavior.AllowGet);
                }
                var dp = new Dictionary<int, string>();
                dp.Add(1, startDate.ToString("dd/MM/yyyy") + " s/d " + endDate.ToString("dd/MM/yyyy"));
                dp.Add(2, string.IsNullOrEmpty(branchHead) ? "ALL" : branchHead);
                dp.Add(3, string.IsNullOrEmpty(area) ? "ALL" : area);
                dp.Add(4, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                dp.Add(5, string.IsNullOrEmpty(dealer) ? "ALL" : dealer);
                dp.Add(6, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                dp.Add(7, string.IsNullOrEmpty(outlet) ? "ALL" : outlet);
                dp.Add(8, string.IsNullOrEmpty(salesman) ? "ALL" : salesman);
                dp.Add(9, isArea ? "1" : "0");
                dp.Add(10, isDealer ? "1" : "0");
                dp.Add(11, isoutlet ? "1" : "0");
                dp.Add(12, isModel ? "1" : "0");
                dp.Add(13, isWarna ? "1" : "0");
                dp.Add(14, string.IsNullOrEmpty(sModel) ? "" : sModel);
                dp.Add(15, SalesType);
                dp.Add(16, "DEALER");

                var fileName = ("SalesTrendReport_" + (isoutlet ? "" : "Outlet ") + (isWarna ? "" : "WARNA ") + (isModel ? (string.IsNullOrEmpty(sModel) ? "TYPE " : sModel + " ") : " ")).Trim() + ".xls";
                if (sModel != "" && isWarna)
                {
                    OmRpSalRgs040BXcl rpt = new OmRpSalRgs040BXcl(ds, dp, fileName);
                    rpt.CreateReport(pserver);
                }
                else
                {

                    OmRpSalRgs040Xcl rpt = new OmRpSalRgs040Xcl(ds, dp, fileName);
                    rpt.CreateReport(pserver);
                }

                return Redirect(Url.Content("~/ReportTemp/" + fileName));

            }
            else if (printType == "2") //productivity trend report
            {
                #region Productivity trend report
                if (!isGrade)
                {

                    #region Not Grade
                    query = string.Format(@"usprpt_OmRpSalRgs037web '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},{9},{10},'{11}','{12}'"
                                , startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"), area, dealer, outlet, branchHead, salesHead, salesman, isWiraniaga, isModel, isWarna, sModel, SalesType)
                                + (string.IsNullOrEmpty(sgroupno) ? "" : "," + sgroupno);
                    cmd.CommandText = query;
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        return Content(snotfound);// Json("Tidak ada Data",JsonRequestBehavior.AllowGet);
                    }
                    var dp = new Dictionary<int, string>();
                    dp.Add(1, startDate.ToString("dd-MMM-yyyy") + " s/d " + endDate.ToString("dd-MMM-yyyy"));
                    dp.Add(2, string.IsNullOrEmpty(branchHead) ? "ALL" : branchHead);
                    dp.Add(3, string.IsNullOrEmpty(area) ? "ALL" : area);
                    dp.Add(4, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                    dp.Add(5, string.IsNullOrEmpty(dealer) ? "ALL" : dealer);
                    dp.Add(6, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                    dp.Add(7, string.IsNullOrEmpty(outlet) ? "ALL" : outlet);
                    dp.Add(8, string.IsNullOrEmpty(salesman) ? "ALL" : salesman);
                    dp.Add(9, isWiraniaga ? "1" : "0");
                    dp.Add(10, isModel ? "1" : "0");
                    dp.Add(11, isWarna ? "1" : "0");
                    dp.Add(12, SalesType);
                    dp.Add(13, string.IsNullOrEmpty(sModel) ? "" : sModel);
                    dp.Add(14, "DEALER");

                    StringBuilder title = new System.Text.StringBuilder();
                    title.Append(isWiraniaga ? "WIRANIAGA " : "");
                    title.Append(isModel ? "TYPE" : "");
                    title.Append(isWarna ? "WARNA " : "");
                    title.ToString().TrimEnd(' ');
                    if (title.ToString() != "")
                        title.Append("_");
                    title.Append(SalesType == "WHOLESALE" ? "WS" : SalesType == "SALES" ? "SALES" : SalesType == "RETAIL" ? "RTL" : SalesType == "FPOL" ? "FPOL" : "REGPOL");

                    var fileName = "ProductivityTrendReport_" + title.ToString().TrimEnd(' ') + "_" + startDate.ToString("ddMMyy") + "-" + endDate.ToString("ddMMyy") + ".xls";

                    if (startDate.Year == endDate.Year)
                    {
                        var rpt = new ProductionTrendInquiry(ds, dp, fileName);
                        rpt.CreateReport(pserver);
                    }
                    else
                    {
                        var rpt = new OmRpSalRgs038Xcl(ds.Tables[0], dp, fileName);
                        rpt.CreateReport(pserver);
                    }

                    return Redirect(Url.Content("~/ReportTemp/" + fileName));

                    #endregion

                }
                else
                {
                    #region Grade
                    if (startDate > endDate)
                    {
                        return Content("<h1>Tanggal mulai tidak boleh lebih besar dari tanggal akhir</h1>");
                    }

                    query = string.Format(@"usprpt_OmRpSalRgs046web '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'"
                    , startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"), area, dealer, outlet, branchHead, salesHead, salesman, SalesType)
                    + (string.IsNullOrEmpty(sgroupno) ? "" : "," + sgroupno);
                    cmd.CommandText = query;
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        return Content(snotfound);// Json("Tidak ada Data",JsonRequestBehavior.AllowGet);
                    }


                    var parameter = new Dictionary<int, string>();
                    parameter.Add(1, startDate.ToString("dd-MMM-yyyy") + " s/d " + endDate.ToString("dd-MMM-yyyy"));
                    parameter.Add(2, string.IsNullOrEmpty(branchHead) ? "ALL" : branchHead);
                    parameter.Add(3, string.IsNullOrEmpty(area) ? "ALL" : area);
                    parameter.Add(4, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                    parameter.Add(5, string.IsNullOrEmpty(dealer) ? "ALL" : dealer);
                    parameter.Add(6, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                    parameter.Add(7, string.IsNullOrEmpty(outlet) ? "ALL" : outlet);
                    parameter.Add(8, string.IsNullOrEmpty(salesman) ? "ALL" : salesman);
                    parameter.Add(9, SalesType);
                    parameter.Add(10, "DEALER");


                    System.Text.StringBuilder title = new System.Text.StringBuilder();
                    title.Append("GRADE ");
                    title.ToString().TrimEnd(' ');
                    if (title.ToString() != "")
                        title.Append("_");

                    title.Append(SalesType == "WHOLESALE" ? "WHOLESALE" : SalesType == "SALES" ? "SALES" : SalesType == "RETAIL" ? "RETAIL" : SalesType == "FPOL" ? "FPOL" : "REGPOL");

                    if (ds.Tables.Count == 2)
                    {
                        return Content(snotfound);
                    }

                    var fileName = "ProductivityTrendReport_" + title.ToString().TrimEnd(' ') + "_" + startDate.ToString("ddMMyy") + "-" + endDate.ToString("ddMMyy") + ".xls";

                    OmRpSalRgs046Xcl rpt = new OmRpSalRgs046Xcl(ds, parameter, fileName);
                    rpt.CreateReport(pserver);
                    return Redirect(Url.Content("~/ReportTemp/" + fileName));
                    #endregion
                }

                #endregion
            }
            else if (printType == "3") //Comparison: Wholesale vs Retail
            {
                #region Comparison: Wholesale vs Retail
                if (isModel && string.IsNullOrEmpty(sModel))
                    isModel = false;

                query = string.Format(@"usprpt_OmRpSalRgs041web '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},{10},'{11}'"
                            , startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"), area, dealer, outlet, branchHead, salesHead, "", salesman, isoutlet, isModel, sModel)
                            + (string.IsNullOrEmpty(sgroupno) ? "" : "," + sgroupno);
                cmd.CommandText = query;
                da.Fill(ds);

                

                if (ds.Tables[0].Rows.Count == 0)
                {
                    return Content(snotfound);// Json("Tidak ada Data",JsonRequestBehavior.AllowGet);
                }
                var dp = new Dictionary<int, string>();
                dp.Add(1, startDate.ToString("dd-MMM-yyyy") + " s/d " + endDate.ToString("dd-MMM-yyyy"));
                dp.Add(2, string.IsNullOrEmpty(branchHead) ? "ALL" : branchHead);
                dp.Add(3, string.IsNullOrEmpty(area) ? "ALL" : area);
                dp.Add(4, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                dp.Add(5, string.IsNullOrEmpty(dealer) ? "ALL" : dealer);
                dp.Add(6, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                dp.Add(7, string.IsNullOrEmpty(outlet) ? "ALL" : outlet);
                dp.Add(8, string.IsNullOrEmpty(salesman) ? "ALL" : salesman);
                //dp.Add(8, isWiraniaga ? "1" : "0");
                dp.Add(9, isoutlet ? "1" : "0");
                dp.Add(10, sModel);

                StringBuilder title = new StringBuilder();
                //title.Append(isWiraniaga ? "WIRANIAGA " : "");
                title.Append(isModel ? "TYPE" : "");
                title.Append(isWarna ? "WARNA " : "");
                title.ToString().TrimEnd(' ');
                if (title.ToString() != "")
                    title.Append("_");
                title.Append(SalesType == "WHOLESALE" ? "WS" : SalesType == "SALES" ? "SALES" : SalesType == "RETAIL" ? "RTL" : SalesType == "FPOL" ? "FPOL" : "REGPOL");

                var fileName = "Comp-WS_RS_" + title.ToString().TrimEnd(' ') + "_" + startDate.ToString("ddMMyy") + "-" + endDate.ToString("ddMMyy") + ".xls";

                var rpt = new OmRpSalRgs041Rpt(ds, dp, fileName);
                rpt.CreateReport(pserver);

                return Redirect(Url.Content("~/ReportTemp/" + fileName));
                #endregion
            }
            else if (printType == "4") //productivity trend report
            {
                #region Comparison: Actual vs Target
                if (isModel && string.IsNullOrEmpty(sModel))
                    isModel = false;

                query = string.Format(@"usprpt_OmRpSalRgs042web '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},'{10}','{11}'"
                            , startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"), area, dealer, outlet, branchHead, salesHead, salesman, isoutlet, isModel, sModel, SalesType)
                            + (string.IsNullOrEmpty(sgroupno) ? "" : "," + sgroupno);

                cmd.CommandText = query;
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    return Content(snotfound);// Json("Tidak ada Data",JsonRequestBehavior.AllowGet);
                }
                var dp = new Dictionary<int, string>();
                dp.Add(1, startDate.ToString("dd-MMM-yyyy") + " s/d " + endDate.ToString("dd-MMM-yyyy"));
                dp.Add(2, string.IsNullOrEmpty(branchHead) ? "ALL" : branchHead);
                dp.Add(3, string.IsNullOrEmpty(area) ? "ALL" : area);
                dp.Add(4, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                dp.Add(5, string.IsNullOrEmpty(dealer) ? "ALL" : dealer);
                dp.Add(6, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                dp.Add(7, string.IsNullOrEmpty(outlet) ? "ALL" : outlet);
                dp.Add(8, string.IsNullOrEmpty(salesman) ? "ALL" : salesman);
                dp.Add(9, isoutlet ? "1" : "0");
                dp.Add(10, sModel);

                StringBuilder title = new StringBuilder();
                title.Append(isModel ? "TYPE" : "");
                title.Append(isWarna ? "WARNA " : "");
                title.ToString().TrimEnd(' ');
                if (title.ToString() != "")
                    title.Append("_");
                title.Append(SalesType == "WHOLESALE" ? "WS" : SalesType == "SALES" ? "SALES" : SalesType == "RETAIL" ? "RTL" : SalesType == "FPOL" ? "FPOL" : "REGPOL");

                var fileName = "Comp-Act_Trgt_" + title.ToString().TrimEnd(' ') + "_" + startDate.ToString("ddMMyy") + "-" + endDate.ToString("ddMMyy") + ".xls";

                var rpt = new OmRpSalRgs042Xls(ds, dp, fileName);
                rpt.CreateReport(pserver);

                return Redirect(Url.Content("~/ReportTemp/" + fileName));
                #endregion
            }
            else
            {

                #region Comparison: Month to Month and Year to Year
                if (isModel && string.IsNullOrEmpty(sModel))
                    isModel = false;

                if (startDate.Year != endDate.Year || startDate.Month != endDate.Month)
                {
                    return Content("<h1>Periode Bulan Harus sama</h1>");// Js
                }

                startDate = new DateTime(endDate.AddMonths(-1).Year, endDate.AddMonths(-1).Month, 1);
                string header1 = startDate.ToString("MMM");

                endDate = new DateTime(endDate.Year, endDate.Month, 1).AddMonths(1).AddDays(-1);
                string header2 = endDate.ToString("MMM");

                query = string.Format(@"usprpt_OmRpSalRgs043web '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',{9},'{10}','{11}'"
                            , startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"), area, dealer, outlet, branchHead, salesHead, salesman, isoutlet, isModel, sModel, SalesType)
                            + (string.IsNullOrEmpty(sgroupno) ? "" : "," + sgroupno);
                cmd.CommandText = query;
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    return Content(snotfound);// Json("Tidak ada Data",JsonRequestBehavior.AllowGet);
                }
                var dp = new Dictionary<int, string>();
                dp.Add(1, startDate.ToString("dd-MMM-yyyy") + " s/d " + endDate.ToString("dd-MMM-yyyy"));
                dp.Add(2, string.IsNullOrEmpty(branchHead) ? "ALL" : branchHead);
                dp.Add(3, string.IsNullOrEmpty(area) ? "ALL" : area);
                dp.Add(4, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                dp.Add(5, string.IsNullOrEmpty(dealer) ? "ALL" : dealer);
                dp.Add(6, string.IsNullOrEmpty(salesHead) ? "ALL" : salesHead);
                dp.Add(7, string.IsNullOrEmpty(outlet) ? "ALL" : outlet);
                dp.Add(8, string.IsNullOrEmpty(salesman) ? "ALL" : salesman);
                dp.Add(9, isoutlet ? "1" : "0");
                dp.Add(10, sModel);
                dp.Add(11, startDate.ToString("MMM"));
                dp.Add(12, endDate.ToString("MMM"));
                dp.Add(13, endDate.AddYears(-1).ToString("yyyy"));
                dp.Add(14, endDate.ToString("yyyy"));
                dp.Add(15, SalesType);

                StringBuilder title = new StringBuilder();
                title.Append(isModel ? "TYPE" : "");
                title.Append(isWarna ? "WARNA " : "");
                title.ToString().TrimEnd(' ');
                if (title.ToString() != "")
                    title.Append("_");
                title.Append(SalesType == "WHOLESALE" ? "WS" : SalesType == "SALES" ? "SALES" : SalesType == "RETAIL" ? "RTL" : SalesType == "FPOL" ? "FPOL" : "REGPOL");

                var fileName = "Comp-MM_YY_" + title.ToString().TrimEnd(' ') + "_" + startDate.ToString("ddMMyy") + "-" + endDate.ToString("ddMMyy") + ".xls";

                var rpt = new OmRpSalRgs043Xls(ds, dp, fileName);
                rpt.CreateReport(pserver);

                return Redirect(Url.Content("~/ReportTemp/" + fileName));
                #endregion
            }
        }


    }
}
