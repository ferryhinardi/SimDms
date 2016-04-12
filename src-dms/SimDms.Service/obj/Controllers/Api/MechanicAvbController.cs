using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;

namespace SimDms.Service.Controllers.Api
{
    public class MechanicAvbController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {

                CompanyCode = CompanyCode,
                //CompanyName = CompanyName,
                BranchCode = BranchCode,
                //BranchName = BranchName,
                //ServiceType = 2,
                //JobOrderDate = DateTime.Now,
                //StartService = DateTime.Now,
                //FinishService = DateTime.Now
            });
        }

        public JsonResult Save(SvMstMecAvb model)
        {
            var record = ctx.SvMstMecAvbs.Find(CompanyCode, BranchCode, model.EmployeeID);
            if (record == null)
            {
                record = new SvMstMecAvb
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    EmployeeID = model.EmployeeID,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                };
                ctx.SvMstMecAvbs.Add(record);
            }

            record.IsAvailable = model.IsAvailable;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            record.IsLocked = false;
            record.LockingBy = CurrentUser.UserId;
            record.LockingDate = DateTime.Now;

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

        public JsonResult UpdateAll()
        {
            
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_UpdateAvailableMechanics";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                cmd.Connection.Dispose();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult getMekanik()
        {
            var records = ctx.Employees.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && (new string[] { "2", "3", "8", "9" }).Contains(p.TitleCode) && p.PersonnelStatus == "1" ).ToList()
                .Select(p => new
                {
                    p.EmployeeID,
                    p.EmployeeName,
                    p.PersonnelStatus,
                    PersonnelStatusDesc = ctx.LookUpDtls.Where(p1 => p1.CompanyCode == CompanyCode && p1.CodeID == "PERS" && p1.LookUpValue == p.PersonnelStatus).FirstOrDefault().LookUpValueName
                });

            return Json(records.AsQueryable().toKG());
        }

        public JsonResult getEmpMekanik(SvMstMecAvb model)
        {
            var records = ctx.Employees.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && (new string[] { "2", "3", "8", "9" }).Contains(p.TitleCode)
                && p.PersonnelStatus == "1" && p.EmployeeID == model.EmployeeID).FirstOrDefault();
            if (records != null)
                return Json(new { success = true, data = records });
            else
                return Json(new { success = false });
        }

        public JsonResult Get(SvMstMecAvb model)
        {
            var record = ctx.SvMstMecAvbs.Find(CompanyCode, BranchCode, model.EmployeeID);
            if (record != null)
            {
                var record1 = ctx.Employees.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.EmployeeID == record.EmployeeID).FirstOrDefault().EmployeeName;
                return Json(new { success = true, data = record, employee = record1  });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        public JsonResult deleteData(SvMstMecAvb model)
        {
            var record = ctx.SvMstMecAvbs.Find(CompanyCode, BranchCode, model.EmployeeID);
            if (record != null)
            {
                ctx.SvMstMecAvbs.Remove(record);
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Proses Delete Stall Gagal!!!" + ex.Message });
            }
        }
    }
}
