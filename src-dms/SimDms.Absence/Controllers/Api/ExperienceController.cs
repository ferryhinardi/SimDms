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
    public class ExperienceController : BaseController
    {
        //
        public JsonResult GetWorkingExperinceData(HrEmployeeExperience model)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrGetEmpExperienceByEmployeeID";
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

        public int GetLatestExSeqNoByEmployeeID(string CompanyCode, string EmployeeID)
        {
            int lastSeq = 0;
            var entity = ctx.HrEmployeeExperiences.OrderByDescending(m => m.ExpSeq).FirstOrDefault();
            if (entity != null)
            {
                lastSeq = entity.ExpSeq + 1;
            }
            else
                lastSeq = 1;
            return lastSeq;
        }

        public JsonResult Delete(HrEmployeeExperience model)
        {
            object returnMsg = null; 
            var entity = ctx.HrEmployeeExperiences.Where(m=>m.CompanyCode == model.CompanyCode && m.EmployeeID == model.EmployeeID && m.ExpSeq == model.ExpSeq).FirstOrDefault();
            if (entity != null)
            {
                ctx.HrEmployeeExperiences.Remove(entity);
            }

            try
            {
                ctx.SaveChanges();
                returnMsg = new {success=true, message=""};
            }
            catch (Exception ex)
            {
                returnMsg = new {success = false, message=ex.InnerException.Message};
            }

            return Json(returnMsg);
        }

        public JsonResult Save(HrEmployeeExperience model)
        {
            object returnMsg = null;
            var entity = ctx.HrEmployeeExperiences.Where(m => m.CompanyCode == model.CompanyCode && m.EmployeeID == model.EmployeeID && m.ExpSeq == model.ExpSeq).FirstOrDefault();
            if (entity == null)
            {
                entity = model;
                entity.CompanyCode = model.CompanyCode;
                entity.EmployeeID = model.EmployeeID;
                entity.ExpSeq = GetLatestExSeqNoByEmployeeID(model.CompanyCode, model.EmployeeID);
                entity.CreatedBy = User.Identity.Name;
                entity.CreatedDate = DateTime.Now;
                ctx.HrEmployeeExperiences.Add(entity);
            }
            entity.UpdateBy = User.Identity.Name;
            entity.UpdateDate = DateTime.Now;
            entity.NameOfCompany = model.NameOfCompany;
            entity.LeaderName = model.LeaderName;
            entity.LeaderPhone = model.LeaderPhone;
            entity.LeaderHP = model.LeaderHP;
            entity.ReasonOfResign = model.ReasonOfResign;
            entity.ResignDate = model.ResignDate;

            try
            {
                ctx.SaveChanges();
                returnMsg = new {success=true};
            }
            catch (Exception ex)
            {
                returnMsg = new { success = false, message = ex.InnerException.Message };
            }
            return Json(returnMsg);
        }
    }
}
