using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.PreSales.Models;
using ClosedXML.Excel;

namespace SimDms.PreSales.Controllers.Api
{
    public class InquiryController : BaseController
    {

        private ActionResult GenerateExcel(XLWorkbook wb, DataTable dt, int lastRow, string fileName, string fontname = "calibri", bool isCustomHeader = false, bool isShowSummary = false, bool isGridlines = true, bool isContentNoBorder = false) 
        {
            var ws = wb.Worksheet(1);
            var tmpLastRow = lastRow;
            var awalRow = lastRow;
            int iCol = 1;
            char iChar = 'A';
            foreach (DataRow dr in dt.Rows)
            {
                iCol = 1;
                iChar = 'A';
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, iCol).Style.NumberFormat.Format = "#,##0";
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, iCol).Style.NumberFormat.Format = "#,##0.0";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (dr[dc.ColumnName].GetType() == typeof(DateTime))
                    {
                        ws.Cell(lastRow + 1, iCol).Style.DateFormat.Format = "dd-MMM-yyyy";
                    }

                    if (tmpLastRow == lastRow)
                    {
                        ws.Cell(lastRow, iCol).Value = dc.ColumnName;
                        // ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.TeaRoseRose).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(10);

                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }
                    else
                    {
                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }

                    iCol++;
                    iChar++;
                }

                lastRow++;
            }
            var rngTable = ws.Range(tmpLastRow , 1, lastRow, iCol - 1);
            if (isShowSummary)
            {
                ws.Cell(lastRow + 1, 1).Value = "TOTAL";
                int j = 2;
                for (char i = 'B'; i < iChar; i++)
                {
                    ws.Cell(lastRow + 1, j).FormulaA1 = "=SUM(" + i + (tmpLastRow + 1) + ":" + i + lastRow + ")";
                    j++;
                }
                if (isContentNoBorder)
                {
                    ws.Row(awalRow).Style
                       .Border.SetTopBorder(XLBorderStyleValues.Thin)
                       .Border.SetBottomBorder(XLBorderStyleValues.Thin);
                    ws.Row(lastRow + 1).Style
                       .Border.SetTopBorder(XLBorderStyleValues.Thin)
                       .Border.SetBottomBorder(XLBorderStyleValues.Thin);

                } else {
                     rngTable = ws.Range(tmpLastRow + 1, 1, lastRow + 1, iCol - 1);
                        rngTable.Style
                        .Border.SetTopBorder(XLBorderStyleValues.Thin)
                        .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                        .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                        .Border.SetRightBorder(XLBorderStyleValues.Thin);
                }
            }
            else {
                if (isContentNoBorder) {
                    ws.Row(awalRow).Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thin)
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin);
                    ws.Row(lastRow).Style
                        .Border.SetBottomBorder(XLBorderStyleValues.Thin);
                }else{
                        rngTable.Style
                    .Border.SetTopBorder(XLBorderStyleValues.Thin)
                    .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                    .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                    .Border.SetRightBorder(XLBorderStyleValues.Thin);
                }
                
            }
            rngTable.Style.Font.FontName = fontname;
            rngTable.Style.Font.FontSize = 8;

            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();
            ws.ShowGridLines = isGridlines;
            ws.Row(awalRow).Height = 32.25;
            ws.Row(awalRow).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Row(awalRow).Style.Font.FontSize = 9;
            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));

            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult IndByPeriode(string firstPeriod, string endPeriod, string outlet, string bm, string spv, string emp, string ReportId, string tanggal, string param1, string param2, string param3, string outletname)
        {
            //ExcelFileWriter excelReport; 
            string fileName = "";
            fileName = ReportId + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmm");
            int a = 1;
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand; 
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "usprpt_PmRpInqPeriodeWeb"; //+ ReportId;

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", outlet);
            cmd.Parameters.AddWithValue("@PeriodBegin", firstPeriod);
            cmd.Parameters.AddWithValue("@PeriodEnd", endPeriod);
            cmd.Parameters.AddWithValue("@BranchManager", bm);
            cmd.Parameters.AddWithValue("@SalesHead", spv);
            cmd.Parameters.AddWithValue("@Salesman", emp);
            cmd.Parameters.AddWithValue("@isexel", "ada");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);
            dt.Columns.Remove("OutletName");
            dt.Columns["InquiryNumber"].ColumnName = "Inquiry Number";
            dt.Columns["AlamatProspek"].ColumnName = "Alamat Prospek";
            dt.Columns["TelpRumah"].ColumnName = "Telp Rumah";
            dt.Columns["NamaPerusahaan"].ColumnName = "Nama Perusahaan";
            dt.Columns["AlamatPerusahaan"].ColumnName = "Alamat Perusahaan";
            dt.Columns["InquiryDate"].ColumnName = "Tgl Inquiry";
            dt.Columns["TipeKendaraan"].ColumnName = "Tipe";
            dt.Columns["Variant"].ColumnName = "Varian";
            dt.Columns["PerolehanData"].ColumnName = "Perolehan Data";
            dt.Columns["Employee"].ColumnName = "Wiraniaga";
            dt.Columns["Supervisor"].ColumnName = "Koordinator";
            dt.Columns["NextFollowUpDate"].ColumnName = "Next Follow Up";
            dt.Columns["LastProgress"].ColumnName = "Last Progress";
            dt.Columns["LastUpdateStatus"].ColumnName = "Last Update Status";
            dt.Columns["SPKDate"].ColumnName = "SPK Date";
            dt.Columns["LostCaseDate"].ColumnName = "Lost Case Date";
            if (dt.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 6;
                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Sheet1");
                var hdrTable = ws.Range("A1:C5");
                hdrTable.Style
                    .Font.SetFontSize(9)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                
                hdrTable.Style
                    .Font.FontName = "Courier New";
                //First Title
                ws.Cell("A1").Value = "Inquiry By Periode";
                ws.Cell("A3").Value = "Tanggal";
                ws.Cell("A4").Value = "Nama Outlet";
                ws.Cell("A5").Value = "Nama Sales Head";
                ws.Cell("A6").Value = "Nama Salesman";
                //ws.Cell("A4").Value = "Nama Sales Koordinator";
                //ws.Cell("A5").Value = "Nama Salesman";
                //First Title Value
                ws.Cell("B3").Value = ": " + tanggal;
                ws.Cell("B4").Value = ": " + outletname;
                ws.Cell("B5").Value = ": " + param1;
                ws.Cell("B6").Value = ": " + param3;
                //ws.Cell("B4").Value = ": " + param2;
                //ws.Cell("B5").Value = ": " + param3;

                return GenerateExcel(wb, dt, lastRow, fileName, "Courier New", false, false, false, true);
            }
        }

        public JsonResult Default()
        {
            string sql = string.Format("exec uspfn_gnInquiryBtn 'CABANG', '{0}', '', '2'", CompanyCode);
            var dealerList = (from p in ctx.Database.SqlQuery<InquiryBtn>(sql)
                              select new InquiryBtn()
                              {
                                  DealerCode = p.DealerCode,
                                  DealerName = p.DealerName
                              }).Select(p => new { value = p.DealerCode, text = p.DealerName }).FirstOrDefault();
            var Emp = ctx.HrEmployees.Where(a => a.RelatedUser == CurrentUser.UserId).FirstOrDefault();
            return Json(new
            {
                success = true,
                data = new
                    {
                        CompanyCode = CompanyCode,
                        CompanyName = CompanyName,
                        BranchCode = BranchCode,
                        EmployeeID = Emp.EmployeeID,
                        PositionID = Emp.Position,
                        //BranchName = BranchName,
                        IsBranch = IsBranch,
                        DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                        DateTo = DateTime.Now,
                        NationalSLS = NationalSLS,
                        Area = (NationalSLS == "0") ? "CABANG" : "",
                        Dealer = (NationalSLS == "0") ? dealerList.value : "",
                        Outlet = (IsBranch) ? BranchCode : ""
                    }
            });

        }

        //public JsonResult DefaultFilter()
        //{
        //    var empl = (from p in ctx.HrEmployees
        //                where p.CompanyCode == CompanyCode
        //                && p.RelatedUser == CurrentUser.UserId
        //                select new
        //                {
        //                    p.EmployeeID,
        //                    p.EmployeeName,
        //                    p.Grade,
        //                    p.TeamLeader,
        //                }).FirstOrDefault();

        //    if (empl != null)
        //    {
        //        var list = new List<HrEmployee>();
        //        var dept = "SALES";
        //        var empl1 = ctx.HrEmployees.Where(p => p.CompanyCode == CompanyCode && p.EmployeeID == empl.EmployeeID && p.Department == dept).FirstOrDefault();
        //        if (empl1 != null)
        //        {
        //            list.Add(empl1);
        //            var empl2 = ctx.HrEmployees.Where(p => p.CompanyCode == CompanyCode && p.EmployeeID == empl1.TeamLeader && p.Department == dept).FirstOrDefault();
        //            if (empl2 != null)
        //            {
        //                list.Add(empl2);
        //                var empl3 = ctx.HrEmployees.Where(p => p.CompanyCode == CompanyCode && p.EmployeeID == empl2.TeamLeader && p.Department == dept).FirstOrDefault();
        //                if (empl3 != null)
        //                {
        //                    list.Add(empl3);
        //                    var empl4 = ctx.HrEmployees.Where(p => p.CompanyCode == CompanyCode && p.EmployeeID == empl3.TeamLeader && p.Department == dept).FirstOrDefault();
        //                    if (empl4 != null)
        //                    {
        //                        list.Add(empl4);
        //                    }
        //                }
        //            }
        //        }

        //        var oSL = list.Where(p => p.Position == "S").FirstOrDefault();
        //        var oSC = list.Where(p => p.Position == "SC").FirstOrDefault();
        //        var oSH = list.Where(p => p.Position == "SH").FirstOrDefault();
        //        var oBM = list.Where(p => p.Position == "BM").FirstOrDefault();
        //        var curdate = DateTime.Now;

        //        return Json(new
        //        {
        //            success = true,
        //            data = new
        //            {
        //                DateFrom = new DateTime(curdate.Year, curdate.Month, 1),
        //                DateTo = curdate,
        //                Nik = (oSL == null) ? "" : oSL.EmployeeID,
        //                Name = (oSL == null) ? "" : oSL.EmployeeName,
        //                NikSC = (oSC == null) ? "" : oSC.EmployeeID,
        //                NameSC = (oSC == null) ? "" : oSC.EmployeeName,
        //                NikSH = (oSH == null) ? "" : oSH.EmployeeID,
        //                NameSH = (oSH == null) ? "" : oSH.EmployeeName,
        //                NikBM = (oBM == null) ? "" : oBM.EmployeeID,
        //                NameBM = (oBM == null) ? "" : oBM.EmployeeName,
        //            }
        //        });
        //    }
        //    else
        //    {
        //        return Json(new { success = false, message = "User tidak/belum link dengan salesman" });
        //    }
        //}

        public JsonResult Combo(string lookup, string positionID, string employeeID, string dealerCode, string outletID)
        {
            string sql = string.Empty;
            string msg = string.Empty;

            #region SALES_ADMIN
            //if (lookup == SALES_ADMIN)
            //{
            //    if (positionID == COO)
            //    {
            //        sql = "select ' ' EmployeeID, '[SELECT ALL]' EmployeeName";
            //    }
            //    else if (positionID == SALES_ADMIN)
            //    {
            //        Position position = ctx.Positions.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.PositionId == BRANCH_MANAGER).FirstOrDefault();
            //        sql = GetChildPosition(position.UserId);
            //    }
            //    else if (positionID == BRANCH_MANAGER)
            //    {
            //        sql = GetChildPosition(CurrentUser.UserId);
            //    }
            //    else if (positionID == SALES_HEAD)
            //    {
            //        sql = GetEmployeeByPosition(positionID, "");
            //    }
            //    else if (positionID == SALES_COORDINATOR)
            //    {
            //        sql = GetEmployeeByPosition(positionID, "");
            //        Position position = ctx.Positions.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.UserId == CurrentUser.UserId).FirstOrDefault();
            //        var salesCoord = ctx.Database.SqlQuery<InqEmployee>(sql).ToList();
            //        var salesCoordID = salesCoord.Where(p => p.EmployeeID == position.EmployeeID).ToList();

            //        sql = GetParentPosition(salesCoordID[0].UserID);
            //    }
            //    else if (positionID == SALESMAN)
            //    {
            //        sql = GetParentPosition(CurrentUser.UserId);
            //        var salesCoord = ctx.Database.SqlQuery<InqEmployee>(sql).ToList();
            //        if (salesCoord.Count == 0)
            //        {
            //            msg = "User belum memiliki Sales Coordinator di Master Team Members ! !";
            //            return Json(new { data = "", message = msg });
            //        }
            //        else
            //        {
            //            sql = GetParentPosition(salesCoord[0].UserID);
            //        }
            //    }
            //}
            #endregion

            #region BRANCH_MANAGER
            if (lookup == BRANCH_MANAGER)
            {
                if (positionID == COO)
                {
                    sql = string.Format(@"
	                    select 
                        tm.EmployeeID, emp.EmployeeName
                        from 
                        PmMstTeamMembers tm 
                        left join GnMstEmployee emp on tm.CompanyCode = emp.CompanyCode 
                            and tm.BranchCode = emp.BranchCode and tm.EmployeeID = emp.EmployeeID
                        where tm.EmployeeID in (select EmployeeID from PmPosition where PositionID = '{1}')
                            and tm.CompanyCode = '{0}'", CompanyCode, BRANCH_MANAGER);

                    if (NationalSLS == "1")
                        sql = string.Format(@"
	                    select distinct BranchHead EmployeeID, BranchHead EmployeeName 
                        from pmHstITS
                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}' and (case when '{1}' = '' then '' else BranchCode end) = '{1}' and BranchHead != ''
                        ", dealerCode == null ? string.Empty : dealerCode, outletID == null ? string.Empty : outletID);
                }
                else if (positionID == SALES_ADMIN)
                {
                    sql = GetEmployeeByPosition(BRANCH_MANAGER, "");
                }
                else if (positionID == BRANCH_MANAGER)
                {
                    sql = GetEmployeeByPosition(positionID, employeeID);
                }
                else if (positionID == SALES_HEAD)
                {
                    sql = GetEmployeeByPosition(positionID, employeeID);
                    var temp1 = ctx.Database.SqlQuery<InqEmployee>(sql).FirstOrDefault();
                    sql = GetParentPosition(temp1.UserID);
                }
                else if (positionID == SALES_COORDINATOR || positionID == SALESMAN)
                {
                    sql = GetEmployeeByPosition(positionID, "");
                    Position position = ctx.Positions.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.UserId == CurrentUser.UserId).FirstOrDefault();
                    var salesCoord = ctx.Database.SqlQuery<InqEmployee>(sql).ToList();
                    var salesCoordID = salesCoord.Where(p => p.EmployeeID == position.EmployeeID).ToList();

                    sql = GetParentPosition(salesCoordID[0].UserID);
                    var salesHead = ctx.Database.SqlQuery<InqEmployee>(sql).ToList();
                    if (salesHead.Count == 0)
                    {
                        msg = "User belum memiliki Sales Head di Master Team Members !";
                        return Json(new { data = "", message = msg });
                    }
                    else
                    {
                        sql = GetParentPosition(salesHead[0].UserID);
                    }
                }
            }
            #endregion

            #region SALES_HEAD
            else if (lookup == SALES_HEAD)
            {
                if (positionID == COO)
                {
                    sql = "select ' ' EmployeeID, '--SELECT ALL--' EmployeeName";
                    if (NationalSLS == "1")
                        sql = string.Format(@"
	                    select distinct SalesHead EmployeeID, SalesHead EmployeeName 
                        from pmHstITS
                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
                            and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
                            and (case when '{2}' = '' then '' else BranchHead end) = '{2}'
                            and SalesHead != ''"
                            , dealerCode, outletID == null ? string.Empty : outletID, employeeID);
                }
                else if (positionID == SALES_ADMIN)
                {
                    sql = GetEmployeeByPosition(BRANCH_MANAGER, "");
                }
                else if (positionID == BRANCH_MANAGER)
                {
                    sql = GetChildPosition(CurrentUser.UserId);
                }
                else if (positionID == SALES_HEAD)
                {
                    sql = GetEmployeeByPosition(positionID, employeeID);
                }
                else if (positionID == SALES_COORDINATOR)
                {
                    sql = GetEmployeeByPosition(positionID, employeeID);
                    var salesCoord = ctx.Database.SqlQuery<InqEmployee>(sql).FirstOrDefault();
                    sql = GetParentPosition(salesCoord.UserID);
                }
                else if (positionID == SALESMAN)
                {
                    sql = GetEmployeeByPosition(positionID, "");
                    Position position = ctx.Positions.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.UserId == CurrentUser.UserId).FirstOrDefault();
                    var salesCoord = ctx.Database.SqlQuery<InqEmployee>(sql).ToList();
                    var salesCoordID = salesCoord.Where(p => p.EmployeeID == position.EmployeeID).ToList();

                    sql = GetParentPosition(salesCoordID[0].UserID);
                    var salesHead = ctx.Database.SqlQuery<InqEmployee>(sql).ToList();
                    if (salesHead.Count == 0)
                    {
                        msg = "User belum memiliki Sales Head di Master Team Members !";
                        return Json(new { data = "", message = msg });
                    }
                    else
                    {
                        sql = GetParentPosition(salesHead[0].UserID);
                    }
                }
            }
            #endregion

            #region SALES_COORDINATOR
            else if (lookup == SALES_COORDINATOR)
            {
                if (positionID == COO || positionID == SALES_ADMIN || positionID == BRANCH_MANAGER)
                {
                    sql = "select ' ' EmployeeID, '--SELECT ALL--' EmployeeName";
                    if (NationalSLS == "1")
                        sql = string.Format(@"
	                    select distinct SalesCoordinator EmployeeID, SalesCoordinator EmployeeName 
                        from pmHstITS
                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
                            and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
                            and (case when '{2}' = '' then '' else SalesHead end) = '{2}'
                            and SalesCoordinator != ''"
                            , dealerCode, outletID == null ? string.Empty : outletID, employeeID);
                }
                else if (positionID == SALES_HEAD)
                {
                    sql = GetChildPosition(CurrentUser.UserId);
                }
                else if (positionID == SALES_COORDINATOR)
                {
                    sql = GetEmployeeByPosition(positionID, employeeID);
                }
                else if (positionID == SALESMAN)
                {
                    sql = GetParentPosition(CurrentUser.UserId);
                }
            }
            #endregion

            #region SALESMAN
            else if (lookup == SALESMAN)
            {
                if (positionID == COO || positionID == SALES_ADMIN || positionID == BRANCH_MANAGER || positionID == SALES_HEAD)
                {
                    sql = "select ' ' EmployeeID, '--SELECT ALL--' EmployeeName";
                    if (NationalSLS == "1")
                        sql = string.Format(@"
	                    select distinct Wiraniaga EmployeeID, Wiraniaga EmployeeName 
                        from pmHstITS
                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
                            and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
                            and (case when '{2}' = '' then '' else SalesCoordinator end) = '{2}'
                            and Wiraniaga != ''"
                            , dealerCode, outletID == null ? string.Empty : outletID, employeeID);
                }
                else if (positionID == SALES_COORDINATOR)
                {
                    sql = GetChildPosition(CurrentUser.UserId);
                }
                else if (positionID == SALESMAN)
                {
                    sql = GetEmployeeByPosition(positionID, employeeID);
                }
            }
            #endregion

            var list = (from p in ctx.Database.SqlQuery<InqEmployee>(sql)
                        select new { value = p.EmployeeID, text = p.EmployeeName }).ToList();

            if (list.Count() == 0)
            {
                if (lookup == BRANCH_MANAGER)
                {
                    if (positionID == SALES_HEAD)
                        msg = "User belum memiliki Sales Head di Master Team Members !";

                    if (positionID == SALES_COORDINATOR || positionID == SALESMAN)
                        msg = "User belum memiliki Branch Manager di Master Team Members !";
                }
                else if (lookup == SALES_HEAD)
                {
                    if (positionID == COO)
                        msg = "User belum memiliki Branch Manager di Master Team Members !";

                    if (positionID == SALES_ADMIN || positionID == BRANCH_MANAGER || positionID == SALES_COORDINATOR || positionID == SALESMAN)
                        msg = "User belum memiliki Sales Head di Master Team Members !";
                }
                else if (lookup == SALES_COORDINATOR)
                {
                    if (positionID == SALES_HEAD || positionID == SALESMAN)
                        msg = "User belum memiliki Sales Coordinator di Master Team Members !";
                }
                else if (lookup == SALESMAN)
                {
                    if (positionID == SALES_COORDINATOR)
                        msg = "User belum memiliki Salesman di Master Team Members !";
                }
            }

            if (msg != string.Empty)
                return Json(new { message = msg });
            else
                return Json(list);
        }

        public JsonResult ComboSalesman(string EmployeeID, string lookup, string dealer, string outlet)
        {
            Position position = ctx.Positions.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.EmployeeID == EmployeeID).FirstOrDefault();
            string sql = string.Empty;

            if (NationalSLS == "1")
            {
                if (lookup == BRANCH_MANAGER)
                {
                    sql = string.Format(@"
	                    select distinct BranchHead EmployeeID, BranchHead EmployeeName 
                        from pmHstITS
                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}' and (case when '{1}' = '' then '' else BranchCode end) = '{1}' and BranchHead != ''
                        ", dealer == null ? string.Empty : dealer, outlet == null ? string.Empty : outlet);
                }
                else if (lookup == SALES_HEAD)
                {
                    sql = string.Format(@"
	                    select distinct SalesHead EmployeeID, SalesHead EmployeeName 
                        from pmHstITS
                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
                            and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
                            and (case when '{2}' = '' then '' else BranchHead end) = '{2}'
                            and SalesHead != ''"
                           , dealer, outlet == null ? string.Empty : outlet, EmployeeID);
                }
                else if (lookup == SALES_COORDINATOR)
                {
                    sql = string.Format(@"
	                    select distinct SalesCoordinator EmployeeID, SalesCoordinator EmployeeName 
                        from pmHstITS
                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
                            and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
                            and (case when '{2}' = '' then '' else SalesHead end) = '{2}'
                            and SalesCoordinator != ''"
                           , dealer, outlet == null ? string.Empty : outlet, EmployeeID);
                }
                else if (lookup == SALESMAN)
                {
                    sql = string.Format(@"
	                    select distinct Wiraniaga EmployeeID, Wiraniaga EmployeeName 
                        from pmHstITS
                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
                            and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
                            and (case when '{2}' = '' then '' else SalesCoordinator end) = '{2}'
                            and Wiraniaga != ''"
                            , dealer, outlet == null ? string.Empty : outlet, EmployeeID);
                }
            }
            else
            {
                if (position != null)
                {
                    //sql = GetChildPosition(position.UserId);
                    if (lookup == BRANCH_MANAGER)
                    {
                        sql = string.Format(@"SELECT 
						distinct(a.EmployeeID), a.EmployeeName
                        FROM
                                HrEmployee a
				        where a.Department = 'sales'
				        and a.position ='BM' and a.PersonnelStatus =1");
                    }

                    if (lookup == SALES_HEAD)
                    {
                        sql = string.Format(@"SELECT 
						distinct(a.EmployeeID), a.EmployeeName
                        FROM
                                HrEmployee a
				        where a.Department = 'sales'
				        and a.position ='SH' and a.PersonnelStatus =1");
                    }

                    if (lookup == SALESMAN)
                    {
                        sql = string.Format(@"SELECT 
						distinct(a.EmployeeID), a.EmployeeName
                        FROM
                                HrEmployee a
				        where a.Department = 'sales'
				        and a.position ='S' and a.PersonnelStatus =1");
                    }
                }
                else
                {
                    sql = "select ' ' EmployeeID, '--SELECT ALL--' EmployeeName";
                }
            }


            var list = (from p in ctx.Database.SqlQuery<InqEmployee>(sql)
                        select new { value = p.EmployeeID, text = p.EmployeeName }).ToList();

            //if (position.PositionId == "30" && lookup == BRANCH_MANAGER)
            //{
            //    var tmldr = ctx.HrEmployees.Where(a => a.EmployeeID == EmployeeID).FirstOrDefault().TeamLeader;
            //    list = (from p in ctx.Database.SqlQuery<InqEmployee>(sql)
            //            where EmployeeID == tmldr    
            //            select new { value = p.EmployeeID, text = p.EmployeeName }).ToList();
            //}

            return Json(list);
        }

        private string GetEmployeeByPosition(string positionID, string employeeID)
        {
            string sql = string.Format(@"SELECT 
                        a.EmployeeID, b.EmployeeName, a.UserID
                    FROM 
                        pmPosition a
                    LEFT JOIN gnMstEmployee b
                        ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.EmployeeID = a.EmployeeID
                    WHERE
                        a.CompanyCode = '{0}' AND ((CASE WHEN '{1}' ='' THEN a.BranchCode END)<>'' 
                        OR (CASE WHEN '{1}' <>'' THEN a.BranchCode END) = '{1}') 
                        AND a.PositionId = '{2}'
                        AND (CASE WHEN '{3}' = '' THEN '' ELSE a.EmployeeID END) = '{3}'
                    ORDER BY b.EmployeeName", CompanyCode, BranchCode, positionID, employeeID);

            return sql;
        }

        private string GetParentPosition(string userID)
        {
            string sql = string.Format(@"SELECT 
                        a.employeeID, b.EmployeeName, c.OutletID, d.OutletName, e.UserID
                    FROM
                        PmMstTeamMembers a
                    LEFT JOIN GnMstEmployee b
                        ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode 
                        AND b.EmployeeId = a.EmployeeID
                    LEFT JOIN PmPosition c
                        ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode 
                        AND c.UserID = '{2}'
                    LEFT JOIN PmBranchOutlets d
	                    ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode 
                        AND d.OutletID = c.OutletID
                    LEFT JOIN PmPosition e
                        ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode 
                        AND e.EmployeeID = a.EmployeeID
                    WHERE
                        a.CompanyCode = '{0}' AND a.BranchCode = '{1}' AND a.IsSupervisor = 1
                        AND TeamID = (SELECT TeamID FROM PmMStteamMembers WHERE CompanyCode ='{0}'
                        AND BranchCode = '{1}' AND EmployeeID = c.EmployeeID AND IsSupervisor = 0)", CompanyCode, BranchCode, userID);

            return sql;
        }

        private string GetChildPosition(string userID)
        {
            string sql = string.Format(@"SELECT 
                        a.EmployeeID, b.EmployeeName, c.OutletID, d.OutletName
                    FROM
                        PmMstTeamMembers a
                    LEFT JOIN GnMstEmployee b
                        ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode 
                        AND b.EmployeeId = a.EmployeeID
                    LEFT JOIN PmPosition c
                        ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode 
                        AND c.UserID = '{2}'
                    LEFT JOIN PmBranchOutlets d
	                    ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode 
                        AND d.OutletID = c.OutletID
                    WHERE
                        a.CompanyCode = '{0}'
                        AND ((CASE WHEN '{1}' = '' THEN a.BranchCode END) <> '' OR (CASE WHEN '{1}' <> '' THEN a.BranchCode END) = '{1}') 
                        AND a.IsSupervisor = 0 
                        AND TeamID = (SELECT TeamID FROM PmMstTeamMembers WHERE CompanyCode = '{0}'
                            AND ((CASE WHEN '{1}' = '' THEN BranchCode END) <> '' OR (CASE WHEN '{1}' <> '' THEN BranchCode END) = '{1}') 
                            AND EmployeeID = c.EmployeeID AND IsSupervisor = 1)", CompanyCode, BranchCode, userID);

            return sql;
        }

        public JsonResult LoadData(string branchManager)
        {
            return Json(new
            {
                BranchManager = branchManager
            });
        }

        public JsonResult DealerMappingAreas()
        {
            string sql = string.Format("exec uspfn_gnInquiryBtn '', '{0}', '', '1'", NationalSLS == "0" ? CompanyCode : "");

            var list = (from p in ctx.Database.SqlQuery<InquiryBtn>(sql)
                        select new InquiryBtn()
                       {
                           Area = p.Area
                       }).Select(p => new { value = p.Area, text = p.Area }).ToList();
            return Json(list);
        }

        public JsonResult DealerMappingDealers(string area)
        {
            string sql = string.Format("exec uspfn_gnInquiryBtn '{0}', '{1}', '', '2'", area, NationalSLS == "0" ? CompanyCode : "");

            var list = (from p in ctx.Database.SqlQuery<InquiryBtn>(sql)
                        select new InquiryBtn()
                        {
                            DealerCode = p.DealerCode,
                            DealerName = p.DealerName
                        }).Select(p => new { value = p.DealerCode, text = p.DealerName }).ToList();
            return Json(list);
        }

        public JsonResult Outlets(string area, string dealer)
        {
            string sql = string.Format("exec uspfn_gnInquiryBtn '{0}', '{1}', '', '3'", area, dealer);

            var list = (from p in ctx.Database.SqlQuery<InquiryBtn>(sql)
                        select new InquiryBtn()
                        {
                            OutletCode = p.OutletCode,
                            OutletName = p.OutletName
                        }).Select(p => new { value = p.OutletCode, text = p.OutletName }).ToList();
            return Json(list);
        }

        public JsonResult ReportTypes()
        {
            List<Combo> listCombo = new List<Combo>();

            Combo combo1 = new Combo();
            combo1.Value = "0";
            combo1.Text = "Summary Inquiry";

            listCombo.Add(combo1);

            Combo combo2 = new Models.Combo();
            combo2.Value = "1";
            combo2.Text = "Saldo Inquiry";

            listCombo.Add(combo2);

            return Json(listCombo.Select(p => new { value = p.Value, text = p.Text }).ToList());
        }

        public JsonResult ProductivityBy()
        {
            List<Combo> listCombo = new List<Combo>();

            Combo combo1 = new Combo();
            combo1.Value = "0";
            combo1.Text = "Salesman";

            listCombo.Add(combo1);

            Combo combo2 = new Models.Combo();
            combo2.Value = "1";
            combo2.Text = "Vehicle Type";

            listCombo.Add(combo2);

            Combo combo3 = new Models.Combo();
            combo3.Value = "2";
            combo3.Text = "Source Data";

            listCombo.Add(combo3);

            return Json(listCombo.Select(p => new { value = p.Value, text = p.Text }).ToList());
        }

        public JsonResult ExportInqWithSts(DateTime DateFrom, DateTime DateTo, string Area, string GroupModel, string ModelType, string Variant)
        {
            //string pDateFrom = Request["DateFrom"];
            //string pDateTo = Request["DateTo"];

            //DateTime dtStart = Convert.ToDateTime(Request["DateFrom"]);
            DateTime dtLastStart = DateFrom.AddMonths(-1);
            string pLastDateFrom = dtLastStart.ToString("yyyyMMdd");

            //DateTime dtEnd = Convert.ToDateTime(Request["DateTo"]);
            DateTime dtLastEnd = DateTo.AddMonths(-1);
            string pLastDateTo = dtLastEnd.ToString("yyyyMMdd");

            //string pArea = Request["Area"];
            //string pGroupModel = Request["GroupModel"];
            //string pModelType = Request["ModelType"];
            //string pVariant = Request["Variant"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_InquiryITSWithStatus";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@StartDate", DateFrom.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@EndDate", DateTo.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@LastStartDate", pLastDateFrom);
            cmd.Parameters.AddWithValue("@LastEndDate", pLastDateTo);
            cmd.Parameters.AddWithValue("@Area", Area);
            cmd.Parameters.AddWithValue("@GroupModel", GroupModel);
            cmd.Parameters.AddWithValue("@TipeKendaraan", ModelType);
            cmd.Parameters.AddWithValue("@Variant", Variant);
            cmd.Parameters.AddWithValue("@Summary", false);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            RenderReport(Server.MapPath("~/Reports/rdlc/its/itsinqwithsts.rdlc"), "InqItsWithStatus", 10, 11.7, "excel", GetJson(dt).ToList());
            return null;
        }

        public ActionResult CreateExcelInqWithSts(DateTime DateFrom, DateTime DateTo, string Area, string Dealer, string Outlet, string GroupModel, string ModelType, string Variant)
        {
            //ExcelFileWriter excelReport;
            string fileName = "InqItsWithStatus";
            //excelReport = new ExcelFileWriter(fileName, DateTime.Now.ToString("dd-MMM-yyyy"), "");

            DateTime dtTempLastStart = DateFrom.AddMonths(-1);
            DateTime dtLastStart = new DateTime(dtTempLastStart.Year, dtTempLastStart.Month, 1);
            string pLastDateFrom = dtLastStart.ToString("yyyyMMdd");

            //DateTime dtEnd = Convert.ToDateTime(Request["DateTo"]);
            DateTime dtTempLastEnd = DateTo.AddMonths(-1);
            int daysInMonth = DateTime.DaysInMonth(dtTempLastEnd.Year, dtTempLastEnd.Month);
            DateTime dtLastEnd = new DateTime(dtTempLastEnd.Year, dtTempLastEnd.Month, daysInMonth);

            string pLastDateTo = dtLastEnd.ToString("yyyyMMdd");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_InquiryITSWithStatus";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", Dealer);
            cmd.Parameters.AddWithValue("@BranchCode", Outlet);
            cmd.Parameters.AddWithValue("@StartDate", DateFrom.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@EndDate", DateTo.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@LastStartDate", pLastDateFrom);
            cmd.Parameters.AddWithValue("@LastEndDate", pLastDateTo);
            cmd.Parameters.AddWithValue("@Area", Area);
            cmd.Parameters.AddWithValue("@GroupModel", GroupModel);
            cmd.Parameters.AddWithValue("@TipeKendaraan", ModelType);
            cmd.Parameters.AddWithValue("@Variant", Variant);
            cmd.Parameters.AddWithValue("@Summary", 0);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet dt = new DataSet();
            da.Fill(dt);

            string pPeriode = DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");
            if (Dealer != string.Empty)
                Dealer = (from p in ctx.DealerMappings
                          where p.DealerCode == Dealer
                          select p.DealerName).FirstOrDefault();
            else
                Dealer = "ALL";

            if (Outlet != string.Empty)
                Outlet = (from p in ctx.DealerOutletMappings
                          where p.OutletCode == Outlet
                          select p.OutletName).FirstOrDefault();
            else
                Outlet = "ALL";
            ExcelFileWriter excelReport = CreateReport(fileName, dt, pPeriode, Area, Dealer, Outlet);
            //CreateExcel(excelReport, Convert.ToInt16(model.Year), Convert.ToInt16(model.Month), model.ProvinceCode, model.RegencyCode, Convert.ToInt16(model.Show));
            string result = excelReport.CloseExcelFileWriter();

            HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + fileName + ".xls");
            this.Response.ContentType = "application/vnd.ms-excel";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(result);
            return File(buffer, "application/vnd.ms-excel");
        }

        public ExcelFileWriter CreateReport(string fileName, DataSet dt0, string pPeriode, string pArea, string pDealer, string pOutlet)
        {
            string companyName = "";
            //if (reportType == 0)
            //{
            //    #region By Dealer

            //    ExcelFileWriter excelReport = new ExcelFileWriter(fileName, "Summary", "Inquiry ITS With Status");
            //    CreateHeaderSummary(excelReport);

            //    foreach (DataRow row in dt0.Tables[0].Rows)
            //    {
            //        if (rowIndex % 6 == 0)
            //        {
            //            excelReport.SetCellValue(row["Area"].ToString(), 8 + rowIndex, 0, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotal : ExcelCellStyle.LeftBorderedStandardWrap);
            //            excelReport.SetCellValue(row["CompanyName"].ToString(), 8 + rowIndex, 1, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotal : ExcelCellStyle.LeftBorderedStandardWrap);
            //            excelReport.SetCellValue(row["BranchName"].ToString(), 8 + rowIndex, 2, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotal : ExcelCellStyle.LeftBorderedStandardWrap);
            //        }
            //        excelReport.SetCellValue(row["LastProgress"].ToString(), 8 + rowIndex, 3, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.LeftBorderedStandardWrap);
            //        excelReport.SetCellValue(row["SaldoAwal"].ToString(), 8 + rowIndex, 4, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["WeekOuts1"].ToString(), 8 + rowIndex, 5, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["WeekOuts2"].ToString(), 8 + rowIndex, 6, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["WeekOuts3"].ToString(), 8 + rowIndex, 7, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["WeekOuts4"].ToString(), 8 + rowIndex, 8, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["WeekOuts5"].ToString(), 8 + rowIndex, 9, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["WeekOuts6"].ToString(), 8 + rowIndex, 10, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["TotalWeekOuts"].ToString(), 8 + rowIndex, 11, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Week1"].ToString(), 8 + rowIndex, 12, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Week2"].ToString(), 8 + rowIndex, 13, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Week3"].ToString(), 8 + rowIndex, 14, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Week4"].ToString(), 8 + rowIndex, 15, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Week5"].ToString(), 8 + rowIndex, 16, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Week6"].ToString(), 8 + rowIndex, 17, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["TotalWeek"].ToString(), 8 + rowIndex, 18, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Total"].ToString(), 8 + rowIndex, 19, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GrayTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);

            //        rowIndex++;
            //    }
            //    companyName = dt0.Tables[1].Rows[0]["CompanyName"].ToString();
            //    excelReport.ChangeSheet(companyName);
            //    CreateHeader(excelReport);

            //    foreach (DataRow row in dt0.Tables[1].Rows)
            //    {
            //        if (companyName != row["CompanyName"].ToString())
            //        {
            //            companyName = row["CompanyName"].ToString();

            //            excelReport.ChangeSheet(companyName);
            //            CreateHeader(excelReport);
            //        }

            //        if (rowIndex % 6 == 0)
            //        {
            //            excelReport.SetCellValue(row["Area"].ToString(), 8 + rowIndex, 0, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
            //            excelReport.SetCellValue(row["CompanyName"].ToString(), 8 + rowIndex, 1, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
            //            excelReport.SetCellValue(row["BranchName"].ToString(), 8 + rowIndex, 2, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
            //            excelReport.SetCellValue(row["TipeKendaraan"].ToString(), 8 + rowIndex, 3, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
            //            excelReport.SetCellValue(row["Variant"].ToString(), 8 + rowIndex, 4, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
            //        }
            //        excelReport.SetCellValue(row["LastProgress"].ToString(), 8 + rowIndex, 5, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotal : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotal : ExcelCellStyle.LeftBorderedStandardWrap);
            //        excelReport.SetCellValue(row["SaldoAwal"].ToString(), 8 + rowIndex, 6, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["WeekOuts1"].ToString(), 8 + rowIndex, 7, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["WeekOuts2"].ToString(), 8 + rowIndex, 8, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["WeekOuts3"].ToString(), 8 + rowIndex, 9, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["WeekOuts4"].ToString(), 8 + rowIndex, 10, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["WeekOuts5"].ToString(), 8 + rowIndex, 11, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["WeekOuts6"].ToString(), 8 + rowIndex, 12, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["TotalWeekOuts"].ToString(), 8 + rowIndex, 13, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Week1"].ToString(), 8 + rowIndex, 14, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Week2"].ToString(), 8 + rowIndex, 15, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Week3"].ToString(), 8 + rowIndex, 16, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Week4"].ToString(), 8 + rowIndex, 17, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Week5"].ToString(), 8 + rowIndex, 18, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Week6"].ToString(), 8 + rowIndex, 19, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["TotalWeek"].ToString(), 8 + rowIndex, 20, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
            //        excelReport.SetCellValue(row["Total"].ToString(), 8 + rowIndex, 21, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.GrayTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.BrownTotalNumber : (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "2" && row["OrderNo1"].ToString() == "2" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.GreenTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);

            //        rowIndex++;
            //    }
            //    excelReport.CloseExcelFileWriter();
            //    #endregion
            //}
            //else
            //{
            #region By Type
            int rowIndex = 0;
            ExcelFileWriter excelReport = new ExcelFileWriter(fileName, "Summary", "Inquiry ITS With Status");
            CreateHeaderSummary(ref excelReport, ref rowIndex, pPeriode, pArea, pDealer, pOutlet);

            foreach (DataRow row in dt0.Tables[0].Rows)
            {
                if (rowIndex % 6 == 0)
                {
                    excelReport.SetCellValue(row["Area"].ToString(), 9 + rowIndex, 0, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                    excelReport.SetCellValue(row["CompanyName"].ToString() != "ZTOTAL" ? row["CompanyName"].ToString() : "", 9 + rowIndex, 1, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                    excelReport.SetCellValue(row["BranchName"].ToString(), 9 + rowIndex, 2, 6, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                }
                excelReport.SetCellValue(row["LastProgress"].ToString(), 9 + rowIndex, 3, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                excelReport.SetCellValue(row["SaldoAwalLast"].ToString(), 9 + rowIndex, 4, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts1Last"].ToString(), 9 + rowIndex, 5, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts2Last"].ToString(), 9 + rowIndex, 6, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts3Last"].ToString(), 9 + rowIndex, 7, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts4Last"].ToString(), 9 + rowIndex, 8, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts5Last"].ToString(), 9 + rowIndex, 9, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["TotalWeekOutsLast"].ToString(), 9 + rowIndex, 10, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week1Last"].ToString(), 9 + rowIndex, 11, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week2Last"].ToString(), 9 + rowIndex, 12, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week3Last"].ToString(), 9 + rowIndex, 13, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week4Last"].ToString(), 9 + rowIndex, 14, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week5Last"].ToString(), 9 + rowIndex, 15, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["TotalWeekLast"].ToString(), 9 + rowIndex, 16, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["TotalLast"].ToString(), 9 + rowIndex, 17, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);

                excelReport.SetCellValue(row["SaldoAwal"].ToString(), 9 + rowIndex, 18, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts1"].ToString(), 9 + rowIndex, 19, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts2"].ToString(), 9 + rowIndex, 20, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts3"].ToString(), 9 + rowIndex, 21, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts4"].ToString(), 9 + rowIndex, 22, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts5"].ToString(), 9 + rowIndex, 23, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["TotalWeekOuts"].ToString(), 9 + rowIndex, 24, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week1"].ToString(), 9 + rowIndex, 25, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week2"].ToString(), 9 + rowIndex, 26, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week3"].ToString(), 9 + rowIndex, 27, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week4"].ToString(), 9 + rowIndex, 28, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week5"].ToString(), 9 + rowIndex, 29, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["TotalWeek"].ToString(), 9 + rowIndex, 30, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 31, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["TotPercent"].ToString(), 9 + rowIndex, 32, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalDecimal : ExcelCellStyle.RightBorderedStandardDecimal, true);

                rowIndex++;
            }

            companyName = (dt0.Tables[1].Rows[0]["TipeKendaraan"].ToString() == "" ? "Unknown" : dt0.Tables[1].Rows[0]["TipeKendaraan"].ToString());
            excelReport.ChangeSheet(companyName);
            CreateHeaderbyType(excelReport, ref rowIndex, pPeriode, pArea, pDealer, pOutlet);
            excelReport.SetCellValue("Tipe" + companyName, 6, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + companyName, 6, 1, 1, 12, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue("Last Month Result", 7, 4, 1, 14, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("This Month Result", 7, 18, 1, 14, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 7, 32, 1, 1, ExcelCellStyle.LeftBorderedBold);
            rowIndex++;

            excelReport.SetCellValue("Area", 7 + rowIndex, 0, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Dealer", 7 + rowIndex, 1, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outlet", 7 + rowIndex, 2, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Status", 7 + rowIndex, 3, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Saldo Awal", 7 + rowIndex, 4, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outstanding", 7 + rowIndex, 5, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("New", 7 + rowIndex, 11, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 7 + rowIndex, 17, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Saldo Awal", 7 + rowIndex, 18, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outstanding", 7 + rowIndex, 19, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("New", 7 + rowIndex, 25, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 7 + rowIndex, 31, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("VS Last Month", 7 + rowIndex, 32, 2, 1, ExcelCellStyle.LeftBorderedBoldWrap);

            excelReport.SetCellValue("1st-7th", 8 + rowIndex, 5, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8 + rowIndex, 6, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8 + rowIndex, 7, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 8, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8 + rowIndex, 9, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8 + rowIndex, 10, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("1st-7th", 8 + rowIndex, 11, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8 + rowIndex, 12, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8 + rowIndex, 13, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 14, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8 + rowIndex, 15, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8 + rowIndex, 16, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("1st-7th", 8 + rowIndex, 19, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8 + rowIndex, 20, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8 + rowIndex, 21, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 22, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8 + rowIndex, 23, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8 + rowIndex, 24, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("1st-7th", 8 + rowIndex, 25, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8 + rowIndex, 26, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8 + rowIndex, 27, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 28, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8 + rowIndex, 29, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8 + rowIndex, 30, 1, 1, ExcelCellStyle.CenterBorderedBold);


            int count = 0;
            foreach (DataRow row in dt0.Tables[1].Rows)
            {
                if (companyName != (row["TipeKendaraan"].ToString() == "" ? "Unknown" : row["TipeKendaraan"].ToString()))
                {
                    companyName = (row["TipeKendaraan"].ToString() == "" ? "Unknown" : row["TipeKendaraan"].ToString());

                    excelReport.ChangeSheet(companyName);
                    CreateHeaderbyType(excelReport, ref rowIndex, pPeriode, pArea, pDealer, pOutlet);

                    excelReport.SetCellValue("Tipe" + companyName, 6, 0, 1, 1, ExcelCellStyle.LeftBold);
                    excelReport.SetCellValue(": " + companyName, 6, 1, 1, 12, ExcelCellStyle.LeftBold);
                    excelReport.SetCellValue("Last Month Result", 7, 4, 1, 14, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("This Month Result", 7, 18, 1, 14, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("%", 7, 32, 1, 1, ExcelCellStyle.LeftBorderedBold);
                    rowIndex++;

                    excelReport.SetCellValue("Area", 7 + rowIndex, 0, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Dealer", 7 + rowIndex, 1, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Outlet", 7 + rowIndex, 2, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Status", 7 + rowIndex, 3, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Saldo Awal", 7 + rowIndex, 4, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Outstanding", 7 + rowIndex, 5, 1, 6, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("New", 7 + rowIndex, 11, 1, 6, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("Total", 7 + rowIndex, 17, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Saldo Awal", 7 + rowIndex, 18, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("Outstanding", 7 + rowIndex, 19, 1, 6, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("New", 7 + rowIndex, 25, 1, 6, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("Total", 7 + rowIndex, 31, 2, 1, ExcelCellStyle.LeftBorderedBold);
                    excelReport.SetCellValue("VS Last Month", 7 + rowIndex, 32, 2, 1, ExcelCellStyle.LeftBorderedBoldWrap);

                    excelReport.SetCellValue("1st-7th", 8 + rowIndex, 5, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("8th-14th", 8 + rowIndex, 6, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("15th-21st", 8 + rowIndex, 7, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 8, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("29th-31st", 8 + rowIndex, 9, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("Sub Total", 8 + rowIndex, 10, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("1st-7th", 8 + rowIndex, 11, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("8th-14th", 8 + rowIndex, 12, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("15th-21st", 8 + rowIndex, 13, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 14, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("29th-31st", 8 + rowIndex, 15, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("Sub Total", 8 + rowIndex, 16, 1, 1, ExcelCellStyle.CenterBorderedBold);

                    excelReport.SetCellValue("1st-7th", 8 + rowIndex, 19, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("8th-14th", 8 + rowIndex, 20, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("15th-21st", 8 + rowIndex, 21, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 22, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("29th-31st", 8 + rowIndex, 23, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("Sub Total", 8 + rowIndex, 24, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("1st-7th", 8 + rowIndex, 25, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("8th-14th", 8 + rowIndex, 26, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("15th-21st", 8 + rowIndex, 27, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("22nd-28th", 8 + rowIndex, 28, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("29th-31st", 8 + rowIndex, 29, 1, 1, ExcelCellStyle.CenterBorderedBold);
                    excelReport.SetCellValue("Sub Total", 8 + rowIndex, 30, 1, 1, ExcelCellStyle.CenterBorderedBold);
                }

                if (count == 0)
                {
                    excelReport.SetCellValue(row["Area"].ToString(), 9 + rowIndex, 0, 6, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                    excelReport.SetCellValue(row["CompanyName"].ToString() != "ZTOTAL" ? row["CompanyName"].ToString() : "", 9 + rowIndex, 1, 6, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                    excelReport.SetCellValue(row["BranchName"].ToString(), 9 + rowIndex, 2, 6, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                    count = 6;
                }

                //excelReport.SetCellValue(row["LastProgress"].ToString(), 9 + rowIndex, 3, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                //excelReport.SetCellValue(row["SaldoAwal"].ToString(), 9 + rowIndex, 4, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["WeekOuts1"].ToString(), 9 + rowIndex, 5, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["WeekOuts2"].ToString(), 9 + rowIndex, 6, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["WeekOuts3"].ToString(), 9 + rowIndex, 7, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["WeekOuts4"].ToString(), 9 + rowIndex, 8, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["WeekOuts5"].ToString(), 9 + rowIndex, 9, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                ////excelReport.SetCellValue(row["WeekOuts6"].ToString(), 9 + rowIndex, 10, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["TotalWeekOuts"].ToString(), 9 + rowIndex, 10, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["Week1"].ToString(), 9 + rowIndex, 11, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["Week2"].ToString(), 9 + rowIndex, 12, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["Week3"].ToString(), 9 + rowIndex, 13, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["Week4"].ToString(), 9 + rowIndex, 14, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["Week5"].ToString(), 9 + rowIndex, 15, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                ////excelReport.SetCellValue(row["Week6"].ToString(), 9 + rowIndex, 17, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["TotalWeek"].ToString(), 9 + rowIndex, 16, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                //excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 17, 1, 1, (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);

                excelReport.SetCellValue(row["LastProgress"].ToString(), 9 + rowIndex, 3, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotal : ExcelCellStyle.LeftBorderedStandardWrap);
                excelReport.SetCellValue(row["SaldoAwalLast"].ToString(), 9 + rowIndex, 4, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts1Last"].ToString(), 9 + rowIndex, 5, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts2Last"].ToString(), 9 + rowIndex, 6, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts3Last"].ToString(), 9 + rowIndex, 7, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts4Last"].ToString(), 9 + rowIndex, 8, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts5Last"].ToString(), 9 + rowIndex, 9, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["TotalWeekOutsLast"].ToString(), 9 + rowIndex, 10, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week1Last"].ToString(), 9 + rowIndex, 11, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week2Last"].ToString(), 9 + rowIndex, 12, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week3Last"].ToString(), 9 + rowIndex, 13, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week4Last"].ToString(), 9 + rowIndex, 14, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week5Last"].ToString(), 9 + rowIndex, 15, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["TotalWeekLast"].ToString(), 9 + rowIndex, 16, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["TotalLast"].ToString(), 9 + rowIndex, 17, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);

                excelReport.SetCellValue(row["SaldoAwal"].ToString(), 9 + rowIndex, 18, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts1"].ToString(), 9 + rowIndex, 19, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts2"].ToString(), 9 + rowIndex, 20, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts3"].ToString(), 9 + rowIndex, 21, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts4"].ToString(), 9 + rowIndex, 22, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["WeekOuts5"].ToString(), 9 + rowIndex, 23, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["TotalWeekOuts"].ToString(), 9 + rowIndex, 24, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week1"].ToString(), 9 + rowIndex, 25, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week2"].ToString(), 9 + rowIndex, 26, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week3"].ToString(), 9 + rowIndex, 27, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week4"].ToString(), 9 + rowIndex, 28, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Week5"].ToString(), 9 + rowIndex, 29, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["TotalWeek"].ToString(), 9 + rowIndex, 30, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["Total"].ToString(), 9 + rowIndex, 31, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalNumber : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalNumber : ExcelCellStyle.RightBorderedStandardNumber, true);
                excelReport.SetCellValue(row["TotPercent"].ToString(), 9 + rowIndex, 32, 1, 1, (row["OrderNo3"].ToString() == "2" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "2") ? ExcelCellStyle.PinkTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "1") ? ExcelCellStyle.PinkTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "1" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.YellowTotalDecimal : (row["OrderNo3"].ToString() == "1" && row["OrderNo"].ToString() == "1" && row["OrderNo1"].ToString() == "0" && row["OrderNo2"].ToString() == "0") ? ExcelCellStyle.BlueTotalDecimal : ExcelCellStyle.RightBorderedStandardDecimal, true);

                count--;
                rowIndex++;
            }
            //excelReport.CloseExcelFileWriter();
            #endregion
            //}

            return excelReport;
        }

        private void CreateHeaderSummary(ref ExcelFileWriter excelReport, ref int rowIndex, string pPeriode, string pArea, string pDealer, string pOutlet)
        {
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);

            excelReport.SetCellValue("ITS Status Reports (Summary)", 0, 0, 1, 13, ExcelCellStyle.Header, false, "20");
            excelReport.FreezeCols(0, 4);

            excelReport.SetCellValue("Periode ", 2, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pPeriode, 2, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Area", 3, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pArea, 3, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Dealer", 4, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pDealer, 4, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Outlet", 5, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pOutlet, 5, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Last Month Result", 6, 4, 1, 14, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("This Month Result", 6, 18, 1, 14, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("%", 6, 32, 1, 1, ExcelCellStyle.LeftBorderedBold);

            excelReport.SetCellValue("Area", 7, 0, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Dealer", 7, 1, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outlet", 7, 2, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Status", 7, 3, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Saldo Awal", 7, 4, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outstanding", 7, 5, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("New", 7, 11, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 7, 17, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Saldo Awal", 7, 18, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("Outstanding", 7, 19, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("New", 7, 25, 1, 6, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Total", 7, 31, 2, 1, ExcelCellStyle.LeftBorderedBold);
            excelReport.SetCellValue("VS Last Month", 7, 32, 2, 1, ExcelCellStyle.LeftBorderedBoldWrap);

            excelReport.SetCellValue("1st-7th", 8, 5, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 6, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 7, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 8, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 9, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8, 10, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("1st-7th", 8, 11, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 12, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 13, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 14, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 15, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8, 16, 1, 1, ExcelCellStyle.CenterBorderedBold);

            excelReport.SetCellValue("1st-7th", 8, 19, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 20, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 21, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 22, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 23, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8, 24, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("1st-7th", 8, 25, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("8th-14th", 8, 26, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("15th-21st", 8, 27, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("22nd-28th", 8, 28, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("29th-31st", 8, 29, 1, 1, ExcelCellStyle.CenterBorderedBold);
            excelReport.SetCellValue("Sub Total", 8, 30, 1, 1, ExcelCellStyle.CenterBorderedBold);

            rowIndex = 0;
        }

        private void CreateHeaderbyType(ExcelFileWriter excelReport, ref int rowIndex, string pPeriode, string pArea, string pDealer, string pOutlet)
        {
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);
            excelReport.SettingColumnWidth(1);

            excelReport.SetCellValue("ITS Status Reports (Detail)", 0, 0, 1, 13, ExcelCellStyle.Header, false, "20");

            excelReport.SetCellValue("Periode ", 2, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pPeriode, 2, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Area", 3, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pArea, 3, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Dealer", 4, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pDealer, 4, 1, 1, 12, ExcelCellStyle.LeftBold);

            excelReport.SetCellValue("Outlet", 5, 0, 1, 1, ExcelCellStyle.LeftBold);
            excelReport.SetCellValue(": " + pOutlet, 5, 1, 1, 12, ExcelCellStyle.LeftBold);

            rowIndex = 0;
        }

        public ActionResult inquiryIts(string StartDate, string EndDate, string Area, string Dealer, string Outlet, string SpID, string DName, string OName)
        {
            string fileName = "";
            fileName = "ITS Report" + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            string dtAwal = (Convert.ToDateTime(StartDate)).ToString("dd-MMM-yyyy");
            string dtAkhir = (Convert.ToDateTime(EndDate)).ToString("dd-MMM-yyyy");

            decimal prcntNow = 0, prcntLate = 0, prcntTotal = 0, prcntHPNow = 0, prcntHPLate = 0, prcntHPTotal = 0;
            decimal ttlNewINQ = 0, ttlNewHPINQ = 0, ttlNewSPK = 0, ttlOutsINQ = 0, ttlOutsHPINQ = 0, ttlOutsSPK = 0;
            decimal gtINQ = 0, gtHP = 0, gtSPK = 0, ttlCancel = 0, ttlLost = 0, ttlTD = 0, ttlFP = 0, ttlSOH = 0;
            decimal ttlprcntNow = 0, ttlprcntLate = 0, ttlprcntTotal = 0, ttlprcntHPNow = 0, ttlprcntHPLate = 0, ttlprcntHPTotal = 0;

            string OutletName = OName;

            if (OutletName == "" || OutletName == null)
            {
                OutletName = "All";
            }

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "EXEC " + SpID + " '" + StartDate + "','" + EndDate + "','" + Area + "','" + Dealer + "','" + Outlet + "'";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            #region ** format Excell **
            else
            {
                int lastRow = 9;
                int rowIndex = 0;

                //var sheetName = "sheet";
                var sheetName = ds.Tables[0].Rows[0][3].ToString();
                var sheets = "";
                var wb = new XLWorkbook();

                #region ** write header sheet 1**
                var ws = wb.Worksheets.Add(sheetName);

                var hdrTable = ws.Range("A1:V8");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A7:V8");

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText();

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                ws.Columns("1").Width = 20;
                ws.Columns("2").Width = 20;
                ws.Columns("3").Width = 10;
                ws.Columns("4").Width = 10;
                ws.Columns("5").Width = 10;
                ws.Columns("6").Width = 10;
                ws.Columns("7").Width = 10;
                ws.Columns("8").Width = 10;
                ws.Columns("9").Width = 10;
                ws.Columns("10").Width = 10;
                ws.Columns("11").Width = 10;
                ws.Columns("12").Width = 10;
                ws.Columns("13").Width = 10;
                ws.Columns("14").Width = 10;
                ws.Columns("15").Width = 10;
                ws.Columns("16").Width = 10;
                ws.Columns("17").Width = 10;
                ws.Columns("18").Width = 10;
                ws.Columns("19").Width = 10;
                ws.Columns("20").Width = 10;
                ws.Columns("21").Width = 10;
                ws.Columns("22").Width = 10;

                //First Names   
                ws.Cell("A1").Value = "ITS Reports";
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A2").Value = "Periode";
                ws.Cell("A3").Value = "Area";
                ws.Cell("A4").Value = "Dealer";
                ws.Cell("A5").Value = "Outlet";

                ws.Cell("B2").Value = dtAwal + " s/d " + dtAkhir;
                ws.Cell("B3").Value = Area;
                ws.Cell("B4").Value = DName;
                ws.Cell("B5").Value = OutletName;

                ws.Range("A7:A8").Merge();
                ws.Range("A7:A8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:A8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:A8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:A8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("A7").Value = "TYPE ";

                ws.Range("B7:B8").Merge();
                ws.Range("B7:B8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("B7:B8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("B7:B8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("B7:B8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("B7").Value = "VARIANT ";

                ws.Range("C7:G7").Merge();
                ws.Range("C7:G7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("C7:G7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("C7:G7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("C7:G7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("C7").Value = "NEW INQ ";

                ws.Range("H7:L7").Merge();
                ws.Range("H7:L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("H7:L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("H7:L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("H7:L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("H7").Value = "OUTSTANDING INQUIRY ";

                ws.Range("M7:Q7").Merge();
                ws.Range("M7:Q7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("M7:Q7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("M7:Q7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("M7:Q7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("M7").Value = "TOTAL ";

                ws.Range("R7:R8").Merge();
                ws.Range("R7:R8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("R7:R8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("R7:R8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("R7:R8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("R7").Value = "SOH ";

                ws.Range("S7:S8").Merge();
                ws.Range("S7:S8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("S7:S8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("S7:S8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("S7:S8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("S7").Value = "CANCEL ";

                ws.Range("T7:T8").Merge();
                ws.Range("T7:T8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("T7:T8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("T7:T9").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("T7:T8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("T7").Value = "LOST ";

                ws.Range("U7:U8").Merge();
                ws.Range("U7:U8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("U7:U8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("U7:U8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("U7:U8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("U7").Value = "TEST DRIVE ";

                ws.Range("V7:V8").Merge();
                ws.Range("V7:V8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("V7:V8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("V7:V8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("V7:V8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("V7").Value = "FAKTUR ";

                ws.Cell("C8").Value = "INQUIRY ";
                ws.Cell("C8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("C8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("C8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("C8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("D8").Value = "HP ";
                ws.Cell("D8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("D8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("D8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("D8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("E8").Value = "% ";
                ws.Cell("E8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("E8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("E8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("E8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("F8").Value = "SPK ";
                ws.Cell("F8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("F8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("F8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("F8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("G8").Value = "% ";
                ws.Cell("G8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("G8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("G8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("G8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("H8").Value = "INQUIRY ";
                ws.Cell("H8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("I8").Value = "HP ";
                ws.Cell("I8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("J8").Value = "% ";
                ws.Cell("J8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("K8").Value = "SPK ";
                ws.Cell("K8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("L8").Value = "% ";
                ws.Cell("L8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("M8").Value = "INQUIRY ";
                ws.Cell("M8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("N8").Value = "HP ";
                ws.Cell("N8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("N8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("N8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("N8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("O8").Value = "% ";
                ws.Cell("O8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("O8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("O8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("O8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("P8").Value = "SPK ";
                ws.Cell("P8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("P8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("P8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("P8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("Q8").Value = "% ";
                ws.Cell("Q8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                #endregion

                foreach (var row in ds.Tables[0].Rows)
                {
                    sheets = ((System.Data.DataRow)(row)).ItemArray[3].ToString();

                    
                    if (sheets != sheetName)
                    {
                        #region ** write total past page **
                        var lastIndex = rowIndex - 1;

                        prcntHPNow = (ttlNewHPINQ / (ttlNewINQ != 0 ? ttlNewINQ : ttlNewHPINQ != 0 ? ttlNewHPINQ : 1));
                        prcntHPLate = (ttlOutsHPINQ / (ttlOutsINQ != 0 ? ttlOutsINQ : ttlOutsHPINQ != 0 ? ttlOutsHPINQ : 1));
                        prcntHPTotal = (gtHP / (gtINQ != 0 ? gtINQ : gtHP != 0 ? gtHP : 1));

                        prcntNow = (ttlNewSPK / (ttlNewHPINQ != 0 ? ttlNewHPINQ : ttlNewSPK != 0 ? ttlNewSPK : 1));
                        prcntLate = (ttlOutsSPK / (ttlOutsINQ != 0 ? ttlOutsINQ : ttlOutsSPK != 0 ? ttlOutsSPK : 1));
                        prcntTotal = (gtSPK / (gtINQ != 0 ? gtINQ : gtSPK != 0 ? gtINQ : 1));

                        ws.Range("A" + (lastRow + lastIndex) + ":" + "B" + (lastRow + lastIndex)).Merge();
                        ws.Range("A" + (lastRow + lastIndex) + ":" + "B" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + lastIndex) + ":" + "B" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + lastIndex) + ":" + "B" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + lastIndex) + ":" + "B" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + lastIndex) + ":" + "B" + (lastRow + lastIndex)).Value = "TOTAL";
                        ws.Range("A" + (lastRow + lastIndex) + ":" + "B" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("A" + (lastRow + lastIndex) + ":" + "B" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Range("A" + (lastRow + lastIndex) + ":" + "B" + (lastRow + lastIndex)).Style.Font.Bold=true;

                        //New Inq INQUIRY
                        ws.Cell("C" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + (lastRow + lastIndex)).Value = ttlNewINQ;
                        ws.Cell("C" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("C" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("C" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //New Inq HP
                        ws.Cell("D" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + (lastRow + lastIndex)).Value = ttlNewHPINQ;
                        ws.Cell("D" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("D" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("D" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //New Inq %
                        ws.Cell("E" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + (lastRow + lastIndex)).Value = prcntHPNow;
                        ws.Cell("E" + (lastRow + lastIndex)).Style.NumberFormat.Format = "0.00%";
                        ws.Cell("E" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("E" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("E" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //New Inq SPK
                        ws.Cell("F" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + (lastRow + lastIndex)).Value = ttlNewSPK;
                        ws.Cell("F" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("F" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("F" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //New Inq %
                        ws.Cell("G" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + (lastRow + lastIndex)).Value = prcntNow;
                        ws.Cell("G" + (lastRow + lastIndex)).Style.NumberFormat.Format = "0.00%";
                        ws.Cell("G" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("G" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("G" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //Out Inq INQUIRY
                        ws.Cell("H" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + (lastRow + lastIndex)).Value = ttlOutsINQ;
                        ws.Cell("H" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("H" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("H" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //Out Inq HP
                        ws.Cell("I" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + (lastRow + lastIndex)).Value = ttlOutsHPINQ;
                        ws.Cell("I" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("I" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("I" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //Out Inq %
                        ws.Cell("J" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + (lastRow + lastIndex)).Value = prcntHPLate;
                        ws.Cell("J" + (lastRow + lastIndex)).Style.NumberFormat.Format = "0.00%";
                        ws.Cell("J" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("J" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("J" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //Out Inq SPK
                        ws.Cell("K" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + (lastRow + lastIndex)).Value = ttlOutsSPK;
                        ws.Cell("K" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("K" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("K" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //Out Inq %
                        ws.Cell("L" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + (lastRow + lastIndex)).Value = prcntLate;
                        ws.Cell("L" + (lastRow + lastIndex)).Style.NumberFormat.Format = "0.00%";
                        ws.Cell("L" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("L" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("L" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //Total inq INQUIRY
                        ws.Cell("M" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + (lastRow + lastIndex)).Value = gtINQ;
                        ws.Cell("M" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("M" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("M" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //Total inq HP
                        ws.Cell("N" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + (lastRow + lastIndex)).Value = gtHP;
                        ws.Cell("N" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("N" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("N" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //Total inq %
                        ws.Cell("O" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + (lastRow + lastIndex)).Value = prcntHPTotal;
                        ws.Cell("O" + (lastRow + lastIndex)).Style.NumberFormat.Format = "0.00%";
                        ws.Cell("O" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("O" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("O" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //Total inq SPK
                        ws.Cell("P" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + (lastRow + lastIndex)).Value = gtSPK;
                        ws.Cell("P" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("P" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("P" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //Total inq %
                        ws.Cell("Q" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + (lastRow + lastIndex)).Value = prcntTotal;
                        ws.Cell("Q" + (lastRow + lastIndex)).Style.NumberFormat.Format = "0.00%";
                        ws.Cell("Q" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("Q" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("Q" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //SOH
                        ws.Cell("R" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + (lastRow + lastIndex)).Value = ttlSOH;
                        ws.Cell("R" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("R" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("R" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //CANCEL
                        ws.Cell("S" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S" + (lastRow + lastIndex)).Value = ttlCancel;
                        ws.Cell("S" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("S" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("S" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //LOST
                        ws.Cell("T" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T" + (lastRow + lastIndex)).Value = ttlLost;
                        ws.Cell("T" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("T" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("T" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //TEST DRIVE
                        ws.Cell("U" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U" + (lastRow + lastIndex)).Value = ttlTD;
                        ws.Cell("U" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("U" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("U" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        //FAKTUR
                        ws.Cell("V" + (lastRow + lastIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("V" + (lastRow + lastIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("V" + (lastRow + lastIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("V" + (lastRow + lastIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("V" + (lastRow + lastIndex)).Value = ttlFP;
                        ws.Cell("V" + (lastRow + lastIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("V" + (lastRow + lastIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                        ws.Cell("V" + (lastRow + lastIndex)).Style.Font.Bold = true;

                        #endregion
                        #region ** write header sheet 1++**
                        lastRow = 9;
                        rowIndex = 0;
                        
                        ws = wb.Worksheets.Add(sheets);

                        hdrTable = ws.Range("A1:V8");
                        hdrTable.Style
                            .Font.SetBold()
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        rngTable = ws.Range("A7:V8");

                        rngTable.Style
                            .Font.SetBold()
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Alignment.SetWrapText();

                        rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        rngTable.Style.Font.Bold = true;

                        ws.Columns("1").Width = 20;
                        ws.Columns("2").Width = 20;
                        ws.Columns("3").Width = 20;
                        ws.Columns("4").Width = 20;
                        ws.Columns("5").Width = 20;
                        ws.Columns("6").Width = 20;
                        ws.Columns("7").Width = 20;
                        ws.Columns("8").Width = 20;
                        ws.Columns("9").Width = 20;
                        ws.Columns("10").Width = 20;
                        ws.Columns("11").Width = 20;
                        ws.Columns("12").Width = 20;
                        ws.Columns("13").Width = 20;
                        ws.Columns("14").Width = 20;
                        ws.Columns("15").Width = 20;
                        ws.Columns("16").Width = 20;
                        ws.Columns("17").Width = 20;
                        ws.Columns("18").Width = 20;
                        ws.Columns("19").Width = 20;
                        ws.Columns("20").Width = 20;
                        ws.Columns("21").Width = 20;
                        ws.Columns("22").Width = 20;

                        //First Names   
                        ws.Cell("A1").Value = "ITS Reports";
                        ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                        ws.Cell("A2").Value = "Periode";
                        ws.Cell("A3").Value = "Area";
                        ws.Cell("A4").Value = "Dealer";
                        ws.Cell("A5").Value = "Outlet";

                        ws.Cell("B2").Value = dtAwal + " s/d " + dtAkhir;
                        ws.Cell("B3").Value = Area;
                        ws.Cell("B4").Value = DName;
                        ws.Cell("B5").Value = OutletName;

                        ws.Range("A7:A8").Merge();
                        ws.Range("A7:A8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A7:A8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A7:A8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A7:A8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("A7").Value = "TYPE ";

                        ws.Range("B7:B8").Merge();
                        ws.Range("B7:B8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("B7:B8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("B7:B8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("B7:B8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B7").Value = "VARIANT ";

                        ws.Range("C7:G7").Merge();
                        ws.Range("C7:G7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("C7:G7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("C7:G7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("C7:G7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C7").Value = "NEW INQ ";

                        ws.Range("H7:L7").Merge();
                        ws.Range("H7:L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("H7:L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("H7:L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("H7:L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H7").Value = "OUTSTANDING INQUIRY ";

                        ws.Range("M7:Q7").Merge();
                        ws.Range("M7:Q7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("M7:Q7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("M7:Q7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("M7:Q7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M7").Value = "TOTAL ";

                        ws.Range("R7:R8").Merge();
                        ws.Range("R7:R8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("R7:R8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("R7:R8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("R7:R8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R7").Value = "SOH ";

                        ws.Range("S7:S8").Merge();
                        ws.Range("S7:S8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("S7:S8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("S7:S8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("S7:S8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S7").Value = "CANCEL ";

                        ws.Range("T7:T8").Merge();
                        ws.Range("T7:T8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("T7:T8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("T7:T9").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("T7:T8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T7").Value = "LOST ";

                        ws.Range("U7:U8").Merge();
                        ws.Range("U7:U8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("U7:U8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("U7:U8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("U7:U8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U7").Value = "TEST DRIVE ";

                        ws.Range("V7:V8").Merge();
                        ws.Range("V7:V8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("V7:V8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("V7:V8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("V7:V8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("V7").Value = "FAKTUR ";

                        ws.Cell("C8").Value = "INQUIRY ";
                        ws.Cell("C8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("D8").Value = "HP ";
                        ws.Cell("D8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("E8").Value = "% ";
                        ws.Cell("E8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("F8").Value = "SPK ";
                        ws.Cell("F8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("G8").Value = "% ";
                        ws.Cell("G8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("H8").Value = "INQUIRY ";
                        ws.Cell("H8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("I8").Value = "HP ";
                        ws.Cell("I8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("J8").Value = "% ";
                        ws.Cell("J8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("K8").Value = "SPK ";
                        ws.Cell("K8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("L8").Value = "% ";
                        ws.Cell("L8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("M8").Value = "INQUIRY ";
                        ws.Cell("M8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("N8").Value = "HP ";
                        ws.Cell("N8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("O8").Value = "% ";
                        ws.Cell("O8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("P8").Value = "SPK ";
                        ws.Cell("P8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("Q8").Value = "% ";
                        ws.Cell("Q8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        prcntNow = 0; prcntLate = 0; prcntTotal = 0; prcntHPNow = 0; prcntHPLate = 0; prcntHPTotal = 0;
                        ttlNewINQ = 0; ttlNewHPINQ = 0; ttlNewSPK = 0; ttlOutsINQ = 0; ttlOutsHPINQ = 0; ttlOutsSPK = 0;
                        gtINQ = 0; gtHP = 0; gtSPK = 0; ttlCancel = 0; ttlLost = 0; ttlTD = 0; ttlFP = 0; ttlSOH = 0;
                        ttlprcntNow = 0; ttlprcntLate = 0; ttlprcntTotal = 0; ttlprcntHPNow = 0; ttlprcntHPLate = 0; ttlprcntHPTotal = 0;

                        sheetName = sheets;

                        #endregion
                    }

                    #region ** fill values **
                    //TYPE
                    ws.Cell("A" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                    ws.Cell("A" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    //VARIANT
                    ws.Cell("B" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                    ws.Cell("B" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    //NEW INQ - INQUIRY
                    ws.Cell("C" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                    ws.Cell("C" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //NEW INQ - HP
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //NEW INQ - %
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Value = Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[9]) * 1 / 100;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.NumberFormat.Format = "0.00%";
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //NEW INQ - SPK
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //NEW INQ - %
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Value = Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[11]) * 1 / 100;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.NumberFormat.Format = "0.00%";
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //OUTSTANDING INQUIRY - INQUIRY
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //OUTSTANDING INQUIRY - HP
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //OUTSTANDING INQUIRY - %
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Value = Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[14]) * 1 / 100;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.NumberFormat.Format = "0.00%";
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //OUTSTANDING INQUIRY - SPK
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //OUTSTANDING INQUIRY - %
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Value = Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[16]) * 1 / 100;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.NumberFormat.Format = "0.00%";
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //TOTAL - INQUIRY
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //TOTAL - HP
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //TOTAL - %
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Value = Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[19]) * 1 / 100;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.NumberFormat.Format = "0.00%";
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //TOTAL - SPK
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //TOTAL - %
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Value = Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[21]) * 1 / 100;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.NumberFormat.Format = "0.00%";
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //SOH
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[25];
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //CANCEL
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[23];
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //LOST
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[22];

                    //TEST DRIVE
                    ws.Cell("U" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                    ws.Cell("U" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    //FAKTUR
                    ws.Cell("V" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[24];
                    ws.Cell("V" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                    ttlNewINQ += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[7]);
                    ttlNewHPINQ += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[8]);
                    ttlNewSPK += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[10]);
                    ttlOutsINQ += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[12]);
                    ttlOutsHPINQ += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[13]);
                    ttlOutsSPK += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[15]);
                    gtINQ += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[17]);
                    gtHP += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[18]);
                    gtSPK += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[20]);
                    ttlSOH += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[25]);
                    ttlCancel += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[23]);
                    ttlLost += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[22]);
                    ttlTD += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[6]);
                    ttlFP += Convert.ToDecimal(((System.Data.DataRow)(row)).ItemArray[24]);

                    rowIndex++;
                    #endregion
                }

                #region ** write total on last last page **
                prcntHPNow = (ttlNewHPINQ / (ttlNewINQ != 0 ? ttlNewINQ : ttlNewHPINQ != 0 ? ttlNewHPINQ : 1));
                prcntHPLate = (ttlOutsHPINQ / (ttlOutsINQ != 0 ? ttlOutsINQ : ttlOutsHPINQ != 0 ? ttlOutsHPINQ : 1));
                prcntHPTotal = (gtHP / (gtINQ != 0 ? gtINQ : gtHP != 0 ? gtHP : 1));

                prcntNow = (ttlNewSPK / (ttlNewHPINQ != 0 ? ttlNewHPINQ : ttlNewSPK != 0 ? ttlNewSPK : 1));
                prcntLate = (ttlOutsSPK / (ttlOutsINQ != 0 ? ttlOutsINQ : ttlOutsSPK != 0 ? ttlOutsSPK : 1));
                prcntTotal = (gtSPK / (gtINQ != 0 ? gtINQ : gtSPK != 0 ? gtINQ : 1));

                ws.Range("A" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex)).Merge();
                ws.Range("A" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex)).Value = "TOTAL";
                ws.Range("A" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Range("A" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Range("A" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //New Inq INQUIRY
                ws.Cell("C" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + (lastRow + rowIndex)).Value = ttlNewINQ;
                ws.Cell("C" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("C" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("C" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //New Inq HP
                ws.Cell("D" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + (lastRow + rowIndex)).Value = ttlNewHPINQ;
                ws.Cell("D" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("D" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("D" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //New Inq %
                ws.Cell("E" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + (lastRow + rowIndex)).Value = prcntHPNow;
                ws.Cell("E" + (lastRow + rowIndex)).Style.NumberFormat.Format = "0.00%";
                ws.Cell("E" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("E" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("E" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //New Inq SPK
                ws.Cell("F" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + (lastRow + rowIndex)).Value = ttlNewSPK;
                ws.Cell("F" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("F" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("F" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //New Inq %
                ws.Cell("G" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + (lastRow + rowIndex)).Value = prcntNow;
                ws.Cell("G" + (lastRow + rowIndex)).Style.NumberFormat.Format = "0.00%";
                ws.Cell("G" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("G" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("G" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //Out Inq INQUIRY
                ws.Cell("H" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + (lastRow + rowIndex)).Value = ttlOutsINQ;
                ws.Cell("H" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("H" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("H" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //Out Inq HP
                ws.Cell("I" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("I" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("I" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("I" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("I" + (lastRow + rowIndex)).Value = ttlOutsHPINQ;
                ws.Cell("I" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("I" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("I" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //Out Inq %
                ws.Cell("J" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("J" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("J" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("J" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("J" + (lastRow + rowIndex)).Value = prcntHPLate;
                ws.Cell("J" + (lastRow + rowIndex)).Style.NumberFormat.Format = "0.00%";
                ws.Cell("J" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("J" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("J" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //Out Inq SPK
                ws.Cell("K" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("K" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("K" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("K" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("K" + (lastRow + rowIndex)).Value = ttlOutsSPK;
                ws.Cell("K" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("K" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("K" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //Out Inq %
                ws.Cell("L" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("L" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("L" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("L" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("L" + (lastRow + rowIndex)).Value = prcntLate;
                ws.Cell("L" + (lastRow + rowIndex)).Style.NumberFormat.Format = "0.00%";
                ws.Cell("L" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("L" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("L" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //Total inq INQUIRY
                ws.Cell("M" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("M" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("M" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("M" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("M" + (lastRow + rowIndex)).Value = gtINQ;
                ws.Cell("M" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("M" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("M" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //Total inq HP
                ws.Cell("N" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("N" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("N" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("N" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("N" + (lastRow + rowIndex)).Value = gtHP;
                ws.Cell("N" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("N" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("N" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //Total inq %
                ws.Cell("O" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("O" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("O" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("O" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("O" + (lastRow + rowIndex)).Value = prcntHPTotal;
                ws.Cell("O" + (lastRow + rowIndex)).Style.NumberFormat.Format = "0.00%";
                ws.Cell("O" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("O" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("O" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //Total inq SPK
                ws.Cell("P" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("P" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("P" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("P" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("P" + (lastRow + rowIndex)).Value = gtSPK;
                ws.Cell("P" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("P" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("P" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //Total inq %
                ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q" + (lastRow + rowIndex)).Value = prcntTotal;
                ws.Cell("Q" + (lastRow + rowIndex)).Style.NumberFormat.Format = "0.00%";
                ws.Cell("Q" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("Q" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("Q" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //SOH
                ws.Cell("R" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("R" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("R" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("R" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("R" + (lastRow + rowIndex)).Value = ttlSOH;
                ws.Cell("R" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("R" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("R" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //CANCEL
                ws.Cell("S" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("S" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("S" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("S" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("S" + (lastRow + rowIndex)).Value = ttlCancel;
                ws.Cell("S" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("S" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("S" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //LOST
                ws.Cell("T" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("T" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("T" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("T" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("T" + (lastRow + rowIndex)).Value = ttlLost;
                ws.Cell("T" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("T" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("T" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //TEST DRIVE
                ws.Cell("U" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("U" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("U" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("U" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("U" + (lastRow + rowIndex)).Value = ttlTD;
                ws.Cell("U" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("U" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("U" + (lastRow + rowIndex)).Style.Font.Bold = true;

                //FAKTUR
                ws.Cell("V" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("V" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("V" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("V" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("V" + (lastRow + rowIndex)).Value = ttlFP;
                ws.Cell("V" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell("V" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = XLColor.LightYellow;
                ws.Cell("V" + (lastRow + rowIndex)).Style.Font.Bold = true;

                #endregion

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
            #endregion
        }

        public ActionResult inquiryItsMktgenexcell(string StartDate, string EndDate, string Area, string Dealer, string Outlet, string SpID, string DName, string OName)
        {
            string fileName = "";
            fileName = "MarketReports" + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            string dtAwal = (Convert.ToDateTime(StartDate)).ToString("dd-MMM-yyyy");
            string dtAkhir = (Convert.ToDateTime(EndDate)).ToString("dd-MMM-yyyy");
            string tipeKendaraan = "";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "EXEC " + SpID + " '" + StartDate + "','" + EndDate + "','" + Area + "','" + Dealer + "','" + Outlet + "'";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            DataTable dt1 = new DataTable();
            DataTable dt2 = new DataTable();

            dt1 = ds.Tables[0];
            dt2 = ds.Tables[1];


            int countRows = dt1.Rows.Count;

            if (dt1.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            #region *** Format Excel ***
            else
            {
                var wb = new XLWorkbook();

                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    int lastRow = 7;
                    decimal ttlOutsInq = 0, ttlNewINQ = 0, ttlOutsSPK = 0, ttlNewSPK = 0, ttlCancelSPK = 0, ttlFP = 0, ttlBalance = 0, ttlAT = 0, ttlMT = 0;
                    decimal gtOutsInq = 0, gtNewINQ = 0, gtOutsSPK = 0, gtNewSPK = 0, gtCancelSPK = 0, gtFP = 0, gtBalance = 0, gtAT = 0, gtMT = 0;

                    string sheetName = dt1.Rows[i][3].ToString();

                    var ws = wb.Worksheets.Add(sheetName);
                    var hdrTable = ws.Range("A1:K7");
                    hdrTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    var rngTable = ws.Range("A7:K7");

                    ws.Range("A7", "K7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("A7", "K7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("A7", "K7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("A7", "K7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                    rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText();

                    rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTable.Style.Font.Bold = true;

                    ws.Columns("1").Width = 25;
                    ws.Columns("2").Width = 25;
                    ws.Columns("3").Width = 15;
                    ws.Columns("4").Width = 15;
                    ws.Columns("5").Width = 15;
                    ws.Columns("6").Width = 15;
                    ws.Columns("7").Width = 15;
                    ws.Columns("8").Width = 15;
                    ws.Columns("9").Width = 15;
                    ws.Columns("10").Width = 15;
                    ws.Columns("11").Width = 15;

                    //First Names   
                    ws.Cell("A1").Value = "Market Reports";
                    ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                    ws.Cell("A2").Value = "Periode";
                    ws.Cell("A3").Value = "Area";
                    ws.Cell("A4").Value = "Dealer";
                    ws.Cell("A5").Value = "Outlet";

                    ws.Cell("B2").Value = dtAwal + " s/d " + dtAkhir;
                    ws.Cell("B3").Value = Area;
                    ws.Cell("B4").Value = DName;
                    ws.Cell("B5").Value = "PT. " + sheetName;

                    ws.Range("A7:A8").Merge();
                    ws.Range("A7:A8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("A7:A8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("A7:A8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("A7:A8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("A7:A8").Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                    ws.Range("B7:B8").Merge();
                    ws.Range("B7:B8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("B7:B8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("B7:B8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("B7:B8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("B7:B8").Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                    ws.Range("C7:C8").Merge();
                    ws.Range("C7:C8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("C7:C8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("C7:C8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("C7:C8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("C7:C8").Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                    ws.Range("D7:D8").Merge();
                    ws.Range("D7:D8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("D7:D8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("D7:D8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("D7:D8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("D7:D8").Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                    ws.Range("E7:E8").Merge();
                    ws.Range("E7:E8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("E7:E8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("E7:E8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("E7:E8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("E7:E8").Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                    ws.Range("F7:F8").Merge();
                    ws.Range("F7:F8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("F7:F8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("F7:F8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("F7:F8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("F7:F8").Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                    ws.Range("G7:G8").Merge();
                    ws.Range("G7:G8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("G7:G8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("G7:G8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("G7:G8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("G7:G8").Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                    ws.Range("H7:H8").Merge();
                    ws.Range("H7:H8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("H7:H8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("H7:H8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("H7:H8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("H7:H8").Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                    ws.Range("I7:I8").Merge();
                    ws.Range("I7:I8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("I7:I8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("I7:I8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("I7:I8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("I7:I8").Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                    ws.Range("J7:K7").Merge();
                    ws.Range("J7:K7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("J7:K7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("J7:K7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("J7:K7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("J7:K7").Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                    ws.Cell("J8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("J8").Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                    ws.Cell("K8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell("K8").Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                    ws.Cell("A7").Value = "TYPE";
                    ws.Cell("B7").Value = "VARIANT";
                    ws.Cell("C7").Value = "OUTS INQ";
                    ws.Cell("D7").Value = "NEW INQ";
                    ws.Cell("E7").Value = "OUTS SPK";
                    ws.Cell("F7").Value = "NEW SPK";
                    ws.Cell("G7").Value = "CANCEL SPK";
                    ws.Cell("H7").Value = "BALANCE";
                    ws.Cell("I7").Value = "FAKTUR";
                    ws.Cell("J7").Value = "TEST DRIVE";
                    ws.Cell("J8").Value = "MT ";
                    ws.Cell("K8").Value = "AT ";

                    lastRow = lastRow + 1;
                    lastRow++;

                    for (int j = 0; j < dt2.Rows.Count; j++)
                    {
                        var check = dt2.Rows[j][3].ToString();
                        if (check == sheetName)
                        {
                            if (tipeKendaraan != "" && tipeKendaraan != (dt2.Rows[j][4].ToString()))
                            {
                                //Total Tipe Kendaraan 
                                ws.Range("A" + lastRow + ":" + "B" + lastRow).Merge();
                                ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;
                                ws.Cell("A" + lastRow).Value = "Total " + tipeKendaraan;

                                //OUTS INQ
                                ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell("C" + lastRow).Value = ttlOutsInq + " ";
                                ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                ws.Cell("C" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                                //NEW INQ
                                ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell("D" + lastRow).Value = ttlNewINQ + " ";
                                ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                ws.Cell("D" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                                //OUTS SPK
                                ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell("E" + lastRow).Value = ttlOutsSPK + " ";
                                ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                ws.Cell("E" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                                //NEW SPK
                                ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell("F" + lastRow).Value = ttlNewSPK + " ";
                                ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                ws.Cell("F" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                                //CANCEL SPK
                                ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell("G" + lastRow).Value = ttlCancelSPK + " ";
                                ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                ws.Cell("G" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                                //BALANCE
                                ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell("H" + lastRow).Value = ttlBalance + " ";
                                ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                ws.Cell("H" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                                //FAKTUR
                                ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell("I" + lastRow).Value = ttlFP + " ";
                                ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                ws.Cell("I" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                                //MT 
                                ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell("J" + lastRow).Value = ttlMT + " ";
                                ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                ws.Cell("J" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                                //AT 
                                ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                ws.Cell("K" + lastRow).Value = ttlAT + " ";
                                ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                                ws.Cell("K" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                                ttlOutsInq = 0; ttlNewINQ = 0; ttlOutsSPK = 0; ttlNewSPK = 0; ttlCancelSPK = 0; ttlFP = 0; ttlBalance = 0; ttlAT = 0; ttlMT = 0;

                                lastRow++;
                                lastRow++;
                            }

                            //TYPE
                            ws.Cell("A" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("A" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("A" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("A" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("A" + lastRow).Value = " " + dt2.Rows[j][4].ToString();
                            ws.Cell("A" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            //VARIANT
                            ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("B" + lastRow).Value = " " + dt2.Rows[j][5].ToString();
                            ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                            //OUTS INQ
                            ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("C" + lastRow).Value = dt2.Rows[j][6].ToString() + " ";
                            ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            //NEW INQ
                            ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("D" + lastRow).Value = dt2.Rows[j][7].ToString() + " ";
                            ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            //OUTS SPK
                            ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("E" + lastRow).Value = dt2.Rows[j][8].ToString() + " ";
                            ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            //NEW SPK
                            ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("F" + lastRow).Value = dt2.Rows[j][9].ToString() + " ";
                            ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            //CANCEL SPK
                            ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("G" + lastRow).Value = dt2.Rows[j][10].ToString() + " ";
                            ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            //BALANCE
                            ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("H" + lastRow).Value = dt2.Rows[j][12].ToString() + " ";
                            ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            //FAKTUR
                            ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("I" + lastRow).Value = dt2.Rows[j][11].ToString() + " ";
                            ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            //MT 
                            ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("J" + lastRow).Value = dt2.Rows[j][13].ToString() + " ";
                            ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            //AT 
                            ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Cell("K" + lastRow).Value = dt2.Rows[j][14].ToString() + " ";
                            ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                            tipeKendaraan = dt2.Rows[j][4].ToString();
                            ttlOutsInq += Convert.ToDecimal(dt2.Rows[j][6].ToString());
                            ttlNewINQ += Convert.ToDecimal(dt2.Rows[j][7].ToString());
                            ttlOutsSPK += Convert.ToDecimal(dt2.Rows[j][8].ToString());
                            ttlNewSPK += Convert.ToDecimal(dt2.Rows[j][9].ToString());
                            ttlCancelSPK += Convert.ToDecimal(dt2.Rows[j][10].ToString());
                            ttlFP += Convert.ToDecimal(dt2.Rows[j][11].ToString());
                            ttlBalance += Convert.ToDecimal(dt2.Rows[j][12].ToString());
                            ttlAT += Convert.ToDecimal(dt2.Rows[j][13].ToString());
                            ttlMT += Convert.ToDecimal(dt2.Rows[j][14].ToString());

                            gtOutsInq += Convert.ToDecimal(dt2.Rows[j][6].ToString());
                            gtNewINQ += Convert.ToDecimal(dt2.Rows[j][7].ToString());
                            gtOutsSPK += Convert.ToDecimal(dt2.Rows[j][8].ToString());
                            gtNewSPK += Convert.ToDecimal(dt2.Rows[j][9].ToString());
                            gtCancelSPK += Convert.ToDecimal(dt2.Rows[j][10].ToString());
                            gtFP += Convert.ToDecimal(dt2.Rows[j][11].ToString());
                            gtBalance += Convert.ToDecimal(dt2.Rows[j][12].ToString());
                            gtAT += Convert.ToDecimal(dt2.Rows[j][13].ToString());
                            gtMT += Convert.ToDecimal(dt2.Rows[j][14].ToString());

                            lastRow++;
                        }
                    }

                    //Last Total Tipe Kendaraan 
                    ws.Range("A" + lastRow + ":" + "B" + lastRow).Merge();
                    ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;
                    ws.Cell("A" + lastRow).Value = "Total " + tipeKendaraan;

                    //OUTS INQ
                    ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Value = ttlOutsInq + " ";
                    ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("C" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                    //NEW INQ
                    ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Value = ttlNewINQ + " ";
                    ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("D" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                    //OUTS SPK
                    ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Value = ttlOutsSPK + " ";
                    ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("E" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                    //NEW SPK
                    ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Value = ttlNewSPK + " ";
                    ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("F" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                    //CANCEL SPK
                    ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Value = ttlCancelSPK + " ";
                    ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("G" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                    //BALANCE
                    ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Value = ttlBalance + " ";
                    ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("H" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                    //FAKTUR
                    ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Value = ttlFP + " ";
                    ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("I" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                    //MT 
                    ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Value = ttlMT + " ";
                    ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("J" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                    //AT 
                    ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Value = ttlAT + " ";
                    ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("K" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGoldenrodYellow;

                    lastRow++;

                    //GrangTotal 
                    ws.Range("A" + lastRow + ":" + "B" + lastRow).Merge();
                    ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Range("A" + lastRow + ":" + "B" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    ws.Cell("A" + lastRow).Value = "Grand Total " + sheetName;

                    //OUTS INQ
                    ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Value = gtOutsInq + " ";
                    ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("C" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGreen;

                    //NEW INQ
                    ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Value = gtNewINQ + " ";
                    ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("D" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGreen;

                    //OUTS SPK
                    ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Value = gtOutsSPK + " ";
                    ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("E" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGreen;

                    //NEW SPK
                    ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Value = gtNewSPK + " ";
                    ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("F" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGreen;

                    //CANCEL SPK
                    ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Value = gtCancelSPK + " ";
                    ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("G" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGreen;

                    //BALANCE
                    ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Value = gtBalance + " ";
                    ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("H" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGreen;

                    //FAKTUR
                    ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Value = gtFP + " ";
                    ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("I" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGreen;

                    //MT 
                    ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Value = gtMT + " ";
                    ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("J" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGreen;

                    //AT 
                    ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Value = gtAT + " ";
                    ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("K" + lastRow).Style.Fill.BackgroundColor = XLColor.LightGreen;
                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
            #endregion
        }

        public ActionResult inquiryItsStatusDSgenexcell(string StartDate, string EndDate, string Area, string Dealer, string Outlet, string SpID, string DName, string OName, string GroupModel, string TipeKendaraan, string Variant, int Type)
        {
            string fileName = "";
            fileName = "ITS Status Reports By Dealer (Summary)" + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            string OutletName = OName;

            if (OutletName == "" || OutletName == null)
            {
                OutletName = "All";
            }

            string dtAwal = (Convert.ToDateTime(StartDate)).ToString("dd-MMM-yyyy");
            string dtAkhir = (Convert.ToDateTime(EndDate)).ToString("dd-MMM-yyyy");
            string year = (Convert.ToDateTime(StartDate)).ToString("yyyy");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "EXEC " + SpID + " '" + StartDate + "','" + EndDate + "','" + Area + "','" + Dealer + "','" + Outlet + "','" + GroupModel + "','" + TipeKendaraan + "','" + Variant + "'," + Type + "";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            #region ** Format Excell **
            else
            {
                int lastRow = 9;
                int rowIndex = 0;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(year);
                var hdrTable = ws.Range("A1:N7");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A7:N7");

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText();

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                ws.Columns("1").Width = 20;
                ws.Columns("2").Width = 10;
                ws.Columns("3").Width = 20;
                ws.Columns("4").Width = 10;
                ws.Columns("5").Width = 10;
                ws.Columns("6").Width = 10;
                ws.Columns("7").Width = 10;
                ws.Columns("8").Width = 10;
                ws.Columns("9").Width = 10;
                ws.Columns("10").Width = 10;
                ws.Columns("11").Width = 10;
                ws.Columns("12").Width = 10;
                ws.Columns("13").Width = 10;
                ws.Columns("14").Width = 10;

                //First Names   
                ws.Cell("A1").Value = "ITS Status Reports By Dealer (Summary)";
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A2").Value = "Periode";
                ws.Cell("A3").Value = "Area";
                ws.Cell("A4").Value = "Dealer";
                ws.Cell("A5").Value = "Outlet";

                ws.Cell("B2").Value = dtAwal + " s/d " + dtAkhir;
                ws.Cell("B3").Value = Area;
                ws.Cell("B4").Value = DName;
                ws.Cell("B5").Value = OutletName;

                ws.Range("A7:A8").Merge();
                ws.Range("A7:A8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:A8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:A8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:A8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("B7:B8").Merge();
                ws.Range("B7:B8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("B7:B8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("B7:B8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("B7:B8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("C7:C8").Merge();
                ws.Range("C7:C8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("C7:C8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("C7:C8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("C7:C8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("D7:D8").Merge();
                ws.Range("D7:D8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("D7:D8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("D7:D8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("D7:D8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("E7:E8").Merge();
                ws.Range("E7:E8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("E7:E8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("E7:E8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("E7:E8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("F7:F8").Merge();
                ws.Range("F7:F8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("F7:F8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("F7:F8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("F7:F8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("G7:G8").Merge();
                ws.Range("G7:G8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("G7:G8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("G7:G8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("G7:G8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("H7:M7").Merge();
                ws.Range("H7:M7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("H7:M7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("H7:M7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("H7:M7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Range("N7:N8").Merge();
                ws.Range("N7:N8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("N7:N8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("N7:N8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("N7:N8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                ws.Cell("A7").Value = "Area ";
                ws.Cell("B7").Value = "Dealer ";
                ws.Cell("C7").Value = "Outlet ";
                ws.Cell("D7").Value = "Tipe ";
                ws.Cell("E7").Value = "Variant ";
                ws.Cell("F7").Value = "Status ";
                ws.Cell("G7").Value = "Saldo Awal ";
                ws.Cell("H7").Value = "Outstanding + New ";
                ws.Cell("N7").Value = "Total";

                ws.Cell("H8").Value = "1 ";
                ws.Cell("H8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("I8").Value = "2 ";
                ws.Cell("I8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("J8").Value = "3 ";
                ws.Cell("J8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("K8").Value = "4 ";
                ws.Cell("K8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("L8").Value = "5 ";
                ws.Cell("L8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("M8").Value = "6 ";
                ws.Cell("M8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                foreach (var row in ds.Tables[0].Rows)
                {
                    var x = XLColor.NoColor;
                    if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "0" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.LightSkyBlue;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.YellowGreen;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "1")
                    {
                        x = XLColor.LightBrown;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "0" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "1")
                    {
                        x = XLColor.TyrianPurple;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "1")
                    {
                        x = XLColor.LightSalmonPink;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.LightGray;
                    }
                    else
                    {
                        x = XLColor.White;
                    }

                    if (rowIndex % 6 == 0)
                    {
                        //Area
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Dealer 
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Outlet 
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Tipe 
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Variant 
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                    }

                    //Status 
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Saldo Awal 
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //1
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //2
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //3
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //4
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //5
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //6
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Total
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    rowIndex++;
                }


                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));

            }
            #endregion
        }

        public ActionResult inquiryItsStatusTSgenexcell(string StartDate, string EndDate, string Area, string Dealer, string Outlet, string SpID, string DName, string OName, string GroupModel, string TipeKendaraan, string Variant, int Type)
        {
            string fileName = "";
            fileName = "ITS Status Reports By Type (Summary)" + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            string OutletName = OName;

            if (OutletName == "" || OutletName == null)
            {
                OutletName = "All";
            }

            string dtAwal = (Convert.ToDateTime(StartDate)).ToString("dd-MMM-yyyy");
            string dtAkhir = (Convert.ToDateTime(EndDate)).ToString("dd-MMM-yyyy");
            string year = (Convert.ToDateTime(StartDate)).ToString("yyyy");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "EXEC " + SpID + " '" + StartDate + "','" + EndDate + "','" + Area + "','" + Dealer + "','" + Outlet + "','" + GroupModel + "','" + TipeKendaraan + "','" + Variant + "'," + Type + "";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);


            if (ds.Tables[0].Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            #region ** Format Excell **
            else
            {
                int lastRow = 9;
                int rowIndex = 0;

                var sheetName = "Sheet";
                var typeK = "";
                var ZTOTAL = "";

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(sheetName);
                foreach (var row in ds.Tables[0].Rows)
                {

                    typeK = ((System.Data.DataRow)(row)).ItemArray[10].ToString();

                    if (typeK == "") { typeK = "Unknowed"; }

                    var x = XLColor.NoColor;
                    if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "0" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.LightSkyBlue;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.YellowGreen;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "1")
                    {
                        x = XLColor.LightPink;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "1")
                    {
                        x = XLColor.LightPink;
                    }
                    else
                    {
                        x = XLColor.White;
                    }


                    #region ** create sheet **
                    if (typeK != sheetName)
                    {
                        lastRow = 9;
                        rowIndex = 0;
                        ws = wb.Worksheets.Add(typeK);
                        var hdrTable = ws.Range("A1:N7");
                        hdrTable.Style
                            .Font.SetBold()
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        var rngTable = ws.Range("A7:N7");

                        rngTable.Style
                            .Font.SetBold()
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Alignment.SetWrapText();

                        rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        rngTable.Style.Font.Bold = true;

                        ws.Columns("1").Width = 20;
                        ws.Columns("2").Width = 10;
                        ws.Columns("3").Width = 20;
                        ws.Columns("4").Width = 10;
                        ws.Columns("5").Width = 10;
                        ws.Columns("6").Width = 10;
                        ws.Columns("7").Width = 10;
                        ws.Columns("8").Width = 10;
                        ws.Columns("9").Width = 10;
                        ws.Columns("10").Width = 10;
                        ws.Columns("11").Width = 10;
                        ws.Columns("12").Width = 10;

                        //First Names   
                        ws.Cell("A1").Value = "ITS Status Reports By Type (Summary)";
                        ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                        ws.Cell("A2").Value = "Periode";
                        ws.Cell("A3").Value = "Area";
                        ws.Cell("A4").Value = "Dealer";
                        ws.Cell("A5").Value = "Outlet";

                        ws.Cell("B2").Value = dtAwal + " s/d " + dtAkhir;
                        ws.Cell("B3").Value = Area;
                        ws.Cell("B4").Value = DName;
                        ws.Cell("B5").Value = OutletName;

                        ws.Range("A7:A8").Merge();
                        ws.Range("A7:A8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A7:A8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A7:A8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A7:A8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Range("B7:B8").Merge();
                        ws.Range("B7:B8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("B7:B8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("B7:B8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("B7:B8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Range("C7:C8").Merge();
                        ws.Range("C7:C8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("C7:C8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("C7:C8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("C7:C8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Range("D7:D8").Merge();
                        ws.Range("D7:D8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("D7:D8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("D7:D8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("D7:D8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Range("E7:E8").Merge();
                        ws.Range("E7:E8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("E7:E8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("E7:E8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("E7:E8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Range("F7:K7").Merge();
                        ws.Range("F7:K7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("F7:K7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("F7:K7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("F7:K7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Range("L7:L8").Merge();
                        ws.Range("L7:L8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("L7:L8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("L7:L8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("L7:L8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("A7").Value = "Area ";
                        ws.Cell("B7").Value = "Dealer ";
                        ws.Cell("C7").Value = "Outlet ";
                        ws.Cell("D7").Value = "Status ";
                        ws.Cell("E7").Value = "Saldo Awal ";
                        ws.Cell("F7").Value = "Outstanding + New ";
                        ws.Cell("L7").Value = "Total ";

                        ws.Cell("F8").Value = "1 ";
                        ws.Cell("F8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("G8").Value = "2 ";
                        ws.Cell("G8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("H8").Value = "3 ";
                        ws.Cell("H8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("I8").Value = "4 ";
                        ws.Cell("I8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("J8").Value = "5 ";
                        ws.Cell("J8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("K8").Value = "6 ";
                        ws.Cell("K8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        sheetName = typeK;
                    }
                    #endregion

                    #region ** fill values **
                    if (rowIndex % 6 == 0)
                    {
                        ZTOTAL = ((System.Data.DataRow)(row)).ItemArray[7].ToString();
                        if (ZTOTAL == "ZTOTAL")
                        {
                            ZTOTAL = "";
                        }
                        //Area
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Dealer 
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Value = ZTOTAL;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Outlet 
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;
                    }

                    //Status 
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Saldo Awal 
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //1
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //2
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //3
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //4
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //5
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //6
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Total
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    rowIndex++;

                    #endregion
                }
                wb.Worksheets.Delete("Sheet");

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
            #endregion
        }

        public ActionResult inquiryItsStatusDDgenexcell(string StartDate, string EndDate, string Area, string Dealer, string Outlet, string SpID, string DName, string OName, string GroupModel, string TipeKendaraan, string Variant, int Type)
        {
            string fileName = "";
            fileName = "ITS Status Reports By Dealer (Detail)" + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            string OutletName = OName;

            if (OutletName == "" || OutletName == null)
            {
                OutletName = "All";
            }

            string dtAwal = (Convert.ToDateTime(StartDate)).ToString("dd-MMM-yyyy");
            string dtAkhir = (Convert.ToDateTime(EndDate)).ToString("dd-MMM-yyyy");
            string year = (Convert.ToDateTime(StartDate)).ToString("yyyy");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "EXEC " + SpID + " '" + StartDate + "','" + EndDate + "','" + Area + "','" + Dealer + "','" + Outlet + "','" + GroupModel + "','" + TipeKendaraan + "','" + Variant + "'," + Type + "";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            #region ** Format excell **
            else
            {
                int lastRow = 9;
                int rowIndex = 0;

                var sheetName = "Summary";
                var sheets = "";
                var x = XLColor.White;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(sheetName);

                #region ** write summary sheet **

                var hdrTable = ws.Range("A1:T7");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A7:T7");

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText();

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                ws.Columns("1").Width = 20;
                ws.Columns("2").Width = 10;
                ws.Columns("3").Width = 20;
                ws.Columns("4").Width = 10;
                ws.Columns("5").Width = 10;
                ws.Columns("6").Width = 10;
                ws.Columns("7").Width = 10;
                ws.Columns("8").Width = 10;
                ws.Columns("9").Width = 10;
                ws.Columns("10").Width = 10;
                ws.Columns("11").Width = 10;
                ws.Columns("12").Width = 10;
                ws.Columns("13").Width = 10;
                ws.Columns("14").Width = 10;
                ws.Columns("15").Width = 10;
                ws.Columns("16").Width = 10;
                ws.Columns("17").Width = 10;
                ws.Columns("18").Width = 10;
                ws.Columns("19").Width = 10;
                ws.Columns("20").Width = 10;

                #region ** write header **
                //First Names   
                ws.Cell("A1").Value = "ITS Status Reports By Dealer (Summary)";
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A2").Value = "Periode";
                ws.Cell("A3").Value = "Area";
                ws.Cell("A4").Value = "Dealer";
                ws.Cell("A5").Value = "Outlet";

                ws.Cell("B2").Value = dtAwal + " s/d " + dtAkhir;
                ws.Cell("B3").Value = Area;
                ws.Cell("B4").Value = DName;
                ws.Cell("B5").Value = OutletName;

                ws.Range("A7:A8").Merge();
                ws.Range("A7:A8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:A8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:A8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:A8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("A7").Value = "Area ";

                ws.Range("B7:B8").Merge();
                ws.Range("B7:B8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("B7:B8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("B7:B8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("B7:B8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("B7").Value = "Dealer ";

                ws.Range("C7:C8").Merge();
                ws.Range("C7:C8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("C7:C8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("C7:C8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("C7:C8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("C7").Value = "Outlet ";

                ws.Range("D7:D8").Merge();
                ws.Range("D7:D8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("D7:D8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("D7:D8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("D7:D8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("D7").Value = "Status ";

                ws.Range("E7:E8").Merge();
                ws.Range("E7:E8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("E7:E8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("E7:E8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("E7:E8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("E7").Value = "Saldo Awal ";

                ws.Range("F7:L7").Merge();
                ws.Range("F7:L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("F7:L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("F7:L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("F7:L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("F7").Value = "Outstanding";

                ws.Range("M7:S7").Merge();
                ws.Range("M7:S7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("M7:S7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("M7:S7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("M7:S7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("M7").Value = "New ";

                ws.Range("T7:T8").Merge();
                ws.Range("T7:T8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("T7:T8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("T7:T8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("T7:T8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("T7").Value = "Total";

                ws.Cell("F8").Value = "1 ";
                ws.Cell("F8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("F8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("F8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("F8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("F8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("G8").Value = "2 ";
                ws.Cell("G8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("G8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("G8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("G8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("G8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("H8").Value = "3 ";
                ws.Cell("H8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("I8").Value = "4 ";
                ws.Cell("I8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("J8").Value = "5 ";
                ws.Cell("J8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("K8").Value = "6 ";
                ws.Cell("K8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("L8").Value = "Sub Total ";
                ws.Cell("L8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("M8").Value = "1 ";
                ws.Cell("M8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("N8").Value = "2 ";
                ws.Cell("N8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("N8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("N8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("N8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("N8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("O8").Value = "3 ";
                ws.Cell("O8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("O8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("O8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("O8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("O8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("P8").Value = "4 ";
                ws.Cell("P8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("P8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("P8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("P8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("P8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("Q8").Value = "5 ";
                ws.Cell("Q8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("R8").Value = "6 ";
                ws.Cell("R8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("R8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("R8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("R8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("R8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("S8").Value = "Sub Total ";
                ws.Cell("S8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("S8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("S8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("S8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("S8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                #endregion

                foreach (var row in ds.Tables[0].Rows)
                {
                    if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "1")
                    {
                        x = XLColor.LightYellow;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "1")
                    {
                        x = XLColor.LightPink;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.LightGray;
                    }
                    else
                    {
                        x = XLColor.White;
                    }

                    #region ** fill values **
                    if (rowIndex % 6 == 0)
                    {
                        //Area
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Dealer 
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Outlet 
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;
                    }

                    //Status 
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Saldo Awal 
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //1
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //2
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //3
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //4
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //5
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //6
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Sub Total
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //1
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //2
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[21];
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //3
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[22];
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //4
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[23];
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //5
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[24];
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //6
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[25];
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Sub Total
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[26];
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Total
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[27];
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    rowIndex++;

                    #endregion

                }
                #endregion

                #region ** write BIT & Total Sheet **

                foreach (var row in ds.Tables[1].Rows)
                {
                    sheets = ((System.Data.DataRow)(row)).ItemArray[7].ToString();

                    if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "1")
                    {
                        x = XLColor.LightGray;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "0" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.LightBlue;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.LightYellow;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "1")
                    {
                        x = XLColor.LightBrown;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "1")
                    {
                        x = XLColor.LightPink;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.LightGreen;
                    }
                    else
                    {
                        x = XLColor.White;
                    }

                    #region ** Create Header BIT & Total Sheet **
                    if (sheets != sheetName)
                    {
                        lastRow = 9;
                        rowIndex = 0;
                        ws = wb.Worksheets.Add(sheets);
                        //var hdrTable = ws.Range("A1:V7");
                        hdrTable = ws.Range("A1:V7");
                        hdrTable.Style
                            .Font.SetBold()
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        //var rngTable = ws.Range("A7:V7");
                        rngTable = ws.Range("A7:V7");

                        rngTable.Style
                            .Font.SetBold()
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Alignment.SetWrapText();

                        rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        rngTable.Style.Font.Bold = true;

                        ws.Columns("1").Width = 20;
                        ws.Columns("2").Width = 10;
                        ws.Columns("3").Width = 20;
                        ws.Columns("4").Width = 10;
                        ws.Columns("5").Width = 10;
                        ws.Columns("6").Width = 10;
                        ws.Columns("7").Width = 10;
                        ws.Columns("8").Width = 10;
                        ws.Columns("9").Width = 10;
                        ws.Columns("10").Width = 10;
                        ws.Columns("11").Width = 10;
                        ws.Columns("12").Width = 10;
                        ws.Columns("13").Width = 10;
                        ws.Columns("14").Width = 10;
                        ws.Columns("15").Width = 10;
                        ws.Columns("16").Width = 10;
                        ws.Columns("17").Width = 10;
                        ws.Columns("18").Width = 10;
                        ws.Columns("19").Width = 10;
                        ws.Columns("20").Width = 10;
                        ws.Columns("21").Width = 10;
                        ws.Columns("22").Width = 10;

                        #region ** write header **
                        //First Names   
                        ws.Cell("A1").Value = "ITS Status Reports By Dealer (Detail)";
                        ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                        ws.Cell("A2").Value = "Periode";
                        ws.Cell("A3").Value = "Area";
                        ws.Cell("A4").Value = "Dealer";
                        ws.Cell("A5").Value = "Outlet";

                        ws.Cell("B2").Value = dtAwal + " s/d " + dtAkhir;
                        ws.Cell("B3").Value = Area;
                        ws.Cell("B4").Value = DName;
                        ws.Cell("B5").Value = OutletName;

                        ws.Range("A7:A8").Merge();
                        ws.Range("A7:A8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A7:A8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A7:A8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A7:A8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("A7").Value = "Area ";

                        ws.Range("B7:B8").Merge();
                        ws.Range("B7:B8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("B7:B8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("B7:B8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("B7:B8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B7").Value = "Dealer ";

                        ws.Range("C7:C8").Merge();
                        ws.Range("C7:C8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("C7:C8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("C7:C8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("C7:C8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C7").Value = "Outlet ";

                        ws.Range("D7:D8").Merge();
                        ws.Range("D7:D8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("D7:D8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("D7:D8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("D7:D8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D7").Value = "Type ";

                        ws.Range("E7:E8").Merge();
                        ws.Range("E7:E8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("E7:E8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("E7:E8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("E7:E8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E7").Value = "Variant ";

                        ws.Range("F7:F8").Merge();
                        ws.Range("F7:F8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("F7:F8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("F7:F8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("F7:F8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F7").Value = "Status ";

                        ws.Range("G7:G8").Merge();
                        ws.Range("G7:G8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("G7:G8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("G7:G8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("G7:G8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G7").Value = "Saldo Awal ";

                        ws.Range("H7:N7").Merge();
                        ws.Range("H7:N7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("H7:N7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("H7:N7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("H7:N7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H7").Value = "Outstanding ";

                        ws.Range("O7:U7").Merge();
                        ws.Range("O7:U7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("O7:U7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("O7:U7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("O7:U7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O7").Value = "New ";

                        ws.Range("V7:V8").Merge();
                        ws.Range("V7:V8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("V7:V8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("V7:V8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("V7:V8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("V7").Value = "Total ";

                        ws.Cell("H8").Value = "1 ";
                        ws.Cell("H8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("I8").Value = "2 ";
                        ws.Cell("I8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("J8").Value = "3 ";
                        ws.Cell("J8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("K8").Value = "4 ";
                        ws.Cell("K8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("L8").Value = "5 ";
                        ws.Cell("L8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("M8").Value = "6 ";
                        ws.Cell("M8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("N8").Value = "Sub Total ";
                        ws.Cell("N8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("O8").Value = "1 ";
                        ws.Cell("O8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("P8").Value = "2 ";
                        ws.Cell("P8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("Q8").Value = "3 ";
                        ws.Cell("Q8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("R8").Value = "4 ";
                        ws.Cell("R8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("S8").Value = "5 ";
                        ws.Cell("S8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("T8").Value = "6 ";
                        ws.Cell("T8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("U8").Value = "Sub Total ";
                        ws.Cell("U8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        sheetName = sheets;

                        #endregion

                    }
                    #endregion

                    #region ** fill values BIT & Total **
                    if (rowIndex % 6 == 0)
                    {
                        //Area
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Dealer 
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Outlet 
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Tipe 
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("D" + (lastRow + rowIndex) + ":" + "D" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Variant 
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("E" + (lastRow + rowIndex) + ":" + "E" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;
                    }

                    //Status 
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Saldo Awal 
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //1
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //2
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //3
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //4
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //5
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //6
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Sub Total
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[21];
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //1
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[22];
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //2
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[23];
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //3
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[24];
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //4
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[25];
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //5
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[26];
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //6
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[27];
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Sub Total
                    ws.Cell("U" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[28];
                    ws.Cell("U" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("U" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Total
                    ws.Cell("V" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[29];
                    ws.Cell("V" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("V" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    rowIndex++;
                    #endregion

                }

                #endregion

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }

            #endregion
        }

        public ActionResult inquiryItsStatusTDgenexcell(string StartDate, string EndDate, string Area, string Dealer, string Outlet, string SpID, string DName, string OName, string GroupModel, string TipeKendaraan, string Variant, int Type)
        {
            string fileName = "";
            fileName = "ITS Status Reports By Type (Detail)" + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            string OutletName = OName;

            if (OutletName == "" || OutletName == null)
            {
                OutletName = "All";
            }

            string dtAwal = (Convert.ToDateTime(StartDate)).ToString("dd-MMM-yyyy");
            string dtAkhir = (Convert.ToDateTime(EndDate)).ToString("dd-MMM-yyyy");
            string year = (Convert.ToDateTime(StartDate)).ToString("yyyy");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "EXEC " + SpID + " '" + StartDate + "','" + EndDate + "','" + Area + "','" + Dealer + "','" + Outlet + "','" + GroupModel + "','" + TipeKendaraan + "','" + Variant + "'," + Type + "";

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            #region ** Format Excell **
            else
            {
                int lastRow = 9;
                int rowIndex = 0;

                var sheetName = "Summary";
                var sheets = "";
                var ZTOTAL = "";
                var x = XLColor.White;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add(sheetName);

                #region ** writer header summary sheet**

                var hdrTable = ws.Range("A1:T7");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A7:T7");

                rngTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                        .Alignment.SetWrapText();

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                ws.Columns("1").Width = 20;
                ws.Columns("2").Width = 10;
                ws.Columns("3").Width = 20;
                ws.Columns("4").Width = 10;
                ws.Columns("5").Width = 10;
                ws.Columns("6").Width = 10;
                ws.Columns("7").Width = 10;
                ws.Columns("8").Width = 10;
                ws.Columns("9").Width = 10;
                ws.Columns("10").Width = 10;
                ws.Columns("11").Width = 10;
                ws.Columns("12").Width = 10;
                ws.Columns("13").Width = 10;
                ws.Columns("14").Width = 10;
                ws.Columns("15").Width = 10;
                ws.Columns("16").Width = 10;
                ws.Columns("17").Width = 10;
                ws.Columns("18").Width = 10;
                ws.Columns("19").Width = 10;
                ws.Columns("20").Width = 10;

                //First Names   
                ws.Cell("A1").Value = "ITS Status Reports By Type (Summary)";
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A2").Value = "Periode";
                ws.Cell("A3").Value = "Area";
                ws.Cell("A4").Value = "Dealer";
                ws.Cell("A5").Value = "Outlet";

                ws.Cell("B2").Value = dtAwal + " s/d " + dtAkhir;
                ws.Cell("B3").Value = Area;
                ws.Cell("B4").Value = DName;
                ws.Cell("B5").Value = OutletName;

                ws.Range("A7:A8").Merge();
                ws.Range("A7:A8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:A8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:A8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("A7:A8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("A7").Value = "Area ";

                ws.Range("B7:B8").Merge();
                ws.Range("B7:B8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("B7:B8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("B7:B8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("B7:B8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("B7").Value = "Dealer ";

                ws.Range("C7:C8").Merge();
                ws.Range("C7:C8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("C7:C8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("C7:C8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("C7:C8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("C7").Value = "Outlet ";

                ws.Range("D7:D8").Merge();
                ws.Range("D7:D8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("D7:D8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("D7:D8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("D7:D8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("D7").Value = "Status ";

                ws.Range("E7:E8").Merge();
                ws.Range("E7:E8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("E7:E8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("E7:E8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("E7:E8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("E7").Value = "Saldo Awal ";

                ws.Range("F7:L7").Merge();
                ws.Range("F7:L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("F7:L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("F7:L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("F7:L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("F7").Value = "Outstanding";

                ws.Range("M7:S7").Merge();
                ws.Range("M7:S7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("M7:S7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("M7:S7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("M7:S7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("M7").Value = "New ";

                ws.Range("T7:T8").Merge();
                ws.Range("T7:T8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Range("T7:T8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Range("T7:T8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Range("T7:T8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("T7").Value = "Total";

                ws.Cell("F8").Value = "1 ";
                ws.Cell("F8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("F8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("F8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("F8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("F8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("G8").Value = "2 ";
                ws.Cell("G8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("G8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("G8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("G8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("G8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("H8").Value = "3 ";
                ws.Cell("H8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("H8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("I8").Value = "4 ";
                ws.Cell("I8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("I8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("J8").Value = "5 ";
                ws.Cell("J8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("J8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("K8").Value = "6 ";
                ws.Cell("K8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("K8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("L8").Value = "Sub Total ";
                ws.Cell("L8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("L8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("M8").Value = "1 ";
                ws.Cell("M8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("M8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("N8").Value = "2 ";
                ws.Cell("N8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("N8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("N8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("N8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("N8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("O8").Value = "3 ";
                ws.Cell("O8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("O8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("O8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("O8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("O8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("P8").Value = "4 ";
                ws.Cell("P8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("P8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("P8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("P8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("P8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("Q8").Value = "5 ";
                ws.Cell("Q8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("Q8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("R8").Value = "6 ";
                ws.Cell("R8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("R8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("R8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("R8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("R8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                ws.Cell("S8").Value = "Sub Total ";
                ws.Cell("S8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                ws.Cell("S8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                ws.Cell("S8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                ws.Cell("S8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell("S8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                foreach (var row in ds.Tables[0].Rows)
                {

                    if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "2" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "2")
                    {
                        x = XLColor.LightPink;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "1")
                    {
                        x = XLColor.LightPink;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.LightYellow;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "0" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.LightBlue;
                    }
                    else
                    {
                        x = XLColor.White;
                    }

                    #region ** fill values summary **
                    if (rowIndex % 6 == 0)
                    {
                        ZTOTAL = ((System.Data.DataRow)(row)).ItemArray[7].ToString();
                        if (ZTOTAL == "ZTOTAL")
                        {
                            ZTOTAL = "";
                        }

                        //Area
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Dealer 
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Value = ZTOTAL;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Outlet 
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;
                    }

                    //Status 
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Saldo Awal 
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //1
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //2
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //3
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //4
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //5
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //6
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    // Sub Total
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //1
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //2
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[21];
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //3
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[22];
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //4
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[23];
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //5
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[24];
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //6
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[25];
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    // Sub Total
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[26];
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    // Sub Total
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[27];
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    rowIndex++;

                    #endregion

                }
                #endregion

                #region ** write header tipe kendaraan sheet **
                foreach (var row in ds.Tables[1].Rows)
                {
                    sheets = ((System.Data.DataRow)(row)).ItemArray[10].ToString();

                    if (sheets == "") { sheets = "Unknowed"; }

                    if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "0" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.LightBlue;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "0")
                    {
                        x = XLColor.LightYellow;
                    }
                    else if ((((System.Data.DataRow)(row)).ItemArray[0]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[1]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[2]).ToString() == "1" && (((System.Data.DataRow)(row)).ItemArray[3]).ToString() == "1")
                    {
                        x = XLColor.LightPink;
                    }
                    else
                    {
                        x = XLColor.White;
                    }

                    #region ** create header tipe kendaraan sheet **
                    if (sheets != sheetName)
                    {
                        lastRow = 9;
                        rowIndex = 0;
                        ws = wb.Worksheets.Add(sheets);
                        //var hdrTable = ws.Range("A1:V7");
                        hdrTable = ws.Range("A1:V7");
                        hdrTable.Style
                            .Font.SetBold()
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        //var rngTable = ws.Range("A7:V7");
                        rngTable = ws.Range("A7:V7");

                        rngTable.Style
                            .Font.SetBold()
                            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                            .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                            .Alignment.SetWrapText();

                        rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        rngTable.Style.Font.Bold = true;

                        ws.Columns("1").Width = 20;
                        ws.Columns("2").Width = 10;
                        ws.Columns("3").Width = 20;
                        ws.Columns("4").Width = 10;
                        ws.Columns("5").Width = 10;
                        ws.Columns("6").Width = 10;
                        ws.Columns("7").Width = 10;
                        ws.Columns("8").Width = 10;
                        ws.Columns("9").Width = 10;
                        ws.Columns("10").Width = 10;
                        ws.Columns("11").Width = 10;
                        ws.Columns("12").Width = 10;
                        ws.Columns("13").Width = 10;
                        ws.Columns("14").Width = 10;
                        ws.Columns("15").Width = 10;
                        ws.Columns("16").Width = 10;
                        ws.Columns("17").Width = 10;
                        ws.Columns("18").Width = 10;
                        ws.Columns("19").Width = 10;
                        ws.Columns("20").Width = 10;

                        //First Names   
                        ws.Cell("A1").Value = "ITS Status Reports By Type (Detail)";
                        ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                        ws.Cell("A2").Value = "Periode";
                        ws.Cell("A3").Value = "Area";
                        ws.Cell("A4").Value = "Dealer";
                        ws.Cell("A5").Value = "Outlet";
                        ws.Cell("A6").Value = "Tipe";

                        ws.Cell("B2").Value = dtAwal + " s/d " + dtAkhir;
                        ws.Cell("B3").Value = Area;
                        ws.Cell("B4").Value = DName;
                        ws.Cell("B5").Value = OutletName;
                        ws.Cell("B6").Value = sheets;

                        ws.Range("A7:A8").Merge();
                        ws.Range("A7:A8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A7:A8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A7:A8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A7:A8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("A7").Value = "Area ";

                        ws.Range("B7:B8").Merge();
                        ws.Range("B7:B8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("B7:B8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("B7:B8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("B7:B8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B7").Value = "Dealer ";

                        ws.Range("C7:C8").Merge();
                        ws.Range("C7:C8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("C7:C8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("C7:C8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("C7:C8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C7").Value = "Outlet ";

                        ws.Range("D7:D8").Merge();
                        ws.Range("D7:D8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("D7:D8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("D7:D8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("D7:D8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D7").Value = "Status ";

                        ws.Range("E7:E8").Merge();
                        ws.Range("E7:E8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("E7:E8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("E7:E8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("E7:E8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E7").Value = "Saldo Awal ";

                        ws.Range("F7:L7").Merge();
                        ws.Range("F7:L7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("F7:L7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("F7:L7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("F7:L7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F7").Value = "Outstanding ";

                        ws.Range("M7:S7").Merge();
                        ws.Range("M7:S7").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("M7:S7").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("M7:S7").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("M7:S7").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M7").Value = "New ";

                        ws.Range("T7:T8").Merge();
                        ws.Range("T7:T8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("T7:T8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("T7:T8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("T7:T8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T7").Value = "Total ";

                        ws.Cell("F8").Value = "1 ";
                        ws.Cell("F8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("G8").Value = "2 ";
                        ws.Cell("G8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("H8").Value = "3 ";
                        ws.Cell("H8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("I8").Value = "4 ";
                        ws.Cell("I8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("J8").Value = "5 ";
                        ws.Cell("J8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("K8").Value = "6 ";
                        ws.Cell("K8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("L8").Value = "Sub Total ";
                        ws.Cell("L8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("M8").Value = "1 ";
                        ws.Cell("M8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("N8").Value = "2 ";
                        ws.Cell("N8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("O8").Value = "3 ";
                        ws.Cell("O8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("P8").Value = "4 ";
                        ws.Cell("P8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("Q8").Value = "5 ";
                        ws.Cell("Q8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;

                        ws.Cell("R8").Value = "6 ";
                        ws.Cell("R8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("S8").Value = "Sub Total ";
                        ws.Cell("S8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("T8").Value = "Total ";
                        ws.Cell("T8").Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T8").Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T8").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T8").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T8").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        sheetName = sheets;
                    }


                    #endregion

                    #region ** fill values tipe kendaraan **
                    if (rowIndex % 6 == 0)
                    {
                        ZTOTAL = ((System.Data.DataRow)(row)).ItemArray[7].ToString();
                        if (ZTOTAL == "ZTOTAL")
                        {
                            ZTOTAL = "";
                        }

                        //Area
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("A" + (lastRow + rowIndex) + ":" + "A" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Dealer 
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Value = ZTOTAL;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("B" + (lastRow + rowIndex) + ":" + "B" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;

                        //Outlet 
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Merge();
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        ws.Range("C" + (lastRow + rowIndex) + ":" + "C" + (lastRow + rowIndex + 5)).Style.Fill.BackgroundColor = x;
                    }

                    //Status 
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("D" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Saldo Awal 
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("E" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //1
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("F" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //2
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("G" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //3
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("H" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //4
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("I" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //5
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("J" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //6
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("K" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Sub Total
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("L" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //1
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[21];
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("M" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //2
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[22];
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("N" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //3
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[23];
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("O" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //4
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[24];
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("P" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //5
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[25];
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("Q" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //6
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[26];
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("R" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Sub Total
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[27];
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("S" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    //Total
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + (lastRow + rowIndex)).Value = ((System.Data.DataRow)(row)).ItemArray[28];
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                    ws.Cell("T" + (lastRow + rowIndex)).Style.Fill.BackgroundColor = x;

                    rowIndex++;
                    #endregion
                }
                #endregion

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
            #endregion
        }

        public ActionResult GenerateITSWithStatusAndTestDrive(DateTime StartDate, DateTime EndDate)
        {
            DateTime now = DateTime.Now;
            string fileName = "ITS_Report_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var data = ctx.Database.SqlQuery<ItsWithStatusAndTestDrive>("exec uspfn_GenerateITSWithStatusAndTestDrive_Rev1 @StartDate=@p0, @EndDate=@p1", StartDate.ToString("yyyyMMdd"), EndDate.ToString("yyyyMMdd"));

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_GenerateITSWithStatusAndTestDrive_Rev1";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@StartDate", StartDate.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@EndDate", EndDate.ToString("yyyyMMdd"));

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt = ds.Tables[0];

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("ITSReport");

            //First Names
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer";
            ws.Cell("C" + recNo).Value = "Abbr";
            ws.Cell("D" + recNo).Value = "Outlet";
            ws.Cell("E" + recNo).Value = "OutletAbbr";
            ws.Cell("F" + recNo).Value = "Date";
            ws.Cell("G" + recNo).Value = "Model";
            ws.Cell("H" + recNo).Value = "Var";
            ws.Cell("I" + recNo).Value = "ColourName";
            ws.Cell("J" + recNo).Value = "Inq";
            ws.Cell("K" + recNo).Value = "InqTestDrive";
            ws.Cell("L" + recNo).Value = "HP";
            ws.Cell("M" + recNo).Value = "HPTestDrive";
            ws.Cell("N" + recNo).Value = "SPK";
            ws.Cell("O" + recNo).Value = "SPKTesDrive";
            ws.Cell("P" + recNo).Value = "LOST";

            recNo++;

            foreach (DataRow row in dt.Rows)
            {
                ws.Cell("A" + recNo).Value = row["Area"].ToString();
                ws.Cell("B" + recNo).Value = row["Dealer"].ToString();
                ws.Cell("C" + recNo).Value = row["Abbr"].ToString();
                ws.Cell("D" + recNo).Value = row["OutletCode"].ToString();
                ws.Cell("E" + recNo).Value = row["OutletAbbreviation"].ToString();
                ws.Cell("F" + recNo).Value = row["Date"].ToString();
                ws.Cell("G" + recNo).Value = row["Model"].ToString();
                ws.Cell("H" + recNo).Value = row["Var"].ToString();
                ws.Cell("I" + recNo).Value = row["ColourName"].ToString();
                ws.Cell("J" + recNo).Value = row["INQ"].ToString();
                ws.Cell("K" + recNo).Value = row["InqTestDrive"].ToString();
                ws.Cell("L" + recNo).Value = row["HP"].ToString();
                ws.Cell("M" + recNo).Value = row["HPTestDrive"].ToString();
                ws.Cell("N" + recNo).Value = row["SPK"].ToString();
                ws.Cell("O" + recNo).Value = row["SPKTestDrive"].ToString();
                ws.Cell("P" + recNo).Value = row["LOST"].ToString();
                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }
        
        public JsonResult PmExecSummary()
        {
            var BranchCode = Request.Params["BranchCode"];
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_GetTotalInquiryAndSpk";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("BranchCode", BranchCode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult PmExecSummary2()
        {
            var BranchCode = Request.Params["BranchCode"];
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_GetTotalInquiryAndSpk2";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("BranchCode", BranchCode);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }
        //public JsonResult Summary()
        //{
        //    SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
        //    cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_SummaryChartKDP";
        //    cmd.CommandType = CommandType.StoredProcedure;
        //    cmd.Parameters.Clear();

        //    cmd.Parameters.AddWithValue("@DateFrom", Request["DateFrom"]);
        //    cmd.Parameters.AddWithValue("@DateTo", Request["DateTo"]);
        //    cmd.Parameters.AddWithValue("@InqType", Request["SummaryType"]);

        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        SqlDataAdapter da = new SqlDataAdapter(cmd);
        //        da.Fill(ds);

        //        return Json(new { success = true, row = GetJsonRow(ds.Tables[0]), data = GetJson(ds.Tables[1]) }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}
    }
}
