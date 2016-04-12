//using SimDms.General.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using SimDms.General.Models.Others;
using SimDms.Common.Models;

namespace SimDms.General.Controllers.Api
{
    public class RoleModuleController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                IsAdmin = false,
                IsActive = true
            });
        }

        [HttpPost]
        public JsonResult Search(SysRoleMenu model)
        {
            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandText = "uspfn_GnSearchRoleMenu";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@RoleId", Request["RoleId"]);

            //try
            //{
            //    cmd.Connection.Open();
            //    cmd.ExecuteNonQuery();
            //    cmd.Connection.Close();

            return Json(new { success = true, data = "success" });
            //}
            //catch (Exception ex)
            //{
            //    return Json(new { success = false, message = ex.Message });
            //}
        }

        public JsonResult Save(SysRoleModuleModels model)
        {
            ResultModel result = InitializeResultModel();
            //var record = ctx.SysRoleModules.Find(model.RoleID, model.ModuleID);

            if (model.ModuleID != null)
            {
                foreach (var item in model.ModuleID)
                {
                    var data = ctx.SysRoleModules.Where(x => x.RoleID==model.RoleID && x.ModuleID==item).FirstOrDefault();
                    if (data == null)
                    {
                        data = new SysRoleModule();
                        data.RoleID = model.RoleID;
                        data.ModuleID = item;

                        ctx.SysRoleModules.Add(data);
                    }
                }
            }

            try
            {
                ctx.SaveChanges();
                result.status = true;
                result.message = "Module has been assigned.";
            }
            catch (Exception)
            {
                result.message = "Cannot assigning module into user.\nPlease, try again later!";
            }

            return Json(result);
        }

        public JsonResult Delete(SysRoleModule model)
        {
            ResultModel result = InitializeResultModel();

            var record = ctx.SysRoleModules.Find(model.RoleID, model.ModuleID);
            if (record != null)
            {
                ctx.SysRoleModules.Remove(record);

                try
                {
                    ctx.SaveChanges();
                    result.status = true;
                    result.message = "Module has been unsassigned from selected user.";
                }
                catch (Exception)
                {
                    result.message = "Cannot delete assigned module from seleced user.\nPlease, try again later!";
                }
            }

            return Json(result);
        }

        public JsonResult AssignModules(SysRoleModule[] models)
        {
            ResultModel result = InitializeResultModel();

            foreach (var item in models)
            {
                var data = ctx.SysRoleModules.Where(x => x.RoleID==item.RoleID && x.ModuleID==item.ModuleID).FirstOrDefault();

                if(data == null) {
                    data = new SysRoleModule();
                    data.ModuleID = item.ModuleID;
                    data.RoleID = item.RoleID;

                    ctx.SysRoleModules.Add(data);
                }
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Modules has been assigned to this user.";
            }
            catch (Exception)
            {
                result.message = "Sorry, modules cannot assigned to this role.";
            }

            return Json(result);
        }

        public JsonResult UnassignModules(SysRoleModuleModels models)
        {
            ResultModel result = InitializeResultModel();

            if (models.ModuleID != null)
            {
                foreach (var item in models.ModuleID)
                {
                    var data = ctx.SysRoleModules.Where(x => x.RoleID == models.RoleID && x.ModuleID == item).FirstOrDefault();

                    if (data != null)
                    {
                        ctx.SysRoleModules.Remove(data);
                    }
                }
            }

            try
            {
                ctx.SaveChanges();

                result.status = true;
                result.message = "Modules has been unassigned from this user.";
            }
            catch (Exception)
            {
                result.message = "Sorry, modules cannot be unassigned from this role.";
            }

            return Json(result);
        }
    }
}
