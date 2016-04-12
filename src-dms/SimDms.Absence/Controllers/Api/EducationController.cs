using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using SimDms.Absence.Models;

namespace SimDms.Absence.Controllers.Api
{
    public class EducationController : BaseController
    {
        public ActionResult Get(HrEmployee model)
        {
            var entity = ctx.HrEmployees.Find(model.CompanyCode, model.EmployeeID);
            if (entity != null)
            {
                return Json(new { success = true, data = entity, isNew = false });
            }
            else
            {
                var data = ctx.HrEmployees.Find(model.CompanyCode, model.EmployeeID);
                return Json(new { success = true, data = data, isNew = true });
            }
        }

        public JsonResult GetEducationData(HrEmployee model)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrGetEducationByEmployeeId";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@EmployeeID", (string.IsNullOrWhiteSpace(model.EmployeeID) ? "" : model.EmployeeID)));
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            var list = GetJson(dt);


            if (list != null)
            {
                return Json(new { success = true, list = list });
            }
            else
            {
                return Json(new { success = false, message = "data not found" }); ;
            }
        }

        public int GetLatestSeqNoByEmployeeID(string CompanyCode, string EmployeeID)
        {
            int lastSeq = 0;
            var entity = ctx.HrEmployeeEducations.OrderByDescending(m => m.EduSeq).FirstOrDefault();
            if (entity != null)
            {
                lastSeq = entity.EduSeq + 1;
            }
            else
                lastSeq = 1;
            return lastSeq;
        }

        public JsonResult Save(HrEmployeeEducation model)
        {
            object returnMsg = null;
            var entity = ctx.HrEmployeeEducations.Where(m=>m.EduSeq == model.EduSeq && m.CompanyCode == model.CompanyCode && m.EmployeeID == model.EmployeeID).FirstOrDefault();
            if (entity == null)
            {
                entity = model;
                entity.EduSeq = GetLatestSeqNoByEmployeeID(model.CompanyCode, model.EmployeeID);
                entity.CreatedBy = User.Identity.Name;
                entity.CreatedDate = DateTime.Now;
                ctx.HrEmployeeEducations.Add(entity);
            }
            entity.Education = ctx.GnMstLookupDtls.Where(m=>m.LookUpValue == model.Education && m.CodeID == "FEDU").FirstOrDefault().LookupValueName;
            entity.YearFinish = model.YearFinish;
            entity.YearBegin = model.YearFinish;
            entity.College = model.College;
            entity.UpdateBy = User.Identity.Name;
            entity.UpdateDate = DateTime.Now;


            try
            {
                ctx.SaveChanges();
                returnMsg = new { success = true };
            }
            catch (Exception ex)
            {
                returnMsg = new { success = false, message = "Gagal menyimpan data" + ex.InnerException.Message };
            }
            return Json(returnMsg);
            
        }

        public JsonResult Delete(HrEmployeeEducation model)
        {
            object returnMsg = null;
            var entity = ctx.HrEmployeeEducations.Where(m => m.EduSeq == model.EduSeq && m.CompanyCode == model.CompanyCode && m.EmployeeID == model.EmployeeID).FirstOrDefault();
            if (entity != null)
                ctx.HrEmployeeEducations.Remove(entity);

            try
            {
                ctx.SaveChanges();
                returnMsg = new {success=true};
            }
            catch (Exception ex)
            {
                returnMsg = new {success=false, message="Gagal menghapus data"+ex.InnerException.Message};
            }
            return Json(returnMsg);
        }

    }
}
