using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Absence.Models;
using SimDms.Absence.Models.Results;

namespace SimDms.Absence.Controllers.Api
{
    public class EmplShiftController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Search()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_AbMappingShiftInit";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@Department", Request["Department"]);
            cmd.Parameters.AddWithValue("@Position", Request["Position"]);
            cmd.Parameters.AddWithValue("@DateFrom", Request["DateFrom"]);
            cmd.Parameters.AddWithValue("@DateTo", Request["DateTo"]);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);

            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                return Json(new { success = true, data = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult Assign()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_AbAssignEmplShift";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@Department", Request["Department"]);
            cmd.Parameters.AddWithValue("@Position", Request["Position"]);
            cmd.Parameters.AddWithValue("@DateFrom", Request["DateFrom"]);
            cmd.Parameters.AddWithValue("@DateTo", Request["DateTo"]);
            cmd.Parameters.AddWithValue("@Shift", Request["Shift"]);
            cmd.Parameters.AddWithValue("@TargetShift", Request["TargetShift"]);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);

            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                return Json(new { success = true, data = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult UpdAssign(string ShiftEdit, string AttdDateEdit, string EmployeeIDEdit)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_AbUpdAssignEmplShift";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ShiftEdit", ShiftEdit);
            cmd.Parameters.AddWithValue("@AttdDateEdit", AttdDateEdit);
            cmd.Parameters.AddWithValue("@EmployeeIDEdit", EmployeeIDEdit);
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);

            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                return Json(new { success = true, data = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult UpdateShift(HrEmployeeShift model)
        {
            var record = ctx.HrEmployeeShifts.Find(CompanyCode, model.EmployeeID, model.AttdDate);
            if (record != null)
            {
                record.ClockInTime = model.ClockInTime;
                record.ClockOutTime = model.ClockOutTime;
                record.CalcOvertime = model.CalcOvertime ?? 0;
                record.ApprOvertime = model.ApprOvertime ?? 0;
                record.UpdatedBy = CurrentUser.UserId;
                record.UpdatedDate = DateTime.Now;
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult UpdateSingleShift()
        {
            ResultModel result = InitializeResult();

            string companyCode = Request["CompanyCode"] ?? "";
            string employeeID = Request["EmployeeID"] ?? "";
            string attdDate = Request["AttdDate"] ?? "";
            string targetShift = Request["TargetShift"] ?? "";

            var data = ctx.HrEmployeeShifts.Where(x => x.CompanyCode==companyCode && x.EmployeeID==employeeID && x.AttdDate==attdDate).FirstOrDefault();

            if (data != null && string.IsNullOrEmpty(targetShift) == false)
            {
                data.ShiftCode = targetShift;
            }

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Shift has been assigned.";
            }
            catch (Exception)
            {
                result.message = "Sorry, shift cannot be assigned to selected Employee.";
            }

            return Json(result);
        }
    }
}
