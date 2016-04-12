using SimDms.Absence.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;

namespace SimDms.Absence.Controllers.Api
{
    public class GridController : BaseController
    {
        public JsonResult Holidays()
        {
            var queryable = ctx.Holidays;
            return Json(GeLang.DataTables<Holiday>.Parse(queryable, Request));
        }

        public JsonResult Shifts()
        {
            var queryable = ctx.Shifts;
            return Json(GeLang.DataTables<Shift>.Parse(queryable, Request));
            //return Json(queryable.KGrid());
        }

        public JsonResult ShiftsKendo()
        {
            var queryable = ctx.Shifts;
            //return Json(GeLang.DataTables<Shift>.Parse(queryable, Request));
            return Json(queryable.KGrid());
        }

        public JsonResult Lookups()
        {
            var queryable = ctx.LookUpDtls;
            return Json(GeLang.DataTables<LookUpDtl>.Parse(queryable, Request));
        }

        public JsonResult HrLookups()
        {
            var queryable = ctx.HrLookupViews.Where(p => p.CompanyCode == CompanyCode);
            //var queryable = ctx.GnMstLookupDtls.Where(p => p.CompanyCode == CompanyCode);
            string filter = Request["GroupFilter"];
            if (string.IsNullOrEmpty(filter) == false)
            {
                queryable = queryable.Where(p => p.CodeID == filter);
            }
            //return Json(GeLang.DataTables<HrLookupView>.Parse(queryable, Request));
            return Json(queryable.DGrid());
        }

        public JsonResult Attendance()
        {
            var queryable = ctx.HrTrnAttendanceFileHdrViews;
            foreach (var item in queryable)
            {
                item.Size = utility.CalculateSize(item.FileSize);
            }
            //return Json(GeLang.DataTables<SimDms.Absence.Models.HrTrnAttendanceFileHdrView>.Parse(queryable, Request));
            return Json(queryable.KGrid());
        }

        public JsonResult AttendanceDetails(string fileID)
        {
            var queryable = ctx.HrTrnAttendanceFileDtlViews.Where(x => x.FileID.Equals(fileID) == true);
            return Json(GeLang.DataTables<SimDms.Absence.Models.HrTrnAttendanceFileDtlView>.Parse(queryable, Request));
        }

        public JsonResult Employees()
        {
            IQueryable<HrEmployeeView> queryable = ctx.HrEmployeeViews.Where(x => x.CompanyCode != "" && x.EmployeeID != "");

            string department = Request["Department"];
            if (string.IsNullOrEmpty(department) == false)
            {
                queryable = queryable.Where(x => x.Department.Equals(department) == true);
            }

            return Json(GeLang.DataTables<SimDms.Absence.Models.HrEmployeeView>.Parse(queryable, Request));
        }

        //public JsonResult SalesDepartmentEmployees()
        //{
        //    IQueryable<HrEmployeeView> queryable = ctx.HrEmployeeViews.Where(x => x.CompanyCode != "" && x.EmployeeID != "");

        //    string department = Request["Department"];
        //    if (string.IsNullOrEmpty(department) == false)
        //    {
        //        queryable = queryable.Where(x => x.Department.Equals(department) == true);
        //    }

        //    return Json(GeLang.DataTables<SimDms.Absence.Models.HrEmployeeView>.Parse(queryable, Request));
        //}


        //public JsonResult ServiceDepartmentEmployees()
        //{
        //    IQueryable<HrEmployeeView> queryable = ctx.HrEmployeeViews.Where(x => x.CompanyCode != "" && x.EmployeeID != "");

        //    string department = Request["Department"];
        //    if (string.IsNullOrEmpty(department) == false)
        //    {
        //        queryable = queryable.Where(x => x.Department.Equals(department) == true);
        //    }

        //    return Json(GeLang.DataTables<SimDms.Absence.Models.HrEmployeeView>.Parse(queryable, Request));
        //}

        public JsonResult Subordinates()
        {
            string Department = Request["Department"];
            string TeamLeaderID = Request["TeamLeaderID"];

            IQueryable<HrEmployeeView> queryable = ctx.HrEmployeeViews.Where(x => x.CompanyCode == CompanyCode && x.TeamLeader == TeamLeaderID);
            return Json(GeLang.DataTables<SimDms.Absence.Models.HrEmployeeView>.Parse(queryable, Request));
        }

        public JsonResult Trainings()
        {
            return null;
        }

        public JsonResult Departments()
        {
            var queryable = ctx.OrgGroups.Where(p => p.CompanyCode == CompanyCode && p.OrgGroupCode == "DEPT");
            return Json(GeLang.DataTables<SimDms.Absence.Models.OrgGroup>.Parse(queryable, Request));
        }

        public JsonResult Positions()
        {
            var queryable = ctx.Positions.Where(p => p.CompanyCode == CompanyCode);
            return Json(GeLang.DataTables<SimDms.Absence.Models.Position>.Parse(queryable, Request));
        }

        public JsonResult EmployeeShifts()
        {
            string department = Request["Department"];
            string position = Request["Position"];
            string sDateFrom = DateTime.ParseExact((string.IsNullOrWhiteSpace(Request["DateFrom"]) ? "01-Jan-1900" : Request["DateFrom"]), "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyMMdd");
            string sDateTo = DateTime.ParseExact((string.IsNullOrWhiteSpace(Request["DateTo"]) ? (Request["DateFrom"] ?? "01-Jan-1900") : Request["DateTo"]), "dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyMMdd");
            string Shift = Request["Shift"] ?? "";
            bool isGrid = Convert.ToBoolean((Request["IsGrid"] ?? "false"));
                                                      
            var queryable = ctx.HrEmployeeShiftView.Where(p => p.CompanyCode == CompanyCode);
            queryable = queryable.Where(p => p.Department == department && string.Compare(sDateFrom, p.AttdDate) <= 0 && string.Compare(p.AttdDate, sDateTo) <= 0);
            if (!string.IsNullOrWhiteSpace(position)) { queryable = queryable.Where(p => p.Position == position); }
            if (!string.IsNullOrWhiteSpace(Shift))
            {
                if (Shift == "-")
                {
                    queryable = queryable.Where(p => p.ShiftCode == "");
                }
                else
                {
                    queryable = queryable.Where(p => p.ShiftCode == Shift);
                }
            }

            //return Json(GeLang.DataTables<HrEmployeeShiftView>.Parse(queryable, Request));

            if (isGrid)
            {
                return Json(queryable);
            }
            return Json(queryable.KGrid());
        }

        public JsonResult Users()
        {
            var queryable = from x in ctx.SysUsers
                            select new Users()
                            {
                                RelatedUser = x.UserId,
                                FullName = x.FullName
                            };
            return Json(GeLang.DataTables<SimDms.Absence.Models.Users>.Parse(queryable, Request));
        }
    }
}
